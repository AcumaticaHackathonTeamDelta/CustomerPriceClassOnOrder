﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="EP404000.aspx.cs" Inherits="Pages_EP404000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<script type="text/javascript">
		function refreshTasksAndEvents(sender, args)
		{
			var top = window.top;
			if (top != window && top.MainFrame != null) top.MainFrame.refreshEventsInfo();
		}
	</script>

    <px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%"
                     PrimaryView="Tasks" TypeName="PX.Objects.EP.EPTaskEnq" PageLoadBehavior="PopulateSavedValues">
        <CallbackCommands>
			<px:PXDSCallbackCommand DependOnGrid="gridTasks" Name="Tasks_ViewDetails" />
			<px:PXDSCallbackCommand DependOnGrid="gridTasks" Name="viewEntity" Visible="False" />
			<px:PXDSCallbackCommand DependOnGrid="gridTasks" Name="viewOwner" Visible="False" />
			<px:PXDSCallbackCommand Name="addNew" RepaintControls="All" PopupCommand="Refresh" PopupCommandTarget="gridTasks" CommitChanges="True"/>
			<px:PXDSCallbackCommand DependOnGrid="gridTasks" Name="cancelActivity" 
				RepaintControls="All" />
			<px:PXDSCallbackCommand DependOnGrid="gridTasks" Name="complete" 
				RepaintControls="All" />
		</CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phL" runat="server">
	<px:PXGrid ID="gridTasks" runat="server" DataSourceID="ds" Width="100%" ActionsPosition="Top" Caption="Tasks"
		OnRowDataBound="grid_RowDataBound" FilesField="NoteFiles" AllowPaging="true" AllowSearch="true" BlankFilterHeader="All Tasks"
		MatrixMode="true" SkinID="PrimaryInquire" AdjustPageSize="Auto" FastFilterFields="Subject" RestrictFields="False" SyncPosition="True">
		<ClientEvents AfterRefresh="refreshTasksAndEvents" />
		<Levels>
			<px:PXGridLevel DataMember="Tasks">
				<Columns>
				    <px:PXGridColumn DataField="CRReminder__ReminderIcon" Width="26px" AllowShowHide="False" Label="Reminder Icon" AllowResize="False" ForceExport="True" />
					<px:PXGridColumn DataField="PriorityIcon" Width="21px" AllowShowHide="False" Label="Priority Icon"
						AllowResize="False" SortField="Priority" ForceExport="True" />
					<px:PXGridColumn DataField="Subject" Width="400px" LinkCommand="Tasks_ViewDetails" />
					<px:PXGridColumn AllowNull="False" DataField="UIStatus" Width="80px" />
					<px:PXGridColumn DataField="PercentCompletion" Width="90px" />
					<px:PXGridColumn DataField="StartDate" DataType="DateTime" DisplayFormat="d" Width="120px"  />
					<px:PXGridColumn DataField="EndDate" DataType="DateTime" DisplayFormat="d" Width="120px" />
					<px:PXGridColumn DataField="CategoryID" />
					<px:PXGridColumn DataField="WorkgroupID" Width="150px" SyncVisible="False"
						SyncVisibility="False" Visible="False" />
					<px:PXGridColumn DataField="OwnerID" Width="150px" LinkCommand="viewOwner" SyncVisible="False"
						SyncVisibility="False" Visible="False" DisplayMode="Text" />
					<px:PXGridColumn DataField="CreatedByID" Width="80px" DisplayMode="Text" SyncVisible="False"
						SyncVisibility="False" Visible="False" />
		            <px:PXGridColumn DataField="Source" Width="150px" LinkCommand="viewEntity" AllowSort="false" AllowFilter="false"/>
                    <px:PXGridColumn DataField="EPView__Read" Width="50px" TextAlign="Center" Type="CheckBox" AllowShowHide="True" Visible="false" SyncVisible="false"/>
				</Columns>
			</px:PXGridLevel>
		</Levels>
	    <ActionBar DefaultAction="viewDetail">
            <CustomItems>
				<px:PXToolBarButton Text="View Details" Key="viewDetail" Visible="False">
					<ActionBar GroupIndex="0" />
					<AutoCallBack Target="ds" Command="Tasks_ViewDetails" />
					<PopupCommand Target="gridTasks" Command="Refresh" />
					<Images Normal="main@Inquiry" />
				</px:PXToolBarButton>
            </CustomItems>    
        </ActionBar>
		<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
