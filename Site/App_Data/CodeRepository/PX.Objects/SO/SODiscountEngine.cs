using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AR;
using System.Diagnostics;
using PX.Objects.CM;

namespace PX.Objects.SO
{
	public abstract class BqlInterface
	{ 
		public readonly PXCache cache;
		public readonly object row;

		public BqlInterface(PXCache cache, object row)
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
	}

	public abstract class DiscountedLine : BqlInterface 
	{
		public virtual decimal? CuryDiscAmt { get; set; }
		public virtual decimal? DiscPct { get; set; }

		public abstract class curyDiscAmt : IBqlField { }
		public abstract class discPct : IBqlField { }

		public DiscountedLine(PXCache cache, object row)
			: base(cache, row)
		{
		}
	}

	public class DiscountedLine<FieldDiscAmt, FieldDiscPct> : DiscountedLine
		where FieldDiscAmt : IBqlField
		where FieldDiscPct : IBqlField
	{
		public DiscountedLine(PXCache cache, object row)
			:base(cache, row)
		{
		}

		public override Type GetField<T>()
		{
			if (typeof(T) == typeof(curyDiscAmt))
			{
				return typeof(FieldDiscAmt);
			}
			if (typeof(T) == typeof(discPct))
			{
				return typeof(FieldDiscPct);
			}
			return null;
		}

		public override decimal? CuryDiscAmt
		{
			get
			{
				return (decimal?)cache.GetValue<FieldDiscAmt>(row);
			}
			set
			{
				cache.SetValue<FieldDiscAmt>(row, value);
			}
		}

		public override decimal? DiscPct
		{
			get
			{
				return (decimal?)cache.GetValue<FieldDiscPct>(row);
			}
			set
			{
				cache.SetValue<FieldDiscPct>(row, value);
			}
		}
	}


	/// <summary>
	/// Calculates Discounts
	/// Breakdown Qty is always in SalesUnits.
	/// Amounts are always in Base Currency.
	/// </summary>
	/// 
	public class SODiscountEngine
	{
		public struct SingleDiscountResult
		{
			#region Fields
			private decimal value;
			private decimal? discount;
			private int? freeItemID;
			private decimal? freeItemQty;
			#endregion

			#region Public Properties

			public decimal? Discount
			{
				get { return discount; }
			}

			public int? FreeItemID
			{
				get { return freeItemID; }
			}

			public decimal? FreeItemQty
			{
				get { return freeItemQty; }
			}

			public decimal Value
			{
				get { return value; }
			}


			public bool IsEmpty
			{
				get
				{
					return Discount == null && FreeItemQty == null;

				}
			}

			#endregion


			public SingleDiscountResult(decimal value, decimal? discount, int? freeItemID, decimal? freeItemQty)
			{
				this.value = value;
				this.discount = discount;
				this.freeItemID = freeItemID;
				this.freeItemQty = freeItemQty;
			}
		}

		public struct DiscountResult
		{
			#region Fields
			private decimal? discount;
			private int? firstFreeItemID;
			private decimal? firstFreeItemQty;
			private int? secondFreeItemID;
			private decimal? secondFreeItemQty;
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
			/// Gets Free Item ID returned by the first Discount Component
			/// </summary>
			public int? FirstFreeItemID
			{
				get { return firstFreeItemID; }
			}

			/// <summary>
			/// Gets Free Item Quantity
			/// </summary>
			public decimal? FirstFreeItemQty
			{
				get { return firstFreeItemQty; }
			}

			/// <summary>
			/// Gets Free Item ID returned by the second Discount Component
			/// </summary>
			public int? SecondFreeItemID
			{
				get { return secondFreeItemID; }
			}

			/// <summary>
			/// Gets Free Item Quantity
			/// </summary>
			public decimal? SecondFreeItemQty
			{
				get { return secondFreeItemQty; }
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
					return discount == null && FirstFreeItemQty == null && SecondFreeItemQty == null;

				}
			}

			#endregion


			internal DiscountResult(decimal? discount, int? firstFreeItemID, decimal? firstFreeItemQty, int? secondFreeItemID, decimal? secondFreeItemQty, bool isAmount)
			{
				this.discount = discount;
				this.firstFreeItemID = firstFreeItemID;
				this.firstFreeItemQty = firstFreeItemQty;
				this.secondFreeItemID = secondFreeItemID;
				this.secondFreeItemQty = secondFreeItemQty;
				this.isAmount = isAmount;
			}

		}



		public static void ClearDetDiscComponents(IDiscountable line)
		{
			ClearDetDiscComponents(line, false);
		}

		public static void ClearDetDiscComponents(IDiscountable line, bool KeepPromo)
		{
			line.DetDiscApp = false;
			line.DetDiscIDC1 = null;
			line.DetDiscIDC2 = null;
			line.DetDiscSeqIDC1 = null;
			line.DetDiscSeqIDC2 = null;

			if (!KeepPromo)
			{
				line.PromoDiscID = null;
			}
		}

		public static void ClearAllDiscComponents(IDiscountable line)
		{
			ClearDetDiscComponents(line);
			line.DocDiscIDC1 = null;
			line.DocDiscIDC2 = null;
			line.DocDiscSeqIDC1 = null;
			line.DocDiscSeqIDC2 = null;
		}

		public static DiscountResult GetDiscount(PXCache sender, DiscountSequence sequence, DateTime Date, int? InventoryID, string UOM, decimal? Qty, decimal? ExtPrice, bool Prorate)
		{
			bool isAmount = sequence.DiscountedFor == DiscountOption.Amount;
			decimal? discount;
			decimal? freeqty;
			int? freeitem;

			SODiscountEngine.GetDiscount(sender, sequence, Date, InventoryID, UOM, Qty, ExtPrice, false, out discount, out freeitem, out freeqty);

			return new SODiscountEngine.DiscountResult(discount, null, 0m, null, 0m, isAmount);
		}

		public static void GetDiscount(PXCache sender, DiscountSequence sequence, DateTime Date, int? InventoryID, string UOM, decimal? Qty, decimal? ExtPrice, bool Prorate, out decimal? Discount, out int? FreeItemID, out decimal? FreeItemQty)
		{
			FreeItemID = null;
			FreeItemQty = 0m;
			Discount = 0m;

			bool isAmount = sequence.DiscountedFor == DiscountOption.Amount;

			if (sequence.BreakBy == BreakdownType.Quantity)
			{
				decimal quantityInSales = Qty ?? 0m;

				if (InventoryID != null)
				{
					quantityInSales = ConvertToSalesUnits(sender, InventoryID, Qty.Value, UOM);
				}


				if (SearchQuantityBreakdown(sender, sequence, Date, quantityInSales, out Discount, out FreeItemQty, Prorate))
				{
					FreeItemID = GetFreeItem(sequence, Date);
				}

			}
			else
			{
				if (SearchAmountBreakdown(sender, sequence, Date, ExtPrice, out Discount, out FreeItemQty))
				{
					FreeItemID = GetFreeItem(sequence, Date);
				}
			}
		}

		protected static SingleDiscountResult CalculateDiscount(PXCache sender, DiscountSequence sequence, decimal breakValue, DateTime date, bool prorate)
		{
			decimal? discount = null;
			int? freeItemID = null;
			decimal? freeItemQty = null;

			if (sequence.BreakBy == BreakdownType.Quantity)
			{
				if (SearchQuantityBreakdown(sender, sequence, date, breakValue, out discount, out freeItemQty, prorate))
				{
					freeItemID = GetFreeItem(sequence, date);
				}
			}
			else
			{
				if (SearchAmountBreakdown(sender, sequence, date, breakValue, out discount, out freeItemQty))
				{
					freeItemID = GetFreeItem(sequence, date);
				}
			}

			return new SingleDiscountResult(breakValue, discount, freeItemID, freeItemQty);
		}

