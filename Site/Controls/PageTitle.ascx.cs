#region Copyright (c) 1994-2006 PXSoft, Inc. All rights reserved.

/* ---------------------------------------------------------------------*
*                               PXSoft, Inc.                            *
*              Copyright (c) 1994-2006 All rights reserved.             *
*                                                                       *
*                                                                       *
* This file and its contents are protected by United States and         *
* International copyright laws.  Unauthorized reproduction and/or       *
* distribution of all or any portion of the code contained herein       *
* is strictly prohibited and will result in severe civil and criminal   *
* penalties.  Any violations of this copyright will be prosecuted       *
* to the fullest extent possible under law.                             *
*                                                                       *
* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *
* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *
* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY PXSoft PRODUCT.          *
*                                                                       *
* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *
* --------------------------------------------------------------------- *
*/

#endregion Copyright (c) 1994-2006 PXSoft, Inc. All rights reserved.

#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using PX.Common;
using PX.Data;
using PX.SM;
using PX.Web.Controls;
using PX.Web.Controls.Wiki;
using PX.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;

#endregion

[Themeable(true)]
public partial class User_PageTitle : TitlePanel, ITitleModuleController
{
	#region Public properties

	private HtmlAnchor LabelScreen
	{
		get
		{
			if (labelScreen == null)
			{
				PXToolBarContainer cont = (PXToolBarContainer)tlbPath.Items["title"];
				foreach (Control c in cont.TemplateContainer.Controls)
					if (c.ID == "lblScreenTitle") { labelScreen = c as HtmlAnchor; break; }
			}
			return labelScreen;
		}
	}

