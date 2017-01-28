using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using PX.SM;

namespace PX.Objects.GL
{
	public class GLBudgetEntry : PXGraph<GLBudgetEntry>, PXImportAttribute.IPXPrepareItems
	{
		#region Selects

		public PXFilter<BudgetFilter> Filter;
		public PXFilter<BudgetDistributeFilter> DistrFilter;
		public PXFilter<BudgetPreloadFilter> PreloadFilter;
		public PXFilter<ManageBudgetDialog> ManageDialog;

		[PXImport(typeof(BudgetFilter))]
		public PXSelect<GLBudgetLine,
			Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
				And<GLBudgetLine.subID, Like<Current<BudgetFilter.subCDWildcard>>>>>>,
				OrderBy<Asc<GLBudgetLine.sortOrder>>> BudgetArticles;

		public PXSelect<GLBudgetLineDetail,
			Where<GLBudgetLineDetail.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
			And<GLBudgetLineDetail.branchID, Equal<Current<BudgetFilter.branchID>>,
			And<GLBudgetLineDetail.finYear, Equal<Optional<BudgetFilter.finYear>>,
			And<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLineDetail.groupID>>>>>>> Allocations;

		public PXSelect<GLBudgetLine, Where<GLBudgetLine.parentGroupID, Equal<Argument<Guid?>>>, 
			OrderBy<Asc<GLBudgetLine.treeSortOrder>>> Tree;

		public PXSelect<Neighbour> Neighbour;

		bool SubEnabled = PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.subAccount>();

		#endregion

		#region Ctor

		private readonly int _Periods;

		private readonly int _PeriodsInCurrentYear;

		public PXSetup<GLSetup> GLSetup;

		public GLBudgetEntry()
		{			
			GLSetup setup = GLSetup.Current;

			FinYear _Year = PXSelect<FinYear>.Select(this);
			if (_Year != null)
			{
				_Periods = (int)_Year.FinPeriods;
			}
			FinYear currentYear = PXSelect<FinYear, Where<FinYear.year, Equal<Required<FinYear.year>>>>.Select(this, Filter.Current.FinYear);
			if (currentYear != null)
			{
				_PeriodsInCurrentYear = (int)currentYear.FinPeriods;
			}
			else
			{
				_PeriodsInCurrentYear = _Periods;
			}

			for (int i = 1; i <= _Periods; i++)
			{
				int j = i;
				string fieldName = "Period" + i;
				BudgetArticles.Cache.Fields.Add(fieldName);
				FieldSelecting.AddHandler(typeof(GLBudgetLine), fieldName, (sender, e) => AllocationFieldSelecting(sender, e, j));
				FieldUpdating.AddHandler(typeof(GLBudgetLine), fieldName, (sender, e) => AllocationFieldUpdating(sender, e, j));
			}
		}

		#endregion

		private SelectedGroup CurrentSelected
		{
			get
			{
				PXCache cache = this.Caches[typeof(SelectedGroup)];
				if (cache.Current == null)
				{
					cache.Insert();
					cache.IsDirty = false;
				}
				return (SelectedGroup)cache.Current;
			}
		}

		protected virtual IEnumerable tree(
			[PXGuid]
			Guid? GroupID
		)
		{
			if (GroupID == null)
			{
				yield return new GLBudgetLine()
				{
					GroupID = Guid.Empty,
					Description = PXSiteMap.RootNode.Title
				};

			}

			foreach (GLBudgetLine article in PXSelect<GLBudgetLine,
			 Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>,
				And<GLBudgetLine.isGroup, Equal<True>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>>.Select(this, GroupID))
			{
				if (SearchForMatchingGroupMask(article.GroupID))
				{
					if (Filter.Current.TreeNodeFilter != null)
					{
						if (article.Description.Contains(Filter.Current.TreeNodeFilter))
						{
							yield return article;
						}
						else
						{
							if (SearchForMatchingChild(article.GroupID))
							{
								yield return article;
							}
							else if (SearchForMatchingParent(article.ParentGroupID))
							{
								yield return article;
							}
						}
					}
					else
					{
						yield return article;
					}
				}
			}
		}

		protected virtual IEnumerable budgetarticles(
			[PXGuid]
			Guid? groupID
		)
		{
			if (groupID == null)
				groupID = this.Tree.Current != null
								? this.Tree.Current.GroupID
								: Guid.Empty;

			this.CurrentSelected.Group = (this.Filter.Current.ShowTree ?? true) ? groupID : Guid.Empty;

			GLBudgetLine parentNode = PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>>>.Select(this, this.CurrentSelected.Group);
			if (parentNode != null)
			{
				this.CurrentSelected.AccountID = parentNode.AccountID != null ? parentNode.AccountID : Int32.MinValue;
				this.CurrentSelected.SubID = parentNode.SubID != null ? parentNode.SubID : Int32.MinValue;
			}
			else
			{
				this.CurrentSelected.AccountID = Int32.MinValue;
				this.CurrentSelected.SubID = Int32.MinValue;
			}
			this.CurrentSelected.AccountMaskWildcard = SubCDUtils.CreateSubCDWildcard(parentNode != null ? parentNode.AccountMask : string.Empty, AccountAttribute.DimensionName);
			this.CurrentSelected.AccountMask = parentNode != null ? parentNode.AccountMask : string.Empty;
			this.CurrentSelected.SubMaskWildcard = SubCDUtils.CreateSubCDWildcard(parentNode != null ? parentNode.SubMask : string.Empty, SubAccountAttribute.DimensionName);
			this.CurrentSelected.SubMask = parentNode != null ? parentNode.SubMask : string.Empty;

            BudgetArticles.Cache.AllowInsert = Filter.Current != null && Filter.Current.BranchID != null && Filter.Current.LedgerID != null && Filter.Current.FinYear != null
                && (Filter.Current.CompareToBranchID == null || Filter.Current.CompareToFinYear == null || Filter.Current.CompareToLedgerId == null);
            BudgetArticles.Cache.AllowDelete = Filter.Current != null && Filter.Current.BranchID != null && Filter.Current.LedgerID != null && Filter.Current.FinYear != null
                && (Filter.Current.CompareToBranchID == null || Filter.Current.CompareToFinYear == null || Filter.Current.CompareToLedgerId == null);

			List<GLBudgetLine> Articles = new List<GLBudgetLine>();
			int SortOrder = 0;
			//Adding logical groups to the grid
			if ((bool)Filter.Current.ShowTree)
			{
				foreach (GLBudgetLine logicalGroup in PXSelect<GLBudgetLine,
						Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
							And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
							And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
							And<GLBudgetLine.accountID, IsNull,
							And<Match<Current<AccessInfo.userName>>>>>>>, 
							OrderBy<Asc<GLBudgetLine.treeSortOrder>>>.Select(this))
				{
					if (logicalGroup.ParentGroupID == groupID)
					{
						logicalGroup.SortOrder = SortOrder++;
						Articles.Add(logicalGroup);
						if (Filter.Current.CompareToBranchID != null && Filter.Current.CompareToLedgerId != null && !string.IsNullOrEmpty(Filter.Current.CompareToFinYear))
						{
							Articles.Add(ComparisonRow(logicalGroup, SortOrder++));
						}
					}
				}
			}

			foreach (PXResult<GLBudgetLine, Account, Sub> result in PXSelectJoin<GLBudgetLine,
					InnerJoin<Account, On<Account.accountID, Equal<GLBudgetLine.accountID>>,
					LeftJoin<Sub, On<Sub.subID, Equal<GLBudgetLine.subID>>>>,
					Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
						And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
						And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
						And<Account.active, Equal<True>,
						And2<Where<Sub.active, Equal<True>,
                        And<Sub.subCD, Like<Current<BudgetFilter.subCDWildcard>>, Or<Sub.subID, IsNull>>>,
						And<Match<Current<AccessInfo.userName>>>>>>>>,
							OrderBy<Asc<GLBudgetLine.accountID,
							Asc<GLBudgetLine.subID>>>>.Select(this))
			{
				GLBudgetLine article = result;

				if (article.Comparison != null && article.Comparison == true) break;

				if ((bool)Filter.Current.ShowTree)
				{
					if ((article.GroupID == null || article.ParentGroupID == groupID) || (article.ParentGroupID == groupID && groupID == Guid.Empty)) //|| article.GroupID == groupID 
					{
						article.SortOrder = SortOrder++;
						Articles.Add(article);
						if (Filter.Current.CompareToBranchID != null && Filter.Current.CompareToLedgerId != null && !string.IsNullOrEmpty(Filter.Current.CompareToFinYear))
						{
							Articles.Add(ComparisonRow(article, SortOrder++));
						}
					}
				}
				else
				{
					if (article.Rollup == null || !(bool)article.Rollup)
					{
						article.SortOrder = SortOrder++;
						Articles.Add(article);
						if (Filter.Current.CompareToBranchID != null && Filter.Current.CompareToLedgerId != null && !string.IsNullOrEmpty(Filter.Current.CompareToFinYear))
						{
							Articles.Add(ComparisonRow(article, SortOrder++));
						}
					}
				}
			}
			return Articles;
		}

		#region Functions

