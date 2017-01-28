<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM204505.aspx.cs" Inherits="Page_SM204505"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Visible="True" Width="100%" runat="server" PrimaryView="Projects" TypeName="PX.SM.ProjectList">
		<CallbackCommands>
			
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
			<%--<px:PXDSCallbackCommand Name="Delete" Visible="False" />--%>
			<%--<px:PXDSCallbackCommand Name="First" StartNewGroup="True" PostData="Self" />--%>
			<px:PXDSCallbackCommand Name="view" CommitChanges="True" DependOnGrid="grid" Visible="False" RepaintControls="All"/>
			<%--<px:PXDSCallbackCommand Name="import" CommitChanges="true" PopupPanel="UploadPackageDlg" />--%>
			<%--<px:PXDSCallbackCommand PopupPanel="UploadPackagePanel" Name="actionImport" />--%>
			<px:PXDSCallbackCommand Name="actionPublish" CommitChanges="true" RepaintControls="All" PopupPanel="PanelCompiler"/>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">

	<px:PXGrid ID="grid" runat="server" 
		Height="400px" 
		Width="100%" 
		AllowPaging="True" 
		ActionsPosition="Top" 
		AutoAdjustColumns="True"
		AllowSearch="true" 
		SkinID="Primary" 
        DataSourceID="ds"
		SyncPosition="True"
		PageSize="50"
        BatchUpdate ="False"
        
        >

		<Levels>
			<px:PXGridLevel DataMember="Projects">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXTextEdit ID="edName" runat="server" DataField="Name" />
					<px:PXCheckBox ID="chkIsWorking" runat="server" DataField="IsWorking" />
					<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
					<px:PXSelector ID="edCreatedByID" runat="server" DataField="CreatedByID" Enabled="False"
						TextField="Username" />
					<px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" DataField="CreatedDateTime"
						DisplayFormat="g" Enabled="False" />
					<px:PXSelector ID="edLastModifiedByID" runat="server" DataField="LastModifiedByID"
						Enabled="False" TextField="Username" />
					<px:PXDateTimeEdit ID="edLastModifiedDateTime" runat="server" DataField="LastModifiedDateTime"
						DisplayFormat="g" Enabled="False" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="IsWorking" Width="30px" Type="CheckBox" AllowCheckAll="True" TextAlign="Center"  AutoCallBack="True" />
					<px:PXGridColumn DataField="IsPublished" Width="60px" Type="CheckBox" TextAlign="Center" />
					<px:PXGridColumn DataField="Name" Width="108px" LinkCommand="view" />
					<px:PXGridColumn DataField="Level" Width="60px"/>
					<px:PXGridColumn DataField="ScreenNames"/>
					<px:PXGridColumn DataField="Description" Width="108px" />
					<px:PXGridColumn AllowUpdate="False" DataField="CreatedByID_Creator_Username"
						Width="108px" />
<%--					<px:PXGridColumn AllowUpdate="False" DataField="CreatedDateTime" Width="90px" />
					<px:PXGridColumn AllowUpdate="False" DataField="LastModifiedByID_Modifier_Username"
						Width="108px" />--%>
					<px:PXGridColumn AllowUpdate="False" DataField="LastModifiedDateTime" Width="90px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	    <ActionBar ActionsVisible="false">
			<Actions>
				<ExportExcel ToolBarVisible="False"/>
				<AdjustColumns ToolBarVisible="False"/>
			</Actions>    
		</ActionBar>  
	</px:PXGrid>
	
		<px:PXUploadFilePanel ID="UploadPackageDlg" runat="server" 
		CommandSourceID="ds"
		AllowedTypes=".zip" Caption="Open Package" PanelID="UploadPackagePanel" 
		OnUpload="uploadPanel_Upload"
		 CommandName="ReloadPage" />
    
    <script type="text/javascript">
	    function ReloadPage()
	    {
	    	window.parent.location.href = window.parent.location.href;

	    }
    </script>
	<px:PXSmartPanel ID="PanelCompiler" runat="server" 
	                 style="height:300px;width:90%;" 
	                 CaptionVisible="True" 
	                 Caption="Compilation" 

	                 RenderIFrame="True" 
	                 RenderVisible="False" 
	                 IFrameName="Compiler"
 
	                 WindowStyle="Flat" 
	                 AutoReload="True" 
	                 ShowMaximizeButton="True" 
	                 AllowMove="True" 
	                 ClientEvents-AfterHide="ReloadPage"
		
		/>
    
    
    	<px:PXSmartPanel ID="PanelPublishExt" runat="server" Caption="Publish to Multiple Companies"  
		CaptionVisible="True"   Key="ViewCompanyList" AutoRepaint="True">
	        
        <px:PXGrid ID="GridCompanyList" runat="server" SkinID="Attributes" Width="600px" Height="200px" BatchUpdate="True" AutoAdjustColumns="True">
			<Levels>
				<px:PXGridLevel DataMember="ViewCompanyList">
					<Columns>
						<px:PXGridColumn DataField="Selected" Width="100px" Type="CheckBox" />
						<px:PXGridColumn DataField="Name" Width="300px" />
						<px:PXGridColumn DataField="ID" Width="100px" />
						<px:PXGridColumn DataField="ParentID" Width="100px" />

					</Columns>
				</px:PXGridLevel>
			</Levels>
			<Mode AllowAddNew="False" AllowDelete="False" />
		</px:PXGrid>
            

		<px:PXFormView ID="ViewPublishOptions" runat="server" DataMember="ViewPublishOptions" DataSourceID="ds" 
			 AutoRepaint="True" SkinID="Transparent">
			
		    <Template>
		    	
		    	<px:PXLayoutRule runat="server" ColumnWidth="600px" StartColumn="True" SuppressLabel="True"/>
			
				<px:PXCheckBox ID="PublishOnlyDB" runat="server" DataField="PublishOnlyDB"  />
				<px:PXCheckBox ID="DisableOptimization" runat="server" DataField="DisableOptimization"  />
                
			</Template>
		</px:PXFormView>
	
		<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
		    <px:PXButton ID="PXButton5" runat="server" DialogResult="OK" Text="OK" PopupPanel="PanelCompiler"></px:PXButton>
			<px:PXButton ID="PXButton6" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
		
	
    

</asp:Content>
