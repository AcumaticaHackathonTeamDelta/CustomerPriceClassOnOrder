using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using PX.Common;
using PX.Objects.CR;
using PX.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

public partial class Page_OU201000 : PX.Web.UI.PXPage
{
	private const string _outlookEmail = "__ouEmail";
	private const string _outlookDisplayName = "__ouDisplayName";
    private const string _outlookIsIncome = "__ouIsIncome";
	
    private const string _outlookEwsUrl = "__ouEwsUrl";
    private const string _outlookAttachemntInfo = "__ouAttachmentInfo";
    private const string _outlookAttachmentToken = "__ouAttachmentToken";

	private string[] _messageFields =
	{
		"MessageId", "To", "CC", "Subject", "ItemId", "EwsUrl", "Token"
	};

	const string oums = "__oums";

	protected void Page_Init(object sender, EventArgs e)
	{
		Page.Header.Controls.Add(
				new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Content/fabric.min.css") + "\" />"));
		Page.Header.Controls.Add(
				new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Content/fabric.components.min.css") + "\" />"));

		this.Master.FindControl("usrCaption").Visible = false;	 
		this.ClientScript.RegisterClientScriptInclude(this.GetType(), "msd", "/Scripts/modernizr-2.8.3.js" );
		this.ClientScript.RegisterClientScriptInclude(this.GetType(), "ms", "//appsforoffice.microsoft.com/lib/1.1/hosted/office.js");		
		//this.ClientScript.RegisterClientScriptInclude(this.GetType(), "ms", VirtualPathUtility.ToAbsolute("~/Scripts/Office/1/office.debug.js"));
		this.ClientScript.RegisterHiddenField(_outlookEmail, null);
		this.ClientScript.RegisterHiddenField(_outlookDisplayName, null);
        this.ClientScript.RegisterHiddenField(_outlookIsIncome, null);

		this.ClientScript.RegisterHiddenField(_outlookEwsUrl, null);
		this.ClientScript.RegisterHiddenField(_outlookAttachemntInfo, null);
		this.ClientScript.RegisterHiddenField(_outlookAttachmentToken, null);

		_messageFields.ForEach(f=> this.ClientScript.RegisterHiddenField(oums + f, null));
		this.ClientScript.RegisterHiddenField("suppressReloadPage", 1.ToString());
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsCallback)
		{
			OUSearchMaint graph = (OUSearchMaint)this.ds.DataGraph;
            if (this.Request.Form[_outlookIsIncome] == "1")
			    graph.Filter.Current.OutgoingEmail = this.Request.Form[_outlookEmail];
			graph.Filter.Current.DisplayName = this.Request.Form[_outlookDisplayName];
			if (this.Request.Form[oums + _messageFields[0]] != null)
			{				

					_messageFields.ForEach(
						f => 
						graph.SourceMessage.Cache.SetValue(graph.SourceMessage.Current, f, 
							this.Request.Form[oums + f])
					);
			}
			graph.SourceMessage.Current.IsIncome = true;
            if (this.Request.Form[_outlookIsIncome] != "1")
                graph.SourceMessage.Current.IsIncome = false;
            
            var token = (string)this.Request.Form[_outlookAttachmentToken];
            if (!String.IsNullOrEmpty(graph.SourceMessage.Current.Token))// && graph.SourceMessage.Current.Body != null)
			{
                graph.CreateActivity.SetEnabled(true);
                graph.CreateCase.SetEnabled(true);
                graph.CreateContact.SetEnabled(true);
                graph.CreateLead.SetEnabled(true);
                graph.CreateOpportunity.SetEnabled(true);
            }
            else
            {
                graph.CreateActivity.SetEnabled(false);
                graph.CreateCase.SetEnabled(false);
                graph.CreateContact.SetEnabled(false);
                graph.CreateLead.SetEnabled(false);
                graph.CreateOpportunity.SetEnabled(false);
            }
		}
	}

	protected void form_OnDataBound(object sender, EventArgs e)
	{
		try
		{
			OUSearchMaint graph = (OUSearchMaint) this.ds.DataGraph;
			if (graph.Filter.Current.ContactType == null)
			{
				((PXTextEdit) this.form.FindControl("edPosition")).Enabled = false;
				((PXTextEdit) this.form.FindControl("edCompany")).Enabled = false;
			}
		}
		catch
		{			
		}
	}
}
