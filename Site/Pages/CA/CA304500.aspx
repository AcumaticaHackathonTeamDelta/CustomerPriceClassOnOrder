<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA304500.aspx.cs" Inherits="Page_CA304500"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="BankStatement" TypeName="PX.Objects.CA.CABankStatementEntry">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="AutoMatch" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="ValidateMatches" StartNewGroup="false" />
			<px:PXDSCallbackCommand Name="CreateAllDocuments" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="Import" Visible="false" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="UploadFile" StartNewGroup="true" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="MatchTrans" Visible="False" DependOnGrid="gridDetailMatches2" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="matchSettingsPanel" PopupPanel="pnlMatchSettings" Visible="True" />
			<px:PXDSCallbackCommand Name="ClearMatch" Visible="false" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="CreateDocument" Visible="false" CommitChanges="true" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="viewDoc" Visible="false" DependOnGrid="gridDetailMatches2" />
			<px:PXDSCallbackCommand Name="viewDetailsDoc" Visible="false" DependOnGrid="grid"  />

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>


<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXUploadDialog ID="pnlNewRev" Key="NewRevisionPanel" runat="server" Height="120px" Style="position: static" Width="560px" Caption="Statement File Upload" AutoSaveFile="false" RenderCheckIn="false" SessionKey="ImportStatementFile" />
	<px:PXTab ID="tab" runat="server" DataMember="BankStatement" Width="100%"
		DefaultControlID="edCashAccountID" FilesIndicator="True" NoteIndicator="True">
		<AutoSize Enabled="true" Container="Window" />
		<Items>
			<px:PXTabItem Text="Statement Summary">
				<Template>
					<px:PXPanel runat="server">
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="s" ControlSize="sm" />
						<px:PXSegmentMask ID="edCashAccountID" runat="server" DataField="CashAccountID" />
						<px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" />
						<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="s" ControlSize="s" />
						<px:PXDropDown Size="S" ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" />
						<px:PXCheckBox SuppressLabel="True" ID="chkReleased" runat="server" DataField="Released" />
						<px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkHold" runat="server" DataField="Hold" />
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="sm" ControlSize="s" />
						<px:PXDateTimeEdit Size="s" CommitChanges="True" ID="edStatementDate" runat="server" DataField="StatementDate" />
						<px:PXDateTimeEdit Size="s" ID="edStartBalanceDate" runat="server" DataField="StartBalanceDate" CommitChanges="true" />
						<px:PXDateTimeEdit Size="s" ID="edEndBalanceDate" runat="server" DataField="EndBalanceDate" CommitChanges="true" />
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="sm" ControlSize="s" />
						<px:PXSelector ID="edCuryID" runat="server" DataField="CuryID" Enabled="False" />
						<px:PXNumberEdit Size="s" CommitChanges="True" ID="edCuryBegBalance" runat="server" AllowNull="False" DataField="CuryBegBalance" />
						<px:PXNumberEdit Size="s" CommitChanges="True" ID="edCuryEndBalance" runat="server" AllowNull="False" DataField="CuryEndBalance" />
					</px:PXPanel>
					<px:PXSplitContainer runat="server" ID="PXSplitContainer1" SplitterPosition="350" SkinID="Horizontal" Panel1MinSize="120" Panel2MinSize="120" Height="400px">
						<AutoSize Enabled="true" />
						<Template1>
							<px:PXGrid ID="grid" runat="server" DataSourceID="ds" SkinID="Details" Caption="Statement Details" MatrixMode="True" NoteField="NoteText" Width="100%">
								<AutoCallBack Target="gridDetailMatches2" Command="Refresh"/>
								<Levels>
									<px:PXGridLevel DataMember="Details">
										<RowTemplate>
											<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
											<px:PXTextEdit Size="s" ID="edExtTranID" runat="server" DataField="ExtTranID" />
											<px:PXSelector ID="edEntryTypeID" runat="server" DataField="EntryTypeID" AutoRefresh="true">
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid" />
												</Parameters>
                                            </px:PXSelector>
											<px:PXDropDown Size="xxs" ID="edOrigModule" runat="server" DataField="OrigModule" Enabled="False" />
											<px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="False" />
											<px:PXSegmentMask ID="edPayeeLocationID" runat="server" DataField="PayeeLocationID">
												<Parameters>
													<px:PXSyncGridParam ControlID="grid" />
												</Parameters>
											</px:PXSegmentMask>
											<px:PXDropDown ID="edDrCr" runat="server" DataField="DrCr" />
											<px:PXDateTimeEdit ID="edTranDate" runat="server" DataField="TranDate" />
											<px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
											<px:PXDateTimeEdit Size="s" ID="edTranEntryDate" runat="server" DataField="TranEntryDate" />
											<px:PXSelector ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID" AutoRefresh="true" >
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid" />
												</Parameters>
                                            </px:PXSelector>
											<px:PXLayoutRule ID="PXLayoutRule5" runat="server" Merge="False" />
											<px:PXCheckBox ID="chkPayeeMatched" runat="server" DataField="PayeeMatched" />
											<px:PXSelector ID="edOrigCuryID" runat="server" DataField="OrigCuryID" />
											<px:PXNumberEdit ID="edCuryOrigAmt" runat="server" AllowNull="False" DataField="CuryOrigAmt" />
											<px:PXTextEdit ID="edExtRefNbr" runat="server" DataField="ExtRefNbr" />
											<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXLayoutRule ID="PXLayoutRule7" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />
											<px:PXLayoutRule ID="PXLayoutRule8" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeeName" runat="server" DataField="PayeeName" />
											<px:PXTextEdit ID="edPayeeAddress1" runat="server" DataField="PayeeAddress1" />
											<px:PXTextEdit ID="edPayeeCity" runat="server" DataField="PayeeCity" />
											<px:PXLayoutRule ID="PXLayoutRule9" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeeState" runat="server" DataField="PayeeState" />
											<px:PXLayoutRule ID="PXLayoutRule10" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeePostalCode" runat="server" DataField="PayeePostalCode" />
											<px:PXLayoutRule ID="PXLayoutRule11" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeePhone" runat="server" DataField="PayeePhone" />
											<px:PXTextEdit ID="edTranCode" runat="server" DataField="TranCode" />
											<px:PXSelector CommitChanges="True" ID="edPayeeBAccountID" runat="server" DataField="PayeeBAccountID" AutoRefresh="true" AutoRepaintColumns="true" AutoGenerateColumns="true" >
												<Parameters>
													<px:PXSyncGridParam ControlID="grid" />
												</Parameters>
											</px:PXSelector>
											<px:PXLayoutRule ID="PXLayoutRule12" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXSelector ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="true">
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid" />
												</Parameters>
                                            </px:PXSelector>
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="Selected" Type="CheckBox" Label="Matched" AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="48px" TextAlign="Center" />
											<px:PXGridColumn DataField="DocumentMatched" Type="CheckBox" Label="Matched" Width="48px" TextAlign="Center" />
											<px:PXGridColumn DataField="ExtTranID" Label="ExtTranID" Width="90px" />
											<px:PXGridColumn DataField="ExtRefNbr" Label="ExtRefNbr" Width="110px" />
											<px:PXGridColumn DataField="TranDate" Label="Tran Date" Width="90px" AutoCallBack="true" />
											<px:PXGridColumn DataField="TranEntryDate" Label="Tran Entry Date" Width="90px" />
											<px:PXGridColumn DataField="DrCr" Label="DrCr" Width="90px" RenderEditorText="True" />
											<px:PXGridColumn DataField="TranCode" Label="Tran Code" Width="63px" />
											<px:PXGridColumn DataField="CuryID" DisplayFormat="&gt;LLLLL" Label="Currency ID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryDebitAmt" Label="Debit" TextAlign="Right" Width="81px" AutoCallBack="true" />
											<px:PXGridColumn AllowNull="False" DataField="CuryCreditAmt" Label="Credit" TextAlign="Right" Width="81px" AutoCallBack="true" />
											<px:PXGridColumn DataField="OrigCuryID" DisplayFormat="&gt;LLLLL" Label="OrigCuryID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryOrigAmt" Label="Cury. Orig. Amt." TextAlign="Right" Width="81px" />
											<px:PXGridColumn DataField="TranDesc" Label="Tran Desc" Width="220px" />
											<px:PXGridColumn DataField="PayeeName" Label="Payee Name" Width="200px" />
											<px:PXGridColumn DataField="PayeeAddress1" Label="Payee Address1" Width="100px" />
											<px:PXGridColumn DataField="PayeeCity" Label="Payee City" Width="100px" />
											<px:PXGridColumn DataField="PayeeState" Label="Payee State" Width="90px" />
											<px:PXGridColumn DataField="PayeePostalCode" Label="Payee Postal Code" Width="90px" />
											<px:PXGridColumn DataField="PayeePhone" Label="Payee Phone" Width="90px" />
											<px:PXGridColumn DataField="CATranID" Label="CA Tran ID" Width="54px" />
											<px:PXGridColumn DataField="CreateDocument" Type="CheckBox" Label="Matched" AutoCallBack="true" Width="64px" TextAlign="Center" />
											<px:PXGridColumn AutoCallBack="True" DataField="OrigModule" Label="Module" RenderEditorText="True" Width="36px" />
											<px:PXGridColumn DataField="PayeeBAccountID" Label="PayeeBAccountID" Width="90px" AutoCallBack="True" MatrixMode="false" />
											<px:PXGridColumn DataField="PayeeBAccountID_BAccountR_acctName" Width="140px" />
											<px:PXGridColumn DataField="PayeeLocationID" Label="Location" Width="54px" DisplayFormat="&gt;AAAAAA" AutoCallBack="True" />
											<px:PXGridColumn AutoCallBack="True" DataField="PaymentMethodID" Label="Payment Method" RenderEditorText="True" Width="81px" />
											<px:PXGridColumn AutoCallBack="True" DataField="PMInstanceID" Label="Payment Method" TextAlign="Right" Width="120px" RenderEditorText="true" TextField="PMInstanceID_CustomerPaymentMethod_Descr" />
											<px:PXGridColumn AllowNull="False" DataField="PayeeMatched" Label="PayeeMatched" TextAlign="Center" Type="CheckBox" />
											<px:PXGridColumn DataField="EntryTypeID" DisplayFormat="&gt;aaaaaaaaaa" Label="Entry Type ID" Width="81px" MatrixMode="false" />
											<px:PXGridColumn AutoGenerateOption="NotSet" DataField="InvoiceInfo" Label="Invoice Nbr." Width="81px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="True" />
								<Mode AllowUpload="True" InitNewRow="True" />                                
								<ActionBar Position="Top" DefaultAction="ViewDetailsDoc">
									<Actions>
										<PageNext ToolBarVisible="Top" GroupIndex="5" />
										<PagePrev ToolBarVisible="Top" GroupIndex="5" />
										<PageFirst ToolBarVisible="Top" GroupIndex="5" />
										<PageLast ToolBarVisible="Top" GroupIndex="5" />
									</Actions>
									<CustomItems>
										<px:PXToolBarButton Text="Clear Match" Key="cmdLS" CommandName="ClearMatch" CommandSourceID="ds" DependOnGrid="grid">
											<ActionBar GroupIndex="1" Order="2" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="Create Document" Key="cmdLS" CommandName="CreateDocument" CommandSourceID="ds" DependOnGrid="grid">
											<ActionBar GroupIndex="1" Order="3" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="View Document" Key="cmdLS" CommandName="ViewDetailsDoc" CommandSourceID="ds" DependOnGrid="grid">
											<ActionBar GroupIndex="1" Order="4" />
										</px:PXToolBarButton>
									</CustomItems>
								</ActionBar>                                
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid ID="gridDetailMatches2" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" SkinID="Details" Caption="Matching CA Transactions">
								<Mode InitNewRow="True" />
								<Parameters>
									<px:PXSyncGridParam ControlID="grid" />
								</Parameters>
								<Levels>
									<px:PXGridLevel DataMember="DetailMatches">
										<RowTemplate>
											<px:PXLayoutRule ID="PXLayoutRule13" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXTextEdit ID="edOrigModule1" runat="server" DataField="OrigModule" />
											<px:PXDropDown ID="edOrigTranType1" runat="server" DataField="OrigTranType" />
											<px:PXLayoutRule ID="PXLayoutRule14" runat="server" Merge="True" />
											<px:PXLabel Size="xs" ID="lblOrigRefNbr1" runat="server" AllowEdit="True" Encode="True">Orig. Doc. Number:</px:PXLabel>
											<px:PXTextEdit Size="s" ID="edOrigRefNbr1" runat="server" DataField="OrigRefNbr" />
											<px:PXLayoutRule ID="PXLayoutRule15" runat="server" Merge="False" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="IsMatched" Label="IsMatched" TextAlign="Center" Type="CheckBox" Width="60px" AutoCallBack="True">
											</px:PXGridColumn>
											<px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="MatchRelevance" Label="Match Relevance" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="Released" Label="lblReleased" TextAlign="Center" Type="CheckBox" Width="60px">
											</px:PXGridColumn>
											<px:PXGridColumn DataField="OrigModule" Label="Module" Width="60px" AutoCallBack="true" />
											<px:PXGridColumn DataField="OrigTranType" Label="Tran. Type" RenderEditorText="True" />
											<px:PXGridColumn DataField="OrigRefNbr" Label="Orig. Doc. Number" Width="90px"/>
											<px:PXGridColumn DataField="ExtRefNbr" Label="Document Ref." Width="78px" />
											<px:PXGridColumn DataField="TranDate" Label="Doc. Date" Width="90px" />
											<px:PXGridColumn DataField="DrCr" Label="Disb. / Receipt" Width="78px" RenderEditorText="True" />
											<px:PXGridColumn DataField="ReferenceID" Label="Business Account" />
											<px:PXGridColumn DataField="ReferenceName" Label="Account Name" Width="180px" />
											<px:PXGridColumn DataField="TranDesc" Label="CATran-Description" Width="180px" />
											<px:PXGridColumn AllowNull="False" DataField="Reconciled" Label="Reconciled" TextAlign="Center" Type="CheckBox" Width="60px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryTranAmt" Label="Amount" TextAlign="Right" Width="100px" />
											<px:PXGridColumn AllowUpdate="False" DataField="CuryID" DisplayFormat="&gt;LLLLL" Label="Currency" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<ActionBar Position="Top" DefaultAction="ViewDoc">
									<Actions>
										<AddNew Enabled="False" MenuVisible="False" />
										<Delete Enabled="False" MenuVisible="False" />
										<PageNext ToolBarVisible="Top" GroupIndex="3" />
										<PagePrev ToolBarVisible="Top" GroupIndex="3" />
										<PageFirst ToolBarVisible="Top" GroupIndex="3" />
										<PageLast ToolBarVisible="Top" GroupIndex="3" />
									</Actions>
									<CustomItems>
										<px:PXToolBarButton Text="Match Selected" Key="cmdLS" CommandName="MatchTrans" CommandSourceID="ds" DependOnGrid="gridDetailMatches2">
											<ActionBar Order="1" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="View Document" Key="cmdLS" CommandName="ViewDoc" CommandSourceID="ds" DependOnGrid="gridDetailMatches2">
											<ActionBar Order="2" />
										</px:PXToolBarButton>
									</CustomItems>
								</ActionBar>                                
								<AutoSize Enabled="true" />
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Statement Balance">
				<Template>
					<px:PXPanel runat="server">
						<px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="s" ControlSize="xs" />
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="sm" />
						<px:PXLabel ID="PXLabel2" runat="server" Encode="True"></px:PXLabel>
						<px:PXNumberEdit Size="s" CommitChanges="True" ID="edCuryBegBalanceRO" runat="server" AllowNull="False" DataField="CuryBegBalanceRO" Enabled="false" />
						<px:PXNumberEdit Size="s" CommitChanges="True" ID="edCuryEndBalanceRO" runat="server" AllowNull="False" DataField="CuryEndBalanceRO" Enabled="false" />
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="xs" ControlSize="xs" />
						<px:PXLabel ID="PXLabel4" runat="server" Encode="True"></px:PXLabel>
						<px:PXLabel ID="PXLabel1" runat="server" Encode="True">Details:</px:PXLabel>
						<px:PXLabel ID="PXLabel3" runat="server" Encode="True">Reconciled:</px:PXLabel>
						<px:PXLayoutRule runat="server" StartColumn="True" />
						<px:PXLabel ID="PXLabel5" runat="server" Encode="True">Total Receipts:</px:PXLabel>
						<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXNumberEdit Size="xxs" ID="edCountDebit" runat="server" AllowNull="False" DataField="CountDebit" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryDebitsTotal" runat="server" AllowNull="False" DataField="CuryDebitsTotal" Enabled="False" SuppressLabel="True" />
						<px:PXLayoutRule runat="server" Merge="False" />
						<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXNumberEdit Size="xxs" ID="edReconciledCountDebit" runat="server" AllowNull="False" DataField="ReconciledCountDebit" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryReconciledDebits" runat="server" AllowNull="False" DataField="CuryReconciledDebits" Enabled="False" SuppressLabel="True" />
						<px:PXLayoutRule runat="server" Merge="False" />
						<px:PXLayoutRule runat="server" StartColumn="True" />
						<px:PXLabel ID="PXLabel6" runat="server" Encode="True">Total Disbursements:</px:PXLabel>
						<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXNumberEdit Size="xxs" ID="edCountCredit" runat="server" AllowNull="False" DataField="CountCredit" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryCreditsTotal" runat="server" AllowNull="False" DataField="CuryCreditsTotal" Enabled="False" SuppressLabel="True" />
						<px:PXLayoutRule runat="server" Merge="False" />
						<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXNumberEdit Size="xxs" ID="edReconciledCountCredit" runat="server" AllowNull="False" DataField="ReconciledCountCredit" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryReconciledCredits" runat="server" AllowNull="False" DataField="CuryReconciledCredits" Enabled="False" SuppressLabel="True" />
						<px:PXLayoutRule runat="server" Merge="False" />
						<px:PXLayoutRule runat="server" StartColumn="True" />
						<px:PXLabel ID="PXLabel7" runat="server" Encode="True">End Balance:</px:PXLabel>
						<px:PXNumberEdit Size="s" ID="edCuryDetailsEndBalance" runat="server" AllowNull="False" DataField="CuryDetailsEndBalance" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryReconciledEndBalance" runat="server" AllowNull="False" DataField="CuryReconciledEndBalance" Enabled="False" SuppressLabel="True" />
						<px:PXLayoutRule runat="server" StartColumn="True" />
						<px:PXLabel ID="PXLabel8" runat="server" Encode="True">Difference:</px:PXLabel>
						<px:PXNumberEdit Size="s" ID="edCuryDetailsBalanceDiff" runat="server" DataField="CuryDetailsBalanceDiff" Enabled="False" SuppressLabel="True" />
						<px:PXNumberEdit Size="s" ID="edCuryReconciledBalanceDiff" runat="server" DataField="CuryReconciledBalanceDiff" Enabled="False" SuppressLabel="True" />
					</px:PXPanel>
					<px:PXSplitContainer runat="server" ID="PXSplitContainer2" SplitterPosition="350" SkinID="Horizontal" Panel1MinSize="120" Panel2MinSize="120"  Height="400px">
						<AutoSize Enabled="true" />
						<Template1>
							<px:PXGrid ID="grid2" runat="server" DataSourceID="ds" SkinID="Details" Caption="Statement Details" MatrixMode="True" NoteField="NoteText" Width="100%">
								<AutoCallBack Target="gridDetailMatches3" Command="Refresh" />
								<Levels>
									<px:PXGridLevel DataMember="DetailsForBalance">
										<RowTemplate>
											<px:PXLayoutRule ID="PXLayoutRule012" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXLayoutRule ID="PXLayoutRule022" runat="server" Merge="True" />
											<px:PXTextEdit Size="s" ID="edExtTranID2" runat="server" DataField="ExtTranID" />
											<px:PXSelector ID="edEntryTypeID2" runat="server" DataField="EntryTypeID" AutoRefresh="true" DisplayMode="Text">
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid2" />
												</Parameters>
                                            </px:PXSelector>
											<px:PXDropDown Size="xxs" ID="edOrigModule2" runat="server" DataField="OrigModule" Enabled="False" />
											<px:PXLayoutRule ID="PXLayoutRule032" runat="server" Merge="False" />
											<px:PXSegmentMask ID="edPayeeLocationID2" runat="server" DataField="PayeeLocationID">
												<Parameters>
													<px:PXSyncGridParam ControlID="grid2" />
												</Parameters>
											</px:PXSegmentMask>
											<px:PXDropDown ID="edDrCr2" runat="server" DataField="DrCr" />
											<px:PXDateTimeEdit ID="edTranDate2" runat="server" DataField="TranDate" />
											<px:PXLayoutRule ID="PXLayoutRule42" runat="server" Merge="True" />
											<px:PXDateTimeEdit Size="s" ID="edTranEntryDate2" runat="server" DataField="TranEntryDate" />
											<px:PXSelector ID="edPaymentMethodID2" runat="server" DataField="PaymentMethodID" AutoRefresh="true" >
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid2" />
												</Parameters>
                                            </px:PXSelector>
											<px:PXLayoutRule ID="PXLayoutRule52" runat="server" Merge="False" />
											<px:PXCheckBox ID="chkPayeeMatched2" runat="server" DataField="PayeeMatched" />
											<px:PXSelector ID="edOrigCuryID2" runat="server" DataField="OrigCuryID" />
											<px:PXNumberEdit ID="edCuryOrigAmt2" runat="server" AllowNull="False" DataField="CuryOrigAmt" />
											<px:PXTextEdit ID="edExtRefNbr2" runat="server" DataField="ExtRefNbr" />
											<px:PXLayoutRule ID="PXLayoutRule62" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXLayoutRule ID="PXLayoutRule72" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edTranDesc2" runat="server" DataField="TranDesc" />
											<px:PXLayoutRule ID="PXLayoutRule82" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeeName2" runat="server" DataField="PayeeName" />
											<px:PXTextEdit ID="edPayeeAddress12" runat="server" DataField="PayeeAddress1" />
											<px:PXTextEdit ID="edPayeeCity2" runat="server" DataField="PayeeCity" />
											<px:PXLayoutRule ID="PXLayoutRule92" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeeState2" runat="server" DataField="PayeeState" />
											<px:PXLayoutRule ID="PXLayoutRule102" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeePostalCode2" runat="server" DataField="PayeePostalCode" />
											<px:PXLayoutRule ID="PXLayoutRule112" runat="server" ColumnSpan="2" />
											<px:PXTextEdit ID="edPayeePhone2" runat="server" DataField="PayeePhone" />
											<px:PXTextEdit ID="edTranCode2" runat="server" DataField="TranCode" />
											<px:PXSelector CommitChanges="True" ID="edPayeeBAccountID2" runat="server" DataField="PayeeBAccountID" AutoRefresh="true" AutoRepaintColumns="true" AutoGenerateColumns="true" >
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid2" />
												</Parameters>
                                            </px:PXSelector>
											<px:PXLayoutRule ID="PXLayoutRule122" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXSelector ID="edPMInstanceID2" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="true">
                                                <Parameters>
													<px:PXSyncGridParam ControlID="grid2" />
												</Parameters>
                                            </px:PXSelector>
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="Selected" Type="CheckBox" Label="Matched" AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="48px" TextAlign="Center" />
											<px:PXGridColumn DataField="DocumentMatched" Type="CheckBox" Label="Matched" Width="48px" TextAlign="Center" />
											<px:PXGridColumn DataField="ExtTranID" Label="ExtTranID" Width="90px" />
											<px:PXGridColumn DataField="ExtRefNbr" Label="ExtRefNbr" Width="110px" />
											<px:PXGridColumn DataField="TranDate" Label="Tran Date" Width="90px" AutoCallBack="true" />
											<px:PXGridColumn DataField="TranEntryDate" Label="Tran Entry Date" Width="90px" />
											<px:PXGridColumn DataField="DrCr" Label="DrCr" Width="90px" RenderEditorText="True" />
											<px:PXGridColumn DataField="TranCode" Label="Tran Code" Width="63px" />
											<px:PXGridColumn DataField="CuryID" DisplayFormat="&gt;LLLLL" Label="Currency ID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryDebitAmt" Label="Debit" TextAlign="Right" Width="81px" AutoCallBack="true" />
											<px:PXGridColumn AllowNull="False" DataField="CuryCreditAmt" Label="Credit" TextAlign="Right" Width="81px" AutoCallBack="true" />
											<px:PXGridColumn DataField="OrigCuryID" DisplayFormat="&gt;LLLLL" Label="OrigCuryID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryOrigAmt" Label="Cury. Orig. Amt." TextAlign="Right" Width="81px" />
											<px:PXGridColumn DataField="TranDesc" Label="Tran Desc" Width="220px" />
											<px:PXGridColumn DataField="PayeeName" Label="Payee Name" Width="200px" />
											<px:PXGridColumn DataField="PayeeAddress1" Label="Payee Address1" Width="100px" />
											<px:PXGridColumn DataField="PayeeCity" Label="Payee City" Width="100px" />
											<px:PXGridColumn DataField="PayeeState" Label="Payee State" Width="90px" />
											<px:PXGridColumn DataField="PayeePostalCode" Label="Payee Postal Code" Width="90px" />
											<px:PXGridColumn DataField="PayeePhone" Label="Payee Phone" Width="90px" />
											<px:PXGridColumn DataField="CATranID" Label="CA Tran ID" Width="54px" />
											<px:PXGridColumn DataField="CreateDocument" Type="CheckBox" Label="Matched" AutoCallBack="true" Width="64px" TextAlign="Center" />
											<px:PXGridColumn AutoCallBack="True" DataField="OrigModule" Label="Module" RenderEditorText="True" Width="36px" />
											<px:PXGridColumn DataField="PayeeBAccountID" Label="PayeeBAccountID" Width="90px" AutoCallBack="True" />
											<px:PXGridColumn DataField="PayeeBAccountID_BAccountR_acctName" Width="140px" />
											<px:PXGridColumn DataField="PayeeLocationID" Label="Location" Width="54px" DisplayFormat="&gt;AAAAAA" AutoCallBack="True" />
											<px:PXGridColumn AutoCallBack="True" DataField="PaymentMethodID" Label="Payment Method" RenderEditorText="True" Width="81px" />
											<px:PXGridColumn AutoCallBack="True" DataField="PMInstanceID" Label="Payment Method" TextAlign="Right" Width="120px" RenderEditorText="true" TextField="PMInstanceID_CustomerPaymentMethod_Descr" />
											<px:PXGridColumn AllowNull="False" DataField="PayeeMatched" Label="PayeeMatched" TextAlign="Center" Type="CheckBox" />
											<px:PXGridColumn DataField="EntryTypeID" DisplayFormat="&gt;aaaaaaaaaa" Label="Entry Type ID" Width="81px" />
											<px:PXGridColumn AutoGenerateOption="NotSet" DataField="InvoiceInfo" Label="Invoice Nbr." Width="81px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="True" />
								<Mode AllowUpload="False" InitNewRow="True" />
								<ActionBar Position="Top" DefaultAction="ViewDetailsDoc">
									<Actions>
										<PageNext ToolBarVisible="Top" GroupIndex="5" />
										<PagePrev ToolBarVisible="Top" GroupIndex="5" />
										<PageFirst ToolBarVisible="Top" GroupIndex="5" />
										<PageLast ToolBarVisible="Top" GroupIndex="5" />
									</Actions>
									<CustomItems>
										<px:PXToolBarButton Text="Clear Match" Key="cmdLS" CommandName="ClearMatch" CommandSourceID="ds" DependOnGrid="grid2">
											<ActionBar GroupIndex="1" Order="2" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="Create Document" Key="cmdLS" CommandName="CreateDocument" CommandSourceID="ds" DependOnGrid="grid2">
											<ActionBar GroupIndex="1" Order="3" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="View Document" Key="cmdLS" CommandName="ViewDetailsDoc" CommandSourceID="ds" DependOnGrid="grid2">
											<ActionBar GroupIndex="1" Order="4" />
										</px:PXToolBarButton>
									</CustomItems>
								</ActionBar>
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid ID="gridDetailMatches3" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" SkinID="Details" Caption="Matching CA Transactions">
								<Mode InitNewRow="True" />
								<Parameters>
									<px:PXSyncGridParam ControlID="grid2" />
								</Parameters>
								<Levels>
									<px:PXGridLevel DataMember="DetailMatches">
										<RowTemplate>
											<px:PXLayoutRule ID="PXLayoutRule132" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
											<px:PXTextEdit ID="edOrigModule12" runat="server" DataField="OrigModule" />
											<px:PXDropDown ID="edOrigTranType12" runat="server" DataField="OrigTranType" />
											<px:PXLayoutRule ID="PXLayoutRule142" runat="server" Merge="True" />
											<px:PXLabel Size="xs" ID="lblOrigRefNbr12" runat="server" AllowEdit="True" Encode="True">Orig. Doc. Number:</px:PXLabel>
											<px:PXTextEdit Size="s" ID="edOrigRefNbr12" runat="server" DataField="OrigRefNbr" />
											<px:PXLayoutRule ID="PXLayoutRule152" runat="server" Merge="False" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="IsMatched" Label="IsMatched" TextAlign="Center" Type="CheckBox" Width="60px" AutoCallBack="True">
											</px:PXGridColumn>
											<px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="MatchRelevance" Label="Match Relevance" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="Released" Label="lblReleased" TextAlign="Center" Type="CheckBox" Width="60px">
											</px:PXGridColumn>
											<px:PXGridColumn DataField="OrigModule" Label="Module" Width="60px" AutoCallBack="true" />
											<px:PXGridColumn DataField="OrigTranType" Label="Tran. Type" RenderEditorText="True" />
											<px:PXGridColumn DataField="OrigRefNbr" Label="Orig. Doc. Number" Width="90px" />
											<px:PXGridColumn DataField="ExtRefNbr" Label="Document Ref." Width="78px" />
											<px:PXGridColumn DataField="TranDate" Label="Doc. Date" Width="90px" />
											<px:PXGridColumn DataField="DrCr" Label="Disb. / Receipt" Width="78px" RenderEditorText="True" />
											<px:PXGridColumn DataField="ReferenceID" Label="Business Account" />
											<px:PXGridColumn DataField="ReferenceName" Label="Account Name" Width="180px" />
											<px:PXGridColumn DataField="TranDesc" Label="CATran-Description" Width="180px" />
											<px:PXGridColumn AllowNull="False" DataField="Reconciled" Label="Reconciled" TextAlign="Center" Type="CheckBox" Width="60px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryTranAmt" Label="Amount" TextAlign="Right" Width="100px" />
											<px:PXGridColumn AllowUpdate="False" DataField="CuryID" DisplayFormat="&gt;LLLLL" Label="Currency" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<ActionBar Position="Top" DefaultAction="ViewDoc">
									<Actions>
										<AddNew Enabled="False" MenuVisible="False" />
										<Delete Enabled="False" MenuVisible="False" />
										<PageNext ToolBarVisible="Top" GroupIndex="3" />
										<PagePrev ToolBarVisible="Top" GroupIndex="3" />
										<PageFirst ToolBarVisible="Top" GroupIndex="3" />
										<PageLast ToolBarVisible="Top" GroupIndex="3" />
									</Actions>
									<CustomItems>
										<px:PXToolBarButton Text="Match Selected" Key="cmdLS" CommandName="MatchTrans" CommandSourceID="ds" DependOnGrid="gridDetailMatches3">
											<ActionBar Order="1" />
										</px:PXToolBarButton>
										<px:PXToolBarButton Text="View Document" Key="cmdLS" CommandName="ViewDoc" CommandSourceID="ds" DependOnGrid="gridDetailMatches3">
											<ActionBar Order="2" />
										</px:PXToolBarButton>
									</CustomItems>
								</ActionBar>
								<AutoSize Enabled="true" />
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Payment Application">
				<Template>
					<px:PXSplitContainer runat="server" ID="PXSplitContainer3" SplitterPosition="350" SkinID="Horizontal" Panel1MinSize="120" Panel2MinSize="120" Height="400px">
						<AutoSize Enabled="true" />
						<Template1>
							<px:PXGrid ID="grid2" runat="server" DataSourceID="ds" SkinID="Details" Caption="Statement Details" MatrixMode="True" Width="100%" FilesIndicator ="false" NoteIndicator="false">
								<AutoCallBack Target="gridAdjustments" Command="Refresh" />
								<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
								<Levels>
									<px:PXGridLevel DataMember="ApplicationDetails">
										<RowTemplate>											
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="ExtTranID" Label="ExtTranID" Width="90px" />
											<px:PXGridColumn DataField="ExtRefNbr" Label="ExtRefNbr" Width="110px" />
											<px:PXGridColumn DataField="TranDate" Label="Tran Date" Width="90px" />											
											<px:PXGridColumn DataField="TranCode" Label="Tran Code" Width="63px" />
											<px:PXGridColumn DataField="CuryID" DisplayFormat="&gt;LLLLL" Label="Currency ID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryDebitAmt" Label="Debit" TextAlign="Right" Width="81px"/>
											<px:PXGridColumn AllowNull="False" DataField="CuryCreditAmt" Label="Credit" TextAlign="Right" Width="81px"/>
											<px:PXGridColumn DataField="OrigCuryID" DisplayFormat="&gt;LLLLL" Label="OrigCuryID" Width="54px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryOrigAmt" Label="Cury. Orig. Amt." TextAlign="Right" Width="81px" />
											<px:PXGridColumn DataField="TranDesc" Label="Tran Desc" Width="220px" />
											<px:PXGridColumn DataField="PayeeName" Label="Payee Name" Width="200px" />
											<px:PXGridColumn DataField="PayeeAddress1" Label="Payee Address1" Width="100px" />
											<px:PXGridColumn DataField="PayeeCity" Label="Payee City" Width="100px" />
											<px:PXGridColumn DataField="PayeeState" Label="Payee State" Width="90px" />
											<px:PXGridColumn DataField="PayeePostalCode" Label="Payee Postal Code" Width="90px" />
											<px:PXGridColumn DataField="PayeePhone" Label="Payee Phone" Width="90px" />
											<px:PXGridColumn DataField="CATranID" Label="CA Tran ID" Width="54px" />
											<px:PXGridColumn DataField="OrigModule" Label="Module" RenderEditorText="True" Width="36px" />
											<px:PXGridColumn DataField="PayeeBAccountID" Label="PayeeBAccountID" Width="90px"/>
											<px:PXGridColumn DataField="PayeeBAccountID_BAccountR_acctName" Width="140px" />
											<px:PXGridColumn DataField="PayeeLocationID" Label="Location" Width="54px" DisplayFormat="&gt;AAAAAA"/>
											<px:PXGridColumn DataField="PaymentMethodID" Label="Payment Method" RenderEditorText="True" Width="81px" />
											<px:PXGridColumn AutoGenerateOption="NotSet" DataField="InvoiceInfo" Label="Invoice Nbr." Width="81px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryTotalAmt" Label="Total Amount" TextAlign="Right" Width="81px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryApplAmt" Label="Applied Amount" TextAlign="Right" Width="81px" />
											<px:PXGridColumn AllowNull="False" DataField="CuryUnappliedBal" Label="Unapplied Bal" TextAlign="Right" Width="81px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="True" />
								<ActionBar>
									<Actions>
										<AddNew Enabled="False" MenuVisible="False" />
										<Delete Enabled="False" MenuVisible="False" />
										<EditRecord Enabled="False" MenuVisible="False" />
									</Actions>
								</ActionBar>
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid ID="gridAdjustments" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" SkinID="Details" Caption="AP/AR Adjustments">
								<Mode InitNewRow="True" />
								<Parameters>
									<px:PXSyncGridParam ControlID="grid2" />
								</Parameters>
								<Levels>
									<px:PXGridLevel DataMember="Adjustments">
										<RowTemplate>
											<px:PXLayoutRule ID="PXLayoutRule16" runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
											<px:PXDropDown ID="edAdjdDocType" runat="server" DataField="AdjdDocType" CommitChanges="True" />
										    <px:PXSelector ID="edAdjdRefNbr" runat="server" AutoRefresh="True" CommitChanges="True" DataField="AdjdRefNbr"/>
											<px:PXNumberEdit ID="edCuryAdjgAmt" runat="server" CommitChanges="True" DataField="CuryAdjgAmt" />
											<px:PXNumberEdit ID="edCuryAdjgDiscAmt" runat="server" CommitChanges="True" DataField="CuryAdjgDiscAmt" />
											<px:PXNumberEdit ID="edCuryAdjgWhTaxAmt" runat="server" CommitChanges="True" DataField="CuryAdjgWhTaxAmt" />
											<px:PXDateTimeEdit ID="edAdjdDocDate" runat="server" DataField="AdjdDocDate" Enabled="False" />
											<px:PXLayoutRule ID="PXLayoutRule25" runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
											<px:PXNumberEdit ID="edAdjdCuryRate" runat="server" CommitChanges="True" DataField="AdjdCuryRate" />
											<px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
											<px:PXNumberEdit ID="edCuryDiscBal" runat="server" DataField="CuryDiscBal" Enabled="False" />
											<px:PXNumberEdit ID="edAdjNbr" runat="server" DataField="AdjNbr" />
											<px:PXNumberEdit ID="edCuryWhTaxBal" runat="server" DataField="CuryWhTaxBal" Enabled="False" />--%>
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="AdjdDocType" Width="100px" Type="DropDownList" AutoCallBack="True" />
											<px:PXGridColumn AutoCallBack="True" DataField="AdjdRefNbr" Width="100px" />
											<px:PXGridColumn AutoCallBack="True" DataField="CuryAdjgAmt" TextAlign="Right" Width="100px" />
											<px:PXGridColumn AutoCallBack="True" DataField="CuryAdjgDiscAmt" TextAlign="Right" Width="100px" />
											<px:PXGridColumn AutoCallBack="True" DataField="CuryAdjgWhTaxAmt" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="AdjdDocDate" Width="100px" />
											<px:PXGridColumn AutoCallBack="True" DataField="AdjdCuryRate" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="CuryDocBal" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="CuryDiscBal" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="CuryWhTaxBal" TextAlign="Right" Width="100px" />
											<px:PXGridColumn DataField="AdjdCuryID" Width="50px" />
											<px:PXGridColumn DataField="AdjdFinPeriodID" />
											<px:PXGridColumn DataField="VendorID" TextAlign="Right" Width="50px" />
											<px:PXGridColumn DataField="AdjNbr" TextAlign="Right" Width="54px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
								<AutoSize Enabled="true" />
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>
		</Items>
	</px:PXTab>
	<px:PXSmartPanel ID="pnlMatchSettings" runat="server" Style="z-index: 108;" Key="matchSettings"
		Caption="Transaction Match Settings" CaptionVisible="True" LoadOnDemand="true"
		ShowAfterLoad="true" AutoCallBack-Command="Refresh"
		AutoCallBack-Target="frmMatchSettings" DesignView="Content">
		<px:PXFormView ID="frmMatchSettings" runat="server" DataSourceID="ds"
			Style="z-index: 100" DataMember="matchSettings"
			Caption="Transaction Match Settings" CaptionVisible="False"
			SkinID="Transparent">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule17" runat="server" LabelsWidth="L" ControlSize="XS" StartGroup="True" GroupCaption="Disbursement Date Matching" />
				<px:PXNumberEdit ID="edDisbursementTranDaysBefore" DataField="DisbursementTranDaysBefore" runat="server" />
				<px:PXNumberEdit ID="edDisbursementTranDaysAfter" DataField="DisbursementTranDaysAfter" runat="server" />
				<px:PXLayoutRule ID="PXLayoutRule18" runat="server" LabelsWidth="L" ControlSize="XS" StartGroup="True" GroupCaption="Receipt Date Matching" />
				<px:PXNumberEdit ID="edReceiptTranDaysBefore" DataField="ReceiptTranDaysBefore" runat="server" />
				<px:PXNumberEdit ID="edReceiptTranDaysAfter" DataField="ReceiptTranDaysAfter" runat="server" />

				<px:PXLayoutRule ID="PXLayoutRule19" runat="server" StartGroup="True" GroupCaption="Weights for Relevance Calculation" />
				<px:PXPanel runat="server" Border="none" RenderStyle="Simple"
					ID="pnlRelativeWeights" RenderSimple="True">
					<px:PXLayoutRule ID="PXLayoutRule20" runat="server" LabelsWidth="L" ControlSize="XS" StartColumn="True" />
					<px:PXNumberEdit ID="edRefNbrCompareWeight" DataField="RefNbrCompareWeight" CommitChanges="True" runat="server" />
					<px:PXNumberEdit ID="edDateCompareWeight" DataField="DateCompareWeight" CommitChanges="True" runat="server" />
					<px:PXNumberEdit ID="edPayeeCompareWeight" DataField="PayeeCompareWeight" CommitChanges="True" runat="server" />
					<px:PXLayoutRule ID="PXLayoutRule21" runat="server" LabelsWidth="XXS" ControlSize="XS" StartColumn="True" />
					<px:PXNumberEdit ID="edRefNbrComparePercent" DataField="RefNbrComparePercent" runat="server" Enabled="False" Size="XS" />
					<px:PXNumberEdit ID="edDateComparePercent" DataField="DateComparePercent" runat="server" Enabled="False" Size="XS" />
					<px:PXNumberEdit ID="edPayeeComparePercent" DataField="PayeeComparePercent" runat="server" Enabled="False" Size="XS" />
				</px:PXPanel>
				<px:PXLayoutRule ID="PXLayoutRule23" runat="server" LabelsWidth="L" ControlSize="XS" StartGroup="True" GroupCaption="Date Range for Relevance Calculation" />
				<px:PXNumberEdit ID="edDateMeanOffset" DataField="DateMeanOffset" runat="server" />
				<px:PXNumberEdit ID="edDateSigma" DataField="DateSigma" runat="server" />
				<px:PXLayoutRule ID="PXLayoutRule24" runat="server" LabelsWidth="XXS" ControlSize="M" StartGroup="True" />
				<px:PXCheckBox ID="chkMatchInSelection" runat="server" DataField="MatchInSelection" />
                <px:PXCheckBox ID="chkskipVoided" runat="server" DataField="SkipVoided" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
			<px:PXButton ID="pbClose" runat="server" DialogResult="OK" Text="Close" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>
