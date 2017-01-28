using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;

namespace PX.Objects.IN
{
	public class INLotSerClassMaint : PXGraph<INLotSerClassMaint, INLotSerClass>
	{
		public PXSelect<INLotSerClass> lotserclass;
		public PXSelect<INLotSerSegment, Where<INLotSerSegment.lotSerClassID, Equal<Current<INLotSerClass.lotSerClassID>>>> lotsersegments;

		private void SetParentChanged(PXCache sender, object Row)
		{
			object parent = PXParentAttribute.SelectParent(sender, Row);

			if (parent != null && lotserclass.Cache.GetStatus(parent) == PXEntryStatus.Notchanged)
			{
				lotserclass.Cache.SetStatus(parent, PXEntryStatus.Updated);
			}
		}

		protected virtual void INLotSerClass_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			INLotSerClass row = (INLotSerClass) e.NewRow;
            INLotSerClass oldrow = (INLotSerClass)e.Row;
            if (oldrow != null && row.LotSerAssign != oldrow.LotSerAssign)
                HasInventoryItemsInUse(row);
            if (row.LotSerTrackExpiration != true && row.LotSerIssueMethod == INLotSerIssueMethod.Expiration 
                && row.LotSerTrack != INLotSerTrack.NotNumbered && row.LotSerAssign != INLotSerAssign.WhenUsed)
			{
                throw new PXSetPropertyException(Messages.LotSerTrackExpirationInvalid, typeof(INLotSerClass.lotSerIssueMethod).Name);
			}
		}

        protected virtual void HasInventoryItemsInUse(INLotSerClass item)
        {
            foreach (InventoryItem r in PXSelect<InventoryItem,
                Where<InventoryItem.lotSerClassID, Equal<Required<INLotSerClass.lotSerClassID>>>>.SelectSingleBound(this, null, item.LotSerClassID))
            {
                throw new PXSetPropertyException(Messages.LotSerAssignCannotBeChanged, r.InventoryCD);
            }
        }

