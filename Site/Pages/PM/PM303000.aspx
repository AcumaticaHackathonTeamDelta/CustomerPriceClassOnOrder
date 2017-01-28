<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM303000.aspx.cs"
    Inherits="Page_PM303000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.ProjectBalanceEntry" PrimaryView="Filter"
        BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="true" />
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewBalance" Visible="False" />
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewTransactions" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edAccountGroupID" runat="server" DataField="AccountGroupID" />
            <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" />
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
                            <px:PXGridLevel DataMember="Items" DataKeyNames="LineNbr">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" />
                                    <px:PXSelector ID="edInventoryID" runat="server" DataField="InventoryID" AutoRefresh="True" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXNumberEdit ID="edQty" runat="server" DataField="Qty" />
                                    <px:PXSelector ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True"/>
                                    <px:PXNumberEdit ID="edRate" runat="server" DataField="Rate" />
                                    <px:PXNumberEdit ID="edAmount" runat="server" DataField="Amount" />
                                    <px:PXNumberEdit ID="edRevisedQty" runat="server" DataField="RevisedQty" />
                                    <px:PXNumberEdit ID="edRevisedAmount" runat="server" DataField="RevisedAmount" />
                                    <px:PXNumberEdit ID="edActualQty" runat="server" DataField="ActualQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edActualAmount" runat="server" DataField="ActualAmount" Enabled="False" />
                                    </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ProjectTaskID" Label="Task" Width="108px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="InventoryID" Label="Inventory ID" Width="108px" />
                                    <px:PXGridColumn DataField="Description" Width="180px"/>
                                    <px:PXGridColumn  DataField="Qty" Label="Qty." TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="UOM" Label="UOM" Width="63px" />
                                    <px:PXGridColumn  DataField="Rate" Label="Rate" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn  DataField="Amount" Label="Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn  DataField="RevisedQty" Label="Revised Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn  DataField="RevisedAmount" Label="Revised Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn   DataField="ActualQty" Label="Actual Qty" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn   DataField="ActualAmount" Label="Actual Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="TaskStatus" Width="81px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar DefaultAction="cmdViewBalance">
                            <CustomItems>
                                <px:PXToolBarButton Text="Balance Details" Key="cmdViewBalance" Visible="False">
                                    <AutoCallBack Command="ViewBalance" Target="ds"/>
                                    <PopupCommand Command="Refresh" Target="grid"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Transactions" Key="cmdViewBalance" Visible="False">
                                    <AutoCallBack Command="ViewTransactions" Target="ds"/>
                                    <PopupCommand Command="Refresh" Target="grid"/>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
