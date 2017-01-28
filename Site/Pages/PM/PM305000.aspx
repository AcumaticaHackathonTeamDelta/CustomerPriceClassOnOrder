<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM305000.aspx.cs"
    Inherits="Page_PM305000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.ProjectBalanceByPeriodEntry"
        PrimaryView="Filter" BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="true" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection"
        EmailingGraph="">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edAccountGroupID" runat="server" DataField="AccountGroupID" DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" DataSourceID="ds" AutoRefresh="True" />
            <px:PXSelector CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" DataSourceID="ds" />
			<px:PXLayoutRule ID="LayoutRule_QTY" runat="server" ControlSize="S" LabelsWidth="XS" StartColumn="True" GroupCaption="Quantity" StartGroup="True" />
			<px:PXNumberEdit ID="edPlanQty" runat="server" DataField="Qty" Enabled="False" />
			<px:PXNumberEdit ID="edRevlQty" runat="server" DataField="RevisedQty" Enabled="False" />
			<px:PXNumberEdit ID="edActuallQty" runat="server" DataField="ActualQty" Enabled="False" />
			<px:PXLayoutRule ID="LayoutRule_Amount" runat="server" ControlSize="S" StartColumn="True" LabelsWidth="S" SuppressLabel="True" GroupCaption="Amount" StartGroup="True" />
			<px:PXNumberEdit ID="edPlanAm" runat="server" DataField="Amount" Enabled="False" />
			<px:PXNumberEdit ID="edRevnAm" runat="server" DataField="RevisedAmount" Enabled="False" />
			<px:PXNumberEdit ID="edActualAm" runat="server" DataField="ActualAmount" Enabled="False" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="341px">
        <Items>
            <px:PXTabItem Text="Project Balance">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="Items" DataKeyNames="AccountGroupID,ProjectID,ProjectTaskID,PeriodID,InventoryID">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXMaskEdit ID="edPeriodID" runat="server" DataField="PeriodID" />
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
                                    <px:PXGridColumn DataField="PeriodID" Label="Fin. Period" Width="63px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="UOM" Label="UOM" Width="63px" />
                                    <px:PXGridColumn DataField="Rate" Label="Rate" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn DataField="Qty" Label="Qty." TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="Amount" Label="Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="RevisedQty" Label="Revised Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="RevisedAmount" Label="Revised Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ActualQty" Label="Actual Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ActualAmount" Label="Actual Amount" TextAlign="Right" Width="81px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
