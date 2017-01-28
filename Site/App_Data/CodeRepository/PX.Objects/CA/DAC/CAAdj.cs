using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.TX;
using PX.TM;
using PX.Objects.EP;
using PX.Objects.CR;
using IRegister = PX.Objects.CM.IRegister;

namespace PX.Objects.CA
{
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(CATranEntry))]
	[PXCacheName(Messages.CashTransactions)]
	[PXEMailSource]
	public partial class CAAdj : PX.Data.IBqlTable, ICADocument, IAssign, IRegister
	{
		public string DocType
		{
			get { return this._AdjTranType; }
			set { _AdjTranType = value; }
		}

		public string RefNbr
		{
			get { return this._AdjRefNbr; }
			set { _AdjRefNbr = value; }
		}

		#region Selected
		public abstract class selected : PX.Data.IBqlField
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
		#region AdjRefNbr
		public abstract class adjRefNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjRefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CAAdj.adjRefNbr,Where<CAAdj.draft,Equal<False>>>))]
		[AutoNumber(typeof(CASetup.registerNumberingID), typeof(CAAdj.tranDate))]
		public virtual String AdjRefNbr
		{
			get
			{
				return this._AdjRefNbr;
			}
			set
			{
				this._AdjRefNbr = value;
			}
		}
		#endregion
		#region AdjTranType
		public abstract class adjTranType : PX.Data.IBqlField
		{
		}
		protected String _AdjTranType;
		[PXDBString(3, IsFixed = true)]
		[CATranType.List]
		[PXDefault(CATranType.CAAdjustment)]
		[PXUIField(DisplayName = "Tran. Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AdjTranType
		{
			get
			{
				return this._AdjTranType;
			}
			set
			{
				this._AdjTranType = value;
			}
		}
		#endregion
		#region TransferNbr
		public abstract class transferNbr : PX.Data.IBqlField
		{
		}
		protected String _TransferNbr;
		[PXDBString(15, IsUnicode = true)]
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
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Document Ref.", Visibility=PXUIVisibility.SelectorVisible)]
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
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
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
		[PXUIField(DisplayName = "Tran. Date", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDefault(CADrCr.CADebit)]
		[PXDBString(1, IsFixed = true)]
		[CADrCr.List()]
		[PXUIField(DisplayName = "Disb. / Receipt", Enabled = false)]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
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
		[PXFieldDescription]
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
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[TranPeriodID(typeof(CAAdj.tranDate))]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[CAOpenPeriod(typeof(CAAdj.tranDate))]
		[PXDefault()]
		[PXUIField(DisplayName = "Fin. Period")]
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
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Zone", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TaxZone.taxZoneID), DescriptionField = typeof(TaxZone.descr), Filterable = true)]
		[PXDefault(typeof(Search<CashAccountETDetail.taxZoneID, Where<CashAccountETDetail.accountID, Equal<Current<CAAdj.cashAccountID>>, And<CashAccountETDetail.entryTypeID, Equal<Current<CAAdj.entryTypeID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo()]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region Draft
		public abstract class draft : PX.Data.IBqlField
		{
		}
		protected Boolean? _Draft;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Draft")]		
		public virtual Boolean? Draft
		{
			get
			{
				return this._Draft;
			}
			set
			{
				this._Draft = value;
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(Search<CASetup.holdEntry>))]
		[PXUIField(DisplayName="Hold")]
		[PXNoUpdate]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
			}
		}
		#endregion
		#region RequestApproval
		public abstract class requestApproval : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequestApproval;
		[PXBool()]		
		[PXDBScalar(typeof(Search<CASetup.requestApproval>))]		
		[PXDefault(typeof(Search<CASetup.requestApproval>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? RequestApproval
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
		#region Approved
		public abstract class approved : PX.Data.IBqlField
		{
		}
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Boolean? Approved
		{
			get
			{
				return this._Approved;
			}
			set
			{
				this._Approved = value;
			}
		}
		#endregion
		#region Rejected
		public abstract class rejected : IBqlField
		{
		}
		protected bool? _Rejected = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Reject", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public bool? Rejected
		{
			get
			{
				return _Rejected;
			}
			set
			{
				_Rejected = value;
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
			}
		}
		#endregion
		#region IsTaxValid
		public abstract class isTaxValid : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Tax is up to date", Enabled = false)]
		public virtual Boolean? IsTaxValid
		{
			get;
			set;
		}
		#endregion
		#region IsTaxPosted
		public abstract class isTaxPosted : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Tax is posted/commited to the external Tax Engine(Avalara)", Enabled = false)]
		public virtual Boolean? IsTaxPosted
		{
			get;
			set;
		}
		#endregion
		#region IsTaxSaved
		public abstract class isTaxSaved : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Tax is saved in external Tax Engine(Avalara)", Enabled = false)]
		public virtual Boolean? IsTaxSaved
		{
			get;
			set;
		}
		#endregion
		#region CuryTaxTotal
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxTotal;
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.taxTotal))]
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTaxTotal
		{
			get
			{
				return this._CuryTaxTotal;
			}
			set
			{
				this._CuryTaxTotal = value;
			}
		}
		#endregion
		#region TaxTotal
		public abstract class taxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxTotal
		{
			get
			{
				return this._TaxTotal;
			}
			set
			{
				this._TaxTotal = value;
			}
		}
		#endregion
				
		#region CuryVatExemptTotal
		public abstract class curyVatExemptTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVatExemptTotal;
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.vatExemptTotal))]
		[PXUIField(DisplayName = "VAT Exempt Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVatExemptTotal
		{
			get
			{
				return this._CuryVatExemptTotal;
			}
			set
			{
				this._CuryVatExemptTotal = value;
			}
		}
		#endregion

		#region VatExemptTaxTotal
		public abstract class vatExemptTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _VatExemptTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? VatExemptTotal
		{
			get
			{
				return this._VatExemptTotal;
			}
			set
			{
				this._VatExemptTotal = value;
			}
		}
		#endregion

		#region CuryVatTaxableTotal
		public abstract class curyVatTaxableTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVatTaxableTotal;
		[PXDBCurrency(typeof(APInvoice.curyInfoID), typeof(APInvoice.vatTaxableTotal))]
		[PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVatTaxableTotal
		{
			get
			{
				return this._CuryVatTaxableTotal;
			}
			set
			{
				this._CuryVatTaxableTotal = value;
			}
		}
		#endregion

		#region VatTaxableTotal
		public abstract class vatTaxableTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _VatTaxableTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? VatTaxableTotal
		{
			get
			{
				return this._VatTaxableTotal;
			}
			set
			{
				this._VatTaxableTotal = value;
			}
		}
		#endregion

		#region CuryTaxAmt
		public abstract class curyTaxAmt : IBqlField { }
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.taxAmt))]
		[PXUIField(DisplayName = "Tax Amount")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTaxAmt { get; set; }
		#endregion
		#region TaxAmt
		public abstract class taxAmt : IBqlField { }
		[PXDBDecimal(4, BqlField = typeof(CAAdj.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxAmt { get; set; }
		#endregion

		#region CurySplitTotal
		public abstract class curySplitTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CurySplitTotal;
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.splitTotal))]
		[PXUIField(DisplayName = "Detail Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CurySplitTotal
		{
			get
			{
				return this._CurySplitTotal;
			}
			set
			{
				this._CurySplitTotal = value;
			}
		}
		#endregion
		#region SplitTotal
		public abstract class splitTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _SplitTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? SplitTotal
		{
			get
			{
				return this._SplitTotal;
			}
			set
			{
				this._SplitTotal = value;
			}
		}
		#endregion
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.tranAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Enabled = false)]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Tran. Amount", Enabled = false)]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CAAdj.cashAccountID>>>>))]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region CuryControlAmt
		public abstract class curyControlAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryControlAmt;
		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.controlAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Total")]
		public virtual Decimal? CuryControlAmt
		{
			get
			{
				return this._CuryControlAmt;
			}
			set
			{
				this._CuryControlAmt = value;
			}
		}
		#endregion
		#region ControlAmt
		public abstract class controlAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ControlAmt
		{
			get
			{
				return this._ControlAmt;
			}
			set
			{
				this._ControlAmt = value;
			}
		}
		#endregion
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSubordinateSelector]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				this._EmployeeID = value;
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
		[PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
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
		[PX.TM.PXOwnerSelector()]
		[PXUIField(DisplayName = "Approver", Enabled = false)]
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.CA, "{0} {1}", new Type[] { typeof(CAAdj.adjTranType), typeof(CAAdj.adjRefNbr) },
			new Type[] { typeof(CAAdj.entryTypeID), typeof(CAAdj.tranDesc), typeof(CAAdj.extRefNbr) },
			NumberFields = new Type[] { typeof(CAAdj.adjRefNbr) },
			Line1Format = "{0:d}{1}{2}{4}", Line1Fields = new Type[] { typeof(CAAdj.tranDate), typeof(CAAdj.entryTypeID), typeof(CAAdj.extRefNbr), typeof(CAAdj.ownerID), typeof(BAccount.acctName) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(CAAdj.tranDesc) }
		)]
		[PXNote(new Type[0], DescriptionField = typeof(CAAdj.adjRefNbr))]
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
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		[PXDBLong()]
		[AdjCashTranID()]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected string _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CATransferStatus.Balanced, PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[CATransferStatus.List]
		[SetStatus]
		[PXDependsOnFields(
			typeof(CAAdj.hold),
			typeof(CAAdj.approved),
			typeof(CAAdj.requestApproval),
			typeof(CAAdj.rejected),
			typeof(CAAdj.released))]
		public virtual string Status
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
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId, InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>, 
									Where<CashAccountETDetail.accountID, Equal<Current<CAAdj.cashAccountID>>, And<CAEntryType.module, Equal<BatchModule.moduleCA>>>>), DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Entry Type", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault()]
		public virtual String EntryTypeID
		{
			get
			{
				return this._EntryTypeID;
			}
			set
			{
				this._EntryTypeID = value;
			}
		}
		#endregion
		#region PaymentsReclassification
		public abstract class paymentsReclassification : PX.Data.IBqlField
		{
		}
		protected Boolean? _PaymentsReclassification;
		[PXBool()]
		[PXDBScalar(typeof(Search<CAEntryType.useToReclassifyPayments, Where<CAEntryType.entryTypeId, Equal<CAAdj.entryTypeID>>>))]
		public virtual Boolean? PaymentsReclassification
		{
			get
			{
				return this._PaymentsReclassification;
			}
			set
			{
				this._PaymentsReclassification = value;
			}
		}
		#endregion
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cleared")]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
			}
		}
		#endregion
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date")]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region TranID_CATran_batchNbr
		public abstract class tranID_CATran_batchNbr : PX.Data.IBqlField
		{
		}
		#endregion

		#region TaxCalcMode
		public abstract class taxCalcMode : IBqlField { }
		protected string _TaxCalcMode;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(VendorClass.taxCalcMode.TaxSetting, typeof(Search<CashAccountETDetail.taxCalcMode, 
			Where<CashAccountETDetail.accountID, Equal<Current<CAAdj.cashAccountID>>,
				And<CashAccountETDetail.entryTypeID, Equal<Current<CAAdj.entryTypeID>>>>>))]
		[VendorClass.taxCalcMode.List]
		[PXUIField(DisplayName = "Tax Calculation Mode")]
		public virtual string TaxCalcMode
		{
			get { return this._TaxCalcMode; }
			set { this._TaxCalcMode = value; }
		}
		#endregion
		#region CuryTaxRoundDiff
		public abstract class curyTaxRoundDiff : IBqlField { }

		[PXDBCurrency(typeof(CAAdj.curyInfoID), typeof(CAAdj.taxRoundDiff), BaseCalc = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Rounding Diff.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public decimal? CuryTaxRoundDiff
		{
			get;
			set;
		}
		#endregion
		#region TaxRoundDiff
		public abstract class taxRoundDiff : IBqlField { }

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public decimal? TaxRoundDiff
		{
			get;
			set;
		}
		#endregion
		#region HasWithHoldTax
		public abstract class hasWithHoldTax : IBqlField { }
		protected bool? _HasWithHoldTax;
		[PXBool]
		[RestrictWithholdingTaxCalcMode(typeof(CAAdj.taxZoneID), typeof(CAAdj.taxCalcMode))]
		public virtual bool? HasWithHoldTax
		{
			get { return this._HasWithHoldTax; }
			set { this._HasWithHoldTax = value; }
		}
		#endregion
		#region HasUseTax
		public abstract class hasUseTax : IBqlField { }
		protected bool? _HasUseTax;
		[PXBool]
		[RestrictUseTaxCalcMode(typeof(CAAdj.taxZoneID), typeof(CAAdj.taxCalcMode))]
		public virtual bool? HasUseTax
		{
			get { return this._HasUseTax; }
			set { this._HasUseTax = value; }
		}
		#endregion
		#region UsesManualVAT
		public abstract class usesManualVAT : IBqlField
		{
		}
		protected bool? _UsesManualVAT;
		[PXDBBool]
		[RestrictManualVAT(typeof(CAAdj.taxZoneID), typeof(CAAdj.taxCalcMode))]
		[PXUIField(DisplayName = "Manual VAT Entry", Enabled = false)]
		public virtual bool? UsesManualVAT
		{
			get
			{
				return this._UsesManualVAT;
			}
			set
			{
				this._UsesManualVAT = value;
			}
		}
		#endregion

		#region IRegister

		public DateTime? DocDate
		{
			get { return TranDate; } 
			set { TranDate = value; }
		}

		public string DocDesc
		{
			get { return TranDesc; }
			set { TranDesc = value; }
		}

		public string OrigModule
		{
			get { return BatchModule.CA; }
			set { }
		}

		public decimal? CuryOrigDocAmt { get { return null; } set {} }
		public decimal? OrigDocAmt { get { return null; } set {} }

		#endregion
	}

	public class SetStatusAttribute : PXEventSubscriberAttribute,
		IPXRowSelectingSubscriber, IPXRowInsertingSubscriber, IPXRowUpdatingSubscriber, IPXRowSelectedSubscriber
	{
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			sender.Graph.FieldUpdating.AddHandler<CAAdj.hold>((cache, e) =>
			{
				PXBoolAttribute.ConvertValue(e);

				CAAdj item = e.Row as CAAdj;
				if (item != null)
				{
					UpdateStatus(cache, item, (bool?)e.NewValue);
				}
			});

			sender.Graph.FieldVerifying.AddHandler<CAAdj.status>((cache, e) => { e.NewValue = cache.GetValue<CAAdj.status>(e.Row); });
		}

		public void UpdateStatus(PXCache cache, CAAdj row, bool? isHold)
		{
			if (isHold == true)
			{
				cache.SetValue(row, _FieldOrdinal, CATransferStatus.Hold);
			}
			else if (row.Approved == false && row.RequestApproval == true)
			{
				cache.SetValue(row, _FieldOrdinal, CATransferStatus.Pending);
			}
			else if (row.Rejected == true)
			{
				cache.SetValue(row, _FieldOrdinal, CATransferStatus.Rejected);
			}
			else if (row.Released == true)
			{
				cache.SetValue(row, _FieldOrdinal, CATransferStatus.Released);
			}
			else
			{
				cache.SetValue(row, _FieldOrdinal, CATransferStatus.Balanced);
			}
		}

		public static void UpdateStatus(PXCache cache, CAAdj row)
		{
			foreach(var attr in cache.GetAttributes<CAAdj.status>(row))
			{
				if(attr is SetStatusAttribute)
				{
					(attr as SetStatusAttribute).UpdateStatus(cache, row, row.Hold);
				}
			}
		}

		public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			var item = (CAAdj)e.Row;
			if (item != null)
				UpdateStatus(sender, item, item.Hold);
		}

		public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			var item = (CAAdj)e.Row;
			UpdateStatus(sender, item, item.Hold);
		}

		public virtual void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			var item = (CAAdj)e.NewRow;
			UpdateStatus(sender, item, item.Hold);
		}

		public virtual void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var item = (CAAdj)e.Row;
			if (item != null)
				UpdateStatus(sender, item, item.Hold);
		}
	}
}