		public static void ApplyDiscountToLine(decimal? Qty, decimal? CuryUnitPrice, decimal? CuryExtPrice, DiscountedLine line, DiscountResult discountResult, int multInv)
		{
			PXCache cache = line.cache;
			object row = line.row;

			decimal qty = Math.Abs(Qty.Value);

			if (!discountResult.IsEmpty)
			{
                ARSetup arsetup = PXSelect<ARSetup>.Select(cache.Graph);
				CommonSetup commonsetup = PXSelect<CommonSetup>.Select(cache.Graph);
				int precision = 4;
				if (commonsetup != null && commonsetup.DecPlPrcCst != null)
					precision = commonsetup.DecPlPrcCst.Value;

				if (discountResult.IsAmount)
				{
                    if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
					{
						decimal discAmt = (discountResult.Discount ?? 0) * qty;
						decimal curyDiscAmt;
						PXCurrencyAttribute.CuryConvCury(cache, row, discAmt, out curyDiscAmt, precision);
						decimal? oldCuryDiscAmt = line.CuryDiscAmt;
						line.CuryDiscAmt = multInv * curyDiscAmt;
						if (line.CuryDiscAmt != oldCuryDiscAmt)
							line.RaiseFieldUpdated<DiscountedLine.curyDiscAmt>(oldCuryDiscAmt);
					}
					else
					{
						decimal discAmt = (discountResult.Discount ?? 0);
						decimal curyDiscAmt;
						PXCurrencyAttribute.CuryConvCury(cache, row, discAmt, out curyDiscAmt, precision);
						decimal? oldCuryDiscAmt = line.CuryDiscAmt;
						line.CuryDiscAmt = multInv * curyDiscAmt;
						if (line.CuryDiscAmt != oldCuryDiscAmt)
							line.RaiseFieldUpdated<DiscountedLine.curyDiscAmt>(oldCuryDiscAmt);
					}

					if (line.CuryDiscAmt != 0 && CuryExtPrice.Value != 0)
					{
						decimal? oldValue = line.DiscPct;
						decimal discPct = line.CuryDiscAmt.Value * 100 / CuryExtPrice.Value;
						line.DiscPct = Math.Round(discPct, 6, MidpointRounding.AwayFromZero);
						if (line.DiscPct != oldValue)
							line.RaiseFieldUpdated<DiscountedLine.discPct>(oldValue);
					}
				}
				else
				{
					decimal? oldValue = line.DiscPct;
					line.DiscPct = Math.Round(discountResult.Discount ?? 0, 6, MidpointRounding.AwayFromZero);
					if (line.DiscPct != oldValue)
						line.RaiseFieldUpdated<DiscountedLine.discPct>(oldValue);
					decimal? oldCuryDiscAmt = line.CuryDiscAmt;
					decimal curyDiscAmt;

                    if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
					{
						decimal salesPriceAfterDiscount = (CuryUnitPrice ?? 0) * (1 - 0.01m * (line.DiscPct ?? 0));
						decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
						curyDiscAmt = qty * (CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(cache, row, extPriceAfterDiscount, CMPrecision.TRANCURY);
						if (curyDiscAmt < 0)//this can happen only due to difference in rounding between unitprice and exprice. ex when Unit price has 4 digits (with value in 3rd place 20.0050) and ext price only 2 
							curyDiscAmt = 0;
					}
					else
					{
						curyDiscAmt = (CuryExtPrice ?? 0) * 0.01m * (line.DiscPct ?? 0);
					}

					// bug 23496 <- line.CuryDiscAmt = multInv * Math.Round(curyDiscAmt, precision, MidpointRounding.AwayFromZero);
					line.CuryDiscAmt = multInv * PXCurrencyAttribute.Round(cache, row, curyDiscAmt, CMPrecision.TRANCURY);
					if (line.CuryDiscAmt != oldCuryDiscAmt)
					{
						line.RaiseFieldUpdated<DiscountedLine.curyDiscAmt>(oldCuryDiscAmt);
					}
				}
			}
			else
			{
				decimal? oldDiscPct = line.DiscPct;
				decimal? oldCuryDiscAmt = line.CuryDiscAmt;
				line.DiscPct = 0;
				if (oldDiscPct != 0)
					line.RaiseFieldUpdated<DiscountedLine.discPct>(oldDiscPct);
				line.CuryDiscAmt = 0;
				if (oldCuryDiscAmt != 0)
					line.RaiseFieldUpdated<DiscountedLine.curyDiscAmt>(oldCuryDiscAmt);
			}
		}

		protected static decimal ConvertToSalesUnits(PXCache sender, int? inventoryID, decimal quantity, string UOM)
		{
			InventoryItem item = GetInventoryItemByID(sender, inventoryID);

			if (item.SalesUnit == UOM)
				return quantity;
			else
			{
				decimal inBase = INUnitAttribute.ConvertToBase(sender, inventoryID, UOM, quantity, INPrecision.QUANTITY);
				return INUnitAttribute.ConvertFromBase(sender, inventoryID, item.SalesUnit, inBase, INPrecision.QUANTITY);
			}
		}

		protected static bool SearchAmountBreakdown(PXCache sender, DiscountSequence sequence, DateTime date, decimal? amount, out decimal? discount, out decimal? freeItemQty)
		{
			decimal breakAmount = Math.Abs(amount ?? 0);

			PXResultset<DiscountDetail> details = PXSelect<DiscountDetail,
						 Where<DiscountDetail.discountSequenceID, Equal<Required<DiscountDetail.discountSequenceID>>,
						 And<DiscountDetail.discountID, Equal<Required<DiscountDetail.discountID>>>>>.Select(sender.Graph, sequence.DiscountSequenceID, sequence.DiscountID);


			decimal minDelta = decimal.MaxValue;
			discount = null;
			freeItemQty = null;
			bool resultFound = false;

			if (amount != null)
			{
				foreach (DiscountDetail detail in details)
				{
					if (sequence.IsPromotion == true)
					{
						//For promo the sequence when applied already was checked for the valid date.
						if (detail.Amount != null)
						{
							if (detail.Amount.Value <= breakAmount)
							{
								decimal delta = breakAmount - detail.Amount.Value;

								if (delta < minDelta)
								{
									discount = detail.Discount;
									freeItemQty = detail.FreeItemQty;
									minDelta = delta;
									resultFound = true;
								}
							}
						}
					}
					else
					{
						if (detail.LastDate != null)
						{
							if (detail.LastDate.Value <= date)
							{
								//current value
								if (detail.Amount != null)
								{
									if (detail.Amount.Value <= breakAmount)
									{
										decimal delta = breakAmount - detail.Amount.Value;

										if (delta < minDelta)
										{
											discount = detail.Discount;
											freeItemQty = detail.FreeItemQty;
											minDelta = delta;
											resultFound = true;
										}
									}
								}
							}
							else
							{
								//last value
								if (detail.LastAmount != null)
								{
									if (detail.LastAmount.Value <= breakAmount)
									{
										decimal delta = breakAmount - detail.LastAmount.Value;
										if (delta < minDelta)
										{
											discount = detail.LastDiscount;
											freeItemQty = detail.LastFreeItemQty;
											minDelta = delta;
											resultFound = true;
										}
									}
								}
							}
						}
					}
				}
			}


			return resultFound;
		}

		protected static bool SearchQuantityBreakdown(PXCache sender, DiscountSequence sequence, DateTime date, decimal quantityInSales, out decimal? discount, out decimal? freeItemQty, bool prorate)
		{
			decimal? breakQtyUsed;
			decimal? minBreakQty;
			bool resultFound = SearchQuantityBreakdown(sender, sequence, date, quantityInSales, out discount, out freeItemQty, out breakQtyUsed, out minBreakQty);

			if (resultFound && prorate && freeItemQty > 0 && breakQtyUsed > 0)
			{
				decimal excessiveQty = quantityInSales - breakQtyUsed.Value;

				decimal? dummy1;
				decimal? dummy2;
				while (excessiveQty >= minBreakQty && excessiveQty > 0)
				{
					decimal? freeItemQtyLocal;
					if (SearchQuantityBreakdown(sender, sequence, date, excessiveQty, out dummy1, out freeItemQtyLocal, out breakQtyUsed, out dummy2))
					{
						freeItemQty += freeItemQtyLocal ?? 0;
						excessiveQty = excessiveQty - breakQtyUsed.Value;

						if (breakQtyUsed.Value == 0)
							break;
					}
					else
						break;
				}
			}



			return resultFound;
		}

		protected static bool SearchQuantityBreakdown(PXCache sender, DiscountSequence sequence, DateTime date, decimal quantityInSales, out decimal? discount, out decimal? freeItemQty, out decimal? breakQtyUsed, out decimal? minBreakQty)
		{
			PXResultset<DiscountDetail> details = PXSelect<DiscountDetail,
					 Where<DiscountDetail.discountSequenceID, Equal<Required<DiscountDetail.discountSequenceID>>,
					 And<DiscountDetail.discountID, Equal<Required<DiscountDetail.discountID>>>>>.Select(sender.Graph, sequence.DiscountSequenceID, sequence.DiscountID);


			decimal minDelta = decimal.MaxValue;
			discount = null;
			freeItemQty = null;
			bool resultFound = false;
			decimal minBreakQtyValue = decimal.MaxValue;
			breakQtyUsed = null;

			foreach (DiscountDetail detail in details)
			{
				if (sequence.IsPromotion == true)
				{
					//For promo the sequence when applied already was checked for the valid date.
					if (detail.Quantity != null)
					{
						if (detail.Quantity.Value <= quantityInSales)
						{
							decimal delta = quantityInSales - detail.Quantity.Value;

							if (delta < minDelta)
							{
								discount = detail.Discount;
								freeItemQty = detail.FreeItemQty;
								minDelta = delta;
								breakQtyUsed = detail.Quantity.Value;
								resultFound = true;
							}
						}

						if (detail.Quantity.Value < minBreakQtyValue)
						{
							minBreakQtyValue = detail.Quantity.Value;
						}
					}
				}
				else
				{
					if (detail.LastDate != null)
					{
						if (detail.LastDate.Value <= date)
						{
							//current value
							if (detail.Quantity != null)
							{
								if (detail.Quantity.Value <= quantityInSales)
								{
									decimal delta = quantityInSales - detail.Quantity.Value;

									if (delta < minDelta)
									{
										discount = detail.Discount;
										freeItemQty = detail.FreeItemQty;
										minDelta = delta;
										breakQtyUsed = detail.Quantity.Value;
										resultFound = true;
									}
								}

								if (detail.Quantity.Value < minBreakQtyValue)
								{
									minBreakQtyValue = detail.Quantity.Value;
								}
							}
						}
						else
						{
							//last value
							if (detail.LastQuantity != null)
							{
								if (detail.LastQuantity.Value <= quantityInSales)
								{
									decimal delta = quantityInSales - detail.LastQuantity.Value;
									if (delta < minDelta)
									{
										discount = detail.LastDiscount;
										freeItemQty = detail.LastFreeItemQty;
										minDelta = delta;
										breakQtyUsed = detail.LastQuantity.Value;
										resultFound = true;
									}
								}

								if (detail.LastQuantity.Value < minBreakQtyValue)
								{
									minBreakQtyValue = detail.LastQuantity.Value;
								}
							}
						}

					}
				}
			}

			minBreakQty = minBreakQtyValue;

			return resultFound;
		}

		protected static int? GetFreeItem(DiscountSequence sequence, DateTime date)
		{
			int? freeItemId = null;

			if (sequence.IsPromotion == false)
			{
				if (sequence.UpdateDate == null)
				{
					freeItemId = sequence.FreeItemID;
				}
				else if (sequence.UpdateDate.Value <= date)
				{
					freeItemId = sequence.FreeItemID;
				}
				else
				{
					freeItemId = sequence.LastFreeItemID;
				}

			}
			else
			{
				freeItemId = sequence.FreeItemID;
			}

			return freeItemId;
		}


		protected static InventoryItem GetInventoryItemByID(PXCache sender, int? inventoryID)
		{
			return PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(sender.Graph, inventoryID);
		}

		protected static ARDiscount GetDiscountByID(PXCache sender, string discountID)
		{
			return PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(sender.Graph, discountID);
		}

		public static DiscountSequence GetDiscountSequenceByID(PXCache sender, string discountID, string discountSequenceID)
		{
			return PXSelect<DiscountSequence,
					Where<DiscountSequence.discountID, Equal<Required<DiscountSequence.discountID>>,
					And<DiscountSequence.discountSequenceID, Equal<Required<DiscountSequence.discountSequenceID>>>>>.Select(sender.Graph, discountID, discountSequenceID);

		}

        public struct DiscountSetup
        {
            public string FirstDiscountID;
            public string SecondDiscountID;
            public bool IsCompoundDiscount;
        }

        public static bool SearchDiscounts(PXCache sender, DiscountSetup group, int? inventoryID, int? customerID, int? locationID, DateTime date, out DiscountSequence firstSequence, out DiscountSequence secondSequence, out bool isCompoundDiscount)
        {
            firstSequence = null;
            secondSequence = null;
            isCompoundDiscount = false;

            if (!string.IsNullOrEmpty(group.FirstDiscountID))
            {
                firstSequence = FindDiscountSequence(sender, group.FirstDiscountID, inventoryID, customerID, locationID, date);

                if (firstSequence != null && !string.IsNullOrEmpty(group.SecondDiscountID))
                {
                    DiscountSequence result2 = FindDiscountSequence(sender, group.SecondDiscountID, inventoryID, customerID, locationID, date);
                    if (result2 != null)
                    {
                        secondSequence = result2;
                        isCompoundDiscount = group.IsCompoundDiscount == true;
                    }
                    else
                    {
                        firstSequence = null;
                    }

                }
            }
            return firstSequence != null;
        }
        private static DiscountSequence FindDiscountSequence(PXCache sender, string discountID, int? inventoryID, int? customerID, int? locationID, DateTime date)
		{
			ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(sender.Graph, discountID);

			DiscountSequence sequence = null;

			if (discount != null)
			{
				switch (discount.ApplicableTo)
				{
					case DiscountTarget.Unconditional:

						sequence = PXSelect<DiscountSequence,
							Where<DiscountSequence.discountID, Equal<Required<DiscountSequence.discountID>>,
							And<DiscountSequence.isActive, Equal<boolTrue>,
							And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
									Or<DiscountSequence.isPromotion, Equal<boolTrue>,
										And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
								>
							>>>.Select(sender.Graph, discountID, date);
						break;

					case DiscountTarget.Customer:

						sequence = PXSelectJoin<DiscountSequence,
							InnerJoin<DiscountCustomer, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomer.discountSequenceID>,
								And<DiscountSequence.discountID, Equal<DiscountCustomer.discountID>>>>,
							Where<DiscountCustomer.discountID, Equal<Required<DiscountCustomer.discountID>>,
							And<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
							And<DiscountSequence.isActive, Equal<boolTrue>,
							And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
									Or<DiscountSequence.isPromotion, Equal<boolTrue>,
										And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
								>
							>>>>.Select(sender.Graph, discountID, customerID, date);
						break;
					case DiscountTarget.Inventory:
						sequence = PXSelectJoin<DiscountSequence,
							InnerJoin<DiscountItem, On<DiscountSequence.discountSequenceID, Equal<DiscountItem.discountSequenceID>,
								And<DiscountSequence.discountID, Equal<DiscountItem.discountID>>>>,
							Where<DiscountItem.discountID, Equal<Required<DiscountItem.discountID>>,
							And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
							And<DiscountSequence.isActive, Equal<boolTrue>,
							And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
									Or<DiscountSequence.isPromotion, Equal<boolTrue>,
										And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
								>
							>>>>.Select(sender.Graph, discountID, inventoryID, date);
						break;
					case DiscountTarget.CustomerAndInventory:
						sequence = PXSelectJoin<DiscountSequence,
							InnerJoin<DiscountItem, On<DiscountSequence.discountSequenceID, Equal<DiscountItem.discountSequenceID>,
								And<DiscountSequence.discountID, Equal<DiscountItem.discountID>>>,
							InnerJoin<DiscountCustomer, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomer.discountSequenceID>,
								And<DiscountSequence.discountID, Equal<DiscountCustomer.discountID>>>>>,
							Where<DiscountItem.discountID, Equal<Required<DiscountItem.discountID>>,
							And<DiscountCustomer.discountID, Equal<Required<DiscountCustomer.discountID>>,
							And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
							And<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
							And<DiscountSequence.isActive, Equal<boolTrue>,
							And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
									Or<DiscountSequence.isPromotion, Equal<boolTrue>,
										And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
								>
							>>>>>>.Select(sender.Graph, discountID, discountID, inventoryID, customerID, date);
						break;
					case DiscountTarget.CustomerAndInventoryPrice:
						InventoryItem item2 = GetInventoryItemByID(sender, inventoryID);
						Debug.Assert(item2 != null, string.Format("Failed to find InventoryItem for the CustomerAndInventoryPrice discount. InventoryID={0}", inventoryID));
						if (item2 != null)
						{
							sequence = PXSelectJoin<DiscountSequence,
								 InnerJoin<DiscountInventoryPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>,
									 And<DiscountSequence.discountID, Equal<DiscountInventoryPriceClass.discountID>>>,
								 InnerJoin<DiscountCustomer, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomer.discountSequenceID>,
									 And<DiscountSequence.discountID, Equal<DiscountCustomer.discountID>>>>>,
								 Where<DiscountInventoryPriceClass.discountID, Equal<Required<DiscountItem.discountID>>,
								 And<DiscountCustomer.discountID, Equal<Required<DiscountCustomer.discountID>>,
								 And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
								 And<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
								 And<DiscountSequence.isActive, Equal<boolTrue>,
								 And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
									 Or<DiscountSequence.isPromotion, Equal<boolTrue>,
											 And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
									 >
								 >>>>>>.Select(sender.Graph, discountID, discountID, item2.PriceClassID, customerID, date);
						}
						break;

					case DiscountTarget.CustomerPrice:
						Location location = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(sender.Graph, customerID, locationID);
						Debug.Assert(location != null, string.Format("Failed to find customer location for the CustomerPrice discount. LocationID={0}", locationID));
						if (location != null)
						{
							sequence = PXSelectJoin<DiscountSequence,
								InnerJoin<DiscountCustomerPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomerPriceClass.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountCustomerPriceClass.discountID>>>>,
								Where<DiscountCustomerPriceClass.discountID, Equal<Required<DiscountCustomerPriceClass.discountID>>,
								And<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
								And<DiscountSequence.isActive, Equal<boolTrue>,
								And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
										Or<DiscountSequence.isPromotion, Equal<boolTrue>,
											And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
									>
								>>>>.Select(sender.Graph, discountID, location.CPriceClassID, date);
						}
						break;
					case DiscountTarget.InventoryPrice:
						InventoryItem item = GetInventoryItemByID(sender, inventoryID);
						Debug.Assert(item != null, string.Format("Failed to find InventoryItem for the InventoryPrice discount. InventoryID={0}", inventoryID));
						if (item != null)
						{
							sequence = PXSelectJoin<DiscountSequence,
								InnerJoin<DiscountInventoryPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountInventoryPriceClass.discountID>>>>,
								Where<DiscountInventoryPriceClass.discountID, Equal<Required<DiscountInventoryPriceClass.discountID>>,
								And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
								And<DiscountSequence.isActive, Equal<boolTrue>,
								And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
										Or<DiscountSequence.isPromotion, Equal<boolTrue>,
											And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
									>
								>>>>.Select(sender.Graph, discountID, item.PriceClassID, date);
						}
						break;
					case DiscountTarget.CustomerPriceAndInventory:
						Location location2 = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(sender.Graph, customerID, locationID);
						Debug.Assert(location2 != null, string.Format("Failed to find customer location for the CustomerPriceAndInventory discount. LocationID={0}", locationID));
						if (location2 != null)
						{
							sequence = PXSelectJoin<DiscountSequence,
								InnerJoin<DiscountCustomerPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomerPriceClass.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountCustomerPriceClass.discountID>>>,
								InnerJoin<DiscountItem, On<DiscountSequence.discountSequenceID, Equal<DiscountItem.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountItem.discountID>>>>>,
								Where<DiscountCustomerPriceClass.discountID, Equal<Required<DiscountCustomerPriceClass.discountID>>,
								And<DiscountItem.discountID, Equal<Required<DiscountItem.discountID>>,
								And<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
								And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
								And<DiscountSequence.isActive, Equal<boolTrue>,
								And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
										Or<DiscountSequence.isPromotion, Equal<boolTrue>,
											And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
									>
								>>>>>>.Select(sender.Graph, discountID, discountID, location2.CPriceClassID, inventoryID, date);
						}
						break;
					case DiscountTarget.CustomerPriceAndInventoryPrice:
						InventoryItem item3 = GetInventoryItemByID(sender, inventoryID);
						Debug.Assert(item3 != null, string.Format("Failed to find InventoryItem for the CustomerPriceAndInventoryPrice discount. InventoryID={0}", inventoryID));
						Location location3 = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(sender.Graph, customerID, locationID);
						Debug.Assert(location3 != null, string.Format("Failed to find customer location for the CustomerPriceAndInventoryPrice discount. LocationID={0}", locationID));
						if (item3 != null && location3 != null)
						{
							sequence = PXSelectJoin<DiscountSequence,
								InnerJoin<DiscountCustomerPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountCustomerPriceClass.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountCustomerPriceClass.discountID>>>,
								InnerJoin<DiscountInventoryPriceClass, On<DiscountSequence.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>,
									And<DiscountSequence.discountID, Equal<DiscountInventoryPriceClass.discountID>>>>>,

								Where<DiscountCustomerPriceClass.discountID, Equal<Required<DiscountCustomerPriceClass.discountID>>,
								And<DiscountInventoryPriceClass.discountID, Equal<Required<DiscountInventoryPriceClass.discountID>>,
								And<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
								And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
								And<DiscountSequence.isActive, Equal<boolTrue>,
								And<Where<DiscountSequence.isPromotion, Equal<boolFalse>,
										Or<DiscountSequence.isPromotion, Equal<boolTrue>,
											And<Required<DiscountSequence.startDate>, Between<DiscountSequence.startDate, DiscountSequence.endDate>>>>
									>
								>>>>>>.Select(sender.Graph, discountID, discountID, location3.CPriceClassID, item3.PriceClassID, date);
						}
						break;
				}
			}

			return sequence;
		}
	}

