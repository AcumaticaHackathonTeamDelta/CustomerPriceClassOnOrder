using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PX.Data;
using PX.Common;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CR;

using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;
using SOInvoice = PX.Objects.SO.SOInvoice;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;

namespace PX.Objects.AR
{
	public class ARDocType : ILabelProvider
	{
		public const string Invoice = "INV";
		public const string DebitMemo = "DRM";
		public const string CreditMemo = "CRM";
		public const string Payment = "PMT";
		public const string VoidPayment = "RPM";
		public const string Prepayment = "PPM";
		public const string Refund = "REF";
		public const string FinCharge = "FCH";
		public const string SmallBalanceWO = "SMB";
		public const string SmallCreditWO = "SMC";
		public const string CashSale = "CSL";
		public const string CashReturn = "RCS";
		public const string Undefined = "UND";
		public const string NoUpdate = Undefined;

		protected static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ Invoice, Messages.Invoice },
			{ DebitMemo, Messages.DebitMemo },
			{ CreditMemo, Messages.CreditMemo },
			{ Payment, Messages.Payment },
			{ VoidPayment, Messages.VoidPayment },
			{ Prepayment, Messages.Prepayment },
			{ Refund, Messages.Refund },
			{ FinCharge, Messages.FinCharge },
			{ SmallBalanceWO, Messages.SmallBalanceWO },
			{ SmallCreditWO, Messages.SmallCreditWO },
			{ CashSale, Messages.CashSale },
			{ CashReturn, Messages.CashReturn },
		};

		protected static readonly IEnumerable<ValueLabelPair> _valuePrintLabelPairs = new ValueLabelList
		{
			{ Invoice, Messages.PrintInvoice },
			{ DebitMemo, Messages.PrintDebitMemo },
			{ CreditMemo, Messages.PrintCreditMemo },
			{ Payment, Messages.PrintPayment },
			{ VoidPayment, Messages.PrintVoidPayment },
			{ Prepayment, Messages.PrintPrepayment },
			{ Refund, Messages.PrintRefund },
			{ FinCharge, Messages.PrintFinCharge },
			{ SmallBalanceWO, Messages.PrintSmallBalanceWO },
			{ SmallCreditWO, Messages.PrintSmallCreditWO },
			{ CashSale, Messages.PrintCashSale },
			{ CashReturn, Messages.PrintCashReturn },
		};

		public static readonly string[] Values = 
		{
			Invoice,
			DebitMemo,
			CreditMemo,
			Payment,
			VoidPayment,
			Prepayment,
			Refund,
			FinCharge,
			SmallBalanceWO,
			SmallCreditWO,
			CashSale,
			CashReturn
		};

		public static readonly string[] Labels = 
		{
			Messages.Invoice,
			Messages.DebitMemo,
			Messages.CreditMemo,
			Messages.Payment,
			Messages.VoidPayment,
			Messages.Prepayment,
			Messages.Refund,
			Messages.FinCharge,
			Messages.SmallBalanceWO,
			Messages.SmallCreditWO,
			Messages.CashSale,
			Messages.CashReturn
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public class CustomListAttribute : LabelListAttribute
				{
			public string[] AllowedValues => _AllowedValues;

			public string[] AllowedLabels => _AllowedLabels;

			public CustomListAttribute(IEnumerable<ValueLabelPair> valueLabelPairs)
				: base (valueLabelPairs)
			{ }
		}

		public class ListAttribute : LabelListAttribute
		{
			public ListAttribute() : base(_valueLabelPairs)
			{ }
		}

        /// <summary>
        /// Defines a Selector of the AR Document types with shorter description.<br/>
        /// In the screens displayed as combo-box.<br/>
        /// Mostly used in the reports.<br/>
        /// </summary>
		public class PrintListAttribute : LabelListAttribute
		{
			public PrintListAttribute() : base(_valuePrintLabelPairs)
			{ }
		}

		public class SOListAttribute  : LabelListAttribute
		{
			private static readonly IEnumerable<ValueLabelPair> _soValueLabelPairs = new ValueLabelList
			{
				{ Invoice, Messages.Invoice },
				{ DebitMemo, Messages.DebitMemo },
				{ CreditMemo, Messages.CreditMemo },
				{ CashSale, Messages.CashSale },
				{ CashReturn, Messages.CashReturn },
				{ NoUpdate, Messages.NoUpdate }
			};

			public SOListAttribute() : base(_soValueLabelPairs)
			{ }
		}

        /// <summary>
        /// Defines a list of the AR Document types, which may be used in the SO module.<br/>
        /// </summary>
		public class SOEntryListAttribute : CustomListAttribute
		{
			private static readonly string[] _soEntryListValues = { Invoice, DebitMemo, CreditMemo, CashSale, CashReturn };

			public SOEntryListAttribute()
				: base(_valueLabelPairs.Where(pair => _soEntryListValues.Contains(pair.Value)))
			{ }
		}

		public class invoice : Constant<string>
		{
			public invoice() : base(Invoice) { ;}
		}

		public class debitMemo : Constant<string>
		{
			public debitMemo() : base(DebitMemo) { ;}
		}

		public class creditMemo : Constant<string>
		{
			public creditMemo() : base(CreditMemo) { ;}
		}

		public class payment : Constant<string>
		{
			public payment() : base(Payment) { ;}
		}

		public class voidPayment : Constant<string>
		{
			public voidPayment() : base(VoidPayment) { ;}
		}
		public class prepayment : Constant<string>
		{
			public prepayment() : base(Prepayment) { ;}
		}
		public class refund : Constant<string>
		{
			public refund() : base(Refund) { ;}
		}

		public class finCharge : Constant<string>
		{
			public finCharge() : base(FinCharge) { ;}
		}

		public class smallBalanceWO : Constant<string>
		{
			public smallBalanceWO() : base(SmallBalanceWO) { ;}
		}

		public class smallCreditWO : Constant<string>
		{
			public smallCreditWO() : base(SmallCreditWO) { ;}
		}
				
		public class undefined : Constant<string> 
		{
			public undefined() : base(Undefined) { ;}
		}

		public class noUpdate : Constant<string>
		{
			public noUpdate() : base(NoUpdate) { ;}
		}

		public class cashSale : Constant<string>
		{
			public cashSale() : base(CashSale) { ;}
		}

		public class cashReturn : Constant<string>
		{
			public cashReturn() : base(CashReturn) { ;}
		}

		public static bool? Payable(string DocType)
		{
			switch (DocType)
			{
				case Invoice:
				case DebitMemo:
				case FinCharge:
				case SmallCreditWO: 
					return true;
				case Payment:
				case Prepayment:
				case CreditMemo:
				case VoidPayment:
				case Refund:
				case SmallBalanceWO:
				case CashSale:
				case CashReturn:
					return false;
				default:
					return null;
			}
		}

		public static Int16? SortOrder(string DocType)
		{
			switch (DocType)
			{
				case Invoice:
				case DebitMemo:
				case FinCharge:
				case CashSale:
					return 0;
				case Prepayment:
					return 1;
				case CreditMemo:
					return 2;
				case Payment:
				case SmallBalanceWO:
					return 3;
				case SmallCreditWO:
				case Refund:
					return 4;
				case VoidPayment:
				case CashReturn:
					return 5;
				default:
					return null;
			}
		}

		public static Decimal? SignBalance(string DocType)
		{
			switch (DocType)
			{
				case Refund:
				case Invoice:
				case DebitMemo:
				case FinCharge:
				case SmallCreditWO: 
					return 1m;
				case CreditMemo:
				case Payment :
				case Prepayment:
				case VoidPayment:
				case SmallBalanceWO:
					return -1m;
				case CashSale:
				case CashReturn:
					return 0;
				default:
					return null;
			}
		}
		
		public static Decimal? SignAmount(string DocType)
		{
			switch (DocType)
			{
				case Refund:
				case Invoice:
				case DebitMemo:
				case FinCharge:
				case SmallCreditWO: 
				case CashSale:
					return 1m;
				case CreditMemo:
				case Payment :
				case Prepayment:
				case VoidPayment:
				case SmallBalanceWO:
				case CashReturn:
					return -1m;
				default:
					return null;
			}
		}

		public static string TaxDrCr(string DocType)
		{
			switch (DocType)
			{
			  //Invoice Types
				case Invoice:
				case DebitMemo:
				case FinCharge:
				case CashSale:
					return DrCr.Credit;
				case CreditMemo:
				case CashReturn:
					return DrCr.Debit;
				default:
					return DrCr.Credit;
			}
		}

		public static string DocClass(string DocType)
		{
			switch (DocType) 
			{
				case Invoice:
				case DebitMemo:
				case CreditMemo:
				case FinCharge:
				case CashSale:
				case CashReturn:
					return GLTran.tranClass.Normal;
				case Payment:
				case VoidPayment:
				case Refund:
					return GLTran.tranClass.Payment;
				case SmallBalanceWO:
				case SmallCreditWO:
				case Prepayment:
					return GLTran.tranClass.Charge;
				default:
					return null;
			}
		}
	}

	public class ARDocStatus : ILabelProvider
	{
		private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ CreditHold, Messages.CreditHold },
			{ CCHold, Messages.CCHold },
			{ Hold, Messages.Hold },
			{ Balanced, Messages.Balanced },
			{ Voided, Messages.Voided },
			{ Scheduled, Messages.Scheduled },
			{ Open, Messages.Open },
			{ Closed, Messages.Closed },
			{ PendingPrint, Messages.PendingPrint },
			{ PendingEmail, Messages.PendingEmail },
			{ Reserved, Messages.Reserved },
		};

		public static readonly string[] Values = 
		{
			CreditHold,
			CCHold,
			Hold,
			Balanced,
			Voided,
			Scheduled,
			Open,
			Closed,
			PendingPrint,
			PendingEmail
		};

		public static readonly string[] Labels = 
		{
			Messages.CreditHold,
			Messages.CCHold,
			Messages.Hold,
			Messages.Balanced,
			Messages.Voided,
			Messages.Scheduled,
			Messages.Open,
			Messages.Closed,
			Messages.PendingPrint,
			Messages.PendingEmail
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public class ListAttribute : LabelListAttribute
		{
			public ListAttribute() : base(_valueLabelPairs)
			{ }
		}

		public const string Hold = "H";
		public const string Balanced = "B";
		public const string Voided = "V";
		public const string Scheduled = "S";
		public const string Open = "N";
		public const string Closed = "C";
		public const string PendingPrint = "P";
		public const string PendingEmail = "E";
		public const string CreditHold = "R";
		public const string CCHold = "W";
		public const string Reserved = "Z";

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class voided : Constant<string>
		{
			public voided() : base(Voided) { ;}
		}

		public class scheduled : Constant<string>
		{
			public scheduled() : base(Scheduled) { ;}
		}

		public class open : Constant<string>
		{
			public open() : base(Open) { ;}
		}

		public class closed : Constant<string>
		{
			public closed() : base(Closed) { ;}
		}

		public class pendingPrint : Constant<string>
		{
			public pendingPrint() : base(PendingPrint) { ;}
		}

		public class pendingEmail : Constant<string>
		{
			public pendingEmail() : base(PendingEmail) { ;}
		}

		public class cCHold : Constant<string>
		{
			public cCHold() : base(CCHold) { ;}
		}

		public class creditHold : Constant<string>
		{
			public creditHold() : base(CreditHold) { ;}
		}

		public class reserved : Constant<string>
		{
			public reserved(): base(Reserved) { }
		}
	}

	/// <summary>
	/// !REV!
	/// The base class for all Accounts Receivable documents.
	/// Provides the fields common to documents of <see cref="ARInvoice"/>, <see cref="ARPayment"/> and <see cref="ARCashSaleEntry"/> types.
	/// </summary>
	[PXPrimaryGraph(new Type[] {
		typeof(SO.SOInvoiceEntry),
		typeof(ARCashSaleEntry),
		typeof(ARInvoiceEntry), 
		typeof(ARPaymentEntry)
	},
		new Type[] {
		typeof(Select<ARInvoice,
			Where<ARInvoice.docType, Equal<Current<ARRegister.docType>>, 
				And<ARInvoice.refNbr, Equal<Current<ARRegister.refNbr>>,
				And<ARInvoice.origModule, Equal<BatchModule.moduleSO>,
				And<ARInvoice.released, Equal<False>>>>>>),
		typeof(Select<ARCashSale, 
			Where<ARCashSale.docType, Equal<Current<ARRegister.docType>>, 
			And<ARCashSale.refNbr, Equal<Current<ARRegister.refNbr>>>>>),
		typeof(Select<ARInvoice, 
			Where<ARInvoice.docType, Equal<Current<ARRegister.docType>>, 
			And<ARInvoice.refNbr, Equal<Current<ARRegister.refNbr>>>>>),
		typeof(Select<ARPayment, 
			Where<ARPayment.docType, Equal<Current<ARRegister.docType>>, 
			And<ARPayment.refNbr, Equal<Current<ARRegister.refNbr>>>>>)
		})]
	[System.SerializableAttribute()]
	[ARRegisterCacheName(Messages.ARDocument)]	
	[DebuggerDisplay("DocType = {DocType}, RefNbr = {RefNbr}")]
	public partial class ARRegister : PX.Data.IBqlTable, IRegister
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;

		/// <summary>
		/// Indicates whether the record is selected for processing.
		/// </summary>
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

		/// <summary>
		/// Identifier of the <see cref="Branch"/>, to which the document belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Branch.BranchID"/> field.
		/// </value>
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
			public const int Length = 3;
		}
		protected String _DocType;

		/// <summary>
		/// Key field.
		/// The type of the document.
		/// </summary>
		/// <value>
		/// Allowed values are:
		/// <c>"INV"</c> - Invoice,
		/// <c>"DRM"</c> - Debit Memo,
		/// <c>"CRM"</c> - Credit Memo,
		/// <c>"PMT"</c> - Payment,
		/// <c>"RPM"</c> - Void Payment,
		/// <c>"PPM"</c> - Prepayment,
		/// <c>"REF"</c> - Refund,
		/// <c>"FCH"</c> - Financial Charge,
		/// <c>"SMB"</c> - Small Balance Write-Off,
		/// <c>"SMC"</c> - Small Credit Write-Off,
		/// <c>"CSL"</c> - Cash Sale,
		/// <c>"RCS"</c> - Cash Return.
		/// </value>
		[PXDBString(docType.Length, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[ARDocType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
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
		#region PrintDocType
		public abstract class printDocType : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The type of the document for printing. Used in reports.
		/// </summary>
		/// <value>
		/// The value of this field is determined solely by the <see cref="DocType"/> field.
		/// </value>
		[PXString(3, IsFixed = true)]
		[ARDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintDocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;

		/// <summary>
		/// Key field.
		/// The reference number of the document.
		/// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[PXSelector(typeof(Search<ARRegister.refNbr, Where<ARRegister.docType, Equal<Optional<ARRegister.docType>>>>),Filterable=true)]
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
		}
		protected String _OrigModule;

		/// <summary>
		/// Module, from which the document originates.
		/// </summary>
		/// <value>
		/// Code of the module of the system. Defaults to "AR".
		/// Possible values are: "GL", "AP", "AR", "CM", "CA", "IN", "DR", "FA", "PM", "TX", "SO", "PO".
		/// </value>
		[PXDBString(2, IsFixed = true)]
		[PXDefault(GL.BatchModule.AR)]
		[PXUIField(DisplayName = "Source", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[GL.BatchModule.FullList()]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;

		/// <summary>
		/// The date of the document.
		/// </summary>
		/// <value>
		/// Defaults to the current <see cref="AccessInfo.BusinessDate">Business Date</see>.
		/// </value>
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region OrigDocDate
		public abstract class origDocDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrigDocDate;

		/// <summary>
		/// The date of the original document (e.g. the one reversed by this document).
		/// </summary>
		[PXDBDate()]
		public virtual DateTime? OrigDocDate
		{
			get
			{
				return this._OrigDocDate;
			}
			set
			{
				this._OrigDocDate = value;
			}
		}
		#endregion
        #region DueDate
        public abstract class dueDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _DueDate;

		/// <summary>
		/// The due date of the document.
		/// </summary>
        [PXDBDate()]
        [PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DueDate
        {
            get
            {
                return this._DueDate;
            }
            set
            {
                this._DueDate = value;
            }
        }
        #endregion
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;

		/// <summary>
		/// <see cref="FinPeriod">Financial Period</see> of the document.
		/// </summary>
		/// <value>
		/// Determined by the <see cref="ARRegister.DocDate">date of the document</see>. Unlike <see cref="ARRegister.FinPeriodID"/>
		/// the value of this field can't be overriden by user.
		/// </value>
		[TranPeriodID(typeof(ARRegister.docDate))]
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

		/// <summary>
		/// <see cref="FinPeriod">Financial Period</see> of the document.
		/// </summary>
		/// <value>
		/// Defaults to the period, to which the <see cref="APRegister.DocDate"/> belongs, but can be overriden by user.
		/// </value>
		[AROpenPeriod(typeof(ARRegister.docDate))]
        [PXDefault()]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;

		/// <summary>
		/// Identifier of the <see cref="Customer"/>, whom the document belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="BAccount.BAccountID"/> field.
		/// </value>
		[CustomerActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName),Filterable=true, TabOrder=2)]
		[PXDefault()]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region CustomerID_Customer_acctName
		public abstract class customerID_Customer_acctName : IBqlField { }
		#endregion
		#region CustomerLocationID
		public abstract class customerLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerLocationID;

		/// <summary>
		/// Identifier of the <see cref="Location"/> of the Customer.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="BAccount.DefLocationID">Default Location</see> of the <see cref="CustomerID">Customer</see> if it is specified,
		/// or to the first found <see cref="Location"/>, associated with the Customer.
		/// Corresponds to the <see cref="Location.LocationID"/> field.
		/// </value>
		[LocationID(typeof(Where<Location.bAccountID, Equal<Optional<ARRegister.customerID>>,
			And<Location.isActive, Equal<True>,
			And<MatchWithBranch<Location.cBranchID>>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, TabOrder = 3)]
		[PXDefault(typeof(Coalesce<
			Search2<BAccountR.defLocationID,
			InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
			Where<BAccountR.bAccountID, Equal<Current<ARRegister.customerID>>,
				And<CRLocation.isActive, Equal<True>,	And<MatchWithBranch<CRLocation.cBranchID>>>>>,
			Search<CRLocation.locationID, 
			Where<CRLocation.bAccountID, Equal<Current<ARRegister.customerID>>, 
			And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.cBranchID>>>>>>))]
		public virtual Int32? CustomerLocationID
		{
			get
			{
				return this._CustomerLocationID;
			}
			set
			{
				this._CustomerLocationID = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;

		/// <summary>
		/// The code of the <see cref="Currency"/> of the document.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// Corresponds to the <see cref="Currency.CuryID"/> field.
		/// </value>
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Company.baseCuryID>))]
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
		#region ARAccountID
		public abstract class aRAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ARAccountID;

		/// <summary>
		/// !REV!
		/// Identifier of the liability <see cref="Account"/>, to which the document will be posted.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Account.AccountID"/> field.
		/// </value>
		[PXDefault]
        [Account(typeof(ARRegister.branchID), typeof(Search<Account.accountID,
                    Where2<Match<Current<AccessInfo.userName>>,
                         And<Account.active, Equal<True>,
                         And<Account.isCashAccount, Equal<False>,
                         And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
                          Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>>>), DisplayName = "AR Account")]
        public virtual Int32? ARAccountID
		{
			get
			{
				return this._ARAccountID;
			}
			set
			{
				this._ARAccountID = value;
			}
		}
		#endregion
		#region ARSubID
		public abstract class aRSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ARSubID;

		/// <summary>
		/// Identifier of the <see cref="Sub">Subaccount</see>, to which the document will be posted.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Sub.SubID"/> field.
		/// </value>
		[PXDefault]
		[SubAccount(typeof(ARRegister.aRAccountID), DescriptionField = typeof(Sub.description), DisplayName = "AR Subaccount", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? ARSubID
		{
			get
			{
				return this._ARSubID;
			}
			set
			{
				this._ARSubID = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;

		/// <summary>
		/// Counter of the document lines, used <i>internally</i> to assign numbers to newly created <see cref="ARTran">lines</see>.
		/// It is not recommended to rely on this field to determine the exact count of lines, because it might not reflect the latter under various conditions.
		/// </summary>
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
		#region AdjCntr
		public abstract class adjCntr : PX.Data.IBqlField
		{
		}
		protected int? _AdjCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual int? AdjCntr
		{
			get
			{
				return _AdjCntr;
			}
			set
			{
				_AdjCntr = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;

		/// <summary>
		/// Identifier of the <see cref="CurrencyInfo">CurrencyInfo</see> object associated with the document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="CurrencyInfoID"/> field.
		/// </value>
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "AR")]
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
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;

		/// <summary>
		/// The amount of the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.origDocAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;

		/// <summary>
		/// The amount of the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;

		/// <summary>
		/// The open balance of the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.docBal), BaseCalc=false)]
		[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;

		/// <summary>
		/// The open balance of the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region CuryOrigDiscAmt
		public abstract class curyOrigDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDiscAmt;

		/// <summary>
		/// !REV!
		/// The cash discount allowed for the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.origDiscAmt))]
		[PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDiscAmt
		{
			get
			{
				return this._CuryOrigDiscAmt;
			}
			set
			{
				this._CuryOrigDiscAmt = value;
			}
		}
		#endregion
		#region OrigDiscAmt
		public abstract class origDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDiscAmt;

		/// <summary>
		/// !REV!
		/// The cash discount allowed for the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigDiscAmt
		{
			get
			{
				return this._OrigDiscAmt;
			}
			set
			{
				this._OrigDiscAmt = value;
			}
		}
		#endregion
		#region CuryDiscTaken
		public abstract class curyDiscTaken : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscTaken;

		/// <summary>
		/// The cash discount actually on the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discTaken))]
		public virtual Decimal? CuryDiscTaken
		{
			get
			{
				return this._CuryDiscTaken;
			}
			set
			{
				this._CuryDiscTaken = value;
			}
		}
		#endregion
		#region DiscTaken
		public abstract class discTaken : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscTaken;

		/// <summary>
		/// The cash discount actually on the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscTaken
		{
			get
			{
				return this._DiscTaken;
			}
			set
			{
				this._DiscTaken = value;
			}
		}
		#endregion
		#region CuryDiscBal
		public abstract class curyDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscBal;

		/// <summary>
		/// The cash discount balance of the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
        [PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discBal), BaseCalc = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDiscBal
		{
			get
			{
				return this._CuryDiscBal;
			}
			set
			{
				this._CuryDiscBal = value;
			}
		}
		#endregion
		#region DiscBal
		public abstract class discBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscBal;

		/// <summary>
		/// The cash discount balance of the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscBal
		{
			get
			{
				return this._DiscBal;
			}
			set
			{
				this._DiscBal = value;
			}
		}
		#endregion

        #region DiscTot
        public abstract class discTot : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscTot;

		/// <summary>
		/// !REV!
		/// The total group and document discount for the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscTot
        {
            get
            {
                return this._DiscTot;
            }
            set
            {
                this._DiscTot = value;
            }
        }
        #endregion
        #region CuryDiscTot
        public abstract class curyDiscTot : PX.Data.IBqlField
        {
        }
		protected Decimal? _CuryDiscTot;

		/// <summary>
		/// !REV!
		/// The total group and document discount for the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
        [PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discTot))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Discount Total", Enabled = true)]
        public virtual Decimal? CuryDiscTot
        {
            get
            {
                return this._CuryDiscTot;
            }
            set
            {
                this._CuryDiscTot = value;
            }
        }
        #endregion

        #region DocDisc
        public abstract class docDisc : PX.Data.IBqlField
        {
        }
		protected Decimal? _DocDisc;

		/// <summary>
		/// !REV!
		/// The document discount amount (without group discounts) for the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DocDisc
        {
            get
            {
                return this._DocDisc;
            }
            set
            {
                this._DocDisc = value;
            }
        }
        #endregion
        #region CuryDocDisc
        public abstract class curyDocDisc : PX.Data.IBqlField
        {
        }
		protected Decimal? _CuryDocDisc;

		/// <summary>
		/// !REV!
		/// The document discount amount (without group discounts) for the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
        [PXCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.docDisc))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Document Discount", Enabled = true)]
        public virtual Decimal? CuryDocDisc
        {
            get
            {
                return this._CuryDocDisc;
            }
            set
            {
                this._CuryDocDisc = value;
            }
        }
        #endregion

        #region CuryChargeAmt
        public abstract class curyChargeAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryChargeAmt;

		/// <summary>
		/// The total of all finance charges applied to the document.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// </summary>
        [PXCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.chargeAmt))]
        [PXUIField(DisplayName = "Finance Charges", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? CuryChargeAmt
        {
            get
            {
                return this._CuryChargeAmt;
            }
            set
            {
                this._CuryChargeAmt = value;
            }
        }
        #endregion
        #region ChargeAmt
        public abstract class chargeAmt : PX.Data.IBqlField
        {
        }
		protected Decimal? _ChargeAmt;

		/// <summary>
		/// The total of all finance charges applied to the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
        [PXDecimal(4)]
        public virtual Decimal? ChargeAmt
        {
            get
            {
                return this._ChargeAmt;
            }
            set
            {
                this._ChargeAmt = value;
            }
        }
        #endregion
		#region DocDesc
		public abstract class docDesc : PX.Data.IBqlField
		{
		}
		protected String _DocDesc;

		/// <summary>
		/// The description of the document.
		/// </summary>
		[PXDBString(150, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
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
		#region DocClass
		public abstract class docClass : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Reserved for internal use.
		/// The read-only class of the document determined by the <see cref="DocType"/>.
		/// Affects the way the document is posted to the General Ledger.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="GLTran.TranClass"/> field.
		/// </value>
		[PXString(1, IsFixed = true)]
		public virtual string DocClass
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.DocClass(_DocType);
			}
			set
			{
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;

		/// <summary>
		/// The number of the <see cref="Batch"/> created from the document on release.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Batch.BatchNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAR>>>))]
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
		#region BatchSeq
		public abstract class batchSeq : PX.Data.IBqlField
		{
		}
		protected Int16? _BatchSeq;

		/// <summary>
		/// !REV!
		/// </summary>
		[PXDBShort()]
		[PXDefault((short)0)]
		public virtual Int16? BatchSeq
		{
			get
			{
				return this._BatchSeq;
			}
			set
			{
				this._BatchSeq = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;

		/// <summary>
		/// When set to <c>true</c>, indicates that the document has been released.
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
		#region OpenDoc
		public abstract class openDoc : PX.Data.IBqlField
		{
		}
		protected Boolean? _OpenDoc;

		/// <summary>
		/// When set to <c>true</c>, indicates that the document is open.
		/// </summary>
		[PXDBBool()]
		[PXDefault(true)]
		public virtual Boolean? OpenDoc
		{
			get
			{
				return this._OpenDoc;
			}
			set
			{
				this._OpenDoc = value;
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;

		/// <summary>
		/// When set to <c>true</c> indicates that the document is on hold and thus cannot be released.
		/// </summary>
		[PXDBBool()]
		[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
		[PXDefault(true, typeof(ARSetup.holdEntry))]
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
		#region Scheduled
		public abstract class scheduled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Scheduled;

		/// <summary>
		/// When set to <c>true</c> indicates that the document is part of a <c>Schedule</c> and serves as a template for generating other documents according to it.
		/// </summary>
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;

		/// <summary>
		/// When set to <c>true</c> indicates that the document has been voided.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
			}
		}
		#endregion
		#region SelfVoidingDoc
		public abstract class selfVoidingDoc : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// When <c>true</c>, indicates that the document can be voided only in full and 
		/// it is not allow to delete reversing applications partially.
		/// </summary>
		[PXBool()]
		[PXDefault(false)]
		[PXFormula(typeof(IIf<Where<ARRegister.docType, Equal<ARDocType.smallBalanceWO>, Or<ARRegister.docType, Equal<ARDocType.smallCreditWO>>>, True, False>))]
		public virtual bool? SelfVoidingDoc { get; set; }
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
		#region ClosedFinPeriodID
		public abstract class closedFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _ClosedFinPeriodID;

		/// <summary>
		/// The <see cref="FinancialPeriod">Financial Period</see>, in which the document was closed.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="FinPeriodID"/> field.
		/// </value>
		[FinPeriodID()]
		[PXUIField(DisplayName = "Closed Period", Visibility = PXUIVisibility.Invisible)]
		public virtual String ClosedFinPeriodID
		{
			get
			{
				return this._ClosedFinPeriodID;
			}
			set
			{
				this._ClosedFinPeriodID = value;
			}
		}
		#endregion
		#region ClosedTranPeriodID
		public abstract class closedTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _ClosedTranPeriodID;

		/// <summary>
		/// The <see cref="FinancialPeriod">Financial Period</see>, in which the document was closed.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="TranPeriodID"/> field.
		/// </value>
		[FinPeriodID()]
		[PXUIField(DisplayName = "Closed Period", Visibility = PXUIVisibility.Invisible)]
		public virtual String ClosedTranPeriodID
		{
			get
			{
				return this._ClosedTranPeriodID;
			}
			set
			{
				this._ClosedTranPeriodID = value;
			}
		}
		#endregion
		#region RGOLAmt
		public abstract class rGOLAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _RGOLAmt;

		/// <summary>
		/// Realized Gain or Loss amount associated with the document.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? RGOLAmt
		{
			get
			{
				return this._RGOLAmt;
			}
			set
			{
				this._RGOLAmt = value;
			}
		}
		#endregion
        #region CuryRoundDiff
        public abstract class curyRoundDiff : IBqlField { }

		/// <summary>
		/// The difference between the original amount of the document and the rounded amount.
		/// Given in the <see cref="CuryID">currency of the document</see>.
		/// Applicable only if <see cref="FeaturesSet.InvoiceRounding">Invoice Rounding</see> feature is enabled.
		/// </summary>
        [PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.roundDiff), BaseCalc = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Rounding Diff.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public decimal? CuryRoundDiff
        {
            get;
            set;
        }
        #endregion
        #region RoundDiff
        public abstract class roundDiff : IBqlField { }

		/// <summary>
		/// The difference between the original amount of the document and the rounded amount.
		/// Given in the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// Applicable only if <see cref="FeaturesSet.InvoiceRounding">Invoice Rounding</see> feature is enabled.
		/// </summary>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public decimal? RoundDiff
        {
            get;
            set;
        }
        #endregion
		#region Payable

		/// <summary>
		/// Read-only field indicating whether the document is payable. Depends solely on the <see cref="DocType"/> field.
		/// Opposite of the <see cref="Paying"/> field.
		/// </summary>
		public virtual Boolean? Payable
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.Payable(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region Paying

		/// <summary>
		/// Read-only field indicating whether the document is paying. Depends solely on the <see cref="DocType"/> field.
		/// Opposite of the <see cref="Payable"/> field.
		/// </summary>
		public virtual Boolean? Paying
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return (ARDocType.Payable(this._DocType) == false);
			}
			set
			{
			}
		}
		#endregion
		#region SortOrder

		/// <summary>
		/// Read-only field determining the sort order for AP documents based on the <see cref="DocType"/> field.
		/// </summary>
		public virtual Int16? SortOrder
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.SortOrder(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region SignBalance

		/// <summary>
		/// Read-only field indicating the sign of the document's impact on AR balance .
		/// Depends solely on the <see cref="DocType"/> field.
		/// </summary>
		/// <value>
		/// Possible values are: <c>1</c>, <c>-1</c> or <c>0</c>.
		/// </value>
		public virtual Decimal? SignBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.SignBalance(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region SignAmount

		/// <summary>
		/// Read-only field indicating the sign of the document amount.
		/// Depends solely on the <see cref="DocType"/> field.
		/// </summary>
		/// <value>
		/// Possible values are: <c>1</c>, <c>-1</c> or <c>0</c>.
		/// </value>
		public virtual Decimal? SignAmount
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.SignAmount(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region ScheduleID
		public abstract class scheduleID : IBqlField
		{
		}
		protected string _ScheduleID;

		/// <summary>
		/// Identifier of the <see cref="Schedule" />, associated with the document.
		/// In case <see cref="Scheduled"/> is <c>true</c>, the field points to the Schedule, to which the document belongs as a template.
		/// Otherwise, the field points to the Schedule, from which this document was generated, if any.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Schedule.ScheduleID"/> field.
		/// </value>
		[PXDBString(10, IsUnicode = true)]
		public virtual string ScheduleID
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
		#region ImpRefNbr
		public abstract class impRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ImpRefNbr;

		/// <summary>
		/// Implementation specific reference number of the document.
		/// This field is neither filled nor used by the core Acumatica itself, but may be utilized by customizations or extensions.
		/// </summary>
		[PXDBString(15, IsUnicode = true)]
		public virtual String ImpRefNbr
		{
			get
			{
				return this._ImpRefNbr;
			}
			set
			{
				this._ImpRefNbr = value;
			}
		}
		#endregion
		#region StatementDate
		public abstract class statementDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StatementDate;

		/// <summary>
		/// The date of the <see cref="ARStatement">Customer Statement</see>, in which the document is reported.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="ARStatement.StatementDate"/> field.
		/// </value>
		[PXDBDate()]
		public virtual DateTime? StatementDate
		{
			get
			{
				return this._StatementDate;
			}
			set
			{
				this._StatementDate = value;
			}
		}
		#endregion
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesPersonID;

		/// <summary>
		/// !REV!
		/// Identifier of the <see cref="CustSalesPeople">Salesperson</see>, whom the document belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="CustDefSalesPeople.SalesPersonID"/> field.
		/// </value>
		[SalesPerson()]
		[PXDefault(typeof(Search<CustDefSalesPeople.salesPersonID, Where<CustDefSalesPeople.bAccountID, Equal<Current<ARRegister.customerID>>, And<CustDefSalesPeople.locationID, Equal<Current<ARRegister.customerLocationID>>, And<CustDefSalesPeople.isDefault, Equal<True>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? SalesPersonID
		{
			get
			{
				return this._SalesPersonID;
			}
			set
			{
				this._SalesPersonID = value;
			}
		}
		#endregion
		#region IsTaxValid
		public abstract class isTaxValid : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// When <c>true</c>, indicates that the amount of tax calculated with the external Tax Engine(Avalara) is up to date.
		/// If this field equals <c>false</c>, the document was updated since last synchronization with the Tax Engine
		/// and taxes might need recalculation.
		/// </summary>
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

		/// <summary>
		/// When <c>true</c>, indicates that the tax information was successfully commited to the external Tax Engine(Avalara).
		/// </summary>
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

		/// <summary>
		/// Indicates whether the tax information related to the document was saved to the external Tax Engine (Avalara).
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Tax is saved in external Tax Engine(Avalara)", Enabled = false)]
		public virtual Boolean? IsTaxSaved
		{
			get;
			set;
		}
		#endregion
		#region OrigDocType
		public abstract class origDocType : PX.Data.IBqlField
		{
		}
		protected String _OrigDocType;

		/// <summary>
		/// The type of the original (source) document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="DocType"/> field.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[ARDocType.List()]
		[PXUIField(DisplayName = "Orig. Doc. Type")]
		public virtual String OrigDocType
		{
			get
			{
				return this._OrigDocType;
			}
			set
			{
				this._OrigDocType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;

		/// <summary>
		/// The reference number of the original (source) document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="RefNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Orig. Ref. Nbr.")]
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
		#region Status
		public abstract class status : IBqlField {}
		protected string _Status;

		/// <summary>
		/// !REV!
		/// The status of the document.
		/// The value of the field is determined by the values of the status flags. Can't be changed directly.
		/// (The status flags are: <see cref="Hold"/>, <see cref="Released"/>, <see cref="Voided"/>, <see cref="Scheduled"/>, etc.)
		/// </summary>
		/// <value>
		/// Possible values are:
		/// <c>"W"</c> - Pending Credit Card Processing,
		/// <c>"R"</c> - Credit Hold,
		/// <c>"H"</c> - Hold,
		/// <c>"B"</c> - Balanced,
		/// <c>"V"</c> - Voided,
		/// <c>"S"</c> - Scheduled,
		/// <c>"N"</c> - Open,
		/// <c>"C"</c> - Closed,
		/// <c>"P"</c> - Pending Print,
		/// <c>"E"</c> - Pending Email,
		/// <c>"Z"</c> - Reserved.
		/// Defaults to Hold (<c>"H"</c>).
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(ARDocStatus.Hold)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[ARDocStatus.List]
		[SetStatus]
		[PXDependsOnFields(
			typeof(ARRegister.voided), 
			typeof(ARRegister.hold), 
			typeof(ARRegister.scheduled), 
			typeof(ARRegister.released), 
			typeof(ARRegister.openDoc))]
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

		#region CuryDiscountedDocTotal
		public abstract class curyDiscountedDocTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryDiscountedDocTotal;

		/// <summary>
		/// The discounted amount of the document.
		/// Given in the <see cref="CuryID"> currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discountedDocTotal))]
		[PXUIField(DisplayName = "Discounted Doc. Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? CuryDiscountedDocTotal
		{
			get
			{
				return _CuryDiscountedDocTotal;
			}
			set
			{
				_CuryDiscountedDocTotal = value;
			}
		}
		#endregion
		#region DiscountedDocTotal
		public abstract class discountedDocTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _DiscountedDocTotal;

		/// <summary>
		/// The discounted amount of the document.
		/// Given in the <see cref="Company.BaseCuryID"> base currency of the company</see>.
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? DiscountedDocTotal
		{
			get
			{
				return _DiscountedDocTotal;
			}
			set
			{
				_DiscountedDocTotal = value;
			}
		}
		#endregion
		#region CuryDiscountedTaxableTotal
		public abstract class curyDiscountedTaxableTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryDiscountedTaxableTotal;

		/// <summary>
		/// The total taxable amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="CuryID"> currency of the document</see>.
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discountedTaxableTotal))]
		[PXUIField(DisplayName = "Discounted Taxable Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? CuryDiscountedTaxableTotal
		{
			get
			{
				return _CuryDiscountedTaxableTotal;
			}
			set
			{
				_CuryDiscountedTaxableTotal = value;
			}
		}
		#endregion
		#region DiscountedTaxableTotal
		public abstract class discountedTaxableTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _DiscountedTaxableTotal;

		/// <summary>
		/// The total taxable amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="Company.BaseCuryID"> base currency of the company</see>.
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? DiscountedTaxableTotal
		{
			get
			{
				return _DiscountedTaxableTotal;
			}
			set
			{
				_DiscountedTaxableTotal = value;
			}
		}
		#endregion
		#region CuryDiscountedPrice
		public abstract class curyDiscountedPrice : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryDiscountedPrice;

		/// <summary>
		/// The total tax amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="CuryID"> currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCurrency(typeof(ARRegister.curyInfoID), typeof(ARRegister.discountedPrice))]
		[PXUIField(DisplayName = "Tax on Discounted Price", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? CuryDiscountedPrice
		{
			get
			{
				return _CuryDiscountedPrice;
			}
			set
			{
				_CuryDiscountedPrice = value;
			}
		}
		#endregion
		#region DiscountedPrice
		public abstract class discountedPrice : PX.Data.IBqlField
		{
		}
		protected decimal? _DiscountedPrice;

		/// <summary>
		/// The total tax amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="Company.BaseCuryID"> base currency of the company</see>.
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? DiscountedPrice
		{
			get
			{
				return _DiscountedPrice;
			}
			set
			{
				_DiscountedPrice = value;
			}
		}
		#endregion

		#region HasPPDTaxes
		public abstract class hasPPDTaxes : PX.Data.IBqlField
		{
		}
		protected bool? _HasPPDTaxes;

		/// <summary>
		/// When <c>true</c>, indicates that the document has taxes, that are reduces cash discount taxable amount on early payment.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? HasPPDTaxes
		{
			get
			{
				return _HasPPDTaxes;
			}
			set
			{
				_HasPPDTaxes = value;
			}
		}
		#endregion
		#region PendingPPD
		public abstract class pendingPPD : PX.Data.IBqlField
		{
		}
		protected bool? _PendingPPD;

		/// <summary>
		/// When <c>true</c>, indicates that the document has been paid in full and 
		/// to close the document, you need to apply the cash discount by generating a credit memo on the "Generate VAT Credit Memos" (AR.50.45.00) form.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? PendingPPD
		{
			get
			{
				return _PendingPPD;
			}
			set
			{
				_PendingPPD = value;
			}
		}
		#endregion

		internal string WarningMessage { get; set; }


        public class SetStatusAttribute : PXEventSubscriberAttribute, IPXRowUpdatingSubscriber, IPXRowInsertingSubscriber
        {
            public override void CacheAttached(PXCache sender)
            {
                base.CacheAttached(sender);

                sender.Graph.FieldUpdating.AddHandler<ARRegister.hold>((cache, e) =>
                {
                    PXBoolAttribute.ConvertValue(e);

                    ARRegister item = e.Row as ARRegister;
                    if (item != null)
                    {
                        StatusSet(cache, item, (bool?)e.NewValue);
                    }
                });

				sender.Graph.FieldVerifying.AddHandler(sender.GetItemType(), "Status", (cache, e) => { e.NewValue = cache.GetValue<ARRegister.status>(e.Row); });
				sender.Graph.RowSelecting.AddHandler(sender.GetItemType(), RowSelecting);
            }

            protected virtual void StatusSet(PXCache cache, ARRegister item, bool? HoldVal)
            {
                if (item.Voided == true)
                {
                    item.Status = ARDocStatus.Voided;
                }
                else if (HoldVal == true)
                {
	                if (item.Released == true)
	                {
		                item.Status = ARDocStatus.Reserved;
	                }
	                else
	                {
		                item.Status = ARDocStatus.Hold;
					}
                }
                else if (item.Scheduled  == true)
                {
                    item.Status = ARDocStatus.Scheduled;
                }
                else if (item.Released == false)
                {
                    item.Status = ARDocStatus.Balanced;
                }
                else if (item.OpenDoc == true)
                {
	                item.Status = ARDocStatus.Open;
                }
                else if (item.OpenDoc == false)
                {
                    item.Status = ARDocStatus.Closed;
                }
            }

            public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
            {
                ARRegister item = (ARRegister)e.Row;
                if (item != null)
                    StatusSet(sender, item, item.Hold);
            }

            public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
            {
                ARRegister item = (ARRegister)e.Row;
                StatusSet(sender, item, item.Hold);
            }

            public virtual void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
            {
                ARRegister item = (ARRegister)e.NewRow;
                StatusSet(sender, item, item.Hold);
            }
        }
	}

	[PXProjection(typeof(Select2<ARRegister,
		InnerJoinSingleTable<Customer, On<Customer.bAccountID, Equal<ARRegister.customerID>>>>))]
	[PXBreakInheritance]
    [Serializable]
	public partial class ARRegisterAccess : Customer
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(ARRegister.docType))]
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
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARRegister.refNbr))]
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
		#region Scheduled
		public abstract class scheduled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Scheduled;
		[PXDBBool(BqlField = typeof(ARRegister.scheduled))]
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
		#region ScheduleID
		public abstract class scheduleID : IBqlField
		{
		}
		protected string _ScheduleID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(ARRegister.scheduleID))]
		public virtual string ScheduleID
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
	}

	[Serializable]
	public partial class ARRegisterAR622000 : IBqlTable
	{
		#region RefNbr
		[PXSelector(typeof(Search2<ARRegister.refNbr,
			InnerJoinSingleTable<Customer, On<ARRegister.customerID, Equal<Customer.bAccountID>>>,
			Where<ARRegister.docType, Equal<Optional<ARRegister.docType>>,
			And<ARRegister.released, Equal<True>,
			And2<Where<ARRegister.finPeriodID, GreaterEqual<Optional<ARRegister.finPeriodID>>, Or<Optional<ARRegister.closedFinPeriodID>, IsNull>>,
			And2<Where<ARRegister.finPeriodID, LessEqual<Optional<ARRegister.tranPeriodID>>, Or<Optional<ARRegister.closedTranPeriodID>, IsNull>>,
			And<Match<Customer, Current<AccessInfo.userName>>>>>>>, OrderBy<Desc<ARRegister.refNbr>>>), Filterable = true)]
		public String RefNbr { get; set; }
		#endregion
	}

	[Serializable]
	public partial class ARRegisterAR610500 : IBqlTable
	{
		#region RefNbr
		[PXSelector(typeof(Search2<ARRegister.refNbr,
			InnerJoinSingleTable<Customer, On<ARRegister.customerID, Equal<Customer.bAccountID>>>,
			Where<ARRegister.docType, Equal<Optional<ARRegister.docType>>,
				And2<Where<ARRegister.finPeriodID, GreaterEqual<Optional<ARRegister.finPeriodID>>, Or<Optional<ARRegister.closedFinPeriodID>, IsNull>>,
				And2<Where<ARRegister.finPeriodID, LessEqual<Optional<ARRegister.tranPeriodID>>, Or<Optional<ARRegister.closedTranPeriodID>, IsNull>>,
				And2<Where<ARRegister.hold, Equal<False>, 
					And<ARRegister.scheduled, Equal<False>, 
					And<ARRegister.voided, Equal<False>>>>, 
				And<Match<Customer, Current<AccessInfo.userName>>>>>>>, 
			OrderBy<Desc<ARRegister.refNbr>>>), Filterable = true)]
		public String RefNbr { get; set; }
		#endregion
	}
}
