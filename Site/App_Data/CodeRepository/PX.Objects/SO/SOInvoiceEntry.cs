using System;
using System.Collections;
using System.Collections.Generic;
using Avalara.AvaTax.Adapter.TaxService;
using PX.CCProcessingBase;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using ItemLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.ItemLotSerial;
using SiteLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.SiteLotSerial;
using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
using LotSerialStatus = PX.Objects.IN.Overrides.INDocumentRelease.LotSerialStatus;
using POLineType = PX.Objects.PO.POLineType;
using POReceiptLine = PX.Objects.PO.POReceiptLine;
using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;
using System.Linq;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.Common;

namespace PX.Objects.SO
{
	public class SOInvoiceEntry : ARInvoiceEntry
	{		
		public PXAction<ARInvoice> selectShipment;
		[PXUIField(DisplayName = "Add Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable SelectShipment(PXAdapter adapter)
		{
			if (this.Document.Cache.AllowDelete)
				shipmentlist.AskExt();
			return adapter.Get();
		}

		public PXAction<ARInvoice> addShipment;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddShipment(PXAdapter adapter)
		{
			bool RequireControlTotal = ARSetup.Current.RequireControlTotal == true;

			foreach (SOOrderShipment shipment in shipmentlist.Cache.Updated)
			{
				if (shipment.Selected == true)
				{
					foreach (PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation> order in 
						PXSelectJoin<SOOrderShipment,
						InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
						InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SOOrder.curyInfoID>>,
						InnerJoin<SOAddress, On<SOAddress.addressID, Equal<SOOrder.billAddressID>>,
						InnerJoin<SOContact, On<SOContact.contactID, Equal<SOOrder.billContactID>>,
						InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>,
						InnerJoin<SOOrderTypeOperation, 
					         On<SOOrderTypeOperation.orderType, Equal<SOOrder.orderType>,
									And<SOOrderTypeOperation.operation, Equal<SOOrderShipment.operation>>>>>>>>>,
					Where<SOOrderShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>, And<SOOrderShipment.orderType, Equal<Current<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>>>>>>.SelectMultiBound(this, new object[] { shipment }))
					{
						SOOrderShipment oshipment = order;

						PXResultset<SOShipLine, SOLine> details = new PXResultset<SOShipLine, SOLine>();

						foreach (PXResult<POReceiptLine, SOLineSplit, SOLine> line in PXSelectJoin<POReceiptLine,
							InnerJoin<SOLineSplit, On<SOLineSplit.pOType, Equal<POReceiptLine.pOType>, And<SOLineSplit.pONbr, Equal<POReceiptLine.pONbr>, And<SOLineSplit.pOLineNbr, Equal<POReceiptLine.pOLineNbr>>>>,
							InnerJoin<SOLine, On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>>,
							Where2<Where<POReceiptLine.lineType, Equal<POLineType.goodsForDropShip>, Or<POReceiptLine.lineType, Equal<POLineType.nonStockForDropShip>>>, And<SOShipmentType.dropShip, Equal<Current<SOOrderShipment.shipmentType>>, And<POReceiptLine.receiptNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOLine.orderType, Equal<Current<SOOrderShipment.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>>>>>>>.SelectMultiBound(this, new object[] { oshipment }))
						{
							details.Add(new PXResult<SOShipLine, SOLine>((SOShipLine)line, line));
						}

						ARSetup.Current.RequireControlTotal = false;
						this.InvoiceOrder((DateTime)this.Accessinfo.BusinessDate, order, details, null, null);
						ARSetup.Current.RequireControlTotal = RequireControlTotal;
                        shipment.HasDetailDeleted = false;
                        shipmentlist.Update(shipment);
						continue;
					}
				}
			}

			shipmentlist.View.Clear();
			//shipmentlist.Cache.Clear(); 
			return adapter.Get();
		}

		public PXAction<ARInvoice> addShipmentCancel;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddShipmentCancel(PXAdapter adapter)
		{
			foreach (SOOrderShipment shipment in shipmentlist.Cache.Updated)
			{
				if (shipment.InvoiceNbr == null)
				{
					shipment.Selected = false;
				}
			}

			shipmentlist.View.Clear();
			//shipmentlist.Cache.Clear();
			return adapter.Get();
		}
        private bool cancelUnitPriceCalculation = false;
		public PXSelect<ARTran, Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>, And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>, And<ARTran.lineType, Equal<SOLineType.freight>>>>, OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>> Freight;
		public PXSelect<ARTran, Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>, And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>, And<ARTran.lineType, Equal<SOLineType.discount>>>>, OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>> Discount;
        public PXSelect<SOInvoice, Where<SOInvoice.docType, Equal<Optional<ARInvoice.docType>>, And<SOInvoice.refNbr, Equal<Optional<ARInvoice.refNbr>>>>> SODocument;
        [PXCopyPasteHiddenView]
        public PXSelectOrderBy<SOOrderShipment, OrderBy<Asc<SOOrderShipment.orderType, Asc<SOOrderShipment.orderNbr, Asc<SOOrderShipment.shipmentNbr, Asc<SOOrderShipment.shipmentType>>>>>> shipmentlist;
		public PXSelect<SOShipment> shipments;
		public PXSelect<SOInvoiceDiscountDetail, Where<SOInvoiceDiscountDetail.tranType, Equal<Current<ARInvoice.docType>>, And<SOInvoiceDiscountDetail.refNbr, Equal<Current<ARInvoice.refNbr>>>>> DiscountDetails;
        public PXSelect<SOInvoiceDiscountDetail, Where<SOInvoiceDiscountDetail.tranType, Equal<Current<SOInvoice.docType>>, And<SOInvoiceDiscountDetail.refNbr, Equal<Current<SOInvoice.refNbr>>>>> SOInvoiceDiscountDetails;
        [PXCopyPasteHiddenView]
        public PXSelect<SOFreightDetail, Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>, And<SOFreightDetail.refNbr, Equal<Current<ARInvoice.refNbr>>>>> FreightDetails;
		public PXSelectReadonly<CCProcTran, Where<CCProcTran.refNbr, Equal<Current<SOInvoice.refNbr>>, 
												And<CCProcTran.docType, Equal<Current<SOInvoice.docType>>>>,
												OrderBy<Desc<CCProcTran.tranNbr>>> ccProcTran;

		public PXSelect<SOAdjust> soadjustments;
        public PXSelect<INTran> inTran;

		public PXSetup<SOOrderType, Where<SOOrderType.orderType, Equal<Optional<SOOrder.orderType>>>> soordertype;

        public PXSelect<ARInvoice> invoiceview;

        [PXViewName(Messages.CustomerCredit)]
        public new ARCustomerCreditHelper<ARInvoice, ARInvoice.customerID> CustomerCreditHelper;

        #region Cache Attached
        #region ARTran
        [PXDBString(2, IsFixed = true)]
		[SOLineType.List()]
		[PXUIField(DisplayName = "Line Type", Visible = false, Enabled = false)]
		[PXDefault(SOLineType.NonInventory)]
		protected virtual void ARTran_LineType_CacheAttached(PXCache sender)
		{ 
		}

		[NonStockNonKitItem(Messages.CannotAddNonStockKitDirectly, typeof(ARTran.sOOrderNbr), Filterable = true)]
		protected virtual void ARTran_InventoryID_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category")]
		[SOInvoiceTax()]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        [PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing, SearchOnDefault = false)]
		protected override void ARTran_TaxCategoryID_CacheAttached(PXCache sender)
		{
		}

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Manual Price", Visible = true)]
		protected virtual void ARTran_ManualPrice_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region ARInvoice
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[ARDocType.SOEntryList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
		protected virtual void ARInvoice_DocType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARInvoiceType.RefNbr(typeof(Search2<AR.Standalone.ARRegisterAlias.refNbr,
			InnerJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<AR.Standalone.ARRegisterAlias.docType>,
				And<ARInvoice.refNbr, Equal<AR.Standalone.ARRegisterAlias.refNbr>>>,
			InnerJoinSingleTable<Customer, On<AR.Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<AR.Standalone.ARRegisterAlias.docType, Equal<Optional<ARInvoice.docType>>,
				And<AR.Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleSO>,
				And<Match<Customer, Current<AccessInfo.userName>>>>>, 
			OrderBy<Desc<AR.Standalone.ARRegisterAlias.refNbr>>>), Filterable = true, IsPrimaryViewCompatible = true)]
		[ARInvoiceType.Numbering()]
		[ARInvoiceNbr()]
		protected virtual void ARInvoice_RefNbr_CacheAttached(PXCache sender)
		{
		}
		[SOOpenPeriod(typeof(ARRegister.docDate))]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void ARInvoice_FinPeriodID_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<Customer.termsID, Where<Customer.bAccountID, Equal<Current<ARInvoice.customerID>>, And<Current<ARInvoice.docType>, NotEqual<ARDocType.creditMemo>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.customer>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		[SOInvoiceTerms()]
		protected virtual void ARInvoice_TermsID_CacheAttached(PXCache sender)
		{
		}
		[PXDBDate()]
		[PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void ARInvoice_DueDate_CacheAttached(PXCache sender)
		{
		}
		[PXDBDate()]
		[PXUIField(DisplayName = "Cash Discount Date", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void ARInvoice_DiscDate_CacheAttached(PXCache sender)
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(ARInvoice.origDocAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void ARInvoice_CuryOrigDocAmt_CacheAttached(PXCache sender)
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(ARInvoice.docBal), BaseCalc = false)]
		[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		protected virtual void ARInvoice_CuryDocBal_CacheAttached(PXCache sender)
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(ARInvoice.origDiscAmt))]
		[PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void ARInvoice_CuryOrigDiscAmt_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region SOAdjust
		[PXDBInt()]
		[PXDefault()]
		protected virtual void SOAdjust_CustomerID_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault()]
		protected virtual void SOAdjust_AdjdOrderType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		protected virtual void SOAdjust_AdjdOrderNbr_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
		[PXDefault()]
		protected virtual void SOAdjust_AdjgDocType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		protected virtual void SOAdjust_AdjgRefNbr_CacheAttached(PXCache sender)
		{
		}
		[PXDBCurrency(typeof(SOAdjust.adjdCuryInfoID), typeof(SOAdjust.adjAmt))]
		[PXFormula(typeof(Sub<SOAdjust.curyOrigAdjdAmt, SOAdjust.curyAdjdBilledAmt>))]
		[PXUIField(DisplayName = "Applied To Order")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void SOAdjust_CuryAdjdAmt_CacheAttached(PXCache sender)
		{
		}
		[PXDBDecimal(4)]
		[PXFormula(typeof(Sub<SOAdjust.origAdjAmt, SOAdjust.adjBilledAmt>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void SOAdjust_AdjAmt_CacheAttached(PXCache sender)
		{
		}
		[PXDBDecimal(4)]
		[PXFormula(typeof(Sub<SOAdjust.curyOrigAdjgAmt, SOAdjust.curyAdjgBilledAmt>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void SOAdjust_CuryAdjgAmt_CacheAttached(PXCache sender)
		{
		}
		[PXDBLong()]
		[PXDefault()]
		[CurrencyInfo(ModuleCode = "SO", CuryIDField = "AdjdOrigCuryID")]
		protected virtual void SOAdjust_AdjdOrigCuryInfoID_CacheAttached(PXCache sender)
		{
		}
		[PXDBLong()]
		[PXDefault()]
		[CurrencyInfo(ModuleCode = "SO", CuryIDField = "AdjgCuryID")]
		protected virtual void SOAdjust_AdjgCuryInfoID_CacheAttached(PXCache sender)
		{
		}
		[PXDBLong()]
		[PXDefault()]
		[CurrencyInfo(ModuleCode = "SO", CuryIDField = "AdjdCuryID")]
		protected virtual void SOAdjust_AdjdCuryInfoID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion

		public virtual IEnumerable ccproctran() 
		{

			Dictionary<int, CCProcTran> existsingTran = new Dictionary<int, CCProcTran>();	
			foreach (CCProcTran iTran in PXSelectReadonly<CCProcTran, 
				Where<CCProcTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
					And<CCProcTran.docType, Equal<Current<ARInvoice.docType>>>>,
				OrderBy<Desc<CCProcTran.tranNbr>>>.SelectMultiBound(this, PXView.Currents)) 
			{
				if (existsingTran.ContainsKey(iTran.TranNbr.Value)) continue;
				existsingTran[iTran.TranNbr.Value] = iTran;
				yield return iTran;
			}
			
			foreach (CCProcTran iTran1 in PXSelectReadonly2<CCProcTran, 
					InnerJoin<SOOrderShipment, On<SOOrderShipment.orderNbr, Equal<CCProcTran.origRefNbr>, 
						And<SOOrderShipment.orderType, Equal<CCProcTran.origDocType>>>>, 
					Where<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>, 
						And<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, 
						And<CCProcTran.refNbr, IsNull>>>,
					OrderBy<Desc<CCProcTran.tranNbr>>>.SelectMultiBound(this, PXView.Currents))
			{
				if (existsingTran.ContainsKey(iTran1.TranNbr.Value)) continue;
				existsingTran[iTran1.TranNbr.Value] = iTran1;
				yield return iTran1;
			}
		}
		public PXSelectReadonly<CCProcTran, Where<CCProcTran.tranNbr, Equal<Current<SOInvoice.cCAuthTranNbr>>>> ccLastTran; 

		public PXSetup<SOSetup> sosetup;
        public PXSetup<ARSetup> arsetup;
		public PXSetup<Company> Company;

		public PXSelect<SOLine2> soline;
		public PXSelect<SOMiscLine2> somiscline;
		public PXSelect<SOTax> sotax;
		public PXSelect<SOTaxTran> sotaxtran;
		public PXSelect<SOOrder> soorder;

		public PXAction<ARInvoice> hold;
		[PXUIField(DisplayName = "Hold")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<ARInvoice> creditHold;
		[PXUIField(DisplayName = "Credit Hold")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable CreditHold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<ARInvoice> flow;
		[PXUIField(DisplayName = "Flow")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Flow(PXAdapter adapter)
		{
			Save.Press();					
			return adapter.Get();
		}

		[PXUIField(DisplayName = "Release", Visible = false)]
		[PXButton()]
		public override IEnumerable Release(PXAdapter adapter)
		{
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice ardoc in adapter.Get<ARInvoice>())
			{
				if (Document.Cache.GetStatus(ardoc) == PXEntryStatus.Notchanged) Document.Cache.SetStatus(ardoc, PXEntryStatus.Updated);
				list.Add(ardoc);
			}

			skipAvalaraCallOnSave = true;
				Save.Press();

			//ARInvoice should always come last in Persisted dictionary since it does have PostInvoice OnSuccess
			PXAutomation.RemovePersisted(this, typeof(ARInvoice), new List<object>(list));

			PXLongOperation.StartOperation(this, delegate ()
			{
				PXTimeStampScope.SetRecordComesFirst(typeof(ARInvoice), true);

				List<ARRegister> listWithTax = new List<ARRegister>();
				foreach (ARInvoice ardoc in list)
				{
					if (ardoc.IsTaxValid != true && AvalaraMaint.IsExternalTax(this, ardoc.TaxZoneID))
					{
						ARInvoice doc = new ARInvoice();
						doc.DocType = ardoc.DocType;
						doc.RefNbr = ardoc.RefNbr;
						doc.OrigModule = ardoc.OrigModule;
						doc.ApplyPaymentWhenTaxAvailable = ardoc.ApplyPaymentWhenTaxAvailable;
						listWithTax.Add(ARExternalTaxCalc.Process(doc));
					}
					else
					{
						listWithTax.Add(ardoc);
					}
				}

				SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();				
				SOOrderShipmentProcess docgraph = PXGraph.CreateInstance<SOOrderShipmentProcess>();

				HashSet<object> processed = new HashSet<object>();
				try
				{
					ARDocumentRelease.ReleaseDoc(listWithTax, adapter.MassProcess, null, delegate (ARRegister ardoc, bool isAborted)
				{
						PXAutomation.RemovePersisted(ie, typeof(ARInvoice), new List<object> { ardoc });

					docgraph.Clear();
					foreach (PXResult<SOOrderShipment, SOOrder> ordershipment in docgraph.Items.View.SelectMultiBound(new object[] { ardoc }))
					{
						SOOrderShipment copy = PXCache<SOOrderShipment>.CreateCopy(ordershipment);
						SOOrder order = ordershipment;
						copy.InvoiceReleased = true;
						docgraph.Items.Update(copy);

						if (order.Completed == true && order.BilledCntr <= 1 && order.ShipmentCntr <= order.BilledCntr + order.ReleasedCntr)
						{
							foreach (SOAdjust adj in docgraph.Adjustments.Select(order.OrderType, order.OrderNbr))
							{
								SOAdjust adjcopy = PXCache<SOAdjust>.CreateCopy(adj);
								adjcopy.CuryAdjdAmt = 0m;
								adjcopy.CuryAdjgAmt = 0m;
								adjcopy.AdjAmt = 0m;
								docgraph.Adjustments.Update(adjcopy);
							}
						}
						processed.Add(ardoc);
					}

						PXAutomation.CompleteSimple(docgraph.Orders.View);
						PXAutomation.CompleteSimple(docgraph.Shipments.View);

					docgraph.Save.Press();
				});
				}
				finally
				{
				PXAutomation.StorePersisted(ie, typeof(ARInvoice), new List<object>(processed));
				}
			});
			return list;
		}

		public PXAction<ARInvoice> post;
		[PXUIField(DisplayName = "Post", Visible = false)]
		[PXButton()]
		protected virtual IEnumerable Post(PXAdapter adapter)
		{
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice order in adapter.Get<ARInvoice>())
			{
				list.Add(order);
			}

			Save.Press();

			PXLongOperation.StartOperation(this, delegate ()
			{
				SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();
				INIssueEntry ingraph = PXGraph.CreateInstance<INIssueEntry>();
				ingraph.FieldVerifying.AddHandler<INTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
				ingraph.FieldVerifying.AddHandler<INTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
				ingraph.FieldVerifying.AddHandler<INTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
				DocumentList<INRegister> inlist = new DocumentList<INRegister>(ingraph);

				bool failed = false;

				foreach (ARInvoice ardoc in list)
				{
					try
					{
						ie.PostInvoice(ingraph, ardoc, inlist);

						if (adapter.MassProcess)
						{
							PXProcessing<ARInvoice>.SetInfo(list.IndexOf(ardoc), ActionsMessages.RecordProcessed);
						}
					}
					catch (Exception ex)
					{
						if (!adapter.MassProcess)
						{
							throw;
						}
						PXProcessing<ARInvoice>.SetError(list.IndexOf(ardoc), ex);
						failed = true;
					} 
				}

				if (ie.sosetup.Current.AutoReleaseIN == true && inlist.Count > 0 && inlist[0].Hold == false)
				{
					INDocumentRelease.ReleaseDoc(inlist, false);
				}

				if (failed)
				{
					throw new PXOperationCompletedWithErrorException(ErrorMessages.SeveralItemsFailed);
				}
			});

			return adapter.Get();
		}

		//throw new PXReportRequiredException(parameters, "SO642000", "Shipment Confirmation");
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected override IEnumerable Report(PXAdapter adapter,
			[PXString(8, InputMask = "CC.CC.CC.CC")]
			string reportID
			)
		{
            List<ARInvoice> list = adapter.Get<ARInvoice>().ToList();
			if (!String.IsNullOrEmpty(reportID))
			{
				Save.Press();
				int i = 0;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				string actualRecoprtID = null;
				foreach (ARInvoice doc in list)
				{
					parameters["ARInvoice.DocType" + i.ToString()] = doc.DocType;
					parameters["ARInvoice.RefNbr" + i.ToString()] = doc.RefNbr;
					actualRecoprtID = new NotificationUtility(this).SearchReport(ARNotificationSource.Customer, customer.Current, reportID, doc.BranchID);					
					i++;
				}
				if (i > 0)
				{
					throw new PXReportRequiredException(parameters, actualRecoprtID, "Report " + actualRecoprtID);
				}
			}
			return list;
		}

		
		#region Credit Card Processing Buttons
		public PXAction<ARInvoice> captureCCPayment;
		[PXUIField(DisplayName = "Capture CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CaptureCCPayment(PXAdapter adapter)
		{
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice item in adapter.Get<ARInvoice>())
			{
				list.Add(item);
				SOInvoice ext = (SOInvoice)SODocument.View.SelectSingleBound(new object[] { item });
				if (this.captureCCPayment.GetEnabled())
				{
				CCPaymentEntry.ReleaseDelegate releaseDelegate = null;
				CCPaymentEntry.UpdateDocStateDelegate docStateDelegate = UpdateSOInvoiceState;
				CCPaymentEntry.CaptureCCPayment<SOInvoice>(ext, this.ccProcTran, releaseDelegate, docStateDelegate);
			}
			}

			return list;
		}

		public PXAction<ARInvoice> authorizeCCPayment;
		[PXUIField(DisplayName = "Authorize CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable AuthorizeCCPayment(PXAdapter adapter)
		{
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice item in adapter.Get<ARInvoice>())
			{
				list.Add(item);
				SOInvoice ext = (SOInvoice)SODocument.View.SelectSingleBound(new object[] { item });
				CCPaymentEntry.AuthorizeCCPayment<SOInvoice>(ext, ccProcTran, UpdateSOInvoiceState);
			}
			return list;
		}

		public PXAction<ARInvoice> voidCCPayment;
		[PXUIField(DisplayName = "Void CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable VoidCCPayment(PXAdapter adapter)
		{
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice item in adapter.Get<ARInvoice>())
			{
				list.Add(item);
				SOInvoice ext = (SOInvoice)SODocument.View.SelectSingleBound(new object[] { item });
				CCPaymentEntry.VoidCCPayment<SOInvoice>(ext, ccProcTran, null, UpdateSOInvoiceState);
			}
			return list;
		}

		public PXAction<ARInvoice> creditCCPayment;
		[PXUIField(DisplayName = "Refund CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CreditCCPayment(PXAdapter adapter)
		{
			foreach (ARInvoice doc in adapter.Get<ARInvoice>())
			{
				SOInvoice ext = (SOInvoice)SODocument.View.SelectSingleBound(new object[] { doc });
				string PCRefTranNbr = ext.RefTranExtNbr;
				if (String.IsNullOrEmpty(ext.RefTranExtNbr))
				{
					this.SODocument.Cache.RaiseExceptionHandling<SOOrder.refTranExtNbr>(ext, ext.RefTranExtNbr, new PXSetPropertyException(AR.Messages.ERR_PCTransactionNumberOfTheOriginalPaymentIsRequired));
				}
				else
				{
					CCPaymentEntry.CreditCCPayment<SOInvoice>(ext, PCRefTranNbr, ccProcTran, null, UpdateSOInvoiceState);
				}
			}
			return adapter.Get();
		}

		#endregion


		public SOInvoiceEntry()
			: base()
		{
			{
				SOSetup record = sosetup.Current;
			}

			Document.View = new PXView(this, false, new Select2<ARInvoice,
			LeftJoinSingleTable<Customer, On<ARInvoice.customerID, Equal<Customer.bAccountID>>>,
			Where<ARInvoice.docType, Equal<Optional<ARInvoice.docType>>,
			And<ARInvoice.origModule, Equal<BatchModule.moduleSO>,
			And<Where<Customer.bAccountID, IsNull,
			Or<Match<Customer, Current<AccessInfo.userName>>>>>>>>());

			this.Views["Document"] = Document.View;

			PXUIFieldAttribute.SetVisible<SOOrderShipment.orderType>(shipmentlist.Cache, null, true);
			PXUIFieldAttribute.SetVisible<SOOrderShipment.orderNbr>(shipmentlist.Cache, null, true);
			PXUIFieldAttribute.SetVisible<SOOrderShipment.shipmentNbr>(shipmentlist.Cache, null, true);

			PXDBLiteDefaultAttribute.SetDefaultForInsert<SOOrderShipment.invoiceNbr>(shipmentlist.Cache, null, true);
			PXDBLiteDefaultAttribute.SetDefaultForUpdate<SOOrderShipment.invoiceNbr>(shipmentlist.Cache, null, true);

			TaxAttribute.SetTaxCalc<ARTran.taxCategoryID>(Transactions.Cache, null, TaxCalc.ManualLineCalc);
			this.ccLastTran.Cache.AllowInsert = false;
			this.ccLastTran.Cache.AllowUpdate = false;
			this.ccLastTran.Cache.AllowDelete = false;
		}

		public override void Persist()
		{
			if (Document.Current != null && IsExternalTax && Document.Current.Released != true)
			{
				//Avalara - validate that valid ShipTo address can be returned - no mixed carriers, and no mixeds shipto address:
				GetToAddress(Document.Current);
			}


			CopyFreightNotesAndFilesToARTran();

            if (this.Caches[typeof(SOOrderShipment)] != null)
			{
                foreach (SOOrderShipment sos in this.Caches[typeof(SOOrderShipment)].Cached)
				{
                    if (sos.HasDetailDeleted == true)
                    {
					throw new PXException(Messages.PartialInvoice);
				}
			}
            }

			foreach (ARInvoice invoice in Document.Cache.Deleted)
			{
				foreach (SOInvoice ext in SODocument.Cache.Deleted)
				{
					if (string.Equals(ext.DocType, invoice.DocType) && string.Equals(ext.RefNbr, invoice.RefNbr) && 
						(invoice.IsCCPayment == true || ext.IsCCPayment == true) && ccProcTran.View.SelectMultiBound(new object[] { invoice, ext }).Count > 0)
					{
						ARPaymentEntry docgraph = PXGraph.CreateInstance<ARPaymentEntry>();
						docgraph.AutoPaymentApp = true;
						docgraph.arsetup.Current.HoldEntry = false;
						docgraph.arsetup.Current.RequireControlTotal = false;

						ARPayment payment = new ARPayment()
						{
							DocType = ARDocType.Payment,
							AdjDate = ext.AdjDate,
							AdjFinPeriodID = ext.AdjFinPeriodID
						};

						payment = PXCache<ARPayment>.CreateCopy(docgraph.Document.Insert(payment));
						payment.CustomerID = invoice.CustomerID;
						payment.CustomerLocationID = invoice.CustomerLocationID;
						payment.ARAccountID = invoice.ARAccountID;
						payment.ARSubID = invoice.ARSubID;

                        payment.PaymentMethodID = ext.PaymentMethodID;
                        payment.PMInstanceID = ext.PMInstanceID;
						payment.CashAccountID = ext.CashAccountID;
						payment.ExtRefNbr = ext.ExtRefNbr ?? string.Format(AR.Messages.ARAutoPaymentRefNbrFormat, payment.DocDate);
						payment.CuryOrigDocAmt = ext.CuryPaymentAmt;

						docgraph.Document.Update(payment);

						using (PXTransactionScope ts = new PXTransactionScope())
						{
							docgraph.Save.Press();

							PXDatabase.Update<CCProcTran>(
								new PXDataFieldAssign("DocType", docgraph.Document.Current.DocType),
								new PXDataFieldAssign("RefNbr", docgraph.Document.Current.RefNbr),
								new PXDataFieldRestrict("DocType", PXDbType.Char, 3, ext.DocType, PXComp.EQ),
								new PXDataFieldRestrict("RefNbr", PXDbType.NVarChar, 15, ext.RefNbr, PXComp.EQ)
								);

							int i = 0;
							bool ccproctranupdated = false;

							foreach (SOOrderShipment order in PXSelect<SOOrderShipment,
								Where<SOOrderShipment.invoiceType, Equal<Required<SOOrderShipment.invoiceType>>,
								And<SOOrderShipment.invoiceNbr, Equal<Required<SOOrderShipment.invoiceNbr>>>>>.Select(docgraph, ext.DocType, ext.RefNbr))
							{
								ccproctranupdated |= PXDatabase.Update<CCProcTran>(
									new PXDataFieldAssign("DocType", docgraph.Document.Current.DocType),
									new PXDataFieldAssign("RefNbr", docgraph.Document.Current.RefNbr),
									new PXDataFieldRestrict("OrigDocType", PXDbType.Char, 3, order.OrderType, PXComp.EQ),
									new PXDataFieldRestrict("OrigRefNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ),
									new PXDataFieldRestrict("RefNbr", PXDbType.NVarChar, 15, null, PXComp.ISNULL)
									);

								if (ccproctranupdated && i > 0)
								{
									throw new PXException(AR.Messages.ERR_CCMultiplyPreauthCombined);
								}

								i++;
							}

							docgraph.ccProcTran.View.Clear();
							docgraph.Document.Cache.RaiseRowSelected(docgraph.Document.Current);

							PXFieldState voidState;
							if ((voidState = (PXFieldState)docgraph.voidCheck.GetState(Document.Current)) == null || voidState.Enabled == false)
							{
								throw new PXException(AR.Messages.ERR_CCTransactionMustBeVoided);
							}

							List<object> tovoid = new List<object>();
							tovoid.Add(docgraph.Document.Current);

							foreach (object item in docgraph.voidCheck.Press(new PXAdapter(new DummyView(docgraph, docgraph.Document.View.BqlSelect, tovoid)))) {; }

							base.Persist();

							ts.Complete();
						}

						return;
					}
				}
			}

			foreach (ARAdjust2 adj in Adjustments.Cache.Inserted)
			{
				if (adj.CuryAdjdAmt == 0m)
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.InsertedDeleted);
				}
			}

			foreach (ARAdjust2 adj in Adjustments.Cache.Updated
				.RowCast<ARAdjust2>()
				.Where(adj => adj.CuryAdjdAmt == 0m))
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.Deleted);
				}

			foreach (ARInvoice ardoc in Document.Cache.Cached
				.Cast<ARInvoice>()
				.Where(ardoc => (Document.Cache.GetStatus(ardoc) == PXEntryStatus.Inserted
						|| Document.Cache.GetStatus(ardoc) == PXEntryStatus.Updated)
					&& ardoc.DocType == ARDocType.Invoice
					&& ardoc.Released == false
					&& ardoc.ApplyPaymentWhenTaxAvailable != true)
)
				{
					SOInvoice ext = SODocument.Select(ardoc.DocType, ardoc.RefNbr);

					if (ardoc.CuryDocBal - ardoc.CuryOrigDiscAmt - ext.CuryPaymentTotal - ext.CuryCCCapturedAmt < 0m)
					{
					foreach (ARAdjust2 adj in Adjustments_Inv.View
						.SelectMultiBound(new object[] { ardoc })
						.RowCast<ARAdjust2>().Where(adj => Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Updated 
							|| Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Inserted 
							|| ((decimal?)Document.Cache.GetValueOriginal<ARInvoice.curyDocBal>(ardoc) != ardoc.CuryDocBal)))
						{
						Adjustments.Cache.SmartSetStatus(adj, PXEntryStatus.Updated);
							Adjustments.Cache.RaiseExceptionHandling<ARAdjust2.curyAdjdAmt>(adj, adj.CuryAdjdAmt, new PXSetPropertyException(AR.Messages.Application_Amount_Cannot_Exceed_Document_Amount));
							throw new PXException(AR.Messages.Application_Amount_Cannot_Exceed_Document_Amount);
						}
					}
				}

			base.Persist();
		}

		public override void RecalcUnbilledTax()
		{
			Dictionary<string, KeyValuePair<string, string>> orders =
																new Dictionary<string, KeyValuePair<string, string>>();

			foreach (ARTran line in Transactions.Select())
			{
				string key = string.Format("{0}.{1}", line.SOOrderType, line.SOOrderNbr);
				if (!orders.ContainsKey(key))
				{
					orders.Add(key, new KeyValuePair<string, string>(line.SOOrderType, line.SOOrderNbr));
				}

			}


			SOOrderEntry soOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();
			soOrderEntry.RowSelecting.RemoveHandler<SOOrder>(soOrderEntry.SOOrder_RowSelecting);
			foreach (KeyValuePair<string, string> kv in orders.Values)
			{
				soOrderEntry.Clear(PXClearOption.ClearAll);
				soOrderEntry.Document.Current = soOrderEntry.Document.Search<SOOrder.orderNbr>(kv.Value, kv.Key);
				soOrderEntry.CalculateAvalaraTax(soOrderEntry.Document.Current);
				soOrderEntry.Persist();
			}
		}

		protected virtual void SOLine2_BaseShippedQty_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				e.FieldName = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine2_ShippedQty_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				e.FieldName = string.Empty;
				e.Cancel = true;
			}
        }

		protected override void ARInvoice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				PXResult<SOOrderShipment, SOOrderType> sotype =
					(PXResult<SOOrderShipment, SOOrderType>)
					PXSelectJoin<SOOrderShipment, 
					InnerJoin<SOOrderType, 
								 On<SOOrderType.orderType, Equal<SOOrderShipment.orderType>>>, 
					Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, 
						And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>>
						.SelectSingleBound(this, new object[] { e.Row });

				if (sotype != null)
				{
					if (string.IsNullOrEmpty(((ARInvoice)e.Row).RefNbr) && ((SOOrderType)sotype).UserInvoiceNumbering == true)
					{
						throw new PXException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<SOOrder.invoiceNbr>(soorder.Cache));
					}

					if (((SOOrderType)sotype).MarkInvoicePrinted == true)
					{
						((ARInvoice)e.Row).Printed = true;
					}

					if (((SOOrderType)sotype).MarkInvoiceEmailed == true)
					{
						((ARInvoice)e.Row).Emailed = true;
					}

					AutoNumberAttribute.SetNumberingId<ARInvoice.refNbr>(Document.Cache, ((SOOrderType)sotype).ARDocType, ((SOOrderType)sotype).InvoiceNumberingID);
				}
			}

			base.ARInvoice_RowPersisting(sender, e);
		}

		protected virtual void ARInvoice_OrigModule_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = GL.BatchModule.SO;
			e.Cancel = true;
		}

		protected override void ARInvoice_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			base.ARInvoice_RowInserted(sender, e);

			SODocument.Cache.Insert();
			SODocument.Cache.IsDirty = false;

			SODocument.Current.AdjDate = ((ARInvoice)e.Row).DocDate;
			SODocument.Current.AdjFinPeriodID = ((ARInvoice)e.Row).FinPeriodID;
			SODocument.Current.AdjTranPeriodID = ((ARInvoice)e.Row).TranPeriodID;
			SODocument.Current.NoteID = ((ARInvoice)e.Row).NoteID;

		}

        protected override void ARTran_CuryUnitPrice_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if(!cancelUnitPriceCalculation)
                base.ARTran_CuryUnitPrice_FieldVerifying(sender, e);
        }

		protected override void ARInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARSetup.Current.RequireControlTotal = (((ARInvoice)e.Row).DocType == ARDocType.CashSale || ((ARInvoice)e.Row).DocType == ARDocType.CashReturn) ? true : ARSetup.Current.RequireControlTotal;

			base.ARInvoice_RowUpdated(sender, e);

			ARInvoice doc = e.Row as ARInvoice;
			if ((doc.DocType == ARDocType.CashSale || doc.DocType == ARDocType.CashReturn) && doc.Released != true)
			{
				if (sender.ObjectsEqual<ARInvoice.curyDocBal, ARInvoice.curyOrigDiscAmt>(e.Row, e.OldRow) == false && doc.CuryDocBal - doc.CuryOrigDiscAmt != doc.CuryOrigDocAmt)
				{
					if (doc.CuryDocBal != null && doc.CuryOrigDiscAmt != null && doc.CuryDocBal != 0)
						sender.SetValueExt<ARInvoice.curyOrigDocAmt>(doc, doc.CuryDocBal - doc.CuryOrigDiscAmt);
					else
						sender.SetValueExt<ARInvoice.curyOrigDocAmt>(doc, 0m);
				}
				else if (sender.ObjectsEqual<ARInvoice.curyOrigDocAmt>(e.Row, e.OldRow) == false)
				{
					if (doc.CuryDocBal != null && doc.CuryOrigDocAmt != null && doc.CuryDocBal != 0)
						sender.SetValueExt<ARInvoice.curyOrigDiscAmt>(doc, doc.CuryDocBal - doc.CuryOrigDocAmt);
					else
						sender.SetValueExt<ARInvoice.curyOrigDiscAmt>(doc, 0m);
				}
			}

			if (doc != null && doc.CuryDocBal != null && doc.Hold != true && doc.CuryDocBal < 0m && SODocument.Current != null && SODocument.Current.CuryPremiumFreightAmt < 0m && (doc.CuryDocBal - SODocument.Current.CuryPremiumFreightAmt) >= 0m)
			{
				sender.RaiseExceptionHandling<ARInvoice.curyDocBal>(doc, doc.CuryDocBal,
					new PXSetPropertyException(Messages.DocumentBalanceNegativePremiumFreight));
			}

			if ((doc.DocType == ARDocType.CashSale || doc.DocType == ARDocType.CashReturn) && doc.Released != true && doc.Hold != true)
			{
				if (doc.CuryDocBal < doc.CuryOrigDocAmt)
				{
					sender.RaiseExceptionHandling<ARInvoice.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, new PXSetPropertyException(AR.Messages.CashSaleOutOfBalance));
				}
				else
				{
					sender.RaiseExceptionHandling<ARInvoice.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, null);
				}
			}

			if (!sender.ObjectsEqual<ARInvoice.customerID, ARInvoice.docDate, ARInvoice.finPeriodID, ARInvoice.curyTaxTotal, ARInvoice.curyOrigDocAmt, ARInvoice.docDesc, ARInvoice.curyOrigDiscAmt, ARInvoice.hold>(e.Row, e.OldRow))
			{
				SOInvoice invoice = (SOInvoice)SODocument.Select();
				if (IsImport && invoice == null && SODocument.Current != null && sender.Current is ARInvoice)
				{
					if ((((ARInvoice)sender.Current).DocType != SODocument.Current.DocType
						|| ((ARInvoice)sender.Current).RefNbr != SODocument.Current.RefNbr)
						&& SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Inserted
						&& sender.Locate(new ARInvoice { DocType = SODocument.Current.DocType, RefNbr = SODocument.Current.RefNbr }) == null)
					{
						SODocument.Cache.Delete(SODocument.Current);
					}
				}
				
				SODocument.Current = invoice ?? (SOInvoice)SODocument.Cache.Insert();
				SODocument.Current.CustomerID = ((ARInvoice)e.Row).CustomerID;

                if ((((ARInvoice)e.Row).DocType == ARDocType.CashSale 
                        || ((ARInvoice)e.Row).DocType == ARDocType.CashReturn 
                        || ((ARInvoice)e.Row).DocType == ARDocType.Invoice) && !sender.ObjectsEqual<ARInvoice.customerID>(e.Row, e.OldRow))
				{
                    SODocument.Cache.SetDefaultExt<SOInvoice.paymentMethodID>(SODocument.Current);
					SODocument.Cache.SetDefaultExt<SOInvoice.pMInstanceID>(SODocument.Current);
				}

				SODocument.Current.AdjDate = ((ARInvoice)e.Row).DocDate;
				SODocument.Current.DepositAfter = ((ARInvoice)e.Row).DocDate;
				SODocument.Current.AdjFinPeriodID = ((ARInvoice)e.Row).FinPeriodID;
				SODocument.Current.AdjTranPeriodID = ((ARInvoice)e.Row).TranPeriodID;
				SODocument.Current.CuryTaxTotal = ((ARInvoice)e.Row).CuryTaxTotal;
				SODocument.Current.TaxTotal = ((ARInvoice)e.Row).TaxTotal;
				SODocument.Current.CuryPaymentAmt = ((ARInvoice)e.Row).CuryOrigDocAmt - ((ARInvoice)e.Row).CuryOrigDiscAmt - SODocument.Current.CuryPaymentTotal;
				SODocument.Current.DocDesc = ((ARInvoice)e.Row).DocDesc;
				SODocument.Current.PaymentProjectID = PM.ProjectDefaultAttribute.NonProject(this);
				SODocument.Current.Hold = ((ARInvoice)e.Row).Hold;

				if (SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Notchanged)
				{
					SODocument.Cache.SetStatus(SODocument.Current, PXEntryStatus.Updated);
				}
			}
        }

		protected override void ARInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			base.ARInvoice_RowSelected(cache, e);

			PXUIFieldAttribute.SetVisible<ARInvoice.projectID>(cache, e.Row, PM.ProjectAttribute.IsPMVisible(this, BatchModule.SO) || PM.ProjectAttribute.IsPMVisible(this, BatchModule.AR) || PXAccess.FeatureInstalled<FeaturesSet.contractManagement>());
			PXUIFieldAttribute.SetVisible<ARTran.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.SO) || PM.ProjectAttribute.IsPMVisible(this, BatchModule.AR));

