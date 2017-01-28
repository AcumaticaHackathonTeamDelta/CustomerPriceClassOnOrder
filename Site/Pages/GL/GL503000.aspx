<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL503000.aspx.cs" Inherits="Page_GL503000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="PeriodList"
		TypeName="PX.Objects.GL.Closing">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" AllowSearch="true"
		SkinID="PrimaryInquire" Caption="Financial Periods" SyncPosition ="true">
		<Levels>
			<px:PXGridLevel DataMember="PeriodList">
				<Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True"
						AllowSort="False" AllowMove="False" Width="30px" />
					<px:PXGridColumn DataField="FinPeriodID" Width="100px" />
					<px:PXGridColumn DataField="Descr" Width="100px" />
					<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" Width="80px" />
					<px:PXGridColumn DataField="APClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
					<px:PXGridColumn DataField="ARClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
					<px:PXGridColumn DataField="INClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
					<px:PXGridColumn DataField="CAClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
					<px:PXGridColumn DataField="FAClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
                    <px:PXGridColumn DataField="PRClosed" TextAlign="Center" Type="CheckBox" Width="80px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
        <Mode AllowUpdate="false" />
	    <AutoSize Container="Window" Enabled="True" MinHeight="400" />
	</px:PXGrid>
</asp:Content>
