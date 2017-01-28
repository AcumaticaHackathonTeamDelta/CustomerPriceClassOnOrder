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

namespace PX.Objects.AR
{	
	[PXProjection(typeof(Select<NotificationSetup,
		Where<NotificationSetup.module, Equal<PXModule.ar>>>), Persistent = true)]
    [Serializable]
	[PXCacheName(Messages.ARNotification)]
	public partial class ARNotification : NotificationSetup
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
		[PXDefault(PXModule.AR)]
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
		[PXDefault(ARNotificationSource.Customer)]
		[PXDBString(10, IsKey = true, InputMask = "")]
		[PXCheckUnique]
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
		[PXDBString(30, IsKey = true, InputMask = "", IsUnicode = true)]
		[PXUIField(DisplayName = "Mailing ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXCheckUnique(typeof(NotificationSetup.module), typeof(NotificationSetup.sourceCD))]
		[PXDefault]
		public override string NotificationCD
		{
			get
			{
				return _NotificationCD;
			}
			set
			{
				_NotificationCD = value;
			}
		}
		#endregion
		#region ReportID
		public new abstract class reportID : PX.Data.IBqlField
		{
		}
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report")]
		[PXSelector(typeof(Search<SiteMap.screenID,
			Where<SiteMap.screenID, Like<PXModule.ar_>, And<SiteMap.url, Like<urlReports>>>,
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

	public class ARNotificationSource
	{
		public const string Customer = "Customer";
		public class customer : Constant<string> { public customer() : base(Customer) { } }
	}
}
