using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using PX.Common;
using PX.Data;
using PX.SM;
using PX.Web.UI;

namespace PX.Data.Wiki.Parser
{
	/// <summary>
	/// Defines settings for all wiki pages in Pure project.
	/// </summary>
	public static class PXWikiSettingsRelative
	{
		public static PXSettings GetSettings(Page page)
		{
			PXSettings settings = new PXSettings
			{
				NamedLinks = false,
				EditLinkText = PXMessages.LocalizeNoPrefix(Messages.Edit),
				CloseLinkText = PXMessages.LocalizeNoPrefix(Messages.Close),
				DefaultStylesPath = page.ResolveUrl /**/("~/App_Themes/Wiki.css"),
				GetCSSUrl = page.ResolveUrl /**/("~/App_Themes/GetCSS.aspx"),
				ArticleShowUrl = page.ResolveUrl /**/("~/Wiki/ShowWiki.aspx"),
				GetFileUrl = page.ResolveUrl /**/("~/Frames/GetFile.ashx"),
				FileEditUrl = page.ResolveUrl /**/("~/Pages/SM/SM202510.aspx"),
				GetRSSUrl = page.ResolveUrl /**/("~/Frames/GetRSS.ashx"),
				HintImageUrl = PXImages.ResolveImageUrl("~/App_Themes/Default/Images/Wiki/info.png"),
				WarnImageUrl = PXImages.ResolveImageUrl("~/App_Themes/Default/Images/Wiki/Warn.png"),
				MagnifyImageUrl = PXImages.ResolveImageUrl("~/App_Themes/Default/Images/Wiki/magnify.png"),
				DefaultExtensionImage = PXImages.ResolveImageUrl("~/App_Themes/Default/Images/Wiki/binary.gif"),
				RSSImageUrl = PXImages.ResolveImageUrl("~/App_Themes/Default/Images/Wiki/rss.gif"),
				FilesDirectAccess = true
			};

			foreach (UploadAllowedFileTypes ext in SitePolicy.AllowedFileTypes)
				if (!string.IsNullOrEmpty(ext.IconUrl) && !settings.ExtensionsImages.ContainsKey(ext.FileExt.ToLower()))
					settings.ExtensionsImages.Add(ext.FileExt.ToLower(), page.ResolveUrl/**/(ext.IconUrl));
			return settings;
		}
	}
}


