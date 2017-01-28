using System;
using PX.Data;

namespace PX.Objects.CR.DAC
{
	[PXCacheName(Messages.CommunacationClass)]
    [Serializable]
	public partial class CommunicationSummary : IBqlTable
	{
		#region UserID
		public abstract class userID : IBqlField { }
		[PXUIField(DisplayName = "UserID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(250, IsUnicode = true)]
		public virtual String UserID { get; set; }
		#endregion
		
		#region Emails
		public abstract class emails : IBqlField { }
		[PXUIField(DisplayName = "Emails", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(250, IsUnicode = true)]
		public virtual String Emails { get; set; }
		#endregion

		#region Tasks
		public abstract class tasks : IBqlField { }
		[PXUIField(DisplayName = "Tasks", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(250, IsUnicode = true)]
		public virtual String Tasks { get; set; }
		#endregion

		#region Events
		public abstract class events : IBqlField { }
		[PXUIField(DisplayName = "Events", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(250, IsUnicode = true)]
		public virtual String Events { get; set; }
		#endregion

		#region Approvals
		public abstract class approvals : IBqlField { }
		[PXUIField(DisplayName = "", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(250, IsUnicode = true)]
		public virtual String Approvals { get; set; }
		#endregion

		#region Announcements
		public abstract class announcements : IBqlField { }
		[PXUIField(DisplayName = "Announcements", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(250, IsUnicode = true)]
		public virtual String Announcements { get; set; }
		#endregion
	}
}
