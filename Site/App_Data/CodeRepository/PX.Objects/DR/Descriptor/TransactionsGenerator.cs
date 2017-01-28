using System;
using System.Collections.Generic;
using System.Linq;

using PX.Data;
using PX.Objects.CM;
using PX.Objects.DR.Descriptor;
using PX.Objects.GL;
using PX.Common;
using PX.Objects.Common.Extensions;

namespace PX.Objects.DR
{
	public class TransactionsGenerator
	{
		protected PXGraph _graph;
		protected DRDeferredCode _code;
		protected Func<decimal, decimal> _roundingFunction;
		protected IFinancialPeriodProvider _financialPeriodProvider;

		/// <param name="roundingFunction">
		/// An optional parameter specifying a function that would be used to round
		/// the calculated transaction amounts. If <c>null</c>, the generator will use
		/// <see cref="PXDBCurrencyAttribute.BaseRound(PXGraph, decimal)"/> by default.
		/// </param>
		/// <param name="financialPeriodProvider">
		/// An optional parameter specifying an object that would be used to manipulate
		/// financial periods, e.g. extract a start date or an end date for a given period ID.
		/// If <c>null</c>, the generator will use <see cref="FinancialPeriodProvider.Default"/>. 
		/// </param>
		public TransactionsGenerator(
			PXGraph graph, 
			DRDeferredCode code, 
			IFinancialPeriodProvider financialPeriodProvider = null,
			Func<decimal, decimal> roundingFunction = null)
		{
			if (graph == null) throw new ArgumentNullException(nameof(graph));
			if (code == null) throw new ArgumentNullException(nameof(code));

			_graph = graph;
			_code = code;
			_roundingFunction = roundingFunction ?? (rawAmount => PXDBCurrencyAttribute.BaseRound(_graph, rawAmount));
			_financialPeriodProvider = financialPeriodProvider ?? FinancialPeriodProvider.Default;
		}

		public virtual IList<DRScheduleTran> GenerateTransactions(
			DRSchedule deferralSchedule, 
			DRScheduleDetail scheduleDetail,
			int? branchID)
		{
			if (deferralSchedule == null) throw new ArgumentNullException(nameof(deferralSchedule));
			if (scheduleDetail == null) throw new ArgumentNullException(nameof(scheduleDetail));

			ValidateTerms(deferralSchedule);

			decimal defAmount = scheduleDetail.TotalAmt.Value;

			List<DRScheduleTran> list = new List<DRScheduleTran>();

			short lineCounter = 0;
			if (_code.ReconNowPct.Value > 0)
			{
				decimal recNowRaw = defAmount * _code.ReconNowPct.Value * 0.01m;
				decimal recNow = _roundingFunction(recNowRaw);
				defAmount -= recNow;

				lineCounter++;

				DRScheduleTran nowTran = new DRScheduleTran
				{
					BranchID = branchID,
					AccountID = scheduleDetail.AccountID,
					SubID = scheduleDetail.SubID,
					Amount = recNow,
					RecDate = deferralSchedule.DocDate,
					FinPeriodID = scheduleDetail.FinPeriodID,
					LineNbr = lineCounter,
					ScheduleID = scheduleDetail.ScheduleID,
					ComponentID = scheduleDetail.ComponentID,
					Status = 
						_code.Method == DeferredMethodType.CashReceipt ? 
							DRScheduleTranStatus.Projected : 
							DRScheduleTranStatus.Open,
				};

				list.Add(nowTran);
			}

			bool isFlexibleDeferralCode = DeferredMethodType.RequiresTerms(_code.Method);

			DateTime documentDate = deferralSchedule.DocDate.Value;
			string documentFinPeriod = _financialPeriodProvider.PeriodFromDate(documentDate);

			int occurrences = isFlexibleDeferralCode ? 
				CalcOccurrences(deferralSchedule.TermStartDate.Value, deferralSchedule.TermEndDate.Value) :
				_code.Occurrences.Value;

			List<DRScheduleTran> deferredList = new List<DRScheduleTran>(_code.Occurrences.Value);
			string deferredPeriod = null;

			for (int i = 0; i < occurrences; i++)
			{
				try
				{
					if (deferredPeriod == null)
					{
						deferredPeriod = isFlexibleDeferralCode ? 
							_financialPeriodProvider.GetPeriod(deferralSchedule.TermStartDate.Value) : 
							_financialPeriodProvider.PeriodPlusPeriod(_graph, scheduleDetail.FinPeriodID, _code.StartOffset.Value);
					}
					else
					{
						deferredPeriod = _financialPeriodProvider.PeriodPlusPeriod(_graph, deferredPeriod, _code.Frequency.Value);
					}
				}
				catch (PXFinPeriodException ex)
				{
					throw new PXException(ex, Messages.NoFinPeriod, _code.DeferredCodeID);
				}

				lineCounter++;

				DateTime recognitionDate = GetRecognitionDate(
					deferredPeriod,
					minimumDate: isFlexibleDeferralCode ? deferralSchedule.TermStartDate.Value : documentDate,
					maximumDate: isFlexibleDeferralCode ? deferralSchedule.TermEndDate : null);

				DRScheduleTran deferralTransaction = new DRScheduleTran
				{
					BranchID = branchID,
					AccountID = scheduleDetail.AccountID,
					SubID = scheduleDetail.SubID,
					RecDate = recognitionDate,
					FinPeriodID = _financialPeriodProvider.PeriodFromDate(recognitionDate),
					LineNbr = lineCounter,
					ScheduleID = scheduleDetail.ScheduleID,
					ComponentID = scheduleDetail.ComponentID,
					Status =
						_code.Method == DeferredMethodType.CashReceipt ? 
							DRScheduleTranStatus.Projected : 
							DRScheduleTranStatus.Open
				};

				deferredList.Add(deferralTransaction);
			}

			SetAmounts(deferredList, defAmount, deferralSchedule);

			if (DeferredMethodType.RequiresTerms(_code) &&
				_code.RecognizeInPastPeriods != true)
			{
				// Adjust recognition dates and financial periods 
				// that are in the past relative to the document date.
				// -
				foreach (DRScheduleTran transaction in deferredList
					.Where(transaction => transaction.RecDate < documentDate))
				{
					transaction.RecDate = documentDate;
					transaction.FinPeriodID = documentFinPeriod;
				}
			}

			list.AddRange(deferredList);
			
			return list;
		}

