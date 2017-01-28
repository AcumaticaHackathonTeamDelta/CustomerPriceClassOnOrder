using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.TM;
using PX.Objects.PO;

namespace PX.Objects.AP
{
	public class APVendorPriceMaint : PXGraph<APVendorPriceMaint>//, PXImportAttribute.IPXPrepareItems
	{
		#region DAC overrides
		#region APVendorPrice
		#region VendorID
		[Vendor]
		[PXDefault(typeof(APVendorPriceFilter.vendorID))]
		[PXParent(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<APVendorPrice.vendorID>>>>))]
		public virtual void APVendorPrice_VendorID_CacheAttached(PXCache sender) { }
		#endregion
		#region InventoryID
		[Inventory(DisplayName = "Inventory ID")]
		[PXDefault(typeof(APVendorPriceFilter.inventoryID))]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<APVendorPrice.inventoryID>>>>))]
		public virtual void APVendorPrice_InventoryID_CacheAttached(PXCache sender) { }
		#endregion
		#endregion
		#endregion
		#region Selects/Views

		public PXSave<APVendorPriceFilter> Save;
		public PXCancel<APVendorPriceFilter> Cancel;

		public PXFilter<APVendorPriceFilter> Filter;

		[PXFilterable]
		public PXSelectJoin<APVendorPrice,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<APVendorPrice.inventoryID>>,
				LeftJoin<Vendor, On<APVendorPrice.vendorID, Equal<Vendor.bAccountID>>>>,
				Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
				And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
				And2<Where<APVendorPrice.vendorID, Equal<Current<APVendorPriceFilter.vendorID>>, Or<Current<APVendorPriceFilter.vendorID>, IsNull>>, 
				And2<Where<APVendorPrice.inventoryID, Equal<Current<APVendorPriceFilter.inventoryID>>, Or<Current<APVendorPriceFilter.inventoryID>, IsNull>>,
                And2<Where2<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Current<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.effectiveDate, IsNull>>, 
                And<Where<APVendorPrice.expirationDate, GreaterEqual<Current<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.expirationDate, IsNull>>>>,
                Or<Current<APVendorPriceFilter.effectiveAsOfDate>, IsNull>>,
				And<Where2<Where<Current<APVendorPriceFilter.itemClassID>, IsNull,
						Or<Current<APVendorPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
					And2<Where<Current<APVendorPriceFilter.ownerID>, IsNull,
						Or<Current<APVendorPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
					And2<Where<Current<APVendorPriceFilter.myWorkGroup>, Equal<boolFalse>,
						 Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<APVendorPriceFilter.currentOwnerID>>>>,
					And<Where<Current<APVendorPriceFilter.workGroupID>, IsNull,
						Or<Current<APVendorPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>,
				OrderBy<Asc<APVendorPrice.inventoryID,
					Asc<APVendorPrice.uOM, Asc<APVendorPrice.breakQty, Asc<APVendorPrice.effectiveDate>>>>>> Records;

		public PXSetup<Company> Company;
		#endregion

		#region Ctor
		public APVendorPriceMaint() 
		{
			FieldDefaulting.AddHandler<CR.BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = CR.BAccountType.VendorType; });
		}
		#endregion

        public PXAction<APVendorPriceFilter> createWorksheet;
        [PXUIField(DisplayName = AR.Messages.CreatePriceWorksheet, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable CreateWorksheet(PXAdapter adapter)
        {
            if (Filter.Current != null)
            {
                this.Save.Press();
                APPriceWorksheetMaint graph = PXGraph.CreateInstance<APPriceWorksheetMaint>();
                APPriceWorksheet worksheet = new APPriceWorksheet();
                graph.Document.Insert(worksheet);
				int startRow = PXView.StartRow;
				int totalRows = 0;

                List<Object> allVendorPrices = new PXView(this, false, PXSelectJoin<APVendorPrice,
                                LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<APVendorPrice.inventoryID>>,
                                LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>>,
                            Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
                                And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
                                And2<Where<APVendorPrice.vendorID, Equal<Required<APVendorPriceFilter.vendorID>>, Or<Required<APVendorPriceFilter.vendorID>, IsNull>>,
                                And2<Where<APVendorPrice.inventoryID, Equal<Required<APVendorPriceFilter.inventoryID>>, Or<Required<APVendorPriceFilter.inventoryID>, IsNull>>,
                                And2<Where2<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.effectiveDate, IsNull>>,
                                And<Where<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.expirationDate, IsNull>>>>,
                                Or<Required<APVendorPriceFilter.effectiveAsOfDate>, IsNull>>,
                                And<Where2<Where<Required<APVendorPriceFilter.itemClassID>, IsNull,
                                    Or<Required<APVendorPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
                                And2<Where<Required<APVendorPriceFilter.ownerID>, IsNull,
                                    Or<Required<APVendorPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
                                And2<Where<Required<APVendorPriceFilter.myWorkGroup>, Equal<False>,
                                    Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<APVendorPriceFilter.currentOwnerID>>>>,
                                And<Where<Required<APVendorPriceFilter.workGroupID>, IsNull,
                                    Or<Required<APVendorPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>,
                            OrderBy<Asc<APVendorPrice.inventoryID,
                                    Asc<APVendorPrice.uOM,
                                    Asc<APVendorPrice.breakQty,
                                    Desc<APVendorPrice.effectiveDate>>>>>>.GetCommand()).Select(PXView.Currents, new object[]{ Filter.Current.VendorID, Filter.Current.VendorID, 
                                    Filter.Current.InventoryID, Filter.Current.InventoryID, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate,
                                    Filter.Current.ItemClassID, Filter.Current.ItemClassID, Filter.Current.OwnerID, Filter.Current.OwnerID, 
                                    Filter.Current.MyWorkGroup, Filter.Current.WorkGroupID, Filter.Current.WorkGroupID},
                                    PXView.Searches, PXView.SortColumns, PXView.Descendings, Records.View.GetExternalFilters(), ref startRow, PXView.MaximumRows, ref totalRows);

                List<Object> groupedVendorPrices = new PXView(this, false, PXSelectJoinGroupBy<APVendorPrice,
                                LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<APVendorPrice.inventoryID>>,
                                LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>>,
                            Where<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
                                And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
                                And2<Where<APVendorPrice.vendorID, Equal<Required<APVendorPriceFilter.vendorID>>, Or<Required<APVendorPriceFilter.vendorID>, IsNull>>,
                                And2<Where<APVendorPrice.inventoryID, Equal<Required<APVendorPriceFilter.inventoryID>>, Or<Required<APVendorPriceFilter.inventoryID>, IsNull>>,
                                And2<Where2<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.effectiveDate, IsNull>>,
                                And<Where<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPriceFilter.effectiveAsOfDate>>, Or<APVendorPrice.expirationDate, IsNull>>>>,
                                Or<Required<APVendorPriceFilter.effectiveAsOfDate>, IsNull>>,
                                And<Where2<Where<Required<APVendorPriceFilter.itemClassID>, IsNull,
                                    Or<Required<APVendorPriceFilter.itemClassID>, Equal<InventoryItem.itemClassID>>>,
                                And2<Where<Required<APVendorPriceFilter.ownerID>, IsNull,
                                    Or<Required<APVendorPriceFilter.ownerID>, Equal<InventoryItem.priceManagerID>>>,
                                And2<Where<Required<APVendorPriceFilter.myWorkGroup>, Equal<False>,
                                    Or<InventoryItem.priceWorkgroupID, InMember<CurrentValue<APVendorPriceFilter.currentOwnerID>>>>,
                                And<Where<Required<APVendorPriceFilter.workGroupID>, IsNull,
                                    Or<Required<APVendorPriceFilter.workGroupID>, Equal<InventoryItem.priceWorkgroupID>>>>>>>>>>>>>,
                            Aggregate<GroupBy<APVendorPrice.vendorID,
                                    GroupBy<APVendorPrice.inventoryID,
                                    GroupBy<APVendorPrice.uOM,
                                    GroupBy<APVendorPrice.breakQty,
                                    GroupBy<APVendorPrice.curyID>>>>>>,
                            OrderBy<Asc<APVendorPrice.inventoryID,
                                    Asc<APVendorPrice.uOM,
                                    Asc<APVendorPrice.breakQty,
                                    Desc<APVendorPrice.effectiveDate>>>>>>.GetCommand()).Select(PXView.Currents, new object[]{ Filter.Current.VendorID, Filter.Current.VendorID, 
                                    Filter.Current.InventoryID, Filter.Current.InventoryID, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate, Filter.Current.EffectiveAsOfDate,
                                    Filter.Current.ItemClassID, Filter.Current.ItemClassID, Filter.Current.OwnerID, Filter.Current.OwnerID, 
                                    Filter.Current.MyWorkGroup, Filter.Current.WorkGroupID, Filter.Current.WorkGroupID},
                                    PXView.Searches, PXView.SortColumns, PXView.Descendings, Records.View.GetExternalFilters(), ref startRow, PXView.MaximumRows, ref totalRows);

                if (allVendorPrices.Count > groupedVendorPrices.Count)
                {
                    throw new PXException(AR.Messages.MultiplePriceRecords);
                }

                foreach (PXResult<APVendorPrice> res in groupedVendorPrices)
                {
                    APVendorPrice price = (APVendorPrice)res;
                    APPriceWorksheetDetail detail = new APPriceWorksheetDetail();
                    detail.RefNbr = graph.Document.Current.RefNbr;
                    detail.VendorID = price.VendorID;
                    detail = graph.Details.Insert(detail);
                    detail.InventoryID = price.InventoryID;
                    detail.UOM = price.UOM;
                    detail.BreakQty = price.BreakQty;
                    detail.CurrentPrice = price.SalesPrice;
                    detail.CuryID = price.CuryID;
                    graph.Details.Update(detail);
                }

                throw new PXRedirectRequiredException(graph, AR.Messages.CreatePriceWorksheet);
            }
            return adapter.Get();
        }


		public virtual void APVendorPriceFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(TM.OwnedFilter.ownerID).Name, e.Row == null || (bool?)sender.GetValue(e.Row, typeof(TM.OwnedFilter.myOwner).Name) == false);
			PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(TM.OwnedFilter.workGroupID).Name, e.Row == null || (bool?)sender.GetValue(e.Row, typeof(TM.OwnedFilter.myWorkGroup).Name) == false);
		}

		//protected virtual void APVendorPrice_EffectiveDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		//{
		//	APVendorPrice price = (APVendorPrice)e.Row;
		//	if (price.IsPromotionalPrice != true)
		//	{
		//		APVendorPrice lastPrice = FindLastPrice(this, price);
		//		if (lastPrice != null && price.EffectiveDate > lastPrice.EffectiveDate && lastPrice.ExpirationDate == null)
		//		{
		//			lastPrice.ExpirationDate = price.EffectiveDate;
		//			Records.Update(lastPrice);
		//			Records.View.RequestRefresh();
		//		}
		//	}
		//}

		protected virtual void APVendorPrice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APVendorPrice row = (APVendorPrice)e.Row;
			if (row.IsPromotionalPrice == true && row.ExpirationDate == null)
			{
				sender.RaiseExceptionHandling<APVendorPrice.expirationDate>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APVendorPrice.expirationDate).Name));
			}
            if (row.IsPromotionalPrice == true && row.EffectiveDate == null)
            {
                sender.RaiseExceptionHandling<APVendorPrice.effectiveDate>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APVendorPrice.effectiveDate).Name));
            }
			if (row.ExpirationDate < row.EffectiveDate)
			{
				sender.RaiseExceptionHandling<APVendorPrice.effectiveDate>(row, row.ExpirationDate, new PXSetPropertyException(AR.Messages.EffectiveDateExpirationDate, PXErrorLevel.RowError));
			}
		}

		protected virtual void APVendorPrice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			APVendorPrice row = e.Row as APVendorPrice;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<APVendorPrice.vendorID>(sender, row, Filter.Current.VendorID == null);
				PXUIFieldAttribute.SetEnabled<APVendorPrice.inventoryID>(sender, row, Filter.Current.InventoryID == null);
			}
		}

		public override void Persist()
		{
			foreach (APVendorPrice price in Records.Cache.Inserted)
			{
				APVendorPrice lastPrice = FindLastPrice(this, price);
				if (lastPrice != null)
				{
					if (lastPrice.EffectiveDate > price.EffectiveDate && price.ExpirationDate == null)
					{
						Records.Cache.RaiseExceptionHandling<APVendorPrice.expirationDate>(price, price.ExpirationDate, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<APVendorPrice.expirationDate>(Records.Cache)));
						throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<APVendorPrice.expirationDate>(Records.Cache));
					}
				}
				ValidateDuplicate(this, Records.Cache, price);
			}
			foreach (APVendorPrice price in Records.Cache.Updated)
			{
				APVendorPrice lastPrice = FindLastPrice(this, price);
				if (lastPrice != null)
				{
					if (lastPrice.EffectiveDate > price.EffectiveDate && price.ExpirationDate == null)
					{
						Records.Cache.RaiseExceptionHandling<APVendorPrice.expirationDate>(price, price.ExpirationDate, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<APVendorPrice.expirationDate>(Records.Cache)));
						throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<APVendorPrice.expirationDate>(Records.Cache));
					}
				}
				ValidateDuplicate(this, Records.Cache, price);
			}
			base.Persist();
			Records.Cache.Clear();
		}

		public static void ValidateDuplicate(PXGraph graph, PXCache sender, APVendorPrice price)
		{
			PXSelectBase<APVendorPrice> selectDuplicate = new PXSelect<APVendorPrice, Where<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
																And<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
																And<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
																And<APVendorPrice.isPromotionalPrice, Equal<Required<APVendorPrice.isPromotionalPrice>>,
																And<APVendorPrice.breakQty, Equal<Required<APVendorPrice.breakQty>>,
																And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,
																And<APVendorPrice.recordID, NotEqual<Required<APVendorPrice.recordID>>>>>>>>>>(graph);
            foreach (APVendorPrice apPrice in selectDuplicate.Select(price.VendorID, price.InventoryID, price.UOM, price.IsPromotionalPrice, price.BreakQty, price.CuryID, price.RecordID))
            {
                if (IsOverlapping(apPrice, price))
                {
                    sender.RaiseExceptionHandling<APVendorPrice.uOM>(price, price.UOM, new PXSetPropertyException(AR.Messages.DuplicateSalesPrice, PXErrorLevel.RowError, apPrice.SalesPrice, apPrice.EffectiveDate.HasValue ? apPrice.EffectiveDate.Value.ToShortDateString() : string.Empty, apPrice.ExpirationDate.HasValue ? apPrice.ExpirationDate.Value.ToShortDateString() : string.Empty));
					throw new PXSetPropertyException(AR.Messages.DuplicateSalesPrice, PXErrorLevel.RowError, apPrice.SalesPrice, apPrice.EffectiveDate.HasValue ? apPrice.EffectiveDate.Value.ToShortDateString() : string.Empty, apPrice.ExpirationDate.HasValue ? apPrice.ExpirationDate.Value.ToShortDateString() : string.Empty);
                }
            }
        }

        public static bool IsOverlapping(APVendorPrice vendorPrice1, APVendorPrice vendorPrice2)
        {
            return ((vendorPrice1.EffectiveDate != null && vendorPrice1.ExpirationDate != null && vendorPrice2.EffectiveDate != null && vendorPrice2.ExpirationDate != null && (vendorPrice1.EffectiveDate <= vendorPrice2.EffectiveDate && vendorPrice1.ExpirationDate >= vendorPrice2.EffectiveDate || vendorPrice1.EffectiveDate <= vendorPrice2.ExpirationDate && vendorPrice1.ExpirationDate >= vendorPrice2.ExpirationDate || vendorPrice1.EffectiveDate >= vendorPrice2.EffectiveDate && vendorPrice1.EffectiveDate <= vendorPrice2.ExpirationDate))
                        || (vendorPrice1.ExpirationDate != null && vendorPrice2.EffectiveDate != null && (vendorPrice2.ExpirationDate == null || vendorPrice1.EffectiveDate == null) && vendorPrice2.EffectiveDate <= vendorPrice1.ExpirationDate)
                        || (vendorPrice1.EffectiveDate != null && vendorPrice2.ExpirationDate != null && (vendorPrice2.EffectiveDate == null || vendorPrice1.ExpirationDate == null) && vendorPrice2.ExpirationDate >= vendorPrice1.EffectiveDate)
                        || (vendorPrice1.EffectiveDate != null && vendorPrice2.EffectiveDate != null && vendorPrice1.ExpirationDate == null && vendorPrice2.ExpirationDate == null)
                        || (vendorPrice1.ExpirationDate != null && vendorPrice2.ExpirationDate != null && vendorPrice1.EffectiveDate == null && vendorPrice2.EffectiveDate == null)
                        || (vendorPrice1.EffectiveDate == null && vendorPrice1.ExpirationDate == null)
                        || (vendorPrice2.EffectiveDate == null && vendorPrice2.ExpirationDate == null));
        }

        #region Cost Calculation

        /// <summary>
        /// Calculates Unit Cost.
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="inventoryID">Inventory</param>
        /// <param name="curyID">Currency</param>
        /// <param name="UOM">Unit of measure</param>
        /// <param name="date">Date</param>
        /// <returns>Unit Cost</returns>
        public static decimal? CalculateUnitCost(PXCache sender, int? vendorID, int? vendorLocationID, int? inventoryID, CurrencyInfo currencyinfo, string UOM, decimal? quantity, DateTime date, decimal? currentUnitCost, bool alwaysFromBaseCurrency = false)
        {
            InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
            UnitCostItem ucItem = FindUnitCost(sender, vendorID, vendorLocationID, inventoryID, currencyinfo.BaseCuryID, alwaysFromBaseCurrency ? currencyinfo.BaseCuryID : currencyinfo.CuryID, Math.Abs(quantity ?? 0m), UOM, date);
			return AdjustUnitCost(sender, ucItem, inventoryID, currencyinfo, UOM, currentUnitCost);
        }

		/// <summary>
		/// Calculates Unit Cost in a given currency only.
		/// </summary>
		/// <param name="sender">Cache</param>
		/// <param name="inventoryID">Inventory</param>
		/// <param name="curyID">Currency</param>
		/// <param name="UOM">Unit of measure</param>
		/// <param name="date">Date</param>
		/// <returns>Unit Cost</returns>
		public static decimal? CalculateCuryUnitCost(PXCache sender, int? vendorID, int? vendorLocationID, int? inventoryID, string curyID, string UOM, decimal? quantity, DateTime date, decimal? currentUnitCost)
		{
			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
			UnitCostItem ucItem = FindUnitCost(sender, vendorID, vendorLocationID, inventoryID, curyID, curyID, Math.Abs(quantity ?? 0m), UOM, date);
			return AdjustUnitCost(sender, ucItem, inventoryID, null, UOM, currentUnitCost);
		}

		public static decimal? AdjustUnitCost(PXCache sender, UnitCostItem ucItem, int? inventoryID, CurrencyInfo currencyinfo, string UOM, decimal? currentUnitCost)
		{
			if (ucItem != null)
			{
				decimal unitCost = ucItem.Cost;

				if (currencyinfo != null && ucItem.CuryID != currencyinfo.CuryID)
				{
					PXCurrencyAttribute.CuryConvCury(sender, currencyinfo, ucItem.Cost, out unitCost);
				}

				if (UOM == null)
				{
					return null;
				}

				if (ucItem.UOM != UOM)
				{
					decimal salesPriceInBase = INUnitAttribute.ConvertFromBase(sender, inventoryID, ucItem.UOM, unitCost, INPrecision.UNITCOST);
					unitCost = INUnitAttribute.ConvertToBase(sender, inventoryID, UOM, salesPriceInBase, INPrecision.UNITCOST);
				}

				if (unitCost == 0m && currentUnitCost != null && currentUnitCost != 0m)
					return currentUnitCost;
				else
					return unitCost;
			}
			return null;
		}

        public static UnitCostItem FindUnitCost(PXCache sender, int? inventoryID, string curyID, string UOM, DateTime date)
        {
            return FindUnitCost(sender, null, null, inventoryID, curyID, curyID, 0m, UOM, date);
        }

        public static UnitCostItem FindUnitCost(PXCache sender, int? vendorID, int? vendorLocationID, int? inventoryID, string baseCuryID, string curyID, decimal? quantity, string UOM, DateTime date)
        {
            PXSelectBase<APVendorPrice> unitCost = new PXSelect<APVendorPrice, Where<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
            And<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
            And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,
            And<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
            
			And<Where2<Where<APVendorPrice.breakQty, LessEqual<Required<APVendorPrice.breakQty>>>,
			And<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>,
					 And<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPrice.expirationDate>>>>,
			Or2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>,
			And<APVendorPrice.expirationDate, IsNull>>,
					 Or<Where<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPrice.expirationDate>>,
			And<APVendorPrice.effectiveDate, IsNull,
            Or<APVendorPrice.effectiveDate, IsNull, And<APVendorPrice.expirationDate, IsNull>>>>>>>>>>>>>>,

            OrderBy<Desc<APVendorPrice.isPromotionalPrice, Desc<APVendorPrice.vendorID, Desc<APVendorPrice.breakQty>>>>>(sender.Graph);

            PXSelectBase<APVendorPrice> unitCostBaseUOM = new PXSelectJoin<APVendorPrice, InnerJoin<InventoryItem, On<InventoryItem.inventoryID, 
                Equal<APVendorPrice.inventoryID>, And<InventoryItem.baseUnit, Equal<APVendorPrice.uOM>>>>,
                Where<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
            And<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
            And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>,

					 And<Where2<Where<APVendorPrice.breakQty, LessEqual<Required<APVendorPrice.breakQty>>>,
					 And<Where2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>,
					 And<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPrice.expirationDate>>>>,
					 Or2<Where<APVendorPrice.effectiveDate, LessEqual<Required<APVendorPrice.effectiveDate>>,
					 And<APVendorPrice.expirationDate, IsNull>>,
					 Or<Where<APVendorPrice.expirationDate, GreaterEqual<Required<APVendorPrice.expirationDate>>,
					 And<APVendorPrice.effectiveDate, IsNull,
                     Or<APVendorPrice.effectiveDate, IsNull, And<APVendorPrice.expirationDate, IsNull>>>>>>>>>>>>>,

            OrderBy<Desc<APVendorPrice.isPromotionalPrice, Desc<APVendorPrice.vendorID, Desc<APVendorPrice.breakQty>>>>>(sender.Graph);

            APVendorPrice item = unitCost.SelectWindowed(0, 1, inventoryID, vendorID, curyID, UOM, quantity, date, date, date, date);

            string uomFound = null;

            if (item == null)
            {
                decimal baseUnitQty = INUnitAttribute.ConvertToBase(sender, inventoryID, UOM, (decimal)quantity, INPrecision.QUANTITY);
                item = unitCostBaseUOM.Select(inventoryID, vendorID, curyID, baseUnitQty, date, date, date, date);

                if (item == null)
                {
                    /*InventoryItem inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
                    if (inventoryItem != null && inventoryItem.BasePrice != null)
                    {
                        return new UnitCostItem(inventoryItem.BaseUnit, (inventoryItem.BasePrice ?? 0m), baseCuryID);
                    }
                    else*/
                        return null;
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

            if (item == null)
            {
                return null;
            }

            return new UnitCostItem(uomFound, (item.SalesPrice ?? 0), item.CuryID);
        }

        public class UnitCostItem
        {
            private string uom;

            public string UOM
            {
                get { return uom; }
            }

            private decimal cost;

            public decimal Cost
            {
                get { return cost; }
            }

            private string curyid;
            public string CuryID
            {
                get { return curyid; }
            }

            public UnitCostItem(string uom, decimal cost, string curyid)
            {
                this.uom = uom;
                this.cost = cost;
                this.curyid = curyid;
            }

        }

        #endregion

		public static APVendorPrice FindLastPrice(PXGraph graph, APVendorPrice price)
		{
			APVendorPrice lastPrice = new PXSelect<APVendorPrice, Where<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>, And<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>, And<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>, And<APVendorPrice.isPromotionalPrice, Equal<Required<APVendorPrice.isPromotionalPrice>>, And<APVendorPrice.breakQty, Equal<Required<APVendorPrice.breakQty>>, And<APVendorPrice.curyID, Equal<Required<APVendorPrice.curyID>>, And<APVendorPrice.recordID, NotEqual<Required<APVendorPrice.recordID>>>>>>>>>, OrderBy<Desc<APVendorPrice.effectiveDate>>>(graph).SelectSingle(price.VendorID, price.InventoryID, price.UOM, price.IsPromotionalPrice, price.BreakQty, price.CuryID, price.RecordID);
			return lastPrice;
		}
	}

	[Serializable]
	public partial class APVendorPriceFilter : IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXUIField(DisplayName = "Vendor")]
		[VendorNonEmployeeActive()]
		[PXParent(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<APVendorPriceFilter.vendorID>>>>))]
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
		[PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXUIField(DisplayName = "Product Workgroup")]
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
	}
}
