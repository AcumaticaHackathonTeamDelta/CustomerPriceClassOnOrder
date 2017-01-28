using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.SM;
using PX.TM;

namespace PX.Objects.WZ
{
    [Serializable]
    [PXPrimaryGraph(typeof(WZScenarioEntry))]
    [PXCacheName("Wizard Scenario")]
    [PXEMailSource]
    [PX.Objects.GL.TableAndChartDashboardType]
	[PXHidden]
    public partial class WZScenario : IBqlTable, PX.Data.EP.IAssign
    {
        #region ScenraioID
        public abstract class scenarioID : IBqlField { }

        protected Guid? _ScenarioID;
        [PXDBGuid(IsKey = true)]
        [PXUIField(DisplayName = "Scenario ID", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Guid? ScenarioID 
        {
            get 
            { 
                return this._ScenarioID; 
            }
            set 
            { 
                this._ScenarioID = value; 
            }
        }
        #endregion
        #region Name
        public abstract class name : IBqlField { }

        protected String _Name;
        [PXDBString(50, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Scenario Name", Visibility=PXUIVisibility.SelectorVisible)]
        public virtual String Name 
        {
            get 
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }

        }
        #endregion
        #region Status
        public abstract class status : IBqlField { }

        protected String _Status;
        [PXDBString(2, IsFixed=true)]
        [WizardScenarioStatuses]
        [PXDefault(WizardScenarioStatusesAttribute._PENDING)]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
        #region ExecutionDate
        public abstract class executionDate : IBqlField { }

        protected DateTime? _ExecutionDate;
        [PXDBDateAndTime]
        [PXUIField(DisplayName = "Execution Date", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual DateTime? ExecutionDate
        {
            get 
            {
                return this._ExecutionDate;
            }
            set
            {
                this._ExecutionDate = value;
            }
        }
        #endregion
        #region ExecutionPeriodID
        public abstract class executionPeriodID : PX.Data.IBqlField
        {
        }
        protected String _ExecutionPeriodID;
        [WZFinPeriod(typeof(WZScenario.executionDate), typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>, And<FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Execution Period", Enabled = false)]
        public virtual String ExecutionPeriodID
        {
            get
            {
                return this._ExecutionPeriodID;
            }
            set
            {
                this._ExecutionPeriodID = value;
            }
        }
        #endregion
        #region Rolename
        public abstract class rolename : IBqlField { }

        protected String _Rolename;
        [PXDBString(64, IsUnicode = true)]
        [PXUIField(DisplayName = "Role Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PX.SM.Roles.rolename, Where<PX.SM.Roles.guest, Equal<False>>>), DescriptionField = typeof(PX.SM.Roles.descr))]
        public virtual String Rolename
        {
            get 
            {
                return this._Rolename;
            }
            set
            {
                this._Rolename = value;
            }
        }
        #endregion
        #region NodeID
        public abstract class nodeID : IBqlField { }

        protected Guid? _NodeID;
        [PXDBGuid]
        [PXDefault]
        [PXUIField(DisplayName = "Site Map Location", Visibility = PXUIVisibility.SelectorVisible)]        
        public virtual Guid? NodeID
        {
            get
            {
                return this._NodeID;
            }
            set
            {
                this._NodeID = value;
            }
        }
        #endregion
        #region ScheduleID
        public abstract class scheduleID : IBqlField { }

        protected String _ScheduleID;

        [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Schedule ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXParent(typeof(Select<Schedule, Where<Schedule.scheduleID, Equal<Current<WZScenario.scheduleID>>>>), LeaveChildren = true)]
        public virtual String ScheduleID 
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
        #region Scheduled
        public abstract class scheduled : PX.Data.IBqlField
        {
        }
        protected Boolean? _Scheduled;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? Scheduled
        {
            get
            {
                return this._Scheduled;
            }
            set
            {
                this._Scheduled = value;
            }
        }
        #endregion
        #region ScenarioOrder
        public abstract class scenarioOrder : IBqlField { }

        protected int? _ScenarioOrder;
        [PXDBInt]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Order")]
        public virtual int? ScenarioOrder
        {
            get
            {
                return this._ScenarioOrder;
            }
            set
            {
                this._ScenarioOrder = value;
            }
        }
        #endregion


        #region AssignmentMapID
        public abstract class assignmentMapID : PX.Data.IBqlField
        {
        }
        protected int? _AssignmentMapID;
        [PXDBInt]
        [PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, 
                            Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeImplementationScenario>>>), SubstituteKey = typeof(EPAssignmentMap.name))]
        [PXUIField(DisplayName = "Assignment Map")]
        public virtual int? AssignmentMapID
        {
            get
            {
                return this._AssignmentMapID;
            }
            set
            {
                this._AssignmentMapID = value;
            }
        }
        #endregion
        #region WorkgroupID
        public abstract class workgroupID : PX.Data.IBqlField
        {
        }
        protected int? _WorkgroupID;
        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Workgroup ID", Enabled = false)]
        public virtual int? WorkgroupID
        {
            get
            {
                return this._WorkgroupID;
            }
            set
            {
                this._WorkgroupID = value;
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : IBqlField
        {
        }
        protected Guid? _OwnerID;
        [PXGuid()]
        [PX.TM.PXOwnerSelector]
        [PXUIField(DisplayName = "Assignee", Enabled = false)]
        public virtual Guid? OwnerID
        {
            get
            {
                return this._OwnerID;
            }
            set
            {
                this._OwnerID = value;
            }
        }
        #endregion

        #region
        public abstract class tasksCompleted : IBqlField {}

        protected String _TasksCompleted;

        [PXString]
        [PXUIField(DisplayName = "Tasks Completed")]
        public virtual String TasksCompleted
        {
            get
            {
                return this._TasksCompleted;
            }
            set
            {
                this._TasksCompleted = value;
            }
        }
        #endregion

        #region IAssign Members

        int? PX.Data.EP.IAssign.WorkgroupID
        {
            get { return WorkgroupID; }
            set { WorkgroupID = value; }
        }

        Guid? PX.Data.EP.IAssign.OwnerID
        {
            get { return OwnerID; }
            set { OwnerID = value; }
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

    }
}
