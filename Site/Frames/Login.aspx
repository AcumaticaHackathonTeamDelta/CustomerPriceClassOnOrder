<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Login.master" ClientIDMode="Static" AutoEventWireup="true" 
	CodeFile="Login.aspx.cs" Inherits="Frames_Login" EnableEventValidation="false" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/MasterPages/Login.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="phLogo" runat="Server">
	<asp:DropDownList runat="server" ID="cmbLang" CssClass="login_lang" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="phUser" Runat="Server">
	<asp:Label runat="server" Visible="False" ID="lblUnderMaintenance" CssClass="login_error"></asp:Label>
	<asp:Label runat="server" Visible="False" ID="lblUnderMaintenanceReason" CssClass="login_error"></asp:Label>

	<asp:HiddenField runat="server" ID="idStorage" />
	<asp:TextBox runat="server" ID="l2" style="display: none"  />
	<asp:TextBox runat="server" ID="l3"  style="display: none"  />
	<asp:TextBox runat="server" ID="txtUser" CssClass="login_user border-box" placeholder="My Username" />
	<asp:TextBox runat="server" ID="p4"  style="display: none" />
	<asp:TextBox runat="server" ID="p1" style="display: none"
		TextMode="Password" />
    <asp:TextBox runat="server" ID="disableAutocomplete" style="display: none" />
	<asp:TextBox runat="server" ID="txtPass" Width="100%" CssClass="login_pass border-box" 
		TextMode="Password" placeholder="My Password" />
				
	<asp:TextBox runat="server" ID="txtDummyPass" CssClass="login_pass dummy border-box" ReadOnly="true" Visible="false" />
	<input runat="server" id="txtVeryDummyPass" type="hidden" />

	<asp:DropDownList runat="server" ID="cmbCompany" CssClass="login_company border-box" AutoPostBack="true" OnSelectedIndexChanged="cmbCompany_SelectedIndexChanged" />
	<input runat="server" id="txtDummyCpny" type="hidden" />

	<asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="login_pass border-box" 
		placeholder="New Password" Visible="False" />
	<asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="login_pass border-box" 
		placeholder="Confirm Password" Visible="False" />
    
    <div style="margin-bottom:0.5em">
        <asp:CheckBox runat="server" ID="chkEula" Visible="false" OnClick="onchkEulaСhanged(this.checked);" style="float:left"/>
        <span style="display:table">
            <asp:Label ID="lbEula" runat="server" Text="Check here to indicate that you have read and agree to the terms of the " Visible="false" CssClass="labelB"/>
            <asp:HyperLink ID="hlEula" runat="server" CssClass="login_link" NavigateUrl="~/EULA/saas.pdf" Text="Acumatica User Agreement" Visible="false" Target="_blank"/>
        </span>
    </div>
    
	<asp:TextBox runat="server" ID="txtRecoveryQuestion" CssClass="login_user border-box"  
		placeholder="Recovery Question" Visible="False" />
	<asp:TextBox runat="server" ID="txtRecoveryAnswer" CssClass="login_user border-box"
			placeholder="Your Answer" Visible="False" />

	<asp:Button runat="server" ID="btnLogin" Text="Sign In" OnClick="btnLogin_Click" CssClass="login_button" OnClientClick="login_Click()" />	
	<asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="logincancel_button" Visible="false" />	

	<script type="text/javascript">
 		function login_Click()
 		{
 			var e = window.event;
 			if (e && (e.ctrlKey || e.shiftKey) && e.preventDefault != null) e.preventDefault();
 		}
 		function onchkEulaСhanged(checked)
 		{
 		    btnLogin = document.getElementById('btnLogin');
            btnLogin.disabled = !checked;  
        }
	</script>
</asp:Content>

<asp:Content ID="extLogins" ContentPlaceHolderID="phExt" Runat="Server">
	<div class="extlogin_caption">
		<asp:Label runat="server" ID="lblExtSign" Text="Or sign in with:" CssClass="extlogin_caption" Visible="false" />
	</div>
	<asp:ImageButton runat="server" src="../Icons/loginFederation.png" ID="btnLoginFederation" class="extlogin_button" alt="logo" OnClick="btnLoginFederation_Click" Visible="false" />
	<asp:ImageButton runat="server" src="../Icons/loginGoogle.png" ID="btnLoginGoogle" class="extlogin_button" alt="logo" OnClick="btnLoginOAuth_Click" CommandName="Google" Visible="false" />	
	<asp:ImageButton runat="server" src="../Icons/loginLiveID.png" ID="btnLoginMicrosoft" class="extlogin_button" alt="logo" OnClick="btnLoginOAuth_Click" CommandName="MicrosoftAccount" Visible="false" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="phInfo" runat="Server">
	<div runat="server" id="login_info" style="display:none;">
		<div id="logOutReasone" runat="server" style="display:none;">
			<div runat="server" id="logOutReasoneMsg" class="login_error">Last update was unsuccessful.</div>
		</div>
		<div id="dbmsMisconfigured" runat="server" style="display:none;">
			<div runat="server" id="dbmsProblems" class="login_error">There are problems on database server side:</div>
			<div class="label">Contact server administrator.</div>
		</div>
		<div id="updateError" runat="server" style="display:none;">
			<div class="login_error">Last update was unsuccessful.</div>
			<div class="label">Contact server administrator.</div>
		</div>
		<div id="customizationError" runat="server" style="display:none;">
			<div class="login_error">Warning: customization failed to apply automatically after the upgrade.</div>
			<div class="label">
				Some functionality may be unavailable.<br /> Contact server administrator.<br />
				Click <a href="#" onclick="document.getElementById('custErrorDetails').style.display='';">
				here</a> to view details about this error.
			</div>
			<div style="display:none; width: 100%; height: 200px; margin-top: 10px;" id="custErrorDetails">
				<pre runat="server" id="custErrorContent"></pre>
			</div>
		</div>
		<div id="passwordRecoveryError" runat="server" style="display:none">
			<div class="login_error" id="passwordRecoveryErrorMsg" runat="server" />
		</div>
	</div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="phLinks" runat="Server">
	<input runat="server" id="txtDummyInstallationID" type="hidden" />
	<asp:HyperLink ID="lnkForgotPswd" runat="server" CssClass="login_link" NavigateUrl="~/PasswordRemind.aspx" 
		Text="Forgot Your Credentials?" />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="phStart" runat="Server">
	<script type='text/javascript'>
		window.onload = function ()
		{
			var editor = document.body.querySelector('[id^="txtUser"]');
			if (editor == null || editor.readOnly) editor = document.body.querySelector('[id^="txtNewPassword"]');
			if (editor && !editor.readOnly) editor.focus();
		}
	</script>
</asp:Content>

