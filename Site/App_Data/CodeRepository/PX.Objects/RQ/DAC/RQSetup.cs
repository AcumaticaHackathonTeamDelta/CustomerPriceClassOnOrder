namespace PX.Objects.RQ
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.EP;
	using PX.Objects.GL;
	using PX.Objects.PO;

	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(RQSetupMaint))]
    [PXCacheName(Messages.RQSetupMaint)]
    public partial class RQSetup : PX.Data.IBqlTable
	{
		#region RequestNumberingID
		public abstract class requestNumberingID : PX.Data.IBqlField
		{
		}
		protected String _RequestNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("RQITEM")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String RequestNumberingID
		{
			get
			{
				return this._RequestNumberingID;
			}
			set
			{
				this._RequestNumberingID = value;
			}
		}
		#endregion									
		#region RequestApproval
		public abstract class requestApproval : PX.Data.IBqlField
		{
		}
		protected bool? _RequestApproval;
        [EPRequireApproval]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Require Approval")]
		public virtual bool? RequestApproval
		{
			get
			{
				return this._RequestApproval;
			}
			set
			{
				this._RequestApproval = value;
			}
		}
		#endregion		
		#region RequestAssignmentMapID
		public abstract class requestAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _RequestAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypePurchaseRequestItem>>>))]
		[PXUIField(DisplayName = "Assignment Map")]
		public virtual int? RequestAssignmentMapID
		{
			get
			{
				return this._RequestAssignmentMapID;
			}
			set
			{
				this._RequestAssignmentMapID = value;
			}
		}
		#endregion		
		#region RequestOverBudgetWarning
		public abstract class requestOverBudgetWarning : PX.Data.IBqlField
		{
		}
		protected String _RequestOverBudgetWarning;
		[PXDBString(1, IsFixed = true)]
		[POReceiptQtyAction.List()]
		[PXDefault(POReceiptQtyAction.AcceptButWarn)]
		[PXUIField(DisplayName = "Over Budget Warning")]
		public virtual String RequestOverBudgetWarning
		{
			get
			{
				return this._RequestOverBudgetWarning;
			}
			set
			{
				this._RequestOverBudgetWarning = value;
			}
		}
		#endregion
		#region MonthRetainRequest
		public abstract class monthRetainRequest : PX.Data.IBqlField
		{
		}
		protected int? _MonthRetainRequest;
		[PXDBInt]
		[PXDefault(3)]
		[PXUIField(DisplayName = "Months Retained")]
		public virtual int? MonthRetainRequest
		{
			get
			{
				return this._MonthRetainRequest;
			}
			set
			{
				this._MonthRetainRequest = value;
			}
		}
		#endregion
		#region RequisitionNumberingID
		public abstract class requisitionNumberingID : PX.Data.IBqlField
		{
		}
		protected String _RequisitionNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("RQREQUISIT")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String RequisitionNumberingID
		{
			get
			{
				return this._RequisitionNumberingID;
			}
			set
			{
				this._RequisitionNumberingID = value;
			}
		}
		#endregion
		#region RequisitionApproval
		public abstract class requisitionApproval : PX.Data.IBqlField
		{
		}
		protected bool? _RequisitionApproval;		
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Require Approval")]		
		public virtual bool? RequisitionApproval
		{
			get
			{
				return this._RequisitionApproval;
			}
			set
			{
				this._RequisitionApproval = value;
			}
		}
		#endregion				
		#region RequisitionAssignmentMapID
		public abstract class requisitionAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _RequisitionAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypePurchaseRequisition>>>))]
		[PXUIField(DisplayName = "Assignment Map")]
		public virtual int? RequisitionAssignmentMapID
		{
			get
			{
				return this._RequisitionAssignmentMapID;
			}
			set
			{
				this._RequisitionAssignmentMapID = value;
			}
		}
		#endregion
		#region RequisitionOverBudgetWarning
		public abstract class requisitionOverBudgetWarning : PX.Data.IBqlField
		{
		}
		protected String _RequisitionOverBudgetWarning;
		[PXDBString(1, IsFixed = true)]
		[POReceiptQtyAction.List()]
		[PXDefault(POReceiptQtyAction.AcceptButWarn)]
		[PXUIField(DisplayName = "Over Budget Warning")]
		public virtual String RequisitionOverBudgetWarning
		{
			get
			{
				return this._RequisitionOverBudgetWarning;
			}
			set
			{
				this._RequisitionOverBudgetWarning = value;
			}
		}
		#endregion		
		#region RequisitionMergeLines
		public abstract class requisitionMergeLines : PX.Data.IBqlField
		{
		}
		protected bool? _RequisitionMergeLines;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Merge Lines by Default")]
		public virtual bool? RequisitionMergeLines
		{
			get
			{
				return this._RequisitionMergeLines;
			}
			set
			{
				this._RequisitionMergeLines = value;
			}
		}
		#endregion				
		#region MonthRetainRequisition
		public abstract class monthRetainRequisition : PX.Data.IBqlField
		{
		}
		protected int? _MonthRetainRequisition;
		[PXDBInt]
		[PXDefault(3)]
		[PXUIField(DisplayName = "Months Retained")]
		public virtual int? MonthRetainRequisition
		{
			get
			{
				return this._MonthRetainRequisition;
			}
			set
			{
				this._MonthRetainRequisition = value;
			}
		}
		#endregion
		#region POHold
		public abstract class pOHold : PX.Data.IBqlField
		{
		}
		protected bool? _POHold;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Create Purchase Order on Hold")]		
		public bool? POHold
		{
			get
			{
				return this._POHold;
			}
			set
			{
				this._POHold = value;
			}
		}
		#endregion

		#region BudgetLedgerId
		public abstract class budgetLedgerId : PX.Data.IBqlField
		{
		}
		protected Int32? _BudgetLedgerId;
		[PXDBInt()]
		[PXUIField(DisplayName = "Budget Ledger", Required = true)]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, Equal<BudgetLedger>>>),
			SubstituteKey = typeof(Ledger.ledgerCD),
			DescriptionField = typeof(Ledger.descr))]
		[PXDefault]
		public virtual Int32? BudgetLedgerId
		{
			get
			{
				return this._BudgetLedgerId;
			}
			set
			{
				this._BudgetLedgerId = value;
			}
		}
		#endregion
		#region BudgetCalculation
		public abstract class budgetCalculation : PX.Data.IBqlField
		{
		}
		protected string _BudgetCalculation;		
		[PXDBString(1, IsFixed = true)]
		[PXDefault(RQBudgetCalculationType.YTD)]
		[PXUIField(DisplayName = "Budget Calculation")]
		[RQBudgetCalculationType.List]
		public virtual string BudgetCalculation
		{
			get
			{
				return this._BudgetCalculation;
			}
			set
			{
				this._BudgetCalculation = value;
			}
		}
		#endregion						
		#region DefaultReqClassID
		public abstract class defaultReqClassID : PX.Data.IBqlField
		{
		}
		protected String _DefaultReqClassID;
		[PXDBString(10, IsUnicode = true)]
		//[PXDefault("DEFAULT")]
		[PXUIField(DisplayName = "Default Request Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(RQRequestClass.reqClassID), DescriptionField = typeof(RQRequestClass.descr), DirtyRead=true)]
		public virtual String DefaultReqClassID
		{
			get
			{
				return this._DefaultReqClassID;
			}
			set
			{
				this._DefaultReqClassID = value;
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
	}

	public static class RQAccountSource
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { None, Requester, Department, RequestClass, PurchaseItem },
				new string[] { Messages.None, Messages.Requester, Messages.Department, Messages.RequestClass, Messages.PurchaseItem }) { }
		}

		public class department : Constant<string>
		{
			public department()
				:base(Department)
			{
			}
		}

		public class purchaseItem : Constant<string>
		{
			public purchaseItem()
				: base(PurchaseItem)
			{
			}
		}
		public const string None = "N";
		public const string Department = "D";
		public const string Requester = "R";
		public const string PurchaseItem = "I";
		public const string RequestClass = "Q";
	}

	public static class RQBudgetCalculationType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { YTD, PTD, Annual},
				new string[] { Messages.YTDValues, Messages.PTDValues, Messages.Annual }) { ; }
		}
		public const string YTD = "Y";
		public const string PTD = "P";		
		public const string Annual = "A";		
	}
}
