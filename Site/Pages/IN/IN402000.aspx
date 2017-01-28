<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="IN402000.aspx.cs" Inherits="Page_IN402000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PageLoadBehavior="PopulateSavedValues" PrimaryView="Filter" TypeName="PX.Objects.IN.InventoryAllocDetEnq" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds"
        Style="z-index: 100" Width="100%" Caption="Selection"
        CaptionAlign="Justify" DataMember="Filter" DefaultControlID="edInventoryID"
        LinkPage="" TabIndex="3100" AllowCollapse="true">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />

            <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True"
                DataSourceID="ds">
            </px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True"
                DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True"
                DataSourceID="ds" />

            <px:PXSegmentMask CommitChanges="True" ID="edSubItemCD" runat="server" DataField="SubItemCD" AutoRefresh="True"
                DataSourceID="ds" />
            <px:PXTextEdit CommitChanges="True" ID="edLotSerialNbr" runat="server" AllowNull="False" DataField="LotSerialNbr" />
            <px:PXTextEdit ID="edBaseUnit" runat="server" AllowNull="False" DataField="BaseUnit" Enabled="False" />
            <px:PXNumberEdit ID="edQtyOnHand" runat="server" DataField="QtyOnHand" Enabled="False" />
            <px:PXNumberEdit ID="edQtyTotalAddition" runat="server" DataField="QtyTotalAddition" Enabled="False" />
            <px:PXNumberEdit ID="edQtTotalDeduction" runat="server" DataField="QtyTotalDeduction" Enabled="False" />
            <px:PXNumberEdit ID="edQtyAvail" runat="server" DataField="QtyAvail" Enabled="False" />
            <px:PXNumberEdit ID="edQtyHardAvail" runat="server" DataField="QtyHardAvail" Enabled="False" />
            <px:PXNumberEdit ID="edQtyNotAvail" runat="server" DataField="QtyNotAvail" Enabled="False" />
            <px:PXNumberEdit ID="edQtyExpired" runat="server" DataField="QtyExpired" Enabled="False" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="S" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtyPOPrepared" runat="server" DataField="QtyPOPrepared" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyPOPrepared" runat="server" DataField="InclQtyPOPrepared" Enabled="False" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXNumberEdit ID="edQtyPOOrders" runat="server" DataField="QtyPOOrders" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyPOOrders" runat="server" DataField="InclQtyPOOrders" Enabled="False" FalseValue="False" TrueValue="True" AutoPostBack="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXNumberEdit ID="edQtyPOReceipts" runat="server" DataField="QtyPOReceipts" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyPOReceipts" runat="server" Enabled="False" DataField="InclQtyPOReceipts" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXNumberEdit ID="edQtyINReceipts" runat="server" DataField="QtyINReceipts" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyINReceipts" runat="server" Enabled="False" DataField="InclQtyINReceipts" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXNumberEdit ID="edQtyInTransit" runat="server" DataField="QtyInTransit" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyInTransit" runat="server" Enabled="False" DataField="InclQtyInTransit" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />
            <px:PXNumberEdit ID="edQtyInTransitToSO" runat="server" DataField="QtyInTransitToSO" Enabled="False" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtyINAssemblySupply" runat="server" DataField="QtyINAssemblySupply" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyINAssemblySupply" runat="server" DataField="InclQtyINAssemblySupply" Enabled="False" />
            <px:PXLayoutRule runat="server" />

            <px:PXNumberEdit ID="edQtyPOFixedPrepared" runat="server" DataField="QtyPOFixedPrepared" Enabled="False" />
            <px:PXNumberEdit ID="edQtyPOFixedOrders" runat="server" DataField="QtyPOFixedOrders" Enabled="False" />
            <px:PXNumberEdit ID="edQtyPOFixedReceipts" runat="server" DataField="QtyPOFixedReceipts" Enabled="False" />
            <px:PXNumberEdit ID="edQtyPODropShipPrepared" runat="server" DataField="QtyPODropShipPrepared" Enabled="False" />
            <px:PXNumberEdit ID="edQtyPODropShipOrders" runat="server" DataField="QtyPODropShipOrders" Enabled="False" />
            <px:PXNumberEdit ID="edQtyPODropShipReceipts" runat="server" DataField="QtyPODropShipReceipts" Enabled="False" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="S" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtySOPrepared" runat="server" DataField="QtySOPrepared" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtySOPrepared" runat="server" DataField="InclQtySOPrepared" Enabled="False" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtySOBooked" runat="server" DataField="QtySOBooked" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtySOBooked" runat="server" Enabled="False" DataField="InclQtySOBooked" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtySOShipping" runat="server" DataField="QtySOShipping" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtySOShipping" runat="server" Enabled="False" DataField="InclQtySOShipping" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtySOShipped" runat="server" DataField="QtySOShipped" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtySOShipped" runat="server" Enabled="False" DataField="InclQtySOShipped" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtySOBackOrdered" runat="server" DataField="QtySOBackOrdered" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtySOBackOrdered" runat="server" DataField="InclQtySOBackOrdered" Enabled="False" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtyINIssues" runat="server" DataField="QtyINIssues" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyINIssues" runat="server" DataField="InclQtyINIssues" Enabled="False" FalseValue="False" TrueValue="True" />
            <px:PXLayoutRule runat="server" />

            <px:PXLayoutRule runat="server" Merge="True" />

            <px:PXNumberEdit ID="edQtyINAssemblyDemand" runat="server" DataField="QtyINAssemblyDemand" Enabled="False" />
            <px:PXCheckBox ID="chkInclQtyINAssemblyDemand" runat="server" DataField="InclQtyINAssemblyDemand" Enabled="False" />
            <px:PXLayoutRule runat="server" />

            <px:PXNumberEdit ID="edQtySOFixed" runat="server" DataField="QtySOFixed" Enabled="False" />
            <px:PXNumberEdit ID="edQtySODropShip" runat="server" DataField="QtySODropShip" Enabled="False" />
            <px:PXTextEdit ID="edLbl" runat="server" DataField="Label" SkinID="Label" Enabled="false" SuppressLabel="true" Size="L" />
            <px:PXTextEdit ID="edLbl2" runat="server" DataField="Label2" SkinID="Label" Enabled="false" SuppressLabel="true" Size="L" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds"
        Style="z-index: 100;" Height="347px" Width="100%" AdjustPageSize="Auto"
        AllowPaging="True" AllowSearch="True" Caption="Allocation Details"
        SkinID="PrimaryInquire" SyncPosition="true" RestrictFields="True">
        <Levels>
            <px:PXGridLevel DataMember="ResultRecords">
                <Columns>
                    <px:PXGridColumn DataField="GridLineNbr" Visible="False" />
                    <px:PXGridColumn DataField="Module" />
                    <px:PXGridColumn DataField="AllocationType" />
                    <px:PXGridColumn DataField="PlanDate" Width="90px" />
                    <px:PXGridColumn DataField="QADocType" />
                    <px:PXGridColumn DataField="RefNoteID" Width="250px" />
                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" />
                    <px:PXGridColumn DataField="LocationID" AllowShowHide="Server" DisplayFormat="&gt;AAAAAAAAAA" />
                    <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" Width="120px" />
                    <px:PXGridColumn AllowNull="False" DataField="PlanQty" TextAlign="Right" Width="100px" />
                    <px:PXGridColumn DataField="BAccountID" />
                    <px:PXGridColumn DataField="AcctName" />

                    <px:PXGridColumn DataField="LocNotAvailable" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn DataField="Expired" TextAlign="Center" Type="CheckBox" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
        <ActionBar DefaultAction="ViewDocument">
           <Actions>
               <Refresh ToolBarVisible="False" />
           </Actions>
        </ActionBar>
    </px:PXGrid>
</asp:Content>
