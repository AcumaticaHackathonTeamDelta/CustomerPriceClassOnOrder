using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AR;
using System.Diagnostics;
using PX.Objects.CM;
using PX.Objects.GL;

namespace PX.Objects.SO
{
    public class DiscountInquiry : PXGraph<DiscountInquiry>
    {
        #region Selects/Views
        public PXFilter<DiscountFilter> Filter;
        [PXFilterable]
        public PXSelect<DiscountInquiryDetails> Document;
        public PXSave<DiscountInquiryDetails> Save;
        public PXCancel<DiscountInquiryDetails> Cancel;
        //public PXAction<DiscountInquiryDetails> SearchDiscount;
        //public PXAction<DiscountInquiryDetails> FindBestAvailable;
        public PXAction<DiscountInquiryDetails> FindBestDiscountJoin;
        #endregion

        public virtual IEnumerable document()
        {
            //Document.Cache.AllowInsert = false;
            //Document.Cache.AllowDelete = false;
            //Document.Cache.AllowUpdate = false;

            Dictionary<DiscountEntitiesI, List<DiscountDetailLine>> cachedDiscounts = PXDatabase.GetSlot<Dictionary<DiscountEntitiesI, List<DiscountDetailLine>>>(typeof(DiscountDetailLine).Name + typeof(DiscountEntitiesI).Name, typeof(Dictionary<DiscountEntitiesI, List<DiscountDetailLine>>));
            foreach (KeyValuePair<DiscountEntitiesI, List<DiscountDetailLine>> discount in cachedDiscounts)
            {
                foreach (DiscountDetailLine detail in discount.Value)
                {
                    DiscountInquiryDetails discountLine = new DiscountInquiryDetails();
                    if (detail.DiscountID != null)
                    {
                        discountLine.DiscountID = detail.DiscountID;
                        discountLine.DiscountSequenceID = detail.DiscountSequenceID;
                        discountLine.Type = detail.Type;
                        //discountLine.ApplicableTo = GetUserFriendlyApplicable(detail.ApplicableToStr);
                        discountLine.EntityID = discount.Key.EntityID;
                        discountLine.EntityType = discount.Key.EntityType;
                        discountLine.BreakBy = detail.BreakBy;
                        discountLine.DiscountedFor = detail.DiscountedFor;

                        if (discountLine.BreakBy == "A")
                        {
                            discountLine.AmountFrom = detail.AmountFrom;
                            discountLine.AmountTo = detail.AmountTo;
                        }
                        else
                        {
                            discountLine.QuantityFrom = detail.AmountFrom;
                            discountLine.QuantityTo = detail.AmountTo;
                        }

                        if (discountLine.DiscountedFor == "A")
                        {
                            discountLine.Discount = detail.Discount;
                            discountLine.DiscountAmount = detail.Discount;
                        }
                        else
                        {
                            discountLine.DiscountPercent = detail.Discount;
                            discountLine.DiscountAmount = Filter.Current.Amount / 100 * detail.Discount;
                        }
                    }
                    //discountLine.BestDiscount = detail.IsBest;

                    yield return discountLine;
                }
            }

            //string. Test only. 
            Dictionary<DiscountEntitiesS, List<DiscountDetailLine>> cachedDiscountsS = PXDatabase.GetSlot<Dictionary<DiscountEntitiesS, List<DiscountDetailLine>>>(typeof(DiscountDetailLine).Name + typeof(DiscountEntitiesS).Name, typeof(Dictionary<DiscountEntitiesS, List<DiscountDetailLine>>));
            foreach (KeyValuePair<DiscountEntitiesS, List<DiscountDetailLine>> discount in cachedDiscountsS)
            {
                foreach (DiscountDetailLine detail in discount.Value)
                {
                    DiscountInquiryDetails discountLine = new DiscountInquiryDetails();
                    if (detail.DiscountID != null)
                    {
                        discountLine.DiscountID = detail.DiscountID;
                        discountLine.DiscountSequenceID = detail.DiscountSequenceID;
                        discountLine.Type = detail.Type;
                        //discountLine.ApplicableTo = GetUserFriendlyApplicable(detail.ApplicableToStr);
                        //discountLine.EntityID = discount.Key.EntityID;
                        discountLine.EntityType = discount.Key.EntityType;
                        discountLine.BreakBy = detail.BreakBy;
                        discountLine.DiscountedFor = detail.DiscountedFor;

                        if (discountLine.BreakBy == "A")
                        {
                            discountLine.AmountFrom = detail.AmountFrom;
                            discountLine.AmountTo = detail.AmountTo;
                        }
                        else
                        {
                            discountLine.QuantityFrom = detail.AmountFrom;
                            discountLine.QuantityTo = detail.AmountTo;
                        }

                        if (discountLine.DiscountedFor == "A")
                        {
                            discountLine.Discount = detail.Discount;
                            discountLine.DiscountAmount = detail.Discount;
                        }
                        else
                        {
                            discountLine.DiscountPercent = detail.Discount;
                            discountLine.DiscountAmount = Filter.Current.Amount / 100 * detail.Discount;
                        }
                    }
                    //discountLine.BestDiscount = detail.IsBest;

                    yield return discountLine;
                }
            }

        }

