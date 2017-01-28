 <%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AR203000.aspx.cs" Inherits="Pages_AR203000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds"  runat="server" Visible="True" Width="100%" TypeName="PX.Objects.RUTROT.RUTROTWorkTypesMaint" PrimaryView="WorkTypes">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="MoveUp" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="MoveDown" CommitChanges="True" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" AllowPaging="True"   AllowSearch="True" SkinID="Primary" TabIndex="100" 
        SyncPosition="true" SyncPositionWithGraph="true">
		<Levels>
			<px:PXGridLevel DataMember="WorkTypes">
				<Columns>
					<px:PXGridColumn DataField="RUTROTType" Width="90px" Type="DropDownList" CommitChanges="true"/>
					<px:PXGridColumn DataField="Description" Width="250px" />
					<px:PXGridColumn DataField="XMLTag" Width="150px" />
					<px:PXGridColumn DataField="StartDate" Width="150px"  CommitChanges="true" />
					<px:PXGridColumn DataField="EndDate" Width="150px" CommitChanges="true" />
					<px:PXGridColumn DataField="Position" Width="150px" CommitChanges="true" />
				</Columns>
				<Mode InitNewRow="True" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXGrid>
</asp:Content>
