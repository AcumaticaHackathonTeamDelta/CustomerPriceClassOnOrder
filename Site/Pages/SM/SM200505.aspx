<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false"
	 Title="Untitled Page" CodeFile="SM200505.aspx.cs" Inherits="Page_SM200505" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" 
		PrimaryView="Prefs" TypeName="PX.SM.PreferencesGeneralMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="Articles" TreeKeys="PageID" />
			<px:PXTreeDataMember TreeView="SiteMap" TreeKeys="NodeID" />
            <px:PXTreeDataMember TreeView="ArticlesForHelp" TreeKeys="PageID" />
		</DataTrees>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Caption="General Settings"
		Style="z-index: 100" Width="100%" DataMember="Prefs" TemplateContainer="" >
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" 
				ColumnSpan="2" ControlSize="XL" LabelsWidth="M" />
			<px:PXLayoutRule runat="server" StartGroup="True" 
				GroupCaption="General Defaults" />
				
                <px:PXTreeSelector ID="edHomePage" runat="server" DataField="HomePage" PopulateOnDemand="True"
					TreeDataMember="SiteMap" TreeDataSourceID="ds" InitialExpandLevel="0" ShowRootNode="False">
					<DataBindings>
						<px:PXTreeItemBinding DataMember="SiteMap" TextField="Title" ValueField="NodeID" ToolTipField="TitleWithPath"
							ImageUrlField="Icon" />
					</DataBindings>
				</px:PXTreeSelector>

                <px:PXTreeSelector ID="edHelponHelp" runat="server" DataField="HelpPage" PopulateOnDemand="True"
					TreeDataMember="ArticlesForHelp" TreeDataSourceID="ds" InitialExpandLevel="0" ShowRootNode="False" SyncPositionWithGraph="True" CommitChanges="true">
					<DataBindings>
						<px:PXTreeItemBinding DataMember="ArticlesForHelp" TextField="Title" ValueField="PageID" ToolTipField="TitleWithPath"
							ImageUrlField="Icon" />
					</DataBindings>
				</px:PXTreeSelector>

				<px:PXDropDown ID="edMapViewer" runat="server" DataField="MapViewer" 
				Size="XL" > </px:PXDropDown>
				<px:PXCheckBox SuppressLabel="False" ID="chkGridActionsText" runat="server" 
				DataField="GridActionsText" AlignLeft="False"> </px:PXCheckBox>
				<px:PXDropDown ID="edTimeZone" runat="server" DataField="TimeZone" 
				Size="XL" />
				<px:PXDropDown ID="edTheme" runat="server" AllowNull="False" 
				DataField="Theme" Size="XL" />
				<px:PXTreeSelector CommitChanges="True" ID="edGetLinkTemplate" 
				runat="server"  PopulateOnDemand="True" ShowRootNode="False" 
				InitialExpandLevel="0" TreeDataSourceID="ds" DataField="GetLinkTemplate" 
				TreeDataMember="Articles">
					<DataBindings>
						<px:PXTreeItemBinding TextField="Title" ValueField="PageID" />
					</DataBindings>
				</px:PXTreeSelector>
            <px:PXTextEdit ID="edPortalExternalAccessLink" runat="server" DataField="PortalExternalAccessLink" 
				Size="XL" />
			<px:PXLayoutRule runat="server" StartGroup="True" 
				GroupCaption="Editor Settings" />
			<px:PXLayoutRule runat="server" Merge="True" />
				<px:PXDropDown Size="XM" ID="edEditorFont" runat="server" 
				DataField="EditorFontName" AllowNull="False" />
				<px:PXDropDown Size="XS" ID="edEditorFontSize" runat="server"
				 DataField="EditorFontSize" AllowNull="False" SuppressLabel="True"   />
			<px:PXLayoutRule runat="server" />
				<px:PXCheckbox ID="edSpellCheck" runat="server" DataField="SpellCheck" />
			<px:PXLayoutRule runat="server" StartGroup="True" 
				GroupCaption="Export to Excel" />
				<px:PXCheckBox SuppressLabel="False" ID="chkBorder" runat="server" 
				DataField="Border" AlignLeft="False"> </px:PXCheckBox>
				<px:PXDropDown ID="edBorderColor" runat="server" DataField="BorderColor"  ColorMode="True" ColorBox="True"
				Size="XL" > </px:PXDropDown>
				<px:PXCheckBox SuppressLabel="False" ID="chkHiddenSkip" runat="server" 
				DataField="HiddenSkip" AlignLeft="False"> </px:PXCheckBox>
			 
				<px:PXLayoutRule runat="server" StartRow="True" GroupCaption="Header" 
				StartGroup="True" ControlSize="M" LabelsWidth="SM">
			</px:PXLayoutRule>
			<px:PXDropDown ID="edHeaderFont" runat="server" DataField="HeaderFont" Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edHeaderFontSize" runat="server" DataField="HeaderFontSize" 
				Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edHeaderFontColor" runat="server"  ColorMode="True"
				DataField="HeaderFontColor" Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edHeaderFontType" runat="server" DataField="HeaderFontType" 
				Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edHeaderFillColor" runat="server" 
				DataField="HeaderFillColor" Size="M">
			</px:PXDropDown>
			<px:PXLayoutRule runat="server" GroupCaption="Body" StartColumn="True" 
				StartGroup="True" ControlSize="M" LabelsWidth="SM">
			</px:PXLayoutRule>
			<px:PXDropDown ID="edBodyFont" runat="server" DataField="BodyFont" Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edBodyFontSize" runat="server" DataField="BodyFontSize" 
				Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edBodyFontColor" runat="server" DataField="BodyFontColor" 
				Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edBodyFontType" runat="server" DataField="BodyFontType" 
				Size="M">
			</px:PXDropDown>
			<px:PXDropDown ID="edBodyFillColor" runat="server" DataField="BodyFillColor" 
				Size="M">
			</px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