			selectShipment.SetEnabled(Document.AllowDelete);

			if (e.Row == null)
				return;

			ARInvoice doc = e.Row as ARInvoice;
			if (((ARInvoice)e.Row).DocType == ARDocType.CashSale || ((ARInvoice)e.Row).DocType == ARDocType.CashReturn)
			{
				PXUIFieldAttribute.SetVisible<ARInvoice.curyOrigDocAmt>(cache, e.Row);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyOrigDocAmt>(cache, e.Row, ((ARInvoice)e.Row).Released == false);
			}

			SODocument.Cache.AllowUpdate = Document.Cache.AllowUpdate;
			FreightDetails.Cache.AllowUpdate = Document.Cache.AllowUpdate;

			CCProcTran lastCCran;
			CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select(), out lastCCran);

			bool isCCCaptured = (ccPaymentState & CCPaymentState.Captured) != 0;
			bool isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
				
			bool isAuthorizedCashSale = (doc.DocType == ARDocType.CashSale && (isCCPreAuthorized || isCCCaptured));
			bool isRefundedCashReturn = doc.DocType == ARDocType.CashReturn && (ccPaymentState & CCPaymentState.Refunded) != 0;
			Transactions.Cache.AllowDelete = Transactions.Cache.AllowDelete && !isAuthorizedCashSale && !isRefundedCashReturn;
			Transactions.Cache.AllowUpdate = Transactions.Cache.AllowUpdate && !isAuthorizedCashSale && !isRefundedCashReturn;
			Transactions.Cache.AllowInsert = Transactions.Cache.AllowInsert && !isAuthorizedCashSale && !isRefundedCashReturn;
			PXUIFieldAttribute.SetEnabled<ARInvoice.curyOrigDocAmt>(cache, doc, !isAuthorizedCashSale && !isRefundedCashReturn);
            PXUIFieldAttribute.SetEnabled<ARInvoice.curyOrigDiscAmt>(cache, doc, !isAuthorizedCashSale && !isRefundedCashReturn);

			release.SetEnabled(arsetup.Current.IntegratedCCProcessing != true || doc.DocType != ARDocType.CashReturn || (ccPaymentState & CCPaymentState.Refunded) != 0);

			Adjustments.Cache.AllowSelect = 
				doc.DocType != ARDocType.CashSale && 
				doc.DocType != ARDocType.CashReturn;
		}

        public override void SetDocTypeList(PXCache cache, PXRowSelectedEventArgs e)
        {
            //doctype list should not be updated in SOInvoiceEntry
        }

		protected virtual void ARInvoice_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			ARInvoice row = (ARInvoice)e.Row;
			
			if (row != null && e.IsReadOnly == false
				&& String.IsNullOrEmpty(row.DocType) == false && String.IsNullOrEmpty(row.RefNbr) == false)
			{
				row.IsCCPayment = false;
				using (new PXConnectionScope())
				{
					PXResult<CustomerPaymentMethodC, CA.PaymentMethod, SOInvoice> pmInstance = (PXResult<CustomerPaymentMethodC, CA.PaymentMethod, SOInvoice>)
										 PXSelectJoin<CustomerPaymentMethodC,
										InnerJoin<CA.PaymentMethod,
											On<CA.PaymentMethod.paymentMethodID, Equal<CustomerPaymentMethodC.paymentMethodID>>,
																			InnerJoin<SOInvoice, On<SOInvoice.pMInstanceID, Equal<CustomerPaymentMethodC.pMInstanceID>>>>,
									Where<SOInvoice.docType, Equal<Required<SOInvoice.docType>>,
										And<SOInvoice.refNbr, Equal<Required<SOInvoice.refNbr>>,
										And<CA.PaymentMethod.paymentType, Equal<CA.PaymentMethodType.creditCard>,
											And<CA.PaymentMethod.aRIsProcessingRequired, Equal<True>>>>>>.Select(this, row.DocType, row.RefNbr);
					if (pmInstance != null)
					{
						row.IsCCPayment = true;
					}
				}
			}
		}


		protected override void ARInvoice_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			string CreditRule = customer.Current.CreditRule;
			try
			{
				base.ARInvoice_CustomerID_FieldUpdated(sender, e);
			}
			finally
			{
				customer.Current.CreditRule = CreditRule;
			}
		}

		protected virtual void SOInvoice_RowSelecting(PXCache sender, PXRowSelectingEventArgs e) 
		{
			if (e.Row != null && e.IsReadOnly == false && ((SOInvoice)e.Row).CuryPaymentTotal == null)
			{
				using (new PXConnectionScope())
				{
					bool IsReadOnly = (sender.GetStatus(e.Row) == PXEntryStatus.Notchanged);
					PXFormulaAttribute.CalcAggregate<ARAdjust2.curyAdjdAmt>(Adjustments.Cache, e.Row, IsReadOnly);
					sender.RaiseFieldUpdated<SOInvoice.curyPaymentTotal>(e.Row, null);

					PXDBCurrencyAttribute.CalcBaseValues<SOInvoice.curyPaymentTotal>(sender, e.Row);
				}
			}
		}

		protected virtual void SOInvoice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            SOInvoice doc = (SOInvoice)e.Row;
			ARInvoice arDoc = this.Document.Current;

			bool docNotOnHold = (arDoc != null) && (arDoc.OpenDoc == true && arDoc.Released == false); //It's always  on Hold ???
			bool enableCCProcess = false;
			bool docTypePayment = (doc.DocType == ARDocType.Invoice || doc.DocType == ARDocType.CashSale);
			bool isCashReturn = doc.DocType == ARDocType.CashReturn;

			doc.IsCCPayment = false;
			doc.PaymentProjectID = PM.ProjectDefaultAttribute.NonProject(this);
			if (doc.PMInstanceID != null)
			{
				PXResult<CustomerPaymentMethodC, CA.PaymentMethod> pmInstance = (PXResult<CustomerPaymentMethodC, CA.PaymentMethod>)
					               PXSelectJoin<CustomerPaymentMethodC,
									InnerJoin<CA.PaymentMethod,
										On<CA.PaymentMethod.paymentMethodID, Equal<CustomerPaymentMethodC.paymentMethodID>>>,
								Where<CustomerPaymentMethodC.pMInstanceID, Equal<Optional<SOInvoice.pMInstanceID>>,
									And<CA.PaymentMethod.paymentType, Equal<CA.PaymentMethodType.creditCard>,
										And<CA.PaymentMethod.aRIsProcessingRequired, Equal<True>>>>>.Select(this, doc.PMInstanceID);
				if (pmInstance != null)
				{
					doc.IsCCPayment = true;
					enableCCProcess = (isCashReturn || docTypePayment || doc.DocType == ARDocType.Refund || doc.DocType == ARDocType.VoidPayment);
				}
			}
			if (arDoc != null)
			arDoc.IsCCPayment = doc.IsCCPayment; 
			bool isCCVoided = false;
			bool isCCCaptured = false;
			bool isCCPreAuthorized = false;
			bool isCCRefunded = false;
			bool isCCVoidingAttempted = false; //Special flag for VoidPayment Release logic 

			if (enableCCProcess)
			{
				CCProcTran lastCCran;
				CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select(), out lastCCran);
				isCCVoided = (ccPaymentState & CCPaymentState.Voided) != 0;
				isCCCaptured = (ccPaymentState & CCPaymentState.Captured) != 0;
				isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
				isCCRefunded = (ccPaymentState & CCPaymentState.Refunded) != 0;
				isCCVoidingAttempted = (ccPaymentState & CCPaymentState.VoidFailed) != 0;
				doc.CCPaymentStateDescr = CCPaymentEntry.FormatCCPaymentState(ccPaymentState);
				doc.CCAuthTranNbr = lastCCran != null ? lastCCran.TranNbr : null; 
			}

			decimal? docBalance = ((ICCPayment)doc).CuryDocBal;
			bool isNotNullAmt = docBalance != null && docBalance > 0;

			bool canAuthorize = docNotOnHold && docTypePayment && !(isCCPreAuthorized || isCCCaptured) && isNotNullAmt;
			bool canCapture = docNotOnHold && docTypePayment && !(isCCCaptured) && isNotNullAmt;
			bool canVoid = docNotOnHold && ((isCCCaptured  || isCCPreAuthorized) && docTypePayment);
			
			this.authorizeCCPayment.SetEnabled(enableCCProcess && canAuthorize);
			this.captureCCPayment.SetEnabled(enableCCProcess && canCapture);
			this.voidCCPayment.SetEnabled(enableCCProcess && canVoid);
			this.creditCCPayment.SetEnabled(enableCCProcess && isCashReturn && !isCCRefunded && doc.RefTranExtNbr != null);
			PXUIFieldAttribute.SetEnabled(this.ccLastTran.Cache, null, false);

			//PXUIFieldAttribute.SetEnabled<SOInvoice.cCAuthExpirationDate>(sender, doc, false);
			//PXUIFieldAttribute.SetEnabled<SOInvoice.curyCCPreAuthAmount>(sender, doc, false);

			PXUIFieldAttribute.SetEnabled<SOInvoice.refTranExtNbr>(sender, doc, doc.PMInstanceID.HasValue && doc.IsCCPayment == true && isCashReturn && !isCCRefunded);
			PXUIFieldAttribute.SetVisible<SOInvoice.cCPaymentStateDescr>(sender, doc, enableCCProcess);
			PXUIFieldAttribute.SetEnabled<SOInvoice.curyDiscTot>(SODocument.Cache, e.Row, Document.Cache.AllowUpdate);
            bool allowPaymentInfo = Document.Cache.AllowUpdate && (((SOInvoice)e.Row).DocType == ARDocType.CashSale || ((SOInvoice)e.Row).DocType == ARDocType.CashReturn || ((SOInvoice)e.Row).DocType == ARDocType.Invoice)
                                        && !isCCPreAuthorized && !isCCCaptured;
            bool isPMInstanceRequired = false;
            if (allowPaymentInfo && (String.IsNullOrEmpty(doc.PaymentMethodID) == false))
            {
                CA.PaymentMethod pm = PXSelect<CA.PaymentMethod, Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
                isPMInstanceRequired = (pm.IsAccountNumberRequired == true);
            }
            PXUIFieldAttribute.SetEnabled<SOInvoice.paymentMethodID>(SODocument.Cache, e.Row, allowPaymentInfo);
			PXUIFieldAttribute.SetEnabled<SOInvoice.pMInstanceID>(SODocument.Cache, e.Row, allowPaymentInfo && isPMInstanceRequired);
			PXUIFieldAttribute.SetEnabled<SOInvoice.cashAccountID>(SODocument.Cache, e.Row, Document.Cache.AllowUpdate && (((SOInvoice)e.Row).PMInstanceID != null || string.IsNullOrEmpty(doc.PaymentMethodID) == false));
			PXUIFieldAttribute.SetEnabled<SOInvoice.extRefNbr>(SODocument.Cache, e.Row, Document.Cache.AllowUpdate && (((SOInvoice)e.Row).PMInstanceID != null || string.IsNullOrEmpty(doc.PaymentMethodID) == false));
			PXUIFieldAttribute.SetEnabled<SOInvoice.cleared>(SODocument.Cache, e.Row, Document.Cache.AllowUpdate && (((SOInvoice)e.Row).PMInstanceID != null || string.IsNullOrEmpty(doc.PaymentMethodID) == false) && (((SOInvoice)e.Row).DocType == ARDocType.CashSale || ((SOInvoice)e.Row).DocType == ARDocType.CashReturn));
			PXUIFieldAttribute.SetEnabled<SOInvoice.clearDate>(SODocument.Cache, e.Row, Document.Cache.AllowUpdate && (((SOInvoice)e.Row).PMInstanceID != null || string.IsNullOrEmpty(doc.PaymentMethodID) == false) && (((SOInvoice)e.Row).DocType == ARDocType.CashSale || ((SOInvoice)e.Row).DocType == ARDocType.CashReturn));

			if (IsExternalTax == true && ((SOInvoice)e.Row).IsTaxValid != true)
				PXUIFieldAttribute.SetWarning<SOInvoice.curyTaxTotal>(sender, e.Row, AR.Messages.TaxIsNotUptodate);

            if (Document.Current != null)
            {
                DiscountDetails.Cache.AllowDelete = false;
                DiscountDetails.Cache.AllowUpdate = false;
                DiscountDetails.Cache.AllowInsert = false;
            }
            else
            {
                DiscountDetails.Cache.AllowDelete = Transactions.Cache.AllowDelete;
                DiscountDetails.Cache.AllowUpdate = Transactions.Cache.AllowUpdate;
                DiscountDetails.Cache.AllowInsert = Transactions.Cache.AllowInsert;
            }

			bool isAuthorizedCashSale = (doc.DocType == ARDocType.CashSale && (isCCPreAuthorized || isCCCaptured));
			PXUIFieldAttribute.SetEnabled<SOInvoice.curyDiscTot>(sender, doc, !isAuthorizedCashSale);
		}

        protected virtual void SOInvoice_PaymentMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetDefaultExt<SOInvoice.pMInstanceID>(e.Row);
            sender.SetDefaultExt<SOInvoice.cashAccountID>(e.Row);
            sender.SetDefaultExt<SOInvoice.isCCCaptureFailed>(e.Row);
        }

		protected virtual void SOInvoice_PMInstanceID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			//sender.SetDefaultExt<SOInvoice.paymentMethodID>(e.Row);
			sender.SetDefaultExt<SOInvoice.cashAccountID>(e.Row);
			sender.SetDefaultExt<SOInvoice.isCCCaptureFailed>(e.Row);
			sender.SetValueExt<SOInvoice.refTranExtNbr>(e.Row, null);
		}

		protected virtual void SOInvoice_CuryDiscTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOInvoice row = e.Row as SOInvoice;
			if (row == null) return;

			if (IsExternalTax == true)
			{
				row.IsTaxValid = false;
			}
		}


		protected virtual void SOInvoice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				SOInvoice doc = (SOInvoice)e.Row;

				if ((doc.DocType == ARDocType.CashSale || doc.DocType == ARDocType.CashReturn))
				{
                    if (String.IsNullOrEmpty(doc.PaymentMethodID) == true)
                    {
                        if (sender.RaiseExceptionHandling<SOInvoice.pMInstanceID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOInvoice.pMInstanceID).Name)))
                        {
                            throw new PXRowPersistingException(typeof(SOInvoice.pMInstanceID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOInvoice.pMInstanceID).Name);
                        }
                    }
                    else
                    {
                        
                        CA.PaymentMethod pm = PXSelect<CA.PaymentMethod, Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
                        bool pmInstanceRequired = (pm.IsAccountNumberRequired == true);
                        if (pmInstanceRequired && doc.PMInstanceID == null)
                        {
                            if (sender.RaiseExceptionHandling<SOInvoice.pMInstanceID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOInvoice.pMInstanceID).Name)))
                            {
                                throw new PXRowPersistingException(typeof(SOInvoice.pMInstanceID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOInvoice.pMInstanceID).Name);
                            }
                        }
                    }
				}

				bool isCashSale = (doc.DocType == AR.ARDocType.CashSale) || (doc.DocType == AR.ARDocType.CashReturn);
                if (isCashSale && SODocument.GetValueExt<SOInvoice.cashAccountID>((SOInvoice)e.Row) == null)
				{
					if (sender.RaiseExceptionHandling<SOInvoice.cashAccountID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOInvoice.cashAccountID).Name)))
					{
						throw new PXRowPersistingException(typeof(SOInvoice.cashAccountID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOInvoice.cashAccountID).Name);
					}
				}

				object acctcd;

				if ((acctcd = SODocument.GetValueExt<SOInvoice.cashAccountID>((SOInvoice)e.Row)) != null && sender.GetValue<SOInvoice.cashAccountID>(e.Row) == null)
				{
					sender.RaiseExceptionHandling<SOInvoice.cashAccountID>(e.Row, null, null);
					sender.SetValueExt<SOInvoice.cashAccountID>(e.Row, acctcd is PXFieldState ? ((PXFieldState)acctcd).Value : acctcd);
				}

                if ((doc.CuryDiscTot ?? 0m) > (Math.Abs((doc.CuryLineTotal ?? 0m) + (doc.CuryMiscTot ?? 0m))))
                {
                    if (sender.RaiseExceptionHandling<SOInvoice.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(AR.Messages.DiscountGreaterLineMiscTotal, PXErrorLevel.Error)))
                    {
                        throw new PXRowPersistingException(typeof(SOInvoice.curyDiscTot).Name, null, AR.Messages.DiscountGreaterLineMiscTotal);
                    }
                }

				//if (doc.PMInstanceID != null && string.IsNullOrEmpty(doc.ExtRefNbr))
				//{
				//    if (sender.RaiseExceptionHandling<SOInvoice.extRefNbr>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOInvoice.extRefNbr).Name)))
				//    {
				//        throw new PXRowPersistingException(typeof(SOInvoice.extRefNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOInvoice.extRefNbr).Name);
				//    }
				//}
			}
		}

		protected virtual void SOInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<SOInvoice.curyDiscTot>(e.OldRow, e.Row))
			{
                AddDiscount(sender, e);
                if (e.ExternalCall) DiscountEngine<ARTran>.CalculateDocumentDiscountRate<SOInvoiceDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, (SODocument.Current.CuryLineTotal ?? 0m + SODocument.Current.CuryMiscTot ?? 0m), SODocument.Current.CuryDiscTot ?? 0m);
			}

			if (!sender.ObjectsEqual<SOInvoice.isCCCaptured>(e.Row, e.OldRow) && ((SOInvoice)e.Row).IsCCCaptured == true)
			{
				ARInvoice copy = (ARInvoice)Document.Cache.CreateCopy(Document.Current);

				copy.CreditHold = false;

				Document.Cache.Update(copy);
			}

			if (!sender.ObjectsEqual<SOInvoice.curyPaymentTotal>(e.OldRow, e.Row))
			{
				ARInvoice ardoc = Document.Search<ARInvoice.refNbr>(((SOInvoice)e.Row).RefNbr, ((SOInvoice)e.Row).DocType);
				//is null on delete operation
				if (ardoc != null)
				{
					((SOInvoice)e.Row).CuryPaymentAmt = ardoc.CuryOrigDocAmt - ardoc.CuryOrigDiscAmt - ((SOInvoice)e.Row).CuryPaymentTotal;
				}
			}

			if (!sender.ObjectsEqual<SOInvoice.pMInstanceID, SOInvoice.paymentMethodID, SOInvoice.cashAccountID>(e.Row, e.OldRow))
			{ 
				ARInvoice ardoc = Document.Search<ARInvoice.refNbr>(((SOInvoice)e.Row).RefNbr, ((SOInvoice)e.Row).DocType);
				//is null on delete operation
				if (ardoc != null)
				{
					ardoc.PMInstanceID = ((SOInvoice)e.Row).PMInstanceID;
					ardoc.PaymentMethodID = ((SOInvoice)e.Row).PaymentMethodID;
					ardoc.CashAccountID = ((SOInvoice)e.Row).CashAccountID;
					
					if (Document.Cache.GetStatus(ardoc) == PXEntryStatus.Notchanged)
					{
						Document.Cache.SetStatus(ardoc, PXEntryStatus.Updated);
					}
				}
			}
		}

		protected override void ARAdjust2_CuryAdjdAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARAdjust2 adj = (ARAdjust2)e.Row;
			Terms terms = PXSelect<Terms, Where<Terms.termsID, Equal<Current<ARInvoice.termsID>>>>.Select(this);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single && (decimal)e.NewValue > 0m)
			{
				throw new PXSetPropertyException(AR.Messages.PrepaymentAppliedToMultiplyInstallments);
			}

			if (adj.CuryDocBal == null)
			{
				PXResult<ARPayment, CurrencyInfo> res = (PXResult<ARPayment, CurrencyInfo>)PXSelectReadonly2<ARPayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>, And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);

				ARPayment payment = PXCache<ARPayment>.CreateCopy(res);
				CurrencyInfo pay_info = (CurrencyInfo)res;
				CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this);

				ARAdjust2 other = PXSelectGroupBy<ARAdjust2, Where<ARAdjust2.adjgDocType, Equal<Required<ARAdjust2.adjgDocType>>, And<ARAdjust2.adjgRefNbr, Equal<Required<ARAdjust2.adjgRefNbr>>, And<ARAdjust2.released, Equal<False>, And<Where<ARAdjust2.adjdDocType, NotEqual<Required<ARAdjust2.adjdDocType>>, Or<ARAdjust2.adjdRefNbr, NotEqual<Required<ARAdjust2.adjdRefNbr>>>>>>>>, Aggregate<GroupBy<ARAdjust2.adjgDocType, GroupBy<ARAdjust2.adjgRefNbr, Sum<ARAdjust2.curyAdjgAmt, Sum<ARAdjust2.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdDocType, adj.AdjdRefNbr);
				if (other != null && other.AdjdRefNbr != null)
				{
					payment.CuryDocBal -= other.CuryAdjgAmt;
					payment.DocBal -= other.AdjAmt;
				}

				decimal CuryDocBal;
				if (string.Equals(pay_info.CuryID, inv_info.CuryID))
				{
					CuryDocBal = (decimal)payment.CuryDocBal;
				}
				else
				{
					PXDBCurrencyAttribute.CuryConvCury(sender, inv_info, (decimal)payment.DocBal, out CuryDocBal);
				}

				adj.CuryDocBal = CuryDocBal - adj.CuryAdjdAmt;
			}

			if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt - (decimal)e.NewValue < 0)
			{
				throw new PXSetPropertyException(AR.Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt).ToString());
			}
		}

        protected override void ARTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            if (e.ExternalCall)
                RecalculateDiscounts(sender, (ARTran)e.Row);
            TaxAttribute.Calculate<ARTran.taxCategoryID>(sender, e);

            if (SODocument.Current != null)
            {
                SODocument.Current.IsTaxValid = false;
                if (SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Notchanged)
                    SODocument.Cache.SetStatus(SODocument.Current, PXEntryStatus.Updated);
            }

            if (Document.Current != null)
            {
                Document.Current.IsTaxValid = false;
                if (SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Notchanged)
                    SODocument.Cache.SetStatus(SODocument.Current, PXEntryStatus.Updated);
            }
        }

		protected override void ARTran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			base.ARTran_RowDeleted(sender, e);
			if (((ARTran)e.Row).LineType == SOLineType.Freight)
				return;


            if (((ARTran)e.Row).InventoryID != null)
            {
                if (((ARTran)e.Row).LineType != SOLineType.Discount)
					DiscountEngine<ARTran>.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate);
                RecalculateTotalDiscount();
            }

			PXResultset<ARTran> siblings = PXSelect<ARTran, Where<ARTran.sOOrderType, Equal<Required<ARTran.sOOrderType>>,
				And<ARTran.sOOrderNbr, Equal<Required<ARTran.sOOrderNbr>>,
				And<ARTran.sOShipmentType, Equal<Required<ARTran.sOShipmentType>>,
				And<ARTran.sOShipmentNbr, Equal<Required<ARTran.sOShipmentNbr>>,
				And<ARTran.tranType, Equal<Required<ARTran.tranType>>,
				And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>>>>>>.SelectWindowed(this, 0, 2,
				((ARTran)e.Row).SOOrderType, ((ARTran)e.Row).SOOrderNbr, ((ARTran)e.Row).SOShipmentType, ((ARTran)e.Row).SOShipmentNbr, ((ARTran)e.Row).TranType, ((ARTran)e.Row).RefNbr);

			if (siblings.Count == 1 && ((ARTran)siblings).LineType == SOLineType.Freight)
			{
				Freight.Delete((ARTran)siblings);
				siblings.Clear();
			}

            SOOrderShipment ordershipment =
             PXSelect<SOOrderShipment, Where<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>,
					And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>,
					And<SOOrderShipment.shipmentType, Equal<Required<SOOrderShipment.shipmentType>>,
					And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>>>>>>.SelectWindowed(this, 0, 1,
                ((ARTran)e.Row).SOOrderType, ((ARTran)e.Row).SOOrderNbr, ((ARTran)e.Row).SOShipmentType, ((ARTran)e.Row).SOShipmentNbr);

            if (siblings.Count == 0)
			{
				if (ordershipment != null)
				{
                    ordershipment.HasDetailDeleted = false;
					shipmentlist.Delete(ordershipment);
				}

				SOFreightDetail freight = FreightDetails.Select().Where(d => ((SOFreightDetail)d).ShipmentType == ((ARTran)e.Row).SOShipmentType && ((SOFreightDetail)d).ShipmentNbr == ((ARTran)e.Row).SOShipmentNbr).FirstOrDefault();
				if (freight != null)
				{
					if ((ordershipment = PXSelect<SOOrderShipment, Where<SOOrderShipment.invoiceType, Equal<Required<SOOrderShipment.invoiceType>>,
					And<SOOrderShipment.invoiceNbr, Equal<Required<SOOrderShipment.invoiceNbr>>,
					And<SOOrderShipment.shipmentType, Equal<Required<SOOrderShipment.shipmentType>>,
					And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>>>>>>.SelectWindowed(this, 0, 1,
					((ARTran)e.Row).TranType, ((ARTran)e.Row).RefNbr, ((ARTran)e.Row).SOShipmentType, ((ARTran)e.Row).SOShipmentNbr)) == null)
					FreightDetails.Delete(freight);
				}
			}
            else
            {
				if (ordershipment != null)
                {
                    ordershipment.HasDetailDeleted = true;
                    shipmentlist.Update(ordershipment);
                }
			}

			if (SODocument.Current != null)
			{
				SODocument.Current.IsTaxValid = false;
				if (SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Notchanged)
					SODocument.Cache.SetStatus(SODocument.Current, PXEntryStatus.Updated);
			}

			if (Document.Current != null)
			{
				Document.Current.IsTaxValid = false;
				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
					Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
			}
		}

		protected override void ARTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARTran row = (ARTran)e.Row;
			ARTran oldRow = (ARTran)e.OldRow;

			if (row != null)
			{
				if (!sender.ObjectsEqual<ARTran.qty,
					ARTran.uOM>(e.OldRow, e.Row))
				{
					SetFlatUnitPrice(sender, row);
				}

                if (!sender.ObjectsEqual<ARTran.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.inventoryID>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<ARTran.qty>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyUnitPrice>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<ARTran.curyExtPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyDiscAmt>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<ARTran.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.manualDisc>(e.Row, e.OldRow) ||
                    !sender.ObjectsEqual<ARTran.discountID>(e.Row, e.OldRow))
                    RecalculateDiscounts(sender, row);

				if (row.ManualDisc != true)
				{
					var discountCode = (ARDiscount)PXSelectorAttribute.Select<ARTran.discountID>(sender, row);
					row.DiscPctDR = (discountCode != null && discountCode.IsAppliedToDR == true) ? row.DiscPct : 0.0m;
				}

                if ((e.ExternalCall || sender.Graph.IsImport)
                    && sender.ObjectsEqual<ARTran.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<ARTran.uOM>(e.Row, e.OldRow)
                    && sender.ObjectsEqual<ARTran.qty>(e.Row, e.OldRow) && sender.ObjectsEqual<ARTran.branchID>(e.Row, e.OldRow)
					&& sender.ObjectsEqual<ARTran.siteID>(e.Row, e.OldRow)
					&& (!sender.ObjectsEqual<ARTran.curyUnitPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyExtPrice>(e.Row, e.OldRow))
					&& row.ManualPrice == oldRow.ManualPrice)
                    row.ManualPrice = true;

				if (row.ManualPrice != true)
				{
					row.CuryUnitPriceDR = row.CuryUnitPrice;
				}

				TaxAttribute.Calculate<ARTran.taxCategoryID>(sender, e);

				//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
				if (SODocument.Current != null && IsExternalTax == true &&
					!sender.ObjectsEqual<ARTran.accountID, ARTran.inventoryID, ARTran.tranDesc, ARTran.tranAmt, ARTran.tranDate, ARTran.taxCategoryID>(e.Row, e.OldRow))
				{
					if (SODocument.Current != null && SODocument.Current.IsTaxValid == true)
					{
					    SODocument.Current.IsTaxValid = false;
					    if (SODocument.Cache.GetStatus(SODocument.Current) == PXEntryStatus.Notchanged)
					        SODocument.Cache.SetStatus(SODocument.Current, PXEntryStatus.Updated);
					}

					if (Document.Current != null)
					{
					    Document.Current.IsTaxValid = false;
					    if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
					        Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
					}
				}
			}
		}

		protected override void ARTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.ARTran_RowSelected(sender, e);

			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<ARTran.inventoryID>(sender, row, row.SOOrderNbr == null);
				PXUIFieldAttribute.SetEnabled<ARTran.qty>(sender, row, row.SOOrderNbr == null);
				PXUIFieldAttribute.SetEnabled<ARTran.uOM>(sender, row, row.SOOrderNbr == null);
			}
		}

		protected override void ARTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((ARTran)e.Row).SOOrderNbr == null)
			{
				sender.SetDefaultExt<ARTran.lineType>(e.Row);
			}

			base.ARTran_InventoryID_FieldUpdated(sender, e);
		}

		protected virtual void ARTran_LineType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && ((ARTran)e.Row).SOOrderNbr == null && ((ARTran)e.Row).InventoryID != null)
			{
				InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<ARTran.inventoryID>(sender, e.Row);

				if (item != null)
				{
					e.NewValue = item.NonStockShip == true ? SOLineType.NonInventory : SOLineType.MiscCharge;
					e.Cancel = true;
				}
			}
		}

		protected override void ARTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARTran.unitPrice>(e.Row);
			sender.SetValue<ARTran.unitPrice>(e.Row, null);
		}

		protected override void ARTran_Qty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			base.ARTran_Qty_FieldUpdated(sender, e);
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				sender.SetDefaultExt<ARTran.tranDate>(row);
				sender.SetValueExt<ARTran.manualDisc>(row, false);

                if (row != null && row.InventoryID != null && row.UOM != null && row.IsFree != true)
                {
                    string customerPriceClass = ARPriceClass.EmptyPriceClass;
                    Location c = location.Select();
                    if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
                        customerPriceClass = c.CPriceClassID;

                    DateTime date = Document.Current.DocDate.Value;

                    if (row.TranType == ARDocType.CreditMemo && row.OrigInvoiceDate != null)
                    {
                        date = row.OrigInvoiceDate.Value;
                    }

                    decimal? price = ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, row.CustomerID, row.InventoryID, currencyinfo.Select(), row.UOM, row.Qty, date, row.CuryUnitPrice);
                    if (price != null)
                    sender.SetValueExt<ARTran.curyUnitPrice>(e.Row, price);
                }
			}
		}
				
		protected override void ARTran_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((ARTran)e.Row).SOOrderNbr) == false)
			{
				//tax category is taken from invoice lines
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void ARTran_SalesPersonID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((ARTran)e.Row).SOOrderNbr) == false)
			{
				//salesperson is taken from invoice lines
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected override void ARTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((ARTran)e.Row).SOOrderType) == false)
			{
                ARTran tran = (ARTran)e.Row;

                if (tran != null)
                {
					InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<ARTran.inventoryID>(sender, e.Row);
					INSite site = (INSite)PXSelectorAttribute.Select<ARTran.siteID>(sender, e.Row);
					ReasonCode reasoncode = (ReasonCode)PXSelectorAttribute.Select<ARTran.reasonCode>(sender, e.Row);
					SOOrderType ordertype = (SOOrderType)PXSelectorAttribute.Select<ARTran.sOOrderType>(sender, e.Row);
					INPostClass postclass = new INPostClass();
					if (item != null)
					{
						postclass = PXSelectReadonly<INPostClass, Where<INPostClass.postClassID, Equal<Required<INPostClass.postClassID>>>>.Select(this, item.PostClassID);
					}

                    Location customerloc = location.Current;

                    if (item == null)
                    {
                        return;
                    }

                    switch (ordertype.SalesAcctDefault)
                    {
                        case SOSalesAcctSubDefault.MaskItem:
                            e.NewValue = GetValue<InventoryItem.salesAcctID>(item);
                            e.Cancel = true;
                            break;
                        case SOSalesAcctSubDefault.MaskSite:
                            e.NewValue = GetValue<INSite.salesAcctID>(site);
                            e.Cancel = true;
                            break;
                        case SOSalesAcctSubDefault.MaskClass:
                            e.NewValue = GetValue<INPostClass.salesAcctID>(postclass);
                            e.Cancel = true;
                            break;
                        case SOSalesAcctSubDefault.MaskLocation:
                            e.NewValue = GetValue<Location.cSalesAcctID>(customerloc);
                            e.Cancel = true;
                            break;
                        case SOSalesAcctSubDefault.MaskReasonCode:
                            e.NewValue = GetValue<ReasonCode.salesAcctID>(reasoncode);
                            e.Cancel = true;
                            break;
                    }
                }
			}
			else
			{
				base.ARTran_AccountID_FieldDefaulting(sender, e);
			}
		}

		protected override void ARTran_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((ARTran)e.Row).SOOrderType) == false)
			{
                ARTran tran = (ARTran)e.Row;

                if (tran != null && tran.AccountID != null)
                {
					InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<ARTran.inventoryID>(sender, e.Row);
					INSite site = (INSite)PXSelectorAttribute.Select<ARTran.siteID>(sender, e.Row);
					ReasonCode reasoncode = (ReasonCode)PXSelectorAttribute.Select<ARTran.reasonCode>(sender, e.Row);
					SOOrderType ordertype = (SOOrderType)PXSelectorAttribute.Select<ARTran.sOOrderType>(sender, e.Row);
					SalesPerson salesperson = (SalesPerson)PXSelectorAttribute.Select<ARTran.salesPersonID>(sender, e.Row);
					INPostClass postclass = new INPostClass();
					if (item != null)
					{
						postclass = PXSelectReadonly<INPostClass, Where<INPostClass.postClassID, Equal<Required<INPostClass.postClassID>>>>.Select(this, item.PostClassID);
					}

                    EPEmployee employee = (EPEmployee)PXSelectJoin<EPEmployee, InnerJoin<SOOrder, On<EPEmployee.userID, Equal<SOOrder.ownerID>>>, Where<SOOrder.orderType, Equal<Required<ARTran.sOOrderType>>, And<SOOrder.orderNbr, Equal<Required<ARTran.sOOrderNbr>>>>>.Select(this, tran.SOOrderType, tran.SOOrderNbr);
                    CRLocation companyloc =
                        PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>, Where<Branch.branchID, Equal<Required<ARTran.branchID>>>>.Select(this, tran.BranchID);
                    Location customerloc = location.Current;

                    object item_SubID = GetValue<InventoryItem.salesSubID>(item);
                    object site_SubID = GetValue<INSite.salesSubID>(site);
                    object postclass_SubID = GetValue<INPostClass.salesSubID>(postclass);
                    object customer_SubID = GetValue<Location.cSalesSubID>(customerloc);
                    object employee_SubID = GetValue<EPEmployee.salesSubID>(employee);
                    object company_SubID = GetValue<CRLocation.cMPSalesSubID>(companyloc);
                    object salesperson_SubID = GetValue<SalesPerson.salesSubID>(salesperson);
                    object reasoncode_SubID = GetValue<ReasonCode.salesSubID>(reasoncode);

                    object value = null;

                    try
                    {
                        value = SOSalesSubAccountMaskAttribute.MakeSub<SOOrderType.salesSubMask>(this, ordertype.SalesSubMask,
                                                                                                 new object[] 
                                                                                             { 
                                                                                                 item_SubID, 
                                                                                                 site_SubID, 
                                                                                                 postclass_SubID, 
                                                                                                 customer_SubID, 
                                                                                                 employee_SubID, 
                                                                                                 company_SubID, 
                                                                                                 salesperson_SubID, 
                                                                                                 reasoncode_SubID 
                                                                                             },
                                                                                                 new Type[] 
                                                                                             { 
                                                                                                 typeof(InventoryItem.salesSubID), 
                                                                                                 typeof(INSite.salesSubID), 
                                                                                                 typeof(INPostClass.salesSubID), 
                                                                                                 typeof(Location.cSalesSubID), 
                                                                                                 typeof(EPEmployee.salesSubID), 
                                                                                                 typeof(Location.cMPSalesSubID), 
                                                                                                 typeof(SalesPerson.salesSubID), 
                                                                                                 typeof(ReasonCode.subID) 
                                                                                             });

                        sender.RaiseFieldUpdating<ARTran.subID>(tran, ref value);
                    }
                    catch (PXMaskArgumentException ex)
                    {
                        sender.RaiseExceptionHandling<ARTran.subID>(e.Row, null, new PXSetPropertyException(ex.Message));
                        value = null;
                    }
                    catch (PXSetPropertyException ex)
                    {
                        sender.RaiseExceptionHandling<ARTran.subID>(e.Row, value, ex);
                        value = null;
                    }

                    e.NewValue = (int?)value;
                    e.Cancel = true;
                }
			}
			else
			{
				base.ARTran_SubID_FieldDefaulting(sender, e);
			}
		}

        #region SOOrderDiscountDetail events

        protected virtual void SOInvoiceDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (Document != null && Document.Current != null)
                Document.Cache.SetValueExt<SOInvoice.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<SOInvoiceDiscountDetail>(DiscountDetails, true));
        }

        #endregion

		public virtual void AddDiscount(PXCache sender, PXRowUpdatedEventArgs e)
		{
            AddDiscountDetails(sender, e);
            
            ARTran discount = (ARTran)Discount.Cache.CreateInstance();
			discount.LineType = SOLineType.Discount;
			discount.DrCr = (Document.Current.DrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
			discount.FreezeManualDisc = true;
			discount = (ARTran)Discount.Select() ?? (ARTran)Discount.Cache.Insert(discount);

			ARTran old_row = (ARTran)Discount.Cache.CreateCopy(discount);

			discount.CuryTranAmt = (decimal?)sender.GetValue<SOInvoice.curyDiscTot>(e.Row);
			discount.TaxCategoryID = null;
			using (new PXLocaleScope(customer.Current.LocaleName))
				discount.TranDesc = PXMessages.LocalizeNoPrefix(Messages.DocDiscDescr);
			
			DefaultDiscountAccountAndSubAccount(discount);

            if (discount.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, discount.ProjectID))
            {
                PM.PMProject project = PXSelect<PM.PMProject, Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>.Select(this, discount.ProjectID);
                if (project != null && project.BaseType != "C")
                {
					PM.PMAccountTask task = PXSelect<PM.PMAccountTask, Where<PM.PMAccountTask.projectID, Equal<Required<PM.PMAccountTask.projectID>>, And<PM.PMAccountTask.accountID, Equal<Required<PM.PMAccountTask.accountID>>>>>.Select(this, discount.ProjectID, discount.AccountID);
                    if (task != null)
                    {
                        discount.TaskID = task.TaskID;
                    }
                    else
                    {
                        Account ac = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, discount.AccountID);
                        throw new PXException(Messages.AccountMappingNotConfigured, project.ContractCD, ac.AccountCD);
                    }
                }
            }
			
			if (Discount.Cache.GetStatus(discount) == PXEntryStatus.Notchanged)
			{
				Discount.Cache.SetStatus(discount, PXEntryStatus.Updated);
			}

			discount.ManualDisc = true; //escape SOManualDiscMode.RowUpdated
			Discount.Cache.RaiseRowUpdated(discount, old_row);

            decimal auotDocDisc = DiscountEngine.GetTotalGroupAndDocumentDiscount<SOInvoiceDiscountDetail>(DiscountDetails);
			if (auotDocDisc == discount.CuryTranAmt)
			{
				discount.ManualDisc = false;
			}
			
			if (discount.CuryTranAmt == 0)
			{
				Discount.Delete(discount);
			}
		}

        public virtual void AddDiscountDetails(PXCache sender, PXRowUpdatedEventArgs e)
        {
            foreach (SOInvoiceDiscountDetail dDetail in DiscountDetails.Select())
            {
                ARInvoiceDiscountDetail arDiscountDetail = new ARInvoiceDiscountDetail();
                SOInvoiceDiscountDetail discountCopy = (SOInvoiceDiscountDetail)DiscountDetails.Cache.CreateCopy(dDetail);
                foreach (ARInvoiceDiscountDetail aDetail in ARDiscountDetails.Select())
                {
                    if (aDetail.DiscountID == discountCopy.DiscountID && aDetail.DiscountSequenceID == discountCopy.DiscountSequenceID && aDetail.Type == discountCopy.Type)
                    {
                        arDiscountDetail = aDetail;
                    }
                }
                arDiscountDetail.CuryDiscountableAmt = discountCopy.CuryDiscountableAmt;
                arDiscountDetail.CuryDiscountAmt = discountCopy.CuryDiscountAmt;
                arDiscountDetail.CuryInfoID = discountCopy.CuryInfoID;
                arDiscountDetail.DiscountableAmt = discountCopy.DiscountableAmt;
                arDiscountDetail.DiscountableQty = discountCopy.DiscountableQty;
                arDiscountDetail.DiscountAmt = discountCopy.DiscountAmt;
                arDiscountDetail.DiscountID = discountCopy.DiscountID;
                arDiscountDetail.DiscountPct = discountCopy.DiscountPct;
                arDiscountDetail.DiscountSequenceID = discountCopy.DiscountSequenceID;
                arDiscountDetail.FreeItemID = discountCopy.FreeItemID;
                arDiscountDetail.FreeItemQty = discountCopy.FreeItemQty;
                arDiscountDetail.Type = discountCopy.Type;
                ARDiscountDetails.Update(arDiscountDetail);
            }
        }

		protected virtual void SOFreightDetail_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOFreightDetail row = e.Row as SOFreightDetail;
			if (row != null && row.TaskID == null)
			{
				sender.SetDefaultExt<SOFreightDetail.taskID>(e.Row);
			}
		}


		protected virtual void SOFreightDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (Document.Current != null)
			{
				PXUIFieldAttribute.SetEnabled<SOFreightDetail.curyPremiumFreightAmt>(sender, e.Row, Document.Current.Released != true);
			}
		}
				
		protected virtual void SOFreightDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOFreightDetail row = e.Row as SOFreightDetail;
			if (row != null)
			{
				foreach (ARTran tran in PXSelect<ARTran,
				Where<ARTran.lineType, Equal<SOLineType.freight>,
				And<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
				And<ARTran.sOShipmentNbr, Equal<Required<ARTran.sOShipmentNbr>>>>>>>.Select(this, row.ShipmentNbr))
				{
					Freight.Delete(tran);
				}

				if (row.CuryTotalFreightAmt != 0)
					AddFreightTransaction(row);
			}
		}

		protected virtual void SOFreightDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SOFreightDetail row = e.Row as SOFreightDetail;
			if (row != null)
			{
				if (row.CuryTotalFreightAmt != 0)
					AddFreightTransaction(row);
			}
		}


		public override IEnumerable transactions()
		{
			return PXSelectJoin<ARTran, 
				LeftJoin<SOLine, On<SOLine.orderType, Equal<ARTran.sOOrderType>, And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>, And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>>, 
			Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>, And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>, 
				And<ARTran.lineType, NotEqual<SOLineType.freight>, And<ARTran.lineType, NotEqual<SOLineType.discount>>>>>, 
			OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>.Select(this);
		}

		public override int ExecuteInsert(string viewName, IDictionary values, params object[] parameters)
		{
			switch (viewName)
			{
				case "Freight":
					values[PXDataUtils.FieldName<ARTran.lineType>()] = SOLineType.Freight;
					break;
				case "Discount":
					values[PXDataUtils.FieldName<ARTran.lineType>()] = SOLineType.Discount;
					break;
			}
			return base.ExecuteInsert(viewName, values, parameters);
		}

		public virtual IEnumerable sHipmentlist()
		{
			PXSelectBase<ARTran> cmd = new PXSelect<ARTran, Where<ARTran.sOShipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<ARTran.sOShipmentType, Equal<Current<SOOrderShipment.shipmentType>>, And<ARTran.sOOrderType, Equal<Current<SOOrderShipment.orderType>>, And<ARTran.sOOrderNbr, Equal<Current<SOOrderShipment.orderNbr>>>>>>>(this);

			DocumentList<ARInvoice, SOInvoice> list = new DocumentList<ARInvoice, SOInvoice>(this);
			list.Add(Document.Current, SODocument.Select());

			bool newInvoice = Transactions.Select().Count == 0;

			HashSet<SOOrderShipment> updated = new HashSet<SOOrderShipment>();

			foreach (SOOrderShipment shipment in shipmentlist.Cache.Updated)
			{
				updated.Add(shipment);
				yield return shipment;
			}

			foreach (PXResult<SOOrderShipment, SOOrder, SOShipLine, SOOrderType, SOOrderTypeOperation> order in 
			PXSelectJoinGroupBy<SOOrderShipment,
			InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
			InnerJoin<SOShipLine, On<SOShipLine.shipmentType, Equal<SOOrderShipment.shipmentType>, And<SOShipLine.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>, And<SOShipLine.origOrderType, Equal<SOOrderShipment.orderType>, And<SOShipLine.origOrderNbr, Equal<SOOrderShipment.orderNbr>>>>>,
			InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrderShipment.orderType>>,
			InnerJoin<SOOrderTypeOperation, 
							On<SOOrderTypeOperation.orderType, Equal<SOOrder.orderType>,
							And<SOOrderTypeOperation.operation, Equal<SOOrderShipment.operation>>>>>>>,
			Where<SOOrderShipment.customerID, Equal<Current<ARInvoice.customerID>>,
				And<SOOrderShipment.hold, Equal<boolFalse>,
				And<SOOrderShipment.confirmed, Equal<boolTrue>, 
				And<SOOrderType.aRDocType, Equal<Current<ARInvoice.docType>>,
				And<SOOrderShipment.invoiceNbr, IsNull>>>>>,
			Aggregate<GroupBy<SOOrderShipment.shipmentNbr, GroupBy<SOOrderShipment.orderType, GroupBy<SOOrderShipment.orderNbr>>>>>.Select(this))
			{
				if (!updated.Contains((SOOrderShipment)order) && cmd.View.SelectSingleBound(new object[] { (SOOrderShipment)order }) == null)
				{
					if (newInvoice || list.Find<ARInvoice.customerID, SOInvoice.billAddressID, SOInvoice.billContactID, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrder)order).CustomerID, ((SOOrder)order).BillAddressID, ((SOOrder)order).BillContactID, ((SOOrder)order).CuryID, ((SOOrder)order).TermsID, false) != null)
					{
						yield return (SOOrderShipment)order;
					}
				}
			}

			foreach (PXResult<SOOrderShipment, SOOrder, POReceiptLine> order in PXSelectJoinGroupBy<SOOrderShipment,
					InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,				
					InnerJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<SOOrderShipment.shipmentNbr>>,
					InnerJoin<SOLineSplit, On<SOLineSplit.pOType, Equal<POReceiptLine.pOType>, And<SOLineSplit.pONbr, Equal<POReceiptLine.pONbr>, And<SOLineSplit.pOLineNbr, Equal<POReceiptLine.pOLineNbr>, And<SOLineSplit.orderType, Equal<SOOrder.orderType>, And<SOLineSplit.orderNbr, Equal<SOOrder.orderNbr>>>>>>,
					InnerJoin<SOLine, On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>,
					InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOLine.orderType>>,
					InnerJoin<SOOrderTypeOperation, 
							On<SOOrderTypeOperation.orderType, Equal<SOLine.orderType>,
							And<SOOrderTypeOperation.operation, Equal<SOLine.operation>>>>>>>>>,
					Where<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>, 
						And2<Where<POReceiptLine.lineType, Equal<POLineType.goodsForDropShip>, Or<POReceiptLine.lineType, Equal<POLineType.nonStockForDropShip>>>,
						And<SOOrder.customerID, Equal<Current<ARInvoice.customerID>>,
						And<SOOrderType.aRDocType, Equal<Current<ARInvoice.docType>>,
						And<SOOrderShipment.invoiceNbr, IsNull>>>>>,
					Aggregate<
						GroupBy<SOOrderShipment.shipmentType, 
						GroupBy<SOOrderShipment.shipmentNbr, 
						GroupBy<SOOrderShipment.orderType, 
						GroupBy<SOOrderShipment.orderNbr,
						Sum<POReceiptLine.receiptQty,
						Sum<POReceiptLine.extWeight,
						Sum<POReceiptLine.extVolume>>>>>>>>>.Select(this))
			{
				if (!updated.Contains((SOOrderShipment)order) && cmd.View.SelectSingleBound(new object[] { (SOOrderShipment)order }) == null)
				{
					if (newInvoice || list.Find<ARInvoice.customerID, SOInvoice.billAddressID, SOInvoice.billContactID, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrder)order).CustomerID, ((SOOrder)order).BillAddressID, ((SOOrder)order).BillContactID, ((SOOrder)order).CuryID, ((SOOrder)order).TermsID, false) != null)
					{
						yield return (SOOrderShipment)order;
					}
				}
			}
		}

		protected virtual void SOOrderShipment_ShipmentNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void SOOrderShipment_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (!string.Equals(((SOOrderShipment)e.Row).ShipmentNbr, Constants.NoShipmentNbr) 
                && ((SOOrderShipment)e.Row).ShipmentNbr != null && ((SOOrderShipment)e.Row).ShipmentType != null)
			{
				SOOrderShipment copy = PXCache<SOOrderShipment>.CreateCopy((SOOrderShipment)e.Row);

				((SOOrderShipment)e.Row).InvoiceType = null;
				((SOOrderShipment)e.Row).InvoiceNbr = null;
				shipmentlist.Cache.SetStatus(((SOOrderShipment)e.Row), PXEntryStatus.Updated);
				shipmentlist.Cache.RaiseRowUpdated(e.Row, copy);

				//Probably not needed because of PXFormula referencing SOShipment
				foreach (SOShipment shipment in PXSelect<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>>>.SelectSingleBound(this, new object[] { e.Row }))
				{
					//persist shipments to workflow
					shipments.Cache.SetStatus(shipment, PXEntryStatus.Updated);
				}

				e.Cancel = true;
			}
		}

        protected virtual void SOOrderShipment_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            SOOrderShipment row = e.Row as SOOrderShipment;
            if (row != null && string.Equals(row.ShipmentNbr, Constants.NoShipmentNbr))
            {
                SOOrder cached = soorder.Locate(new SOOrder { OrderType = row.OrderType, OrderNbr = row.OrderNbr });
                if (cached != null)
                { 
                    cached.ShipmentCntr--;

	                if (cached.ShipmentCntr == 0 && (string.Equals(row.ShipmentNbr, Constants.NoShipmentNbr)))
	                {
		                cached.Completed = false;
		                cached.Status = SOOrderStatus.Open;
	                }

                    soorder.Update(cached);
                }
            }
        }

		protected virtual void SOOrderShipment_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<SOOrderShipment.selected>(sender, e.Row, true);
		}

        protected virtual void SOOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            SOOrder doc = (SOOrder)e.Row;
            if (e.Operation == PXDBOperation.Update)
            {
                if (doc.ShipmentCntr < 0 || doc.OpenShipmentCntr < 0 || doc.ShipmentCntr < doc.BilledCntr + doc.ReleasedCntr && doc.Behavior == SOBehavior.SO)
                {
                    throw new PXSetPropertyException(Messages.InvalidShipmentCounters);
                }
            }
        }

        protected virtual void SOOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			SOOrder oldRow = e.OldRow as SOOrder;
			if (row != null && oldRow != null && row.UnbilledOrderQty != oldRow.UnbilledOrderQty)
			{
				row.IsUnbilledTaxValid = false;
			}
			
			if (e.OldRow != null)
			{
				ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.OldRow, -((SOOrder)e.OldRow).UnbilledOrderTotal, -((SOOrder)e.Row).OpenOrderTotal);
			}
			ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.Row, ((SOOrder)e.Row).UnbilledOrderTotal, ((SOOrder)e.Row).OpenOrderTotal);
		}

		public bool TransferApplicationFromSalesOrder;
		public override IEnumerable adjustments()
		{
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this);

			int applcount = 0;
			foreach (PXResult<ARAdjust2, ARPayment, AR.Standalone.ARRegisterAlias, CurrencyInfo> res in Adjustments_Inv.Select())
			{
				ARPayment payment = PXCache<ARPayment>.CreateCopy(res);
				ARAdjust2 adj = res;
				CurrencyInfo pay_info = res;

				PXCache<ARRegister>.RestoreCopy(payment, (AR.Standalone.ARRegisterAlias)res);
				ARPayment originalPayment = PXCache<ARPayment>.CreateCopy(payment);

				if (adj == null) continue;

				ARAdjust2 other = PXSelectGroupBy<ARAdjust2, Where<ARAdjust2.adjgDocType, Equal<Required<ARAdjust2.adjgDocType>>, And<ARAdjust2.adjgRefNbr, Equal<Required<ARAdjust2.adjgRefNbr>>, And<ARAdjust2.released, Equal<False>, And<Where<ARAdjust2.adjdDocType, NotEqual<Required<ARAdjust2.adjdDocType>>, Or<ARAdjust2.adjdRefNbr, NotEqual<Required<ARAdjust2.adjdRefNbr>>>>>>>>, Aggregate<GroupBy<ARAdjust2.adjgDocType, GroupBy<ARAdjust2.adjgRefNbr, Sum<ARAdjust2.curyAdjgAmt, Sum<ARAdjust2.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdDocType, adj.AdjdRefNbr);
				if (other != null && other.AdjdRefNbr != null)
				{
					payment.CuryDocBal -= other.CuryAdjgAmt;
					payment.DocBal -= other.AdjAmt;
				}

				BalanceCalculation.CalculateApplicationDocumentBalance(
					Adjustments.Cache, 
					payment, 
					adj, 
					pay_info, 
					inv_info);

                yield return new PXResult<ARAdjust2, ARPayment>(adj, originalPayment);
				applcount++;
			}

			if (Document.Current != null && (Document.Current.DocType == ARDocType.Invoice || Document.Current.DocType == ARDocType.DebitMemo) && Document.Current.Released == false)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Adjustments.Cache, Document.Cache, arbalances.Cache, SODocument.Cache))
				{
					//same as ARInvoiceEntry but without released constraint and with hold constraint
					PXSelectBase<ARPayment> cmd = new PXSelectReadonly2<ARPayment,
						InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>,
						LeftJoin<ARAdjust2,
							On<ARAdjust2.adjgDocType, Equal<ARPayment.docType>,
							And<ARAdjust2.adjgRefNbr, Equal<ARPayment.refNbr>,
							And<ARAdjust2.adjNbr, Equal<ARPayment.adjCntr>,
							And<ARAdjust2.released, Equal<False>,
							And<ARAdjust2.hold, Equal<False>,
							And<ARAdjust2.voided, Equal<False>,
							And<Where<ARAdjust2.adjdDocType, NotEqual<Current<ARInvoice.docType>>,
								Or<ARAdjust2.adjdRefNbr, NotEqual<Current<ARInvoice.refNbr>>>>>>>>>>>>>,
						Where<ARPayment.customerID, Equal<Current<ARInvoice.customerID>>,
							And2<Where<ARPayment.docType, Equal<ARDocType.payment>,
								Or<ARPayment.docType, Equal<ARDocType.prepayment>,
								Or<ARPayment.docType, Equal<ARDocType.creditMemo>>>>,
							And<ARPayment.openDoc, Equal<boolTrue>,
							And<ARAdjust2.adjdRefNbr, IsNull>>>>>(this);

					//this delegate is invoked in processing to transfer applications from sales order
					//date and period constraints are not valid in this case
					if (!TransferApplicationFromSalesOrder)
					{
						cmd.Join<LeftJoin<SOAdjust,
						On<SOAdjust.adjgDocType, Equal<ARPayment.docType>,
							And<SOAdjust.adjgRefNbr, Equal<ARPayment.refNbr>,
							And<SOAdjust.adjAmt, Greater<decimal0>>>>>>();

						cmd.WhereAnd<Where<ARPayment.docDate, LessEqual<Current<ARInvoice.docDate>>,
							And<ARPayment.finPeriodID, LessEqual<Current<ARInvoice.finPeriodID>>,
							And<SOAdjust.adjgRefNbr, IsNull>>>>();
					}

					foreach (PXResult<AR.ARPayment> res in cmd.SelectWindowed(0, Constants.MaxNumberOfPaymentsAndMemos - applcount))
					{
						ARPayment payment = PXCache<ARPayment>.CreateCopy(res);
						ARAdjust2 adj = new ARAdjust2();
						CurrencyInfo pay_info = PXResult.Unwrap<CurrencyInfo>(res);

						adj.CustomerID = payment.CustomerID;
						adj.AdjdCustomerID = Document.Current.CustomerID;
						adj.AdjdDocType = Document.Current.DocType;
						adj.AdjdRefNbr = Document.Current.RefNbr;
						adj.AdjdBranchID = Document.Current.BranchID;
						adj.AdjgDocType = payment.DocType;
						adj.AdjgRefNbr = payment.RefNbr;
						adj.AdjgBranchID = payment.BranchID;
						adj.AdjNbr = payment.AdjCntr;

						ARAdjust2 other = PXSelectGroupBy<ARAdjust2, Where<ARAdjust2.adjgDocType, Equal<Required<ARAdjust2.adjgDocType>>, And<ARAdjust2.adjgRefNbr, Equal<Required<ARAdjust2.adjgRefNbr>>, And<ARAdjust2.released, Equal<False>, And<Where<ARAdjust2.adjdDocType, NotEqual<Required<ARAdjust2.adjdDocType>>, Or<ARAdjust2.adjdRefNbr, NotEqual<Required<ARAdjust2.adjdRefNbr>>>>>>>>, Aggregate<GroupBy<ARAdjust2.adjgDocType, GroupBy<ARAdjust2.adjgRefNbr, Sum<ARAdjust2.curyAdjgAmt, Sum<ARAdjust2.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdDocType, adj.AdjdRefNbr);
						if (other != null && other.AdjdRefNbr != null)
						{
							payment.CuryDocBal -= other.CuryAdjgAmt;
							payment.DocBal -= other.AdjAmt;
						}

						if (Adjustments.Cache.Locate(adj) == null)
						{
							adj.AdjgCuryInfoID = payment.CuryInfoID;
							adj.AdjdOrigCuryInfoID = Document.Current.CuryInfoID;
							//if LE constraint is removed from payment selection this must be reconsidered
							adj.AdjdCuryInfoID = Document.Current.CuryInfoID;

							decimal CuryDocBal;
							if (string.Equals(pay_info.CuryID, inv_info.CuryID))
							{
								CuryDocBal = payment.CuryDocBal ?? 0m;
							}
							else
							{
								PXDBCurrencyAttribute.CuryConvCury(Adjustments.Cache, inv_info, payment.DocBal ?? 0m, out CuryDocBal);
							}
							adj.CuryDocBal = CuryDocBal;

							yield return new PXResult<ARAdjust2, ARPayment>(Adjustments.Insert(adj), payment);
						}
					}
				}
			}
		}

        public virtual Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<SOOrderDiscountDetail, SOLine>> CollectGroupDiscountToLineCorrelation(SOOrder order)
        {
            PXCache cache = this.Caches[typeof(SOLine)];
            PXSelectBase<SOLine> transactions = new PXSelect<SOLine, Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>(this);
            PXSelectBase<SOOrderDiscountDetail> discountdetail = new PXSelect<SOOrderDiscountDetail, Where<SOOrderDiscountDetail.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderDiscountDetail.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>(this);

            this.Defaults.Add(typeof(SOOrder), () => { return order; });
            try
            {
                return DiscountEngine<SOLine>.CollectGroupDiscountToLineCorrelation<SOOrderDiscountDetail>(cache, transactions, discountdetail, order.CustomerLocationID, (DateTime)order.OrderDate, false);
            }
            finally { this.Defaults.Remove(typeof(SOOrder)); }
        }

		public virtual void InvoiceOrder(DateTime invoiceDate, PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType> order, Customer customer, DocumentList<ARInvoice, SOInvoice> list)
		{
			InvoiceOrder(invoiceDate, order, null, customer, list);
		}

		public virtual void InvoiceOrder(DateTime invoiceDate, PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType> order, PXResultset<SOShipLine, SOLine> details, Customer customer, DocumentList<ARInvoice, SOInvoice> list)
		{
			ARInvoice newdoc;
			SOOrder soOrder = order;
			SOOrderType ordertype = order;
			SOOrderShipment orderShipment = order;

            decimal ApprovedBalance = 0;
			HashSet<SOOrder> accountedForOrders = new HashSet<SOOrder>(new LSSOLine.Comparer<SOOrder>(this));
            
            PXRowUpdated ApprovedBalanceCollector = delegate (PXCache sender, PXRowUpdatedEventArgs e)
            {
                ARInvoice ARDoc = (ARInvoice)e.Row;

				//Discounts can reduce the balance - adjust the creditHold if it was wrongly set:
				if ((decimal)ARDoc.DocBal <= ApprovedBalance && ARDoc.CreditHold == true)
				{
					object OldRow = sender.CreateCopy(ARDoc);
					sender.SetValueExt<ARInvoice.creditHold>(ARDoc, false);
					sender.RaiseRowUpdated(ARDoc, OldRow);
				}

				//Maximum approved balance for an invoice is the sum of all approved order amounts:
				if ((bool)((SOOrder)order).ApprovedCredit)
                {
					if (!accountedForOrders.Contains((SOOrder)order))
                    {
						ApprovedBalance += ((SOOrder)order).ApprovedCreditAmt.GetValueOrDefault();
						accountedForOrders.Add((SOOrder)order);
                    }

					ARDoc.ApprovedCreditAmt = ApprovedBalance;
					ARDoc.ApprovedCredit = true;
                }
            };
            CustomerCreditHelper.AppendPreUpdatedEvent(typeof(ARInvoice), ApprovedBalanceCollector);

            if (list != null)
			{
				bool iscc = false;
				DateTime? orderInvoiceDate = (sosetup.Current.UseShipDateForInvoiceDate == true && soOrder.InvoiceDate == null ? ((SOOrderShipment)order).ShipDate : soOrder.InvoiceDate);

				if (soOrder.BillSeparately == false)
				{
					iscc = PXSelectReadonly<CCProcTran, Where<CCProcTran.origDocType, Equal<Required<CCProcTran.origDocType>>, And<CCProcTran.origRefNbr, Equal<Required<CCProcTran.origRefNbr>>, And<CCProcTran.refNbr, IsNull>>>>.Select(this, soOrder.OrderType, soOrder.OrderNbr).Count > 0;
				}

				if (soOrder.PaymentCntr == 0 && soOrder.BillSeparately == false && iscc == false)
				{
					if (soOrder.PaymentMethodID == null && soOrder.CashAccountID == null)
						newdoc = list.Find<ARInvoice.docType, ARInvoice.docDate, ARInvoice.branchID, ARInvoice.customerID, ARInvoice.customerLocationID, SOInvoice.billAddressID, SOInvoice.billContactID, SOInvoice.extRefNbr, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrderType)order).ARDocType, orderInvoiceDate ?? invoiceDate, soOrder.BranchID, soOrder.CustomerID, soOrder.CustomerLocationID, soOrder.BillAddressID, soOrder.BillContactID, soOrder.ExtRefNbr, soOrder.CuryID, soOrder.TermsID, false) ?? (ARInvoice)new ARInvoice();
					else if (soOrder.CashAccountID == null)
						newdoc = list.Find<ARInvoice.docType, ARInvoice.docDate, ARInvoice.branchID, ARInvoice.customerID, ARInvoice.customerLocationID, SOInvoice.billAddressID, SOInvoice.billContactID, SOInvoice.pMInstanceID, SOInvoice.extRefNbr, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrderType)order).ARDocType, orderInvoiceDate ?? invoiceDate, soOrder.BranchID, soOrder.CustomerID, soOrder.CustomerLocationID, soOrder.BillAddressID, soOrder.BillContactID, soOrder.PMInstanceID, soOrder.ExtRefNbr, soOrder.CuryID, soOrder.TermsID, false) ?? (ARInvoice)new ARInvoice();					
					else if (AvalaraMaint.IsExternalTax(this, soOrder.TaxZoneID))
						newdoc = list.Find<ARInvoice.docType, ARInvoice.docDate, ARInvoice.branchID, ARInvoice.customerID, ARInvoice.customerLocationID, SOInvoice.billAddressID, SOInvoice.shipAddressID, SOInvoice.billContactID, SOInvoice.pMInstanceID, SOInvoice.cashAccountID, SOInvoice.extRefNbr, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrderType)order).ARDocType, orderInvoiceDate ?? invoiceDate, soOrder.BranchID, soOrder.CustomerID, soOrder.CustomerLocationID, soOrder.BillAddressID, orderShipment.ShipAddressID, soOrder.BillContactID, soOrder.PMInstanceID, soOrder.CashAccountID, soOrder.ExtRefNbr, soOrder.CuryID, soOrder.TermsID, false) ?? (ARInvoice)new ARInvoice();
					else
						newdoc = list.Find<ARInvoice.docType, ARInvoice.docDate, ARInvoice.branchID, ARInvoice.customerID, ARInvoice.customerLocationID, SOInvoice.billAddressID, SOInvoice.billContactID, SOInvoice.pMInstanceID, SOInvoice.cashAccountID, SOInvoice.extRefNbr, ARInvoice.curyID, ARInvoice.termsID, ARInvoice.hidden>(((SOOrderType)order).ARDocType, orderInvoiceDate ?? invoiceDate, soOrder.BranchID, soOrder.CustomerID, soOrder.CustomerLocationID, soOrder.BillAddressID, soOrder.BillContactID, soOrder.PMInstanceID, soOrder.CashAccountID, soOrder.ExtRefNbr, soOrder.CuryID, soOrder.TermsID, false) ?? (ARInvoice)new ARInvoice();
				}
				else
				{
					newdoc = list.Find<ARInvoice.hidden, ARInvoice.hiddenOrderType, ARInvoice.hiddenOrderNbr>(true, soOrder.OrderType, soOrder.OrderNbr);
					if (newdoc == null)
					{
						newdoc = new ARInvoice();
						newdoc.HiddenOrderType = soOrder.OrderType;
						newdoc.HiddenOrderNbr = soOrder.OrderNbr;
						newdoc.Hidden = true;
					}
				}

				if (newdoc.RefNbr != null)
				{
					Document.Current = this.Document.Search<ARInvoice.refNbr>(newdoc.RefNbr, newdoc.DocType);
				}
				else
				{
					this.Clear();

					string docType = ((SOOrderType)order).ARDocType;
					if (((SOOrderShipment)order).Operation == ((SOOrderType)order).DefaultOperation)
					{
						newdoc.DocType = docType;
					}
					else
					{
						//for RMA switch document type if previous shipment was not invoiced previously in the current run, i.e. list.Find() returned null
 						newdoc.DocType = 
							docType == ARDocType.Invoice ? ARDocType.CreditMemo :
							docType == ARDocType.DebitMemo ? ARDocType.CreditMemo :
							docType == ARDocType.CreditMemo ? ARDocType.Invoice :
							docType == ARDocType.CashSale ? ARDocType.CashReturn :
							docType == ARDocType.CashReturn ? ARDocType.CashSale :
							null;
					}

					newdoc.DocDate = orderInvoiceDate ?? invoiceDate;

					if (string.IsNullOrEmpty(soOrder.FinPeriodID) == false)
					{
						newdoc.FinPeriodID = soOrder.FinPeriodID;
					}

					if (soOrder.InvoiceNbr != null)
					{
						newdoc.RefNbr = soOrder.InvoiceNbr;
						newdoc.RefNoteID = soOrder.NoteID;
					}

					if (((SOOrderType)order).UserInvoiceNumbering == true && string.IsNullOrEmpty(newdoc.RefNbr))
					{
						throw new PXException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<SOOrder.invoiceNbr>(soorder.Cache));
					}

					if (((SOOrderType)order).UserInvoiceNumbering == false && !string.IsNullOrEmpty(newdoc.RefNbr))
					{
						throw new PXException(Messages.MustBeUserNumbering, ((SOOrderType)order).InvoiceNumberingID);
					}

					AutoNumberAttribute.SetNumberingId<ARInvoice.refNbr>(Document.Cache, ((SOOrderType)order).ARDocType, ((SOOrderType)order).InvoiceNumberingID);

					newdoc = (ARInvoice)Document.Cache.CreateCopy(this.Document.Insert(newdoc));

					newdoc.BranchID = soOrder.BranchID;
					newdoc.CustomerID = ((SOOrder)order).CustomerID;
					newdoc.CustomerLocationID = ((SOOrder)order).CustomerLocationID;
					newdoc.TermsID = ((SOOrder)order).TermsID;
					newdoc.DiscDate = ((SOOrder)order).DiscDate;
					newdoc.DueDate = ((SOOrder)order).DueDate;
					newdoc.TaxZoneID = ((SOOrder)order).TaxZoneID;
					newdoc.AvalaraCustomerUsageType = ((SOOrder)order).AvalaraCustomerUsageType;
					newdoc.SalesPersonID = ((SOOrder)order).SalesPersonID;
					newdoc.DocDesc = ((SOOrder)order).OrderDesc;
					newdoc.InvoiceNbr = ((SOOrder)order).CustomerOrderNbr;
					newdoc.CuryID = ((SOOrder)order).CuryID;
					newdoc.ProjectID = ((SOOrder)order).ProjectID ?? PM.ProjectDefaultAttribute.NonProject(this);
				    newdoc.Hold = ordertype.InvoiceHoldEntry;

					if (((SOOrderType)order).MarkInvoicePrinted == true)
					{
						newdoc.Printed = true;
					}

					if (((SOOrderType)order).MarkInvoiceEmailed == true)
					{
						newdoc.Emailed = true;
					}

					if (soOrder.PMInstanceID != null || string.IsNullOrEmpty(soOrder.PaymentMethodID) == false)
					{
						newdoc.PMInstanceID = soOrder.PMInstanceID;
						newdoc.PaymentMethodID = soOrder.PaymentMethodID;
						newdoc.CashAccountID = soOrder.CashAccountID;
					}

					using (new PXReadDeletedScope())
					{
					newdoc = this.Document.Update(newdoc);
					}


					if (soOrder.PMInstanceID != null || string.IsNullOrEmpty(soOrder.PaymentMethodID) == false)
					{
						SODocument.Current.PMInstanceID = soOrder.PMInstanceID;
						SODocument.Current.PaymentMethodID = soOrder.PaymentMethodID;
						if (SODocument.Current.CashAccountID != soOrder.CashAccountID)
							SODocument.SetValueExt<SOInvoice.cashAccountID>(SODocument.Current, soOrder.CashAccountID);
						if (SODocument.Current.CashAccountID == null)
							SODocument.Cache.SetDefaultExt<SOInvoice.cashAccountID>(SODocument.Current);
						if (SODocument.Current.ARPaymentDepositAsBatch == true && SODocument.Current.DepositAfter == null)
							SODocument.Current.DepositAfter = SODocument.Current.AdjDate;
						SODocument.Current.ExtRefNbr = soOrder.ExtRefNbr;
						//clear error in case invoice currency different from default cash account for customer
						SODocument.Cache.RaiseExceptionHandling<SOInvoice.cashAccountID>(SODocument.Current, null, null);
					}

					foreach (CurrencyInfo info in this.currencyinfo.Select())
					{
						if (((SOOrder)order).InvoiceDate != null)
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, (CurrencyInfo)order);
							info.CuryInfoID = newdoc.CuryInfoID;
						}
						else
						{
							info.CuryRateTypeID = ((CurrencyInfo)order).CuryRateTypeID;
						}
					}
					AddressAttribute.CopyRecord<ARInvoice.billAddressID>(this.Document.Cache, newdoc, (SOAddress)order, true);
					ContactAttribute.CopyRecord<ARInvoice.billContactID>(this.Document.Cache, newdoc, (SOContact)order, true);					
				}
			}
			else
			{
				newdoc = (ARInvoice)Document.Cache.CreateCopy(Document.Current);

                if (Transactions.SelectSingle() == null)
                {
                    newdoc.CustomerID = ((SOOrder)order).CustomerID;
                    newdoc.ProjectID = ((SOOrder)order).ProjectID;
                    newdoc.CustomerLocationID = ((SOOrder)order).CustomerLocationID;
                    newdoc.SalesPersonID = ((SOOrder)order).SalesPersonID;
                    newdoc.TaxZoneID = ((SOOrder)order).TaxZoneID;
                    newdoc.AvalaraCustomerUsageType = ((SOOrder)order).AvalaraCustomerUsageType;
                    newdoc.DocDesc = ((SOOrder)order).OrderDesc;
                    newdoc.InvoiceNbr = ((SOOrder)order).CustomerOrderNbr;
                    newdoc.TermsID = ((SOOrder)order).TermsID;

					foreach (CurrencyInfo info in this.currencyinfo.Select())
					{
						PXCache<CurrencyInfo>.RestoreCopy(info, (CurrencyInfo)order);
						info.CuryInfoID = newdoc.CuryInfoID;
						this.currencyinfo.Update(info);
						newdoc.CuryID = info.CuryID;
					}
                }

                newdoc = this.Document.Update(newdoc);

				

				AddressAttribute.CopyRecord<ARInvoice.billAddressID>(this.Document.Cache, newdoc, (SOAddress)order, true);
				ContactAttribute.CopyRecord<ARInvoice.billContactID>(this.Document.Cache, newdoc, (SOContact)order, true);								
			}

			PXSelectBase<SOInvoiceDiscountDetail> selectInvoiceDiscounts = new PXSelect<SOInvoiceDiscountDetail,
			Where<SOInvoiceDiscountDetail.tranType, Equal<Current<SOInvoice.docType>>,
			And<SOInvoiceDiscountDetail.refNbr, Equal<Current<SOInvoice.refNbr>>,
			And<SOInvoiceDiscountDetail.orderType, Equal<Required<SOInvoiceDiscountDetail.orderType>>,
			And<SOInvoiceDiscountDetail.orderNbr, Equal<Required<SOInvoiceDiscountDetail.orderNbr>>>>>>>(this);

			foreach (SOInvoiceDiscountDetail detail in selectInvoiceDiscounts.Select(((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
			{
			    DiscountDetails.Delete(detail);
			}

			TaxAttribute.SetTaxCalc<ARTran.taxCategoryID>(this.Transactions.Cache, null, TaxCalc.ManualCalc);

			if (details != null)
			{
                PXCache cache = this.Caches[typeof(SOShipLine)];
				foreach (SOShipLine shipline in details)
				{
                    //there should be no parent record of SOLineSplit2 type.
					cache.Insert(shipline);
				}
			}

			//DropShip Receipt/Shipment cannot be invoiced twice thats why we have to be sure that all SOPO links at this point in that Receipt are valid:

			PXSelectBase<POReceiptLine> selectUnlinkedDropShips = new PXSelectJoin<POReceiptLine,
				InnerJoin<PO.POLine, On<PO.POLine.orderType, Equal<POReceiptLine.pOType>, And<PO.POLine.orderNbr, Equal<POReceiptLine.pONbr>, And<PO.POLine.lineNbr, Equal<POReceiptLine.pOLineNbr>>>>,
				LeftJoin<SOLineSplit, On<SOLineSplit.pOType, Equal<POReceiptLine.pOType>, And<SOLineSplit.pONbr, Equal<POReceiptLine.pONbr>, And<SOLineSplit.pOLineNbr, Equal<POReceiptLine.pOLineNbr>>>>>>,
				Where<POReceiptLine.receiptType, Equal<PO.POReceiptType.poreceipt>, 
				And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
				And<SOLineSplit.pOLineNbr, IsNull,
				And<Where<POReceiptLine.lineType, Equal<POLineType.goodsForDropShip>, Or<POReceiptLine.lineType, Equal<POLineType.nonStockForDropShip>>>>>>>>(this);

			var rs = selectUnlinkedDropShips.Select(((SOOrderShipment)order).ShipmentNbr);
			if (rs.Count > 0)
			{
				foreach (POReceiptLine line in rs)
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, line.InventoryID);
					PXTrace.WriteError(Messages.SOPOLinkIsIvalidInPOOrder, line.PONbr, item?.InventoryCD);
				}

				throw new PXException(Messages.SOPOLinkIsIvalid);
			}


			DateTime? origInvoiceDate = null;
			bool updateINRequired = false;
			List<ARTranComparable> tranList = new List<ARTranComparable>();
            //And<ARTran.sOShipmentLineNbr, Equal<SOShipLine.lineNbr>, 
            foreach (PXResult<SOShipLine, SOLine, SOOrderTypeOperation, ARTran> res in PXSelectJoin<SOShipLine, InnerJoin<SOLine, On<SOLine.orderType, Equal<SOShipLine.origOrderType>, And<SOLine.orderNbr, Equal<SOShipLine.origOrderNbr>, And<SOLine.lineNbr, Equal<SOShipLine.origLineNbr>>>>, InnerJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.orderType, Equal<SOLine.orderType>, And<SOOrderTypeOperation.operation, Equal<SOLine.operation>>>, LeftJoin<ARTran, On<ARTran.sOShipmentNbr, Equal<SOShipLine.shipmentNbr>, And<ARTran.sOShipmentType, Equal<SOShipLine.shipmentType>, And<ARTran.sOOrderType, Equal<SOShipLine.origOrderType>, And<ARTran.sOOrderNbr, Equal<SOShipLine.origOrderNbr>, And<ARTran.sOOrderLineNbr, Equal<SOShipLine.origLineNbr>>>>>>>>>, Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>, And<SOShipLine.origOrderType, Equal<Required<SOShipLine.origOrderType>>, And<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>>>>>.Select(this, ((SOOrderShipment)order).ShipmentNbr, ((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
            {
                ARTran artran = (ARTran)res;
                if (artran.RefNbr == null || (artran.RefNbr != null && this.Transactions.Cache.GetStatus(artran) == PXEntryStatus.Deleted))

			{
				SOLine orderline = (SOLine)res;
				SOShipLine shipline = (SOShipLine)res;

                if (Math.Abs((decimal)shipline.BaseShippedQty) < 0.0000005m && !string.Equals(shipline.ShipmentNbr, Constants.NoShipmentNbr))
                {
                    continue;
                }

				if (origInvoiceDate == null && orderline.InvoiceDate != null)
					origInvoiceDate = orderline.InvoiceDate;

					ARTran newtran = CreateTranFromShipLine(newdoc, ordertype, ((SOOrderTypeOperation)res).Operation, orderline, ref shipline);

				tranList.Add(new ARTranComparable(newtran, orderline, null));
				}
            }

			PXSelectBase<ARTran> cmd = new PXSelect<ARTran, Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>, And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>, And<ARTran.sOOrderType, Equal<Current<SOMiscLine2.orderType>>, And<ARTran.sOOrderNbr, Equal<Current<SOMiscLine2.orderNbr>>, And<ARTran.sOOrderLineNbr, Equal<Current<SOMiscLine2.lineNbr>>>>>>>>(this);

			foreach (SOMiscLine2 orderline in PXSelect<SOMiscLine2, Where<SOMiscLine2.orderType, Equal<Required<SOMiscLine2.orderType>>, And<SOMiscLine2.orderNbr, Equal<Required<SOMiscLine2.orderNbr>>, And<Where<SOMiscLine2.curyUnbilledAmt, Greater<decimal0>, Or<SOMiscLine2.curyLineAmt, LessEqual<decimal0>>>>>>>.Select(this, ((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
			{
				if (cmd.View.SelectSingleBound(new object[] { Document.Current, orderline }) == null)
				{
					ARTran newtran = CreateTranFromMiscLine((SOOrderShipment)order, orderline);

					tranList.Add(new ARTranComparable(newtran, null, orderline));
				}
			}

			tranList.Sort();

			foreach (ARTranComparable ts in tranList)
			{
				if (ts.MiscLine != null)
				{
					if (this.Document.Current != null && ((this.Document.Current.CuryLineTotal ?? 0m) + (ts.Tran.CuryLineAmt ?? 0m)) < 0m)
						continue;

					ARTran newtran = this.Transactions.Insert(ts.Tran);

					PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(SOMiscLine2)], ts.MiscLine, Caches[typeof(ARTran)], newtran,
						((SOOrderType)order).CopyLineNotesToInvoice, ((SOOrderType)order).CopyLineFilesToInvoice);
				}
				else if (ts.OrderLine != null)
				{
					ARTran newtran = ts.Tran;
					foreach (ARTran existing in Transactions.Cache.Inserted)
					{
						if (Transactions.Cache.ObjectsEqual<ARTran.sOShipmentNbr, ARTran.sOShipmentType, ARTran.sOOrderType, ARTran.sOOrderNbr, ARTran.sOOrderLineNbr>(newtran, existing))
						{
							Transactions.Cache.RestoreCopy(newtran, existing);
							break;
						}
					}

                    foreach (ARTran existing in Transactions.Cache.Updated)
                    {
                        if (Transactions.Cache.ObjectsEqual<ARTran.sOShipmentNbr, ARTran.sOShipmentType, ARTran.sOOrderType, ARTran.sOOrderNbr, ARTran.sOOrderLineNbr>(newtran, existing))
                        {
                            Transactions.Cache.RestoreCopy(newtran, existing);
                            break;
                        }
                    }

					if (newtran.LineNbr == null)
					{
                        try
                        {
                            cancelUnitPriceCalculation = true;
                            newtran = this.Transactions.Insert(newtran);
                        }
						catch (PXSetPropertyException e)
                        {
                            throw new PXErrorContextProcessingException(this, PXParentAttribute.SelectParent(this.Caches[typeof(ARTran)], newtran, typeof(SOLine2)), e);
                        }
                        finally
                        {
                            cancelUnitPriceCalculation = false;
						}

                        SOOrderType orderType = order;
						PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(SOLine)], ts.OrderLine, Caches[typeof(ARTran)], newtran,
							orderType.CopyLineNotesToInvoice == true && (orderType.CopyLineNotesToInvoiceOnlyNS == false || ts.OrderLine.LineType == SOLineType.NonInventory),
							orderType.CopyLineFilesToInvoice == true && (orderType.CopyLineFilesToInvoiceOnlyNS == false || ts.OrderLine.LineType == SOLineType.NonInventory));
				}
					else
					{
						newtran = this.Transactions.Update(newtran);
						TaxAttribute.Calculate<ARTran.taxCategoryID>(Transactions.Cache, new PXRowUpdatedEventArgs(newtran, null, true));
			}

					if (newtran.LineType == SOLineType.Inventory && newtran.Qty != 0m)
						updateINRequired = true;
				}
			}

			SODocument.Current = (SOInvoice)SODocument.Select() ?? (SOInvoice)SODocument.Cache.Insert();
			SODocument.Current.BillAddressID = soOrder.BillAddressID;
			SODocument.Current.BillContactID = soOrder.BillContactID;

			if (SODocument.Current.ShipAddressID == null || SODocument.Current.ShipAddressID != soOrder.ShipAddressID)
			{
				if (SODocument.Current.ShipAddressID == null)
					SODocument.Current.ShipAddressID = ((SOOrderShipment)order).ShipAddressID;
				else if (SODocument.Current.ShipAddressID != ((SOOrderShipment)order).ShipAddressID)
					SODocument.Current.ShipAddressID = soOrder.ShipAddressID;
			}
			if (SODocument.Current.ShipContactID == null || SODocument.Current.ShipContactID != soOrder.ShipContactID)
			{
				if (SODocument.Current.ShipContactID == null)
					SODocument.Current.ShipContactID = ((SOOrderShipment)order).ShipContactID;
				else if (SODocument.Current.ShipContactID != ((SOOrderShipment)order).ShipContactID)
					SODocument.Current.ShipContactID = soOrder.ShipContactID;
			}

			SODocument.Current.IsCCCaptured = soOrder.IsCCCaptured;
			SODocument.Current.IsCCCaptureFailed = soOrder.IsCCCaptureFailed;
			SODocument.Current.PaymentProjectID = PM.ProjectDefaultAttribute.NonProject(this);
			
			if (soOrder.IsCCCaptured == true)
			{
				SODocument.Current.CuryCCCapturedAmt = soOrder.CuryCCCapturedAmt;
				SODocument.Current.CCCapturedAmt = soOrder.CCCapturedAmt;
			}

			SODocument.Current.RefTranExtNbr = soOrder.RefTranExtNbr;

			SOOrderShipment shipment = PXCache<SOOrderShipment>.CreateCopy((SOOrderShipment)order);
			shipment.InvoiceType = SODocument.Current.DocType;
			shipment.InvoiceNbr = SODocument.Current.RefNbr;
			shipment.CreateINDoc = updateINRequired;
            shipmentlist.Cache.Update(shipment);

            if (string.Equals(shipment.ShipmentNbr, Constants.NoShipmentNbr))
            {
                SOOrder cached = soorder.Locate(order);
                if (cached != null)
                {
                    if ((cached.Behavior == SOBehavior.SO || cached.Behavior == SOBehavior.RM) && cached.OpenLineCntr == 0)
                    {
                        cached.Completed = true;
                        cached.Status = SOOrderStatus.Completed;
                    }
                    cached.ShipmentCntr++;
                    soorder.Update(cached);
                }
            }

			FillFreightDetails((SOOrder)order, shipment);

            /*In case Discounts were not recalculated add prorated discounts */
            if (ordertype.RecalculateDiscOnPartialShipment != true)
            {
                //add prorated document discount details from invoice:
                PXSelectBase<SOOrderDiscountDetail> selectOrderDocGroupDiscounts = new PXSelect<SOOrderDiscountDetail,
                Where<SOOrderDiscountDetail.orderType, Equal<Required<SOOrderDiscountDetail.orderType>>,
                And<SOOrderDiscountDetail.orderNbr, Equal<Required<SOOrderDiscountDetail.orderNbr>>>>>(this);

                decimal? rate = 1m;
                if (soOrder.LineTotal > 0m && shipment.ShipmentType != SOShipmentType.DropShip)
                    rate = shipment.LineTotal / soOrder.LineTotal;

                Dictionary<DiscountSequenceKey, List<ARTran>> invoiceToOrderCorrelation = new Dictionary<DiscountSequenceKey, List<ARTran>>();

                foreach (SOOrderDiscountDetail docGroupDisc in selectOrderDocGroupDiscounts.Select(((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
                {
                    SOInvoiceDiscountDetail dd = new SOInvoiceDiscountDetail();
                    dd.SkipDiscount = docGroupDisc.SkipDiscount;
                    dd.Type = docGroupDisc.Type;
                    dd.DiscountID = docGroupDisc.DiscountID;
                    dd.DiscountSequenceID = docGroupDisc.DiscountSequenceID;
                    dd.OrderType = docGroupDisc.OrderType;
                    dd.OrderNbr = docGroupDisc.OrderNbr;
                    dd.TranType = newdoc.DocType;
                    dd.RefNbr = newdoc.RefNbr;
                    dd.IsManual = docGroupDisc.IsManual;
                    dd.DiscountPct = docGroupDisc.DiscountPct;
                    dd.FreeItemID = docGroupDisc.FreeItemID;
                    dd.FreeItemQty = docGroupDisc.FreeItemQty;

                    if (docGroupDisc.Type == DiscountType.Group)
                    {
                        Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<SOOrderDiscountDetail, SOLine>> grLinesOrderCorrelation = CollectGroupDiscountToLineCorrelation(soOrder);

                        foreach (KeyValuePair<DiscountSequenceKey, DiscountEngine<SOLine>.DiscountDetailToLineCorrelation<SOOrderDiscountDetail, SOLine>> dsGroup in grLinesOrderCorrelation)
                        {
                            if (dsGroup.Key.DiscountID == docGroupDisc.DiscountID && dsGroup.Key.DiscountSequenceID == docGroupDisc.DiscountSequenceID)
                            {
                                DiscountSequenceKey dsKey = new DiscountSequenceKey();
                                dsKey.DiscountID = dsGroup.Key.DiscountID;
                                dsKey.DiscountSequenceID = dsGroup.Key.DiscountSequenceID;
                                decimal invoicedGroupAmt = 0m;
                                foreach (SOLine soLine in dsGroup.Value.listOfApplicableLines)
                                {
                                    foreach (ARTran tran in Transactions.Select())
                                    {
                                        if (soLine.OrderNbr == tran.SOOrderNbr && soLine.LineNbr == tran.SOOrderLineNbr)
                                        {
                                            invoicedGroupAmt += (tran.CuryLineAmt ?? 0m);
                                            if (invoiceToOrderCorrelation.ContainsKey(dsKey))
                                            {
                                                invoiceToOrderCorrelation[dsKey].Add(tran);
                                            }
                                            else
                                            {
                                                invoiceToOrderCorrelation.Add(dsKey, new List<ARTran>());
                                                invoiceToOrderCorrelation[dsKey].Add(tran);
                                            }

                                        }
                                    }
                                }
                                rate = (invoicedGroupAmt / (decimal)dsGroup.Value.discountDetailLine.CuryDiscountableAmt);
                            }
                        }
                    }

                    SOInvoiceDiscountDetail located = DiscountDetails.Locate(dd);
                    //LineNbr prevents Locate() to work as intended. To review.
                    if (located == null)
                    {
                        foreach (SOInvoiceDiscountDetail detail in DiscountDetails.Cache.Cached)
                        {
                            if (detail.DiscountID == dd.DiscountID && detail.DiscountSequenceID == dd.DiscountSequenceID && detail.OrderType == dd.OrderType 
                                && detail.OrderNbr == dd.OrderNbr && detail.TranType == dd.TranType && detail.RefNbr == dd.RefNbr && detail.Type == dd.Type)
                                located = detail;
                        }
                    }
                    if (located != null)
                    {
                        if (docGroupDisc.Type == DiscountType.Group)
                        {
                            located.DiscountAmt = docGroupDisc.DiscountAmt * rate;
                            located.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
                            located.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
                            located.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
                            located.DiscountableQty = docGroupDisc.DiscountableQty * rate;
                        }
                        else
                        {
                            located.DiscountAmt += docGroupDisc.DiscountAmt * rate;
                            located.CuryDiscountAmt += docGroupDisc.CuryDiscountAmt * rate;
                            located.DiscountableAmt += docGroupDisc.DiscountableAmt * rate;
                            located.CuryDiscountableAmt += docGroupDisc.CuryDiscountableAmt * rate;
                            located.DiscountableQty += docGroupDisc.DiscountableQty * rate;
                        }
						if (DiscountDetails.Cache.GetStatus(located) == PXEntryStatus.Deleted)
							DiscountDetails.Insert(located);
						else
                        DiscountDetails.Update(located);
                    }
                    else
                    {
                        dd.DiscountAmt = docGroupDisc.DiscountAmt * rate;
                        dd.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
                        dd.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
                        dd.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
                        dd.DiscountableQty = docGroupDisc.DiscountableQty * rate;

                        DiscountDetails.Insert(dd);
                    }
                    if (soOrder.LineTotal > 0m)
                        rate = shipment.LineTotal / soOrder.LineTotal;
                }
                RecalculateTotalDiscount();

                PXSelectBase<ARTran> orderLinesSelect = new PXSelectJoin<ARTran, LeftJoin<SOLine,
                On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>>,
                Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
                And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
                And<ARTran.sOOrderType, Equal<Required<ARTran.sOOrderType>>,
                And<ARTran.sOOrderNbr, Equal<Required<ARTran.sOOrderNbr>>>>>>,
                OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>(this);

                PXSelectBase<SOInvoiceDiscountDetail> orderDiscountDetailsSelect = new PXSelect<SOInvoiceDiscountDetail, Where<SOInvoiceDiscountDetail.tranType, Equal<Current<SOInvoice.docType>>, And<SOInvoiceDiscountDetail.refNbr, Equal<Current<SOInvoice.refNbr>>,
                    And<SOInvoiceDiscountDetail.orderType, Equal<Required<SOInvoiceDiscountDetail.orderType>>, And<SOInvoiceDiscountDetail.orderNbr, Equal<Required<SOInvoiceDiscountDetail.orderNbr>>>>>>>(this);

                DiscountEngine<ARTran>.CalculateGroupDiscountRate<SOInvoiceDiscountDetail>(Transactions.Cache, orderLinesSelect, null, invoiceToOrderCorrelation, orderDiscountDetailsSelect, false, soOrder.OrderType, soOrder.OrderNbr);
                DiscountEngine<ARTran>.CalculateDocumentDiscountRate<SOInvoiceDiscountDetail>(Transactions.Cache, orderLinesSelect, null, orderDiscountDetailsSelect, (SODocument.Current.CuryLineTotal ?? 0m + SODocument.Current.CuryMiscTot ?? 0m), SODocument.Current.CuryDiscTot ?? 0m, soOrder.OrderType, soOrder.OrderNbr, true);
            }
            else
            {
                //Recalculate all discounts
                foreach (ARTran tran in Transactions.Select())
                {
                    RecalculateDiscounts(this.Transactions.Cache, tran);
                    this.Transactions.Update(tran);
                }
            }

			if (!IsExternalTax)
			{
				foreach (PXResult<SOTaxTran, Tax> res in PXSelectJoin<SOTaxTran,
				InnerJoin<Tax, On<SOTaxTran.taxID, Equal<Tax.taxID>>>,
				Where<SOTaxTran.orderType, Equal<Required<SOTaxTran.orderType>>,
					And<SOTaxTran.orderNbr, Equal<Required<SOTaxTran.orderNbr>>>>>.Select(this, ((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
				{
					SOTaxTran tax = (SOTaxTran)res;
					ARTaxTran newtax = new ARTaxTran();
					newtax.Module = BatchModule.AR;
					Taxes.Cache.SetDefaultExt<ARTaxTran.origTranType>(newtax);
					Taxes.Cache.SetDefaultExt<ARTaxTran.origRefNbr>(newtax);
					Taxes.Cache.SetDefaultExt<ARTaxTran.lineRefNbr>(newtax);
					newtax.TranType = Document.Current.DocType;
					newtax.RefNbr = Document.Current.RefNbr;
					newtax.TaxID = tax.TaxID;
					newtax.TaxRate = 0m;

					foreach (ARTaxTran existingTaxTran in this.Taxes.Cache.Cached.RowCast<ARTaxTran>().Where(a =>
						this.Taxes.Cache.GetStatus(a) != PXEntryStatus.Deleted &&
						this.Taxes.Cache.GetStatus(a) != PXEntryStatus.InsertedDeleted))
					{
						if (Taxes.Cache.ObjectsEqual<ARTaxTran.module>(newtax, existingTaxTran) && Taxes.Cache.ObjectsEqual<ARTaxTran.refNbr>(newtax, existingTaxTran)
							&& Taxes.Cache.ObjectsEqual<ARTaxTran.tranType>(newtax, existingTaxTran) && Taxes.Cache.ObjectsEqual<ARTaxTran.taxID>(newtax, existingTaxTran))
						{
							this.Taxes.Delete(existingTaxTran);
					}
					}

					newtax = this.Taxes.Insert(newtax);
				}
			}

			decimal? CuryApplAmt = 0m;
			bool Calculated = false;

			

			foreach (SOAdjust soadj in PXSelectJoin<SOAdjust, InnerJoin<AR.ARPayment, On<AR.ARPayment.docType, Equal<SOAdjust.adjgDocType>, And<AR.ARPayment.refNbr, Equal<SOAdjust.adjgRefNbr>>>>, Where<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>, And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>, And<AR.ARPayment.openDoc, Equal<True>>>>>.Select(this, ((SOOrderShipment)order).OrderType, ((SOOrderShipment)order).OrderNbr))
			{
				ARAdjust2 prev_adj = null;
				bool found = false;

				PXResultset<ARAdjust2> resultset = null;

				try
				{
					TransferApplicationFromSalesOrder = true;
					resultset = Adjustments.Select();
				}
				finally
				{
					TransferApplicationFromSalesOrder = false;
				}

				foreach (ARAdjust2 adj in resultset )
				{
					if (Calculated)
					{
						CuryApplAmt -= adj.CuryAdjdAmt;
					}

					if (string.Equals(adj.AdjgDocType, soadj.AdjgDocType) && string.Equals(adj.AdjgRefNbr, soadj.AdjgRefNbr))
					{
						if (soadj.CuryAdjdAmt > 0m)
						{
							ARAdjust2 copy = PXCache<ARAdjust2>.CreateCopy(adj);
							copy.CuryAdjdAmt += (soadj.CuryAdjdAmt > adj.CuryDocBal) ? adj.CuryDocBal : soadj.CuryAdjdAmt;
							copy.AdjdOrderType = soadj.AdjdOrderType;
							copy.AdjdOrderNbr = soadj.AdjdOrderNbr;
							prev_adj = Adjustments.Update(copy);
						}

						found = true;

						if (Calculated)
						{
							CuryApplAmt += adj.CuryAdjdAmt;
							break;
						}
					}

					CuryApplAmt += adj.CuryAdjdAmt;
				}

				//if soadjust is not available in adjustments mark as billed
				if (!found)
				{
				/*
					soadj.Billed = true;
					soadjustments.Cache.SetStatus(soadj, PXEntryStatus.Updated);
				*/
				}

				Calculated = true;

				if (!IsExternalTax)
				{
					if (CuryApplAmt > Document.Current.CuryDocBal - Document.Current.CuryOrigDiscAmt && prev_adj != null)
					{
						prev_adj = PXCache<ARAdjust2>.CreateCopy(prev_adj);

						if (prev_adj.CuryAdjdAmt > (CuryApplAmt - (Document.Current.CuryDocBal - Document.Current.CuryOrigDiscAmt)))
						{
							prev_adj.CuryAdjdAmt -= (CuryApplAmt - (Document.Current.CuryDocBal - Document.Current.CuryOrigDiscAmt));
							CuryApplAmt = Document.Current.CuryDocBal - Document.Current.CuryOrigDiscAmt;
						}
						else
						{
							CuryApplAmt -= prev_adj.CuryAdjdAmt;
							prev_adj.CuryAdjdAmt = 0m;
						}

						prev_adj = Adjustments.Update(prev_adj);
					}
				}
			}

			newdoc = (ARInvoice)Document.Cache.CreateCopy(Document.Current);
			newdoc.OrigDocDate = origInvoiceDate;
			SOInvoice socopy = (SOInvoice)SODocument.Cache.CreateCopy(SODocument.Current);
            
			PXFormulaAttribute.CalcAggregate<ARAdjust2.curyAdjdAmt>(Adjustments.Cache, SODocument.Current, false);
			Document.Cache.RaiseFieldUpdated<SOInvoice.curyPaymentTotal>(SODocument.Current, null);
			PXDBCurrencyAttribute.CalcBaseValues<SOInvoice.curyPaymentTotal>(SODocument.Cache, SODocument.Current);

			SODocument.Cache.RaiseRowUpdated(SODocument.Current, socopy);

			List<string> ordersdistinct = new List<string>();
			foreach (SOOrderShipment shipments in PXSelect<SOOrderShipment, Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>>.Select(this))
			{
				string key = string.Format("{0}|{1}", shipments.OrderType, shipments.OrderNbr);
				if (!ordersdistinct.Contains(key))
				{
					ordersdistinct.Add(key);
				}

				if (list != null && ordersdistinct.Count > 1)
				{
					newdoc.InvoiceNbr = null;
					newdoc.SalesPersonID = null;
					newdoc.DocDesc = null;
					break;
				}

				#region Update FreeItemQty for DiscountDetails based on shipments
				
				PXSelectBase<SOShipmentDiscountDetail> selectShipmentDiscounts = new PXSelect<SOShipmentDiscountDetail,
						Where<SOShipmentDiscountDetail.orderType, Equal<Required<SOShipmentDiscountDetail.orderType>>,
						And<SOShipmentDiscountDetail.orderNbr, Equal<Required<SOShipmentDiscountDetail.orderNbr>>,
						And<SOShipmentDiscountDetail.shipmentNbr, Equal<Required<SOShipmentDiscountDetail.shipmentNbr>>>>>>(this);

				foreach (SOShipmentDiscountDetail sdd in selectShipmentDiscounts.Select(shipments.OrderType, shipments.OrderNbr, shipments.ShipmentNbr))
				{
                    bool discountDetailLineExist = false;

                    foreach (SOInvoiceDiscountDetail idd in DiscountDetails.Select())
                    {
                        if (idd.TranType == newdoc.DocType && idd.RefNbr == newdoc.RefNbr
                            && idd.OrderType == shipments.OrderType && idd.OrderNbr == shipments.OrderNbr
                            && idd.DiscountID == sdd.DiscountID && idd.DiscountSequenceID == sdd.DiscountSequenceID)
					{
                            discountDetailLineExist = true;
						if (idd.FreeItemID == null)
						{
							idd.FreeItemID = sdd.FreeItemID;
							idd.FreeItemQty = sdd.FreeItemQty;
						}
						else
							idd.FreeItemQty = sdd.FreeItemQty;
					}
                    }

                    if (!discountDetailLineExist)
					{
                        SOInvoiceDiscountDetail idd = new SOInvoiceDiscountDetail();
                        idd.Type = DiscountType.Group;
						idd.TranType = newdoc.DocType;
						idd.RefNbr = newdoc.RefNbr;
						idd.OrderType = sdd.OrderType;
						idd.OrderNbr = sdd.OrderNbr;
						idd.DiscountID = sdd.DiscountID;
						idd.DiscountSequenceID = sdd.DiscountSequenceID;
						idd.FreeItemID = sdd.FreeItemID;
						idd.FreeItemQty = sdd.FreeItemQty;

						DiscountDetails.Insert(idd);
					}
				} 

				#endregion
			}

            this.Document.Update(newdoc);

			if (list != null)
			{
				if (Transactions.Search<ARTran.sOOrderType, ARTran.sOOrderNbr, ARTran.sOShipmentType, ARTran.sOShipmentNbr>(shipment.OrderType, shipment.OrderNbr, shipment.ShipmentType, shipment.ShipmentNbr).Count > 0)
				{
					try
					{
						this.Document.Current.ApplyPaymentWhenTaxAvailable = true;
						this.Save.Press();
					}
					finally
					{
						this.Document.Current.ApplyPaymentWhenTaxAvailable = false;
					}


					if (list.Find(this.Document.Current) == null)
					{
						list.Add(this.Document.Current, this.SODocument.Current);
					}
				}
				else
				{
					this.Clear();
				}
			}
            CustomerCreditHelper.RemovePreUpdatedEvent(typeof(ARInvoice), ApprovedBalanceCollector);
		}

		protected virtual ARTran CreateTranFromMiscLine(SOOrderShipment orderShipment, SOMiscLine2 orderline)
		{
			ARTran newtran = new ARTran();
			newtran.BranchID = orderline.BranchID;
			newtran.AccountID = orderline.SalesAcctID;
			newtran.SubID = orderline.SalesSubID;
			newtran.SOOrderType = orderline.OrderType;
			newtran.SOOrderNbr = orderline.OrderNbr;
			newtran.SOOrderLineNbr = orderline.LineNbr;
			newtran.SOShipmentNbr = orderShipment.ShipmentNbr;
			newtran.SOShipmentType = orderShipment.ShipmentType;
			newtran.SOShipmentLineNbr = null;

			newtran.LineType = SOLineType.MiscCharge;
			newtran.InventoryID = orderline.InventoryID;
			newtran.TaskID = orderline.TaskID;
			newtran.SalesPersonID = orderline.SalesPersonID;
			newtran.Commissionable = orderline.Commissionable;
			newtran.UOM = orderline.UOM;
			newtran.Qty = orderline.UnbilledQty;
			newtran.BaseQty = orderline.BaseUnbilledQty;
			newtran.CuryUnitPrice = orderline.CuryUnitPrice;
			newtran.CuryDiscAmt = orderline.CuryDiscAmt;
			newtran.CuryTranAmt = orderline.CuryUnbilledAmt;
			newtran.TranDesc = orderline.TranDesc;
			newtran.TaxCategoryID = orderline.TaxCategoryID;
			newtran.DiscPct = orderline.DiscPct;

			newtran.IsFree = orderline.IsFree;
			newtran.ManualPrice = true;
			newtran.ManualDisc = orderline.ManualDisc == true || orderline.IsFree == true;
			newtran.FreezeManualDisc = true;

			newtran.DiscountID = orderline.DiscountID;
			newtran.DiscountSequenceID = orderline.DiscountSequenceID;

			newtran.DetDiscIDC1 = orderline.DetDiscIDC1;
			newtran.DetDiscIDC2 = orderline.DetDiscIDC2;
			newtran.DetDiscSeqIDC1 = orderline.DetDiscSeqIDC1;
			newtran.DetDiscSeqIDC2 = orderline.DetDiscSeqIDC2;
			newtran.DetDiscApp = orderline.DetDiscApp;
			newtran.DocDiscIDC1 = orderline.DocDiscIDC1;
			newtran.DocDiscIDC2 = orderline.DocDiscIDC2;
			newtran.DocDiscSeqIDC1 = orderline.DocDiscSeqIDC1;
			newtran.DocDiscSeqIDC2 = orderline.DocDiscSeqIDC2;

			newtran.DRTermStartDate = orderline.DRTermStartDate;
			newtran.DRTermEndDate = orderline.DRTermEndDate;
			newtran.CuryUnitPriceDR = orderline.CuryUnitPriceDR;
			newtran.DiscPctDR = orderline.DiscPctDR;
			newtran.DefScheduleID = orderline.DefScheduleID;
			return newtran;
		}

		protected virtual ARTran CreateTranFromShipLine(ARInvoice newdoc, SOOrderType ordertype, string operation, SOLine orderline, ref SOShipLine shipline)
		{
			ARTran newtran = new ARTran();
			newtran.BranchID = orderline.BranchID;
			newtran.AccountID = orderline.SalesAcctID;
			newtran.SubID = orderline.SalesSubID;
			newtran.SOOrderType = shipline.OrigOrderType;
			newtran.SOOrderNbr = shipline.OrigOrderNbr;
			newtran.SOOrderLineNbr = shipline.OrigLineNbr;
			newtran.SOShipmentNbr = shipline.ShipmentNbr;
			newtran.SOShipmentType = shipline.ShipmentType;
			newtran.SOShipmentLineNbr = shipline.LineNbr;

			newtran.LineType = orderline.LineType;
			newtran.InventoryID = shipline.InventoryID;
			newtran.SiteID = orderline.SiteID;
			newtran.ReasonCode = orderline.ReasonCode;
			newtran.UOM = shipline.UOM;

			newtran.DRTermStartDate = orderline.DRTermStartDate;
			newtran.DRTermEndDate = orderline.DRTermEndDate;
			newtran.CuryUnitPriceDR = orderline.CuryUnitPriceDR;
			newtran.DiscPctDR = orderline.DiscPctDR;
			newtran.DefScheduleID = orderline.DefScheduleID;

			foreach (SOShipLine other in this.Caches[typeof(SOShipLine)].Cached)
			{
				if (this.Caches[typeof(SOShipLine)].ObjectsEqual<SOShipLine.shipmentNbr, SOShipLine.shipmentType, SOShipLine.origOrderType, SOShipLine.origOrderNbr, SOShipLine.origLineNbr>(shipline, other) && shipline.LineNbr != other.LineNbr)
				{
					shipline = PXCache<SOShipLine>.CreateCopy(shipline);
					shipline.Qty += other.ShippedQty;
					shipline.BaseShippedQty += other.BaseShippedQty;

					newtran.SOShipmentLineNbr = null;
				}
			}

			newtran.Qty = shipline.ShippedQty;
			newtran.BaseQty = shipline.BaseShippedQty;

			newtran.Commissionable = orderline.Commissionable;
			newtran.GroupDiscountRate = orderline.GroupDiscountRate;
			newtran.DocumentDiscountRate = orderline.DocumentDiscountRate;

			decimal shippedQtyInBaseUnits = 0m;
			decimal shippedQtyInOrderUnits = 0m;

			try
			{
				shippedQtyInBaseUnits = INUnitAttribute.ConvertToBase(Transactions.Cache, newtran.InventoryID, shipline.UOM, shipline.ShippedQty.Value, INPrecision.QUANTITY);
				shippedQtyInOrderUnits = INUnitAttribute.ConvertFromBase(Transactions.Cache, newtran.InventoryID, orderline.UOM, shippedQtyInBaseUnits, INPrecision.QUANTITY);
			}
			catch (PXSetPropertyException e)
			{
				throw new PXErrorContextProcessingException(this, orderline, e);
			}

			bool useLineDiscPct = ordertype.RecalculateDiscOnPartialShipment != true || orderline.ManualDisc == true;

			if (shippedQtyInOrderUnits != orderline.OrderQty || shipline.UOM != orderline.UOM)
			{
				decimal curyUnitPriceInOrderUnits = orderline.CuryUnitPrice.Value;
				if (orderline.CuryUnitPrice == 0 && orderline.CuryLineAmt != 0 && orderline.OrderQty != 0)
				{
					curyUnitPriceInOrderUnits = PXCurrencyAttribute.Round(Transactions.Cache, newtran, orderline.CuryLineAmt.Value / orderline.OrderQty.Value, CMPrecision.TRANCURY);
				}

				decimal curyUnitPriceInBaseUnits = INUnitAttribute.ConvertFromBase(Transactions.Cache, newtran.InventoryID, orderline.UOM, curyUnitPriceInOrderUnits, INPrecision.UNITCOST);
				decimal curyUnitPriceInShippedUnits = INUnitAttribute.ConvertToBase(Transactions.Cache, newtran.InventoryID, shipline.UOM, curyUnitPriceInBaseUnits, INPrecision.UNITCOST);

				if (arsetup.Current.LineDiscountTarget == LineDiscountTargetType.SalesPrice)
				{
					decimal? salesPriceAfterDiscount = curyUnitPriceInShippedUnits * (useLineDiscPct ? (1m - orderline.DiscPct / 100m) : 1m);
					decimal? curyTranAmt = shipline.ShippedQty * PXCurrencyAttribute.Round(Transactions.Cache, newtran, salesPriceAfterDiscount ?? 0, CMPrecision.TRANCURY);
					newtran.CuryTranAmt = PXCurrencyAttribute.Round(Transactions.Cache, newtran, curyTranAmt ?? 0m, CMPrecision.TRANCURY);
				}
				else
				{
					decimal? curyTranAmt = shipline.ShippedQty * curyUnitPriceInShippedUnits * (useLineDiscPct ? (1m - orderline.DiscPct / 100m) : 1m);
					newtran.CuryTranAmt = PXCurrencyAttribute.Round(Transactions.Cache, newtran, curyTranAmt ?? 0m, CMPrecision.TRANCURY);
				}

				if (orderline.CuryUnitPrice != 0)
					newtran.CuryUnitPrice = curyUnitPriceInShippedUnits;
				newtran.CuryDiscAmt = (shipline.ShippedQty * curyUnitPriceInShippedUnits) - newtran.CuryTranAmt;
			}
			else
			{
				newtran.CuryUnitPrice = orderline.CuryUnitPrice;
				newtran.CuryTranAmt = orderline.CuryLineAmt;
				newtran.CuryDiscAmt = orderline.CuryDiscAmt;
			}

			if (newdoc.DocType == ordertype.ARDocType && ordertype.DefaultOperation != operation)
			{
				//keep BaseQty positive for PXFormula
				newtran.Qty = -newtran.Qty;
				newtran.CuryDiscAmt = -newtran.CuryDiscAmt;
				newtran.CuryTranAmt = -newtran.CuryTranAmt;
			}

			newtran.ProjectID = orderline.ProjectID;
			newtran.TaskID = orderline.TaskID;
			newtran.TranDesc = orderline.TranDesc;
			newtran.SalesPersonID = orderline.SalesPersonID;
			newtran.TaxCategoryID = orderline.TaxCategoryID;
			newtran.DiscPct = (useLineDiscPct ? orderline.DiscPct : 0m);

			newtran.IsFree = orderline.IsFree;
			newtran.ManualPrice = true;
			newtran.ManualDisc = orderline.ManualDisc == true || orderline.IsFree == true;
			newtran.FreezeManualDisc = true;

			newtran.DiscountID = orderline.DiscountID;
			newtran.DiscountSequenceID = orderline.DiscountSequenceID;

			newtran.DetDiscIDC1 = orderline.DetDiscIDC1;
			newtran.DetDiscIDC2 = orderline.DetDiscIDC2;
			newtran.DetDiscSeqIDC1 = orderline.DetDiscSeqIDC1;
			newtran.DetDiscSeqIDC2 = orderline.DetDiscSeqIDC2;
			newtran.DetDiscApp = orderline.DetDiscApp;
			newtran.DocDiscIDC1 = orderline.DocDiscIDC1;
			newtran.DocDiscIDC2 = orderline.DocDiscIDC2;
			newtran.DocDiscSeqIDC1 = orderline.DocDiscSeqIDC1;
			newtran.DocDiscSeqIDC2 = orderline.DocDiscSeqIDC2;
			return newtran;
		}

		public virtual void PostInvoice(INIssueEntry docgraph, ARInvoice invoice, DocumentList<INRegister> list)
		{
			SOOrderEntry oe = null;
			SOShipmentEntry se = null;

			foreach (PXResult<SOOrderShipment, SOOrder> res in PXSelectJoin<SOOrderShipment, InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>>, Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>>.SelectMultiBound(this, new object[] { invoice }))
			{
				if (((SOOrderShipment)res).ShipmentType == SOShipmentType.DropShip)
				{
					if (se == null)
					{
						se = PXGraph.CreateInstance<SOShipmentEntry>();
					}
					else
					{
						se.Clear();
					}
                    se.PostReceipt(docgraph, res, invoice, list);
				}
				else if (string.Equals(((SOOrderShipment)res).ShipmentNbr, Constants.NoShipmentNbr))
				{
					if (oe == null)
					{
						oe = PXGraph.CreateInstance<SOOrderEntry>();
					}
					else
					{
						oe.Clear();
					}
					oe.PostOrder(docgraph, (SOOrder)res, list, (SOOrderShipment)res);
				}
				else
				{
					if (se == null)
					{
						se = PXGraph.CreateInstance<SOShipmentEntry>();

						se.Caches[typeof(SiteStatus)] = docgraph.Caches[typeof(SiteStatus)];
						se.Caches[typeof(LocationStatus)] = docgraph.Caches[typeof(LocationStatus)];
						se.Caches[typeof(LotSerialStatus)] = docgraph.Caches[typeof(LotSerialStatus)];
						se.Caches[typeof(SiteLotSerial)] = docgraph.Caches[typeof(SiteLotSerial)];
						se.Caches[typeof(ItemLotSerial)] = docgraph.Caches[typeof(ItemLotSerial)];

						se.Views.Caches.Remove(typeof(SiteStatus));
						se.Views.Caches.Remove(typeof(LocationStatus));
						se.Views.Caches.Remove(typeof(LotSerialStatus));
						se.Views.Caches.Remove(typeof(SiteLotSerial));
						se.Views.Caches.Remove(typeof(ItemLotSerial));
					}
					else
					{
						se.Clear();
					}
					se.PostShipment(docgraph, res, list, invoice);
				}
			}		
		}

		public virtual void DefaultDiscountAccountAndSubAccount(ARTran tran)
		{
			ARTran firstTranWithOrderType = PXSelect<ARTran, Where<ARTran.tranType, Equal<Current<SOInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<SOInvoice.refNbr>>,
				And<ARTran.sOOrderType, IsNotNull>>>>.Select(this);

			if (firstTranWithOrderType != null)
			{
				SOOrderType type = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>>.Select(this, firstTranWithOrderType.SOOrderType);

				if (type != null)
				{
					Location customerloc = location.Current;
					CRLocation companyloc =
						PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>>, Where<Branch.branchID, Equal<Current<ARRegister.branchID>>>>.Select(this);

					switch (type.DiscAcctDefault)
					{
						case SODiscAcctSubDefault.OrderType:
							tran.AccountID = (int?)GetValue<SOOrderType.discountAcctID>(type);
							break;
						case SODiscAcctSubDefault.MaskLocation:
							tran.AccountID = (int?)GetValue<Location.cDiscountAcctID>(customerloc);
							break;
					}


					if (tran.AccountID == null)
					{
						tran.AccountID = type.DiscountAcctID;
					}

					Discount.Cache.RaiseFieldUpdated<ARTran.accountID>(tran, null);

					if (tran.AccountID != null)
					{
						object ordertype_SubID = GetValue<SOOrderType.discountSubID>(type);
						object customer_Location = GetValue<Location.cDiscountSubID>(customerloc);
						object company_Location = GetValue<CRLocation.cMPDiscountSubID>(companyloc);

						object value = SODiscSubAccountMaskAttribute.MakeSub<SOOrderType.discSubMask>(this, type.DiscSubMask,
								new object[] { ordertype_SubID, customer_Location, company_Location },
								new Type[] { typeof(SOOrderType.discountSubID), typeof(Location.cDiscountSubID), typeof(Location.cMPDiscountSubID) });

						Discount.Cache.RaiseFieldUpdating<ARTran.subID>(tran, ref value);

						tran.SubID = (int?)value;
					}
				}
			}

		}

		#region Freight
		public virtual void FillFreightDetails(SOOrder order, SOOrderShipment ordershipment)
		{
            SOFreightDetail freightdet;

			if (ordershipment.ShipmentType == SOShipmentType.DropShip && order.CuryFreightTot != 0m)
			{
				freightdet = PXSelect<SOFreightDetail, Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>,
					And<SOFreightDetail.refNbr, Equal<Current<SOFreightDetail.refNbr>>,
					And<SOFreightDetail.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>,
					And<SOFreightDetail.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>>>>>>.SelectSingleBound(this, new object[] { ordershipment });

                if (freightdet == null)
                {
                    freightdet = new SOFreightDetail();
                    freightdet.CuryInfoID = Document.Current.CuryInfoID;
                    freightdet.ShipmentNbr = ordershipment.ShipmentNbr;
                    freightdet.ShipmentType = ordershipment.ShipmentType;
					if ((PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>>>>.SelectSingleBound(this, new object[] { ordershipment })).Count == 1)
					{
						freightdet.OrderType = ordershipment.OrderType;
						freightdet.OrderNbr = ordershipment.OrderNbr;
					}
                    freightdet.ProjectID = order.ProjectID;

                    freightdet = FreightDetails.Insert(freightdet);
                }

                freightdet = PXCache<SOFreightDetail>.CreateCopy(freightdet);

                freightdet.ShipTermsID = order.ShipTermsID; //!!
                freightdet.ShipVia = order.ShipVia; //!!
                freightdet.ShipZoneID = order.ShipZoneID; //!!
                freightdet.TaxCategoryID = order.FreightTaxCategoryID;

				decimal baseQtyRcpRate = 1m;
				decimal curyDropShipLineAmt = 0m;

				//prorate by base receipted qty and then by amount
				foreach (PXResult<SOLine, SOLineSplit, PO.POLine, POReceiptLine> res in PXSelectJoin<SOLine,
					InnerJoin<SOLineSplit, On<SOLineSplit.orderType, Equal<SOLine.orderType>, And<SOLineSplit.orderNbr, Equal<SOLine.orderNbr>, And<SOLineSplit.lineNbr, Equal<SOLine.lineNbr>>>>,
					InnerJoin<PO.POLine, On<PO.POLine.orderType, Equal<SOLineSplit.pOType>, And<PO.POLine.orderNbr, Equal<SOLineSplit.pONbr>, And<PO.POLine.lineNbr, Equal<SOLineSplit.pOLineNbr>>>>,
					InnerJoin<POReceiptLine, On<POReceiptLine.pOLineNbr, Equal<PO.POLine.lineNbr>, And<POReceiptLine.pONbr, Equal<PO.POLine.orderNbr>, And<POReceiptLine.pOType, Equal<PO.POLine.orderType>>>>>>>,
					Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
					And<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>>>.Select(this, ordershipment.ShipmentNbr, ordershipment.OrderType, ordershipment.OrderNbr))
                {
					SOLine soline = (SOLine)res;
					POReceiptLine pOReceiptline = (POReceiptLine)res;

					if ((pOReceiptline.BaseReceiptQty ?? 0m) > 0m && (soline.BaseOrderQty ?? 0m) > 0m && sosetup.Current.FreightAllocation == FreightAllocationList.Prorate)
						baseQtyRcpRate = (decimal)(pOReceiptline.BaseReceiptQty / soline.BaseOrderQty);
					curyDropShipLineAmt += (soline.CuryLineAmt ?? 0m) * baseQtyRcpRate;
                }

				decimal? ShipRatio = order.CuryLineTotal > 0m ? (curyDropShipLineAmt / order.CuryLineTotal) : 1m;
				freightdet.CuryLineTotal += PXDBCurrencyAttribute.Round(FreightDetails.Cache, freightdet, (decimal)(order.CuryLineTotal * (ShipRatio > 1m ? 1m : ShipRatio) ?? 0m), CMPrecision.TRANCURY);
				freightdet.CuryFreightCost += PXDBCurrencyAttribute.Round(FreightDetails.Cache, freightdet, (decimal)(order.CuryFreightCost * (ShipRatio > 1m ? 1m : ShipRatio) ?? 0m), CMPrecision.TRANCURY);
				freightdet.CuryFreightAmt += PXDBCurrencyAttribute.Round(FreightDetails.Cache, freightdet, (decimal)(order.CuryFreightAmt * (ShipRatio > 1m ? 1m : ShipRatio) ?? 0m), CMPrecision.TRANCURY);

				if (sosetup.Current.FreightAllocation == FreightAllocationList.Prorate)
                {
					//we cannot handle weight and volume the same way - we need separate rates for them
					freightdet.CuryPremiumFreightAmt += PXDBCurrencyAttribute.Round(FreightDetails.Cache, freightdet, (decimal)(order.CuryPremiumFreightAmt * (ShipRatio > 1m ? 1m : ShipRatio) ?? 0m), CMPrecision.TRANCURY);
				}
				else
				{
					SOOrderShipment prev_shipment = PXSelectReadonly<SOOrderShipment,
						Where2<Where<SOOrderShipment.shipmentNbr, NotEqual<Current<SOOrderShipment.shipmentNbr>>,
						Or<SOOrderShipment.shipmentType, NotEqual<SOShipmentType.dropShip>>>,
						And<SOOrderShipment.orderType, Equal<Current<SOOrderShipment.orderType>>,
						And<SOOrderShipment.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>,
						And<SOOrderShipment.invoiceNbr, IsNotNull/*,
						And<SOFreightDetail.curyPremiumFreightAmt, NotEqual<decimal0>>*/>>>>>.SelectSingleBound(this, new object[] { ordershipment });

					if (prev_shipment == null)
						freightdet.CuryPremiumFreightAmt += order.CuryPremiumFreightAmt;
				}

				Guid? ownerGuid = null;
				if (freightdet.OrderNbr != null)
					ownerGuid = order.OwnerID;

				if (freightdet.AccountID == null)
				{
					int? accountID;
					object subID;
					GetFreightAccountAndSubAccount(order, order.ShipVia, ownerGuid, out accountID, out subID);
					freightdet.AccountID = accountID;
					FreightDetails.Cache.RaiseFieldUpdating<SOFreightDetail.subID>(freightdet, ref subID);
					freightdet.SubID = (int?)subID;
                }

                freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;
                FreightDetails.Update(freightdet);

				//calculate freight amount diffrence in case order is fully invoiced
				if (order.OpenLineCntr == 0m)
				{
					PXResultset<SOFreightDetail> freightDetails = PXSelect<SOFreightDetail, Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>, And<SOFreightDetail.refNbr, Equal<Current<ARInvoice.refNbr>>, And<SOFreightDetail.orderType, Equal<Required<SOFreightDetail.orderType>>, And<SOFreightDetail.orderNbr, Equal<Required<SOFreightDetail.orderNbr>>>>>>>.Select(this, ordershipment.OrderType, ordershipment.OrderNbr);
					PXResultset<SOOrderShipment> orderShipments = PXSelect<SOOrderShipment, Where<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>>.Select(this, ordershipment.OrderType, ordershipment.OrderNbr);
					HashSet<string> distinctShipmentTypes = new HashSet<string>(orderShipments.FirstTableItems.Select(_ => _.ShipmentType));

					//if only DropShipments were processed
					if (freightDetails.Count > 1 && freightDetails.Count == orderShipments.Count && distinctShipmentTypes.Count() == 1)
					{
						decimal totalInvoicedFreight = 0m;
						foreach (SOFreightDetail freightDetail in freightDetails)
						{
							totalInvoicedFreight += freightDetail.CuryFreightAmt ?? 0m;
						}

						decimal freightDiff = (order.CuryFreightAmt ?? 0m) - totalInvoicedFreight;
						if (freightDiff != 0m)
						{
							freightdet.CuryFreightAmt += freightDiff;

							freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;
							FreightDetails.Update(freightdet);
						}
					}

					if (freightDetails.Count > 1 && freightDetails.Count == orderShipments.Count)
					{
						decimal totalInvoicedPremiumFreight = 0m;
						foreach (SOFreightDetail freightDetail in freightDetails)
						{
							totalInvoicedPremiumFreight += freightDetail.CuryPremiumFreightAmt ?? 0m;
						}

						decimal premiumFreightDiff = (order.CuryPremiumFreightAmt ?? 0m) - totalInvoicedPremiumFreight;
						if (premiumFreightDiff != 0m)
						{
							freightdet.CuryPremiumFreightAmt += premiumFreightDiff;

							freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;
							FreightDetails.Update(freightdet);
						}
					}
				}

                if (freightdet.CuryTotalFreightAmt > 0 && freightdet.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, freightdet.ProjectID))
                {
                    Account ac = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, freightdet.AccountID);
                    throw new PXException(Messages.TaskWasNotAssigned, ac.AccountCD);
                }
                return;
			}
			
			if (string.Equals(ordershipment.ShipmentNbr, Constants.NoShipmentNbr))
			{
				freightdet = PXSelect<SOFreightDetail, Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>, And<SOFreightDetail.refNbr, Equal<Current<SOFreightDetail.refNbr>>, And<SOFreightDetail.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>>>>>.SelectSingleBound(this, new object[] { ordershipment });

				if (freightdet == null)
				{
					freightdet = new SOFreightDetail();
					freightdet.CuryInfoID = Document.Current.CuryInfoID;
					freightdet.ShipmentNbr = ordershipment.ShipmentNbr;
					freightdet.ShipmentType = ordershipment.ShipmentType;
                    freightdet.OrderType = ordershipment.OrderType;
                    freightdet.OrderNbr = ordershipment.OrderNbr;
					freightdet.ProjectID = order.ProjectID;


					freightdet = FreightDetails.Insert(freightdet);
				}

				freightdet = PXCache<SOFreightDetail>.CreateCopy(freightdet);

				freightdet.ShipTermsID = order.ShipTermsID; //!!
				freightdet.ShipVia = order.ShipVia; //!!
				freightdet.ShipZoneID = order.ShipZoneID; //!!
				freightdet.TaxCategoryID = order.FreightTaxCategoryID;

				freightdet.Weight = 0m;
				freightdet.Volume = 0m;
				freightdet.CuryLineTotal = 0m;
				freightdet.CuryFreightCost = 0m;
				freightdet.CuryFreightAmt = 0m;
				freightdet.CuryPremiumFreightAmt = 0m;

				Guid? ownerGuid = null;
				bool firstIteration = true;

				foreach (PXResult<SOOrderShipment, SOOrder> res in PXSelectJoin<SOOrderShipment, InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>>, Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>, And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>, And<SOOrderShipment.shipmentNbr, Equal<Constants.noShipmentNbr>>>>>.SelectMultiBound(this, new object[] { ordershipment }))
				{
					SOOrder order2 = (SOOrder)res;
					freightdet.Weight += order2.OrderWeight;
					freightdet.Volume += order2.OrderVolume;
					freightdet.CuryLineTotal += order2.CuryLineTotal;
					freightdet.CuryFreightCost += order2.CuryFreightCost;
					freightdet.CuryFreightAmt += order2.CuryFreightAmt;
					freightdet.CuryPremiumFreightAmt += order2.CuryPremiumFreightAmt;
					if (firstIteration)
					{
						ownerGuid = order2.OwnerID;
						firstIteration = false;
					}
					else if (ownerGuid != null && ownerGuid == order2.OwnerID)
						ownerGuid = order2.OwnerID;
					else
						ownerGuid = null;

				}

				if (freightdet.AccountID == null)
				{
					int? accountID;
					object subID;
					GetFreightAccountAndSubAccount(order, order.ShipVia, ownerGuid, out accountID, out subID);
					freightdet.AccountID = accountID;
					FreightDetails.Cache.RaiseFieldUpdating<SOFreightDetail.subID>(freightdet, ref subID);
					freightdet.SubID = (int?)subID;
				}

				freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;
				FreightDetails.Update(freightdet);

				if (freightdet.CuryTotalFreightAmt > 0 && freightdet.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, freightdet.ProjectID))
				{
					Account ac = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, freightdet.AccountID);
					throw new PXException(Messages.TaskWasNotAssigned, ac.AccountCD);
				}

			}
			else if (ordershipment.ShipmentType != SOShipmentType.DropShip)
			{
				freightdet = PXSelect<SOFreightDetail, Where<SOFreightDetail.shipmentNbr, Equal<Required<SOFreightDetail.shipmentNbr>>,
					And<SOFreightDetail.shipmentType, Equal<Required<SOFreightDetail.shipmentType>>,
					And<SOFreightDetail.docType, Equal<Required<SOFreightDetail.docType>>,
					And<SOFreightDetail.refNbr, Equal<Required<SOFreightDetail.refNbr>>>>>>>.Select(this, ordershipment.ShipmentNbr, ordershipment.ShipmentType, Document.Current.DocType, Document.Current.RefNbr);

				SOFreightDetail anyfreight = PXSelect<SOFreightDetail,
					Where2<Where<SOFreightDetail.refNbr, NotEqual<Current<SOOrderShipment.invoiceNbr>>,
					Or<SOFreightDetail.docType, NotEqual<Current<SOOrderShipment.invoiceType>>>>,
					And<SOFreightDetail.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>,
					And<SOFreightDetail.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>>>>>.SelectSingleBound(this, new object[] { ordershipment });

				if (freightdet == null && anyfreight == null)
				{
					SOShipment shipment = PXSelect<SOShipment, Where<SOShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>, And<SOShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>>>>.Select(this, new object[] { ordershipment });

					if (shipment != null)
					{
						freightdet = new SOFreightDetail();
						freightdet.CuryInfoID = Document.Current.CuryInfoID;
						freightdet.ShipmentNbr = ordershipment.ShipmentNbr;
						freightdet.ShipmentType = ordershipment.ShipmentType;
						if ((PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>>>>.SelectSingleBound(this, new object[] { ordershipment })).Count == 1)
                        {
                            freightdet.OrderType = ordershipment.OrderType;
                            freightdet.OrderNbr = ordershipment.OrderNbr;
                        }
						freightdet.ProjectID = order.ProjectID;

						freightdet.ShipTermsID = shipment.ShipTermsID;
						freightdet.ShipVia = shipment.ShipVia;
						freightdet.ShipZoneID = shipment.ShipZoneID;
						freightdet.TaxCategoryID = shipment.TaxCategoryID;
						freightdet.Weight = shipment.ShipmentWeight;
						freightdet.Volume = shipment.ShipmentVolume;
						freightdet.LineTotal = shipment.LineTotal;
						PXCurrencyAttribute.CuryConvCury<SOFreightDetail.curyLineTotal>(FreightDetails.Cache, freightdet);
						freightdet.FreightCost = shipment.FreightCost;
						PXCurrencyAttribute.CuryConvCury<SOFreightDetail.curyFreightCost>(FreightDetails.Cache, freightdet);
						freightdet.FreightAmt = shipment.FreightAmt;
						PXCurrencyAttribute.CuryConvCury<SOFreightDetail.curyFreightAmt>(FreightDetails.Cache, freightdet);
						freightdet.CuryPremiumFreightAmt = 0m;

						Guid? ownerGuid = null;
						bool firstIteration = true;

						//recalculate All Premium Freight for Shipment
						foreach (PXResult<SOOrderShipment, SOOrder> res in PXSelectJoin<SOOrderShipment, InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>>, Where<SOOrderShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, NotEqual<SOShipmentType.dropShip>>>>.SelectMultiBound(this, new object[] { ordershipment }))
						{
							SOOrderShipment ordershipment2 = (SOOrderShipment)res;
							SOOrder order2 = (SOOrder)res;

							if (firstIteration)
							{
								ownerGuid = order2.OwnerID;
								firstIteration = false;
							}
							else if (ownerGuid != null && ownerGuid == order2.OwnerID)
								ownerGuid = order2.OwnerID;
							else
								ownerGuid = null;

							if (sosetup.Current.FreightAllocation == FreightAllocationList.Prorate)
							{
								if (order2.CuryLineTotal > 0m)
								{
									decimal? ShipRatio = ordershipment2.LineTotal / order2.LineTotal;
									freightdet.CuryPremiumFreightAmt += PXDBCurrencyAttribute.Round(FreightDetails.Cache, freightdet, (decimal)(order2.CuryPremiumFreightAmt * (ShipRatio > 1m ? 1m : ShipRatio) ?? 0m), CMPrecision.TRANCURY);
								}
								else
								{
									freightdet.CuryPremiumFreightAmt += order2.CuryPremiumFreightAmt;
								}
							}
							else
							{
								SOOrderShipment prev_shipment = PXSelectReadonly<SOOrderShipment,
									Where2<Where<SOOrderShipment.shipmentNbr, NotEqual<Current<SOOrderShipment.shipmentNbr>>,
									Or<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>>>,
									And<SOOrderShipment.orderType, Equal<Current<SOOrderShipment.orderType>>,
									And<SOOrderShipment.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>,
									And<SOOrderShipment.invoiceNbr, IsNotNull/*,
									And<SOFreightDetail.curyPremiumFreightAmt, NotEqual<decimal0>>*/>>>>>.SelectSingleBound(this, new object[] { ordershipment2 });
								if (prev_shipment == null)
								{
									freightdet.CuryPremiumFreightAmt += order2.CuryPremiumFreightAmt;
								}
							}
						}

						freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;

						int? accountID;
						object subID;
						GetFreightAccountAndSubAccount(order, shipment.ShipVia, ownerGuid, out accountID, out subID);
						freightdet.AccountID = accountID;
						FreightDetails.Cache.RaiseFieldUpdating<SOFreightDetail.subID>(freightdet, ref subID);
						freightdet.SubID = (int?)subID;

						freightdet = FreightDetails.Insert(freightdet);

						if (order.OpenLineCntr == 0m)
						{
							PXResultset<SOFreightDetail> freightDetails = PXSelect<SOFreightDetail, Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>, And<SOFreightDetail.refNbr, Equal<Current<ARInvoice.refNbr>>, And<SOFreightDetail.orderType, Equal<Required<SOFreightDetail.orderType>>, And<SOFreightDetail.orderNbr, Equal<Required<SOFreightDetail.orderNbr>>>>>>>.Select(this, ordershipment.OrderType, ordershipment.OrderNbr);
							PXResultset<SOOrderShipment> orderShipments = PXSelect<SOOrderShipment, Where<SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>>.Select(this, ordershipment.OrderType, ordershipment.OrderNbr);

							if (freightDetails.Count > 1 && freightDetails.Count == orderShipments.Count)
							{
								decimal totalInvoicedPremiumFreight = 0m;
								foreach (SOFreightDetail freightDetail in freightDetails)
								{
									totalInvoicedPremiumFreight += freightDetail.CuryPremiumFreightAmt ?? 0m;
								}

								decimal premiumFreightDiff = (order.CuryPremiumFreightAmt ?? 0m) - totalInvoicedPremiumFreight;
								if (premiumFreightDiff != 0m)
								{
									freightdet.CuryPremiumFreightAmt += premiumFreightDiff;

									freightdet.CuryTotalFreightAmt = freightdet.CuryPremiumFreightAmt + freightdet.CuryFreightAmt;
									FreightDetails.Update(freightdet);
								}
							}
						}
					}
				}
			}
		}

		public virtual void AddFreightTransaction(SOFreightDetail fd)
		{
			ARTran freight = new ARTran();
			using (new PXLocaleScope(customer.Current.LocaleName))
				freight.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.FreightDescr, fd.ShipVia);
			freight.SOShipmentNbr = fd.ShipmentNbr;
			freight.SOShipmentType = fd.ShipmentType ?? SOShipmentType.Issue;
            freight.SOOrderType = fd.OrderType;
            freight.SOOrderNbr = fd.OrderNbr;
			freight.LineType = SOLineType.Freight;
			freight.CuryTranAmt = fd.CuryTotalFreightAmt;
			freight.TaxCategoryID = fd.TaxCategoryID;
			freight.AccountID = fd.AccountID;
			freight.SubID = fd.SubID;
			freight.ProjectID = fd.ProjectID;

			if (fd.TaskID != null)
				freight.TaskID = fd.TaskID;
			
			freight = (ARTran)Freight.Cache.Insert(freight);

			if (freight.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, freight.ProjectID))
			{
				Account ac = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, freight.AccountID);
				throw new PXException(Messages.TaskWasNotAssigned, ac.AccountCD);
			}
			
		}

		public virtual void CopyFreightNotesAndFilesToARTran()
		{
			foreach (SOFreightDetail fd in FreightDetails.Select())
			{
				foreach (ARTran tran in PXSelect<ARTran,
				Where<ARTran.lineType, Equal<SOLineType.freight>,
				And<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
				And<ARTran.sOShipmentNbr, Equal<Required<ARTran.sOShipmentNbr>>,
				And<ARTran.sOShipmentType, NotEqual<SOShipmentType.dropShip>>>>>>>.Select(this, fd.ShipmentNbr))
				{
					PXNoteAttribute.CopyNoteAndFiles(FreightDetails.Cache, fd, Freight.Cache, tran);
				}
			}
		}


		public virtual void GetFreightAccountAndSubAccount(SOOrder order, string ShipVia, Guid? ownerGuid, out int? accountID, out object subID)
		{
			accountID = null;
			subID = null;
			SOOrderType type = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>>.Select(this, order.OrderType);

			if (type != null)
			{
				Location customerloc = location.Current;
				Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(this, ShipVia);

                CRLocation companyloc =
                        PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>>, Where<Branch.branchID, Equal<Current<ARRegister.branchID>>>>.Select(this);
				EPEmployee employee = (EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<SOOrder.ownerID>>>>.Select(this, ownerGuid);

				switch (type.FreightAcctDefault)
				{
					case SOFreightAcctSubDefault.OrderType:
						accountID = (int?)GetValue<SOOrderType.freightAcctID>(type);
						break;
					case SOFreightAcctSubDefault.MaskLocation:
						accountID = (int?)GetValue<Location.cFreightAcctID>(customerloc);
						break;
					case SOFreightAcctSubDefault.MaskShipVia:
						accountID = (int?)GetValue<Carrier.freightSalesAcctID>(carrier);
						break;
				}

				if (accountID == null)
				{
					accountID = type.FreightAcctID;

					if (accountID == null)
					{
						throw new PXException(Messages.FreightAccountIsRequired);
					}

				}

				if (accountID != null)
				{
					object orderType_SubID = GetValue<SOOrderType.freightSubID>(type);
					object customer_Location_SubID = GetValue<Location.cFreightSubID>(customerloc);
					object carrier_SubID = GetValue<Carrier.freightSalesSubID>(carrier);
                    object branch_SubID = GetValue<CRLocation.cMPFreightSubID>(companyloc);
					object employee_SubID = GetValue<EPEmployee.salesSubID>(employee);

					if (employee_SubID != null)
					subID = SOFreightSubAccountMaskAttribute.MakeSub<SOOrderType.freightSubMask>(this, type.FreightSubMask,
								new object[] { orderType_SubID, customer_Location_SubID, carrier_SubID, branch_SubID, employee_SubID },
								new Type[] { typeof(SOOrderType.freightSubID), typeof(Location.cFreightSubID), typeof(Carrier.freightSalesSubID), typeof(Location.cMPFreightSubID), typeof(EPEmployee.salesSubID) });
					else
						subID = SOFreightSubAccountMaskAttribute.MakeSub<SOOrderType.freightSubMask>(this, type.FreightSubMask,
							new object[] { orderType_SubID, customer_Location_SubID, carrier_SubID, branch_SubID, customer_Location_SubID },
							new Type[] { typeof(SOOrderType.freightSubID), typeof(Location.cFreightSubID), typeof(Carrier.freightSalesSubID), typeof(Location.cMPFreightSubID), typeof(Location.cFreightSubID) });
				}
			}
		}
		#endregion

		#region Discount

        public override void RecalculateDiscounts(PXCache sender, ARTran line)
        {
            if (line.InventoryID != null && line.Qty != null && line.CuryLineAmt != null && line.IsFree != true)
            {
                DateTime? docDate = Document.Current.DocDate;
                int? customerLocationID = Document.Current.CustomerLocationID;

						//Recalculate discounts on Sales Order date
						/*SOLine soline = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                        And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                        And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>.Select(this, line.SOOrderType, line.SOOrderNbr, line.SOOrderLineNbr);
                if (soline != null)
                {
                    docDate = soline.OrderDate;
						}*/

				DiscountEngine<ARTran>.SetDiscounts<SOInvoiceDiscountDetail>(sender, Transactions, line, DiscountDetails, Document.Current.BranchID, customerLocationID, Document.Current.CuryID, docDate, false, recalcdiscountsfilter.Current);

                foreach (SOInvoiceDiscountDetail discountDetail in DiscountDetails.Select())
                {
                    if (discountDetail.OrderType == null)
                        discountDetail.OrderType = line.SOOrderType ?? Messages.NoOrderType;
                    if (discountDetail.OrderNbr == null)
                        discountDetail.OrderNbr = line.SOOrderNbr ?? Messages.NoOrder;
                }
                RecalculateTotalDiscount();
            }
        }

		public virtual void RecalculateTotalDiscount()
		{
			if (Document.Current != null && SODocument.Current != null)
			{
				SOInvoice old_row = PXCache<SOInvoice>.CreateCopy(SODocument.Current);
                SODocument.Cache.SetValueExt<SOInvoice.curyDiscTot>(SODocument.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<SOInvoiceDiscountDetail>(SOInvoiceDiscountDetails));
				SODocument.Cache.RaiseRowUpdated(SODocument.Current, old_row);
			}
		}

		public bool ProrateDiscount
		{
			get
			{
				SOSetup sosetup = PXSelect<SOSetup>.Select(this);

				if (sosetup == null)
				{
					return true;//default true
				}
				else
				{
					if (sosetup.ProrateDiscounts == null)
						return true;
					else
						return sosetup.ProrateDiscounts == true;
				}

			}
		}
		#endregion

		public static void UpdateSOInvoiceState(IBqlTable aDoc, CCTranType aLastOperation)
		{
			SOInvoiceEntry graph = PXGraph.CreateInstance<SOInvoiceEntry>();
			graph.UpdateDocState(aDoc as SOInvoice, aLastOperation);
		}

        public virtual void UpdateDocState(SOInvoice doc, CCTranType lastOperation)
        {
            this.Document.Current = Document.Search<ARInvoice.refNbr>(doc.RefNbr, doc.DocType);

            bool needUpdate = CCPaymentEntry.UpdateCapturedState<SOInvoice>(doc, this.ccProcTran.Select());

            if (needUpdate)
            {
                //doc.PreAuthTranNumber = null;
                doc = this.SODocument.Update(doc);
                Document.Search<ARInvoice.refNbr>(doc.RefNbr, doc.DocType);
				if (doc.IsCCCaptured == true) 
				{
					foreach (CCProcTran tran in this.ccProcTran.Select()) 
					{
						if (String.IsNullOrEmpty(tran.RefNbr) || String.IsNullOrEmpty(tran.DocType)) 
						{
							tran.DocType = doc.DocType;
							tran.RefNbr = doc.RefNbr;
							this.ccProcTran.Update(tran);
						}
					}
				}
                this.Save.Press();
            }
        }

		internal sealed class DummyView : PXView
		{
			List<object> _Records;
			internal DummyView(PXGraph graph, BqlCommand command, List<object> records)
				: base(graph, true, command)
			{
				_Records = records;
			}
			public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
			{
				return _Records;
			}
		}

		#region Avalara Tax

		public override void ApplyAvalaraTax(ARInvoice invoice, GetTaxResult result)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { invoice });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			//Clear all existing Tax transactions:
			foreach (PXResult<ARTaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { invoice }))
			{
				ARTaxTran taxTran = (ARTaxTran)res;
				Taxes.Delete(taxTran);
			}

			this.Views.Caches.Add(typeof(Tax));

			bool requireControlTotal = ARSetup.Current.RequireControlTotal == true;

			if (invoice.Hold != true)
				ARSetup.Current.RequireControlTotal = false;

			try
			{
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

						ARTaxTran tax = new ARTaxTran();
						tax.Module = BatchModule.AR;
						tax.TranType = invoice.DocType;
						tax.RefNbr = invoice.RefNbr;
						tax.TaxID = taxID;
						tax.CuryTaxAmt = Math.Abs(result.TaxSummary[i].Tax);
						tax.CuryTaxableAmt = Math.Abs(result.TaxSummary[i].Taxable);
                        tax.TaxRate = Convert.ToDecimal(result.TaxSummary[i].Rate) * 100;
						tax.TaxType = "S";
						tax.TaxBucketID = 0;
						tax.AccountID = vendor.SalesTaxAcctID;
						tax.SubID = vendor.SalesTaxSubID;
                        tax.JurisType = result.TaxSummary[i].JurisType.ToString();
                        tax.JurisName = result.TaxSummary[i].JurisName;

						Taxes.Insert(tax);
					}

				SOInvoice soInvoice = PXSelect<SOInvoice, Where<SOInvoice.docType, Equal<Required<SOInvoice.docType>>, And<SOInvoice.refNbr, Equal<Required<SOInvoice.refNbr>>>>>.Select(this, invoice.DocType, invoice.RefNbr);

				invoice.CuryTaxTotal = Math.Abs(result.TotalTax);
				Document.Cache.SetValueExt<ARInvoice.isTaxSaved>(invoice, true);
			}
			finally
			{
				ARSetup.Current.RequireControlTotal = requireControlTotal;
			}


			if (invoice.ApplyPaymentWhenTaxAvailable == true)
			{
				PXSelectBase<ARAdjust2> select = new PXSelectJoin<ARAdjust2,
					InnerJoin<ARPayment, On<ARAdjust2.adjgDocType, Equal<ARPayment.docType>, And<ARAdjust2.adjgRefNbr, Equal<ARPayment.refNbr>>>>,
					Where<ARAdjust2.adjdDocType, Equal<Required<ARInvoice.docType>>,
					And<ARAdjust2.adjdRefNbr, Equal<Required<ARInvoice.refNbr>>>>>(this);

				decimal amountApplied = 0m;
				foreach (PXResult<ARAdjust2, ARPayment> res in select.Select(invoice.DocType, invoice.RefNbr))
				{
					ARAdjust2 row = (ARAdjust2)res;
					ARPayment payment = (ARPayment)res;

					ARAdjust2 copy = PXCache<ARAdjust2>.CreateCopy(row);
					amountApplied += (copy.CuryAdjdAmt ?? 0m);

					if (amountApplied > (invoice.CuryDocBal ?? 0m))
					{
						decimal newAdjdAmt = (copy.CuryAdjdAmt ?? 0m) - (amountApplied - (invoice.CuryDocBal ?? 0m));
						copy.CuryAdjdAmt = newAdjdAmt > 0m ? newAdjdAmt : 0m;
					}
					Adjustments.Update(copy);
				}
			}

		}

		public override decimal? GetDocDiscount()
		{
			return SODocument.Current.CuryDiscTot;
		}
		
		#endregion

		public class ARTranComparable : IComparable<ARTranComparable> 
		{
			public ARTranComparable(ARTran tran, SOLine orderLine, SOMiscLine2 miscLine)
			{
				this.Tran = tran;
				this.OrderLine = orderLine;
				this.MiscLine = miscLine;
				if (orderLine != null)
					this.LineNbr = orderLine.LineNbr.GetValueOrDefault(1000);
				else if (miscLine != null)
					this.LineNbr = miscLine.LineNbr.GetValueOrDefault(1000);
			}

			public int LineNbr { get; private set; }
			public ARTran Tran { get; private set; }
			public SOLine OrderLine { get; private set; }
			public SOMiscLine2 MiscLine { get; private set; }


			public int CompareTo(ARTranComparable other)
			{
				return this.LineNbr.CompareTo(other.LineNbr);
			}
		}
	}
}
