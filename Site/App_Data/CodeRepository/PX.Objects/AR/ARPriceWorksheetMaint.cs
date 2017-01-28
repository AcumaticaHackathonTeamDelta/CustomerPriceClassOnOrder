using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.Common.Extensions;

using PX.Objects.AR.Repositories;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.CR;

namespace PX.Objects.AR
{
    [Serializable]
    public class ARPriceWorksheetMaint : PXGraph<ARPriceWorksheetMaint, ARPriceWorksheet>
    {
        #region Selects/Views

        public PXSelect<ARPriceWorksheet> Document;
        [PXImport(typeof(ARPriceWorksheet))]
        public PXSelectJoin<ARPriceWorksheetDetail,
                LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARPriceWorksheetDetail.inventoryID>>>, Where<ARPriceWorksheetDetail.refNbr, Equal<Current<ARPriceWorksheet.refNbr>>>,
                OrderBy<Asc<ARPriceWorksheetDetail.priceType, Asc<ARPriceWorksheetDetail.priceCode, Asc<InventoryItem.inventoryCD, Asc<ARPriceWorksheetDetail.breakQty>>>>>> Details;
        public PXSetup<ARSetup> ARSetup;
        public PXSelect<ARSalesPrice> ARSalesPrices;
		public PXSelect<BAccount,
					Where<BAccount.type, Equal<BAccountType.customerType>,
						Or<BAccount.type, Equal<BAccountType.combinedType>>>> CustomerCode;
        public PXSelect<Customer> Customer;
        public PXSelect<ARPriceClass> CustPriceClassCode;

        [PXCopyPasteHiddenView]
        public PXFilter<CopyPricesFilter> CopyPricesSettings;
        [PXCopyPasteHiddenView]
        public PXFilter<CalculatePricesFilter> CalculatePricesSettings;
        public PXSelect<CurrencyInfo> CuryInfo;

		protected readonly CustomerRepository CustomerRepository;

        #endregion

        public ARPriceWorksheetMaint()
        {
            ARSetup setup = ARSetup.Current;
			CustomerRepository = new CustomerRepository(this);
        }

        public string GetPriceType(string viewname)
        {
            string priceType = PriceTypeList.Customer;
            if (viewname.Contains(typeof(ARPriceWorksheetDetail).Name) && Details.Current != null)
                priceType = Details.Current.PriceType;
            if (viewname.Contains(typeof(AddItemParameters).Name) && addItemParameters.Current != null)
                priceType = addItemParameters.Current.PriceType;
            if (viewname.Contains(typeof(CopyPricesFilter).Name) && CopyPricesSettings.Current != null)
                if (viewname.Contains(typeof(CopyPricesFilter.sourcePriceCode).Name.First().ToString().ToUpper()
                    + String.Join(String.Empty, typeof(CopyPricesFilter.sourcePriceCode).Name.Skip(1))))
                    priceType = CopyPricesSettings.Current.SourcePriceType;
                else
                    priceType = CopyPricesSettings.Current.DestinationPriceType;
            return priceType;
        }

        #region Overrides
		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
		{
			if ((viewName.Contains(typeof(ARPriceWorksheetDetail.priceCode).Name) || viewName.Contains("PriceCode")) && this.ViewNames.ContainsKey(CustomerCode.View) && this.ViewNames.ContainsKey(CustPriceClassCode.View))
			{
				if (GetPriceType(viewName) == PriceTypeList.Customer)
				{
					viewName = this.ViewNames[CustomerCode.View];
					sortcolumns = new string[] { typeof(CR.BAccount.acctCD).Name };
					ModifyFilters(filters, typeof(ARPriceWorksheetDetail.priceCode).Name, typeof(CR.BAccount.acctCD).Name);
					ModifyFilters(filters, typeof(ARPriceWorksheetDetail.description).Name, typeof(CR.BAccount.acctName).Name);
				}
				else
				{
					viewName = this.ViewNames[CustPriceClassCode.View];
					sortcolumns = new string[] { typeof(ARPriceClass.priceClassID).Name };
					ModifyFilters(filters, typeof(ARPriceWorksheetDetail.priceCode).Name, typeof(ARPriceClass.priceClassID).Name);
					ModifyFilters(filters, typeof(ARPriceWorksheetDetail.description).Name, typeof(ARPriceClass.description).Name);
				}
			}
			IEnumerable ret = base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
			return ret;
		}

		public override Type GetItemType(string viewName)
		{
			if ((viewName.Contains(typeof(ARPriceWorksheetDetail.priceCode).Name) || viewName.Contains("PriceCode")) && this.ViewNames.ContainsKey(CustomerCode.View) && this.ViewNames.ContainsKey(CustPriceClassCode.View))
			{
				if (GetPriceType(viewName) == PriceTypeList.Customer)
				{
					viewName = this.ViewNames[CustomerCode.View];
				}
				else
				{
					viewName = this.ViewNames[CustPriceClassCode.View];
				}
			}
			return base.GetItemType(viewName);
		}

		private PXFilterRow[] ModifyFilters(PXFilterRow[] filters, string originalFieldName, string newFieldName)
		{
			if (filters != null)
				foreach (PXFilterRow filter in filters)
				{
					if (string.Compare(filter.DataField, originalFieldName, true) == 0)
						filter.DataField = newFieldName;
				}
			return filters;
		}

        public override object GetValueExt(string viewName, object data, string fieldName)
        {
            if (viewName.Contains(typeof(ARPriceWorksheetDetail.priceCode).Name))
            {
                if (GetPriceType(viewName) == PriceTypeList.Customer)
                {
                    viewName = this.ViewNames[CustomerCode.View];
                    if (fieldName == PriceCodeInfo.PriceCodeFieldName)
                        fieldName = typeof(CR.BAccount.acctCD).Name;
                    else if (fieldName == PriceCodeInfo.PriceCodeDescrFieldName)
                        fieldName = typeof(CR.BAccount.acctName).Name;
                    else
                        return null;
                }
                else
                {
                    viewName = this.ViewNames[CustPriceClassCode.View];
                    if (fieldName == PriceCodeInfo.PriceCodeFieldName)
                        fieldName = typeof(ARPriceClass.priceClassID).Name;
                    else if (fieldName == PriceCodeInfo.PriceCodeDescrFieldName)
                        fieldName = typeof(ARPriceClass.description).Name;
                    else
                        return null;
                }
            }
            return base.GetValueExt(viewName, data, fieldName);
        }

