using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.CS;
using System.Collections;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.TM;

namespace PX.Objects.RQ
{
	//Cache merged
    [Serializable]
    [PXHidden]
	public partial class VendorContact : Contact
	{
		#region ContactID

		public new abstract class contactID : IBqlField { }

		#endregion
	}

	public class RQRequisitionEntry : PXGraph<RQRequisitionEntry>
	{
		public PXFilter<BAccount> cbaccount;
		public PXFilter<Vendor> cbendor;
		public PXFilter<EPEmployee> cemployee;

		public PXSave<RQRequisition> Save;
		public PXCancel<RQRequisition> Cancel;
		public PXInsert<RQRequisition> Insert;
		public PXDelete<RQRequisition> Delete;
		public PXFirst<RQRequisition> First;
		public PXPrevious<RQRequisition> Previous;
		public PXNext<RQRequisition> Next;
		public PXLast<RQRequisition> Last;


		[PXViewName(Messages.RQRequisition)]
		public PXSelectJoin<RQRequisition,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<RQRequisition.customerID>>,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<RQRequisition.vendorID>>>>,
			Where2<Where<Customer.bAccountID, IsNull,
								Or<Match<Customer, Current<AccessInfo.userName>>>>,
 				 And<Where<Vendor.bAccountID, IsNull,
								Or<Match<Vendor, Current<AccessInfo.userName>>>>>>> Document;

		public PXSelect<RQRequisition, Where<RQRequisition.reqNbr, Equal<Current<RQRequisition.reqNbr>>>> CurrentDocument;
		public PXSelect<InventoryItem> invItems;
		public PXSelect<RQRequisitionLine, Where<RQRequisitionLine.reqNbr, Equal<Optional<RQRequisition.reqNbr>>>> Lines;

		public PXSelectJoin<RQBiddingVendor,
					LeftJoin<Location, On<Location.bAccountID, Equal<RQBiddingVendor.vendorID>,
								And<Location.locationID, Equal<RQBiddingVendor.vendorLocationID>>>>,
					Where<RQBiddingVendor.reqNbr, Equal<Optional<RQRequisition.reqNbr>>>> Vendors;

		[PXViewName(CR.Messages.Employee)]
		public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<RQRequisition.employeeID>>>> Employee;  

		public PXSelect<RQBidding,
			Where<RQBidding.reqNbr, Equal<Current<RQRequisition.reqNbr>>>> Bidding;
		[PXViewName(PO.Messages.POShipAddress)]
		public PXSelect<POShipAddress, Where<POShipAddress.addressID, Equal<Current<RQRequisition.shipAddressID>>>> Shipping_Address;
		[PXViewName(PO.Messages.POShipContact)]
		public PXSelect<POShipContact, Where<POShipContact.contactID, Equal<Current<RQRequisition.shipContactID>>>> Shipping_Contact;
		[PXViewName(PO.Messages.PORemitAddress)]
		public PXSelect<PORemitAddress, Where<PORemitAddress.addressID, Equal<Current<RQRequisition.remitAddressID>>>> Remit_Address;
		[PXViewName(PO.Messages.PORemitContact)]
		public PXSelect<PORemitContact, Where<PORemitContact.contactID, Equal<Current<RQRequisition.remitContactID>>>> Remit_Contact;

		public PXFilter<RQRequisitionStatic> Filter;
		public PXFilter<RQRequestLineFilter> RequestFilter;

		public PXSelectJoin<RQRequisitionContent,
								InnerJoin<RQRequestLine,
											 On<RQRequestLine.orderNbr, Equal<RQRequisitionContent.orderNbr>,
											 And<RQRequestLine.lineNbr, Equal<RQRequisitionContent.lineNbr>>>,
								InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequisitionContent.orderNbr>>>>,
								Where<RQRequisitionContent.reqNbr, Equal<Current<RQRequisitionStatic.reqNbr>>,
									And<RQRequisitionContent.reqLineNbr, Equal<Current<RQRequisitionStatic.lineNbr>>>>> Contents;

		public PXSelectJoin<RQRequestLine,
					InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequestLine.orderNbr>,
								And<RQRequest.status, Equal<RQRequestStatus.open>>>>,
					Where<RQRequestLine.openQty, Greater<PX.Objects.CS.decimal0>,
					And<RQRequestLine.orderNbr, Equal<Required<RQRequestLine.orderNbr>>,
					And<RQRequestLine.lineNbr, Equal<Required<RQRequestLine.lineNbr>>>>>> SourceRequestLines;

		public PXSelectJoin<RQRequestLineSelect,
						 InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequestLineSelect.orderNbr>, And<RQRequest.status, Equal<RQRequestStatus.open>>>,
							LeftJoinSingleTable<Customer,On<Customer.bAccountID, Equal<RQRequest.employeeID>>>>,			
					Where<RQRequestLineSelect.openQty, Greater<PX.Objects.CS.decimal0>,
						And2<Where<Customer.bAccountID, IsNull,
								Or<Match<Customer, Current<AccessInfo.userName>>>>, 
						And<Where<Current<RQRequisition.customerID>, IsNull,
									 Or<RQRequest.employeeID, Equal<Current<RQRequisition.customerID>>,
									And<RQRequest.locationID, Equal<Current<RQRequisition.customerLocationID>>>>>>>>> SourceRequests;

		public PXSelect<RQRequisitionOrder, Where<RQRequisitionOrder.reqNbr, Equal<Optional<RQRequisition.reqNbr>>>> ReqOrders;

		public PXSelectJoin<POOrder,
				InnerJoin<RQRequisitionOrder,
							 On<RQRequisitionOrder.orderCategory, Equal<RQOrderCategory.po>,
							 And<RQRequisitionOrder.orderType, Equal<POOrder.orderType>,
							And<RQRequisitionOrder.orderNbr, Equal<POOrder.orderNbr>>>>>,
			Where<RQRequisitionOrder.reqNbr, Equal<Optional<RQRequisition.reqNbr>>>> POOrders;

		public PXSelectJoin<SOOrder,
				InnerJoin<RQRequisitionOrder,
							 On<RQRequisitionOrder.orderCategory, Equal<RQOrderCategory.so>,
							 And<RQRequisitionOrder.orderType, Equal<SOOrder.orderType>,
							And<RQRequisitionOrder.orderNbr, Equal<SOOrder.orderNbr>>>>>,
			Where<RQRequisitionOrder.reqNbr, Equal<Optional<RQRequisition.reqNbr>>>> SOOrders;

		public PXSelect<POOrder, Where<POOrder.rQReqNbr, Equal<Current<RQRequisition.reqNbr>>>> Orders;
		public PXSelectJoin<POLine,
				InnerJoin<POOrder,
							 On<POOrder.orderType, Equal<POLine.orderType>,
							And<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
			Where<POLine.rQReqNbr, Equal<Current<RQRequisitionStatic.reqNbr>>,
			And<POLine.rQReqLineNbr, Equal<Current<RQRequisitionStatic.lineNbr>>>>> OrderLines;

		[PXHidden]
		public PXSelect<VendorContact,
			Where<VendorContact.contactID, Equal<Current<Vendor.defContactID>>>> 
			VndrCont;

		[PXHidden]
		[CRReference(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<RQRequisition.vendorID>>>>))]
		[CRDefaultMailTo(typeof(Select2<VendorContact, 
			InnerJoin<Vendor, On<VendorContact.contactID, Equal<Vendor.defContactID>>>, 
			Where<Vendor.bAccountID, Equal<Current<RQRequisition.vendorID>>>>))]
		public CRActivityList<RQRequisition>
			Activity;