        private string GetUserFriendlyApplicable(string applicableTo)
        {
            switch (applicableTo)
            {
                case DiscountTarget.Customer:
                    {
                        return Messages.Customer;
                    }
                case DiscountTarget.Inventory:
                    {
                        return Messages.Discount_Inventory;
                    }
                case DiscountTarget.InventoryPrice:
                    {
                        return Messages.InventoryPrice;
                    }
                case DiscountTarget.CustomerAndInventory:
                    {
                        return Messages.CustomerAndInventory;
                    }
                case DiscountTarget.CustomerAndInventoryPrice:
                    {
                        return Messages.CustomerAndInventoryPrice;
                    }
                case DiscountTarget.CustomerPrice:
                    {
                        return Messages.CustomerPrice;
                    }
                case DiscountTarget.CustomerPriceAndInventory:
                    {
                        return Messages.CustomerPriceAndInventory;
                    }
                case DiscountTarget.CustomerPriceAndInventoryPrice:
                    {
                        return Messages.CustomerPriceAndInventoryPrice;
                    }
                case DiscountTarget.Warehouse:
                    {
                        return Messages.Warehouse;
                    }
                case DiscountTarget.WarehouseAndInventory:
                    {
                        return Messages.WarehouseAndInventory;
                    }
                case DiscountTarget.WarehouseAndCustomer:
                    {
                        return Messages.WarehouseAndCustomer;
                    }
                case DiscountTarget.WarehouseAndInventoryPrice:
                    {
                        return Messages.WarehouseAndInventoryPrice;
                    }
                case DiscountTarget.WarehouseAndCustomerPrice:
                    {
                        return Messages.WarehouseAndCustomerPrice;
                    }
                case DiscountTarget.Branch:
                    {
                        return Messages.Branch;
                    }
                case DiscountTarget.Vendor:
                    {
                        return AP.Messages.VendorUnconditional;
                    }
                case DiscountTarget.VendorLocation:
                    {
                        return AP.Messages.Vendor_Location;
                    }
                case DiscountTarget.VendorLocationaAndInventory:
                    {
                        return AP.Messages.VendorLocationaAndInventory;
                    }
                case DiscountTarget.Unconditional:
                    {
                        return Messages.Unconditional;
                    }
            }
            return "Unknown";
        }

        public virtual void DiscountFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            DiscountFilter row = (DiscountFilter)e.Row;
            if (row == null) return;

            //ApplicableToEnum fieldsToShow = DiscountEngine.SetApplicableToEnum(row.ApplicableTo);
            //PXUIFieldAttribute.SetVisible<DiscountFilter.inventoryID>(Filter.Cache, null, (fieldsToShow & ApplicableToEnum.InventoryItem) != ApplicableToEnum.None);
            //PXUIFieldAttribute.SetVisible<DiscountFilter.customerID>(Filter.Cache, null, (fieldsToShow & ApplicableToEnum.Customer) != ApplicableToEnum.None);
            //PXUIFieldAttribute.SetVisible<DiscountFilter.customerPriceClassID>(Filter.Cache, null, (fieldsToShow & ApplicableToEnum.CustomerPriceClass) != ApplicableToEnum.None);
            //PXUIFieldAttribute.SetVisible<DiscountFilter.inventoryPriceClassID>(Filter.Cache, null, (fieldsToShow & ApplicableToEnum.InventoryPriceClass) != ApplicableToEnum.None);

        }

        protected virtual void DiscountFilter_ApplicableTo_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            DiscountFilter row = e.Row as DiscountFilter;
            if (row != null)
            {
                if (row.Type == DiscountType.Document)
                {                     
                    e.ReturnState = PXStringState.CreateInstance(e.ReturnValue, 1, false, "ApplicableTo", false, 1, null,
                                                new string[] { DiscountTarget.Customer, DiscountTarget.CustomerAndBranch, DiscountTarget.CustomerPrice, DiscountTarget.CustomerPriceAndBranch, DiscountTarget.Unconditional },
                                                new string[] { Messages.Customer, Messages.CustomerAndBranch, Messages.CustomerPrice, Messages.CustomerPriceAndBranch, Messages.Unconditional }.Select(l => PXMessages.LocalizeNoPrefix(l)).ToArray(),
                                                true, DiscountTarget.Customer);

                    return;
                }

            }

