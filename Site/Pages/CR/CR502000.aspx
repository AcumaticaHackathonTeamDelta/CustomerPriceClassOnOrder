<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR502000.aspx.cs" Inherits="Page_CR502000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Width="100%"  AutoCallBack="True" Visible="True" TypeName="PX.Objects.CR.CampaignMemberMassProcess"
		PrimaryView="Operations" PageLoadBehavior="PopulateSavedValues">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Process" CommitChanges="true" StartNewGroup="True"/> 
			<px:PXDSCallbackCommand DependOnGrid="grdItems" CommitChanges="True" Name="Items_ViewDetails"
				Visible="True" />
			<px:PXDSCallbackCommand DependOnGrid="grdItems" CommitChanges="True" Name="Items_BAccount_ViewDetails"
				Visible="True" />
			<px:PXDSCallbackCommand DependOnGrid="grdItems" CommitChanges="True" Name="Items_BAccountParent_ViewDetails"
				Visible="True" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">	
	<px:PXFormView ID="form1" runat="server" DataSourceID="ds" Width="100%" DataMember="Operations" Caption="Filter">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
			<px:PXSelector CommitChanges="True" ID="edCampaignID" runat="server" DataField="CampaignID" AllowEdit="true" />			
			<px:PXDropDown ID="edAction" runat="server" DataField="Action" CommitChanges="True" /> 
			</Template>
		<CallbackCommands>
			<Save PostData="Page" />
		</CallbackCommands>
	</px:PXFormView>	
	<px:PXFormView ID="fvCriteria" runat="server" Width="100%" Caption="Filter" DisableDataBinding="True"> 
	<Template> 		
			<px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True" SuppressLabel="True" />
		<px:PXFilterEditor ID="mainFilter" runat="server" SkinID="External" FilterView="Items$FilterHeader" 
		FilterRowsView="Items$FilterRow" DataSourceID="ds" LinkedGridID="grdItems">
		</px:PXFilterEditor>
	</Template>
		<CallbackCommands>
			<Save PostData="Page" />
		</CallbackCommands>	
	</px:PXFormView>		
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grdItems" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
		Width="100%" ActionsPosition="Top" Caption="Campaign Members" AllowPaging="True"
		AdjustPageSize="auto" ExternalFilter="true"
		SkinID="Inquire"  AutoGenerateColumns="AppendDynamic">
		<Levels>
			<px:PXGridLevel DataMember="Items">
				<Columns>
					<px:PXGridColumn AllowCheckAll="True" AllowShowHide="False" DataField="Selected"
						TextAlign="Center" Type="CheckBox" Width="40px" AutoCallBack="True" />
                    <px:PXGridColumn DataField="CRCampaignMembers__Status" Width="120px" />                        
					<px:PXGridColumn DataField="ContactType" Width="60px" />
					<px:PXGridColumn DataField="DisplayName" Width="130px" LinkCommand="Items_ViewDetails" />
					<px:PXGridColumn DataField="Title" Width="50px" />
					<px:PXGridColumn DataField="FirstName" Width="100px" />
					<px:PXGridColumn DataField="MidName" Width="100px" />
					<px:PXGridColumn DataField="LastName" Width="100px" />
					<px:PXGridColumn DataField="Salutation" Width="180px" />
					<px:PXGridColumn DataField="BAccount__AcctCD" Width="150px" LinkCommand="Items_BAccount_ViewDetails" />
					<px:PXGridColumn DataField="FullName" Width="200px" />
					<px:PXGridColumn DataField="BAccountParent__AcctCD" Width="150px" LinkCommand="Items_BAccountParent_ViewDetails" />
					<px:PXGridColumn DataField="BAccountParent__AcctName" Width="200px" />
					<px:PXGridColumn DataField="IsActive" Width="60px" Type="CheckBox" />
					<px:PXGridColumn DataField="EMail" Width="190px" />
                    <px:PXGridColumn DataField="ClassID" Width="90px" Visible="False" />
                    <px:PXGridColumn DataField="Status" Width="90px" Visible="False" />
                    <px:PXGridColumn DataField="Source" Width="90px" Visible="False" />
					<px:PXGridColumn DataField="Address__PostalCode" Width="90px" Visible="False" />
					<px:PXGridColumn DataField="Address__CountryID" Width="90px" Visible="False" />
					<px:PXGridColumn DataField="Address__State" Width="90px" Visible="False" />
					<px:PXGridColumn DataField="Address__City" Width="90px" />
					<px:PXGridColumn DataField="Address__AddressLine1" Width="300px" />
					<px:PXGridColumn DataField="Address__AddressLine2" Width="300px" />
					<px:PXGridColumn DataField="Phone1" DisplayFormat="+#(###) ###-####" Width="130px" />
					<px:PXGridColumn DataField="Phone2" DisplayFormat="+#(###) ###-####" Width="130px" />
					<px:PXGridColumn DataField="Phone3" DisplayFormat="+#(###) ###-####" Width="130px" />
					<px:PXGridColumn DataField="Fax" DisplayFormat="+#(###) ###-####" Width="130px" />
					<px:PXGridColumn DataField="WorkgroupID" Width="90px" />
					<px:PXGridColumn DataField="OwnerID" Width="90px" DisplayMode="Text"/>
					<px:PXGridColumn DataField="CreatedByID_Creator_Username" Width="100px"
						SyncVisible="False" SyncVisibility="False" Visible="False" />
					<px:PXGridColumn DataField="CreatedDateTime" Width="90px" />
					<px:PXGridColumn DataField="LastModifiedByID_Modifier_Username" Width="100px"
						SyncVisible="False" SyncVisibility="False" Visible="False" />
					<px:PXGridColumn DataField="LastModifiedDateTime" Width="90px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar PagerVisible="False">
			<CustomItems>
				<px:PXToolBarButton Key="cmdShowDetails" Visible="false">
					<AutoCallBack Target="ds" Command="Items_ViewDetails" />
					<Images Normal="main@DataEntry" />
					<ActionBar GroupIndex="0" />
				</px:PXToolBarButton>
			</CustomItems>
			<Actions>
				<FilterShow Enabled="False" />
				<FilterSet Enabled="False" />
			</Actions>
		</ActionBar>
		<Mode AllowAddNew="False" AllowDelete="False" />
		<CallbackCommands>
			<Save PostData="Page" />
		</CallbackCommands>
	</px:PXGrid>
    <px:PXSmartPanel ID="pnlUpdateMember" runat="server" Height="90px" Width="407px" Style="z-index: 108;
		left: 351px; position: absolute; top: 99px" CaptionVisible="true"
		Caption="Update Members" DesignView="Content" LoadOnDemand="true" Key="UpdateMembers" ShowAfterLoad="true"
        AutoCallBack-Command="Refresh" AutoCallBack-Enabled="true" AutoCallBack-Target="frmUpdateMember">
		<px:PXFormView ID="frmUpdateMember" runat="server" DataSourceID="ds" Style="z-index: 100"
			Width="100%" DataMember="Operations" SkinID="Transparent">
			<Template>
				 <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />                
				<px:PXDropDown CommitChanges="True" ID="edAddMemberStatus" runat="server" AllowNull="False"
								DataField="Status" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons" Style="margin-right: 25px;">
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Apply" Width="63px"
				Height="20px" />
			<px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" Width="63px"
				Height="20px" Style="margin-left: 5px" />
        </px:PXPanel>		
	</px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlAddMember" runat="server" Height="90px" Width="407px" Style="z-index: 108;
		left: 351px; position: absolute; top: 99px"  CaptionVisible="true"
		Caption="Add New Members" DesignView="Content" LoadOnDemand="true" Key="AddMembers" ShowAfterLoad="true"
        AutoCallBack-Command="Refresh" AutoCallBack-Enabled="true" AutoCallBack-Target="frmAddMemberParams">
		<px:PXFormView ID="frmAddMemberParams" runat="server" DataSourceID="ds" Style="z-index: 100"
			Width="100%" DataMember="Operations" SkinID="Transparent">
			<Template>
				    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />                
				<px:PXDropDown CommitChanges="True" ID="edAddMemberStatus" runat="server" AllowNull="False"
								DataField="Status" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons" Style="margin-right: 25px;">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="Apply" Width="63px" Height="20px" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" Width="63px" Height="20px" Style="margin-left: 5px" />
        </px:PXPanel>		
	</px:PXSmartPanel>
</asp:Content>
