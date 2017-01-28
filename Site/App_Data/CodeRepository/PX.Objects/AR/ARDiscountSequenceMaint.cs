using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using System.Collections;
using PX.Objects.IN;
using PX.Objects.GL;
using System.Diagnostics;
using System.Linq;

namespace PX.Objects.AR
{
    [Serializable]
	public class ARDiscountSequenceMaint : PXGraph<ARDiscountSequenceMaint>
	{
        public ARDiscountSequenceMaint()
        {
            PXDBDefaultAttribute.SetDefaultForInsert<DiscountSequence.discountSequenceID>(Sequence.Cache, null, false);
        }

		#region Selects/Views
				
		public PXSelect<ARDiscountEx, Where<ARDiscountEx.discountID, Equal<Current<DiscountSequence.discountID>>>> Discount;
        public PXSelectJoin<DiscountSequence, InnerJoin<ARDiscount, On<DiscountSequence.discountID, Equal<ARDiscount.discountID>>>> Sequence;

        public PXSelect<DiscountSequence, Where<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>, 
            And<DiscountSequence.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> CurrentSequence;

        public PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
            And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>, OrderBy<Asc<DiscountDetail.quantity, Asc<DiscountDetail.amount>>>> Details;

        public PXSelect<DiscountSequenceDetail, Where<DiscountSequenceDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
            And<DiscountSequenceDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>, OrderBy<Asc<DiscountSequenceDetail.quantity, Asc<DiscountSequenceDetail.amount>>>> SequenceDetails;

		[PXImport(typeof(DiscountSequence))]
		public PXSelectJoin<DiscountItem,
			InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<DiscountItem.inventoryID>>>,
			Where<DiscountItem.discountID, Equal<Current<DiscountSequence.discountID>>,
			And<DiscountItem.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> Items;
		
		[PXImport(typeof(DiscountSequence))]
		public PXSelectJoin<DiscountCustomer,
		    InnerJoin<Customer, On<DiscountCustomer.customerID, Equal<Customer.bAccountID>>>,
		    Where<DiscountCustomer.discountID, Equal<Current<DiscountSequence.discountID>>,
		    And<DiscountCustomer.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> Customers;

		public PXSelectJoin<DiscountCustomerPriceClass,
			InnerJoin<ARPriceClass, On<DiscountCustomerPriceClass.customerPriceClassID, Equal<ARPriceClass.priceClassID>>>,
			Where<DiscountCustomerPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
			And<DiscountCustomerPriceClass.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> CustomerPriceClasses;

		public PXSelectJoin<DiscountInventoryPriceClass,
			InnerJoin<INPriceClass, On<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<INPriceClass.priceClassID>>>,
			Where<DiscountInventoryPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
			And<DiscountInventoryPriceClass.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> InventoryPriceClasses;

        public PXSelectJoin<DiscountBranch,
            InnerJoin<Branch, On<DiscountBranch.branchID, Equal<Branch.branchID>>>,
            Where<DiscountBranch.discountID, Equal<Current<DiscountSequence.discountID>>,
            And<DiscountBranch.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> Branches;

        public PXSelectJoin<DiscountSite,
            InnerJoin<INSite, On<DiscountSite.siteID, Equal<INSite.siteID>>>,
            Where<DiscountSite.discountID, Equal<Current<DiscountSequence.discountID>>,
            And<DiscountSite.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>>>> Sites;

		public PXFilter<UpdateSettingsFilter> UpdateSettings;

		#endregion
			
		#region Buttons/Actions

		private const string NewKey = " <NEW>";

		public PXSave<DiscountSequence> Save;
		[PXCancelButton]
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		protected virtual IEnumerable Cancel(PXAdapter a)
		{
			DiscountSequence current = null;
			string discountID = null;
			string sequenceID = null;

			#region Extract Keys
			if (a.Searches != null)
			{
				if (a.Searches.Length > 0)
					discountID = (string)a.Searches[0];
				if (a.Searches.Length > 1)
					sequenceID = (string)a.Searches[1];
			}
			#endregion

			DiscountSequence seq = PXSelect<DiscountSequence, 
				Where<DiscountSequence.discountSequenceID, Equal<Required<DiscountSequence.discountSequenceID>>,
				And<DiscountSequence.discountID, Equal<Required<DiscountSequence.discountID>>>>>.Select(this, sequenceID, discountID);

			ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(this, discountID);

			bool insertNewSequence = false;
			if (seq == null)
			{
                if (a.Searches != null && a.Searches.Length > 1)
                {
                    a.Searches[1] = null;
                }
				insertNewSequence = true;
			}

			if (Discount.Current != null && Discount.Current.DiscountID != discountID)
			{
				sequenceID = null;
			}

            foreach (PXResult<DiscountSequence, ARDiscount> headerCanceled in (new PXCancel<DiscountSequence>(this, "Cancel")).Press(a))
			{
                current = (DiscountSequence)headerCanceled;
			}

			if (insertNewSequence)
			{
				Sequence.Cache.Remove(current);

				DiscountSequence newSeq = new DiscountSequence();
				newSeq.DiscountID = discountID;

				if (discount != null)
				{
					newSeq.Description = discount.Description;
				}

				current = Sequence.Insert(newSeq);
				Sequence.Cache.IsDirty = false;

				if (discount != null)
				{
					if (discount.IsAutoNumber == false)
						current.DiscountSequenceID = sequenceID;
					else
						current.DiscountSequenceID = NewKey;

					Sequence.Cache.Normalize();
				}
			}
			
			yield return current;
		}

		public PXAction<DiscountSequence> cancel;
		public PXInsert<DiscountSequence> Insert;
		public PXDelete<DiscountSequence> Delete;
		public PXFirst<DiscountSequence> First;
		public PXPrevious<DiscountSequence> Prev;
		public PXNext<DiscountSequence> Next;
		public PXLast<DiscountSequence> Last;

		public PXAction<DiscountSequence> updateDiscounts;
		[PXUIField(DisplayName = "Update Discounts", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = true)]
		[PXProcessButton]
        public virtual IEnumerable UpdateDiscounts(PXAdapter adapter)
		{
			if (Sequence.Current != null)
			{
                if (UpdateSettings.AskExt() == WebDialogResult.OK)
                {
                    Save.Press();
                    ARUpdateDiscounts.UpdateDiscount(Sequence.Current.DiscountID, Sequence.Current.DiscountSequenceID, UpdateSettings.Current.FilterDate);
                    Sequence.Current.tstamp = PXDatabase.SelectTimeStamp();
                    Save.Press();
					SelectTimeStamp();
                    Details.Cache.Clear();
                    Details.Cache.ClearQueryCache();
                    CurrentSequence.Cache.Clear();
                    CurrentSequence.Cache.ClearQueryCache();
                    //PXLongOperation.StartOperation(this, delegate() { SOUpdateDiscounts.UpdateDiscount(Sequence.Current.DiscountID, Sequence.Current.DiscountSequenceID, UpdateSettings.Current.FilterDate); });
                }
			}

			return adapter.Get();
		}

