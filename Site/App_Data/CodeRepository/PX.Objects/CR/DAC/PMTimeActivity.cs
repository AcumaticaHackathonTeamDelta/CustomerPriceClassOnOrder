using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PR;
using PX.SM;
using PX.TM;
using PX.Web.UI;

namespace PX.Objects.CR
{
	[CRTimeActivityPrimaryGraph]
	[Serializable]
	[PXCacheName(Messages.TimeActivity)]
	public partial class PMTimeActivity : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }
		#endregion

		#region NoteID
		public abstract class noteID : IBqlField { }
		
		[PXSequentialNote(new Type[0], SuppressActivitiesCount = true, IsKey = true)]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region RefNoteID
		public abstract class refNoteID : IBqlField { }

		[PXDBGuid]
		[PXUIField(Visible = false)]
		[PXParent(typeof(Select<CRActivity, Where<CRActivity.noteID, Equal<Current<refNoteID>>>>), ParentCreate = true)]
		public virtual Guid? RefNoteID { get; set; }
		#endregion

		#region ParentTaskNoteID
		public abstract class parentTaskNoteID : IBqlField { }

		[PXDBGuid]
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing)]		
        [CRTaskSelector]
		[PXRestrictor(typeof(Where<CRActivity.ownerID, Equal<Current<AccessInfo.userID>>>), null)]
		[PXUIField(DisplayName = "Task")]
		public virtual Guid? ParentTaskNoteID { get; set; }
		#endregion

		#region TrackTime
		public abstract class trackTime : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Track Time")]
		[PXFormula(typeof(IIf<FeatureInstalled<FeaturesSet.timeReportingModule>, IsNull<Selector<Current<CRActivity.type>, EPActivityType.requireTimeByDefault>, False>, False>))]
		public virtual bool? TrackTime { get; set; }
		#endregion

		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10)]
		[PXUIField(Visible = false)]
		public virtual string TimeCardCD { get; set; }
		#endregion
		
		#region TimeSheetCD
		public abstract class timeSheetCD : IBqlField { }

		[PXDBString(15)]
		[PXUIField(Visible = false)]
		public virtual string TimeSheetCD { get; set; }
		#endregion

		#region Summary
		public abstract class summary : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Summary", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXNavigateSelector(typeof(summary))]
		public virtual string Summary { get; set; }
		#endregion

		#region Date
		public abstract class date : IBqlField { }

		[PXDBDateAndTime(DisplayNameDate = "Date", DisplayNameTime = "Time", UseTimeZone = true)]
		[PXUIField(DisplayName = "Date")]
		[PXFormula(typeof(IsNull<Current<CRActivity.startDate>, Current<CRSMEmail.startDate>>))]
		public virtual DateTime? Date { get; set; }
		#endregion
		
		#region Owner
		public abstract class ownerID : IBqlField { }

		[PXDBGuid]
		[PXChildUpdatable(AutoRefresh = true)]
		[PXSubordinateOwnerSelector]
		[PXUIField(DisplayName = "Owner")]
		public virtual Guid? OwnerID { get; set; }
		#endregion
		
		#region EarningTypeID
		public abstract class earningTypeID : IBqlField { }

		[PXDBString(2, IsFixed = true, IsUnicode = false, InputMask = ">LL")]
		[PXDefault("RG", typeof(Search<EPSetup.regularHoursType>), PersistingCheck = PXPersistingCheck.Null)]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Earning Type")]
		public virtual string EarningTypeID { get; set; }
		#endregion

		#region IsBillable
		public abstract class isBillable : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Billable", FieldClass = "BILLABLE")]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Switch<
			Case<Where<IsNull<Current<CRActivity.classID>, Current<CRSMEmail.classID>>, Equal<CRActivityClass.task>, 
				Or<IsNull<Current<CRActivity.classID>, Current<CRSMEmail.classID>>, Equal<CRActivityClass.events>>>, False,
			Case<Where2<FeatureInstalled<FeaturesSet.timeReportingModule>, And<trackTime, Equal<True>>>,
				IsNull<Selector<earningTypeID, EPEarningType.isbillable>, False>>>,
			False>))]
		public virtual bool? IsBillable { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : IBqlField { }

		[EPActivityProjectDefault(typeof(isBillable))]
		[EPProject(typeof(ownerID), FieldClass = ProjectAttribute.DimensionName)]
		[PXFormula(typeof(
			Switch<
				Case<Where<Not<FeatureInstalled<FeaturesSet.projectModule>>>, DefaultValue<projectID>,
				Case<Where<isBillable, Equal<True>, And<Current2<projectID>, Equal<NonProject>>>, Null,
				Case<Where<isBillable, Equal<False>, And<Current2<projectID>, IsNull>>, DefaultValue<projectID>>>>,
			projectID>))]
		public virtual int? ProjectID { get; set; }
		#endregion

		#region ProjectTaskID
		public abstract class projectTaskID : IBqlField { }

		[ProjectTask(typeof(projectID), BatchModule.EP, DisplayName = "Project Task")]
		[PXFormula(typeof(Switch<
			Case<Where<Current2<projectID>, Equal<NonProject>>, Null>,
			projectTaskID>))]
		public virtual int? ProjectTaskID { get; set; }
        #endregion

        #region ApproverID
        public abstract class approverID : IBqlField { }

        [PXDBInt]
        [PXEPEmployeeSelector]
        [PXFormula(typeof(
            Switch<
                Case<Where<Current2<projectID>, Equal<NonProject>>, Null, Case<Where<Current2<projectTaskID>, IsNull>, Null>>,
                Selector<projectTaskID, PMTask.approverID>>
            ))]
        [PXUIField(DisplayName = "Approver", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? ApproverID { get; set; }
        #endregion

        #region ApprovalStatus
        public abstract class approvalStatus : IBqlField { }

        [PXDBString(2, IsFixed = true)]
        [ApprovalStatus]
        [PXUIField(DisplayName = "Approval Status", Enabled = false)]
        [PXFormula(typeof(Switch<
            Case<Where<trackTime, Equal<True>, And<
				Where<Current2<approvalStatus>, IsNull, Or<Current2<approvalStatus>, Equal<ActivityStatusAttribute.open>>>>>, ActivityStatusAttribute.open,
            Case<Where<released, Equal<True>>, ActivityStatusAttribute.released,
            Case<Where<approverID, IsNotNull>, ActivityStatusAttribute.pendingApproval>>>,
            ActivityStatusAttribute.completed>))]
        public virtual string ApprovalStatus { get; set; }

        #endregion

        #region ApprovedDate
        public abstract class approvedDate : IBqlField { }

        [PXDBDate(DisplayMask = "d", PreserveTime = true)]
        [PXUIField(DisplayName = "Approved Date")]
        public virtual DateTime? ApprovedDate { get; set; }
        #endregion


        #region ContractID
        public abstract class contractID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Contract", Visible = false)]
		[PXSelector(typeof(Search2<Contract.contractID,
			LeftJoin<ContractBillingSchedule, On<Contract.contractID, Equal<ContractBillingSchedule.contractID>>>,
			Where<Contract.isTemplate, NotEqual<True>,
				And<Contract.baseType, Equal<Contract.ContractBaseType>>>,
			OrderBy<Desc<Contract.contractCD>>>),
			DescriptionField = typeof(Contract.description),
			SubstituteKey = typeof(Contract.contractCD), Filterable = true)]
		[PXRestrictor(typeof(Where<Contract.status, Equal<Contract.status.active>>), Messages.ContractIsNotActive)]
		[PXRestrictor(typeof(Where<Current<AccessInfo.businessDate>, LessEqual<Contract.graceDate>, Or<Contract.expireDate, IsNull>>), Messages.ContractExpired)]
		[PXRestrictor(typeof(Where<Current<AccessInfo.businessDate>, GreaterEqual<Contract.startDate>>), Messages.ContractActivationDateInFuture, typeof(Contract.startDate))]
		public virtual int? ContractID { get; set; }
		#endregion

		#region TimeSpent
		public abstract class timeSpent : IBqlField { }

		[PXDBInt]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Time Spent")]
		public virtual int? TimeSpent { get; set; }
		#endregion

		#region OvertimeSpent
		public abstract class overtimeSpent : IBqlField { }

		[PXDBInt]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Switch<Case<Where<Selector<earningTypeID, EPEarningType.isOvertime>, Equal<True>>, timeSpent>, int0>))]
		[PXUIField(DisplayName = "Overtime", Enabled = false)]
		public virtual int? OvertimeSpent { get; set; }
		#endregion

		#region TimeBillable
		public abstract class timeBillable : IBqlField { }

		[PXDBInt]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(
			Switch<Case<Where<isBillable, Equal<True>>, timeSpent,
				Case<Where<isBillable, Equal<False>>, int0>>,
				timeBillable>))]
		[PXUIField(DisplayName = "Billable Time", FieldClass = "BILLABLE")]
		[PXUIVerify(typeof(Where<timeSpent, IsNull, 
			Or<timeBillable, IsNull, 
				Or<timeSpent, GreaterEqual<timeBillable>>>>), PXErrorLevel.Error, Messages.BillableTimeCannotBeGreaterThanTimeSpent)]
		[PXUIVerify(typeof(Where<isBillable, NotEqual<True>, 
			Or<timeBillable, NotEqual<int0>>>), PXErrorLevel.Error, Messages.BillableTimeMustBeOtherThanZero,
			CheckOnInserted = false, CheckOnVerify = false)]
		public virtual int? TimeBillable { get; set; }
		#endregion

		#region OvertimeBillable
		public abstract class overtimeBillable : IBqlField { }

		[PXDBInt]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIVerify(typeof(Where<overtimeSpent, IsNull, 
			Or<overtimeBillable, IsNull, 
				Or<overtimeSpent, GreaterEqual<overtimeBillable>>>>), PXErrorLevel.Error, Messages.OvertimeBillableCannotBeGreaterThanOvertimeSpent)]
		[PXFormula(typeof(
			Switch<Case<Where<isBillable, Equal<True>, And<overtimeSpent, GreaterEqual<timeBillable>>>, timeBillable,
				Case<Where<isBillable, Equal<True>, And<overtimeSpent, GreaterEqual<Zero>>>, overtimeBillable,
					Case<Where<isBillable, Equal<False>>, int0>>>,
				overtimeBillable>))]
		[PXUIField(DisplayName = "Billable Overtime", FieldClass = "BILLABLE")]
		public virtual int? OvertimeBillable { get; set; }
		#endregion

		#region Billed
		public abstract class billed : IBqlField { }

		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Billed", FieldClass = "BILLABLE")]
		public virtual bool? Billed { get; set; }
		#endregion

		#region Released
		public abstract class released : IBqlField { }

		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Released", Enabled = false, Visible = false, FieldClass = "BILLABLE")]
		public virtual bool? Released { get; set; }
		#endregion

		#region IsCorrected
		public abstract class isCorrected : IBqlField { }

		/// <summary>
		/// If true this Activity has been corrected in the Timecard and is no longer valid. Please hide this activity in all lists displayed in the UI since there is another valid activity.
		/// The valid activity has a refence back to the corrected activity via OrigTaskID field. 
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? IsCorrected { get; set; }
		#endregion

		#region OrigNoteID
		public abstract class origNoteID : IBqlField { }

		/// <summary>
		/// Use for correction. Stores the reference to the original activity.
		/// </summary>
		[PXDBGuid]
		public virtual Guid? OrigNoteID { get; set; }
		#endregion

		#region TranID
		public abstract class tranID : IBqlField { }

		[PXDBLong]
		public virtual long? TranID { get; set; }
		#endregion

		#region WeekID
		public abstract class weekID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Time Card Week", Enabled = false)]
		[PXWeekSelector2]
		[PXFormula(typeof(Default<date>))]
		[EPActivityDefaultWeek(typeof(date))]
		public virtual int? WeekID { get; set; }
		#endregion

		#region LabourItemID
		public abstract class labourItemID : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false)]
		public virtual int? LabourItemID { get; set; }
		#endregion
		
		#region OvertimeItemID
		public abstract class overtimeItemID : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false)]
		public virtual int? OvertimeItemID { get; set; }
		#endregion

		#region JobID
		public abstract class jobID : IBqlField { }

		[PXDBInt]
		[PXSelector(typeof(PRJobCode.jobID), SubstituteKey = typeof(PRJobCode.jobCD))]
		[PXUIField(DisplayName = "Job Code", FieldClass = PRSetup.PayrollFieldClass)]
		public virtual int? JobID { get; set; }
		#endregion

		#region ShiftID
		public abstract class shiftID : IBqlField { }

		[PXDBInt]
		[PXSelector(typeof(PRShiftCode.shiftID), SubstituteKey = typeof(PRShiftCode.shiftCD))]
		[PXUIField(DisplayName = "Shift Code", FieldClass = PRSetup.WorkShiftsFieldClass)]
		public virtual int? ShiftID { get; set; }
		#endregion

		#region EmployeeRate
		public abstract class employeeRate : IBqlField { }

		/// <summary>
		/// Stores Employee's Hourly rate at the time the activity was released to PM
		/// </summary>
		[IN.PXDBPriceCost]
		[PXUIField(Visible = false)]
		public virtual decimal? EmployeeRate { get; set; }
		#endregion

		#region SummaryLineNbr
		public abstract class summaryLineNbr : IBqlField { }

		/// <summary>
		/// This is a adjusting activity for the summary line in the Timecard.
		/// </summary>
		[PXDBInt]
		public virtual int? SummaryLineNbr { get; set; }
		#endregion

		#region ARDocType
		public abstract class arDocType : IBqlField
		{
		}
		[AR.ARDocType.List()]
		[PXString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String ARDocType { get; set; }
		#endregion

		#region ARDocType
		public abstract class arRefNbr : IBqlField
		{
		}
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<AR.ARRegister.refNbr, Where<AR.ARRegister.docType, Equal<Current<arDocType>>>>), DescriptionField = typeof(AR.ARRegister.docType))]
		public virtual string ARRefNbr { get; set; }
		#endregion



		#region CreatedByID
		public abstract class createdByID : IBqlField { }

		[PXDBCreatedByID(DontOverrideValue = true)]
		[PXUIField(DisplayName = "Created By", Enabled = false)]
		public virtual Guid? CreatedByID { get; set; }
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField { }

		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField { }

		[PXUIField(DisplayName = "Created At", Enabled = false)]
		[PXDBCreatedDateTimeUtc]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField { }

		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion

		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField { }

		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID { get; set; }
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField { }

		[PXDBLastModifiedDateTimeUtc]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : IBqlField { }

		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }
        #endregion



        #region NeedToBeDeleted
        public abstract class needToBeDeleted : IBqlField { }

        [PXBool]
        [PXFormula(typeof(Switch<
				Case<Where<trackTime, NotEqual<True>,
					And<Where<projectID, IsNull, Or<projectID, Equal<NonProject>>>>>, True>,
			False>))]
		public bool? NeedToBeDeleted { get; set; }
        #endregion
    }



    [Serializable]
	[CRActivityPrimaryGraph]
	[PXBreakInheritance]
	[PXProjection(typeof(Select2<CRActivity,
		LeftJoin<PMTimeActivity,
			On<PMTimeActivity.refNoteID, Equal<CRActivity.noteID>>>,
		Where<PMTimeActivity.isCorrected, Equal<False>, 
			Or<PMTimeActivity.isCorrected, IsNull>>>), Persistent = true)]
	public partial class CRPMTimeActivity : CRActivity
	{
		#region Selected
		public abstract new class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public override bool? Selected { get; set; }
		#endregion
		


		#region CRActivity

		#region NoteID
		public abstract new class noteID : IBqlField { }

		[PXSequentialNote(new Type[0], SuppressActivitiesCount = true, IsKey = true, BqlField = typeof(CRActivity.noteID))]
		[PXTimeTag(typeof(noteID))]
		[CRPMTimeActivityStatisticFormulas]
		public override Guid? NoteID { get; set; }
		#endregion

		#region ParentNoteID
		public abstract new class parentNoteID : IBqlField { }

		[PXUIField(DisplayName = "Task")]
		[PXDBGuid(BqlField = typeof(CRActivity.parentNoteID))]
		[PXSelector(typeof(Search<CRActivity.noteID>), DescriptionField = typeof(CRActivity.subject), DirtyRead = true)]
		[PXRestrictor(typeof(Where<CRActivity.classID, Equal<CRActivityClass.task>, Or<CRActivity.classID, Equal<CRActivityClass.events>>>), null)]
		public override Guid? ParentNoteID { get; set; }
		#endregion

		#region RefNoteID
		public abstract new class refNoteID : IBqlField { }

		[PXDBGuid(BqlField = typeof(CRActivity.refNoteID))]
		//[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing, DefaultForUpdate = false)]
		[PXParent(typeof(Select<CRActivityStatistics, Where<CRActivityStatistics.noteID, Equal<Current<refNoteID>>>>), LeaveChildren = true, ParentCreate = true)]
		public override Guid? RefNoteID { get; set; }
		#endregion

		#region Source
		public abstract new class source : IBqlField { }

		[PXString(IsUnicode = true)]
		[PXUIField(DisplayName = "Related Entity Description", Enabled = false)]
		[PXFormula(typeof(EntityDescription<refNoteID>))]
		public override string Source { get; set; }
		#endregion

		#region ClassID
		public abstract new class classID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.classID))]
		[CRActivityClass]
		[PXDefault(typeof(CRActivityClass.activity))]
		[PXUIField(DisplayName = "Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public override int? ClassID { get; set; }
		#endregion

		#region ClassIcon
		public abstract new class classIcon : IBqlField { }

		[PXUIField(DisplayName = "Class Icon", IsReadOnly = true)]
		[PXImage]
		[PXFormula(typeof(Switch<Case<Where<classID, Equal<CRActivityClass.task>>, CRActivity.classIcon.task,
			Case<Where<classID, Equal<CRActivityClass.events>>, CRActivity.classIcon.events,
			Case<Where<classID, Equal<CRActivityClass.email>, And<incoming, Equal<True>>>, CRActivity.classIcon.email,
			Case<Where<classID, Equal<CRActivityClass.email>, And<outgoing, Equal<True>>>, CRActivity.classIcon.emailResponse
			>>>>,
			Selector<Current2<type>, EPActivityType.imageUrl>>))]
		public override string ClassIcon { get; set; }
		#endregion

		#region ClassInfo
		public abstract new class classInfo : IBqlField
		{
			public class emailResponse : Constant<string>
			{
				public emailResponse() : base(Messages.EmailResponseClassInfo) { }
			}
		}

		[PXString]
		[PXUIField(DisplayName = "Type", Enabled = false)]
		[PXFormula(typeof(Switch<
			Case<Where<Current2<noteID>, IsNull>, StringEmpty,
			Case<Where<classID, Equal<CRActivityClass.activity>, And<type, IsNotNull>>, Selector<type, EPActivityType.description>,
			Case<Where<classID, Equal<CRActivityClass.email>, And<incoming, Equal<True>>>, classInfo.emailResponse>>>,
			String<classID>>))]
		public override string ClassInfo { get; set; }
		#endregion

		#region Type
		public abstract new class type : IBqlField { }

		[PXDBString(5, IsFixed = true, IsUnicode = false, BqlField = typeof(CRActivity.type))]
		[PXUIField(DisplayName = "Type", Required = true)]
		[PXSelector(typeof(EPActivityType.type), DescriptionField = typeof(EPActivityType.description))]
		[PXRestrictor(typeof(Where<EPActivityType.active, Equal<True>>), Messages.InactiveActivityType, typeof(EPActivityType.type))]
		[PXRestrictor(typeof(Where<EPActivityType.isInternal, Equal<True>>), Messages.ExternalActivityType, typeof(EPActivityType.type))]
		[PXDefault(typeof(Search<EPActivityType.type,
			Where<EPActivityType.isInternal, Equal<True>,
				And<EPActivityType.isDefault, Equal<True>,
				And<Current<classID>, Equal<CRActivityClass.activity>>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		public override string Type { get; set; }
		#endregion

		#region Subject
		public abstract new class subject : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true, BqlField = typeof(CRActivity.subject))]
		[PXDefault]
		[PXUIField(DisplayName = "Summary", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXNavigateSelector(typeof(subject))]
		public override string Subject { get; set; }
		#endregion

		#region Location
		public abstract new class location : IBqlField { }

		[PXDBString(255, IsUnicode = true, InputMask = "", BqlField = typeof(CRActivity.location))]
		[PXUIField(DisplayName = "Location")]
		public override string Location { get; set; }
		#endregion

		#region Body
		public abstract new class body : IBqlField { }

		[PXDBText(IsUnicode = true, BqlField = typeof(CRActivity.body))]
		[PXUIField(DisplayName = "Activity Details")]
		public override string Body { get; set; }
		#endregion

		#region Priority
		public abstract new class priority : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.priority))]
		[PXUIField(DisplayName = "Priority")]
		[PXDefault(1, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXIntList(
			new [] { 0, 1, 2 },
			new [] { Messages.Low, Messages.Normal, Messages.High })]
		public override int? Priority { get; set; }
		#endregion

		#region PriorityIcon
		public abstract new class priorityIcon : IBqlField { }

		[PXUIField(DisplayName = "Priority Icon", IsReadOnly = true)]
		[PXImage(HeaderImage = (Sprite.AliasControl + "@" + Sprite.Control.PriorityHead))]
		[PXFormula(typeof(Switch<
			Case<Where<priority, Equal<int0>>, CRActivity.priorityIcon.low,
			Case<Where<priority, Equal<int2>>, CRActivity.priorityIcon.high>>>))]
		public override string PriorityIcon { get; set; }
		#endregion

		#region UIStatus
		public abstract new class uistatus : IBqlField { }

		[PXDBString(2, IsFixed = true, BqlField = typeof(CRActivity.uistatus))]
		[PXFormula(typeof(Switch<
			Case<Where<type, IsNull>, ActivityStatusAttribute.open,
			Case<Where<trackTime, IsNull, Or<trackTime, Equal<False>>>, ActivityStatusAttribute.completed>>,
			ActivityStatusAttribute.open>))]
		[ActivityStatus]
		[PXUIField(DisplayName = "Status")]
		[PXDefault(ActivityStatusAttribute.Open, PersistingCheck = PXPersistingCheck.Nothing)]
		public override string UIStatus { get; set; }
		#endregion

		#region IsOverdue
		public abstract new class isOverdue : IBqlField { }

		[PXBool]
		public override bool? IsOverdue
		{
			[PXDependsOnFields(typeof(uistatus), typeof(endDate))]
			get
			{
				return
					UIStatus != ActivityStatusAttribute.Completed &&
					UIStatus != ActivityStatusAttribute.Canceled &&
					EndDate != null &&
					EndDate < PX.Common.PXTimeZoneInfo.Now;
			}
		}
		#endregion

		#region IsCompleteIcon
		public abstract new class isCompleteIcon : IBqlField { }

		[PXUIField(DisplayName = "Complete Icon", IsReadOnly = true)]
		[PXImage(HeaderImage = (Sprite.AliasControl + "@" + Sprite.Control.CompleteHead))]
		[PXFormula(typeof(Switch<Case<Where<uistatus, Equal<ActivityStatusListAttribute.completed>>, CRActivity.isCompleteIcon.completed>>))]
		public override String IsCompleteIcon { get; set; }
		#endregion

		#region CategoryID
		public abstract new class categoryID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.categoryID))]
		[PXSelector(typeof(EPEventCategory.categoryID), SubstituteKey = typeof(EPEventCategory.description))]
		[PXUIField(DisplayName = "Category")]
		public override int? CategoryID { get; set; }
		#endregion

		#region AllDay
		public abstract new class allDay : IBqlField { }

		[EPAllDay(typeof(startDate), typeof(endDate), BqlField = typeof(CRActivity.allDay))]
		[PXUIField(DisplayName = "All Day")]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public override bool? AllDay { get; set; }
		#endregion

		#region StartDate
		public abstract new class startDate : IBqlField { }

		[EPStartDate(AllDayField = typeof(allDay), DisplayName = "Start Date", DisplayNameDate = "Date", DisplayNameTime = "Start Time", BqlField = typeof(CRActivity.startDate))]
		[PXFormula(typeof(TimeZoneNow))]
		[PXUIField(DisplayName = "Start Date")]
		public override DateTime? StartDate { get; set; }
		#endregion

		#region EndDate
		public abstract new class endDate : IBqlField { }

		[EPEndDate(typeof(classID), typeof(startDate), typeof(timeSpent), AllDayField = typeof(allDay), BqlField = typeof(CRActivity.endDate))]
		[PXUIField(DisplayName = "End Time")]
		public override DateTime? EndDate { get; set; }
		#endregion

		#region CompletedDate
		public abstract new class completedDate : IBqlField { }

		[PXDBDate(InputMask = "g", PreserveTime = true, BqlField = typeof(CRActivity.completedDate))]
		[PXUIField(DisplayName = "Completed At", Enabled = false)]
		[PXFormula(typeof(
			Switch<
				Case<Where<uistatus, Equal<ActivityStatusAttribute.completed>>, PXDateAndTimeAttribute.now>,
			completedDate>))]
		public override DateTime? CompletedDate { get; set; }
		#endregion
		
		#region DayOfWeek
		public abstract new class dayOfWeek : IBqlField { }

		[PXInt]
		[PXUIField(DisplayName = PX.Objects.EP.Messages.DayOfWeek)]
		[DayOfWeek]
		public override int? DayOfWeek
		{
			[PXDependsOnFields(typeof(startDate))]
			get
			{
				var date = StartDate;
				return date != null ? (int?)((int)date.Value.DayOfWeek) : null;
			}
		}
		#endregion

		#region PercentCompletion
		public abstract new class percentCompletion : IBqlField { }

		[PXDBInt(MinValue = 0, MaxValue = 100, BqlField = typeof(CRActivity.percentCompletion))]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Completion (%)")]
		public override int? PercentCompletion { get; set; }
		#endregion

		#region Owner
		public abstract new class ownerID : IBqlField { }

		[PXDBGuid(BqlField = typeof(CRActivity.ownerID))]
		[PXChildUpdatable(AutoRefresh = true)]
		[PXOwnerSelector(typeof(workgroupID))]
		[PXUIField(DisplayName = "Owner")]
		[PXDefault(typeof(Coalesce<
			Search<EPCompanyTreeMember.userID,
				Where<EPCompanyTreeMember.workGroupID, Equal<Current<workgroupID>>,
				And<EPCompanyTreeMember.userID, Equal<Current<AccessInfo.userID>>>>>,
			Search<CREmployee.userID,
				Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>,
				And<Current<workgroupID>, IsNull>>>>),
				PersistingCheck = PXPersistingCheck.Nothing)]
		public override Guid? OwnerID { get; set; }
		#endregion

		#region Workgroup
		public abstract new class workgroupID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.workgroupID))]
		[PXChildUpdatable(UpdateRequest = true)]
		[PXUIField(DisplayName = "Workgroup")]
		[PXSubordinateGroupSelector]
		public override int? WorkgroupID { get; set; }
		#endregion

		#region IsExternal
		public abstract new class isExternal : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.isExternal))]
		[PXUIField(Visible = false)]
		public override bool? IsExternal { get; set; }
		#endregion

		#region IsPrivate
		public abstract new class isPrivate : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.isPrivate))]
		[PXUIField(DisplayName = "Internal")]
		[PXFormula(typeof(IsNull<Selector<type, EPActivityType.privateByDefault>, False>))]
		public override bool? IsPrivate { get; set; }
		#endregion

		#region Incoming
		public abstract new class incoming : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.incoming))]
		[PXFormula(typeof(Switch<Case<Where<type, IsNotNull>, Selector<type, EPActivityType.incoming>>, False>))]
		[PXUIField(DisplayName = "Incoming")]
		public override bool? Incoming { get; set; }
		#endregion

		#region Outgoing
		public abstract new class outgoing : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.outgoing))]
		[PXFormula(typeof(Switch<Case<Where<type, IsNotNull>, Selector<type, EPActivityType.outgoing>>, False>))]
		[PXUIField(DisplayName = "Outgoing")]
		public override bool? Outgoing { get; set; }
		#endregion

		#region Synchronize
		public abstract new class synchronize : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.synchronize))]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Synchronize")]
		public override bool? Synchronize { get; set; }
		#endregion

		#region BAccountID
		public abstract new class bAccountID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.bAccountID))]
		[PXUIField(DisplayName = "Business Account")]
		public override int? BAccountID { get; set; }
		#endregion

		#region ContactID
		public abstract new class contactID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.contactID))]
		[PXUIField(DisplayName = "Contact")]
		public override int? ContactID { get; set; }
		#endregion

		#region EntityDescription

		public abstract new class entityDescription : IBqlField { }

		[PXString(InputMask = "")]
		[PXUIField(DisplayName = "Entity", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, IsReadOnly = true)]
		public override string EntityDescription { get; set; }
		#endregion

		#region ShowAsID
		public abstract new class showAsID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.showAsID))]
		[PXUIField(DisplayName = "Show As")]
		[PXSelector(typeof(EPEventShowAs.showAsID), SubstituteKey = typeof(EPEventShowAs.description))]
		public override int? ShowAsID { get; set; }
		#endregion

		#region IsLocked
		public abstract new class isLocked : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.isLocked))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Locked")]
		[PXFormula(typeof(released))]
		public override bool? IsLocked { get; set; }
		#endregion



		#region DeletedDatabaseRecord
		public abstract new class deletedDatabaseRecord : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.deletedDatabaseRecord))]
		[PXDefault(false)]
		public override bool? DeletedDatabaseRecord { get; set; }
		#endregion

		#region Overrides
		public abstract new class createdDateTime : IBqlField { }
		#endregion

		#endregion

		#region PMTimeActivity

		#region TimeActivityNoteID
		public abstract class timeActivityNoteID : IBqlField { }

		[PXDBSequentialGuid(BqlField = typeof(PMTimeActivity.noteID))]
		[PXExtraKey]
		public virtual Guid? TimeActivityNoteID { get; set; }
		#endregion

		#region TimeActivityRefNoteID
		public abstract class timeActivityRefNoteID : IBqlField { }

		[PXDBGuid(BqlField = typeof(PMTimeActivity.refNoteID))]
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing, DefaultForUpdate = false)]
		public virtual Guid? TimeActivityRefNoteID
		{
			[PXDependsOnFields(typeof(noteID))]
			get
			{
				return NoteID;
			}
		}
		#endregion

		#region ParentTaskNoteID
		public abstract class parentTaskNoteID : IBqlField { }

		[PXDBGuid(BqlField = typeof(PMTimeActivity.parentTaskNoteID))]
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Task")]
		[PXFormula(typeof(
			Switch<
				Case<Where<Current2<classID>, Equal<CRActivityClass.task>>, noteID>,
			parentTaskNoteID>))]
		public virtual Guid? ParentTaskNoteID { get; set; }
		#endregion

		#region TrackTime
		public abstract class trackTime : IBqlField { }

		[PXDBBool(BqlField = typeof(PMTimeActivity.trackTime))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Track Time")]
		[PXFormula(typeof(IIf<FeatureInstalled<FeaturesSet.timeReportingModule>, IsNull<Selector<type, EPActivityType.requireTimeByDefault>, False>, False>))]
		public virtual bool? TrackTime { get; set; }
		#endregion

		#region TimeCardCD
		public abstract class timeCardCD : IBqlField { }

		[PXDBString(10, BqlField = typeof(PMTimeActivity.timeCardCD))]
		[PXUIField(Visible = false)]
		public virtual string TimeCardCD { get; set; }
		#endregion

		#region TimeSheetCD
		public abstract class timeSheetCD : IBqlField { }

		[PXDBString(15, BqlField = typeof(PMTimeActivity.timeSheetCD))]
		[PXUIField(Visible = false)]
		public virtual string TimeSheetCD { get; set; }
		#endregion

		#region Summary
		public abstract class summary : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true, BqlField = typeof(PMTimeActivity.summary))]
		[PXDefault]
		[PXFormula(typeof(subject))]
		[PXUIField(DisplayName = "Summary", Visibility = PXUIVisibility.SelectorVisible)]
		[PXNavigateSelector(typeof(summary))]
		public virtual string Summary { get; set; }
		#endregion

		#region Date
		public abstract class date : IBqlField { }

		[PXDBDateAndTime(DisplayNameDate = "Date", DisplayNameTime = "Time", UseTimeZone = true, BqlField = typeof(PMTimeActivity.date))]
		[PXUIField(DisplayName = "Date")]
		[PXFormula(typeof(startDate))]
		public virtual DateTime? Date { get; set; }
		#endregion

		#region TimeActivityOwner
		public abstract class timeActivityOwner : IBqlField { }

		[PXDBGuid(BqlField = typeof(PMTimeActivity.ownerID))]
		[PXChildUpdatable(AutoRefresh = true)]
		[PXOwnerSelector(typeof(workgroupID))]
		[PXUIField(DisplayName = "Owner")]
		[PXFormula(typeof(ownerID))]
		public virtual Guid? TimeActivityOwner { get; set; }
		#endregion
		
		#region ApproverID
		public abstract class approverID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.approverID))]
		[PXEPEmployeeSelector]
		[PXFormula(typeof(
			Switch<
				Case<Where<Current2<projectID>, Equal<NonProject>>, Null,
				Case<Where<Current2<projectTaskID>, IsNull>, Null>>,
				Selector<projectTaskID, PMTask.approverID>>
			))]
		[PXUIField(DisplayName = "Approver", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? ApproverID { get; set; }
		#endregion

		#region ApprovalStatus
		public abstract class approvalStatus : IBqlField { }

		[PXDBString(2, IsFixed = true, BqlField = typeof(PMTimeActivity.approvalStatus))]
		[ActivityStatusList]
		[PXUIField(DisplayName = "Approval Status", Enabled = false)]
		[PXFormula(typeof(Switch<
			Case<Where<trackTime, Equal<True>, And<Current2<approvalStatus>, IsNull>>, ActivityStatusAttribute.open,
			Case<Where<released, Equal<True>>, ActivityStatusAttribute.released,
			Case<Where<approverID, IsNotNull>, ActivityStatusAttribute.pendingApproval>>>,
			ActivityStatusAttribute.completed>))]
		public virtual string ApprovalStatus { get; set; }

		#endregion

		#region ApprovedDate
		public abstract class approvedDate : IBqlField { }

		[PXDBDate(DisplayMask = "d", PreserveTime = true, BqlField = typeof(PMTimeActivity.approvedDate))]
		[PXUIField(DisplayName = "Approved Date")]
		public virtual DateTime? ApprovedDate { get; set; }
		#endregion

		#region EarningTypeID
		public abstract class earningTypeID : IBqlField { }

		[PXDBString(2, IsFixed = true, IsUnicode = false, InputMask = ">LL", BqlField = typeof(PMTimeActivity.earningTypeID))]
		[PXDefault("RG", typeof(Search<EPSetup.regularHoursType>), PersistingCheck = PXPersistingCheck.Null)]
		[PXUIRequired(typeof(trackTime))]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Earning Type")]
		public virtual string EarningTypeID { get; set; }
		#endregion

		#region IsBillable
		public abstract class isBillable : IBqlField { }

		[PXDBBool(BqlField = typeof(PMTimeActivity.isBillable))]
		[PXUIField(DisplayName = "Billable", FieldClass = "BILLABLE")]
		[PXFormula(typeof(Switch<
			Case<Where<classID, Equal<CRActivityClass.task>, Or<classID, Equal<CRActivityClass.events>>>, False,
			Case<Where2<FeatureInstalled<FeaturesSet.timeReportingModule>, And<trackTime, Equal<True>>>,
				IsNull<Selector<earningTypeID, EPEarningType.isbillable>, False>>>,
			False>))]
		public virtual bool? IsBillable { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : IBqlField { }

		[EPActivityProjectDefault(typeof(isBillable))]
		[EPProject(typeof(ownerID), FieldClass = ProjectAttribute.DimensionName, BqlField = typeof(PMTimeActivity.projectID))]
		[PXFormula(typeof(Switch<
			Case<Where<Not<FeatureInstalled<FeaturesSet.projectModule>>>, DefaultValue<projectID>,
			Case<Where<parentNoteID, IsNotNull,
					And<Selector<parentNoteID, Selector<projectID, PMProject.contractCD>>, IsNotNull>>,
				Selector<parentNoteID, Selector<projectID, PMProject.contractCD>>,
			Case<Where<isBillable, Equal<True>, And<Current2<projectID>, Equal<NonProject>>>, Null,
			Case<Where<isBillable, Equal<False>, And<Current2<projectID>, IsNull>>, DefaultValue<projectID>>>>>,
			projectID>))]
		public virtual int? ProjectID { get; set; }
		#endregion

		#region ProjectTaskID
		public abstract class projectTaskID : IBqlField { }

		[ProjectTask(typeof(projectID), BatchModule.EP, DisplayName = "Project Task", BqlField = typeof(PMTimeActivity.projectTaskID))]
		[PXFormula(typeof(Switch<
			Case<Where<Current2<projectID>, Equal<NonProject>>, Null,
			Case<Where<parentNoteID, IsNotNull>,
				Selector<parentNoteID, Selector<projectTaskID, PMTask.taskCD>>>>,
			projectTaskID>))]
		public virtual int? ProjectTaskID { get; set; }
		#endregion

		#region ContractID
		public abstract class contractID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.contractID))]
		[PXUIField(DisplayName = "Contract", Visible = false)]
		[PXSelector(typeof(Search2<Contract.contractID,
				LeftJoin<ContractBillingSchedule, On<Contract.contractID, Equal<ContractBillingSchedule.contractID>>>,
			Where<Contract.isTemplate, NotEqual<True>,
				And<Contract.baseType, Equal<Contract.ContractBaseType>>>,
			OrderBy<Desc<Contract.contractCD>>>),
			DescriptionField = typeof(Contract.description),
			SubstituteKey = typeof(Contract.contractCD), Filterable = true)]
		[PXRestrictor(typeof(Where<Contract.status, Equal<Contract.status.active>>), Messages.ContractIsNotActive)]
		[PXRestrictor(typeof(Where<Current<AccessInfo.businessDate>, LessEqual<Contract.graceDate>, Or<Contract.expireDate, IsNull>>), Messages.ContractExpired)]
		[PXRestrictor(typeof(Where<Current<AccessInfo.businessDate>, GreaterEqual<Contract.startDate>>), Messages.ContractActivationDateInFuture, typeof(Contract.startDate))]
		public virtual int? ContractID { get; set; }
		#endregion

		#region TimeSpent
		public abstract class timeSpent : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Time Spent")]
		[PXFormula(typeof(Switch<Case<Where<trackTime, NotEqual<True>>, int0>, timeSpent>))]
		public virtual int? TimeSpent { get; set; }
		#endregion

		#region OvertimeSpent
		public abstract class overtimeSpent : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.overtimeSpent))]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Switch<Case<Where<Selector<earningTypeID, EPEarningType.isOvertime>, Equal<True>>, timeSpent>, int0>))]
		[PXUIField(DisplayName = "Overtime", Enabled = false)]
		public virtual int? OvertimeSpent { get; set; }
		#endregion

		#region TimeBillable
		public abstract class timeBillable : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(
			Switch<Case<Where<isBillable, Equal<True>>, timeSpent,
				Case<Where<isBillable, Equal<False>>, int0>>,
			timeBillable>))]
		[PXUIField(DisplayName = "Billable Time", FieldClass = "BILLABLE")]
		[PXUIVerify(typeof(Where<timeSpent, IsNull,
			Or<timeBillable, IsNull,
			Or<timeSpent, GreaterEqual<timeBillable>>>>), PXErrorLevel.Error, Messages.BillableTimeCannotBeGreaterThanTimeSpent)]
		[PXUIVerify(typeof(Where<isBillable, NotEqual<True>,
			Or<timeBillable, NotEqual<int0>>>), PXErrorLevel.Error, Messages.BillableTimeMustBeOtherThanZero)]
		public virtual int? TimeBillable { get; set; }
		#endregion

		#region OvertimeBillable
		public abstract class overtimeBillable : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.overtimeBillable))]
		[PXTimeList]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIVerify(typeof(Where<overtimeSpent, IsNull,
			Or<overtimeBillable, IsNull,
				Or<overtimeSpent, GreaterEqual<overtimeBillable>>>>), PXErrorLevel.Error, Messages.OvertimeBillableCannotBeGreaterThanOvertimeSpent)]
		[PXFormula(typeof(
			Switch<Case<Where<isBillable, Equal<True>, And<overtimeSpent, GreaterEqual<timeBillable>>>, timeBillable,
				Case<Where<isBillable, Equal<True>, And<overtimeSpent, GreaterEqual<Zero>>>, overtimeBillable,
				Case<Where<isBillable, Equal<False>>, int0>>>,
			overtimeBillable>))]
		[PXUIField(DisplayName = "Billable Overtime", FieldClass = "BILLABLE")]
		public virtual int? OvertimeBillable { get; set; }
		#endregion

		#region Billed
		public abstract class billed : IBqlField { }

		[PXDBBool(BqlField = typeof(PMTimeActivity.billed))]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Billed", FieldClass = "BILLABLE")]
		public virtual bool? Billed { get; set; }
		#endregion

		#region Released
		public abstract class released : IBqlField { }

		[PXDBBool(BqlField = typeof(PMTimeActivity.released))]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Released", Enabled = false, Visible = false, FieldClass = "BILLABLE")]
		public virtual bool? Released { get; set; }
		#endregion

		#region IsCorrected
		public abstract class isCorrected : IBqlField { }

		/// <summary>
		/// If true this Activity has been corrected in the Timecard and is no longer valid. Please hide this activity in all lists displayed in the UI since there is another valid activity.
		/// The valid activity has a refence back to the corrected activity via OrigTaskID field. 
		/// </summary>
		[PXDBBool(BqlField = typeof(PMTimeActivity.isCorrected))]
		[PXDefault(false)]
		public virtual bool? IsCorrected { get; set; }
		#endregion

		#region OrigNoteID
		public abstract class origNoteID : IBqlField { }

		/// <summary>
		/// Use for correction. Stores the reference to the original activity.
		/// </summary>
		[PXDBGuid(BqlField = typeof(PMTimeActivity.origNoteID))]
		public virtual Guid? OrigNoteID { get; set; }
		#endregion

		#region TranID
		public abstract class tranID : IBqlField { }

		[PXDBLong(BqlField = typeof(PMTimeActivity.tranID))]
		public virtual long? TranID { get; set; }
		#endregion

		#region WeekID
		public abstract class weekID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.weekID))]
		[PXUIField(DisplayName = "Time Card Week", Enabled = false)]
		[PXWeekSelector2()]
		[PXFormula(typeof(Default<date, trackTime>))]
		[EPActivityDefaultWeek(typeof(date))]
		public virtual int? WeekID { get; set; }
		#endregion

		#region LabourItemID
		public abstract class labourItemID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.labourItemID))]
		[PXUIField(Visible = false)]
		public virtual int? LabourItemID { get; set; }
		#endregion

		#region OvertimeItemID
		public abstract class overtimeItemID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.overtimeItemID))]
		[PXUIField(Visible = false)]
		public virtual int? OvertimeItemID { get; set; }
		#endregion

		#region JobID
		public abstract class jobID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.jobID))]
		[PXSelector(typeof(PRJobCode.jobID), SubstituteKey = typeof(PRJobCode.jobCD))]
		[PXUIField(DisplayName = "Job Code", FieldClass = PRSetup.PayrollFieldClass)]
		public virtual int? JobID { get; set; }
		#endregion

		#region ShiftID
		public abstract class shiftID : IBqlField { }

		[PXDBInt(BqlField = typeof(PMTimeActivity.shiftID))]
		[PXSelector(typeof(PRShiftCode.shiftID), SubstituteKey = typeof(PRShiftCode.shiftCD))]
		[PXUIField(DisplayName = "Shift Code", FieldClass = PRSetup.WorkShiftsFieldClass)]
		public virtual int? ShiftID { get; set; }
		#endregion

		#region EmployeeRate
		public abstract class employeeRate : IBqlField { }

		/// <summary>
		/// Stores Employee's Hourly rate at the time the activity was released to PM
		/// </summary>
		[IN.PXDBPriceCost(BqlField = typeof(PMTimeActivity.employeeRate))]
		[PXUIField(Visible = false)]
		public virtual decimal? EmployeeRate { get; set; }
		#endregion

		#region SummaryLineNbr
		public abstract class summaryLineNbr : IBqlField { }

		/// <summary>
		/// This is a adjusting activity for the summary line in the Timecard.
		/// </summary>
		[PXDBInt(BqlField = typeof(PMTimeActivity.summaryLineNbr))]
		public virtual int? SummaryLineNbr { get; set; }
		#endregion

		#region ARDocType
		public abstract class arDocType : IBqlField
		{
		}
		[AR.ARDocType.List()]
		[PXString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String ARDocType { get; set; }
		#endregion

		#region ARDocType
		public abstract class arRefNbr : IBqlField
		{
		}
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<AR.ARRegister.refNbr, Where<AR.ARRegister.docType, Equal<Current<arDocType>>>>), DescriptionField = typeof(AR.ARRegister.docType))]
		public virtual string ARRefNbr { get; set; }
		#endregion



		#region TimeActivityCreatedByID
		public abstract class timeActivityCreatedByID : IBqlField { }

		[PXDBCreatedByID(DontOverrideValue = true, BqlField = typeof(PMTimeActivity.createdByID))]
		[PXUIField(DisplayName = "Created By", Enabled = false)]
		public virtual Guid? TimeActivityCreatedByID { get; set; }
		#endregion

		#region TimeActivityCreatedByScreenID
		public abstract class timeActivityCreatedByScreenID : IBqlField { }

		[PXDBCreatedByScreenID(BqlField = typeof(PMTimeActivity.createdByScreenID))]
		public virtual string TimeActivityCreatedByScreenID { get; set; }
		#endregion

		#region TimeActivityCreatedDateTime
		public abstract class timeActivityCreatedDateTime : IBqlField { }

		[PXUIField(DisplayName = "Created At", Enabled = false)]
		[PXDBCreatedDateTimeUtc(BqlField = typeof(PMTimeActivity.createdDateTime))]
		public virtual DateTime? TimeActivityCreatedDateTime { get; set; }
		#endregion

		#region TimeActivityLastModifiedByID
		public abstract class timeActivityLastModifiedByID : IBqlField { }

		[PXDBLastModifiedByID(BqlField = typeof(PMTimeActivity.lastModifiedByID))]
		public virtual Guid? TimeActivityLastModifiedByID { get; set; }
		#endregion

		#region TimeActivityLastModifiedByScreenID
		public abstract class timeActivityLastModifiedByScreenID : IBqlField { }

		[PXDBLastModifiedByScreenID(BqlField = typeof(PMTimeActivity.lastModifiedByScreenID))]
		public virtual string TimeActivityLastModifiedByScreenID { get; set; }
		#endregion

		#region TimeActivityLastModifiedDateTime
		public abstract class timeActivityLastModifiedDateTime : IBqlField { }

		[PXDBLastModifiedDateTimeUtc(BqlField = typeof(PMTimeActivity.lastModifiedDateTime))]
		public virtual DateTime? TimeActivityLastModifiedDateTime { get; set; }
		#endregion

		#endregion
	}
	
	[Serializable]
	[PXHidden]
	[PXBreakInheritance]
	public partial class CRChildActivity : CRPMTimeActivity
	{
		#region Overrides

		public new abstract class noteID : IBqlField { }
		public new abstract class classID : IBqlField { }
		public abstract new class uistatus : IBqlField { }
		public new abstract class createdDateTime : IBqlField { }
		public new abstract class ownerID : IBqlField { }
		public new abstract class isCorrected : IBqlField { }

		#endregion



		#region ParentNoteID
		public new abstract class parentNoteID : IBqlField { }

		[PXUIField(DisplayName = "Parent")]
		[PXDBGuid]
		[PXSelector(typeof(Search<CRPMTimeActivity.noteID>), DirtyRead = true)]
		[PXRestrictor(typeof(Where<CRPMTimeActivity.classID, Equal<CRActivityClass.task>, Or<CRPMTimeActivity.classID, Equal<CRActivityClass.events>>>), null)]
		[PXDBDefault(typeof(CRPMTimeActivity.noteID), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Guid? ParentNoteID { get; set; }
		#endregion
		
		#region UIStatus
		[PXDBString(2, IsFixed = true)]
		[ActivityStatus]
		[PXUIField(DisplayName = "Status")]
		[PXDefault(ActivityStatusAttribute.Open, PersistingCheck = PXPersistingCheck.Nothing)]
		public override string UIStatus { get; set; }
		#endregion
	}
}
