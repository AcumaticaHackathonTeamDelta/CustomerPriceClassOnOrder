using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.AR;

namespace PX.Objects.CA
{
	public class PaymentTypeInstanceMaint : PXGraph<PaymentTypeInstanceMaint, PaymentTypeInstance>
	{

		#region Public Selects
		public PaymentTypeInstanceMaint()
		{
			//ARSetup setup = ARSetup.Current;
			this.Details.Cache.AllowInsert = false;
			this.Details.Cache.AllowDelete = false;
			PXUIFieldAttribute.SetEnabled<PaymentTypeInstanceDetail.detailID>(this.Details.Cache, null, false);

		}
		public PXSelect<PaymentTypeInstance,
				Where<PaymentTypeInstance.cashAccountID, Equal<Optional<PaymentTypeInstance.cashAccountID>>>> PaymentTypeInstance;
		public PXSelectJoin<PaymentTypeInstanceDetail, LeftJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<PaymentTypeInstanceDetail.paymentMethodID>,
					And<PaymentMethodDetail.detailID, Equal<PaymentTypeInstanceDetail.detailID>,
                    And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForAPCards>>>>>,
				Where<PaymentTypeInstanceDetail.pTInstanceID, Equal<Current<PaymentTypeInstance.pTInstanceID>>>,
                    OrderBy<Asc<PaymentMethodDetail.orderIndex>>> Details;
		public PXSelect<PaymentMethodDetail,
				Where<PaymentMethodDetail.paymentMethodID, Equal<Optional<PaymentTypeInstance.paymentMethodID>>,
                And<PaymentMethodDetail.useFor,Equal<PaymentMethodDetailUsage.useForAPCards>>>> PMDetails;
		public PXSelect<PaymentMethod,
				Where<PaymentMethod.paymentMethodID, Equal<Optional<PaymentTypeInstance.paymentMethodID>>>> PaymentTypeDef;

		//public PXSetup<ARSetup> ARSetup;
		#endregion

		#region Main Record Events

		protected virtual void PaymentTypeInstance_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
			if (row != null)
			{
				cache.SetDefaultExt<PaymentTypeInstance.pTInstanceID>(row);
			}
		}

