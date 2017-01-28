<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU203010.aspx.cs" Inherits="Page_AU203010"
	Title="Automation Step Maintenance" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<pxa:AUDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.SM.AUTableExtensionMaint"
		PrimaryView="Fields" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cancel" />
		    <px:PXDSCallbackCommand Name="WZAddField_Next" CommitChanges="True" Visible="false"/>                
            <px:PXDSCallbackCommand Name="WZAddField_Prev" CommitChanges="True" Visible="false"/>                            
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />            
            <px:PXDSCallbackCommand CommitChanges="True" Name="addDefault" />            
            <px:PXDSCallbackCommand CommitChanges="True" Name="combos" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="addSelector" />            
            <px:PXDSCallbackCommand CommitChanges="True" Name="addCustomCode" />            
		</CallbackCommands>
	</pxa:AUDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXSplitContainer runat="server" ID="splitFields" SplitterPosition="250" Style="border-top: solid 1px #BBBBBB;" >
			<Template1>
			    <px:PXGrid ID="gridFields" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
				Width="100%" AdjustPageSize="Auto" BorderWidth="0px" AllowSearch="True" SkinID="Primary" SyncPosition="true" AllowPaging="false" Caption="Fields" CaptionVisible="true">
			        <AutoCallBack Enabled="true" Command="Refresh" Target="gridProps"/>						        
					<Levels>
						<px:PXGridLevel DataMember="Fields"   >
						    <Mode AutoInsert="false"/>							        
							<Columns>									        
								<px:PXGridColumn DataField="FieldName" Width="150px" AutoCallBack="true" />     
                                <px:PXGridColumn AllowNull="False" DataField="IsOverride" TextAlign="Center" Type="CheckBox" Width="80px" />                                       
							</Columns>
						</px:PXGridLevel>
					</Levels>
					<AutoSize Enabled="True" MinHeight="150" />
                    <ActionBar ActionsVisible="False">                        
                    </ActionBar>  
				</px:PXGrid>               
			</Template1>
            <Template2>
                 <px:PXGrid ID="gridProps" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
				Width="100%" AdjustPageSize="Auto" BorderWidth="0px" AllowSearch="True" SkinID="Details"
				MatrixMode="true" AllowPaging="false" Caption="Field Properties" CaptionVisible="true">					        
					<Levels>
						<px:PXGridLevel DataMember="FieldProps" >
							<Mode InitNewRow="True" />
							<Columns>
									<px:PXGridColumn AllowNull="False" DataField="IsOverride" TextAlign="Center" Type="CheckBox"
										Width="80px" />
									<px:PXGridColumn DataField="PropertyName" Width="200px" />									        
									<px:PXGridColumn DataField="OriginalValue" Width="200px" />
									<px:PXGridColumn DataField="OverrideValue" Width="200px" />
							</Columns>
						</px:PXGridLevel>
					</Levels>
					<AutoSize Enabled="True" MinHeight="150" />
                    <ActionBar ActionsVisible="false"/>                              
				</px:PXGrid>
            </Template2>
			<AutoSize Enabled="true" Container="Window" />	                    					                          
		</px:PXSplitContainer>	     		
    <px:PXSmartPanel ID="spInsertField" runat="server" Width="550px" Height="350px" Caption="Field Creation Wizard" 
		CaptionVisible="True" Key="addField" LoadOnDemand="True" ShowAfterLoad="true"
		AutoCallBack-Enabled="true" AutoCallBack-Target="PXWizard1" AutoCallBack-Command="Refresh"
		CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"	>
		<px:PXWizard ID="PXWizard1" runat="server" Width="100%" Height="240px" DataMember="addField" SkinID="Flat" CaptionVisible="True" Caption="Field Creation Wizard">
		    <NextCommand Command="WZAddField_Next" Target="ds"/>                        
            <PrevCommand Command="WZAddField_Prev" Target="ds"/>                        
			<AutoSize Enabled="true" />
            <Pages>
                <px:PXWizardPage>
                    <Template>
                    <px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" ColumnWidth="60px" />
                    <px:PXLabel runat="server" ID="lblStep1" Width="370px" Style="font-weight: bold">Step 1 of 3</px:PXLabel>
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" />
                    <px:PXLabel runat="server" ID="lblStep1Desc" Width="370px" Style="font-weight: bold">Please select what do you want to do</px:PXLabel>
                    <px:PXGroupBox ID="gbOperation" runat="server" Caption="" DataField="Operation" RenderSimple="True" RenderStyle="Simple" Height="101" >
                        <Template>
					        <px:PXRadioButton ID="rbAdd" runat="server" GroupName="gbOperation"
						        Text="Add Custom Field" Value="0" />
                            <px:PXRadioButton ID="rbChangeCustom" runat="server" GroupName="gbOperation"
						        Text="Change Custom Field" Value="1" />
                            <px:PXRadioButton ID="rbChange" runat="server" GroupName="gbOperation"
						        Text="Modify Base Field" Value="2" />                                        
                        </Template>
				        <ContentLayout LabelsWidth="S" Layout="Stack" />
			        </px:PXGroupBox>
                    </Template>					                
                </px:PXWizardPage>
                <px:PXWizardPage>
                    <Template>
                        <px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" ColumnWidth="60px" />
                        <px:PXLabel runat="server" ID="lblStep1" Width="370px" Style="font-weight: bold">Step 2 of 3</px:PXLabel>
                        <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" />
                        <px:PXLabel runat="server" ID="lblStep2" Width="370px" Style="font-weight: bold">Please specify attributes for field</px:PXLabel>                        
						<px:PXTextEdit runat="server" ID="edNewFieldName" DataField="NewFieldName" CommitChanges="true"/>
                        <px:PXDropDown runat="server" ID="edCustFieldName" DataField="CustomFieldName" CommitChanges="true"/>
                        <px:PXDropDown runat="server" ID="edBaseFiendName" DataField="BaseFieldName" CommitChanges="true" />
                        <px:PXDropDown runat="server" ID="edStorageType" DataField="StorageType"/>  
                        <px:PXDropDown runat="server" ID="edControlType" DataField="ControlType"/>  
                        <px:PXDropDown runat="server" ID="edDataType" DataField="DataType" CommitChanges="true" />  
                        <px:PXTextEdit runat="server" ID="edLength" DataField="Length"/>  
                    </Template>
                </px:PXWizardPage>  
                 <px:PXWizardPage>
                    <Template>
                        <px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" ColumnWidth="60px" />
                        <px:PXLabel runat="server" ID="lblStep1" Width="370px" Style="font-weight: bold">Step 3 of 3</px:PXLabel>
                        <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" />
                        <px:PXLabel runat="server" ID="lblStep2" Width="370px" Height="40px" Style="font-weight: bold">Please review your action and press finish button to apply the changes to selected customization project:</px:PXLabel> 
                        <px:PXTextEdit runat="server" ID="edSummary" DataField="Summary" SkinID="Label" AutoSize="true" TextMode="MultiLine" Height="120px" SuppressLabel="true" />
                    </Template>             
                  </px:PXWizardPage>
            </Pages>
        </px:PXWizard>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlCombos" runat="server" Style="z-index: 108;
					left: 351px; position: absolute; top: 99px" Width="530px" Caption="Combo Box Values"
						CaptionVisible="true" LoadOnDemand="true" Key="ComboValues" AutoCallBack-Enabled="true"
						AutoCallBack-Target="gridCombos" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
						CallBackMode-PostData="Page">
				<px:PXGrid ID="gridCombos" runat="server" DataSourceID="ds" Height="243px" Style="z-index: 100" Width="100%" AutoAdjustColumns="true">
					<Mode InitNewRow="True"></Mode>						    					    
					<Levels>						    
						<px:PXGridLevel  DataMember="ComboValues" DataKeyNames="Value">
							<Columns>
									<px:PXGridColumn AllowNull="False" DataField="IsActive" TextAlign="Center" Type="CheckBox" Width="60px" />
									<px:PXGridColumn AllowNull="False" DataField="Sequence" TextAlign="Left" Width="80px"  />
									<px:PXGridColumn DataField="Value" />
									<px:PXGridColumn DataField="Description" Width="200px" />
							</Columns>
						</px:PXGridLevel>
					</Levels>
					<CallbackCommands>
						<FetchRow RepaintControls="None" />
					</CallbackCommands>
                     <AutoSize Enabled="true"/>
				</px:PXGrid>
				<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
                    <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Save" />
                    <px:PXButton ID="PXButton2" runat="server" DialogResult="No" Text="Cancel" />
                </px:PXPanel>
	</px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlDefault" runat="server" Style="z-index: 108;
					left: 351px; position: absolute; top: 99px" Width="530px" Height="300px" Caption="Default Value Editor"
						CaptionVisible="true" LoadOnDemand="true" Key="wsDefault" AutoCallBack-Enabled="true"
						AutoCallBack-Target="tabDefault" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
						CallBackMode-PostData="Page"  AllowResize="false">
				<px:PXTab runat="server" ID="tabDefault" Width="100%" DataMember="wsDefault" MarkRequired="true">
				    <Items>
				        <px:PXTabItem Text="Properties">
				            <Template>
				                <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True"/>
				                <px:PXDropDown runat="server" ID="edSource" DataField="Source" CommitChanges="true"/>
                                <px:PXTextEdit runat="server" ID="edManual" DataField="Manual" />
                                <px:PXDropDown runat="server" ID="edSystemConstant" DataField="SystemConstant" />
                                <px:PXTextEdit runat="server" ID="edTableName" DataField="TableName" CommitChanges="true"/>
				                <px:PXDropDown runat="server" ID="edFieldName" DataField="FieldName" CommitChanges="true" SyncStateWithCommand="True" />
                                <px:PXLayoutRule runat="server" ControlSize="L" LabelsWidth="SM" />
                                <px:PXTextEdit runat="server" ID="edSearch" DataField="Search" CommitChanges="true" TextMode="MultiLine" Height="100px" />
				            </Template>
				        </px:PXTabItem>
                    </Items>
				    <AutoSize Enabled="true"/>				        
				</px:PXTab>
				<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
                    <px:PXButton ID="PXButton3" runat="server" DialogResult="OK" Text="Save" />
                    <px:PXButton ID="PXButton4" runat="server" DialogResult="No" Text="Cancel" />
                </px:PXPanel>
	</px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlSelector" runat="server" Style="z-index: 108;
					left: 351px; position: absolute; top: 99px" Width="530px" Height="300px"  Caption="Lookup Editor"
						CaptionVisible="true" LoadOnDemand="true" Key="wsSelector" AutoCallBack-Enabled="true"
						AutoCallBack-Target="tabSelector" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
						CallBackMode-PostData="Page" AllowResize="false">
				    <px:PXTab runat="server" ID="tabSelector" Width="100%" Height="200px" DataMember="wsSelector">
				        <Items>
				            <px:PXTabItem Text="Properties">
				                <Template>
				                    <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True"/>				                    
                                    <px:PXTextEdit runat="server" ID="edTableName" DataField="TableName" CommitChanges="true"/>
				                    <px:PXDropDown runat="server" ID="edFieldName" DataField="FieldName" CommitChanges="true" />
                                    <px:PXDropDown runat="server" ID="edDisplayFieldName" DataField="DisplayFieldName"/>
                                    <px:PXDropDown runat="server" ID="edDescriptionFieldName" DataField="DescriptionFieldName" />
				                    <px:PXCheckBox runat="server" ID="chkAllowFilters" DataField="AllowFilters"/>                                    
				                </Template>
				            </px:PXTabItem>
                            <px:PXTabItem Text="Query Editor">
                                <Template>
                                    <px:PXLayoutRule runat="server" ControlSize="L" LabelsWidth="SM" />                                    
                                    <px:PXTextEdit runat="server" ID="edSearch" DataField="Search" CommitChanges="true" TextMode="MultiLine" Height="200px" />
                                </Template>
                            </px:PXTabItem>
                            <px:PXTabItem Text="Columns">
                                <Template>
                                    <px:PXGrid runat="server" ID="gridColumns" SkinID="DetailsInTab" Height="200px" Width="100%">
                                       		<Levels>
                                                <px:PXGridLevel DataMember="wsSelectorColumn">
                                                    <Columns>
                                                        <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="80px" />
                                                        <px:PXGridColumn DataField="FieldName" Width="100px" />
                                                        <px:PXGridColumn DataField="DisplayName" Width="100px" />
                                                    </Columns>
                                                </px:PXGridLevel>
                                             </Levels>
                                    </px:PXGrid>
                                </Template>
                             </px:PXTabItem>
                        </Items>			
                         <AutoSize Enabled="true"/>			        
				    </px:PXTab>
				<px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
                    <px:PXButton ID="PXButton5" runat="server" DialogResult="OK" Text="Save" />
                    <px:PXButton ID="PXButton6" runat="server" DialogResult="No" Text="Cancel" />
                </px:PXPanel>
	</px:PXSmartPanel>
     <px:PXSmartPanel ID="pnlCustomCode" runat="server" Style="z-index: 108;
					left: 351px; position: absolute; top: 99px" Width="550px" Height="550px" Caption="Custom Code Editor"
						CaptionVisible="true" LoadOnDemand="true" Key="wsCustomCode" AutoCallBack-Enabled="true"
						AutoCallBack-Target="frmCode" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
						CallBackMode-PostData="Page" AllowResize="false">
				    <px:PXFormView runat="server" ID="frmCode" Width="100%" DataMember="wsCustomCode" MarkRequired="true" SkinID="Transparent">
				            <Template>
				                <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="XL" StartColumn="True"/>
				                <px:PXLabel runat="server" ID="lblBaseCode" Text="Base Code" />
                                <px:PXTextEdit runat="server" ID="edBaseCode" DataField="BaseCode" TextMode="MultiLine" Height="200px" SuppressLabel="true" Width="500px" />
                                <px:PXLabel runat="server" ID="lblOverrideCode" Text="Override Code" />
                                <px:PXTextEdit runat="server" ID="edOverrideCode" DataField="OverrideCode" TextMode="MultiLine" Height="200px" SuppressLabel="true" Width="500px" />
				            </Template>
                          <AutoSize Enabled="true"/>		
				    </px:PXFormView>
				<px:PXPanel ID="PXPanel4" runat="server" SkinID="Buttons">
                    <px:PXButton ID="PXButton7" runat="server" DialogResult="OK" Text="Save" />
                    <px:PXButton ID="PXButton8" runat="server" DialogResult="No" Text="Cancel" />
                </px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>

