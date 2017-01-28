using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing;

namespace PX.Objects.CA
{
	public class PaymentMethodConverter : PXGraph<PaymentMethodConverter>
	{
		public PXFilter<Filter> filter;
		public PXFilteredProcessing<CustomerPaymentMethod, Filter, Where<CustomerPaymentMethod.converted, Equal<False>, And<CustomerPaymentMethod.paymentMethodID, Equal<Current<Filter.oldPaymentMethodID>>, 
			And<CustomerPaymentMethod.cCProcessingCenterID, Equal<Current<Filter.oldCCProcessingCenterID>>>>>> CustomerPaymentMethodList;
		public PXSelect<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Optional<Filter.newCCProcessingCenterID>>>> ProcessingCenters;
		public PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Optional<CustomerPaymentMethod.pMInstanceID>>>> NewCustomerPM;
		public PXSelectJoin<CustomerPaymentMethodDetail, LeftJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethodDetail.paymentMethodID>,
					And<PaymentMethodDetail.detailID, Equal<CustomerPaymentMethodDetail.detailID>,
					And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>, 
					Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Optional<CustomerPaymentMethod.pMInstanceID>>>> CustomerPMDetails;
		[Serializable]
		public partial class Filter : IBqlTable
		{
			#region OldPaymentMethodID
			public abstract class oldPaymentMethodID : PX.Data.IBqlField
			{
			}
			protected String _OldPaymentMethodID;
			[PXDBString(10, IsUnicode = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Old Payment Method ID", Visibility = PXUIVisibility.SelectorVisible)]
			[PXSelector(typeof (Search<PaymentMethod.paymentMethodID, Where<PaymentMethod.paymentType, Equal<PaymentMethodType.creditCard>>>))]
			public virtual String OldPaymentMethodID
			{
				get { return this._OldPaymentMethodID; }
				set { this._OldPaymentMethodID = value; }
			}
			#endregion
			#region OldCCProcessingCenterID
			public abstract class oldCCProcessingCenterID : PX.Data.IBqlField
			{
			}
			[PXDBString(10, IsUnicode = true)]
			[PXDefault(typeof(Search2<CCProcessingCenterPmntMethod.processingCenterID,
				InnerJoin<PaymentMethod, On<CCProcessingCenterPmntMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>,
				Where<PaymentMethod.paymentMethodID, Equal<Current<Filter.oldPaymentMethodID>>, And<CCProcessingCenterPmntMethod.isDefault, Equal<True>>>>))]
			[PXSelector(typeof(Search2<CCProcessingCenterPmntMethod.processingCenterID,
				InnerJoin<PaymentMethod, On<CCProcessingCenterPmntMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>,
				Where<PaymentMethod.paymentMethodID, Equal<Current<Filter.oldPaymentMethodID>>>>))]
			[PXUIField(DisplayName = "Old Proc. Center ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual string OldCCProcessingCenterID { get; set; }
			#endregion
			#region NewCCProcessingCenterID
			public abstract class newCCProcessingCenterID : PX.Data.IBqlField
			{
			}
			[PXDBString(10, IsUnicode = true)]
			[PXDefault()]
			[PXSelector(typeof(Search<CCProcessingCenter.processingCenterID>))]
			[PXUIField(DisplayName = "New Proc. Center ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual string NewCCProcessingCenterID { get; set; }
			#endregion
		}

		public PXCancel<Filter> Cancel;

		protected virtual void Filter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
			Filter row = (Filter)e.Row;
			PXUIFieldAttribute.SetEnabled<Filter.oldCCProcessingCenterID>(sender, e.Row, (!string.IsNullOrEmpty(row.OldPaymentMethodID)));
			PXUIFieldAttribute.SetEnabled<Filter.newCCProcessingCenterID>(sender, e.Row, (!string.IsNullOrEmpty(row.OldCCProcessingCenterID)));
			bool newDataIsFilled = !string.IsNullOrEmpty(row.NewCCProcessingCenterID);
			CustomerPaymentMethodList.SetProcessEnabled(newDataIsFilled);
			CustomerPaymentMethodList.SetProcessAllEnabled(newDataIsFilled);
			if (newDataIsFilled)
			{
//				PaymentMethod newPM = PaymentMethods.Select();
				CCProcessingCenter newCCPC = ProcessingCenters.Select();
				CustomerPaymentMethodList.SetProcessDelegate(cpm => ConvertCustomerPaymentMethod(cpm, newCCPC));
			}
		}

		private void ConvertCustomerPaymentMethod(CustomerPaymentMethod cpm, CCProcessingCenter newCCPC)
		{
			if (newCCPC == null)
			{
				throw new PXException(Messages.NotSetProcessingCenter);
			}
			if (!CCPaymentProcessing.IsFeatureSupported(newCCPC, CCProcessingFeature.Tokenization))
			{
				throw new PXException(Messages.ProcessingCenterShouldTokenization);
			}
			PaymentMethodUpdater updaterGreph = PXGraph.CreateInstance<PaymentMethodUpdater>();
			updaterGreph.ConvertCustomerPaymentMethod(cpm, newCCPC);
		}

		protected virtual void Filter_OldPaymentMethodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Filter filter = e.Row as Filter;
			if (filter == null) return;
			cache.SetDefaultExt<Filter.oldCCProcessingCenterID>(filter);
		}

		protected virtual void Filter_OldCCProcessingCenterID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Filter filter = e.Row as Filter;
			if (filter == null) return;
			if (!string.IsNullOrEmpty(filter.OldCCProcessingCenterID))
			{
				CCProcessingCenter procCenter = PXSelect<CCProcessingCenter,
					Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>.Select(this, filter.OldCCProcessingCenterID);
				if (CCPaymentProcessing.IsFeatureSupported(procCenter, CCProcessingFeature.Tokenization))
				{
					throw new PXSetPropertyException(Messages.ProcessingCenterShouldNotTokenization);
				}
			}
		}

		protected virtual void Filter_NewCCProcessingCenterID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Filter filter = e.Row as Filter;
			if (filter == null) return;
			if (!string.IsNullOrEmpty(filter.NewCCProcessingCenterID))
			{
				CCProcessingCenter procCenter = PXSelect<CCProcessingCenter,
					Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>.Select(this, filter.NewCCProcessingCenterID);
				if (!CCPaymentProcessing.IsFeatureSupported(procCenter, CCProcessingFeature.Tokenization))
				{
					throw new PXSetPropertyException(Messages.ProcessingCenterShouldTokenization);
				}
			}
		}

		protected virtual PaymentMethodDetail FindSameTypeTemplate(PaymentMethod targetPM, PaymentMethodDetail baseDetail)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
				And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,
				And<PaymentMethodDetail.isIdentifier, Equal<Required<PaymentMethodDetail.isIdentifier>>,
				And<PaymentMethodDetail.isExpirationDate, Equal<Required<PaymentMethodDetail.isExpirationDate>>,
				And<PaymentMethodDetail.isOwnerName, Equal<Required<PaymentMethodDetail.isOwnerName>>,
				And<PaymentMethodDetail.isCCProcessingID, Equal<Required<PaymentMethodDetail.isCCProcessingID>>>>>>>>>.
					Select(this, targetPM.PaymentMethodID, baseDetail.IsIdentifier, baseDetail.IsExpirationDate, baseDetail.IsOwnerName, baseDetail.IsCCProcessingID);
			return res;
		}

