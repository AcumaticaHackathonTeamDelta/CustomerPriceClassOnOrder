using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.Common.Extensions;

using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.IN;

namespace PX.Objects.AP
{
    [Serializable]
    public class APPriceWorksheetMaint : PXGraph<APPriceWorksheetMaint, APPriceWorksheet>
    {
        #region Selects/Views

        public PXSelect<APPriceWorksheet> Document;
        [PXImport(typeof(APPriceWorksheet))]
        public PXSelectJoin<APPriceWorksheetDetail,
                LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<APPriceWorksheetDetail.inventoryID>>>, Where<APPriceWorksheetDetail.refNbr, Equal<Current<APPriceWorksheet.refNbr>>>,
                OrderBy<Asc<APPriceWorksheetDetail.vendorID, Asc<InventoryItem.inventoryCD, Asc<APPriceWorksheetDetail.breakQty>>>>> Details;
        public PXSetup<APSetup> APSetup;
        public PXSelect<APVendorPrice> APVendorPrices;
        public PXSelect<Vendor> Vendor;

        [PXCopyPasteHiddenView]
        public PXFilter<CopyPricesFilter> CopyPricesSettings;
        [PXCopyPasteHiddenView]
        public PXFilter<CalculatePricesFilter> CalculatePricesSettings;
        public PXSelect<CurrencyInfo> CuryInfo;

        #endregion

        public APPriceWorksheetMaint()
        {
            APSetup setup = APSetup.Current;
        }

        #region Actions

