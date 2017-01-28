<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM304000.aspx.cs"
    Inherits="Page_PM304000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.RegisterEntry" PrimaryView="Document"
        PageLoadBehavior="GoLastRecord">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="Release" CommitChanges="true" StartNewGroup="True"/>
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewAllocationSorce" Visible="False"/>
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewProject" Visible="False"/>
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewTask" Visible="False"/>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" DefaultControlID="edModule"
        Caption="Transaction Summary">
        <Parameters>
            <px:PXQueryStringParam Name="PMRegister.module" QueryStringField="Module" Type="String" OnLoadOnly="True" />
        </Parameters>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXDropDown ID="edModule" runat="server"  DataField="Module" SelectedIndex="-1" />
            <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" DataSourceID="ds" />
            <px:PXDropDown ID="edStatus" runat="server"  DataField="Status" Enabled="False" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXDropDown ID="edOrigDocType" runat="server" DataField="OrigDocType" Enabled="False" />
            <px:PXTextEdit ID="edOrigDocNbr" runat="server" DataField="OrigDocNbr" Enabled="False" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXNumberEdit ID="edQtyTotal" runat="server" DataField="QtyTotal" Enabled="False" />
            <px:PXNumberEdit ID="edBillableQtyTotal" runat="server" DataField="BillableQtyTotal" Enabled="False" />
            <px:PXNumberEdit ID="edAmtTotal" runat="server" DataField="AmtTotal" Enabled="False" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="Details" Caption="Transaction Details"
               SyncPosition="true">
        <Levels>
            <px:PXGridLevel DataMember="Transactions">
                <RowTemplate>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" AutoRefresh="True" />
                    <px:PXSegmentMask ID="edBranchID" runat="server" DataField="BranchID" />
                    <px:PXSelector Size="s" ID="edFinPeriodID" runat="server" DataField="FinPeriodID" />
                    <px:PXSegmentMask Size="xs" ID="edTaskID" runat="server" DataField="TaskID" AutoRefresh="True" />
                    <px:PXSelector Size="s" ID="edBatchNbr" runat="server" DataField="BatchNbr" AllowEdit="True" />
                    <px:PXSegmentMask ID="edAccountGroupID" runat="server" DataField="AccountGroupID" />
                    <px:PXCheckBox ID="chkBilled" runat="server" DataField="Billed" />
                    <px:PXSegmentMask ID="edResourceID" runat="server" DataField="ResourceID" />
                    <px:PXSelector ID="edBAccountID" runat="server" DataField="BAccountID" />
                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" />
                    <px:PXSelector ID="edInventoryID" runat="server" DataField="InventoryID" />
                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                    <px:PXSelector ID="edUOM" runat="server" DataField="UOM" />
                    <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True" />
                    <px:PXNumberEdit Size="xs" ID="edQty" runat="server" DataField="Qty" />
                    <px:PXCheckBox ID="chkAllocated" runat="server" DataField="Allocated" />
                    <px:PXCheckBox ID="chkBillable" runat="server" Checked="True" DataField="Billable" />
                    <px:PXCheckBox ID="chkReleased" runat="server" DataField="Released" />
                    <px:PXNumberEdit ID="edBillableQty" runat="server" DataField="BillableQty" />
                    <px:PXNumberEdit ID="edUnitRate" runat="server" DataField="UnitRate" />
                    <px:PXNumberEdit ID="edAmount" runat="server" DataField="Amount" />
                    <px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID" AutoRefresh="True"/>
                    <px:PXSegmentMask ID="edSubID" runat="server" DataField="SubID" />
                    <px:PXSegmentMask ID="edOffsetAccountID" runat="server" DataField="OffsetAccountID" AutoRefresh="True"/>
                    <px:PXSegmentMask ID="edOffsetSubID" runat="server" DataField="OffsetSubID" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="BranchID" Label="Branch" Width="81px" />
                    <px:PXGridColumn DataField="ProjectID" Label="Project" Width="108px" AutoCallBack="true" LinkCommand="ViewProject" />
                    <px:PXGridColumn DataField="TaskID" Label="Task" Width="108px" LinkCommand="ViewTask" />
                    <px:PXGridColumn DataField="AccountGroupID" Label="Account Group" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="ResourceID" Label="Resource" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="BAccountID" Label="Customer/Vendor" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="LocationID" Label="Location" Width="108px" />
                    <px:PXGridColumn DataField="InventoryID" Label="InventoryID" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="Description" Label="Description" Width="108px" />
                    <px:PXGridColumn DataField="UOM" Label="UOM" Width="63px" />
                    <px:PXGridColumn  DataField="Qty" Label="Qty" TextAlign="Right" Width="63px" AutoCallBack="true" />
                    <px:PXGridColumn  DataField="Billable" Label="Billable" TextAlign="Center" Type="CheckBox" Width="63px" />
                    <px:PXGridColumn  DataField="BillableQty" Label="BillableQty" TextAlign="Right" Width="63px" AutoCallBack="true" />
                    <px:PXGridColumn  DataField="UnitRate" Label="UnitRate" TextAlign="Right" Width="63px" />
                    <px:PXGridColumn  DataField="Amount" Label="Amount" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn DataField="StartDate" Width="90px" Visible="false" />
                    <px:PXGridColumn DataField="EndDate" Width="90px" Visible="false" />
                    <px:PXGridColumn DataField="AccountID" Label="Account" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="SubID" Label="Subaccount" Width="108px" />
                    <px:PXGridColumn DataField="OffsetAccountID" Label="Offset Account" Width="108px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="OffsetSubID" Label="Offset SubAccount" Width="108px" />
                    <px:PXGridColumn DataField="Date" AutoCallBack="true" />
                    <px:PXGridColumn DataField="FinPeriodID" Width="108px" />
                    <px:PXGridColumn DataField="BatchNbr" Width="108px" />
                    <px:PXGridColumn DataField="EarningType" Width="100px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="OvertimeMultiplier" Width="70px" />
                    <px:PXGridColumn  DataField="UseBillableQty" Label="UseBillableQty" TextAlign="Center" Type="CheckBox"  Width="140px" />
                    <px:PXGridColumn  DataField="Allocated" Label="Allocated" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn  DataField="Released" Label="Released" TextAlign="Center" Type="CheckBox" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
            <CustomItems>
                <px:PXToolBarButton Text="Transaction" Key="cmdViewAllocationSorce">
                    <AutoCallBack Command="ViewAllocationSorce" Target="ds" />
                </px:PXToolBarButton>
            </CustomItems>
        </ActionBar>
        <Mode InitNewRow="True"></Mode>
    </px:PXGrid>
</asp:Content>
