using PX.CCProcessingBase;
using PX.Data;
using PX.Objects.CA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PX.Objects.AR.CCPaymentProcessing
{
	public class CCCustomerInformationManager
	{

		private class strComparer : IComparer<string>
		{
			int IComparer<string>.Compare(string x, string y)
			{
				return x.Length == y.Length ? (-1) * x.CompareTo(y) : (-1) * x.Length.CompareTo(y.Length);
			}
		}

		private CCPaymentProcessing _processingGraph;

		private string _processingCenterID;

		private CCProcessingFeature _feature;

		public ProcessingContext Context
		{
			get; protected set;
		}

		private ICCPaymentProcessing _processor
		{
			get
			{
				return _processingGraph.initializeProcessing(_processingCenterID, _feature, Context);
			}
		}

		public CCCustomerInformationManager(string processingCenterID, CCProcessingFeature feature, ProcessingContext context)
		{
			_processingGraph = PXGraph.CreateInstance<CCPaymentProcessing>();
			_processingCenterID = processingCenterID;
			_feature = feature;
			Context = context;
			_processingGraph.checkProcessing(_processingCenterID, _feature, Context);
		}

		public void ProcessAPIResponse(APIResponse response)
		{
			_processingGraph.ProcessAPIResponse(response);
		}

		public void SetNewContext(ProcessingContext newContext)
		{
			Context = newContext;
		}

		public virtual string CreateCustomer()
		{
			APIResponse response = new APIResponse();
			string res;
			((ICCTokenizedPaymnetProcessing)_processor).CreateCustomer(response, out res);
			ProcessAPIResponse(response);
			return res;
		}

		public virtual bool CheckCustomerID()
		{
			APIResponse response = new APIResponse();
			((ICCTokenizedPaymnetProcessing)_processor).CheckCustomerID(response);
			ProcessAPIResponse(response);
			return response.isSucess;
		}

		public virtual void DeleteCustomerID()
		{
			APIResponse response = new APIResponse();
			((ICCTokenizedPaymnetProcessing)_processor).DeleteCustomer(response);
			ProcessAPIResponse(response);
		}

		public virtual string CreatePaymentMethod()
		{
			string newID;
			APIResponse response = new APIResponse();
			((ICCTokenizedPaymnetProcessing)_processor).CreatePMI(response, out newID);
			ProcessAPIResponse(response);
			return newID;
		}

		public virtual SyncPMResponse GetPaymentMethod()
		{
			APIResponse apiResponse = new APIResponse();
			SyncPMResponse syncResponse = new SyncPMResponse();
			((ICCTokenizedPaymnetProcessing)_processor).GetPMI(apiResponse, syncResponse);
			ProcessAPIResponse(apiResponse);
			return syncResponse;
		}

		public virtual void DeletePaymentMethod()
		{
			APIResponse apiResponse = new APIResponse();
			((ICCTokenizedPaymnetProcessing)_processor).DeletePMI(apiResponse);
			ProcessAPIResponse(apiResponse);
		}

		public virtual void CreatePaymentMethodHostedForm()
		{
			APIResponse apiResponse = new APIResponse();
			string callbackURL = ((ICCProcessingHostedForm)_processor).GetCallbackURL();
			((ICCProcessingHostedForm)_processor).CreatePaymentMethodHostedForm(apiResponse, callbackURL);
			ProcessAPIResponse(apiResponse);
		}

		public virtual SyncPMResponse SynchronizePaymentMethods()
		{
			APIResponse apiResponse = new APIResponse();
			SyncPMResponse syncResponse = new SyncPMResponse();
			((ICCProcessingHostedForm)_processor).SynchronizePaymentMethods(apiResponse, syncResponse);
			ProcessAPIResponse(apiResponse);
			return syncResponse;
		}

		public virtual void ManagePaymentMethodHostedForm()
		{
			APIResponse apiResponse = new APIResponse();
			string callbackURL = ((ICCProcessingHostedForm)_processor).GetCallbackURL();
			((ICCProcessingHostedForm)_processor).ManagePaymentMethodHostedForm(apiResponse, callbackURL);
		}

		public static void CreatePaymentMethodHF<TPaymentMethodType>(PXGraph graph, PXSelectBase<TPaymentMethodType> customerPaymentMethodView, TPaymentMethodType currentCutomerPaymenMethod)
			where TPaymentMethodType : CustomerPaymentMethod, new()
		{
			if (graph == null || customerPaymentMethodView == null || currentCutomerPaymenMethod == null)
				return;
			CCCustomerInformationManager cim = new CCCustomerInformationManager(currentCutomerPaymenMethod.CCProcessingCenterID, CCProcessingFeature.HostedForm,
				new ProcessingContext() { aCustomerID = currentCutomerPaymenMethod.BAccountID, aPMInstanceID = currentCutomerPaymenMethod.PMInstanceID, callerGraph = graph });
			if (currentCutomerPaymenMethod.CustomerCCPID == null)
			{
				string id = cim.CreateCustomer();
				TPaymentMethodType cpm = (TPaymentMethodType)customerPaymentMethodView.Cache.CreateCopy(currentCutomerPaymenMethod);
				cpm.CustomerCCPID = id;
				customerPaymentMethodView.Update(cpm);
			}
			cim.CreatePaymentMethodHostedForm();
		}

		public static PXResultset<CustomerPaymentMethodDetail> GetAllCustomersCardsInProcCenter(PXGraph graph, int? BAccountID, string CCProcessingCenterID)
		{
			return PXSelectJoin<CustomerPaymentMethodDetail,
						InnerJoin<PaymentMethodDetail, On<CustomerPaymentMethodDetail.paymentMethodID, Equal<PaymentMethodDetail.paymentMethodID>,
							And<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>>>,
						InnerJoin<CustomerPaymentMethod,
							On<CustomerPaymentMethodDetail.pMInstanceID, Equal<CustomerPaymentMethod.pMInstanceID>>>>,
						Where<CustomerPaymentMethod.bAccountID, Equal<Required<CustomerPaymentMethod.bAccountID>>,
							And<CustomerPaymentMethod.cCProcessingCenterID, Equal<Required<CustomerPaymentMethod.cCProcessingCenterID>>,
							And<PaymentMethodDetail.isCCProcessingID, Equal<True>>>>>.Select(graph, BAccountID, CCProcessingCenterID);
		}

		public static void SyncPaymentMethodsHF<TPaymentMethodType, TDetialsType>(PXGraph graph, PXSelectBase<TPaymentMethodType> customerPaymentMethodView,
																				PXSelectBase<TDetialsType> detailsView, TPaymentMethodType currentCustomerPaymentMethod)
			where TPaymentMethodType : CustomerPaymentMethod, new()
			where TDetialsType : CustomerPaymentMethodDetail, new()
		{
			if (graph == null || customerPaymentMethodView == null || detailsView == null || currentCustomerPaymentMethod == null)
				return;
			CCCustomerInformationManager cim = new CCCustomerInformationManager(currentCustomerPaymentMethod.CCProcessingCenterID, CCProcessingFeature.HostedForm,
				new ProcessingContext() { aCustomerID = currentCustomerPaymentMethod.BAccountID, aPMInstanceID = currentCustomerPaymentMethod.PMInstanceID, callerGraph = graph });
			int attempt = 1;
			string newPMID = string.Empty;
			//AuthorizeNet sometimes failes to process new card in time when using Hosted Form Method
			while ((attempt <= (cim.Context.processingCenter.SyncRetryAttemptsNo ?? 0) + 1) && string.IsNullOrEmpty(newPMID))
			{
				Thread.Sleep(cim.Context.processingCenter.SyncRetryDelayMs ?? 0);
				SyncPMResponse syncResponse = cim.SynchronizePaymentMethods();
				TPaymentMethodType customerPaymentMethod = customerPaymentMethodView.Current;
				if (syncResponse.PMList != null && syncResponse.PMList.Count > 0)
				{
					var sortedPMDict = new SortedDictionary<string, Dictionary<string, string>>(syncResponse.PMList, new strComparer());
					PXResultset<CustomerPaymentMethodDetail> otherCards = GetAllCustomersCardsInProcCenter(graph, customerPaymentMethod.BAccountID, currentCustomerPaymentMethod.CCProcessingCenterID);
					foreach (string pmID in sortedPMDict.Keys)
					{
						bool detailExists = false;
						foreach (CustomerPaymentMethodDetail detail in otherCards)
						{
							if (detail.Value == pmID)
							{
								detailExists = true;
								break;
							}
						}
						if (!detailExists)
						{
							newPMID = pmID;
							break;
						}
					}
					if (!string.IsNullOrEmpty(newPMID))
					{
						//Authorize.Net does not return unmasked dates when all customer cards are requested, only when a single card is requested by id
						Dictionary<string, string> resultDict = new Dictionary<string, string>();
						resultDict = sortedPMDict[newPMID];
						string expirationDate = String.Empty;
						if (CCProcessingUtils.isFeatureSupported(graph, customerPaymentMethod.PMInstanceID, CCProcessingFeature.UnmaskedExpirationDate) &&
							sortedPMDict[newPMID].TryGetValue(((ICreditCardDataReader)cim._processingGraph).Key_CardExpiryDate, out expirationDate))
						{
							DateTime? expDate = CustomerPaymentMethodMaint.ParseExpiryDate(graph, customerPaymentMethod, expirationDate);
							if (expDate == null)
							{
								string CCPID = resultDict.Where(kvp => kvp.Value == newPMID).First().Key;
								foreach (TDetialsType det in detailsView.Select())
								{
									if (det.DetailID == CCPID)
									{
										det.Value = newPMID;
										detailsView.Update(det);
									}
								}
								SyncPMResponse customerPaymentProfile = cim.GetPaymentMethod();
								if (customerPaymentProfile.PMList != null && syncResponse.PMList.Count > 0)
								{
									resultDict = customerPaymentProfile.PMList.FirstOrDefault().Value;
								}
							}
						}
						foreach (TDetialsType det in detailsView.Select())
						{
							if (resultDict.ContainsKey(det.DetailID))
							{
								det.Value = resultDict[det.DetailID];
								detailsView.Update(det);
							}
						}
					}
				}
				attempt++;
			}
			if (string.IsNullOrEmpty(newPMID))
			{
				throw new PXException(Messages.FailedToSyncCC);
			}
		}

		public static void ManagePaymentMethodHF<TPaymentMethodType>(PXGraph graph, TPaymentMethodType currentCutomerPaymenMethod)
			where TPaymentMethodType : CustomerPaymentMethod, new()
		{
			if (graph == null || currentCutomerPaymenMethod == null)
				return;
			CustomerPaymentMethodDetail ccpID = PXSelectJoin<CustomerPaymentMethodDetail, InnerJoin<PaymentMethodDetail,
						On<CustomerPaymentMethodDetail.paymentMethodID, Equal<PaymentMethodDetail.paymentMethodID>, And<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>,
							And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
						Where<PaymentMethodDetail.isCCProcessingID, Equal<True>, And<CustomerPaymentMethodDetail.pMInstanceID,
							Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>>.SelectWindowed(graph, 0, 1, currentCutomerPaymenMethod.PMInstanceID);
			if (ccpID != null && !string.IsNullOrEmpty(ccpID.Value))
			{
				CCCustomerInformationManager cim = new CCCustomerInformationManager(currentCutomerPaymenMethod.CCProcessingCenterID, CCProcessingFeature.HostedForm,
					new ProcessingContext() { aCustomerID = currentCutomerPaymenMethod.BAccountID, aPMInstanceID = currentCutomerPaymenMethod.PMInstanceID, callerGraph = graph});
				cim.ManagePaymentMethodHostedForm();
			}
		}

		public static void SyncNewPMI<TPaymentMethodType, TDetialsType>(PXGraph graph, PXSelectBase<TPaymentMethodType> customerPaymentMethodView, PXSelectBase<TDetialsType> detailsView)
			where TPaymentMethodType : CustomerPaymentMethod, new()
			where TDetialsType : CustomerPaymentMethodDetail, new()
		{
			bool isHF = CCProcessingUtils.isHFPaymentMethod(graph, customerPaymentMethodView.Current.PMInstanceID);
			bool isConverting = customerPaymentMethodView.Current.Selected == true;
			isHF = isHF && !isConverting;
			TDetialsType CCPIDDet = null;
			bool isIDFilled = false;
			bool isOtherDetsFilled = false;
			foreach (PXResult<TDetialsType, PaymentMethodDetail> det in detailsView.Select())
			{
				TDetialsType cpmd = det;
				PaymentMethodDetail cmd = det;
				if (cmd.IsCCProcessingID == true)
				{
					isIDFilled = cpmd.Value != null;
					CCPIDDet = (TDetialsType)detailsView.Cache.CreateCopy(cpmd);
				}
				else
				{
					isOtherDetsFilled = cpmd.Value != null || isOtherDetsFilled;
				}
			}
			if (CCPIDDet == null)
			{
				//something's very wrong
				throw new PXException(Messages.NOCCPID, customerPaymentMethodView.Current.Descr);
			}
			if (isIDFilled && isOtherDetsFilled)
			{
				return;
			}

			if ((isIDFilled || isOtherDetsFilled) && !isHF || isIDFilled && !isOtherDetsFilled)
			{
				CCCustomerInformationManager cim = new CCCustomerInformationManager(customerPaymentMethodView.Current.CCProcessingCenterID, CCProcessingFeature.Tokenization,
					new ProcessingContext() {
						aCustomerID = customerPaymentMethodView.Current.BAccountID,
						aPMInstanceID = customerPaymentMethodView.Current.PMInstanceID,
						callerGraph = graph
					});
				if (customerPaymentMethodView.Current.CustomerCCPID == null)
				{
					string id = cim.CreateCustomer();
					TPaymentMethodType cpm = (TPaymentMethodType)customerPaymentMethodView.Cache.CreateCopy(customerPaymentMethodView.Current);
					cpm.CustomerCCPID = id;
					customerPaymentMethodView.Update(cpm);
				}
				if (isOtherDetsFilled)
				{
					string newPMId = cim.CreatePaymentMethod();
					CCPIDDet.Value = newPMId;
					CCPIDDet = detailsView.Update(CCPIDDet);
				}
				SyncPMResponse syncResponse = cim.GetPaymentMethod();
				if (syncResponse.PMList.ContainsKey(CCPIDDet.Value))
				{
					foreach (PXResult<TDetialsType, PaymentMethodDetail> det in detailsView.Select())
					{
						TDetialsType cpmd = det;
						if (cpmd.DetailID == CCPIDDet.DetailID)
							continue;
						string detailValue;
						if (!syncResponse.PMList[CCPIDDet.Value].TryGetValue(cpmd.DetailID, out detailValue))
						{
							detailValue = null;
						}
						TDetialsType newcpmd = (TDetialsType)detailsView.Cache.CreateCopy(cpmd);
						newcpmd.Value = detailValue;
						detailsView.Update(newcpmd);
					}
				}
				else
				{
					throw new PXException(Messages.CouldntGetPMIDetails, customerPaymentMethodView.Current.Descr);
				}
			}
		}

		public static void SyncExistingPMI(PXGraph graph, PXSelectBase<CustomerPaymentMethod> customerPaymentMethodView, PXSelectBase<CustomerPaymentMethodDetail> detailsView)
		{
			string CCPID = null;
			foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> det in detailsView.Select())
			{
				CustomerPaymentMethodDetail cpmd = (CustomerPaymentMethodDetail)det;
				PaymentMethodDetail pmd = (PaymentMethodDetail)det;
				if (pmd.IsCCProcessingID == true)
				{
					CCPID = cpmd.Value;
					break;
				}
			}
			if (String.IsNullOrEmpty(CCPID))
			{
				throw new PXException(Messages.CreditCardTokenIDNotFound);
			}
			CCCustomerInformationManager cim = new CCCustomerInformationManager(customerPaymentMethodView.Current.CCProcessingCenterID, CCProcessingFeature.Tokenization,
				new ProcessingContext() {
					aCustomerID = customerPaymentMethodView.Current.BAccountID,
					aPMInstanceID = customerPaymentMethodView.Current.PMInstanceID,
					callerGraph = graph});
			SyncPMResponse response = cim.GetPaymentMethod();
			if (response.PMList.Count == 0)
			{
				throw new PXException(Messages.CreditCardNotFoundInProcCenter, CCPID, customerPaymentMethodView.Current.CCProcessingCenterID);
			}
			foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> det in detailsView.Select())
			{
				CustomerPaymentMethodDetail cpmd = (CustomerPaymentMethodDetail)det;
				PaymentMethodDetail pmd = (PaymentMethodDetail)det;
				if (pmd.IsCCProcessingID != true && response.PMList.First().Value.ContainsKey(cpmd.DetailID))
				{
					cpmd.Value = response.PMList.First().Value[cpmd.DetailID];
					detailsView.Update(cpmd);
				}
			}
		}

		public static void SyncDeletePMI(PXGraph graph, PXSelectBase<CustomerPaymentMethod> customerPaymentMethodView, PXSelectBase<CustomerPaymentMethodDetail> detailsView)
		{
			IEnumerator cpmEnumerator = customerPaymentMethodView.Cache.Deleted.GetEnumerator();
			if (cpmEnumerator.MoveNext())
			{
				CustomerPaymentMethod current = (CustomerPaymentMethod)cpmEnumerator.Current;
				CCProcessingCenter processingCenter = CCProcessingUtils.getProcessingCenter(graph, current.CCProcessingCenterID);
				if (!string.IsNullOrEmpty(current.CCProcessingCenterID) && processingCenter.SyncronizeDeletion == true)
				{
					CCCustomerInformationManager cim = new CCCustomerInformationManager(current.CCProcessingCenterID, CCProcessingFeature.Tokenization,
						new ProcessingContext() { aCustomerID = current.BAccountID, aPMInstanceID = current.PMInstanceID });
					CustomerPaymentMethodDetail ccpidCPMDet = null;
					PaymentMethodDetail ccpidPMDet = PXSelect<PaymentMethodDetail,
						Where<PaymentMethodDetail.paymentMethodID, Equal<Optional<CustomerPaymentMethod.paymentMethodID>>,
							And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>, And<PaymentMethodDetail.isCCProcessingID, Equal<True>>>>>.Select(graph, current.PaymentMethodID);
					foreach (CustomerPaymentMethodDetail deletedDet in detailsView.Cache.Deleted)
					{
						if (deletedDet.DetailID == ccpidPMDet.DetailID)
						{
							ccpidCPMDet = deletedDet;
							break;
						}
					}
					if (ccpidCPMDet != null && !string.IsNullOrEmpty(ccpidCPMDet.Value))
					{
						cim.DeletePaymentMethod();
					}
				}
			}
		}
	}
}
