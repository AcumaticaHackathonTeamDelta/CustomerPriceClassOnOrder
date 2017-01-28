<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM203010.aspx.cs" Inherits="Page_SM201020"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <pxa:DynamicDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True"
		Width="100%" PrimaryView="UserProfile" TypeName="PX.SM.MyProfileMaint,PX.SM.SMAccessPersonalMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="SaveUsers" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="changePassword" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="GetCalendarSyncUrl" Visible="false" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="resetTimeZone" Visible="false" />
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="SiteMap" TreeKeys="NodeID" />
		</DataTrees>
	</pxa:DynamicDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <script type="text/javascript">
    	function btnResetTimeZone_Click(sender, args) {
    		var findTimeZoneElem = function (container) {
    			if (container.childNodes)
    				for (var i = 0; i < container.childNodes.length; i++) {
    					var child = container.childNodes[i];
    					if (child && child.getAttribute && String.compare(child.getAttribute("timeZone"), "1", true)) return child;
    					var innerTable = findTimeZoneElem(child);
    					if (innerTable) return innerTable;
    				}
    		}
    		var tzElem = findTimeZoneElem(document.body);
    		//if (tzElem) alert(tzElem);
    	}
	</script>
	<px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="400px" Style="z-index: 100"
		Width="100%" DataMember="UserProfile" Caption="Edit your profile" OnDataBound="tab_DataBound"
		OnInit="tab_Init" RepaintOnDemand="False">
		<Items>
			<px:PXTabItem Text="General Info">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" ControlSize="XL" LabelsWidth="M">
					</px:PXLayoutRule>
					<px:PXControlParam Type="String" PropertyName='NewDataKey["Username"]' Name="Username"	ControlID="tab" />
					<px:PXTextEdit ID="edUsername" runat="server"  DataField="Username" />
					<px:PXTextEdit ID="edFirstName"	runat="server" DataField="FirstName" />
					<px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" />
					<px:PXMaskEdit ID="edPhone"	runat="server" DataField="Phone" />
					<px:PXMailEdit ID="edEmail" runat="server" DataField="Email" />
					<px:PXButton Style="left: 162px; position: absolute; top: 144px" ID="btnChangePassword"
						runat="server" Width="110px" Height="20px" Text="Change password" PopupPanel="pnlResetPassword">
					</px:PXButton>
					<px:PXSmartPanel ID="pnlResetPassword" runat="server" Caption="Change password"
						LoadOnDemand="True" Position="UnderOwner" Width="400px" AutoReload="True">
						<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" ControlSize="M" LabelsWidth="SM">
						</px:PXLayoutRule>
						<px:PXTextEdit ID="edOldPassword" runat="server" DataField="OldPassword" TextMode="Password" Required="True" />
						<px:PXTextEdit ID="edNewPassword" runat="server" DataField="NewPassword" TextMode="Password" Required="True" />
						<px:PXTextEdit ID="edConfirmPassword" runat="server" DataField="ConfirmPassword" TextMode="Password" Required="True" />
						<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
							<px:PXButton ID="btnOk" runat="server" DialogResult="OK" Text="OK">
								<AutoCallBack Command="changePassword" Enabled="True" Target="ds" />
							</px:PXButton>
							<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
						</px:PXPanel>
					</px:PXSmartPanel>
					<px:PXTextEdit ID="edPasswordQuestion" runat="server" DataField="PasswordQuestion" />
					<px:PXTextEdit ID="edPasswordAnswer" runat="server" DataField="PasswordAnswer" />
					<px:PXTextEdit ID="edComment" runat="server" DataField="Comment" TextMode="MultiLine" />
					<px:PXLabel ID="PXHole" runat="server"></px:PXLabel>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Personal Settings">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"   />
					<px:PXFormView ID="formUserPrefs" runat="server" DataMember="UserPrefs" RenderStyle="Simple">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" 
								ControlSize="XL"/>
							<px:PXSelector ID="edPdfCertificateName" runat="server" DataField="PdfCertificateName" />
							<px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
							<px:PXDropDown ID="edTimeZone" runat="server" DataField="TimeZone" />
							<px:PXButton Size="m" ID="btnResetTimeZone" runat="server" Height="20px" Text="Reset"
								ToolTip="Reset To Calendar Time Zone" CommandSourceID="ds" CommandName="resetTimeZone"
								Hidden="True">
								<ClientEvents Click="btnResetTimeZone_Click" />
							</px:PXButton>
							<px:PXLayoutRule ID="PXLayoutRule3" runat="server" />
							<px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
							<px:PXDropDown Size="XM" ID="edEditorFont" runat="server" DataField="EditorFontName"
								AllowNull="False" />
							<px:PXDropDown SuppressLabel="True" Size="XS" ID="edEditorFontSize" runat="server"
								DataField="EditorFontSize" AllowNull="False" />
							<px:PXLayoutRule ID="PXLayoutRule5" runat="server" />
							<px:PXSelector ID="edBranchCD" runat="server" DataField="DefBranchID" ValueField="BranchID"
								TextField="BranchCD">
								<GridProperties FastFilterFields="BranchCD">
								</GridProperties>
							</px:PXSelector>
							<px:PXTreeSelector ID="edHomePage" runat="server" DataField="HomePage" PopulateOnDemand="True"
								TreeDataMember="SiteMap" TreeDataSourceID="ds" InitialExpandLevel="0" ShowRootNode="False">
								<DataBindings>
									<px:PXTreeItemBinding DataMember="SiteMap" TextField="Title" ValueField="NodeID" ToolTipField="TitleWithPath"
										ImageUrlField="Icon" />
								</DataBindings>
							</px:PXTreeSelector>
							<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartGroup="True" GroupCaption="Export to Excel" />
							<px:PXCheckBox ID="chkHiddenSkip" runat="server" DataField="HiddenSkip" />
							<px:PXCheckBox ID="chkBorder" runat="server" DataField="Border" />
							<px:PXDropDown Size="M" ID="edBorderColor" runat="server" 
								DataField="BorderColor" />
							<px:PXLayoutRule ID="PXLayoutRule7" runat="server" ControlSize="M" LabelsWidth="SM" StartRow="True" />
							<px:PXLayoutRule ID="PXLayoutRule8" runat="server" StartGroup="True" GroupCaption="Header" />
							<px:PXDropDown ID="edHeaderFont" runat="server" DataField="HeaderFont" />
							<px:PXDropDown ID="edHeaderFontSize" runat="server" DataField="HeaderFontSize" />
							<px:PXDropDown ID="edHeaderFontColor" runat="server" DataField="HeaderFontColor" />
							<px:PXDropDown ID="edHeaderFontType" runat="server" DataField="HeaderFontType" />
							<px:PXDropDown ID="edHeaderFillColor" runat="server" DataField="HeaderFillColor" />
							<px:PXLayoutRule ID="PXLayoutRule9" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
							<px:PXLayoutRule ID="PXLayoutRule10" runat="server" StartGroup="True" GroupCaption="Body" />
							<px:PXDropDown ID="edBodyFont" runat="server" DataField="BodyFont" />
							<px:PXDropDown ID="edBodyFontSize" runat="server" DataField="BodyFontSize" />
							<px:PXDropDown ID="edBodyFontColor" runat="server" DataField="BodyFontColor" />
							<px:PXDropDown ID="edBodyFontType" runat="server" DataField="BodyFontType" />
							<px:PXDropDown ID="edBodyFillColor" runat="server" DataField="BodyFillColor" />
						</Template>
					</px:PXFormView>					
				</Template>
			</px:PXTabItem>
		</Items>	
		<AutoSize Container="Window" Enabled="True" MinHeight="100" MinWidth="300" />
	</px:PXTab>
</asp:Content>

