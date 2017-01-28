using PX.CCProcessingBase;
using PX.Data;
using PX.Objects.CA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PX.Objects.AR.CCPaymentProcessing
{
	[Flags]
	public enum CCPaymentState
	{
		None = 0,
		PreAuthorized = 1,
		PreAuthorizationFailed = 2,
		Captured = 4,
		CaptureFailed = 8,
		Voided = 16,
		VoidFailed = 32,
		Refunded = 64,
		RefundFailed = 128,
		PreAuthorizationExpired = 256,
	}

	public interface ICCPayment
	{
		int? PMInstanceID
		{
			get; set;
		}
		decimal? CuryDocBal
		{
			get; set;
		}
		string CuryID
		{
			get; set;
		}
		string DocType
		{
			get; set;
		}
		string RefNbr
		{
			get; set;
		}
		string OrigDocType
		{
			get;
		}
		string OrigRefNbr
		{
			get;
		}
	}

	public interface ICCCapturePayment : ICCPayment
	{
		bool? IsCCCaptured
		{
			get; set;
		}
		bool? IsCCCaptureFailed
		{
			get; set;
		}
		decimal? CuryCCCapturedAmt
		{
			get; set;
		}
	}

	public interface ICCAuthorizePayment : ICCPayment
	{
		bool? IsCCAuthorized
		{
			get; set;
		}
		decimal? CuryCCPreAuthAmount
		{
			get; set;
		}
		DateTime? CCAuthExpirationDate
		{
			get; set;
		}
	}
		
	public static class CCProcessingUtils
	{
		#region Processing Center Methods

		public static IEnumerable getPMdetails(PXGraph graph, int? PMInstanceID, bool CCPIDCondition, bool OtherDetailsCondition)
		{
			foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> res in PXSelectJoin<CustomerPaymentMethodDetail, InnerJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethodDetail.paymentMethodID>,
				And<PaymentMethodDetail.detailID, Equal<CustomerPaymentMethodDetail.detailID>,
					And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
				Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(graph, PMInstanceID))
			{
				PaymentMethodDetail pmd = res;
				bool isCCPid = pmd.IsCCProcessingID == true;
				if (CCPIDCondition && isCCPid 
					|| OtherDetailsCondition && !isCCPid)
				{
					yield return res;
				}
			}
		}

		public static string GetExpirationDateFormat(PXGraph graph, string ProcessingCenterID)
		{
			PXResultset<CCProcessingCenter> pc = PXSelectJoin<CCProcessingCenter, LeftJoin<CCProcessingCenterDetail,
				On<CCProcessingCenterDetail.processingCenterID, Equal<CCProcessingCenter.processingCenterID>,
					And<CCProcessingCenterDetail.detailID, Equal<Required<CCProcessingCenterDetail.detailID>>>>>,
				Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>
				.Select(graph, InterfaceConstants.ExpDateFormatDetailID, ProcessingCenterID);
			if (pc.Count == 0)
			{
				return null;
			}
			CCProcessingCenterDetail detail = pc[0].GetItem<CCProcessingCenterDetail>();
			if (string.IsNullOrEmpty(detail.DetailID))
			{
				return null;
			}
			return detail.Value;
		}
			

		public static bool isCCPIDFilled(PXGraph graph, int? PMInstanceID)
		{
			if (PMInstanceID == null || PMInstanceID.Value < 0)
				return false;

			CustomerPaymentMethod cpm = PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID,
				Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(graph, PMInstanceID);

			if (cpm == null)
				return false;

			PXResultset<PaymentMethodDetail> paymentMethodDetail = PXSelectJoin<PaymentMethodDetail, LeftJoin<CustomerPaymentMethodDetail,
				On<CustomerPaymentMethodDetail.paymentMethodID, Equal<PaymentMethodDetail.paymentMethodID>,
					And<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>,
						And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,
						And<CustomerPaymentMethodDetail.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>>>>,
				Where<PaymentMethodDetail.isCCProcessingID, Equal<True>,
					And<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>>>>.Select(graph, PMInstanceID, cpm.PaymentMethodID);

			PaymentMethodDetail pmIDDetail = paymentMethodDetail.Count > 0 ? paymentMethodDetail[0].GetItem<PaymentMethodDetail>() : null;
			CustomerPaymentMethodDetail ccpIDDetail = paymentMethodDetail.Count > 0 ? paymentMethodDetail[0].GetItem<CustomerPaymentMethodDetail>() : null;

			if (isTokenizedPaymentMethod(graph, PMInstanceID) && pmIDDetail == null)
			{
				throw new PXException(Messages.PaymentMethodNotConfigured);
			}
			return ccpIDDetail != null && !string.IsNullOrEmpty(ccpIDDetail.Value);
		}

		public static bool isTokenizedPaymentMethod(PXGraph graph, int? PMInstanceID)
		{
			return isFeatureSupported(graph, PMInstanceID, CCProcessingFeature.Tokenization);
		}

		public static bool isHFPaymentMethod(PXGraph graph, int? PMInstanceID)
		{
			CustomerPaymentMethod current = getCustomerPaymentMethod(graph, PMInstanceID);
			if (current == null)
				return false;
			CCProcessingCenter processingCenter = getProcessingCenter(graph, current.CCProcessingCenterID);
			return CCPaymentProcessing.IsFeatureSupported(processingCenter, CCProcessingFeature.HostedForm) && (processingCenter.AllowDirectInput != true);
		}

		public static bool isExpirationDateUnmasked(PXGraph graph, int? PMInstanceID)
		{
			return isFeatureSupported(graph, PMInstanceID, CCProcessingFeature.UnmaskedExpirationDate);
		}

		public static bool isFeatureSupported(PXGraph graph, int? PMInstanceID, CCProcessingFeature FeatureName)
		{
			CustomerPaymentMethod current = getCustomerPaymentMethod(graph, PMInstanceID);
			if (current == null)
				return false;
			CCProcessingCenter processingCenter = getProcessingCenter(graph, current.CCProcessingCenterID);
			return CCPaymentProcessing.IsFeatureSupported(processingCenter, FeatureName);
		}

		public static CustomerPaymentMethod getCustomerPaymentMethod(PXGraph graph, int? PMInstanceID)
		{
			CustomerPaymentMethod current = null;
			if (PMInstanceID != null)
			{
				current = PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(graph, PMInstanceID);
			}
			if (current == null)
			{
				//assumin that payment method was just deleted
				IEnumerator cpmEnumerator = graph.Caches[typeof(CustomerPaymentMethod)].Deleted.GetEnumerator();
				if (cpmEnumerator.MoveNext())
				{
					current = (CustomerPaymentMethod)cpmEnumerator.Current;
				}
			}
			return current;
		}

		public static CCProcessingCenter getProcessingCenter(PXGraph graph, string ProcessingCenterID)
		{
			return PXSelect<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>.Select(graph, ProcessingCenterID);
		}

		public static bool? CCProcessingCenterNeedsExpDateUpdate(PXGraph graph, CCProcessingCenter ProcessingCenter)
		{
			if (CCPaymentProcessing.IsFeatureSupported(ProcessingCenter, CCProcessingFeature.UnmaskedExpirationDate))
			{
				PXResultset<CustomerPaymentMethod> unupdatedCpms = PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.cCProcessingCenterID,
					Equal<Required<CustomerPaymentMethod.cCProcessingCenterID>>, And<CustomerPaymentMethod.expirationDate, IsNull>>>.Select(graph, ProcessingCenter.ProcessingCenterID);
				return unupdatedCpms.Count != 0;				
			}
			return null;
		}

		public static string GetTokenizedPMsString(PXGraph graph)
		{
			List<CCProcessingCenter> tokenizedPCs = new List<CCProcessingCenter>();
			HashSet<string> pmSet = new HashSet<string>();
			foreach (CCProcessingCenter pc in PXSelect<CCProcessingCenter, Where<CCProcessingCenter.isActive, Equal<True>>>.Select(graph))
			{
				if (CCPaymentProcessing.IsFeatureSupported(pc, CCProcessingFeature.Tokenization) 
					&& CCProcessingCenterNeedsExpDateUpdate(graph, pc) != false)
				{
					tokenizedPCs.Add(pc);
				}
			}

			foreach (CCProcessingCenter pc in tokenizedPCs)
			{
				foreach (PXResult<CustomerPaymentMethod, PaymentMethod> tokenizedPM in PXSelectJoinGroupBy<CustomerPaymentMethod,
					InnerJoin<PaymentMethod, On<CustomerPaymentMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>,
					Where<CustomerPaymentMethod.cCProcessingCenterID, Equal<Required<CustomerPaymentMethod.cCProcessingCenterID>>>,
					Aggregate<GroupBy<CustomerPaymentMethod.paymentMethodID, GroupBy<PaymentMethod.descr>>>>.Select(graph, pc.ProcessingCenterID))
				{
					PaymentMethod pm = tokenizedPM;
					pmSet.Add(pm.Descr);
				}
			}

			if (pmSet.Count == 0)
			{
				return string.Empty;
			}

			StringBuilder sb = new StringBuilder();

			foreach (string descr in pmSet)
			{
				if (sb.Length > 0)
				{
					sb.Append(", ");
				}
				sb.Append(descr);
			}

			return sb.ToString();
		}
		
		#endregion
	}

	public sealed class DocDateWarningDisplay : IPXCustomInfo
	{
		public void Complete(PXLongRunStatus status, PXGraph graph)
		{
			if (status == PXLongRunStatus.Completed && graph is ARPaymentEntry)
			{
				((ARPaymentEntry)graph).RowSelected.AddHandler<ARPayment>((sender, e) =>
				{
					ARPayment payment = e.Row as ARPayment;
					if (payment != null && payment.Released == false && DateTime.Compare(payment.AdjDate.Value, _NewDate) != 0)
					{
						sender.RaiseExceptionHandling<ARPayment.adjDate>(payment, payment.AdjDate, new PXSetPropertyException(Messages.ApplicationDateChanged, PXErrorLevel.Warning));
					}
				});
			}
		}
		private readonly DateTime _NewDate;
		public DocDateWarningDisplay(DateTime newDate)
		{
			_NewDate = newDate;
		}
	}
}