	public class SODiscountEngine<Line> : SODiscountEngine
		where Line : class, IBqlTable, IDiscountable, new()
	{
		#region Field Name Constants

		private const string FirstDetailDiscountID = "DetDiscIDC1";
		private const string FirstDetailDiscountSequenceID = "DetDiscSeqIDC1";
		private const string SecondDetailDiscountID = "DetDiscIDC2";
		private const string SecondDetailDiscountSequenceID = "DetDiscSeqIDC2";
		private const string FirstDocumentDiscountID = "DocDiscIDC1";
		private const string FirstDocumentDiscountSequenceID = "DocDiscSeqIDC1";
		private const string SecondDocumentDiscountID = "DocDiscIDC2";
		private const string SecondDocumentDiscountSequenceID = "DocDiscSeqIDC2";

		private const string DiscountID = "DiscountID";
		private const string DiscountSequenceID = "DiscountSequenceID";
		private const string Type = "Type";
		private const string DiscPct = "DiscPct";
		private const string CuryDiscAmt = "CuryDiscAmt";
		private const string CuryUnitPrice = "CuryUnitPrice";
		#endregion

		/// <summary>
		/// Sets applicable Discount Components. 
		/// </summary>
		/// <param name="line">Current Line</param>
		/// <param name="customerLocationID">Customer's Location</param>
		/// <exception cref="ArgumentNullException">This exception is thrown if either sender or line parameter is null.</exception>
		/// <exception cref="ArgumentException">This exception is thrown if Date property is not initialized.</exception>
		public static void SetDiscounts(PXCache sender, Line line, int? customerLocationID, DateTime date)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");
			if (line == null)
				throw new ArgumentNullException("line");

		}

