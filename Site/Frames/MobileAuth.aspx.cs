using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Frames_MobileAuth : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var request = HttpContext.Current.Request;
        var result = request.QueryString["result"];
        if (string.Equals(result, "success", StringComparison.OrdinalIgnoreCase))
        {
            lblSuccess.Visible = true;
            imgSuccess.Visible = true;
        }

        if (string.Equals(result, "fail", StringComparison.OrdinalIgnoreCase))
        {
            lblFail.Visible = true;
            imgFail.Visible = true;
        }
    }
}