<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM302000.aspx.cs" Inherits="Page_PM302000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Task" TypeName="PX.Objects.PM.ProjectTaskEntry" BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="true" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Delete" PopupVisible="true" ClosePopup="true" />
            <px:PXDSCallbackCommand Name="First" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewBalance" Visible="False" />
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewTransactions" Visible="False" />
            <px:PXDSCallbackCommand Name="AutoBudget" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Task" LinkPage="" Caption="Task Summary" FilesIndicator="True"
        NoteIndicator="True">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" DataSourceID="ds">
            </px:PXSegmentMask>
            <px:PXSegmentMask ID="edTaskCD" runat="server" DataField="TaskCD" AutoRefresh="True" DataSourceID="ds">
            </px:PXSegmentMask>
            <px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID" Enabled="False" DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" DataSourceID="ds" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSelector ID="edRateTableID" runat="server" DataField="RateTableID" DataSourceID="ds" />
            <px:PXSelector CommitChanges="True" ID="edAllocationID" runat="server" DataField="AllocationID" DataSourceID="ds" />
            <px:PXSelector CommitChanges="True" ID="edBillingID" runat="server" DataField="BillingID" DataSourceID="ds" />
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" DataField="Status" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="341px" DataSourceID="ds" DataMember="TaskProperties" LinkPage="">
        <Items>
            <px:PXTabItem Text="General Settings">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
                    <px:PXDateTimeEdit ID="edPlannedStartDate" runat="server" DataField="PlannedStartDate" CommitChanges="True" />
                    <px:PXDateTimeEdit ID="edPlannedEndDate" runat="server" DataField="PlannedEndDate" CommitChanges="True" />
                    <px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" CommitChanges="True" />
                    <px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" CommitChanges="True" />
                    <px:PXDropDown ID="edCompletedPctMethod" runat="server" DataField="CompletedPctMethod" CommitChanges="True" />
                    <px:PXNumberEdit ID="edCompletedPct" runat="server" DataField="CompletedPct" />
                    <px:PXSegmentMask ID="edDefaultAccountID" runat="server" DataField="DefaultAccountID" />
                    <px:PXSegmentMask ID="edDefaultSubID" runat="server" DataField="DefaultSubID" />
                     <px:PXSegmentMask ID="PXSegmentMask1" runat="server" DataField="DefaultAccrualAccountID" />
                    <px:PXSegmentMask ID="PXSegmentMask2" runat="server" DataField="DefaultAccrualSubID" />
                    <px:PXDropDown ID="edBillingOption" runat="server" DataField="BillingOption" />
                    <px:PXSelector ID="edApproverID" runat="server" DataField="ApproverID" AutoRefresh="True" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Visibility Settings" />
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkVisibleInGL" runat="server" DataField="VisibleInGL" />
                    <px:PXCheckBox ID="chkVisibleInAP" runat="server" DataField="VisibleInAP" />
                    <px:PXCheckBox ID="chkVisibleInAR" runat="server" DataField="VisibleInAR" />
                    <px:PXCheckBox ID="chkVisibleInSO" runat="server" DataField="VisibleInSO" />
                    <px:PXCheckBox ID="chkVisibleInPO" runat="server" DataField="VisibleInPO" />
                    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" />
                    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkVisibleInEP" runat="server" DataField="VisibleInEP" />
                    <px:PXCheckBox ID="chkVisibleInIN" runat="server" DataField="VisibleInIN" />
                    <px:PXCheckBox ID="chkVisibleInCA" runat="server" DataField="VisibleInCA" />
                    <px:PXCheckBox ID="chkVisibleInCR" runat="server" DataField="VisibleInCR" />
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Budget">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="DetailsInTab" SyncPosition="True">
                        <Levels>
                            <px:PXGridLevel DataMember="ProjectStatus" DataKeyNames="LineNbr">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSelector Size="s" ID="edInventoryID" runat="server" DataField="InventoryID" AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edAccountGroupID" runat="server" DataField="AccountGroupID" />
                                    <px:PXCheckBox ID="chkIsProduction" runat="server" DataField="IsProduction" />
                                    <px:PXDropDown Size="xs" ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXNumberEdit ID="edQty" runat="server" DataField="Qty" />
                                    <px:PXSelector ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True" />
                                    <px:PXNumberEdit ID="edRate" runat="server" DataField="Rate" />
                                    <px:PXNumberEdit ID="edAmount" runat="server" DataField="Amount" />
                                    <px:PXNumberEdit ID="edRevisedQty" runat="server" DataField="RevisedQty" />
                                    <px:PXNumberEdit ID="edRevisedAmount" runat="server" DataField="RevisedAmount" />
                                    <px:PXNumberEdit ID="edActualQty" runat="server" DataField="ActualQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edActualAmount" runat="server" DataField="ActualAmount" Enabled="False" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Type" Label="Type" RenderEditorText="True" Width="81px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="AccountGroupID" Label="Account Group" Width="108px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="InventoryID" Label="Inventory ID" Width="108px" />
                                    <px:PXGridColumn DataField="Description" Width="180px">
                                        <Header Text="Description"></Header>
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="Qty" Label="Qty." TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="UOM" Label="UOM" Width="63px" />
                                    <px:PXGridColumn DataField="Rate" Label="Rate" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn DataField="Amount" Label="Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="RevisedQty" Label="Revised Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="RevisedAmount" Label="Revised Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ActualQty" Label="Actual Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ActualAmount" Label="Actual Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="IsProduction" Label="Production" AutoCallBack="True" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="Performance" TextAlign="Right" Width="100px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar DefaultAction="cmdViewBalance">
                            <CustomItems>
                                <px:PXToolBarButton Text="Balance Details" Key="cmdViewBalance" Visible="False">
                                    <AutoCallBack Command="ViewBalance" Target="ds" />
                                    <PopupCommand Command="Refresh" Target="grid" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Transactions" Key="cmdViewBalance" Visible="False">
                                    <AutoCallBack Command="ViewTransactions" Target="ds" />
                                    <PopupCommand Command="Refresh" Target="grid" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Recurring Billing">
                <Template>
                    <px:PXGrid ID="GridBillingItems" runat="server" DataSourceID="ds" Width="100%" Height="100%" SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="BillingItems" DataKeyNames="ContractItemID">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edInventoryID2" runat="server" DataField="InventoryID" />
                                    <px:PXDropDown ID="edResetUsage" runat="server" DataField="ResetUsage" SelectedIndex="-1" />
                                    <px:PXNumberEdit ID="edCuryItemFee" runat="server" DataField="CuryItemFee" />
                                    <px:PXTextEdit ID="edDescription2" runat="server" DataField="Description" />
                                    <px:PXNumberEdit ID="edIncluded" runat="server" DataField="Included" />
                                    <px:PXSelector ID="edUOM2" runat="server" DataField="UOM" />
                                    <px:PXNumberEdit ID="edUsed" runat="server" DataField="Used" Enabled="False" />
                                    <px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID" />
                                    <px:PXSegmentMask Size="s" ID="edSubMask" runat="server" DataField="SubMask" DataMember="_PMRECBILL_Segments_" />
                                    <px:PXSegmentMask Size="s" ID="edSubID" runat="server" DataField="SubID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="InventoryID" Label="Non-Stock Item" Width="108px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="Description" Label="Description" Width="200px" />
                                    <px:PXGridColumn DataField="CuryItemFee" Label="Item Fee" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="AccountSource" RenderEditorText="True" Width="90px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="SubMask" Width="108px" RenderEditorText="True" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="AccountID" Label="Account" Width="63px" />
                                    <px:PXGridColumn DataField="SubID" Label="Subaccount" Width="108px" />
                                    <px:PXGridColumn DataField="ResetUsage" RenderEditorText="True" Width="108px" />
                                    <px:PXGridColumn DataField="Included" Label="Included" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="UOM" Label="UOM" Width="63px" />
                                    <px:PXGridColumn DataField="Used" Label="Used" TextAlign="Right" Width="81px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Activity History" LoadOnDemand="true">
                <Template>
                    <pxa:PXGridWithPreview ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
                        FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Details" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray" PreviewPanelStyle="z-index: 100; background-color: Window"
                        PreviewPanelSkinID="Preview" BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">
                        <ActionBar DefaultAction="cmdViewActivity" PagerVisible="False">
                            <Actions>
                                <AddNew Enabled="False" />
                                <Delete Enabled="False" />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Key="cmdAddTask">
                                    <AutoCallBack Command="NewTask" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEvent">
                                    <AutoCallBack Command="NewEvent" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEmail">
                                    <AutoCallBack Command="NewMailActivity" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddActivity">
                                    <AutoCallBack Command="NewActivity" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdViewActivity" Visible="false">
                                    <ActionBar MenuVisible="false" />
                                    <AutoCallBack Command="ViewActivity" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Activities">
                                <RowTemplate>
                					<px:PXTimeSpan TimeMode="True" ID="edTimeSpent" runat="server" DataField="TimeSpent" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edOvertimeSpent" runat="server" DataField="OvertimeSpent" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edTimeBillable" runat="server" DataField="TimeBillable" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edOvertimeBillable" runat="server" DataField="OvertimeBillable" InputMask="hh:mm" MaxHours="99" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsCompleteIcon" Width="21px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="PriorityIcon" Width="21px" AllowShowHide="False" AllowResize="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassIcon" Width="31px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassInfo" />
                                    <px:PXGridColumn DataField="RefNoteID" Visible="false" AllowShowHide="False" />
                                    <px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="297px" />
                                    <px:PXGridColumn DataField="UIStatus" />
                                    <px:PXGridColumn DataField="Released" Width="80px" />
                                    <px:PXGridColumn DataField="StartDate" Width="108px" />
                                    <px:PXGridColumn DataField="CategoryID" />
                                    <px:PXGridColumn DataField="IsBillable" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="TimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="OvertimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="TimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="OvertimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="false" SyncVisible="False" SyncVisibility="False" Width="108px" />
                                    <px:PXGridColumn DataField="WorkgroupID" Width="90px" />
                                    <px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" Width="150px" DisplayMode="Text"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <CallbackCommands>
                            <Refresh CommitChanges="True" PostData="Page" />
                        </CallbackCommands>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <GridMode AllowAddNew="False" AllowUpdate="False" />
                        <PreviewPanelTemplate>
                            <px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine" MaxLength="50" Width="100%" Height="100%" SkinID="Label" >
                                      <AutoSize Container="Parent" Enabled="true" />
                                </px:PXHtmlView>
                        </PreviewPanelTemplate>
                    </pxa:PXGridWithPreview>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" Width="100%" Height="100%" SkinID="DetailsInTab" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <Columns>
                                    <px:PXGridColumn DataField="AttributeID" TextAlign="Left" Width="220px" AllowShowHide="False" TextField="AttributeID_description" />
    								<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" Width="75px" />
                                    <px:PXGridColumn DataField="Value" Width="148px" RenderEditorText="True" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
