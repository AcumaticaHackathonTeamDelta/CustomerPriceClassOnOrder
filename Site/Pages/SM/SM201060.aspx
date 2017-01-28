<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM201060.aspx.cs" Inherits="Page_SM201060"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Prefs"
		TypeName="PX.SM.PreferencesSecurityMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Caption="General Settings"
		Style="z-index: 100" Width="100%" DataMember="Prefs" TemplateContainer="">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="L" ControlSize="XL"
				ColumnSpan="2" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Password Policy" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkisPasswordDayAge"
				runat="server" DataField="IsPasswordDayAge" AlignLeft="True" Size="L" />
			<px:PXNumberEdit Size="XS" ID="edPasswordDayAge" runat="server" DataField="PasswordDayAge"
				SuppressLabel="True" />
			<px:PXLabel Size="xs" ID="lblPasswordDayAge" runat="server" Text="Days" Height="18px"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkIsPasswordMinLength"
				runat="server" DataField="IsPasswordMinLength" AlignLeft="True" Size="L" />
			<px:PXNumberEdit Size="XS" ID="edPasswordMinLength" runat="server" DataField="PasswordMinLength"
				SuppressLabel="True" />
			<px:PXLabel Size="xs" ID="lblPasswordMinLength" runat="server" Text="Characters"
				Height="18px"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXCheckBox SuppressLabel="True" ID="chkPasswordComplexity" runat="server" DataField="PasswordComplexity"
				AlignLeft="True" />
			<px:PXTextEdit ID="edPasswordRegexCheck" runat="server" DataField="PasswordRegexCheck" />
			<px:PXTextEdit ID="edPasswordRegexCheckMessage" runat="server" DataField="PasswordRegexCheckMessage"
				TextMode="MultiLine"   />
			<px:PXLabel ID="PXHole" runat="server"></px:PXLabel>
			<px:PXLayoutRule runat="server" StartColumn="True" StartRow="True" 
				ControlSize="SM" LabelsWidth="M" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Account Lockout Policy" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit Size="XS" ID="edAccountLockoutThreshold" runat="server" 
				DataField="AccountLockoutThreshold" />
			<px:PXLabel Size="m" ID="lblUnsuccess" runat="server" Text="Unsuccessful Login Attempts"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit Size="XS" ID="edAccountLockoutDuration" runat="server" 
				DataField="AccountLockoutDuration" />
			<px:PXLabel Size="xs" ID="lblLockMinutes" runat="server" Text="Minutes"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit Size="XS" ID="edAccountLockoutReset" runat="server" 
				DataField="AccountLockoutReset" />
			<px:PXLabel Size="xs" ID="lblResetMinutes" runat="server" Text="Minutes"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Encryption Certificates" />
			<px:PXSelector ID="edDBCertificateName" runat="server" DataField="DBCertificateName"
				DataSourceID="ds" />
			<px:PXSelector ID="edPdfCertificateName" runat="server" DataField="PdfCertificateName"
				DataSourceID="ds" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" 
				LabelsWidth="M">
			</px:PXLayoutRule>
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Audit" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit Size="XS" ID="edTraceMonthsKeep" runat="server" 
				DataField="TraceMonthsKeep" />
			<px:PXLabel Size="XS" ID="lblMonth" runat="server" Text="Months"></px:PXLabel>
			<px:PXLayoutRule runat="server" />
			<px:PXPanel ID="PXPanel1" runat="server" RenderSimple="True" 
				RenderStyle="Simple">
				<px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True"/>
				<px:PXCheckBox ID="chkTraceOperationLogin" runat="server" AlignLeft="True" DataField="TraceOperationLogin" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationLoginFailed" runat="server" AlignLeft="True" DataField="TraceOperationLoginFailed" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationLogout" runat="server" AlignLeft="True" DataField="TraceOperationLogout" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationAccessScreen" runat="server" AlignLeft="True" DataField="TraceOperationAccessScreen" SuppressLabel="True" />
				<px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationSessionExpired" runat="server" AlignLeft="True" DataField="TraceOperationSessionExpired" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationLicenseExceeded" runat="server" AlignLeft="True" DataField="TraceOperationLicenseExceeded" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceSendMail" runat="server" AlignLeft="True" DataField="TraceOperationSendMail" SuppressLabel="True" />
				<px:PXCheckBox ID="chkTraceOperationSendMailFailed" runat="server" AlignLeft="True" DataField="TraceOperationSendMailFailed" SuppressLabel="True" />
				<px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
                <px:PXCheckBox ID="chkTraceOperationODataRefresh" runat="server" AlignLeft="True" DataField="TraceOperationODataRefresh" SuppressLabel="True" />
                <px:PXCheckBox ID="chkTraceOperationCustomizationPublished" runat="server" AlignLeft="True" DataField="TraceOperationCustomizationPublished" SuppressLabel="True" />
			</px:PXPanel>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="PXGrid1" runat="server" Height="400px" Width="100%" Style="z-index: 100"
		Caption="Allowed External Identity Providers" AllowPaging="True" AllowSearch="true" AdjustPageSize="Auto"
		DataSourceID="ds" SkinID="ShortList">
		<Levels>
			<px:PXGridLevel DataMember="Identities">
				<Columns>
					<px:PXGridColumn DataField="ProviderName" Width="108px" />
					<px:PXGridColumn DataField="Active" Width="90px" TextAlign="Center" Type="CheckBox"  />
					<px:PXGridColumn DataField="Realm" Width="250px" />
					<px:PXGridColumn DataField="ApplicationID" Width="150px" />
					<px:PXGridColumn DataField="ApplicationSecret" Width="150px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="True" Container="Window" MinHeight="100" />
		<ActionBar>
			<Actions>
				<EditRecord Enabled="False" />
				<NoteShow Enabled="False" />
				<PageNext Enabled="False" />
				<PagePrev Enabled="False" />
				<PageFirst Enabled="False" />
				<PageLast Enabled="False" />
			</Actions>
		</ActionBar>
	</px:PXGrid>
</asp:Content>
