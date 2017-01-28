using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using PX.Data;
using PX.Api;
using PX.Web.UI;
using System.Collections.Generic;

public partial class Page_SM207025 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.Page.IsCallback)
		{
			PXGrid grid = this.tab.FindControl("gridMapping") as PX.Web.UI.PXGrid;
			if (grid != null)
				this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridID", "var gridID=\"" + grid.ClientID + "\";", true);
		}
	}

	protected void edValue_InternalFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		List<string> res = new List<string>();
		SYExportMaint graph = (SYExportMaint)this.ds.DataGraph;
		if (graph.Mappings.Current == null || string.IsNullOrEmpty(graph.Mappings.Current.ScreenID))
			return;

        PXSiteMap.ScreenInfo info = ScreenUtils.GetScreenInfo(graph.Mappings.Current.ScreenID);
		Dictionary<string, bool> addedViews = new Dictionary<string, bool>();
		foreach (string viewname in info.Containers.Keys)
		{
			int index = viewname.IndexOf(": ");
			if (index != -1 && addedViews.ContainsKey(viewname.Substring(0, index)))
				continue;
			addedViews.Add(viewname, true);
			foreach (PX.Data.Description.FieldInfo field in info.Containers[viewname].Fields)
				res.Add("[" + viewname + "." + field.FieldName + "]");
		}
		e.Result = string.Join(";", res.ToArray());
	}

	protected void edValue_ExternalFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		List<string> res = new List<string>();
		foreach (SYProviderField field in PXSelect<SYProviderField, Where<SYProviderField.providerID, Equal<Current<SYMapping.providerID>>,
										And<SYProviderField.objectName, Equal<Current<SYMapping.providerObject>>,
										And<SYProviderField.isActive, Equal<True>>>>,
										OrderBy<Asc<SYProviderField.displayName>>>.Select(this.ds.DataGraph))
		{
			res.Add("[" + field.Name + "]");
		}
		e.Result = string.Join(";", res.ToArray());
	}
}
