using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Globalization;
using PX.Api;
using PX.Data;

using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.PO;
using PX.Objects.CM;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    #region Line fields classes
    public abstract class BqlInterfaceDE
    {
        public readonly PXCache cache;
        public readonly object row;

        public BqlInterfaceDE(PXCache cache, object row)
        {
            this.cache = cache;
            this.row = row;
        }

        public abstract Type GetField<T>() where T : IBqlField;

        public virtual void RaiseFieldUpdated<T>(object oldValue)
            where T : IBqlField
        {
            cache.RaiseFieldUpdated(GetField<T>().Name, row, oldValue);
        }

		public virtual void RaiseFieldVerifying<T>(ref object newValue)
		where T : IBqlField
		{
			cache.RaiseFieldVerifying(GetField<T>().Name, row, ref newValue);
		}
    }

    public abstract class DiscountLineFields : BqlInterfaceDE
    {
        public virtual bool SkipDisc { get; set; }
        public virtual decimal? CuryDiscAmt { get; set; }
        public virtual decimal? DiscPct { get; set; }
        public virtual string DiscountID { get; set; }
        public virtual string DiscountSequenceID { get; set; }
        public virtual bool ManualDisc { get; set; }
        public virtual bool ManualPrice { get; set; }
        public virtual string LineType { get; set; }
        public virtual bool? IsFree { get; set; }

        public abstract class skipDisc : IBqlField { }
        public abstract class curyDiscAmt : IBqlField { }
        public abstract class discPct : IBqlField { }
        public abstract class discountID : IBqlField { }
        public abstract class discountSequenceID : IBqlField { }
        public abstract class manualDisc : IBqlField { }
        public abstract class manualPrice : IBqlField { }
        public abstract class lineType : IBqlField { }
        public abstract class isFree : IBqlField { }

        public DiscountLineFields(PXCache cache, object row)
            : base(cache, row)
        {
        }
    }

    public class DiscountLineFields<SkipDiscField, CuryDiscAmtField, DiscPctField, DiscountIDField, DiscountSequenceIDField, ManualDiscField, ManualPriceField, LineTypeField, IsFreeField> : DiscountLineFields
        where SkipDiscField : IBqlField
        where CuryDiscAmtField : IBqlField
        where DiscPctField : IBqlField
        where DiscountIDField : IBqlField
        where DiscountSequenceIDField : IBqlField
        where ManualDiscField : IBqlField
        where ManualPriceField : IBqlField
        where LineTypeField : IBqlField
        where IsFreeField : IBqlField
    {
        public DiscountLineFields(PXCache cache, object row)
            : base(cache, row)
        {
        }

        public override Type GetField<T>()
        {
            if (typeof(T) == typeof(skipDisc))
            {
                return typeof(SkipDiscField);
            }
            if (typeof(T) == typeof(curyDiscAmt))
            {
                return typeof(CuryDiscAmtField);
            }
            if (typeof(T) == typeof(discPct))
            {
                return typeof(DiscPctField);
            }
            if (typeof(T) == typeof(discountID))
            {
                return typeof(DiscountIDField);
            }
            if (typeof(T) == typeof(discountSequenceID))
            {
                return typeof(DiscountSequenceIDField);
            }
            if (typeof(T) == typeof(manualDisc))
            {
                return typeof(ManualDiscField);
            }
            if (typeof(T) == typeof(manualPrice))
            {
                return typeof(ManualPriceField);
            }
            if (typeof(T) == typeof(lineType))
            {
                return typeof(LineTypeField);
            }
            if (typeof(T) == typeof(isFree))
            {
                return typeof(IsFreeField);
            }
            return null;
        }

        public override bool SkipDisc
        {
            get
            {
                return (bool?)cache.GetValue<SkipDiscField>(row) == true;
            }
            set
            {
                cache.SetValue<SkipDiscField>(row, value);
            }
        }
        public override decimal? CuryDiscAmt
        {
            get
            {
                return (decimal?)cache.GetValue<CuryDiscAmtField>(row);
            }
            set
            {
                cache.SetValue<CuryDiscAmtField>(row, value);
            }
        }

        public override decimal? DiscPct
        {
            get
            {
                return (decimal?)cache.GetValue<DiscPctField>(row);
            }
            set
            {
                cache.SetValue<DiscPctField>(row, value);
            }
        }
        public override string DiscountID
        {
            get
            {
                return (string)cache.GetValue<DiscountIDField>(row);
            }
            set
            {
                cache.SetValue<DiscountIDField>(row, value);
            }
        }
        public override string DiscountSequenceID
        {
            get
            {
                return (string)cache.GetValue<DiscountSequenceIDField>(row);
            }
            set
            {
                cache.SetValue<DiscountSequenceIDField>(row, value);
            }
        }
        public override bool ManualDisc
        {
            get
            {
                return (bool?) cache.GetValue<ManualDiscField>(row) == true;
            }
            set
            {
                cache.SetValue<ManualDiscField>(row, value);
            }
        }
        public override bool ManualPrice
        {
            get
            {
                return (bool?)cache.GetValue<ManualPriceField>(row) == true;
            }
            set
            {
                cache.SetValue<ManualPriceField>(row, value);
            }
        }
        public override string LineType
        {
            get
            {
                return (string)cache.GetValue<LineTypeField>(row);
            }
            set
            {
                cache.SetValue<LineTypeField>(row, value);
            }
        }
        public override bool? IsFree
        {
            get
            {
                return (bool?)cache.GetValue<IsFreeField>(row) == true;
            }
            set
            {
                cache.SetValue<IsFreeField>(row, value);
            }
        }
    }

    public abstract class AmountLineFields : BqlInterfaceDE
	{
        public virtual decimal? Quantity { get; set; }
        public virtual decimal? CuryUnitPrice { get; set; }
        public virtual decimal? CuryExtPrice { get; set; }
        public virtual decimal? CuryLineAmount { get; set; }
        public virtual string UOM { get; set; }
        public virtual decimal? GroupDiscountRate { get; set; }
        public virtual decimal? DocumentDiscountRate { get; set; }
        public virtual string TaxCategoryID { get; set; }

		public abstract class quantity : IBqlField { }
        public abstract class curyUnitPrice : IBqlField { }
		public abstract class curyExtPrice : IBqlField { }
        public abstract class curyLineAmount : IBqlField { }
        public abstract class uOM : IBqlField { }
        public abstract class groupDiscountRate : IBqlField { }
        public abstract class documentDiscountRate : IBqlField { }
        public abstract class taxCategoryID : IBqlField { }
        
		public AmountLineFields(PXCache cache, object row)
			: base(cache, row)
		{
		}
	}

    public class AmountLineFields<QuantityField, CuryUnitPriceField, CuryExtPriceField, CuryLineAmountField, UOMField, GroupDiscountRateField, DocumentDiscountRateField> : AmountLineFields
        where QuantityField : IBqlField
        where CuryUnitPriceField : IBqlField
        where CuryExtPriceField : IBqlField
        where CuryLineAmountField : IBqlField
        where UOMField : IBqlField
        where GroupDiscountRateField : IBqlField
        where DocumentDiscountRateField : IBqlField
    {
        public AmountLineFields(PXCache cache, object row)
            : base(cache, row)
        {
        }

        public override Type GetField<T>()
        {
            if (typeof(T) == typeof(quantity))
            {
                return typeof(QuantityField);
            }
            if (typeof(T) == typeof(curyUnitPrice))
            {
                return typeof(CuryUnitPriceField);
            }
            if (typeof(T) == typeof(curyExtPrice))
            {
                return typeof(CuryExtPriceField);
            }
            if (typeof(T) == typeof(curyLineAmount))
            {
                return typeof(CuryLineAmountField);
            }
            if (typeof(T) == typeof(uOM))
            {
                return typeof(UOMField);
            }
            if (typeof(T) == typeof(groupDiscountRate))
            {
                return typeof(GroupDiscountRateField);
            }
            if (typeof(T) == typeof(documentDiscountRate))
            {
                return typeof(DocumentDiscountRateField);
            }
            return null;
        }

        public override decimal? Quantity
        {
            get
            {
                return (decimal?)cache.GetValue<QuantityField>(row);
            }
            set
            {
                cache.SetValue<QuantityField>(row, value);
            }
        }

        public override decimal? CuryUnitPrice
        {
            get
            {
                return (decimal?)cache.GetValue<CuryUnitPriceField>(row);
            }
            set
            {
                cache.SetValue<CuryUnitPriceField>(row, value);
            }
        }

        public override decimal? CuryExtPrice
        {
            get
            {
                return (decimal?)cache.GetValue<CuryExtPriceField>(row);
            }
            set
            {
                cache.SetValue<CuryExtPriceField>(row, value);
            }
        }

        public override decimal? CuryLineAmount
        {
            get
            {
                return (decimal?)cache.GetValue<CuryLineAmountField>(row);
            }
            set
            {
                cache.SetValue<CuryLineAmountField>(row, value);
            }
        }
        public override string UOM
        {
            get
            {
                return (string)cache.GetValue<UOMField>(row);
            }
            set
            {
                cache.SetValue<UOMField>(row, value);
            }
        }
        public override decimal? GroupDiscountRate
        {
            get
            {
                return (decimal?)cache.GetValue<GroupDiscountRateField>(row);
            }
            set
            {
                cache.SetValue<GroupDiscountRateField>(row, value);
            }
        }
        public override decimal? DocumentDiscountRate
        {
            get
            {
                return (decimal?)cache.GetValue<DocumentDiscountRateField>(row);
            }
            set
            {
                cache.SetValue<DocumentDiscountRateField>(row, value);
            }
        }
    }

    public abstract class LineEntitiesFields : BqlInterfaceDE
    {
        public virtual int? InventoryID { get; set; }
        public virtual int? CustomerID { get; set; }
        public virtual int? SiteID { get; set; }
        public virtual int? BranchID { get; set; }
        public virtual int? VendorID { get; set; }

        public abstract class inventoryID : IBqlField { }
        public abstract class customerID : IBqlField { }
        public abstract class siteID : IBqlField { }
        public abstract class branchID : IBqlField { }
        public abstract class vendorID : IBqlField { }

        public LineEntitiesFields(PXCache cache, object row)
            : base(cache, row)
        {
        }
    }

    public class LineEntitiesFields<InventoryField, CustomerField, SiteField, BranchField, VendorField> : LineEntitiesFields
        where InventoryField : IBqlField
        where CustomerField : IBqlField
        where SiteField : IBqlField
        where BranchField : IBqlField
        where VendorField : IBqlField
    {
        public LineEntitiesFields(PXCache cache, object row)
            : base(cache, row)
        {
        }

        public override Type GetField<T>()
        {
            if (typeof(T) == typeof(inventoryID))
            {
                return typeof(InventoryField);
            }
            if (typeof(T) == typeof(customerID))
            {
                return typeof(CustomerField);
            }
            if (typeof(T) == typeof(siteID))
            {
                return typeof(SiteField);
            }
            if (typeof(T) == typeof(branchID))
            {
                return typeof(BranchField);
            }
            if (typeof(T) == typeof(vendorID))
            {
                return typeof(VendorField);
            }
            return null;
        }

        public override int? InventoryID
        {
            get
            {
                return (int?)cache.GetValue<InventoryField>(row);
            }
            set
            {
                cache.SetValue<InventoryField>(row, value);
            }
        }

        public override int? CustomerID
        {
            get
            {
                return (int?)cache.GetValue<CustomerField>(row);
            }
            set
            {
                cache.SetValue<CustomerField>(row, value);
            }
        }

        public override int? SiteID
        {
            get
            {
                return (int?)cache.GetValue<SiteField>(row);
            }
            set
            {
                cache.SetValue<SiteField>(row, value);
            }
        }

        public override int? BranchID
        {
            get
            {
                return (int?)cache.GetValue<BranchField>(row);
            }
            set
            {
                cache.SetValue<BranchField>(row, value);
            }
        }
        public override int? VendorID
        {
            get
            {
                return (int?)cache.GetValue<VendorField>(row);
            }
            set
            {
                cache.SetValue<VendorField>(row, value);
            }
        }
    }
    #endregion

    public class DiscountEngine
    {
        #region DiscountDetailLine-related functions

        /// <summary>
        /// Returns single best discount details line for a given Entities set
        /// </summary>
        /// <param name="sender">Cache</param>
        /// <param name="dline">Discount-related fields</param>
        /// <param name="type">Line or Document</param>
        public virtual DiscountDetailLine SelectBestDiscount(PXCache sender, DiscountLineFields dline, HashSet<KeyValuePair<object, string>> entities, string type, decimal curyAmount, decimal quantity, DateTime date)
        {
            DiscountEngine.GetDiscountTypes();
            List<DiscountDetailLine> foundDiscounts = new List<DiscountDetailLine>();
            if (entities != null)
            {
                foundDiscounts = SelectApplicableDiscounts(sender, dline, SelectApplicableEntitytDiscounts(entities, type), curyAmount, quantity, type, date);
                if (foundDiscounts.Count != 0)
                {
                    return foundDiscounts[0];
                }
            }
            return new DiscountDetailLine();
        }

        /// <summary>
        /// Returns single DiscountDetails line on a given DiscountSequenceKey
        /// </summary>
        /// <param name="discountSequence">Applicable Discount Sequence</param>
        /// <param name="amount">Discountable amount</param>
        /// <param name="quantity">Discountable quantity</param>
        /// <param name="discountFor">Discount type: line, group or document</param>
        public virtual DiscountDetailLine SelectApplicableDiscount(PXCache sender, DiscountLineFields dline, DiscountSequenceKey discountSequence, decimal? curyDiscountableAmount, decimal? discountableQuantity, string type, DateTime date)
        {
            HashSet<DiscountSequenceKey> discountSequences = new HashSet<DiscountSequenceKey>();
			discountSequences.Add(discountSequence);
            List<DiscountDetailLine> discountDetails = SelectApplicableDiscounts(sender, dline, discountSequences, curyDiscountableAmount, discountableQuantity, type, date);
            if (discountDetails.Count != 0)
            {
                return discountDetails[0];
            }
            return new DiscountDetailLine();
        }

        /// <summary>
        /// Returns single DiscountDetails line. Accepts HashSet of DiscountSequenceKey
        /// </summary>
        /// <param name="discountSequence">Applicable Discount Sequences</param>
        /// <param name="amount">Discountable amount</param>
        /// <param name="quantity">Discountable quantity</param>
        /// <param name="discountFor">Discount type: line, group or document</param>
        public virtual DiscountDetailLine SelectApplicableDiscount(PXCache sender, DiscountLineFields dline, HashSet<DiscountSequenceKey> discountSequences, decimal? curyDiscountableAmount, decimal? discountableQuantity, string type, DateTime date)
        {
            List<DiscountDetailLine> discountDetails = SelectApplicableDiscounts(sender, dline, discountSequences, curyDiscountableAmount, discountableQuantity, type, date);
            if (discountDetails.Count != 0)
            {
                return discountDetails[0];
            }
            return new DiscountDetailLine();
        }

        /// <summary>
        /// Returns all Discount Sequences applicable to a given entities set.
        /// </summary>
        /// <param name="entities">Entities dictionary</param>
        /// <param name="discountType">Line, Group or Document</param>
        /// <returns></returns>
        public virtual HashSet<DiscountSequenceKey> SelectApplicableEntitytDiscounts(HashSet<KeyValuePair<object, string>> entities, string discountType, bool skipManual = true)
        {
            bool IsARDiscount = true;
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();
            HashSet<DiscountSequenceKey> applicableDiscounts = new HashSet<DiscountSequenceKey>();
            HashSet<DiscountSequenceKey> allFoundDiscounts = new HashSet<DiscountSequenceKey>();
            Dictionary<ApplicableToCombination, ImmutableHashSet<DiscountSequenceKey>> applicableDiscountsByEntity = new Dictionary<ApplicableToCombination, ImmutableHashSet<DiscountSequenceKey>>();

            int? vendorID = null;
            foreach (KeyValuePair<object, string> vendorEntity in entities)
            {
                if (vendorEntity.Value == DiscountTarget.Vendor)
                {
                    vendorID = (int)vendorEntity.Key;
                    IsARDiscount = false;
                }
            }

            foreach (KeyValuePair<object, string> entity in entities)
            {
                ImmutableHashSet<DiscountSequenceKey> applicableEntityDiscounts = ImmutableHashSet.Create<DiscountSequenceKey>();

                if (IsARDiscount)
                {
                    applicableEntityDiscounts = GetApplicableEntityARDiscounts(entity);
                }
                else
                {
                    applicableEntityDiscounts = GetApplicableEntityAPDiscounts(entity, vendorID);
                }

                if (applicableEntityDiscounts.Count != 0)
                {
                    applicableDiscountsByEntity.Add(SetApplicableToCombination(entity.Value), applicableEntityDiscounts);

					foreach (DiscountSequenceKey dkey in applicableEntityDiscounts)
					{
						allFoundDiscounts.Add(dkey);
					}
                }
            }

            //add all applicable unconditional discounts
            if (IsARDiscount)
                foreach (DiscountSequenceKey unconditionalDiscountSequence in GetUnconditionalDiscountsByType(discountType))
                {
                    if (!skipManual || (skipManual && cachedDiscountTypes[unconditionalDiscountSequence.DiscountID].IsManual != true))
                    {
						applicableDiscounts.Add(unconditionalDiscountSequence);
                    }
                }

            //searching for correct entity discounts
            foreach (DiscountSequenceKey discountSequence in allFoundDiscounts)
            {
                if (cachedDiscountTypes.ContainsKey(discountSequence.DiscountID) && cachedDiscountTypes[discountSequence.DiscountID].Type == discountType && (!skipManual || (skipManual && !cachedDiscountTypes[discountSequence.DiscountID].IsManual))
                    && ((IsARDiscount && !cachedDiscountTypes[discountSequence.DiscountID].IsVendorDiscount) || (!IsARDiscount && cachedDiscountTypes[discountSequence.DiscountID].IsVendorDiscount)))
                {
                    ApplicableToCombination combinedApplicableTo = ApplicableToCombination.None;
                    bool correctDiscount = true;

                    foreach (KeyValuePair<ApplicableToCombination, ImmutableHashSet<DiscountSequenceKey>> singleEntity in applicableDiscountsByEntity)
                    {
                        if ((singleEntity.Key & cachedDiscountTypes[discountSequence.DiscountID].ApplicableToEnum) != ApplicableToCombination.None)
                        {
                            if (!singleEntity.Value.Contains(discountSequence))
                            {
                                correctDiscount = false;
                                break;
                            }
                            combinedApplicableTo = combinedApplicableTo | singleEntity.Key;
                        }
                    }
                    if (combinedApplicableTo == cachedDiscountTypes[discountSequence.DiscountID].ApplicableToEnum && correctDiscount)
                    {
						applicableDiscounts.Add(discountSequence);
                    }
                }
            }
            return applicableDiscounts;
        }

        /// <summary>
        /// Returns best available discount for Line and Document discount types. Returns list of all applicable discounts for Group discount type. 
        /// </summary>
        /// <param name="discountSequences">Applicable Discount Sequences</param>
        /// <param name="curyDiscountableAmount">Discountable amount</param>
        /// <param name="discountableQuantity">Discountable quantity</param>
        /// <param name="discountFor">Discount type: line, group or document</param>
        public virtual List<DiscountDetailLine> SelectApplicableDiscounts(PXCache sender, DiscountLineFields dline, HashSet<DiscountSequenceKey> discountSequences, decimal? curyDiscountableAmount, decimal? discountableQuantity, string type, DateTime date, bool ignoreCurrency = false)
        {
            decimal discountableAmount = (decimal)curyDiscountableAmount;

            if (sender == null)
                throw new ArgumentNullException("sender");

            if (!ignoreCurrency && dline != null && dline.row != null && dline.cache.GetValue(dline.row, typeof(CM.CurrencyInfo.curyInfoID).Name) != null)
                PXCurrencyAttribute.CuryConvBase(sender, dline.row, (decimal)curyDiscountableAmount, out discountableAmount);

            List<DiscountDetailLine> discountsToReturn = new List<DiscountDetailLine>();

            DiscountDetailLine bestAmountDiscount = new DiscountDetailLine();
            DiscountDetailLine bestPercentDiscount = new DiscountDetailLine();

            if (type != DiscountType.Group)
            {
                bestAmountDiscount = SelectSingleBestDiscount(sender, dline, discountSequences.ToList(), discountableAmount, (decimal)discountableQuantity, DiscountOption.Amount, date);
                bestPercentDiscount = SelectSingleBestDiscount(sender, dline, discountSequences.ToList(), discountableAmount, (decimal)discountableQuantity, DiscountOption.Percent, date);

                if (bestAmountDiscount.DiscountID != null && bestPercentDiscount.DiscountID != null)
                {
                    if (bestAmountDiscount.Discount < discountableAmount / 100 * bestPercentDiscount.Discount)
                    {
                        discountsToReturn.Add(bestPercentDiscount);
                    }
                    else
                    {
                        discountsToReturn.Add(bestAmountDiscount);
                    }
                }
                else
                {
                    if (bestAmountDiscount.DiscountID != null && bestPercentDiscount.DiscountID == null)
                        discountsToReturn.Add(bestAmountDiscount);
                    if (bestAmountDiscount.DiscountID == null && bestPercentDiscount.DiscountID != null)
                        discountsToReturn.Add(bestPercentDiscount);
                }
            }
            else
            {
                discountsToReturn.Add(SelectAllApplicableDiscounts(sender, dline, discountSequences.ToList(), discountableAmount, (decimal)discountableQuantity, date));
            }
            return discountsToReturn;
        }

        public virtual DiscountDetailLine CreateDiscountDetails(PXCache sender, DiscountLineFields dline, PXResult<DiscountSequence, DiscountSequenceDetail> discountResult, DateTime date)
        {
			CommonSetup commonsetup = PXSelect<CommonSetup>.Select(sender.Graph);
            int precision = 4;
			if (commonsetup != null && commonsetup.DecPlPrcCst != null)
				precision = commonsetup.DecPlPrcCst.Value;

			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes  = GetCachedDiscountCodes();

            DiscountSequenceDetail bestDiscountDetail = (DiscountSequenceDetail)discountResult;
            DiscountSequence bestDiscountSequence = (DiscountSequence)discountResult;
            if (bestDiscountDetail != null && bestDiscountSequence != null)
            {
                DiscountDetailLine newDiscountDetail = new DiscountDetailLine();
                newDiscountDetail.DiscountID = bestDiscountDetail.DiscountID;
                newDiscountDetail.DiscountSequenceID = bestDiscountDetail.DiscountSequenceID;
                newDiscountDetail.Type = cachedDiscountTypes[bestDiscountDetail.DiscountID].Type;
                newDiscountDetail.DiscountedFor = bestDiscountSequence.DiscountedFor;
                newDiscountDetail.BreakBy = bestDiscountSequence.BreakBy;
                decimal discountAmount;
                decimal curyAmount;
                decimal curyAmountTo;
                if (bestDiscountSequence.IsPromotion == true || bestDiscountDetail.LastDate <= date)
                {
                    if (newDiscountDetail.DiscountedFor == DiscountOption.Amount && newDiscountDetail.Type != DiscountType.Line)
                    {
                        PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)bestDiscountDetail.Discount, out discountAmount, precision);
                        newDiscountDetail.Discount = discountAmount;
                    }
                    else
                        newDiscountDetail.Discount = bestDiscountDetail.Discount;
                    if (bestDiscountSequence.BreakBy == "Q")
                    {
                        newDiscountDetail.AmountFrom = bestDiscountDetail.Quantity;
                        newDiscountDetail.AmountTo = bestDiscountDetail.QuantityTo;
                    }
                    else
                    {
                        if (dline != null)
                        {
                            PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)bestDiscountDetail.Amount, out curyAmount, precision);
                            newDiscountDetail.AmountFrom = curyAmount;
                            
                            if (bestDiscountDetail.AmountTo != null)
                            {
                                PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)bestDiscountDetail.AmountTo, out curyAmountTo, precision);
                                newDiscountDetail.AmountTo = curyAmountTo;
                            }
                            else
                            {
                                newDiscountDetail.AmountTo = null;
                            }
                        }
                        else
                        {
                            newDiscountDetail.AmountFrom = bestDiscountDetail.Amount;
                            newDiscountDetail.AmountTo = bestDiscountDetail.AmountTo;
                        }
                    }
                    if (bestDiscountSequence.DiscountedFor == DiscountOption.FreeItem)
                    {
                        newDiscountDetail.freeItemID = bestDiscountSequence.FreeItemID;
                        newDiscountDetail.freeItemQty = bestDiscountDetail.FreeItemQty;
                    }
                    newDiscountDetail.Prorate = bestDiscountSequence.Prorate;
                    return newDiscountDetail;
                }
                else
                {
                    if (newDiscountDetail.DiscountedFor == DiscountOption.Amount && newDiscountDetail.Type != DiscountType.Line)
                    {
                        PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)bestDiscountDetail.Discount, out discountAmount, precision);
                        newDiscountDetail.Discount = discountAmount;
                    }
                    else
                        newDiscountDetail.Discount = bestDiscountDetail.Discount;
                    if (bestDiscountSequence.BreakBy == "Q")
                    {
                        newDiscountDetail.AmountFrom = bestDiscountDetail.Quantity;
                        newDiscountDetail.AmountTo = bestDiscountDetail.QuantityTo;
                    }
                    else
                    {
                        if (dline != null)
                        {
                            PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)(bestDiscountDetail.Amount ?? 0m), out curyAmount, precision);
                            newDiscountDetail.AmountFrom = curyAmount;
                            if (bestDiscountDetail.AmountTo != null)
                            {
                                PXCurrencyAttribute.CuryConvCury(sender, dline.row, (decimal)(bestDiscountDetail.AmountTo ?? 0m), out curyAmountTo, precision);
                                newDiscountDetail.AmountTo = curyAmountTo;
                            }
                            else
                            {
                                newDiscountDetail.AmountTo = null;
                            }
                        }
                        else
                        {
                            newDiscountDetail.AmountFrom = bestDiscountDetail.Amount;
                            newDiscountDetail.AmountTo = bestDiscountDetail.AmountTo;
                        }
                    }
                    if (bestDiscountSequence.DiscountedFor == DiscountOption.FreeItem)
                    {
                        newDiscountDetail.freeItemID = bestDiscountSequence.LastFreeItemID;
                        newDiscountDetail.freeItemQty = bestDiscountDetail.FreeItemQty;
                    }
                    newDiscountDetail.Prorate = bestDiscountSequence.Prorate;
                    return newDiscountDetail;
                }
            }
            return new DiscountDetailLine();
        }

        #endregion

        #region Discount codes and Entity discount caches

        #region Discount Codes Cache
        protected static DCCache DiscountCodesCache
        {
            get
            {
                DCCache codes = PX.Common.PXContext.GetSlot<DCCache>();
                if (codes == null)
                {
                    codes = PX.Common.PXContext.SetSlot<DCCache>(PXDatabase.GetSlot<DCCache>(typeof(DiscountCode).Name, typeof(ConcurrentDictionary<string, DiscountCode>), typeof(ARDiscount), typeof(AP.APDiscount)));
                }
                return codes;
            }
        }

        protected class DCCache : IPrefetchable
        {
            private ConcurrentDictionary<string, DiscountCode> _cachedDiscountCodes;
            public ConcurrentDictionary<string, DiscountCode> cachedDiscountCodes
            {
                get { return _cachedDiscountCodes; }
            }

            public void Prefetch()
            {
                _cachedDiscountCodes = new ConcurrentDictionary<string, DiscountCode>();

                foreach (PXDataRecord discountType in PXDatabase.SelectMulti<ARDiscount>(
                new PXDataField<ARDiscount.discountID>(),
                new PXDataField<ARDiscount.type>(),
                new PXDataField<ARDiscount.isManual>(),
                new PXDataField<ARDiscount.excludeFromDiscountableAmt>(),
                new PXDataField<ARDiscount.skipDocumentDiscounts>(),
                new PXDataField<ARDiscount.applicableTo>()))
                {
                    DiscountCode type = new DiscountCode();
                    type.IsVendorDiscount = false;
                    type.Type = discountType.GetString(1);
                    type.IsManual = (bool)discountType.GetBoolean(2);
                    type.ExcludeFromDiscountableAmt = (bool)discountType.GetBoolean(3);
                    type.SkipDocumentDiscounts = (bool)discountType.GetBoolean(4);
                    type.ApplicableToEnum = SetApplicableToCombination(discountType.GetString(5)); //bitmask

                    _cachedDiscountCodes.GetOrAdd(discountType.GetString(0), type);
                }

                foreach (PXDataRecord discountType in PXDatabase.SelectMulti<AP.APDiscount>(
                new PXDataField<AP.APDiscount.bAccountID>(),
                new PXDataField<AP.APDiscount.discountID>(),
                new PXDataField<AP.APDiscount.type>(),
                new PXDataField<AP.APDiscount.isManual>(),
                new PXDataField<AP.APDiscount.excludeFromDiscountableAmt>(),
                new PXDataField<AP.APDiscount.skipDocumentDiscounts>(),
                new PXDataField<AP.APDiscount.applicableTo>()))
                {
                    DiscountCode type = new DiscountCode();
                    type.IsVendorDiscount = true;
                    type.VendorID = discountType.GetInt32(0);
                    type.Type = discountType.GetString(2);
                    type.IsManual = (bool)discountType.GetBoolean(3);
                    type.ExcludeFromDiscountableAmt = (bool)discountType.GetBoolean(4);
                    type.SkipDocumentDiscounts = (bool)discountType.GetBoolean(5);
                    type.ApplicableToEnum = SetApplicableToCombination(discountType.GetString(6)); //bitmask

                    _cachedDiscountCodes.GetOrAdd(discountType.GetString(1), type);

                }
            }
        }
        #endregion

        #region Discount Entities Cache
        
        public static void UpdateEntitytCache()
        {           
            PX.Common.PXContext.SetSlot<DECache<DiscountSequence>>(PXDatabase.GetSlot<DECache<DiscountSequence>>(typeof(DiscountSequence).Name, typeof(DiscountSequence)));
        }

        protected class DECache<TableType> : IPrefetchable
            where TableType : IBqlTable
        {
            public void Prefetch()
            {
                ClearAllEntityCaches();
            }
        }

        #endregion

        /// <summary>
        /// Returns dictionary of cached discount codes. Dictionary key is DiscountID
        /// </summary>
        public static ConcurrentDictionary<string, DiscountCode> GetCachedDiscountCodes()
        {
			ConcurrentDictionary<string, DiscountCode> cachedDiscountCodes = DiscountCodesCache.cachedDiscountCodes;
            if (cachedDiscountCodes.Count == 0)
            {
                GetDiscountTypes(false);
            }
            return cachedDiscountCodes;
        }

        /// <summary>
        /// Collects all discount types and unconditional AR discounts
        /// </summary>
        /// <param name="clearCache">Set to true to clear discount types cache and recreate it.</param>
        public static void GetDiscountTypes(bool clearCache = false)
        {
            SelectUnconditionalDiscounts(clearCache);
        }

		public static void ClearAllEntityDiscounts<Table, EntityType>()
		where Table : IBqlTable
		{
			ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>));
			cachedDiscountEntities.Clear();
		}

		public static void ClearAllEntityDiscountsTwoKeys<Table, EntityType1, EntityType2>()
			where Table : IBqlTable
		{
			ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>));
			cachedDiscountEntities.Clear();
		}

		public static void ClearEntityDiscounts<Table, EntityType>(EntityType entityID)
			where Table : IBqlTable
		{
			ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>));
			if (cachedDiscountEntities.ContainsKey(entityID))
			{
				cachedDiscountEntities.Remove(entityID);
			}
		}

		public static void ClearEntityDiscountsTwoKeys<Table, EntityType1, EntityType2>(EntityType1 entityID1, EntityType2 entityID2)
			where Table : IBqlTable
		{
			ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>));
			KeyValuePair<EntityType1, EntityType2> entityKey = new KeyValuePair<EntityType1, EntityType2>(entityID1, entityID2);
			if (cachedDiscountEntities.ContainsKey(entityKey))
			{
				cachedDiscountEntities.Remove(entityKey);
			}
		}

		private static HashSet<DiscountSequenceKey> GetUnconditionalDiscountsByType(string type)
		{
			ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>>>(Messages.Unconditional, typeof(ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>>));
			if (cachedDiscountEntities.ContainsKey(Messages.Unconditional))
			{
				HashSet<DiscountSequenceKey> unconditionalDiscounts = new HashSet<DiscountSequenceKey>();
				foreach (DiscountSequenceKey discountSequence in cachedDiscountEntities[Messages.Unconditional])
				{
					if (GetCachedDiscountCodes().ContainsKey(discountSequence.DiscountID))
					{
						if (GetCachedDiscountCodes()[discountSequence.DiscountID].Type == type)
							unconditionalDiscounts.Add(discountSequence);
					}
					else
					{
						SelectUnconditionalDiscounts(true);
						break;
					}
				}
				return unconditionalDiscounts;
			}
			return new HashSet<DiscountSequenceKey>();
		}

		public static HashSet<DiscountSequenceKey> SelectUnconditionalDiscounts(bool clearCache)
		{
			ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>>>(Messages.Unconditional, typeof(ConcurrentDictionary<object, ImmutableHashSet<DiscountSequenceKey>>));
			if (clearCache)
			{
				cachedDiscountEntities.Clear();
			}

			if (!cachedDiscountEntities.ContainsKey(Messages.Unconditional))
			{
				cachedDiscountEntities.GetOrAdd(Messages.Unconditional, ImmutableHashSet.Create<DiscountSequenceKey>());
				foreach (PXResult<ARDiscount, DiscountSequence> unconditionalDiscount in PXSelectJoin<ARDiscount, InnerJoin<DiscountSequence, On<DiscountSequence.discountID, Equal<ARDiscount.discountID>>>, Where<ARDiscount.applicableTo, Equal<DiscountTarget.unconditional>>>.Select(new PXGraph()))
				{
					DiscountSequence discountSequence = (DiscountSequence)unconditionalDiscount;

					DiscountSequenceKey discountSequenceT = new DiscountSequenceKey();
					discountSequenceT.DiscountID = discountSequence.DiscountID;
					discountSequenceT.DiscountSequenceID = discountSequence.DiscountSequenceID;
					cachedDiscountEntities[Messages.Unconditional] = cachedDiscountEntities[Messages.Unconditional].Add(discountSequenceT);
				}
			}
			return new HashSet<DiscountSequenceKey>(cachedDiscountEntities[Messages.Unconditional]); // Uses ordinal hashset for backward compatibility
        }

        #endregion

        #region Entity-specific functions

        private ImmutableHashSet<DiscountSequenceKey> GetApplicableEntityARDiscounts(KeyValuePair<object, string> entity)
        {
            ImmutableHashSet<DiscountSequenceKey> applicableEntityDiscounts = ImmutableHashSet.Create<DiscountSequenceKey>();
            switch (entity.Value)
            {
                case DiscountTarget.Customer:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountCustomer, DiscountCustomer.customerID, int>((int)entity.Key);
                    break;
                case DiscountTarget.Inventory:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountItem, DiscountItem.inventoryID, int>((int)entity.Key);
                    break;
                case DiscountTarget.CustomerPrice:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountCustomerPriceClass, DiscountCustomerPriceClass.customerPriceClassID, string>((string)entity.Key);
                    break;
                case DiscountTarget.InventoryPrice:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountInventoryPriceClass, DiscountInventoryPriceClass.inventoryPriceClassID, string>((string)entity.Key);
                    break;
                case DiscountTarget.Branch:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountBranch, DiscountBranch.branchID, int>((int)entity.Key);
                    break;
                case DiscountTarget.Warehouse:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountSite, DiscountSite.siteID, int>((int)entity.Key);
                    break;
            }
            return applicableEntityDiscounts;
        }

        private ImmutableHashSet<DiscountSequenceKey> GetApplicableEntityAPDiscounts(KeyValuePair<object, string> entity, int? vendorID)
        {
            ImmutableHashSet<DiscountSequenceKey> applicableEntityDiscounts = ImmutableHashSet.Create<DiscountSequenceKey>();
            switch (entity.Value)
            {
                case DiscountTarget.Inventory:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountItem, DiscountItem.inventoryID, int>((int)entity.Key);
                    break;
                case DiscountTarget.Vendor:
                    applicableEntityDiscounts = SelectEntitytDiscounts<AP.APDiscountVendor, AP.APDiscountVendor.vendorID, int>((int)entity.Key);
                    break;
                case DiscountTarget.InventoryPrice:
                    applicableEntityDiscounts = SelectEntitytDiscounts<DiscountInventoryPriceClass, DiscountInventoryPriceClass.inventoryPriceClassID, string>((string)entity.Key);
                    break;
                case DiscountTarget.VendorLocation:
                    if (vendorID != null)
                        applicableEntityDiscounts = SelectEntityDiscountsWithTwoKeys<AP.APDiscountLocation, AP.APDiscountLocation.vendorID, AP.APDiscountLocation.locationID, int, int>((int)vendorID, (int)entity.Key);
                    break;
            }
            return applicableEntityDiscounts;
        }

        public static ApplicableToCombination SetApplicableToCombination(string applicableTo)
        {
            ApplicableToCombination ApplicableTo = new ApplicableToCombination();
            switch (applicableTo)
            {
                case DiscountTarget.Customer:
                    ApplicableTo = ApplicableToCombination.Customer;
                    break;
                case DiscountTarget.Inventory:
                    ApplicableTo = ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.CustomerPrice:
                    ApplicableTo = ApplicableToCombination.CustomerPriceClass;
                    break;
                case DiscountTarget.InventoryPrice:
                    ApplicableTo = ApplicableToCombination.InventoryPriceClass;
                    break;
                case DiscountTarget.CustomerAndInventory:
                    ApplicableTo = ApplicableToCombination.Customer | ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.CustomerAndInventoryPrice:
                    ApplicableTo = ApplicableToCombination.Customer | ApplicableToCombination.InventoryPriceClass;
                    break;
                case DiscountTarget.CustomerPriceAndInventory:
                    ApplicableTo = ApplicableToCombination.CustomerPriceClass | ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.CustomerPriceAndBranch:
                    ApplicableTo = ApplicableToCombination.CustomerPriceClass | ApplicableToCombination.Branch;
                    break;
                case DiscountTarget.CustomerPriceAndInventoryPrice:
                    ApplicableTo = ApplicableToCombination.CustomerPriceClass | ApplicableToCombination.InventoryPriceClass;
                    break;
                case DiscountTarget.CustomerAndBranch:
                    ApplicableTo = ApplicableToCombination.Customer | ApplicableToCombination.Branch;
                    break;
                case DiscountTarget.Warehouse:
                    ApplicableTo = ApplicableToCombination.Warehouse;
                    break;
                case DiscountTarget.WarehouseAndCustomer:
                    ApplicableTo = ApplicableToCombination.Warehouse | ApplicableToCombination.Customer;
                    break;
                case DiscountTarget.WarehouseAndCustomerPrice:
                    ApplicableTo = ApplicableToCombination.Warehouse | ApplicableToCombination.CustomerPriceClass;
                    break;
                case DiscountTarget.WarehouseAndInventory:
                    ApplicableTo = ApplicableToCombination.Warehouse | ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.WarehouseAndInventoryPrice:
                    ApplicableTo = ApplicableToCombination.Warehouse | ApplicableToCombination.InventoryPriceClass;
                    break;
                case DiscountTarget.Branch:
                    ApplicableTo = ApplicableToCombination.Branch;
                    break;
                case DiscountTarget.Vendor: //Unconditional for Vendor
                    ApplicableTo = ApplicableToCombination.Vendor;
                    break;
                case DiscountTarget.VendorAndInventory:
                    ApplicableTo = ApplicableToCombination.Vendor | ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.VendorAndInventoryPrice:
                    ApplicableTo = ApplicableToCombination.Vendor | ApplicableToCombination.InventoryPriceClass;
                    break;
                case DiscountTarget.VendorLocation:
                    ApplicableTo = ApplicableToCombination.Location;
                    break;
                case DiscountTarget.VendorLocationAndInventory:
                    ApplicableTo = ApplicableToCombination.Location | ApplicableToCombination.InventoryItem;
                    break;
                case DiscountTarget.Unconditional:
                    ApplicableTo = ApplicableToCombination.Unconditional;
                    break;
            }

            return ApplicableTo;
        }

        public static void ClearAllEntityCaches()
        {
            DiscountEngine.SelectUnconditionalDiscounts(true);
            DiscountEngine.ClearAllEntityDiscounts<DiscountCustomer, int>();
            DiscountEngine.ClearAllEntityDiscounts<DiscountItem, int>();
            DiscountEngine.ClearAllEntityDiscounts<DiscountCustomerPriceClass, string>();
            DiscountEngine.ClearAllEntityDiscounts<DiscountInventoryPriceClass, string>();
            DiscountEngine.ClearAllEntityDiscounts<DiscountBranch, int>();
            DiscountEngine.ClearAllEntityDiscounts<DiscountSite, int>();
            DiscountEngine.ClearAllEntityDiscounts<AP.APDiscountVendor, int>();
            DiscountEngine.ClearAllEntityDiscountsTwoKeys<AP.APDiscountLocation, int, int>();
        }

        public static void ClearEntityCaches(PXCache sender, string discountID, string discountSequenceID, string applicableTo, int? vendorID = null)
        {
            DiscountEngine.SelectUnconditionalDiscounts(true);

            ApplicableToCombination applicable = SetApplicableToCombination(applicableTo);

            if ((applicable & ApplicableToCombination.Customer) == ApplicableToCombination.Customer)
            {
                foreach (DiscountCustomer entity in PXSelect<DiscountCustomer, Where<DiscountCustomer.discountID, Equal<Required<DiscountCustomer.discountID>>,
                    And<DiscountCustomer.discountSequenceID, Equal<Required<DiscountCustomer.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountCustomer, int>((int)entity.CustomerID);
                }
            }
            if ((applicable & ApplicableToCombination.InventoryItem) == ApplicableToCombination.InventoryItem)
            {
                foreach (DiscountItem entity in PXSelect<DiscountItem, Where<DiscountItem.discountID, Equal<Required<DiscountItem.discountID>>,
                    And<DiscountItem.discountSequenceID, Equal<Required<DiscountItem.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountItem, int>((int)entity.InventoryID);
                }
            }
            if ((applicable & ApplicableToCombination.CustomerPriceClass) == ApplicableToCombination.CustomerPriceClass)
            {
                foreach (DiscountCustomerPriceClass entity in PXSelect<DiscountCustomerPriceClass, Where<DiscountCustomerPriceClass.discountID, Equal<Required<DiscountCustomerPriceClass.discountID>>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<Required<DiscountCustomerPriceClass.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountCustomerPriceClass, string>((string)entity.CustomerPriceClassID);
                }
            }
            if ((applicable & ApplicableToCombination.InventoryPriceClass) == ApplicableToCombination.InventoryPriceClass)
            {
                foreach (DiscountInventoryPriceClass entity in PXSelect<DiscountInventoryPriceClass, Where<DiscountInventoryPriceClass.discountID, Equal<Required<DiscountInventoryPriceClass.discountID>>,
                    And<DiscountInventoryPriceClass.discountSequenceID, Equal<Required<DiscountInventoryPriceClass.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountInventoryPriceClass, string>((string)entity.InventoryPriceClassID);
                }
            }
            if ((applicable & ApplicableToCombination.Branch) == ApplicableToCombination.Branch)
            {
                foreach (DiscountBranch entity in PXSelect<DiscountBranch, Where<DiscountBranch.discountID, Equal<Required<DiscountBranch.discountID>>,
                    And<DiscountBranch.discountSequenceID, Equal<Required<DiscountBranch.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountBranch, int>((int)entity.BranchID);
                }
            }
            if ((applicable & ApplicableToCombination.Warehouse) == ApplicableToCombination.Warehouse)
            {
                foreach (DiscountSite entity in PXSelect<DiscountSite, Where<DiscountSite.discountID, Equal<Required<DiscountSite.discountID>>,
                    And<DiscountSite.discountSequenceID, Equal<Required<DiscountSite.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<DiscountSite, int>((int)entity.SiteID);
                }
            }
            if (vendorID != null && (applicable & ApplicableToCombination.Vendor) == ApplicableToCombination.Vendor)
            {
                foreach (AP.APDiscountVendor entity in PXSelect<AP.APDiscountVendor, Where<AP.APDiscountVendor.discountID, Equal<Required<AP.APDiscountVendor.discountID>>,
                    And<AP.APDiscountVendor.discountSequenceID, Equal<Required<AP.APDiscountVendor.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscounts<AP.APDiscountVendor, int>((int)entity.VendorID);
                }
            }
            if (vendorID != null && (applicable & ApplicableToCombination.Location) == ApplicableToCombination.Location)
            {
                foreach (AP.APDiscountLocation entity in PXSelect<AP.APDiscountLocation, Where<AP.APDiscountLocation.discountID, Equal<Required<AP.APDiscountLocation.discountID>>,
                    And<AP.APDiscountLocation.discountSequenceID, Equal<Required<AP.APDiscountLocation.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
                {
                    DiscountEngine.ClearEntityDiscountsTwoKeys<AP.APDiscountLocation, int, int>((int)entity.VendorID, (int)entity.LocationID);
                }
            }
        }

        #endregion

        #region Selects

        /// <summary>
        /// Returns single best available discount
        /// </summary>
        /// <param name="discountSequences">Applicable Discount Sequences</param>
        /// <param name="amount">Discountable amount</param>
        /// <param name="quantity">Discountable quantity</param>
        /// <param name="discountFor">Discounted for: amount, percent or free item</param>
        public virtual DiscountDetailLine SelectSingleBestDiscount(PXCache sender, DiscountLineFields dline, List<DiscountSequenceKey> discountSequences, decimal amount, decimal quantity, string discountFor, DateTime date)
        {
            if (discountSequences == null || discountSequences.Count() == 0)
            {
                return new DiscountDetailLine();
            }
            PXView bestDiscountView = GetDiscountSelectCommand(discountSequences.Count(), false);

            PXResult<DiscountSequence, DiscountSequenceDetail> bestDiscount = (PXResult<DiscountSequence, DiscountSequenceDetail>)bestDiscountView.SelectSingle(GetRequiredDiscountParams(discountSequences, amount, quantity, discountFor, date));
            DiscountSequence sequence = (DiscountSequence)bestDiscount;
            if (sequence != null && ((sequence.BreakBy == BreakdownType.Amount && amount == 0m) || (sequence.BreakBy == BreakdownType.Quantity && quantity == 0m)))
                return new DiscountDetailLine();
            else
                return CreateDiscountDetails(sender, dline, bestDiscount, date);
        }

        /// <summary>
        /// Returns list of all applicable discounts
        /// </summary>
        /// <param name="discountSequences">Applicable Discount Sequences</param>
        /// <param name="amount">Discountable amount</param>
        /// <param name="quantity">Discountable quantity</param>
        public virtual List<DiscountDetailLine> SelectAllApplicableDiscounts(PXCache sender, DiscountLineFields dline, List<DiscountSequenceKey> discountSequences, decimal amount, decimal quantity, DateTime date)
        {
            List<DiscountDetailLine> applicableDiscountDetails = new List<DiscountDetailLine>();
            if (discountSequences == null || discountSequences.Count() == 0)
            {
                return applicableDiscountDetails; // return null?
            }
            PXView bestDiscountView = GetDiscountSelectCommand(discountSequences.Count(), true);
            foreach (PXResult<DiscountSequence, DiscountSequenceDetail> bestDiscount in bestDiscountView.SelectMulti(GetRequiredDiscountParams(discountSequences, amount, quantity, string.Empty, date)))
            {
                DiscountSequence sequence = (DiscountSequence)bestDiscount;
                if (sequence != null && !((sequence.BreakBy == BreakdownType.Amount && amount == 0m) || (sequence.BreakBy == BreakdownType.Quantity && quantity == 0m)))
                {
                    int existingLineIndex = applicableDiscountDetails.FindIndex(x => x.DiscountID == sequence.DiscountID && x.DiscountSequenceID == sequence.DiscountSequenceID && x.DiscountedFor == sequence.DiscountedFor);
                    DiscountDetailLine newLine = CreateDiscountDetails(sender, dline, bestDiscount, date);
                    if (existingLineIndex < 0)
                        applicableDiscountDetails.Add(newLine);
                    else if (applicableDiscountDetails[existingLineIndex].Discount < newLine.Discount)
                        applicableDiscountDetails[existingLineIndex] = newLine;
                }
            }
            return applicableDiscountDetails;
        }

        //GetDiscountSelectCommand and GetRequiredDiscountParams should be synchronized
        private PXView GetDiscountSelectCommand(int numOfDiscountSequences, bool isGroup)
        {
            if (numOfDiscountSequences == 0)
            {
                return null;
            }
            List<Type> discountTypes = new List<Type>();
            discountTypes.Add(typeof(Select2<,,,>));
            discountTypes.Add(typeof(DiscountSequence));
            discountTypes.Add(typeof(LeftJoin<,>));
            discountTypes.Add(typeof(DiscountSequenceDetail));
            discountTypes.Add(typeof(On<DiscountSequenceDetail.discountID, Equal<DiscountSequence.discountID>, And<DiscountSequenceDetail.discountSequenceID, Equal<DiscountSequence.discountSequenceID>>>));
            discountTypes.Add(typeof(Where2<,>));
            discountTypes.Add(typeof(Where<Where2<Where<
                //Current discount
                DiscountSequence.breakBy, Equal<Required<DiscountSequence.breakBy>>,
                And<DiscountSequenceDetail.amount, LessEqual<Required<DiscountSequenceDetail.amount>>,
                And<DiscountSequenceDetail.amountTo, Greater<Required<DiscountSequenceDetail.amountTo>>,

                Or<DiscountSequence.breakBy, Equal<Required<DiscountSequence.breakBy>>,
                And<DiscountSequenceDetail.quantity, LessEqual<Required<DiscountSequenceDetail.quantity>>,
                And<DiscountSequenceDetail.quantityTo, Greater<Required<DiscountSequenceDetail.quantityTo>>,

                Or<Where2<Where<DiscountSequence.breakBy, Equal<Required<DiscountSequence.breakBy>>,
                And<Where2<Where<DiscountSequenceDetail.amountTo, IsNull,
                Or<DiscountSequenceDetail.amountTo, Equal<decimal0>>>,
                And<DiscountSequenceDetail.amount, LessEqual<Required<DiscountSequenceDetail.amount>>>>>>,

                Or<Where<DiscountSequence.breakBy, Equal<Required<DiscountSequence.breakBy>>,
                And<Where2<Where<DiscountSequenceDetail.quantityTo, IsNull,
                Or<DiscountSequenceDetail.quantityTo, Equal<decimal0>>>,
                And<DiscountSequenceDetail.quantity, LessEqual<Required<DiscountSequenceDetail.quantity>>>>>>>>>>>>>>>,

                And<Where<DiscountSequence.isActive, Equal<True>,
                And<Where2<Where<DiscountSequence.isPromotion, Equal<False>, 
                And<Where2<Where<DiscountSequenceDetail.lastDate, LessEqual<Required<DiscountSequenceDetail.lastDate>>, And<DiscountSequenceDetail.isLast, Equal<False>>>, Or<Where<DiscountSequenceDetail.lastDate, Greater<Required<DiscountSequenceDetail.lastDate>>, And<DiscountSequenceDetail.isLast, Equal<True>>>>>>>,
                Or<Where<DiscountSequence.isPromotion, Equal<True>,
                And<DiscountSequence.startDate, LessEqual<Required<DiscountSequence.startDate>>,
                And<DiscountSequence.endDate, GreaterEqual<Required<DiscountSequence.endDate>>>>>>>>>>>>));


            //discounted for (amount or percent, non-group only)
            if (!isGroup)
            {
                discountTypes.Add(typeof(And<,,>));
                discountTypes.Add(typeof(DiscountSequence.discountedFor));
                discountTypes.Add(typeof(Equal<Required<DiscountSequence.discountedFor>>));
            }
            discountTypes.Add(typeof(And<>));
            discountTypes.Add(typeof(Where<,,>));

            for (int i = 0; i < numOfDiscountSequences; i++)
            {
                discountTypes.Add(typeof(DiscountSequenceDetail.discountID));
                discountTypes.Add(typeof(Equal<Required<DiscountSequenceDetail.discountID>>));
                discountTypes.Add((i != numOfDiscountSequences - 1) ? typeof(And<,,>) : typeof(And<,>));
                discountTypes.Add(typeof(DiscountSequenceDetail.discountSequenceID));
                discountTypes.Add(typeof(Equal<Required<DiscountSequenceDetail.discountSequenceID>>));
                if (i != numOfDiscountSequences - 1) discountTypes.Add(typeof(Or<,,>));
            }
            discountTypes.Add(typeof(OrderBy<Desc<DiscountSequenceDetail.discount>>));


            return new PXView(new PXGraph(), false, BqlCommand.CreateInstance(BqlCommand.Compose(discountTypes.ToArray())));
        }

        //GetDiscountSelectCommand and GetRequiredDiscountParams should be synchronized
        public virtual object[] GetRequiredDiscountParams(List<DiscountSequenceKey> discountSequences, decimal amount, decimal quantity, string discountFor, DateTime date)
        {
            List<Object> requiredParams = new List<object>();

            //Current discount
            requiredParams.Add(BreakdownType.Amount);
            requiredParams.Add(Math.Abs(amount));
            requiredParams.Add(Math.Abs(amount));
            requiredParams.Add(BreakdownType.Quantity);
            requiredParams.Add(Math.Abs(quantity));
            requiredParams.Add(Math.Abs(quantity));

            requiredParams.Add(BreakdownType.Amount);
            requiredParams.Add(Math.Abs(amount));
            requiredParams.Add(BreakdownType.Quantity);
            requiredParams.Add(Math.Abs(quantity));
            requiredParams.Add(date);
            requiredParams.Add(date);
            requiredParams.Add(date);

            requiredParams.Add(date);


            if (discountFor != string.Empty) requiredParams.Add(discountFor);
            for (int i = 0; i < discountSequences.Count(); i++)
            {
                requiredParams.Add(discountSequences[i].DiscountID);
                requiredParams.Add(discountSequences[i].DiscountSequenceID);
            }
            return requiredParams.ToArray();
        }

		protected ARDiscountSequenceMaint arDiscountSequence;

        public virtual ImmutableHashSet<DiscountSequenceKey> SelectEntitytDiscounts<Table, EntityField, EntityType>(EntityType entityID)
            where Table : IBqlTable
            where EntityField : IBqlField
        {
			ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<EntityType, ImmutableHashSet<DiscountSequenceKey>>));

			if (!cachedDiscountEntities.ContainsKey(entityID))
			{
				cachedDiscountEntities.GetOrAdd(entityID, ImmutableHashSet.Create<DiscountSequenceKey>());

				IEnumerable entityDiscounts = PXDatabase.SelectMulti<Table>(
					new PXDataField("DiscountID"),
					new PXDataField("DiscountSequenceID"),
					new PXDataField<EntityField>(),
					new PXDataFieldValue(typeof(EntityField).Name, typeof(EntityType) == typeof(Int32) ? PXDbType.Int : PXDbType.NVarChar, entityID));

				var parametersD = new List<object> { };
				var parametersS = new List<object> { };
				List<KeyValuePair<object, object>> validCombinations = new List<KeyValuePair<object, object>>();

				foreach (PXDataRecord discountEntity in entityDiscounts)
				{
					parametersD.Add(discountEntity.GetString(0));
					parametersS.Add(discountEntity.GetString(1));
					validCombinations.Add(new KeyValuePair<object, object>(discountEntity.GetString(0), discountEntity.GetString(1)));
				}

				if (parametersD != null && parametersD.Count > 0)
				{
					BqlCommand select = new Select<DiscountSequence,
					Where<DiscountSequence.isActive, Equal<True>>>();
					select = select.WhereAnd(Data.InHelper<DiscountSequence.discountID>.Create(parametersD.Count));
					select = select.WhereAnd(Data.InHelper<DiscountSequence.discountSequenceID>.Create(parametersS.Count));

					if (arDiscountSequence == null)
						arDiscountSequence = PXGraph.CreateInstance<ARDiscountSequenceMaint>();

					foreach (DiscountSequence seq in new PXView(arDiscountSequence, true, select).
							SelectMulti(parametersD.Concat(parametersS).ToArray()))
					{
						if (IsValidDiscountToSequenceCombination(validCombinations, seq.DiscountID, seq.DiscountSequenceID))
						{
							DiscountSequenceKey discountSequence = new DiscountSequenceKey();
							discountSequence.DiscountID = seq.DiscountID;
							discountSequence.DiscountSequenceID = seq.DiscountSequenceID;
							cachedDiscountEntities[entityID] = cachedDiscountEntities[entityID].Add(discountSequence);
						}
					}
				}
			}
            return cachedDiscountEntities[entityID];
        }

		protected AP.APDiscountSequenceMaint apDiscountSequence;

        public virtual ImmutableHashSet<DiscountSequenceKey> SelectEntityDiscountsWithTwoKeys<Table, KeyField1, KeyField2, EntityType1, EntityType2>(EntityType1 entityID1, EntityType2 entityID2)
            where Table : IBqlTable
            where KeyField1 : IBqlField
            where KeyField2 : IBqlField
        {
			ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>> cachedDiscountEntities = PXDatabase.GetSlot<ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>>(typeof(Table).Name, typeof(ConcurrentDictionary<KeyValuePair<EntityType1, EntityType2>, ImmutableHashSet<DiscountSequenceKey>>));
            KeyValuePair<EntityType1, EntityType2> entityKey = new KeyValuePair<EntityType1, EntityType2>(entityID1, entityID2);

			if (!cachedDiscountEntities.ContainsKey(entityKey))
			{
				cachedDiscountEntities.GetOrAdd(entityKey, ImmutableHashSet.Create<DiscountSequenceKey>());

				IEnumerable entityDiscounts = PXDatabase.SelectMulti<Table>(
					new PXDataField("DiscountID"),
					new PXDataField("DiscountSequenceID"),
					new PXDataField<KeyField1>(),
					new PXDataField<KeyField2>(),
					new PXDataFieldValue(typeof(KeyField1).Name, typeof(EntityType1) == typeof(Int32) ? PXDbType.Int : PXDbType.NVarChar, entityID1),
					new PXDataFieldValue(typeof(KeyField2).Name, typeof(EntityType2) == typeof(Int32) ? PXDbType.Int : PXDbType.NVarChar, entityID2));

				var parametersD = new List<object> { };
				var parametersS = new List<object> { };
				List<KeyValuePair<object, object>> validCombinations = new List<KeyValuePair<object, object>>();

				foreach (PXDataRecord discountEntity in entityDiscounts)
				{
					parametersD.Add(discountEntity.GetString(0));
					parametersS.Add(discountEntity.GetString(1));
					validCombinations.Add(new KeyValuePair<object, object>(discountEntity.GetString(0), discountEntity.GetString(1)));
				}
				if (parametersD != null && parametersD.Count > 0)
				{
					BqlCommand select = new Select<DiscountSequence,
					Where<DiscountSequence.isActive, Equal<True>>>();
					select = select.WhereAnd(Data.InHelper<DiscountSequence.discountID>.Create(parametersD.Count));
					select = select.WhereAnd(Data.InHelper<DiscountSequence.discountSequenceID>.Create(parametersS.Count));

					if (apDiscountSequence == null)
						apDiscountSequence = PXGraph.CreateInstance<AP.APDiscountSequenceMaint>();

					foreach (DiscountSequence seq in new PXView(apDiscountSequence, true, select).
							SelectMulti(parametersD.Concat(parametersS).ToArray()))
					{
						if (IsValidDiscountToSequenceCombination(validCombinations, seq.DiscountID, seq.DiscountSequenceID))
						{
							DiscountSequenceKey discountSequence = new DiscountSequenceKey();
							discountSequence.DiscountID = seq.DiscountID;
							discountSequence.DiscountSequenceID = seq.DiscountSequenceID;
							cachedDiscountEntities[entityKey] = cachedDiscountEntities[entityKey].Add(discountSequence);
						}
					}
				}
			}
            return cachedDiscountEntities[entityKey];
        }

		private bool IsValidDiscountToSequenceCombination(List<KeyValuePair<object, object>> validCombinations, string discountID, string discountSequenceID)
		{
			foreach (KeyValuePair<object, object> validCombination in validCombinations)
			{
				if (validCombination.Key.ToString() == discountID && validCombination.Value.ToString() == discountSequenceID)
					return true;
			}
			return false;
		}

        public virtual KeyValuePair<DiscountSequenceKey, HashSet<object>> SelectDiscountEntities<Table, EntityField, EntityType>(KeyValuePair<DiscountSequenceKey, HashSet<object>> discountEntities)
            where Table : IBqlTable
            where EntityField : IBqlField
        {
            HashSet<object> entites = new HashSet<object>();
            entites.Add(discountEntities.Value);

            foreach (PXDataRecord discountEntity in PXDatabase.SelectMulti<Table>(
                new PXDataField<EntityField>(),
                new PXDataFieldValue("DiscountID", PXDbType.NVarChar, discountEntities.Key.DiscountID),
                new PXDataFieldValue("DiscountSequenceID", PXDbType.NVarChar, discountEntities.Key.DiscountSequenceID)))
            {
                if (typeof(EntityType) == typeof(Int32))
                {
                    entites.Add(discountEntity.GetInt32(0));
                }
                else
                {
                    entites.Add(discountEntity.GetString(0));
                }
            }
            return new KeyValuePair<DiscountSequenceKey, HashSet<object>>(discountEntities.Key, entites);
        }

        public virtual HashSet<DiscountSequenceKey> SelectDiscountSequences(string discountID)
        {
            HashSet<DiscountSequenceKey> sequences = new HashSet<DiscountSequenceKey>();

            foreach (PXDataRecord discountSequence in PXDatabase.SelectMulti<DiscountSequence>(
                new PXDataField<DiscountSequence.discountSequenceID>(),
                new PXDataFieldValue("DiscountID", PXDbType.NVarChar, discountID)))
            {
                DiscountSequenceKey sequence = new DiscountSequenceKey();
                sequence.DiscountID = discountID;
                sequence.DiscountSequenceID = discountSequence.GetString(0);
				sequences.Add(sequence);
            }
            return sequences;
        }

        public static DiscountSequence SelectDiscountSequence(PXCache sender, string discountID, string discountSequenceID)
        {
            foreach (DiscountSequence sequence in PXSelect<DiscountSequence, Where<DiscountSequence.discountID, Equal<Required<DiscountSequence.discountID>>, And<DiscountSequence.discountSequenceID, Equal<Required<DiscountSequence.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID))
            {
                return sequence;
            }
            return new DiscountSequence();
        }

        protected static InventoryItem GetInventoryItemByID(PXCache sender, int? inventoryID)
        {
            return PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
        }

        public static Location GetLocationByLocationID(PXCache sender, int? bAccountID, int? locationID)
        {
			return PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
						And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(sender.Graph, bAccountID, locationID);
        }

        public static decimal GetDiscountLimit(PXCache sender, int? customerID, int? vendorID = null)
        {
            decimal discountLimit = 100m;
            if (customerID != null)
            {
                foreach (PXResult<CustomerClass, Customer> cClass in PXSelectJoin<CustomerClass, LeftJoinSingleTable<Customer, On<Customer.customerClassID, Equal<CustomerClass.customerClassID>>>, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.SelectWindowed(sender.Graph, 0, 1, customerID))
                    if (cClass != null)
                    {
                        discountLimit = ((CustomerClass)cClass).DiscountLimit ?? 100m;
                    }
            }
            else if (vendorID != null)
                discountLimit = (decimal)Math.Pow((double)discountLimit, 3);
            return discountLimit;
        }

        public static decimal GetTotalGroupAndDocumentDiscount<DiscountDetail>(PXSelectBase<DiscountDetail> discountDetails, bool docOnly = false)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            decimal total = 0;
            foreach (DiscountDetail record in discountDetails.Select())
            {
                if (record.SkipDiscount != true && ((docOnly && record.Type == DiscountType.Document) || !docOnly))
                    total += record.CuryDiscountAmt ?? 0;
            }
            return total;
        }

        #region Pricing part
        //Returns best ARSalesPrice
        public ARSalesPrice GetARPrice(PXCache sender, int inventoryID, int customerID, string customerPriceClassID, string curyID, string UOM, decimal quantity, DateTime date)
        {
            ARSalesPrice salesPrice = PXSelect<ARSalesPrice, Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>, 
            And<ARSalesPrice.curyID, Equal<Required<ARSalesPrice.curyID>>,
            And<ARSalesPrice.uOM, Equal<Required<ARSalesPrice.uOM>>,
            And<ARSalesPrice.breakQty, LessEqual<Required<ARSalesPrice.breakQty>>,

            And2<Where2<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>, 
                And<ARSalesPrice.isPromotionalPrice, Equal<False>>>,
            Or<Where<ARSalesPrice.effectiveDate, LessEqual<Required<ARSalesPrice.effectiveDate>>, 
                And<ARSalesPrice.expirationDate, GreaterEqual<Required<ARSalesPrice.expirationDate>>, And<ARSalesPrice.isPromotionalPrice, Equal<True>>>>>>,

            And<Where<ARSalesPrice.customerID, Equal<Required<ARSalesPrice.customerID>>>>>>>>>,
                OrderBy<Desc<ARSalesPrice.priceCode, Desc<ARSalesPrice.isPromotionalPrice, Desc<ARSalesPrice.breakQty>>>>>
                    .SelectWindowed(sender.Graph, 0, 1, inventoryID, curyID, UOM, quantity, date, date, date, customerID);
            return salesPrice;
        }

        //Returns best APVendorPrice
        public AP.APVendorPrice GetAPPrice(PXCache sender, int inventoryID, int vendorID, string curyID, string UOM, decimal quantity, DateTime date)
        {
            AP.APVendorPrice salesPrice = PXSelect<AP.APVendorPrice, Where<AP.APVendorPrice.inventoryID, Equal<Required<AP.APVendorPrice.inventoryID>>,
            And<AP.APVendorPrice.curyID, Equal<Required<AP.APVendorPrice.curyID>>,
            And<AP.APVendorPrice.uOM, Equal<Required<AP.APVendorPrice.uOM>>,
            And<AP.APVendorPrice.breakQty, LessEqual<Required<AP.APVendorPrice.breakQty>>,

            And2<Where2<Where<AP.APVendorPrice.effectiveDate, LessEqual<Required<AP.APVendorPrice.effectiveDate>>,
                And<AP.APVendorPrice.isPromotionalPrice, Equal<False>>>,
            Or<Where<AP.APVendorPrice.effectiveDate, LessEqual<Required<AP.APVendorPrice.effectiveDate>>,
                And<AP.APVendorPrice.expirationDate, GreaterEqual<Required<AP.APVendorPrice.expirationDate>>, And<AP.APVendorPrice.isPromotionalPrice, Equal<True>>>>>>,

            And<Where<AP.APVendorPrice.vendorID, Equal<Required<AP.APVendorPrice.vendorID>>>>>>>>>,
                OrderBy<Desc<AP.APVendorPrice.vendorID, Desc<AP.APVendorPrice.isPromotionalPrice, Desc<AP.APVendorPrice.breakQty>>>>>
                    .SelectWindowed(sender.Graph, 0, 1, inventoryID, curyID, UOM, quantity, date, date, date, vendorID);
            return salesPrice;
        }
        #endregion

        #endregion

        public class DiscountDetailToLineCorrelation<DiscountDetail, Line>
            where Line : class, IBqlTable, new()
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            public DiscountDetail discountDetailLine { get; set; }
            public List<Line> listOfApplicableLines { get; set; }
        }

	}

	public static class ConcurrentDictionaryEx
	{
		public static bool Remove<TKey, TValue>(
		  this ConcurrentDictionary<TKey, TValue> self, TKey key)
		{
			return ((IDictionary<TKey, TValue>)self).Remove(key);
		}
	}

	public class DiscountEngine<Line> : DiscountEngine
        where Line : class, IBqlTable, new()
    {
        public PXSetup<ARSetup> arsetup;

        #region BqlFields
        private abstract class discPct : IBqlField { }
        private abstract class curyDiscAmt : IBqlField { }

        private abstract class inventoryID : IBqlField { }
        private abstract class customerID : IBqlField { }
        private abstract class siteID : IBqlField { }
        private abstract class branchID : IBqlField { }
        private abstract class vendorID : IBqlField { }

        private abstract class orderQty : IBqlField { }
        private abstract class receiptQty : IBqlField { }
        private abstract class curyUnitPrice : IBqlField { }
        private abstract class curyUnitCost : IBqlField { }
        private abstract class curyExtPrice : IBqlField { }
        private abstract class curyExtCost : IBqlField { }
        private abstract class curyLineAmt : IBqlField { }
        private abstract class shippedQty : IBqlField { }
        private abstract class groupDiscountRate : IBqlField { }
        private abstract class documentDiscountRate : IBqlField { }
        private abstract class uOM : IBqlField { }

        private abstract class curyTranAmt : IBqlField { }

        private abstract class baseQty : IBqlField { }
        private abstract class qty : IBqlField { }

        private abstract class skipDisc : IBqlField { }
        private abstract class discountID : IBqlField { }
        private abstract class discountSequenceID : IBqlField { }
        private abstract class manualDisc : IBqlField { }
        private abstract class manualPrice : IBqlField { }
        private abstract class lineType : IBqlField { }
        private abstract class isFree : IBqlField { }

        #endregion

        #region Set Line/Group/Document discounts functions
        /// <summary>
        /// Sets best available discount for a given line. Recalculates all group discounts. Sets best available document discount.
        /// </summary>
        /// <typeparam name="DiscountDetail">DiscountDetails table</typeparam>
        /// <param name="lines">Transaction lines</param>
        /// <param name="line">Current line</param>
        /// <param name="discountDetails">Discount Details</param>
        /// <param name="locationID">Customer or Vendor LocationID</param>
        /// <param name="date">Date</param>
        public static void SetDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line line, PXSelectBase<DiscountDetail> discountDetails, int? branchID, int? locationID, string curyID, DateTime? date, bool skipFreeItems = false, RecalcDiscountsParamFilter recalcFilter = null)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {           
            if (branchID == null || locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
            UpdateEntitytCache();
            ARSetup arsetup = ARSetupSelect.Select(sender.Graph);
            if ((!sender.Graph.IsImport || sender.Graph.IsMobile) && (arsetup.IgnoreDiscountsIfPriceDefined == false || (arsetup.IgnoreDiscountsIfPriceDefined == true && !GetUnitPrice(sender, line, locationID, curyID, date.Value).isBAccountSpecific)))
            {
                SetLineDiscount(sender, GetDiscountEntitiesDiscounts(sender, line, locationID, true), line, date.Value, recalcFilter);
            }

            RecalculateGroupAndDocumentDiscounts(sender, lines, line, discountDetails, branchID, locationID, date, skipFreeItems);
        }

        public static void SetLineDiscountOnly(PXCache sender, Line line, DiscountLineFields dline, string DiscountID, decimal? unitPrice, decimal? extPrice, decimal? qty, int? locationID, int? customerID, string curyID, DateTime? date, int? branchID = null, int? inventoryID = null,  bool needDiscountID = true)
        {
            if (locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
            UpdateEntitytCache();
            ARSetup arsetup = ARSetupSelect.Select(sender.Graph);
            if (arsetup.IgnoreDiscountsIfPriceDefined == false || (arsetup.IgnoreDiscountsIfPriceDefined == true && !GetUnitPrice(sender, line, locationID, curyID, date.Value).isBAccountSpecific))
            {
                HashSet<KeyValuePair<object, string>> entities = GetDiscountEntitiesDiscounts(sender, line, locationID, true, branchID, inventoryID, customerID);

                DiscountEngine.GetDiscountTypes();

                if (extPrice != null && qty != null)
                {
                    DiscountEngine de = new DiscountEngine();

                    HashSet<DiscountSequenceKey> discountSequencesByDiscountID = de.SelectDiscountSequences(DiscountID);
                    HashSet<DiscountSequenceKey> applicableDiscountSequences = de.SelectApplicableEntitytDiscounts(entities, DiscountType.Line, false);

                    if (needDiscountID)
						applicableDiscountSequences.IntersectWith(discountSequencesByDiscountID);

                    decimal discountTargetPrice = 0m;
                    if (GetLineDiscountTarget(sender, line) == LineDiscountTargetType.SalesPrice)
                        discountTargetPrice = (decimal)unitPrice;
                    else
                        discountTargetPrice = (decimal)extPrice;

                    DiscountDetailLine discount = de.SelectApplicableDiscount(sender, dline, applicableDiscountSequences, discountTargetPrice, (decimal)qty, DiscountType.Line, date.Value);

                    if (discount.DiscountID != null)
                    {
                        decimal curyDiscountAmt = CalculateDiscount(sender, discount, dline, discountTargetPrice, (decimal)qty, date.Value, DiscountType.Line);

                        DiscountResult dResult = new DiscountResult(discount.DiscountedFor == "A" ? curyDiscountAmt : discount.Discount, discount.DiscountedFor == "A" ? true : false);

                        ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);
                        dline.DiscountID = discount.DiscountID;
                        dline.DiscountSequenceID = discount.DiscountSequenceID;
                        dline.RaiseFieldUpdated<DiscountLineFields.discountSequenceID>(null);
                    }
                    else
                    {
                        DiscountResult dResult = new DiscountResult(0m, true);

                        ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Recalculate Prices and Discounts action. Recalculates line discounts. Recalculates all group discounts. Sets best available document discount.
        /// </summary>
        /// <typeparam name="DiscountDetail">DiscountDetails table</typeparam>
        /// <param name="recalcFilter">Current RecalcDiscountsParamFilter</param>
        public static void RecalculatePricesAndDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime? date, RecalcDiscountsParamFilter recalcFilter)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            try
            {
                UpdateEntitytCache(); 
                recalcFilter.UseRecalcFilter = true;
                if (recalcFilter.RecalcTarget == RecalcDiscountsParamFilter.AllLines)
                {
                    List<Line> documentDetails = GetDocumentDetails(sender, lines);
                    foreach (Line line in documentDetails)
                    {
                        RecalculatePricesAndDiscountsOnLine(sender, line, recalcFilter);
                    }
                }
                else
                {
                    if (currentLine != null)
                    {
                        RecalculatePricesAndDiscountsOnLine(sender, currentLine, recalcFilter);
                    }
                    else
                        throw new PXException(Messages.NoLineSelected);
                }
            }
            finally
            {
                recalcFilter.UseRecalcFilter = false;
            }
        }

        /// <summary>
        /// Recalculate Prices and Discounts with predefined parameters
        /// </summary>
        /// <typeparam name="DiscountDetail">DiscountDetails table</typeparam>
        public static void AutoRecalculatePricesAndDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime? date, bool recalculatePrices = true)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            if (locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
            RecalcDiscountsParamFilter recalcFilter = new RecalcDiscountsParamFilter();
            recalcFilter.RecalcTarget = RecalcDiscountsParamFilter.AllLines;
            recalcFilter.OverrideManualDiscounts = false;
            recalcFilter.OverrideManualPrices = false;
            recalcFilter.RecalcDiscounts = true;
            recalcFilter.RecalcUnitPrices = recalculatePrices;
            RecalculatePricesAndDiscounts<DiscountDetail>(sender, lines, currentLine, discountDetails, locationID, date, recalcFilter);
        }

        public static void RecalculatePricesAndDiscountsOnLine(PXCache sender, Line line, RecalcDiscountsParamFilter recalcFilter)
        {
            Line oldLine = (Line)sender.CreateCopy(line);
            DiscountLineFields dFields = GetDiscountedLine(sender, line);
            DiscountLineFields oldDFields = GetDiscountedLine(sender, oldLine);
            AmountLineFields lineAmountsFields = GetDiscountDocumentLine(sender, line);
            LineEntitiesFields lineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);
            if ((bool)recalcFilter.OverrideManualPrices)
                dFields.ManualPrice = false;
            //to unify
            if (lineEntities.InventoryID != null && dFields.ManualPrice != true 
                && (recalcFilter.RecalcUnitPrices == true || (lineAmountsFields.CuryUnitPrice == 0m && lineAmountsFields.CuryExtPrice == 0m && recalcFilter.OverrideManualPrices == false)))
            {
                if (line.GetType() == typeof(SOLine))
                    sender.RaiseFieldUpdated(lineAmountsFields.GetField<AmountLineFields.quantity>().Name, line, 0m);
                else
                {
                    sender.RaiseFieldUpdated(lineAmountsFields.GetField<AmountLineFields.curyUnitPrice>().Name, line, 0m);
                    sender.SetDefaultExt(line, lineAmountsFields.GetField<AmountLineFields.curyUnitPrice>().Name);
                }
            }
            if ((bool)recalcFilter.RecalcDiscounts)
            {
                if (recalcFilter.OverrideManualDiscounts == true) dFields.ManualDisc = false;
                if (dFields.ManualDisc != true) dFields.DiscountID = null;
                if (oldDFields.DiscountID == null) oldDFields.DiscountID = string.Empty;
            }
            sender.IsDirty = true;
            sender.RaiseRowUpdated(line, oldLine);
            if (sender.GetStatus(line) == PXEntryStatus.Notchanged)
                sender.SetStatus(line, PXEntryStatus.Updated);
        }

        /// <summary>
        /// Recalculates all group discounts. Sets best available document discount
        /// </summary>
        /// <typeparam name="DiscountDetail">DiscountDetail table</typeparam>
        public static void RecalculateGroupAndDocumentDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, int? branchID, int? locationID, DateTime? date, bool skipFreeItems = false)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            if (branchID == null || locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
            UpdateEntitytCache(); 
            if (!SetGroupDiscounts(sender, lines, currentLine, discountDetails, locationID, date.Value, skipFreeItems))
            {
				SetDocumentDiscount(sender, lines, discountDetails, currentLine, branchID, locationID, date.Value);
            }
        }

        //Recalculates all discounts for a given line only
        public static void RecalculateDiscountsLine<DiscountDetail>(PXCache sender, PXSelectBase<Line> allLines, Line newLine, Line oldLine, PXSelectBase<DiscountDetail> discountDetails, int? branchID, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            UpdateEntitytCache(); 
            
            SetLineDiscount(sender, GetDiscountEntitiesDiscounts(sender, newLine, locationID, true), newLine, date);

            UpdateGroupDiscountsOnGivenLine(sender, newLine, oldLine, discountDetails, locationID, date);

            SetDocumentDiscount(sender, allLines, discountDetails, null, branchID, locationID, date);
        }

        /// <summary>
        /// Sets selected manual line discount.
        /// </summary>
        public static void UpdateManualLineDiscount<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line line, PXSelectBase<DiscountDetail> discountDetails, int? branchID, int? locationID, DateTime? date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            if (branchID == null || locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;

            UpdateEntitytCache(); 

            SetManualLineDiscount(sender, GetDiscountEntitiesDiscounts(sender, line, locationID, true), line, date.Value);

            RecalculateGroupAndDocumentDiscounts(sender, lines, line, discountDetails, branchID, locationID, date.Value);
        }

        /// <summary>
        /// Updates document discount.
        /// </summary>
		public static void UpdateDocumentDiscount<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, PXSelectBase<DiscountDetail> discountDetails, int? branchID, int? locationID, DateTime? date, bool recalcDocDiscount)
			where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			if (branchID == null || locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
			UpdateEntitytCache();
			if (recalcDocDiscount)
			{
				AdjustGroupDiscountRates(sender, lines, discountDetails, locationID, date.Value);

				SetDocumentDiscount(sender, lines, discountDetails, null, branchID, locationID, date.Value);

				discountDetails.View.RequestRefresh();
			}
			else
				CalculateDocumentDiscountRate(sender, GetDocumentDetails(sender, lines), null, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Document), false);
		}

        /// <summary>
        /// Inserts manual group or document discount.
        /// </summary>
        public static void InsertDocGroupDiscount<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, PXSelectBase<DiscountDetail> discountDetails, DiscountDetail currentDiscountDetailLine, string DiscountID, string DiscountSequenceID, int? branchID, int? locationID, DateTime? date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            if (branchID == null || locationID == null || date == null || !PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>()) return;
            UpdateEntitytCache(); 
            SetManualGroupDocDiscount(sender, lines, null, discountDetails, currentDiscountDetailLine, DiscountID, DiscountSequenceID, branchID, locationID, date.Value);
        }

        #endregion

        #region Line Discounts
        //Calculates line-level discounts
        public static void SetLineDiscount(PXCache sender, HashSet<KeyValuePair<object, string>> entities, Line line, DateTime date, RecalcDiscountsParamFilter recalcFilter = null)
        {
            AmountLineFields documentLine = GetDiscountDocumentLine(sender, line);
            DiscountLineFields dline = GetDiscountedLine(sender, line);
            
            object unitPrice = documentLine.CuryUnitPrice;
            object extPrice = documentLine.CuryExtPrice;
            object qty = documentLine.Quantity;

            bool overrideManualDiscount = recalcFilter != null && recalcFilter.UseRecalcFilter == true && recalcFilter.OverrideManualDiscounts == true;

            if (!dline.ManualDisc || overrideManualDiscount)
            {
                DiscountEngine.GetDiscountTypes();
                
                if (extPrice != null && qty != null)
                {
                    DiscountEngine de = new DiscountEngine();
                    decimal discountTargetPrice = 0m;
                    if (GetLineDiscountTarget(sender, line) == LineDiscountTargetType.SalesPrice)
                        discountTargetPrice = (decimal)unitPrice;
                    else
                        discountTargetPrice = (decimal)extPrice;

                    if (overrideManualDiscount)
                    {
                        dline.ManualDisc = false;
                        dline.RaiseFieldUpdated<DiscountLineFields.manualDisc>(null);
                    }

                    DiscountDetailLine discount = de.SelectBestDiscount(sender, dline, entities, DiscountType.Line, discountTargetPrice, (decimal)qty, date);
                    if (discount.DiscountID != null)
                    {
                        decimal curyDiscountAmt = CalculateDiscount(sender, discount, dline, discountTargetPrice, (decimal)qty, date, DiscountType.Line);

                        DiscountResult dResult = new DiscountResult(discount.DiscountedFor == "A" ? curyDiscountAmt : discount.Discount, discount.DiscountedFor == "A" ? true : false);

                        ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);
                        dline.DiscountID = discount.DiscountID;
                        //dline.RaiseFieldUpdated<DiscountLineFields.discountID>(null);
                        dline.DiscountSequenceID = discount.DiscountSequenceID;
                        dline.RaiseFieldUpdated<DiscountLineFields.discountSequenceID>(null);
                    }
                    else
                    {
                        DiscountResult dResult = new DiscountResult(0m, true);

                        ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Sets manual line discount
        /// </summary>
        public static void SetManualLineDiscount(PXCache sender, HashSet<KeyValuePair<object, string>> entities, Line line, DateTime date)
        {
            AmountLineFields documentLine = GetDiscountDocumentLine(sender, line);
            DiscountLineFields dline = GetDiscountedLine(sender, line);

            object unitPrice = documentLine.CuryUnitPrice;
            object extPrice = documentLine.CuryExtPrice;
            object qty = documentLine.Quantity;

            string DiscountID = dline.DiscountID;

            if (extPrice != null && qty != null)
            {
                DiscountEngine de = new DiscountEngine();
                DiscountEngine.GetDiscountTypes();
                
                HashSet<DiscountSequenceKey> discountSequencesByDiscountID = de.SelectDiscountSequences(DiscountID);
                HashSet<DiscountSequenceKey> applicableDiscountSequences = de.SelectApplicableEntitytDiscounts(entities, DiscountType.Line, false);

				applicableDiscountSequences.IntersectWith(discountSequencesByDiscountID);

                decimal discountTargetPrice = 0m;
                if (GetLineDiscountTarget(sender, line) == LineDiscountTargetType.SalesPrice)
                    discountTargetPrice = (decimal)unitPrice;
                else
                    discountTargetPrice = (decimal)extPrice;

                DiscountDetailLine discount = de.SelectApplicableDiscount(sender, dline, applicableDiscountSequences, discountTargetPrice, (decimal)qty, DiscountType.Line, date);

                dline.ManualDisc = true;
                dline.RaiseFieldUpdated<DiscountLineFields.manualDisc>(null);

                if (discount.DiscountID != null)
                {
                    decimal curyDiscountAmt = CalculateDiscount(sender, discount, dline, discountTargetPrice, (decimal)qty, date, DiscountType.Line);

                    DiscountResult dResult = new DiscountResult(discount.DiscountedFor == "A" ? curyDiscountAmt : discount.Discount, discount.DiscountedFor == "A" ? true : false);

                    ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);

                    dline.DiscountSequenceID = discount.DiscountSequenceID;
                    dline.RaiseFieldUpdated<DiscountLineFields.discountSequenceID>(null);
                }
                else
                {
                    DiscountResult dResult = new DiscountResult(0m, true);

                    ApplyDiscountToLine(sender, line, (decimal)qty, (decimal)unitPrice, (decimal)extPrice, dline, dResult, 1);

                    if (DiscountID != null)
                    {
                        PXUIFieldAttribute.SetWarning<DiscountLineFields.discountID>(sender, line, PXMessages.LocalizeFormatNoPrefixNLA(Messages.NoDiscountFound, DiscountID));
                        if (sender.Graph.IsImport) dline.DiscountID = null;
                    }
                }
            }
        }

        private const string DiscountID = "DiscountID";
        private const string DiscountSequenceID = "DiscountSequenceID";
        private const string TypeFieldName = "Type";

        /// <summary>
        /// Applies line-level discount to a line
        /// </summary>
        /// <param name="Qty">Quantity</param>
        /// <param name="CuryUnitPrice">Cury Unit Price</param>
        /// <param name="CuryExtPrice">Cury Ext. Price</param>
        /// <param name="dline">Discount will be applied to this line</param>
        /// <param name="discountResult">DiscountResult (discount percent/amount and isAmount flag)</param>
        /// <param name="multInv"></param>
        public static void ApplyDiscountToLine(PXCache sender, Line line, decimal? Qty, decimal? CuryUnitPrice, decimal? CuryExtPrice, DiscountLineFields dline, DiscountResult discountResult, int multInv)
        {
            PXCache cache = dline.cache;
            object row = dline.row;

            decimal qty = Math.Abs(Qty.Value);

            if (!discountResult.IsEmpty)
            {
                LineEntitiesFields efields = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);
                string LineDiscountTarget = GetLineDiscountTarget(sender, line);
				CommonSetup commonsetup = PXSelect<CommonSetup>.Select(cache.Graph);
                int precision = 4;
				if (commonsetup != null && commonsetup.DecPlPrcCst != null)
					precision = commonsetup.DecPlPrcCst.Value;

                if (discountResult.IsAmount)
                {
                    if (LineDiscountTarget == LineDiscountTargetType.SalesPrice)
                    {
                        decimal discAmt = (discountResult.Discount ?? 0) * qty;
                        decimal curyDiscAmt;
                        PXCurrencyAttribute.CuryConvCury(cache, row, discAmt, out curyDiscAmt, precision);
                        decimal? oldCuryDiscAmt = dline.CuryDiscAmt;
                        dline.CuryDiscAmt = multInv * curyDiscAmt;
                        if (dline.CuryDiscAmt > Math.Abs(CuryExtPrice ?? 0m))
                        {
                            dline.CuryDiscAmt = CuryExtPrice;
                            PXUIFieldAttribute.SetWarning<DiscountLineFields.curyDiscAmt>(sender, line, AR.Messages.LineDiscountAmtMayNotBeGreaterExtPrice);
                        }
                        if (dline.CuryDiscAmt != oldCuryDiscAmt)
                        {
							UpdateDiscAmt<DiscountLineFields.curyDiscAmt>(dline, oldCuryDiscAmt, dline.CuryDiscAmt);
                        }
                    }
                    else
                    {
                        decimal discAmt = (discountResult.Discount ?? 0);
                        decimal curyDiscAmt;
                        PXCurrencyAttribute.CuryConvCury(cache, row, discAmt, out curyDiscAmt, precision);
                        decimal? oldCuryDiscAmt = dline.CuryDiscAmt;
                        dline.CuryDiscAmt = multInv * curyDiscAmt;
                        if (dline.CuryDiscAmt > Math.Abs(CuryExtPrice ?? 0m))
                        {
                            dline.CuryDiscAmt = CuryExtPrice;
                            PXUIFieldAttribute.SetWarning<DiscountLineFields.curyDiscAmt>(sender, line, AR.Messages.LineDiscountAmtMayNotBeGreaterExtPrice);
                        }
						if (dline.CuryDiscAmt != oldCuryDiscAmt)
						{
							UpdateDiscAmt<DiscountLineFields.curyDiscAmt>(dline, oldCuryDiscAmt, dline.CuryDiscAmt);
						}
                    }

                    if (dline.CuryDiscAmt != 0 && CuryExtPrice.Value != 0)
                    {
                        decimal? oldValue = dline.DiscPct;
                        decimal discPct = dline.CuryDiscAmt.Value * 100 / CuryExtPrice.Value;
                        dline.DiscPct = Math.Round(discPct, 6, MidpointRounding.AwayFromZero);
						if (dline.DiscPct != oldValue)
							UpdateDiscPct<DiscountLineFields.discPct>(dline, oldValue, dline.DiscPct);
                    }
                }
                else
                {
                    decimal? oldValue = dline.DiscPct;
                    dline.DiscPct = Math.Round(discountResult.Discount ?? 0, 6, MidpointRounding.AwayFromZero);
					if (dline.DiscPct != oldValue)
						UpdateDiscPct<DiscountLineFields.discPct>(dline, oldValue, dline.DiscPct);
                    decimal? oldCuryDiscAmt = dline.CuryDiscAmt;
                    decimal curyDiscAmt;

                    if (LineDiscountTarget == LineDiscountTargetType.SalesPrice)
                    {
                        decimal salesPriceAfterDiscount = (CuryUnitPrice ?? 0) * (1 - 0.01m * (dline.DiscPct ?? 0));
                        decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
                        curyDiscAmt = qty * (CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(cache, row, extPriceAfterDiscount, CMPrecision.TRANCURY);
						if (curyDiscAmt < 0 && Math.Abs(curyDiscAmt) < 0.01m )//this can happen only due to difference in rounding between unitprice and exprice. ex when Unit price has 4 digits (with value in 3rd place 20.0050) and ext price only 2 
                            curyDiscAmt = 0;
                    }
                    else
                    {
                        curyDiscAmt = (CuryExtPrice ?? 0) * 0.01m * (dline.DiscPct ?? 0);
                    }

                    dline.CuryDiscAmt = multInv * PXCurrencyAttribute.Round(cache, row, curyDiscAmt, CMPrecision.TRANCURY);
                    if (Math.Abs(dline.CuryDiscAmt ?? 0m) > Math.Abs(CuryExtPrice ?? 0m))
                    {
                        dline.CuryDiscAmt = CuryExtPrice;
                        PXUIFieldAttribute.SetWarning<DiscountLineFields.curyDiscAmt>(sender, line, AR.Messages.LineDiscountAmtMayNotBeGreaterExtPrice);
                    }
                    if (dline.CuryDiscAmt != oldCuryDiscAmt)
                    {
						UpdateDiscAmt<DiscountLineFields.curyDiscAmt>(dline, oldCuryDiscAmt, dline.CuryDiscAmt);
                    }
                }
            }
            else if (!sender.Graph.IsImport)
            {
                decimal? oldDiscPct = dline.DiscPct;
                decimal? oldCuryDiscAmt = dline.CuryDiscAmt;
                string oldDiscountID = dline.DiscountID;
                dline.DiscPct = 0;
                if (oldDiscPct != 0)
                    dline.RaiseFieldUpdated<DiscountLineFields.discPct>(oldDiscPct);
                dline.CuryDiscAmt = 0;
                if (oldCuryDiscAmt != 0)
                    dline.RaiseFieldUpdated<DiscountLineFields.curyDiscAmt>(oldCuryDiscAmt);
				if (dline.DiscPct == 0m && dline.CuryDiscAmt == 0m)
				{
					dline.DiscountID = null;
					dline.DiscountSequenceID = null;
                }
            }
        }

		private static void UpdateDiscPct<Field>(DiscountLineFields dline, decimal? oldValue, decimal? newValue)
			where Field : IBqlField
		{
			object discPct = newValue;
			dline.RaiseFieldVerifying<Field>(ref discPct);
			dline.DiscPct = (decimal)discPct;
			dline.RaiseFieldUpdated<Field>(oldValue);
		}

		private static void UpdateDiscAmt<Field>(DiscountLineFields dline, decimal? oldValue, decimal? newValue)
			where Field : IBqlField
		{
			object curyDiscAmt = newValue;
			dline.RaiseFieldVerifying<Field>(ref curyDiscAmt);
			dline.CuryDiscAmt = (decimal)curyDiscAmt;
			dline.RaiseFieldUpdated<Field>(oldValue);
		}

        #endregion

        #region Group Discounts
        //Updates group discounts for a given line only. To review.
        public static bool UpdateGroupDiscountsOnGivenLine<DiscountDetail>(PXCache sender, Line newLine, Line oldLine, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            bool skipDocumentDiscounts = false;
            Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails = new Dictionary<DiscountSequenceKey, DiscountDetail>();

            DiscountEngine de = new DiscountEngine();
            HashSet<DiscountSequenceKey> applicableGroupDiscounts = de.SelectApplicableEntitytDiscounts(GetDiscountEntitiesDiscounts(sender, newLine, locationID, true), "G");

            //check for oldLine == null
            AmountLineFields newDocumentLine = GetDiscountDocumentLine(sender, newLine);
            AmountLineFields oldDocumentLine = GetDiscountDocumentLine(sender, oldLine);

            if (newDocumentLine.CuryLineAmount != null && newDocumentLine.Quantity != null)
            {
                foreach (DiscountSequenceKey discountSequence in applicableGroupDiscounts)
                {
                    DiscountDetail trace = GetDiscountDetail(sender, discountDetails, discountSequence.DiscountID, discountSequence.DiscountSequenceID, "G");
                    if (trace != null)
                    {
                        decimal newDiscountableAmount = (decimal)(trace.CuryDiscountableAmt + (newDocumentLine.CuryLineAmount - oldDocumentLine.CuryLineAmount));
                        decimal newDiscountableQuantity = (decimal)(trace.DiscountableQty + (newDocumentLine.Quantity - oldDocumentLine.Quantity));

                        DiscountDetailLine discountDetail = de.SelectApplicableDiscount(sender, GetDiscountedLine(sender, newLine), discountSequence, trace.CuryDiscountableAmt + (newDocumentLine.CuryLineAmount - oldDocumentLine.CuryLineAmount), trace.DiscountableQty + (newDocumentLine.Quantity - oldDocumentLine.Quantity), "G", date);

                        if (discountDetail.DiscountedFor == "A")
                        {
                            trace.CuryDiscountAmt = discountDetail.Discount;
                        }
                        else
                        {
                            trace.CuryDiscountAmt = (decimal)newDiscountableAmount / 100 * discountDetail.Discount;
                        }
                        trace.DiscountPct = discountDetail.Discount;

                        trace.CuryDiscountableAmt = newDiscountableAmount;
                        trace.DiscountableQty = newDiscountableQuantity;

                        trace.FreeItemQty = discountDetail.freeItemQty;

                        discountDetails.Update(trace);
                    }
                    else
                    {
                        DiscountDetailLine discountDetail = de.SelectApplicableDiscount(sender, GetDiscountedLine(sender, newLine), discountSequence, newDocumentLine.CuryLineAmount, newDocumentLine.Quantity, "G", date);
                        if (discountDetail.DiscountID != null)
                        {
                            DiscountDetail newDiscountDetail = new DiscountDetail();
                            newDiscountDetail.DiscountID = discountDetail.DiscountID;
                            newDiscountDetail.DiscountSequenceID = discountDetail.DiscountSequenceID;
                            if (discountDetail.DiscountedFor == "A")
                            {
                                newDiscountDetail.CuryDiscountAmt = discountDetail.Discount;
                            }
                            else
                            {
                                newDiscountDetail.CuryDiscountAmt = (decimal)newDocumentLine.CuryLineAmount / 100 * discountDetail.Discount;
                            }
                            newDiscountDetail.DiscountPct = discountDetail.Discount;
                            newDiscountDetail.Type = DiscountType.Group;

                            newDiscountDetail.CuryDiscountableAmt = newDocumentLine.CuryLineAmount;
                            newDiscountDetail.DiscountableQty = newDocumentLine.Quantity;

                            newDiscountDetail.FreeItemID = discountDetail.freeItemID;
                            newDiscountDetail.FreeItemQty = discountDetail.freeItemQty;

                            discountDetails.Insert(newDiscountDetail);
                        }
                    }
                }
            }
            return skipDocumentDiscounts;
        }

        //Collects all applicable group discounts and adds them do Discount Details
        public static bool SetGroupDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime date, bool skipFreeItems = false)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            bool skipDocumentDiscounts = false;
            Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails = new Dictionary<DiscountSequenceKey, DiscountDetail>();
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            List<Line> documentDetails = GetDocumentDetails(sender, lines);
            Dictionary<DiscountSequenceKey, List<Line>> grLines = new Dictionary<DiscountSequenceKey, List<Line>>();

            if (documentDetails.Count != 0)
            {
                Dictionary<DiscountSequenceKey, DiscountableValues> grLine = new Dictionary<DiscountSequenceKey, DiscountableValues>();

                CollectGroupDiscountCodes(sender, documentDetails, discountDetails, grLine, grLines, locationID, date);

                DiscountEngine de = new DiscountEngine();
                List<DiscountDetailLine> discounts = new List<DiscountDetailLine>();
                foreach (KeyValuePair<DiscountSequenceKey, DiscountableValues> applicableGroup in grLine)
                {
                    if (!skipFreeItems || (skipFreeItems && SelectDiscountSequence(sender, applicableGroup.Key.DiscountID, applicableGroup.Key.DiscountSequenceID).DiscountedFor != DiscountOption.FreeItem))
                    {
						HashSet<DiscountSequenceKey> applicableDiscount = new HashSet<DiscountSequenceKey>();
						applicableDiscount.Add(applicableGroup.Key);

                        DiscountLineFields discountedLine = GetDiscountedLine(sender, documentDetails[0]);

                        List<DiscountDetailLine> applicableDiscounts = de.SelectApplicableDiscounts(sender, discountedLine, applicableDiscount, applicableGroup.Value.CuryDiscountableAmount, applicableGroup.Value.DiscountableQuantity, DiscountType.Group, date);
                        InsertGroupDiscountsToDiscountDetails<DiscountDetail>(sender, discountedLine, applicableDiscounts, date, newDiscountDetails, (decimal)applicableGroup.Value.CuryDiscountableAmount, (decimal)applicableGroup.Value.DiscountableQuantity);
                    }
                }

                RemoveUnapplicableDiscountDetails(sender, discountDetails, newDiscountDetails.Keys.ToList(), DiscountType.Group);

                foreach (KeyValuePair<DiscountSequenceKey, DiscountDetail> dDetail in newDiscountDetails)
                {
                    DiscountDetail updatedInsertedDiscountDetail = UpdateInsertDiscountTrace<DiscountDetail>(sender, discountDetails, dDetail.Value);
                    if (updatedInsertedDiscountDetail.SkipDiscount != true && cachedDiscountTypes[updatedInsertedDiscountDetail.DiscountID].SkipDocumentDiscounts)
                    {
                        skipDocumentDiscounts = true;
                    }
                }
                if (skipDocumentDiscounts)
                {
                    RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Document);
                    CalculateDocumentDiscountRate(sender, documentDetails, null, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Document), false);
                }

                CalculateGroupDiscountRate(sender, documentDetails, currentLine, grLines, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group), false);
            }
            else
            {
                RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Group);
            }
            return skipDocumentDiscounts;
        }

        //Collects all applicable group discounts and updates GroupDiscountRates
        public static Dictionary<DiscountSequenceKey, List<Line>> AdjustGroupDiscountRates<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails = new Dictionary<DiscountSequenceKey, DiscountDetail>();
            List<Line> documentDetails = GetDocumentDetails(sender, lines);
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();
            Dictionary<DiscountSequenceKey, List<Line>> grLines = new Dictionary<DiscountSequenceKey, List<Line>>();

            if (documentDetails.Count != 0)
            {
                Dictionary<DiscountSequenceKey, DiscountableValues> grLine = new Dictionary<DiscountSequenceKey, DiscountableValues>();

                CollectGroupDiscountCodes(sender, documentDetails, discountDetails, grLine, grLines, locationID, date);

                CalculateGroupDiscountRate(sender, documentDetails, null, grLines, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group), true);
            }
            return grLines;
        }

        //Collects all applicable group discounts and creates correlation between group discounts and document lines
        public static Dictionary<DiscountSequenceKey, DiscountDetailToLineCorrelation<DiscountDetail, Line>> CollectGroupDiscountToLineCorrelation<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, PXSelectBase<DiscountDetail> discountDetails, int? locationID, DateTime date, bool freeItemsOnly)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails = new Dictionary<DiscountSequenceKey, DiscountDetail>();

            Dictionary<DiscountSequenceKey, DiscountDetailToLineCorrelation<DiscountDetail, Line>> lineCorrelation = new Dictionary<DiscountSequenceKey, DiscountDetailToLineCorrelation<DiscountDetail, Line>>();

            List<Line> documentDetails = GetDocumentDetails(sender, lines);

			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            if (documentDetails.Count != 0)
            {
                Dictionary<DiscountSequenceKey, DiscountableValues> grLine = new Dictionary<DiscountSequenceKey, DiscountableValues>();

                Dictionary<DiscountSequenceKey, List<Line>> grLines = new Dictionary<DiscountSequenceKey, List<Line>>();
                DiscountEngine de = new DiscountEngine();

                CollectGroupDiscountCodes(sender, documentDetails, discountDetails, grLine, grLines, locationID, date);

                List<DiscountDetailLine> discounts = new List<DiscountDetailLine>();
                foreach (KeyValuePair<DiscountSequenceKey, DiscountableValues> applicableGroup in grLine)
                {
                    if ((freeItemsOnly && SelectDiscountSequence(sender, applicableGroup.Key.DiscountID, applicableGroup.Key.DiscountSequenceID).DiscountedFor == DiscountOption.FreeItem) || !freeItemsOnly)
                    {
                        HashSet<DiscountSequenceKey> applicableDiscount = new HashSet<DiscountSequenceKey>();
						applicableDiscount.Add(applicableGroup.Key);

                        DiscountLineFields discountedLine = GetDiscountedLine(sender, documentDetails[0]);

                        List<DiscountDetailLine> applicableDiscounts = de.SelectApplicableDiscounts(sender, discountedLine, applicableDiscount, applicableGroup.Value.CuryDiscountableAmount, applicableGroup.Value.DiscountableQuantity, DiscountType.Group, date, true);
                        InsertGroupDiscountsToDiscountDetails<DiscountDetail>(sender, discountedLine, applicableDiscounts, date, newDiscountDetails, (decimal)applicableGroup.Value.CuryDiscountableAmount, (decimal)applicableGroup.Value.DiscountableQuantity);
                    }
                }

                foreach (KeyValuePair<DiscountSequenceKey, DiscountDetail> dLine in newDiscountDetails)
                {
                    if (grLines.ContainsKey(dLine.Key))
                    {
                        lineCorrelation.Add(dLine.Key, new DiscountDetailToLineCorrelation<DiscountDetail, Line> { discountDetailLine = dLine.Value, listOfApplicableLines = grLines[dLine.Key] });
                    }
                }
            }

            return lineCorrelation;
        }

        public static void CollectGroupDiscountCodes<DiscountDetail>(PXCache sender, List<Line> documentDetails, PXSelectBase<DiscountDetail> discountDetails,
            Dictionary<DiscountSequenceKey, DiscountableValues> grLine, Dictionary<DiscountSequenceKey, List<Line>> grLines, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            DiscountEngine de = new DiscountEngine();
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            foreach (Line line in documentDetails)
            {
                DiscountLineFields discountedLine = GetDiscountedLine(sender, line);
                bool excludeFromDiscountableAmount = false;
                if (discountedLine.DiscountID != null)
                    excludeFromDiscountableAmount = cachedDiscountTypes[discountedLine.DiscountID].ExcludeFromDiscountableAmt;

                if (!excludeFromDiscountableAmount)
                {
                    HashSet<DiscountSequenceKey> applicableGroupDiscounts = de.SelectApplicableEntitytDiscounts(GetDiscountEntitiesDiscounts(sender, line, locationID, true), DiscountType.Group, false);

                    applicableGroupDiscounts = RemoveUnapplicableManualDiscounts(sender, discountDetails, applicableGroupDiscounts, DiscountType.Group);

                    DiscountableValues discountableValues = new DiscountableValues();
                    discountableValues.CuryDiscountableAmount = GetDiscountDocumentLine(sender, line).CuryLineAmount ?? 0m;
                    discountableValues.DiscountableQuantity = GetDiscountDocumentLine(sender, line).Quantity ?? 0m;

                    foreach (DiscountSequenceKey dSequence in applicableGroupDiscounts)
                    {
                        if (grLine.ContainsKey(dSequence))
                        {
                            DiscountableValues newDiscountableValues = new DiscountableValues();
                            newDiscountableValues.CuryDiscountableAmount = grLine[dSequence].CuryDiscountableAmount + discountableValues.CuryDiscountableAmount;
                            newDiscountableValues.DiscountableQuantity = grLine[dSequence].DiscountableQuantity + discountableValues.DiscountableQuantity;
                            grLine[dSequence] = newDiscountableValues;
                        }
                        else
                        {
                            grLine.Add(dSequence, discountableValues);
                        }

                        if (grLines.ContainsKey(dSequence))
                        {
                            grLines[dSequence].Add(line);
                        }
                        else
                        {
                            grLines.Add(dSequence, new List<Line> { line });
                        }
                    }
                }
            }
        }

        public static void CalculateGroupDiscountRate<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, Dictionary<DiscountSequenceKey, List<Line>> grLines, PXSelectBase<DiscountDetail> discountDetails, bool recalculateTaxes, string docType, string docNbr, bool recalcAll = true)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
			List<Line> documentDetails = GetDocumentDetails(sender, lines, (docType != null && docNbr != null) ? new object[] { docType, docNbr } : null);
			CalculateGroupDiscountRate(sender, documentDetails, currentLine, grLines, discountDetails, recalculateTaxes, new object[] { docType, docNbr, DiscountType.Group }, recalcAll);
        }

		public static void CalculateGroupDiscountRate<DiscountDetail>(PXCache sender, List<Line> documentDetails, Line currentLine, Dictionary<DiscountSequenceKey, List<Line>> grLines, PXSelectBase<DiscountDetail> discountDetails, bool recalculateTaxes, object[] parameters, bool recalcAll = true)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountDetail> discountDetailsList = new List<DiscountDetail>();
            foreach (DiscountDetail dDetail in GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group, parameters))
            {
                if (dDetail.SkipDiscount != true)
                    discountDetailsList.Add(dDetail);
            }
            CalculateGroupDiscountRate<DiscountDetail>(sender, documentDetails, currentLine, grLines, discountDetailsList, recalculateTaxes, recalcAll);
        }

        public static void CalculateGroupDiscountRate<DiscountDetail>(PXCache sender, List<Line> documentDetails, Line currentLine, Dictionary<DiscountSequenceKey, List<Line>> grLines, List<DiscountDetail> newDiscountDetails, bool recalculateTaxes, bool recalcAll = true)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            foreach (Line line in documentDetails)
            {
                AmountLineFields lineAmt = GetDiscountDocumentLine(sender, line);
                Line oldLine = (Line)sender.CreateCopy(line);
                Line oldCurrentLine = (Line)sender.CreateCopy(currentLine);
                decimal lineGroupAmt = 0m;
                if (recalcAll)
                    lineGroupAmt = (decimal)lineAmt.CuryLineAmount;
                else
                    lineGroupAmt = (decimal)lineAmt.CuryLineAmount * (decimal)lineAmt.GroupDiscountRate;
                if (lineAmt.CuryLineAmount != null && lineAmt.CuryLineAmount != 0m)
                {
                    foreach (KeyValuePair<DiscountSequenceKey, List<Line>> detailsLine in grLines)
                    {
                        foreach (Line grDetLine in detailsLine.Value)
                        {
                            if (grDetLine.Equals(line))
                            {
                                foreach (DiscountDetail dDetail in newDiscountDetails)
                                {
                                    if (dDetail.SkipDiscount != true)
                                    {
                                        DiscountSequenceKey dsKey = new DiscountSequenceKey();
                                        dsKey.DiscountID = dDetail.DiscountID;
                                        dsKey.DiscountSequenceID = dDetail.DiscountSequenceID;
                                        if (detailsLine.Key.Equals(dsKey))
                                        {
                                            if (lineGroupAmt > 0 && dDetail.CuryDiscountableAmt != null && (decimal)dDetail.CuryDiscountableAmt > 0)
                                            {
                                                lineGroupAmt -= (decimal)lineAmt.CuryLineAmount * (decimal)dDetail.CuryDiscountAmt / (decimal)dDetail.CuryDiscountableAmt;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    decimal? GroupDiscountRate = lineGroupAmt / GetDiscountDocumentLine(sender, oldLine).CuryLineAmount;
                    lineAmt.GroupDiscountRate = Math.Round(GroupDiscountRate ?? 1m, 6);
                    if (GetDiscountDocumentLine(sender, oldLine).GroupDiscountRate != lineAmt.GroupDiscountRate)
                    {
                        if (!CompareRows(oldLine, oldCurrentLine)) TX.TaxAttribute.Calculate<AmountLineFields.taxCategoryID>(sender, new PXRowUpdatedEventArgs(line, oldLine, false));
                        if (sender.GetStatus(line) == PXEntryStatus.Notchanged) sender.SetStatus(line, PXEntryStatus.Updated);
                    }
                }
            }
        }

        public static void CalculateDocumentDiscountRate<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, decimal lineTotal, decimal discountTotal, string docType = null, string docNbr = null, bool alwaysRecalc = false, bool overrideDiscountTotal = false)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
			CalculateDocumentDiscountRate<DiscountDetail>(sender, lines, null, currentLine, discountDetails, lineTotal, discountTotal, docType, docNbr, alwaysRecalc, overrideDiscountTotal);
        }

		public static void CalculateDocumentDiscountRate<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, List<Line> documentDetailsList, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, decimal lineTotal, decimal discountTotal, string docType = null, string docNbr = null, bool alwaysRecalc = false, bool overrideDiscountTotal = false, bool skipParameters = false)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<Line> documentDetails = documentDetailsList ?? GetDocumentDetails(sender, lines, new object[] { docType, docNbr });

            decimal totalGroupDiscountAmt = 0m;
            decimal totalDocumentDiscountAmt = 0m;

            foreach (DiscountDetail dDetail in GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group, skipParameters ? null : ((docType != null && docNbr != null) ? new object[] { docType, docNbr, DiscountType.Group } : (new object[] { DiscountType.Group }))))
            {
                if (dDetail.SkipDiscount != true)
                    totalGroupDiscountAmt += (decimal)dDetail.CuryDiscountAmt;
            }
            foreach (DiscountDetail dDetail in GetDiscountDetailsByType(sender, discountDetails, DiscountType.Document, skipParameters ? null : ((docType != null && docNbr != null) ? new object[] { docType, docNbr, DiscountType.Document } : (new object[] { DiscountType.Document }))))
            {
                if (dDetail.SkipDiscount != true)
                    totalDocumentDiscountAmt += (decimal)dDetail.CuryDiscountAmt;
            }

			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            if ((Math.Abs(PXCurrencyAttribute.RoundCury(sender, lines, totalGroupDiscountAmt) + PXCurrencyAttribute.RoundCury(sender, lines, totalDocumentDiscountAmt)
                - PXCurrencyAttribute.RoundCury(sender, lines, discountTotal)) > 0.000005m) ||
                (totalGroupDiscountAmt == 0m && totalDocumentDiscountAmt == 0m && discountTotal == 0m) || alwaysRecalc)
            {
                if (overrideDiscountTotal)
                {
                    totalGroupDiscountAmt = 0m;
                    totalDocumentDiscountAmt = discountTotal;
                }

                decimal updLineTotal = 0m;
                foreach (Line line in documentDetails)
                {
                    AmountLineFields lineAmtFields = GetDiscountDocumentLine(sender, line);
                    DiscountLineFields discountFields = GetDiscountedLine(sender, line);
                    if (discountFields.DiscountID == null || (discountFields.DiscountID != null && cachedDiscountTypes.ContainsKey(discountFields.DiscountID) && !cachedDiscountTypes[discountFields.DiscountID].ExcludeFromDiscountableAmt))
                    {
                        updLineTotal += (decimal)lineAmtFields.CuryLineAmount;
                    }
                }
                updLineTotal = updLineTotal - totalGroupDiscountAmt;

                foreach (Line line in documentDetails)
                {
                    AmountLineFields lineAmtFields = GetDiscountDocumentLine(sender, line);
                    DiscountLineFields discountFields = GetDiscountedLine(sender, line);

                    Line oldLine = (Line)sender.CreateCopy(line);
                    Line oldCurrentLine = (Line)sender.CreateCopy(currentLine);

                    decimal? DocumentDiscountRate;
                    if (updLineTotal != 0m && discountTotal != 0m && (decimal)lineAmtFields.CuryLineAmount != 0m
                        && (discountFields.DiscountID == null || (discountFields.DiscountID != null && cachedDiscountTypes.ContainsKey(discountFields.DiscountID) && !cachedDiscountTypes[discountFields.DiscountID].ExcludeFromDiscountableAmt)))
                        DocumentDiscountRate = 1 - ((decimal)lineAmtFields.CuryLineAmount * totalDocumentDiscountAmt / updLineTotal) / (decimal)lineAmtFields.CuryLineAmount;
                    else
                        DocumentDiscountRate = 1;

                    lineAmtFields.DocumentDiscountRate = Math.Round(DocumentDiscountRate ?? 1m, 6);

                    if (totalGroupDiscountAmt == 0m)
                        lineAmtFields.GroupDiscountRate = 1;

                    if (GetDiscountDocumentLine(sender, oldLine).DocumentDiscountRate != lineAmtFields.DocumentDiscountRate)
                    {
                        if (!CompareRows(oldLine, oldCurrentLine)) TX.TaxAttribute.Calculate<AmountLineFields.taxCategoryID>(sender, new PXRowUpdatedEventArgs(line, oldLine, false));
                        if (sender.GetStatus(line) == PXEntryStatus.Notchanged) sender.SetStatus(line, PXEntryStatus.Updated);
                    }
                }
            }
        }

        private static void CalculateDocumentDiscountRate<DiscountDetail>(PXCache sender, List<Line> documentDetails, Line currentLine, List<DiscountDetail> newDiscountDetails, bool recalcAll = true)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            decimal totalDocumentDiscountAmt = 0m;
            decimal totalDocumentDiscountableAmt = 0m;

			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            foreach (DiscountDetail dDetail in newDiscountDetails)
            {
                if (dDetail.SkipDiscount != true && dDetail.Type == DiscountType.Document)
                {
                    totalDocumentDiscountAmt += (decimal)dDetail.CuryDiscountAmt;
                    totalDocumentDiscountableAmt += (decimal)dDetail.CuryDiscountableAmt;
                }
            }
            foreach (Line line in documentDetails)
            {
                AmountLineFields lineAmtFields = GetDiscountDocumentLine(sender, line);
                DiscountLineFields discountFields = GetDiscountedLine(sender, line);
                Line oldLine = (Line)sender.CreateCopy(line);
                Line oldCurrentLine = (Line)sender.CreateCopy(currentLine);
                decimal lineAmt = (decimal)lineAmtFields.CuryLineAmount * (decimal)lineAmtFields.GroupDiscountRate;

                decimal? DocumentDiscountRate;
                if (lineAmtFields.CuryLineAmount != null && lineAmtFields.CuryLineAmount != 0m && totalDocumentDiscountAmt != 0m && totalDocumentDiscountableAmt != 0m && GetDiscountDocumentLine(sender, oldLine).CuryLineAmount != 0m
                    && (discountFields.DiscountID == null || (discountFields.DiscountID != null && cachedDiscountTypes.ContainsKey(discountFields.DiscountID) && !cachedDiscountTypes[discountFields.DiscountID].ExcludeFromDiscountableAmt)))
                    DocumentDiscountRate = 1 - ((decimal)lineAmtFields.CuryLineAmount * totalDocumentDiscountAmt / totalDocumentDiscountableAmt) / GetDiscountDocumentLine(sender, oldLine).CuryLineAmount;
                else
                    DocumentDiscountRate = 1;

                lineAmtFields.DocumentDiscountRate = Math.Round(DocumentDiscountRate ?? 1m, 6);

                if (GetDiscountDocumentLine(sender, oldLine).DocumentDiscountRate != lineAmtFields.DocumentDiscountRate)
                {
                    if (!CompareRows(oldLine, oldCurrentLine)) TX.TaxAttribute.Calculate<AmountLineFields.taxCategoryID>(sender, new PXRowUpdatedEventArgs(line, oldLine, false));
                    if (sender.GetStatus(line) == PXEntryStatus.Notchanged) sender.SetStatus(line, PXEntryStatus.Updated);
                }
            }
        }

        //Returns DiscountDetail line
        public static DiscountDetail CreateDiscountDetailsLine<DiscountDetail>(PXCache sender, DiscountDetailLine discount, DiscountLineFields discountedLine, decimal curyDiscountedLineAmount, decimal discountedLineQty, DateTime date, string type)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            DiscountDetail newDiscountDetail = new DiscountDetail();
            newDiscountDetail.DiscountID = discount.DiscountID;
            newDiscountDetail.DiscountSequenceID = discount.DiscountSequenceID;

			decimal rawValue = CalculateDiscount(sender, discount, discountedLine, curyDiscountedLineAmount, discountedLineQty, date, type);
			newDiscountDetail.CuryDiscountAmt = PXDBCurrencyAttribute.Round(sender, newDiscountDetail, rawValue, CMPrecision.TRANCURY);


			if (discount.DiscountedFor == DiscountOption.Percent)
            {
                newDiscountDetail.DiscountPct = discount.Discount;
            }
            newDiscountDetail.Type = type;

            newDiscountDetail.CuryDiscountableAmt = curyDiscountedLineAmount;
            newDiscountDetail.DiscountableQty = (decimal)discountedLineQty;

            newDiscountDetail.FreeItemID = discount.freeItemID;
            newDiscountDetail.FreeItemQty = CalculateFreeItemQuantity(sender, discount, discountedLine, curyDiscountedLineAmount, discountedLineQty, date, type);

            return newDiscountDetail;
        }

        //Inserts group discounts to Discount Details
        public static bool InsertGroupDiscountsToDiscountDetails<DiscountDetail>(PXCache sender, DiscountLineFields discountedLine, List<DiscountDetailLine> discounts, DateTime date, Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails, decimal discountedLineAmount, decimal discountedLineQty)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            bool skipDocumentDiscounts = false;
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();
            DiscountEngine de = new DiscountEngine();

            foreach (DiscountDetailLine discount in discounts)
            {
                if (discount.DiscountID != null)
                {
                    //skip document discounts
                    if (cachedDiscountTypes[discount.DiscountID].SkipDocumentDiscounts)
                    {
                        skipDocumentDiscounts = true;
                    }

                    DiscountDetail newDiscountDetail = CreateDiscountDetailsLine<DiscountDetail>(sender, discount, discountedLine, discountedLineAmount, discountedLineQty, date, DiscountType.Group);

                    DiscountSequenceKey discountSequence = new DiscountSequenceKey();
                    discountSequence.DiscountID = newDiscountDetail.DiscountID;
                    discountSequence.DiscountSequenceID = newDiscountDetail.DiscountSequenceID;

                    //review
                    if (!newDiscountDetails.ContainsKey(discountSequence))
                    {
                        newDiscountDetails.Add(discountSequence, newDiscountDetail);
                    }
                    else
                    {
                        newDiscountDetails[discountSequence].CuryDiscountableAmt = null;
                        //newDiscountDetails[discountSequence].CuryDiscountableAmt += newDiscountDetail.CuryDiscountableAmt;
                        newDiscountDetails[discountSequence].DiscountableQty = null;
                        newDiscountDetails[discountSequence].DiscountPct = null;
                        newDiscountDetails[discountSequence].CuryDiscountAmt += newDiscountDetail.CuryDiscountAmt;
                    }
                }
            }
            return skipDocumentDiscounts;
        }

        #endregion

        #region Document Discounts
        //Collects Document Details lines, discountedLineAmount and calculates document discount
        public static void SetDocumentDiscount<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, PXSelectBase<DiscountDetail> discountDetails, Line currentLine, int? branchID, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<Line> documentDetails = GetDocumentDetails(sender, lines);
            decimal totalLineAmount;
            decimal curyTotalLineAmount;
            SumAmounts(sender, documentDetails, out totalLineAmount, out curyTotalLineAmount);
            if (documentDetails.Count != 0)
            {
                SetDocumentDiscount(sender, documentDetails.First(), GetDiscountEntitiesDiscounts(sender, documentDetails.First(), locationID, false, branchID), totalLineAmount, curyTotalLineAmount, discountDetails, date);
            }
            else
            {
                RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Document);
            }
			CalculateDocumentDiscountRate(sender, documentDetails, currentLine, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Document), false);
        }

        //Calculates document discount
        public static void SetDocumentDiscount<DiscountDetail>(PXCache sender, Line line, HashSet<KeyValuePair<object, string>> entities, decimal totalLineAmount, decimal curyTotalLineAmount, PXSelectBase<DiscountDetail> discountDetails, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            DiscountEngine de = new DiscountEngine();
            DiscountEngine.GetDiscountTypes();
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            List<DiscountDetail> discountDetailsByType = GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group);
            foreach (DiscountDetail detail in discountDetailsByType)
            {
                if (detail.SkipDiscount != true && cachedDiscountTypes[detail.DiscountID].SkipDocumentDiscounts)
                {
                    RemoveUnapplicableDiscountDetails(sender, discountDetails, new List<DiscountSequenceKey>(), DiscountType.Document);
                    return;
                }
            }
            decimal totalGroupDiscountAmount;
            decimal curyTotalGroupDiscountAmount;
            GetDiscountAmountByType(discountDetails.Cache, discountDetailsByType, DiscountType.Group, out totalGroupDiscountAmount, out curyTotalGroupDiscountAmount);

            List<DiscountSequenceKey> manualDiscounts = CollectManualDiscounts(sender, discountDetails, DiscountType.Document);

            LineEntitiesFields lineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);
            decimal discountLimit = DiscountEngine.GetDiscountLimit(sender, lineEntities.CustomerID, lineEntities.VendorID);
            List<DiscountDetailLine> documentDiscounts = new List<DiscountDetailLine>();
            if ((curyTotalLineAmount / 100 * discountLimit) > curyTotalGroupDiscountAmount || discountLimit == 0m)
            {
                if (curyTotalLineAmount != 0)
                {
                    documentDiscounts.Add(de.SelectBestDiscount(sender, GetDiscountedLine(sender, line), entities, DiscountType.Document, curyTotalLineAmount - curyTotalGroupDiscountAmount, 0, date));
                    foreach (DiscountSequenceKey manualDiscount in manualDiscounts)
                    {
                        documentDiscounts.Add(de.SelectApplicableDiscount(sender, GetDiscountedLine(sender, line), manualDiscount, curyTotalLineAmount - curyTotalGroupDiscountAmount, 0, DiscountType.Document, date));
                    }

                    List<DiscountSequenceKey> dKeys = new List<DiscountSequenceKey>();
                    foreach (DiscountDetailLine documentDiscount in documentDiscounts)
                    {
                        if (documentDiscount.DiscountID != null && documentDiscount.DiscountSequenceID != null)
                        {
                            DiscountSequenceKey dKey = new DiscountSequenceKey();
                            dKey.DiscountID = documentDiscount.DiscountID;
                            dKey.DiscountSequenceID = documentDiscount.DiscountSequenceID;
                            dKeys.Add(dKey);
                        }
                    }
                    RemoveUnapplicableDiscountDetails(sender, discountDetails, dKeys, DiscountType.Document);
                    decimal totalDocumentDiscountAmount = 0m;
                    foreach (DiscountDetailLine documentDiscount in documentDiscounts)
                    {
                        if (documentDiscount.DiscountID != null)
                        {
                            DiscountDetail newDiscountDetail = CreateDiscountDetailsLine<DiscountDetail>(sender, documentDiscount, GetDiscountedLine(sender, line), curyTotalLineAmount - curyTotalGroupDiscountAmount, 0, date, DiscountType.Document);
                            newDiscountDetail.CuryDiscountableAmt = curyTotalLineAmount - curyTotalGroupDiscountAmount;
                            totalDocumentDiscountAmount += newDiscountDetail.CuryDiscountAmt ?? 0m;
                            DiscountDetail dDetail = UpdateInsertDiscountTrace<DiscountDetail>(sender, discountDetails, newDiscountDetail);
                            if ((curyTotalLineAmount / 100 * discountLimit) < (totalDocumentDiscountAmount + curyTotalGroupDiscountAmount))
                            {
                                SetDiscountLimitException(sender, discountDetails, DiscountType.Document, PXMessages.LocalizeFormatNoPrefix(AR.Messages.DocDiscountExceedLimit, discountLimit));
                            }
                        }
                    }
                }
            }
            else if (curyTotalLineAmount != 0 && curyTotalGroupDiscountAmount > 0)
            {
                SetDiscountLimitException(sender, discountDetails, DiscountType.Group, AR.Messages.GroupDiscountExceedLimit);
            }
            else
            {
                RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Document);
            }
        }

        private static void SetDiscountLimitException<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, string discountType, string message, DiscountDetail currentDiscountLine = null)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountDetail> dDetails = GetDiscountDetailsByType(sender, discountDetails, discountType);
            if (dDetails.Count != 0 )
            {
                discountDetails.Cache.RaiseExceptionHandling(DiscountID, dDetails[0], null, new PXSetPropertyException(message, PXErrorLevel.Warning));
            }
            else if (currentDiscountLine != null)
            {
                discountDetails.Cache.RaiseExceptionHandling(DiscountID, currentDiscountLine, null, new PXSetPropertyException(message, PXErrorLevel.Warning));
            }
        }
        #endregion

        #region Manual Group/Document Discounts
        /// <summary>
        /// Sets manual group or document discount
        /// </summary>
        public static void SetManualGroupDocDiscount<DiscountDetail>(PXCache sender, PXSelectBase<Line> lines, Line currentLine, PXSelectBase<DiscountDetail> discountDetails, DiscountDetail currentDiscountDetailLine, string manualDiscountID, string manualDiscountSequenceID, int? branchID, int? locationID, DateTime date)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<Line> documentDetails = GetDocumentDetails(sender, lines);
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();

            DiscountEngine de = new DiscountEngine();

            if (cachedDiscountTypes.ContainsKey(manualDiscountID) && documentDetails.Count != 0)
            {
                bool isManualDiscountApplicable = false;
                bool isManualDiscountApplied = false;
                decimal totalGroupDiscountAmount;
                decimal curyTotalGroupDiscountAmount;
                decimal totalLineAmount;
                decimal curyTotalLineAmount;
                List<DiscountDetail> discountDetailsByType = GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group);
                GetDiscountAmountByType(discountDetails.Cache, discountDetailsByType, DiscountType.Group, out totalGroupDiscountAmount, out curyTotalGroupDiscountAmount);
                SumAmounts(sender, documentDetails, out totalLineAmount, out curyTotalLineAmount);               
                LineEntitiesFields lineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, documentDetails.First());
                decimal discountLimit = DiscountEngine.GetDiscountLimit(sender, lineEntities.CustomerID, lineEntities.VendorID);

                if (cachedDiscountTypes[manualDiscountID].Type == DiscountType.Group)
                {
                    Dictionary<DiscountSequenceKey, DiscountableValues> grLine = new Dictionary<DiscountSequenceKey, DiscountableValues>();
                    Dictionary<DiscountSequenceKey, List<Line>> grLines = new Dictionary<DiscountSequenceKey, List<Line>>();
                    foreach (Line line in documentDetails)
                    {
                        HashSet<DiscountSequenceKey> applicableGroupDiscounts = de.SelectApplicableEntitytDiscounts(GetDiscountEntitiesDiscounts(sender, line, locationID, true), cachedDiscountTypes[manualDiscountID].Type, false);

                        HashSet<DiscountSequenceKey> selectedGroupDiscounts = new HashSet<DiscountSequenceKey>();

                        foreach (DiscountSequenceKey sequenceKey in applicableGroupDiscounts)
                        {
                            if (sequenceKey.DiscountID == manualDiscountID && (manualDiscountSequenceID == null || (manualDiscountSequenceID != null && sequenceKey.DiscountSequenceID == manualDiscountSequenceID)))
                            {
								selectedGroupDiscounts.Add(sequenceKey);
                                isManualDiscountApplicable = true;
                            }
                        }

                        if (isManualDiscountApplicable)
                        {
                            DiscountLineFields discountedLine = GetDiscountedLine(sender, line);
                            bool excludeFromDiscountableAmount = false;
                            if (discountedLine.DiscountID != null)
                                excludeFromDiscountableAmount = cachedDiscountTypes[discountedLine.DiscountID].ExcludeFromDiscountableAmt;

                            if (!excludeFromDiscountableAmount)
                            {
                                DiscountableValues discountableValues = new DiscountableValues();
                                discountableValues.CuryDiscountableAmount = GetDiscountDocumentLine(sender, line).CuryLineAmount ?? 0m;
                                discountableValues.DiscountableQuantity = GetDiscountDocumentLine(sender, line).Quantity ?? 0m;

                                foreach (DiscountSequenceKey dSequence in selectedGroupDiscounts)
                                {
                                    if (grLine.ContainsKey(dSequence))
                                    {
                                        DiscountableValues newDiscountableValues = new DiscountableValues();
                                        newDiscountableValues.CuryDiscountableAmount = grLine[dSequence].CuryDiscountableAmount + discountableValues.CuryDiscountableAmount;
                                        newDiscountableValues.DiscountableQuantity = grLine[dSequence].DiscountableQuantity + discountableValues.DiscountableQuantity;
                                        grLine[dSequence] = newDiscountableValues;
                                    }
                                    else
                                    {
                                        grLine.Add(dSequence, discountableValues);
                                    }

                                    if (grLines.ContainsKey(dSequence))
                                    {
                                        grLines[dSequence].Add(line);
                                    }
                                    else
                                    {
                                        grLines.Add(dSequence, new List<Line> { line });
                                    }
                                }
                            }
                        }
                    }

                    //disabled for now. Uncomment for 39383
                    //if (grLine.Count > 1)
                    //    return;
                    Dictionary<DiscountSequenceKey, DiscountDetail> newDiscountDetails = new Dictionary<DiscountSequenceKey, DiscountDetail>();
                    foreach (KeyValuePair<DiscountSequenceKey, DiscountableValues> applicableGroup in grLine)
                    {
                        HashSet<DiscountSequenceKey> applicableDiscount = new HashSet<DiscountSequenceKey>();
						applicableDiscount.Add(applicableGroup.Key);

                        DiscountLineFields discountedLine = GetDiscountedLine(sender, documentDetails[0]);
                        List<DiscountDetailLine> discountDetailLines = de.SelectApplicableDiscounts(sender, discountedLine, applicableDiscount, applicableGroup.Value.CuryDiscountableAmount, applicableGroup.Value.DiscountableQuantity, DiscountType.Group, date);

                        if (discountDetailLines.Count != 0)
                        {
                            DiscountDetail newDiscountDetail = CreateDiscountDetailsLine<DiscountDetail>(sender, discountDetailLines.First(), discountedLine, (decimal)applicableGroup.Value.CuryDiscountableAmount, (decimal)applicableGroup.Value.DiscountableQuantity, date, DiscountType.Group);
                            newDiscountDetail.CuryDiscountableAmt = applicableGroup.Value.CuryDiscountableAmount;
                            newDiscountDetail.DiscountableQty = applicableGroup.Value.DiscountableQuantity;
                            currentDiscountDetailLine.DiscountSequenceID = newDiscountDetail.DiscountSequenceID;
                            currentDiscountDetailLine.Type = newDiscountDetail.Type;
                            currentDiscountDetailLine.CuryDiscountableAmt = newDiscountDetail.CuryDiscountableAmt;
                            currentDiscountDetailLine.DiscountableQty = newDiscountDetail.DiscountableQty;
                            currentDiscountDetailLine.DiscountPct = newDiscountDetail.DiscountPct;
                            currentDiscountDetailLine.CuryDiscountAmt = newDiscountDetail.CuryDiscountAmt;
                            currentDiscountDetailLine.FreeItemID = newDiscountDetail.FreeItemID;
                            currentDiscountDetailLine.FreeItemQty = newDiscountDetail.FreeItemQty;
                            isManualDiscountApplied = true;

                            if ((curyTotalLineAmount / 100 * discountLimit) < (newDiscountDetail.CuryDiscountAmt + curyTotalGroupDiscountAmount))
                            {
                                SetDiscountLimitException(sender, discountDetails, DiscountType.Group, PXMessages.LocalizeFormatNoPrefix(AR.Messages.OnlyGroupDiscountExceedLimit, discountLimit), currentDiscountDetailLine);
                            }
                        }
                        if (cachedDiscountTypes[manualDiscountID].SkipDocumentDiscounts)
                        {
                            RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Document);
                        }
                    }

                    CalculateGroupDiscountRate(sender, documentDetails, currentLine, grLines, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Group), true, false);
                }
                if (cachedDiscountTypes[manualDiscountID].Type == DiscountType.Document)
                {
                    HashSet<DiscountSequenceKey> applicableDiscounts = de.SelectApplicableEntitytDiscounts(GetDiscountEntitiesDiscounts(sender, documentDetails.First(), locationID, false, branchID), cachedDiscountTypes[manualDiscountID].Type, false);
                    HashSet<DiscountSequenceKey> selectedGroupDiscounts = new HashSet<DiscountSequenceKey>();
                    isManualDiscountApplicable = false;
                    isManualDiscountApplied = false;
                    foreach (DiscountSequenceKey sequenceKey in applicableDiscounts)
                    {
                        if (sequenceKey.DiscountID == manualDiscountID && (manualDiscountSequenceID == null || (manualDiscountSequenceID != null && sequenceKey.DiscountSequenceID == manualDiscountSequenceID)))
                        {
							selectedGroupDiscounts.Add(sequenceKey);
                            isManualDiscountApplicable = true;
                        }
                    }
                    if (isManualDiscountApplicable)
                    {
                        foreach (DiscountDetail detail in discountDetailsByType)
                        {
                            if (detail.SkipDiscount != true && cachedDiscountTypes[detail.DiscountID].SkipDocumentDiscounts)
                            {
                                discountDetails.Cache.RaiseExceptionHandling(DiscountID, currentDiscountDetailLine, manualDiscountID, new PXSetPropertyException(Messages.DocumentDicountCanNotBeAdded, PXErrorLevel.Error));
                                return;
                            }
                        }
                        if (((curyTotalLineAmount / 100 * discountLimit) > curyTotalGroupDiscountAmount) || discountLimit == 0m)
                        {
                            if (curyTotalLineAmount != 0)
                            {
                                DiscountDetailLine discount = de.SelectApplicableDiscount(sender, GetDiscountedLine(sender, documentDetails.First()), selectedGroupDiscounts, totalLineAmount - totalGroupDiscountAmount, 0, DiscountType.Document, date);

                                if (discount.DiscountID != null)
                                {
                                    DiscountDetail newDiscountDetail = CreateDiscountDetailsLine<DiscountDetail>(sender, discount, GetDiscountedLine(sender, documentDetails.First()), curyTotalLineAmount - curyTotalGroupDiscountAmount, 0, date, DiscountType.Document);
                                    newDiscountDetail.CuryDiscountableAmt = curyTotalLineAmount - curyTotalGroupDiscountAmount;
                                    currentDiscountDetailLine.DiscountSequenceID = newDiscountDetail.DiscountSequenceID;
                                    currentDiscountDetailLine.Type = newDiscountDetail.Type;
                                    currentDiscountDetailLine.CuryDiscountableAmt = newDiscountDetail.CuryDiscountableAmt;
                                    currentDiscountDetailLine.DiscountableQty = 0;
                                    currentDiscountDetailLine.DiscountPct = newDiscountDetail.DiscountPct;
                                    currentDiscountDetailLine.CuryDiscountAmt = newDiscountDetail.CuryDiscountAmt;
                                    currentDiscountDetailLine.FreeItemID = null;
                                    currentDiscountDetailLine.FreeItemQty = 0;
                                    isManualDiscountApplied = true;

                                    if ((curyTotalLineAmount / 100 * discountLimit) < (newDiscountDetail.CuryDiscountAmt + curyTotalGroupDiscountAmount))
                                    {
                                        SetDiscountLimitException(sender, discountDetails, DiscountType.Document, PXMessages.LocalizeFormatNoPrefix(AR.Messages.DocDiscountExceedLimit, discountLimit), currentDiscountDetailLine);
                                    }
                                }
                                else
                                {
                                    RemoveUnapplicableDiscountDetails(sender, discountDetails, null, DiscountType.Document);
                                }
                            }
                        }
                        else
                        {
                            SetDiscountLimitException(sender, discountDetails, DiscountType.Group, AR.Messages.GroupDiscountExceedLimit);
                        }
                        CalculateDocumentDiscountRate(sender, documentDetails, currentLine, GetDiscountDetailsByType(sender, discountDetails, DiscountType.Document), false);
                    }
                }
                if (!isManualDiscountApplicable || !isManualDiscountApplied)
                {
                    if (manualDiscountID != null && manualDiscountSequenceID == null)
                        discountDetails.Cache.RaiseExceptionHandling(DiscountID, currentDiscountDetailLine, manualDiscountID, new PXSetPropertyException(Messages.NoApplicableSequenceFound, PXErrorLevel.Error));
                    if (manualDiscountID != null && manualDiscountSequenceID != null)
                    {
                        currentDiscountDetailLine.DiscountSequenceID = null;
						discountDetails.Cache.RaiseExceptionHandling(DiscountSequenceID, currentDiscountDetailLine, null, new PXSetPropertyException(Messages.UnapplicableSequence, PXErrorLevel.Error, manualDiscountSequenceID));
                    }
                }
            }
        }
        #endregion

        #region Prices

        public struct UnitPriceVal
        {
            public decimal? CuryUnitPrice;
            public bool isBAccountSpecific;
            public bool isPromotional;
        }

        public static UnitPriceVal GetUnitPrice(PXCache sender, Line line, int? locationID, string curyID, DateTime date)
        {
            DiscountEngine de = new DiscountEngine();

            AmountLineFields afields = GetDiscountDocumentLine(sender, line);
            LineEntitiesFields efields = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);

            UnitPriceVal unitPriceVal = new UnitPriceVal();
            if (efields.CustomerID != null && efields.InventoryID != null)
            {
				Location loc = DiscountEngine.GetLocationByLocationID(sender, efields.CustomerID, locationID);
				ARSalesPrice ARsp = de.GetARPrice(sender, (int)efields.InventoryID, (int)efields.CustomerID, loc.CPriceClassID, curyID, afields.UOM, (decimal)afields.Quantity, date);
                if (ARsp != null)
                {
                    unitPriceVal.CuryUnitPrice = ARsp.SalesPrice;
                    unitPriceVal.isBAccountSpecific = ARsp.CustomerID != null ? true : false;
                    unitPriceVal.isPromotional = ARsp.IsPromotionalPrice ?? false;
                }
            }
            //Ignore discounts when vendor-specific price found is disabled for now 
            /*else if (efields.VendorID != null)
            {
                AP.APVendorPrice APsp = de.GetAPPrice(sender, (int)efields.InventoryID, (int)efields.VendorID, curyID, afields.UOM, (decimal)afields.Quantity, date);
                if (APsp != null)
                {
                    unitPriceVal.CuryUnitPrice = APsp.SalesPrice;
                    unitPriceVal.isBAccountSpecific = APsp.VendorID != null ? true : false;
                    unitPriceVal.isPromotional = APsp.IsPromotionalPrice ?? false;
                }
            }*/
            return unitPriceVal;
        }

        public static void SetUnitPrice(PXCache sender, Line line, UnitPriceVal unitPriceVal)
        {
            AmountLineFields afields = GetDiscountDocumentLine(sender, line);
            if (unitPriceVal.CuryUnitPrice != null)
            {
                decimal? oldCuryUnitPrice = afields.CuryUnitPrice;
                afields.CuryUnitPrice = unitPriceVal.CuryUnitPrice;
                afields.RaiseFieldUpdated<AmountLineFields.curyUnitPrice>(oldCuryUnitPrice);
            }
        }
        #endregion

        #region Total Discount Amount and total Free Item quantity + prorate
        /// <summary>
        /// Calculates total discount amount. Prorates Amount discounts if needed.
        /// </summary>
        /// <returns>Returns total CuryDiscountAmt</returns>
        public static decimal CalculateDiscount(PXCache sender, DiscountDetailLine discount, DiscountLineFields dline, decimal curyAmount, decimal quantity, DateTime date, string type)
        {
            decimal totalDiscount = 0m;

            if (discount.DiscountedFor == "A")
            {
                if ((bool)discount.Prorate && discount.AmountFrom != null && discount.AmountFrom != 0m)
                {
                    DiscountEngine de = new DiscountEngine();

                    DiscountDetailLine intDiscount = discount;
                    decimal intCuryLineAmount = curyAmount;
                    decimal intLineQty = quantity;
                    totalDiscount = 0m;

                    DiscountSequenceKey discountSequence = new DiscountSequenceKey();
                    discountSequence.DiscountID = discount.DiscountID;
                    discountSequence.DiscountSequenceID = discount.DiscountSequenceID;
                    HashSet<DiscountSequenceKey> ds = new HashSet<DiscountSequenceKey>();
					ds.Add(discountSequence);
                    do
                    {
                        if (discount.BreakBy == "A")
                        {
                            if (intCuryLineAmount < (intDiscount.AmountFrom ?? 0m))
                            {
                                intDiscount = de.SelectApplicableDiscount(sender, dline, discountSequence, intCuryLineAmount, intLineQty, type, date);
                                if (intDiscount.DiscountID != null)
                                {
                                    totalDiscount += intDiscount.Discount ?? 0m;
                                    intCuryLineAmount -= intDiscount.AmountFrom ?? 0m;
                                }
                                else
                                {
                                    intDiscount = new DiscountDetailLine();
                                }
                            }
                            else
                            {
                                totalDiscount += intDiscount.Discount ?? 0m;
                                intCuryLineAmount -= intDiscount.AmountFrom ?? 0m;
                            }

                        }
                        else
                        {
                            if (intLineQty < (intDiscount.AmountFrom ?? 0m))
                            {
                                intDiscount = de.SelectApplicableDiscount(sender, dline, discountSequence, intCuryLineAmount, intLineQty, type, date);
                                if (intDiscount.DiscountID != null)
                                {
                                    totalDiscount += intDiscount.Discount ?? 0m;
                                    intLineQty -= intDiscount.AmountFrom ?? 0m;
                                }
                                else
                                {
                                    intDiscount = new DiscountDetailLine();
                                }
                            }
                            else
                            {
                                totalDiscount += intDiscount.Discount ?? 0m;
                                intLineQty -= intDiscount.AmountFrom ?? 0m;
                            }

                        }
                        if (intDiscount.AmountFrom == 0m) intDiscount = new DiscountDetailLine();
                    }
                    while (intDiscount.DiscountID != null);
                }
                else
                {
                    totalDiscount = discount.Discount ?? 0m;
                }
            }
            else if (discount.DiscountedFor == "P")
            {
                totalDiscount = curyAmount / 100 * (discount.Discount ?? 0m);
            }
            return totalDiscount;
        }

        /// <summary>
        /// Calculates total free item quantity. Prorates Free-Item discounts if needed.
        /// </summary>
        /// <returns>Returns total FreeItemQty</returns>
        public static decimal CalculateFreeItemQuantity(PXCache sender, DiscountDetailLine discount, DiscountLineFields dline, decimal curyAmount, decimal quantity, DateTime date, string type)
        {
            decimal totalFreeItems = 0m;

            if (discount.DiscountedFor == "F")
            {
                if ((bool)discount.Prorate && discount.AmountFrom != null && discount.AmountFrom != 0m)
                {
                    DiscountEngine de = new DiscountEngine();
                    DiscountDetailLine intDiscount = discount;
                    decimal intCuryLineAmount = curyAmount;
                    decimal intLineQty = quantity;
                    totalFreeItems = 0m;

                    DiscountSequenceKey discountSequence = new DiscountSequenceKey();
                    discountSequence.DiscountID = discount.DiscountID;
                    discountSequence.DiscountSequenceID = discount.DiscountSequenceID;
                    do
                    {
                        if (discount.BreakBy == "A")
                        {
                            if (intCuryLineAmount < (intDiscount.AmountFrom ?? 0m))
                            {
                                intDiscount = de.SelectApplicableDiscount(sender, dline, discountSequence, intCuryLineAmount, intLineQty, type, date);
                                if (intDiscount.DiscountID != null)
                                {
                                    totalFreeItems += intDiscount.freeItemQty ?? 0m;
                                    intCuryLineAmount -= intDiscount.AmountFrom ?? 0m;
                                }
                                else
                                {
                                    intDiscount = new DiscountDetailLine();
                                }
                            }
                            else
                            {
                                totalFreeItems += intDiscount.freeItemQty ?? 0m;
                                intCuryLineAmount -= intDiscount.AmountFrom ?? 0m;
                            }
                        }
                        else
                        {
                            if (intLineQty < (intDiscount.AmountFrom ?? 0m))
                            {
                                intDiscount = de.SelectApplicableDiscount(sender, dline, discountSequence, intCuryLineAmount, intLineQty, type, date);
                                if (intDiscount.DiscountID != null)
                                {
                                    totalFreeItems += intDiscount.freeItemQty ?? 0m;
                                    intLineQty -= intDiscount.AmountFrom ?? 0m;
                                }
                                else
                                {
                                    intDiscount = new DiscountDetailLine();
                                }
                            }
                            else
                            {
                                totalFreeItems += intDiscount.freeItemQty ?? 0m;
                                intLineQty -= intDiscount.AmountFrom ?? 0m;
                            }
                        }
                        if (intDiscount.AmountFrom == 0m) intDiscount = new DiscountDetailLine();
                    }
                    while (intDiscount.DiscountID != null);
                }
                else
                {
                    totalFreeItems = discount.freeItemQty ?? 0m;
                }
            }
            return totalFreeItems;
        }
        #endregion

        #region Utils
        //Returns total discount amount
        public static void GetDiscountAmountByType<DiscountDetail>(PXCache sender, List<DiscountDetail> discountDetails, string type, out decimal totalDiscountAmount, out decimal curyTotalDiscountAmt)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            totalDiscountAmount = 0m;
            curyTotalDiscountAmt = 0m;
            foreach (DiscountDetail detail in discountDetails)
            {
                if (detail.Type == type && detail.SkipDiscount != true)
                {
                    curyTotalDiscountAmt += detail.CuryDiscountAmt ?? 0;
                    decimal baseDiscountAmt;
                    PXCurrencyAttribute.CuryConvBase(sender, detail, detail.CuryDiscountAmt ?? 0m, out baseDiscountAmt, true);
                    totalDiscountAmount += baseDiscountAmt;
                }
            }
        }

        //Collect manual Discounts
        public static List<DiscountSequenceKey> CollectManualDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, string type)
             where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountSequenceKey> manualDiscounts = new List<DiscountSequenceKey>();
            List<DiscountDetail> trace = GetDiscountDetailsByType(sender, discountDetails, type);
            foreach (DiscountDetail discountDetail in trace)
            {
                if (discountDetail.IsManual == true)
                {
                    DiscountSequenceKey dKey = new DiscountSequenceKey();
                    dKey.DiscountID = discountDetail.DiscountID;
                    dKey.DiscountSequenceID = discountDetail.DiscountSequenceID;
                    manualDiscounts.Add(dKey);
                }
            }
            return manualDiscounts;
        }

        //Removes all unapplicable manual Discounts
        public static HashSet<DiscountSequenceKey> RemoveUnapplicableManualDiscounts<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, HashSet<DiscountSequenceKey> allApplicableDiscounts, string type)
             where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();
            List<DiscountDetail> trace = GetDiscountDetailsByType(sender, discountDetails, type);
            HashSet<DiscountSequenceKey> applicableDiscounts = new HashSet<DiscountSequenceKey>();
            foreach (DiscountSequenceKey discountSequence in allApplicableDiscounts)
            {
                if (cachedDiscountTypes[discountSequence.DiscountID].IsManual)
                {
                    foreach (DiscountDetail discountDetail in trace)
                    {
                        DiscountSequenceKey discountSequenceKey = new DiscountSequenceKey();
                        discountSequenceKey.DiscountID = discountDetail.DiscountID;
                        discountSequenceKey.DiscountSequenceID = discountDetail.DiscountSequenceID;
                        if (discountSequence.Equals(discountSequenceKey) && cachedDiscountTypes[discountDetail.DiscountID].IsManual)
                        {
							applicableDiscounts.Add(discountSequence);
                        }
                    }
                }
                else
                {
					applicableDiscounts.Add(discountSequence);
                }
            }
            return applicableDiscounts;
        }

        //Call to remove all unapplicable Discount Details
        public static void RemoveUnapplicableDiscountDetails<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, List<DiscountSequenceKey> newDiscountDetails, string type, bool removeManual = true)
             where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountDetail> trace = GetDiscountDetailsByType(sender, discountDetails, type);
            foreach (DiscountDetail discountDetail in trace)
            {
                if (discountDetail.IsManual == false || (discountDetail.IsManual == true && removeManual))
                {
                    if (newDiscountDetails != null)
                    {
                        DiscountSequenceKey discountSequence = new DiscountSequenceKey();
                        discountSequence.DiscountID = discountDetail.DiscountID;
                        discountSequence.DiscountSequenceID = discountDetail.DiscountSequenceID;
                        if (!newDiscountDetails.Contains(discountSequence))
                        {
                            UpdateUnapplicableDiscountLine(discountDetails, discountDetail);
                        }
                    }
                    else
                    {
                        UpdateUnapplicableDiscountLine(discountDetails, discountDetail);
                    }
                }
            }
        }

        private static void UpdateUnapplicableDiscountLine<DiscountDetail>(PXSelectBase<DiscountDetail> discountDetails, DiscountDetail discountDetail)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            if (discountDetail.SkipDiscount != true)
                discountDetails.Delete(discountDetail);
            else
            {
                discountDetail.CuryDiscountableAmt = 0m;
                discountDetail.CuryDiscountAmt = 0m;
                discountDetail.DiscountableQty = 0m;
                discountDetail.DiscountPct = 0m;
                discountDetails.Update(discountDetail);
            }
        }
        //Updates or inserts Discount Detail
        public static DiscountDetail UpdateInsertOneDiscountTraceLine<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, DiscountDetail trace, DiscountDetail newTrace)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            //DiscountDetail trace = GetDiscountDetail(sender, discountDetails, newTrace.DiscountID, newTrace.DiscountSequenceID, newTrace.Type);

            if (trace != null)
            {
                trace.CuryDiscountableAmt = newTrace.CuryDiscountableAmt;
                trace.DiscountableQty = newTrace.DiscountableQty;
                trace.CuryDiscountAmt = newTrace.CuryDiscountAmt ?? 0;
                trace.DiscountPct = newTrace.DiscountPct;
                trace.FreeItemID = newTrace.FreeItemID;
                trace.FreeItemQty = newTrace.FreeItemQty;

                return discountDetails.Update(trace);
            }
            else
            {
                return discountDetails.Insert(newTrace);
            }
        }
        
        //Updates or inserts Discount Details
        public static DiscountDetail UpdateInsertDiscountTrace<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, DiscountDetail newTrace)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            DiscountDetail trace = GetDiscountDetail(sender, discountDetails, newTrace.DiscountID, newTrace.DiscountSequenceID, newTrace.Type);

            if (trace != null)
            {
                trace.CuryDiscountableAmt = newTrace.CuryDiscountableAmt;
                trace.DiscountableQty = newTrace.DiscountableQty;
                trace.CuryDiscountAmt = newTrace.CuryDiscountAmt ?? 0m;
                trace.DiscountPct = newTrace.DiscountPct;
                trace.FreeItemID = newTrace.FreeItemID;
                trace.FreeItemQty = newTrace.FreeItemQty;

                return discountDetails.Update(trace);
            }
            else
            {
                return discountDetails.Insert(newTrace);
            }

        }

		//Returns dictionary of discountable entities
		private static HashSet<KeyValuePair<object, string>> GetDiscountEntitiesDiscounts(PXCache sender, Line line, int? locationID, bool isLineOrGroupDiscount, int? branchID = null, int? inventoryID = null, int? customerID = null)
		{
			LineEntitiesFields lineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);

			HashSet<KeyValuePair<object, string>> entities = new HashSet<KeyValuePair<object, string>>();

			if (lineEntities.VendorID != null && lineEntities.CustomerID == null)
			{
				entities.Add(new KeyValuePair<object, string>(lineEntities.VendorID, DiscountTarget.Vendor));

				if (isLineOrGroupDiscount)
				{
					if (locationID != null)
					{
						entities.Add(new KeyValuePair<object, string>(locationID, DiscountTarget.VendorLocation));
					}

					int? entityInventoryID = lineEntities.InventoryID ?? inventoryID;
					if (entityInventoryID != null)
					{
						entities.Add(new KeyValuePair<object, string>(entityInventoryID, DiscountTarget.Inventory));
						InventoryItem item = DiscountEngine.GetInventoryItemByID(sender, entityInventoryID);
						if (item != null && item.PriceClassID != null)
						{
							entities.Add(new KeyValuePair<object, string>(item.PriceClassID, DiscountTarget.InventoryPrice));
						}
					}
				}
			}
			else
			{
				int? entityCustomerID = lineEntities.CustomerID ?? customerID;
				if (entityCustomerID != null)
				{
					entities.Add(new KeyValuePair<object, string>(entityCustomerID, DiscountTarget.Customer));
					if (locationID != null)
					{
						Location location = DiscountEngine.GetLocationByLocationID(sender, entityCustomerID, locationID);
						if (location != null && location.CPriceClassID != null)
						{
							entities.Add(new KeyValuePair<object, string>(location.CPriceClassID, DiscountTarget.CustomerPrice));
						}
					}
				}

				int? entityBranchID = lineEntities.BranchID ?? branchID;
				if (entityBranchID != null)
					entities.Add(new KeyValuePair<object, string>(entityBranchID, DiscountTarget.Branch));

				if (isLineOrGroupDiscount)
				{
					int? entityInventoryID = lineEntities.InventoryID ?? inventoryID;
					if (entityInventoryID != null)
					{
						entities.Add(new KeyValuePair<object, string>(entityInventoryID, DiscountTarget.Inventory));
						InventoryItem item = DiscountEngine.GetInventoryItemByID(sender, entityInventoryID);
						if (item != null && item.PriceClassID != null)
						{
							entities.Add(new KeyValuePair<object, string>(item.PriceClassID, DiscountTarget.InventoryPrice));
						}
					}
					if (lineEntities.SiteID != null)
						entities.Add(new KeyValuePair<object, string>(lineEntities.SiteID, DiscountTarget.Warehouse));
				}
			}
			return entities;
		}

        //Returns AmountLineFields. Add new types here.
        //QuantityField: Quantity
        //CuryUnitPriceField: Cury Unit Price
        //CuryExtPriceField: Quantity * Cury Unit Price field
        //CuryLineAmountField: (Quantity * Cury Unit Price field) - Cury Discount Amount field
        public static AmountLineFields GetDiscountDocumentLine(PXCache sender, Line line)
        {
            if (typeof(Line) == typeof(SOLine))
            {
                return new AmountLineFields<orderQty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (typeof(Line) == typeof(ARTran))
            {
                return new AmountLineFields<qty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (typeof(Line) == typeof(PX.Objects.AP.APTran))
            {
                return new AmountLineFields<qty, curyUnitCost, curyLineAmt, curyTranAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (typeof(Line) == typeof(POLine))
            {
                return new AmountLineFields<orderQty, curyUnitCost, curyLineAmt, curyExtCost, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (typeof(Line) == typeof(POReceiptLine))
            {
                return new AmountLineFields<receiptQty, curyUnitCost, curyLineAmt, curyExtCost, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (typeof(Line) == typeof(SOShipLine))
            {
                return new AmountLineFields<shippedQty, curyUnitCost, curyLineAmt, curyExtCost, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            return new AmountLineFields<orderQty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
        }

        //Returns line fields to be updated
        public static DiscountLineFields GetDiscountedLine(PXCache sender, Line line)
        {
            return new DiscountLineFields<skipDisc, curyDiscAmt, discPct, discountID, discountSequenceID, manualDisc, manualPrice, lineType, isFree>(sender, line);
        }

        //Returns list of Discount Details by type
        public static List<DiscountDetail> GetDiscountDetailsByType<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, string type, object[] parameters = null)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountDetail> discountDetailToReturn = new List<DiscountDetail>();

            if (parameters == null)
            {
                foreach (DiscountDetail detail in discountDetails.Select())
                {
                    if (detail.Type == type)
                        discountDetailToReturn.Add(detail);
                }
            }
            else
            {
                PXCache cache = sender.Graph.Caches[typeof(DiscountDetail)];

                Type DiscountDetail_Type = cache.GetBqlField(TypeFieldName);

                #region Compose WHERE type
                Type whereType = BqlCommand.Compose(
                        typeof(Where<,>),
                        DiscountDetail_Type,
                        typeof(Equal<>),
                        typeof(Required<>),
                        DiscountDetail_Type
                        );
                #endregion

                PXView view = new PXView(sender.Graph, false, discountDetails.View.BqlSelect.WhereAnd(whereType));
                view.Cache.ClearQueryCache();

                foreach (DiscountDetail detail in (view.SelectMulti(parameters)))
                {
                    discountDetailToReturn.Add(detail);
                }
            }
            return discountDetailToReturn;
        }

        //Returns one Discount Details line
        private static DiscountDetail GetDiscountDetail<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, string discountID, string discountSequenceID, string type)
        where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            PXCache cache = sender.Graph.Caches[typeof(DiscountDetail)];

            Type DiscountDetail_DiscountID = cache.GetBqlField(DiscountID);
            Type DiscountDetail_DiscountSequenceID = cache.GetBqlField(DiscountSequenceID);
            Type DiscountDetail_Type = cache.GetBqlField(TypeFieldName);

            #region Compose WHERE type
            /* Composition:
             Where<SOOrderDiscountDetail.discountID, Equal<Required<SOOrderDiscountDetail.discountID>>,
                        And<SOOrderDiscountDetail.discountSequenceID, Equal<Required<SOOrderDiscountDetail.discountSequenceID>>,
                        And<SOOrderDiscountDetail.type, Equal<Required<SOOrderDiscountDetail.type>>>>>
            */

            Type whereType = BqlCommand.Compose(
                    typeof(Where<,,>),
                    DiscountDetail_DiscountID,
                    typeof(Equal<>),
                    typeof(Required<>),
                    DiscountDetail_DiscountID,
                    typeof(And<,,>),
                    DiscountDetail_DiscountSequenceID,
                    typeof(Equal<>),
                    typeof(Required<>),
                    DiscountDetail_DiscountSequenceID,
                    typeof(And<,>),
                    DiscountDetail_Type,
                    typeof(Equal<>),
                    typeof(Required<>),
                    DiscountDetail_Type
                    );

            #endregion

            PXView view = new PXView(sender.Graph, false, discountDetails.View.BqlSelect.WhereAnd(whereType));
            DiscountDetail ddetail = (DiscountDetail)view.SelectSingle(discountID, discountSequenceID, type);
            if (ddetail == null)
            {
                view.Cache.ClearQueryCache();
                ddetail = (DiscountDetail)view.SelectSingle(discountID, discountSequenceID, type);
            }
            return ddetail;

        }

        //Returns list of Document Details lines
        private static List<Line> GetDocumentDetails(PXCache sender, PXSelectBase<Line> documentDetails, object[] parameters = null)
        {
            //Type Line_IsManual = sender.GetBqlField(IsManualDiscount);

            PXView view = documentDetails.View;

            List<Line> documentDetailsToReturn = new List<Line>();
            foreach (var detail in (parameters == null ? view.SelectMulti() : view.SelectMulti(parameters)))
            {
                if (detail.GetType().Equals(typeof(Line)))
                {
                    DiscountLineFields discountedLine = GetDiscountedLine(sender, (Line)detail);
                    if ((discountedLine.IsFree == null || discountedLine.IsFree == false) && (discountedLine.LineType == null || discountedLine.LineType != SOLineType.Discount))
                        documentDetailsToReturn.Add((Line)detail);
                }
                else
                {
                    Line detailLine = ((PXResult<Line>)detail)[typeof(Line)] as Line;
                    DiscountLineFields discountedLine = GetDiscountedLine(sender, detailLine);
                    if ((discountedLine.IsFree == null || discountedLine.IsFree == false) && (discountedLine.LineType == null || discountedLine.LineType != SOLineType.Discount))
                        documentDetailsToReturn.Add(detailLine);
                }
            }

            return documentDetailsToReturn;
        }

        /// <summary>
        /// Sums line amounts. Returns modified totalLineAmt and curyTotalLineAmt
        /// </summary>
        public static void SumAmounts(PXCache sender, List<Line> lines, out decimal totalLineAmt, out decimal curyTotalLineAmt)
        {
            totalLineAmt = 0m;
            curyTotalLineAmt = 0m;

			ConcurrentDictionary<string, DiscountCode> cachedDiscountTypes = GetCachedDiscountCodes();
            foreach (Line line in lines)
            {
                bool excludeFromDiscountableAmount = false;
                AmountLineFields item = GetDiscountDocumentLine(sender, line);
                DiscountLineFields discountedLine = GetDiscountedLine(sender, line);

                if (discountedLine.LineType == null || discountedLine.LineType != SOLineType.Discount)
                {
                    if (discountedLine.DiscountID != null)
                        excludeFromDiscountableAmount = cachedDiscountTypes[discountedLine.DiscountID].ExcludeFromDiscountableAmt;

                    decimal curyLineAmt;
                    if (!excludeFromDiscountableAmount)
                        curyLineAmt = item.CuryLineAmount ?? 0;
                    else
                        curyLineAmt = 0;

                    decimal baseLineAmt;
                    PXCurrencyAttribute.CuryConvBase(sender, item, curyLineAmt, out baseLineAmt, true);
                    totalLineAmt += baseLineAmt;
                    curyTotalLineAmt += curyLineAmt;
                }
            }
        }

        public static void ClearDiscount(PXCache sender, Line line)
        {
            DiscountLineFields discountLine = GetDiscountedLine(sender, line);
            discountLine.DiscountID = null;
            discountLine.DiscountSequenceID = null;
        }

        public static bool CompareRows<Row>(Row row1, Row row2, params string[] ignore) where Row : class
        {
            if (row1 != null && row2 != null)
            {
                Type type = typeof(Row);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo propertyInfo in
                    type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(propertyInfo.Name))
                    {
                        object row1Value = type.GetProperty(propertyInfo.Name).GetValue(row1, null);
                        object row2Value = type.GetProperty(propertyInfo.Name).GetValue(row2, null);
                        if (row1Value != row2Value && (row1Value == null || !row1Value.Equals(row2Value)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return row1 == row2;
        }

        public static string GetLineDiscountTarget(PXCache sender, Line line)
        {
            LineEntitiesFields efields = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, line);
            string LineDiscountTarget = LineDiscountTargetType.ExtendedPrice;
            if (efields != null && efields.VendorID != null && efields.CustomerID == null)
            {
                AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.SelectSingleBound(sender.Graph, null, efields.VendorID);
                if (vendor != null) LineDiscountTarget = vendor.LineDiscountTarget;
            }
            else
            {
                ARSetup arsetup = PXSelect<ARSetup>.Select(sender.Graph);
                if (arsetup != null) LineDiscountTarget = arsetup.LineDiscountTarget;
            }
            return LineDiscountTarget;
        }

        public static void ValidateDiscountDetails<DiscountDetail>(PXSelectBase<DiscountDetail> discountDetails)
            where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
        {
            List<DiscountDetail> documentDiscounts = GetDiscountDetailsByType<DiscountDetail>(discountDetails.Cache, discountDetails, DiscountType.Document);
            if (documentDiscounts.Count > 1)
            {
                discountDetails.Cache.RaiseExceptionHandling(DiscountID, documentDiscounts.First(), documentDiscounts.First().DiscountID, new PXSetPropertyException(AR.Messages.OnlyOneDocumentDiscountAllowed, PXErrorLevel.Error));
                throw new PXSetPropertyException(AR.Messages.OnlyOneDocumentDiscountAllowed, PXErrorLevel.RowError);
            }
            List<DiscountDetail> groupDiscounts = GetDiscountDetailsByType<DiscountDetail>(discountDetails.Cache, discountDetails, DiscountType.Group);
            foreach (DiscountDetail outerDiscount in groupDiscounts)
            {
                foreach (DiscountDetail innerDiscount in groupDiscounts)
                {
                    if (outerDiscount.LineNbr != innerDiscount.LineNbr && outerDiscount.DiscountID == innerDiscount.DiscountID && outerDiscount.DiscountSequenceID == innerDiscount.DiscountSequenceID)
                    {
                        discountDetails.Cache.RaiseExceptionHandling(DiscountID, innerDiscount, innerDiscount.DiscountID, new PXSetPropertyException(AR.Messages.DuplicateGroupDiscount, PXErrorLevel.RowError));
                        throw new PXSetPropertyException(AR.Messages.DuplicateGroupDiscount, PXErrorLevel.RowError);
                    }
                }
            }
        }

        #endregion
    }

    #region Structs

    /// <summary>
    /// Used for calculation of intersections of discounts. Every new value should be power of two.
    /// </summary>
    [Flags]
    public enum ApplicableToCombination
    {
        None = 0,
        Customer = 1,
        InventoryItem = 2,
        CustomerPriceClass = 4,
        InventoryPriceClass = 8,
        Vendor = 16,
        Warehouse = 32,
        Branch = 64,
        Location = 128,
        Unconditional = 256
    }

    /// <summary>
    /// Stores Discount Codes. Both AP and AR. IsVendorDiscount should be true for AP discounts.
    /// </summary>
    public struct DiscountCode
    {
        public bool IsVendorDiscount;
        public int? VendorID;
        public string Type;
        public ApplicableToCombination ApplicableToEnum;
        public bool IsManual;
        public bool ExcludeFromDiscountableAmt;
        public bool SkipDocumentDiscounts;
    }

    /// <summary>
    /// Stores discount sequence keys
    /// </summary>
    public struct DiscountSequenceKey
    {
        public string DiscountID;
        public string DiscountSequenceID;
    }

    /// <summary>
    /// Stores Discount Details lines combined with Discount Code and Discount Sequence fields
    /// </summary>
    public struct DiscountDetailLine
    {
        public string DiscountID;
        public string DiscountSequenceID;
        public ApplicableToCombination ApplicableToCombined;
        public string Type; // L, G or P
        public string DiscountedFor; // A or P or F
        public string BreakBy;
        public decimal? AmountFrom; //Amount or Quantity
        public decimal? AmountTo; //Amount or Quantity
        public decimal? Discount; //Amount or Percent
        public int? freeItemID;
        public decimal? freeItemQty;
        public bool? Prorate;
    }

    /// <summary>
    /// Used in group discounts only
    /// </summary>
    public struct DiscountableValues
    {
        public decimal? CuryDiscountableAmount;
        public decimal? DiscountableQuantity;
    }

    public struct DiscountResult
    {
        #region Fields
        private decimal? discount;
        private bool isAmount;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets Discount. Its either a Percent or an Amount. Check <see cref="IsAmount"/> property.
        /// </summary>
        public decimal? Discount
        {
            get { return discount; }
        }

        /// <summary>
        /// Returns true if Discount is an Amount; otherwise false.
        /// </summary>
        public bool IsAmount
        {
            get { return isAmount; }
        }

        /// <summary>
        /// Returns True if No Discount was found.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Discount == null || Discount == 0m; 
            }
        }
        #endregion

        internal DiscountResult(decimal? discount, bool isAmount)
        {
            this.discount = discount;
            this.isAmount = isAmount;
        }
    }

    #endregion

    #region Filters
    /// <summary>
    /// Recalculate Prices and Discounts filter
    /// </summary>
    [Serializable()]
    public partial class RecalcDiscountsParamFilter : IBqlTable
    {
        #region RecalcTarget
        public abstract class recalcTarget : PX.Data.IBqlField
        {
        }
        protected String _RecalcTarget;
        [PXDBString(3, IsFixed = true)]
        [PXDefault(RecalcDiscountsParamFilter.AllLines)]
        [PXStringList(
                new string[] { RecalcDiscountsParamFilter.CurrentLine, RecalcDiscountsParamFilter.AllLines },
                new string[] { AR.Messages.CurrentLine, AR.Messages.AllLines })]
        [PXUIField(DisplayName = "Recalculate")]
        public virtual String RecalcTarget
        {
            get
            {
                return this._RecalcTarget;
            }
            set
            {
                this._RecalcTarget = value;
            }
        }
        public const string CurrentLine = "LNE";
        public const string AllLines = "ALL";
        #endregion
        #region RecalcUnitPrices
        public abstract class recalcUnitPrices : PX.Data.IBqlField
        {
        }
        protected Boolean? _RecalcUnitPrices;
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Set Current Unit Prices", Visible = true)]
        public virtual Boolean? RecalcUnitPrices
        {
            get
            {
                return this._RecalcUnitPrices;
            }
            set
            {
                this._RecalcUnitPrices = value;
            }
        }
        #endregion
        #region OverrideManualPrices
        public abstract class overrideManualPrices : PX.Data.IBqlField
        {
        }
        protected Boolean? _OverrideManualPrices;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIEnabled(typeof(RecalcDiscountsParamFilter.recalcUnitPrices))]
        [PXFormula(typeof(Switch<Case<Where<RecalcDiscountsParamFilter.recalcUnitPrices, Equal<False>>, False>, RecalcDiscountsParamFilter.overrideManualPrices>))]
        [PXUIField(DisplayName = "Override Manual Prices", Visible = true)]
        public virtual Boolean? OverrideManualPrices
        {
            get
            {
                return this._OverrideManualPrices;
            }
            set
            {
                this._OverrideManualPrices = value;
            }
        }
        #endregion
        #region RecalcDiscounts
        public abstract class recalcDiscounts : PX.Data.IBqlField
        {
        }
        protected Boolean? _RecalcDiscounts;
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Recalculate Discounts")]
        public virtual Boolean? RecalcDiscounts
        {
            get
            {
                return this._RecalcDiscounts;
            }
            set
            {
                this._RecalcDiscounts = value;
            }
        }
        #endregion
        #region OverrideManualDiscounts
        public abstract class overrideManualDiscounts : PX.Data.IBqlField
        {
        }
        protected Boolean? _OverrideManualDiscounts;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIEnabled(typeof(RecalcDiscountsParamFilter.recalcDiscounts))]
        [PXFormula(typeof(Switch<Case<Where<RecalcDiscountsParamFilter.recalcDiscounts, Equal<False>>, False>, RecalcDiscountsParamFilter.overrideManualDiscounts>))]
        [PXUIField(DisplayName = "Override Manual Line Discounts")]
        public virtual Boolean? OverrideManualDiscounts
        {
            get
            {
                return this._OverrideManualDiscounts;
            }
            set
            {
                this._OverrideManualDiscounts = value;
            }
        }
        #endregion
        #region UseRecalcFilter
        public abstract class useRecalcFilter : PX.Data.IBqlField
        {
        }
        protected Boolean? _UseRecalcFilter;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? UseRecalcFilter
        {
            get
            {
                return this._UseRecalcFilter;
            }
            set
            {
                this._UseRecalcFilter = value;
            }
        }
        #endregion
    }

    #endregion

    #region Discount Targets and options

    public static class DiscountType
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { Line, Group, Document },
                new string[] { Messages.Line, Messages.Group, Messages.Document }) { ; }
        }
        public const string Line = "L";
        public const string Group = "G";
        public const string Document = "D";
        public const string Flat = "F";

        public class LineDiscount : Constant<string>
        {
            public LineDiscount() : base(DiscountType.Line) { ;}
        }

        public class GroupDiscount : Constant<string>
        {
            public GroupDiscount() : base(DiscountType.Group) { ;}
        }

        public class DocumentDiscount : Constant<string>
        {
            public DocumentDiscount() : base(DiscountType.Document) { ;}
        }

        //to del
        public class FlatDiscount : Constant<string>
        {
            public FlatDiscount() : base(DiscountType.Flat) { ;}
        }
    }
    
    public static class DiscountOption
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { Percent, Amount, FreeItem },
                new string[] { Messages.Percent, Messages.Amount, Messages.FreeItem }) { ; }
        }
        public const string Percent = "P";
        public const string Amount = "A";
        public const string FreeItem = "F";

        public class PercentDiscount : Constant<string>
        {
            public PercentDiscount() : base(DiscountOption.Percent) { ;}
        }
        public class AmountDiscount : Constant<string>
        {
            public AmountDiscount() : base(DiscountOption.Amount) { ;}
        }
        public class FreeItemDiscount : Constant<string>
        {
            public FreeItemDiscount() : base(DiscountOption.FreeItem) { ;}
        }

    }

    public static class BreakdownType
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { Quantity, Amount },
                new string[] { Messages.Quantity, Messages.Amount }) { ; }
        }
        public const string Quantity = "Q";
        public const string Amount = "A";

        public class QuantityBreakdown : Constant<string>
        {
            public QuantityBreakdown() : base(BreakdownType.Quantity) { ;}
        }
        public class AmountBreakdown : Constant<string>
        {
            public AmountBreakdown() : base(BreakdownType.Amount) { ;}
        }

    }

    /// <summary>
    /// Discount Targets
    /// </summary>
    public static class DiscountTarget
    {
        public const string Customer = "CU";
        public const string CustomerAndInventory = "CI";
        public const string CustomerAndInventoryPrice = "CP";
        public const string CustomerPrice = "CE";
        public const string CustomerPriceAndInventory = "PI";
        public const string CustomerPriceAndInventoryPrice = "PP";
        public const string CustomerAndBranch = "CB";
        public const string CustomerPriceAndBranch = "PB";

        public const string Warehouse = "WH";
        public const string WarehouseAndInventory = "WI";
        public const string WarehouseAndCustomer = "WC";
        public const string WarehouseAndInventoryPrice = "WP";
        public const string WarehouseAndCustomerPrice = "WE";

        public const string Branch = "BR";

        public const string Vendor = "VE";
        public const string VendorAndInventory = "VI";
        public const string VendorAndInventoryPrice = "VP";
        public const string VendorLocation = "VL";
        public const string VendorLocationAndInventory = "LI";

        public const string Inventory = "IN";
        public const string InventoryPrice = "IE";

        public const string Unconditional = "UN";

        public class customer : Constant<string>
        {
            public customer() : base(DiscountTarget.Customer) { ;}
        }
        public class customerAndInventory : Constant<string>
        {
            public customerAndInventory() : base(DiscountTarget.CustomerAndInventory) { ;}
        }
        public class customerAndInventoryPrice : Constant<string>
        {
            public customerAndInventoryPrice() : base(DiscountTarget.CustomerAndInventoryPrice) { ;}
        }
        public class customerPrice : Constant<string>
        {
            public customerPrice() : base(DiscountTarget.CustomerPrice) { ;}
        }
        public class customerPriceAndInventory : Constant<string>
        {
            public customerPriceAndInventory() : base(DiscountTarget.CustomerPriceAndInventory) { ;}
        }
        public class customerPriceAndInventoryPrice : Constant<string>
        {
            public customerPriceAndInventoryPrice() : base(DiscountTarget.CustomerPriceAndInventoryPrice) { ;}
        }
        public class customerAndBranch : Constant<string>
        {
            public customerAndBranch() : base(DiscountTarget.CustomerAndBranch) { ;}
        }
        public class customerPriceAndBranch : Constant<string>
        {
            public customerPriceAndBranch() : base(DiscountTarget.CustomerPriceAndBranch) { ;}
        }
        public class warehouse : Constant<string>
        {
            public warehouse() : base(DiscountTarget.Warehouse) { ;}
        }
        public class warehouseAndInventory : Constant<string>
        {
            public warehouseAndInventory() : base(DiscountTarget.WarehouseAndInventory) { ;}
        }
        public class warehouseAndCustomer : Constant<string>
        {
            public warehouseAndCustomer() : base(DiscountTarget.WarehouseAndCustomer) { ;}
        }
        public class warehouseAndInventoryPrice : Constant<string>
        {
            public warehouseAndInventoryPrice() : base(DiscountTarget.WarehouseAndInventoryPrice) { ;}
        }
        public class warehouseAndCustomerPrice : Constant<string>
        {
            public warehouseAndCustomerPrice() : base(DiscountTarget.WarehouseAndCustomerPrice) { ;}
        }
        public class branch : Constant<string>
        {
            public branch() : base(DiscountTarget.Branch) { ;}
        }
        public class vendor : Constant<string>
        {
            public vendor() : base(DiscountTarget.Vendor) { ;}
        }
        public class vendorAndInventory : Constant<string>
        {
            public vendorAndInventory() : base(DiscountTarget.VendorAndInventory) { ;}
        }
        public class vendorAndInventoryPrice : Constant<string>
        {
            public vendorAndInventoryPrice() : base(DiscountTarget.VendorAndInventoryPrice) { ;}
        }
        public class vendorLocation : Constant<string>
        {
            public vendorLocation() : base(DiscountTarget.VendorLocation) { ;}
        }
        public class vendorLocationAndInventory : Constant<string>
        {
            public vendorLocationAndInventory() : base(DiscountTarget.VendorLocationAndInventory) { ;}
        }
        public class inventory : Constant<string>
        {
            public inventory() : base(DiscountTarget.Inventory) { ;}
        }
        public class inventoryPrice : Constant<string>
        {
            public inventoryPrice() : base(DiscountTarget.InventoryPrice) { ;}
        }
        public class unconditional : Constant<string>
        {
            public unconditional() : base(DiscountTarget.Unconditional) { ;}
        }

    }
    #endregion

    #region Interfaces
    public interface IDiscountDetail
    {
        int? LineNbr { get; set; }
        bool? SkipDiscount { get; set; }
        string DiscountID { get; set; }
        string DiscountSequenceID { get; set; }
        string Type { get; set; }
        decimal? CuryDiscountableAmt { get; set; }
        decimal? DiscountableQty { get; set; }
        decimal? CuryDiscountAmt { get; set; }
        decimal? DiscountPct { get; set; }
        int? FreeItemID { get; set; }
        decimal? FreeItemQty { get; set; }
        bool? IsManual { get; set; }
    }
    #endregion

    #region ManualDiscount attribute
    /// <summary>
    /// Sets ManualDisc flag based on the values of the depending fields. Manual Flag is set when user
    /// overrides the discount values.
    /// This attribute also updates the relative fields. Ex: Updates Discount Amount when Discount Pct is modified.
    /// </summary>
    public class ManualDiscountMode : PXEventSubscriberAttribute, IPXRowUpdatedSubscriber, IPXRowInsertedSubscriber
    {
        private Type curyDiscAmtT;
        private Type curyTranAmtT;
        private Type freezeDiscT;
        private Type discPctT;

        #region BqlFields
        private abstract class discPct : IBqlField { }
        private abstract class curyDiscAmt : IBqlField { }

        private abstract class inventoryID : IBqlField { }
        private abstract class customerID : IBqlField { }
        private abstract class siteID : IBqlField { }
        private abstract class branchID : IBqlField { }
        private abstract class vendorID : IBqlField { }

        private abstract class orderQty : IBqlField { }
        private abstract class receiptQty : IBqlField { }
        private abstract class curyUnitPrice : IBqlField { }
        private abstract class curyUnitCost : IBqlField { }
        private abstract class curyExtPrice : IBqlField { }
        private abstract class curyExtCost : IBqlField { }
        private abstract class curyLineAmt : IBqlField { }
        private abstract class groupDiscountRate : IBqlField { }
        private abstract class documentDiscountRate : IBqlField { }
        private abstract class uOM : IBqlField { }

        private abstract class curyTranAmt : IBqlField { }
        private abstract class qty : IBqlField { }

        private abstract class skipDisc : IBqlField { }
        private abstract class discountID : IBqlField { }
        private abstract class discountSequenceID : IBqlField { }
        private abstract class manualDisc : IBqlField { }
        private abstract class manualPrice : IBqlField { }
        private abstract class lineType : IBqlField { }
        private abstract class isFree : IBqlField { }

        #endregion

        public ManualDiscountMode(Type curyDiscAmt, Type curyTranAmt, Type discPct, Type freezeManualDisc)
            : this(curyDiscAmt, curyTranAmt, discPct)
        {
            if (curyDiscAmt == null)
                throw new ArgumentNullException();

            if (curyTranAmt == null)
                throw new ArgumentNullException();

            this.freezeDiscT = freezeManualDisc;
            this.curyTranAmtT = curyTranAmt;
        }

        public ManualDiscountMode(Type curyDiscAmt, Type curyTranAmt, Type discPct)
            : this(curyDiscAmt, discPct)
        {
            if (curyDiscAmt == null)
                throw new ArgumentNullException();

            if (curyTranAmt == null)
                throw new ArgumentNullException();

            this.curyTranAmtT = curyTranAmt;
        }

        public ManualDiscountMode(Type curyDiscAmt, Type discPct)
        {
            if (curyDiscAmt == null)
                throw new ArgumentNullException("curyDiscAmt");
            if (discPct == null)
                throw new ArgumentNullException("discPct");

            this.curyDiscAmtT = curyDiscAmt;
            this.discPctT = discPct;
        }

        public static AmountLineFields GetDiscountDocumentLine(PXCache sender, object line)
        {
            if (line.GetType() == typeof(SOLine))
            {
                return new AmountLineFields<orderQty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (line.GetType() == typeof(ARTran))
            {
                return new AmountLineFields<qty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (line.GetType() == typeof(PX.Objects.AP.APTran))
            {
                return new AmountLineFields<qty, curyUnitCost, curyLineAmt, curyTranAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (line.GetType() == typeof(POLine))
            {
                return new AmountLineFields<orderQty, curyUnitCost, curyLineAmt, curyExtCost, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            if (line.GetType() == typeof(POReceiptLine))
            {
                return new AmountLineFields<receiptQty, curyUnitCost, curyLineAmt, curyExtCost, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
            }
            return new AmountLineFields<orderQty, curyUnitPrice, curyExtPrice, curyLineAmt, uOM, groupDiscountRate, documentDiscountRate>(sender, line);
        }

        //Returns line fields to be updated
        public static DiscountLineFields GetDiscountedLine(PXCache sender, object line)
        {
            return new DiscountLineFields<skipDisc, curyDiscAmt, discPct, discountID, discountSequenceID, manualDisc, manualPrice, lineType, isFree>(sender, line);
        }

		public static string GetLineDiscountTarget(PXCache sender, LineEntitiesFields efields)
        {
            string LineDiscountTarget = LineDiscountTargetType.ExtendedPrice;
            if (efields != null && efields.VendorID != null)
            {
                AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.SelectSingleBound(sender.Graph, null, efields.VendorID);
                if (vendor != null) LineDiscountTarget = vendor.LineDiscountTarget;
            }
            else
            {
                ARSetup arsetup = PXSelect<ARSetup>.Select(sender.Graph);
                if (arsetup != null) LineDiscountTarget = arsetup.LineDiscountTarget;
            }
            return LineDiscountTarget;
        }

        #region IPXRowUpdatedSubscriber Members

        public virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            AmountLineFields lineAmountsFields = GetDiscountDocumentLine(sender, e.Row);
            DiscountLineFields lineDiscountFields = GetDiscountedLine(sender, e.Row);

            AmountLineFields oldLineAmountsFields = GetDiscountDocumentLine(sender, e.OldRow);
            DiscountLineFields oldLineDiscountFields = GetDiscountedLine(sender, e.OldRow);

            LineEntitiesFields lineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, e.Row);
            LineEntitiesFields oldLineEntities = new LineEntitiesFields<inventoryID, customerID, siteID, branchID, vendorID>(sender, e.OldRow);

            bool manualMode = false;//by default AutoMode.
            bool useDiscPct = false;//by default value in DiscAmt has higher priority than DiscPct when both are modified.
            bool keepDiscountID = true;//should be set to true if user changes discount code code manually

            //Force Auto Mode 
            if (lineDiscountFields.ManualDisc == false && oldLineDiscountFields.ManualDisc == true)
            {
                manualMode = false;
            }

            //Change to Manual Mode based on fields changed:
            if (lineDiscountFields.ManualDisc == true || sender.Graph.IsCopyPasteContext)
                manualMode = true;

            //if (row.IsFree == true && oldRow.IsFree != true)
            //    manualMode = true;

            if (lineDiscountFields.DiscPct != oldLineDiscountFields.DiscPct && lineEntities.InventoryID == oldLineEntities.InventoryID)
            {
                manualMode = true;
                useDiscPct = true;
            }

			//use DiscPct when only Quantity/CuryUnitPrice/CuryExtPrice was changed
			if (lineDiscountFields.DiscPct == oldLineDiscountFields.DiscPct && lineDiscountFields.CuryDiscAmt == oldLineDiscountFields.CuryDiscAmt &&
				(lineAmountsFields.Quantity != oldLineAmountsFields.Quantity || lineAmountsFields.CuryUnitPrice != oldLineAmountsFields.CuryUnitPrice || lineAmountsFields.CuryExtPrice != oldLineAmountsFields.CuryExtPrice))
			{
				useDiscPct = true;
			}

			if (lineDiscountFields.CuryDiscAmt != oldLineDiscountFields.CuryDiscAmt)
            {
                manualMode = true;
                useDiscPct = false;
            }

            if (e.ExternalCall && (((Math.Abs((lineDiscountFields.CuryDiscAmt ?? 0m) - (oldLineDiscountFields.CuryDiscAmt ?? 0m)) > 0.0000005m)  || (Math.Abs((lineDiscountFields.DiscPct ?? 0m) - (oldLineDiscountFields.DiscPct ?? 0m)) > 0.0000005m)) && lineDiscountFields.DiscountID == oldLineDiscountFields.DiscountID))
            {
                keepDiscountID = false;
            }

            //if only CuryLineAmt (Ext.Price) was changed for a line with DiscoutAmt<>0
            //for Contracts Qty * UnitPrice * Prorate(<>1) = ExtPrice
            if (lineAmountsFields.CuryLineAmount != oldLineAmountsFields.CuryLineAmount && lineAmountsFields.Quantity == oldLineAmountsFields.Quantity && lineAmountsFields.CuryUnitPrice == oldLineAmountsFields.CuryUnitPrice && lineAmountsFields.CuryExtPrice == oldLineAmountsFields.CuryExtPrice && lineDiscountFields.DiscPct == oldLineDiscountFields.DiscPct && lineDiscountFields.CuryDiscAmt == oldLineDiscountFields.CuryDiscAmt && lineDiscountFields.CuryDiscAmt != 0)
            {
                manualMode = true;
            }

			decimal? validLineAmtRaw;
            decimal? validLineAmt = null;
            if (lineAmountsFields.CuryLineAmount != oldLineAmountsFields.CuryLineAmount)
            {
                if (useDiscPct)
                {
                    decimal val = lineAmountsFields.CuryExtPrice ?? 0;

                    decimal disctAmt;
                    if (GetLineDiscountTarget(sender, lineEntities) == LineDiscountTargetType.SalesPrice)
                    {
                        disctAmt = PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * (lineAmountsFields.CuryUnitPrice ?? 0m), CMPrecision.TRANCURY)
                            - PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * PXDBPriceCostAttribute.Round((lineAmountsFields.CuryUnitPrice ?? 0m) * (1 - (lineDiscountFields.DiscPct ?? 0m) * 0.01m)), CMPrecision.TRANCURY);
                    }
                    else
                    {
                        disctAmt = val * (lineDiscountFields.DiscPct ?? 0m) * 0.01m;
                        disctAmt = PXCurrencyAttribute.Round(sender, lineDiscountFields, disctAmt, CMPrecision.TRANCURY);
                    }

                    validLineAmtRaw = lineAmountsFields.CuryExtPrice - disctAmt;
                    validLineAmt = PXCurrencyAttribute.Round(sender, lineAmountsFields, validLineAmtRaw ?? 0, CMPrecision.TRANCURY);

                }
                else
                {
                    if (lineDiscountFields.CuryDiscAmt > lineAmountsFields.CuryExtPrice)
                    {
                        validLineAmtRaw = lineAmountsFields.CuryExtPrice;
                    }
                    else
                    {
                        validLineAmtRaw = lineAmountsFields.CuryExtPrice - lineDiscountFields.CuryDiscAmt;
                    }
                    validLineAmt = PXCurrencyAttribute.Round(sender, lineAmountsFields, validLineAmtRaw ?? 0, CMPrecision.TRANCURY);
                }

                if (lineAmountsFields.CuryLineAmount != validLineAmt && lineDiscountFields.DiscPct != oldLineDiscountFields.DiscPct)
                    manualMode = true;
            }

            sender.SetValue(e.Row, this.FieldName, manualMode);

            //Process only Manual Mode:
            if (manualMode || sender.Graph.IsCopyPasteContext)
            {
				if (manualMode && !keepDiscountID && !sender.Graph.IsImport)
				{
					lineDiscountFields.DiscountID = null;
					lineDiscountFields.DiscountSequenceID = null;
				}

				//Update related fields:
				if (lineAmountsFields.Quantity == 0 && oldLineAmountsFields.Quantity > 0)
                {
                    sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), 0m);
                    sender.SetValueExt(e.Row, sender.GetField(typeof(discPct)), 0m);
                }
                else if (lineAmountsFields.CuryLineAmount != oldLineAmountsFields.CuryLineAmount && !useDiscPct)
                {
                    decimal? extAmt = lineAmountsFields.CuryExtPrice ?? 0;
                    if (extAmt - lineAmountsFields.CuryLineAmount >= 0)
                    {
                        if (lineDiscountFields.CuryDiscAmt > Math.Abs(extAmt ?? 0m))
                        {
                            sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), lineAmountsFields.CuryExtPrice);
                            PXUIFieldAttribute.SetWarning<DiscountLineFields.curyDiscAmt>(sender, e.Row, AR.Messages.LineDiscountAmtMayNotBeGreaterExtPrice);
                        }
                        else
                            sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), extAmt - lineAmountsFields.CuryLineAmount);
						if (extAmt > 0 && !sender.Graph.IsCopyPasteContext)
                        {
                            decimal? pct = 100 * lineDiscountFields.CuryDiscAmt / extAmt;
                            sender.SetValueExt(e.Row, sender.GetField(typeof(discPct)), pct);
                        }
                    }
					else if (extAmt > 0 && !sender.Graph.IsCopyPasteContext)
					{
						decimal? pct = 100 * lineDiscountFields.CuryDiscAmt / extAmt;
						sender.SetValueExt(e.Row, sender.GetField(typeof(discPct)), (pct ?? 0m) < -100m ? -100m : pct);
						if ((pct ?? 0m) < -100m)
						{
							sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), -lineAmountsFields.CuryExtPrice);
							PXUIFieldAttribute.SetWarning<DiscountLineFields.curyDiscAmt>(sender, e.Row, AR.Messages.LineDiscountAmtMayNotBeGreaterExtPrice);
						}
					}
                }
                else if (lineDiscountFields.CuryDiscAmt != oldLineDiscountFields.CuryDiscAmt)
                {
                    if (lineAmountsFields.CuryExtPrice > 0 && !sender.Graph.IsCopyPasteContext)
                    {
                        decimal? pct = (lineDiscountFields.CuryDiscAmt ?? 0) * 100 / lineAmountsFields.CuryExtPrice;
                        sender.SetValueExt(e.Row, sender.GetField(typeof(discPct)), pct);
                    }
                }
                else if (lineDiscountFields.DiscPct != oldLineDiscountFields.DiscPct)
                {
                    decimal val = lineAmountsFields.CuryExtPrice ?? 0;

                    decimal amt;
                    if (GetLineDiscountTarget(sender, lineEntities) == LineDiscountTargetType.SalesPrice)
                    {
                        if (lineAmountsFields.CuryUnitPrice != 0 && lineAmountsFields.Quantity != 0)//if sales price is available
                        {
                            amt = PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * (lineAmountsFields.CuryUnitPrice ?? 0m), CMPrecision.TRANCURY)
                                - PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * PXDBPriceCostAttribute.Round((lineAmountsFields.CuryUnitPrice ?? 0m) * (1 - (lineDiscountFields.DiscPct ?? 0m) * 0.01m)), CMPrecision.TRANCURY);
                        }
                        else
                        {
                            amt = val * (lineDiscountFields.DiscPct ?? 0m) * 0.01m;
                        }
                    }
                    else
                    {
                        amt = val * (lineDiscountFields.DiscPct ?? 0m) * 0.01m;
                    }

                    sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), amt);
                }
                else if (validLineAmt != null && lineAmountsFields.CuryLineAmount != validLineAmt)
                {
                    decimal val = lineAmountsFields.CuryExtPrice ?? 0;

                    decimal amt;
                    if (GetLineDiscountTarget(sender, lineEntities) == LineDiscountTargetType.SalesPrice)
                    {
                        if (lineAmountsFields.CuryUnitPrice != 0 && lineAmountsFields.Quantity != 0)//if sales price is available
                        {
                            amt = PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * (lineAmountsFields.CuryUnitPrice ?? 0m), CMPrecision.TRANCURY)
                                - PXCurrencyAttribute.Round(sender, lineAmountsFields, (lineAmountsFields.Quantity ?? 0m) * PXDBPriceCostAttribute.Round((lineAmountsFields.CuryUnitPrice ?? 0m) * (1 - (lineDiscountFields.DiscPct ?? 0m) * 0.01m)), CMPrecision.TRANCURY);
                        }
                        else
                        {
                            amt = val * (lineDiscountFields.DiscPct ?? 0m) * 0.01m;
                        }
                    }
                    else
                    {
                        amt = val * (lineDiscountFields.DiscPct ?? 0m) * 0.01m;
                    }

                    sender.SetValueExt(e.Row, sender.GetField(typeof(curyDiscAmt)), amt);
                }

                //if (lineDiscountFields.IsFree == true)
                //{
                //    //certain fields must be disabled/enabled:
                //    sender.RaiseRowSelected(e.Row);
                //}
            }
        }

        #endregion

        #region IPXRowInsertedSubscriber Members

        public virtual void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            bool freeze = freezeDiscT != null && ((bool?)sender.GetValue(e.Row, sender.GetField(freezeDiscT))) == true;

            //When a new row is inserted there is 2 possible ways of handling it:
            //1. Sync the Discounts fields DiscAmt and DiscPct and calculate LineAmt as ExtPrice - DiscAmt. If DiscAmt <> 0 then ManualDisc flag is set.
            //2. Add line as is without changing any of the fields.
            //First Mode is typically executed when a user adds a line to Invoice from UI. Moreover the user enters Only Ext.Amt on the UI.
            //Second Mode is when a line from SOOrder is added to SOInvoice - in this case all discounts must be freezed and line must be added as is.

            if (!freeze && !sender.Graph.IsImport && !sender.Graph.IsCopyPasteContext)
            {
                AmountLineFields lineAmountsFields = GetDiscountDocumentLine(sender, e.Row);
                DiscountLineFields lineDiscountFields = GetDiscountedLine(sender, e.Row);
				
				if (lineDiscountFields.CuryDiscAmt != null && lineDiscountFields.CuryDiscAmt != 0 && lineAmountsFields.CuryExtPrice != 0)
                {
					decimal? revCuryDiscAmt = null;
					if (lineDiscountFields.DiscPct != null)
					{
						revCuryDiscAmt = PXDBCurrencyAttribute.RoundCury(sender, e.Row, (decimal)((lineAmountsFields.CuryExtPrice ?? 0m) / 100 * lineDiscountFields.DiscPct));
					}
					if (revCuryDiscAmt != null && revCuryDiscAmt != lineDiscountFields.CuryDiscAmt)
					{
						lineDiscountFields.DiscPct = 100 * lineDiscountFields.CuryDiscAmt / lineAmountsFields.CuryExtPrice;
					}
					sender.SetValue(e.Row, lineAmountsFields.GetField<AmountLineFields.curyLineAmount>().Name, lineAmountsFields.CuryExtPrice - lineDiscountFields.CuryDiscAmt);
				}
                else if (lineDiscountFields.DiscPct != null && lineDiscountFields.DiscPct != 0)
                {
					lineDiscountFields.CuryDiscAmt = (lineAmountsFields.CuryExtPrice ?? 0) * (lineDiscountFields.DiscPct ?? 0) * 0.01m;
					sender.SetValue(e.Row, lineAmountsFields.GetField<AmountLineFields.curyLineAmount>().Name, lineAmountsFields.CuryExtPrice - lineDiscountFields.CuryDiscAmt);
                }
                else if (lineAmountsFields.CuryExtPrice != null && lineAmountsFields.CuryExtPrice != 0m && (lineAmountsFields.CuryLineAmount == null || lineAmountsFields.CuryLineAmount == 0m))
                {
                    sender.SetValue(e.Row, lineAmountsFields.GetField<AmountLineFields.curyLineAmount>().Name, lineAmountsFields.CuryExtPrice);
                }
            }
        }

        #endregion
    }
    #endregion
}

