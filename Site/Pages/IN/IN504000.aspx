<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="IN504000.aspx.cs" Inherits="Page_IN504000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Width="100%" TypeName="PX.Objects.IN.PIGenerator"
                     PageLoadBehavior="PopulateSavedValues" Visible="True" PrimaryView="GeneratorSettings"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="203px" Style="z-index: 100"
		Width="100%" DataMember="GeneratorSettings" DefaultControlID="edPIClassID">
<Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
		<Items>
			<px:PXTabItem Text="Warehouse/Inventory Selection">
				<Template>					
					<px:PXLayoutRule runat="server" StartColumn="True"  LabelsWidth="S" ControlSize="XM" />
                            <px:PXSelector ID="edPIClassID" runat="server" DataField="PIClassID">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXSelector>
							<px:PXTextEdit ID="edPIDescr" runat="server" DataField="Descr" />
							<px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID">
								<GridProperties FastFilterFields="Descr">
									
									<Layout ColumnsMenu="False" />
								 </GridProperties>
								<Items>
									<px:PXMaskItem EditMask="AlphaNumeric" Length="10" Separator="-" TextCase="Upper" />
								</Items>
								<AutoCallBack Command="Save" Target="tab" />
							</px:PXSegmentMask>
                            <px:PXDropDown ID="edMethod" runat="server" AllowNull="False" DataField="Method" Enabled="False" />

                            <px:PXDropDown ID="edSelectedMethod" runat="server" AllowNull="False" 
                                DataField="SelectedMethod" Enabled="False" />
                            <px:PXSelector ID="edCycleID" runat="server" DataField="CycleID">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXSelector>
 					<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True"  LabelsWidth="M" ControlSize="XM" />
                           <px:PXSelector ID="edABCCodeID" runat="server" DataField="ABCCodeID">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXSelector>
                            <px:PXSelector ID="edMovementClassID" runat="server" 
                                DataField="MovementClassID">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXSelector>
                            <px:PXSelector ID="edItemClassID" runat="server" DataField="ItemClassID">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXSelector>
                            <px:PXNumberEdit ID="edRandomItemsLimit" runat="server" DataField="RandomItemsLimit" ValueType="Int16">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXNumberEdit>
						    <px:PXCheckBox ID="chkByFrequency" runat="server" DataField="ByFrequency">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXCheckBox>
                              <px:PXNumberEdit ID="edBlankLines" runat="server" DataField="BlankLines" ValueType="Int16" Width="51px">
                                <AutoCallBack Command="Save" Target="tab" />
                               </px:PXNumberEdit>
                            <px:PXDateTimeEdit ID="edMaxLastCountDate" runat="server" 
                                DataField="MaxLastCountDate" MaxValue="9999-06-06">
                                <AutoCallBack Command="Save" Target="tab" />
                            </px:PXDateTimeEdit>
						</Template>					
			</px:PXTabItem>
			<px:PXTabItem Text="Location Selection">
				<Template>
					<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="200px" Width="100%"
						BorderWidth="0px" SkinID="Details" Style="height: 200px;" AllowPaging="True">
						<Levels>
							<px:PXGridLevel  DataMember="locations" DataKeyNames="PIClassID,LocationID">
								<RowTemplate>
                                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID">
                                        <Items>
                                            <px:PXMaskItem EditMask="AlphaNumeric" Length="10" Separator="-" 
                                                TextCase="Upper" />
                                        </Items>
                                        
                                    </px:PXSegmentMask>
                                </RowTemplate>
								<Columns>									
									<px:PXGridColumn DataField="LocationID" DisplayFormat="&gt;AAAAAAAAAA" />									
                                    <px:PXGridColumn DataField="INLocation__Descr"  Width="200px" />
                                    <px:PXGridColumn AllowNull="False" DataField="INLocation__PickPriority" 
                                        DataType="Int16" DefValueText="1" TextAlign="Right" />									
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
	</px:PXTab>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="100px" Style="z-index: 100; width: 100%" 
		AdjustPageSize="Auto" AllowPaging="True" AllowSearch="True"
		BatchUpdate="True" SkinID="PrimaryInquire" TabIndex="200" Caption="Details" 
		DataSourceID="ds">
		<Levels>
			<px:PXGridLevel  DataMember="PreliminaryResultRecs">
				<Columns>
					<px:PXGridColumn DataField="LineNbr" DataType="Int32" TextAlign="Right" />
					<px:PXGridColumn DataField="TagNumber" DataType="Int32" TextAlign="Right" />
					<px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" />
					<px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A" />
					<px:PXGridColumn DataField="LocationID" AllowShowHide="Server" DisplayFormat="&gt;AAAAAAAAAA" />
					<px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server"  Width="120px" />
					<px:PXGridColumn AutoGenerateOption="NotSet" DataField="ExpireDate" 
                        DataType="DateTime" Label="Expiry Date" Width="90px" />
					<px:PXGridColumn DataField="BookQty" DataType="Decimal" Decimals="2" TextAlign="Right" Width="100px" />
					<px:PXGridColumn DataField="BaseUnit" />
					<px:PXGridColumn DataField="Descr" Width="207px" />				    
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
		<ActionBar>
			<Actions>
				<Refresh Enabled="False" />
			</Actions>
		</ActionBar>
	</px:PXGrid>
</asp:Content>
