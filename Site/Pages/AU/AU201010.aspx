<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU201010.aspx.cs" Inherits="Page_AU201010"
	Title="Automation Step Maintenance" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<pxa:AUDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.SM.AUScreenConditionMaint"
		PrimaryView="Conditions" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cacnel" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />            
		</CallbackCommands>
	</pxa:AUDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXSplitContainer runat="server" ID="splitConditions" SplitterPosition="250" Style="border-top: solid 1px #BBBBBB;" >
			<Template1>
			    <px:PXGrid ID="gridConditions" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
				Width="100%" AdjustPageSize="Auto" BorderWidth="0px" AllowSearch="True" SkinID="Primary" SyncPosition="true" AllowPaging="false" Caption="Conditions" CaptionVisible="true">
			        <AutoCallBack Enabled="true" Command="Refresh" Target="gridFilters"/>						        
					<Levels>
						<px:PXGridLevel DataMember="Conditions" >							        
							<Columns>									        
								<px:PXGridColumn DataField="ItemCD" Width="150px" AutoCallBack="true" />     
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
                <px:PXGrid ID="gridFilters" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
				Width="100%" AdjustPageSize="Auto" BorderWidth="0px" AllowSearch="True" SkinID="Details"
				MatrixMode="true" AllowPaging="false" AutoAdjustColumns="true" Caption="Condition Properties" CaptionVisible="true">					        
					<Levels>
						<px:PXGridLevel DataMember="Filters" >
							<Mode InitNewRow="True" />
							<Columns>
									<px:PXGridColumn AllowNull="False" DataField="IsActive" TextAlign="Center" Type="CheckBox"
										Width="60px" />
									<px:PXGridColumn AllowNull="False" DataField="OpenBrackets" Type="DropDownList" Width="100px"
										AutoCallBack="true" />
									<px:PXGridColumn DataField="FieldName" Width="200px" Type="DropDownList" AutoCallBack="true" />
									<px:PXGridColumn AllowNull="False" DataField="Condition" Type="DropDownList" />
									<px:PXGridColumn DataField="Value" Width="200px" />
									<px:PXGridColumn DataField="Value2" Width="200px" />
									<px:PXGridColumn AllowNull="False" DataField="CloseBrackets" Type="DropDownList"
										Width="60px" />
									<px:PXGridColumn AllowNull="False" DataField="Operator" Type="DropDownList" Width="60px" />
							</Columns>
						</px:PXGridLevel>
					</Levels>
					<AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar ActionsVisible="True">
                            <Actions> 
                                <AdjustColumns  ToolBarVisible = "false" />
                                <ExportExcel  ToolBarVisible ="false" />
                                <Refresh ToolBarVisible ="False" />
                            </Actions>
                    </ActionBar>  
				</px:PXGrid>
            </Template2>
			<AutoSize Enabled="true" Container="Window" />	                    					                          
		</px:PXSplitContainer>	     		
</asp:Content>
