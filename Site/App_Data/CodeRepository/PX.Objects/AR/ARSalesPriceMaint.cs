using System;
using System.Collections.Generic;
using PX.Data;
using System.Collections;
using PX.Objects.AR.Repositories;
using PX.Objects.IN;
using PX.TM;
using PX.Objects.SO;
using PX.Objects.GL;
using PX.Objects.CM;
using System.Linq;
using OwnedFilter = PX.TM.OwnedFilter;

namespace PX.Objects.AR
{
    public class ARSalesPriceMaint : PXGraph<ARSalesPriceMaint>
    {

        #region DAC Overrides
        #region ARSalesPrice
        #region PriceType
        [PXDBString(1, IsFixed = true)]
        [PXDefault]
        [PriceTypes.List]
        [PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual void ARSalesPrice_PriceType_CacheAttached(PXCache sender) { }
        #endregion
        #region PriceCode
        [PXString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault(typeof(ARSalesPriceFilter.priceCode), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
	[PXPriceCodeSelector(typeof(ARSalesPrice.priceCode), typeof(ARSalesPrice.priceCode), typeof(ARSalesPrice.description), 
			ValidateValue = false, DescriptionField = typeof(ARSalesPrice.description))]
        public virtual void ARSalesPrice_PriceCode_CacheAttached(PXCache sender) { }
        #endregion
        #region InventoryID
        [Inventory(DisplayName = "Inventory ID")]
        [PXDefault(typeof(ARSalesPriceFilter.inventoryID))]
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<ARSalesPrice.inventoryID>>>>))]
        public virtual void ARSalesPrice_InventoryID_CacheAttached(PXCache sender) { }
        #endregion
        #endregion
        #endregion

        #region Selects/Views
        public PXFilter<ARSalesPriceFilter> Filter;

        [PXFilterable]
        public PXSelectJoin<ARSalesPrice,
            LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARSalesPrice.inventoryID>>>,
            Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
            And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
            And2<Where<Required<ARSalesPriceFilter.priceType>, Equal<PriceTypes.allPrices>, Or<ARSalesPrice.priceType, Equal<Required<ARSalesPriceFilter.priceType>>>>,
            And2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<Required<ARSalesPriceFilter.priceCode>, IsNull>>>,
            And2<Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPriceFilter.inventoryID>>, Or<Required<ARSalesPriceFilter.inventoryID>, IsNull>>,
            And2<Where2<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.effectiveDate, IsNull>>,
            And<Where<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.expirationDate, IsNull>>>>,
            Or<Required<ARSalesPriceFilter.effectiveAsOfDate>, IsNull>>,
            And<Where2<Where<Required<ARSalesPriceFilter.itemClassID>, IsNull,
                    Or<Required<ARSalesPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
                And2<Where<Required<ARSalesPriceFilter.inventoryPriceClassID>, IsNull,
                    Or<Required<ARSalesPriceFilter.inventoryPriceClassID>, Equal<InventoryItem.priceClassID>>>,
                And2<Where<Required<ARSalesPriceFilter.ownerID>, IsNull,
                    Or<Required<ARSalesPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
                And2<Where<Required<ARSalesPriceFilter.myWorkGroup>, Equal<False>,
                         Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<ARSalesPriceFilter.currentOwnerID>>>>,
                And<Where<Required<ARSalesPriceFilter.workGroupID>, IsNull,
                    Or<Required<ARSalesPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>>>,
				OrderBy<Asc<ARSalesPrice.inventoryID,
                        Asc<ARSalesPrice.priceType,
                        Asc<ARSalesPrice.uOM, Asc<ARSalesPrice.breakQty, Asc<ARSalesPrice.effectiveDate>>>>>>> Records;

        public PXSetup<Company> Company;
        public PXSetup<ARSetup> arsetup;

	    public PXSelect<CR.BAccount,
					Where<CR.BAccount.type, Equal<CR.BAccountType.customerType>,
						Or<CR.BAccount.type, Equal<CR.BAccountType.combinedType>>>> CustomerCode;

        public PXSelect<Customer> Customer;
        public PXSelect<ARPriceClass> CustPriceClassCode;

	    protected readonly CustomerRepository CustomerRepository;

		public virtual IEnumerable records()
		{
			var filters = PXView.Filters;
			var list = new List<PXFilterRow>();
			foreach (PXFilterRow f in filters)
			{
				list.Add(f);
				if (String.Equals(f.DataField, typeof(ARSalesPrice.priceCode).Name, StringComparison.OrdinalIgnoreCase)
					&& f.Condition != PXCondition.LIKE && f.Condition != PXCondition.LLIKE && f.Condition != PXCondition.RLIKE
					&& f.Value != null)
				{
					var customer = CustomerRepository.FindByCD(f.Value.ToString());
					if (customer != null)
					{
						f.OrigValue = f.Value = customer.AcctCD;
						if (f.Value2 != null)
						{
							customer = CustomerRepository.FindByCD(f.Value2.ToString());
							if (customer != null)
							{
								f.OrigValue2 = f.Value2 = customer.AcctCD;
							}
						}
					}
				}
			}
			filters.Clear();
			filters.Add(list.ToArray());

			ARSalesPriceFilter filter = Filter.Current;

			string priceCode = ParsePriceCode(this, filter.PriceType, filter.PriceCode);

			CommandPreparing.AddHandler<ARSalesPrice.inventoryID>(ARSalesPriceInventoryIDCommandPreparing);
			CommandPreparing.AddHandler<ARSalesPrice.priceType>(ARSalesPricePriceTypeCommandPreparing);
			foreach (PXResult<ARSalesPrice> res in QSelect(this, Records.View.BqlSelect, new object[] { filter.PriceType, filter.PriceType, filter.PriceType == PriceTypes.Customer ? priceCode : null, filter.PriceType == PriceTypes.CustomerPriceClass ? priceCode : null, priceCode, filter.InventoryID, filter.InventoryID, filter.EffectiveAsOfDate, filter.EffectiveAsOfDate, filter.EffectiveAsOfDate, filter.ItemClassID, filter.ItemClassID, filter.InventoryPriceClassID, filter.InventoryPriceClassID, filter.OwnerID, filter.OwnerID, filter.MyWorkGroup, filter.WorkGroupID, filter.WorkGroupID }))
			{
				ARSalesPrice price = res;
				yield return price;
			}
			CommandPreparing.RemoveHandler<ARSalesPrice.inventoryID>(ARSalesPriceInventoryIDCommandPreparing);
			CommandPreparing.RemoveHandler<ARSalesPrice.priceType>(ARSalesPricePriceTypeCommandPreparing);
		}

