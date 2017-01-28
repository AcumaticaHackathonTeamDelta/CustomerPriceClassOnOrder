using System;
using System.Diagnostics;

using PX.Data;
using PX.Data.EP;

using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.PM;

namespace PX.Objects.DR
{
	[Serializable]
	[DebuggerDisplay("SheduleID={ScheduleID} DocType={DocType} RefNbr={RefNbr}")]
	[PXPrimaryGraph(typeof(DraftScheduleMaint))]
	[PXCacheName(Messages.Schedule)]
	public partial class DRSchedule : PX.Data.IBqlTable
	{
		#region ScheduleID
		public abstract class scheduleID : PX.Data.IBqlField
		{
		}
		[PXParent(typeof(Select<ARTran, Where<ARTran.tranType, Equal<Current<DRSchedule.docType>>, And<ARTran.refNbr, Equal<Current<DRSchedule.refNbr>>, And<ARTran.lineNbr, Equal<Current<DRSchedule.lineNbr>>>>>>))]
		[PXParent(typeof(Select<APTran, Where<APTran.tranType, Equal<Current<DRSchedule.docType>>, And<APTran.refNbr, Equal<Current<DRSchedule.refNbr>>, And<APTran.lineNbr, Equal<Current<DRSchedule.lineNbr>>>>>>))]
		[PXSelector(typeof(Search<DRSchedule.scheduleID>), Filterable = true)]
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = Messages.ScheduleID, Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual int? ScheduleID
		{
			get;
			set;
		}
		#endregion

