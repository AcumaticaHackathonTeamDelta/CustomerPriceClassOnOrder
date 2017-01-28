<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CS207700.aspx.cs" Inherits="Page_CS207700" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CS.CarrierPluginMaint" PrimaryView="Plugin">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="Certify" CommitChanges="true" PostData="Page" Visible="false" />
            <px:PXDSCallbackCommand Name="CopyPaste" Visible="false" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Carrier Plug-in Summary" DataMember="Plugin" FilesIndicator="True" NoteIndicator="True" TemplateContainer=""
		TabIndex="5100">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSelector ID="edCarrierPluginID" runat="server" DataField="CarrierPluginID" DataSourceID="ds" />
			<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
			<px:PXSelector CommitChanges="True" ID="edPluginTypeName" runat="server" DataField="PluginTypeName" DataSourceID="ds" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
			<px:PXDropDown ID="edUnitType" runat="server" AllowNull="False" DataField="UnitType" />
			<px:PXSelector ID="edUOM" runat="server" DataField="UOM" DataSourceID="ds" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="365px">
		<Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
		<Items>
			<px:PXTabItem Text="Plug-in Parameters">
				<Template>
					<px:PXGrid ID="PXGridSettings" runat="server" DataSourceID="ds" AllowFilter="False" Width="100%" SkinID="DetailsInTab" Height="100%" MatrixMode="True">
						<Levels>
							<px:PXGridLevel DataMember="Details">
								<RowTemplate>
									<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXMaskEdit ID="edDetailID" runat="server" DataField="DetailID" />
									<px:PXTextEdit ID="edDescr" runat="server" AllowNull="False" DataField="Descr" />
									<px:PXTextEdit ID="edValue" runat="server" DataField="Value" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="DetailID" Width="90px" />
									<px:PXGridColumn AllowNull="False" DataField="Descr" Width="300px" />
									<px:PXGridColumn DataField="Value" Width="300px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<ActionBar DefaultAction="gridSchedules">
							<CustomItems>
								<px:PXToolBarButton Text="Prepare Certification Files" Key="cert">
								    <AutoCallBack Command="Certify" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Customer Accounts">
				<Template>
					<px:PXGrid ID="PXGridAccounts" runat="server" DataSourceID="ds" AllowFilter="False" Width="100%" SkinID="DetailsInTab" Height="100%" FastFilterFields="CustomerID,CustomerID_description,CarrierAccount"
						TabIndex="6900">
						<Levels>
							<px:PXGridLevel DataMember="CustomerAccounts">
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXCheckBox ID="chkIsActive" runat="server" Checked="True" DataField="IsActive" />
									<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" />
									<px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID" />
									<px:PXSegmentMask ID="edCustomerLocationID" runat="server" DataField="CustomerLocationID" AutoRefresh="True" />
									<px:PXTextEdit ID="edCarrierAccount" runat="server" DataField="CarrierAccount" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="IsActive" Label="Active" TextAlign="Center" Type="CheckBox" Width="40px" />
									<px:PXGridColumn DataField="CustomerID" Label="Customer" Width="100px" CommitChanges="true" />
									<px:PXGridColumn DataField="CustomerID_description" Width="200px" />
									<px:PXGridColumn DataField="CustomerLocationID" Width="100px" Label="Location" />
									<px:PXGridColumn DataField="CarrierAccount" Width="100px" />
									<px:PXGridColumn DataField="PostalCode" Width="100px">
									</px:PXGridColumn>
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="180" />
	</px:PXTab>
</asp:Content>
