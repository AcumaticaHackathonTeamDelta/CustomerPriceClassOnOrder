using System.Collections;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;

namespace PX.Objects.DA
{
    public class SOOrderEntryDAExtension : PXGraphExtension<SOOrderEntry>
    {
        public virtual void SOOrder_DAPriceClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (SOOrder) e.Row;
            if (row == null)
            {
                return;
            }

            var extension = row.GetExtension<SOOrderDAExtension>();

            if (extension == null)
            {
                return;
            }

            foreach (SOLine transaction in Base.Transactions.Select())
            {
                SetSalesPrice(sender, Base.Document.Current, transaction);
            }
        }

        public virtual void SOLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            del?.Invoke(sender, e);
            SetSalesPrice(sender, Base.Document.Current, (SOLine)e.Row);
        }

        public virtual void SOLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            del?.Invoke(sender, e);
            SetSalesPrice(sender, Base.Document.Current, (SOLine)e.Row);
        }

        public virtual void SOLine_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            del?.Invoke(sender, e);
            SetSalesPrice(sender, Base.Document.Current, (SOLine)e.Row);
        }

        public virtual void SOLine_ManualPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            del?.Invoke(sender, e);
            SetSalesPrice(sender, Base.Document.Current, (SOLine) e.Row);
        }

        public virtual bool SetSalesPrice(PXCache sender, SOOrder order, SOLine line)
        {
            var salesPrice = GetSalesPrice(sender, Base.Document.Current, line);

            if (salesPrice == null 
                || line == null
                || line.ManualPrice.GetValueOrDefault())
            {
                return false;
            }

            Base.Transactions.Cache.SetValueExt<SOLine.curyUnitPrice>(line, GetSalesPrice(sender, Base.Document.Current, line));

            return true;
        }

        public virtual decimal? GetSalesPrice(PXCache sender, SOOrder order, SOLine line)
        {
            if (line == null
                || order == null)
            {
                return null;
            }

            var extension = order.GetExtension<SOOrderDAExtension>();

            if (extension == null)
            {
                return null;
            }

            return  ARSalesPriceMaint.CalculateSalesPrice(sender, extension.DAPriceClassID, order.CustomerID, line.InventoryID,
                    Base.currencyinfo.Select(), line.UOM, line.Qty, order.OrderDate.GetValueOrDefault(), line.CuryUnitPrice) ?? 0m;
        }
    }
}