		/// <summary>
		/// If applicable, creates a single related transaction for all original posted transactions
		/// whose recognition date is earlier than (or equal to) the current document date.
		/// Does not set any amounts.
		/// </summary>
		/// <param name="transactionList">
		/// Transaction list where the new transaction will be put (if created).
		/// </param>
		/// <param name="lineCounter">
		/// Transaction line counter. Will be incremented if any transactions are created by this procedure.
		/// </param>
		private void AddRelatedTransactionForPostedBeforeDocumentDate(
			IList<DRScheduleTran> transactionList,
			DRScheduleDetail relatedScheduleDetail,
			IEnumerable<DRScheduleTran> originalPostedTransactions,
			int? branchID,
			ref short lineCounter)
		{
			IEnumerable<DRScheduleTran> originalTransactionsPostedBeforeDocumentDate =
				originalPostedTransactions.Where(transaction => transaction.RecDate <= relatedScheduleDetail.DocDate);

			if (originalTransactionsPostedBeforeDocumentDate.Any())
			{
				++lineCounter;

				DRScheduleTran relatedTransaction = new DRScheduleTran
				{
					BranchID = branchID,
					AccountID = relatedScheduleDetail.AccountID,
					SubID = relatedScheduleDetail.SubID,
					RecDate = relatedScheduleDetail.DocDate,
					FinPeriodID = relatedScheduleDetail.FinPeriodID,
					LineNbr = lineCounter,
					ScheduleID = relatedScheduleDetail.ScheduleID,
					ComponentID = relatedScheduleDetail.ComponentID,
					Status = DRScheduleTranStatus.Open
				};

				transactionList.Add(relatedTransaction);
			}
		}

