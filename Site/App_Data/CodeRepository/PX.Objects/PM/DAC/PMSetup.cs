using PX.Data;
using System;
using PX.Objects.CS;
using PX.Objects.EP;

namespace PX.Objects.PM
{
	
	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(SetupMaint))]
    [PXCacheName(Messages.PMSetupMaint)]
	public partial class PMSetup : PX.Data.IBqlTable, IAssignedMap
	{
		public const string DefaultNonProjectCode = "X";

		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXUIField(Visible=false)]
		public virtual Boolean? IsActive
		{
            get { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }			
		}
		#endregion		
		#region NonProjectCode
		public abstract class nonProjectCode : PX.Data.IBqlField
		{
		}
		protected string _NonProjectCode;
		[PXDefault(DefaultNonProjectCode)]
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Non-Project Code")]
		public virtual string NonProjectCode
		{
			get
			{
				return this._NonProjectCode;
			}
			set
			{
				this._NonProjectCode = value;
			}
		}
		#endregion
		#region TranNumbering
		public abstract class tranNumbering : PX.Data.IBqlField
		{
		}
		protected String _TranNumbering;
		[PXDefault("PMTRAN")]
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Transaction Numbering Sequence")]
		public virtual String TranNumbering
		{
			get
			{
				return this._TranNumbering;
			}
			set
			{
				this._TranNumbering = value;
			}
		}
		#endregion
		#region AutoPost
		public abstract class autoPost : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoPost;
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName = "Automatically Post on Release")]
		public virtual Boolean? AutoPost
		{
			get
			{
				return this._AutoPost;
			}
			set
			{
				this._AutoPost = value;
			}
		}
		#endregion
		#region AutoReleaseAllocation
		public abstract class autoReleaseAllocation : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoReleaseAllocation;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Automatically Release Allocations")]
		public virtual Boolean? AutoReleaseAllocation
		{
			get
			{
				return this._AutoReleaseAllocation;
			}
			set
			{
				this._AutoReleaseAllocation = value;
			}
		}
		#endregion
		#region BatchNumberingID
		public abstract class batchNumberingID : IBqlField
		{
		}
		protected String _BatchNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("BATCH")]
		[PXUIField(DisplayName = "Batch Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String BatchNumberingID
		{
			get
			{
				return this._BatchNumberingID;
			}
			set
			{
				this._BatchNumberingID = value;
			}
		}
		#endregion
		#region ExpenseAccountSource
		public abstract class expenseAccountSource : PX.Data.IBqlField
		{
		}
		protected String _ExpenseAccountSource;
		[PXDBString(1, IsFixed = true)]
		[PMExpenseAccountSource.List()]
		[PXDefault(PMExpenseAccountSource.InventoryItem)]
		[PXUIField(DisplayName = "Expense Account Source", Required = true)]
		public virtual String ExpenseAccountSource
		{
			get
			{
				return this._ExpenseAccountSource;
			}
			set
			{
				this._ExpenseAccountSource = value;
			}
		}
		#endregion
		#region ExpenseSubMask
		public abstract class expenseSubMask : PX.Data.IBqlField
		{
		}
		protected String _ExpenseSubMask;
		[PXDefault]
        [SubAccountMaskAttribute(DisplayName = "Expense Sub. Source")]
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
        #region ExpenseAccrualAccountSource
        public abstract class expenseAccrualAccountSource : PX.Data.IBqlField
        {
        }
        protected String _ExpenseAccrualAccountSource;
        [PXDBString(1, IsFixed = true)]
        [PMExpenseAccountSource.AccrualList()]
        [PXDefault(PMExpenseAccountSource.InventoryItem)]
        [PXUIField(DisplayName = "Expense Accrual Account Source", Required = true)]
        public virtual String ExpenseAccrualAccountSource
        {
            get
            {
                return this._ExpenseAccrualAccountSource;
            }
            set
            {
                this._ExpenseAccrualAccountSource = value;
            }
        }
        #endregion
        #region ExpenseAccrualSubMask
        public abstract class expenseAccrualSubMask : PX.Data.IBqlField
        {
        }
        protected String _ExpenseAccrualSubMask;
        [PXDefault]
        [SubAccountMaskAttribute(DisplayName = "Expense Accrual Sub. Source")]
        public virtual String ExpenseAccrualSubMask
        {
            get
            {
                return this._ExpenseAccrualSubMask;
            }
            set
            {
                this._ExpenseAccrualSubMask = value;
            }
        }
        #endregion
		#region AssignmentMapID
		public abstract class assignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _AssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeProject>>>))]
		[PXUIField(DisplayName = "Project Approval Map")]
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
        #region AssignmentNotificationID
        public abstract class  assignmentNotificationID : PX.Data.IBqlField
        {
        }
        protected int? _AssignmentNotificationID;
        [PXDBInt]
        [PXSelector(typeof(Search<PX.SM.Notification.notificationID>))]
        [PXUIField(DisplayName = "Pending Approval Notification")]
        public virtual int? AssignmentNotificationID
        {
            get
            {
                return this._AssignmentNotificationID;
            }
            set
            {
                this._AssignmentNotificationID = value;
            }
        }
        #endregion
		#region CutoffDate
		public abstract class cutoffDate : PX.Data.IBqlField
		{
		}
		protected String _CutoffDate;
		[PXDBString(1, IsFixed = true)]
		[PMCutOffDate.List()]
		[PXDefault(PMCutOffDate.Included)]
		[PXUIField(DisplayName = "Billing Cut-off Date", Required = true)]
		public virtual String CutoffDate
		{
			get
			{
				return this._CutoffDate;
			}
			set
			{
				this._CutoffDate = value;
			}
		}
		#endregion
		#region VisibleInGL
		public abstract class visibleInGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInGL;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "GL")]
		public virtual Boolean? VisibleInGL
		{
			get
			{
				return this._VisibleInGL;
			}
			set
			{
				this._VisibleInGL = value;
			}
		}
		#endregion
		#region VisibleInAP
		public abstract class visibleInAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "AP")]
		public virtual Boolean? VisibleInAP
		{
			get
			{
				return this._VisibleInAP;
			}
			set
			{
				this._VisibleInAP = value;
			}
		}
		#endregion
		#region VisibleInAR
		public abstract class visibleInAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "AR")]
		public virtual Boolean? VisibleInAR
		{
			get
			{
				return this._VisibleInAR;
			}
			set
			{
				this._VisibleInAR = value;
			}
		}
		#endregion
		#region VisibleInSO
		public abstract class visibleInSO : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInSO;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "SO")]
		public virtual Boolean? VisibleInSO
		{
			get
			{
				return this._VisibleInSO;
			}
			set
			{
				this._VisibleInSO = value;
			}
		}
		#endregion
		#region VisibleInPO
		public abstract class visibleInPO : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInPO;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "PO")]
		public virtual Boolean? VisibleInPO
		{
			get
			{
				return this._VisibleInPO;
			}
			set
			{
				this._VisibleInPO = value;
			}
		}
		#endregion
		#region VisibleInEP
		public abstract class visibleInEP : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInEP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "TE")]
		public virtual Boolean? VisibleInEP
		{
			get
			{
				return this._VisibleInEP;
			}
			set
			{
				this._VisibleInEP = value;
			}
		}
		#endregion
		#region VisibleInIN
		public abstract class visibleInIN : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInIN;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "IN")]
		public virtual Boolean? VisibleInIN
		{
			get
			{
				return this._VisibleInIN;
			}
			set
			{
				this._VisibleInIN = value;
			}
		}
		#endregion
		#region VisibleInCA
		public abstract class visibleInCA : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInCA;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "CA")]
		public virtual Boolean? VisibleInCA
		{
			get
			{
				return this._VisibleInCA;
			}
			set
			{
				this._VisibleInCA = value;
			}
		}
		#endregion
		#region VisibleInCR
		public abstract class visibleInCR : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInCR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "CR")]
		public virtual Boolean? VisibleInCR
		{
			get
			{
				return this._VisibleInCR;
			}
			set
			{
				this._VisibleInCR = value;
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

	public static class PMExpenseAccountSource
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { InventoryItem, Project, Task, Employee },
				new string[] { Messages.AccountSource_LaborItem, Messages.AccountSource_Project, Messages.AccountSource_Task, Messages.AccountSource_Resource }) { ; }
		}

        public class AccrualListAttribute : PXStringListAttribute
        {
            public AccrualListAttribute()
                : base(
                new string[] { InventoryItem, Project, Task },
                new string[] { Messages.AccountSource_LaborItem_Accrual, Messages.AccountSource_ProjectAccrual, Messages.AccountSource_Task_Accrual }) { ; }
        }

		public const string Project = "P";
		public const string Task = "T";
		public const string InventoryItem = "I";
		public const string Employee = "E";
	}

	public static class PMCutOffDate
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Included, Excluded },
				new string[] { Messages.Included, Messages.Excluded }) { ; }
		}

		public const string Included = "I";
		public const string Excluded = "E";
	}
}