		#endregion

		#region Event Handlers

		protected virtual void DiscountDetail_StartDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			DiscountDetail row = e.Row as DiscountDetail;
			if (row != null)
			{
				if (Sequence.Current != null && Sequence.Current.StartDate != null)
					e.NewValue = Sequence.Current.StartDate;
				else
					e.NewValue = Accessinfo.BusinessDate;
			}
		}

        protected virtual void DiscountSequence_IsPromotion_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            DiscountSequence row = e.Row as DiscountSequence;
            if (row != null)
            {
                if (row.IsPromotion == true)
                {
                    row.PendingFreeItemID = null;
                    row.LastFreeItemID = null;
                }
                else
                {
                    row.EndDate = null;
                }
            }
        }

		protected virtual void DiscountSequence_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DiscountSequence row = e.Row as DiscountSequence;
			ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(this, row.DiscountID);

			if (row != null && discount != null)
			{
				if (discount.Type == DiscountType.Document)
					row.BreakBy = BreakdownType.Amount;

				if (discount.Type == DiscountType.Flat)
				{
					row.BreakBy = BreakdownType.Quantity;
					row.DiscountedFor = DiscountOption.Amount;
				}
			}
		}

        protected virtual void DiscountDetail_Amount_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            DiscountDetail row = e.Row as DiscountDetail;
            if (row != null && Sequence.Current.IsPromotion == true)
            {
                PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.amount, Less<Required<DiscountDetail.amount>>>>>, OrderBy<Desc<DiscountDetail.amount>>>.SelectSingleBound(this, null, row.Amount);
                if (prevDetailLine != null)
                {
                    DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                    prevLine.AmountTo = row.Amount;
                    Details.Update(prevLine);
                }
                PXResult<DiscountDetail> nextDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.amount, Greater<Required<DiscountDetail.amount>>>>>, OrderBy<Asc<DiscountDetail.amount>>>.SelectSingleBound(this, null, row.Amount);
                if (nextDetailLine == null)
                {
                    row.AmountTo = null;
                }
                else
                {
                    DiscountDetail nextLine = (DiscountDetail)nextDetailLine;
                    row.AmountTo = nextLine.Amount;
                }
            }
        }

        protected virtual void DiscountDetail_Quantity_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            DiscountDetail row = e.Row as DiscountDetail;
            if (row != null && Sequence.Current.IsPromotion == true)
            {
                PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.quantity, Less<Required<DiscountDetail.quantity>>>>>, OrderBy<Desc<DiscountDetail.quantity>>>.SelectSingleBound(this, null, row.Quantity);
                if (prevDetailLine != null)
                {
                    DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                    prevLine.QuantityTo = row.Quantity;
                    Details.Update(prevLine);
                }
                PXResult<DiscountDetail> nextDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.quantity, Greater<Required<DiscountDetail.quantity>>>>>, OrderBy<Asc<DiscountDetail.quantity>>>.SelectSingleBound(this, null, row.Quantity);
                if (nextDetailLine == null)
                {
                    row.QuantityTo = null;
                }
                else
                {
                    DiscountDetail nextLine = (DiscountDetail)nextDetailLine;
                    row.QuantityTo = nextLine.Quantity;
                }
            }
        }

		protected virtual void DiscountSequence_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(this, ((DiscountSequence)e.Row).DiscountID);
			SetControlsState(sender, e.Row as DiscountSequence, discount);
			SetGridColumnsState(discount);

            if (discount != null && discount.Type != DiscountType.Group)
            {
                Dictionary<string, string> allowed = new DiscountOption.ListAttribute().ValueLabelDic;
                allowed.Remove(DiscountOption.FreeItem);
                PXStringListAttribute.SetList<DiscountSequence.discountedFor>(sender, e.Row,
                                                               allowed.Keys.ToArray(),
                                                               allowed.Values.ToArray());
            }
		}

        protected virtual void DiscountSequence_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            DiscountSequence newRow = e.NewRow as DiscountSequence;
            if (newRow != null && newRow.IsActive == true && newRow.DiscountedFor == DiscountOption.FreeItem && newRow.FreeItemID == null)
            {
                newRow.IsActive = false;

                if (newRow.IsPromotion == true)
                {
                    CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.freeItemID>(e.NewRow, newRow.FreeItemID,
                        new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmpty, PXErrorLevel.Warning));
                    CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.isActive>(e.NewRow, newRow.IsActive,
                        new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmpty, PXErrorLevel.Warning));
                }
                else
                {
                    CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.freeItemID>(e.NewRow, newRow.FreeItemID,
                        new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmptyPending, PXErrorLevel.Warning));
                    CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.isActive>(e.NewRow, newRow.IsActive,
                        new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmptyPending, PXErrorLevel.Warning));
                }
            }
        }

		protected virtual void DiscountSequence_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			DiscountSequence row = e.Row as DiscountSequence;
			if (row != null)
			{
                if (row.DiscountSequenceID == null)
                {
                    throw new PXRowPersistingException(typeof(DiscountSequence.discountSequenceID).Name, null, ErrorMessages.FieldIsEmpty);
                }
                if (row.IsPromotion == true && row.EndDate == null)
				{
					if (sender.RaiseExceptionHandling<DiscountSequence.endDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty)))
					{
						throw new PXRowPersistingException(typeof(DiscountSequence.endDate).Name, null, ErrorMessages.FieldIsEmpty);
					}
				}
                if (row.IsPromotion == true && row.EndDate != null && row.StartDate != null && row.EndDate < row.StartDate)
                {
					if (sender.RaiseExceptionHandling<DiscountSequence.endDate>(e.Row, row.EndDate, new PXSetPropertyException(AR.Messages.EffectiveDateExpirationDate)))
                    {
						throw new PXRowPersistingException(typeof(DiscountSequence.endDate).Name, row.EndDate, AR.Messages.EffectiveDateExpirationDate);
                    }
                }
                if (row.DiscountedFor == DiscountOption.FreeItem && row.IsActive == true && row.FreeItemID == null)
                {
                    row.IsActive = false;
                    if (row.IsPromotion == true)
                        CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.freeItemID>(e.Row, row.FreeItemID,
                            new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmpty, PXErrorLevel.Error));
                    else
                        CurrentSequence.Cache.RaiseExceptionHandling<DiscountSequence.freeItemID>(e.Row, row.FreeItemID,
                            new PXSetPropertyException(AR.Messages.FreeItemMayNotBeEmptyPending, PXErrorLevel.Error));
                }
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				ARDiscount discount = Discount.Current;
				if (discount != null)
				{
					PXDBDefaultAttribute.SetDefaultForInsert<DiscountSequence.discountSequenceID>(sender, e.Row, discount.IsAutoNumber == true);
				}
			}
		}

		protected virtual void DiscountSequence_DiscountSequenceID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<DiscountSequence.discountID>>>>.Select(this, ((DiscountSequence)e.Row).DiscountID);
				if (discount != null && discount.IsAutoNumber == true)
				{
					e.NewValue = NewKey;
				}
			}
		}
			
        //RowUpdated event handlers
		protected virtual void DiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			DiscountDetail row = e.Row as DiscountDetail;
			if (row != null)
			{
				if (!sender.ObjectsEqual<DiscountDetail.pendingQuantity, DiscountDetail.pendingAmount, DiscountDetail.pendingDiscountPercent, DiscountDetail.pendingFreeItemQty>(e.Row, e.OldRow))
				{
					if (row.StartDate == null)
					{
						if (Sequence.Current != null && Sequence.Current.StartDate != null)
							row.StartDate = Sequence.Current.StartDate;
						else
							row.StartDate = Accessinfo.BusinessDate;
					}
				}
			}
		}

        protected virtual void DiscountDetail_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            DiscountDetail row = e.Row as DiscountDetail;
            if (row != null)
            {
                PXResult<DiscountSequenceDetail> LastDiscountDetailLine = PXSelect<DiscountSequenceDetail, Where<DiscountSequenceDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                    And<DiscountSequenceDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountSequenceDetail.discountDetailsID, NotEqual<Required<DiscountSequenceDetail.discountDetailsID>>, 
                    And<DiscountSequenceDetail.lineNbr, Equal<Required<DiscountSequenceDetail.lineNbr>>, And<DiscountSequenceDetail.isLast, Equal<True>>>>>>, OrderBy<Asc<DiscountSequenceDetail.quantity>>>.SelectSingleBound(this, null, row.DiscountDetailsID, row.LineNbr);
                if (LastDiscountDetailLine != null)
                    SequenceDetails.Delete(LastDiscountDetailLine);
            }
        }

        protected virtual void DiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            DiscountDetail row = e.Row as DiscountDetail;
            if (row != null)
            {
                if (Sequence.Current.BreakBy == BreakdownType.Quantity)
                {
                    PXResult<DiscountDetail> nextDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                    And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.quantity, Greater<Required<DiscountDetail.quantity>>>>>, OrderBy<Asc<DiscountDetail.quantity>>>.SelectSingleBound(this, null, row.Quantity);
                    if (nextDetailLine == null)
                    {
                        PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                        And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.quantity, Less<Required<DiscountDetail.quantity>>>>>, OrderBy<Desc<DiscountDetail.quantity>>>.SelectSingleBound(this, null, row.Quantity);
                        if (prevDetailLine != null)
                        {
                            DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                            prevLine.QuantityTo = null;
                            Details.Update(prevLine);
                        }
                    }
                    else
                    {
                        PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                        And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.quantity, Less<Required<DiscountDetail.quantity>>>>>, OrderBy<Desc<DiscountDetail.quantity>>>.SelectSingleBound(this, null, row.Quantity);
                        if (prevDetailLine != null)
                        {
                            DiscountDetail nextLine = (DiscountDetail)nextDetailLine;
                            DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                            prevLine.QuantityTo = nextLine.Quantity;
                            Details.Update(prevLine);
                        }
                    }
                }
                else
                {
                    PXResult<DiscountDetail> nextDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                    And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.amount, Greater<Required<DiscountDetail.amount>>>>>, OrderBy<Asc<DiscountDetail.amount>>>.SelectSingleBound(this, null, row.Amount);
                    if (nextDetailLine == null)
                    {
                        PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                        And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.amount, Less<Required<DiscountDetail.amount>>>>>, OrderBy<Desc<DiscountDetail.amount>>>.SelectSingleBound(this, null, row.Amount);
                        if (prevDetailLine != null)
                        {
                            DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                            prevLine.AmountTo = null;
                            Details.Update(prevLine);
                        }
                    }
                    else
                    {
                        PXResult<DiscountDetail> prevDetailLine = PXSelect<DiscountDetail, Where<DiscountDetail.discountID, Equal<Current<DiscountSequence.discountID>>,
                        And<DiscountDetail.discountSequenceID, Equal<Current<DiscountSequence.discountSequenceID>>, And<DiscountDetail.amount, Less<Required<DiscountDetail.amount>>>>>, OrderBy<Desc<DiscountDetail.amount>>>.SelectSingleBound(this, null, row.Amount);
                        if (prevDetailLine != null)
                        {
                            DiscountDetail nextLine = (DiscountDetail)nextDetailLine;
                            DiscountDetail prevLine = (DiscountDetail)prevDetailLine;
                            prevLine.AmountTo = nextLine.Amount;
                            Details.Update(prevLine);
                        }
                    }
                }
            }
        }

       	#endregion

        #region Validation
        
        private bool RunVerification()
        {
            if (Discount.Current != null && Sequence.Current != null && Sequence.Current.IsActive == true)
            {
                switch (Discount.Current.ApplicableTo)
                {
					case DiscountTarget.Unconditional:
                		return VerifyUnconditional();
                    case DiscountTarget.Customer:
                        return VerifyCustomer();
                    case DiscountTarget.Inventory:
                        return VerifyItem();
                    case DiscountTarget.CustomerPrice:
                        return VerifyCustomerPriceClass();
                    case DiscountTarget.InventoryPrice:
                        return VerifyInventoryPriceClass();
                    case DiscountTarget.CustomerAndInventory:
                        return VerifyCombination_Customer_Inventory();
                    case DiscountTarget.CustomerAndInventoryPrice:
                        return VerifyCombination_Customer_InventoryPriceClass();
                    case DiscountTarget.CustomerPriceAndInventory:
                        return VerifyCombination_CustomerPriceClass_Inventory();
                    case DiscountTarget.CustomerPriceAndInventoryPrice:
                        return VerifyCombination_CustomerPriceClass_InventoryPriceClass();
                    case DiscountTarget.Branch:
                        return VerifyBranch();
                    case DiscountTarget.Warehouse:
                        return VerifyWarehouse();
                    case DiscountTarget.WarehouseAndCustomer:
                        return VerifyCombination_Warehouse_Customer();
                    case DiscountTarget.WarehouseAndCustomerPrice:
                        return VerifyCombination_Warehouse_CustomerPriceClass();
                    case DiscountTarget.WarehouseAndInventory:
                        return VerifyCombination_Warehouse_Inventory();
                    case DiscountTarget.WarehouseAndInventoryPrice:
                        return VerifyCombination_Warehouse_InventoryPriceClass();
                }
            }

            return true;
        }

		private bool VerifyUnconditional()
		{
			bool success = true;

			if (!IsUncoditionalValid())
			{
				success = false;
				Sequence.Cache.RaiseExceptionHandling<DiscountSequence.discountSequenceID>(Sequence.Current, Sequence.Current.DiscountSequenceID, new PXSetPropertyException(Messages.UnconditionalDiscUniqueConstraint, PXErrorLevel.Error));
			}

			return success;
		}

		private bool IsUncoditionalValid()
		{
			//Check for duplicates in other sequences that overlap with current:
			DiscountSequence existing = null;

			if (Sequence.Current.IsPromotion == true)
			{
				#region Search duplicates in promotional sequences

				existing = PXSelectReadonly<DiscountSequence,
									Where<DiscountSequence.isActive, Equal<True>,
									And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
									And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>,
									And<DiscountSequence.isPromotion, Equal<True>,
										And<Where2<Where<
												DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
												Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
										>>>>>.SelectWindowed(this, 0, 1);

				if (existing != null)
				{
					return false;
				}

				#endregion
			}
			else
			{
				#region Search duplicates in NON promotional sequences

				existing = PXSelectReadonly<DiscountSequence,
										Where<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
										And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>,
										And<DiscountSequence.isPromotion, Equal<False>>>>>>.SelectWindowed(this, 0, 1);

				if (existing != null)
				{
					return false;
				}

				#endregion
			}

			return true;

		}

        private bool VerifyCustomer()
        {
            bool success = true;

            foreach (PXResult<DiscountCustomer, Customer> row in Customers.Select())
            {
                if (Customers.Cache.GetStatus((DiscountCustomer)row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyCustomer(((DiscountCustomer)row).CustomerID))
                    {
                        success = false;
                        Customers.Cache.RaiseExceptionHandling<DiscountCustomer.customerID>((DiscountCustomer)row, ((Customer)row).AcctCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyCustomer(int? customerID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountCustomer existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountCustomer,
                                                InnerJoin<DiscountSequence, On<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                                Where<DiscountCustomer.discountID, Equal<Current<DiscountSequence.discountID>>,
                                                And<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                                                And<DiscountSequence.isActive, Equal<True>,
                                                And<DiscountSequence.isPromotion, Equal<True>,
												And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomer,
                                        InnerJoin<DiscountSequence, On<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountCustomer.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, customerID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyItem()
        {
            bool success = true;

            foreach (PXResult<DiscountItem, InventoryItem> row in Items.Select())
            {
                if (Items.Cache.GetStatus((DiscountItem)row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyItem(((DiscountItem)row).InventoryID))
                    {
                        success = false;
                        Items.Cache.RaiseExceptionHandling<DiscountItem.inventoryID>((DiscountItem)row, ((InventoryItem)row).InventoryCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyItem(int? inventoryID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountItem existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountItem,
                                        InnerJoin<DiscountSequence, On<DiscountItem.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountItem.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<True>,
                                        And<Where2<Where<
                                                DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                        >>>>>>.SelectWindowed(this, 0, 1, inventoryID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountItem,
                                        InnerJoin<DiscountSequence, On<DiscountItem.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountItem.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, inventoryID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCustomerPriceClass()
        {
            bool success = true;

            foreach (DiscountCustomerPriceClass row in CustomerPriceClasses.Select())
            {
                if (CustomerPriceClasses.Cache.GetStatus(row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyCustomerPriceClass(row.CustomerPriceClassID))
                    {
                        success = false;
                        CustomerPriceClasses.Cache.RaiseExceptionHandling<DiscountCustomerPriceClass.customerPriceClassID>(row, row.CustomerPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyCustomerPriceClass(string priceClassID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountCustomerPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                                                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                                Where<DiscountCustomerPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
                                                And<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                                                And<DiscountSequence.isActive, Equal<True>,
												And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, priceClassID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                                        InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountCustomerPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, priceClassID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyInventoryPriceClass()
        {
            bool success = true;

            foreach (DiscountInventoryPriceClass row in InventoryPriceClasses.Select())
            {
                if (InventoryPriceClasses.Cache.GetStatus(row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyInventoryPriceClass(row.InventoryPriceClassID))
                    {
                        success = false;
                        InventoryPriceClasses.Cache.RaiseExceptionHandling<DiscountInventoryPriceClass.inventoryPriceClassID>(row, row.InventoryPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyInventoryPriceClass(string priceClassID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountInventoryPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountInventoryPriceClass,
                                        InnerJoin<DiscountSequence, On<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountInventoryPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<True>,
                                        And<Where2<Where<
                                                DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                        >>>>>>.SelectWindowed(this, 0, 1, priceClassID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountInventoryPriceClass,
                                        InnerJoin<DiscountSequence, On<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountInventoryPriceClass.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
										And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, priceClassID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyBranch()
        {
            bool success = true;

            foreach (PXResult<DiscountBranch, Branch> row in Branches.Select())
            {
                if (Branches.Cache.GetStatus((DiscountBranch)row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyBranch(((DiscountBranch)row).BranchID))
                    {
                        success = false;
                        Branches.Cache.RaiseExceptionHandling<DiscountBranch.branchID>((DiscountBranch)row, ((Branch)row).BranchCD, new PXSetPropertyException(Messages.UniqueBranchConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyBranch(int? branchID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountBranch existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountBranch,
                                        InnerJoin<DiscountSequence, On<DiscountBranch.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountBranch.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountBranch.branchID, Equal<Required<DiscountBranch.branchID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
                                        And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<True>,
                                        And<Where2<Where<
                                                DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                        >>>>>>.SelectWindowed(this, 0, 1, branchID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountBranch,
                                        InnerJoin<DiscountSequence, On<DiscountBranch.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountBranch.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountBranch.branchID, Equal<Required<DiscountBranch.branchID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
                                        And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, branchID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyWarehouse()
        {
            bool success = true;

            foreach (PXResult<DiscountSite, INSite> row in Sites.Select())
            {
                if (Sites.Cache.GetStatus((DiscountSite)row) != PXEntryStatus.Deleted)
                {
                    if (!VerifyWarehouse(((DiscountSite)row).SiteID))
                    {
                        success = false;
                        Sites.Cache.RaiseExceptionHandling<DiscountSite.siteID>((DiscountSite)row, ((INSite)row).SiteCD, new PXSetPropertyException(Messages.UniqueWarehouseConstraint, PXErrorLevel.Error));
                    }
                }
            }

            return success;
        }

        private bool VerifyWarehouse(int? siteID)
        {
            //Check for duplicates in other sequences that overlap with current:
            DiscountSite existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences

                existing = PXSelectReadonly2<DiscountSite,
                                        InnerJoin<DiscountSequence, On<DiscountSite.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountSite.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
                                        And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<True>,
                                        And<Where2<Where<
                                                DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                        >>>>>>.SelectWindowed(this, 0, 1, siteID);


                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountSite,
                                        InnerJoin<DiscountSequence, On<DiscountSite.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                                            And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>,
                                        Where<DiscountSite.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                                        And<DiscountSequence.isActive, Equal<True>,
                                        And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                                        And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, siteID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Customer_Inventory()
        {
            bool success = true;

            foreach (PXResult<DiscountItem, InventoryItem> item in Items.Select())
            {
                if (Items.Cache.GetStatus((DiscountItem)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomer, Customer> customer in Customers.Select())
                    {
                        if (Customers.Cache.GetStatus((DiscountCustomer)customer) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Customer_Inventory(((DiscountCustomer)customer).CustomerID, ((DiscountItem)item).InventoryID))
                            {
                                success = false;
                                Customers.Cache.RaiseExceptionHandling<DiscountCustomer.customerID>((DiscountCustomer)customer, ((Customer)customer).AcctCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Items.Cache.RaiseExceptionHandling<DiscountItem.inventoryID>((DiscountItem)item, ((InventoryItem)item).InventoryCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Customer_Inventory(int? customerID, int? itemID)
        {
            DiscountCustomer existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountItem, On<DiscountCustomer.discountID, Equal<DiscountItem.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountItem.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerID, itemID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountItem, On<DiscountCustomer.discountID, Equal<DiscountItem.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountItem.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>>.SelectWindowed(this, 0, 1, customerID, itemID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Customer_InventoryPriceClass()
        {
            bool success = true;

            foreach (PXResult<DiscountInventoryPriceClass, INPriceClass> item in InventoryPriceClasses.Select())
            {
                if (InventoryPriceClasses.Cache.GetStatus((DiscountInventoryPriceClass)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomer, Customer> customer in Customers.Select())
                    {
                        if (Customers.Cache.GetStatus((DiscountCustomer)customer) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Customer_InventoryPriceClass(((DiscountCustomer)customer).CustomerID, ((DiscountInventoryPriceClass)item).InventoryPriceClassID))
                            {
                                success = false;
                                Customers.Cache.RaiseExceptionHandling<DiscountCustomer.customerID>((DiscountCustomer)customer, ((Customer)customer).AcctCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                InventoryPriceClasses.Cache.RaiseExceptionHandling<DiscountInventoryPriceClass.inventoryPriceClassID>((DiscountInventoryPriceClass)item, ((DiscountInventoryPriceClass)item).InventoryPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Customer_InventoryPriceClass(int? customerID, string priceClassID)
        {
            DiscountCustomer existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountInventoryPriceClass, On<DiscountCustomer.discountID, Equal<DiscountInventoryPriceClass.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerID, priceClassID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountInventoryPriceClass, On<DiscountCustomer.discountID, Equal<DiscountInventoryPriceClass.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, customerID, priceClassID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_CustomerPriceClass_Inventory()
        {
            bool success = true;

            foreach (PXResult<DiscountItem, InventoryItem> item in Items.Select())
            {
                if (Items.Cache.GetStatus((DiscountItem)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomerPriceClass, ARPriceClass> cpc in CustomerPriceClasses.Select())
                    {
                        if (CustomerPriceClasses.Cache.GetStatus((DiscountCustomerPriceClass)cpc) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_CustomerPriceClass_Inventory(((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, ((DiscountItem)item).InventoryID))
                            {
                                success = false;
                                CustomerPriceClasses.Cache.RaiseExceptionHandling<DiscountCustomerPriceClass.customerPriceClassID>((DiscountCustomerPriceClass)cpc, ((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Items.Cache.RaiseExceptionHandling<DiscountItem.inventoryID>((DiscountItem)item, ((InventoryItem)item).InventoryCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_CustomerPriceClass_Inventory(string customerPriceClassID, int? itemID)
        {
            DiscountCustomerPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountItem, On<DiscountCustomerPriceClass.discountID, Equal<DiscountItem.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountItem.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, itemID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountItem, On<DiscountCustomerPriceClass.discountID, Equal<DiscountItem.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountItem.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, itemID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_CustomerPriceClass_InventoryPriceClass()
        {
            bool success = true;

            foreach (PXResult<DiscountInventoryPriceClass, INPriceClass> ipc in InventoryPriceClasses.Select())
            {
                if (InventoryPriceClasses.Cache.GetStatus((DiscountInventoryPriceClass)ipc) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomerPriceClass, ARPriceClass> cpc in CustomerPriceClasses.Select())
                    {
                        if (CustomerPriceClasses.Cache.GetStatus((DiscountCustomerPriceClass)cpc) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_CustomerPriceClass_InventoryPriceClass(((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, ((DiscountInventoryPriceClass)ipc).InventoryPriceClassID))
                            {
                                success = false;
                                CustomerPriceClasses.Cache.RaiseExceptionHandling<DiscountCustomerPriceClass.customerPriceClassID>((DiscountCustomerPriceClass)cpc, ((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                InventoryPriceClasses.Cache.RaiseExceptionHandling<DiscountInventoryPriceClass.inventoryPriceClassID>((DiscountInventoryPriceClass)ipc, ((DiscountInventoryPriceClass)ipc).InventoryPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_CustomerPriceClass_InventoryPriceClass(string customerPriceClassID, string priceClassID)
        {
            DiscountCustomerPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountInventoryPriceClass, On<DiscountCustomerPriceClass.discountID, Equal<DiscountInventoryPriceClass.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, priceClassID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountInventoryPriceClass, On<DiscountCustomerPriceClass.discountID, Equal<DiscountInventoryPriceClass.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountInventoryPriceClass.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSequence.isActive, Equal<True>,
				And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, priceClassID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Warehouse_Customer()
        {
            bool success = true;

            foreach (PXResult<DiscountSite, INSite> item in Sites.Select())
            {
                if (Sites.Cache.GetStatus((DiscountSite)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomer, Customer> customer in Customers.Select())
                    {
                        if (Customers.Cache.GetStatus((DiscountCustomer)customer) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Warehouse_Customer(((DiscountCustomer)customer).CustomerID, ((DiscountSite)item).SiteID))
                            {
                                success = false;
                                Customers.Cache.RaiseExceptionHandling<DiscountCustomer.customerID>((DiscountCustomer)customer, ((Customer)customer).AcctCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Sites.Cache.RaiseExceptionHandling<DiscountSite.siteID>((DiscountSite)item, ((INSite)item).SiteCD, new PXSetPropertyException(Messages.UniqueWarehouseConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Warehouse_Customer(int? customerID, int? siteID)
        {
            DiscountCustomer existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountSite, On<DiscountCustomer.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerID, siteID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomer,
                InnerJoin<DiscountSite, On<DiscountCustomer.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomer.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomer.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomer.customerID, Equal<Required<DiscountCustomer.customerID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>>.SelectWindowed(this, 0, 1, customerID, siteID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Warehouse_CustomerPriceClass()
        {
            bool success = true;

            foreach (PXResult<DiscountSite, INSite> item in Sites.Select())
            {
                if (Sites.Cache.GetStatus((DiscountSite)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountCustomerPriceClass, ARPriceClass> cpc in CustomerPriceClasses.Select())
                    {
                        if (CustomerPriceClasses.Cache.GetStatus((DiscountCustomerPriceClass)cpc) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Warehouse_CustomerPriceClass(((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, ((DiscountSite)item).SiteID))
                            {
                                success = false;
                                CustomerPriceClasses.Cache.RaiseExceptionHandling<DiscountCustomerPriceClass.customerPriceClassID>((DiscountCustomerPriceClass)cpc, ((DiscountCustomerPriceClass)cpc).CustomerPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Sites.Cache.RaiseExceptionHandling<DiscountSite.siteID>((DiscountSite)item, ((INSite)item).SiteCD, new PXSetPropertyException(Messages.UniqueWarehouseConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Warehouse_CustomerPriceClass(string customerPriceClassID, int? siteID)
        {
            DiscountCustomerPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountSite, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, siteID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountCustomerPriceClass,
                InnerJoin<DiscountSite, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountCustomerPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountCustomerPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountCustomerPriceClass.customerPriceClassID, Equal<Required<DiscountCustomerPriceClass.customerPriceClassID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, customerPriceClassID, siteID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Warehouse_Inventory()
        {
            bool success = true;

            foreach (PXResult<DiscountSite, INSite> item in Sites.Select())
            {
                if (Sites.Cache.GetStatus((DiscountSite)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountItem, InventoryItem> inv in Items.Select())
                    {
                        if (Items.Cache.GetStatus((DiscountItem)inv) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Warehouse_Inventory(((DiscountItem)inv).InventoryID, ((DiscountSite)item).SiteID))
                            {
                                success = false;
                                Items.Cache.RaiseExceptionHandling<DiscountItem.inventoryID>((DiscountItem)inv, ((InventoryItem)inv).InventoryCD, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Sites.Cache.RaiseExceptionHandling<DiscountSite.siteID>((DiscountSite)item, ((INSite)item).SiteCD, new PXSetPropertyException(Messages.UniqueWarehouseConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Warehouse_Inventory(int? inventoryID, int? siteID)
        {
            DiscountItem existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountItem,
                InnerJoin<DiscountSite, On<DiscountItem.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountItem.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountItem.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountItem.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, inventoryID, siteID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountItem,
                InnerJoin<DiscountSite, On<DiscountItem.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountItem.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountItem.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountItem.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountItem.inventoryID, Equal<Required<DiscountItem.inventoryID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>>.SelectWindowed(this, 0, 1, inventoryID, siteID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        private bool VerifyCombination_Warehouse_InventoryPriceClass()
        {
            bool success = true;

            foreach (PXResult<DiscountSite, INSite> item in Sites.Select())
            {
                if (Sites.Cache.GetStatus((DiscountSite)item) != PXEntryStatus.Deleted)
                {
                    foreach (PXResult<DiscountInventoryPriceClass, INPriceClass> ipc in InventoryPriceClasses.Select())
                    {
                        if (InventoryPriceClasses.Cache.GetStatus((DiscountInventoryPriceClass)ipc) != PXEntryStatus.Deleted)
                        {
                            if (!VerifyCombination_Warehouse_InventoryPriceClass(((DiscountInventoryPriceClass)ipc).InventoryPriceClassID, ((DiscountSite)item).SiteID))
                            {
                                success = false;
                                InventoryPriceClasses.Cache.RaiseExceptionHandling<DiscountInventoryPriceClass.inventoryPriceClassID>((DiscountInventoryPriceClass)ipc, ((DiscountInventoryPriceClass)ipc).InventoryPriceClassID, new PXSetPropertyException(Messages.UniqueItemConstraint, PXErrorLevel.Error));
                                Sites.Cache.RaiseExceptionHandling<DiscountSite.siteID>((DiscountSite)item, ((INSite)item).SiteCD, new PXSetPropertyException(Messages.UniqueWarehouseConstraint, PXErrorLevel.Error));
                            }
                        }
                    }
                }

            }

            return success;
        }

        private bool VerifyCombination_Warehouse_InventoryPriceClass(string inventoryPriceClassID, int? siteID)
        {
            DiscountInventoryPriceClass existing = null;

            if (Sequence.Current.IsPromotion == true)
            {
                #region Search duplicates in promotional sequences
                existing = PXSelectReadonly2<DiscountInventoryPriceClass,
                InnerJoin<DiscountSite, On<DiscountInventoryPriceClass.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountInventoryPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<True>,
                                                And<Where2<Where<
                                                        DiscountSequence.startDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>,
                                                        Or<DiscountSequence.endDate, Between<Current<DiscountSequence.startDate>, Current<DiscountSequence.endDate>>>>>
                                                >>>>>>.SelectWindowed(this, 0, 1, inventoryPriceClassID, siteID);
                if (existing != null)
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region Search duplicates in NON promotional sequences

                existing = PXSelectReadonly2<DiscountInventoryPriceClass,
                InnerJoin<DiscountSite, On<DiscountInventoryPriceClass.discountID, Equal<DiscountSite.discountID>,
                    And<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSite.discountSequenceID>>>,
                InnerJoin<DiscountSequence, On<DiscountInventoryPriceClass.discountID, Equal<DiscountSequence.discountID>,
                    And<DiscountInventoryPriceClass.discountSequenceID, Equal<DiscountSequence.discountSequenceID>,
                    And<DiscountSequence.discountSequenceID, NotEqual<Current<DiscountSequence.discountSequenceID>>>>>>>,

                Where<DiscountInventoryPriceClass.inventoryPriceClassID, Equal<Required<DiscountInventoryPriceClass.inventoryPriceClassID>>,
                And<DiscountSite.siteID, Equal<Required<DiscountSite.siteID>>,
                And<DiscountSequence.isActive, Equal<True>,
                And<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>,
                And<DiscountSequence.isPromotion, Equal<False>>>>>>>.SelectWindowed(this, 0, 1, inventoryPriceClassID, siteID);

                if (existing != null)
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

		#endregion

		private void SetControlsState(PXCache sender, DiscountSequence row, ARDiscount discount)
		{
			if (row != null)
			{
                updateDiscounts.SetEnabled(!(sender.GetStatus(row) == PXEntryStatus.Inserted && discount != null && discount.IsAutoNumber == true) && row.IsPromotion != true);

                PXUIFieldAttribute.SetEnabled<DiscountSequence.endDate>(sender, row, row.IsPromotion == true);
				PXUIFieldAttribute.SetRequired<DiscountSequence.endDate>(sender, row.IsPromotion == true);
				PXUIFieldAttribute.SetRequired<DiscountSequence.startDate>(sender, row.IsPromotion == true);
                PXUIFieldAttribute.SetVisible<DiscountSequence.startDate>(sender, row, true);
                PXUIFieldAttribute.SetVisible<DiscountSequence.startDate>(sender, row, row.IsPromotion == true);
				PXUIFieldAttribute.SetVisible<DiscountSequence.endDate>(sender, row, row.IsPromotion == true);

				PXUIFieldAttribute.SetVisible<DiscountSequence.updateDate>(sender, row, row.IsPromotion == false);

                PXUIFieldAttribute.SetEnabled<DiscountSequence.breakBy>(sender, row, (discount != null && (discount.Type == DiscountType.Group || discount.Type == DiscountType.Line)));
                PXUIFieldAttribute.SetEnabled<DiscountSequence.pendingFreeItemID>(sender, row, row.DiscountedFor == DiscountOption.FreeItem && IsFreeItemApplicable(row.DiscountID));

                PXUIFieldAttribute.SetEnabled<DiscountSequence.freeItemID>(sender, row, row.IsPromotion == true);

                PXUIFieldAttribute.SetEnabled<DiscountSequence.prorate>(sender, row, row.DiscountedFor == DiscountOption.FreeItem || row.DiscountedFor == DiscountOption.Amount);
				PXUIFieldAttribute.SetVisible<DiscountSequence.pendingFreeItemID>(sender, row, row.IsPromotion != true);
				PXUIFieldAttribute.SetVisible<DiscountSequence.lastFreeItemID>(sender, row, row.IsPromotion != true);
			}
		}

		private void SetGridColumnsState(ARDiscount discount)
		{
            if (Sequence.Current != null)
            {
                #region Show All Columns
                PXUIFieldAttribute.SetVisible<DiscountDetail.amountTo>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.amount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.pendingAmount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastAmount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.quantityTo>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.quantity>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.pendingQuantity>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastQuantity>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.discount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscount>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.discountPercent>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscountPercent>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscountPercent>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.freeItemQty>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.pendingFreeItemQty>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastFreeItemQty>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.startDate>(Details.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountDetail.lastDate>(Details.Cache, null, true);

                PXUIFieldAttribute.SetVisible<DiscountItem.amount>(Items.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountItem.quantity>(Items.Cache, null, true);
                PXUIFieldAttribute.SetVisible<DiscountItem.uOM>(Items.Cache, null, true);

                #endregion

                #region Hide Selective Columns

                if (Sequence.Current.DiscountedFor != DiscountOption.FreeItem)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.freeItemQty>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastFreeItemQty>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingFreeItemQty>(Details.Cache, null, false);
                }
                if (Sequence.Current.BreakBy == BreakdownType.Quantity)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.amount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.amountTo>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingAmount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastAmount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountItem.amount>(Items.Cache, null, false);
                }
                else
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.quantity>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.quantityTo>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingQuantity>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastQuantity>(Details.Cache, null, false);

                    PXUIFieldAttribute.SetVisible<DiscountItem.quantity>(Items.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountItem.uOM>(Items.Cache, null, false);
                }

                if (Sequence.Current.IsPromotion == true)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastAmount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastFreeItemQty>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastQuantity>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingAmount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingFreeItemQty>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingQuantity>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.startDate>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDate>(Details.Cache, null, false);

                }

                if (Sequence.Current.DiscountedFor == DiscountOption.Amount)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.discountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscountPercent>(Details.Cache, null, false);
                }
                if (Sequence.Current.DiscountedFor == DiscountOption.Percent)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.discount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscount>(Details.Cache, null, false);
                }
                if (Sequence.Current.DiscountedFor == DiscountOption.FreeItem)
                {
                    PXUIFieldAttribute.SetVisible<DiscountDetail.discount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscount>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.discountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.lastDiscountPercent>(Details.Cache, null, false);
                    PXUIFieldAttribute.SetVisible<DiscountDetail.pendingDiscountPercent>(Details.Cache, null, false);
                }

                #endregion

                #region Enable Columns

                if (Sequence.Current.IsPromotion == true)
                {
                    PXUIFieldAttribute.SetEnabled<DiscountDetail.amount>(Details.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<DiscountDetail.quantity>(Details.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<DiscountDetail.discount>(Details.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<DiscountDetail.discountPercent>(Details.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<DiscountDetail.freeItemQty>(Details.Cache, null, true);
                }

                #endregion
            }
		}

		private bool IsFreeItemApplicable(string discountID)
		{
			ARDiscount discount = PXSelect<ARDiscount, Where<ARDiscount.discountID, Equal<Required<ARDiscount.discountID>>>>.Select(this, discountID);

			if (discount == null)
				return true;
			else
			{
                //Free items are valid for group discounts only for now
                if (discount.Type == DiscountType.Group)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
			
		//TODO: reuse NetTools
		private static string IncNumber(string str, int count)
		{
			int i;
			bool j = true;
			int intcount = count;

			StringBuilder bld = new StringBuilder();
			for (i = str.Length; i > 0; i--)
			{
				string c = str.Substring(i - 1, 1);

				if (System.Text.RegularExpressions.Regex.IsMatch(c, "[^0-9]"))
				{
					j = false;
				}

				if (j && System.Text.RegularExpressions.Regex.IsMatch(c, "[0-9]"))
				{
					int digit = Convert.ToInt16(c);

					string s_count = Convert.ToString(intcount);
					int digit2 = Convert.ToInt16(s_count.Substring(s_count.Length - 1, 1));

					bld.Append((digit + digit2) % 10);

					intcount -= digit2;
					intcount += ((digit + digit2) - (digit + digit2) % 10);

					intcount /= 10;

					if (intcount == 0)
					{
						j = false;
					}
				}
				else
				{
					bld.Append(c);
				}
			}

			if (intcount != 0)
			{
				throw new ArithmeticException("");
			}

			char[] chars = bld.ToString().ToCharArray();
			Array.Reverse(chars);
			return new string(chars);
		}

        public override void Persist()
        {
            if (!RunVerification())
            {
                throw new PXException(Messages.DiscountsNotvalid);
            }

            ARDiscountEx discount = PXSelect<ARDiscountEx, Where<ARDiscountEx.discountID, Equal<Current<DiscountSequence.discountID>>>>.Select(this);

            if (discount != null && Sequence.Current != null)
            {
                if (discount.IsAutoNumber == true && Sequence.Cache.GetStatus(Sequence.Current) == PXEntryStatus.Inserted)
                {
                    string lastNumber = string.IsNullOrEmpty(discount.LastNumber) ? string.Format("{0}0000", discount.DiscountID) : discount.LastNumber;

                    if (!char.IsDigit(lastNumber[lastNumber.Length - 1]))
                    {
                        lastNumber = string.Format("{0}0000", lastNumber);
                    }

                    discount.LastNumber = IncNumber(lastNumber, 1);
                    Discount.Update(discount);
                }
            }

            base.Persist();
        }

		#region Local Types

        [Serializable]
        public partial class ARDiscountEx : ARDiscount
        {
            #region DiscountID
            public new abstract class discountID : PX.Data.IBqlField
            {
            }
            #endregion

            #region Tab VisibleExp Support

            #region ShowListOfItems
            public abstract class showListOfItems : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowListOfItems
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.Inventory
                    || this.ApplicableTo == DiscountTarget.CustomerAndInventory
                    || this.ApplicableTo == DiscountTarget.CustomerPriceAndInventory
                    || this.ApplicableTo == DiscountTarget.WarehouseAndInventory;
                }
                set
                {
                }
            }
            #endregion

            #region ShowCustomers
            public abstract class showCustomers : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowCustomers
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.Customer
                        || this.ApplicableTo == DiscountTarget.CustomerAndInventory
                        || this.ApplicableTo == DiscountTarget.CustomerAndInventoryPrice
                        || this.ApplicableTo == DiscountTarget.WarehouseAndCustomer
                        || this.ApplicableTo == DiscountTarget.CustomerAndBranch;
                }
                set
                {
                }
            }
            #endregion

            #region ShowCustomerPriceClass
            public abstract class showCustomerPriceClass : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowCustomerPriceClass
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.CustomerPrice
                        || this.ApplicableTo == DiscountTarget.CustomerPriceAndInventory
                        || this.ApplicableTo == DiscountTarget.CustomerPriceAndInventoryPrice
                        || this.ApplicableTo == DiscountTarget.WarehouseAndCustomerPrice
                        || this.ApplicableTo == DiscountTarget.CustomerPriceAndBranch;
                }
                set
                {
                }
            }
            #endregion

            #region ShowInventoryPriceClass
            public abstract class showInventoryPriceClass : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowInventoryPriceClass
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.InventoryPrice
                        || this.ApplicableTo == DiscountTarget.CustomerAndInventoryPrice
                        || this.ApplicableTo == DiscountTarget.CustomerPriceAndInventoryPrice
                        || this.ApplicableTo == DiscountTarget.WarehouseAndInventoryPrice;

                }
                set
                {
                }
            }
            #endregion

            #region ShowBranches
            public abstract class showBranches : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowBranches
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.Branch
                        || this.ApplicableTo == DiscountTarget.CustomerAndBranch
                        || this.ApplicableTo == DiscountTarget.CustomerPriceAndBranch;
                }
                set
                {
                }
            }
            #endregion

            #region ShowSites
            public abstract class showSites : PX.Data.IBqlField
            {
            }

            [PXBool()]
            [PXUIField(Visibility = PXUIVisibility.Visible)]
            public virtual Boolean? ShowSites
            {
                [PXDependsOnFields(typeof(applicableTo))]
                get
                {
                    return this.ApplicableTo == DiscountTarget.Warehouse
                        || this.ApplicableTo == DiscountTarget.WarehouseAndCustomer
                        || this.ApplicableTo == DiscountTarget.WarehouseAndCustomerPrice
                        || this.ApplicableTo == DiscountTarget.WarehouseAndInventory
                        || this.ApplicableTo == DiscountTarget.WarehouseAndInventoryPrice;
                }
                set
                {
                }
            }
            #endregion
            #endregion
        }
		
		[Serializable]
		public partial class UpdateSettingsFilter : IBqlTable
		{
			#region FilterDate
			public abstract class filterDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _FilterDate;
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXDate()]
			[PXUIField(DisplayName = "Filter Date", Required = true)]
			public virtual DateTime? FilterDate
			{
				get
				{
					return this._FilterDate;
				}
				set
				{
					this._FilterDate = value;
				}
			}
			#endregion
		}

		
		#endregion
	}
}