		/// <summary>
		/// Adds a related transaction for every original transaction
		/// in <paramref name="originalTransactions"/> using information
		/// from the provided related <see cref="DRScheduleDetail"/>.
		/// Does not set any transaction amounts.
		/// </summary>
		/// <param name="transactionList">
		/// Transaction list where the new transaction will be put (if created).
		/// </param>
		/// <param name="lineCounter">
		/// Transaction line counter. Will be incremented if any transactions are created by this procedure.
		/// </param>
		private void AddRelatedTransactions(
			IList<DRScheduleTran> transactionList,
			DRScheduleDetail relatedScheduleDetail,
			IEnumerable<DRScheduleTran> originalTransactions,
			int? branchID,
			ref short lineCounter)
		{
			foreach (DRScheduleTran originalTransaction in originalTransactions)
			{
				++lineCounter;

				DRScheduleTran relatedTransaction = new DRScheduleTran
				{
					BranchID = branchID,
					AccountID = relatedScheduleDetail.AccountID,
					SubID = relatedScheduleDetail.SubID,
					LineNbr = lineCounter,
					ScheduleID = relatedScheduleDetail.ScheduleID,
					ComponentID = relatedScheduleDetail.ComponentID,
					Status = DRScheduleTranStatus.Open
				};

				var maxDate = originalTransaction.RecDate.Value < relatedScheduleDetail.DocDate.Value
					? relatedScheduleDetail.DocDate
					: originalTransaction.RecDate;

				relatedTransaction.RecDate = maxDate;

				var maxPeriod = string.CompareOrdinal(originalTransaction.FinPeriodID, relatedScheduleDetail.FinPeriodID) < 0
					? relatedScheduleDetail.FinPeriodID
					: originalTransaction.FinPeriodID;

				relatedTransaction.FinPeriodID = maxPeriod;

				transactionList.Add(relatedTransaction);
			}
		}

		/// <summary>
		/// During the related transaction generation, checks that the original open transaction
		/// collection doesn't contain any non-open transactions.
		/// </summary>
		private static void ValidateOpenTransactions(IEnumerable<DRScheduleTran> originalOpenTransactions)
		{
			if (originalOpenTransactions == null) return;

			if (originalOpenTransactions.Any(transaction => 
				transaction.Status != DRScheduleTranStatus.Open &&
				transaction.Status != DRScheduleTranStatus.Projected))
			{
				throw new PXArgumentException(
					nameof(originalOpenTransactions),
					Messages.CollectionContainsPostedTransactions);
			}
		}

		/// <summary>
		/// During the related transaction generation, checks that the original posted transaction
		/// collection doesn't contain any non-posted transactions.
		/// </summary>
		private static void ValidatePostedTransactions(IEnumerable<DRScheduleTran> originalPostedTransactions)
		{
			if (originalPostedTransactions == null) return;

			if (originalPostedTransactions.Any(transaction => transaction.Status != DRScheduleTranStatus.Posted))
			{
				throw new PXArgumentException(
					nameof(originalPostedTransactions),
					Messages.CollectionContainsNonPostedTransactions);
			}
		}

