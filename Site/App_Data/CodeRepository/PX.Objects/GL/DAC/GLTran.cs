using System.Collections.Generic;
using PX.Common;

namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.CS;
	using PX.Objects.PM;
	using PX.Objects.CR;
	using PX.Objects.TX;

	/// <summary>
	/// Represents a journal transaction.
	/// The transactions are grouped in <see cref="Batch">batches</see> and edited through the Journal Transactions (GL.30.10.00) screen
	/// (corresponds to the <see cref="JournalEntry"/> graph).
	/// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.Transaction)]
	[PXPrimaryGraph(typeof(JournalEntry))]
	public partial class GLTran : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField { }

		/// <summary>
		/// Used for selection on screens.
		/// </summary>
		[PXBool]
		[PXUIField(DisplayName = "Selected", Visible = false)]
		public virtual bool? Selected { get; set; }
		#endregion
		#region IncludedInReclassHistory
		public abstract class includedInReclassHistory : IBqlField { }

		/// <summary>
		/// Used for storing state of "Reclassification History" button in GL301000 and GL404000
		/// </summary>
		[PXBool]
		[PXUIField(DisplayName = "Included in Reclass. History")]
		public virtual bool? IncludedInReclassHistory { get; set; }
		#endregion

		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;

		/// <summary>
		/// Identifier of the <see cref="Branch"/>, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Batch.BranchID">branch of the parent batch</see>.
		/// Corresponds to the <see cref="Branch.BranchID"/> field.
		/// </value>
		[Branch(typeof(Batch.branchID))]
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
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;

		/// <summary>
		/// Key field.
		/// The code of the module, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Batch.Module">module of the parent batch</see>.
		/// Possible values are:
		/// "GL", "AP", "AR", "CM", "CA", "IN", "DR", "FA", "PM", "TX", "SO", "PO".
		/// </value>
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(Batch))]
		[PXUIField(DisplayName="Module",Visibility=PXUIVisibility.Visible,Visible=false)]
		[BatchModule.List()]
		public virtual String Module
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
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;

		/// <summary>
		/// Key field.
		/// The number of the <see cref="Batch"/>, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Batch.BatchNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(Batch))]
		[PXParent(typeof(Select<Batch,Where<Batch.module,Equal<Current<GLTran.module>>,And<Batch.batchNbr, Equal<Current<GLTran.batchNbr>>>>>))]
		[PXUIField(DisplayName = "Batch Number", Visibility = PXUIVisibility.Visible, Visible = false)]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;

		/// <summary>
		/// Key field. Auto-generated.
		/// The number of the transaction in the <see cref="Batch"/>.
		/// </summary>
		/// <value>
		/// Note that the sequence of line numbers of the transactions belonging to a single batch may include gaps.
		/// </value>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		[PXLineNbr(typeof(Batch.lineCntr))]
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
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		protected Int32? _LedgerID;

		/// <summary>
		/// Identifier of the <see cref="Ledger"/>, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// If the <see cref="Ledger.BalanceType">Balance Type</see> of the <see cref="Batch.LedgerID">ledger of the batch</see> is Actual (<c>"A"</c>),
		/// defaults to the <see cref="Branch.LedgerID">ledger of the branch</see>. Otherwise defaults to the <see cref="Batch.LedgerID">ledger of the batch</see>.
		/// Corresponds to the <see cref="Ledger.LedgerID"/> field.
		/// </value>
		[PXDBInt()]
		[PXFormula(typeof(Switch<Case<Where<Selector<Current<Batch.ledgerID>, Ledger.balanceType>, Equal<LedgerBalanceType.actual>>, Selector<GLTran.branchID, Branch.ledgerID>>, Current<Batch.ledgerID>>))]
		[PXDefault()]
		public virtual Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion		
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;

		/// <summary>
		/// Identifier of the <see cref="Account"/> of the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Account.AccountID"/> field.
		/// </value>
		[Account(typeof(GLTran.branchID), LedgerID = typeof(GLTran.ledgerID), DescriptionField = typeof(Account.description))]
		[PXDefault]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;

		/// <summary>
		/// Identifier of the <see cref="Sub">Subaccount</see> of the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Sub.SubID"/> field.
		/// </value>
		[SubAccount(typeof(GLTran.accountID), typeof(GLTran.branchID), true)]
		[PXDefault]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;

		/// <summary>
		/// Identifier of the <see cref="PMProject">Project</see> associated with the transaction,
		/// or the <see cref="PMSetup.NonProjectCode">non-project code</see> indicating that the transaction is not related to any particular project.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PMProject.ProjectID"/> field.
		/// </value>
		[GLProjectDefault(typeof(GLTran.ledgerID), AccountType = typeof(GLTran.accountID), PersistingCheck = PXPersistingCheck.Nothing)]
		[ActiveProjectForModule(BatchModule.GL, true, AccountFieldType = typeof(accountID))]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;

		/// <summary>
		/// Identifier of the <see cref="PMTask">Task</see> associated with the transaction.
		/// The field is relevant only if the Projects module has been activated.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PMTask.TaskID"/> field.
		/// </value>
		[ActiveProjectTask(typeof(GLTran.projectID), BatchModule.GL, DisplayName = "Project Task")]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;

		/// <summary>
		/// The reference number of the transaction.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Batch.RefNbr">reference number of the batch</see>.
		/// Can be overriden by user.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBLiteDefault(typeof(Batch.refNbr), DefaultForUpdate = false, DefaultForInsert = false)]
		[PXUIField(DisplayName = "Ref. Number", Visibility = PXUIVisibility.Visible)]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;

		/// <summary>
		/// Identifier of the <see cref="InventoryItem">Inventory Item</see>, associated with the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="InventoryItem.InventoryID"/> field.
		/// </value>
		[IN.Inventory(Enabled = false,Visible = false)]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;

		/// <summary>
		/// The code of the <see cref="INUnit">Unit of Measure</see> for the <see cref="Qty">qunatity</see> of the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="INUnit.fromUnit"/> field.
		/// </value>
		[IN.INUnit(typeof(GLTran.inventoryID), typeof(GLTran.accountID), typeof(GLTran.accountRequireUnits))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;

		/// <summary>
		/// The quantity of the transaction.
		/// </summary>
		[IN.PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region DebitAmt
		public abstract class debitAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _DebitAmt;

		/// <summary>
		/// The debit amount of the transaction.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// See also the <see cref="CuryDebitAmt"/> field.
		/// </summary>
		[PXDBBaseCury(typeof(GLTran.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<Batch.debitTotal>))]
		[PXUIField(DisplayName = "DebitAmt")]
		public virtual Decimal? DebitAmt
		{
			get
			{
				return this._DebitAmt;
			}
			set
			{
				this._DebitAmt = value;
			}
		}
		#endregion
		#region CreditAmt
		public abstract class creditAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CreditAmt;

		/// <summary>
		/// The credit amount of the transaction.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// See also the <see cref="CuryCreditAmt"/> field.
		/// </summary>
		[PXDBBaseCury(typeof(GLTran.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<Batch.creditTotal>))]
		[PXUIField(DisplayName = "CreditAmt")]
		public virtual Decimal? CreditAmt
		{
			get
			{
				return this._CreditAmt;
			}
			set
			{
				this._CreditAmt = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;

		/// <summary>
		/// Identifier of the <see cref="PX.Objects.CM.CurrencyInfo">CurrencyInfo</see> object associated with the transaction.
		/// </summary>
		/// <value>
		/// Auto-generated. Corresponds to the <see cref="PX.Objects.CM.CurrencyInfo.CurrencyInfoID"/> field.
		/// </value>
		[PXDBLong()]
		[CurrencyInfo(typeof(CurrencyInfo.curyInfoID))]
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
		#region CuryDebitAmt
		public abstract class curyDebitAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDebitAmt;
	//	[PXDBDecimal(4)]

		/// <summary>
		/// The debit amount of the transaction.
		/// Given in the <see cref="Batch.CuryID">currency</see> of the batch.
		/// See also the <see cref="DebitAmt"/> field.
		/// </summary>
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Debit Amount", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(SumCalc<Batch.curyDebitTotal>))]
		[PXDBCurrency(typeof(GLTran.curyInfoID),typeof(GLTran.debitAmt))]
		public virtual Decimal? CuryDebitAmt
		{
			get
			{
				return this._CuryDebitAmt;
			}
			set
			{
				this._CuryDebitAmt = value;
			}
		}
		#endregion
		#region CuryCreditAmt
		public abstract class curyCreditAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCreditAmt;

		/// <summary>
		/// The credit amount of the transaction.
		/// Given in the <see cref="Batch.CuryID">currency</see> of the batch.
		/// See also the <see cref="CreditAmt"/> field.
		/// </summary>
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Credit Amount", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(SumCalc<Batch.curyCreditTotal>))]
		[PXDBCurrency(typeof(GLTran.curyInfoID), typeof(GLTran.creditAmt))]
		public virtual Decimal? CuryCreditAmt
		{
			get
			{
				return this._CuryCreditAmt;
			}
			set
			{
				this._CuryCreditAmt = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;

		/// <summary>
		/// Indicates whether the transaction has been released.
		/// </summary>
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
		#region Posted
		public abstract class posted : PX.Data.IBqlField
		{
		}
		protected Boolean? _Posted;

		/// <summary>
		/// Indicates whether the transaction has been posted.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Posted
		{
			get
			{
				return this._Posted;
			}
			set
			{
				this._Posted = value;
			}
		}
		#endregion
		#region NonBillable
		public abstract class nonBillable : PX.Data.IBqlField
		{
		}
		protected Boolean? _NonBillable;

		/// <summary>
		/// When set to <c>true</c>, indicates that the transaction is non-billable in the <see cref="ProjectID">Project</see>
		/// This means that when releasing the batch the system will set the <see cref="PMTran.Billable"/> field of
		/// the <see cref="PMTran">project transaction</see> generated from this transaction to <c>false</c>.
		/// This field is relevant only if the Projects module has been activated and integrated with the General Ledger module.
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Non Billable", FieldClass = ProjectAttribute.DimensionName)]
		public virtual Boolean? NonBillable
		{
			get
			{
				return this._NonBillable;
			}
			set
			{
				this._NonBillable = value;
			}
		}
		#endregion
		#region IsInterCompany
		public abstract class isInterCompany : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsInterCompany;

		/// <summary>
		/// When <c>true</c>, indicates that the transaction is an inter-branch transaction.
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsInterCompany
		{
			get
			{
				return this._IsInterCompany;
			}
			set
			{
				this._IsInterCompany = value;
			}
		}
		#endregion
		#region SummPost
		public abstract class summPost : PX.Data.IBqlField
		{
		}
		protected Boolean? _SummPost;

		/// <summary>
		/// When set to <c>true</c>, indicates that the system must summarize the transaction with other ones when posting.
		/// In this case all transactions being posted are grouped by <see cref="AccountID">Account</see> and <see cref="SubID">Subaccount</see>.
		/// Then, the amounts are summarized and a single transaction is created for each group.
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? SummPost
		{
			get
			{
				return this._SummPost;
			}
			set
			{
				this._SummPost = value;
			}
		}
		#endregion
		#region ZeroPost
		public abstract class zeroPost : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// When set to <c>true</c>, indicates that the system must post the transaction even if its amounts are equal to zero.
		/// </summary>
		[PXBool()]
		public virtual Boolean? ZeroPost 
		{
			get;
			set;
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;

		/// <summary>
		/// The module, to which the original batch (e.g. the one reversed by the parent batch of this transaction) belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Batch.Module"/> field.
		/// </value>
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Orig. Module", Visible = false)]
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
		#region OrigBatchNbr
		public abstract class origBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigBatchNbr;

		/// <summary>
		/// The number of the original batch (e.g. the one reversed by the parent batch of this transaction).
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Batch.BatchNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Orig. Batch Nbr.", Visible = false)]
		public virtual String OrigBatchNbr
		{
			get
			{
				return this._OrigBatchNbr;
			}
			set
			{
				this._OrigBatchNbr = value;
			}
		}
		#endregion
		#region OrigLineNbr
		public abstract class origLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigLineNbr;

		/// <summary>
		/// The number of the corresponding transaction in the original batch (e.g. the one reversed by the parent batch of this transaction).
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="GLTran.LineNbr"/>.
		/// </value>
		[PXDBInt()]
		[PXUIField(DisplayName = "Orig. Line Nbr.", Visible = false)]
		public virtual Int32? OrigLineNbr
		{
			get
			{
				return this._OrigLineNbr;
			}
			set
			{
				this._OrigLineNbr = value;
			}
		}
		#endregion
		#region OrigAccountID
		public abstract class origAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigAccountID;

		/// <summary>
		/// Identifier of the <see cref="Account"/> associated with the original document.
		/// This field is populated for the transactions updating the account, which is different from the account of the original document.
		/// For example, if upon release of an Accounts Receivable document an RGOL account must be updated, the system will generate a transaction
		/// with <see cref="AccountID"/> set to the RGOL account and <see cref="OrigAccountID"/> set to
		/// the Accounts Receivable account associated with the document being released.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Account.AccountID"/> field.
		/// </value>
		[Account(DisplayName = "Original Account", Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? OrigAccountID
		{
			get
			{
				return this._OrigAccountID;
			}
			set
			{
				this._OrigAccountID = value;
			}
		}
		#endregion
		#region OrigSubID
		public abstract class origSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigSubID;

		/// <summary>
		/// Identifier of the <see cref="Sub">Subaccount</see> associated with the original document.
		/// For more information see the <see cref="OrigAccountID"/> field.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Sub.SubID"/> field.
		/// </value>
		[SubAccount(typeof(GLTran.origAccountID), DisplayName = "Original Subaccount", Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? OrigSubID
		{
			get
			{
				return this._OrigSubID;
			}
			set
			{
				this._OrigSubID = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int32? _TranID;

		/// <summary>
		/// A unique identifier of the transaction reserved for internal use.
		/// </summary>
		[PXDBIdentity()]
		public virtual Int32? TranID
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
			public const string Consolidation = "CON";
		}
		protected String _TranType;

		/// <summary>
		/// The type of the original document or transaction.
		/// This field is populated when a document is released in another module, such as Accounts Receivable or Accounts Payable.
		/// For example, when an <see cref="PX.Objects.AR.ARInvoice">ARInvoice</see> is released, the system will set this field for
		/// the resulting transactions to the document's <see cref="PX.Objects.AR.ARInvoice.DocType">DocType</see>.
		/// </summary>
		[PXDBString(3, IsFixed = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "TranType")]
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
		#region TranClass
		public abstract class tranClass : PX.Data.IBqlField
		{
			public const string Normal = "N";
			public const string Charge = "U";
			public const string Discount = "D";
			public const string Payment = "P";
			public const string WriteOff = "B";
			public const string Tax = "T";
			public const string WithholdingTax = "W";

			public const string Consolidation = "C";

			public const string RealizedAndRoundingGOL = "R";
			public const string UnrealizedAndRevaluationGOL = "G";

			//Also can contain: 
			//AccountType value - Revaluation
			//TranslationLineType value - Translations
		}
		protected String _TranClass;

		/// <summary>
		/// Reserved for internal use.
		/// The class of the document or transaction defined by the line.
		/// This field affects posting of documents and transactions to GL.
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(tranClass.Normal)]
		public virtual String TranClass
		{
			get
			{
				return this._TranClass;
			}
			set
			{
				this._TranClass = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;

		/// <summary>
		/// The description of the transaction.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Transaction Description", Visibility = PXUIVisibility.Visible)]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;

		/// <summary>
		/// The date of the transaction.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Batch.DateEntered">date of the parent batch</see>.
		/// </value>
		[PXDBDate()]
		[PXDefault(typeof(Batch.dateEntered))]
		[PXUIField(DisplayName = "Transaction Date", Visibility = PXUIVisibility.Visible, Enabled =false)]
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
		#region TranLineNbr
		public abstract class tranLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _TranLineNbr;

		/// <summary>
		/// Reserved for internal use.
		/// This field is populated on release of a document in another module with the number of the corresponding line in that document.
		/// This field is not populated when <see cref="SummPost"/> is on and in some other cases.
		/// </summary>
		[PXDBInt()]
		public virtual Int32? TranLineNbr
		{
			get
			{
				return this._TranLineNbr;
			}
			set
			{
				this._TranLineNbr = value;
			}
		}
		#endregion
		#region ReferenceID
		public abstract class referenceID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReferenceID;

		/// <summary>
		/// Identifier of the <see cref="PX.Objects.AR.Customer">Customer</see> or <see cref="PX.Objects.AP.Vendor">Vendor</see>
		/// associated with the transaction.
		/// This field is populated when a document is released in Accounts Receivable or Accounts Payable module.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Objects.AR.Customer.BAccountID">Customer.BAccountID</see> and
		/// <see cref="PX.Objects.AP.Vendor.BAccountID">Vendor.BAccountID</see> fields.
		/// </value>
		[PXDBInt()]
		[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, NotEqual<BAccountType.companyType>,
			And<BAccountR.type, NotEqual<BAccountType.prospectType>>>>), SubstituteKey = typeof(BAccountR.acctCD))]
		[PXUIField(DisplayName = "Customer/Vendor", Enabled = false, Visible = false)]		
		public virtual Int32? ReferenceID
		{
			get
			{
				return this._ReferenceID;
			}
			set
			{
				this._ReferenceID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;

		/// <summary>
		/// Identifier of the <see cref="FinPeriod">Financial Period</see>, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// Is equal to the <see cref="Batch.FinPeriodID"/> of the parent batch.
		/// For the explanation of the difference between this field and the <see cref="TranPeriodID"/>
		/// see the descriptions of the corresponding fields in the <see cref="Batch"/> class.
		/// </value>
		[PXDBDefault(typeof(Batch.finPeriodID))]
		[GL.FinPeriodID()]
		[PXUIField(DisplayName = "Post Period")]
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

		/// <summary>
		/// Identifier of the <see cref="FinPeriod">Financial Period</see>, to which the transaction belongs.
		/// </summary>
		/// <value>
		/// Is equal to the <see cref="Batch.TranPeriodID"/> of the parent batch.
		/// For the explanation of the difference between this field and the <see cref="FinPeriodID"/>
		/// see the descriptions of the corresponding fields in the <see cref="Batch"/> class.
		/// </value>
		[TranPeriodID(typeof(GLTran.tranDate))]
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
		#region PostYear
		public abstract class postYear : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The read-only year, to which the transaction is posted.
		/// </summary>
		/// <value>
		/// The value of this field is determined from the <see cref="FinPeriodID"/>.
		/// </value>
		[PXString(4, IsFixed = true)]
		public virtual String PostYear
		{
			[PXDependsOnFields(typeof(finPeriodID))]
			get
			{
				return (_FinPeriodID == null) ? null : FiscalPeriodUtils.FiscalYear(this._FinPeriodID);
			}
		}
		#endregion
		#region TranYear
		public abstract class tranYear : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The read-only year, to which the transaction is posted.
		/// </summary>
		/// <value>
		/// The value of this field is determined from the <see cref="TranPeriodID"/>.
		/// </value>
		[PXString(4, IsFixed = true)]
		public virtual String TranYear
		{
			[PXDependsOnFields(typeof(tranPeriodID))]
			get
			{
				return (_TranPeriodID == null) ? null : FiscalPeriodUtils.FiscalYear(this._TranPeriodID);
			}
		}
		#endregion
		#region NextPostYear
		public abstract class nextPostYear : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The read-only field providing the year, following the <see cref="PostYear"/>.
		/// </summary>
		[PXString(6, IsFixed = true)]
		public virtual String NextPostYear
		{
			[PXDependsOnFields(typeof(postYear))]
			get
			{
				return (this.PostYear == null) ? null : AutoNumberAttribute.NextNumber(this.PostYear) + "00";
			}
		}
		#endregion
		#region NextTranYear
		public abstract class nextTranYear : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The read-only field providing the year, following the <see cref="TranYear"/>.
		/// </summary>
		[PXString(6, IsFixed = true)]
		public virtual String NextTranYear
		{
			[PXDependsOnFields(typeof(tranYear))]
			get
			{
				return (this.TranYear == null) ? null : AutoNumberAttribute.NextNumber(this.TranYear) + "00";
			}
		}
		#endregion
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;

		/// <summary>
		/// Identifier of the <see cref="PX.Objects.CA.CATran">Cash Transaction</see> associated with this transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Objects.CA.CATran.TranID">CATran.TranID</see> field.
		/// </value>
		[PXDBLong()]
		[GLCashTranID()]
		public virtual Int64? CATranID
		{
			get
			{
				return this._CATranID;
			}
			set
			{
				this._CATranID = value;
			}
		}
		#endregion
		#region PMTranID
		public abstract class pMTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _PMTranID;

		/// <summary>
		/// Identifier of the <see cref="PMTran">Project Transaction</see> associated with this transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PMTran.TranID"/> field.
		/// </value>
		[PXDBChildIdentity(typeof(PMTran.tranID))]
		[PXDBLong()]
		public virtual Int64? PMTranID
		{
			get
			{
				return this._PMTranID;
			}
			set
			{
				this._PMTranID = value;
			}
		}
		#endregion
		#region OrigPMTranID
		public abstract class origPMTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _OrigPMTranID;

		/// <summary>
		/// Identifier of the <see cref="PMTran">Project Transaction</see> associated with the original transaction
		/// (e.g. the transaction reversed by this one).
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PMTran.TranID"/> field.
		/// </value>
		[PXDBLong()]
		public virtual Int64? OrigPMTranID
		{
			get
			{
				return this._OrigPMTranID;
			}
			set
			{
				this._OrigPMTranID = value;
			}
		}
		#endregion
		#region LedgerBalanceType
		public abstract class ledgerBalanceType : PX.Data.IBqlField
		{
		}
		protected string _LedgerBalanceType;

		/// <summary>
		/// The type of balance of the <see cref="Ledger"/>, associated with the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Ledger.BalanceType"/> field.
		/// </value>
		[PXString(1, IsFixed = true, InputMask = "")]
		public virtual string LedgerBalanceType
		{
			get
			{
				return this._LedgerBalanceType;
			}
			set
			{
				this._LedgerBalanceType = value;
			}
		}
		#endregion
		#region AccountRequireUnits
		public abstract class accountRequireUnits : PX.Data.IBqlField
		{
		}
		protected Boolean? _AccountRequireUnits;

		/// <summary>
		/// Indicates whether the account associated with the transaction requires <see cref="UOM">Units of Measure</see> to be specified.
		/// </summary>
		[PXBool()]
		public virtual Boolean? AccountRequireUnits
		{
			get
			{
				return this._AccountRequireUnits;
			}
			set
			{
				this._AccountRequireUnits = value;
			}
		}
		#endregion
		#region AccountBranchID
		[Obsolete(Common.Messages.FieldIsObsoleteRemoveInAcumatica7)]
		public abstract class accountBranchID : IBqlField { }

		[Obsolete(Common.Messages.FieldIsObsoleteRemoveInAcumatica7)]
		[PXInt]
		public virtual int? AccountBranchID
		{
			get;
			set;
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected String _TaxID;

		/// <summary>
		/// Identifier of the <see cref="Tax"/> associated with the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Tax.TaxID"/> field.
		/// </value>
		[PXDBString(TX.Tax.taxID.Length, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Tax ID")]
		[PXSelector(typeof(Search<Tax.taxID, Where2<Where<Tax.taxType, Equal<CSTaxType.sales>,
									Or<Tax.taxType, Equal<CSTaxType.vat>,
									Or<Tax.taxType, Equal<CSTaxType.use>,
									Or<Tax.taxType,Equal<CSTaxType.withholding>>>>>,
								And<Tax.deductibleVAT,Equal<False>,
								And<Where2<Where<Tax.purchTaxAcctID, Equal<Current<GLTran.accountID>>,
									And<Tax.purchTaxSubID, Equal<Current<GLTran.subID>>>>,
									Or<Where<Tax.salesTaxAcctID, Equal<Current<GLTran.accountID>>,
									And<Tax.salesTaxSubID, Equal<Current<GLTran.subID>>>>>>>>>>))]
		//[PXSelector(typeof(Search<Tax.taxID, Where<Tax.taxType, Equal<CSTaxType.sales>,
		//                            Or<Tax.taxType, Equal<CSTaxType.vat>,
		//                            Or<Tax.taxType, Equal<CSTaxType.use>>>>>))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;

		/// <summary>
		/// Identifier of the <see cref="TaxCategory">Tax Category</see> associated with the transaction.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="TaxCategory.TaxCategoryID"/> field.
		/// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TX.TaxCategory.taxCategoryID), DescriptionField = typeof(TX.TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TX.TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TX.TaxCategory.taxCategoryID))]
		[PXDefault(typeof(Search<Account.taxCategoryID, Where<Account.accountID, Equal<Current<GLTran.accountID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;

		/// <summary>
		/// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
		/// </value>
		[PXNote()]
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
		#region ReclassificationProhibited
		public abstract class reclassificationProhibited : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReclassificationProhibited;

		/// <summary>
		/// Indicates that the transaction can't be reclassified.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? ReclassificationProhibited
		{
			get
			{
				return this._ReclassificationProhibited;
			}
			set
			{
				this._ReclassificationProhibited = value;
			}
		}
		#endregion
		#region ReclassBatchModule
		public abstract class reclassBatchModule : PX.Data.IBqlField
		{
		}
		protected String _ReclassBatchModule;

		/// <summary>
		/// The module of the <see cref="Batch"/>, by which the transaction has been reclassified.
		/// </summary>
		[PXUIField(DisplayName = "Reclass. Batch Module")]
		[PXDBString(2, IsFixed = true)]
		public virtual String ReclassBatchModule
		{
			get
			{
				return this._ReclassBatchModule;
			}
			set
			{
				this._ReclassBatchModule = value;
			}
		}
		#endregion
		#region ReclassBatchNbr
		public abstract class reclassBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _ReclassBatchNbr;

		/// <summary>
		/// The number of the <see cref="Batch"/>, by which the transaction has been reclassified.
		/// </summary>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Reclass. Batch Number")]
		public virtual String ReclassBatchNbr
		{
			get
			{
				return this._ReclassBatchNbr;
			}
			set
			{
				this._ReclassBatchNbr = value;
			}
		}
		#endregion
		#region IsReclassReverse
		public abstract class isReclassReverse : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsReclassReverse;

		/// <summary>
		/// This field distinguishes reversed transaction in the reclassification pair from transaction on destination account.
		/// When <c>true</c>, indicates that the transaction is a reversed of source transaction.
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "IsReclassReverse")]
		public virtual Boolean? IsReclassReverse
		{
			get
			{
				return this._IsReclassReverse;
			}
			set
			{
				this._IsReclassReverse = value;
			}
		}
		#endregion
		#region Reclassified
		public abstract class reclassified : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reclassified;

		/// <summary>
		/// Indicates that the transaction has been reclassified.
		/// It is set on Reclass Batch Releasing. This fact is used.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Reclassified")]
		public virtual Boolean? Reclassified
		{
			get
			{
				return this._Reclassified;
			}
			set
			{
				this._Reclassified = value;
			}
		}
		#endregion
		#region ReclassifiedWithTranDate
		public abstract class reclassifiedWithTranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReclassifiedWithTranDate;

		/// <summary>
		/// Indicates the date, by which the transaction has been reclassified.
		/// </summary>
		[PXDBDate()]
		public virtual DateTime? ReclassifiedWithTranDate
		{
			get
			{
				return this._ReclassifiedWithTranDate;
			}
			set
			{
				this._ReclassifiedWithTranDate = value;
			}
		}
		#endregion
		#region ReclassSourceTranModule
		public abstract class reclassSourceTranModule : PX.Data.IBqlField
		{
		}
		protected String _ReclassSourceTranModule;

		/// <summary>
		/// Part of key of source transaction (first in reclassification chain), that has been reclassified.
		/// </value>
		[PXDBString(2, IsFixed = true)]
		public virtual String ReclassSourceTranModule
		{
			get
			{
				return this._ReclassSourceTranModule;
			}
			set
			{
				this._ReclassSourceTranModule = value;
			}
		}
		#endregion
		#region ReclassSourceTranBatchNbr
		public abstract class reclassSourceTranBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _ReclassSourceTranBatchNbr;

		/// <summary>
		/// Part of key of source transaction (first in reclassification chain), that has been reclassified.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		public virtual String ReclassSourceTranBatchNbr
		{
			get
			{
				return this._ReclassSourceTranBatchNbr;
			}
			set
			{
				this._ReclassSourceTranBatchNbr = value;
			}
		}
		#endregion
		#region ReclassSourceTranLineNbr
		public abstract class reclassSourceTranLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _ReclassSourceTranLineNbr;

		/// <summary>
		/// Part of key of source transaction (first in reclassification chain), that has been reclassified.
		/// </value>
		[PXDBInt()]
		public virtual Int32? ReclassSourceTranLineNbr
		{
			get
			{
				return this._ReclassSourceTranLineNbr;
			}
			set
			{
				this._ReclassSourceTranLineNbr = value;
			}
		}
		#endregion
		#region ReclassSeqNbr
		public abstract class reclassSeqNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _ReclassSeqNbr;

		/// <summary>
		/// Sequence number of reclassification pair in reclassification chain.
		/// </summary>
		[PXDBInt()]
		[PXUIField(DisplayName = "Reclass. Sequence Nbr.", Visible = false)]
		public virtual Int32? ReclassSeqNbr
		{
			get
			{
				return this._ReclassSeqNbr;
			}
			set
			{
				this._ReclassSeqNbr = value;
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

		public static string GetKeyImage(string module, string batchNbr, int? lineNbr)
		{
			return string.Format("{0}:{1}, {2}:{3}, {4}:{5}", typeof(GLTran.module).Name, module,
																typeof(GLTran.batchNbr).Name, batchNbr,
																typeof(GLTran.lineNbr).Name, lineNbr);
		}

		public string GetKeyImage()
		{
			return GetKeyImage(Module, BatchNbr, LineNbr);
		}

		public static string GetImage(string module, string batchNbr, int? lineNbr)
		{
			return string.Format("{0}[{1}]", EntityHelper.GetFriendlyEntityName(typeof (GLTran)),
				GetKeyImage(module, batchNbr, lineNbr));
		}

		public override string ToString()
		{
			return GetImage(Module, BatchNbr, LineNbr);
		}
	}

	public class GLTranKey : IBqlTable
	{
		public GLTranKey()
		{
			
		}

		public GLTranKey(GLTran tran)
		{
			Module = tran.Module;
			BatchNbr = tran.BatchNbr;
			LineNbr = tran.LineNbr;
		}

		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;

		[PXString(2, IsKey = true)]
		public virtual String Module
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
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;

		[PXString(15, IsKey = true)]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;

		[PXInt(IsKey = true)]
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
	}
}
