using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.EP;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine4 = PX.Objects.SO.SOLine4;
using PX.Objects.SO;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using System.Linq;
using CRLocation = PX.Objects.CR.Standalone.Location;
using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;
using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
using LotSerialStatus = PX.Objects.IN.Overrides.INDocumentRelease.LotSerialStatus;
using ItemLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.ItemLotSerial;
using SiteLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.SiteLotSerial;

namespace PX.Objects.PO
{   
    [Serializable]
	public class POReceiptEntry : PXGraph<POReceiptEntry, POReceipt>
	{
		#region DAC Overrides
		[Account(typeof(POReceiptLine.branchID), DisplayName = "Accrual Account", Filterable = false, DescriptionField = typeof(Account.description), Visible = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void POReceiptLine_POAccrualAcctID_CacheAttached(PXCache sender) { }

		[SubAccount(typeof(POReceiptLine.branchID), typeof(POReceiptLine.pOAccrualAcctID), DisplayName = "Accrual Sub.", Filterable = true, Visible = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void POReceiptLine_POAccrualSubID_CacheAttached(PXCache sender) { }

		[PXDBInt()]
		[PXDBLiteDefault(typeof(SOAddress.addressID))]
		protected virtual void SOOrderShipment_ShipAddressID_CacheAttached(PXCache sender) { }

        #region SiteID
        [IN.Site(DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]
        protected virtual void INRegister_SiteID_CacheAttached(PXCache sender) { }
		#endregion
        #region ToSiteID
        [IN.ToSite(DisplayName = "To Warehouse", DescriptionField = typeof(INSite.descr))]
        protected virtual void INRegister_ToSiteID_CacheAttached(PXCache sender) { }
        #endregion
        #endregion

        #region Selects
        [PXViewName(Messages.POReceipt)]
		public PXSelectJoin<POReceipt,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>>,
			Where<POReceipt.receiptType, Equal<Optional<POReceipt.receiptType>>,
			And<Where<Vendor.bAccountID, IsNull,
			Or<Match<Vendor, Current<AccessInfo.userName>>>>>>> Document;
		public PXSelect<POReceipt, Where<POReceipt.receiptType, Equal<Current<POReceipt.receiptType>>,
				And<POReceipt.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>> CurrentDocument;
		[PXViewName(Messages.POReceiptLine)]
		public PXSelect<POReceiptLine, Where<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>> transactions;

        [PXCopyPasteHiddenView()]
		public PXSelect<POReceiptLineSplit, Where<POReceiptLineSplit.receiptNbr, Equal<Current<POReceiptLine.receiptNbr>>,
											And<POReceiptLineSplit.lineNbr, Equal<Current<POReceiptLine.lineNbr>>,
											And<Where<POLineType.goodsForInventory, Equal<Current<POReceiptLine.lineType>>,
												Or<POLineType.goodsForSalesOrder, Equal<Current<POReceiptLine.lineType>>,
												Or<POLineType.goodsForDropShip, Equal<Current<POReceiptLine.lineType>>>>>>>>> splits;
        [PXCopyPasteHiddenView]
        public PXSelect<POReceiptDiscountDetail, Where<POReceiptDiscountDetail.receiptType, Equal<Current<POReceipt.receiptType>>, And<POReceiptDiscountDetail.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>, OrderBy<Asc<POReceiptDiscountDetail.receiptType, Asc<POReceiptDiscountDetail.receiptNbr>>>> DiscountDetails;

		public PXSetup<POSetup> posetup;
		public CMSetupSelect cmsetup;
		public PXSetup<APSetup> apsetup;
		public PXSetupOptional<INSetup> insetup;
	    public PXSetupOptional<CommonSetup> commonsetup; 
		public PXSetup<Branch, Where<Branch.bAccountID, Equal<Optional<POReceipt.vendorID>>>> company;

		[PXViewName(AP.Messages.Vendor)]
		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<POReceipt.vendorID>>>> vendor;
		[PXViewName(AP.Messages.VendorClass)]
		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;
		
		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<POReceipt.taxZoneID>>>> taxzone;
		[PXViewName(AP.Messages.VendorLocation)]
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<POReceipt.vendorID>>, And<Location.locationID, Equal<Optional<POReceipt.vendorLocationID>>>>> location;

		public PXSelect<POReceiptTax, Where<POReceiptTax.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>, OrderBy<Asc<POReceiptTax.receiptNbr, Asc<POReceiptTax.taxID>>>> Tax_Rows;
		public TaxTranSelect<POReceipt, POReceipt.termsID, POReceiptTaxTran, POReceiptTaxTran.taxID,
			Where<POReceiptTaxTran.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>> Taxes;
		public PXSelect<POOrderReceipt, Where<POOrderReceipt.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>> ReceiptOrders;

		public PXSelect<POLineR> purchaseLinesUPD;
		public PXSelect<POLineUOpen> poLinesOpenUPD;
		public PXSelect<POOrder> poOrderUPD;
		public PXSelect<POTax> poOrderTaxUPD;
		public PXSelect<POTaxTran> poOrderTaxTranUPD;

		public PXSelect<POItemCostManager.POVendorInventoryPriceUpdate> priceStatus;
		public PXSelect<ReasonCode, Where<ReasonCode.reasonCodeID, Equal<Required<ReasonCode.reasonCodeID>>>> reasonCode;
		[PXCopyPasteHiddenFields(typeof(LandedCostTran.aPDocType), typeof(LandedCostTran.aPRefNbr), typeof(LandedCostTran.aPCuryInfoID), typeof(LandedCostTran.iNDocType), typeof(LandedCostTran.iNRefNbr))]
		public PXSelect<LandedCostTran, Where<LandedCostTran.pOReceiptNbr, Equal<Current<POReceipt.receiptNbr>>,
			And<LandedCostTran.source, Equal<LandedCostTranSource.fromPO>>>> landedCostTrans;

		public PXSelect<CurrencyInfo> currencyinfo;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<LandedCostTran.curyInfoID>>>> CurInfoLC;

		public LSPOReceiptLine lsselect;

		public PXSelect<SOLine4> solineselect;
        public PXSelect<SOLineSplit> solinesplitselect;
		public PXSelect<SO.SOTax> sotaxselect;
		public PXSelect<SO.SOTaxTran> sotaxtranselect;
		public PXSelect<SOOrder> soorderselect;
		public PXSelect<SOAddress> soaddressselect;
		public PXSelect<SOOrderShipment> ordershipmentselect;
		
		public PXSelect<POLine> poline;
		public PXSelect<INItemXRef> xrefs;
		public PXSetupOptional<TXAvalaraSetup> avalaraSetup;
        public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;

		#region Add PO Order sub-form
		public PXFilter<POOrderFilter> filter;
		public PXFilter<POReceiptLineS> addReceipt;

		[PXCopyPasteHiddenView()]
		public PXSelectJoin<POLineS,
				InnerJoin<POOrder, 
						   On<POOrder.orderType, Equal<POReceiptEntry.POLineS.orderType>, 
						  And<POOrder.orderNbr, Equal<POReceiptEntry.POLineS.orderNbr>>>>,
			Where<POLineS.orderType, Equal<Current<POOrderFilter.orderType>>,				
				And<POLineS.lineType, NotEqual<POLineType.service>,
				And<POLineS.lineType, NotEqual<POLineType.description>,
				And2<Where<Current<POOrderFilter.vendorID>, IsNull,
								Or<POLineS.vendorID, Equal<Current<POOrderFilter.vendorID>>>>,
				And2<Where<Current<POOrderFilter.vendorLocationID>, IsNull,
								Or<POOrder.vendorLocationID, Equal<Current<POOrderFilter.vendorLocationID>>>>,
				And2<Where<Current<POOrderFilter.orderNbr>, IsNull,
								Or<POLineS.orderNbr, Equal<Current<POOrderFilter.orderNbr>>>>,
				And2<Where<Current<POOrderFilter.inventoryID>, IsNull,
								Or<POLineS.inventoryID, Equal<Current<POOrderFilter.inventoryID>>>>,
                And<Where<Current<POOrderFilter.subItemID>, IsNull,
                                Or<POLineS.subItemID, Equal<Current<POOrderFilter.subItemID>>>>>>>>>>>>> poLinesSelection;
		
		[PXCopyPasteHiddenView()]
        public PXSelectJoin<INTran,
            InnerJoin<INRegister, On<INRegister.docType, Equal<INTran.docType>, And<INRegister.refNbr, Equal<INTran.refNbr>>>,
            InnerJoin<INSite, On<INSite.siteID, Equal<INRegister.toSiteID>>,
            InnerJoin<INTranInTransit, On<INTranInTransit.tranType, Equal<INTran.tranType>, And<INTranInTransit.refNbr, Equal<INTran.refNbr>, And<INTranInTransit.lineNbr, Equal<INTran.lineNbr>>>>,
            LeftJoin<POReceiptLine, On<POReceiptLine.origTranType, Equal<INTranType.transfer>, And<POReceiptLine.origRefNbr, Equal<INRegister.refNbr>, And<POReceiptLine.released, Equal<False>>>>>>>>,
            Where<INRegister.origModule, Equal<GL.BatchModule.moduleSO>,
                And<INRegister.docType, Equal<INDocType.transfer>,
                And<INRegister.released, Equal<True>,
                And<POReceiptLine.receiptNbr, IsNull,
                And2<Where<Current<POReceipt.siteID>, IsNull,
                    Or<INRegister.toSiteID, Equal<Current<POReceipt.siteID>>>>,
                And2<Match<INSite, Current<AccessInfo.userName>>,
                And2<Where<Current<POOrderFilter.shipFromSiteID>, IsNull,
                    Or<INRegister.siteID, Equal<Current<POOrderFilter.shipFromSiteID>>>>,
                And2<Where<Current<POOrderFilter.sOOrderNbr>, IsNull,
                                Or<INTran.sOOrderType, Equal<SOOrderTypeConstants.transferOrder>, And<INTran.sOOrderNbr, Equal<Current<POOrderFilter.sOOrderNbr>>>>>,
                And2<Where<Current<POOrderFilter.inventoryID>, IsNull,
                                Or<INTran.inventoryID, Equal<Current<POOrderFilter.inventoryID>>>>,
                And<Where<Current<POOrderFilter.subItemID>, IsNull,
                                Or<INTran.subItemID, Equal<Current<POOrderFilter.subItemID>>>>>>>>>>>>>>> intranSelection;

        [PXCopyPasteHiddenView()]
		public PXSelect<POOrderS,
								Where<POOrderS.hold, Equal<boolFalse>,
								And2<Where<Current<POReceipt.vendorID>, IsNull,
								     Or<POOrderS.vendorID, Equal<Current<POReceipt.vendorID>>>>,
								And2<Where<Current<POReceipt.vendorLocationID>, IsNull,
										 Or<POOrderS.vendorLocationID, Equal<Current<POReceipt.vendorLocationID>>>>,
								And2<Where<Current<POOrderFilter.vendorID>, IsNull,
										 Or<POOrderS.vendorID, Equal<Current<POOrderFilter.vendorID>>>>,
								And2<Where<Current<POOrderFilter.vendorLocationID>, IsNull,
										 Or<POOrderS.vendorLocationID, Equal<Current<POOrderFilter.vendorLocationID>>>>,
								And2<Where<POOrderS.curyID, Equal<Current<POReceipt.curyID>>,
									Or<Current<POOrderFilter.anyCurrency>, Equal<boolTrue>>>,								
								And2<Where2<Where<Current<POOrderFilter.orderType>, IsNull,
											 And<Where<POOrderS.orderType, Equal<POOrderType.regularOrder>,
															Or<POOrderS.orderType, Equal<POOrderType.dropShip>>>>>,
										Or<POOrderS.orderType, Equal<Current<POOrderFilter.orderType>>>>,
								And2<Where<POOrderS.shipToBAccountID, Equal<Current<POOrderFilter.shipToBAccountID>>,
									Or<Current<POOrderFilter.shipToBAccountID>, IsNull>>,
								And<Where<POOrderS.shipToLocationID, Equal<Current<POOrderFilter.shipToLocationID>>,
								Or<Current<POOrderFilter.shipToLocationID>, IsNull>>>>>>>>>>>,			
								OrderBy<Asc<POOrderS.orderDate>>> openOrders;


		[PXCopyPasteHiddenView()]
		public PXSelectJoin<SOOrderShipment, InnerJoin<INRegister, On<INRegister.docType, Equal<SOOrderShipment.invtDocType>, And<INRegister.refNbr, Equal<SOOrderShipment.invtRefNbr>>>>,
				Where<INRegister.origModule, Equal<GL.BatchModule.moduleSO>, And<INRegister.docType, Equal<INDocType.transfer>, And<INRegister.released, Equal<True>>>>> openTransfers;

		[PXCopyPasteHiddenView()]
		public PXSelect<INRegister> dummyOpenTransfers; //used only for CacheAttached

		public IEnumerable OpenTransfers()
		{
			Dictionary<POReceiptLine, int> usedTransfer = new Dictionary<POReceiptLine, int>(new SOOrderShipmentComparer());

			int count;
			foreach (POReceiptLine receiptLine in transactions.Cache.Inserted)
			{
				if (receiptLine.OrigRefNbr != null)
				{
					usedTransfer.TryGetValue(receiptLine, out count);
					usedTransfer[receiptLine] = count + 1;
				}
			}

			foreach (POReceiptLine receiptLine in transactions.Cache.Deleted)
			{
				if (receiptLine.OrigRefNbr != null && transactions.Cache.GetStatus(receiptLine) != PXEntryStatus.InsertedDeleted)
				{
					usedTransfer.TryGetValue(receiptLine, out count);
					usedTransfer[receiptLine] = count - 1;
				}
			}

			BqlCommand selectSites = BqlCommand.CreateInstance(typeof(Select<INSite, Where<MatchWithBranch<INSite.branchID>>>));
			PXResultset<SOOrderShipment> results;
			PXCache cache = this.Caches[typeof(INSite)];
			using (new PXReadBranchRestrictedScope())
			{ 
				results = PXSelectJoinGroupBy<SOOrderShipment,
					InnerJoin<INRegister, On<INRegister.docType, Equal<SOOrderShipment.invtDocType>, And<INRegister.refNbr, Equal<SOOrderShipment.invtRefNbr>>>,
					InnerJoin<INTran, On<INTran.docType, Equal<INRegister.docType>, And<INTran.refNbr, Equal<INRegister.refNbr>, And<INTran.sOOrderType, Equal<SOOrderShipment.orderType>, And<INTran.sOOrderNbr, Equal<SOOrderShipment.orderNbr>, And<INTran.sOShipmentType, Equal<SOOrderShipment.shipmentType>, And<INTran.sOShipmentNbr, Equal<SOOrderShipment.shipmentNbr>>>>>>>,
				InnerJoin<INSite, On<INSite.siteID, Equal<INRegister.toSiteID>>,
				InnerJoin<INTransferInTransit, On<INTransferInTransit.refNoteID, Equal<INRegister.noteID>>,
					LeftJoin<POReceiptLine, On<POReceiptLine.origTranType, Equal<INTranType.transfer>, And<POReceiptLine.origRefNbr, Equal<INRegister.refNbr>, And<POReceiptLine.origLineNbr, Equal<INTran.lineNbr>, And<POReceiptLine.released, Equal<False>>>>>>>>>>,
			Where<INRegister.origModule, Equal<GL.BatchModule.moduleSO>,
				And<INRegister.docType, Equal<INDocType.transfer>,
				And<INRegister.released, Equal<True>,
				And<POReceiptLine.receiptNbr, IsNull,
				And2<Match<INSite, Current<AccessInfo.userName>>,
				And2<Where<Current<POReceipt.siteID>, IsNull,
					Or<INRegister.toSiteID, Equal<Current<POReceipt.siteID>>>>,
				And<Where<Current<POOrderFilter.shipFromSiteID>, IsNull,
						Or<INRegister.siteID, Equal<Current<POOrderFilter.shipFromSiteID>>>>>>>>>>>,
					Aggregate<GroupBy<SOOrderShipment.shipmentNbr, GroupBy<SOOrderShipment.orderNbr, GroupBy<SOOrderShipment.orderType, Count>>>>>.Select(this);
			}

			foreach (var result in results)
			{
				if (selectSites.Meet(cache, PXResult.Unwrap<INSite>(result)))
				{
					POReceiptLine receiptLine = new POReceiptLine();
					receiptLine.SOOrderType = ((SOOrderShipment)result).OrderType;
					receiptLine.SOOrderNbr = ((SOOrderShipment)result).OrderNbr;
					receiptLine.SOShipmentNbr = ((SOOrderShipment)result).ShipmentNbr;
					if (usedTransfer.TryGetValue(receiptLine, out count))
					{
						usedTransfer.Remove(receiptLine);
						if (count < result.RowCount)
							yield return new PXResult<SOOrderShipment, INRegister>((SOOrderShipment)result, PXResult.Unwrap<INRegister>(result));
			}
					else
						yield return new PXResult<SOOrderShipment, INRegister>((SOOrderShipment)result, PXResult.Unwrap<INRegister>(result));

				}
			}

			foreach (POReceiptLine deletedTran in usedTransfer.Where(_ => _.Value < 0).Select(_ => _.Key))
			{
				yield return (PXResult<SOOrderShipment, INRegister>)
					PXSelectJoin<SOOrderShipment, InnerJoin<INRegister, On<INRegister.docType, Equal<SOOrderShipment.invtDocType>, And<INRegister.refNbr, Equal<SOOrderShipment.invtRefNbr>>>>,
					Where<SOOrderShipment.invtRefNbr, Equal<Required<SOOrderShipment.invtRefNbr>>,
					And<SOOrderShipment.invtDocType, Equal<INDocType.transfer>, 
					And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>,
					And<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>>>>>>.Select(this, deletedTran.OrigRefNbr, deletedTran.SOOrderNbr, deletedTran.SOOrderType);
			}


		}

	    #endregion

		#region Add Receipt sub-form
		#endregion
		#endregion

		#region Custom Buttons
		public ToggleCurrency<POReceipt> CurrencyView;

		public new PXAction<POReceipt> Cancel;
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		[PXCancelButton]
		protected virtual IEnumerable cancel(PXAdapter adapter)
		{
			//Reports an error if user enters number of existing document with different type
			PXResult<POReceipt, Vendor> row = null;
			foreach (PXResult<POReceipt, Vendor> it in (new PXCancel<POReceipt>(this, "Cancel")).Press(adapter))
			{
				row = it;
				POReceipt rct = it;
				if (!String.IsNullOrEmpty(rct.ReceiptNbr))
				{
					POReceipt another = PXSelectReadonly<POReceipt, Where<POReceipt.receiptNbr, Equal<Required<POReceipt.receiptNbr>>>>.Select(this, rct.ReceiptNbr);
					if (another != null && another.ReceiptType != rct.ReceiptType)
					{
						POReceiptType.ListAttribute namesList = new POReceiptType.ListAttribute();
						string typeName = namesList.ValueLabelDic[rct.ReceiptType];
						this.Document.Cache.RaiseExceptionHandling<POReceipt.receiptNbr>(rct, rct.ReceiptNbr, new PXSetPropertyException(Messages.ReceiptNumberBelongsToDocumentHavingDifferentType, rct.ReceiptNbr, typeName, another.ReceiptType));
						this.Document.Current.ReceiptNbr = null;
					}
				}
			}
			yield return row;
		}

        public PXAction<POReceipt> release;
        [PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXProcessButton]
        public virtual IEnumerable Release(PXAdapter adapter)
        {
            PXCache cache = this.Document.Cache;
            List<POReceipt> list = new List<POReceipt>();
            foreach (POReceipt indoc in adapter.Get<POReceipt>())
            {
                if (indoc.Hold == false && indoc.Released == false)
                {  
                    cache.Update(indoc);
                    list.Add(indoc);
                }
            }

            if (list.Count == 0)
            {
                throw new PXException(Messages.Document_Status_Invalid);
            }
            Save.Press();
            PXLongOperation.StartOperation(this, delegate() { POReleaseReceipt.ReleaseDoc(list, false); });
            return list;
        }

        [CRReference(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<POReceipt.vendorID>>>>))]
        public CRActivityList<POReceipt>
            Activity;

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

        [PXViewName(Messages.VendorContact)]
        public PXSelectJoin<Contact, InnerJoin<Vendor, On<Contact.contactID, Equal<Vendor.defContactID>>>, Where<Vendor.bAccountID, Equal<Current<POReceipt.vendorID>>>> contact;
        
        public PXAction<POReceipt> notification;
        [PXUIField(DisplayName = "Notifications", Visible = false)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
        protected virtual IEnumerable Notification(PXAdapter adapter,
        [PXString]
		string notificationCD
        )
        {
            foreach (POReceipt receipt in adapter.Get<POReceipt>())
            {
                Document.Cache.Current = receipt;
                var parameters = new Dictionary<string, string>();
                parameters["POReceipt.ReceiptType"] = receipt.ReceiptType;
                parameters["POReceipt.ReceiptNbr"] = receipt.ReceiptNbr;
                Activity.SendNotification(APNotificationSource.Vendor, notificationCD, receipt.BranchID, parameters);

                yield return receipt;
            }
        }

        public PXAction<POReceipt> action;
        [PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Action(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<POReceipt> inquiry;
        [PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Inquiry(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<POReceipt> report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Report(PXAdapter adapter,			
			
			[PXString(8, InputMask = "CC.CC.CC.CC")]
			[PXStringList(new string[] { "PO646000", "PO632000", "PO622000" }, new string[] { "Purchase Receipt", Messages.ReportPOReceiptBillingDetails, Messages.ReportPOReceipAllocated })]
			string reportID)
        {
            List<POReceipt> list = adapter.Get<POReceipt>().ToList();
			if (string.IsNullOrEmpty(reportID) == false)
			{
                Save.Press();
				int i = 0;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
                foreach (POReceipt doc in list)
				{

					if (reportID == "PO632000")
					{
						parameters["FinPeriodID"] = (string)Document.GetValueExt<POReceipt.finPeriodID>(doc);
                        parameters["ReceiptNbr"] = doc.ReceiptNbr;
					}
					else
					{
                        parameters["ReceiptType"] = doc.ReceiptType;
						parameters["ReceiptNbr"] = doc.ReceiptNbr;
					}
					i++;

				}
				if (i > 0)
				{
					throw new PXReportRequiredException(parameters, reportID, string.Format("Report {0}", reportID));
				}
			}
            return list;
        }


		public PXAction<POReceipt> assign;
		[PXUIField(DisplayName = Messages.Assign, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Process)]
		public virtual IEnumerable Assign(PXAdapter adapter)
		{
			if (posetup.Current.DefaultReceiptAssignmentMapID == null)
			{
				throw new PXSetPropertyException(Messages.AssignNotSetup_Receipt, "PO Setup");
			}

			EPAssignmentProcessHelper<POReceipt> aph = new EPAssignmentProcessHelper<POReceipt>();
			aph.Assign(Document.Current, posetup.Current.DefaultReceiptAssignmentMapID);

			Document.Update(Document.Current);

			return adapter.Get();
		}

		public PXAction<POReceipt> addPOOrder;
		[PXUIField(DisplayName = Messages.AddPOOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrder(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 this.Document.Current.Released != true)
			{				
                if (this.filter.Current.ResetFilter == true)
				{
					this.filter.Cache.Remove(this.filter.Current);
					this.filter.Cache.Insert(new POOrderFilter());
				}
				else
					this.filter.Cache.RaiseRowSelected(this.filter.Current);

			    if (this.openOrders.AskExt(true) == WebDialogResult.OK)
					return AddPOOrder2(adapter);
			}
			openOrders.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<POReceipt> addPOOrder2;
		[PXUIField(DisplayName = Messages.AddPOOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrder2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 this.Document.Current.Released != true)
				{
					bool hasError = false;
					foreach (POOrderS it in this.openOrders.Cache.Updated)
					{
					if (it.Selected == true)
						{
						if (IsPassingFilter(this.filter.Current, it))
							//Some orders may be filtered out but still be in the updated Cache
							{
								string message;
								if (!CanAddOrder(it, out message))
								{
									hasError = true;
								}
								else
								{
									this.AddPurchaseOrder(it);
								}
							}
							it.Selected = false;
						}
					}
					if (hasError)
						throw new PXException(Messages.SomeOrdersMayNotBeAddedTypeOrShippingDestIsDifferent);
				}
			return adapter.Get();
		}

        public PXAction<POReceipt> addTransfer;
        [PXUIField(DisplayName = Messages.AddTransfer, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXLookupButton]
        public virtual IEnumerable AddTransfer(PXAdapter adapter)
        {
            if (this.Document.Current != null &&
                 this.Document.Current.Released != true)
            {
                if (this.filter.Current.ResetFilter == true)
                {
                    this.filter.Cache.Remove(this.filter.Current);
                    this.filter.Cache.Insert(new POOrderFilter());
                }
                else
                    this.filter.Cache.RaiseRowSelected(this.filter.Current);

                if (this.openTransfers.AskExt(true) == WebDialogResult.OK)
                    return AddTransfer2(adapter);
            }
            openTransfers.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<POReceipt> addTransfer2;
        [PXUIField(DisplayName = Messages.AddTransfer, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddTransfer2(PXAdapter adapter)
        {
            if (this.Document.Current != null &&
                 this.Document.Current.Released != true)
            {
                bool hasError = false;
	            foreach (SOOrderShipment it in this.openTransfers.Cache.Updated)
                {
                    if (it.Selected == true)
                    {
						using (new PXReadBranchRestrictedScope())
	                    {
                        AddTransferDoc(it);
	                    }

                        it.Selected = false;
                    }
                }
                if (hasError)
                    throw new PXException(Messages.SomeOrdersMayNotBeAddedTypeOrShippingDestIsDifferent);
            }
            return adapter.Get();
        }

        public PXAction<POReceipt> addINTran;
        [PXUIField(DisplayName = Messages.AddTransferLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddINTran(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<POReceipt> addINTran2;
        [PXUIField(DisplayName = Messages.AddTransferLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddINTran2(PXAdapter adapter)
        {
            return adapter.Get();
        }

		public PXAction<POReceipt> addPOOrderLine;
		[PXUIField(DisplayName = Messages.AddPOOrderLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrderLine(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released != true)
			{
                if (this.filter.Current.ResetFilter == true)
				{
					this.filter.Cache.Remove(this.filter.Current);
					this.filter.Cache.Insert(new POOrderFilter());
				}
				else
					this.filter.Cache.RaiseRowSelected(this.filter.Current);

                if (this.poLinesSelection.AskExt((graph, view) => graph.Views[view].Cache.Clear(), true) == WebDialogResult.OK)
				{
					return AddPOOrderLine2(adapter);
				}
			}
			poLinesSelection.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<POReceipt> addPOOrderLine2;
		[PXUIField(DisplayName = Messages.AddPOOrderLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrderLine2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released != true)
					foreach (POLine it in poLinesSelection.Cache.Updated)
					{
						if ((bool)it.Selected)
						{							
							try
							{
								this._skipUIUpdate = true;
									this.AddPOLine(it);
								AddPOOrderReceipt(it.OrderType, it.OrderNbr);
							}
							finally
							{
								this._skipUIUpdate = false;
							}							
							it.Selected = false;
						}
					}
			return adapter.Get();
		}

		public PXAction<POReceipt> addPOReceiptLine;
		[PXUIField(DisplayName = Messages.AddLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOReceiptLine(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released != true)
			{
				this.filter.Current.AllowAddLine = false;
				if (addReceipt.AskExt(
						(graph, view) => ((POReceiptEntry)graph).ResetReceiptFilter(false)) == WebDialogResult.OK)
				{
					try
					{
						this._skipUIUpdate = IsImport;
						AddReceiptLine();
					}
					finally
					{
						this._skipUIUpdate = false;
					}

				}
			}
			return adapter.Get();
		}
		public PXAction<POReceipt> addPOReceiptLine2;
		[PXUIField(DisplayName = Messages.AddLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPOReceiptLine2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released != true &&
					AddReceiptLine())
			{
				ResetReceiptFilter(true);
				this.addReceipt.View.RequestRefresh();	
			}
			return adapter.Get();
		}
		private void ResetReceiptFilter(bool keepDescription)
		{
			POReceiptLineS s = this.addReceipt.Current;
			this.addReceipt.Cache.Remove(s);
			this.addReceipt.Cache.Insert(new POReceiptLineS());
			this.addReceipt.Current.ByOne = s.ByOne;
			this.addReceipt.Current.AutoAddLine = s.AutoAddLine;		
            if (keepDescription)
				this.addReceipt.Current.Description = s.Description;		
			
		}

		protected virtual bool AddReceiptLine()
		{
			if (addReceipt.Current.VendorLocationID == null && addReceipt.Current.OrigRefNbr == null ||
					addReceipt.Current.Qty <= 0)
				return false;
			
			POLineS poLine =
				addReceipt.Current.PONbr != null && addReceipt.Current.POLineNbr != null
					? this.poLinesSelection.Search<POLineS.orderType, POLineS.orderNbr, POLineS.lineNbr>(
						addReceipt.Current.POType, addReceipt.Current.PONbr, addReceipt.Current.POLineNbr)
					: null;
			
			int? inventoryID = addReceipt.Current.InventoryID;
			if (inventoryID == null && poLine != null)
				inventoryID = poLine.InventoryID;

			if (inventoryID == null)
				return false;

			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);

			if (item == null)
				return false;

			POReceiptLine line = null;

			if (this.Document.Current.VendorID == null && addReceipt.Current.VendorID != null)
			{
				POReceipt r = PXCache<POReceipt>.CreateCopy(this.Document.Current);
				r.VendorID = poLine != null ? poLine.VendorID : addReceipt.Current.VendorID;
				r.VendorLocationID = poLine != null ? poLine.VendorLocationID : addReceipt.Current.VendorLocationID;
				this.Document.Update(r);
			}

			if (this.Document.Current.SiteID == null && addReceipt.Current.SiteID != null)
			{
				POReceipt r = PXCache<POReceipt>.CreateCopy(this.Document.Current);
				r.SiteID = addReceipt.Current.SiteID;
				this.Document.Update(r);
			}

			if (poLine != null)
			{
				line = PXSelect<POReceiptLine,
					Where<POReceiptLine.receiptType, Equal<Current<POReceipt.receiptType>>,
						And<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>,
							And<POReceiptLine.pOType, Equal<Current<POLine.orderType>>,
								And<POReceiptLine.pONbr, Equal<Current<POLine.orderNbr>>,
									And<POReceiptLine.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>>>>
					.SelectSingleBound(this, new object[] { poLine });
				if (line != null && addReceipt.Current.SiteID != null && line.SiteID != addReceipt.Current.SiteID)
					line = null;

				if (line == null)
				{
					//Note: AddPOLine() in its implementation always adds all unreceived qty. 
					//In order to add only what is specified in the 'Add Receipt Line' dialog set the order qty to the desired value:
					if (poLine.BaseOrderQty != addReceipt.Current.BaseReceiptQty)
					{
					poLine.OrderQty = addReceipt.Current.ReceiptQty;
					poLine.BaseOrderQty = addReceipt.Current.BaseReceiptQty;
						poLine.OrderedQtyAltered = true;
					}
					

					line = AddPOLine(poLine, true);//Add Receipt line without adding the split (for performance), it will be added later in this method.
				}
			}

			INTran intran = PXSelect<INTran, Where<INTran.docType, Equal<Required<INTran.docType>>, And<INTran.refNbr, Equal<Required<INTran.refNbr>>, And<INTran.lineNbr, Equal<Required<INTran.lineNbr>>>>>>.Select(this, INDocType.Transfer, addReceipt.Current.OrigRefNbr, addReceipt.Current.OrigLineNbr);
			if (intran != null)
			{
				line = PXSelect<POReceiptLine,
							Where<POReceiptLine.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
								And<POReceiptLine.origTranType, Equal<Required<POReceiptLine.origTranType>>,
								And<POReceiptLine.origRefNbr, Equal<Required<POReceiptLine.origRefNbr>>,
								And<POReceiptLine.origLineNbr, Equal<Required<POReceiptLine.origLineNbr>>>>>>>
								.Select(this, Document.Current.ReceiptNbr, intran.TranType, intran.RefNbr, intran.LineNbr);

				if (line == null)
				{
					line = AddTransferLine(intran);

					if (item.StkItem == true && line != null && addReceipt.Current.LocationID != null)
					{
						foreach (POReceiptLineSplit split in splits.Select())
						{
							split.LocationID = addReceipt.Current.LocationID;
							splits.Update(split);
						}
					}
				}
			}

			if (line == null && Document.Current != null && Document.Current.ReceiptType == POReceiptType.TransferReceipt)
			{
				throw new PXException(ErrorMessages.CantInsertRecord);
			}

			if (line == null)
			{
				string lineType = (item.StkItem != true) ? POLineType.NonStock : POLineType.GoodsForInventory;
				line = PXSelect<POReceiptLine,
				Where<POReceiptLine.receiptType, Equal<Current<POReceipt.receiptType>>,
					And<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>,
					And<POReceiptLine.lineType, Equal<Required<POReceiptLine.lineType>>,
					And<POReceiptLine.inventoryID, Equal<Required<POReceiptLine.inventoryID>>,
					And<POReceiptLine.subItemID, Equal<Required<POReceiptLine.subItemID>>,
					And<POReceiptLine.pOLineNbr, IsNull,
					And2<Where<Current<POReceiptLineS.siteID>, IsNull, Or<POReceiptLine.siteID, Equal<Current<POReceiptLineS.siteID>>>>,
					And<Where<Current<POReceiptLineS.curyUnitCost>, Equal<decimal0>, Or<POReceiptLine.curyUnitCost, Equal<Current<POReceiptLineS.curyUnitCost>>>>>>>>>>>>>
				.SelectSingleBound(this, null, lineType, addReceipt.Current.InventoryID, addReceipt.Current.SubItemID);

				if (line == null)
				{
					line = PXCache<POReceiptLine>.CreateCopy(this.transactions.Insert(new POReceiptLine()));
					line.LineType = lineType;
					line.InventoryID = addReceipt.Current.InventoryID;
					line.UOM = addReceipt.Current.UOM;
					if (addReceipt.Current.SubItemID != null)
						line.SubItemID = addReceipt.Current.SubItemID;
					if (addReceipt.Current.SiteID != null)
						line.SiteID = addReceipt.Current.SiteID;
					line = PXCache<POReceiptLine>.CreateCopy(this.transactions.Update(line));
					if (addReceipt.Current.CuryUnitCost > 0)
						line.CuryUnitCost = addReceipt.Current.CuryUnitCost;
					line = PXCache<POReceiptLine>.CreateCopy(this.transactions.Update(line));
				}
			}

			string subitem = string.Empty;

			if (line != null && intran == null)
			{
				line = PXCache<POReceiptLine>.CreateCopy(line);
				transactions.Current = line;
				if (item.StkItem == true)
				{
					POReceiptLineSplit split = new POReceiptLineSplit();
					split.Qty = addReceipt.Current.BaseReceiptQty;
					if (addReceipt.Current.LocationID != null)
					{
						split.LocationID = addReceipt.Current.LocationID;
					}
					if (addReceipt.Current.LotSerialNbr != null)
						split.LotSerialNbr = addReceipt.Current.LotSerialNbr;
					if (addReceipt.Current.ExpireDate != null)
						split.ExpireDate = addReceipt.Current.ExpireDate;
					this.splits.Insert(split);
					
					if (!IsImport)
					{
						subitem = insetup.Current.UseInventorySubItem == true && split.SubItemID != null
							? ":" + splits.GetValueExt<POReceiptLineSplit.subItemID>(split)
							: string.Empty;
					}
				}
				else
				{
					line.ReceiptQty += addReceipt.Current.ReceiptQty;
					this.transactions.Update(line);
				}

			}

			if (line != null)
			{
				if (!IsImport)
				{
					addReceipt.Current.Description =
						poLine != null
							? PXMessages.LocalizeFormatNoPrefixNLA(Messages.ReceiptAddedForPO,
								transactions.GetValueExt<POReceiptLine.inventoryID>(line).ToString().Trim(),
								subitem,
								addReceipt.Cache.GetValueExt<POReceiptLineS.receiptQty>(addReceipt.Current).ToString(),
								addReceipt.Current.UOM,
								poLine.OrderNbr)
							: PXMessages.LocalizeFormatNoPrefixNLA(Messages.ReceiptAdded,
								transactions.GetValueExt<POReceiptLine.inventoryID>(line).ToString().Trim(),
								subitem,
								addReceipt.Cache.GetValueExt<POReceiptLineS.receiptQty>(addReceipt.Current).ToString(),
								addReceipt.Current.UOM);
				}
				
				if (addReceipt.Current.BarCode != null && addReceipt.Current.SubItemID != null)
				{
					INItemXRef xref =
						PXSelect<INItemXRef,
							Where<INItemXRef.inventoryID, Equal<Current<POReceiptLineS.inventoryID>>,
								And<INItemXRef.alternateID, Equal<Current<POReceiptLineS.barCode>>,
									And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>>
							.SelectSingleBound(this, null);
					if (xref == null)
					{
						xref = new INItemXRef();
						xref.InventoryID = addReceipt.Current.InventoryID;
						xref.AlternateID = addReceipt.Current.BarCode;
						xref.AlternateType = INAlternateType.Barcode;
						xref.SubItemID = addReceipt.Current.SubItemID;
						xref.BAccountID = 0;
						this.xrefs.Insert(xref);
					}
				}
			}
			
			return line != null;
		}

		public PXAction<POReceipt> viewPOOrder;
		[PXUIField(DisplayName = Messages.ViewPOOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewPOOrder(PXAdapter adapter)
		{
			if (this.transactions.Current != null)
			{
				POReceiptLine row = this.transactions.Current;
				if (string.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr))
				{
					throw new PXException(Messages.POReceiptLineDoesNotReferencePOOrder);
				}
				POOrderEntry graph = PXGraph.CreateInstance<POOrderEntry>();
				graph.Document.Current = graph.Document.Search<POOrder.orderNbr>(row.PONbr, row.POType);
                throw new PXRedirectRequiredException(graph, true, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public PXAction<POReceipt> viewINDocument;
		[PXUIField(DisplayName = Messages.ViewINDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewINDocument(PXAdapter adapter)
		{
			POReceipt doc = this.Document.Current;
			if (doc != null && (bool)doc.Released == true)
			{
				INRegister inDoc = PXSelectJoin<INRegister, InnerJoin<INTran, On<INTran.docType, Equal<INRegister.docType>,
															And<INTran.refNbr, Equal<INRegister.refNbr>>>>,
										Where<INTran.pOReceiptNbr, Equal<Required<INTran.pOReceiptNbr>>, And<Where<INRegister.docType, Equal<INDocType.receipt>,
											Or<INRegister.docType, Equal<INDocType.issue>>>>>>.SelectWindowed(this, 0, 1, doc.ReceiptNbr);
				if (inDoc != null)
				{
					if (inDoc.DocType == INDocType.Receipt)
					{
						INReceiptEntry inReceiptGraph = PXGraph.CreateInstance<INReceiptEntry>();
						inReceiptGraph.receipt.Current = inReceiptGraph.receipt.Search<INRegister.refNbr>(inDoc.RefNbr);
						throw new PXRedirectRequiredException(inReceiptGraph, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
					else if (inDoc.DocType == INDocType.Issue)
					{
						INIssueEntry inIssueGraph = PXGraph.CreateInstance<INIssueEntry>();
						inIssueGraph.issue.Current = inIssueGraph.issue.Search<INRegister.refNbr>(inDoc.RefNbr);
						throw new PXRedirectRequiredException(inIssueGraph, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<POReceipt> viewAPDocument;
		[PXUIField(DisplayName = Messages.ViewAPDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewAPDocument(PXAdapter adapter)
		{
			POReceipt doc = this.Document.Current;
			if (doc != null && (bool)doc.Released == true)
			{
				APRegister apDoc = PXSelectJoin<APRegister, InnerJoin<APTran, On<APTran.tranType, Equal<APRegister.docType>,
															And<APTran.refNbr, Equal<APRegister.refNbr>>>>,
										Where<APTran.receiptNbr, Equal<Required<APTran.receiptNbr>>, And<APTran.receiptLineNbr, IsNotNull>>, OrderBy<Desc<APRegister.refNbr>>>.SelectWindowed(this, 0, 1, doc.ReceiptNbr);
				if (apDoc != null)
				{
					APInvoiceEntry invoiceGraph = PXGraph.CreateInstance<APInvoiceEntry>();
					invoiceGraph.Document.Current = invoiceGraph.Document.Search<APRegister.refNbr>(apDoc.RefNbr, apDoc.DocType);
					if (invoiceGraph.Document.Current != null)
						throw new PXRedirectRequiredException(invoiceGraph, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}
			return adapter.Get();
		}


		public PXAction<POReceipt> viewLCINDocument;
		[PXUIField(DisplayName = Messages.ViewINDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewLCINDocument(PXAdapter adapter)
		{
			LandedCostTran doc = this.landedCostTrans.Current;
			if (doc != null && String.IsNullOrEmpty(doc.INRefNbr) != true)
			{
				INAdjustmentEntry inReceiptGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
				inReceiptGraph.adjustment.Current = inReceiptGraph.adjustment.Search<INRegister.refNbr>(doc.INRefNbr);
				throw new PXRedirectRequiredException(inReceiptGraph, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };

			}
			return adapter.Get();
		}

		public PXAction<POReceipt> viewLCAPInvoice;
		[PXUIField(DisplayName = Messages.ViewAPDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewLCAPInvoice(PXAdapter adapter)
		{
			LandedCostTran doc = this.landedCostTrans.Current;
			if (doc != null && (bool)String.IsNullOrEmpty(doc.APRefNbr) != true)
			{
				APInvoiceEntry invoiceGraph = PXGraph.CreateInstance<APInvoiceEntry>();
				invoiceGraph.Document.Current = invoiceGraph.Document.Search<APRegister.refNbr>(doc.APRefNbr, doc.APDocType);
				if (invoiceGraph.Document.Current != null)
					throw new PXRedirectRequiredException(invoiceGraph, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}
				
		public PXAction<POReceipt> createAPDocument;
		[PXUIField(DisplayName = Messages.CreateAPInvoice, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable CreateAPDocument(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				this.Document.Current.Released == true)
			{

				POReceipt doc = this.Document.Current;
				if (doc.UnbilledLineTotal != Decimal.Zero)
				{
					APRegister apDoc = PXSelectJoin<APRegister, InnerJoin<APTran, On<APTran.tranType, Equal<APRegister.docType>,
															And<APTran.refNbr, Equal<APRegister.refNbr>>>>,
										Where<APTran.receiptNbr, Equal<Required<APTran.receiptNbr>>>, OrderBy<Desc<APRegister.refNbr>>>.SelectWindowed(this, 0, 1, doc.ReceiptNbr);
					bool firstDoc = (apDoc == null);
					bool hasUnreased = firstDoc ? false : (apDoc.Released == false);
					if (!hasUnreased)
					{
						apDoc = PXSelectJoin<APRegister, InnerJoin<APTran, On<APTran.tranType, Equal<APRegister.docType>,
															And<APTran.refNbr, Equal<APRegister.refNbr>>>>,
										Where<APTran.receiptNbr, Equal<Required<APTran.receiptNbr>>,
											And<APRegister.released, Equal<False>>>,
											OrderBy<Desc<APRegister.refNbr>>>.SelectWindowed(this, 0, 1, doc.ReceiptNbr);
						hasUnreased = (apDoc != null);
					}
					if (hasUnreased)
					{
						throw new PXException(Messages.UnreasedAPDocumentExistsForPOReceipt);
					}
					APInvoiceEntry invoiceGraph = PXGraph.CreateInstance<APInvoiceEntry>();
					DocumentList<APInvoice> created = new DocumentList<APInvoice>(invoiceGraph);
					CurrencyInfo info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(this, doc.CuryInfoID);
					invoiceGraph.InvoicePOReceipt(doc, info, created, false, true);
                    invoiceGraph.AttachPrepayment();
					throw new PXRedirectRequiredException(invoiceGraph, Messages.CreateAPInvoice);
				}
				else
				{
					throw new PXException(Messages.AllTheLinesOfPOReceiptAreAlreadyBilled);
				}

			}
			return adapter.Get();
		}

        public PXAction<POReceipt> recalculateDiscountsAction;
        [PXUIField(DisplayName = "Recalculate Prices and Discounts", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXButton]
        public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
        {
            if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
            {
                DiscountEngine<POReceiptLine>.RecalculatePricesAndDiscounts<POReceiptDiscountDetail>(transactions.Cache, transactions, transactions.Current, DiscountDetails, Document.Current.VendorLocationID, Document.Current.ReceiptDate, recalcdiscountsfilter.Current);
            }
            return adapter.Get();
        }

        public PXAction<POReceipt> recalcOk;
        [PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable RecalcOk(PXAdapter adapter)
        {
            return adapter.Get();
        }

		#endregion

		public POReceiptEntry()
		{
			POSetup record = posetup.Current;
			this.Actions.Move("Insert", "Cancel");
			RowUpdated.AddHandler<POReceipt>(ParentFieldUpdated);
			this.poLinesSelection.Cache.AllowInsert = false;
			this.poLinesSelection.Cache.AllowDelete = false;
			this.poLinesSelection.Cache.AllowUpdate = true;
			this.openOrders.Cache.AllowInsert = false;
			this.openOrders.Cache.AllowDelete = false;
			this.openOrders.Cache.AllowUpdate = true;
			this.openTransfers.Cache.AllowInsert = false;
			this.openTransfers.Cache.AllowDelete = false;
			this.openTransfers.Cache.AllowUpdate = true;
			PXUIFieldAttribute.SetVisible<LandedCostTran.pOReceiptType>(this.landedCostTrans.Cache, null, false);
			PXUIFieldAttribute.SetVisible<LandedCostTran.pOReceiptNbr>(this.landedCostTrans.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.vendorID>(this.landedCostTrans.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.vendorLocationID>(this.landedCostTrans.Cache, null, false);
			PXUIFieldAttribute.SetVisible<LandedCostTran.aPDocType>(this.landedCostTrans.Cache, null, true);
			PXUIFieldAttribute.SetVisible<LandedCostTran.aPRefNbr>(this.landedCostTrans.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.aPDocType>(this.landedCostTrans.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.aPRefNbr>(this.landedCostTrans.Cache, null, false);

			bool isInvoiceNbrRequired = (bool)apsetup.Current.RequireVendorRef;
			PXDefaultAttribute.SetPersistingCheck<LandedCostTran.invoiceNbr>(this.landedCostTrans.Cache, null, isInvoiceNbrRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXUIFieldAttribute.SetRequired<LandedCostTran.invoiceNbr>(this.landedCostTrans.Cache, isInvoiceNbrRequired);

			PXDefaultAttribute.SetPersistingCheck<LandedCostTran.descr>(this.landedCostTrans.Cache, null, PXPersistingCheck.NullOrBlank);
			PXUIFieldAttribute.SetRequired<LandedCostTran.descr>(this.landedCostTrans.Cache, true);

			PXDefaultAttribute.SetPersistingCheck<POReceipt.invoiceNbr>(this.Document.Cache, null, isInvoiceNbrRequired ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXUIFieldAttribute.SetRequired<POReceipt.invoiceNbr>(this.Document.Cache, isInvoiceNbrRequired);

			PXDefaultAttribute.SetPersistingCheck<POReceipt.invoiceDate>(this.Document.Cache, null, PXPersistingCheck.NullOrBlank);
			PXUIFieldAttribute.SetRequired<POReceipt.invoiceDate>(this.Document.Cache, true);

			PXUIFieldAttribute.SetEnabled(this.openOrders.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<POOrderS.selected>(this.openOrders.Cache, null, true);

			PXUIFieldAttribute.SetEnabled(openTransfers.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INRegister.selected>(openTransfers.Cache, null, true);

			PXDimensionSelectorAttribute.SetValidCombo<POReceiptLine.subItemID>(transactions.Cache, false);
			PXDimensionSelectorAttribute.SetValidCombo<POReceiptLineSplit.subItemID>(splits.Cache, false);
			PXUIFieldAttribute.SetVisible<POReceiptLine.projectID>(transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO));
			PXUIFieldAttribute.SetVisible<POReceiptLine.taskID>(transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO));

			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });

			this.Views.Caches.Remove(typeof(POLineS));
			this.Views.Caches.Remove(typeof(POOrderS));

            PXUIFieldAttribute.SetVisible<SOOrderShipment.orderType>(this.Caches[typeof(SOOrderShipment)], null, true);
            PXUIFieldAttribute.SetVisible<SOOrderShipment.orderNbr>(this.Caches[typeof(SOOrderShipment)], null, true);
            PXUIFieldAttribute.SetVisible<SOOrderShipment.shipmentNbr>(this.Caches[typeof(SOOrderShipment)], null, true);

			TaxAttribute.SetTaxCalc<POReceiptLine.taxCategoryID>(transactions.Cache, null, TaxCalc.ManualLineCalc);
        }

		#region POReceipt Events

		protected virtual void POReceipt_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			bool requireControlTotal = (bool)posetup.Current.RequireReceiptControlTotal;
			POReceipt doc = (POReceipt)e.Row;

            if (e.ExternalCall && !sender.ObjectsEqual<POReceipt.receiptDate>(e.OldRow, e.Row) || !sender.ObjectsEqual<POReceipt.vendorLocationID>(e.OldRow, e.Row))
            {
                DiscountEngine<POReceiptLine>.AutoRecalculatePricesAndDiscounts<POReceiptDiscountDetail>(transactions.Cache, transactions, null, DiscountDetails, doc.VendorLocationID, doc.ReceiptDate, false);
            }
            if (e.ExternalCall && sender.GetStatus(doc) != PXEntryStatus.Deleted && !sender.ObjectsEqual<POOrder.curyDiscTot>(e.OldRow, e.Row))
            {
                DiscountEngine<POReceiptLine>.CalculateDocumentDiscountRate<POReceiptDiscountDetail>(transactions.Cache, transactions, null, DiscountDetails, doc.CuryLineTotal ?? 0m, doc.CuryDiscTot ?? 0m);
            }

            if (requireControlTotal == false || doc.ReceiptType == POReceiptType.TransferReceipt)
			{
				if (PXCurrencyAttribute.IsNullOrEmpty(((POReceipt)e.Row).CuryOrderTotal) == false)
				{
					sender.SetValue<POReceipt.curyControlTotal>(e.Row, ((POReceipt)e.Row).CuryOrderTotal);
				}
				else
				{
					sender.SetValue<POReceipt.curyControlTotal>(e.Row, 0m);
				}
            }

            if (requireControlTotal == false)
            {
				if (PXCurrencyAttribute.IsNullOrEmpty(((POReceipt)e.Row).OrderQty) == false)
				{
					sender.SetValue<POReceipt.controlQty>(e.Row, ((POReceipt)e.Row).OrderQty);
				}
				else
				{
					sender.SetValue<POReceipt.controlQty>(e.Row, 0m);
				}
			}

			bool released = (bool)((POReceipt)e.Row).Released;
			if (((POReceipt)e.Row).Hold == false && released == false)
			{
                if (requireControlTotal && doc.ReceiptType != POReceiptType.TransferReceipt)
				{
					if (((POReceipt)e.Row).CuryOrderTotal != ((POReceipt)e.Row).CuryControlTotal)
					{
						sender.RaiseExceptionHandling<POReceipt.curyControlTotal>(e.Row, ((POReceipt)e.Row).CuryControlTotal, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else
					{
						sender.RaiseExceptionHandling<POReceipt.curyControlTotal>(e.Row, ((POReceipt)e.Row).CuryControlTotal, null);
					}

                }

                if (requireControlTotal)
                {
					if (((POReceipt)e.Row).OrderQty != ((POReceipt)e.Row).ControlQty)
					{
						sender.RaiseExceptionHandling<POReceipt.controlQty>(e.Row, ((POReceipt)e.Row).ControlQty, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else
					{
						sender.RaiseExceptionHandling<POReceipt.controlQty>(e.Row, ((POReceipt)e.Row).ControlQty, null);
					}
				}

				if (doc.CuryOrderTotal < Decimal.Zero && doc.Hold == false)
				{
					if (sender.RaiseExceptionHandling<POReceipt.curyOrderTotal>(e.Row, doc.CuryOrderTotal, new PXSetPropertyException(Messages.POReceiptTotalAmountMustBeNonNegative, typeof(POReceipt.curyOrderTotal).Name)))
					{
						throw new PXRowPersistingException(typeof(POReceipt.curyOrderTotal).Name, null, Messages.POReceiptTotalAmountMustBeNonNegative, typeof(POReceipt.curyOrderTotal).Name);
					}
				}
				else
				{
					sender.RaiseExceptionHandling<POReceipt.curyOrderTotal>(e.Row, null, null);					
				}
			}
			else 
			{
				sender.RaiseExceptionHandling<POReceipt.curyLineTotal>(e.Row, null, null);
				sender.RaiseExceptionHandling<POReceipt.curyControlTotal>(e.Row, null, null);
				sender.RaiseExceptionHandling<POReceipt.controlQty>(e.Row, null, null);
			}
		}

        protected virtual void POReceipt_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }
            POReceipt doc = (POReceipt)e.Row;
            bool released = (bool)doc.Released;
            bool requireControlTotal = (bool)posetup.Current.RequireReceiptControlTotal;
            bool hasTransactions = false;
            bool isReturn = (doc.ReceiptType == POReceiptType.POReturn);
            bool isTransferReceipt = (doc.ReceiptType == POReceiptType.TransferReceipt);
            bool isWarehouseSet = doc.SiteID != null;


            this.landedCostTrans.Cache.AllowInsert = this.landedCostTrans.Cache.AllowUpdate = this.landedCostTrans.Cache.AllowDelete = !isReturn && !released;
            PXFieldState orderSelector = (PXFieldState)this.filter.Cache.GetStateExt<POOrderFilter.orderNbr>(null);
            poLinesSelection.WhereNew<Where<POLineS.orderType, Equal<Current<POOrderFilter.orderType>>,
				And<POOrder.curyID, Equal<Current<POReceipt.curyID>>,
				And<POLineS.lineType, NotEqual<POLineType.service>,
                And<POLineS.lineType, NotEqual<POLineType.description>,
                And2<Where<Current<POOrderFilter.vendorID>, IsNull,
                                Or<POLineS.vendorID, Equal<Current<POOrderFilter.vendorID>>>>,
                And2<Where<Current<POOrderFilter.vendorLocationID>, IsNull,
                                Or<POOrder.vendorLocationID, Equal<Current<POOrderFilter.vendorLocationID>>>>,
                And2<Where<Current<POOrderFilter.orderNbr>, IsNull,
                                Or<POLineS.orderNbr, Equal<Current<POOrderFilter.orderNbr>>>>,
                And2<Where<Current<POOrderFilter.inventoryID>, IsNull,
                                Or<POLineS.inventoryID, Equal<Current<POOrderFilter.inventoryID>>>>,
                And<Where<Current<POOrderFilter.subItemID>, IsNull,
                                Or<POLineS.subItemID, Equal<Current<POOrderFilter.subItemID>>,
                                Or<Not<FeatureInstalled<FeaturesSet.subItem>>>>>>>>>>>>>>>();
            openOrders.WhereNew<Where<POOrderS.hold, Equal<boolFalse>,
                And2<Where<Current<POReceipt.vendorID>, IsNull,
                     Or<POOrderS.vendorID, Equal<Current<POReceipt.vendorID>>>>,
                And2<Where<Current<POReceipt.vendorLocationID>, IsNull,
                         Or<POOrderS.vendorLocationID, Equal<Current<POReceipt.vendorLocationID>>>>,
                And2<Where<Current<POOrderFilter.vendorID>, IsNull,
                         Or<POOrderS.vendorID, Equal<Current<POOrderFilter.vendorID>>>>,
                And2<Where<Current<POOrderFilter.vendorLocationID>, IsNull,
                         Or<POOrderS.vendorLocationID, Equal<Current<POOrderFilter.vendorLocationID>>>>,
                And2<Where<POOrderS.curyID, Equal<Current<POReceipt.curyID>>,
                    Or<Current<POOrderFilter.anyCurrency>, Equal<boolTrue>>>,
                And2<Where2<Where<Current<POOrderFilter.orderType>, IsNull,
                             And<Where<POOrderS.orderType, Equal<POOrderType.regularOrder>,
                                            Or<POOrderS.orderType, Equal<POOrderType.dropShip>>>>>,
                        Or<POOrderS.orderType, Equal<Current<POOrderFilter.orderType>>>>,
                And2<Where<POOrderS.shipToBAccountID, Equal<Current<POOrderFilter.shipToBAccountID>>,
                    Or<Current<POOrderFilter.shipToBAccountID>, IsNull>>,
                And<Where<POOrderS.shipToLocationID, Equal<Current<POOrderFilter.shipToLocationID>>,
                Or<Current<POOrderFilter.shipToLocationID>, IsNull>>>>>>>>>>>>();
            PXView view = this.Views[orderSelector.ViewName];
            view.WhereNew<Where<POOrderS.orderType, Equal<Optional<POOrderFilter.orderType>>,
                And<POOrderS.curyID, Equal<Current<POReceipt.curyID>>,
                And<POOrderS.hold, Equal<boolFalse>,
                And2<Where<Current<POReceipt.vendorID>, IsNull,
                                     Or<POOrderS.vendorID, Equal<Current<POReceipt.vendorID>>>>,
                And2<Where<Current<POReceipt.vendorLocationID>, IsNull,
                                         Or<POOrderS.vendorLocationID, Equal<Current<POReceipt.vendorLocationID>>>>,
                And2<Where<POOrderS.shipToBAccountID, Equal<Current<POOrderFilter.shipToBAccountID>>,
                                    Or<Current<POOrderFilter.shipToBAccountID>, IsNull>>,
                                And<Where<POOrderS.shipToLocationID, Equal<Current<POOrderFilter.shipToLocationID>>,
                            Or<Current<POOrderFilter.shipToLocationID>, IsNull>>>>>>>>>>();
            if (isReturn)
            {
                poLinesSelection.WhereAnd<Where<POLineS.receivedQty, Greater<decimal0>>>();
                openOrders.WhereAnd<Where<POOrderS.cancelled, Equal<boolFalse>,
                        And<POOrderS.receivedQty, Greater<decimal0>>>>();
                view.WhereAnd<Where<POOrderS.cancelled, Equal<boolFalse>,
                        And<POOrderS.receivedQty, Greater<decimal0>>>>();
            }
            else
            {
                poLinesSelection.WhereAnd<Where<POOrder.status, Equal<POOrderStatus.open>,
                          And<POOrder.receipt, Equal<boolFalse>,
                            And<POLineS.completed, Equal<boolFalse>>>>>();
                openOrders.WhereAnd<Where<POOrderS.cancelled, Equal<boolFalse>,
                        And<POOrderS.status, Equal<POOrderStatus.open>,
                        And<POOrderS.receipt, Equal<boolFalse>>>>>();
                view.WhereAnd<Where<POOrderS.cancelled, Equal<boolFalse>,
                        And<POOrderS.status, Equal<POOrderStatus.open>,
                        And<POOrderS.receipt, Equal<boolFalse>>>>>();
            }
            if (this._skipUIUpdate == false)
            {
				var mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
				PXUIFieldAttribute.SetVisible<POReceipt.curyID>(sender, doc, mcFeatureInstalled);
                PXUIFieldAttribute.SetRequired<POReceipt.termsID>(sender, true);
				PXUIFieldAttribute.SetVisible<LandedCostTran.curyID>(this.landedCostTrans.Cache, null, mcFeatureInstalled);

                PXUIFieldAttribute.SetVisible<LandedCostTran.aPDocType>(this.landedCostTrans.Cache, null, released);
                PXUIFieldAttribute.SetVisible<LandedCostTran.aPRefNbr>(this.landedCostTrans.Cache, null, released);
                PXUIFieldAttribute.SetVisible<LandedCostTran.iNDocType>(this.landedCostTrans.Cache, null, released);
                PXUIFieldAttribute.SetVisible<LandedCostTran.iNRefNbr>(this.landedCostTrans.Cache, null, released);
                PXUIFieldAttribute.SetEnabled(this.landedCostTrans.Cache, null, !isReturn && !released);
            }

            this.filter.Current.VendorID = doc.VendorID;
            this.filter.Current.VendorLocationID = doc.VendorLocationID;
            bool curyenabled = true;
            if (vendor.Current != null && (bool)vendor.Current.AllowOverrideCury == false)
            {
                curyenabled = false;
            }
            this.viewINDocument.SetEnabled((bool)doc.Released == true);
            this.viewAPDocument.SetEnabled((bool)doc.Released == true && (bool)doc.AutoCreateInvoice == true);

            PXUIFieldAttribute.SetEnabled(this.poLinesSelection.Cache, null, false);
            if (doc == null || (bool)doc.Released)
            {
                PXUIFieldAttribute.SetEnabled(sender, doc, false);
                sender.AllowDelete = false;
                sender.AllowUpdate = true;
                this.Taxes.Cache.AllowInsert = this.Taxes.Cache.AllowDelete = this.Taxes.Cache.AllowUpdate = false;
                PXDefaultAttribute.SetPersistingCheck<POReceipt.invoiceNbr>(sender, doc, PXPersistingCheck.Nothing);
            }
            else
            {
                if (this._skipUIUpdate == false)
                {
                    PXUIFieldAttribute.SetEnabled(sender, e.Row, released == false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.status>(sender, doc, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.orderQty>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyLineTotal>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyOrderTotal>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyVatExemptTotal>(sender, doc, false);
                    PXUIFieldAttribute.SetEnabled<POOrder.curyVatTaxableTotal>(sender, doc, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyTaxTotal>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyID>(sender, e.Row, curyenabled);
                    PXUIFieldAttribute.SetEnabled<POReceipt.hold>(sender, e.Row, true);
                    PXUIFieldAttribute.SetEnabled<POReceipt.termsID>(sender, e.Row, true);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyUnbilledTotal>(sender, doc, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyUnbilledLineTotal>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyUnbilledTaxTotal>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.unbilledQty>(sender, e.Row, false);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyID>(sender, doc, curyenabled);
                    PXUIFieldAttribute.SetEnabled<POReceipt.autoCreateInvoice>(sender, doc, !isTransferReceipt);

                    bool autoCreateInvoice = (bool)doc.AutoCreateInvoice;
                    PXUIFieldAttribute.SetRequired<POReceipt.invoiceNbr>(sender, autoCreateInvoice);
                    PXUIFieldAttribute.SetEnabled<POReceipt.termsID>(sender, doc, (!isReturn) && autoCreateInvoice);
                    PXUIFieldAttribute.SetEnabled<POReceipt.dueDate>(sender, doc, (!isReturn) && autoCreateInvoice);
                    PXUIFieldAttribute.SetEnabled<POReceipt.discDate>(sender, doc, (!isReturn) && autoCreateInvoice);
                    PXUIFieldAttribute.SetEnabled<POReceipt.curyDiscAmt>(sender, doc, (!isReturn) && autoCreateInvoice);
                    PXUIFieldAttribute.SetRequired<POReceipt.termsID>(sender, (!isReturn) && autoCreateInvoice);
                    PXDefaultAttribute.SetPersistingCheck<POReceipt.termsID>(sender, doc, (!isReturn) && autoCreateInvoice ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);

                    PXUIFieldAttribute.SetEnabled<POReceipt.invoiceDate>(sender, doc, autoCreateInvoice);
                    PXDefaultAttribute.SetPersistingCheck<POReceipt.invoiceDate>(sender, doc, autoCreateInvoice ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
                    PXUIFieldAttribute.SetRequired<POReceipt.invoiceDate>(sender, autoCreateInvoice);

                    bool isInvoiceNbrRequired = autoCreateInvoice && (bool)apsetup.Current.RequireVendorRef;
                    PXDefaultAttribute.SetPersistingCheck<POReceipt.invoiceNbr>(sender, doc, isInvoiceNbrRequired ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
                    PXUIFieldAttribute.SetRequired<POReceipt.invoiceNbr>(sender, isInvoiceNbrRequired);

                    PXUIFieldAttribute.SetVisible<POOrderS.receivedQty>(this.openOrders.Cache, null, isReturn);
                    PXUIFieldAttribute.SetVisible<POOrderS.curyReceivedCost>(this.openOrders.Cache, null, isReturn);
                    PXUIFieldAttribute.SetVisible<POOrderS.leftToReceiveQty>(this.openOrders.Cache, null, !isReturn);
                    PXUIFieldAttribute.SetVisible<POOrderS.curyLeftToReceiveCost>(this.openOrders.Cache, null, !isReturn);
                    PXUIFieldAttribute.SetVisible<POLineS.receivedQty>(this.poLinesSelection.Cache, null, isReturn);
                    PXUIFieldAttribute.SetVisible<POLineS.leftToReceiveQty>(this.poLinesSelection.Cache, null, !isReturn);

                    hasTransactions = (this.transactions.SelectWindowed(0, 1).Count > 0);
                    PXUIFieldAttribute.SetEnabled<POReceipt.receiptType>(sender, doc, !hasTransactions);
                    PXUIFieldAttribute.SetEnabled<POReceipt.vendorID>(sender, doc, !hasTransactions);
                    PXUIFieldAttribute.SetEnabled<POReceipt.siteID>(sender, doc, !hasTransactions);

                    sender.AllowDelete = true;
                    sender.AllowUpdate = true;
                    PXUIFieldAttribute.SetEnabled<POLineS.selected>(this.poLinesSelection.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled(this.Taxes.Cache, null, (bool)doc.AutoCreateInvoice);
                }
            }
            bool hasVendor = (doc.VendorID != null && doc.VendorLocationID != null);
            lsselect.AllowInsert = !released && !isTransferReceipt && hasVendor;
            lsselect.AllowUpdate = !released && hasVendor;
            lsselect.AllowDelete = !released;

            DiscountDetails.Cache.AllowDelete = transactions.Cache.AllowDelete;
            DiscountDetails.Cache.AllowUpdate = transactions.Cache.AllowUpdate;
            DiscountDetails.Cache.AllowInsert = transactions.Cache.AllowInsert;

            Taxes.Cache.AllowDelete = transactions.Cache.AllowDelete;
            Taxes.Cache.AllowInsert = transactions.Cache.AllowInsert;

            if (this._skipUIUpdate == false)
            {
                PXUIFieldAttribute.SetEnabled<POReceipt.receiptNbr>(sender, doc);
                PXUIFieldAttribute.SetEnabled<POReceipt.receiptType>(sender, doc);

                //bool hasLCTrans = this.landedCostTrans.Select().Count >0;
                release.SetEnabled(e.Row != null && (((POReceipt)e.Row).Hold == false && (released == false)));

                PXUIFieldAttribute.SetVisible<POReceipt.controlQty>(sender, e.Row, requireControlTotal || (bool)released);
				PXUIFieldAttribute.SetVisible<POReceipt.curyControlTotal>(sender, e.Row, (requireControlTotal || (bool)released) && !isTransferReceipt);

                bool allowAddOrder = !released && doc.VendorID != null && doc.VendorLocationID != null;
                bool allowSplits = !released && hasTransactions;
                this.addPOOrder.SetEnabled(allowAddOrder);
                this.addPOOrderLine.SetEnabled(allowAddOrder);
                bool allowCreateAPDocument = released
                    && (doc.UnbilledLineTotal != Decimal.Zero)
                    && !isTransferReceipt;
                this.createAPDocument.SetEnabled(allowCreateAPDocument);
            }

            if (IsExternalTax == true && doc.IsTaxValid != true && !isTransferReceipt)
            {
                PXUIFieldAttribute.SetWarning<POReceipt.curyTaxTotal>(sender, e.Row, AR.Messages.TaxIsNotUptodate);
            }

            if (IsExternalTax == true && doc.IsUnbilledTaxValid != true && !isTransferReceipt)
            {
                PXUIFieldAttribute.SetWarning<POReceipt.curyUnbilledTaxTotal>(sender, e.Row, AR.Messages.TaxIsNotUptodate);
            }

            addLinePopupHandler = isTransferReceipt ? (PopupHandler)new AddTransferPopupHandler(this) : new AddReceiptPopupHandler(this);
            PXUIFieldAttribute.SetVisible<POReceipt.siteID>(sender, e.Row, isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.vendorID>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.vendorLocationID>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyID>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.autoCreateInvoice>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.invoiceNbr>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyVatTaxableTotal>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyVatExemptTotal>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyOrderTotal>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyDiscTot>(sender, e.Row, !isTransferReceipt);

            addPOOrder.SetVisible(!isTransferReceipt);
            addPOOrderLine.SetVisible(!isTransferReceipt);
            viewPOOrder.SetVisible(!isTransferReceipt);
            addTransfer.SetVisible(isTransferReceipt);
            addTransfer.SetEnabled(isWarehouseSet && isTransferReceipt);

            Taxes.AllowSelect = !isTransferReceipt;

            PXUIFieldAttribute.SetVisible<POReceipt.termsID>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.invoiceDate>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.dueDate>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.discDate>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyDiscAmt>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.taxZoneID>(sender, e.Row, !isTransferReceipt);

            PXUIFieldAttribute.SetVisible<POReceipt.unbilledQty>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyUnbilledTotal>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyUnbilledLineTotal>(sender, e.Row, !isTransferReceipt);
            PXUIFieldAttribute.SetVisible<POReceipt.curyUnbilledTaxTotal>(sender, e.Row, !isTransferReceipt);

            PXUIFieldAttribute.SetEnabled<LandedCostTran.aPDocType>(this.landedCostTrans.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<LandedCostTran.aPRefNbr>(this.landedCostTrans.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<LandedCostTran.iNDocType>(this.landedCostTrans.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<LandedCostTran.iNRefNbr>(this.landedCostTrans.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<LandedCostTran.lCTranID>(this.landedCostTrans.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<LandedCostTran.pOReceiptType>(this.landedCostTrans.Cache, null, false);
        }

		private bool isDeleting = false;
		protected virtual void POReceipt_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			POReceipt doc = (POReceipt)e.Row;
			isDeleting = true;

		}

		protected virtual void POReceipt_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POReceipt doc = (POReceipt)e.Row;
			if (doc.CuryOrderTotal < Decimal.Zero && doc.Hold == false)
			{
				if (sender.RaiseExceptionHandling<POReceipt.curyOrderTotal>(e.Row, doc.CuryLineTotal, new PXSetPropertyException(Messages.POReceiptTotalAmountMustBeNonNegative, typeof(POReceipt.curyOrderTotal).Name)))
				{
					throw new PXRowPersistingException(typeof(POReceipt.curyLineTotal).Name, null, Messages.POReceiptTotalAmountMustBeNonNegative, typeof(POReceipt.curyOrderTotal).Name);
				}
			}

            if (doc.CuryDiscTot > Math.Abs(doc.CuryLineTotal ?? 0m))
            {
                if (sender.RaiseExceptionHandling<POReceipt.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(Messages.DiscountGreaterLineTotal, PXErrorLevel.Error)))
                {
                    throw new PXRowPersistingException(typeof(POReceipt.curyDiscTot).Name, null, Messages.DiscountGreaterLineTotal);
                }
            }
		}

		protected virtual void POReceipt_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POReceipt.branchID>(e.Row);
			sender.SetDefaultExt<POReceipt.taxZoneID>(e.Row);
		}

		protected virtual void POReceipt_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceipt row = (POReceipt)e.Row;

			company.RaiseFieldUpdated(sender, e.Row);
			vendor.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<POReceipt.autoCreateInvoice>(row);

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<POReceipt.curyID>(e.Row) == null)
				{
				    CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<POReceipt.curyInfoID>(sender, e.Row);
    
				    string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				    if (string.IsNullOrEmpty(message) == false)
				    {
					    sender.RaiseExceptionHandling<POReceipt.receiptDate>(e.Row, ((POReceipt)e.Row).ReceiptDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				    }
    
				    if (info != null)
				    {
					    ((POReceipt)e.Row).CuryID = info.CuryID;
				    }
			    }
			}

			sender.SetDefaultExt<POReceipt.vendorLocationID>(e.Row);
    		sender.SetDefaultExt<POReceipt.termsID>(e.Row);

		}

		protected virtual void POReceipt_AutoCreateInvoice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceipt row = (POReceipt)e.Row;
			if ((bool)row.AutoCreateInvoice == false)
			{

				foreach (POReceiptLine it in this.transactions.Select())
				{
					this.transactions.SetValueExt<POReceiptLine.taxCategoryID>(it, it.TaxCategoryID);
					break;
				}
			}
		}

		protected virtual void POReceipt_InvoiceDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceipt row = (POReceipt)e.Row;
			if (row != null)
			{
				e.NewValue = row.ReceiptDate;
			}
		}

		protected virtual void POReceipt_TermsID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceipt row = (POReceipt)e.Row;
			if (row != null && row.ReceiptType == POReceiptType.POReturn)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region Receipt Line Events
		protected virtual void POReceiptLine_ReasonCode_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null)
			{
				if (row.ReceiptType == POReceiptType.POReturn)
				{
					e.NewValue = posetup.Current.RCReturnReasonCodeID;
				}
			}
		}

		protected object GetValue<Field>(object data)
			where Field : IBqlField
		{
			if (data == null) return null;
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

		protected virtual void POReceiptLine_POAccrualAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row != null)
			{
				if (IsAccrualAccountRequired(row))
				{
					if (row.ReceiptType == POReceiptType.POReceipt)
					{
						var item = (PXResult<InventoryItem, INPostClass, INSite>)
						PXSelectJoin<InventoryItem,
							LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>,
							LeftJoin<INSite, On<INSite.siteID, Equal<Required<POLine.siteID>>>>>,
						Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
						.Select(this, row.SiteID, row.InventoryID);

						if (item != null && ((INPostClass)item).PostClassID != null && (((InventoryItem)item).StkItem == true || ((InventoryItem)item).NonStockReceipt == true))
						{
							e.NewValue = INReleaseProcess.GetPOAccrualAcctID<INPostClass.pOAccrualAcctID>(this, ((INPostClass)item).POAccrualAcctDefault, item, item, item, vendor.Current);
							if (e.NewValue != null)
							{
								e.Cancel = true;
							}
						}
					}
					else
					{
						ReasonCode res = this.reasonCode.Select(row.ReasonCode);
						if (res != null)
						{
							e.NewValue = res.AccountID;
							e.Cancel = true;
						}
					}
				}
				else
				{
					e.NewValue = null;
					e.Cancel = true;
				}
			}
		}

		protected virtual void POReceiptLine_POAccrualSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row != null)
			{
				if (IsAccrualAccountRequired(row))
				{
					if (row.ReceiptType == POReceiptType.POReceipt)
					{
						var item = (PXResult<InventoryItem, INPostClass, INSite>)
						PXSelectJoin<InventoryItem,
							LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>,
							LeftJoin<INSite, On<INSite.siteID, Equal<Required<POLine.siteID>>>>>,
						Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
						.Select(this, row.SiteID, row.InventoryID);

						if (item != null && ((INPostClass)item).PostClassID != null && (((InventoryItem)item).StkItem == true || ((InventoryItem)item).NonStockReceipt == true))
						{
							try
							{
								e.NewValue = INReleaseProcess.GetPOAccrualSubID<INPostClass.pOAccrualSubID>(this, ((INPostClass)item).POAccrualAcctDefault, ((INPostClass)item).POAccrualSubMask, item, item, item, vendor.Current);
							}
							catch (PXMaskArgumentException) { }
							if (e.NewValue != null)
							{
								e.Cancel = true;
							}
						}
					}
					else
					{
						ReasonCode rc = this.reasonCode.Select(row.ReasonCode);
						if (rc != null)
						{
							if (row.InventoryID != null && row.SiteID != null && row.POAccrualAcctID != null)
							{
								PXResult<InventoryItem, INPostClass> res = (PXResult<InventoryItem, INPostClass>)PXSelectJoin<InventoryItem, InnerJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>>, Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>.Select(this, row.InventoryID);
								INSite invSite = PXSelect<INSite, Where<INSite.siteID, Equal<Required<POLine.siteID>>>>.Select(this, row.SiteID);
								if (res != null && invSite != null)
								{
									InventoryItem invItem = (InventoryItem)res;
									INPostClass postClass = (INPostClass)res;
									e.NewValue = this.GetReasonCodeSubID(rc, invItem, invSite, postClass);
									e.Cancel = true;
								}
							}
						}
					}
				}
				else
				{
					e.NewValue = null;
					e.Cancel = true;
				}
			}
		}


		protected virtual void POReceiptLine_ExpenseAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null)
			{
				if (row.ReceiptType == POReceiptType.POReturn )
				{
					ReasonCode res = this.reasonCode.Select(row.ReasonCode);
					if (res != null)
					{
						e.NewValue = res.AccountID;
						e.Cancel = true;
					}
				}
				else if (row.ReceiptType == POReceiptType.POReceipt)
				{
					if (String.IsNullOrEmpty(row.PONbr) || String.IsNullOrEmpty(row.POType) || row.POLineNbr == null)
					{
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
								e.NewValue = GetValue<Carrier.freightExpenseAcctID>(carrier) ?? posetup.Current.FreightExpenseAcctID;
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
				}
			}
		}

		protected virtual void POReceiptLine_ExpenseSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null)
			{
				if (row.ReceiptType == POReceiptType.POReturn )
				{
					ReasonCode rc = this.reasonCode.Select(row.ReasonCode);
					if (rc != null)
					{
						if (row.InventoryID != null && row.SiteID != null && row.ExpenseAcctID != null)
						{
							PXResult<InventoryItem, INPostClass> res = (PXResult<InventoryItem, INPostClass>)PXSelectJoin<InventoryItem, InnerJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>>, Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>.Select(this, row.InventoryID);
							INSite invSite = PXSelect<INSite, Where<INSite.siteID, Equal<Required<POLine.siteID>>>>.Select(this, row.SiteID);
							if (res != null && invSite != null)
							{
								InventoryItem invItem = (InventoryItem)res;
								INPostClass postClass = (INPostClass)res;
								e.NewValue = this.GetReasonCodeSubID(rc, invItem, invSite, postClass);
								e.Cancel = true;
							}
						}
					}
				}
				else if (row.ReceiptType == POReceiptType.POReceipt)
				{
					if (String.IsNullOrEmpty(row.PONbr) || String.IsNullOrEmpty(row.POType) || row.POLineNbr == null)
					{
						var item = (PXResult<InventoryItem, INPostClass, INSite>)
								PXSelectJoin<InventoryItem,
									LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>,
									LeftJoin<INSite, On<INSite.siteID, Equal<Required<POLine.siteID>>>>>,
								Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>
								.Select(this, row.SiteID, row.InventoryID);

						Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>
							.Select(this, location.Current.VCarrierID);

						EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>
							.Select(this, PXAccess.GetUserID());

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
								e.NewValue = GetValue<Carrier.freightExpenseSubID>(carrier) ?? posetup.Current.FreightExpenseSubID;
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
					else
					{
						e.Cancel = true;
					}
				}
			}
		}

		protected virtual void POReceiptLine_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (TaxAttribute.GetTaxCalc<POReceiptLine.taxCategoryID>(sender, e.Row) == TaxCalc.Calc && taxzone.Current != null && !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) && ((POReceiptLine)e.Row).InventoryID == null)
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
			}
			if (vendor != null && vendor.Current != null && (bool)vendor.Current.TaxAgency == true)
			{
				((POReceiptLine)e.Row).TaxCategoryID = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void POReceiptLine_InvtMult_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null)
			{
				e.NewValue = INTranType.InvtMult(row.TranType);
			}
		}

		protected virtual void POReceiptLine_ReceiptQty_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null)
			{
				e.NewValue = row.LineType == POLineType.Freight ? Decimal.One : Decimal.Zero;
			}
		}

		protected virtual void POReceiptLine_ReceiptQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row != null && row.ReceiptQty == 0)
			{
					sender.SetDefaultExt<POReceiptLine.curyUnitCost>(e.Row);
			}
		}

		protected virtual void POLine_SubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row != null)
			{
				sender.SetDefaultExt<POReceiptLine.curyUnitCost>(e.Row);
			}
		}

		protected virtual void POReceiptLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			this.inventoryIDChanging = true;
			sender.SetDefaultExt<POReceiptLine.unitVolume>(e.Row);
			sender.SetDefaultExt<POReceiptLine.unitWeight>(e.Row);
			sender.SetDefaultExt<POReceiptLine.uOM>(e.Row);
			sender.SetDefaultExt<POReceiptLine.tranDesc>(e.Row);
			sender.SetDefaultExt<POReceiptLine.siteID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.expenseAcctID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.expenseSubID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.pOAccrualAcctID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.pOAccrualSubID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.taxCategoryID>(e.Row);
			sender.SetDefaultExt<POReceiptLine.unitCost>(e.Row);
			sender.SetDefaultExt<POReceiptLine.curyUnitCost>(e.Row);
			sender.SetValue<POReceiptLine.unitCost>(e.Row, null);
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

		protected virtual void POReceiptLine_UOM_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (e.Row != null && !this.inventoryIDChanging)
			{
				row.LastBaseReceivedQty = row.BaseReceiptQty;
				//this code addresses in PO the problem with empty UOM, similar to the problem in SO (AC-76179)
                if (e.NewValue == null && (POLineType.IsStock(row.LineType) || (POLineType.IsNonStock(row.LineType) && row.InventoryID != null)))
                {
                    throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(POReceiptLine.uOM).Name);
                }
			}
		}

		protected virtual void POReceiptLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			string oldUOM = (string)e.OldValue;
			if (!this.inventoryIDChanging)
			{
				decimal qty = decimal.Zero;
				decimal curyUnitCost = row.CuryUnitCost ?? Decimal.Zero;
				if (row != null && (!string.IsNullOrEmpty(oldUOM) && !string.IsNullOrEmpty(row.UOM) && row.UOM != oldUOM))
				{
					if (row.LastBaseReceivedQty.HasValue && row.LastBaseReceivedQty.Value != decimal.Zero)
					{
						qty = INUnitAttribute.ConvertFromBase<POReceiptLine.inventoryID>(sender, e.Row, row.UOM, row.LastBaseReceivedQty.Value, INPrecision.QUANTITY);
					}

					curyUnitCost = INUnitAttribute.ConvertFromBase<POReceiptLine.inventoryID>(sender, e.Row, oldUOM, curyUnitCost, INPrecision.UNITCOST);
					curyUnitCost = INUnitAttribute.ConvertToBase<POReceiptLine.inventoryID>(sender, e.Row, row.UOM, curyUnitCost, INPrecision.UNITCOST);

				sender.SetValueExt<POReceiptLine.receiptQty>(e.Row, qty);
				sender.SetValueExt<POReceiptLine.curyUnitCost>(e.Row, curyUnitCost);
			}
		}
		}

		protected virtual void POReceiptLine_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (row != null && row.InventoryID != null)
			{
				if (String.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr) || row.POLineNbr.HasValue == false)
				{
					sender.SetDefaultExt<POReceiptLine.expenseAcctID>(e.Row);
					sender.SetDefaultExt<POReceiptLine.expenseSubID>(e.Row);
					sender.SetDefaultExt<POReceiptLine.pOAccrualAcctID>(e.Row);
					sender.SetDefaultExt<POReceiptLine.pOAccrualSubID>(e.Row);

					sender.SetDefaultExt<POReceiptLine.curyUnitCost>(e.Row);
				}
			}
		}

		protected virtual void POReceiptLine_LineType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POReceiptLine.receiptQty>(e.Row);
		}

        protected virtual void POReceiptLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            POReceiptLine row = (POReceiptLine)e.Row;
            if (row != null)
            {
                POLine poOriginLine = null;
                if (row.PONbr != null && row.POType != null && row.POLineNbr != null)
                {
                    poOriginLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                        And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                        And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
                }
                if (poOriginLine == null)
                {
                    e.NewValue = DefaultUnitCost(sender, row) ?? 0m;
                }
            }
        }

		protected virtual void POReceiptLine_CuryUnitCost_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			Decimal? value = (Decimal?)e.NewValue;
			if (value.HasValue && value < Decimal.Zero &&
				(row.LineType != POLineType.NonStock
				&& row.LineType != POLineType.Service
				&& row.LineType != POLineType.MiscCharges
				&& row.LineType != POLineType.Freight
				&& row.LineType != POLineType.NonStockForDropShip))
			{
				throw new PXSetPropertyException(Messages.UnitCostMustBeNonNegativeForStockItem);
			}
		}

		protected virtual void POReceiptLine_ReasonCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (String.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr) || row.POLineNbr.HasValue == false)
			{
				sender.SetDefaultExt<POReceiptLine.expenseAcctID>(e.Row);
				sender.SetDefaultExt<POReceiptLine.expenseSubID>(e.Row);
				sender.SetDefaultExt<POReceiptLine.pOAccrualAcctID>(e.Row);
				sender.SetDefaultExt<POReceiptLine.pOAccrualSubID>(e.Row);
			}
		}

		protected virtual void POReceiptLine_AllowComplete_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			e.ReturnValue = false;
			if (row != null
					&& String.IsNullOrEmpty(row.POType) == false
					&& String.IsNullOrEmpty(row.PONbr) == false)
			{

				POLineR source = PXSelect<POLineR, Where<POLineR.orderType,
							Equal<Required<POLineR.orderType>>,
			And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
				And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);

				e.ReturnValue = (source != null) ? source.AllowComplete : false;
			}
		}
		protected virtual void POReceiptLine_AllowOpen_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			e.ReturnValue = false;
			if (row != null
					&& String.IsNullOrEmpty(row.POType) == false
					&& String.IsNullOrEmpty(row.PONbr) == false)
			{

				POLineR source = PXSelect<POLineR, Where<POLineR.orderType,
							Equal<Required<POLineR.orderType>>,
				And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
				And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);

				e.ReturnValue = (source != null) ? source.AllowComplete : false;
			}
		}

		protected virtual void POReceiptLine_AllowComplete_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (String.IsNullOrEmpty(row.POType) == false
				&& String.IsNullOrEmpty(row.PONbr) == false
				&& row.POLineNbr.HasValue)
			{

				POLineR source = PXSelect<POLineR, Where<POLineR.orderType, Equal<Required<POLineR.orderType>>,
				And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
					And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				if (source != null && source.AllowComplete != (row.AllowComplete ?? false))
				{
					source.AllowComplete = (row.AllowComplete ?? false);
					this.Caches[typeof(POLine)].Update(source);
				}
				this.transactions.View.RequestRefresh();
			}
		}
		protected virtual void POReceiptLine_AllowOpen_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLine row = (POReceiptLine)e.Row;
			if (String.IsNullOrEmpty(row.POType) == false
				&& String.IsNullOrEmpty(row.PONbr) == false
				&& row.POLineNbr.HasValue)
			{

				POLineR source = PXSelect<POLineR, Where<POLineR.orderType, Equal<Required<POLineR.orderType>>,
				And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
					And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				if (source != null && source.AllowComplete != (row.AllowOpen ?? false))
				{
					source.AllowComplete = (row.AllowOpen ?? false);
					this.Caches[typeof(POLine)].Update(source);
				}
				this.transactions.View.RequestRefresh();
			}
		}


        protected virtual void POReceiptLine_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POReceiptLine row = e.Row as POReceiptLine;
            if (row != null && e.ExternalCall)
            {
                DiscountEngine<POReceiptLine>.UpdateManualLineDiscount<POReceiptDiscountDetail>(sender, transactions, row, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate);
            }
        }

		protected virtual void POReceiptLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            POReceiptLine row = e.Row as POReceiptLine;
            if (row != null)
			{
				bool fromPO = !(string.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr) || row.POLineNbr == null);
				bool isStockItem = POLineType.IsStock(row.LineType);
				bool isNonStockItem = POLineType.IsNonStock(row.LineType);
                bool isReceipt = (row.ReceiptType == POReceiptType.POReceipt);
				bool isReturn = (row.ReceiptType == POReceiptType.POReturn);
                bool isTransferReceipt = (row.ReceiptType == POReceiptType.TransferReceipt);

				bool isFreight = row.LineType == POLineType.Freight;
				bool isDropShip = (row.LineType == POLineType.GoodsForDropShip);
				bool isNonStockKit = (!row.IsStkItem ?? false) && (row.IsKit ?? false);

                PXUIFieldAttribute.SetEnabled<POReceiptLine.curyExtCost>(sender, row, false);
                PXUIFieldAttribute.SetEnabled<POReceiptLine.uOM>(sender, row, (isStockItem || row.LineType == POLineType.NonStock));
				PXUIFieldAttribute.SetEnabled<POReceiptLine.inventoryID>(sender, row, (!fromPO && !isFreight));
				PXUIFieldAttribute.SetEnabled<POReceiptLine.lineType>(sender, row, !fromPO);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.projectID>(sender, row, !isStockItem || isDropShip);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.taskID>(sender, row, !isStockItem || isDropShip);

				PXUIFieldAttribute.SetEnabled<POReceiptLine.subItemID>(sender, row, isStockItem && !isNonStockKit);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.curyUnitCost>(sender, row, isStockItem || isNonStockItem);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.curyLineAmt>(sender, row, isStockItem || isNonStockItem || isFreight);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.receiptQty>(sender, row, isStockItem || isNonStockItem);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.baseReceiptQty>(sender, row, isStockItem || isNonStockItem);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.locationID>(sender, e.Row, isStockItem && (!isDropShip));
				PXUIFieldAttribute.SetEnabled<POReceiptLine.allowComplete>(sender, e.Row, fromPO);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.allowOpen>(sender, e.Row, fromPO);
				
				if (isDropShip)
				{
					PXUIFieldAttribute.SetEnabled<POReceiptLine.lotSerialNbr>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POReceiptLine.expireDate>(sender, e.Row, false);
				}

				if (isReturn)
				{
					PXUIFieldAttribute.SetEnabled<POReceiptLine.expenseAcctID>(sender, e.Row, isNonStockItem);
					PXUIFieldAttribute.SetEnabled<POReceiptLine.expenseSubID>(sender, e.Row, isNonStockItem);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled<POReceiptLine.expenseAcctID>(sender, e.Row, IsExpenseAccountRequired(row));
					PXUIFieldAttribute.SetEnabled<POReceiptLine.expenseSubID>(sender, e.Row, IsExpenseAccountRequired(row));
				}
				
				PXUIFieldAttribute.SetEnabled<POReceiptLine.pOType>(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.pONbr>(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.pOLineNbr>(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<POReceiptLine.reasonCode>(sender, e.Row, isReturn);
				PXUIFieldAttribute.SetRequired<POReceiptLine.reasonCode>(sender, isReturn);

                PXUIFieldAttribute.SetEnabled<POReceiptLine.pOAccrualAcctID>(sender, e.Row, IsAccrualAccountRequired(row));
                PXUIFieldAttribute.SetEnabled<POReceiptLine.pOAccrualSubID>(sender, e.Row, IsAccrualAccountRequired(row));

                PXUIFieldAttribute.SetEnabled<POReceiptLine.sOOrderType>(sender, e.Row, false);
                PXUIFieldAttribute.SetEnabled<POReceiptLine.sOOrderNbr>(sender, e.Row, false);
                PXUIFieldAttribute.SetEnabled<POReceiptLine.sOOrderLineNbr>(sender, e.Row, false);
            }

            if (Document.Current != null && (e.Row == null || this.IsImport == false))
            {
                bool isReceipt = (Document.Current.ReceiptType == POReceiptType.POReceipt);
                bool isReturn = (Document.Current.ReceiptType == POReceiptType.POReturn);
                bool isTransferReceipt = (Document.Current.ReceiptType == POReceiptType.TransferReceipt);

                PXUIFieldAttribute.SetVisible<POReceiptLine.curyUnitCost>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.curyExtCost>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.curyLineAmt>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.taxCategoryID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.expenseAcctID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.expenseSubID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.pOAccrualAcctID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.pOAccrualSubID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.projectID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.taskID>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.pOType>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.pONbr>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.pOLineNbr>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.manualDisc>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.curyDiscAmt>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.discPct>(sender, null, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.discountID>(sender, null, !isTransferReceipt);

                PXUIFieldAttribute.SetVisible<POReceiptLine.reasonCode>(sender, null, isReturn);
                PXUIFieldAttribute.SetVisible<POReceiptLine.allowComplete>(sender, null, isReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.allowOpen>(sender, null, isReturn);
                PXUIFieldAttribute.SetVisible<POReceiptLine.sOOrderType>(sender, null, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.sOOrderNbr>(sender, null, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLine.sOOrderLineNbr>(sender, null, isTransferReceipt);
				PXUIFieldAttribute.SetVisible<POReceiptLine.sOShipmentNbr>(sender, null, isTransferReceipt);
        }
        }
		
        public virtual void VerifyTransferLine(PXCache sender, POReceiptLine row)
        {
            if (row.ReceiptType != POReceiptType.TransferReceipt || (Document.Current.Released ?? false))
                return;

            POReceiptLine check1 = PXSelectReadonly2<POReceiptLine, InnerJoin<POReceipt, On<POReceiptLine.receiptType, Equal<POReceipt.receiptType>, And<POReceiptLine.receiptNbr, Equal<POReceipt.receiptNbr>>>>,
                Where<POReceiptLine.receiptType, Equal<Current<POReceiptLine.receiptType>>,
                And<POReceiptLine.receiptNbr, NotEqual<Current<POReceiptLine.receiptNbr>>,
                And<POReceipt.released, NotEqual<True>,
                And<POReceiptLine.origRefNbr, Equal<Current<POReceiptLine.origRefNbr>>,
                And<POReceiptLine.origLineNbr, Equal<Current<POReceiptLine.origLineNbr>>>>>>>>.SelectSingleBound(this, new object[] { row });

            if (check1 != null)
                throw new PXRowPersistingException(typeof(POReceiptLine.lineNbr).Name, row.LineNbr, Messages.LineCannotBeReceiptedTwicePO, check1.ReceiptNbr, check1.LineNbr);

            INTran check2 = PXSelectReadonly<INTran, 
                Where<INTran.docType, Equal<INDocType.receipt>,
                And2<
                    Where<INTran.pOReceiptType, NotEqual<Current<POReceiptLine.receiptType>>,
                    Or<INTran.pOReceiptNbr, NotEqual<Current<POReceiptLine.receiptNbr>>,
                    Or<INTran.pOReceiptLineNbr, NotEqual<Current<POReceiptLine.lineNbr>>>>>,
                And<INTran.origRefNbr, Equal<Current<POReceiptLine.origRefNbr>>,
                And<INTran.origLineNbr, Equal<Current<POReceiptLine.origLineNbr>>>>>>>.SelectSingleBound(this, new object[] { row });

            if (check2 != null)
                throw new PXRowPersistingException(typeof(POReceiptLine.lineNbr).Name, row.LineNbr, Messages.LineCannotBeReceiptedTwiceIN, check2.RefNbr, check2.LineNbr);
        }

        protected virtual void POReceiptLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            POReceiptLine row = (POReceiptLine)e.Row;
            if (row != null)
            {
                if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
                    VerifyTransferLine(sender, row);
                bool isReturn = (row.ReceiptType == POReceiptType.POReturn);
                bool fromPO = !(string.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr) || row.POLineNbr == null);
                bool isStockItem = (row.LineType == POLineType.GoodsForInventory ||
                    row.LineType == POLineType.GoodsForSalesOrder ||
                    row.LineType == POLineType.GoodsForReplenishment ||
                    row.LineType == POLineType.GoodsForDropShip);
				bool isNonStockKit = (!row.IsStkItem ?? false) && (row.IsKit ?? false);
                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.inventoryID>(sender, row, POLineType.IsStock(row.LineType) || POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POReceiptLine.subItemID>(sender, row, (isStockItem && !isNonStockKit) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.receiptQty>(sender, row, (isStockItem || row.LineType == POLineType.NonStock) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.baseReceiptQty>(sender, row, (isStockItem || row.LineType == POLineType.NonStock) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

				PXDefaultAttribute.SetPersistingCheck<POReceiptLine.expenseAcctID>(sender, e.Row, (!IsExpenseAccountRequired(row) || (row.LineType == POLineType.NonStock && isReturn)) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
				PXDefaultAttribute.SetPersistingCheck<POReceiptLine.expenseSubID>(sender, e.Row, (!IsExpenseAccountRequired(row) || (row.LineType == POLineType.NonStock && isReturn)) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.pOAccrualAcctID>(sender, e.Row, IsAccrualAccountRequired(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<POReceiptLine.pOAccrualSubID>(sender, e.Row, IsAccrualAccountRequired(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.siteID>(sender, row, isReturn || row.LineType == POLineType.Description || row.LineType == POLineType.Freight || POLineType.IsService(row.LineType) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.uOM>(sender, e.Row, (POLineType.IsStock(row.LineType) || (POLineType.IsNonStock(row.LineType) && row.InventoryID != null)) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                
                PXDefaultAttribute.SetPersistingCheck<POReceiptLine.reasonCode>(sender, e.Row,
                    isReturn ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);

				if (row.ReceiptQty <= Decimal.Zero && Document.Current != null && Document.Current.Hold == false)
				{
					sender.RaiseExceptionHandling<POReceiptLine.receiptQty>(row, row.ReceiptQty,
						new PXSetPropertyException(Messages.POLineQuantityMustBeGreaterThanZero, PXErrorLevel.Error));
				}

                if (!(string.IsNullOrEmpty(row.POType) || string.IsNullOrEmpty(row.PONbr) || (row.LineNbr == null)))
                {
                    CheckRctForPOQuantityRule(sender, row, false);
                }
				
				if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update))
				{
				    CheckForSingleLocation(sender, row);
				    CheckSplitsForSameTask(sender, row);
				    CheckLocationTaskRule(sender, row, row.LocationID);
					CheckOrderTaskRule(sender, row, row.TaskID);
				}

				bool hasDeductibleVAT = false;
				if (row.InventoryID != null)
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.
						 SelectWindowed(this, 0, 1, row.InventoryID);
					if (item.StkItem == true || item.NonStockReceipt == true)
					{
						hasDeductibleVAT = PXSelectJoin<POReceiptTax, InnerJoin<Tax, On<POReceiptTax.taxID, Equal<Tax.taxID>>>,
							Where<POReceiptTax.receiptNbr, Equal<Required<POReceiptTax.receiptNbr>>,
								And<POReceiptTax.lineNbr, Equal<Required<POReceiptTax.lineNbr>>,
								And<Tax.deductibleVAT, Equal<True>>>>>.Select(this, row.ReceiptNbr, row.LineNbr).Count != 0;
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
						new PXSetPropertyException<POReceiptLine.inventoryID>(Messages.DeductibleVATNotAllowed, PXErrorLevel.RowError));
					throw new PXException(ErrorMessages.RecordRaisedErrors, prefix, PXUIFieldAttribute.GetItemName(sender));
				}
			}
        }

		protected virtual bool IsExpenseAccountRequired(POReceiptLine line)
		{
			return (line.LineType == POLineType.Description
				|| POLineType.IsStock(line.LineType)) == false || line.ReceiptType == POReceiptType.POReturn;
		}

        protected virtual bool IsAccrualAccountRequired(POReceiptLine line)
        {
			return line.ReceiptType != POReceiptType.POReturn && line.ReceiptType != POReceiptType.TransferReceipt && (POLineType.IsStock(line.LineType) || POLineType.IsNonStock(line.LineType) && !POLineType.IsService(line.LineType));
        }

        protected virtual void POReceiptLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
                POReceiptLine row = (POReceiptLine)e.Row;
                POReceiptLine oldRow = (POReceiptLine)e.OldRow;

	            if (IsCopyPasteContext && row.LineType == POLineType.GoodsForDropShip)
				{
					row.LineType = POLineType.GoodsForInventory;
				}

                ClearUnused(row, row.LineType);

                POLine poOriginLine = null;
				if (row.PONbr != null && row.POType != null && row.POLineNbr != null)
				{
					poOriginLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
						And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
						And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				}

                if (row.BaseReceiptQty != oldRow.BaseReceiptQty || row.LineAmt != oldRow.LineAmt)
                {
                    this.UpdatePOLineCompleteFlag(row, false, poOriginLine);
                }

                try
                {
                    CheckRctForPOQuantityRule(sender, row, true, poOriginLine);
                }
                finally
                {
                    this.inventoryIDChanging = false;
                }

				if (!sender.ObjectsEqual<POReceiptLine.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.inventoryID>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POReceiptLine.receiptQty>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.curyUnitCost>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POReceiptLine.curyLineAmt>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.curyDiscAmt>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<POReceiptLine.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.manualDisc>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POReceiptLine.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.siteID>(e.Row, e.OldRow))
                    RecalculateDiscounts(sender, row);

                if ((e.ExternalCall || sender.Graph.IsImport)
                    && sender.ObjectsEqual<POReceiptLine.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<POReceiptLine.uOM>(e.Row, e.OldRow)
                    && sender.ObjectsEqual<POReceiptLine.receiptQty>(e.Row, e.OldRow) && sender.ObjectsEqual<POReceiptLine.branchID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<POReceiptLine.siteID>(e.Row, e.OldRow)
                    && (!sender.ObjectsEqual<POReceiptLine.curyUnitCost>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptLine.curyLineAmt>(e.Row, e.OldRow)))
                    row.ManualPrice = true;

            TaxAttribute.Calculate<POReceiptLine.taxCategoryID>(sender, e);

            //if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
            if (Document.Current != null && IsExternalTax == true &&
                !sender.ObjectsEqual<POReceiptLine.inventoryID, POReceiptLine.tranDesc, POReceiptLine.extCost, POReceiptLine.receiptDate, POReceiptLine.taxCategoryID>(e.Row, e.OldRow))
            {
                Document.Current.IsTaxValid = false;
            }
        }

		protected virtual void POReceiptLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            POReceiptLine row = (POReceiptLine)e.Row;

			if (IsCopyPasteContext && row.LineType == POLineType.GoodsForDropShip)
			{
				row.LineType = POLineType.GoodsForInventory;
			}

			ClearUnused(row, row.LineType);
            POReceipt doc = (POReceipt)this.Document.Current;
            POLine poOriginLine = null;
            if (row.PONbr != null && row.POType != null && row.POLineNbr != null)
            {
                poOriginLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                    And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                    And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
                this.UpdatePOLineCompleteFlag(row, false, poOriginLine);
            }
            if (poOriginLine == null && (row.CuryUnitCost == null || row.CuryUnitCost == Decimal.Zero))
            {
                DefaultUnitCost(sender, row);
            }

            RecalculateDiscounts(sender, (POReceiptLine)e.Row);

            try
            {
                CheckRctForPOQuantityRule(sender, row, true, poOriginLine);
            }
            finally
            {
                this.inventoryIDChanging = false;
            }
			
            TaxAttribute.Calculate<POReceiptLine.taxCategoryID>(sender, e);
        }

        protected virtual void POReceiptLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            POReceiptLine row = (POReceiptLine)e.Row;
            if (row.PONbr != null && row.POType != null)
            {
                if (row.POLineNbr != null && row.BaseReceiptQty >= 0)
                    this.UpdatePOLineCompleteFlag(row, true);
                this.DeleteUnusedReference(row, this.isDeleting);
            }

            if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
            {
                DiscountEngine<POReceiptLine>.RecalculateGroupAndDocumentDiscounts(sender, transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate);
                RecalculateTotalDiscount();
            }
        }

        protected virtual void POReceiptLine_ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            POReceiptLine row = e.Row as POReceiptLine;
            if (row == null) return;
            if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO))
            {
                if (PM.POActiveProjectAttribute.IsRequired(row.LineType))
                {
                    if (location.Current != null && location.Current.VDefProjectID != null)
                    {
                        PX.Objects.PM.PMProject project = PXSelect<PM.PMProject, Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>.Select(this, location.Current.VDefProjectID);
                        if (project != null)
                            e.NewValue = project.ContractCD;
                    }
                }

                bool isStockItem = (row.LineType == POLineType.GoodsForInventory ||
                    row.LineType == POLineType.GoodsForSalesOrder ||
                    row.LineType == POLineType.GoodsForReplenishment ||
                    row.LineType == POLineType.GoodsForDropShip);

                if (isStockItem)
                {
                    if (row.LocationID != null)
                    {
                        PXResultset<INLocation> result = PXSelectJoin<INLocation,
                            LeftJoin<PMProject, On<PMProject.contractID, Equal<INLocation.projectID>>>,
                            Where<INLocation.siteID, Equal<Required<INLocation.siteID>>,
                            And<INLocation.locationID, Equal<Required<INLocation.locationID>>>>>.Select(sender.Graph, row.SiteID, row.LocationID);

                        foreach (PXResult<INLocation, PMProject> res in result)
                        {
                            PMProject project = (PMProject)res;
							if (project != null && project.ContractCD != null && project.VisibleInPO == true)
                            {
                                e.NewValue = project.ContractCD;
                                return;
                            }
                        }

                    }

                }
            }
        }

		protected virtual void POReceiptLine_TaskID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row == null) return;
			if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.PO))
			{
				bool isStockItem = (row.LineType == POLineType.GoodsForInventory ||
					row.LineType == POLineType.GoodsForSalesOrder ||
					row.LineType == POLineType.GoodsForReplenishment ||
					row.LineType == POLineType.GoodsForDropShip);

				if (isStockItem)
				{
					if (row.LocationID != null && ProjectDefaultAttribute.IsProject(this, row.ProjectID) )
					{
						PXResultset<PMTask> tasks = PXSelectJoin<PMTask,
									LeftJoin<INLocation, On<PMTask.projectID, Equal<INLocation.projectID>, And<INLocation.active, Equal<True>>>>,
									Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.visibleInPO, Equal<True>>>>.Select(this, row.ProjectID);

						HashSet<int> validTasks = new HashSet<int>();
						HashSet<int> tasksForLocation = new HashSet<int>();
						foreach (PXResult<PMTask, INLocation> res in tasks)
						{
							PMTask task = (PMTask)res;
							INLocation location = (INLocation)res;

							validTasks.Add(task.TaskID.Value);

							if (task.TaskID == location.TaskID)
							{
								tasksForLocation.Add(task.TaskID.Value);
							}
						}

						POLine poLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
								And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
								And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);

						if (poLine != null && poLine.TaskID != null)
						{
							if (tasksForLocation.Contains(poLine.TaskID.Value))
							{
								e.NewValue = poLine.TaskID;
								return;
							}
						}

						if (tasksForLocation.Count > 0)
						{
							e.NewValue = tasksForLocation.First();
							return;
						}

						if (validTasks.Count > 0)
						{
							e.NewValue = validTasks.First();
							return;
						}
					}

				}
			}
		}

		protected virtual void POReceiptLine_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POReceiptLine row = e.Row as POReceiptLine;
			if (row == null) return;
			if (!(e.NewValue is Int32)) return;

			CheckOrderTaskRule(sender, row, (int?)e.NewValue);
		}
		
        protected virtual void POReceiptLine_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POReceiptLine row = e.Row as POReceiptLine;
            if (row == null) return;

            bool isStockItem = (row.LineType == POLineType.GoodsForInventory ||
                    row.LineType == POLineType.GoodsForSalesOrder ||
                    row.LineType == POLineType.GoodsForReplenishment ||
                    row.LineType == POLineType.GoodsForDropShip);

            if (isStockItem)//For StockItems the Project and Task is a determined by Warehouse Location.
            {
                sender.SetDefaultExt<POReceiptLine.projectID>(e.Row); //will set pending value for TaskID to null if project is changed. This is the desired behavior for all other screens.
				if (sender.GetValuePending<POReceiptLine.taskID>(e.Row) == null) //To redefault the TaskID in currecnt screen - set the Pending value from NULL to NOTSET
					sender.SetValuePending<POReceiptLine.taskID>(e.Row, PXCache.NotSetValue);
				sender.SetDefaultExt<POReceiptLine.taskID>(e.Row);
            }
        }
		
		protected virtual void POReceiptLine_LocationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	    {
			POReceiptLine row = e.Row as POReceiptLine;
			if (row == null) return;

			CheckLocationTaskRule(sender, row, e.NewValue);
	    }
		
		#endregion

        #region POLine Events
        protected virtual void POLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        #region LandedCostTran Events

        protected virtual void LandedCostTran_LandedCostCodeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            if (e.Row != null)
            {
                LandedCostTran row = (LandedCostTran)e.Row;
                sender.SetDefaultExt<LandedCostTran.descr>(e.Row);
                sender.SetDefaultExt<LandedCostTran.vendorID>(e.Row);
                sender.SetDefaultExt<LandedCostTran.vendorLocationID>(e.Row);
                sender.SetDefaultExt<LandedCostTran.taxCategoryID>(e.Row);
                sender.SetDefaultExt<LandedCostTran.lCAccrualAcct>(e.Row);
                sender.SetDefaultExt<LandedCostTran.lCAccrualSub>(e.Row);
                sender.SetDefaultExt<LandedCostTran.termsID>(e.Row);
                doCancel0 = true;
            }
        }

        protected virtual void LandedCostTran_PostponeAP_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.Row != null)
            {
                sender.SetDefaultExt<LandedCostTran.lCAccrualAcct>(e.Row);
                sender.SetDefaultExt<LandedCostTran.lCAccrualSub>(e.Row);
            }
        }
        protected virtual void LandedCostTran_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.Row != null)
            {
                LandedCostTran row = (LandedCostTran)e.Row;
                if (!doCancel0)
                {

                    if (row.VendorID != null)
                    {
                        CRLocation loc = PXSelectJoin<CRLocation, InnerJoin<BAccount, On<CRLocation.locationID,
                                        Equal<BAccount.defLocationID>,
                                        And<CRLocation.bAccountID, Equal<BAccount.bAccountID>>>>,
                                        Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(this, row.VendorID);
                        if (loc != null)
                        {
                            sender.SetValue<LandedCostTran.vendorLocationID>(e.Row, loc.LocationID);
                            sender.SetValuePending<LandedCostTran.vendorLocationID>(e.Row, loc.LocationCD);
                        }
                    }
                }
                sender.SetDefaultExt<LandedCostTran.curyID>(e.Row);
                sender.SetValuePending<LandedCostTran.curyID>(e.Row, ((LandedCostTran)e.Row).CuryID);

            }
        }

        protected virtual void LandedCostTran_VendorLocationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (doCancel0)
            {
                e.NewValue = ((LandedCostTran)e.Row).VendorLocationID;
                e.Cancel = true;
                doCancel0 = false;
            }
        }

        protected virtual void LandedCostTran_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetDefaultExt<LandedCostTran.taxCategoryID>(e.Row);
        }


        protected virtual void LandedCostTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            LandedCostTran row = (LandedCostTran)e.Row;
            if (row != null && e.Operation != PXDBOperation.Delete)
            {
                PXDefaultAttribute.SetPersistingCheck<LandedCostTran.invoiceDate>(sender, e.Row, row.PostponeAP == true ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
                bool isInvoiceNbrNotRequired = (bool)apsetup.Current.RequireVendorRef == false ? true : row.PostponeAP ?? false;
                PXDefaultAttribute.SetPersistingCheck<LandedCostTran.invoiceNbr>(sender, e.Row, isInvoiceNbrNotRequired ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
            }
        }

        protected virtual void LandedCostTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            LandedCostTran row = (LandedCostTran)e.Row;
            if (!string.IsNullOrEmpty(row.INRefNbr) || (!string.IsNullOrEmpty(row.APRefNbr)))
                throw new PXException(Messages.LandedCostTranIsReferenced);

        }

        #endregion

        #region POOrderFilter events
        protected virtual void POOrderFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            POOrderFilter row = (POOrderFilter)e.Row;
            POReceipt doc = this.Document.Current;
            if (row != null && doc != null)
            {
                if (!String.IsNullOrEmpty(doc.POType))
                    row.OrderType = doc.POType;
                if (doc.ShipToBAccountID.HasValue)
                    row.ShipToBAccountID = doc.ShipToBAccountID;
                if (doc.ShipToLocationID.HasValue)
                    row.ShipToLocationID = doc.ShipToLocationID;
                PXUIFieldAttribute.SetEnabled<POOrderFilter.orderType>(sender, row, (String.IsNullOrEmpty(doc.POType)));

                {
                    PXStringListAttribute.SetList<POOrderFilter.orderType>(sender, null,
                        new string[] { POOrderType.RegularOrder, POOrderType.DropShip },
                        new string[] { Messages.RegularOrder, Messages.DropShip });

                    PXUIFieldAttribute.SetEnabled<POOrderFilter.shipToBAccountID>(sender, row, (!doc.ShipToBAccountID.HasValue));
                    PXUIFieldAttribute.SetEnabled<POOrderFilter.shipToLocationID>(sender, row, (!doc.ShipToLocationID.HasValue));
                    PXUIFieldAttribute.SetVisible<POOrderFilter.shipToBAccountID>(sender, row,
                                                                                                                                                (row.OrderType == POOrderType.DropShip ||
                                                                                                                                                 string.IsNullOrEmpty(row.OrderType)));
                    PXUIFieldAttribute.SetVisible<POOrderFilter.shipToLocationID>(sender, row,
                                                                                                                                                (row.OrderType == POOrderType.DropShip ||
                                                                                                                                                 string.IsNullOrEmpty(row.OrderType)));
                }
                bool vendorVisible = row.VendorID == null && this.Document.Current.VendorID == null;
                PXUIFieldAttribute.SetVisible<POLineS.orderNbr>(this.Caches[typeof(POLineS)], null, row.OrderNbr == null);
                PXUIFieldAttribute.SetVisible<POLineS.vendorID>(this.Caches[typeof(POLineS)], null, vendorVisible);
                PXUIFieldAttribute.SetVisible<POLineS.vendorLocationID>(this.Caches[typeof(POLineS)], null, vendorVisible);
                //	this.addPOOrderLine2.SetVisible(row.AllowAddLine == true);
                this.addPOOrderLine2.SetEnabled(row.AllowAddLine == true);

            }
        }

        protected virtual void POOrderFilter_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POOrderFilter row = (POOrderFilter)e.Row;
            this.poLinesSelection.Cache.Clear();
        }

        protected virtual void POOrderFilter_OrderType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POOrderFilter row = (POOrderFilter)e.Row;
            sender.SetDefaultExt<POOrderFilter.shipToBAccountID>(e.Row);
            sender.SetValuePending<POOrderFilter.orderNbr>(e.Row, null);
            this.poLinesSelection.Cache.Clear();
            this.openOrders.Cache.Clear();
        }

        protected virtual void POOrderFilter_ShipToBAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POOrderFilter row = (POOrderFilter)e.Row;
            sender.SetDefaultExt<POOrderFilter.shipToLocationID>(e.Row);
            this.poLinesSelection.Cache.Clear();
            this.openOrders.Cache.Clear();
        }
        #endregion

        #region AddReceiptLine Events
        protected virtual void POReceiptLineS_BarCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.ExternalCall)
            {
                var rec = (PXResult<INItemXRef, InventoryItem, INSubItem>)
                          PXSelectJoin<INItemXRef,
                            InnerJoin<InventoryItem,
                                            On<InventoryItem.inventoryID, Equal<INItemXRef.inventoryID>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noPurchases>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.markedForDeletion>>>>>,
                            InnerJoin<INSubItem,
                                         On<INSubItem.subItemID, Equal<INItemXRef.subItemID>>>>,
                            Where<INItemXRef.alternateID, Equal<Current<POReceiptLineS.barCode>>,
                                            And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>
                            .SelectSingleBound(this, new object[] { e.Row });
                if (rec != null)
                {
                    sender.SetValue<POReceiptLineS.inventoryID>(e.Row, null);
                    sender.SetValuePending<POReceiptLineS.inventoryID>(e.Row, ((InventoryItem)rec).InventoryCD);
                    sender.SetValuePending<POReceiptLineS.subItemID>(e.Row, ((INSubItem)rec).SubItemCD);
                }
                else
                {
                    sender.SetValuePending<POReceiptLineS.inventoryID>(e.Row, null);
                    sender.SetValuePending<POReceiptLineS.subItemID>(e.Row, null);
                }
            }
        }
        protected virtual void POReceiptLineS_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.ExternalCall)
            {
                POReceiptLineS row = e.Row as POReceiptLineS;
                if (e.OldValue != null && row.InventoryID != null)
                    row.BarCode = null;
                sender.SetDefaultExt<POReceiptLineS.subItemID>(e);
            }
        }

        protected virtual void POReceiptLineS_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            POReceiptLineS row = e.Row as POReceiptLineS;
            if (row != null && row.InventoryID != null && row.SubItemID != null && !string.IsNullOrEmpty(row.UOM))
            {
                POReceipt doc = this.Document.Current;
                if (doc != null)
                {
                    decimal? curyUOMUnitCost = POItemCostManager.Fetch<POReceiptLineS.inventoryID, POReceiptLineS.curyInfoID>(sender.Graph, row,
                        row.VendorID, row.VendorLocationID, doc.ReceiptDate,
                        doc.CuryID, row.InventoryID, row.SubItemID, row.SiteID, row.UOM);

                    if (!curyUOMUnitCost.HasValue)
                        curyUOMUnitCost = Decimal.Zero;
                    e.NewValue = curyUOMUnitCost;
                }
            }
        }
        protected virtual void POReceiptLineS_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            POReceiptLineS row = e.NewRow as POReceiptLineS;
            POReceiptLineS old = e.Row as POReceiptLineS;
            if (!e.ExternalCall || row == null || old == null)
                return;

            if (row.ByOne != old.ByOne && row.ByOne == true)
                row.Qty = 1m;

            if (row.InventoryID != null &&
                    (row.InventoryID != old.InventoryID ||
                     row.SubItemID != old.SubItemID ||
                     row.VendorLocationID != old.VendorLocationID ||
                     row.ShipFromSiteID != old.ShipFromSiteID ||
                     row.FetchMode == true))
            {
                if (addLinePopupHandler.View.Answer == WebDialogResult.None)
                {
                    this.filter.Cache.Remove(this.filter.Current);
                    this.filter.Cache.Insert(new POOrderFilter()
                    {
                        VendorID = row.VendorID,
                        VendorLocationID = row.VendorLocationID,
                        ShipFromSiteID = row.ShipFromSiteID,
                        OrderType = row.POType,
                        OrderNbr = row.PONbr,
                        InventoryID = row.InventoryID,
                        SubItemID = row.SubItemID,
                        ResetFilter = true,
                        AllowAddLine = false
                    });
                }

                addLinePopupHandler.TryGetSourceItem(row);
            }
        }

        public abstract class PopupHandler
        {
            protected POReceiptEntry _graph;
            public PopupHandler(POReceiptEntry graph)
            {
                _graph = graph;
            }

            public abstract PXView View { get; }
            public abstract object GetSourceItem();
            public abstract void TryGetSourceItem(POReceiptLineS filter);
            public abstract void SetFilterToSource(PXCache sender, POReceiptLineS filter, object _source);
            public abstract void SetFilterToError(PXCache sender, POReceiptLineS filter);
        }

        public class AddReceiptPopupHandler : PopupHandler
        {
            public AddReceiptPopupHandler(POReceiptEntry graph)
                : base(graph)
            {
            }

            public override PXView View
            {
                get
                {
                    return _graph.poLinesSelection.View;
                }
            }

            public override void TryGetSourceItem(POReceiptLineS filter)
            {
                PXResultset<POLineS> polineResultSet = _graph.poLinesSelection.Select();
                POLineS source = null;
                int openLineCount = 0;
                foreach (POLineS itemsource in polineResultSet)
                {
                    POLineR polineR = _graph.purchaseLinesUPD.Search<POLineR.orderType, POLineR.orderNbr, POLineR.lineNbr>(
                                                                itemsource.OrderType, itemsource.OrderNbr, itemsource.LineNbr);
                    if (polineR == null || itemsource.OrderQty - polineR.ReceivedQty <= 0)
                    {
                        itemsource.Selected = false;
                        continue;
                    }
                    if (itemsource.Selected == true)
                    {
                        source = itemsource;
                        openLineCount = 1;
                        break;
                    }
                    openLineCount += 1;
                    if (source == null) source = itemsource;
                }

                if (openLineCount > 1)
                {
                    filter.FetchMode = true;
                    source = null;
                    if (_graph.poLinesSelection.AskExt((graph, view) => graph.Views[view].Cache.Clear()) == WebDialogResult.OK)
                    {
                        foreach (POLineS s in _graph.poLinesSelection.Cache.Updated)
                        {
                            if (s.Selected == true)
                                source = s;
                        }
                    }
                    if (source == null && _graph.poLinesSelection.View.Answer == WebDialogResult.OK)
                        _graph.poLinesSelection.AskExt((graph, view) => graph.Views[view].Cache.Clear());

                }
                _graph.poLinesSelection.View.SetAnswer(null, WebDialogResult.None);
                filter.FetchMode = false;
            }

            public override object GetSourceItem()
        {
                PXResultset<POLineS> polineResultSet = _graph.poLinesSelection.Select();
                POLineS source = null;
                int openLineCount = 0;
                foreach (POLineS itemsource in polineResultSet)
                {
                    POLineR polineR = _graph.purchaseLinesUPD.Search<POLineR.orderType, POLineR.orderNbr, POLineR.lineNbr>(
                                                                itemsource.OrderType, itemsource.OrderNbr, itemsource.LineNbr);
                    if (polineR == null ||
                        (_graph.Document.Current.ReceiptType == POReceiptType.POReceipt && itemsource.OrderQty - polineR.ReceivedQty <= 0) ||
                        (_graph.Document.Current.ReceiptType == POReceiptType.POReturn && polineR.ReceivedQty <= 0))
                    {
                        itemsource.Selected = false;
                        continue;
                    }
                    if (itemsource.Selected == true)
                    {
                        source = itemsource;
                        openLineCount = 1;
                        break;
                    }
                    openLineCount += 1;
                    if (source == null) source = itemsource;
                }

                if (openLineCount > 1)
                {
                    source = null;
                    if (_graph.poLinesSelection.View.Answer == WebDialogResult.OK)
                    {
                        foreach (POLineS s in _graph.poLinesSelection.Cache.Updated)
                        {
                            if (s.Selected == true)
                                source = s;
                        }
                    }
                }

                return source;
            }

            public override void SetFilterToSource(PXCache sender, POReceiptLineS filter, object _source)
                {
                POLineS source = _source as POLineS;

                if (filter.VendorID == null)
                    {
                        POOrder order = PXSelect<POOrder,
                            Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                                And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                        .SelectWindowed(_graph, 0, 1, source.OrderType, source.OrderNbr);
                    filter.VendorID = order.VendorID;
                    filter.VendorLocationID = order.VendorLocationID;
                    }
                filter.SubItemID = source.SubItemID;
                filter.UOM = source.UOM;
                filter.SiteID = source.SiteID;
                filter.POType = source.OrderType;
                filter.PONbr = source.OrderNbr;
                filter.POLineNbr = source.LineNbr;
                filter.CuryUnitCost = source.CuryUnitCost;
                sender.SetDefaultExt<POReceiptLineS.locationID>(filter);

                var rec = (PXResult<INLotSerClass, InventoryItem>)
                        PXSelectJoin<INLotSerClass,
                            InnerJoin<InventoryItem, On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>,
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .SelectWindowed(_graph, 0, 1, filter.InventoryID);
                INLotSerClass lotclass = rec;
                InventoryItem item = rec;

                Decimal? openQty = null;
                Decimal? baseOpenQty = null;
                if (source != null)
                {
                    POLineR polineR = source != null
                                        ? _graph.purchaseLinesUPD.Search<POLineR.orderType, POLineR.orderNbr, POLineR.lineNbr>(
                                            source.OrderType, source.OrderNbr, source.LineNbr)
                                        : null;

                    if (polineR != null)
                    {
                        openQty = _graph.Document.Current.ReceiptType == POReceiptType.POReceipt
                                    ? source.OrderQty - polineR.ReceivedQty
                                    : polineR.ReceivedQty;
                        baseOpenQty = _graph.Document.Current.ReceiptType == POReceiptType.POReceipt
                                        ? source.BaseOrderQty - polineR.BaseReceivedQty
                                        : polineR.BaseReceivedQty;
                    }
                    if (openQty < 0)
                    {
                        openQty = 0;
                        baseOpenQty = 0;
                    }
                }

                decimal qty = openQty.GetValueOrDefault();

                if (lotclass != null &&
                    lotclass.LotSerTrack == INLotSerTrack.SerialNumbered && lotclass.LotSerAssign == INLotSerAssign.WhenReceived)
                {
                    qty = 1;
                    filter.UOM = item.BaseUnit;
                }
                else if (filter.ByOne == true || source == null)
                    qty = 1;

                sender.SetValueExt<POReceiptLineS.receiptQty>(filter, qty);

                if (baseOpenQty != null && filter.BaseReceiptQty > baseOpenQty)
                    sender.SetValueExt<POReceiptLineS.receiptQty>(filter, 0m);

                if (source != null)
                    sender.SetValueExt<POReceiptLineS.curyUnitCost>(filter, source.CuryUnitCost);
            }

            public override void SetFilterToError(PXCache sender, POReceiptLineS filter)
            {
                filter.PONbr = null;
                filter.POLineNbr = null;
                sender.SetDefaultExt<POReceiptLineS.uOM>(filter);
                sender.SetDefaultExt<POReceiptLineS.siteID>(filter);
                sender.SetValueExt<POReceiptLineS.receiptQty>(filter, 0m);

                if (filter.VendorID != null)
                    sender.SetDefaultExt<POReceiptLineS.curyUnitCost>(filter);
                sender.RaiseExceptionHandling<POReceiptLineS.pONbr>
                    (filter, null, new PXSetPropertyException(Messages.POSourceNotFound, PXErrorLevel.Warning));
            }
        }

        public class AddTransferPopupHandler : PopupHandler
        {
            public AddTransferPopupHandler(POReceiptEntry graph)
                : base(graph)
            {
            }

            public override PXView View
            {
                get
                {
                    return _graph.intranSelection.View;
                }
            }

            public override void TryGetSourceItem(POReceiptLineS filter)
            {
                PXResultset<INTran> intranResultSet = _graph.intranSelection.Select();
                INTran source = null;
                int openLineCount = 0;
                foreach (INTran itemsource in intranResultSet)
                {
                    //if (false)
                    //{
                    //    itemsource.Selected = false;
                    //    continue;
                    //}
                    if (itemsource.Selected == true)
                    {
                        source = itemsource;
                        openLineCount = 1;
                        break;
                    }
                    openLineCount += 1;
                    if (source == null) source = itemsource;
                }

                if (openLineCount > 1)
                {
                    filter.FetchMode = true;
                    source = null;
                    if (_graph.intranSelection.AskExt((graph, view) => graph.Views[view].Cache.Clear()) == WebDialogResult.OK)
                    {
                        foreach (INTran s in _graph.intranSelection.Cache.Updated)
                        {
                            if (s.Selected == true)
                                source = s;
                        }
                    }
                    if (source == null && _graph.intranSelection.View.Answer == WebDialogResult.OK)
                        _graph.intranSelection.AskExt((graph, view) => graph.Views[view].Cache.Clear());

                }
                _graph.intranSelection.View.SetAnswer(null, WebDialogResult.None);
                filter.FetchMode = false;
            }


            public override object GetSourceItem()
            {
                PXResultset<INTran> intranResultSet = _graph.intranSelection.Select();
                INTran source = null;
                int openLineCount = 0;
                foreach (INTran itemsource in intranResultSet)
                {
                    //if (false)
                    //{
                    //    itemsource.Selected = false;
                    //    continue;
                    //}
                    if (itemsource.Selected == true)
                    {
                        source = itemsource;
                        openLineCount = 1;
                        break;
                    }
                    openLineCount += 1;
                    if (source == null) source = itemsource;
                }

                if (openLineCount > 1)
                {
                    source = null;
                    if (_graph.intranSelection.View.Answer == WebDialogResult.OK)
                    {
                        foreach (INTran s in _graph.intranSelection.Cache.Updated)
                        {
                            if (s.Selected == true)
                                source = s;
                        }
                    }
                }

                return source;
                }

            public override void SetFilterToSource(PXCache sender, POReceiptLineS filter, object _source)
            {
                INTran source = _source as INTran;

                //if (filter.VendorID == null)
                //{
                //    POOrder order = PXSelect<POOrder,
                //        Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                //            And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                //        .SelectWindowed(_graph, 0, 1, source.OrderType, source.OrderNbr);
                //    filter.VendorID = order.VendorID;
                //    filter.VendorLocationID = order.VendorLocationID;
                //}
                filter.SubItemID = source.SubItemID;
                filter.UOM = source.UOM;
                filter.ShipFromSiteID = source.SiteID;
                filter.SiteID = source.ToSiteID;
                filter.SOOrderType = source.SOOrderType;
                filter.SOOrderNbr = source.SOOrderNbr;
                filter.SOOrderLineNbr = source.SOOrderLineNbr;
                filter.SOShipmentNbr = source.SOShipmentNbr;
                filter.OrigRefNbr = source.RefNbr;
                filter.OrigLineNbr = source.LineNbr;
                filter.CuryUnitCost = source.UnitCost;
                sender.SetDefaultExt<POReceiptLineS.locationID>(filter);

                var rec = (PXResult<INLotSerClass, InventoryItem>)
                        PXSelectJoin<INLotSerClass,
                            InnerJoin<InventoryItem, On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>,
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .SelectWindowed(_graph, 0, 1, filter.InventoryID);
                INLotSerClass lotclass = rec;
                InventoryItem item = rec;

                Decimal? openQty = source.Qty;
                Decimal? baseOpenQty = source.BaseQty;

                decimal qty = openQty.GetValueOrDefault();

                if (lotclass != null &&
                    lotclass.LotSerTrack == INLotSerTrack.SerialNumbered && lotclass.LotSerAssign == INLotSerAssign.WhenReceived)
                {
                    qty = 1;
                    filter.UOM = item.BaseUnit;
                }
                else if (filter.ByOne == true || source == null)
                    qty = 1;

                sender.SetValueExt<POReceiptLineS.receiptQty>(filter, qty);

                if (baseOpenQty != null && filter.BaseReceiptQty > baseOpenQty)
                    sender.SetValueExt<POReceiptLineS.receiptQty>(filter, 0m);

                if (source != null)
                    sender.SetValueExt<POReceiptLineS.curyUnitCost>(filter, source.UnitCost);
            }

            public override void SetFilterToError(PXCache sender, POReceiptLineS filter)
            {
                filter.SOOrderType = null;
                filter.SOOrderNbr = null;
                filter.SOOrderLineNbr = null;
                filter.OrigRefNbr = null;
                filter.OrigLineNbr = null;
                sender.SetDefaultExt<POReceiptLineS.uOM>(filter);
                sender.SetDefaultExt<POReceiptLineS.siteID>(filter);
                sender.SetValueExt<POReceiptLineS.receiptQty>(filter, 0m);

                if (filter.VendorID != null)
                    sender.SetDefaultExt<POReceiptLineS.curyUnitCost>(filter);
                sender.RaiseExceptionHandling<POReceiptLineS.sOOrderNbr>
                    (filter, null, new PXSetPropertyException(Messages.INSourceNotFound, PXErrorLevel.Error));
            }
        }

        public PopupHandler addLinePopupHandler;

        protected virtual void POReceiptLineS_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POReceiptLineS row = e.Row as POReceiptLineS;
            POReceiptLineS old = e.OldRow as POReceiptLineS;
            if (!e.ExternalCall || row == null || old == null)
                return;

            if (row.ByOne != old.ByOne && row.ByOne == true && row.Qty != 1)
            {
                sender.SetValueExt<POReceiptLineS.receiptQty>(row, 1m);
            }

            if (row.InventoryID != null &&
                  (row.InventoryID != old.InventoryID ||
                     row.SubItemID != old.SubItemID ||
                     row.VendorLocationID != old.VendorLocationID ||
                     row.ShipFromSiteID != old.ShipFromSiteID ||
                     row.FetchMode == true))
            {

                object source = addLinePopupHandler.GetSourceItem();
                if (source != null)
                {
                    addLinePopupHandler.SetFilterToSource(sender, row, source);
            }
                else
                {
                    addLinePopupHandler.SetFilterToError(sender, row);
                }
            }
            else if (row.UOM != old.UOM && row.InventoryID == old.InventoryID && row.InventoryID != null)
            {
                if (old.UOM != null && row.UOM != null)
                {
                    Decimal? cost = row.CuryUnitCost;
                    string oldUOM = old.UOM;
                    decimal qty = decimal.Zero;
                    decimal curyUnitCost = row.CuryUnitCost ?? Decimal.Zero;

                    qty = (old.Qty == row.Qty)
                            ? INUnitAttribute.ConvertFromBase<POReceiptLineS.inventoryID>(sender, e.Row, row.UOM,
                                                                                          old.BaseReceiptQty.Value,
                                                                                          INPrecision.QUANTITY)
                            : row.Qty.Value;

                    if (row.VendorID != null && row.InventoryID != null && row.CuryUnitCost == 0m)
                        sender.SetDefaultExt<POReceiptLineS.curyUnitCost>(row);

                    if (old.CuryUnitCost == row.CuryUnitCost)
                    {
                        curyUnitCost = INUnitAttribute.ConvertFromBase<POReceiptLineS.inventoryID>(sender, e.Row, oldUOM, curyUnitCost,
                                                                                                   INPrecision.UNITCOST);
                        curyUnitCost = INUnitAttribute.ConvertToBase<POReceiptLineS.inventoryID>(sender, e.Row, row.UOM, curyUnitCost,
                                                                                                 INPrecision.UNITCOST);
                    }
                    else
                        curyUnitCost = row.CuryUnitCost.Value;

                    sender.SetValueExt<POReceiptLineS.receiptQty>(e.Row, qty);
                    sender.SetValueExt<POReceiptLineS.curyUnitCost>(e.Row, curyUnitCost);
                }
                else
                    sender.SetDefaultExt<POReceiptLineS.curyUnitCost>(row);
            }
            bool complete = false;
            if (row.AutoAddLine == true && row.Qty > 0 && row.VendorID != null && row.InventoryID != null && row.BarCode != null && row.SubItemID != null && row.LocationID != null)
            {
                complete = row.LotSerialNbr != null;
                if (!complete)
                {
                    INLotSerClass lotclass =
                        PXSelectJoin<INLotSerClass,
                            InnerJoin<InventoryItem, On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>,
                            Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                            .SelectWindowed(this, 0, 1, row.InventoryID);
                    complete = lotclass.LotSerTrack == INLotSerTrack.NotNumbered || lotclass.AutoNextNbr == true;
                }
                if (complete)
                {
                    AddReceiptLine();
                    ResetReceiptFilter(true);
                }
            }
            if (!complete)
                row.Description = null;

        }
        protected virtual void POReceiptLineS_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            POReceiptLineS row = e.Row as POReceiptLineS;
            if (row != null)
            {
                INLotSerClass lotclass =
                PXSelectJoin<INLotSerClass,
                    InnerJoin<InventoryItem, On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>,
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                .SelectWindowed(this, 0, 1, row.InventoryID);

                bool requestLotSer = lotclass != null && lotclass.LotSerTrack != INLotSerTrack.NotNumbered &&
                                     lotclass.LotSerAssign == INLotSerAssign.WhenReceived;
                bool isTransferReceipt = Document.Current.ReceiptType == POReceiptType.TransferReceipt;

                PXUIFieldAttribute.SetEnabled<POReceiptLineS.pOType>(sender, row, row.PONbr == null);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.lotSerialNbr>(sender, row, requestLotSer && lotclass.AutoNextNbr == false);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.expireDate>(sender, row, requestLotSer && lotclass.LotSerTrackExpiration == true);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.vendorID>(sender, row, row.PONbr == null && this.Document.Current.VendorID == null);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.vendorLocationID>(sender, row, row.PONbr == null && this.Document.Current.VendorLocationID == null);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.siteID>(sender, row, row.PONbr == null && !isTransferReceipt);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.uOM>(sender, row, !(requestLotSer && lotclass.LotSerTrack == INLotSerTrack.SerialNumbered) && row.ByOne != true && !isTransferReceipt);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.receiptQty>(sender, row, !(requestLotSer && lotclass.LotSerTrack == INLotSerTrack.SerialNumbered) && row.ByOne != true && !isTransferReceipt);
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.inventoryID>(sender, row, (string.IsNullOrEmpty(row.BarCode) || row.InventoryID == null));
                PXUIFieldAttribute.SetEnabled<POReceiptLineS.curyUnitCost>(sender, row, !isTransferReceipt);

                PXUIFieldAttribute.SetVisible<POReceiptLineS.vendorID>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.vendorLocationID>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.pOType>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.pONbr>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.pOLineNbr>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.curyUnitCost>(sender, row, !isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.shipFromSiteID>(sender, row, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.sOOrderType>(sender, row, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.sOOrderNbr>(sender, row, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.sOOrderLineNbr>(sender, row, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.sOShipmentNbr>(sender, row, isTransferReceipt);
                PXUIFieldAttribute.SetVisible<POReceiptLineS.curyExtCost>(sender, row, !isTransferReceipt);

                //addPOReceiptLine.SetEnabled(!isTransferReceipt || row.SOOrderLineNbr != null);
                //addPOReceiptLine2.SetEnabled(!isTransferReceipt || row.SOOrderLineNbr != null);

                PXSetPropertyException warning = null;
                if (row.BarCode != null && row.InventoryID != null && row.SubItemID != null)
                {
                    INItemXRef xref =
                    PXSelect<INItemXRef,
                        Where<INItemXRef.inventoryID, Equal<Current<POReceiptLineS.inventoryID>>,
                            And<INItemXRef.alternateID, Equal<Current<POReceiptLineS.barCode>>,
                                And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>>
                        .SelectSingleBound(this, new object[] { e.Row });
                    if (xref == null)
                        warning = new PXSetPropertyException(Messages.BarCodeAddToItem, PXErrorLevel.Warning);
                }
                sender.RaiseExceptionHandling<POReceiptLineS.barCode>(e.Row, ((POReceiptLineS)e.Row).BarCode, warning);
            }
        }
        protected virtual void INItemXRef_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        #region POReceiptDiscountDetail events

        protected virtual void POReceiptDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (Document != null && Document.Current != null)
                Document.Cache.SetValueExt<POReceipt.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<POReceiptDiscountDetail>(DiscountDetails, true));
        }

        protected virtual void POReceiptDiscountDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            POReceiptDiscountDetail discountDetail = (POReceiptDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null && discountDetail.DiscountID != null)
            {
                discountDetail.IsManual = true;

                DiscountEngine<POReceiptLine>.InsertDocGroupDiscount<POReceiptDiscountDetail>(transactions.Cache, transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate);
                RecalculateTotalDiscount();
            }
        }

        protected virtual void POReceiptDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POReceiptDiscountDetail discountDetail = (POReceiptDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null)
            {
                if (!sender.ObjectsEqual<POReceiptDiscountDetail.skipDiscount>(e.Row, e.OldRow))
                {
                    DiscountEngine<POReceiptLine>.UpdateDocumentDiscount<POReceiptDiscountDetail>(transactions.Cache, transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate, discountDetail.Type != DiscountType.Document);
                RecalculateTotalDiscount();
            }
                if (!sender.ObjectsEqual<POReceiptDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POReceiptDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
                {
                    discountDetail.IsManual = true;
                    DiscountEngine<POReceiptLine>.InsertDocGroupDiscount<POReceiptDiscountDetail>(transactions.Cache, transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<POReceiptDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate);
                    RecalculateTotalDiscount();
        }
            }
        }

        protected virtual void POReceiptDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            POReceiptDiscountDetail discountDetail = (POReceiptDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null)
            {
                DiscountEngine<POReceiptLine>.UpdateDocumentDiscount<POReceiptDiscountDetail>(transactions.Cache, transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.ReceiptDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document));
            }
            RecalculateTotalDiscount();
        }
        #endregion

		#region POReceiptTaxTran Events
		protected virtual void POReceiptTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<POReceiptTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}
		#endregion

		#region SOLineSplit Events
		[PXDBString(IsKey = true, IsUnicode = true)]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOLineSplit.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOLineSplit.orderNbr>>>>>))]
		[PXDefault()]
		protected virtual void SOLineSplit_OrderNbr_CacheAttached(PXCache sender)
		{
		}

		[PXDBDate()]
		[PXDefault()]
		protected virtual void SOLineSplit_OrderDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		[INItemPlanIDSimple()]
		protected virtual void SOLineSplit_PlanID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		protected virtual void SOLineSplit_SiteID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		protected virtual void SOLineSplit_LocationID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		public override void Persist()
        {
            if (Document.Current != null && Document.Current.Hold == false)
            {
                bool showException = false;
                foreach (POReceiptLine poLine in transactions.Select())
                {
                    if (poLine.ReceiptQty <= 0)
                    {
                        this.transactions.Cache.RaiseExceptionHandling<POReceiptLine.receiptQty>(poLine, poLine.ReceiptQty, new PXSetPropertyException(Messages.POLineQuantityMustBeGreaterThanZero, PXErrorLevel.Error));
                        showException = true;
                    }
				}

				if (showException) throw new PXException(Messages.POLineQuantityMustBeGreaterThanZero);

	            ValidateDuplicateSerialsOnDropship();

            }
            foreach (LandedCostTran iTran in this.landedCostTrans.Select())
            {
                string message;
                if (!this.ValidateLCTran(iTran, out message))
                {
                    if (this.landedCostTrans.Cache.RaiseExceptionHandling<LandedCostTran.landedCostCodeID>(iTran, iTran.LandedCostCodeID, new PXSetPropertyException(message, PXErrorLevel.RowError)))
                    {
                        throw new PXRowPersistingException(typeof(LandedCostTran.landedCostCodeID).Name, iTran.LandedCostCodeID, message);
                    }
                    throw new PXException(message);
                }
            }
            DiscountEngine<POReceiptLine>.ValidateDiscountDetails(DiscountDetails);

			if ((!this.IsImport || this._forcePriceUpdate) && (posetup.Current.VendorPriceUpdate == POVendorPriceUpdateType.Receipt))
			{
				List<POReceiptLine> linesToUpdateItemCost = new List<POReceiptLine>();
				foreach (POReceiptLine row in transactions.Cache.Cached)
				{
					if ((transactions.Cache.GetStatus(row) == PXEntryStatus.Inserted || transactions.Cache.GetStatus(row) == PXEntryStatus.Updated)
						&& row != null && row.InventoryID != null && row.CuryUnitCost > 0)
					{
						linesToUpdateItemCost.Add(row);
					}
				}

				linesToUpdateItemCost.Sort((x, y) => ((int)x.LineNbr).CompareTo((int)y.LineNbr));

				foreach (POReceiptLine row in linesToUpdateItemCost)
				{
					POItemCostManager.Update(this,
							this.Document.Current.VendorID,
							this.Document.Current.VendorLocationID,
							this.Document.Current.CuryID,
							row.InventoryID,
							row.SubItemID,
							row.UOM,
							row.CuryUnitCost.Value);
					this._forcePriceUpdate = false;
				}
			}

            base.Persist();
            this.poLinesSelection.Cache.Clear();
            this.openOrders.Cache.Clear();

            if (Document.Current != null && IsExternalTax == true && !SkipAvalaraTaxProcessing && Document.Current.IsTaxValid != true && Document.Current.ReceiptType != POReceiptType.TransferReceipt)
            {
                PXLongOperation.StartOperation(this, delegate()
                {
                    POReceipt doc = new POReceipt();
                    doc.ReceiptType = Document.Current.ReceiptType;
                    doc.ReceiptNbr = Document.Current.ReceiptNbr;
                    POExternalTaxCalc.Process(doc);
                });
            }
        }

		protected virtual void ValidateDuplicateSerialsOnDropship()
		{
			if (Document.Current != null && Document.Current.Hold != true)
			{
				HashSet<string> uniqueSerials = new HashSet<string>();

				bool duplicateFound = false;
				foreach (POReceiptLineSplit split in splits.Select())
				{
					if (split.LineType == POLineType.GoodsForDropShip && lsselect.IsTrackSerial(splits.Cache, split))
					{
						string key = string.Format("{0}.{1}", split.InventoryID, split.LotSerialNbr);

						if (uniqueSerials.Contains(key))
						{
							duplicateFound = true;

							POReceiptLine detail = (POReceiptLine) PXParentAttribute.SelectParent(splits.Cache, split, typeof (POReceiptLine));

							if (detail != null)
							{
								transactions.Cache.RaiseExceptionHandling<POReceiptLine.inventoryID>(detail, null, new PXSetPropertyException(Messages.ContainsDuplicateSerialNumbers, PXErrorLevel.RowError));
							}
						}
						else
						{
							uniqueSerials.Add(key);
						}
					}
				}

				if (duplicateFound)
				{
					throw new PXException(Messages.DuplicateSerialNumbers);
				}
			}
		}

        #region Currency Info
        protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CurrencyInfo info = e.Row as CurrencyInfo;
            if (info != null)
            {
                bool curyenabled = info.AllowUpdate(this.transactions.Cache);
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
                CurrencyInfo row = (CurrencyInfo)e.Row;
                POReceipt doc = (POReceipt)this.Document.Current;
                if (row != null && doc != null && row.CuryInfoID == doc.CuryInfoID)
                {
                    if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
                    {
                        e.NewValue = vendor.Current.CuryID;
                        e.Cancel = true;
                    }
                }
                else
                {
                    LandedCostTran lcTran = this.landedCostTrans.Current;
                    if (row != null && lcTran != null && row.CuryInfoID == lcTran.CuryInfoID)
                    {
                        e.NewValue = row.CuryID;
                        e.Cancel = true;
                    }
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
                e.NewValue = ((POReceipt)Document.Cache.Current).ReceiptDate;
                e.Cancel = true;
            }
        }


        #endregion

        #region Internal Utility functions
        public int? GetReasonCodeSubID(ReasonCode reasoncode, InventoryItem item, INSite site, INPostClass postclass)
        {
            int? reasoncode_SubID = (int?)Caches[typeof(ReasonCode)].GetValue<ReasonCode.subID>(reasoncode);
            int? item_SubID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.reasonCodeSubID>(item);
            int? site_SubID = (int?)Caches[typeof(INSite)].GetValue<INSite.reasonCodeSubID>(site);
            int? class_SubID = (int?)Caches[typeof(INPostClass)].GetValue<INPostClass.reasonCodeSubID>(postclass);

            object value = IN.ReasonCodeSubAccountMaskAttribute.MakeSub<ReasonCode.subMask>(this, reasoncode.SubMask,
                new object[] { reasoncode_SubID, item_SubID, site_SubID, class_SubID },
                new Type[] { typeof(ReasonCode.subID), typeof(InventoryItem.reasonCodeSubID), typeof(INSite.reasonCodeSubID), typeof(INPostClass.reasonCodeSubID) });

            Caches[typeof(ReasonCode)].RaiseFieldUpdating<ReasonCode.subID>(reasoncode, ref value);
            return (int?)value;
        }

        protected virtual decimal? DefaultUnitCost(PXCache sender, POReceiptLine row)
        {
            if (row != null && row.InventoryID != null && row.SubItemID != null && !string.IsNullOrEmpty(row.UOM))
            {
                POReceipt doc = this.Document.Current;
                if (doc != null && doc.VendorID != null)
                {
                    decimal? vendorUnitCost = null;
					if (row.ManualPrice != true || row.CuryUnitCost == null)
					{
                    if (row != null && row.InventoryID != null && row.UOM != null)
                    {
                        CurrencyInfo curyInfo = this.currencyinfo.Search<CurrencyInfo.curyInfoID>(doc.CuryInfoID);
                        vendorUnitCost = APVendorPriceMaint.CalculateUnitCost(sender, row.VendorID, doc.VendorLocationID, row.InventoryID, curyInfo, row.UOM, row.ReceiptQty, (DateTime)doc.ReceiptDate, row.CuryUnitCost);
                    }

                    if (vendorUnitCost == null)
                    {
							vendorUnitCost = POItemCostManager.Fetch<POReceiptLine.inventoryID, POReceiptLine.curyInfoID>(sender.Graph, row,
                            doc.VendorID, doc.VendorLocationID, doc.ReceiptDate,
                            doc.CuryID, row.InventoryID, row.SubItemID, row.SiteID, row.UOM);
                    }
					}
                    else
						vendorUnitCost = row.CuryUnitCost;

					return vendorUnitCost;
                }
            }
			return null;
            /*if(row.InventoryID!= null && row.SiteID!=null)
            {
                INItemSite item = PXSelect<INItemSite,
                            Where<INItemSite.inventoryID, Equal<Required<INItemSite.inventoryID>>,
                                And<INItemSite.siteID,Equal<Required<INItemSite.siteID>>>>>.
                            Select(this, row.InventoryID,row.SiteID);
                decimal curyUOMUnitCost =Decimal.Zero;
                if (item != null)
                {
                    decimal UOMUnitCost = INUnitAttribute.ConvertToBase<POReceiptLine.inventoryID>(sender, row, row.UOM,(decimal) item.LastCost, INPrecision.UNITCOST);
                    PXCurrencyAttribute.CuryConvCury<POReceiptLine.curyInfoID>(sender, row, UOMUnitCost, out curyUOMUnitCost);					
                }
                sender.SetValueExt<POReceiptLine.curyUnitCost>(row, curyUOMUnitCost);
            }
             */

        }

        protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            if (!sender.ObjectsEqual<POReceipt.receiptDate, POReceipt.curyID>(e.Row, e.OldRow))
            {
                foreach (POReceiptLine tran in this.transactions.Select())
                {
                    if (transactions.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
                    {
                        transactions.Cache.SetStatus(tran, PXEntryStatus.Updated);
                    }
                }
            }
        }

        public virtual void AddPurchaseOrder(POOrder aOrder)
        {
            this.filter.Cache.Remove(this.filter.Current);
            this.filter.Cache.Insert(new POOrderFilter());
            this.filter.Current.OrderType = aOrder.OrderType;
            this.filter.Current.OrderNbr = aOrder.OrderNbr;
            try
            {
                this._skipUIUpdate = true;
                foreach (POLine iLn in poLinesSelection.Select(aOrder.OrderType, aOrder.OrderNbr))
				{
						AddPOLine(iLn, false);
				}
				AddPOOrderReceipt(aOrder.OrderType, aOrder.OrderNbr);
			}
			finally
			{
				this._skipUIUpdate = false;
			}
		}

        public virtual void AddTransferDoc(SOOrderShipment aOrder)
        {
            try
            {
                this._skipUIUpdate = true;

                foreach (INTran iTran in PXSelect<INTran, Where<INTran.docType, Equal<Required<INTran.docType>>, And<INTran.refNbr, Equal<Required<INTran.refNbr>>, And<INTran.sOOrderType, Equal<Required<INTran.sOOrderType>>, And<INTran.sOOrderNbr, Equal<Required<INTran.sOOrderNbr>>, And<INTran.sOShipmentType, Equal<Required<INTran.sOShipmentType>>, And<INTran.sOShipmentNbr, Equal<Required<INTran.sOShipmentNbr>>>>>>>>>.Select(this, aOrder.InvtDocType, aOrder.InvtRefNbr, aOrder.OrderType, aOrder.OrderNbr, aOrder.ShipmentType, aOrder.ShipmentNbr))
                {
                    AddTransferLine(iTran);
                }

            }
            finally
            {
                this._skipUIUpdate = true;
            }
        }

		public virtual POReceiptLine AddPOLine(POLine aLine)
	    {
		    return AddPOLine(aLine, false);
	    }

		protected virtual POReceiptLine AddPOLine(POLine aLine, bool isLSEntryBlocked)
		{
			POReceipt doc = this.Document.Current;
			if (doc != null)
			{
                if (doc.ReceiptType == POReceiptType.TransferReceipt)
					return null;

				decimal baseQtyDelta = Decimal.Zero;
				decimal curyCostDelta = Decimal.Zero;
				decimal costDelta = Decimal.Zero;
				Dictionary<int, POReceiptLine> currentRows = new Dictionary<int, POReceiptLine>();
				foreach (POReceiptLine iLine in PXSelect<POReceiptLine,
															Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
																And<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
																And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
																And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>>>>>>.Select(this, doc.ReceiptNbr, aLine.OrderType, aLine.OrderNbr, aLine.LineNbr))
				{
					POReceiptLine original = PXSelectReadonly<POReceiptLine,
											Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
												And<POReceiptLine.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>>.Select(this, iLine.ReceiptNbr, iLine.LineNbr);

					baseQtyDelta += (iLine.BaseReceiptQty ?? Decimal.Zero) - (original != null ? ((decimal)original.BaseReceiptQty) : Decimal.Zero);
					curyCostDelta += (Decimal)iLine.CuryExtCost - (original != null ? ((decimal)original.CuryExtCost) : Decimal.Zero);
					costDelta += (Decimal)iLine.ExtCost - (original != null ? ((decimal)original.ExtCost) : Decimal.Zero);
					currentRows[iLine.LineNbr.Value] = iLine;
				}
				//Find deleted
				foreach (POReceiptLine iOrig in PXSelectReadonly<POReceiptLine,
															Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
																And<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
																And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
																And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>>>>>>.Select(this, doc.ReceiptNbr, aLine.OrderType, aLine.OrderNbr, aLine.LineNbr))
				{
					if (!currentRows.ContainsKey(iOrig.LineNbr.Value))
					{
						baseQtyDelta -= (decimal)iOrig.BaseReceiptQty;
						curyCostDelta -= (decimal)iOrig.CuryExtCost;
						costDelta -= (decimal)iOrig.ExtCost;
					}
				}
				decimal qtyDelta = baseQtyDelta;
				POReceiptLine line = new POReceiptLine();
				if (baseQtyDelta != Decimal.Zero)
				{
					if (aLine.InventoryID != null && !String.IsNullOrEmpty(aLine.UOM))
						qtyDelta = INUnitAttribute.ConvertFromBase(this.transactions.Cache, aLine.InventoryID, aLine.UOM, baseQtyDelta, INPrecision.QUANTITY);
				}
				Copy(line, aLine, qtyDelta, baseQtyDelta, curyCostDelta);
				this._forcePriceUpdate = true;
				if (line.ReceiptQty >= Decimal.Zero)
				{
					try
					{
						line.IsLSEntryBlocked = isLSEntryBlocked;//split line is not created if IsLSEntryBlocked = TRUE
						line = this.transactions.Insert(line);
					}
					finally
					{
						if (line != null)
						{
							line.IsLSEntryBlocked = false;
						}
					}
				}
				else
				{
					line.ReceiptQty = line.BaseReceiptQty = line.CuryExtCost = line.ExtCost = Decimal.Zero;
					line = this.transactions.Insert(line);
					this.transactions.Cache.RaiseExceptionHandling<POReceiptLine.receiptQty>(line, line.ReceiptQty, new PXSetPropertyException(Messages.QuantityReceivedForOrderLineExceedsOrdersQuatity, PXErrorLevel.Warning));
				}
				line.AlternateID = aLine.AlternateID;
				


				return line;
			}
			return null;
		}

        public virtual POReceiptLine AddTransferLine(INTran aLine)
		{
			POReceipt doc = this.Document.Current;
            if (doc == null || doc.ReceiptType != POReceiptType.TransferReceipt)
                return null;
			foreach (POReceiptLine r in PXSelect<POReceiptLine,
															Where<POReceiptLine.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
                                                                And<POReceiptLine.origTranType, Equal<Required<POReceiptLine.origTranType>>,
                                                                And<POReceiptLine.origRefNbr, Equal<Required<POReceiptLine.origRefNbr>>,
                                                                And<POReceiptLine.origLineNbr, Equal<Required<POReceiptLine.origLineNbr>>>>>>>
                                                                .Select(this, doc.ReceiptNbr, aLine.TranType, aLine.RefNbr, aLine.LineNbr))
			{
				return null;
			}

			POReceiptLine newtran = null;
			INTran prev_tran = null;

            foreach (PXResult<INTran, INTranSplit, INTranInTransit> res in
				PXSelectJoin<INTran,
				InnerJoin<INTranSplit,
							 On<INTranSplit.tranType, Equal<INTran.tranType>,
							And<INTranSplit.refNbr, Equal<INTran.refNbr>,
							And<INTranSplit.lineNbr, Equal<INTran.lineNbr>>>>,
                InnerJoin<INTranInTransit,
                    On<INTranInTransit.tranType, Equal<INTran.tranType>,
                             And<INTranInTransit.refNbr, Equal<INTran.refNbr>,
                             And<INTranInTransit.lineNbr, Equal<INTran.lineNbr>>>>>>,
                Where<INTran.tranType, Equal<Required<INTran.tranType>>,
                    And<INTran.refNbr, Equal<Required<INTran.refNbr>>,
                    And<INTran.lineNbr, Equal<Required<INTran.lineNbr>>,
					And<INTran.docType, Equal<INDocType.transfer>,
					And<INTran.invtMult, Equal<shortMinus1>>>>>>,
				OrderBy<Asc<INTran.tranType, Asc<INTran.refNbr, Asc<INTran.lineNbr>>>>>
                .Select(this, aLine.TranType, aLine.RefNbr, aLine.LineNbr))
			{
                INTran tran = res;
                INTranSplit split = res;
                INTranInTransit plan = res;

				if (object.Equals(prev_tran, tran) == false)
				{
					newtran = PXCache<POReceiptLine>.CreateCopy(this.transactions.Insert(new POReceiptLine()));
					Copy(newtran, aLine, 0, 0, 0);
                    newtran.OrigPlanType = plan.PlanType;

					splits.Current = null;

					newtran = transactions.Update(newtran);

					if (splits.Current != null)
					{
						splits.Delete(splits.Current);
					}
					prev_tran = tran;
				}
				if (split.Qty > 0)
				{
					POReceiptLineSplit newsplit = new POReceiptLineSplit();
					newsplit.ReceiptNbr = newtran.ReceiptNbr;
					newsplit.LineNbr = newtran.LineNbr;

					newsplit = PXCache<POReceiptLineSplit>.CreateCopy(splits.Insert(newsplit));
					newsplit.InvtMult = (short)1;
					newsplit.SiteID = tran.ToSiteID;
					newsplit.LotSerialNbr = split.LotSerialNbr;
					newsplit.ExpireDate = split.ExpireDate;
					newsplit.Qty = split.Qty;					
					newsplit = splits.Update(newsplit);
				}
				newtran = PXCache<POReceiptLine>.CreateCopy(newtran);
				string BaseCuryID = new PXSetup<Company>(this).Current.BaseCuryID;
				Decimal curyUnitCost = tran.UnitCost.Value;
				if (this.Document.Current.CuryID != BaseCuryID)
					PXCurrencyAttribute.CuryConvCury<POReceipt.curyInfoID>(this.Document.Cache, this.Document.Current, curyUnitCost, out curyUnitCost, false);
				newtran.CuryUnitCost = curyUnitCost;
				newtran = transactions.Update(newtran);
				if (newtran.BaseQty == tran.BaseQty)
				{
					//fix conversion rounding
					newtran = PXCache<POReceiptLine>.CreateCopy(newtran);
					newtran.UnitCost = tran.Qty != 0m ? tran.TranCost / newtran.Qty : 0m;
					newtran.ExtCost = tran.TranCost;
					newtran = transactions.Update(newtran);
				}
			}

            return newtran;
		}

		protected virtual void AddPOOrderReceipt(string aOrderType, string aOrderNbr)
		{
			POOrderReceipt receiptOrder = null;
			foreach (POOrderReceipt it in this.ReceiptOrders.Select())
			{
				if (it.POType == aOrderType && it.PONbr == aOrderNbr) receiptOrder = it;
			}
			if (receiptOrder == null)
			{
				POOrder order = PXSelectReadonly<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
													And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, aOrderType, aOrderNbr);
				if (order != null)
				{
					receiptOrder = new POOrderReceipt();
					receiptOrder.POType = aOrderType;
					receiptOrder.PONbr = aOrderNbr;
					receiptOrder = this.ReceiptOrders.Insert(receiptOrder);
					POReceipt doc = this.Document.Current;
					doc.POType = order.OrderType;
					if (order.OrderType == POOrderType.DropShip)
					{
						doc.ShipToBAccountID = order.ShipToBAccountID;
						doc.ShipToLocationID = order.ShipToLocationID;
						this.Document.Update(doc);
					}
				}
			}
		}
		protected virtual void CheckRctForPOQuantityRule(PXCache sender, POReceiptLine row, bool displayAsWarning)
		{
			CheckRctForPOQuantityRule(sender, row, displayAsWarning, null);
		}

