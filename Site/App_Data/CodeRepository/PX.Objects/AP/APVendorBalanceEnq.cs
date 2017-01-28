using System;
using System.Collections.Generic;
using System.Collections;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CR;

namespace PX.Objects.AP
{
	[Serializable]
	[PXProjection(typeof(Select5<CuryAPHistory,
		InnerJoin<GL.FinPeriod,
			On<GL.FinPeriod.finPeriodID, GreaterEqual<CuryAPHistory.finPeriodID>>>,
		Aggregate<
		GroupBy<CuryAPHistory.branchID,		
		GroupBy<CuryAPHistory.vendorID,
		GroupBy<CuryAPHistory.accountID,
		GroupBy<CuryAPHistory.subID,
		GroupBy<CuryAPHistory.curyID,
		Max<CuryAPHistory.finPeriodID,
		GroupBy<GL.FinPeriod.finPeriodID>>>>>>>>>))]
	[PXPrimaryGraph(
		new Type[] {
			typeof(APDocumentEnq),
			typeof(APVendorBalanceEnq)
		},
		new Type[] {
			typeof(Where<APHistoryByPeriod.vendorID, IsNotNull>),
			typeof(Where<APHistoryByPeriod.vendorID, IsNull>)
		},
		Filters = new Type[] {
			typeof(APDocumentEnq.APDocumentFilter),
			typeof(APVendorBalanceEnq.APHistoryFilter)
		})]
	[PXCacheName(Messages.APHistoryByPeriod)]
	public partial class APHistoryByPeriod : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt(IsKey = true, BqlField = typeof(CuryAPHistory.branchID))]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(IsKey = true, BqlField = typeof(CuryAPHistory.vendorID))]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(IsKey = true, BqlField = typeof(CuryAPHistory.accountID))]
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
		[SubAccount(IsKey = true, BqlField = typeof(CuryAPHistory.subID))]
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
		[PXDBString(5, IsUnicode = true, IsKey=true, InputMask = ">LLLLL", BqlField = typeof(CuryAPHistory.curyID))]
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
		[GL.FinPeriodID(BqlField = typeof(CuryAPHistory.finPeriodID))]
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
	[PXCacheName(Messages.BaseAPHistoryByPeriod)]
	[PXProjection(typeof(Select5<APHistory,
		InnerJoin<GL.FinPeriod,
			On<GL.FinPeriod.finPeriodID, GreaterEqual<APHistory.finPeriodID>>>,
		Aggregate<
        GroupBy<APHistory.branchID,
		GroupBy<APHistory.vendorID,
		GroupBy<APHistory.accountID,
		GroupBy<APHistory.subID,
		Max<APHistory.finPeriodID,
		GroupBy<GL.FinPeriod.finPeriodID>>>>>>>>))]
	[PXPrimaryGraph(
		new Type[] {
			typeof(APDocumentEnq),
			typeof(APVendorBalanceEnq)
		},
		new Type[] {
			typeof(Where<BaseAPHistoryByPeriod.vendorID, IsNotNull>),
			typeof(Where<BaseAPHistoryByPeriod.vendorID, IsNull>)
		},
		Filters = new Type[] {
			typeof(APDocumentEnq.APDocumentFilter),
			typeof(APVendorBalanceEnq.APHistoryFilter)
		})]
	public partial class BaseAPHistoryByPeriod : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt(IsKey = true, BqlField = typeof(APHistory.branchID))]
		[PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
        [PXUIField(DisplayName = "Branch")]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(IsKey = true, BqlField = typeof(APHistory.vendorID), CacheGlobal = true)]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(IsKey = true, BqlField = typeof(APHistory.accountID))]
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
		[SubAccount(IsKey = true, BqlField = typeof(APHistory.subID))]
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
		[GL.FinPeriodID(BqlField = typeof(APHistory.finPeriodID))]
        [PXUIField(DisplayName = "Last Activity Period")]
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

	}

	[PX.Objects.GL.TableAndChartDashboardType]
	public class APVendorBalanceEnq : PXGraph<APVendorBalanceEnq>
	{
		#region Internal Types
		[Serializable]
		public partial class APHistoryFilter : IBqlTable
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
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[GL.Account(null, typeof(Search5<Account.accountID,
						InnerJoin<APHistory, On<Account.accountID, Equal<APHistory.accountID>>>,
						Where<Match<Current<AccessInfo.userName>>>,
					   Aggregate<GroupBy<Account.accountID>>>),
				DisplayName = "AP Account", DescriptionField = typeof(GL.Account.description))]
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
			protected String _SubID;
			[PXDBString(30, IsUnicode = true)]
			[PXUIField(DisplayName = "AP Subaccount", Visibility = PXUIVisibility.Invisible, FieldClass = SubAccountAttribute.DimensionName)]
			[PXDimension("SUBACCOUNT", ValidComboRequired = false)]
			public virtual String SubID
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
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
			[PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
			[PXUIField(DisplayName = "Currency")]
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
			#region CashAcctID
			public abstract class cashAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _CashAcctID;
			[GL.CashAccount()]
			public virtual Int32? CashAcctID
			{
				get
				{
					return this._CashAcctID;
				}
				set
				{
					this._CashAcctID = value;
				}
			}
			#endregion
			#region PaymentMethodID
			public abstract class paymentMethodID : PX.Data.IBqlField
			{
			}
			protected String _PaymentMethodID;
			[PXDBString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Payment Method")]
            [PXSelector(typeof(Search<CA.PaymentMethod.paymentMethodID, Where<CA.PaymentMethod.useForAP, Equal<True>>>), DescriptionField = typeof(CA.PaymentMethod.descr))]			
			public virtual String PaymentMethodID
			{
				get
				{
					return this._PaymentMethodID;
				}
				set
				{
					this._PaymentMethodID = value;
				}
			}
			#endregion
			#region VendorClassID
			public abstract class vendorClassID : PX.Data.IBqlField
			{
			}
			protected String _VendorClassID;
			[PXDBString(10, IsUnicode = true)]
			[PXSelector(typeof(VendorClass.vendorClassID), DescriptionField = typeof(VendorClass.descr), CacheGlobal = true)]
			[PXUIField(DisplayName = "Vendor Class")]
			public virtual String VendorClassID
			{
				get
				{
					return this._VendorClassID;
				}
				set
				{
					this._VendorClassID = value;
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
			[CR.BAccount.status.ListAttribute()]
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
			[PXUIField(DisplayName = "Vendors with Balance Only")]
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
			#region FinPeriodID
			public abstract class finPeriodID : PX.Data.IBqlField
			{
			}
			protected String _FinPeriodID;
			//[APClosedPeriod()]
			[APAnyPeriodFilterable()]
			[PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.Visible)]
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
			#region SplitByCurrency
			public abstract class splitByCurrency : PX.Data.IBqlField
			{
			}
			protected bool? _SplitByCurrency;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Split by Currency")]
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
			#region SubCD Wildcard
			public abstract class subCDWildcard : PX.Data.IBqlField { };
			[PXDBString(30, IsUnicode = true)]
			public virtual String SubCDWildcard
			{
				get
				{
					return SubCDUtils.CreateSubCDWildcard(this._SubID, SubAccountAttribute.DimensionName);
				}
			}



			#endregion

			#region CuryBalanceSummary
			public abstract class curyBalanceSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryBalanceSummary;
			[PXBaseCury(typeof(APHistoryFilter.curyID))]
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
			[PXBaseCury()]
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
	
			#region CuryDepositsSummary
			public abstract class curyDepositsSummary : PX.Data.IBqlField
			{
			}

			protected Decimal? _CuryDepositsSummary;			
			[PXCury(typeof(APHistoryFilter.curyID))]
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
			//[PXDBDecimal()]
			[PXBaseCury()]
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[PXDBInt()]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			public virtual void ClearSummary() 
			{
				this.BalanceSummary = Decimal.Zero;
				this.DepositsSummary = Decimal.Zero;
				this.CuryBalanceSummary = Decimal.Zero;
				this.CuryDepositsSummary = Decimal.Zero;
			}
		}

		[Serializable]
		public partial class APHistoryResult : IBqlTable
		{
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[PXDBInt()]
			[PXDefault()]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region AcctCD
			public abstract class acctCD : PX.Data.IBqlField
			{
			}
			protected string _AcctCD;
			[PXDimensionSelector(VendorAttribute.DimensionName, typeof(Vendor.acctCD), typeof(acctCD),
				typeof(Vendor.acctCD), typeof(Vendor.acctName))]
			[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
			[PXUIField(DisplayName = "Vendor ID", Visibility = PXUIVisibility.SelectorVisible)]
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
			[PXUIField(DisplayName = "Vendor Name", Visibility = PXUIVisibility.SelectorVisible)]
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
			#region FinPeriod
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			[PXUIField(DisplayName = "Balance", Visible = false)]
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
			#region CuryPurchases
			public abstract class curyPurchases : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryPurchases;
			[PXDBCury(typeof(APHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Purchases")]
			public virtual Decimal? CuryPurchases
			{
				get
				{
					return this._CuryPurchases;
				}
				set
				{
					this._CuryPurchases = value;
				}
			}
			#endregion
			#region Purchases
			public abstract class purchases : PX.Data.IBqlField
			{
			}
			protected Decimal? _Purchases;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Purchases")]
			public virtual Decimal? Purchases
			{
				get
				{
					return this._Purchases;
				}
				set
				{
					this._Purchases = value;
				}
			}
			#endregion
			#region CuryPayments
			public abstract class curyPayments : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryPayments;
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			#region CuryWhTax
			public abstract class curyWhTax : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryWhTax;
			[PXDBCury(typeof(APHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Tax Withheld")]
			public virtual Decimal? CuryWhTax
			{
				get
				{
					return this._CuryWhTax;
				}
				set
				{
					this._CuryWhTax = value;
				}
			}
			#endregion
			#region WhTax
			public abstract class whTax : PX.Data.IBqlField
			{
			}
			protected Decimal? _WhTax;
			[PXBaseCury()]
			[PXUIField(DisplayName = "PTD Tax Withheld")]
			public virtual Decimal? WhTax
			{
				get
				{
					return this._WhTax;
				}
				set
				{
					this._WhTax = value;
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Credit Adjustments")]
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
			[PXUIField(DisplayName = "PTD Credit Adjustments")]
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
			[PXUIField(DisplayName = "Currency PTD Debit Adjustments")]
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
			[PXUIField(DisplayName = "PTD Debit Adjustments")]
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
			#region CuryDeposits
			public abstract class curyDeposits : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDeposits;
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			[PXDBCury(typeof(APHistoryResult.curyID))]
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
			#region NoteID
			public abstract class noteID : PX.Data.IBqlField
			{
			}
			protected Guid? _NoteID;
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
			public virtual void RecalculateEndBalance()
			{
				const decimal zero = Decimal.Zero;
				this.RecalculateBalance();
				this.CuryEndBalance = (this.CuryBegBalance ?? zero) + (this.CuryBalance ?? zero);
				this.EndBalance = (this.BegBalance ?? zero) + (this.Balance ?? zero);
			}
			public virtual void RecalculateBalance()
			{
				const decimal zero = Decimal.Zero;
				this.Balance = (this.Purchases ?? zero)
							   - (this.Payments ?? zero)
							   - (this.Discount ?? zero)
							   - (this.WhTax ?? zero)
							   + (this.RGOL ?? zero)
							   - (this.DrAdjustments ?? zero)
							   + (this.CrAdjustments ?? zero);
				this.CuryBalance = (this.CuryPurchases ?? zero)
							   - (this.CuryPayments ?? zero)
							   - (this.CuryDiscount ?? zero)
							   - (this.CuryWhTax ?? zero)							   
							   - (this.CuryDrAdjustments ?? zero)
							   + (this.CuryCrAdjustments ?? zero);
			}
			public virtual void CopyValueToCuryValue(string aBaseCuryID)
			{
				this.CuryBegBalance = this.BegBalance ?? Decimal.Zero;				
				this.CuryPurchases = this.Purchases ?? Decimal.Zero;
				this.CuryPayments = this.Payments ?? Decimal.Zero;
				this.CuryDiscount = this.Discount ?? Decimal.Zero;
				this.CuryWhTax = this.WhTax ?? Decimal.Zero;
				this.CuryCrAdjustments = this.CrAdjustments ?? Decimal.Zero;
				this.CuryDrAdjustments = this.DrAdjustments ?? Decimal.Zero;
				this.CuryDeposits = this.Deposits ?? Decimal.Zero;
				this.CuryDepositsBalance = this.DepositsBalance ?? Decimal.Zero;
				this.CuryID = aBaseCuryID;
				this.CuryEndBalance = this.EndBalance ?? Decimal.Zero;
				this.Converted = true;
			}
		}

		#region Service Types
		[Serializable]
		[PXProjection(typeof(Select4<CuryAPHistory,
			Aggregate<
			GroupBy<CuryAPHistory.branchID,
			GroupBy<CuryAPHistory.vendorID,
			GroupBy<CuryAPHistory.accountID,
			GroupBy<CuryAPHistory.subID,
			GroupBy<CuryAPHistory.curyID,
			Max<CuryAPHistory.finPeriodID
			>>>>>>>>))]
		[PXCacheName(Messages.APLatestHistory)]
		public partial class APLatestHistory : PX.Data.IBqlTable
		{
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryAPHistory.branchID))]
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryAPHistory.vendorID))]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[PXDBInt(IsKey = true, BqlField = typeof(CuryAPHistory.accountID))]
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
			[PXDBInt(IsKey = true, BqlField = typeof(CuryAPHistory.subID))]
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
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(CuryAPHistory.curyID))]
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
			[GL.FinPeriodID(BqlField = typeof(CuryAPHistory.finPeriodID))]
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
		public sealed class APH : CuryAPHistory
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
			#region VendorID
			public new abstract class vendorID : PX.Data.IBqlField { }
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
			public abstract new class curyTranPtdDeposits : PX.Data.IBqlField	{}
			#endregion
			#region CuryTranYtdDeposits
			public abstract new class curyTranYtdDeposits : PX.Data.IBqlField	{	}
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

		}

		private sealed class decimalZero : Constant<decimal>
		{
			public decimalZero()
				: base(Decimal.Zero)
			{
			}
		}
		#endregion
		#endregion

		#region Views/Selects
		
		public PXFilter<APHistoryFilter> Filter;
		public PXCancel<APHistoryFilter> Cancel;
		[PXFilterable]
		public PXSelect<APHistoryResult> History;
		public PXSetup<APSetup> APSetup;
		public PXSetup<Company> Company;

		protected virtual IEnumerable history()
		{
			APHistoryFilter header = Filter.Current;
			APHistoryResult[] empty = null;
			if (header == null)
			{
				return empty;
			}
			header.ClearSummary();
			string fiscalPeriod = header.FinPeriodID;
			Dictionary<KeyValuePair<int, string>, APHistoryResult> result;
			if (fiscalPeriod == null)
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
			PXCache cache = this.Caches[typeof(APHistoryFilter)];
			if (cache != null)
			{
				APHistoryFilter filter = cache.Current as APHistoryFilter;
				if (filter != null)
				{
					filter.ClearSummary();
					foreach (APHistoryResult it in this.History.Select())
					{
						Aggregate(filter, it);
					}
				}
			}
			yield return cache.Current;
			cache.IsDirty = false;
		}

		#endregion

		#region Ctor+overrides
		public APVendorBalanceEnq()
		{
			APSetup setup = this.APSetup.Current;
			Company company = this.Company.Current;
			this.History.Cache.AllowDelete = false;
			this.History.Cache.AllowInsert = false;
			this.History.Cache.AllowUpdate = false;
			this.reports.MenuAutoOpen = true;
			this.reports.AddMenuAction(this.aPBalanceByVendorReport);
			this.reports.AddMenuAction(this.vendorHistoryReport);
			this.reports.AddMenuAction(this.aPAgedPastDueReport);
			this.reports.AddMenuAction(this.aPAgedOutstandingReport);
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

		public PXAction<APHistoryFilter> viewDetails;
		public PXAction<APHistoryFilter> previousPeriod;
		public PXAction<APHistoryFilter> nextPeriod;
		public PXAction<APHistoryFilter> reports;
		public PXAction<APHistoryFilter> aPBalanceByVendorReport;
		public PXAction<APHistoryFilter> vendorHistoryReport;
		public PXAction<APHistoryFilter> aPAgedPastDueReport;
		public PXAction<APHistoryFilter> aPAgedOutstandingReport;
		

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDetails(PXAdapter adapter)
		{
			if (this.History.Current != null && this.Filter.Current != null)
			{
				APHistoryResult res = this.History.Current;
				APHistoryFilter currentFilter = this.Filter.Current;
				APDocumentEnq graph = PXGraph.CreateInstance<APDocumentEnq>();
				APDocumentEnq.APDocumentFilter filter = graph.Filter.Current;
				Copy(filter, currentFilter);
				filter.VendorID = res.VendorID;
				filter.BalanceSummary = null;
				graph.Filter.Update(filter);
				filter = graph.Filter.Select();
				throw new PXRedirectRequiredException(graph, "Vendor Details");
			}
			return Filter.Select();
		}
		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable PreviousPeriod(PXAdapter adapter)
		{
			APHistoryFilter filter = Filter.Current as APHistoryFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindPrevPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable NextPeriod(PXAdapter adapter)
		{
			APHistoryFilter filter = Filter.Current as APHistoryFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindNextPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Reports(PXAdapter adapter)
		{
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.APBalanceByVendorReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable APBalanceByVendorReport(PXAdapter adapter)
		{
			APHistoryFilter filter = Filter.Current;
			APHistoryResult history = History.Current;

			if (filter != null && history != null)
			{
				Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APHistoryResult.vendorID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(filter.FinPeriodID))
				{
					parameters["PeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
				}
				parameters["VendorID"] = vendor.AcctCD;
				throw new PXReportRequiredException(parameters, "AP632500", Messages.APBalanceByVendorReport);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.VendorHistoryReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable VendorHistoryReport(PXAdapter adapter)
		{
			APHistoryFilter filter = Filter.Current;
			APHistoryResult history = History.Current;
			if (filter != null && history != null)
			{
				Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APHistoryResult.vendorID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(filter.FinPeriodID))
				{
					parameters["FromPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
					parameters["ToPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
				}
				parameters["VendorID"] = vendor.AcctCD;
				throw new PXReportRequiredException(parameters, "AP652000", Messages.VendorHistoryReport);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.APAgedPastDueReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable APAgedPastDueReport(PXAdapter adapter)
		{
			APHistoryResult history = History.Current;
			if (history != null)
			{
				Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APHistoryResult.vendorID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["VendorID"] = vendor.AcctCD;
				throw new PXReportRequiredException(parameters, "AP631000", Messages.APAgedPastDueReport);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.APAgedOutstandingReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable APAgedOutstandingReport(PXAdapter adapter)
		{
			APHistoryResult history = History.Current;
			if (history != null)
			{
				Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APHistoryResult.vendorID>>>>.Select(this);
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["VendorID"] = vendor.AcctCD;
				throw new PXReportRequiredException(parameters, "AP631500", Messages.APAgedOutstandingReport);
			}
			return adapter.Get();
		}

		#endregion

		#region Event Handlers

		public virtual void APHistoryFilter_SubID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		public virtual void APHistoryFilter_AccountID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APHistoryFilter header = e.Row as APHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.AccountID = null;
			}
		}

		public virtual void APHistoryFilter_CashAcctID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APHistoryFilter header = e.Row as APHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.CashAcctID = null;
			}
		}

		public virtual void APHistoryFilter_CuryID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APHistoryFilter header = e.Row as APHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.CuryID = null;
			}
		}

		public virtual void APHistoryFilter_FinPeriodID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APHistoryFilter header = e.Row as APHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.FinPeriodID = null;
			}
		}

		public virtual void APHistoryFilter_VendorClassID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APHistoryFilter header = e.Row as APHistoryFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.VendorClassID = null;
			}
		}

		public virtual void APHistoryFilter_CuryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			cache.SetDefaultExt<APHistoryFilter.splitByCurrency>(e.Row);			
		}

		public virtual void APHistoryFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			APHistoryFilter row = (APHistoryFilter)e.Row;
			if (row != null)
			{
				Company company = this.Company.Current;
				bool mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
				PXUIFieldAttribute.SetVisible<APHistoryFilter.showWithBalanceOnly>(cache, row, true);
				PXUIFieldAttribute.SetVisible<APHistoryFilter.byFinancialPeriod>(cache, row, true);
				PXUIFieldAttribute.SetVisible<APHistoryFilter.curyID>(cache, row, mcFeatureInstalled);

				
				bool isCurySelected = string.IsNullOrEmpty(row.CuryID) == false;
				bool isForeignCurrency = string.IsNullOrEmpty(row.CuryID) == false && (company.BaseCuryID != row.CuryID);
				bool isBaseCurySelected = string.IsNullOrEmpty(row.CuryID) == false && (company.BaseCuryID == row.CuryID);
				bool splitByCurrency = (row.SplitByCurrency ?? false);

				PXUIFieldAttribute.SetVisible<APHistoryFilter.splitByCurrency>(cache, row, mcFeatureInstalled && !isCurySelected);
				PXUIFieldAttribute.SetEnabled<APHistoryFilter.splitByCurrency>(cache, row, mcFeatureInstalled && !isCurySelected);
				PXUIFieldAttribute.SetVisible<APHistoryFilter.curyBalanceSummary>(cache, row, isForeignCurrency);
				PXUIFieldAttribute.SetVisible<APHistoryFilter.curyDepositsSummary>(cache, row, isForeignCurrency);

				PXCache detailCache = this.History.Cache;
				bool hideCuryColumns = (!mcFeatureInstalled) || (isBaseCurySelected) || (!isCurySelected && !splitByCurrency);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyID>(this.History.Cache, null, mcFeatureInstalled && (isCurySelected || splitByCurrency));
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyBalance>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyPayments>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyPurchases>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyWhTax>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyDiscount>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyCrAdjustments>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyDrAdjustments>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyDeposits>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyDepositsBalance>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyBegBalance>(detailCache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyEndBalance>(History.Cache, null, !hideCuryColumns);
				PXUIFieldAttribute.SetVisible<APHistoryResult.rGOL>(History.Cache, null, mcFeatureInstalled);
				
				PXUIFieldAttribute.SetVisible<APHistoryResult.balance>(detailCache, null, false);
				PXUIFieldAttribute.SetVisible<APHistoryResult.curyBalance>(detailCache, null, false);
				PXUIFieldAttribute.SetVisible<APHistoryResult.finPeriodID>(History.Cache, null);
				PXUIFieldAttribute.SetVisible<APHistoryResult.begBalance>(History.Cache, null);
				PXUIFieldAttribute.SetVisible<APHistoryResult.endBalance>(History.Cache, null);			
			}
		}

		#endregion
		
		protected virtual void RetrieveHistory(APHistoryFilter header, out Dictionary<KeyValuePair<int, string>, APHistoryResult> result)
		{
			result = new Dictionary<KeyValuePair<int, string>, APHistoryResult>();

			bool isCurySelected = string.IsNullOrEmpty(header.CuryID) == false;
			bool splitByCurrency = header.SplitByCurrency ?? false;
			bool useFinancial = (header.ByFinancialPeriod == true);

			#region FiscalPeriodUndefined
			PXSelectBase<APLatestHistory> sel = new PXSelectJoinGroupBy<APLatestHistory, InnerJoin<Vendor,
								On<APLatestHistory.vendorID, Equal<Vendor.bAccountID>,
								And<Match<Vendor, Current<AccessInfo.userName>>>>,
								LeftJoin<Sub, On<APLatestHistory.subID, Equal<Sub.subID>>,
								LeftJoin<CuryAPHistory, On<APLatestHistory.accountID, Equal<CuryAPHistory.accountID>,
								And<APLatestHistory.branchID, Equal<CuryAPHistory.branchID>,
								And<APLatestHistory.vendorID, Equal<CuryAPHistory.vendorID>,
								And<APLatestHistory.subID, Equal<CuryAPHistory.subID>,
								And<APLatestHistory.curyID, Equal<CuryAPHistory.curyID>,
								And<APLatestHistory.lastActivityPeriod, Equal<CuryAPHistory.finPeriodID>>>>>>>>>>,
								Aggregate<
									Sum<CuryAPHistory.finBegBalance,
									Sum<CuryAPHistory.curyFinBegBalance,
									Sum<CuryAPHistory.finYtdBalance,
									Sum<CuryAPHistory.curyFinYtdBalance,
									Sum<CuryAPHistory.tranBegBalance,
									Sum<CuryAPHistory.curyTranBegBalance,
									Sum<CuryAPHistory.tranYtdBalance,
									Sum<CuryAPHistory.curyTranYtdBalance,

									Sum<CuryAPHistory.finPtdPayments,
									Sum<CuryAPHistory.finPtdPurchases,
									Sum<CuryAPHistory.finPtdDiscTaken,
									Sum<CuryAPHistory.finPtdWhTax,
									Sum<CuryAPHistory.finPtdCrAdjustments,
									Sum<CuryAPHistory.finPtdDrAdjustments,
									Sum<CuryAPHistory.finPtdRGOL,
									Sum<CuryAPHistory.finPtdDeposits,
									Sum<CuryAPHistory.finYtdDeposits,


									Sum<CuryAPHistory.tranPtdPayments,
									Sum<CuryAPHistory.tranPtdPurchases,
									Sum<CuryAPHistory.tranPtdDiscTaken,
									Sum<CuryAPHistory.tranPtdWhTax,
									Sum<CuryAPHistory.tranPtdCrAdjustments,
									Sum<CuryAPHistory.tranPtdDrAdjustments,
									Sum<CuryAPHistory.tranPtdRGOL,
									Sum<CuryAPHistory.tranPtdDeposits,
									Sum<CuryAPHistory.tranYtdDeposits,


									Sum<CuryAPHistory.curyFinPtdPayments,
									Sum<CuryAPHistory.curyFinPtdPurchases,
									Sum<CuryAPHistory.curyFinPtdDiscTaken,
									Sum<CuryAPHistory.curyFinPtdWhTax,
									Sum<CuryAPHistory.curyFinPtdCrAdjustments,
									Sum<CuryAPHistory.curyFinPtdDrAdjustments,
									Sum<CuryAPHistory.curyFinPtdDeposits,
									Sum<CuryAPHistory.curyFinYtdDeposits,

									Sum<CuryAPHistory.curyTranPtdPayments,
									Sum<CuryAPHistory.curyTranPtdPurchases,
									Sum<CuryAPHistory.curyTranPtdDiscTaken,
									Sum<CuryAPHistory.curyTranPtdWhTax,
									Sum<CuryAPHistory.curyTranPtdCrAdjustments,
									Sum<CuryAPHistory.curyTranPtdDrAdjustments,
									Sum<CuryAPHistory.curyTranPtdDeposits,
									Sum<CuryAPHistory.curyTranYtdDeposits,

									GroupBy<APLatestHistory.lastActivityPeriod,
									GroupBy<APLatestHistory.curyID,
									GroupBy<Vendor.bAccountID>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
									(this);

			if (header.BranchID != null)
			{
				sel.WhereAnd<Where<APLatestHistory.branchID, Equal<Current<APHistoryFilter.branchID>>>>();
			}

			if (header.AccountID != null)
			{
				sel.WhereAnd<Where<APLatestHistory.accountID, Equal<Current<APHistoryFilter.accountID>>>>();
			}

			if (!SubCDUtils.IsSubCDEmpty(header.SubID))
			{
				sel.WhereAnd<Where<Sub.subCD, Like<Current<APHistoryFilter.subCDWildcard>>>>();
			}
			if (isCurySelected)
			{
				sel.WhereAnd<Where<APLatestHistory.curyID, Equal<Current<APHistoryFilter.curyID>>>>();
			}
			if (header.VendorClassID != null)
			{
				sel.WhereAnd<Where<Vendor.vendorClassID, Equal<Current<APHistoryFilter.vendorClassID>>>>();
			}
			if (header.VendorID != null)
			{
				sel.WhereAnd<Where<Vendor.bAccountID, Equal<Current<APHistoryFilter.vendorID>>>>();
			}

			if (header.Status != null)
			{
				sel.WhereAnd<Where<Vendor.status, Equal<Current<APHistoryFilter.status>>>>();
			}


			foreach (PXResult<APLatestHistory, Vendor, Sub, CuryAPHistory> record in sel.Select())
			{
				Vendor vendor = record;
				CuryAPHistory history = record;
				APHistoryResult res = new APHistoryResult();
				CopyFrom(res, vendor);
				CopyFrom(res, history, useFinancial);
				res.FinPeriodID = history.FinPeriodID;
				string keyCuryID = (isCurySelected || splitByCurrency) ? history.CuryID : this.Company.Current.BaseCuryID;
				KeyValuePair<int, string> key = new KeyValuePair<int, string>(vendor.BAccountID.Value, keyCuryID);

				if ((!isCurySelected) && splitByCurrency == false)
				{
					res.CopyValueToCuryValue(this.Company.Current.BaseCuryID);
				}

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

		protected virtual void RetrieveHistoryForPeriod(APHistoryFilter header, out Dictionary<KeyValuePair<int, string>, APHistoryResult> result)
		{
			result = new Dictionary<KeyValuePair<int, string>, APHistoryResult>();
			bool isCurySelected = string.IsNullOrEmpty(header.CuryID) == false;
			bool splitByCurrency = header.SplitByCurrency ?? false;
			bool useFinancial = (header.ByFinancialPeriod == true);
			string fiscalPeriod = header.FinPeriodID;

			#region Specific Fiscal Period
			PXSelectBase<APHistoryByPeriod> sel = new PXSelectJoinGroupBy<APHistoryByPeriod,
							InnerJoin<Vendor,
							On<APHistoryByPeriod.vendorID, Equal<Vendor.bAccountID>,
							And<Match<Vendor, Current<AccessInfo.userName>>>>,
							LeftJoin<Sub, On<APHistoryByPeriod.subID, Equal<Sub.subID>>,
							LeftJoin<CuryAPHistory, On<APHistoryByPeriod.accountID, Equal<CuryAPHistory.accountID>,
							And<APHistoryByPeriod.branchID, Equal<CuryAPHistory.branchID>,
							And<APHistoryByPeriod.vendorID, Equal<CuryAPHistory.vendorID>,
							And<APHistoryByPeriod.subID, Equal<CuryAPHistory.subID>,
							And<APHistoryByPeriod.curyID, Equal<CuryAPHistory.curyID>,
							And<APHistoryByPeriod.finPeriodID, Equal<CuryAPHistory.finPeriodID>>>>>>>,

							LeftJoin<APH, On<APHistoryByPeriod.accountID, Equal<APH.accountID>,
							And<APHistoryByPeriod.branchID, Equal<APH.branchID>,
							And<APHistoryByPeriod.vendorID, Equal<APH.vendorID>,
							And<APHistoryByPeriod.subID, Equal<APH.subID>,
							And<APHistoryByPeriod.curyID, Equal<APH.curyID>,
							And<APHistoryByPeriod.lastActivityPeriod, Equal<APH.finPeriodID>>>>>>>>>>>,

							Where<APHistoryByPeriod.finPeriodID, Equal<Required<APHistoryFilter.finPeriodID>>>,
							Aggregate<
								Sum<CuryAPHistory.finBegBalance,
								Sum<CuryAPHistory.curyFinBegBalance,
								Sum<CuryAPHistory.finYtdBalance,
								Sum<CuryAPHistory.tranBegBalance,
								Sum<CuryAPHistory.curyTranBegBalance,
								Sum<CuryAPHistory.tranYtdBalance,

								Sum<CuryAPHistory.finPtdCrAdjustments,
								Sum<CuryAPHistory.finPtdDiscTaken,
								Sum<CuryAPHistory.finPtdDrAdjustments,
								Sum<CuryAPHistory.finPtdPayments,
								Sum<CuryAPHistory.finPtdPurchases,
								Sum<CuryAPHistory.finPtdWhTax,
								Sum<CuryAPHistory.finPtdRGOL,
								Sum<CuryAPHistory.finPtdDeposits,
								Sum<CuryAPHistory.finYtdDeposits,

								Sum<CuryAPHistory.tranPtdCrAdjustments,
								Sum<CuryAPHistory.tranPtdDiscTaken,
								Sum<CuryAPHistory.tranPtdDrAdjustments,
								Sum<CuryAPHistory.tranPtdPayments,
								Sum<CuryAPHistory.tranPtdPurchases,
								Sum<CuryAPHistory.tranPtdWhTax,
								Sum<CuryAPHistory.tranPtdRGOL,
								Sum<CuryAPHistory.tranPtdDeposits,
								Sum<CuryAPHistory.tranYtdDeposits,

								Sum<CuryAPHistory.curyFinPtdCrAdjustments,
								Sum<CuryAPHistory.curyFinPtdDiscTaken,
								Sum<CuryAPHistory.curyFinPtdDrAdjustments,
								Sum<CuryAPHistory.curyFinPtdPayments,
								Sum<CuryAPHistory.curyFinPtdPurchases,
								Sum<CuryAPHistory.curyFinPtdWhTax,
								Sum<CuryAPHistory.curyFinPtdDeposits,
								Sum<CuryAPHistory.curyFinYtdDeposits,

								Sum<CuryAPHistory.curyTranPtdCrAdjustments,
								Sum<CuryAPHistory.curyTranPtdDiscTaken,
								Sum<CuryAPHistory.curyTranPtdDrAdjustments,
								Sum<CuryAPHistory.curyTranPtdPayments,
								Sum<CuryAPHistory.curyTranPtdPurchases,
								Sum<CuryAPHistory.curyTranPtdWhTax,
								Sum<CuryAPHistory.curyTranPtdDeposits,
								Sum<CuryAPHistory.curyTranYtdDeposits,

								Sum<APH.curyFinYtdBalance,
								Sum<APH.finYtdBalance,
								Sum<APH.curyTranYtdBalance,
								Sum<APH.tranYtdBalance,
								Sum<APH.curyFinYtdBalance,
								Sum<APH.finBegBalance,
								Sum<APH.curyFinBegBalance,
								Sum<APH.tranBegBalance,
								Sum<APH.curyTranBegBalance,
								Sum<APH.curyFinYtdDeposits,
								Sum<APH.curyTranYtdDeposits,
								Sum<APH.finYtdDeposits,
								Sum<APH.tranYtdDeposits,
								Sum<APH.curyFinPtdDeposits,

								GroupBy<APHistoryByPeriod.lastActivityPeriod,
								GroupBy<APHistoryByPeriod.finPeriodID,
								GroupBy<APHistoryByPeriod.curyID,
								GroupBy<Vendor.bAccountID>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
								(this);
			if (isCurySelected)
			{
				sel.WhereAnd<Where<APHistoryByPeriod.curyID, Equal<Current<APHistoryFilter.curyID>>>>();
			}

			if (header.BranchID != null)
			{
				sel.WhereAnd<Where<APHistoryByPeriod.branchID, Equal<Current<APHistoryFilter.branchID>>>>();
			}

			if (header.AccountID != null)
			{
				sel.WhereAnd<Where<APHistoryByPeriod.accountID, Equal<Current<APHistoryFilter.accountID>>>>();
			}
			if (!SubCDUtils.IsSubCDEmpty(header.SubID))
			{
				sel.WhereAnd<Where<Sub.subCD, Like<Current<APHistoryFilter.subCDWildcard>>>>();
			}
			if (header.VendorClassID != null)
			{
				sel.WhereAnd<Where<Vendor.vendorClassID, Equal<Current<APHistoryFilter.vendorClassID>>>>();
			}
			if (header.VendorID != null)
			{
				sel.WhereAnd<Where<Vendor.bAccountID, Equal<Current<APHistoryFilter.vendorID>>>>();
			}
			if (header.Status != null)
			{
				sel.WhereAnd<Where<Vendor.status, Equal<Current<APHistoryFilter.status>>>>();
			}
			foreach (PXResult<APHistoryByPeriod, Vendor, Sub, CuryAPHistory, APH> record in sel.Select(fiscalPeriod))
			{
				Vendor vendor = record;
				CuryAPHistory history = record;
				APH lastActivity = record;
				APHistoryByPeriod hstByPeriod = record;
				APHistoryResult res = new APHistoryResult();
				CopyFrom(res, vendor);
				CopyFrom(res, history, useFinancial);
				if (string.IsNullOrEmpty(res.CuryID))
				{
					res.CuryID = hstByPeriod.CuryID;
				}

				string keyCuryID = (isCurySelected || splitByCurrency) ? hstByPeriod.CuryID : this.Company.Current.BaseCuryID;
				KeyValuePair<int, string> key = new KeyValuePair<int, string>(vendor.BAccountID.Value, keyCuryID);
				res.FinPeriodID = lastActivity.FinPeriodID;
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
					Aggregate(result[key], res);
				}
				else
				{
					result[key] = res;
				}
			}
			#endregion
		}
		
		#region Utility Functions

		protected virtual string GetLastActivityPeriod(int? aVendorID)
		{
			PXSelectBase<CuryAPHistory> activitySelect = new PXSelect<CuryAPHistory, Where<CuryAPHistory.vendorID, Equal<Required<CuryAPHistory.vendorID>>>, OrderBy<Desc<CuryAPHistory.finPeriodID>>>(this);
			CuryAPHistory result = (CuryAPHistory)activitySelect.View.SelectSingle(aVendorID);
			if (result != null)
				return result.FinPeriodID;
			return null;
		}
		protected virtual void CopyFrom(APHistoryResult aDest, Vendor aVendor)
		{
			aDest.AcctCD = aVendor.AcctCD;
			aDest.AcctName = aVendor.AcctName;
			aDest.CuryID = aVendor.CuryID;
			aDest.VendorID = aVendor.BAccountID;
			aDest.NoteID = aVendor.NoteID;


		}
		protected virtual void CopyFrom(APHistoryResult aDest, CuryAPHistory aHistory, bool aIsFinancial)
		{
			if (aIsFinancial)
			{
				aDest.CuryBegBalance = aHistory.CuryFinBegBalance ?? Decimal.Zero;
				aDest.CuryPurchases = aHistory.CuryFinPtdPurchases ?? Decimal.Zero;
				aDest.CuryPayments = aHistory.CuryFinPtdPayments ?? Decimal.Zero;
				aDest.CuryDiscount = aHistory.CuryFinPtdDiscTaken ?? Decimal.Zero;
				aDest.CuryWhTax = aHistory.CuryFinPtdWhTax ?? Decimal.Zero;
				aDest.CuryCrAdjustments = aHistory.CuryFinPtdCrAdjustments ?? Decimal.Zero;
				aDest.CuryDrAdjustments = aHistory.CuryFinPtdDrAdjustments ?? Decimal.Zero;
				aDest.CuryDeposits = aHistory.CuryFinPtdDeposits ?? Decimal.Zero;
				aDest.CuryDepositsBalance = -aHistory.CuryFinYtdDeposits ?? Decimal.Zero;

				aDest.BegBalance = aHistory.FinBegBalance ?? Decimal.Zero;
				aDest.Purchases = aHistory.FinPtdPurchases ?? Decimal.Zero;
				aDest.Payments = aHistory.FinPtdPayments ?? Decimal.Zero;
				aDest.Discount = aHistory.FinPtdDiscTaken ?? Decimal.Zero;
				aDest.WhTax = aHistory.FinPtdWhTax ?? Decimal.Zero;
				aDest.RGOL = -aHistory.FinPtdRGOL ?? Decimal.Zero;
				aDest.CrAdjustments = aHistory.FinPtdCrAdjustments ?? Decimal.Zero;
				aDest.DrAdjustments = aHistory.FinPtdDrAdjustments ?? Decimal.Zero;
				aDest.Deposits = aHistory.FinPtdDeposits ?? Decimal.Zero;
				aDest.DepositsBalance = -aHistory.FinYtdDeposits ?? Decimal.Zero;
				aDest.CuryID = aHistory.CuryID;				
			}
			else
			{
				aDest.CuryBegBalance = aHistory.CuryTranBegBalance ?? Decimal.Zero;
				aDest.CuryPurchases = aHistory.CuryTranPtdPurchases ?? Decimal.Zero;
				aDest.CuryPayments = aHistory.CuryTranPtdPayments ?? Decimal.Zero;
				aDest.CuryDiscount = aHistory.CuryTranPtdDiscTaken ?? Decimal.Zero;
				aDest.CuryWhTax = aHistory.CuryTranPtdWhTax ?? Decimal.Zero;
				aDest.CuryCrAdjustments = aHistory.CuryTranPtdCrAdjustments ?? Decimal.Zero;
				aDest.CuryDrAdjustments = aHistory.CuryTranPtdDrAdjustments ?? Decimal.Zero;
				aDest.CuryDeposits = aHistory.CuryTranPtdDeposits ?? Decimal.Zero;
				aDest.CuryDepositsBalance = -aHistory.CuryTranYtdDeposits ?? Decimal.Zero;

				aDest.BegBalance = aHistory.TranBegBalance ?? Decimal.Zero;
				aDest.Purchases = aHistory.TranPtdPurchases ?? Decimal.Zero;
				aDest.Payments = aHistory.TranPtdPayments ?? Decimal.Zero;
				aDest.Discount = aHistory.TranPtdDiscTaken ?? Decimal.Zero;
				aDest.WhTax = aHistory.TranPtdWhTax ?? Decimal.Zero;
				aDest.RGOL = -aHistory.TranPtdRGOL ?? Decimal.Zero;
				aDest.CrAdjustments = aHistory.TranPtdCrAdjustments ?? Decimal.Zero;
				aDest.DrAdjustments = aHistory.TranPtdDrAdjustments ?? Decimal.Zero;
				aDest.Deposits = aHistory.TranPtdDeposits ?? Decimal.Zero;
				aDest.DepositsBalance = -aHistory.TranYtdDeposits ?? Decimal.Zero;
				aDest.CuryID = aHistory.CuryID;
			}
			aDest.RecalculateEndBalance();
		}

		protected virtual void CopyFrom(APHistoryResult aDest, CuryAPHistory aHistory, bool aUseCurrency, bool aIsFinancial)
		{
			if (aIsFinancial)
			{

				if (aUseCurrency)
				{
					aDest.BegBalance = aHistory.CuryFinBegBalance ?? Decimal.Zero;
					aDest.Purchases = aHistory.CuryFinPtdPurchases ?? Decimal.Zero;
					aDest.Payments = aHistory.CuryFinPtdPayments ?? Decimal.Zero;
					aDest.Discount = aHistory.CuryFinPtdDiscTaken ?? Decimal.Zero;
					aDest.WhTax = aHistory.CuryFinPtdWhTax ?? Decimal.Zero;
					aDest.RGOL = Decimal.Zero;
					aDest.CrAdjustments = aHistory.CuryFinPtdCrAdjustments ?? Decimal.Zero;
					aDest.DrAdjustments = aHistory.CuryFinPtdDrAdjustments ?? Decimal.Zero;
					
					aDest.Deposits = aHistory.CuryFinPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.CuryFinYtdDeposits ?? Decimal.Zero;
					aDest.CuryID = aHistory.CuryID;
				}
				else
				{
					aDest.BegBalance = aHistory.FinBegBalance ?? Decimal.Zero;
					aDest.Purchases = aHistory.FinPtdPurchases ?? Decimal.Zero;
					aDest.Payments = aHistory.FinPtdPayments ?? Decimal.Zero;
					aDest.Discount = aHistory.FinPtdDiscTaken ?? Decimal.Zero;
					aDest.WhTax = aHistory.FinPtdWhTax ?? Decimal.Zero;
					aDest.RGOL = -aHistory.FinPtdRGOL ?? Decimal.Zero;
					aDest.CrAdjustments = aHistory.FinPtdCrAdjustments ?? Decimal.Zero;
					aDest.DrAdjustments = aHistory.FinPtdDrAdjustments ?? Decimal.Zero;
					aDest.Deposits = aHistory.FinPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.FinYtdDeposits ?? Decimal.Zero;
					
				}
			}
			else
			{
				if (aUseCurrency)
				{
					aDest.BegBalance = aHistory.CuryTranBegBalance ?? Decimal.Zero;
					aDest.Purchases = aHistory.CuryTranPtdPurchases ?? Decimal.Zero;
					aDest.Payments = aHistory.CuryTranPtdPayments ?? Decimal.Zero;
					aDest.Discount = aHistory.CuryTranPtdDiscTaken ?? Decimal.Zero;
					aDest.WhTax = aHistory.CuryTranPtdWhTax ?? Decimal.Zero;
					aDest.RGOL = Decimal.Zero;
					aDest.CrAdjustments = aHistory.CuryTranPtdCrAdjustments ?? Decimal.Zero;
					aDest.DrAdjustments = aHistory.CuryTranPtdDrAdjustments ?? Decimal.Zero;
					aDest.Deposits = aHistory.CuryTranPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.CuryTranYtdDeposits ?? Decimal.Zero;
					aDest.CuryID = aHistory.CuryID;
				}
				else
				{
					aDest.BegBalance = aHistory.TranBegBalance ?? Decimal.Zero;
					aDest.Purchases = aHistory.TranPtdPurchases ?? Decimal.Zero;
					aDest.Payments = aHistory.TranPtdPayments ?? Decimal.Zero;
					aDest.Discount = aHistory.TranPtdDiscTaken ?? Decimal.Zero;
					aDest.WhTax = aHistory.TranPtdWhTax ?? Decimal.Zero;
					aDest.RGOL = -aHistory.TranPtdRGOL ?? Decimal.Zero;
					aDest.CrAdjustments = aHistory.TranPtdCrAdjustments ?? Decimal.Zero;
					aDest.DrAdjustments = aHistory.TranPtdDrAdjustments ?? Decimal.Zero;
					aDest.Deposits = aHistory.TranPtdDeposits ?? Decimal.Zero;
					aDest.DepositsBalance = -aHistory.TranYtdDeposits ?? Decimal.Zero;
				}
			}
			aDest.RecalculateEndBalance();
		}
		protected virtual void Aggregate(APHistoryResult aDest, APHistoryResult aSrc)
		{			
			aDest.CuryBegBalance += aSrc.CuryBegBalance ?? Decimal.Zero;
			aDest.CuryCrAdjustments += aSrc.CuryCrAdjustments ?? Decimal.Zero;
			aDest.CuryDrAdjustments += aSrc.CuryDrAdjustments ?? Decimal.Zero;
			aDest.CuryDiscount += aSrc.CuryDiscount ?? Decimal.Zero;
			aDest.CuryWhTax += aSrc.CuryWhTax ?? Decimal.Zero;
			aDest.CuryPurchases += aSrc.CuryPurchases ?? Decimal.Zero;
			aDest.CuryPayments += aSrc.CuryPayments ?? Decimal.Zero;			
			aDest.CuryDeposits += aSrc.CuryDeposits ?? Decimal.Zero;
			aDest.CuryDepositsBalance += aSrc.CuryDepositsBalance ?? Decimal.Zero;
			
			aDest.BegBalance += aSrc.BegBalance ?? Decimal.Zero;
			aDest.CrAdjustments += aSrc.CrAdjustments ?? Decimal.Zero;
			aDest.DrAdjustments += aSrc.DrAdjustments ?? Decimal.Zero;
			aDest.Discount += aSrc.Discount ?? Decimal.Zero;
			aDest.WhTax += aSrc.WhTax ?? Decimal.Zero;
			aDest.Purchases += aSrc.Purchases ?? Decimal.Zero;
			aDest.Payments += aSrc.Payments ?? Decimal.Zero;
			aDest.RGOL += aSrc.RGOL ?? Decimal.Zero;
			aDest.Deposits += aSrc.Deposits ?? Decimal.Zero;
			aDest.DepositsBalance += aSrc.DepositsBalance ?? Decimal.Zero;
			aDest.RecalculateEndBalance();
		}

		protected virtual void Aggregate(APHistoryFilter aDest, APHistoryResult aSrc)
		{
			aDest.CuryBalanceSummary += aSrc.CuryEndBalance ?? Decimal.Zero;
			aDest.CuryDepositsSummary += aSrc.CuryDepositsBalance ?? Decimal.Zero;
			aDest.BalanceSummary += aSrc.EndBalance??Decimal.Zero;
			aDest.DepositsSummary += aSrc.DepositsBalance??Decimal.Zero;
		}
		protected virtual void AggregateLatest(APHistoryResult aDest, APHistoryResult aSrc)
		{
			if (aSrc.FinPeriodID == aDest.FinPeriodID)
			{
				Aggregate(aDest, aSrc);
			}
			else
			{
				if (string.Compare(aSrc.FinPeriodID,aDest.FinPeriodID)<0)
				{
					//Just update Beg Balance
					aDest.BegBalance += aSrc.EndBalance?? Decimal.Zero;
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
					aDest.WhTax = aSrc.WhTax ?? Decimal.Zero;
					aDest.Purchases = aSrc.Purchases ?? Decimal.Zero;
					aDest.Payments = aSrc.Payments ?? Decimal.Zero;
					aDest.RGOL = aSrc.RGOL ?? Decimal.Zero;
					aDest.FinPeriodID = aSrc.FinPeriodID;
					aDest.Deposits = aSrc.Deposits;
					aDest.DepositsBalance = (aDest.DepositsBalance?? Decimal.Zero) + (aSrc.DepositsBalance??Decimal.Zero);

					aDest.CuryBegBalance = (aDest.CuryEndBalance ?? Decimal.Zero) + (aSrc.CuryBegBalance ?? Decimal.Zero);
					aDest.CuryCrAdjustments = aSrc.CuryCrAdjustments ?? Decimal.Zero;
					aDest.CuryDrAdjustments = aSrc.CuryDrAdjustments ?? Decimal.Zero;
					aDest.CuryDiscount = aSrc.CuryDiscount ?? Decimal.Zero;
					aDest.CuryWhTax = aSrc.CuryWhTax ?? Decimal.Zero;
					aDest.CuryPurchases = aSrc.CuryPurchases ?? Decimal.Zero;
					aDest.CuryPayments = aSrc.CuryPayments ?? Decimal.Zero;					
					aDest.CuryDeposits = aSrc.CuryDeposits;
					aDest.CuryDepositsBalance = (aDest.CuryDepositsBalance ?? Decimal.Zero) + (aSrc.CuryDepositsBalance ?? Decimal.Zero);
				}
				aDest.RecalculateEndBalance();
			}
		}

		protected virtual void RemoveZeroBalances(Dictionary<KeyValuePair<int, string>, APHistoryResult> aResult) 
		{
			List<KeyValuePair<int, string>> toRemove = new List<KeyValuePair<int, string>>();			
			foreach (KeyValuePair<KeyValuePair<int,string>, APHistoryResult> iRes in aResult)
			{
				if ((iRes.Value.EndBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.DepositsBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.CuryEndBalance ?? Decimal.Zero) == Decimal.Zero
					&& (iRes.Value.CuryDepositsBalance ?? Decimal.Zero) == Decimal.Zero)
				{
					toRemove.Add(iRes.Key);
				}
			}

			foreach (KeyValuePair<int, string> iKey in toRemove)
			{
				aResult.Remove(iKey);
			}
		}
		public static void Copy(APDocumentEnq.APDocumentFilter filter, APHistoryFilter histFilter)
		{
			filter.BranchID = histFilter.BranchID;
			filter.FinPeriodID = histFilter.FinPeriodID;
			filter.AccountID = histFilter.AccountID;
			filter.SubCD = histFilter.SubID;
			filter.CuryID = histFilter.CuryID;
			filter.ByFinancialPeriod = histFilter.ByFinancialPeriod;
		}
		public static void Copy(APHistoryFilter histFilter, APDocumentEnq.APDocumentFilter filter)
		{
			histFilter.BranchID = filter.BranchID;
			histFilter.VendorID = filter.VendorID;
			histFilter.FinPeriodID = filter.FinPeriodID;
			histFilter.AccountID = filter.AccountID;
			histFilter.SubID = filter.SubCD;
			histFilter.CuryID = filter.CuryID;
			histFilter.ByFinancialPeriod = filter.ByFinancialPeriod;
		}
	
		#endregion
    }
}
