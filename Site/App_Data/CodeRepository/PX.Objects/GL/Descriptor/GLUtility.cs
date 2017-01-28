using System;
using PX.Common;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.GL
{
	public static class AccountRules
	{
		public static bool IsCreditBalance(string accountType)
		{
			if (IsGIRLSAccount(accountType)) return true;
			if (IsDEALAccount(accountType)) return false;
			throw new PXException(Messages.UnknownAccountTypeDetected, accountType);
		}

		/// <summary>
		/// Returns <c>true</c> if the provided Account Type has Credit balance,
		/// that is belongs to the Gains, Income, Revenues, Liabilities and Stock-holder equity group.
		/// In Acumatica, Gains, Revenues and Stock-holder equity account types are not implemented.
		/// </summary>
		/// <param name="accountType"><see cref="Account.Type"/></param>
		public static bool IsGIRLSAccount(string accountType)
			{
			return accountType == AccountType.Income || accountType == AccountType.Liability;
			}

		/// <summary>
		/// Returns <c>true</c> if the provided Account Type has Debit balance,
		/// that is belongs to the Dividends, Expenses, Assets and Losses group.
		/// In Acumatica, Dividends and Losses account types are not implemented.
		/// </summary>
		/// <param name="accountType"><see cref="Account.Type"/></param>
		public static bool IsDEALAccount(string accountType)
			{
			return accountType == AccountType.Expense || accountType == AccountType.Asset;
		}

		public static decimal CalcSaldo(string aAcctType, decimal aDebitAmt, decimal aCreditAmt)
		{
			return (IsCreditBalance(aAcctType) ? (aCreditAmt - aDebitAmt) : (aDebitAmt - aCreditAmt));
		}

	}
	public class GLUtility
	{
		public static bool IsAccountHistoryExist(PXGraph graph, int? accountID)
		{
			PXSelectBase select = new PXSelect<GLHistory, Where<GLHistory.accountID, Equal<Required<GLHistory.accountID>>>>(graph);
			Object result = select.View.SelectSingle(accountID);
			return (result != null);
		}

		public static bool IsLedgerHistoryExist(PXGraph graph, int? ledgerID)
		{
			PXSelectBase select = new PXSelect<GLHistory, Where<GLHistory.ledgerID, Equal<Required<GLHistory.ledgerID>>>>(graph);
			Object result = select.View.SelectSingle(ledgerID);
			return (result != null);
		}
	}
	public static class FiscalPeriodUtils
	{
		public static string FiscalYear(string aFiscalPeriod)
		{
			return aFiscalPeriod.Substring(0, YEAR_LENGTH);
		}

		public static string PeriodInYear(string aFiscalPeriod)
		{
			return aFiscalPeriod.Substring(YEAR_LENGTH, PERIOD_LENGTH);
		}

		/// <summary>
		/// Attempts to extract integer values of the financial year and the number of the financial period inside the year
		/// from a Financial Period ID (<see cref="FinPeriod.FinPeriodID"/>).
		/// </summary>
		/// <param name="fiscalPeriodID">The ID of the period to extract parts from</param>
		/// <param name="year">Output: the financial year, to which the period belongs</param>
		/// <param name="periodNbr">Output: the number of the period in its financial year</param>
		/// <returns><c>true</c> upon success or <c>false</c> if failed to parse due to incorrect format of the input period ID</returns>
		public static bool TryParse(string fiscalPeriodID, out int year, out int periodNbr)
		{
			try
			{
				year = int.Parse(FiscalPeriodUtils.FiscalYear(fiscalPeriodID));
				periodNbr = int.Parse(FiscalPeriodUtils.PeriodInYear(fiscalPeriodID));
			}
			catch (FormatException)
			{
				year = -1;
				periodNbr = -1;
				return false;
			}
			return true;
		}

		public static string Assemble(string aYear, string aPeriod)
		{
			if (aYear.Length != 4 || aPeriod.Length != 2)
			{
				throw new PXArgumentException((string)null, Messages.YearOrPeriodFormatIncorrect);
			}
			return (aYear + aPeriod);
		}

		public const int YEAR_LENGTH = 4;
		public const int PERIOD_LENGTH = 2;
		public const int FULL_LENGHT = YEAR_LENGTH + PERIOD_LENGTH;
		public const string FirstPeriodOfYear = "01";
		public static FinPeriod FindPrevPeriod(PXGraph graph, string fiscalPeriodId, bool aClosedOrActive )
		{
			FinPeriod nextperiod = null;
			if (!string.IsNullOrEmpty(fiscalPeriodId))
			{
				if (aClosedOrActive)
				{
					nextperiod = PXSelect<FinPeriod,
										Where2<
											Where<FinPeriod.closed, Equal<True>,
											Or<FinPeriod.active, Equal<True>>>,
										And<FinPeriod.finPeriodID,
										Less<Required<FinPeriod.finPeriodID>>>>,
										OrderBy<Desc<FinPeriod.finPeriodID>>
										>.SelectWindowed(graph, 0, 1, fiscalPeriodId);
				}
				else 
				{
					nextperiod = PXSelect<FinPeriod,
									Where<FinPeriod.finPeriodID,
									Less<Required<FinPeriod.finPeriodID>>>,
									OrderBy<Desc<FinPeriod.finPeriodID>>
									>.SelectWindowed(graph, 0, 1, fiscalPeriodId);
				}
			}
			if (nextperiod == null)
			{
				nextperiod = FindLastPeriod(graph, true);

			}
			return nextperiod;
		}
		public static FinPeriod FindNextPeriod(PXGraph graph, string fiscalPeriodId, bool aClosedOrActive)
		{
			FinPeriod nextperiod = null;
			if (!string.IsNullOrEmpty(fiscalPeriodId))
			{
				if(aClosedOrActive){
				nextperiod = PXSelect<FinPeriod,
										Where2<
											Where<FinPeriod.closed, Equal<True>,
											Or<FinPeriod.active, Equal<True>>>,
										And<FinPeriod.finPeriodID,
										Greater<Required<FinPeriod.finPeriodID>>>>,
										OrderBy<Asc<FinPeriod.finPeriodID>>
										>.SelectWindowed(graph, 0, 1, fiscalPeriodId);
				}
				else 
				{
					nextperiod = PXSelect<FinPeriod,
									Where<FinPeriod.finPeriodID,
									Less<Required<FinPeriod.finPeriodID>>>,
									OrderBy<Desc<FinPeriod.finPeriodID>>
									>.SelectWindowed(graph, 0, 1, fiscalPeriodId);
				}
			}
			if (nextperiod == null)
			{
				nextperiod = FindLastPeriod(graph, true);
			}
			return nextperiod;
		}
		public static FinPeriod FindLastPeriod(PXGraph graph, bool aClosedOrActive)
		{
			return (aClosedOrActive ? (PXSelect<FinPeriod,
					Where<FinPeriod.closed, Equal<True>,
											Or<FinPeriod.active, Equal<True>>>,
					OrderBy<Desc<FinPeriod.finPeriodID>>
					>.SelectWindowed(graph, 0, 1)) : (
					PXSelectOrderBy<FinPeriod, OrderBy<Desc<FinPeriod.finPeriodID>>
					>.SelectWindowed(graph, 0, 1)));
		}
		public static void VerifyFinPeriod<fieldPeriod, fieldClosed>(PXGraph graph, PXCache cache, object row, PXSelectBase<FinPeriod> finperiod)
			where fieldPeriod : class, IBqlField
			where fieldClosed : class, IBqlField
		{
			if (finperiod.Current != null)
			{
				GLSetup glsetup = PXSetup<GLSetup>.Select(graph);
				object isClosed = finperiod.Cache.GetValue<fieldClosed>(finperiod.Current);

				if (finperiod.Current.Active != true || isClosed.Equals(true) && glsetup != null && glsetup.PostClosedPeriods != true)
				{
					BqlCommand select = BqlCommand.CreateInstance(typeof(Select<FinPeriod,
						Where<FinPeriod.active, Equal<True>,
							And<FinPeriod.finPeriodID, Greater<Required<FinPeriod.finPeriodID>>>>,
						OrderBy<Asc<FinPeriod.finPeriodID>>>))
						.WhereAnd(BqlCommand.Compose(typeof(Where<,>), typeof(fieldClosed), typeof(NotEqual<True>)));

					object docPeriod = cache.GetValue<fieldPeriod>(row);
					FinPeriod firstopen = new PXView(graph, false, select).SelectSingle(docPeriod) as FinPeriod;
					
					if (firstopen == null)
					{
						string userPeriod = Mask.Format("##-####", finperiod.Cache.GetValueExt<FinPeriod.finPeriodID>(finperiod.Current).ToString());
						throw new PXSetPropertyException(GL.Messages.NoActivePeriodAfter, userPeriod);
					}

					cache.SetValue<fieldPeriod>(row, firstopen.FinPeriodID);
				}
			}
		}
	}

}
