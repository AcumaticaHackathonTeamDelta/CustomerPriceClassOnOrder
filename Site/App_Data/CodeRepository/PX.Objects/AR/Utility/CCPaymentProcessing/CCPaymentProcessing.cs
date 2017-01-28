using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Compilation;
using PX.CCProcessingBase;
using PX.Common;
using PX.Data;
using PX.Objects.CA;

namespace PX.Objects.AR.CCPaymentProcessing
{
	public class ProcessingContext
	{
		public CCProcessingCenter processingCenter = null;
		public int? aPMInstanceID = null;
		public int? aCustomerID = 0;
		public string aCustomerCD = null;
		public string aDocType = null;
		public string aRefNbr = null;
		public PXGraph callerGraph = null;
	}

	public class CCPaymentProcessing : PXGraph<CCPaymentProcessing>, 
										IProcessingCenterSettingsStorage, 
										ICustomerDataReader, ICreditCardDataReader, IDocDetailsDataReader

	{
		

		#region Public Members
		public PXSelect<CCProcTran, Where<CCProcTran.tranNbr, Equal<Required<CCProcTran.tranNbr>>>> procTran;
		public PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>> pmInstance;
		public PXSelect<CustomerPaymentMethodDetail,
						Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Required<CustomerPaymentMethodDetail.pMInstanceID>>,
						And<CustomerPaymentMethodDetail.detailID, Equal<Required<CustomerPaymentMethodDetail.detailID>>>>> pmICVVDetail;

		public virtual bool TestCredentials(PXGraph callerGraph, string processingCenterID)
		{
			ICCPaymentProcessing processor = initializeProcessing(processingCenterID, CCProcessingFeature.Base, 
				new ProcessingContext() { callerGraph = callerGraph }, true);
			APIResponse apiResponse = new APIResponse();
			processor.TestCredentials(apiResponse);
			ProcessAPIResponse(apiResponse);
			return apiResponse.isSucess;
		}

		public virtual void ValidateSettings(PXGraph callerGraph, string processingCenterID, ISettingsDetail settingDetail)
		{
			ICCPaymentProcessing processor = initializeProcessing(processingCenterID, CCProcessingFeature.Base, 
				new ProcessingContext() { callerGraph = callerGraph }, true);
			CCErrors result = processor.ValidateSettings(settingDetail);
			if (result.source != CCErrors.CCErrorSource.None)
			{
				throw new PXSetPropertyException(result.ErrorMessage);
			}
		}

		#endregion

		#region Private Members		

		private ProcessingContext _context;

		#endregion

		#region Internal Members

		internal ProcessingContext Context
		{
			get
			{
				return _context;
			}
			set
			{
				_context = new ProcessingContext()
				{
					processingCenter = value.processingCenter,
					callerGraph = value.callerGraph,
					aCustomerCD = value.aCustomerCD,
					aCustomerID = value.aCustomerID,
					aDocType = value.aDocType,
					aPMInstanceID = value.aPMInstanceID,
					aRefNbr = value.aRefNbr
				};
			}
		}

		#endregion

		#region Public Processing Functions
		public virtual bool Authorize(int aPMInstanceID, bool aCapture, string aCuryID, decimal aAmount, string aDocType, string aRefNbr, ref int aTranNbr)
		{
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPMInstanceID, out customer);
			if (pmInstance == null)
			{
				throw new PXException(Messages.CreditCardWithID_0_IsNotDefined, aPMInstanceID);
			}
			if (!IsValidForProcessing(pmInstance, customer)) return false;
			
			CCTranType tranType = aCapture ? CCTranType.AuthorizeAndCapture : CCTranType.AuthorizeOnly;
			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPMInstanceID;
			tran.DocType = aDocType;
			tran.RefNbr = aRefNbr;
			tran.CuryID = aCuryID;
			tran.Amount = aAmount;
			return this.DoTransaction(ref aTranNbr, tranType, tran, null, customer.AcctCD, null);
		}