		/// <summary>
		/// Generates the related transactions given the list of original 
		/// </summary>
		/// <param name="relatedScheduleDetail">
		/// The schedule detail
		/// to which the related transactions will pertain.
		/// </param>
		/// <param name="originalOpenTransactions">
		/// Original transactions in the Open (or Projected) status.
		/// </param>
		/// <param name="originalPostedTransactions">
		/// Original transactions in the Posted status.
		/// </param>
		/// <param name="amountToDistributeForUnposted">
		/// Amount to distribute among the related transactions that are
		/// created for original Open transactions.
		/// </param>
		/// <param name="amountToDistributeForPosted">
		/// Amount to distribute among the related transactions that are
		/// created for original Posted transactions.
		/// </param>
		/// <param name="branchID">
		/// Branch ID for the related transactions.
		/// </param>
		/// <returns></returns>
		public virtual IList<DRScheduleTran> GenerateRelatedTransactions(
			DRScheduleDetail relatedScheduleDetail,
			IEnumerable<DRScheduleTran> originalOpenTransactions,
			IEnumerable<DRScheduleTran> originalPostedTransactions,
			decimal amountToDistributeForUnposted,
			decimal amountToDistributeForPosted,
			int? branchID)
		{
			ValidateOpenTransactions(originalOpenTransactions);
			ValidatePostedTransactions(originalPostedTransactions);

			List<DRScheduleTran> transactionList = new List<DRScheduleTran>();

			short lineCounter = 0;
			int transactionsAddedDuringPreviousStep;

			// Handle posted transactions
			// -
			if (originalPostedTransactions != null && originalPostedTransactions.Any())
			{
				decimal originalPostedTransactionsTotal =
					originalPostedTransactions.Sum(transaction => transaction.Amount ?? 0);

				AddRelatedTransactionForPostedBeforeDocumentDate(
					transactionList,
					relatedScheduleDetail,
					originalPostedTransactions,
					branchID,
					ref lineCounter);

				transactionsAddedDuringPreviousStep = lineCounter;

				decimal originalPostedBeforeDocumentDateSum = originalPostedTransactions
					.Where(transaction => transaction.RecDate <= relatedScheduleDetail.DocDate)
					.Sum(transaction => transaction.Amount ?? 0);

				decimal multiplier = amountToDistributeForPosted / originalPostedTransactionsTotal;

				// Amount to be distributed across other transactions that
				// are related to the original posted transactions.
				// -
				decimal residualPostedAmount = amountToDistributeForPosted;

				if (transactionList.Any())
				{
					transactionList[0].Amount = multiplier * originalPostedBeforeDocumentDateSum;
					residualPostedAmount -= transactionList[0].Amount ?? 0;
				}

				IEnumerable<DRScheduleTran> originalPostedAfterDocumentDate =
					originalPostedTransactions.Where(transaction => transaction.RecDate > relatedScheduleDetail.DocDate);

				AddRelatedTransactions(
					transactionList,
					relatedScheduleDetail,
					originalPostedAfterDocumentDate,
					branchID,
					ref lineCounter);
				
				// Set amounts for related transactions
				// -
				decimal relatedTransactionTotal = 0;
				
				if (originalPostedAfterDocumentDate.Any())
				{
					originalPostedAfterDocumentDate.SkipLast(1).ForEach((originalTransaction, i) =>
					{
						decimal rawTransactionAmount = multiplier * originalTransaction.Amount ?? 0;
						transactionList[transactionsAddedDuringPreviousStep + i].Amount = _roundingFunction(rawTransactionAmount);

						relatedTransactionTotal += transactionList[transactionsAddedDuringPreviousStep + i].Amount ?? 0;
					});

					transactionList[transactionList.Count - 1].Amount = residualPostedAmount - relatedTransactionTotal;
				}
			}

			transactionsAddedDuringPreviousStep = lineCounter;

			// Handle open transactions
			// -
			if (originalOpenTransactions != null && originalOpenTransactions.Any())
			{
				decimal originalOpenTransactionsTotal = 
					originalOpenTransactions.Sum(transaction => transaction.Amount ?? 0);

				AddRelatedTransactions(
					transactionList,
					relatedScheduleDetail,
					originalOpenTransactions,
					branchID,
					ref lineCounter);

				// Set amounts for related transactions
				// -
				decimal multiplier = amountToDistributeForUnposted / originalOpenTransactionsTotal;
				decimal relatedTransactionTotal = 0;

				originalOpenTransactions.SkipLast(1).ForEach((originalTransaction, i) =>
				{
					decimal rawTransactionAmount = multiplier * originalTransaction.Amount.Value;
					transactionList[transactionsAddedDuringPreviousStep + i].Amount = _roundingFunction(rawTransactionAmount);

					relatedTransactionTotal += transactionList[transactionsAddedDuringPreviousStep + i].Amount ?? 0;
				});

				transactionList[transactionList.Count - 1].Amount = amountToDistributeForUnposted - relatedTransactionTotal;
			}
			else if (amountToDistributeForUnposted > 0)
			{
				++lineCounter;

				DRScheduleTran deferralTransaction = new DRScheduleTran
				{
					Amount = amountToDistributeForUnposted,
					BranchID = branchID,
					AccountID = relatedScheduleDetail.AccountID,
					SubID = relatedScheduleDetail.SubID,
					RecDate = relatedScheduleDetail.DocDate,
					FinPeriodID = relatedScheduleDetail.FinPeriodID,
					LineNbr = lineCounter,
					ScheduleID = relatedScheduleDetail.ScheduleID,
					ComponentID = relatedScheduleDetail.ComponentID,
					Status = DRScheduleTranStatus.Open
				};

				transactionList.Add(deferralTransaction);
			}

			return transactionList;
		}