            e.ReturnState = PXStringState.CreateInstance(e.ReturnValue, 1, false, "ApplicableTo", false, 1, null,
                                                new string[] { DiscountTarget.Customer, DiscountTarget.Inventory, DiscountTarget.InventoryPrice, DiscountTarget.CustomerAndInventory, DiscountTarget.CustomerAndInventoryPrice, 
                        DiscountTarget.CustomerPrice, DiscountTarget.CustomerPriceAndInventory, DiscountTarget.CustomerPriceAndInventoryPrice, DiscountTarget.Warehouse, DiscountTarget.WarehouseAndInventory, 
                        DiscountTarget.WarehouseAndCustomer, DiscountTarget.WarehouseAndInventoryPrice, DiscountTarget.WarehouseAndCustomerPrice, DiscountTarget.Branch, 
                        DiscountTarget.Unconditional },
                                                new string[] { Messages.Customer, Messages.Discount_Inventory, Messages.InventoryPrice, Messages.CustomerAndInventory, Messages.CustomerAndInventoryPrice, 
                        Messages.CustomerPrice, Messages.CustomerPriceAndInventory, Messages.CustomerPriceAndInventoryPrice, Messages.Warehouse, Messages.WarehouseAndInventory, 
                        Messages.WarehouseAndCustomer, Messages.WarehouseAndInventoryPrice, Messages.WarehouseAndCustomerPrice, Messages.Branch, 
                        Messages.Unconditional }.Select(l => PXMessages.LocalizeNoPrefix(l)).ToArray(),
                                                true, DiscountTarget.Inventory);
        }

        /*[PXButton]
        [PXUIField(DisplayName = "Find Applicable Discounts", MapEnableRights = PXCacheRights.Update)]
        public virtual IEnumerable searchDiscount(PXAdapter adapter)
        {
            string before = typeof(SODiscountCustomer.discountID).Name;
            String after = before.Substring(0, 1).ToUpper() + before.Substring(1);

            //string sd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeof(SODiscountCustomer.discountID).Name);

            //PXResultset<SODiscountS> sDisc = PXSelect<SODiscountS>.Select(this);

            //BqlCommand sdiscountTst = BqlCommand.CreateInstance(typeof(Select<SODiscountS>));
            //IEnumerable dbSelect2 = PXDatabase.Select(this, sdiscountTst, 1000);



            BqlCommand bql = BqlCommand.CreateInstance(typeof(Select<>), typeof(SODiscount));
            //var bqlJoin = PXSelectJoin<SODiscount,
            //    InnerJoin<SODiscountSequence, On<SODiscount.discountID, Equal<SODiscountSequence.discountID>>,
            //InnerJoin<SODiscountDetail, On<SODiscountDetail.discountID, Equal<SODiscountSequence.discountID>,
            //And<SODiscountDetail.discountSequenceID, Equal<SODiscountSequence.discountSequenceID>>>>>>.Select(this);

            //Collecting all discount codes
            IEnumerable discountTypes = PXDatabase.SelectMulti<SODiscount>(
                new PXDataField<SODiscount.discountID>(),
                new PXDataField<SODiscount.type>(),
                new PXDataField<SODiscount.applicableTo>());

            List<DiscountTypeT> cachedDiscountTypes = PXDatabase.GetSlot<List<DiscountTypeT>>(typeof(DiscountTypeT).Name, typeof(List<DiscountTypeT>));
            cachedDiscountTypes.Clear();
            foreach (PXDataRecord discountType in discountTypes)
            {
                DiscountTypeT type = new DiscountTypeT();
                type.DiscountID = discountType.GetString(0);
                type.Type = discountType.GetString(1);
                type.ApplicableToStr = discountType.GetString(2);
                cachedDiscountTypes.Add(type);
            }

            #region Entities

            //BqlCommand discountCustomer = BqlCommand.CreateInstance(typeof(Select2<SODiscountCustomer,
            //    InnerJoin<SODiscountSequence, On<SODiscountSequence.discountID, Equal<SODiscountCustomer.discountID>,
            //    And<SODiscountSequence.discountSequenceID, Equal<SODiscountCustomer.discountSequenceID>>>,
            //    InnerJoin<SODiscount, On<SODiscount.discountID, Equal<SODiscountCustomer.discountID>>,
            //    InnerJoin<SODiscountDetail, On<SODiscountDetail.discountID, Equal<SODiscountCustomer.discountID>,
            //    And<SODiscountDetail.discountSequenceID, Equal<SODiscountCustomer.discountSequenceID>>>>>>,
            //    Where<SODiscountCustomer.customerID, Equal<Current<DiscountFilter.customerID>>>>));
            //IEnumerable dbSelect2 = PXDatabase.Select(this, discountCustomer, 1000);


            //Discounts on current Customer
            List<DiscountEntity> discountEntitiesList = new List<DiscountEntity>();

            DiscountEngine.CollectEntities<SODiscountCustomer>(discountEntitiesList, typeof(SODiscountCustomer.customerID).Name, Filter.Current.CustomerID);
            DiscountEngine.CollectEntities<SODiscountItem>(discountEntitiesList, typeof(SODiscountItem.inventoryID).Name, Filter.Current.InventoryID);

            #endregion

            //Collecting all applicable discount sequences
            IEnumerable discountSequences = PXDatabase.SelectMulti<SODiscountSequence>(
              new PXDataField<SODiscountSequence.discountID>(),
              new PXDataField<SODiscountSequence.discountSequenceID>(),
              new PXDataField<SODiscountSequence.discountedFor>(),
              new PXDataField<SODiscountSequence.breakBy>(),
              new PXDataFieldValue<SODiscountSequence.isActive>(true));


            List<DiscountSequence> discountSequencesList = new List<DiscountSequence>();
            HashSet<DiscountSequence> hsString = new HashSet<DiscountSequence>();
            foreach (PXDataRecord discountSequence in discountSequences)
            {
                DiscountSequence sequence = new DiscountSequence();
                sequence.DiscountID = discountSequence.GetString(0);
                sequence.DiscountSequenceID = discountSequence.GetString(1);
                sequence.DiscountedFor = discountSequence.GetString(2);
                sequence.BreakBy = discountSequence.GetString(3);
                int dsSeq = cachedDiscountTypes.FindIndex(x => x.DiscountID == sequence.DiscountID);
                sequence.Type = cachedDiscountTypes[dsSeq].Type;
                sequence.ApplicableTo = cachedDiscountTypes[dsSeq].ApplicableToStr;
                int? idx = discountEntitiesList.FindIndex(x => x.DiscountSequenceID == sequence.DiscountSequenceID);
                if (idx >= 0)
                {
                    sequence.EntityID = discountEntitiesList[(int)idx].EntityID;
                    discountSequencesList.Add(sequence);
                }
            }

            //Collecting all applicable discount details
            IEnumerable discountDetails = PXDatabase.SelectMulti<SODiscountDetail>(
              new PXDataField<SODiscountDetail.discountID>(),
              new PXDataField<SODiscountDetail.discountSequenceID>(),
              new PXDataField<SODiscountDetail.amount>(),
              new PXDataField<SODiscountDetail.quantity>(),
              new PXDataField<SODiscountDetail.discount>());

            //List<DiscountDetail> discountDetailsList = new List<DiscountDetail>();
            List<SimplifiedDiscount> cachedDiscounts = PXDatabase.GetSlot<List<SimplifiedDiscount>>(typeof(SimplifiedDiscount).Name, typeof(List<SimplifiedDiscount>));
            cachedDiscounts.Clear();
            foreach (PXDataRecord discountDetail in discountDetails)
            {
                SimplifiedDiscount detail = new SimplifiedDiscount();
                detail.DiscountID = discountDetail.GetString(0);
                detail.DiscountSequenceID = discountDetail.GetString(1);

                //DiscountSequence ds = hsString.Select(x => x.DiscountSequenceID == detail.DiscountSequenceID);
                int idx = discountSequencesList.FindIndex(x => x.DiscountSequenceID == detail.DiscountSequenceID);
                if (idx >= 0)
                {
                    detail.Type = discountSequencesList[idx].Type;
                    detail.ApplicableTo = discountSequencesList[idx].ApplicableTo;
                    detail.BreakBy = discountSequencesList[idx].BreakBy;
                    detail.DiscountedFor = discountSequencesList[idx].DiscountedFor;
                    detail.EntityID = discountSequencesList[idx].EntityID;

                    detail.AmountFrom = discountDetail.GetDecimal(2);
                    detail.QuantityFrom = discountDetail.GetDecimal(3);
                    detail.DiscountPercent = discountDetail.GetDecimal(4);
                    //discountDetailsList.Add(detail);
                    cachedDiscounts.Add(detail);
                }
            }

            return adapter.Get();
        }*/

        /*[PXButton]
        [PXUIField(DisplayName = "Find Best Applicable", MapEnableRights = PXCacheRights.Update)]
        public virtual IEnumerable findBestAvailable(PXAdapter adapter)
        {
            //Collecting all discount codes
            DiscountEngine.GetDiscountTypes();

            string CustomerID = "CustomerID";
            string InventoryID = "InventoryID";

            if (Filter.Current.CustomerID != null)
                FindBestDiscount<SODiscountCustomer>(CustomerID, Filter.Current.CustomerID, "C", Filter.Current.Type);
            //CollectEntitytDiscounts<SODiscountCustomer>(CustomerID, Filter.Current.CustomerID, "C", Filter.Current.Type);
            if (Filter.Current.InventoryID != null)
                FindBestDiscount<SODiscountItem>(InventoryID, Filter.Current.InventoryID, "I", Filter.Current.Type);
            //CollectEntitytDiscounts<SODiscountItem>(InventoryID, Filter.Current.InventoryID, "I", Filter.Current.Type);
            return adapter.Get();
        }*/

        [PXButton]
        [PXUIField(DisplayName = "Find Best Discount", MapEnableRights = PXCacheRights.Update, Visible=true)]
        public virtual IEnumerable findBestDiscountJoin(PXAdapter adapter)
        {
            //Collecting all discount codes
            //DiscountEngine.CollectDiscountTypes();
            DiscountEngine.GetDiscountTypes();
           // FindBestAvailable.Press(); //- add to test
            //SelectBestEntitytDiscount((int)Filter.Current.CustomerID, Entities.Customer, "L", (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity);

            Dictionary<DiscountEntitiesI, List<DiscountDetailLine>> cachedDiscountsI = PXDatabase.GetSlot<Dictionary<DiscountEntitiesI, List<DiscountDetailLine>>>(typeof(DiscountDetailLine).Name + typeof(DiscountEntitiesI).Name, typeof(Dictionary<DiscountEntitiesI, List<DiscountDetailLine>>));
            Dictionary<DiscountEntitiesS, List<DiscountDetailLine>> cachedDiscountsS = PXDatabase.GetSlot<Dictionary<DiscountEntitiesS, List<DiscountDetailLine>>>(typeof(DiscountDetailLine).Name + typeof(DiscountEntitiesS).Name, typeof(Dictionary<DiscountEntitiesS, List<DiscountDetailLine>>));

            cachedDiscountsI.Clear();
            cachedDiscountsS.Clear();

            //DiscountEntitiesI entityI = new DiscountEntitiesI();
            DiscountEntitiesS entityS = new DiscountEntitiesS();
            List<DiscountDetailLine> foundDiscounts = new List<DiscountDetailLine>();

            DiscountEngine de = new DiscountEngine(); // to del!

            Dictionary<object, string> entites = new Dictionary<object, string>();

            if (Filter.Current.CustomerID != null) entites.Add(Filter.Current.CustomerID, DiscountTarget.Customer);
            if (Filter.Current.InventoryID != null) entites.Add(Filter.Current.InventoryID, DiscountTarget.Inventory);
            if (Filter.Current.CustomerPriceClassID != null) entites.Add(Filter.Current.CustomerPriceClassID, DiscountTarget.CustomerPrice);
            if (Filter.Current.InventoryPriceClassID != null) entites.Add(Filter.Current.InventoryPriceClassID, DiscountTarget.InventoryPrice);
            if (Filter.Current.BranchID != null) entites.Add(Filter.Current.BranchID, DiscountTarget.Branch);
            if (Filter.Current.SiteID != null) entites.Add(Filter.Current.SiteID, DiscountTarget.Warehouse);

            //entites.Add(271, DiscountTarget.Inventory);

            if (entites.Count != 0)
            {
                entityS.EntityID = "TEST";
                entityS.EntityType = "T";
                if (Filter.Current.Type == "A")
                {
                    foundDiscounts.Add(de.SelectBestDiscount(Filter.Cache, null, entites, "L", (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity, (DateTime)Filter.Current.DocDate));
                    foundDiscounts.Add(de.SelectApplicableDiscounts(Filter.Cache, null, de.SelectApplicableEntitytDiscounts(entites, "G"), Filter.Current.Amount, Filter.Current.Quantity, "G", (DateTime)Filter.Current.DocDate));
                    foundDiscounts.Add(de.SelectBestDiscount(Filter.Cache, null, entites, "D", (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity, (DateTime)Filter.Current.DocDate));
                }
                else
                {
                    foundDiscounts.Add(de.SelectBestDiscount(Filter.Cache, null, entites, Filter.Current.Type, (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity, (DateTime)Filter.Current.DocDate));
                }
                foreach (DiscountDetailLine discountDetail in foundDiscounts)
                {
                    if (cachedDiscountsS.ContainsKey(entityS))
                        cachedDiscountsS[entityS].Add(discountDetail);
                    else
                        cachedDiscountsS.Add(entityS, new List<DiscountDetailLine> { discountDetail });
                }
            }
            /*
            foreach (object t in new object[] { Filter.Current.CustomerID, Filter.Current.InventoryID, Filter.Current.CustomerPriceClassID, Filter.Current.InventoryPriceClassID })
            {
                if (Filter.Current.CustomerID != null)
                {
                    entityI.EntityID = (int)Filter.Current.CustomerID;
                    entityI.EntityType = Entities.Customer;
                    bestDiscount = de.SelectBestEntitytDiscount(Filter.Current.CustomerID, Entities.Customer, Filter.Current.Type, (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity);
                    if (cachedDiscountsI.ContainsKey(entityI))
                        cachedDiscountsI[entityI].Add(bestDiscount);
                    else
                        cachedDiscountsI.Add(entityI, new List<DiscountDetails> { bestDiscount });
                }
                if (Filter.Current.InventoryID != null)
                {
                    entityI.EntityID = (int)Filter.Current.InventoryID;
                    entityI.EntityType = Entities.Inventory;
                    bestDiscount = de.SelectBestEntitytDiscount(Filter.Current.InventoryID, Entities.Inventory, Filter.Current.Type, (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity);
                    if (cachedDiscountsI.ContainsKey(entityI))
                        cachedDiscountsI[entityI].Add(bestDiscount);
                    else
                        cachedDiscountsI.Add(entityI, new List<DiscountDetails> { bestDiscount });
                }
                if (Filter.Current.CustomerPriceClassID != null)
                {
                    entityS.EntityID = (string)Filter.Current.CustomerPriceClassID;
                    entityS.EntityType = Entities.CustomerPrice;
                    bestDiscount = de.SelectBestEntitytDiscount(Filter.Current.CustomerPriceClassID, Entities.CustomerPrice, Filter.Current.Type, (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity);
                    if (cachedDiscountsS.ContainsKey(entityS))
                        cachedDiscountsS[entityS].Add(bestDiscount);
                    else
                        cachedDiscountsS.Add(entityS, new List<DiscountDetails> { bestDiscount });
                }
                if (Filter.Current.InventoryPriceClassID != null)
                {
                    entityS.EntityID = (string)Filter.Current.InventoryPriceClassID;
                    entityS.EntityType = Entities.InventoryPrice;
                    bestDiscount = de.SelectBestEntitytDiscount(Filter.Current.InventoryPriceClassID, Entities.InventoryPrice, Filter.Current.Type, (decimal)Filter.Current.Amount, (decimal)Filter.Current.Quantity);
                    if (cachedDiscountsS.ContainsKey(entityS))
                        cachedDiscountsS[entityS].Add(bestDiscount);
                    else
                        cachedDiscountsS.Add(entityS, new List<DiscountDetails> { bestDiscount });
                }
            }*/

            return adapter.Get();
        }

        //Obsolete (for Find Best Applicable button)
        
        
        //public virtual DiscountDetails FindBestDiscount<Table>(string fieldName, int? entityID, string entityType, string type) where Table : IBqlTable
        //{
        //    DiscountEngine de = new DiscountEngine(); // to del!

        //    Dictionary<DiscountEntitiesI, List<DiscountDetails>> cachedDiscountsT = PXDatabase.GetSlot<Dictionary<DiscountEntitiesI, List<DiscountDetails>>>(typeof(DiscountDetails).Name + typeof(DiscountEntitiesI).Name, typeof(Dictionary<DiscountEntitiesI, List<DiscountDetails>>));
        //    DiscountEntitiesI entityToSearch = new DiscountEntitiesI();
        //    entityToSearch.EntityID = (int)entityID;
        //    entityToSearch.EntityType = entityType;
        //    if (!cachedDiscountsT.ContainsKey(entityToSearch))
        //    {
        //        cachedDiscountsT.Add(entityToSearch, new List<DiscountDetails>());
        //        de.CollectEntitytDiscounts<Table>(fieldName, entityToSearch);
        //    }
        //    DiscountDetails bestAmountDiscount = new DiscountDetails();
        //    bestAmountDiscount.Discount = 0;
        //    DiscountDetails bestPercentDiscount = new DiscountDetails();
        //    bestPercentDiscount.Discount = 0;

        //    cachedDiscountsT[entityToSearch].RemoveAll(x => x.IsBest == true); //to del

        //    //Searching best available discount (1 on amount and 1 on quantity)
        //    foreach (DiscountDetails detail in cachedDiscountsT[entityToSearch])
        //    {
        //        if ((detail.BreakBy == "A" && Filter.Current.Amount >= detail.From && Filter.Current.Amount < detail.To)
        //            || (detail.BreakBy == "Q" && Filter.Current.Quantity >= detail.From && Filter.Current.Quantity < detail.To))
        //        {
        //            if (detail.DiscountedFor == "A" && detail.Discount > bestAmountDiscount.Discount) bestAmountDiscount = detail;
        //            if (detail.DiscountedFor == "P" && detail.Discount > bestPercentDiscount.Discount) bestPercentDiscount = detail;
        //        }
        //    }
        //    bestAmountDiscount.IsBest = true;
        //    bestPercentDiscount.IsBest = true;

        //    if (bestPercentDiscount.Discount != 0 && bestAmountDiscount.Discount != 0)
        //    {
        //        if ((Filter.Current.Amount / 100 * bestPercentDiscount.Discount) > bestAmountDiscount.Discount)
        //        {
        //            //cachedDiscountsT[entityToSearch].Add(bestPercentDiscount); //obsolete
        //            return bestPercentDiscount;
        //        }
        //        else
        //        {
        //            //cachedDiscountsT[entityToSearch].Add(bestAmountDiscount); //obsolete
        //            return bestAmountDiscount;
        //        }
        //    }
        //    else
        //        if (bestPercentDiscount.Discount == 0 && bestAmountDiscount.Discount != 0)
        //        {
        //            //cachedDiscountsT[entityToSearch].Add(bestAmountDiscount); //obsolete
        //            return bestAmountDiscount;
        //        }
        //        else
        //            if (bestPercentDiscount.Discount != 0 && bestAmountDiscount.Discount == 0)
        //            {
        //                //cachedDiscountsT[entityToSearch].Add(bestPercentDiscount); //obsolete
        //                return bestPercentDiscount;
        //            }
        //            else
        //            {
        //                return new DiscountDetails();
        //            }
        //    /*bestAmountDiscount.IsBest = true;
        //    if (bestAmountDiscount.DiscountID != null)
        //    cachedDiscountsT[entityToSearch].Add(bestAmountDiscount);
        //    bestPercentDiscount.IsBest = true;
        //    if (bestPercentDiscount.DiscountID != null)
        //    cachedDiscountsT[entityToSearch].Add(bestPercentDiscount);*/

        //}
    }

    public struct DiscountEntitiesI
    {
        public int EntityID;
        public string EntityType;
        //public string ApplicableTo;
    }

    public struct DiscountEntitiesS
    {
        public string EntityID;
        public string EntityType;
        //public string ApplicableTo;
    }

    [Serializable]
    public partial class DiscountInquiryDetails : PX.Data.IBqlTable
    {
        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Discount Code", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        //[PXSelector(typeof(SODiscount.discountID))]
        public virtual String DiscountID
        {
            get
            {
                return this._DiscountID;
            }
            set
            {
                this._DiscountID = value;
            }
        }
        #endregion
        #region DiscountSequenceID
        public abstract class discountSequenceID : PX.Data.IBqlField
        {
        }
        protected String _DiscountSequenceID;
        [PXString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
        [PXUIField(DisplayName = "Discount Sequence", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual String DiscountSequenceID
        {
            get
            {
                return this._DiscountSequenceID;
            }
            set
            {
                this._DiscountSequenceID = value;
            }
        }
        #endregion
        #region Type
        public abstract class type : PX.Data.IBqlField
        {
        }
        protected String _Type;
        [PXString(1, IsFixed = true)]
        [DiscountType.List()]
        [PXUIField(DisplayName = "Discount Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual String Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
        #endregion
        #region ApplicableTo
        public abstract class applicableTo : PX.Data.IBqlField
        {
            public class customer : Constant<string>
            {
                public customer()
                    : base(DiscountTarget.Customer)
                {
                }
            }

            public class customerAndInventory : Constant<string>
            {
                public customerAndInventory()
                    : base(DiscountTarget.CustomerAndInventory)
                {
                }
            }

            public class customerAndInventoryPrice : Constant<string>
            {
                public customerAndInventoryPrice()
                    : base(DiscountTarget.CustomerAndInventoryPrice)
                {
                }
            }

            public class customerPrice : Constant<string>
            {
                public customerPrice()
                    : base(DiscountTarget.CustomerPrice)
                {
                }
            }

            public class customerPriceAndInventory : Constant<string>
            {
                public customerPriceAndInventory()
                    : base(DiscountTarget.CustomerPriceAndInventory)
                {
                }
            }

            public class customerPriceAndInventoryPrice : Constant<string>
            {
                public customerPriceAndInventoryPrice()
                    : base(DiscountTarget.CustomerPriceAndInventoryPrice)
                {
                }
            }

            public class inventory : Constant<string>
            {
                public inventory()
                    : base(DiscountTarget.Inventory)
                {
                }
            }

            public class inventoryPrice : Constant<string>
            {
                public inventoryPrice()
                    : base(DiscountTarget.InventoryPrice)
                {
                }
            }

            public class unconditional : Constant<string>
            {
                public unconditional()
                    : base(DiscountTarget.Unconditional)
                {
                }
            }
        }
        protected String _ApplicableTo;
        [PXString()]
        [PXUIField(DisplayName = "Applicable To", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual String ApplicableTo
        {
            get
            {
                return this._ApplicableTo;
            }
            set
            {
                this._ApplicableTo = value;
            }
        }
        #endregion
        #region DiscountedFor
        public abstract class discountedFor : PX.Data.IBqlField
        {
        }
        protected String _DiscountedFor;
        [PXString(1, IsFixed = true)]
        [DiscountOption.List]
        [PXUIField(DisplayName = "Discount by", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual String DiscountedFor
        {
            get
            {
                return this._DiscountedFor;
            }
            set
            {
                this._DiscountedFor = value;
            }
        }
        #endregion
        #region BreakBy
        public abstract class breakBy : PX.Data.IBqlField
        {
        }
        protected String _BreakBy;
        [PXString(1, IsFixed = true)]
        [BreakdownType.List]
        [PXUIField(DisplayName = "Break by", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual String BreakBy
        {
            get
            {
                return this._BreakBy;
            }
            set
            {
                this._BreakBy = value;
            }
        }
        #endregion
        #region AmountFrom
        public abstract class amountFrom : PX.Data.IBqlField
        {
        }
        protected Decimal? _AmountFrom;
        [PXPriceCost(MinValue = 0)]
        [PXUIField(DisplayName = "Amount From", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? AmountFrom
        {
            get
            {
                return this._AmountFrom;
            }
            set
            {
                this._AmountFrom = value;
            }
        }
        #endregion
        #region AmountTo
        public abstract class amountTo : PX.Data.IBqlField
        {
        }
        protected Decimal? _AmountTo;
        [PXPriceCost(MinValue = 0)]
        [PXUIField(DisplayName = "Amount To", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? AmountTo
        {
            get
            {
                return this._AmountTo;
            }
            set
            {
                this._AmountTo = value;
            }
        }
        #endregion
        #region QuantityFrom
        public abstract class quantityFrom : PX.Data.IBqlField
        {
        }
        protected Decimal? _QuantityFrom;
        [PXQuantity(MinValue = 0)]
        [PXUIField(DisplayName = "Quantity From", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? QuantityFrom
        {
            get
            {
                return this._QuantityFrom;
            }
            set
            {
                this._QuantityFrom = value;
            }
        }
        #endregion
        #region QuantityTo
        public abstract class quantityTo : PX.Data.IBqlField
        {
        }
        protected Decimal? _QuantityTo;
        [PXQuantity(MinValue = 0)]
        [PXUIField(DisplayName = "Quantity To", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? QuantityTo
        {
            get
            {
                return this._QuantityTo;
            }
            set
            {
                this._QuantityTo = value;
            }
        }
        #endregion

        #region Discount
        public abstract class discount : PX.Data.IBqlField
        {
        }
        protected Decimal? _Discount;
        [PXDecimal(typeof(Search2<Currency.decimalPlaces, InnerJoin<Company, On<Company.baseCuryID, Equal<Currency.curyID>>>>))]
        [PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? Discount
        {
            get
            {
                return this._Discount;
            }
            set
            {
                this._Discount = value;
            }
        }
        #endregion
        #region DiscountPercent
        public abstract class discountPercent : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscountPercent;
        [PXDecimal(typeof(Search2<Currency.decimalPlaces, InnerJoin<Company, On<Company.baseCuryID, Equal<Currency.curyID>>>>))]
        [PXUIField(DisplayName = "Percent", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? DiscountPercent
        {
            get
            {
                return this._DiscountPercent;
            }
            set
            {
                this._DiscountPercent = value;
            }
        }
        #endregion
        #region EntityID
        public abstract class entityID : PX.Data.IBqlField
        {
        }
        protected Int32? _EntityID;
        [PXUIField(DisplayName = "Entity ID", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Int32? EntityID
        {
            get
            {
                return this._EntityID;
            }
            set
            {
                this._EntityID = value;
            }
        }
        #endregion
        #region EntityType
        public abstract class entityType : PX.Data.IBqlField
        {
        }
        protected String _EntityType;
        [PXString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Entity Type", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual String EntityType
        {
            get
            {
                return this._EntityType;
            }
            set
            {
                this._EntityType = value;
            }
        }
        #endregion
        //#region BestDiscount
        //public abstract class bestDiscount : PX.Data.IBqlField
        //{
        //}
        //protected Boolean? _BestDiscount;
        //[PXBool()]
        //[PXUIField(DisplayName = "Best Discount", Visibility = PXUIVisibility.Visible, Enabled = false)]
        //public virtual Boolean? BestDiscount
        //{
        //    get
        //    {
        //        return this._BestDiscount;
        //    }
        //    set
        //    {
        //        this._BestDiscount = value;
        //    }
        //}
        //#endregion
        #region DiscountAmount
        public abstract class discountAmount : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscountAmount;
        [PXDecimal(typeof(Search2<Currency.decimalPlaces, InnerJoin<Company, On<Company.baseCuryID, Equal<Currency.curyID>>>>))]
        [PXUIField(DisplayName = "Discount Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? DiscountAmount
        {
            get
            {
                return this._DiscountAmount;
            }
            set
            {
                this._DiscountAmount = value;
            }
        }
        #endregion
    }

    [Serializable]
    public partial class DiscountFilter : IBqlTable
    {
        #region ApplicableTo
        public abstract class applicableTo : PX.Data.IBqlField
        {
            //public class ListAttribute : PXStringListAttribute
            //{
            //    public ListAttribute()
            //        : base(
            //        new string[] { DiscountTarget.Customer, DiscountTarget.Inventory, DiscountTarget.InventoryPrice, DiscountTarget.CustomerAndInventory, DiscountTarget.CustomerAndInventoryPrice, 
            //            DiscountTarget.CustomerPrice, DiscountTarget.CustomerPriceAndInventory, DiscountTarget.CustomerPriceAndInventoryPrice, DiscountTarget.Warehouse, DiscountTarget.WarehouseAndInventory, 
            //            DiscountTarget.WarehouseAndCustomer, DiscountTarget.WarehouseAndInventoryPrice, DiscountTarget.WarehouseAndCustomerPrice, DiscountTarget.Branch, DiscountTarget.Vendor, 
            //            DiscountTarget.Unconditional },
            //        new string[] { Messages.Customer, Messages.Discount_Inventory, Messages.InventoryPrice, Messages.CustomerAndInventory, Messages.CustomerAndInventoryPrice, 
            //            Messages.CustomerPrice, Messages.CustomerPriceAndInventory, Messages.CustomerPriceAndInventoryPrice, Messages.Warehouse, Messages.WarehouseAndInventory, 
            //            Messages.WarehouseAndCustomer, Messages.WarehouseAndInventoryPrice, Messages.WarehouseAndCustomerPrice, Messages.Branch, Messages.Vendor, 
            //            Messages.Unconditional }) { ; }
            //}

            //public const string Customer = "C";
            //public const string CustomerAndInventory = "A";
            //public const string CustomerAndInventoryPrice = "D";
            //public const string CustomerPrice = "P";
            //public const string CustomerPriceAndInventory = "B";
            //public const string CustomerPriceAndInventoryPrice = "E";

            //public const string Warehouse = "W";
            //public const string WarehouseAndInventory = "R";
            //public const string WarehouseAndCustomer = "H";
            //public const string WarehouseAndInventoryPrice = "O";
            //public const string WarehouseAndCustomerPrice = "S";
            //public const string Branch = "N";
            //public const string Vendor = "M";

            //public const string Inventory = "I";
            //public const string InventoryPrice = "V";
            //public const string Unconditional = "U";
            
            public class customer : Constant<string>
            {
                public customer()
                    : base(DiscountTarget.Customer)
                {
                }
            }

            public class customerAndInventory : Constant<string>
            {
                public customerAndInventory()
                    : base(DiscountTarget.CustomerAndInventory)
                {
                }
            }

            public class customerAndInventoryPrice : Constant<string>
            {
                public customerAndInventoryPrice()
                    : base(DiscountTarget.CustomerAndInventoryPrice)
                {
                }
            }

            public class customerPrice : Constant<string>
            {
                public customerPrice()
                    : base(DiscountTarget.CustomerPrice)
                {
                }
            }

            public class customerPriceAndInventory : Constant<string>
            {
                public customerPriceAndInventory()
                    : base(DiscountTarget.CustomerPriceAndInventory)
                {
                }
            }

            public class customerPriceAndInventoryPrice : Constant<string>
            {
                public customerPriceAndInventoryPrice()
                    : base(DiscountTarget.CustomerPriceAndInventoryPrice)
                {
                }
            }

            public class inventory : Constant<string>
            {
                public inventory()
                    : base(DiscountTarget.Inventory)
                {
                }
            }

            public class inventoryPrice : Constant<string>
            {
                public inventoryPrice()
                    : base(DiscountTarget.InventoryPrice)
                {
                }
            }

            public class unconditional : Constant<string>
            {
                public unconditional()
                    : base(DiscountTarget.Unconditional)
                {
                }
            }
        }
        protected String _ApplicableTo;
        [PXString(1, IsFixed = true)]
        //[applicableTo.List()]
        [PXDefault(DiscountTarget.Inventory)]
        [PXUIField(DisplayName = "Applicable To", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String ApplicableTo
        {
            get
            {
                return this._ApplicableTo;
            }
            set
            {
                this._ApplicableTo = value;
            }
        }
        #endregion
        #region CustomerID
        public abstract class customerID : PX.Data.IBqlField
        {
        }
        protected Int32? _CustomerID;
        [Customer(DescriptionField = typeof(Customer.acctName))]
        public virtual Int32? CustomerID
        {
            get
            {
                return this._CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [Inventory(typeof(Search<InventoryItem.inventoryID, Where<Match<Current<AccessInfo.userName>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))]
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
        #region CustomerPriceClassID
        public abstract class customerPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _CustomerPriceClassID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(ARPriceClass.priceClassID))]
        [PXUIField(DisplayName = "Customer Price Class", Visibility = PXUIVisibility.Visible)]
        public virtual String CustomerPriceClassID
        {
            get
            {
                return this._CustomerPriceClassID;
            }
            set
            {
                this._CustomerPriceClassID = value;
            }
        }
        #endregion
        #region InventoryPriceClassID
        public abstract class inventoryPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _InventoryPriceClassID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(INPriceClass.priceClassID))]
        [PXUIField(DisplayName = "Inventory Price Class", Visibility = PXUIVisibility.Visible)]
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
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch()]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
        #region SiteID
        public abstract class siteID : PX.Data.IBqlField
        {
        }
        protected Int32? _SiteID;
        [Site(DescriptionField = typeof(INSite.descr), DisplayName = "Warehouse")]
        public virtual Int32? SiteID
        {
            get
            {
                return this._SiteID;
            }
            set
            {
                this._SiteID = value;
            }
        }
        #endregion

        #region Type
        public abstract class type : PX.Data.IBqlField
        {
        }
        protected String _Type;
        [PXString(1, IsFixed = true)]
        [PXDefault(DiscountType.Line)]
        [DiscountType.List()]
        [PXUIField(DisplayName = "Discount Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
        #endregion
        #region Amount
        public abstract class amount : PX.Data.IBqlField
        {
        }
        protected Decimal? _Amount;
        [PXPriceCost(MinValue = 0)]
        [PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = true)]
        public virtual Decimal? Amount
        {
            get
            {
                return this._Amount;
            }
            set
            {
                this._Amount = value;
            }
        }
        #endregion
        #region Quantity
        public abstract class quantity : PX.Data.IBqlField
        {
        }
        protected Decimal? _Quantity;
        [PXQuantity(MinValue = 0)]
        [PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible, Enabled = true)]
        public virtual Decimal? Quantity
        {
            get
            {
                return this._Quantity;
            }
            set
            {
                this._Quantity = value;
            }
        }
        #endregion
        #region DocDate
        public abstract class docDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _DocDate;
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXDBDate()]
        [PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? DocDate
        {
            get
            {
                return this._DocDate;
            }
            set
            {
                this._DocDate = value;
            }
        }
        #endregion
    }

    /*[Serializable]
    [PXProjection(typeof(Select<SODiscount>))]
    public partial class SODiscountS : SODiscount
    {
        #region CaseID

        public new abstract class discountID : IBqlField { }

        #endregion

        #region CustomerID

        public new abstract class type : IBqlField { }

        #endregion

        #region ContactID

        public new abstract class applicableTo : IBqlField { }

        #endregion
    }

    public interface IDiscountEntity
    {
        string DiscountID { get; set; }
        string DiscountSequenceID { get; set; }
        int? EntityID { get; set; }
    }*/
}