		protected virtual void CheckLocationTaskRule(PXCache sender, POReceiptLine row, object newLocationID)
	    {
			if (newLocationID != null && POLineType.IsStock(row.LineType) && row.LineType != POLineType.GoodsForDropShip && row.SiteID != null && row.POType != null && row.PONbr != null && row.POLineNbr != null)
			{
				POLine poLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
								And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
								And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				
				if (poLine != null && poLine.TaskID != null)
				{
					INLocation selectedLocation = 
						(INLocation)PXSelectorAttribute.Select(sender, row, sender.GetField(typeof(POReceiptLine.locationID)), newLocationID);

					if (selectedLocation != null && (selectedLocation.ProjectID != poLine.ProjectID || (selectedLocation.TaskID != poLine.TaskID && selectedLocation.TaskID != null)))
					{
						if (posetup.Current.OrderRequestApproval == true)
						{
							sender.RaiseExceptionHandling<POReceiptLine.locationID>(row, selectedLocation.LocationCD,
								new PXSetPropertyException(Messages.LocationIsNotMappedToTaskError, PXErrorLevel.Error));
						}
						else
						{
							sender.RaiseExceptionHandling<POReceiptLine.locationID>(row, selectedLocation.LocationCD,
								new PXSetPropertyException(Messages.LocationIsNotMappedToTaskWarning, PXErrorLevel.Warning));
						}
					}
				}
			}
	    }

