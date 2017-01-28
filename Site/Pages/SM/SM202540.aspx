<%@ Page Title="External File Storage" Language="C#" MasterPageFile="~/MasterPages/FormDetail.master"
	AutoEventWireup="true" CodeFile="SM202540.aspx.cs" Inherits="Pages_SM_SM202540" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" TypeName="PX.SM.BlobStorageMaint" Visible="True"
		PrimaryView="Filter" PageLoadBehavior="GoFirstRecord" Width="100%">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView runat="server" ID="FormFilter" DataSourceID="ds" DataMember="Filter"
		Width="100%" Caption="Options">
		<Template>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True" />
		    <px:PXDropDown ID="PXDropDown1" CommitChanges="True" runat="server" DataField="Provider" />
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True" />
		    <px:PXCheckBox ID="PXCheckBox1" runat="server" DataField="AllowWrite" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid runat="server" ID="GridSettings" DataSourceID="ds" Width="100%" Caption="Provider Settings">
		<AutoSize Enabled="true" Container="Window" />
		<Levels>
			<px:PXGridLevel DataMember="Settings">
				<Columns>
					<px:PXGridColumn DataField="Name" Width="200" />
					<px:PXGridColumn DataField="Value" Width="400" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
</asp:Content>