		protected virtual PaymentMethodDetail FindCCPID(PaymentMethod pm)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
				And<PaymentMethodDetail.isCCProcessingID, Equal<True>>>>.Select(this, pm.PaymentMethodID);
			return res;
		}
	}

	public class PaymentMethodUpdater : PXGraph<PaymentMethodUpdater, CustomerPaymentMethod>
	{
		public PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Optional<CustomerPaymentMethod.pMInstanceID>>>> CustomerPM;
		public PXSelectJoin<CustomerPaymentMethodDetail, InnerJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethodDetail.paymentMethodID>,
					And<PaymentMethodDetail.detailID, Equal<CustomerPaymentMethodDetail.detailID>,
					And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
					Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Optional<CustomerPaymentMethod.pMInstanceID>>>> CustomerPMDetails;

		public PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Optional<CustomerPaymentMethod.paymentMethodID>>>> PM;
		public PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Optional<CustomerPaymentMethod.paymentMethodID>>>> PMDetails;
		public PXSelect<CCProcessingCenterPmntMethod> ProcessingCenterPM;

		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search<CCProcessingCenterPmntMethod.processingCenterID, Where<CCProcessingCenterPmntMethod.paymentMethodID, Equal<Current<CustomerPaymentMethod.paymentMethodID>>>>), DirtyRead = true)]
		protected virtual void CustomerPaymentMethod_CCProcessingCenterID_CacheAttached(PXCache sender)
		{
		}

		public void ConvertCustomerPaymentMethod(CustomerPaymentMethod cpm, CCProcessingCenter newCCPC)
		{
			CCProcessingCenterPmntMethod newProcessingCenterPM = PXSelect<CCProcessingCenterPmntMethod, 
				Where<CCProcessingCenterPmntMethod.paymentMethodID, Equal<Required<CCProcessingCenterPmntMethod.paymentMethodID>>, 
				And<CCProcessingCenterPmntMethod.processingCenterID, Equal<Required<CCProcessingCenterPmntMethod.processingCenterID>>>>>.Select(this, cpm.PaymentMethodID, newCCPC.ProcessingCenterID);
			if (newProcessingCenterPM == null)
			{
				newProcessingCenterPM = (CCProcessingCenterPmntMethod)ProcessingCenterPM.Cache.CreateInstance();
				newProcessingCenterPM.PaymentMethodID = cpm.PaymentMethodID;
				newProcessingCenterPM.ProcessingCenterID = newCCPC.ProcessingCenterID;
				ProcessingCenterPM.Insert(newProcessingCenterPM);
			}

			CustomerPaymentMethod currCPM = (CustomerPaymentMethod)CustomerPM.Cache.CreateCopy(cpm);
			currCPM.CCProcessingCenterID = newCCPC.ProcessingCenterID;
			CustomerPM.Cache.SetDefaultExt<CustomerPaymentMethod.customerCCPID>(currCPM);
			
			currCPM.Selected = true;
			currCPM = CustomerPM.Update(currCPM);
			CustomerPM.Current = currCPM;
			
			PXResultset<PaymentMethodDetail> oldDetails = PMDetails.Select(currCPM.PaymentMethodID);
			foreach (PaymentMethodDetail oldDetail in oldDetails)
			{
				PaymentMethodDetail newDetail = (PaymentMethodDetail)PMDetails.Cache.CreateCopy(oldDetail);
				newDetail.ValidRegexp = null;
				PMDetails.Update(newDetail);
			}

			PaymentMethod CurrPM = PM.Select();
			PaymentMethodDetail CCPID = FindCCPID(CurrPM);

			if (CCPID == null)
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					PaymentMethodDetail res;
					CCPID = (PaymentMethodDetail) PMDetails.Cache.CreateInstance();
					CCPID.PaymentMethodID = currCPM.PaymentMethodID;
					CCPID.UseFor = PaymentMethodDetailUsage.UseForARCards;
					CCPID.DetailID = "CCPID";
					CCPID.Descr = "Payment Profile ID";
					CCPID.IsCCProcessingID = true;
					CCPID.IsRequired = true;
					res = PMDetails.Insert(CCPID);
					if (res == null)
					{
						throw new PXException(Messages.CouldNotInsertPMDetail);
					}
					else
					{
						PMDetails.Cache.Persist(PXDBOperation.Insert);
					}
					ts.Complete();
				}
			}

			CustomerPaymentMethodDetail newCCPIDPM = PXSelect<CustomerPaymentMethodDetail, Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Required<CustomerPaymentMethodDetail.pMInstanceID>>,
				And<CustomerPaymentMethodDetail.paymentMethodID, Equal<Required<CustomerPaymentMethodDetail.paymentMethodID>>,
					And<CustomerPaymentMethodDetail.detailID, Equal<Required<CustomerPaymentMethodDetail.detailID>>>>>>.Select(this, currCPM.PMInstanceID, currCPM.PaymentMethodID, CCPID.DetailID);
			if (newCCPIDPM != null)
			{
				newCCPIDPM.Value = null;
				CustomerPMDetails.Update(newCCPIDPM);
			}
			else
			{
				newCCPIDPM = new CustomerPaymentMethodDetail();
				newCCPIDPM.PMInstanceID = currCPM.PMInstanceID;
				newCCPIDPM.PaymentMethodID = currCPM.PaymentMethodID;
				newCCPIDPM.DetailID = CCPID.DetailID;
				CustomerPMDetails.Insert(newCCPIDPM);
			}

			CCCustomerInformationManager.SyncNewPMI(this, CustomerPM, CustomerPMDetails);
			currCPM.Converted = true;
			currCPM = CustomerPM.Update(currCPM);
			this.Save.Press();
		}

		protected virtual PaymentMethodDetail FindSameTypeTemplate(PaymentMethod targetPM, PaymentMethodDetail baseDetail)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
				And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,
				And<PaymentMethodDetail.isIdentifier, Equal<Required<PaymentMethodDetail.isIdentifier>>,
				And<PaymentMethodDetail.isExpirationDate, Equal<Required<PaymentMethodDetail.isExpirationDate>>,
				And<PaymentMethodDetail.isOwnerName, Equal<Required<PaymentMethodDetail.isOwnerName>>,
				And<PaymentMethodDetail.isCCProcessingID, Equal<Required<PaymentMethodDetail.isCCProcessingID>>>>>>>>>.
					Select(this, targetPM.PaymentMethodID, baseDetail.IsIdentifier, baseDetail.IsExpirationDate, baseDetail.IsOwnerName, baseDetail.IsCCProcessingID);
			return res;
		}

		protected virtual PaymentMethodDetail FindCCPID(PaymentMethod pm)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
				And<PaymentMethodDetail.isCCProcessingID, Equal<True>>>>.Select(this, pm.PaymentMethodID);
			return res;
		}
	}
}
