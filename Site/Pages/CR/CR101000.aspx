<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR101000.aspx.cs" Inherits="Page_CR101000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Width="100%" PrimaryView="CRSetupRecord"
		TypeName="PX.Objects.CR.CRSetupMaint" Visible="True">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" ContainerID="tab" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phF" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="643px" DataSourceID="ds" DataMember="CRSetupRecord"
		LoadOnDemand="True" EmailingGraph="">
		<Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
		<Items>
			<px:PXTabItem Text="General Settings">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXLayoutRule runat="server" GroupCaption="Numbering Sequence" StartGroup="True" />
					<px:PXSelector ID="edOpportunityNumberingID" runat="server" DataField="OpportunityNumberingID"
						AllowEdit="True" />
					<px:PXSelector ID="edCaseNumberingID" runat="server" DataField="CaseNumberingID"
						AllowEdit="True" />
					<px:PXSelector ID="edMassMailNumberingID" runat="server" DataField="MassMailNumberingID"
						AllowEdit="True" />
					<px:PXSelector ID="edCampaignNumberingID" runat="server" DataField="CampaignNumberingID"
						AllowEdit="True" />	
					<px:PXLayoutRule runat="server" GroupCaption="Classes" StartGroup="True" />
					<px:PXSelector ID="edDefaultLeadClassID" runat="server" DataField="DefaultLeadClassID" AllowEdit="True" />
					<px:PXSelector ID="edDefaultContactClassID" runat="server" DataField="DefaultContactClassID" AllowEdit="True" />
					<px:PXSelector ID="edDefaultCustomerClassID" runat="server" DataField="DefaultCustomerClassID"
						AllowEdit="True" />	
					<px:PXSelector ID="edDefaultOpportunityClassID" runat="server" DataField="DefaultOpportunityClassID"
						AllowEdit="True" />
					<px:PXSelector ID="edDefaultCaseClassID" runat="server" DataField="DefaultCaseClassID"
						AllowEdit="True">
						<GridProperties>
							<Layout ColumnsMenu="False" />
						</GridProperties>
					</px:PXSelector>
					<px:PXLayoutRule runat="server" GroupCaption="Assignment Map" StartGroup="True" />
					<px:PXSelector ID="edLeadDefaultAssignmentMapID" runat="server" DataField="LeadDefaultAssignmentMapID"
						TextField="Name" AllowEdit="True">
						<GridProperties>
							<Layout ColumnsMenu="False"></Layout>
						</GridProperties>
					</px:PXSelector>
					<px:PXSelector ID="edContactDefaultAssignmentMapID" runat="server" DataField="ContactDefaultAssignmentMapID"
						TextField="Name" AllowEdit="True">
						<GridProperties>
							<Layout ColumnsMenu="False"></Layout>
						</GridProperties>
					</px:PXSelector>
					<px:PXLayoutRule runat="server" Merge="True" />
					<px:PXSelector Size="xm" ID="edDefaultBAccountAssignmentMapID" runat="server" DataField="DefaultBAccountAssignmentMapID"
						TextField="Name" AllowEdit="True">
						<GridProperties>
							<Layout ColumnsMenu="False"></Layout>
						</GridProperties>
					</px:PXSelector>
					<px:PXLayoutRule runat="server" />
					<px:PXLayoutRule runat="server" Merge="True" />
					<px:PXSelector Size="xm" ID="edDefaultOpportunityAssignmentMapID" runat="server"
						DataField="DefaultOpportunityAssignmentMapID" TextField="Name" AllowEdit="True" />
					<px:PXLayoutRule runat="server" />
					<px:PXSelector ID="edDefaultCaseAssignmentMapID" runat="server" DataField="DefaultCaseAssignmentMapID"
						TextField="Name" AllowEdit="True" />
					
					<px:PXLayoutRule runat="server" GroupCaption="General" StartGroup="True" />
					<px:PXPanel ID="PXPanel1" runat="server" RenderSimple="True" RenderStyle="Simple">
						<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="SM" StartColumn="True"/>  
						<px:PXLayoutRule ID="PXLayoutRule2" runat="server" />
						<px:PXCheckBox ID="lblShowEmailAuthors" runat="server" DataField="ShowAuthorForCaseEmail" Size="M"/>
						<px:PXCheckBox ID="chkCopyNotes" runat="server" Checked="True" DataField="CopyNotes" Size="M"/>
						<px:PXCheckBox ID="chkCopyFiles" runat="server" Checked="True" DataField="CopyFiles" Size="M"/>	
						<px:PXLayoutRule ID="PXLayoutRule10" runat="server" />
						
						<px:PXLayoutRule ID="PXLayoutRule11" runat="server" Merge="True" />
						<px:PXSelector ID="edDefaultRateTypeID" runat="server" AllowEdit="True" DataField="DefaultRateTypeID" Size="sM" style="margin-right: 20px;">
							<GridProperties>
								<Layout ColumnsMenu="False" />
							</GridProperties>
						</px:PXSelector>
						<px:PXCheckBox ID="chkAllowOverrideRate" runat="server" DataField="AllowOverrideRate">
						</px:PXCheckBox>
						<px:PXLayoutRule runat="server" />
						
						<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXSelector ID="edDefaultCuryID" runat="server" AllowEdit="True" DataField="DefaultCuryID"
							Size="sM" style="margin-right: 20px;">
							<GridProperties>
								<Layout ColumnsMenu="False" />
							</GridProperties>
						</px:PXSelector>
						<px:PXCheckBox ID="chkAllowOverrideCury" runat="server" DataField="AllowOverrideCury">
						</px:PXCheckBox>
						<px:PXLayoutRule runat="server" />
					</px:PXPanel>

					<px:PXLayoutRule ID="PXLayoutRule5" runat="server" GroupCaption="Purge Settings" StartGroup="True" />
					<px:PXPanel ID="PXPanel2" runat="server" RenderSimple="True" RenderStyle="Simple">
						<px:PXLayoutRule ID="PXLayoutRule7" runat="server" LabelsWidth="XL" ControlSize="XXS" StartColumn="True"/>  
						<px:PXLayoutRule runat="server" Merge="True" />
			            <px:PXTextEdit ID="edAgeNotConverted" runat="server" DataField="PurgeAgeOfNotConvertedLeads" />
                        <px:PXLabel ID="PXLabel1" runat="server">Months</px:PXLabel>
                        <px:PXLayoutRule runat="server"/>
						<px:PXLayoutRule ID="PXLayoutRule12" runat="server" Merge="True" />
			            <px:PXTextEdit ID="edPeriodWithoutActivity" runat="server" DataField="PurgePeriodWithoutActivity" />
                        <px:PXLabel ID="PXLabel2" runat="server">Months</px:PXLabel>
						<px:PXLayoutRule ID="PXLayoutRule13" runat="server"/>
					</px:PXPanel>

				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Duplicate Validation Settings">
                <Template>
                    <px:PXPanel ID="validationForm" runat="server" Style="z-index: 100" Width="100%" CaptionVisible="False" RenderStyle="Simple" ContentLayout-OuterSpacing="Around">
                        <px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="XL" ControlSize="XXS" />
                        <px:PXNumberEdit ID="edLeadValidationThreshold" Size="xxs" runat="server" DataField="LeadValidationThreshold" Decimals="2" ValueType="Decimal" CommitChanges="true"/>
                        <px:PXNumberEdit ID="edAccountValidationThreshold" Size="xxs" runat="server" DataField="AccountValidationThreshold" Decimals="2" ValueType="Decimal"/>
                        <px:PXNumberEdit ID="edLeadToAccountValidationThreshold" Size="xxs" runat="server" DataField="LeadToAccountValidationThreshold" Decimals="2" ValueType="Decimal"/>
                        <px:PXNumberEdit ID="edCloseLeadsWithoutActivitiesScore" Size="xxs" runat="server" DataField="CloseLeadsWithoutActivitiesScore" Decimals="2" ValueType="Decimal"/>
						<px:PXCheckBox ID="lblContactEmailUnique" runat="server" DataField="ContactEmailUnique" CommitChanges="true"/>
                        <px:PXCheckBox ID="lblValidateContactDuplicatesOnEntry" runat="server" Size="XL" DataField="ValidateContactDuplicatesOnEntry"/>
                        <px:PXCheckBox ID="lblValidateAccountDuplicatesOnEntry" runat="server" DataField="ValidateAccountDuplicatesOnEntry"/>
                    </px:PXPanel>
                        <px:PXTab runat="server" RepaintOnDemand="false" ID="validationTabs">
                            <Items>
                            <px:PXTabItem Text="Lead and Contact Validation Rules">
                                <Template>
                                    <px:PXGrid ID="gridContactValidationRules" SkinID="Details" runat="server" ActionsPosition="Top"
						                AllowSearch="True" DataSourceID="ds" Height="100px" Width="100%" BorderWidth="0px"  MatrixMode="true" FilesIndicator="false" NoteIndicator="false">
                                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100" Container="Parent"/>
						                <Levels>
							                <px:PXGridLevel DataMember="LeadContactValidationRules">
								                <Columns>
									                <px:PXGridColumn DataField="MatchingField" AutoCallBack="True" Width="180px" />
									                <px:PXGridColumn DataField="ScoreWeight" Width="120px" />
									                <px:PXGridColumn DataField="TransformationRule" Width="180px" />
								                </Columns>
							                </px:PXGridLevel>
						                </Levels>
                                        <AutoSize Enabled="True" MinHeight="200" />
					                </px:PXGrid>
                                </Template>
                            </px:PXTabItem>
                            <px:PXTabItem Text="Lead and Account Validation Rules">
                                <Template>
                                    <px:PXGrid ID="gridLeadAccountValidationRules" SkinID="Details" runat="server" ActionsPosition="Top"
						                AllowSearch="True" DataSourceID="ds" Height="100px" Width="100%" BorderWidth="0px"  MatrixMode="true" FilesIndicator="false" NoteIndicator="false">
                                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100"/>
						                <Levels>
							                <px:PXGridLevel DataMember="LeadAccountValidationRules">
								                <Columns>
									                <px:PXGridColumn DataField="MatchingField" AutoCallBack="True" Width="180px" />
									                <px:PXGridColumn DataField="ScoreWeight" Width="120px" />
									                <px:PXGridColumn DataField="TransformationRule" Width="180px" />
								                </Columns>
							                </px:PXGridLevel>
						                </Levels>
                                        <AutoSize Enabled="True" MinHeight="200" />
					                </px:PXGrid>
                                </Template>
                            </px:PXTabItem>
                            <px:PXTabItem Text="Account Validation Rules">
                                <Template>
                                    <px:PXGrid ID="gridAccountValidationRules" SkinID="Details" runat="server" ActionsPosition="Top"
						                AllowSearch="True" DataSourceID="ds" Height="100px" Width="100%" BorderWidth="0px"  MatrixMode="true" FilesIndicator="false" NoteIndicator="false">
                                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100"/>
						                <Levels>
							                <px:PXGridLevel DataMember="AccountValidationRules">
								                <Columns>
									                <px:PXGridColumn DataField="MatchingField" AutoCallBack="True" Width="180px" />
									                <px:PXGridColumn DataField="ScoreWeight" Width="120px" />
									                <px:PXGridColumn DataField="TransformationRule" Width="180px" />
								                </Columns>
							                </px:PXGridLevel>
						                </Levels>
                                        <AutoSize Enabled="True" MinHeight="200" />
					                </px:PXGrid>
                                </Template>
                            </px:PXTabItem>
                            </Items>
                            <AutoSize Enabled="True" MinHeight="100" MinWidth="100" Container="Parent"/>
                        </px:PXTab>
                    </Template>
            </px:PXTabItem>
			<px:PXTabItem Text="Opportunity Probabilities">
				<Template>
					&nbsp;&nbsp;
					<px:PXGrid ID="gridOppoturnityProbabilities" runat="server" SkinID="Details" ActionsPosition="Top"
						AllowSearch="True" DataSourceID="ds" Height="100px" Style="z-index: 100; left: 0px;
						position: absolute; top: 0px" Width="100%" BorderWidth="0px">
						<Levels>
							<px:PXGridLevel DataMember="OpportunityProbabilities" DataKeyNames="StageCode">
								<Columns>
									<px:PXGridColumn TextAlign="Left" DataField="StageCode" Width="120px" Type="DropDownList" />
									<px:PXGridColumn TextAlign="Right" DataField="Probability" Width="54px" AllowNull="False" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Campaign Types">
                <Template>
					&nbsp;
					<px:PXGrid ID="gridCampaignType" SkinID="Details" runat="server" ActionsPosition="Top"
						AllowSearch="True" DataSourceID="ds" Height="100px" Style="z-index: 283; left: 0px;
						position: absolute; top: 0px" Width="100%" BorderWidth="0px">
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Levels>
							<px:PXGridLevel DataMember="CampaignType" DataKeyNames="TypeID">
								<Columns>
									<px:PXGridColumn DisplayFormat="&gt;aaaaaaaaaa" DataField="TypeID" Width="81px" />
									<px:PXGridColumn DataField="Description" Width="180px" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>
