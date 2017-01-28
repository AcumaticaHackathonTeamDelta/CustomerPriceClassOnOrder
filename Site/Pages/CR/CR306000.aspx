<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CR306000.aspx.cs" Inherits="Page_CR306000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CRCaseMaint" PrimaryView="Case">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true"/>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" PopupVisible="True" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="True" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="Release" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Action" StartNewGroup="true" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Assign" Visible="False" RepaintControls="All" />
            <px:PXDSCallbackCommand Name="ViewInvoice" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="ViewActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="OpenActivityOwner" Visible="False" CommitChanges="True" DependOnGrid="gridActivities" />
            <px:PXDSCallbackCommand Name="Relations_EntityDetails" Visible="False" CommitChanges="True" DependOnGrid="grdRelations" />
            <px:PXDSCallbackCommand Name="Relations_ContactDetails" Visible="False" CommitChanges="True" DependOnGrid="grdRelations" />
            <px:PXDSCallbackCommand Name="CaseRefs_CRCase_ViewDetails" Visible="False" CommitChanges="True" DependOnGrid="gridCaseReferencesDependsOn" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="TakeCase" PopupVisible="True" />
        </CallbackCommands>
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Case" Caption="Case Summary" NoteIndicator="True" FilesIndicator="True"
        LinkIndicator="True" NotifyIndicator="True" DefaultControlID="edCaseCD">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
            <px:PXSelector ID="edCaseCD" runat="server" DataField="CaseCD" FilterByAllFields="True" AutoRefresh="True" />
            <px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" DataField="CreatedDateTime" DisplayFormat="g" Enabled="False" Size="SM"/>
            <px:PXDateTimeEdit ID="edLastModifiedDateTime" runat="server" DataField="LastActivity" DisplayFormat="g" Enabled="False" Size="SM"/>
            <px:PXDateTimeEdit ID="edSLAETA" runat="server" DataField="SLAETA" DisplayFormat="g" Enabled="False" EditFormat="g" Size="SM"/>
            <px:PXDateTimeEdit ID="edResolutionDate" runat="server" DataField="ResolutionDate" Enabled="False" Size="SM"/>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSelector CommitChanges="True" ID="edCaseClassID" runat="server" DataField="CaseClassID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint"
                AutoRefresh="True" />
            <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" DisplayMode="Text" TextMode="Search" TextField="displayName" AutoRefresh="True"
                AllowEdit="True" FilterByAllFields="True" />
            <px:PXSelector ID="OwnerID" runat="server" DataField="OwnerID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" AutoRefresh="true" CommitChanges="True"/>
            <px:PXLayoutRule ID="PXLayoutRule4" runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edSubject" runat="server" DataField="Subject" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" DataField="Status" AllowNull="False" />
            <px:PXDropDown CommitChanges="True" ID="edResolution" runat="server" DataField="Resolution" AllowNull="False"/>
            <px:PXDropDown CommitChanges="True" ID="edSeverity" runat="server" DataField="Severity" SelectedIndex="-1" AllowNull="False" />
            <px:PXDropDown ID="edPriority" runat="server" DataField="Priority" SelectedIndex="-1" AllowNull="False" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="400px" DataSourceID="ds" DataMember="CaseCurrent" MarkRequired="Dynamic">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXRichTextEdit ID="edDescription" runat="server" Style="border-width: 0px; width: 100%;" DataField="Description" 
						AllowLoadTemplate="false"  AllowDatafields="false"  AllowMacros="true" AllowSourceMode="true" AllowAttached="true" AllowSearch="true">
						<AutoSize Enabled="True" />
						<LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="Notifications" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M"/>
					</px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Additional Info">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Business Account Details" />
                    <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint"
                        AutoRefresh="True" Enabled="false" />
                    <px:PXSegmentMask CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True" AllowEdit="True" FilterByAllFields="True" TextMode="Search"
                        DisplayMode="Hint" />
                    <px:PXSelector CommitChanges="True" ID="edContractID" runat="server" DataField="ContractID" DisplayMode="Hint" TextMode="Search" AutoRefresh="True" AllowEdit="True"
                        FilterByAllFields="True" />
                    <px:PXLayoutRule ID="PXLayoutRule9" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Billing" />
                    <px:PXCheckBox CommitChanges="True" ID="chkIsBillable" runat="server" DataField="IsBillable" />
                    <px:PXCheckBox CommitChanges="True" ID="chkManualBillableTimes" runat="server" DataField="ManualBillableTimes" />
                    <px:PXTimeEdit ID="edTimeBillable" runat="server" DataField="TimeBillable"/>
                    <px:PXTimeEdit ID="edOvertimeBillable" runat="server" DataField="OvertimeBillable"/>
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartGroup="True" LabelsWidth="M" ControlSize="XM" StartColumn="True" GroupCaption="Service Details" />
                    <px:PXSelector ID="WorkgroupID" runat="server" DataField="WorkgroupID" CommitChanges="True" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
                    <px:PXMaskEdit ID="edInitResponse" runat="server" DataField="InitResponse" InputMask="### hrs ## mins" />
                    <px:PXMaskEdit ID="edTimeSpent" runat="server" DataField="TimeSpent" InputMask="### hrs ## mins" />
                    <px:PXMaskEdit ID="edOvertimeSpent" runat="server" DataField="OvertimeSpent" InputMask="### hrs ## mins" />
                    <px:PXMaskEdit ID="edTimeToResolution" runat="server" DataField="TimeResolution" InputMask="### hrs ## mins" />
                    <px:PXDateTimeEdit ID="edLastIncomingDate" runat="server" DataField="CRActivityStatistics__LastIncomingActivityDate" Enabled="False" Size="SM"/>
                    <px:PXDateTimeEdit ID="edLastOutgoingDate" runat="server" DataField="CRActivityStatistics__LastOutgoingActivityDate" Enabled="False" Size="SM"/>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%" Height="200px" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <Columns>
                                    <px:PXGridColumn DataField="AttributeID" TextAlign="Left" Width="250px" AllowShowHide="False" TextField="AttributeID_description" />
    								<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" Width="80px"/>
                                    <px:PXGridColumn DataField="Value" Width="300px" AllowShowHide="False" AllowSort="False" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="200" />
                        <ActionBar>
                            <Actions>
                                <Search Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Activities" LoadOnDemand="true">
                <Template>
                    <pxa:PXGridWithPreview ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
                        FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Inquire" PreviewPanelStyle="z-index: 100; background-color: Window"
                        PreviewPanelSkinID="Preview" BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">
                        <ActionBar ActionsText="true" DefaultAction="cmdViewActivity" PagerVisible="False">
                            <CustomItems>
                                <px:PXToolBarButton Key="cmdAddTask">
                                    <AutoCallBack Command="NewTask" Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEvent">
                                    <AutoCallBack Command="NewEvent" Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEmail">
                                    <AutoCallBack Command="NewMailActivity" Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddActivity">
                                    <AutoCallBack Command="NewActivity" Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Activities">
			                    <RowTemplate>
					                <px:PXTimeSpan TimeMode="True" ID="edTimeSpent" runat="server" DataField="TimeSpent" InputMask="hh:mm" MaxHours="99" />
					                <px:PXTimeSpan TimeMode="True" ID="edTimeBillable" runat="server" DataField="TimeBillable" InputMask="hh:mm" MaxHours="99" />
					                <px:PXTimeSpan TimeMode="True" ID="edOvertimeSpent" runat="server" DataField="OvertimeSpent" InputMask="hh:mm" MaxHours="99" />
					                <px:PXTimeSpan TimeMode="True" ID="edOvertimeBillable" runat="server" DataField="OvertimeBillable" InputMask="hh:mm" MaxHours="99" />
                                </RowTemplate>                                
                                <Columns>
                                    <px:PXGridColumn DataField="IsCompleteIcon" Width="21px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="PriorityIcon" Width="21px" AllowShowHide="False" AllowResize="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassIcon" Width="31px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassInfo" />
                                    <px:PXGridColumn DataField="RefNoteID" Visible="false" AllowShowHide="False" />
                                    <px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="297px" />
                                    <px:PXGridColumn DataField="UIStatus" />
                                    <px:PXGridColumn DataField="Released" Width="80px" />
                                    <px:PXGridColumn DataField="StartDate" DisplayFormat="g" Width="120px" />
                                    <px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="120px" Visible="False" />
                                    <px:PXGridColumn DataField="CategoryID" />
                                    <px:PXGridColumn AllowNull="False" DataField="IsBillable" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="TimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="OvertimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="TimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="OvertimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="CreatedByID_Creator_Username" Visible="false" SyncVisible="False" SyncVisibility="False" Width="108px" />
                                    <px:PXGridColumn DataField="WorkgroupID" Width="90px" />
                                    <px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" Width="150px" DisplayMode="Text"/>
                                    <px:PXGridColumn DataField="ProjectID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="ProjectTaskID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <CallbackCommands>
                            <Refresh CommitChanges="True" PostData="Page" />
                        </CallbackCommands>
                        <GridMode AllowAddNew="False" AllowUpdate="False" />
                        <PreviewPanelTemplate>
                            <px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine" MaxLength="50" Width="100%" Height="100px" SkinID="Label" >
                                      <AutoSize Container="Parent" Enabled="true" />
                                </px:PXHtmlView>
                        </PreviewPanelTemplate>
                        <AutoSize Enabled="true" />
                        <GridMode AllowAddNew="False" AllowDelete="False" AllowFormEdit="False" AllowUpdate="False" AllowUpload="False" />
                    </pxa:PXGridWithPreview>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Related Cases" LoadOnDemand="true">
                <Template>
                    <px:PXGrid ID="gridCaseReferencesDependsOn" runat="server" DataSourceID="ds" Height="162px" Style="z-index: 101; left: 0px; position: absolute; top: 0px;" AllowSearch="True"
                        ActionsPosition="Top" SkinID="Details" Width="100%" BorderWidth="0px">
                        <Levels>
                            <px:PXGridLevel DataMember="CaseRefs">
                                <Columns>
                                    <px:PXGridColumn DataField="ChildCaseID" Width="100px" RenderEditorText="True" AutoCallBack="True" LinkCommand="CaseRefs_CRCase_ViewDetails" />
                                    <px:PXGridColumn DataField="RelationType" Width="100px" RenderEditorText="True" AutoCallBack="true" />
                                    <px:PXGridColumn DataField="CRCaseRelated__Subject" Width="250px" />
                                    <px:PXGridColumn DataField="CRCaseRelated__Status" Width="100px" />
                                    <px:PXGridColumn DataField="CRCaseRelated__OwnerID" Width="150px" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="CRCaseRelated__WorkgroupID" Width="150px" />
                                </Columns>
                                <Mode InitNewRow="true" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="300" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Relations" LoadOnDemand="True">
                <Template>
                    <px:PXGrid ID="grdRelations" runat="server" Height="400px" Width="100%" AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details">
                        <Levels>
                            <px:PXGridLevel DataMember="Relations">
                                <Columns>
                                    <px:PXGridColumn DataField="Role" Width="120px" />
                                    <px:PXGridColumn DataField="EntityID" Width="160px" AutoCallBack="true" TextField="EntityCD" LinkCommand="Relations_EntityDetails" />
                                    <px:PXGridColumn DataField="Name" Width="200px" />
                                    <px:PXGridColumn DataField="ContactID" Width="160px" AutoCallBack="true" TextField="ContactName" DisplayMode="Text" LinkCommand="Relations_ContactDetails" />
                                    <px:PXGridColumn DataField="Email" Width="120px" />
                                    <px:PXGridColumn DataField="AddToCC" Width="70px" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXSelector ID="edRelEntityID" runat="server" DataField="EntityID" FilterByAllFields="True" />
                                    <px:PXSelector ID="edRelContactID" runat="server" DataField="ContactID" FilterByAllFields="True" AutoRefresh="True" />
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <Mode InitNewRow="True" />
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="100" MinWidth="300" />
    </px:PXTab>
</asp:Content>
