using System;
using PX.Data;
using System.Collections;

namespace PX.Objects.GL
{
	[Serializable]
	public partial class AccountByYearFilter : GLHistoryEnqFilter
	{
		public new abstract class branchID : IBqlField {}
		public new abstract class ledgerID : IBqlField {}
		public new abstract class accountID : IBqlField {}
		public new abstract class subID : IBqlField {}
		public new abstract class subCD : IBqlField {}
		public new abstract class subCDWildcard : IBqlField {}
		public new abstract class begFinPeriod : IBqlField {}
		public new abstract class showCuryDetail : IBqlField {}

		public override string BegFinPeriod => _FinYear != null ? FirstPeriodOfYear(_FinYear) : null;

		#region FinYear
		public abstract class finYear : IBqlField {}
		protected string _FinYear;
		[PXDBString(4)]
		[PXDefault]
		[PXUIField(DisplayName = "Financial Year", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.startDate>>>))]
		public virtual string FinYear
		{
			get { return _FinYear; }
			set { _FinYear = value; }
		}
		#endregion
	}

	[TableAndChartDashboardType]
	public class AccountHistoryByYearEnq : PXGraph<AccountHistoryByYearEnq>
	{
		#region Type Override events
		[AccountAny]
		protected virtual void GLHistoryEnquiryResult_AccountID_CacheAttached(PXCache sender)
		{
		}

		[FinPeriodID(IsKey = true)]
		[PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 0)]
		protected virtual void GLHistoryEnquiryResult_LastActivityPeriod_CacheAttached(PXCache sender)
		{
		}

		#endregion

		public PXCancel<AccountByYearFilter> Cancel;
		public PXAction<AccountByYearFilter> PreviousPeriod;
		public PXAction<AccountByYearFilter> NextPeriod;
		public PXFilter<AccountByYearFilter> Filter;
		public PXAction<AccountByYearFilter> accountDetails;
		public PXAction<AccountByYearFilter> accountBySub;
		[PXFilterable]
		public PXSelectOrderBy<GLHistoryEnquiryResult, OrderBy<Asc<GLHistoryEnquiryResult.lastActivityPeriod>>> EnqResult;
		public PXSetup<GLSetup> glsetup;
		public PXSelect<Account, Where<Account.accountID, Equal<Current<AccountByYearFilter.accountID>>>> AccountInfo;

		public FinYear fiscalyear => PXSelectReadonly<FinYear, Where<FinYear.year, Equal<Current<AccountByYearFilter.finYear>>>>.Select(this);

		private AccountByYearFilter CurrentFilter => Filter.Current;

		public AccountHistoryByYearEnq()
		{
			GLSetup setup = glsetup.Current;
			EnqResult.Cache.AllowInsert = false;
			EnqResult.Cache.AllowDelete = false;
			EnqResult.Cache.AllowUpdate = false;

		}
		#region Button Delegates
		[PXUIField(DisplayName = ActionsMessages.Previous, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable previousperiod(PXAdapter adapter)
		{
			AccountByYearFilter filter = CurrentFilter;
			FinYear nextperiod = PXSelect<FinYear,
				Where<FinYear.year, Less<Current<AccountByYearFilter.finYear>>>,
				OrderBy<Desc<FinYear.year>>>.SelectWindowed(this, 0, 1) 
			?? PXSelectOrderBy<FinYear,
				OrderBy<Desc<FinYear.year>>>.SelectWindowed(this, 0, 1);

			filter.FinYear = nextperiod?.Year;
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.Next, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable nextperiod(PXAdapter adapter)
		{
			AccountByYearFilter filter = CurrentFilter;
			FinYear nextperiod = PXSelect<FinYear,
				Where<FinYear.year, Greater<Current<AccountByYearFilter.finYear>>>,
				OrderBy<Asc<FinYear.year>>>.SelectWindowed(this, 0, 1) 
			?? PXSelectOrderBy<FinYear,
				OrderBy<Asc<FinYear.year>>>.SelectWindowed(this, 0, 1);

			filter.FinYear = nextperiod?.Year;
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.ViewAccountDetails)]
		[PXButton]
		protected virtual IEnumerable AccountDetails(PXAdapter adapter)
		{
			if (EnqResult.Current != null)
			{
				if (EnqResult.Current.AccountID == glsetup.Current.YtdNetIncAccountID)
					throw new PXException(Messages.DetailsReportIsNotAllowedForYTDNetIncome);
				AccountByPeriodEnq graph = CreateInstance<AccountByPeriodEnq>();
				AccountByPeriodFilter filter = PXCache<AccountByPeriodFilter>.CreateCopy(graph.Filter.Current);
				filter.AccountID = Filter.Current.AccountID;
				filter.LedgerID = Filter.Current.LedgerID;
				filter.BranchID = Filter.Current.BranchID;
				filter.SubID = Filter.Current.SubCD;
				filter.StartPeriodID = EnqResult.Current.LastActivityPeriod;
				filter.EndPeriodID = filter.StartPeriodID;
				filter.ShowCuryDetail = Filter.Current.ShowCuryDetail;
				graph.Filter.Update(filter);
				graph.Filter.Select(); // to calculate totals
				throw new PXRedirectRequiredException(graph, "Account Details");
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.ViewAccountBySub)]
		[PXButton]
		protected virtual IEnumerable AccountBySub(PXAdapter adapter)
		{
			if (EnqResult.Current != null)
			{
				AccountHistoryBySubEnq graph = CreateInstance<AccountHistoryBySubEnq>();
				GLHistoryEnqFilter filter = PXCache<GLHistoryEnqFilter>.CreateCopy(graph.Filter.Current);
				filter.AccountID = Filter.Current.AccountID;
				filter.LedgerID = Filter.Current.LedgerID;
				filter.BranchID = Filter.Current.BranchID;
				filter.SubCD = Filter.Current.SubCD;
				filter.FinPeriodID = EnqResult.Current.LastActivityPeriod;
				filter.ShowCuryDetail = Filter.Current.ShowCuryDetail;
				graph.Filter.Update(filter);
				throw new PXRedirectRequiredException(graph, "Account by Subaccount");
			}
			return adapter.Get();
		}
		#endregion

		protected virtual IEnumerable enqResult()
		{
			AccountByYearFilter filter = CurrentFilter;
			bool showCurrency = filter.ShowCuryDetail.HasValue && filter.ShowCuryDetail.Value;

			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.curyID>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.curyPtdCreditTotal>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.curyPtdDebitTotal>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.curyBegBalance>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.curyEndBalance>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.signCuryBegBalance>(EnqResult.Cache, null, showCurrency);
			PXUIFieldAttribute.SetVisible<GLHistoryEnquiryResult.signCuryEndBalance>(EnqResult.Cache, null, showCurrency);

			if (filter.AccountID == null || filter.LedgerID == null || filter.FinYear == null) yield break; //Prevent code from accessing database;

			PXSelectBase<GLHistoryByPeriod> cmd = new PXSelectJoinGroupBy<GLHistoryByPeriod,
				InnerJoin<Account, On<GLHistoryByPeriod.accountID, Equal<Account.accountID>, 
					And<Match<Account, Current<AccessInfo.userName>>>>,
				InnerJoin<FinPeriod, On<GLHistoryByPeriod.finPeriodID, Equal<FinPeriod.finPeriodID>>,
				InnerJoin<Sub, On<GLHistoryByPeriod.subID, Equal<Sub.subID>, 
					And<Match<Sub, Current<AccessInfo.userName>>>>,
				LeftJoin<GLHistory, On<GLHistoryByPeriod.accountID, Equal<GLHistory.accountID>,
					And<GLHistoryByPeriod.subID, Equal<GLHistory.subID>,
					And<GLHistoryByPeriod.branchID, Equal<GLHistory.branchID>,
					And<GLHistoryByPeriod.ledgerID, Equal<GLHistory.ledgerID>,
					And<GLHistoryByPeriod.finPeriodID, Equal<GLHistory.finPeriodID>>>>>>,
				LeftJoin<AH, On<GLHistoryByPeriod.ledgerID, Equal<AH.ledgerID>,
					And<GLHistoryByPeriod.branchID, Equal<AH.branchID>,
					And<GLHistoryByPeriod.accountID, Equal<AH.accountID>,
					And<GLHistoryByPeriod.subID, Equal<AH.subID>,
					And<GLHistoryByPeriod.lastActivityPeriod, Equal<AH.finPeriodID>>>>>>>>>>>,
				Where<GLHistoryByPeriod.ledgerID, Equal<Current<AccountByYearFilter.ledgerID>>,
					And<FinPeriod.finYear, Equal<Current<AccountByYearFilter.finYear>>,
					And<GLHistoryByPeriod.accountID, Equal<Current<AccountByYearFilter.accountID>>,
					And<Where2<
						Where<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>, And<Where<Account.type, Equal<AccountType.asset>,
							Or<Account.type, Equal<AccountType.liability>>>>>,
						Or<Where<GLHistoryByPeriod.lastActivityPeriod, GreaterEqual<Required<GLHistoryByPeriod.lastActivityPeriod>>,
							And<Where<Account.type, Equal<AccountType.expense>,
							Or<Account.type, Equal<AccountType.income>,
							Or<Account.accountID, Equal<Current<GLSetup.ytdNetIncAccountID>>>>>>>>>>>>>,
				Aggregate<Sum<AH.finYtdBalance,
					Sum<AH.curyFinYtdBalance,
					Sum<GLHistory.finPtdDebit,
					Sum<GLHistory.finPtdCredit,
					Sum<GLHistory.finBegBalance,
					Sum<GLHistory.finYtdBalance,
					Sum<GLHistory.curyFinBegBalance,
					Sum<GLHistory.curyFinYtdBalance,
					Sum<GLHistory.curyFinPtdCredit,
					Sum<GLHistory.curyFinPtdDebit,
					GroupBy<GLHistoryByPeriod.ledgerID,
					GroupBy<GLHistoryByPeriod.accountID,
					GroupBy<GLHistoryByPeriod.finPeriodID>>>>>>>>>>>>>>>(this);

			if (filter.SubID != null)
			{
				cmd.WhereAnd<Where<GLHistoryByPeriod.subID, Equal<Current<AccountByYearFilter.subID>>>>();
			}
			if (filter.BranchID != null)
			{
				cmd.WhereAnd<Where<GLHistoryByPeriod.branchID, Equal<Current<AccountByYearFilter.branchID>>>>();
			}
			if (!SubCDUtils.IsSubCDEmpty(filter.SubCD))
			{
				cmd.WhereAnd<Where<Sub.subCD, Like<Current<AccountByYearFilter.subCDWildcard>>>>();
			}

			foreach (PXResult<GLHistoryByPeriod, Account, FinPeriod, Sub, GLHistory, AH> it in cmd.Select(filter.BegFinPeriod))
			{
				GLHistoryByPeriod baseview = it;
				Account acct = it;
				GLHistory ah = it;
				AH ah1 = it;

				GLHistoryEnquiryResult item = new GLHistoryEnquiryResult
				{
					AccountID = baseview.AccountID,
					AccountCD = acct.AccountCD,
					LedgerID = baseview.LedgerID,
					LastActivityPeriod = baseview.FinPeriodID,
					PtdCreditTotal = ah.FinPtdCredit,
					PtdDebitTotal = ah.FinPtdDebit,
					CuryID = ah1.CuryID,
					Type = acct.Type,
					EndBalance = ah1.FinYtdBalance
				};
				if (!string.IsNullOrEmpty(ah1.CuryID))
				{
					item.CuryEndBalance = ah1.CuryFinYtdBalance; 
					item.CuryPtdCreditTotal = ah.CuryFinPtdCredit;
					item.CuryPtdDebitTotal = ah.CuryFinPtdDebit;
				}
				else
				{
					item.CuryEndBalance = null;
					item.CuryPtdCreditTotal = null;
					item.CuryPtdDebitTotal = null;
				}
				item.recalculate(true); // End balance is considered as correct digit - so we need to calculate begBalance base on ending one
				item.recalculateSignAmount(glsetup.Current?.TrialBalanceSign == GLSetup.trialBalanceSign.Reversed);
				yield return item;
			}
		}
		public override bool IsDirty => false;

		#region Events
		protected virtual void AccountByYearFilter_AccountID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (e.NewValue != null && !(e.NewValue is string))
			{
				Account acct = PXSelect<Account>.Search<Account.accountID>(this, e.NewValue);
				if (acct != null)
				{
					e.NewValue = acct.AccountCD;
				}
			}
		}
		protected virtual void AccountByYearFilter_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			AccountByYearFilter filter = e.Row as AccountByYearFilter;
			if (filter != null)
			{
				if (!string.IsNullOrEmpty(filter.FinPeriodID))
				{
					filter.FinYear = FiscalPeriodUtils.FiscalYear(filter.FinPeriodID); //Fill year from finPeriodID
				}

				if (string.IsNullOrEmpty(filter.FinYear))
				{
					DateTime businessDate = Accessinfo.BusinessDate.Value;
					filter.FinYear = businessDate.Year.ToString("0000");
				}
			}
		}
		protected virtual void AccountByYearFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			EnqResult.Select();
		}
		protected virtual void AccountByYearFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			GLHistoryEnqFilter row = (GLHistoryEnqFilter)e.Row;
			if (row?.AccountID != null)
			{
				Account acctDef = AccountInfo.Current == null || row.AccountID != AccountInfo.Current.AccountID ? AccountInfo.Select() : AccountInfo.Current;
				bool isDenominated = !string.IsNullOrEmpty(acctDef.CuryID);
				PXUIFieldAttribute.SetEnabled<AccountByYearFilter.showCuryDetail>(cache, e.Row, isDenominated);
				if(!isDenominated)
				{
					row.ShowCuryDetail = false;
				}
			}
		}
		protected virtual void AccountByYearFilter_SubCD_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		protected virtual void AccountByYearFilter_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<AccountByYearFilter.ledgerID>(e.Row);
		}
		#endregion
	}
}
