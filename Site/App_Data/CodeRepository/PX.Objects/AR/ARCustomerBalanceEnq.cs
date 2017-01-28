using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CR;

namespace PX.Objects.AR
{
	[Serializable]
	[PXProjection(typeof(Select5<ARHistory,
		InnerJoin<GL.FinPeriod,
			On<GL.FinPeriod.finPeriodID, GreaterEqual<ARHistory.finPeriodID>>>,
		Aggregate<
		GroupBy<ARHistory.branchID,
		GroupBy<ARHistory.customerID,
		GroupBy<ARHistory.accountID,
		GroupBy<ARHistory.subID,
		Max<ARHistory.finPeriodID,
		GroupBy<GL.FinPeriod.finPeriodID>>>>>>>>))]
	[PXPrimaryGraph(
		new Type[] {
			typeof(ARDocumentEnq),
			typeof(ARCustomerBalanceEnq)
		},
		new Type[] {
			typeof(Where<BaseARHistoryByPeriod.customerID, IsNotNull>),
			typeof(Where<BaseARHistoryByPeriod.customerID, IsNull>)
		},
		Filters = new Type[] {
			typeof(ARDocumentEnq.ARDocumentFilter),
			typeof(ARCustomerBalanceEnq.ARHistoryFilter)
		})]
	[PXCacheName(Messages.BaseARHistoryByPeriod)]
	public partial class BaseARHistoryByPeriod : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(IsKey = true, BqlField = typeof(ARHistory.branchID))]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer(IsKey = true, BqlField = typeof(ARHistory.customerID), CacheGlobal = true)]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(IsKey = true, BqlField = typeof(ARHistory.accountID))]
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
		[SubAccount(IsKey = true, BqlField = typeof(ARHistory.subID))]
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;
		[GL.FinPeriodID(BqlField = typeof(ARHistory.finPeriodID))]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(GL.FinPeriod.finPeriodID))]
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

	}


	[Serializable]
	[PXProjection(typeof(Select5<CuryARHistory,
		InnerJoin<GL.FinPeriod,
			On<GL.FinPeriod.finPeriodID, GreaterEqual<CuryARHistory.finPeriodID>>>,
		Aggregate<
		GroupBy<CuryARHistory.branchID,
		GroupBy<CuryARHistory.customerID,
		GroupBy<CuryARHistory.accountID,
		GroupBy<CuryARHistory.subID,
		GroupBy<CuryARHistory.curyID,
		Max<CuryARHistory.finPeriodID,
		GroupBy<GL.FinPeriod.finPeriodID>>>>>>>>>))]
	[PXCacheName(Messages.ARHistoryByPeriod)]
	public partial class ARHistoryByPeriod : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(IsKey = true, BqlField = typeof(CuryARHistory.branchID))]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer(IsKey = true, BqlField = typeof(CuryARHistory.customerID))]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(IsKey = true, BqlField = typeof(CuryARHistory.accountID))]
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
		[SubAccount(IsKey = true, BqlField = typeof(CuryARHistory.subID))]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">LLLLL", BqlField = typeof(CuryARHistory.curyID))]
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;
		[GL.FinPeriodID(BqlField = typeof(CuryARHistory.finPeriodID))]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(GL.FinPeriod.finPeriodID))]
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

	}

	[PX.Objects.GL.TableAndChartDashboardType]
	public class ARCustomerBalanceEnq : PXGraph<ARCustomerBalanceEnq>
	{
		#region Internal Types
		[Serializable]
		public partial class ARHistoryFilter : IBqlTable
		{
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[Branch()]
			[PXRestrictor(typeof(Where<True, Equal<True>>), GL.Messages.BranchInactive, ReplaceInherited = true)]
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
			#region ARAcctID
			public abstract class aRAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _ARAcctID;
			[GL.Account(null,typeof(Search5<Account.accountID,
						InnerJoin<ARHistory, On<Account.accountID, Equal<ARHistory.accountID>>>,
						Where<Match<Current<AccessInfo.userName>>>,
					   Aggregate<GroupBy<Account.accountID>>>),
				DisplayName = "AR Account", DescriptionField = typeof(GL.Account.description))]
			public virtual Int32? ARAcctID
			{
				get
				{
					return this._ARAcctID;
				}
				set
				{
					this._ARAcctID = value;
				}
			}
			#endregion
			#region ARSubID
			public abstract class aRSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _ARSubID;
			[GL.SubAccount(DisplayName = "AR Sub.", DescriptionField = typeof(GL.Sub.description), Visible = false)]
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
			#region SubCD
			public abstract class subCD : PX.Data.IBqlField
			{
			}
			protected String _SubCD;
			[PXDBString(30, IsUnicode = true)]
			[PXUIField(DisplayName = "AR Subaccount", Visibility = PXUIVisibility.Invisible, FieldClass = SubAccountAttribute.DimensionName)]
			[PXDimension("SUBACCOUNT", ValidComboRequired = false)]
			public virtual String SubCD
			{
				get
				{
					return this._SubCD;
				}
				set
				{
					this._SubCD = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
			[PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
			[PXUIField(DisplayName = "Currency ID")]
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
			#region CustomerClassID
			public abstract class customerClassID : PX.Data.IBqlField
			{
			}
			protected String _CustomerClassID;
			[PXDBString(10, IsUnicode = true)]
			[PXSelector(typeof(CustomerClass.customerClassID), DescriptionField = typeof(CustomerClass.descr), CacheGlobal = true)]
			[PXUIField(DisplayName = "Customer Class")]
			public virtual String CustomerClassID
			{
				get
				{
					return this._CustomerClassID;
				}
				set
				{
					this._CustomerClassID = value;
				}
			}
			#endregion
			#region Status
			public abstract class status : PX.Data.IBqlField
			{
			}
			protected String _Status;
			[PXDBString(1, IsFixed = true)]
			[PXUIField(DisplayName = "Status")]
			[BAccount.status.List()]
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
			#region ShowWithBalanceOnly
			public abstract class showWithBalanceOnly : PX.Data.IBqlField
			{
			}
			protected bool? _ShowWithBalanceOnly;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Customers with Balance Only")]
			public virtual bool? ShowWithBalanceOnly
			{
				get
				{
					return this._ShowWithBalanceOnly;
				}
				set
				{
					this._ShowWithBalanceOnly = value;
				}
			}
			#endregion
			#region Period
			public abstract class period : PX.Data.IBqlField
			{
			}
			protected String _Period;
			
			[ARAnyPeriodFilterable()]
			[PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.Visible)]
			public virtual String Period
			{
				get
				{
					return this._Period;
				}
				set
				{
					this._Period = value;
				}
			}
			#endregion
			#region ByFinancialPeriod
			public abstract class byFinancialPeriod : PX.Data.IBqlField
			{
			}
			protected bool? _ByFinancialPeriod;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "By Financial Period")]
			public virtual bool? ByFinancialPeriod
			{
				get
				{
					return this._ByFinancialPeriod;
				}
				set
				{
					this._ByFinancialPeriod = value;
				}
			}
			#endregion
			#region SubCD Wildcard
			public abstract class subCDWildcard : PX.Data.IBqlField { };
			[PXDBString(30, IsUnicode = true)]
			public virtual String SubCDWildcard
			{
				get
				{
					return SubCDUtils.CreateSubCDWildcard(this._SubCD, SubAccountAttribute.DimensionName);
				}
			}



			#endregion
			#region CuryBalanceSummary
			public abstract class curyBalanceSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryBalanceSummary;
			[PXCury(typeof(ARHistoryFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Balance (Currency)", Enabled = false)]
			public virtual Decimal? CuryBalanceSummary
			{
				get
				{
					return this._CuryBalanceSummary;
				}
				set
				{
					this._CuryBalanceSummary = value;
				}
			}
			#endregion
			#region BalanceSummary
			public abstract class balanceSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _BalanceSummary;
			[PXDBBaseCury]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Balance", Enabled = false)]
			public virtual Decimal? BalanceSummary
			{
				get
				{
					return this._BalanceSummary;
				}
				set
				{
					this._BalanceSummary = value;
				}
			}
			#endregion

			#region CuryRevaluedSummary
			public abstract class curyRevaluedSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryRevaluedSummary;
			[PXCury(typeof(ARHistoryFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Unrealized Gain/Loss", Enabled = false)]
			public virtual Decimal? CuryRevaluedSummary
			{
				get
				{
					return this._CuryRevaluedSummary;
				}
				set
				{
					this._CuryRevaluedSummary = value;
				}
			}
			#endregion
			#region RevaluedSummary
			public abstract class revaluedSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _RevaluedSummary;
			[PXDBBaseCury]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Unrealized Gain/Loss", Enabled = false)]
			public virtual Decimal? RevaluedSummary
			{
				get
				{
					return this._RevaluedSummary;
				}
				set
				{
					this._RevaluedSummary = value;
				}
			}


			#endregion

			#region CuryDepositsSummary
			public abstract class curyDepositsSummary : PX.Data.IBqlField
			{
			}

			protected Decimal? _CuryDepositsSummary;
			[PXCury(typeof(ARHistoryFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Prepayments (Currency)", Enabled = false)]
			public virtual Decimal? CuryDepositsSummary
			{
				get
				{
					return this._CuryDepositsSummary;
				}
				set
				{
					this._CuryDepositsSummary = value;
				}
			}
			#endregion
			#region DepositsSummary
			public abstract class depositsSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _DepositsSummary;
			[PXDBBaseCury]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Prepayments", Enabled = false)]
			public virtual Decimal? DepositsSummary
			{
				get
				{
					return this._DepositsSummary;
				}
				set
				{
					this._DepositsSummary = value;
				}
			}
			#endregion

			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[PXDBInt()]
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
			#region SplitByCurrency
			public abstract class splitByCurrency : PX.Data.IBqlField
			{
			}
			protected bool? _SplitByCurrency;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Split By Currency")]
			public virtual bool? SplitByCurrency
			{
				get
				{
					return this._SplitByCurrency;
				}
				set
				{
					this._SplitByCurrency = value;
				}
			}
			#endregion
			#region IncludeChildAccounts
			public abstract class includeChildAccounts : IBqlField { }

			[PXDBBool]
			[PXDefault(typeof(Search<CS.FeaturesSet.parentChildAccount>))]
			[PXUIField(DisplayName = "Consolidate by Parent")]
			public virtual bool? IncludeChildAccounts { get; set; }
			#endregion
			public virtual void ClearSummary()
			{
				this._BalanceSummary = Decimal.Zero;
				this._RevaluedSummary = Decimal.Zero;
				this._DepositsSummary = Decimal.Zero;
				this._CuryBalanceSummary = Decimal.Zero;
				this._CuryRevaluedSummary = Decimal.Zero;
				this._CuryDepositsSummary = Decimal.Zero;
			}
		}

		[Serializable]
		public partial class ARHistoryResult : IBqlTable
		{
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[PXDBInt()]
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
			#region AcctCD
			public abstract class acctCD : PX.Data.IBqlField
			{
			}
			protected string _AcctCD;
			[PXDimensionSelector(CustomerAttribute.DimensionName, typeof(Customer.acctCD), typeof(acctCD),
				typeof(Customer.acctCD), typeof(Customer.acctName))]
			[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
			[PXUIField(DisplayName = "Customer ID", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String AcctCD
			{
				get
				{
					return this._AcctCD;
				}
				set
				{
					this._AcctCD = value;
				}
			}
			#endregion
			#region AcctName
			public abstract class acctName : PX.Data.IBqlField
			{
			}
			protected String _AcctName;
			[PXDBString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Customer Name", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String AcctName
			{
				get
				{
					return this._AcctName;
				}
				set
				{
					this._AcctName = value;
				}
			}
			#endregion
			#region FinPeriodID
			public abstract class finPeriodID : PX.Data.IBqlField { };
			protected String _FinPeriodID;
			[GL.FinPeriodID()]
			[PXUIField(DisplayName = "Last Activity Period", Visibility = PXUIVisibility.SelectorVisible)]
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
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true)]
			[PXUIField(DisplayName = "Currency ID", Visibility = PXUIVisibility.SelectorVisible)]
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
			#region CuryBegBalance
			public abstract class curyBegBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryBegBalance;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency Beginning Balance", Visible = false)]
			public virtual Decimal? CuryBegBalance
			{
				get
				{
					return this._CuryBegBalance;
				}
				set
				{
					this._CuryBegBalance = value;
				}
			}
			#endregion
			#region BegBalance
			public abstract class begBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _BegBalance;
			[PXBaseCury()]
			[PXUIField(DisplayName = "Beginning Balance", Visible = false)]
			public virtual Decimal? BegBalance
			{
				get
				{
					return this._BegBalance;
				}
				set
				{
					this._BegBalance = value;
				}
			}
			#endregion
			#region CuryEndBalance
			public abstract class curyEndBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryEndBalance;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency Ending Balance", Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Decimal? CuryEndBalance
			{
				get
				{
					return this._CuryEndBalance;
				}
				set
				{
					this._CuryEndBalance = value;
				}
			}
			#endregion
			#region EndBalance
			public abstract class endBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _EndBalance;
			[PXBaseCury()]
			[PXUIField(DisplayName = "Ending Balance", Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Decimal? EndBalance
			{
				get
				{
					return this._EndBalance;
				}
				set
				{
					this._EndBalance = value;
				}
			}
			#endregion
			#region CuryBalance
			public abstract class curyBalance : PX.Data.IBqlField
			{
			}

			protected Decimal? _CuryBalance;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency Balance", Visible = false)]
			public virtual Decimal? CuryBalance
			{
				get
				{
					return this._CuryBalance;
				}
				set
				{
					this._CuryBalance = value;
				}
			}
			#endregion
			#region Balance
			public abstract class balance : PX.Data.IBqlField
			{
			}

			protected Decimal? _Balance;
			[PXBaseCury()]
			[PXUIField(DisplayName = "Balance", Visible=false)]
			public virtual Decimal? Balance
			{
				get
				{
					return this._Balance;
				}
				set
				{
					this._Balance = value;
				}
			}
			#endregion

			#region CurySales
			public abstract class curySales: PX.Data.IBqlField
			{
			}
			protected Decimal? _CurySales;

			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Sales")]
			public virtual Decimal? CurySales
			{
				get
				{
					return this._CurySales;
				}
				set
				{
					this._CurySales = value;
				}
			}
			#endregion
			#region Sales
			public abstract class sales : PX.Data.IBqlField
			{
			}
			protected Decimal? _Sales;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Sales")]
			public virtual Decimal? Sales
			{
				get
				{
					return this._Sales;
				}
				set
				{
					this._Sales = value;
				}
			}
			#endregion

			#region CuryPayments
			public abstract class curyPayments : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryPayments;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Payments")]
			public virtual Decimal? CuryPayments
			{
				get
				{
					return this._CuryPayments;
				}
				set
				{
					this._CuryPayments = value;
				}
			}
			#endregion
			#region Payments
			public abstract class payments : PX.Data.IBqlField
			{
			}
			protected Decimal? _Payments;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Payments")]
			public virtual Decimal? Payments
			{
				get
				{
					return this._Payments;
				}
				set
				{
					this._Payments = value;
				}
			}
			#endregion
			#region CuryDiscount
			public abstract class curyDiscount : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDiscount;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Cash Discount Taken")]
			public virtual Decimal? CuryDiscount
			{
				get
				{
					return this._CuryDiscount;
				}
				set
				{
					this._CuryDiscount = value;
				}
			}
			#endregion
			#region Discount
			public abstract class discount : PX.Data.IBqlField
			{
			}
			protected Decimal? _Discount;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Cash Discount Taken")]
			public virtual Decimal? Discount
			{
				get
				{
					return this._Discount;
				}
				set
				{
					this._Discount = value;
				}
			}
			#endregion
			#region RGOL
			public abstract class rGOL : PX.Data.IBqlField
			{
			}
			protected Decimal? _RGOL;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Realized Gain/Loss")]
			public virtual Decimal? RGOL
			{
				get
				{
					return this._RGOL;
				}
				set
				{
					this._RGOL = value;
				}
			}
			#endregion
			#region CuryCrAdjustments
			public abstract class curyCrAdjustments : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryCrAdjustments;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Credit Memos")]
			public virtual Decimal? CuryCrAdjustments
			{
				get
				{
					return this._CuryCrAdjustments;
				}
				set
				{
					this._CuryCrAdjustments = value;
				}
			}
			#endregion
			#region CrAdjustments
			public abstract class crAdjustments : PX.Data.IBqlField
			{
			}
			protected Decimal? _CrAdjustments;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Credit Memos")]
			public virtual Decimal? CrAdjustments
			{
				get
				{
					return this._CrAdjustments;
				}
				set
				{
					this._CrAdjustments = value;
				}
			}
			#endregion
			#region CuryDrAdjustments
			public abstract class curyDrAdjustments : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDrAdjustments;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Debit Memos")]
			public virtual Decimal? CuryDrAdjustments
			{
				get
				{
					return this._CuryDrAdjustments;
				}
				set
				{
					this._CuryDrAdjustments = value;
				}
			}
			#endregion
			#region DrAdjustments
			public abstract class drAdjustments : PX.Data.IBqlField
			{
			}
			protected Decimal? _DrAdjustments;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Debit Memos")]
			public virtual Decimal? DrAdjustments
			{
				get
				{
					return this._DrAdjustments;
				}
				set
				{
					this._DrAdjustments = value;
				}
			}
			#endregion
			#region COGS
			public abstract class cOGS : PX.Data.IBqlField
			{
			}
			protected Decimal? _COGS;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "PTD COGS")]
			public virtual Decimal? COGS
			{
				get
				{
					return this._COGS;
				}
				set
				{
					this._COGS = value;
				}
			}
			#endregion
			#region FinPtdRevaluated
			public abstract class finPtdRevaluated : PX.Data.IBqlField
			{
			}
			protected Decimal? _FinPtdRevaluated;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unrealized Gain/Loss")]
			public virtual Decimal? FinPtdRevaluated
			{
				get
				{
					return this._FinPtdRevaluated;
				}
				set
				{
					this._FinPtdRevaluated = value;
				}
			}
			#endregion
			#region CuryFinCharges
			public abstract class curyFinCharges : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryFinCharges;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Currency PTD Overdue Charges", Visible = false)]
			public virtual Decimal? CuryFinCharges
			{
				get
				{
					return this._CuryFinCharges;
				}
				set
				{
					this._CuryFinCharges = value;
				}
			}
			#endregion
			#region FinCharges
			public abstract class finCharges : PX.Data.IBqlField
			{
			}
			protected Decimal? _FinCharges;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "PTD Overdue Charges", Visible = false)]
			public virtual Decimal? FinCharges
			{
				get
				{
					return this._FinCharges;
				}
				set
				{
					this._FinCharges = value;
				}
			}
			#endregion
			#region CuryDeposits
			public abstract class curyDeposits : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDeposits;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Prepayments")]
			public virtual Decimal? CuryDeposits
			{
				get
				{
					return this._CuryDeposits;
				}
				set
				{
					this._CuryDeposits = value;
				}
			}
			#endregion
			#region Deposits
			public abstract class deposits : PX.Data.IBqlField
			{
			}
			protected Decimal? _Deposits;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Prepayments")]
			public virtual Decimal? Deposits
			{
				get
				{
					return this._Deposits;
				}
				set
				{
					this._Deposits = value;
				}
			}
			#endregion
			#region CuryDepositsBalance
			public abstract class curyDepositsBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDepositsBalance;
			[PXDBCury(typeof(ARHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency Prepayments Balance")]
			public virtual Decimal? CuryDepositsBalance
			{
				get
				{
					return this._CuryDepositsBalance;
				}
				set
				{
					this._CuryDepositsBalance = value;
				}
			}
			#endregion
			#region DepositsBalance
			public abstract class depositsBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _DepositsBalance;
			[PXBaseCury()]
			[PXUIField(DisplayName = "Prepayments Balance")]
			public virtual Decimal? DepositsBalance
			{
				get
				{
					return this._DepositsBalance;
				}
				set
				{
					this._DepositsBalance = value;
				}
			}
			#endregion
			#region Converted
			public abstract class converted : PX.Data.IBqlField
			{
			}
			protected bool? _Converted;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Converted to Base Currency", Visible = false, Enabled = false)]
			public virtual bool? Converted
			{
				get
				{
					return this._Converted;
				}
				set
				{
					this._Converted = value;
				}
			}
			#endregion
			#region NoteID
			protected Guid? _NoteID;
			public abstract class noteID : PX.Data.IBqlField
			{
			}
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
			public virtual void RecalculateEndBalance()
			{
				const decimal zero = 0m;
				this.RecalculateBalance();
				this.EndBalance = (this.BegBalance ?? zero) +
							   +(this.Balance ?? zero);
				this.CuryEndBalance = (this.CuryBegBalance ?? zero) +
							   +(this.CuryBalance ?? zero);
			}

			public virtual void RecalculateBalance()
			{
				const decimal zero = 0m;
				this.Balance = (this.Sales ?? zero)
							   - (this.Payments ?? zero)
							   - (this.Discount ?? zero)							   
							   + (this.RGOL ?? zero)
							   - (this.CrAdjustments ?? zero)
							   + (this.FinCharges ?? zero)
							   + (this.DrAdjustments ?? zero);
				this.CuryBalance = (this.CurySales ?? zero)
							   - (this.CuryPayments ?? zero)
							   - (this.CuryDiscount ?? zero)							   
							   - (this.CuryCrAdjustments ?? zero)
							   + (this.CuryFinCharges ?? zero)
							   + (this.CuryDrAdjustments ?? zero);
							   
			}

			public virtual void CopyValueToCuryValue(string aBaseCuryID)
			{
				this.CuryBegBalance = this.BegBalance ?? Decimal.Zero;
				this.CurySales = this.Sales ?? Decimal.Zero;
				this.CuryPayments = this.Payments ?? Decimal.Zero;
				this.CuryDiscount = this.Discount ?? Decimal.Zero;
				this.CuryFinCharges = this.FinCharges?? Decimal.Zero;
				this.CuryCrAdjustments = this.CrAdjustments ?? Decimal.Zero;
				this.CuryDrAdjustments = this.DrAdjustments ?? Decimal.Zero;
				this.CuryDeposits = this.Deposits ?? Decimal.Zero;
				this.CuryDepositsBalance = this.DepositsBalance ?? Decimal.Zero;
				this.CuryEndBalance = this.EndBalance ?? Decimal.Zero;
				this.CuryID = aBaseCuryID;
				this.Converted = true;
			}
		}

		[Serializable]
		[PXProjection(typeof(Select4<CuryARHistory,
			Aggregate<
			GroupBy<CuryARHistory.branchID,
			GroupBy<CuryARHistory.customerID,
			GroupBy<CuryARHistory.accountID,
			GroupBy<CuryARHistory.subID,
			GroupBy<CuryARHistory.curyID,
			Max<CuryARHistory.finPeriodID
			>>>>>>>>))]
		[PXCacheName(Messages.ARLatestHistory)]
		public partial class ARLatestHistory : PX.Data.IBqlTable
		{
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryARHistory.branchID))]
			[PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
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
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryARHistory.customerID))]
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
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryARHistory.accountID))]
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
			[PXDBInt(IsKey = true, BqlField = typeof(CuryARHistory.subID))]
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
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(CuryARHistory.curyID))]
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
			#region LastActivityPeriod
			public abstract class lastActivityPeriod : PX.Data.IBqlField
			{
			}
			protected String _LastActivityPeriod;
			[GL.FinPeriodID(BqlField = typeof(CuryARHistory.finPeriodID))]
			public virtual String LastActivityPeriod
			{
				get
				{
					return this._LastActivityPeriod;
				}
				set
				{
					this._LastActivityPeriod = value;
				}
			}
			#endregion
		}
		[Serializable]
		[PXHidden]
		public sealed class ARH : CuryARHistory
		{
			#region BranchID
			public new abstract class branchID : PX.Data.IBqlField { }
			#endregion
			#region AccountID
			public new abstract class accountID : PX.Data.IBqlField { }
			#endregion
			#region SubID
			public abstract new class subID : PX.Data.IBqlField { }
			#endregion
			#region FinPeriodID
			public new abstract class finPeriodID : PX.Data.IBqlField { }
			#endregion
			#region CustomerID
			public new abstract class customerID : PX.Data.IBqlField { }
			#endregion
			#region CuryID
			public new abstract class curyID : PX.Data.IBqlField { }
			#endregion
			#region FinBegBalance
			public new abstract class finBegBalance : PX.Data.IBqlField { }
			#endregion
			#region FinYtdBalance
			public new abstract class finYtdBalance : PX.Data.IBqlField { }
			#endregion

			#region TranBegBalance
			public new abstract class tranBegBalance : PX.Data.IBqlField { }
			#endregion
			#region TranYtdBalance
			public new abstract class tranYtdBalance : PX.Data.IBqlField { }
			#endregion

			#region CuryFinBegBalance
			public new abstract class curyFinBegBalance : PX.Data.IBqlField { }
			#endregion
			#region CuryFinYtdBalance
			public new abstract class curyFinYtdBalance : PX.Data.IBqlField { }
			#endregion

			#region CuryTranBegBalance
			public new abstract class curyTranBegBalance : PX.Data.IBqlField { }
			#endregion
			#region CuryTranYtdBalance
			public new abstract class curyTranYtdBalance : PX.Data.IBqlField { }
			#endregion

			#region CuryTranPtdDeposits
			public abstract new class curyTranPtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region CuryTranYtdDeposits
			public abstract new class curyTranYtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region TranPtdDeposits
			public abstract new class tranPtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region TranYtdDeposits
			public abstract new class tranYtdDeposits : PX.Data.IBqlField { }
			#endregion

			#region CuryFinPtdDeposits
			public abstract new class curyFinPtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region CuryFinYtdDeposits
			public abstract new class curyFinYtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region FinPtdDeposits
			public abstract new class finPtdDeposits : PX.Data.IBqlField { }
			#endregion
			#region FinYtdDeposits
			public abstract new class finYtdDeposits : PX.Data.IBqlField { }
			#endregion


			#region FinPtdRevaluated
			public abstract class finPtdRevaluated : PX.Data.IBqlField { }
			#endregion

		}

		private sealed class decimalZero : Constant<decimal>
		{
			public decimalZero()
				: base(0m)
			{
			}
		}
		#endregion

		#region Public Membsers
		public PXFilter<ARHistoryFilter> Filter;
		[PXFilterable]
		public PXSelect<ARHistoryResult> History;
		public PXSetup<ARSetup> ARSetup;
		public PXSetup<Company> Company;
		#endregion
		
		#region Ctor + Overrides

		public ARCustomerBalanceEnq()
		{
			ARSetup setup = ARSetup.Current;
			Company company = this.Company.Current;
			this.History.Cache.AllowDelete = false;
			this.History.Cache.AllowInsert = false;
			this.History.Cache.AllowUpdate = false;
			this.reports.MenuAutoOpen = true;
			this.reports.AddMenuAction(this.aRBalanceByCustomerReport);
			this.reports.AddMenuAction(this.customerHistoryReport);
			this.reports.AddMenuAction(this.aRAgedPastDueReport);
			this.reports.AddMenuAction(this.aRAgedOutstandingReport);

		}
		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Actions

		public PXCancel<ARHistoryFilter> Cancel;
		public PXAction<ARHistoryFilter> viewDetails;
		public PXAction<ARHistoryFilter> previousPeriod;
		public PXAction<ARHistoryFilter> nextPeriod;
		public PXAction<ARHistoryFilter> aRBalanceByCustomerReport;
		public PXAction<ARHistoryFilter> customerHistoryReport;
		public PXAction<ARHistoryFilter> aRAgedPastDueReport;
		public PXAction<ARHistoryFilter> aRAgedOutstandingReport;
		public PXAction<ARHistoryFilter> reports;

		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable PreviousPeriod(PXAdapter adapter)
		{
			ARHistoryFilter filter = Filter.Current as ARHistoryFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindPrevPeriod(this, filter.Period, true);
			if (nextperiod != null)
			{
				filter.Period = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable NextPeriod(PXAdapter adapter)
		{
			ARHistoryFilter filter = Filter.Current as ARHistoryFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindNextPeriod(this, filter.Period, true);
			if (nextperiod != null)
			{
				filter.Period = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDetails(PXAdapter adapter)
		{
			if (this.History.Current != null && this.Filter.Current != null)
			{
				ARHistoryResult res = this.History.Current;
				ARHistoryFilter currentFilter = this.Filter.Current;
				ARDocumentEnq graph = PXGraph.CreateInstance<ARDocumentEnq>();

				ARDocumentEnq.ARDocumentFilter filter = graph.Filter.Current;
				Copy(filter, currentFilter);
				filter.CustomerID = res.CustomerID;				
				filter.BalanceSummary = null;
				graph.Filter.Update(filter);
				filter = graph.Filter.Select();
				throw new PXRedirectRequiredException(graph, "Customer Details");

			}
			return Filter.Select();
		}

		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Reports(PXAdapter adapter)
		{
			return adapter.Get();
		}


		[PXUIField(DisplayName = Messages.ARBalanceByCustomerReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ARBalanceByCustomerReport(PXAdapter adapter)
		{
			ARHistoryFilter filter = Filter.Current;
			ARHistoryResult history = History.Current;

			if (filter != null && history != null)
			{
				Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARHistoryResult.customerID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(filter.Period))
				{
					parameters["PeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.Period);
				}
				parameters["CustomerID"] = customer.AcctCD;
				throw new PXReportRequiredException(parameters, "AR632500", PXBaseRedirectException.WindowMode.NewWindow, Messages.ARBalanceByCustomerReport);
			}
			return adapter.Get();
		}


		[PXUIField(DisplayName = Messages.CustomerHistoryReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable CustomerHistoryReport(PXAdapter adapter)
		{
			ARHistoryFilter filter = Filter.Current;
			ARHistoryResult history = History.Current;
			if (filter != null && history != null)
			{
				Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARHistoryResult.customerID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(filter.Period))
				{
					parameters["FromPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.Period);
					parameters["ToPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.Period);
				}
				parameters["CustomerID"] = customer.AcctCD;
				throw new PXReportRequiredException(parameters, "AR652000", PXBaseRedirectException.WindowMode.NewWindow , Messages.CustomerHistoryReport);
			}
			return adapter.Get();
		}


		[PXUIField(DisplayName = Messages.ARAgedPastDueReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ARAgedPastDueReport(PXAdapter adapter)
		{
			ARHistoryResult history = History.Current;
			if (history != null)
			{
				Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARHistoryResult.customerID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["CustomerID"] = customer.AcctCD;
				throw new PXReportRequiredException(parameters, "AR631000" ,PXBaseRedirectException.WindowMode.NewWindow, Messages.ARAgedPastDueReport);
			}
			return adapter.Get();
		}


		[PXUIField(DisplayName = Messages.ARAgedOutstandingReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ARAgedOutstandingReport(PXAdapter adapter)
		{
			ARHistoryResult history = History.Current;
			if (history != null)
			{
				Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARHistoryResult.customerID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["CustomerID"] = customer.AcctCD;
				throw new PXReportRequiredException(parameters, "AR631500", PXBaseRedirectException.WindowMode.NewWindow , Messages.ARAgedOutstandingReport);
			}
			return adapter.Get();
		}
		#endregion
		
		#region Select Overrides	
		
		protected virtual IEnumerable history()
		{
			ARHistoryFilter header = Filter.Current;
			ARHistoryResult[] empty = null;
			if (header == null)
			{
				return empty;
			}
			header.ClearSummary();
			Dictionary<KeyValuePair<int, string>, ARHistoryResult> result;
			if (header.Period == null)
			{
				RetrieveHistory(header, out result);
			}
			else
			{
				RetrieveHistoryForPeriod(header, out result);
			}
			if (header.ShowWithBalanceOnly ?? false)
			{
				RemoveZeroBalances(result);
			}

			bool anyDoc = result.Count > 0;
			this.viewDetails.SetEnabled(anyDoc);
			return result.Values;
		}

		protected virtual IEnumerable filter()
		{
			PXCache cache = this.Caches[typeof(ARHistoryFilter)];
			if (cache != null)
			{
				ARHistoryFilter filter = cache.Current as ARHistoryFilter;
				if (filter != null)
				{					
					filter.ClearSummary();
					bool byPeriod = !string.IsNullOrEmpty(filter.Period);
					foreach (ARHistoryResult it in this.History.Select())
					{
						Aggregate(filter, it); 
					}
				}
			}
			yield return cache.Current;
			cache.IsDirty = false;
		}

		protected virtual void RetrieveHistory(ARHistoryFilter header, out Dictionary<KeyValuePair<int, string>, ARHistoryResult> result) 
		{
			result = new Dictionary<KeyValuePair<int, string>, ARHistoryResult>();
			bool isCurySelected = string.IsNullOrEmpty(header.CuryID) == false;
			bool splitByCurrency = header.SplitByCurrency ?? false;
			bool useFinancial = (header.ByFinancialPeriod == true);
			
			#region FiscalPeriodUndefined
			Type typeCustomer = header.IncludeChildAccounts == true
				? typeof(Customer.consolidatingBAccountID)
				: typeof(Customer.bAccountID);
			Type select = BqlCommand.Compose(
				typeof(Select5<,,>), typeof(ARLatestHistory),
				typeof(InnerJoin<Customer, On<ARLatestHistory.customerID, Equal<Customer.bAccountID>,
						And<Match<Customer, Current<AccessInfo.userName>>>>,
					LeftJoin<Sub, On<ARLatestHistory.subID, Equal<Sub.subID>>,
					LeftJoin<CuryARHistory, On<ARLatestHistory.accountID, Equal<CuryARHistory.accountID>,
						And<ARLatestHistory.branchID, Equal<CuryARHistory.branchID>,
						And<ARLatestHistory.customerID, Equal<CuryARHistory.customerID>,
						And<ARLatestHistory.subID, Equal<CuryARHistory.subID>,
						And<ARLatestHistory.curyID, Equal<CuryARHistory.curyID>,
						And<ARLatestHistory.lastActivityPeriod, Equal<CuryARHistory.finPeriodID>>>>>>>>>>),
				
				typeof(Aggregate<>),
				typeof(Sum<,>), typeof(CuryARHistory.finBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.finYtdBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinYtdBalance),
				typeof(Sum<,>), typeof(CuryARHistory.tranBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.tranYtdBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranYtdBalance),

				typeof(Sum<,>), typeof(CuryARHistory.finPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdRGOL),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdCOGS),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdFinCharges),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdRevalued),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.finYtdDeposits),

				typeof(Sum<,>), typeof(CuryARHistory.tranPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdRGOL),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdCOGS),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdFinCharges),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.tranYtdDeposits),

				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinYtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdFinCharges),

				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranYtdDeposits),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdFinCharges),

				typeof(GroupBy<,>), typeof(ARLatestHistory.lastActivityPeriod),
				typeof(GroupBy<,>), typeof(ARLatestHistory.curyID),
				typeof(GroupBy<>), typeCustomer);

			select = BqlCommand.AppendJoin(select, BqlCommand.Compose(typeof(InnerJoin<,>), 
				typeof(CustomerMaster), typeof(On<,>), typeof(CustomerMaster.bAccountID), typeof(Equal<>), typeCustomer));

			BqlCommand cmd = BqlCommand.CreateInstance(select);
			PXView view = new PXView(this, false, cmd);

			if (header.BranchID != null)
			{
				view.WhereAnd<Where<ARLatestHistory.branchID, Equal<Current<ARHistoryFilter.branchID>>>>();
			}
			if (header.ARAcctID != null)
			{
				view.WhereAnd<Where<ARLatestHistory.accountID, Equal<Current<ARHistoryFilter.aRAcctID>>>>();
			}
			if (header.ARSubID != null)
			{
				view.WhereAnd<Where<ARLatestHistory.subID, Equal<Current<ARHistoryFilter.aRSubID>>>>();
			}
			if (isCurySelected)
			{
				view.WhereAnd<Where<ARLatestHistory.curyID, Equal<Current<ARHistoryFilter.curyID>>>>();
			}

			AppendCommonWhereFilters(view, header);
			
			foreach (PXResult<ARLatestHistory, Customer, Sub, CuryARHistory, CustomerMaster> record in view.SelectMulti())
			{
				CustomerMaster customer = record;
				CuryARHistory history = record;
				ARHistoryResult res = new ARHistoryResult();
				CopyFrom(res, customer);
				CopyFrom(res, history, useFinancial);
				res.FinPeriodID = history.FinPeriodID;
				string keyCuryID = (isCurySelected || splitByCurrency) ? history.CuryID : this.Company.Current.BaseCuryID;
				if (!isCurySelected && !splitByCurrency)
				{
					res.CopyValueToCuryValue(this.Company.Current.BaseCuryID);
					res.RecalculateEndBalance();
				}
				KeyValuePair<int, string> key = new KeyValuePair<int, string>(customer.BAccountID.Value, keyCuryID);
				if (result.ContainsKey(key))
				{
					AggregateLatest(result[key], res);
				}
				else
				{
					result[key] = res;
				}
			}
			#endregion
		}
		protected virtual void RetrieveHistoryForPeriod(ARHistoryFilter header, out Dictionary<KeyValuePair<int, string>, ARHistoryResult> result) 
		{
			result = new Dictionary<KeyValuePair<int, string>, ARHistoryResult>();
			bool isCurySelected = string.IsNullOrEmpty(header.CuryID) == false;
			bool splitByCurrency = header.SplitByCurrency ?? false;			
			bool useFinancial = (header.ByFinancialPeriod == true);
			
			#region Specific Fiscal Period
			Type typeCustomer = header.IncludeChildAccounts == true
				? typeof(Customer.consolidatingBAccountID)
				: typeof(Customer.bAccountID);
			Type select = BqlCommand.Compose(
				typeof(Select5<,,,>), typeof(ARHistoryByPeriod),
				typeof(InnerJoin<Customer, On<ARHistoryByPeriod.customerID, Equal<Customer.bAccountID>,
						And<Match<Customer, Current<AccessInfo.userName>>>>,
					LeftJoin<Sub, On<ARHistoryByPeriod.subID, Equal<Sub.subID>>,
					LeftJoin<CuryARHistory, On<ARHistoryByPeriod.accountID, Equal<CuryARHistory.accountID>,
						And<ARHistoryByPeriod.branchID, Equal<CuryARHistory.branchID>,
						And<ARHistoryByPeriod.customerID, Equal<CuryARHistory.customerID>,
						And<ARHistoryByPeriod.subID, Equal<CuryARHistory.subID>,
						And<ARHistoryByPeriod.curyID, Equal<CuryARHistory.curyID>,
						And<ARHistoryByPeriod.finPeriodID, Equal<CuryARHistory.finPeriodID>>>>>>>,
					LeftJoin<ARH, On<ARHistoryByPeriod.accountID, Equal<ARH.accountID>,
						And<ARHistoryByPeriod.branchID, Equal<ARH.branchID>,
						And<ARHistoryByPeriod.customerID, Equal<ARH.customerID>,
						And<ARHistoryByPeriod.subID, Equal<ARH.subID>,
						And<ARHistoryByPeriod.curyID, Equal<ARH.curyID>,
						And<ARHistoryByPeriod.lastActivityPeriod, Equal<ARH.finPeriodID>>>>>>>>>>>),
				
				typeof(Where<ARHistoryByPeriod.finPeriodID, Equal<Current<ARHistoryFilter.period>>>),
			
				typeof(Aggregate<>),
				typeof(Sum<,>), typeof(CuryARHistory.finBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.finYtdBalance),
				typeof(Sum<,>), typeof(CuryARHistory.tranBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranBegBalance),
				typeof(Sum<,>), typeof(CuryARHistory.tranYtdBalance),

				typeof(Sum<,>), typeof(CuryARHistory.finPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdRGOL),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdCOGS),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdFinCharges),
				typeof(Sum<,>), typeof(CuryARHistory.finPtdRevalued),

				typeof(Sum<,>), typeof(CuryARHistory.tranPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdRGOL),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdCOGS),
				typeof(Sum<,>), typeof(CuryARHistory.tranPtdFinCharges),

				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.curyFinPtdFinCharges),

				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdCrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdDiscounts),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdDrAdjustments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdPayments),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdSales),
				typeof(Sum<,>), typeof(CuryARHistory.curyTranPtdFinCharges),

				typeof(Sum<,>), typeof(ARH.curyFinYtdBalance),
				typeof(Sum<,>), typeof(ARH.finYtdBalance),
				typeof(Sum<,>), typeof(ARH.curyTranYtdBalance),
				typeof(Sum<,>), typeof(ARH.tranYtdBalance),
				typeof(Sum<,>), typeof(ARH.curyFinYtdBalance),
				typeof(Sum<,>), typeof(ARH.finBegBalance),
				typeof(Sum<,>), typeof(ARH.curyFinBegBalance),
				typeof(Sum<,>), typeof(ARH.tranBegBalance),
				typeof(Sum<,>), typeof(ARH.curyTranBegBalance),
				typeof(GroupBy<,>), typeof(ARHistoryByPeriod.lastActivityPeriod),
				typeof(GroupBy<,>), typeof(ARHistoryByPeriod.finPeriodID),
				typeof(GroupBy<,>), typeof(ARHistoryByPeriod.curyID),
				typeof(GroupBy<>), typeCustomer);

			select = BqlCommand.AppendJoin(select, BqlCommand.Compose(typeof(InnerJoin<,>),
				typeof(CustomerMaster), typeof(On<,>), typeof(CustomerMaster.bAccountID), typeof(Equal<>), typeCustomer));

			BqlCommand cmd = BqlCommand.CreateInstance(select);
			PXView view = new PXView(this, false, cmd);

			if (isCurySelected)
			{
				view.WhereAnd<Where<ARHistoryByPeriod.curyID, Equal<Current<ARHistoryFilter.curyID>>>>();
			}
			if (header.BranchID != null)
			{
				view.WhereAnd<Where<ARHistoryByPeriod.branchID, Equal<Current<ARHistoryFilter.branchID>>>>();
			}
			if (header.ARAcctID != null)
			{
				view.WhereAnd<Where<ARHistoryByPeriod.accountID, Equal<Current<ARHistoryFilter.aRAcctID>>>>();
			}
			if (header.ARSubID != null)
			{
				view.WhereAnd<Where<ARHistoryByPeriod.subID, Equal<Current<ARHistoryFilter.aRSubID>>>>();
			}

			AppendCommonWhereFilters(view, header);
			
			foreach (PXResult<ARHistoryByPeriod, Customer, Sub, CuryARHistory, ARH, CustomerMaster> record in view.SelectMulti())
			{
				CustomerMaster customer = record;
				CuryARHistory history = record;
				ARH lastActivity = record;
				ARHistoryByPeriod hstByPeriod = record;
				ARHistoryResult res = new ARHistoryResult();
				CopyFrom(res, customer);
				CopyFrom(res, history, useFinancial);

				res.FinPeriodID = lastActivity.FinPeriodID;
				if (string.IsNullOrEmpty(res.CuryID))
				{
					res.CuryID = hstByPeriod.CuryID;
				}
				string keyCuryID = (isCurySelected || splitByCurrency) ? hstByPeriod.CuryID : this.Company.Current.BaseCuryID;
				KeyValuePair<int, string> key = new KeyValuePair<int, string>(customer.BAccountID.Value, keyCuryID);

				if ((history.FinPeriodID == null) || (history.FinPeriodID != lastActivity.FinPeriodID))
				{
					if (useFinancial)
					{
						res.EndBalance = res.BegBalance = lastActivity.FinYtdBalance ?? Decimal.Zero;
						res.CuryEndBalance = res.CuryBegBalance = lastActivity.CuryFinYtdBalance ?? Decimal.Zero;
						res.DepositsBalance = -lastActivity.FinYtdDeposits ?? Decimal.Zero;
						res.CuryDepositsBalance = -lastActivity.CuryFinYtdDeposits ?? Decimal.Zero;
					}
					else
					{
						res.EndBalance = res.BegBalance = lastActivity.TranYtdBalance ?? Decimal.Zero;
						res.CuryEndBalance = res.CuryBegBalance = lastActivity.CuryTranYtdBalance ?? Decimal.Zero;
						res.CuryDepositsBalance = -lastActivity.CuryTranYtdDeposits ?? Decimal.Zero;
						res.DepositsBalance = -lastActivity.TranYtdDeposits ?? Decimal.Zero;
					}
				}
				if ((!isCurySelected) && splitByCurrency == false)
				{
					res.CopyValueToCuryValue(this.Company.Current.BaseCuryID);
				}
				if (result.ContainsKey(key))
				{
					var resultRowToAgregate = result[key];

					if (string.CompareOrdinal(resultRowToAgregate.FinPeriodID, res.FinPeriodID) < 0)
					{
						resultRowToAgregate.FinPeriodID = res.FinPeriodID;
					}
					
					Aggregate(resultRowToAgregate, res);
				}
				else
				{
					result[key] = res;
				}
			}
			#endregion
		}

		protected virtual void AppendCommonWhereFilters(PXView view, ARHistoryFilter filter)
		{
			if (!SubCDUtils.IsSubCDEmpty(filter.SubCD))
			{
				view.WhereAnd<Where<Sub.subCD, Like<Current<ARHistoryFilter.subCDWildcard>>>>();
			}
			if (filter.CustomerClassID != null)
			{
				view.WhereAnd<Where<Customer.customerClassID, Equal<Current<ARHistoryFilter.customerClassID>>>>();
			}
			if (filter.CustomerID != null)
			{
				view.WhereAnd<Where<Customer.bAccountID, Equal<Current<ARHistoryFilter.customerID>>,
					Or<Customer.consolidatingBAccountID, Equal<Current<ARHistoryFilter.customerID>>,
						And<Current<ARHistoryFilter.includeChildAccounts>, Equal<True>>>>>();
			}
			if (filter.Status != null)
			{
				view.WhereAnd<Where<Customer.status, Equal<Current<ARHistoryFilter.status>>>>();
			}
		}
		#endregion

		#region Event Subscribers

		public virtual void ARHistoryFilter_CuryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			cache.SetDefaultExt<ARHistoryFilter.splitByCurrency>(e.Row);
		}


		public virtual void ARHistoryFilter_SubCD_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		public virtual void ARHistoryFilter_ARAcctID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			ARHistoryFilter header = e.Row as ARHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.ARAcctID = null;
			}
		}
		public virtual void ARHistoryFilter_ARSubID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			ARHistoryFilter header = e.Row as ARHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.ARSubID = null;
			}
		}
		public virtual void ARHistoryFilter_CuryID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			ARHistoryFilter header = e.Row as ARHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.CuryID = null;
			}
		}
		public virtual void ARHistoryFilter_Period_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			ARHistoryFilter header = e.Row as ARHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.Period = null;
			}
		}
		public virtual void ARHistoryFilter_CustomerClassID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			ARHistoryFilter header = e.Row as ARHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.CustomerClassID = null;
			}
		}
		protected virtual void ARHistoryFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARHistoryFilter row = e.Row as ARHistoryFilter;
			if (row == null) return;		

			Company company = this.Company.Current;
			bool mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
			PXUIFieldAttribute.SetVisible<ARHistoryFilter.showWithBalanceOnly>(sender, row, true);
			PXUIFieldAttribute.SetVisible<ARHistoryFilter.byFinancialPeriod>(sender, row, true);
			PXUIFieldAttribute.SetVisible<ARHistoryFilter.curyID>(sender, row, mcFeatureInstalled);

			PXUIFieldAttribute.SetVisible<ARHistoryFilter.includeChildAccounts>(sender, row, PXAccess.FeatureInstalled<CS.FeaturesSet.parentChildAccount>());

			bool isCurySelected = string.IsNullOrEmpty(row.CuryID) == false;
			bool isForeignCurrency = string.IsNullOrEmpty(row.CuryID) == false && (company.BaseCuryID != row.CuryID);
			bool isBaseCurySelected = string.IsNullOrEmpty(row.CuryID) == false && (company.BaseCuryID == row.CuryID);
			bool splitByCurrency = (row.SplitByCurrency ?? false);

			PXUIFieldAttribute.SetVisible<ARHistoryFilter.splitByCurrency>(sender, row, mcFeatureInstalled && !isCurySelected);
			PXUIFieldAttribute.SetEnabled<ARHistoryFilter.splitByCurrency>(sender, row, mcFeatureInstalled && !isCurySelected);
			PXUIFieldAttribute.SetVisible<ARHistoryFilter.curyBalanceSummary>(sender, row, isForeignCurrency);
			PXUIFieldAttribute.SetVisible<ARHistoryFilter.curyDepositsSummary>(sender, row, isForeignCurrency);

			PXCache detailCache = this.History.Cache;
			bool hideCuryColumns = (!mcFeatureInstalled) || (isBaseCurySelected) || (!isCurySelected && !splitByCurrency);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyID>(this.History.Cache, null, mcFeatureInstalled && (isCurySelected || splitByCurrency));
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyBalance>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyPayments>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curySales>(detailCache, null, !hideCuryColumns);			
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyDiscount>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyCrAdjustments>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyDrAdjustments>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyDeposits>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyDepositsBalance>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyBegBalance>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyEndBalance>(detailCache, null, !hideCuryColumns);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.rGOL>(History.Cache, null, mcFeatureInstalled);

			PXUIFieldAttribute.SetVisible<ARHistoryResult.balance>(detailCache, null, false);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.curyBalance>(detailCache, null, false);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.finPeriodID>(detailCache, null);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.begBalance>(detailCache, null);
			PXUIFieldAttribute.SetVisible<ARHistoryResult.endBalance>(detailCache, null);	
		}
	
		#endregion

		#region Utility Functions
		
		protected virtual string GetLastActivityPeriod(int? aCustomerID)
		{
			PXSelectBase<CuryARHistory> activitySelect = new PXSelect<CuryARHistory, Where<CuryARHistory.customerID, Equal<Required<CuryARHistory.customerID>>>, OrderBy<Desc<CuryARHistory.finPeriodID>>>(this);
			CuryARHistory result = (CuryARHistory)activitySelect.View.SelectSingle(aCustomerID);
			if (result != null)
				return result.FinPeriodID;
			return null;
		}

		protected virtual void CopyFrom(ARHistoryResult aDest, Customer aCustomer)
		{
			aDest.AcctCD = aCustomer.AcctCD;
			aDest.AcctName = aCustomer.AcctName;
			aDest.CuryID = aCustomer.CuryID;
			aDest.CustomerID = aCustomer.BAccountID;
			aDest.NoteID = aCustomer.NoteID;
		}

		protected virtual void CopyFrom(ARHistoryResult aDest, CuryARHistory aHistory, bool aIsFinancial)
		{
			if (aIsFinancial)
			{
				aDest.CuryBegBalance = aHistory.CuryFinBegBalance ?? Decimal.Zero;
				aDest.CurySales = aHistory.CuryFinPtdSales ?? Decimal.Zero;
				aDest.CuryPayments = aHistory.CuryFinPtdPayments ?? Decimal.Zero;
				aDest.CuryDiscount = aHistory.CuryFinPtdDiscounts ?? Decimal.Zero;
				aDest.CuryCrAdjustments = aHistory.CuryFinPtdCrAdjustments ?? Decimal.Zero;
				aDest.CuryDrAdjustments = aHistory.CuryFinPtdDrAdjustments ?? Decimal.Zero;
				aDest.CuryDeposits = aHistory.CuryFinPtdDeposits ?? Decimal.Zero;
				aDest.CuryDepositsBalance = -aHistory.CuryFinYtdDeposits ?? Decimal.Zero;
				aDest.CuryFinCharges = aHistory.CuryFinPtdFinCharges ??  Decimal.Zero;
				
				aDest.BegBalance = aHistory.FinBegBalance ?? Decimal.Zero;
				aDest.Sales = aHistory.FinPtdSales?? Decimal.Zero;
				aDest.Payments = aHistory.FinPtdPayments ?? Decimal.Zero;
				aDest.Discount = aHistory.FinPtdDiscounts ?? Decimal.Zero;
				aDest.RGOL = -aHistory.FinPtdRGOL ?? Decimal.Zero;
				aDest.CrAdjustments = aHistory.FinPtdCrAdjustments ?? Decimal.Zero;
				aDest.DrAdjustments = aHistory.FinPtdDrAdjustments ?? Decimal.Zero;
				aDest.Deposits = aHistory.FinPtdDeposits ?? Decimal.Zero;
				aDest.DepositsBalance = -aHistory.FinYtdDeposits ?? Decimal.Zero;
				aDest.FinCharges = aHistory.FinPtdFinCharges ??  Decimal.Zero;
				aDest.COGS = aHistory.FinPtdCOGS?? Decimal.Zero;
				aDest.FinPtdRevaluated = aHistory.FinPtdRevalued ?? Decimal.Zero;
				aDest.CuryID = aHistory.CuryID;
			}
			else
			{
				aDest.CuryBegBalance = aHistory.CuryTranBegBalance ?? Decimal.Zero;
				aDest.CurySales = aHistory.CuryTranPtdSales ?? Decimal.Zero;
				aDest.CuryPayments = aHistory.CuryTranPtdPayments ?? Decimal.Zero;
				aDest.CuryDiscount = aHistory.CuryTranPtdDiscounts ?? Decimal.Zero;
				aDest.CuryCrAdjustments = aHistory.CuryTranPtdCrAdjustments ?? Decimal.Zero;
				aDest.CuryDrAdjustments = aHistory.CuryTranPtdDrAdjustments ?? Decimal.Zero;
				aDest.CuryDeposits = aHistory.CuryTranPtdDeposits ?? Decimal.Zero;
				aDest.CuryDepositsBalance = -aHistory.CuryTranYtdDeposits ?? Decimal.Zero;
				aDest.CuryFinCharges = aHistory.CuryTranPtdFinCharges ?? Decimal.Zero;

				aDest.BegBalance = aHistory.TranBegBalance ?? Decimal.Zero;
				aDest.Sales = aHistory.TranPtdSales ?? Decimal.Zero;
				aDest.Payments = aHistory.TranPtdPayments ?? Decimal.Zero;
				aDest.Discount = aHistory.TranPtdDiscounts ?? Decimal.Zero;
				aDest.RGOL = -aHistory.TranPtdRGOL ?? Decimal.Zero;
				aDest.CrAdjustments = aHistory.TranPtdCrAdjustments ?? Decimal.Zero;
				aDest.DrAdjustments = aHistory.TranPtdDrAdjustments ?? Decimal.Zero;
				aDest.Deposits = aHistory.TranPtdDeposits ?? Decimal.Zero;
				aDest.DepositsBalance = -aHistory.TranYtdDeposits ?? Decimal.Zero;
				aDest.FinCharges = aHistory.TranPtdFinCharges ?? Decimal.Zero;
				aDest.COGS = aHistory.TranPtdCOGS ?? Decimal.Zero;
				aDest.FinPtdRevaluated = aHistory.FinPtdRevalued ?? Decimal.Zero;
			
				aDest.CuryID = aHistory.CuryID;
			}
			aDest.RecalculateEndBalance();
		}
		protected virtual void CopyFrom(ARHistoryResult aDest, CuryARHistory aHistory, bool aUseCurrency, bool aIsFinancial)
		{
			if (aIsFinancial)
			{
				if (aUseCurrency)
				{
					aDest.Sales = aHistory.CuryFinPtdSales ?? 0m;
					aDest.Payments = aHistory.CuryFinPtdPayments ?? 0m;
					aDest.Discount = aHistory.CuryFinPtdDiscounts ?? 0m;
					aDest.RGOL = 0m;
					aDest.CrAdjustments = aHistory.CuryFinPtdCrAdjustments ?? 0m;
					aDest.DrAdjustments = aHistory.CuryFinPtdDrAdjustments ?? 0m;
					aDest.BegBalance = aHistory.CuryFinBegBalance ?? 0m;
					aDest.CuryID = aHistory.CuryID;					
					aDest.FinPtdRevaluated = Decimal.Zero;
					aDest.Deposits = aHistory.CuryFinPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.CuryFinYtdDeposits ?? Decimal.Zero;					
				}
				else
				{
					aDest.Sales = aHistory.FinPtdSales ?? 0m;
					aDest.Payments = aHistory.FinPtdPayments ?? 0m;
					aDest.Discount = aHistory.FinPtdDiscounts ?? 0m;
					aDest.RGOL = -aHistory.FinPtdRGOL ?? 0m;
					aDest.CrAdjustments = aHistory.FinPtdCrAdjustments ?? 0m;
					aDest.DrAdjustments = aHistory.FinPtdDrAdjustments ?? 0m;
					aDest.BegBalance = aHistory.FinBegBalance ?? 0m;
					aDest.COGS = aHistory.FinPtdCOGS?? 0m;
					aDest.FinCharges = aHistory.FinPtdFinCharges ?? 0m;
					aDest.FinPtdRevaluated = aHistory.FinPtdRevalued ?? Decimal.Zero;
					aDest.Deposits = aHistory.FinPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.FinYtdDeposits ?? Decimal.Zero;					
				}
			}
			else
			{
				if (aUseCurrency)
				{
					aDest.Sales = aHistory.CuryTranPtdSales ?? 0m;
					aDest.Payments = aHistory.CuryTranPtdPayments ?? 0m;
					aDest.Discount = aHistory.CuryTranPtdDiscounts ?? 0m;
					aDest.RGOL = 0m;
					aDest.CrAdjustments = aHistory.CuryTranPtdCrAdjustments ?? 0m;
					aDest.DrAdjustments = aHistory.CuryTranPtdDrAdjustments ?? 0m;
					aDest.BegBalance = aHistory.CuryTranBegBalance ?? 0m;
					aDest.CuryID = aHistory.CuryID;
					aDest.FinPtdRevaluated = Decimal.Zero;
					aDest.Deposits = aHistory.CuryTranPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.CuryTranYtdDeposits ?? Decimal.Zero;
					
				}
				else
				{
					aDest.Sales = aHistory.TranPtdSales ?? 0m;
					aDest.Payments = aHistory.TranPtdPayments ?? 0m;
					aDest.Discount = aHistory.TranPtdDiscounts ?? 0m;
					aDest.RGOL = -aHistory.TranPtdRGOL ?? 0m;
					aDest.CrAdjustments = aHistory.TranPtdCrAdjustments ?? 0m;
					aDest.DrAdjustments = aHistory.TranPtdDrAdjustments ?? 0m;
					aDest.BegBalance = aHistory.TranBegBalance ?? 0m;
					aDest.COGS = aHistory.TranPtdCOGS ?? 0m;
					aDest.FinCharges = aHistory.TranPtdFinCharges ?? 0m;
					aDest.FinPtdRevaluated = aHistory.FinPtdRevalued ?? Decimal.Zero;
					aDest.Deposits = aHistory.TranPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.TranYtdDeposits ?? Decimal.Zero;
				}
			}
			aDest.RecalculateEndBalance();
		}
		protected virtual void Aggregate(ARHistoryResult aDest, ARHistoryResult aSrc)
		{
			aDest.CuryBegBalance += aSrc.CuryBegBalance ?? Decimal.Zero;
			aDest.CuryCrAdjustments += aSrc.CuryCrAdjustments ?? Decimal.Zero;
			aDest.CuryDrAdjustments += aSrc.CuryDrAdjustments ?? Decimal.Zero;
			aDest.CuryDiscount += aSrc.CuryDiscount ?? Decimal.Zero;
			aDest.CurySales += aSrc.CurySales ?? Decimal.Zero;
			aDest.CuryPayments += aSrc.CuryPayments ?? Decimal.Zero;
			aDest.CuryFinCharges += aSrc.CuryFinCharges ?? Decimal.Zero;			
			aDest.CuryDeposits += aSrc.CuryDeposits ?? Decimal.Zero;
			aDest.CuryDepositsBalance += aSrc.CuryDepositsBalance ?? Decimal.Zero;

			aDest.BegBalance += aSrc.BegBalance ?? Decimal.Zero;
			aDest.CrAdjustments += aSrc.CrAdjustments ?? Decimal.Zero;
			aDest.DrAdjustments += aSrc.DrAdjustments ?? Decimal.Zero;
			aDest.Discount += aSrc.Discount ?? Decimal.Zero;
			aDest.Sales+= aSrc.Sales ?? Decimal.Zero;
			aDest.Payments += aSrc.Payments ?? Decimal.Zero;
			aDest.FinCharges += aSrc.FinCharges ?? Decimal.Zero;
			aDest.RGOL += aSrc.RGOL ?? Decimal.Zero;
			aDest.FinPtdRevaluated += aSrc.FinPtdRevaluated ?? Decimal.Zero;
			aDest.Deposits += aSrc.Deposits ?? Decimal.Zero;
			aDest.DepositsBalance += aSrc.DepositsBalance ?? Decimal.Zero;
			
			aDest.RecalculateEndBalance();
		}
		protected virtual void Aggregate(ARHistoryFilter aDest, ARHistoryResult aSrc)
		{
			aDest.CuryBalanceSummary += aSrc.CuryEndBalance ?? Decimal.Zero;
			aDest.BalanceSummary += aSrc.EndBalance ?? Decimal.Zero;

			aDest.RevaluedSummary += aSrc.FinPtdRevaluated ?? Decimal.Zero;

			aDest.CuryDepositsSummary += aSrc.CuryDepositsBalance ?? Decimal.Zero;
			aDest.DepositsSummary += aSrc.DepositsBalance ?? Decimal.Zero;
		}
		protected virtual void AggregateLatest(ARHistoryResult aDest, ARHistoryResult aSrc)
		{
			if (aSrc.FinPeriodID == aDest.FinPeriodID)
			{
				Aggregate(aDest, aSrc);
			}
			else
			{
				if (string.Compare(aSrc.FinPeriodID, aDest.FinPeriodID) < 0)
				{
					//Just update Beg Balance
					aDest.BegBalance += aSrc.EndBalance ?? Decimal.Zero;
					aDest.DepositsBalance += aSrc.DepositsBalance ?? Decimal.Zero;
					aDest.CuryBegBalance += aSrc.CuryEndBalance ?? Decimal.Zero;
					aDest.CuryDepositsBalance += aSrc.CuryDepositsBalance ?? Decimal.Zero;
				}
				else
				{
					//Invert 
					aDest.BegBalance = (aDest.EndBalance ?? Decimal.Zero) + (aSrc.BegBalance ?? Decimal.Zero);
					aDest.CrAdjustments = aSrc.CrAdjustments ?? Decimal.Zero;
					aDest.DrAdjustments = aSrc.DrAdjustments ?? Decimal.Zero;
					aDest.Discount = aSrc.Discount ?? Decimal.Zero;
					aDest.Sales = aSrc.Sales ?? Decimal.Zero;
					aDest.Payments = aSrc.Payments ?? Decimal.Zero;
					aDest.RGOL = aSrc.RGOL ?? Decimal.Zero;
					aDest.FinPeriodID = aSrc.FinPeriodID;
					aDest.FinPtdRevaluated = aSrc.FinPtdRevaluated ?? Decimal.Zero;
					aDest.FinCharges = aSrc.FinCharges ?? Decimal.Zero;
					aDest.Deposits = aSrc.Deposits ?? Decimal.Zero;
					aDest.DepositsBalance = (aDest.DepositsBalance ?? Decimal.Zero) + (aSrc.DepositsBalance ?? Decimal.Zero);

					aDest.CuryBegBalance = (aDest.CuryEndBalance ?? Decimal.Zero) + (aSrc.CuryBegBalance ?? Decimal.Zero);
					aDest.CuryCrAdjustments = aSrc.CuryCrAdjustments ?? Decimal.Zero;
					aDest.CuryDrAdjustments = aSrc.CuryDrAdjustments ?? Decimal.Zero;
					aDest.CuryDiscount = aSrc.CuryDiscount ?? Decimal.Zero;
					aDest.CurySales = aSrc.CurySales ?? Decimal.Zero;
					aDest.CuryPayments = aSrc.CuryPayments ?? Decimal.Zero;
					aDest.CuryFinCharges = aSrc.CuryFinCharges ??Decimal.Zero;
					aDest.CuryDeposits = aSrc.CuryDeposits ?? Decimal.Zero;
					aDest.CuryDepositsBalance = (aDest.CuryDepositsBalance ?? Decimal.Zero) + (aSrc.CuryDepositsBalance ?? Decimal.Zero);					
				}
				aDest.RecalculateEndBalance();
			}
		}
		protected virtual void RemoveZeroBalances(Dictionary<KeyValuePair<int,string>, ARHistoryResult> aResult)
		{
			List<KeyValuePair<int,string>> toRemove = new List<KeyValuePair<int,string>>();
			foreach (KeyValuePair<KeyValuePair<int,string>, ARHistoryResult> iRes in aResult)
			{
				if ((iRes.Value.EndBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.FinPtdRevaluated ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.DepositsBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.CuryEndBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.CuryDepositsBalance ?? Decimal.Zero) == Decimal.Zero)
				{
					toRemove.Add(iRes.Key);
				}
			}
			foreach (KeyValuePair<int,string> iKey in toRemove)
			{
				aResult.Remove(iKey);
			}
		}
		
		public static void Copy(ARDocumentEnq.ARDocumentFilter filter, ARHistoryFilter histFilter)
		{
			filter.BranchID = histFilter.BranchID;
			filter.Period = histFilter.Period;
			filter.SubCD = histFilter.SubCD;
			filter.ARAcctID = histFilter.ARAcctID;
			filter.ARSubID = histFilter.ARSubID;
			filter.CuryID = histFilter.CuryID;
			filter.ByFinancialPeriod = histFilter.ByFinancialPeriod;
			filter.IncludeChildAccounts = histFilter.IncludeChildAccounts;
		}
		public static void Copy(ARHistoryFilter histFilter, ARDocumentEnq.ARDocumentFilter filter)
		{
			histFilter.BranchID = filter.BranchID;
			histFilter.CustomerID = filter.CustomerID;
			histFilter.Period = filter.Period;
			histFilter.SubCD = filter.SubCD;
			histFilter.ARAcctID = filter.ARAcctID;
			histFilter.ARSubID = filter.ARSubID;
			histFilter.CuryID = filter.CuryID;
			histFilter.ByFinancialPeriod = filter.ByFinancialPeriod;
			histFilter.IncludeChildAccounts = filter.IncludeChildAccounts;
		}

		#endregion
	}
}