		/// <summary>
		/// Checks the presence and consistency of deferral term start / end dates,
		/// as well as ensures that the document date is no later than the Term End Date 
		/// in case recognizing in past periods is forbidden. 
		/// </summary>
		/// <param name="deferralSchedule">
		/// Deferral schedule from which the document date will be taken.
		/// </param>
		protected virtual void ValidateTerms(DRSchedule deferralSchedule)
		{
			if (!DeferredMethodType.RequiresTerms(_code)) return;

			if (!deferralSchedule.TermStartDate.HasValue ||
				!deferralSchedule.TermEndDate.HasValue)
			{
				throw new PXException(Messages.CannotGenerateTransactionsWithoutTerms);
			}

			if (deferralSchedule.TermStartDate > deferralSchedule.TermEndDate)
			{
				throw new PXException(
					Messages.TermCantBeNegative, 
					deferralSchedule.TermEndDate, 
					deferralSchedule.TermStartDate);
			}

			if (_code.RecognizeInPastPeriods != true &&
				deferralSchedule.DocDate > deferralSchedule.TermEndDate)
			{
				throw new PXException(Messages.TermEndDateIsEarlierThanDocDate, _code.DeferredCodeID);
			}
		}

		protected virtual int CalcOccurrences(DateTime startDate, DateTime endDate)
		{
			return _financialPeriodProvider
				.PeriodsBetweenInclusive(_graph, startDate, endDate)
				.Where((_, periodIndex) => periodIndex % _code.Frequency == 0)
				.Count();
		}

		/// <summary>
		/// Returns the appropriate recognition date in a given financial period,
		/// taking into account the <see cref="DRScheduleOption"/> settings of the
		/// deferral code, as well as the absolute recognition date boundaries provided
		/// as arguments to this method.
		/// </summary>
		/// <param name="finPeriod">The financial period in which recognition must happen.</param>
		/// <param name="minimumDate">The earliest date where recognition is allowed.</param>
		/// <param name="maximumDate">The latest date where recognition is allowed.</param>
		/// <returns></returns>
		protected DateTime GetRecognitionDate(string finPeriod, DateTime minimumDate, DateTime? maximumDate)
		{
			DateTime date = minimumDate;

			switch (_code.ScheduleOption)
			{
				case DRScheduleOption.ScheduleOptionStart:
					date = _financialPeriodProvider.PeriodStartDate(_graph, finPeriod);
					break;

				case DRScheduleOption.ScheduleOptionEnd:
					date = _financialPeriodProvider.PeriodEndDate(_graph, finPeriod);
					break;

				case DRScheduleOption.ScheduleOptionFixedDate:
					DateTime startDate = _financialPeriodProvider.PeriodStartDate(_graph, finPeriod);
					DateTime endDate = _financialPeriodProvider.PeriodEndDate(_graph, finPeriod);

					if (_code.FixedDay.Value <= startDate.Day)
						date = startDate;
					else if (_code.FixedDay.Value >= endDate.Day)
						date = endDate;
					else
						date = new DateTime(startDate.Year, startDate.Month, _code.FixedDay.Value);
					break;
			}

			if (date < minimumDate && _code.StartOffset >= 0)
				return minimumDate;
			else if (date > maximumDate)
				return maximumDate.Value;

			return date;
		}

		private void SetAmounts(IList<DRScheduleTran> deferredTransactions, decimal deferredAmount, DRSchedule schedule)
		{
			switch (_code.Method)
			{
				case DeferredMethodType.CashReceipt:
				case DeferredMethodType.EvenPeriods:
					SetAmountsEvenPeriods(deferredTransactions, deferredAmount);
					break;
				case DeferredMethodType.ProrateDays:
					SetAmountsProrateDays(deferredTransactions, deferredAmount, schedule.DocDate.Value);
					break;
				case DeferredMethodType.ExactDays:
					SetAmountsExactDays(deferredTransactions, deferredAmount);
					break;
				case DeferredMethodType.FlexibleProrateDays:
					SetAmountsFlexibleProrateByDays(deferredTransactions, deferredAmount, schedule.TermStartDate.Value, schedule.TermEndDate.Value);
					break;
				case DeferredMethodType.FlexibleExactDays:
					SetAmountsFlexibleByDays(deferredTransactions, deferredAmount, schedule.TermStartDate.Value, schedule.TermEndDate.Value);
					break;
			}
		}