		protected virtual void CheckOrderTaskRule(PXCache sender, POReceiptLine row, int? newTaskID)
		{
			if (row.POType != null && row.PONbr != null && row.POLineNbr != null && (!POLineType.IsStock(row.LineType) || row.LineType == POLineType.GoodsForDropShip))
			{
				POLine poLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
								And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
								And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);

				if (poLine != null && poLine.TaskID != null && poLine.TaskID != newTaskID)
				{
					PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.TaskID);
					string taskCd = task != null ? taskCd = task.TaskCD : null;

					if (posetup.Current.OrderRequestApproval == true)
					{
						sender.RaiseExceptionHandling<POReceiptLine.taskID>(row, taskCd,
							new PXSetPropertyException(Messages.TaskDiffersError, PXErrorLevel.Error));
					}
					else
					{
						sender.RaiseExceptionHandling<POReceiptLine.taskID>(row, taskCd,
							new PXSetPropertyException(Messages.TaskDiffersWarning, PXErrorLevel.Warning));
					}
				}
			}
		}

		protected virtual bool CheckForSingleLocation(PXCache sender, POReceiptLine row)
		{
			if (POLineType.IsStock(row.LineType) && row.LineType != POLineType.GoodsForDropShip && row.TaskID != null && row.LocationID == null)
			{
				sender.RaiseExceptionHandling<POReceiptLine.locationID>(row, null, new PXSetPropertyException(IN.Messages.RequireSingleLocation));
				return false;
			}

			return true;
		}
		
		protected virtual bool CheckSplitsForSameTask(PXCache sender, POReceiptLine row)
		{
			if (POLineType.IsStock(row.LineType))
			{
				if (row.HasMixedProjectTasks == true)
				{
					sender.RaiseExceptionHandling<POReceiptLine.locationID>(row, null, new PXSetPropertyException(IN.Messages.MixedProjectsInSplits));
					return false;
				}

			}

			return true;
		}
	
		protected virtual void CheckRctForPOQuantityRule(PXCache sender, POReceiptLine row, bool displayAsWarning, POLine aOriginLine)
		{
			POLine poLine = aOriginLine;
			if (poLine == null || (poLine.OrderType != row.POType || poLine.OrderNbr != row.PONbr || poLine.LineNbr != row.POLineNbr))
			{
				poLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
							And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
							And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
			}
			if (poLine != null &&
				 (poLine.RcptQtyAction == POReceiptQtyAction.Reject ||
					poLine.RcptQtyAction == POReceiptQtyAction.AcceptButWarn))
			{
				if (poLine.OrderQty != Decimal.Zero)
				{
					decimal qty = (decimal)row.ReceiptQty;
					if (row.InventoryID != null)
					{
						if (!string.IsNullOrEmpty(poLine.UOM) && !string.IsNullOrEmpty(row.UOM) && poLine.UOM != row.UOM)
						{
							qty = INUnitAttribute.ConvertFromTo<POReceiptLine.inventoryID>(sender, row, row.UOM, poLine.UOM, qty, INPrecision.QUANTITY);
						}
						foreach (POReceiptLine iLine in PXSelect<POReceiptLine,
												Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
													And<POReceiptLine.lineNbr, NotEqual<Required<POReceiptLine.lineNbr>>,
														And<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
														And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
														And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>>>>>>>.Select(this, row.ReceiptNbr, row.LineNbr, row.POType, row.PONbr, row.POLineNbr))
						{
							if (!string.IsNullOrEmpty(poLine.UOM) && !string.IsNullOrEmpty(iLine.UOM) && poLine.UOM != iLine.UOM && iLine.Qty.HasValue)
							{
								qty += INUnitAttribute.ConvertFromTo<POReceiptLine.inventoryID>(sender, iLine, iLine.UOM, poLine.UOM, iLine.Qty.Value, INPrecision.QUANTITY);
							}
						}
					}
                    decimal minQty = PXDBQuantityAttribute.Round((poLine.RcptQtyMin.Value / 100.0m) * poLine.OpenQty.Value);
					decimal ratio = (qty / poLine.OrderQty.Value) * 100.0m;
					decimal maxRatio = ratio;
					POLineR poLineUpdated = PXSelect<POLineR, Where<POLineR.orderType, Equal<Required<POLineR.orderType>>,
										And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
								And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
					if (poLineUpdated != null)
						maxRatio = (poLineUpdated.ReceivedQty.Value / poLine.OrderQty.Value) * 100.0m;

					if (row.ReceiptType == POReceiptType.POReceipt)
					{
						PXErrorLevel errorLevel = ((!displayAsWarning && (poLine.RcptQtyAction == POReceiptQtyAction.Reject))
						                           	? PXErrorLevel.Error
						                           	: PXErrorLevel.Warning);
                        if (qty < minQty)
					{
							sender.RaiseExceptionHandling<POReceiptLine.receiptQty>(row, row.ReceiptQty,
							                                                        new PXSetPropertyException(
							                                                        	Messages.ReceiptLineQtyDoesNotMatchMinPOQuantityRules,
							                                                        	errorLevel));
					}
					if (maxRatio > poLine.RcptQtyMax)
					{
							sender.RaiseExceptionHandling<POReceiptLine.receiptQty>(row, row.ReceiptQty,
							                                                        new PXSetPropertyException(
							                                                        	Messages.ReceiptLineQtyDoesNotMatchMaxPOQuantityRules,
							                                                        	errorLevel));
						}
					}
					else if (poLineUpdated != null && poLineUpdated.ReceivedQty < 0)
					{
						sender.RaiseExceptionHandling<POReceiptLine.receiptQty>(row, row.ReceiptQty,
																													new PXSetPropertyException(
																														Messages.ReceiptLineQtyGoNegative,
																														PXErrorLevel.Error));
					}

				}
			}
		}
		protected virtual void UpdatePOLineCompleteFlag(POReceiptLine row, bool isDeleted)
		{
			UpdatePOLineCompleteFlag(row, isDeleted, null);
		}

		protected virtual void UpdatePOLineCompleteFlag(POReceiptLine row, bool isDeleted, POLine aOriginLine)
		{
			if (row.PONbr != null && row.POType != null && row.POLineNbr != null)
			{
				POLineR poLineCurrent = PXSelect<POLineR, Where<POLineR.orderType, Equal<Required<POLineR.orderType>>,
											And<POLineR.orderNbr, Equal<Required<POLineR.orderNbr>>,
									And<POLineR.lineNbr, Equal<Required<POLineR.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				POLine poLineOrigin = aOriginLine;
				if (poLineOrigin == null || (poLineOrigin.OrderType != row.POType || poLineOrigin.OrderNbr != row.PONbr || poLineOrigin.LineNbr != row.POLineNbr))
				{
					poLineOrigin = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
											And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
									And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POType, row.PONbr, row.POLineNbr);
				}
				if (poLineCurrent != null && poLineOrigin != null)
				{
                    if (row.ReceiptType == POReceiptType.POReceipt)
                    {
                        bool CompleteByAmount = false;
                        InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);

                        if (item != null && item.CompletePOLine == CompletePOLineTypes.Amount &&
                            item.NonStockReceipt == true)
                        {
                            CompleteByAmount = true;
                        }

                        bool needToClosePO;

                        if (CompleteByAmount)
                        {
                            decimal treshold = (poLineOrigin.LineAmt.Value * poLineOrigin.RcptQtyThreshold.Value) / 100.0m;
                            needToClosePO = (poLineCurrent.ReceivedCost >= treshold);
                        }
                        else
                        {
                            decimal treshold = (poLineOrigin.BaseOrderQty.Value * poLineOrigin.RcptQtyThreshold.Value) / 100.0m;
                            needToClosePO = (poLineCurrent.BaseReceivedQty.Value >= treshold);
                        }
                        
                        if (needToClosePO != poLineCurrent.AllowComplete)
                        {
                            poLineCurrent.AllowComplete = needToClosePO;
                            this.Caches[typeof(POLineR)].Update(poLineCurrent);
                            this.transactions.View.RequestRefresh();
                        }
                        row.AllowComplete = poLineCurrent.AllowComplete;
                    }
                    else
                    {
                        bool needToOpen = poLineCurrent.Completed == true;
                        if (needToOpen != poLineCurrent.AllowComplete)
                        {
                            poLineCurrent.AllowComplete = needToOpen;
                            this.Caches[typeof(POLineR)].Update(poLineCurrent);
                            this.transactions.View.RequestRefresh();
                        }
                        row.AllowOpen = poLineCurrent.AllowComplete;
                    }
				}
			}
		}

		protected virtual void DeleteUnusedReference(POReceiptLine aLine, bool skipReceiptUpdate)
		{
			if (string.IsNullOrEmpty(aLine.POType) || string.IsNullOrEmpty(aLine.PONbr)) return;

			string aOrderType = aLine.POType;
			string aOrderNbr = aLine.PONbr;
			POReceipt doc = this.Document.Current;
			POOrderReceipt receiptOrder = null;
			foreach (POOrderReceipt it in this.ReceiptOrders.Select())
			{
				if (it.POType == aOrderType && it.PONbr == aOrderNbr) receiptOrder = it;
			}

			foreach (POReceiptLine iLine in PXSelect<POReceiptLine,
										Where<POReceiptLine.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
											And<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
												And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>>>>>.Select(this, aLine.ReceiptNbr, aOrderType, aOrderNbr))
			{
				if (iLine.LineNbr != aLine.LineNbr)
					return;  //Other reference to the same receipt exists				
			}
			if (receiptOrder != null)
				this.ReceiptOrders.Delete(receiptOrder);

			if (!skipReceiptUpdate)
			{
				POOrder order = PXSelectReadonly<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
														And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, aOrderType, aOrderNbr);
				if (doc.POType == aLine.POType)
				{
					POReceiptLine typeRef = null;
					foreach (PXResult<POReceiptLine, POOrder> it in PXSelectJoin<POReceiptLine, InnerJoin<POOrder, On<POOrder.orderType, Equal<POReceiptLine.pOType>,
																				And<POOrder.orderNbr, Equal<POReceiptLine.pONbr>>>>,
																	Where<POReceiptLine.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
																	And<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>>>>.Select(this, aLine.ReceiptNbr, aOrderType))
					{
						POReceiptLine iLine = (POReceiptLine)it;
						POOrder iOrder = (POOrder)it;
						if (iLine.LineNbr == aLine.LineNbr) continue;
						typeRef = iLine;
						if (!doc.ShipToBAccountID.HasValue || !doc.ShipToLocationID.HasValue
							|| (doc.ShipToBAccountID == iOrder.ShipToBAccountID && doc.ShipToLocationID == iOrder.ShipToLocationID))
							return;	//Another reference to the same receipt type/destination exists 						
					}

					doc.ShipToBAccountID = null;
					doc.ShipToLocationID = null;
					if (typeRef == null)
						doc.POType = null;
					this.Document.Update(doc);
				}
			}
		}

		public virtual void Copy(POReceiptLine aDest, POLine aSrc, decimal aQtyAdj, decimal aBaseQtyAdj, decimal aCuryExtCostAdj)
		{
			aDest.BranchID = aSrc.BranchID;
			aDest.POType = aSrc.OrderType;
			aDest.PONbr = aSrc.OrderNbr;
			aDest.POLineNbr = aSrc.LineNbr;
			aDest.InventoryID = aSrc.InventoryID;
			aDest.SubItemID = aSrc.SubItemID;
			aDest.SiteID = aSrc.SiteID;
			aDest.LineType = aSrc.LineType;
			if (this.Document.Current.ReceiptType == POReceiptType.POReturn)
			{
				aDest.LineType =
						POLineType.IsStock(aSrc.LineType) ? POLineType.GoodsForInventory
					: POLineType.IsNonStock(aSrc.LineType) ? POLineType.NonStock
					: POLineType.Freight;
			}

			aDest.TaxCategoryID = aSrc.TaxCategoryID;
			aDest.TranDesc = aSrc.TranDesc;
			aDest.UnitCost = aSrc.UnitCost;
            //volume and weight must be defaulted
			aDest.UnitVolume = null;
			aDest.UnitWeight = null;
			aDest.UOM = aSrc.UOM;
			aDest.VendorID = aSrc.VendorID;
			aDest.AlternateID = aSrc.AlternateID;			
			aDest.CuryUnitCost = aSrc.CuryUnitCost;
            if (posetup.Current.DefaultReceiptQty == DefaultReceiptQuantity.Zero)
            {
                aDest.BaseQty = Decimal.Zero;
                aDest.Qty = Decimal.Zero;
            }
            else
            {
                aDest.BaseQty =
                    this.Document.Current.ReceiptType == POReceiptType.POReturn
                    ? aSrc.BaseReceivedQty - aBaseQtyAdj
                    : aSrc.LeftToReceiveBaseQty - aBaseQtyAdj;
                aDest.Qty =
                    this.Document.Current.ReceiptType == POReceiptType.POReturn
                    ? aSrc.ReceivedQty - aQtyAdj
                    : aSrc.LeftToReceiveQty - aQtyAdj;
            }
						
			if (aSrc.OrderedQtyAltered != true && aDest.BaseQty == aSrc.BaseOrderQty)
			{
				aDest.CuryLineAmt = aSrc.CuryLineAmt;
			}

			aDest.ExpenseAcctID = aSrc.ExpenseAcctID;
			aDest.ExpenseSubID = aSrc.ExpenseSubID;
			aDest.AllowComplete = true;
			aDest.AllowOpen = true;

            aDest.ManualPrice = aSrc.ManualPrice ?? false;
            aDest.ManualDisc = true;
            aDest.DiscPct = aSrc.DiscPct;
            aDest.CuryDiscAmt = aSrc.CuryDiscAmt * aDest.Qty / aSrc.OrderQty;
            aDest.DiscountID = aSrc.DiscountID;
            aDest.DiscountSequenceID = aSrc.DiscountSequenceID;
            aDest.FreezeManualDisc = true;

			bool isStockItem = (aDest.LineType == POLineType.GoodsForInventory ||
					aDest.LineType == POLineType.GoodsForSalesOrder ||
					aDest.LineType == POLineType.GoodsForReplenishment);

			aDest.ProjectID = aSrc.ProjectID;
			aDest.TaskID = aSrc.TaskID;

			if (isStockItem && aSrc.TaskID != null)
			{
				//try no derive Location from Task.
				PXResultset<INLocation> locations = PXSelect<INLocation, Where<INLocation.siteID, Equal<Required<INLocation.siteID>>,
					And<INLocation.projectID, Equal<Required<INLocation.projectID>>,
						And<INLocation.taskID, Equal<Required<INLocation.taskID>>,
						And<INLocation.active, Equal<True>>>>>>.Select(this, aSrc.SiteID, aSrc.ProjectID, aSrc.TaskID);

				if (locations.Count == 0)
				{
					INLocation wildcardLocation = PXSelect<INLocation, Where<INLocation.siteID, Equal<Required<INLocation.siteID>>,
					And<INLocation.projectID, Equal<Required<INLocation.projectID>>,
					And<INLocation.taskID, IsNull, And<INLocation.active, Equal<True>>>>>>.Select(this, aSrc.SiteID, aSrc.ProjectID);

					if (wildcardLocation != null)
					{
						aDest.LocationID = wildcardLocation.LocationID;
					}
					else
					{
						aDest.LocationID = null;
						aDest.ProjectID = null;
						aDest.TaskID = null;
					}
				}
				else if (locations.Count == 1)
				{
					aDest.LocationID = ((INLocation)locations[0]).LocationID;
				}
				else
				{
					aDest.LocationID = null;
					aDest.ProjectID = null;
					aDest.TaskID = null;
				}
			}

			if (aSrc.LineType == POLineType.Freight || aSrc.LineType == POLineType.NonStock
				|| aSrc.LineType == POLineType.Service || aSrc.LineType == POLineType.MiscCharges)
			{				
				if (this.Document.Current.ReceiptType == POReceiptType.POReceipt)
				{
			    	aDest.CuryExtCost = (aSrc.CuryExtCost - aSrc.CuryReceivedCost) - aCuryExtCostAdj;
	            }
				else
				{
					aDest.CuryExtCost = aSrc.CuryReceivedCost - aCuryExtCostAdj;				
				}
			}
		}

        public virtual void Copy(POReceiptLine aDest, INTran aSrc, decimal aQtyAdj, decimal aBaseQtyAdj, decimal aCuryExtCostAdj)
        {
            aDest.POType = null;
            aDest.PONbr = null;
            aDest.POLineNbr = null;
            aDest.InventoryID = aSrc.InventoryID;
            aDest.SubItemID = aSrc.SubItemID;
            aDest.SiteID = aSrc.ToSiteID;
            aDest.LocationID = aSrc.ToLocationID;
            aDest.LineType = POLineType.GoodsForInventory;
            aDest.UOM = aSrc.UOM;
            aDest.Qty = 0;
            aDest.BaseQty = 0;
            aDest.OrigBranchID = aSrc.BranchID;
            aDest.OrigTranType = aSrc.TranType;
            aDest.OrigRefNbr = aSrc.RefNbr;
            aDest.OrigLineNbr = aSrc.LineNbr;
            aDest.SOOrderType = aSrc.SOOrderType;
            aDest.SOOrderNbr = aSrc.SOOrderNbr;
            aDest.SOOrderLineNbr = aSrc.SOOrderLineNbr;
			aDest.SOShipmentType = aSrc.SOShipmentType;
			aDest.SOShipmentNbr = aSrc.SOShipmentNbr;
            aDest.ExpenseAcctID = null;
            aDest.ExpenseSubID = null;
            aDest.ReasonCode = null;
            aDest.TaxCategoryID = null;
            aDest.TranDesc = aSrc.TranDesc;
            aDest.UnitCost = 0m;
            //aDest.UnitVolume = aSrc.UnitVolume;
            //aDest.UnitWeight = aSrc.UnitWeight;
            //aDest.VendorID = aSrc.VendorID;
            //aDest.AlternateID = aSrc.AlternateID;
            //aDest.CuryUnitCost = aSrc.CuryUnitCost;
            aDest.AllowComplete = true;
            aDest.AllowOpen = true;

            if (aDest.LineType != POLineType.GoodsForInventory)//for stock items the project and task is taken from the Location via PODefaultProjectAttribute.
            {
                aDest.ProjectID = aSrc.ProjectID;
                aDest.TaskID = aSrc.TaskID;
            }
        }

		protected static void ClearUnused(POReceiptLine aLine, string aLineType)
		{
            if (aLine.LineType == POLineType.Freight ||
                aLine.LineType == POLineType.MiscCharges ||
                aLine.LineType == POLineType.NonStock ||
                aLine.LineType == POLineType.NonStockForDropShip ||
                aLine.LineType == POLineType.NonStockForSalesOrder)
            {
				aLine.SubItemID = null;
				if (aLine.LineType != POLineType.NonStock && 
					aLine.LineType != POLineType.NonStockForDropShip &&
					aLine.LineType != POLineType.NonStockForSalesOrder)
				{
					aLine.InventoryID = null;
					aLine.UOM = null;
					aLine.CuryUnitCost = decimal.Zero;
					aLine.UnitCost = decimal.Zero;
				}
				aLine.VoucheredQty = Decimal.Zero;
				aLine.BaseVoucheredQty = Decimal.Zero;
				aLine.UnbilledQty = aLine.ReceiptQty;
				aLine.BaseUnbilledQty = aLine.BaseReceiptQty;
				aLine.LocationID = null;
			}
		}

		protected virtual bool CanAddOrder(POOrder aOrder, out string message)
		{
			POReceipt doc = this.Document.Current;
			if (!String.IsNullOrEmpty(doc.POType))
			{
				if (doc.CuryID != aOrder.CuryID)
				{
					message = String.Format(PXMessages.LocalizeNoPrefix(Messages.PurchaseOrderHasCurrencyDifferentFromPOReceipt),PXMessages.LocalizeNoPrefix(aOrder.OrderType),aOrder.OrderNbr);
					return false;
				}

				if (doc.POType != aOrder.OrderType)
				{
					message = String.Format(PXMessages.LocalizeNoPrefix(Messages.PurchaseOrderHasTypeDifferentFromOthersInPOReceipt), PXMessages.LocalizeNoPrefix(aOrder.OrderType),aOrder.OrderNbr);
					return false;
				}
				if ((doc.ShipToBAccountID.HasValue || doc.ShipToLocationID.HasValue))
				{
					if (doc.ShipToBAccountID != aOrder.ShipToBAccountID || doc.ShipToLocationID != aOrder.ShipToLocationID)
					{
						message = String.Format(PXMessages.LocalizeNoPrefix(Messages.PurchaseOrderHasShipDestinationDifferentFromOthersInPOReceipt), PXMessages.LocalizeNoPrefix(aOrder.OrderType),aOrder.OrderNbr);
						return false;
					}
				}
			}
			message = string.Empty;
			return true;
		}

		protected static bool IsPassingFilter(POOrderFilter aFilter, POOrder aOrder)
		{
			if (!string.IsNullOrEmpty(aFilter.OrderType) && aOrder.OrderType != aFilter.OrderType) return false;
			if (aFilter.ShipToBAccountID != null && aOrder.ShipToBAccountID != aFilter.ShipToBAccountID) return false;
			if (aFilter.ShipToLocationID != null && aOrder.ShipToLocationID != aFilter.ShipToLocationID) return false;
			return true;
		}

		protected static bool IsPassingFilter(POOrderFilter aFilter, POLine aLine)
		{
			if (aLine.OrderType != aFilter.OrderType) return false;
			if (aLine.OrderNbr != aFilter.OrderNbr) return false;
			return true;
		}

		protected virtual bool ValidateLCTran(LandedCostTran row, out string message)
		{
			decimal valueToCompare = decimal.Zero;
			int count = 0;
			if (row != null && !String.IsNullOrEmpty(row.LandedCostCodeID))
			{
				LandedCostCode code = PXSelect<LandedCostCode,
						Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, row.LandedCostCodeID);

                var lch = new LandedCostHelper(this, false);
				foreach (POReceiptLine it in this.transactions.Select())
				{
                    if (lch.IsApplicable(row, code, it))
					{
						valueToCompare += LandedCostHelper.GetBaseValue(code, it);
					}
					count++;
				}

                if(!lch.HasApplicableTransfers)
                {
                    message = Messages.LandedCostCannotBeDistributed;
                    return false;
                }

				switch (code.AllocationMethod)
				{
					case LandedCostAllocationMethod.ByCost:
						message = Messages.LandedCostReceiptTotalCostIsZero;
						break;
					case LandedCostAllocationMethod.ByWeight:
						message = Messages.LandedCostReceiptTotalWeightIsZero;
						break;
					case LandedCostAllocationMethod.ByVolume:
						message = Messages.LandedCostReceiptTotalVolumeIsZero;
						break;
					case LandedCostAllocationMethod.ByQuantity:
						message = Messages.LandedCostReceiptTotalQuantityIsZero;
						break;
					case LandedCostAllocationMethod.None:
						message = Messages.LandedCostReceiptNoReceiptLines;
						valueToCompare = count;
						break;
					default:
						message = Messages.LandedCostUnknownAllocationType;
						break;
				}
				if (valueToCompare == Decimal.Zero)
				{
					return false;
				}
			}
			message = String.Empty;
			return true;
		}
        #endregion

        #region Release Functions
        public virtual void ReleaseReceipt(INReceiptEntry docgraph, AP.APInvoiceEntry invoiceGraph, POReceipt aDoc, DocumentList<INRegister> aINCreated, DocumentList<AP.APInvoice> aAPCreated, bool aIsMassProcess)
		{
			this.Clear();
			docgraph.Clear();

            docgraph.FieldVerifying.AddHandler<INTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });

            docgraph.insetup.Current.RequireControlTotal = false;
			docgraph.insetup.Current.HoldEntry = false;

			invoiceGraph.Clear();

			invoiceGraph.APSetup.Current.RequireControlTotal = false;
			invoiceGraph.APSetup.Current.RequireControlTaxTotal = false;
			invoiceGraph.APSetup.Current.HoldEntry = false;

			this.Document.Current = Document.Search<POReceipt.receiptNbr>(aDoc.ReceiptNbr, aDoc.ReceiptType);

			INRegister newdoc = new INRegister();
			if (newdoc.RefNbr != null)
			{
				docgraph.receipt.Current = docgraph.receipt.Search<INRegister.refNbr>(newdoc.RefNbr);
			}
			else
			{
				newdoc.BranchID = aDoc.BranchID;
				newdoc.DocType = POReceiptType.GetINDocType(aDoc.ReceiptType);
				newdoc.SiteID = null;
				newdoc.TranDate = aDoc.ReceiptDate;
				newdoc.FinPeriodID = aDoc.FinPeriodID;
				newdoc.OrigModule = GL.BatchModule.PO;

				docgraph.receipt.Insert(newdoc);
			}

			POReceiptLine prev_line = null;
			INTran newline = null;
			Dictionary<string, string> orderCheckClosed = new Dictionary<string, string>();
			HashSet<PXResult<INItemPlan, INPlanType>> podemand = new HashSet<PXResult<INItemPlan, INPlanType>>();
			List<INItemPlan> posupply = new List<INItemPlan>();
			bool hasStockItems = false;
			PXSelectBase<POReceiptTax> inclusiveLineTaxSelect = new PXSelectJoin<POReceiptTax,
									InnerJoin<Tax, On<POReceiptTax.taxID, Equal<Tax.taxID>,
										And<Tax.taxCalcLevel, Equal<CSTaxCalcLevel.inclusive>>>>,
									Where<POReceiptTax.receiptNbr, Equal<Required<POReceiptTax.receiptNbr>>,
										And<POReceiptTax.lineNbr, Equal<Required<POReceiptTax.lineNbr>>>>>(this);
			POReceiptTax receiptLineTax = null;

			foreach (PXResult<POReceiptLine, POReceiptLineSplit, INTran, INItemPlan, INPlanType, INSite, InventoryItem, POOrder, POAddress> res in PXSelectJoin<POReceiptLine,
				LeftJoin<POReceiptLineSplit, On<POReceiptLineSplit.receiptNbr, Equal<POReceiptLine.receiptNbr>, And<POReceiptLineSplit.lineNbr, Equal<POReceiptLine.lineNbr>>>,
				LeftJoin<INTran, On<INTran.pOReceiptNbr, Equal<POReceiptLine.receiptNbr>, And<INTran.pOReceiptLineNbr, Equal<POReceiptLine.lineNbr>>>,
				LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<POReceiptLineSplit.planID>>,
				LeftJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>,
				LeftJoin<INSite, On<INSite.siteID, Equal<POReceiptLine.siteID>>,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POReceiptLine.inventoryID>>,
				LeftJoin<POOrder, On<POOrder.orderType, Equal<POReceiptLine.pOType>, And<POOrder.orderNbr, Equal<POReceiptLine.pONbr>>>,
				LeftJoin<POAddress, On<POOrder.shipAddressID, Equal<POAddress.addressID>>>>>>>>>>,
				Where<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>,
					And<INTran.refNbr, IsNull>>,
					OrderBy<Asc<POReceiptLine.receiptNbr, Asc<POReceiptLine.lineNbr>>>>.Select(this))
			{
				POReceiptLine line = (POReceiptLine)res;
				POReceiptLineSplit split = (POReceiptLineSplit)res;
				INItemPlan plan = PXCache<INItemPlan>.CreateCopy((INItemPlan)res);
				INPlanType plantype = (INPlanType)res;
				INSite insite = res;
				POLineUOpen poLine = null;
				InventoryItem inItem = (InventoryItem)res;
				POOrder poOrder = (POOrder)res;
				POAddress poAddress = (POAddress)res;

				if (!string.IsNullOrEmpty(line.PONbr))
				{
					//Need special select for correct updating data merging in the cache
					poLine = PXSelect<POLineUOpen, Where<POLineUOpen.orderType, Equal<Required<POLineUOpen.orderType>>,
						And<POLineUOpen.orderNbr, Equal<Required<POLineUOpen.orderNbr>>,
							And<POLineUOpen.lineNbr, Equal<Required<POLineUOpen.lineNbr>>>>>>.Select(this, line.POType, line.PONbr,
																									 line.POLineNbr);
				}

				//Splits actually exist only for the Stock Items
				if (!string.IsNullOrEmpty(split.ReceiptNbr))
				{
					if (!string.IsNullOrEmpty(plantype.PlanType) && (bool)plantype.DeleteOnEvent)
					{
						Caches[typeof(INItemPlan)].Delete(plan);
						Caches[typeof(POReceiptLineSplit)].SetStatus(split, PXEntryStatus.Updated);
						split = (POReceiptLineSplit)Caches[typeof(POReceiptLineSplit)].Locate(split);
						if (split != null) split.PlanID = null;
						Caches[typeof(POReceiptLineSplit)].IsDirty = true;
					}
					else if (!string.IsNullOrEmpty(plantype.PlanType) && string.IsNullOrEmpty(plantype.ReplanOnEvent) == false)
					{
						plan.PlanType = plantype.ReplanOnEvent;
						Caches[typeof(INItemPlan)].Update(plan);
						Caches[typeof(POReceiptLineSplit)].SetStatus(split, PXEntryStatus.Updated);
						Caches[typeof(POReceiptLineSplit)].IsDirty = true;
					}
				}
				if (Caches[typeof(POReceiptLine)].ObjectsEqual(prev_line, line) == false || (newline != null && newline.InventoryID != split.InventoryID))
				{
					if (line.IsStockItem() || (line.IsNonStockItem() && inItem != null && inItem.NonStockReceipt == true && insetup.Current.UpdateGL == true && line.POAccrualAcctID != null))
					{
						hasStockItems = true;
						newline = new INTran();
						newline.BranchID = line.OrigTranType == INTranType.Transfer ? insite.BranchID : line.BranchID;
						newline.TranType = line.OrigTranType == INTranType.Transfer ? line.OrigTranType : POReceiptType.GetINTranType(aDoc.ReceiptType);
						newline.POReceiptType = line.ReceiptType;
						newline.POReceiptNbr = line.ReceiptNbr;
						newline.POReceiptLineNbr = line.LineNbr;
						newline.POLineType = line.LineType;
						newline.OrigBranchID = line.OrigBranchID;
						newline.OrigTranType = line.OrigTranType;
						newline.OrigRefNbr = line.OrigRefNbr;
						newline.OrigLineNbr = line.OrigLineNbr;

						if (line.OrigTranType == INTranType.Transfer)
						{
							newline.AcctID = null;
							newline.SubID = null;
							newline.InvtAcctID = null;
							newline.InvtSubID = null;
							newline.OrigPlanType = line.OrigPlanType;
						}
						else
						{
							newline.AcctID = line.POAccrualAcctID;
							newline.SubID = line.POAccrualSubID;
							newline.ReclassificationProhibited = true;

							newline.InvtAcctID = line.ExpenseAcctID;
							newline.InvtSubID = line.ExpenseSubID;

							newline.OrigPlanType = null;
						}

						newline.InventoryID = line.IsStockItem() ? split.InventoryID : line.InventoryID;
						newline.SiteID = line.SiteID;

						if ((object.Equals(line.InventoryID, split.InventoryID) == false) && !line.IsNonStockItem())
						{
							newline.SubItemID = split.SubItemID;
							newline.LocationID = split.LocationID;
							newline.UOM = split.UOM;
							newline.UnitPrice = 0m;
							newline.UnitCost = 0m;
							newline.TranDesc = null;
						}
						else
						{
							newline.SubItemID = line.SubItemID;
							newline.LocationID = line.LocationID;
							newline.UOM = line.UOM;
							newline.UnitCost = line.UnitCost;
							newline.TranDesc = line.TranDesc;
							newline.ReasonCode = line.ReasonCode;
							newline.UnitCost = line.UnitCost ?? Decimal.Zero;
						}

						newline.InvtMult = line.InvtMult;
						newline.Qty = line.IsStockItem() ? Decimal.Zero : line.Qty;
						newline.ExpireDate = line.ExpireDate;
						newline.ProjectID = line.ProjectID;
						newline.TaskID = line.TaskID;
						newline = docgraph.lsselect.Insert(newline);
						receiptLineTax = inclusiveLineTaxSelect.Select(line.ReceiptNbr, line.LineNbr);
						//All the ReceiptTax for inclusive tax have the same taxable amount 
					}
					else
					{
						newline = null;
						receiptLineTax = null;
					}

					if (posetup.Current.VendorPriceUpdate == POVendorPriceUpdateType.ReleaseReceipt &&
						line.InventoryID != null && line.CuryUnitCost != null)
						POItemCostManager.Update(this,
																		 this.Document.Current.VendorID,
																		 this.Document.Current.VendorLocationID,
																		 this.Document.Current.CuryID,
																		 line.InventoryID,
																		 line.SubItemID,
																		 line.UOM,
																		 line.CuryUnitCost.Value);


					if (poLine != null && !(string.IsNullOrEmpty(poLine.OrderType) || String.IsNullOrEmpty(poLine.OrderNbr) || poLine.LineNbr == null))
					{
						decimal delta = line.ReceiptQty ?? Decimal.Zero;
						PXCache poLineCache = this.Caches[typeof(POLineUOpen)];
						poLine = PXCache<POLineUOpen>.CreateCopy(poLine);
						if (line.InventoryID != null && !String.IsNullOrEmpty(line.UOM) && !String.IsNullOrEmpty(poLine.UOM))
							delta = INUnitAttribute.ConvertFromBase(Caches[typeof(POReceiptLine)], line.InventoryID, poLine.UOM, line.BaseReceiptQty.Value, INPrecision.QUANTITY);

						poLine.OpenQty -= delta * line.InvtMult;
						if (poLine.OpenQty < 0)
							poLine.OpenQty = 0;

						poLine.CuryOpenAmt -= line.CuryLineAmt.GetValueOrDefault() * line.InvtMult;
						if (poLine.CuryOpenAmt < 0)
							poLine.CuryOpenAmt = 0;

						if (poLine.AllowComplete == true && poLine.Completed != true)
						{
							POReceipt unreleasedRcpt = PXSelectReadonly2<POReceipt, InnerJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<POReceipt.receiptNbr>>>,
									Where<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
										And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
											And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>,
												And<POReceipt.released, Equal<False>,
										And<POReceipt.receiptNbr, NotEqual<Required<POReceipt.receiptNbr>>>>>>>>.Select(this, line.POType, line.PONbr, line.POLineNbr, line.ReceiptNbr);
							bool isLastUnreleased = (unreleasedRcpt == null);
							if (isLastUnreleased)
							{
								poLine.Completed = true;
								poLine.OpenQty = 0;
								poLine.CuryOpenAmt = 0;
								orderCheckClosed[poLine.OrderNbr] = poLine.OrderNbr;
							}
						}

						if (poLine.LineType == POLineType.GoodsForSalesOrder ||
							poLine.LineType == POLineType.NonStockForSalesOrder)
						{
							decimal? SupplyQty = line.BaseReceiptQty;

							foreach (PXResult<INItemPlan, INPlanType> demandres in PXSelectJoin<INItemPlan, InnerJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>>, Where<INItemPlan.supplyPlanID, Equal<Required<INItemPlan.supplyPlanID>>, And<INPlanType.isDemand, Equal<boolTrue>, And<INPlanType.isFixed, Equal<boolTrue>>>>>.Select(this, poLine.PlanID))
							{
								INItemPlan demand = demandres;

								if (SupplyQty >= demand.PlanQty)
								{
									INItemPlan supply = new INItemPlan();
									supply.InventoryID = line.InventoryID;
									supply.SiteID = line.SiteID;
									supply.SubItemID = line.SubItemID;
									supply.PlanQty = demand.PlanQty;
									supply.DemandPlanID = demand.PlanID;
									supply.RefNoteID = demand.RefNoteID;
									supply.PlanDate = demand.PlanDate;
									supply.PlanType = INPlanConstants.Plan64;
									supply.FixedSource = line.SiteID != demand.SiteID ? INReplenishmentSource.Transfer : INReplenishmentSource.None;
									supply.SourceSiteID = line.SiteID;
									supply.Hold = false;

									posupply.Add(supply);
									SupplyQty -= demand.PlanQty;
									podemand.Add(demandres);
								}
								else if (SupplyQty > 0m)
								{
									INItemPlan supply = new INItemPlan();
									supply.InventoryID = line.InventoryID;
									supply.SiteID = line.SiteID;
									supply.SubItemID = line.SubItemID;
									supply.PlanQty = SupplyQty;
									supply.DemandPlanID = demand.PlanID;
									supply.RefNoteID = demand.RefNoteID;
									supply.PlanDate = demand.PlanDate;
									supply.PlanType = INPlanConstants.Plan64;
									supply.Hold = false;
									supply.FixedSource = line.SiteID != demand.SiteID ? INReplenishmentSource.Transfer : INReplenishmentSource.None;
									supply.SourceSiteID = line.SiteID;

									posupply.Add(supply);
									SupplyQty = 0m;
								}

								if (poLine.Completed == true)
								{
									podemand.Add(demandres);
								}
							}
						}
						if (poLine.LineType == POLineType.GoodsForDropShip ||
							poLine.LineType == POLineType.NonStockForDropShip)
						{
							foreach (PXResult<SOLine4, SOLineSplit> sores in PXSelectJoin<SOLine4,
								InnerJoin<SOLineSplit, On<SOLineSplit.orderType, Equal<SOLine4.orderType>, And<SOLineSplit.orderNbr, Equal<SOLine4.orderNbr>, And<SOLineSplit.lineNbr, Equal<SOLine4.lineNbr>>>>>,
								Where<SOLineSplit.pOType, Equal<Current<POReceiptLine.pOType>>,
									And<SOLineSplit.pONbr, Equal<Current<POReceiptLine.pONbr>>,
										And<SOLineSplit.pOLineNbr, Equal<Current<POReceiptLine.pOLineNbr>>>>>>
								.SelectMultiBound(this, new object[] { line }))
							{
								SOLine4 soline = sores;
								SOLineSplit sosplit = PXCache<SOLineSplit>.CreateCopy(sores);
								SOLine4 copy = PXCache<SOLine4>.CreateCopy(soline);

								if (poLine.Completed == true)
								{
									copy.BaseShippedQty += line.BaseReceiptQty;
									PXDBQuantityAttribute.CalcBaseQty<SOLine4.baseShippedQty>(solineselect.Cache, copy);
									copy.UnbilledQty -= (copy.OrderQty - copy.ShippedQty);
									//revert back for formulas
									copy.ShippedQty = soline.ShippedQty;
									copy.OpenQty = 0m;
									copy.CuryOpenAmt = 0m;
									copy.Cancelled = true;
									copy.OpenLine = false;
									solineselect.Update(copy);

									sosplit.BaseShippedQty += line.BaseReceiptQty;
									PXDBQuantityAttribute.CalcTranQty<SOLineSplit.baseShippedQty>(solinesplitselect.Cache, sosplit);
									sosplit.Completed = true;
									sosplit.POReceiptType = line.ReceiptType;
									sosplit.POReceiptNbr = line.ReceiptNbr;
									sosplit.POCompleted = true;
									sosplit.PlanID = null;

									solinesplitselect.Update(sosplit);

									SOOrder order;
									if ((order = (SOOrder)PXParentAttribute.SelectParent(solineselect.Cache, copy)) != null)
									{
										order.OpenLineCntr--;

										SOOrderShipment oshipment = new PXResult<SOOrder, POReceiptLine>(order, line);

										if (poAddress.IsDefaultAddress != true)
										{
											
											//Ship-To address was changed in PO order and should be copied to SOOrderShipment.
											SOAddress address = new SOAddress();
											address.CustomerID = poAddress.BAccountID;
											address.CustomerAddressID = poAddress.BAccountAddressID;
											address.RevisionID = poAddress.RevisionID;
											address.IsDefaultAddress = poAddress.IsDefaultAddress;
											address.AddressLine1 = poAddress.AddressLine1;
											address.AddressLine2 = poAddress.AddressLine2;
											address.AddressLine3 = poAddress.AddressLine3;
											address.City = poAddress.City;
											address.CountryID = poAddress.CountryID;
											address.State = poAddress.State;
											address.PostalCode = poAddress.PostalCode;

											address = soaddressselect.Insert(address);
											oshipment.ShipAddressID = address.AddressID;
										}
										
										SOOrderShipment existing = PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>, And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>>>>.Select(this, oshipment.ShipmentNbr, oshipment.OrderType, oshipment.OrderNbr);
                                        if (existing == null)
                                        {
                                            ordershipmentselect.Insert(oshipment);
                                            order.ShipmentCntr++;
                                        }
                                        else
                                            existing.ShipmentQty += line.BaseReceiptQty;
                                    }

									if (order.OpenShipmentCntr == 0 && order.OpenLineCntr == 0)
									{
										//invoke automation
										order.Status = SO.SOOrderStatus.Shipping;
										soorderselect.Search<SOOrder.orderNbr>(order.OrderNbr, order.OrderType);
									}

									foreach (INItemPlan soplan in PXSelectJoin<INItemPlan, InnerJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>>, Where<INItemPlan.supplyPlanID, Equal<Required<INItemPlan.supplyPlanID>>, And<INPlanType.isDemand, Equal<boolTrue>, And<INPlanType.isFixed, Equal<boolTrue>>>>>.Select(this, poLine.PlanID))
									{
										this.Caches[typeof(INItemPlan)].Delete(soplan);
									}
								}
								else
								{
									//Even if the poline is partially complete, the receipt will still be available for Invoicing - thus ShipmentCntr should be increased.
									SOOrder order;
									if ((order = (SOOrder)PXParentAttribute.SelectParent(solineselect.Cache, copy)) != null)
									{
										SOOrderShipment oshipment = new PXResult<SOOrder, POReceiptLine>(order, line);
										SOOrderShipment existing = PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>, And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>>>>.Select(this, oshipment.ShipmentNbr, oshipment.OrderType, oshipment.OrderNbr);
										if (existing == null)
										{
											ordershipmentselect.Insert(oshipment);
											order.ShipmentCntr++;
										}
                                        else
                                            existing.ShipmentQty += line.BaseReceiptQty;
									}

									copy.BaseShippedQty += line.BaseReceiptQty;
									solineselect.Update(copy);

									sosplit.BaseShippedQty += line.BaseReceiptQty;
									PXDBQuantityAttribute.CalcTranQty<SOLineSplit.baseShippedQty>(solinesplitselect.Cache, sosplit);
									sosplit.POReceiptType = line.ReceiptType;
									sosplit.POReceiptNbr = line.ReceiptNbr;

									solinesplitselect.Update(sosplit);
								}
							}
						}

						poLine = (POLineUOpen)poLineCache.Update(poLine);
					}

					if (poLine == null && line.OrigTranType == INTranType.Transfer && !string.IsNullOrEmpty(line.OrigRefNbr))
					{
						//all demand will be attached to first split
						INTranSplit intransit = PXSelect<INTranSplit, Where<INTranSplit.tranType, Equal<Required<INTranSplit.tranType>>, And<INTranSplit.refNbr, Equal<Required<INTranSplit.refNbr>>, And<INTranSplit.lineNbr, Equal<Required<INTranSplit.lineNbr>>>>>>.Select(this, line.OrigTranType, line.OrigRefNbr, line.OrigLineNbr);

						decimal? SupplyQty = line.BaseReceiptQty;

						//TODO: revise constraint And<INPlanType.isDemand, Equal<boolTrue>, And<INPlanType.isFixed, Equal<boolTrue>>>
						foreach (PXResult<INItemPlan, INPlanType, SOLineSplit2> demandres in PXSelectJoin<INItemPlan, InnerJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>, LeftJoin<SOLineSplit2, On<SOLineSplit2.planID, Equal<INItemPlan.planID>>>>, Where<INItemPlan.supplyPlanID, Equal<Required<INItemPlan.supplyPlanID>>>>.Select(this, intransit.PlanID))
						{
							INItemPlan demand = demandres;
							SOLineSplit2 sodemand = demandres;

							if (SupplyQty >= demand.PlanQty)
							{
								INItemPlan supply = new INItemPlan();
								supply.InventoryID = line.InventoryID;
								supply.SiteID = line.SiteID;
								supply.SubItemID = line.SubItemID;
								supply.PlanQty = demand.PlanQty;
								supply.DemandPlanID = demand.PlanID;
								supply.RefNoteID = demand.RefNoteID;
								supply.PlanDate = demand.PlanDate;
								supply.PlanType = INPlanConstants.Plan64;
								supply.Hold = false;

								posupply.Add(supply);

								SupplyQty -= demand.PlanQty;
								podemand.Add(demandres);
							}
							else if (SupplyQty > 0m)
							{
								INItemPlan supply = new INItemPlan();
								supply.InventoryID = line.InventoryID;
								supply.SiteID = line.SiteID;
								supply.SubItemID = line.SubItemID;
								supply.PlanQty = SupplyQty;
								supply.DemandPlanID = demand.PlanID;
								supply.RefNoteID = demand.RefNoteID;
								supply.PlanDate = demand.PlanDate;
								supply.PlanType = INPlanConstants.Plan64;
								supply.Hold = false;

								posupply.Add(supply);

								SupplyQty = 0m;
							}

							//TODO: revise this criteria for Intransit
							if (line.AllowComplete == true)
							{
								podemand.Add(demandres);
							}
						}
					}
				}
				prev_line = line;
				if (newline != null && !string.IsNullOrEmpty(split.ReceiptNbr))
				{
					INTranSplit newsplit = (INTranSplit)newline;
					newsplit.SplitLineNbr = null;
					newsplit.SubItemID = split.SubItemID;
					newsplit.LocationID = split.LocationID;
					newsplit.LotSerialNbr = split.LotSerialNbr;
					newsplit.InventoryID = split.InventoryID;
					newsplit.UOM = split.UOM;
					newsplit.Qty = split.Qty;
					newsplit.ExpireDate = split.ExpireDate;
					newsplit.POLineType = poLine != null ? poLine.LineType : line.LineType;
					newsplit.OrigPlanType = line.OrigTranType == INTranType.Transfer ? line.OrigPlanType : null;
					newsplit = docgraph.splits.Insert(newsplit);

					//TODO: revise constraint 
					foreach (INItemPlan demand in podemand)
					{
						demand.SupplyPlanID = newsplit.PlanID;
						docgraph.Caches[typeof(INItemPlan)].SetStatus(demand, PXEntryStatus.Updated);
					}

					podemand.Clear();

					decimal? UnassignedQty = newsplit.BaseQty;
					foreach (INItemPlan supply in posupply.ToArray())
					{
						if (UnassignedQty <= 0)
						{
							break;
						}

						if (!string.IsNullOrEmpty(newsplit.LotSerialNbr))
						{
							INItemPlan copy = PXCache<INItemPlan>.CreateCopy(supply);
							copy.PlanQty = Math.Min((decimal)supply.PlanQty, (decimal)UnassignedQty);
							copy.LotSerialNbr = newsplit.LotSerialNbr;
							copy.SupplyPlanID = newsplit.PlanID;
							docgraph.Caches[typeof(INItemPlan)].Insert(copy);

							supply.PlanQty -= copy.PlanQty;
							UnassignedQty -= copy.PlanQty;

							if (supply.PlanQty <= 0m)
							{
								posupply.Remove(supply);
							}
						}
						else
						{
							supply.SupplyPlanID = newsplit.PlanID;
							docgraph.Caches[typeof(INItemPlan)].Insert(supply);

							posupply.Remove(supply);
						}
					}
					docgraph.splits.Cache.RaiseRowUpdated(newsplit, newsplit);
				}
				if (newline != null && newline.InventoryID == line.InventoryID)
				{
					//This needs to be done after splits insert - to override recalculation made by them
					if (receiptLineTax != null)
						newline.TranCost = receiptLineTax.TaxableAmt ?? Decimal.Zero;
					else
						newline.TranCost = prev_line.ExtCost ?? Decimal.Zero;
				}

				//Non-Stock Item
				if (newline == null || poLine != null && poLine.LineType == POLineType.NonStockForSalesOrder)
				{
					var planselect = new PXSelect<INItemPlan>(this);
					INPlanType nonstockplantype = new INPlanType { ReplanOnEvent = INPlanConstants.Plan60 };

					var planlist = podemand.ToList();
					planlist.AddRange(posupply.ConvertAll<PXResult<INItemPlan, INPlanType>>(_ => new PXResult<INItemPlan, INPlanType>(planselect.Insert(_), nonstockplantype)));

					SOOrderEntry.ProcessPOReceipt(this, planlist, aDoc.ReceiptType, aDoc.ReceiptNbr);

					podemand.Clear();
					posupply.Clear();
				}
			}

			if (hasStockItems)
			{
				INRegister copy = PXCache<INRegister>.CreateCopy(docgraph.receipt.Current);
				PXFormulaAttribute.CalcAggregate<INTran.qty>(docgraph.transactions.Cache, copy);
				PXFormulaAttribute.CalcAggregate<INTran.tranAmt>(docgraph.transactions.Cache, copy);
				PXFormulaAttribute.CalcAggregate<INTran.tranCost>(docgraph.transactions.Cache, copy);
				docgraph.receipt.Update(copy);
			}
			if ((bool)aDoc.AutoCreateInvoice)
			{
				CurrencyInfo currenceInfo = this.currencyinfo.Search<CurrencyInfo.curyInfoID>(aDoc.CuryInfoID);
				invoiceGraph.InvoicePOReceipt(aDoc, currenceInfo, aAPCreated, false, true);
				invoiceGraph.AttachPrepayment();
			}
			POReceipt receipt_copy = (POReceipt)this.Document.Cache.CreateCopy(this.Document.Current);
			receipt_copy.Released = true;
			receipt_copy = this.Document.Update(receipt_copy);

			foreach (string orderNbr in orderCheckClosed.Keys)
			{
				POLineUOpen line =
					PXSelect<POLineUOpen,
					Where<POLineUOpen.orderNbr, Equal<Required<POLineUOpen.orderNbr>>,
						And<POLineUOpen.lineType, NotEqual<POLineType.description>,
						And<POLineUOpen.completed, Equal<Required<POLineUOpen.completed>>>>>>.Select(this, orderNbr, false);
				POLineUOpen cancelled =
					PXSelect<POLineUOpen,
					Where<POLineUOpen.orderNbr, Equal<Required<POLineUOpen.orderNbr>>,
						And<POLineUOpen.lineType, NotEqual<POLineType.description>,
						And<POLineUOpen.cancelled, Equal<Required<POLineUOpen.cancelled>>>>>>.Select(this, orderNbr, true);
				if (line == null)
				{
					POOrder order = PXSelect<POOrder, Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>.Select(this, orderNbr);
					if (order != null && order.Status != POOrderStatus.Closed && order.Hold != true)
					{
						POOrder upd = poOrderUPD.Cache.CreateCopy(order) as POOrder;
						if (upd != null)
						{
							upd.Receipt = cancelled == null;
							upd.Status = POOrderStatus.Closed;
							poOrderUPD.Update(upd);
						}
					}
				}
			}
			List<INRegister> forReleaseIN = new List<INRegister>();
			List<APRegister> forReleaseAP = new List<APRegister>();
			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					this.Save.Press();
					bool isINDocValid = (docgraph.transactions.Select().Count > 0);
					if (isINDocValid) //Skip saving empty document
					{
						docgraph.Save.Press();
					}
					if ((bool)aDoc.AutoCreateInvoice)
					{
						invoiceGraph.Save.Press();
						APInvoice apDoc = PXSelect<APInvoice, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>, And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(this, invoiceGraph.Document.Current.DocType, invoiceGraph.Document.Current.RefNbr);
						apDoc.Passed = true;
						if (aAPCreated != null)
						{
							aAPCreated.Add(apDoc);
						}
						if (this.posetup.Current.AutoReleaseAP == true && apDoc.Hold == false)
						{
							forReleaseAP.Add((APRegister)apDoc);
							PXTimeStampScope.DuplicatePersisted(this.Caches[typeof(APRegister)], apDoc, typeof(APInvoice));
						}
					}
					if (isINDocValid && this.posetup.Current.AutoReleaseIN == true && docgraph.receipt.Current.Hold == false)
					{
						forReleaseIN.Add(docgraph.receipt.Current);
					}
					ts.Complete();
				}
			}


			if (aINCreated.Find(docgraph.receipt.Current) == null)
			{
				aINCreated.Add(docgraph.receipt.Current);
			}

			if (forReleaseAP.Count > 0)
			{
				try
				{
					APDocumentRelease.ReleaseDoc(forReleaseAP, aIsMassProcess);
				}
				catch (PXException ex)
				{
					throw new PXException(Messages.APDocumentFailedToReleaseDuringPOReceiptRelease, ex.Message);
				}
			}

			if (forReleaseIN.Count > 0)
			{
				try
				{
					INDocumentRelease.ReleaseDoc(forReleaseIN, false);
				}
				catch (PXException ex)
				{
					throw new PXException(Messages.INDocumentFailedToReleaseDuringPOReceiptRelease, ex.Message);
				}
			}

			if (this.landedCostTrans.Select().Count > 0)
			{
				this.ReleaseLandedCostTrans(this.Document.Current, aINCreated, aAPCreated);
			}

		}

		public virtual INReceiptEntry CreateReceiptEntry()
		{
			INReceiptEntry re = PXGraph.CreateInstance<INReceiptEntry>();

			return re;
		}

		public virtual INIssueEntry CreateIssueEntry()
		{
			INIssueEntry ie = PXGraph.CreateInstance<INIssueEntry>();

			this.Caches[typeof(SiteStatus)] = ie.Caches[typeof(SiteStatus)];
			this.Caches[typeof(LocationStatus)] = ie.Caches[typeof(LocationStatus)];
			this.Caches[typeof(LotSerialStatus)] = ie.Caches[typeof(LotSerialStatus)];
			this.Caches[typeof(SiteLotSerial)] = ie.Caches[typeof(SiteLotSerial)];
			this.Caches[typeof(ItemLotSerial)] = ie.Caches[typeof(ItemLotSerial)];

			this.Views.Caches.Remove(typeof(SiteStatus));
			this.Views.Caches.Remove(typeof(LocationStatus));
			this.Views.Caches.Remove(typeof(LotSerialStatus));
			this.Views.Caches.Remove(typeof(SiteLotSerial));
			this.Views.Caches.Remove(typeof(ItemLotSerial));

			ie.FieldDefaulting.AddHandler<SiteStatus.negAvailQty>((sender, e) =>
			{
				INItemClass itemclass = PXSelectReadonly<INItemClass, Where<INItemClass.itemClassID, Equal<Current<SiteStatus.itemClassID>>>>.SelectMultiBound(sender.Graph, new object[] { e.Row });
				e.NewValue = itemclass != null && itemclass.NegQty == true;
				e.Cancel = true;
			});

			return ie;
		}

		public virtual void ReleaseReturn(INIssueEntry docgraph, AP.APInvoiceEntry invoiceGraph, POReceipt aDoc, DocumentList<INRegister> aCreated, DocumentList<AP.APInvoice> aInvoiceCreated, bool aIsMassProcess)
		{
			this.Clear();

			docgraph.Clear();

			docgraph.insetup.Current.RequireControlTotal = false;
			docgraph.insetup.Current.HoldEntry = false;

			invoiceGraph.Clear();

			invoiceGraph.APSetup.Current.RequireControlTotal = false;
			invoiceGraph.APSetup.Current.HoldEntry = false;

			docgraph.insetup.Current.RequireControlTotal = false;

			this.Document.Current = Document.Search<POReceipt.receiptNbr>(aDoc.ReceiptNbr, aDoc.ReceiptType);

			//INRegister newdoc = aINCreated.Find<INRegister.docType, INRegister.siteID, INRegister.tranDate>(aDoc.ReceiptType, SiteID, aDoc.ReceiptDate) ?? new INRegister();
			INRegister newdoc = new INRegister();
			if (newdoc.RefNbr != null)
			{
				docgraph.issue.Current = docgraph.issue.Search<INRegister.refNbr>(newdoc.RefNbr);
			}
			else
			{
                newdoc.DocType = POReceiptType.GetINDocType(aDoc.ReceiptType);
				newdoc.SiteID = null;
				newdoc.TranDate = aDoc.ReceiptDate;
                newdoc.FinPeriodID = aDoc.FinPeriodID;
                newdoc.OrigModule = GL.BatchModule.PO;
				docgraph.issue.Insert(newdoc);
			}


			POReceiptLine prev_line = null;
			INTran newline = null;
			Dictionary<string, string> orderCheckClosed = new Dictionary<string, string>();
			bool hasStockItems = false;
			PXSelectBase<POReceiptTax> inclusiveLineTaxSelect = new PXSelectJoin<POReceiptTax,
									InnerJoin<Tax, On<POReceiptTax.taxID, Equal<Tax.taxID>,
										And<Tax.taxCalcLevel, Equal<CSTaxCalcLevel.inclusive>>>>,
									Where<POReceiptTax.receiptNbr, Equal<Required<POReceiptTax.receiptNbr>>,
										And<POReceiptTax.lineNbr, Equal<Required<POReceiptTax.lineNbr>>>>>(this);
			POReceiptTax receiptLineTax = null;

			foreach (PXResult<POReceiptLine, POReceiptLineSplit, POLineUOpen, INTran, INItemPlan, INPlanType> res in PXSelectJoin<POReceiptLine,
				LeftJoin<POReceiptLineSplit, On<POReceiptLineSplit.receiptNbr, Equal<POReceiptLine.receiptNbr>, And<POReceiptLineSplit.lineNbr, Equal<POReceiptLine.lineNbr>>>,
				LeftJoin<POLineUOpen, On<POLineUOpen.orderType, Equal<POReceiptLine.pOType>, And<POLineUOpen.orderNbr, Equal<POReceiptLine.pONbr>, And<POLineUOpen.lineNbr, Equal<POReceiptLine.pOLineNbr>>>>,
				LeftJoin<INTran, On<INTran.pOReceiptNbr, Equal<POReceiptLine.receiptNbr>, And<INTran.pOReceiptLineNbr, Equal<POReceiptLine.lineNbr>>>,
				LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<POReceiptLineSplit.planID>>,
				LeftJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>>>>>>,
				Where<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>,
					And<INTran.refNbr, IsNull>>,
					OrderBy<Asc<POReceiptLine.receiptNbr, Asc<POReceiptLine.lineNbr>>>>.Select(this))
			{
				POReceiptLine line = (POReceiptLine)res;
				POReceiptLineSplit split = (POReceiptLineSplit)res;
				INItemPlan plan = PXCache<INItemPlan>.CreateCopy((INItemPlan)res);
				INPlanType plantype = (INPlanType)res;
				POLineUOpen poLine = (POLineUOpen)res;
				//Splits actually exist only for the Stock Items
				if (!string.IsNullOrEmpty(split.ReceiptNbr))
				{
					if (!string.IsNullOrEmpty(plantype.PlanType) && (bool)plantype.DeleteOnEvent)
					{
						Caches[typeof(INItemPlan)].Delete(plan);
						Caches[typeof(POReceiptLineSplit)].SetStatus(split, PXEntryStatus.Updated);
                        split = (POReceiptLineSplit)Caches[typeof(POReceiptLineSplit)].Locate(split);
                        if (split != null) split.PlanID = null;
						Caches[typeof(POReceiptLineSplit)].IsDirty = true;
					}
					else if (!string.IsNullOrEmpty(plantype.PlanType) && string.IsNullOrEmpty(plantype.ReplanOnEvent) == false)
					{
						plan.PlanType = plantype.ReplanOnEvent;
						Caches[typeof(INItemPlan)].Update(plan);
						//split.Confirmed = true;
						Caches[typeof(POReceiptLineSplit)].SetStatus(split, PXEntryStatus.Updated);
						Caches[typeof(POReceiptLineSplit)].IsDirty = true;
					}
				}
				if ((Caches[typeof(POReceiptLine)].ObjectsEqual(prev_line, line) == false || object.Equals(line.InventoryID, split.InventoryID) == false))
				{
					if (line.IsStockItem() && split.IsStockItem == true)
					{
						hasStockItems = true;
						newline = new INTran();
						newline.BranchID = line.BranchID;
                        newline.TranType = POReceiptType.GetINTranType(aDoc.ReceiptType);
                        newline.POReceiptType = line.ReceiptType;
						newline.POReceiptNbr = line.ReceiptNbr;
						newline.POReceiptLineNbr = line.LineNbr;

                        newline.AcctID = line.POAccrualAcctID;
                        newline.SubID = line.POAccrualSubID;
						newline.ReclassificationProhibited = true;
						newline.InventoryID = split.InventoryID;
						if ((object.Equals(line.InventoryID, split.InventoryID) == false))
						{
							newline.SubItemID = split.SubItemID;
							newline.LocationID = split.LocationID;
							newline.UOM = split.UOM;
							newline.UnitPrice = 0m;
							newline.UnitCost = 0m;
							newline.TranDesc = null;
						}
						else
						{
						newline.SubItemID = line.SubItemID;
							newline.LocationID = line.LocationID;
						newline.UOM = line.UOM;
							newline.TranDesc = line.TranDesc;
							newline.UnitCost = line.UnitCost ?? Decimal.Zero;
						}
						newline.ReasonCode = line.ReasonCode;
						newline.SiteID = line.SiteID;
						newline.InvtMult = line.InvtMult;
						newline.Qty = Decimal.Zero;
						newline.ExpireDate = line.ExpireDate;
						newline.ProjectID = line.ProjectID;
						newline.TaskID = line.TaskID;

						newline = docgraph.lsselect.Insert(newline);
						receiptLineTax = inclusiveLineTaxSelect.Select(line.ReceiptNbr, line.LineNbr); //All the ReceiptTax for inclusive tax have the same taxable amount 
					}
					else
					{
						newline = null;
						receiptLineTax = null;
					}

					
					if (poLine != null && !(string.IsNullOrEmpty(poLine.OrderType) || String.IsNullOrEmpty(poLine.OrderNbr) || poLine.LineNbr == null))
					{
						decimal delta = line.ReceiptQty ?? Decimal.Zero;
                        PXCache poLineCache = this.Caches[typeof(POLineUOpen)];
						poLine = PXCache<POLineUOpen>.CreateCopy(poLine);
						if (line.InventoryID != null && !String.IsNullOrEmpty(line.UOM) && !String.IsNullOrEmpty(poLine.UOM))
							delta = INUnitAttribute.ConvertFromBase(Caches[typeof(POReceiptLine)], line.InventoryID, poLine.UOM, line.BaseReceiptQty.Value, INPrecision.QUANTITY);

                        poLine.OpenQty -= delta * line.InvtMult;
						if (poLine.OpenQty < 0)
							poLine.OpenQty = 0;

						poLine.CuryOpenAmt -= line.CuryLineAmt.GetValueOrDefault() * line.InvtMult;
						if (poLine.CuryOpenAmt < 0)
							poLine.CuryOpenAmt = 0;

						if (poLine.AllowComplete == true && poLine.Completed == true)
							{
								poLine.Completed = false;
								orderCheckClosed[poLine.OrderNbr] = poLine.OrderNbr;
							}
                        poLine = (POLineUOpen)poLineCache.Update(poLine);
					}
				
				}
				prev_line = line;
				if (newline != null && !string.IsNullOrEmpty(split.ReceiptNbr))
				{
					INTranSplit newsplit = (INTranSplit)newline;
					newsplit.SplitLineNbr = null;
					newsplit.InventoryID = split.InventoryID;
					newsplit.SubItemID = split.SubItemID;
					newsplit.LocationID = split.LocationID;
					newsplit.LotSerialNbr = split.LotSerialNbr;
					newsplit.UOM = split.UOM;
					newsplit.Qty = split.Qty;
					newsplit.ExpireDate = split.ExpireDate;
                    newsplit.POLineType = poLine.LineType;
					newsplit = docgraph.splits.Insert(newsplit);
					docgraph.splits.Cache.RaiseRowUpdated(newsplit, newsplit);
				}
				if (newline != null)
				{
					//This needs to be done after splits insert - to override recalculation made by them
					if (receiptLineTax != null)
						newline.TranCost = receiptLineTax.TaxableAmt ?? Decimal.Zero;
					else
						newline.TranCost = prev_line.ExtCost ?? Decimal.Zero;
				}
			}

			if (hasStockItems)
			{
				INRegister copy = PXCache<INRegister>.CreateCopy(docgraph.issue.Current);
				PXFormulaAttribute.CalcAggregate<INTran.qty>(docgraph.transactions.Cache, copy);
				PXFormulaAttribute.CalcAggregate<INTran.tranAmt>(docgraph.transactions.Cache, copy);
				PXFormulaAttribute.CalcAggregate<INTran.tranCost>(docgraph.transactions.Cache, copy);
				docgraph.issue.Update(copy);
			}
			if ((bool)aDoc.AutoCreateInvoice)
			{
				CurrencyInfo currenceInfo = this.currencyinfo.Search<CurrencyInfo.curyInfoID>(aDoc.CuryInfoID);
				invoiceGraph.InvoicePOReceipt(aDoc, currenceInfo, aInvoiceCreated, false, true);
			}
			POReceipt receipt_copy = (POReceipt)this.Document.Cache.CreateCopy(this.Document.Current);
			receipt_copy.Released = true;
			receipt_copy = this.Document.Update(receipt_copy);

			foreach (string orderNbr in orderCheckClosed.Keys)
			{
					POOrder order = PXSelect<POOrder, Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>.Select(this, orderNbr);
				if (order != null && order.Status == POOrderStatus.Closed && order.Hold != true)
					{
						POOrder upd = poOrderUPD.Cache.CreateCopy(order) as POOrder;
						if (upd != null)
						{
						upd.Receipt = false;
						upd.Status = POOrderStatus.Open; //???
							poOrderUPD.Update(upd);
						}
					}
				}
			List<INRegister> forReleaseIN = new List<INRegister>();
			List<APRegister> forReleaseAP = new List<APRegister>();

			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					this.Save.Press();
					bool isINDocValid = (docgraph.transactions.Select().Count > 0);
					if (isINDocValid) //Skip saving empty document - if receipt containes Non-Stocks only
					{
						docgraph.Save.Press();
					}

					if ((bool)aDoc.AutoCreateInvoice)
					{
						invoiceGraph.Save.Press();
						APInvoice apDoc = invoiceGraph.Document.Current;
						if (this.posetup.Current.AutoReleaseAP == true && apDoc.Hold == false)
							forReleaseAP.Add((APRegister)apDoc);
					}

					if (isINDocValid && this.posetup.Current.AutoReleaseIN == true && docgraph.issue.Current.Hold == false)
					{
						forReleaseIN.Add(docgraph.issue.Current);
					}
					ts.Complete();
				}
			}



			if (aCreated.Find(docgraph.issue.Current) == null)
			{
				aCreated.Add(docgraph.issue.Current);
			}

			if (forReleaseAP.Count > 0)
			{
				try
				{
					APDocumentRelease.ReleaseDoc(forReleaseAP, aIsMassProcess);
				}
                catch (PXException ex)
				{
					throw new PXException(Messages.APDocumentFailedToReleaseDuringPOReceiptRelease, ex.Message);
				}
			}
			if (forReleaseIN.Count > 0)
			{
				try
				{
					INDocumentRelease.ReleaseDoc(forReleaseIN, false);
				}
				catch (PXException ex)
				{
					throw new PXException(Messages.INDocumentFailedToReleaseDuringPOReceiptRelease, ex.Message);
				}
			}
		}

		public virtual void ReleaseLandedCostTrans(POReceipt aDoc, DocumentList<INRegister> aINCreated, DocumentList<APInvoice> aAPCreated)
		{
			this.Document.Current = Document.Search<POReceipt.receiptNbr>(aDoc.ReceiptNbr, aDoc.ReceiptType);
			int count = this.landedCostTrans.Select().Count;
			if (count > 0)
			{
				LandedCostProcess lcGraph = PXGraph.CreateInstance<LandedCostProcess>();
				List<LandedCostTran> lcTrans = new List<LandedCostTran>(count);
				foreach (LandedCostTran itr in this.landedCostTrans.Select())
				{
					lcTrans.Add(itr);
				}
				lcGraph.ReleaseLCTrans(lcTrans, aINCreated, aAPCreated);
			}
#if false
			Dictionary<int, APInvoiceEntry> apGraphs = new Dictionary<int, APInvoiceEntry>();
			Dictionary<int, INAdjustmentEntry> inGraphs = new Dictionary<int, INAdjustmentEntry>();
			foreach (LandedCostTran iTran in this.landedCostTrans.Select())
			{
				if (string.IsNullOrEmpty(iTran.APDocType) || string.IsNullOrEmpty(iTran.APRefNbr) && iTran.CuryLCAmount > Decimal.Zero)
				{
					APInvoiceEntry apGraph = PXGraph.CreateInstance<APInvoiceEntry>();
					apGraph.InvoiceLandedCost(iTran, aAPCreated, false);
					apGraphs[iTran.LCTranID.Value] = apGraph;
				}

				LandedCostCode lcCode = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, iTran.LandedCostCodeID);
				List<POReceiptLine> result = LandedCostUtils.AllocateOverRCTLines(this.transactions.Select(), this.transactions.Cache, lcCode, iTran);
				if (result.Count > 0)
				{
					INAdjustmentEntry inGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
					inGraph.Clear();

					inGraph.insetup.Current.RequireControlTotal = false;
					inGraph.insetup.Current.HoldEntry = false;

					INRegister newdoc = new INRegister();
					newdoc.DocType = INDocType.Adjustment;
					newdoc.SiteID = null;
					newdoc.TranDate = aDoc.ReceiptDate;
					inGraph.adjustment.Insert(newdoc);

					INRegister inReceipt = PXSelectJoin<INRegister, InnerJoin<INTran, On<INTran.docType, Equal<INRegister.docType>,
															And<INTran.refNbr, Equal<INRegister.refNbr>>>>,
										Where<INTran.pOReceiptNbr, Equal<Required<INTran.pOReceiptNbr>>>>.SelectWindowed(this, 0, 1, aDoc.ReceiptNbr);
					if (inReceipt != null)
					{
						foreach (POReceiptLine it in result)
						{
							if (!it.IsStockItem()) continue; //Handle separately
							INTran tran = new INTran();
							tran.InventoryID = it.InventoryID;
							tran.SubItemID = it.SubItemID;
							tran.SiteID = it.SiteID;
							tran.LocationID = it.LocationID;
							tran.LotSerialNbr = it.LotSerialNbr;
							tran.UOM = it.UOM;
							tran.Qty = it.ReceiptQty;
							tran.TranDesc = iTran.Descr;
							tran.UnitCost = Decimal.Zero;
							tran.TranCost = it.ExtCost;
							tran.ReasonCode = lcCode.ReasonCode;
							tran.OrigTranType = inReceipt.DocType;
							tran.OrigRefNbr = inReceipt.RefNbr;
							tran.AcctID = lcCode.LCAccrualAcct;
							tran.SubID = lcCode.LCAccrualSub;
							tran = inGraph.transactions.Insert(tran);
						}
					}
					inGraphs[iTran.LCTranID.Value] = inGraph;

				}
			}

			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					foreach (LandedCostTran iTran in this.landedCostTrans.Select())
					{
						bool needUpdate = false;
						if (apGraphs.ContainsKey(iTran.LCTranID.Value))
						{
							APInvoiceEntry apGraph = apGraphs[iTran.LCTranID.Value];
							apGraph.Save.Press();
							iTran.APDocType = apGraph.Document.Current.DocType;
							iTran.APRefNbr = apGraph.Document.Current.RefNbr;
							needUpdate = true;
						}
						if (inGraphs.ContainsKey(iTran.LCTranID.Value))
						{
							INAdjustmentEntry inGraph = inGraphs[iTran.LCTranID.Value];
							inGraph.Save.Press();
							iTran.INDocType = inGraph.adjustment.Current.DocType;
							iTran.INRefNbr = inGraph.adjustment.Current.RefNbr;
							needUpdate = true;
						}
						if (needUpdate)
							this.landedCostTrans.Update(iTran);
					}
					this.Save.Press();
					ts.Complete();
				}
			}
			