		/// <summary>
		/// Removes Discount Details that are not applicable. Typically you should call this method after 
		/// new Discounts are set for the line. Or when they are cleared as a result of Manual Disc mode.
		/// </summary>
		/// <typeparam name="DiscountDetail">DiscountDetails Type</typeparam>
		/// <param name="transactions">Lines</param>
		/// <param name="discountDetails">Discount details</param>
		public static void RemoveUnappliableDiscountDetails<DiscountDetail>(PXSelectBase<Line> transactions, PXSelectBase<DiscountDetail> discountDetails)
			where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			List<string> detDiscounts = new List<string>();
			List<string> docDiscounts = new List<string>();
			foreach (Line line in transactions.Select())
			{
				if (!string.IsNullOrEmpty(line.DetDiscIDC1) && !string.IsNullOrEmpty(line.DetDiscSeqIDC1))
				{
					string key = string.Format("{0}.{1}", line.DetDiscIDC1, line.DetDiscSeqIDC1);
					if (!detDiscounts.Contains(key))
					{
						detDiscounts.Add(key);
					}
				}

				if (!string.IsNullOrEmpty(line.DetDiscIDC2) && !string.IsNullOrEmpty(line.DetDiscSeqIDC2))
				{
					string key = string.Format("{0}.{1}", line.DetDiscIDC2, line.DetDiscSeqIDC2);
					if (!detDiscounts.Contains(key))
					{
						detDiscounts.Add(key);
					}
				}

				if (!string.IsNullOrEmpty(line.DocDiscIDC1) && !string.IsNullOrEmpty(line.DocDiscSeqIDC1))
				{
					string key = string.Format("{0}.{1}", line.DocDiscIDC1, line.DocDiscSeqIDC1);
					if (!docDiscounts.Contains(key))
					{
						docDiscounts.Add(key);
					}
				}

				if (!string.IsNullOrEmpty(line.DocDiscIDC2) && !string.IsNullOrEmpty(line.DocDiscSeqIDC2))
				{
					string key = string.Format("{0}.{1}", line.DocDiscIDC2, line.DocDiscSeqIDC2);
					if (!docDiscounts.Contains(key))
					{
						docDiscounts.Add(key);
					}
				}
			}

