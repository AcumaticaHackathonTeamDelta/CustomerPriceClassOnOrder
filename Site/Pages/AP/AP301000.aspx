<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AP301000.aspx.cs" Inherits="Page_AP301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.AP.APInvoiceEntry" PrimaryView="Document">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="Release" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="Prebook" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="VoidInvoice" CommitChanges="true" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="Action" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="Inquiry" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="Report" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="ReverseInvoice" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="ReclassifyBatch" />
			<px:PXDSCallbackCommand Visible="false" Name="VendorRefund" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="VoidDocument" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="PayInvoice" />
			<px:PXDSCallbackCommand Visible="False" Name="ViewSchedule" CommitChanges="true" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Visible="false" Name="CreateSchedule" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewBatch" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewVoucherBatch" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewWorkBook" />
			<px:PXDSCallbackCommand Visible="false" Name="NewVendor" />
			<px:PXDSCallbackCommand Visible="false" Name="EditVendor" />
			<px:PXDSCallbackCommand Visible="false" Name="VendorDocuments" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOReceipt2" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddReceiptLine2" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrder2" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOReceipt" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddReceiptLine" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrder" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="LinkLine" CommitChanges="true" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewPODocument" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewLCPOReceipt" DependOnGrid="gridLCTran" />
			<px:PXDSCallbackCommand Visible="false" Name="ViewLCINDocument" DependOnGrid="gridLCTran" />
			<px:PXDSCallbackCommand Visible="false" Name="AutoApply" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="ViewPayment" DependOnGrid="detgrid" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPostLandedCostTran" CommitChanges="true" />
			<px:PXDSCallbackCommand CommitChanges="True" Visible="False" Name="LsLCSplits" DependOnGrid="gridLCTran" />
			<px:PXDSCallbackCommand Name="RecalculateDiscountsAction" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="RecalcOk" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Style="z-index: 100" Width="100%"
		DataMember="Document" Caption="Document Summary" NoteIndicator="True"
		FilesIndicator="True" ActivityIndicator="True" ActivityField="NoteActivity"
		LinkIndicator="True" NotifyIndicator="True" DefaultControlID="edDocType"
		TabIndex="100" DataSourceID="ds" MarkRequired="Dynamic">
		<CallbackCommands>
			<Save PostData="Self" />
		</CallbackCommands>
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			<px:PXDropDown ID="edDocType" runat="server" DataField="DocType" />
			<px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr"
				AutoRefresh="True" DataSourceID="ds">
				<GridProperties FastFilterFields="InvoiceNbr" />
			</px:PXSelector>
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
            <px:PXCheckBox ID="chkRequestApproval" runat="server" DataField="RequestApproval" />                    
			<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkHold" runat="server" DataField="Hold" />
			<px:PXDateTimeEdit CommitChanges="True" ID="edDocDate" runat="server" DataField="DocDate" />
			<px:PXSelector CommitChanges="True" ID="edFinPeriodID" runat="server"
				DataField="FinPeriodID" DataSourceID="ds" />
			<px:PXTextEdit CommitChanges="True" ID="edInvoiceNbr" runat="server" DataField="InvoiceNbr" />
			<px:PXLayoutRule runat="server" ColumnSpan="2" />
			<px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />
			<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True" />
			<px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server"
				DataField="VendorID" AllowAddNew="True" AllowEdit="True" DataSourceID="ds" AutoRefresh="True" />
			<px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server"
				AutoRefresh="True" DataField="VendorLocationID" DataSourceID="ds" />
			<pxa:PXCurrencyRate ID="edCury" DataField="CuryID" runat="server"
				RateTypeView="_APInvoice_CurrencyInfo_" DataMember="_Currency_"
				DataSourceID="ds"></pxa:PXCurrencyRate>
			<px:PXSelector CommitChanges="True" ID="edTermsID" runat="server"
				DataField="TermsID" DataSourceID="ds" />
			<px:PXDateTimeEdit CommitChanges="True" ID="edDueDate" runat="server" DataField="DueDate" />
			<px:PXDateTimeEdit CommitChanges="True" ID="edDiscDate" runat="server" DataField="DiscDate" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			<px:PXPanel ID="PXPanel1" runat="server" RenderSimple="True" RenderStyle="Simple">
				<px:PXLayoutRule runat="server" StartColumn="True" />
				<px:PXNumberEdit ID="edCuryLineTotal" runat="server" DataField="CuryLineTotal" Enabled="False" />
				<px:PXNumberEdit CommitChanges="True" ID="edCuryDiscTot" runat="server" Enabled="False" DataField="CuryDiscTot" />
				<px:PXNumberEdit ID="CuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" />
				<px:PXNumberEdit ID="CuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" />
				<px:PXNumberEdit ID="edCuryTaxTotal" runat="server" DataField="CuryTaxTotal" Enabled="False" Size="s" />
				<px:PXNumberEdit ID="edCuryOrigWhTaxAmt" runat="server" DataField="CuryOrigWhTaxAmt" Enabled="False" />
				<px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
				<px:PXNumberEdit ID="edCuryRoundDiff" runat="server" CommitChanges="True" DataField="CuryRoundDiff" Enabled="False" />
				<px:PXNumberEdit ID="edCuryOrigDocAmt" runat="server" CommitChanges="True" DataField="CuryOrigDocAmt" />
				<px:PXNumberEdit ID="edCuryTaxAmt" runat="server" CommitChanges="True" DataField="CuryTaxAmt" />
				<px:PXNumberEdit ID="edCuryOrigDiscAmt" runat="server" CommitChanges="True" DataField="CuryOrigDiscAmt" />
				<px:PXCheckBox ID="chkLCEnabled" runat="server" DataField="LCEnabled" />
			</px:PXPanel>
		</Template>
	</px:PXFormView>


	<style type="text/css">
		.leftDocTemplateCol {
			width: 50%;
			float: left;
			min-width: 90px;
		}

		.rightDocTemplateCol {
			margin-left: 51%;
			min-width: 90px;
		}
	</style>
	<px:PXGrid ID="docsTemplate" runat="server" Visible="false">
		<Levels>
			<px:PXGridLevel>
				<Columns>
					<px:PXGridColumn Key="Template">
						<CellTemplate>
							<div id="ParentDiv1" class="leftDocTemplateCol">
								<div id="div11" class="Field0"><%# ((PXGridCellContainer)Container).Text("refNbr") %></div>
								<div id="div12" class="Field1"><%# ((PXGridCellContainer)Container).Text("docDate") %></div>
							</div>
							<div id="ParentDiv2" class="rightDocTemplateCol">
								<span id="span21" class="Field1"><%# ((PXGridCellContainer)Container).Text("curyOrigDocAmt") %></span>
								<span id="span22" class="Field1"><%# ((PXGridCellContainer)Container).Text("curyID") %></span>
								<div id="div21" class="Field1"><%# ((PXGridCellContainer)Container).Text("status") %></div>
							</div>
							<div id="div3" class="Field1"><%# ((PXGridCellContainer)Container).Text("vendorID_Vendor_acctName") %></div>
						</CellTemplate>
					</px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="300px" Style="z-index: 100;" Width="100%">
		<Items>
			<px:PXTabItem Text="Document Details">
				<Template>
					<px:PXGrid ID="grid" runat="server" Style="z-index: 100;" Width="100%" Height="300px" SyncPosition="True" SkinID="DetailsInTab" DataSourceID="ds" TabIndex="18300" KeepPosition="True">
						<Levels>
							<px:PXGridLevel DataMember="Transactions">
								<Columns>
									<px:PXGridColumn DataField="BranchID" Width="100px" AutoCallBack="True" AllowShowHide="Server" />
									<px:PXGridColumn DataField="Date" />
									<px:PXGridColumn DataField="LineNbr" TextAlign="Right" Width="50px" />
									<px:PXGridColumn DataField="InventoryID" Width="100px" AutoCallBack="True" />
									<px:PXGridColumn DataField="POReceiptLine__SubItemID" Label="Subitem" />
									<px:PXGridColumn DataField="TranDesc" Width="200px" AutoCallBack="True" />
									<px:PXGridColumn DataField="Qty" TextAlign="Right" Width="81px" AutoCallBack="True" />
									<px:PXGridColumn DataField="UOM" Width="50px" AutoCallBack="True" />
									<px:PXGridColumn DataField="CuryUnitCost" TextAlign="Right" Width="81px" AutoCallBack="True" />
									<px:PXGridColumn DataField="CuryLineAmt" TextAlign="Right" Width="81px" AutoCallBack="True" />
									<px:PXGridColumn DataField="DiscPct" TextAlign="Right" AutoCallBack="True" />
									<px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" AutoCallBack="True" />
									<px:PXGridColumn DataField="CuryDiscCost" TextAlign="Right" AutoCallBack="True" />
									<px:PXGridColumn DataField="ManualDisc" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="DiscountID" RenderEditorText="True" TextAlign="Left" AllowShowHide="Server" Width="90px" AutoCallBack="True" />
									<px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" Width="90px" />
									<px:PXGridColumn DataField="CuryTranAmt" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="AccountID" Width="100px" AutoCallBack="True" />
									<px:PXGridColumn DataField="AccountID_Account_description" Width="100px" SyncVisibility="false" />
									<px:PXGridColumn DataField="SubID" Width="200px" AutoCallBack="True" />
									<px:PXGridColumn DataField="ProjectID" Label="Project" Width="100px" AutoCallBack="True" />
									<px:PXGridColumn DataField="TaskID" Label="Task" Width="100px" AutoCallBack="True" />
									<px:PXGridColumn DataField="NonBillable" Label="Non Billable" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="Box1099" Width="200px" AllowShowHide="Server" />
									<px:PXGridColumn DataField="DeferredCode" Width="50px" AutoCallBack="True" />
									<px:PXGridColumn DataField="DefScheduleID" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="TaxCategoryID" Width="50px" AutoCallBack="True" />
									<px:PXGridColumn DataField="POOrderType" />
									<px:PXGridColumn DataField="PONbr" />
									<px:PXGridColumn DataField="POLineNbr" TextAlign="Right" />
									<px:PXGridColumn DataField="ReceiptNbr" />
									<px:PXGridColumn DataField="ReceiptLineNbr" TextAlign="Right" />
									<px:PXGridColumn DataField="PPVDocType" />
									<px:PXGridColumn DataField="PPVRefNbr" TextAlign="Right" />
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" AllowAddNew="True" AutoRefresh="True" />
									<px:PXSegmentMask ID="edPOReceiptLine__SubItemID" runat="server" DataField="POReceiptLine__SubItemID" Enabled="False" />
									<px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" />
									<px:PXNumberEdit ID="edQty" runat="server" DataField="Qty" />
									<px:PXNumberEdit ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" />
									<px:PXNumberEdit ID="edCuryLineAmt" runat="server" DataField="CuryLineAmt" />
									<px:PXSelector ID="edDiscountCode" runat="server" DataField="DiscountID" CommitChanges="True" AllowEdit="True" />
									<px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" />
									<px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" />
									<px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="true" />
									<px:PXNumberEdit ID="edCuryTranAmt" runat="server" DataField="CuryTranAmt" Enabled="False" />
									<px:PXSegmentMask CommitChanges="True" ID="edAccountID" runat="server" DataField="AccountID" AutoRefresh="True" />
									<px:PXSegmentMask ID="edSubID" runat="server" DataField="SubID" AutoRefresh="True" />
									<px:PXDropDown ID="edBox1099" runat="server" DataField="Box1099" />
									<px:PXLayoutRule runat="server" ColumnSpan="2" />
									<px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
									<px:PXSelector ID="edDeferredCode" runat="server" DataField="DeferredCode" />
									<px:PXSelector ID="edDefScheduleID" runat="server" DataField="DefScheduleID" AutoRefresh="True" />
									<px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" CommitChanges="True" AutoRefresh="True" />
									<px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" />
									<px:PXSegmentMask CommitChanges="True" ID="edTaskID" runat="server" DataField="TaskID" AutoRefresh="True" />
									<px:PXDropDown ID="edPOOrderType" runat="server" DataField="POOrderType" Enabled="False" />
									<px:PXSelector ID="edPONbr" runat="server" DataField="PONbr" Enabled="False" AllowEdit="True" />
									<px:PXSelector ID="edReceiptNbr" runat="server" DataField="ReceiptNbr" Enabled="False" AllowEdit="True" />
									<px:PXDropDown ID="edPPVDocType" runat="server" DataField="PPVDocType" Enabled="False" AllowEdit="True" />
									<px:PXSelector ID="edPPVRefNbr" runat="server" DataField="PPVRefNbr" Enabled="False" AllowEdit="True" />
								</RowTemplate>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
						<Mode InitNewRow="True" AllowFormEdit="True" AllowUpload="True" AutoInsertField="CuryLineAmt" />
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton Text="View Schedule" Key="cmdViewSchedule">
									<AutoCallBack Command="ViewSchedule" Target="ds" />
									<PopupCommand Command="Cancel" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Add PO Receipt" Key="cmdRT">
									<AutoCallBack Command="AddPOReceipt" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Add PO Receipt Line" Key="cmdRTL">
									<AutoCallBack Command="AddReceiptLine" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Add PO Order" Key="cmdPO">
									<AutoCallBack Command="AddPOOrder" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton StateColumn="InventoryID">
									<AutoCallBack Command="LinkLine" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Financial Details" LoadOnDemand="false">
				<Template>
					<px:PXFormView ID="form2" runat="server" Style="z-index: 100" Width="100%" DataMember="CurrentDocument" CaptionVisible="False" SkinID="Transparent" DataSourceID="ds" TabIndex="18500">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Link to GL" StartGroup="True" />
							<px:PXSelector ID="edBatchNbr" runat="server" DataField="BatchNbr" Enabled="False" AllowEdit="True" DataSourceID="ds" />
							<px:PXSelector ID="edPrebookBatchNbr" runat="server" DataField="PrebookBatchNbr" Enabled="False" AllowEdit="true" DataSourceID="ds" />
							<px:PXSelector ID="edVoidBatchNbr" runat="server" DataField="VoidBatchNbr" Enabled="False" AllowEdit="true" DataSourceID="ds" />
							<px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" DataSourceID="ds" />
							<px:PXSegmentMask ID="edAPAccountID" runat="server" DataField="APAccountID" CommitChanges="True" DataSourceID="ds" />
							<px:PXSegmentMask ID="edAPSubID" runat="server" DataField="APSubID" CommitChanges="True" AutoRefresh="True" DataSourceID="ds" />
							<px:PXSegmentMask ID="edPrebookAcctID" runat="server" DataField="PrebookAcctID" CommitChanges="True" DataSourceID="ds" />
							<px:PXSegmentMask ID="edPrebookSubID" runat="server" DataField="PrebookSubID" CommitChanges="True" AutoRefresh="True" DataSourceID="ds" />
							<px:PXLayoutRule runat="server" GroupCaption="Default Payment Info" StartGroup="True" />
							<px:PXCheckBox ID="chkSeparateCheck" runat="server" DataField="SeparateCheck" />
							<px:PXCheckBox CommitChanges="True" ID="chkPaySel" runat="server" DataField="PaySel" />
							<px:PXDateTimeEdit ID="edPayDate" runat="server" DataField="PayDate" />
							<px:PXSegmentMask CommitChanges="True" ID="edPayLocationID" runat="server" AutoRefresh="True" DataField="PayLocationID" DataSourceID="ds" />
							<px:PXSelector CommitChanges="True" ID="edPayTypeID" runat="server" DataField="PayTypeID" DataSourceID="ds" />
							<px:PXSegmentMask CommitChanges="True" ID="edPayAccountID" runat="server" DataField="PayAccountID" DataSourceID="ds" />
							<px:PXLayoutRule runat="server" ControlSize="XM" GroupCaption="Tax" LabelsWidth="SM" StartColumn="True" StartGroup="True" />
							<px:PXSelector CommitChanges="True" ID="edTaxZoneID" runat="server" DataField="TaxZoneID" DataSourceID="ds" />
							<px:PXCheckBox ID="chkUsesManualVAT" runat="server" DataField="UsesManualVAT" Enabled="false" />
                            <px:PXDropDown runat="server" ID="edTaxCalcMode" DataField="TaxCalcMode" CommitChanges="true" />
                            <px:PXLayoutRule runat="server" ControlSize="XM" GroupCaption="Assigned To" LabelsWidth="SM" StartGroup="True" />
                            <px:PXSelector ID="edEmployeeWorkgroupID" CommitChanges="true" runat="server" AutoRefresh="True" DataField="EmployeeWorkgroupID" DataSourceID="ds" />
                            <px:PXSelector ID="edEmployeeID" CommitChanges="true" runat="server" AutoRefresh="True" DataField="EmployeeID" DataSourceID="ds" />
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="S" GroupCaption="Voucher Details" />
                            <px:PXFormView ID="VoucherDetails" runat="server" RenderStyle="Simple"
                                DataMember="Voucher" DataSourceID="ds" TabIndex="1100">
                                <Template>
                                    <px:PXTextEdit ID="linkGLVoucherBatch" runat="server" DataField="VoucherBatchNbr" Enabled="false">
                                        <LinkCommand Target="ds" Command="ViewVoucherBatch"></LinkCommand>
                                    </px:PXTextEdit>
                                    <px:PXTextEdit ID="linkGLWorkBook" runat="server" DataField="WorkBookID" Enabled="false">
                                        <LinkCommand Target="ds" Command="ViewWorkBook"></LinkCommand>
                                    </px:PXTextEdit>
                                </Template>
                            </px:PXFormView>
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Tax Details">
				<Template>
					<px:PXGrid ID="grid1" runat="server" Style="z-index: 100;" Height="300px"
						Width="100%" ActionsPosition="Top" SkinID="DetailsInTab" DataSourceID="ds"
						TabIndex="3900">
						<AutoSize Enabled="True" MinHeight="150" />
						<LevelStyles>
							<RowForm Width="300px">
							</RowForm>
						</LevelStyles>
						<ActionBar>
							<Actions>
								<Search Enabled="False" />
								<Save Enabled="False" />
							</Actions>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Taxes">
								<Columns>
									<px:PXGridColumn DataField="TaxID" Width="100px" />
									<px:PXGridColumn DataField="TaxRate" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="CuryTaxableAmt" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="CuryTaxAmt" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="NonDeductibleTaxRate" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="CuryExpenseAmt" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="Tax__TaxType"/>
									<px:PXGridColumn DataField="Tax__PendingTax" TextAlign="Center" Type="CheckBox" Width="60px"/>
									<px:PXGridColumn DataField="Tax__ReverseTax" TextAlign="Center" Type="CheckBox" Width="60px"/>
									<px:PXGridColumn DataField="Tax__ExemptTax" TextAlign="Center" Type="CheckBox" Width="60px"/>
									<px:PXGridColumn DataField="Tax__StatisticalTax" TextAlign="Center" Type="CheckBox" Width="60px"/>
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Landed Costs" BindingContext="form"  LoadOnDemand="True" VisibleExp="DataControls[&quot;chkLCEnabled&quot;].Value = 1">
				<Template>
					<px:PXGrid ID="gridLCTran" runat="server" Style="z-index: 100; left: 0px; top: 0px; height: 269px;" Width="100%" ActionsPosition="Top" BorderWidth="0px" SkinID="Details" DataSourceID="ds" Height="269px"
						TabIndex="18100">
						<Levels>
							<px:PXGridLevel DataMember="landedCostTrans" SortOrder="GroupTranID">
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
									<px:PXSelector CommitChanges="True" ID="edLandedCostCodeID" runat="server" AutoRefresh="True" DataField="LandedCostCodeID" />
									<px:PXNumberEdit ID="edCuryLCAmount" runat="server" DataField="CuryLCAmount" />
									<px:PXDropDown CommitChanges="True" ID="edPOReceiptType" runat="server" DataField="POReceiptType" />
									<px:PXSelector CommitChanges="True" ID="edPOReceiptNbr" runat="server" DataField="POReceiptNbr" AllowEdit="true" AutoRefresh="True" />
									<px:PXSegmentMask ID="edInventoryID1" runat="server" DataField="InventoryID" AutoRefresh="True" />
									<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
									<px:PXSelector ID="edTaxCategoryID1" runat="server" DataField="TaxCategoryID" AutoRefresh="True" />
									<px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" />
									<px:PXSegmentMask Size="xs" ID="edVendorID" runat="server" DataField="VendorID" />
									<px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True" />
									<px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" />
									<px:PXNumberEdit CommitChanges="True" ID="edLCTranID" runat="server" DataField="LCTranID" Enabled="False" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="LCTranID" TextAlign="Right" Visible="False" />
									<px:PXGridColumn DataField="LandedCostCodeID" Width="117px" AutoCallBack="True" />
									<px:PXGridColumn DataField="Descr" Width="351px" />
									<px:PXGridColumn DataField="VendorID" Width="81px" AutoCallBack="True" />
									<px:PXGridColumn DataField="VendorLocationID" Width="54px" />
									<px:PXGridColumn DataField="CuryLCAPEffAmount" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="TaxCategoryID" Label="Tax Category" Width="81px" />
									<px:PXGridColumn DataField="POReceiptType" Width="110px" AutoCallBack="true" CommitChanges="true" />
									<px:PXGridColumn DataField="POReceiptNbr" AutoCallBack="true" Width="115px" NullText="&lt;SPLIT&gt;" />
									<px:PXGridColumn DataField="InventoryID" Width="115px" />
									<px:PXGridColumn DataField="SiteID" Width="81px" AutoCallBack="True" />
									<px:PXGridColumn DataField="LocationID" Width="81px" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
						<Mode InitNewRow="True" AllowFormEdit="True" />
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton Text="View PO Receipt">
									<AutoCallBack Command="ViewLCPOReceipt" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="View LC IN Adjustment" Key="cmdViewLCTran">
									<AutoCallBack Command="ViewLCINDocument" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Add Posted Landed Cost" Key="cmdAddPLCT">
									<AutoCallBack Command="AddPostLandedCostTran" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="LC Splits" Key="cmdLS" CommandName="LsLCSplits" CommandSourceID="ds" DependOnGrid="gridLCTran" />
							</CustomItems>
						</ActionBar>
						<AutoSize Enabled="True" MinHeight="150" />
						<Mode InitNewRow="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Approval Details" BindingContext="form" VisibleExp="DataControls[&quot;chkRequestApproval&quot;].Value = 1" RepaintOnDemand="false">
				<Template>
					<px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True" Style="left: 0; top: 0;">
						<AutoSize Enabled="True" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						<Levels>
							<px:PXGridLevel DataMember="Approval">
								<Columns>
									<px:PXGridColumn DataField="ApproverEmployee__AcctCD" Width="160px" />
									<px:PXGridColumn DataField="ApproverEmployee__AcctName" Width="160px" />
									<px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" Width="100px" />
									<px:PXGridColumn DataField="ApprovedByEmployee__AcctName" Width="160px" />
									<px:PXGridColumn DataField="ApproveDate" Width="90px" />
									<px:PXGridColumn DataField="Status" AllowNull="False" AllowUpdate="False" RenderEditorText="True" />
									<px:PXGridColumn DataField="WorkgroupID" Width="150px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Discount Details">
				<Template>
					<px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None">
						<Levels>
							<px:PXGridLevel DataMember="DiscountDetails" DataKeyNames="DocType,RefNbr,DiscountID,DiscountSequenceID,Type">
								<RowTemplate>
									<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
									<px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
									<px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID"
										AllowEdit="True" edit="1" />
									<px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
									<px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
									<px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AutoRefresh="true" AllowEdit="true" />
									<px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
									<px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
									<px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" />
									<px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" />
									<px:PXSelector ID="edPOReceiptRefNbr" runat="server" DataField="ReceiptNbr" Enabled="false" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="SkipDiscount" Width="75px" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="DiscountID" Width="90px" CommitChanges="true" />
									<px:PXGridColumn DataField="DiscountSequenceID" Width="90px" CommitChanges="true" />
									<px:PXGridColumn DataField="Type" RenderEditorText="True" Width="90px" />
									<px:PXGridColumn DataField="IsManual" Width="75px" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" Width="90px" />
									<px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" Width="90px" />
									<px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="DiscountPct" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="OrderNbr" TextAlign="Right" Width="81px" />
									<px:PXGridColumn DataField="ReceiptNbr" TextAlign="Right" Width="81px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Applications">
				<Template>
					<px:PXGrid ID="detgrid" runat="server" Style="z-index: 100;" Width="100%" Height="300px" SkinID="DetailsInTab">
						<ActionBar DefaultAction="ViewPayment">
							<CustomItems>
								<px:PXToolBarButton Text="Auto Apply" Tooltip="Auto Apply">
									<AutoCallBack Command="AutoApply" Target="ds">
										<Behavior CommitChanges="True" />
									</AutoCallBack>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="Adjustments">
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXNumberEdit CommitChanges="True" ID="edCuryAdjdAmt" runat="server" DataField="CuryAdjdAmt" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="DisplayDocType" Width="100px" Type="DropDownList" />
									<px:PXGridColumn DataField="DisplayRefNbr" Width="100px" AutoCallBack="True" LinkCommand="ViewPayment" />
									<px:PXGridColumn DataField="CuryAdjdAmt" AutoCallBack="True" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="DisplayDocDate" Width="90px" />
									<px:PXGridColumn DataField="CuryDocBal" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="DisplayDocDesc" Width="250px" />
									<px:PXGridColumn DataField="DisplayCuryID" Width="50px" />
									<px:PXGridColumn DataField="DisplayFinPeriodID" />
									<px:PXGridColumn DataField="APPayment__ExtRefNbr" Width="90px" />
									<px:PXGridColumn DataField="AdjdDocType" Width="18px" />
									<px:PXGridColumn DataField="AdjdRefNbr" Width="90px" />
									<px:PXGridColumn DataField="DisplayStatus" Label="Status" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<CallbackCommands>
			<Search CommitChanges="True" PostData="Page" />
			<Refresh CommitChanges="True" PostData="Page" />
		</CallbackCommands>
		<AutoSize Container="Window" Enabled="True" MinHeight="180" />
	</px:PXTab>
	<px:PXSmartPanel ID="PanelAddPOReceipt" runat="server" Style="z-index: 108;" Width="900px" Height="400px"
		Key="poreceiptslist" AutoCallBack-Command="Refresh" AutoCallBack-Target="grid4" CommandSourceID="ds"
		Caption="Add PO Receipt" CaptionVisible="True" LoadOnDemand="True" ContentLayout-OuterSpacing="None">
		<px:PXFormView ID="frmPOrderFilter" runat="server" DataMember="filter" Style="z-index: 100;" Width="100%" CaptionVisible="false" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="MS" ControlSize="M" />
				<px:PXSelector CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="grid4" runat="server" Width="100%" Height="200px" BatchUpdate="True" SkinID="Inquire"
			DataSourceID="ds" FastFilterFields="ReceiptNbr" TabIndex="16900">
			<Levels>
				<px:PXGridLevel DataMember="poreceiptslist" DataKeyNames="ReceiptType,ReceiptNbr">
					<Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowMove="False" AllowSort="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="26px" />
						<px:PXGridColumn DataField="ReceiptNbr" Width="108px" />
						<px:PXGridColumn DataField="ReceiptType" />
						<px:PXGridColumn DataField="VendorID" Width="81px" />
						<px:PXGridColumn DataField="VendorLocationID" Width="63px" />
						<px:PXGridColumn DataField="CuryID" Width="60px" />
						<px:PXGridColumn DataField="ReceiptDate" Width="90px" />
						<px:PXGridColumn DataField="OrderQty" TextAlign="Right" Width="81px" />
						<px:PXGridColumn DataField="CuryOrderTotal" TextAlign="Right" Width="100px" />
						<px:PXGridColumn DataField="UnbilledQty" TextAlign="Right" Width="100px" />
						<px:PXGridColumn DataField="CuryUnbilledTotal" TextAlign="Right" Width="100px" />
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Add & Close" />
			<px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelAddPOReceiptLine" runat="server" Caption="Add Receipt Line" CaptionVisible="True"
		Height="400px" Width="900px" Key="poReceiptLinesSelection" LoadOnDemand="True" ShowAfterLoad="True" AutoCallBack-Command="Refresh" AutoCallBack-Target="gridOL">
		<px:PXFormView ID="frmPOFilter" runat="server" DataMember="filter" Style="z-index: 100;" Width="100%" CaptionVisible="false" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="MS" ControlSize="M" />
				<px:PXSelector CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="gridOL" runat="server" Height="200px" Width="100%" SkinID="Inquire" DataSourceID="ds"
			FastFilterFields="InvenotryID,TranDesc" TabIndex="17300">
			<Levels>
				<px:PXGridLevel DataMember="poReceiptLinesSelection">
					<Columns>
						<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" TextAlign="Center" Width="26px" AutoCallBack="True" CommitChanges="True" AllowResize="False" AllowShowHide="False" />
						<px:PXGridColumn DataField="PONbr" Width="100px" />
						<px:PXGridColumn DataField="POType" Width="70px" />
						<px:PXGridColumn DataField="ReceiptNbr" Width="100px" />
						<px:PXGridColumn DataField="POReceipt__InvoiceNbr" Width="80px" />
						<px:PXGridColumn DataField="InventoryID" Width="100px" />
						<px:PXGridColumn DataField="SubItemID" Width="70px" />
						<px:PXGridColumn DataField="SiteID" Width="100px" />
						<px:PXGridColumn DataField="UOM" />
						<px:PXGridColumn DataField="LineNbr" Width="70px" />
						<px:PXGridColumn DataField="POReceipt__CuryID" Width="70px" />
						<px:PXGridColumn DataField="ReceiptQty" Width="50px" />
						<px:PXGridColumn DataField="CuryExtCost" Width="95px" />
						<px:PXGridColumn DataField="UnbilledQty" Width="50px" />
						<px:PXGridColumn DataField="TotalCuryUnbilledAmount" Width="95px" />
						<px:PXGridColumn DataField="TranDesc" Width="200px" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton3" runat="server" Text="Add" SyncVisible="false">
				<AutoCallBack Command="AddReceiptLine2" Target="ds" />
			</px:PXButton>
			<px:PXButton ID="PXButton4" runat="server" DialogResult="OK" Text="Add & Close" />
			<px:PXButton ID="PXButton5" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelAddPOOrder" runat="server" Width="800px" Height="400px" Key="poorderslist" CommandSourceID="ds"
		Caption="Add PO Order" CaptionVisible="True" LoadOnDemand="True" AutoCallBack-Command="Refresh" AutoCallBack-Target="PXGrid1">
		<px:PXGrid ID="PXGrid1" runat="server" Height="200px" Width="100%" BatchUpdate="True" SkinID="Inquire"
			DataSourceID="ds" FastFilterFields="OrderNbr" TabIndex="17500">
			<AutoSize Enabled="true" />
			<Levels>
				<px:PXGridLevel DataMember="poorderslist">
					<Columns>
						<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" TextAlign="Center" AllowResize="False" Width="26px" />
						<px:PXGridColumn DataField="OrderNbr" Width="100px" />
						<px:PXGridColumn DataField="OrderType" />
						<px:PXGridColumn DataField="VendorID" Width="100px" />
						<px:PXGridColumn DataField="VendorLocationID" Width="80px" />
						<px:PXGridColumn DataField="OrderDate" Width="90px" />
						<px:PXGridColumn DataField="CuryID" Width="60px" />
						<px:PXGridColumn DataField="CuryOrderTotal" TextAlign="Right" Width="81px" />
						<px:PXGridColumn DataField="LeftToBillQty" TextAlign="Right" Width="81px" />
						<px:PXGridColumn DataField="CuryLeftToBillCost" TextAlign="Right" Width="81px" />
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
		</px:PXGrid>
		<px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton6" runat="server" Text="Add" SyncVisible="false">
				<AutoCallBack Command="AddPOOrder2" Target="ds" />
			</px:PXButton>
			<px:PXButton ID="PXButton7" runat="server" DialogResult="OK" Text="Add & Close" />
			<px:PXButton ID="PXButton8" runat="server" DialogResult="No" Text="Cancel" SyncVisible="false" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelAddPostLandedCostTran" runat="server" Width="800px" Height="400px" Style="z-index: 108;" Key="landedCostTranSelection" AutoCallBack-Command="Refresh" AutoCallBack-Target="gridLCSelection"
		CommandSourceID="ds" Caption="Add Postponed Landed Cost" CaptionVisible="True" LoadOnDemand="True">
		<px:PXGrid ID="gridLCSelection" runat="server" Height="200px" Width="100%" BatchUpdate="True" SkinID="Inquire"
			DataSourceID="ds" FastFilterFields="POReceiptNbr,VendorID" TabIndex="17700">
			<AutoSize Enabled="true" />
			<Levels>
				<px:PXGridLevel DataMember="landedCostTranSelection">
					<Columns>
                        <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="26px" />
						<px:PXGridColumn DataField="LCTranID" Width="100px" />
						<px:PXGridColumn DataField="LandedCostCodeID" />
						<px:PXGridColumn DataField="VendorID" Width="100px" />
						<px:PXGridColumn DataField="VendorLocationID" Width="80px" />
						<px:PXGridColumn DataField="POReceiptNbr" Width="100px" />
						<px:PXGridColumn DataField="CuryLCAmount" TextAlign="Right" Width="100px" />
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
		</px:PXGrid>
		<px:PXPanel ID="PXPanel4" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton9" runat="server" DialogResult="OK" Text="Add & Close" />
			<px:PXButton ID="PXButton10" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelLCS" runat="server" Style="z-index: 108;" Width="800px" Height="400px" Caption="Landed Cost Splits"
		CaptionVisible="True" Key="landedCostTrans" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True"
		AutoCallBack-Target="grdLCSplit">
		<px:PXGrid ID="grdLCSplit" runat="server" Width="100%" Height="240px" SkinID="Details" DataSourceID="ds" TabIndex="17900">
			<Mode InitNewRow="True" />
			<Parameters>
				<px:PXSyncGridParam ControlID="gridLCTran" />
			</Parameters>
			<Levels>
				<px:PXGridLevel DataMember="LCTranSplit">
					<RowTemplate>
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="MS" ControlSize="M" />
						<px:PXSelector CommitChanges="True" ID="edPOReceiptNbr2" runat="server" DataField="POReceiptNbr" AutoRefresh="True" />
						<px:PXSegmentMask ID="edInventoryID2" runat="server" DataField="InventoryID" AutoRefresh="True" />
					</RowTemplate>
					<Columns>
						<px:PXGridColumn DataField="POReceiptNbr" Label="PO Receipt Nbr." AutoCallBack="True" Width="100px" />
						<px:PXGridColumn DataField="InventoryID" Label="Inventory ID" Width="100px" />
						<px:PXGridColumn DataField="Descr" Width="250px">
						</px:PXGridColumn>
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel5" SkinID="Buttons" runat="server">
			<px:PXButton ID="PXButton11" runat="server" DialogResult="OK" Text="Ok" />
			<px:PXButton ID="PXButton12" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="panelDuplicate" runat="server" Caption="Duplicate Reference Nbr." CaptionVisible="true" LoadOnDemand="true" Key="duplicatefilter"
		AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
		CallBackMode-PostData="Page">
		<div style="padding: 5px">
			<px:PXFormView ID="formCopyTo" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="duplicatefilter">
				<ContentStyle BackColor="Transparent" BorderStyle="None" />
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
					<px:PXLabel Size="xl" ID="lblMessage" runat="server">Record already exists. Please enter new Reference Nbr.</px:PXLabel>
					<px:PXMaskEdit CommitChanges="True" ID="edRefNbr" runat="server" DataField="RefNbr" InputMask="&gt;CCCCCCCCCCCCCCC" />
				</Template>
			</px:PXFormView>
		</div>
		<px:PXPanel ID="PXPanel7" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton14" runat="server" DialogResult="OK" Text="OK" CommandSourceID="ds" />
			<px:PXButton ID="PXButton15" runat="server" DialogResult="Cancel" Text="Cancel" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<%-- Recalculate Prices and Discounts --%>
	<px:PXSmartPanel ID="PanelRecalcDiscounts" runat="server" Caption="Recalculate Prices" CaptionVisible="true" LoadOnDemand="true" Key="recalcdiscountsfilter"
		AutoCallBack-Enabled="true" AutoCallBack-Target="formRecalcDiscounts" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
		CallBackMode-PostData="Page">
		<div style="padding: 5px">
			<px:PXFormView ID="formRecalcDiscounts" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="recalcdiscountsfilter">
				<Activity Height="" HighlightColor="" SelectedColor="" Width="" />
				<ContentStyle BackColor="Transparent" BorderStyle="None" />
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
					<px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" CommitChanges="true" />
					<px:PXCheckBox CommitChanges="True" ID="chkRecalcUnitPrices" runat="server" DataField="RecalcUnitPrices" />
					<px:PXCheckBox CommitChanges="True" ID="chkOverrideManualPrices" runat="server" DataField="OverrideManualPrices" />
					<px:PXCheckBox CommitChanges="True" ID="chkRecalcDiscounts" runat="server" DataField="RecalcDiscounts" />
					<px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" />
				</Template>
			</px:PXFormView>
		</div>
		<px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton13" runat="server" DialogResult="OK" Text="OK" CommandName="RecalcOk" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelLinkLine" runat="server" Width="1100px" Height="500px" Key="linkLineFilter" CommandSourceID="ds" Caption="Link Line" CaptionVisible="True" LoadOnDemand="False" AutoCallBack-Command="Refresh" AutoCallBack-Target="LinkLineFilterForm">
		<px:PXFormView runat="server" ID="LinkLineFilterForm" DataMember="linkLineFilter" Style="z-index: 100;" Width="100%" CaptionVisible="false" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule02" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSelector runat="server" ID="edPOOrderNbr" DataField="POOrderNbr" CommitChanges="True" AutoRefresh="True" />
				<px:PXSelector runat="server" ID="edSiteID" DataField="SiteID" CommitChanges="True" AutoRefresh="True" />
				<px:PXSelector runat="server" ID="edInventoryID" DataField="InventoryID" />
				<px:PXSelector runat="server" ID="edUOM" DataField="UOM" />
				<px:PXLayoutRule ID="PXLayoutRule01" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
				<px:PXGroupBox CommitChanges="True" RenderStyle="RoundBorder" ID="gpMode" runat="server" Caption="Selected Mode" DataField="SelectedMode">
					<Template>
						<px:PXRadioButton runat="server" ID="rOrder" Value="O" />
						<px:PXRadioButton runat="server" ID="rReceipt" Value="R" />
					</Template>
				</px:PXGroupBox>
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="LinkLineGrid" runat="server" Height="200px" Width="100%" BatchUpdate="False" SkinID="Inquire" DataSourceID="ds" FastFilterFields="PONbr,ReceiptNbr,POReceipt__InvoiceNbr,TranDesc,SiteID" TabIndex="17500" FilesIndicator="False" NoteIndicator="False">
			<AutoSize Enabled="true" />
			<Levels>
				<px:PXGridLevel DataMember="linkLineReceiptTran">
					<Columns>
						<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="False" AllowSort="False" AllowMove="False" TextAlign="Center" Width="20px" AutoCallBack="True" CommitChanges="True" AllowResize="False" AllowShowHide="False" />
						<px:PXGridColumn DataField="PONbr" Width="100px" />
						<px:PXGridColumn DataField="POType" Width="70px" />
						<px:PXGridColumn DataField="ReceiptNbr" Width="100px" />
						<px:PXGridColumn DataField="POReceipt__InvoiceNbr" Width="80px" />
						<px:PXGridColumn DataField="SubItemID" Width="70px" />
						<px:PXGridColumn DataField="SiteID" Width="100px" />
						<px:PXGridColumn DataField="LineNbr" Width="70px" />
						<px:PXGridColumn DataField="POReceipt__CuryID" Width="70px" />
						<px:PXGridColumn DataField="ReceiptQty" Width="50px" />
						<px:PXGridColumn DataField="CuryExtCost" Width="95px" />
						<px:PXGridColumn DataField="UnbilledQty" Width="50px" />
						<px:PXGridColumn DataField="TotalCuryUnbilledAmount" Width="95px" />
						<px:PXGridColumn DataField="TranDesc" Width="200px" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<Mode AllowAddNew="False" AllowDelete="False" />
		</px:PXGrid>
		<px:PXGrid ID="LinkLineOrderGrid" runat="server" Height="200px" Width="100%" BatchUpdate="False" SkinID="Inquire" DataSourceID="ds" FastFilterFields="POOrder__OrderNbr,POOrder__VendorRefNbr,TranDesc,SiteID" TabIndex="17500" FilesIndicator="False" NoteIndicator="False">
			<AutoSize Enabled="true" />
			<Levels>
				<px:PXGridLevel DataMember="linkLineOrderTran">
					<Columns>
						<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="False" AllowSort="False" AllowMove="False" TextAlign="Center" Width="20px" AutoCallBack="True" CommitChanges="True" AllowResize="False" AllowShowHide="False" />
						<px:PXGridColumn DataField="POOrder__OrderNbr" Width="100px" />
						<px:PXGridColumn DataField="POOrder__OrderType" Width="100px" />
						<px:PXGridColumn DataField="LineNbr" Width="100px" />
						<px:PXGridColumn DataField="POOrder__VendorRefNbr" Width="80px" />
						<px:PXGridColumn DataField="SubItemID" Width="70px" />
						<px:PXGridColumn DataField="SiteID" Width="100px" />
						<px:PXGridColumn DataField="POOrder__CuryID" Width="70px" />
						<px:PXGridColumn DataField="OrderQty" Width="50px" />
						<px:PXGridColumn DataField="curyLineAmt" Width="95px" />
						<px:PXGridColumn DataField="OrderUnbilledQty" Width="50px" />
						<px:PXGridColumn DataField="OrderCuryUnbilledAmt" Width="95px" />
						<px:PXGridColumn DataField="TranDesc" Width="200px" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<Mode AllowAddNew="False" AllowDelete="False" />
		</px:PXGrid>
		<px:PXPanel ID="LinkLineButtons" runat="server" SkinID="Buttons">
			<px:PXButton ID="LinkLineSave" runat="server" DialogResult="Yes" Text="Save" />
			<px:PXButton ID="LinkLineCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>
