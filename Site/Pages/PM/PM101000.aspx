<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM101000.aspx.cs"
    Inherits="Page_PM101000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.SetupMaint" PrimaryView="Setup">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Setup" Caption="Settings"
        EmailingGraph="">
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
            <px:PXCheckBox ID="chkIsActive" runat="server" Checked="True" DataField="IsActive" />
            <px:PXSelector ID="edTranNumbering" runat="server"  DataField="TranNumbering" Text="PMTRAN" 
                DataSourceID="ds" />
            <px:PXSelector ID="edBatchNumberingID" runat="server"  DataField="BatchNumberingID" Text="BATCH" 
                DataSourceID="ds" />
            <px:PXTextEdit ID="edNonProjectCode" runat="server"  DataField="NonProjectCode" Text="X" />
			<px:PXSelector ID="edAssignmentMapID" runat="server" DataField="AssignmentMapID" TextField="Name" AllowEdit="True" DataSourceID="ds" edit="1" />
 			<px:PXSelector ID="edAssignmentNotificationID" runat="server" DataField="AssignmentNotificationID" TextField="Name" AllowEdit="True" DataSourceID="ds" edit="1" />                                           
			<px:PXDropDown ID="edCutOffdate" runat="server"  DataField="CutoffDate"/>
            <px:PXCheckBox ID="chkAutoPost" runat="server" DataField="AutoPost" />
            <px:PXCheckBox ID="chkAutoReleaseAllocation" runat="server" DataField="AutoReleaseAllocation" />
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartGroup="True" GroupCaption="Visibility Settings" />
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
            <px:PXLayoutRule runat="server" />
            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Account Settings" />
            <px:PXDropDown ID="edExpenseAccountSource" runat="server"  DataField="ExpenseAccountSource" SelectedIndex="-1" />
            <px:PXSegmentMask ID="edExpenseSubMask" runat="server" DataField="ExpenseSubMask" DataMember="_PMSETUP_Segments_" 
                DataSourceID="ds" />
             <px:PXDropDown ID="edExpenseAccrualAccountSource" runat="server"  DataField="ExpenseAccrualAccountSource" SelectedIndex="-1" />
            <px:PXSegmentMask ID="edExpenseAccrualSubMask" runat="server" DataField="ExpenseAccrualSubMask" DataMember="_PMSETUP_Segments_" 
                DataSourceID="ds" />
        </Template>
    </px:PXFormView>
</asp:Content>
