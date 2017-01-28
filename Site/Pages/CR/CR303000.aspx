<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR303000.aspx.cs" Inherits="Page_CR303000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<script type="text/javascript">
		function refreshTasksAndEvents(ds, context)
		{
			if (context.command == "Cancel")
			{
				var top = window.top;
				if (top != window && top.MainFrame != null) top.MainFrame.refreshEventsInfo();
			}
		}
	</script>
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.BusinessAccountMaint"
		PrimaryView="BAccount">
		<ClientEvents CommandPerformed="refreshTasksAndEvents" />
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" PopupVisible="true" />
			<px:PXDSCallbackCommand Name="First" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Action" StartNewGroup="true" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Merge" Visible="False" CommitChanges="True"/>
            <px:PXDSCallbackCommand Name="MarkAsValidated" Visible="False" />
            <px:PXDSCallbackCommand Name="CheckForDuplicates"  Visible="False" />
			<px:PXDSCallbackCommand DependOnGrid="grdContacts" Name="Contacts_ViewDetails" Visible="False" />
			<px:PXDSCallbackCommand Name="AddContact" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand DependOnGrid="grdLocations" Name="Locations_ViewDetails"
				Visible="False" />
			<px:PXDSCallbackCommand Name="AddLocation" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand DependOnGrid="grdLocations" Name="SetDefaultLocation" Visible="False"
				RepaintControlsIDs="grdLocations" />
			<px:PXDSCallbackCommand Name="AddOpportunity" CommitChanges="True" Visible="False" />
			<px:PXDSCallbackCommand Name="Opportunities_ViewDetails" Visible="False" DependOnGrid="gridOpportunities" />
            <px:PXDSCallbackCommand Name="Opportunities_BAccount_ViewDetails" Visible="False" DependOnGrid="gridOpportunities" />
			<px:PXDSCallbackCommand Name="Opportunities_Contact_ViewDetails" Visible="False" DependOnGrid="gridOpportunities" />
			<px:PXDSCallbackCommand Name="AddCase" CommitChanges="True" Visible="False" />
			<px:PXDSCallbackCommand Name="Cases_ViewDetails" Visible="False" DependOnGrid="gridCases" />
			<px:PXDSCallbackCommand DependOnGrid="grdContracts" Name="Contracts_ViewDetails"
				Visible="False" />
			<px:PXDSCallbackCommand DependOnGrid="grdContracts" Name="Contracts_Location_ViewDetails"
				Visible="False" />
			<px:PXDSCallbackCommand Name="Contracts_BAccount_ViewDetails" Visible="False" DependOnGrid="grdContracts"/>
			<px:PXDSCallbackCommand DependOnGrid="grdOrders" Name="Orders_ViewDetails" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewVendor" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewCustomer" Visible="False" />
			<px:PXDSCallbackCommand Name="ConverToCustomer" Visible="False" />
			<px:PXDSCallbackCommand Name="ConverToVendor" Visible="False" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" DependOnGrid="grdContacts" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ViewActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="OpenActivityOwner" Visible="False" CommitChanges="True" DependOnGrid="gridActivities" />
			<px:PXDSCallbackCommand Name="ViewDefLocationOnMap" CommitChanges="True" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewMainOnMap" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="ValidateAddresses" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="Members_CRCampaign_ViewDetails" Visible="False" CommitChanges="True"
				DependOnGrid="grdCampaignHistory" />
			<px:PXDSCallbackCommand Name="Subscriptions_CRMarketingList_ViewDetails" Visible="False"
				CommitChanges="True" DependOnGrid="grdMarketingLists" />
			<px:PXDSCallbackCommand Name="Relations_EntityDetails" Visible="False" CommitChanges="True"
				DependOnGrid="grdRelations" />
			<px:PXDSCallbackCommand Name="Relations_ContactDetails" Visible="False" CommitChanges="True"
				DependOnGrid="grdRelations" />
            <px:PXDSCallbackCommand Visible="false" DependOnGrid="PXGridDuplicates" Name="Duplicates_Contact_ViewDetails" />
			<px:PXDSCallbackCommand Visible="false" DependOnGrid="PXGridDuplicates" Name="ViewDuplicateAccount" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXSmartPanel ID="pnlChangeID" runat="server"  Caption="Specify New ID"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="ChangeIDDialog" CreateOnDemand="false" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formChangeID" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOK">
            <px:PXFormView ID="formChangeID" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
                DataMember="ChangeIDDialog">
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule ID="rlAcctCD" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXSegmentMask ID="edAcctCD" runat="server" DataField="CD" />
                </Template>
            </px:PXFormView>
            <px:PXPanel ID="pnlChangeIDButton" runat="server" SkinID="Buttons">
                <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK" >
                    <AutoCallBack Target="formChangeID" Command="Save" />
                </px:PXButton>
				<px:PXButton ID="PXButton3" runat="server" DialogResult="Cancel" Text="Cancel" />						
            </px:PXPanel>
    </px:PXSmartPanel>
	<px:PXFormView ID="form" runat="server" Width="100%" Caption="Account Summary"
		DataMember="BAccount" DataSourceID="ds" NoteIndicator="True" LinkIndicator="True"
		NotifyIndicator="True" FilesIndicator="True" DefaultControlID="edAcctCD">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM"
				ControlSize="XM" />
			<px:PXSegmentMask ID="edAcctCD" runat="server" DataField="AcctCD" AutoRefresh="True"
				FilterByAllFields="True" />
			<px:PXTextEdit CommitChanges="True" ID="edAcctName" runat="server" DataField="AcctName" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" Size="S" />
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM"
				ControlSize="XM" />
			<px:PXSelector CommitChanges="True" ID="edWorkgroupID" runat="server" DataField="WorkgroupID"
				TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
			<px:PXSelector ID="OwnerID" runat="server" DataField="OwnerID" TextMode="Search"
				DisplayMode="Text" FilterByAllFields="True" />
			<px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" Size="SM" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%" DataMember="CurrentBAccount">
		<Items>
			<px:PXTabItem Text="Details" RepaintOnDemand="false">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" ControlSize="XM"
						LabelsWidth="SM" />
					<px:PXLayoutRule ID="PXLayoutRule4" runat="server" GroupCaption="Main Contact" StartGroup="True" />
					<px:PXFormView ID="frmDefContact" runat="server" CaptionVisible="False" DataMember="DefContact"
						DataSourceID="ds" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartColumn="True" LabelsWidth="SM"
								ControlSize="XM" />
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXLabel ID="PXLabel1" runat="server" Size="SM" Text="Attention"></px:PXLabel>
							<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" SuppressLabel="true"/>
                            <px:PXLayoutRule runat="server"></px:PXLayoutRule>
							<px:PXMailEdit ID="edEMail" runat="server" DataField="EMail" CommandName="NewMailActivity"
								CommandSourceID="ds" CommitChanges="True"/>
							<px:PXLinkEdit ID="edWebSite" runat="server" DataField="WebSite" CommitChanges="True"/>
                            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
                            <px:PXLabel ID="LPhone1" runat="server" Size="SM" />
                            <px:PXDropDown ID="Phone1Type" runat="server" DataField="Phone1Type" Size="XS" SuppressLabel="True" CommitChanges="True" />
					        <px:PXMaskEdit ID="Phone1" runat="server" DataField="Phone1" Width="164px" LabelID="LPhone1" />
                            <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" />
                            <px:PXLabel ID="LPhone2" runat="server" Size="SM" />
                            <px:PXDropDown ID="Phone2Type" runat="server" DataField="Phone2Type" Size="XS" SuppressLabel="True" CommitChanges="True"/>
					        <px:PXMaskEdit ID="Phone2" runat="server" DataField="Phone2" Width="164px" LabelID="LPhone2" />
                            <px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
                            <px:PXLabel ID="LFax" runat="server" Size="SM" />
                            <px:PXDropDown ID="FaxType" runat="server" DataField="FaxType" Size="XS" SuppressLabel="True" CommitChanges="True" />
					        <px:PXMaskEdit ID="Fax" runat="server" DataField="Fax" Width="164px" LabelID="LFax"/>
							<px:PXLayoutRule ID="PXLayoutRule23" runat="server" />
                            <px:PXCheckBox ID="edDuplicateFoundFrm" runat="server" DataField="DuplicateFound" Visible="False" />                            
						</Template>
						<ContentLayout OuterSpacing="None" />
						<ContentStyle BackColor="Transparent" BorderStyle="None">
						</ContentStyle>
					</px:PXFormView>
					<px:PXLayoutRule ID="PXLayoutRule6" runat="server" GroupCaption="Main Address" StartGroup="True" />
					<px:PXFormView ID="frmDefAddress" runat="server" CaptionVisible="False" DataSourceID="ds"
						DataMember="AddressCurrent" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="SM"
								ControlSize="XM" />
							<px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
							<px:PXLayoutRule ID="PXLayoutRule18" runat="server" />
							<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
							<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
							<px:PXSelector ID="edCountryID" runat="server" AllowEdit="True" DataField="CountryID"
								FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" CommitChanges="True" />
							<px:PXSelector ID="edState" runat="server" AutoRefresh="True" DataField="State" CommitChanges="true"
								FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" />
							<px:PXLayoutRule ID="PXLayoutRule15" runat="server" Merge="True" />
							<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" Size="S" CommitChanges="True" />
							<px:PXButton ID="btnViewOnMap" runat="server" CommandName="ViewMainOnMap" CommandSourceID="ds"
								Size="xs" Text="View On Map" Height="20" />
							<px:PXLayoutRule ID="PXLayoutRule9" runat="server" />
						</Template>
						<ContentLayout OuterSpacing="None" />
						<ContentStyle BackColor="Transparent" BorderStyle="None">
						</ContentStyle>
					</px:PXFormView>
					<px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartGroup="True" GroupCaption="CRM" StartColumn="True" ControlSize="XM" LabelsWidth="M" />
					<px:PXSelector ID="ClassID" runat="server" CommitChanges="True" DataField="ClassID"
						FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" />
					<px:PXTextEdit ID="edAcctReferenceNbr" runat="server" DataField="AcctReferenceNbr" />
					<px:PXSegmentMask ID="edParentBAccountID" runat="server" DataField="ParentBAccountID"
						AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" CommitChanges="true" />
                    <px:PXDropDown ID="edDuplicateStatus1" runat="server" DataField="Contact__DuplicateStatus" Enabled="False"/>                  
                    <px:PXDateTimeEdit ID="edLastIncomingDate" runat="server" DataField="CRActivityStatistics__LastIncomingActivityDate" Enabled="False" Size="SM"/>
                    <px:PXDateTimeEdit ID="edLastOutgoingDate" runat="server" DataField="CRActivityStatistics__LastOutgoingActivityDate" Enabled="False" Size="SM"/>
					<px:PXLayoutRule ID="PXLayoutRule10" runat="server" ControlSize="XM" LabelsWidth="SM"
						StartColumn="True" />
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Duplicates" BindingContext="frmDefContact" VisibleExp="DataControls[&quot;edDuplicateFoundFrm&quot;].Value == true" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="PXGridDuplicates" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%"
						Height="200px" MatrixMode="True">
					    <ActionBar>
							<CustomItems>
					            <px:PXToolBarButton Text="Merge" Key="cmdMerge">
                                        <AutoCallBack Command="Merge" Target="ds"></AutoCallBack>
                                        <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Duplicates">
								<Columns>
								    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="80px" AllowCheckAll="True"/>
                                    <px:PXGridColumn DataField="Contact2__ContactType" TextAlign="Left" Width="100px" AllowShowHide="False" />
                                    <px:PXGridColumn DataField="Contact2__DuplicateStatus" TextAlign="Left" Width="140px" />
									<px:PXGridColumn DataField="Contact2__LastModifiedDateTime" TextAlign="Left" Width="160px" />
									<px:PXGridColumn DataField="Contact2__DisplayName" TextAlign="Left" Width="180px" AllowShowHide="False"  LinkCommand="Duplicates_Contact_ViewDetails" />									
									<px:PXGridColumn DataField="Contact2__BAccountID" TextAlign="Left" Width="180px" LinkCommand="ViewDuplicateAccount"  />
                                    <px:PXGridColumn DataField="BAccountR__Type" TextAlign="Left" Width="80px" />
                                    <px:PXGridColumn DataField="BAccountR__AcctName" TextAlign="Left" Width="180px" />
									<px:PXGridColumn DataField="Contact2__Email" TextAlign="Left" Width="180px" AllowShowHide="False" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="200" />
						<ActionBar PagerVisible="False">
							<Actions>
								<Search Enabled="False" />
							</Actions>
						</ActionBar>
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Attributes">
				<Template>
					<px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%"
						Height="200px" MatrixMode="True">
						<Levels>
							<px:PXGridLevel DataMember="Answers">
								<Columns>
									<px:PXGridColumn DataField="AttributeID" TextAlign="Left" Width="250px" AllowShowHide="False"
										TextField="AttributeID_description" />
    								<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" Width="75px" />
									<px:PXGridColumn DataField="Value" Width="300px" AllowShowHide="False" AllowSort="False" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="200" />
						<ActionBar>
							<Actions>
								<Search Enabled="False" />
							</Actions>
						</ActionBar>
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Activities" LoadOnDemand="True">
				<Template>
					<pxa:PXGridWithPreview ID="gridActivities" runat="server" DataSourceID="ds" Width="100%"
						AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
						FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Inquire" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray"
						PreviewPanelStyle="z-index: 100; background-color: Window" PreviewPanelSkinID="Preview"
						BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">
						<ActionBar DefaultAction="cmdViewActivity" CustomItemsGroup="0" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Key="cmdAddTask">
									<AutoCallBack Command="NewTask" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddEvent">
									<AutoCallBack Command="NewEvent" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddEmail">
									<AutoCallBack Command="NewMailActivity" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddActivity">
									<AutoCallBack Command="NewActivity" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Activities">
								<Columns>
									<px:PXGridColumn DataField="IsCompleteIcon" Width="21px" AllowShowHide="False" AllowResize="False"
										ForceExport="True" />
									<px:PXGridColumn DataField="PriorityIcon" Width="21px" AllowShowHide="False" AllowResize="False"
										ForceExport="True" />
									<px:PXGridColumn DataField="ClassIcon" Width="31px" AllowShowHide="False" AllowResize="False"
										ForceExport="True" />
									<px:PXGridColumn DataField="ClassInfo" Width="60px" />
									<px:PXGridColumn DataField="RefNoteID" Visible="false" AllowShowHide="False" />
									<px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="297px" />
									<px:PXGridColumn DataField="UIStatus" />
                                    <px:PXGridColumn DataField="Released" TextAlign="Center" Type="CheckBox" Width="80px" />
									<px:PXGridColumn DataField="StartDate" DisplayFormat="g" Width="120px" />
                                    <px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="120px" Visible="False" />
									<px:PXGridColumn DataField="TimeSpent" Width="80px" />
									<px:PXGridColumn DataField="CreatedByID" Visible="false" AllowShowHide="False" />
									<px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="false"
										SyncVisible="False" SyncVisibility="False" Width="108px">
										<NavigateParams>
											<px:PXControlParam Name="PKID" ControlID="gridActivities" PropertyName="DataValues[&quot;CreatedByID&quot;]" />
										</NavigateParams>
									</px:PXGridColumn>
									<px:PXGridColumn DataField="WorkgroupID" Width="90px" />
									<px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" Width="150px" DisplayMode="Text"/>
                                    <px:PXGridColumn DataField="ProjectID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="ProjectTaskID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false"/>
                                    <px:PXGridColumn DataField="Source" Width="120" AllowResize="True" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<PreviewPanelTemplate>
							<px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine"
								MaxLength="50" Width="100%" Height="100%" SkinID="Label" >
                                      <AutoSize Container="Parent" Enabled="true" />
                                </px:PXHtmlView>
						</PreviewPanelTemplate>
						<AutoSize Enabled="true" />
						<GridMode AllowAddNew="False" AllowDelete="False" AllowFormEdit="False" AllowUpdate="False" AllowUpload="False" />
					</pxa:PXGridWithPreview>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Contacts" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdContacts" runat="server" DataSourceID="ds" Height="522px" Width="100%"
						SkinID="Inquire" AllowSearch="True">
						<Levels>
							<px:PXGridLevel DataMember="Contacts">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="280px" LinkCommand="Contacts_ViewDetails" />
									<px:PXGridColumn DataField="Salutation" Width="160px" />
									<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn DataField="Address__City" Width="180px" />
									<px:PXGridColumn DataField="EMail" Width="200px" />
									<px:PXGridColumn DataField="Phone1" Width="140px" />
									<px:PXGridColumn DataField="WorkgroupID" Width="108px" />
									<px:PXGridColumn DataField="ContactType" Width="108px"/>
									<px:PXGridColumn DataField="OwnerID" Width="108px" DisplayMode="Text" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar DefaultAction="cmdViewContact" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton ImageKey="AddNew" Tooltip="Add New Contact" DisplayStyle="Image">
									<AutoCallBack Command="AddContact" Target="ds">
									</AutoCallBack>
									<PopupCommand Command="Refresh" Target="grdContacts">
									</PopupCommand>
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Contact Details" Key="cmdViewContact" Visible="False">
									<AutoCallBack Command="Contacts_ViewDetails" Target="ds">
									</AutoCallBack>
									<PopupCommand Command="Refresh" Target="grdContacts">
									</PopupCommand>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Delivery Settings" LoadOnDemand="true">
				<Template>
					<px:PXFormView ID="frmDefLocation1" runat="server" DataMember="DefLocation" DataSourceID="ds"
						Width="100%" CaptionVisible="False" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule12" runat="server" StartColumn="True" LabelsWidth="SM"
								ControlSize="XM" />
							<px:PXLayoutRule ID="PXLayoutRule13" runat="server" StartGroup="True" GroupCaption="Shipping Contact" />
							<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkIsContactSameAsMain"
								runat="server" DataField="IsContactSameAsMain" />
							<px:PXPanel ID="PXPanel3" runat="server" RenderStyle="Simple" RenderSimple="True">
								<px:PXFormView ID="frmDefLocationContact" runat="server" DataMember="DefLocationContact"
									DataSourceID="ds" SkinID="Transparent">
									<Template>
										<px:PXLayoutRule ID="PXLayoutRule14" runat="server" LabelsWidth="SM" ControlSize="XM"
											StartColumn="True" />
                                        <px:PXLayoutRule ID="PXLayoutRule8" runat="server" Merge="True" />
                                        <px:PXLabel ID="lblSalutation1" runat="server">Attention:</px:PXLabel>
										<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" SuppressLabel="True" />
                                        <px:PXLayoutRule ID="PXLayoutRule22" runat="server" />
										<px:PXMailEdit ID="edEMail" runat="server" DataField="EMail" />
										<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
										<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" />
										<px:PXMaskEdit ID="edFax" runat="server" DataField="Fax" />
									</Template>
									<ContentLayout OuterSpacing="None" />
								</px:PXFormView>
							</px:PXPanel>
							<px:PXLayoutRule ID="PXLayoutRule15" runat="server" GroupCaption="Shipping Address"
								StartGroup="True" />
							<px:PXLayoutRule ID="PXLayoutRule16" runat="server" Merge="True" />
							<px:PXPanel ID="PXPanel4" runat="server" RenderStyle="Simple" RenderSimple="True">
								<px:PXFormView ID="frmDefLocationAddress" runat="server" DataMember="DefLocationAddress"
									DataSourceID="ds" SkinID="Transparent">
									<Template>
										<px:PXLayoutRule ID="PXLayoutRule14" runat="server" Merge="True" SuppressLabel="False" />
										<px:PXFormView ID="frmDefLocationCurrent" runat="server" DataMember="DefLocationCurrent"
											DataSourceID="ds" RenderStyle="Simple">
											<Template>
												<px:PXLayoutRule ID="PXLayoutRule14" runat="server" SuppressLabel="False" LabelsWidth="SM" />
												<px:PXCheckBox ID="chkIsSameAsMaint" runat="server" DataField="IsAddressSameAsMain"
													CommitChanges="true">
												</px:PXCheckBox>
											</Template>
											<ContentStyle BackColor="Transparent"/>
										</px:PXFormView>
										<px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" />
										<px:PXLayoutRule ID="PXLayoutRule17" runat="server" LabelsWidth="SM" ControlSize="XM" />
										<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
										<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
										<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
										<px:PXSelector ID="edCountryID2" runat="server" AllowEdit="True" DataField="CountryID"
											FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" CommitChanges="True" />
										<px:PXSelector ID="edState2" runat="server" AutoRefresh="True" DataField="State"
											CommitChanges="true" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" />
										<px:PXLayoutRule ID="PXLayoutRule18" runat="server" Merge="True" LabelsWidth="SM" />
										<px:PXMaskEdit Size="S" ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
										<px:PXButton ID="btnViewOnMap1" runat="server" CommandName="ViewDefLocationOnMap"
											CommandSourceID="ds" Size="xs" Text="View On Map" Height="20">
										</px:PXButton>
									</Template>
									<ContentLayout OuterSpacing="None" />
								</px:PXFormView>
							</px:PXPanel>
							<px:PXLayoutRule ID="PXLayoutRule19" runat="server" />
							<px:PXLayoutRule ID="PXLayoutRule20" runat="server" ControlSize="XM" LabelsWidth="SM"
								StartColumn="True" />
							<px:PXLayoutRule ID="PXLayoutRule21" runat="server" StartGroup="True" GroupCaption="Default Location Settings" />
							<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
							<px:PXTextEdit ID="edTaxRegistrationID" runat="server" DataField="TaxRegistrationID" />
							<px:PXSelector ID="edCTaxZoneID" runat="server" DataField="CTaxZoneID" AllowEdit="True" />
							<px:PXSelector ID="edCBranchID" runat="server" DataField="CBranchID" AllowEdit="True" />
							<px:PXSelector ID="edCPriceClassID" runat="server" DataField="CPriceClassID" AllowEdit="True" />
							<px:PXLayoutRule ID="PXLayoutRule1" runat="server" GroupCaption="Shipping Instructions" />
							<px:PXSegmentMask ID="edCSiteID" runat="server" DataField="CSiteID" AllowEdit="True" />
							<px:PXSelector CommitChanges="True" ID="edCarrierID" runat="server" DataField="CCarrierID"
								AllowEdit="True" />
							<px:PXSelector ID="edShipTermsID" runat="server" DataField="CShipTermsID" AllowEdit="True" />
							<px:PXSelector ID="edShipZoneID" runat="server" DataField="CShipZoneID" AllowEdit="True" />
							<px:PXSelector ID="edFOBPointID" runat="server" DataField="CFOBPointID" AllowEdit="True" />
                             <px:PXCheckBox ID="chkResedential" runat="server" DataField="CResedential" />
                            <px:PXCheckBox ID="chkSaturdayDelivery" runat="server" DataField="CSaturdayDelivery" />
							<px:PXCheckBox ID="chkInsurance" runat="server" DataField="CInsurance" />
							<px:PXDropDown ID="edCShipComplete" runat="server" DataField="CShipComplete" />
							<px:PXNumberEdit ID="edCOrderPriority" runat="server" DataField="COrderPriority" />
							<px:PXNumberEdit ID="edLeadTime" runat="server" DataField="CLeadTime" />
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None">
						</ContentStyle>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Locations" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdLocations" runat="server" DataSourceID="ds" Height="100%" Width="100%"
						SkinID="Inquire" AllowSearch="True">
						<Levels>
							<px:PXGridLevel DataMember="Locations">
								<Columns>
									<px:PXGridColumn DataField="LocationCD" LinkCommand="Locations_ViewDetails" Width="80px" />
									<px:PXGridColumn DataField="Descr" Width="220px" />
									<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn DataField="IsDefault" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn DataField="Address__City" Width="180px" />
									<px:PXGridColumn DataField="Address__CountryID" RenderEditorText="True" AutoCallBack="True" />
									<px:PXGridColumn DataField="Address__State" Width="120px" RenderEditorText="True" />
									<px:PXGridColumn DataField="CTaxZoneID" />
									<px:PXGridColumn DataField="CPriceClassID" Width="81px" AllowShowHide="Server" />
									<px:PXGridColumn DataField="CSalesAcctID" Width="108px" AutoCallBack="True" AllowShowHide="Server" />
									<px:PXGridColumn DataField="CSalesSubID" Width="100px" AllowShowHide="Server" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<LevelStyles>
							<RowForm Height="400px" Width="800px">
							</RowForm>
						</LevelStyles>
						<ActionBar DefaultAction="cmdViewLocation">
							<CustomItems>
								<px:PXToolBarButton ImageKey="AddNew" Tooltip="Add New Location" DisplayStyle="Image">
									<AutoCallBack Command="AddLocation" Target="ds" />
									<PopupCommand Command="Refresh" Target="grdLocations" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Location Details" Key="cmdViewLocation" Visible="False">
									<AutoCallBack Command="Locations_ViewDetails" Target="ds" />
									<PopupCommand Command="Refresh" Target="grdLocations" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Set as Default">
									<AutoCallBack Command="SetDefaultLocation" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Relations" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="grdRelations" runat="server" Height="400px" Width="100%" AllowPaging="True"
						ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details">
						<Levels>
							<px:PXGridLevel DataMember="Relations">
								<Columns>
									<px:PXGridColumn DataField="Role" Width="120px" />
									<px:PXGridColumn DataField="EntityID" Width="160px" AutoCallBack="true" TextField="EntityCD"
										LinkCommand="Relations_EntityDetails" />
									<px:PXGridColumn DataField="Name" Width="200px" />
									<px:PXGridColumn DataField="ContactID" Width="160px" AutoCallBack="true" TextField="ContactName"
										DisplayMode="Text" LinkCommand="Relations_ContactDetails" />
									<px:PXGridColumn DataField="Email" Width="120px" />
									<px:PXGridColumn DataField="AddToCC" Width="70px" Type="CheckBox" TextAlign="Center" />
								</Columns>
								<RowTemplate>
									<px:PXSelector ID="edRelEntityID" runat="server" DataField="EntityID" FilterByAllFields="True" />
									<px:PXSelector ID="edRelContactID" runat="server" DataField="ContactID" FilterByAllFields="True" AutoRefresh="True" />
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="True" />
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Opportunities" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="gridOpportunities" runat="server" DataSourceID="ds" Height="423px"
						Width="100%" AllowSearch="True" ActionsPosition="Top" SkinID="Inquire">
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Levels>
							<px:PXGridLevel DataMember="Opportunities">
								<Columns>
									<px:PXGridColumn DataField="OpportunityID" Width="130px" LinkCommand="Opportunities_ViewDetails" />
									<px:PXGridColumn DataField="OpportunityName" Width="151px" />
									<px:PXGridColumn DataField="StageID" Width="108px" />
									<px:PXGridColumn DataField="CROpportunityProbability__Probability" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="Status" Width="81px" RenderEditorText="True" />
									<px:PXGridColumn DataField="CuryProductsAmount" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="CuryID" Width="54px" />
									<px:PXGridColumn DataField="CloseDate" Width="130px" />
                                    <px:PXGridColumn DataField="BAccount__AcctCD" Width="100px" LinkCommand="Opportunities_BAccount_ViewDetails"/>
                                    <px:PXGridColumn DataField="BAccount__AcctName" Width="150px" />
									<px:PXGridColumn DataField="Contact__DisplayName" Width="120px" LinkCommand="Opportunities_Contact_ViewDetails" />
									<px:PXGridColumn DataField="WorkgroupID" Width="108px" />
									<px:PXGridColumn DataField="OwnerID" Width="108px" DisplayMode="Text" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						<ActionBar DefaultAction="cmdOpportunityDetails" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Key="cmdAddOpportunity" ImageKey="AddNew" Tooltip="Add New Opportunity" DisplayStyle="Image">
									<AutoCallBack Command="AddOpportunity" Target="ds" /> 
									<ActionBar GroupIndex="0" Order="4"/>
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Opportunity Details" Key="cmdOpportunityDetails">
									<AutoCallBack Command="Opportunities_ViewDetails" Target="ds" />
									<ActionBar GroupIndex="2" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Cases" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="gridCases" runat="server" DataSourceID="ds" Height="423px" Width="100%"
						AllowSearch="True" SkinID="Inquire" AllowPaging="true" AdjustPageSize="Auto"
						BorderWidth="0px">
						<ActionBar DefaultAction="cmdViewCaseDetails" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Key="cmdAddCase" ImageKey="AddNew" Tooltip="Add New Case" DisplayStyle="Image">
									<AutoCallBack Command="AddCase" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Case Details" Key="cmdViewCaseDetails" Visible="false">
									<ActionBar GroupIndex="0" />
									<AutoCallBack Command="Cases_ViewDetails" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Cases">
								<Columns>
									<px:PXGridColumn DataField="CaseCD" Width="80px" LinkCommand="Cases_ViewDetails" />
									<px:PXGridColumn DataField="Subject" Width="300px" />
									<px:PXGridColumn DataField="CaseClassID" Width="80px" />
                                    <px:PXGridColumn DataField="ContractID" Width="90px" />
									<px:PXGridColumn DataField="Severity" Width="54px" RenderEditorText="True" />
									<px:PXGridColumn DataField="Status" Width="72px" RenderEditorText="True" />
									<px:PXGridColumn DataField="Resolution" Width="72px" RenderEditorText="True" />
									<px:PXGridColumn DataField="CreatedDateTime" Width="90px" />
									<px:PXGridColumn DataField="InitResponse" Width="63px" DisplayFormat="###:##:##" />
									<px:PXGridColumn DataField="TimeEstimated" Width="63px" DisplayFormat="###:##:##" />
									<px:PXGridColumn DataField="ResolutionDate" Width="90px" />
									<px:PXGridColumn DataField="WorkgroupID" Width="108px" />
									<px:PXGridColumn DataField="OwnerID" Width="108px" DisplayMode="Text" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Contracts" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdContracts" runat="server" DataSourceID="ds" Height="522px" Width="100%"
						SkinID="Inquire">
						<Levels>
							<px:PXGridLevel DataMember="Contracts">
								<Columns>
									<px:PXGridColumn DataField="ContractCD" LinkCommand="Contracts_ViewDetails" Width="120px" />
									<px:PXGridColumn DataField="BAccount__AcctCD" Width="100px" LinkCommand="Contracts_BAccount_ViewDetails"/>
									<px:PXGridColumn DataField="BAccount__AcctName" Width="150px" />
									<px:PXGridColumn DataField="Description" Width="200px" />
									<px:PXGridColumn DataField="LocationID" Width="80px" LinkCommand="Contracts_Location_ViewDetails" />
									<px:PXGridColumn DataField="Status" />
									<px:PXGridColumn DataField="ExpireDate" Width="90px" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar DefaultAction="cmdViewContract" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Text="Contract Details" Key="cmdViewContract" Visible="False">
									<ActionBar GroupIndex="2" />
									<AutoCallBack Command="Contracts_ViewDetails" Target="ds" />
									<PopupCommand Command="Refresh" Target="grdContracts" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Orders" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdOrders" runat="server" DataSourceID="ds" Height="522px" Width="100%"
						SkinID="Inquire" AllowSearch="True">
						<Levels>
							<px:PXGridLevel DataMember="Orders">
								<Columns>
									<px:PXGridColumn DataField="OrderType" Width="72px" />
									<px:PXGridColumn DataField="OrderNbr" Width="90px" LinkCommand="Orders_ViewDetails" />
									<px:PXGridColumn DataField="OrderDesc" Width="117px" />
									<px:PXGridColumn DataField="CustomerOrderNbr" Width="90px" LinkCommand="Orders_ViewDetails" />
									<px:PXGridColumn DataField="Status" Width="81px" />
									<px:PXGridColumn DataField="RequestDate" Width="81px" />
									<px:PXGridColumn DataField="ShipDate" Width="81px" />
									<px:PXGridColumn DataField="ShipVia" Width="90px" />
									<px:PXGridColumn DataField="ShipZoneID" Width="81px" />
									<px:PXGridColumn DataField="OrderWeight" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="OrderVolume" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="OrderQty" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="CuryID" Width="54px" />
									<px:PXGridColumn DataField="CuryOrderTotal" TextAlign="Right" Width="81px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar DefaultAction="cmdViewOrder" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Text="Order Details" Key="cmdViewOrder" Visible="False">
									<ActionBar GroupIndex="2" />
									<AutoCallBack Command="Orders_ViewDetails" Target="ds" />
									<PopupCommand Command="Refresh" Target="grdOrders" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Campaigns" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="grdCampaignHistory" runat="server" Height="400px" Width="100%"
						AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds"
						SkinID="Inquire">
						<Levels>
							<px:PXGridLevel DataMember="Members">
								<Columns>
									<px:PXGridColumn DataField="CampaignID" Width="200px" AutoCallBack="true" LinkCommand="Members_CRCampaign_ViewDetails" />
									<px:PXGridColumn DataField="CRCampaign__CampaignName" Width="200px" />
									<px:PXGridColumn DataField="Status" Width="72px" RenderEditorText="True" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Marketing Lists" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="grdMarketingLists" runat="server" Height="400px" Width="100%"
						AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds"
						SkinID="Inquire">
						<Levels>
							<px:PXGridLevel DataMember="Subscriptions">
								<Columns>
									<px:PXGridColumn DataField="MarketingListID" Width="120px" AutoCallBack="true" TextField="CRMarketingList__MailListCode"
										LinkCommand="Subscriptions_CRMarketingList_ViewDetails" />
									<px:PXGridColumn DataField="CRMarketingList__Name" Width="200px" />
									<px:PXGridColumn DataField="Activated" Width="60px" Type="CheckBox" />
									<px:PXGridColumn DataField="Format" Width="80px" />
                                    <px:PXGridColumn DataField="CRMarketingList__MailListCode" Width="200px" SyncVisible="False" SyncVisibility="False" Visible="False" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
	</px:PXTab>
	<px:PXSmartPanel ID="pnlActivityContacts" runat="server" Height="81px" Style="z-index: 108;
		left: 352px; position: absolute; top: 99px" Width="376px" CaptionVisible="True"
		Caption="Select Contact" DesignView="Content" LoadOnDemand="True" Key="ActivityContacts"
		AutoCallBack-Target="frmActivityContacts" AutoCallBack-Command="Refresh">
		<div style="padding-top: 9px; padding-left: 9px; padding-right: 9px">
			<px:PXFormView ID="frmActivityContacts" runat="server" DataSourceID="ds"
				Width="100%" DataMember="ActivityContacts" SkinID="Transparent">
				<Template>
					<table id="tbl" style="vertical-align: top; height: 20px;" cellspacing="0" cellpadding="0">
						<tr align="left">
							<td>
								<px:PXLabel ID="lblContactID" runat="server">Contact :</px:PXLabel>
							</td>
							<td style="width: 9px;">
								&nbsp;
							</td>
							<td align="left">
								<px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID"
									LabelID="lblContactID" Width="261px" TextField="DisplayName" AutoRefresh="true"
									TextMode="Search" DisplayMode="Hint" FilterByAllFields="True" />
							</td>
						</tr>
					</table>
				</Template>
			</px:PXFormView>
		</div>
		<div style="padding: 9px; text-align: right;">
			<px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" Width="63px"
				Height="20px" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" Width="63px"
				Height="20px" Style="margin-left: 5px" />
		</div>
	</px:PXSmartPanel>
       <px:PXSmartPanel ID="spMergeParamsDlg" runat="server"
        Width="600" Caption="Please resolve the conflicts" CaptionVisible="True" Key="Duplicates" LoadOnDemand="True" ShowAfterLoad="true"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formMerge" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOk" CancelButtonID="btnCancel">
        <px:PXFormView ID="formMerge" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" SkinID="Transparent"
            DataMember="mergeParams">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True" />
                <px:PXSelector CommitChanges="True" ID="edBAccountID" runat="server" DataField="BAccountID" FilterByAllFields="True" DisplayMode="Text" TextMode="Search" AutoRefresh="True"/>                
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="grdFields" runat="server" Height="350px" Width="100%" DataSourceID="ds" MatrixMode="True">
                    <Levels>
                        <px:PXGridLevel DataMember="ValueConflicts">
                            <Columns>
                                <px:PXGridColumn DataField="DisplayName" Width="108px" />
                                <px:PXGridColumn DataField="Value" Width="190px" AutoCallBack="True" />
                            </Columns>
                            <Layout ColumnsMenu="False" />
                            <Mode AllowAddNew="false" AllowDelete="false" />
                        </px:PXGridLevel>
                    </Levels>
                    <ActionBar>
                        <Actions>
                            <ExportExcel Enabled="False" />
                            <AddNew Enabled="False" />
                            <FilterShow Enabled="False" />
                            <FilterSet Enabled="False" />
                            <Save Enabled="False" />
                            <Delete Enabled="False" />
                            <NoteShow Enabled="False" />
                            <Search Enabled="False" />
                            <AdjustColumns Enabled="False" />
                        </Actions>
                    </ActionBar>
                    <CallbackCommands>
                        <Save PostData="Page" />
                    </CallbackCommands>
                      <AutoSize Enabled="true" />
                </px:PXGrid>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" Text="OK" DialogResult="OK" />
            <px:PXButton ID="PXButton2" runat="server" Text="Cancel" DialogResult="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