			foreach (DiscountDetail detail in discountDetails.Select())
			{
				string key = string.Format("{0}.{1}", detail.DiscountID, detail.DiscountSequenceID);

				if (detail.Type == DiscountType.Line)
				{
					if (!detDiscounts.Contains(key))
					{
						discountDetails.Delete(detail);
					}
				}
				else if (detail.Type == DiscountType.Document)
				{
					if (!docDiscounts.Contains(key))
					{
						discountDetails.Delete(detail);
					}
				}
			}
		}

		/// <summary>
		/// Recalculates the discounts for the given line. Updates or adds a record to DiscountDetails.
		/// </summary>
		/// <exception cref="ArgumentNullException">This exception is thrown if either graph, sender or line parameter is null.</exception>
		/// <exception cref="ArgumentException">This exception is thrown if Qty or CuryLineAmt properties of the given Line object is not properly initialized.</exception>
		public static void UpdateDiscountDetails<DiscountDetail>(PXCache sender, PXSelectBase<Line> transactions, PXSelectBase<DiscountDetail> discountDetails, Line line, int? customerLocationID, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			if (transactions == null)
				throw new ArgumentNullException("transactions");

			if (line == null)
				throw new ArgumentNullException("line");

			if (line.InventoryID == null)
				throw new ArgumentException(Messages.DiscountEngine_InventoryIDIsNull);

			if (line.Qty == null)
				throw new ArgumentException(Messages.DiscountEngine_QtyIsNull);

			UpdateLineLevelDiscountDetail<DiscountDetail>(sender, transactions, discountDetails, line, date, prorate);
			UpdateDocumentLevelDiscountDetail<DiscountDetail>(sender, transactions, discountDetails, line, date, prorate);
		}

		/// <summary>
		/// Calculate Discount and set CuryDiscAmt and DiscPct for a given line. 
		/// </summary>
		/// <param name="sender">Cache</param>
		/// <param name="line">Current Line</param>
		/// <param name="customerLocationID">Customer's LocationID</param>
		///        
		public static void CalculateLineDiscounts(PXCache sender, Line line, int? customerLocationID, DateTime date)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			if (line == null)
				throw new ArgumentNullException("line");

			if (line.Qty == null)
				throw new ArgumentException(Messages.DiscountEngine_QtyIsNull);
			if (line.CuryExtPrice == null)
				throw new ArgumentException(Messages.DiscountEngine_CuryExtPriceIsNull);

			int multInv = 1;

			if (line.Qty < 0 || line.CuryExtPrice < 0)
				multInv = -1;

			if (!string.IsNullOrEmpty(line.DetDiscIDC1))
			{
				ARDiscount ds = GetDiscountByID(sender, line.DetDiscIDC1);

				if (ds.Type == DiscountType.Flat)
				{
					ApplyFlatPriceDiscounts(sender, line, customerLocationID, date, multInv);
				}
				else
				{
					decimal baseExtPrice;
					PXCurrencyAttribute.CuryConvBase(sender, line, line.CuryExtPrice ?? 0, out baseExtPrice, false);

					decimal qty = Math.Abs(line.Qty.Value);
                    ARSetup arsetup = ARSetupSelect.Select(sender.Graph);
                    DiscountResult discountResult = CalculateDetailDiscount(sender, line, customerLocationID, date, baseExtPrice, qty, false, arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice);
					ApplyDiscountToLine(sender, line, discountResult, multInv);
				}
			}
			else
			{
				decimal? oldDiscPct = line.DiscPct;
				decimal? oldCuryDiscAmt = line.CuryDiscAmt;
				line.DiscPct = 0;
				if (oldDiscPct != 0)
					sender.RaiseFieldUpdated(DiscPct, line, oldDiscPct);
				line.CuryDiscAmt = 0;
				if (oldCuryDiscAmt != 0)
					sender.RaiseFieldUpdated(CuryDiscAmt, line, oldCuryDiscAmt);
			}
		}

		/// <summary>
		/// Return True if Flat-Price Discounts are applicable for this line.
		/// </summary>
		public static bool IsFlatPriceApplicable(PXCache sender, Line line)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			if (line == null)
				throw new ArgumentNullException("line");

			if (!string.IsNullOrEmpty(line.DetDiscIDC1))
			{
				ARDiscount ds = GetDiscountByID(sender, line.DetDiscIDC1);

				if (ds.Type == DiscountType.Flat)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Calculates Unit Price based on Flat-Price Discounts. Result Unit Price will be in the UOM of the given line.
		/// </summary>
		public static decimal? CalculateFlatPrice(PXCache sender, Line line, int? customerLocationID, DateTime date)
		{
			decimal? result = null;

			if (!string.IsNullOrEmpty(line.DetDiscIDC1))
			{
				ARDiscount ds = GetDiscountByID(sender, line.DetDiscIDC1);

				if (ds.Type == DiscountType.Flat)
				{
					decimal baseExtPrice;
					PXDBCurrencyAttribute.CuryConvBase(sender, line, line.CuryExtPrice ?? 0, out baseExtPrice);
					decimal qty = Math.Abs(line.Qty.Value);

					DiscountSequence firstSeq = GetDiscountSequenceByID(sender, line.DetDiscIDC1, line.DetDiscSeqIDC1);
					if (firstSeq != null)
					{
						SingleDiscountResult dr = CalculateDiscount(sender, firstSeq, line, date, false);
						if (!dr.IsEmpty)
						{
							InventoryItem item = GetInventoryItemByID(sender, line.InventoryID);
							if (item != null)
							{
								decimal? flatPriceInBase = INUnitAttribute.ConvertFromBase(sender, line.InventoryID, item.SalesUnit, dr.Discount ?? 0, INPrecision.UNITCOST);
								result = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, flatPriceInBase ?? 0, INPrecision.UNITCOST);
							}
						}
					}
				}
			}

			return result;
		}

		private abstract class discPct : IBqlField { }
		private abstract class curyDiscAmt : IBqlField { }

		private static void ApplyDiscountToLine(PXCache sender, Line line, DiscountResult discountResult, int multInv)
		{
			ApplyDiscountToLine(line.Qty, line.CuryUnitPrice, line.CuryExtPrice, new DiscountedLine<curyDiscAmt, discPct>(sender, line), discountResult, multInv);
		}

		private static void ApplyFlatPriceDiscounts(PXCache sender, Line line, int? customerLocationID, DateTime date, int multInv)
		{
			decimal baseExtPrice;
			PXDBCurrencyAttribute.CuryConvBase(sender, line, line.CuryExtPrice ?? 0, out baseExtPrice);

			decimal qty = Math.Abs(line.Qty.Value);

			DiscountSequence firstSeq = GetDiscountSequenceByID(sender, line.DetDiscIDC1, line.DetDiscSeqIDC1);
			if (firstSeq != null)
			{
				DiscountSequence secondSeq = GetDiscountSequenceByID(sender, line.DetDiscIDC2, line.DetDiscSeqIDC2);
				if (secondSeq != null)
				{
					SingleDiscountResult discountResult = CalculateDiscount(sender, firstSeq, line, date, false);

					DiscountResult res = new DiscountResult(discountResult.Discount, discountResult.FreeItemID, discountResult.FreeItemQty, null, null, secondSeq.DiscountedFor == DiscountOption.Amount);
					ApplyDiscountToLine(sender, line, res, multInv);
				}
			}
		}

		public static void ValidateMinGrossProfitUnitPrice<InfoKeyField>(PXCache sender, Line line, ref decimal? curyNewUnitPrice)
		where InfoKeyField : IBqlField
		{
			if (sender.Graph.UnattendedMode) return;
			SOSetup sosetup = SOSetupSelect.Select(sender.Graph);
			if (sosetup.MinGrossProfitValidation == MinGrossProfitValidationType.None) return;

			if (line != null && line.InventoryID != null && line.UOM != null && curyNewUnitPrice >= 0 && line.IsFree != true)
			{
				var r = (PXResult<InventoryItem, INItemCost>)
					PXSelectJoin<InventoryItem,
						LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
					Where<InventoryItem.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>>>
					.Select(sender.Graph, line.InventoryID);
				InventoryItem item = r;
				INItemCost cost = r;

				if (item != null)
				{
					decimal minPrice = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, PXPriceCostAttribute.MinPrice(item, cost), INPrecision.UNITCOST);

					if (sender.GetValue<InfoKeyField>(line) != null)
					{
						PXDBCurrencyAttribute.CuryConvCury<InfoKeyField>(sender, line, minPrice, out minPrice, true);
					}

					if (curyNewUnitPrice < minPrice)
					{
						switch (sosetup.MinGrossProfitValidation)
						{
							case MinGrossProfitValidationType.Warning:
								sender.RaiseExceptionHandling(CuryUnitPrice, line, curyNewUnitPrice, new PXSetPropertyException(Messages.GrossProfitValidationFailed, PXErrorLevel.Warning));
								break;
							case MinGrossProfitValidationType.SetToMin:
								curyNewUnitPrice = minPrice;
								sender.RaiseExceptionHandling(CuryUnitPrice, line, curyNewUnitPrice, new PXSetPropertyException(Messages.GrossProfitValidationFailedAndUnitPriceFixed, PXErrorLevel.Warning));
								break;
							default:
								break;
						}
					}
					else if (sosetup.MinGrossProfitValidation != MinGrossProfitValidationType.None && minPrice == 0m && item.ValMethod != INValMethod.Standard)
					{
						sender.RaiseExceptionHandling(CuryUnitPrice, line, curyNewUnitPrice, new PXSetPropertyException(Messages.GrossProfitValidationFailedNoCost, PXErrorLevel.Warning));
					}
				}
			}
		}

		/// <summary>
		/// Validates Discount %. The Unit Price with discount for an item cannot be less then StdCost plus Minimal Gross Profit.
		/// </summary>
		/// <param name="sender">Cache</param>
		/// <param name="line">Target row</param>
		/// <param name="unitPrice">UnitPrice in base currency</param>
		/// <param name="newDiscPct">new Discount %</param>
		public static void ValidateMinGrossProfitPct(PXCache sender, Line line, decimal? unitPrice, ref decimal? newDiscPct)
		{
			if (sender.Graph.UnattendedMode) return;
			SOSetup sosetup = SOSetupSelect.Select(sender.Graph);
			if (sosetup.MinGrossProfitValidation == MinGrossProfitValidationType.None) return;

            if (line != null && unitPrice > 0 && newDiscPct > 0 && line.IsFree != true)
			{
				var r = (PXResult<InventoryItem, INItemCost>)
					PXSelectJoin<InventoryItem,
						LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
					Where<InventoryItem.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>>>
					.Select(sender.Graph, line.InventoryID);
				InventoryItem item = r;
				INItemCost cost = r;

				if (item != null)
				{
					decimal? unitPriceAfterDiscount = unitPrice - (newDiscPct * unitPrice * 0.01m);
					decimal? minPrice = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, PXPriceCostAttribute.MinPrice(item, cost), INPrecision.UNITCOST);
					decimal? maxDiscPct = (unitPrice - minPrice) * 100 / unitPrice;

					if (unitPriceAfterDiscount < minPrice)
					{
						switch (sosetup.MinGrossProfitValidation)
						{
							case MinGrossProfitValidationType.Warning:
								sender.RaiseExceptionHandling(DiscPct, line, newDiscPct, new PXSetPropertyException(Messages.GrossProfitValidationFailed, PXErrorLevel.Warning));
								break;
							case MinGrossProfitValidationType.SetToMin:
								newDiscPct = maxDiscPct;
								sender.RaiseExceptionHandling(DiscPct, line, newDiscPct, new PXSetPropertyException(Messages.GrossProfitValidationFailedAndFixed, PXErrorLevel.Warning));
								break;
							default:
								break;
						}
					}
					else if (sosetup.MinGrossProfitValidation != MinGrossProfitValidationType.None && minPrice == 0m && item.ValMethod != INValMethod.Standard)
					{
						sender.RaiseExceptionHandling(DiscPct, line, newDiscPct, new PXSetPropertyException(Messages.GrossProfitValidationFailedNoCost, PXErrorLevel.Warning));
					}
				}
			}
		}

		/// <summary>
		/// Validates Discount Amount. The Unit Price with discount for an item cannot be less then StdCost plus Minimal Gross Profit.
		/// </summary>
		/// <param name="sender">Cache</param>
		/// <param name="line">Target row</param>
		/// <param name="unitPrice">UnitPrice in base currency</param>
		/// <param name="newDiscPct">new Discount amount</param>
		public static void ValidateMinGrossProfitAmt(PXCache sender, Line line, decimal? unitPrice, ref decimal? newDisc)
		{
			if (sender.Graph.UnattendedMode) return;
			SOSetup sosetup = SOSetupSelect.Select(sender.Graph);
			if (sosetup.MinGrossProfitValidation == MinGrossProfitValidationType.None) return;

            if (line != null && Math.Abs(line.Qty.GetValueOrDefault()) > 0 && newDisc > 0 && unitPrice > 0 && line.IsFree != true)
			{
				var r = (PXResult<InventoryItem, INItemCost>)
					PXSelectJoin<InventoryItem,
						LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
					Where<InventoryItem.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>>>
					.Select(sender.Graph, line.InventoryID);
				InventoryItem item = r;
				INItemCost cost = r;

				if (item != null)
				{
					decimal? unitPriceAfterDiscount = unitPrice - (newDisc / Math.Abs(line.Qty.Value));
					decimal? minPrice = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, PXPriceCostAttribute.MinPrice(item, cost), INPrecision.UNITCOST);
					decimal? maxDisc = Math.Abs(line.Qty.Value) * (unitPrice - minPrice);

					if (unitPriceAfterDiscount < minPrice)
					{
						switch (sosetup.MinGrossProfitValidation)
						{
							case MinGrossProfitValidationType.Warning:
								sender.RaiseExceptionHandling(CuryDiscAmt, line, newDisc, new PXSetPropertyException(Messages.GrossProfitValidationFailed, PXErrorLevel.Warning));
								break;
							case MinGrossProfitValidationType.SetToMin:
								newDisc = maxDisc;
								sender.RaiseExceptionHandling(CuryDiscAmt, line, newDisc, new PXSetPropertyException(Messages.GrossProfitValidationFailedAndFixed, PXErrorLevel.Warning));
								break;
							default:
								break;
						}
					}
					else if (sosetup.MinGrossProfitValidation != MinGrossProfitValidationType.None && minPrice == 0m && item.ValMethod != INValMethod.Standard)
					{
						sender.RaiseExceptionHandling(CuryDiscAmt, line, newDisc, new PXSetPropertyException(Messages.GrossProfitValidationFailedNoCost, PXErrorLevel.Warning));
					}
				}

			}
		}

		private static void UpdateLineLevelDiscountDetail<DiscountDetail>(PXCache sender, PXSelectBase<Line> transactions, PXSelectBase<DiscountDetail> discountDetails, Line line, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			IList<Line> lines1 = GetDetailDependableLines(sender, transactions, line.DetDiscIDC1, line.DetDiscSeqIDC1);
			IList<Line> lines2 = GetDetailDependableLines(sender, transactions, line.DetDiscIDC2, line.DetDiscSeqIDC2);

			IList<DiscountDetail> details = UpdateLineLevelDiscountDetail<DiscountDetail>(sender, lines1, lines2, line, date, prorate);

			foreach (DiscountDetail detail in details)
			{
				UpdateInsertDiscountTrace(sender, discountDetails, detail);
			}
		}

		public static IList<DiscountDetail> UpdateLineLevelDiscountDetail<DiscountDetail>(PXCache sender, IList<Line> lines1, IList<Line> lines2, Line line, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			List<DiscountDetail> list = new List<DiscountDetail>();

			if (!string.IsNullOrEmpty(line.DetDiscIDC2) &&
				(line.DetDiscIDC1 != line.DetDiscIDC2 || line.DetDiscSeqIDC1 != line.DetDiscSeqIDC2)
				)
			{
				//Combinations: COMP1 = A COMP2 = B

				DiscountDetail first = AggregateDetailDiscounts<DiscountDetail>(sender, lines1, line.DetDiscIDC1, line.DetDiscSeqIDC1, date, prorate);
				if (first != null)
				{
					list.Add(first);
				}

				DiscountDetail second = AggregateComplexDiscounts<DiscountDetail>(sender, lines2, line, date, prorate);

				if (second != null)
				{
					list.Add(second);
				}
			}
			else
			{
				//Combinations: COMP1 = A COMP2 = NULL, COMP1 = A COMP2 = A

				DiscountDetail first = AggregateDetailDiscounts<DiscountDetail>(sender, lines1, line.DetDiscIDC1, line.DetDiscSeqIDC1, date, prorate);
				if (first != null)
				{
					list.Add(first);
				}
			}

			return list;
		}

		public static IList<DiscountDetail> UpdateDocumentLevelDiscountDetail<DiscountDetail>(PXCache sender, IList<Line> lines1, IList<Line> lines2, Line line, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			List<DiscountDetail> list = new List<DiscountDetail>();

			DiscountDetail first = AggregateDocumentDiscounts<DiscountDetail>(sender, lines1, line.DocDiscIDC1, line.DocDiscSeqIDC1, date, prorate);

			if (first != null)
			{
				list.Add(first);
			}


			DiscountDetail second = AggregateDocumentDiscounts<DiscountDetail>(sender, lines2, line.DocDiscIDC2, line.DocDiscSeqIDC2, date, prorate);

			if (second != null)
			{
				list.Add(second);
			}

			return list;
		}

		public static void UpdateDocumentLevelDiscountDetail<DiscountDetail>(PXCache sender, PXSelectBase<Line> transactions, PXSelectBase<DiscountDetail> discountDetails, Line line, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			IList<Line> lines1 = GetDocumentDependableLines(sender, transactions, line.DocDiscIDC1, line.DocDiscSeqIDC1);
			IList<Line> lines2 = GetDocumentDependableLines(sender, transactions, line.DocDiscIDC2, line.DocDiscSeqIDC2);
			IList<DiscountDetail> details = UpdateDocumentLevelDiscountDetail<DiscountDetail>(sender, lines1, lines2, line, date, prorate);

			foreach (DiscountDetail detail in details)
			{
				UpdateInsertDiscountTrace<DiscountDetail>(sender, discountDetails, detail);
			}
		}

		public static void UpdateInsertDiscountTrace<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, DiscountDetail newTrace)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			DiscountDetail trace = GetDiscountDetail(sender, discountDetails, newTrace.DiscountID, newTrace.DiscountSequenceID, newTrace.Type);

			if (trace != null)
			{
				trace.CuryDiscountableAmt = newTrace.CuryDiscountableAmt;
				trace.DiscountableQty = newTrace.DiscountableQty;
				trace.CuryDiscountAmt = newTrace.CuryDiscountAmt ?? 0;
				trace.DiscountPct = newTrace.DiscountPct;
				trace.FreeItemID = newTrace.FreeItemID;
				trace.FreeItemQty = newTrace.FreeItemQty;

				discountDetails.Update(trace);
			}
			else
			{
				discountDetails.Insert(newTrace);
			}
		}

		private static DiscountDetail GetDiscountDetail<DiscountDetail>(PXCache sender, PXSelectBase<DiscountDetail> discountDetails, string discountID, string discountSequenceID, string type)