		#region ProxyScheduleID
		public abstract class proxyScheduleID : PX.Data.IBqlField
		{
		}
		[PXString]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual string ProxyScheduleID => 
			this.ScheduleID < 0 ? null : this.ScheduleID.ToString();
		#endregion

		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected string _Module;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(BatchModule.AR)]
		[PXUIField(DisplayName = "Module")]
		[PX.Data.EP.PXFieldDescription]
		public virtual string Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
		[PXDefault(ARDocType.Invoice)]
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Doc. Type", Enabled = true)]
		[PX.Data.EP.PXFieldDescription]
		public virtual string DocType
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
		}
		protected string _RefNbr;
		[PX.Data.EP.PXFieldDescription]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[DRDocumentSelector(typeof(DRSchedule.module), typeof(DRSchedule.docType), typeof(DRSchedule.bAccountID))]
		[PXFormula(typeof(Default<DRSchedule.documentType>))]
		public virtual string RefNbr
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
		protected int? _LineNbr;
		[PXDBInt]
		[PXUIField(DisplayName = Messages.LineNumber)]
		[DRLineSelector(typeof(DRSchedule.module), typeof(DRSchedule.docType), typeof(DRSchedule.refNbr))]
		[PXFormula(typeof(Default<DRSchedule.refNbr>))]
		[PXUIRequired(typeof(Where<DRSchedule.refNbr, IsNotNull>))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? LineNbr
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate()]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = Messages.Date, Enabled = false)]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected string _FinPeriodID;
		[PXDefault]
		[AROpenPeriod(typeof(DRSchedule.docDate))]
		[PXUIField(DisplayName = "Fin. Period", Enabled = false)]
		public virtual string FinPeriodID
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected int? _BAccountID;
		[PXDBInt]
		[PXSelector(
			typeof(Search<BAccountR.bAccountID, 
				Where<BAccountR.type, Equal<Current<DRSchedule.bAccountType>>,
					Or<BAccount.type, Equal<BAccountType.combinedType>>>>), 
			typeof(BAccountR.acctCD), 
			typeof(BAccountR.acctName), 
			typeof(BAccountR.type), 
			SubstituteKey = typeof(BAccountR.acctCD),
			DescriptionField = typeof(BAccountR.acctName))]
		[PXUIField(DisplayName = "Customer/Vendor", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Null)]
		[PXFormula(typeof(Default<DRSchedule.documentType>))]
		public virtual int? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region BAccountLocID
		public abstract class bAccountLocID : PX.Data.IBqlField
		{
		}
		protected int? _BAccountLocID;
		[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<DRSchedule.bAccountID>>>>))]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<DRSchedule.bAccountID>>>))]
		public virtual int? BAccountLocID
		{
			get
			{
				return this._BAccountLocID;
			}
			set
			{
				this._BAccountLocID = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected string _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Transaction Descr.", Visibility = PXUIVisibility.Visible)]
		public virtual string TranDesc
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
		#region IsCustom
		public abstract class isCustom : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsCustom;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Custom", Enabled = false)]
		public virtual Boolean? IsCustom
		{
			get
			{
				return this._IsCustom;
			}
			set
			{
				this._IsCustom = value;
			}
		}
		#endregion
		#region IsDraft
		public abstract class isDraft : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsDraft;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Draft", Visible = false)]
		public virtual Boolean? IsDraft
		{
			get
			{
				return this._IsDraft;
			}
			set
			{
				this._IsDraft = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : IBqlField { }
		[ProjectDefault]
		[PXDimensionSelector(ProjectAttribute.DimensionName,
			typeof(Search<PMProject.contractID, Where<PMProject.isTemplate, NotEqual<True>>>),
			typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.customerID), typeof(PMProject.description), typeof(PMProject.status), DescriptionField = typeof(PMProject.description))]
		[PXDBInt]
		[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible)]
		[PXFormula(typeof(Default<DRSchedule.documentType>))]
		[PXUIVerify(typeof(Where<
			DRSchedule.projectID, IsNull,
			Or<
				Selector<DRSchedule.bAccountID, BAccount.type>,
				Equal<BAccountType.vendorType>,
			Or<
				Selector<DRSchedule.projectID, PMProject.nonProject>,
				Equal<True>,
			Or<
				Selector<DRSchedule.projectID, PMProject.customerID>, 
				Equal<DRSchedule.bAccountID>>>>>),
			PXErrorLevel.Warning,
			PM.Warnings.ProjectCustomerDontMatchTheDocument)]
		public virtual int? ProjectID { get; set; }
		#endregion
		#region TaskID
		public abstract class taskID : IBqlField { }

		[ProjectTask(typeof(DRSchedule.projectID), DisplayName = "Project Task", AllowNullIfContract = true)]
		[PXFormula(typeof(Default<DRSchedule.documentType>))]
		public virtual int? TaskID { get; set; }
		#endregion

		#region DocumentType
		public abstract class documentType : PX.Data.IBqlField
		{
		}
		protected string _DocumentType;
		[PXString(3, IsFixed = true)]
		[DRScheduleDocumentType.List]
		[PXUIField(DisplayName = "Doc. Type", Enabled = false, Required = true)]
		public virtual string DocumentType
		{
			get
			{
				return this._DocumentType;
			}
			set
			{
				this._DocumentType = value;
			}
		}
		#endregion
		#region BAccountType
		public abstract class bAccountType : PX.Data.IBqlField
		{
		}
		protected string _BAccountType;
		[PXUIField(DisplayName = "Entity Type")]
		[PXDefault(CR.BAccountType.CustomerType, PersistingCheck=PXPersistingCheck.Nothing)]
		[PXString(2, IsFixed = true)]
		[PXStringList(new string[] { CR.BAccountType.VendorType, CR.BAccountType.CustomerType },
				new string[] { CR.Messages.VendorType, CR.Messages.CustomerType })]
		public virtual string BAccountType
		{
			get
			{
				return this._BAccountType;
			}
			set
			{
				this._BAccountType = value;
			}
		}
		#endregion
		#region OrigLineAmt
		public abstract class origLineAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigLineAmt;
		[PXBaseCury]
		[PXUIField(DisplayName = Messages.LineAmount, Enabled = false)]
		public virtual Decimal? OrigLineAmt
		{
			get
			{
				return this._OrigLineAmt;
			}
			set
			{
				this._OrigLineAmt = value;
			}
		}
		#endregion
		#region DocumentTypeEx
		public abstract class documentTypeEx : PX.Data.IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[DRScheduleDocumentType.List]
		[PXUIField(DisplayName = "Doc. Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual string DocumentTypeEx
		{
			[PXDependsOnFields(typeof(module), typeof(docType))]
			get
			{
				return DRScheduleDocumentType.BuildDocumentType(Module, DocType);
			}
		}
		#endregion

		#region TermStartDate
		public abstract class termStartDate : IBqlField { }

		protected DateTime? _TermStartDate;

		[PXDBDate]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Term Start Date")]
		public virtual DateTime? TermStartDate
		{
			get { return _TermStartDate; }
			set { _TermStartDate = value; }
		}
		#endregion
		#region TermEndDate
		public abstract class termEndDate : IBqlField { }

		protected DateTime? _TermEndDate;

		[PXDBDate]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Term End Date")]
		public virtual DateTime? TermEndDate
		{
			get { return _TermEndDate; }
			set { _TermEndDate = value; }
		}
		#endregion

		#region Status
		public abstract class status : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Status")]
		[DRScheduleStatus.List]
		public string Status
		{
			get;
			set;
		}
		#endregion

		#region NoteID

		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(DRSchedule.scheduleID))]
		public virtual Guid? NoteID { get; set; }

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
		protected string _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID
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
		protected string _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID
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

}
