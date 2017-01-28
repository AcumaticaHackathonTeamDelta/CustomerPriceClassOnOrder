namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CS;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]	

	[PXPrimaryGraph(new Type[] {
					typeof(INReceiptEntry),
					typeof(INIssueEntry),
					typeof(INTransferEntry),
					typeof(INAdjustmentEntry),
					typeof(KitAssemblyEntry),
					typeof(KitAssemblyEntry)},
					new Type[] {
					typeof(Where<INRegister.docType, Equal<INDocType.receipt>>),
					typeof(Where<INRegister.docType, Equal<INDocType.issue>>),
					typeof(Where<INRegister.docType, Equal<INDocType.transfer>>),
					typeof(Where<INRegister.docType, Equal<INDocType.adjustment>>),
					typeof(Select<INKitRegister, Where<INKitRegister.docType, Equal<INDocType.production>, And<INKitRegister.refNbr, Equal<Current<INRegister.refNbr>>>>>),
					typeof(Select<INKitRegister, Where<INKitRegister.docType, Equal<INDocType.disassembly>, And<INKitRegister.refNbr, Equal<Current<INRegister.refNbr>>>>>),
					})]
	[INRegisterCacheName(Messages.Register)]
	public partial class INRegister : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
			public const string DisplayName = "Document Type";
		}
		protected String _DocType;
		[PXDBString(1, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[INDocType.List()]
		[PXUIField(DisplayName = docType.DisplayName, Enabled = false, Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
			public const string DisplayName = "Reference Nbr.";
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName= refNbr.DisplayName, Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INRegister.refNbr, Where<INRegister.docType, Equal<Optional<INRegister.docType>>>, OrderBy<Desc<INRegister.refNbr>>>), Filterable = true)]
		[INDocType.Numbering()]
		[PX.Data.EP.PXFieldDescription]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
            public const string PI = "PI";

            public class List : PXStringListAttribute
            {
                public List()
					: base(new string[] { GL.BatchModule.SO, GL.BatchModule.PO, GL.BatchModule.IN, PI, GL.BatchModule.AP },
					new string[] { GL.Messages.ModuleSO, GL.Messages.ModulePO, GL.Messages.ModuleIN, Messages.ModulePI, GL.Messages.ModuleAP })
                { 
                }
            }
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(GL.BatchModule.IN)]
		[PXUIField(DisplayName = "Source", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[origModule.List()]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
        #region OrigRefNbr
        public abstract class origRefNbr : PX.Data.IBqlField
        {
        }
        protected String _OrigRefNbr;
        [PXString(15, IsUnicode = true)]
        public virtual String OrigRefNbr
        {
            get
            {
                return this._OrigRefNbr;
            }
            set
            {
                this._OrigRefNbr = value;
            }
        }
        #endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse ID", DescriptionField=typeof(INSite.descr))]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region ToSiteID
		public abstract class toSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToSiteID;
		[IN.ToSite(DisplayName = "To Warehouse ID",DescriptionField = typeof(INSite.descr))]
		public virtual Int32? ToSiteID
		{
			get
			{
				return this._ToSiteID;
			}
			set
			{
				this._ToSiteID = value;
			}
		}
		#endregion
		#region TransferType
		public abstract class transferType : PX.Data.IBqlField
		{
		}
		protected String _TransferType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INTransferType.OneStep)]
		[INTransferType.List()]
		[PXUIField(DisplayName = "Transfer Type")]
		public virtual String TransferType
		{
			get
			{
				return this._TransferType;
			}
			set
			{
				this._TransferType = value;
			}
		}
		#endregion
		#region TransferNbr
		public abstract class transferNbr : PX.Data.IBqlField
		{
		}
		protected String _TransferNbr;
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Transfer Nbr.")]
		public virtual String TransferNbr
		{
			get
			{
				return this._TransferNbr;
			}
			set
			{
				this._TransferNbr = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(INSetup.holdEntry))]
		[PXUIField(DisplayName="Hold")]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Status", Visibility=PXUIVisibility.SelectorVisible, Enabled=false)]
		[INDocStatus.List()]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(released), typeof(hold))]
			get
			{
				return this._Status;
			}
			set
			{
				//this._Status = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[INOpenPeriod(typeof(INRegister.tranDate))]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[TranPeriodID(typeof(INRegister.tranDate))]
		public virtual String TranPeriodID
		{
			get
			{
				return this._TranPeriodID;
			}
			set
			{
				this._TranPeriodID = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
		#region TotalQty
		public abstract class totalQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Qty.", Visibility=PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual Decimal? TotalQty
		{
			get
			{
				return this._TotalQty;
			}
			set
			{
				this._TotalQty = value;
			}
		}
		#endregion
		#region TotalAmount
		public abstract class totalAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalAmount;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Amount", Visibility=PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual Decimal? TotalAmount
		{
			get
			{
				return this._TotalAmount;
			}
			set
			{
				this._TotalAmount = value;
			}
		}
		#endregion
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
		#region ControlQty
		public abstract class controlQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Control Qty.")]
		public virtual Decimal? ControlQty
		{
			get
			{
				return this._ControlQty;
			}
			set
			{
				this._ControlQty = value;
			}
		}
		#endregion
		#region ControlAmount
		public abstract class controlAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlAmount;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Amount")]
		public virtual Decimal? ControlAmount
		{
			get
			{
				return this._ControlAmount;
			}
			set
			{
				this._ControlAmount = value;
			}
		}
		#endregion
		#region ControlCost
		public abstract class controlCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Cost")]
		public virtual Decimal? ControlCost
		{
			get
			{
				return this._ControlCost;
			}
			set
			{
				this._ControlCost = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleIN>>>))]
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
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "External Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ExtRefNbr
		{
			get
			{
				return this._ExtRefNbr;
			}
			set
			{
				this._ExtRefNbr = value;
			}
		}
		#endregion			
		#region KitInventoryID
		public abstract class kitInventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _KitInventoryID;
		[PXDBInt]
		public virtual Int32? KitInventoryID
		{
			get
			{
				return this._KitInventoryID;
			}
			set
			{
				this._KitInventoryID = value;
			}
		}
		#endregion
		#region KitRevisionID
		public abstract class kitRevisionID : PX.Data.IBqlField
		{
		}
		protected String _KitRevisionID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		public virtual String KitRevisionID
		{
			get
			{
				return this._KitRevisionID;
			}
			set
			{
				this._KitRevisionID = value;
			}
		}
		#endregion
		#region KitLineNbr
		public abstract class kitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _KitLineNbr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? KitLineNbr
		{
			get
			{
				return this._KitLineNbr;
			}
			set
			{
				this._KitLineNbr = value;
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.IN, "{0}: {1}", new Type[] { typeof(INRegister.docType), typeof(INRegister.refNbr) },
			new Type[] { typeof(INRegister.tranDesc), typeof(INRegister.extRefNbr), typeof(INRegister.transferNbr) },
			NumberFields = new Type[] { typeof(INRegister.refNbr) },
			Line1Format = "{0}{1:d}{2}{3}{4}", Line1Fields = new Type[] { typeof(INRegister.extRefNbr), typeof(INRegister.tranDate), typeof(INRegister.transferType), typeof(INRegister.transferNbr), typeof(INRegister.status) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(INRegister.tranDesc) },
			WhereConstraint = typeof(Where<INRegister.docType, NotEqual<INDocType.production>, And<INRegister.docType, NotEqual<INDocType.disassembly>>>)
		)]
		[PXNote(DescriptionField = typeof(INRegister.refNbr), 
			Selector = typeof(INRegister.refNbr))]
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
		#region IsPPVTran
		public abstract class isPPVTran : IBqlField
		{
		}
		protected bool? _IsPPVTran = false;
		[PXBool]
		[PXDefault(false)]
		public virtual bool? IsPPVTran
		{
			get
			{
				return _IsPPVTran;
			}
			set
			{
				_IsPPVTran = value;
			}
		}
		#endregion
		#region Methods
		protected virtual void SetStatus()
		{
			if (this._Hold != null && (bool)this._Hold)
			{
				this._Status = INDocStatus.Hold;
			}
			else if (this._Released != null && this._Released == false)
			{
				this._Status = INDocStatus.Balanced;
			}
			else 
			{
				this._Status = INDocStatus.Released;
			}
		}
		#endregion
	}

    [PXProjection(typeof(Select4<INItemPlan, Where<INItemPlan.planType, Equal<INPlanConstants.plan42>, Or<INItemPlan.planType, Equal<INPlanConstants.plan44>>>, Aggregate<GroupBy<INItemPlan.refNoteID>>>))]
    [Serializable]
    [PXHidden]
	public partial class INTransferInTransit : IBqlTable
	{
		#region RefNoteID
		public abstract class refNoteID : PX.Data.IBqlField
		{
		}
		protected Guid? _RefNoteID;
		[PXDBGuid(BqlField=typeof(INItemPlan.refNoteID))]
		public virtual Guid? RefNoteID
		{
			get
			{
				return this._RefNoteID;
			}
			set
			{
				this._RefNoteID = value;
			}
		}
		#endregion
	}

    [PXProjection(typeof(Select5<INItemPlan, InnerJoin<INTranSplit, On<INTranSplit.planID, Equal<INItemPlan.planID>>>, 
        Where<INItemPlan.planType, Equal<INPlanConstants.plan42>, Or<INItemPlan.planType, Equal<INPlanConstants.plan44>>>, Aggregate<GroupBy<INTranSplit.tranType, GroupBy<INTranSplit.refNbr, GroupBy<INTranSplit.lineNbr>>>>>))]
    [Serializable]
    [PXHidden]
    public partial class INTranInTransit : IBqlTable
    {
        #region TranType
        public abstract class tranType : PX.Data.IBqlField
        {
        }
        protected String _TranType;
        [PXDBString(3, IsFixed = true, BqlField = typeof(INTranSplit.tranType))]
        public virtual String TranType
        {
            get
            {
                return this._TranType;
            }
            set
            {
                this._TranType = value;
            }
        }
        #endregion
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        protected String _RefNbr;
        [PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(INTranSplit.refNbr))]
        public virtual String RefNbr
        {
            get
            {
                return this._RefNbr;
            }
            set
            {
                this._RefNbr = value;
            }
        }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;
        [PXDBInt(IsKey = true, BqlField = typeof(INTranSplit.lineNbr))]
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
        #region PlanType
        public abstract class planType : IBqlField { }
        [PXDBString(2, IsFixed = true, BqlField = typeof(INItemPlan.planType))]
        public string PlanType
        {
            get;
            set;
        }
        #endregion 
    }


	public class INDocType
	{
		public class NumberingAttribute : AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(INRegister.docType), typeof(INRegister.tranDate),
					new string[] { Issue, Receipt, Transfer, Adjustment, Production, Change, Disassembly },
					new Type[] { typeof(INSetup.issueNumberingID), typeof(INSetup.receiptNumberingID), typeof(INSetup.receiptNumberingID), typeof(INSetup.adjustmentNumberingID), typeof(INSetup.kitAssemblyNumberingID), typeof(INSetup.kitAssemblyNumberingID), typeof(INSetup.kitAssemblyNumberingID) }) { ; }
		}
		
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { Issue, Receipt, Transfer, Adjustment, Production, Disassembly },
			new string[] { Messages.Issue, Messages.Receipt, Messages.Transfer, Messages.Adjustment, Messages.Production, Messages.Disassembly }) { }
		}

		public class KitListAttribute : PXStringListAttribute
		{
			public KitListAttribute()
				: base(
			new string[] { Production, Disassembly },
			new string[] { Messages.Production, Messages.Disassembly }) { }
		}

		public class SOListAttribute : PXStringListAttribute
		{
			public SOListAttribute()
				: base(
			new string[] { Issue, Receipt, Transfer, Adjustment, Production, Disassembly, DropShip },
			new string[] { Messages.Issue, Messages.Receipt, Messages.Transfer, Messages.Adjustment, Messages.Production, Messages.Disassembly, Messages.DropShip }) { }
		}

		public const string Undefined = "0";
		public const string Issue = "I";
		public const string Receipt = "R";
		public const string Transfer = "T";
		public const string Adjustment = "A";
		public const string Production = "P";
		public const string Change = "C";
		public const string Disassembly = "D";
		public const string DropShip = "H";

		public class undefined : Constant<string>
		{
			public undefined() : base(Undefined) { ;}
		}
		
		public class issue : Constant<string>
		{
			public issue() : base(Issue) { ;}
		}

		public class receipt : Constant<string>
		{
			public receipt() : base(Receipt) { ;}
		}

		public class transfer : Constant<string>
		{
			public transfer() : base(Transfer) { ;}
		}

		public class adjustment : Constant<string>
		{
			public adjustment() : base(Adjustment) { ;}
		}

		public class production : Constant<string>
		{
			public production() : base(Production) { ;}
		}
		public class change : Constant<string>
		{
			public change() : base(Change) { ;}
		}
		public class disassembly : Constant<string>
		{
			public disassembly() : base(Disassembly) { ;}
		}
		public class dropShip : Constant<string>
		{
			public dropShip() : base(DropShip) { ;}
		}
	}

	public class INDocStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Hold, Balanced, Released },
				new string[] { Messages.Hold, Messages.Balanced, Messages.Released }) { ; }
		}

		public const string Hold = "H";
		public const string Balanced = "B";
		public const string Released = "R";

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

	}

	public class INTransferType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { OneStep, TwoStep },
			new string[] { Messages.OneStep, Messages.TwoStep }) { }
		}

		public const string OneStep = "1";
		public const string TwoStep = "2";


		public class oneStep : Constant<string>
		{
			public oneStep() : base(OneStep) { ;}
		}

		public class twoStep : Constant<string>
		{
			public twoStep() : base(TwoStep) { ;}
		}
	}
}
