using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page_GL307000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		string workBookID =  Page.Request.QueryString["WorkBookID"];
		if (string.IsNullOrEmpty(workBookID)) 
		{
			//this.
		}
	}
}
