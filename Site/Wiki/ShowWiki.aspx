<%@ Page Language="C#" MasterPageFile="~/MasterPages/Workspace.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="ShowWiki.aspx.cs" Inherits="Page_ShowWiki"
    Title="Untitled Page" %>

<%@ Register TagPrefix="a" Namespace="System.Windows.Forms" Assembly="System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" %>

<%@ MasterType VirtualPath="~/MasterPages/Workspace.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
		<script type="text/javascript">
			function initializeDataSource(sender, ev)
			{
				px_alls['form'].focus();
			}
		</script>
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.SM.WikiShowReader"
        PrimaryView="Pages" style="float: left">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="getFile" Visible="False" />
            <px:PXDSCallbackCommand Name="viewProps" Visible="False" />
            <px:PXDSCallbackCommand Name="checkOut" Visible="False" />
            <px:PXDSCallbackCommand Name="undoCheckOut" Visible="False" />
        </CallbackCommands>
        <ClientEvents Initialize="initializeDataSource" />
    </px:PXDataSource>

    <px:PXDataSource ID="dsTemplate" runat="server" Visible="False"
        PrimaryView="Pages" TypeName="PX.SM.WikiNotificationTemplateMaintenanceNoRefresh">
        <DataTrees>
            <px:PXTreeDataMember TreeKeys="Key" TreeView="EntityItems" />
        </DataTrees>
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="cancel" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>

    <px:PXToolBar ID="toolbar1" runat="server" SkinID="Navigation" ImageSet="main">
        <Items>
            <px:PXToolBarButton Key="print" Text="Print" Target="main" Tooltip="Print Current Article" ImageKey="Print" DisplayStyle="Text" />
            <px:PXToolBarButton Key="export" Text="Export" ImageKey="Export" DisplayStyle="Text">
                <MenuItems>
                    <px:PXMenuItem Text="Plain Text">
                    </px:PXMenuItem>
                    <px:PXMenuItem Text="Word">
                    </px:PXMenuItem>
                </MenuItems>
            </px:PXToolBarButton>
        </Items>
        <Layout ItemsAlign="Left" />
        <ClientEvents ButtonClick="onButtonClick" />
    </px:PXToolBar>
    <div style="clear: left" />

    <div id="Summary" runat="server">
        <px:PXFormView ID="PXFormView3" runat="server" CaptionVisible="False" Style="margin: 15px; padding-top: 15px; padding-left: 15px; padding-bottom: 15px; position: static; background-color: #22b14c;"
            Width="890px" AllowFocus="False" RenderStyle="Simple" Visible="False">
            <Template>
                <px:PXLabel ID="UserMessage" runat="server" Text="Chto-to" Style="position: static; color: white; font-size: 14pt; height: 60px;" />
            </Template>
        </px:PXFormView>

        <px:PXFormView ID="PXFormView1" runat="server" CaptionVisible="False" Style="position: static;"
            Width="925px" AllowFocus="False" RenderStyle="Simple">
            <Template>
                <div style="padding: 5px; position: static;">
                    <div style="border-style: none;">
                        <table style="position: static; margin-left: 5px; border-color: #ECE9E8; height: 60px;" width="auto">
                            <tr>
                                <td style="height: 60px; width: auto;">
                                    <table style="position: static; margin-left: 5px; border-color: #ECE9E8; height: 60px;" width="auto">
                                        <tr>
                                            <td>
                                                <px:PXLabel runat="server" ID="PXKB" Text="KB:" Style="font-size: 18pt; text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXCategori" Text="Category:" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXProduct" Text="Applies to:" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                                <td style="height: 60px; width: 100%;" />

                                <td style="height: 70px; width: auto; border: solid; border-color: black; border-width: thin; margin-right: 5px; padding-right: 5px;">
                                    <table style="position: static; margin-left: 5px; border-color: #ECE9E8; height: 82px; margin-right: 5px; padding-right: 5px;" width="auto">
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXKBName" Text="Article:" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXCreateDate" Text="Created Date: " Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXLastPublished" Text="Last Modified:" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXViews" Text="Views:" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 12px;">
                                                <px:PXLabel runat="server" ID="PXRating" Text="Rating:" Style="text-wrap: none; white-space: nowrap" />
                                                <px:PXImage runat="server" ID="PXImage1" ImageUrl="main@FavoritesGray" />
                                                <px:PXImage runat="server" ID="PXImage2" ImageUrl="main@FavoritesGray" />
                                                <px:PXImage runat="server" ID="PXImage3" ImageUrl="main@FavoritesGray" />
                                                <px:PXImage runat="server" ID="PXImage4" ImageUrl="main@FavoritesGray" />
                                                <px:PXImage runat="server" ID="PXImage5" ImageUrl="main@FavoritesGray" />
                                                <px:PXLabel runat="server" ID="PXdAvRate" Text="" Style="text-wrap: none; white-space: nowrap" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </Template>
        </px:PXFormView>
    </div>

    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100;"
        Width="100%" DataMember="Pages" DataKeyNames="PageID" SkinID="Transparent" NoteIndicator="False" FilesIndicator="False">
        <Searches>
            <px:PXQueryStringParam Name="PageID" QueryStringField="PageID" Type="String" />
            <px:PXQueryStringParam Name="Language" QueryStringField="Language" Type="String" />
            <px:PXQueryStringParam Name="PageRevisionID" QueryStringField="PageRevisionID" Type="Int32" />
            <px:PXQueryStringParam Name="Wiki" QueryStringField="Wiki" Type="String" />
            <px:PXQueryStringParam Name="Art" QueryStringField="Art" Type="String" />
            <px:PXQueryStringParam Name="Parent" QueryStringField="From" Type="String" />
            <px:PXControlParam ControlID="form" Name="PageID" PropertyName="NewDataKey[&quot;PageID&quot;]" Type="String" />
        </Searches>
        <AutoSize Enabled="True" Container="Window" />
    </px:PXFormView>

    <div id="Rating" runat="server">
        <px:PXFormView ID="PXFormView2" runat="server" CaptionVisible="False" Style="position: static;"
            Width="100%" AllowFocus="False">
            <Template>
                <div style="padding: 5px; position: static;">
                    <div style="border-style: none;">
                        <table style="position: static; border-color: #ECE9E8; height: 20px;" cellpadding="0" cellspacing="0" width="100% ">
                            <tr>
                                <td style="margin-left: 15px; height: 12px; width: 60px;">
                                    <px:PXLabel runat="server" ID="lblRate" Text="Rate this article :" Style="margin-left: 10px; text-wrap: none; white-space: nowrap" />
                                </td>

                                <td style="height: 20px;">
                                    <px:PXDropDown ID="Rate" runat="server" Style="height: 20px; width: 110px; margin-left: 10px" OnCallBack="ddRate_PageRate">
                                        <AutoCallBack Command="ddRate_PageRate">
                                        </AutoCallBack>
                                    </px:PXDropDown>
                                </td>

                                <td style="height: 20px; white-space: nowrap;">
                                    <px:PXButton ID="Button" runat="server" Style="height: 20px; margin-left: 10px;" Text="Rate!" OnCallBack="Rate_PageRate">
                                        <AutoCallBack Command="Rate_PageRate">
                                        </AutoCallBack>
                                    </px:PXButton>
                                </td>

                                <td style="height: 20px; width: 100%;" />

                                <td style="height: 20px; width: 60px;">
                                    <px:PXButton ID="PXButton2" runat="server" Style="height: 20px;" Text="Feedback" OnCallBack="Feedback_Rate">
                                        <AutoCallBack Command="Feedback_Rate">
                                        </AutoCallBack>
                                    </px:PXButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </Template>
        </px:PXFormView>
    </div>

    <div id="dvAnalytics" runat="server">
    </div>

    <div id="SmartPanels" runat="server">
       <px:PXSmartPanel ID="pnlGetLink" runat="server" Caption="This article URL" ForeColor="Black"
            Height="117px" Style="position: static" Width="353px" Position="UnderOwner">
            <px:PXLabel ID="lblLink" runat="server" Style="position: absolute; left: 9px; top: 9px;"
                Text="Internal Link :"></px:PXLabel>
            <px:PXTextEdit ID="edtLink" runat="server" Style="position: absolute; left: 81px; top: 9px;"
                Width="256px">
            </px:PXTextEdit>
            <px:PXLabel ID="lblUrl" runat="server" Style="position: absolute; left: 9px; top: 36px;"
                Text="External Link :"></px:PXLabel>
            <px:PXTextEdit ID="edtUrl" runat="server" Style="position: absolute; left: 81px; top: 36px;"
                Width="256px">
            </px:PXTextEdit>
            <px:PXLabel ID="lblPublicUrl" runat="server" Style="position: absolute; left: 9px; top: 63px;"
                Text="Public Link :"></px:PXLabel>
            <px:PXTextEdit ID="edPublicUrl" runat="server" Style="position: absolute; left: 81px; top: 63px;"
                Width="256px">
            </px:PXTextEdit>
            <px:PXButton ID="PXButton1" runat="server" DialogResult="Cancel" Style="left: 263px; position: absolute; top: 90px; height: 20px;"
                Text="Close" Width="80px">
            </px:PXButton>
        </px:PXSmartPanel>
        <px:PXSmartPanel ID="pnlWikiText" runat="server" CaptionVisible="True" Height="544px"
            Style="position: static" Width="814px">
            <px:PXTextEdit ID="edWikiText" runat="server" Height="536px" Style="position: static; color: Black;"
                TextMode="MultiLine" Width="806px" ReadOnly="True">
            </px:PXTextEdit>
        </px:PXSmartPanel>
    </div>

    <script type="text/javascript">
        px_callback.baseProcessRedirect = px_callback.processRedirect;
        px_callback.processRedirect = function (result, context) {
            var flag = true;
            if (context == null) context = this.context;
            if (context != null && context.context != null)
                if (context.context.command == "delete") {
                    __refreshMainMenu(); flag = false;
                }
            if (flag) this.baseProcessRedirect(result, context);
        }

        function onButtonClick(sender, e) {
            if (e.button.key == "print") {
                // printLink is defined on server
                window.open(printLink, '',
                    'scrollbars=yes,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no,menubar=yes');
            }
        }
    </script>
</asp:Content>