	public PXDataViewBar DataViewBar
	{
		get { return this.tlbDataView; }
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// Gets or sets the skin to apply to the control.
	/// </summary>
	[Browsable(true)]
	public override string SkinID
	{
		get { return base.SkinID; }
		set { base.SkinID = value; }
	}

	/// <summary>
	/// Gets or sets the screen image url.
	/// </summary>
	[Category("Appearance"), Description("The screen logo image url.")]
	public string ScreenImage
	{
		get { return screenImage; }
		set { screenImage = value; }
	}

	/// <summary>
	/// Gets or sets the screen identifier.
	/// </summary>
	[Category("Appearance"), Description("The screen identifier.")]
	public override string ScreenID
	{
		get { return screenID; }
		set { screenID = value; }
	}

	/// <summary>
	/// Gets or sets the screen title.
	/// </summary>
	[Category("Appearance"), Description("The screen title.")]
	public override string ScreenTitle
	{
		get { return screenTitle; }
		set
		{
			screenTitle = value;

			if (LabelScreen != null)
			{
				LabelScreen.InnerText = screenTitle;
				Page.Title = screenTitle;
			}
		}
	}

	/// <summary>
	/// Gets or sets branch visibility in title.
	/// </summary>
	public bool BranchAvailable
	{
		get
		{
			if (tlbPath != null && tlbPath.Items != null && tlbPath.Items.Count > 0)
			{
				return tlbPath.Items["branch"].Visible;
			}
			return false;
		}
		set
		{
			if (tlbPath != null && tlbPath.Items != null && tlbPath.Items.Count > 0)
			{
				tlbPath.Items["branch"].Visible = value;
				//tlbPath.Items["branchLabel"].Visible = value;
				//tlbPath.Items["branchSep"].Visible = value;
			}
		}
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// Gets or sets the favorites availability.
	/// </summary>
	[Category("Appearance"), Description("The favorites availability."), DefaultValue(true)]
	public override bool FavoriteAvailable
	{
		get { return favoriteAvalable; }
		set
		{
			favoriteAvalable = value;
			tlbPath.Items["favorites"].Visible = value;
		}
	}

	/// <summary>
	/// Gets or sets the refresh availability.
	/// </summary>
	[Category("Appearance"), Description("The refresh availability.")]
	public bool RefreshAvailable
	{
		get { return Button(_TOOLBAR_REFRESH).Visible; }
		set { Button(_TOOLBAR_REFRESH).Visible = value; }
	}

	/// <summary>
	/// Gets or sets the logount availability.
	/// </summary>
	[Category("Appearance"), Description("The help availability.")]
	public bool HelpAvailable
	{
		get { return Button(_TOOLBAR_HELP).Visible; }
		set { Button(_TOOLBAR_HELP).Visible = value; }
	}

	/// <summary>
	/// Gets or sets the customization availability.
	/// </summary>
	[Category("Appearance"), Description("The customization availability.")]
	public override bool CustomizationAvailable
	{
		get { return customizationAvalable; }
		set { customizationAvalable = value; }
	}

	/// <summary>
	/// Gets or sets the web services availability.
	/// </summary>
	[Category("Appearance"), Description("The web services availability.")]
	public override bool WebServicesAvailable
	{
		get { return webServicesAvailable; }
		set { webServicesAvailable = value; }
	}

	/// <summary>
	/// Gets or sets the audit history availability.
	/// </summary>
	[Category("Appearance"), Description("The audit history availability.")]
	public override bool AuditHistoryAvailable
	{
		get { return auditHistoryAvailable; }
		set { auditHistoryAvailable = value; }
	}

	#endregion

	#region Event handlers

	//---------------------------------------------------------------------------
	/// <summary>
	/// The page Init event handler.
	/// </summary>
	protected void Page_Init(object sender, EventArgs e)
	{
		string hide = this.Page.Request.QueryString[PXUrl.HidePageTitle];
		if (!string.IsNullOrEmpty(hide)) { this.Visible = false; return; }

		this.Page.InitComplete += new EventHandler(Page_InitComplete);

		if (PXDataSource.RedirectHelper.IsPopupPage(Page))
		{
			if (!PXList.Provider.HasList(PXSiteMap.CurrentNode.ScreenID))
				tlbPath.Items["syncTOC"].Visible = false;
            this.FavoriteAvailable = false;
			if (!PXDataSource.RedirectHelper.IsPopupInline(Page))
				GetBranchCombo().Enabled = false;
		}

		if (PXSiteMap.IsPortal)
		{
            this.CustomizationAvailable = PXAccess.GetAdministratorRoles().Any(System.Web.Security.Roles.IsUserInRole);
            this.BranchAvailable = false;
		    this.FavoriteAvailable = false;
			pnlTBR.CssClass = "panelTBRSP";
		}

		JSManager.RegisterModule(new MSScriptRenderer(Page.ClientScript), typeof(AppJS), AppJS.PageTitle);

		if (PXContext.PXIdentity.Authenticated)
		{
			userName = PXContext.PXIdentity.IdentityName;
			string branch = PXAccess.GetBranchCD();
			if (!string.IsNullOrEmpty(branch)) userName += ":" + branch;
		}

	    if (screenID == null)
	    {
            this.screenID = ControlHelper.GetScreenID();
	    }
		if (screenTitle == null)
		{
			this.screenID = ControlHelper.GetScreenID();
			//PX.Common.PXContext.SetScreenID(screenID); // moved to data source

			if (System.Web.SiteMap.CurrentNode != null)
			{
				if (company == null || System.Web.SiteMap.CurrentNode.ParentNode != null)
					screenTitle = PXSiteMap.CurrentNode.Title;
				else
					screenTitle = company;
			}
		}

		var date = PXContext.GetBusinessDate();
		if (date != null) PXDateTimeEdit.SetDefaultDate((DateTime)date);

		if (!Page.IsCallback) Session.Remove("StoredSearch");
		Uploader.FileUploadFinished += Uploader_FileUploadFinished;
		tlbTools.ItemCreate += tlbTools_ItemCreate;
	}
	protected void Page_InitComplete(object sender, EventArgs e)
	{
		this.InitializeModules();
		this.FillBrachesList();
		this.InitAuditMenu();

		if (!this.Page.IsCallback || ControlHelper.IsReloadPage(tlbPath))
		{
			var favBtn = (PXToolBarButton)tlbPath.Items["favorites"];
			PXSiteMapNode currentNode = System.Web.SiteMap.CurrentNode as PXSiteMapNode;
			if (currentNode == null || String.IsNullOrEmpty(currentNode.ScreenID))
			{
				favBtn.Visible = false;
			}
			else
			{
				tlbPath.CallbackUpdatable = favBtn.DynamicText = true;
				favBtn.Pushed = this.IsInFavorites(null);
				favBtn.Tooltip = favBtn.Pushed ? InfoMessages.RemoveFromFavorites : InfoMessages.AddToFavorites;
			}
		}

		if (ControlHelper.IsReloadPage(tlbPath))
		{
			var filterBtn = (PXToolBarButton)tlbPath.Items["clearFilter"];
			string filterName = this.GetListFilterName();
			if (!string.IsNullOrEmpty(filterName))
			{
				filterBtn.Visible = true;
				filterBtn.Text = filterName + " ";
			}
			tlbPath.CallbackUpdatable = filterBtn.DynamicText = true;
		}

		// Force the customization controls creation !!!
		this.EnsureChildControls();
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		RegisterModules();
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// The page Load event handler.
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		var ds = PXPage.GetDefaultDataSource(this.Page);
		if (ds != null && ds.HasActiveContext)
		{
			if (ControlHelper.IsReloadPage(this))
			{
				var ctx = ds.DataGraph.GetContextViewDescriptor();
				if (ctx != null)
				{
					if (ctx.HeaderValues != null)
					{
						screenTitle = string.Join(" - ", ctx.HeaderValues).ToUpper();
					}
					var cm = PXCallbackManager.GetInstance();
					PXCallbackManager.RegisterJavaScript(
						string.Format("px.elemByID('{0}').innerHTML = '{1}';", LabelScreen.ClientID, screenTitle));
				}
			}
			else screenTitle = " ";
		}

		if (!string.IsNullOrEmpty(screenTitle))
		{
			screenTitle = HttpUtility.HtmlDecode(screenTitle);
			Page.Title = screenTitle;
		}
		if (!this.Visible) return;

		Page.ClientScript.RegisterClientScriptBlock(GetType(), "addToFavorites", "var addToFavorites=\"" + PXMessages.Localize(InfoMessages.AddToFavorites) + "\";", true);
		Page.ClientScript.RegisterClientScriptBlock(GetType(), "removeFromFavorites", "var removeFromFavorites=\"" + PXMessages.Localize(InfoMessages.RemoveFromFavorites) + "\";", true);
		if (!this.Page.IsCallback || ControlHelper.IsReloadPage(tlbPath))
		{
			InitHelpMenu();
			RearrangeAuditMenu();
		}

		if (this.Request.RawUrl.IndexOf("CS100000.aspx", StringComparison.InvariantCultureIgnoreCase) < 0 &&
						this.Request.RawUrl.IndexOf("/soap/", StringComparison.InvariantCultureIgnoreCase) == -1 &&
						this.Request.RawUrl.IndexOf("/wiki/", StringComparison.InvariantCultureIgnoreCase) == -1)
		{
			if (!PXAccess.FeatureSetInstalled("PX.Objects.CS.FeaturesSet"))
			{
				PXSiteMapNode cs = PXSiteMap.Provider.FindSiteMapNodeByScreenID("CS100000");
				if (cs != null)
				{
					string navigateUrl = ResolveUrl(cs.Url);
					if (!Page.IsCallback) Response.Redirect(navigateUrl);
				}
			}
		}

		string localPath = Request.Url.LocalPath;
		if (!localPath.EndsWith("Main.aspx") && !Request.Url.Query.Contains("PopupPanel=On"))
		{
			if (!localPath.EndsWith("Default.aspx"))
			{
				string lastUrl = (string)PXContext.Session["LastUrl"];
				if (String.IsNullOrEmpty(lastUrl) || lastUrl.EndsWith("Default.aspx"))
					Controls.Add(new LiteralControl("<script  type=\"text/javascript\">try { window.top.lastUrl=null; } catch (ex) {}</script>\n"));
			}
			PXContext.Session.SetString("LastUrl", Request.RawUrl);
		}

		if (!Page.IsPostBack && !String.IsNullOrEmpty(ScreenID))
			PX.Data.PXAuditJournal.Register(ScreenID);

		if (!string.IsNullOrEmpty(screenTitle))
		{
			LabelScreen.InnerText = screenTitle;

			string relPath = ResolveUrl(Request.RawUrl);
			string url = ResolveUrl(Request.RawUrl);
			LabelScreen.HRef = (Page != null && Page.GetType().Name == "wiki_showwiki_aspx") ? PXSessionStateStore.GetSessionUrl(HttpContext.Current, url) : url;

			if (PXSiteMap.CurrentNode != null && PXList.Provider.HasList(PXSiteMap.CurrentNode.ScreenID))
			{
				string listScreenID = PXList.Provider.GetListID(PXSiteMap.CurrentNode.ScreenID);
				PXSiteMapNode listNode = PXSiteMap.Provider.FindSiteMapNodeByScreenID(listScreenID);
				if (listNode != null)
				{
					LabelScreen.HRef = ResolveUrl(listNode.Url);
					LabelScreen.ServerClick += (o, args) =>
					{
						PXList.Provider.SetCurrentSearches(PXSiteMap.CurrentNode.ScreenID, null);
						Response.Redirect(ResolveUrl(listNode.Url));
					};
				}
			}
			Page.Title = screenTitle;
			Uploader.NamePrefix = screenTitle;
		}

		if (!Page.IsCallback)
		{
			Page.EnableViewState = false;
			RegisterSyncTreeVars();
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "toolbarNum", "var __toolbarID=\"" + this.tlbTools.ClientID + "\";", true);
		}
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// The About panel load event handler.
	/// </summary>
	protected void pnlAbout_LoadContent(object sender, EventArgs e)
	{
		PXLabel lbl = (PXLabel)pnlAbout.FindControl("lblVersion");
		lbl.Text = this.GetVersion(false);

		if (PX.SM.UpdateMaint.CheckForUpdates())
		{
			lbl = (PXLabel)pnlAbout.FindControl("lblUpdates");
			lbl.Text = PXMessages.LocalizeFormatNoPrefix(PX.AscxControlsMessages.PageTitle.Updates, PXVersionInfo.Version);
			lbl.Style["display"] = "";
		}

		lbl = (PXLabel)pnlAbout.FindControl("lblCopyright2");
		lbl.Text = PXMessages.LocalizeFormatNoPrefix(PX.AscxControlsMessages.PageTitle.Copyright2);

		lbl = (PXLabel)pnlAbout.FindControl("lblInstallationID");
		// hiding InstallationID if it is empty
		if (String.IsNullOrEmpty(PXVersionInfo.InstallationID)) lbl.Visible = false;
		lbl.Text = PXMessages.LocalizeFormatNoPrefix(PX.AscxControlsMessages.PageTitle.InstallationID, PXLicenseHelper.InstallationID);

		string copyR = PXVersionInfo.Copyright;
		lbl = (PXLabel)pnlAbout.FindControl("lblCopyright1");
		if (!string.IsNullOrEmpty(copyR)) lbl.Text = copyR;
	}

	/// <summary>
	/// The Audit panel load event handler.
	/// </summary>
	protected void pnlAudit_LoadContent(object sender, EventArgs e)
	{
		PXDataSource datasource = ((PXPage)this.Page).DefaultDataSource;

		PX.Data.Process.AUAuditPanelInfo info = null;
		if (datasource != null) info = PX.Data.Process.PXAuditHelper.CollectInfo(datasource.DataGraph, datasource.PrimaryView);
		if (info != null)
		{
			foreach (String field in PX.Data.Process.PXAuditHelper.FIELDS)
			{
				PXTextEdit edit = (PXTextEdit)frmAudit.FindControl("ed" + field);
				if (edit != null)
				{
					Object result = typeof(PX.Data.Process.AUAuditPanelInfo).InvokeMember(field, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, info, null);
					edit.Text = result == null ? null : result.ToString();
				}
			}
		}
		else
		{
			frmAudit.Visible = false;
			((PXLabel)pnlAudit.FindControl("lblWarning")).Visible = true;
		}

		PXSiteMapNode node = PXSiteMap.Provider.FindSiteMapNodeByScreenID("SM205510");
		if (node != null)
		{
			PXButton btnActivate = (PXButton)pnlAuditButtons.FindControl("btnAuditActivate");
			btnActivate.Visible = true;
			btnActivate.NavigateUrl = String.Concat(ResolveUrl(node.Url), "?ScreenId=", this.screenID.Replace(".", ""));
		}
	}

	private void tlbTools_ItemCreate(object sender, PXToolBarItemEventArgs e)
	{
		if (Page.IsCallback)
			return;

		if (e.Item.Key == _TOOLBAR_FILES)
			GetAttachedFiles();
	}

	private void Uploader_FileUploadFinished(object sender, PXFileUploadEventArgs e)
	{
		AttachUploadedFile();
		GetAttachedFiles();
	}

	//---------------------------------------------------------------------------
	protected void tlbTools_CallBack(object sender, PXCallBackEventArgs e)
	{
		if (e.Argument == "refresh")
		{
			if (!PXSiteMap.IsPortal || !Page.Request.Path.ToLower().Contains("reportlauncher"))
			{
				Session.Remove(VirtualPathUtility.ToAbsolute(Page.Request.Path));
				RefreshByRedirect();
			}
		}

		if (e.Argument == "help")
		{
			throw new Exception("Redirect:" + HelpUrl);
		}

		if (e.Command.Name == "updateFileMenu")
		{
			PXToolBarButton btn = this.GetAttachedFiles();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < btn.MenuItems.Count - 3; i++)
			{
				sb.Append(btn.MenuItems[i].Text);
				sb.Append("\t");
				sb.Append(btn.MenuItems[i].NavigateUrl);
				sb.Append("\t");
				sb.Append(btn.MenuItems[i].Target);
				sb.Append("\t");
				sb.Append(btn.MenuItems[i].Value);
				sb.Append(";");
			}
			e.Result = sb.ToString();
		}
		if (e.Command.Name == "auditHistory")
			btnAuditDetails_CallBack(sender, e);
	}
	protected void btnAuditDetails_CallBack(object sender, PXCallBackEventArgs e)
	{
		if (this.Page is PXPage)
		{
			PXDataSource datasource = ((PXPage)this.Page).DefaultDataSource;
			if (datasource != null)
			{
				String key = PX.Data.Process.PXAuditHelper.CollectAudit(datasource.DataGraph, datasource.PrimaryView);
				String message = null;
				if (key != null) message = String.Concat("Redirect4:", ResolveUrl("~/frames/audit.aspx"), "?key=", key, "&preserveSession=true");
				else message = PX.Data.ErrorMessages.AuditNotAvailable;

				throw new Exception(message);
			}
		}
	}

	private void RefreshByRedirect()
	{
		string screen = PXSiteMap.CurrentScreenID ?? ScreenID;
		if (!string.IsNullOrEmpty(screen))
		{
			screen = screen.Replace(".", string.Empty);
			if (screen.Length > 2 && screen.Substring(2) == "000000")
				throw new Exception(string.Concat("Redirect:", ResolveUrl/**/("~/frames/default.aspx"), "?scrid=", screen));
		}
		throw new Exception("Redirect:" + Request.RawUrl);
	}
	#endregion

	#region Method to work with Branches

	/// <summary>
	/// Find the braches drop down control.
	/// </summary>
	private PXDropDown GetBranchCombo()
	{
		PXToolBarContainer cont = (PXToolBarContainer)tlbPath.Items["branch"];
		PXDropDown dropDown = null;
		foreach (Control c in cont.TemplateContainer.Controls)
			if (c.ID == "cmdBranch") { dropDown = c as PXDropDown; break; }
		return dropDown;
	}

	/// <summary>
	/// Fill the branches drop-down list.
	/// </summary>
	private void FillBrachesList()
	{
		PXToolBarContainer cont = (PXToolBarContainer)tlbPath.Items["branch"];
		PXDropDown dropDown = this.GetBranchCombo();
		if (ControlHelper.IsReloadPage(dropDown)) dropDown.CallbackUpdatable = true;

		string currBranchId = null;
		int? currentBranch = PXAccess.GetBranchID();

		List<PXAccess.BranchCollection.Info> items = GetBranches().ToList();
		items.Sort((a, b) => string.Compare(a.Cd, b.Cd));

		dropDown.Items.Clear();
		foreach (PXAccess.BranchCollection.Info item in items)
		{
			var current = (currentBranch != null && currentBranch.Equals(item.Id));
			dropDown.Items.Add(new PXListItem(item.Name, item.Id.ToString()));
			if (current) currBranchId = item.Id.ToString();
		}

		if (PXAccess.FeatureInstalled("PX.Objects.CS.FeaturesSet+Branch") && dropDown.Items.Count > 0)
		{
			if (ControlHelper.IsReloadPage(tlbPath)) tlbPath.CallbackUpdatable = true;
			if (currentBranch != null) dropDown.Value = currBranchId;
			else dropDown.Items.Insert(0, new PXListItem("Select Branch", null));
			dropDown.ItemChanged += Branch_ItemClick;
		}
		else
		{
			cont.Visible = false;
			PXToolBarLabel lbl = tlbPath.Items["branchLabel"] as PXToolBarLabel;
			if (lbl != null) lbl.Visible = false;
			lbl = tlbPath.Items["branchSep"] as PXToolBarLabel;
			if (lbl != null) lbl.Visible = false;
		}
	}

	/// <summary>
	/// Gets the braches enumerator.
	/// </summary>
	private IEnumerable<PXAccess.BranchCollection.Info> GetBranches()
	{
		var currentUser = PXAccess.GetUserName();
		foreach (PXAccess.BranchCollection.Info item in PXAccess.GetBranches(currentUser))
			yield return item;
	}

	/// <summary>
	/// The branch menu item click event handler.
	/// </summary>
	private void Branch_ItemClick(object sender, PXListItemEventArgs e)
	{
		var targetBranch = int.Parse(e.Item.Value);
		if (targetBranch < 1) return;

		PXDropDown dropDown = this.GetBranchCombo();
		foreach (PXAccess.BranchCollection.Info item in GetBranches())
			if (targetBranch.Equals(item.Id))
			{
				PXContext.SetBranchID(targetBranch);
				HttpCookie branchCooky = HttpContext.Current.Response.Cookies["UserBranch"];
				if (dropDown != null) dropDown.Value = item.Id;

				//if (this.Page is PXPage) ((PXPage)this.Page).ResetCurrentUser(HttpContext.Current);

				string branchObj = targetBranch.ToString();
				if (branchCooky == null)
					HttpContext.Current.Response.Cookies.Add(new HttpCookie("UserBranch", branchObj));
				else
					branchCooky.Value = branchObj;
				break;
			}
	}
	#endregion

	#region Methods to work with Favorites

	/// <summary>
	/// The tlbPath callback event handler.
	/// </summary>
	protected void tlbPath_CallBack(object sender, PXCallBackEventArgs e)
	{
		if (e.Command.Name == "AddFav" && PXSiteMap.CurrentNode != null)
		{
			Guid nodeID = PXSiteMap.CurrentNode.NodeID;
			if (!IsInFavorites(nodeID))
				AddFavorite(screenTitle, nodeID);
			else
			{
				PXDatabase.Delete<Favorite>(
						new PXDataFieldRestrict("UserID", PXAccess.GetUserID()),
						new PXDataFieldRestrict("SiteMapID", nodeID)
				);
			}
			PXContext.Session.FavoritesExists["FavoritesExists"] = null;
			PXSiteMap.FavoritesProvider.Clear();

			// check if favorites exists
			using (PXDataRecord exist = PXDatabase.SelectSingle<Favorite>(
					new PXDataField("UserID"), new PXDataFieldValue("UserID", PXAccess.GetUserID())))
			{
				e.Result = (exist == null) ? "0" : "1";
			}
		}
		else if (e.Command.Name == "ClearFilter")
		{
			var ds = PXPage.GetDefaultDataSource(this.Page);
			if (ds != null)
			{
				var action = ds.DataGraph.Actions[clearFilterCommand];
				if (action != null) action.Press();
			}
			PXCallbackManager.RegisterJavaScript(
				"if (window['__px_alls'] && __px_alls(this) && __px_alls(this)['tlbPath'])__px_alls(this)['tlbPath'].items['clearFilter'].setVisible(false);");
		}
	}

	/// <summary>
	/// Check if current node in Favorites.
	/// </summary>
	private bool IsInFavorites(Guid? siteId)
	{
		Guid nodeID = siteId.HasValue ? siteId.Value : PXSiteMap.CurrentNode.NodeID;
		using (PXDataRecord exist = PXDatabase.SelectSingle<Favorite>(
				new PXDataField("SiteMapID"), new PXDataFieldValue("SiteMapID", nodeID),
				new PXDataFieldValue("UserID", PXAccess.GetUserID())))
		{
			return exist != null;
		}
	}

	/// <summary>
	/// Append node with specified tutle to favorites.
	/// </summary>
	private void AddFavorite(string title, Guid? siteId)
	{
		if (String.IsNullOrEmpty(title) && siteId == null) return;

		Guid folderID = PXSiteMap.RootNode.NodeID;
		string neutralTitle;

		using (PXDataRecord record = PXDatabase.SelectSingle<PX.SM.SiteMap>(new PXDataField("Title"),
																																				new PXDataFieldValue("NodeID", siteId)))
		{
			neutralTitle = record.GetString(0);
		}

		if (!string.IsNullOrEmpty(neutralTitle))
		{
			using (PXDataRecord rec_id = PXDatabase.SelectSingle<Favorite>(
					new PXDataField("Max(Position)+1"),
					new PXDataFieldValue("UserID", PXAccess.GetUserID())))
			{
				int pos = 1;
				if (rec_id != null)
				{
					pos = rec_id.GetInt32(0) ?? 1;
				}
				PXDatabase.Insert<Favorite>(
						new PXDataFieldAssign("UserID", PXAccess.GetUserID()),
						new PXDataFieldAssign("NodeID", Guid.NewGuid()),
						new PXDataFieldAssign("Title", neutralTitle),
						new PXDataFieldAssign("ParentID", folderID),
						new PXDataFieldAssign("Position", pos),
						new PXDataFieldAssign("SiteMapID", siteId));
			}
		}
	}
	#endregion

	#region Helper methods

	private void RearrangeAuditMenu()
	{
		PXToolBarButton btn = (PXToolBarButton)tlbTools.Items[_TOOLBAR_HELP];

		if (btn != null && btn.MenuItems != null && btn.MenuItems.Count >= 3 && auditMenuEnabled)
		{
			PXMenuItem auditItem = btn.MenuItems[0];
			btn.MenuItems.RemoveAt(0);
			btn.MenuItems.Insert(btn.MenuItems.Count - 3, auditItem);
		}
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// Initialize the user name button.
	/// </summary>
	private void InitHelpMenu()
	{
		PXToolBarButton btn = (PXToolBarButton)tlbTools.Items[_TOOLBAR_HELP];

		Func<string, string, string, string> func = delegate(string txt, string val, string labelID)
		{
			return string.Format(
					"<div class='size-xs inline-block'>{0}</div> <span id='{1}'>{2}</span>", txt, labelID, val);
		};
		Func<PXMenuItem, PXMenuItem> addItem = delegate(PXMenuItem item) { btn.MenuItems.Add(item); return item; };

		var prefix = "";
		if (Page is PXPage)
		{
			bool isCustomized = ((PXPage)Page).IsPageCustomized;
			if (isCustomized)
			{
				prefix = "CST.";
			}
		}

		var lastItem = addItem(new PXMenuItem(func(btn.Text, prefix + HttpUtility.HtmlEncode(this.screenID), "screenID")));
		lastItem.NavigateUrl = this.HelpUrl;
		lastItem.OpenFrameset = true;
		lastItem = addItem(new PXMenuItem(Msg.GetLink) { CommandSourceID = "tlbDataView", CommandName = "LinkShow" });

		PXMenuItem webItem = null;
		if (!String.IsNullOrEmpty(this.screenID))
		{
			PXSiteMapNode node = PXSiteMap.Provider.FindSiteMapNodeByScreenID(screenID.Replace(".", ""));
			if (WebServicesAvailable && (node != null && !String.IsNullOrEmpty(node.GraphType)
					&& System.Web.Compilation.PXBuildManager.GetType(node.GraphType, false) != null) || (node != null && node.Url.ToLower().Contains("frames/reportlauncher.aspx")) || (node != null && node.Url.ToLower().Contains("frames/rmlauncher.aspx")))
			{
				PXMenuItem item = new PXMenuItem(ActionsMessages.WebService);
				item.NavigateUrl = "~/Soap/" + screenID.Replace(".", "") + ".asmx";
				item.OpenFrameset = false;
				item.Target = "_blank";
				item.ShowSeparator = true;
				btn.MenuItems.Add(webItem = item);
			}
		}
		if (webItem == null) lastItem.ShowSeparator = true;

		if (!String.IsNullOrEmpty(this.screenID) && PXSiteMap.CurrentNode != null)
		{
			PXSiteMapNode node = PXSiteMap.CurrentNode;
			if (PXList.Provider.IsList(screenID))
			{
				string entryScreenID = PXList.Provider.GetEntryScreenID(screenID);
				node = PXSiteMap.Provider.FindSiteMapNodeByScreenID(entryScreenID) ?? node;
			}
			string accessUrl = string.Format("~/Pages/SM/SM201020.aspx?Screen={0}", node.NodeID.ToString());
			addItem(new PXMenuItem("Access Rights...") { NavigateUrl = accessUrl, Target = "_blank" });
		}
		addItem(new PXMenuItem("Trace...") { NavigateUrl = "~/Frames/Trace.aspx?preserveSession=true", Target = "_blank", Value = "trace" });
		addItem(new PXMenuItem("About...") { PopupPanel = "pnlAbout" });
	}

	private void InitAuditMenu()
	{
		auditMenuEnabled = false;
		PXToolBarButton btn = (PXToolBarButton)tlbTools.Items[_TOOLBAR_HELP];

		if (AuditHistoryAvailable && !String.IsNullOrEmpty(this.screenID) && PX.Data.Process.PXAuditHelper.IsUserAuditor)
		{
			PXMenuItem item = null;
			if (PX.Data.PXDatabase.AuditRequired(this.screenID.Replace(".", "")))
			{
				item = new PXMenuItem("Audit History...");
				item.AutoCallBack.Command = "auditHistory";
				item.AutoCallBack.ActiveBehavior = true;
				item.AutoCallBack.Behavior.PostData = PostDataMode.Page;
				item.AutoCallBack.Behavior.Name = "auditHistory";
				item.Value = "auditHistory";
			}
			else if (this.Page is PXPage)
			{
				//Don't initialize Audit History button on wiki page
				if (!(PXSiteMap.CurrentNode is PXWikiMapNode))
				{
					PXDataSource datasource = ((PXPage)this.Page).DefaultDataSource;
					if (datasource != null && PX.Data.Process.PXAuditHelper.IsInfoAvailable(datasource.DataGraph, datasource.PrimaryView))
					{
						PXCache cache = datasource.DataGraph.Views[datasource.PrimaryView].Cache;
						item = new PXMenuItem("Audit History...") { PopupPanel = "pnlAudit" };
					}
				}
			}

			if (item != null)
			{
				btn.MenuItems.Add(item);
				auditMenuEnabled = true;
			}
		}
	}

	/// <summary>
	/// Gets the current version of the system.
	/// </summary>
	private string GetVersion(bool raw)
	{
		string ver = PXVersionInfo.Version;
		if (!raw) ver = PXMessages.LocalizeFormatNoPrefix(PX.AscxControlsMessages.PageTitle.Version, ver);

		string cstProjects = Customization.CstWebsiteStorage.PublishedProjectList;
		if (!string.IsNullOrEmpty(cstProjects))
			ver += " " + PXMessages.LocalizeNoPrefix(PX.AscxControlsMessages.PageTitle.Customization) + cstProjects;
		return ver;
	}

	/// <summary>
	/// Sets the specified control CSS class.
	/// </summary>
	private void SetControlCss(WebControl ctrl, string cssClassName)
	{
		if (string.IsNullOrEmpty(cssClassName)) return;
		ctrl.ControlStyle.Reset();
		ctrl.CssClass = cssClassName;
	}

	//---------------------------------------------------------------------------
	private PXToolBarButton Button(string button)
	{
		foreach (PXToolBarItem item in tlbTools.Items)
			if (item.Key == button) return item as PXToolBarButton;
		return null;
	}

	private bool IsCallback(params string[] paramsEnds)
	{
		if (Page.IsCallback && Request.Params["__CALLBACKID"] != null)
		{
			if (paramsEnds.Length == 0) return true;
			for (int i = 0; i < paramsEnds.Length; i++)
				if (Request.Params["__CALLBACKID"].EndsWith(paramsEnds[i]))
					return true;
		}
		return false;
	}

	public static Guid GetArticleID(string name)
	{
		if (!String.IsNullOrEmpty(name))
		{
			return PXSiteMap.WikiProvider.GetWikiPageIDByPageName(name);
		}
		return Guid.Empty;
	}

	private string HelpUrl
	{
		get
		{
			string help = WebConfigurationManager.AppSettings[helpKey] ?? helpUrl;
			string url;
            if (Page.GetType().FullName == "ASP.wiki_showwiki_aspx")
            {
                url = string.Format(help, Guid.Empty.ToString());
                PreferencesGeneral row = PXSelect<PreferencesGeneral>.Select(new PXGraph());
                if (row != null)
                {
                    if(row.HelpPage != null)
                        url = string.Format(help, row.HelpPage.ToString());
                }                                
            }
            
			else
			{
				// Replace Help link for the List screen by the link from the Entry Screen
				string actualScreenID = ScreenID;
				PXSiteMapNode node = PXSiteMap.CurrentNode;

                string pageName = actualScreenID.With(_ => _.Replace('.', '_').ToString());
                url = string.Format(help, GetArticleID(pageName));
                WikiPage getPage = PXSelect<WikiPage,
                    Where<WikiPage.name, Equal<Required<WikiPage.name>>>>.Select(new PXGraph(), pageName);

                if (getPage == null)
                {
                    if (node != null && PXList.Provider.IsList(node.ScreenID))
                    {
                        string entryScreenID = PXList.Provider.GetEntryScreenID(node.ScreenID);
                        if (!String.IsNullOrEmpty(entryScreenID))
                            actualScreenID = Mask.Format("CC.CC.CC.CC", entryScreenID);
                        url = string.Format(help, GetArticleID(actualScreenID.With(_ => _.Replace('.', '_').ToString())));
                    }
                    else
                    {
                        if (getPage == null)
                        {
                            if (node.Url.LastIndexOf("/") > -1)
                            {
                                string newpageName = node.Url.Substring(node.Url.LastIndexOf("/"), 9);
                                if (newpageName.Length == 9)
                                {
                                    pageName = (newpageName[1].ToString() + newpageName[2].ToString() + '_' +
                                        newpageName[3].ToString() + newpageName[4].ToString() + '_' +
                                        newpageName[5].ToString() + newpageName[6].ToString() + '_' +
                                        newpageName[7].ToString() + newpageName[8].ToString());
                                    pageName = pageName.ToUpper();
                                    url = string.Format(help, GetArticleID(pageName));
                                }
                            }
                        }
                    }
                }
			}
			return HttpUtility.UrlPathEncode(url);
		}
	}

	private void AttachUploadedFile()
	{
		UploadFileMaintenance filesAccessor = PXGraph.CreateInstance<UploadFileMaintenance>();
		if (Uploader.UploadedFile != null && Uploader.UploadedFile.UID != null &&
				!string.IsNullOrEmpty(PXSiteMap.CurrentScreenID))
		{
			filesAccessor.AttachToScreen(Uploader.UploadedFile.UID.Value, PXSiteMap.CurrentScreenID);
		}
	}

	//---------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	private PXToolBarButton GetAttachedFiles()
	{
		PXToolBarButton btn = Button(_TOOLBAR_FILES);
		string url = Page.AppRelativeVirtualPath;

		btn.MenuItems.Clear();
		if (string.IsNullOrEmpty(url))
		{
			btn.Visible = false;
			return btn;
		}

		UploadFileMaintenance filesAccessor = PXGraph.CreateInstance<UploadFileMaintenance>();
		foreach (string filename in filesAccessor.GetFileNamesAttachedToScreen(PXSiteMap.CurrentScreenID))
		{
			PXMenuItem item = new PXMenuItem(FileInfo.GetShortName(filename));
			item.NavigateUrl = ResolveUrl("~/Frames/GetFile.ashx") + "?file=" + HttpUtility.UrlEncode(filename);
			item.Target = "_blank";
			item.RenderLink = false;
			item.Style.CssClass = "MenuItem";
			item.Value = ResolveUrl("~/Pages/SM/SM202510.aspx") + "?fileID=" + HttpUtility.UrlEncode(filename);
			btn.MenuItems.Add(item);
		}

		PXMenuItem mi = new PXMenuItem("Attach file...");
		mi.ShowSeparator = true;
		mi.PopupPanel = "Uploader";
		mi.Value = "notfilelink";
		btn.MenuItems.Add(mi);


		String[] importScenarios = PX.Api.SYImportMaint.GetAvailableMappings(this.ScreenID);
		String[] exportScenarios = PX.Api.SYExportMaint.GetAvailableMappings(this.ScreenID);
		if (importScenarios.Length > 0)
		{
			mi = new PXMenuItem("Import Scenarios");
			mi.NavigateUrl = "~/pages/sm/SM206036.aspx";
			foreach (String scenario in importScenarios)
			{
				PXMenuItem submi = new PXMenuItem(scenario);
				submi.NavigateUrl = ResolveUrl("~/Main.aspx?ScreenId=SM206036&Name=") + HttpUtility.UrlEncode(scenario);
				submi.Target = "_blank";
				submi.RenderLink = false;
				mi.ChildItems.Add(submi);
			}
			if (exportScenarios.Length <= 0) mi.ShowSeparator = true;
			btn.MenuItems.Add(mi);
		}
		if (exportScenarios.Length > 0)
		{
			mi = new PXMenuItem("Export Scenarios");
			mi.NavigateUrl = "~/pages/sm/SM207036.aspx";
			foreach (String scenario in exportScenarios)
			{
				PXMenuItem submi = new PXMenuItem(scenario);
				submi.NavigateUrl = ResolveUrl("~/Main.aspx?ScreenId=SM207036&Name=") + HttpUtility.UrlEncode(scenario);
				submi.Target = "_blank";
				submi.RenderLink = false;
				mi.ChildItems.Add(submi);
			}
			mi.ShowSeparator = true;
			btn.MenuItems.Add(mi);
		}

		if (!this.Page.IsCallback)
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "filesMenuIndex", "var __filesMenuIndex=" + tlbTools.Items.IndexOf(Button(_TOOLBAR_FILES)).ToString() + ";", true);
		return btn;
	}
	#endregion

	#region Methods to register script

	private void RegisterSyncTreeVars()
	{
		//var screen = string.Format("{0}{1}", Customization.WebsiteEntryPoints.GetCustomizationMarker(), screenID);
		Page.ClientScript.RegisterClientScriptBlock(GetType(), "screenID",
			"var __screenID=\"" + HttpUtility.HtmlEncode(screenID) + "\";", true);
	}

	private void RegisterModules()
	{
		IScriptRenderer scriptRenderer = JSManager.GetRenderer(this.Page);
		JSManager.RegisterSystemModules(this.Page);
		JSManager.RegisterCallbackModules(this.Page);
		JSManager.RegisterModule(scriptRenderer, typeof(PXTextEdit), JS.TextEdit);
		JSManager.RegisterModule(scriptRenderer, typeof(PXCheckBox), JS.CheckBox);
		JSManager.RegisterModule(scriptRenderer, typeof(PXDropDown), JS.DropDown);
	}
	#endregion

	#region Variables

	private const string _TOOLBAR_REFRESH = "keyBtnRefresh";
	private const string _TOOLBAR_FILES = "files";
	private const string _TOOLBAR_HELP = "help";
	private const string helpUrl = "~/Wiki/Show.aspx?pageid={0}";
	private const string helpKey = "helpUrl";
	private const string clearFilterCommand = "__ResetListNavigation";

	private bool auditMenuEnabled;
	private bool customizationAvalable = true;
	private bool favoriteAvalable = true;
	private bool webServicesAvailable = true;
	private bool auditHistoryAvailable = true;
	private string screenImage, screenTitle, screenID, userName;
	private string company = null;
	private HtmlAnchor labelScreen = null;

	#endregion

	#region Customization

	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		if (customizationAvalable && this.Visible)
		{
			Customization.WebsiteEntryPoints.AddCustomizationMenuAndDialogs(CustomizationContainer);
		}
	}
	#endregion