		protected virtual void PaymentTypeInstance_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
			if (row != null && (string.IsNullOrEmpty(row.PaymentMethodID)==false))
			{
				this.ClearDetails();
				this.AddDetails();				
			}
		}

		protected virtual void PaymentTypeInstance_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
			if (row != null)
			{
				if (row.BAccountID != null)
				{
					this.bAccountID = row.BAccountID;
				}

				PXUIFieldAttribute.SetEnabled<PaymentTypeInstance.descr>(cache, row, false);

				if (!String.IsNullOrEmpty(row.PaymentMethodID))
				{
					PaymentMethod pmDef = (PaymentMethod)this.PaymentTypeDef.Select();

					//bool singleInstance = pmDef.ARIsOnePerCustomer ?? false;
					bool singleInstance = false;
					bool isIDMaskExists = false;
					if (!singleInstance)
					{
						foreach (PaymentMethodDetail iDef in this.PMDetails.Select(row.PaymentMethodID))
						{
							if ((iDef.IsIdentifier ?? false) && (!string.IsNullOrEmpty(iDef.DisplayMask)))
							{
								isIDMaskExists = true;
								break;
							}
						}
					}
					if (!(isIDMaskExists || singleInstance))
					{
						PXUIFieldAttribute.SetEnabled<PaymentTypeInstance.descr>(cache, row, true);
					}
					PXDefaultAttribute.SetPersistingCheck<PaymentTypeInstance.descr>(cache, row, (isIDMaskExists ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank));
				}

				bool isInserted = (cache.GetStatus(e.Row) == PXEntryStatus.Inserted);
				PXUIFieldAttribute.SetEnabled<PaymentTypeInstance.paymentMethodID>(cache, row, (isInserted || String.IsNullOrEmpty(row.PaymentMethodID)));
				if (!isInserted && (!String.IsNullOrEmpty(row.PaymentMethodID)))
				{
					this.MergeDetailsWithDefinition(row.PaymentMethodID);
				}
				if (!isInserted) 
				{
					this.PaymentTypeInstance.Cache.AllowDelete = !this.HasTransactions(row);					
				}
			}
		}
		protected virtual void PaymentTypeInstance_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
				PaymentMethod def = this.PaymentTypeDef.Select();
				//bool singleInstance = def.ARIsOnePerCustomer ?? false;
				bool singleInstance = false;
				if (singleInstance)
				{
					PaymentTypeInstance existing = PXSelect<PaymentTypeInstance,
					Where<PaymentTypeInstance.bAccountID, Equal<Required<PaymentTypeInstance.bAccountID>>,
					And<PaymentTypeInstance.paymentMethodID, Equal<Required<PaymentTypeInstance.paymentMethodID>>,
					And<PaymentTypeInstance.pTInstanceID, NotEqual<Required<PaymentTypeInstance.pTInstanceID>>>>>>.Select(this, row.BAccountID, row.PaymentMethodID, row.PTInstanceID);
					if (existing != null)
					{
						//throw new PXException(Messages.PaymentTypeIsAlreadyDefined);
					}
				}
				foreach (PaymentTypeInstanceDetail iDet in this.Details.Select())
				{
					if (!ValidateDetail(iDet))
						e.Cancel = true;
				}
			}
		}
		protected virtual void PaymentTypeInstance_BAccountID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
			if (row != null && row.BAccountID == null)
			{
				row.BAccountID = this.bAccountID;
			}
		}
		protected virtual void PaymentTypeInstance_PaymentTypeID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			PaymentTypeInstance row = (PaymentTypeInstance)e.Row;
			this.ClearDetails();
			this.AddDetails();
			//row.CashAccountID = null;
			//cache.SetDefaultExt<PaymentTypeInstance.cashAccountID>(e.Row);
			PaymentMethod pmDef = this.PaymentTypeDef.Select();
			//bool singleInstance = pmDef.ARIsOnePerCustomer ?? false;
			bool singleInstance = false; 
			if (singleInstance)
			{
				row.Descr = pmDef.Descr;
			}
			this.Details.View.RequestRefresh();
		}
		#endregion

		#region PM Details Events
		protected virtual void PaymentTypeInstanceDetail_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			PaymentTypeInstanceDetail row = (PaymentTypeInstanceDetail)e.Row;
			PaymentMethodDetail def = this.FindTemplate(row);
			if (def.IsIdentifier ?? false)
			{
				this.PaymentTypeInstance.Current.Descr = null;
			}
		}
		protected virtual void PaymentTypeInstanceDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PaymentTypeInstanceDetail row = (PaymentTypeInstanceDetail)e.Row;
			if (row != null)
			{
				PaymentMethodDetail iTempl = this.FindTemplate(row);
				bool isRequired = (iTempl != null) && (iTempl.IsRequired ?? false);
				PXDefaultAttribute.SetPersistingCheck<PaymentTypeInstanceDetail.value>(cache, row, (isRequired) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				bool showDecripted = !(iTempl.IsEncrypted ?? false);
				PXRSACryptStringAttribute.SetDecrypted<PaymentTypeInstanceDetail.value>(cache, row, showDecripted);
			}


		}
		protected virtual void PaymentTypeInstanceDetail_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				PaymentTypeInstanceDetail row = (PaymentTypeInstanceDetail)e.Row;
				//ValidateDetail(row);
			}

		}
		protected virtual void PaymentTypeInstanceDetail_Value_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			PaymentTypeInstanceDetail row = e.Row as PaymentTypeInstanceDetail;
			PaymentMethodDetail def = this.FindTemplate(row);
			if (def != null)
			{
				if (def.IsIdentifier ?? false)
				{
					string id = CustomerPaymentMethodMaint.IDObfuscator.MaskID(row.Value, def.DisplayMask);
					if (this.PaymentTypeInstance.Current.Descr != id)
					{
						PaymentTypeInstance parent = this.PaymentTypeInstance.Current;
						parent.Descr = String.Format("{0}:{1}", parent.PaymentMethodID, id);
						this.PaymentTypeInstance.Update(parent);
					}
				}
				//bool isExpirationDate = def.IsExpirationDate ?? false;
				bool isExpirationDate = false;
				if (isExpirationDate)
				{
					PaymentTypeInstance parent = this.PaymentTypeInstance.Current;
					try
					{
						parent.ExpirationDate = ParseExpiryDate(row.Value);
					}
					catch (FormatException)
					{
						parent.ExpirationDate = null;
						this.Details.Cache.RaiseExceptionHandling<PaymentTypeInstanceDetail.value>(row, row.Value, new PXSetPropertyException(Messages.ERR_IncorrectFormatOfPTInstanceExpiryDate));
					}
					this.PaymentTypeInstance.Update(parent);
				}
			}
		}


		#endregion

		#region Internal Functions
		protected virtual PaymentMethodDetail FindTemplate(PaymentTypeInstanceDetail aDet)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
				And<PaymentMethodDetail.detailID, Equal<Required<PaymentMethodDetail.detailID>>,
                    And<PaymentMethodDetail.useFor,Equal<PaymentMethodDetailUsage.useForAPCards>>>>>.Select(this, aDet.PaymentMethodID, aDet.DetailID);
			return res;
		}
		protected virtual void ClearDetails()
		{
			foreach (PaymentTypeInstanceDetail iDet in this.Details.Select())
			{
				this.Details.Delete(iDet);
			}
		}
		protected virtual void AddDetails()
		{
			if (this.PaymentTypeInstance.Current != null)
			{
				string pmID = PaymentTypeInstance.Current.PaymentMethodID;
				if (!String.IsNullOrEmpty(pmID))
				{
					foreach (PaymentMethodDetail it in this.PMDetails.Select(pmID))
					{
						PaymentTypeInstanceDetail det = new PaymentTypeInstanceDetail();
						det.DetailID = it.DetailID;
						det.PaymentMethodID = pmID;
						det = this.Details.Insert(det);

					}
				}
			}
		}

		protected virtual void MergeDetailsWithDefinition(string aPaymentType)
		{
			if (aPaymentType != this.mergedPaymentType)
			{
				List<PaymentMethodDetail> toAdd = new List<PaymentMethodDetail>();
				foreach (PaymentMethodDetail it in this.PMDetails.Select(aPaymentType))
				{
					PaymentTypeInstanceDetail detail = null;
					foreach (PaymentTypeInstanceDetail iPDet in this.Details.Select())
					{
						if (iPDet.DetailID == it.DetailID)
						{
							detail = iPDet;
							break;
						}
					}
					if (detail == null)
					{
						toAdd.Add(it);
					}
				}
				using (ReadOnlyScope rs = new ReadOnlyScope(this.Details.Cache))
				{
					foreach (PaymentMethodDetail it in toAdd)
					{
						PaymentTypeInstanceDetail detail = new PaymentTypeInstanceDetail();
						detail.DetailID = it.DetailID;
						detail = this.Details.Insert(detail);
					}
					if (toAdd.Count > 0)
					{
						this.Details.View.RequestRefresh();
					}
				}
				this.mergedPaymentType = aPaymentType;
			}
		}
		protected virtual bool ValidateDetail(PaymentTypeInstanceDetail aRow)
		{
			PaymentMethodDetail iTempl = this.FindTemplate(aRow);
			if (iTempl != null && ((iTempl.IsRequired ?? false) || (iTempl.IsIdentifier ?? false)) && String.IsNullOrEmpty(aRow.Value))
			{
				this.Details.Cache.RaiseExceptionHandling<PaymentTypeInstanceDetail.value>(aRow, aRow.Value, new PXSetPropertyException(Messages.ERR_RequiredValueNotEnterd));
				return false;
			}
			return true;
		}

		protected virtual bool HasTransactions(PaymentTypeInstance aRow) 
		{
			PXEntryStatus status = this.PaymentTypeInstance.Cache.GetStatus(aRow);
			if (status == PXEntryStatus.Inserted && status == PXEntryStatus.InsertedDeleted) 
			{
				return false;
			}
			PTInstTran tran = PXSelectReadonly<PTInstTran, Where<PTInstTran.pTInstanceID, Equal<Required<PTInstTran.pTInstanceID>>>>.SelectWindowed(this, 0, 1, aRow.PTInstanceID);
			return tran!=null;
		}
		#endregion

		#region Utilities
		public static DateTime ParseExpiryDate(string aValue)
		{
			DateTime datetime;
			try
			{
				datetime = DateTime.ParseExact(aValue, "Myyyy", null);
			}
			catch (FormatException)
			{
				datetime = DateTime.ParseExact(aValue, "Myy", null);
			}
			//Set Date to last date of month
			return new DateTime(datetime.Year, datetime.Month, 1).AddMonths(1).AddDays(-1);
		}

		#endregion

		#region Private Functions
		private int? bAccountID;
		private string mergedPaymentType;
		#endregion

	}

}
