<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AR504000.aspx.cs" Inherits="Page_AR504000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="Filter" TypeName="PX.Objects.AR.ARScheduleRun" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection" DefaultControlID="edStartDate">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXDateTimeEdit CommitChanges="True" ID="edStartDate" runat="server" DataField="StartDate" />
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartGroup="True" GroupCaption="Stop" />
			<px:PXGroupBox ID="gbLimitType" runat="server" Caption="Stop" CommitChanges="True"
				DataField="LimitTypeSel" RenderSimple="True" RenderStyle="Simple">
				<Template>
				    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True"/>				   
					<px:PXRadioButton ID="rbTillDate" runat="server" GroupName="gbLimitType" Value="D" />
					<px:PXRadioButton ID="rbMultipleTimes" runat="server" GroupName="gbLimitType" Value="M" />
				    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True"/>				   
                    <px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" SuppressLabel="True" CommitChanges="True"/>
				    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" Merge="True"/>				   
				    <px:PXNumberEdit ID="edTimes" runat="server" DataField="RunLimit" SuppressLabel="True"/>
				</Template>
				<ContentLayout LabelsWidth="S" />
			</px:PXGroupBox>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" DataSourceID="ds" BatchUpdate="True"
		AllowSearch="true" Caption="Schedules" SkinID="PrimaryInquire" SyncPosition="true" FastFilterFields="ScheduleID, ScheduleName">
		<Levels>
			<px:PXGridLevel DataMember="Schedule_List">
				<Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="30px" />
					<px:PXGridColumn DataField="ScheduleID" LinkCommand="editDetail" />
					<px:PXGridColumn DataField="ScheduleName" Width="180px" />
					<px:PXGridColumn DataField="StartDate" Width="90px" />
					<px:PXGridColumn DataField="EndDate" Width="90px" />
					<px:PXGridColumn DataField="RunCntr" TextAlign="Right" Width="54px" />
					<px:PXGridColumn DataField="RunLimit" TextAlign="Right" Width="54px" />
					<px:PXGridColumn DataField="NextRunDate" Width="90px" />
					<px:PXGridColumn DataField="LastRunDate" Width="90px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="400" />
	</px:PXGrid>
</asp:Content>