		public override void Persist()
		{
			CheckForDuplicateDetails();

			base.Persist();
		}

        #endregion

        #region Actions

        public PXAction<ARPriceWorksheet> ReleasePriceWorksheet;
        [PXUIField(DisplayName = ActionsMessages.Release, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable releasePriceWorksheet(PXAdapter adapter)
        {
            List<ARPriceWorksheet> list = new List<ARPriceWorksheet>();
            if (Document.Current != null)
            {
                this.Save.Press();
                list.Add(Document.Current);
                PXLongOperation.StartOperation(this, delegate() { ReleaseWorksheet(Document.Current); });
            }
            return list;
        }

        public static void ReleaseWorksheet(ARPriceWorksheet priceWorksheet)
        {
            ARPriceWorksheetMaint salesPriceWorksheetMaint = PXGraph.CreateInstance<ARPriceWorksheetMaint>();
            salesPriceWorksheetMaint.Document.Current = priceWorksheet;

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (ARPriceWorksheetDetail priceLine in salesPriceWorksheetMaint.Details.Select())
                {
                    PXResultset<ARSalesPrice> salesPrices = PXSelect<ARSalesPrice, Where<
                            ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                            And2<Where2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>, And<ARSalesPrice.priceType, Equal<PriceTypes.customer>>>,
                            Or2<Where<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>, And<ARSalesPrice.priceType, Equal<PriceTypes.customerPriceClass>>>,
                                Or<Where<ARSalesPrice.custPriceClassID, IsNull, And<ARSalesPrice.customerID, IsNull, And<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>>>>>>>,
                            And<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                            And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,
                            And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
                            And<ARSalesPrice.breakQty, Equal<Required<ARSalesPrice.breakQty>>,
                            And<ARSalesPrice.isPromotionalPrice, Equal<Required<ARSalesPrice.isPromotionalPrice>>>>>>>>>,
                            OrderBy<Asc<ARSalesPrice.effectiveDate, Asc<ARSalesPrice.expirationDate>>>>.Select(salesPriceWorksheetMaint,
                            priceLine.PriceType, priceLine.CustomerID, priceLine.CustPriceClassID, priceLine.InventoryID, priceLine.UOM, priceLine.CuryID,
                            priceLine.BreakQty, salesPriceWorksheetMaint.Document.Current.IsPromotional ?? false);

                    if (salesPriceWorksheetMaint.Document.Current.IsPromotional != true || salesPriceWorksheetMaint.Document.Current.ExpirationDate == null)
                    {
                        bool insertNewPrice = true;
                        if (salesPrices.Count > 0)
                        {
                            foreach (ARSalesPrice salesPrice in salesPrices)
                            {
                                if (salesPriceWorksheetMaint.ARSetup.Current.RetentionType == RetentionTypeList.FixedNumOfMonths)
                                {
                                    if (salesPriceWorksheetMaint.Document.Current.OverwriteOverlapping == true)
                                    {
                                        if ((salesPriceWorksheetMaint.Document.Current.ExpirationDate == null && salesPrice.EffectiveDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate) ||
                                            (salesPriceWorksheetMaint.Document.Current.ExpirationDate != null && salesPrice.EffectiveDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.ExpirationDate))
                                        {
                                            salesPriceWorksheetMaint.ARSalesPrices.Delete(salesPrice);
                                        }
                                        else if (((salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null)
                                            || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate) || (salesPrice.EffectiveDate < salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPriceWorksheetMaint.Document.Current.EffectiveDate <= salesPrice.ExpirationDate))
                                            && salesPriceWorksheetMaint.Document.Current.IsPromotional != true && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                                        }
                                    }
                                    else
                                    {
                                        if ((salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate) ||
                                            salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) insertNewPrice = false;
                                        if (salesPrice.EffectiveDate < salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            ARSalesPrice newSalesPrice = (ARSalesPrice)salesPriceWorksheetMaint.ARSalesPrices.Cache.CreateCopy(salesPrice);
                                            salesPrice.SalesPrice = priceLine.PendingPrice;
                                            salesPrice.EffectiveDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate;
                                            salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);

                                            newSalesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            newSalesPrice.RecordID = null;
                                            salesPriceWorksheetMaint.ARSalesPrices.Insert(newSalesPrice);
                                        }
                                        else if (salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                        {
                                            salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);

                                            salesPriceWorksheetMaint.ARSalesPrices.Insert(CreateSalesPrice(priceLine, false, salesPriceWorksheetMaint.Document.Current.EffectiveDate, salesPriceWorksheetMaint.Document.Current.ExpirationDate));
                                        }
                                        else if (salesPrice.EffectiveDate == salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == salesPriceWorksheetMaint.Document.Current.ExpirationDate)
                                        {
                                            salesPrice.SalesPrice = priceLine.PendingPrice;
                                            salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                                        }
                                        else if ((salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate))
                                        {
                                            salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                            salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((salesPrice.EffectiveDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate) || (salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null && salesPrice.ExpirationDate < salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1)))
                                    {
                                        salesPriceWorksheetMaint.ARSalesPrices.Delete(salesPrice);
                                    }
                                    else if (((salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate == null) || (salesPrice.EffectiveDate == null && salesPrice.ExpirationDate == null) ||
                                        ((salesPrice.EffectiveDate < salesPriceWorksheetMaint.Document.Current.EffectiveDate || salesPrice.EffectiveDate == null) && salesPriceWorksheetMaint.Document.Current.EffectiveDate <= salesPrice.ExpirationDate)) && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                                    {
                                        salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                        salesPrice.EffectiveDate = null;
                                        salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                                    }

                                }
                            }

                            if (insertNewPrice)
                            {
                                if (salesPriceWorksheetMaint.Document.Current.OverwriteOverlapping == true || salesPriceWorksheetMaint.ARSetup.Current.RetentionType == RetentionTypeList.LastPrice)
                                {
                                    salesPriceWorksheetMaint.ARSalesPrices.Insert(CreateSalesPrice(priceLine, false, salesPriceWorksheetMaint.Document.Current.EffectiveDate, salesPriceWorksheetMaint.Document.Current.ExpirationDate));
                                }
                                else
                                {
                                    ARSalesPrice minSalesPrice = PXSelect<ARSalesPrice, Where<
                                        ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                                        And2<Where2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>, And<ARSalesPrice.custPriceClassID, IsNull>>,
                                        Or2<Where<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>, And<ARSalesPrice.customerID, IsNull>>,
                                            Or<Where<ARSalesPrice.custPriceClassID, IsNull, And<ARSalesPrice.customerID, IsNull>>>>>,
                                        And<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                                        And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,
                                        And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
                                        And<ARSalesPrice.breakQty, Equal<Required<ARSalesPrice.breakQty>>,
                                        And<ARSalesPrice.effectiveDate, IsNotNull,
                                        And<Where<ARSalesPrice.effectiveDate, GreaterEqual<Required<ARSalesPrice.effectiveDate>>>>>>>>>>>,
                                        OrderBy<Asc<ARSalesPrice.effectiveDate>>>.SelectSingleBound(salesPriceWorksheetMaint, new object[] { },
                                            priceLine.PriceType, priceLine.CustomerID, priceLine.CustPriceClassID, priceLine.InventoryID, priceLine.UOM, priceLine.CuryID,
                                            priceLine.BreakQty, salesPriceWorksheetMaint.Document.Current.EffectiveDate);

                                    salesPriceWorksheetMaint.ARSalesPrices.Insert(CreateSalesPrice(priceLine, false, salesPriceWorksheetMaint.Document.Current.EffectiveDate, (minSalesPrice != null && minSalesPrice.EffectiveDate != null && minSalesPrice.EffectiveDate.Value != null) ? (DateTime?)minSalesPrice.EffectiveDate.Value.AddDays(-1) : salesPriceWorksheetMaint.Document.Current.ExpirationDate));
                                }
                            }
                        }
                        else
                        {
                            salesPriceWorksheetMaint.ARSalesPrices.Insert(CreateSalesPrice(priceLine, false, salesPriceWorksheetMaint.Document.Current.EffectiveDate, salesPriceWorksheetMaint.Document.Current.ExpirationDate));
                        }
                    }
                    else
                    {
                        foreach (ARSalesPrice salesPrice in salesPrices)
                        {
                            if (salesPrice.EffectiveDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate <= salesPriceWorksheetMaint.Document.Current.ExpirationDate)
                            {
                                salesPriceWorksheetMaint.ARSalesPrices.Delete(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate <= salesPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                            {
                                salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate >= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.EffectiveDate < salesPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.ExpirationDate && salesPriceWorksheetMaint.Document.Current.ExpirationDate.Value != null)
                            {
                                salesPrice.EffectiveDate = salesPriceWorksheetMaint.Document.Current.ExpirationDate.Value.AddDays(1);
                                salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                            }
                            else if (salesPrice.EffectiveDate <= salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPrice.ExpirationDate >= salesPriceWorksheetMaint.Document.Current.ExpirationDate
                                && salesPrice.ExpirationDate > salesPriceWorksheetMaint.Document.Current.EffectiveDate && salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value != null)
                            {
                                salesPrice.ExpirationDate = salesPriceWorksheetMaint.Document.Current.EffectiveDate.Value.AddDays(-1);
                                salesPriceWorksheetMaint.ARSalesPrices.Update(salesPrice);
                            }
                        }
                        salesPriceWorksheetMaint.ARSalesPrices.Insert(CreateSalesPrice(priceLine, true, salesPriceWorksheetMaint.Document.Current.EffectiveDate, salesPriceWorksheetMaint.Document.Current.ExpirationDate));
                    }

                    if (salesPriceWorksheetMaint.ARSetup.Current.RetentionType == RetentionTypeList.FixedNumOfMonths && salesPriceWorksheetMaint.ARSetup.Current.NumberOfMonths != 0)
                    {
                        foreach (ARSalesPrice salesPrice in salesPrices)
                        {
                            if (salesPrice.ExpirationDate != null && ((DateTime)salesPrice.ExpirationDate).AddMonths(salesPriceWorksheetMaint.ARSetup.Current.NumberOfMonths ?? 0) < salesPriceWorksheetMaint.Document.Current.EffectiveDate)
                            {
                                salesPriceWorksheetMaint.ARSalesPrices.Delete(salesPrice);
                            }
                        }
                    }

                }

                priceWorksheet.Status = SPWorksheetStatus.Released;
                salesPriceWorksheetMaint.Document.Update(priceWorksheet);
                salesPriceWorksheetMaint.Document.Current.Status = SPWorksheetStatus.Released;

                salesPriceWorksheetMaint.Persist();
                ts.Complete();
            }
        }

        private static ARSalesPrice CreateSalesPrice(ARPriceWorksheetDetail priceLine, bool? isPromotional, DateTime? effectiveDate, DateTime? expirationDate)
        {
            ARSalesPrice newSalesPrice = new ARSalesPrice();
            newSalesPrice.PriceType = priceLine.PriceType;
            newSalesPrice.CustomerID = priceLine.CustomerID;
            newSalesPrice.CustPriceClassID = priceLine.CustPriceClassID;
            newSalesPrice.InventoryID = priceLine.InventoryID;
            newSalesPrice.UOM = priceLine.UOM;
            newSalesPrice.BreakQty = priceLine.BreakQty;
            newSalesPrice.SalesPrice = priceLine.PendingPrice;
            newSalesPrice.CuryID = priceLine.CuryID;
            newSalesPrice.TaxID = priceLine.TaxID;
            newSalesPrice.IsPromotionalPrice = isPromotional;
            newSalesPrice.EffectiveDate = effectiveDate;
            newSalesPrice.ExpirationDate = expirationDate;
            return newSalesPrice;
        }

        #region AddItem Lookup
        [PXCopyPasteHiddenView]
        public PXFilter<AddItemFilter> addItemFilter;
        [PXCopyPasteHiddenView]
        public PXFilter<AddItemParameters> addItemParameters;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public ARAddItemLookup<ARAddItemSelected, AddItemFilter> addItemLookup;

        public PXAction<ARPriceWorksheet> addItem;
        [PXUIField(DisplayName = Messages.AddItem, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable AddItem(PXAdapter adapter)
        {
            if (addItemLookup.AskExt() == WebDialogResult.OK)
            {
                return AddSelItems(adapter);
            }
            return adapter.Get();
        }

        public PXAction<ARPriceWorksheet> addSelItems;
        [PXUIField(DisplayName = Messages.Add, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddSelItems(PXAdapter adapter)
        {
            if ((addItemParameters.Current.PriceType != PriceTypes.BasePrice && addItemParameters.Current.PriceCode != null) || (addItemParameters.Current.PriceType == PriceTypes.BasePrice && addItemParameters.Current.PriceCode == null))
            {
                foreach (ARAddItemSelected line in addItemLookup.Cache.Cached)
                {
                    if (line.Selected == true)
                    {
                        string priceCode = addItemParameters.Current.PriceCode;
                        if (addItemParameters.Current.PriceType == PriceTypeList.Customer)
                        {
	                        var customer = CustomerRepository.FindByCD(addItemParameters.Current.PriceCode);
	                        if (customer != null)
	                        {
		                        priceCode = customer.BAccountID.ToString();
	                        }
                        }
                        ARPriceWorksheetDetail newline = new ARPriceWorksheetDetail();
                        newline.InventoryID = line.InventoryID;
                        newline.CuryID = addItemParameters.Current.CuryID;
                        newline.UOM = line.BaseUnit;
                        newline.PriceType = addItemParameters.Current.PriceType;
                        newline.CurrentPrice = GetItemPrice(this, addItemParameters.Current.PriceType, priceCode, newline.InventoryID, newline.CuryID, Document.Current.EffectiveDate);
                        if (addItemParameters.Current.PriceType == PriceTypes.Customer)
                            newline.CustomerID = Convert.ToInt32(priceCode);
                        else
                            newline.CustPriceClassID = priceCode;
                        newline.PriceCode = addItemParameters.Current.PriceCode;
                        newline = PXCache<ARPriceWorksheetDetail>.CreateCopy(Details.Update(newline));
                    }
                }
                addItemFilter.Cache.Clear();
                addItemLookup.Cache.Clear();
                addItemParameters.Cache.Clear();
            }
            else
            {
                if (string.IsNullOrEmpty(addItemParameters.Current.PriceCode))
                    addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.priceCode>(addItemParameters.Current, addItemParameters.Current.PriceCode,
                        new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error, typeof(AddItemParameters.priceCode).Name));
            }

            return adapter.Get();
        }

        public override IEnumerable<PXDataRecord> ProviderSelect(BqlCommand command, int topCount, params PXDataValue[] pars)
        {
            return base.ProviderSelect(command, topCount, pars);
        }
        #endregion

        public PXAction<ARPriceWorksheet> copyPrices;
        [PXUIField(DisplayName = Messages.CopyPrices, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable CopyPrices(PXAdapter adapter)
        {
            if (CopyPricesSettings.AskExt() == WebDialogResult.OK)
            {
                if (CopyPricesSettings.Current != null)
                {
                    if (CopyPricesSettings.Current.SourcePriceType != PriceTypes.BasePrice && CopyPricesSettings.Current.SourcePriceCode == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.sourcePriceCode>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.sourcePriceCode).Name));
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
                    if (CopyPricesSettings.Current.DestinationPriceType != PriceTypes.BasePrice && CopyPricesSettings.Current.DestinationPriceCode == null)
                    {
                        CopyPricesSettings.Cache.RaiseExceptionHandling<CopyPricesFilter.destinationPriceCode>(CopyPricesSettings.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CopyPricesFilter.destinationPriceCode).Name));
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

        public static void CopyPricesProc(ARPriceWorksheet priceWorksheet, CopyPricesFilter copyFilter)
        {
            ARPriceWorksheetMaint salesPriceWorksheetMaint = PXGraph.CreateInstance<ARPriceWorksheetMaint>();
            salesPriceWorksheetMaint.Document.Update((ARPriceWorksheet)salesPriceWorksheetMaint.Document.Cache.CreateCopy(priceWorksheet));
            salesPriceWorksheetMaint.CopyPricesSettings.Current = copyFilter;
            string sourcePriceCode = copyFilter.SourcePriceCode;
            if (copyFilter.SourcePriceType == PriceTypeList.Customer)
            {
				var customer = salesPriceWorksheetMaint.CustomerRepository.FindByCD(copyFilter.SourcePriceCode);
	            if (customer != null)
	            {
		            sourcePriceCode = customer.BAccountID.ToString();
	            }
            }

            PXResultset<ARSalesPrice> salesPrices = PXSelectGroupBy<ARSalesPrice, Where<
                    ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                    And2<Where2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>, And<ARSalesPrice.custPriceClassID, IsNull>>,
                    Or2<Where<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>, And<ARSalesPrice.customerID, IsNull>>,
                        Or<Where<ARSalesPrice.custPriceClassID, IsNull, And<ARSalesPrice.customerID, IsNull>>>>>,
                    And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
                    And<ARSalesPrice.isPromotionalPrice, Equal<Required<ARSalesPrice.isPromotionalPrice>>,
                    And<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>, And<ARSalesPrice.expirationDate, IsNull>>,
                    Or<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>, And<ARSalesPrice.expirationDate, Greater<Required<ARSalesPrice.effectiveDate>>>>>>>>>>>,
                                    Aggregate<GroupBy<ARSalesPrice.priceType,
                                    GroupBy<ARSalesPrice.customerID,
                                    GroupBy<ARSalesPrice.custPriceClassID,
                                    GroupBy<ARSalesPrice.inventoryID,
                                    GroupBy<ARSalesPrice.uOM,
                                    GroupBy<ARSalesPrice.breakQty,
                                    GroupBy<ARSalesPrice.curyID>>>>>>>>,
                    OrderBy<Asc<ARSalesPrice.effectiveDate, Asc<ARSalesPrice.expirationDate>>>>.Select(salesPriceWorksheetMaint,
                    copyFilter.SourcePriceType, copyFilter.SourcePriceType == PriceTypes.Customer ? sourcePriceCode : null,
                    copyFilter.SourcePriceType == PriceTypes.CustomerPriceClass ? sourcePriceCode : null, copyFilter.SourceCuryID,
                    copyFilter.IsPromotional ?? false, copyFilter.EffectiveDate, copyFilter.EffectiveDate, copyFilter.EffectiveDate);

            string destinationPriceCode = copyFilter.DestinationPriceCode;
            if (copyFilter.DestinationPriceType == PriceTypeList.Customer)
            {
				var customer = salesPriceWorksheetMaint.CustomerRepository.FindByCD(copyFilter.DestinationPriceCode);
	            if (customer != null)
	            {
		            destinationPriceCode = customer.BAccountID.ToString();
	            }
            }

            foreach (ARSalesPrice salesPrice in salesPrices)
            {
                ARPriceWorksheetDetail newline = new ARPriceWorksheetDetail();
                newline.PriceType = copyFilter.DestinationPriceType;
                newline.PriceCode = copyFilter.DestinationPriceCode;
                if (salesPriceWorksheetMaint.CopyPricesSettings.Current.DestinationPriceType == PriceTypes.Customer)
                    newline.CustomerID = Convert.ToInt32(destinationPriceCode);
                else
                    newline.CustPriceClassID = destinationPriceCode;
                newline.InventoryID = salesPrice.InventoryID;
                newline.UOM = salesPrice.UOM;
                newline.BreakQty = salesPrice.BreakQty;

                decimal currentSalesPrice = salesPrice.SalesPrice ?? 0m;

                if (copyFilter.SourceCuryID != copyFilter.DestinationCuryID)
                {
                    currentSalesPrice = ConvertSalesPrice(salesPriceWorksheetMaint, copyFilter.RateTypeID, copyFilter.SourceCuryID, copyFilter.DestinationCuryID, copyFilter.CurrencyDate, salesPrice.SalesPrice ?? 0m);
                }
                newline.CurrentPrice = currentSalesPrice;
                newline.CuryID = copyFilter.DestinationCuryID;
                newline.TaxID = salesPrice.TaxID;
                salesPriceWorksheetMaint.Details.Update(newline);
            }
            salesPriceWorksheetMaint.Save.Press();
            salesPriceWorksheetMaint.CopyPricesSettings.Cache.Clear();
            PXRedirectHelper.TryRedirect(salesPriceWorksheetMaint, PXRedirectHelper.WindowMode.Same);
        }

        public static decimal ConvertSalesPrice(ARPriceWorksheetMaint graph, string curyRateTypeID, string fromCuryID, string toCuryID, DateTime? curyEffectiveDate, decimal salesPrice)
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

        public PXAction<ARPriceWorksheet> calculatePrices;
        [PXUIField(DisplayName = Messages.CalcPendingPrices, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
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
                foreach (ARPriceWorksheetDetail sp in Details.Select())
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
                        case PriceBasisTypes.LastCost:
                            skipUpdate = settings.UpdateOnZero != true && (ic.LastCost == null || ic.LastCost == 0);
                            if (!skipUpdate)
                            {
                                correctedAmtInBaseUnit = (ic.LastCost ?? 0) + ((ii.MarkupPct ?? 0) * 0.01m * (ic.LastCost ?? 0));

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
                        case PriceBasisTypes.StdCost:
                            if (ii.ValMethod != INValMethod.Standard)
                            {
                                skipUpdate = settings.UpdateOnZero != true && (ic.AvgCost == null || ic.AvgCost == 0);
                                correctedAmtInBaseUnit = (ic.AvgCost ?? 0) + ((ii.MarkupPct ?? 0) * 0.01m * (ic.AvgCost ?? 0));
                            }
                            else
                            {
                                skipUpdate = settings.UpdateOnZero != true && (ii.StdCost == null || ii.StdCost == 0);
                                correctedAmtInBaseUnit = (ii.StdCost ?? 0) + ((ii.MarkupPct ?? 0) * 0.01m * (ii.StdCost ?? 0));
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
                        case PriceBasisTypes.PendingPrice:
                            skipUpdate = settings.UpdateOnZero != true && (sp.PendingPrice == null || sp.PendingPrice == 0);
                            correctedAmt = sp.PendingPrice ?? 0m;
                            break;
                        case PriceBasisTypes.CurrentPrice:
                            skipUpdate = settings.UpdateOnZero != true && (sp.CurrentPrice == null || sp.CurrentPrice == 0);
                            correctedAmt = sp.CurrentPrice ?? 0;
                            break;
                        case PriceBasisTypes.RecommendedPrice:
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

                        ARPriceWorksheetDetail u = (ARPriceWorksheetDetail)Details.Cache.CreateCopy(sp);
                        u.PendingPrice = correctedAmt;
                        Details.Update(u);
                    }
                }

            }
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

        #region ARPriceWorksheet event handlers

        protected virtual void ARPriceWorksheet_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARPriceWorksheet tran = (ARPriceWorksheet)e.Row;
            if (tran == null) return;

            bool allowEdit = (tran.Status == SPWorksheetStatus.Hold || tran.Status == SPWorksheetStatus.Open);

            ReleasePriceWorksheet.SetEnabled(tran.Hold == false && tran.Status == SPWorksheetStatus.Open);
            addItem.SetEnabled(tran.Hold == true && tran.Status == SPWorksheetStatus.Hold);
            copyPrices.SetEnabled(tran.Hold == true && tran.Status == SPWorksheetStatus.Hold);
            calculatePrices.SetEnabled(tran.Hold == true && tran.Status == SPWorksheetStatus.Hold);

            Details.Cache.AllowInsert = allowEdit;
            Details.Cache.AllowDelete = allowEdit;
            Details.Cache.AllowUpdate = allowEdit;

            Document.Cache.AllowDelete = tran.Status != SPWorksheetStatus.Released;

            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.hold>(sender, tran, tran.Status != SPWorksheetStatus.Released);
            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.descr>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.effectiveDate>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.expirationDate>(sender, tran, allowEdit && (ARSetup.Current.RetentionType != AR.RetentionTypeList.LastPrice || (ARSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice && tran.IsPromotional == true)));
            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.isPromotional>(sender, tran, allowEdit);
            PXUIFieldAttribute.SetEnabled<ARPriceWorksheet.overwriteOverlapping>(sender, tran, allowEdit && tran.IsPromotional != true && ARSetup.Current.RetentionType != AR.RetentionTypeList.LastPrice);

            if (ARSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice || tran.IsPromotional == true) tran.OverwriteOverlapping = true;
            if (ARSetup.Current.RetentionType == AR.RetentionTypeList.LastPrice && tran.IsPromotional != true) tran.ExpirationDate = null;


        }

        protected virtual void ARPriceWorksheet_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ARPriceWorksheet doc = (ARPriceWorksheet)e.Row;
            if (doc == null) return;

            doc.Status = doc.Hold == false ? SPWorksheetStatus.Open : SPWorksheetStatus.Hold;
        }

        protected virtual void ARPriceWorksheet_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            ARPriceWorksheet doc = (ARPriceWorksheet)e.Row;
            if (doc == null) return;

            if (doc.IsPromotional == true && doc.ExpirationDate == null)
                sender.RaiseExceptionHandling<ARPriceWorksheet.expirationDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheet.expirationDate).Name));
        }

        protected virtual void ARPriceWorksheet_EffectiveDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARPriceWorksheet doc = (ARPriceWorksheet)e.Row;
            if (doc == null) return;

            if (e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheet.effectiveDate).Name);

            if (doc.IsPromotional == true && doc.ExpirationDate != null && doc.ExpirationDate < (DateTime)e.NewValue)
            {
                throw new PXSetPropertyException(PXMessages.LocalizeFormat(Messages.ExpirationLessThanEffective));
            }
        }

        protected virtual void ARPriceWorksheet_ExpirationDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARPriceWorksheet doc = (ARPriceWorksheet)e.Row;
            if (doc == null) return;

            if (doc.IsPromotional == true && e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheet.expirationDate).Name);