		protected virtual void INLotSerClass_LotSerTrack_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INLotSerClass row = (INLotSerClass)e.Row;
			if (row != null && row.LotSerTrack == INLotSerTrack.NotNumbered)
			{
				row.RequiredForDropship = false;
				foreach (INLotSerSegment segment in this.lotsersegments.Select())
					this.lotsersegments.Delete(segment);
			}
			if (row.LotSerTrack != INLotSerTrack.SerialNumbered || row.AutoNextNbr != true)
			{
				row.AutoSerialMaxCount = 0;
			}

		}
		
		protected virtual void INLotSerClass_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INLotSerClass row = (INLotSerClass)e.Row;
			if (row != null)
			{				
				INItemClass classUse =
					PXSelect<INItemClass, Where<INItemClass.lotSerClassID, Equal<Required<INItemClass.lotSerClassID>>>>.SelectWindowed(
						this, 0, 1, row.LotSerClassID);
				InventoryItem itemUse =
					PXSelect<InventoryItem, Where<InventoryItem.lotSerClassID, Equal<Required<INItemClass.lotSerClassID>>>>.
						SelectWindowed(this, 0, 1, row.LotSerClassID);

				PXUIFieldAttribute.SetEnabled<INLotSerClass.lotSerTrack>(sender, row, classUse == null && itemUse == null);
				PXUIFieldAttribute.SetEnabled<INLotSerClass.lotSerTrackExpiration>(sender, row, classUse == null && itemUse == null);

				bool enable = row.LotSerTrack != INLotSerTrack.NotNumbered;

				this.lotsersegments.Cache.AllowInsert = enable;
				this.lotsersegments.Cache.AllowUpdate = enable;
				this.lotsersegments.Cache.AllowDelete = enable;

				PXUIFieldAttribute.SetEnabled<INLotSerClass.lotSerNumVal>(sender, row, enable && row.LotSerNumShared == true);
				PXUIFieldAttribute.SetEnabled<INLotSerClass.autoNextNbr>(sender, row, enable);
				PXUIFieldAttribute.SetEnabled<INLotSerClass.lotSerNumShared>(sender, row, enable);
				PXUIFieldAttribute.SetEnabled<INLotSerClass.requiredForDropship>(sender, row, row.LotSerTrack != INLotSerTrack.NotNumbered);
				PXUIFieldAttribute.SetEnabled<INLotSerClass.autoSerialMaxCount>(sender, row, enable && row.AutoNextNbr == true && row.LotSerTrack == INLotSerTrack.SerialNumbered);
				PXUIFieldAttribute.SetVisible<INLotSerClass.lotSerIssueMethod>(sender, row, row.LotSerAssign == INLotSerAssign.WhenReceived);
			}			
		}

		protected virtual void INLotSerSegment_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SetParentChanged(sender, e.Row);
		}
		
		protected virtual void INLotSerSegment_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SetParentChanged(sender, e.Row);
		}

		protected virtual void INLotSerSegment_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			SetParentChanged(sender, e.Row);
		}
		protected virtual void INLotSerSegment_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INLotSerSegment row = e.Row as INLotSerSegment;
			if (row == null) return;
			if (row.SegmentType == INLotSerSegmentType.NumericVal)
				row.SegmentValue = this.lotserclass.Current.LotSerNumVal;
			PXUIFieldAttribute.SetEnabled<INLotSerSegment.segmentValue>(sender, row,
				row.SegmentType == INLotSerSegmentType.FixedConst || row.SegmentType == INLotSerSegmentType.DateConst);
		}

		protected virtual void INLotSerClass_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			INLotSerClass cls = (INLotSerClass) e.Row;
			if(cls == null) return;

			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (cls.LotSerTrack != INLotSerTrack.NotNumbered && cls.LotSerNumShared == true && string.IsNullOrEmpty(cls.LotSerNumVal))
				{
					sender.RaiseExceptionHandling<INLotSerClass.lotSerNumVal>(cls, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof (INLotSerClass.lotSerNumVal).Name));
				}
			}
		}

		public override void Persist()
		{
			foreach (INLotSerClass lsclass in lotserclass.Cache.Inserted)
			{
				lsclass.LotSerFormatStr = INLotSerialNbrAttribute.MakeFormatStr(lotserclass.Cache, lsclass);

                if (lsclass.LotSerTrack != INLotSerTrack.NotNumbered && lsclass.AutoNextNbr == true)
				{
					int NumbericCount = 0;
					foreach (INLotSerSegment lssegment in lotsersegments.View.SelectMultiBound(new object[] { lsclass }))
					{
						if (lssegment.SegmentType == INLotSerSegmentType.NumericVal)
						{
							NumbericCount++;
						}
					}

					if (NumbericCount == 0)
					{
						throw new PXException(Messages.NumericLotSerSegmentNotExists, Messages.NumericVal);
					}
					else if (NumbericCount > 1)
					{
						throw new PXException(Messages.NumericLotSerSegmentMultiple, Messages.NumericVal);
					}
				}
			}

			foreach (INLotSerClass lsclass in lotserclass.Cache.Updated)
			{
				lsclass.LotSerFormatStr = INLotSerialNbrAttribute.MakeFormatStr(lotserclass.Cache, lsclass);

                if (lsclass.LotSerTrack != INLotSerTrack.NotNumbered && lsclass.AutoNextNbr == true)
				{
					int NumbericCount = 0;
					foreach (INLotSerSegment lssegment in lotsersegments.View.SelectMultiBound(new object[] { lsclass }))
					{
						if (lssegment.SegmentType == INLotSerSegmentType.NumericVal)
						{
							NumbericCount++;
						}
					}

					if (NumbericCount == 0)
					{
						throw new PXException(Messages.NumericLotSerSegmentNotExists, Messages.NumericVal);
					}
					else if (NumbericCount > 1)
					{
						throw new PXException(Messages.NumericLotSerSegmentMultiple, Messages.NumericVal);
					}
				}
			}

			base.Persist();
		}
	}
}