		private void SetAmountsEvenPeriods(IList<DRScheduleTran> deferredTransactions, decimal deferredAmount)
		{
			if (deferredTransactions.Count > 0)
			{
				decimal amtRaw = deferredAmount / deferredTransactions.Count;
				decimal amt = _roundingFunction(amtRaw);
				decimal recAmt = 0;
				for (int i = 0; i < deferredTransactions.Count - 1; i++)
				{
					deferredTransactions[i].Amount = amt;
					recAmt += amt;
				}

				deferredTransactions[deferredTransactions.Count - 1].Amount = deferredAmount - recAmt;
			}
		}

		private void SetAmountsProrateDays(
			IList<DRScheduleTran> deferredTransactions, 
			decimal deferredAmount, 
			DateTime documentDate)
		{
			if (deferredTransactions.Count > 0)
			{
				if (deferredTransactions.Count == 1)
				{
					deferredTransactions[0].Amount = deferredAmount;
				}
				else
				{
					string currentPeriod = _financialPeriodProvider.PeriodFromDate(documentDate);
					DateTime startDateOfCurrentPeriod = _financialPeriodProvider.PeriodStartDate(_graph, currentPeriod);

					if (_code.StartOffset > 0 || startDateOfCurrentPeriod == documentDate)
					{
						SetAmountsEvenPeriods(deferredTransactions, deferredAmount);
					}
					else
					{
						// Returns the last day of the month with time 12:00 AM!
						// -
						DateTime endDateOfCurrentPeriod = _financialPeriodProvider.PeriodEndDate(_graph, currentPeriod);
						TimeSpan spanOfCurrentPeriod = endDateOfCurrentPeriod.Subtract(startDateOfCurrentPeriod);

						// One is added to the time because 30.12.2009 12:00 AM minus 01.12.2009 12:00 AM will give 29.
						// -
						int daysInPeriod = spanOfCurrentPeriod.Days + 1;

						TimeSpan span = endDateOfCurrentPeriod.Subtract(documentDate);

						// +1?
						// -
						int daysInFirstPeriod = span.Days;

						decimal amtRaw = deferredAmount / (deferredTransactions.Count - 1);
						decimal firstRaw = amtRaw * daysInFirstPeriod / daysInPeriod;

						decimal amt = _roundingFunction(amtRaw);
						decimal firstAmt = _roundingFunction(firstRaw);

						deferredTransactions[0].Amount = firstAmt;

						decimal recAmt = firstAmt;
						for (int i = 1; i < deferredTransactions.Count - 1; i++)
						{
							deferredTransactions[i].Amount = amt;
							recAmt += amt;
						}

						deferredTransactions[deferredTransactions.Count - 1].Amount = deferredAmount - recAmt;
					}
				}
			}
		}

		private void SetAmountsExactDays(IList<DRScheduleTran> deferredTransactions, decimal deferredAmount)
		{
			int totalDays = 0;
			foreach (DRScheduleTran row in deferredTransactions)
			{
				DateTime endDateOfPeriod = _financialPeriodProvider.PeriodEndDate(_graph, row.FinPeriodID);
				DateTime startDateOfPeriod = _financialPeriodProvider.PeriodStartDate(_graph, row.FinPeriodID);
				TimeSpan spanOfPeriod = endDateOfPeriod.Subtract(startDateOfPeriod);
				totalDays += spanOfPeriod.Days + 1;
			}

			decimal amountPerDay = deferredAmount / totalDays;

			decimal recAmt = 0;
			for (int i = 0; i < deferredTransactions.Count - 1; i++)
			{
				DateTime endDateOfPeriod = _financialPeriodProvider.PeriodEndDate(_graph, deferredTransactions[i].FinPeriodID);
				DateTime startDateOfPeriod = _financialPeriodProvider.PeriodStartDate(_graph, deferredTransactions[i].FinPeriodID);
				TimeSpan spanOfPeriod = endDateOfPeriod.Subtract(startDateOfPeriod);

				decimal amountRaw = (spanOfPeriod.Days + 1) * amountPerDay;
				decimal amount = _roundingFunction(amountRaw);

				deferredTransactions[i].Amount = amount;
				recAmt += amount;
			}

			deferredTransactions[deferredTransactions.Count - 1].Amount = deferredAmount - recAmt;
		}