		public static IEnumerable QSelect(PXGraph graph, BqlCommand bqlCommand, object[] viewParameters)
		{
			var view = new PXView(graph, false, bqlCommand);
			var startRow = PXView.StartRow;
			int totalRows = 0;
			var list = view.Select(PXView.Currents, viewParameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
								   ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}

        #endregion

        #region Ctors
        public ARSalesPriceMaint()
        {
	        CustomerRepository = new CustomerRepository(this);
        }

	    #endregion

        #region Buttons/Actions
        public PXSave<ARSalesPriceFilter> Save;
        public PXCancel<ARSalesPriceFilter> Cancel;

        public PXAction<ARSalesPriceFilter> createWorksheet;
        [PXUIField(DisplayName = Messages.CreatePriceWorksheet, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable CreateWorksheet(PXAdapter adapter)
        {
            if (Filter.Current != null)
            {
				Save.Press();
                string priceCode = ParsePriceCode(this, Filter.Current.PriceType, Filter.Current.PriceCode);
				ARPriceWorksheetMaint graph = CreateInstance<ARPriceWorksheetMaint>();
                ARPriceWorksheet worksheet = new ARPriceWorksheet();
                graph.Document.Insert(worksheet);
                int startRow = PXView.StartRow;
                int totalRows = 0;

				List<object> allSalesPrices = new PXView(this, false, PXSelectJoin<ARSalesPrice,
                            LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARSalesPrice.inventoryID>>,
                            LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>>,
                            Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
                            And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
                            And2<Where<Required<ARSalesPriceFilter.priceType>, Equal<PriceTypes.allPrices>, Or<ARSalesPrice.priceType, Equal<Required<ARSalesPriceFilter.priceType>>>>,
                            And2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<Required<ARSalesPriceFilter.priceCode>, IsNull>>>,
                            And2<Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPriceFilter.inventoryID>>, Or<Required<ARSalesPriceFilter.inventoryID>, IsNull>>,
                            And2<Where2<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.effectiveDate, IsNull>>,
                            And<Where<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.expirationDate, IsNull>>>>,
                            Or<Required<ARSalesPriceFilter.effectiveAsOfDate>, IsNull>>,
                            And<Where2<Where<Required<ARSalesPriceFilter.itemClassID>, IsNull,
                                    Or<Required<ARSalesPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
                                And2<Where<Required<ARSalesPriceFilter.inventoryPriceClassID>, IsNull,
                                    Or<Required<ARSalesPriceFilter.inventoryPriceClassID>, Equal<InventoryItem.priceClassID>>>,
                                And2<Where<Required<ARSalesPriceFilter.ownerID>, IsNull,
                                    Or<Required<ARSalesPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
                                And2<Where<Required<ARSalesPriceFilter.myWorkGroup>, Equal<False>,
                                         Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<ARSalesPriceFilter.currentOwnerID>>>>,
                                And<Where<Required<ARSalesPriceFilter.workGroupID>, IsNull,
                                    Or<Required<ARSalesPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>>>>.GetCommand()).Select(PXView.Currents, new object[] { Filter.Current.PriceType, Filter.Current.PriceType, Filter.Current.PriceType == PriceTypes.Customer ? priceCode : null, Filter.Current.PriceType == PriceTypes.CustomerPriceClass ? priceCode : null, priceCode, Filter.Current.InventoryID, Filter.Current.InventoryID, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.ItemClassID, Filter.Current.ItemClassID, Filter.Current.InventoryPriceClassID, Filter.Current.InventoryPriceClassID, Filter.Current.OwnerID, Filter.Current.OwnerID, Filter.Current.MyWorkGroup, Filter.Current.WorkGroupID, Filter.Current.WorkGroupID }, PXView.Searches, PXView.SortColumns, PXView.Descendings, Records.View.GetExternalFilters(), ref startRow, PXView.MaximumRows, ref totalRows);

				List<object> groupedSalesPrices = new PXView(this, false, PXSelectJoinGroupBy<ARSalesPrice,
                    LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARSalesPrice.inventoryID>>,
                    LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>>,
                    Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
                    And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
                    And2<Where<Required<ARSalesPriceFilter.priceType>, Equal<PriceTypes.allPrices>, Or<ARSalesPrice.priceType, Equal<Required<ARSalesPriceFilter.priceType>>>>,
                    And2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPriceFilter.priceCode>>, Or<Required<ARSalesPriceFilter.priceCode>, IsNull>>>,
                    And2<Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPriceFilter.inventoryID>>, Or<Required<ARSalesPriceFilter.inventoryID>, IsNull>>,
                    And2<Where2<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.effectiveDate, IsNull>>,
                    And<Where<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPriceFilter.effectiveAsOfDate>>, Or<ARSalesPrice.expirationDate, IsNull>>>>,
                        Or<Required<ARSalesPriceFilter.effectiveAsOfDate>, IsNull>>,
                    And<Where2<Where<Required<ARSalesPriceFilter.itemClassID>, IsNull,
                        Or<Required<ARSalesPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
                    And2<Where<Required<ARSalesPriceFilter.inventoryPriceClassID>, IsNull,
                        Or<Required<ARSalesPriceFilter.inventoryPriceClassID>, Equal<InventoryItem.priceClassID>>>,
                    And2<Where<Required<ARSalesPriceFilter.ownerID>, IsNull,
                        Or<Required<ARSalesPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
                    And2<Where<Required<ARSalesPriceFilter.myWorkGroup>, Equal<False>,
                        Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<ARSalesPriceFilter.currentOwnerID>>>>,
                    And<Where<Required<ARSalesPriceFilter.workGroupID>, IsNull,
                        Or<Required<ARSalesPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>>>,
                    Aggregate<GroupBy<ARSalesPrice.priceType,
                                    GroupBy<ARSalesPrice.customerID,
                                    GroupBy<ARSalesPrice.custPriceClassID,
                                    GroupBy<ARSalesPrice.inventoryID,
                                    GroupBy<ARSalesPrice.uOM,
                                    GroupBy<ARSalesPrice.breakQty,
                                    GroupBy<ARSalesPrice.curyID>>>>>>>>>.GetCommand()).Select(PXView.Currents, new object[] { Filter.Current.PriceType, Filter.Current.PriceType, Filter.Current.PriceType == PriceTypes.Customer ? priceCode : null, Filter.Current.PriceType == PriceTypes.CustomerPriceClass ? priceCode : null, priceCode, Filter.Current.InventoryID, Filter.Current.InventoryID, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.ItemClassID, Filter.Current.ItemClassID, Filter.Current.InventoryPriceClassID, Filter.Current.InventoryPriceClassID, Filter.Current.OwnerID, Filter.Current.OwnerID, Filter.Current.MyWorkGroup, Filter.Current.WorkGroupID, Filter.Current.WorkGroupID }, PXView.Searches, PXView.SortColumns, PXView.Descendings, Records.View.GetExternalFilters(), ref startRow, PXView.MaximumRows, ref totalRows);
                if (allSalesPrices.Count > groupedSalesPrices.Count)
                {
                    throw new PXException(Messages.MultiplePriceRecords);
                }

                foreach (PXResult<ARSalesPrice> res in groupedSalesPrices)
                {
					ARSalesPrice price = res;
					ARPriceWorksheetDetail detail = new ARPriceWorksheetDetail
					{
						RefNbr = graph.Document.Current.RefNbr,
						PriceType = price.PriceType
					};
                    if (detail.PriceType == PriceTypes.Customer)
                    {
						Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, price.CustomerID);
                        if (customer != null) detail.PriceCode = customer.AcctCD;
                    }
                    else
                    {
                        detail.PriceCode = price.CustPriceClassID != ARPriceClass.EmptyPriceClass ? price.CustPriceClassID : null;
                    }
                    detail.CustomerID = price.CustomerID;
                    detail.CustPriceClassID = price.CustPriceClassID != ARPriceClass.EmptyPriceClass ? price.CustPriceClassID : null;
                    detail = graph.Details.Insert(detail);
                    detail.InventoryID = price.InventoryID;
                    detail.UOM = price.UOM;
                    detail.BreakQty = price.BreakQty;
                    detail.CurrentPrice = price.SalesPrice;
                    detail.CuryID = price.CuryID;
                    detail.TaxID = price.TaxID;
                    graph.Details.Update(detail);

                }

                throw new PXRedirectRequiredException(graph, Messages.CreatePriceWorksheet);
            }
            return adapter.Get();
        }

        #endregion

        #region Event Handlers
        public override void Persist()
        {
            foreach (ARSalesPrice price in Records.Cache.Inserted)
            {
                ARSalesPrice lastPrice = FindLastPrice(this, price);
				if (lastPrice?.EffectiveDate > price.EffectiveDate && price.ExpirationDate == null)
                    {
                        Records.Cache.RaiseExceptionHandling<ARSalesPrice.expirationDate>(price, price.ExpirationDate, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<ARSalesPrice.expirationDate>(Records.Cache)));
                        throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<ARSalesPrice.expirationDate>(Records.Cache));
                    }
                ValidateDuplicate(this, Records.Cache, price);
            }
            foreach (ARSalesPrice price in Records.Cache.Updated)
            {
                ARSalesPrice lastPrice = FindLastPrice(this, price);
				if (lastPrice?.EffectiveDate > price.EffectiveDate && price.ExpirationDate == null)
                    {
                        Records.Cache.RaiseExceptionHandling<ARSalesPrice.expirationDate>(price, price.ExpirationDate, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<ARSalesPrice.expirationDate>(Records.Cache)));
                        throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<ARSalesPrice.expirationDate>(Records.Cache));
                    }
                ValidateDuplicate(this, Records.Cache, price);
            }
            base.Persist();
        }

        protected virtual void ARSalesPrice_SalesPrice_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            ARSalesPrice row = e.Row as ARSalesPrice;
            if (row != null)
            {
                InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>>>.Select(this, row.InventoryID);
                if (item != null)
                {
                    if (row.CuryID == Company.Current.BaseCuryID)
                    {
						e.NewValue = row.UOM == item.BaseUnit 
							? item.BasePrice 
							: INUnitAttribute.ConvertToBase(sender, item.InventoryID, row.UOM ?? item.SalesUnit, item.BasePrice.Value, INPrecision.UNITCOST);
                    }
                }
            }
            else
            {
                e.NewValue = 0m;
            }

        }

        protected virtual void ARSalesPrice_PriceType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            ARSalesPrice row = e.Row as ARSalesPrice;
            if (row != null)
            {
                if (Filter.Current != null && Filter.Current.PriceType != PriceTypes.AllPrices)
                    e.NewValue = Filter.Current.PriceType;
                else
                    e.NewValue = PriceTypes.BasePrice;
            }

        }

        protected virtual void ARSalesPrice_SalesPrice_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARSalesPrice row = e.Row as ARSalesPrice;
            if (row != null && MinGrossProfitValidation != MinGrossProfitValidationType.None && row.EffectiveDate != null && PXAccess.FeatureInstalled<CS.FeaturesSet.distributionModule>())
            {
                var r = (PXResult<InventoryItem, INItemCost>)
                PXSelectJoin<InventoryItem,
                    LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.SelectWindowed(this, 0, 1, row.InventoryID);

                InventoryItem item = r;
                INItemCost cost = r;
                if (item != null)
                {
                    decimal newValue = (decimal)e.NewValue;
                    if (row.UOM != item.BaseUnit)
                    {
                        try
                        {
                            newValue = INUnitAttribute.ConvertFromBase(sender, item.InventoryID, row.UOM, newValue, INPrecision.UNITCOST);
                        }
                        catch (PXUnitConversionException)
                        {
                            sender.RaiseExceptionHandling<ARSalesPrice.salesPrice>(row, e.NewValue, new PXSetPropertyException(SO.Messages.FailedToConvertToBaseUnits, PXErrorLevel.Warning));
                            return;
                        }
                    }

                    decimal minPrice = PXPriceCostAttribute.MinPrice(item, cost);
                    if (row.CuryID != Company.Current.BaseCuryID)
                    {
                        ARSetup arsetup = PXSetup<ARSetup>.Select(this);

                        if (string.IsNullOrEmpty(arsetup.DefaultRateTypeID))
                        {
                            throw new PXException(SO.Messages.DefaultRateNotSetup);
                        }

                        minPrice = ConvertAmt(Company.Current.BaseCuryID, row.CuryID, arsetup.DefaultRateTypeID, row.EffectiveDate.Value, minPrice);
                    }


                    if (newValue < minPrice)
                    {
                        switch (MinGrossProfitValidation)
                        {
                            case MinGrossProfitValidationType.Warning:
                                sender.RaiseExceptionHandling<ARSalesPrice.salesPrice>(row, e.NewValue, new PXSetPropertyException(SO.Messages.GrossProfitValidationFailed, PXErrorLevel.Warning));
                                break;
                            case MinGrossProfitValidationType.SetToMin:
                                e.NewValue = minPrice;
                                sender.RaiseExceptionHandling<ARSalesPrice.salesPrice>(row, e.NewValue, new PXSetPropertyException(SO.Messages.GrossProfitValidationFailedAndPriceFixed, PXErrorLevel.Warning));
                                break;
                        }
                    }
					else if (MinGrossProfitValidation != MinGrossProfitValidationType.None && minPrice == 0m && item.ValMethod != INValMethod.Standard)
					{
						sender.RaiseExceptionHandling<ARSalesPrice.salesPrice>(row, e.NewValue, new PXSetPropertyException(SO.Messages.GrossProfitValidationFailedNoCost, PXErrorLevel.Warning));
					}
                }
            }
        }

        protected virtual void ARSalesPrice_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetDefaultExt<ARSalesPrice.salesPrice>(e.Row);
        }


        protected virtual void ARSalesPrice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ARSalesPrice row = (ARSalesPrice)e.Row;
            ARSalesPrice oldRow = (ARSalesPrice)e.OldRow;
            if (!sender.ObjectsEqual<ARSalesPrice.priceType>(row, oldRow))
                row.PriceCode = null;

            if (!sender.ObjectsEqual<ARSalesPrice.priceCode>(row, oldRow))
            {
                UpdateCustomerAndPriceClassFields(row);
            }
        }

        protected virtual void ARSalesPrice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARSalesPrice row = (ARSalesPrice)e.Row;
            if (row != null)
            {
                PXUIFieldAttribute.SetEnabled<ARSalesPrice.priceType>(sender, row, Filter.Current.PriceType == PriceTypes.AllPrices);
                PXUIFieldAttribute.SetEnabled<ARSalesPrice.inventoryID>(sender, row, Filter.Current.InventoryID == null);
                PXUIFieldAttribute.SetEnabled<ARSalesPrice.priceCode>(sender, row, row.PriceType != PriceTypes.BasePrice);
            }
        }

        protected virtual void ARSalesPrice_PriceCode_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            ARSalesPrice det = (ARSalesPrice)e.Row;
			if (det?.PriceType == null) return;

                if (det.PriceType == PriceTypes.Customer)
                {
				Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, det.CustomerID);
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

        protected virtual void ARSalesPrice_PriceCode_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARSalesPrice det = (ARSalesPrice)e.Row;
            if (det == null) return;
            if (e.NewValue == null)
                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARSalesPrice.priceCode).Name);

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

        protected virtual void ARSalesPriceFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(OwnedFilter.ownerID).Name, e.Row == null || (bool?)sender.GetValue(e.Row, typeof(OwnedFilter.myOwner).Name) == false);
            PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(OwnedFilter.workGroupID).Name, e.Row == null || (bool?)sender.GetValue(e.Row, typeof(OwnedFilter.myWorkGroup).Name) == false);
            PXUIFieldAttribute.SetEnabled<ARSalesPrice.priceCode>(sender, e.Row, (((ARSalesPriceFilter)e.Row).PriceType != PriceTypes.AllPrices && ((ARSalesPriceFilter)e.Row).PriceType != PriceTypes.BasePrice));
        }

        protected virtual void ARSalesPriceFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ARSalesPriceFilter filter = (ARSalesPriceFilter)e.Row;
            ARSalesPriceFilter oldFilter = (ARSalesPriceFilter)e.OldRow;
            if (!sender.ObjectsEqual<ARSalesPriceFilter.priceType>(oldFilter, filter))
                filter.PriceCode = null;
        }

        protected virtual void ARSalesPrice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            ARSalesPrice row = (ARSalesPrice)e.Row;
            if (row.IsPromotionalPrice == true && row.ExpirationDate == null)
            {
                sender.RaiseExceptionHandling<ARSalesPrice.expirationDate>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARSalesPrice.expirationDate).Name));
            }
            if (row.IsPromotionalPrice == true && row.EffectiveDate == null)
            {
                sender.RaiseExceptionHandling<ARSalesPrice.effectiveDate>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARSalesPrice.effectiveDate).Name));
            }
            if (row.ExpirationDate < row.EffectiveDate)
            {
                sender.RaiseExceptionHandling<ARSalesPrice.expirationDate>(row, row.ExpirationDate, new PXSetPropertyException(Messages.EffectiveDateExpirationDate, PXErrorLevel.RowError));
            }

            if (row.PriceType != PriceTypes.BasePrice && row.PriceCode == null)
            {
                sender.RaiseExceptionHandling<ARSalesPrice.priceCode>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARSalesPrice.priceCode).Name));
                return;
            }

            UpdateCustomerAndPriceClassFields(row);
        }

		protected virtual void ARSalesPriceInventoryIDCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.External) == PXDBOperation.External && e.Value == null)
			{
				e.FieldName = SqlDialect.quoteTableAndColumn(typeof(InventoryItem).Name, typeof(InventoryItem.inventoryCD).Name);
				e.Value = 0;
				e.Cancel = true;
			}
		}

		protected virtual void ARSalesPricePriceTypeCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.External) == PXDBOperation.External && e.Value == null)
			{
				e.FieldName = SqlDialect.quoteTableAndColumn(typeof(ARSalesPrice).Name, typeof(ARSalesPrice.priceType).Name);
				e.Value = "";
				e.Cancel = true;
			}
		}

		#endregion

        #region Private Members
        public string GetPriceType(string viewname)
        {
			return viewname.Contains(typeof(ARSalesPriceFilter).Name) && Filter.Current != null
				? Filter.Current.PriceType
				: (viewname.Contains(typeof(ARSalesPrice).Name) && Records.Current != null
					? Records.Current.PriceType
					: PriceTypeList.Customer);
        }

        #region Overrides
        public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
        {
			if ((viewName.Contains(typeof(ARSalesPrice.priceCode).Name) || viewName.Contains("PriceCode")) && ViewNames.ContainsKey(CustomerCode.View) && ViewNames.ContainsKey(CustPriceClassCode.View))
            {
                if (GetPriceType(viewName) == PriceTypeList.Customer)
                {
					viewName = ViewNames[CustomerCode.View];
					if (sortcolumns.Any())
					{
						sortcolumns[0] = typeof(CR.BAccount.acctCD).Name;
					}
					ModifyFilters(filters, typeof(ARSalesPrice.priceCode).Name, typeof(CR.BAccount.acctCD).Name);
					ModifyFilters(filters, typeof(ARSalesPrice.description).Name, typeof(CR.BAccount.acctName).Name);
				}
                else
                {
					viewName = ViewNames[CustPriceClassCode.View];
					if (sortcolumns.Any())
					{
						sortcolumns[0] = typeof(ARPriceClass.priceClassID).Name;
					}
                    ModifyFilters(filters, typeof(ARSalesPrice.priceCode).Name, typeof(ARPriceClass.priceClassID).Name);
                }
            }
            IEnumerable ret = base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
            return ret;
        }

		public override Type GetItemType(string viewName)
		{
			if ((viewName.Contains(typeof(ARSalesPrice.priceCode).Name) || viewName.Contains("PriceCode")) && this.ViewNames.ContainsKey(CustomerCode.View) && this.ViewNames.ContainsKey(CustPriceClassCode.View))
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

		private static void ModifyFilters(PXFilterRow[] filters, string originalFieldName, string newFieldName)
        {
            if (filters != null)
			{
                foreach (PXFilterRow filter in filters)
                {
                    if (filter.DataField == originalFieldName)
					{
                        filter.DataField = newFieldName;
                }
				}
			}
        }

        public override object GetValueExt(string viewName, object data, string fieldName)
        {
            if (viewName.Contains(typeof(ARSalesPrice.priceCode).Name))
            {
                if (GetPriceType(viewName) == PriceTypeList.Customer)
                {
					viewName = ViewNames[CustomerCode.View];
					switch (fieldName)
					{
						case PriceCodeInfo.PriceCodeFieldName:
                        fieldName = typeof(CR.BAccount.acctCD).Name;
							break;
						case PriceCodeInfo.PriceCodeDescrFieldName:
                        fieldName = typeof(CR.BAccount.acctName).Name;
							break;
						default:
                        return null;
                }
				}
                else
                {
					viewName = ViewNames[CustPriceClassCode.View];
					switch (fieldName)
					{
						case PriceCodeInfo.PriceCodeFieldName:
                        fieldName = typeof(ARPriceClass.priceClassID).Name;
							break;
						case PriceCodeInfo.PriceCodeDescrFieldName:
                        fieldName = typeof(ARPriceClass.description).Name;
							break;
						default:
                        return null;
                }
            }
			}
            return base.GetValueExt(viewName, data, fieldName);
        }
        #endregion

        public static void ValidateDuplicate(PXGraph graph, PXCache sender, ARSalesPrice price)
        {
            PXSelectBase<ARSalesPrice> selectDuplicate = new PXSelect<ARSalesPrice, Where<ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                                                    And2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>, And<ARSalesPrice.custPriceClassID, IsNull,
                                                    Or<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>, And<ARSalesPrice.customerID, IsNull,
                                                    Or<ARSalesPrice.custPriceClassID, IsNull, And<ARSalesPrice.customerID, IsNull>>>>>>,
                                                    And<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                                                    And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,
                                                    And<ARSalesPrice.isPromotionalPrice, Equal<Required<ARSalesPrice.isPromotionalPrice>>,
                                                    And<ARSalesPrice.breakQty, Equal<Required<ARSalesPrice.breakQty>>,
                                                    And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
                                                    And<ARSalesPrice.recordID, NotEqual<Required<ARSalesPrice.recordID>>>>>>>>>>>(graph);

            string priceCode = ParsePriceCode(graph, price.PriceType, price.PriceCode);
            int? customerID = null;
            if (price.PriceType == PriceTypes.Customer && priceCode != null)
			{
				customerID = int.Parse(priceCode);
			}
            foreach (ARSalesPrice arPrice in selectDuplicate.Select(price.PriceType, customerID, price.PriceType == PriceTypes.CustomerPriceClass ? priceCode : null, price.InventoryID, price.UOM, price.IsPromotionalPrice, price.BreakQty, price.CuryID, price.RecordID))
            {
                if (IsOverlapping(arPrice, price))
                {
					PXSetPropertyException exception = new PXSetPropertyException(
						Messages.DuplicateSalesPrice,
						PXErrorLevel.RowError,
						arPrice.SalesPrice,
						arPrice.EffectiveDate?.ToShortDateString() ?? string.Empty,
						arPrice.ExpirationDate?.ToShortDateString() ?? string.Empty);
					sender.RaiseExceptionHandling<ARSalesPrice.uOM>(price, price.UOM, exception);
					throw exception;
                }
            }
        }

        public static ARSalesPrice FindLastPrice(PXGraph graph, ARSalesPrice price)
        {
            string priceCode = ParsePriceCode(graph, price.PriceType, price.PriceCode);
            ARSalesPrice lastPrice = new PXSelect<ARSalesPrice, Where<ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                And2<Where2<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPriceFilter.priceCode>>, And<ARSalesPrice.custPriceClassID, IsNull>>,
                Or<Where<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPriceFilter.priceCode>>, And<ARSalesPrice.customerID, IsNull>>>>,
                And<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,
                And<ARSalesPrice.isPromotionalPrice, Equal<Required<ARSalesPrice.isPromotionalPrice>>,
                And<ARSalesPrice.breakQty, Equal<Required<ARSalesPrice.breakQty>>,
                And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
                And<ARSalesPrice.recordID, NotEqual<Required<ARSalesPrice.recordID>>>>>>>>>>,
                OrderBy<Desc<ARSalesPrice.effectiveDate>>>(graph).SelectSingle(price.PriceType, price.PriceType == PriceTypes.Customer ? priceCode : null, price.PriceType == PriceTypes.CustomerPriceClass ? priceCode : null, price.InventoryID, price.UOM, price.IsPromotionalPrice, price.BreakQty, price.CuryID, price.RecordID);
            return lastPrice;
        }

        private static string ParsePriceCode(PXGraph graph, string priceType, string priceCode)
        {
            if (priceCode != null)
            {
                if (priceType == PriceTypes.Customer)
                {
	                var customerRepository = new CustomerRepository(graph);
					
					Customer customer = customerRepository.FindByCD(priceCode);
	                if (customer != null)
	                {
		                return customer.BAccountID.ToString();
	                }
                }
				return priceType == PriceTypes.CustomerPriceClass ? priceCode : null;
            }
            else
				return null;

        }

        private decimal ConvertAmt(string from, string to, string rateType, DateTime effectiveDate, decimal amount, decimal? customRate = 1)
        {
            if (from == to)
			{
                return amount;
			}

            CurrencyRate rate = getCuryRate(from, to, rateType, effectiveDate);

            if (rate == null)
            {
                return amount * customRate ?? 1;
            }
            else
            {
                return rate.CuryMultDiv == "M" ? amount * rate.CuryRate ?? 1 : amount / rate.CuryRate ?? 1;
            }
        }

        private CurrencyRate getCuryRate(string from, string to, string curyRateType, DateTime curyEffDate)
        {
            return PXSelectReadonly<CurrencyRate,
                            Where<CurrencyRate.toCuryID, Equal<Required<CurrencyRate.toCuryID>>,
                            And<CurrencyRate.fromCuryID, Equal<Required<CurrencyRate.fromCuryID>>,
                            And<CurrencyRate.curyRateType, Equal<Required<CurrencyRate.curyRateType>>,
                            And<CurrencyRate.curyEffDate, LessEqual<Required<CurrencyRate.curyEffDate>>>>>>,
                            OrderBy<Desc<CurrencyRate.curyEffDate>>>.SelectWindowed(this, 0, 1, to, from, curyRateType, curyEffDate);
        }

        private string MinGrossProfitValidation
        {
            get
            {
                SOSetup sosetup = PXSelect<SOSetup>.Select(this);
                if (sosetup != null)
                {
                    if (string.IsNullOrEmpty(sosetup.MinGrossProfitValidation))
                        return MinGrossProfitValidationType.Warning;
                    else
                        return sosetup.MinGrossProfitValidation;
                }
                else
                    return MinGrossProfitValidationType.Warning;

            }
        }

        private void UpdateCustomerAndPriceClassFields(ARSalesPrice row)
        {
			switch (row.PriceType)
            {
				case PriceTypeList.Customer:
					Customer customer = CustomerRepository.FindByCD(row.PriceCode);
                if (customer != null)
                {
                    row.CustomerID = customer.BAccountID;
                    row.CustPriceClassID = null;
                }
					break;
				case PriceTypeList.CustomerPriceClass:
                row.CustomerID = null;
                row.CustPriceClassID = row.PriceCode;
					break;
				case PriceTypeList.BasePrice:
                row.CustomerID = null;
                row.CustPriceClassID = null;
					break;
            }
        }

        public static bool IsOverlapping(ARSalesPrice salesPrice1, ARSalesPrice salesPrice2)
        {
            return ((salesPrice1.EffectiveDate != null && salesPrice1.ExpirationDate != null && salesPrice2.EffectiveDate != null && salesPrice2.ExpirationDate != null && (salesPrice1.EffectiveDate <= salesPrice2.EffectiveDate && salesPrice1.ExpirationDate >= salesPrice2.EffectiveDate || salesPrice1.EffectiveDate <= salesPrice2.ExpirationDate && salesPrice1.ExpirationDate >= salesPrice2.ExpirationDate || salesPrice1.EffectiveDate >= salesPrice2.EffectiveDate && salesPrice1.EffectiveDate <= salesPrice2.ExpirationDate))
                        || (salesPrice1.ExpirationDate != null && salesPrice2.EffectiveDate != null && (salesPrice2.ExpirationDate == null || salesPrice1.EffectiveDate == null) && salesPrice2.EffectiveDate <= salesPrice1.ExpirationDate)
                        || (salesPrice1.EffectiveDate != null && salesPrice2.ExpirationDate != null && (salesPrice2.EffectiveDate == null || salesPrice1.ExpirationDate == null) && salesPrice2.ExpirationDate >= salesPrice1.EffectiveDate)
                        || (salesPrice1.EffectiveDate != null && salesPrice2.EffectiveDate != null && salesPrice1.ExpirationDate == null && salesPrice2.ExpirationDate == null)
                        || (salesPrice1.ExpirationDate != null && salesPrice2.ExpirationDate != null && salesPrice1.EffectiveDate == null && salesPrice2.EffectiveDate == null)
                        || (salesPrice1.EffectiveDate == null && salesPrice1.ExpirationDate == null)
                        || (salesPrice2.EffectiveDate == null && salesPrice2.ExpirationDate == null));
        }
        #endregion

        #region Sales Price Calculation

        /// <summary>
        /// Calculates Sales Price.
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="inventoryID">Inventory</param>
        /// <param name="curyID">Currency</param>
        /// <param name="UOM">Unit of measure</param>
        /// <param name="date">Date</param>
        /// <returns>Sales Price.</returns>
        /// <remarks>AlwaysFromBaseCury flag in the SOSetup is considered when performing the calculation.</remarks>
        public static decimal? CalculateSalesPrice(PXCache sender, string custPriceClass, int? inventoryID, CurrencyInfo currencyinfo, string UOM, DateTime date)
        {
            bool alwaysFromBase = false;

            ARSetup arsetup = (ARSetup)sender.Graph.Caches[typeof(ARSetup)].Current ?? PXSelect<ARSetup>.Select(sender.Graph);
            if (arsetup != null)
            {
                alwaysFromBase = arsetup.AlwaysFromBaseCury == true;
            }

			return CalculateSalesPrice(sender, custPriceClass, null, inventoryID, currencyinfo, 0m, UOM, date, alwaysFromBase);
        }

        /// <summary>
        /// Calculates Sales Price.
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="inventoryID">Inventory</param>
        /// <param name="curyID">Currency</param>
        /// <param name="UOM">Unit of measure</param>
        /// <param name="date">Date</param>
        /// <returns>Sales Price.</returns>
        /// <remarks>AlwaysFromBaseCury flag in the SOSetup is considered when performing the calculation.</remarks>
        public static decimal? CalculateSalesPrice(PXCache sender, string custPriceClass, int? customerID, int? inventoryID, CurrencyInfo currencyinfo, string UOM, decimal? quantity, DateTime date, decimal? currentUnitPrice)
        {
            bool alwaysFromBase = false;

            ARSetup arsetup = (ARSetup)sender.Graph.Caches[typeof(ARSetup)].Current ?? PXSelect<ARSetup>.Select(sender.Graph);
            if (arsetup != null)
            {
                alwaysFromBase = arsetup.AlwaysFromBaseCury == true;
            }

			decimal? salesPrice = CalculateSalesPrice(sender, custPriceClass, customerID, inventoryID, currencyinfo, Math.Abs(quantity ?? 0m), UOM, date, alwaysFromBase);
			return (salesPrice == null || salesPrice == 0) && currentUnitPrice != null && currentUnitPrice != 0m
				? currentUnitPrice
				: salesPrice;
        }

        /// <summary>
        /// Calculates Sales Price.
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="inventoryID">Inventory</param>
        /// <param name="curyID">Currency</param>
        /// <param name="UOM">Unit of measure</param>
        /// <param name="date">Date</param>
        /// <param name="alwaysFromBaseCurrency">If true sales price is always calculated (converted) from Base Currency.</param>
        /// <returns>Sales Price.</returns>
        public static decimal? CalculateSalesPrice(PXCache sender, string custPriceClass, int? inventoryID, CurrencyInfo currencyinfo, string UOM, DateTime date, bool alwaysFromBaseCurrency)
        {
            return CalculateSalesPrice(sender, custPriceClass, null, inventoryID, currencyinfo, 0m, UOM, date, alwaysFromBaseCurrency);
        }

        /// <summary>
        /// Calculates Sales Price.
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="inventoryID">Inventory</param>
        /// <param name="curyID">Currency</param>
        /// <param name="UOM">Unit of measure</param>
        /// <param name="date">Date</param>
        /// <param name="alwaysFromBaseCurrency">If true sales price is always calculated (converted) from Base Currency.</param>
        /// <returns>Sales Price.</returns>
        public static decimal? CalculateSalesPrice(PXCache sender, string custPriceClass, int? customerID, int? inventoryID, CurrencyInfo currencyinfo, decimal? quantity, string UOM, DateTime date, bool alwaysFromBaseCurrency)
        {
            //InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
            SalesPriceItem spItem;
            try
            {
                spItem = FindSalesPrice(sender, custPriceClass, customerID, inventoryID, currencyinfo.BaseCuryID, alwaysFromBaseCurrency ? currencyinfo.BaseCuryID : currencyinfo.CuryID, Math.Abs(quantity ?? 0m), UOM, date);
            }
            catch (PXUnitConversionException)
            {
                return null;
            }

            if (spItem != null)
            {
                decimal salesPrice = spItem.Price;

                if (spItem.CuryID != currencyinfo.CuryID)
                {
                    if (currencyinfo.CuryRate == null)
                        throw new PXSetPropertyException(CM.Messages.RateNotFound, PXErrorLevel.Warning);
                    PXCurrencyAttribute.CuryConvCury(sender, currencyinfo, spItem.Price, out salesPrice);
                }

                if (UOM == null)
                {
                    return null;
                }

                if (spItem.UOM != UOM)
                {
                    decimal salesPriceInBase = INUnitAttribute.ConvertFromBase(sender, inventoryID, spItem.UOM, salesPrice, INPrecision.UNITCOST);
                    salesPrice = INUnitAttribute.ConvertToBase(sender, inventoryID, UOM, salesPriceInBase, INPrecision.UNITCOST);
                }

                return salesPrice;
            }

            return null;
        }

        public static SalesPriceItem FindSalesPrice(PXCache sender, string custPriceClass, int? inventoryID, string curyID, string UOM, DateTime date)
        {
            return FindSalesPrice(sender, custPriceClass, null, inventoryID, new PXSetup<Company>(sender.Graph).Current.BaseCuryID, curyID, 0m, UOM, date);
        }

        public static SalesPriceItem FindSalesPrice(PXCache sender, string custPriceClass, int? customerID, int? inventoryID, string baseCuryID, string curyID, decimal? quantity, string UOM, DateTime date)
        {
            PXSelectBase<ARSalesPrice> salesPrice = new PXSelect<ARSalesPrice, Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                     And2<Where2<Where<ARSalesPrice.priceType, Equal<PriceTypes.customer>, And<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>>>,
                     Or2<Where<ARSalesPrice.priceType, Equal<PriceTypes.customerPriceClass>, And<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>>>,
                     Or<Where<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>, And<Required<ARSalesPrice.customerID>, IsNull, And<Required<ARSalesPrice.custPriceClassID>, IsNull>>>>>>,
            And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
            And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,

            And<Where2<Where<ARSalesPrice.breakQty, LessEqual<Required<ARSalesPrice.breakQty>>>,
            And<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>,
                     And<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPrice.expirationDate>>>>,
            Or2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>,
            And<ARSalesPrice.expirationDate, IsNull>>,
                     Or<Where<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPrice.expirationDate>>,
            And<ARSalesPrice.effectiveDate, IsNull,
            Or<ARSalesPrice.effectiveDate, IsNull, And<ARSalesPrice.expirationDate, IsNull>>>>>>>>>>>>>>,

            OrderBy<Asc<ARSalesPrice.priceType, Desc<ARSalesPrice.isPromotionalPrice, Desc<ARSalesPrice.breakQty>>>>>(sender.Graph);

            PXSelectBase<ARSalesPrice> selectWithBaseUOM = new PXSelectJoin<ARSalesPrice, InnerJoin<InventoryItem,
                         On<InventoryItem.inventoryID, Equal<ARSalesPrice.inventoryID>,
                         And<InventoryItem.baseUnit, Equal<ARSalesPrice.uOM>>>>, Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>,
                     And2<Where2<Where<ARSalesPrice.priceType, Equal<PriceTypes.customer>, And<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>>>,
                     Or2<Where<ARSalesPrice.priceType, Equal<PriceTypes.customerPriceClass>, And<ARSalesPrice.custPriceClassID, Equal<Required<ARSalesPrice.custPriceClassID>>>>,
                        Or<Where<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>, And<Required<ARSalesPrice.customerID>, IsNull, And<Required<ARSalesPrice.custPriceClassID>, IsNull>>>>>>,
                     And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,

                     And<Where2<Where<ARSalesPrice.breakQty, LessEqual<Required<ARSalesPrice.breakQty>>>,
                     And<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>,
                     And<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPrice.expirationDate>>>>,
                     Or2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>,
                     And<ARSalesPrice.expirationDate, IsNull>>,
                     Or<Where<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPrice.expirationDate>>,
                     And<ARSalesPrice.effectiveDate, IsNull,
                     Or<ARSalesPrice.effectiveDate, IsNull, And<ARSalesPrice.expirationDate, IsNull>>>>>>>>>>>>>,

            OrderBy<Asc<ARSalesPrice.priceType, Desc<ARSalesPrice.isPromotionalPrice, Desc<ARSalesPrice.breakQty>>>>>(sender.Graph);

            ARSalesPrice item = salesPrice.SelectWindowed(0, 1, inventoryID, customerID, custPriceClass, customerID, custPriceClass, curyID, UOM, quantity, date, date, date, date);

			string uomFound;

            if (item == null)
            {
                decimal baseUnitQty = INUnitAttribute.ConvertToBase(sender, inventoryID, UOM, (decimal)quantity, INPrecision.QUANTITY);
                item = selectWithBaseUOM.Select(inventoryID, customerID, custPriceClass, customerID, custPriceClass, curyID, baseUnitQty, date, date, date, date);

                if (item == null)
                {
                    item = salesPrice.SelectWindowed(0, 1, inventoryID, customerID, custPriceClass, null, null, curyID, UOM, quantity, date, date, date, date);
                    if (item == null)
                    {
                        item = selectWithBaseUOM.Select(inventoryID, customerID, custPriceClass, null, null, curyID, baseUnitQty, date, date, date, date);

                        if (item == null)
                        {
                            InventoryItem inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
							return inventoryItem?.BasePrice != null ? new SalesPriceItem(inventoryItem.BaseUnit, inventoryItem.BasePrice ?? 0m, baseCuryID) : null;
                            }
                            uomFound = item.UOM;
                        }
                    else
                    {
                        uomFound = UOM;
                    }

                }
                else
                {
                    uomFound = item.UOM;
                }
            }
            else
            {
                uomFound = UOM;
            }
            return new SalesPriceItem(uomFound, (item.SalesPrice ?? 0), item.CuryID);
        }

        public class SalesPriceItem
        {
			public string UOM { get; }

			public decimal Price { get; }

			public string CuryID { get; }

            public SalesPriceItem(string uom, decimal price, string curyid)
            {
				UOM = uom;
				Price = price;
				CuryID = curyid;
            }

        }

        #endregion

    }

	public sealed class CustomerPriceClassAttribute : PXSelectorAttribute
        {
		public CustomerPriceClassAttribute():base(typeof(ARPriceClass.priceClassID))
            {
			DescriptionField = typeof(ARPriceClass.description);
        }
    }

    [Serializable]
    public partial class ARSalesPriceFilter : IBqlTable
    {
        #region PriceType
        public abstract class priceType : PX.Data.IBqlField
        {
        }
        protected String _PriceType;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(PriceTypes.AllPrices)]
        [PriceTypes.ListWithAll]
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
        [PXDBString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
        [PXPriceCodeSelector(typeof(ARSalesPrice.priceCode), new Type[] { typeof(ARSalesPrice.priceCode), typeof(ARSalesPrice.description) }, ValidateValue = false)]
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

        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [Inventory(DisplayName = "Inventory ID")]
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
        #region EffectiveAsOfDate
        public abstract class effectiveAsOfDate : PX.Data.IBqlField
        {
        }
        private DateTime? _EffectiveAsOfDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Effective As Of", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? EffectiveAsOfDate
        {
            get
            {
                return this._EffectiveAsOfDate;
            }
            set
            {
                this._EffectiveAsOfDate = value;
            }
        }
        #endregion
        #region ItemClassID
        public abstract class itemClassID : PX.Data.IBqlField
        {
        }
        protected String _ItemClassID;
        [PXDBString(10)]
        [PXUIField(DisplayName = "Item Class ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(INItemClass.itemClassID), DescriptionField = typeof(INItemClass.descr))]
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
        #region InventoryPriceClassID
        public abstract class inventoryPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _InventoryPriceClassID;
        [PXDBString(10)]
        [PXSelector(typeof(INPriceClass.priceClassID))]
        [PXUIField(DisplayName = "Price Class", Visibility = PXUIVisibility.Visible)]
        public virtual String InventoryPriceClassID
        {
            get
            {
                return this._InventoryPriceClassID;
            }
            set
            {
                this._InventoryPriceClassID = value;
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
        #region OwnerID
        public abstract class ownerID : PX.Data.IBqlField
        {
        }
        protected Guid? _OwnerID;
        [PXDBGuid]
        [PXUIField(DisplayName = "Price Manager")]
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
        #region WorkGroupID
        public abstract class workGroupID : PX.Data.IBqlField
        {
        }
        protected Int32? _WorkGroupID;
        [PXDBInt]
        [PXUIField(DisplayName = "Price Workgroup")]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID,
            Where<EPCompanyTree.workGroupID, Owned<Current<AccessInfo.userID>>>>),
         SubstituteKey = typeof(EPCompanyTree.description))]
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
        #region FilterSet
        public abstract class filterSet : PX.Data.IBqlField
        {
        }
        [PXDefault(false)]
        [PXDBBool]
        public virtual Boolean? FilterSet
        {
            get
            {
                return
                    this.OwnerID != null ||
                    this.WorkGroupID != null ||
                    this.MyWorkGroup == true;
            }
        }
        #endregion
    }
}
