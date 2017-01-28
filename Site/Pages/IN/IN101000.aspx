<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="IN101000.aspx.cs" Inherits="Page_IN101000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.IN.INSetupMaint" PrimaryView="setup">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="ViewRestrictionGroup" Visible="False" DependOnGrid="gridGroups" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="684px" Style="z-index: 100" Width="100%" DataMember="setup">
		<Activity HighlightColor="" SelectedColor="" Width="" Height="" />
		<Items>
			<px:PXTabItem Text="General Settings">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Numbering Settings" />
					<px:PXSelector ID="edBatchNumberingID" runat="server" DataField="BatchNumberingID" Text="BATCH" AllowEdit="True" />
					<px:PXSelector ID="edReceiptNumberingID" runat="server" DataField="ReceiptNumberingID" Text="INRECEIPT" AllowEdit="True" />
					<px:PXSelector ID="edIssueNumberingID" runat="server" DataField="IssueNumberingID" Text="INISSUE" AllowEdit="True" />
					<px:PXSelector ID="edAdjustmentNumberingID" runat="server" DataField="AdjustmentNumberingID" Text="INADJUST" AllowEdit="True" />
					<px:PXSelector ID="edKitAssemblyNumberingID" runat="server" DataField="KitAssemblyNumberingID" Text="INKITASSY" AllowEdit="True" />
					<px:PXSelector ID="edPINumberingID" runat="server" AllowEdit="True" DataField="PINumberingID" Text="PIID" />
					<px:PXSelector ID="edReplenishmentNumberingID" runat="server" DataField="ReplenishmentNumberingID" Text="INREPL" AllowEdit="True" />
					<px:PXSelector ID="edServiceItemNumbering" runat="server" DataField="ServiceItemNumberingID" AllowEdit="True" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Inventory Options" />
					<px:PXSelector ID="edWeightUOM" runat="server" DataField="WeightUOM" AllowEdit="True" Size="S" />
					<px:PXSelector ID="edVolumeUOM" runat="server" DataField="VolumeUOM" AllowEdit="True" Size="S" />
					<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkUseInventory" runat="server" Checked="True" DataField="UseInventorySubItem" />
					<px:PXCheckBox ID="chkReplanBackOrders" runat="server" DataField="ReplanBackOrders" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Account Settings" />
					<px:PXSegmentMask CommitChanges="True" ID="edARClearingAcctID" runat="server" DataField="ARClearingAcctID" />
					<px:PXSegmentMask ID="edARClearingSubID" runat="server" DataField="ARClearingSubID" AutoRefresh="True" />
					<px:PXSegmentMask CommitChanges="True" ID="edINTransitAcctID" runat="server" DataField="INTransitAcctID" />
					<px:PXSegmentMask ID="edINTransitSubID" runat="server" DataField="INTransitSubID" AutoRefresh="True" />
					<px:PXSegmentMask CommitChanges="True" ID="edINProgressAcctID" runat="server" DataField="INProgressAcctID" />
					<px:PXSegmentMask ID="edINProgressSubID" runat="server" DataField="INProgressSubID" AutoRefresh="True" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Posting Settings" />
					<px:PXCheckBox SuppressLabel="True" ID="chkUpdateGL" runat="server" DataField="UpdateGL" />
					<px:PXCheckBox SuppressLabel="True" ID="chkSummPost" runat="server" DataField="SummPost" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAutoPost" runat="server" DataField="AutoPost" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Data Entry Settings" />
					<px:PXCheckBox SuppressLabel="True" ID="PXCheckBox4" runat="server" Checked="True" DataField="HoldEntry" />
					<px:PXCheckBox SuppressLabel="True" ID="PXCheckBox5" runat="server" Checked="True" DataField="RequireControlTotal" />
					<px:PXCheckBox SuppressLabel="True" ID="chkByOne" runat="server" DataField="AddByOneBarcode" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAutoAdd" runat="server" DataField="AutoAddLineBarcode" />
					<px:PXSelector CommitChanges="True" ID="edDfltItemClassID" runat="server" DataField="DfltItemClassID" Text="DEFAULT" AllowEdit="True" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Default Reason Codes" />
					<px:PXSelector CommitChanges="True" ID="edReceiptReasonCode" runat="server" DataField="ReceiptReasonCode" AllowEdit="True" />
					<px:PXSelector CommitChanges="True" ID="edIssuesReasonCode" runat="server" DataField="IssuesReasonCode" AllowEdit="True" />
					<px:PXSelector CommitChanges="True" ID="edAdjustmentReasonCode" runat="server" DataField="AdjustmentReasonCode" AllowEdit="True" />
					<px:PXSelector CommitChanges="True" ID="edPIAdjReasonCode" runat="server" DataField="PIReasonCode" AllowEdit="True" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Physical Inventory Settings" />
					<px:PXCheckBox SuppressLabel="True" ID="chkPIUseTags" runat="server" DataField="PIUseTags" />
					<px:PXNumberEdit ID="edPILastTagNumber" runat="server" DataField="PILastTagNumber" />
					<px:PXNumberEdit CommitChanges="True" ID="edTurnoverPeriodsPerYear" runat="server" DataField="TurnoverPeriodsPerYear" />
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Subitem/Restriction Groups" BindingContext="form" VisibleExp="DataControls[&quot;chkUseInventory&quot;].Value = 1">
				<Template>
					<px:PXGrid ID="gridGroups" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" AdjustPageSize="Auto" AllowSearch="True" SkinID="Details" BorderWidth="0px">
						<ActionBar>
							<Actions>
								<NoteShow Enabled="False" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton Text="Group Details" CommandSourceID="ds" CommandName="ViewRestrictionGroup" />
							</CustomItems>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Groups" DataKeyNames="GroupName">
								<Columns>
									<px:PXGridColumn AllowUpdate="False" DataField="GroupName" Width="150px" AutoCallBack="True" />
									<px:PXGridColumn AllowUpdate="False" DataField="Description" Width="200px" />
									<px:PXGridColumn AllowUpdate="False" DataField="Active" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn AllowUpdate="False" DataField="GroupType" Label="Visible To Entities" RenderEditorText="True" Width="171px" />
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
									<px:PXCheckBox SuppressLabel="True" ID="chkSelected" runat="server" DataField="Included" />
									<px:PXSelector ID="edGroupName" runat="server" DataField="GroupName" />
									<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
									<px:PXCheckBox SuppressLabel="True" ID="chkActive" runat="server" Checked="True" DataField="Active" />
									<px:PXDropDown ID="edGroupType" runat="server" DataField="GroupType" Enabled="False" />
								</RowTemplate>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="true" />
	</px:PXTab>
</asp:Content>
