<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AR202800.aspx.cs" Inherits="Page_AR202800" Title="AR Statement Cycle" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="ARStatementCycleRecord" TypeName="PX.Objects.AR.ARStatementMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="recreateLast" CommitChanges="true" StartNewGroup="True" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%" DataMember="ARStatementCycleRecord" Caption="Statement Cycle" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="True" DefaultControlID="edStatementCycleId"
		TabIndex="100" DataSourceID="ds">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="L" ColumnSpan="3" GroupCaption="General Settings" StartGroup="True" />
			<px:PXSelector ID="edStatementCycleId" runat="server" DataField="StatementCycleId" DataSourceID="ds" />
			<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
			<px:PXDropDown CommitChanges="True" ID="edPrepareOn" runat="server" DataField="PrepareOn" SelectedIndex="1" />
			<px:PXNumberEdit ID="edDay00" runat="server" DataField="Day00" CommitChanges="true" />
			<px:PXNumberEdit ID="edDay01" runat="server" DataField="Day01"  CommitChanges="true" />
			<px:PXDateTimeEdit ID="edLastStmtDate" runat="server" DataField="LastStmtDate" Enabled="False" />
            <px:PXCheckBox ID="chkRequirePaymentApplication" runat="server" DataField="RequirePaymentApplication" />
			<px:PXLayoutRule runat="server" GroupCaption="Aging Settings" StartGroup="True" />
			<px:PXPanel ID="PXPanel1" runat="server" Caption="Aging Settings" ContentLayout-InnerSpacing="False" RenderSimple="True" ContentLayout-OuterSpacing="Horizontal">
				<px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" ColumnWidth = "XXS" />
				<px:PXLabel ID="RowNumberHeader" runat="server" Text="Aging Period" />
				<px:PXLabel ID="RowNumber1" runat="server">1</px:PXLabel>
				<px:PXLabel ID="RowNumber2" runat="server">2</px:PXLabel>
				<px:PXLabel ID="RowNumber3" runat="server">3</px:PXLabel>
				<px:PXLabel ID="RowNumber4" runat="server">4</px:PXLabel>
				<px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
				<px:PXLabel ID="DaysHeader" runat="server" Size="xs" Text="Days Due" Width="60px" />
				<px:PXNumberEdit ID="edAgeDays00" runat="server" DataField="AgeDays00" Size="XXS" TabIndex="107"/>
				<px:PXNumberEdit ID="edAgeDays01" runat="server" DataField="AgeDays01" Size="XXS" TabIndex="109"/>
				<px:PXNumberEdit ID="edAgeDays02" runat="server" DataField="AgeDays02" Size="XXS" TabIndex="111"/>
				<px:PXLabel ID="DaysOver" runat="server" Size="xs" Text="Over Days" />
				<px:PXLayoutRule runat="server" ControlSize="M" StartColumn="True" SuppressLabel="True" />
				<px:PXLabel ID="MessageHeader" runat="server" Size="xs" Text="Message Description" Width="200px" />
				<px:PXTextEdit ID="edAgeMsg00" runat="server" DataField="AgeMsg00" SuppressLabel="True" TabIndex="108"/>
				<px:PXTextEdit ID="edAgeMsg01" runat="server" DataField="AgeMsg01" SuppressLabel="True" TabIndex="110"/>
				<px:PXTextEdit ID="edAgeMsg02" runat="server" DataField="AgeMsg02" SuppressLabel="True" TabIndex="112"/>
				<px:PXTextEdit ID="edAgeMsg03" runat="server" DataField="AgeMsg03" SuppressLabel="True" TabIndex="113"/>
			</px:PXPanel>
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Finance Charge Settings" ColumnSpan="3" />
			<px:PXCheckBox CommitChanges="True" ID="chkFinChargeApply" runat="server" DataField="FinChargeApply" />
			<px:PXCheckBox ID="chkRequireFinChargeProcessing" runat="server" DataField="RequireFinChargeProcessing" />
			<px:PXSelector ID="edFinChargeID" runat="server" DataField="FinChargeID" DataSourceID="ds" />
		</Template>
		<AutoSize Enabled="True" Container="Window" MinHeight="300" MinWidth="400" />
	</px:PXFormView>
</asp:Content>
