using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;

using PX.TM;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using PX.Objects.PM;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.PO
{
	#region Dac Types Overrides
    [PXCacheName(Messages.POShipAddressFull)]
    [Serializable]
	public partial class POShipAddress : POAddress
	{
		#region AddressID
		public new abstract class addressID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}

		[PXDBInt()]
		//[PXDBDefault(typeof(Company.bAccountID))]
		public override Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region BAccountAddressID
		public new abstract class bAccountAddressID : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsDefaultAddress
		public new abstract class isDefaultAddress : PX.Data.IBqlField
		{
		}
		#endregion
		#region OverrideAddress
		public new abstract class overrideAddress : PX.Data.IBqlField
		{
		}
		#endregion
		#region RevisionID
		public new abstract class revisionID : PX.Data.IBqlField
		{
		}

		#endregion
		#region CountryID
		public new abstract class countryID : PX.Data.IBqlField
		{
		}

		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(Search<GL.Branch.countryID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXUIField(DisplayName = "Country")]
		[PXSelector(typeof(Search<CS.Country.countryID>), DescriptionField = typeof(Country.description), CacheGlobal = true)]
		public override String CountryID
		{
			get
			{
				return this._CountryID;
			}
			set
			{
				this._CountryID = value;
			}
		}
		#endregion
		#region StateID
		public new abstract class state : PX.Data.IBqlField
		{
		}

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[CR.State(typeof(POShipAddress.countryID), DescriptionField = typeof(State.name))]
		public override String State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
			}
		}

		#endregion
		#region PostalCode
		public new abstract class postalCode : PX.Data.IBqlField
		{
		}
		[PXDBString(20)]
		[PXUIField(DisplayName = "Postal Code")]
		[PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), countryIdField: typeof(POShipAddress.countryID))]
		public override String PostalCode
		{
			get
			{
				return this._PostalCode;
			}
			set
			{
				this._PostalCode = value;
			}
		}
		#endregion
		#region IsValidated
		public new abstract class isValidated : PX.Data.IBqlField
		{
		}
		#endregion
	}

    [PXCacheName(Messages.PORemitAddressFull)]
    [Serializable]
	public partial class PORemitAddress : POAddress
	{
		#region AddressID
		public new abstract class addressID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}

		[PXDBInt()]
		//[PXDBDefault(typeof(Company.bAccountID))]
		public override Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region BAccountAddressID
		public new abstract class bAccountAddressID : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsDefaultAddress
		public new abstract class isDefaultAddress : PX.Data.IBqlField
		{
		}
		#endregion
		#region OverrideAddress
		public new abstract class overrideAddress : PX.Data.IBqlField
		{
		}
		#endregion
		#region RevisionID
		public new abstract class revisionID : PX.Data.IBqlField
		{
		}

		#endregion
		#region CountryID
		public new abstract class countryID : PX.Data.IBqlField
		{
		}

		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(Search<GL.Branch.countryID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXUIField(DisplayName = "Country")]
		[PXSelector(typeof(Search<CS.Country.countryID>), DescriptionField = typeof(Country.description), CacheGlobal = true)]
		public override String CountryID
		{
			get
			{
				return this._CountryID;
			}
			set
			{
				this._CountryID = value;
			}
		}

		#endregion
		#region StateID
		public new abstract class state : PX.Data.IBqlField
		{
		}
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[CR.State(typeof(PORemitAddress.countryID), DescriptionField = typeof(State.name))]
		public override String State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
			}
		}

		#endregion
		#region PostalCode
		public new abstract class postalCode : PX.Data.IBqlField
		{
		}
		[PXDBString(20)]
		[PXUIField(DisplayName = "Postal Code")]
		[PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), countryIdField: typeof(PORemitAddress.countryID))]
		public override String PostalCode
		{
			get
			{
				return this._PostalCode;
			}
			set
			{
				this._PostalCode = value;
			}
		}
		#endregion
		#region IsValidated
		public new abstract class isValidated : PX.Data.IBqlField
		{
		}
		#endregion
	}

    [PXCacheName(Messages.POShipContactFull)]
    [Serializable]
	public partial class POShipContact : POContact
	{
		#region ContactID
		public new abstract class contactID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountContactID
		public new abstract class bAccountContactID : PX.Data.IBqlField
		{
		}
		#endregion
		#region RevisionID
		public new abstract class revisionID : PX.Data.IBqlField
		{
		}

		#endregion
		#region IsDefaultContact
		public new abstract class isDefaultContact : PX.Data.IBqlField
		{
		}
		#endregion
		#region OverrideContact
		public new abstract class overrideContact : PX.Data.IBqlField
		{
		}
		#endregion
	}

    [PXCacheName(Messages.PORemitContactFull)]
    [Serializable]
	public partial class PORemitContact : POContact
	{
		#region ContactID
		public new abstract class contactID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BAccountContactID
		public new abstract class bAccountContactID : PX.Data.IBqlField
		{
		}
		#endregion
		#region RevisionID
		public new abstract class revisionID : PX.Data.IBqlField
		{
		}

		#endregion
		#region IsDefaultContact
		public new abstract class isDefaultContact : PX.Data.IBqlField
		{
		}
		#endregion
		#region OverrideContact
		public new abstract class overrideContact : PX.Data.IBqlField
		{
		}
		#endregion
	}
	#endregion
	   
    [Serializable]
	public class POOrderEntry : PXGraph<POOrderEntry, POOrder>, PXImportAttribute.IPXPrepareItems
	{
		#region Ctor + Public Selects
		public PXSetup<Company> company;
		public PXSelect<IN.InventoryItem> dummy_stockitem_for_redirect_newitem;
		[PXViewName(Messages.POOrder)]
		public PXSelectJoin<POOrder,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POOrder.vendorID>>>,
			Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>,
			And<Where<Vendor.bAccountID, IsNull,
			Or<Match<Vendor, Current<AccessInfo.userName>>>>>>> Document;
		public PXSelect<POOrder, Where<POOrder.orderType, Equal<Current<POOrder.orderType>>,
				And<POOrder.orderNbr, Equal<Current<POOrder.orderNbr>>>>> CurrentDocument;
		[PXViewName(Messages.POLine)]
		[PXImport(typeof(POOrder))]
		[PXCopyPasteHiddenFields(typeof(POLine.completed), typeof(POLine.cancelled))]
		public PXSelect<POLine, Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Optional<POOrder.orderNbr>>>>, OrderBy<Asc<POLine.orderType, Asc<POLine.orderNbr, Asc<POLine.lineNbr>>>>> Transactions;
		public PXSelect<POTax, Where<POTax.orderType, Equal<Current<POOrder.orderType>>, And<POTax.orderNbr, Equal<Current<POOrder.orderNbr>>>>, OrderBy<Asc<POTax.orderType, Asc<POTax.orderNbr, Asc<POTax.taxID>>>>> Tax_Rows;
		public TaxTranSelect<POOrder, POOrder.termsID, POTaxTran, POTaxTran.taxID,
			Where<POTaxTran.orderType, Equal<Current<POOrder.orderType>>,
				And<POTaxTran.orderNbr, Equal<Current<POOrder.orderNbr>>>>> Taxes;
        [PXCopyPasteHiddenView]
        public PXSelect<POOrderDiscountDetail, Where<POOrderDiscountDetail.orderType, Equal<Current<POOrder.orderType>>, And<POOrderDiscountDetail.orderNbr, Equal<Current<POOrder.orderNbr>>>>, OrderBy<Asc<POOrderDiscountDetail.orderType, Asc<POOrderDiscountDetail.orderNbr>>>> DiscountDetails;

		[PXViewName(Messages.PORemitAddress)]
		public PXSelect<PORemitAddress, Where<PORemitAddress.addressID, Equal<Current<POOrder.remitAddressID>>>> Remit_Address;
		[PXViewName(Messages.PORemitContact)]
		public PXSelect<PORemitContact, Where<PORemitContact.contactID, Equal<Current<POOrder.remitContactID>>>> Remit_Contact;
		[PXViewName(Messages.POShipAddress)]
		public PXSelect<POShipAddress, Where<POShipAddress.addressID, Equal<Current<POOrder.shipAddressID>>>> Shipping_Address;
		[PXViewName(Messages.POShipContact)]
		public PXSelect<POShipContact, Where<POShipContact.contactID, Equal<Current<POOrder.shipContactID>>>> Shipping_Contact;

		[CRReference(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<POOrder.vendorID>>>>))]
		[CRDefaultMailTo(typeof(Select<PORemitContact, Where<PORemitContact.contactID, Equal<Current<POOrder.remitContactID>>>>))]
		public CRActivityList<POOrder>
			Activity;

        public PXSelect<POSetupApproval, Where<POSetupApproval.orderType, Equal<Optional<POOrder.orderType>>>> SetupApproval;

		[PXViewName(Messages.Approval)]
		public EPApprovalAutomation<POOrder, POOrder.approved, POOrder.rejected, POOrder.hold, POSetupApproval> Approval;

		[PXCopyPasteHiddenView]
		public PXSelect<INReplenishmentOrder> Replenihment;
		[PXCopyPasteHiddenView]
		public PXSelect<INReplenishmentLine,
			Where<INReplenishmentLine.pOType, Equal<Current<POLine.orderType>>,
				And<INReplenishmentLine.pONbr, Equal<Current<POLine.orderNbr>>,
				And<INReplenishmentLine.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>> ReplenishmentLines;

		public PXSelect<POItemCostManager.POVendorInventoryPriceUpdate> priceStatus;
		public PXSelect<CurrencyInfo> currencyinfo;
		public PXSetup<POSetup> POSetup;
		public PXSetup<APSetup> apsetup;
		public PXSetup<INSetup> INSetup;
		public PXSetup<Branch, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>> Company;

		public PXSetupOptional<TXAvalaraSetup> avalaraSetup;
        [PXCopyPasteHiddenView]
        public PXSelectJoin<POReceipt,
            LeftJoin<POOrderReceipt, On<POReceipt.receiptNbr, Equal<POOrderReceipt.receiptNbr>>>, 
            Where<POOrderReceipt.pOType, Equal<Current<POOrder.orderType>>, And<POOrderReceipt.pONbr, Equal<Current<POOrder.orderNbr>>>>> Receipts;
        [PXViewName(CR.Messages.MainContact)]
        public PXSelect<Contact> DefaultCompanyContact;    
        protected virtual IEnumerable defaultCompanyContact()
        {
            List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
            foreach (PXResult<GL.Branch, BAccountR, Contact> res in PXSelectJoin<GL.Branch,
                                        LeftJoin<BAccountR, On<GL.Branch.bAccountID, Equal<BAccountR.bAccountID>>,
                                        LeftJoin<Contact, On<BAccountR.defContactID, Equal<Contact.contactID>>>>,
                                        Where<GL.Branch.branchID, Equal<Required<GL.Branch.branchID>>>>.Select(this, branches != null ? (int?)branches[0] : null))
            {
                yield return (Contact)res;
                break;
            }
        }

        #region SiteStatus Lookup
		public PXFilter<POSiteStatusFilter> sitestatusfilter;
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public POSiteStatusLookup<POSiteStatusSelected, INSiteStatusFilter> sitestatus;

		public PXAction<POOrder> addInvBySite;
		[PXUIField(DisplayName = "Add Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AddInvBySite(PXAdapter adapter)
		{
			sitestatusfilter.Cache.Clear();
			if (this.Document.Current.Hold == true)
			{
				if (sitestatus.AskExt() == WebDialogResult.OK)
				{
					return AddInvSelBySite(adapter);
				}
				sitestatusfilter.Cache.Clear();
				sitestatus.Cache.Clear();
			}
			return adapter.Get();
		}

		public PXAction<POOrder> addInvSelBySite;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddInvSelBySite(PXAdapter adapter)
		{
			if (this.Document.Current.Hold == true)
			{
				foreach (POSiteStatusSelected line in sitestatus.Cache.Cached)
				{
					if (line.Selected == true && line.QtySelected > 0)
					{
						POLine newline = new POLine();
						newline.SiteID = line.SiteID;
						newline.LineType = this.Document.Current.OrderType == POOrderType.DropShip
																? POLineType.GoodsForDropShip
																: POLineType.GoodsForInventory;
						newline.InventoryID = line.InventoryID;
						newline.SubItemID = line.SubItemID;
						newline.UOM = line.PurchaseUnit;
						newline.OrderQty = line.QtySelected;
						newline.AlternateID = line.AlternateID;
						Transactions.Insert(newline);
					}
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
				Location location = PXSelect<Location,
										Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
										And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>.SelectWindowed(this, 0, 1);
				if (location != null && location.VSiteID != null)
					row.SiteID = location.VSiteID;
				row.VendorID = Document.Current.VendorID;
			}
		}
		#endregion


		[PXViewName(AP.Messages.Vendor)]
		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<POOrder.vendorID>>>> vendor;
		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;
		[PXViewName(CR.Messages.Employee)]
		public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<POOrder.employeeID>>>> Employee;

		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<POOrder.taxZoneID>>>> taxzone;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<POOrder.vendorID>>, And<Location.locationID, Equal<Optional<POOrder.vendorLocationID>>>>> location;

		#region Add PO Order sub-form
		public PXFilter<POOrderFilter> filter;
		public PXSelect<POLineS,
					Where<POLineS.orderType, Equal<Current<POOrderFilter.orderType>>,
								And<POLineS.orderNbr, Equal<Current<POOrderFilter.orderNbr>>,
								And<POLineS.lineType, NotEqual<POLineType.description>,
								And<POLineS.completed, Equal<boolFalse>>>>>>
								poLinesSelection;

		public PXSelect<POOrderS,
								Where<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
								And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
								And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
								And<POOrderS.hold, Equal<boolFalse>,
								And<POOrderS.cancelled, Equal<boolFalse>,
								And<POOrderS.approved, Equal<boolTrue>>>>>>>,
								OrderBy<Asc<POOrderS.orderNbr>>>
								openOrders;

		public PXSelect<POLineR> poLiner;
		public PXSelect<POOrderR,
			Where<POOrderR.orderType, Equal<Required<POOrderR.orderType>>,
			And<POOrderR.orderNbr, Equal<Required<POOrderR.orderNbr>>,
			And<POOrderR.status, Equal<Required<POOrderR.status>>>>>> poOrder;

		#endregion

		#region SO Demand sub-form
		[PXCopyPasteHiddenView]
		public PXSelectJoin<SOLineSplit3,
			LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<SOLineSplit3.planID>, And<INItemPlan.supplyPlanID, Equal<Optional<POLine.planID>>>>,
            LeftJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.orderType, Equal<SOLineSplit3.orderType>, And<SOOrderTypeOperation.operation, Equal<SOLineSplit3.operation>>>>>,
			Where<SOLineSplit3.pOType, Equal<Optional<POLine.orderType>>,
				And<SOLineSplit3.pONbr, Equal<Optional<POLine.orderNbr>>,
				And<SOLineSplit3.pOLineNbr, Equal<Optional<POLine.lineNbr>>>>>> FixedDemand;
		#endregion
        public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;
        //Required for right cache creation to prevent incorrect planning which occurs when lines import from Excel
        public PXSelect<INItemPlan> itemPlans;

		public POOrderEntry()
		{
			POSetup setup = POSetup.Current;
			APSetup apSetup = apsetup.Current;
			INSetup inSetup = INSetup.Current;
			RowUpdated.AddHandler<POOrder>(ParentFieldUpdated);
			POOrderFilter currentFilter = filter.Current;
			this.poLiner.Cache.AllowInsert = false;
			this.poLiner.Cache.AllowDelete = false;

			this.poLinesSelection.Cache.AllowInsert = false;
			this.poLinesSelection.Cache.AllowDelete = false;
			this.poLinesSelection.Cache.AllowUpdate = true;

			this.openOrders.Cache.AllowInsert = false;
			this.openOrders.Cache.AllowDelete = false;
			this.openOrders.Cache.AllowUpdate = true;

			FixedDemand.AllowDelete = false;
			FixedDemand.AllowInsert = false;

			PXFieldState state = (PXFieldState)this.Transactions.Cache.GetStateExt<POLine.inventoryID>(null);
			viewInventoryID = state != null ? state.ViewName : null;

			PXUIFieldAttribute.SetVisible<POLine.projectID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO));
			PXUIFieldAttribute.SetVisible<POLine.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO));

            TaxAttribute.SetTaxCalc<POLine.taxCategoryID>(Transactions.Cache, null, TaxCalc.ManualLineCalc);

			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}
		protected readonly string viewInventoryID;
		#region Select Overloads

#if false
		private bool skipCheck;
		
		public virtual IEnumerable openorders()
		{
			POOrder row = this.Document.Current;
			if (row == null) yield break;

			PXSelectBase<POOrderS> select = new PXSelect<POOrderS,
								Where<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
								And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
								And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
								And<POOrderS.hold, Equal<boolFalse>,
								And<POOrderS.cancelled, Equal<boolFalse>,
								And<POOrderS.approved, Equal<boolTrue>>>>>>>,
								OrderBy<Asc<POOrderS.orderDate>>>(this);
			foreach (POOrderS it in select.Select())
			{
				if (!this.skipCheck)
				{
					if (it.ExpirationDate.HasValue && it.ExpirationDate < row.OrderDate)
					{
						this.openOrders.Cache.RaiseExceptionHandling<POOrderS.expirationDate>(it, it.ExpirationDate, new PXSetPropertyException(Messages.POBlanketOrderExpiresBeforeTheDateOfDocument, PXErrorLevel.RowWarning, it.ExpirationDate.Value));
					}
				}
				yield return it;
			}
		}  