		private bool SearchForMatchingGroupMask(Guid? GroupID)
		{
			PXResultset<GLBudgetLine> childGroups = PXSelect<GLBudgetLine, Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>, And<GLBudgetLine.isGroup, Equal<True>>>>.Select(this, GroupID);
			if (PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>, And<Match<Current<AccessInfo.userName>>>>>.Select(this, GroupID).Count == 0)
			{
				if (PXSelect<GLBudgetLine, Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>, And<Match<Current<AccessInfo.userName>>>>>.Select(this, GroupID).Count == 0)
				{
					foreach (GLBudgetLine childGroup in childGroups)
					{
						if (SearchForMatchingGroupMask(childGroup.GroupID)) return true;
					}
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
			return false;
		}


		private bool SearchForMatchingChild(Guid? GroupID)
		{
			PXResultset<GLBudgetLine> childGroups = PXSelect<GLBudgetLine, Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>, And<GLBudgetLine.isGroup, Equal<True>>>>.Select(this, GroupID);
			foreach (GLBudgetLine childGroup in childGroups)
			{
				if (!childGroup.Description.Contains(Filter.Current.TreeNodeFilter))
				{
					if (SearchForMatchingChild(childGroup.GroupID)) return true;
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		private bool SearchForMatchingParent(Guid? ParentGroupID)
		{
			GLBudgetLine parentGroup = PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>>>.Select(this, ParentGroupID);
			if (parentGroup != null)
			{
				if (!parentGroup.Description.Contains(Filter.Current.TreeNodeFilter))
				{
					if (SearchForMatchingParent(parentGroup.ParentGroupID)) return true;
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool MatchMask(string accountCD, string mask)
		{
			if (mask.Length == 0 && accountCD.Length > 0)
			{
				for (int i = 0; i == accountCD.Length; i++)
				{
					mask += "?";
				}
			}
			if (mask.Length > 0 && accountCD.Length > 0 && mask.Length > accountCD.Length)
			{
				mask = mask.Substring(0, accountCD.Length);
			}
			for (int i = 0; i < mask.Length; i++)
			{
				if (i >= accountCD.Length || mask[i] != '?' && mask[i] != accountCD[i])
				{
					return false;
				}
			}
			return true;
		}

		private GLBudgetLine ComparisonRow(GLBudgetLine article, int sortOrder)
		{
			GLBudgetLine compare = new GLBudgetLine
			{
				IsGroup = false,
				Released = false,
				BranchID = Filter.Current.CompareToBranchID,
				LedgerID = Filter.Current.CompareToLedgerId,
				FinYear = Filter.Current.CompareToFinYear,
				AccountID = article.AccountID,
				SubID = article.SubID,

				GroupID = article.GroupID,
				ParentGroupID = article.ParentGroupID,

				Description = Messages.BudgetArticleDescrCompared,
				Rollup = article.Rollup,
				Comparison = true,
				Compared = article.Compared,
				Amount = 0m,
				SortOrder = sortOrder
			};
			PXUIFieldAttribute.SetEnabled(BudgetArticles.Cache, compare, false);
			if (article.Compared != null)
			{
				foreach (decimal t in article.Compared)
				{
					compare.Amount += t;
				}
			}
			//All comparison values are taken from GLHistory
			compare.AllocatedAmount = compare.Amount;
			compare.ReleasedAmount = compare.Amount;
			if (BudgetArticles.Cache.GetStatus(compare) == PXEntryStatus.Notchanged) 
				BudgetArticles.Cache.SetStatus(compare, PXEntryStatus.Held);
			return compare;
		}

		protected virtual void UpdateAlloc(decimal value, GLBudgetLine article, int fieldNbr)
		{
			GLBudgetLineDetail alloc = GetAlloc(article, fieldNbr);
			decimal? delta;
			if (alloc != null)
			{
				delta = value - alloc.Amount;
				if (delta != 0m)
				{
					alloc.Amount = value;
					Allocations.Update(alloc);
					article.Allocated = null;
				}
			}
			else
			{
				delta = value;
				if (delta != 0m)
				{
					FinPeriod period = PXSelect<FinPeriod,
						Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>,
						And<FinPeriod.periodNbr, Equal<Required<FinPeriod.periodNbr>>>>>
						.Select(this, article.FinYear, fieldNbr.ToString("00"));
					if (period != null)
					{
						alloc = new GLBudgetLineDetail
						{
							GroupID = article.GroupID,
							BranchID = article.BranchID,
							LedgerID = article.LedgerID,
							FinYear = article.FinYear,
							AccountID = article.AccountID,
							SubID = article.SubID,
							FinPeriodID = period.FinPeriodID,
							Amount = value
						};
						Allocations.Insert(alloc);
						article.Allocated = null;
					}
				}
			}
			RollupAllocation(article, fieldNbr, delta);
		}

		protected virtual GLBudgetLineDetail GetAlloc(GLBudgetLine article, int fieldNbr)
		{
			string period = article.FinYear + fieldNbr.ToString("00");
			foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
			{
				if (alloc.FinPeriodID == period)
				{
					return alloc;
				}
			}
			return null;
		}

		protected virtual void EnsureAlloc(GLBudgetLine article)
		{
			if (article.Allocated != null) return;

			article.Allocated = new decimal[_Periods];
			foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
			{
				int idx = int.Parse(alloc.FinPeriodID.Substring(4)) - 1;
				article.Allocated[idx] = alloc.Amount ?? 0m;
			}
		}

		protected bool suppressIDs;
		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
		{
			IEnumerable ret = base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
			suppressIDs = viewName == "BudgetArticles";
			return ret;
		}

		protected virtual GLBudgetLine GetPrevArticle(GLBudgetLine article)
		{
			return article.FinYear != null ? PXSelect<GLBudgetLine,
				Where<GLBudgetLine.branchID, Equal<Current<GLBudgetLine.branchID>>,
					And<GLBudgetLine.ledgerID, Equal<Current<GLBudgetLine.ledgerID>>,
					And<GLBudgetLine.accountID, Equal<Current<GLBudgetLine.accountID>>,
					And<GLBudgetLine.subID, Equal<Current<GLBudgetLine.subID>>,
					And<GLBudgetLine.finYear, Equal<Required<GLBudgetLine.finYear>>>>>>>>.SelectSingleBound(this, 
					new object[] { article }, (int.Parse(article.FinYear) - 1).ToString(CultureInfo.InvariantCulture)) : null;
		}

		protected virtual void PopulateComparison(int? branchID, int? ledgerID, string finYear)
		{
			foreach (GLBudgetLine article in BudgetArticles.Cache.Cached)
			{
				article.Compared = new decimal[_Periods];
			}
			if (branchID == null || ledgerID == null || String.IsNullOrEmpty(finYear)) return;

			Ledger ledger = PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<BudgetFilter.compareToLedgerID>>>>.Select(this, ledgerID);

			bool BudgetArticlesCacheStatus = BudgetArticles.Cache.IsDirty;

			foreach (PXResult<GLBudgetLine, FinPeriod, Account, GLHistory> result in PXSelectJoin<GLBudgetLine,
				InnerJoin<FinPeriod, On<True, Equal<True>>,
				InnerJoin<Account, On<Account.accountID, Equal<GLBudgetLine.accountID>>,
				LeftJoin<GLHistory, On<GLHistory.accountID, Equal<GLBudgetLine.accountID>,
					And<GLHistory.subID, Equal<GLBudgetLine.subID>,
					And<GLHistory.finPeriodID, Equal<FinPeriod.finPeriodID>,
					And<GLHistory.ledgerID, Equal<Required<BudgetFilter.compareToLedgerID>>,
					And<GLHistory.branchID, Equal<Required<BudgetFilter.compareToBranchID>>>>>>>>>>,
				Where<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
					And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					And<FinPeriod.finYear, Equal<Required<BudgetFilter.compareToFinYear>>,
					And<Match<Current<AccessInfo.userName>>>>>>>,
				OrderBy<Asc<GLBudgetLine.accountID,
					Asc<GLBudgetLine.subID,
					Asc<FinPeriod.finPeriodID>>>>>.Select(this, ledgerID, branchID, finYear))
			{
				GLBudgetLine article = result;
				GLHistory hist = result;
				Account acct = result;
				FinPeriod period = result;

				if (int.Parse(period.FinPeriodID.Substring(4)) > _Periods)
				{
					break;
				}

				decimal histExpense = (hist.CuryFinPtdDebit ?? 0m) - (hist.CuryFinPtdCredit ?? 0m);
				decimal histIncome = (hist.CuryFinPtdCredit ?? 0m) - (hist.CuryFinPtdDebit ?? 0m);

				//Collecting amounts for aggregating articles
				if (article.AccountID != null && article.SubID != null && (article.AccountMask.Contains('?') || article.SubMask.Contains('?')) && ledger.BalanceType != LedgerBalanceType.Budget)
				{
					histExpense = 0;
					histIncome = 0;
					foreach (PXResult<GLHistory, Account, Sub, FinPeriod> aggregatingResult in PXSelectJoin<GLHistory,
					LeftJoin<Account, On<GLHistory.accountID, Equal<Account.accountID>>,
					LeftJoin<Sub, On<GLHistory.subID, Equal<Sub.subID>>,
					InnerJoin<FinPeriod, On<True, Equal<True>,
					And<GLHistory.finPeriodID, Equal<FinPeriod.finPeriodID>,
					And<GLHistory.ledgerID, Equal<Required<BudgetFilter.compareToLedgerID>>,
					And<GLHistory.branchID, Equal<Required<BudgetFilter.compareToBranchID>>>>>>>>>,
						Where<FinPeriod.finYear, Equal<Required<BudgetFilter.compareToFinYear>>,
						And<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>,
							And<Account.accountCD, Like<Required<Account.accountCD>>,
								And<Sub.subCD, Like<Required<Sub.subCD>>>>>>>.Select(this, 
								ledgerID, branchID, finYear, period.FinPeriodID, 
								SubCDUtils.CreateSubCDWildcard(article.AccountMask, AccountAttribute.DimensionName),
								SubCDUtils.CreateSubCDWildcard(article.SubMask, SubAccountAttribute.DimensionName)))
					{
						GLHistory histAggr = aggregatingResult;
						histExpense += (histAggr.CuryFinPtdDebit ?? 0m) - (histAggr.CuryFinPtdCredit ?? 0m);
						histIncome += (histAggr.CuryFinPtdCredit ?? 0m) - (histAggr.CuryFinPtdDebit ?? 0m);
					}
				}

				if (article.Compared == null)
				{
					article.Compared = new decimal[_Periods];
				}
				if (acct.Type == AccountType.Asset || acct.Type == AccountType.Expense)
				{
					article.Compared[int.Parse(period.FinPeriodID.Substring(4)) - 1] = histExpense;
					RollupComparison(article, int.Parse(period.FinPeriodID.Substring(4)) - 1, histExpense);
				}
				else
				{
					article.Compared[int.Parse(period.FinPeriodID.Substring(4)) - 1] = histIncome;
					RollupComparison(article, int.Parse(period.FinPeriodID.Substring(4)) - 1, histIncome);
				}
			}
			if (!BudgetArticlesCacheStatus)
			{
				BudgetArticles.Cache.IsDirty = false;
			}
		}

		private void RollupComparison(GLBudgetLine row, int period, decimal? delta)
		{
			GLBudgetLine parentNode = PXSelect<GLBudgetLine,
				Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>.Select(this,
				row.ParentGroupID);
			if (parentNode != null)
			{
				if (parentNode.Compared == null)
				{
					parentNode.Compared = new decimal[_Periods];
				}
				parentNode.Compared[period] += (decimal)delta;
				BudgetArticles.Update(parentNode);
				RollupComparison(parentNode, period, delta);
			}
		}

		private void RollupArticleAmount(GLBudgetLine row, decimal? delta)
		{
			GLBudgetLine parentNode = PXSelect<GLBudgetLine,
				Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>.Select(this,
				row.ParentGroupID);
			if (parentNode != null)
			{
				parentNode.Amount = parentNode.Amount ?? 0;
				parentNode.Amount += delta;
				BudgetArticles.Update(parentNode);
			}
		}

		protected virtual void RollupAllocation(GLBudgetLine article, int fieldNbr, decimal? delta)
		{
			if (article.ParentGroupID != null && article.ParentGroupID != Guid.Empty && delta != 0m)
			{
				GLBudgetLine rollupArticle = PXSelect<GLBudgetLine,
						Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
						And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
						And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
						And<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>>>>>>
						.Select(this, article.ParentGroupID);
				if (rollupArticle != null)
				{
					GLBudgetLineDetail rollupAlloc = GetAlloc(rollupArticle, fieldNbr);
					if (rollupAlloc != null && rollupAlloc.LedgerID != null)
					{
						rollupAlloc.Amount += delta;
						rollupAlloc = Allocations.Update(rollupAlloc);
					}
					else
					{
						rollupAlloc = new GLBudgetLineDetail();
						rollupAlloc.GroupID = rollupArticle.GroupID;
						rollupAlloc.BranchID = rollupArticle.BranchID;
						rollupAlloc.LedgerID = rollupArticle.LedgerID;
						rollupAlloc.FinYear = rollupArticle.FinYear;
						rollupAlloc.AccountID = rollupArticle.AccountID;
						rollupAlloc.SubID = rollupArticle.SubID;
						rollupAlloc.FinPeriodID = rollupArticle.FinYear + fieldNbr.ToString("00");
						rollupAlloc.Amount = delta;
						rollupAlloc = Allocations.Insert(rollupAlloc);
					}
					rollupArticle.Allocated = null;
					rollupArticle.Released = false;
					RollupAllocation(rollupArticle, fieldNbr, delta);
				}
			}
		}

		private GLBudgetLine PutIntoInnerGroup(GLBudgetLine newArticle, GLBudgetLine parentArticle)
		{
			GLBudgetLine returnArticle = parentArticle;
			foreach (GLBudgetLine child in PXSelect<GLBudgetLine,
				Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>.Select(this, parentArticle.GroupID))
			{
				if (MatchMask(((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, newArticle.AccountID)).AccountCD, child.AccountMask ?? String.Empty)
					&& MatchMask(((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, newArticle.SubID)).SubCD, child.SubMask ?? String.Empty))
				{
					returnArticle = child;
					returnArticle = PutIntoInnerGroup(newArticle, child);
				}
			}
			return returnArticle;
		}

		private GLBudgetLine LocateInBudgetArticlesCache(PXResultset<GLBudgetLine> existingArticles, GLBudgetLine line, bool IncludeGroups)
		{
			foreach (GLBudgetLine article in existingArticles)
			{
				if (BudgetArticles.Cache.GetStatus(article) != PXEntryStatus.Deleted && BudgetArticles.Cache.GetStatus(article) != PXEntryStatus.InsertedDeleted && article.GroupID != line.GroupID)
				{
					if (article.BranchID.Equals(line.BranchID) && article.LedgerID.Equals(line.LedgerID) && article.FinYear.Equals(line.FinYear) && article.AccountID.Equals(line.AccountID) && article.SubID.Equals(line.SubID))
					{
						if (IncludeGroups)
						{
							return article;
						}
						else
						{
							if (!(bool)article.IsGroup)
							{
								return article;
							}
						}
					}
				}
			}
			return null;
		}

		private GLBudgetLineDetail LocateInBudgetAllocationsCache(PXResultset<GLBudgetLineDetail> existingAllocations, GLBudgetLineDetail line)
		{
			foreach (GLBudgetLineDetail allocation in existingAllocations)
			{
				if (Allocations.Cache.GetStatus(allocation) != PXEntryStatus.Deleted && Allocations.Cache.GetStatus(allocation) != PXEntryStatus.InsertedDeleted && allocation.GroupID != line.GroupID)
				{
					if (allocation.BranchID.Equals(line.BranchID) && allocation.LedgerID.Equals(line.LedgerID) && allocation.FinYear.Equals(line.FinYear) && allocation.AccountID.Equals(line.AccountID) && allocation.SubID.Equals(line.SubID) && allocation.FinPeriodID == line.FinPeriodID)
					{
						return allocation;
					}
				}
			}
			return null;
		}

		private void PreloadBudgetTree()
		{
			foreach (Neighbour neighbour in Neighbour.Select())
			{
				if (neighbour.LeftEntityType.Contains("GLBudgetTree") || neighbour.RightEntityType.Contains("GLBudgetTree"))
				{
					neighbour.LeftEntityType = neighbour.LeftEntityType.Replace("GLBudgetTree", "GLBudgetLine");
					neighbour.RightEntityType = neighbour.RightEntityType.Replace("GLBudgetTree", "GLBudgetLine");
					Neighbour.Update(neighbour);
				}
			}
			
			CurrentSelected.Group = Guid.Empty;
			PXResultset<GLBudgetTree> BudgetTree = PXSelect<GLBudgetTree>.Select(this);
			List<GLBudgetLine> articlesToBeInserted = new List<GLBudgetLine>();
			foreach (GLBudgetTree node in BudgetTree)
			{
				if (node.GroupID != node.ParentGroupID)
				{
					if ((node.AccountID != null && node.SubID != null) || ((bool)node.IsGroup && node.AccountID == null))
					{
						GLBudgetLine article = new GLBudgetLine();
						article.Rollup = node.Rollup;
						article.IsGroup = node.IsGroup;
						article.GroupID = node.GroupID;
						article.IsPreloaded = true;
						article.ParentGroupID = node.ParentGroupID;
						article.BranchID = Filter.Current.BranchID;
						article.LedgerID = Filter.Current.LedgerID;
						article.FinYear = Filter.Current.FinYear;
						article.Description = node.Description;
						article.AccountID = node.AccountID;
						article.SubID = node.SubID;
						article.TreeSortOrder = node.SortOrder;

						article.AccountMask = node.AccountMask;
						article.SubMask = node.SubMask;
						article.GroupID = node.GroupID;
						article.ParentGroupID = node.ParentGroupID;

						article.GroupMask = node.GroupMask;
						articlesToBeInserted.Add(article);
					}
				}
			}

			//Search for orphan lines (no parent & GroupID != Guid.Empty)
			List<GLBudgetLine> orphanLines = new List<GLBudgetLine>();
			foreach (GLBudgetLine article in articlesToBeInserted)
			{
				if (article.ParentGroupID != Guid.Empty)
				{
					if (articlesToBeInserted.Where(x => x.GroupID == article.ParentGroupID).Count() == 0)
					{
						article.Rollup = true;
						orphanLines.Add(article);
					}
				}
			}

			//Put articles into groups if possible
			foreach (GLBudgetLine article in orphanLines)
			{
				if (article.AccountID != null && article.SubID != null && !(bool)article.IsGroup)
				{
					article.ParentGroupID = PutIntoNewInnerGroup(Guid.Empty, article, articlesToBeInserted);
				}
			}

			//Generate new Guids
			foreach (GLBudgetLine articleGroup in articlesToBeInserted)
			{
				Guid tempGuid = Guid.NewGuid();

				foreach (GLBudgetLine articleParentGroup in articlesToBeInserted)
				{
					if (articleParentGroup.ParentGroupID == articleGroup.GroupID)
					{
						articleParentGroup.ParentGroupID = tempGuid;
					}
				}
				articleGroup.GroupID = tempGuid;
			}
			foreach (GLBudgetLine article in articlesToBeInserted)
			{
				BudgetArticles.Insert(article);
			}

		}

		private Guid PutIntoNewInnerGroup(Guid groupID, GLBudgetLine article, List<GLBudgetLine> articlesToBeInserted)
		{
			Guid parentGroupID = groupID;
			foreach (GLBudgetLine child in groupID == Guid.Empty ? articlesToBeInserted : articlesToBeInserted.Where(x => x.ParentGroupID == groupID))
			{
				if (article.GroupID != child.GroupID && (bool)child.IsGroup)
				{
					if (child.AccountMask != null && child.SubMask != null && MatchMask(article.AccountMask, child.AccountMask) && MatchMask(article.SubMask, child.SubMask))
					{
						parentGroupID = (Guid)child.GroupID;
						parentGroupID = PutIntoNewInnerGroup(parentGroupID, article, articlesToBeInserted);
					}
				}
			}
			return parentGroupID;
		}

		#endregion

		#region Implementation of IPXPrepareItems

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			Account acct = PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(this, values["AccountID"]);
			Sub sub = PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(this, values["SubID"]);

            CS.Dimension subDimension = PXSelectReadonly<CS.Dimension, Where<CS.Dimension.dimensionID, Equal<Required<CS.Dimension.dimensionID>>>>.Select(this, SubAccountAttribute.DimensionName);
            if (sub == null && subDimension != null && subDimension.Validate == false && SubEnabled)
                sub = new Sub();

			if ((acct != null && sub != null && SubEnabled) || (acct != null && !SubEnabled))
			{
				GLBudgetLine article = new GLBudgetLine();
				if (SubEnabled)
				{
					article = PXSelect<GLBudgetLine,
					Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
						And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
						And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					And<GLBudgetLine.accountID, Equal<Required<GLBudgetLine.accountID>>,
					And<GLBudgetLine.subID, Equal<Required<GLBudgetLine.subID>>>>>>>>.Select(this, acct.AccountID, sub.SubID);
				}
				else
				{
					article = PXSelect<GLBudgetLine,
						Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
							And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
							And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
						And<GLBudgetLine.accountID, Equal<Required<GLBudgetLine.accountID>>>>>>>.Select(this, acct.AccountID);
				}
				if (article != null)
				{
					if ((bool)article.IsGroup == true)
					{
						return false;
					}

					values.Add("IsUploaded", true);
					values.Add("GroupID", article.GroupID);
					values.Add("ParentGroupID", article.ParentGroupID);
					values.Add("GroupMask", article.GroupMask);

					keys["GroupID"] = article.GroupID;
					keys["ParentGroupID"] = article.ParentGroupID;
				}
				else
				{
					Guid groupID = Guid.NewGuid();
					values.Add("GroupID", groupID);
					keys["GroupID"] = groupID;

					values.Add("ParentGroupID", Guid.Empty);
					keys["ParentGroupID"] = Guid.Empty;
					values.Add("IsUploaded", true);

					foreach (GLBudgetLine group in PXSelect<GLBudgetLine,
					Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
					And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					And<GLBudgetLine.isGroup, Equal<True>>>>>>.Select(this))
					{
						if ((SubEnabled && group.AccountMask != null && group.AccountMask != string.Empty && group.SubMask != null && group.SubMask != String.Empty)
							|| (!SubEnabled && group.AccountMask != null && group.AccountMask != string.Empty && group.SubMask != null))
						{
							if ((SubEnabled && MatchMask(values["AccountID"].ToString().Trim(), group.AccountMask) && MatchMask(values["SubID"].ToString().Trim(), group.SubMask))
								|| (!SubEnabled && MatchMask(values["AccountID"].ToString().Trim(), group.AccountMask)))
							{
								GLBudgetLine foundLine = PutInGroup(values["AccountID"].ToString().Trim(), (values["SubID"] ?? String.Empty).ToString().Trim(), group);
								values["ParentGroupID"] = foundLine.ParentGroupID ?? group.GroupID;
								keys["ParentGroupID"] = values["ParentGroupID"];
								if (group.GroupMask != null)
								{
									if (!values.Contains("GroupMask")) values.Add("GroupMask", foundLine.GroupMask ?? group.GroupMask);
									else values["GroupMask"] = foundLine.GroupMask ?? group.GroupMask;
								}
							}
						}
					}
				}
			}
			else
			{
				return false;
			}
			return true;
		}

		public bool RowImporting(string viewName, object row)
		{
			return row == null;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		public virtual void PrepareItems(string viewName, IEnumerable items)
		{
		}

		private GLBudgetLine PutInGroup(string AccountID, string SubID, GLBudgetLine parentArticle)
		{
			GLBudgetLine foundLine = new GLBudgetLine();
			foreach (GLBudgetLine child in PXSelect<GLBudgetLine,
				Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
				And<GLBudgetLine.isGroup, Equal<True>>>>>>>.Select(this, parentArticle.GroupID))
			{
				if (MatchMask(SubID, child.SubMask ?? String.Empty) && MatchMask(AccountID, child.AccountMask ?? String.Empty))
				{
					GLBudgetLine tmpLine = PutInGroup(AccountID, SubID, child);
                    if (tmpLine.GroupID != null || tmpLine.ParentGroupID != null)
					{
						foundLine = tmpLine;
					}
					else
					{
						foundLine.ParentGroupID = (Guid)child.GroupID;
						foundLine.GroupMask = child.GroupMask;
					}
				}
			}
			return foundLine;
		}

		#endregion

		#region Actions
		public PXSave<BudgetFilter> Save;
		public PXCancel<BudgetFilter> Cancel;
		public PXDelete<BudgetFilter> Delete;
		public PXAction<BudgetFilter> First;
		public PXAction<BudgetFilter> Prev;
		public PXAction<BudgetFilter> Next;
		public PXAction<BudgetFilter> WNext;
		public PXAction<BudgetFilter> Last;
		public PXAction<BudgetFilter> ShowPreload;
		public PXAction<BudgetFilter> Preload;
		public PXAction<BudgetFilter> Distribute;
		public PXAction<BudgetFilter> DistributeOK;
		public PXAction<BudgetFilter> ShowManage;
		public PXAction<BudgetFilter> ManageOK;

		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXCancelButton]
		public virtual IEnumerable cancel(PXAdapter adapter)
		{
			BudgetFilter oldfilter = Filter.Current;
			oldfilter.CompareToFinYear = null;
			oldfilter.SubIDFilter = null;
			Clear();
			Filter.Cache.RestoreCopy(Filter.Current, oldfilter);
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.Delete, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXDeleteButton]
		public virtual IEnumerable delete(PXAdapter adapter)
		{
			bool deleteAllowed = true;
			PXResultset<GLBudgetLine> budget = PXSelect<GLBudgetLine,
							Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
							And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
							And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this);
			foreach (GLBudgetLine budgetLine in budget)
			{
				if (budgetLine.ReleasedAmount > 0)
				{
					deleteAllowed = false;
					break;
				}
			}
			if (deleteAllowed)
			{
				foreach (GLBudgetLine budgetLine in budget)
				{
					BudgetArticles.Delete(budgetLine);
				}
				Filter.Current.FinYear = null;
				this.Save.Press();
			}
			else
			{
				BudgetArticles.Ask(Messages.BudgetDeleteTitle, Messages.BudgetDeleteMessage, MessageButtons.OK);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.Previous, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton(ConfirmationMessage = null)]
		public virtual IEnumerable prev(PXAdapter adapter)
		{
			if (!BudgetArticles.Cache.IsDirty)
			{
				GLBudgetLine article = PXSelectGroupBy<GLBudgetLine,
					Where<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					And<GLBudgetLine.finYear, Less<Current<BudgetFilter.finYear>>>>,
					Aggregate<Max<GLBudgetLine.finYear>>>
					.Select(this);
				if (article == null || article.FinYear == null)
				{
					return Last.Press(adapter);
				}
				Filter.Current.FinYear = article.FinYear;
				Filter.Update(Filter.Current);
			}
			else
			{
				BudgetArticles.Ask(Messages.BudgetPendingChangesTitle, Messages.BudgetPendingChangesMessage, MessageButtons.OK);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.Next, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton(ConfirmationMessage = null)]
		public virtual IEnumerable next(PXAdapter adapter)
		{
			if (!BudgetArticles.Cache.IsDirty)
			{
				GLBudgetLine article = PXSelectGroupBy<GLBudgetLine,
					Where<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					And<GLBudgetLine.finYear, Greater<Required<BudgetFilter.finYear>>>>,
					Aggregate<Min<GLBudgetLine.finYear>>>
					.Select(this, Filter.Current.FinYear ?? "");
				if (article == null || article.FinYear == null)
				{
					return First.Press(adapter);
				}
				Filter.Current.FinYear = article.FinYear;
				Filter.Update(Filter.Current);
			}
			else
			{
				BudgetArticles.Ask(Messages.BudgetPendingChangesTitle, Messages.BudgetPendingChangesMessage, MessageButtons.OK);
			}
			return adapter.Get();
		}

		[PXButton]
		[PXUIField(MapEnableRights = PXCacheRights.Update, Visible = false)]
		public virtual IEnumerable wnext(PXAdapter adapter)
		{
			bool errorHappened = false;
			if (PreloadFilter.Current.LedgerID == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.ledgerID>(PreloadFilter.Current, PreloadFilter.Current.LedgerID, 
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}
			if (PreloadFilter.Current.FinYear == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.finYear>(PreloadFilter.Current, PreloadFilter.Current.FinYear, 
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}
			
			if (PreloadFilter.Current.ChangePercent == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.changePercent>(PreloadFilter.Current, PreloadFilter.Current.ChangePercent, 
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}

			if (!MatchMask(PreloadFilter.Current.AccountIDFilter ?? String.Empty, CurrentSelected.AccountMask ?? String.Empty))
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.accountIDFilter>(PreloadFilter.Current, PreloadFilter.Current.AccountIDFilter, 
					new PXSetPropertyException(String.Format(PXMessages.LocalizeNoPrefix(Messages.BudgetAccountNotAllowed), CurrentSelected.AccountMask)));
				errorHappened = true;
			}

			if (!MatchMask(PreloadFilter.Current.SubIDFilter ?? String.Empty, CurrentSelected.SubMask ?? String.Empty))
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.subIDFilter>(PreloadFilter.Current, PreloadFilter.Current.SubIDFilter, 
					new PXSetPropertyException(String.Format(PXMessages.LocalizeNoPrefix(Messages.BudgetSubaccountNotAllowed), CurrentSelected.SubMask)));
				errorHappened = true;
			}

			if (errorHappened)
			{
				return adapter.Get();
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.First, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXFirstButton(ConfirmationMessage = null)]
		public virtual IEnumerable first(PXAdapter adapter)
		{
			if (!BudgetArticles.Cache.IsDirty)
			{
				GLBudgetLine article = PXSelectGroupBy<GLBudgetLine,
					Where<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>>,
					Aggregate<Min<GLBudgetLine.finYear>>>
					.Select(this);
				if (article != null && article.FinYear != null)
				{
					Filter.Current.FinYear = article.FinYear;
					Filter.Update(Filter.Current);
				}
			}
			else
			{
				BudgetArticles.Ask(Messages.BudgetPendingChangesTitle, Messages.BudgetPendingChangesMessage, MessageButtons.OK);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = ActionsMessages.Last, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLastButton(ConfirmationMessage = null)]
		public virtual IEnumerable last(PXAdapter adapter)
		{
			if (!BudgetArticles.Cache.IsDirty)
			{
				GLBudgetLine article = PXSelectGroupBy<GLBudgetLine,
					Where<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>>,
					Aggregate<Max<GLBudgetLine.finYear>>>
					.Select(this);
				if (article != null && article.FinYear != null)
				{
					Filter.Current.FinYear = article.FinYear;
					Filter.Update(Filter.Current);
				}
			}
			else
			{
				BudgetArticles.Ask(Messages.BudgetPendingChangesTitle, Messages.BudgetPendingChangesMessage, MessageButtons.OK);
			}
			return adapter.Get();
		}
		
		[PXUIField(DisplayName = Messages.Distribute, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void distribute()
		{
			DistrFilter.AskExt();
		}

		[PXUIField(DisplayName = "Load", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void distributeOK()
		{
			BudgetDistributeFilter filter = DistrFilter.Current;

			PXResultset<GLBudgetLine> articlesToDistribute;

			FinYear currentYear = PXSelect<FinYear, Where<FinYear.year, Equal<Required<FinYear.year>>>>.Select(this, Filter.Current.FinYear);

			int periodsToDistribute = _Periods > (int)currentYear.FinPeriods ? (int)currentYear.FinPeriods : _Periods;
			int periodsTotal = periodsToDistribute;
			bool hasAdjustmentPeriod = false;
			PXResultset<FinPeriod> finPeriods = PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>>>.Select(this, Filter.Current.FinYear);
			foreach (FinPeriod finPeriod in finPeriods)
			{
				if (finPeriod.EndDate == finPeriod.StartDate && Int32.Parse(finPeriod.PeriodNbr) == periodsToDistribute)
				{
					periodsToDistribute--;
					hasAdjustmentPeriod = true;
					break;
				}
			}

			if (filter.ApplyToAll != null && filter.ApplyToAll.Value == true)
			{
				PXResultset<GLBudgetLine> articlesInGroup = PXSelect<GLBudgetLine,
				Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
				And<GLBudgetLine.isGroup, Equal<False>,
				And<Match<Current<AccessInfo.userName>>>>>>>>>.Select(this, BudgetArticles.Current.ParentGroupID);
				articlesToDistribute = articlesInGroup;
				if (filter.ApplyToSubGroups != null && filter.ApplyToSubGroups.Value == true)
				{
					PXResultset<GLBudgetLine> allChilds = collectChildNodes(BudgetArticles.Current.ParentGroupID);
					articlesToDistribute = allChilds;
				}
			}
			else
			{
				articlesToDistribute = PXSelect<GLBudgetLine,
				Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>,
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
				And<GLBudgetLine.isGroup, Equal<False>>>>>>>.Select(this, BudgetArticles.Current.GroupID); ;
			}

			foreach (GLBudgetLine article in articlesToDistribute)
			{
				if (!(bool)article.IsGroup)
				{
					decimal amt = article.Amount ?? 0m;
					int prec = Filter.Current.Precision ?? 2;
					decimal minunit = (decimal)(1 / Math.Pow(10, (prec)));

					switch (filter.Method)
					{
						case BudgetDistributeFilter.method.Evenly:
							decimal periodAmt = (decimal)Math.Round((double)amt / periodsToDistribute, prec, MidpointRounding.AwayFromZero);
							decimal delta = amt - periodAmt * periodsToDistribute;
							if (delta < 0m)
							{
								periodAmt -= minunit;
								delta = amt - periodAmt * periodsToDistribute;
							}
							for (int i = 0; i < periodsTotal; i++)
							{
								decimal val = periodAmt;
								if (delta > 0)
								{
									val += minunit;
									delta -= minunit;
								}
								if (hasAdjustmentPeriod && i + 1 == periodsTotal)
								{
									val = 0;
								}
								UpdateAlloc(val, article, i + 1);
							}
							break;
						case BudgetDistributeFilter.method.PreviousYear:
							{
								GLBudgetLine prev = GetPrevArticle(article);
								if (prev == null) break;

								decimal allocated = 0m;
								int max_idx = 0;
								prev.Allocated = null;
								EnsureAlloc(prev);
								for (int i = 0; i < periodsTotal; i++)
								{
									decimal val = (decimal)Math.Round((double)(article.Amount * prev.Allocated[i] / prev.AllocatedAmount), prec, MidpointRounding.AwayFromZero);
									allocated += val;
									if (prev.Allocated[i] > prev.Allocated[max_idx]) max_idx = i;
									UpdateAlloc(val, article, i + 1);
								}
								GLBudgetLineDetail alloc = GetAlloc(article, max_idx + 1);
								if (alloc != null)
								UpdateAlloc((alloc.Amount ?? 0m) + amt - allocated, article, max_idx + 1);
							}
							break;
						case BudgetDistributeFilter.method.ComparedValues:
							{
								decimal allocated = 0m;
								int max_idx = 0;
								decimal compared = article.Compared.Sum();
								if (compared == 0) break;
								for (int i = 0; i < periodsTotal; i++)
								{
									decimal val = (decimal)Math.Round((double)(article.Amount * article.Compared[i] / compared), prec, MidpointRounding.AwayFromZero);
									allocated += val;
									if (article.Compared[i] > article.Compared[max_idx]) max_idx = i;
									UpdateAlloc(val, article, i + 1);
								}
								GLBudgetLineDetail alloc = GetAlloc(article, max_idx + 1);
								if (alloc != null)
								UpdateAlloc((alloc.Amount ?? 0m) + amt - allocated, article, max_idx + 1);
							}
							break;
					}
				}
			}
		}

		[PXButton]
		[PXUIField(DisplayName = Messages.PreloadArticles, MapEnableRights = PXCacheRights.Update)]
		public virtual IEnumerable showPreload(PXAdapter adapter)
		{
			PreloadFilter.Select();
			GLBudgetLine parentNode = PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>>>.Select(this, this.CurrentSelected.Group);

			PXStringState acctStrState = (PXStringState)PreloadFilter.Cache.GetStateExt(null, typeof(BudgetPreloadFilter.fromAccount).Name);
			string acctWildCard = new String('?', acctStrState.InputMask.Length - 1);
			PXStringState subStrState = (PXStringState)BudgetArticles.Cache.GetStateExt(null, typeof(GLBudgetLine.subID).Name);
			string subWildCard = new String('?', subStrState.InputMask.Length - 1);

			if (parentNode != null)
			{
				PreloadFilter.Current.AccountCDWildcard = SubCDUtils.CreateSubCDWildcard(parentNode != null ? parentNode.AccountMask : string.Empty, AccountAttribute.DimensionName);
				if (parentNode.AccountMask != null)
				{
					Account AcctFrom = PXSelect<Account, Where<Account.active, Equal<True>,
						And<Account.accountCD, Like<Required<SelectedGroup.accountMaskWildcard>>>>,
						OrderBy<Asc<Account.accountCD>>>.SelectWindowed(this, 0, 1, PreloadFilter.Current.AccountCDWildcard);
					Account AcctTo = PXSelect<Account, Where<Account.active, Equal<True>,
						And<Account.accountCD, Like<Required<BudgetPreloadFilter.accountCDWildcard>>>>,
						OrderBy<Desc<Account.accountCD>>>.SelectWindowed(this, 0, 1, PreloadFilter.Current.AccountCDWildcard);
					PreloadFilter.Current.FromAccount = AcctFrom != null ? AcctFrom.AccountID : null;
					PreloadFilter.Current.ToAccount = AcctTo != null ? AcctTo.AccountID : null;
				}
				else
				{
					PreloadFilter.Current.FromAccount = null;
					PreloadFilter.Current.ToAccount = null;
				}
				PreloadFilter.Current.AccountIDFilter = parentNode.AccountMask ?? acctWildCard;
				PreloadFilter.Current.SubIDFilter = parentNode.SubMask ?? subWildCard;
			}
			else
			{
				PreloadFilter.Current.FromAccount = null;
				PreloadFilter.Current.ToAccount = null;
				PreloadFilter.Current.AccountIDFilter = acctWildCard;
				PreloadFilter.Current.SubIDFilter = subWildCard;
			}
			if (PreloadFilter.Current.BranchID == null) PreloadFilter.Current.BranchID = Filter.Current.CompareToBranchID;
			if (PreloadFilter.Current.LedgerID == null) PreloadFilter.Current.LedgerID = Filter.Current.CompareToLedgerId;
			if (PreloadFilter.Current.FinYear == null) PreloadFilter.Current.FinYear = Filter.Current.CompareToFinYear;
			BudgetArticles.AskExt();
			return adapter.Get();
		}

		private PXResultset<GLBudgetLine> collectChildNodes(Guid? GroupID)
		{
			PXResultset<GLBudgetLine> childNodes = new PXResultset<GLBudgetLine>();
			PXResultset<GLBudgetLine> childGroups = PXSelect<GLBudgetLine, 
				Where<GLBudgetLine.parentGroupID, Equal<Required<GLBudgetLine.parentGroupID>>, 
				And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					And<Match<Current<AccessInfo.userName>>>>>>>>.Select(this, GroupID);
			foreach (PXResult<GLBudgetLine> childGroup in childGroups)
			{
				childNodes.Add(childGroup);
				childNodes.AddRange(collectChildNodes(((GLBudgetLine)childGroup).GroupID));
			}
			return childNodes;
		}

		[PXButton]
		[PXUIField(DisplayName = "Manage Budget", MapEnableRights = PXCacheRights.Update)]
		public virtual IEnumerable showManage(PXAdapter adapter)
		{
			ManageDialog.AskExt();
			return adapter.Get();
		}

		 [PXUIField(DisplayName = "ManageAction", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		 [PXButton]
		 public virtual void manageOK()
		 {
			 ManageDialog.Select();
			 ManageBudgetDialog manageDialog = ManageDialog.Current;
			 switch (manageDialog.Method)
			 {
				 case ManageBudgetDialog.method.RollbackBudget:
					 {
						 PXResultset<GLBudgetLine> allArticles = PXSelect<GLBudgetLine,
							Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
							And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
							And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this);
						 PXResultset<GLBudgetLineDetail> allAllocations = PXSelect<GLBudgetLineDetail,
							Where<GLBudgetLineDetail.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
							And<GLBudgetLineDetail.branchID, Equal<Current<BudgetFilter.branchID>>,
							And<GLBudgetLineDetail.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this);

						 foreach (GLBudgetLine article in allArticles)
						 {
							 if (article.ReleasedAmount == 0)
							 {
								 if (!(bool)article.IsGroup && !(bool)article.IsPreloaded && !(bool)article.Rollup)
								 {
									 RollupArticleAmount(article, -article.Amount);
									 foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
									 {
										 RollupAllocation(article, int.Parse(alloc.FinPeriodID.Substring(4)), -alloc.Amount);
									 }
									 BudgetArticles.Delete(article);
								 }
							 }
							 else if (article.ReleasedAmount != article.AllocatedAmount || article.ReleasedAmount != article.Amount)
							 {
								 foreach (GLBudgetLineDetail allocation in allAllocations)
								 {
									 if (allocation.GroupID == article.GroupID)
									 {
										 decimal? delta = allocation.ReleasedAmount - allocation.Amount;
										 allocation.Amount = allocation.ReleasedAmount;
										 Allocations.Update(allocation);
										 RollupAllocation(article, int.Parse(allocation.FinPeriodID.Substring(4)), delta);
									 }
								 }
								 article.Amount = article.ReleasedAmount;
								 article.Released = true;
								 article.Allocated = null;
								 EnsureAlloc(article);
								 BudgetArticles.Update(article);
							 }
							 else if (article.ReleasedAmount == article.AllocatedAmount && article.ReleasedAmount == article.Amount && article.Released != true)
							 {
								 bool released = true;
								 foreach (GLBudgetLineDetail allocation in allAllocations)
								 {
									 if (allocation.GroupID == article.GroupID)
									 {
										 if (allocation.ReleasedAmount != allocation.Amount)
										 {
											 released = false;
										 }
									 }
								 }
								 if (released)
								 {
									 article.Released = true;
									 BudgetArticles.Update(article);
								 }
							 }
						 }
						 BudgetArticles.View.RequestRefresh();
						 break;
					 }
				 case ManageBudgetDialog.method.ConvertBudget:
					 {
						 ConvertBudget();
						 break;
					 }
			 }
		 }

		 private void ConvertBudget()
		 {
			 if (Filter.Ask(Messages.BudgetUpdateTitle, Messages.BudgetUpdateMessage, MessageButtons.OKCancel) == WebDialogResult.OK)
			 {
				 foreach (GLBudgetLine article in PXSelect<GLBudgetLine,
					 Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
					 And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					 And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					 And<GLBudgetLine.released, Equal<False>,
					 And<GLBudgetLine.isGroup, Equal<False>>>>>>>.Select(this))
				 {
					 decimal allocatedAmt = 0;
					 foreach (GLBudgetLineDetail allocation in Allocations.Select(article.FinYear, article.GroupID))
					 {
						 allocatedAmt += (decimal)allocation.Amount;
					 }
					 article.AllocatedAmount = allocatedAmt;
					 BudgetArticles.Update(article);
				 }

				 CurrentSelected.AccountMaskWildcard = "%";
				 CurrentSelected.AccountMask = "";
				 CurrentSelected.SubMask = "";
				 CurrentSelected.SubMaskWildcard = "%";

				 List<GLBudgetLine> oldArticles = new List<GLBudgetLine>();
				 List<GLBudgetLineDetail> oldAllocations = new List<GLBudgetLineDetail>();

				 foreach (GLBudgetLine budgetLine in PXSelect<GLBudgetLine,
				 Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				 And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				 And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this))
				 {
					 if (!(bool)budgetLine.IsGroup || ((bool)budgetLine.IsGroup && budgetLine.AccountID != null && budgetLine.SubID != null))
					 {
						 Guid tempGuid = Guid.NewGuid();
						 foreach (GLBudgetLineDetail budgetLineDetail in Allocations.Select(Filter.Current.FinYear, budgetLine.GroupID))
						 {
							 GLBudgetLineDetail tmpAllocation = Allocations.Cache.CreateCopy(budgetLineDetail) as GLBudgetLineDetail;
							 tmpAllocation.GroupID = tempGuid;
							 oldAllocations.Add(tmpAllocation);
						 }
						 GLBudgetLine tmpArticle = BudgetArticles.Cache.CreateCopy(budgetLine) as GLBudgetLine;
						 tmpArticle.GroupID = tempGuid;
						 oldArticles.Add(tmpArticle);
					 }
				 }

				 //Check for conflicting lines
				 foreach (GLBudgetLine oldArticle in oldArticles)
				 {
					 foreach (GLBudgetTree treeNode in PXSelect<GLBudgetTree>.Select(this))
					 {
						 if ((bool)treeNode.IsGroup && treeNode.AccountID != null && treeNode.SubID != null)
						 {
							 string oldAccountCD = ((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, oldArticle.AccountID)).AccountCD;
							 string newAccountCD = ((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, treeNode.AccountID)).AccountCD;
							 string oldSubCD = ((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, oldArticle.SubID)).SubCD;
							 string newSubCD = ((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, treeNode.SubID)).SubCD;
							 if (MatchMask(oldAccountCD, newAccountCD) && MatchMask(oldSubCD, newSubCD))
							 {
								 throw new PXException(Messages.BudgetUpdateConflictMessage, oldAccountCD, oldSubCD);
							 }
						 }
					 }
				 }

				 foreach (GLBudgetLine budgetLine in PXSelect<GLBudgetLine,
				 Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				 And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				 And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this))
				 {
					 BudgetArticles.Delete(budgetLine);
				 }

				 PreloadBudgetTree();

				 PXResultset<GLBudgetLine> existingGroups = PXSelect<GLBudgetLine,
					 Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
					 And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					 And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this);

				 //Put old articles into new groups
				 foreach (GLBudgetLine oldArticle in oldArticles)
				 {
					 bool groupFound = false;
					 foreach (GLBudgetLine newGroup in existingGroups)
					 {
                         if (newGroup.AccountMask != null && (newGroup.AccountMask != String.Empty || newGroup.SubMask != String.Empty))
						 {
							 if (MatchMask(((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, oldArticle.AccountID)).AccountCD, newGroup.AccountMask ?? String.Empty)
								 && MatchMask(((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, oldArticle.SubID)).SubCD, newGroup.SubMask ?? String.Empty))
							 {
								 groupFound = true;
								 if ((bool)newGroup.IsGroup)
								 {
									 oldArticle.ParentGroupID = newGroup.GroupID;
									 oldArticle.GroupMask = newGroup.GroupMask;
									 oldArticle.ParentGroupID = PutIntoInnerGroup(oldArticle, newGroup).GroupID;
									 if (newGroup.AccountID != null && newGroup.SubID != null)
									 {
										 oldArticle.Rollup = true;
									 }
								 }
							 }
						 }
					 }
					 if (!groupFound)
					 {
						 oldArticle.ParentGroupID = Guid.Empty;
					 }
				 }
				 foreach (GLBudgetLine oldArticle in oldArticles)
				 {
					 GLBudgetLine locatedArticle = LocateInBudgetArticlesCache(existingGroups, oldArticle, true);
					 if (!(bool)oldArticle.IsGroup && locatedArticle == null)
					 {
						 foreach (GLBudgetLineDetail oldAllocation in oldAllocations.Where(x => x.GroupID == oldArticle.GroupID))
						 {
							 Allocations.Insert(oldAllocation);
							 RollupAllocation(oldArticle, int.Parse(oldAllocation.FinPeriodID.Substring(4)), oldAllocation.Amount);
						 }
						 BudgetArticles.Insert(oldArticle);
					 }
					 else if (!(bool)oldArticle.IsGroup && locatedArticle != null)
					 {
						 foreach (GLBudgetLineDetail oldAllocation in oldAllocations.Where(x => x.GroupID == oldArticle.GroupID))
						 {
							 oldAllocation.GroupID = locatedArticle.GroupID;
							 Allocations.Insert(oldAllocation);
							 RollupAllocation(locatedArticle, int.Parse(oldAllocation.FinPeriodID.Substring(4)), oldAllocation.Amount);
						 }
						 locatedArticle.Amount = oldArticle.Amount;
						 locatedArticle.Released = oldArticle.Released;
						 locatedArticle.ReleasedAmount = oldArticle.ReleasedAmount;
						 BudgetArticles.Update(locatedArticle);
					 }
				 }
				 foreach (GLBudgetLine oldArticle in oldArticles)
				 {
					 GLBudgetLine locatedArticle = LocateInBudgetArticlesCache(existingGroups, oldArticle, true);
					 if ((bool)oldArticle.IsGroup && locatedArticle != null)
					 {
						 locatedArticle.Released = oldArticle.Released;
						 BudgetArticles.Update(locatedArticle);
					 }
				 }


			 }
		 }

		[PXButton]
		[PXUIField(MapEnableRights = PXCacheRights.Update, Visible = false)]
		public virtual IEnumerable preload(PXAdapter adapter)
		{
			bool errorHappened = false;
			if (PreloadFilter.Current.LedgerID == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.ledgerID>(PreloadFilter.Current, PreloadFilter.Current.LedgerID, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}
			if (PreloadFilter.Current.FinYear == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.finYear>(PreloadFilter.Current, PreloadFilter.Current.FinYear, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}
			if (PreloadFilter.Current.ChangePercent == null)
			{
				PreloadFilter.Cache.RaiseExceptionHandling<BudgetPreloadFilter.changePercent>(PreloadFilter.Current, PreloadFilter.Current.ChangePercent, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.RowError));
				errorHappened = true;
			}
			if (errorHappened)
			{
				return adapter.Get();
			}
			this.Persist();

			Account AccountFrom = PXSelect<Account, Where<Account.accountID, Equal<Current<BudgetPreloadFilter.fromAccount>>>>.Select(this);
			Account AccountTo = PXSelect<Account, Where<Account.accountID, Equal<Current<BudgetPreloadFilter.toAccount>>>>.Select(this);
			if (AccountFrom == null)
			{
				AccountFrom = PXSelectOrderBy<Account, OrderBy<Asc<Account.accountCD>>>.SelectWindowed(this, 0, 1);
			}
			if (AccountTo == null)
			{
				AccountTo = PXSelectOrderBy<Account, OrderBy<Desc<Account.accountCD>>>.SelectWindowed(this, 0, 1);
			}

			int? Account = null;
			int? Subaccount = null;
			Guid? groupID = null;

			List<GLBudgetLine> articlesFromHistory = new List<GLBudgetLine>();
			List<GLBudgetLineDetail> allocationsFromHistory = new List<GLBudgetLineDetail>();
			foreach (PXResult<GLHistory, FinPeriod, Sub, Account> result in PXSelectJoin<GLHistory,
					InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, Equal<GLHistory.finPeriodID>>,
					InnerJoin<Sub, On<Sub.subID, Equal<GLHistory.subID>>,
					InnerJoin<Account, On<Account.accountID, Equal<GLHistory.accountID>>>>>,
					Where<GLHistory.ledgerID, Equal<Current<BudgetPreloadFilter.ledgerID>>,
						And<GLHistory.branchID, Equal<Current<BudgetPreloadFilter.branchID>>,
						And<FinPeriod.finYear, Equal<Current<BudgetPreloadFilter.finYear>>,
						And<Account.accountCD, GreaterEqual<Required<Account.accountCD>>,
						And<Account.accountCD, LessEqual<Required<Account.accountCD>>,
						And<Account.accountCD, Like<Required<Account.accountCD>>,
						And<Sub.subCD, Like<Current<BudgetPreloadFilter.subCDWildcard>>,
                        And<FinPeriod.active, Equal<True>,
                        And<GLHistory.accountID, NotEqual<Current<GL.GLSetup.ytdNetIncAccountID>>>>>>>>>>>, OrderBy<Asc<Account.accountID>>>.Select(this,
						AccountFrom.AccountCD, AccountTo.AccountCD,
						SubCDUtils.CreateSubCDWildcard(PreloadFilter.Current.AccountIDFilter != null ? PreloadFilter.Current.AccountIDFilter : string.Empty, AccountAttribute.DimensionName)))
			{
				GLHistory history = result;
				Account acct = result;
				FinPeriod period = result;
				if (period.PeriodNbr != null && Int32.Parse(period.PeriodNbr) <= _PeriodsInCurrentYear)
				{
					if (history.AccountID != Account || history.SubID != Subaccount)
					{
						GLBudgetLine newArticle = new GLBudgetLine();
						newArticle.BranchID = Filter.Current.BranchID;
						newArticle.LedgerID = Filter.Current.LedgerID;
						newArticle.FinYear = Filter.Current.FinYear;
						newArticle.AccountID = history.AccountID;
						newArticle.SubID = history.SubID;
						newArticle.Released = false;
						newArticle.ParentGroupID = CurrentSelected.Group ?? Guid.Empty;
						newArticle.GroupID = Guid.NewGuid();
						newArticle.Amount = 0;

						Account = history.AccountID;
						Subaccount = history.SubID;
						groupID = newArticle.GroupID;

						articlesFromHistory.Add(newArticle);
					}
					GLBudgetLineDetail alloc = new GLBudgetLineDetail();
					alloc.GroupID = groupID;
					alloc.BranchID = Filter.Current.BranchID;
					alloc.LedgerID = Filter.Current.LedgerID;
					alloc.FinYear = Filter.Current.FinYear;
					alloc.AccountID = history.AccountID;
					alloc.SubID = history.SubID;
					alloc.FinPeriodID = history.FinPeriodID.Replace(history.FinYear, Filter.Current.FinYear);
					//! Should be account-type dependant. Double check
					if (acct.Type == AccountType.Asset || acct.Type == AccountType.Expense)
					{
						alloc.Amount = Math.Round((decimal)((history.CuryFinPtdDebit - history.CuryFinPtdCredit) * PreloadFilter.Current.ChangePercent / 100), Filter.Current.Precision ?? 2);
					}
					else
					{
						alloc.Amount = Math.Round((decimal)((history.CuryFinPtdCredit - history.CuryFinPtdDebit) * PreloadFilter.Current.ChangePercent / 100), Filter.Current.Precision ?? 2);
					}
					allocationsFromHistory.Add(alloc);
				}
			}

			PXResultset<GLBudgetLine> existingArticles = PXSelect<GLBudgetLine,
				Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
					And<Match<Current<AccessInfo.userName>>>>>>>.Select(this, 0);

            PXResultset<GLBudgetLine> existingHiddenArticles = PXSelect<GLBudgetLine,
                Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
                And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
                And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>,
                    And<Not<Match<Current<AccessInfo.userName>>>>>>>>.Select(this, 0);

			PXResultset<GLBudgetLineDetail> existingAllocations = PXSelect<GLBudgetLineDetail,
				Where<GLBudgetLineDetail.branchID, Equal<Current<BudgetFilter.branchID>>,
				And<GLBudgetLineDetail.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
				And<GLBudgetLineDetail.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this, 0);

			//Collecting child nodes of the selected group
			PXResultset<GLBudgetLine> articlesInCurrentGroup = new PXResultset<GLBudgetLine>();
			if (CurrentSelected.Group != Guid.Empty)
			{
				articlesInCurrentGroup.Add(PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetTree.groupID>>>>.Select(this, CurrentSelected.Group));
			}
			articlesInCurrentGroup.AddRange(collectChildNodes(CurrentSelected.Group));

			//Collecting not-in-selected-group nodes
			PXResultset<GLBudgetLine> articlesNotInCurrentGroup = new PXResultset<GLBudgetLine>();
			foreach (PXResult<GLBudgetLine> existingArticle in existingArticles)
			{
				bool found = false;
				foreach (PXResult<GLBudgetLine> articleInCurrentGroup in articlesInCurrentGroup)
				{
					if (((GLBudgetLine)articleInCurrentGroup).GroupID == ((GLBudgetLine)existingArticle).GroupID)
					{
						found = true;
					}
				}
				if (!found)
				{
					articlesNotInCurrentGroup.Add(existingArticle);
				}
			}

			//Put new articles into existing groups if possible
			foreach (GLBudgetLine newArticle in articlesFromHistory)
			{
				bool skipArticle = false;
				//Do not update/add articles that are not in the selected group
				foreach (GLBudgetLine existingArticle in articlesNotInCurrentGroup)
				{
					if (existingArticle.AccountMask != null)
					{
						if (MatchMask(((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, newArticle.AccountID)).AccountCD, existingArticle.AccountMask ?? String.Empty)
							&& MatchMask(((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, newArticle.SubID)).SubCD, existingArticle.SubMask ?? String.Empty))
						{
							skipArticle = true;
						}
					}
				}
				foreach (GLBudgetLine existingArticle in articlesInCurrentGroup)
				{
					if (existingArticle.AccountMask != null)
					{
						if (MatchMask(((Account)PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, newArticle.AccountID)).AccountCD, existingArticle.AccountMask ?? String.Empty)
							&& MatchMask(((Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, newArticle.SubID)).SubCD, existingArticle.SubMask ?? String.Empty))
						{
							skipArticle = false;
							if ((bool)existingArticle.IsGroup || !String.IsNullOrEmpty(existingArticle.AccountMask) || !String.IsNullOrEmpty(existingArticle.SubMask))
							{
								GLBudgetLine innerArticle = PutIntoInnerGroup(newArticle, existingArticle);
                                if ((innerArticle.AccountID == newArticle.AccountID && innerArticle.SubID == newArticle.SubID) ||
                                    (!String.IsNullOrEmpty(innerArticle.AccountMask) || !String.IsNullOrEmpty(innerArticle.SubMask)))
                                {
                                    newArticle.ParentGroupID = innerArticle.GroupID;
                                    newArticle.GroupMask = innerArticle.GroupMask;
                                }
                                if (innerArticle.AccountID != null && innerArticle.SubID != null)
								{
									newArticle.Rollup = true;
								}
							}
							else
							{
								newArticle.ParentGroupID = existingArticle.ParentGroupID;
								newArticle.GroupMask = existingArticle.GroupMask;
							}
						}
					}
				}
				if (skipArticle)
				{
					foreach (GLBudgetLineDetail allocLine in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
					{
						allocLine.GroupID = Guid.Empty;
					}
					newArticle.GroupID = Guid.Empty;
				}
			}

			//Remove all articles/allocations that should not be processed
			articlesFromHistory.RemoveAll(x => x.GroupID == Guid.Empty);
			allocationsFromHistory.RemoveAll(x => x.GroupID == Guid.Empty);

			switch (PreloadFilter.Current.PreloadAction)
			{
				//Reload all
				case 0:
					{
						foreach (GLBudgetLine article in articlesInCurrentGroup)
						{
							if (!(bool)article.IsPreloaded && article.ReleasedAmount == 0)
							{
								BudgetArticles.Delete(article);
							}
						}
						foreach (GLBudgetLine newArticle in articlesFromHistory)
						{
                            if (LocateInBudgetArticlesCache(existingArticles, newArticle, true) == null && LocateInBudgetArticlesCache(existingHiddenArticles, newArticle, true) == null)
							{
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									if (LocateInBudgetAllocationsCache(existingAllocations, newAllocation) == null)
									{
										newArticle.Amount += newAllocation.Amount;
										Allocations.Insert(newAllocation);
										RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
									}
								}
								BudgetArticles.Insert(newArticle);
							}
						}

						break;
					}
				//Update existing
				case 1:
					{
						foreach (GLBudgetLine newArticle in articlesFromHistory)
						{
							GLBudgetLine locatedArticle = LocateInBudgetArticlesCache(articlesInCurrentGroup, newArticle, false);
							if (locatedArticle != null && !(bool)locatedArticle.IsGroup && !locatedArticle.AccountMask.Contains('?') && !locatedArticle.SubMask.Contains('?'))
							{
								locatedArticle.Allocated = null;
								newArticle.ParentGroupID = locatedArticle.ParentGroupID;
								foreach (GLBudgetLineDetail allocation in existingAllocations)
								{
									if (allocation.GroupID == locatedArticle.GroupID)
									{
										RollupAllocation(locatedArticle, int.Parse(allocation.FinPeriodID.Substring(4)), -allocation.Amount);
										allocation.Amount = 0;
										Allocations.Update(allocation);
									}
								}
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									GLBudgetLineDetail locatedAlloc = LocateInBudgetAllocationsCache(existingAllocations, newAllocation);
									if (locatedAlloc != null)
									{
										locatedAlloc.Amount = newAllocation.Amount;
										newArticle.Amount += locatedAlloc.Amount;
										Allocations.Update(locatedAlloc);
										RollupAllocation(locatedArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
									}
									else
									{
										newAllocation.GroupID = locatedArticle.GroupID;
										newArticle.Amount += newAllocation.Amount;
										Allocations.Insert(newAllocation);
										RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
									}
								}
								locatedArticle.Amount = newArticle.Amount;
								BudgetArticles.Update(locatedArticle);
							}
							//Updating amounts on Aggregating lines
							else if (newArticle.Rollup != null && (bool)newArticle.Rollup)
							{
								GLBudgetLine parentNode = PXSelect<GLBudgetLine,
									Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>,
									And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
									And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
									And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>.Select(this,
									newArticle.ParentGroupID);
                                if (parentNode != null && parentNode.Cleared == null)
								{
									parentNode.Amount = 0;
									parentNode.Cleared = true;
									BudgetArticles.Update(newArticle);

									foreach (GLBudgetLineDetail allocation in PXSelect<GLBudgetLineDetail,
										Where<GLBudgetLineDetail.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
										And<GLBudgetLineDetail.branchID, Equal<Current<BudgetFilter.branchID>>,
										And<GLBudgetLineDetail.finYear, Equal<Current<BudgetFilter.finYear>>,
										And<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLineDetail.groupID>>>>>>>.Select(this, parentNode.GroupID))
									{
										RollupAllocation(parentNode, int.Parse(allocation.FinPeriodID.Substring(4)), -allocation.Amount);
										allocation.Amount = 0;
										Allocations.Update(allocation);
									}
								}
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									newArticle.Amount += newAllocation.Amount;
									Allocations.Insert(newAllocation);
									RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
								}
								BudgetArticles.Insert(newArticle);
							}
						}
						//Removing rollup lines
						foreach (GLBudgetLine article in BudgetArticles.Cache.Cached)
						{
							if (article.Rollup != null && (bool)article.Rollup)
							{
								BudgetArticles.Delete(article);
							}
							if (article.Cleared != null)
							{
								article.Cleared = null;
								article.Amount = article.AllocatedAmount;
								BudgetArticles.Update(article);
							}
						}
						break;
					}
				//Update and Add
				case 2:
					{
						foreach (GLBudgetLine newArticle in articlesFromHistory)
						{
							GLBudgetLine locatedArticle = LocateInBudgetArticlesCache(articlesInCurrentGroup, newArticle, true);
							if (locatedArticle != null && !locatedArticle.AccountMask.Contains('?') && !locatedArticle.SubMask.Contains('?'))
							{
								if (locatedArticle.IsGroup == false)
								{
									if (locatedArticle.Rollup != null && (bool)locatedArticle.Rollup)
									{
										BudgetArticles.Delete(locatedArticle);
									}
									else
									{
										locatedArticle.Allocated = null;
										newArticle.ParentGroupID = locatedArticle.ParentGroupID;
										foreach (GLBudgetLineDetail allocation in existingAllocations)
										{
											if (allocation.GroupID == locatedArticle.GroupID)
											{
												RollupAllocation(locatedArticle, int.Parse(allocation.FinPeriodID.Substring(4)), -allocation.Amount);
												allocation.Amount = 0;
												Allocations.Update(allocation);
											}
										}
									}
									foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
									{
										GLBudgetLineDetail locatedAllocation = LocateInBudgetAllocationsCache(existingAllocations, newAllocation);
										if (locatedAllocation != null)
										{
											locatedAllocation.Amount = newAllocation.Amount;
											newArticle.Amount += locatedAllocation.Amount;
											Allocations.Update(locatedAllocation);
											RollupAllocation(locatedArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
										}
										else
										{
											newAllocation.GroupID = locatedArticle.GroupID;
											newArticle.Amount += newAllocation.Amount;
											Allocations.Insert(newAllocation);
											RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
										}
									}
									locatedArticle.Amount = newArticle.Amount;
									BudgetArticles.Update(locatedArticle);
								}
							}
							//Updating amounts on Aggregating lines
							else if (newArticle.Rollup != null && (bool)newArticle.Rollup)
							{
								GLBudgetLine parentNode = PXSelect<GLBudgetLine,
									Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>,
									And<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
									And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
									And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>>.Select(this,
									newArticle.ParentGroupID);
                                if (parentNode != null && parentNode.Cleared == null)
								{
									parentNode.Amount = 0;
									parentNode.Cleared = true;
									BudgetArticles.Update(newArticle);

									foreach (GLBudgetLineDetail allocation in PXSelect<GLBudgetLineDetail,
										Where<GLBudgetLineDetail.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
										And<GLBudgetLineDetail.branchID, Equal<Current<BudgetFilter.branchID>>,
										And<GLBudgetLineDetail.finYear, Equal<Current<BudgetFilter.finYear>>,
										And<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLineDetail.groupID>>>>>>>.Select(this, parentNode.GroupID))
									{
										RollupAllocation(parentNode, int.Parse(allocation.FinPeriodID.Substring(4)), -allocation.Amount);
										allocation.Amount = 0;
										Allocations.Update(allocation);
									}
								}
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									newArticle.Amount += newAllocation.Amount;
									Allocations.Insert(newAllocation);
									RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
								}
								BudgetArticles.Insert(newArticle);
							}
                            else if (LocateInBudgetArticlesCache(existingHiddenArticles, newArticle, true) == null)
							{
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									newArticle.Amount += newAllocation.Amount;
									Allocations.Insert(newAllocation);
									RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
								}
								BudgetArticles.Insert(newArticle);
							}
						}
						//Removing rollup lines
						foreach (GLBudgetLine article in BudgetArticles.Cache.Cached)
						{
							if (article.Rollup != null && (bool)article.Rollup)
							{
								BudgetArticles.Delete(article);
							}
							if (article.Cleared != null)
							{
								article.Cleared = null;
								article.Amount = article.AllocatedAmount;
								BudgetArticles.Update(article);
							}
						}
						break;
					}
				//Add
				case 3:
					{
						foreach (GLBudgetLine newArticle in articlesFromHistory)
						{
                            if (LocateInBudgetArticlesCache(existingArticles, newArticle, true) == null && LocateInBudgetArticlesCache(existingHiddenArticles, newArticle, true) == null && (newArticle.Rollup == null || !(bool)newArticle.Rollup))
							{
								foreach (GLBudgetLineDetail newAllocation in allocationsFromHistory.Where(x => x.GroupID == newArticle.GroupID))
								{
									newArticle.Amount += newAllocation.Amount;
									Allocations.Insert(newAllocation);
									RollupAllocation(newArticle, int.Parse(newAllocation.FinPeriodID.Substring(4)), newAllocation.Amount);
								}
								BudgetArticles.Insert(newArticle);
							}
						}
						break;
					}
			}
			return adapter.Get();
		}

		#endregion

		#region Events

		#region GLBudgetLineDetail

		protected virtual void AllocationFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, int fieldNbr)
		{
			GLBudgetLine article = (GLBudgetLine)e.Row;
			e.ReturnState = PXDecimalState.CreateInstance(e.ReturnState, Filter.Current.Precision, "Period" + fieldNbr, false, 0, decimal.MinValue, decimal.MaxValue);
			((PXDecimalState)e.ReturnState).DisplayName = String.Format(PXMessages.LocalizeNoPrefix(Messages.PeriodFormatted), fieldNbr);
			((PXDecimalState)e.ReturnState).Enabled = true;
			((PXDecimalState)e.ReturnState).Visible = true;
			((PXDecimalState)e.ReturnState).Visibility = PXUIVisibility.Visible;
			if (article != null)
			{
				decimal val;
				if (article.IsGroup != null && (bool)article.IsGroup)
				{
					((PXDecimalState)e.ReturnState).Enabled = false;
				}
				if (article.Comparison == true && article.Compared != null && fieldNbr <= article.Compared.Length)
				{
					((PXDecimalState)e.ReturnState).Enabled = false;
					val = article.Compared[fieldNbr - 1];
				}
				else
				{
					EnsureAlloc(article);
					val = article.Allocated[fieldNbr - 1];
				}
				e.ReturnValue = val;
			}
		}

		protected virtual void AllocationFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e, int fieldNbr)
		{
			string s = e.NewValue as string;
			decimal parsedValue;
			decimal? val = s != null
							   ? (decimal.TryParse(s, NumberStyles.Any, sender.Graph.Culture, out parsedValue) ? parsedValue : 0m)
							   : (decimal?)e.NewValue;
			UpdateAlloc(val ?? 0m, (GLBudgetLine)e.Row, fieldNbr);
			((GLBudgetLine)e.Row).Released = false;
		}

		#endregion

		#region Filters

		protected virtual void BudgetDistributeFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			BudgetFilter f = Filter.Current;
			GLBudgetLine article = BudgetArticles.Current;
			if (article == null) return;

			Dictionary<string, string> allowed = new BudgetDistributeFilter.method.ListAttribute().ValueLabelDic;
			if (f == null || f.CompareToBranchID == null || f.CompareToLedgerId == null || string.IsNullOrEmpty(f.CompareToFinYear) || article.Compared == null || article.Compared.Sum() == 0m)
			{
				allowed.Remove(BudgetDistributeFilter.method.ComparedValues);
			}

			GLBudgetLine prev = GetPrevArticle(article);
			if (prev == null || prev.AllocatedAmount == 0m)
			{
				allowed.Remove(BudgetDistributeFilter.method.PreviousYear);
			}

			PXStringListAttribute.SetList<BudgetDistributeFilter.method>(sender, e.Row,
														   allowed.Keys.ToArray(),
														   allowed.Values.ToArray());

			BudgetDistributeFilter row = e.Row as BudgetDistributeFilter;
			PXUIFieldAttribute.SetEnabled<BudgetDistributeFilter.applyToSubGroups>(sender, row, row.ApplyToAll != null && (bool)row.ApplyToAll);
			if (row.ApplyToAll == null || !(bool)row.ApplyToAll) row.ApplyToSubGroups = false;
		}

		protected virtual void BudgetFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			BudgetFilter row = (BudgetFilter)e.Row;
			this.ShowPreload.SetEnabled(row != null && row.BranchID != null && row.LedgerID != null && row.FinYear != null);
			this.ShowManage.SetEnabled(row != null && row.BranchID != null && row.LedgerID != null && row.FinYear != null);
			BudgetArticles.Cache.AllowInsert = row != null && row.BranchID != null && row.LedgerID != null && row.FinYear != null;
			if (row != null && row.Precision == null)
			{
				Currency currency = PXSelectJoin<Currency,
					InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<Currency.curyID>>>,
					Where<Ledger.ledgerID, Equal<Current<BudgetFilter.ledgerID>>>>
					.Select(this);
				row.Precision = currency != null ? currency.DecimalPlaces : 2;
			}
			//Hide "Show Tree" when Budget Configuration is empty
			PXUIFieldAttribute.SetVisible(sender, typeof(BudgetFilter.showTree).Name, PXSelect<GLBudgetTree>.Select(this).Count == 0 ? false : true);
			if (row != null)
			{
				if (PXSelect<GLBudgetTree>.Select(this).Count == 0) row.ShowTree = false;
				PXUIFieldAttribute.SetVisible(BudgetArticles.Cache, typeof(GLBudgetLine.isGroup).Name, (bool)row.ShowTree);
			}
			if (row.LedgerID != null && row.CompareToLedgerId != null && ((Ledger)PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Current<BudgetFilter.ledgerID>>>>.Select(this)).BaseCuryID !=
				((Ledger)PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<BudgetFilter.ledgerID>>>>.Select(this, row.CompareToLedgerId)).BaseCuryID)
			{
				PXUIFieldAttribute.SetWarning<BudgetFilter.compareToLedgerID>(sender, row, Messages.BudgetDifferentCurrency);
			}
			else
			{
				PXUIFieldAttribute.SetWarning<BudgetFilter.compareToLedgerID>(sender, row, null);
			}
		}

		protected virtual void BudgetFilter_SubIDFilter_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void BudgetFilter_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			BudgetFilter row = (BudgetFilter)e.Row;
			BudgetFilter newRow = (BudgetFilter)e.NewRow;
			if ((newRow.BranchID != row.BranchID || newRow.LedgerID != row.LedgerID || newRow.FinYear != row.FinYear) && BudgetArticles.Cache.IsDirty)
			{
				newRow.BranchID = row.BranchID;
				newRow.LedgerID = row.LedgerID;
				newRow.FinYear = row.FinYear;
				BudgetArticles.Ask(Messages.BudgetPendingChangesTitle, Messages.BudgetPendingChangesMessage, MessageButtons.OK);
			}
		}

		protected virtual void BudgetFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			BudgetFilter row = (BudgetFilter)e.Row;
			if (row == null) return;
			if (Filter.Current.BranchID != null && Filter.Current.LedgerID != null && Filter.Current.FinYear != null)
			{
				if (PXSelect<GLBudgetTree>.Select(this).Count != 0 && PXSelect<GLBudgetLine,
				Where<GLBudgetLine.branchID, Equal<Current<BudgetFilter.branchID>>,
					And<GLBudgetLine.ledgerID, Equal<Current<BudgetFilter.ledgerID>>,
					And<GLBudgetLine.finYear, Equal<Current<BudgetFilter.finYear>>>>>>.Select(this).Count == 0)
				{
                    WebDialogResult result = Filter.Ask(Messages.BudgetArticlesPreloadFromConfigurationTitle, Messages.BudgetArticlesPreloadFromConfiguration, MessageButtons.YesNo);
                    if (result == WebDialogResult.Yes)
					{
						PreloadBudgetTree();
                        this.Persist();
                    }
                    else
                    {
                        Filter.Current.FinYear = null;
					}
				}
			}
			BudgetFilter old = (BudgetFilter)e.OldRow;
			if (old.CompareToBranchID != Filter.Current.CompareToBranchID || old.CompareToLedgerId != Filter.Current.CompareToLedgerId || old.CompareToFinYear != Filter.Current.CompareToFinYear || Filter.Current.CompareToFinYear != null)
			{
				PopulateComparison(Filter.Current.CompareToBranchID, Filter.Current.CompareToLedgerId, Filter.Current.CompareToFinYear);
			}
		}

		protected virtual void BudgetPreloadFilter_AccountIDFilter_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			PXStringState strState = (PXStringState)sender.GetStateExt(null, typeof(BudgetPreloadFilter.fromAccount).Name);
			PXDBStringAttribute.SetInputMask(sender, typeof(BudgetPreloadFilter.accountIDFilter).Name, strState.InputMask.Replace('#', 'C'));
		}

		protected virtual void BudgetPreloadFilter_AccountIDFilter_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (e.NewValue != null)
			{
				PXStringState strState = (PXStringState)sender.GetStateExt(null, typeof(BudgetPreloadFilter.fromAccount).Name);
				e.NewValue = ((string)e.NewValue).PadRight(strState.InputMask.Length - 1, '?').Replace(' ', '?');
			}
		}

		protected virtual void BudgetPreloadFilter_SubIDFilter_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void BudgetPreloadFilter_SubIDFilter_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			PXStringState strState = (PXStringState)sender.GetStateExt(null, typeof(BudgetPreloadFilter.subIDFilter).Name);
			if (e.NewValue != null)
			{
				e.NewValue = ((string)e.NewValue).PadRight(strState.InputMask.Length - 1, '?').Replace(' ', '?');
			}
			else
			{
				e.NewValue = string.Empty;
				e.NewValue = ((string)e.NewValue).PadRight(strState.InputMask.Length - 1, '?').Replace(' ', '?');
			}
		}

