using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.SM;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.CS;

namespace PX.Objects.AP
{
	[PXProjection(typeof(Select<NotificationSetup,
		Where<NotificationSetup.module, Equal<PXModule.ap>>>), Persistent = true)]
    [Serializable]
	[PXCacheName(Messages.APNotification)]
	public partial class APNotification : NotificationSetup
	{
		#region SetupID
		public new abstract class setupID : PX.Data.IBqlField
		{
		}
		#endregion
		#region Module
		public new abstract class module : PX.Data.IBqlField
		{
		}
		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXDefault(PXModule.AP)]
		public override string Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region SourceCD
		public new abstract class sourceCD : PX.Data.IBqlField
		{
		}
		[PXDefault(APNotificationSource.Vendor)]
		[PXDBString(10, IsKey = true, InputMask = ">aaaaaaaaaa")]
		public override string SourceCD
		{
			get
			{
				return this._SourceCD;
			}
			set
			{
				this._SourceCD = value;
			}
		}
		#endregion
		#region NotificationCD
		public new abstract class notificationCD : PX.Data.IBqlField
		{
		}
		#endregion
		#region ReportID
		public new abstract class reportID : PX.Data.IBqlField
		{
		}
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report")]
		[PXSelector(typeof(Search<SiteMap.screenID,
			Where<SiteMap.screenID, Like<PXModule.ap_>, And<SiteMap.url, Like<urlReports>>>,
			OrderBy<Asc<SiteMap.screenID>>>), typeof(SiteMap.screenID), typeof(SiteMap.title),
			Headers = new string[] { CA.Messages.ReportID, CA.Messages.ReportName },
			DescriptionField = typeof(SiteMap.title))]
		public override String ReportID
		{
			get
			{
				return this._ReportID;
			}
			set
			{
				this._ReportID = value;
			}
		}
		#endregion
		#region TemplateID
		public abstract class templateID : PX.Data.IBqlField
		{
		}
		#endregion
		#region Active
		public new abstract class active : PX.Data.IBqlField
		{
		}
		#endregion
	}

	public class APNotificationSource
	{
		public const string Vendor = "Vendor";
		public class vendor : Constant<string> { public vendor() : base(Vendor) { } }
	}
}
