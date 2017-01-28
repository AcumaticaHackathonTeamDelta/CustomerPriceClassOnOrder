using PX.CCProcessingBase;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing;
using System;

namespace PX.Objects.CA
{

	[Serializable]
	public class CCUpdateExpirationDatesProcess : PXGraph<CCUpdateExpirationDatesProcess>
	{

		public partial class CPMFilter : IBqlTable
		{
			#region ProcessingCenterID
			public abstract class processingCenterID : PX.Data.IBqlField
			{
			}
			protected String _ProcessingCenterID;
			[PXString(10, IsUnicode = true, IsKey = true)]
			[PXSelector(typeof(Search<CCProcessingCenter.processingCenterID>))]
			[PXUIField(DisplayName = "Proc. Center ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual String ProcessingCenterID
			{
				get
				{
					return this._ProcessingCenterID;
				}
				set
				{
					this._ProcessingCenterID = value;
				}
			}
			#endregion
		}

		public PXCancel<CPMFilter> Cancel;
		public PXFilter<CPMFilter> Filter;
		public PXFilteredProcessing<CustomerPaymentMethod, CPMFilter, Where<CustomerPaymentMethod.cCProcessingCenterID, 
			Equal<Current<CPMFilter.processingCenterID>>, And<CustomerPaymentMethod.expirationDate, IsNull>>> CustomerPaymentMethods;

		public CCUpdateExpirationDatesProcess()
		{
			this.CustomerPaymentMethods.SetProcessDelegate(cpm => DoDateUpdateProcess(cpm));
		}

		public static void DoDateUpdateProcess(CustomerPaymentMethod cpm)
		{
			CustomerPaymentMethodMaint cpmGraph = PXGraph.CreateInstance<CustomerPaymentMethodMaint>();
			cpmGraph.CustomerPaymentMethod.Current = cpm;
			CCCustomerInformationManager.SyncExistingPMI(cpmGraph, cpmGraph.CustomerPaymentMethod, cpmGraph.DetailsAll);
			cpm.ExpirationDate = cpmGraph.CustomerPaymentMethod.Current.ExpirationDate;
			if (cpm.ExpirationDate == null)
			{
				string formatString = CCProcessingUtils.GetExpirationDateFormat(cpmGraph, cpm.CCProcessingCenterID);
				string responseString = null;
				foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> details in cpmGraph.DetailsAll.Select())
				{
					PaymentMethodDetail pmDetail = details;
					CustomerPaymentMethodDetail cpmDetail = details;
					if (pmDetail.IsExpirationDate == true)
					{
						responseString = cpmDetail.Value;
						break;
					}
				}
				throw new PXException(Messages.ExpDateParseFailed, InterfaceConstants.ExpDateFormatDesc, responseString, formatString);
			}
			cpmGraph.Save.Press();
		}

	}
}
