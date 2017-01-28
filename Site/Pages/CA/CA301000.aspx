<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA301000.aspx.cs" Inherits="Page_CA301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXSmartPanel ID="AddFilterPanel" Key="AddFilter" runat="server" Caption="Quick Transaction" CaptionVisible="True" AutoCallBack-Target="AddFilter" AutoCallBack-Command="Refresh"
		Overflow="Hidden">
		<px:PXFormView ID="AddFilter" runat="server"  DataMember="AddFilter" Caption="Quick Transaction" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" />
				<px:PXSelector CommitChanges="True" ID="edEntryTypeId" runat="server" DataField="EntryTypeID" AutoRefresh="True" />
				<px:PXDateTimeEdit ID="edTranDate" runat="server" CommitChanges="True" DataField="TranDate" />
				<px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" size = "S"/>
				<pxa:PXCurrencyRate ID="edCury" runat="server" DataField="CuryID" DataMember="_Currency_" RateTypeView="_AddTrxFilter_CurrencyInfo_" />
				<px:PXTextEdit ID="edExtRefNbr" runat="server" CommitChanges="True" DataField="ExtRefNbr" />
				<px:PXSelector CommitChanges="True" ID="edReferenceID" runat="server" DataField="ReferenceID" AutoRefresh="True" />
				<px:PXSelector CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True" />
				<px:PXSelector CommitChanges="True" ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID" />
				<px:PXSelector ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="True" AutoGenerateColumns="True" />
				<px:PXNumberEdit ID="edCuryTranAmt" runat="server" DataField="CuryTranAmt" />
				<px:PXLayoutRule ID="PXLayoutRule2" runat="server" ColumnSpan="2" />
				<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
				<px:PXLayoutRule ID="PXLayoutRule3" runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True" />
				<px:PXDropDown ID="edDrCr" runat="server" CommitChanges="True" DataField="DrCr" Enabled="False" />
				<px:PXTextEdit CommitChanges="True" ID="PXTextEdit1" runat="server" DataField="OrigModule" Enabled="False" />
				<px:PXDropDown Size="s" ID="edStatus" runat="server" DataField="Status" Enabled="False" />
				<px:PXCheckBox CommitChanges="True" ID="chkHold" runat="server" DataField="Hold" />
				<px:PXCheckBox ID="chkCleared" runat="server" DataField="Cleared" />
				<px:PXSegmentMask ID="edAccountID" runat="server" CommitChanges="True" DataField="AccountID" />
				<px:PXSegmentMask ID="edSubID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="SubID" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="OK" runat="server" DialogResult="OK" Text="Save" />
			<px:PXButton ID="Cancel" runat="server" DialogResult="No" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Transfer" TypeName="PX.Objects.CA.CashTransferEntry">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" StartNewGroup="True" Name="release" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="prepareAdd" Visible="False" />
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="viewDoc" PopupCommand="Refresh" PopupCommandTarget="grid" />
			<px:PXDSCallbackCommand Name="ViewInBatch" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewOutBatch" Visible="False" />
			<px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%" DataMember="Transfer" Caption="Transfer summary" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="True" ActivityField="NoteActivity" LinkIndicator="True"
		NotifyIndicator="True" DefaultControlID="edTransferNbr" AllowCollapse="true">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			<px:PXSelector ID="edTransferNbr" runat="server" DataField="TransferNbr">
			</px:PXSelector>
			<px:PXDropDown Size="s" ID="edStatus" runat="server" DataField="Status" Enabled="False" />
			<px:PXCheckBox CommitChanges="True" ID="chkHold" runat="server" DataField="Hold" />
			<px:PXLayoutRule ID="PXLayoutRule5" runat="server" ControlSize="s" LabelsWidth="s" StartColumn="True" />
			<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" Size="L" />
			<px:PXNumberEdit ID="edRGOLAmt" runat="server" DataField="RGOLAmt" Enabled="False" />
			<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartRow="True" />
			<px:PXPanel ID="PXPanelDebit" runat="server" Caption="Source Account" Width="100%" RenderStyle="Fieldset">
				<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSegmentMask CommitChanges="True" ID="edOutAccountID" runat="server" DataField="OutAccountID" />
				<px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" />
				<px:PXDateTimeEdit CommitChanges="True" ID="edOutDate" runat="server" DataField="OutDate" />
				<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkClearedOut" runat="server" DataField="ClearedOut" />
				<px:PXLayoutRule ID="PXLayoutRule8" runat="server" />
				<px:PXTextEdit CommitChanges="True" ID="edOutExtRefNbr" runat="server" DataField="OutExtRefNbr" />
				<pxa:PXCurrencyRate DataField="OutCuryID" RateField="OutCuryRate" ID="edCuryOut" runat="server" RateTypeView="_CATransfer_CurrencyInfo_OutCuryInfoID_" DataMember="_Currency_"></pxa:PXCurrencyRate>
				<px:PXNumberEdit CommitChanges="True" ID="edCuryTranOut" runat="server" DataField="CuryTranOut" />
				<px:PXLayoutRule ID="PXLayoutRule9" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
				<px:PXSelector ID="edBatchNbrOut" runat="server" DataField="TranIDOut_CATran_batchNbr" AllowEdit="True" Enabled="False" />
				<px:PXDateTimeEdit ID="edClearDateOut" runat="server" DataField="ClearDateOut" />
				<px:PXNumberEdit ID="edGLBalanceOut" runat="server" DataField="OutGLBalance" Enabled="False" />
				<px:PXNumberEdit ID="edCashBalanceOut" runat="server" DataField="CashBalanceOut" Enabled="False" />
				<px:PXNumberEdit ID="edTranOut" runat="server" DataField="TranOut" Enabled="False" />
			</px:PXPanel>
			<px:PXPanel ID="PXPanelCredit" runat="server" Caption="Destination Account" Width="100%" RenderStyle="Fieldset">
				<px:PXLayoutRule ID="PXLayoutRule10" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSegmentMask CommitChanges="True" ID="edInAccountID" runat="server" DataField="InAccountID" />
				<px:PXLayoutRule ID="PXLayoutRule1" runat="server" Merge="True" />
				<px:PXDateTimeEdit CommitChanges="True" ID="edInDate" runat="server" DataField="InDate" />
				<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkClearedIn" runat="server" DataField="ClearedIn" />
				<px:PXLayoutRule ID="PXLayoutRule2" runat="server" />
				<pxa:PXCurrencyRate DataField="InCuryID" RateField="InCuryRate" ID="RateType2" runat="server" RateTypeView="_CATransfer_CurrencyInfo_InCuryInfoID_" DataMember="_Currency_"></pxa:PXCurrencyRate>
				<px:PXTextEdit ID="edInExtRefNbr" runat="server" DataField="InExtRefNbr" />
				<px:PXNumberEdit CommitChanges="True" ID="edCuryTranIn" runat="server" DataField="CuryTranIn" />
				<px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
				<px:PXSelector ID="edBatchNbrIn" runat="server" DataField="TranIDIn_CATran_batchNbr" AllowEdit="True" Enabled="False" />
				<px:PXDateTimeEdit ID="PXDateTimeEdit1" runat="server" DataField="ClearDateIn" />
				<px:PXNumberEdit ID="edGLBalanceIn" runat="server" DataField="InGLBalance" Enabled="False" />
				<px:PXNumberEdit ID="edCashBalanceIn" runat="server" DataField="CashBalanceIn" Enabled="False" />
				<px:PXNumberEdit ID="edTranIn" runat="server" DataField="TranIn" Enabled="False" />
			</px:PXPanel>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="150px" Width="100%" ActionsPosition="Top" Caption="Additional Charges" SkinID="Details">
		<Mode InitNewRow="True" />
		<ActionBar DefaultAction="viewDoc">
		    <Actions>
		        <AddNew ToolBarVisible="False" />
		    </Actions>
			<CustomItems>
				<px:PXToolBarButton Text="Add Expense" Key="cmdAddFilter">
					<AutoCallBack Command="prepareAdd" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="TransferTran">
				<Columns>
					<px:PXGridColumn DataField="CashAccountID" />
					<px:PXGridColumn DataField="ExtRefNbr" LinkCommand="viewDoc" />
					<px:PXGridColumn DataField="TranDate" Width="90px" />
					<px:PXGridColumn DataField="CASplit__TranDesc" Width="200px" />
					<px:PXGridColumn DataField="CAAdj__EntryTypeID" Width="100px" />
					<px:PXGridColumn DataField="CASplit__AccountID" Width="100px" />
					<px:PXGridColumn DataField="CASplit__SubID" Width="150px" />
					<px:PXGridColumn DataField="CASplit__CuryTranAmt" TextAlign="Right" Width="100px" />
					<px:PXGridColumn DataField="CuryID" />
					<px:PXGridColumn DataField="Cleared" Width="60px" Type="CheckBox" TextAlign="Center" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
