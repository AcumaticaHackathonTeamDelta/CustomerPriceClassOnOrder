using System;
using PX.Data;

namespace PX.SM
{
	[Serializable]
	public partial class ReportUsers : Users
	{
		#region Username

		public new abstract class username : IBqlField { }

		#endregion

		#region FriendlyName

		public abstract class friendlyName : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Username")]
		[SMReportSubstitute(typeof(ReportUsers.username), typeof(ReportUsers.username))]
		public virtual String FriendlyName { get; set; }

		#endregion
	}
}