        public PXAction<APPriceWorksheet> ReleasePriceWorksheet;
        [PXUIField(DisplayName = ActionsMessages.Release, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable releasePriceWorksheet(PXAdapter adapter)
        {
            List<APPriceWorksheet> list = new List<APPriceWorksheet>();
            if (Document.Current != null)
            {
                this.Save.Press();
                list.Add(Document.Current);
                PXLongOperation.StartOperation(this, delegate() { ReleaseWorksheet(Document.Current); });
            }
            return list;
        }

        public static void ReleaseWorksheet(APPriceWorksheet priceWorksheet)
        {
            APPriceWorksheetMaint vendorPriceWorksheetMaint = PXGraph.CreateInstance<APPriceWorksheetMaint>();
            vendorPriceWorksheetMaint.Document.Current = priceWorksheet;

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (APPriceWorksheetDetail priceLine in vendorPriceWorksheetMaint.Details.Select())
                {
                    PXResultset<APVendorPrice> salesPrices = PXSelect<APVendorPrice, Where<
                            APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                            And<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
                            And<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
                            And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,
                            And<APVendorPrice.breakQty, Equal<Required<APVendorPrice.breakQty>>,
                            And<APVendorPrice.isPromotionalPrice, Equal<Required<APVendorPrice.isPromotionalPrice>>>>>>>>,
                            OrderBy<Asc<APVendorPrice.effectiveDate, Asc<APVendorPrice.expirationDate>>>>.Select(vendorPriceWorksheetMaint,
                            priceLine.VendorID, priceLine.InventoryID, priceLine.UOM, priceLine.CuryID,
                            priceLine.BreakQty, vendorPriceWorksheetMaint.Document.Current.IsPromotional ?? false);

                    if (vendorPriceWorksheetMaint.Document.Current.IsPromotional != true || vendorPriceWorksheetMaint.Document.Current.ExpirationDate == null)
                    {
                        bool insertNewPrice = true;
                        if (salesPrices.Count > 0)
                        {
                            foreach (APVendorPrice salesPrice in salesPrices)
                            {
                                if (vendorPriceWorksheetMaint.APSetup.Current.RetentionType == AR.RetentionTypeList.FixedNumOfMonths)
                                {
                                    if (vendorPriceWorksheetMaint.Document.Current.OverwriteOverlapping == true)
                                    {
                                        if ((vendorPriceWorksheetMaint.Document.Current.ExpirationDate == null && salesPrice.EffectiveDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate) ||
                                            (vendorPriceWorksheetMaint.Document.Current.ExpirationDate != null && salesPrice.EffectiveDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.ExpirationDate))
                                        {
                                            vendorPriceWorksheetMaint.APVendorPrices.Delete(salesPrice);
                                        }
                                        else if (((salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null)
                                            || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate) || (salesPrice.EffectiveDate < vendorPriceWorksheetMaint.Document.Current.EffectiveDate && vendorPriceWorksheetMaint.Document.Current.EffectiveDate <= salesPrice.ExpirationDate))
                                            && vendorPriceWorksheetMaint.Document.Current.IsPromotional != true && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                                        }
                                    }
                                    else
                                    {
                                        if ((salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate) ||
                                            salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) insertNewPrice = false;
                                        if (salesPrice.EffectiveDate < vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            APVendorPrice newSalesPrice = (APVendorPrice)vendorPriceWorksheetMaint.APVendorPrices.Cache.CreateCopy(salesPrice);
                                            salesPrice.SalesPrice = priceLine.PendingPrice;
                                            salesPrice.EffectiveDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate;
                                            vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);

                                            newSalesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            newSalesPrice.RecordID = null;
                                            vendorPriceWorksheetMaint.APVendorPrices.Insert(newSalesPrice);
                                        }
                                        else if (salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);

                                            vendorPriceWorksheetMaint.APVendorPrices.Insert(CreateSalesPrice(priceLine, false, vendorPriceWorksheetMaint.Document.Current.EffectiveDate, null));
                                        }
                                        else if (salesPrice.EffectiveDate == vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == vendorPriceWorksheetMaint.Document.Current.ExpirationDate)
                                        {
                                            salesPrice.SalesPrice = priceLine.PendingPrice;
                                            vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                                        }
                                        else if ((salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate))
                                        {
                                            salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((salesPrice.EffectiveDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate) || (vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null && salesPrice.ExpirationDate < vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1)))
                                    {
                                        vendorPriceWorksheetMaint.APVendorPrices.Delete(salesPrice);
                                    }
                                    else if (((salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null) ||
                                        ((salesPrice.EffectiveDate < vendorPriceWorksheetMaint.Document.Current.EffectiveDate || salesPrice.EffectiveDate == null) && vendorPriceWorksheetMaint.Document.Current.EffectiveDate <= salesPrice.ExpirationDate)) && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                    {
                                        salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                        salesPrice.EffectiveDate = null;
                                        vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                                    }
                                }
                            }

                            if (insertNewPrice)
                            {
                                if (vendorPriceWorksheetMaint.Document.Current.OverwriteOverlapping == true || vendorPriceWorksheetMaint.APSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice)
                                {
                                    vendorPriceWorksheetMaint.APVendorPrices.Insert(CreateSalesPrice(priceLine, false, vendorPriceWorksheetMaint.Document.Current.EffectiveDate, vendorPriceWorksheetMaint.Document.Current.ExpirationDate));
                                }
                                else
                                {
                                    APVendorPrice minSalesPrice = PXSelect<APVendorPrice, Where<
                                        APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                                        And<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
                                        And<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
                                        And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,
                                        And<APVendorPrice.breakQty, Equal<Required<APVendorPrice.breakQty>>,
                                        And<APVendorPrice.effectiveDate, IsNotNull,
                                        And<Where<APVendorPrice.effectiveDate, GreaterEqual<Required<APVendorPrice.effectiveDate>>>>>>>>>>,
                                        OrderBy<Asc<APVendorPrice.effectiveDate>>>.SelectSingleBound(vendorPriceWorksheetMaint, new object[] { },
                                            priceLine.VendorID, priceLine.InventoryID, priceLine.UOM, priceLine.CuryID,
                                            priceLine.BreakQty, vendorPriceWorksheetMaint.Document.Current.EffectiveDate);

                                    vendorPriceWorksheetMaint.APVendorPrices.Insert(CreateSalesPrice(priceLine, false, vendorPriceWorksheetMaint.Document.Current.EffectiveDate, (minSalesPrice != null && minSalesPrice.EffectiveDate != null && minSalesPrice.EffectiveDate.Value != null) ? (DateTime?)minSalesPrice.EffectiveDate.Value.AddDays(-1) : vendorPriceWorksheetMaint.Document.Current.ExpirationDate));
                                }
                            }
                        }
                        else
                        {
                            vendorPriceWorksheetMaint.APVendorPrices.Insert(CreateSalesPrice(priceLine, false, vendorPriceWorksheetMaint.Document.Current.EffectiveDate, vendorPriceWorksheetMaint.Document.Current.ExpirationDate));
                        }
                    }
                    else
                    {
                        foreach (APVendorPrice salesPrice in salesPrices)
                        {
                            if (salesPrice.EffectiveDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate <= vendorPriceWorksheetMaint.Document.Current.ExpirationDate)
                            {
                                vendorPriceWorksheetMaint.APVendorPrices.Delete(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate <= vendorPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                            {
                                salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate >= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.EffectiveDate < vendorPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.ExpirationDate && vendorPriceWorksheetMaint.Document.Current.ExpirationDate.Value != null)
                            {
                                salesPrice.EffectiveDate = vendorPriceWorksheetMaint.Document.Current.ExpirationDate.Value.AddDays(1);
                                vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate <= vendorPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= vendorPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate > vendorPriceWorksheetMaint.Document.Current.EffectiveDate && vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                            {
                                salesPrice.ExpirationDate = vendorPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                vendorPriceWorksheetMaint.APVendorPrices.Update(salesPrice);
                            }
                        }
                        vendorPriceWorksheetMaint.APVendorPrices.Insert(CreateSalesPrice(priceLine, true, vendorPriceWorksheetMaint.Document.Current.EffectiveDate, vendorPriceWorksheetMaint.Document.Current.ExpirationDate));
                    }

                    if (vendorPriceWorksheetMaint.APSetup.Current.RetentionType == AR.RetentionTypeList.FixedNumOfMonths && vendorPriceWorksheetMaint.APSetup.Current.NumberOfMonths != 0)
                    {
                        foreach (APVendorPrice salesPrice in salesPrices)
                        {
                            if (salesPrice.ExpirationDate != null && ((DateTime)salesPrice.ExpirationDate).AddMonths(vendorPriceWorksheetMaint.APSetup.Current.NumberOfMonths ?? 0) < vendorPriceWorksheetMaint.Document.Current.EffectiveDate)
                            {
                                vendorPriceWorksheetMaint.APVendorPrices.Delete(salesPrice);
                            }
                        }
                    }
                }

                priceWorksheet.Status = AR.SPWorksheetStatus.Released;
                vendorPriceWorksheetMaint.Document.Update(priceWorksheet);
                vendorPriceWorksheetMaint.Document.Current.Status = AR.SPWorksheetStatus.Released;

                vendorPriceWorksheetMaint.Persist();

                ts.Complete();
            }
        }

        private static APVendorPrice CreateSalesPrice(APPriceWorksheetDetail priceLine, bool? isPromotional, DateTime? effectiveDate, DateTime? expirationDate)
        {
            APVendorPrice newSalesPrice = new APVendorPrice();
            newSalesPrice.VendorID = priceLine.VendorID;
            newSalesPrice.InventoryID = priceLine.InventoryID;
            newSalesPrice.UOM = priceLine.UOM;
            newSalesPrice.BreakQty = priceLine.BreakQty;
            newSalesPrice.SalesPrice = priceLine.PendingPrice;
            newSalesPrice.CuryID = priceLine.CuryID;
            newSalesPrice.IsPromotionalPrice = isPromotional;
            newSalesPrice.EffectiveDate = effectiveDate;
            newSalesPrice.ExpirationDate = expirationDate;
            return newSalesPrice;
        }

        #region SiteStatus Lookup
        [PXCopyPasteHiddenView]
        public PXFilter<AddItemFilter> addItemFilter;
        [PXCopyPasteHiddenView]
        public PXFilter<AddItemParameters> addItemParameters;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public ARAddItemLookup<ARAddItemSelected, AddItemFilter> addItemLookup;

        public PXAction<APPriceWorksheet> addItem;
        [PXUIField(DisplayName = AR.Messages.AddItem, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable AddItem(PXAdapter adapter)
        {
            if (addItemLookup.AskExt() == WebDialogResult.OK)
            {
                return AddSelItems(adapter);
            }
            return adapter.Get();
        }

        public PXAction<APPriceWorksheet> addSelItems;
        [PXUIField(DisplayName = AR.Messages.Add, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddSelItems(PXAdapter adapter)
        {
            if (addItemParameters.Current.VendorID != null)
            {
                foreach (ARAddItemSelected line in addItemLookup.Cache.Cached)
                {
                    if (line.Selected == true)
                    {
                        int? priceCode = addItemParameters.Current.VendorID;

                        PXResultset<APVendorPrice> salesPrices = PXSelectGroupBy<APVendorPrice, Where<
                            APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                            And<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
                            And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>>>>,
                                    Aggregate<GroupBy<APVendorPrice.vendorID,
                                    GroupBy<APVendorPrice.inventoryID,
                                    GroupBy<APVendorPrice.uOM,
                                    GroupBy<APVendorPrice.breakQty,
                                    GroupBy<APVendorPrice.curyID>>>>>>>.SelectMultiBound(this, null, priceCode, line.InventoryID, addItemParameters.Current.CuryID);
                        if (salesPrices.Count > 0)
                        {
                            foreach (APVendorPrice salesPrice in salesPrices)
                            {
                                APPriceWorksheetDetail newline = new APPriceWorksheetDetail();
                                newline.VendorID = addItemParameters.Current.VendorID;
                                newline.InventoryID = salesPrice.InventoryID;
                                newline.UOM = salesPrice.UOM;
                                newline.BreakQty = salesPrice.BreakQty;
                                newline.CurrentPrice = salesPrice.SalesPrice;
                                newline.CuryID = addItemParameters.Current.CuryID;
                                newline = PXCache<APPriceWorksheetDetail>.CreateCopy(Details.Update(newline));
                            }
                        }
                        else
                        {
                            APPriceWorksheetDetail newline = new APPriceWorksheetDetail();
                            newline.InventoryID = line.InventoryID;
                            newline.CuryID = addItemParameters.Current.CuryID;
                            newline.UOM = line.BaseUnit;
                            newline.VendorID = addItemParameters.Current.VendorID;
                            newline.CurrentPrice = 0m;
                            newline = PXCache<APPriceWorksheetDetail>.CreateCopy(Details.Update(newline));
                        }
                    }
                }
                addItemFilter.Cache.Clear();
                addItemLookup.Cache.Clear();
                addItemParameters.Cache.Clear(); 
            }
            else
            {
                if (addItemParameters.Current.VendorID == null)
                    addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.vendorID>(addItemParameters.Current, addItemParameters.Current.VendorID,
                        new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error, typeof(AddItemParameters.vendorID).Name));
            }
            return adapter.Get();
        }

        public override IEnumerable<PXDataRecord> ProviderSelect(BqlCommand command, int topCount, params PXDataValue[] pars)
        {
            return base.ProviderSelect(command, topCount, pars);
        }

		public override void Persist()
		{
			CheckForDuplicateDetails();

			base.Persist();
		}

		#region AddItemParameters event handlers
		protected virtual void AddItemParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            AddItemParameters det = (AddItemParameters)e.Row;
            if (det == null) return;
			PXUIFieldAttribute.SetVisible<AddItemParameters.curyID>(sender, det, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
        }

        protected virtual void AddItemParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            AddItemParameters parameters = (AddItemParameters)e.Row;
            if (parameters == null) return;

            if (!sender.ObjectsEqual<AddItemParameters.vendorID>(e.Row, e.OldRow))
            {
                PXResult<Vendor> vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.Select(this, parameters.VendorID);
                if (vendor != null)
                {
                    if (((Vendor)vendor).CuryID != null)
                        parameters.CuryID = ((Vendor)vendor).CuryID;
                    else
                        sender.SetDefaultExt<AddItemParameters.curyID>(e.Row);
                }
            }
        }
        #endregion

        #endregion

        public PXAction<APPriceWorksheet> copyPrices;
        [PXUIField(DisplayName = AR.Messages.CopyPrices, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable CopyPrices(PXAdapter adapter)
        {
            if (CopyPricesSettings.AskExt() == WebDialogResult.OK)
            {
                if (CopyPricesSettings.Current != null)
                {
                    if (CopyPricesSettings.Current.SourceVendorID == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.sourceVendorID>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.sourceVendorID).Name));
                        return adapter.Get();
                    }
                    if (CopyPricesSettings.Current.SourceCuryID == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.sourceCuryID>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.sourceCuryID).Name));
                        return adapter.Get();
                    }
                    if (CopyPricesSettings.Current.EffectiveDate == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.effectiveDate>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.effectiveDate).Name));
                        return adapter.Get();
                    }
                    if (CopyPricesSettings.Current.DestinationVendorID == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.destinationVendorID>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.destinationVendorID).Name));
                        return adapter.Get();
                    }
                    if (CopyPricesSettings.Current.DestinationCuryID == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.destinationCuryID>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.destinationCuryID).Name));
                        return adapter.Get();
                    }
                    if (CopyPricesSettings.Current.DestinationCuryID != CopyPricesSettings.Current.SourceCuryID && CopyPricesSettings.Current.RateTypeID == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.rateTypeID>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.rateTypeID).Name));
                        return adapter.Get();
                    }
                    PXLongOperation.StartOperation(this, delegate() { CopyPricesProc(Document.Current, CopyPricesSettings.Current); });
                }
            }
            return adapter.Get();
        }

        public static void CopyPricesProc(APPriceWorksheet priceWorksheet, CopyPricesFilter copyFilter)
        {
            APPriceWorksheetMaint vendorPriceWorksheetMaint = PXGraph.CreateInstance<APPriceWorksheetMaint>();
            vendorPriceWorksheetMaint.Document.Update((APPriceWorksheet)vendorPriceWorksheetMaint.Document.Cache.CreateCopy(priceWorksheet));
            vendorPriceWorksheetMaint.CopyPricesSettings.Current = copyFilter;
            int? sourcePriceCode = copyFilter.SourceVendorID;          
            
            PXResultset<APVendorPrice> salesPrices = PXSelectGroupBy<APVendorPrice, Where<
                    APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                    And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,
                    And<APVendorPrice.isPromotionalPrice, Equal<Required<APVendorPrice.isPromotionalPrice>>,
                    And<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>, And<APVendorPrice.expirationDate, IsNull>>,
                    Or<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>, And<APVendorPrice.expirationDate, Greater<Required<APVendorPrice.effectiveDate>>>>>>>>>>,
                        Aggregate<GroupBy<APVendorPrice.vendorID,
                        GroupBy<APVendorPrice.inventoryID,
                        GroupBy<APVendorPrice.uOM,
                        GroupBy<APVendorPrice.breakQty,
                        GroupBy<APVendorPrice.curyID>>>>>>,
                    OrderBy<Asc<APVendorPrice.effectiveDate, Asc<APVendorPrice.expirationDate>>>>.Select(vendorPriceWorksheetMaint,
                    sourcePriceCode, copyFilter.SourceCuryID,
                    copyFilter.IsPromotional ?? false, copyFilter.EffectiveDate, copyFilter.EffectiveDate, copyFilter.EffectiveDate);

            int? destinationPriceCode = copyFilter.DestinationVendorID;

            foreach (APVendorPrice salesPrice in salesPrices)
            {
                APPriceWorksheetDetail newline = new APPriceWorksheetDetail();
                newline.VendorID = destinationPriceCode;
                newline.InventoryID = salesPrice.InventoryID;
                newline.UOM = salesPrice.UOM;
                newline.BreakQty = salesPrice.BreakQty;
                
                decimal currentSalesPrice = salesPrice.SalesPrice ?? 0m;

                if (copyFilter.SourceCuryID != copyFilter.DestinationCuryID)
                {
                    currentSalesPrice = ConvertSalesPrice(vendorPriceWorksheetMaint, copyFilter.RateTypeID, copyFilter.SourceCuryID, copyFilter.DestinationCuryID, copyFilter.CurrencyDate, salesPrice.SalesPrice ?? 0m);
                }
                newline.CurrentPrice = currentSalesPrice;
                newline.CuryID = copyFilter.DestinationCuryID;
                vendorPriceWorksheetMaint.Details.Update(newline);
            }
            vendorPriceWorksheetMaint.Save.Press();
            vendorPriceWorksheetMaint.CopyPricesSettings.Cache.Clear();
            PXRedirectHelper.TryRedirect(vendorPriceWorksheetMaint, PXRedirectHelper.WindowMode.Same);
        }

        public static decimal ConvertSalesPrice(APPriceWorksheetMaint graph, string curyRateTypeID, string fromCuryID, string toCuryID, DateTime? curyEffectiveDate, decimal salesPrice)
        {
            decimal result = salesPrice;
            if (curyRateTypeID == null || curyRateTypeID == null || curyEffectiveDate == null)
                return result;
            CurrencyInfo info = new CurrencyInfo();
            info.BaseCuryID = fromCuryID;
            info.CuryID = toCuryID;
            info.CuryRateTypeID = curyRateTypeID;
            info = (CurrencyInfo)graph.CuryInfo.Cache.Update(info);
            info.SetCuryEffDate(graph.CuryInfo.Cache, curyEffectiveDate);
            graph.CuryInfo.Cache.Update(info);
            PXCurrencyAttribute.CuryConvCury(graph.CuryInfo.Cache, info, salesPrice, out result);
            graph.CuryInfo.Cache.Delete(info);
            return result;
        }

        public PXAction<APPriceWorksheet> calculatePrices;
        [PXUIField(DisplayName = AR.Messages.CalcPendingPrices, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable CalculatePrices(PXAdapter adapter)
        {
            if (CalculatePricesSettings.AskExt() == WebDialogResult.OK)
            {
                CalculatePendingPrices(CalculatePricesSettings.Current);
            }
            SelectTimeStamp();
            return adapter.Get();
        }

        protected readonly string viewPriceCode;

        private void CalculatePendingPrices(CalculatePricesFilter settings)
        {
            if (settings != null)
            {
                foreach (APPriceWorksheetDetail sp in Details.Select())
                {
                    bool skipUpdate = false;
                    decimal correctedAmt = 0;
                    decimal correctedAmtInBaseUnit = 0;
                    decimal? result;
                    var r = (PXResult<InventoryItem, INItemCost>)
                                    PXSelectJoin<InventoryItem,
                                        LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
                                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                                        .SelectWindowed(this, 0, 1, sp.InventoryID);
                    InventoryItem ii = r;
                    INItemCost ic = r;
                    switch (settings.PriceBasis)
                    {
                        case AR.PriceBasisTypes.LastCost:
                            skipUpdate = settings.UpdateOnZero != true && (ic.LastCost == null || ic.LastCost == 0);
                            if (!skipUpdate)
                            {
                                correctedAmtInBaseUnit = (ic.LastCost ?? 0);

                                if (ii.BaseUnit != sp.UOM)
                                {
                                    if (TryConvertToBase(Caches[typeof(InventoryItem)], ii.InventoryID, sp.UOM, correctedAmtInBaseUnit, out result))
                                    {
                                        correctedAmt = result.Value;
                                    }
                                }
                                else
                                {
                                    correctedAmt = correctedAmtInBaseUnit;
                                }
                            }

                            break;
                        case AR.PriceBasisTypes.StdCost:
                            if (ii.ValMethod != INValMethod.Standard)
                            {
                                skipUpdate = settings.UpdateOnZero != true && (ic.AvgCost == null || ic.AvgCost == 0);
                                correctedAmtInBaseUnit = (ic.AvgCost ?? 0);
                            }
                            else
                            {
                                skipUpdate = settings.UpdateOnZero != true && (ii.StdCost == null || ii.StdCost == 0);
                                correctedAmtInBaseUnit = (ii.StdCost ?? 0);
                            }

                            if (ii.BaseUnit != sp.UOM)
                            {
                                if (TryConvertToBase(Caches[typeof(InventoryItem)], ii.InventoryID, sp.UOM, correctedAmtInBaseUnit, out result))
                                {
                                    correctedAmt = result.Value;
                                }
                            }
                            else
                            {
                                correctedAmt = correctedAmtInBaseUnit;
                            }

                            break;
                        case AR.PriceBasisTypes.PendingPrice:
                            skipUpdate = settings.UpdateOnZero != true && (sp.PendingPrice == null || sp.PendingPrice == 0);
                            correctedAmt = sp.PendingPrice ?? 0m;
                            break;
                        case AR.PriceBasisTypes.CurrentPrice:
                            skipUpdate = settings.UpdateOnZero != true && (sp.CurrentPrice == null || sp.CurrentPrice == 0);
                            correctedAmt = sp.CurrentPrice ?? 0;
                            break;
                        case AR.PriceBasisTypes.RecommendedPrice:
                            skipUpdate = settings.UpdateOnZero != true && (ii.RecPrice == null || ii.RecPrice == 0);
                            correctedAmt = ii.RecPrice ?? 0;
                            break;
                    }

                    if (!skipUpdate)
                    {
                        if (settings.CorrectionPercent != null)
                        {
                            correctedAmt = correctedAmt * (settings.CorrectionPercent.Value * 0.01m);
                        }

                        if (settings.Rounding != null)
                        {
                            correctedAmt = Math.Round(correctedAmt, settings.Rounding.Value, MidpointRounding.AwayFromZero);
                        }

                        APPriceWorksheetDetail u = (APPriceWorksheetDetail)Details.Cache.CreateCopy(sp);
                        u.PendingPrice = correctedAmt;
                        Details.Update(u);
                    }
                }

            }
        }

		private void CheckForDuplicateDetails()
		{
			IEnumerable<APPriceWorksheetDetail> worksheetDetails = PXSelect<
				APPriceWorksheetDetail,
				Where<
					APPriceWorksheetDetail.refNbr, Equal<Current<APPriceWorksheetDetail.refNbr>>>>
				.Select(this)
				.RowCast<APPriceWorksheetDetail>()
				.ToArray();

			IEqualityComparer<APPriceWorksheetDetail> duplicateComparer =
				new FieldSubsetEqualityComparer<APPriceWorksheetDetail>(
					Details.Cache,
					typeof(APPriceWorksheetDetail.refNbr),
					typeof(APPriceWorksheetDetail.vendorID),
					typeof(APPriceWorksheetDetail.inventoryID),
					typeof(APPriceWorksheetDetail.subItemID),
					typeof(APPriceWorksheetDetail.uOM),
					typeof(APPriceWorksheetDetail.curyID),
					typeof(APPriceWorksheetDetail.breakQty));

			IEnumerable<APPriceWorksheetDetail> duplicates = worksheetDetails
				.GroupBy(detail => detail, duplicateComparer)
				.Where(duplicatesGroup => duplicatesGroup.HasAtLeastTwoItems())
				.Flatten();

			foreach (APPriceWorksheetDetail duplicate in duplicates)
			{
				Details.Cache.RaiseExceptionHandling<APPriceWorksheetDetail.vendorID>(
					duplicate,
					duplicate.VendorID,
					new PXSetPropertyException(
						Messages.DuplicateVendorPrice,
						PXErrorLevel.RowError,
						typeof(APPriceWorksheetDetail.vendorID).Name));
			}
		}

		private decimal GetItemPrice(PXGraph graph, int? vendorID, int? inventoryID, string toCuryID, DateTime? curyEffectiveDate)
        {
            InventoryItem inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);
            if (inventoryItem != null && inventoryItem.BasePrice != null)
            {
                return ConvertSalesPrice(this, new CMSetupSelect(this).Current.APRateTypeDflt, new PXSetup<GL.Company>(this).Current.BaseCuryID, toCuryID, curyEffectiveDate, inventoryItem.BasePrice ?? 0m);
            }
            return 0m;
        }

        private bool TryConvertToBase(PXCache cache, int? inventoryID, string uom, decimal value, out decimal? result)
        {
            result = null;

            try
            {
                result = INUnitAttribute.ConvertToBase(cache, inventoryID, uom, value, INPrecision.UNITCOST);
                return true;
            }
            catch (PXUnitConversionException)
            {
                return false;
            }
        }
        #endregion

        #region APPriceWorksheet event handlers

        protected virtual void APPriceWorksheet_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            APPriceWorksheet tran = (APPriceWorksheet)e.Row;
            if (tran == null) return;

            bool allowEdit = (tran.Status == AR.SPWorksheetStatus.Hold || tran.Status == AR.SPWorksheetStatus.Open);
            ReleasePriceWorksheet.SetEnabled(tran.Hold == false && tran.Status == AR.SPWorksheetStatus.Open);
            addItem.SetEnabled(tran.Hold == true && allowEdit);
            copyPrices.SetEnabled(tran.Hold == true && allowEdit);
            calculatePrices.SetEnabled(tran.Hold == true && allowEdit);

            Details.Cache.AllowInsert = allowEdit;
            Details.Cache.AllowDelete = allowEdit;
            Details.Cache.AllowUpdate = allowEdit;

            Document.Cache.AllowDelete = tran.Status != AR.SPWorksheetStatus.Released;

            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.hold>(sender, tran, tran.Status != AR.SPWorksheetStatus.Released);
            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.descr>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.effectiveDate>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.expirationDate>(sender, tran, allowEdit && (APSetup.Current.RetentionType != AR.RetentionTypeList.LastPrice || (APSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice && tran.IsPromotional == true)));
            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.isPromotional>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<APPriceWorksheet.overwriteOverlapping>(sender, tran, allowEdit && tran.IsPromotional != true && APSetup.Current.RetentionType != AR.RetentionTypeList.LastPrice);

            if (APSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice || tran.IsPromotional == true) tran.OverwriteOverlapping = true;
            if (APSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice && tran.IsPromotional != true) tran.ExpirationDate = null;
        }

        protected virtual void APPriceWorksheet_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            APPriceWorksheet doc = (APPriceWorksheet)e.Row;
            if (doc == null) return;

            doc.Status = doc.Hold == false ? AR.SPWorksheetStatus.Open : AR.SPWorksheetStatus.Hold;
        }

        protected virtual void APPriceWorksheet_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            APPriceWorksheet doc = (APPriceWorksheet)e.Row;
            if (doc == null) return;

            if (doc.IsPromotional == true && doc.ExpirationDate == null)
                sender.RaiseExceptionHandling<APPriceWorksheet.expirationDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPriceWorksheet.expirationDate).Name));
        }

        protected virtual void APPriceWorksheet_EffectiveDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            APPriceWorksheet doc = (APPriceWorksheet)e.Row;
            if (doc == null) return;

            if (e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPriceWorksheet.effectiveDate).Name);

            if (doc.IsPromotional == true && doc.ExpirationDate != null && doc.ExpirationDate < (DateTime)e.NewValue)
            {
                throw new PXSetPropertyException(PXMessages.LocalizeFormat(AR.Messages.ExpirationLessThanEffective));
            }
        }

        protected virtual void APPriceWorksheet_ExpirationDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            APPriceWorksheet doc = (APPriceWorksheet)e.Row;
            if (doc == null) return;

            if (doc.IsPromotional == true && e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPriceWorksheet.expirationDate).Name);

            if (doc.IsPromotional == true && doc.EffectiveDate != null && doc.EffectiveDate > (DateTime)e.NewValue)
            {
                throw new PXSetPropertyException(PXMessages.LocalizeFormat(AR.Messages.ExpirationLessThanEffective));
            }
        }

        #endregion

        #region APPriceWorksheetDetail event handlers

        protected virtual void APPriceWorksheetDetail_BreakQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            //add verification
            APPriceWorksheetDetail det = (APPriceWorksheetDetail)e.Row;
            if (det == null) return;
        }

        protected virtual void APPriceWorksheetDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            APPriceWorksheetDetail det = (APPriceWorksheetDetail)e.Row;
            if (det == null) return;

            if (e.ExternalCall && det.VendorID != null && det.InventoryID != null && det.CuryID != null && Document.Current != null && Document.Current.EffectiveDate != null && det.CurrentPrice == 0m)
                det.CurrentPrice = GetItemPrice(this, det.VendorID, det.InventoryID, det.CuryID, Document.Current.EffectiveDate);
        }

        protected virtual void APPriceWorksheetDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            APPriceWorksheetDetail det = (APPriceWorksheetDetail)e.Row;
            if (det == null) return;

            if (e.ExternalCall && (!sender.ObjectsEqual<APPriceWorksheetDetail.uOM>(e.Row, e.OldRow) || !sender.ObjectsEqual<APPriceWorksheetDetail.inventoryID>(e.Row, e.OldRow)
                || !sender.ObjectsEqual<APPriceWorksheetDetail.curyID>(e.Row, e.OldRow)))
            {
                det.CurrentPrice = 0m;
            }
        }

        protected virtual void APPriceWorksheetDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            APPriceWorksheetDetail det = (APPriceWorksheetDetail)e.Row;
            if (det == null) return;

			if (Document.Current?.Hold != true && det.PendingPrice == null)
			{
				sender.RaiseExceptionHandling<APPriceWorksheetDetail.pendingPrice>(
					det, 
					det.PendingPrice,
					new PXSetPropertyException(
						ErrorMessages.FieldIsEmpty, 
						PXErrorLevel.Error, 
						typeof(APPriceWorksheetDetail.pendingPrice).Name));

				return;
			}

			if (det.VendorID == null)
            {
                sender.RaiseExceptionHandling<APPriceWorksheetDetail.vendorID>(det, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPriceWorksheetDetail.vendorID).Name));
                return;
            }

            if (Document.Current != null && Document.Current.Hold != true && det.PendingPrice == null)
                sender.RaiseExceptionHandling<APPriceWorksheetDetail.pendingPrice>(det, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPriceWorksheetDetail.pendingPrice).Name));
        }
        #endregion

        #region CopyPricesFilter event handlers

        protected virtual void CopyPricesFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CopyPricesFilter filter = (CopyPricesFilter)e.Row;
            if (filter == null) return;

            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.rateTypeID>(sender, filter, filter.SourceCuryID != filter.DestinationCuryID);
            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.currencyDate>(sender, filter, filter.SourceCuryID != filter.DestinationCuryID);

	        bool MCFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
            PXUIFieldAttribute.SetVisible<CopyPricesFilter.sourceCuryID>(sender, filter, MCFeatureInstalled);
            PXUIFieldAttribute.SetVisible<CopyPricesFilter.destinationCuryID>(sender, filter, MCFeatureInstalled);
            PXUIFieldAttribute.SetVisible<CopyPricesFilter.currencyDate>(sender, filter, MCFeatureInstalled);
            PXUIFieldAttribute.SetVisible<CopyPricesFilter.rateTypeID>(sender, filter, MCFeatureInstalled);
        }

        protected virtual void CopyPricesFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            CopyPricesFilter parameters = (CopyPricesFilter)e.Row;
            if (parameters == null) return;

            if (!sender.ObjectsEqual<CopyPricesFilter.sourceVendorID>(e.Row, e.OldRow))
            {
                PXResult<Vendor> vendor = PXSelect<Vendor, Where<Vendor.acctCD, Equal<Required<Vendor.acctCD>>>>.Select(this, parameters.SourceVendorID);
                if (vendor != null)
                {
                    parameters.SourceCuryID = ((Vendor)vendor).CuryID;
                }
            }
            if (!sender.ObjectsEqual<CopyPricesFilter.destinationVendorID>(e.Row, e.OldRow))
            {
                PXResult<Vendor> vendor = PXSelect<Vendor, Where<Vendor.acctCD, Equal<Required<Vendor.acctCD>>>>.Select(this, parameters.DestinationVendorID);
                if (vendor != null)
                {
                    parameters.DestinationCuryID = ((Vendor)vendor).CuryID;
                }
            }
        }

        #endregion
    }

    [Serializable]
    public partial class AddItemFilter : INSiteStatusFilter
    {
        #region Inventory
        public new abstract class inventory : PX.Data.IBqlField
        {
        }
        #endregion
        #region PriceClassID
        public abstract class priceClassID : PX.Data.IBqlField
        {
        }
        protected String _PriceClassID;
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Price Class ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<INPriceClass.priceClassID>), DescriptionField = typeof(INItemClass.descr))]
        public virtual String PriceClassID
        {
            get
            {
                return this._PriceClassID;
            }
            set
            {
                this._PriceClassID = value;
            }
        }
        #endregion
        #region CurrentOwnerID
        public abstract class currentOwnerID : PX.Data.IBqlField
        {
        }

        [PXDBGuid]
        [CR.CRCurrentOwnerID]
        public virtual Guid? CurrentOwnerID { get; set; }
        #endregion
        #region MyOwner
        public abstract class myOwner : PX.Data.IBqlField
        {
        }
        protected Boolean? _MyOwner;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Me")]
        public virtual Boolean? MyOwner
        {
            get
            {
                return _MyOwner;
            }
            set
            {
                _MyOwner = value;
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : PX.Data.IBqlField
        {
        }
        protected Guid? _OwnerID;
        [PXDBGuid]
        [PXUIField(DisplayName = "Product Manager")]
        [PX.TM.PXSubordinateOwnerSelector]
        public virtual Guid? OwnerID
        {
            get
            {
                return (_MyOwner == true) ? CurrentOwnerID : _OwnerID;
            }
            set
            {
                _OwnerID = value;
            }
        }
        #endregion
        #region WorkGroupID
        public abstract class workGroupID : PX.Data.IBqlField
        {
        }
        protected Int32? _WorkGroupID;
        [PXDBInt]
        [PXUIField(DisplayName = "Product  Workgroup")]
        [PXSelector(typeof(Search<TM.EPCompanyTree.workGroupID,
            Where<TM.EPCompanyTree.workGroupID, TM.Owned<Current<AccessInfo.userID>>>>),
         SubstituteKey = typeof(TM.EPCompanyTree.description))]
        public virtual Int32? WorkGroupID
        {
            get
            {
                return (_MyWorkGroup == true) ? null : _WorkGroupID;
            }
            set
            {
                _WorkGroupID = value;
            }
        }
        #endregion
        #region MyWorkGroup
        public abstract class myWorkGroup : PX.Data.IBqlField
        {
        }
        protected Boolean? _MyWorkGroup;
        [PXDefault(false)]
        [PXDBBool]
        [PXUIField(DisplayName = "My", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? MyWorkGroup
        {
            get
            {
                return _MyWorkGroup;
            }
            set
            {
                _MyWorkGroup = value;
            }
        }
        #endregion
    }

    [Serializable]
    public partial class AddItemParameters : IBqlTable
    {
        #region VendorID
        public abstract class vendorID : PX.Data.IBqlField
        {
        }
        protected Int32? _VendorID;
        [Vendor]
        [PXDefault]
        public virtual Int32? VendorID
        {
            get
            {
                return this._VendorID;
            }
            set
            {
                this._VendorID = value;
            }
        }
        #endregion
        #region CuryID
        public abstract class curyID : PX.Data.IBqlField
        {
        }
        protected string _CuryID;
        [PXString(5)]
        [PXDefault(typeof(Search<GL.Company.baseCuryID>))]
        [PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
        [PXUIField(DisplayName = "Currency")]
        public virtual string CuryID
        {
            get
            {
                return this._CuryID;
            }
            set
            {
                this._CuryID = value;
            }
        }
        #endregion
    }

    [System.SerializableAttribute()]
    [PXProjection(typeof(Select2<InventoryItem,
        LeftJoin<INItemClass,
                        On<INItemClass.itemClassID, Equal<InventoryItem.itemClassID>>,
        LeftJoin<INPriceClass,
                        On<INPriceClass.priceClassID, Equal<InventoryItem.priceClassID>>,
        LeftJoin<INUnit,
                    On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
                 And<INUnit.fromUnit, Equal<InventoryItem.salesUnit>,
                 And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>
                            >>>>,
        Where<CurrentMatch<InventoryItem, AccessInfo.userName>>>), Persistent = false)]
    public partial class ARAddItemSelected : IBqlTable
    {
        #region Selected
        public abstract class selected : PX.Data.IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }
        #endregion

        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [Inventory(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
        [PXDefault()]
        public virtual Int32? InventoryID
        {
            get
            {
                return this._InventoryID;
            }
            set
            {
                this._InventoryID = value;
            }
        }
        #endregion

        #region InventoryCD
        public abstract class inventoryCD : PX.Data.IBqlField
        {
        }
        protected string _InventoryCD;
        [PXDefault()]
        [InventoryRaw(BqlField = typeof(InventoryItem.inventoryCD))]
        public virtual String InventoryCD
        {
            get
            {
                return this._InventoryCD;
            }
            set
            {
                this._InventoryCD = value;
            }
        }
        #endregion

        #region Descr
        public abstract class descr : PX.Data.IBqlField
        {
        }

        protected string _Descr;
        [PXDBString(60, IsUnicode = true, BqlField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String Descr
        {
            get
            {
                return this._Descr;
            }
            set
            {
                this._Descr = value;
            }
        }
        #endregion

        #region ItemClassID
        public abstract class itemClassID : PX.Data.IBqlField
        {
        }
        protected string _ItemClassID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.itemClassID))]
        [PXUIField(DisplayName = "Item Class ID", Visible = true)]
        public virtual String ItemClassID
        {
            get
            {
                return this._ItemClassID;
            }
            set
            {
                this._ItemClassID = value;
            }
        }
        #endregion

        #region ItemClassDescription
        public abstract class itemClassDescription : PX.Data.IBqlField
        {
        }
        protected String _ItemClassDescription;
        [PXDBString(250, IsUnicode = true, BqlField = typeof(INItemClass.descr))]
        [PXUIField(DisplayName = "Item Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
        public virtual String ItemClassDescription
        {
            get
            {
                return this._ItemClassDescription;
            }
            set
            {
                this._ItemClassDescription = value;
            }
        }
        #endregion

        #region PriceClassID
        public abstract class priceClassID : PX.Data.IBqlField
        {
        }

        protected string _PriceClassID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.priceClassID))]
        [PXUIField(DisplayName = "Price Class ID", Visible = true)]
        public virtual String PriceClassID
        {
            get
            {
                return this._PriceClassID;
            }
            set
            {
                this._PriceClassID = value;
            }
        }
        #endregion

        #region PriceClassDescription
        public abstract class priceClassDescription : PX.Data.IBqlField
        {
        }
        protected String _PriceClassDescription;
        [PXDBString(250, IsUnicode = true, BqlField = typeof(INPriceClass.description))]
        [PXUIField(DisplayName = "Price Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
        public virtual String PriceClassDescription
        {
            get
            {
                return this._PriceClassDescription;
            }
            set
            {
                this._PriceClassDescription = value;
            }
        }
        #endregion

        #region BaseUnit
        public abstract class baseUnit : PX.Data.IBqlField
        {
        }

        protected string _BaseUnit;
        [INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible, BqlField = typeof(InventoryItem.baseUnit))]
        public virtual String BaseUnit
        {
            get
            {
                return this._BaseUnit;
            }
            set
            {
                this._BaseUnit = value;
            }
        }
        #endregion

        #region CuryID
        public abstract class curyID : PX.Data.IBqlField
        {
        }
        protected String _CuryID;
        [PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
        [PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String CuryID
        {
            get
            {
                return this._CuryID;
            }
            set
            {
                this._CuryID = value;
            }
        }
        #endregion

        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        protected Int64? _CuryInfoID;
        [PXLong()]
        [CM.CurrencyInfo()]
        public virtual Int64? CuryInfoID
        {
            get
            {
                return this._CuryInfoID;
            }
            set
            {
                this._CuryInfoID = value;
            }
        }
        #endregion

        #region CuryUnitPrice
        public abstract class curyUnitPrice : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryUnitPrice;
        //[PXCalcCurrency(typeof(SOSiteStatusSelected.curyInfoID), typeof(SOSiteStatusSelected.baseUnitPrice))]
        [PXUIField(DisplayName = "Last Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryUnitPrice
        {
            get
            {
                return this._CuryUnitPrice;
            }
            set
            {
                this._CuryUnitPrice = value;
            }
        }
        #endregion
        #region PriceWorkgroupID
        public abstract class priceWorkgroupID : PX.Data.IBqlField
        {
        }
        protected Int32? _PriceWorkgroupID;
        [PXDBInt(BqlField = typeof(InventoryItem.priceWorkgroupID))]
        [EP.PXWorkgroupSelector]
        [PXUIField(DisplayName = "Price Workgroup")]
        public virtual Int32? PriceWorkgroupID
        {
            get
            {
                return this._PriceWorkgroupID;
            }
            set
            {
                this._PriceWorkgroupID = value;
            }
        }
        #endregion

        #region PriceManagerID
        public abstract class priceManagerID : PX.Data.IBqlField
        {
        }
        protected Guid? _PriceManagerID;
        [PXDBGuid(BqlField = typeof(InventoryItem.priceManagerID))]
        [PX.TM.PXCompanyTreeSelector]
        //[PX.TM.PXOwnerSelector(typeof(POReceipt.workgroupID))]
        [TM.PXOwnerSelector(typeof(InventoryItem.priceWorkgroupID))]
        [PXUIField(DisplayName = "Price Manager")]
        public virtual Guid? PriceManagerID
        {
            get
            {
                return this._PriceManagerID;
            }
            set
            {
                this._PriceManagerID = value;
            }
        }
        #endregion
    }

    public class ARAddItemLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
        where Status : class, IBqlTable, new()
        where StatusFilter : AddItemFilter, new()
    {
        #region Ctor
        public ARAddItemLookup(PXGraph graph)
            : base(graph)
        {
            graph.RowSelecting.AddHandler(typeof(ARAddItemSelected), OnRowSelecting);
        }

        public ARAddItemLookup(PXGraph graph, Delegate handler)
            : base(graph, handler)
        {
            graph.RowSelecting.AddHandler(typeof(ARAddItemSelected), OnRowSelecting);
        }
        #endregion
        protected virtual void OnRowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
            //remove
        }

        protected override void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            base.OnFilterSelected(sender, e);
            AddItemFilter filter = (AddItemFilter)e.Row;
            PXCache status = sender.Graph.Caches[typeof(ARAddItemSelected)];
            PXUIFieldAttribute.SetVisible<ARAddItemSelected.curyID>(status, null, true);
        }
    }
    [Serializable]
    public partial class CalculatePricesFilter : IBqlTable
    {
        #region CorrectionPercent
        public abstract class correctionPercent : PX.Data.IBqlField
        {
        }

        protected Decimal? _CorrectionPercent;

        [PXDefault("100.00")]
        [PXDecimal(6, MinValue = 0, MaxValue = 1000)]
        [PXUIField(DisplayName = "% of Original Price", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? CorrectionPercent
        {
            get
            {
                return this._CorrectionPercent;
            }
            set
            {
                this._CorrectionPercent = value;
            }
        }
        #endregion

        #region Rounding
        public abstract class rounding : PX.Data.IBqlField
        {
        }

        protected Int16? _Rounding;
		[PXDefault((short)2, typeof(Search<CommonSetup.decPlPrcCst>))]
        [PXDBShort(MinValue = 0, MaxValue = 6)]
        [PXUIField(DisplayName = "Decimal Places", Visibility = PXUIVisibility.Visible)]
        public virtual Int16? Rounding
        {
            get
            {
                return this._Rounding;
            }
            set
            {
                this._Rounding = value;
            }
        }
        #endregion

        #region PriceBasis
        public abstract class priceBasis : PX.Data.IBqlField
        {
        }
        protected String _PriceBasis;
        [PXString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Price Basis")]
        [PriceBasisTypes.List()]
        [PXDefault(PriceBasisTypes.CurrentPrice)]
        public virtual String PriceBasis
        {
            get
            {
                return this._PriceBasis;
            }
            set
            {
                this._PriceBasis = value;
            }
        }
        #endregion

        #region UpdateOnZero
        public abstract class updateOnZero : IBqlField
        {
        }
        protected bool? _UpdateOnZero;
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Update with Zero Price when Basis is Zero", Visibility = PXUIVisibility.Service)]
        public virtual bool? UpdateOnZero
        {
            get
            {
                return _UpdateOnZero;
            }
            set
            {
                _UpdateOnZero = value;
            }
        }
        #endregion
    }

    public static class PriceBasisTypes
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { LastCost, StdCost, CurrentPrice, PendingPrice, RecommendedPrice },
                new string[] { Messages.LastCost, Messages.StdCost, Messages.CurrentPrice, Messages.PendingPrice, Messages.RecommendedPrice }) { ; }
        }
        public const string LastCost = "L";
        public const string StdCost = "S";
        public const string CurrentPrice = "P";
        public const string PendingPrice = "N";
        public const string RecommendedPrice = "R";
    }

    [Serializable]
    public partial class CopyPricesFilter : IBqlTable
    {
        #region SourceVendorID
        public abstract class sourceVendorID : PX.Data.IBqlField
        {
        }
        protected Int32? _SourceVendorID;
        [Vendor(DisplayName="Source Vendor")]
        [PXDefault]
        public virtual Int32? SourceVendorID
        {
            get
            {
                return this._SourceVendorID;
            }
            set
            {
                this._SourceVendorID = value;
            }
        }
        #endregion
        #region SourceCuryID
        public abstract class sourceCuryID : PX.Data.IBqlField
        {
        }
        protected string _SourceCuryID;
        [PXString(5)]
        [PXDefault(typeof(Search<GL.Company.baseCuryID>))]
        [PXSelector(typeof(CM.Currency.curyID))]
        [PXUIField(DisplayName = "Source Currency", Required = true)]
        public virtual string SourceCuryID
        {
            get
            {
                return this._SourceCuryID;
            }
            set
            {
                this._SourceCuryID = value;
            }
        }
        #endregion
        #region EffectiveDate
        public abstract class effectiveDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _EffectiveDate;
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Effective As Of", Required = true)]
        public virtual DateTime? EffectiveDate
        {
            get
            {
                return this._EffectiveDate;
            }
            set
            {
                this._EffectiveDate = value;
            }
        }
        #endregion
        #region IsPromotional
        public abstract class isPromotional : IBqlField
        {
        }
        protected bool? _IsPromotional;
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Promotional Price")]
        public virtual bool? IsPromotional
        {
            get
            {
                return _IsPromotional;
            }
            set
            {
                _IsPromotional = value;
            }
        }
        #endregion

        #region DestinationVendorID
        public abstract class destinationVendorID : PX.Data.IBqlField
        {
        }
        protected Int32? _DestinationVendorID;
        [Vendor(DisplayName = "Destination Vendor")]
        [PXDefault]
        public virtual Int32? DestinationVendorID
        {
            get
            {
                return this._DestinationVendorID;
            }
            set
            {
                this._DestinationVendorID = value;
            }
        }
        #endregion
        #region DestinationCuryID
        public abstract class destinationCuryID : PX.Data.IBqlField
        {
        }
        protected string _DestinationCuryID;
        [PXString(5)]
        [PXDefault(typeof(Search<GL.Company.baseCuryID>))]
        [PXSelector(typeof(CM.Currency.curyID))]
        [PXUIField(DisplayName = "Destination Currency", Required = true)]
        public virtual string DestinationCuryID
        {
            get
            {
                return this._DestinationCuryID;
            }
            set
            {
                this._DestinationCuryID = value;
            }
        }
        #endregion

        #region RateTypeID
        public abstract class rateTypeID : PX.Data.IBqlField
        {
        }
        protected String _RateTypeID;
        [PXString(6)]
        [PXDefault()]
        [PXSelector(typeof(PX.Objects.CM.CurrencyRateType.curyRateTypeID))]
        [PXUIField(DisplayName = "Rate Type")]
        public virtual String RateTypeID
        {
            get
            {
                return this._RateTypeID;
            }
            set
            {
                this._RateTypeID = value;
            }
        }
        #endregion
        #region CurrencyDate
        public abstract class currencyDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _CurrencyDate;
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Currency Effective Date")]
        public virtual DateTime? CurrencyDate
        {
            get
            {
                return this._CurrencyDate;
            }
            set
            {
                this._CurrencyDate = value;
            }
        }
        #endregion
        #region CustomRate
        public abstract class customRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _CustomRate;
        [PXDefault("1.00")]
        [PXDecimal(6, MinValue = 0)]
        [PXUIField(DisplayName = "Currency Rate", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? CustomRate
        {
            get
            {
                return this._CustomRate;
            }
            set
            {
                this._CustomRate = value;
            }
        }
        #endregion
    }
}