		public virtual bool Authorize(ICCPayment aPmtInfo, bool aCapture, ref int aTranNbr)
		{			
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPmtInfo.PMInstanceID, out customer);
			if (pmInstance == null)
			{
				throw new PXException(Messages.CreditCardWithID_0_IsNotDefined, aPmtInfo.PMInstanceID);
			}
			CCProcessingCenter procCenter = this.findProcessingCenter(pmInstance.PMInstanceID, null);
			if (procCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterIsNotSpecified, pmInstance.Descr);
			}
			if (!(procCenter.IsActive ?? false))
			{
				throw new PXException(Messages.ERR_CCProcessingIsInactive, procCenter.ProcessingCenterID);
			}
			if (!IsValidForProcessing(pmInstance, customer)) return false;

			CCTranType tranType = aCapture ? CCTranType.AuthorizeAndCapture : CCTranType.AuthorizeOnly;
			CCProcTran tran = new CCProcTran();
			tran.Copy(aPmtInfo);
			return this.DoTransaction(ref aTranNbr, tranType, tran, null, customer.AcctCD, procCenter);
		}

		public virtual bool Capture(int? aPMInstanceID, int? aAuthTranNbr, string aCuryID, decimal? aAmount, ref int aTranNbr)
		{
			CCProcTran authTran = this.findTransaction(aAuthTranNbr);
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPMInstanceID, out customer);
			
			if (authTran == null)
			{
				throw new PXException(Messages.ERR_CCAuthorizationTransactionIsNotFound, aAuthTranNbr);
			}
			if (!IsAuthorized(authTran))
			{
				throw new PXException(Messages.ERR_CCProcessingReferensedTransactionNotAuthorized, aAuthTranNbr);
			}

			if (authTran.ExpirationDate.HasValue && authTran.ExpirationDate.Value < PXTimeZoneInfo.Now) 
			{
				throw new PXException(Messages.ERR_CCAuthorizationTransactionHasExpired,aAuthTranNbr);
			}
			if (!IsCCValidForProcessing(aPMInstanceID)) return false;

			CCProcessingCenter procCenter = this.findProcessingCenter(authTran.ProcessingCenterID);
			if (procCenter == null) 
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedForAuthIsNotValid, authTran.ProcessingCenterID, aAuthTranNbr);
			}
			if (!(procCenter.IsActive ?? false)) 
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedForAuthIsNotActive, procCenter, aAuthTranNbr);
			}
			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPMInstanceID;
			tran.RefTranNbr = aAuthTranNbr;
			tran.DocType = authTran.DocType;
			tran.RefNbr = authTran.RefNbr;
			tran.CuryID = aCuryID;
			tran.Amount = aAmount;
			tran.OrigDocType = authTran.OrigDocType;
			tran.OrigRefNbr = authTran.OrigRefNbr;
			
			return this.DoTransaction(ref aTranNbr, CCTranType.PriorAuthorizedCapture, tran, authTran, customer.AcctCD, procCenter);
		}

		public virtual bool CaptureOnly(ICCPayment aPmtInfo, string aAuthorizationNbr,  ref int aTranNbr)
		{
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPmtInfo.PMInstanceID, out customer);
			if (string.IsNullOrEmpty(aAuthorizationNbr))
			{
				throw new PXException(Messages.ERR_CCExternalAuthorizationNumberIsRequiredForCaptureOnlyTrans, aAuthorizationNbr);
			}
			CCProcessingCenter procCenter = this.findProcessingCenter(pmInstance.CCProcessingCenterID);
			if (procCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedForAuthIsNotValid, pmInstance.CCProcessingCenterID, aTranNbr);
			}
			if (!(procCenter.IsActive ?? false))
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedForAuthIsNotActive, procCenter, aTranNbr);
			}
			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;
			CCProcTran tran = new CCProcTran();
			tran.Copy(aPmtInfo);
			CCProcTran refTran = new CCProcTran();
			refTran.AuthNumber = aAuthorizationNbr;
			return this.DoTransaction(ref aTranNbr, CCTranType.CaptureOnly, tran, refTran, customer.AcctCD, procCenter);			
		}

		public virtual bool Void(int? aPMInstanceID, int? aRefTranNbr, ref int aTranNbr)
		{
			CCProcTran refTran = this.findTransaction(aRefTranNbr);
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPMInstanceID, out customer);
			if (refTran == null)
			{
				throw new PXException(Messages.ERR_CCTransactionToVoidIsNotFound, aRefTranNbr);
			}
			if (!MayBeVoided(refTran))
			{
				throw new PXException(Messages.ERR_CCProcessingTransactionMayNotBeVoided, refTran.TranType);
			}
			if (!IsAuthorized(refTran))
			{
				throw new PXException(Messages.ERR_CCProcessingReferensedTransactionNotAuthorized, aRefTranNbr);
			}
			if (!IsCCValidForProcessing(aPMInstanceID)) return false;
			CCProcessingCenter procCenter = this.findProcessingCenter(refTran.ProcessingCenterID);
			if (procCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotFound, refTran.ProcessingCenterID, aRefTranNbr);
			}
			if (!(procCenter.IsActive ?? false))
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotFound, procCenter, aRefTranNbr);
			}
			
			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPMInstanceID;
			tran.RefTranNbr = aRefTranNbr;
			tran.DocType = refTran.DocType;
			tran.RefNbr = refTran.RefNbr;
			tran.CuryID = refTran.CuryID;
			tran.Amount = refTran.Amount;
			tran.OrigDocType = refTran.OrigDocType;
			tran.OrigRefNbr = refTran.OrigRefNbr;

			return this.DoTransaction(ref aTranNbr, CCTranType.Void, tran, refTran, customer.AcctCD, procCenter);
		}
		
		public virtual bool VoidOrCredit(int? aPMInstanceID, int? aRefTranNbr, ref int aTranNbr) 
		{
			if(!this.Void(aPMInstanceID, aRefTranNbr, ref aTranNbr))
			{
				CCProcTran refTRan = this.findTransaction(aRefTranNbr);
				if(MayBeCredited(refTRan))
					return this.Credit(aPMInstanceID, aRefTranNbr,  null, null, ref aTranNbr);
			}
			return true;
		}

		public virtual bool Credit(ICCPayment aPmtInfo, string aExtRefTranNbr, ref int aTranNbr)
		{
			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;

			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPmtInfo.PMInstanceID, out customer);
			CCProcTran refTran = PXSelect<CCProcTran, Where<CCProcTran.pMInstanceID, Equal<Required<CCProcTran.pMInstanceID>>,
												And<CCProcTran.pCTranNumber, Equal<Required<CCProcTran.pCTranNumber>>,
													And<Where<CCProcTran.tranType,Equal<CCTranTypeCode.authorizeAndCapture>,
														Or<CCProcTran.tranType,Equal<CCTranTypeCode.priorAuthorizedCapture>>>>>>, 
														OrderBy<Desc<CCProcTran.tranNbr>>>.Select(this, aPmtInfo.PMInstanceID, aExtRefTranNbr);
			if (refTran != null)
			{
				return Credit(aPmtInfo, refTran.TranNbr.Value, ref aTranNbr);
			}
			else 
			{
				CCProcTran tran = new CCProcTran();
				tran.Copy(aPmtInfo);
				tran.RefTranNbr = null;
				tran.RefPCTranNumber = aExtRefTranNbr;
				refTran = new CCProcTran();
				refTran.PCTranNumber = aExtRefTranNbr;
				return this.DoTransaction(ref aTranNbr, CCTranType.Credit, tran, refTran, customer.AcctCD, null);
			}			
		}

		public virtual bool Credit(ICCPayment aPmtInfo, int aRefTranNbr, ref int aTranNbr) 
		{
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPmtInfo.PMInstanceID, out customer);
			CCProcTran authTran = this.findTransaction(aRefTranNbr);
			if (authTran == null)
			{
				throw new PXException(Messages.ERR_CCTransactionToCreditIsNotFound, aRefTranNbr);
			}
			if (!MayBeCredited(authTran))
			{
				throw new PXException(Messages.ERR_CCProcessingTransactionMayNotBeCredited, authTran.TranType);
			}

			if (!IsAuthorized(authTran))
			{
				throw new PXException(Messages.ERR_CCProcessingReferensedTransactionNotAuthorized, aRefTranNbr);
			}

			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;

			CCProcessingCenter procCenter = this.findProcessingCenter(authTran.ProcessingCenterID);
			if (procCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotFound, authTran.ProcessingCenterID, aRefTranNbr);
			}

			if (!(procCenter.IsActive ?? false))
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotActive, authTran.ProcessingCenterID, aRefTranNbr);
			}
			CCProcTran tran = new CCProcTran();
			tran.Copy(aPmtInfo);
			tran.RefTranNbr = authTran.TranNbr;

			if (!aPmtInfo.CuryDocBal.HasValue)
			{			
				tran.CuryID = authTran.CuryID;
				tran.Amount = authTran.Amount;
			}
			return this.DoTransaction(ref aTranNbr, CCTranType.Credit, tran, authTran, customer.AcctCD, procCenter);
			
		}

		protected virtual bool Credit(int? aPMInstanceID, int? aRefTranNbr, string aCuryID, decimal? aAmount, ref int aTranNbr)
		{
			CCProcTran authTran = this.findTransaction(aRefTranNbr);
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPMInstanceID, out customer);
			if (authTran == null)
			{
				throw new PXException(Messages.ERR_CCTransactionToCreditIsNotFound, aRefTranNbr);
			}
			if (!MayBeCredited(authTran))
			{
				throw new PXException(Messages.ERR_CCProcessingTransactionMayNotBeCredited, authTran.TranType);
			}

			if (!IsAuthorized(authTran))
			{
				throw new PXException(Messages.ERR_CCProcessingReferensedTransactionNotAuthorized, aRefTranNbr);
			}
			if (!IsCCValidForProcessing(aPMInstanceID)) return false;

			CCProcessingCenter procCenter = this.findProcessingCenter(authTran.ProcessingCenterID);
			if(procCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotFound, authTran.ProcessingCenterID,aRefTranNbr);
			}

			if (!(procCenter.IsActive ?? false))
			{
				throw new PXException(Messages.ERR_CCProcessingCenterUsedInReferencedTranNotActive, authTran.ProcessingCenterID, aRefTranNbr);
			}
			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPMInstanceID;
			tran.DocType = authTran.DocType;
			tran.RefNbr = authTran.RefNbr;
			tran.OrigDocType = authTran.OrigDocType;
			tran.OrigRefNbr = authTran.OrigRefNbr;
			tran.RefTranNbr = authTran.TranNbr;
			if (aAmount.HasValue)
			{
				tran.CuryID = aCuryID;
				tran.Amount = aAmount;
			}
			else 
			{
				tran.CuryID = authTran.CuryID;
				tran.Amount = authTran.Amount;
			}
			return this.DoTransaction(ref aTranNbr, CCTranType.Credit, tran, authTran, customer.AcctCD, procCenter);
		}

		public virtual bool RecordAuthorization(ICCPayment aPmtInfo,  string aExtTranRef, Decimal? aAuthAmount, DateTime? aExpirationDate, ref int? aTranNbr)
		{
			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;		

			CCProcessingCenter procCenter = this.findProcessingCenter(aPmtInfo.PMInstanceID, aPmtInfo.CuryID);
			
			if (procCenter == null || string.IsNullOrEmpty(procCenter.ProcessingTypeName))
			{
				throw new PXException(Messages.ERR_ProcessingCenterForCardNotConfigured);
			}

			CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, procCenter.CashAccountID);
			if (cashAccount.CuryID != aPmtInfo.CuryID)
			{
				throw new PXException(Messages.ProcessingCenterCurrencyDoesNotMatch, aPmtInfo.CuryID, cashAccount.CuryID);
			}

			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPmtInfo.PMInstanceID;
			tran.OrigDocType = aPmtInfo.OrigDocType;
			tran.OrigRefNbr = aPmtInfo.OrigRefNbr;
			tran.DocType = aPmtInfo.DocType;
			tran.RefNbr = aPmtInfo.RefNbr;
			tran.CuryID = aPmtInfo.CuryID;
			tran.Amount = aAuthAmount.HasValue? aAuthAmount: aPmtInfo.CuryDocBal;
			tran.PCResponseReasonText = Messages.ImportedExternalCCTransaction;
			tran.ExpirationDate = aExpirationDate;
			tran.PCTranNumber = aExtTranRef;
			tran.ProcessingCenterID = procCenter.ProcessingCenterID;
			this.RecordTransaction(ref aTranNbr, CCTranType.AuthorizeOnly, tran, procCenter);
			return true;
		}

		public virtual bool RecordCapture(ICCPayment aPmtInfo, string aExtTranRef, string aExtAuthNbr, Decimal? aAmount, ref int? aTranNbr)
		{
			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;

			CCProcessingCenter procCenter = this.findProcessingCenter(aPmtInfo.PMInstanceID, aPmtInfo.CuryID);

			if (procCenter == null || string.IsNullOrEmpty(procCenter.ProcessingTypeName))
			{
				throw new PXException(Messages.ERR_ProcessingCenterForCardNotConfigured);
			}

			CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, procCenter.CashAccountID);
			if (cashAccount.CuryID != aPmtInfo.CuryID)
			{
				throw new PXException(Messages.ProcessingCenterCurrencyDoesNotMatch, aPmtInfo.CuryID, cashAccount.CuryID);
			}

			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aPmtInfo.PMInstanceID;
			tran.OrigDocType = aPmtInfo.OrigDocType;
			tran.OrigRefNbr = aPmtInfo.OrigRefNbr;
			tran.DocType = aPmtInfo.DocType;
			tran.RefNbr = aPmtInfo.RefNbr;
			tran.CuryID = aPmtInfo.CuryID;
			tran.Amount = aAmount.HasValue ? aAmount : aPmtInfo.CuryDocBal;		
			tran.PCTranNumber = aExtTranRef;
			tran.AuthNumber = aExtAuthNbr;
			tran.PCResponseReasonText = Messages.ImportedExternalCCTransaction;
			tran.ProcessingCenterID = procCenter.ProcessingCenterID;
			this.RecordTransaction(ref aTranNbr, CCTranType.AuthorizeAndCapture, tran, procCenter);
			return true;
		}

		public virtual void RecordVoidingTran(CCProcTran aOrigTran, string aExtTranRef, out int aTranNbr)
		{			
			CCProcTran tran = new CCProcTran();
			tran.PMInstanceID = aOrigTran.PMInstanceID;
			tran.ProcessingCenterID = aOrigTran.ProcessingCenterID;
			tran.OrigDocType = aOrigTran.OrigDocType;
			tran.OrigRefNbr = aOrigTran.OrigRefNbr;
			tran.DocType = aOrigTran.DocType;
			tran.RefNbr = aOrigTran.RefNbr;
			tran.CuryID = aOrigTran.CuryID;
			tran.Amount = aOrigTran.Amount;
			tran.RefTranNbr = aOrigTran.TranNbr;
			tran.PCTranNumber = aExtTranRef;
			tran.TranType = CCTranTypeCode.GetTypeCode(CCTranType.Void);
			FillRecordedTran(tran, Messages.LostExpiredTranVoided);
			tran = this.procTran.Insert(tran);
			this.Actions.PressSave();
			aTranNbr = tran.TranNbr.Value;						
		}

		public virtual bool RecordCredit(ICCPayment aPmtInfo, string aRefPCTranNbr, string aExtTranRef, string aExtAuthNbr, out int? aTranNbr)
		{
			aTranNbr = null;
			if (!IsCCValidForProcessing(aPmtInfo.PMInstanceID)) return false;
			CCProcTran tran = new CCProcTran();
			tran.Copy(aPmtInfo);
			CCProcTran origCCTran = null;
			if (!String.IsNullOrEmpty(aRefPCTranNbr))
			{
			   origCCTran = PXSelect<CCProcTran, Where<CCProcTran.pMInstanceID, Equal<Required<CCProcTran.pMInstanceID>>,
												And<CCProcTran.pCTranNumber, Equal<Required<CCProcTran.pCTranNumber>>,
													And<Where<CCProcTran.tranType, Equal<CCTranTypeCode.authorizeAndCapture>,
														Or<CCProcTran.tranType, Equal<CCTranTypeCode.priorAuthorizedCapture>>>>>>,
														OrderBy<Desc<CCProcTran.tranNbr>>>.Select(this, aPmtInfo.PMInstanceID, aRefPCTranNbr);
			}

			if (origCCTran != null && (aPmtInfo.PMInstanceID == origCCTran.PMInstanceID))
			{
				//Override Orig Doc Ref by those from orig CC Tran if any
				tran.RefTranNbr = origCCTran.TranNbr;
				tran.ProcessingCenterID = origCCTran.ProcessingCenterID;
			}
			else
			{
				CCProcessingCenter procCenter = this.findProcessingCenter(aPmtInfo.PMInstanceID, aPmtInfo.CuryID);
				if (procCenter == null || string.IsNullOrEmpty(procCenter.ProcessingTypeName))
				{
					throw new PXException(Messages.ERR_ProcessingCenterForCardNotConfigured);
				}
				CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, procCenter.CashAccountID);
				if (cashAccount.CuryID != aPmtInfo.CuryID)
				{
					throw new PXException(Messages.ProcessingCenterCurrencyDoesNotMatch, aPmtInfo.CuryID, cashAccount.CuryID);
				}
				tran.ProcessingCenterID = procCenter.ProcessingCenterID;
				tran.RefPCTranNumber = aRefPCTranNbr;
			}
			tran.PCTranNumber = aExtTranRef;
			tran.AuthNumber = aExtAuthNbr;
			tran.TranType = CCTranTypeCode.GetTypeCode(CCTranType.Credit);
			FillRecordedTran(tran);
			tran = this.procTran.Insert(tran);
			this.Actions.PressSave();
			aTranNbr = tran.TranNbr;
			return true;
		}
		#endregion

		#region Public Static Functions
		public static bool IsFeatureSupported(Type pluginType, CCProcessingFeature feature)
		{
			bool result;
			switch (feature)
			{
				case CCProcessingFeature.Tokenization:
					result = typeof(ICCTokenizedPaymnetProcessing).IsAssignableFrom(pluginType);
					break;
				case CCProcessingFeature.HostedForm:
					result = typeof(ICCProcessingHostedForm).IsAssignableFrom(pluginType);
					break;
				case CCProcessingFeature.UnmaskedExpirationDate:
					result = typeof(ICCProcessingExpDateFormatting).IsAssignableFrom(pluginType);
					break;
				default:
					result = false;
					break;
			}
			return result;
		}

		public static bool IsFeatureSupported(CCProcessingCenter ccProcessingCenter, CCProcessingFeature feature)
		{
			return ccProcessingCenter != null && !string.IsNullOrEmpty(ccProcessingCenter.ProcessingTypeName) && IsFeatureSupported(PXBuildManager.GetType(ccProcessingCenter.ProcessingTypeName, true), feature);
		}

		public static bool IsAuthorized(CCProcTran aTran)
		{
			return (aTran.TranStatus == CCTranStatusCode.Approved) && (aTran.ProcStatus == CCProcStatus.Finalized);
		}

		public static bool MayBeVoided(CCProcTran aOrigTran)
		{
			switch (aOrigTran.TranType)
			{
				case CCTranTypeCode.Authorize:
				case CCTranTypeCode.AuthorizeAndCapture:
				case CCTranTypeCode.PriorAuthorizedCapture:
				case CCTranTypeCode.CaptureOnly:
					return true;
			}
			return false;
		}

		public static bool MayBeCredited(CCProcTran aOrigTran)
		{
			switch (aOrigTran.TranType)
			{
				case CCTranTypeCode.AuthorizeAndCapture:
				case CCTranTypeCode.PriorAuthorizedCapture:
				case CCTranTypeCode.CaptureOnly:
					return true;
			}
			return false;
		}

		public static bool IsExpired(CustomerPaymentMethod aPMInstance) 
		{
			return (aPMInstance.ExpirationDate.HasValue && aPMInstance.ExpirationDate.Value < DateTime.Now);
		}

		protected static void FillRecordedTran(CCProcTran aTran, string aReasonText = Messages.ImportedExternalCCTransaction)
		{
			aTran.PCResponseReasonText = aReasonText;
			aTran.TranNbr = null;
			aTran.StartTime = aTran.EndTime = PXTimeZoneInfo.Now;
			aTran.ExpirationDate = null;
			aTran.TranStatus = CCTranStatusCode.Approved;
			aTran.ProcStatus = CCProcStatus.Finalized;
		}

		#endregion

		#region Internal Processing Functions

		internal void checkProcessing(string processingCenterID, CCProcessingFeature feature, ProcessingContext newContext)
		{
			_context = newContext;
			CCProcessingCenter procCenter = findProcessingCenter(processingCenterID);
			checkProcessingCenter(procCenter);
			_context.processingCenter = procCenter;
			if (feature != CCProcessingFeature.Base && !IsFeatureSupported(procCenter, feature))
			{
				throw new PXException(Messages.FeatureNotSupportedByProcessing, feature.ToString());
			}
		}

		internal ICCPaymentProcessing initializeProcessing(string processingCenterID, CCProcessingFeature feature, ProcessingContext newContext, 
			bool doCheck = false)
		{
			if (doCheck)
			{
				checkProcessing(processingCenterID, feature, newContext);
			}
			else
			{
				_context = newContext;
			}
			if (_context.processingCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterNotFound);
			}
			ICCPaymentProcessing processor = CreateCCPaymentProcessorInstance(_context.processingCenter);
			processor.Initialize(this, this, this);
			return processor;
		}

		internal virtual void ProcessAPIResponse(APIResponse apiResponse)
		{
			if (!apiResponse.isSucess && apiResponse.ErrorSource != CCErrors.CCErrorSource.None)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, string> kvp in apiResponse.Messages)
				{
					stringBuilder.Append(kvp.Key);
					stringBuilder.Append(": ");
					stringBuilder.Append(kvp.Value);
					stringBuilder.Append(". ");
				}
				throw new PXException(Messages.CardProcessingError, CCErrors.GetDescription(apiResponse.ErrorSource), stringBuilder.ToString());
			}
		}

		protected virtual bool DoTransaction(ref int aTranNbr, CCTranType aTranType, CCProcTran aTran, CCProcTran aRefTran, string aCustomerCD, CCProcessingCenter aProcCenter)
		{
			
			if (aProcCenter == null)
			{
				aProcCenter = this.findProcessingCenter(aTran.PMInstanceID, aTran.CuryID);
			}

			if (aProcCenter == null || string.IsNullOrEmpty(aProcCenter.ProcessingTypeName))
			{
				throw new PXException(Messages.ERR_ProcessingCenterForCardNotConfigured);
			}
			
			CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, aProcCenter.CashAccountID);
			if (cashAccount.CuryID != aTran.CuryID)
			{
				throw new PXException(Messages.ProcessingCenterCurrencyDoesNotMatch, aTran.CuryID, cashAccount.CuryID);
			}
			
			aTran.ProcessingCenterID = aProcCenter.ProcessingCenterID;
			aTran.TranType = CCTranTypeCode.GetTypeCode(aTranType);

			aTran.CVVVerificationStatus = CVVVerificationStatusCode.RelyOnPriorVerification;
			bool cvvVerified = false;
			bool needCvvVerification = isCvvVerificationRequired(aTranType);
			if (needCvvVerification) 
			{
				bool isStored;
				CCProcTran verifyTran = this.findCVVVerifyingTran(aTran.PMInstanceID, out isStored);
				if (verifyTran != null) 
				{
					cvvVerified = true;
					if(!isStored)
						this.UpdateCvvVerificationStatus(verifyTran);
				}
				if(!cvvVerified)
					aTran.CVVVerificationStatus = CVVVerificationStatusCode.NotVerified;
			}
			aTran = this.StartTransaction(aTran, aProcCenter.OpenTranTimeout);
			aTranNbr = aTran.TranNbr.Value;

			ProcessingInput inputData = new ProcessingInput();
			Copy(inputData, aTran);			
			if (!string.IsNullOrEmpty(aCustomerCD))
			{
				inputData.CustomerCD = aCustomerCD;
			}
			if (aRefTran != null)
			{
				inputData.OrigRefNbr = (aTranType == CCTranType.CaptureOnly)? aRefTran.AuthNumber: aRefTran.PCTranNumber;
			}

			if (needCvvVerification) 
			{
				inputData.VerifyCVV = !cvvVerified;
			}

			if (_context == null)
			{
				_context = new ProcessingContext();
			}
			_context.processingCenter = aProcCenter;
			_context.aCustomerCD = aCustomerCD;
			_context.aPMInstanceID = inputData.PMInstanceID;
			_context.aDocType = inputData.DocType;
			_context.aRefNbr = inputData.DocRefNbr;
			//context.aRefNbr = inputData.
			ICCPaymentProcessing processor = CreateCCPaymentProcessorInstance(aProcCenter);
			processor.Initialize(this, this, this, this);
			ProcessingResult result = new ProcessingResult();
			bool hasError = true;
			try
			{
				hasError = !processor.DoTransaction(aTranType, inputData, result);				
			}
			catch (WebException webExn)
			{
				hasError = true;
				result.ErrorSource = CCErrors.CCErrorSource.Network;
				result.ErrorText = webExn.Message;
			}
			catch (Exception exn)
			{
				hasError = true;
				result.ErrorSource = CCErrors.CCErrorSource.Internal;
				result.ErrorText = exn.Message;
				throw new PXException(Messages.ERR_CCPaymentProcessingInternalError,aTranNbr, exn.Message);
			}
			finally
			{
				CCProcTran tran =  this.EndTransaction(aTranNbr, result, (hasError ? CCProcStatus.Error : CCProcStatus.Finalized));
				if (!hasError)
				{
					this.ProcessTranResult(tran, aRefTran, result);
				}
			}
			return result.isAuthorized;
		}

		protected virtual CCProcTran StartTransaction(CCProcTran aTran, int? aAutoExpTimeout)
		{
			aTran.TranNbr = null;
			aTran.StartTime = PXTimeZoneInfo.Now;
			if (aAutoExpTimeout.HasValue) 
			{
				aTran.ExpirationDate = aTran.StartTime.Value.AddSeconds(aAutoExpTimeout.Value);
			}
			aTran = this.procTran.Insert(aTran);
			this.Actions.PressSave();			
			return this.procTran.Current;
		}

		protected virtual CCProcTran EndTransaction(int aTranID, ProcessingResult aRes, string aProcStatus)
		{
			CCProcTran tran = this.procTran.Select(aTranID);
			this.procTran.Cache.CreateCopy(tran);
			Copy(tran, aRes);
			tran.ProcStatus = aProcStatus;
			tran.EndTime = PXTimeZoneInfo.Now;

			if(aRes.ExpireAfterDays.HasValue)
				tran.ExpirationDate = tran.EndTime.Value.AddDays(aRes.ExpireAfterDays.Value);
			else
				tran.ExpirationDate = null;
			tran = this.procTran.Update(tran);
			this.Actions.PressSave();
			this.UpdateCvvVerificationStatus(tran);
			return tran;			
		}

		protected virtual void ProcessTranResult(CCProcTran aTran, CCProcTran aRefTran, ProcessingResult aResult) 
		{
			if (aTran.TranType == CCTranTypeCode.GetTypeCode(CCTranType.Void) && 
				aRefTran.IsManuallyEntered()) 
			{
				if (aResult.TranStatus != CCTranStatus.Approved
					&& ((aResult.ResultFlag & (CCResultFlag.OrigTransactionNotFound|CCResultFlag.OrigTransactionExpired)) != CCResultFlag.None)) 
				{
					//Force Void over Missed of Expired Authorization transaction;
					int voidingTranNbr;
					RecordVoidingTran(aRefTran, null, out voidingTranNbr);
					aResult.isAuthorized = true; 
				}
			}
		}
		
		protected virtual void RecordTransaction(ref int? aTranNbr, CCTranType aTranType, CCProcTran aTran, CCProcessingCenter aProcCenter)
		{
			//Add later - use ProcessCenter to fill ExpirationDate
			aTran.ProcessingCenterID = aProcCenter.ProcessingCenterID;
			aTran.TranType = CCTranTypeCode.GetTypeCode(aTranType);
			aTran.CVVVerificationStatus = CVVVerificationStatusCode.RelyOnPriorVerification;
			bool cvvVerified = false;
			bool needCvvVerification = isCvvVerificationRequired(aTranType);
			if (needCvvVerification)
			{
				bool isStored;
				CCProcTran verifyTran = this.findCVVVerifyingTran(aTran.PMInstanceID, out isStored);
				if (verifyTran != null)
				{
					cvvVerified = true;					
				}
				if (!cvvVerified)
					aTran.CVVVerificationStatus = CVVVerificationStatusCode.NotVerified;
			}
			aTran.TranNbr = null;
			aTran.StartTime = aTran.EndTime = PXTimeZoneInfo.Now;
			if (aTranType != CCTranType.AuthorizeOnly)
			{
				aTran.ExpirationDate = null;
			}
			aTran.TranStatus = CCTranStatusCode.Approved;
			aTran.ProcStatus = CCProcStatus.Finalized;
			aTran = this.procTran.Insert(aTran);
			this.Actions.PressSave();
			aTranNbr = aTran.TranNbr;
		}

		#endregion

		#region Internal Reading Functions

		protected static bool isCvvVerificationRequired(CCTranType aType)
		{
			return (aType == CCTranType.AuthorizeOnly || aType == CCTranType.AuthorizeAndCapture);
		}

		protected virtual CCProcessingCenter ReadProcessingCenterDefinition(string aProcessingCenterID)
		{
			PXSelectBase<CCProcessingCenter> definition = new PXSelect<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>(this);
			CCProcessingCenter result = (CCProcessingCenter)definition.Select(aProcessingCenterID);
			return result;
		}

		protected virtual CCProcessingCenter findProcessingCenter(int? aPMInstanceID, string aCuryId)
		{
			CCProcessingCenter result;
			PXGraph contextGraph = _context != null && _context.callerGraph != null ? _context.callerGraph : this;
			result = PXSelectJoin<CCProcessingCenter, InnerJoin<CustomerPaymentMethod,
				On<CustomerPaymentMethod.cCProcessingCenterID, Equal<CCProcessingCenter.processingCenterID>>>,
				Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(contextGraph, aPMInstanceID);
			if (result != null)
			{
				return result;
			}
			if (aCuryId != null)
			{
				result = PXSelectJoin<CCProcessingCenter,
					InnerJoin<CCProcessingCenterPmntMethod,
						On<CCProcessingCenter.processingCenterID, Equal<CCProcessingCenterPmntMethod.processingCenterID>,
							And<CCProcessingCenter.isActive, Equal<BQLConstants.BitOn>>>,
						InnerJoin<CustomerPaymentMethod,
							On<CustomerPaymentMethod.paymentMethodID,
								Equal<CCProcessingCenterPmntMethod.paymentMethodID>,
								And<CCProcessingCenterPmntMethod.isActive, Equal<BQLConstants.BitOn>>>,
							InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CCProcessingCenter.cashAccountID>>>>>,
					Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>,
						And<CashAccount.curyID, Equal<Required<CashAccount.curyID>>>>,
					OrderBy<Desc<CCProcessingCenterPmntMethod.isDefault>>>.Select(contextGraph, aPMInstanceID, aCuryId);
			}
			else
			{
				result = PXSelectJoin<CCProcessingCenter,
					InnerJoin<CCProcessingCenterPmntMethod,
						On<CCProcessingCenter.processingCenterID, Equal<CCProcessingCenterPmntMethod.processingCenterID>,
							And<CCProcessingCenter.isActive, Equal<BQLConstants.BitOn>>>,
						InnerJoin<CustomerPaymentMethod,
							On<CustomerPaymentMethod.paymentMethodID,
								Equal<CCProcessingCenterPmntMethod.paymentMethodID>,
								And<CCProcessingCenterPmntMethod.isActive, Equal<BQLConstants.BitOn>>>,
							InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CCProcessingCenter.cashAccountID>>>>>,
					Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>,
					OrderBy<Desc<CCProcessingCenterPmntMethod.isDefault>>>.Select(contextGraph, aPMInstanceID);
			}
			return result;
		}

		protected virtual CCProcessingCenter findProcessingCenter(string aProcCenterID)
		{
			PXGraph contextGraph = _context != null && _context.callerGraph != null ? _context.callerGraph : this;
			CCProcessingCenter result = PXSelect<CCProcessingCenter,								
										Where<CCProcessingCenter.processingCenterID,
										Equal<Required<CCProcessingCenter.processingCenterID>>>>.Select(contextGraph, aProcCenterID);
			return result;
		}

		protected virtual CCProcessingCenter findProcessindCenterFromPM(string PaymentMethodID)
		{
			PXGraph contextGraph = _context != null && _context.callerGraph != null ? _context.callerGraph : this;
			CCProcessingCenter result = PXSelectJoin<CCProcessingCenter, InnerJoin<CCProcessingCenterPmntMethod,
								On<CCProcessingCenter.processingCenterID, Equal<CCProcessingCenterPmntMethod.processingCenterID>,
									And<CCProcessingCenter.isActive, Equal<BQLConstants.BitOn>>>, 
								InnerJoin<PaymentMethod, On<CCProcessingCenterPmntMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>>,
								Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.SelectWindowed(contextGraph, 0, 1, PaymentMethodID);
			return result;
		}

		protected virtual void checkProcessingCenter(CCProcessingCenter processingCenter)
		{
			if (processingCenter == null)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterNotFound);
			}
			if (processingCenter.IsActive != true)
			{
				throw new PXException(Messages.ERR_CCProcessingCenterIsNotActive);
			}
			if (string.IsNullOrEmpty(processingCenter.ProcessingTypeName))
			{
				throw new PXException(Messages.ERR_ProcessingCenterForCardNotConfigured);
			}
		}

		protected virtual CCProcTran findTransaction(int? aTranNbr)
		{
			return (CCProcTran)this.procTran.Select(aTranNbr);
		}

		protected virtual CCProcTran findCVVVerifyingTran(int? aPMInstanceID, out bool aIsStored) 
		{
			CustomerPaymentMethod pmInstance = this.pmInstance.Select(aPMInstanceID);
			if (pmInstance.CVVVerifyTran.HasValue)
			{
				aIsStored = true;
				return this.findTransaction(pmInstance.CVVVerifyTran);
			}
			else 
			{
				aIsStored = false;
				CCProcTran verifyingTran = PXSelect<CCProcTran, Where<CCProcTran.pMInstanceID, Equal<Required<CCProcTran.pMInstanceID>>,
									And<CCProcTran.procStatus, Equal<CCProcStatus.finalized>,
									And<CCProcTran.tranStatus, Equal<CCTranStatusCode.approved>,
									And<CCProcTran.cVVVerificationStatus, Equal<CVVVerificationStatusCode.match>
									>>>>, OrderBy<Desc<CCProcTran.tranNbr>>>.Select(this, aPMInstanceID);
				return verifyingTran;
			}			
		}

		protected virtual CustomerPaymentMethod findPMInstance(int? aPMInstanceID, out Customer aCustomer) 
		{
			PXResult<CustomerPaymentMethod, Customer> res = (PXResult<CustomerPaymentMethod, Customer>)PXSelectJoin<CustomerPaymentMethod,
													InnerJoin<Customer, On<Customer.bAccountID, Equal<CustomerPaymentMethod.bAccountID>>>,
												   Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(this, aPMInstanceID);
			if (res != null) 
			{
				aCustomer = (Customer)res;
				return (CustomerPaymentMethod)res;
			}
			aCustomer = null;
			return null;
		}

		protected virtual void UpdateCvvVerificationStatus(CCProcTran aTran) 
		{
			if (aTran.TranStatus ==  CCTranStatusCode.Approved &&
				aTran.CVVVerificationStatus == CVVVerificationStatusCode.Match && 
				(aTran.TranType == CCTranTypeCode.AuthorizeAndCapture || aTran.TranType == CCTranTypeCode.Authorize))
			{
				CustomerPaymentMethod pmInstance = this.pmInstance.Select(aTran.PMInstanceID);
				if (!pmInstance.CVVVerifyTran.HasValue)
				{
					pmInstance.CVVVerifyTran = aTran.TranNbr;
					CustomerPaymentMethodDetail cvvDetail = this.pmICVVDetail.Select(aTran.PMInstanceID, CreditCardAttributes.CVV);
					if (cvvDetail != null)
						this.pmICVVDetail.Delete(cvvDetail);
					this.pmInstance.Update(pmInstance);
					this.Actions.PressSave();
				}
			}
		}

		public static ICCPaymentProcessing CreateCCPaymentProcessorInstance(CCProcessingCenter aProcCenter) 
		{
			ICCPaymentProcessing processor = null;
			try
			{
				Type processorType = PXBuildManager.GetType(aProcCenter.ProcessingTypeName, true);
				processor = (ICCPaymentProcessing)Activator.CreateInstance(processorType);
			}
			catch (HttpException)
			{
				throw new PXException(Messages.ERR_ProcessingCenterTypeIsInvalid, aProcCenter.ProcessingTypeName, aProcCenter.ProcessingCenterID);
			}
			catch (Exception)
			{
				throw new PXException(Messages.ERR_ProcessingCenterTypeInstanceCreationFailed, aProcCenter.ProcessingTypeName, aProcCenter.ProcessingCenterID);
			}
			return processor;
		}

		protected virtual bool IsCCValidForProcessing(int? aPMInstanceID)
		{
			Customer customer;
			CustomerPaymentMethod pmInstance = this.findPMInstance(aPMInstanceID, out customer);
			if (pmInstance == null)
			{
				throw new PXException(Messages.CreditCardWithID_0_IsNotDefined, aPMInstanceID);
			}
			return IsValidForProcessing(pmInstance, customer);
		}

		protected virtual bool IsValidForProcessing(CustomerPaymentMethod pmInstance, Customer customer)
		{
			if (pmInstance == null)
			{
				throw new PXException(Messages.CreditCardWithID_0_IsNotDefined);
			}
			if (pmInstance != null && pmInstance.IsActive == false)
			{
				throw new PXException(Messages.InactiveCreditCardMayNotBeProcessed, pmInstance.Descr);
			}
			if (IsExpired(pmInstance))
			{
				throw new PXException(Messages.ERR_CCCreditCardHasExpired, pmInstance.ExpirationDate.Value, customer.AcctCD);
			}
			return true;
		} 

		#endregion	

		#region  Utility Funcions

		public static void Copy(ProcessingInput aDst, CCProcTran aSrc)
		{
			aDst.TranID = aSrc.TranNbr.Value;
			aDst.PMInstanceID = aSrc.PMInstanceID.Value;
			bool useOrigDoc = String.IsNullOrEmpty(aSrc.DocType);
			aDst.DocType = useOrigDoc ? aSrc.OrigDocType : aSrc.DocType;
			aDst.DocRefNbr = useOrigDoc ? aSrc.OrigRefNbr : aSrc.RefNbr;
			aDst.Amount = aSrc.Amount.Value;
		}

		public static void Copy(CCProcTran aDst, ProcessingResult aSrc)
		{
			aDst.PCTranNumber = aSrc.PCTranNumber;
			aDst.PCResponseCode = aSrc.PCResponseCode;
			aDst.PCResponseReasonCode = aSrc.PCResponseReasonCode;
			aDst.AuthNumber = aSrc.AuthorizationNbr;
			aDst.PCResponse = aSrc.PCResponse;
			aDst.PCResponseReasonText = aSrc.PCResponseReasonText;
			aDst.CVVVerificationStatus = CVVVerificationStatusCode.GetCCVCode(aSrc.CcvVerificatonStatus);
			aDst.TranStatus= CCTranStatusCode.GetCode(aSrc.TranStatus);
			if (aSrc.ErrorSource != CCErrors.CCErrorSource.None)
			{
				aDst.ErrorSource = CCErrors.GetCode(aSrc.ErrorSource);
				aDst.ErrorText = aSrc.ErrorText;
				if (aSrc.ErrorSource != CCErrors.CCErrorSource.ProcessingCenter)
				{
					aDst.PCResponseReasonText = aSrc.ErrorText;
				}
			}
		}
		
		#endregion 
		
		#region IProcessingCenterSettingsStorage Members
		void IProcessingCenterSettingsStorage.ReadSettings(Dictionary<string, string> aSettings)
		{
			if (_context == null)
			{
				throw new PXException(CA.Messages.ProcessingObjectTypeIsNotSpecified);
			}
			PXGraph contextGraph = _context.callerGraph ?? this;
			PXSelectBase<CCProcessingCenterDetail> settings = new PXSelect<CCProcessingCenterDetail, Where<CCProcessingCenterDetail.processingCenterID,
						Equal<Required<CCProcessingCenterDetail.processingCenterID>>>>(contextGraph);
			foreach (CCProcessingCenterDetail it in settings.Select(_context.processingCenter.ProcessingCenterID))
			{
				aSettings[it.DetailID] = it.Value;
			}
		}
		#endregion

		#region ICustomerDataReader Members

		void ICustomerDataReader.ReadData(Dictionary<string, string> aData)
		{
			if (_context == null)
			{
				throw new PXException(CA.Messages.ProcessingObjectTypeIsNotSpecified);
			}
			PXGraph contextGraph = _context.callerGraph ?? this;
			PXResult<Customer, CR.Address, CR.Contact> result = null;
			if (_context.aCustomerID != 0)
			{
				result = (PXResult<Customer, CR.Address, CR.Contact>) PXSelectJoin<Customer,LeftJoin<CR.Address, On<CR.Address.addressID, Equal<Customer.defBillAddressID>>,
																							LeftJoin<CR.Contact, On<CR.Contact.contactID, Equal<Customer.defBillContactID>>>>,
																							Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(contextGraph, _context.aCustomerID);
			}
			else if (!string.IsNullOrEmpty(_context.aCustomerCD))
			{
				result = (PXResult<Customer, CR.Address, CR.Contact>)PXSelectJoin<Customer, LeftJoin<CR.Address, On<CR.Address.addressID, Equal<Customer.defBillAddressID>>,
																							LeftJoin<CR.Contact, On<CR.Contact.contactID, Equal<Customer.defBillContactID>>>>,
																							Where<Customer.acctCD, Equal<Required<Customer.acctCD>>>>.Select(contextGraph, _context.aCustomerCD);
			}
			PXResult<CustomerPaymentMethod, CR.Address, CR.Contact> pmResult = (PXResult<CustomerPaymentMethod, CR.Address, CR.Contact>)PXSelectJoin<CustomerPaymentMethod,
																	LeftJoin<CR.Address, On<CR.Address.addressID, Equal<CustomerPaymentMethod.billAddressID>>,
																	LeftJoin<CR.Contact, On<CR.Contact.contactID, Equal<CustomerPaymentMethod.billContactID>>>>,
																	Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(contextGraph, _context.aPMInstanceID);
			
			if (result != null) 
			{
				Customer customer = (Customer)result;
				CR.Address billAddress= null;
				CR.Contact billContact = null;
				if(pmResult != null && ((CR.Address)pmResult).AddressID.HasValue)
					billAddress = (CR.Address)pmResult;
				else
					billAddress = (CR.Address)result;
				if(pmResult != null && ((CR.Contact)pmResult).ContactID.HasValue)
					billContact = (CR.Contact)pmResult;
				else
					billContact = (CR.Contact)result;
				
				aData[key_CustomerCD] = customer.AcctCD;
				aData[key_CustomerName] = customer.AcctName;
				if (pmResult != null && !string.IsNullOrEmpty(((CustomerPaymentMethod) pmResult).CustomerCCPID))
				{
					aData[Key_Customer_CCProcessingID] = ((CustomerPaymentMethod) pmResult).CustomerCCPID;
				}
				if (billAddress.AddressID.HasValue) 
				{
					aData[key_Country] = billAddress.CountryID;
					aData[key_State] = billAddress.State;
					aData[key_City] = billAddress.City;
					aData[key_Address] = ExtractStreetAddress(billAddress);
					aData[key_PostalCode] = billAddress.PostalCode;
				}
				if (billContact.ContactID.HasValue)
				{                    
					aData[key_Customer_FirstName] = billContact.FirstName;
					aData[key_Customer_LastName] = billContact.LastName;
					aData[key_Phone] = billContact.Phone1;
					aData[key_Fax] = billContact.Fax;
					aData[key_Email] = billContact.EMail;					
				}
			}

			
		}

		string ICustomerDataReader.Key_CustomerName
		{
			get { return key_CustomerName; }
		}

		string ICustomerDataReader.Key_CustomerCD
		{
			get { return key_CustomerCD; }
		}

		string ICustomerDataReader.Key_Customer_FirstName
		{
			get { return key_Customer_FirstName; }
		}

		string ICustomerDataReader.Key_Customer_LastName
		{
			get { return key_Customer_LastName; }
		}

		string ICustomerDataReader.Key_BillAddr_Country
		{
			get { return key_Country; }
		}

		string ICustomerDataReader.Key_BillAddr_State
		{
			get { return key_State; }
		}

		string ICustomerDataReader.Key_BillAddr_City
		{
			get { return key_City; }
		}

		string ICustomerDataReader.Key_BillAddr_Address
		{
			get { return key_Address; }
		}

		string ICustomerDataReader.Key_BillAddr_PostalCode
		{
			get { return key_PostalCode; }
		}
		string ICustomerDataReader.Key_BillContact_Phone
		{
			get { return key_Phone; }
		}
		string ICustomerDataReader.Key_BillContact_Fax
		{
			get { return key_Fax; }
		}

		string ICustomerDataReader.Key_BillContact_Email
		{
			get { return key_Email; }
		}

		string ICustomerDataReader.Key_Customer_CCProcessingID
		{
			get { return Key_Customer_CCProcessingID; }
		}


		#region Private Constants
		private const string key_CustomerCD = "CustomerCD";
		private const string key_CustomerName = "CustomerName";
		private const string key_Customer_FirstName = "CustomerFirstName";
		private const string key_Customer_LastName = "CustomerLastName";
		private const string key_Country = "CountryID";
		private const string key_State = "State";
		private const string key_City = "City";
		private const string key_Address = "Address";
		private const string key_PostalCode = "PostalCode";
		private const string key_Phone = "Phone";
		private const string key_Fax = "Fax";
		private const string key_Email = "Email";
		private const string key_Customer_ID = "CustomerIP";
		private const string Key_Customer_CCProcessingID = "CCProcessingID";
		#endregion

		public static string ExtractStreetAddress(CR.Address aAddress) 
		{
			string result = aAddress.AddressLine1;
			if (string.IsNullOrEmpty(aAddress.AddressLine1))
			{
				if(!string.IsNullOrEmpty(result))
					result += " ";
				result += aAddress.AddressLine1;
			}
			if (string.IsNullOrEmpty(aAddress.AddressLine2))
			{
				if (!string.IsNullOrEmpty(result))
					result += " ";
				result += aAddress.AddressLine2;
			}
			return result;
		}
		#endregion

		#region ICreditCardDataReader Members

		private string Key_PMCCProcessingID;

		void ICreditCardDataReader.ReadData(Dictionary<string, string> aData)
		{
			if (_context == null)
			{
				throw new PXException(CA.Messages.ProcessingObjectTypeIsNotSpecified);
			}
			PXGraph contextGraph = _context.callerGraph ?? this;
			PXResultset<CustomerPaymentMethodDetail> PMDselect = null;
			if (_context.aPMInstanceID != null)
			{
				PMDselect = PXSelectJoin<CustomerPaymentMethodDetail,
						InnerJoin<PaymentMethodDetail,
							On<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>,
								And<CustomerPaymentMethodDetail.paymentMethodID, Equal<PaymentMethodDetail.paymentMethodID>>>>,
					Where<CustomerPaymentMethodDetail.pMInstanceID,
						Equal<Required<CustomerPaymentMethodDetail.pMInstanceID>>>>.Select(contextGraph, _context.aPMInstanceID);	
			}
			if (PMDselect != null)
			{
				foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> dets in PMDselect)
				{
					PaymentMethodDetail pmd = dets;
					CustomerPaymentMethodDetail cpmd = dets;
					if (pmd.IsCCProcessingID == true)
					{
						Key_PMCCProcessingID = pmd.DetailID;
					}
					aData[pmd.DetailID] = cpmd.Value;
				}
			}
		}

		string ICreditCardDataReader.Key_CardNumber
		{
			get { return CreditCardAttributes.CardNumber; }
		}

		string ICreditCardDataReader.Key_CardExpiryDate
		{
			get { return CreditCardAttributes.ExpirationDate; }
		}

		string ICreditCardDataReader.Key_CardCVV
		{
			get { return CreditCardAttributes.CVV; }
		}

		string ICreditCardDataReader.Key_PMCCProcessingID
		{
			get { return Key_PMCCProcessingID; }
		}

		#endregion

		#region IDocDetailsDataReader Members

		void IDocDetailsDataReader.ReadData(List<DocDetailInfo> aData)
		{
			if (_context == null)
			{
				throw new PXException(CA.Messages.ProcessingObjectTypeIsNotSpecified);
			}
			PXGraph contextGraph = _context.callerGraph ?? this;
			PXSelectBase<ARAdjust> sel = new PXSelect<ARAdjust, Where<ARAdjust.adjgDocType,
														Equal<Required<ARAdjust.adjgDocType>>,
														And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>,
														And<ARAdjust.voided, Equal<False>>>>>(contextGraph);
			foreach (ARAdjust it in sel.Select(_context.aDocType, _context.aRefNbr))
			{
				DocDetailInfo item = new DocDetailInfo();
				item.ItemID = String.Format("{0}{1}", it.AdjdDocType, it.AdjdRefNbr);
				item.ItemName = String.Format("{0} {1}", it.AdjdDocType, it.AdjdRefNbr);
				item.ItemDescription = String.Format("Payment of invoice {0}{1} - {2}", it.AdjdDocType, it.AdjdRefNbr, it.CuryAdjgAmt);
				item.Quantity = 1;
				item.IsTaxable = null;
				item.Price = it.CuryAdjgAmt.Value;
				aData.Add(item);
			}
		}

		#endregion
		
	}

	public enum CCProcessingFeature
	{
		Base,
		Tokenization,
		HostedForm,
		UnmaskedExpirationDate
	}
}
