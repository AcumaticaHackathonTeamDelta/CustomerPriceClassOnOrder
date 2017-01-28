using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CR;
using System.Diagnostics;
using PX.SM;

namespace PX.Objects.IN
{
	public class INItemClassMaint : PXGraph<INItemClassMaint, INItemClass>
	{
		[PXDBString(255)]
        [PXUIField(DisplayName = "Specific Type")]
		[PXStringList(new string[] { "PX.Objects.CS.SegmentValue", "PX.Objects.IN.InventoryItem" },
			new string[] { "Subitem", "Inventory Item Restriction" })]
		protected virtual void RelationGroup_SpecificType_CacheAttached(PXCache sender)
		{
		}

		[PXDefault]
		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDimensionSelector(InventoryAttribute.DimensionName, typeof(InventoryItem.inventoryCD), typeof(InventoryItem.inventoryCD))]
		protected virtual void InventoryItem_InventoryCD_CacheAttached(PXCache sender)
		{
		}

		[PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true)]
		protected virtual void InventoryItem_ItemClassID_CacheAttached(PXCache sender)
		{
		}

		[PXUIField(DisplayName = "Lot/Serial Class")]
		[PXDBString(10, IsUnicode = true)]
		protected virtual void InventoryItem_LotSerClassID_CacheAttached(PXCache sender)
		{
		}

		[PXUIField(DisplayName = "Posting Class")]
		[PXDBString(10, IsUnicode = true)]
		protected virtual void InventoryItem_PostClassID_CacheAttached(PXCache sender)
		{
		}


		[PXHidden]
		public PXSelect<INLotSerClass> lotSerClass;
		public PXSetup<INSetup> inSetup;
		public PXSelect<INItemClass> itemclass;
		public PXSelect<INItemClass, Where<INItemClass.itemClassID, Equal<Current<INItemClass.itemClassID>>>> itemclasssettings; 
		public PXSetup<INLotSerClass, Where<INLotSerClass.lotSerClassID, Equal<Current<INItemClass.lotSerClassID>>>> lotserclass;
		public INUnitSelect2<INUnit, INItemClass.itemClassID, INItemClass.salesUnit, INItemClass.purchaseUnit, INItemClass.baseUnit, INItemClass.lotSerClassID> classunits;
		public PXSelect<INItemClassSubItemSegment, Where<INItemClassSubItemSegment.itemClassID, Equal<Current<INItemClass.itemClassID>>>, OrderBy<Asc<INItemClassSubItemSegment.segmentID>>> segments;
	    public CSAttributeGroupList<INItemClass, InventoryItem> Mapping;
		public PXSelect<INItemClassRep, Where<INItemClassRep.itemClassID, Equal<Current<INItemClass.itemClassID>>>> replenishment;
		public PXSelect<SegmentValue> segmentvalue;
		public PXSelect<PX.SM.RelationGroup> Groups;		
		protected System.Collections.IEnumerable groups()
		{
			foreach (PX.SM.RelationGroup group in PXSelect<PX.SM.RelationGroup>.Select(this))
			{
				if (group.SpecificModule == null || group.SpecificModule == typeof(InventoryItem).Namespace
					|| itemclass.Current != null && PX.SM.UserAccess.IsIncluded(itemclass.Current.GroupMask, group))
				{
					Groups.Current = group;
					yield return group;
				}
			}
		}

		public PXSelect<InventoryItem,
			Where<InventoryItem.itemClassID, Equal<Current<INItemClass.itemClassID>>>> Items;

        public PXAction<INItemClass> viewGroupDetails;
        [PXUIField(DisplayName = Messages.ViewRestrictionGroup, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable ViewGroupDetails(PXAdapter adapter)
        {
            if (Groups.Current != null)
            {
				RelationGroups graph = CreateInstance<RelationGroups>();
                graph.HeaderGroup.Current = graph.HeaderGroup.Search<RelationHeader.groupName>(Groups.Current.GroupName);
                throw new PXRedirectRequiredException(graph, false, Messages.ViewRestrictionGroup);
            }
            return adapter.Get();
        }


		public PXAction<INItemClass> viewRestrictionGroups;
		[PXUIField(DisplayName = GL.Messages.ViewRestrictionGroups, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewRestrictionGroups(PXAdapter adapter)
		{
			if (itemclass.Current != null)
			{
				INAccessDetailByClass graph = CreateInstance<INAccessDetailByClass>();
				graph.Class.Current = graph.Class.Search<INItemClass.itemClassID>(itemclass.Current.ItemClassID);
				throw new PXRedirectRequiredException(graph, false, "Restricted Groups");
			}
			return adapter.Get();
		}
               
		public PXAction<INItemClass> resetGroup;
		[PXProcessButton]
		[PXUIField(DisplayName = Messages.ApplyRestrictionSettings)]
		protected virtual IEnumerable ResetGroup(PXAdapter adapter)
		{
			if (itemclass.Ask(Messages.Warning, CS.Messages.GroupUpdateConfirm, MessageButtons.OKCancel) == WebDialogResult.OK)
			{
				Save.Press();
				string classID = itemclass.Current.ItemClassID;
				PXLongOperation.StartOperation(this, delegate()
				{
					Reset(classID);
				});
			}
			return adapter.Get();
		}
		protected static void Reset(string classID)
		{
			INItemClassMaint graph = PXGraph.CreateInstance<INItemClassMaint>();
			INItemClass itemclass = graph.itemclass.Current = graph.itemclass.Search<INItemClass.itemClassID>(classID);
			if (itemclass != null)
			{
				PXDatabase.Update<InventoryItem>(new PXDataFieldRestrict<InventoryItem.itemClassID>(itemclass.ItemClassID),
					new PXDataFieldAssign<InventoryItem.groupMask>(itemclass.GroupMask));
			}
		}

		public INItemClassMaint()
		{
			PXUIFieldAttribute.SetVisible<INUnit.toUnit>(classunits.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INUnit.toUnit>(classunits.Cache, null, false);

			PXUIFieldAttribute.SetVisible<INUnit.sampleToUnit>(classunits.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<INUnit.sampleToUnit>(classunits.Cache, null, false);

			PXUIFieldAttribute.SetEnabled(Groups.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<PX.SM.RelationGroup.included>(Groups.Cache, null, true);

			if (!PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
			{
				action.SetVisible(false);
				resetGroup.SetVisible(false);
				PXUIFieldAttribute.SetVisible(replenishment.Cache, null, false);
				PXUIFieldAttribute.SetVisible(itemclass.Cache, null, false);
				PXUIFieldAttribute.SetVisible(Groups.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemClass.itemClassID>(itemclass.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemClass.descr>(itemclass.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemClass.itemType>(itemclass.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemClass.taxCategoryID>(itemclass.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemClass.baseUnit>(itemclass.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemClass.priceClassID>(itemclass.Cache, null, true);
			}
		}

		public virtual IEnumerable Segments()
		{
			foreach (INItemClassSubItemSegment seg in segments.Cache.Updated)
			{
				yield return seg;
			}

			if (itemclass.Current == null || string.IsNullOrEmpty(itemclass.Current.ItemClassID))
			{
				yield break;
			}

			foreach (PXResult<Segment, SegmentValue, INItemClassSubItemSegment> res in PXSelectJoin<Segment, LeftJoin<SegmentValue, On<SegmentValue.dimensionID, Equal<Segment.dimensionID>, And<SegmentValue.segmentID, Equal<Segment.segmentID>, And<SegmentValue.isConsolidatedValue, Equal<boolTrue>>>>, LeftJoin<INItemClassSubItemSegment, On<INItemClassSubItemSegment.itemClassID, Equal<Current<INItemClass.itemClassID>>, And<INItemClassSubItemSegment.segmentID, Equal<Segment.segmentID>>>>>, Where<Segment.dimensionID, Equal<SubItemAttribute.dimensionName>>>.Select(this))
			{
				INItemClassSubItemSegment seg = (INItemClassSubItemSegment)res;
				if (seg.SegmentID == null)
				{
					seg.SegmentID = ((Segment)res).SegmentID;
					seg.ItemClassID = itemclass.Current.ItemClassID;
					seg.IsActive = true;
				}

				seg.DefaultValue = ((SegmentValue)res).Value;
				PXUIFieldAttribute.SetEnabled<INItemClassSubItemSegment.defaultValue>(segments.Cache, seg, string.IsNullOrEmpty(seg.DefaultValue));

				INItemClassSubItemSegment cached;

				if ((cached = (INItemClassSubItemSegment)segments.Cache.Locate(seg)) == null || segments.Cache.GetStatus(cached) == PXEntryStatus.Notchanged)
				{
					yield return seg;
				}
			}

			segments.Cache.IsDirty = false;
		}


        #region MyButtons (MMK)
        public PXAction<INItemClass> action;
        [PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Action(PXAdapter adapter)
        {
            return adapter.Get();
        }

        #endregion

        protected virtual void INItemClassSubItemSegment_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				INItemClassSubItemSegment seg = (INItemClassSubItemSegment)e.Row;

				if (seg.IsActive == false && string.IsNullOrEmpty(seg.DefaultValue))
				{
					if (sender.RaiseExceptionHandling<INItemClassSubItemSegment.defaultValue>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(INItemClassSubItemSegment.defaultValue).Name)))
					{
						throw new PXRowPersistingException(typeof(INItemClassSubItemSegment.defaultValue).Name, null, ErrorMessages.FieldIsEmpty, typeof(INItemClassSubItemSegment.defaultValue).Name);
					}
				}

				SegmentValue val = (SegmentValue)PXSelect<SegmentValue, Where<SegmentValue.dimensionID, Equal<SubItemAttribute.dimensionName>, And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>, And<SegmentValue.isConsolidatedValue, Equal<boolTrue>>>>>.Select(this, seg.SegmentID);

				if (val == null)
				{
					val = (SegmentValue)PXSelect<SegmentValue, Where<SegmentValue.dimensionID, Equal<SubItemAttribute.dimensionName>, And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>, And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(this, seg.SegmentID, seg.DefaultValue);

					if (val != null)
					{
						val.IsConsolidatedValue = true;
						segmentvalue.Cache.SetStatus(val, PXEntryStatus.Updated);
						segmentvalue.Cache.IsDirty = true;
					}
				}
			}
		}

		public override int Persist(Type cacheType, PXDBOperation operation)
		{
			if (cacheType == typeof(INUnit) && operation == PXDBOperation.Update)
			{
				base.Persist(cacheType, PXDBOperation.Insert);
			}
			return base.Persist(cacheType, operation);
		}

		protected virtual void INItemClass_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			bool StockItem = (e.Row == null || ((INItemClass)e.Row).StkItem == true);

			INItemTypes.CustomListAttribute stringlist;

			if (StockItem)
			{
				stringlist = new INItemTypes.StockListAttribute();
			}
			else
			{
				stringlist = new INItemTypes.NonStockListAttribute();
			}

			PXStringListAttribute.SetList<INItemClass.itemType>(itemclass.Cache, e.Row, stringlist.AllowedValues, stringlist.AllowedLabels);
		}

		protected virtual void INItemClass_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (e.Row != null && !PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
			{
				sender.SetValueExt<INItemClass.stkItem>(e.Row, false);
			}
		}

		protected virtual void INItemClass_ItemType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				e.NewValue = ((INItemClass)e.Row).StkItem == true ? INItemTypes.FinishedGood : INItemTypes.NonStockItem;
			}
		}

		protected virtual void INItemClass_ValMethod_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				e.NewValue = ((INItemClass)e.Row).StkItem == true ? INValMethod.Average : INValMethod.Standard;
			}
		}

		protected virtual void INItemClass_StkItem_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<INItemClass.itemType>(e.Row);
			sender.SetDefaultExt<INItemClass.valMethod>(e.Row);
		}

		protected virtual void INItemClass_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{

			INSetup iNSetupRec = (INSetup)PXSelect<INSetup, Where<INSetup.dfltItemClassID, Equal<Current<INItemClass.itemClassID>>>>.SelectWindowed(this, 0, 1);
			if (iNSetupRec != null)
			{
				throw new PXException(Messages.ThisItemClassCanNotBeDeletedBecauseItIsUsedIn, "Inventory Setup");
			}

			InventoryItem inventoryItemRec = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.itemClassID, Equal<Current<INItemClass.itemClassID>>>>.SelectWindowed(this, 0, 1);
			if (inventoryItemRec != null)
			{
				throw new PXException(Messages.ThisItemClassCanNotBeDeletedBecauseItIsUsedIn, "Inventory Item: " + inventoryItemRec.InventoryCD);
			}

			INLocation iNLocationRec = (INLocation)PXSelect<INLocation, Where<INLocation.primaryItemClassID, Equal<Current<INItemClass.itemClassID>>>>.SelectWindowed(this, 0, 1);
			if (iNLocationRec != null)
			{
				INSite iNSiteRec = (INSite)PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, iNLocationRec.SiteID);
				throw new PXException(Messages.ThisItemClassCanNotBeDeletedBecauseItIsUsedIn, "Warehouse/Location: " + (iNSiteRec.SiteCD ?? "") + "/" + iNLocationRec.LocationCD);
			}

		}

		protected virtual void INItemClass_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (((INItemClass)e.Row).ValMethod == INValMethod.Specific && lotserclass.Current != null && (lotserclass.Current.LotSerTrack == INLotSerTrack.NotNumbered || lotserclass.Current.LotSerAssign != INLotSerAssign.WhenReceived))
				{
					if (sender.RaiseExceptionHandling<INItemClass.valMethod>(e.Row, INValMethod.Specific, new PXSetPropertyException(Messages.SpecificOnlyNumbered)))
					{
						throw new PXRowPersistingException(typeof(INItemClass.valMethod).Name, INValMethod.Specific, Messages.SpecificOnlyNumbered, typeof(INItemClass.valMethod).Name);
					}
				}
			}
		}

		protected virtual void INItemClassRep_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INItemClassRep row = (INItemClassRep)e.Row;
			if (row != null)
			{
				bool isTransfer = (row.ReplenishmentSource == INReplenishmentSource.Transfer);
				bool isFixedReorder = (row.ReplenishmentMethod == INReplenishmentMethod.FixedReorder);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.replenishmentMethod>(sender, row, row.ReplenishmentSource != INReplenishmentSource.PurchaseToOrder && row.ReplenishmentSource != INReplenishmentSource.DropShipToOrder);
                PXUIFieldAttribute.SetEnabled<INItemClassRep.replenishmentSourceSiteID>(sender, row, row.ReplenishmentSource == INReplenishmentSource.PurchaseToOrder || row.ReplenishmentSource == INReplenishmentSource.DropShipToOrder || row.ReplenishmentSource == INReplenishmentSource.Transfer || row.ReplenishmentSource == INReplenishmentSource.Purchased);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.transferLeadTime>(sender, e.Row, isTransfer);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.transferERQ>(sender, e.Row, isTransfer && isFixedReorder && row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.forecastModelType>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.forecastPeriodType>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.historyDepth>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.launchDate>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.terminationDate>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemClassRep.serviceLevelPct>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXDefaultAttribute.SetPersistingCheck<INItemClassRep.transferLeadTime>(sender, e.Row, isTransfer ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<INItemClassRep.transferERQ>(sender, e.Row, (isTransfer && isFixedReorder) ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			}
		}

		protected virtual void INItemClassRep_ReplenishmentSource_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemClassRep r = e.Row as INItemClassRep;
			if (r == null) return;
			if (r.ReplenishmentSource == INReplenishmentSource.PurchaseToOrder || r.ReplenishmentSource == INReplenishmentSource.DropShipToOrder)
			{
				sender.SetValueExt<INItemClassRep.replenishmentMethod>(r, INReplenishmentMethod.None);
			}
			if (r.ReplenishmentSource != INReplenishmentSource.PurchaseToOrder && r.ReplenishmentSource != INReplenishmentSource.DropShipToOrder && r.ReplenishmentSource != INReplenishmentSource.Transfer)
			{
				sender.SetDefaultExt<INItemClassRep.replenishmentSourceSiteID>(r);
			}
			sender.SetDefaultExt<INItemClassRep.transferLeadTime>(e.Row);	
		
		}


		public override void Persist()
		{
			if (itemclass.Current != null && string.IsNullOrEmpty(itemclass.Current.LotSerClassID) && !PXAccess.FeatureInstalled<FeaturesSet.lotSerialTracking>())
			{
				itemclass.Current.LotSerClassID = INLotSerClass.GetDefaultLotSerClass(this);
			}

			if (itemclass.Current != null && Groups.Cache.IsDirty)
			{
				PX.SM.UserAccess.PopulateNeighbours<INItemClass>(itemclass, Groups, typeof(SegmentValue));
				PXSelectorAttribute.ClearGlobalCache<INItemClass>();
			}
			base.Persist();
			Groups.Cache.Clear();
			GroupHelper.Clear();
		}

		protected virtual void RelationGroup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PX.SM.RelationGroup group = e.Row as PX.SM.RelationGroup;
			if (itemclass.Current != null && group != null && Groups.Cache.GetStatus(group) == PXEntryStatus.Notchanged)
			{
				group.Included = PX.SM.UserAccess.IsIncluded(itemclass.Current.GroupMask, group);
			}
		}

		protected virtual void RelationGroup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

	}

}