where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			PXCache cache = sender.Graph.Caches[typeof(DiscountDetail)];

			Type DiscountDetail_DiscountID = cache.GetBqlField(DiscountID);
			Type DiscountDetail_DiscountSequenceID = cache.GetBqlField(DiscountSequenceID);
			Type DiscountDetail_Type = cache.GetBqlField(Type);

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

			return (DiscountDetail)view.SelectSingle(discountID, discountSequenceID, type);

		}

		private static DiscountDetail AggregateComplexDiscounts<DiscountDetail>(PXCache sender, IList<Line> lines, Line currentLine, DateTime date, bool prorate)
			where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
            ARSetup arsetup = PXSelectReadonly<ARSetup>.Select(sender.Graph);
			CommonSetup commonsetup = PXSelectReadonly<CommonSetup>.Select(sender.Graph);
			int precision = 4;
			if (commonsetup != null && commonsetup.DecPlPrcCst != null)
				precision = commonsetup.DecPlPrcCst.Value;

			DiscountSequence sequence = GetDiscountSequenceByID(sender, currentLine.DetDiscIDC2, currentLine.DetDiscSeqIDC2);
			if (sequence == null)
				return null;

			if (lines.Count == 0)
				return null;

			decimal totalValue = 0;
			decimal totalDiscount = 0;
			decimal totalFreeItemQty = 0;

			foreach (Line line in lines)
			{
                if (line.ManualDisc == true || line.IsFree == true)
                    continue;

				decimal lineDiscount = 0;

				if (line.DetDiscApp == false)
				{
					SingleDiscountResult dr = CalculateDiscount(sender, sequence, line, date, prorate);
					totalValue += dr.Value;
					if (!dr.IsEmpty)
					{
						lineDiscount = dr.Discount ?? 0;
						if (line.IsFree != true)
							totalFreeItemQty += dr.FreeItemQty ?? 0;
					}

                    if (arsetup != null && sequence.DiscountedFor == DiscountOption.Amount && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
					{
						lineDiscount = lineDiscount * Math.Abs(line.Qty.Value);
					}

					if (sequence.DiscountedFor == DiscountOption.Percent)
					{
						decimal discountAmt;
                        if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
						{
							decimal salesPriceAfterDiscount = (line.CuryUnitPrice ?? 0) * 0.01m * lineDiscount;
							discountAmt = Math.Abs(line.Qty ?? 0) * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
						}
						else
						{
							discountAmt = Math.Abs(line.Qty ?? 0) * (line.CuryUnitPrice ?? 0) * 0.01m * lineDiscount;
						}

						totalDiscount += discountAmt;
					}
					else
						totalDiscount += lineDiscount;
				}
				else
				{
					DiscountSequence firstSequence = GetDiscountSequenceByID(sender, line.DetDiscIDC1, line.DetDiscSeqIDC1);
					SingleDiscountResult firstComponent = CalculateDiscount(sender, firstSequence, line, date, prorate);
					if (!firstComponent.IsEmpty)
					{
						decimal breakValue;

						if (firstSequence.BreakBy == BreakdownType.Amount && sequence.BreakBy == BreakdownType.Amount)
						{
							if (firstSequence.DiscountedFor == DiscountOption.Percent)
							{
								breakValue = firstComponent.Value * (1 - 0.01m * firstComponent.Discount.Value);
							}
							else
							{
								breakValue = firstComponent.Value - firstComponent.Discount.Value;
							}
						}
						else
						{
							if (sequence.BreakBy == BreakdownType.Quantity)
							{
								breakValue = ConvertToSalesUnits(sender, line.InventoryID, line.Qty.Value, line.UOM);
							}
							else
							{
								breakValue = line.CuryLineAmt.Value;
							}
						}

						SingleDiscountResult dr = CalculateDiscount(sender, sequence, breakValue, date, prorate);
						totalValue += dr.Value;
						if (!dr.IsEmpty)
						{
							if (line.IsFree != true)
								totalFreeItemQty += dr.FreeItemQty ?? 0;
							lineDiscount += dr.Discount.Value;
						}
					}

					decimal firstDiscount = firstComponent.Discount ?? 0;
                    if (arsetup != null && sequence.DiscountedFor == DiscountOption.Amount && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
					{
						firstDiscount = firstDiscount * Math.Abs(line.Qty.Value);
						lineDiscount = lineDiscount * Math.Abs(line.Qty.Value);
					}

					if (sequence.DiscountedFor == DiscountOption.Percent)
					{
						decimal discountAmt;
                        if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
						{
							decimal salesPriceAfterFirstDiscount = (line.CuryUnitPrice ?? 0) * (1 - 0.01m * firstDiscount);
							decimal salesPriceAfterDiscount = salesPriceAfterFirstDiscount * 0.01m * lineDiscount;
							discountAmt = Math.Abs(line.Qty ?? 0) * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
						}
						else
						{
							discountAmt = Math.Abs(line.Qty ?? 0) * (line.CuryUnitPrice ?? 0) * (1 - 0.01m * firstDiscount) * 0.01m * lineDiscount;
						}

						totalDiscount += discountAmt;
					}
					else
						totalDiscount += lineDiscount;

				}


			}

			DiscountDetail trace = new DiscountDetail();
			trace.DiscountID = currentLine.DetDiscIDC2;
			trace.DiscountSequenceID = currentLine.DetDiscSeqIDC2;
			trace.Type = DiscountType.Line;
			trace.FreeItemID = GetFreeItem(sequence, date);
			if (trace.FreeItemID != null)
				trace.FreeItemQty = totalFreeItemQty;

			decimal discAmt = totalDiscount;
			decimal curyDiscAmt;
			PXCurrencyAttribute.CuryConvCury(sender, lines[0], discAmt, out curyDiscAmt);
			trace.CuryDiscountAmt = curyDiscAmt;

			decimal linesSum = SumAmounts(sender, lines);
			if (linesSum > 0)
				trace.DiscountPct = totalDiscount * 100 / linesSum;//prorating

			if (sequence.BreakBy == BreakdownType.Quantity)
				trace.DiscountableQty = totalValue;
			else
			{
				decimal curyDiscAmt2;
				PXCurrencyAttribute.CuryConvCury(sender, lines[0], totalValue, out curyDiscAmt2);
				trace.CuryDiscountableAmt = curyDiscAmt2;
			}

			return trace;

		}

		private static DiscountDetail AggregateDetailDiscounts<DiscountDetail>(PXCache sender, IList<Line> lines, string discountID, string discountSequenceID, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			DiscountSequence sequence = GetDiscountSequenceByID(sender, discountID, discountSequenceID);
			if (sequence == null)
				return null;

			decimal baseTotalAmt = SumAmounts(sender, lines);

			if (lines.Count == 0)
				return null;

            ARSetup arsetup = PXSelectReadonly<ARSetup>.Select(sender.Graph);
			CommonSetup commonsetup = PXSelectReadonly<CommonSetup>.Select(sender.Graph);
			int precision = 4;
			if (commonsetup != null && commonsetup.DecPlPrcCst != null)
				precision = commonsetup.DecPlPrcCst.Value;


			decimal totalValue = 0;//in base currency
			decimal totalFreeItemQty = 0;
			decimal totalDiscountAmt = 0;
			decimal totalDiscountAmtRaw = 0; //no rounding
			foreach (Line line in lines)
			{
				if (line.ManualDisc == true || line.IsFree == true)
					continue;

				decimal qty = Math.Abs(line.Qty ?? 0);
				decimal lineDiscountAmt = 0;//in base
				decimal lineDiscountAmtRaw = 0;//in base no rounding

				if (line.DetDiscIDC2 == discountID
					&& line.DetDiscSeqIDC2 == discountSequenceID)
				{
					SingleDiscountResult firstComponent = CalculateDiscount(sender, sequence, line, date, prorate);
					totalValue += firstComponent.Value;//in base currency
					if (!firstComponent.IsEmpty)
					{
						if (line.IsFree != true)
							totalFreeItemQty += firstComponent.FreeItemQty ?? 0;

						if (firstComponent.Discount != null)
						{
							if (sequence.DiscountedFor == DiscountOption.Amount)
							{
								lineDiscountAmt += firstComponent.Discount.Value;

							}
							else
							{
								decimal curyDiscAmt;
                                if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
								{
									decimal salesPriceAfterDiscount = (line.CuryUnitPrice ?? 0) * (1 - 0.01m * firstComponent.Discount.Value);
									decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
									curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(sender, line, extPriceAfterDiscount, CMPrecision.TRANCURY);
								}
								else
								{
									curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) * 0.01m * firstComponent.Discount.Value;
								}

								decimal baseDiscAmt;
								PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmt, out baseDiscAmt);

								lineDiscountAmt += baseDiscAmt;

								decimal baseDiscAmtRaw;
								decimal curyDiscAmtRaw = qty * (line.CuryUnitPrice ?? 0) * 0.01m * firstComponent.Discount.Value;
								PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmtRaw, out baseDiscAmtRaw, true);
								lineDiscountAmtRaw += baseDiscAmtRaw;
							}
						}

						if (line.DetDiscApp == true)
						{
							decimal breakValue;
							if (sequence.DiscountedFor == DiscountOption.Percent)
							{
								breakValue = firstComponent.Discount.Value * firstComponent.Value / 100;
							}
							else
								breakValue = firstComponent.Discount.Value;

							SingleDiscountResult dr = CalculateDiscount(sender, sequence, breakValue, date, prorate);
							totalValue += dr.Value;//in base currency
							if (!dr.IsEmpty)
							{
								if (line.IsFree != true)
									totalFreeItemQty += dr.FreeItemQty ?? 0;
								if (dr.Discount != null)
								{
									if (sequence.DiscountedFor == DiscountOption.Amount)
									{
										lineDiscountAmt += dr.Discount.Value;

									}
									else
									{

										decimal curyDiscAmt;
                                        if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
										{
											decimal salesPriceAfterDiscount = (line.CuryUnitPrice ?? 0) * (1 - 0.01m * dr.Discount.Value);
											decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
											curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(sender, line, extPriceAfterDiscount, CMPrecision.TRANCURY);
										}
										else
										{
											curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
										}


										decimal baseDiscAmt;
										PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmt, out baseDiscAmt);
										lineDiscountAmt += baseDiscAmt;

										decimal baseDiscAmtRaw;
										decimal curyDiscAmtRaw = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
										PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmtRaw, out baseDiscAmtRaw, true);
										lineDiscountAmtRaw += baseDiscAmtRaw;
									}
								}
							}
						}
						else
						{
							SingleDiscountResult dr = CalculateDiscount(sender, sequence, line, date, prorate);
							totalValue += dr.Value;//in base currency
							if (!dr.IsEmpty)
							{
								if (line.IsFree != true)
									totalFreeItemQty += dr.FreeItemQty ?? 0;
								if (dr.Discount != null)
								{
									if (sequence.DiscountedFor == DiscountOption.Amount)
									{
										lineDiscountAmt += dr.Discount.Value;
									}
									else
									{
										decimal curyDiscAmt;
                                        if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
										{
											decimal salesPriceAfterDiscount = (line.CuryUnitPrice ?? 0) * (1 - 0.01m * dr.Discount.Value);
											decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
											curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(sender, line, extPriceAfterDiscount, CMPrecision.TRANCURY);
										}
										else
										{
											curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
										}

										decimal baseDiscAmt;
										PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmt, out baseDiscAmt);
										lineDiscountAmt += baseDiscAmt;

										decimal baseDiscAmtRaw;
										decimal curyDiscAmtRaw = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
										PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmtRaw, out baseDiscAmtRaw, true);
										lineDiscountAmtRaw += baseDiscAmtRaw;
									}
								}
							}

						}
					}
				}
				else
				{
					SingleDiscountResult dr = CalculateDiscount(sender, sequence, line, date, prorate);
					totalValue += dr.Value;//in base currency
					if (!dr.IsEmpty)
					{
						if (line.IsFree != true)
							totalFreeItemQty += dr.FreeItemQty ?? 0;
						if (dr.Discount != null)
						{
							if (sequence.DiscountedFor == DiscountOption.Amount)
							{
								lineDiscountAmt += dr.Discount.Value;
							}
							else
							{
								decimal curyDiscAmt;
                                if (arsetup != null && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
								{
									decimal salesPriceAfterDiscount = (line.CuryUnitPrice ?? 0) * (1 - 0.01m * dr.Discount.Value);
									decimal extPriceAfterDiscount = qty * Math.Round(salesPriceAfterDiscount, precision, MidpointRounding.AwayFromZero);
									curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) - PXCurrencyAttribute.Round(sender, line, extPriceAfterDiscount, CMPrecision.TRANCURY);
								}
								else
								{
									curyDiscAmt = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
								}


								decimal baseDiscAmt;
								PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmt, out baseDiscAmt);
								lineDiscountAmt += baseDiscAmt;

								decimal baseDiscAmtRaw;
								decimal curyDiscAmtRaw = qty * (line.CuryUnitPrice ?? 0) * 0.01m * dr.Discount.Value;
								PXCurrencyAttribute.CuryConvBase(sender, line, curyDiscAmtRaw, out baseDiscAmtRaw, true);
								lineDiscountAmtRaw += baseDiscAmtRaw;
							}
						}
					}
				}

                if (arsetup != null && sequence.DiscountedFor == DiscountOption.Amount && arsetup.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
				{
					lineDiscountAmt = lineDiscountAmt * qty;
					lineDiscountAmtRaw = lineDiscountAmtRaw * qty;
				}

				totalDiscountAmt += lineDiscountAmt;
				totalDiscountAmtRaw += lineDiscountAmtRaw;
			}

			ARDiscount dc = GetDiscountByID(sender, discountID);

			DiscountDetail trace = new DiscountDetail();
			trace.DiscountID = discountID;
			trace.DiscountSequenceID = discountSequenceID;
			trace.Type = dc.Type;
			trace.FreeItemID = GetFreeItem(sequence, date);
			if (trace.FreeItemID != null)
				trace.FreeItemQty = totalFreeItemQty;

			if (dc.Type == DiscountType.Line)
			{
				if (sequence.DiscountedFor == DiscountOption.Percent)
				{
					if (totalDiscountAmtRaw > 0)
					{
						decimal curyDiscAmt;
						PXCurrencyAttribute.CuryConvCury(sender, lines[0], totalDiscountAmt, out curyDiscAmt);
						trace.CuryDiscountAmt = curyDiscAmt;

						if (baseTotalAmt > 0)
							trace.DiscountPct = totalDiscountAmtRaw * 100 / baseTotalAmt;//prorating
					}
				}
				else
				{
					decimal curyDiscAmt;
					PXCurrencyAttribute.CuryConvCury(sender, lines[0], totalDiscountAmt, out curyDiscAmt);
					trace.CuryDiscountAmt = curyDiscAmt;

					if (baseTotalAmt > 0)
						trace.DiscountPct = totalDiscountAmt * 100 / baseTotalAmt;//prorating
				}
			}
			else if (dc.Type == DiscountType.Flat)
			{
				trace.DiscountPct = 0;
				trace.CuryDiscountAmt = 0;
			}

			if (sequence.BreakBy == BreakdownType.Quantity)
				trace.DiscountableQty = totalValue;
			else
			{
				decimal curyDiscAmt;
				PXCurrencyAttribute.CuryConvCury(sender, lines[0], totalValue, out curyDiscAmt);
				trace.CuryDiscountableAmt = curyDiscAmt;
			}
			return trace;
		}

		public static DiscountDetail AggregateDocumentDiscounts<DiscountDetail>(PXCache sender, IList<Line> dependableLines, string discountID, string discountSequenceID, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			DiscountSequence sequence = GetDiscountSequenceByID(sender, discountID, discountSequenceID);
			if (sequence == null)
				return null;

			if (dependableLines.Count == 0)
				return null;

			INSetup insetup = PXSelectReadonly<INSetup>.Select(sender.Graph);

			decimal linesSum = SumAmounts(sender, dependableLines, true);
			decimal aggregatedValue;
			if (sequence.BreakBy == BreakdownType.Quantity)
			{
				aggregatedValue = SumQuantity(sender, dependableLines);
			}
			else
			{
				aggregatedValue = linesSum;
			}

			SingleDiscountResult dr = CalculateDiscount(sender, sequence, aggregatedValue, date, prorate);

			DiscountDetail trace = new DiscountDetail();
			trace.DiscountID = discountID;
			trace.DiscountSequenceID = discountSequenceID;
			trace.Type = DiscountType.Document;
			trace.FreeItemID = dr.FreeItemID;
			if (trace.FreeItemID != null)
				trace.FreeItemQty = dr.FreeItemQty ?? 0;
			if (sequence.DiscountedFor == DiscountOption.Percent)
			{
				trace.DiscountPct = dr.Discount ?? 0;

				decimal discAmt = linesSum * 0.01m * (dr.Discount ?? 0);
				decimal curyDiscAmt;
				PXCurrencyAttribute.CuryConvCury(sender, dependableLines[0], discAmt, out curyDiscAmt);
				trace.CuryDiscountAmt = curyDiscAmt;
			}
			else
			{
				decimal discAmt = dr.Discount ?? 0;
				decimal curyDiscAmt;
				PXCurrencyAttribute.CuryConvCury(sender, dependableLines[0], discAmt, out curyDiscAmt);
				trace.CuryDiscountAmt = curyDiscAmt;

				if (linesSum > 0)
					trace.DiscountPct = (dr.Discount ?? 0) * 100 / linesSum;//prorating
			}
			if (sequence.BreakBy == BreakdownType.Quantity)
				trace.DiscountableQty = dr.Value;
			else
			{
				decimal discAmt = dr.Value;
				decimal curyDiscAmt;
				PXCurrencyAttribute.CuryConvCury(sender, dependableLines[0], discAmt, out curyDiscAmt);
				trace.CuryDiscountableAmt = curyDiscAmt;
			}

			return trace;
		}

		public static DiscountDetail AggregateFreeItemDiscounts<DiscountDetail>(PXCache sender, IList<SOShipLine> dependableLines, string discountID, string discountSequenceID, DateTime date, bool prorate)
		where DiscountDetail : class, IBqlTable, IDiscountDetail, new()
		{
			DiscountSequence sequence = GetDiscountSequenceByID(sender, discountID, discountSequenceID);
			if (sequence == null)
				return null;

			if (dependableLines.Count == 0)
				return null;

			CommonSetup commonsetup = PXSelectReadonly<CommonSetup>.Select(sender.Graph);
			int precision = 4;
			if (commonsetup != null && commonsetup.DecPlPrcCst != null)
				precision = commonsetup.DecPlPrcCst.Value;

			decimal linesSum = 0;
			decimal qtySum = 0;
			foreach (SOShipLine item in dependableLines)
			{
				linesSum += item.LineAmt ?? 0;
				qtySum += ConvertToSalesUnits(sender, item.InventoryID, Math.Abs(item.Qty ?? 0), item.UOM);
			}

			decimal aggregatedValue;
			if (sequence.BreakBy == BreakdownType.Quantity)
			{
				aggregatedValue = qtySum;
			}
			else
			{
				aggregatedValue = linesSum;
			}

			SingleDiscountResult dr = CalculateDiscount(sender, sequence, aggregatedValue, date, prorate);

			DiscountDetail trace = new DiscountDetail();
			trace.DiscountID = discountID;
			trace.DiscountSequenceID = discountSequenceID;
			trace.Type = DiscountType.Document;
			trace.FreeItemID = dr.FreeItemID;
			if (trace.FreeItemID != null)
				trace.FreeItemQty = dr.FreeItemQty ?? 0;
			if (sequence.BreakBy == BreakdownType.Quantity)
				trace.DiscountableQty = dr.Value;

			return trace;
		}


		private static IList<Line> GetDetailDependableLines(PXCache sender, PXSelectBase<Line> transactions, string discountID, string discountSequenceID)
		{
			PXCache cache = sender.Graph.Caches[typeof(Line)];

			Type DetDiscIDC1 = cache.GetBqlField(FirstDetailDiscountID);
			Type DetDiscSeqIDC1 = cache.GetBqlField(FirstDetailDiscountSequenceID);
			Type DetDiscIDC2 = cache.GetBqlField(SecondDetailDiscountID);
			Type DetDiscSeqIDC2 = cache.GetBqlField(SecondDetailDiscountSequenceID);

			return GetDependableLines(sender, transactions, discountID, discountSequenceID, DetDiscIDC1, DetDiscSeqIDC1, DetDiscIDC2, DetDiscSeqIDC2);
		}

		private static IList<Line> GetDocumentDependableLines(PXCache sender, PXSelectBase<Line> transactions, string discountID, string discountSequenceID)
		{
			PXCache cache = sender.Graph.Caches[typeof(Line)];

			Type DocDiscIDC1 = cache.GetBqlField(FirstDocumentDiscountID);
			Type DocDiscSeqIDC1 = cache.GetBqlField(FirstDocumentDiscountSequenceID);
			Type DocDiscIDC2 = cache.GetBqlField(SecondDocumentDiscountID);
			Type DocDiscSeqIDC2 = cache.GetBqlField(SecondDocumentDiscountSequenceID);

			return GetDependableLines(sender, transactions, discountID, discountSequenceID, DocDiscIDC1, DocDiscSeqIDC1, DocDiscIDC2, DocDiscSeqIDC2);
		}

		private static IList<Line> GetDependableLines(PXCache sender, PXSelectBase<Line> transactions, string discountID, string discountSequenceID, Type discIDC1, Type discSeqIDC1, Type discIDC2, Type discSeqIDC2)
		{
			#region Compose WHERE type
			/* Composition:
             Where<SOLine.firstDetailDiscountID, Equal<Required<SOLine.firstDetailDiscountID>>,
                And<SOLine.firstDetailDiscountSequenceID, Equal<Required<SOLine.firstDetailDiscountSequenceID>>
                    , Or<SOLine.secondDetailDiscountID, Equal<Required<SOLine.secondDetailDiscountID>>,
                    And<SOLine.secondDetailDiscountSequenceID, Equal<Required<SOLine.secondDetailDiscountSequenceID>>>>>
                    >
            */

			Type whereType = BqlCommand.Compose(
					typeof(Where<,,>),
					discIDC1,
					typeof(Equal<>),
					typeof(Required<>),
					discIDC1,
					typeof(And<,,>),
					discSeqIDC1,
					typeof(Equal<>),
					typeof(Required<>),
					discSeqIDC1,
					typeof(Or<,,>),
					discIDC2,
					typeof(Equal<>),
					typeof(Required<>),
					discIDC2,
					typeof(And<,>),
					discSeqIDC2,
					typeof(Equal<>),
					typeof(Required<>),
					discSeqIDC2
					);

			#endregion

			PXView view = sender.Graph.TypedViews.GetView(transactions.View.BqlSelect.WhereAnd(whereType), false);
			List<object> records = view.SelectMulti(discountID, discountSequenceID, discountID, discountSequenceID);
			List<Line> list = new List<Line>(records.Count);
			foreach (object record in records)
			{
				list.Add((Line)record);
			}

			return list;
		}

		private static DiscountResult CalculateDetailDiscount(PXCache sender, Line line, int? locationID, DateTime date, decimal? amount, decimal? quantity, bool prorate, bool isUnitPriceDiscount)
		{
			#region Parameter Validations

			if (sender == null)
				throw new ArgumentNullException("sender");

			if (amount == null)
				throw new ArgumentNullException("amount");

			if (quantity == null)
				throw new ArgumentNullException("Quantity");

			#endregion

			decimal? discount = null; //depending on the type of firstSequence its either an Amount or Percentage.
			int? firstFreeItemID = null;
			decimal? firstFreeItemQty = null;
			int? secondFreeItemID = null;
			decimal? secondFreeItemQty = null;


			DiscountSequence firstSequence = GetDiscountSequenceByID(sender, line.DetDiscIDC1, line.DetDiscSeqIDC1);
			DiscountSequence secondSequence = GetDiscountSequenceByID(sender, line.DetDiscIDC2, line.DetDiscSeqIDC2);
			bool isCompoundDiscount = line.DetDiscApp == true;

			if (firstSequence != null)
			{
				bool isAmount = firstSequence.DiscountedFor == DiscountOption.Amount;

				#region Process First Component

				GetDiscount(sender, firstSequence, date, line.InventoryID, line.UOM, quantity.Value, line.CuryLineAmt, prorate, out discount, out firstFreeItemID, out firstFreeItemQty);

				#endregion

				if (secondSequence != null)
				{
					decimal breakValue;
					if (secondSequence.BreakBy == BreakdownType.Quantity)
					{
						if (line.InventoryID != null)
							breakValue = ConvertToSalesUnits(sender, line.InventoryID, quantity.Value, line.UOM);
						else
						{
							breakValue = quantity.GetValueOrDefault();
						}
					}
					else
					{
						breakValue = amount.Value;
					}

					if (firstSequence.DiscountedFor == DiscountOption.Percent
						&& secondSequence.DiscountedFor == DiscountOption.Percent
						&& isCompoundDiscount)
					{
						/*Calculate Compound discounts with percents:
						  Discount = 100 - (100-d1)x(100-d2)/100;
						*/
						SingleDiscountResult res = CalculateDiscount(sender, secondSequence, breakValue, date, prorate);
						if (!res.IsEmpty)
						{
							secondFreeItemID = res.FreeItemID;
							secondFreeItemQty = res.FreeItemQty;

							discount = 100 - (100 - (discount ?? 0)) * (100 - res.Discount.Value) / 100;
						}
					}
					else
					{
						if (secondSequence.BreakBy == BreakdownType.Amount && isCompoundDiscount)
						{
							if (firstSequence.BreakBy == BreakdownType.Amount)
							{
								if (firstSequence.DiscountedFor == DiscountOption.Percent)
									breakValue = breakValue - (breakValue * 0.01m * (discount ?? 0));
								else
									breakValue = breakValue - (discount ?? 0);
							}
							else
							{
								//    if (firstSequence.DiscountedFor == DiscountOption.Percent)
								//        breakValue = breakValue - (breakValue * 0.01m * (discount ?? 0));
								//    else
								//        breakValue = breakValue - (discount ?? 0);
							}
						}

						SingleDiscountResult res = CalculateDiscount(sender, secondSequence, breakValue, date, prorate);

						if (!res.IsEmpty)
						{
							decimal result = res.Discount.Value;
							secondFreeItemID = res.FreeItemID;
							secondFreeItemQty = res.FreeItemQty;

							if (firstSequence.DiscountedFor != secondSequence.DiscountedFor)
							{
								
								if (firstSequence.DiscountedFor == DiscountOption.Amount)
								{
									//convert second discount to amount
									result = result * breakValue / 100m;
									isAmount = true;

								}
								else
								{
									isAmount = true;

									//convert first discount to amount:
									if (isUnitPriceDiscount)
									{
										discount = line.CuryUnitPrice * discount / 100m;
									}
									else
									{
										discount = line.CuryLineAmt * discount / 100m;
									}
								}
							}

							discount = (discount ?? 0) + result;
						}
					}
				}

				return new DiscountResult(discount, firstFreeItemID, firstFreeItemQty, secondFreeItemID, secondFreeItemQty, isAmount);
			}
			else
			{
				return new DiscountResult();
			}
		}

		public static SingleDiscountResult CalculateDiscount(PXCache sender, DiscountSequence sequence, Line line, DateTime date, bool prorate)
		{
			decimal breakValue;
			if (sequence.BreakBy == BreakdownType.Quantity)
			{
				breakValue = ConvertToSalesUnits(sender, line.InventoryID, Math.Abs(line.Qty ?? 0), line.UOM);
			}
			else
			{
				if (line.CuryExtPrice != null && line.CuryExtPrice != 0)
				{
					decimal curyLineAmt = line.CuryExtPrice ?? 0;// (line.CuryUnitPrice ?? 0) * (line.Qty ?? 0);//don't use line.CuryLineAmt because its already modified with discount;
					PXCurrencyAttribute.CuryConvBase(sender, line, curyLineAmt, out breakValue, false);
				}
				else
				{
					breakValue = 0;
				}
			}


			return CalculateDiscount(sender, sequence, breakValue, date, prorate);
		}

		private static decimal SumAmounts(PXCache sender, IList<Line> lines)
		{
			return SumAmounts(sender, lines, false);
		}

		private static decimal SumAmounts(PXCache sender, IList<Line> lines, bool includeLineDiscount)
		{
			decimal result = 0;

			foreach (Line item in lines)
			{
				decimal curyLineAmt;
				if (includeLineDiscount)
					curyLineAmt = item.CuryLineAmt ?? 0;
				else
					curyLineAmt = (item.CuryUnitPrice ?? 0) * Math.Abs(item.Qty ?? 0);

				decimal baseLineAmt;
				PXCurrencyAttribute.CuryConvBase(sender, item, curyLineAmt, out baseLineAmt, true);
				result += baseLineAmt;
			}

			return result;
		}

		private static decimal SumQuantity(PXCache sender, IList<Line> lines)
		{
			decimal result = 0;

			foreach (Line item in lines)
			{
				if ( item.IsFree != true )
					result += ConvertToSalesUnits(sender, item.InventoryID, Math.Abs(item.Qty ?? 0), item.UOM);
			}

			return result;
		}
	}

	public interface IDiscountable
	{
		int? InventoryID { get; }
		int? CustomerID { get; }
		decimal? Qty { get; }
		Int64? CuryInfoID { get; }
		decimal? CuryLineAmt { get; }
		decimal? CuryExtPrice { get; }
		decimal? CuryDiscAmt { get; set; }
		decimal? DiscPct { get; set; }
		decimal? CuryUnitPrice { get; }
		string UOM { get; }
		bool? ManualDisc { get; }
		bool? IsFree { get; }

		string DetDiscIDC1 { get; set; }
		string DetDiscSeqIDC1 { get; set; }
		string DetDiscIDC2 { get; set; }
		string DetDiscSeqIDC2 { get; set; }
		bool? DetDiscApp { get; set; }

		string DocDiscIDC1 { get; set; }
		string DocDiscSeqIDC1 { get; set; }
		string DocDiscIDC2 { get; set; }
		string DocDiscSeqIDC2 { get; set; }
		string PromoDiscID { get; set; }
	}
}
