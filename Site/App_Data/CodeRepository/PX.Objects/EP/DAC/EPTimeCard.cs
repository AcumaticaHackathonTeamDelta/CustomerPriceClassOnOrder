using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.TM;

namespace PX.Objects.EP
{
	[PXPrimaryGraph(typeof(TimeCardMaint))]
	[PXCacheName(Messages.TimeCard)]
	[Serializable]
	[PXEMailSource]
	public partial class EPTimeCard : IBqlTable, IAssign
	{

		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
		[PXDefault]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(EPSetup.timeCardNumberingID), typeof(AccessInfo.businessDate))]
		[PXSelector(typeof(Search2<EPTimeCard.timeCardCD,
			InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPTimeCard.employeeID>>>,
			Where<EPTimeCard.createdByID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
						 Or<EPTimeCard.noteID, Approver<Current<AccessInfo.userID>>,
						 Or<EPTimeCard.employeeID, WingmanUser<Current<AccessInfo.userID>>>>>>>>),
			typeof(EPTimeCard.timeCardCD),
			typeof(EPTimeCard.employeeID),
			typeof(EPTimeCard.weekDescription),
			typeof(EPTimeCard.status))]
		[PXFieldDescription]
		public virtual String TimeCardCD { get; set; }
		#endregion

		#region EmployeeID
		public abstract class employeeID : IBqlField { }

		[PXDBInt]
		[PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
		[PXUIField(DisplayName = "Employee")]
		[PXSubordinateAndWingmenSelector]
		[PXFieldDescription]
		public virtual Int32? EmployeeID { get; set; }
		#endregion
		#region Status

		public abstract class status : IBqlField { }

		public const string ApprovedStatus = "A";
		public const string HoldStatus = "H";
		public const string ReleasedStatus = "R";
		public const string OpenStatus = "O";
		public const string RejectedStatus = "C";

		[PXDBString(1)]
		[PXDefault("H")]
		[PXStringList(new[] { HoldStatus, OpenStatus, ApprovedStatus, RejectedStatus, ReleasedStatus }, new[] { "On Hold", "Pending Approval", "Approved", "Rejected", "Released" })]
		[PXUIField(DisplayName = "Status", Enabled=false)]
		public virtual String Status { get; set; }

		#endregion
		#region WeekID

		public abstract class weekId : IBqlField { }

		protected Int32? _WeekID;
		[PXDBInt]
		[PXUIField(DisplayName = "Week")]
		[PXWeekSelector2(DescriptionField = typeof(EPWeekRaw.shortDescription))]
		public virtual Int32? WeekID
		{
			get
			{
				return this._WeekID;
			}
			set
			{
				this._WeekID = value;
			}
		}

		#endregion
		#region OrigTimeCardCD
		public abstract class origTimeCardCD : IBqlField { }
		[PXUIField(DisplayName = "Orig. Ref. Nbr.", Enabled = false)]
		[PXDBString(10, IsUnicode = true)]
		public virtual String OrigTimeCardCD { get; set; }
		#endregion
		#region IsApproved

		public abstract class isApproved : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsApproved { get; set; }

		#endregion
		#region IsRejected

		public abstract class isRejected : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsRejected { get; set; }

		#endregion
		#region IsHold

		public abstract class isHold : IBqlField { }

		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsHold { get; set; }

		#endregion
		#region IsReleased

		public abstract class isReleased : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsReleased { get; set; }

		#endregion
		#region WorkgroupID
		public abstract class workgroupID : IBqlField { }

		[PXInt]
		[PXUIField(DisplayName = "Workgroup ID", Visible = false)]
		[PXSelector(typeof(EPCompanyTreeOwner.workGroupID), SubstituteKey = typeof(EPCompanyTreeOwner.description))]
		public virtual int? WorkgroupID { get; set; }
		#endregion
		#region OwnerID
		public abstract class ownerID : IBqlField { }

		[PXGuid]
		[PXUIField(Visible = false)]
		public virtual Guid? OwnerID { get; set; }
		#endregion
		#region SummaryLineCntr
		public abstract class summaryLineCntr : PX.Data.IBqlField
		{
		}
		protected int? _SummaryLineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual int? SummaryLineCntr
		{
			get
			{
				return this._SummaryLineCntr;
			}
			set
			{
				this._SummaryLineCntr = value;
			}
		}
		#endregion

		#region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(typeof(EPTimeCard),
			DescriptionField = typeof(EPTimeCard.timeCardCD),
			Selector = typeof(EPTimeCard.timeCardCD)
			)]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
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


		#region Unbound Fields (Calculated in the TimecardMaint graph)

		#region Selected
		public abstract class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }
		#endregion

		#region WeekStartDate (Used in Report)

		public abstract class weekStartDate : IBqlField { }

		protected DateTime? _WeekStartDate;
		[PXDate]
		[PXUIField(DisplayName = "Week Start Date")]
		[PXFormula(typeof(Selector<EPTimeCard.weekId, EPWeekRaw.startDate>))]
		public virtual DateTime? WeekStartDate {
			get { return _WeekStartDate; }
			set { _WeekStartDate = value; }
		}

		#endregion

		public abstract class weekDescription : IBqlField { }
		[PXString]
		[PXUIField(DisplayName = "Week")]
		[PXFormula(typeof(Selector<EPTimeCard.weekId, EPWeekRaw.description>))]
		public virtual String WeekDescription { get; set; }

		public abstract class weekShortDescription : IBqlField { }
		[PXString]
		[PXUIField(DisplayName = "Week")]
		[PXFieldDescription]
		[PXFormula(typeof(Selector<EPTimeCard.weekId, EPWeekRaw.shortDescription>))]
		public virtual String WeekShortDescription { get; set; }

		public abstract class timeSpentCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Time Spent", Enabled = false)]
		public virtual Int32? TimeSpentCalc { get; set; }

		public abstract class overtimeSpentCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Overtime Spent", Enabled = false)]
		public virtual Int32? OvertimeSpentCalc { get; set; }

		public abstract class totalSpentCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Total Time Spent", Enabled = false)]
		public virtual Int32? TotalSpentCalc { get; set; }

		public abstract class timeBillableCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Billable", Enabled = false)]
		public virtual Int32? TimeBillableCalc { get; set; }

		public abstract class overtimeBillableCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Billable Overtime", Enabled = false)]
		public virtual Int32? OvertimeBillableCalc { get; set; }

		public abstract class totalBillableCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXUIField(DisplayName = "Total Billable", Enabled = false)]
		public virtual Int32? TotalBillableCalc { get; set; }

		public abstract class timecardType : IBqlField { }
		[PXString]
		[PXStringList(new string[] { "N", "C", "D" }, new string[] { "Normal", "Correction", "Normal-Corrected" })]
		[PXUIField(DisplayName = "Type", Enabled = false)]
		public virtual string TimecardType
		{
			get;set;// { return string.IsNullOrEmpty(OrigTimeCardCD) ? "N" : "C"; }

		}

		public abstract class billingRateCalc : IBqlField { }
		[PXInt]
		[PXUIField(DisplayName = "Billing Ratio", Enabled = false)]
		public virtual Int32? BillingRateCalc
		{
			get
			{
				if (TotalSpentCalc != 0)
				{
					return TotalBillableCalc * 100 / TotalSpentCalc;
				}
				else
				{
					return 0;
				}
			}
		}

		#endregion
	}



	#region Projections

	[PXProjection(typeof(Select5<PMTimeActivity,
		InnerJoin<EPEarningType,
			On<EPEarningType.typeCD, Equal<PMTimeActivity.earningTypeID>>>,
		Where<PMTimeActivity.timeCardCD, IsNotNull, And<EPEarningType.isOvertime, Equal<False>,
			And<PMTimeActivity.trackTime, Equal<True>>>>,
		Aggregate<
			GroupBy<PMTimeActivity.timeCardCD,
			Sum<PMTimeActivity.timeSpent,
			Sum<PMTimeActivity.timeBillable>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class TimecardRegularFinalTotals : IBqlTable
	{
		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10, BqlField = typeof(PMTimeActivity.timeCardCD))]
		public virtual string TimeCardCD { get; set; }
		#endregion

		#region TimeSpent
		public abstract class timeSpent : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
		public virtual int? TimeSpent { get; set; }
		#endregion

		#region TimeBillable
		public abstract class timeBillable : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
		public virtual int? TimeBillable { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select5<PMTimeActivity,
		InnerJoin<EPEarningType, 
			On<EPEarningType.typeCD, Equal<PMTimeActivity.earningTypeID>>>,
		Where<PMTimeActivity.timeCardCD, IsNotNull, And<EPEarningType.isOvertime, Equal<True>,
			And<PMTimeActivity.trackTime, Equal<True>>>>,
		Aggregate<
			GroupBy<PMTimeActivity.timeCardCD,
			Sum<PMTimeActivity.timeSpent,
			Sum<PMTimeActivity.timeBillable>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class TimecardOvertimeFinalTotals : IBqlTable
	{
		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10, BqlField = typeof(PMTimeActivity.timeCardCD))]
		public virtual string TimeCardCD { get; set; }
		#endregion

		#region OvertimeSpent
		public abstract class overtimeSpent : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
		public virtual int? OvertimeSpent { get; set; }
		#endregion

		#region OvertimeBillable
		public abstract class overtimeBillable : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
		public virtual int? OvertimeBillable { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select5<PMTimeActivity,
		InnerJoin<EPEarningType, 
			On<EPEarningType.typeCD, Equal<PMTimeActivity.earningTypeID>>>,
		Where<PMTimeActivity.timeCardCD, IsNull, And<EPEarningType.isOvertime, Equal<False>,
			And<PMTimeActivity.trackTime, Equal<True>>>>,
		Aggregate<
			GroupBy<PMTimeActivity.weekID,
			GroupBy<PMTimeActivity.ownerID,
			Sum<PMTimeActivity.timeSpent,
			Sum<PMTimeActivity.timeBillable>>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class TimecardRegularCurrentTotals : IBqlTable
	{
		#region WeekID
		public abstract class weekID : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.weekID))]
		public virtual int? WeekID { get; set; }
		#endregion

		#region Owner
		public abstract class owner : IBqlField { }
		
		[PXDBGuid(BqlField = typeof(PMTimeActivity.ownerID))]
		public virtual Guid? Owner { get; set; }
		#endregion

		#region TimeSpent
		public abstract class timeSpent : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
		public virtual int? TimeSpent { get; set; }
		#endregion

		#region TimeBillable
		public abstract class timeBillable : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
		public virtual int? TimeBillable { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select5<PMTimeActivity,
		InnerJoin<EPEarningType, 
			On<EPEarningType.typeCD, Equal<PMTimeActivity.earningTypeID>>>,
		Where<PMTimeActivity.timeCardCD, IsNull, And<EPEarningType.isOvertime, Equal<True>,
			And<PMTimeActivity.trackTime, Equal<True>>>>,
		Aggregate<
			GroupBy<PMTimeActivity.weekID,
			GroupBy<PMTimeActivity.ownerID,
			Sum<PMTimeActivity.timeSpent,
			Sum<PMTimeActivity.timeBillable>>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class TimecardOvertimeCurrentTotals : IBqlTable
	{
		#region WeekID
		public abstract class weekID : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.weekID))]
		public virtual int? WeekID { get; set; }
		#endregion

		#region Owner
		public abstract class owner : IBqlField { }
		
		[PXDBGuid(BqlField = typeof(PMTimeActivity.ownerID))]
		public virtual Guid? Owner { get; set; }
		#endregion

		#region OvertimeSpent
		public abstract class overtimeSpent : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
		public virtual int? OvertimeSpent { get; set; }
		#endregion

		#region OvertimeBillable
		public abstract class overtimeBillable : IBqlField { }
		
		[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
		public virtual int? OvertimeBillable { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select2<EPTimeCard,
					InnerJoin<EPEmployeeEx, On<EPEmployeeEx.bAccountID, Equal<EPTimeCard.employeeID>>,
					LeftJoin<TimecardRegularFinalTotals, On<TimecardRegularFinalTotals.timeCardCD, Equal<EPTimeCard.timeCardCD>, And<EPTimeCard.isHold, Equal<False>>>,
					LeftJoin<TimecardOvertimeFinalTotals, On<TimecardOvertimeFinalTotals.timeCardCD, Equal<EPTimeCard.timeCardCD>, And<EPTimeCard.isHold, Equal<False>>>,
					LeftJoin<TimecardRegularCurrentTotals, On<TimecardRegularCurrentTotals.weekID, Equal<EPTimeCard.weekId>, And<EPTimeCard.isHold, Equal<True>, And<EPEmployeeEx.userID, Equal<TimecardRegularCurrentTotals.owner>>>>,
					LeftJoin<TimecardOvertimeCurrentTotals, On<TimecardOvertimeCurrentTotals.weekID, Equal<EPTimeCard.weekId>, And<EPTimeCard.isHold, Equal<True>, And<EPEmployeeEx.userID, Equal<TimecardOvertimeCurrentTotals.owner>>>>>>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class TimecardWithTotals : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC", BqlField = typeof(EPTimeCard.timeCardCD))]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search2<EPTimeCard.timeCardCD,
			InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPTimeCard.employeeID>>>,
			Where<EPTimeCard.createdByID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
						 Or<EPTimeCard.noteID, Approver<Current<AccessInfo.userID>>>>>>>),
			typeof(EPTimeCard.timeCardCD),
			typeof(EPTimeCard.employeeID),
			typeof(EPTimeCard.weekDescription),
			typeof(EPTimeCard.status))]
		[PXFieldDescription]
		public virtual String TimeCardCD { get; set; }
		#endregion

		#region EmployeeID
		public abstract class employeeID : IBqlField { }

		[PXDBInt(BqlField = typeof(EPTimeCard.employeeID))]
		[PXUIField(DisplayName = "Employee")]
		[PXSubordinateAndWingmenSelector]
		[PXFieldDescription]
		public virtual Int32? EmployeeID { get; set; }
		#endregion
		#region Status

		public abstract class status : IBqlField { }

		[PXDBString(1, BqlField = typeof(EPTimeCard.status))]
		[PXStringList(new[] { EPTimeCard.HoldStatus, EPTimeCard.OpenStatus, EPTimeCard.ApprovedStatus, EPTimeCard.RejectedStatus, EPTimeCard.ReleasedStatus }, new[] { "On Hold", "Pending Approval", "Approved", "Rejected", "Released" })]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public virtual String Status { get; set; }

		#endregion
		#region WeekID

		public abstract class weekId : IBqlField { }

		protected Int32? _WeekID;
		[PXDBInt(BqlField = typeof(EPTimeCard.weekId))]
		[PXUIField(DisplayName = "Week")]
		[PXWeekSelector2(DescriptionField = typeof(EPWeekRaw.shortDescription))]
		public virtual Int32? WeekID
		{
			get
			{
				return this._WeekID;
			}
			set
			{
				this._WeekID = value;
			}
		}

		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(typeof(EPTimeCard), BqlField = typeof(EPTimeCard.noteID),
			DescriptionField = typeof(EPTimeCard.timeCardCD),
			Selector = typeof(EPTimeCard.timeCardCD)
			)]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region IsApproved

		public abstract class isApproved : IBqlField { }

		[PXDBBool(BqlField = typeof(EPTimeCard.isApproved))]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsApproved { get; set; }

		#endregion
		#region IsRejected

		public abstract class isRejected : IBqlField { }

		[PXDBBool(BqlField = typeof(EPTimeCard.isRejected))]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsRejected { get; set; }

		#endregion
		#region IsHold

		public abstract class isHold : IBqlField { }

		[PXDBBool(BqlField = typeof(EPTimeCard.isHold))]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsHold { get; set; }

		#endregion
		#region IsReleased

		public abstract class isReleased : IBqlField { }

		[PXDBBool(BqlField = typeof(EPTimeCard.isReleased))]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? IsReleased { get; set; }

		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID(BqlField = typeof(EPTimeCard.createdByID))]
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

		#region TimeSpentFinal
		public abstract class timeSpentFinal : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardRegularFinalTotals.timeSpent))]
		public virtual Int32? TimeSpentFinal
		{
			get;
			set;
		}
		#endregion
		#region TimeBillableFinal
		public abstract class timeBillableFinal : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardRegularFinalTotals.timeBillable))]
		public virtual Int32? TimeBillableFinal
		{
			get;
			set;
		}
		#endregion
		#region OvertimeSpentFinal
		public abstract class overtimeSpentFinal : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardOvertimeFinalTotals.overtimeSpent))]
		public virtual Int32? OvertimeSpentFinal
		{
			get;
			set;
		}
		#endregion
		#region OvertimeBillableFinal
		public abstract class overtimeBillableFinal : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardOvertimeFinalTotals.overtimeBillable))]
		public virtual Int32? OvertimeBillableFinal
		{
			get;
			set;
		}
		#endregion
		#region TimeSpentCurrent
		public abstract class timeSpentCurrent : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardRegularCurrentTotals.timeSpent))]
		public virtual Int32? TimeSpentCurrent
		{
			get;
			set;
		}
		#endregion
		#region TimeBillableCurrent
		public abstract class timeBillableCurrent : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardRegularCurrentTotals.timeBillable))]
		public virtual Int32? TimeBillableCurrent
		{
			get;
			set;
		}
		#endregion
		#region OvertimeSpentCurrent
		public abstract class overtimeSpentCurrent : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardOvertimeCurrentTotals.overtimeSpent))]
		public virtual Int32? OvertimeSpentCurrent
		{
			get;
			set;
		}
		#endregion
		#region OvertimeBillableCurrent
		public abstract class overtimeBillableCurrent : IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(TimecardOvertimeCurrentTotals.overtimeBillable))]
		public virtual Int32? OvertimeBillableCurrent
		{
			get;
			set;
		}
		#endregion


		public abstract class timeSpentCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(timeSpentFinal), typeof(timeSpentCurrent))]
		[PXUIField(DisplayName = "Time Spent", Enabled = false)]
		public virtual Int32? TimeSpentCalc
		{
			get { return TimeSpentFinal.GetValueOrDefault() + TimeSpentCurrent.GetValueOrDefault(); }
		}

		public abstract class overtimeSpentCalc : IBqlField { }

		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(overtimeSpentFinal), typeof(overtimeSpentCurrent))]
		[PXUIField(DisplayName = "Overtime Spent", Enabled = false)]
		public virtual Int32? OvertimeSpentCalc
		{
			get
			{
				int val = OvertimeSpentFinal.GetValueOrDefault() + OvertimeSpentCurrent.GetValueOrDefault();

				if (val == 0)
					return null;
				else
					return val;
			}
		}

		public abstract class totalSpentCalc : IBqlField { }

		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(timeSpentCalc), typeof(overtimeSpentCalc))]
		[PXUIField(DisplayName = "Total Time Spent", Enabled = false)]
		public virtual Int32? TotalSpentCalc
		{
			get { return TimeSpentCalc + OvertimeSpentCalc.GetValueOrDefault(); }
		}

		public abstract class timeBillableCalc : IBqlField { }

		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(timeBillableFinal), typeof(timeBillableCurrent))]
		[PXUIField(DisplayName = "Billable", Enabled = false)]
		public virtual Int32? TimeBillableCalc
		{
			get
			{
				int val = TimeBillableFinal.GetValueOrDefault() + TimeBillableCurrent.GetValueOrDefault();

				if (val == 0)
					return null;
				else
					return val;
			}
		}

		public abstract class overtimeBillableCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(overtimeBillableFinal), typeof(overtimeBillableCurrent))]
		[PXUIField(DisplayName = "Billable Overtime", Enabled = false)]
		public virtual Int32? OvertimeBillableCalc
		{
			get
			{
				int val = OvertimeBillableFinal.GetValueOrDefault() + OvertimeBillableCurrent.GetValueOrDefault();
				if (val == 0)
					return null;
				else
					return val;
			}
		}

		public abstract class totalBillableCalc : IBqlField { }
		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(timeBillableCalc), typeof(overtimeBillableCalc))]
		[PXUIField(DisplayName = "Total Billable", Enabled = false)]
		public virtual Int32? TotalBillableCalc
		{
			get
			{
				int val = TimeBillableCalc.GetValueOrDefault() + OvertimeBillableCalc.GetValueOrDefault();
				if (val == 0)
					return null;
				else
					return val;
			}
		}


		public abstract class billingRateCalc : IBqlField { }
		[PXInt]
		[PXDependsOnFields(typeof(totalBillableCalc), typeof(totalSpentCalc))]
		[PXUIField(DisplayName = "Billing Ratio", Enabled = false)]
		public virtual Int32? BillingRateCalc
		{
			get
			{
				if (TotalSpentCalc != 0)
				{
					return TotalBillableCalc * 100 / TotalSpentCalc;
				}
				else
				{
					return 0;
				}
			}
		}

		#region WeekStartDate (Used in Report)

		public abstract class weekStartDate : IBqlField { }

		protected DateTime? _WeekStartDate;
		[PXDate]
		[PXUIField(DisplayName = "Week Start Date", Visible = false)]
		[PXFormula(typeof(Selector<EPTimeCard.weekId, EPWeekRaw.startDate>))]
		public virtual DateTime? WeekStartDate
		{
			get
			{
				return PXWeekSelector2Attribute.GetWeekStartDate(WeekID.GetValueOrDefault());
			}
		}

		#endregion
	}

	public class EPEmployeeEx : EPEmployee
	{
		public new abstract class bAccountID : IBqlField { }
		public new abstract class userID : IBqlField { }
	}

	#endregion
}
