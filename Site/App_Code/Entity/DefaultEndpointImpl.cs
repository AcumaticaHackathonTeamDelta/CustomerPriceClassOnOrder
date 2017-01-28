
using System;
using System.Linq;
using PX.Api;
using PX.Api.ContractBased;
using PX.Api.ContractBased.Models;
using PX.Api.ContractBased.Soap;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.AR;
using PX.Objects.CA;
using System.Collections.Generic;

[PXVersion("5.30.001", "Default")]
[PXVersion("6.00.001", "Default")]
public class DefaultEndpointImpl
{
    [FieldsProcessed(new [] {
        "OrderType",
        "OrderNbr",
        "OrderLineNbr",
        "InventoryID"
    })]
    protected void ShipmentDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var shipmentEntry = (SOShipmentEntry)graph;

        var filterCache = shipmentEntry.addsofilter.Cache;
        var filter = (AddSOFilter)filterCache.Current;

        var orderType = targetEntity.Fields.Single(f => f.Name == "OrderType") as EntityValueField;
        var orderNbr = targetEntity.Fields.Single(f => f.Name == "OrderNbr") as EntityValueField;

        filter.OrderType = orderType.Value;
        filter.OrderNbr = orderNbr.Value;
        filterCache.Update(filter);

        var orderLineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderLineNbr") as EntityValueField;
        Func<PXResult<SOShipmentPlan>, bool> lineCriteria;
        if (orderLineNbr != null && !string.IsNullOrEmpty(orderLineNbr.Value))
        {
            var nbr = int.Parse(orderLineNbr.Value);
            lineCriteria = r => r.GetItem<SOLineSplit>().LineNbr == nbr;
        }
        else
        {
            var inventoryId = targetEntity.Fields.SingleOrDefault(f => f.Name == "InventoryID") as EntityValueField;
            if (inventoryId == null || string.IsNullOrWhiteSpace(inventoryId.Value))
                throw new InvalidOperationException("Neither OrderLineNbr nor InventoryID specified for ShipmentDetail");
            var inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(graph, inventoryId.Value).FirstOrDefault().GetItem<InventoryItem>();
            lineCriteria = r => r.GetItem<SOLineSplit>().InventoryID == inventoryItem.InventoryID;
        }

        SOShipmentPlan item = shipmentEntry.soshipmentplan.Select().Single(lineCriteria);
        item.Selected = true;
        shipmentEntry.soshipmentplan.Cache.Update(item);
        AssertNoErrors(shipmentEntry.soshipmentplan.Cache, item);
        shipmentEntry.Actions["AddSO"].Press();

        var shipLineCurrent = shipmentEntry.Caches[typeof(SOShipLine)].Current as SOShipLine;
        if (shipLineCurrent == null)
            throw new InvalidOperationException("Can't add ShipmentDetail");

