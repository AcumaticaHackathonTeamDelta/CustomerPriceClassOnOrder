using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;

namespace PX.Objects.Common
{
	public static class BalanceCalculation
	{
		/// <summary>
		/// For an application of a payment to an invoice, calculates the 
		/// payment's document balance in the currency of the target invoice 
		/// document.
		/// </summary>
		/// <param name="paymentBalanceInBase">
		/// The balance of the payment document in the base currency.
		/// </param>
		/// <param name="paymentBalanceInCurrency">
		/// The balance of the payment document in the document's currency.
		/// Will be re-used in case when the invoice's currency is the same
		/// as the payment's currency.
		/// </param>
		public static decimal CalculateApplicationDocumentBalance(
			PXCache cache,
			CurrencyInfo paymentCurrencyInfo,
			CurrencyInfo invoiceCurrencyInfo,
			decimal? paymentBalanceInBase,
			decimal? paymentBalanceInCurrency)
		{
			decimal applicationDocumentBalance;

			if (String.Equals(paymentCurrencyInfo.CuryID, invoiceCurrencyInfo.CuryID))
			{
				applicationDocumentBalance = paymentBalanceInCurrency ?? 0m;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvCury(
					cache,
					invoiceCurrencyInfo,
					paymentBalanceInBase ?? 0m,
					out applicationDocumentBalance);
			}

			return applicationDocumentBalance;
		}

		/// <summary>
		/// For an application of a payment to an invoice, calculates the 
		/// value of the <see cref="ARAdjust.CuryDocBal"/> field, which is
		/// the remaining balance of the applied payment.
		/// </summary>
		public static void CalculateApplicationDocumentBalance(
			PXCache cache,
			ARPayment payment,
			IAdjustment application,
			CurrencyInfo paymentCurrencyInfo,
			CurrencyInfo invoiceCurrencyInfo)
		{
			decimal CuryDocBal = CalculateApplicationDocumentBalance(
				cache,
				paymentCurrencyInfo,
				invoiceCurrencyInfo,
				payment.DocBal,
				payment.CuryDocBal);

			if (application == null) return;

			if (application.Released == false)
			{
				if (application.CuryAdjdAmt > CuryDocBal)
				{
					// TODO: if reconsidered need to calculate RGOL
					// -
					application.CuryDocBal = CuryDocBal;
					application.CuryAdjdAmt = 0m;
				}
				else
				{
					application.CuryDocBal = CuryDocBal - application.CuryAdjdAmt;
				}
			}
			else
			{
				application.CuryDocBal = CuryDocBal;
			}
		}

		/// <summary>
		/// Forces the document's control total amount to be equal to the 
		/// document's outstanding balance, afterwards updating the
		/// record in the relevant cache.
		/// </summary>
		public static void ForceDocumentControlTotals(
			PXGraph graph,
			IInvoice invoice)
		{
			if (invoice.CuryOrigDocAmt != invoice.CuryDocBal)
			{
				invoice.CuryOrigDocAmt = invoice.CuryDocBal ?? 0m;
				graph.Caches[invoice.GetType()].Update(invoice);
			}
		}
	}
}
