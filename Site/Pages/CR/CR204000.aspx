<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR204000.aspx.cs" Inherits="Page_CR204000"
	Title="Mailing Lists" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CRMarketingListMaint"
		PrimaryView="MailLists">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="Details" DependOnGrid="grdSubscribers" Visible="false" />
			<px:PXDSCallbackCommand Name="Process" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="ProcessAll" Visible="False" CommitChanges="true" />
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
		</DataTrees>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
		Width="100%" DataMember="MailLists"  Caption="Marketing List Info"
		 FilesIndicator="True" 
		 NoteIndicator="True"
		ActivityIndicator="true" ActivityField="NoteActivity" 
		DefaultControlID="edMailListCode" >
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True"  LabelsWidth="s" ControlSize="M" /> 
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
			<px:PXSegmentMask Size="SM" ID="edMailListCode" runat="server" DataField="MailListCode"  />
			<px:PXCheckBox ID="chkIsActive" runat="server" DataField="IsActive" />
			<px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="False" />

			<px:PXTextEdit ID="edName" runat="server" AllowNull="False" DataField="Name"  />
			<px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True"  LabelsWidth="s" ControlSize="sm" />

			<px:PXTreeSelector CommitChanges="True" ID="edWorkgroupID" runat="server" DataField="WorkgroupID" TreeDataMember="_EPCompanyTree_Tree_"
				TreeDataSourceID="ds"  PopulateOnDemand="true"
				InitialExpandLevel="0" ShowRootNode="false">
				<DataBindings>
					<px:PXTreeItemBinding TextField="Description" ValueField="Description" />
				</DataBindings>
			</px:PXTreeSelector>
			<px:PXSelector CommitChanges="True" ID="edOwnerID" runat="server" DataField="OwnerID" AutoRefresh="True" TextField="acctname"  /></Template>
		
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" 
		DataMember="MailListsCurrent"  DataSourceID="ds" RepaintOnDemand="false" >
		<Items>
			<px:PXTabItem Text="Configuration Options">
				<Template>
				<px:PXPanel ID="PXPanel1" runat="server">
				<px:PXLayoutRule runat="server" StartColumn="True" />
				<px:PXDropDown ID="edMethod" runat="server" DataField="Method" AllowNull="false" />
				<px:PXCheckBox CommitChanges="True" ID="edIsDynamic" runat="server" DataField="IsDynamic"  />
				<px:PXCheckBox ID="edIsSelfManaged" runat="server" DataField="IsSelfManaged" />
				
				<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Exclude Contacts Marked As"/>
				<px:PXPanel ID="pnlMarks" runat="server" RenderSimple="True" RenderStyle="Simple">
				<px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartColumn="True"  LabelsWidth="M" ControlSize="XM" SuppressLabel="True"/>

				<px:PXLayoutRule ID="PXLayoutRule6" runat="server" Merge="True" />
				<px:PXCheckBox SuppressLabel="True" ID="edNoFax" runat="server" DataField="NoFax" AlignLeft="true" Width="90"/>
				<px:PXCheckBox ID="edNoMail" runat="server" DataField="NoMail" AlignLeft="true" Width="90"/>
				<px:PXCheckBox ID="edNoMarketingMaterials" runat="server" DataField="NoMarketing" AlignLeft="true" Width="90"/>
				<px:PXLayoutRule ID="PXLayoutRule7" runat="server" Merge="False" />

				<px:PXLayoutRule ID="PXLayoutRule8" runat="server" Merge="True" />

				<px:PXCheckBox SuppressLabel="True" ID="edNoCall" runat="server" DataField="NoCall" AlignLeft="true" Width="90"/>
				<px:PXCheckBox ID="edNoEMail" runat="server" DataField="NoEMail" AlignLeft="true" Width="90"/>
				<px:PXCheckBox ID="edNoMassMail" runat="server" DataField="NoMassMail" AlignLeft="true" Width="90"/>
				<px:PXLayoutRule ID="PXLayoutRule9" runat="server" Merge="False" />
				</px:PXPanel> 
				</px:PXPanel>
				<px:PXRichTextEdit ID="edDescription" runat="server" DataField="Description" Style="border-width: 0px; width: 100%;"
					AllowAttached="true" AllowSearch="true" AllowMacros="true" AllowLoadTemplate="false" AllowSourceMode="true">
                    <LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="Notifications" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M"/>
					<AutoSize Enabled="True" MinHeight="216" />
				</px:PXRichTextEdit>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="List Members" RepaintOnDemand="false" VisibleExp="DataControls[&quot;edIsDynamic&quot;].Value != true">
				<Template>
					<px:PXGrid ID="grdSubscribers" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100; border: 0px;"
						Width="100%" SkinID="Details" AllowPaging="True" ActionsPosition="Top"
						AllowSearch="true" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="MailRecipients" >
								<Columns>
									<px:PXGridColumn DataField="ContactID" TextField="ContactBAccount__Contact" Width="200px" AutoCallBack="true" DisplayMode="Text" />
									<px:PXGridColumn DataField="ContactBAccount__ContactType" Width="150px" />
									<px:PXGridColumn DataField="ContactBAccount__EMail" Width="200px" />
                                    <px:PXGridColumn DataField="BAccountR__AcctCD" Width="200px" />
                                    <px:PXGridColumn DataField="BAccountR__AcctName" Width="200px" />									
                                    <px:PXGridColumn AllowNull="False" DataField="Format" Type="DropDownList" Width="90px" />
									<px:PXGridColumn DataField="Activated" Width="60px" Type="CheckBox" />
								</Columns>
                                <RowTemplate>
                                    <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID">
                                        <GridProperties FastFilterFields="DisplayName,Contact__EMail">
                                        </GridProperties>
                                    </px:PXSelector>
                                </RowTemplate>
								<Mode AllowUpload="True" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="216" />
						<ActionBar PagerVisible="False">
							<PagerSettings Mode="NextPrevFirstLast" />
							<Actions>
								<Save Enabled="False" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Selection Criteria">
				<Template>
					<div style="position: relative; height: 210px; width: 100%; left: 0px; top: 0px;
						border-bottom: solid 1px DarkGrey">
						<px:PXFilterEditor ID="mainFilter" runat="server" SkinID="External" FilterRowsView="FilteredItems$FilterRow"
							DataSourceID="ds" Width="700px" Style="left: 5px; position: absolute; top: 5px" LinkedGridID="grdItems"
							AllowRowsCommit="true" ShowDefaultFilter="true" Height="130px">
						</px:PXFilterEditor>
					</div>
					<px:PXGrid ID="grdItems" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100; border: 0px"
						Width="100%" ActionsPosition="Top" Caption="Selection Preview" RepaintColumns="true" 
						AllowPaging="True" AdjustPageSize="auto" AutoGenerateColumns="AppendDynamic" ExternalFilter="true" SkinID="Inquire">
						<Levels>
							<px:PXGridLevel DataMember="FilteredItems" >
								<Columns>
									<px:PXGridColumn AllowCheckAll="True" AllowNull="False" AllowShowHide="Server" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="40px" />
									<px:PXGridColumn DataField="ContactType" Width="54px" />
									<px:PXGridColumn AllowUpdate="False" DataField="ContactID" Visible="false" AllowShowHide="False" />
									<px:PXGridColumn DataField="FullName" Width="150px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="DisplayName"  Width="280px">
										<NavigateParams>
											<px:PXControlParam Name="ContactID" ControlID="grdItems" PropertyName="DataValues[&quot;ContactID&quot;]" />
										</NavigateParams>
									</px:PXGridColumn>
									<px:PXGridColumn AllowNull="False" DataField="IsActive" TextAlign="Center" Type="CheckBox" Width="60px" Visible="False" />
									<px:PXGridColumn DataField="ClassID" RenderEditorText="True" TextAlign="Center" Width="60px" Visible="False" />
									<px:PXGridColumn DataField="Source" Width="54px" RenderEditorText="True" Type="DropDownList" Visible="False" />
									<px:PXGridColumn DataField="Title" Width="54px" Visible="False" />
									<px:PXGridColumn DataField="Salutation" Width="160px" />
									<px:PXGridColumn DataField="BAccountID" Width="100px" />									
									<px:PXGridColumn DataField="BAccount__Type" Width="100px" />									
									<px:PXGridColumn DataField="FirstName" Width="100px" />
									<px:PXGridColumn DataField="LastName" Width="100px" />
									<px:PXGridColumn DataField="EMail" Width="200px" />
									<px:PXGridColumn DataField="Address__PostalCode" Width="90px" Visible="False" />
									<px:PXGridColumn DataField="Address__CountryID" Width="90px" Visible="False" />
									<px:PXGridColumn DataField="Address__City" Width="90px" Visible="False" />									
									<px:PXGridColumn DataField="Address__State" Width="90px" Visible="False" />
									<px:PXGridColumn DataField="Address__AddressLine1" Width="90px" Visible="False" />
									<px:PXGridColumn DataField="Address__AddressLine2" Width="90px" Visible="False" />
									<px:PXGridColumn DataField="Phone1" DisplayFormat="+#(###) ###-####" Width="140px" />
									<px:PXGridColumn DataField="Phone2" DisplayFormat="+#(###) ###-####" Width="140px" />
									<px:PXGridColumn DataField="Phone3" DisplayFormat="+#(###) ###-####" Width="140px" />
									<px:PXGridColumn DataField="Fax" DisplayFormat="+#(###) ###-####" Width="140px" />
									<px:PXGridColumn DataField="WebSite" Width="140px" />
									<px:PXGridColumn DataField="DateOfBirth" Width="90px" />
									<px:PXGridColumn DataField="CreatedByID_Creator_Username" Width="108px" />
									<px:PXGridColumn DataField="CreatedDateTime" Width="90px" />
									<px:PXGridColumn DataField="WorkgroupID" Width="90px" />
									<px:PXGridColumn DataField="OwnerID" Width="90px" DisplayMode="Text" />
									<px:PXGridColumn DataField="Status" Width="90px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="216" />
						<ActionBar DefaultAction="Details"  PagerVisible="False">
							<PagerSettings Mode="NextPrevFirstLast" />
							<CustomItems>
								<pxa:PXGridProcessing Key="cmdProc" ListItems="Add Members,Remove Members" 
									ParameterName="action" AutoCallback="True" DataMember="Operations" DataField="Action" />
							</CustomItems>
							<Actions>
								<FilterShow Enabled="False" />
								<FilterSet Enabled="False" />
							</Actions>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						<CallbackCommands>
							<Save PostData="Page" />
						</CallbackCommands>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Enabled="True" Container="Window" />
	</px:PXTab>
</asp:Content>
