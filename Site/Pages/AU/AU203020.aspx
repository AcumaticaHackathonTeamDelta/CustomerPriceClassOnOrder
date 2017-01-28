<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU203020.aspx.cs" Inherits="Page_AU203020"
	Title="Automation Step Maintenance" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<pxa:AUDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.SM.AUTableScreenExtensionMaint"
		PrimaryView="Screens" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cacnel" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />            
		</CallbackCommands>
	</pxa:AUDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
           
	<px:PXSplitContainer runat="server" ID="splitScreens" SplitterPosition="350" Style="border-top: solid 1px #BBBBBB;"  >
			<Template1>
			   <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" SkinID="Primary"
				Width="100%" AdjustPageSize="Auto" BorderWidth="0px"  AllowSearch="True" SyncPosition="true" AllowPaging="false" AutoAdjustColumns="true" Caption="Screen" CaptionVisible="true">
					<Levels>
						<px:PXGridLevel DataMember="Screens" >							        
							<Columns>									        
								<px:PXGridColumn DataField="ScreenID" Width="150px" AutoCallBack="true" />     
                                <px:PXGridColumn DataField="SiteMap__Title" Width="150px" AutoCallBack="true" />     
							</Columns>
						</px:PXGridLevel>
					</Levels>
                    <AutoCallBack Enabled="true" Command="Refresh" Target="gridProps"/>	
					<AutoSize Enabled="True" />
                    <ActionBar ActionsVisible="false">                        
                    </ActionBar>  		            
				</px:PXGrid>
			</Template1>
            <Template2>
                <px:PXSplitContainer runat="server" ID="splitFields" SkinID="Horizontal" >
                    	<Template1>
			                <px:PXGrid ID="gridFields" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
				            Width="100%" AdjustPageSize="Auto" BorderWidth="0px" AllowSearch="True" SkinID="Primary" SyncPosition="true" AllowPaging="false" Caption="Fields" CaptionVisible="true">
			                    <AutoCallBack Enabled="true" Command="Refresh" Target="gridProps"/>						        
					            <Levels>
						            <px:PXGridLevel DataMember="Fields" >							        
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
                    <AutoSize Enabled="true"/>	
                </px:PXSplitContainer> 
            </Template2>
			<AutoSize Enabled="true" Container="Window" />	                    					                          
		</px:PXSplitContainer>	     		
</asp:Content>
