<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="IN201000.aspx.cs"
    Inherits="Page_IN201000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%" TypeName="PX.Objects.IN.INItemClassMaint"
        PrimaryView="itemclass">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="ViewRestrictionGroups" Visible="False" />
            <px:PXDSCallbackCommand Name="ResetGroup" StartNewGroup="true" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="ShowDetails" DependOnGrid="AttributesGrid" Visible="False" />
            <px:PXDSCallbackCommand Name="Action" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ViewGroupDetails" Visible="False" DependOnGrid="grid" />
        </CallbackCommands>
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="itemclass"
        Caption="Item Class Summary" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="true" ActivityField="NoteActivity"
        DefaultControlID="edItemClassID">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSelector ID="edItemClassID" runat="server" DataField="ItemClassID" />
            <px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="530px" DataSourceID="ds" DataMember="itemclasssettings">
        <Items>
            <px:PXTabItem Text="General Settings">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
                    <px:PXCheckBox CommitChanges="True" ID="chkStkItem" runat="server" DataField="StkItem" />
                    <px:PXCheckBox ID="chkNegQty" runat="server" DataField="NegQty" />
                    <px:PXDropDown ID="edItemType" runat="server" DataField="ItemType" />
                    <px:PXDropDown ID="edValMethod" runat="server" AllowNull="False" DataField="ValMethod" SelectedIndex="1" />
                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AllowEdit="True" AutoRefresh="True"/>
                    <px:PXSelector ID="edPostClassID" runat="server" DataField="PostClassID" AllowEdit="True" />
                    <px:PXSelector ID="edLotSerClassID" runat="server" DataField="LotSerClassID" AllowEdit="True" />
                    <px:PXSelector ID="edPriceClassID" runat="server" DataField="PriceClassID" AllowEdit="True" />
                    <px:PXSegmentMask ID="edDfltSiteID" runat="server" DataField="DfltSiteID" AllowEdit="True" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Availability Calculation" />
                    <px:PXPanel ID="PXPanel1" runat="server" RenderSimple="True" RenderStyle="Simple">
                        <px:PXLayoutRule runat="server" StartColumn="True" />
                        <px:PXCheckBox ID="chkInclQtyINIssues" runat="server" AlignLeft="True" DataField="InclQtyINIssues" />
                        <px:PXCheckBox ID="chkInclQtySOPrepared" runat="server" AlignLeft="True" DataField="InclQtySOPrepared" />
                        <px:PXCheckBox ID="chkInclQtySOBooked" runat="server" AlignLeft="True" DataField="InclQtySOBooked" />
                        <px:PXCheckBox ID="chkInclQtySOShipped" runat="server" AlignLeft="True" DataField="InclQtySOShipped" />
                        <px:PXCheckBox ID="chkInclQtySOShipping" runat="server" AlignLeft="True" DataField="InclQtySOShipping" />
                        <px:PXCheckBox ID="chkInclQtyINAssemblyDemand" runat="server" AlignLeft="True" DataField="InclQtyINAssemblyDemand" />
                        <px:PXCheckBox ID="chkInclQtySOBackOrdered" runat="server" AlignLeft="True" DataField="InclQtySOBackOrdered" />
                        <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" />
                        <px:PXCheckBox ID="chkInclQtyINReceipts" runat="server" AlignLeft="True" DataField="InclQtyINReceipts" />
                        <px:PXCheckBox ID="chkInclQtyInTransit" runat="server" AlignLeft="True" DataField="InclQtyInTransit" />
                        <px:PXCheckBox ID="chkInclQtyPOReceipts" runat="server" AlignLeft="True" DataField="InclQtyPOReceipts" />
                        <px:PXCheckBox ID="chkInclQtyPOPrepared" runat="server" AlignLeft="True" DataField="InclQtyPOPrepared" />
                        <px:PXCheckBox ID="chkInclQtyPOOrders" runat="server" AlignLeft="True" DataField="InclQtyPOOrders" />
                        <px:PXCheckBox ID="chkInclQtyINAssemblySupply" runat="server" AlignLeft="True" DataField="InclQtyINAssemblySupply" />
                        <px:PXCheckBox ID="chkInclQtySOReverse" runat="server" AlignLeft="True" DataField="InclQtySOReverse" />
                    </px:PXPanel>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Unit of Measure" />
                    <px:PXPanel ID="PXPanel2" runat="server" RenderSimple="True" RenderStyle="Simple" Width="400px">
                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                        <px:PXSelector CommitChanges="True" ID="edBaseUnit" runat="server" DataField="BaseUnit" AllowEdit="True" />
                        <px:PXSelector CommitChanges="True" ID="edSalesUnit" runat="server" DataField="SalesUnit" AllowEdit="True" />
                        <px:PXSelector CommitChanges="True" ID="edPurchaseUnit" runat="server" DataField="PurchaseUnit" AllowEdit="True" />
                        <px:PXGrid ID="gridUnits" runat="server" DataSourceID="ds" Height="132" SkinID="ShortList" >
                            <Mode InitNewRow="True" />
                            <Levels>
                                <px:PXGridLevel DataMember="classunits" DataKeyNames="UnitType,ItemClassID,InventoryID,ToUnit,FromUnit">
                                    <RowTemplate>
                                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                        <px:PXDropDown ID="edUnitType" runat="server" DataField="UnitType" SelectedIndex="2" />
                                        <px:PXTextEdit ID="edItemClassID2" runat="server" DataField="ItemClassID" />
                                        <px:PXNumberEdit ID="edInventoryID" runat="server" DataField="InventoryID" />
                                        <px:PXMaskEdit ID="edFromUnit" runat="server" DataField="FromUnit" InputMask=">aaaaaa" />
                                        <px:PXMaskEdit ID="edSampleToUnit" runat="server" DataField="SampleToUnit" InputMask=">aaaaaa" />
                                        <px:PXDropDown ID="edUnitMultDiv" runat="server" DataField="UnitMultDiv" />
                                        <px:PXNumberEdit ID="edUnitRate" runat="server" DataField="UnitRate" /></RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn AllowNull="False" DataField="UnitType" Width="99px" Visible="False" RenderEditorText="True" />
                                        <px:PXGridColumn AllowNull="False" DataField="ItemClassID" Width="36px" Visible="False" />
                                        <px:PXGridColumn AllowNull="False" DataField="InventoryID" Visible="False" TextAlign="Right" Width="54px" />
                                        <px:PXGridColumn DataField="FromUnit" Width="72px" />
                                        <px:PXGridColumn AllowNull="False" DataField="UnitMultDiv" Width="90px" RenderEditorText="True" />
                                        <px:PXGridColumn AllowNull="False" DataField="UnitRate" TextAlign="Right" Width="108px" />
                                        <px:PXGridColumn DataField="SampleToUnit" Width="72px" />
                                    </Columns>
                                    <Layout FormViewHeight="" />
                                </px:PXGridLevel>
                            </Levels>
                            <Layout ColumnsMenu="False" />
                        </px:PXGrid>
                    </px:PXPanel>
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Price Management" LabelsWidth="S" ControlSize="XM" />
                    <px:PXTreeSelector ID="edPriceWorkgroupID" runat="server" DataField="PriceWorkgroupID" TreeDataMember="_EPCompanyTree_Tree_"
                        TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0" ShowRootNode="False">
                        <DataBindings>
                            <px:PXTreeItemBinding TextField="Description" ValueField="Description" />
                        </DataBindings>
                    </px:PXTreeSelector>
                    <px:PXSelector ID="edPriceManagerID" runat="server" DataField="PriceManagerID" AllowEdit="True" />
                    <px:PXNumberEdit ID="edMinGrossProfitPct" runat="server" DataField="MinGrossProfitPct" />
                    <px:PXNumberEdit ID="edMarkupPct" runat="server" DataField="MarkupPct" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Replenishment Settings" RepaintOnDemand="False" >
                <Template>
					 <px:PXFormView ID="PXFormView3" runat="server" DataMember="itemclasssettings" DataSourceID="ds" Style="z-index: 100" Width="100%">
                        <ContentStyle BackColor="Transparent" BorderStyle="None">
                        </ContentStyle>
                        <Template>
                            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="true" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXDropDown ID="edDemandCalculation" runat="server" DataField="DemandCalculation" AllowNull="false" />
                        </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="repGrid" runat="server" Height="250px" Width="100%" DataSourceID="ds" SkinID="DetailsInTab">
                        <Mode InitNewRow="true" />
                        <Levels>
                            <px:PXGridLevel DataMember="replenishment">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector ID="edReplenishmentClassID" runat="server" DataField="ReplenishmentClassID" AllowEdit="true" />
                                    <px:PXDropDown ID="edReplenishmentMethod" runat="server" AllowNull="False" DataField="ReplenishmentMethod" />
                                    <px:PXDropDown ID="edReplenishmentSource" runat="server" AllowNull="False" DataField="ReplenishmentSource" />
                                    <px:PXSegmentMask ID="edReplenishmentSourceSiteID" runat="server" DataField="ReplenishmentSourceSiteID" />									
                                    <px:PXSelector ID="edReplenishmentPolicyID" runat="server" DataField="ReplenishmentPolicyID" AllowEdit="true" />
									<px:PXNumberEdit ID="edTransferLeadTime" runat="server" AllowNull="false" Size="xxs" DataField="TransferLeadTime" />
									<px:PXNumberEdit ID="edTransferERQ" runat="server" AllowNull="false" Size="xxs" DataField="TransferERQ" />									
									<px:PXNumberEdit ID="edHistoryDepth" runat="server" AllowNull="false" Size="xxs" DataField="HistoryDepth" />
                                    <px:PXDateTimeEdit ID="edLaunchDate" runat="server" DataField="LaunchDate" />
                                    <px:PXDateTimeEdit ID="edTerminationDate" runat="server" DataField="TerminationDate" />
									<px:PXNumberEdit ID="edServiceLevelPct" runat="server" AllowNull="false" DataField="ServiceLevelPct"/>
                                </RowTemplate>
                                <Columns>
									<px:PXGridColumn DataField="ReplenishmentClassID" DisplayFormat="&gt;aaaaaaaaaa" AutoCallBack="true" />
                                    <px:PXGridColumn DataField="ReplenishmentPolicyID" DisplayFormat="&gt;aaaaaaaaaa" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="ReplenishmentSource" AutoCallBack="true" Width="140px" />
									<px:PXGridColumn AllowNull="False" DataField="ReplenishmentMethod" AutoCallBack="true" Width="140px" />
                                    <px:PXGridColumn DataField="ReplenishmentSourceSiteID" DisplayFormat="&gt;AAAAAAAAAA" Width="140px" />
									<px:PXGridColumn DataField="TransferLeadTime" Width="120px" />
									<px:PXGridColumn DataField="TransferERQ" Width="100px" />
									<px:PXGridColumn DataField="ForecastModelType" Width="140px"/>
									<px:PXGridColumn DataField="ForecastPeriodType" Width="50px"/>
									<px:PXGridColumn DataField="HistoryDepth" Width="50px" AllowNull = "False" />
                                    <px:PXGridColumn DataField="LaunchDate" Width="90px" />
                                    <px:PXGridColumn DataField="TerminationDate" Width="100px" />
									<px:PXGridColumn DataField="ServiceLevelPct" Width="100px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Subitem / Restriction Groups">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" AdjustPageSize="Auto"
                        AllowSearch="True" SkinID="Details" BorderWidth="0px">
                        <ActionBar>
                            <Actions>
                                <NoteShow Enabled="false" />
                            </Actions>
                            <CustomItems>
							    <px:PXToolBarButton Text="Group Details" CommandSourceID="ds" CommandName="ViewGroupDetails"/>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Groups">
                                <Mode AllowAddNew="False" AllowDelete="False" />
                                <Columns>
                                    <px:PXGridColumn AllowNull="False" DataField="Included" TextAlign="Center" Type="CheckBox" Width="40px" RenderEditorText="True"
                                        AllowCheckAll="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="GroupName" Width="150px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="SpecificType" Width="150px" Type="DropDownList" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="Description" Width="200px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Active" TextAlign="Center" Type="CheckBox" Width="40px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="GroupType" Label="Visible To Entities" RenderEditorText="True"
                                        Width="171px" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox SuppressLabel="True" ID="chkSelected" runat="server" DataField="Included" />
                                    <px:PXSelector ID="edGroupName" runat="server" DataField="GroupName" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXCheckBox SuppressLabel="True" ID="chkActive" runat="server" Checked="True" DataField="Active" />
                                    <px:PXDropDown ID="edGroupType" runat="server" AllowNull="False" DataField="GroupType" Enabled="False" />
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="AttributesGrid" runat="server" SkinID="Details" ActionsPosition="Top" DataSourceID="ds" Width="100%" BorderWidth="0px"
                        Style="left: 0px; top: 0px; height: 13px">
                        <Levels>
                            <px:PXGridLevel DataMember="Mapping">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector ID="edCRAttributeID" runat="server" DataField="AttributeID" AutoRefresh="true" FilterByAllFields="True" />
                                    <px:PXTextEdit ID="edDescription2" runat="server" AllowNull="False" DataField="Description" />
                                    <px:PXCheckBox ID="chkRequired" runat="server" DataField="Required" />
                                    <px:PXNumberEdit ID="edSortOrder" runat="server" DataField="SortOrder" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AttributeID" DisplayFormat="&gt;aaaaaaaaaa" Width="81px" AutoCallBack="True" LinkCommand = "ShowDetails" />
                                    <px:PXGridColumn AllowNull="False" DataField="Description" Width="351px" />
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Required" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="ControlType" Type="DropDownList" Width="63px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Show Details" Tooltip="Show Attribute Details">
                                    <AutoCallBack Command="ShowDetails" Target="ds">
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
