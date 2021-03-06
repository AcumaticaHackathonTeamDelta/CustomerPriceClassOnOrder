﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="DR201510.aspx.cs" Inherits="Pages_DR_DR201510" Title="Deferred Schedules" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.DR.DRSchedulePrimary" PrimaryView="Items"/>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="PrimaryInquire" AdjustPageSize="Auto"
        AllowPaging="True" Caption="Time Cards" FastFilterFields="BAccountID,RefNbr" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataMember="Items">
                <Columns>
                    <px:PXGridColumn DataField="ScheduleID" Width="108px" />
					<px:PXGridColumn DataField="Status" Width="90px" />
                    <px:PXGridColumn DataField="BAccountType" Width="108px" />
                    <px:PXGridColumn DataField="BAccountID" Width="163px" />
                    <px:PXGridColumn DataField="DocumentTypeEx" Width="90px" />
                    <px:PXGridColumn DataField="RefNbr" Width="90px" LinkCommand="ViewDoc" />
                 </Columns>
            </px:PXGridLevel>
        </Levels>
         <ActionBar DefaultAction="update"/>
         <AutoSize Container="Window" Enabled="True" MinHeight="150" />
         <Mode AllowAddNew="False" AllowUpdate="False" AllowDelete="False"/>
	 </px:PXGrid>
</asp:Content>
