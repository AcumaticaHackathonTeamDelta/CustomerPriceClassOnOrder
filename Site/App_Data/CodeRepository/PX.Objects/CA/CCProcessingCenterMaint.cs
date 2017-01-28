using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.CCProcessingBase;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.AR.CCPaymentProcessing;
using System.Web.Compilation;

namespace PX.Objects.CA
{
	public class CCProcessingCenterMaint : PXGraph<CCProcessingCenterMaint, CCProcessingCenter>, IProcessingCenterSettingsStorage
	{
		public PXSelect<CCProcessingCenter> ProcessingCenter;
		public PXSelect<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, 
			Equal<Current<CCProcessingCenter.processingCenterID>>>> CurrentProcessingCenter;
		public PXSelect<CCProcessingCenterDetail, Where<CCProcessingCenterDetail.processingCenterID, Equal<Current<CCProcessingCenter.processingCenterID>>>> Details;
		public PXSelect<CCProcessingCenterPmntMethod, Where<CCProcessingCenterPmntMethod.processingCenterID, Equal<Current<CCProcessingCenter.processingCenterID>>>> PaymentMethods;
		public PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<CCProcessingCenter.cashAccountID>>>> CashAccount;

		public PXAction<CCProcessingCenter> testCredentials;
		[PXUIField(DisplayName = "Test Credentials", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable TestCredentials(PXAdapter adapter)
		{
			CCProcessingCenter row = this.ProcessingCenter.Current;
			if (string.IsNullOrEmpty(row.ProcessingCenterID))
			{
				throw new PXException(Messages.ProcessingCenterNotSelected);
			}
			if(string.IsNullOrEmpty(row.ProcessingTypeName))
			{
				throw new PXException(Messages.ProcessingPluginNotSelected);
			}
			CCPaymentProcessing paymentProcessing = PXGraph.CreateInstance<CCPaymentProcessing>();
			if (paymentProcessing.TestCredentials(this, row.ProcessingCenterID))
			{
				ProcessingCenter.Ask(Messages.Result,Messages.CredentialsAccepted, MessageButtons.OK);
			}
			return adapter.Get();
		}

		public PXAction<CCProcessingCenter> updateExpirationDate;
		[PXUIField(DisplayName = "Update Expiration Dates", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable UpdateExpirationDate(PXAdapter adapter)
		{
			if (ProcessingCenter.Current != null)
			{
				CCProcessingCenter row = ProcessingCenter.Current;
				if (string.IsNullOrEmpty(CCProcessingUtils.GetExpirationDateFormat(this, row.ProcessingCenterID)))
				{
					ICCPaymentProcessing processor = this.CreateAndInitProcessor(row);
					this.ImportExpDateFromPC(processor);
					this.Save.Press();
				}				
				CCUpdateExpirationDatesProcess updateGraph = PXGraph.CreateInstance<CCUpdateExpirationDatesProcess>();
				updateGraph.Filter.Current = new CCUpdateExpirationDatesProcess.CPMFilter() { ProcessingCenterID = row.ProcessingCenterID };
				throw new PXRedirectRequiredException(updateGraph, true, "UpdateExpirationDate") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public CCProcessingCenterMaint()
		{
			GL.GLSetup setup = GLSetup.Current;
			this.ProcessingCenter.Cache.AllowDelete = false;
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterPmntMethod.isDefault>(this.PaymentMethods.Cache, null, false);
		}

		public PXSetup<GL.GLSetup> GLSetup;
		[PXRemoveBaseAttribute(typeof(PXSelectorAttribute))]
		protected virtual void CCProcessingCenterPmntMethod_ProcessingCenterID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void CCProcessingCenter_ProcessingTypeName_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CCProcessingCenter row = e.Row as CCProcessingCenter;
			if (row == null) return;
			if (row.ProcessingTypeName != null)
			{
				if (ProcessingCenter.Ask(Messages.ResetDetailsToDefault, MessageButtons.OKCancel) == WebDialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
		}

		protected virtual void CCProcessingCenter_ProcessingTypeName_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CCProcessingCenter row = e.Row as CCProcessingCenter;
			if (row == null) return;
			bool isTokenized = CCPaymentProcessing.IsFeatureSupported(row, CCProcessingFeature.Tokenization);
			bool isHF = CCPaymentProcessing.IsFeatureSupported(row, CCProcessingFeature.HostedForm);
			sender.SetValue<CCProcessingCenter.allowDirectInput>(row, isTokenized && !isHF);
			foreach (CCProcessingCenterDetail detail in Details.Select())
			{
				Details.Delete(detail);
			}
			this.ImportSettings();
		}

		protected virtual void CCProcessingCenter_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			CCProcessingCenter row = e.Row as CCProcessingCenter;
			if (row == null) return;
			row.NeedsExpDateUpdate = CCProcessingUtils.CCProcessingCenterNeedsExpDateUpdate(this, row);
		}
		

		protected virtual void CCProcessingCenter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled<CCProcessingCenter.processingTypeName>(sender, e.Row, (bool)((CCProcessingCenter)e.Row).IsActive);
			PXUIFieldAttribute.SetRequired<CCProcessingCenter.processingTypeName>(sender, (bool)((CCProcessingCenter)e.Row).IsActive);
			CCProcessingCenter row = (CCProcessingCenter)e.Row;
			bool isTokenized = CCPaymentProcessing.IsFeatureSupported(row, CCProcessingFeature.HostedForm);
			PXUIFieldAttribute.SetVisible<CCProcessingCenter.allowDirectInput>(sender, row, isTokenized);
			PXUIFieldAttribute.SetVisible<CCProcessingCenter.syncronizeDeletion>(sender, row, isTokenized);
			PXUIFieldAttribute.SetVisible<CCProcessingCenter.syncRetryAttemptsNo>(sender, row, isTokenized);
			PXUIFieldAttribute.SetVisible<CCProcessingCenter.syncRetryDelayMs>(sender, row, isTokenized);
			this.testCredentials.SetEnabled((row != null) && !string.IsNullOrEmpty(row.ProcessingCenterID) && !string.IsNullOrEmpty(row.ProcessingTypeName));
			this.updateExpirationDate.SetVisible(row != null && row.NeedsExpDateUpdate == true);
			this.ProcessingCenter.Cache.AllowDelete = !HasTransactions((CCProcessingCenter)e.Row);
			PaymentMethods.AllowInsert = false;
			PaymentMethods.AllowDelete = false;
		}

		protected virtual void CCProcessingCenter_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete && e.Row != null && (bool)((CCProcessingCenter)e.Row).IsActive && string.IsNullOrEmpty(((CCProcessingCenter)e.Row).ProcessingTypeName))
			{
				if (sender.RaiseExceptionHandling<CCProcessingCenter.processingTypeName>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CCProcessingCenter.processingTypeName).Name)))
				{
					throw new PXRowPersistingException(typeof(CCProcessingCenter.processingTypeName).Name, null, ErrorMessages.FieldIsEmpty, typeof(CCProcessingCenter.processingTypeName).Name);
				}
			}
		}
		protected virtual void CCProcessingCenterDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterDetail.detailID>(cache, e.Row, false);
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterDetail.descr>(cache, e.Row, false);
			CCProcessingCenterDetail row = e.Row as CCProcessingCenterDetail;
			if (row != null)
			{
				if (row.IsEncryptionRequired == true && row.IsEncrypted != true && Details.Cache.GetStatus(row) == PXEntryStatus.Notchanged)
				{
					Details.Cache.SetStatus(row, PXEntryStatus.Updated);
					Details.Cache.IsDirty = true;
					PXUIFieldAttribute.SetWarning<CCProcessingCenterDetail.value>(Details.Cache, row, Messages.CryptoSettingsChanged);
				}
			}
		}
		
		private bool errorKey;
		protected virtual void CCProcessingCenterDetail_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			if (errorKey)
			{
				errorKey = false;
				e.Cancel = true;
			}
			else
			{
				string detID = ((CCProcessingCenterDetail)e.Row).DetailID;				
				bool isExist = false;
				foreach (CCProcessingCenterDetail it in this.Details.Select())
				{
					if (it.DetailID == detID)
					{
						isExist = true;
					}
				}

				if (isExist)
				{
					cache.RaiseExceptionHandling<CCProcessingCenterDetail.detailID>(e.Row, detID, new PXException(Messages.RowIsDuplicated));  //Messages.DuplicatedCCProcessingCenterDetail
					e.Cancel = true;
				}
			}
		}
		protected virtual void CCProcessingCenterDetail_DetailID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			CCProcessingCenterDetail a = e.Row as CCProcessingCenterDetail;
			if (a.DetailID != null)
			{
				errorKey = true;
			}
		}

		protected virtual void CCProcessingCenterPmntMethod_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CCProcessingCenterPmntMethod row = e.Row as CCProcessingCenterPmntMethod;
			if (row == null) return;
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterPmntMethod.paymentMethodID>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterPmntMethod.isActive>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterPmntMethod.isDefault>(sender, null, false);
		}

		protected virtual void CCProcessingCenterPmntMethod_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			if (errorKey)
			{
				errorKey = false;
				e.Cancel = true;
			}
			else
			{
				CCProcessingCenterPmntMethod row = e.Row as CCProcessingCenterPmntMethod;
				string detID = row.ProcessingCenterID;
				bool isExist = false;
				foreach (CCProcessingCenterPmntMethod it in this.PaymentMethods.Select())
				{
					if (!Object.ReferenceEquals(it, row) && (it.PaymentMethodID == row.PaymentMethodID) && (it.ProcessingCenterID == it.ProcessingCenterID) )
					{
						isExist = true;
					}
				}
				if (isExist)
				{
					cache.RaiseExceptionHandling<CCProcessingCenterPmntMethod.paymentMethodID>(e.Row, detID, new PXException(Messages.PaymentMethodIsAlreadyAssignedToTheProcessingCenter));
					e.Cancel = true;
				}
			}
		}
		protected virtual void CCProcessingCenterPmntMethod_ProcessingCenterID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			CCProcessingCenterPmntMethod row = e.Row as CCProcessingCenterPmntMethod;
			if (row.PaymentMethodID!= null)
			{
				errorKey = true;
			}
		}
		
		protected virtual bool HasTransactions(CCProcessingCenter aRow) 
		{
			CCProcTran tran = PXSelect<CCProcTran, Where<CCProcTran.processingCenterID, Equal<Required<CCProcTran.processingCenterID>>>,OrderBy<Desc<CCProcTran.tranNbr>>>.Select(this,aRow.ProcessingCenterID);
			return (tran != null);
		}

		#region IProcessingCenterSettingsStorage Members

		public virtual void ReadSettings(Dictionary<string, CCProcessingCenterDetail> aSettings)
		{
			CCProcessingCenter row = this.ProcessingCenter.Current;
			foreach (CCProcessingCenterDetail iDet in this.Details.Select())
			{
				aSettings[iDet.DetailID] = iDet;
			}
		}

		public virtual void ReadSettings(Dictionary<string, string> aSettings)
		{
			CCProcessingCenter row = this.ProcessingCenter.Current;
			foreach (CCProcessingCenterDetail iDet in this.Details.Select())
			{
				aSettings[iDet.DetailID] = iDet.Value;
			}
		}

		protected virtual void ImportSettings()
		{
			CCProcessingCenter row = this.ProcessingCenter.Current;

			if (string.IsNullOrEmpty(row.ProcessingCenterID))
			{
				throw new PXException(Messages.ProcessingCenterIDIsRequiredForImport);
			}
			if (string.IsNullOrEmpty(row.ProcessingTypeName))
			{
				throw new PXException(Messages.ProcessingObjectTypeIsNotSpecified);
			}

			ICCPaymentProcessing processor = this.CreateAndInitProcessor(row);
			this.ImportSettingsFromPC(processor);
			this.ImportExpDateFromPC(processor);
		}

		private void ImportSettingsFromPC(ICCPaymentProcessing aProcessor)
		{
			CCProcessingCenter row = this.ProcessingCenter.Current;
			Dictionary<string, CCProcessingCenterDetail> currentSettings = new Dictionary<string,CCProcessingCenterDetail>();
			ReadSettings(currentSettings);
			List<ISettingsDetail> processorSettings = new List<ISettingsDetail>();
			aProcessor.ExportSettings(processorSettings);
			foreach (ISettingsDetail it in processorSettings)
			{
				if (!currentSettings.ContainsKey(it.DetailID))
				{
					CCProcessingCenterDetail detail = new CCProcessingCenterDetail();
					detail.Copy(it);
					detail = this.Details.Insert(detail);
				}
				else 
				{
					CCProcessingCenterDetail detail = currentSettings[it.DetailID];
					detail.Copy(it);
					detail = this.Details.Update(detail);
				}
			}
		}

		private void ImportExpDateFromPC(ICCPaymentProcessing aProcessor)
		{
			if (aProcessor is ICCProcessingExpDateFormatting)
			{
				CCProcessingCenterDetail detail = GetExpFormatDetail((ICCProcessingExpDateFormatting)aProcessor);
				detail = this.Details.Insert(detail);
			}
		}

		private ICCPaymentProcessing CreateAndInitProcessor(CCProcessingCenter row)
		{
			ICCPaymentProcessing processor = CCPaymentProcessing.CreateCCPaymentProcessorInstance(row);
			if (processor == null)
			{
				throw new PXException(Messages.InstanceOfTheProcessingTypeCanNotBeCreated, row.ProcessingTypeName);
			}
			processor.Initialize(this, null, null);
			return processor;
		}

		public CCProcessingCenterDetail GetExpFormatDetail(ICCProcessingExpDateFormatting provider)
		{
			CCProcessingCenterDetail detail = new CCProcessingCenterDetail();
			detail.DetailID = InterfaceConstants.ExpDateFormatDetailID;
			detail.Descr = InterfaceConstants.ExpDateFormatDesc;
			detail.Value = provider.GetExpirationDateFormat();
			return detail;
		}

		#endregion

		protected virtual void CCProcessingCenterDetail_Value_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CCProcessingCenterDetail row = e.Row as CCProcessingCenterDetail;
			if (row != null)
			{
				string fieldName = typeof(CCProcessingCenterDetail.value).Name;

				switch (row.ControlType)
				{
					case ControlTypeDefintion.Combo:
						List<string> labels = new List<string>();
						List<string> values = new List<string>();
						foreach (KeyValuePair<string, string> kv in row.GetComboValues())
						{
							values.Add(kv.Key);
							labels.Add(kv.Value);
						}
						e.ReturnState = PXStringState.CreateInstance(e.ReturnState, CCProcessingCenterDetail.ValueFieldLength, null, fieldName, false, 1, null,
																values.ToArray(), labels.ToArray(), true, null);
						break;
					case ControlTypeDefintion.CheckBox:
						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(Boolean), false, null, -1, null, null, null, fieldName,
								null, null, null, PXErrorLevel.Undefined, null, null, null, PXUIVisibility.Undefined, null, null, null);
						break;
					
					case ControlTypeDefintion.Password:
						//handled by PXRSACryptStringWithConditional attribute
						break;
					default:
						break;
				}
			}
		}

		class procDetail : ISettingsDetail
		{
			public int? ControlType { get; set; }
			public string Descr { get; set; }
			public string DetailID { get; set; }
			public string Value { get; set; }
			public bool? IsEncryptionRequired { get; set; }

			private IList<KeyValuePair<string, string>> comboValues;

			public IList<KeyValuePair<string, string>> GetComboValues()
			{
				return comboValues;
			}
			public void SetComboValues(IList<KeyValuePair<string, string>> list)
			{
				comboValues = list;
			}
		}

		protected virtual void CCProcessingCenterDetail_Value_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue == null) return;
			
			CCProcessingCenter processingCenter = ProcessingCenter.Current;
			CCProcessingCenterDetail procCenterDetail = (CCProcessingCenterDetail)e.Row;
			//skip validation for special values - plugins don't know about this detail ids
			if (!InterfaceConstants.SpecialDetailIDs.Contains(procCenterDetail.DetailID))
			{
				CCPaymentProcessing processingGraph = PXGraph.CreateInstance<CCPaymentProcessing>();
				ISettingsDetail idetail = new procDetail() { DetailID = procCenterDetail.DetailID, Value = (string)e.NewValue };
				processingGraph.ValidateSettings(this, processingCenter.ProcessingCenterID, idetail);
			}
		}
	}

}
