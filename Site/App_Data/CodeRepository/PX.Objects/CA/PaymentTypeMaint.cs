using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;

namespace PX.Objects.CA
{
	
#if false
	public class PaymentTypeMaint : PXGraph<PaymentTypeMaint, PaymentType>
    {
        
        #region Ctor + Selects
        public PXSelect<PaymentType> PaymentType;
        public PXSelect<PaymentType, Where<PaymentType.paymentTypeID, Equal<Current<PaymentType.paymentTypeID>>>> CurrentPaymentType;
        public PXSelect<PaymentTypeDetail, Where<PaymentTypeDetail.paymentTypeID, Equal<Current<PaymentType.paymentTypeID>>>, OrderBy<Asc<PaymentTypeDetail.orderIndex>>> Details;
        public PXSelect<PaymentTypeDetail,
                Where<PaymentTypeDetail.paymentTypeID, Equal<Current<PaymentType.paymentTypeID>>,
                And<Where<PaymentTypeDetail.useFor,Equal<PaymentTypeDetailUsage.useForCashAccount>,
                    Or<PaymentTypeDetail.useFor,Equal<PaymentTypeDetailUsage.useForAll>>>>>,
                OrderBy<Asc<PaymentTypeDetail.orderIndex>>> DetailsForCashAccount;
        public PXSelect<PaymentTypeDetail,
                Where<PaymentTypeDetail.paymentTypeID, Equal<Current<PaymentType.paymentTypeID>>,
                And<Where<PaymentTypeDetail.useFor,Equal<PaymentTypeDetailUsage.useForVendor>,
                  Or<PaymentTypeDetail.useFor,Equal<PaymentTypeDetailUsage.useForAll>>>>>,
                OrderBy<Asc<PaymentTypeDetail.orderIndex>>> DetailsForVendor;

        public PaymentTypeMaint()
        {
            GL.GLSetup setup = GLSetup.Current;
        }
        public PXSetup<GL.GLSetup> GLSetup; 
        #endregion

		protected virtual void PaymentType_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
            
            PaymentType row = (PaymentType)e.Row;
			bool allowInstances = (row.AllowInstances == true);
			bool printChecks	= (row.PrintChecks    == true);
			bool createBatch = (row.CreateBatch == true);
			PXCache detCache = this.Details.Cache;            
			PXUIFieldAttribute.SetEnabled<PaymentType.reportID> (sender, row, printChecks);
			PXUIFieldAttribute.SetEnabled<PaymentType.printRemittanceReport>(sender, row, printChecks);
			PXUIFieldAttribute.SetEnabled<PaymentType.remittanceReportID>(sender, row, printChecks && (row.PrintRemittanceReport == true));
			PXUIFieldAttribute.SetRequired<PaymentType.reportID>(sender, printChecks);

			PXUIFieldAttribute.SetVisible<PaymentTypeDetail.displayMask>	(detCache, null, allowInstances);
			PXUIFieldAttribute.SetVisible<PaymentTypeDetail.isIdentifier>	(detCache, null, allowInstances);
			PXUIFieldAttribute.SetVisible<PaymentTypeDetail.isEncrypted>	(detCache, null, allowInstances);

			//PXUIFieldAttribute.SetEnabled<PaymentType.cashAccountSettings>(sender, row, !allowInstances);
			//PXUIFieldAttribute.SetEnabled<PaymentType.vendorSettings>	  (sender, row, !allowInstances);
			PXUIFieldAttribute.SetEnabled<PaymentType.printChecks>		  (sender, row, !allowInstances);
			PXUIFieldAttribute.SetEnabled<PaymentType.stubLines>		  (sender, row, !allowInstances && printChecks);

			PXUIFieldAttribute.SetEnabled<PaymentType.createBatch>(sender, row, !allowInstances && !printChecks);
			PXUIFieldAttribute.SetEnabled<PaymentType.batchExportSYMappingID>(sender, row, createBatch);
			PXUIFieldAttribute.SetRequired<PaymentType.batchExportSYMappingID>(sender, createBatch);			
		}

		protected virtual void PaymentType_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			PaymentType row = (PaymentType)e.Row;
			if (row == null) return;

			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				if ((bool)row.PrintChecks)
				{
					if (string.IsNullOrEmpty(row.ReportID))
						sender.RaiseExceptionHandling<PaymentType.reportID>	(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(PaymentType.reportID).Name));
					if (row.StubLines == null || row.StubLines <=0)
						sender.RaiseExceptionHandling<PaymentType.stubLines>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(PaymentType.stubLines).Name));

					if (row.PrintRemittanceReport == true)
					{
						if (string.IsNullOrEmpty(row.RemittanceReportID))
							sender.RaiseExceptionHandling<PaymentType.remittanceReportID>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(PaymentType.remittanceReportID).Name));
					}
				}
			}
		}

		protected virtual void PaymentType_AllowInstances_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PaymentType row = (PaymentType)e.Row;
			if ((bool)row.AllowInstances)
			{
				//row.VendorSettings		= false;
				//row.CashAccountSettings = false;
				row.PrintChecks			= false;
			}
			else 
			{
				//sender.SetDefaultExt<PaymentType.vendorSettings>(row);
				//sender.SetDefaultExt<PaymentType.cashAccountSettings>(row);
				sender.SetDefaultExt<PaymentType.printChecks>(row);
			}
		}

		protected virtual void PaymentType_PrintChecks_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PaymentType row = (PaymentType)e.Row;
			if ((bool)row.PrintChecks )
			{
				row.CreateBatch = false;
				row.ReportID = null;
			}
			else
			{
				sender.SetDefaultExt<PaymentType.createBatch>(row);
			}
		}

        public override int ExecuteInsert(string viewName, IDictionary values, params object[] parameters)
        {
            switch (viewName)
            {
                case "DetailsForCashAccount":
                    values[CS.PXDataUtils.FieldName<PaymentTypeDetail.useFor>()] = "C";
                    break;
                case "DetailsForVendor":
                    values[CS.PXDataUtils.FieldName<PaymentTypeDetail.useFor>()] = "V";
                    break;
            }
            return base.ExecuteInsert(viewName, values, parameters);
        }

        protected virtual void PaymentTypeDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e) 
		{
			PXUIFieldAttribute.SetEnabled<PaymentTypeDetail.detailID>(cache, e.Row, false);
		}
        private bool errorKey;
        protected virtual void PaymentTypeDetail_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
        {
            if (errorKey)
            {
                errorKey = false;
                e.Cancel = true;
            }
            else
            {
                //string detID = ((PaymentTypeDetail)e.Row).DetailID;
                int? detID = ((PaymentTypeDetail)e.Row).DetailID;
                bool isExist = false;
                foreach (PaymentTypeDetail it in this.Details.Select())
                {
                    if (it.DetailID == detID)
                    {
                        isExist = true;
                    }
                }

                if (isExist)
                {
                    cache.RaiseExceptionHandling<PaymentTypeDetail.detailID>(e.Row, detID, new PXException(Messages.DuplicatedPaymentTypeDetail));
                    e.Cancel = true;
                }
            }
        }
        protected virtual void PaymentTypeDetail_DetailID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
        {
            PaymentTypeDetail a = e.Row as PaymentTypeDetail;
            if (a.DetailID != null)
            {
                errorKey = true;
            }
        }                

#if false
		protected virtual void PaymentTypeDetail_OrderIndex_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			PaymentTypeDetail a = e.Row as PaymentTypeDetail;
			if (a.DetailID != null)
			{
				e.NewValue = (short) a.DetailID.Value;
			}
		}
#endif
    }
  
#endif
}
