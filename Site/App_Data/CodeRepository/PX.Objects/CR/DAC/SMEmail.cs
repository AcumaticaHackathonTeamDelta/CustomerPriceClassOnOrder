using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.SM;
using PX.TM;
using PX.Web.UI;

namespace PX.Objects.CR
{
	[Serializable]
	[CRSMEmailPrimaryGraphAttribute]
	[PXCacheName(Messages.SystemEmail)]
	public partial class SMEmail : IBqlTable 
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
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing, DefaultForUpdate = false)]
		[PXParent(typeof(Select<CRActivity, Where<CRActivity.noteID, Equal<Current<refNoteID>>>>), ParentCreate = true)]
		public virtual Guid? RefNoteID { get; set; }
		#endregion
		
		#region Subject
		public abstract class subject : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Summary", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXNavigateSelector(typeof(subject))]
		public virtual string Subject { get; set; }
		#endregion
		
		#region Body
		public abstract class body : IBqlField { }

		[PXDBText(IsUnicode = true)]
		[PXUIField(DisplayName = "Activity Details")]
		public virtual string Body { get; set; }
		#endregion

		#region MPStatus
		public abstract class mpstatus : IBqlField { }

		[PXDBString(2, IsFixed = true, IsUnicode = false)]
		[MailStatusList]
		[PXDefault(ActivityStatusAttribute.Draft)]
		[PXUIField(DisplayName = "Mail Status", Enabled = false)]
		public virtual string MPStatus { get; set; }
		#endregion

		#region ImcUID
		public abstract class imcUID : IBqlField { }

		[PXDBGuid(true)]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual Guid? ImcUID
		{
			get { return _ImcUID ?? (_ImcUID = Guid.NewGuid()); }
			set { _ImcUID = value; }
		}
		protected Guid? _ImcUID;
		#endregion

		#region Pop3UID
		public abstract class pop3UID : IBqlField { }

		[PXDBString(150)]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual string Pop3UID { get; set; }
		#endregion

		#region ImapUID
		public abstract class imapUID : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual int? ImapUID { get; set; }
		#endregion

		#region MailAccountID
		public abstract class mailAccountID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "From")]
		public int? MailAccountID { get; set; }

		#endregion

		#region MailFrom
		public abstract class mailFrom : IBqlField { }

		[PXDBString(500, IsUnicode = true)]
		[PXUIField(DisplayName = "From")]
		public virtual string MailFrom { get; set; }
		#endregion

		#region MailReply
		public abstract class mailReply : IBqlField { }

		[PXDBString(500, IsUnicode = true)]
		[PXUIField(DisplayName = "Reply")]
		public virtual string MailReply { get; set; }
		#endregion

		#region MailTo
		public abstract class mailTo : IBqlField { }

		[PXDBString(1000, IsUnicode = true)]
		[PXUIField(DisplayName = "To")]
		public virtual string MailTo { get; set; }
		#endregion

		#region MailCc
		public abstract class mailCc : IBqlField { }

		[PXDBString(1000, IsUnicode = true)]
		[PXUIField(DisplayName = "CC")]
		public virtual string MailCc { get; set; }
		#endregion

		#region MailBcc
		public abstract class mailBcc : IBqlField { }

		[PXDBString(1000, IsUnicode = true)]
		[PXUIField(DisplayName = "BCC")]
		public virtual string MailBcc { get; set; }
		#endregion

		#region RetryCount
		public abstract class retryCount : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false)]
		[PXDefault(0)]
		public virtual int? RetryCount { get; set; }
		#endregion

		#region MessageId
		public abstract class messageId : IBqlField { }

		[PXDBString(255)] // TODO: need review length
		[PXUIField(Visible = false)]
		public virtual string MessageId { get; set; }
		#endregion

		#region MessageReference
		public abstract class messageReference : IBqlField { }

		[PXDBString]
		[PXUIField(Visible = false)]
		public virtual string MessageReference { get; set; }
		#endregion

		#region Exception
		public abstract class exception : IBqlField { }

		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "Error Message")]
		public virtual string Exception { get; set; }
        #endregion

        #region Exception
        public abstract class redexception : IBqlField { }

        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Error Message")]
        public virtual string RedException
        {
            get
            {
                return CacheUtility.GetErrorDescription(this.Exception);
            }
            
        }
        #endregion

        #region Format
        public abstract class format : IBqlField { }

		[PXDefault(EmailFormatListAttribute.Html, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(255)]
		[PXUIField(DisplayName = "Format")]
		[EmailFormatList]
		public virtual string Format { get; set; }
		#endregion

		#region ReportFormat
		public abstract class reportFormat : IBqlField { }

		[PXDBString(10)]
		[PXUIField(DisplayName = "Format")]
		[PXStringList(
			new[] { "PDF", "HTML", "Excel" }, 
			new[] { "PDF", "HTML", "Excel" })]
		public virtual string ReportFormat { get; set; }
		#endregion

		#region ID
		public abstract class id : IBqlField { }
		
		[PXDBIdentity]
		[PXUIField(Visible = false)]
		public virtual int? ID { get; set; }
		#endregion

		#region Ticket
		public abstract class ticket : IBqlField { }

		[PXDBInt]
		[PXUIField(Visible = false)]
		public virtual int? Ticket { get; set; }
		#endregion

		#region IsIncome
		public abstract class isIncome : IBqlField { }
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Is Income")]
		public virtual bool? IsIncome { get; set; }
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

		#region Source
		public abstract  class source : IBqlField { }

		[PXString(IsUnicode = true)]
		public virtual string Source { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select<SMEmail>))]
	[PXHidden]	
	public partial class SMEmailBody : IBqlTable
	{
		#region NoteID
		public abstract class noteID : IBqlField {}

		[PXDBGuid(IsKey = true, BqlField = typeof(SMEmail.noteID))]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region RefNoteID
		public abstract class refNoteID : IBqlField { }

		[PXDBGuid(BqlField = typeof(SMEmail.refNoteID))]
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing, DefaultForUpdate = false)]
		[PXParent(typeof(Select<CRActivity, Where<CRActivity.noteID, Equal<Current<refNoteID>>>>), ParentCreate = true)]
		public virtual Guid? RefNoteID { get; set; }
		#endregion

		#region Body
		public abstract class body : IBqlField { }

		[PXDBText(IsUnicode = true, BqlField = typeof(SMEmail.body))]
		[PXUIField(DisplayName = "Activity Details")]
		public virtual string Body { get; set; }
		#endregion

	}


	[Serializable]
	[CRActivityPrimaryGraph]
	[PXBreakInheritance]
	[PXProjection(typeof(Select2<CRActivity,
		InnerJoin<SMEmail,
			On<SMEmail.refNoteID, Equal<CRActivity.noteID>>>,
		Where<CRActivity.classID, Equal<CRActivityClass.email>,
				Or<CRActivity.classID, Equal<CRActivityClass.emailRouting>>>>), Persistent = true)]
	[PXCacheName(Messages.EmailActivity)]
	public partial class CRSMEmail : CRActivity
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
		[CRSMEmailStatisticFormulas]
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
		[PXParent(typeof(Select<CRActivityStatistics, Where<CRActivityStatistics.noteID, Equal<Current<refNoteID>>>>), LeaveChildren = true, ParentCreate = true)]
		[PXUIField(DisplayName = "References Nbr.")]
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
			Case<Where<classID, Equal<CRActivityClass.email>, And<isIncome, Equal<True>>>, classInfo.emailResponse>>>,
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
		//Empty for Email
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

		#region MPStatus
		public abstract class mpstatus : IBqlField { }

		[PXDBString(2, IsFixed = true, IsUnicode = false, BqlField = typeof(SMEmail.mpstatus))]
		[MailStatusList]
		[PXDefault(ActivityStatusAttribute.Draft)]
		[PXUIField(DisplayName = "Mail Status", Enabled = false)]
		public virtual string MPStatus { get; set; }
		#endregion

		#region UIStatus
		public abstract new class uistatus : IBqlField { }

		[PXDBString(2, IsFixed = true, BqlField = typeof(CRActivity.uistatus))]
		[PXFormula(typeof(Switch<
			Case<Where<mpstatus, Equal<MailStatusListAttribute.archived>>, uistatus,
			Case<Where<type, IsNull>, ActivityStatusAttribute.open,
			Case<Where<mpstatus, Equal<DoubleSpace>>, ActivityStatusAttribute.completed,
			Case<Where<mpstatus, Equal<MailStatusListAttribute.processed>>, ActivityStatusAttribute.completed,
			Case<Where<mpstatus, Equal<MailStatusListAttribute.deleted>,
							Or<mpstatus, Equal<MailStatusListAttribute.failed>,
							Or<mpstatus, Equal<MailStatusListAttribute.canceled>>>>, ActivityStatusAttribute.canceled>>>>>,
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

		[EPEndDate(typeof(classID), typeof(startDate), AllDayField = typeof(allDay), BqlField = typeof(CRActivity.endDate))]
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

		#region OwnerID
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
		[PXFormula(typeof(Default<workgroupID>))]
		public override Guid? OwnerID { get; set; }
		#endregion

		#region WorkgroupID
		public abstract new class workgroupID : IBqlField { }

		[PXDBInt(BqlField = typeof(CRActivity.workgroupID))]
		[PXChildUpdatable(UpdateRequest = true)]
		[PXUIField(DisplayName = "Workgroup")]
		[PXSelector(typeof(Search<EPCompanyTree.workGroupID>), 
			SubstituteKey = typeof(EPCompanyTree.description), 
			DescriptionField = typeof(EPCompanyTree.description))]
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
		public override bool? Incoming
		{
			[PXDependsOnFields(typeof(isIncome))]
			get
			{
				return IsIncome;
			}
		}
		#endregion

		#region Outgoing
		public abstract new class outgoing : IBqlField { }

		[PXDBBool(BqlField = typeof(CRActivity.outgoing))]
		[PXFormula(typeof(Switch<Case<Where<type, IsNotNull>, Selector<type, EPActivityType.outgoing>>, False>))]
		[PXUIField(DisplayName = "Outgoing")]
		public override bool? Outgoing
		{
			[PXDependsOnFields(typeof(isIncome))]
			get
			{
				return !IsIncome;
			}
		}
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

		#region SMEmail

		#region EmailNoteID
		public abstract class emailNoteID : IBqlField { }

		[PXDBSequentialGuid(BqlField = typeof(SMEmail.noteID))]
		[PXExtraKey]
		public virtual Guid? EmailNoteID { get; set; }
		#endregion

		#region EmailRefNoteID
		public abstract class emailRefNoteID : IBqlField { }

		[PXDBGuid(BqlField = typeof (SMEmail.refNoteID))]
		[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing, DefaultForUpdate = false)]
		public virtual Guid? EmailRefNoteID
		{
			[PXDependsOnFields(typeof(noteID))]
			get
			{
				return NoteID;
			}
		}
		#endregion

		#region EmailSubject
		public abstract class emailSubject : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true, BqlField = typeof (SMEmail.subject))]
		[PXDefault]
		[PXUIField(DisplayName = "Summary", Visibility = PXUIVisibility.SelectorVisible)]
		[PXNavigateSelector(typeof (subject))]
		public virtual string EmailSubject
		{
			[PXDependsOnFields(typeof (subject))]
			get
			{
				return Subject;
			}
		}

		#endregion

		#region Body
		public abstract new class body : IBqlField { }

		[PXDBText(IsUnicode = true, BqlField = typeof(SMEmail.body))]
		[PXUIField(DisplayName = "Activity Details")]
		public override string Body { get; set; }
		#endregion
		
		#region ImcUID
		public abstract class imcUID : IBqlField { }

		[PXDBGuid(true, BqlField = typeof(SMEmail.imcUID))]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual Guid? ImcUID
		{
			get { return _ImcUID ?? (_ImcUID = Guid.NewGuid()); }
			set { _ImcUID = value; }
		}
		protected Guid? _ImcUID;
		#endregion

		#region Pop3UID
		public abstract class pop3UID : IBqlField { }

		[PXDBString(150, BqlField = typeof(SMEmail.pop3UID))]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual string Pop3UID { get; set; }
		#endregion

		#region ImapUID
		public abstract class imapUID : IBqlField { }

		[PXDBInt(BqlField = typeof(SMEmail.imapUID))]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual int? ImapUID { get; set; }
		#endregion

		#region MailAccountID
		public abstract class mailAccountID : IBqlField { }

		[PXDBInt(BqlField = typeof(SMEmail.mailAccountID))]
		[PXUIField(DisplayName = "From")]
		public int? MailAccountID { get; set; }

		#endregion

		#region MailFrom
		public abstract class mailFrom : IBqlField { }

		[PXDBString(500, IsUnicode = true, BqlField = typeof(SMEmail.mailFrom))]
		[PXUIField(DisplayName = "From")]
		public virtual string MailFrom { get; set; }
		#endregion

		#region MailReply
		public abstract class mailReply : IBqlField { }

		[PXDBString(500, IsUnicode = true, BqlField = typeof(SMEmail.mailReply))]
		[PXUIField(DisplayName = "Reply")]
		public virtual string MailReply { get; set; }
		#endregion

		#region MailTo
		public abstract class mailTo : IBqlField { }

		[PXDBString(1000, IsUnicode = true, BqlField = typeof(SMEmail.mailTo))]
		[PXUIField(DisplayName = "To")]
		public virtual string MailTo { get; set; }
		#endregion

		#region MailCc
		public abstract class mailCc : IBqlField { }

		[PXDBString(1000, IsUnicode = true, BqlField = typeof(SMEmail.mailCc))]
		[PXUIField(DisplayName = "CC")]
		public virtual string MailCc { get; set; }
		#endregion

		#region MailBcc
		public abstract class mailBcc : IBqlField { }

		[PXDBString(1000, IsUnicode = true, BqlField = typeof(SMEmail.mailBcc))]
		[PXUIField(DisplayName = "BCC")]
		public virtual string MailBcc { get; set; }
		#endregion

		#region RetryCount
		public abstract class retryCount : IBqlField { }

		[PXDBInt(BqlField = typeof(SMEmail.retryCount))]
		[PXUIField(Visible = false)]
		[PXDefault(0)]
		public virtual int? RetryCount { get; set; }
		#endregion

		#region MessageId
		public abstract class messageId : IBqlField { }

		[PXDBString(255, BqlField = typeof(SMEmail.messageId))] // TODO: need review length
		[PXUIField(Visible = false)]
		public virtual string MessageId { get; set; }
		#endregion

		#region MessageReference
		public abstract class messageReference : IBqlField { }

		[PXDBString(BqlField = typeof(SMEmail.messageReference))]
		[PXUIField(Visible = false)]
		public virtual string MessageReference { get; set; }
		#endregion

		#region Exception
		public abstract class exception : IBqlField { }

		[PXDBString(IsUnicode = true, BqlField = typeof(SMEmail.exception))]
		[PXUIField(DisplayName = "Error Message")]
		public virtual string Exception { get; set; }
		#endregion

		#region Format
		public abstract class format : IBqlField { }

		[PXDefault(EmailFormatListAttribute.Html, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(255, BqlField = typeof(SMEmail.format))]
		[PXUIField(DisplayName = "Format")]
		[EmailFormatList]
		public virtual string Format { get; set; }
		#endregion

		#region ReportFormat
		public abstract class reportFormat : IBqlField { }

		[PXDBString(10, BqlField = typeof(SMEmail.reportFormat))]
		[PXUIField(DisplayName = "Format")]
		[PXStringList(
			new[] { "PDF", "HTML", "Excel" },
			new[] { "PDF", "HTML", "Excel" })]
		public virtual string ReportFormat { get; set; }
		#endregion

		#region ID
		public abstract class id : IBqlField { }

		[PXDBIdentity(BqlField = typeof(SMEmail.id))]
		[PXUIField(Visible = false)]
		public virtual int? ID { get; set; }
		#endregion

		#region Ticket
		public abstract class ticket : IBqlField { }

		[PXDBInt(BqlField = typeof(SMEmail.ticket))]
		[PXUIField(Visible = false)]
		public virtual int? Ticket { get; set; }
		#endregion

		#region IsIncome
		public abstract class isIncome : IBqlField { }

		[PXDBBool(BqlField = typeof(SMEmail.isIncome))]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Is Income")]
		public virtual bool? IsIncome { get; set; }
		#endregion



		#region EmailCreatedByID
		public abstract class emailCreatedByID : IBqlField { }

		[PXDBCreatedByID(DontOverrideValue = true, BqlField = typeof(SMEmail.createdByID))]
		[PXUIField(DisplayName = "Created By", Enabled = false)]
		public virtual Guid? EmailCreatedByID { get; set; }
		#endregion

		#region EmailCreatedByScreenID
		public abstract class emailCreatedByScreenID : IBqlField { }

		[PXDBCreatedByScreenID(BqlField = typeof(SMEmail.createdByScreenID))]
		public virtual string EmailCreatedByScreenID { get; set; }
		#endregion

		#region EmailCreatedDateTime
		public abstract class emailCreatedDateTime : IBqlField { }

		[PXUIField(DisplayName = "Created At", Enabled = false)]
		[PXDBCreatedDateTimeUtc(BqlField = typeof(SMEmail.createdDateTime))]
		public virtual DateTime? EmailCreatedDateTime { get; set; }
		#endregion

		#region EmailLastModifiedByID
		public abstract class emailLastModifiedByID : IBqlField { }

		[PXDBLastModifiedByID(BqlField = typeof(SMEmail.lastModifiedByID))]
		public virtual Guid? EmailLastModifiedByID { get; set; }
		#endregion

		#region EmailLastModifiedByScreenID
		public abstract class emailLastModifiedByScreenID : IBqlField { }

		[PXDBLastModifiedByScreenID(BqlField = typeof(SMEmail.lastModifiedByScreenID))]
		public virtual string EmailLastModifiedByScreenID { get; set; }
		#endregion

		#region EmailLastModifiedDateTime
		public abstract class emailLastModifiedDateTime : IBqlField { }

		[PXDBLastModifiedDateTimeUtc(BqlField = typeof(SMEmail.lastModifiedDateTime))]
		public virtual DateTime? EmailLastModifiedDateTime { get; set; }
		#endregion

		#endregion
	}
}