#endif
		}
		#endregion

		#region Internal variables
		private bool inventoryIDChanging = false;
		private bool doCancel0;
		private bool _skipUIUpdate = false;
		private bool _forcePriceUpdate = false;
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
            //[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
            [Vendor(typeof(Search<BAccountR.bAccountID,
                Where<BAccountR.type, Equal<BAccountType.companyType>, Or<Vendor.type, NotEqual<BAccountType.employeeType>>>>), Visibility = PXUIVisibility.SelectorVisible, CacheGlobal = true, Filterable = true)]
            [PXRestrictor(typeof(Where<Vendor.status, IsNull,
                                    Or<Vendor.status, Equal<BAccount.status.active>,
                                    Or<Vendor.status, Equal<BAccount.status.oneTime>>>>), AP.Messages.VendorIsInStatus, typeof(Vendor.status))]
			[PXDefault(typeof(POReceipt.vendorID))]
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
			[PXDefault(typeof(POReceipt.vendorLocationID))]
			[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POOrderFilter.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]			
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
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[POLineInventoryItem()]
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
			#region SubItemID
			public abstract class subItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _SubItemID;
			[SubItem(typeof(POOrderFilter.inventoryID))]
			[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
				Where<InventoryItem.inventoryID, Equal<Current2<POOrderFilter.inventoryID>>,
				And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
				PersistingCheck = PXPersistingCheck.Nothing)]
			[PXFormula(typeof(Default<POOrderFilter.inventoryID>))]
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
			#region OrderType
			public abstract class orderType : PX.Data.IBqlField
			{
			}
			protected String _OrderType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(POOrderType.RegularOrder)]
			[POOrderType.RegularDropShipList()]
			[PXUIField(DisplayName = "Type")]
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
				LeftJoin<Vendor, On<POOrderS.vendorID, Equal<Vendor.bAccountID>>>,
				Where<POOrderS.orderType, Equal<Optional<POOrderFilter.orderType>>,
				And<POOrderS.curyID, Equal<Current<POReceipt.curyID>>,
				And<POOrderS.hold, Equal<boolFalse>,
				And2<Where<Current<POReceipt.vendorID>, IsNull,
								     Or<POOrderS.vendorID, Equal<Current<POReceipt.vendorID>>>>,
				And2<Where<Current<POReceipt.vendorLocationID>, IsNull,
										 Or<POOrderS.vendorLocationID, Equal<Current<POReceipt.vendorLocationID>>>>,				
				And2<Where<POOrderS.shipToBAccountID, Equal<Current<POOrderFilter.shipToBAccountID>>,
									Or<Current<POOrderFilter.shipToBAccountID>, IsNull>>,
								And<Where<POOrderS.shipToLocationID, Equal<Current<POOrderFilter.shipToLocationID>>,
						    Or<Current<POOrderFilter.shipToLocationID>, IsNull>>>>>>>>>>), Filterable = true)]
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
            #region SOOrderNbr
            public abstract class sOOrderNbr : PX.Data.IBqlField
            {
            }
            protected String _SOOrderNbr;
            [PXDBString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Transfer Nbr.", Visible = false)]
            [PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<SOOrderTypeConstants.transferOrder>>>))]
            public virtual String SOOrderNbr
            {
                get
                {
                    return this._SOOrderNbr;
                }
                set
                {
                    this._SOOrderNbr = value;
                }
            }
            #endregion
			#region ShipToBAccountID
			public abstract class shipToBAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _ShipToBAccountID;
			[PXDBInt()]
			[PXSelector(typeof(Search2<BAccount2.bAccountID,
			LeftJoin<AR.Customer, On<AR.Customer.bAccountID, Equal<BAccount2.bAccountID>>>,
			Where<Optional<POOrderFilter.orderType>, Equal<POOrderType.regularOrder>,
			Or<Where<AR.Customer.bAccountID, IsNotNull, And<Optional<POOrderFilter.orderType>, Equal<POOrderType.dropShip>>>>>>),
				typeof(BAccount.acctCD), typeof(BAccount.acctName), typeof(BAccount.type), typeof(BAccount.acctReferenceNbr), typeof(BAccount.parentBAccountID),
			SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName))]
			[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Ship To")]
			public virtual Int32? ShipToBAccountID
			{
				get
				{
					return this._ShipToBAccountID;
				}
				set
				{
					this._ShipToBAccountID = value;
				}
			}
			#endregion
			#region ShipToLocationID
			public abstract class shipToLocationID : PX.Data.IBqlField
			{
			}
			protected Int32? _ShipToLocationID;
			[LocationID(typeof(Where<Location.bAccountID, Equal<Optional<POOrderFilter.shipToBAccountID>>>), DescriptionField = typeof(Location.descr))]
			[PXDefault(typeof(Search<BAccount2.defLocationID, Where<BAccount2.bAccountID, Equal<Current<POOrderFilter.shipToBAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Shipping Location")]
			public virtual Int32? ShipToLocationID
			{
				get
				{
					return this._ShipToLocationID;
				}
				set
				{
					this._ShipToLocationID = value;
				}
			}
			#endregion
            #region ShipFromSiteID
            public abstract class shipFromSiteID : PX.Data.IBqlField
            {
            }
            protected Int32? _ShipFromSiteID;
            [Site(DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]
            public virtual Int32? ShipFromSiteID
            {
                get
                {
                    return this._ShipFromSiteID;
                }
                set
                {
                    this._ShipFromSiteID = value;
                }
            }
            #endregion
			#region AnyCurrency
			public abstract class anyCurrency : PX.Data.IBqlField
			{
			}
			protected Boolean? _AnyCurrency;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Show POs in All Currencies")]
			public virtual Boolean? AnyCurrency
			{
				get
				{
					return this._AnyCurrency;
				}
				set
				{
					this._AnyCurrency = value;
				}
			}
			#endregion
			#region ResetFilter
			public abstract class resetFilter : PX.Data.IBqlField
			{
			}
			protected Boolean? _ResetFilter;
			[PXDBBool()]
			[PXDefault(false)]			
			public virtual Boolean? ResetFilter
			{
				get
				{
					return this._ResetFilter;
				}
				set
				{
					this._ResetFilter = value;
				}
			}
			#endregion
			#region hideAddButton
			public abstract class allowAddLine : PX.Data.IBqlField
			{
			}
			protected Boolean? _AllowAddLine;
			[PXDBBool()]
			[PXDefault(true)]
			public virtual Boolean? AllowAddLine
			{
				get
				{
					return this._AllowAddLine;
				}
				set
				{
					this._AllowAddLine = value;
				}
			}
			#endregion
		}



        [PXProjection(typeof(Select<POLine>), Persistent = false)]
        [Serializable]
		public partial class POLineS : POLine
		{			
			#region OrderType
			public new abstract class orderType : PX.Data.IBqlField
			{
			}
			[PXDBString(2, IsKey = true, IsFixed = true)]
			[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
			public override String OrderType
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
			public new abstract class orderNbr : PX.Data.IBqlField
			{
			}
			
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]			
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = true)]
			public override String OrderNbr
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
			public new abstract class lineNbr : PX.Data.IBqlField
			{
			}
			[PXDBInt(IsKey = true)]
			[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]			
			public override Int32? LineNbr
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
			#region VendorID
			public new abstract class vendorID : PX.Data.IBqlField
			{
			}
			[Vendor(typeof(Search<BAccountR.bAccountID,
				Where<BAccountR.type, Equal<BAccountType.companyType>,
						Or<Vendor.type, NotEqual<BAccountType.employeeType>>>>),
				DescriptionField = typeof(BAccount.acctName), CacheGlobal = true, Filterable = true)]			
			public override Int32? VendorID
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
			#region ReceivedQty
			public new abstract class receivedQty : PX.Data.IBqlField
			{
			}
			#endregion
			#region BaseReceivedQty
			public new abstract class baseReceivedQty : PX.Data.IBqlField
			{
			}

			#endregion
			#region CuryInfoID
			public new abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			public override Int64? CuryInfoID
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
			#region CuryReceivedCost
			public new abstract class curyReceivedCost : PX.Data.IBqlField
			{
			}

			#endregion
			#region ReceivedCost
			public new abstract class receivedCost : PX.Data.IBqlField
			{
			}

			#endregion
			#region Selected
			public new abstract class selected : PX.Data.IBqlField
			{
			}
			#endregion
			#region PlanID
			public new abstract class planID : PX.Data.IBqlField
			{
			}

			[PXDBLong()]
			public override Int64? PlanID
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
			#region LineType
			public new abstract class lineType : PX.Data.IBqlField
			{
			}
			#endregion
			#region Cancelled
			public new abstract class cancelled : PX.Data.IBqlField
			{
			}
			#endregion
			#region Completed
			public new abstract class completed : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrderQty
			public new abstract class orderQty : PX.Data.IBqlField
			{
			}
			#endregion
			#region ClosedQty
			public new abstract class closedQty : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			public override Decimal? ClosedQty
			{
				get
				{
					return this._ClosedQty;
				}
				set
				{
					this._ClosedQty = value;
				}
			}
			#endregion
			#region CuryClosedAmt
			public new abstract class curyClosedAmt : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			public override Decimal? CuryClosedAmt
			{
				get
				{
					return this._CuryClosedAmt;
				}
				set
				{
					this._CuryClosedAmt = value;
				}
			}
			#endregion
			#region TaxCategoryID
			public new abstract class taxCategoryID : PX.Data.IBqlField
			{
			}

			[PXDBString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
            [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
            [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
            public override String TaxCategoryID
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
			#region InventoryID
			public new abstract class inventoryID : PX.Data.IBqlField
			{
			}
			//Cross-Item Attributes causes an unwanted actions on FieldUpdating event
			[Inventory(Filterable = true, DisplayName = "Inventory ID")]
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
			#region SubItemID
			public new abstract class subItemID : PX.Data.IBqlField
			{
			}
			#endregion
			#region PromisedDate
			public new abstract class promisedDate : PX.Data.IBqlField
			{
			}			
			[PXDBDate()]			
			[PXUIField(DisplayName = "Promised Date")]
			public override DateTime? PromisedDate
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
		}


		[PXProjection(typeof(Select5<POOrder,
										InnerJoin<POLineS,
											 On<POLineS.orderType, Equal<POOrder.orderType>,
											And<POLineS.orderNbr, Equal<POOrder.orderNbr>,
											And<POLineS.lineType, NotEqual<POLineType.service>,
											And<POLineS.lineType, NotEqual<POLineType.description>,
											And2<Where<CurrentValue<POReceiptLineS.inventoryID>, IsNull,
															Or<POLineS.inventoryID, Equal<CurrentValue<POReceiptLineS.inventoryID>>>>,
                                            And<Where<CurrentValue<POReceiptLineS.subItemID>, IsNull,
                                                         Or<POLineS.subItemID, Equal<CurrentValue<POReceiptLineS.subItemID>>>>>>>>>>>,
											Aggregate
												<GroupBy<POOrder.orderType,
												GroupBy<POOrder.orderNbr,
												GroupBy<POOrder.orderDate,
												GroupBy<POOrder.curyID,
												GroupBy<POOrder.curyOrderTotal,
												GroupBy<POOrder.hold,
												GroupBy<POOrder.status,
												GroupBy<POOrder.receipt,
												GroupBy<POOrder.cancelled,
												GroupBy<POOrder.isTaxValid,
												GroupBy<POOrder.isOpenTaxValid,
												Sum<POLineS.orderQty,
												Sum<POLineS.receivedQty,
												Sum<POLineS.baseReceivedQty,
												Sum<POLineS.curyReceivedCost,
												Sum<POLineS.receivedCost>>>>>>>>>>>>>>>>>>), Persistent = false)]
        [Serializable]
		public partial class POOrderS : POOrder
		{
			#region Selected
			public new abstract class selected : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrderType
			public new abstract class orderType : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrderNbr
			public new abstract class orderNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region ReceivedQty
			public abstract class receivedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceivedQty;
			[PXDBQuantity(HandleEmptyKey = true, BqlField = typeof(POLineS.receivedQty))]
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
			[PXDBDecimal(6, BqlField = typeof(POLineS.baseReceivedQty))]
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
			[PXDBDecimal(6, BqlField = typeof(POLineS.curyReceivedCost))]
			[PXUIField(DisplayName = "Received Amt", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
			[PXDBDecimal(6, BqlField = typeof(POLineS.receivedCost))]
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
			#region Hold
			public new abstract class hold : PX.Data.IBqlField
			{
			}
			#endregion
			#region Cancelled
			public new abstract class cancelled : PX.Data.IBqlField
			{
			}
			#endregion
			#region Receipt
			public new abstract class receipt : PX.Data.IBqlField
			{
			}
			#endregion
			#region IsTaxValid
			public new abstract class isTaxValid : PX.Data.IBqlField
			{
			}
			#endregion
			#region IsOpenTaxValid
			public new abstract class isOpenTaxValid : PX.Data.IBqlField
			{
			}
			#endregion
			#region CuryLeftToReceiveCost
			public abstract class curyLeftToReceiveCost : PX.Data.IBqlField
			{
			}


			[PXCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.leftToReceiveCost))]
			[PXUIField(DisplayName = "Open Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual Decimal? CuryLeftToReceiveCost
			{
                [PXDependsOnFields(typeof(curyLineTotal), typeof(curyReceivedCost))]
				get
				{
					return (this.CuryLineTotal - this.CuryReceivedCost);
				}
			}
			#endregion
			#region LeftToReceiveCost
			public abstract class leftToReceiveCost : PX.Data.IBqlField
			{
			}

			[PXBaseCury()]
			public virtual Decimal? LeftToReceiveCost
			{
                [PXDependsOnFields(typeof(lineTotal), typeof(receivedCost))]
				get
				{
					return (this.LineTotal - this.ReceivedCost);
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
                [PXDependsOnFields(typeof(orderQty), typeof(receivedQty))]
				get
				{
					return (this.OrderQty - this.ReceivedQty);
				}
			}
			#endregion
			#region CuryInfoID
			public new abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			public override Int64? CuryInfoID
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
		}
        [Serializable]
		public partial class POReceiptLineS : PX.Data.IBqlTable
		{
			#region BarCode
			public abstract class barCode : PX.Data.IBqlField
			{
			}
			protected String _BarCode;
			[PXDBString(255)]
			[PXUIField(DisplayName = "Barcode")]
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
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[POLineInventoryItem()]
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[Vendor(typeof(Search<BAccountR.bAccountID,
				Where<BAccountR.type, Equal<BAccountType.companyType>,
						Or<Vendor.type, NotEqual<BAccountType.employeeType>>>>), 
				CacheGlobal = true, Filterable = true)]
			[PXDBDefault(typeof(POReceipt.vendorID))]
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
			[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<POReceiptLineS.vendorID>>>>))]		
            [LocationID(typeof(Where<Location.bAccountID, Equal<Current<POReceiptLineS.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Vendor Location")]
			[PXFormula(typeof(Default<POReceiptLineS.vendorID>))]
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
            #region ShipFromSiteID
            public abstract class shipFromSiteID : PX.Data.IBqlField
            {
            }
            protected Int32? _ShipFromSiteID;
            [Site(DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]
            public virtual Int32? ShipFromSiteID
            {
                get
                {
                    return this._ShipFromSiteID;
                }
                set
                {
                    this._ShipFromSiteID = value;
                }
            }
            #endregion
			#region POType
			public abstract class pOType : PX.Data.IBqlField
			{
			}
			protected String _POType;
			[PXDBString(2, IsFixed = true)]
			[POOrderType.RegularDropShipList()]
			[PXDefault(POOrderType.RegularOrder)]
			[PXUIField(DisplayName = "Order Type")]
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
			[PXDBString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "Order Nbr.")]
			[PO.RefNbr(typeof(Search2<POOrder.orderNbr,
				LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
				And<Match<Vendor, Current<AccessInfo.userName>>>>>,
				Where<POOrder.orderType, Equal<Optional<POReceiptLineS.pOType>>,
                And<Vendor.bAccountID, IsNotNull>>,
				OrderBy<Desc<POOrder.orderNbr>>>), Filterable = true)]
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
			[PXDBInt()]
			[PXUIField(DisplayName = "Line Nbr.")]
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
            #region SOOrderType
            public abstract class sOOrderType : PX.Data.IBqlField
            {
            }
            protected String _SOOrderType;
            [PXDBString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Transfer Type", Enabled = false)]
            [PXSelector(typeof(Search<SOOrder.orderType>))]
            public virtual String SOOrderType
            {
                get
                {
                    return this._SOOrderType;
                }
                set
                {
                    this._SOOrderType = value;
                }
            }
            #endregion
            #region SOOrderNbr
            public abstract class sOOrderNbr : PX.Data.IBqlField
            {
            }
            protected String _SOOrderNbr;
            [PXDBString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Transfer Nbr.", Enabled = false)]
            [PXSelector(typeof(Search<SOOrder.orderNbr>))]
            public virtual String SOOrderNbr
            {
                get
                {
                    return this._SOOrderNbr;
                }
                set
                {
                    this._SOOrderNbr = value;
                }
            }
            #endregion
            #region SOOrderLineNbr
            public abstract class sOOrderLineNbr : PX.Data.IBqlField
            {
            }
            protected Int32? _SOOrderLineNbr;
            [PXDBInt()]
            [PXUIField(DisplayName = "Line Nbr.", Enabled = false)]
            public virtual Int32? SOOrderLineNbr
            {
                get
                {
                    return this._SOOrderLineNbr;
                }
                set
                {
                    this._SOOrderLineNbr = value;
                }
            }
            #endregion
            #region SOShipmentNbr
            public abstract class sOShipmentNbr : PX.Data.IBqlField
            {
            }
            protected String _SOShipmentNbr;
            [PXDBString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Shipment Nbr.", Enabled = false)]
            [PXSelector(typeof(Search<SOShipment.shipmentNbr>))]
            public virtual String SOShipmentNbr
            {
                get
                {
                    return this._SOShipmentNbr;
                }
                set
                {
                    this._SOShipmentNbr = value;
                }
            }
            #endregion
            #region OrigRefNbr
            public abstract class origRefNbr : PX.Data.IBqlField
            {
            }
            protected String _OrigRefNbr;
            [PXDBString(15, IsUnicode = true)]
            public virtual String OrigRefNbr
            {
                get
                {
                    return this._OrigRefNbr;
                }
                set
                {
                    this._OrigRefNbr = value;
                }
            }
            #endregion
            #region OrigLineNbr
            public abstract class origLineNbr : PX.Data.IBqlField
            {
            }
            protected Int32? _OrigLineNbr;
            [PXDBInt()]
            public virtual Int32? OrigLineNbr
            {
                get
                {
                    return this._OrigLineNbr;
                }
                set
                {
                    this._OrigLineNbr = value;
                }
            }
            #endregion
			#region TranType
			public abstract class tranType : PX.Data.IBqlField
			{
			}

			public string TranType
			{
				get
				{
					return POReceiptType.GetINTranType(POReceiptType.POReceipt);
				}
			}
			#endregion
			#region InvtMult
			public abstract class invtMult : PX.Data.IBqlField
			{
			}
			protected Int16? _InvtMult;
			[PXDBShort()]
			[PXDefault()]
			public virtual Int16? InvtMult
			{
				get
				{
					return this._InvtMult;
				}
				set
				{
					this._InvtMult = value;
				}
			}
			#endregion
			#region SubItemID
			public abstract class subItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _SubItemID;
			[SubItem(typeof(POReceiptLineS.inventoryID))]			
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
			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;

			[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<POReceiptLineS.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			[INUnit(typeof(POReceiptLineS.inventoryID))]
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
			#region SiteID
			public abstract class siteID : PX.Data.IBqlField
			{
			}
			protected Int32? _SiteID;
			[IN.POSiteAvail(typeof(POReceiptLineS.inventoryID), typeof(POReceiptLineS.subItemID))]
			[PXDefault(typeof(Coalesce<Search<CR.Location.vSiteID,
				Where<CR.Location.locationID, Equal<Current2<POReceiptLineS.vendorLocationID>>,
					And<CR.Location.bAccountID, Equal<Current2<POReceiptLineS.vendorID>>>>>,
						Search<InventoryItem.dfltSiteID, Where<InventoryItem.inventoryID, Equal<Current2<POReceiptLineS.inventoryID>>>>>))]
			[PXFormula(typeof(Default<POReceiptLineS.vendorLocationID>))]
			[PXFormula(typeof(Default<POReceiptLineS.inventoryID>))]
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
			#region LocationID
			public abstract class locationID : PX.Data.IBqlField
			{
			}
			protected Int32? _LocationID;
			[LocationAvail(typeof(POReceiptLineS.inventoryID), typeof(POReceiptLineS.subItemID), typeof(POReceiptLineS.siteID), typeof(POReceiptLineS.tranType), typeof(POReceiptLineS.invtMult), KeepEntry = false)]
			[PXFormula(typeof(Default<POReceiptLineS.siteID>))]
			[PXFormula(typeof(Default<POReceiptLineS.inventoryID>))]			
			public virtual Int32? LocationID
			{
				get
				{
					return this._LocationID;
				}
				set
				{
					this._LocationID = value;
				}
			}
			#endregion
			#region LotSerialNbr
			public abstract class lotSerialNbr : PX.Data.IBqlField
			{
			}
			protected String _LotSerialNbr;
			[LotSerialNbr]
			public virtual String LotSerialNbr
			{
				get
				{
					return this._LotSerialNbr;
				}
				set
				{
					this._LotSerialNbr = value;
				}
			}
			#endregion
			#region ExpireDate
			public abstract class expireDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ExpireDate;
			[PXDBDate(InputMask = "d", DisplayMask = "d")]
			[PXUIField(DisplayName = "Expiration Date")]
			public virtual DateTime? ExpireDate
			{
				get
				{
					return this._ExpireDate;
				}
				set
				{
					this._ExpireDate = value;
				}
			}
			#endregion

			#region ReceiptQty
			public abstract class receiptQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptQty;

			[PXDBQuantity(typeof(POReceiptLineS.uOM), typeof(POReceiptLineS.baseReceiptQty), HandleEmptyKey = true, MinValue = 0)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(null, typeof(SumCalc<POReceipt.orderQty>))]
			[PXUIField(DisplayName = "Receipt Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? ReceiptQty
			{
				get
				{
					return this._ReceiptQty;
				}
				set
				{
					this._ReceiptQty = value;
				}
			}

			public virtual Decimal? Qty
			{
				get
				{
					return this._ReceiptQty;
				}
				set
				{
					this._ReceiptQty = value;
				}
			}
			#endregion
			#region BaseReceiptQty
			public abstract class baseReceiptQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseReceiptQty;

			[PXDBDecimal(6)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(null, typeof(SumCalc<POLineR.baseReceivedQty>))]
			public virtual Decimal? BaseReceiptQty
			{
				get
				{
					return this._BaseReceiptQty;
				}
				set
				{
					this._BaseReceiptQty = value;
				}
			}			
			#endregion			
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _CuryInfoID;
			[PXDBLong()]
			[CurrencyInfo(typeof(POReceipt.curyInfoID))]
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

			[PXDBDecimal(typeof(Search<CommonSetup.decPlPrcCst>))]
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
			#region CuryExtCost
			public abstract class curyExtCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryExtCost;
			
			[PXDBDecimal()]
			[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXFormula(typeof(Mult<POReceiptLineS.receiptQty, POReceiptLineS.curyUnitCost>))]
			[PXFormula(null, typeof(SumCalc<POLineR.curyReceivedCost>))]
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
			#region FetchMode
			public abstract class fetchMode : PX.Data.IBqlField
			{
			}
			protected Boolean? _FetchMode;
			[PXDBBool()]			
			public virtual Boolean? FetchMode
			{
				get
				{
					return this._FetchMode;
				}
				set
				{
					this._FetchMode = value;					
				}
			}
			#endregion
			#region ByOne
			public abstract class byOne : PX.Data.IBqlField
			{
			}
			protected Boolean? _ByOne;
			[PXDBBool()]
			[PXUIField(DisplayName = "Add One Unit per Barcode")]
			[PXDefault(typeof(POSetup.receiptByOneBarcodeReceiptBarcode), PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual Boolean? ByOne
			{
				get
				{
					return this._ByOne;
				}
				set
				{
					this._ByOne = value;
				}
			}
			#endregion
			#region AutoAddLine
			public abstract class autoAddLine : PX.Data.IBqlField
			{
			}
			protected Boolean? _AutoAddLine;
			[PXDBBool()]
			[PXDefault(typeof(POSetup.autoAddLineReceiptBarcode), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Add Line Automatically")]
			public virtual Boolean? AutoAddLine
			{
				get
				{
					return this._AutoAddLine;
				}
				set
				{
					this._AutoAddLine = value;
				}
			}
			#endregion
			#region Description
			public abstract class description : PX.Data.IBqlField
			{
			}
			protected String _Description;
			[PXDBString(255)]
			[PXUIField(DisplayName = "")]
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
		}
		#endregion

		#region Avalara Tax

		public bool IsExternalTax
		{
			get
			{
				TX.TaxZone tz = PXSelect<TX.TaxZone, Where<TX.TaxZone.taxZoneID, Equal<Current<POReceipt.taxZoneID>>>>.Select(this);
				if (tz != null)
					return tz.IsExternal.GetValueOrDefault(false);
				else
					return false;
			}
		}

		public virtual void CalculateAvalaraTax(POReceipt receipt)
		{
			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);

			AvaAddress.AddressSvc addressService = new AvaAddress.AddressSvc();
			AvalaraMaint.SetupService(this, addressService);

			GetTaxRequest getRequest = null;
			GetTaxRequest getRequestUnbilled = null;
			bool isValidByDefault = true;

			if (receipt.IsTaxValid != true)
			{
				getRequest = BuildGetTaxRequest(receipt);

				if (getRequest.Lines.Count > 0)
				{
					isValidByDefault = false;
				}
				else
				{
					getRequest = null;
				}
			}

			if (receipt.IsUnbilledTaxValid != true)
			{
				getRequestUnbilled = BuildGetTaxRequestUnbilled(receipt);
				if (getRequestUnbilled.Lines.Count > 0)
				{
					isValidByDefault = false;
				}
				else
				{
					getRequestUnbilled = null;
				}
			}

			if (isValidByDefault)
			{
				PXDatabase.Update<POReceipt>(
					new PXDataFieldAssign("IsTaxValid", true),
					new PXDataFieldAssign("IsUnbilledTaxValid", true),
					new PXDataFieldRestrict("ReceiptType", PXDbType.VarChar, 2, receipt.ReceiptType, PXComp.EQ),
					new PXDataFieldRestrict("ReceiptNbr", PXDbType.NVarChar, 15, receipt.ReceiptNbr, PXComp.EQ)
					);
				receipt.IsTaxValid = true;
				receipt.IsUnbilledTaxValid = true;
				PXTimeStampScope.PutPersisted(this.Document.Cache, receipt, PXDatabase.SelectTimeStamp());

				return;
			}

			GetTaxResult result = null;
			GetTaxResult resultUnbilled = null;

			bool getTaxFailed = false;
			if (getRequest != null)
			{
				result = service.GetTax(getRequest);
				if (result.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}
			if (getRequestUnbilled != null)
			{
				resultUnbilled = service.GetTax(getRequestUnbilled);
				if (resultUnbilled.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}

			if (!getTaxFailed)
			{
				try
				{
					ApplyAvalaraTax(receipt, result, resultUnbilled);
					PXDatabase.Update<POReceipt>(
						new PXDataFieldAssign("IsTaxValid", true),
					new PXDataFieldAssign("IsUnbilledTaxValid", true),
					new PXDataFieldRestrict("ReceiptType", PXDbType.VarChar, 2, receipt.ReceiptType, PXComp.EQ),
					new PXDataFieldRestrict("ReceiptNbr", PXDbType.NVarChar, 15, receipt.ReceiptNbr, PXComp.EQ)
						);
					receipt.IsTaxValid = true;
					receipt.IsUnbilledTaxValid = true;
					PXTimeStampScope.PutPersisted(this.Document.Cache, receipt, PXDatabase.SelectTimeStamp());

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

		protected virtual GetTaxRequest BuildGetTaxRequest(POReceipt receipt)
		{
			if (receipt == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Location loc = (Location)location.View.SelectSingleBound(new object[] { receipt });
			Vendor vend = (Vendor)vendor.View.SelectSingleBound(new object[] { receipt });

			IAddressBase fromAddress = GetFromAddress(receipt);
			IAddressBase toAddress = GetToAddress(receipt);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, receipt.BranchID);
			request.CurrencyCode = receipt.CuryID;
			request.CustomerCode = vend.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("PO.{0}.{1}", receipt.ReceiptType, receipt.ReceiptNbr);
			request.DocDate = receipt.ReceiptDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(receipt);

			int mult = 1;

			if (!string.IsNullOrEmpty(loc.CAvalaraCustomerUsageType))
			{
				request.CustomerUsageType = loc.CAvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.SalesOrder;

			PXSelectBase<POReceiptLine> select = new PXSelectJoin<POReceiptLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POReceiptLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<InventoryItem.salesAcctID>>>>,
				Where<POReceiptLine.receiptType, Equal<Current<POReceipt.receiptType>>, And<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>,
				OrderBy<Asc<POReceiptLine.lineNbr>>>(this);

			//request.Discount = receipt.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<POReceiptLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { receipt }))
			{
				POReceiptLine tran = (POReceiptLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);
				line.Amount = mult * tran.CuryExtCost.GetValueOrDefault();
				line.Description = tran.TranDesc;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Math.Abs(Convert.ToDouble(tran.ReceiptQty.GetValueOrDefault()));
				line.Discounted = request.Discount > 0;
				line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

				if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
					line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;

				request.Lines.Add(line);
			}

			return request;
		}

		protected virtual GetTaxRequest BuildGetTaxRequestUnbilled(POReceipt receipt)
		{
			if (receipt == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Location loc = (Location)location.View.SelectSingleBound(new object[] { receipt });
			Vendor vend = (Vendor)vendor.View.SelectSingleBound(new object[] { receipt });

			IAddressBase fromAddress = GetFromAddress(receipt);
			IAddressBase toAddress = GetToAddress(receipt);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, receipt.BranchID);
			request.CurrencyCode = receipt.CuryID;
			request.CustomerCode = vend.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("PO.{0}.{1}", receipt.ReceiptType, receipt.ReceiptNbr);
			request.DocDate = receipt.ReceiptDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(receipt);

			int mult = 1;

			if (!string.IsNullOrEmpty(loc.CAvalaraCustomerUsageType))
			{
				request.CustomerUsageType = loc.CAvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.SalesOrder;

			PXSelectBase<POReceiptLine> select = new PXSelectJoin<POReceiptLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<POReceiptLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<InventoryItem.salesAcctID>>>>,
				Where<POReceiptLine.receiptType, Equal<Current<POReceipt.receiptType>>, And<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>,
				OrderBy<Asc<POReceiptLine.lineNbr>>>(this);

			//request.Discount = receipt.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<POReceiptLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { receipt }))
			{
				POReceiptLine tran = (POReceiptLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				if (tran.UnbilledAmt > 0)
				{
					Line line = new Line();
					line.No = Convert.ToString(tran.LineNbr);
                    line.Amount = mult * tran.CuryUnbilledAmt.GetValueOrDefault();
					line.Description = tran.TranDesc;
					line.DestinationAddress = request.DestinationAddress;
					line.OriginAddress = request.OriginAddress;
					line.ItemCode = item.InventoryCD;
					line.Qty = Convert.ToDouble(tran.BaseUnbilledQty.GetValueOrDefault());
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
		protected virtual void ApplyAvalaraTax(POReceipt receipt, GetTaxResult result, GetTaxResult resultUnbilled)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { receipt });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			//Clear all existing Tax transactions:
			foreach (PXResult<POReceiptTaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { receipt }))
			{
				POReceiptTaxTran taxTran = (POReceiptTaxTran)res;
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

				POReceiptTaxTran tax = new POReceiptTaxTran();
				tax.ReceiptNbr = receipt.ReceiptNbr;
				tax.TaxID = taxID;
				tax.CuryTaxAmt = Math.Abs(result.TaxSummary[i].Tax);
				tax.CuryTaxableAmt = Math.Abs(result.TaxSummary[i].Taxable);
				tax.TaxRate = Convert.ToDecimal(result.TaxSummary[i].Rate) * 100;
				tax.JurisType = result.TaxSummary[i].JurisType.ToString();
				tax.JurisName = result.TaxSummary[i].JurisName;

				Taxes.Insert(tax);
			}

			bool requireReceiptControlTotal = posetup.Current.RequireReceiptControlTotal == true;
			if (receipt.Hold != true)
				posetup.Current.RequireReceiptControlTotal = false;

			try
			{
				Document.SetValueExt<POReceipt.curyTaxTotal>(receipt, Math.Abs(result.TotalTax));
				if (resultUnbilled != null)
					Document.SetValueExt<POReceipt.curyUnbilledTaxTotal>(receipt, Math.Abs(resultUnbilled.TotalTax));
				Document.Update(receipt);
			}
			finally
			{
				posetup.Current.RequireReceiptControlTotal = requireReceiptControlTotal;
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

		protected virtual IAddressBase GetToAddress(POReceipt receipt)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(receipt.BranchID))
				return (Address)res;

			return null;
		}

		protected virtual CRLocation GetBranchLocation(POReceipt receipt)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, CRLocation> res in select.Select(receipt.BranchID))
				return (CRLocation)res;

			return null;
		}

		protected virtual IAddressBase GetFromAddress(POReceipt receipt)
		{
			Address vendorAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, vendor.Current.DefAddressID);

			return vendorAddress;
		}

		protected virtual string GetAvalaraLocationCode(POReceipt order)
		{
			return null;
		}

		#endregion

        #region Discounts
        protected virtual void RecalculateDiscounts(PXCache sender, POReceiptLine line)
        {
            if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && line.InventoryID != null && line.Qty != null && line.CuryLineAmt != null)
            {
                DiscountEngine<POReceiptLine>.SetDiscounts<POReceiptDiscountDetail>(sender, transactions, line, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.CuryID, Document.Current.ReceiptDate, true, recalcdiscountsfilter.Current);

                RecalculateTotalDiscount();
            }
        }

        private void RecalculateTotalDiscount()
        {
            if (Document.Current != null)
            {
				POReceipt copy = PXCache<POReceipt>.CreateCopy(Document.Current);
				copy.CuryDiscTot = DiscountEngine.GetTotalGroupAndDocumentDiscount<POReceiptDiscountDetail>(DiscountDetails);
				Document.Update(copy);
            }
        }
        #endregion


		public class SOOrderShipmentComparer : IEqualityComparer<POReceiptLine>
		{
			public SOOrderShipmentComparer()
			{
			}

			#region IEqualityComparer<POReceiptLine> Members

			public bool Equals(POReceiptLine x, POReceiptLine y)
			{
				return x.SOOrderType == y.SOOrderType && x.SOOrderNbr == y.SOOrderNbr && x.SOShipmentNbr == y.SOShipmentNbr;
			}

			public int GetHashCode(POReceiptLine obj)
			{
				unchecked
				{
					return obj.SOShipmentNbr.GetHashCode() + 37 * obj.SOOrderType.GetHashCode() + 109 * obj.SOOrderNbr.GetHashCode();
				}
			}

			#endregion
		}


	}

	[PXProjection(typeof(Select<POLine>), Persistent = true)]
    [Serializable]
	public partial class POLineR : IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
		[PXDefault()]
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
		[PXDefault()]
		[PXParent(typeof(Select<POOrder, Where<POOrder.orderType, Equal<Current<POLineR.orderType>>, And<POOrder.orderNbr, Equal<Current<POLineR.orderNbr>>>>>))]
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
		[PXDBString(2, IsFixed = true, BqlField = typeof(POLineR.lineType))]
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
		#region ReceivedQty
		public abstract class receivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedQty;
		[PXDBQuantity(BqlField = typeof(POLine.receivedQty))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible)]
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

		[PXDBBaseQuantity(typeof(POLineR.uOM), typeof(POLineR.receivedQty), BqlField = typeof(POLine.baseReceivedQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region CuryReceivedCost
		public abstract class curyReceivedCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryReceivedCost;

		[PXDBCurrency(typeof(POLineR.curyInfoID), typeof(POLineR.receivedCost), BqlField = typeof(POLine.curyReceivedCost))]
		[PXUIField(DisplayName = "Received Cost", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(POLineR.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
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
		#region Completed
		public abstract class completed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Completed;
		[PXDBBool(BqlField = typeof(POLine.completed))]
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
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool(BqlField = typeof(POLine.cancelled))]
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
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(POLineR.uOM), typeof(POLineR.baseOpenQty), HandleEmptyKey = true, BqlField = typeof(POLine.openQty))]
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
		[PXDBDecimal(6, BqlField = typeof(POLine.baseOpenQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		[PXDBInt(BqlField = typeof(POLine.inventoryID))]
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
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp(BqlField = typeof(POLine.Tstamp))]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region AllowComplete
		public abstract class allowComplete : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowComplete;
		[PXDBBool(BqlField = typeof(POLine.allowComplete))]
		[PXUIField(DisplayName = "Allow Complete", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? AllowComplete
		{
			get
			{
				return this._AllowComplete;
			}
			set
			{
				this._AllowComplete = value;
			}
		}
		#endregion

	}

	[PXProjection(typeof(Select<POLine>), Persistent = true)]
    [Serializable]
	public partial class POLineUOpen : IBqlTable, IItemPlanMaster
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
		[PXDefault()]
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
		[PXDefault()]
		[PXParent(typeof(Select<POOrder, Where<POOrder.orderType, Equal<Current<POLineUOpen.orderType>>, And<POOrder.orderNbr, Equal<Current<POLineUOpen.orderNbr>>>>>))]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(POLineUOpen.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
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
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong(BqlField = typeof(POLine.planID))]
		[POLineRPlanID(typeof(POOrder.noteID), typeof(POOrder.hold))]
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
		#region Completed
		public abstract class completed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Completed;
		[PXDBBool(BqlField = typeof(POLine.completed))]
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
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool(BqlField = typeof(POLine.cancelled))]
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
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(POLineUOpen.uOM), typeof(POLineUOpen.baseOpenQty), HandleEmptyKey = true, BqlField = typeof(POLine.openQty))]
		[PXFormula(null, typeof(SumCalc<POOrder.openOrderQty>))]
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
		[PXDBDecimal(6, BqlField = typeof(POLine.baseOpenQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region CuryOpenAmt
		public abstract class curyOpenAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOpenAmt;
		[PXDBCurrency(typeof(POLineUOpen.curyInfoID), typeof(POLineUOpen.openAmt), BqlField = typeof(POLine.curyOpenAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryOpenAmt
		{
			get
			{
				return this._CuryOpenAmt;
			}
			set
			{
				this._CuryOpenAmt = value;
			}
		}
		#endregion
		#region OpenAmt
		public abstract class openAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenAmt;
		[PXDBDecimal(4, BqlField = typeof(POLine.openAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OpenAmt
		{
			get
			{
				return this._OpenAmt;
			}
			set
			{
				this._OpenAmt = value;
			}
		}
		#endregion
		#region CuryUnitCost
		public abstract class curyUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitCost;

		[PXDBDecimal(4, BqlField = typeof(POLine.curyUnitCost))]
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
		#region GroupDiscountRate
		public abstract class groupDiscountRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _GroupDiscountRate;
		[PXDBDecimal(6, BqlField = typeof(POLine.groupDiscountRate))]
		[PXDefault(TypeCode.Decimal, "1.0")]
		public virtual Decimal? GroupDiscountRate
		{
			get
			{
				return this._GroupDiscountRate;
			}
			set
			{
				this._GroupDiscountRate = value;
			}
		}
		#endregion
        #region DocumentDiscountRate
        public abstract class documentDiscountRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _DocumentDiscountRate;
        [PXDBDecimal(6, BqlField = typeof(POLine.documentDiscountRate))]
        [PXDefault(TypeCode.Decimal, "1.0")]
        public virtual Decimal? DocumentDiscountRate
        {
            get
            {
                return this._DocumentDiscountRate;
            }
            set
            {
                this._DocumentDiscountRate = value;
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
		[POOpenTaxR(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(BqlField = typeof(POLine.inventoryID))]
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
		#region PromisedDate
		public abstract class promisedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PromisedDate;
		[PXDBDate(BqlField = typeof(POLine.promisedDate))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDBInt(BqlField = typeof(POLine.subItemID))]
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
		[PXDBInt(BqlField = typeof(POLine.siteID))]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt(BqlField = typeof(POLine.vendorID))]
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

		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp(BqlField = typeof(POLine.Tstamp))]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region AllowComplete
		public abstract class allowComplete : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowComplete;
		[PXDBBool(BqlField = typeof(POLine.allowComplete))]
		[PXUIField(DisplayName = "Allow Complete", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? AllowComplete
		{
			get
			{
				return this._AllowComplete;
			}
			set
			{
				this._AllowComplete = value;
			}
		}
		#endregion

#if false
		#region Fields, Used for the Blanket Update
		#region BaseOrderQty
		public abstract class baseOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOrderQty;
		[PXDBDecimal(6, BqlField = typeof(POLine.baseOrderQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		//[PXFormula(null, typeof(SumCalc<POLineR.baseReceivedQty>))]
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
		#region POType
		public abstract class pOType : PX.Data.IBqlField
		{
		}
		protected String _POType;
		[PXDBString(2, IsFixed = true, BqlField = typeof(POLine.pOType))]
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
		[PXDBString(15, IsUnicode = true, BqlField = typeof(POLine.pONbr))]
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
		[PXDBInt(BqlField = typeof(POLine.pOLineNbr))]
		[PXParent(typeof(Select<POLineR,
			Where<POLineR.orderType, Equal<Current<POLineUOpen.pOType>>,
										And<POLineR.orderType, Equal<POOrderType.blanket>,
										And<POLineR.orderNbr, Equal<Current<POLineUOpen.pONbr>>,
										And<POLineR.lineNbr, Equal<Current<POLineUOpen.pOLineNbr>>>>>>>))]
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

		#region EffBaseOrderQty
		//Used for Blanket Qty recalculation on POLine update during Receipt Release
		public abstract class effBaseOrderQty : PX.Data.IBqlField
		{
		}

		[PXDecimal(6)]
		[PXFormula(null, typeof(SumCalc<POLineR.baseReceivedQty>))]
		public virtual Decimal? EffBaseOrderQty
		{
			get
			{
				bool isLineClosed = (this.Completed ?? false) || (this.Cancelled ?? false);
				return (isLineClosed ? this._BaseReceivedQty : this._BaseOrderQty);
			}
			set
			{

			}
		}
		#endregion
		#endregion
#endif
	}

}


