<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL201000.aspx.cs" Inherits="Page_GL201000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%"
		TypeName="PX.Objects.GL.FiscalPeriodMaint" PrimaryView="FiscalYear" PageLoadBehavior="GoLastRecord">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" Visible ="true" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="autoFill" />
            <px:PXDSCallbackCommand Name="updateYearOK" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="CopyPaste" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server"   Width="100%"
		DataMember="FiscalYear" Caption="Financial Year" NoteIndicator="True" FilesIndicator="True"
		ActivityIndicator="True" ActivityField="NoteActivity" DefaultControlID="edYear">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
			<px:PXSelector ID="edYear" runat="server" DataField="Year"  >
				<AutoCallBack Command="Cancel" Target="ds">
				</AutoCallBack>
			</px:PXSelector>
			<px:PXButton ID="pbGenerate" runat="server" CommandName="autoFill" CommandSourceID="ds"
				Visible="False" Text="pbGenerate">
			</px:PXButton>
			<px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" Enabled="False" />
			<px:PXNumberEdit ID="edFinPeriods" runat="server" DataField="FinPeriods" Enabled="False" />
            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" />
            <px:PXCheckBox OnValueChange="Commit" ID="CustomPeriods" runat="server" DataField="CustomPeriods" AlignLeft="True" />
		</Template>
	</px:PXFormView>

</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server"  Height="150px" 
		Width="100%" Caption="Financial Year Periods" SkinID="Details" >
        <Levels>
			<px:PXGridLevel DataMember="Periods">
				<Columns>
					<px:PXGridColumn   DataField="PeriodNbr" />
					<px:PXGridColumn DataField="FinPeriodID" />
					<px:PXGridColumn DataField="StartDate" Width="90px" />
					<px:PXGridColumn DataField="EndDateUI" Width="90px" AutoCallBack="true" />
                    <px:PXGridColumn DataField="Length" Width="40px" />
					<px:PXGridColumn DataField="Descr" Width="120px" />
                    <px:PXGridColumn DataField="IsAdjustment" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="APClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="ARClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="INClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="Closed" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="CAClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
					<px:PXGridColumn DataField="FAClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
                    <px:PXGridColumn DataField="PRClosed" TextAlign="Center" Type="CheckBox" Width="40px" />
				</Columns>
                <RowTemplate>
					<px:PXDateTimeEdit ID="EndDateUI" runat="server" DataField="EndDateUI" CommitChanges="true">
                        <AutoCallBack Enabled="true" ActiveBehavior="true" >
                            <Behavior RepaintControlsIDs="grid" CommitChanges="true" RepaintControls="All"/>
                        </AutoCallBack>
					</px:PXDateTimeEdit>
				</RowTemplate>
				<Layout FormViewHeight=""></Layout>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<Mode InitNewRow="True" AutoInsert="True"/>
		<ActionBar>
			<Actions>
				<AddNew Enabled="true" />
				<Delete Enabled="true" />
				<NoteShow Enabled="False" />
			</Actions>
		</ActionBar>
	</px:PXGrid>

    <px:PXSmartPanel ID="spSaveDlg" runat="server" Key="SaveDialog" AutoCallBack-Command="Refresh"
		AutoCallBack-Target="SavePrm" AutoCallBack-Enabled="true" LoadOnDemand="True"
		AcceptButtonID="cbOk" CancelButtonID="cbCancel" Caption="Update Financial Year"
		CaptionVisible="True" DesignView="Content" CommandName="updateYearOK" CommandSourceID="ds">
		<px:PXFormView ID="SavePrm" runat="server" DataSourceID="ds" Style="z-index: 108"
			Width="100%" DataMember="SaveDialog" Caption="Update Financial Year" SkinID="Transparent">
			<Activity Height="" HighlightColor="" SelectedColor="" Width="" />
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule2" runat="server" />
                <px:PXTextEdit ID="edMessage" runat="server" DataField="Message" TextMode="MultiLine" Height="67px" SuppressLabel="true" SkinID="none" Style="background-color:#E5E9EE; resize: none"  >
                    <Padding Bottom="0px" />
                    <Border> 
                        <Bottom Width="0px" />
                        <Top Width="0px" />
                        <Left Width="0px" />
                        <Right Width="0px" />
                    </Border>
				</px:PXTextEdit>
                <px:PXDropDown ID="edMethod" runat="server" DataField="Method" Width="300" CommitChanges="true"/>
                <px:PXCheckBox ID="edMoveDayOfWeek" runat="server" DataField="MoveDayOfWeek" Width="280" CommitChanges="true"/>
                <px:PXHtmlView ID="edMethodDescription" runat="server" DataField="MethodDescription" TextMode="MultiLine"
                    MaxLength="50" Width="419" Height="100" Style="background-color:#EEEEEE; resize: none" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="cbOk" runat="server" Text="OK" DialogResult="OK" />
			<px:PXButton ID="cbCancel" runat="server" Text="Cancel" DialogResult="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>
