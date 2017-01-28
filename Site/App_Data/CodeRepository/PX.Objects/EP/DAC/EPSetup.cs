using PX.Objects.CA;
using PX.SM;

namespace PX.Objects.EP
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.IN;
	using PX.Objects.CR;

	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(EPSetupMaint))]
    [PXCacheName(Messages.EPSetup)]
	public partial class EPSetup : PX.Data.IBqlTable, IAssignedMap
	{
		public const string Minute = "MINUTE";
		public const string Hour = "HOUR";

		#region ClaimNumberingID
		public abstract class claimNumberingID : PX.Data.IBqlField
		{
		}
		protected String _ClaimNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("EPCLAIM")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Expense Claim Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String ClaimNumberingID
		{
			get
			{
				return this._ClaimNumberingID;
			}
			set
			{
				this._ClaimNumberingID = value;
			}
		}
		#endregion
		#region PerRetainTran
		public abstract class perRetainTran : PX.Data.IBqlField
		{
		}
		protected Int16? _PerRetainTran;
		[PXDBShort()]
		[PXDefault((short)99)]
		[PXUIField(DisplayName = "Keep Transactions for", Visibility = PXUIVisibility.Visible)]
		public virtual Int16? PerRetainTran
		{
			get
			{
				return this._PerRetainTran;
			}
			set
			{
				this._PerRetainTran = value;
			}
		}
		#endregion
		#region PerRetainHist
		public abstract class perRetainHist : PX.Data.IBqlField
		{
		}
		protected Int16? _PerRetainHist;
		[PXDBShort()]
		[PXDefault((short)120)]
		[PXUIField(DisplayName = "Periods to Retain History", Visibility = PXUIVisibility.Invisible)]
		public virtual Int16? PerRetainHist
		{
			get
			{
				return this._PerRetainHist;
			}
			set
			{
				this._PerRetainHist = value;
			}
		}
		#endregion
		#region HoldEntry
		public abstract class holdEntry : PX.Data.IBqlField
		{
		}
		protected Boolean? _HoldEntry;
		[PXDBBool()]
		[PXDefault(true)]
        [PXUIField(DisplayName = "Hold Expense Claims on Entry")]
		public virtual Boolean? HoldEntry
		{
			get
			{
				return this._HoldEntry;
			}
			set
			{
				this._HoldEntry = value;
			}
		}
		#endregion
		#region CopyNotesAR
		public abstract class copyNotesAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyNotesAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Notes to AR Documents")]
		public virtual Boolean? CopyNotesAR
		{
			get
			{
				return this._CopyNotesAR;
			}
			set
			{
				this._CopyNotesAR = value;
			}
		}
		#endregion
		#region CopyFilesAR
		public abstract class copyFilesAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyFilesAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Files to AR Documents")]
		public virtual Boolean? CopyFilesAR
		{
			get
			{
				return this._CopyFilesAR;
			}
			set
			{
				this._CopyFilesAR = value;
			}
		}
		#endregion
		#region CopyNotesAP
		public abstract class copyNotesAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyNotesAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Notes to AP Documents")]
		public virtual Boolean? CopyNotesAP
		{
			get
			{
				return this._CopyNotesAP;
			}
			set
			{
				this._CopyNotesAP = value;
			}
		}
		#endregion
		#region CopyFilesAP
		public abstract class copyFilesAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyFilesAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Files to AP Documents")]
		public virtual Boolean? CopyFilesAP
		{
			get
			{
				return this._CopyFilesAP;
			}
			set
			{
				this._CopyFilesAP = value;
			}
		}
		#endregion
		#region CopyNotesPM
		public abstract class copyNotesPM : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyNotesPM;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Notes to PM Documents")]
		public virtual Boolean? CopyNotesPM
		{
			get
			{
				return this._CopyNotesPM;
			}
			set
			{
				this._CopyNotesPM = value;
			}
		}
		#endregion
		#region CopyFilesPM
		public abstract class copyFilesPM : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyFilesPM;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Files to PM Documents")]
		public virtual Boolean? CopyFilesPM
		{
			get
			{
				return this._CopyFilesPM;
			}
			set
			{
				this._CopyFilesPM = value;
			}
		}
		#endregion
		#region AutomaticReleaseAP
		public abstract class automaticReleaseAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutomaticReleaseAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Automatically Release AP Documents")]
		public virtual Boolean? AutomaticReleaseAP
		{
			get
			{
				return this._AutomaticReleaseAP;
			}
			set
			{
				this._AutomaticReleaseAP = value;
			}
		}
		#endregion
		#region AutomaticReleaseAR
		public abstract class automaticReleaseAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutomaticReleaseAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Automatically Release AR Documents")]
		public virtual Boolean? AutomaticReleaseAR
		{
			get
			{
				return this._AutomaticReleaseAR;
			}
			set
			{
				this._AutomaticReleaseAR = value;
			}
		}
		#endregion				
		#region AutomaticReleasePM
		public abstract class automaticReleasePM : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutomaticReleasePM;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Automatically Release PM Documents")]
		public virtual Boolean? AutomaticReleasePM
		{
			get
			{
				return this._AutomaticReleasePM;
			}
			set
			{
				this._AutomaticReleasePM = value;
			}
		}
		#endregion				
		
		#region ClaimAssignmentMapID
		public abstract class claimAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _ClaimAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeExpenceClaim>>>))]
        [PXUIField(DisplayName = "Expense Claim Approval Map")]
		public virtual int? ClaimAssignmentMapID
		{
			get
			{
				return this._ClaimAssignmentMapID;
			}
			set
			{
				this._ClaimAssignmentMapID = value;
			}
		}
		#endregion
        #region ClaimAssignmentNotificationID
        public abstract class claimAssignmentNotificationID : PX.Data.IBqlField
        {
        }
        protected int? _ClaimAssignmentNotificationID;
        [PXDBInt]
        [PXSelector(typeof(Search<Notification.notificationID>))]
        [PXUIField(DisplayName = "Expense Claim Notification")]
        public virtual int? ClaimAssignmentNotificationID
        {
            get
            {
                return this._ClaimAssignmentNotificationID;
            }
            set
            {
                this._ClaimAssignmentNotificationID = value;
            }
        }
        #endregion
		#region ExpenseSubMask
		public abstract class expenseSubMask : PX.Data.IBqlField
		{
		}
		protected String _ExpenseSubMask;
		[PXDefault()]
		[SubAccountMaskAttribute(DisplayName = "Combine Expense Sub. From")]
		public virtual String ExpenseSubMask
		{
			get
			{
				return this._ExpenseSubMask;
			}
			set
			{
				this._ExpenseSubMask = value;
			}
		}
		#endregion
		#region SalesSubMask
		public abstract class salesSubMask : PX.Data.IBqlField
		{
		}
		protected String _SalesSubMask;
		[PXDefault()]
		[SubAccountMaskAttribute(DisplayName = "Combine Sales Sub. From")]
		public virtual String SalesSubMask
		{
			get
			{
				return this._SalesSubMask;
			}
			set
			{
				this._SalesSubMask = value;
			}
		}
		#endregion
		#region SendOnlyEventCard

		public abstract class sendOnlyEventCard : IBqlField { }

		protected Boolean? _SendOnlyEventCard;

		[PXDBBool]
		[PXUIField(DisplayName = "Only iCalendar Card")]
		[PXDefault(false)]
		public virtual Boolean? SendOnlyEventCard
		{
			get { return _SendOnlyEventCard; }
			set { _SendOnlyEventCard = value; }
		}

		#endregion
		#region IsSimpleNotification

		public abstract class isSimpleNotification : IBqlField { }

		protected Boolean? _IsSimpleNotification;

		[PXDBBool]
		[PXUIField(DisplayName = "Simple Notification")]
		[PXDefault(true)]
		public virtual Boolean? IsSimpleNotification
		{
			get { return _IsSimpleNotification; }
			set { _IsSimpleNotification = value; }
		}

		#endregion
		#region AddContactInformation

		public abstract class addContactInformation : IBqlField { }

		protected Boolean? _AddContactInformation;

		[PXDBBool]
		[PXUIField(DisplayName = "Add Contact Information")]
		[PXDefault(false)]
		public virtual Boolean? AddContactInformation
		{
			get { return _AddContactInformation; }
			set { _AddContactInformation = value; }
		}

		#endregion
		#region InvitationTemplate
		public abstract class invitationTemplateID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Invitation Template")]
		[PXSelector(typeof(Search<Notification.notificationID>),
			DescriptionField = typeof(Notification.name))]
		public virtual Int32? InvitationTemplateID { get; set; }
		#endregion
		#region RescheduleTemplateID
		public abstract class rescheduleTemplateID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Reschedule Template")]
		[PXSelector(typeof(Search<Notification.notificationID>),
			DescriptionField = typeof(Notification.name))]
		public virtual Int32? RescheduleTemplateID { get; set; }
		#endregion
		#region CancelInvitationTemplateID
		public abstract class cancelInvitationTemplateID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Cancel Invitation Template")]
		[PXSelector(typeof(Search<Notification.notificationID>),
			DescriptionField = typeof(Notification.name))]
		public virtual Int32? CancelInvitationTemplateID { get; set; }
		#endregion
		#region SearchOnlyInWorkingTime

		public abstract class searchOnlyInWorkingTime : IBqlField { }

		protected Boolean? _SearchOnlyInWorkingTime;

		[PXDBBool]
		[PXUIField(DisplayName = "Search Only In Working Time")]
		[PXDefault(false)]
		public virtual Boolean? SearchOnlyInWorkingTime
		{
			get { return _SearchOnlyInWorkingTime; }
			set { _SearchOnlyInWorkingTime = value; }
		}

		#endregion
		#region RequireTimes

		public abstract class requireTimes : IBqlField { }

		protected Boolean? _RequireTimes;

		[PXDBBool]
		[PXUIField(DisplayName = "Require Time On Activity")]
		[PXDefault(true)]
		public virtual Boolean? RequireTimes
		{
			get { return _RequireTimes; }
			set { _RequireTimes = value; }
		}

		#endregion
		#region TimeCardNumberingID
		public abstract class timeCardNumberingID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault("TIMECARD")]
		[PXUIField(DisplayName = "Time Card Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String TimeCardNumberingID { get; set; }
		#endregion
		#region TimeCardAssignmentMapID
		public abstract class timeCardAssignmentMapID : IBqlField { }

		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeTimeCard>>>))]
        [PXUIField(DisplayName = "Time Card Approval Map")]
		public virtual int? TimeCardAssignmentMapID { get; set; }
		#endregion
        #region TimeCardAssignmentNotificationID
        public abstract class timeCardAssignmentNotificationID : PX.Data.IBqlField
        {
        }
        protected int? _TimeCardAssignmentNotificationID;
        [PXDBInt]
        [PXSelector(typeof(Search<Notification.notificationID>))]
        [PXUIField(DisplayName = "Time Card Notification")]
        public virtual int? TimeCardAssignmentNotificationID
        {
            get
            {
                return this._TimeCardAssignmentNotificationID;
            }
            set
            {
                this._TimeCardAssignmentNotificationID = value;
            }
        }
        #endregion
        #region EquipmentTimeCardNumberingID
        public abstract class equipmentTimeCardNumberingID : PX.Data.IBqlField
        {
        }
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault("EQTIMECARD")]
		[PXUIField(DisplayName = "Equipment Time Card Numbering Sequence", FieldClass = "PROJECT")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual String EquipmentTimeCardNumberingID { get; set; }
        #endregion
        #region EquipmentTimeCardAssignmentMapID
        public abstract class equipmentTimeCardAssignmentMapID : IBqlField { }

        [PXDBInt]
        [PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
            Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeEquipmentTimeCard>>>))]
		[PXUIField(DisplayName = "Equipment Time Card Approval Map", FieldClass = "PROJECT")]
        public virtual int? EquipmentTimeCardAssignmentMapID { get; set; }
        #endregion
        #region EquipmentTimeCardAssignmentNotificationID
        public abstract class equipmentTimeCardAssignmentNotificationID : PX.Data.IBqlField
        {
        }
        protected int? _EquipmentTimeCardAssignmentNotificationID;
        [PXDBInt]
        [PXSelector(typeof(Search<Notification.notificationID>))]
		[PXUIField(DisplayName = "Equipment Time Card Notification", FieldClass = "PROJECT")]
        public virtual int? EquipmentTimeCardAssignmentNotificationID
        {
            get
            {
                return this._EquipmentTimeCardAssignmentNotificationID;
            }
            set
            {
                this._EquipmentTimeCardAssignmentNotificationID = value;
            }
        }
        #endregion

        #region ActivityTimeUnit
        public abstract class activityTimeUnit : IBqlField { }

		[PXDefault(Minute, PersistingCheck = PXPersistingCheck.Nothing)]
		[INUnit(DisplayName = "Activity Time Unit", Visible=false)]
		public virtual String ActivityTimeUnit { get; set; }
		#endregion
		#region EmployeeRateUnit
		public abstract class employeeRateUnit : IBqlField { }

		[PXDefault(Hour, PersistingCheck = PXPersistingCheck.Nothing)]
		[INUnit(DisplayName = "Employee Hour Rate Unit", Visible=false)]
		public virtual String EmployeeRateUnit { get; set; }
		#endregion

		#region GroupTransactgion
		public abstract class groupTransactgion : IBqlField
		{
		}
		[PXDBString(3, IsUnicode = false)]
		[PXDefault(EPGroupTransaction.DoNotSplit)]
		[EPGroupTransaction.ListAttribule]
		[PXUIField(DisplayName = "Group Transaction by", Visibility = PXUIVisibility.SelectorVisible, Visible=false)]
		public virtual String GroupTransactgion { get; set; }
		#endregion

		#region DefaultActivityType
		public abstract class defaultActivityType : IBqlField
		{
		}
		[PXDBString(5, IsUnicode = false, IsFixed = true)]
		[PXUIRequired(typeof(IIf<FeatureInstalled<FeaturesSet.timeReportingModule>, True, False>))]
		[PXDefault("W", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<EPActivityType.type, Where<EPActivityType.isInternal, Equal<True>>>), DescriptionField = typeof(EPActivityType.description))]
		[PXRestrictor(typeof(Where<EPActivityType.active, Equal<True>>), CR.Messages.InactiveActivityType, typeof(EPActivityType.type))]
		[PXUIField(DisplayName = "Default Time Activity Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DefaultActivityType { get; set; }
		#endregion

		#region MinBillableTime
		public abstract class minBillableTime : IBqlField
		{
		}
		[PXDBInt()]
		[PXDefault(0)]
		[PXTimeList(5, 12)]
		[PXUIField(DisplayName = "Min Billable Time", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? MinBillableTime { get; set; }
		#endregion

		#region RegularHoursType
		public abstract class regularHoursType : IBqlField
		{
		}
		[PXDBString(2, IsUnicode = false, IsFixed = true)]
		[PXDefault(typeof(Search<EPEarningType.typeCD, Where<EPEarningType.typeCD, Equal<EarningTypeRG>>>))]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Regular Hours Earning Type")]
		public virtual String RegularHoursType { get; set; }
		#endregion

		#region HolidaysType
		public abstract class holidaysType : IBqlField
		{
		}
		[PXDBString(2, IsUnicode = false, IsFixed = true)]
		[PXDefault(typeof(Search<EPEarningType.typeCD, Where<EPEarningType.typeCD, Equal<EarningTypeHL>>>))]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Holiday Earning Type")]
		public virtual String HolidaysType { get; set; }
		#endregion

		#region VacationsType
		public abstract class vacationsType : IBqlField
		{
		}
		[PXDBString(2, IsUnicode = false, IsFixed = true)]
		[PXDefault(typeof(Search<EPEarningType.typeCD, Where<EPEarningType.typeCD, Equal<EarningTypeVL>>>))]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Vacations Earning Type")]
		public virtual String VacationsType { get; set; }
		#endregion

		#region isPreloadHolidays
		public abstract class ispreloadHolidays : IBqlField { }
		[PXDBBool]
        [PXUIField(DisplayName = "Preload Holidays on Time Card Entry", Visible = false)]
		[PXDefault(false)]
		public virtual Boolean? isPreloadHolidays { get; set; }
		#endregion
        
        #region DefTasksFilterID
        public abstract class defTasksFilterID : IBqlField { }
        [PXDBLong]
        [PXUIField(DisplayName = "Default Task Filter")]
        [PXSelector(typeof(Search<FilterHeader.filterID, Where<FilterHeader.isShared, Equal<True>>>),
            DescriptionField = typeof(FilterHeader.filterName))]
        public virtual Int64? DefTasksFilterID { get; set; }
        #endregion
        
		#region DefEventsFilterID
        public abstract class defEventsFilterID : IBqlField { }
        [PXDBLong]
        [PXUIField(DisplayName = "Default Event Filter")]
        [PXSelector(typeof(Search<FilterHeader.filterID, Where<FilterHeader.isShared, Equal<True>>>),
            DescriptionField = typeof(FilterHeader.filterName))]
        public virtual Int64? DefEventsFilterID { get; set; }
        #endregion

        #region PostToOffBalance
        public abstract class postToOffBalance : IBqlField { }
        [PXDBBool]
        [PXUIField(DisplayName = "Post to Off-Balance Account Group")]
        [PXDefault(false)]
        public virtual Boolean? PostToOffBalance { get; set; }
        #endregion
        #region OffBalanceAccountGroupID
        public abstract class offBalanceAccountGroupID : PX.Data.IBqlField
        {
        }
        protected Int32? _OffBalanceAccountGroupID;
        [PM.AccountGroup(typeof(Where<PM.PMAccountGroup.type, Equal<PM.PMAccountType.offBalance>>), DisplayName="Off-Balance Account Group")]
        public virtual Int32? OffBalanceAccountGroupID
        {
            get
            {
                return this._OffBalanceAccountGroupID;
            }
            set
            {
                this._OffBalanceAccountGroupID = value;
            }
        }
        #endregion

		#region CustomWeek

		public abstract class customWeek : IBqlField { }

		protected Boolean? _CustomWeek;

		[PXDBBool]
		[PXUIField(DisplayName = "Custom Week Configuration")]
		[PXDefault(false)]
		public virtual Boolean? CustomWeek
		{
			get { return _CustomWeek; }
			set { _CustomWeek = value; }
		}

		#endregion

		#region FirstDayOfWeek

		public abstract class firstDayOfWeek : IBqlField { }

		[PXDBInt]
		[PXIntList(new[] { (int)DayOfWeek.Sunday, (int)DayOfWeek.Monday, (int)DayOfWeek.Tuesday, 
				(int)DayOfWeek.Wednesday, (int)DayOfWeek.Thursday, (int)DayOfWeek.Friday, (int)DayOfWeek.Saturday },
			new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" })]
		[PXUIField(DisplayName = "First Day of Week")]
		[PXUIEnabled(typeof(customWeek))]
		[PXDefault((int)DayOfWeek.Sunday, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? FirstDayOfWeek { get; set; }

		#endregion

		#region System
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
		#endregion

		#region ClaimDetailsAssignmentMapID
		public abstract class claimDetailsAssignmentMapID : IBqlField
		{
		}
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeExpenceClaimDetails>>>))]
		[PXUIField(DisplayName = "Expense Receipt Approval Map")]
		public virtual int? ClaimDetailsAssignmentMapID
		{
			get;
			set;
		}
		#endregion

		#region ClaimDetailsAssignmentNotificationID
		public abstract class claimDetailsAssignmentNotificationID : IBqlField
		{
		}
		[PXDBInt]
		[PXSelector(typeof(Search<Notification.notificationID>))]
		[PXUIField(DisplayName = "Expense Receipt Notification")]
		public virtual int? ClaimDetailsAssignmentNotificationID
		{
			get;
			set;
		}
		#endregion

		#region IAssignedMap members
		[PXInt]
		public int? AssignmentMapID
		{
			get;
			set;
		}

		[PXInt]
		public int? AssignmentNotificationID
		{
			get;
			set;
		}

		[PXBool]
		public bool? IsActive
		{
			get;
			set;
		}
		#endregion

		public sealed class EarningTypeRG : Constant<string>
		{
			public EarningTypeRG() : base("RG") { }
		}

		public sealed class EarningTypeHL : Constant<string>
		{
			public EarningTypeHL() : base("HL") { }
		}

		public sealed class EarningTypeVL : Constant<string>
		{
			public EarningTypeVL() : base("VL") { }
		}

		protected class CopyNoteSettings : PXNoteAttribute.IPXCopySettings
		{
			public CopyNoteSettings(bool? copyNotes = false, bool? copyFiles = false)
			{
				CopyNotes = copyNotes;
				CopyFiles = copyFiles;
			}

			public bool? CopyNotes { get; set; }
			public bool? CopyFiles { get; set; }
		}

		public PXNoteAttribute.IPXCopySettings GetCopyNoteSettings<TModule>()
		{
			return typeof (TModule) == typeof (PXModule.ar)
					? new CopyNoteSettings(CopyNotesAR, CopyFilesAR)
				: (typeof (TModule) == typeof (PXModule.ap)
					? new CopyNoteSettings(CopyNotesAP, CopyFilesAP)
				: (typeof (TModule) == typeof (PXModule.pm)
						? new CopyNoteSettings(CopyNotesPM, CopyFilesPM)
					: new CopyNoteSettings()));
		}
	}

    public static class EPTimeCardType
    {
        public class ListAttribute: PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[]{ TimeCardDefault, TimeCardSimple },
                new string[] { Messages.TimeCardDefault, Messages.TimeCardSimple }) { ; }
        }
        public const string TimeCardDefault = "D";
        public const string TimeCardSimple = "S";

		public class timeCardDefault : Constant<string>
		{
			public timeCardDefault() : base(TimeCardDefault) { ;}
		}
		public class timeCardSimple : Constant<string>
		{
			public timeCardSimple() : base(TimeCardSimple) { ;}
		}
    }

	public static class EPGroupTransaction
	{
		public class ListAttribule : PXStringListAttribute
		{
			public ListAttribule() : base
				(
					new string[] { DoNotSplit, Week, Project, Employee, WeekEmployee, ProjectEmployee, WeekProject, WeekProjectEmployee }
					, new string[] { Messages.DoNotSplit, Messages.Week, PM.Messages.Project, Messages.Employee, Messages.WeekEmployee, Messages.ProjectEmployee, Messages.WeekProject, Messages.WeekProjectEmployee }
				) { }
		}

		public const string DoNotSplit = "N";
		public const string Week = "W";
		public const string Project = "P";
		public const string Employee = "E";
		public const string WeekEmployee = "WE";
		public const string ProjectEmployee = "PE";
		public const string WeekProject = "WP";
		public const string WeekProjectEmployee = "WPE";

		public class doNotSplit : Constant<string> { public doNotSplit() : base(DoNotSplit) { } }
		public class week : Constant<string> { public week() : base(Week) { } }
		public class project : Constant<string> { public project() : base(Project) { } }
		public class employee : Constant<string> { public employee() : base(Employee) { } }
		public class weekEmployee: Constant<string> { public weekEmployee() : base(WeekEmployee) { } }
		public class projectEmployee : Constant<string> { public projectEmployee() : base(ProjectEmployee) { } }
		public class weekProject : Constant<string> { public weekProject() : base(WeekProject) { } }
		public class weekProjectEmployee : Constant<string> { public weekProjectEmployee() : base(WeekProjectEmployee) { } }
	}
		
}
