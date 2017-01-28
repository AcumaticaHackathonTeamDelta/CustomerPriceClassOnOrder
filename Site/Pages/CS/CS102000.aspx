<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CS102000.aspx.cs" Inherits="Page_CS102000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">

	<script language="javascript" type="text/javascript">
		function commandResult(ds, context) {
			if (context.command == "Save") __refreshMainMenu();
		}
	</script>

	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CS.BranchMaint" PrimaryView="BAccount">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="ValidateAddresses" Visible="True" CommitChanges="True" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand DependOnGrid="grdLocations" Name="SetDefault" Visible="False" />
			<px:PXDSCallbackCommand DependOnGrid="grdLocations" Name="ViewLocation" Visible="False" />
			<px:PXDSCallbackCommand Name="NewLocation" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewMainOnMap" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewDefLocationOnMap" Visible="false" />
			<px:PXDSCallbackCommand DependOnGrid="grdEmployees" Name="ViewContact" Visible="False" />
			<px:PXDSCallbackCommand Name="NewContact" Visible="False" />
		</CallbackCommands>
		<ClientEvents CommandPerformed="commandResult" />
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="PXFormView1" runat="server" Width="100%" Caption="Branch Summary" DataMember="BAccount" NoteIndicator="True" FilesIndicator="True" DefaultControlID="edAcctCD">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXSegmentMask ID="edAcctCD" runat="server" DataField="AcctCD" />
			<px:PXTextEdit CommitChanges="True" ID="edAcctName" runat="server" DataField="AcctName" />

			<px:PXLayoutRule runat="server" StartColumn="True" ColumnWidth="XM" />
			<px:PXCheckBox ID="chkActive" runat="server" DataField="Active" CommitChanges="true" AlignLeft="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="522px" Width="100%" DataMember="CurrentBAccount" DataSourceID="ds">
		<Items>
			<px:PXTabItem Text="General Info" RepaintOnDemand="false">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
					<px:PXFormView ID="DefContact" runat="server" Caption="Main Contact" DataMember="DefContact" RenderStyle="Fieldset" DataSourceID="ds">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
							<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
							<px:PXMailEdit ID="edEMail" runat="server" DataField="EMail" CommitChanges="True" />
							<px:PXLinkEdit ID="edWebSite" runat="server" DataField="WebSite" CommitChanges="True" />
							<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
							<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" />
							<px:PXMaskEdit ID="edFax" runat="server" DataField="Fax" />
						</Template>
					</px:PXFormView>
					<px:PXFormView ID="DefAddress" runat="server" Caption="Main Address" DataMember="DefAddress" RenderStyle="Fieldset" DataSourceID="ds" SyncPosition="true">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXCheckBox ID="edIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
							<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
							<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
							<px:PXSelector ID="edCountryID" runat="server" AllowEdit="True" CommitChanges="True" DataField="CountryID" DataSourceID="ds" />
							<px:PXSelector ID="edState" runat="server" AllowEdit="True" AutoRefresh="True" DataField="State" CommitChanges="True" DataSourceID="ds" />
							<px:PXLayoutRule runat="server" Merge="True" />
							<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" Size="s" CommitChanges="True" />
							<px:PXButton ID="btnViewMainOnMap" runat="server" CommandName="ViewMainOnMap" CommandSourceID="ds" Size="xs" Text="View on Map" />
							<px:PXLayoutRule runat="server" />

						</Template>
					</px:PXFormView>
					<px:PXLayoutRule runat="server" LabelsWidth="SM" StartColumn="True" ControlSize="XM" />
					<px:PXLayoutRule runat="server" GroupCaption="Legal Information" StartGroup="True" />
					<px:PXTextEdit ID="TaxRegistrationID" runat="server" DataField="TaxRegistrationID" />

					<px:PXFormView ID="PXFormViewLocation" runat="server" DataMember="IntLocations" RenderStyle="Simple" DataSourceID="ds">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule2" runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXTextEdit ID="edFullName" runat="server" DataField="CAvalaraExemptionNumber" />
							<px:PXDropDown ID="edSalutation" runat="server" DataField="CAvalaraCustomerUsageType" />
						</Template>
					</px:PXFormView>
					<px:PXCheckBox ID="edReporting1099" runat="server" DataField="Reporting1099" CommitChanges="True" />

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Configuration Settings" />
					<px:PXSelector ID="edBranchRolename" runat="server" AutoRefresh="True" DataField="BranchRoleName" />
					<px:PXSelector ID="edBranchCountryID" runat="server" AllowEdit="True" DataField="BranchCountryID" />
					<px:PXFormView ID="Company" runat="server" Caption="Base Currency Settings (Shared)" DataMember="Company" RenderStyle="Fieldset" DataSourceID="ds">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXSelector CommitChanges="True" ID="edBaseCuryID" runat="server" DataField="BaseCuryID" DataSourceID="ds" />
						</Template>
					</px:PXFormView>
					<px:PXFormView ID="CompanyCurrency" runat="server" DataMember="CompanyCurrency" RenderStyle="Simple" DataSourceID="ds">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
							<px:PXTextEdit ID="edCurySymbol" runat="server" DataField="CurySymbol" />
							<px:PXNumberEdit ID="edDecimalPlaces" runat="server" DataField="DecimalPlaces" />
						</Template>
					</px:PXFormView>
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Misc Settings (Shared)" />
					<px:PXFormView ID="CommonSettings" runat="server" DataMember="commonsetup" DataSourceID="ds" RenderStyle="Simple">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXNumberEdit ID="edDecPlQty" runat="server" DataField="DecPlQty" />
							<px:PXNumberEdit ID="edDecPlPrcCst" runat="server" DataField="DecPlPrcCst" />
						</Template>
					</px:PXFormView>
					<px:PXFormView ID="MiscSettings" runat="server" DataMember="Company" DataSourceID="ds" RenderStyle="Simple">
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
							<px:PXTextEdit ID="PhoneMask" runat="server" DataField="PhoneMask" />
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Delivery Settings">
				<Template>
					<px:PXFormView ID="frmDefLocation" runat="server" DataMember="DefLocation" Width="100%" CaptionVisible="False" TemplateContainer="" DataSourceID="ds" RenderStyle="Normal" SyncPosition="true">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Delivery Contact" StartGroup="True" />
							<px:PXCheckBox ID="chkIsContactSameAsMain" runat="server" CommitChanges="True" DataField="IsContactSameAsMain" SuppressLabel="True" />
							<px:PXFormView ID="DefLocationContact" runat="server" DataMember="DefLocationContact" RenderStyle="Simple" DataSourceID="ds">
								<Template>
									<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
									<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
									<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
									<px:PXMailEdit ID="edEMail" runat="server" DataField="EMail" CommitChanges="True" />
									<px:PXLinkEdit ID="edWebSite" runat="server" DataField="WebSite" CommitChanges="True" />
									<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
									<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" />
									<px:PXMaskEdit ID="edFax" runat="server" DataField="Fax" />
								</Template>
							</px:PXFormView>
							<px:PXLayoutRule runat="server" GroupCaption="Delivery Address" StartGroup="True" />
							<px:PXCheckBox ID="chkIsMain" runat="server" CommitChanges="True" DataField="IsAddressSameAsMain" SuppressLabel="True" />
							<px:PXCheckBox ID="edIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
							<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
							<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
							<px:PXSelector ID="edCountryID" runat="server" AllowEdit="True" CommitChanges="True" DataField="CountryID" DataSourceID="ds" />
							<px:PXSelector ID="edState" runat="server" AllowEdit="True" AutoRefresh="True" DataField="State" DataSourceID="ds" />
							<px:PXLayoutRule runat="server" Merge="True" />
							<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" Size="s" CommitChanges="True" />
							<px:PXButton ID="btnViewDefLoactionOnMap" runat="server" CommandName="ViewDefLocationOnMap" CommandSourceID="ds" Size="xs" Text="View on Map" />
							<px:PXLayoutRule runat="server" />
							<px:PXLayoutRule runat="server" LabelsWidth="SM" StartColumn="True" ControlSize="XM" />
							<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Shipping Instructions" />
							<px:PXSelector ID="edVTaxZoneID" runat="server" DataField="VTaxZoneID" AllowEdit="True" DataSourceID="ds" />
							<px:PXDropDown ID="edCShipComplete" runat="server" DataField="CShipComplete" SelectedIndex="2" />
						</Template>
						<Activity HighlightColor="" SelectedColor="" />
						<ContentStyle BackColor="Transparent" BorderStyle="None">
						</ContentStyle>
						<AutoSize Enabled="True" MinHeight="500" MinWidth="400" />
					</px:PXFormView>
				</Template>
				<ContentLayout ColumnsWidth="XL" LabelsWidth="M" />
			</px:PXTabItem>
			<px:PXTabItem Text="GL Accounts">
				<Template>
					<px:PXFormView ID="frmDefLocationGL" runat="server" DataMember="DefLocation" Width="100%" CaptionVisible="False" TemplateContainer="" DataSourceID="ds" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
							<px:PXSegmentMask ID="edCMPSalesSubID" runat="server" DataField="CMPSalesSubID" DataSourceID="ds" />
							<px:PXSegmentMask ID="edCMPExpenseSubID" runat="server" DataField="CMPExpenseSubID" DataSourceID="ds" />
							<px:PXSegmentMask ID="edCMPFreightSubID" runat="server" DataField="CMPFreightSubID" DataSourceID="ds" />
							<px:PXSegmentMask ID="edCMPDiscountSubID" runat="server" DataField="CMPDiscountSubID" DataSourceID="ds" />
							<px:PXSegmentMask ID="edCMPGainlossSubID" runat="server" DataField="CMPGainLossSubID" DataSourceID="ds" />
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Logo">
				<Template>
					<px:PXFormView ID="frmLogo" runat="server" DataMember="CurrentBAccount" Width="100%" CaptionVisible="False" TemplateContainer="" DataSourceID="ds" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
							<px:PXLabel ID="lblImgUploader" runat="server"> Branch Logo (Width:150px, Height:37px):</px:PXLabel>
							<px:PXImageUploader ID="imgUploader" runat="server" DataField="BranchLogoName" Height="120px" Width="400px" AllowUpload="True" />
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Employees">
				<Template>
					<px:PXGrid ID="grdEmployees" runat="server" Height="150px" Width="100%" ActionsPosition="Top" AllowSearch="True" SkinID="Details" DataSourceID="ds">
						<Mode AllowColMoving="False" AllowAddNew="False" AllowDelete="False" />
						<ActionBar DefaultAction="cmdViewContact">
							<CustomItems>
								<px:PXToolBarButton Text="New Employee">
									<AutoCallBack Command="NewContact" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Employee Details" Key="cmdViewContact">
									<AutoCallBack Command="ViewContact" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Employees" DataKeyNames="AcctCD">
								<RowTemplate>
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="Contact__IsActive" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn DataField="AcctCD" Width="80px" LinkCommand="ViewContact" />
									<px:PXGridColumn DataField="Contact__DisplayName" Width="150px" />
									<px:PXGridColumn DataField="DepartmentID" Width="80px" />
									<px:PXGridColumn DataField="Address__City" Width="80px" />
									<px:PXGridColumn DataField="Address__State" Width="80px" />
									<px:PXGridColumn DataField="Contact__Phone1" Width="120px" />
									<px:PXGridColumn DataField="Contact__EMail" Width="120px" />
									<px:PXGridColumn DataField="Status" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="1099 Settings" BindingContext="tab" VisibleExp="DataControls[&quot;edReporting1099&quot;].Value == true">
				<Template>
					<px:PXLayoutRule runat="server" ID="Rule7" GroupCaption="E-Filing Settings" StartRow="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXTextEdit runat="server" ID="edTCC1" DataField="TCC" />
					<px:PXCheckBox runat="server" ID="edCFSFiler2" DataField="CFSFiler" Text="CFSFiler" />
					<px:PXCheckBox runat="server" ID="edForeignEntity3" DataField="ForeignEntity" Text="ForeignEntity" />
					<px:PXTextEdit runat="server" ID="edContactName4" DataField="ContactName" />
					<px:PXTextEdit runat="server" ID="edCTelNumber5" DataField="CTelNumber" />
					<px:PXTextEdit runat="server" ID="edCEmail6" DataField="CEmail" />
					<px:PXTextEdit runat="server" ID="edNameControl1" DataField="NameControl" />
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="ROT & RUT Settings">
				<Template>
					<px:PXFormView runat="server" DataSourceID="ds" SkinID="Transparent" DataMember="CurrentBAccount" ID="RUTROTSettings">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
							<px:PXCheckBox runat="server" DataField="AllowsRUTROT" CommitChanges="True" ID="edRUTROTFlag" />
							<px:PXSelector runat="server" DataField="RUTROTCuryID" ID="edRUTROTCury" CommitChanges="true" Size="s" />
							<px:PXNumberEdit runat="server" DataField="RUTROTClaimNextRefNbr" ID="edRUTROTNextRef" CommitChanges="True" />
							<px:PXTextEdit runat="server" DataField="RUTROTOrgNbrValidRegEx" ID="edRUTROTOrgNbrRegEx" />

							<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="XM" GroupCaption="ROT Settings" />
							<px:PXNumberEdit runat="server" DataField="ROTPersonalAllowanceLimit" ID="edROTAllowance" />
							<px:PXNumberEdit runat="server" DataField="ROTExtraAllowanceLimit" ID="edROTAllowanceExtra" />
							<px:PXNumberEdit runat="server" DataField="ROTDeductionPct" ID="edROTPercent" />

							<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="XM" GroupCaption="RUT Settings" />
							<px:PXNumberEdit runat="server" DataField="RUTPersonalAllowanceLimit" ID="edRUTAllowance" />
							<px:PXNumberEdit runat="server" DataField="RUTExtraAllowanceLimit" ID="edRUTAllowanceExtra" />
							<px:PXNumberEdit runat="server" DataField="RUTDeductionPct" ID="edRUTPercent" />

						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="533" MinWidth="600" />
	</px:PXTab>
</asp:Content>
