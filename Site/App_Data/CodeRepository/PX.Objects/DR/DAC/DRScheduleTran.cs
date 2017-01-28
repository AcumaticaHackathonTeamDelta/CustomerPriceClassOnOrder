using System;
using System.Diagnostics;

using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;

namespace PX.Objects.DR
{
	[Serializable]
	[DebuggerDisplay("SheduleID={ScheduleID} LineNbr={LineNbr} Amount={Amount} RecDate={RecDate}")]
	[PXCacheName(Messages.DRScheduleTran)]
	public partial class DRScheduleTran : PX.Data.IBqlTable
	{
		#region ScheduleID
		public abstract class scheduleID : PX.Data.IBqlField
		{
		}
		protected Int32? _ScheduleID;
		[PXParent(typeof(Select<DRScheduleDetail, Where<DRScheduleDetail.scheduleID, Equal<Current<DRScheduleTran.scheduleID>>, And<DRScheduleDetail.componentID, Equal<Current<DRScheduleTran.componentID>>>>>))]
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(DRScheduleDetail.scheduleID))]
		[PXUIField(DisplayName = "Schedule ID", Enabled = false, Visibility=PXUIVisibility.SelectorVisible)]
		//[PXSelector(typeof(DRSchedule.scheduleID))]
		public virtual Int32? ScheduleID
		{
			get
			{
				return this._ScheduleID;
			}
			set
			{
				this._ScheduleID = value;
			}
		}
		#endregion
		#region ComponentID
		public abstract class componentID : PX.Data.IBqlField
		{
		}
		protected Int32? _ComponentID;
		[PXDBLiteDefault(typeof(DRScheduleDetail.componentID))]
		[PXDBInt(IsKey=true)]
		public virtual Int32? ComponentID
		{
			get
			{
				return this._ComponentID;
			}
			set
			{
				this._ComponentID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXLineNbr(typeof(DRScheduleDetail.lineCntr))]
		[PXUIField(DisplayName = Messages.TransactionNumber, Enabled = false)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch()]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(DRScheduleTranStatus.Open)]
		[DRScheduleTranStatus.List]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
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
		#region RecDate
		public abstract class recDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _RecDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Rec. Date")]
		public virtual DateTime? RecDate
		{
			get
			{
				return this._RecDate;
			}
			set
			{
				this._RecDate = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Tran. Date", Enabled=false)]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Amount")]
		public virtual Decimal? Amount
		{
			get
			{
				return this._Amount;
			}
			set
			{
				this._Amount = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDefault(typeof(DRScheduleDetail.accountID))]
		[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
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
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[PXDefault(typeof(DRScheduleDetail.subID))]
		[SubAccount(typeof(DRScheduleTran.accountID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodID()]
		[PXUIField(DisplayName = "Fin. Period", Enabled=false)]
		public virtual String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Nbr.", Enabled=false)]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region AdjgDocType
		public abstract class adjgDocType : PX.Data.IBqlField
		{
		}
		protected String _AdjgDocType;
		[PXDBString(3, IsFixed = true, InputMask = "")]
		public virtual String AdjgDocType
		{
			get
			{
				return this._AdjgDocType;
			}
			set
			{
				this._AdjgDocType = value;
			}
		}
		#endregion
		#region AdjgRefNbr
		public abstract class adjgRefNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjgRefNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String AdjgRefNbr
		{
			get
			{
				return this._AdjgRefNbr;
			}
			set
			{
				this._AdjgRefNbr = value;
			}
		}
		#endregion
		#region AdjNbr
		public abstract class adjNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjNbr;
		[PXDBInt()]
		public virtual Int32? AdjNbr
		{
			get
			{
				return this._AdjNbr;
			}
			set
			{
				this._AdjNbr = value;
			}
		}
		#endregion
		#region IsSamePeriod
		[Obsolete(
			"This BQL field is obsolete and will be removed in Acumatica 6.0. To check whether a transaction " +
			"is in the same period as the incoming transaction, perform an explicit check.",
			false)]
		public abstract class isSamePeriod : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsSamePeriod;

		/// <summary>
		/// The property is obsolete and is not used anywhere.
		/// Cannot be removed due to a bug in Copy-Paste functionality - AC-77988. Should be removed once the source bug is fixed.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Same Period", Enabled = true, Visible = false)]
		[Obsolete(
			"This property is obsolete and will be removed in Acumatica 6.0. To check whether a transaction " +
			"is in the same period as the incoming transaction, perform an explicit check.", 
			false)]
		public virtual Boolean? IsSamePeriod
		{
			get
			{
				return this._IsSamePeriod;
			}
			set
			{
				this._IsSamePeriod = value;
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

	public static class DRScheduleTranStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
				new [] { Open, Posted, Projected },
				new [] { Messages.Open, Messages.Posted, Messages.Projected })
			{ }
		}

		public const string Open = "O";
		public const string Posted = "P";
		public const string Projected = "J";

		public class OpenStatus : Constant<string>
		{
			public OpenStatus() : base(DRScheduleTranStatus.Open) { ;}
		}

		public class PostedStatus : Constant<string>
		{
			public PostedStatus() : base(DRScheduleTranStatus.Posted) { ;}
		}

		public class ProjectedStatus : Constant<string>
		{
			public ProjectedStatus() : base(DRScheduleTranStatus.Projected) { ;}
		}
	}
}