            if (doc.IsPromotional == true && doc.EffectiveDate != null && doc.EffectiveDate > (DateTime)e.NewValue)
            {
                throw new PXSetPropertyException(PXMessages.LocalizeFormat(Messages.ExpirationLessThanEffective));
            }
        }

        #endregion

        #region ARPriceWorksheetDetail event handlers

        protected virtual void ARPriceWorksheetDetail_PriceCode_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;

            if (det != null && det.PriceType != null)
            {
                if (det.PriceType == PriceTypes.Customer)
                {
					var customer = CustomerRepository.FindByID(det.CustomerID);
                    if (customer != null)
                    {
                        e.ReturnState = customer.AcctCD;
                        det.PriceCode = customer.AcctCD;
                    }
                }
                else
                {
                    if (e.ReturnState == null)
                        e.ReturnState = det.CustPriceClassID;
                    if (det.PriceCode == null)
                        det.PriceCode = det.CustPriceClassID;
                }
            }
        }

        protected virtual void ARPriceWorksheetDetail_PriceCode_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;
            if (det == null) return;
            if (e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheetDetail.priceCode).Name);

            if (det.PriceType == PriceTypeList.Customer)
            {
	            CustomerRepository.GetByCD(e.NewValue.ToString());
            }
            if (det.PriceType == PriceTypeList.CustomerPriceClass)
            {
                if (PXSelect<ARPriceClass, Where<ARPriceClass.priceClassID, Equal<Required<ARPriceClass.priceClassID>>>>.Select(this, e.NewValue.ToString()).Count == 0)
                    throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ValueDoesntExist, Messages.CustomerPriceClass, e.NewValue.ToString()));
            }
        }

        protected virtual void ARPriceWorksheetDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;
            if (det == null) return;

            PXUIFieldAttribute.SetEnabled<ARPriceWorksheetDetail.priceCode>(sender, det, det.PriceType != PriceTypeList.BasePrice);
        }

        protected virtual void ARPriceWorksheetDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;
            if (det == null) return;

            if (det.PriceType != null && det.PriceCode != null)
            {
                if (det.PriceType == PriceTypeList.Customer)
                {
	                var customer = CustomerRepository.FindByCD(det.PriceCode);
                    if (customer != null)
                    {
                        det.CustomerID = customer.BAccountID;
                        det.CustPriceClassID = null;
                    }
                }
                else
                {
                    det.CustomerID = null;
                    det.CustPriceClassID = det.PriceCode;
                }
                if (e.ExternalCall && det.InventoryID != null && det.CuryID != null && Document.Current != null && Document.Current.EffectiveDate != null && det.CurrentPrice == 0m)
                    det.CurrentPrice = GetItemPrice(this, det.PriceType, det.PriceCode, det.InventoryID, det.CuryID, Document.Current.EffectiveDate);
            }
        }

        protected virtual void ARPriceWorksheetDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;
            if (det == null) return;

            if (!sender.ObjectsEqual<ARPriceWorksheetDetail.priceType>(e.Row, e.OldRow))
                det.PriceCode = null;

            if (e.ExternalCall && (!sender.ObjectsEqual<ARPriceWorksheetDetail.priceCode>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARPriceWorksheetDetail.uOM>(e.Row, e.OldRow)
                || !sender.ObjectsEqual<ARPriceWorksheetDetail.inventoryID>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARPriceWorksheetDetail.curyID>(e.Row, e.OldRow)))
            {
                det.CurrentPrice = GetItemPrice(this, det.PriceType, det.PriceCode, det.InventoryID, det.CuryID, Document.Current.EffectiveDate);
            }

            if (!sender.ObjectsEqual<ARPriceWorksheetDetail.priceCode>(e.Row, e.OldRow))
            {
                if (det.PriceType == PriceTypeList.Customer)
                {
	                var customer = CustomerRepository.FindByCD(det.PriceCode);
					if (customer != null)
                    {
						det.CustomerID = customer.BAccountID;
                        det.CustPriceClassID = null;
                    }
                }
                else
                {
                    det.CustomerID = null;
                    det.CustPriceClassID = det.PriceCode;
                }
            }
        }

		private void CheckForDuplicateDetails()
		{
			IEnumerable<ARPriceWorksheetDetail> worksheetDetails = PXSelect<
				ARPriceWorksheetDetail,
				Where<
					ARPriceWorksheetDetail.refNbr, Equal<Current<ARPriceWorksheetDetail.refNbr>>>>
				.Select(this)
				.RowCast<ARPriceWorksheetDetail>()
				.ToArray();

			IEqualityComparer<ARPriceWorksheetDetail> duplicateComparer =
				new FieldSubsetEqualityComparer<ARPriceWorksheetDetail>(
					Details.Cache,
					typeof(ARPriceWorksheetDetail.refNbr),
					typeof(ARPriceWorksheetDetail.priceType),
					typeof(ARPriceWorksheetDetail.customerID),
					typeof(ARPriceWorksheetDetail.custPriceClassID),
					typeof(ARPriceWorksheetDetail.inventoryID),
					typeof(ARPriceWorksheetDetail.subItemID),
					typeof(ARPriceWorksheetDetail.uOM),
					typeof(ARPriceWorksheetDetail.curyID),
					typeof(ARPriceWorksheetDetail.breakQty));

			IEnumerable<ARPriceWorksheetDetail> duplicates = worksheetDetails
				.GroupBy(detail => detail, duplicateComparer)
				.Where(duplicatesGroup => duplicatesGroup.HasAtLeastTwoItems())
				.Flatten();
			
			foreach (ARPriceWorksheetDetail duplicate in duplicates)
			{
				Details.Cache.RaiseExceptionHandling<ARPriceWorksheetDetail.priceCode>(
					duplicate,
					duplicate.PriceCode,
					new PXSetPropertyException(
						Messages.DuplicateSalesPriceWS,
						PXErrorLevel.RowError,
						typeof(ARPriceWorksheetDetail.priceCode).Name));
			}
		}

        private decimal GetItemPrice(PXGraph graph, string priceType, string priceCode, int? inventoryID, string toCuryID, DateTime? curyEffectiveDate)
        {
            InventoryItem inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);
            if (inventoryItem != null && inventoryItem.BasePrice != null)
            {
                return ConvertSalesPrice(this, new CMSetupSelect(this).Current.ARRateTypeDflt, new PXSetup<GL.Company>(this).Current.BaseCuryID, toCuryID, curyEffectiveDate, inventoryItem.BasePrice ?? 0m);
            }
            return 0m;
        }

        protected virtual void ARPriceWorksheetDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            ARPriceWorksheetDetail det = (ARPriceWorksheetDetail)e.Row;

            if (det == null) return;

			if (Document.Current?.Hold != true && det.PendingPrice == null)
			{
				sender.RaiseExceptionHandling<ARPriceWorksheetDetail.pendingPrice>(
					det, 
					det.PendingPrice,
					new PXSetPropertyException(
						ErrorMessages.FieldIsEmpty, 
						PXErrorLevel.Error, 
						typeof(ARPriceWorksheetDetail.pendingPrice).Name));

				return;
			}

            if ((det.PriceType == PriceTypes.Customer && det.CustomerID == null) || (det.PriceType == PriceTypes.CustomerPriceClass && det.CustPriceClassID == null))
            {
                sender.RaiseExceptionHandling<ARPriceWorksheetDetail.priceCode>(det, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheetDetail.priceCode).Name));
                return;
            }

            if (Document.Current != null && Document.Current.Hold != true && det.PendingPrice == null)
                sender.RaiseExceptionHandling<ARPriceWorksheetDetail.pendingPrice>(det, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARPriceWorksheetDetail.pendingPrice).Name));

            if (det.PriceType == PriceTypeList.Customer)
            {
	            var customer = CustomerRepository.FindByCD(det.PriceCode);
                if (customer != null)
                {
                    det.CustomerID = customer.BAccountID;
                    det.CustPriceClassID = null;
                }
            }
            else
            {
                det.CustomerID = null;
                det.CustPriceClassID = det.PriceCode;
            }
        }
        #endregion

        #region CopyPricesFilter event handlers

        protected virtual void CopyPricesFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CopyPricesFilter filter = (CopyPricesFilter)e.Row;
            if (filter == null) return;

            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.sourceCuryID>(sender, filter, ARSetup.Current.AlwaysFromBaseCury == false);
            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.destinationCuryID>(sender, filter, ARSetup.Current.AlwaysFromBaseCury == false);

            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.rateTypeID>(sender, filter, filter.SourceCuryID != filter.DestinationCuryID);
            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.currencyDate>(sender, filter, filter.SourceCuryID != filter.DestinationCuryID);

            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.sourcePriceCode>(sender, filter, filter.SourcePriceType != PriceTypeList.BasePrice);
            PXUIFieldAttribute.SetEnabled<CopyPricesFilter.destinationPriceCode>(sender, filter, filter.DestinationPriceType != PriceTypeList.BasePrice);

	        bool mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
			PXUIFieldAttribute.SetVisible<CopyPricesFilter.sourceCuryID>(sender, filter, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CopyPricesFilter.destinationCuryID>(sender, filter, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CopyPricesFilter.currencyDate>(sender, filter, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CopyPricesFilter.rateTypeID>(sender, filter, mcFeatureInstalled);
        }


        protected virtual void CopyPricesFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            CopyPricesFilter parameters = (CopyPricesFilter)e.Row;
            if (parameters == null) return;

            if (!sender.ObjectsEqual<CopyPricesFilter.sourcePriceType>(e.Row, e.OldRow))
                parameters.SourcePriceCode = null;
            if (!sender.ObjectsEqual<CopyPricesFilter.destinationPriceType>(e.Row, e.OldRow))
                parameters.DestinationPriceCode = null;

            if (!sender.ObjectsEqual<CopyPricesFilter.sourcePriceCode>(e.Row, e.OldRow))
            {
                if (parameters.SourcePriceType == PriceTypes.Customer)
                {
                    PXResult<Customer> customer = PXSelect<AR.Customer, Where<AR.Customer.acctCD, Equal<Required<AR.Customer.acctCD>>>>.Select(this, parameters.SourcePriceCode);
                    if (customer != null)
                    {
                        parameters.SourceCuryID = ((Customer)customer).CuryID;
                    }
                }
            }
            if (!sender.ObjectsEqual<CopyPricesFilter.destinationPriceCode>(e.Row, e.OldRow))
            {
                if (parameters.DestinationPriceType == PriceTypes.Customer)
                {
                    PXResult<Customer> customer = PXSelect<AR.Customer, Where<AR.Customer.acctCD, Equal<Required<AR.Customer.acctCD>>>>.Select(this, parameters.DestinationPriceCode);
                    if (customer != null)
                    {
                        parameters.DestinationCuryID = ((Customer)customer).CuryID;
                    }
                }
            }
        }
        #endregion

        #region AddItemParameters event handlers
        protected virtual void AddItemParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            AddItemParameters filter = (AddItemParameters)e.Row;
            if (filter == null) return;
			PXUIFieldAttribute.SetVisible<AddItemParameters.curyID>(sender, filter, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
            PXUIFieldAttribute.SetEnabled<AddItemParameters.priceCode>(sender, filter, filter.PriceType != PriceTypeList.BasePrice);
        }

        protected virtual void AddItemParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            AddItemParameters parameters = (AddItemParameters)e.Row;
            if (parameters == null) return;

            if (!sender.ObjectsEqual<AddItemParameters.priceType>(e.Row, e.OldRow))
                parameters.PriceCode = null;

            if (!sender.ObjectsEqual<AddItemParameters.priceCode>(e.Row, e.OldRow))
            {
                if (parameters.PriceType == PriceTypes.Customer)
                {
                    PXResult<Customer> customer = PXSelect<AR.Customer, Where<AR.Customer.acctCD, Equal<Required<AR.Customer.acctCD>>>>.Select(this, parameters.PriceCode);
                    if (customer != null)
                    {
                        if (((Customer)customer).CuryID != null)
                            parameters.CuryID = ((Customer)customer).CuryID;
                        else
                            sender.SetDefaultExt<AddItemParameters.curyID>(e.Row);
                    }
                }
            }
        }
        #endregion
    }

    public static class PriceCodeInfo
    {
        public const string PriceCodeFieldName = "PriceCode";
        public const string PriceCodeDescrFieldName = "Description";
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
        #region PriceType
        public abstract class priceType : PX.Data.IBqlField
        {
        }
        protected String _PriceType;
        [PXString(1)]
        [PXDefault(PriceTypeList.CustomerPriceClass)]
        [PriceTypeList.List()]
        [PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String PriceType
        {
            get
            {
                return this._PriceType;
            }
            set
            {
                this._PriceType = value;
            }
        }
        #endregion
        #region PriceCode
        public abstract class priceCode : PX.Data.IBqlField
        {
        }
        protected String _PriceCode;
        [PXString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        [PXSelector(typeof(ARPriceWorksheetDetail.priceCode), new Type[] { typeof(ARPriceWorksheetDetail.priceCode), typeof(ARPriceWorksheetDetail.description) }, ValidateValue = false)]
        public virtual String PriceCode
        {
            get
            {
                return this._PriceCode;
            }
            set
            {
                this._PriceCode = value;
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
        #region SourcePriceType
        public abstract class sourcePriceType : PX.Data.IBqlField
        {
        }
        protected String _SourcePriceType;
        [PXString(1, IsFixed = true)]
        [PXDefault(PriceTypeList.CustomerPriceClass)]
        [PriceTypeList.List()]
        [PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String SourcePriceType
        {
            get
            {
                return this._SourcePriceType;
            }
            set
            {
                this._SourcePriceType = value;
            }
        }
        #endregion
        #region SourcePriceCode
        public abstract class sourcePriceCode : PX.Data.IBqlField
        {
        }
        protected String _SourcePriceCode;
        [PXString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault()]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(ARPriceWorksheetDetail.priceCode), new Type[] { typeof(ARPriceWorksheetDetail.priceCode), typeof(ARPriceWorksheetDetail.description) }, ValidateValue = false)]
        public virtual String SourcePriceCode
        {
            get
            {
                return this._SourcePriceCode;
            }
            set
            {
                this._SourcePriceCode = value;
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

        #region DestinationPriceType
        public abstract class destinationPriceType : PX.Data.IBqlField
        {
        }
        protected String _DestinationPriceType;
        [PXString(1, IsFixed = true)]
        [PXDefault(PriceTypeList.CustomerPriceClass)]
        [PriceTypeList.List()]
        [PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String DestinationPriceType
        {
            get
            {
                return this._DestinationPriceType;
            }
            set
            {
                this._DestinationPriceType = value;
            }
        }
        #endregion
        #region DestinationPriceCode
        public abstract class destinationPriceCode : PX.Data.IBqlField
        {
        }
        protected String _DestinationPriceCode;
        [PXString(30, InputMask = ">aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [PXDefault()]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(ARPriceWorksheetDetail.priceCode), new Type[] { typeof(ARPriceWorksheetDetail.priceCode), typeof(ARPriceWorksheetDetail.description) }, ValidateValue = false)]
        public virtual String DestinationPriceCode
        {
            get
            {
                return this._DestinationPriceCode;
            }
            set
            {
                this._DestinationPriceCode = value;
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
        [PXDefault(typeof(ARSetup.defaultRateTypeID))]
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