        var allocations = targetEntity.Fields.SingleOrDefault(f => string.Equals(f.Name, "Allocations")) as EntityListField;
        if (allocations != null) // clear already allocated lines, to replace them with new
        {
                shipLineCurrent.ShippedQty = 0;
                shipLineCurrent.BaseShippedQty = 0;
                shipmentEntry.Caches[typeof(SOShipLine)].Update(shipLineCurrent);

            var current = shipmentEntry.splits.Cache.Current as SOShipLineSplit;
            if (current != null)
            {
                shipmentEntry.splits.Cache.Clear();
            }

            var current2 = shipmentEntry.Caches[typeof(SOLineSplit2)].Current as SOLineSplit2;
            if (current2 != null)
            {
                current2.ShippedQty = 0;
                shipmentEntry.Caches[typeof(SOLineSplit2)].Update(current2);
            }
        }
    }

    private void AssertNoErrors(PXCache cache, object current)
    {
        var errors = PXUIFieldAttribute.GetErrors(cache, current);
        if (errors.Count == 0)
            return;

        throw new InvalidOperationException(string.Join("\n", errors.Select(p => p.Key + ": " + p.Value)));
    }

    [FieldsProcessed(new[] {
        "ShipmentNbr",
        "OrderNbr",
        "OrderType"
    })]
    protected void SalesInvoiceDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (SOInvoiceEntry)graph;

        var shipmentNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "ShipmentNbr") as EntityValueField;
        var orderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderNbr") as EntityValueField;
        var orderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderType") as EntityValueField;

        var shipment = shipmentNbr != null ? shipmentNbr.Value : null;
        var number = orderNbr != null ? orderNbr.Value : null;
        var type = orderType != null ? orderType.Value : null;

        if (shipment == null && number == null && type == null)
        {
            throw new PXException("Shipment or Sales Order information not filled.");
        }
        
        var shipments = maint.shipmentlist.Select().Select(s => s.GetItem<SOOrderShipment>())
            .Where(s => (shipment == null || s.ShipmentNbr.OrdinalEquals(shipment))
                && (number == null || s.OrderNbr.OrdinalEquals(number))
                && (type == null || s.OrderType.OrdinalEquals(type)));

        if (!shipments.Any())
        {
            throw new PXException("Shipments not found.");
        }

        foreach (var item in shipments)
        {
            item.Selected = true;
            maint.shipmentlist.Update(item);
            maint.Actions["AddShipment"].Press();
        }
    }

    [FieldsProcessed(new[] {
        "AttributeID",
        "Value"
    })]
    protected void AttributeValue_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var attributeIdField = targetEntity.Fields.Single(f => f.Name == "AttributeID") as EntityValueField;
        var valueField = targetEntity.Fields.Single(f => f.Name == "Value") as EntityValueField;

        var view = graph.Views[PX.Objects.CS.Messages.CSAnswers];
        var cache = view.Cache;

        var rows = view.SelectMulti().OrderBy(row =>
        {
            var orderState = cache.GetStateExt(row, "Order") as PXFieldState;
            return orderState.Value;
        }).ToArray();

        foreach (var row in rows)
        {
            var attributeId = (cache.GetStateExt(row, "AttributeID") as PXFieldState).Value.ToString();
            if (attributeIdField.Value.OrdinalEquals(attributeId))
            {
                var state = cache.GetStateExt(row, "Value") as PXStringState;
                if (state != null && state.ValueLabelDic != null)
                {
                    foreach (var rec in state.ValueLabelDic)
                    {
                        if (rec.Value == valueField.Value)
                        {
                            valueField.Value = rec.Key;
                            break;
                        }
                    }
                }
                cache.SetValueExt(row, "Value", valueField.Value);
                cache.Update(row);
                break;
            }
        }
        if (graph.IsDirty)
        {
            graph.Actions["Save"].Press();
        }
    }

    [FieldsProcessed(new[] {
        "POLineNbr",
        "POOrderType",
        "POOrderNbr"
    })]
    protected void PurchaseReceiptDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var receiptEntry = (POReceiptEntry)graph;

        var lineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POLineNbr") as EntityValueField;
        var orderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderType") as EntityValueField;
        var orderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderNbr") as EntityValueField;

        bool insertViaAddPO = lineNbr != null && orderNbr != null && orderType != null;

        if (!insertViaAddPO && (lineNbr != null || orderType != null || orderNbr != null))
        {
            throw new PXException("POLineNbr, POOrderType, POOrderNbr must be filled to add purchase order to details.");
        }

        var detailsCache = receiptEntry.transactions.Cache;

        if (insertViaAddPO)
        {
            receiptEntry.filter.Cache.Remove(receiptEntry.filter.Current);
            receiptEntry.filter.Cache.Insert(new POReceiptEntry.POOrderFilter());
            var filter = receiptEntry.filter.Current;

            var state = receiptEntry.filter.Cache.GetStateExt(filter, "OrderType") as PXStringState;
            if (state != null && state.AllowedLabels.Contains(orderType.Value))
            {
                orderType.Value = state.ValueLabelDic.Single(p => p.Value == orderType.Value).Key;
            }

            receiptEntry.filter.Cache.SetValueExt(filter, "OrderType", orderType.Value);
            receiptEntry.filter.Cache.SetValueExt(filter, "OrderNbr", orderNbr.Value);
            receiptEntry.filter.Update(filter);

            var orders = receiptEntry.poLinesSelection.Select().Select(r => r.GetItem<POReceiptEntry.POLineS>());
            var order = orders.FirstOrDefault(o => o.LineNbr == int.Parse(lineNbr.Value));
            if (order == null)
            {
                throw new PXException("Purchase order line not found.");
            }

            order.Selected = true;
            receiptEntry.poLinesSelection.Update(order);
            receiptEntry.Actions["AddPOOrderLine2"].Press();
        }
        else
        {
            var row = detailsCache.CreateInstance();
            detailsCache.Insert(row);
            detailsCache.Current = row;
        }
    }

    /// <summary>
    /// Adds all lines of a given order to the shipment.
    /// </summary>
    protected void Action_AddOrder(PXGraph graph, ActionImpl action)
    {
        SOShipmentEntry shipmentEntry = (SOShipmentEntry)graph;
        shipmentEntry.addsofilter.Current.OrderType = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderType")).Value;
        shipmentEntry.addsofilter.Current.OrderNbr = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderNbr")).Value;
        shipmentEntry.addsofilter.Update(shipmentEntry.addsofilter.Current);

        foreach (SOShipmentPlan line in shipmentEntry.soshipmentplan.Select())
        {
            line.Selected = true;
            shipmentEntry.soshipmentplan.Update(line);
        }

        shipmentEntry.addSO.Press();
    }

    [FieldsProcessed(new[] {
        "Name",
        "Description",
        "Value"
    })]
    protected void CustomerPaymentMethodDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (CustomerPaymentMethodMaint)graph;

        var name = (EntityValueField)targetEntity.Fields.SingleOrDefault(f => f.Name.OrdinalEquals("Name"));
        var description = (EntityValueField)targetEntity.Fields.SingleOrDefault(f => f.Name.OrdinalEquals("Description"));
        var value = (EntityValueField)targetEntity.Fields.Single(f => f.Name.OrdinalEquals("Value"));

        var cache = maint.Details.Cache;
        foreach (CustomerPaymentMethodDetail detail in maint.Details.Select())
        {
            var selectorRow = PXSelectorAttribute.Select(cache, detail, "DetailID") as PaymentMethodDetail;
            if ((name != null && (selectorRow.Descr == name.Value || detail.DetailID == name.Value))
                || (description != null && (selectorRow.Descr == description.Value || selectorRow.DetailID == description.Value)))
            {

                cache.SetValueExt(detail, "Value", value.Value);
                maint.Details.Update(detail);
                break;
            }
        }
    }

    [FieldsProcessed(new[] {
        "ParentCategoryID",
        "Description"
    })]
    protected void ItemSalesCategory_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (INCategoryMaint)graph;

        maint.Folders.Current = maint.Folders.SelectSingle();
        maint.Actions["AddCategory"].Press();

        var parentCategoryId = entity.Fields.SingleOrDefault(f => f.Name.Equals("ParentCategoryID", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
        var description = entity.Fields.SingleOrDefault(f => f.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)) as EntityValueField;

        var item = maint.Folders.Cache.ActiveRow as INCategory;

        var cache = maint.Folders.Cache;

        if (parentCategoryId != null && !string.IsNullOrEmpty(parentCategoryId.Value))
        {
            cache.SetValueExt(item, "ParentID", int.Parse(parentCategoryId.Value));
        }

        if (description != null && !string.IsNullOrEmpty(description.Value))
        {
            cache.SetValueExt(item, "Description", description.Value);
        }

        maint.Folders.Cache.Current = item;
    }

    [FieldsProcessed(new[] {
        "TypeID",
        "Description",
        "WarehouseID"
    })]
    protected void PhysicalInventoryReview_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (INPIReview)graph;

        var typeId = targetEntity.Fields.SingleOrDefault(f => f.Name == "TypeID") as EntityValueField;
        var description = targetEntity.Fields.SingleOrDefault(f => f.Name == "Description") as EntityValueField;
        var warehouseid = targetEntity.Fields.SingleOrDefault(f => f.Name == "WarehouseID") as EntityValueField;


        var cache = maint.GeneratorSettings.Cache;
        cache.Clear();
        cache.Insert(new PIGeneratorSettings());

        var updateDic = new Dictionary<string, object>();
        if (typeId != null)
        {
            updateDic.Add("PIClassID", typeId.Value);
        }
        maint.ExecuteUpdate(maint.GeneratorSettings.View.Name, new Dictionary<string, object>(), updateDic);

        updateDic = new Dictionary<string, object>();
        if (description != null)
        {
            updateDic.Add("Descr", description.Value);
        }
        if (warehouseid != null)
        {
            updateDic.Add("SiteID", warehouseid.Value);
        }
        maint.ExecuteUpdate(maint.GeneratorSettings.View.Name, new Dictionary<string, object>(), updateDic);

        maint.GeneratorSettings.View.SetAnswer(null, WebDialogResult.OK);
        maint.Insert.Press();
    }


    protected void ItemSalesCategory_Delete(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (INCategoryMaint)graph;
        maint.Actions["DeleteCategory"].Press();
        maint.Actions["Save"].Press();
    }

    [FieldsProcessed(new[] {
        "Description",
        "ParentCategoryID"
    })]
    protected void ItemSalesCategory_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (INCategoryMaint)graph;

        var item = maint.Folders.Cache.Current as INCategory;

        var description = entity.Fields.SingleOrDefault(f => f.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
        if (description != null && !string.IsNullOrEmpty(description.Value))
        {
			maint.Folders.Cache.SetValueExt<INCategory.description>(item, description.Value);
        }

        var parent = entity.Fields.SingleOrDefault(f => f.Name.Equals("ParentCategoryID", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
        if (parent != null && !string.IsNullOrEmpty(parent.Value))
        {
            item.ParentID = int.Parse(parent.Value);
        }

        maint.Folders.Update(item);
    }

    protected INUnit GetINUnit(InventoryItemMaint maint, EntityValueField fromUnit, EntityValueField toUnit)
    {
        var conversions = maint.itemunits.Select();
        return conversions.Select(c => c[typeof(INUnit)] as INUnit)
            .FirstOrDefault(c => c != null 
                && (toUnit == null || string.IsNullOrEmpty(toUnit.Value) || string.Equals(c.ToUnit, toUnit.Value)) 
                && string.Equals(c.FromUnit, fromUnit.Value));
    }

    [FieldsProcessed(new[] {
        "ToUOM",
        "FromUOM"
    })]
    protected void InventoryItemUOMConversion_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
    {
        var maint = (InventoryItemMaint)graph;

        var fromUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "FromUOM") as EntityValueField;
        var toUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "ToUOM") as EntityValueField;

        var conversion = GetINUnit(maint, fromUnit, toUnit);       
        if (conversion == null)
        {
            conversion = maint.itemunits.Insert(new INUnit()
            {
                ToUnit = toUnit != null && !string.IsNullOrEmpty(toUnit.Value) ? toUnit.Value : null,
                FromUnit = fromUnit.Value
            });
        }

        maint.itemunits.Current = conversion;
    }


	protected void InventoryItemUOMConversion_Delete(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
	{
		var maint = (InventoryItemMaint)graph;

		var fromUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "FromUOM") as EntityValueField;
		var toUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "ToUOM") as EntityValueField;

        var conversion = GetINUnit(maint, fromUnit, toUnit);
        if (conversion != null)
		{
			maint.itemunits.Delete(conversion);
			maint.Save.Press();
		}
	}
}