		protected virtual void BudgetPreloadFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			BudgetPreloadFilter row = (BudgetPreloadFilter)e.Row;
			if (row == null) return;
			if (Filter.Current.LedgerID != null && row.LedgerID != null && ((Ledger)PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Current<BudgetFilter.ledgerID>>>>.Select(this)).BaseCuryID !=
				((Ledger)PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<BudgetPreloadFilter.ledgerID>>>>.Select(this, row.LedgerID)).BaseCuryID)
			{
				PXUIFieldAttribute.SetWarning<BudgetPreloadFilter.ledgerID>(cache, row, Messages.BudgetDifferentCurrency);
			}
			else
			{
				PXUIFieldAttribute.SetWarning<BudgetPreloadFilter.ledgerID>(cache, row, null);
			}
		}

		#endregion

		#region GLBudgetLine
		
		protected virtual void GLBudgetLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GLBudgetLine row = (GLBudgetLine)e.Row;
			if (row == null) return;
			Distribute.SetEnabled(row.Comparison == null ? true : false);
            if ((row.Amount != null && row.Amount != 0) || (row.AllocatedAmount != null && row.AllocatedAmount != 0))
            {
                PXUIFieldAttribute.SetEnabled<GLBudgetLine.accountID>(sender, row, false);
                PXUIFieldAttribute.SetEnabled<GLBudgetLine.subID>(sender, row, false);
            }
            if (row.Released != null && row.Released == true)
			{
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.accountID>(sender, row, !(bool)row.Released);
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.subID>(sender, row, !(bool)row.Released);
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.description>(sender, row, !(bool)row.Released);
			}
			if (row.IsGroup != null && (bool)row.IsGroup)
			{
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.accountID>(sender, row, false);
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.subID>(sender, row, false);
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.amount>(sender, row, false);
			}
			if (row.IsPreloaded != null && row.IsPreloaded == true)
			{
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.accountID>(sender, row, false);
				PXUIFieldAttribute.SetEnabled<GLBudgetLine.subID>(sender, row, false);
			}
			sender.RaiseExceptionHandling<GLBudgetLine.allocatedAmount>(row, row.AllocatedAmount, row.Amount != row.AllocatedAmount ?
				(new PXSetPropertyException(Messages.BudgetLineAmountNotEqualAllocated, PXErrorLevel.RowWarning)) : null);
		}

		protected virtual void GLBudgetLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			GLBudgetLine row = (GLBudgetLine)e.Row;
			if (row == null) return;
			//Allocated Amount is not updated correctly when uploading records from file. To investigate
			if ((row.AccountID != null && row.SubID != null && row.Amount != 0 && row.AllocatedAmount == 0) || (row.IsGroup != null && (bool)row.IsGroup))
			{
				decimal allocatedAmt = 0;
				foreach (GLBudgetLineDetail alloc in PXSelect<GLBudgetLineDetail, Where<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLineDetail.groupID>>>>.Select(this, row.GroupID))
				{
					allocatedAmt += (decimal)alloc.Amount;
				}
				row.AllocatedAmount = allocatedAmt;
			}
			//Update amounts in all parent nodes
			if (row.ParentGroupID != Guid.Empty)
			{
				GLBudgetLine oldRow = (GLBudgetLine)e.OldRow;
				RollupArticleAmount(row, row.Amount - oldRow.Amount);
			}
			if ((bool)row.IsGroup) EnsureAlloc(row);
		}

		protected virtual void GLBudgetLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			GLBudgetLine row = (GLBudgetLine)e.Row;
			if (row.Comparison != null && (bool)row.Comparison)
			{
				e.Cancel = true;
				BudgetArticles.Cache.SetStatus(row, PXEntryStatus.Held);
			}
			if ((bool)row.IsGroup)
			{
				PXDefaultAttribute.SetPersistingCheck<GLBudgetLine.accountID>(sender, e.Row, PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<GLBudgetLine.subID>(sender, e.Row, PXPersistingCheck.Nothing);
			}
			if (row.AccountMask == null && row.SubMask == null && row.AccountID != null && row.SubID != null)
			{
				Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<GLBudgetLine.accountID>>>>.Select(this, row.AccountID);
				Sub sub = PXSelect<Sub, Where<Sub.subID, Equal<Required<GLBudgetLine.subID>>>>.Select(this, row.SubID);
				row.AccountMask = account != null ? account.AccountCD : null;
				row.SubMask = sub != null ? sub.SubCD : null;
			}
			if (!SubEnabled)
			{
				List<dublicates> listCached = new List<dublicates>();
				foreach (GLBudgetLine item in sender.Cached)
				{
					if (!(bool)item.IsGroup && item.AccountID != null)
					{
						dublicates line = new dublicates();
						line.branch = (int)item.BranchID;
						line.ledger = (int)item.LedgerID;
						line.finYear = item.FinYear;
						line.account = (int)item.AccountID;
						listCached.Add(line);
					}
				}
				if (listCached.Count != listCached.Distinct().Count())
				{
					BudgetArticles.Cache.RaiseExceptionHandling<GLBudgetLine.accountID>(e.Row, row.AccountID,
							new PXSetPropertyException(Messages.DuplicateAccountSubEntry, PXErrorLevel.RowError));
				}
			}
		}

		protected virtual void GLBudgetLineDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			GLBudgetLineDetail row = (GLBudgetLineDetail)e.Row;
			if (row.AccountID == null || row.SubID == null)
			{
				PXDefaultAttribute.SetPersistingCheck<GLBudgetLineDetail.accountID>(sender, e.Row, PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<GLBudgetLineDetail.subID>(sender, e.Row, PXPersistingCheck.Nothing);
			}
		}

		private struct dublicates
		{
			public int branch;
			public int ledger;
			public string finYear;
			public int account;
			public int sub;
		}

		protected virtual void GLBudgetLine_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			GLBudgetLine row = (GLBudgetLine)e.NewRow;
			GLBudgetLine oldRow = (GLBudgetLine)e.Row;
			if (row == null) return;

			//Searching for duplicates
			if (SubEnabled && row.SubID != null)
			{
				if (row.AccountID != oldRow.AccountID || row.SubID != oldRow.SubID)
				{
					List<dublicates> listCached = new List<dublicates>();
					foreach (GLBudgetLine item in sender.Cached)
					{
						if (!(bool)item.IsGroup && item.AccountID != null && item.SubID != null)
						{
							dublicates line = new dublicates();
							line.branch = (int)item.BranchID;
							line.ledger = (int)item.LedgerID;
							line.finYear = item.FinYear;
							line.account = (int)item.AccountID;
							line.sub = (int)item.SubID;
							listCached.Add(line);
						}
					}
					if (listCached.Count != listCached.Distinct().Count())
					{
						e.Cancel = true;
						throw new PXException(Messages.DuplicateAccountSubEntry, PXErrorLevel.RowError);
					}
				}
			}
		}

		protected virtual void GLBudgetLine_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			GLBudgetLine row = e.Row as GLBudgetLine;
			if (row == null) return;

			if (!(bool)row.IsGroup && row.IsUploaded != null && (row.AccountID == null || row.SubID == null))
			{
				e.Cancel = true;
			}

			row.GroupID = row.GroupID ?? Guid.NewGuid();

			if (CurrentSelected.AccountID != null && CurrentSelected.AccountID != Int32.MinValue && CurrentSelected.SubID != null && CurrentSelected.SubID != Int32.MinValue)
			{
				row.Rollup = true;
			}
			//To rewrite
			if (row.ParentGroupID == Guid.Empty || row.ParentGroupID == null)
			{
				row.ParentGroupID = CurrentSelected.Group ?? Guid.Empty;
			}

			if (row.GroupMask == null) row.GroupMask = new byte[0];
			if (row.ParentGroupID != Guid.Empty && row.GroupMask.Length == 0)
			{
				GLBudgetLine parentNode = PXSelect<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Required<GLBudgetLine.groupID>>>>.Select(this, row.ParentGroupID);
				if (parentNode != null)
				row.GroupMask = parentNode.GroupMask;
			}
		}
		
		protected virtual void GLBudgetLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			GLBudgetLine article = e.Row as GLBudgetLine;
			foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
			{
				Allocations.Update(alloc);
			}

			PXFormulaAttribute.CalcAggregate<GLBudgetLineDetail.amount>(Allocations.Cache, article);

			//Update amounts in all parent nodes
			if (article.ParentGroupID != Guid.Empty)
			{
				RollupArticleAmount(article, article.Amount);
			}
		}

		protected virtual void GLBudgetLine_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			GLBudgetLine article = e.Row as GLBudgetLine;
			//Released articles cannot be deleted
			if (e.ExternalCall)
			{
				foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
				{
					if (alloc.ReleasedAmount != 0m)
					{
						throw new PXException(Messages.ReleasedBudgetArticleCanNotBeDeleted);
					}
				}
				//Show error when trying to delete preconfigured article/group
				if (article.IsPreloaded != null && (bool)article.IsPreloaded)
				{
					throw new PXException(Messages.PreconfiguredArticlesCanNotBeDeleted);
				}
			}
			//Show error when trying to delete comparison line
			if (article.Comparison != null && (bool)article.Comparison)
			{
				throw new PXException(Messages.ComparisonLinesCanNotBeDeleted);
			}

			//Update amounts in all parent nodes
			if (e.ExternalCall)
			{
				if (article.ParentGroupID != Guid.Empty)
				{
					RollupArticleAmount(article, -article.Amount);
					foreach (GLBudgetLineDetail alloc in Allocations.Select(article.FinYear, article.GroupID))
					{
						RollupAllocation(article, int.Parse(alloc.FinPeriodID.Substring(4)), -alloc.Amount);
					}
				}
			}
		}

		protected virtual void GLBudgetLine_AccountID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			GLBudgetLine article = e.Row as GLBudgetLine;
			if (suppressIDs && article != null && article.Comparison == true)
			{
				e.ReturnValue = null;
			}
		}

		protected virtual void GLBudgetLine_SubID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			GLBudgetLine article = e.Row as GLBudgetLine;
			if (suppressIDs && article != null && article.Comparison == true)
			{
				e.ReturnValue = null;
			}
		}

		protected virtual void GLBudgetLine_Amount_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GLBudgetLine row = (GLBudgetLine)e.Row;
			if (row == null) return;
			row.Released = false;
		}

		protected virtual void GLBudgetLine_AccountID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			GLBudgetLine row = e.Row as GLBudgetLine;
			if (row == null) return;
			if ((row.IsGroup != null && (bool)row.IsGroup) || (row.IsPreloaded != null && (bool)row.IsPreloaded))
			{
				e.Cancel = true;
			}
		}

		protected virtual void GLBudgetLine_SubID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			GLBudgetLine row = e.Row as GLBudgetLine;
			if (row == null) return;
			if (row.IsGroup != null && (bool)row.IsGroup)
			{
				e.Cancel = true;
			}

            Sub sub = (Sub)PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this, e.NewValue);
    		if (sub != null && !MatchMask(sub.SubCD, CurrentSelected.SubMask ?? String.Empty))
			{
				throw new PXSetPropertyException(String.Format(PXMessages.LocalizeNoPrefix(Messages.BudgetSubaccountNotAllowed), CurrentSelected.SubMask));
			}
		}

		#endregion

		protected virtual void ManageBudgetDialog_Message_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = PXMessages.LocalizeNoPrefix(Messages.BudgetRollbackMessage);
		}

		protected virtual void ManageBudgetDialog_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ManageBudgetDialog row = (ManageBudgetDialog)e.Row;
			if (row == null) return;
			switch (row.Method)
			{
				case ManageBudgetDialog.method.RollbackBudget:
					{
						row.Message = PXMessages.LocalizeNoPrefix(Messages.BudgetRollbackMessage);
						break;
					}
				case ManageBudgetDialog.method.ConvertBudget:
					{
						row.Message = PXMessages.LocalizeNoPrefix(Messages.BudgetConvertMessage);
						break;
					}
			}
		}

		#endregion

	}

	[Serializable]
    [PXHidden]
	public partial class SelectedGroup : IBqlTable
	{
		#region Group
		public abstract class group : PX.Data.IBqlField
		{
		}
		protected Guid? _Group;
		[PXDBGuid(IsKey = true)]
		public virtual Guid? Group
		{
			get
			{
				return this._Group;
			}
			set
			{
				this._Group = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXInt()]
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
		protected int? _SubID;
		[PXInt()]
		public virtual int? SubID
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
		#region AccountMask
		public abstract class accountMask : IBqlField { };
		[PXDBString(10, IsUnicode = true)]
		public virtual string AccountMask { get; set; }
		#endregion
		#region SubMask
		public abstract class subMask : IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual string SubMask { get; set; }
		#endregion
		#region AccountMask Wildcard
		public abstract class accountMaskWildcard : IBqlField { };
		[PXDBString(10, IsUnicode = true)]
		public virtual string AccountMaskWildcard { get; set; }
		#endregion
		#region SubMask Wildcard
		public abstract class subMaskWildcard : IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual string SubMaskWildcard { get; set; }
		#endregion
	}

	[Serializable]
	public partial class BudgetDistributeFilter : IBqlTable
	{
		#region Method
		public abstract class method : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new[] { Evenly, PreviousYear, ComparedValues },
				new[] { Messages.Evenly, Messages.PreviousYear, Messages.ComparedValues }) { }
			}

			public class NonComparedListAttribute : PXStringListAttribute
			{
				public NonComparedListAttribute()
					: base(
				new[] { Evenly, PreviousYear },
				new[] { Messages.Evenly, Messages.PreviousYear }) { }
			}

			public const string Evenly = "E";
			public const string PreviousYear = "P";
			public const string ComparedValues = "C";

			public class evenly : Constant<string>
			{
				public evenly() : base(Evenly) { }
			}
			public class previousYear : Constant<string>
			{
				public previousYear() : base(PreviousYear) { }
			}
			public class comparedValues : Constant<string>
			{
				public comparedValues() : base(ComparedValues) { }
			}
			#endregion
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(method.Evenly)]
		[method.List]
		[PXUIField(DisplayName = "Distribution Method")]
		public virtual String Method { get; set; }
		#endregion
		#region ApplyToAll
		public abstract class applyToAll : IBqlField { }
		[PXUIField(DisplayName = "Apply to all articles in this node")]
		[PXBool]
		public virtual bool? ApplyToAll { get; set; }
		#endregion
		#region ApplyToSubGroups
		public abstract class applyToSubGroups : IBqlField { }
        [PXUIField(DisplayName = "Apply to subarticles", Enabled = false)]
		[PXBool]
		public virtual bool? ApplyToSubGroups { get; set; }
		#endregion
	}

	[Serializable]
	public partial class BudgetPreloadFilter : IBqlTable
	{
		#region BranchID
		public abstract class branchID : IBqlField { }
		[Branch(typeof(BudgetFilter.branchID), Required = true)]
		public virtual int? BranchID { get; set; }
		#endregion

		#region LedgerId
		public abstract class ledgerID : IBqlField { }
		[PXDBInt]
		[PXUIField(DisplayName = "Ledger", Required = true)]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, Equal<LedgerBalanceType.actual>, Or<Ledger.balanceType, Equal<LedgerBalanceType.budget>>>>),
			SubstituteKey = typeof(Ledger.ledgerCD),
			DescriptionField = typeof(Ledger.descr))]
		[PXDefault(typeof(BudgetFilter.compareToLedgerID))]
		public virtual int? LedgerID { get; set; }
		#endregion

		#region FinYear
		public abstract class finYear : IBqlField { }
		[PXUIField(DisplayName = "Financial Year", Required = true)]
		[PXDBString(4)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>))]
		[PXDefault(typeof(BudgetFilter.compareToFinYear))]
		public virtual String FinYear { get; set; }
		#endregion

		#region ChangePercent
		public abstract class changePercent : IBqlField { }
		protected short? _ChangePercent;
		[PXShort()]
		[PXDefault((short)100)]
		[PXUIField(DisplayName = "Multiplier (in %)", Required = true)]
		public virtual short? ChangePercent
		{
			get
			{
				return this._ChangePercent;
			}
			set
			{
				this._ChangePercent = value;
			}
		}
		#endregion

		#region FromAccount
		public abstract class fromAccount : IBqlField { }
		[PXDBInt]
        [PXDimensionSelector(AccountAttribute.DimensionName, (typeof(Search<Account.accountID, Where<Account.accountCD, Like<Current<SelectedGroup.accountMaskWildcard>>>, OrderBy<Asc<Account.accountCD>>>)), typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
		[PXUIField(DisplayName = "Account from", Visibility = PXUIVisibility.Visible)]
		public virtual int? FromAccount { get; set; }
		#endregion

		#region ToAccount
		public abstract class toAccount : IBqlField { }
		[PXDBInt]
		[PXDimensionSelector(AccountAttribute.DimensionName, (typeof(Search<Account.accountID, Where<Account.accountCD, Like<Current<SelectedGroup.accountMaskWildcard>>>, OrderBy<Asc<Account.accountCD>>>)), typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
		[PXUIField(DisplayName = "Account to", Visibility = PXUIVisibility.Visible)]
		public virtual int? ToAccount { get; set; }
		#endregion

		#region AccountIDFilter
		public abstract class accountIDFilter : IBqlField { }
		[AccountRaw(DisplayName = "Account Mask")]
		public virtual string AccountIDFilter { get; set; }
		#endregion
		
		#region SubIDFilter
		public abstract class subIDFilter : IBqlField { }
		[SubAccountRaw(DisplayName = "Subaccount Mask")]
		public virtual string SubIDFilter { get; set; }
		#endregion

		#region SubCD Wildcard
		public abstract class subCDWildcard : IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual string SubCDWildcard
		{
			get
			{
				return SubCDUtils.CreateSubCDWildcard(this.SubIDFilter, SubAccountAttribute.DimensionName);
			}
		}
		#endregion

		#region AccountCD Wildcard
		public abstract class accountCDWildcard : IBqlField { };
		[PXDBString(10, IsUnicode = true)]
		public virtual string AccountCDWildcard { get; set; }
		#endregion

		#region PreloadAction
		[PXUIField()]
		[PXDefault((short)2)]
		[PXShort]
		public short? PreloadAction
		{
			get { return this._PreloadAction; }
			set { if (value != null) this._PreloadAction = value; }
		}
		protected short? _PreloadAction;
		#endregion

		#region Strategy
		public abstract class strategy : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new[] { UpdateExisting, UpdateAndLoad, LoadNotExisting },
				new[] { Messages.UpdateExisting, Messages.UpdateAndLoad, Messages.LoadNotExisting }) { }
			}

			public const string ReloadAll = "R";
			public const string UpdateExisting = "U";
			public const string UpdateAndLoad = "F";
			public const string LoadNotExisting = "L";

			public class reloadAll : Constant<string>
			{
				public reloadAll() : base(ReloadAll) { }
			}
			public class updateExisting : Constant<string>
			{
				public updateExisting() : base(UpdateExisting) { }
			}
			public class updateAndLoad : Constant<string>
			{
				public updateAndLoad() : base(UpdateAndLoad) { }
			}
			public class loadNotExisting : Constant<string>
			{
				public loadNotExisting() : base(LoadNotExisting) { }
			}
			#endregion
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(strategy.UpdateAndLoad)]
		[strategy.List]
		public virtual String Strategy { get; set; }
		#endregion
	}

	[Serializable]
	public partial class BudgetFilter : IBqlTable
	{
		#region BranchID
		public abstract class branchID : IBqlField { }
		[Branch(Required = true)]
		public virtual int? BranchID { get; set; }
		#endregion

		#region LedgerId
		public abstract class ledgerID : IBqlField { }
		[PXDBInt]
		[PXUIField(DisplayName = "Ledger", Required = true)]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, Equal<LedgerBalanceType.budget>, Or<Ledger.balanceType, Equal<LedgerBalanceType.statistical>>>>),
			SubstituteKey = typeof(Ledger.ledgerCD),
			DescriptionField = typeof(Ledger.descr))]
		public virtual int? LedgerID { get; set; }
		#endregion

		#region FinYear
		public abstract class finYear : IBqlField { }
		[PXUIField(DisplayName = "Financial Year", Required = true)]
		[PXDBString(4)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>))]
		public virtual String FinYear { get; set; }
		#endregion

		#region ShowTree
		public abstract class showTree : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowTree;
		[PXBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Tree View")]
		public virtual Boolean? ShowTree
		{
			get
			{
				return this._ShowTree;
			}
			set
			{
				this._ShowTree = value;
			}
		}
		#endregion

		#region CompareToBranchID
		public abstract class compareToBranchID : IBqlField { }
		[Branch(DisplayName = "Compare To Branch", Required = false)]
		public virtual int? CompareToBranchID { get; set; }
		#endregion

		#region CompareToLedgerID
		public abstract class compareToLedgerID : IBqlField { }
		[PXDBInt]
		[PXUIField(DisplayName = "Compare To Ledger")]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, NotEqual<LedgerBalanceType.report>>>),
			SubstituteKey = typeof(Ledger.ledgerCD),
			DescriptionField = typeof(Ledger.descr))]
		[PXDefault(typeof(Search<Branch.ledgerID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? CompareToLedgerId { get; set; }
		#endregion

		#region CompareToFinYear
		public abstract class compareToFinYear : IBqlField { }
		[PXUIField(DisplayName = "Compare To Year")]
		[PXDBString(4)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>))]
		public virtual string CompareToFinYear { get; set; }
		#endregion

		#region SubIDFilter
		public abstract class subIDFilter : IBqlField { }
		[SubAccountRaw(DisplayName = "Subaccount Filter")]
		public virtual string SubIDFilter { get; set; }
		#endregion

		#region SubCD Wildcard
		public abstract class subCDWildcard : IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual string SubCDWildcard
		{
			get
			{
				return SubCDUtils.CreateSubCDWildcard(this.SubIDFilter, SubAccountAttribute.DimensionName);
			}
		}
		#endregion

		#region Tree Node Filter
		public abstract class treeNodeFilter : IBqlField { };
		[PXUIField(DisplayName = "Tree Node Filter")]
		[PXDBString(30, IsUnicode = true)]
		public virtual string TreeNodeFilter { get; set; }
		#endregion

		public short? Precision;

		#region Report fields
		#region RepLedgerId
		public abstract class repLedgerID : IBqlField { }
		[PXDBInt()]
		[PXDefault(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, Equal<LedgerBalanceType.budget>>, OrderBy<Desc<Ledger.ledgerID>>>))]
		[PXUIField(DisplayName = "Ledger", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, Equal<LedgerBalanceType.budget>>>),
			SubstituteKey = typeof(Ledger.ledgerCD),
			DescriptionField = typeof(Ledger.descr))]
		public virtual int? RepLedgerID { get; set; }
		#endregion

		#region RepFinYear
		public abstract class repFinYear : PX.Data.IBqlField { }
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Financial Year", Required = true)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>))]
		public virtual DateTime? RepFinYear { get; set; }
		#endregion
		#endregion
	}

	public class BudgetLedger : Constant<string>
	{
		public BudgetLedger()
			: base("B")
		{
		}
	}

	[Serializable]
	public partial class ManageBudgetDialog : IBqlTable
	{
		#region Message
		[PXString(IsUnicode = true)]
		[PXUIField(Enabled = false)]
		public virtual String Message { get; set; }
		#endregion
		#region Method
		public abstract class method : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new[] { RollbackBudget, ConvertBudget },
				new[] { Messages.BudgetRollback, Messages.BudgetConvert }) { }
			}

			public const string RollbackBudget = "R";
			public const string ConvertBudget = "C";

			public class rollbackBudget : Constant<string>
			{
				public rollbackBudget() : base(RollbackBudget) { }
			}
			public class convertBudget : Constant<string>
			{
				public convertBudget() : base(ConvertBudget) { }
			}
			#endregion
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(method.RollbackBudget)]
		[method.List]
		[PXUIField(DisplayName = Messages.BudgetManageAction)]
		public virtual String Method { get; set; }
		#endregion
	}
}
