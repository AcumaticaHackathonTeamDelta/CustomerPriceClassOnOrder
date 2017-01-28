using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.SM;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.CS;

namespace PX.Objects.SO
{
	[PXProjection(typeof(Select<NotificationSetup,
		Where<NotificationSetup.module, Equal<PXModule.so>>>), Persistent = true)]
    [Serializable]
	public partial class SONotification : NotificationSetup
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
		[PXDefault(PXModule.SO)]
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
		[PXDefault(SONotificationSource.Customer)]
		[PXDBString(10, InputMask = ">aaaaaaaaaa")]
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
		[PXUIField(DisplayName = "Report ID")]
		[PXSelector(typeof(Search<SiteMap.screenID,
			Where<SiteMap.screenID, Like<PXModule.so_>, And<SiteMap.url, Like<urlReports>>>,
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

	public class SONotificationSource
	{
		public const string Customer = "Customer";
		public class customer : Constant<string> { public customer() : base(Customer) { } }
	}
}
