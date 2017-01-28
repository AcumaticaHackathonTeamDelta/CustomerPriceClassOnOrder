using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.BQLConstants;
using PX.Objects.CS;


namespace PX.Objects.GL
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class GLBudgetRelease : PXGraph<GLBudgetRelease>
	{
		#region CacheAttachedEvents
		#region GLBudgetLine_Selected
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected", Visible = true, Enabled = true, Visibility = PXUIVisibility.Visible)]
		protected virtual void GLBudgetLine_Selected_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region GLBudgetLine_LedgerID
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(BudgetFilter.ledgerID))]
		[PXUIField(DisplayName="Ledger", Enabled = false, Visible = true, Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD))]
		protected virtual void GLBudgetLine_LedgerID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion

		public PXCancel<GLBudgetLine> Cancel;
        public PXAction<GLBudgetLine> EditDetail;

		[PXFilterable]
		public PXProcessing<GLBudgetLine,
			Where<GLBudgetLine.released, NotEqual<boolTrue>,
			And<GLBudgetLine.amount, Equal<GLBudgetLine.allocatedAmount>,
			And<GLBudgetLine.accountID, IsNotNull,
			And<GLBudgetLine.subID, IsNotNull,
			And<GLBudgetLine.rollup, Equal<False>,
			And<Match<Current<AccessInfo.userName>>>>>>>>,
			OrderBy<Asc<GLBudgetLine.finYear>>> BudgetArticles;

        [PXEditDetailButton]
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable editDetail(PXAdapter adapter)
		{
			if (BudgetArticles.Current != null)
			{
				GLBudgetEntry graph = PXGraph.CreateInstance<GLBudgetEntry>();
				graph.Filter.Current.BranchID = BudgetArticles.Current.BranchID;
				graph.Filter.Current.LedgerID = BudgetArticles.Current.LedgerID;
				graph.Filter.Current.FinYear = BudgetArticles.Current.FinYear;
                throw new PXRedirectRequiredException(graph, true, "View Budget") { Mode = PXBaseRedirectException.WindowMode.NewWindow};
			}
			return adapter.Get();
		}

		public GLBudgetRelease()
        {
			GLSetup setup = GLSetup.Current;
			BudgetArticles.SetProcessCaption(Messages.ProcRelease);
            BudgetArticles.SetProcessAllCaption(Messages.ProcReleaseAll);
			BudgetArticles.SetProcessDelegate(Approve);

			PXUIFieldAttribute.SetVisible<GLBudgetLine.branchID>(BudgetArticles.Cache, null, true);
			PXUIFieldAttribute.SetVisible<GLBudgetLine.ledgerID>(BudgetArticles.Cache, null, true);
			PXUIFieldAttribute.SetVisible<GLBudgetLine.finYear>(BudgetArticles.Cache, null, true);
		}

		public PXSetup<GLSetup> GLSetup;

		public static void Approve(List<GLBudgetLine> list)
		{
			bool anyFailed = false;
			GLBudgetEntry graph = PXGraph.CreateInstance<GLBudgetEntry>();
			graph.Views.Caches.Add(typeof(AccountHistory));

			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					GLBudgetLine article = list[i];
					Ledger ledger = PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>>>.Select(graph, article.LedgerID);
					if (article.AllocatedAmount != article.Amount)
					{
						PXProcessing<GLBudgetLine>.SetError(i, Messages.BudgetArticleIsNotAllocatedProperly);
						anyFailed = true;
						continue;
					}
					Account acct = PXSelect<Account,
						Where<Account.accountID, Equal<Required<Account.accountID>>>>
						.Select(graph, article.AccountID);
					foreach (GLBudgetLineDetail alloc in PXSelect<GLBudgetLineDetail,
						Where<GLBudgetLineDetail.ledgerID, Equal<Required<GLBudgetLineDetail.ledgerID>>,
						And<GLBudgetLineDetail.branchID, Equal<Required<GLBudgetLineDetail.branchID>>,
						And<GLBudgetLineDetail.finYear, Equal<Required<GLBudgetLineDetail.finYear>>,
						And<GLBudgetLineDetail.accountID, Equal<Required<GLBudgetLineDetail.accountID>>,
						And<GLBudgetLineDetail.subID, Equal<Required<GLBudgetLineDetail.subID>>>>>>>>
						.Select(graph, article.LedgerID, article.BranchID, article.FinYear, article.AccountID, article.SubID))
					{
						decimal delta = (decimal)((alloc.Amount ?? 0m) - (alloc.ReleasedAmount ?? 0m));
						if (delta != 0m)
						{
							AccountHistory accthist = new AccountHistory
								{
									BranchID = alloc.BranchID,
									AccountID = alloc.AccountID,
									FinPeriodID = alloc.FinPeriodID,
									LedgerID = alloc.LedgerID,
									SubID = alloc.SubID,
									CuryID = null,
									BalanceType = ledger.BalanceType
								};
							accthist = (AccountHistory)graph.Caches[typeof(AccountHistory)].Insert(accthist);
							if (accthist == null)
							{
								PXProcessing<GLBudgetLine>.SetError(i, Messages.BudgetApproveUnexpectedError);
								anyFailed = true;
							}
							else
							{
								accthist.YtdBalance += delta;
								accthist.CuryFinYtdBalance = accthist.YtdBalance;
								accthist.TranYtdBalance += delta;
								accthist.CuryTranYtdBalance = accthist.TranYtdBalance;
								if (acct.Type == AccountType.Asset || acct.Type == AccountType.Expense)
								{
									accthist.FinPtdDebit += delta;
									accthist.CuryFinPtdDebit = accthist.FinPtdDebit;
									accthist.TranPtdDebit += delta;
									accthist.CuryTranPtdDebit = accthist.TranPtdDebit;
									//accthist.FinPtdCredit += 0;
									accthist.CuryFinPtdCredit = accthist.FinPtdCredit;
									//accthist.TranPtdCredit += 0;
									accthist.CuryTranPtdCredit = accthist.TranPtdCredit;
								}
								else
								{
									accthist.FinPtdCredit += delta;
									accthist.CuryFinPtdCredit = accthist.FinPtdCredit;
									accthist.TranPtdCredit += delta;
									accthist.CuryTranPtdCredit = accthist.TranPtdCredit;
									//accthist.FinPtdDebit += 0;
									accthist.CuryFinPtdDebit = accthist.FinPtdDebit;
									//accthist.TranPtdDebit += 0;
									accthist.CuryTranPtdDebit = accthist.TranPtdDebit;
								}
							}
						}
						alloc.ReleasedAmount = alloc.Amount;
						graph.Caches[typeof(GLBudgetLineDetail)].Update(alloc);
					}
					article.ReleasedAmount = article.Amount;
					article.Released = true;
					graph.Caches[typeof(GLBudgetLine)].Update(article);
					PXProcessing<GLBudgetLine>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					PXProcessing<GLBudgetLine>.SetError(i, e.Message);
					throw;
				}
			}
			graph.Save.Press();
			if (anyFailed)
			{
				throw new PXException(Messages.BudgetItemsApprovalFailure);
			}
		}

		public class AHAccumulatorAttribute : PXAccumulatorAttribute
		{
			public AHAccumulatorAttribute()
				: base(new Type[] {
					typeof(GLHistory.finYtdBalance),
					typeof(GLHistory.tranYtdBalance),
					typeof(GLHistory.curyFinYtdBalance),
					typeof(GLHistory.curyTranYtdBalance),
					typeof(GLHistory.finYtdBalance),
					typeof(GLHistory.tranYtdBalance),
					typeof(GLHistory.curyFinYtdBalance),
					typeof(GLHistory.curyTranYtdBalance)
					},
						new Type[] {
					typeof(GLHistory.finBegBalance),
					typeof(GLHistory.tranBegBalance),
					typeof(GLHistory.curyFinBegBalance),
					typeof(GLHistory.curyTranBegBalance),
					typeof(GLHistory.finYtdBalance),
					typeof(GLHistory.tranYtdBalance),
					typeof(GLHistory.curyFinYtdBalance),
					typeof(GLHistory.curyTranYtdBalance)
					}
				)
			{
			}

			protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
			{
				if (!base.PrepareInsert(sender, row, columns))
				{
					return false;
				}

				AccountHistory hist = (AccountHistory)row;

				columns.RestrictPast<GLHistory.finPeriodID>(PXComp.GE, hist.FinPeriodID.Substring(0, 4) + "01");
				columns.RestrictFuture<GLHistory.finPeriodID>(PXComp.LE, hist.FinPeriodID.Substring(0, 4) + "99");

				return true;
			}
		}

		[Serializable]
		[AHAccumulatorAttribute]
        [PXHidden]
		public partial class AccountHistory : GLHistory
		{
			#region LedgerID
			[PXDBInt(IsKey = true)]
			public override Int32? LedgerID
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
			[PXDBInt(IsKey = true)]
			public override Int32? AccountID
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
			[PXDBInt(IsKey = true)]
			public override Int32? SubID
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
			#region FinPeriod
			[PXDBString(6, IsFixed = true, IsKey = true)]
			public override String FinPeriodID
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
			#region BalanceType
			[PXDBString(1, IsFixed = true)]
			[PXDefault()]
			public override String BalanceType
			{
				get
				{
					return this._BalanceType;
				}
				set
				{
					this._BalanceType = value;
				}
			}
			#endregion
			#region CuryID
			[PXDBString(5, IsUnicode = true)]
			public override String CuryID
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
		}

	}
}