	#region ITitleModuleController

	void ITitleModuleController.AppendControl(Control control)
	{
		Page.Form.Controls.Add(control);
	}

	void ITitleModuleController.AppendToolbarItem(PXToolBarItem item)
	{
		tlbTools.Items.Insert(1, item);
		if (item.Key == "reminder") item.Visible = false;
		if (PXSiteMap.IsPortal)
		{
			if (!System.Web.Security.Roles.IsUserInRole(PXAccess.GetAdministratorRoles().First()))
			{
				item.Visible = false;
			}
		}
	}

	Page ITitleModuleController.Page
	{
		get { return Page; }
	}

	private void InitializeModules()
	{
		List<ITitleModule> list = new List<ITitleModule>(TitleModuleService.Handlers);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			ITitleModule module = list[i];
			module.Initialize(this);
		}
	}


	protected void tlbDataView_DataBound(object sender, EventArgs e)
	{
	}
	#endregion

	#region Filters in Entry Screens

	private string GetListFilterName()
	{
		string listScreenID = PXList.Provider.GetListID(PXSiteMap.CurrentNode.ScreenID);
		if (listScreenID != null) // check if you need to add the control at all
		{
			PXFilterTuple filters = PXList.Provider.GetListFilters(PXSiteMap.CurrentNode.ScreenID, PXList.Provider.GetListView(listScreenID));
			if (filters != null)
			{
				if (filters.FilterID != null)
				{
					string dynamicFilterName = DynamicFilterManager.GetFilterName((long) filters.FilterID);
					if (!String.IsNullOrEmpty(dynamicFilterName))
						return dynamicFilterName;
					FilterHeader filter = FilterHeader.Definition.Get().FirstOrDefault(f => f.FilterID == filters.FilterID);
					if (filter != null)
						return filter.FilterName;
				}
				else if (filters.FilterRows != null)
				{
					return PXMessages.LocalizeNoPrefix(Msg.GrdFilterApplied);
				}
			}
		}
		return null;
	}

	#endregion
}
