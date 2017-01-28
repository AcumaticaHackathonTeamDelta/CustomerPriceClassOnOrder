using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.SO;
using PX.Objects.GL;
using System.Collections;
using PX.Objects.DR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.SM;
using PX.TM;
using PX.Objects.TX;
using PX.Web.UI;
using ItemStats = PX.Objects.IN.Overrides.INDocumentRelease.ItemStats;
using PX.Objects.AR;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.IN
{
	[PXProjection(typeof(Select4<INSiteStatus, Where<boolTrue, Equal<boolTrue>>, Aggregate<GroupBy<INSiteStatus.inventoryID, GroupBy<INSiteStatus.siteID, Sum<INSiteStatus.qtyOnHand, Sum<INSiteStatus.qtyAvail, Sum<INSiteStatus.qtyNotAvail>>>>>>>))]
    [Serializable]
	[PXCacheName(Messages.INSiteStatusSummary)]
	public partial class INSiteStatusSummary : INSiteStatus
	{
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? InventoryID
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
		#region SiteID
		public new abstract class siteID : PX.Data.IBqlField
		{
		}
		#endregion
		#region QtyOnHand
		public new abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		#endregion
	}

	[PXProjection(typeof(Select4<INLocationStatus, Where<boolTrue, Equal<boolTrue>>, Aggregate<GroupBy<INLocationStatus.inventoryID, GroupBy<INLocationStatus.siteID, GroupBy<INLocationStatus.locationID, Sum<INLocationStatus.qtyOnHand, Sum<INLocationStatus.qtyAvail>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class INLocationStatusSummary : INLocationStatus
	{
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		#endregion
		#region SiteID
		public new abstract class siteID : PX.Data.IBqlField
		{
		}
		#endregion
		#region LocationID
		public new abstract class locationID : PX.Data.IBqlField
		{
		}
		#endregion
		#region QtyOnHand
		public new abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		#endregion
	}


    public class InventoryHelper
    {
		/// <summary>
		/// For a given data record containing a deferral code and a default deferral term,
		/// generates a warning message when the specified deferral code is flexible, but the specified
		/// default deferral term is zero.
		/// </summary>
		/// <param name="sender">The cache containing the records.</param>
		/// <param name="row">The currently processed data record.</param>
		/// <typeparam name="DeferralCode">The DAC field type for deferral code.</param>
		/// <typeparam name="DefaultTerm">The DAC field type for default deferral term.</param>
		public static void CheckZeroDefaultTerm<DeferralCode, DefaultTerm>(PXCache sender, object row)
			where DeferralCode : IBqlField 
			where DefaultTerm : IBqlField
		{
			string deferralCodeValue = sender.GetValue<DeferralCode>(row) as string;
			decimal? defaultTermValue = sender.GetValue<DefaultTerm>(row) as decimal?;

			bool displayWarning = false;

			if (deferralCodeValue != null)
			{
				DRDeferredCode deferralCodeRecord =
					PXSelect<
						DRDeferredCode,
						Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>
					.Select(sender.Graph, deferralCodeValue);

				if (deferralCodeRecord != null &&
					DeferredMethodType.RequiresTerms(deferralCodeRecord) &&
					defaultTermValue.HasValue &&
					defaultTermValue == 0)
				{
					displayWarning = true;
				}
			}

			sender.RaiseExceptionHandling(
				typeof(DefaultTerm).Name,
				row,
				defaultTermValue,
				displayWarning ? new PXSetPropertyException(Messages.NoDefaultTermSpecified, PXErrorLevel.Warning) : null);
		}

        public static List<CacheEntityItem> TemplateEntity(PXGraph graph, string parent, PXSiteMap.ScreenInfo _info)
        {
            List<CacheEntityItem> ret = new List<CacheEntityItem>();
            if (parent == null)
            {
                int i = 0;
                if (_info.GraphName != null)
                {
                    foreach (Data.Description.PXViewDescription viewdescr in _info.Containers.Values)
                    {
                        CacheEntityItem item = new CacheEntityItem();
                        item.Key = viewdescr.ViewName;
                        item.SubKey = viewdescr.ViewName;
                        item.Path = null;
                        item.Name = viewdescr.DisplayName;
                        item.Number = i++;
                        item.Icon = Sprite.Main.GetFullUrl(Sprite.Main.Box);
                        ret.Add(item);
                    }
                }
            }
            else
            {
                string[] viewname = null;
                _info.Views.TryGetValue(parent, out viewname);
                if (viewname != null)
                {
                    int f = 0;
                    var tempgraph = (PXGraph)PXGraph.CreateInstance(GraphHelper.GetType(_info.GraphName));
                    if (!tempgraph.Views.ContainsKey(parent))
                        return null;

                    foreach (PXFieldState field in PXFieldState.GetFields(graph, tempgraph.Views[parent].BqlSelect.GetTables(), false))
                    {
                        CacheEntityItem item = new CacheEntityItem();
                        item.Key = parent + "." + field.Name;
                        item.SubKey = field.Name;
                        item.Path = "((" +
                                                    (string.IsNullOrEmpty(parent)
                                                    ? field.Name
                                                    : parent + "." + field.Name) + "))";
                        item.Name = field.DisplayName;
                        item.Number = f++;
                        item.Icon = Sprite.Main.GetFullUrl(Sprite.Main.BoxIn);
                        ret.Add(item);
                    }
                }
            }
            return ret;
        }
    }

    

	public class InventoryItemMaint : PXGraph<InventoryItemMaint>
	{
        #region delegates
        protected virtual IEnumerable categories(
            [PXInt]
            int? categoryID
        )
        {
            if (categoryID == null)
            {
                yield return new INCategory()
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }
            foreach (INCategory item in PXSelect<INCategory,
            Where<INCategory.parentID,
                Equal<Required<INCategory.parentID>>>,
                OrderBy<Asc<INCategory.sortOrder>>>.Select(this, categoryID))
            {
                if (!string.IsNullOrEmpty(item.Description))
                    yield return item;
            }
        }
        #endregion
        
        #region Cache Attached

		[PXDBString(255)]
        [PXUIField(DisplayName = "Specific Type")]
		[PXStringList(new string[] { "PX.Objects.CS.SegmentValue", "PX.Objects.IN.InventoryItem" },
			new string[] { "Subitem", "Inventory Item Restriction" })]
		protected virtual void RelationGroup_SpecificType_CacheAttached(PXCache sender)
		{
		}

        [PXDefault()]
		[InventoryRaw(typeof(Where<InventoryItem.stkItem, Equal<True>>), IsKey = true, DisplayName = "Inventory ID", Filterable = true)]
        protected virtual void InventoryItem_InventoryCD_CacheAttached(PXCache sender)
        {
        }	
	
		[PXDBString(1, IsFixed = true, BqlField = typeof(InventoryItem.itemType))]
		[PXDefault(INItemTypes.FinishedGood, typeof(Search<INItemClass.itemType, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible)]
		[INItemTypes.StockList()]
		protected virtual void InventoryItem_ItemType_CacheAttached(PXCache sender)
		{			
		}		

        #region ItemClassID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRestrictor(typeof(Where<INItemClass.stkItem, Equal<True>>), Messages.ItemClassIsNonStock)]
        protected virtual void InventoryItem_ItemClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion

		[PXDBScalar(typeof(Search<INLotSerClass.lotSerNumVal, Where<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>))]		
		protected virtual void InventoryItem_LotSerNumSharedVal_CacheAttached(PXCache sender)
		{
		}

		[PXDBScalar(typeof(Search<INLotSerClass.lotSerNumShared, Where<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>))]
		protected virtual void InventoryItem_LotSerNumShared_CacheAttached(PXCache sender)
		{
		}

		[StockItem(IsKey = true, DirtyRead = true)]
		[PXDefault()]
		protected virtual void SiteStatus_InventoryID_CacheAttached(PXCache sender)
		{
		}


		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POVendorInventory.vendorID>>>),
			DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location")]
		[PXFormula(typeof(Selector<POVendorInventory.vendorID, Vendor.defLocationID>))]
		[PXParent(typeof(Select<Location, Where<Location.bAccountID, Equal<Current<POVendorInventory.vendorID>>,
												And<Location.locationID, Equal<Current<POVendorInventory.vendorLocationID>>>>>))]
		protected virtual void POVendorInventory_VendorLocationID_CacheAttached(PXCache sender)
		{
		}

		#region INItemSite
		[StockItem(IsKey = true, DirtyRead = true, CacheGlobal = false, TabOrder = 1)]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemSite.inventoryID>>>>))]
		[PXDefault()]
		protected virtual void INItemSite_InventoryID_CacheAttached(PXCache sender)
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[ItemSite]
		[PXUIField(DisplayName = "Warehouse", Enabled = false, TabOrder = 2)]
		protected virtual void INItemSite_SiteID_CacheAttached(PXCache sender)
		{
		}		
		#endregion
		#region INItemCategory
		[PXDBInt(IsKey = true)]
		[PXSelector(typeof(INCategory.categoryID), DescriptionField = typeof(INCategory.description))]
		[PXUIField(DisplayName = "Category ID")]
		protected virtual void INItemCategory_CategoryID_CacheAttached(PXCache sender)
		{
		}
		[StockItem(IsKey = true, DirtyRead = true)]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemCategory.inventoryID>>>>))]
		[PXDBLiteDefault(typeof(InventoryItem.inventoryID))]
		protected virtual void INItemCategory_InventoryID_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region INItemXRef	
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<INItemXRef.inventoryID>>>))]
		[Inventory(Filterable = true, DirtyRead = true, Enabled = false, IsKey = true)]
		[PXDBDefault(typeof(InventoryItem.inventoryID), DefaultForInsert = true, DefaultForUpdate = false)]		
		protected virtual void INItemXRef_InventoryID_CacheAttached(PXCache sender)
		{
		}
		#endregion
        #region LotSerClassID
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(INLotSerClass.lotSerClassID), DescriptionField = typeof(INLotSerClass.descr))]
        [PXUIField(DisplayName = "Lot/Serial Class")]
        [PXDefault(typeof(Search<INItemClass.lotSerClassID, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
        protected virtual void InventoryItem_LotSerClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostClassID
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(INPostClass.postClassID), DescriptionField = typeof(INPostClass.descr))]
        [PXUIField(DisplayName = "Posting Class")]
        [PXDefault(typeof(Search<INItemClass.postClassID, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
        protected virtual void InventoryItem_PostClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #endregion

		[PXHidden]
		public PXSelect<INLotSerClass> lotSerClass;
		[PXHidden]
		public PXSelect<Location> location; // it's needed to let Location lookup
        public PXSelect<BAccount> baccount; // it's needed to let Customer lookup (Cross-reference tab) to work properly in case AlternateType = [Customer Part Number] 
		public PXSelect<AP.Vendor> vendor;
		public PXSelect<AR.Customer> customer;
		public PXSetup<INSetup> insetup;
		public PXSetupOptional<TX.TXAvalaraSetup> avalaraSetup; 
		[PXCopyPasteHiddenFields(typeof(InventoryItem.body))]
		public PXSelect<InventoryItem,			
			Where<InventoryItem.stkItem, Equal<boolTrue>,
			And<Match<Current<AccessInfo.userName>>>>>
			Item;

		[PXCopyPasteHiddenView]
		public INSubItemSegmentValueList SegmentValues;

        [PXViewName(Messages.InventoryItem)]
		public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> ItemSettings;
		public PXSelect<INItemCost, Where<INItemCost.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> ItemCosts;
		public INUnitSelect<INUnit, InventoryItem.inventoryID, InventoryItem.itemClassID, InventoryItem.salesUnit, InventoryItem.purchaseUnit, InventoryItem.baseUnit, InventoryItem.lotSerClassID> itemunits;

		[PXCopyPasteHiddenView()]
		public PXSelectJoin<INItemSite, 
                            InnerJoin<INSite, On<INSite.siteID, Equal<INItemSite.siteID>>, 
                            LeftJoin<INSiteStatusSummary, On<INSiteStatusSummary.inventoryID, Equal<INItemSite.inventoryID>, 
                                And<INSiteStatusSummary.siteID, Equal<INItemSite.siteID>>>>>, 
                            Where<INItemSite.inventoryID, Equal<Current<InventoryItem.inventoryID>>, 
                                And<Match<INSite, Current<AccessInfo.userName>>>>> itemsiterecords;
		public PXSelect<INItemXRef, Where<INItemXRef.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> itemxrefrecords;
		public PXSetup<INPostClass, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>> postclass;
		public PXSetup<INLotSerClass, Where<INLotSerClass.lotSerClassID, Equal<Current<InventoryItem.lotSerClassID>>>> lotserclass;
		public PXSelect<INComponent, Where<INComponent.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> Components;
		public PXSelect<ARSalesPrice> SalesPrice;
		public PXSelect<INItemClass, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>> ItemClass;

		public PXSelect<INItemRep, Where<INItemRep.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> replenishment;
		public PXSelect<INSubItemRep, 
					 Where<INSubItemRep.inventoryID, Equal<Current<INItemRep.inventoryID>>,
					 And<INSubItemRep.replenishmentClassID, Equal<Current<INItemRep.replenishmentClassID>>>>> subreplenishment;

		public PXSelect<INItemSiteReplenishment, Where<INItemSiteReplenishment.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> itemsitereplenihments;
		
		public POVendorInventorySelect<POVendorInventory, 
			LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POVendorInventory.inventoryID>>,
			LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POVendorInventory.vendorID>>,
			LeftJoin<CRLocation, On<CRLocation.bAccountID, Equal<POVendorInventory.vendorID>,
						And<CRLocation.locationID, Equal<POVendorInventory.vendorLocationID>>>>>>,
			Where<POVendorInventory.inventoryID, Equal<Current<InventoryItem.inventoryID>>>,
			InventoryItem> VendorItems;

        public PXSelect<INItemBoxEx, Where<INItemBoxEx.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> Boxes;

        public PXSelect<INSiteStatus> insitestatus;
        public PXSelect<PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus> sitestatus;

        public PXSelect<ItemStats> itemstats;

        public CRAttributeList<InventoryItem> Answers;
        
        
        public PXSelectJoin<INItemCategory,
	InnerJoin<INCategory, On<INCategory.categoryID, Equal<INItemCategory.categoryID>>>,
	Where<INItemCategory.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> Category;
        

        public PXSelectOrderBy<INCategory, OrderBy<Asc<INCategory.sortOrder>>> Categories;

        public PXSelect<CacheEntityItem,
            Where<CacheEntityItem.path, Equal<CacheEntityItem.path>>,
            OrderBy<Asc<CacheEntityItem.number>>> EntityItems;

        [PXCopyPasteHiddenView]
        public PXSelect<INItemPlan, Where<INItemPlan.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> itemplans;
        [PXCopyPasteHiddenView]
        public PXSelect<INSiteStatus, Where<INSiteStatus.inventoryID, Equal<Current<InventoryItem.inventoryID>>, And<Where<INSiteStatus.qtyOnHand, NotEqual<decimal0>, Or<INSiteStatus.qtyAvail, NotEqual<decimal0>>>>>> nonemptysitestatuses;

        public PXSelect<INPIClassItem> inpiclassitems;
        public PXSelect<ARPriceWorksheetDetail> arpriceworksheetdetails;
        public PXSelect<DiscountItem> discountitems;
        public PXSelect<PM.PMProjectStatus> pmprojectstatuses;
        public PXSelect<PM.PMItemRate> pmitemrates;
        public PXSelect<INKitSpecHdr> kitheaders;
        public PXSelect<INKitSpecStkDet> kitspecs;
        
        #region Delegates
        protected IEnumerable entityItems(string parent)
        {
            PXSiteMapNode siteMap = PXSiteMap.Provider.FindSiteMapNodeByScreenID("IN202500");
            if (siteMap != null)
                foreach (var entry in EMailSourceHelper.TemplateEntity(this, parent, null, siteMap.GraphType, true))
                    yield return entry;
        }	
        #endregion
		
		public PXSelect<PX.SM.RelationGroup> Groups;
		protected System.Collections.IEnumerable groups()
		{
			foreach (PX.SM.RelationGroup group in PXSelect<PX.SM.RelationGroup>.Select(this))
			{
                if ((group.SpecificModule == null || group.SpecificModule == typeof(InventoryItem).Namespace)
					&& (group.SpecificType == null || group.SpecificType == typeof(SegmentValue).FullName || group.SpecificType == typeof(InventoryItem).FullName)
					|| (Item.Current != null && PX.SM.UserAccess.IsIncluded(Item.Current.GroupMask, group)))
				{
					Groups.Current = group;
					yield return group;
				}
			}
		}

		public PXSetup<Company> Company;

		public InventoryItemMaint()
		{
			INSetup record = insetup.Current;

			PXUIFieldAttribute.SetVisible<Vendor.curyID>(this.Caches[typeof (Vendor)], null,
				PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			PXUIFieldAttribute.SetVisible<INUnit.toUnit>(itemunits.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INUnit.toUnit>(itemunits.Cache, null, false);

			PXUIFieldAttribute.SetVisible<INUnit.sampleToUnit>(itemunits.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<INUnit.sampleToUnit>(itemunits.Cache, null, false);

			PXUIFieldAttribute.SetVisible<InventoryItem.pPVAcctID>(Item.Cache, null, true);
			PXUIFieldAttribute.SetVisible<InventoryItem.pPVSubID>(Item.Cache, null, true);

			PXUIFieldAttribute.SetVisible<InventoryItem.discAcctID>(Item.Cache, null, false);
			PXUIFieldAttribute.SetVisible<InventoryItem.discSubID>(Item.Cache, null, false);

			itemsiterecords.Cache.AllowInsert = false;
			itemsiterecords.Cache.AllowDelete = false;

			PXUIFieldAttribute.SetEnabled(itemsiterecords.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INItemSite.isDefault>(itemsiterecords.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<INItemSite.siteStatus>(itemsiterecords.Cache, null, true);

			PXUIFieldAttribute.SetEnabled(Groups.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<PX.SM.RelationGroup.included>(Groups.Cache, null, true);

			bool enableSubItemReplenishment = PXAccess.FeatureInstalled<FeaturesSet.replenishment>() && PXAccess.FeatureInstalled<FeaturesSet.subItem>();
			subreplenishment.AllowSelect = enableSubItemReplenishment;

			PXDBDefaultAttribute.SetDefaultForInsert<INItemXRef.inventoryID>(itemxrefrecords.Cache, null, true);
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });

			action.AddMenuAction(ChangeID);
		}

		#region Buttons Definition		

		[PXCancelButton]
		[PXUIField(MapEnableRights = PXCacheRights.Select)]
		protected virtual System.Collections.IEnumerable Cancel(PXAdapter a)
		{
			foreach (InventoryItem e in (new PXCancel<InventoryItem>(this, "Cancel")).Press(a))
			{				
				if (Item.Cache.GetStatus(e) == PXEntryStatus.Inserted)
				{
					InventoryItem e1 = PXSelect<InventoryItem,
						Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>, And<InventoryItem.stkItem, Equal<False>>>>
						.Select(this, e.InventoryCD);
					if (e1 != null)
					{
						Item.Cache.RaiseExceptionHandling<InventoryItem.inventoryCD>(e, e.InventoryCD, 
							new PXSetPropertyException(Messages.NonStockItemExists));
					}
				}
				yield return e;
			}
		}
		public PXSave<InventoryItem> Save;
		public PXAction<InventoryItem> cancel;
		public PXInsert<InventoryItem> Insert;
		public PXCopyPasteAction<InventoryItem> Edit; 
		public PXDelete<InventoryItem> Delete;
		public PXFirst<InventoryItem> First;
		public PXPrevious<InventoryItem> Prev;
		public PXNext<InventoryItem> Next;
		public PXLast<InventoryItem> Last;		

		public PXChangeID<InventoryItem, InventoryItem.inventoryCD> ChangeID;


		#endregion

		#region InventoryItem Event Handlers

		protected virtual void InventoryItem_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null && ((InventoryItem)e.Row).LotSerNumShared == true)
			{
				sender.SetValue<InventoryItem.lotSerNumVal>(e.Row, sender.GetValue<InventoryItem.lotSerNumSharedVal>(e.Row));
			}
		}

		protected virtual void InventoryItem_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;
			if (row == null) return;

			PXUIFieldAttribute.SetEnabled<InventoryItem.lotSerNumVal>(sender, e.Row,
				(lotserclass.Current != null && lotserclass.Current.LotSerNumShared == false && lotserclass.Current.LotSerTrack != INLotSerTrack.NotNumbered));
			
			if (lotserclass.Current != null && lotserclass.Current.LotSerTrack == INLotSerTrack.NotNumbered && e.Row != null)
                ((InventoryItem)e.Row).LotSerNumVal = null;

			PXUIFieldAttribute.SetEnabled<InventoryItem.cOGSSubID>(sender, e.Row, (postclass.Current != null && postclass.Current.COGSSubFromSales == false));
			PXUIFieldAttribute.SetEnabled<InventoryItem.stdCstVarAcctID>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetEnabled<InventoryItem.stdCstVarSubID>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetEnabled<InventoryItem.stdCstRevAcctID>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetEnabled<InventoryItem.stdCstRevSubID>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetEnabled<InventoryItem.pendingStdCost>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetEnabled<InventoryItem.pendingStdCostDate>(sender, e.Row, e.Row != null && ((InventoryItem)e.Row).ValMethod == INValMethod.Standard);
			PXUIFieldAttribute.SetVisible<InventoryItem.defaultSubItemOnEntry>(sender, null, insetup.Current.UseInventorySubItem == true);			
			PXUIFieldAttribute.SetEnabled<POVendorInventory.isDefault>(this.VendorItems.Cache, null, true);
			INAcctSubDefault.Required(sender, e);
            bool hasremainder = nonemptysitestatuses.SelectSingle() != null;
            PXUIFieldAttribute.SetEnabled<InventoryItem.baseUnit>(sender, e.Row, !hasremainder);

			//Multiple Components are not supported for CashReceipt Deferred Revenue:
			DRDeferredCode dc = PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Current<InventoryItem.deferredCode>>>>.Select(this);
            PXUIFieldAttribute.SetEnabled<InventoryItem.defaultSubItemID>(sender, e.Row, insetup.Current.UseInventorySubItem == true);

			SetDefaultTermControlsState(sender, (InventoryItem)e.Row);

			//Initial State for Components:
			Components.Cache.AllowDelete = false;
			Components.Cache.AllowInsert = false;
			Components.Cache.AllowUpdate = false;

            if (e.Row != null)
			    if (((InventoryItem)e.Row).IsSplitted == true)
			    {
				    Components.Cache.AllowDelete = true;
				    Components.Cache.AllowInsert = true;
				    Components.Cache.AllowUpdate = true;
				    ((InventoryItem)e.Row).TotalPercentage = SumComponentsPercentage();
				    PXUIFieldAttribute.SetEnabled<InventoryItem.useParentSubID>(sender, e.Row, true);
			    }
			    else
			    {
				    ((InventoryItem)e.Row).TotalPercentage = 100;
				    ((InventoryItem)e.Row).UseParentSubID = false;
				    PXUIFieldAttribute.SetEnabled<InventoryItem.useParentSubID>(sender, e.Row, false);
			    }

			Boxes.Cache.AllowInsert = row.PackageOption != INPackageOption.Manual && PXAccess.FeatureInstalled<FeaturesSet.autoPackaging>();
			Boxes.Cache.AllowUpdate = row.PackageOption != INPackageOption.Manual && PXAccess.FeatureInstalled<FeaturesSet.autoPackaging>();
			Boxes.Cache.AllowSelect = PXAccess.FeatureInstalled<FeaturesSet.autoPackaging>();
				
			if (row.PackageOption == INPackageOption.Quantity)
			{
				PXUIFieldAttribute.SetEnabled<InventoryItem.packSeparately>(Item.Cache, Item.Current, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.qty>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.uOM>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxQty>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxWeight>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxVolume>(Boxes.Cache, null, false);
			}
			else if (row.PackageOption == INPackageOption.Weight)
			{
				PXUIFieldAttribute.SetEnabled<InventoryItem.packSeparately>(Item.Cache, Item.Current, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.qty>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.uOM>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxQty>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxWeight>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxVolume>(Boxes.Cache, null, false);
			}
			else if (row.PackageOption == INPackageOption.WeightAndVolume)
			{
				PXUIFieldAttribute.SetEnabled<InventoryItem.packSeparately>(Item.Cache, Item.Current, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.qty>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.uOM>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxQty>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxWeight>(Boxes.Cache, null, true);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxVolume>(Boxes.Cache, null, true);

			}
			else if (row.PackageOption == INPackageOption.Manual)
			{
				Boxes.Cache.AllowSelect = false;
				PXUIFieldAttribute.SetEnabled<InventoryItem.packSeparately>(Item.Cache, Item.Current, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.qty>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.uOM>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxQty>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxWeight>(Boxes.Cache, null, false);
				PXUIFieldAttribute.SetVisible<INItemBoxEx.maxVolume>(Boxes.Cache, null, false);

			}

			if (PXAccess.FeatureInstalled<FeaturesSet.autoPackaging>())
				ValidatePackaging(row);

			InventoryHelper.CheckZeroDefaultTerm<InventoryItem.deferredCode, InventoryItem.defaultTerm>(sender, row);
		}

        protected virtual void InventoryItem_LotSerClassID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (e.Row != null && !PXAccess.FeatureInstalled<FeaturesSet.lotSerialTracking>())
            {
				e.NewValue = INLotSerClass.GetDefaultLotSerClass(this);
				e.Cancel = true;
			}
        }

		protected virtual void InventoryItem_LotSerClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<InventoryItem.lotSerNumVal>(e.Row);
			sender.SetValuePending<InventoryItem.lotSerNumVal>(e.Row, PXCache.NotSetValue);
		}

		protected virtual void InventoryItem_LotSerClassID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			INItemLotSerial status =
				PXSelect<INItemLotSerial,
				Where<INItemLotSerial.inventoryID, Equal<Required<INItemLotSerial.inventoryID>>,					
					And<INItemLotSerial.qtyOnHand, NotEqual<decimal0>>>>
					.SelectWindowed(this, 0, 1, ((InventoryItem)e.Row).InventoryID);

			INSiteStatus sitestatus =
				PXSelect<INSiteStatus,
				Where<INSiteStatus.inventoryID, Equal<Required<INSiteStatus.inventoryID>>,
				  And<Where<INSiteStatus.qtyOnHand, NotEqual<decimal0>,
						  Or<INSiteStatus.qtyINReceipts, NotEqual<decimal0>,
						  Or<INSiteStatus.qtyInTransit, NotEqual<decimal0>,
						  Or<INSiteStatus.qtyINIssues, NotEqual<decimal0>,
						  Or<INSiteStatus.qtyINAssemblyDemand, NotEqual<decimal0>,
						  Or<INSiteStatus.qtyINAssemblySupply, NotEqual<decimal0>>>>>>>>>>
				.SelectWindowed(this, 0, 1, ((InventoryItem)e.Row).InventoryID);
			if (status != null || sitestatus != null)
			{
                INLotSerClass oldClass = (INLotSerClass)PXSelectorAttribute.Select<InventoryItem.lotSerClassID>(sender, e.Row);
				INLotSerClass newClass =
                    (INLotSerClass)PXSelectorAttribute.Select<InventoryItem.lotSerClassID>(sender, e.Row, e.NewValue);
				if (newClass == null || (oldClass.LotSerTrack != newClass.LotSerTrack ||
				    oldClass.LotSerTrackExpiration != newClass.LotSerTrackExpiration ||
				    oldClass.LotSerAssign != newClass.LotSerAssign))
				{
					throw new PXSetPropertyException(Messages.ItemLotSerClassVerifying);
				}
			}
		
		}

        protected virtual void InventoryItem_DefaultSubItemID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
	        if (this.IsImport)
		        e.Cancel = true;
        }

		protected virtual void InventoryItem_DeferredCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var item = e.Row as InventoryItem;
			UpdateSplittedFromDeferralCode(sender, item);
			SetDefaultTerm(sender, item, typeof(InventoryItem.deferredCode), typeof(InventoryItem.defaultTerm), typeof(InventoryItem.defaultTermUOM));
		}

		protected virtual void INComponent_DeferredCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SetDefaultTerm(sender, e.Row, typeof(INComponent.deferredCode), typeof(INComponent.defaultTerm), typeof(INComponent.defaultTermUOM));
		}

		public static void UpdateSplittedFromDeferralCode(PXCache cache, InventoryItem item)
		{
			if (item == null)
				return;

			var code = (DRDeferredCode)PXSelectorAttribute.Select<InventoryItem.deferredCode>(cache, item);
			cache.SetValueExt<InventoryItem.isSplitted>(item, code != null && code.MultiDeliverableArrangement == true);
		}

		public static void SetDefaultTerm(PXCache cache, object row, Type deferralCode, Type defaultTerm, Type defaultTermUOM)
		{
			if (row == null)
				return;

			var code = (DRDeferredCode)PXSelectorAttribute.Select(cache, row, deferralCode.Name);

			if(code == null || DeferredMethodType.RequiresTerms(code) == false)
			{
				cache.SetDefaultExt(row, defaultTerm.Name);
				cache.SetDefaultExt(row, defaultTermUOM.Name);
			}
		}

		public static void SetDefaultTermControlsState(PXCache cache, InventoryItem item)
		{
			if (item == null)
				return;

			var code = (DRDeferredCode)PXSelectorAttribute.Select<InventoryItem.deferredCode>(cache, item);
			bool enableTerms = DeferredMethodType.RequiresTerms(code);
			PXUIFieldAttribute.SetEnabled<InventoryItem.defaultTerm>(cache, item, enableTerms);
			PXUIFieldAttribute.SetEnabled<InventoryItem.defaultTermUOM>(cache, item, enableTerms);
		}

		public static void SetComponentControlsState(PXCache cache, INComponent component)
		{
			if (component == null)
				return;

			bool isResidual = component.AmtOption == INAmountOption.Residual;

			PXDefaultAttribute.SetPersistingCheck<INComponent.deferredCode>(cache, component, isResidual ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

			PXUIFieldAttribute.SetEnabled<INComponent.deferredCode>(cache, component, isResidual == false);
			PXUIFieldAttribute.SetEnabled<INComponent.fixedAmt>(cache, component, component.AmtOption == INAmountOption.FixedAmt);
			PXUIFieldAttribute.SetEnabled<INComponent.percentage>(cache, component, component.AmtOption == INAmountOption.Percentage);

			var code = (DRDeferredCode)PXSelectorAttribute.Select<INComponent.deferredCode>(cache, component);
			bool enableTerms = DeferredMethodType.RequiresTerms(code) && isResidual == false;
			PXUIFieldAttribute.SetEnabled<INComponent.defaultTerm>(cache, component, enableTerms);
			PXUIFieldAttribute.SetEnabled<INComponent.defaultTermUOM>(cache, component, enableTerms);
		}

		public static void CheckSameTermOnAllComponents(PXCache cache, IEnumerable<INComponent> components)
		{
			DRTerms.Term term = null;

			foreach(var component in components)
			{
				var code = (DRDeferredCode)PXSelectorAttribute.Select<INComponent.deferredCode>(cache, component);
				if(DeferredMethodType.RequiresTerms(code))
				{
					if(term == null)
					{
						term = new DRTerms.Term(component.DefaultTerm, component.DefaultTermUOM);
						continue;
					}

					if(term.Length != component.DefaultTerm || term.UOM != component.DefaultTermUOM)
					{
						if (cache.RaiseExceptionHandling<INComponent.defaultTerm>(component, component.DefaultTerm,
							new PXSetPropertyException<INComponent.defaultTerm>(DR.Messages.DefaultTermMustBeTheSameForAllComponents)))
							throw new PXRowPersistingException(typeof(INComponent.defaultTerm).Name, component.DefaultTerm, DR.Messages.DefaultTermMustBeTheSameForAllComponents);
					}
				}
			}
		}

		protected virtual void InventoryItem_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			InventoryItem item = (InventoryItem)e.Row;

			foreach (PXResult<INItemSite, INSite, INSiteStatusSummary> res in itemsiterecords.Select())
			{
			    INItemSite itemsite = res;
			    INSite site = res;
			    INPostClass pclass = postclass.Current;
				bool IsUpdateFlag = false;
				bool IsInserted = itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted;
                sender.RaiseRowSelected(e.Row);

				//TODO: remove obsolete code
				/*
				if (IsInserted)
				{
					if (site != null && site.OverrideInvtAccSub == true)
					{
						itemsite.InvtAcctID = site.InvtAcctID;
						itemsite.InvtSubID = site.InvtSubID;
						IsUpdateFlag = true;
					}
					else if (pclass != null)
					{
						itemsite.InvtAcctID = INReleaseProcess.GetAccountDefaults<INPostClass.invtAcctID>(this, item, site, pclass);
						itemsite.InvtSubID = INReleaseProcess.GetAccountDefaults<INPostClass.invtSubID>(this, item, site, pclass);
						IsUpdateFlag = true;
					}
				}
				else if (pclass != null)
				{
					InventoryItem olditem = (InventoryItem)e.OldRow;
					HashSet<char> maskset = new HashSet<char>(pclass.InvtSubMask.ToCharArray());
					if (pclass.InvtAcctDefault == INAcctSubDefault.MaskItem
						&& maskset.Count == 1 && maskset.Contains(INAcctSubDefault.MaskItem[0])
						&& olditem.InvtAcctID == itemsite.InvtAcctID
						&& olditem.InvtSubID == itemsite.InvtSubID)
					{
						itemsite.InvtAcctID = item.InvtAcctID;
						itemsite.InvtSubID = item.InvtSubID;
						IsUpdateFlag = true;
					}
				}
				*/

				if (string.Equals(((InventoryItem)e.Row).ValMethod, ((InventoryItem)e.OldRow).ValMethod) == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.ValMethod = item.ValMethod;
					IsUpdateFlag = true;
				}

				if (itemsite.ValMethod == INValMethod.Standard && (itemsite.StdCostOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted))
				{
					itemsite.PendingStdCost = item.PendingStdCost;
					itemsite.PendingStdCostDate = item.PendingStdCostDate;
					itemsite.StdCost = item.StdCost;
					itemsite.StdCostDate = item.StdCostDate;
					itemsite.LastStdCost = item.LastStdCost;

					IsUpdateFlag = true;
				}

				if (itemsite.BasePriceOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.BasePrice = item.BasePrice;

					IsUpdateFlag = true;
				}

				if (itemsite.MarkupPctOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.MarkupPct = item.MarkupPct;

					IsUpdateFlag = true;
				}

				if (itemsite.RecPriceOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.RecPrice = item.RecPrice;

					IsUpdateFlag = true;
				}

				if (itemsite.ABCCodeOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.ABCCodeID = item.ABCCodeID;
					itemsite.ABCCodeIsFixed = item.ABCCodeIsFixed;

					IsUpdateFlag = true;
				}

				if (itemsite.MovementClassOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.MovementClassID = item.MovementClassID;
					itemsite.MovementClassIsFixed = item.MovementClassIsFixed;

					IsUpdateFlag = true;
				}
				if (itemsite.PreferredVendorOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{
					itemsite.PreferredVendorID = item.PreferredVendorID;
					itemsite.PreferredVendorLocationID = item.PreferredVendorLocationID;
					IsUpdateFlag = true;
				}

                if (string.Equals(((InventoryItem)e.Row).SalesUnit, ((InventoryItem)e.OldRow).SalesUnit) == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
                {
                    itemsite.DfltSalesUnit = item.SalesUnit;
                    IsUpdateFlag = true;
                }

                if (string.Equals(((InventoryItem)e.Row).PurchaseUnit, ((InventoryItem)e.OldRow).PurchaseUnit) == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
                {
                    itemsite.DfltPurchaseUnit = item.PurchaseUnit;
                    IsUpdateFlag = true;
                }

				if (INItemSiteMaint.DefaultItemReplenishment(this, itemsite))
					IsUpdateFlag = true;

				if (IsUpdateFlag && itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Notchanged)
				{
					itemsiterecords.Cache.SetStatus(itemsite, PXEntryStatus.Updated);
				}
			}

			if (lotserclass.Current != null && lotserclass.Current.LotSerNumShared == true && string.Equals(item.LotSerNumVal, lotserclass.Current.LotSerNumVal) == false)
			{
				item.LotSerNumVal = lotserclass.Current.LotSerNumVal;
			}

					}

		protected virtual void INItemCost_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INItemCost row = e.Row as INItemCost;
			if (row == null) return;
			bool lastCostEnabled = !(Item.Current.ValMethod == INValMethod.Standard || Item.Current.ValMethod == INValMethod.Specific) 
									&& ( itemsiterecords.SelectSingle(row.InventoryID) != null || !PXAccess.FeatureInstalled<FeaturesSet.warehouse>());
			PXUIFieldAttribute.SetEnabled<INItemCost.lastCost>(sender, e.Row, lastCostEnabled);
		}

		protected virtual void INItemCost_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			INItemCost row = (INItemCost)e.Row;			
			if (row != null && row.LastCost != 0m && row.LastCost != null)
			{
				UdateLastCost(row);
			}
		}
		protected virtual void INItemCost_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			INItemCost row = (INItemCost)e.Row;
			INItemCost oldRow = (INItemCost)e.OldRow;
			if (row != null && oldRow != null && row.LastCost != oldRow.LastCost &&
				  row.LastCost != null)
			{
				UdateLastCost(row);
			}
		}
		private void UdateLastCost(INItemCost row)
		{
			foreach (ItemStats stats in itemstats.Cache.Inserted)
			{
				itemstats.Cache.Delete(stats);
			}
			foreach (INItemSite itemsite in itemsiterecords.Select(row.InventoryID))
			{
				ItemStats stats = new ItemStats();
				stats.InventoryID = itemsite.InventoryID;
				stats.SiteID = itemsite.SiteID;
				stats = itemstats.Insert(stats);
				stats.LastCost = row.LastCost;
                stats.LastCostDate = DateTime.Now;
			}
		}
		protected virtual void INSubItemRep_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			UpdateSubItemSiteReplenishment(e.Row, PXDBOperation.Insert);
		}
		protected virtual void INSubItemRep_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			UpdateSubItemSiteReplenishment(e.Row, PXDBOperation.Update);
		}
		protected virtual void INSubItemRep_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			UpdateSubItemSiteReplenishment(e.Row, PXDBOperation.Delete);
		}
		private void UpdateSubItemSiteReplenishment(object item, PXDBOperation operation)
		{
			INSubItemRep row = item as INSubItemRep;
            if (row == null || row.InventoryID == null || row.SubItemID == null) return;

            foreach (INItemSite itemsite in
				PXSelect<INItemSite, 
					Where<INItemSite.inventoryID, Equal<Required<INItemSite.inventoryID>>,
						And<INItemSite.subItemOverride, Equal<boolFalse>>>,
                OrderBy<Asc<INItemSite.inventoryID>>>.Select(this, row.InventoryID))
			{
                if (itemsite.ReplenishmentClassID != row.ReplenishmentClassID) continue;
                PXCache source = this.Caches[typeof(INItemSiteReplenishment)];
				INItemSiteReplenishment r = PXSelect<INItemSiteReplenishment,
					Where<INItemSiteReplenishment.inventoryID, Equal<Required<INItemSiteReplenishment.inventoryID>>,
						And<INItemSiteReplenishment.siteID, Equal<Required<INItemSiteReplenishment.siteID>>,
						And<INItemSiteReplenishment.subItemID, Equal<Required<INItemSiteReplenishment.subItemID>>>>>>
                    .SelectWindowed(this, 0, 1, row.InventoryID, itemsite.SiteID, row.SubItemID);

				if (r == null)
				{
                    if (operation == PXDBOperation.Delete) continue;

					r = new INItemSiteReplenishment();
					operation = PXDBOperation.Insert;
					r.InventoryID = row.InventoryID;
					r.SiteID = itemsite.SiteID;
					r.SubItemID = row.SubItemID;
				}
				else
					r = PXCache<INItemSiteReplenishment>.CreateCopy(r);
				
				r.SafetyStock = row.SafetyStock;				
				r.MinQty = row.MinQty;
				r.MaxQty = row.MaxQty;
				r.TransferERQ = row.TransferERQ;
				r.ItemStatus = row.ItemStatus;

                switch (operation)
				{
					case PXDBOperation.Insert:
						source.Insert(r);
						break;
					case PXDBOperation.Update:
						source.Update(r);
						break;
					case PXDBOperation.Delete:
						source.Delete(r);
						break;
				}				
			}
			
		}		

		protected virtual void ItemStats_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && ((ItemStats)e.Row).InventoryID < 0 && this.Item.Current != null)
			{
				int? _KeyToAbort = (int?)Item.Cache.GetValue<InventoryItem.inventoryID>(Item.Current);
				if (!_persisted.ContainsKey(_KeyToAbort))
				{
					_persisted.Add(_KeyToAbort, ((ItemStats)e.Row).InventoryID);
				}
				((ItemStats)e.Row).InventoryID = _KeyToAbort;
				sender.Normalize();
			}
		}

		Dictionary<int?, int?> _persisted = new Dictionary<int?, int?>();

		protected virtual void ItemStats_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.TranStatus == PXTranStatus.Aborted && (e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				int? _KeyToAbort;
				if (_persisted.TryGetValue(((ItemStats)e.Row).InventoryID, out _KeyToAbort))
				{
					((ItemStats)e.Row).InventoryID = _KeyToAbort;
				}
			}
		}

		protected virtual void InventoryItem_LotSerNumVal_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (lotserclass.Current != null && (bool)lotserclass.Current.LotSerNumShared)
				{
					e.FieldName = string.Empty;
					e.Cancel = true;
				}
			}
		}

		protected virtual void InventoryItem_LotSerNumVal_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				INLotSerClass lsclass = (INLotSerClass)lotserclass.View.SelectSingleBound(new object[] { e.Row });
				InventoryItem item = (InventoryItem)e.Row;

				if (lsclass != null && lsclass.LotSerNumShared == false)
				{
					string itemLotSerNumVal = sender.GetValuePending<InventoryItem.lotSerNumVal>(e.Row) as string ?? item.LotSerNumVal;

					if (string.IsNullOrEmpty(lsclass.LotSerNumVal) && string.IsNullOrEmpty(itemLotSerNumVal))
					{
						e.NewValue = "000000";
					}
					else
					{
						int currentNum;
						if (int.TryParse(itemLotSerNumVal ?? "0", out currentNum))
						{
							string result = currentNum.ToString(lsclass.LotSerNumVal ?? "000000");
							int checkNum;
							if (int.TryParse(result, out checkNum) && checkNum == currentNum)
							{
								e.NewValue = result;
							}
							else
							{
								e.NewValue = itemLotSerNumVal;
							}
						}
					}

					e.Cancel = true;
				}
			}
		}

		protected virtual void InventoryItem_DfltSiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemSite itemsite = (INItemSite)PXSelect<INItemSite, Where<INItemSite.inventoryID, Equal<Current<InventoryItem.inventoryID>>, And<INItemSite.siteID, Equal<Current<InventoryItem.dfltSiteID>>>>>.Select(this);

			INSite site = (INSite)PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, ((InventoryItem)e.Row).DfltSiteID);

			if (itemsite != null)
			{
				itemsite = PXCache<INItemSite>.CreateCopy(itemsite);
				itemsite.IsDefault = true;
				itemsiterecords.Update(itemsite);

				//DfltSiteID should follow locations in DAC
				((InventoryItem)e.Row).DfltShipLocationID = itemsite.DfltShipLocationID;
				((InventoryItem)e.Row).DfltReceiptLocationID = itemsite.DfltReceiptLocationID;
			}
			else if (site != null)
			{
				itemsite = new INItemSite();
				itemsite.InventoryID = ((InventoryItem)e.Row).InventoryID;
				itemsite.SiteID = ((InventoryItem)e.Row).DfltSiteID;
				IN.INItemSiteMaint.DefaultItemSiteByItem(this, itemsite, (InventoryItem)e.Row, site, postclass.Current);
				itemsite.IsDefault = true;
				itemsite.StdCostOverride = false;
				itemsite.DfltReceiptLocationID = site.ReceiptLocationID;
				itemsite.DfltShipLocationID = site.ShipLocationID;
				itemsiterecords.Insert(itemsite);

				//default item locations in this case too
				((InventoryItem)e.Row).DfltShipLocationID = itemsite.DfltShipLocationID; // already set from site
				((InventoryItem)e.Row).DfltReceiptLocationID = itemsite.DfltReceiptLocationID;
			}
			else
			{
				((InventoryItem)e.Row).DfltShipLocationID = null;
				((InventoryItem)e.Row).DfltReceiptLocationID = null;
			}
		}

		protected virtual void INItemSite_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		public static void VerifyComponentPercentages(PXCache itemCache, InventoryItem item, IEnumerable<INComponent> components)
		{
			var hasResiduals = components.Any(c => c.AmtOption == INAmountOption.Residual);

			if (item.TotalPercentage != 100 && hasResiduals == false)
			{
				if (itemCache.RaiseExceptionHandling<InventoryItem.totalPercentage>(item, item.TotalPercentage, new PXSetPropertyException(Messages.SumOfAllComponentsMustBeHundred)))
				{
					throw new PXRowPersistingException(typeof(InventoryItem.totalPercentage).Name, item.TotalPercentage, Messages.SumOfAllComponentsMustBeHundred);
				}
			}
			else if (item.TotalPercentage >= 100 && hasResiduals == true)
			{
				if (itemCache.RaiseExceptionHandling<InventoryItem.totalPercentage>(item, item.TotalPercentage, new PXSetPropertyException(Messages.SumOfAllComponentsMustBeLessHundredWithResiduals)))
				{
					throw new PXRowPersistingException(typeof(InventoryItem.totalPercentage).Name, item.TotalPercentage, Messages.SumOfAllComponentsMustBeLessHundredWithResiduals);
				}
			}
		}

		public static void VerifyOnlyOneResidualComponent(PXCache itemCache, InventoryItem item, IEnumerable<INComponent> components)
		{
			if(components.Count(c => c.AmtOption == INAmountOption.Residual) > 1)
			{
				if (itemCache.RaiseExceptionHandling<InventoryItem.totalPercentage>(item, item.TotalPercentage, new PXSetPropertyException(Messages.OnlyOneResidualComponentAllowed)))
				{
					throw new PXRowPersistingException(typeof(InventoryItem.totalPercentage).Name, item.TotalPercentage, Messages.OnlyOneResidualComponentAllowed);
				}
			}
		}

		protected virtual void InventoryItem_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			INAcctSubDefault.Required(sender, e);
			InventoryItem row = e.Row as InventoryItem;

			if (row.IsSplitted == true)
			{
				if (string.IsNullOrEmpty(row.DeferredCode))
				{
					if (sender.RaiseExceptionHandling<InventoryItem.deferredCode>(e.Row, row.DeferredCode, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, typeof(InventoryItem.deferredCode).Name)))
					{
						throw new PXRowPersistingException(typeof(InventoryItem.deferredCode).Name, row.DeferredCode, Data.ErrorMessages.FieldIsEmpty, typeof(InventoryItem.deferredCode).Name);
					}
				}

				var components = Components.Select().RowCast<INComponent>().ToList();

				VerifyComponentPercentages(sender, row, components);
				VerifyOnlyOneResidualComponent(sender, row, components);
				CheckSameTermOnAllComponents(Components.Cache, components);
			}

			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (((InventoryItem)e.Row).ValMethod == INValMethod.Specific && lotserclass.Current != null && (lotserclass.Current.LotSerTrack == INLotSerTrack.NotNumbered || lotserclass.Current.LotSerAssign != INLotSerAssign.WhenReceived))
				{
					if (sender.RaiseExceptionHandling<InventoryItem.valMethod>(e.Row, INValMethod.Specific, new PXSetPropertyException(Messages.SpecificOnlyNumbered)))
					{
						throw new PXRowPersistingException(typeof(InventoryItem.valMethod).Name, INValMethod.Specific, Messages.SpecificOnlyNumbered, typeof(InventoryItem.valMethod).Name);
					}
				}
			}

			if (e.Operation == PXDBOperation.Delete)
			{
				PXDatabase.Delete<INSiteStatus>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("QtyOnHand", PXDbType.Decimal, 8, 0m, PXComp.EQ),
					new PXDataFieldRestrict("QtyAvail", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

				PXDatabase.Delete<INLocationStatus>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("QtyOnHand", PXDbType.Decimal, 8, 0m, PXComp.EQ),
					new PXDataFieldRestrict("QtyAvail", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

				PXDatabase.Delete<INLotSerialStatus>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("QtyOnHand", PXDbType.Decimal, 8, 0m, PXComp.EQ),
					new PXDataFieldRestrict("QtyAvail", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

				PXDatabase.Delete<INCostStatus>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("QtyOnHand", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

				PXDatabase.Delete<INItemCostHist>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("FinYtdQty", PXDbType.Decimal, 8, 0m, PXComp.EQ),
					new PXDataFieldRestrict("FinYtdCost", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

				PXDatabase.Delete<INItemSiteHist>(
					new PXDataFieldRestrict("InventoryID", PXDbType.Int, row.InventoryID),
					new PXDataFieldRestrict("FinYtdQty", PXDbType.Decimal, 8, 0m, PXComp.EQ)
					);

                PXDatabase.Delete<CSAnswers>(new PXDataFieldRestrict("RefNoteID", PXDbType.UniqueIdentifier, ((InventoryItem)e.Row).NoteID));
            }

			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				INLotSerClass cls = lotserclass.Current;
				if (cls != null && cls.LotSerTrack != INLotSerTrack.NotNumbered && row.LotSerNumShared == false && string.IsNullOrEmpty(row.LotSerNumVal))
				{
                    sender.RaiseExceptionHandling<InventoryItem.lotSerNumVal>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(InventoryItem.lotSerNumVal).Name));
				}
			}

		}

		private bool AlwaysFromBaseCurrency
		{
			get
			{
				bool alwaysFromBase = false;

				ARSetup arsetup = PXSelect<ARSetup>.Select(this);
				if (arsetup != null)
				{
					alwaysFromBase = arsetup.AlwaysFromBaseCury == true;
				}

				return alwaysFromBase;
			}
		}

		protected virtual void InventoryItem_DfltReceiptLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemSite itemsite = (INItemSite)PXSelect<INItemSite, Where<INItemSite.inventoryID, Equal<Current<InventoryItem.inventoryID>>, And<INItemSite.siteID, Equal<Current<InventoryItem.dfltSiteID>>>>>.Select(this);

			if (itemsite != null)
			{
				itemsite.DfltReceiptLocationID = ((InventoryItem)e.Row).DfltReceiptLocationID;
				if (itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Notchanged)
				{
					itemsiterecords.Cache.SetStatus(itemsite, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void InventoryItem_DfltShipLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemSite itemsite = (INItemSite)PXSelect<INItemSite, Where<INItemSite.inventoryID, Equal<Current<InventoryItem.inventoryID>>, And<INItemSite.siteID, Equal<Current<InventoryItem.dfltSiteID>>>>>.Select(this);

			if (itemsite != null)
			{
				itemsite.DfltShipLocationID = ((InventoryItem)e.Row).DfltShipLocationID;
				if (itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Notchanged)
				{
					itemsiterecords.Cache.SetStatus(itemsite, PXEntryStatus.Updated);
				}
			}
		}
		
		protected virtual void InventoryItem_DefaultSubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			AddVendorDetail(sender, (InventoryItem)e.Row);
		}
		
		protected virtual void InventoryItem_PreferredVendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			AddVendorDetail(sender, (InventoryItem)e.Row);
		}

		private POVendorInventory AddVendorDetail(PXCache sender, InventoryItem row)
		{
			if (row == null || row.PreferredVendorID == null || row.DefaultSubItemID == null)
			{
				return null;
			}

			POVendorInventory item = 
				PXSelect<POVendorInventory,
			Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>,
			And<POVendorInventory.subItemID, Equal<Required<InventoryItem.defaultSubItemID>>,
			And<POVendorInventory.vendorID, Equal<Required<POVendorInventory.vendorID>>,
            And<Where<POVendorInventory.vendorLocationID, Equal<Required<InventoryItem.preferredVendorLocationID>>,				        
                         Or<POVendorInventory.vendorLocationID, IsNull>>>>>>>
            .SelectWindowed(this, 0, 1, row.InventoryID, row.DefaultSubItemID, row.PreferredVendorID, row.PreferredVendorLocationID);
			if (item == null)
			{
				item = new POVendorInventory();
				item.InventoryID = row.InventoryID;
				item.SubItemID = row.DefaultSubItemID;
				item.PurchaseUnit = row.PurchaseUnit;
				item.VendorID = row.PreferredVendorID;
				item.VendorLocationID = row.PreferredVendorLocationID;				
				item = (POVendorInventory)VendorItems.Cache.Insert(item);				
			}
			return item;
		}

		protected virtual void InventoryItem_ItemClassID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			this.doResetDefaultsOnItemClassChange = false;
				InventoryItem row = (InventoryItem)e.Row;
				INItemClass ic = (INItemClass)PXSelectorAttribute.Select<INItemClass.itemClassID>(cache, row, e.NewValue);

				if (ic != null)
				{
					this.doResetDefaultsOnItemClassChange = true;
					if (e.ExternalCall && cache.GetStatus(row) != PXEntryStatus.Inserted)
					{
						if (Item.Ask(AR.Messages.Warning, Messages.ItemClassChangeWarning, MessageButtons.YesNo) == WebDialogResult.No)
						{
							this.doResetDefaultsOnItemClassChange = false;
						}
					}
				}
			}
		protected virtual void InventoryItem_ItemClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
				if (doResetDefaultsOnItemClassChange)
				{
					sender.SetDefaultExt<InventoryItem.postClassID>(e.Row);
					sender.SetDefaultExt<InventoryItem.priceClassID>(e.Row);
					sender.SetDefaultExt<InventoryItem.priceWorkgroupID>(e.Row);
					sender.SetDefaultExt<InventoryItem.priceManagerID>(e.Row);
					sender.SetDefaultExt<InventoryItem.markupPct>(e.Row);
					sender.SetDefaultExt<InventoryItem.minGrossProfitPct>(e.Row);
					

					INItemClass ic = ItemClass.Select();
					if (ic != null)
					{
						sender.SetValue<InventoryItem.priceWorkgroupID>(e.Row, ic.PriceWorkgroupID);
						sender.SetValue<InventoryItem.priceManagerID>(e.Row, ic.PriceManagerID);
					}					

				sender.SetDefaultExt<InventoryItem.lotSerClassID>(e.Row);

				//sales and purchase units must be cleared not to be added to item unit conversions on base unit change.
				sender.SetValueExt<InventoryItem.baseUnit>(e.Row, null);
				sender.SetValue<InventoryItem.salesUnit>(e.Row, null);
				sender.SetValue<InventoryItem.purchaseUnit>(e.Row, null);
				sender.SetDefaultExt<InventoryItem.baseUnit>(e.Row);
				sender.SetDefaultExt<InventoryItem.salesUnit>(e.Row);
				sender.SetDefaultExt<InventoryItem.purchaseUnit>(e.Row);
				sender.SetDefaultExt<InventoryItem.dfltSiteID>(e.Row);
				sender.SetDefaultExt<InventoryItem.valMethod>(e.Row);

					sender.SetDefaultExt<InventoryItem.taxCategoryID>(e.Row);
					sender.SetDefaultExt<InventoryItem.itemType>(e.Row);
				}
		
			AppendGroupMask(((InventoryItem)e.Row).ItemClassID, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);

			if ((InventoryItem)e.Row != null && ((InventoryItem)e.Row).ItemClassID != null && e.ExternalCall)
			{
				Answers.Cache.Clear();
			}

			if (sender.GetStatus(e.Row) == PXEntryStatus.Inserted)
			{
				foreach (INItemRep r in this.replenishment
					.Select(sender.GetValue<InventoryItem.inventoryID>(e.Row)))
					this.replenishment.Delete(r);

				foreach (INItemClassRep r in PXSelect<INItemClassRep,
					Where<INItemClassRep.itemClassID, Equal<Required<INItemClassRep.itemClassID>>>>
					.Select(this, sender.GetValue<InventoryItem.itemClassID>(e.Row)))
				{
					INItemRep ri = new INItemRep();
					ri.ReplenishmentClassID = r.ReplenishmentClassID;
					ri.ReplenishmentMethod = r.ReplenishmentMethod;
					ri.ReplenishmentPolicyID = r.ReplenishmentPolicyID;
					ri.LaunchDate = r.LaunchDate;
					ri.TerminationDate = r.TerminationDate;
					this.replenishment.Insert(ri);
				}
			}
		}

		protected virtual void InventoryItem_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			((InventoryItem)e.Row).TotalPercentage = 100;
			foreach (INItemClassRep r in PXSelect<INItemClassRep,
					Where<INItemClassRep.itemClassID, Equal<Required<INItemClassRep.itemClassID>>>>
					.Select(this, ((InventoryItem)e.Row).ItemClassID))
			{
				INItemRep ri = new INItemRep();
				ri.ReplenishmentClassID = r.ReplenishmentClassID;
				ri.ReplenishmentMethod = r.ReplenishmentMethod;
				ri.ReplenishmentPolicyID = r.ReplenishmentPolicyID;
				ri.LaunchDate = r.LaunchDate;
				ri.TerminationDate = r.TerminationDate;
				this.replenishment.Insert(ri);
			}
			INItemClass ic = ItemClass.Select();
			if (((InventoryItem)e.Row).InventoryCD != null &&
				((InventoryItem)e.Row).ItemClassID != null &&
				((InventoryItem)e.Row).DfltSiteID == ic.DfltSiteID)
				sender.SetDefaultExt<InventoryItem.dfltSiteID>(e.Row);
				
			AppendGroupMask(((InventoryItem)e.Row).ItemClassID, true);
			_JustInserted = true;
		}

		protected virtual void InventoryItem_PostClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<InventoryItem.invtAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.invtSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.salesAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.salesSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.cOGSAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.cOGSSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.discAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.discSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.stdCstVarAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.stdCstVarSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.stdCstRevAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.stdCstRevSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pPVAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pPVSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pOAccrualAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pOAccrualSubID>(e.Row);

			sender.SetDefaultExt<InventoryItem.reasonCodeSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.lCVarianceAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.lCVarianceSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.deferralAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.deferralSubID>(e.Row);
		}

		protected virtual void InventoryItem_PurchaseUnit_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			InventoryItem row = (InventoryItem)e.Row;
			if (row == null || string.Compare(row.PurchaseUnit, (string)e.OldValue, true) == 0) return;

			PXSelectBase<POVendorInventory> selectVendorDetails = new PXSelect<POVendorInventory,
				Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>,
				And<Where<POVendorInventory.purchaseUnit, Equal<Required<POVendorInventory.purchaseUnit>>, Or<POVendorInventory.purchaseUnit, Equal<Required<POVendorInventory.purchaseUnit>>>>>>>(this);

			var result = selectVendorDetails.Select(row.InventoryID, row.PurchaseUnit, e.OldValue);

			foreach (POVendorInventory detailWithOldPurchaseUnit in result.Where(x => ((POVendorInventory)x).PurchaseUnit == (string)e.OldValue))
			{
				POVendorInventory existing = result.Where(x => ((POVendorInventory)x).PurchaseUnit == row.PurchaseUnit && ((POVendorInventory)x).VendorID == detailWithOldPurchaseUnit.VendorID).FirstOrDefault();
				if (existing == null)
				{
					if (detailWithOldPurchaseUnit.LastPrice != null)
						detailWithOldPurchaseUnit.LastPrice = POItemCostManager.ConvertUOM(this, row, (string)e.OldValue, detailWithOldPurchaseUnit.LastPrice.Value, row.PurchaseUnit);
					detailWithOldPurchaseUnit.PurchaseUnit = row.PurchaseUnit;
					this.VendorItems.Update(detailWithOldPurchaseUnit);
				}
			}
		}

		protected virtual void Vendor_CuryID_FieldSelecting(PXCache sedner, PXFieldSelectingEventArgs e)
		{
			if (e.ReturnValue == null)
				e.ReturnValue = this.Company.Current.BaseCuryID;
		}

        protected virtual void POVendorInventory_IsDefault_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			POVendorInventory current = e.Row as POVendorInventory;
            if ((POVendorInventory)this.VendorItems.SelectWindowed(0, 1, current.InventoryID) == null)
			{
				e.NewValue = true;
				e.Cancel = true;
			}
		}

		protected virtual void POVendorInventory_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			POVendorInventory current = e.Row as POVendorInventory;								
			if (current.VendorID != null && current.SubItemID != null && current.IsDefault == true)
			{
                    InventoryItem upd = this.Item.Current;
				upd.PreferredVendorID = current.IsDefault == true ? current.VendorID : null;
				upd.PreferredVendorLocationID = current.IsDefault == true ? current.VendorLocationID : null;
					Item.Update(upd);
			}
		}

		protected virtual void POVendorInventory_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			POVendorInventory current = e.Row as POVendorInventory;
			if (e.OldRow == null || current == null || current.VendorID == null || current.SubItemID == null)
				return;

                if ((current.IsDefault == true && (this.Item.Current.PreferredVendorID != current.VendorID || this.Item.Current.PreferredVendorLocationID != current.VendorLocationID) ||
                        (current.IsDefault != true && this.Item.Current.PreferredVendorID == current.VendorID && this.Item.Current.PreferredVendorLocationID == current.VendorLocationID)))
				{
                    InventoryItem upd = this.Item.Current;
					upd.PreferredVendorID = current.IsDefault == true ? current.VendorID : null;
					upd.PreferredVendorLocationID = current.IsDefault == true ? current.VendorLocationID : null;
					Item.Update(upd);
				if (current.IsDefault == true)
				{
					foreach (POVendorInventory vendorInventory in VendorItems.Select())
					{
						if (vendorInventory.RecordID != current.RecordID && vendorInventory.IsDefault == true)
							VendorItems.Cache.SetValue<POVendorInventory.isDefault>(vendorInventory, false);
					}
					this.VendorItems.Cache.ClearQueryCache();
					this.VendorItems.View.RequestRefresh();
				}
			}

					}

        protected virtual void POVendorInventory_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
        {
            InventoryItem upd = PXCache<InventoryItem>.CreateCopy(this.Item.Current);
            POVendorInventory vendor = e.Row as POVendorInventory;
            object isdefault = cache.GetValueExt<POVendorInventory.isDefault>(e.Row);
            if (isdefault is PXFieldState)
            {
                isdefault = ((PXFieldState)isdefault).Value;
            }
			if ((bool?)isdefault == true)
			{
				upd.PreferredVendorID = null;
				upd.PreferredVendorLocationID = null;
				this.Item.Update(upd);
			}
			}	

		protected virtual void InventoryItem_ValMethod_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((InventoryItem)e.Row).ValMethod != null && string.Equals(((InventoryItem)e.Row).ValMethod, (string)e.NewValue) == false)
			{
				INCostStatus coststatus = (INCostStatus)PXSelect<INCostStatus, Where<INCostStatus.inventoryID, Equal<Current<InventoryItem.inventoryID>>, And<INCostStatus.qtyOnHand, NotEqual<decimal0>>>>.Select(this);

				if (coststatus != null)
				{
					if (((InventoryItem)e.Row).ValMethod == INValMethod.Specific ||
							(string)e.NewValue == INValMethod.Specific ||
							(string)e.NewValue == INValMethod.Standard ||
                            ((InventoryItem)e.Row).ValMethod == INValMethod.FIFO)
					{
                        var attrlist = sender.GetAttributesReadonly(e.Row, "ValMethod");
                        PXStringListAttribute listattr = (PXStringListAttribute)attrlist.First(
                            (PXEventSubscriberAttribute attr) => { return attr is INValMethod.ListAttribute; });
                        string oldval, newval;
                        listattr.ValueLabelDic.TryGetValue(((InventoryItem)e.Row).ValMethod, out oldval);
                        listattr.ValueLabelDic.TryGetValue((string)e.NewValue, out newval);
                        throw new PXSetPropertyException(Messages.ValMethodCannotBeChanged, oldval, newval);
					}					
					sender.RaiseExceptionHandling<InventoryItem.valMethod>
							(e.Row, e.NewValue, new PXSetPropertyException(Messages.ValMethodChanged, PXErrorLevel.Warning));
				}
			}
		}

		protected virtual void InventoryItem_IsSplitted_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;
			if (row != null)
			{
				if (row.IsSplitted == false)
				{
					foreach (INComponent c in Components.Select())
					{
						Components.Delete(c);
					}

					row.TotalPercentage = 100;
				}
				else
					row.TotalPercentage = 0;
			}
		}

		protected virtual void InventoryItem_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
            var row = e.Row as InventoryItem;
            if (row == null)
                return;
            INItemPlan itemplan = itemplans.SelectSingle();
            if (itemplan != null)
			{
				EntityHelper helper = new EntityHelper(this);
                throw new PXException(Messages.ItemIsUsed, row.InventoryCD, helper.GetEntityRowID(itemplan.RefNoteID).Trim());
			}

            var sitestat = nonemptysitestatuses.SelectSingle();

            if (sitestat != null)
                throw new PXException(Messages.ItemHasStockRemainder, row.InventoryCD, insitestatus.GetValueExt<INSiteStatus.siteID>(sitestat));

            CheckExistence<RQ.RQRequestLine, RQ.RQRequestLine.inventoryID>(row, Messages.ItemHasRequest, typeof(RQ.RQRequestLine.orderNbr).Name);
            CheckExistence<RQ.RQRequisitionLine, RQ.RQRequisitionLine.inventoryID>(row, Messages.ItemHasRequisition, typeof(RQ.RQRequisitionLine.reqNbr).Name);
            CheckExistence<AP.APTran, AP.APTran.inventoryID>(row, Messages.ItemHasAPBill, typeof(AP.APTran.refNbr).Name);
            CheckExistence<INPIDetail, INPIDetail.inventoryID>(row, Messages.ItemHasPhysicalInventoryReview, typeof(INPIDetail.pIID).Name);
            CheckExistence<PM.PMTran, PM.PMTran.inventoryID>(row, Messages.ItemHasProjectTransactions, typeof(PM.PMTran.refNbr).Name);
            CheckExistence<POLine, POLine.inventoryID>(row, Messages.ItemHasPurchaseOrders, typeof(POLine.orderType).Name, typeof(POLine.orderNbr).Name);
            CheckExistence<SOLine, SOLine.inventoryID>(row, Messages.ItemHasSalesOrders, typeof(SOLine.orderType).Name, typeof(SOLine.orderNbr).Name);
            CheckExistence<INKitSpecStkDet, INKitSpecStkDet.compInventoryID>(row, Messages.ItemHasKitSpecifications);
		}

        public void CheckExistence<TEntity, TInventoryField>(InventoryItem ii, String errorMsg, params String[] plist)
            where TEntity : class, IBqlTable, new()
            where TInventoryField: IBqlField
        {
            TEntity ent = (TEntity)PXSelect<TEntity, Where<TInventoryField, Equal<Current<InventoryItem.inventoryID>>>>.SelectSingleBound(this, new object[] { ii });

            if (ent != null)
            {
                var paramlist = new List<String>() { ii.InventoryCD };

                foreach (var p in plist)
                {
                    paramlist.Add(this.Caches[typeof(TEntity)].GetValueExt(ent, p).ToString());
                }
                throw new PXException(errorMsg, paramlist.ToArray());
            }
        }

		protected virtual void InventoryItem_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (e.Row == null) { return; }

			InventoryItem ii_rec = (InventoryItem)e.Row;

			// deleting only inventory-specific uoms
			foreach (INUnit inunit_rec in PXSelect<INUnit, Where<INUnit.unitType, Equal<short1>, And<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>>>>.Select(this, ii_rec.InventoryID))
			{
				itemunits.Delete(inunit_rec);
			}
		}

		protected virtual void InventoryItem_PackageOption_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;
			if (row == null) return;

            if (e.NewValue.ToString() == INPackageOption.Quantity && Boxes.Select().Count == 0)
			{
				sender.RaiseExceptionHandling<InventoryItem.packageOption>(row, e.NewValue,
				                                   new PXSetPropertyException(Messages.BoxesRequired, PXErrorLevel.Warning));
			}
			
		}

		protected virtual void InventoryItem_PackageOption_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;
			if (row == null) return;

			if (row.PackageOption == INPackageOption.Quantity)
			{
				row.PackSeparately = true;
			}
			else if (row.PackageOption == INPackageOption.Manual)
			{
				row.PackSeparately = false;

				foreach (INItemBoxEx box in Boxes.Select())
				{
					Boxes.Delete(box);
				}
			}
			else if (row.PackageOption == INPackageOption.WeightAndVolume)
			{
				row.PackSeparately = false;
			}
		}

		#endregion

		#region INItemXRef Event Handlers

		protected virtual void INItemXRef_InventoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Item.Current != null)
			{
				e.NewValue = Item.Current.InventoryID;
				e.Cancel = true;
			}
		}

		protected virtual void INItemXRef_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void INItemXRef_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				if (((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
				{
					((INItemXRef)e.Row).BAccountID = (int)0;
				}
				((INItemXRef)e.Row).InventoryID = Item.Current.InventoryID;
				sender.Normalize();
			}
		}
		
		protected virtual void INItemXRef_BAccountID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null && (int?)e.ReturnValue == 0 && ((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
			{
				e.ReturnValue = null;
				e.Cancel = true;
			}
		}

        protected virtual void INItemXRef_BAccountID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (e.Row != null && ((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
			{
				e.NewValue = (int)0;
				e.Cancel = true;
			}
		}
		
		protected virtual void INItemXRef_BAccountID_ExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs e)
		{
			if (e.Row != null && ((INItemXRef)e.Row).BAccountID == null && (e.NewValue is int && ((int)e.NewValue) == 0 || e.NewValue is string && ((string)e.NewValue) == "0"))
			{
				((INItemXRef)e.Row).BAccountID = 0;
				e.Cancel = true;
			}
		}

		protected virtual void INItemXRef_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
			{
				e.Cancel = true;
			}		
		}
        protected virtual void INItemXRef_AlternateID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            var row = e.Row as INItemXRef;
             
            if (row == null || Item.Current == null || row.AlternateID == null)
                return;

            if(!PXDimensionAttribute.MatchMask<InventoryItem.inventoryCD>(Item.Cache, row.AlternateID))
            {
                sender.RaiseExceptionHandling<INItemXRef.alternateID>(row, row.AlternateID, new PXSetPropertyException(Messages.AlternateIDDoesNotCorrelateWithCurrentSegmentRules, PXErrorLevel.Warning));
            }
        }
        #endregion

        #region INItemSite Event Handlers

        protected virtual void INItemSite_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			if ((bool)((INItemSite)e.Row).IsDefault && ((INItemSite)e.NewRow).IsDefault == false)
			{
				((INItemSite)e.NewRow).IsDefault = true;
			}
		}

		protected virtual void INItemSite_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if ((bool)((INItemSite)e.Row).IsDefault)
			{
				SetSiteDefault(sender, e);
			}

			INItemSite row = e.Row as INItemSite;

			if (row != null && insetup.Current.UseInventorySubItem != true && row.InventoryID != null && row.SiteID != null)
			{
				PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus sitem = new Overrides.INDocumentRelease.SiteStatus();
				sitem.InventoryID = row.InventoryID;
				sitem.SiteID = row.SiteID;
				sitestatus.Insert(sitem);
			}
		}

		protected virtual void INItemSite_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (((INItemSite)e.OldRow).IsDefault == false && (bool)((INItemSite)e.Row).IsDefault)
			{
				SetSiteDefault(sender, e);
			}
		}

		protected virtual void INItemSite_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INItemSite row = (INItemSite)e.Row;
			if (e.Row != null)
			{
				bool isTransfer = (row != null) && INReplenishmentSource.IsTransfer(row.ReplenishmentSource);
				if (isTransfer && row.ReplenishmentSourceSiteID == row.SiteID)
				{
					sender.RaiseExceptionHandling<INItemSite.replenishmentSourceSiteID>(e.Row, row.ReplenishmentSourceSiteID, new PXSetPropertyException(Messages.ReplenishmentSourceSiteMustBeDifferentFromCurrenSite, PXErrorLevel.Warning));
				}
				else
				{
					sender.RaiseExceptionHandling<INItemSite.replenishmentSourceSiteID>(e.Row, row.ReplenishmentSourceSiteID, null);
				}
			}

			if (row != null && row.InvtAcctID == null)
			{
				INSite insite = PXSelectReadonly<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, row.SiteID);

				try
				{
					INItemSiteMaint.DefaultInvtAcctSub(this, row, Item.Current, insite, postclass.Current);
				}
				catch (PXMaskArgumentException) { }
			}
		}

		public virtual void INItemSite_InvtAcctID_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				if (((INItemSite)e.Row).OverrideInvtAcctSub != true)
				{
					e.FieldName = string.Empty;
					e.Cancel = true;
				}
			}
		}

		public virtual void INItemSite_InvtSubID_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				if (((INItemSite)e.Row).OverrideInvtAcctSub != true)
				{
					e.FieldName = string.Empty;
					e.Cancel = true;
				}
			}
		}


		#endregion

		#region RelationGroup Event Handlers

		protected virtual void RelationGroup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PX.SM.RelationGroup group = e.Row as PX.SM.RelationGroup;
			if (Item.Current != null && group != null && Groups.Cache.GetStatus(group) == PXEntryStatus.Notchanged)
			{
				group.Included = PX.SM.UserAccess.IsIncluded(Item.Current.GroupMask, group);
			}
		}

		protected virtual void RelationGroup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion

		#region INComponent Event Handles

		protected virtual void INComponent_Percentage_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null && row.AmtOption == INAmountOption.Percentage)
			{
				row.Percentage = GetRemainingPercentage();
			}
		}

		protected virtual void INComponent_ComponentID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.ComponentID);
				var deferralCode = (DRDeferredCode)PXSelectorAttribute.Select<InventoryItem.deferredCode>(Item.Cache, item);
				bool useDeferralFromItem = deferralCode != null && deferralCode.MultiDeliverableArrangement != true;

				if (item != null)
				{
					row.SalesAcctID = item.SalesAcctID;
					row.SalesSubID = item.SalesSubID;
					row.UOM = item.SalesUnit;
					row.DeferredCode = useDeferralFromItem ? item.DeferredCode : null;
					row.DefaultTerm = item.DefaultTerm;
					row.DefaultTermUOM = item.DefaultTermUOM;
				}
			}
		}

		protected virtual void INComponent_AmtOption_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null)
			{
				if (row.AmtOption == INAmountOption.Percentage)
				{
					row.FixedAmt = null;
					row.Percentage = GetRemainingPercentage();
				}
				else
				{
					row.Percentage = 0;
				}

				if (row.AmtOption == INAmountOption.Residual)
				{
					sender.SetValueExt<INComponent.deferredCode>(row, null);
					sender.SetDefaultExt<INComponent.fixedAmt>(row);
					sender.SetDefaultExt<INComponent.percentage>(row);
				}
			}
		}

		protected virtual void INComponent_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var component = e.Row as INComponent;
			SetComponentControlsState(sender, component);

			InventoryHelper.CheckZeroDefaultTerm<INComponent.deferredCode, INComponent.defaultTerm>(sender, component);
		}

		#endregion
		
		#region INItemRep Event Handler
		protected virtual void INItemRep_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INItemRep row = e.Row as INItemRep;
			if (row != null)
			{
				bool isTransfer = INReplenishmentSource.IsTransfer(row.ReplenishmentSource);
				PXUIFieldAttribute.SetEnabled<INItemRep.replenishmentMethod>(sender, e.Row, row.ReplenishmentSource != INReplenishmentSource.PurchaseToOrder && row.ReplenishmentSource != INReplenishmentSource.DropShipToOrder);
                PXUIFieldAttribute.SetEnabled<INItemRep.replenishmentSourceSiteID>(sender, e.Row, row.ReplenishmentSource == INReplenishmentSource.PurchaseToOrder || row.ReplenishmentSource == INReplenishmentSource.DropShipToOrder || row.ReplenishmentSource == INReplenishmentSource.Transfer || row.ReplenishmentSource == INReplenishmentSource.Purchased);
				PXUIFieldAttribute.SetEnabled<INItemRep.maxShelfLife>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.launchDate>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.terminationDate>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.serviceLevelPct>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.safetyStock>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.minQty>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.maxQty>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.forecastModelType>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.forecastPeriodType>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.historyDepth>(sender, e.Row, row.ReplenishmentMethod != INReplenishmentMethod.None);
				PXUIFieldAttribute.SetEnabled<INItemRep.transferERQ>(sender, e.Row, isTransfer && row.ReplenishmentMethod == INReplenishmentMethod.FixedReorder);
				PXUIFieldAttribute.SetEnabled<INSubItemRep.transferERQ>(this.subreplenishment.Cache, null, isTransfer && row.ReplenishmentMethod == INReplenishmentMethod.FixedReorder);
			}
			this.subreplenishment.Cache.AllowInsert =
					e.Row != null && (string.IsNullOrEmpty(row.ReplenishmentClassID) == false) && insetup.Current.UseInventorySubItem == true;
		}

		protected virtual void INItemRep_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			INItemRep r = e.Row as INItemRep;
            if (r != null && r.ReplenishmentClassID != null)
				UpdateItemSiteReplenishment(r);
		}

		protected virtual void INItemRep_ReplenishmentSource_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemRep r = e.Row as INItemRep;
			if (r == null) return;
			if (r.ReplenishmentSource == INReplenishmentSource.PurchaseToOrder || r.ReplenishmentSource == INReplenishmentSource.DropShipToOrder)
			{
				sender.SetValueExt<INItemRep.replenishmentMethod>(r, INReplenishmentMethod.None);
			}
			if (r.ReplenishmentSource != INReplenishmentSource.PurchaseToOrder && r.ReplenishmentSource != INReplenishmentSource.DropShipToOrder && r.ReplenishmentSource != INReplenishmentSource.Transfer)
			{
				sender.SetDefaultExt<INItemRep.replenishmentSourceSiteID>(r);
			}
		}

		protected virtual void INItemRep_ReplenishmentMethod_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INItemRep r = e.Row as INItemRep;
			if (r == null) return;
			if (r.ReplenishmentMethod == INReplenishmentMethod.None)
			{
				sender.SetDefaultExt<INItemRep.maxShelfLife>(e.Row);
				sender.SetDefaultExt<INItemRep.launchDate>(e.Row);
				sender.SetDefaultExt<INItemRep.terminationDate>(e.Row);
				sender.SetDefaultExt<INItemRep.serviceLevelPct>(e.Row);
				sender.SetDefaultExt<INItemRep.safetyStock>(e.Row);
				sender.SetDefaultExt<INItemRep.minQty>(e.Row);
				sender.SetDefaultExt<INItemRep.maxQty>(e.Row);
				sender.SetDefaultExt<INItemRep.forecastModelType>(e.Row);
				sender.SetDefaultExt<INItemRep.forecastPeriodType>(e.Row);
				sender.SetDefaultExt<INItemRep.historyDepth>(e.Row);
			}
		}

		protected virtual void INItemRep_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			INItemRep r = e.Row as INItemRep;
			if (r == null) return;
			if (INReplenishmentSource.IsTransfer(r.ReplenishmentSource) == false)
			{
				r.ReplenishmentSourceSiteID = null;
			}
			UpdateItemSiteReplenishment(r);

		}
		protected virtual void INItemRep_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			INItemRep r = e.Row as INItemRep;
			if (r == null) return;
			INItemRep def = new INItemRep();
			def.ReplenishmentClassID = r.ReplenishmentClassID;
			UpdateItemSiteReplenishment(def);
		}
		private void UpdateItemSiteReplenishment(INItemRep r)
		{
			foreach (PXResult<INItemSite, INSite> rec in itemsiterecords.Select())
			{
				INItemSite itemsite = rec;
				INSite site = rec;
				if (itemsite.ReplenishmentPolicyOverride == true ||
					(itemsite.ReplenishmentClassID == site.ReplenishmentClassID &&
					 itemsite.ReplenishmentClassID != r.ReplenishmentClassID)) continue;

				bool IsUpdateFlag = false;
				if (itemsite.ReplenishmentPolicyOverride == false)
				{
					itemsite.ReplenishmentClassID = r.ReplenishmentClassID;
					itemsite.ReplenishmentPolicyID = r.ReplenishmentPolicyID;
					itemsite.ReplenishmentSource = r.ReplenishmentSource;
					itemsite.ReplenishmentSourceSiteID = r.ReplenishmentSourceSiteID;
					itemsite.ReplenishmentMethod = r.ReplenishmentMethod;
					IsUpdateFlag = true;
				}
				if (itemsite.MaxShelfLifeOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.MaxShelfLife = r.MaxShelfLife; IsUpdateFlag = true; }
				if (itemsite.LaunchDateOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.LaunchDate = r.LaunchDate; IsUpdateFlag = true; }
				if (itemsite.TerminationDateOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.TerminationDate = r.TerminationDate; IsUpdateFlag = true; }
				if (itemsite.SafetyStockOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.SafetyStock = r.SafetyStock; IsUpdateFlag = true; }
				if (itemsite.MinQtyOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.MinQty = r.MinQty; IsUpdateFlag = true; }
				if (itemsite.MaxQtyOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.MaxQty = r.MaxQty; IsUpdateFlag = true; }
				if (itemsite.TransferERQOverride == false || itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Inserted)
				{ itemsite.TransferERQ = r.TransferERQ; IsUpdateFlag = true; }

				if (IsUpdateFlag && itemsiterecords.Cache.GetStatus(itemsite) == PXEntryStatus.Notchanged)
				{
					itemsiterecords.Cache.SetStatus(itemsite, PXEntryStatus.Updated);
				}
			}
		}
		#endregion

		protected virtual void INItemBoxEx_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INItemBoxEx row = e.Row as INItemBoxEx;
			if (row == null) return;

			if (Item.Current == null) return;
			
			if (Item.Current.PackageOption == INPackageOption.Weight || Item.Current.PackageOption == INPackageOption.WeightAndVolume)
			{
				row.MaxQty = CalculateMaxQtyInBox(Item.Current, row);
			}
		}

		protected virtual void INItemBoxEx_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			INItemBoxEx row = e.Row as INItemBoxEx;
			if (row == null) return;

			CSBox box = PXSelect<CSBox, Where<CSBox.boxID, Equal<Current<INItemBoxEx.boxID>>>>.Select(this);
			if (box != null)
			{
				row.MaxWeight = box.MaxWeight;
				row.MaxVolume = box.MaxVolume;
				row.BoxWeight = box.BoxWeight;
				row.Description = box.Description;
			}

			if (Item.Current.PackageOption == INPackageOption.Weight || Item.Current.PackageOption == INPackageOption.WeightAndVolume)
			{
				row.MaxQty = CalculateMaxQtyInBox(Item.Current, row);
			}
		}

		protected virtual void INUnit_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INUnit row = (INUnit) e.Row;
			PXFieldState state = (PXFieldState)sender.GetStateExt<INUnit.fromUnit>(row);
			uint msgNbr;
			String msqPrefix;
			if (Item.Current != null && row.ToUnit == Item.Current.BaseUnit && (state.Error == null || state.Error == PXMessages.Localize(Messages.BaseUnitNotSmallest, out msgNbr, out msqPrefix) || state.ErrorLevel == PXErrorLevel.RowInfo))
			{
				if (row.UnitMultDiv == MultDiv.Multiply && row.UnitRate < 1 || row.UnitMultDiv == MultDiv.Divide && row.UnitRate > 1)
					sender.RaiseExceptionHandling<INUnit.fromUnit>(row, row.FromUnit, new PXSetPropertyException(Messages.BaseUnitNotSmallest, PXErrorLevel.RowWarning));
				else
					sender.RaiseExceptionHandling<INUnit.fromUnit>(row, row.FromUnit, null);
			}
		}

		protected virtual void POVendorInventory_PurchaseUnit_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POVendorInventory row = e.Row as POVendorInventory;
			if (row == null) return;

			foreach(INUnit unit in this.Caches[typeof(INUnit)].Inserted)
			{
				if ( unit.UnitType == INUnitType.InventoryItem && unit.InventoryID == row.InventoryID && string.Equals(unit.FromUnit, (string)e.NewValue, StringComparison.InvariantCultureIgnoreCase))
				{
					e.Cancel = true;
				}
			}
		}

		private decimal GetRemainingPercentage()
		{
			decimal result = 100;

			foreach (INComponent comp in Components.Select())
			{
				if (comp.AmtOption == INAmountOption.Percentage)
				result -= (comp.Percentage ?? 0);
			}

			if (result > 0)
				return result;
			else
				return 0;
		}

		private decimal SumComponentsPercentage()
		{
			decimal result = 0;

			foreach (INComponent comp in Components.Select())
			{
				if (comp.AmtOption == INAmountOption.Percentage)
				result += (comp.Percentage ?? 0);
			}

			return result;
		}

		protected virtual void AppendGroupMask(string itemClassID, bool clear)
		{
			if (!String.IsNullOrEmpty(itemClassID))
			{
				INItemClass ic = PXSelect<INItemClass,
					Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>
					.Select(this, itemClassID);
				if (ic != null && ic.GroupMask != null)
				{
					if (clear)
					{
						Groups.Cache.Clear();
					}
					foreach (PX.SM.RelationGroup group in Groups.Select())
					{
						for (int i = 0; i < group.GroupMask.Length && i < ic.GroupMask.Length; i++)
						{
							if (group.Included != true && group.GroupMask[i] != 0x00 && (ic.GroupMask[i] & group.GroupMask[i]) == group.GroupMask[i])
							{
								group.Included = true;
								if (Groups.Cache.GetStatus(group) == PXEntryStatus.Notchanged || Groups.Cache.GetStatus(group) == PXEntryStatus.Held)
								{
									Groups.Cache.SetStatus(group, PXEntryStatus.Updated);
								}
								Groups.Cache.IsDirty = true;
								break;
							}
						}
					}
				}
			}
		}

		protected bool _JustInserted;
		public override bool IsDirty
		{
			get
			{
				if (_JustInserted && !IsContractBasedAPI)
				{
					return false;
				}
				return base.IsDirty;
			}
		}

		protected virtual void SetSiteDefault(PXCache sender, PXRowUpdatedEventArgs e)
		{
			InventoryItem item = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, ((INItemSite)e.Row).InventoryID);

			if (item != null)
			{
				item.DfltSiteID = ((INItemSite)e.Row).SiteID;
				item.DfltReceiptLocationID = ((INItemSite)e.Row).DfltReceiptLocationID;
				item.DfltShipLocationID = ((INItemSite)e.Row).DfltShipLocationID;

				if (Item.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
				{
					Item.Cache.SetStatus(item, PXEntryStatus.Updated);
				}
			}

			bool IsRefreshNeeded = false;

			foreach (INItemSite rec in itemsiterecords.Select())
			{
				if (object.Equals(rec.SiteID, ((INItemSite)e.Row).SiteID) == false && (bool)rec.IsDefault)
				{
					rec.IsDefault = false;
					if (itemsiterecords.Cache.GetStatus(rec) == PXEntryStatus.Notchanged)
					{
					itemsiterecords.Cache.SetStatus(rec, PXEntryStatus.Updated);
					}

					IsRefreshNeeded = true;
				}
			}

			if (IsRefreshNeeded)
			{
				itemsiterecords.View.RequestRefresh();
			}
		}

		protected virtual void SetSiteDefault(PXCache sender, PXRowInsertedEventArgs e)
		{
			SetSiteDefault(sender, new PXRowUpdatedEventArgs(e.Row, null, e.ExternalCall));
		}

		public override void Persist()
		{
			if (Item.Current != null && Groups.Cache.IsDirty)
			{
				PX.SM.UserAccess.PopulateNeighbours<InventoryItem>(Item.Cache, Item.Current,
					new PXDataFieldValue[] {
						new PXDataFieldValue(typeof(InventoryItem.inventoryID).Name, PXDbType.Int, 4, Item.Current.InventoryID, PXComp.NE),
						new PXDataFieldValue(typeof(InventoryItem.stkItem).Name, PXDbType.Bit, 1, 1, PXComp.EQ)
					},
					Groups,
					typeof(SegmentValue));
				PXSelectorAttribute.ClearGlobalCache<InventoryItem>();
			}

			base.Persist();
			Groups.Cache.Clear();
			GroupHelper.Clear();
		}

		protected virtual void ValidatePackaging(InventoryItem row)
		{
			PXUIFieldAttribute.SetError<InventoryItem.weightUOM>(Item.Cache, row, null);
			PXUIFieldAttribute.SetError<InventoryItem.baseItemWeight>(Item.Cache, row, null);
			PXUIFieldAttribute.SetError<InventoryItem.volumeUOM>(Item.Cache, row, null);
			PXUIFieldAttribute.SetError<InventoryItem.baseItemVolume>(Item.Cache, row, null);
			
			//validate weight & volume:
			if (row.PackageOption == INPackageOption.Weight || row.PackageOption == INPackageOption.WeightAndVolume)
			{
				if (string.IsNullOrEmpty(row.WeightUOM))
					Item.Cache.RaiseExceptionHandling<InventoryItem.weightUOM>(row, row.WeightUOM, new PXSetPropertyException(Messages.ValueIsRequiredForAutoPackage, PXErrorLevel.Warning));

				if (row.BaseItemWeight <= 0)
					Item.Cache.RaiseExceptionHandling<InventoryItem.baseItemWeight>(row, row.BaseItemWeight, new PXSetPropertyException(Messages.ValueIsRequiredForAutoPackage, PXErrorLevel.Warning));

				if (row.PackageOption == INPackageOption.WeightAndVolume)
				{
					if (string.IsNullOrEmpty(row.VolumeUOM))
						Item.Cache.RaiseExceptionHandling<InventoryItem.volumeUOM>(row, row.VolumeUOM, new PXSetPropertyException(Messages.ValueIsRequiredForAutoPackage, PXErrorLevel.Warning));

					if (row.BaseItemVolume <= 0)
						Item.Cache.RaiseExceptionHandling<InventoryItem.baseItemVolume>(row, row.BaseItemVolume, new PXSetPropertyException(Messages.ValueIsRequiredForAutoPackage, PXErrorLevel.Warning));
				}
			}

			//validate boxes:
			foreach (INItemBoxEx box in Boxes.Select())
			{
				PXUIFieldAttribute.SetError<INItemBoxEx.boxID>(Boxes.Cache, box, null);
				PXUIFieldAttribute.SetError<INItemBoxEx.maxQty>(Boxes.Cache, box, null);

				if ((row.PackageOption == INPackageOption.Weight || row.PackageOption == INPackageOption.WeightAndVolume) && box.MaxWeight.GetValueOrDefault() == 0)
				{
					Boxes.Cache.RaiseExceptionHandling<INItemBoxEx.boxID>(box, box.BoxID, new PXSetPropertyException(Messages.MaxWeightIsNotDefined, PXErrorLevel.Warning));
				}

				if (row.PackageOption == INPackageOption.WeightAndVolume && box.MaxVolume.GetValueOrDefault() == 0)
				{
					Boxes.Cache.RaiseExceptionHandling<INItemBoxEx.boxID>(box, box.BoxID, new PXSetPropertyException(Messages.MaxVolumeIsNotDefined, PXErrorLevel.Warning));
				}

				if ((row.PackageOption == INPackageOption.Weight || row.PackageOption == INPackageOption.WeightAndVolume) && 
					(box.MaxWeight.GetValueOrDefault() < row.BaseItemWeight.GetValueOrDefault() ||
					(box.MaxVolume > 0 && row.BaseItemVolume > box.MaxVolume)))
				{
					Boxes.Cache.RaiseExceptionHandling<INItemBoxEx.boxID>(box, box.BoxID, new PXSetPropertyException(Messages.ItemDontFitInTheBox, PXErrorLevel.Warning));
				}

			}
		}

		#region Actions

		public PXAction<InventoryItem> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2, 3 }, new string[] 
			{ 
					"Update Price",
					"Update Cost",
                    "View Restriction Group"                    

			})]
			int? actionID
			)
		{
			switch (actionID)
			{
				case 2:
					if (ItemSettings.Current != null && ItemSettings.Current.PendingStdCostDate != null)
					{
						if (ItemSettings.Current.ValMethod == INValMethod.Standard)
						{
							INCostStatus layer = PXSelect<INCostStatus, Where<INCostStatus.inventoryID, Equal<Current<InventoryItem.inventoryID>>,
								And<INCostStatus.qtyOnHand, NotEqual<decimal0>>>>.SelectWindowed(this, 0, 1);

							if (layer == null)
							{
								InventoryItem old_row = PXCache<InventoryItem>.CreateCopy(ItemSettings.Current);

								ItemSettings.Current.LastStdCost = ItemSettings.Current.StdCost;
								ItemSettings.Current.StdCostDate = ItemSettings.Current.PendingStdCostDate.GetValueOrDefault((DateTime)this.Accessinfo.BusinessDate);
								ItemSettings.Current.StdCost = ItemSettings.Current.PendingStdCost;
								ItemSettings.Current.PendingStdCost = 0;
								ItemSettings.Current.PendingStdCostDate = null;

								ItemSettings.Cache.Update(ItemSettings.Current);

								this.Save.Press();
							}
							else
							{
								throw new PXException(Messages.QtyOnHandExists);
							}
						}
					}
					break;
				case 3:
					if (Item.Current != null)
					{
						INAccessDetailByItem graph = CreateInstance<INAccessDetailByItem>();
						graph.Item.Current = graph.Item.Search<InventoryItem.inventoryCD>(Item.Current.InventoryCD);
						throw new PXRedirectRequiredException(graph, false, "Restricted Groups");
					}
					break;
			}
			return adapter.Get();
		}


		public PXAction<InventoryItem> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2, 3, 4, 5, 6, 7 }, new string[] 
			{ 
					"Summary", 
					"Allocation Details", 
					"Transaction Summary", 
					"Transaction Details", 
					"Transaction History" ,
					"Sales Prices",
					"Vendor Prices"
			})]
			int? inquiryID
			)
		{
			switch (inquiryID)
			{
				case 1:
					if (Item.Current != null)
					{
						InventorySummaryEnq graph = PXGraph.CreateInstance<InventorySummaryEnq>();
						graph.Filter.Current.InventoryID = Item.Current.InventoryID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Inventory Summary");
					}
					break;
				case 2:
					if (Item.Current != null)
					{
						InventoryAllocDetEnq graph = PXGraph.CreateInstance<InventoryAllocDetEnq>();
						graph.Filter.Current.InventoryID = Item.Current.InventoryID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Inventory Allocation Details");
					}
					break;
				case 3:
					if (Item.Current != null)
					{
						InventoryTranSumEnq graph = PXGraph.CreateInstance<InventoryTranSumEnq>();
						graph.Filter.Current.InventoryID = Item.Current.InventoryID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Inventory Transaction Summary");
					}
					break;
				case 4:
					if (Item.Current != null)
					{
						InventoryTranDetEnq graph = PXGraph.CreateInstance<InventoryTranDetEnq>();
						graph.Filter.Current.InventoryID = Item.Current.InventoryID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Inventory Transaction Details");
					}
					break;
				case 5:
					if (Item.Current != null)
					{
						InventoryTranHistEnq graph = PXGraph.CreateInstance<InventoryTranHistEnq>();
						graph.Filter.Current.InventoryID = Item.Current.InventoryID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Inventory Transaction History");
					}
					break;
                case 6:
                    if (Item.Current != null)
                    {
                        ARSalesPriceMaint graph = PXGraph.CreateInstance<ARSalesPriceMaint>();
                        graph.Filter.Current.InventoryID = Item.Current.InventoryID;
                        throw new PXRedirectRequiredException(graph, "Sales Prices");
                    }
                    break;
                case 7:
                    if (Item.Current != null)
                    {
                        APVendorPriceMaint graph = PXGraph.CreateInstance<APVendorPriceMaint>();
                        graph.Filter.Current.InventoryID = Item.Current.InventoryID;
                        throw new PXRedirectRequiredException(graph, "Vendor Prices");
                    }
                    break;

			}
			return adapter.Get();
		}

		public PXAction<InventoryItem> addWarehouseDetail;
		[PXUIField(DisplayName = "Add Warehouse Detail", MapEnableRights = PXCacheRights.Select)]
		[PXInsertButton]
		protected virtual IEnumerable AddWarehouseDetail(PXAdapter adapter)
		{
            foreach (InventoryItem item in adapter.Get())
			{
				if (item.InventoryID > 0)
				{
					INItemSiteMaint maint = PXGraph.CreateInstance<INItemSiteMaint>();
					PXCache cache = maint.itemsiterecord.Cache;
					IN.INItemSite rec = (IN.INItemSite)cache.CreateCopy(cache.Insert());
					rec.InventoryID = item.InventoryID;
					cache.Update(rec);
					cache.IsDirty = false;
					throw new PXRedirectRequiredException(maint, "Add Warehouse Detail");
				}
				yield return item;
			}
		}
		public PXAction<InventoryItem> updateReplenishment;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = "Reset to Default", MapEnableRights = PXCacheRights.Update)]
		protected virtual IEnumerable UpdateReplenishment(PXAdapter adapter)
		{
			if (this.replenishment.Current != null && insetup.Current.UseInventorySubItem == true) 
                foreach (INSubItemRep rep in this.subreplenishment.Select())
				{
					INSubItemRep upd = PXCache<INSubItemRep>.CreateCopy(rep);
					upd.SafetyStock = this.replenishment.Current.SafetyStock;
					upd.MinQty = this.replenishment.Current.MinQty;
					upd.MaxQty = this.replenishment.Current.MaxQty;
					this.subreplenishment.Update(upd);
				}				
			return adapter.Get();			
		}

		public PXAction<InventoryItem> generateSubitems;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
		[PXUIField(DisplayName = "Generate Subitems", MapEnableRights = PXCacheRights.Update)]
		protected virtual IEnumerable GenerateSubitems(PXAdapter adapter)
		{
			if (this.replenishment.Current != null && insetup.Current.UseInventorySubItem == true)
			{
				PXResultset<Segment> r = PXSelect<Segment,
					Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>>
					.Select(this, SubItemAttribute.DimensionName);
				List<string>[] values = new List<string>[r.Count];				
				int segments = 0;
				int counts = 0;
                foreach (Segment s in r)
				{					
					values[segments] = new List<string>();
                    foreach (SegmentValue v in PXSelect<SegmentValue,
						Where<SegmentValue.dimensionID, Equal<Required<SegmentValue.dimensionID>>,
							And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>>>>
							.Select(this, s.DimensionID, s.SegmentID))
					{
						values[segments].Add(v.Value);						
						counts += 1;
					}					
					segments += 1;					
				}
				int[] index = new int[segments];				

                for (int c = 0; c < counts; c++)
				{
					StringBuilder subitem = new StringBuilder(); 
					for (int i = 0; i < segments; i++)
					{
						subitem.Append(values[i][index[i]]);
					}
                    for (int k = segments - 1; k >= 0; k--)
					{
						index[k] += 1;
                        if (index[k] < values[k].Count)
							break;
						index[k] = 0;
					}
					INSubItemRep ins = new INSubItemRep();
					ins.InventoryID = this.Item.Current.InventoryID;
					ins.ReplenishmentClassID = this.replenishment.Current.ReplenishmentClassID;
					this.subreplenishment.SetValueExt<INSubItemRep.subItemID>(ins, subitem.ToString());
					this.subreplenishment.Insert(ins);
				}				
			}
			return adapter.Get();
		}

		public PXAction<InventoryItem> generateLotSerialNumber;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
		[PXUIField(DisplayName = "Generate Lot/Serial Number", Visible = false)]
		protected virtual IEnumerable GenerateLotSerialNumber(PXAdapter adapter)
		{
			foreach (InventoryItem item in adapter.Get())
			{
				item.LotSerNumberResult = INLotSerialNbrAttribute.GetNextNumber(adapter.View.Cache, item.InventoryID.GetValueOrDefault());
				yield return item;
			}
		}
		#endregion

		public static void Redirect(int? inventoryID)
		{
			Redirect(inventoryID, false);
		}

		public static void Redirect(int? inventoryID, bool newWindow)
		{
			InventoryItemMaint graph = PXGraph.CreateInstance<InventoryItemMaint>();
			graph.Item.Current = graph.Item.Search<InventoryItem.inventoryID>(inventoryID);
			if (graph.Item.Current != null)
			{
                if (newWindow)
                    throw new PXRedirectRequiredException(graph, true, Messages.InventoryItem) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				else
					throw new PXRedirectRequiredException(graph, Messages.InventoryItem);
			}
		}

		#region Private members
		private bool doResetDefaultsOnItemClassChange;
		#endregion

		private string SalesPriceUpdateUnit
		{
			get
			{
				return SalesPriceUpdateUnitType.BaseUnit;
			}
		}

        public PXAction<InventoryItem> viewGroupDetails;
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

		protected virtual decimal? CalculateMaxQtyInBox(InventoryItem item, INItemBoxEx box)
		{
			decimal? resultWeight = null;
			decimal? resultVolume = null;
			
			if (item.BaseWeight > 0)
			{
				if (box.MaxWeight > 0)
				{
					resultWeight = Math.Floor((box.MaxWeight.Value - box.BoxWeight.GetValueOrDefault()) / item.BaseWeight.Value);
				}
			}

			if (item.PackageOption == INPackageOption.Weight)
			{
				return resultWeight;
			}

			if (item.BaseVolume > 0)
			{
				if (box.MaxVolume > 0)
				{
					resultVolume = Math.Floor(box.MaxVolume.Value / item.BaseVolume.Value);
				}
			}

			
			if (resultWeight != null && resultVolume != null)
			{
				return Math.Min(resultWeight.Value, resultVolume.Value);
			}

			if (resultWeight != null)
				return resultWeight;

			if (resultVolume != null)
				return resultVolume;

			return null;
		}
	}

	public class NonStockItemMaint : PXGraph<NonStockItemMaint>
	{
		#region DAC Overrides
		


		#endregion

        #region delegates
        protected virtual IEnumerable categories(
            [PXInt]
            int? categoryID
        )
        {
            if (categoryID == null)
            {
                yield return new INCategory()
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }
            foreach (INCategory item in PXSelect<INCategory,
            Where<INCategory.parentID,
                Equal<Required<INCategory.parentID>>>,
                OrderBy<Asc<INCategory.sortOrder>>>.Select(this, categoryID))
            {
                if (!string.IsNullOrEmpty(item.Description))
                    yield return item;
            }
        }
        #endregion

        [PXViewName(Messages.InventoryItem)]
		public PXSelect<InventoryItem, Where<InventoryItem.stkItem, Equal<False>, And<Match<Current<AccessInfo.userName>>>>> Item;
		[PXCopyPasteHiddenFields(typeof(InventoryItem.body))]
		public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> ItemSettings;
		public INUnitSelect<INUnit, InventoryItem.inventoryID, InventoryItem.itemClassID, InventoryItem.salesUnit, InventoryItem.purchaseUnit, InventoryItem.baseUnit, InventoryItem.lotSerClassID> itemunits;
		public PXSetupOptional<INSetup> insetup;
		public PXSetupOptional<CommonSetup> commonsetup;
		public PXSetup<Company> Company;
		public PXSetupOptional<TX.TXAvalaraSetup> avalaraSetup; 
		public PXSelect<INComponent, Where<INComponent.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> Components;
		public PXSelect<ARSalesPrice> SalesPrice;
		public PXSelect<INItemClass, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>> ItemClass;
		public POVendorInventorySelect<POVendorInventory,
				LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POVendorInventory.vendorID>>,
				LeftJoin<CRLocation, On<CRLocation.bAccountID, Equal<POVendorInventory.vendorID>,
							And<CRLocation.locationID, Equal<POVendorInventory.vendorLocationID>>>>>,
				Where<POVendorInventory.inventoryID, Equal<Current<InventoryItem.inventoryID>>>,
				InventoryItem> VendorItems;
		public CRAttributeList<InventoryItem> Answers;
        public PXSelect<INItemXRef, Where<INItemXRef.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> itemxrefrecords;
        public PXSelect<GL.Branch, Where<GL.Branch.branchID, Equal<Required<GL.Branch.branchID>>>> CurrentBranch;

        public PXSelectJoin<INItemCategory,
    InnerJoin<INCategory, On<INCategory.categoryID, Equal<INItemCategory.categoryID>>>,
    Where<INItemCategory.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> Category;

        public PXSelectOrderBy<INCategory, OrderBy<Asc<INCategory.sortOrder>>> Categories;

        public PXSelect<CacheEntityItem,
            Where<CacheEntityItem.path, Equal<CacheEntityItem.path>>,
            OrderBy<Asc<CacheEntityItem.number>>> EntityItems;

        #region Delegates
        protected IEnumerable entityItems(string parent)
        {
            PXSiteMapNode siteMap = PXSiteMap.Provider.FindSiteMapNodeByScreenID("IN202000");
            if (siteMap != null)
                foreach (var entry in EMailSourceHelper.TemplateEntity(this, parent, null, siteMap.GraphType, true))
                    yield return entry;
        }
        #endregion

		public NonStockItemMaint()
		{
			INSetup record = insetup.Current;

			PXUIFieldAttribute.SetVisible<INUnit.toUnit>(itemunits.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INUnit.toUnit>(itemunits.Cache, null, false);

			PXUIFieldAttribute.SetVisible<INUnit.sampleToUnit>(itemunits.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<INUnit.sampleToUnit>(itemunits.Cache, null, false);
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });

			action.AddMenuAction(ChangeID);
		}
		#region Cache Attached
		#region INComponent
		[PXDBDefault(typeof(InventoryItem.inventoryID))]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INComponent.inventoryID>>>>))]
		[PXDBInt(IsKey = true)]
		protected virtual void INComponent_InventoryID_CacheAttached(PXCache sender)
		{			
		}
		[PXDefault()]
		[Inventory(typeof(Search<InventoryItem.inventoryID, Where<Match<Current<AccessInfo.userName>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr), Filterable = true, IsKey = true, DisplayName = "Inventory ID")]
		protected virtual void INComponent_ComponentID_CacheAttached(PXCache sender)
		{			
		}
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Deferral Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXRestrictor(typeof(Where<DRDeferredCode.multiDeliverableArrangement, NotEqual<boolTrue>>), DR.Messages.ComponentsCantUseMDA)]
		[PXSelector(typeof(DRDeferredCode.deferredCodeID))]
		protected virtual void INComponent_DeferredCode_CacheAttached(PXCache sender)
		{			
		}
		#endregion

		#region POVendorInventory
        [PXDBInt()]
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<POVendorInventory.inventoryID>>>>))]
        [PXDBLiteDefault(typeof(InventoryItem.inventoryID))]
        protected virtual void POVendorInventory_InventoryID_CacheAttached(PXCache sender)
        {
        }

		[SubItem(typeof(POVendorInventory.inventoryID), DisplayName = "Subitem")]
		protected virtual void POVendorInventory_SubItemID_CacheAttached(PXCache sender)
		{
		}

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POVendorInventory.vendorID>>>),
			DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location")]
		[PXFormula(typeof(Selector<POVendorInventory.vendorID, Vendor.defLocationID>))]
		[PXParent(typeof(Select<Location, Where<Location.bAccountID, Equal<Current<POVendorInventory.vendorID>>,
												And<Location.locationID, Equal<Current<POVendorInventory.vendorLocationID>>>>>))]
		protected virtual void POVendorInventory_VendorLocationID_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region INItemXRef
        [SubItem(typeof(INItemXRef.inventoryID), IsKey = true, Disabled = true)]
        [PXDefault()]
        public virtual void INItemXRef_SubItemID_CacheAttached(PXCache sender)
        {
        }

		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<INItemXRef.inventoryID>>>))]
		[Inventory(Filterable = true, DirtyRead = true, Enabled = false, IsKey = true)]
        [PXDBDefault(typeof(InventoryItem.inventoryID), DefaultForInsert = true, DefaultForUpdate = false)]
        protected virtual void INItemXRef_InventoryID_CacheAttached(PXCache sender)
        {
        }
		#endregion

        #region INItemCategory
        [NonStockItem(IsKey = true, DirtyRead = true)]
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemCategory.inventoryID>>>>))]
        [PXDBLiteDefault(typeof(InventoryItem.inventoryID))]
        protected virtual void INItemCategory_InventoryID_CacheAttached(PXCache sender)
        {
        }
        #endregion

		#endregion

		#region Buttons Definition

		[PXCancelButton]
		[PXUIField(MapEnableRights = PXCacheRights.Select)]
		protected virtual System.Collections.IEnumerable Cancel(PXAdapter a)
		{
			foreach (InventoryItem e in (new PXCancel<InventoryItem>(this, "Cancel")).Press(a))
			{
				if (Item.Cache.GetStatus(e) == PXEntryStatus.Inserted)
				{
					InventoryItem e1 = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>, And<InventoryItem.stkItem, Equal<True>>>>
						.Select(this, e.InventoryCD);
					if (e1 != null)
					{
						Item.Cache.RaiseExceptionHandling<InventoryItem.inventoryCD>(e, e.InventoryCD,
							new PXSetPropertyException(Messages.StockItemExists));
					}
				}
				yield return e;
			}
		}
		public PXSave<InventoryItem> Save;
		public PXAction<InventoryItem> cancel;
		public PXInsert<InventoryItem> Insert;
		public PXCopyPasteAction<InventoryItem> Edit; 
		public PXDelete<InventoryItem> Delete;
		public PXFirst<InventoryItem> First;
		public PXPrevious<InventoryItem> Prev;
		public PXNext<InventoryItem> Next;
		public PXLast<InventoryItem> Last;

		public PXChangeID<InventoryItem, InventoryItem.inventoryCD> ChangeID;

		#endregion

		#region InventoryItem Event Handlers

		protected virtual void InventoryItem_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var item = e.Row as InventoryItem;
            if (item == null)
				return;

            //Multiple Components are not supported for CashReceipt Deferred Revenue:
			DRDeferredCode dc = PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Current<InventoryItem.deferredCode>>>>.Select(this);
            PXUIFieldAttribute.SetEnabled<POVendorInventory.isDefault>(this.VendorItems.Cache, null, true);
			
			//Initial State for Components:
			Components.Cache.AllowDelete = false;
			Components.Cache.AllowInsert = false;
			Components.Cache.AllowUpdate = false;

			InventoryItemMaint.SetDefaultTermControlsState(sender, item);

			if (item.IsSplitted == true)
			{
				Components.Cache.AllowDelete = true;
				Components.Cache.AllowInsert = true;
				Components.Cache.AllowUpdate = true;
				item.TotalPercentage = SumComponentsPercentage();
				PXUIFieldAttribute.SetEnabled<InventoryItem.useParentSubID>(sender, item, true);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled<InventoryItem.useParentSubID>(sender, item, false);
				item.UseParentSubID = false;
				item.TotalPercentage = 100;
			}
			if (item.NonStockReceipt == true)
			{
				PXUIFieldAttribute.SetRequired<InventoryItem.postClassID>(sender, true);
			}
			else
			{
				PXUIFieldAttribute.SetRequired<InventoryItem.postClassID>(sender, false);
			}
			InventoryHelper.CheckZeroDefaultTerm<InventoryItem.deferredCode, InventoryItem.defaultTerm>(sender, item);
		}

		protected virtual void InventoryItem_ItemClassID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			InventoryItem row = (InventoryItem)e.Row;
			INItemClass ic = (INItemClass)PXSelectorAttribute.Select<INItemClass.itemClassID>(cache, row, e.NewValue);
			this.doResetDefaultsOnItemClassChange = false;

			if (ic != null)
			{
				this.doResetDefaultsOnItemClassChange = true;
				if (e.ExternalCall && cache.GetStatus(row) != PXEntryStatus.Inserted)
				{
					if (Item.Ask(AR.Messages.Warning, Messages.ItemClassChangeWarning, MessageButtons.YesNo) == WebDialogResult.No)
					{
						this.doResetDefaultsOnItemClassChange = false;
					}
				}
			}

		}

		protected virtual void InventoryItem_ItemClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetValueExt<InventoryItem.baseUnit>(e.Row, null);
			sender.SetValue<InventoryItem.salesUnit>(e.Row, null);
			sender.SetValue<InventoryItem.purchaseUnit>(e.Row, null);

			sender.SetDefaultExt<InventoryItem.baseUnit>(e.Row);
			sender.SetDefaultExt<InventoryItem.salesUnit>(e.Row);
			sender.SetDefaultExt<InventoryItem.purchaseUnit>(e.Row);
			sender.SetDefaultExt<InventoryItem.dfltSiteID>(e.Row);

			if (doResetDefaultsOnItemClassChange)
			{
				if (PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
				{
					sender.SetDefaultExt<InventoryItem.deferredCode>(e.Row);
					sender.SetDefaultExt<InventoryItem.postClassID>(e.Row);
					sender.SetDefaultExt<InventoryItem.markupPct>(e.Row);
					sender.SetDefaultExt<InventoryItem.minGrossProfitPct>(e.Row);
				}

				sender.SetDefaultExt<InventoryItem.taxCategoryID>(e.Row);
				sender.SetDefaultExt<InventoryItem.itemType>(e.Row);
				sender.SetDefaultExt<InventoryItem.priceClassID>(e.Row);

				INItemClass ic = ItemClass.Select();
				if (ic != null)
				{
					sender.SetValue<InventoryItem.priceWorkgroupID>(e.Row, ic.PriceWorkgroupID);
					sender.SetValue<InventoryItem.priceManagerID>(e.Row, ic.PriceManagerID);
				}
			}
		}

		protected virtual void InventoryItem_PostClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<InventoryItem.invtAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.invtSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.salesAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.salesSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.cOGSAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.cOGSSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pOAccrualAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pOAccrualSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pPVAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.pPVSubID>(e.Row);
			sender.SetDefaultExt<InventoryItem.deferralAcctID>(e.Row);
			sender.SetDefaultExt<InventoryItem.deferralSubID>(e.Row);
		}

		protected virtual void InventoryItem_DeferredCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var item = e.Row as InventoryItem;
			InventoryItemMaint.UpdateSplittedFromDeferralCode(sender, item);
			InventoryItemMaint.SetDefaultTerm(sender, item, typeof(InventoryItem.deferredCode), typeof(InventoryItem.defaultTerm), typeof(InventoryItem.defaultTermUOM));
		}

		protected virtual void INComponent_DeferredCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			InventoryItemMaint.SetDefaultTerm(sender, e.Row, typeof(INComponent.deferredCode), typeof(INComponent.defaultTerm), typeof(INComponent.defaultTermUOM));
		}

		protected virtual void InventoryItem_IsSplitted_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;
			if (row != null)
			{
				if (row.IsSplitted == false)
				{
					foreach (INComponent c in Components.Select())
					{
						Components.Delete(c);
					}

					row.TotalPercentage = 100;
				}
				else
					row.TotalPercentage = 0;
			}
		}

		protected virtual void InventoryItem_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			((InventoryItem)e.Row).TotalPercentage = 100;
		}

		protected virtual void InventoryItem_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			InventoryItem row = e.Row as InventoryItem;

			if (row.IsSplitted == true)
			{
				if (string.IsNullOrEmpty(row.DeferredCode))
				{
					if (sender.RaiseExceptionHandling<InventoryItem.deferredCode>(e.Row, row.DeferredCode, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, typeof(InventoryItem.deferredCode).Name)))
					{
						throw new PXRowPersistingException(typeof(InventoryItem.deferredCode).Name, row.DeferredCode, Data.ErrorMessages.FieldIsEmpty, typeof(InventoryItem.deferredCode).Name);
					}
				}

				var components = Components.Select().RowCast<INComponent>().ToList();

				InventoryItemMaint.VerifyComponentPercentages(sender, row, components);
				InventoryItemMaint.VerifyOnlyOneResidualComponent(sender, row, components);
				InventoryItemMaint.CheckSameTermOnAllComponents(Components.Cache, components);
			}

			if (!PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
			{
				row.NonStockReceipt = false;
				row.NonStockShip = false;
			}

			if (row.NonStockReceipt == true)
			{
				if (string.IsNullOrEmpty(row.PostClassID))
				{
					throw new PXRowPersistingException(typeof(InventoryItem.postClassID).Name, row.PostClassID, Data.ErrorMessages.FieldIsEmpty, typeof(InventoryItem.postClassID).Name);
				}
			}

			if (e.Operation == PXDBOperation.Delete)
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
                    PXDatabase.Delete<CSAnswers>(new PXDataFieldRestrict("RefNoteID", PXDbType.UniqueIdentifier, ((InventoryItem)e.Row).NoteID));
                    ts.Complete(this);
				}
			}
		}

		private bool AlwaysFromBaseCurrency
		{
			get
			{
				bool alwaysFromBase = false;

				ARSetup arsetup = PXSelect<ARSetup>.Select(this);
				if (arsetup != null)
				{
					alwaysFromBase = arsetup.AlwaysFromBaseCury == true;
				}

				return alwaysFromBase;
			}
		}

		protected virtual void InventoryItem_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (e.Row == null) { return; }

			InventoryItem ii_rec = (InventoryItem)e.Row;

			// deleting only inventory-specific uoms
			foreach (INUnit inunit_rec in PXSelect<INUnit, Where<INUnit.unitType, Equal<short1>, And<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>>>>.Select(this, ii_rec.InventoryID))
			{
				itemunits.Delete(inunit_rec);
			}
		}

		#endregion

		#region INComponent Event Handles

		protected virtual void INComponent_Percentage_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null && row.AmtOption == INAmountOption.Percentage)
			{
				row.Percentage = GetRemainingPercentage();
			}
		}

		protected virtual void INComponent_ComponentID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.ComponentID);
				var deferralCode = (DRDeferredCode)PXSelectorAttribute.Select<InventoryItem.deferredCode>(Item.Cache, item);
				bool useDeferralFromItem = deferralCode != null && deferralCode.MultiDeliverableArrangement != true;

				if (item != null)
				{
					row.SalesAcctID = item.SalesAcctID;
					row.SalesSubID = item.SalesSubID;
					row.UOM = item.SalesUnit;
					row.DeferredCode = useDeferralFromItem ? item.DeferredCode : null;
					row.DefaultTerm = item.DefaultTerm;
					row.DefaultTermUOM = item.DefaultTermUOM;
				}
			}
		}


		protected virtual void INComponent_AmtOption_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INComponent row = e.Row as INComponent;
			if (row != null)
			{
				if (row.AmtOption == INAmountOption.Percentage)
				{
					row.FixedAmt = null;
					row.Percentage = GetRemainingPercentage();
				}
				else
				{
					row.Percentage = 0;
				}

				if (row.AmtOption == INAmountOption.Residual)
				{
					sender.SetValueExt<INComponent.deferredCode>(row, null);
					sender.SetDefaultExt<INComponent.fixedAmt>(row);
					sender.SetDefaultExt<INComponent.percentage>(row);
				}
			}
		}


		protected virtual void INComponent_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var component = e.Row as INComponent;
			InventoryItemMaint.SetComponentControlsState(sender, component);

			InventoryHelper.CheckZeroDefaultTerm<INComponent.deferredCode, INComponent.defaultTerm>(sender, component);
		}

		#endregion

		protected virtual void INUnit_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INUnit row = (INUnit)e.Row;
			PXFieldState state = (PXFieldState)sender.GetStateExt<INUnit.fromUnit>(row);
			uint msgNbr;
			String msqPrefix;
			if (row.ToUnit == Item.Current.BaseUnit && (state.Error == null || state.Error == PXMessages.Localize(Messages.BaseUnitNotSmallest, out msgNbr, out msqPrefix) || state.ErrorLevel == PXErrorLevel.RowInfo))
			{
				if (row.UnitMultDiv == MultDiv.Multiply && row.UnitRate < 1 || row.UnitMultDiv == MultDiv.Divide && row.UnitRate > 1)
					sender.RaiseExceptionHandling<INUnit.fromUnit>(row, row.FromUnit, new PXSetPropertyException(Messages.BaseUnitNotSmallest, PXErrorLevel.RowWarning));
				else
					sender.RaiseExceptionHandling<INUnit.fromUnit>(row, row.FromUnit, null);
			}
		}

		public PXAction<InventoryItem> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Update)]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2, 3 }, new string[] 
			{ 
					"Update Price", 
					"Update Cost",
          "View Restriction Groups"                    
			})]
			int? actionID
			)
		{
			switch (actionID)
			{
				case 2:
					if (ItemSettings.Current != null && ItemSettings.Current.PendingStdCostDate != null)
					{
						ItemSettings.Current.LastStdCost = ItemSettings.Current.StdCost;
						ItemSettings.Current.StdCostDate = ItemSettings.Current.PendingStdCostDate.GetValueOrDefault((DateTime)this.Accessinfo.BusinessDate);
						ItemSettings.Current.StdCost = ItemSettings.Current.PendingStdCost;
						ItemSettings.Current.PendingStdCost = 0;
						ItemSettings.Current.PendingStdCostDate = null;
						ItemSettings.Update(ItemSettings.Current);

						this.Save.Press();
					}

					break;
				case 3:
					if (Item.Current != null)
					{
						INAccessDetailByItem graph = CreateInstance<INAccessDetailByItem>();
						graph.Item.Current = graph.Item.Search<InventoryItem.inventoryCD>(Item.Current.InventoryCD);
						throw new PXRedirectRequiredException(graph, false, "Restricted Groups");
					}
					break;
            }
            return adapter.Get();
        }

        public PXAction<InventoryItem> inquiry;
        [PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Inquiry(PXAdapter adapter,
            [PXInt]
			[PXIntList(new int[] { 1, 2 }, new string[] 
			{ 
					"Sales Prices",
					"Vendor Prices"
			})]
			int? inquiryID
            )
        {
            switch (inquiryID)
            {
                case 1:
                    if (Item.Current != null)
                    {
                        ARSalesPriceMaint graph = PXGraph.CreateInstance<ARSalesPriceMaint>();
                        graph.Filter.Current.InventoryID = Item.Current.InventoryID;
                        throw new PXRedirectRequiredException(graph, "Sales Prices");
                    }
                    break;
                case 2:
                    if (Item.Current != null)
                    {
                        APVendorPriceMaint graph = PXGraph.CreateInstance<APVendorPriceMaint>();
                        graph.Filter.Current.InventoryID = Item.Current.InventoryID;
                        throw new PXRedirectRequiredException(graph, "Vendor Prices");
                    }
                    break;

			}
			return adapter.Get();
		}


		private decimal GetRemainingPercentage()
		{
			decimal result = 100;

			foreach (INComponent comp in Components.Select())
			{
				if (comp.AmtOption == INAmountOption.Percentage)
				result -= (comp.Percentage ?? 0);
			}

			if (result > 0)
				return result;
			else
				return 0;
		}

		private decimal SumComponentsPercentage()
		{
			decimal result = 0;

			foreach (INComponent comp in Components.Select())
			{
				if (comp.AmtOption == INAmountOption.Percentage)
				result += (comp.Percentage ?? 0);
			}

			return result;
		}

		#region Private members
		private bool doResetDefaultsOnItemClassChange;
		#endregion

		private string SalesPriceUpdateUnit
		{
			get
			{
				return SalesPriceUpdateUnitType.BaseUnit;
			}
        }

        #region InventoryItem
        #region InventoryCD
        [PXDefault()]
		[InventoryRaw(typeof(Where<InventoryItem.stkItem, Equal<False>>), IsKey = true, DisplayName = "Inventory ID", Filterable = true)]
        public virtual void InventoryItem_InventoryCD_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostClassID
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(INPostClass.postClassID), DescriptionField = typeof(INPostClass.descr))]
        [PXUIField(DisplayName = "Posting Class", Required = true)]
        [PXDefault(typeof(Search<INItemClass.postClassID, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual void InventoryItem_PostClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region LotSerClassID
        [PXDBString(10, IsUnicode = true)]
        public virtual void InventoryItem_LotSerClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region ItemType
        [PXDBString(1, IsFixed = true)]
        [PXDefault(INItemTypes.NonStockItem, typeof(Search<INItemClass.itemType, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible)]
        [INItemTypes.NonStockList()]
        public virtual void InventoryItem_ItemType_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region ValMethod
        [PXDBString(1, IsFixed = true)]
        [PXDefault(INValMethod.Standard)]
        public virtual void InventoryItem_ValMethod_CacheAttached(PXCache sender)
        {
        }
        #endregion
		#region InvtAcctID
		[Account(DisplayName = "Expense Accrual Account", DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Search<INPostClass.invtAcctID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual void InventoryItem_InvtAcctID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region InvtSubID
		[SubAccount(typeof(InventoryItem.invtAcctID), DisplayName = "Expense Accrual Sub.", DescriptionField = typeof(Sub.description))]
		[PXDefault(typeof(Search<INPostClass.invtSubID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual void InventoryItem_InvtSubID_CacheAttached(PXCache sender)
		{
		}
		#endregion
        #region COGSAcctID
        [PXDefault(typeof(Search<INPostClass.cOGSAcctID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>))]
        [Account(DisplayName = "Expense Account", DescriptionField = typeof(Account.description))]
        public virtual void InventoryItem_COGSAcctID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region COGSSubID
        [PXDefault(typeof(Search<INPostClass.cOGSSubID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>))]
        [SubAccount(typeof(InventoryItem.cOGSAcctID), DisplayName = "Expense Sub.", DescriptionField = typeof(Sub.description))]
        public virtual void InventoryItem_COGSSubID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region StkItem
        [PXDBBool()]
        [PXDefault(false)]
        public virtual void InventoryItem_StkItem_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region ItemClassID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRestrictor(typeof(Where<INItemClass.stkItem, NotEqual<True>>), Messages.ItemClassIsStock)]
        protected virtual void InventoryItem_ItemClassID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region KitItem
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Is a Kit")]
        public virtual void InventoryItem_KitItem_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region DefaultSubItemOnEntry
        [PXDBBool()]
        [PXDefault(false)]
        public virtual void InventoryItem_DefaultSubItemOnEntry_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region DeferredCode
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa", BqlField = typeof(InventoryItem.deferredCode))]
        [PXUIField(DisplayName = "Deferral Code")]
        [PXSelector(typeof(Search<DRDeferredCode.deferredCodeID>))]
        [PXDefault(typeof(Search<INItemClass.deferredCode, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual void InventoryItem_DeferredCode_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region IsSplitted
        [PXDBBool(BqlField = typeof(InventoryItem.isSplitted))]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Split into Components")]
        public virtual void InventoryItem_IsSplitted_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region UseParentSubID
        [PXDBBool(BqlField = typeof(InventoryItem.useParentSubID))]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Use Component Subaccounts")]
        public virtual void InventoryItem_UseParentSubID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region TotalPercentage
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Percentage", Enabled = false)]
        public virtual void InventoryItem_TotalPercentage_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region NonStockReceipt
        [PXDBBool(BqlField = typeof(InventoryItem.nonStockReceipt))]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Require Receipt")]
        public virtual void InventoryItem_NonStockReceipt_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region NonStockShip
        [PXDBBool(BqlField = typeof(InventoryItem.nonStockShip))]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Require Shipment")]
        public virtual void InventoryItem_NonStockShip_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #endregion


        #region POInventoryItem
        protected virtual void POVendorInventory_IsDefault_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            POVendorInventory current = e.Row as POVendorInventory;
            if ((POVendorInventory)this.VendorItems.SelectWindowed(0, 1, current.InventoryID) == null)
            {
                e.NewValue = true;
                e.Cancel = true;
            }
        }

        protected virtual void POVendorInventory_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            POVendorInventory current = e.Row as POVendorInventory;
			if (current.VendorID != null && current.IsDefault == true)
            {
                    InventoryItem upd = this.Item.Current;
                upd.PreferredVendorID = current.IsDefault == true ? current.VendorID : null;
                upd.PreferredVendorLocationID = current.IsDefault == true ? current.VendorLocationID : null;
				Item.Update(upd);
            }
        }

        protected virtual void POVendorInventory_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            POVendorInventory current = e.Row as POVendorInventory;
			if (e.OldRow == null || current == null || current.VendorID == null)
				return;

                if ((current.IsDefault == true && (this.Item.Current.PreferredVendorID != current.VendorID || this.Item.Current.PreferredVendorLocationID != current.VendorLocationID) ||
                        (current.IsDefault != true && this.Item.Current.PreferredVendorID == current.VendorID && this.Item.Current.PreferredVendorLocationID == current.VendorLocationID)))
                {
                    InventoryItem upd = this.Item.Current;
                    upd.PreferredVendorID = current.IsDefault == true ? current.VendorID : null;
                    upd.PreferredVendorLocationID = current.IsDefault == true ? current.VendorLocationID : null;
				Item.Update(upd);
				if (current.IsDefault == true)
				{
					foreach (POVendorInventory vendorInventory in VendorItems.Select())
					{
						if (vendorInventory.RecordID != current.RecordID && vendorInventory.IsDefault == true)
							VendorItems.Cache.SetValue<POVendorInventory.isDefault>(vendorInventory, false);
					}
					this.VendorItems.Cache.ClearQueryCache();
                    this.VendorItems.View.RequestRefresh();
                }
        }


        }

        protected virtual void POVendorInventory_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
        {
            InventoryItem upd = PXCache<InventoryItem>.CreateCopy(this.Item.Current);
            POVendorInventory vendor = e.Row as POVendorInventory;
            object isdefault = cache.GetValueExt<POVendorInventory.isDefault>(e.Row);
            if (isdefault is PXFieldState)
            {
                isdefault = ((PXFieldState)isdefault).Value;
            }
            if ((bool?)isdefault == true)
            {
                upd.PreferredVendorID = null;
                upd.PreferredVendorLocationID = null;
                this.Item.Update(upd);
            }
        }
        #endregion POInventoryItem

        #region INItemXRef Event Handlers
        protected virtual void INItemXRef_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            if (!IsImport)
            {
                INItemXRef row = (INItemXRef)e.Row;
                string alterrmsg = PXUIFieldAttribute.GetError<INItemXRef.alternateID>(sender, e.Row);
                string baccerrmsg = PXUIFieldAttribute.GetError<INItemXRef.bAccountID>(sender, e.Row);

                bool isbaccenabled = ((INItemXRef)e.Row).AlternateType == INAlternateType.VPN || ((INItemXRef)e.Row).AlternateType == INAlternateType.CPN;

                e.Cancel = (isbaccenabled && (string.IsNullOrEmpty(baccerrmsg) == false || (row.BAccountID == null || (int)row.BAccountID == 0)))
                    || (row.AlternateID == null || string.IsNullOrEmpty(alterrmsg) == false);
            }
        }

        protected virtual void INItemXRef_InventoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (Item.Current != null)
            {
                e.NewValue = Item.Current.InventoryID;
                e.Cancel = true;
            }
        }

		protected virtual void INItemXRef_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void INItemXRef_SubItemID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void INItemXRef_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
            {
                if (((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
                {
                    ((INItemXRef)e.Row).BAccountID = (int)0;
                }
                ((INItemXRef)e.Row).InventoryID = Item.Current.InventoryID;
                sender.Normalize();
            }
        }

        protected virtual void INItemXRef_BAccountID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (e.Row != null && (int?)e.ReturnValue == 0 && ((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
            {
                e.ReturnValue = null;
                e.Cancel = true;
            }
        }

        protected virtual void INItemXRef_BAccountID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.Row != null && e.NewValue == null && ((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
            {
                e.NewValue = (int)0;
                e.Cancel = true;
            }
        }

        protected virtual void INItemXRef_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (((INItemXRef)e.Row).AlternateType != INAlternateType.VPN && ((INItemXRef)e.Row).AlternateType != INAlternateType.CPN)
            {
                e.Cancel = true;
            }
        }

        protected virtual void INItemXRef_AlternateID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            var row = e.Row as INItemXRef;

            if (row == null || Item.Current == null || row.AlternateID == null)
                return;

            if (!PXDimensionAttribute.MatchMask<InventoryItem.inventoryCD>(Item.Cache, row.AlternateID))
            {
                sender.RaiseExceptionHandling<INItemXRef.alternateID>(row, row.AlternateID, new PXSetPropertyException(Messages.AlternateIDDoesNotCorrelateWithCurrentSegmentRules, PXErrorLevel.Warning));
            }
        }
        #endregion
    }


    [Serializable]
    [PXHidden]
    public partial class DFVendorInventory : IBqlTable
	{
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBInt(IsKey = true)]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region AddLeadTimeDays
		public abstract class addLeadTimeDays : PX.Data.IBqlField
		{
		}
		protected Int16? _AddLeadTimeDays;
		[PXDefault((short)0)]
		[PXDBShort()]
		[PXUIField(DisplayName = "Add. Lead Time (Days)")]
		public virtual Int16? AddLeadTimeDays
		{
			get
			{
				return this._AddLeadTimeDays;
			}
			set
			{
				this._AddLeadTimeDays = value;
			}
		}
		#endregion
		#region VLeadTime
		public abstract class vLeadTime : PX.Data.IBqlField
		{
		}
		protected Int16? _VLeadTime;
		[PXShort(MinValue = 0, MaxValue = 100000)]
		[PXUIField(DisplayName = "Vendor Lead Time (Days)", Enabled = false)]		
		public virtual Int16? VLeadTime
		{
			get
			{
				return this._VLeadTime;
			}
			set
			{
				this._VLeadTime = value;
			}
		}
		#endregion
		#region Active
		public abstract class active : PX.Data.IBqlField
		{
		}
		protected Boolean? _Active;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual Boolean? Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				this._Active = value;
			}
		}
		#endregion
		#region MinOrdFreq
		public abstract class minOrdFreq : PX.Data.IBqlField
		{
		}
		protected Int32? _MinOrdFreq;
		[PXDBInt()]
		[PXUIField(DisplayName = "Min. Order Freq.(Days)")]
		[PXDefault(0)]
		public virtual Int32? MinOrdFreq
		{
			get
			{
				return this._MinOrdFreq;
			}
			set
			{
				this._MinOrdFreq = value;
			}
		}
		#endregion
		#region MinOrdQty
		public abstract class minOrdQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinOrdQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Min. Order Qty.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? MinOrdQty
		{
			get
			{
				return this._MinOrdQty;
			}
			set
			{
				this._MinOrdQty = value;
			}
		}
		#endregion
		#region MaxOrdQty
		public abstract class maxOrdQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxOrdQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Max Order Qty.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? MaxOrdQty
		{
			get
			{
				return this._MaxOrdQty;
			}
			set
			{
				this._MaxOrdQty = value;
			}
		}
		#endregion
		#region LotSize
		public abstract class lotSize : PX.Data.IBqlField
		{
		}
		protected Decimal? _LotSize;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Lot Size")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? LotSize
		{
			get
			{
				return this._LotSize;
			}
			set
			{
				this._LotSize = value;
			}
		}
		#endregion
		#region ERQ
		public abstract class eRQ : PX.Data.IBqlField
		{
		}
		protected Decimal? _ERQ;
		[PXDBQuantity]
		[PXUIField(DisplayName = "ERQ")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ERQ
		{
			get
			{
				return this._ERQ;
			}
			set
			{
				this._ERQ = value;
			}
		}
		#endregion		
	}
}
