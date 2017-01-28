using System;
using PX.Data;
using PX.Objects.EP;
using PX.SM;

namespace PX.Objects.CS.Email
{
	[Serializable]
	[PXPrimaryGraph(typeof(EmailsSyncMaint))]
	[PXHidden]
	public class EMailAccountSyncFilter : IBqlTable
	{
		#region ServerID
		public abstract class serverID : IBqlField { }
		[PXInt()]
		[PXUIField(DisplayName = "Exchange Server")]
		[PXSelector(typeof(Search<EMailSyncServer.accountID>), typeof(EMailSyncServer.accountCD), typeof(EMailSyncServer.address), typeof(EMailSyncServer.defaultPolicyName), 
			SubstituteKey = typeof(EMailSyncServer.accountCD), ValidateValue = false)]
		public virtual Int32? ServerID { get; set; }
		#endregion
		#region PolicyName
		public abstract class policyName : PX.Data.IBqlField
		{
		}
		[PXString(255, InputMask = "")]
		[PXUIField(DisplayName = "Policy Name")]
		[PXSelector(typeof(Search<EMailSyncPolicy.policyName>),
			DescriptionField = typeof(EMailSyncPolicy.description), ValidateValue = false)]
		public virtual String PolicyName { get; set; }
		#endregion
	}


	#region Lite DACs

	[PXTable(typeof(BAccount.bAccountID))]
	[PXCacheName(Messages.Employee)]
	[PXPrimaryGraph(
		new Type[] { typeof(EmployeeMaint) }, 
		new Type[]
		{
			typeof(Select<EP.EPEmployee,
				Where<EP.EPEmployee.bAccountID, Equal<Current<EPEmployee.bAccountID>>>>)
        }
	)]
	[PXHidden]
	public sealed class EPEmployee : BAccount
	{
		#region BAccountID
		public new abstract class bAccountID : IBqlField { }
		#endregion
		#region ParentBAccountID
		public new abstract class parentBAccountID : IBqlField { }
		#endregion
		#region DefContactID
		public new abstract class defContactID : IBqlField { }
		#endregion
		#region AcctName
		public new abstract class acctName : IBqlField { }
		#endregion
		#region AcctCD
		public abstract new class acctCD : IBqlField { }

		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Employee ID", Visibility = PXUIVisibility.SelectorVisible)]
		public override String AcctCD { get; set; }
		#endregion
		#region CalendarID
		public abstract class calendarID : IBqlField { }

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Calendar", Visibility = PXUIVisibility.SelectorVisible)]
		public String CalendarID { get; set; }
		#endregion
		#region UserID
		public abstract class userID : IBqlField { }
		[PXDBGuid]
		[PXUIField(DisplayName = "Employee Login", Visibility = PXUIVisibility.Visible)]
		public Guid? UserID { get; set; }
		#endregion
	}

	[PXCacheName(CR.Messages.BAccount)]
	[PXHidden]
	public class BAccount : IBqlTable
	{
		#region BAccountID
		public abstract class bAccountID : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public Int32? BAccountID { get; set; }
		#endregion
		#region ParentBAccountID
		public abstract class parentBAccountID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Parent Account", Visibility = PXUIVisibility.SelectorVisible)]
		public Int32? ParentBAccountID { get; set; }
		#endregion
		#region DefContactID
		public abstract class defContactID : IBqlField { }

		[PXDBInt]
		public Int32? DefContactID { get; set; }
		#endregion
		#region AcctName
		public abstract class acctName : IBqlField { }

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Account Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AcctName { get; set; }
		#endregion
		#region AcctCD
		public abstract class acctCD : IBqlField { }

		[PXDBString(30, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Account ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AcctCD { get; set; }
		#endregion
	}
	
	#endregion

}
