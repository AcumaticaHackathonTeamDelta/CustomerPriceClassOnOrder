using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using System.Collections;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.RQ
{
	public class RQBiddingEntry : PXGraph<RQBiddingEntry>
	{
		public PXSave<RQBiddingVendor> Save;
		public PXAction<RQBiddingVendor> cancel;
		public PXBiddingInsert<RQBiddingVendor> Insert;				
		public PXDelete<RQBiddingVendor> Delete;
		public PXFirst<RQBiddingVendor> First;						
		public PXAction<RQBiddingVendor> previous;
		[PXUIField(DisplayName = ActionsMessages.Previous, MapEnableRights = PXCacheRights.Select)]
		[PXPreviousButton]
		protected virtual IEnumerable Previous(PXAdapter adapter)
		{
			foreach (PXResult<RQBiddingVendor, RQRequisition> item in new PXPrevious<RQBiddingVendor>(this, "Previous").Press(adapter))
			{
				if (Vendor.Cache.GetStatus((RQBiddingVendor)item) == PXEntryStatus.Inserted && adapter.Searches != null)
				{
					adapter.Searches = null;
					foreach (PXResult<RQBiddingVendor, RQRequisition> inserted in Insert.Press(adapter))
					{
						yield return inserted;
					}
				}
				else
				{
					yield return item;
				}
			}
		}
		public PXAction<RQBiddingVendor> next;
		[PXUIField(DisplayName = ActionsMessages.Next, MapEnableRights = PXCacheRights.Select)]
		[PXNextButton]
		protected virtual IEnumerable Next(PXAdapter adapter)
		{
			foreach (PXResult<RQBiddingVendor, RQRequisition> item in new PXNext<RQBiddingVendor>(this, "Next").Press(adapter))
			{
				if (Vendor.Cache.GetStatus((RQBiddingVendor)item) == PXEntryStatus.Inserted && adapter.Searches != null)
				{
					adapter.Searches = null;
					foreach (PXResult<RQBiddingVendor, RQRequisition> inserted in Insert.Press(adapter))
					{
						yield return inserted;
					}
				}
				else
				{
					yield return item;
				}
			}
		}
		public PXLast<RQBiddingVendor> Last;

		#region Additional Buttons
		public PXAction<RQRequest> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Process)]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (RQRequest current in adapter.Get<RQRequest>())
			{
				if (current != null)
				{
					bool needSave = false;
					Save.Press();
					PORemitAddress address = this.Remit_Address.Select();
					if (address != null && address.IsDefaultAddress == false && address.IsValidated == false)
					{
						if (PXAddressValidator.Validate<PORemitAddress>(this, address, true))
							needSave = true;
					}
					
					if (needSave == true)
						this.Save.Press();
				}
				yield return current;
			}
		} 
		#endregion

		#region Cache Attached
		
		#region RQBiddingVendor
		[PXDBIdentity]
		protected virtual void RQBiddingVendor_LineID_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
		[PXDefault("", PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXSelectorAttribute(
			typeof(Search<RQRequisition.reqNbr,
			Where<RQRequisition.status, Equal<RQRequisitionStatus.bidding>>>),
			typeof(RQRequisition.status),
			typeof(RQRequisition.employeeID),
			typeof(RQRequisition.vendorID),
			Filterable = true)]
		[PXUIField(DisplayName = "Requisition")]
		protected virtual void RQBiddingVendor_ReqNbr_CacheAttached(PXCache sender)
		{
		}		
		#region VendorID
		[PXDefault]
		[VendorNonEmployeeActive(typeof(Search2<BAccountR.bAccountID,
			LeftJoin<RQBiddingVendor,
						On<RQBiddingVendor.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>,
						And<RQBiddingVendor.vendorID, Equal<BAccountR.bAccountID>>>>>), 
						Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, IsKey = true)]
		[PXRestrictor(typeof(Where<RQBiddingVendor.reqNbr, IsNotNull>), Messages.VendorNotInBidding)]
		protected virtual void RQBiddingVendor_VendorID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region VendorLocationID
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQBiddingVendor.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, IsKey = true)]			
		protected virtual void RQBiddingVendor_VendorLocationID_CacheAttached(PXCache sender)
		{			
		}
		#endregion		
		#endregion
		
		#endregion
		public PXSelectJoin<RQBiddingVendor,
			InnerJoin<RQRequisition, On<RQRequisition.reqNbr, Equal<RQBiddingVendor.reqNbr>>,
			 LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<RQBiddingVendor.vendorID>>,
			 LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<RQRequisition.customerID>>>>>,			
			Where<RQRequisition.status, Equal<RQRequisitionStatus.bidding>,
			And2<Where<Vendor.bAccountID, IsNull, 
			       Or<Match<Vendor,Current<AccessInfo.userName>>>>,
			And<Where<Customer.bAccountID, IsNull, 
			       Or<Match<Customer,Current<AccessInfo.userName>>>>>>>> Vendor;

		public PXSetup<Vendor,
		 Where<Vendor.bAccountID, Equal<Current<RQBiddingVendor.vendorID>>>> BVendor;
		
		public PXSelect<RQBiddingVendor,
			Where<RQBiddingVendor.lineID, Equal<Current<RQBiddingVendor.lineID>>>> CurrentDocument;

		public PXSelect<RQRequisitionLineBidding,			
			Where<RQRequisitionLineBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>>> Lines;

		public PXSelect<RQBidding,
			Where<RQBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>,
			And<RQBidding.vendorID, Equal<Current<RQBiddingVendor.vendorID>>,
			And<RQBidding.vendorLocationID, Equal<Current<RQBiddingVendor.vendorLocationID>>>>>> Bidding;

		public PXSelect<PORemitAddress, Where<PORemitAddress.addressID, Equal<Current<RQBiddingVendor.remitAddressID>>>> Remit_Address;
		public PXSelect<PORemitContact, Where<PORemitContact.contactID, Equal<Current<RQBiddingVendor.remitContactID>>>> Remit_Contact;
		
		public PXSelect<RQRequisitionLine> rqline;
		public PXSelect<RQRequisition> rq;
		public PXSelect<RQRequestLine> reqline;
		public PXSelect<RQRequest> req;
		public CMSetupSelect cmsetup;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<RQBiddingVendor.curyInfoID>>>> currencyinfo;
		public ToggleCurrency<RQBiddingVendor> CurrencyView;

		public RQBiddingEntry()
		{			
			this.Lines.Cache.AllowInsert = this.Lines.Cache.AllowDelete = false;
			PXUIFieldAttribute.SetEnabled(this.Lines.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<RQRequisitionLineBidding.minQty>(this.Lines.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<RQRequisitionLineBidding.quoteNumber>(this.Lines.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<RQRequisitionLineBidding.quoteQty>(this.Lines.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<RQRequisitionLineBidding.curyQuoteUnitCost>(this.Lines.Cache, null, true);
		}

		protected virtual IEnumerable lines()
		{
			if (Vendor.Current == null || Vendor.Current.VendorLocationID == null)
				yield break;
			using (ReadOnlyScope scope = new ReadOnlyScope(this.Lines.Cache))
			{				
				bool reset = !Bidding.Cache.IsDirty;
				PXResultset<RQRequisitionLineBidding> list =
					PXSelectJoin<RQRequisitionLineBidding,
						LeftJoin<RQBidding,
									On<RQBidding.reqNbr, Equal<RQRequisitionLineBidding.reqNbr>,
								 And<RQBidding.lineNbr, Equal<RQRequisitionLineBidding.lineNbr>,
								And<RQBidding.vendorID, Equal<Current<RQBiddingVendor.vendorID>>,
								And<RQBidding.vendorLocationID, Equal<Current<RQBiddingVendor.vendorLocationID>>>>>>>,
						Where<RQRequisitionLineBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>>>
						.Select(this);

				if (reset) this.Lines.Cache.Clear();
				foreach (PXResult<RQRequisitionLineBidding, RQBidding> item in list)
				{
					RQRequisitionLineBidding result = item;
					RQBidding bidding = item;
					bidding = Bidding.Locate(bidding) ?? item;

					result = (RQRequisitionLineBidding)this.Lines.Cache.CreateCopy(result);
					result.QuoteNumber = bidding.QuoteNumber;
					result.QuoteQty = bidding.QuoteQty ?? 0m;
					result.CuryInfoID = Vendor.Current.CuryInfoID;
					result.CuryQuoteUnitCost = bidding.CuryQuoteUnitCost ?? 0m;
					result.QuoteUnitCost = bidding.QuoteUnitCost ?? 0m;
					result.CuryQuoteExtCost = bidding.CuryQuoteExtCost ?? 0m;
					result.QuoteExtCost = bidding.QuoteExtCost ?? 0m;
					result.MinQty = bidding.MinQty ?? 0m;										

					if (bidding.CuryQuoteUnitCost == null && result.InventoryID != null)
					{			
						POItemCostManager.ItemCost cost = 
							POItemCostManager.Fetch(this,
								Vendor.Current.VendorID,
								Vendor.Current.VendorLocationID, null,
								(string)Vendor.GetValueExt<RQBiddingVendor.curyID>(Vendor.Current),
								result.InventoryID, result.SubItemID, null, result.UOM);
						result.CuryQuoteUnitCost =
							cost.Convert<RQRequisitionLineBidding.inventoryID, RQRequisitionLineBidding.curyInfoID>(this, result, result.UOM); ;
					}

					if (result.CuryQuoteUnitCost == null)
						result.CuryQuoteUnitCost = 0m;

					result = this.Lines.Insert(result) ?? (RQRequisitionLineBidding)this.Lines.Cache.Locate(result);
					yield return result;
				}
			}
		}
		
		protected virtual void RQRequisitionLineBidding_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}
		protected virtual void RQRequisitionLineBidding_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (this.Vendor.Cache.GetStatus(this.Vendor.Current) == PXEntryStatus.Notchanged
				|| this.Vendor.Cache.GetStatus(this.Vendor.Current) == PXEntryStatus.Held)
			{
				this.Vendor.Cache.SetStatus(this.Vendor.Current, PXEntryStatus.Updated);
			}

			RQRequisitionLineBidding newRow = (RQRequisitionLineBidding)e.Row;
			RQRequisitionLineBidding oldRow = (RQRequisitionLineBidding)e.OldRow;
			if (newRow.MinQty != oldRow.MinQty ||
				 newRow.QuoteUnitCost != oldRow.QuoteUnitCost ||
				 newRow.QuoteQty != oldRow.QuoteQty ||
				 newRow.QuoteNumber != oldRow.QuoteNumber)
			{
				RQBidding bidding =
					PXSelect<RQBidding,
					Where<RQBidding.reqNbr, Equal<Required<RQBidding.reqNbr>>,
						And<RQBidding.lineNbr, Equal<Required<RQBidding.lineNbr>>,
						And<RQBidding.vendorID, Equal<Required<RQBidding.vendorID>>,
						And<RQBidding.vendorLocationID, Equal<Required<RQBidding.vendorLocationID>>>>>>>.SelectWindowed(
						this, 0, 1,
					Vendor.Current.ReqNbr,
					newRow.LineNbr,
					Vendor.Current.VendorID,
					Vendor.Current.VendorLocationID);
				if (bidding == null)
				{
					bidding = new RQBidding();
					bidding.VendorID = Vendor.Current.VendorID;
					bidding.VendorLocationID = Vendor.Current.VendorLocationID;
					bidding.ReqNbr = Vendor.Current.ReqNbr;
					bidding.CuryInfoID = Vendor.Current.CuryInfoID;
					bidding.LineNbr = newRow.LineNbr;
				}
				else
					bidding = (RQBidding)this.Bidding.Cache.CreateCopy(bidding);

				bidding.QuoteQty = newRow.QuoteQty;
				bidding.QuoteNumber = newRow.QuoteNumber;
				bidding.QuoteUnitCost = newRow.QuoteUnitCost;
				bidding.CuryQuoteUnitCost = newRow.CuryQuoteUnitCost;
				bidding.MinQty = newRow.MinQty;

				this.Bidding.Update(bidding);
			}
		}

		protected virtual void RQBiddingVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			if (row == null) return;

			if(row.EntryDate == null)
				row.EntryDate = sender.Graph.Accessinfo.BusinessDate;

			if (BVendor.Current != null && BVendor.Current.AllowOverrideCury == true)
			{
				RQBidding bidding = (RQBidding)this.Bidding.View.SelectSingleBound(new object[] { e.Row });
				PXUIFieldAttribute.SetEnabled<RQBiddingVendor.curyID>(sender, e.Row, bidding == null);
			}
			else
				PXUIFieldAttribute.SetEnabled<RQBiddingVendor.curyID>(sender, e.Row, false);

			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.curyTotalQuoteExtCost>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.totalQuoteExtCost>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.totalQuoteQty>(sender, e.Row, false);
			
			PORemitAddress remitAddress = this.Remit_Address.Select();
			bool enableAddressValidation = (remitAddress != null && remitAddress.IsDefaultAddress == false && remitAddress.IsValidated == false);
			this.validateAddresses.SetEnabled(enableAddressValidation);
		}
		
		protected virtual void RQBiddingVendor_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			if (row != null)
			{		
				PORemitAddressAttribute.DefaultRecord<RQBiddingVendor.remitAddressID>(sender, e.Row);
				PORemitContactAttribute.DefaultRecord<RQBiddingVendor.remitContactID>(sender, e.Row);					
			}
			BVendor.Current = (Vendor)BVendor.View.SelectSingleBound(new object[] { e.Row });
		}

		protected virtual void RQBiddingVendor_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			if (row != null)
			{
				if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>() && row.ReqNbr != null && row.VendorID != null)
				{
					BVendor.Current = (Vendor) BVendor.View.SelectSingleBound(new object[] {e.Row});
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<RQBiddingVendor.curyInfoID>(sender, e.Row);
					sender.SetDefaultExt<RQBiddingVendor.curyID>(e.Row);
				}
			}
		}

		#region CurrencyInfo events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				Vendor v = BVendor.Current;
				if (v != null && !string.IsNullOrEmpty(v.CuryID))
				{
					e.NewValue = v.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				Vendor v = BVendor.Current;
				if (v != null)
				{
					e.NewValue = v.CuryRateTypeID ?? cmsetup.Current.APRateTypeDflt;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Vendor.Cache.Current != null)
			{
				e.NewValue = ((RQBiddingVendor)Vendor.Cache.Current).EntryDate;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info != null)
			{
				bool curyenabled = info.IsReadOnly != true;
				bool curyrateenabled = info.AllowUpdate(this.Bidding.Cache);

				if (BVendor.Current != null)
				{
					if (BVendor.Current.AllowOverrideCury != true)
						curyenabled = false;

					if (BVendor.Current.AllowOverrideRate != true)
						curyrateenabled = false;
				}
				
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyID>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyrateenabled);
			}
		}
		#endregion
		[PXCancelButton]
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		protected virtual IEnumerable Cancel(PXAdapter a)
		{
			if (a.Searches != null && a.Searches.Length == 3)
			{
				if (this.Vendor.Current != null && this.Vendor.Current.ReqNbr != (string)a.Searches[0])
				{
					PXResult<RQBiddingVendor,Vendor, Location> res = (PXResult<RQBiddingVendor,Vendor, Location>)
						PXSelectJoin<RQBiddingVendor,						
						InnerJoin<Vendor, On<Vendor.bAccountID, Equal<RQBiddingVendor.vendorID>>,
						InnerJoin<Location, On<Location.bAccountID, Equal<Vendor.bAccountID>, And<Location.locationID, Equal<Vendor.defLocationID>>>>>,
						Where<RQBiddingVendor.reqNbr, Equal<Required<RQBiddingVendor.reqNbr>>,
							And<Match<Vendor,Current<AccessInfo.userName>>>>>
						.Select(this, (string)a.Searches[0]);

					Vendor vendor = null;  
					Location location = null;
					if (res != null)
					{
						vendor = res;
						location = res;
					}

					if (vendor == null || vendor.BAccountID == null)
					{
						a.Searches[1] = null;
						a.Searches[2] = null;
					}
					else
					{
						a.Searches[1] = vendor.AcctCD;
						a.Searches[2] = location.LocationCD;
					}
				}
				else
				{
					if (a.Searches[1] != null && a.Searches[2] != null)
					{
						Location loc = PXSelectJoin<Location,
							InnerJoin<Vendor,
										 On<Vendor.bAccountID, Equal<Location.bAccountID>>>,
							Where<Vendor.acctCD, Equal<Required<Vendor.acctCD>>,
							  And<Location.locationCD, Equal<Required<Location.locationCD>>,
								And<Match<Vendor, Current<AccessInfo.userName>>>>>>
								.SelectWindowed(this, 0, 1, a.Searches[1], a.Searches[2]);
						if(loc == null)
							a.Searches[2] = null;						
					}

					if (a.Searches[1] != null && a.Searches[2] == null)
					{					
							Location loc = PXSelectJoin<Location,
								InnerJoin<Vendor, 
											 On<Vendor.bAccountID, Equal<Location.bAccountID>, And<Vendor.defLocationID, Equal<Location.locationID>>>>,
								Where<Vendor.acctCD,Equal<Required<Vendor.acctCD>>,
									And<Match<Vendor,Current<AccessInfo.userName>>>>>
									.SelectWindowed(this, 0, 1, a.Searches[1]);
							if (loc != null)
								a.Searches[2] = loc.LocationCD;					
					}	
				}
				
			}
			foreach (object e in (new PXCancel<RQBiddingVendor>(this, "Cancel")).Press(a))							
				yield return e;			
		}		

		public class PXBiddingInsert<TNode> : PXInsert<TNode>
			where TNode : RQBiddingVendor, new()
		{
			public PXBiddingInsert(PXGraph graph, string name)
			: base(graph, name)
			{
			}
			[PXInsertButton]
			[PXUIField(DisplayName = "Insert", MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Insert)]
			protected override IEnumerable Handler(PXAdapter adapter)
			{
				List<object> items = new List<object>();				
				foreach(object item in base.Handler(adapter))
				{
					RQBiddingVendor b = item is PXResult ? (RQBiddingVendor)((PXResult)item)[0] : (RQBiddingVendor)item;
					b.VendorID = null;
					b.VendorLocationID = null;					
					items.Add(item);
					foreach (Type type in _Graph.Caches.Keys)
						_Graph.Caches[type].IsDirty = false;					
				}
				return items;
			}
		}
	}
}
