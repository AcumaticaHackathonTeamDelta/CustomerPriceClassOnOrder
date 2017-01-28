<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AR101000.aspx.cs" Inherits="Page_AR101000"
	Title="AR Setup" EnableSessionState="True" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="ARSetupRecord" TypeName="PX.Objects.AR.ARSetupMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="669px" Width="100%" DataMember="ARSetupRecord"
		DefaultControlID="edBatchNumberingID" TabIndex="100" DataSourceID="ds">
		<Items>
			<px:PXTabItem Text="General Settings">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XM" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Numbering Settings" />
					<px:PXSelector ID="edBatchNumberingID" runat="server" DataField="BatchNumberingID" Text="BATCH" AllowEdit="True" />
					<px:PXSelector ID="edInvoiceNumberingID" runat="server" DataField="InvoiceNumberingID" Text="ARINVOICE" AllowEdit="True" />
					<px:PXSelector ID="edPaymentNumberingID" runat="server" DataField="PaymentNumberingID" Text="ARPAYMENT" AllowEdit="True" />
					<px:PXSelector ID="edDebitAdjNumberingID" runat="server" DataField="DebitAdjNumberingID" Text="ARINVOICE" AllowEdit="True" />
					<px:PXSelector ID="edCreditAdjNumberingID" runat="server" DataField="CreditAdjNumberingID" Text="ARINVOICE" AllowEdit="True" />
					<px:PXSelector ID="edWriteOffNumberingID" runat="server" DataField="WriteOffNumberingID" Text="ARINVOICE" AllowEdit="True" />
					<px:PXSelector ID="edFinChargeNumberingID" runat="server" DataField="FinChargeNumberingID" Text="ARINVOICE" AllowEdit="True" />
					<px:PXSelector ID="edPriceWSNumberingID" runat="server" DataField="PriceWSNumberingID" Text="ARPRICEWS" AllowEdit="True" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Posting Settings" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAutoPost" runat="server" DataField="AutoPost" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkSummaryPost" runat="server" DataField="SummaryPost" AlignLeft="True" Size="Empty" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Data Entry Settings" />
					<px:PXSelector ID="edDfltCustomerClassID" runat="server" AllowEdit="True" DataField="DfltCustomerClassID" CommitChanges="true" />
					<px:PXSegmentMask ID="edSalesSubMask" runat="server" DataField="SalesSubMask" />
					<px:PXDropDown ID="edInvoiceRounding" runat="server" DataField="InvoiceRounding" CommitChanges="true" />
					<px:PXDropDown ID="edInvoicePrecision" runat="server" DataField="InvoicePrecision" />
					<px:PXCheckBox SuppressLabel="True" ID="chkHoldEntry" runat="server" Checked="True" DataField="HoldEntry" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkRequireControlTotal" runat="server" Checked="True" DataField="RequireControlTotal" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkRequireExtRef" runat="server" DataField="RequireExtRef" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkCreditCheckError" runat="server" DataField="CreditCheckError" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkPrintBeforeRelease" runat="server" DataField="PrintBeforeRelease" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkEmailBeforeRelease" runat="server" DataField="EmailBeforeRelease" AlignLeft="True" Size="Empty" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Data Processing Settings" />
					<px:PXCheckBox SuppressLabel="True" ID="chkIntegratedCCProcessing" runat="server" DataField="IntegratedCCProcessing" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAgeCredits" runat="server" DataField="AgeCredits" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkDefFinChargeFromCycle" runat="server" DataField="DefFinChargeFromCycle" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkFinChargeOnCharge" runat="server" DataField="FinChargeOnCharge" AlignLeft="True" Size="Empty" />
					<px:PXCheckBox SuppressLabel="True" ID="chkFinChargeFirst" runat="server" DataField="FinChargeFirst" AlignLeft="True" Size="Empty" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Salesperson Commission Settings" />
					<px:PXDropDown ID="edSPCommnCalcType" runat="server" DataField="SPCommnCalcType" />
					<px:PXDropDown ID="edSPCommnPeriodType" runat="server" DataField="SPCommnPeriodType" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Consolidation Settings" />
					<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkConsolidatedStatement" runat="server" DataField="ConsolidatedStatement" AlignLeft="True" Size="Empty" />
					<px:PXSegmentMask ID="edStatementBranchID" runat="server" DataField="StatementBranchID" />
					<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkConsolidatedDunningLetter" runat="server" DataField="ConsolidatedDunningLetter" AlignLeft="True" Size="Empty" />
					<px:PXSegmentMask ID="edDunningLetterBranchID" runat="server" DataField="DunningLetterBranchID" />
					<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartGroup="True" GroupCaption="Default Write-Off Reason Codes" />
					<px:PXSelector ID="PXSelector1" runat="server" DataField="BalanceWriteOff" AllowEdit="True" AllowAddNew="True" />
					<px:PXSelector ID="PXSelector2" runat="server" DataField="CreditWriteOff" AllowEdit="True" AllowAddNew="True" />
					<px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartGroup="True" GroupCaption="VAT Recalculation Settings" />
					<px:PXCheckBox ID="chkAutoReleasePPDCreditMemo" runat="server" DataField="AutoReleasePPDCreditMemo" AlignLeft="True" />
					<px:PXTextEdit ID="edPPDCreditMemoDescr" runat="server" DataField="PPDCreditMemoDescr" />
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Price/Discount Calculation">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartGroup="True" GroupCaption="Price Calculation Details" />
					<px:PXSelector ID="edDefaultRateTypeID" runat="server" DataField="DefaultRateTypeID" AllowEdit="True" edit="1" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAlwaysFromBaseCury" runat="server" DataField="AlwaysFromBaseCury" />
					<px:PXLayoutRule ID="PXLayoutRule4" runat="server" GroupCaption="Price Retention Settings" StartGroup="True" />
					<px:PXDropDown ID="edRetentionType" runat="server" DataField="RetentionType" CommitChanges="true" />
					<px:PXNumberEdit ID="edNumberOfMonths" runat="server" AllowNull="False" DataField="NumberOfMonths" CommitChanges="true" />
					<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartGroup="True" GroupCaption="Discount Application Details" />
					<px:PXDropDown ID="edLineDiscountTarget" runat="server" AllowNull="False" DataField="LineDiscountTarget" />
					<px:PXCheckBox SuppressLabel="True" ID="edIgnoreDiscountsIfPriceDefined" runat="server" DataField="IgnoreDiscountsIfPriceDefined" />
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Dunning Settings">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Dunning Process Settings" />
					<px:PXDropDown ID="edDunningLetterProcessType" runat="server" DataField="DunningLetterProcessType" CommitChanges="true" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAutoReleaseDunningLetter" runat="server" DataField="AutoReleaseDunningLetter" />
					<px:PXCheckBox SuppressLabel="True" ID="chkIncludeNonOverdueDunning" runat="server" DataField="IncludeNonOverdueDunning" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Dunning Fee Settings" />
					<px:PXSelector CommitChanges="True" ID="edDunningFeeInventoryID" runat="server" DataField="DunningFeeInventoryID" AutoRefresh="True" />
					<px:PXCheckBox SuppressLabel="True" ID="chkAutoReleaseDunningFee" runat="server" DataField="AutoReleaseDunningFee" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
					<px:PXGrid ID="gridNS" runat="server" SkinID="DetailsInTab" Width="100%" Height="300px" AllowPaging="False" TabIndex="200" DataSourceID="ds">
						<AutoCallBack Target="gridNR" Command="Refresh" />
						<Mode InitNewRow="True" AllowSort="False" />
						<Levels>
							<px:PXGridLevel DataMember="DunningSetup">
								<Columns>
									<px:PXGridColumn DataField="DunningLetterLevel" Label="Dunning Letter Level" TextAlign="Right" Width="150px" />
									<px:PXGridColumn DataField="DueDays" Label="Due Days" TextAlign="Right" Width="120px" CommitChanges="true" />
									<px:PXGridColumn DataField="DaysToSettle" Label="Due Days" TextAlign="Right" Width="120px" CommitChanges="true" />
									<px:PXGridColumn DataField="Descr" Label="Descr" TextAlign="Left" Width="200px" MaxLength="60" />
									<px:PXGridColumn DataField="DunningFee" Label="Dunning Letter Fee" TextAlign="Right" Width="100px" CommitChanges="true" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<ActionBar>
							<Actions>
								<ExportExcel Enabled="false" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Mailing Settings">
				<Template>
					<px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="300" SkinID="Horizontal" Height="500px">
						<AutoSize Enabled="true" />
						<Template1>
							<px:PXGrid ID="gridNS" runat="server" SkinID="DetailsInTab" Width="100%" Height="150px" Caption="Default Sources" AdjustPageSize="Auto" AllowPaging="True" TabIndex="300" DataSourceID="ds">
								<AutoCallBack Target="gridNR" Command="Refresh" />
								<Levels>
									<px:PXGridLevel DataMember="Notifications">
										<RowTemplate>
											<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXMaskEdit ID="edNotificationCD" runat="server" DataField="NotificationCD" />
											<px:PXSelector ID="edNotificationID" runat="server" DataField="NotificationID" ValueField="Name" />
											<px:PXDropDown ID="edFormat" runat="server" DataField="Format" />
											<px:PXCheckBox ID="chkActive" runat="server" DataField="Active" />
											<px:PXSelector ID="edReportID" runat="server" DataField="ReportID" ValueField="ScreenID" />
											<px:PXSelector ID="edEMailAccount" runat="server" DataField="EMailAccountID" DisplayMode="Text" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="NotificationCD" Width="120px" />
											<px:PXGridColumn DataField="EMailAccountID" Width="200px" DisplayMode="Text" />
											<px:PXGridColumn DataField="ReportID" Width="150px" AutoCallBack="True" />
											<px:PXGridColumn DataField="NotificationID" Width="150px" AutoCallBack="True" />
											<px:PXGridColumn DataField="Format" RenderEditorText="True" Width="70px" AutoCallBack="True" />
											<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" Width="60px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="True" />
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid ID="gridNR" runat="server" SkinID="DetailsInTab" Width="100%" Caption="Default Recipients" AdjustPageSize="Auto" AllowPaging="True" TabIndex="400" DataSourceID="ds">
								<Parameters>
									<px:PXSyncGridParam ControlID="gridNS" />
								</Parameters>
								<CallbackCommands>
									<Save CommitChangesIDs="gridNR" RepaintControls="None" />
									<FetchRow RepaintControls="None" />
								</CallbackCommands>
								<Levels>
									<px:PXGridLevel DataMember="Recipients" DataKeyNames="RecipientID">
										<Columns>
											<px:PXGridColumn DataField="ContactType" RenderEditorText="True" Width="100px" AutoCallBack="True">
											</px:PXGridColumn>
											<px:PXGridColumn DataField="OriginalContactID" Visible="False" AllowShowHide="False" />
											<px:PXGridColumn DataField="ContactID" Width="250px">
												<NavigateParams>
													<px:PXControlParam Name="ContactID" ControlID="gridNR" PropertyName="DataValues[&quot;OriginalContactID&quot;]" />
												</NavigateParams>
											</px:PXGridColumn>
											<px:PXGridColumn DataField="Format" RenderEditorText="True" Width="60px" AutoCallBack="True">
											</px:PXGridColumn>
											<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" Width="60px">
											</px:PXGridColumn>
											<px:PXGridColumn DataField="Hidden" TextAlign="Center" Type="CheckBox" Width="60px">
											</px:PXGridColumn>
										</Columns>
										<RowTemplate>
											<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
											<px:PXSelector ID="edContactID" runat="server" DataField="ContactID" AutoRefresh="True"
												ValueField="DisplayName" AllowEdit="True">
											</px:PXSelector>
										</RowTemplate>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="True" />
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="300" MinWidth="100" />
	</px:PXTab>
</asp:Content>
