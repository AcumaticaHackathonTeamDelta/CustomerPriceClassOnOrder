using System;
using PX.Data;

namespace PX.Objects.CT
{
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ContractRenewalHistory)]
	public partial class ContractRenewalHistory : PX.Data.IBqlTable
	{
		#region ContractID
		public abstract class contractID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContractID;
		[PXParent(typeof(Select<Contract, Where<Contract.contractID, Equal<Current<ContractRenewalHistory.contractID>>>>))]
		[PXDBDefault(typeof(Contract.contractID))]
		[PXDBInt(IsKey = true)]
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
		#region RevID
		public abstract class revID : PX.Data.IBqlField
		{
		}
		[PXDBInt(MinValue = 1, IsKey = true)]
		[PXDefault(typeof(Contract.revID), PersistingCheck = PXPersistingCheck.Null)]
		public virtual int? RevID { get; set; }
		#endregion
		#region RenewalDate
		public abstract class renewalDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _RenewalDate;
		[PXDBDate]
		[PXDefault]
		public virtual DateTime? RenewalDate
		{
			get
			{
				return this._RenewalDate;
			}
			set
			{
				this._RenewalDate = value;
			}
		}
		#endregion
		#region ActionBusinessDate
		public abstract class actionBusinessDate : IBqlField {}
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? ActionBusinessDate { get; set; }
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[Contract.status.List]
		[PXDefault(Contract.status.Draft)]
		[PXUIField(DisplayName = "Status", Required = true, Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region Action
		public abstract class action : PX.Data.IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[ContractAction.List]
		[PXDefault(ContractAction.Create)]
        [PXUIField(DisplayName = "Action")]
		public virtual String Action{ get; set; }
		#endregion

        #region ChildContract
        public abstract class childContractID : PX.Data.IBqlField
        {
        }

        protected Int32? _ChildContractID;
        [PXDBInt]
        [PXSelector(typeof(Contract.contractID), SubstituteKey = typeof(Contract.contractCD))]
        [PXUIField(DisplayName = "Related Contract")]
        public virtual Int32? ChildContractID
        {
            get
            {
                return this._ChildContractID;
            }
            set
            {
                this._ChildContractID = value;
            }
        }
        #endregion ChildContract
        #region ExpireDate
        public abstract class expireDate : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? ExpireDate { get; set; }
        #endregion
        #region EffectiveFrom
        public abstract class effectiveFrom : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? EffectiveFrom { get; set; }
        #endregion
        #region ActivationDate
        public abstract class activationDate : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? ActivationDate { get; set; }
        #endregion
        #region StartDate
        public abstract class startDate : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? StartDate { get; set; }
        #endregion
        #region NextDate
        public abstract class nextDate : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? NextDate { get; set; }
        #endregion
        #region LastDate
        public abstract class lastDate : PX.Data.IBqlField
        {
        }

        [PXDBDate()]
        public virtual DateTime? LastDate { get; set; }
        #endregion
        #region StartBilling
        public abstract class startBilling : PX.Data.IBqlField
        {
        }

        [PXDBDate]
        public virtual DateTime? StartBilling { get; set; }
        #endregion
        #region TerminationDate
        public abstract class terminationDate : PX.Data.IBqlField {}

        [PXDBDate]
        public virtual DateTime? TerminationDate { get; set; }
        #endregion
        #region IsActive
        public abstract class isActive : PX.Data.IBqlField
        {
        }

        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? IsActive { get; set; }
        #endregion
        #region IsCompleted
        public abstract class isCompleted : PX.Data.IBqlField
        {
        }

        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? IsCompleted { get; set; }
        #endregion
        #region IsCancelled
        public abstract class isCancelled : PX.Data.IBqlField
        {
        }

        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? IsCancelled { get; set; }
        #endregion
        #region IsPendingUpdate
        public abstract class isPendingUpdate : PX.Data.IBqlField
        {
        }

        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? IsPendingUpdate { get; set; }
        #endregion

		#region Date
		public abstract class date : IBqlField {}
		[PXDate]
		[PXUIField(DisplayName = "Date")]
		[PXFormula(typeof(Switch<
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.setup>>, ContractRenewalHistory.startDate,
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.activate>,
				Or<ContractRenewalHistory.action, Equal<ContractAction.setupAndActivate>>>, 
				Switch<Case<Where<ContractRenewalHistory.effectiveFrom, Greater<ContractRenewalHistory.activationDate>>, 
					ContractRenewalHistory.effectiveFrom>, 
					ContractRenewalHistory.activationDate>,
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.bill>>, ContractRenewalHistory.lastDate,
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.renew>,
				Or<action, Equal<ContractAction.create>>>, ContractRenewalHistory.renewalDate,
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.terminate>>, ContractRenewalHistory.terminationDate,
			Case<Where<ContractRenewalHistory.action, Equal<ContractAction.upgrade>>, ContractRenewalHistory.actionBusinessDate
			>>>>>>>))]
		public virtual DateTime? Date { get; set; }
		#endregion

        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }

        protected String _DiscountID;
        [PXDBString(10, IsUnicode = true)]
        public virtual String DiscountID
        {
            get
            {
                return this._DiscountID;
            }
            set
            {
                this._DiscountID = value;
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
		[PXDBCreatedByID(DisplayName = "User")]
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
		[PXUIField(DisplayName = "Modified Time")]
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

	public static class ContractAction
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
                    new string[] { Create, Activate, Bill, Renew, Terminate, Upgrade, Setup, SetupAndActivate },
                    new string[] { ActionMessages.Create, ActionMessages.Activate, ActionMessages.Bill, ActionMessages.Renew, ActionMessages.Terminate, ActionMessages.Upgrade, ActionMessages.Setup, ActionMessages.SetupAndActivate })
			{
			}
		}
		public const string Create = "N";
		public const string Activate = "A";
		public const string Bill = "B";
		public const string Renew = "R";
		public const string Terminate = "T";
		public const string Upgrade = "U";
		public const string Setup = "S";
		public const string SetupAndActivate = "M";

		public class create : Constant<string>
		{
			public create()
				: base(ContractAction.Create)
			{
			}
		}
		public class activate : Constant<string>
		{
			public activate()
				: base(ContractAction.Activate)
			{
			}
		}
		public class bill : Constant<string>
		{
			public bill()
				: base(ContractAction.Bill)
			{
			}
		}
		public class renew : Constant<string>
		{
			public renew()
				: base(ContractAction.Renew)
			{
			}
		}
		public class terminate : Constant<string>
		{
			public terminate()
				: base(ContractAction.Terminate)
			{
			}
		}

		public class upgrade : Constant<string>
		{
			public upgrade()
				: base(ContractAction.Upgrade)
			{
			}
		}

		public class setup : Constant<string>
		{
			public setup()
				: base(ContractAction.Setup)
			{
			}
		}

		public class setupAndActivate : Constant<string>
		{
			public setupAndActivate()
				: base(ContractAction.SetupAndActivate)
			{
			}
		}
	}
}
