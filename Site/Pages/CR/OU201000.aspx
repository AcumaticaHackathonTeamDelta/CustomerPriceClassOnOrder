<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="OU201000.aspx.cs" Inherits="Page_OU201000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">       
    <px:PXDataSource ID="ds" runat="server" Width="100%" PrimaryView="Filter"
        TypeName=" PX.Objects.CR.OUSearchMaint" Visible="false" PageLoadBehavior="InsertRecord">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="createLead" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="createContact" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="createCase" />            
            <px:PXDSCallbackCommand CommitChanges="True" Name="createOpportunity" /> 
            <px:PXDSCallbackCommand CommitChanges="True" Name="createActivity" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="logOut" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" Width="100%" Height="643px" DataSourceID="ds" DataMember="Filter" SkinID="Transparent" OnDataBound="form_OnDataBound">
        <Template>                         
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XM"/>
			<px:PXLayoutRule Merge="True" runat="server"  ControlSize="XM" SuppressLabel="true"/> 
            <px:PXTextEdit ID="edSDisplayName" runat="server" DataField="DisplayName" AllowEdit="True"  SkinID="BlueLabel" SuppressLabel="true" Style="color: #0078d7;"/>
			<px:PXButton ID="edHelp" runat="server" Target="_blank" BackColor="Transparent" Style="min-width:10px; width:25px; border-style: none" ToolTip="Help" AlignLeft="false"
				NavigateUrl="../../Main.aspx?ScreenId=ShowWiki&pageid=875b6f76-2820-4ea9-a420-7f7a8bbfb619">
				<Images Normal="~/Icons/OutlookHelp.png" />
			</px:PXButton>
			<px:PXButton ID="epLogOut" runat="server" SupressLabel="true" CommandName="logOut" CommandSourceID="ds" Style="min-width:10px; width:25px; border-style: none" BackColor="Transparent">
				<Images Normal="~/Icons/OutlookLogOut.png" />
			</px:PXButton>
			<px:PXLayoutRule Merge="False" runat="server" />

			<px:PXLayoutRule runat="server" StartGroup="True" ControlSize="M" LabelsWidth="S" />
            <px:PXTextEdit ID="edSEmail" runat="server" DataField="Email" AllowEdit="True" SkinID="BlueLabel" SuppressLabel="true" Style="color: #0078d7;"/>            
            <px:PXDropDown ID="edOutgoingEmail" runat="server" CommitChanges="true" Hidden="true" AutoRefresh="true" DataField="OutgoingEmail" />
            <px:PXTextEdit ID="edErrorMessage" runat="server" DataField="ErrorMessage" AllowEdit="False" SuppressLabel="true" SkinID="Label" Style="color: red" Width="450px" />
            
            <px:PXLayoutRule Merge="True" runat="server" SuppressLabel="true"/>
                <px:PXLabel ID="edSpace0" runat="server" Width="6px" Hidden="true" />
                <px:PXButton ID="edGoCreateLead" runat="server" CommandName="goCreateLead" CommandSourceID="ds" Width="150px" Hidden="true" CssClass="ms-Button"/>
                <px:PXLabel ID="edSpace1" runat="server" Width="10px" Hidden="true" />
                <px:PXButton ID="edGoCreateContact" runat="server" CommandName="goCreateContact" CommandSourceID="ds" Width="150px" Hidden="true" CssClass="ms-Button"/>
            <px:PXLayoutRule Merge="false" runat="server"/>                        

            <px:PXLayoutRule runat="server" GroupCaption="Contact Information" StartGroup="True" ControlSize="M" LabelsWidth="S" />
            <px:PXTextEdit ID="edPosition" runat="server" DataField="Salutation" />
            <px:PXSelector ID="edBAccountID" runat="server" CommitChanges="True" DataField="BAccountID" SimpleSelect="True"/>
            <px:PXTextEdit ID="edCompany" runat="server" DataField="FullName"/>            
            <px:PXDropDown ID="edSource" runat="server" DataField="Source" SimpleSelect="True" />
            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" SimpleSelect="True" />                        
            <px:PXLayoutRule Merge="True" runat="server"/>
            <px:PXTextEdit ID="edEntityName" runat="server" DataField="EntityName" SuppressLabel="true" SkinID="Label" Size="S" Style="color:RGBA(0,0,0,0.54)" />
            <px:PXTextEdit ID="edEntityID" runat="server" DataField="EntityID" Size="M" SuppressLabel="true" Style="margin-left: -16px" />
              
                          
            <px:PXLayoutRule runat="server" GroupCaption="New Case Details" StartGroup="True " ControlSize="M" LabelsWidth="S" />
            <px:PXSelector ID="edCaseClassID" runat="server" DataField="NewCase.CaseClassID" SimpleSelect="True" AutoRefresh="true" CommitChanges="true" />            
            <px:PXSelector ID="edContract" runat="server" DataField="NewCase.ContractID" AutoRefresh="true" SimpleSelect="True"/> 
            <px:PXDropDown ID="edSeverity" runat="server" DataField="NewCase.Severity" SimpleSelect="True" />            
            <px:PXTextEdit ID="edCaseDescription" runat="server" DataField="NewCase.Subject"/>
            <px:PXLayoutRule runat="server" GroupCaption="New Opportunity Details" StartGroup="True" ControlSize="M" LabelsWidth="S" />    
            <px:PXSelector ID="edOpportunityClassID" runat="server" DataField="NewOpportunity.CROpportunityClassID" SimpleSelect="True" AutoRefresh="true" />            
            <px:PXTextEdit ID="edOpportunityName"  runat="server" DataField="NewOpportunity.OpportunityName" />  
            <px:PXDropDown ID="edStageID" runat="server" DataField="NewOpportunity.StageID" SimpleSelect="True" />                                   
            <px:PXDateTimeEdit ID="edCloseDate" runat="server" DataField="NewOpportunity.CloseDate"/>
            <px:PXLayoutRule Merge="true" runat="server" ControlSize="M" LabelsWidth="S"/>         
            <px:PXNumberEdit CommitChanges="True" ID="edCuryAmount" runat="server" DataField="NewOpportunity.ManualAmount" />
            <px:PXSelector ID="edOpportunityCurrencyID" runat="server" DataField="NewOpportunity.CurrencyID" SimpleSelect="True" AutoRefresh="true"  SuppressLabel="true" Width="84px"/>
            <px:PXLayoutRule Merge="false" runat="server" ControlSize="M" LabelsWidth="S"/>
			<px:PXSelector ID="edBranchID" runat="server" DataField="NewOpportunity.BranchID" SimpleSelect="True" AutoRefresh="true" />
            
            <px:PXLayoutRule runat="server" GroupCaption="Log Activity" StartGroup="True " ControlSize="M" LabelsWidth="S" />            
            <px:PXTextEdit ID="edActivitySummary"  runat="server" DataField="NewActivity.Subject" />  
            <px:PXSelector ID="edActivityCaseCD" runat="server" DataField="NewActivity.CaseCD" SimpleSelect="True" AutoRefresh="true" />            
            <px:PXSelector ID="edActivityOpportunityCD" runat="server" DataField="NewActivity.OpportunityID" SimpleSelect="True" AutoRefresh="true" />            
            <px:PXCheckBox Id="chkLinkContact" runat="server" DataField="NewActivity.IsLinkContact" CommitChanges="true" Width="120px"/>
            <px:PXCheckBox Id="chkLinkCase" runat="server" DataField="NewActivity.IsLinkCase" CommitChanges="true" Width="120px"/>
            <px:PXCheckBox Id="chkLinkOpportunity" runat="server" DataField="NewActivity.IsLinkOpportunity" CommitChanges="true" Width="120px"/>
			<px:PXLayoutRule runat="server" GroupCaption=" " StartGroup="True " ControlSize="M" LabelsWidth="S" /> 

			<px:PXLabel ID="edFake" runat="server" DataField="Fake" Height="10px"></px:PXLabel>
			<px:PXLayoutRule Merge="true" runat="server"/>
			<px:PXButton ID="edBack" runat="server" CommandName="back" AlignLeft="true" CommandSourceID="ds" Width="100px" Hidden="true" CssClass="ms-Button"/>
			<px:PXButton ID="edCreateActivity" runat="server" CommandName="createActivity" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
			<px:PXButton ID="edCreateOpportunity" runat="server" CommandName="createOpportunity" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
			<px:PXButton ID="edCreateCase" runat="server" CommandName="createCase" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
			<px:PXButton ID="edCreateLead" runat="server" CommandName="createLead" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>                
            <px:PXButton ID="edCreateContact" runat="server" CommandName="createContact" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>

            <px:PXLayoutRule runat="server" LabelsWidth="XS" ControlSize="M" SuppressLabel="True" />
            <px:PXButton ID="edViewContact" runat="server" CommandName="viewContact" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
            <px:PXButton ID="edViewBAccount" runat="server" CommandName="viewBAccount" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
            <px:PXButton ID="edViewEntity" runat="server" CommandName="viewEntity" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>  
            <px:PXButton ID="edGoCreateActivity" runat="server" CommandName="goCreateActivity" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button" />      
            <px:PXButton ID="edGoCreateCase" runat="server" CommandName="goCreateCase" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
            <px:PXButton ID="edGoCreateOpportunity" runat="server" CommandName="goCreateOpportunity" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>
            <px:PXButton ID="edReply" runat="server" CommandName="reply" CommandSourceID="ds" Width="200px" Hidden="true" CssClass="ms-Button"/>

			<px:PXLabel runat="server" />
        </Template>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXFormView>
    <script type="text/javascript">
    	var officeContext = false;
    	var officeInitRun = false;

    	if (window.Office != null)
    	{
    		// main frame load event handler
    		Office.initialize = function ()
    		{
    			officeInitRun = true;
    			var message = Office.context.mailbox.item;
    			if (message == null) return;

    			px.elemByID("__ouIsIncome").value = "1";
    			px.elemByID("__ouEmail").value = message.sender.emailAddress;
    			px.elemByID("__ouDisplayName").value = message.sender.displayName;
    			if (message.sender.emailAddress == "" ||
						message.sender.emailAddress == Office.context.mailbox.userProfile.emailAddress)
    			{
    				px.elemByID("__ouIsIncome").value = "0";
    				px.elemByID("__ouDisplayName").value = "";
    			}
    			if (window.pageLoaded) initWindow();
    			else __px_cm(window).registerAfterLoad(initWindow);
    		};
    		officeContext = true;
    	}

    	function execReloadPage()
    	{
    		__px_cm(window).reloadPage();
    		var form = px_alls["form"];
    		if (form) form.refresh();
    	}

    	function initWindow()
    	{
    		saveMessage(execReloadPage);
    	}

        function saveMessage(completeCallback)
        {
            var message = Office.context.mailbox.item;
            if (message == null)
            {
                px.elemByID("__oumsMessageId").value = null;
                return;
            }                        
            px.elemByID("__oumsMessageId").value = message.internetMessageId;
            px.elemByID("__oumsItemId").value = message.itemId;
            px.elemByID("__oumsSubject").value = message.subject;
            px.elemByID("__oumsTo").value = serializeAddressList(message.to);
            px.elemByID("__oumsCC").value = serializeAddressList(message.cc);
            px.elemByID("__oumsEwsUrl").value = Office.context.mailbox.ewsUrl;

            getAsyncAttachmentToken(completeCallback);
        }

        function getAsyncAttachmentToken(completeCallback) {
            if (px.elemByID("__oumsToken").value == "") {
                try {
                    Office.context.mailbox.getCallbackTokenAsync(
                    function (asyncResult) {
                        if (asyncResult.status === "succeeded") {
                            px.elemByID("__oumsToken").value = asyncResult.value;
                        }
                        else {
                            px.elemByID("__oumsToken").value = "none";
                            document.body.insertBefore(document.createTextNode('Get Token error. Status = : ' + asyncResult.status), document.body.firstChild);
                        }
                    });
                } catch (ex) {
                    px.elemByID("__oumsToken").value = "none";
                    document.body.insertBefore(document.createTextNode('Error occured: ' + ex), document.body.firstChild);
                }
            }
            completeCallback();
        };

        function serializeAddressList(list)
        {            
            var msg = "";
            if(list!=null)
                list.forEach(function (recip, index)
                {
                    msg = msg + "\"" + recip.displayName + "\"" + " <" + recip.emailAddress + ">;";
                });
            return msg;
        }
    </script>
</asp:Content>
