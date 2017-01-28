using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;

namespace PX.Objects.SO
{
	public class SOOrderShipmentProcess : PXGraph<SOOrderShipmentProcess, SOOrderShipment>
	{
        public PXSelect<SOOrder> Orders;
        public PXSelect<SOShipment> Shipments;

		public PXAction<SOShipment> flow;
		[PXUIField(DisplayName = "Flow")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Flow(PXAdapter adapter)
		{
			Save.Press();
			return adapter.Get();
		}

		public PXSelectJoin<SOOrderShipment,
			InnerJoin<SOOrder, 
				On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, 
				And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
			InnerJoin<ARInvoice,
				On<ARInvoice.docType, Equal<SOOrderShipment.invoiceType>,
					And<ARInvoice.refNbr, Equal<SOOrderShipment.invoiceNbr>,
						And<ARInvoice.released, Equal<boolTrue>>>>>>,
			Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, 
				And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>> Items;
		public PXSelect<SOAdjust,
				Where<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>,
					And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>>>> Adjustments;

        protected virtual void SOOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            SOOrder doc = (SOOrder)e.Row;
            if (e.Operation == PXDBOperation.Update)
            {
                if (doc.ShipmentCntr < 0 || doc.OpenShipmentCntr < 0 || doc.ShipmentCntr < doc.BilledCntr + doc.ReleasedCntr && doc.Behavior == SOBehavior.SO)
                {
                    throw new PXSetPropertyException(Messages.InvalidShipmentCounters);
                }
            } 
        }

		public SOOrderShipmentProcess()
		{
		}

	}
}
