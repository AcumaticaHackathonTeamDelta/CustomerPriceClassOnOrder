<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU203000.aspx.cs" Inherits="Page_AU203000"
	 %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<pxa:AUDataSource ID="ds" runat="server" Width="100%" TypeName="PX.SM.AUProjectTableMaint" PrimaryView="Tables" Visible="true">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="viewAction" />
		</CallbackCommands>		
	</pxa:AUDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	 <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" SkinID="Primary" Style="z-index: 100; border-top: solid 1px #BBBBBB;"
	    Width="100%" AdjustPageSize="Auto" BorderWidth="0px"  AllowSearch="True" SyncPosition="true" AllowPaging="false" AutoAdjustColumns="true">
		<Levels>
			<px:PXGridLevel DataMember="Tables" >							        
				<Columns>									        
					<px:PXGridColumn DataField="TableName" Width="150px" AutoCallBack="true" />     
                    <px:PXGridColumn DataField="Description" Width="150px" />  
                    <px:PXGridColumn DataField="IsActive" Width="100px" Type="CheckBox"/>        
                    <px:PXGridColumn DataField="CreatedDateTime" Width="100px" />
                    <px:PXGridColumn DataField="LastModifiedByID" Width="100px" />                                
                    <px:PXGridColumn DataField="LastModifiedDateTime" Width="100px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="True" MinHeight="150" Container="Window" />
        <ActionBar ActionsVisible="false">                        
        </ActionBar>  		            
	</px:PXGrid>         	     		
</asp:Content>