		[PXHidden]
		[CRReference(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<RQBiddingVendor.vendorID>>>>))]
		[CRDefaultMailTo(typeof(Select2<VendorContact,
			InnerJoin<Vendor, On<VendorContact.contactID, Equal<Vendor.defContactID>>>,
			Where<Vendor.bAccountID, Equal<Current<RQBiddingVendor.vendorID>>>>))]
		public CRActivityList<RQRequisition>
			BidActivity;

		public PXSelect<RQSetupApproval, Where<RQSetupApproval.type, Equal<RQType.requisition>>> SetupApproval;
		[PXViewName(Messages.Approval)]
		public EPApprovalAutomation<RQRequisition, RQRequisition.approved, RQRequisition.rejected, RQRequisition.hold, RQSetupApproval> Approval;

		public PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Optional<RQRequisition.vendorID>>>> bAccount;
		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<RQRequisition.vendorID>>>> vendor;
		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Optional<POOrder.vendorID>>, And<Location.locationID, Equal<Optional<POOrder.vendorLocationID>>>>> location;
		public PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<RQRequisition.customerID>>>> customer;
		public PXSetup<CustomerClass, Where<CustomerClass.customerClassID, Equal<Optional<Customer.customerClassID>>>> customerclass;
		public PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<RQBiddingVendor.vendorID>>>> vendorBidder;



		public PXSelect<PORemitAddress, Where<PORemitAddress.addressID, Equal<Current<RQBiddingVendor.remitAddressID>>>> Bidding_Remit_Address;
		public PXSelect<PORemitContact, Where<PORemitContact.contactID, Equal<Current<RQBiddingVendor.remitContactID>>>> Bidding_Remit_Contact;

		public PXSetup<RQSetup> Setup;
		public PXSetup<INSetup> INSetup;
		public CMSetupSelect cmsetup;
		public PXSetup<Company> company;
		public PXSelect<RQRequest> request;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Optional<RQRequisition.curyInfoID>>>> currencyinfo;
		public ToggleCurrency<RQRequisition> CurrencyView;

        #region SiteStatus Lookup
        public PXFilter<POSiteStatusFilter> sitestatusfilter;
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public INSiteStatusLookup<RQSiteStatusSelected, INSiteStatusFilter> sitestatus;

		public PXAction<RQRequisition> addInvBySite;
		[PXUIField(DisplayName = "Add Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AddInvBySite(PXAdapter adapter)
		{
			sitestatusfilter.Cache.Clear();
			if (sitestatus.AskExt() == WebDialogResult.OK)
			{
				return AddInvSelBySite(adapter);
			}
			sitestatusfilter.Cache.Clear();
			sitestatus.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<RQRequisition> addInvSelBySite;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddInvSelBySite(PXAdapter adapter)
		{
			foreach (RQSiteStatusSelected line in sitestatus.Cache.Cached)
			{
				if (line.Selected == true && line.QtySelected > 0)
				{
					RQRequisitionLine newline = new RQRequisitionLine();
					newline.SiteID = line.SiteID;
					newline.InventoryID = line.InventoryID;
					newline.SubItemID = line.SubItemID;
					newline.UOM = line.PurchaseUnit;
					newline.OrderQty = line.QtySelected;
					Lines.Insert(newline);
				}
			}
			sitestatus.Cache.Clear();
			return adapter.Get();
		}
		protected virtual void POSiteStatusFilter_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			POSiteStatusFilter row = (POSiteStatusFilter)e.Row;
			if (row != null && Document.Current != null)
			{
				PXUIFieldAttribute.SetEnabled<POSiteStatusFilter.onlyAvailable>(sitestatusfilter.Cache, row, Document.Current.VendorID != null);
				row.OnlyAvailable = Document.Current.VendorID != null;
				row.VendorID = Document.Current.VendorID;
			}
		}

		public PXAction<RQRequisition> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (RQRequisition current in adapter.Get<RQRequisition>())
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

					POShipAddress shipAddress = this.Shipping_Address.Select();
					if (shipAddress != null && shipAddress.IsDefaultAddress == false && shipAddress.IsValidated == false)
					{
						if (PXAddressValidator.Validate<POShipAddress>(this, shipAddress, true))
							needSave = true;
					}
					if (needSave == true)
						this.Save.Press();
				}
				yield return current;
			}
		}
		#endregion

		#region EPApproval Cahce Attached
		[PXDBDate()]
		[PXDefault(typeof(RQRequisition.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(RQRequisition.employeeID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(RQRequisition.description), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		[CurrencyInfo(typeof(RQRequisition.curyInfoID))]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(RQRequisition.curyEstExtCostTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(RQRequisition.estExtCostTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region Redefine attributes
		[PXDBDate()]
		[PXUIField(DisplayName = "Entry Date", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void RQBiddingVendor_EntryDate_CacheAttached(PXCache sender)
		{
		}
		#endregion
		public RQRequisitionEntry()
		{
			PXUIFieldAttribute.SetEnabled(SourceRequests.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<RQRequestLineSelect.selected>(SourceRequests.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<RQRequestLineSelect.selectQty>(SourceRequests.Cache, null, true);
			this.Views.Caches.Add(typeof(RQRequestLine));
			this.SourceRequests.WhereAndCurrent<RQRequestLineFilter>(
				typeof(RQRequestLineFilter.ownerID).Name,
				typeof(RQRequestLineFilter.workGroupID).Name);
			PXUIFieldAttribute.SetVisible<RQRequestLineFilter.subItemID>(this.RequestFilter.Cache, null,
				INSetup.Current.UseInventorySubItem == true);

			PXStringListAttribute.SetList<InventoryItem.itemType>(
					invItems.Cache, null,
					new string[] { INItemTypes.FinishedGood, INItemTypes.Component, INItemTypes.SubAssembly, INItemTypes.NonStockItem, INItemTypes.LaborItem, INItemTypes.ServiceItem, INItemTypes.ChargeItem, INItemTypes.ExpenseItem },
					new string[] { IN.Messages.FinishedGood, IN.Messages.Component, IN.Messages.SubAssembly, IN.Messages.NonStockItem, IN.Messages.LaborItem, IN.Messages.ServiceItem, IN.Messages.ChargeItem, IN.Messages.ExpenseItem }
					);

			this.POOrders.Cache.AllowInsert = this.POOrders.Cache.AllowUpdate = this.POOrders.Cache.AllowDelete = false;
			this.SOOrders.Cache.AllowInsert = this.SOOrders.Cache.AllowUpdate = this.SOOrders.Cache.AllowDelete = false;
		}

		public PXAction<RQRequisition> ViewBidding;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.BiddingBtn, Visible = false)]
		public virtual IEnumerable viewBidding(PXAdapter adapter)
		{
			if (this.IsDirty) this.Save.Press();
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				RQBiddingProcess graph = PXGraph.CreateInstance<RQBiddingProcess>();
				graph.Document.Current = graph.Document.Search<RQRequisition.reqNbr>(item.ReqNbr);
				if (graph.Document.Current != null)
				{
					graph.State.Current.SingleMode = true;
					throw new PXPopupRedirectException(graph, "View Bidding", true);
				}
				yield return item;
			}
		}

		public PXAction<RQRequisition> Transfer;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(Visible = false)]
		public virtual IEnumerable transfer(PXAdapter adapter)
		{
			RQRequisition destination = null;
			foreach (RQRequisition req in adapter.Get<RQRequisition>())
			{
				foreach (RQRequisitionLine line in PXSelect<RQRequisitionLine,
								Where<RQRequisitionLine.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>,
									And<RQRequisitionLine.transferRequest, Equal<Required<RQRequisitionLine.transferRequest>>>>>.Select(this, req.ReqNbr, true))
				{
					if (destination == null)
					{
						string source = null;
						if (Filter.AskExt() == WebDialogResult.OK)
							source = Filter.Current.ReqNbr;
						else
							yield break;

						if (source != null)
							this.Document.Current = this.Document.Search<RQRequisition.reqNbr>(source);
						else
							this.Document.Cache.Insert();
						destination = this.Document.Current;
					}
					this.Lines.Cache.Delete(line);
					RQRequisitionLine newLine = (RQRequisitionLine)this.Lines.Cache.CreateCopy(line);
					newLine.ReqNbr = destination.ReqNbr;
					newLine.LineNbr = null;
					newLine.AlternateID = null;
					newLine.TransferQty = 0m;
					newLine.TransferRequest = false;
					newLine.TransferType = RQTransferType.Transfer;
					newLine.SourceTranReqNbr = line.ReqNbr;
					newLine.SourceTranLineNbr = line.LineNbr;
					this.Lines.Insert(newLine);
				}
				yield return req;
			}

			if (destination == null)
				throw new PXException(Messages.TransferLinesNotExsist);
		}

		public PXAction<RQRequisition> Merge;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.BoxIn)]
		[PXUIField(DisplayName = Messages.MergeLines)]
		public virtual IEnumerable merge(PXAdapter adapter)
		{
			foreach (RQRequisition req in adapter.Get<RQRequisition>())
			{
				RQRequisitionLine destination = null;
				bool merged = false;
				foreach (RQRequisitionLine line in
								PXSelect<RQRequisitionLine,
								Where<RQRequisitionLine.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>>,
								OrderBy<Asc<RQRequisitionLine.byRequest,
												Asc<RQRequisitionLine.inventoryID,
												Asc<RQRequisitionLine.uOM,
												Asc<RQRequisitionLine.expenseAcctID,
												Asc<RQRequisitionLine.expenseSubID>>>>>>>.Select(this, req.ReqNbr, true))
				{
					if (line.Selected != true) continue;

					if (destination == null ||
							destination.InventoryID != line.InventoryID ||
							destination.ExpenseAcctID != line.ExpenseAcctID ||
							destination.ExpenseSubID != line.ExpenseSubID ||
							destination.ByRequest != line.ByRequest)
					{
						if (destination != null && !merged)
							this.Lines.Cache.RaiseExceptionHandling<RQRequisitionLine.selected>(destination, true,
								new PXSetPropertyException(
									(destination.InventoryID == null) ? Messages.MergeLinesInventoryID :
									Messages.MergeLinesNoSource, PXErrorLevel.Warning));

						destination = line;
						destination.Selected = false;
						merged = false;
						continue;
					}
					merged = true;

					decimal? curyEstUnitCost = line.CuryEstUnitCost;
					decimal? curyEstExtCost = line.CuryEstExtCost;
					if (destination.CuryEstExtCost == 0)
						destination.CuryEstExtCost = curyEstUnitCost * destination.OrderQty;

					if (line.ByRequest == true)
					{
						Decimal? newCuryExyExtCost = destination.CuryEstExtCost + curyEstExtCost;
						foreach (PXResult<RQRequisitionContent, RQRequestLine> rec in
							PXSelectJoin<RQRequisitionContent,
								InnerJoin<RQRequestLine, On<RQRequestLine.orderNbr, Equal<RQRequisitionContent.orderNbr>,
									And<RQRequestLine.lineNbr, Equal<RQRequisitionContent.lineNbr>>>>,
								Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
									And<RQRequisitionContent.reqLineNbr, Equal<Required<RQRequisitionContent.reqLineNbr>>>>>.Select(this,
																																																									line.ReqNbr,
																																																									line.LineNbr))
						{
							RQRequisitionContent c = (RQRequisitionContent)rec;
							RQRequestLine i = (RQRequestLine)rec;
							this.Contents.Delete(c);
							RQRequisitionContent content = this.UpdateContent(destination, i, c.ItemQty.GetValueOrDefault());
							if (c.ItemQty.GetValueOrDefault() > 0 && content.ReqQty.GetValueOrDefault() == 0m)
							{
								content.ReqQty =
									INUnitAttribute.ConvertFromBase(this.Lines.Cache, line.InventoryID, line.UOM,
																									content.BaseReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
								this.Contents.Update(content);
							}

							if (destination.CuryEstUnitCost != curyEstUnitCost)
							{
								destination = PXCache<RQRequisitionLine>.CreateCopy(destination);
								destination.CuryEstUnitCost = newCuryExyExtCost / destination.OrderQty;
								destination = (RQRequisitionLine)this.Lines.Cache.Update(destination);
							}
						}
					}
					else
					{
						destination = PXCache<RQRequisitionLine>.CreateCopy(destination);
						decimal? qty = (destination.UOM == line.UOM)
							? line.OrderQty
							: INUnitAttribute.ConvertFromBase(this.Lines.Cache, line.InventoryID, line.UOM, line.BaseOrderQty.GetValueOrDefault(), INPrecision.QUANTITY);

						if (destination.CuryEstUnitCost != curyEstUnitCost)
							destination.CuryEstUnitCost = (destination.CuryEstExtCost + curyEstExtCost) / (destination.OrderQty + qty);
						destination.OrderQty += qty;

						destination = (RQRequisitionLine)this.Lines.Cache.Update(destination);
					}

					this.Lines.Cache.Delete(line);
				}
				if (!merged && destination != null)
				{
					this.Lines.Cache.RaiseExceptionHandling<RQRequisitionLine.selected>(destination, true,
								new PXSetPropertyException(
									(destination.InventoryID == null) ? Messages.MergeLinesInventoryID :
									Messages.MergeLinesNoSource, PXErrorLevel.Warning));
				}
				yield return req;
			}
		}

		public PXAction<RQRequisition> AddRequestLine;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.AddRequest)]
		public virtual IEnumerable addRequestLine(PXAdapter adapter)
		{
			PXView.InitializePanel initAction = delegate(PXGraph graph, string view)
			{
				PXCache filter = graph.Caches[typeof(RQRequestLineFilter)];
				RQRequestLineFilter filterCurrent = (RQRequestLineFilter)filter.CreateCopy(filter.Current);
				filterCurrent.InventoryID = null;
				filterCurrent.SubItemID = null;
				filterCurrent.AddExists = this.Setup.Current.RequisitionMergeLines;
				filterCurrent.AllowUpdate = true;
				filter.Update(filterCurrent);
			};
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				if (item.Hold == true && this.SourceRequests.AskExt(initAction) == WebDialogResult.OK)
				{
					foreach (RQRequestLineSelect line in this.SourceRequests.Select())
					{
						if (line.Selected == true && line.SelectQty > 0m)
						{
							RQRequestLine upd = this.SourceRequestLines.SelectWindowed(0, 1, line.OrderNbr, line.LineNbr);
							if (upd != null)
								InsertRequestLine(upd, line.SelectQty.GetValueOrDefault(), this.RequestFilter.Current.AddExists == true);
						}
					}
					this.SourceRequests.Cache.Clear();
					this.SourceRequests.View.Clear();
					this.SourceRequests.View.RequestRefresh();
				}
				yield return item;
			}
		}

		public PXAction<RQRequisition> AddRequestContent;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
		[PXUIField(DisplayName = Messages.AddRequest)]
		public virtual IEnumerable addRequestContent(PXAdapter adapter)
		{
			PXView.InitializePanel initAction = delegate(PXGraph graph, string view)
			{
				PXCache filter = graph.Caches[typeof(RQRequestLineFilter)];

				RQRequisitionLine line = PXSelect<RQRequisitionLine,
					Where<RQRequisitionLine.reqNbr, Equal<Current<RQRequisitionStatic.reqNbr>>,
						And<RQRequisitionLine.lineNbr, Equal<Current<RQRequisitionStatic.lineNbr>>>>>.Select(this);
				RQRequestLineFilter filterCurrent = (RQRequestLineFilter)filter.CreateCopy(filter.Current);
				if (line != null)
				{
					filterCurrent.InventoryID = line.InventoryID;
					filterCurrent.SubItemID = line.SubItemID;
					filterCurrent.AddExists = true;
					filterCurrent.AllowUpdate = false;
				}
				filter.Update(filterCurrent);
			};
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				if (item.Hold == true && this.SourceRequests.AskExt(initAction) == WebDialogResult.OK)
				{
					foreach (RQRequestLineSelect line in this.SourceRequests.Select())
					{
						if (line.Selected == true && line.SelectQty > 0m)
						{
							RQRequestLine upd = this.SourceRequestLines.SelectWindowed(0, 1, line.OrderNbr, line.LineNbr);
							InsertRequestLine(upd, line.SelectQty.GetValueOrDefault(),
								true);
						}
						line.Selected = false;
						line.SelectQty = 0;
						this.SourceRequests.Cache.Update(line);
					}
					this.Contents.View.RequestRefresh();
				}
				yield return item;
			}
		}

		public PXAction<RQRequisition> ViewDetails;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.RequestDetails)]
		public virtual IEnumerable viewDetails(PXAdapter adapter)
		{
			PXView.InitializePanel initAction = delegate(PXGraph graph, string view)
			{
				PXCache filter = graph.Caches[typeof(RQRequisitionStatic)];
				RQRequisitionLine line = graph.Caches[typeof(RQRequisitionLine)].Current as RQRequisitionLine;
				RQRequisitionStatic filterCurrent = new RQRequisitionStatic();
				if (line != null)
				{
					filterCurrent.ReqNbr = line.ReqNbr;
					filterCurrent.LineNbr = line.LineNbr;
				}
				filter.Current = filterCurrent;
			};
			if (this.Lines.Current != null && this.Lines.Current.ByRequest == true)
				this.Contents.AskExt(initAction);
			this.Contents.ClearDialog();
			return adapter.Get();
		}

		public PXAction<RQRequisition> ViewRequest;
		[PXUIField(DisplayName = Messages.ViewRequest)]
		[PXLookupButton]
		public virtual IEnumerable viewRequest(PXAdapter adapter)
		{
			if (this.Contents.Current != null)
			{
				EntityHelper helper = new EntityHelper(this);
				helper.NavigateToRow(typeof(RQRequest).FullName, new object[] { this.Contents.Current.OrderNbr }, PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		public PXAction<RQRequisition> ViewPOOrder;
		[PXUIField(DisplayName = Messages.ViewOrder)]
		[PXLookupButton]
		public virtual IEnumerable viewPOOrder(PXAdapter adapter)
		{
			if (this.POOrders.Current != null)
			{
				POOrderEntry graph = PXGraph.CreateInstance<POOrderEntry>();
				graph.Document.Current = graph.Document.Search<POOrder.orderNbr>(this.POOrders.Current.OrderNbr, this.POOrders.Current.OrderType);
				if (graph.Document.Current != null)
					throw new PXRedirectRequiredException(graph, true, "View Order"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
			}
			return adapter.Get();
		}
		public PXAction<RQRequisition> ViewSOOrder;
		[PXUIField(DisplayName = Messages.ViewOrder)]
		[PXLookupButton]
		public virtual IEnumerable viewSOOrder(PXAdapter adapter)
		{
			if (this.SOOrders.Current != null)
			{
				SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
				graph.Document.Current = graph.Document.Search<SOOrder.orderNbr>(this.SOOrders.Current.OrderNbr, this.SOOrders.Current.OrderType);
				if (graph.Document.Current != null)
					throw new PXRedirectRequiredException(graph, true, "View Order") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}
		public PXAction<RQRequisition> ViewOrderByLine;
		[PXUIField(DisplayName = Messages.ViewOrder)]
		[PXLookupButton]
		public virtual IEnumerable viewOrderByLine(PXAdapter adapter)
		{
			if (this.OrderLines.Current != null)
			{
				POOrderEntry graph = PXGraph.CreateInstance<POOrderEntry>();
				graph.Document.Current = graph.Document.Search<POOrder.orderNbr>(this.OrderLines.Current.OrderNbr, this.OrderLines.Current.OrderType);
				if (graph.Document.Current != null)
					throw new PXRedirectRequiredException(graph, true, "View Order"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
			}
			return adapter.Get();
		}

		public PXAction<RQRequisition> Assign;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.Assign, Visible = false)]
		public virtual IEnumerable assign(PXAdapter adapter)
		{
			foreach (RQRequisition req in adapter.Get<RQRequisition>())
			{
				if (Setup.Current.RequisitionAssignmentMapID != null)
				{
					var helper = new EPAssignmentProcessHelper<RQRequisition>();
					helper.Assign(req, Setup.Current.RequisitionAssignmentMapID);
                    req.WorkgroupID = req.ApprovalWorkgroupID;
                    req.OwnerID = req.ApprovalOwnerID;

				}
				yield return req;
			}
		}

		public PXAction<RQRequisition> createPOOrder;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.CreateOrders)]
		public virtual IEnumerable CreatePOOrder(PXAdapter adapter)
		{
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				this.Document.Current = item;
				bool validateResult = true;
				foreach (RQRequisitionLine line in this.Lines.Select(item.ReqNbr))
				{
					if (!ValidateOpenState(line, PXErrorLevel.Error))
						validateResult = false;
				}
				if (!validateResult)
					throw new PXRowPersistingException(typeof(RQRequisition).Name, item, Messages.UnableToCreateOrders);

				this.Document.Current = this.Document.Search<RQRequisition.reqNbr>(item.ReqNbr);

				using (PXTransactionScope scope = new PXTransactionScope())
				{
					try
					{
						POOrderEntry graph = PXGraph.CreateInstance<POOrderEntry>();			
						graph.TimeStamp = this.TimeStamp;
						graph.Document.Current = null;

						POOrder oldOrder = PXSelect<POOrder, Where<POOrder.rQReqNbr, Equal<Required<POOrder.rQReqNbr>>>>.SelectWindowed(this, 0, 1, item.ReqNbr);
						if (oldOrder == null)
						{
							PO4SO po4so = new PO4SO();
							if (item.VendorID != null && item.VendorLocationID != null)
							{
								POOrder order = (POOrder)graph.Document.Cache.CreateInstance();
								order.OrderType = item.POType;

								foreach (PXResult<RQRequisitionLine, RQBidding, RQBiddingVendor,RQRequisitionLineCustomers> rec in
									PXSelectJoin<RQRequisitionLine,
										LeftJoin<RQBidding,
													On<RQBidding.reqNbr, Equal<RQRequisitionLine.reqNbr>,
													And<RQBidding.lineNbr, Equal<RQRequisitionLine.lineNbr>,
													And<RQBidding.vendorID, Equal<Current<RQRequisition.vendorID>>,
													And<RQBidding.vendorLocationID, Equal<Current<RQRequisition.vendorLocationID>>>>>>,
										LeftJoin<RQBiddingVendor,
												On<RQBiddingVendor.reqNbr, Equal<RQBidding.reqNbr>,
												And<RQBiddingVendor.vendorID, Equal<RQBidding.vendorID>,
												And<RQBiddingVendor.vendorLocationID, Equal<RQBidding.vendorLocationID>>>>,
										LeftJoin<RQRequisitionLineCustomers,
													On<RQRequisitionLineCustomers.reqNbr, Equal<RQRequisitionLine.reqNbr>,
												 And<RQRequisitionLineCustomers.reqLineNbr, Equal<RQRequisitionLine.lineNbr>>>>>>,
									Where<RQRequisitionLine.reqNbr, Equal<Current<RQRequisition.reqNbr>>>>
									.SelectMultiBound(this, new object[]{item}))
								{
									RQRequisitionLine line = rec;
									RQRequisitionLineCustomers cust = rec;
									RQBiddingVendor biddier = rec;
									RQBidding bid = rec;

									decimal custqty = 0, qty = line.OrderQty.Value;

									if (bid.OrderQty == null || (bid.OrderQty == 0 && qty < bid.MinQty)) 
										bid = new RQBidding() { MinQty = 0, QuoteQty = 0, OrderQty = 0, CuryQuoteUnitCost = 0 };
									
									if (bid.OrderQty == 0) bid.OrderQty = bid.QuoteQty;	
								
									if (cust.ReqQty > 0) custqty = cust.ReqQty.Value;
									if (item.CustomerLocationID != null)custqty = qty;

									if (custqty > 0) qty -= custqty;									
	
									if (order != null)
									{
										order = PXCache<POOrder>.CreateCopy(graph.Document.Insert(order));
										order.VendorID = item.VendorID;
										order.VendorLocationID = item.VendorLocationID;
										order.RemitAddressID = item.RemitAddressID;
										order.RemitContactID = item.RemitContactID;
										order.VendorRefNbr = item.VendorRefNbr;
										order.TermsID = item.TermsID;
										order = PXCache<POOrder>.CreateCopy(graph.Document.Update(order));
										if (bid.CuryID != null)
										{
											order.CuryID = (string) Bidding.GetValueExt<RQBidding.curyID>(bid);
											order.CuryInfoID = CopyCurrenfyInfo(graph, bid.CuryInfoID);
										}
										else
										{
											order.CuryID = item.CuryID;
											order.CuryInfoID = CopyCurrenfyInfo(graph, item.CuryInfoID);											
										}
										order.FOBPoint = item.FOBPoint;
										order.ShipDestType = item.ShipDestType;
										order.ShipToBAccountID = item.ShipToBAccountID;
										order.ShipToLocationID = item.ShipToLocationID;
										order.ShipContactID = item.ShipContactID;
										order.ShipAddressID = item.ShipAddressID;
										order.ShipContactID = item.ShipContactID;
										order.ShipVia = biddier.ShipVia;
										order.FOBPoint = biddier.FOBPoint;
										order.RQReqNbr = item.ReqNbr;
										order = graph.Document.Update(order);
										order = null;
									}
									InventoryItem inv = (InventoryItem)PXSelectorAttribute.Select<RQRequisitionLine.inventoryID>(this.Lines.Cache, line);
									string lineType = inv != null && inv.StkItem == true ? POLineType.GoodsForSalesOrder : line.LineType;									
									
								
									Decimal minQty = custqty < bid.OrderQty ? custqty: bid.OrderQty.Value;
									po4so.Add(line.LineNbr, InsertPOLine(graph, line, minQty, bid.CuryQuoteUnitCost, lineType));
									custqty -= minQty;
									bid.OrderQty -= minQty;

									if (custqty > 0)
										po4so.Add(line.LineNbr, InsertPOLine(graph, line, custqty, line.CuryEstUnitCost, lineType));

									minQty = qty < bid.OrderQty ? qty : bid.OrderQty.Value;
									InsertPOLine(graph, line, minQty, bid.CuryQuoteUnitCost, line.LineType);
									qty -= minQty;


									if(qty > 0)
										InsertPOLine(graph, line, qty, line.CuryEstUnitCost, line.LineType);									
								}
							}
							else
							{
								Dictionary<int?, Decimal> usedLine = new Dictionary<int?, Decimal>();
								foreach (PXResult<RQBidding, RQRequisitionLine, RQRequisitionLineCustomers, Vendor, RQBiddingVendor> e
										in
									PXSelectJoin<RQBidding,
									InnerJoin<RQRequisitionLine,
												 On<RQRequisitionLine.reqNbr, Equal<RQBidding.reqNbr>,
												And<RQRequisitionLine.lineNbr, Equal<RQBidding.lineNbr>>>,
									LeftJoin<RQRequisitionLineCustomers,
												On<RQRequisitionLineCustomers.reqNbr, Equal<RQRequisitionLine.reqNbr>,
											 And<RQRequisitionLineCustomers.reqLineNbr, Equal<RQRequisitionLine.lineNbr>>>,
									InnerJoin<Vendor,
													On<Vendor.bAccountID, Equal<RQBidding.vendorID>>,
									LeftJoin<RQBiddingVendor,
												On<RQBiddingVendor.reqNbr, Equal<RQBidding.reqNbr>,
												And<RQBiddingVendor.vendorID, Equal<RQBidding.vendorID>,
												And<RQBiddingVendor.vendorLocationID, Equal<RQBidding.vendorLocationID>>>>>>>>,
								 Where<RQBidding.reqNbr, Equal<Required<RQBidding.reqNbr>>,
									 And<RQBidding.orderQty, Greater<CS.decimal0>>>,
								OrderBy<Asc<RQBidding.vendorID, Asc<RQBidding.vendorLocationID, Asc<RQRequisitionLine.lineNbr>>>>>.Select(this, item.ReqNbr))
								{
									RQBidding bidding = e;
									RQRequisitionLine line = e;
									RQBiddingVendor biddier = e;
									RQRequisitionLineCustomers cust = e;
									Vendor vendor = e;
									POOrder order = null;
									if (graph.Document.Current == null ||
										 graph.Document.Current.VendorID != bidding.VendorID ||
										 graph.Document.Current.VendorLocationID != bidding.VendorLocationID)
									{
										if (graph.IsDirty)
										{
											PersistOrder(graph);
											graph.Clear();
											graph.TimeStamp = this.TimeStamp;
										}
										order = (POOrder)graph.Document.Cache.CreateInstance();
									}
									Decimal custQty = 0;
									Decimal qty = bidding.OrderQty.Value;

									int? key = bidding != null ? bidding.LineID : line.LineNbr;

									if (cust.ReqQty == null && item.CustomerLocationID != null)
										custQty = qty;

									if (cust.ReqQty > 0)
										custQty = cust.ReqQty.Value;


									if (custQty > 0 && usedLine.ContainsKey(key))
										custQty -= usedLine[key];

									if (custQty > qty)
										custQty = qty;

									if (qty > 0 && order != null)
									{
										order.OrderType = item.POType;
										order = PXCache<POOrder>.CreateCopy(graph.Document.Insert(order));
										order.VendorID = bidding.VendorID;
										order.VendorLocationID = bidding.VendorLocationID;
										order = PXCache<POOrder>.CreateCopy(graph.Document.Update(order));
										order.CuryID = (string)Bidding.GetValueExt<RQBidding.curyID>(bidding);
										order.CuryInfoID = CopyCurrenfyInfo(graph, bidding.CuryInfoID);
										order.ShipDestType = item.ShipDestType;
										order.ShipToBAccountID = item.ShipToBAccountID;
										order.ShipToLocationID = item.ShipToLocationID;
										order.ShipContactID = item.ShipContactID;
										order.ShipAddressID = item.ShipAddressID;
										order.ShipVia = biddier.ShipVia;
										order.FOBPoint = biddier.FOBPoint;
										order.RemitAddressID = biddier.RemitAddressID;
										order.RemitContactID = biddier.RemitContactID;
										order.RQReqNbr = item.ReqNbr;
										order = graph.Document.Update(order);
										order = null;
									}

									if (custQty > 0)
									{
										po4so.Add(line.LineNbr, InsertPOLine(graph, line, custQty, bidding.CuryQuoteUnitCost, line.LineType));
										if (!usedLine.ContainsKey(key)) usedLine[key] = 0;
										usedLine[key] += custQty;
									}
									if (qty > custQty)
										InsertPOLine(graph, line, qty - custQty, bidding.CuryQuoteUnitCost, line.LineType);
								}
							}
							
							PersistOrder(graph);
							graph.Clear();
							if (po4so.Count > 0)
							{
								SOOrderEntry sograph = PXGraph.CreateInstance<SOOrderEntry>();
								PXCache vendorCache = sograph.Caches[typeof(Vendor)];

								sograph.TimeStamp = this.TimeStamp;
								sograph.Document.Current = null;
								sograph.SOPOLinkShowDocumentsOnHold = true;

								foreach (RQSOSource e in GetSOSource(item))
								{
									if (sograph.Document.Current == null ||
										sograph.Document.Current.CustomerID != e.CustomerID ||
										sograph.Document.Current.CustomerLocationID != e.CustomerLocationID)
									{
										if (sograph.IsDirty)
											sograph.Save.Press();

									    PXResultset<SOOrder> quotes = PXSelectJoin<SOOrder,
									        InnerJoin<RQRequisitionOrder,
									            On<RQRequisitionOrder.orderType, Equal<SOOrder.orderType>,
									           And<RQRequisitionOrder.orderNbr, Equal<SOOrder.orderNbr>>>>,
									        Where<RQRequisitionOrder.orderCategory, Equal<Required<RQRequisitionOrder.orderCategory>>,
									          And<RQRequisitionOrder.reqNbr, Equal<Required<RQRequisitionOrder.reqNbr>>,
									          And<SOOrder.status, Equal<SOOrderStatus.open>>>>>.SelectWindowed(this, 0, 2,
									                    RQOrderCategory.SO, item.ReqNbr);

										SOOrder order = (SOOrder)sograph.Document.Cache.CreateInstance();
										order.OrderType = "SO";
										order = sograph.Document.Insert(order);
										order = PXCache<SOOrder>.CreateCopy(sograph.Document.Search<SOOrder.orderNbr>(order.OrderNbr));
										order.CustomerID = e.CustomerID;
										order.CustomerLocationID = e.CustomerLocationID;
										order = PXCache<SOOrder>.CreateCopy(sograph.Document.Update(order));
										order.CuryID = item.CuryID;
										order.CuryInfoID = CopyCurrenfyInfo(sograph, item.CuryInfoID);
									    if (quotes.Count == 1)
									    {
									        SOOrder quote = quotes[0];
									        order.OrigOrderType = quote.OrderType;
									        order.OrigOrderNbr = quote.OrderNbr;
									    }
										order = sograph.Document.Update(order);
										sograph.Save.Press();

										RQRequisitionOrder link = new RQRequisitionOrder();
										link.OrderCategory = RQOrderCategory.SO;
										link.OrderType = sograph.Document.Current.OrderType;
										link.OrderNbr = sograph.Document.Current.OrderNbr;
										this.ReqOrders.Insert(link);
									}

									if (!po4so.ContainsKey(e.LineNbr)) continue;

									foreach (POLine poline in po4so[e.LineNbr])
									{
										if (poline.OrderQty > 0)
										{
											Decimal qty = poline.OrderQty.Value;
											if (e.OrderQty < qty)
												qty = e.OrderQty.Value;
											if (qty > 0)
											{
												SOLine quote =
													PXSelectJoin<SOLine,
													InnerJoin<RQRequisitionLine,
																 On<RQRequisitionLine.qTOrderNbr, Equal<SOLine.orderNbr>,
																And<RQRequisitionLine.qTLineNbr, Equal<SOLine.lineNbr>>>,
													InnerJoin<SOOrder,
																 On<SOOrder.orderType, Equal<SOLine.orderType>,
																 And<SOOrder.orderNbr, Equal<SOLine.orderNbr>,
																And<SOOrder.status, Equal<SOOrderStatus.open>>>>>>,
													Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
														And<RQRequisitionLine.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>,
														And<RQRequisitionLine.lineNbr, Equal<Required<RQRequisitionLine.lineNbr>>>>>>
														.Select(this, "QT", poline.RQReqNbr, poline.RQReqLineNbr);

												SOLine line = (SOLine)sograph.Transactions.Cache.CreateInstance();
												line.OrderType = sograph.Document.Current.OrderType;
												line.OrderNbr = sograph.Document.Current.OrderNbr;
												line = PXCache<SOLine>.CreateCopy(sograph.Transactions.Insert(line));
												line.InventoryID = e.InventoryID;
												if (line.InventoryID != null)
													line.SubItemID = e.SubItemID;												
												line.UOM = e.UOM;
												line.Qty = qty;
												line.SiteID = poline.SiteID;
												if (quote != null)
												{
													line.ManualPrice = true;
													line.CuryUnitPrice = quote.CuryUnitPrice;
                                                    line.OrigOrderType = quote.OrderType;
                                                    line.OrigOrderNbr = quote.OrderNbr;
                                                    line.OrigLineNbr = quote.LineNbr;
												}
												else if (e.IsUseMarkup == true)
												{
													line.ManualPrice = true;
													decimal profit = (1m + e.MarkupPct.GetValueOrDefault() / 100m);
													if (sograph.Document.Current.CuryID == item.CuryID)
														line.CuryUnitPrice = e.CuryEstUnitCost * profit;
													else
													{
														line.UnitPrice = e.EstUnitCost * profit;
														PXCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(
															sograph.Transactions.Cache,
															line);
													}
												}

												line.POCreate = true;
												line.POSource = item.POType == POOrderType.DropShip ? INReplenishmentSource.DropShipToOrder : INReplenishmentSource.PurchaseToOrder;
												line.VendorID = poline.VendorID;

												line = PXCache<SOLine>.CreateCopy(sograph.Transactions.Update(line));
												foreach (POLine3 supply in sograph.POSupply())
													{
													if (supply.OrderNbr == poline.OrderNbr)
													{
														supply.Selected = true;
														sograph.posupply.Update(supply);
													}														
												}
												
												sograph.LinkSupplyDemand();

												if (sograph.IsDirty)
													sograph.Save.Press();

												poline.OrderQty -= qty;
												e.OrderQty -= qty;
											}
										}
									}
								}
								if (sograph.IsDirty)
									sograph.Save.Press();

								//Close all quotation
								foreach (RQRequisitionOrder o in
									PXSelect<RQRequisitionOrder,
									Where<RQRequisitionOrder.reqNbr, Equal<Required<RQRequisitionOrder.reqNbr>>,
										And<RQRequisitionOrder.orderCategory, Equal<RQOrderCategory.so>>>>.Select(this, item.ReqNbr))
								{
									sograph.Document.Current = sograph.Document.Search<SOOrder.orderNbr>(o.OrderNbr, o.OrderType);
									if (sograph.Document.Current != null && sograph.Document.Current.OrderType == "QT")
									{
										SOOrder upd = PXCache<SOOrder>.CreateCopy(sograph.Document.Current);
										upd.Status = SOOrderStatus.Completed;
										upd.Completed = true;
										sograph.Document.Update(upd);
										sograph.Save.Press();
									}
								}
							}
						}

						if (this.IsDirty) this.Save.Press();
					}
					catch
					{
						this.Clear();
						throw;
					}
					scope.Complete();
					yield return item;
				}
			}
		}



		private IEnumerable GetSOSource(RQRequisition item)
		{
			if (item.CustomerLocationID != null)
			{
				foreach (RQRequisitionLine l in this.Lines.Select(item.ReqNbr))
				{
					RQSOSource source = new RQSOSource();
					source.LineNbr = l.LineNbr.GetValueOrDefault();
					source.CustomerID = item.CustomerID;
					source.CustomerLocationID = item.CustomerLocationID;
					source.InventoryID = l.InventoryID;
					source.UOM = l.UOM;
					source.SubItemID = l.SubItemID;
					source.OrderQty = l.OrderQty;
					source.IsUseMarkup = l.IsUseMarkup;
					source.MarkupPct = l.MarkupPct;
					source.EstUnitCost = l.EstUnitCost;
					source.CuryEstUnitCost = l.CuryEstUnitCost;
					yield return source;
				}
			}
			else
			{
				foreach (PXResult<RQRequisitionContent, RQRequestLine, RQRequest, RQRequestClass, RQRequisitionLine> e in
					PXSelectJoin<RQRequisitionContent,
										InnerJoin<RQRequestLine, On<RQRequestLine.orderNbr, Equal<RQRequisitionContent.orderNbr>,
											And<RQRequestLine.lineNbr, Equal<RQRequisitionContent.lineNbr>>>,
										InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequisitionContent.orderNbr>>,
										InnerJoin<RQRequestClass, On<RQRequestClass.reqClassID, Equal<RQRequest.reqClassID>>,
										InnerJoin<RQRequisitionLine, On<RQRequisitionLine.reqNbr, Equal<RQRequisitionContent.reqNbr>,
											And<RQRequisitionLine.lineNbr, Equal<RQRequisitionContent.reqLineNbr>>>>>>>,
										Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
											And<RQRequestClass.customerRequest, Equal<CS.boolTrue>>>,
										OrderBy<
											Asc<RQRequest.employeeID,
											Asc<RQRequisitionContent.reqLineNbr>>>>
										.Select(this, item.ReqNbr))
				{
					RQRequest r = e;
					RQRequestLine l = e;
					RQRequisitionContent c = e;
					RQRequisitionLine i = e;
					RQSOSource source = new RQSOSource();
					source.UOM = l.UOM;
					source.CustomerID = r.EmployeeID;
					source.CustomerLocationID = r.LocationID;
					source.LineNbr = c.ReqLineNbr.GetValueOrDefault();
					source.InventoryID = l.InventoryID;
					source.SubItemID = l.SubItemID;
					source.OrderQty = c.ReqQty;
					source.IsUseMarkup = i.IsUseMarkup;
					source.MarkupPct = i.MarkupPct;
					source.EstUnitCost = l.EstUnitCost;
					source.CuryEstUnitCost = l.CuryEstUnitCost;
					yield return source;
				}
			}
		}
		public PXAction<RQRequisition> createQTOrder;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.CreateQuotation)]
		public virtual IEnumerable CreateQTOrder(PXAdapter adapter)
		{
			SOOrderEntry sograph = PXGraph.CreateInstance<SOOrderEntry>();
			List<SOOrder> list = new List<SOOrder>();
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				RQRequisition result = item;
				RQRequisitionOrder req =
				PXSelectJoin<RQRequisitionOrder,
					InnerJoin<SOOrder,
								 On<SOOrder.orderNbr, Equal<RQRequisitionOrder.orderNbr>,
								And<SOOrder.status, Equal<SOOrderStatus.open>>>>,
					Where<RQRequisitionOrder.reqNbr, Equal<Required<RQRequisitionOrder.reqNbr>>,
						And<RQRequisitionOrder.orderCategory, Equal<RQOrderCategory.so>>>>
						.Select(this, item.ReqNbr);

				if (item.CustomerID != null && req == null)
				{
					this.Document.Current = item;

					bool validateResult = true;
					foreach (RQRequisitionLine line in this.Lines.Select(item.ReqNbr))
					{
						if (!ValidateOpenState(line, PXErrorLevel.Error))
							validateResult = false;
					}
					if (!validateResult)
						throw new PXRowPersistingException(typeof(RQRequisition).Name, item, Messages.UnableToCreateOrders);

					sograph.TimeStamp = this.TimeStamp;
					sograph.Document.Current = null;
					foreach (PXResult<RQRequisitionLine, InventoryItem> r in
						PXSelectJoin<RQRequisitionLine,
						LeftJoin<InventoryItem,
									On<InventoryItem.inventoryID, Equal<RQRequisitionLine.inventoryID>>>,
						Where<RQRequisitionLine.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>.Select(this, item.ReqNbr))
					{
						RQRequisitionLine l = r;
						InventoryItem i = r;
						RQBidding bidding =
							item.VendorID == null ?
							PXSelect<RQBidding,
							Where<RQBidding.reqNbr, Equal<Current<RQRequisitionLine.reqNbr>>,
								And<RQBidding.lineNbr, Equal<Current<RQRequisitionLine.lineNbr>>,
								And<RQBidding.orderQty, Greater<decimal0>>>>,
								OrderBy<Desc<RQBidding.quoteUnitCost>>>
								.SelectSingleBound(this, new object[] { l }) :
							PXSelect<RQBidding,
							Where<RQBidding.reqNbr, Equal<Current<RQRequisitionLine.reqNbr>>,
								And<RQBidding.lineNbr, Equal<Current<RQRequisitionLine.lineNbr>>,
								And<RQBidding.vendorID, Equal<Current<RQRequisition.vendorID>>,
								And<RQBidding.vendorLocationID, Equal<Current<RQRequisition.vendorLocationID>>>>>>>
								.SelectSingleBound(this, new object[] { l, item });

						if (sograph.Document.Current == null)
						{
							SOOrder order = (SOOrder)sograph.Document.Cache.CreateInstance();
							order.OrderType = "QT";
							order = sograph.Document.Insert(order);
							order = PXCache<SOOrder>.CreateCopy(sograph.Document.Search<SOOrder.orderNbr>(order.OrderNbr));
							order.CustomerID = item.CustomerID;
							order.CustomerLocationID = item.CustomerLocationID;
							order = PXCache<SOOrder>.CreateCopy(sograph.Document.Update(order));
							order.CuryID = item.CuryID;
							order.CuryInfoID = CopyCurrenfyInfo(sograph, item.CuryInfoID);
							sograph.Document.Update(order);
							sograph.Save.Press();
							order = sograph.Document.Current;
							list.Add(order);

							RQRequisitionOrder link = new RQRequisitionOrder();
							link.OrderCategory = RQOrderCategory.SO;
							link.OrderType = order.OrderType;
							link.OrderNbr = order.OrderNbr;
							this.ReqOrders.Insert(link);							
						}
						SOLine line = (SOLine)sograph.Transactions.Cache.CreateInstance();
						line.OrderType = sograph.Document.Current.OrderType;
						line.OrderNbr = sograph.Document.Current.OrderNbr;
						line = PXCache<SOLine>.CreateCopy(sograph.Transactions.Insert(line));
						line.InventoryID = l.InventoryID;
						if(line.InventoryID != null)
							line.SubItemID = l.SubItemID;
						line.UOM = l.UOM;
						line.Qty = l.OrderQty;
						if (l.SiteID != null)
							line.SiteID = l.SiteID;

						if (l.IsUseMarkup == true)
						{
							string curyID = item.CuryID;
							decimal profit = (1m + l.MarkupPct.GetValueOrDefault() / 100m);
							line.ManualPrice = true;
							decimal unitPrice = l.EstUnitCost.GetValueOrDefault();
							decimal curyUnitPrice = l.CuryEstUnitCost.GetValueOrDefault();

							if (bidding != null && bidding.MinQty <= line.OrderQty && bidding.OrderQty >= line.OrderQty)
							{
								curyID = (string)Bidding.GetValueExt<RQBidding.curyID>(bidding);
								unitPrice = bidding.QuoteUnitCost.GetValueOrDefault();
								curyUnitPrice = bidding.CuryQuoteUnitCost.GetValueOrDefault();
							}

							if (curyID == sograph.Document.Current.CuryID)
								line.CuryUnitPrice = curyUnitPrice * profit;
							else
							{
								line.UnitPrice = unitPrice * profit;
								PXCurrencyAttribute.CuryConvCury<SOLine.curyUnitPrice>(
									sograph.Transactions.Cache,
									line);
							}
						}

						line = PXCache<SOLine>.CreateCopy(sograph.Transactions.Update(line));
						RQRequisitionLine upd = PXCache<RQRequisitionLine>.CreateCopy(l);
						l.QTOrderNbr = line.OrderNbr;
						l.QTLineNbr = line.LineNbr;
						this.Lines.Update(l);
					}
					using (PXTransactionScope scope = new PXTransactionScope())
					{
						try
						{
							if (sograph.IsDirty) sograph.Save.Press();
							RQRequisition upd = PXCache<RQRequisition>.CreateCopy(item);
							upd.Quoted = true;
							result = this.Document.Update(upd);
							this.Save.Press();
						}
						catch
						{
							this.Clear();
							throw;
						}
						scope.Complete();
					}
				}
				else
				{
					RQRequisition upd = PXCache<RQRequisition>.CreateCopy(item);
					upd.Quoted = true;
					result = this.Document.Update(upd);
					this.Save.Press();
				}
				yield return result;
			}
			if(list.Count == 1 && adapter.MassProcess == true)
			{
				sograph.Clear();
				sograph.SelectTimeStamp();
				sograph.Document.Current = list[0];
				throw new PXRedirectRequiredException(sograph, SO.Messages.SOOrder);
			}
		}

		public PXAction<RQRequisition> ViewLineDetails;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		[PXUIField(DisplayName = Messages.PurchaseDetails)]
		public virtual IEnumerable viewLineDetails(PXAdapter adapter)
		{
			if (Document.Current != null &&
					Document.Current.Released == true &&
					Lines.Current != null)
			{
				this.Filter.Current.ReqNbr = Lines.Current.ReqNbr;
				this.Filter.Current.LineNbr = Lines.Current.LineNbr;
				OrderLines.AskExt();
				OrderLines.ClearDialog();
			}
			return adapter.Get();
		}

		public PXAction<RQRequisition> ChooseVendor;
		[PXButton]
		[PXUIField(DisplayName = Messages.ChooseVendor)]
		public virtual IEnumerable chooseVendor(PXAdapter adapter)
		{
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				if (this.Vendors.Current != null &&
					(item.Status == RQRequisitionStatus.Bidding || item.Status == RQRequisitionStatus.Hold))
				{
					foreach (RQBidding bid in PXSelect<RQBidding,
					Where<RQBidding.reqNbr, Equal<Required<RQBidding.reqNbr>>,
						And<RQBidding.orderQty, Greater<Required<RQBidding.orderQty>>>>>.Select(this, item.ReqNbr, 0m))
					{
						RQBidding b = (RQBidding)this.Bidding.Cache.CreateCopy(bid);
						b.OrderQty = 0;
						this.Bidding.Update(b);
					}

					RQRequisition upd = (RQRequisition)this.Document.Cache.CreateCopy(item);
					upd.VendorID = this.Vendors.Current.VendorID;
					upd.VendorLocationID = this.Vendors.Current.VendorLocationID;
					upd.RemitAddressID = this.Vendors.Current.RemitAddressID;
					upd.RemitContactID = this.Vendors.Current.RemitContactID;
					upd = this.Document.Update(upd);
					upd = (RQRequisition)Document.Search<RQRequisition.reqNbr>(upd.ReqNbr);
					yield return upd;
				}
				else
					yield return item;
			}
		}
		public PXAction<RQRequisition> ResponseVendor;
		[PXButton]
		[PXUIField(DisplayName = Messages.VendorResponse)]
		public virtual IEnumerable responseVendor(PXAdapter adapter)
		{
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				if (this.Vendors.Current != null)
				{
					RQBiddingEntry graph = PXGraph.CreateInstance<RQBiddingEntry>();
					graph.Vendor.Current =
						graph.Vendor.Search<RQBiddingVendor.reqNbr,
							RQBiddingVendor.lineID>(this.Vendors.Current.ReqNbr, this.Vendors.Current.LineID);
					if (graph.Vendor.Current != null)
						throw new PXRedirectRequiredException(graph, "Vendor Response");
				}
				yield return item;
			}
		}


		public PXAction<RQRequisition> VendorInfo;
		[PXButton]
		[PXUIField(DisplayName = Messages.VendorInfo)]
		public virtual IEnumerable vendorInfo(PXAdapter adapter)
		{
			foreach (RQRequisition item in adapter.Get<RQRequisition>())
			{
				if (this.Vendors.Current != null)
					this.Vendors.AskExt();
				yield return item;
			}
		}

		private POLine InsertPOLine(POOrderEntry graph, RQRequisitionLine line, Decimal? qty, Decimal? unitCost, string lineType)
		{
			if (qty <= 0) return null;
			POLine ooline = (POLine)graph.Transactions.Cache.CreateInstance();
			ooline.OrderType = graph.Document.Current.OrderType;
			ooline.OrderNbr = graph.Document.Current.OrderNbr;
			ooline = PXCache<POLine>.CreateCopy(graph.Transactions.Insert(ooline));
			ooline.LineType = lineType;			
			ooline.InventoryID = line.InventoryID;
			if (ooline.InventoryID != null)
				ooline.SubItemID = line.SubItemID;
			ooline.TranDesc = line.Description;
			ooline.UOM = line.UOM;
			ooline.AlternateID = line.AlternateID;
			ooline = graph.Transactions.Update(ooline);
			if (line.SiteID != null)
				graph.Transactions.Cache.RaiseExceptionHandling<POLine.siteID>(ooline, null, null);

			ooline = PXCache<POLine>.CreateCopy(ooline);

			if (line.SiteID != null)
			{
				graph.Transactions.Cache.RaiseExceptionHandling<POLine.siteID>(ooline, null, null);
				ooline.SiteID = line.SiteID;
			}

			ooline.OrderQty = qty;
			if (unitCost != null)
				ooline.CuryUnitCost = unitCost;

			ooline.RcptQtyAction = line.RcptQtyAction;
			ooline.RcptQtyMin = line.RcptQtyMin;
			ooline.RcptQtyMax = line.RcptQtyMax;
			ooline.RcptQtyThreshold = line.RcptQtyThreshold;
			ooline.RQReqNbr = line.ReqNbr;
			ooline.RQReqLineNbr = line.LineNbr.GetValueOrDefault();
			if (lineType != POLineType.GoodsForInventory)
			{
				if (line.ExpenseAcctID != null)
					ooline.ExpenseAcctID = line.ExpenseAcctID;
				if (line.ExpenseAcctID != null && line.ExpenseSubID != null)
					ooline.ExpenseSubID = line.ExpenseSubID;				
				ooline.ProjectID = PM.ProjectDefaultAttribute.NonProject(graph);
			}
			if(line.PromisedDate != null)
				ooline.PromisedDate = line.PromisedDate;
			if(line.RequestedDate != null)
				ooline.RequestedDate = line.RequestedDate;

			PXNoteAttribute.CopyNoteAndFiles(Lines.Cache, line, graph.Transactions.Cache, ooline);

			ooline = graph.Transactions.Update(ooline);
			PXUIFieldAttribute.SetError<POLine.subItemID>(graph.Transactions.Cache, ooline, null);
			PXUIFieldAttribute.SetError<POLine.expenseSubID>(graph.Transactions.Cache, ooline, null);
			return ooline;
		}
		private void PersistOrder(POOrderEntry graph)
		{
			POOrder upd = (POOrder)graph.Document.Cache.CreateCopy(graph.Document.Current);
			if(Setup.Current.POHold != true)
				upd.Hold = false;			
			upd.CuryControlTotal = upd.CuryOrderTotal;
			graph.Document.Update(upd);
			PXAdapter adapter = new PXAdapter(graph.Document);
			adapter.StartRow = 0;
			adapter.MaximumRows = 1;
			adapter.Searches = new object[] { upd.OrderType, upd.OrderNbr };
			adapter.SortColumns = new string[] { typeof(POOrder.orderType).Name, typeof(POOrder.orderNbr).Name };
			foreach (object item in graph.hold.Press(adapter)) ;
			
			graph.Save.Press();

			RQRequisitionOrder link = new RQRequisitionOrder();
			link.OrderCategory = RQOrderCategory.PO;
			link.OrderType = graph.Document.Current.OrderType;
			link.OrderNbr = graph.Document.Current.OrderNbr;
			this.ReqOrders.Insert(link);
		}


		public PXAction<RQRequisition> hold;
		[PXUIField(Visible = false)]
		[PXButton]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			foreach (RQRequisition order in adapter.Get<RQRequisition>())
			{
				this.Document.Current = order;
				if (order.Hold == true)
				{
					yield return order;
				}
				else
				{
					if (order.Hold != true && order.Approved != true)
					{
						if (Setup.Current.RequisitionAssignmentMapID != null)
						{
							var helper = new EPAssignmentProcessHelper<RQRequisition>();
							helper.Assign(order, Setup.Current.RequisitionAssignmentMapID);
                            order.WorkgroupID = order.ApprovalWorkgroupID;
                            order.OwnerID = order.ApprovalOwnerID;
						}
					}
					yield return (RQRequisition)Document.Search<RQRequisition.reqNbr>(order.ReqNbr);
				}

			}
		}

        public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
        {
            if (viewName.ToLower() == "document" && values != null)
            {
                if (IsExport || IsMobile || IsContractBasedAPI)
                {
                    Document.Cache.Locate(keys);
                    if (values.Contains("Hold") && values["Hold"] != PXCache.NotSetValue && values["Hold"] != null)
                    {
                        var hold = Document.Current.Hold ?? false;
                        if (Convert.ToBoolean(values["Hold"]) != hold)
                        {
                            ((PXAction<RQRequisition>)this.Actions["Hold"]).PressImpl(false);
                        }
                    }
                }
            }
            return base.ExecuteUpdate(viewName, keys, values, parameters);
        }

                public PXAction<RQRequisition> action;
		[PXUIField(DisplayName = "Actions")]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
		[PXInt]
		[PXIntList(new int[] { 1, 2 }, new string[] { "Persist", "Update" })]
		int? actionID,
		[PXBool]
		bool refresh,
		[PXString]
		string actionName
		)
		{
			List<RQRequisition> result = new List<RQRequisition>();
			if (actionName != null)
			{
				PXAction a = this.Actions[actionName];
				if (a != null)
					result.AddRange(a.Press(adapter).Cast<RQRequisition>());
			}
			else
				result.AddRange(adapter.Get<RQRequisition>());

			if (refresh)
			{
				foreach (RQRequisition order in result)									
					Document.Search<RQRequisition.reqNbr>(order.ReqNbr);				
			}
			switch (actionID)
			{
				case 1:
					Save.Press();
					break;
				case 2:
					break;
			}
			return result;
		}

		public PXAction<RQRequisition> vendorNotifications;
		[PXUIField(DisplayName = "Vendor Notifications"/*, Visible = false*/)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable VendorNotifications(PXAdapter adapter,
			[PXString] string notificationCD,
			[PXBool] bool currentVendor,
			[PXBool] bool updateVendorStatus
			)
		{
			foreach (RQRequisition order in adapter.Get<RQRequisition>())
			{
				Document.Cache.Current = order;

				PXResultset<RQBiddingVendor> source = null;
				if (currentVendor)
				{
					source = new PXResultset<RQBiddingVendor>();
					source.Add(new PXResult<RQBiddingVendor>(this.Vendors.Current));
				}
				bool error = false;
				foreach (RQBiddingVendor vndr in source ?? Vendors.Select(order.ReqNbr))
				{
					this.Vendors.Current = vndr;
						var parameters = new Dictionary<string, string>();										
						if (currentVendor == true || vndr.Status != true)
						{
							bool updateStatus = updateVendorStatus; 
							try
							{
								string vendorCD = (string)this.Vendors.GetValueExt<RQBiddingVendor.vendorID>(vndr);
								parameters["ReqNbr"] = Document.Current.ReqNbr;
								parameters["VendorID"] = vendorCD;
								BidActivity.SendNotification(APNotificationSource.Vendor, notificationCD, order.BranchID, parameters);
							}
							catch (Exception e)
							{
								if(currentVendor) throw;								
								PXTrace.WriteError(e);
								error = true;
								updateStatus = false;
								Vendors.Cache.RaiseExceptionHandling<RQBiddingVendor.status>(vndr, false, e);
							}

							if (updateStatus)
							{
								RQBiddingVendor upd = PXCache<RQBiddingVendor>.CreateCopy(vndr);
								upd.Status = true;
								this.Vendors.Update(upd);
								Save.Press();
							}
						}
					}
				if (error) throw new PXException(ErrorMessages.SeveralItemsFailed);
				yield return order;
			}			
		}


		public PXAction<RQRequisition> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Report(PXAdapter adapter,
			[PXString(8, InputMask = "CC.CC.CC.CC")]
			string reportID
			)
		{
            List<RQRequisition> list = adapter.Get<RQRequisition>().ToList();
			if (!String.IsNullOrEmpty(reportID))
			{
				Save.Press();				
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["ReqNbr"] = Document.Current.ReqNbr;
				throw new PXReportRequiredException(parameters, reportID, "Report " + reportID);				
			}
			return list;
		}
		

		#region CurrencyInfo events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo row = (CurrencyInfo)e.Row;
				RQRequisition doc = (RQRequisition)this.Document.Current;
				if (row != null && doc != null && row.CuryInfoID == doc.CuryInfoID)
				{
					if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryID))
					{
						e.NewValue = customer.Current.CuryID;
						e.Cancel = true;
					}
					else if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
					{
						e.NewValue = vendor.Current.CuryID;
						e.Cancel = true;
					}
				}				
				else if (vendorBidder.Current != null && !string.IsNullOrEmpty(vendorBidder.Current.CuryID))
				{
					e.NewValue = vendorBidder.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo row = (CurrencyInfo)e.Row;
				RQRequisition doc = (RQRequisition)this.Document.Current;
				if (row != null && doc != null && row.CuryInfoID == doc.CuryInfoID)
				{
					if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryID))
					{
						e.NewValue = customer.Current.CuryRateTypeID ?? cmsetup.Current.ARRateTypeDflt;
						e.Cancel = true;
					}
					else if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
					{
						e.NewValue = vendor.Current.CuryRateTypeID ?? cmsetup.Current.APRateTypeDflt;
						e.Cancel = true;
					}
				}			
				else if (vendorBidder.Current != null)
				{
					e.NewValue = vendorBidder.Current.CuryRateTypeID ?? cmsetup.Current.APRateTypeDflt;
					e.Cancel = true;
				}
				else
				{
					e.NewValue = cmsetup.Current.APRateTypeDflt;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((RQRequisition)Document.Cache.Current).OrderDate;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info != null)
			{
				bool curyenabled = info.IsReadOnly != true && this.Lines.Cache.AllowInsert && this.Lines.Cache.AllowDelete;
				bool curyrateenabled = info.AllowUpdate(this.Lines.Cache);
				
				RQRequisition doc = (RQRequisition)this.Document.Current;
				if (doc != null && info.CuryInfoID == doc.CuryInfoID)
				{
					if (customer.Current != null)
					{
						if (customer.Current.AllowOverrideCury != true)
							curyenabled = false;

						if (customer.Current.AllowOverrideRate != true)
							curyrateenabled = false;
					}
					else if (vendor.Current != null)
					{
						if (vendor.Current.AllowOverrideCury != true)
							curyenabled = false;

						if (vendor.Current.AllowOverrideRate != true)
							curyrateenabled = false;
					}
				}
				else if (doc != null)
				{
					PXResult<RQBiddingVendor, Vendor> v =
						(PXResult<RQBiddingVendor, Vendor>)
						PXSelectJoin<RQBiddingVendor,
						InnerJoin<Vendor, On<RQBiddingVendor.vendorID, Equal<Vendor.bAccountID>>>,
						Where<RQBiddingVendor.curyInfoID, Equal<Required<RQBiddingVendor.curyInfoID>>>>
						.SelectWindowed(this, 0, 1, info.CuryInfoID);

					Vendor vendor = v != null ? (Vendor)v : null;

					curyenabled = (vendor != null) ? vendor.AllowOverrideCury == true : false;
					curyrateenabled = (vendor != null) ? vendor.AllowOverrideRate == true : false;
				}

				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyID>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyrateenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyrateenabled);
			}
		}
		#endregion

		protected virtual void RQRequisition_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			if (row == null) return;
			bool rHold = (row.Hold == true);
			Transfer.SetEnabled(rHold);
			Merge.SetEnabled(rHold);
			ViewLineDetails.SetEnabled(row.Status == RQRequisitionStatus.Released);
			AddRequestLine.SetEnabled(rHold);
			AddRequestContent.SetEnabled(rHold);
			CMSetup setup = cmsetup.Current;
			PXUIFieldAttribute.SetVisible<RQRequest.curyID>(sender, row, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			bool noLines = this.Lines.Select().Count == 0;

			bool enableCurrenty = false;
			if (noLines)
				if (customer.Current != null)
					enableCurrenty = customer.Current.AllowOverrideCury == true;
				else if (this.vendor.Current != null)
					enableCurrenty = this.vendor.Current.AllowOverrideCury == true;
				else
					enableCurrenty = true;

			PXUIFieldAttribute.SetEnabled<RQRequisition.customerID>(sender, row, noLines);
			PXUIFieldAttribute.SetEnabled<RQRequisition.customerLocationID>(sender, row, noLines);
			PXUIFieldAttribute.SetEnabled<RQRequisition.curyID>(sender, row, enableCurrenty);
			PXUIFieldAttribute.SetVisible<RQRequisition.quoted>(sender, row, row.CustomerLocationID != null);
			this.addInvBySite.SetEnabled(rHold && !(bool)row.Released);
			POShipAddress shipAddress = this.Shipping_Address.Select();
			PORemitAddress remitAddress = this.Remit_Address.Select();
			bool enableAddressValidation = (row.Released== false && row.Cancelled == false)
				&& ((shipAddress != null && shipAddress.IsDefaultAddress == false && shipAddress.IsValidated == false)
				|| (remitAddress != null && remitAddress.IsDefaultAddress == false && remitAddress.IsValidated == false));
			this.validateAddresses.SetEnabled(enableAddressValidation);
            Vendors.Cache.AllowUpdate = Vendors.Cache.AllowInsert = Vendors.Cache.AllowDelete = row.Status != RQRequisitionStatus.Released;
		}
		protected virtual void RQRequisition_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (e.Row != null)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Shipping_Address.Cache, Shipping_Contact.Cache))
				{
					POShipAddressAttribute.DefaultRecord<RQRequisition.shipAddressID>(sender, e.Row);
					POShipContactAttribute.DefaultRecord<RQRequisition.shipContactID>(sender, e.Row);
				}
			}
		}
		protected virtual void RQRequisition_ShipDestType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<RQRequisition.shipToBAccountID>(e.Row);
		}
		protected virtual void RQRequisition_ShipToBAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<RQRequisition.shipToLocationID>(e.Row);
			}
		}
		protected virtual void RQRequisition_ShipToLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			if (row != null)
			{
				POShipAddressAttribute.DefaultRecord<RQRequisition.shipAddressID>(sender, e.Row);
				POShipContactAttribute.DefaultRecord<RQRequisition.shipContactID>(sender, e.Row);
			}
		}
		protected virtual void RQRequisition_ShipToLocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			if (row != null && (row.ShipDestType == POShippingDestination.CompanyLocation)
				&& (row.VendorID != null && row.VendorLocationID != null))
			{
				Location vendorLocation = PXSelectReadonly<Location,
												Where<Location.bAccountID, Equal<Required<RQRequisition.vendorID>>,
												And<Location.locationID, Equal<Required<RQRequisition.vendorLocationID>>>>>.Select(this, row.VendorID, row.VendorLocationID);
				if (vendorLocation != null && vendorLocation.VBranchID != null)
				{
					e.NewValue = vendorLocation.VBranchID;
					e.Cancel = true;
				}
			}
		}
		protected virtual void RQRequisition_BiddingComplete_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			e.NewValue = row.VendorLocationID != null;
		}

		protected virtual void RQRequisition_Quoted_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			e.NewValue = row.CustomerLocationID == null;
		}

		protected virtual void RQRequisition_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			this.customer.Current = null;
			sender.SetDefaultExt<RQRequisition.customerLocationID>(e.Row);
			sender.SetDefaultExt<RQRequisition.curyID>(e.Row);
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<RQRequisition.curyID>(e.Row) == null)
			{
				if (sender.GetValuePending<RQRequisition.vendorID>(e.Row) != null) return;

				CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<RQRequisition.curyInfoID>(sender, e.Row);

				string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) == false)
				{
					sender.RaiseExceptionHandling<RQRequisition.orderDate>(e.Row, ((SOOrder)e.Row).OrderDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				}

				if (info != null)
				{
					((RQRequisition)e.Row).CuryID = info.CuryID;
				}
			}
		}
		}

		protected virtual void RQRequisition_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			RQRequisition newRow = (RQRequisition)e.NewRow;

			if (row != null &&
				 (row.CustomerID != newRow.CustomerID || row.CustomerLocationID != newRow.CustomerLocationID))
			{
				RQRequisitionContent cont =
					PXSelectJoin<RQRequisitionContent,
						InnerJoin<RQRequest,
									 On<RQRequest.orderNbr, Equal<RQRequisitionContent.orderNbr>>>,
					Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
						And<Where<RQRequest.employeeID, NotEqual<Required<RQRequest.employeeID>>,
									 Or<RQRequest.locationID, NotEqual<Required<RQRequest.locationID>>>>>>>.SelectWindowed(this, 0, 1,
									 row.ReqNbr, newRow.CustomerID, newRow.CustomerLocationID);
				if (cont != null &&
					this.Contents.Ask(Messages.AskConfirmation,
					Messages.CustomerUpdateConfirmation,
					MessageButtons.YesNo) == WebDialogResult.No)
				{
					newRow.CustomerID = row.CustomerID;
					newRow.CustomerLocationID = row.CustomerLocationID;
				}
			}
		}
		protected virtual void RQRequisition_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.OldRow;
			RQRequisition newRow = (RQRequisition)e.Row;

			if (newRow.CustomerID == null && newRow.POType == POOrderType.DropShip)
				newRow.POType = POOrderType.RegularOrder;

			if (row.POType != newRow.POType &&
				newRow.POType == POOrderType.DropShip &&
				newRow.ShipDestType != POShippingDestination.Customer)
			{
				RQRequisition upd = PXCache<RQRequisition>.CreateCopy(newRow);
				upd.ShipDestType = POShippingDestination.Customer;
				upd.ShipToBAccountID = row.CustomerID;
				upd.ShipToLocationID = row.CustomerLocationID;
				this.Document.Update(upd);
			}
			else if (row.POType != newRow.POType &&
				newRow.POType != POOrderType.DropShip &&
				newRow.ShipDestType == POShippingDestination.Customer)
			{
				RQRequisition upd = PXCache<RQRequisition>.CreateCopy(newRow);
				upd.ShipDestType = POShippingDestination.CompanyLocation;
				sender.SetDefaultExt<RQRequisition.shipDestType>(upd);
				this.Document.Update(upd);
			}
			if (row.CustomerID == null &&
					row.VendorID != newRow.VendorID && PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				Vendor rVendor = vendor.Current;
				if (rVendor == null || rVendor.BAccountID != ((RQRequisition)e.Row).VendorID)
				{
					vendor.RaiseFieldUpdated(sender, e.Row);
					rVendor = vendor.Current;
				}
				if (rVendor != null)
				{
					CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy(currencyinfo.Select(row.CuryInfoID));

					RQBiddingVendor bidder =
						PXSelect<RQBiddingVendor,
							Where<RQBiddingVendor.reqNbr, Equal<Required<RQBiddingVendor.reqNbr>>,
								And<RQBiddingVendor.vendorID, Equal<Required<RQBiddingVendor.vendorID>>,
									And<RQBiddingVendor.vendorLocationID, Equal<Required<RQBiddingVendor.vendorLocationID>>>>>>
							.SelectWindowed(this, 0, 1,
															newRow.ReqNbr,
															newRow.VendorID,
															newRow.VendorLocationID);

					CurrencyInfo vendorInfo = bidder != null ? currencyinfo.Select(bidder.CuryInfoID) : null;
					bool externalCall = e.ExternalCall;
					if (vendorInfo == null)
					{
						vendorInfo = new CurrencyInfo();
						vendorInfo.CuryID = vendor.Current.CuryID ?? company.Current.BaseCuryID;
						vendorInfo.CuryRateTypeID = vendor.Current.CuryRateTypeID ?? cmsetup.Current.APRateTypeDflt;
						externalCall = true;
					}

					bool update = false;

					if (vendorInfo.CuryID != info.CuryID &&
							(vendor.Current.AllowOverrideCury != true || externalCall != true))
					{
						info.CuryID = vendorInfo.CuryID;
						update = true;
					}

					if (vendorInfo.CuryRateTypeID != info.CuryRateTypeID &&
							(vendor.Current.AllowOverrideRate != true || externalCall != true))
					{
						info.CuryRateTypeID = vendorInfo.CuryRateTypeID;
						update = true;
					}

					if (update)
					{
						currencyinfo.Update(info);

						string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
						if (string.IsNullOrEmpty(message) == false)
						{
							sender.RaiseExceptionHandling<RQRequisition.orderDate>(e.Row, ((RQRequisition)e.Row).OrderDate,
																																		 new PXSetPropertyException(message, PXErrorLevel.Warning));
						}
						row.CuryID = info.CuryID;

						foreach (RQRequisitionLine line in this.Lines.View.SelectMultiBound(new object[] { e.Row }))
						{
							RQRequisitionLine upd = (RQRequisitionLine)this.Lines.Cache.CreateCopy(line);
							Decimal unitCost;
							PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>
								(this.Lines.Cache, upd, upd.EstUnitCost ?? 0, out unitCost);
							upd.CuryEstUnitCost = unitCost;

							this.Lines.Cache.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(upd);
							this.Lines.Cache.SetDefaultExt<RQRequisitionLine.siteID>(upd);
							this.Lines.Update(upd);
						}
					}
				}
			}
			if (newRow.Hold != true)
			{
				if (Lines.Select().Any(_ => ((RQRequisitionLine)_).InventoryID == null))
					sender.RaiseExceptionHandling<RQRequisition.hold>(newRow, newRow.Hold, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<RQRequisitionLine.inventoryID>(Lines.Cache), PXErrorLevel.Error));
				else
					sender.RaiseExceptionHandling<RQRequisition.hold>(newRow, newRow.Hold, null);
			}

		}

		protected virtual void RQRequisition_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			row.Quoted = row.CustomerLocationID == null;

			if (row != null && row.Hold == true)
				foreach (RQRequisitionLine line in this.Lines.Select(row.ReqNbr))
				{
					RQRequisitionLine upd = (RQRequisitionLine)this.Lines.Cache.CreateCopy(line);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(upd);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.siteID>(upd);
					this.Lines.Update(upd);
				}
		}

		protected virtual void RQRequisition_POType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			if (row.CustomerID == null && (string)e.NewValue == POOrderType.DropShip)
				throw new PXSetPropertyException(Messages.DropShipRequisition);
		}


		protected virtual void RQRequisition_VendorID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			RQRequisition row = (RQRequisition)e.Row;
			Vendor vendor = PXSelect<Vendor, Where<Vendor.acctCD, Equal<Required<Vendor.acctCD>>>>
				.SelectWindowed(this, 0, 1, e.NewValue);
			if (vendor != null && PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo current = currencyinfo.Select(row.CuryInfoID);
				if (vendor.CuryID == null) vendor.CuryID = company.Current.BaseCuryID;
				if (vendor.CuryRateTypeID == null) vendor.CuryRateTypeID = cmsetup.Current.APRateTypeDflt;

				string message = null;
				if (vendor.AllowOverrideCury != true && vendor.CuryID != current.CuryID)
					message = PXMessages.LocalizeFormatNoPrefixNLA(Messages.RequisitionVendorCuryIDValidation, current.CuryID);
				else if (vendor.AllowOverrideRate != true && vendor.CuryRateTypeID != current.CuryRateTypeID)
					message = PXMessages.LocalizeFormatNoPrefixNLA(Messages.RequisitionVendorCuryRateIDValidation, current.CuryRateTypeID);

				if (message != null &&
					this.Document.Ask(row, Messages.Warning, message, MessageButtons.OKCancel)
					!= WebDialogResult.OK)
				{
					e.Cancel = true;
				}
			}
		}

		protected virtual void RQRequisition_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<RQRequisition.vendorLocationID>(e.Row);
			sender.SetDefaultExt<RQRequisition.termsID>(e.Row);
			object VendorRefNbr = ((RQRequisition)e.Row).VendorRefNbr;
			sender.RaiseFieldVerifying<RQRequisition.vendorRefNbr>(e.Row, ref VendorRefNbr);

		}

		protected virtual void RQRequisition_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Location current = (Location)this.location.Current;
			RQRequisition row = (RQRequisition)e.Row;
			if (current == null || (current.BAccountID != row.VendorID || current.LocationID != row.VendorLocationID))
			{
				current = this.location.Select();
				this.location.Current = current;
			}

			sender.SetDefaultExt<RQRequisition.shipVia>(e.Row);
			sender.SetDefaultExt<RQRequisition.fOBPoint>(e.Row);
			if (row.ShipDestType == POShippingDestination.Vendor)
				sender.SetDefaultExt<RQRequisition.shipToLocationID>(e.Row);
			sender.SetDefaultExt<RQRequisition.biddingComplete>(e.Row);

			PORemitAddressAttribute.DefaultRecord<RQRequisition.remitAddressID>(sender, e.Row);
			PORemitContactAttribute.DefaultRecord<RQRequisition.remitContactID>(sender, e.Row);

			foreach (RQRequisitionLine line in this.Lines.Select(row.ReqNbr))
			{
				RQRequisitionLine upd = (RQRequisitionLine)this.Lines.Cache.CreateCopy(line);
				this.Lines.Cache.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(upd);				
				try
				{
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.siteID>(upd);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.rcptQtyAction>(upd);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.rcptQtyMin>(upd);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.rcptQtyMax>(upd);
					this.Lines.Cache.SetDefaultExt<RQRequisitionLine.rcptQtyThreshold>(upd);
				}
				catch
				{					
				}
				this.Lines.Update(upd);				
			}
		}

		protected virtual void RQRequisition_VendorRequestSent_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RQRequisition doc = (RQRequisition)e.Row;
			if (doc.VendorRequestSent == true)
			{
				foreach (RQBiddingVendor bidder in this.Vendors.Select(doc.ReqNbr))
				{
					RQBiddingVendor upd = PXCache<RQBiddingVendor>.CreateCopy(bidder);
					upd.Status = true;
					this.Vendors.Update(upd);
				}
			}
		}
		protected virtual void RQRequisition_Hold_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			RQRequisition doc = (RQRequisition)e.Row;
			if (doc.Hold == true && doc.Hold != (bool?)e.OldValue)
			{
				cache.SetDefaultExt<RQRequisition.biddingComplete>(e.Row);
				cache.SetDefaultExt<RQRequisition.quoted>(e.Row);

				if (doc.Cancelled == true)
					cache.SetValueExt<RQRequisition.cancelled>(doc, false);

				foreach (RQBiddingVendor bidder in this.Vendors.Select(doc.ReqNbr))
				{
					RQBiddingVendor upd = PXCache<RQBiddingVendor>.CreateCopy(bidder);
					upd.Status = false;
					this.Vendors.Update(upd);
				}
			}
		}

		protected virtual void RQRequisitionLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<RQRequisitionLine.uOM>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.description>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.subItemID>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.estUnitCost>(e.Row);
			sender.SetValue<RQRequisitionLine.curyEstUnitCost>(e.Row, null);
			sender.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.promisedDate>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.markupPct>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.rcptQtyAction>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.rcptQtyMax>(e.Row);
			sender.SetDefaultExt<RQRequisitionLine.rcptQtyMin>(e.Row);			
		}

		protected virtual void RQRequisitionLine_SiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			RQRequisitionLine line = (RQRequisitionLine)e.Row;

            if (line == null) return;

			if (this.Document.Current != null && this.Document.Current.CustomerLocationID != null)
				return;

			int? siteID = null;
			foreach (RQRequisitionContent rq in
				PXSelect<RQRequisitionContent,
				Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
					And<RQRequisitionContent.reqLineNbr, Equal<Required<RQRequisitionContent.reqLineNbr>>>>>
					.Select(this, line.ReqNbr, line.LineNbr))
			{
				PXResult<RQRequest, RQRequestClass, Location> res =
					(PXResult<RQRequest, RQRequestClass, Location>)
					PXSelectJoin<RQRequest,
					InnerJoin<RQRequestClass, On<RQRequestClass.reqClassID, Equal<RQRequest.reqClassID>>,
					InnerJoin<Location, On<Location.bAccountID, Equal<RQRequest.employeeID>, And<Location.locationID, Equal<RQRequest.locationID>>>>>,
					Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.SelectWindowed(this, 0, 1, rq.OrderNbr);

				RQRequestClass reqClass = res;
				Location location = res;

				if (location == null || location.LocationCD == null) continue;
				int? locationSiteID = reqClass.CustomerRequest == true ?
					location.CSiteID :
					location.CMPSiteID;

				if (siteID == null)
				{
					siteID = locationSiteID;
				}
				else if (siteID != locationSiteID)
				{
					siteID = null;
					break;
				}
			}
			if (siteID != null)
			{
				e.NewValue = siteID;
				e.Cancel = true;
			}
		}
		protected virtual void RQRequisitionLine_Availability_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			RQRequisitionLine row = (RQRequisitionLine)e.Row;
			if (row == null)
			{
				e.ReturnValue = string.Empty;
				return;
			}

			INSiteStatus availability = PXSelect<INSiteStatus,
				Where<INSiteStatus.inventoryID, Equal<Required<RQRequisitionLine.inventoryID>>,
				And<INSiteStatus.subItemID, Equal<Required<RQRequisitionLine.subItemID>>,
				And<INSiteStatus.siteID, Equal<Required<RQRequisitionLine.siteID>>>>>>.SelectWindowed(this, 0, 1, row.InventoryID, row.SubItemID, row.SiteID);

			if (availability != null)
			{
				availability.QtyOnHand = INUnitAttribute.ConvertFromBase<RQRequisitionLine.inventoryID, RQRequisitionLine.uOM>(sender, e.Row, (decimal)availability.QtyOnHand, INPrecision.QUANTITY);
				availability.QtyAvail = INUnitAttribute.ConvertFromBase<RQRequisitionLine.inventoryID, RQRequisitionLine.uOM>(sender, e.Row, (decimal)availability.QtyAvail, INPrecision.QUANTITY);
				availability.QtyNotAvail = INUnitAttribute.ConvertFromBase<RQRequisitionLine.inventoryID, RQRequisitionLine.uOM>(sender, e.Row, (decimal)availability.QtyNotAvail, INPrecision.QUANTITY);
				availability.QtyHardAvail = INUnitAttribute.ConvertFromBase<RQRequisitionLine.inventoryID, RQRequisitionLine.uOM>(sender, e.Row, (decimal)availability.QtyHardAvail, INPrecision.QUANTITY);

				e.ReturnValue = PXMessages.LocalizeFormatNoPrefix(IN.Messages.Availability_Info,
						sender.GetValue<RQRequisitionLine.uOM>(e.Row), FormatQty(availability.QtyOnHand), FormatQty(availability.QtyAvail), FormatQty(availability.QtyHardAvail));

			}
			else
			{
				e.ReturnValue = string.Empty;
			}
		}

		protected virtual string FormatQty(decimal? value)
		{
			return (value == null) ? string.Empty : ((decimal)value).ToString("N" + CommonSetupDecPl.Qty.ToString(), System.Globalization.NumberFormatInfo.CurrentInfo);
		}

		protected virtual void RQRequisitionLine_SubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (this.Document.Current.Hold == true)
			{
				sender.SetValue<RQRequisitionLine.curyEstUnitCost>(e.Row, null);
				sender.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(e.Row);
			}
		}
		protected virtual void RQRequisitionLine_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(e.Row);
		}
		protected virtual void RQRequisitionLine_CuryEstUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			RQRequisitionLine row = e.Row as RQRequisitionLine;
			if (row != null)
			{
				e.NewValue = row.CuryEstUnitCost;
				RQRequisition order = this.Document.Current;
				RQBidding bidding = PXSelect<RQBidding,
					Where<RQBidding.reqNbr, Equal<Current<RQRequisitionLine.reqNbr>>,
						And<RQBidding.lineNbr, Equal<Current<RQRequisitionLine.lineNbr>>,
						And<RQBidding.vendorID, Equal<Current<RQRequisition.vendorID>>,
						And<RQBidding.vendorLocationID, Equal<Current<RQRequisition.vendorLocationID>>>>>>>
					.SelectSingleBound(this, new object[] { row, order });

				if (bidding != null && bidding.MinQty > row.OrderQty && row.OrderQty < bidding.QuoteQty)
				{
					if ((string)Bidding.GetValueExt<RQBidding.curyID>(bidding) == order.CuryID)
						e.NewValue = bidding.CuryQuoteUnitCost;
					else
					{
						Decimal unitCost;
						PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>
							(this.Lines.Cache, row, bidding.QuoteUnitCost ?? 0, out unitCost);
						e.NewValue = unitCost;
					}
				}
				else if (order != null && row.InventoryID != null && order.Hold == true)
				{
					Decimal? newPrice = POItemCostManager.Fetch<RQRequisitionLine.inventoryID, RQRequisitionLine.curyInfoID>(sender.Graph, row,
						order.VendorID, order.VendorLocationID, order.OrderDate, order.CuryID,
						row.InventoryID, row.SubItemID, row.SiteID, row.UOM, e.NewValue != null);
					if (newPrice > 0)
						e.NewValue = newPrice;
				}
			}
		}
		protected virtual void RQRequisitionLine_Cancelled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RQRequisitionLine row = (RQRequisitionLine)e.Row;
			row.OrderQty = 0;
		}

		protected virtual void RQRequisitionLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetValue<RQRequisitionLine.curyEstUnitCost>(e.Row, null);
			sender.SetDefaultExt<RQRequisitionLine.curyEstUnitCost>(e.Row);
		}

		protected virtual void RQRequisitionLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			RQRequisitionLine row = (RQRequisitionLine)e.Row;
			RQRequisitionLine oldRow = (RQRequisitionLine)e.OldRow;
			if (this.Document.Current != null && this.Document.Current.Hold == true && !(row.Cancelled == true))
				row.OriginQty = row.OrderQty;

			if (row != null && row.ByRequest == true && row.UOM != oldRow.UOM)
			{
				foreach (RQRequisitionContent content in PXSelect<RQRequisitionContent,
						Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>,
							And<RQRequisitionContent.reqLineNbr, Equal<Required<RQRequisitionLine.lineNbr>>>>>.Select(this, row.ReqNbr, row.LineNbr))
				{
					RQRequisitionContent upd = PXCache<RQRequisitionContent>.CreateCopy(content);
					upd.RecalcOnly = true;
					this.Contents.Update(upd);
				}
			}
		}
		protected virtual void RQRequisitionLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RQRequisitionLine row = (RQRequisitionLine)e.Row;
			if (row == null) return;
			PXDefaultAttribute.SetPersistingCheck<RQRequisitionLine.inventoryID>(sender, row, Document.Current.Hold == true ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
			if (row.InventoryID == null)
			{
				if(Document.Current.Hold != true)
					sender.RaiseExceptionHandling<RQRequisitionLine.inventoryID>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<RQRequisitionLine.inventoryID>(sender), PXErrorLevel.Error));
				else
					sender.RaiseExceptionHandling<RQRequisitionLine.inventoryID>(row, null, null);
			}

			//PXUIFieldAttribute.SetEnabled<RQRequisitionLine.uOM>(sender, row, !(row.ByRequest == true));
			PXUIFieldAttribute.SetEnabled<RQRequisitionLine.orderQty>(sender, row, !(row.ByRequest == true));
			PXUIFieldAttribute.SetEnabled<RQRequestLine.subItemID>(sender, row, row.InventoryID != null &&  row.LineType == POLineType.GoodsForInventory);

			PXUIFieldAttribute.SetEnabled<RQRequisitionLine.siteID>(sender, row, true);
			PXUIFieldAttribute.SetEnabled<RQRequisitionLine.markupPct>(sender, row, row.IsUseMarkup == true);

			if (this.Document.Current.Status == RQRequisitionStatus.PendingApproval ||
				this.Document.Current.Status == RQRequisitionStatus.Open ||
				this.Document.Current.Status == RQRequisitionStatus.PendingQuotation)
			{
				ValidateOpenState(row, PXErrorLevel.Warning);
			}
			PXUIFieldAttribute.SetEnabled<RQRequisitionLine.alternateID>(sender, row,
				this.Document.Current.VendorLocationID != null);
		}


		private bool ValidateOpenState(RQRequisitionLine row, PXErrorLevel level)
		{
			bool result = true;
			Type[] requestOnOpen =
				row.LineType == POLineType.GoodsForInventory && row.InventoryID != null
					? new Type[] {typeof (RQRequisitionLine.uOM), typeof (RQRequisitionLine.siteID), typeof (RQRequisitionLine.subItemID)}
					: row.LineType == POLineType.NonStock
						  ? new Type[] {typeof (RQRequisitionLine.uOM), typeof (RQRequisitionLine.siteID),}
						  : new Type[] {typeof (RQRequisitionLine.uOM)};


			foreach (Type type in requestOnOpen)
			{
				object value = this.Lines.Cache.GetValue(row, type.Name);
				if (value == null)
				{
					this.Lines.Cache.RaiseExceptionHandling(type.Name, row, null,
						new PXSetPropertyException(Messages.ShouldBeDefined, level));
					result = false;
				}
				else
					this.Lines.Cache.RaiseExceptionHandling(type.Name, row, value, null);
			}
			return result;
		}

		protected virtual void RQRequestLineSelect_SelectQty_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			RQRequestLineSelect row = (RQRequestLineSelect)e.Row;
			if (e.NewValue == null || (Decimal)e.NewValue > row.OpenQty)
			{
				e.NewValue = row.OpenQty;
			}
			if (e.NewValue != null && (Decimal)e.NewValue > 0)
				row.Selected = true;
		}

		protected virtual void RQRequestLineSelect_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			RQRequestLineSelect row = (RQRequestLineSelect)e.NewRow;
			RQRequestLineSelect old = (RQRequestLineSelect)e.Row;
			if (row.Selected == true && old.Selected != row.Selected && row.SelectQty.GetValueOrDefault() == 0m)
			{
				row.SelectQty = row.OpenQty;
				row.BaseSelectQty = row.OpenQty;
			}
		}
		protected virtual void RQRequestLineFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RQRequestLineFilter row = (RQRequestLineFilter)e.Row;
			if (row == null) return;
			PXUIFieldAttribute.SetEnabled<RQRequestLineFilter.inventoryID>(sender, null, row.AllowUpdate == true);
			PXUIFieldAttribute.SetEnabled<RQRequestLineFilter.subItemID>(sender, null, row.AllowUpdate == true);
			PXUIFieldAttribute.SetEnabled<RQRequestLineFilter.addExists>(sender, null, row.AllowUpdate == true);
		}

		protected virtual void RQRequisitionContent_ItemQty_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			RQRequisitionContent row = (RQRequisitionContent)e.Row;

			RQRequestLine line = PXSelect<RQRequestLine,
				Where<RQRequestLine.orderNbr, Equal<Required<RQRequestLine.orderNbr>>,
					And<RQRequestLine.lineNbr, Equal<Required<RQRequestLine.lineNbr>>>>>.Select(this, row.OrderNbr, row.LineNbr);
			Decimal delta = ((Decimal?)e.NewValue).GetValueOrDefault() - row.ItemQty.GetValueOrDefault();
			if (delta > 0 && line.OpenQty < delta)
			{
				e.NewValue = row.ItemQty + line.OpenQty;
				sender.RaiseExceptionHandling<RQRequisitionContent.itemQty>(row, null,
					new PXSetPropertyException(Messages.InsuffQty_LineQtyUpdated, PXErrorLevel.Warning));
			}
		}

		protected virtual void RQRequisitionContent_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			RQRequisitionContent row = (RQRequisitionContent)e.NewRow;
			RQRequisitionContent old = (RQRequisitionContent)e.Row;

			RQRequestLine line = PXSelect<RQRequestLine,
				Where<RQRequestLine.orderNbr, Equal<Required<RQRequestLine.orderNbr>>,
					And<RQRequestLine.lineNbr, Equal<Required<RQRequestLine.lineNbr>>>>>.Select(this, row.OrderNbr, row.LineNbr);

			RQRequisitionLine req = PXSelect<RQRequisitionLine,
							Where<RQRequisitionLine.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>,
								And<RQRequisitionLine.lineNbr, Equal<Required<RQRequisitionLine.lineNbr>>>>>.Select(this, row.ReqNbr, row.ReqLineNbr);
			if (row.RecalcOnly == true)
			{
				row.ReqQty = INUnitAttribute.ConvertFromBase(sender, req.InventoryID, req.UOM, row.BaseReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
			}
			else if (line.InventoryID == null)
			{
				if (row.ItemQty != old.ItemQty)
				{
					row.BaseItemQty =
					row.ReqQty =
					row.BaseReqQty = row.ItemQty;
				}
				else if (row.ReqQty != old.ReqQty)
				{
					row.BaseItemQty =
					row.ItemQty =
					row.BaseReqQty = row.ReqQty;
				}
			}
			else if (line.InventoryID == req.InventoryID)
			{
				if (row.ItemQty != old.ItemQty)
				{
					row.BaseReqQty =
					row.BaseItemQty = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, row.ItemQty.GetValueOrDefault(), INPrecision.QUANTITY);
					row.ReqQty = INUnitAttribute.ConvertFromBase(sender, req.InventoryID, req.UOM, row.BaseReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
				}
				if (row.ReqQty != old.ReqQty)
				{
					row.BaseReqQty =
					row.BaseItemQty = INUnitAttribute.ConvertToBase(sender, req.InventoryID, req.UOM, row.ReqQty.GetValueOrDefault(), INPrecision.QUANTITY);

					Decimal value = INUnitAttribute.ConvertFromBase(sender, line.InventoryID, line.UOM, row.BaseReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
					object newValue = value;
					sender.RaiseFieldVerifying<RQRequisitionContent.itemQty>(row, ref newValue);

					row.ItemQty = (Decimal)newValue;
					if ((Decimal)newValue != value)
					{
						row.BaseReqQty =
						row.BaseItemQty = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, row.ItemQty.GetValueOrDefault(), INPrecision.QUANTITY);
						row.ReqQty = INUnitAttribute.ConvertFromBase(sender, req.InventoryID, req.UOM, row.BaseReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
					}
				}
			}
			else
			{
				if (row.ItemQty != old.ItemQty)
					row.BaseItemQty = INUnitAttribute.ConvertToBase(sender, line.InventoryID, line.UOM, row.ItemQty.GetValueOrDefault(), INPrecision.QUANTITY);

				if (row.ReqQty != old.ReqQty)
					row.BaseReqQty = INUnitAttribute.ConvertToBase(sender, req.InventoryID, req.UOM, row.ReqQty.GetValueOrDefault(), INPrecision.QUANTITY);
			}
		}

		protected virtual void RQBiddingVendor_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			if (row != null && row.VendorLocationID != null)
			{
				row.ReqNbr = this.Document.Current.ReqNbr;
				PORemitAddressAttribute.DefaultRecord<RQBiddingVendor.remitAddressID>(sender, e.Row);
				PORemitContactAttribute.DefaultRecord<RQBiddingVendor.remitContactID>(sender, e.Row);
				e.Cancel = !ValidateBiddingVendorDuplicates(sender, row, null);
			}
		}

		protected virtual void RQBiddingVendor_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			RQBiddingVendor newRow = (RQBiddingVendor)e.NewRow;
			if (row != null && newRow != null && row != newRow &&
				 (row.VendorID != newRow.VendorID || row.VendorLocationID != newRow.VendorLocationID))
				e.Cancel = !ValidateBiddingVendorDuplicates(sender, newRow, row);
		}

		protected virtual void RQBiddingVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RQBiddingVendor row = (RQBiddingVendor)e.Row;
			if (row == null) return;
			RQBidding b =
			PXSelect<RQBidding,
				Where<RQBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>,
					And<RQBidding.vendorID, Equal<Current<RQBiddingVendor.vendorID>>,
						And<RQBidding.vendorLocationID, Equal<Current<RQBiddingVendor.vendorLocationID>>>>>>
						.SelectSingleBound(this, new object[] { row });

			bool enabled = b == null ||
										 row.VendorID == null ||
										 row.VendorLocationID == null;

			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.vendorID>(sender, row, enabled);
			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.vendorLocationID>(sender, row, enabled);
			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.curyID>(sender, row, enabled);
			PXUIFieldAttribute.SetEnabled<RQBiddingVendor.curyInfoID>(sender, row, enabled);
		}
		protected virtual void RQBiddingVendor_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				vendorBidder.Current = (Vendor)vendorBidder.View.SelectSingleBound(new object[] { e.Row });
				sender.SetDefaultExt<RQBiddingVendor.curyID>(e.Row);

				if (e.ExternalCall)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<RQBiddingVendor.curyInfoID>(sender, e.Row);

					string message = PXUIFieldAttribute.GetError<RQBiddingVendor.curyID>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<RQRequisition.orderDate>(e.Row, ((SOOrder)e.Row).OrderDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						((RQBiddingVendor)e.Row).CuryID = info.CuryID;
						((RQBiddingVendor)e.Row).CuryInfoID = info.CuryInfoID;
					}
				}				
			}
		}
		
		protected virtual void RQBiddingVendor_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PORemitAddressAttribute.DefaultRecord<RQBiddingVendor.remitAddressID>(sender, e.Row);
			PORemitContactAttribute.DefaultRecord<RQBiddingVendor.remitContactID>(sender, e.Row);
		}

		protected virtual void RQBiddingVendor_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			vendorBidder.Current = null;
		}

		protected virtual void RQBiddingVendor_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			vendorBidder.Current = null;
		}

		protected virtual void RQRequest_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			RQRequest row = e.Row as RQRequest;
			if (e.Operation == PXDBOperation.Update && row != null)
			{
				row.Status = row.OpenOrderQty > 0 ? RQRequestStatus.Open : RQRequestStatus.Closed;
			}
		}

		private bool ValidateBiddingVendorDuplicates(PXCache sender, RQBiddingVendor row, RQBiddingVendor oldRow)
		{
			if (row.VendorLocationID != null)
				foreach (RQBiddingVendor sibling in Vendors.Select(row.ReqNbr ?? this.Document.Current.ReqNbr))
				{
					if (sibling.VendorID == row.VendorID &&
							sibling.VendorLocationID == row.VendorLocationID &&
							row.LineID != sibling.LineID)
					{
						if (oldRow == null || oldRow.VendorID != row.VendorID)
							sender.RaiseExceptionHandling<RQBiddingVendor.vendorID>(
								row, row.VendorID, new PXSetPropertyException(ErrorMessages.DuplicateEntryAdded)
								);
						if (oldRow == null || oldRow.VendorLocationID != row.VendorLocationID)
							sender.RaiseExceptionHandling<RQBiddingVendor.vendorLocationID>(
								row, row.VendorLocationID, new PXSetPropertyException(ErrorMessages.DuplicateEntryAdded)
								);
						return false;
					}
				}
			PXUIFieldAttribute.SetError<RQBiddingVendor.vendorID>(sender, row, null);
			PXUIFieldAttribute.SetError<RQBiddingVendor.vendorLocationID>(sender, row, null);
			return true;
		}

		public virtual void InsertRequestLine(RQRequestLine line, Decimal selectQty, bool mergeLines)
		{
			RQRequisitionLine req = null;
			RQRequest r = PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.SelectWindowed(this, 0, 1, line.OrderNbr);
			if (r == null) return;

			if (mergeLines && line.InventoryID != null)
				req = PXSelect<RQRequisitionLine,
					Where<RQRequisitionLine.reqNbr, Equal<Required<RQRequisitionLine.reqNbr>>,
						And<RQRequisitionLine.inventoryID, Equal<Required<RQRequisitionLine.inventoryID>>,
						And<RQRequisitionLine.description, Equal<Required<RQRequisitionLine.description>>,
						And<RQRequisitionLine.subItemID, Equal<Required<RQRequisitionLine.subItemID>>,
						And<RQRequisitionLine.expenseAcctID, Equal<Required<RQRequisitionLine.expenseAcctID>>,
						And<RQRequisitionLine.expenseSubID, Equal<Required<RQRequisitionLine.expenseSubID>>,
						And<RQRequisitionLine.byRequest, Equal<Required<RQRequisitionLine.byRequest>>>>>>>>>>
						.Select(this, Document.Current.ReqNbr,
						line.InventoryID, line.Description, line.SubItemID, 
						line.ExpenseAcctID, line.ExpenseSubID, true);

			if (req == null)
			{
				req = new RQRequisitionLine();
				req.ReqNbr = Document.Current.ReqNbr;
				req = PXCache<RQRequisitionLine>.CreateCopy(this.Lines.Insert(req));
				req.InventoryID = line.InventoryID;
				req.SubItemID = line.SubItemID;
				req.Description = line.Description;
				req.UOM = line.UOM;
				req.OrderQty = 0m;
				if (Document.Current.CuryID == r.CuryID)
					req.CuryEstUnitCost = line.CuryEstUnitCost;
				else
				{
					Decimal unitCost;
					PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>(this.Lines.Cache,
						line, line.EstUnitCost.GetValueOrDefault(), out unitCost);
					req.CuryEstUnitCost = unitCost;
				}
				req.ExpenseAcctID = line.ExpenseAcctID;
				req.ExpenseSubID = line.ExpenseSubID;
				req.RequestedDate = line.RequestedDate;
				req.PromisedDate = line.PromisedDate;
				req.ByRequest = true;
				req = this.Lines.Update(req);

				PXNoteAttribute.CopyNoteAndFiles(SourceRequestLines.Cache, line, Lines.Cache, req);
			}
			else
			{
				decimal? curyEstUnitCost = null;
				decimal? curyEstExtCost = null;
				if (Document.Current.CuryID == r.CuryID)
				{
					curyEstUnitCost = line.CuryEstUnitCost;
					curyEstExtCost = line.CuryEstExtCost;
				}
				else
				{
					Decimal unitCost;
					PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>(this.Lines.Cache,
						line, line.EstUnitCost.GetValueOrDefault(), out unitCost);
					curyEstUnitCost = unitCost;
					PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>(this.Lines.Cache,
						line, line.EstExtCost.GetValueOrDefault(), out unitCost);
					curyEstExtCost = unitCost;
				}
				if (curyEstUnitCost != 0 && curyEstUnitCost != req.CuryEstExtCost)
				{
					if (req.CuryEstUnitCost == 0)
					{
						req.CuryEstUnitCost = curyEstUnitCost;
					}

					decimal? total = req.CuryEstExtCost + curyEstExtCost;

					req = PXCache<RQRequisitionLine>.CreateCopy(req);
					req.CuryEstUnitCost = total / (req.OrderQty + selectQty);
					req = this.Lines.Update(req);
				}
			}
			UpdateContent(req, line, selectQty);
			if (req != null)
			{
				req = PXCache<RQRequisitionLine>.CreateCopy(req);
				if (req.LineType == null && req.InventoryID == null)
					req.LineType = POLineType.Service;
				this.Lines.Cache.SetDefaultExt<RQRequisitionLine.siteID>(req);
				this.Lines.Cache.Update(req);
			}
		}

		private RQRequisitionContent UpdateContent(RQRequisitionLine req, RQRequestLine line, Decimal selectQty)
		{
			RQRequisitionContent content = PXSelect<RQRequisitionContent,
				Where<RQRequisitionContent.orderNbr, Equal<Required<RQRequisitionContent.orderNbr>>,
					And<RQRequisitionContent.lineNbr, Equal<Required<RQRequisitionContent.lineNbr>>,
					And<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
					And<RQRequisitionContent.reqLineNbr, Equal<Required<RQRequisitionContent.reqLineNbr>>>>>>>.Select(this, line.OrderNbr, line.LineNbr, req.ReqNbr, req.LineNbr);
			if (content == null)
			{
				content = new RQRequisitionContent();
				content.OrderNbr = line.OrderNbr;
				content.LineNbr = line.LineNbr;
				content.ReqNbr = req.ReqNbr;
				content.ReqLineNbr = req.LineNbr;
				content = this.Contents.Insert(content);
			}
			content = (RQRequisitionContent)this.Contents.Cache.CreateCopy(content);
			content.ItemQty += selectQty;
			return this.Contents.Update(content);
		}

		private long? CopyCurrenfyInfo(PXGraph graph, long? SourceCuryInfoID)
		{
			CurrencyInfo curryInfo = currencyinfo.Select(SourceCuryInfoID);
			curryInfo.CuryInfoID = null;
			graph.Caches[typeof (CurrencyInfo)].Clear();
			curryInfo = (CurrencyInfo)graph.Caches[typeof(CurrencyInfo)].Insert(curryInfo);
			return curryInfo.CuryInfoID;
		}

		private class PO4SO : Dictionary<int?, List<POLine>>
		{
			public virtual void Add(int? key, POLine line)
			{
				if (line == null) return;

				List<POLine> source;
				if (!this.TryGetValue(key, out source))
					this[key] = source = new List<POLine>();

				source.Add(line);
			}
		}
	}

    [Serializable]
	public partial class RQRequisitionStatic : IBqlTable
	{
		#region SourceReqNbr
		public abstract class sourceReqNbr : PX.Data.IBqlField
		{
		}
		protected String _SourceReqNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Source Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(RQSetup.requisitionNumberingID), typeof(RQRequisition.orderDate))]
		[PXSelectorAttribute(
			typeof(Search<RQRequisition.reqNbr, Where<RQRequisition.status, Equal<RQRequisitionStatus.hold>>>),
			typeof(RQRequisition.employeeID),
			typeof(RQRequisition.vendorID),
			Filterable = true)]
		public virtual String SourceReqNbr
		{
			get
			{
				return this._SourceReqNbr;
			}
			set
			{
				this._SourceReqNbr = value;
			}
		}
		#endregion
		#region ReqNbr
		public abstract class reqNbr : PX.Data.IBqlField
		{
		}
		protected String _ReqNbr;

		[PXDBString(15, IsUnicode = true, InputMask = "")]
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
	}
	[PX.TM.OwnedEscalatedFilter.Projection(
		typeof(RQRequestLineFilter),
		typeof(RQRequestLine),
		typeof(InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequestLine.orderNbr>,
									And<RQRequest.status, Equal<RQRequestStatus.open>,
									And<RQRequestLine.openQty, Greater<PX.Objects.CS.decimal0>>>>>),
		null,
		typeof(RQRequest.workgroupID),
		typeof(RQRequest.ownerID),
		typeof(RQRequest.orderDate))]
    [Serializable]
	public partial class RQRequestLineSelect : IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected string _OrderNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(RQRequestLine.orderNbr))]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(RQRequestLine.lineNbr))]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(RQRequest.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region SelectQty
		public abstract class selectQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _SelectQty;
		[PXQuantity(typeof(RQRequestLineSelect.uOM), typeof(RQRequestLineSelect.baseSelectQty))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Select Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? SelectQty
		{
			get
			{
				return this._SelectQty;
			}
			set
			{
				this._SelectQty = value;
			}
		}
		#endregion
		#region BaseSelectQty
		public abstract class baseSelectQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseSelectQty;
		[PXDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? BaseSelectQty
		{
			get
			{
				return this._BaseSelectQty;
			}
			set
			{
				this._BaseSelectQty = value;
			}
		}
		#endregion
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseOpenQty), HandleEmptyKey = true, BqlField = typeof(RQRequestLine.openQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
		public virtual Decimal? OpenQty
		{
			get
			{
				return this._OpenQty;
			}
			set
			{
				this._OpenQty = value;
			}
		}
		#endregion
		#region BaseOpenQty
		public abstract class baseOpenQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOpenQty;
		[PXDBDecimal(6, BqlField = typeof(RQRequestLine.baseOpenQty))]
		public virtual Decimal? BaseOpenQty
		{
			get
			{
				return this._BaseOpenQty;
			}
			set
			{
				this._BaseOpenQty = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true, BqlField = typeof(RQRequestLine.inventoryID))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(RQRequestLine.inventoryID), BqlField = typeof(RQRequestLine.subItemID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(50, IsUnicode = true, BqlField = typeof(RQRequestLine.description))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<RQRequestLineSelect.inventoryID>>>>))]
		[INUnit(typeof(RQRequestLineSelect.inventoryID), DisplayName = "UOM", BqlField = typeof(RQRequestLine.uOM))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField=typeof(RQRequestLine.vendorID))]
		[PXDefault(
			typeof(
			Search2<Vendor.bAccountID,
				LeftJoin<InventoryItem,
							On<InventoryItem.preferredVendorID, Equal<Vendor.bAccountID>,
						And<InventoryItem.inventoryID, Equal<Current<RQRequestLine.inventoryID>>>>>,
			Where2<
					Where<Current<RQRequest.vendorID>, IsNotNull, And<Vendor.bAccountID, Equal<Current<RQRequest.vendorID>>>>,
			Or<
					Where<Current<RQRequest.vendorID>, IsNull, And<InventoryItem.preferredVendorID, IsNotNull>>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region VendorLocationID
		public abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorLocationID;

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequestLine.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(RQRequestLine.vendorLocationID))]
		[PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<RQRequestLine.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region VendorName
		public abstract class vendorName : PX.Data.IBqlField
		{
		}
		protected String _VendorName;
		[PXString(50, IsUnicode = true)]
		[PXDBScalar(typeof(Search<Vendor.acctName, Where<Vendor.bAccountID, Equal<RQRequestLine.vendorID>>>))]
		[PXDefault(typeof(Search<Vendor.acctName, Where<Vendor.bAccountID, Equal<Current<RQRequestLine.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Name", Enabled = false)]
		public virtual String VendorName
		{
			get
			{
				return this._VendorName;
			}
			set
			{
				this._VendorName = value;
			}
		}
		#endregion
		#region VendorRefNbr
		public abstract class vendorRefNbr : PX.Data.IBqlField
		{
		}
		protected String _VendorRefNbr;
		[PXDBString(40, IsUnicode = true, BqlField = typeof(RQRequestLine.vendorRefNbr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Ref.")]
		public virtual String VendorRefNbr
		{
			get
			{
				return this._VendorRefNbr;
			}
			set
			{
				this._VendorRefNbr = value;
			}
		}
		#endregion
		#region VendorDescription
		public abstract class vendorDescription : PX.Data.IBqlField
		{
		}
		protected String _VendorDescription;
		[PXDBString(100, IsUnicode = true, BqlField=typeof(RQRequestLine.vendorDescription))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Description")]
		public virtual String VendorDescription
		{
			get
			{
				return this._VendorDescription;
			}
			set
			{
				this._VendorDescription = value;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[PXDBString(50, IsUnicode = true, InputMask = "", BqlField=typeof(RQRequestLine.alternateID))]		
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion
		#region RequestedDate
		public abstract class requestedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _RequestedDate;
		[PXDBDate(BqlField=typeof(RQRequestLine.requestedDate))]
		[PXUIField(DisplayName = "Required Date")]
		public virtual DateTime? RequestedDate
		{
			get
			{
				return this._RequestedDate;
			}
			set
			{
				this._RequestedDate = value;
			}
		}
		#endregion
		#region PromisedDate
		public abstract class promisedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PromisedDate;
		[PXDBDate(BqlField=typeof(RQRequestLine.promisedDate))]
		[PXUIField(DisplayName = "Promised Date")]
		public virtual DateTime? PromisedDate
		{
			get
			{
				return this._PromisedDate;
			}
			set
			{
				this._PromisedDate = value;
			}
		}
		#endregion
		#region ReqQty
		public abstract class reqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReqQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseReqQty), HandleEmptyKey = true, BqlField=typeof(RQRequestLine.reqQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Requisition Qty.", Enabled = false)]
		public virtual Decimal? ReqQty
		{
			get
			{
				return this._ReqQty;
			}
			set
			{
				this._ReqQty = value;
			}
		}
		#endregion
		#region BaseReqQty
		public abstract class baseReqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseReqQty;
		[PXDBDecimal(6, BqlField = typeof(RQRequestLine.baseReqQty))]
		public virtual Decimal? BaseReqQty
		{
			get
			{
				return this._BaseReqQty;
			}
			set
			{
				this._BaseReqQty = value;
			}
		}
		#endregion	
	}

	//Add fields
	[System.SerializableAttribute()]
	public partial class RQRequestLineFilter : RQRequestSelection
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true)]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(RQRequestLineFilter.inventoryID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region AllowUpdate
		public abstract class allowUpdate : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowUpdate = true;
		[PXDBBool()]
		[PXDefault(true)]
		public virtual Boolean? AllowUpdate
		{
			get
			{
				return this._AllowUpdate;
			}
			set
			{
				this._AllowUpdate = value;
			}
		}
		#endregion
	}
}
