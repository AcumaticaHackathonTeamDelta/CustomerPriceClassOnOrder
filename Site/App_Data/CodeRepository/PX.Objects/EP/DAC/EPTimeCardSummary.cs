using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PR;

namespace PX.Objects.EP
{
	[PXCacheName(Messages.TimeCardSummary)]
	[Serializable]
	public partial class EPTimeCardSummary : IBqlTable
	{
		#region TimeCardCD

		public abstract class timeCardCD : IBqlField { }

        [PXDBDefault(typeof(EPTimeCard.timeCardCD))]
		[PXDBString(10, IsKey = true)]
		[PXUIField(Visible = false)]
        [PXParent(typeof(Select<EPTimeCard, Where<EPTimeCard.timeCardCD, Equal<Current<EPTimeCardSummary.timeCardCD>>>>))]
		public virtual string TimeCardCD { get; set; }

		#endregion

		#region LineNbr
		public abstract class lineNbr : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(EPTimeCard.summaryLineCntr))]
		[PXUIField(Visible = false)]
		public virtual int? LineNbr { get; set; }
		#endregion

		#region EarningType
		public abstract class earningType : IBqlField { }

		[PXDBString(2, IsFixed = true, IsUnicode = false, InputMask=">LL")]
		[PXDefault(typeof(Search<EPSetup.regularHoursType>))]
		[PXSelector(typeof(EPEarningType.typeCD))]
		[PXUIField(DisplayName = "Earning Type")]
		public virtual string EarningType { get; set; }
		#endregion

		#region JobID
		public abstract class jobID : IBqlField { }
		
		[PXDBInt()]
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

		#region ParentNoteID
		public abstract class parentNoteID : IBqlField { }
		
		[PXUIField(DisplayName = "Task ID")]
		[PXDBGuid]
        [CRTaskSelector]
        [PXRestrictor(typeof(Where<CRActivity.ownerID, Equal<Current<AccessInfo.userID>>>), null)]
        public virtual Guid? ParentNoteID { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : IBqlField { }
		[ProjectDefault(BatchModule.EP, ForceProjectExplicitly = true)]
		[EPTimeCardProject]
		public virtual int? ProjectID { get; set; }
		#endregion

        #region ProjectDescription
        public abstract class projectDescription : IBqlField { }

        [PXFormula(typeof(Selector<projectID, PMProject.description>))]
        [PXUIField(DisplayName = "Project Description", IsReadOnly = true)]
        [PXString]
        public virtual string ProjectDescription { get; set; }
        #endregion

		#region ProjectTaskID
		public abstract class projectTaskID : IBqlField { }

		[EPTimecardProjectTask(typeof(projectID), BatchModule.EP, DisplayName = "Project Task")]
		public virtual int? ProjectTaskID { get; set; }
		#endregion

        #region ProjectTaskDescription
        public abstract class projectTaskDescription : IBqlField { }

        [PXUIField(DisplayName = "Project Task Description", IsReadOnly=true)]
        [PXFormula(typeof(Selector<projectTaskID, PMTask.description>))]
        [PXString]
        public virtual string ProjectTaskDescription { get; set; }
        #endregion

		#region TimeSpent
		public abstract class timeSpent : IBqlField { }

		[PXInt]
		[PXTimeList]
		[PXDependsOnFields(typeof(mon), typeof(tue), typeof(wed), typeof(thu), typeof(fri), typeof(sat), typeof(sun))]
		[PXUIField(DisplayName = "Time Spent", Enabled = false)]
		public virtual int? TimeSpent
		{
			get
			{
				return Mon.GetValueOrDefault() +
					   Tue.GetValueOrDefault() +
					   Wed.GetValueOrDefault() +
					   Thu.GetValueOrDefault() +
					   Fri.GetValueOrDefault() +
					   Sat.GetValueOrDefault() +
					   Sun.GetValueOrDefault();
			}
		}
		#endregion

		#region Sun
		public abstract class sun : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Sun")]
		public virtual int? Sun { get; set; }
		#endregion
		#region Mon
		public abstract class mon : IBqlField { }
		
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Mon")]
		public virtual int? Mon { get; set; }
		#endregion
		#region Tue
		public abstract class tue : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Tue")]
		public virtual int? Tue { get; set; }
		#endregion
		#region Wed
		public abstract class wed : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Wed")]
		public virtual int? Wed { get; set; }
		#endregion
		#region Thu
		public abstract class thu : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Thu")]
		public virtual int? Thu { get; set; }
		#endregion
		#region Fri
		public abstract class fri : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Fri")]
		public virtual int? Fri { get; set; }
		#endregion
		#region Sat
		public abstract class sat : IBqlField { }

        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Sat")]
		public virtual int? Sat { get; set; }
		#endregion
		
		#region IsBillable
		public abstract class isBillable : IBqlField { }
		
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "Billable")]
		public virtual bool? IsBillable { get; set; }
		#endregion

		#region Description
		public abstract class description : IBqlField { }
		
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual string Description { get; set; }
		#endregion
		
		#region ApprovalStatus
		public abstract class approvalStatus : IBqlField { }

		[PXString(2, IsFixed = true)]
		[ApprovalStatusList]
		[PXUIField(DisplayName = "Approval Status", Enabled = false)]
		public virtual string ApprovalStatus { get; set; }
		#endregion

		#region ApproverID
		public abstract class approverID : IBqlField { }

		[PXInt]
		[PXEPEmployeeSelector]
		[PXFormula(typeof(
			Switch<
				Case<Where<Selector<projectTaskID, PMTask.approverID>, IsNotNull>, Selector<projectTaskID, PMTask.approverID>>, 
				Case<Where<Selector<projectID, CT.Contract.approverID>, IsNotNull>, Selector<projectID, CT.Contract.approverID>>>
			))]
		[PXUIField(DisplayName = "Approver", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual int? ApproverID { get; set; }
		#endregion

		#region System Columns
		#region NoteID
		public abstract class noteID : IBqlField { }
		
		[PXNote()]
		public virtual Guid? NoteID { get; set; }
		#endregion
		#region tstamp
		public abstract class Tstamp : IBqlField { }
		
		[PXDBTimestamp()]
		public virtual byte[] tstamp { get; set; }
		#endregion
		#region CreatedByID
		public abstract class createdByID : IBqlField { }
		
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField { }
		
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField { }
		
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField { }
		
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField { }
		
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField { }
		
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion

        public int? GetTimeTotal(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return Mon;
                case DayOfWeek.Tuesday: return Tue;
                case DayOfWeek.Wednesday: return Wed;
                case DayOfWeek.Thursday: return Thu;
                case DayOfWeek.Friday: return Fri;
                case DayOfWeek.Saturday: return Sat;
                case DayOfWeek.Sunday: return Sun;

                default:
                    return null;
            }
        }

        public int? GetTimeTotal()
        {
            return Mon.GetValueOrDefault() + Tue.GetValueOrDefault() + Wed.GetValueOrDefault() + Thu.GetValueOrDefault() +
                   Fri.GetValueOrDefault() + Sat.GetValueOrDefault() + Sun.GetValueOrDefault();

        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", EarningType, Mon, Tue, Wed, Thu, Fri, Sat, Sun);
        }

	}
}
