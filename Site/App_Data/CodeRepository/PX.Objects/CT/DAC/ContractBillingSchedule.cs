using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.FA;

namespace PX.Objects.CT
{
	[PXPrimaryGraph(typeof(ContractMaint))]
	[Serializable]
	[PXCacheName(Messages.ContractBillingSchedule)]
	public partial class ContractBillingSchedule : IBqlTable
	{
		#region ContractID
		public abstract class contractID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContractID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(Contract.contractID))]
		[PXParent(typeof(Select<Contract, Where<Contract.contractID, Equal<Current<ContractBillingSchedule.contractID>>>>))]
		public virtual Int32? ContractID
		{
			get
			{
				return this._ContractID;
			}
			set
			{
				this._ContractID = value;
			}
		}
		#endregion
		#region Type
		public abstract class type : IBqlField {}
		protected String _Type;
		[PXDefault(BillingType.Monthly)]
		[PXDBString(1, IsFixed = true)]
		[BillingType.ListForContract]
		[PXUIField(DisplayName = "Billing Period", Required=true)]
		public virtual String Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}
		#endregion
		#region NextDate
		public abstract class nextDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _NextDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Next Billing Date", Visibility=PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual DateTime? NextDate
		{
			get
			{
				return this._NextDate;
			}
			set
			{
				this._NextDate = value;
			}
		}
		#endregion
		#region LastDate
		public abstract class lastDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Billing Date", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual DateTime? LastDate
		{
			get
			{
				return this._LastDate;
			}
			set
			{
				this._LastDate = value;
			}
		}
		#endregion
		#region BillTo
		public abstract class billTo : IBqlField
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute() 
					: base(
						new string[] { ParentAccount, CustomerAccount, SpecificAccount },
						new string[] { Messages.ParentAccount, Messages.CustomerAccount, Messages.SpecificAccount }) {}
			}

			public const string ParentAccount = "P";
			public const string CustomerAccount = "M";
			public const string SpecificAccount = "S";

			public class parentAccount : Constant<string>
			{
				public parentAccount() : base(billTo.ParentAccount) { ;}
			}
			public class customerAccount : Constant<string>
			{
				public customerAccount() : base(billTo.CustomerAccount) { ;}
			}
			public class specificAccount : Constant<string>
			{
				public specificAccount() : base(billTo.SpecificAccount) { ;}
			}
		}
		protected String _BillTo;
		[PXDefault(billTo.CustomerAccount, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(1, IsFixed = true)]
		[billTo.List]
		[PXUIField(DisplayName = "Bill To")]
		public virtual String BillTo
		{
			get
			{
				return this._BillTo;
			}
			set
			{
				this._BillTo = value;
			}
		}
		#endregion
		#region StartBilling
		public abstract class startBilling : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartBilling;
		[PXDBDate]
		[PXUIField(DisplayName = "Billing Schedule Starts On", Enabled=false)]
		public virtual DateTime? StartBilling
		{
			get
			{
				return this._StartBilling;
			}
			set
			{
				this._StartBilling = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[CustomerActive(DescriptionField = typeof(Customer.acctName), Visibility=PXUIVisibility.SelectorVisible, DisplayName="Account")]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[PXDefault(typeof(Search<BAccount.defLocationID, Where<BAccount.bAccountID, Equal<Current<ContractBillingSchedule.accountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[CS.LocationID(typeof(Where<Location.bAccountID, Equal<Current<ContractBillingSchedule.accountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr))]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion

		#region System Columns
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#endregion
	}

	public static class BillingType
	{


		public class ListForProjectAttribute : PXStringListAttribute
		{
			public ListForProjectAttribute()
				: base(
				new string[] { Weekly, Monthly, Quarterly, Annual, OnDemand },
				new string[] { Messages.Weekly, Messages.Monthly, Messages.Quarterly, Messages.Annual, Messages.OnDemand }) { ; }
		 }

		public class ListForContractAttribute : PXStringListAttribute
		{
			public ListForContractAttribute()
				: base(
				new string[] { Weekly, Monthly, Quarterly, SemiAnnual, Annual, OnDemand, Statement },
				new string[] { Messages.Weekly, Messages.Monthly, Messages.Quarterly, Messages.SemiAnnual, Messages.Annual, Messages.OnDemand, Messages.StatementBased }) { ; }
		}
		public const string Statement = "S";
		public const string Quarterly = "Q";
		public const string Monthly = "M";
		public const string Annual = "A";
		public const string SemiAnnual = "6";
		public const string Weekly = "W";
		public const string OnDemand = "D";

		public class BillingStatement : Constant<string>
		{
			public BillingStatement() : base(BillingType.Statement) { ;}
		}
		public class BillingQuarterly : Constant<string>
		{
			public BillingQuarterly() : base(BillingType.Quarterly) { ;}
		}
		public class BillingMonthly : Constant<string>
		{
			public BillingMonthly() : base(BillingType.Monthly) { ;}
		}
		public class BillingAnnual : Constant<string>
		{
			public BillingAnnual() : base(BillingType.Annual) { ;}
		}
		public class BillingSemiAnnual : Constant<string>
		{
			public BillingSemiAnnual() : base(BillingType.SemiAnnual) { ;}
		}
		public class BillingWeekly : Constant<string>
		{
			public BillingWeekly() : base(BillingType.Weekly) { ;}
		}
		public class BillingOnDemand : Constant<string>
		{
			public BillingOnDemand() : base(BillingType.OnDemand) { ;}
		}
	}
}