#endif
		#endregion
		#endregion

		#region Buttons
		public PXAction<POOrder> hold;
		[PXUIField(DisplayName = "Hold", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
            return adapter.Get();
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
                            ((PXAction<POOrder>)this.Actions["hold"]).PressImpl(false);
                            values["Hold"] = PXCache.NotSetValue;
                        }
                    }
                }
            }
            return base.ExecuteUpdate(viewName, keys, values, parameters);
        }

        public PXAction<POOrder> action;
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
			List<POOrder> result = new List<POOrder>();
			if (actionName != null)
			{
				PXAction a = this.Actions[actionName];
				if (a != null)
					foreach (PXResult<POOrder> e in a.Press(adapter))
						result.Add(e);
			}
			else
				foreach (POOrder e in adapter.Get<POOrder>())
					result.Add(e);

			if (refresh)
			{
				foreach (POOrder order in result)
					Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);
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

		public PXAction<POOrder> complete;
		[PXUIField(DisplayName = "Complete", Visible = false)]
		[PXButton]
		protected virtual IEnumerable Complete(PXAdapter adapter)
		{
			foreach (PXResult<POOrder> r in adapter.Get())
			{
				POOrder doc = r;
				Document.Current = doc;
				foreach (POLine line in this.Transactions.Select())
				{
					if (line.Completed == true) continue;

					POLine upd = (POLine)this.Transactions.Cache.CreateCopy(line);
					upd.Completed = true;
					this.Transactions.Update(upd);
				}
				CheckOpenStatus(doc);
				yield return r;
			}
		}

		public PXAction<POOrder> notification;
		[PXUIField(DisplayName = "Notifications", Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Notification(PXAdapter adapter,
		[PXString]
		string notificationCD)
		{
			foreach (POOrder order in adapter.Get<POOrder>())
			{
				Document.Cache.Current = order;
				var parameters = new Dictionary<string, string>();
				parameters["POOrder.OrderType"] = order.OrderType;
				parameters["POOrder.OrderNbr"] = order.OrderNbr;
				
				using (var ts = new PXTransactionScope())
				{
				Activity.SendNotification(APNotificationSource.Vendor, notificationCD, order.BranchID, parameters);
					this.Save.Press();

					ts.Complete();
				}

				yield return order;
			}
		}

		public PXAction<POOrder> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2 }, new string[] { "Vendor Details", "Activities" })]
			int? inquiryID
			)
		{
			switch (inquiryID)
			{
				case 1:
					if (vendor.Current != null)
					{
						APDocumentEnq graph = PXGraph.CreateInstance<APDocumentEnq>();
						graph.Filter.Current.VendorID = vendor.Current.BAccountID;
						graph.Filter.Select();
						throw new PXRedirectRequiredException(graph, "Vendor Details");
					}
					break;
				case 2:
					if (Document.Current != null)
						Activity.ButtonViewAllActivities.PressButton(adapter);
					break;
			}
			return adapter.Get();
		}

		public PXAction<POOrder> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Report(PXAdapter adapter,
			[PXString(8, InputMask = "CC.CC.CC.CC")]
			string reportID,
			[PXBool]
			bool sendByEmail,
			[PXBool]
			bool refresh
			)
		{
            List<POOrder> list = adapter.Get<POOrder>().ToList();
			if (!String.IsNullOrEmpty(reportID))
			{
                
                Save.Press();
				int i = 0;
				string actualReportID = null;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				foreach (POOrder order in list)
				{
					parameters["POOrder.OrderType" + i.ToString()] = order.OrderType;
					parameters["POOrder.OrderNbr" + i.ToString()] = order.OrderNbr;
					Location loc =
					PXSelect<Location,
					Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
						And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this, order.VendorID, order.VendorLocationID);
					i++;
					if (actualReportID == null)
						actualReportID = new NotificationUtility(this).SearchReport(APNotificationSource.Vendor, vendor.Current, reportID, order.BranchID);
					if (refresh)
						Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);
				}
				if (i > 0)
				{
					if (sendByEmail)
					{
						try
						{
							if (ReportNotificationGenerator.Send(actualReportID, parameters).Any())
							{
								this.SelectTimeStamp();
								Save.Press();
							}
							else
							{
								throw new PXException(ErrorMessages.MailSendFailed);
							}
						}
						finally
						{
							Clear();
						}
					}
					else
					{
						Save.Press();
						throw new PXReportRequiredException(parameters, actualReportID, "Report " + actualReportID);
					}
				}
			}
			return list;
		}
		public PXAction<POOrder> viewDemand;
		[PXUIField(DisplayName = Messages.ViewDemand, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewDemand(PXAdapter adapter)
		{
			INReplenishmentLine line = this.ReplenishmentLines.SelectWindowed(0, 1);
			if (line != null)
				this.ReplenishmentLines.AskExt();
			else
				this.FixedDemand.AskExt();
			return adapter.Get();
		}

		public ToggleCurrency<POOrder> CurrencyView;

		public PXAction<POOrder> newVendor;
		[PXUIField(DisplayName = Messages.NewVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewVendor(PXAdapter adapter)
		{
			VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
			throw new PXRedirectRequiredException(graph, Messages.NewVendor);
		}

		public PXAction<POOrder> editVendor;
		[PXUIField(DisplayName = Messages.EditVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditVendor(PXAdapter adapter)
		{
			if (vendor.Current != null)
			{
				VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
				graph.BAccount.Current = (VendorR)vendor.Current;
				throw new PXRedirectRequiredException(graph, Messages.EditVendor);
			}
			return adapter.Get();
		}

		public PXAction<POLine> newItem;
		[PXUIField(DisplayName = Messages.NewItem, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewItem(PXAdapter adapter)
		{
			InventoryItemMaint graph = PXGraph.CreateInstance<InventoryItemMaint>();
			throw new PXRedirectRequiredException(graph, "New Item");
		}

		public PXAction<POLine> editItem;
		[PXUIField(DisplayName = Messages.EditItem, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditItem(PXAdapter adapter)
		{
			if (Transactions.Current != null && Transactions.Current.InventoryID != null)
			{
				InventoryItemMaint graph = PXGraph.CreateInstance<InventoryItemMaint>();
				graph.Item.Current.InventoryID = Transactions.Current.InventoryID;
				graph.Item.Cache.RaiseFieldUpdated("InvenotoryID", graph.Item.Current, null);
				throw new PXRedirectRequiredException(graph, "Edit Item");
			}
			return adapter.Get();
		}

		public PXAction<POOrder> addPOOrder;
		[PXUIField(DisplayName = Messages.AddBlanketOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrder(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 this.Document.Current.Hold == true &&
				 POOrderType.IsUseBlanket(this.Document.Current.OrderType))
			{
				if (this.openOrders.AskExt() == WebDialogResult.OK)
				{
					//this.skipCheck = true;		
					foreach (POOrderS it in this.openOrders.Cache.Updated)
					{
						if ((bool)it.Selected)
							this.AddPurchaseOrder(it);
						it.Selected = false;
					}
					//this.skipCheck = false;
				}
				else
				{
					//this.skipCheck = true;		
					foreach (POOrderS it in this.openOrders.Cache.Updated)
						it.Selected = false;
					//this.skipCheck = false;	
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> addPOOrderLine;
		[PXUIField(DisplayName = Messages.AddBlanketLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrderLine(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Hold == true &&
					POOrderType.IsUseBlanket(this.Document.Current.OrderType))
			{
				if (this.poLinesSelection.AskExt() == WebDialogResult.OK)
				{
					foreach (POLineS it in poLinesSelection.Cache.Updated)
					{
						if ((bool)it.Selected)
							this.AddPOLine(it, this.filter.Current.OrderType == POOrderType.Blanket);
						it.Selected = false;
					}
				}
				else
				{
					foreach (POLineS it in poLinesSelection.Cache.Updated)
						it.Selected = false;
				}
			}

			return adapter.Get();
		}

		public PXAction<POOrder> createPOReceipt;
		[PXUIField(DisplayName = Messages.CreatePOReceipt, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable CreatePOReceipt(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 (this.Document.Current.OrderType == POOrderType.RegularOrder
					|| this.Document.Current.OrderType == POOrderType.DropShip))
			{
				POOrder order = (POOrder)this.Document.Current;
				if (order.Status == POOrderStatus.Open)
				{

					bool needsPOReceipt = false;
					foreach (POLine iLn in this.Transactions.Select())
					{
						if (NeedsPOReceipt(iLn, true))
						{
							needsPOReceipt = true;
							break;
						}
					}
					if (needsPOReceipt)
					{
						this.Save.Press();

						PXLongOperation.StartOperation(this, delegate()
						{
						POReceiptEntry receiptGraph = PXGraph.CreateInstance<POReceiptEntry>();
						POReceipt receipt = new POReceipt();
						receipt.ReceiptType = POReceiptType.POReceipt;
						receipt.BranchID = order.BranchID;
						receipt.VendorID = order.VendorID;
						receipt.VendorLocationID = order.VendorLocationID;
						receipt.TermsID = order.TermsID;
						receipt.CuryID = order.CuryID;
							if (!string.IsNullOrEmpty(order.TaxZoneID))
							receipt.TaxZoneID = order.TaxZoneID;
						receipt = receiptGraph.Document.Insert(receipt);
						POReceipt copy = (POReceipt)receiptGraph.Document.Cache.CreateCopy(receipt);
						copy.CuryID = order.CuryID;
						copy = receiptGraph.Document.Update(copy);
						receiptGraph.AddPurchaseOrder(order);
						if (receiptGraph.transactions.Cache.IsDirty)
							throw new PXRedirectRequiredException(receiptGraph, Messages.POReceiptRedirection);

					throw new PXException(Messages.POReceiptFromOrderCreation_NoApplicableLinesFound);
						});
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> createAPInvoice;
		[PXUIField(DisplayName = Messages.CreateAPInvoice, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXProcessButton]
		public virtual IEnumerable CreateAPInvoice(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 (this.Document.Current.OrderType == POOrderType.RegularOrder
					|| this.Document.Current.OrderType == POOrderType.DropShip))
			{
				POOrder order = (POOrder)this.Document.Current;
				if (order.Status == POOrderStatus.Open)
				{      
                        bool needsAPInvoice = false;
                        foreach (POLine iLn in this.Transactions.Select())
                        {
                            if (NeedsAPInvoice(iLn, true))
                            {
                                needsAPInvoice = true;
                                break;
                            }
                        }
                        if (needsAPInvoice)
                        {
                            this.Save.Press();
                            APInvoiceEntry receiptGraph = PXGraph.CreateInstance<APInvoiceEntry>();
                            receiptGraph.InvoicePOOrder(order, true);
                            receiptGraph.AttachPrepayment();
                            throw new PXRedirectRequiredException(receiptGraph, Messages.POReceiptRedirection);
                        }
                        else
                        {
                            throw new PXException(Messages.APInvoicePOOrderCreation_NoApplicableLinesFound);
                        }                   
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (POOrder current in adapter.Get<POOrder>())
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

		protected virtual void AddPurchaseOrder(POOrderS aOrder)
		{

			PXSelectBase<POLineS> sel = new PXSelect<POLineS,
																Where<POLineS.orderType, Equal<Required<POLine.orderType>>,
																	And<POLineS.orderNbr, Equal<Required<POLine.orderNbr>>,
																	And<POLineS.cancelled, NotEqual<boolTrue>,
																	And<POLineS.completed, NotEqual<boolTrue>>>>>>(this);
			foreach (POLineS iLn in sel.Select(aOrder.OrderType, aOrder.OrderNbr))
			{
				if (aOrder.OrderType == POOrderType.StandardBlanket ||
					iLn.LineType != POLineType.Service)
					AddPOLine(iLn, aOrder.OrderType == POOrderType.Blanket);
			}
			POOrder order = this.Document.Current;
			if (string.IsNullOrEmpty(order.VendorRefNbr))
			{
				order.VendorRefNbr = aOrder.VendorRefNbr;
				order = this.Document.Update(order);
			}
		}

		protected virtual void AddPOLine(POLineS aLine, bool blanked)
		{
			POLine line = null;
			if (blanked)
				foreach (POLine iLn in this.Transactions.Select())
					if (iLn.POType == aLine.OrderType &
							iLn.PONbr == aLine.OrderNbr &&
							iLn.POLineNbr == aLine.LineNbr)
					{
						line = iLn;
						break;
					}

			if (line == null)
			{
				line = new POLine();

				line.BranchID = aLine.BranchID;
				line.InventoryID = aLine.InventoryID;
				line.SubItemID = aLine.SubItemID;
				line.SiteID = aLine.SiteID;
				line.TaxCategoryID = aLine.TaxCategoryID;
				line.TranDesc = aLine.TranDesc;
				line.UnitCost = aLine.UnitCost;
				line.UnitVolume = aLine.UnitVolume;
				line.UnitWeight = aLine.UnitWeight;
				line.UOM = aLine.UOM;
				line.AlternateID = aLine.AlternateID;
				line.CuryInfoID = aLine.CuryInfoID;
				line.CuryUnitCost = aLine.CuryUnitCost;
				line.ManualPrice = aLine.ManualPrice;
				line.ExpenseAcctID = aLine.ExpenseAcctID;
				line.ExpenseSubID = aLine.ExpenseSubID;
				line.RcptQtyMin = aLine.RcptQtyMin;
				line.RcptQtyMax = aLine.RcptQtyMax;
				line.RcptQtyThreshold = aLine.RcptQtyThreshold;
				line.RcptQtyAction = aLine.RcptQtyAction;
				line.POType = aLine.OrderType;
				line.PONbr = aLine.OrderNbr;
				line.POLineNbr = aLine.LineNbr;
				line.ProjectID = aLine.ProjectID;
				line.TaskID = aLine.TaskID;

			if (blanked)
			{
					line.OrderQty = aLine.LeftToReceiveQty;
			}
			else
			{
					line.OrderQty = aLine.OrderQty;
			}
				if (aLine.LineType == POLineType.Freight || aLine.LineType == POLineType.NonStock
					|| aLine.LineType == POLineType.Service || aLine.LineType == POLineType.MiscCharges)
			{
					line.CuryExtCost = (aLine.CuryExtCost - aLine.CuryReceivedCost);
					line.ExtCost = (aLine.ExtCost - aLine.ReceivedCost);
				}
				line.OpenQty = line.OrderQty;
				line  = (POLine) this.Transactions.Cache.Insert(line);
				if ( line.LineType != aLine.LineType && POLineType.IsDefault(line.LineType) )
				{
					line.LineType = aLine.LineType;
				}
			}
		}

        public PXAction<POOrder> recalculateDiscountsAction;
        [PXUIField(DisplayName = "Recalculate Prices", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXButton]
        public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
        {
            if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
            {
                DiscountEngine<POLine>.RecalculatePricesAndDiscounts<POOrderDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, DiscountDetails, Document.Current.VendorLocationID, Document.Current.OrderDate, recalcdiscountsfilter.Current);
            }
            return adapter.Get();
        }

        public PXAction<POOrder> recalcOk;
        [PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable RecalcOk(PXAdapter adapter)
        {
            return adapter.Get();
        }
        public virtual void CreatePrePaymentProc(APInvoiceEntry target)
        {
            APInvoice prepayment;

            if (target.Document.Current == null)
            {
                prepayment = target.Document.Insert(new APInvoice { DocType = APDocType.Prepayment });
            }
            else
            {
                prepayment = target.Document.Current;
            } 
            prepayment.DocDesc = Document.Current.OrderDesc;
            prepayment.VendorID = Document.Current.VendorID;
            target.Document.Update(prepayment);
	        prepayment.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
            prepayment.PONumber = Document.Current.OrderNbr;
            prepayment.InvoiceNbr = Document.Current.OrderNbr;
            prepayment.DueDate = Document.Current.OrderDate;
            prepayment.TaxZoneID = Document.Current.TaxZoneID;
            target.Document.Update(prepayment);

            TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(target.Transactions.Cache, null, TaxCalc.ManualCalc);

            foreach (POLine line in Transactions.Select())
            {
                APTran tran = new APTran();
                
                tran.InventoryID = line.InventoryID;
                tran.ProjectID = line.ProjectID;
                tran.TaskID = line.TaskID;
                tran.TranDesc = line.TranDesc;
                tran.UOM = line.UOM;
                tran.Qty = line.OrderQty;
                tran.CuryUnitCost = line.CuryUnitCost;
                tran.CuryLineAmt = line.CuryLineAmt;
                tran.CuryTranAmt = line.CuryExtCost;

                tran.POOrderType = Document.Current.OrderType;
                tran.PONbr = Document.Current.OrderNbr;

                target.Transactions.Insert(tran);
            }

            foreach (POTaxTran line in Taxes.Select())
            {
                APTaxTran tran = new APTaxTran();
                tran.CuryTaxableAmt = line.CuryTaxableAmt;
                tran.CuryTaxAmt = line.CuryTaxAmt;
                tran.TaxRate = line.TaxRate;
                tran.TaxID = line.TaxID;

                target.Taxes.Insert(tran);
            }
        }
        public PXAction<POOrder> createPrepayment;
        [PXUIField(DisplayName = "Create Prepayment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]     
        [PXButton(OnClosingPopup=PXSpecialButtonType.Cancel)]
        public virtual void CreatePrepayment()
        {
            if (Document.Current != null)
            {
                this.Save.Press();
                APInvoiceEntry target = PXGraph.CreateInstance<APInvoiceEntry>();
                if (Document.Current.PrepaymentRefNbr == null)
                {
                    target.Document.Insert(new APInvoice { DocType = APDocType.Prepayment });
                    CreatePrePaymentProc(target);

                    throw new PXPopupRedirectException(target, "New Prepayment", true);
                }
                else
                {
                    throw new InvalidOperationException("Prepayment already exists");
                }
            }
        }

		#endregion
		#region Internal Functions
		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<POOrder.orderDate, POOrder.curyID>(e.Row, e.OldRow))
			{
				if (poordercache.Current.StatusUpdated != true)
				{
					foreach (POLine tran in Transactions.Select())
					{
						if (Transactions.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
						{
							Transactions.Cache.SetStatus(tran, PXEntryStatus.Updated);
						}
					}
					poordercache.Current.StatusUpdated = true;
				}
			}
		}

		private object GetAcctSub<Field>(PXCache cache, object data) where Field : IBqlField
		{
			object NewValue = cache.GetValueExt<Field>(data);
			if (NewValue is PXFieldState)
			{
				return ((PXFieldState)NewValue).Value;
			}
			else
			{
				return NewValue;
			}
		}

		public static bool NeedsPOReceipt(POLine aLine, bool skipCompleted)
		{
			if (!(aLine.LineType == POLineType.GoodsForDropShip ||
					aLine.LineType == POLineType.NonStockForDropShip ||
					aLine.LineType == POLineType.NonStockForSalesOrder ||
					aLine.LineType == POLineType.GoodsForInventory ||
					aLine.LineType == POLineType.GoodsForSalesOrder ||
					aLine.LineType == POLineType.GoodsForReplenishment ||
					aLine.LineType == POLineType.NonStock ||
					aLine.LineType == POLineType.Freight))
			{
				return false;
			}
			if (skipCompleted && (aLine.Completed == true || aLine.Cancelled == true)) return false;
			return true;
		}

		public static bool NeedsAPInvoice(POLine aLine, bool skipCompleted)
		{
			if (!(aLine.LineType == POLineType.Service || aLine.LineType == POLineType.MiscCharges))
			{
				return false;
			}
			if (skipCompleted && (aLine.Completed == true || aLine.Cancelled == true)) return false;
			return true;
		}

		#endregion

		#region Events

		#region Internal Variables
		protected bool _ExceptionHandling = false;
		private bool _blockUIUpdate = false;

		#endregion

		#region POOrder
		public PXFilter<POOrderCache> poordercache;

        protected virtual void POOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            POOrder doc = e.Row as POOrder;

            if (doc == null)
            {
                return;
            }
            doc.RequestApproval = POSetup.Current.OrderRequestApproval;

            PXUIFieldAttribute.SetVisible<POOrder.hold>(cache, null, true);
            PXUIFieldAttribute.SetRequired<POOrder.termsID>(cache, true);
            bool isBlanket = doc.OrderType == POOrderType.Blanket;
            PXUIFieldAttribute.SetVisible<POOrder.expirationDate>(cache, null, isBlanket);
            PXUIFieldAttribute.SetVisible<POOrder.expectedDate>(cache, null, !isBlanket);

            bool curyenabled = !(vendor.Current != null && (bool)vendor.Current.AllowOverrideCury == false);

            this.createPOReceipt.SetEnabled((doc.OrderType == POOrderType.RegularOrder || doc.OrderType == POOrderType.DropShip)
                    && doc.Status == POOrderStatus.Open);
            this.createAPInvoice.SetEnabled((doc.OrderType == POOrderType.RegularOrder || doc.OrderType == POOrderType.DropShip)
                    && doc.Status == POOrderStatus.Open);

            PXUIFieldAttribute.SetEnabled(this.poLinesSelection.Cache, null, false);
            PXUIFieldAttribute.SetEnabled(this.openOrders.Cache, null, false);

            if (doc.Cancelled != true && doc.Status != POOrderStatus.Closed)
            {
                if (!this._blockUIUpdate)
                {
                    PXUIFieldAttribute.SetEnabled(cache, doc, true);
                    PXUIFieldAttribute.SetEnabled<POOrder.status>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyOrderTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyDiscTot>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyLineTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyTaxTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyID>(cache, doc, curyenabled);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyOpenLineTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyOpenOrderTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.openOrderQty>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.rQReqNbr>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyOpenTaxTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.shipToBAccountID>(cache, doc, (doc.ShipDestType != POShippingDestination.CompanyLocation && doc.ShipDestType != POShippingDestination.Site));
                    PXUIFieldAttribute.SetEnabled<POOrder.shipToLocationID>(cache, doc, (doc.ShipDestType != POShippingDestination.Site));
                    PXUIFieldAttribute.SetEnabled<POOrder.siteID>(cache, doc, (doc.ShipDestType == POShippingDestination.Site));
                    PXUIFieldAttribute.SetEnabled<POOrder.curyVatExemptTotal>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyVatTaxableTotal>(cache, doc, false);

                    PXUIFieldAttribute.SetRequired<POOrder.siteID>(cache, true);
                    PXUIFieldAttribute.SetRequired<POOrder.shipToBAccountID>(cache, (doc.ShipDestType != POShippingDestination.Site));
                    PXUIFieldAttribute.SetRequired<POOrder.shipToLocationID>(cache, (doc.ShipDestType != POShippingDestination.Site));
                    PXDefaultAttribute.SetPersistingCheck<POOrder.siteID>(cache, doc, (doc.ShipDestType == POShippingDestination.Site) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                    PXDefaultAttribute.SetPersistingCheck<POOrder.shipToLocationID>(cache, doc, (doc.ShipDestType != POShippingDestination.Site) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                    PXDefaultAttribute.SetPersistingCheck<POOrder.shipToBAccountID>(cache, doc, (doc.ShipDestType != POShippingDestination.Site) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

                    //calculate only on data entry, differences from the applications will be moved to RGOL upon closure
                    PXDBCurrencyAttribute.SetBaseCalc<POOrder.curyOrderTotal>(cache, null, true);
                    PXDBCurrencyAttribute.SetBaseCalc<POOrder.curyTaxTotal>(cache, null, true); //???
                    PXDBCurrencyAttribute.SetBaseCalc<POOrder.curyOpenTaxTotal>(cache, null, true); //???

                    PXUIFieldAttribute.SetEnabled<POOrder.hold>(cache, doc, true);
                    PXUIFieldAttribute.SetEnabled<POOrder.termsID>(cache, doc, true);
                    PXUIFieldAttribute.SetEnabled<POOrder.expectedDate>(cache, doc, true);
                    PXUIFieldAttribute.SetEnabled<POOrder.cancelled>(cache, doc, doc.Status == POOrderStatus.Open);

                    PXUIFieldAttribute.SetEnabled<POOrderS.selected>(this.openOrders.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<POLineS.selected>(this.poLinesSelection.Cache, null, true);
                }
            }

            if (POSetup.Current.OrderRequestApproval != true && doc.Approved == true)
            {
                PXUIFieldAttribute.SetVisible<POOrder.approved>(cache, doc, false);
                PXUIFieldAttribute.SetVisible<POOrder.receipt>(cache, doc, false);
            }
            else
            {
                PXUIFieldAttribute.SetVisible<POOrder.approved>(cache, doc, true);
                PXUIFieldAttribute.SetVisible<POOrder.receipt>(cache, doc, true);
                PXDefaultAttribute.SetPersistingCheck<POOrder.employeeID>(cache, doc, POSetup.Current.OrderRequestApproval != true && doc.Approved == true ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
            }

            PXUIFieldAttribute.SetEnabled<POOrder.orderType>(cache, doc);
            PXUIFieldAttribute.SetEnabled<POOrder.orderNbr>(cache, doc);

            DiscountDetails.Cache.AllowDelete = Transactions.Cache.AllowDelete;
            DiscountDetails.Cache.AllowUpdate = Transactions.Cache.AllowUpdate;
            DiscountDetails.Cache.AllowInsert = Transactions.Cache.AllowInsert;

            Remit_Address.Cache.AllowUpdate = !(doc.Printed == true || doc.Emailed == true);
            Remit_Contact.Cache.AllowUpdate = doc.Emailed == false;
            if (doc != null && vendor.Current != null && (bool)vendor.Current.TaxAgency == true)
            {
                doc.TaxZoneID = string.Empty;
                PXUIFieldAttribute.SetEnabled<POOrder.taxZoneID>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<POLine.taxCategoryID>(Transactions.Cache, null, false);
            }

            editVendor.SetEnabled(doc != null && vendor != null && vendor.Current != null);

            //purchaseOrder.SetEnabled(doc != null && doc.Status == POOrderStatus.Open);

            if (doc.VendorID != null && this._blockUIUpdate == false)
            {
                if (Transactions.Current != null || Transactions.Select().Count > 0)
                {
                    PXUIFieldAttribute.SetEnabled<POOrder.vendorID>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.vendorLocationID>(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyID>(cache, doc, false);
                }
                if (poordercache.Current.IsUsed == null)
                {
                    POLine usedLines = PXSelect<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                                        And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                            And<Where<POLine.receivedQty, Greater<decimal0>,
                                                Or<POLine.completed, Equal<boolTrue>,
                                                    Or<POLine.cancelled, Equal<boolTrue>>>>>>>>.Select(this, doc.OrderType, doc.OrderNbr);
                    poordercache.Current.IsUsed = (usedLines != null);
                }
                PXUIFieldAttribute.SetEnabled<POOrder.orderDate>(cache, doc, poordercache.Current.IsUsed != true);
            }
            if (!this._blockUIUpdate)
            {
                bool showControlTotal = POSetup.Current.GetRequireControlTotal(doc.OrderType);
                PXUIFieldAttribute.SetVisible<POOrder.curyControlTotal>(cache, e.Row, showControlTotal);
                PXUIFieldAttribute.SetEnabled<POOrder.curyDiscTot>(cache, doc, true);
                PXUIFieldAttribute.SetVisible<POOrder.shipToBAccountID>(cache, doc, (doc.ShipDestType != POShippingDestination.Site));
                PXUIFieldAttribute.SetVisible<POOrder.shipToLocationID>(cache, doc, (doc.ShipDestType != POShippingDestination.Site));
                PXUIFieldAttribute.SetVisible<POOrder.siteID>(cache, doc, (doc.ShipDestType == POShippingDestination.Site));
                PXUIFieldAttribute.SetEnabled<POOrder.sOOrderType>(cache, doc, doc.OrderType == POOrderType.DropShip);
                PXUIFieldAttribute.SetEnabled<POOrder.sOOrderNbr>(cache, doc, doc.OrderType == POOrderType.DropShip);
                POShipAddress shipAddress = this.Shipping_Address.Select();
                PORemitAddress remitAddress = this.Remit_Address.Select();
                bool enableAddressValidation = (doc.Cancelled == false)
                    && ((shipAddress != null && shipAddress.IsDefaultAddress == false && shipAddress.IsValidated == false)
                    || (remitAddress != null && remitAddress.IsDefaultAddress == false && remitAddress.IsValidated == false));
                this.validateAddresses.SetEnabled(enableAddressValidation);
            }

            if (IsExternalTax == true && doc.IsTaxValid != true)
            {
                PXUIFieldAttribute.SetWarning<POOrder.curyTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);
            }

            if (doc != null && doc.ShipDestType == POShippingDestination.Site)
            {
                var siteIdErrorString = PXUIFieldAttribute.GetError<POOrder.siteID>(cache, e.Row);
                if (siteIdErrorString == null)
                {
                    var siteIdErrorMessage = doc.SiteIdErrorMessage;
                    if(!string.IsNullOrWhiteSpace(siteIdErrorMessage))
                    {
                        cache.RaiseExceptionHandling<POOrder.siteID>(e.Row, cache.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(siteIdErrorMessage, PXErrorLevel.Error));
                    }
                }
            }
        }

		protected virtual void POOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;

            if (e.ExternalCall && (!sender.ObjectsEqual<POOrder.orderDate>(e.OldRow, e.Row) || !sender.ObjectsEqual<POOrder.vendorLocationID>(e.OldRow, e.Row)))
            {
                AR.DiscountEngine<POLine>.AutoRecalculatePricesAndDiscounts<POOrderDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, doc.VendorLocationID, doc.OrderDate);
            }
            if (e.ExternalCall && sender.GetStatus(doc) != PXEntryStatus.Deleted && !sender.ObjectsEqual<POOrder.curyDiscTot>(e.OldRow, e.Row))
            {
                DiscountEngine<POLine>.CalculateDocumentDiscountRate<POOrderDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, doc.CuryLineTotal ?? 0m, doc.CuryDiscTot ?? 0m);
            }
			if (doc.Cancelled != null && (bool)doc.Cancelled == false)
			{
				if (!POSetup.Current.GetRequireControlTotal(doc.OrderType))
				{
					if (doc.CuryOrderTotal != doc.CuryControlTotal)
					{
						if (doc.CuryOrderTotal != null && doc.CuryOrderTotal != 0)
							sender.SetValueExt<POOrder.curyControlTotal>(e.Row, doc.CuryOrderTotal);
						else
							sender.SetValueExt<POOrder.curyControlTotal>(e.Row, 0m);
					}
				}
			}
			if ((bool)doc.Hold == false && doc.Cancelled == false)
			{
				if (doc.CuryControlTotal != doc.CuryOrderTotal)
				{
					sender.RaiseExceptionHandling<POOrder.curyControlTotal>(e.Row, doc.CuryControlTotal, new PXSetPropertyException(Messages.DocumentOutOfBalance));
				}
				else
				{
					sender.RaiseExceptionHandling<POOrder.curyControlTotal>(e.Row, doc.CuryControlTotal, null);
				}

				if (doc.CuryLineTotal < Decimal.Zero && doc.Hold == false)
				{
					if (sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, doc.CuryLineTotal, new PXSetPropertyException(Messages.POOrderTotalAmountMustBeNonNegative, typeof(POOrder.curyLineTotal).Name)))
					{
						throw new PXRowPersistingException(typeof(POOrder.curyLineTotal).Name, null, Messages.POOrderTotalAmountMustBeNonNegative, typeof(POOrder.curyLineTotal).Name);
					}
				}
				else
				{
					sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, null, null);
				}
			}
			else 
			{
				sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, null, null);
			}

			if (this.Document.View.Answer == WebDialogResult.Yes && !sender.ObjectsEqual<POOrder.orderDate>(e.Row, e.OldRow))
			{
				foreach (POLine iLine in this.Transactions.Select())
				{
					if ((bool)iLine.Completed || (bool)iLine.Cancelled || (iLine.ReceivedQty > 0)) continue;
					this.Transactions.Cache.SetDefaultExt<POLine.requestedDate>(iLine);
				}
			}

			if (this.Document.View.Answer == WebDialogResult.Yes && !sender.ObjectsEqual<POOrder.expectedDate>(e.Row, e.OldRow))
			{
				foreach (POLine iLine in this.Transactions.Select())
				{
					if ((bool)iLine.Completed || (bool)iLine.Cancelled || (iLine.ReceivedQty > 0)) continue;
					POLine copy = PXCache<POLine>.CreateCopy(iLine);
					this.Transactions.Cache.SetDefaultExt<POLine.promisedDate>(copy);
					this.Transactions.Cache.Update(copy);
				}
			}

		}

		protected virtual void POOrder_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (e.Row != null)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Shipping_Address.Cache, Shipping_Contact.Cache))
				{
                    try
                    {
					POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
                    }
                    catch (SharedRecordMissingException)
                    {
                        sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
                    }
                    try
                    {
					POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
				}
                    catch (SharedRecordMissingException)
                    {
                        sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
                    }
                }
			}
		}

		protected virtual void POOrder_ShipDestType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row == null) return;
			
			if (row.ShipDestType == POShippingDestination.Site)
			{
				sender.SetDefaultExt<POOrder.siteID>(e.Row);
				sender.SetValueExt<POOrder.shipToBAccountID>(e.Row, null);
				sender.SetValueExt<POOrder.shipToLocationID>(e.Row, null);
			}
			else
			{
				sender.SetValueExt<POOrder.siteID>(e.Row, null);
				sender.SetDefaultExt<POOrder.shipToBAccountID>(e.Row);
				sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);
			}
		}

		protected virtual void POOrder_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;

			if (row != null && row.ShipDestType == POShippingDestination.Site)
			{
                string siteIdErrorMessage = string.Empty;

                try
                {
				POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
                }
                catch (SharedRecordMissingException)
                {
                    sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
                    sender.SetValueExt<POOrder.shipAddressID>(e.Row, null);
                    siteIdErrorMessage = Messages.ShippingAddressMayNotBeEmpty;
                }
                try
                {
				POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
			}
                catch (SharedRecordMissingException)
                {
                    sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
                    sender.SetValueExt<POOrder.shipContactID>(e.Row, null);
                    siteIdErrorMessage = Messages.ShippingContactMayNotBeEmpty;
                }

                sender.SetValueExt<POOrder.siteIdErrorMessage>(e.Row, siteIdErrorMessage);
            }
		}

		protected virtual void POOrder_ShipToBAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);
			}
		}

		protected virtual void POOrder_ShipToLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
                try
                {
				POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
                }
                catch (SharedRecordMissingException)
                {
                    sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
                }
                try
                {
				POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
			}
                catch (SharedRecordMissingException)
                {
                    sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
                }
            }
		}

		protected virtual void POOrder_ShipToLocationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null && row.ShipDestType==POShippingDestination.Site)
			{
				e.Cancel = true;
				e.NewValue = null;
			}
		}

		protected virtual void POOrder_ShipToBAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row == null) return;

			if (row.ShipDestType == POShippingDestination.Site)
			{
				e.Cancel = true;
				e.NewValue = null;
			}
			else if (row.OrderType == POOrderType.DropShip)
			{
				SOLineSplit demand = PXSelectJoin<SOLineSplit,
					InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOLineSplit.orderType>, And<SOOrder.orderNbr, Equal<SOLineSplit.orderNbr>>>>,
					Where<SOLineSplit.pOType, Equal<Current<POOrder.orderType>>,					
					And<SOLineSplit.pONbr, Equal<Current<POOrder.orderNbr>>,
					And<SOOrder.customerID, NotEqual<Required<SOOrder.customerID>>>>>>.Select(this, e.NewValue);

				if (demand != null)
				{
					Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, e.NewValue);
					var ex = new PXSetPropertyException(Messages.CustomerCannotBeChangedForOrderWithActiveDemand);
					ex.ErrorValue = customer?.AcctCD;

					throw ex;
				}
		}
		}

		protected virtual void POOrder_EmployeeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null && (this.POSetup.Current.UpdateSubOnOwnerChange ?? false))
			{
				foreach (POLine iLn in this.Transactions.Select())
				{
					if (iLn.LineType == POLineType.NonStock || iLn.LineType == POLineType.Service)
					{
						this.Transactions.Cache.SetDefaultExt<POLine.expenseSubID>(iLn);
					}
				}
			}
		}

		protected virtual void POOrder_Status_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			if (((POOrder)e.Row).Status == POOrderStatus.Open && (string)e.OldValue != POOrderStatus.Open)
			{
				CheckOpenStatus((POOrder)e.Row);
			}
		}
        protected void UpdatePrepayment(POOrder order)
        {
            if (order != null && order.PrepaymentDocType != null && order.PrepaymentRefNbr != null)
            {
                APInvoiceEntry target = PXGraph.CreateInstance<APInvoiceEntry>();
                target.Document.Current = PXSelect<APInvoice, Where<APInvoice.refNbr, Equal<Required<POOrder.prepaymentRefNbr>>, And<APInvoice.docType, Equal<Required<POOrder.prepaymentDocType>>>>>.Select(target, order.PrepaymentRefNbr, order.PrepaymentDocType);
                          
                if (target.Document.Current != null && target.Document.Current.ReleasedOrPrebooked == false && target.Document.Current.Scheduled == false && target.Document.Current.Voided == false)
                {
                    target.Document.Current.Hold = true;
                    target.Document.Current.DocDesc = Document.Current.OrderDesc;
                    target.Document.Current.DueDate = Document.Current.OrderDate;
                    target.Document.Current.TaxZoneID = Document.Current.TaxZoneID;
                    target.Document.Current.CuryTaxTotal = Document.Current.CuryTaxTotal;
                    target.Document.Current.CuryVatExemptTotal = Document.Current.CuryVatExemptTotal;
                    target.Document.Current.CuryVatTaxableTotal = Document.Current.CuryVatTaxableTotal;
                    target.Document.Update(target.Document.Current);
                    foreach (APTran tran in target.Transactions.Select())
                    {
                        if (tran.PONbr == order.OrderNbr)
                        {
                            target.Transactions.Delete(tran);
                        }
                    }

                    bool exactmatch = target.Transactions.Select().Count == 0;

                    foreach (POLine line in Transactions.Select())
                    {
                        APTran tran = new APTran();

                        tran = target.Transactions.Insert(tran);
                        tran.InventoryID = line.InventoryID;
                        tran.ProjectID = line.ProjectID;
                        tran.TaskID = line.TaskID;
                        tran.TranDesc = line.TranDesc;
                        tran.UOM = line.UOM;
                        tran.Qty = line.OrderQty;
                        tran.CuryUnitCost = line.CuryUnitCost;
                        tran.CuryLineAmt = line.CuryLineAmt;
                        tran.CuryTranAmt = line.CuryExtCost;
                        tran.POOrderType = order.OrderType;
                        tran.PONbr = order.OrderNbr;

                        target.Transactions.Insert(tran);
                    }

                    if (exactmatch)
                    {
                        target.Taxes.Select().ForEach(a => { target.Taxes.Delete(a); });

                        foreach (POTaxTran line in Taxes.Select())
                        {
                            APTaxTran tran = new APTaxTran();
                            tran.CuryTaxableAmt = line.CuryTaxableAmt;
                            tran.CuryTaxAmt = line.CuryTaxAmt;
                            tran.TaxRate = line.TaxRate;
                            tran.TaxID = line.TaxID;

                            target.Taxes.Insert(tran);
                        } 
                    }

                    target.Save.Press();
                }
            }
        }

		protected virtual void POOrder_Cancelled_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null && (bool)e.NewValue == true)
			{
				POReceipt receipt = PXSelectJoin<POReceipt,
										InnerJoin<POOrderReceipt, On<POOrderReceipt.receiptNbr, Equal<POReceipt.receiptNbr>>>,
										Where<POOrderReceipt.pOType, Equal<Required<POReceiptLine.pOType>>,
											And<POOrderReceipt.pONbr, Equal<Required<POReceiptLine.pONbr>>,
											And<POReceipt.released, Equal<False>>>>>.Select(this, row.OrderType, row.OrderNbr);
				if (receipt != null)
				{
					throw new PXException(Messages.POOrderHasUnreleaseReceiptsAndCantBeCancelled);
				}
			}
		}

		protected virtual void POOrder_Cancelled_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (doc.Cancelled == true)
			{
				doc.Status = POOrderStatus.Cancelled;
			}
			else
			{
				doc.Status = doc.Hold == true ? POOrderStatus.Hold : POOrderStatus.Open;
				PXUIFieldAttribute.SetEnabled<POOrder.hold>(cache, e.Row, true);
			}

			foreach (POLine line in this.Transactions.Select())
			{
				bool isCompletedAndLinkedToSO = (line.Completed == true || line.Cancelled == true) && IsLinkedToSO(line);

				bool? newState = null;
				if (doc.Status == POOrderStatus.Cancelled && line.Completed != true)
				{
					newState = true;
				}
				if (doc.Status != POOrderStatus.Cancelled && line.Cancelled == true)
				{
					if (isCompletedAndLinkedToSO)
						newState = null;
					else
					newState = false;
				}
				if (newState != null)
				{
					POLine upd = (POLine)this.Transactions.Cache.CreateCopy(line);
					upd.Cancelled = newState;
					upd.Completed = newState;
					this.Transactions.Update(upd);
				}
			}
			CheckOpenStatus(doc);
		}
		protected virtual void POOrder_Hold_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			if ((bool?)e.NewValue != true) return;
			POOrderReceipt receiptOpen =
			PXSelectJoin<POOrderReceipt,
				InnerJoin<POReceipt,
					On<POReceipt.receiptNbr, Equal<POOrderReceipt.receiptNbr>,
						And<POReceipt.released, Equal<boolFalse>>>>,
				Where<POOrderReceipt.pOType, Equal<Current<POOrder.orderType>>,
					And<POOrderReceipt.pONbr, Equal<Current<POOrder.orderNbr>>>>>.SelectSingleBound(this, new object[] { e.Row });
			if (receiptOpen != null)
				throw new PXSetPropertyException(Messages.PurchaseOrderOnHoldWithReceipt, receiptOpen.POType, receiptOpen.PONbr, receiptOpen.ReceiptNbr);

		}
		protected virtual void POOrder_Hold_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (doc.Hold == true)
			{
				doc.Status = POOrderStatus.Hold;

				cache.SetDefaultExt<POOrder.printed>(doc);
				cache.SetDefaultExt<POOrder.emailed>(doc);

				if (doc.Cancelled == true)
					cache.SetValueExt<POOrder.cancelled>(doc, false);
			}
		}

		protected virtual void POOrder_Printed_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (doc.Status == POOrderStatus.PendingPrint && doc.Printed == true)
			{
				doc.Status = POOrderStatus.Hold;
			}
		}

		protected virtual void POOrder_Emailed_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (doc.Status == POOrderStatus.PendingEmail && doc.Emailed == true)
				doc.Status = POOrderStatus.Hold;
		}

		protected virtual void POOrder_DontPrint_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			doc.Status = POOrderStatus.Hold;
		}

		protected virtual void POOrder_DontEmail_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			doc.Status = POOrderStatus.Hold;
		}

		protected virtual void POOrder_OrderType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = POOrderType.RegularOrder;
		}
		protected virtual void POOrder_Approved_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = POSetup.Current != null ? POSetup.Current.OrderRequestApproval != true : true;
		}

		protected virtual void POOrder_DontPrint_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				e.NewValue = location.Current != null ? location.Current.VPrintOrder != true : true;
			}
		}

		protected virtual void POOrder_DontEmail_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				e.NewValue = location.Current != null ? location.Current.VEmailOrder != true : true;
			}
		}


		protected virtual void POOrder_ExpectedDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			Location vendorLocation = this.location.Current;
			if (row != null && row.OrderDate.HasValue)
			{
				int offset = (vendorLocation != null ? (int)(vendorLocation.VLeadTime ?? 0) : 0);
				e.NewValue = row.OrderDate.Value.AddDays(offset);
			}
		}


		protected virtual void POOrder_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Location current = (Location)this.location.Current;
			POOrder row = (POOrder)e.Row;
			if (current == null || (current.BAccountID != row.VendorID || current.LocationID != row.VendorLocationID))
			{
				current = this.location.Select();
				this.location.Current = current;
			}

			sender.SetDefaultExt<POOrder.branchID>(e.Row);
			sender.SetDefaultExt<POOrder.taxZoneID>(e.Row);
			sender.SetDefaultExt<POOrder.shipVia>(e.Row);
			sender.SetDefaultExt<POOrder.fOBPoint>(e.Row);
			sender.SetDefaultExt<POOrder.expectedDate>(e.Row);

			sender.SetDefaultExt<POOrder.approved>(e.Row);
			sender.SetDefaultExt<POOrder.dontPrint>(e.Row);
			sender.SetDefaultExt<POOrder.dontEmail>(e.Row);
			sender.SetDefaultExt<POOrder.printed>(e.Row);
			sender.SetDefaultExt<POOrder.emailed>(e.Row);

			sender.SetDefaultExt<POOrder.shipDestType>(e.Row);
			sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);

			PORemitAddressAttribute.DefaultRecord<POOrder.remitAddressID>(sender, e.Row);
			PORemitContactAttribute.DefaultRecord<POOrder.remitContactID>(sender, e.Row);
           
		}

		protected virtual void POOrder_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<POOrder.curyID>(e.Row) == null)
				{
				    CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<POOrder.curyInfoID>(sender, e.Row);
    
				    string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				    if (string.IsNullOrEmpty(message) == false)
				    {
					    sender.RaiseExceptionHandling<POOrder.orderDate>(e.Row, ((POOrder)e.Row).OrderDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				    }
    
				    if (info != null)
				    {
					    ((POOrder)e.Row).CuryID = info.CuryID;
				    }
			    }
			}

			sender.SetDefaultExt<POOrder.vendorLocationID>(e.Row);
			sender.SetDefaultExt<POOrder.termsID>(e.Row);

            object VendorRefNbr = ((POOrder)e.Row).VendorRefNbr;
			sender.RaiseFieldVerifying<POOrder.vendorRefNbr>(e.Row, ref VendorRefNbr);
		}

		protected virtual void POOrder_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (PXSelectGroupBy<POOrderReceipt,
				Where<POOrderReceipt.pONbr, Equal<Required<POOrderReceipt.pONbr>>>,
				Aggregate<Count>>.Select(this, doc.OrderNbr).RowCount > 0)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderHasReceiptsAndCannotBeDeleted);
			}

			Transactions.View.SetAnswer("POLineLinkedToSOLine", WebDialogResult.OK);
        }

		protected virtual void POOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;

			if (string.IsNullOrEmpty(doc.TermsID))
			{
				if (sender.RaiseExceptionHandling<POOrder.termsID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(POOrder.termsID).Name)))
				{
					throw new PXRowPersistingException(typeof(POOrder.termsID).Name, null, ErrorMessages.FieldIsEmpty, typeof(POOrder.termsID).Name);
				}
			}

			if (doc.CuryLineTotal < Decimal.Zero && doc.Hold == false)
			{
				if (sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, doc.CuryLineTotal, new PXSetPropertyException(Messages.POOrderTotalAmountMustBeNonNegative, typeof(POOrder.curyLineTotal).Name)))
				{
					throw new PXRowPersistingException(typeof(POOrder.curyLineTotal).Name, null, Messages.POOrderTotalAmountMustBeNonNegative, typeof(POOrder.curyLineTotal).Name);
				}
			}

            if (doc.CuryDiscTot > Math.Abs(doc.CuryLineTotal ?? 0m))
            {
                if (sender.RaiseExceptionHandling<POOrder.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(Messages.DiscountGreaterLineTotal, PXErrorLevel.Error)))
                {
                    throw new PXRowPersistingException(typeof(POOrder.curyDiscTot).Name, null, Messages.DiscountGreaterLineTotal);
                }
            }
		}
		protected virtual void POOrder_OrderDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (this.Transactions.Select().Count > 0)
			{
				this.Document.Ask(Messages.Warning, Messages.POOrderOrderDateChangeConfirmation, MessageButtons.YesNo, MessageIcon.Question);
			}
		}

		protected virtual void POOrder_OrderDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CurrencyInfoAttribute.SetEffectiveDate<POOrder.orderDate>(sender, e);

			sender.SetDefaultExt<POOrder.expectedDate>(e.Row);
		}
		/*
		protected virtual void PORemitAddress_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			if(this.Document.Current != null && this.Document.Current.Hold == false)	
				PXUIFieldAttribute.SetEnabled(cache, e.Row, false);
		}
		
		protected virtual void PORemitContact_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			if (this.Document.Current != null && this.Document.Current.Hold == false)
				PXUIFieldAttribute.SetEnabled(cache, e.Row, false);
		}

		protected virtual void POShipAddress_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			if (this.Document.Current != null && this.Document.Current.Hold == false)
				PXUIFieldAttribute.SetEnabled(cache, e.Row, false);
		}
		
		protected virtual void POShipContact_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			if (this.Document.Current != null && this.Document.Current.Hold == false)
				PXUIFieldAttribute.SetEnabled(cache, e.Row, false);
		}		
		*/

		protected virtual void POOrder_ExpectedDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (this.Transactions.Select().Count > 0)
			{
				this.Document.Ask(Messages.Warning, Messages.POOrderPromisedDateChangeConfirmation, MessageButtons.YesNo, MessageIcon.Question);
			}
		}

		protected virtual bool CheckOpenStatus(POOrder document)
		{
			if (document.Status == POOrderStatus.Open)
			{
				POLine line =
				PXSelect<POLine, Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
					And<POLine.completed, Equal<Required<POLine.completed>>>>>.SelectWindowed(this, 0, 1, document.OrderNbr, false);
				string newStatus = line != null ? POOrderStatus.Open : POOrderStatus.Closed;
				if (line == null)
				{
					POLine cancelled =
						PXSelect<POLine,
						Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
							And<POLine.cancelled, Equal<Required<POLine.cancelled>>>>>.SelectWindowed(this, 0, 1, document.OrderNbr, true);
					if (cancelled == null)
						document.Receipt = true;
				}
				else
					document.Receipt = false;

				if (newStatus != document.Status)
				{
					document.Status = newStatus;
					return true;
				}
			}
			return false;
        }

        [PXDBString(10, IsUnicode = true)]
        [PXDBDefault(typeof(POOrder.taxZoneID))]
        [PXUIFieldAttribute(DisplayName = "Vendor Tax Zone", Enabled = false)]
        protected virtual void POTaxTran_TaxZoneID_CacheAttached(PXCache sender)
        {
        }

        protected virtual void POTaxTran_TaxZoneID_ExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs e)
        {
            Exception ex = e.Exception as PXSetPropertyException;
            if (ex != null)
            {
                Document.Cache.RaiseExceptionHandling<POOrder.taxZoneID>(Document.Current, null, ex);
            }
        }

		protected virtual void POTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<POTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}
		#endregion

		#region POLine Events
		protected virtual void POLine_OrderQty_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null)
			{
				e.NewValue = row.LineType == POLineType.Freight ? Decimal.One : Decimal.Zero;
			}
		}

		protected object GetValue<Field>(object data)
			where Field : IBqlField
		{
			if (data == null) return null;
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

        protected virtual void POLine_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POLine row = e.Row as POLine;
            if (row != null)
            {
                if (row.OrderQty == 0)
                {
                    sender.SetValueExt<POLine.curyDiscAmt>(row, decimal.Zero);
                    sender.SetValueExt<POLine.discPct>(row, decimal.Zero);
                }
                else
                {
                    sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
                }
            }
        }

		protected virtual void POLine_ExpenseAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;
			var item = (PXResult<InventoryItem, INPostClass, INSite>)
							PXSelectJoin<InventoryItem,
								LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>,
								LeftJoin<INSite, On<INSite.siteID, Equal<Required<POLine.siteID>>>>>,
							Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
							.Select(this, row.SiteID, row.InventoryID);

			Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>
				.Select(this, location.Current.VCarrierID);

			switch (row.LineType)
			{
				case POLineType.Description:
					e.Cancel = true;
					break;
				case POLineType.Freight:
					e.NewValue = GetValue<Carrier.freightExpenseAcctID>(carrier) ?? POSetup.Current.FreightExpenseAcctID;
					e.Cancel = true;
					break;
				default:
					if (item != null)
					{
						//
						if (((INPostClass)item).PostClassID != null && POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType))
						{
							try
							{
								e.NewValue = INReleaseProcess.GetAcctID<INPostClass.cOGSAcctID>(this, ((INPostClass)item).COGSAcctDefault, item, item, item);
							}
							catch (PXMaskArgumentException)
							{
							}
						}
						else if (POLineType.IsNonStock(row.LineType))
						{
							e.NewValue = ((InventoryItem)item).COGSAcctID ?? location.Current.VExpenseAcctID;
						}
						//
					}
					if (e.NewValue != null)
						e.Cancel = true;
					break;
			}
		}

		protected virtual void POLine_ExpenseAcctID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.projectID>(e.Row);
		}

		protected virtual void POLine_ExpenseSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;
			var item = (PXResult<InventoryItem, INPostClass, INSite>)
							PXSelectJoin<InventoryItem,
								LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>,
								LeftJoin<INSite, On<INSite.siteID, Equal<Required<POLine.siteID>>>>>,
							Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
							.Select(this, row.SiteID, row.InventoryID);

			Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>
				.Select(this, location.Current.VCarrierID);

			EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>
				.Select(this, this.Document.Current.EmployeeID);

			CRLocation companyloc = PXSelectJoin<CRLocation,
				InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>,
							And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>,
				InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>,
				Where<Branch.branchID, Equal<Required<POLine.branchID>>>>.Select(this, row.BranchID);

			switch (row.LineType)
			{
				case POLineType.Description:

					break;
				case POLineType.Freight:
					e.NewValue = GetValue<Carrier.freightExpenseSubID>(carrier) ?? POSetup.Current.FreightExpenseSubID;
					e.Cancel = true;
					break;
				default:
					if (item != null)
					{
						//
						if (((INPostClass)item).PostClassID != null && POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType))
						{
							try
							{
								e.NewValue = INReleaseProcess.GetSubID<INPostClass.cOGSSubID>(this, ((INPostClass)item).COGSAcctDefault, ((INPostClass)item).COGSSubMask, item, item, item);
							}
							catch (PXMaskArgumentException)
							{
							}
						}
						else
						{
							e.NewValue = null;
						}

						if (POLineType.IsNonStock(row.LineType))
						{
							int? projectID = row.ProjectID ?? PM.ProjectDefaultAttribute.NonProject(this);
							PM.PMProject project = PXSelect<PM.PMProject, Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>.Select(this, projectID);
							int? projectTask_SubID = null;
							PM.PMTask task = PXSelect<PM.PMTask, Where<PM.PMTask.taskID, Equal<Required<PM.PMTask.taskID>>>>.Select(this, row.TaskID);
							if (task != null)
								projectTask_SubID = task.DefaultSubID;
						
							object subCD =
								AP.SubAccountMaskAttribute.MakeSub<APSetup.expenseSubMask>(this, apsetup.Current.ExpenseSubMask,
																				new[]
								                                                           	{
								                                                           		GetValue<Location.vExpenseSubID>(location.Current),
								                                                           		e.NewValue ?? GetValue<InventoryItem.cOGSSubID>((InventoryItem)item),
								                                                           		GetValue<EPEmployee.expenseSubID>(employee),
								                                                           		GetValue<CRLocation.cMPExpenseSubID>(companyloc),
																								project.DefaultSubID,
																								projectTask_SubID
								                                                           	},
																				new[]
								                                                           	{
								                                                           		typeof (Location.vExpenseSubID),
								                                                           		typeof (InventoryItem.cOGSSubID),
								                                                           		typeof (EPEmployee.expenseSubID),
								                                                           		typeof (CRLocation.cMPExpenseSubID),
																								typeof(PM.PMProject.defaultSubID),
																								typeof(PM.PMTask.defaultSubID)
								                                                           	}
									);
							sender.RaiseFieldUpdating<POReceiptLine.expenseSubID>(e.Row, ref subCD);
							e.NewValue = subCD;
						}
						else
						{
							e.NewValue = null;
						}
					}
					e.Cancel = true;
					break;
			}

		}

		protected virtual void POLine_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (TaxAttribute.GetTaxCalc<POLine.taxCategoryID>(sender, e.Row) == TaxCalc.Calc && taxzone.Current != null && !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) && ((POLine)e.Row).InventoryID == null)
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
			}
			if (vendor != null && vendor.Current != null && (bool)vendor.Current.TaxAgency == true)
			{
				((POLine)e.Row).TaxCategoryID = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void POLine_UnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (((POLine)e.Row).InventoryID == null)
			{
				e.NewValue = 0m;
			}
		}

		/*When PO line is entered the system should substitute the Inventory unit price
		based on following logic:
		   1: Look for Inventory ID/Subitem ID in vendor price list. If found:
			  a) If the PO currency and default PO units are the same as in vendor
				 price list set the the unit cost as in vendor price list. 
		  		 The search is done strictly by the Currency And UOM of the specific row;
				 If the Date provided is less then LastEffectiveDate the LastEffectivePrice is taken,
				 else -  EffectivePrice.				
				 
		   2: If no records found for the inventory item use last received cost form
		Inventory Item to receive the base currency price. ID PO currency is different
		from the base currency convert the amount to foreign currency using rate. Last
		received price in PO is always specified in Base units. Convert the price to
		the units specified in PO order. */
        protected virtual void POLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (!this.skipCostDefaulting)
            {
                POLine row = e.Row as POLine;
				if (row != null && row.InventoryID.HasValue)
                    {
                        POOrder doc = this.Document.Current;
                        if (doc != null && doc.VendorID != null)
                        {
						if (row.ManualPrice != true || row.CuryUnitCost == null)
						{
                            decimal? vendorUnitCost = null;
							if (row.InventoryID != null && row.UOM != null)
                            {
                                DateTime date = Document.Current.OrderDate.Value;
                                CurrencyInfo curyInfo = this.currencyinfo.Search<CurrencyInfo.curyInfoID>(doc.CuryInfoID);
                                vendorUnitCost = APVendorPriceMaint.CalculateUnitCost(sender, row.VendorID, doc.VendorLocationID, row.InventoryID, curyInfo, row.UOM, row.OrderQty, date, row.CuryUnitCost);
                                e.NewValue = vendorUnitCost;
                            }
                            if (vendorUnitCost == null)
                                e.NewValue = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(sender.Graph, row,
                                    doc.VendorID, doc.VendorLocationID, doc.OrderDate, doc.CuryID, row.InventoryID, row.SubItemID, row.SiteID, row.UOM);
                        }
						else
							e.NewValue = row.CuryUnitCost ?? 0m;
                    }
                }
            }
        }
		
		protected virtual void POLine_CuryUnitCost_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			Decimal? value = (Decimal?)e.NewValue;
			if ( value.HasValue && value < Decimal.Zero && 
				(row.LineType != POLineType.NonStock 
				&& row.LineType != POLineType.Service
				&& row.LineType != POLineType.MiscCharges
				&& row.LineType != POLineType.Freight
				&& row.LineType != POLineType.NonStockForDropShip))
			{
				throw new PXSetPropertyException(Messages.UnitCostMustBeNonNegativeForStockItem);
			}
		}
		protected virtual void POLine_PromisedDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null)
			{
				if (row.InventoryID != null)
				{
					InventoryItem item = PXSelect<InventoryItem,
						Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>.
						Select(this, row.InventoryID);

					POOrder order = this.Document.Current;

					PXResult<CRLocation, POVendorInventory> r =
						(PXResult<CRLocation, POVendorInventory>)
						PXSelectJoin<CRLocation,
						LeftJoin<POVendorInventory,
									On<POVendorInventory.inventoryID, Equal<Required<POLine.inventoryID>>,
						     And<POVendorInventory.subItemID, Equal<Required<POLine.subItemID>>,
								 And<POVendorInventory.vendorID, Equal<CRLocation.bAccountID>,
								 And<Where<POVendorInventory.vendorLocationID, Equal<CRLocation.locationID>,
								         Or<POVendorInventory.vendorLocationID, IsNull>>>>>>>,
						Where<CRLocation.bAccountID, Equal<Required<POLine.vendorID>>,
							And<CRLocation.locationID, Equal<Required<POLine.vendorLocationID>>>>,
						OrderBy<Desc<POVendorInventory.vendorLocationID, 
										 Asc<CRLocation.locationID>>>>
							.SelectWindowed(this, 0, 1, row.InventoryID, row.SubItemID, order.VendorID, order.VendorLocationID);
					if (r == null) return;

					CRLocation location = r;
					POVendorInventory vendorCatalogue = r;

					if (order.ExpectedDate == null)
					{
						e.NewValue =
							order.OrderDate.Value.AddDays(
							location.VLeadTime.GetValueOrDefault() + vendorCatalogue.AddLeadTimeDays.GetValueOrDefault());
					}

					else
					{
						e.NewValue =
							order.ExpectedDate.Value.AddDays(
							vendorCatalogue.AddLeadTimeDays.GetValueOrDefault());
					}
				}
			}
		}

		protected virtual void POLine_LineType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.orderQty>(e.Row);
			sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
			sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
		}
	
		protected virtual void POLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
			sender.SetValue<POLine.unitCost>(e.Row, null);
		}

		private bool skipCostDefaulting = false;


		protected virtual void POLine_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;
			if (row.InventoryID == null) return;


			SOLineSplit link = PXSelect<SOLineSplit, Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>, And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>, And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>.SelectWindowed(this, 0, 1, row.OrderType, row.OrderNbr, row.LineNbr);
			if (link != null && link.InventoryID != (int?) e.NewValue)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, e.NewValue);

				var ex = new PXSetPropertyException<POLine.inventoryID>(Messages.ChangingInventoryForLinkedRecord);
				ex.ErrorValue = item?.InventoryCD;

				throw ex;
			}
		}

		protected virtual void POLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			this.skipCostDefaulting = true;
			sender.SetDefaultExt<POLine.vendorID>(e.Row);
			sender.SetDefaultExt<POLine.subItemID>(e.Row);
			sender.SetDefaultExt<POLine.siteID>(e.Row);
			sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
			sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
			sender.SetDefaultExt<POLine.taxCategoryID>(e.Row);
			sender.SetDefaultExt<POLine.uOM>(e.Row);
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			this.skipCostDefaulting = false;
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
			sender.SetValue<POLine.unitCost>(e.Row, null);
			sender.SetDefaultExt<POLine.siteID>(e.Row);
            sender.SetDefaultExt<POLine.unitWeight>(e.Row);
            sender.SetDefaultExt<POLine.unitVolume>(e.Row);


			POLine tran = e.Row as POLine;
			IN.InventoryItem item = PXSelectorAttribute.Select<IN.InventoryItem.inventoryID>(sender, tran) as IN.InventoryItem;
			if (item != null && tran != null)
			{
				tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, "Descr", vendor.Current?.LocaleName);
			}
		}

		protected virtual void POLine_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;

			sender.SetDefaultExt<POLine.expenseSubID>(row);
		}

		protected virtual void POLine_TaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;

			sender.SetDefaultExt<POLine.expenseSubID>(row);
		}

		protected virtual void POLine_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;
			if (!(e.NewValue is Int32)) return;

			if (e.NewValue != null && POLineType.IsStock(row.LineType) && row.SiteID != null)
			{
				HashSet<int> validWarehouses = new HashSet<int>();
				HashSet<int> explicitWarehouses = new HashSet<int>();

				PXResultset<INLocation> projectLocations = PXSelect<INLocation,
					Where<INLocation.projectID, Equal<Required<INLocation.projectID>>,
					And2<Where<INLocation.taskID, IsNull>, Or<INLocation.taskID, Equal<Required<INLocation.taskID>>>>>>.Select(this, row.ProjectID, e.NewValue);

				foreach (INLocation taskLocation in projectLocations)
				{
					validWarehouses.Add(taskLocation.SiteID.Value);

					if (taskLocation.TaskID == (int?)e.NewValue)
					{
						explicitWarehouses.Add(taskLocation.SiteID.Value);
					}
				}

				if (explicitWarehouses.Count > 0 && !explicitWarehouses.Contains(row.SiteID.Value) ||
					!validWarehouses.Contains(row.SiteID.Value))
				{
					string taskCD = null;
					PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.TaskID);
					if (task != null)
					{
						taskCD = task.TaskCD; // For Error.
					}

					if (explicitWarehouses.Count > 0)
					{
						INSite warehouse = PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, explicitWarehouses.First());
						sender.RaiseExceptionHandling<POLine.taskID>(row, taskCD, new PXSetPropertyException(Messages.ProjectTaskIsAssociatedWithAnotherWarehouse, PXErrorLevel.Warning, warehouse.SiteCD));
					}
					else
					{
						sender.RaiseExceptionHandling<POLine.taskID>(row, taskCD, new PXSetPropertyException(Messages.ProjectTaskIsNotAssociatedWithAnyInLocation, PXErrorLevel.Warning));
					}
				}

			}

		}

		protected virtual void POLine_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null &&
				 (row.LineType == POLineType.GoodsForInventory ||
					row.LineType == POLineType.GoodsForSalesOrder ||
					row.LineType == POLineType.NonStock ||
					row.LineType == POLineType.Service ||
					row.LineType == POLineType.GoodsForReplenishment ||
					row.LineType == POLineType.GoodsForDropShip)
					&& row.SiteID != null)
			{
				sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
				sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
				sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			}
		}

		protected virtual void POLine_SubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.uOM>(e.Row);
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
		}

        protected virtual void POLine_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POLine row = e.Row as POLine;
            if (row != null && e.ExternalCall)
            {
                DiscountEngine<POLine>.UpdateManualLineDiscount<POOrderDiscountDetail>(sender, Transactions, row, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate);
            }
        }

		protected virtual void POLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			POOrder doc = this.Document.Current;
			if (row == null) return;

			if (IsExport) return;//for performance 

			bool isLinkedToSO = row.Completed == true && IsLinkedToSO(row);

			if (this.Document.Current.Hold != true || isLinkedToSO)
			{
				PXUIFieldAttribute.SetEnabled(sender, e.Row, false);

				if ((this.Document.Current.Status == POOrderStatus.Balanced ||
						this.Document.Current.Status == POOrderStatus.Open) && !isLinkedToSO)
				{
					PXUIFieldAttribute.SetEnabled<POLine.promisedDate>(sender, e.Row, true);
					PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row,
																			row.ReceivedQty == 0 ||
																			row.ReceivedQty < row.OrderQty * row.RcptQtyThreshold / 100);

					PXUIFieldAttribute.SetEnabled<POLine.completed>(sender, e.Row,
																			row.ReceivedQty == 0 ||
																			row.ReceivedQty >= row.OrderQty * row.RcptQtyThreshold / 100);
				}

				this.Transactions.Cache.RaiseExceptionHandling<POLine.lineType>(row, row.LineType, new PXSetPropertyException((this.Document.Current.Status == POOrderStatus.Hold && isLinkedToSO) ? Messages.POLineLinkedToSO : string.Empty, PXErrorLevel.RowWarning));
			}
			else
			{
				if (!this._blockUIUpdate)
				{
					bool isNonStockKit = (!row.IsStockItem ?? false) && (row.IsKit ?? false);
					PXUIFieldAttribute.SetEnabled(sender, e.Row, true);
					PXUIFieldAttribute.SetEnabled<POLine.completed>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.receivedQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyReceivedCost>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POLine.curyExtCost>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.openQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.discountSequenceID>(sender, e.Row, false);
					switch (row.LineType)
					{
						case POLineType.Description:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.branchID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, e.Row, true);
							break;

						case POLineType.Freight:
						case POLineType.MiscCharges:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.branchID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.taxCategoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.curyLineAmt>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.projectID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.taskID>(sender, e.Row, true);
							break;

						case POLineType.NonStock:
						case POLineType.Service:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.siteID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.subItemID>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, row.ReceivedQty == 0 && row.ReceivedCost == 0);
							PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row, true);
                            PXUIFieldAttribute.SetEnabled<POLine.curyExtCost>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.uOM>(sender, e.Row, row.ReceivedQty == 0 && row.ReceivedCost == 0);
							break;

						default:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.subItemID>(sender, e.Row, !isNonStockKit && row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.uOM>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.discountSequenceID>(sender, e.Row, false);
							break;
					}

					PXUIFieldAttribute.SetEnabled<POLine.lineType>(sender, e.Row, POLineType.IsDefault(row.LineType) && row.ReceivedQty == 0 && row.ReceivedCost == 0);
					PXUIFieldAttribute.SetEnabled<POLine.receivedQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyReceivedCost>(sender, e.Row, false);

					PXUIFieldAttribute.SetEnabled<POLine.expenseAcctID>(sender, e.Row, !(row.LineType == POLineType.Description || POLineType.IsStock(row.LineType)));
					PXUIFieldAttribute.SetEnabled<POLine.expenseSubID>(sender, e.Row, !(row.LineType == POLineType.Description || POLineType.IsStock(row.LineType)));
				}
				if (doc.Cancelled == false && doc.Status != POOrderStatus.Closed)
				{
					if (row.POType == POOrderType.Blanket && !String.IsNullOrEmpty(row.PONbr))
					{
						POOrder source = PXSelectReadonly<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
												And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, row.POType, row.PONbr);
						if (source != null && source.ExpirationDate != null && source.ExpirationDate < doc.OrderDate)
						{
							this.Transactions.Cache.RaiseExceptionHandling<POLine.lineType>(row, row.LineType, new PXSetPropertyException(Messages.SourcePOOrderExpiresBeforeTheDateOfDocument, PXErrorLevel.RowWarning, source.ExpirationDate.Value, source.OrderNbr));
						}
					}
					//bool isStockItem = (row.LineType == POLineType.GoodsForInventory || row.LineType == POLineType.GoodsForSalesOrder || row.LineType == POLineType.GoodsForDropShip);

					//PXDefaultAttribute.SetPersistingCheck<POLine.orderQty>(sender, row, (isStockItem) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
					//PXDefaultAttribute.SetPersistingCheck<POLine.curyUnitCost>(sender, row, (isStockItem) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
					//if (isStockItem )
					//{
					//    if(row.OrderQty == Decimal.Zero)
					//        sender.RaiseExceptionHandling<POLine.orderQty>(row, row.OrderQty, new PXSetPropertyException("Order Qty should no be 0 for the stock items", PXErrorLevel.Warning));
					//    if(row.CuryUnitCost == Decimal.Zero)
					//        sender.RaiseExceptionHandling<POLine.curyUnitCost>(row, row.CuryUnitCost, new PXSetPropertyException("Unit Cost should not be 0 for the stock items", PXErrorLevel.Warning));

					//}
				}
            }

            PXUIFieldAttribute.SetEnabled<POLine.pONbr>(sender, e.Row, false);
            PXUIFieldAttribute.SetEnabled<POLine.pOType>(sender, e.Row, false);

        }

		protected virtual void POLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Row != null)
			{
				POLine row = (POLine)e.Row;

				bool isForInventory = (row.LineType == POLineType.GoodsForInventory ||
					row.LineType == POLineType.GoodsForSalesOrder ||
					row.LineType == POLineType.GoodsForReplenishment ||
					row.LineType == POLineType.GoodsForDropShip);
				bool isNonStock = POLineType.IsNonStock(row.LineType);
				bool isIntegratedCost = (row.LineType == POLineType.Freight || row.LineType == POLineType.MiscCharges);
				bool isNonStockKit = (!row.IsStockItem ?? false) && (row.IsKit ?? false);

                PXDefaultAttribute.SetPersistingCheck<POLine.inventoryID>(sender, e.Row, POLineType.IsStock(row.LineType) || POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.subItemID>(sender, e.Row, (isForInventory && !isNonStockKit) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.uOM>(sender, e.Row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.orderQty>(sender, e.Row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.baseOrderQty>(sender, e.Row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.curyUnitCost>(sender, e.Row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POLine.unitCost>(sender, e.Row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

				PXDefaultAttribute.SetPersistingCheck<POLine.expenseAcctID>(sender, e.Row, row.LineType == POLineType.Description || POLineType.IsStock(row.LineType) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
				PXDefaultAttribute.SetPersistingCheck<POLine.expenseSubID>(sender, e.Row, row.LineType == POLineType.Description || POLineType.IsStock(row.LineType) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

				PXDefaultAttribute.SetPersistingCheck<POLine.curyExtCost>(sender, e.Row, row.LineType == POLineType.Description ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

				PXDefaultAttribute.SetPersistingCheck<POLine.siteID>(sender, e.Row, isForInventory || isNonStock && row.LineType != POLineType.Service ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				bool isStockItem = isForInventory;
				if (isStockItem || isNonStock && row.LineType != POLineType.Service)
				{
					if (row.OrderQty == Decimal.Zero)
						sender.RaiseExceptionHandling<POLine.orderQty>(row, row.OrderQty, new PXSetPropertyException(Messages.POLineQuantityMustBeGreaterThanZero, PXErrorLevel.Error));
					if (isStockItem)
					{
						if (row.CuryUnitCost == Decimal.Zero && row.CuryLineAmt != Decimal.Zero)
							sender.RaiseExceptionHandling<POLine.curyUnitCost>(row, row.CuryUnitCost, new PXSetPropertyException(Messages.UnitCostShouldBeNonZeroForStockItems, PXErrorLevel.Warning));
					}
				}

				bool hasDeductibleVAT = false;
				if (row.InventoryID != null)
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.
						 SelectWindowed(this, 0, 1, row.InventoryID);
					if (item.StkItem == true || item.NonStockReceipt == true )
					{
						hasDeductibleVAT = PXSelectJoin<POTax, InnerJoin<Tax, On<POTax.taxID, Equal<Tax.taxID>>>,
							Where<POTax.orderType, Equal<Required<POTax.orderType>>, And<POTax.orderNbr, Equal<Required<POTax.orderNbr>>,
								And<POTax.lineNbr, Equal<Required<POTax.lineNbr>>,
								And<Tax.deductibleVAT, Equal<True>>>>>>.Select(this, row.OrderType, row.OrderNbr, row.LineNbr).Count != 0;
					}
				}

				if (hasDeductibleVAT == true)
				{
					string prefix;
					switch (sender.GetStatus(row))
					{
						case PXEntryStatus.Inserted:
							prefix = ErrorMessages.GetLocal(ErrorMessages.Inserting);
							break;
						default:
							prefix = ErrorMessages.GetLocal(ErrorMessages.Updating);
							break;
					}
					sender.RaiseExceptionHandling(null, row, null,
						new PXSetPropertyException<POLine.inventoryID>(Messages.DeductibleVATNotAllowed, PXErrorLevel.RowError));
					throw new PXException(ErrorMessages.RecordRaisedErrors, prefix, PXUIFieldAttribute.GetItemName(sender));
				}
			}
		}

		protected virtual void POLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			ClearUnused(row, row.LineType);

            RecalculateDiscounts(sender, (POLine)e.Row);

            TaxAttribute.Calculate<POLine.taxCategoryID>(sender, e);
		}

        protected virtual void POLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POLine row = (POLine)e.Row;
            if (row.InventoryID != ((POLine)e.OldRow).InventoryID)
            {

            }
            if (row.LineType != ((POLine)e.OldRow).LineType)
            {
                ClearUnused(row, row.LineType);
            }

            //if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
            if (Document.Current != null && IsExternalTax == true &&
                !sender.ObjectsEqual<POLine.inventoryID, POLine.tranDesc, POLine.extCost, POLine.promisedDate, POLine.taxCategoryID>(e.Row, e.OldRow))
            {
                Document.Current.IsTaxValid = false;
                Document.Update(Document.Current);
            }

            if (!sender.ObjectsEqual<POLine.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.inventoryID>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POLine.orderQty>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyUnitCost>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POLine.curyLineAmt>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyDiscAmt>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POLine.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.manualDisc>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POLine.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.siteID>(e.Row, e.OldRow))
                RecalculateDiscounts(sender, row);

            if ((e.ExternalCall || sender.Graph.IsImport)
                && sender.ObjectsEqual<POLine.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<POLine.uOM>(e.Row, e.OldRow)
                && sender.ObjectsEqual<POLine.orderQty>(e.Row, e.OldRow) && sender.ObjectsEqual<POLine.branchID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<POLine.siteID>(e.Row, e.OldRow)
                && (!sender.ObjectsEqual<POLine.curyUnitCost>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyLineAmt>(e.Row, e.OldRow)))
                row.ManualPrice = true;

            TaxAttribute.Calculate<POLine.taxCategoryID>(sender, e);
        }

		protected virtual void POLineR_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POLineR row = (POLineR)e.Row;
			if (row.OrderType == POOrderType.Blanket)
			{

				POLine orig = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
								And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
								And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.OrderType, row.OrderNbr, row.LineNbr);
				if (orig != null)
				{
					decimal? open = orig.OrderQty - row.ReceivedQty;
					if (open < 0) open = 0;
					bool isCompleted = !(open > 0);
					bool stateChanged = false;

					if (row.OpenQty != open)
					{
						POLineR upd = PXCache<POLineR>.CreateCopy(row);
						upd.OpenQty = open;
						if (row.Completed != isCompleted)
						{
							stateChanged = true;
							upd.Completed = isCompleted;
						}
						poLiner.Cache.Update(upd);

						if (row.ReceivedQty > orig.OrderQty)
						{

							foreach (POLine iSrc in PXSelect<POLine, Where<POLine.orderType, Equal<Current<POOrder.orderType>>,
								And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>,
								And<POLine.pOType, Equal<Required<POLine.pOType>>,
								And<POLine.pONbr, Equal<Required<POLine.pONbr>>,
								And<POLine.pOLineNbr, Equal<Required<POLine.pOLineNbr>>>>>>>>.Select(this, row.OrderType, row.OrderNbr,
																										 row.LineNbr))
							{
								PXEntryStatus status = this.Transactions.Cache.GetStatus(iSrc);
								if (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated)
								{
									this.Transactions.Cache.RaiseExceptionHandling<POLine.orderQty>(iSrc, iSrc.OrderQty,
														new PXSetPropertyException(Messages.OrderLineQtyExceedsQuantityInBlanketOrder,
																					PXErrorLevel.Warning, orig.OrderQty,
																					row.OrderNbr));
								}
							}
						}
					}

					if (stateChanged)
					{
						POOrderR order =
							poOrder.Select(row.OrderType, row.OrderNbr,
							isCompleted ? POOrderStatus.Open : POOrderStatus.Closed);

						POLineR rowNotInState = PXSelect<POLineR,
							Where<POLineR.orderType, Equal<Required<POLineR.orderType>>,
							And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
							And<POLineR.completed, Equal<Required<POLineR.completed>>>>>>
							.Select(this, row.OrderType, row.OrderNbr, !isCompleted);

						if (order != null && (isCompleted && rowNotInState == null || !isCompleted)) //Order has just two state - we need just alter one.
						{
							POOrderR upd = (POOrderR)poOrder.Cache.CreateCopy(order);
							upd.Status = isCompleted ? POOrderStatus.Closed : POOrderStatus.Open;
							poOrder.Update(upd);
						}
					}
				}
			}
		}

		protected virtual void POLine_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.ReceivedQty > 0)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasReceiptsAndCannotBeDeleted);
			}

			if (row.LineType == POLineType.GoodsForSalesOrder || row.LineType == POLineType.GoodsForDropShip || 
				row.LineType == POLineType.NonStockForSalesOrder || row.LineType == POLineType.NonStockForDropShip)
			{
				SOLineSplit3 first = (PXResult<SOLineSplit3>)this.FixedDemand.View.SelectMultiBound(new object[] { e.Row }).FirstOrDefault();

				if (first != null && Transactions.View.Ask("POLineLinkedToSOLine", PXMessages.LocalizeFormatNoPrefixNLA(Messages.POLineLinkedToSOLine, first.OrderNbr), MessageButtons.OKCancel) == WebDialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		protected virtual void POLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			foreach (PXResult<SOLineSplit3, INItemPlan, SOOrderTypeOperation> r in this.FixedDemand.View.SelectMultiBound(new object[] { e.Row }))
			{
				SOLineSplit3 upd = PXCache<SOLineSplit3>.CreateCopy(r);
				upd.POType = null;
				upd.PONbr = null;
				upd.POLineNbr = null;
				upd.RefNoteID = null;
				this.FixedDemand.Update(upd);

				INItemPlan plan = r;
                SOOrderTypeOperation op = r;
				if (plan.PlanType != null)
				{
                    if (upd.POCreate == false && op.OrderPlanType != null)
                    {
                        plan.PlanType = op.OrderPlanType;
                    }
					plan.SupplyPlanID = null;
					sender.Graph.Caches[typeof(INItemPlan)].Update(plan);
				}
			}

            if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
            {
                DiscountEngine<POLine>.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate);
                RecalculateTotalDiscount();
            }
		}

		protected virtual void POLine_Completed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.Completed == true)
			{
				if (CheckOpenStatus(Document.Current))
					Document.View.RequestRefresh();
			}
		}
		protected virtual void POLine_Completed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null && (bool)e.NewValue == true)
			{
				object newValue = e.NewValue;
				sender.RaiseFieldVerifying<POLine.cancelled>(row, ref newValue);
			}
		}
		protected virtual void POLine_Cancelled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.Cancelled == true)
			{
				sender.SetValueExt<POLine.completed>(row, true);
			}
			else if (row.OpenQty < row.OrderQty * row.RcptQtyThreshold / 100)
				sender.SetValueExt<POLine.completed>(row, false);
		}

		protected virtual void POLine_Cancelled_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null && (bool)e.NewValue == true)
			{
				POReceipt receipt = PXSelectJoin<POReceipt,
										InnerJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<POReceipt.receiptNbr>>>,
										Where<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
											And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
											And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>,
											And<POReceipt.released, Equal<False>>>>>>.Select(this, row.OrderType, row.OrderNbr, row.LineNbr);
				if (receipt != null)
				{
					throw new PXSetPropertyException(Messages.POLineHasUnreleaseReceiptsAndCantBeCompletedOrCancelled);
				}
			}
		}


		protected virtual void POLine_ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null)
			{
				if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO) &&
					PM.POActiveProjectAttribute.IsRequired(row.LineType))
				{
					if (location.Current != null && location.Current.VDefProjectID != null)
					{
						PX.Objects.PM.PMProject project = PXSelect<PM.PMProject, Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>.Select(this, location.Current.VDefProjectID);
						if (project != null)
							e.NewValue = project.ContractCD;
					}
				}
			}
		}


		#endregion

		#region POLineR events
		protected virtual void POLineR_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			sender.SetStatus(e.Row, PXEntryStatus.Notchanged); //This is important to prevent updating (by API) after this event .
			e.Cancel = true;
		}
		protected virtual void POLineR_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion

		#region SOLineSplit3 events
		protected virtual void SOLineSplit3_POUOM_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			SOLineSplit3 soline = (SOLineSplit3)e.Row;
			if (soline == null) return;
			POLine orig_line = PXSelect<POLine, Where<POLine.orderType, Equal<Current<SOLineSplit3.pOType>>,
				And<POLine.orderNbr, Equal<Current<SOLineSplit3.pONbr>>,
				And<POLine.lineNbr, Equal<Current<SOLineSplit3.pOLineNbr>>>>>>.SelectSingleBound(this, new object[] { soline });

			e.ReturnValue = (orig_line != null && orig_line.UOM != null) ? orig_line.UOM : soline.UOM;
		}
		protected virtual void SOLineSplit3_POUOMOrderQty_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			SOLineSplit3 soline = (SOLineSplit3)e.Row;
			if (soline == null) return;

			POLine orig_line = PXSelect<POLine, Where<POLine.orderType, Equal<Current<SOLineSplit3.pOType>>,
				And<POLine.orderNbr, Equal<Current<SOLineSplit3.pONbr>>,
				And<POLine.lineNbr, Equal<Current<SOLineSplit3.pOLineNbr>>>>>>.SelectSingleBound(this, new object[] { soline });
			if (orig_line != null)
			{
				string uom = orig_line.UOM ?? soline.UOM;
				if (string.Equals(soline.UOM, uom) == false)
				{
					decimal BaseOrderQty = INUnitAttribute.ConvertToBase<SOLineSplit3.inventoryID>(sender, soline, soline.UOM, (decimal)soline.OrderQty, INPrecision.QUANTITY);
					e.ReturnValue = INUnitAttribute.ConvertFromBase<SOLineSplit3.inventoryID>(sender, soline, uom, BaseOrderQty, INPrecision.QUANTITY);
				}
				else
				{
					e.ReturnValue = soline.OrderQty;
				}
			}
		}
		#endregion

		#region Vendor
		protected virtual void Vendor_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true; //Vendor should not be updated from this screen
		}
		#endregion

		#region Currency Info
		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info != null)
			{
				bool curyenabled = info.AllowUpdate(this.Transactions.Cache);
				if (vendor.Current != null && !(bool)vendor.Current.AllowOverrideRate)
				{
					curyenabled = false;
				}

				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyenabled);
			}
		}

		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
				{
					e.NewValue = vendor.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryRateTypeID))
				{
					e.NewValue = vendor.Current.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((POOrder)Document.Cache.Current).OrderDate;
				e.Cancel = true;
			}
		}


		#endregion

        #region POOrderDiscountDetail events

        protected virtual void POOrderDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (Document != null && Document.Current != null)
                Document.Cache.SetValueExt<POOrder.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<POOrderDiscountDetail>(DiscountDetails, true));
        }

        protected virtual void POOrderDiscountDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null && discountDetail.DiscountID != null)
            {
                discountDetail.IsManual = true;

                DiscountEngine<POLine>.InsertDocGroupDiscount<POOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate);
                RecalculateTotalDiscount();
            }
        }

        protected virtual void POOrderDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null)
            {
                if (!sender.ObjectsEqual<POOrderDiscountDetail.skipDiscount>(e.Row, e.OldRow))
                {
                    DiscountEngine<POLine>.UpdateDocumentDiscount<POOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, discountDetail.Type != DiscountType.Document);
                    RecalculateTotalDiscount();
                }
                if (!sender.ObjectsEqual<POOrderDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POOrderDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
                {
                    discountDetail.IsManual = true;
                    DiscountEngine<POLine>.InsertDocGroupDiscount<POOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<POOrderDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate);
                    RecalculateTotalDiscount();
                }
            }
        }

        protected virtual void POOrderDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null)
            {
                DiscountEngine<POLine>.UpdateDocumentDiscount<POOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document));
            }
            RecalculateTotalDiscount();
        }

        #endregion

		protected static void ClearUnused(POLine aLine, string aLineType)
		{
			if (aLine.LineType == POLineType.Description
				|| aLine.LineType == POLineType.Freight || aLine.LineType == POLineType.MiscCharges)
			{
				aLine.InventoryID = null;
				aLine.SubItemID = null;
				aLine.UOM = null;
				//aLine.OrderQty = aLine.LineType == POLineType.Description?Decimal.Zero:Decimal.One;
				//aLine.BaseOrderQty = aLine.LineType == POLineType.Description ? Decimal.Zero : Decimal.One;
				aLine.VoucheredQty = Decimal.Zero;
				aLine.BaseVoucheredQty = Decimal.Zero;
				aLine.ReceivedQty = Decimal.Zero;
				aLine.BaseReceivedQty = Decimal.Zero;
				aLine.OpenQty = aLine.OrderQty;
				aLine.BaseOpenQty = aLine.BaseOrderQty;
				aLine.CuryUnitCost = decimal.Zero;
				aLine.UnitCost = decimal.Zero;
				aLine.UnitVolume = decimal.Zero;
				aLine.UnitWeight = decimal.Zero;
				aLine.RcptQtyAction = POReceiptQtyAction.Accept;
				aLine.RcptQtyMax = 100;
				aLine.RcptQtyMin = Decimal.Zero;
			}

			if (aLine.LineType == POLineType.Description)
			{
				aLine.SiteID = null;
				aLine.ExpenseAcctID = null;
				aLine.ExpenseSubID = null;
			}
		}

		public bool IsLinkedToSO(POLine row)
		{
			return (row.LineType == POLineType.GoodsForSalesOrder || row.LineType == POLineType.GoodsForDropShip ||
					row.LineType == POLineType.NonStockForSalesOrder || row.LineType == POLineType.NonStockForDropShip);
		}
		#endregion

		#region Internal Member Definitions
        [Serializable]
		public partial class POOrderFilter : IBqlTable
		{
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}

			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
			[PXDefault(typeof(POOrder.vendorID))]
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
			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(POOrderType.Blanket)]
			[POOrderType.BlanketList()]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PO.RefNbr(typeof(Search2<POOrderS.orderNbr,
				InnerJoin<Vendor, On<POOrderS.vendorID, Equal<Vendor.bAccountID>>>,
				 Where<POOrderS.orderType, Equal<Current<POOrderEntry.POOrderFilter.orderType>>,
				And<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
				And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
				And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
				And<POOrderS.hold, Equal<boolFalse>,
				And<POOrderS.cancelled, Equal<boolFalse>,
				And<POOrderS.approved, Equal<boolTrue>,
				And<Where<POOrderS.orderType, Equal<POOrderType.blanket>,
								 Or<POOrderS.orderType, Equal<POOrderType.standardBlanket>>>>>>>>>>>>), Filterable = true)]
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
		}

		[Serializable]
		public partial class POOrderCache : IBqlTable
		{
			#region IsUsed
			public virtual bool? IsUsed
			{
				get;
				set;
			}
			#endregion
			#region StatusUpdated
			public virtual bool? StatusUpdated
			{
				get;
				set;
			}
			#endregion
		}

		[PXProjection(typeof(Select<POLine>), Persistent = false)]
        [PXCacheName(Messages.POLineS)]
        [Serializable]
		public partial class POLineS : IBqlTable
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
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[Branch(BqlField = typeof(POLine.branchID))]
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
			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
			[PXDBDefault(typeof(POOrder.orderType))]
			[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;

			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POLine.orderNbr))]
			[PXDBDefault(typeof(POOrder.orderNbr))]
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
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(POLine.lineNbr))]
			[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
			[PXLineNbr(typeof(POOrder.lineCntr))]
			public virtual Int32? LineNbr
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
			#region LineType
			public abstract class lineType : PX.Data.IBqlField
			{
			}
			protected String _LineType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(POLine.lineType))]
			[PXDefault(POLineType.GoodsForInventory)]
			[POLineType.List()]
			[PXUIField(DisplayName = "Line Type")]
			public virtual String LineType
			{
				get
				{
					return this._LineType;
				}
				set
				{
					this._LineType = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[POLineInventoryItem(Filterable = true, BqlField = typeof(POLine.inventoryID))]
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
			[SubItem(typeof(POLineS.inventoryID), BqlField = typeof(POLine.subItemID))]
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
			#region SiteID
			public abstract class siteID : PX.Data.IBqlField
			{
			}
			protected Int32? _SiteID;
			[SiteAvail(typeof(POLineS.inventoryID), typeof(POLineS.subItemID), BqlField = typeof(POLine.siteID))]
			[PXDefault(typeof(Search<Location.vSiteID,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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

			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;


			[INUnit(typeof(POLineS.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
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
			#region OrderQty
			public abstract class orderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderQty;
			[PXDBQuantity(typeof(POLineS.uOM), typeof(POLineS.baseOrderQty), HandleEmptyKey = true, BqlField = typeof(POLine.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(null, typeof(SumCalc<POOrder.orderQty>))]
			[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region BaseOrderQty
			public abstract class baseOrderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseOrderQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseOpenQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? BaseOrderQty
			{
				get
				{
					return this._BaseOrderQty;
				}
				set
				{
					this._BaseOrderQty = value;
				}
			}
			#endregion
			#region ReceivedQty
			public abstract class receivedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceivedQty;
			[PXDBQuantity(typeof(POLineS.uOM), typeof(POLineS.baseReceivedQty), HandleEmptyKey = true, BqlField = typeof(POLine.receivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? ReceivedQty
			{
				get
				{
					return this._ReceivedQty;
				}
				set
				{
					this._ReceivedQty = value;
				}
			}
			#endregion
			#region BaseReceivedQty
			public abstract class baseReceivedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseReceivedQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseReceivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Base Received Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? BaseReceivedQty
			{
				get
				{
					return this._BaseReceivedQty;
				}
				set
				{
					this._BaseReceivedQty = value;
				}
			}
			#endregion
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _CuryInfoID;
			[PXDBLong(BqlField = typeof(POLine.curyInfoID))]
			public virtual Int64? CuryInfoID
			{
				get
				{
					return this._CuryInfoID;
				}
				set
				{
					this._CuryInfoID = value;
				}
			}
			#endregion
			#region CuryUnitCost
			public abstract class curyUnitCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryUnitCost;

			[PXDBCurrency(typeof(POLineS.curyInfoID), typeof(POLineS.unitCost), BqlField = typeof(POLine.curyUnitCost))]
			[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryUnitCost
			{
				get
				{
					return this._CuryUnitCost;
				}
				set
				{
					this._CuryUnitCost = value;
				}
			}
			#endregion
			#region UnitCost
			public abstract class unitCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnitCost;

			[PXDBPriceCost(BqlField = typeof(POLine.unitCost))]
			public virtual Decimal? UnitCost
			{
				get
				{
					return this._UnitCost;
				}
				set
				{
					this._UnitCost = value;
				}
			}
			#endregion
			#region ManualPrice
			public abstract class manualPrice : PX.Data.IBqlField
			{
			}
			protected Boolean? _ManualPrice;
			[PXDBBool(BqlField = typeof(POLine.manualPrice))]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Manual Price", Visibility = PXUIVisibility.Visible)]
			public virtual Boolean? ManualPrice
			{
				get
				{
					return this._ManualPrice;
				}
				set
				{
					this._ManualPrice = value;
				}
			}
			#endregion
			#region CuryExtCost
			public abstract class curyExtCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryExtCost;
			[PXDBCurrency(typeof(POLineS.curyInfoID), typeof(POLineS.extCost), BqlField = typeof(POLine.curyExtCost))]
			[PXUIField(DisplayName = "Extended Amt.", Visibility = PXUIVisibility.SelectorVisible)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryExtCost
			{
				get
				{
					return this._CuryExtCost;
				}
				set
				{
					this._CuryExtCost = value;
				}
			}
			#endregion
			#region ExtCost
			public abstract class extCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _ExtCost;

			[PXDBBaseCury(BqlField = typeof(POLine.extCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? ExtCost
			{
				get
				{
					return this._ExtCost;
				}
				set
				{
					this._ExtCost = value;
				}
			}
			#endregion
			#region TaxCategoryID
			public abstract class taxCategoryID : PX.Data.IBqlField
			{
			}
			protected String _TaxCategoryID;
			[PXDBString(10, IsUnicode = true, BqlField = typeof(POLine.taxCategoryID))]
			[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
			//[POTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
			//[POOpenTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
            [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
            [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
            public virtual String TaxCategoryID
			{
				get
				{
					return this._TaxCategoryID;
				}
				set
				{
					this._TaxCategoryID = value;
				}
			}
			#endregion
			#region ExpenseAcctID
			public abstract class expenseAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _ExpenseAcctID;
			[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable = false, BqlField = typeof(POLine.expenseAcctID))]
			public virtual Int32? ExpenseAcctID
			{
				get
				{
					return this._ExpenseAcctID;
				}
				set
				{
					this._ExpenseAcctID = value;
				}
			}
			#endregion
			#region ExpenseSubID
			public abstract class expenseSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _ExpenseSubID;

			[SubAccount(typeof(POLineS.expenseAcctID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true, BqlField = typeof(POLine.expenseSubID))]
			public virtual Int32? ExpenseSubID
			{
				get
				{
					return this._ExpenseSubID;
				}
				set
				{
					this._ExpenseSubID = value;
				}
			}
			#endregion
			#region AlternateID
			public abstract class alternateID : PX.Data.IBqlField
			{
			}
			protected String _AlternateID;
			[PXDBString(50, IsUnicode = true, BqlField = typeof(POLine.alternateID), InputMask = "")]
			[PXUIField(DisplayName = "Alternate ID")]
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
			#region TranDesc
			public abstract class tranDesc : PX.Data.IBqlField
			{
			}
			protected String _TranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(POLine.tranDesc))]
			[PXUIField(DisplayName = "Line Description", Visibility = PXUIVisibility.Visible)]
			public virtual String TranDesc
			{
				get
				{
					return this._TranDesc;
				}
				set
				{
					this._TranDesc = value;
				}
			}
			#endregion
			#region UnitWeight
			public abstract class unitWeight : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnitWeight;
			[PXDBDecimal(6, BqlField = typeof(POLine.unitWeight))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unit Weight")]
			public virtual Decimal? UnitWeight
			{
				get
				{
					return this._UnitWeight;
				}
				set
				{
					this._UnitWeight = value;
				}
			}
			#endregion
			#region UnitVolume
			public abstract class unitVolume : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnitVolume;
			[PXDBDecimal(6, BqlField = typeof(POLine.unitVolume))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unit Volume")]
			public virtual Decimal? UnitVolume
			{
				get
				{
					return this._UnitVolume;
				}
				set
				{
					this._UnitVolume = value;
				}
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[POProjectDefault(typeof(POLine.lineType))]
			[ActiveProjectForModule(BatchModule.PO, BqlField = typeof(POLine.projectID))]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region TaskID
			public abstract class taskID : PX.Data.IBqlField
			{
			}
			protected Int32? _TaskID;
			[ActiveProjectTask(typeof(POLine.projectID), BatchModule.PO, DisplayName = "Project Task", BqlField = typeof(POLine.taskID))]
			public virtual Int32? TaskID
			{
				get
				{
					return this._TaskID;
				}
				set
				{
					this._TaskID = value;
				}
			}
			#endregion
			#region LeftToReceiveQty
			public abstract class leftToReceiveQty : PX.Data.IBqlField
			{
			}
			[PXQuantity()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Open Qty.", Visibility = PXUIVisibility.Invisible)]
			public virtual Decimal? LeftToReceiveQty
			{
				[PXDependsOnFields(typeof(orderQty), typeof(receivedQty))]
				get
				{
					return (this._OrderQty - this._ReceivedQty);
				}
			}
			#endregion
			#region CuryReceivedCost
			public abstract class curyReceivedCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryReceivedCost;

			[PXDBCurrency(typeof(POLineS.curyInfoID), typeof(POLineS.receivedCost), BqlField = typeof(POLine.curyReceivedCost))]
			[PXUIField(DisplayName = "Received Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]

			public virtual Decimal? CuryReceivedCost
			{
				get
				{
					return this._CuryReceivedCost;
				}
				set
				{
					this._CuryReceivedCost = value;
				}
			}
			#endregion
			#region ReceivedCost
			public abstract class receivedCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceivedCost;
			[PXDBDecimal(6, BqlField = typeof(POLine.receivedCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Cost")]
			public virtual Decimal? ReceivedCost
			{
				get
				{
					return this._ReceivedCost;
				}
				set
				{
					this._ReceivedCost = value;
				}
			}
			#endregion
			#region RcptQtyMin
			public abstract class rcptQtyMin : PX.Data.IBqlField
			{
			}
			protected Decimal? _RcptQtyMin;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyMin))]
			[PXDefault(typeof(Search<Location.vRcptQtyMin,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Min. Receipt (%)")]
			public virtual Decimal? RcptQtyMin
			{
				get
				{
					return this._RcptQtyMin;
				}
				set
				{
					this._RcptQtyMin = value;
				}
			}
			#endregion
			#region RcptQtyMax
			public abstract class rcptQtyMax : PX.Data.IBqlField
			{
			}
			protected Decimal? _RcptQtyMax;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyMax))]
			[PXDefault(typeof(Search<Location.vRcptQtyMax,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Max. Receipt (%)")]
			public virtual Decimal? RcptQtyMax
			{
				get
				{
					return this._RcptQtyMax;
				}
				set
				{
					this._RcptQtyMax = value;
				}
			}
			#endregion
			#region RcptQtyThreshold
			public abstract class rcptQtyThreshold : PX.Data.IBqlField
			{
			}
			protected Decimal? _RcptQtyThreshold;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyThreshold))]
			[PXDefault(typeof(Search<Location.vRcptQtyThreshold,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Complete On (%)")]
			public virtual Decimal? RcptQtyThreshold
			{
				get
				{
					return this._RcptQtyThreshold;
				}
				set
				{
					this._RcptQtyThreshold = value;
				}
			}
			#endregion
			#region RcptQtyAction
			public abstract class rcptQtyAction : PX.Data.IBqlField
			{
			}
			protected String _RcptQtyAction;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POLine.rcptQtyAction))]
			[POReceiptQtyAction.List()]
			[PXDefault(typeof(Search<Location.vRcptQtyAction,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Receipt Action")]
			public virtual String RcptQtyAction
			{
				get
				{
					return this._RcptQtyAction;
				}
				set
				{
					this._RcptQtyAction = value;
				}
			}
			#endregion
			#region Cancelled
			public abstract class cancelled : PX.Data.IBqlField
			{
			}
			protected Boolean? _Cancelled;
			[PXDBBool(BqlField = typeof(POLine.cancelled))]
			[PXUIField(DisplayName = "Canceled", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Cancelled
			{
				get
				{
					return this._Cancelled;
				}
				set
				{
					this._Cancelled = value;
				}
			}
			#endregion
			#region Completed
			public abstract class completed : PX.Data.IBqlField
			{
			}
			protected Boolean? _Completed;
			[PXDBBool(BqlField = typeof(POLine.completed))]
			[PXUIField(DisplayName = "Completed", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Completed
			{
				get
				{
					return this._Completed;
				}
				set
				{
					this._Completed = value;
				}
			}
			#endregion
		}

		[PXProjection(typeof(Select5<POOrder,
										InnerJoin<POLine, On<POLine.orderType, Equal<POOrder.orderType>,
											And<POLine.orderNbr, Equal<POOrder.orderNbr>,
											And<POLine.cancelled, NotEqual<boolTrue>,
											And<POLine.completed, NotEqual<boolTrue>,
											And<Where<POOrder.orderType, Equal<POOrderType.standardBlanket>,
														 Or<POLine.lineType, NotEqual<POLineType.description>>>>>>>>>,
											Where<POOrder.orderType, Equal<POOrderType.blanket>,
												 Or<POOrder.orderType, Equal<POOrderType.standardBlanket>>>,
											Aggregate
												<GroupBy<POOrder.orderType,
												GroupBy<POOrder.orderNbr,
												GroupBy<POOrder.orderDate,
												GroupBy<POOrder.curyID,
												GroupBy<POOrder.curyOrderTotal,
												GroupBy<POOrder.hold,
												GroupBy<POOrder.receipt,
												GroupBy<POOrder.cancelled,
												GroupBy<POOrder.approved,
												GroupBy<POOrder.isTaxValid,
												GroupBy<POOrder.isOpenTaxValid,
												Sum<POLine.orderQty,
												Sum<POLine.baseOrderQty,
												Sum<POLine.receivedQty,
												Sum<POLine.baseReceivedQty,
												Sum<POLine.curyExtCost,
												Sum<POLine.extCost,
												Sum<POLine.curyReceivedCost,
												Sum<POLine.receivedCost>>>>>>>>>>>>>>>>>>>>>), Persistent = false)]
        [Serializable]
		public partial class POOrderS : IBqlTable
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

			#region ReceivedQty
			public abstract class receivedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceivedQty;
			[PXDBQuantity(HandleEmptyKey = true, BqlField = typeof(POLine.receivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? ReceivedQty
			{
				get
				{
					return this._ReceivedQty;
				}
				set
				{
					this._ReceivedQty = value;
				}
			}
			#endregion
			#region BaseReceivedQty
			public abstract class baseReceivedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseReceivedQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseReceivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Base Received Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? BaseReceivedQty
			{
				get
				{
					return this._BaseReceivedQty;
				}
				set
				{
					this._BaseReceivedQty = value;
				}
			}
			#endregion
			#region CuryReceivedCost
			public abstract class curyReceivedCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryReceivedCost;
			[PXDBDecimal(6, BqlField = typeof(POLine.curyReceivedCost))]
			[PXUIField(DisplayName = "Received Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryReceivedCost
			{
				get
				{
					return this._CuryReceivedCost;
				}
				set
				{
					this._CuryReceivedCost = value;
				}
			}
			#endregion
			#region ReceivedCost
			public abstract class receivedCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceivedCost;
			[PXDBDecimal(6, BqlField = typeof(POLine.receivedCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Cost")]
			public virtual Decimal? ReceivedCost
			{
				get
				{
					return this._ReceivedCost;
				}
				set
				{
					this._ReceivedCost = value;
				}
			}
			#endregion
			#region OpenLineQty
			public abstract class openLineQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OpenLineQty;
			[PXDBQuantity(HandleEmptyKey = true, BqlField = typeof(POLine.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Open Line Qty Total")]
			public virtual Decimal? OpenLineQty
			{
				get
				{
					return this._OpenLineQty;
				}
				set
				{
					this._OpenLineQty = value;
				}
			}
			#endregion
			#region BaseOpenLineQty
			public abstract class baseOpenLineQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseOpenLineQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseOrderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? BaseOpenLineQty
			{
				get
				{
					return this._BaseOpenLineQty;
				}
				set
				{
					this._BaseOpenLineQty = value;
				}
			}
			#endregion
			#region CuryOpenLineCost
			public abstract class curyOpenLineCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryOpenLineCost;
			[PXDBDecimal(6, BqlField = typeof(POLine.curyExtCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Open Line Cost Total")]
			public virtual Decimal? CuryOpenLineCost
			{
				get
				{
					return this._CuryOpenLineCost;
				}
				set
				{
					this._CuryOpenLineCost = value;
				}
			}
			#endregion
			#region OpenLineCost
			public abstract class openLineCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _OpenLineCost;
			[PXDBDecimal(6, BqlField = typeof(POLine.extCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]

			public virtual Decimal? OpenLineCost
			{
				get
				{
					return this._OpenLineCost;
				}
				set
				{
					this._OpenLineCost = value;
				}
			}
			#endregion

			#region CuryLeftToReceiveCost
			public abstract class curyLeftToReceiveCost : PX.Data.IBqlField
			{
			}

			[PXCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.leftToReceiveCost))]
			[PXUIField(DisplayName = "Open Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryLeftToReceiveCost
			{
				[PXDependsOnFields(typeof(curyOpenLineCost), typeof(curyReceivedCost))]
				get
				{
					return (this.CuryOpenLineCost - this.CuryReceivedCost);
				}
			}
			#endregion
			#region LeftToReceiveCost
			public abstract class leftToReceiveCost : PX.Data.IBqlField
			{
			}
			[PXBaseCury()]
			[PXUIField(DisplayName = "Open Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? LeftToReceiveCost
			{
				[PXDependsOnFields(typeof(openLineCost), typeof(receivedCost))]
				get
				{
					return (this.OpenLineCost - this.ReceivedCost);
				}
			}
			#endregion
			#region LeftToReceiveQty
			public abstract class leftToReceiveQty : PX.Data.IBqlField
			{
			}
			[PXQuantity()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Open Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? LeftToReceiveQty
			{
				[PXDependsOnFields(typeof(openLineQty), typeof(receivedQty))]
				get
				{
					return (this.OpenLineQty - this.ReceivedQty);
				}
			}
			#endregion

			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POOrder.orderType))]
			[POOrderType.List()]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POOrder.orderNbr))]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PO.Numbering()]
			[PO.RefNbr(typeof(Search2<POOrderS.orderNbr,
				InnerJoinSingleTable<Vendor, On<POOrderS.vendorID, Equal<Vendor.bAccountID>>>,
				Where<POOrderS.orderType, Equal<Optional<POOrderS.orderType>>,
				And<Match<Vendor, Current<AccessInfo.userName>>>>>), Filterable = true)]
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(POOrder.vendorID))]
			[PXDefault()]
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

			[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POOrderS.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(POOrder.vendorLocationID))]
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
			#region OrderDate
			public abstract class orderDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _OrderDate;

			[PXDBDate(BqlField = typeof(POOrder.orderDate))]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual DateTime? OrderDate
			{
				get
				{
					return this._OrderDate;
				}
				set
				{
					this._OrderDate = value;
				}
			}
			#endregion
			#region ExpectedDate
			public abstract class expectedDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ExpectedDate;

			[PXDBDate(BqlField = typeof(POOrder.expectedDate))]
			[PXDefault(typeof(POOrderS.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Promised On")]
			public virtual DateTime? ExpectedDate
			{
				get
				{
					return this._ExpectedDate;
				}
				set
				{
					this._ExpectedDate = value;
				}
			}
			#endregion
			#region ExpectedDate
			public abstract class expirationDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ExpirationDate;

			[PXDBDate(BqlField = typeof(POOrder.expirationDate))]
			[PXUIField(DisplayName = "Expired On")]
			public virtual DateTime? ExpirationDate
			{
				get
				{
					return this._ExpirationDate;
				}
				set
				{
					this._ExpirationDate = value;
				}
			}
			#endregion
			#region Status
			public abstract class status : PX.Data.IBqlField
			{
			}
			protected String _Status;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POOrder.status))]
			[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[POOrderStatus.List()]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.IBqlField
			{
			}
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(POOrder.hold))]
			[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
			[PXDefault(true)]
			public virtual Boolean? Hold
			{
				get
				{
					return this._Hold;
				}
				set
				{
					this._Hold = value;
				}
			}
			#endregion
			#region Approved
			public abstract class approved : PX.Data.IBqlField
			{
			}
			protected Boolean? _Approved;
			[PXDBBool(BqlField = typeof(POOrder.approved))]
			[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible)]
			[PXDefault(true)]
			public virtual Boolean? Approved
			{
				get
				{
					return this._Approved;
				}
				set
				{
					this._Approved = value;
				}
			}
			#endregion
			#region Cancelled
			public abstract class cancelled : PX.Data.IBqlField
			{
			}
			protected Boolean? _Cancelled;
			[PXDBBool(BqlField = typeof(POOrder.cancelled))]
			[PXUIField(DisplayName = "Cancel", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Cancelled
			{
				get
				{
					return this._Cancelled;
				}
				set
				{
					this._Cancelled = value;
				}
			}
			#endregion
			#region Receipt
			public abstract class receipt : PX.Data.IBqlField
			{
			}
			protected Boolean? _Receipt;
			[PXDBBool(BqlField = typeof(POOrder.receipt))]
			[PXUIField(DisplayName = "Receipt", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Receipt
			{
				get
				{
					return this._Receipt;
				}
				set
				{
					this._Receipt = value;
				}
			}
			#endregion
			#region IsTaxValid
			public abstract class isTaxValid : PX.Data.IBqlField
			{
			}
			[PXDBBool(BqlField = typeof(POOrder.isTaxValid))]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Tax is up to date", Enabled = false)]
			public virtual Boolean? IsTaxValid
			{
				get;
				set;
			}
			#endregion
			#region IsOpenTaxValid
			public abstract class isOpenTaxValid : PX.Data.IBqlField
			{
			}
			[PXDBBool(BqlField = typeof(POOrder.isOpenTaxValid))]
			[PXDefault(false)]
			public virtual Boolean? IsOpenTaxValid
			{
				get;
				set;
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POOrder.curyID))]
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
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _CuryInfoID;
			[PXDBLong(BqlField = typeof(POOrder.curyInfoID))]
			public virtual Int64? CuryInfoID
			{
				get
				{
					return this._CuryInfoID;
				}
				set
				{
					this._CuryInfoID = value;
				}
			}
			#endregion
			#region VendorRefNbr
			public abstract class vendorRefNbr : PX.Data.IBqlField
			{
			}
			protected String _VendorRefNbr;
			[PXDBString(40, IsUnicode = true, BqlField = typeof(POOrder.vendorRefNbr))]
			[PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
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
			#region CuryOrderTotal
			public abstract class curyOrderTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryOrderTotal;

			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.orderTotal), BqlField = typeof(POOrder.curyOrderTotal))]
			[PXUIField(DisplayName = "Order Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual Decimal? CuryOrderTotal
			{
				get
				{
					return this._CuryOrderTotal;
				}
				set
				{
					this._CuryOrderTotal = value;
				}
			}
			#endregion
			#region OrderTotal
			public abstract class orderTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderTotal;
			[PXDBBaseCury(BqlField = typeof(POOrder.orderTotal))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? OrderTotal
			{
				get
				{
					return this._OrderTotal;
				}
				set
				{
					this._OrderTotal = value;
				}
			}
			#endregion
			#region OrderQty
			public abstract class orderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderQty;
			[PXDBQuantity(BqlField = typeof(POOrder.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region TermsID
			public abstract class termsID : PX.Data.IBqlField
			{
			}
			protected String _TermsID;
			[PXDBString(10, IsUnicode = true, IsFixed = true, BqlField = typeof(POOrder.termsID))]
			[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]

			public virtual String TermsID
			{
				get
				{
					return this._TermsID;
				}
				set
				{
					this._TermsID = value;
				}
			}
			#endregion
			#region OrderDesc
			public abstract class orderDesc : PX.Data.IBqlField
			{
			}
			protected String _OrderDesc;
			[PXDBString(60, IsUnicode = true, BqlField = typeof(POOrder.orderDesc))]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String OrderDesc
			{
				get
				{
					return this._OrderDesc;
				}
				set
				{
					this._OrderDesc = value;
				}
			}
			#endregion
			#region CuryLineTotal
			public abstract class curyLineTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryLineTotal;
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.lineTotal), BqlField = typeof(POOrder.curyLineTotal))]
			[PXUIField(DisplayName = "Line Total", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Decimal? CuryLineTotal
			{
				get
				{
					return this._CuryLineTotal;
				}
				set
				{
					this._CuryLineTotal = value;
				}
			}
			#endregion
			#region LineTotal
			public abstract class lineTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _LineTotal;
			[PXDBBaseCury(BqlField = typeof(POOrder.lineTotal))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? LineTotal
			{
				get
				{
					return this._LineTotal;
				}
				set
				{
					this._LineTotal = value;
				}
			}
			#endregion
		}

		[PXProjection(typeof(Select<POOrder>), Persistent = true)]
        [Serializable]
		public partial class POOrderR : IBqlTable
		{
			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POOrder.orderType))]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;

			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POOrder.orderNbr))]
			[PXDefault()]
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
			#region Status
			public abstract class status : PX.Data.IBqlField
			{
			}
			protected String _Status;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POOrder.status))]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.IBqlField
			{
			}
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(POOrder.hold))]
			public virtual Boolean? Hold
			{
				get
				{
					return this._Hold;
				}
				set
				{
					this._Hold = value;
				}
			}
			#endregion
		}

		[PXProjection(typeof(Select2<SOLineSplit,
					InnerJoin<SOLine, On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>>), new Type[] { typeof(SOLineSplit) } )]
        [Serializable]
		public partial class SOLineSplit3 : IBqlTable
		{
			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(SOLineSplit.orderType))]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Type", Enabled = false)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(SOLineSplit.orderNbr))]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
			[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<SOLineSplit3.orderType>>>>))]
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
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.lineNbr))]
			[PXUIField(DisplayName = "Line Nbr.", Enabled = false)]
			public virtual Int32? LineNbr
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
			#region SplitLineNbr
			public abstract class splitLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _SplitLineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.splitLineNbr))]
			public virtual Int32? SplitLineNbr
			{
				get
				{
					return this._SplitLineNbr;
				}
				set
				{
					this._SplitLineNbr = value;
				}
			}
			#endregion
            #region Operation
            public abstract class operation : PX.Data.IBqlField
            {
            }
            protected String _Operation;
            [PXDBString(1, IsFixed = true, BqlField = typeof(SOLineSplit.operation))]
            public virtual String Operation
            {
                get
                {
                    return this._Operation;
                }
                set
                {
                    this._Operation = value;
                }
            }
            #endregion
			#region LineType
			public abstract class lineType : PX.Data.IBqlField
			{
			}
			protected String _LineType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(SOLine.lineType))]
			[PXUIField(DisplayName = "Line Type", Enabled = false)]
			public virtual String LineType
			{
				get
				{
					return this._LineType;
				}
				set
				{
					this._LineType = value;
				}
			}
			#endregion
            #region POCreate
            public abstract class pOCreate : PX.Data.IBqlField
            {
            }
            protected bool? _POCreate;
            [PXDBBool(BqlField = typeof(SOLineSplit.pOCreate))]
            public virtual bool? POCreate
            {
                get
                {
                    return this._POCreate;
                }
                set
                {
                    this._POCreate = value;
                }
            }
            #endregion
			#region POType
			public abstract class pOType : PX.Data.IBqlField
			{
			}
			protected String _POType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(SOLineSplit.pOType))]
			[PXUIField(DisplayName = "PO Type", Enabled = false)]
			public virtual String POType
			{
				get
				{
					return this._POType;
				}
				set
				{
					this._POType = value;
				}
			}
			#endregion
			#region PONbr
			public abstract class pONbr : PX.Data.IBqlField
			{
			}
			protected String _PONbr;
			[PXDBString(15, IsUnicode = true, BqlField = typeof(SOLineSplit.pONbr))]
			[PXDBDefault(typeof(POOrder.orderNbr), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "PO Nbr.", Enabled = false)]
			public virtual String PONbr
			{
				get
				{
					return this._PONbr;
				}
				set
				{
					this._PONbr = value;
				}
			}
			#endregion
			#region POLineNbr
			public abstract class pOLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _POLineNbr;
			[PXDBInt(BqlField = typeof(SOLineSplit.pOLineNbr))]
			[PXUIField(DisplayName = "PO Line Nbr.", Enabled = false)]
			public virtual Int32? POLineNbr
			{
				get
				{
					return this._POLineNbr;
				}
				set
				{
					this._POLineNbr = value;
				}
			}
			#endregion
            #region RefNoteID
            public abstract class refNoteID : PX.Data.IBqlField
            {
            }
            protected Guid? _RefNoteID;
            [PXDBGuid(BqlField = typeof(SOLineSplit.refNoteID))]
            public virtual Guid? RefNoteID
            {
                get
                {
                    return this._RefNoteID;
                }
                set
                {
                    this._RefNoteID = value;
                }
            }
            #endregion
			#region RequestDate
			public abstract class requestDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _RequestDate;
			[PXDBDate(BqlField = typeof(SOLine.requestDate))]
			[PXDefault()]
			[PXUIField(DisplayName = "Requested", Enabled = false)]
			public virtual DateTime? RequestDate
			{
				get
				{
					return this._RequestDate;
				}
				set
				{
					this._RequestDate = value;
				}
			}
			#endregion
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[AR.Customer(BqlField = typeof(SOLine.customerID), Enabled = false)]
			[PXDefault()]
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
            #region SiteID
            public abstract class siteID : PX.Data.IBqlField
            {
            }
            protected Int32? _SiteID;
            [SiteAvail(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.subItemID), BqlField = typeof(SOLineSplit.siteID), DisplayName = "Warehouse")]
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
			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;
			[INUnit(typeof(SOLineSplit.inventoryID), DisplayName = "Orig. UOM", BqlField = typeof(SOLineSplit.uOM), Enabled = false)]
			[PXDefault()]
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
			#region OrderQty
			public abstract class orderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderQty;
			[PXDBQuantity(BqlField = typeof(SOLineSplit.qty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Orig. Quantity", Enabled = false)]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region POUOM
			public abstract class pOUOM : PX.Data.IBqlField
			{
			}
			protected String _POUOM;
			[PXString(6, IsUnicode = true, InputMask = ">aaaaaa")]
			[PXUIField(DisplayName = "UOM", Enabled = false)]
			public virtual String POUOM
			{
				get
				{
					return this._POUOM;
				}
				set
				{
					this._POUOM = value;
				}
			}
			#endregion
			#region POUOMOrderQty
			public abstract class pOUOMOrderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _POUOMOrderQty;
			[PXQuantity]
			[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Quantity", Enabled = false)]
			public virtual Decimal? POUOMOrderQty
			{
				get
				{
					return this._POUOMOrderQty;
				}
				set
				{
					this._POUOMOrderQty = value;
				}
			}
			#endregion
			#region PlanID
			public abstract class planID : PX.Data.IBqlField
			{
			}
			protected Int64? _PlanID;
			[PXDBLong(BqlField = typeof(SOLineSplit.planID))]
			public virtual Int64? PlanID
			{
				get
				{
					return this._PlanID;
				}
				set
				{
					this._PlanID = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[CrossItem(INPrimaryAlternateType.CPN, Filterable = true, BqlField = typeof(SOLineSplit.inventoryID))]
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
			#region ShipDate
			public abstract class shipDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ShipDate;
			[PXDBDate(BqlField = typeof(SOLineSplit.shipDate))]
			[PXUIField(DisplayName = "Ship On", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual DateTime? ShipDate
			{
				get
				{
					return this._ShipDate;
				}
				set
				{
					this._ShipDate = value;
				}
			}
			#endregion
			#region TranDesc
			public abstract class tranDesc : PX.Data.IBqlField
			{
			}
			protected String _TranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(SOLine.tranDesc))]
			[PXUIField(DisplayName = "Line Description")]
			public virtual String TranDesc
			{
				get
				{
					return this._TranDesc;
				}
				set
				{
					this._TranDesc = value;
				}
			}
			#endregion
			#region SalesAcctID
			public abstract class salesAcctID : PX.Data.IBqlField
			{
			}
			[PXDBInt(BqlField = typeof(SOLine.salesAcctID))]
			public virtual Int32? SalesAcctID
			{
				get;
				set;
			}
			#endregion
			#region SalesSubID
			public abstract class salesSubID : PX.Data.IBqlField
			{
			}
			[PXDBInt(BqlField = typeof(SOLine.salesSubID))]
			public virtual Int32? SalesSubID
			{
				get;
				set;
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[PXDBInt(BqlField = typeof(SOLine.projectID))]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region TaskID
			public abstract class taskID : PX.Data.IBqlField
			{
			}
			protected Int32? _TaskID;
			[PXDBInt(BqlField = typeof(SOLine.taskID))]
			public virtual Int32? TaskID
			{
				get
				{
					return this._TaskID;
				}
				set
				{
					this._TaskID = value;
				}
			}
			#endregion

			#region NoteID
			public abstract class noteID : PX.Data.IBqlField
			{
			}
			protected Guid? _NoteID;
			[PXNote(BqlField = typeof(SOLine.noteID))]
			public virtual Guid? NoteID
			{
				get
				{
					return this._NoteID;
				}
				set
				{
					this._NoteID = value;
				}
			}
			#endregion

		}
		#endregion
		
		#region Implementation of IPXPrepareItems

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (string.Compare(viewName, "Transactions", true) == 0)
			{
				if (values.Contains("orderType")) values["orderType"] = Document.Current.OrderType;
				else values.Add("orderType", Document.Current.OrderType);

				if (values.Contains("orderNbr")) values["orderNbr"] = Document.Current.OrderNbr;
				else values.Add("orderNbr", Document.Current.OrderNbr);
				this._blockUIUpdate = true;
			}

			return true;
		}

		public bool RowImporting(string viewName, object row)
		{
			return row == null;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		public virtual void PrepareItems(string viewName, IEnumerable items)
		{
		}

		#endregion

		#region EPApproval Cahce Attached
		[PXDBDate()]
		[PXDefault(typeof(POOrder.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(POOrder.employeeID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(POOrder.orderDesc), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		[CurrencyInfo(typeof(POOrder.curyInfoID))]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(POOrder.curyOrderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(POOrder.orderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region Avalara Tax

		public bool IsExternalTax
		{
			get
			{
				TX.TaxZone tz = PXSelect<TX.TaxZone, Where<TX.TaxZone.taxZoneID, Equal<Current<POOrder.taxZoneID>>>>.Select(this);
				if (tz != null)
					return tz.IsExternal.GetValueOrDefault(false);
				else
					return false;
			}
		}

		public virtual void CalculateAvalaraTax(POOrder order)
		{
			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);

			AvaAddress.AddressSvc addressService = new AvaAddress.AddressSvc();
			AvalaraMaint.SetupService(this, addressService);

			GetTaxRequest getRequest = null;
			GetTaxRequest getRequestOpen = null;
			
			bool isValidByDefault = true;

			if (order.IsTaxValid != true)
			{
				getRequest = BuildGetTaxRequest(order);

				if (getRequest.Lines.Count > 0)
				{
					isValidByDefault = false;
				}
				else
				{
					getRequest = null;
				}
			}

			if (order.IsOpenTaxValid != true)
			{
				getRequestOpen = BuildGetTaxRequestOpen(order);
				if (getRequestOpen.Lines.Count > 0)
				{
					isValidByDefault = false;
				}
				else
				{
					getRequestOpen = null;
				}
			}

			if (isValidByDefault)
			{
				PXDatabase.Update<POOrder>(
					new PXDataFieldAssign("IsTaxValid", true),
					new PXDataFieldAssign("IsOpenTaxValid", true),
					new PXDataFieldRestrict("OrderType", PXDbType.VarChar, 2, order.OrderType, PXComp.EQ),
					new PXDataFieldRestrict("OrderNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ)
					);
				order.IsTaxValid = true;
				order.IsOpenTaxValid = true;
				PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());
				return;
			}

			GetTaxResult result = null;
			GetTaxResult resultOpen = null;
			
			bool getTaxFailed = false;
			if (getRequest != null)
			{
				result = service.GetTax(getRequest);
				if (result.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}
			if (getRequestOpen != null)
			{
				resultOpen = service.GetTax(getRequestOpen);
				if (resultOpen.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}

			if (!getTaxFailed)
			{
				try
				{
					ApplyAvalaraTax(order, result, resultOpen);
					PXDatabase.Update<POOrder>(
						new PXDataFieldAssign("IsTaxValid", true),
						new PXDataFieldAssign("IsOpenTaxValid", true),
						new PXDataFieldRestrict("OrderType", PXDbType.VarChar, 2, order.OrderType, PXComp.EQ),
						new PXDataFieldRestrict("OrderNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ)
						);
					order.IsTaxValid = true;
					order.IsOpenTaxValid = true;
					PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());

				}
				catch (PXOuterException ex)
				{
					string msg = TX.Messages.FailedToApplyTaxes;
					foreach (string err in ex.InnerMessages)
					{
						msg += Environment.NewLine + err;
					}

					throw new PXException(ex, msg);
				}
				catch (Exception ex)
				{
					string msg = TX.Messages.FailedToApplyTaxes;
					msg += Environment.NewLine + ex.Message;

					throw new PXException(ex, msg);
				}
			}
			else
			{
				LogMessages(result);

				throw new PXException(TX.Messages.FailedToGetTaxes);
			}
		}

		protected virtual GetTaxRequest BuildGetTaxRequest(POOrder order)
		{
			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Location loc = (Location)location.View.SelectSingleBound(new object[] { order });
			Vendor vend = (Vendor)vendor.View.SelectSingleBound(new object[] { order });

			IAddressBase fromAddress = GetFromAddress(order);
			IAddressBase toAddress = GetToAddress(order);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, order.BranchID);
			request.CurrencyCode = order.CuryID;
			request.CustomerCode = vend.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("PO.{0}.{1}", order.OrderType, order.OrderNbr);
			request.DocDate = order.ExpectedDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(loc.CAvalaraCustomerUsageType))
			{
				request.CustomerUsageType = loc.CAvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.PurchaseOrder;

			PXSelectBase<POLine> select = new PXSelectJoin<POLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<InventoryItem.salesAcctID>>>>,
				Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>>>,
				OrderBy<Asc<POLine.lineNbr>>>(this);

			//request.Discount = order.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<POLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				POLine tran = (POLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);
				line.Amount = mult * tran.CuryExtCost.GetValueOrDefault();
				line.Description = tran.TranDesc;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Math.Abs(Convert.ToDouble(tran.OrderQty.GetValueOrDefault()));
				line.Discounted = request.Discount > 0;
				line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

				if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
					line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;

				request.Lines.Add(line);
			}

			return request;
		}

		protected virtual GetTaxRequest BuildGetTaxRequestOpen(POOrder order)
		{
			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Vendor vend = (Vendor)vendor.View.SelectSingleBound(new object[] { order });
			Location loc = (Location)location.View.SelectSingleBound(new object[] { order });

			IAddressBase fromAddress = GetFromAddress(order);
			IAddressBase toAddress = GetToAddress(order);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, order.BranchID);
			request.CurrencyCode = order.CuryID;
			request.CustomerCode = vend.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("PO.{0}.{1}", order.OrderType, order.OrderNbr);
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(loc.CAvalaraCustomerUsageType))
			{
				request.CustomerUsageType = loc.CAvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}


			request.DocType = DocumentType.PurchaseOrder;

			PXSelectBase<POLine> select = new PXSelectJoin<POLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<InventoryItem.salesAcctID>>>>,
				Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>>>,
				OrderBy<Asc<POLine.lineNbr>>>(this);


			foreach (PXResult<POLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				POLine tran = (POLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				if (tran.OpenAmt > 0)
				{
					Line line = new Line();
					line.No = Convert.ToString(tran.LineNbr);
					line.Amount = mult*tran.CuryOpenAmt.GetValueOrDefault();
					line.Description = tran.TranDesc;
					line.DestinationAddress = request.DestinationAddress;
					line.OriginAddress = request.OriginAddress;
					line.ItemCode = item.InventoryCD;
					line.Qty = Convert.ToDouble(tran.BaseOpenQty.GetValueOrDefault());
					line.Discounted = request.Discount > 0;
					line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

					if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
						line.RevAcct = salesAccount.AccountCD;

					line.TaxCode = tran.TaxCategoryID;

					request.Lines.Add(line);
				}
			}

			return request;
		}

		protected bool SkipAvalaraTaxProcessing = false;
		protected virtual void ApplyAvalaraTax(POOrder order, GetTaxResult result, GetTaxResult resultOpen)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { order });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			//Clear all existing Tax transactions:
			foreach (PXResult<POTaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { order }))
			{
				POTaxTran taxTran = (POTaxTran)res;
				Taxes.Delete(taxTran);
			}

			this.Views.Caches.Add(typeof(Tax));

			for (int i = 0; i < result.TaxSummary.Count; i++)
			{
                string taxID = result.TaxSummary[i].TaxName;
				if (string.IsNullOrEmpty(taxID))
					taxID = result.TaxSummary[i].JurisCode;

				if (string.IsNullOrEmpty(taxID))
				{
					PXTrace.WriteInformation(Messages.EmptyValuesFromAvalara);
					continue;
				}

				//Insert Tax if not exists - just for the selectors sake
				Tax tx = PXSelect<Tax, Where<Tax.taxID, Equal<Required<Tax.taxID>>>>.Select(this, taxID);
				if (tx == null)
				{
					tx = new Tax();
					tx.TaxID = taxID;
					//tx.Descr = string.Format("Avalara {0} {1}%", taxID, Convert.ToDecimal(result.TaxSummary[i].Rate)*100);
					tx.Descr = string.Format("Avalara {0}", taxID);
					tx.TaxType = CSTaxType.Sales;
					tx.TaxCalcType = CSTaxCalcType.Doc;
					tx.TaxCalcLevel = avalaraSetup.Current.IsInclusiveTax == true ? CSTaxCalcLevel.Inclusive : CSTaxCalcLevel.CalcOnItemAmt;
					tx.TaxApplyTermsDisc = CSTaxTermsDiscount.ToTaxableAmount;
					tx.SalesTaxAcctID = vendor.SalesTaxAcctID;
					tx.SalesTaxSubID = vendor.SalesTaxSubID;
					tx.ExpenseAccountID = vendor.TaxExpenseAcctID;
					tx.ExpenseSubID = vendor.TaxExpenseSubID;
					tx.TaxVendorID = taxZone.TaxVendorID;
					tx.IsExternal = true;

					this.Caches[typeof(Tax)].Insert(tx);
				}

				POTaxTran tax = new POTaxTran();
				tax.OrderType = order.OrderType;
				tax.OrderNbr = order.OrderNbr;
				tax.TaxID = taxID;
				tax.CuryTaxAmt = Math.Abs(result.TaxSummary[i].Tax);
				tax.CuryTaxableAmt = Math.Abs(result.TaxSummary[i].Taxable);
				tax.TaxRate = Convert.ToDecimal(result.TaxSummary[i].Rate) * 100;
				tax.JurisType = result.TaxSummary[i].JurisType.ToString();
				tax.JurisName = result.TaxSummary[i].JurisName;

				Taxes.Insert(tax);
			}

			bool requireBlanketControlTotal = POSetup.Current.RequireBlanketControlTotal == true;
			bool requireDropShipControlTotal = POSetup.Current.RequireDropShipControlTotal == true;
			bool requireOrderControlTotal = POSetup.Current.RequireOrderControlTotal == true;

			if (order.Hold != true)
			{
				POSetup.Current.RequireBlanketControlTotal = false;
				POSetup.Current.RequireDropShipControlTotal = false;
				POSetup.Current.RequireOrderControlTotal = false;
			}

			try
			{
				Document.SetValueExt<POOrder.curyTaxTotal>(order, Math.Abs(result.TotalTax));
				if (resultOpen != null)
					Document.SetValueExt<POOrder.curyOpenTaxTotal>(order, Math.Abs(resultOpen.TotalTax));
			}
			finally
			{
				POSetup.Current.RequireBlanketControlTotal = requireBlanketControlTotal;
				POSetup.Current.RequireDropShipControlTotal = requireDropShipControlTotal;
				POSetup.Current.RequireOrderControlTotal = requireOrderControlTotal;
			}

			try
			{
				SkipAvalaraTaxProcessing = true;
				this.Save.Press();
			}
			finally
			{
				SkipAvalaraTaxProcessing = false;
			}
		}

		protected virtual void LogMessages(BaseResult result)
		{
			foreach (AvaMessage msg in result.Messages)
			{
				switch (result.ResultCode)
				{
					case SeverityLevel.Exception:
					case SeverityLevel.Error:
						PXTrace.WriteError(msg.Summary + ": " + msg.Details);
						break;
					case SeverityLevel.Warning:
						PXTrace.WriteWarning(msg.Summary + ": " + msg.Details);
						break;
				}
			}
		}

		protected virtual IAddressBase GetToAddress(POOrder order)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(order.BranchID))
				return (Address)res;

			return null;
		}

		protected virtual CRLocation GetBranchLocation(POOrder order)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, CRLocation> res in select.Select(order.BranchID))
				return (CRLocation)res;

			return null;
		}

		protected virtual IAddressBase GetFromAddress(POOrder order)
		{
			Address vendorAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, vendor.Current.DefAddressID);

			return vendorAddress;
		}

		protected virtual string GetAvalaraLocationCode(POOrder order)
		{
			return null;
		}


		#endregion

        #region Discounts
        protected virtual void RecalculateDiscounts(PXCache sender, POLine line)
        {
            if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && line.InventoryID != null && line.OrderQty != null && line.CuryLineAmt != null)
            {
                DiscountEngine<POLine>.SetDiscounts<POOrderDiscountDetail>(sender, Transactions, line, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.CuryID, Document.Current.OrderDate, true, recalcdiscountsfilter.Current);

                RecalculateTotalDiscount();
            }
        }

        private void RecalculateTotalDiscount()
        {
            if (Document.Current != null)
            {
                POOrder old_row = PXCache<POOrder>.CreateCopy(Document.Current);
                Document.Cache.SetValueExt<POOrder.curyDiscTot>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<POOrderDiscountDetail>(DiscountDetails));
                Document.Cache.RaiseRowUpdated(Document.Current, old_row);
            }
        }
		#endregion

		public override void Persist()
		{
			DiscountEngine<POLine>.ValidateDiscountDetails(DiscountDetails);

			foreach (POOrder doc in Document.Cache.Deleted)
			{
				APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
				graph.Document.Current = PXSelect<APInvoice, Where<APInvoice.refNbr, Equal<Required<POOrder.prepaymentRefNbr>>, And<APInvoice.docType, Equal<Required<POOrder.prepaymentDocType>>>>>.Select(graph, doc.PrepaymentRefNbr, doc.PrepaymentDocType);
				if (graph.Document.Current != null && graph.Document.Current.ReleasedOrPrebooked == false && graph.Document.Current.Scheduled == false)
					graph.Delete.Press();
			}
			foreach (POOrder doc in Document.Cache.Updated)
			{
				try
				{
					UpdatePrepayment(doc);
				}
				catch (Exception ex)
				{
					Document.Cache.RaiseExceptionHandling<POOrder.prepaymentRefNbr>(doc, doc.PrepaymentRefNbr, new PXSetPropertyException(Messages.PrepaymentNotUpdated + ex.Message, PXErrorLevel.Warning));
				}
			}

			if (POSetup.Current.VendorPriceUpdate == POVendorPriceUpdateType.Purchase && this.Document.Current != null && this.Document.Current.UpdateVendorCost != false)
			{
				List<POLine> linesToUpdateItemCost = new List<POLine>();
				foreach (POLine row in Transactions.Cache.Cached)
				{
					if ((Transactions.Cache.GetStatus(row) == PXEntryStatus.Inserted || Transactions.Cache.GetStatus(row) == PXEntryStatus.Updated) &&
					row.InventoryID != null && POOrderType.IsUseBlanket(row.OrderType) && row.CuryUnitCost > 0)
					{
						linesToUpdateItemCost.Add(row);
					}
				}
				linesToUpdateItemCost.Sort((x, y) => ((int)x.LineNbr).CompareTo((int)y.LineNbr));

				foreach (POLine row in linesToUpdateItemCost)
				{
					POItemCostManager.Update(this,
						this.Document.Current.VendorID,
						this.Document.Current.VendorLocationID,
						this.Document.Current.CuryID,
						row.InventoryID,
						row.SubItemID,
						row.UOM,
						row.CuryUnitCost.Value);
				}
			}

			SOOrderEntry.ProcessPOOrder(this, Document.Current);

			base.Persist();

			if (Document.Current != null && IsExternalTax == true && !SkipAvalaraTaxProcessing && Document.Current.IsTaxValid != true)
			{
				PXLongOperation.StartOperation(this, delegate ()
				{
					POOrder doc = new POOrder();
					doc.OrderType = Document.Current.OrderType;
					doc.OrderNbr = Document.Current.OrderNbr;
					POExternalTaxCalc.Process(doc);
				});
			}
		}
	}

	[PXHidden()]
	[PXProjection(typeof(Select4<POVendorInventory, Where<POVendorInventory.vendorID, Equal<CurrentValue<POSiteStatusFilter.vendorID>>>, Aggregate<GroupBy<POVendorInventory.vendorID, GroupBy<POVendorInventory.inventoryID, GroupBy<POVendorInventory.subItemID>>>>>), Persistent = false)]
	public class POVendorInventoryOnly : IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.vendorID))]
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
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.inventoryID))]
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
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.subItemID))]
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
	}

	public class POSiteStatusLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
		where StatusFilter : INSiteStatusFilter, new()
		where Status : class, IBqlTable, new()
	{
		public POSiteStatusLookup(PXGraph graph)
			:base(graph)
		{ 
		}

		public POSiteStatusLookup(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
		}

		protected override PXView CreateIntView(PXGraph graph)
		{
			PXCache cache = graph.Caches[typeof(Status)];
			PXCache fcache = graph.Caches[typeof(StatusFilter)];

			List<Type> select = new List<Type>();

			if (fcache.Current != null && ((StatusFilter)fcache.Current).OnlyAvailable == true)
			{
				select.Add(typeof(Select2<,,>));
				select.Add(typeof(Status));
				select.Add(typeof(InnerJoin<POVendorInventoryOnly, 
					On<POVendorInventoryOnly.inventoryID, Equal<POSiteStatusSelected.inventoryID>, 
						And<POVendorInventoryOnly.vendorID, Equal<Current<POSiteStatusFilter.vendorID>>,
						And<Where<POVendorInventoryOnly.subItemID, Equal<POSiteStatusSelected.subItemID>, Or<POSiteStatusSelected.subItemID, IsNull>>>>>>));
				select.Add(CreateWhere(graph));

				Type selectType = BqlCommand.Compose(select.ToArray());

				return new LookupView(graph, BqlCommand.CreateInstance(selectType));
			}

			return base.CreateIntView(graph);
		}
	}

    [Serializable]
	public partial class POSiteStatusFilter : INSiteStatusFilter
	{
		#region OnlyAvailable
		public new abstract class onlyAvailable : PX.Data.IBqlField
		{
		}
		[PXBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Only Vendor's Items")]
		public override bool? OnlyAvailable
		{
			get
			{
				return base._OnlyAvailable;
			}
			set
			{
				base._OnlyAvailable = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt]
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
	}

	[System.SerializableAttribute()]
	[PXProjection(typeof(Select2<InventoryItem,
        LeftJoin<INSiteStatus,
                        On<INSiteStatus.inventoryID, Equal<InventoryItem.inventoryID>>,
		LeftJoin<INSubItem,
                        On<INSubItem.subItemID, Equal<INSiteStatus.subItemID>>,
		LeftJoin<INSite,
						On<INSite.siteID, Equal<INSiteStatus.siteID>>,
		LeftJoin<INItemXRef,
						On<INItemXRef.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INItemXRef.alternateType, Equal<INAlternateType.barcode>,
						And<Where<INItemXRef.subItemID, Equal<INSiteStatus.subItemID>,
							Or<INSiteStatus.subItemID, IsNull>>>>>,
	    LeftJoin<INItemPartNumber,
						On<INItemPartNumber.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INItemPartNumber.alternateID, Like<CurrentValue<POSiteStatusFilter.inventory_Wildcard>>,						
						And2<Where<INItemPartNumber.bAccountID, Equal<Zero>, 
						    Or<INItemPartNumber.bAccountID, Equal<CurrentValue<POOrder.vendorID>>,
							Or<INItemPartNumber.alternateType, Equal<INAlternateType.cPN>>>>, 										
						And<Where<INItemPartNumber.subItemID, Equal<INSiteStatus.subItemID>,
						    Or<INSiteStatus.subItemID, IsNull>>>>>>,
		LeftJoin<INItemClass,
						On<INItemClass.itemClassID, Equal<InventoryItem.itemClassID>>,
		LeftJoin<INPriceClass,
						On<INPriceClass.priceClassID, Equal<InventoryItem.priceClassID>>,
		LeftJoin<Vendor,
						On<Vendor.bAccountID, Equal<InventoryItem.preferredVendorID>>,
		LeftJoin<INUnit,
					    On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
							  And<INUnit.unitType, Equal<int1>,
				        And<INUnit.fromUnit, Equal<InventoryItem.purchaseUnit>,
				        And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>>>>>>>>>>>>,
		Where2<CurrentMatch<InventoryItem, AccessInfo.userName>,
			 And2<Where<INSiteStatus.siteID, IsNull, Or<INSite.branchID, IsNotNull, And<CurrentMatch<INSite, AccessInfo.userName>>>>,
			 And2<Where<INSiteStatus.subItemID, IsNull,
				Or<CurrentMatch<INSubItem, AccessInfo.userName>>>,
			 And<InventoryItem.stkItem, Equal<boolTrue>,
			 And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
			 And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noPurchases>>>>>>>>), Persistent = false)]
	public partial class POSiteStatusSelected : IBqlTable
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

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
		[PXDefault()]
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

		#region InventoryCD
		public abstract class inventoryCD : PX.Data.IBqlField
		{
		}
		protected string _InventoryCD;
		[PXDefault()]
		[InventoryRaw(BqlField = typeof(InventoryItem.inventoryCD))]
		public virtual String InventoryCD
		{
			get
			{
				return this._InventoryCD;
			}
			set
			{
				this._InventoryCD = value;
			}
		}
		#endregion

		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}

		protected string _Descr;
		[PXDBString(60, IsUnicode = true, BqlField = typeof(InventoryItem.descr))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion

		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected string _ItemClassID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.itemClassID))]
		[PXUIField(DisplayName = "Item Class ID", Visible = false)]
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

		#region ItemClassDescription
		public abstract class itemClassDescription : PX.Data.IBqlField
		{
		}
		protected String _ItemClassDescription;
		[PXDBString(250, IsUnicode = true, BqlField = typeof(INItemClass.descr))]
		[PXUIField(DisplayName = "Item Class Description", Visible = false)]
		public virtual String ItemClassDescription
		{
			get
			{
				return this._ItemClassDescription;
			}
			set
			{
				this._ItemClassDescription = value;
			}
		}
		#endregion

		#region PriceClassID
		public abstract class priceClassID : PX.Data.IBqlField
		{
		}

		protected string _PriceClassID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.priceClassID))]
		[PXUIField(DisplayName = "Price Class ID", Visible = false)]
		public virtual String PriceClassID
		{
			get
			{
				return this._PriceClassID;
			}
			set
			{
				this._PriceClassID = value;
			}
		}
		#endregion

		#region PriceClassDescription
		public abstract class priceClassDescription : PX.Data.IBqlField
		{
		}
		protected String _PriceClassDescription;
		[PXDBString(250, IsUnicode = true, BqlField = typeof(INPriceClass.description))]
		[PXUIField(DisplayName = "Price Class Description", Visible = false)]
		public virtual String PriceClassDescription
		{
			get
			{
				return this._PriceClassDescription;
			}
			set
			{
				this._PriceClassDescription = value;
			}
		}
		#endregion

		#region PreferredVendorID
		public abstract class preferredVendorID : PX.Data.IBqlField
		{
		}

		protected Int32? _PreferredVendorID;
		[AP.Vendor(DisplayName = "Preferred Vendor ID", Required = false, DescriptionField = typeof(AP.Vendor.acctName), BqlField = typeof(InventoryItem.preferredVendorID), Visible = false)]
		public virtual Int32? PreferredVendorID
		{
			get
			{
				return this._PreferredVendorID;
			}
			set
			{
				this._PreferredVendorID = value;
			}
		}
		#endregion

		#region PreferredVendorDescription
		public abstract class preferredVendorDescription : PX.Data.IBqlField
		{
		}
		protected String _PreferredVendorDescription;
		[PXDBString(250, IsUnicode = true, BqlField = typeof(Vendor.acctName))]
		[PXUIField(DisplayName = "Preferred Vendor Name", Visible = false)]
		public virtual String PreferredVendorDescription
		{
			get
			{
				return this._PreferredVendorDescription;
			}
			set
			{
				this._PreferredVendorDescription = value;
			}
		}
		#endregion

		#region BarCode
		public abstract class barCode : PX.Data.IBqlField
		{
		}
		protected String _BarCode;
		[PXDBString(255, BqlField = typeof(INItemXRef.alternateID))]
		//[PXUIField(DisplayName = "Barcode")]
		public virtual String BarCode
		{
			get
			{
				return this._BarCode;
			}
			set
			{
				this._BarCode = value;
			}
		}
		#endregion

		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[PXDBString(225, IsUnicode = true, InputMask = "", BqlField = typeof(INItemPartNumber.alternateID))]
		[PXUIField(DisplayName = "Alternate ID")]
		[PXExtraKey]
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

		#region AlternateType
		public abstract class alternateType : PX.Data.IBqlField
		{
		}
		protected String _AlternateType;
		[PXDBString(4, BqlField = typeof(INItemPartNumber.alternateType))]
		[INAlternateType.List()]
		[PXDefault(INAlternateType.Global)]
		[PXUIField(DisplayName = "Alternate Type")]
		public virtual String AlternateType
		{
			get
			{
				return this._AlternateType;
			}
			set
			{
				this._AlternateType = value;
			}
		}
		#endregion

		#region Descr
		public abstract class alternateDescr : PX.Data.IBqlField
		{
		}
		protected String _AlternateDescr;
		[PXDBString(60, IsUnicode = true, BqlField = typeof(INItemPartNumber.descr))]
		[PXUIField(DisplayName = "Alternate Description", Visible = false)]
		public virtual String AlternateDescr
		{
			get
			{
				return this._AlternateDescr;
			}
			set
			{
				this._AlternateDescr = value;
			}
		}
		#endregion		

		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected int? _SiteID;
		[PXUIField(DisplayName = "Site")]
		[SiteAttribute(BqlField = typeof(INSiteStatus.siteID), IsKey = true)]
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

        #region SiteCD
        public abstract class siteCD : PX.Data.IBqlField
        {
        }
        protected String _SiteCD;
        [PXDBString(IsUnicode = true, BqlField = typeof(INSite.siteCD))]
		[PXDimension(SiteAttribute.DimensionName)]
		public virtual String SiteCD
        {
            get
            {
                return this._SiteCD;
            }
            set
            {
                this._SiteCD = value;
            }
        }
        #endregion

        #region SubItemID
        public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected int? _SubItemID;
		[SubItem(typeof(POSiteStatusSelected.inventoryID),BqlField = typeof(INSubItem.subItemID), IsKey = true)]
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

		#region SubItemCD
		public abstract class subItemCD : PX.Data.IBqlField
		{
		}
		protected String _SubItemCD;
		[PXDBString(IsUnicode = true, BqlField = typeof(INSubItem.subItemCD))]
		[PXDimension(SubItemAttribute.DimensionName)]
		public virtual String SubItemCD
		{
			get
			{
				return this._SubItemCD;
			}
			set
			{
				this._SubItemCD = value;
			}
		}
		#endregion

		#region BaseUnit
		public abstract class baseUnit : PX.Data.IBqlField
		{
		}

		protected string _BaseUnit;
		[PXDefault(typeof(Search<INItemClass.baseUnit, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible, BqlField = typeof(InventoryItem.baseUnit))]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}
		#endregion

		#region PurchaseUnit
		public abstract class purchaseUnit : PX.Data.IBqlField
		{
		}
		protected string _PurchaseUnit;
		[INUnit(typeof(POSiteStatusSelected.inventoryID), DisplayName = "Purchase Unit", BqlField = typeof(InventoryItem.purchaseUnit))]
		public virtual String PurchaseUnit
		{
			get
			{
				return this._PurchaseUnit;
			}
			set
			{
				this._PurchaseUnit = value;
			}
		}
		#endregion

		#region QtySelected
		public abstract class qtySelected : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySelected;
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Selected")]
		public virtual Decimal? QtySelected
		{
			get
			{
				return this._QtySelected ?? 0m;
			}
			set
			{
				if (value != null && value != 0m)
					this._Selected = true;
				this._QtySelected = value;
			}
		}
		#endregion

		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity(BqlField = typeof(INSiteStatus.qtyOnHand))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion

		#region QtyOnHandExt
		public abstract class qtyOnHandExt : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHandExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatus.qtyOnHand, INUnit.unitRate>>,
			Div<INSiteStatus.qtyOnHand, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHandExt
		{
			get
			{
				return this._QtyOnHandExt;
			}
			set
			{
				this._QtyOnHandExt = value;
			}
		}
		#endregion

		#region QtyAvail
		public abstract class qtyAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyAvail;
		[PXDBQuantity(BqlField = typeof(INSiteStatus.qtyAvail))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvail
		{
			get
			{
				return this._QtyAvail;
			}
			set
			{
				this._QtyAvail = value;
			}
		}
		#endregion

		#region QtyAvailExt
		public abstract class qtyAvailExt : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyAvailExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatus.qtyAvail, INUnit.unitRate>>,
			Div<INSiteStatus.qtyAvail, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvailExt
		{
			get
			{
				return this._QtyAvailExt;
			}
			set
			{
				this._QtyAvailExt = value;
			}
		}
		#endregion

		#region QtyPOPrepared
		public abstract class qtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOPrepared;
		[PXDBQuantity(BqlField = typeof(INSiteStatus.qtyPOPrepared))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Prepared")]
		public virtual Decimal? QtyPOPrepared
		{
			get
			{
				return this._QtyPOPrepared;
			}
			set
			{
				this._QtyPOPrepared = value;
			}
		}
		#endregion

		#region QtyPOPreparedExt
		public abstract class qtyPOPreparedExt : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOPreparedExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
				Mult<INSiteStatus.qtyPOPrepared, INUnit.unitRate>>,
				Div<INSiteStatus.qtyPOPrepared, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Prepared")]
		public virtual Decimal? QtyPOPreparedExt
		{
			get
			{
				return this._QtyPOPreparedExt;
			}
			set
			{
				this._QtyPOPreparedExt = value;
			}
		}
		#endregion

		#region QtyPOOrders
		public abstract class qtyPOOrders : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOOrders;
		[PXDBQuantity(BqlField = typeof(INSiteStatus.qtyPOOrders))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Orders")]
		public virtual Decimal? QtyPOOrders
		{
			get
			{
				return this._QtyPOOrders;
			}
			set
			{
				this._QtyPOOrders = value;
			}
		}
		#endregion

		#region QtyPOOrdersExt
		public abstract class qtyPOOrdersExt : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOOrdersExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
				Mult<INSiteStatus.qtyPOOrders, INUnit.unitRate>>,
				Div<INSiteStatus.qtyPOOrders, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Orders")]
		public virtual Decimal? QtyPOOrdersExt
		{
			get
			{
				return this._QtyPOOrdersExt;
			}
			set
			{
				this._QtyPOOrdersExt = value;
			}
		}
		#endregion

		#region QtyPOReceipts
		public abstract class qtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOReceipts;
		[PXDBQuantity(BqlField = typeof(INSiteStatus.qtyPOReceipts))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Receipts")]
		public virtual Decimal? QtyPOReceipts
		{
			get
			{
				return this._QtyPOReceipts;
			}
			set
			{
				this._QtyPOReceipts = value;
			}
		}
		#endregion

		#region QtyPOReceiptsExt
		public abstract class qtyPOReceiptsExt : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOReceiptsExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatus.qtyPOReceipts, INUnit.unitRate>>,
			Div<INSiteStatus.qtyPOReceipts, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Receipts")]
		public virtual Decimal? QtyPOReceiptsExt
		{
			get
			{
				return this._QtyPOReceiptsExt;
			}
			set
			{
				this._QtyPOReceiptsExt = value;
			}
		}
		#endregion
	}

}
