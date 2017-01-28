using System;
using System.Drawing;
using System.Web.Compilation;
using PX.Data;
using PX.SM;
using PX.Web.UI;
using PX.Web.Controls;
using PX.Data.Wiki.Parser;

public partial class Page_SM201020 : PX.Web.UI.PXPage
{
	private const string _CALENDAR_SYNC_HANDLER_TYPE = "PX.Objects.EP.PXCalendarSyncHandler";
	private static readonly System.Reflection.MethodInfo _getSyncUrlMethod;

	static Page_SM201020()
	{
		Type syncHandlerType = PXBuildManager.GetType(_CALENDAR_SYNC_HANDLER_TYPE, false);
		if (syncHandlerType != null)
		{
			_getSyncUrlMethod = syncHandlerType.GetMethod("GetSyncUrl", new Type[] {typeof (System.Web.HttpContext), typeof (string)});
		}
	}

	protected void Page_Init(object sender, EventArgs e)
	{
		if (tab != null && _getSyncUrlMethod == null)
		{
			PXTabItem item = tab.Items["calendar"];
			if (item != null) item.Visible = false;
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		PXFormView form = this.tab.FindControl("formUserPrefs") as PXFormView;
		if (form == null) return;
		FillColors((PXDropDown)form.DataControls["edBorderColor"]);
		FillColors((PXDropDown)form.DataControls["edHeaderFontColor"]);
		FillColors((PXDropDown)form.DataControls["edHeaderFillColor"]);
		FillColors((PXDropDown)form.DataControls["edBodyFontColor"]);
		FillColors((PXDropDown)form.DataControls["edBodyFillColor"]);
		FillFonts((PXDropDown)form.DataControls["edBodyFont"]);
		FillFonts((PXDropDown)form.DataControls["edHeaderFont"]);
		FillFontSizes((PXDropDown)form.DataControls["edBodyFontSize"]);
		FillFontSizes((PXDropDown)form.DataControls["edHeaderFontSize"]);
	}

	protected void tab_Init(object sender, EventArgs e)
	{
		bool existMyProfileMaint = System.String.Compare(ds.TypeName, "PX.SM.MyProfileMaint", StringComparison.OrdinalIgnoreCase) == 0;
		tab.Items.Remove(tab.Items[existMyProfileMaint ? "searchSettingsSimple" : "searchSettings"]);
	}

	protected void tab_DataBound(object sender, EventArgs e)
	{
		PXButton button = (PXButton)this.tab.FindControl("btnChangePassword");
		if (button != null)
		{
			PXSmartPanel panel = (PXSmartPanel)this.tab.FindControl("pnlChangePassword");
			PXTextEdit edit = (PXTextEdit)panel.FindControl("edNewPassword");
			if (edit != null)
			{
				button.Enabled = edit.Enabled;
				button.Hidden = edit.Hidden;
			}
		}

		int index;
		if (int.TryParse(Request["tab"], out index) && index < this.tab.Items.Count)
			this.tab.SelectedIndex = index;
	}

	protected void cmdCheckMailSettings_CallBack(object sender, PXCallBackEventArgs e)
	{
		try
		{
			((SMAccessPersonalMaint)ds.DataGraph).getCalendarSyncUrl(
				new PXAdapter(ds.DataGraph.Views[ds.PrimaryView]));
		}
		catch (PXDialogRequiredException ex)
		{
			if (_getSyncUrlMethod != null)
			{
				ex.SetMessage((string)_getSyncUrlMethod.Invoke(null,
						new object[] { System.Web.HttpContext.Current , ex.Message }));
				ex.DataSourceID = ds.ID;
			}
			throw ex;
		}
	}

	private void FillColors(PXDropDown dd)
	{
		string[] colors = PXSpecialResources.ColorNames;
		dd.Items.Clear();
		foreach (string n in colors)
		{
			string nLocalized = n;
			PXLocalizerRepository.SpecialLocalizer.LocalizeColorName(ref nLocalized);

			dd.Items.Add(new PXListItem(string.Empty, nLocalized));
		}
	}

	private void FillFonts(PXDropDown dd)
	{
		dd.Items.Clear();
		string[] fonts = PXSpecialResources.FontNames;
		foreach (string ff in fonts)
		{
			string ffLocalized = ff;
			PXLocalizerRepository.SpecialLocalizer.LocalizeFontName(ref ffLocalized);

			dd.Items.Add(new PXListItem(string.Empty, ffLocalized));
		}
	}

	private void FillFontSizes(PXDropDown dd)
	{
		int[] size = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
		dd.Items.Clear();
		foreach (int s in size) dd.Items.Add(new PXListItem(string.Empty, s.ToString()));
	}
}