		private void SetAmountsFlexibleByDays(IList<DRScheduleTran> deferredTransactions, decimal deferredAmount, DateTime startDate, DateTime endDate)
		{
			if (deferredTransactions.Any() == false)
				return;

			var days = (int)(endDate - startDate).TotalDays + 1;
			var dailyRate = deferredAmount / days;

			if(deferredTransactions.Count > 1)
			{
				var firstTran = deferredTransactions[0];
				var secondTran = deferredTransactions[1];
				int daysUnderFirstTran = (int)_financialPeriodProvider.PeriodStartDate(_graph, secondTran.FinPeriodID).Subtract(startDate).TotalDays;
				firstTran.Amount = _roundingFunction(daysUnderFirstTran * dailyRate);
			}

			var transactionsButFirst = deferredTransactions.Skip(1);
			foreach (var item in transactionsButFirst.Zip(transactionsButFirst.Skip(1), (tran, next) => new { Tran = tran, NextTran = next }))
			{
				var start = _financialPeriodProvider.PeriodStartDate(_graph, item.Tran.FinPeriodID);
				var end = _financialPeriodProvider.PeriodStartDate(_graph, item.NextTran.FinPeriodID);
				int daysCovered = (int)end.Subtract(start).TotalDays;

				item.Tran.Amount = _roundingFunction(daysCovered * dailyRate);
			}

			var lastTran = deferredTransactions.Last();
			lastTran.Amount = 0; //to ensure correct Sum below
			lastTran.Amount = deferredAmount - deferredTransactions.Sum(t => t.Amount);
		}

		private void SetAmountsFlexibleProrateByDays(IList<DRScheduleTran> deferredTransactions, decimal deferredAmount, DateTime startDate, DateTime endDate)
		{
			var coveredPeriods = _financialPeriodProvider.PeriodsBetweenInclusive(
				_graph,
				startDate, 
				endDate);

			var firstPeriodPortion = 1.0m - PeriodProportionByDays(startDate, shift: -1);
			var lastPeriodPortion = PeriodProportionByDays(endDate);

			var periodsTotal = coveredPeriods.Count() == 1 ? 1
				: coveredPeriods.Count() - 2 + firstPeriodPortion + lastPeriodPortion;

			var periodlyRate = deferredAmount / periodsTotal;

			foreach (var item in deferredTransactions.Zip(deferredTransactions.Skip(1), (tran, next) => new { Tran = tran, NextTran = next }))
			{
				var periodsCount = (decimal)coveredPeriods
					.SkipWhile(p => p.FinPeriodID != item.Tran.FinPeriodID)
					.TakeWhile(p => p.FinPeriodID != item.NextTran.FinPeriodID)
					.Count();

				if (item.Tran.FinPeriodID == coveredPeriods.First().FinPeriodID)
				{
					periodsCount += firstPeriodPortion - 1.0m;
				}

				item.Tran.Amount = _roundingFunction(periodlyRate * periodsCount);
			}

			var lastTran = deferredTransactions.Last();
			lastTran.Amount = 0;
			var lastAmount = deferredAmount - deferredTransactions.Sum(t => t.Amount);
			lastTran.Amount = _roundingFunction(lastAmount.Value);
		}

		#region Periods and Dates

		private decimal PeriodProportionByDays(DateTime date, int shift = 0)
		{
			var periodID = _financialPeriodProvider.PeriodFromDate(date);
			var start = _financialPeriodProvider.PeriodStartDate(_graph, periodID);
			var end = _financialPeriodProvider.PeriodEndDate(_graph, periodID);

			int days = (int)end.Subtract(start).TotalDays + 1;
			return (decimal)(date.Subtract(start).TotalDays + 1 + shift) / days;
		}

		#endregion
	}
}
