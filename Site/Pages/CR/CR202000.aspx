<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR202000.aspx.cs" Inherits="Page_CR202000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CampaignMaint"
		PrimaryView="Campaign">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="MultipleInsert" Visible="False" />
			<px:PXDSCallbackCommand Name="MultipleDelete" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
		Caption="Campaign Summary" DataMember="Campaign" FilesIndicator="True" NoteIndicator="True"
		ActivityIndicator="True" ActivityField="NoteActivity" DefaultControlID="edCampaignID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXSelector ID="edCampaignID" runat="server" DataField="CampaignID" NullText="<NEW>"
				DataSourceID="ds" Size="SM" />
			<px:PXCheckBox ID="chkIsActive" runat="server" Checked="True" DataField="IsActive" />
			<px:PXLayoutRule runat="server" />
			<px:PXLayoutRule runat="server" ColumnSpan="2" />
			<px:PXTextEdit ID="edCampaignName" runat="server" AllowNull="False" DataField="CampaignName" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XS" ControlSize="SM" />
			<px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="290px" DataSourceID="ds" DataMember="CampaignCurrent">
		<Items>
			<px:PXTabItem Text="Campaign Details">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartColumn="True" LabelsWidth="SM"
						ControlSize="M" />
					<px:PXSelector ID="edCampaignType" runat="server" AllowNull="False" DataField="CampaignType" />
					<px:PXDropDown ID="edDefaultMemberStatus" runat="server" AllowNull="False" DataField="DefaultMemberStatus"
						Size="S" />
					<px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" />
					<px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" />
					<px:PXNumberEdit ID="edExpectedRevenue" runat="server" DataField="ExpectedRevenue"
						NullText="0.00" />
					<px:PXNumberEdit ID="edPlannedBudget" runat="server" DataField="PlannedBudget" NullText="0.00" />
					<px:PXNumberEdit ID="edActualCost" runat="server" DataField="ActualCost" NullText="0.00" />
					<px:PXNumberEdit ID="edExpectedResponse" runat="server" DataField="ExpectedResponse"
						NullText="0.00" />
					<px:PXNumberEdit ID="edMailsSent" runat="server" DataField="MailsSent" NullText="0" />
					<px:PXTextEdit ID="edPromoCodeID" runat="server" AllowNull="False" DataField="PromoCodeID" />
					<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="SM"
						ControlSize="M" />
					<px:PXNumberEdit ID="edLeadsGenerated" runat="server" DataField="LeadsGenerated"
						Enabled="false" />
					<px:PXNumberEdit ID="edLeadsConverted" runat="server" DataField="LeadsConverted"
						Enabled="false" />
					<px:PXNumberEdit ID="edContacts" runat="server" DataField="Contacts" />
					<px:PXNumberEdit ID="edResponses" runat="server" DataField="Responses" />
					<px:PXNumberEdit ID="edOpportunities" runat="server" DataField="Opportunities" />
					<px:PXNumberEdit ID="edClosedOpportunities" runat="server" DataField="ClosedOpportunities" />
					<px:PXNumberEdit ID="edOpportunitiesValue" runat="server" DataField="OpportunitiesValue"
						NullText="0.00" />
					<px:PXNumberEdit ID="edClosedOpportunitiesValue" runat="server" DataField="ClosedOpportunitiesValue"
						NullText="0.00" /></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Members">
				<Template>
					<px:PXGrid ID="grdCampaignMembers" runat="server" SkinID="DetailsInTab" Height="400px"
						Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" ActionsPosition="Top"
						AllowSearch="true" DataSourceID="ds" BorderWidth="0px">
						<Levels>
							<px:PXGridLevel DataMember="CampaignMembers">
								<Columns>
									<px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" AllowMove="False"
										AllowSort="False" TextAlign="Center" Type="CheckBox" Width="30px" />
									<px:PXGridColumn DataField="CampaignID" Width="81px" Visible="False" AllowShowHide="False" />
									<px:PXGridColumn DataField="ContactID" Width="250px" TextField="Contact__DisplayName"
										AutoCallBack="true" DisplayMode="Text"/>
									<px:PXGridColumn DataField="Contact__Salutation" Width="150px" AllowUpdate="False" />
									<px:PXGridColumn DataField="Contact__EMail" Width="150px" AllowUpdate="False" />
									<px:PXGridColumn DataField="Contact__Phone1" Width="150px" AllowUpdate="False" DisplayFormat="+# (###) ###-#### Ext:####" />
									<px:PXGridColumn DataField="Contact__BAccountID" AllowUpdate="False" DisplayFormat="CCCCCCCCCC"
										Width="100px" />
								    <%--<px:PXGridColumn AllowUpdate="False" DataField="Contact__BAccountID_BAccount_acctName" Width="120px" />--%>
									<px:PXGridColumn AllowNull="False" DataField="Status" Width="72px" RenderEditorText="True" />
									<px:PXGridColumn AllowNull="False" DataField="Contact__IsActive" TextAlign="Center"
										Type="CheckBox" Width="60px" />
									<px:PXGridColumn DataField="Contact__Phone2" DisplayFormat="+#(###) ###-####" Width="140px"
										Visible="false" />
									<px:PXGridColumn DataField="Contact__Phone3" DisplayFormat="+#(###) ###-####" Width="140px"
										Visible="false" />
									<px:PXGridColumn DataField="Contact__Fax" DisplayFormat="+#(###) ###-####" Width="140px"
										Visible="false" />
									<px:PXGridColumn DataField="Contact__WebSite" Width="140px" Visible="false" />
									<px:PXGridColumn DataField="Contact__DateOfBirth" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Contact__CreatedByID" Width="108px" Visible="false" />
									<px:PXGridColumn DataField="Contact__LastModifiedByID" Width="108px" Visible="false" />
									<px:PXGridColumn DataField="Contact__CreatedDateTime" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Contact__LastModifiedDateTime" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Contact__WorkgroupID" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Contact__OwnerID" Width="90px" Visible="false" />
									<px:PXGridColumn AllowNull="False" DataField="Contact__ClassID" TextAlign="Center"
										Width="60px" Visible="false" />
									<px:PXGridColumn DataField="Contact__Source" Width="54px" Visible="false" />
									<px:PXGridColumn DataField="Contact__Title" Width="54px" Visible="false" />
									<px:PXGridColumn DataField="Contact__FirstName" Width="100px" />
									<px:PXGridColumn DataField="Contact__MidName" Width="100px" />
									<px:PXGridColumn DataField="Contact__LastName" Width="100px" />
									<px:PXGridColumn DataField="Address__AddressLine1" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Address__AddressLine2" Width="90px" Visible="false" />
									<px:PXGridColumn DataField="Contact__Status" Width="90px" />
								</Columns>
                                <RowTemplate>
                                    <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="Contact__DisplayName" />
                                </RowTemplate>
							</px:PXGridLevel>
						</Levels>
                        <Mode InitNewRow="true" AllowUpload="True"/>
						<ActionBar DefaultAction="cmdViewDoc">
							<CustomItems>
								<px:PXToolBarButton Text="Add new members" Key="cmdMultipleInsert">
								    <AutoCallBack Command="MultipleInsert" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Delete selected" Key="cmdMultipleDelete">
								    <AutoCallBack Command="MultipleDelete" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<AutoSize Enabled="True" MinHeight="200" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
	</px:PXTab>
</asp:Content>
