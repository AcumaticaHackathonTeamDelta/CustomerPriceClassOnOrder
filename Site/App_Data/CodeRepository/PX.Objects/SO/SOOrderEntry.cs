using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using PX.CCProcessingBase;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using POLine = PX.Objects.PO.POLine;
using POOrder = PX.Objects.PO.POOrder;
using System.Threading.Tasks;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AR.CCPaymentProcessing;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;

namespace PX.Objects.SO
{
	[Serializable]    
	public class SOOrderEntry : PXGraph<SOOrderEntry, SOOrder>, PXImportAttribute.IPXPrepareItems
	{
		[Serializable]
		public partial class CCProcTranV : CCProcTran
		{
			#region TranNbr
			public new abstract class tranNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region PMInstanceID
			public new abstract class pMInstanceID : PX.Data.IBqlField
			{
			}
			#endregion
			#region ProcessingCenterID
			public new abstract class processingCenterID : PX.Data.IBqlField
			{
			}
			#endregion
			#region DocType
			public new abstract class docType : PX.Data.IBqlField
			{
			}
			#endregion
			#region RefNbr
			public new abstract class refNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region TranType
			#endregion
			#region ProcStatus
			public new abstract class procStatus : PX.Data.IBqlField
			{
			}
			#endregion
			#region TranStatus
			public new abstract class tranStatus : PX.Data.IBqlField
			{
			}
			#endregion
			#region RefTranNbr
			public new abstract class refTranNbr : PX.Data.IBqlField
			{
			}

			#endregion
		}
		public ToggleCurrency<SOOrder> CurrencyView;
		public PXFilter<Vendor> _Vendor;

		/// <summary>
		/// If true the SO-PO Link dialog will display PO Orders on hold.
		/// </summary>
		/// <remarks>This setting is used when linking On-Hold PO Orders with SO created through RQRequisitionEntry.</remarks>
		public bool SOPOLinkShowDocumentsOnHold { get; set; }

		#region Selects
		[PXViewName(Messages.SOOrder)]
		public PXSelectJoin<SOOrder,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<SOOrder.customerID>>>,
			Where<SOOrder.orderType, Equal<Optional<SOOrder.orderType>>,
			And<Where<Customer.bAccountID, IsNull,
			Or<Match<Customer, Current<AccessInfo.userName>>>>>>> Document;
		[PXCopyPasteHiddenFields(typeof(SOOrder.cancelled), typeof(SOOrder.preAuthTranNumber), typeof(SOOrder.ownerID), typeof(SOOrder.workgroupID))]
		public PXSelect<SOOrder, Where<SOOrder.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> CurrentDocument;

		public PXSelect<RUTROT.RUTROT, Where<True, Equal<False>>> Rutrots;

		[PXViewName(Messages.SOLine)]
		[PXImport(typeof(SOOrder))]
        [PXCopyPasteHiddenFields(typeof(SOLine.completed))]
		public PXSelect<SOLine, Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>, OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>> Transactions;
		public PXSelect<SOTax, Where<SOTax.orderType, Equal<Current<SOOrder.orderType>>, And<SOTax.orderNbr, Equal<Current<SOOrder.orderNbr>>>>, OrderBy<Asc<SOTax.orderType, Asc<SOTax.orderNbr, Asc<SOTax.taxID>>>>> Tax_Rows;
		public TaxTranSelect<SOOrder, SOOrder.termsID, SOTaxTran, SOTaxTran.taxID,
			Where<SOTaxTran.orderType, Equal<Current<SOOrder.orderType>>, 
				And<SOTaxTran.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> Taxes;
		public PXSelectJoin<SOOrderShipment, LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>, And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>, Where<SOOrderShipment.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> shipmentlist;

		[PXViewName(Messages.BillingAddress)]
		public PXSelect<SOBillingAddress, Where<SOBillingAddress.addressID, Equal<Current<SOOrder.billAddressID>>>> Billing_Address;
		[PXViewName(Messages.ShippingAddress)]
		public PXSelect<SOShippingAddress, Where<SOShippingAddress.addressID, Equal<Current<SOOrder.shipAddressID>>>> Shipping_Address;
		[PXViewName(Messages.BillingContact)]
		public PXSelect<SOBillingContact, Where<SOBillingContact.contactID, Equal<Current<SOOrder.billContactID>>>> Billing_Contact;
		[PXViewName(Messages.ShippingContact)]
		public PXSelect<SOShippingContact, Where<SOShippingContact.contactID, Equal<Current<SOOrder.shipContactID>>>> Shipping_Contact;

		public PXSelect<SOSetupApproval, Where<SOSetupApproval.orderType, Equal<Optional<SOOrder.orderType>>>> SetupApproval;

        [PXViewName(Messages.CustomerCredit)]
        public SOCustomerCreditHelper<SOOrder, SOOrder.customerID> CustomerCreditHelper;
        [PXViewName(Messages.Approval)]
		public EPApprovalAutomation<SOOrder, SOOrder.approved, SOOrder.rejected, SOOrder.hold, SOSetupApproval> Approval;

		[PXCopyPasteHiddenView()]
		public PlanningHelper<SOLineSplit> Planning;

        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>> currencyinfo;

		public PXSetup<ARSetup> arsetup;
		[PXViewName(AR.Messages.Customer)]
		public PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<SOOrder.customerID>>>> customer;
		public PXSetup<CustomerClass, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>> customerclass;
		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<SOOrder.taxZoneID>>>> taxzone;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<SOOrder.customerID>>, And<Location.locationID, Equal<Optional<SOOrder.customerLocationID>>>>> location;
		public PXSelect<ARBalances> arbalances;
		public PXSetup<SOOrderType, Where<SOOrderType.orderType, Equal<Optional<SOOrder.orderType>>>> soordertype;
		public PXSetup<SOOrderTypeOperation,
			Where<SOOrderTypeOperation.orderType, Equal<Optional<SOOrderType.orderType>>,
			And<SOOrderTypeOperation.operation, Equal<Optional<SOOrderType.defaultOperation>>>>> sooperation;

		[PXCopyPasteHiddenView()]
		[PXFilterable]
		public PXSelect<SOLineSplit, Where<SOLineSplit.orderType, Equal<Current<SOLine.orderType>>, And<SOLineSplit.orderNbr, Equal<Current<SOLine.orderNbr>>, And<SOLineSplit.lineNbr, Equal<Current<SOLine.lineNbr>>>>>> splits;
		public PXSelect<SOLine, Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>, And<SOLine.lineType, NotEqual<SOLineType.miscCharge>, And<SOLine.isFree, Equal<boolTrue>>>>>, OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>> FreeItems;
		[PXCopyPasteHiddenView()]
		public PXSelect<SOOrderDiscountDetail, Where<SOOrderDiscountDetail.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderDiscountDetail.orderNbr, Equal<Current<SOOrder.orderNbr>>>>, OrderBy<Asc<SOOrderDiscountDetail.orderType, Asc<SOOrderDiscountDetail.orderNbr>>>> DiscountDetails;

		public PXSetup<INSetup> insetup;
		public PXSetup<SOSetup> sosetup;
		public PXSetup<Branch, InnerJoin<INSite, On<INSite.branchID, Equal<Branch.branchID>>>, Where<INSite.siteID, Equal<Optional<SOOrder.destinationSiteID>>>> Company;

		public PXSelect<CurrencyInfo> DummyCuryInfo;
		public LSSOLine lsselect;

		public PXFilter<SOParamFilter> soparamfilter;
		public PXFilter<AddInvoiceFilter> addinvoicefilter;
		public PXFilter<CopyParamFilter> copyparamfilter;
		public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;

        public PXSelect<INTranSplit> intransplit;

		public PXFilter<InvoiceSplits> invoicesplits;

		public PXSelect<SOLine, Where<SOLine.orderType, Equal<Optional<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Optional<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Optional<SOLine.lineNbr>>>>>> currentposupply;
		[PXCopyPasteHiddenView()]
		public PXSelect<POLine3> posupply;
        [PXCopyPasteHiddenView()]
        public PXSelect<POOrder> poorderlink;

		[PXCopyPasteHiddenView()]
		public PXSelectJoin<SOLineSplit2, LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<SOLineSplit2.planID>>, LeftJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.orderType, Equal<SOLineSplit2.orderType>, And<SOOrderTypeOperation.operation, Equal<SOLineSplit2.operation>>>>>, Where<SOLineSplit2.sOOrderType, Equal<Optional<SOLineSplit.orderType>>, And<SOLineSplit2.sOOrderNbr, Equal<Optional<SOLineSplit.orderNbr>>, And<SOLineSplit2.sOLineNbr, Equal<Optional<SOLineSplit.lineNbr>>, And<SOLineSplit2.sOSplitLineNbr, Equal<Optional<SOLineSplit.splitLineNbr>>>>>>> sodemand;

		[CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<SOOrder.customerID>>>>))]
		[CRDefaultMailTo(typeof(Select<SOShippingContact, Where<SOShippingContact.contactID, Equal<Current<SOOrder.shipContactID>>>>))]
		public CRActivityList<SOOrder>
			Activity;

		public PXSelect<SOSalesPerTran, Where<SOSalesPerTran.orderType, Equal<Current<SOOrder.orderType>>, And<SOSalesPerTran.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> SalesPerTran;
		public PXSelect<SOOrderSite, Where<SOOrderSite.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderSite.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> SiteList;

		public PXSelect<SOOrder, Where<SOOrder.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> DocumentProperties;
		public PXSelect<SOPackageInfoEx, Where<SOPackageInfoEx.orderType, Equal<Current<SOOrder.orderType>>, And<SOPackageInfoEx.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> Packages;
		public PXSelect<SOCarrierRate, Where<SOCarrierRate.method, IsNotNull>, OrderBy<Asc<SOCarrierRate.deliveryDate, Asc<SOCarrierRate.amount>>>> CarrierRates;
		public PXSelect<Carrier, Where<Carrier.isExternal, Equal<BQLConstants.BitOn>, And<Carrier.carrierPluginID, IsNotNull, And<Carrier.pluginMethod, IsNotNull>>>> PlugIns;

		public PXSelect<INReplenishmentOrder> Replenihment;
		public PXSelect<INReplenishmentLine,
			Where<INReplenishmentLine.sOType, Equal<Current<SOLine.orderType>>,
				And<INReplenishmentLine.sONbr, Equal<Current<SOLine.orderNbr>>,
				And<INReplenishmentLine.sOLineNbr, Equal<Current<SOLine.lineNbr>>>>>> ReplenishmentLines;

		[PXCopyPasteHiddenView()]
		public PXSelectJoin<SOAdjust, InnerJoin<ARPayment, On<ARPayment.docType, Equal<SOAdjust.adjgDocType>, And<ARPayment.refNbr, Equal<SOAdjust.adjgRefNbr>>>>> Adjustments;
		public PXSelectJoin<SOAdjust,
                            InnerJoin<ARRegisterAlias, On<ARRegisterAlias.docType, Equal<SOAdjust.adjgDocType>, And<ARRegisterAlias.refNbr, Equal<SOAdjust.adjgRefNbr>>>,
                            InnerJoinSingleTable<ARPayment, On<ARRegisterAlias.docType, Equal<ARPayment.docType>, And<ARRegisterAlias.refNbr, Equal<ARPayment.refNbr>>>,
                            InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARRegisterAlias.curyInfoID>>>>>,
							Where<SOAdjust.adjdOrderType, Equal<Current<SOOrder.orderType>>,
								And<SOAdjust.adjdOrderNbr, Equal<Current<SOOrder.orderNbr>>>>> Adjustments_Raw;

		public PXSelectJoin<INReplenishmentLine,
			InnerJoin<INItemPlan, On<INItemPlan.planID, Equal<INReplenishmentLine.planID>>>,
			Where<INReplenishmentLine.sOType, Equal<Current<SOLine.orderType>>,
				And<INReplenishmentLine.sONbr, Equal<Current<SOLine.orderNbr>>,
					And<INReplenishmentLine.sOLineNbr, Equal<Current<SOLine.lineNbr>>>>>> replenishment;

		public PXSelect<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Current<CustomerPaymentMethodC.cCProcessingCenterID>>>> PMProcessingCenter;

		public PXSelect<RUTROT.RUTROTDistribution,
					Where<True, Equal<False>>> RRDistribution;

		[PXViewName(CR.Messages.SalesPerson)]
		public PXSelect<EPEmployee, Where<EPEmployee.salesPersonID, Equal<Current<SOOrder.salesPersonID>>>> SalesPerson;

        [PXViewName(CR.Messages.MainContact)]
		public PXSelect<Contact> DefaultCompanyContact;

        


        protected virtual IEnumerable defaultCompanyContact()
		{
			List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
			foreach (PXResult<Branch, BAccountR, Contact> res in PXSelectJoin<Branch, 
										LeftJoin<BAccountR, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>, 
										LeftJoin<Contact, On<BAccountR.defContactID, Equal<Contact.contactID>>>>, 
										Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, branches != null ? (int?)branches[0] : null))
			{
				yield return (Contact)res;
				break;
			}
		}

		[PXHidden]
		public PXSelect<CR.CROpportunity> 
			OpportunityBackReference;

		protected virtual IEnumerable opportunityBackReference()
		{
			return OpportunityBackReference.Cache.Updated;
		}

		public virtual IEnumerable adjustments()
		{
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>>.Select(this);

			foreach (PXResult<SOAdjust, ARRegisterAlias, ARPayment, CurrencyInfo> res in Adjustments_Raw.Select())
			{
                ARPayment payment = (ARPayment)res;
				SOAdjust adj = (SOAdjust)res;
				CurrencyInfo pay_info = (CurrencyInfo)res;

				if (adj == null)
					continue;

                if (payment != null)
                    PXCache<ARRegister>.RestoreCopy(payment, (ARRegisterAlias)res);

				SOAdjust other = PXSelectGroupBy<SOAdjust, Where<SOAdjust.adjgDocType, Equal<Required<SOAdjust.adjgDocType>>, And<SOAdjust.adjgRefNbr, Equal<Required<SOAdjust.adjgRefNbr>>, And<Where<SOAdjust.adjdOrderType, NotEqual<Required<SOAdjust.adjdOrderType>>, Or<SOAdjust.adjdOrderNbr, NotEqual<Required<SOAdjust.adjdOrderNbr>>>>>>>, Aggregate<GroupBy<SOAdjust.adjgDocType, GroupBy<SOAdjust.adjgRefNbr, Sum<SOAdjust.curyAdjgAmt, Sum<SOAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdOrderType, adj.AdjdOrderNbr);
				if (other != null && other.AdjdOrderNbr != null)
				{
					payment.CuryDocBal -= other.CuryAdjgAmt;
					payment.DocBal -= other.AdjAmt;
				}

				ARAdjust fromar = PXSelectGroupBy<ARAdjust, Where<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>, And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>, And<ARAdjust.released, Equal<boolFalse>>>>, Aggregate<GroupBy<ARAdjust.adjgDocType, GroupBy<ARAdjust.adjgRefNbr, Sum<ARAdjust.curyAdjgAmt, Sum<ARAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);
				if (fromar != null && fromar.AdjdRefNbr != null)
				{
					payment.CuryDocBal -= fromar.CuryAdjgAmt;
					payment.DocBal -= fromar.AdjAmt;
				}

				decimal CuryDocBal;
				if (string.Equals(pay_info.CuryID, inv_info.CuryID))
				{
					CuryDocBal = (decimal)payment.CuryDocBal;
				}
				else
				{
					PXDBCurrencyAttribute.CuryConvCury(Adjustments.Cache, inv_info, (decimal)payment.DocBal, out CuryDocBal);
				}

				if (adj.CuryAdjdAmt > CuryDocBal)
				{
					//if reconsidered need to calc RGOL
					adj.CuryDocBal = CuryDocBal;
					adj.CuryAdjdAmt = 0m;
				}
				else
				{
					adj.CuryDocBal = CuryDocBal - adj.CuryAdjdAmt;
				}

				RecalcTotals(CurrentDocument.Cache, (SOOrder)CurrentDocument.Cache.Current);
				yield return res;
			}

			//if (Document.Current != null && (Document.Current.ARDocType == ARDocType.Invoice || Document.Current.ARDocType == ARDocType.DebitMemo) && Document.Current.Completed == false && Document.Current.Cancelled == false)
			if (Document.Current != null && Document.Current.ARDocType == null)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Adjustments.Cache, Document.Cache, arbalances.Cache))
				{
					PXSelectBase<ARPayment> s = new PXSelectReadonly2<ARPayment,
						InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>,
						LeftJoin<SOAdjust, On<SOAdjust.adjgDocType, Equal<ARPayment.docType>,
							And<SOAdjust.adjgRefNbr, Equal<ARPayment.refNbr>,
							And<SOAdjust.adjdOrderType, Equal<Current<SOOrder.orderType>>,
							And<SOAdjust.adjdOrderNbr, Equal<Current<SOOrder.orderNbr>>>>>>>>,
						Where<ARPayment.customerID, Equal<Current<SOOrder.customerID>>,
						And2<Where<ARPayment.docType, Equal<ARDocType.payment>, Or<ARPayment.docType, Equal<ARDocType.prepayment>, Or<ARPayment.docType, Equal<ARDocType.creditMemo>>>>,
						And<ARPayment.openDoc, Equal<boolTrue>,
						And<SOAdjust.adjdOrderNbr, IsNull>>>>>(this);
					
					foreach (PXResult<ARPayment, CurrencyInfo> res in s.Select())
					{
						ARPayment payment = res;
						SOAdjust adj = new SOAdjust();

						adj.CustomerID = Document.Current.CustomerID;
						adj.AdjdOrderType = Document.Current.OrderType;
						adj.AdjdOrderNbr = Document.Current.OrderNbr;
						adj.AdjgDocType = payment.DocType;
						adj.AdjgRefNbr = payment.RefNbr;

						if (Adjustments.Cache.Locate(adj) == null)
						{
							yield return new PXResult<SOAdjust, ARPayment>(Adjustments.Insert(adj), payment);
						}
					}
				}
			}
		}

		public PXFilter<CustomerPaymentMethodInputMode> InputModeFilter;

		public PXSelect<CustomerPaymentMethodC,
		Where<CustomerPaymentMethodC.pMInstanceID, Equal<Optional<SOOrder.pMInstanceID>>>> DefPaymentMethodInstance;

		public PXSelectJoin<CustomerPaymentMethodDetail, InnerJoin<PaymentMethodDetail,
			On<CustomerPaymentMethodDetail.paymentMethodID, Equal<PaymentMethodDetail.paymentMethodID>,
				And<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>,
					And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
			Where<PaymentMethodDetail.isCCProcessingID, Equal<True>, And<CustomerPaymentMethodDetail.pMInstanceID, Equal<Optional<SOOrder.pMInstanceID>>>>> ccpIdDet;

		public PXSelectJoin<CustomerPaymentMethodDetail,
			  InnerJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethodDetail.paymentMethodID>,
			  And<PaymentMethodDetail.detailID, Equal<CustomerPaymentMethodDetail.detailID>,
			  And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
				Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Optional<SOOrder.pMInstanceID>>>, OrderBy<Asc<PaymentMethodDetail.orderIndex>>> DefPaymentMethodInstanceDetails;

		public PXSelectJoin<CustomerPaymentMethodDetail,
			  InnerJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethodDetail.paymentMethodID>,
			  And<PaymentMethodDetail.detailID, Equal<CustomerPaymentMethodDetail.detailID>,
			  And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>>,
				Where<CustomerPaymentMethodDetail.pMInstanceID, Equal<Optional<SOOrder.pMInstanceID>>>, OrderBy<Asc<PaymentMethodDetail.orderIndex>>> DefPaymentMethodInstanceDetailsAll;

		public virtual IEnumerable defPaymentMethodInstanceDetails()
		{
			if (DefPaymentMethodInstance.Current != null)
			{
				int? pmInstanceId = DefPaymentMethodInstance.Current.PMInstanceID;
				bool isTokenized = CCProcessingUtils.isTokenizedPaymentMethod(this, pmInstanceId);
				bool isHF = CCProcessingUtils.isHFPaymentMethod(this, pmInstanceId);
				bool showToken = InputModeFilter.Current != null && InputModeFilter.Current.InputMode == InputModeType.Token && isTokenized;
				bool showDetails = InputModeFilter.Current == null || InputModeFilter.Current.InputMode == InputModeType.Details && !isHF;
				return CCProcessingUtils.getPMdetails(this, DefPaymentMethodInstance.Current.PMInstanceID, showToken , showDetails);
			}
			else
			{
				return null;
			}
			
		}

		public PXSelect<CustomerProcessingCenterID, Where<CustomerProcessingCenterID.bAccountID, Equal<Current<SOOrder.customerID>>,
			And<CustomerProcessingCenterID.cCProcessingCenterID, Equal<Required<CustomerPaymentMethod.cCProcessingCenterID>>,
			And<CustomerProcessingCenterID.customerCCPID, Equal<Required<CustomerPaymentMethod.customerCCPID>>>>>> CustomerProcessingID;

		public PXSelect<PaymentMethodDetail,
			Where<PaymentMethodDetail.paymentMethodID, Equal<Optional<CustomerPaymentMethodC.paymentMethodID>>,
			 And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>> PMDetails;
		public PXSelect<PaymentMethod,
		  Where<PaymentMethod.paymentMethodID, Equal<Optional<CustomerPaymentMethod.paymentMethodID>>>> PaymentMethodDef;

		public PXSelectReadonly2<CCProcTran, LeftJoin<CCProcTranV, On<CCProcTranV.pMInstanceID,
						Equal<CCProcTran.pMInstanceID>,
						And<CCProcTranV.refTranNbr, Equal<CCProcTran.tranNbr>,
						And<CCProcTranV.procStatus, Equal<CCProcStatus.finalized>,
						And<CCProcTranV.tranStatus, Equal<CCTranStatusCode.approved>>>>>>,
			   Where<CCProcTran.origRefNbr, Equal<Optional<SOOrder.orderNbr>>,
						And<CCProcTran.origDocType, Equal<Optional<SOOrder.orderType>>,
						And<CCProcTran.tranType, Equal<CCTranTypeCode.authorize>,
						And<CCProcTran.procStatus, Equal<CCProcStatus.finalized>,
						And<CCProcTran.tranStatus, Equal<CCTranStatusCode.approved>,
						And<CCProcTranV.tranNbr, IsNull>>>>>>,
			   OrderBy<Desc<CCProcTran.tranNbr>>> ccAuthTrans;

		public PXSelectReadonly<CCProcTran, Where<CCProcTran.origRefNbr, Equal<Current<SOOrder.orderNbr>>,
							And<CCProcTran.origDocType, Equal<Current<SOOrder.orderType>>>>,
							OrderBy<Desc<CCProcTran.tranNbr>>> ccProcTran; //All CC trans

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<CCProcTran.origDocType>, And<SOOrder.orderNbr, Equal<CCProcTran.origRefNbr>>>>))]
		protected virtual void CCProcTran_RefNbr_CacheAttached(PXCache sender)
		{
		}

		public virtual IEnumerable POSupply()
		{
			SOLine soline = (SOLine)currentposupply.Select();

			List<POLine3> ret = new List<POLine3>();

			foreach (PXResult<POLine3, PO.POOrder, INItemPlan> res in
				PXSelectJoinGroupBy<POLine3,
				InnerJoin<PO.POOrder, On<PO.POOrder.orderNbr, Equal<POLine3.orderNbr>>,
				LeftJoin<INItemPlan, On<INItemPlan.supplyPlanID, Equal<POLine3.planID>>>>,
				Where2<
					Where2<Where<Current<SOLine.pOSource>, Equal<INReplenishmentSource.purchaseToOrder>, 
									 And<Where<POLine3.orderType, Equal<PO.POOrderType.regularOrder>,
													Or<POLine3.orderType, Equal<PO.POOrderType.blanket>>>>>,
							Or<Where<Current<SOLine.pOSource>, Equal<INReplenishmentSource.dropShipToOrder>,
									 And<Where<POLine3.orderType, Equal<PO.POOrderType.dropShip>,
									And<Current<SOLine.customerID>, Equal<PO.POOrder.shipToBAccountID>,
													Or <POLine3.orderType, Equal<PO.POOrderType.blanket>,
													Or<POOrder.shipDestType, NotEqual<PO.POShippingDestination.customer>>>>>>>>>,
				And2<Where<POLine3.lineType, Equal<PO.POLineType.goodsForInventory>,
					Or<POLine3.lineType, Equal<PO.POLineType.nonStock>,					
					Or<POLine3.lineType, Equal<PO.POLineType.goodsForDropShip>,
					Or<POLine3.lineType, Equal<PO.POLineType.nonStockForDropShip>,
					Or<POLine3.lineType, Equal<PO.POLineType.goodsForSalesOrder>,
					Or<POLine3.lineType, Equal<PO.POLineType.nonStockForSalesOrder>,					
					Or<POLine3.lineType, Equal<PO.POLineType.goodsForReplenishment>>>>>>>>,
				And2<Where<POLine3.inventoryID, Equal<Current<SOLine.inventoryID>>,
				And2<Where<Current<SOLine.subItemID>, IsNull, 
						Or<POLine3.subItemID, Equal<Current<SOLine.subItemID>>>>,
					And<POLine3.siteID, Equal<Current<SOLine.pOSiteID>>>>>,
				And2<Where<Current<SOLine.vendorID>, IsNull,
					Or<POLine3.vendorID, Equal<Current<SOLine.vendorID>>>>, 
				And<Current2<SOLine.pOSource>, IsNotNull>>>>>,
				Aggregate<GroupBy<POLine3.orderType,
				GroupBy<POLine3.orderNbr,
				GroupBy<POLine3.lineNbr,
				GroupBy<POLine3.cancelled,
				GroupBy<POLine3.completed,
				GroupBy<POOrder.hold,
				Max<POLine3.baseOpenQty, Max<POLine3.baseReceivedQty, Max<POLine3.baseOrderQty, Sum<INItemPlan.planQty>>>>>>>>>>>>.SelectMultiBound(this, new object[] { soline }))
			{
				POLine3 supply = PXCache<POLine3>.CreateCopy(res);
				POOrder poorder = res;
				INItemPlan demand = res;

				if (soline.POSource == INReplenishmentSource.DropShipToOrder &&
					  supply.OrderType == PO.POOrderType.RegularOrder) continue;

				bool IsLinkedToSO = false;
				if (supply.OrderType == PO.POOrderType.DropShip)
				{
					//relation should be 1:1
					SOLineSplit dropshiplink = PXSelect<SOLineSplit, Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>, And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>, And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>.SelectWindowed(this, 0, 1, supply.OrderType, supply.OrderNbr, supply.LineNbr);
					IsLinkedToSO = (dropshiplink != null);
				}

				SOLineSplit link = PXSelect<SOLineSplit, Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>, And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>, And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>, And<SOLineSplit.orderType, Equal<Current<SOLine.orderType>>, And<SOLineSplit.orderNbr, Equal<Current<SOLine.orderNbr>>, And<SOLineSplit.lineNbr, Equal<Current<SOLine.lineNbr>>>>>>>>>.SelectWindowed(this, 0, 1, supply.OrderType, supply.OrderNbr, supply.LineNbr);

				if ((poorder.Hold == false || SOPOLinkShowDocumentsOnHold ) && supply.OrderType != PO.POOrderType.DropShip && supply.Completed == false && supply.Cancelled == false &&
						(supply.BaseOpenQty - (demand.PlanQty ?? 0m) > 0m) ||
					(poorder.Hold == false || SOPOLinkShowDocumentsOnHold) && supply.OrderType == PO.POOrderType.DropShip && supply.Cancelled == false && IsLinkedToSO == false &&
						(supply.Completed == false && supply.BaseOrderQty >= 0m || supply.BaseReceivedQty >= 0m) ||
					link != null)
				{
					supply.Selected = (link != null);
					supply.SOOrderType = soline.OrderType;
					supply.SOOrderNbr = soline.OrderNbr;
					supply.SOOrderLineNbr = soline.LineNbr;
					supply.SOOrderSplitLineNbr = (link != null) ? link.SplitLineNbr : null;
					supply.VendorRefNbr = poorder.VendorRefNbr;
					supply.DemandQty = demand.PlanQty ?? 0m;

					POLine3 cached;

					if ((cached = posupply.Locate(supply)) == null)
					{
						cached = (POLine3)posupply.Update(supply);
						if (posupply.Cache.GetStatus(cached) == PXEntryStatus.Updated)
						{
							posupply.Cache.SetStatus(cached, PXEntryStatus.Held);
					}
					}
					ret.Add(cached);
				}
			}

			return ret;
		}

		public virtual IEnumerable Invoicesplits()
		{
			List<InvoiceSplits> list = new List<InvoiceSplits>(invoicesplits.Cache.Inserted.Cast<InvoiceSplits>());
								
			if (list.Count > 0)
				return list; //return cached.

			int lineNbr = 0;

			if (addinvoicefilter.Current != null && addinvoicefilter.Current.RefNbr != null)
			{
				SOInvoicedRecords splits = lsselect.SelectInvoicedRecords(addinvoicefilter.Current.RefNbr);

				foreach (SOInvoicedRecords.Record record in splits.Records)
				{
					bool expand = record.Transactions.Count > 0;

					if (record.Item.KitItem == true && record.Item.StkItem == false)
					{
						expand = addinvoicefilter.Current.Expand == true && record.Transactions.Count > 0;
					}

					List<InvoiceSplits> expandedList = new List<InvoiceSplits>();
					foreach (SOInvoicedRecords.INTransaction tr in record.Transactions.Values)
					{
						foreach (Tuple<INTranSplit, bool> s in tr.Splits)
						{
							if (s.Item2)//IsAvailable
							{
								InvoiceSplits invSplit = CreateInvoiceSplits(record.ARTran, record.SOLine, record.SalesPerTran, tr.Transaction, s.Item1);
								expandedList.Add(invSplit);
					
								if (!string.IsNullOrEmpty(s.Item1.LotSerialNbr) && Document.Current.Behavior == SOBehavior.CM && record.Item.StkItem == false)
								{
									expand = true; //force expand.
								}
							}
						}
					}

					if (expand)
					{
						foreach (InvoiceSplits split in expandedList)
						{
							split.LineNbr = lineNbr++;
							InvoiceSplits invSplit = invoicesplits.Insert(split);
							list.Add(invSplit);
						}
					}
					else
					{
						InvoiceSplits invSplit = CreateInvoiceSplits(record.ARTran, record.SOLine, record.SalesPerTran, null, null);
						invSplit.LineNbr = lineNbr++;
						invSplit = invoicesplits.Insert(invSplit);
						list.Add(invSplit);
					}
				}
							
			}

			return list;
		}

		public virtual InvoiceSplits CreateInvoiceSplits(ARTran artran, SOLine line, SOSalesPerTran sptran, INTran tran, INTranSplit split)
		{
					InvoiceSplits invSplit = new InvoiceSplits();
					invSplit.TranTypeARTran = artran.TranType;
					invSplit.RefNbrARTran = artran.RefNbr;
					invSplit.LineNbrARTran = artran.LineNbr;

					invSplit.OrderTypeSOLine = line.OrderType;
					invSplit.OrderNbrSOLine = line.OrderNbr;
					invSplit.LineNbrSOLine = line.LineNbr;
					invSplit.LineTypeSOLine = line.LineType;
			invSplit.TranDesc = line.TranDesc;
			invSplit.InventoryID = line.InventoryID;
			invSplit.SiteID = line.SiteID;
			invSplit.LocationID = line.LocationID;
			invSplit.LotSerialNbr = line.LotSerialNbr;
			invSplit.UOM = artran.UOM;
			invSplit.Qty = artran.Qty;
			invSplit.BaseQty = artran.BaseQty;

			if (tran != null)
			{
					invSplit.DocTypeINTran = tran.DocType;
					invSplit.RefNbrINTran = tran.RefNbr;
					invSplit.LineNbrINTran = tran.LineNbr;
				invSplit.InventoryID = tran.InventoryID;
				invSplit.SubItemID = split.SubItemID ?? tran.SubItemID;
				invSplit.SiteID = tran.SiteID;
				invSplit.LocationID = split.LocationID ?? tran.LocationID;
					invSplit.LotSerialNbr = split.LotSerialNbr;
				invSplit.UOM = split.UOM ?? tran.UOM;
				invSplit.Qty = split.Qty ?? tran.Qty;
				invSplit.BaseQty = split.BaseQty ?? tran.BaseQty;

					invSplit.DocTypeINTranSplit = split.DocType ?? String.Empty;
					invSplit.RefNbrINTranSplit = split.RefNbr ?? String.Empty;
					invSplit.LineNbrINTranSplit = split.LineNbr ?? 0;
					invSplit.SplitLineNbrINTranSplit = split.SplitLineNbr ?? 0;
			}

			if (sptran != null)
			{
					invSplit.OrderTypeSOSalesPerTran = sptran.OrderType;
					invSplit.OrderNbrSOSalesPerTran = sptran.OrderNbr;
					invSplit.SalespersonIDSOSalesPerTran = sptran.SalespersonID;
		}

			return invSplit;
		}

		protected virtual void POLine3_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			//TODO: change to warnings and disable selection
			//if (supply.OrderType != PO.POOrderType.DropShip && 
			//        (supply.BaseOpenQty - demand.PlanQty > 0m && soline.ShipComplete != SOShipComplete.ShipComplete || supply.BaseOpenQty - demand.PlanQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m) &&
			//        supply.Completed == false && supply.Cancelled == false ||
			//    supply.OrderType == PO.POOrderType.DropShip && IsLinkedToSO == false && (supply.Completed == false && 
			//        (supply.BaseOrderQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m || soline.ShipComplete != SOShipComplete.ShipComplete) && supply.BaseOrderQty <= soline.BaseOrderQty * soline.CompleteQtyMax / 100m || 
			//        (supply.BaseReceivedQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m || soline.ShipComplete == SOShipComplete.CancelRemainder) && supply.BaseReceivedQty <= soline.BaseOrderQty * soline.CompleteQtyMax / 100m) || 
			//    link != null) 
			POLine3 supply = e.Row as POLine3;
            if (supply == null) return;
			SOLine soline = PXSelect<SOLine, Where<SOLine.orderType, Equal<Current<POLine3.sOOrderType>>, And<SOLine.orderNbr, Equal<Current<POLine3.sOOrderNbr>>, And<SOLine.lineNbr, Equal<Current<POLine3.sOOrderLineNbr>>>>>>.Select(this);

			bool RegularOrderMinMax = supply != null && soline != null && supply.OrderType != PO.POOrderType.DropShip && 
					(supply.BaseOpenQty - supply.DemandQty > 0m && soline.ShipComplete != SOShipComplete.ShipComplete || supply.BaseOpenQty - supply.DemandQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m);

			bool DropShipOrderMinMax = supply != null && soline != null && supply.OrderType == PO.POOrderType.DropShip && 
					(supply.Completed == false && (supply.BaseOrderQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m || soline.ShipComplete != SOShipComplete.ShipComplete) && supply.BaseOrderQty <= soline.BaseOrderQty * soline.CompleteQtyMax / 100m || 
					(supply.BaseReceivedQty >= soline.BaseOrderQty * soline.CompleteQtyMin / 100m || soline.ShipComplete == SOShipComplete.CancelRemainder) && supply.BaseReceivedQty <= soline.BaseOrderQty * soline.CompleteQtyMax / 100m);

            bool PartiallyReceipted = supply != null && soline != null && supply.Selected == true && supply.BaseOrderQty - supply.BaseOpenQty > 0 && supply.SOOrderSplitLineNbr != null;

            PXUIFieldAttribute.SetEnabled<POLine3.selected>(sender, e.Row, (RegularOrderMinMax || DropShipOrderMinMax || supply.SOOrderSplitLineNbr != null) && !PartiallyReceipted);
            if (PartiallyReceipted) PXUIFieldAttribute.SetWarning<POLine3.selected>(sender, e.Row, Messages.PurchaseOrderCannotBeDeselected);
		}

		public virtual IEnumerable carrierRates()
		{
			if (Document.Current != null)
			{
				return CarrierRates.Cache.Cached;
			}

			return new List<SOCarrierRate>();
		}

		public PXSetupOptional<TXAvalaraSetup> avalaraSetup; 
		#endregion

		
		#region Buttons And Delegates
		public PXAction<SOOrder> pOSupplyOK;
		[PXUIField(DisplayName = "PO Link", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable POSupplyOK(PXAdapter adapter)
		{

			if (Transactions.Current != null &&
				Transactions.Current.POCreate == true &&
				currentposupply.AskExt((graph, viewName) =>
					{
						foreach (POLine3 supply in posupply.Cache.Updated.ToArray<POLine3>())
						{
							if (supply.SOOrderSplitLineNbr == null)
							{
								posupply.Cache.SetStatus(supply, PXEntryStatus.Notchanged);
								posupply.Cache.Remove(supply);
							}
						}
					}) == WebDialogResult.OK)
			{
				LinkSupplyDemand();
			}
			
			return adapter.Get();
		}

		public virtual void LinkSupplyDemand()
		{
				//PXSelect<SOLineSplit, Where<SOLineSplit.orderType, Equal<Required<SOLineSplit.orderType>>, And<SOLineSplit.orderNbr, Equal<Required<SOLineSplit.orderNbr>>, And<SOLineSplit.lineNbr, Equal<Required<SOLineSplit.lineNbr>>>>>>.Select(this, supply.SOOrderType, supply.SOOrderNbr, supply.SOOrderLineNbr)
				List<SOLineSplit> splits = this.splits.Select().RowCast<SOLineSplit>().ToList();

				//unlink first
				foreach (POLine3 supply in posupply.Cache.Updated)
				{
					SOLine line = (SOLine)currentposupply.Select(supply.SOOrderType, supply.SOOrderNbr, supply.SOOrderLineNbr);

					line = PXCache<SOLine>.CreateCopy(line);

					if (supply.Selected == false && supply.SOOrderSplitLineNbr != null)
					{
						foreach (SOLineSplit split in splits)
						{
							if (split.POType == supply.OrderType && split.PONbr == supply.OrderNbr && split.POLineNbr == supply.LineNbr &&
								split.POCompleted == false && split.Completed == false || supply.OrderType == POOrderType.DropShip)
							{
                                if (split.POType != null && split.PONbr != null && split.POType == supply.OrderType && split.PONbr == supply.OrderNbr)
                                {
                                    POOrder poorder = PXSelect<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                                            And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, supply.OrderType, supply.OrderNbr);

                                    if (poorder != null && poorder.OrderType == split.POType && poorder.OrderNbr == split.PONbr)
                                    {
                                        if (split.RefNoteID == poorder.NoteID)
                                            split.RefNoteID = null;
                                        if (poorder.SOOrderType == split.OrderType && poorder.SOOrderNbr == split.OrderNbr)
                                        {
                                            poorder.SOOrderType = null;
                                            poorder.SOOrderNbr = null;
                                            poorderlink.Update(poorder);
                                        }
                                    }
                                }
                                
								split.POType = null;
								split.PONbr = null;
								split.POLineNbr = null;
								split.POCompleted = false;
								split.ReceivedQty = 0m;
								split.ShippedQty = 0m;
								split.Completed = false;

								this.splits.Update(split);

								if (supply.OrderType == PO.POOrderType.DropShip)
								{
									line.ShippedQty = 0m;
									line.UnbilledQty = line.OrderQty;
									line.OpenQty = line.OrderQty;
									line.ClosedQty = 0m;
									line.Completed = false;

									Transactions.Update(line);
								}
							}
						}
						supply.SOOrderSplitLineNbr = null;
					}
				}

				//then link
				foreach (POLine3 supply in posupply.Cache.Updated)
				{
						SOLine line = (SOLine)currentposupply.Select(supply.SOOrderType, supply.SOOrderNbr, supply.SOOrderLineNbr);

					line = PXCache<SOLine>.CreateCopy(line);

					if (supply.Selected == true && supply.SOOrderSplitLineNbr == null)
					{
						decimal? BaseOpenQty = supply.BaseOrderQty - supply.DemandQty;

						for (int i = 0; i < splits.Count; i++)
						{
							SOLineSplit split = PXCache<SOLineSplit>.CreateCopy(splits[i]);

							//TODO: it should not be possible to unallocate TR schedules
							if (string.IsNullOrEmpty(split.SOOrderNbr) && string.IsNullOrEmpty(split.PONbr) && split.IsAllocated == false && split.Completed == false && split.BaseQty > 0m)
							{
								if (supply.OrderType != POOrderType.Blanket)
								{
								supply.LineType =
									(line.POSource == INReplenishmentSource.DropShipToOrder) ?
									(line.LineType == SOLineType.Inventory ? POLineType.GoodsForDropShip : POLineType.NonStockForDropShip) :
									(line.LineType == SOLineType.Inventory ? POLineType.GoodsForSalesOrder : POLineType.NonStockForSalesOrder);
								}

								supply.SOOrderSplitLineNbr = split.SplitLineNbr;

								INItemPlan plan;
								if (supply.Completed == false)
								{
									plan = PXSelect<INItemPlan, Where<INItemPlan.planID, Equal<Required<INItemPlan.planID>>>>.Select(this, supply.PlanID);
									if (plan == null) continue;
									if (supply.OrderType != PX.Objects.PO.POOrderType.Blanket)
									{
										plan = PXCache<INItemPlan>.CreateCopy(plan);
										plan.PlanType = (line.POSource == INReplenishmentSource.DropShipToOrder ? INPlanConstants.Plan74 : INPlanConstants.Plan76);
										this.Caches[typeof(INItemPlan)].Update(plan);
									}

									if (supply.OrderType == PO.POOrderType.DropShip)
									{
                                        POReceiptLine receipted = PXSelectJoinGroupBy<POReceiptLine,
                                            InnerJoin<POReceipt, On<POReceipt.receiptType, Equal<POReceiptLine.receiptType>, And<POReceipt.receiptNbr, Equal<POReceiptLine.receiptNbr>>>>,
                                            Where<POReceiptLine.pOType, Equal<Current<POLine3.orderType>>,
                                            And<POReceiptLine.pONbr, Equal<Current<POLine3.orderNbr>>,
                                            And<POReceiptLine.pOLineNbr, Equal<Current<POLine3.lineNbr>>,
                                            And<POReceipt.released, Equal<True>>>>>,
                                            Aggregate<Sum<POReceiptLine.baseReceiptQty>>>.SelectSingleBound(this, new object[] { supply });

                                        split.BaseShippedQty = receipted.BaseReceiptQty ?? 0m;
										PXDBQuantityAttribute.CalcTranQty<SOLineSplit.shippedQty>(this.splits.Cache, split);
                                        split.OpenQty = (split.Qty - split.ShippedQty);

										line.BaseShippedQty += split.BaseShippedQty;
										PXDBQuantityAttribute.CalcTranQty<SOLine.shippedQty>(Transactions.Cache, line);
										line.OpenQty = (line.OrderQty - line.ShippedQty);
										line.ClosedQty = line.ShippedQty;

										Transactions.Update(line);
									}
								}

								plan = PXSelect<INItemPlan, Where<INItemPlan.planID, Equal<Required<INItemPlan.planID>>>>.Select(this, split.PlanID);

							if (supply.OrderType == PO.POOrderType.DropShip)
								{
								foreach ( POReceiptLine porl in PXSelectJoin<POReceiptLine, 
                                    InnerJoin<POReceipt, 
                                        On<POReceipt.receiptType, Equal<POReceiptLine.receiptType>, 
                                        And<POReceipt.receiptNbr, Equal<POReceiptLine.receiptNbr>,
                                        And<POReceipt.released, Equal<True>>>>>, 
                                    Where<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
										And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
										And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>>>>>.Select(this, supply.OrderType, supply.OrderNbr, supply.LineNbr))
								{
									SOOrderShipment os = shipmentlist.Select().Where(s => ((SOOrderShipment)s).ShipmentNbr == porl.ReceiptNbr).FirstOrDefault();
									if (os == null)
									{
										os = new SOOrderShipment();
										os.OrderType = Document.Current.OrderType;
										os.OrderNbr = Document.Current.OrderNbr;
										os.ShipAddressID = Document.Current.ShipAddressID;
										os.ShipContactID = Document.Current.ShipContactID;
										os.ShipmentType = INDocType.DropShip;
										os.ShipmentNbr = porl.ReceiptNbr;
										os.Operation = SOOperation.Issue;
										os.ShipDate = porl.ReceiptDate;
										os.CustomerID = Document.Current.CustomerID;
										os.CustomerLocationID = Document.Current.CustomerLocationID;
										os.SiteID = null;
										os.ShipmentWeight = porl.ExtWeight;
										os.ShipmentVolume = porl.ExtVolume;
										os.ShipmentQty = porl.ReceiptQty;
										os.LineTotal = 0m;
										os.Confirmed = true;

										os.OrderType = Document.Current.OrderType;
										os.OrderNbr = Document.Current.OrderNbr;
										shipmentlist.Insert(os);

										Document.Current.ShipmentCntr++;
									}
									else
									{
										os.ShipmentWeight += porl.ExtWeight;
										os.ShipmentVolume += porl.ExtVolume;
										os.ShipmentQty += porl.ReceiptQty;
										shipmentlist.Update(os);
									}
									}


								if (supply.Completed == true)
								{
									this.Caches[typeof (INItemPlan)].Delete(plan);

									split.BaseShippedQty = supply.BaseReceivedQty;
									PXDBQuantityAttribute.CalcTranQty<SOLineSplit.shippedQty>(this.splits.Cache, split);
									split.Completed = true;
									split.PlanID = null;

									line.BaseShippedQty += split.BaseShippedQty;
									PXDBQuantityAttribute.CalcTranQty<SOLine.shippedQty>(Transactions.Cache, line);
									line.UnbilledQty -= (line.OrderQty - line.ShippedQty);
									line.OpenQty = 0m;
									line.ClosedQty = line.OrderQty;
									line.Completed = true;

									Transactions.Update(line);
								}
							}
								else
								{
									plan = PXCache<INItemPlan>.CreateCopy(plan);

									plan.PlanType = (supply.OrderType == PO.POOrderType.Blanket) ?
										(line.POSource == INReplenishmentSource.PurchaseToOrder ? INPlanConstants.Plan6B : INPlanConstants.Plan6E) :
										(line.POSource == INReplenishmentSource.DropShipToOrder ? INPlanConstants.Plan6D : INPlanConstants.Plan66);

									plan.FixedSource = INReplenishmentSource.Purchased;
									plan.SupplyPlanID = supply.PlanID;

									POOrder poorder = PXSelect<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
											And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, supply.OrderType, supply.OrderNbr);

									if (poorder != null)
									{
										plan.VendorID = poorder.VendorID;
										plan.VendorLocationID = poorder.VendorLocationID;
									}
									this.Caches[typeof(INItemPlan)].Update(plan);
								}

								split.POCreate = true;
								split.VendorID = supply.VendorID;
								split.POType = supply.OrderType;
								split.PONbr = supply.OrderNbr;
								split.POLineNbr = supply.LineNbr;

								if (split.BaseQty <= BaseOpenQty)
								{
									BaseOpenQty -= split.BaseQty;

									this.splits.Update(split);
								}
								else
								{
									SOLineSplit copy = PXCache<SOLineSplit>.CreateCopy(split);

									copy.SplitLineNbr = null;
									copy.IsAllocated = false;
									copy.POCreate = true;
									copy.POCompleted = false;
									copy.POSource = null;
									copy.VendorID = null;
									copy.POType = null;
									copy.PONbr = null;
									copy.POLineNbr = null;
									copy.BaseQty = copy.BaseQty - BaseOpenQty;
									copy.Qty = INUnitAttribute.ConvertFromBase(this.splits.Cache, copy.InventoryID, copy.UOM, (decimal)copy.BaseQty, INPrecision.QUANTITY);
									copy.ShippedQty = 0m;
									copy.ReceivedQty = 0m;
                                    copy.UnreceivedQty = copy.BaseQty;
									copy.PlanID = null;
									copy.Completed = false;

									split.BaseQty = BaseOpenQty;
									split.Qty = INUnitAttribute.ConvertFromBase(this.splits.Cache, split.InventoryID, split.UOM, (decimal)split.BaseQty, INPrecision.QUANTITY);
									BaseOpenQty = 0m;
									this.splits.Update(split);

									if ((copy = this.splits.Insert(copy)) != null)
									{
										splits.Insert(i + 1, copy);
									}
								}
							}

							if (BaseOpenQty <= 0m) break;
						}
					}
                    else if (supply.Selected != true)
					{
						posupply.Cache.SetStatus(supply, PXEntryStatus.Notchanged);
						posupply.Cache.Remove(supply);
					}
				}

			}

		public PXAction<SOOrder> hold;
		[PXUIField(DisplayName = "Hold")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<SOOrder> cancelled;
		[PXUIField(Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Cancelled(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<SOOrder> creditHold;
		[PXUIField(DisplayName = "Credit Hold")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable CreditHold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<SOOrder> flow;
		[PXUIField(DisplayName = "Flow")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Flow(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 5, 6 }, new string[] { "OnShipment", "OnDeleteInvoice" })]
			int? actionID,
			[PXString(1)]
			[SOOrderStatus.List()]
			string orderStatus1,
			[PXString(1)]
			[SOOrderStatus.List()]
			string orderStatus2,
			[PXString(1)]
			[SOShipmentStatus.List()]
			string shipmentStatus1,
			[PXString(1)]
			[SOShipmentStatus.List()]
			string shipmentStatus2)
		{
			switch (actionID)
			{
				case 5: //OnShipment //OBSOLETE - REMOVE
					{
						List<SOOrder> list = new List<SOOrder>();
						foreach (SOOrder order in adapter.Get<SOOrder>())
						{
							list.Add(order);
						}

						Save.Press();

						PXLongOperation.StartOperation(this, delegate()
						{
							SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();

							foreach (SOOrder order in list)
							{
								SOOrder item = docgraph.Document.Search<SOOrder.orderNbr>(order.OrderNbr, order.OrderType);

								if ((int)item.OpenShipmentCntr > 0)
								{
									item.Status = orderStatus1;
								}
								else
								{
									item.Status = orderStatus2;
								}

								docgraph.Document.Cache.SetStatus(item, PXEntryStatus.Updated);

								docgraph.Document.Search<SOOrder.orderNbr>(order.OrderNbr, order.OrderType);

								docgraph.Save.Press();
							}
						});
					}
					break;
				case 6: //OnDeleteInvoice //OBSOLETE - REMOVE
					{
						List<SOOrder> list = new List<SOOrder>();
						foreach (SOOrder order in adapter.Get<SOOrder>())
						{
							list.Add(order);
						}

						Save.Press();

						PXLongOperation.StartOperation(this, delegate()
						{
							SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();

							foreach (SOOrder order in list)
							{
								SOOrder item = docgraph.Document.Search<SOOrder.orderNbr>(order.OrderNbr, order.OrderType);

								PXResultset<SOOrderShipment> shipments = PXSelect<SOOrderShipment, Where<SOOrderShipment.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>.SelectMultiBound(docgraph, new object[] { order });

								if (shipments.Count == 0)
								{
									item.Status = orderStatus1;

									docgraph.Document.Cache.SetStatus(item, PXEntryStatus.Updated);
								}
								else
								{
									foreach (SOOrderShipment ordershipment in shipments)
									{
										if (string.IsNullOrEmpty(ordershipment.InvoiceNbr))
										{
											SOShipment shipment = (SOShipment)PXSelect<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>>>.SelectSingleBound(docgraph, new object[] { ordershipment });

											shipment.Status = shipmentStatus1;

											docgraph.Caches[typeof(SOShipment)].SetStatus(shipment, PXEntryStatus.Updated);
										}
									}

									if (!docgraph.Views.Caches.Contains(typeof(SOShipment)))
									{
										docgraph.Views.Caches.Add(typeof(SOShipment));
									}
								}
								docgraph.Save.Press();
							}
						});
					}
					break;
				default:
					Save.Press();
					break;
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2, 3, 4, 5 }, new string[] { "Create Shipment", "Apply Assignment Rules", "Create Invoice", "Post Invoice to IN", "Create Purchase Order" })]
			int? actionID,
			[PXDate]
			DateTime? shipDate,
			[PXSelector(typeof(INSite.siteCD))]			
			string siteCD,
			[SOOperation.List]
			string operation,
			[PXString()]
			string ActionName
			)
		{			
			if (!string.IsNullOrEmpty(ActionName))
			{
				PXAction action = this.Actions[ActionName];

				if (action != null)
				{
					Save.Press();
					List<object> result = new List<object>();
					foreach (object data in action.Press(adapter))
					{
						result.Add(data);
					}
					return result;
				}
			}

			List<SOOrder> list = new List<SOOrder>();
			foreach (SOOrder order in adapter.Get<SOOrder>())
			{
				list.Add(order);
			}

			switch (actionID)
			{
				case 1:
					{
						if (shipDate != null)
						{
							soparamfilter.Current.ShipDate = shipDate;
						}

						if (soparamfilter.Current.ShipDate == null)
						{
							soparamfilter.Current.ShipDate = Accessinfo.BusinessDate;
						}

						if (siteCD != null)
						{
							soparamfilter.Cache.SetValueExt<SOParamFilter.siteID>(soparamfilter.Current, siteCD);
						}

						if (!adapter.MassProcess)
						{
							if (soparamfilter.Current.SiteID == null)
							{
								PXResultset<SOOrderSite> osites = PXSelectJoin<SOOrderSite,
									InnerJoin<INSite, On<INSite.siteID, Equal<SOOrderSite.siteID>>>,
									Where<SOOrderSite.orderType, Equal<Current<SOOrder.orderType>>,
										And<SOOrderSite.orderNbr, Equal<Current<SOOrder.orderNbr>>,
										And<Match<INSite, Current<AccessInfo.userName>>>>>>.Select(this);
								SOOrderSite preferred;
								if (osites.Count == 1)
								{
									soparamfilter.Current.SiteID = ((SOOrderSite)osites).SiteID;
								}
								else if ((preferred = PXSelectJoin<SOOrderSite,
									InnerJoin<INSite, On<INSite.siteID, Equal<SOOrderSite.siteID>>>,
									Where<SOOrderSite.orderType, Equal<Current<SOOrder.orderType>>,
										And<SOOrderSite.orderNbr, Equal<Current<SOOrder.orderNbr>>,
										And<SOOrderSite.siteID, Equal<Current<SOOrder.defaultSiteID>>,
										And<Match<INSite, Current<AccessInfo.userName>>>>>>>.Select(this)) != null)
								{
									soparamfilter.Current.SiteID = preferred.SiteID;
								}
							}
							soparamfilter.AskExt(true);
						}
						if (soparamfilter.Current.SiteID != null || adapter.MassProcess)
						{
							Save.Press();
							PXAutomation.RemovePersisted(this, typeof(SOOrder), new List<object>(list));

							SOParamFilter filter = soparamfilter.Current;
							PXLongOperation.StartOperation(this, delegate()
							{
								bool anyfailed = false;
								SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
								DocumentList<SOShipment> created = new DocumentList<SOShipment>(docgraph);
								foreach (SOOrder order in list)
								{
									if (adapter.MassProcess)
									{
										PXProcessing<SOOrder>.SetCurrentItem(order);
									}

									List<int?> sites = new List<int?>();

									if (filter.SiteID != null)
									{
										sites.Add(filter.SiteID);
									}
									else
									{
										foreach (SOShipmentPlan plan in PXSelectGroupBy<SOShipmentPlan, Where<SOShipmentPlan.orderType, Equal<Current<SOOrder.orderType>>, And<SOShipmentPlan.orderNbr, Equal<Current<SOOrder.orderNbr>>>>, Aggregate<GroupBy<SOShipmentPlan.siteID>>, OrderBy<Asc<SOShipmentPlan.siteID>>>.SelectMultiBound(docgraph, new object[] { order }))
										{
											sites.Add(plan.SiteID);
										}
									}

									foreach (int? SiteID in sites)
									{
										try
										{
											using (var ts = new PXTransactionScope())
											{
												PXTimeStampScope.SetRecordComesFirst(typeof(SOOrder), true);
												docgraph.CreateShipment(order, SiteID, filter.ShipDate, adapter.MassProcess, operation, created);
												PXAutomation.CompleteAction(docgraph);
												var items = new List<object> { order };
												PXAutomation.RemovePersisted(docgraph, typeof(SOOrder), items);
												PXAutomation.RemoveProcessing(docgraph, typeof(SOOrder), items);
												ts.Complete();
											}

											if (adapter.MassProcess)
											{
												PXProcessing<SOOrder>.SetProcessed();
											}
										}
										catch (SOShipmentException ex)
										{
											if (!adapter.MassProcess)
											{
												throw;
											}
											order.LastSiteID = SiteID;
											order.LastShipDate = filter.ShipDate;

											docgraph.Clear(PXClearOption.PreserveTimeStamp);

											List<object> failed = new List<object>();
											failed.Add(order);
											PXAutomation.StorePersisted(docgraph, typeof(SOOrder), failed);

											PXAutomation.CompleteAction(docgraph);
											PXAutomation.RemovePersisted(docgraph, typeof(SOOrder), failed);

											PXTrace.WriteInformation(ex);
											PXProcessing<SOOrder>.SetWarning(ex);
										}
										catch (Exception ex)
										{
											if (!adapter.MassProcess)
											{
												throw;
											}
											PXProcessing<SOOrder>.SetError(ex);
											anyfailed = true;
										}


									}
								}
								if (!adapter.MassProcess && created.Count > 0)
								{
									using (new PXTimeStampScope(null))
									{
										docgraph.Clear();
										docgraph.Document.Current = docgraph.Document.Search<SOShipment.shipmentNbr>(created[0].ShipmentNbr);
										throw new PXRedirectRequiredException(docgraph, "Shipment");
									}
								}

								if (anyfailed)
								{
									throw new PXOperationCompletedWithErrorException(ErrorMessages.SeveralItemsFailed);
								}
							});
						}
					}
					break;
				case 2:
					{
						if (sosetup.Current.DefaultOrderAssignmentMapID == null)
						{
							throw new PXSetPropertyException(Messages.AssignNotSetup, Messages.SOSetup);
						}
						EPAssignmentProcessHelper<SOOrder> aph = new EPAssignmentProcessHelper<SOOrder>();
						aph.Assign(Document.Current, sosetup.Current.DefaultOrderAssignmentMapID);
						Document.Update(Document.Current);
					}
					break;
				case 3:
					{
						Save.Press();

						PXLongOperation.StartOperation(this, delegate()
						{
							SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
							DocumentList<ARInvoice, SOInvoice> created = new DocumentList<ARInvoice, SOInvoice>(docgraph);

                            SOOrderEntry.InvoiceOrder(adapter.Arguments, list, created, adapter.MassProcess);

							if (!adapter.MassProcess && created.Count > 0)
							{
								using (new PXTimeStampScope(null))
								{
									SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();
									ie.Document.Current = ie.Document.Search<ARInvoice.docType, ARInvoice.refNbr>(((ARInvoice)created[0]).DocType, ((ARInvoice)created[0]).RefNbr, ((ARInvoice)created[0]).DocType);
									throw new PXRedirectRequiredException(ie, "Invoice");
								}
							}
						});
					}
					break;
				case 4:
					{
						Save.Press();

						PXLongOperation.StartOperation(this, delegate()
						{
							SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();
							DocumentList<INRegister> created = new DocumentList<INRegister>(docgraph);
							INIssueEntry ie = PXGraph.CreateInstance<INIssueEntry>();
							ie.FieldVerifying.AddHandler<INTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
							ie.FieldVerifying.AddHandler<INTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
                            foreach (SOOrder order in list)
                            {
                                try
                                {
                                    if (adapter.MassProcess) PXProcessing<SOOrder>.SetCurrentItem(order);

                                    docgraph.PostOrder(ie, order, created);
                                }
                                catch (Exception ex)
                                {
                                    if (!adapter.MassProcess)
                                    {
                                        throw;
                                    }
                                    PXProcessing<SOOrder>.SetError(ex);
                                }
                            }

							if (docgraph.sosetup.Current.AutoReleaseIN == true && created.Count > 0 && created[0].Hold == false)
							{
								INDocumentRelease.ReleaseDoc(created, false);
							}
						});
					}
					break;
				case 5:
					if (list.Count > 0)
					{
                        Save.Press();
						POCreate graph = PXGraph.CreateInstance<POCreate>();
						graph.Filter.Current.OrderType = list[0].OrderType;
						graph.Filter.Current.OrderNbr = list[0].OrderNbr;
						throw new PXRedirectRequiredException(graph, PO.Messages.PurchaseOrderCreated);
					}
					break;
				case 6:
					if (list.Count > 0)
					{
						Save.Press();
						SOCreate graph = PXGraph.CreateInstance<SOCreate>();
						graph.Filter.Current.OrderType = list[0].OrderType;
						graph.Filter.Current.OrderNbr = list[0].OrderNbr;
						throw new PXRedirectRequiredException(graph, Messages.TransferOrderCreated);
					}
					break;
				default:
					Save.Press();
					break;
			}
			return list;
		}

		public PXAction<SOOrder> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton(MenuAutoOpen = true)]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { }, new string[] { })]
			int? inquiryID,
			[PXString()]
			string ActionName
			)
		{
			if (!string.IsNullOrEmpty(ActionName))
			{
				PXAction action = this.Actions[ActionName];

				if (action != null)
				{
					Save.Press();
					foreach (object data in action.Press(adapter)) ;
				}
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Report(PXAdapter adapter,
			[PXString(8, InputMask = "CC.CC.CC.CC")]
			string reportID
			)
		{
            List<SOOrder> list = adapter.Get<SOOrder>().ToList();
			if (!String.IsNullOrEmpty(reportID))
			{
				Save.Press();
				int i = 0;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				string actualReportID = null;
				foreach (SOOrder order in list)
				{
					parameters["SOOrder.OrderType" + i.ToString()] = order.OrderType;
					parameters["SOOrder.OrderNbr" + i.ToString()] = order.OrderNbr;
					if(actualReportID == null)
						actualReportID = new NotificationUtility(this).SearchReport(ARNotificationSource.Customer, customer.Current, reportID, order.BranchID);
					i++;
				}
				if (i > 0)
				{
					throw new PXReportRequiredException(parameters, actualReportID,
						"Report " + actualReportID);
				}
			}
			return list;
		}
		public PXAction<SOOrder> notification;
		[PXUIField(DisplayName = "Notifications", Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Notification(PXAdapter adapter,
		[PXString]
		string notificationCD
		)
		{
			foreach (SOOrder order in adapter.Get<SOOrder>())
			{
				var parameters = new Dictionary<string, string>();
				parameters["SOOrder.OrderType"] = order.OrderType;
				parameters["SOOrder.OrderNbr"] = order.OrderNbr;
				Activity.SendNotification(ARNotificationSource.Customer, notificationCD, order.BranchID, parameters);
				yield return order;
			}
		}
		public PXAction<SOOrder> prepareInvoice;
		[PXUIField(DisplayName = "Prepare Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable PrepareInvoice(PXAdapter adapter)
		{
			List<SOOrder> list = adapter.Get<SOOrder>().ToList();

            foreach (SOOrder order in list)
            {
                if (this.Document.Cache.GetStatus(order) != PXEntryStatus.Inserted)
                    this.Document.Cache.SetStatus(order, PXEntryStatus.Updated);
            }
			
			if (!adapter.MassProcess)
			{
				try
				{
					RecalculateAvalaraTaxesSync = true;
					Save.Press();
				}
				finally
				{
					RecalculateAvalaraTaxesSync = false;
				}
			}
			
			PXLongOperation.StartOperation(this, delegate()
			{
				SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
				DocumentList<ARInvoice, SOInvoice> created = new DocumentList<ARInvoice, SOInvoice>(docgraph);

				SOOrderEntry.InvoiceOrder(adapter.Arguments, list, created, adapter.MassProcess);

				if (!adapter.MassProcess && created.Count > 0)
				{
					using (new PXTimeStampScope(null))
					{
						SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();
						ie.Document.Current = ie.Document.Search<ARInvoice.docType, ARInvoice.refNbr>(((ARInvoice)created[0]).DocType, ((ARInvoice)created[0]).RefNbr, ((ARInvoice)created[0]).DocType);
						throw new PXRedirectRequiredException(ie, "Invoice");
					}
				}
			});
			return list;
		}

		public PXAction<SOOrder> addInvoice;
		[PXUIField(DisplayName = Messages.AddInvoice, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddInvoice(PXAdapter adapter)
		{
			try
			{
				if ((IsCreditMemoOrder || IsRMAOrder) && Transactions.Cache.AllowInsert && invoicesplits.AskExt() == WebDialogResult.OK)
				{
					SOLine prev_line = null;
					Dictionary<object, ARTran> discounts_aggregated = new Dictionary<object, ARTran>();
					Dictionary<ARTran, bool> invoice_lines_distinct = new Dictionary<ARTran, bool>(new LSSOLine.Comparer<ARTran>(this));
					SOLine existing = null;

					foreach (InvoiceSplits res in invoicesplits.Cache.Cached)
					{

						if (res.Selected == true)
						{
							INTranSplit split = PXSelect<INTranSplit, Where<INTranSplit.docType, Equal<Required<INTranSplit.docType>>, And<INTranSplit.lineNbr, Equal<Required<INTranSplit.lineNbr>>, And<INTranSplit.refNbr, Equal<Required<INTranSplit.refNbr>>, And<INTranSplit.splitLineNbr, Equal<Required<INTranSplit.splitLineNbr>>>>>>>.Select(this, res.DocTypeINTranSplit, res.LineNbrINTranSplit, res.RefNbrINTranSplit, res.SplitLineNbrINTranSplit);
							INTran tran =  PXSelect<INTran, Where<INTran.docType, Equal<Required<INTran.docType>>, And<INTran.lineNbr, Equal<Required<INTran.lineNbr>>, And<INTran.refNbr, Equal<Required<INTran.refNbr>>>>>>.Select(this, res.DocTypeINTran, res.LineNbrINTran, res.RefNbrINTran);
							SOLine line =  PXSelect<SOLine, Where<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.orderType, Equal<Required<SOLine.orderType>>>>>>.Select(this, res.LineNbrSOLine, res.OrderNbrSOLine, res.OrderTypeSOLine);
							ARTran artran =  PXSelect<ARTran, Where<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>, And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>, And<ARTran.tranType, Equal<Required<ARTran.tranType>>>>>>.Select(this, res.LineNbrARTran, res.RefNbrARTran, res.TranTypeARTran);
							ARRegister invoice = PXSelect<ARRegister, Where<ARRegister.docType, Equal<Required<ARRegister.docType>>, And<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>>.Select(this, res.TranTypeARTran, res.RefNbrARTran);
							SOSalesPerTran pertran = PXSelect<SOSalesPerTran, Where<SOSalesPerTran.orderNbr, Equal<Required<SOSalesPerTran.orderNbr>>, And<SOSalesPerTran.orderType, Equal<Required<SOSalesPerTran.orderType>>, And<SOSalesPerTran.salespersonID, Equal<Required<SOSalesPerTran.salespersonID>>>>>>.Select(this, res.OrderNbrSOSalesPerTran, res.OrderTypeSOSalesPerTran, res.SalespersonIDSOSalesPerTran);
							DRSchedule drSchedule = null;
							if (!string.IsNullOrEmpty(artran.DeferredCode))
							{
								drSchedule = PXSelect<DRSchedule, Where<DRSchedule.module, Equal<BatchModule.moduleAR>,
									And<DRSchedule.docType, Equal<Required<ARTran.tranType>>,
									And<DRSchedule.refNbr, Equal<Required<ARTran.refNbr>>,
									And<DRSchedule.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>>.Select(this, artran.TranType, artran.RefNbr, artran.LineNbr);
							}


							if (Transactions.Cache.ObjectsEqual(prev_line, line) == false || res.InventoryID != line.InventoryID)
							{
								existing = PXSelect<SOLine, Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>, 
									And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>, 
									And<SOLine.origOrderType, Equal<Required<SOLine.origOrderType>>, 
									And<SOLine.origOrderNbr, Equal<Required<SOLine.origOrderNbr>>, 
									And<SOLine.origLineNbr, Equal<Required<SOLine.origLineNbr>>,
									And<SOLine.inventoryID, Equal<Required<SOLine.inventoryID>>,
									And<SOLine.invoiceNbr, Equal<Required<SOLine.invoiceNbr>>>>>>>>>>.Select(this, line.OrderType, line.OrderNbr, line.LineNbr, res.InventoryID, (tran!= null ? tran.ARRefNbr : (artran != null ? artran.RefNbr : null)));
								if (existing != null)
								{
									Transactions.Current = existing;

									bool exists;
									if (!invoice_lines_distinct.TryGetValue(artran, out exists))
									{
										invoice_lines_distinct[artran] = true;
										if (discounts_aggregated.ContainsKey(existing))
										{
											discounts_aggregated[existing].DiscAmt += artran.DiscAmt;
											discounts_aggregated[existing].ExtPrice += artran.ExtPrice;
											discounts_aggregated[existing].BaseQty += artran.BaseQty;
										}
										else
										{
											discounts_aggregated[existing] = new ARTran();
											discounts_aggregated[existing].DiscAmt = artran.DiscAmt;
											discounts_aggregated[existing].ExtPrice = artran.ExtPrice;
											discounts_aggregated[existing].BaseQty = artran.BaseQty;

										}
									}
								}
								else
								{
									SOLine newline = new SOLine();
									newline.BranchID = line.BranchID;
									newline.Operation = SOOperation.Receipt;

									if (tran != null) newline.InvoiceNbr = tran.ARRefNbr;
									else if (artran != null) newline.InvoiceNbr = artran.RefNbr;
			
									newline.InvoiceDate = artran.TranDate;
									newline.OrigOrderType = res.OrderTypeSOLine;
									newline.OrigOrderNbr = res.OrderNbrSOLine;
									newline.OrigLineNbr = res.LineNbrSOLine;
									newline.SalesAcctID = null;
									newline.SalesSubID = null;
									newline.TaxCategoryID = line.TaxCategoryID;
									newline.SalesPersonID = res.SalespersonIDSOSalesPerTran;
									newline.Commissionable = artran.Commissionable;

									newline.ManualPrice = true;
									newline.ManualDisc = true;

									newline.InventoryID = res.InventoryID;
									newline.SubItemID = res.SubItemID;
									newline.SiteID = res.SiteID;
									//newline.LocationID = res.LocationID;
									newline.LotSerialNbr = res.LotSerialNbr;
									newline.UOM = (newline.InventoryID == artran.InventoryID) ? artran.UOM : res.UOM;
									newline.CuryInfoID = Document.Current.CuryInfoID;

									if (line.LineType == SOLineType.MiscCharge || line.LineType == SOLineType.NonInventory || tran == null)
									{
										if (newline.InventoryID == artran.InventoryID)
										newline.UnitCost = artran.Qty > 0m ? (artran.TranCost / artran.Qty) : artran.TranCost;
									}
									else
									{
										if (newline.InventoryID == tran.InventoryID)
										newline.UnitCost = tran.Qty > 0m ? (tran.TranCost / tran.Qty) : tran.TranCost;
									}

									newline.DiscPctDR = artran.DiscPctDR;
									if(artran.CuryUnitPriceDR != null && invoice != null)
									{
										if(Document.Current.CuryID == invoice.CuryID)
										{
											newline.CuryUnitPriceDR = artran.CuryUnitPriceDR;
										}
										else
										{
											decimal unitPriceDR = 0m;
											PXDBCurrencyAttribute.CuryConvBase<ARTran.curyInfoID>(this.Caches[typeof(ARTran)], artran, artran.CuryUnitPriceDR ?? 0m, out unitPriceDR);

											decimal orderCuryUnitPriceDR = 0m;
											PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Transactions.Cache, newline, unitPriceDR, out orderCuryUnitPriceDR);
											newline.CuryUnitPriceDR = orderCuryUnitPriceDR;
										}
									}

									newline.DRTermStartDate = artran.DRTermStartDate;
									newline.DRTermEndDate = artran.DRTermEndDate;

									newline.ReasonCode = line.ReasonCode;
									newline.TaskID = line.TaskID;

									if (drSchedule != null)
									{
										newline.DefScheduleID = drSchedule.ScheduleID;
									}

									decimal CuryUnitCost;
									PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Transactions.Cache, newline, (decimal)newline.UnitCost, out CuryUnitCost);
									newline.CuryUnitCost = CuryUnitCost;

									if (invoice != null && newline.InventoryID == artran.InventoryID)
									{
										if (Document.Current.CuryID == invoice.CuryID)
										{
											decimal UnitPrice;
											newline.CuryUnitPrice = artran.CuryUnitPrice;
											PXDBCurrencyAttribute.CuryConvBase<SOLine.curyInfoID>(Transactions.Cache, newline, (decimal)newline.CuryUnitPrice, out UnitPrice);
											newline.UnitPrice = UnitPrice;
										}
										else
										{
											decimal CuryUnitPrice;
											newline.UnitPrice = artran.UnitPrice;
											PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Transactions.Cache, newline, (decimal)newline.UnitPrice, out CuryUnitPrice);
											newline.CuryUnitPrice = CuryUnitPrice;
										}
									}

									existing = Transactions.Insert(newline);

									if (SalesPerTran.Current != null && SalesPerTran.Cache.ObjectsEqual<SOSalesPerTran.salespersonID>(pertran, SalesPerTran.Current))
									{
										SOSalesPerTran salespertran_copy = PXCache<SOSalesPerTran>.CreateCopy(SalesPerTran.Current);
										SalesPerTran.Cache.SetValueExt<SOSalesPerTran.commnPct>(SalesPerTran.Current, pertran.CommnPct);
										SalesPerTran.Cache.RaiseRowUpdated(SalesPerTran.Current, salespertran_copy);
									}

									invoice_lines_distinct[artran] = true;
									discounts_aggregated[existing] = new ARTran();
									discounts_aggregated[existing].DiscAmt = artran.DiscAmt;
									discounts_aggregated[existing].ExtPrice = artran.ExtPrice;
									discounts_aggregated[existing].BaseQty = artran.BaseQty;

									//clear splits
									lsselect.RaiseRowDeleted(Transactions.Cache, existing);
								}
							}
							else
							{
								bool exists;
								if (!invoice_lines_distinct.TryGetValue(artran, out exists))
								{
									invoice_lines_distinct[artran] = true;
									discounts_aggregated[existing].DiscAmt += artran.DiscAmt;
									discounts_aggregated[existing].ExtPrice += artran.ExtPrice;
									discounts_aggregated[existing].BaseQty += artran.BaseQty;
								}
							}
							prev_line = line;
							
							SOLine copy;
							copy = PXCache<SOLine>.CreateCopy(existing);

							bool processSplits = split != null && (lsselect.IsLSEntryEnabled || lsselect.IsAllocationEntryEnabled) && (!string.IsNullOrEmpty(split.LotSerialNbr) || lsselect.IsLocationEnabled);
							if (!processSplits)
							{
								copy.BaseQty += res.BaseQty;
							}

							if (copy.BaseQty == 0m)
							{
								copy.LineAmt = artran.TranAmt;
								decimal CuryLineAmt;
								PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Transactions.Cache, copy, (decimal)copy.LineAmt, out CuryLineAmt);
								copy.CuryLineAmt = CuryLineAmt;
							}

                            PXDBQuantityAttribute.CalcTranQty<SOLine.orderQty>(Transactions.Cache, copy);

							if (discounts_aggregated[existing].BaseQty - copy.BaseQty < 0.000005m)
							{
								decimal CuryDiscAmt;
								PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Transactions.Cache, copy, (decimal)discounts_aggregated[existing].DiscAmt, out CuryDiscAmt);
								copy.DiscAmt = discounts_aggregated[existing].DiscAmt;
								copy.CuryDiscAmt = CuryDiscAmt;
							}
							else if (discounts_aggregated[existing].ExtPrice != 0)
							{
								copy.DiscPct = Math.Round((decimal)(100m * discounts_aggregated[existing].DiscAmt / discounts_aggregated[existing].ExtPrice), 6, MidpointRounding.AwayFromZero);
							}

							try
							{
								Transactions.Cache.Update(copy);
							}
							catch (PXSetPropertyException) { ; }

							if (processSplits)
							{
								SOLineSplit newsplit = new SOLineSplit();
								newsplit.SubItemID = split.SubItemID;
								if (lsselect.IsLocationEnabled)
									newsplit.LocationID = split.LocationID;
								newsplit.LotSerialNbr = split.LotSerialNbr;
								newsplit.ExpireDate = split.ExpireDate;
								newsplit.UOM = split.UOM;
								
								newsplit = splits.Insert(newsplit);
								newsplit.Qty = split.Qty;
								newsplit = splits.Update(newsplit);
								string error = PXUIFieldAttribute.GetError<SOLineSplit.qty>(splits.Cache, newsplit);

								if (!string.IsNullOrEmpty(error))
								{
									newsplit.Qty = 0;
									newsplit = splits.Update(newsplit);
								}
							}
						}
					}
				}

				if (addinvoicefilter.Current != null)
				{
					addinvoicefilter.Current.RefNbr = null;
				}
			}
			finally
			{
				this.invoicesplits.Cache.Clear();
				this.invoicesplits.View.Clear();
			}

			return adapter.Get();
		}

		public PXAction<SOOrder> addInvoiceOK;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddInvoiceOK(PXAdapter adapter)
		{
			invoicesplits.View.Answer = WebDialogResult.OK;

			return AddInvoice(adapter);
		}

		public PXAction<SOOrder> checkCopyParams;
		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable CheckCopyParams(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<SOOrder> reopenOrder;
		[PXUIField(DisplayName = "Re-open Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ReopenOrder(PXAdapter adapter)
		{
			Document.Current.Completed = false;
            Document.Current.Cancelled = false;
            if (Document.Current.Behavior == "SO")
			{
				Document.Current.Status = SOOrderStatus.BackOrder;
			}
			else
			{
				Document.Current.Status = SOOrderStatus.Open;
			}
			Document.Cache.Update(Document.Current);
			return adapter.Get();
		}

		public PXAction<SOOrder> copyOrder;
		[PXUIField(DisplayName = "Copy Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable CopyOrder(PXAdapter adapter)
		{
            List<SOOrder> list = adapter.Get<SOOrder>().ToList();
			if (copyparamfilter.AskExt(setStateFilter, true) == WebDialogResult.OK && string.IsNullOrEmpty(copyparamfilter.Current.OrderType) == false)
			{
				this.Save.Press();
				SOOrder order = PXCache<SOOrder>.CreateCopy(Document.Current);

				this.CopyOrderProc(order, copyparamfilter.Current);

				List<SOOrder> rs = new List<SOOrder> {Document.Current};
				return rs;
			}
            return list;
		}

		private void setStateFilter(PXGraph aGraph, string ViewName)
		{
			checkCopyParams.SetEnabled(!string.IsNullOrEmpty(copyparamfilter.Current.OrderType) && !string.IsNullOrEmpty(copyparamfilter.Current.OrderNbr));
		}


		public virtual void CopyOrderProc(SOOrder order, CopyParamFilter copyFilter)
		{
			string NewOrderType = copyFilter.OrderType;
			string NewOrderNbr = copyFilter.OrderNbr;
			bool RecalcUnitPrices = (bool)copyFilter.RecalcUnitPrices;
			bool OverrideManualPrices = (bool)copyFilter.OverrideManualPrices;
			bool RecalcDiscounts = (bool)copyFilter.RecalcDiscounts;
			bool OverrideManualDiscounts = (bool)copyFilter.OverrideManualDiscounts;

            SOOrderType ordertype = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>>.Select(this, order.OrderType);

            this.Clear(PXClearOption.PreserveTimeStamp);

			foreach (PXResult<SOOrder, CurrencyInfo> res in PXSelectJoin<SOOrder, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SOOrder.curyInfoID>>>, Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>, And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(this, order.OrderType, order.OrderNbr))
			{
				SOOrder quoteorder = (SOOrder)res;
				if (quoteorder.Behavior == SOOrderTypeConstants.QuoteOrder)
				{
					quoteorder.Completed = true;
					quoteorder.Status = SOOrderStatus.Completed;
					Document.Cache.SetStatus(quoteorder, PXEntryStatus.Updated);
				}

				CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
				info.CuryInfoID = null;
				info.IsReadOnly = false;
				info = this.currencyinfo.Insert(info);
				CurrencyInfo copyinfo = PXCache<CurrencyInfo>.CreateCopy(info);

				SOOrder neworder = new SOOrder();
				neworder.CuryInfoID = info.CuryInfoID;
				neworder.OrderType = NewOrderType;
				neworder.OrderNbr = NewOrderNbr;
				neworder = Document.Insert(neworder);

				//Automation
				neworder = Document.Search<SOOrder.orderNbr>(neworder.OrderNbr, neworder.OrderType);

				//Disable tax calculation for freight as well
				TaxAttribute.SetTaxCalc<SOOrder.freightTaxCategoryID>(this.Document.Cache, null, TaxCalc.ManualCalc);

				SOOrder copyorder = PXCache<SOOrder>.CreateCopy(quoteorder);
				copyorder.OwnerID = neworder.OwnerID;
				copyorder.WorkgroupID = null;
				copyorder.OrderType = neworder.OrderType;
				copyorder.OrderNbr = neworder.OrderNbr;
				copyorder.Behavior = neworder.Behavior;
				copyorder.ARDocType = neworder.ARDocType;
				copyorder.DefaultOperation = neworder.DefaultOperation;
				copyorder.ShipAddressID = neworder.ShipAddressID;
				copyorder.ShipContactID = neworder.ShipContactID;
				copyorder.BillAddressID = neworder.BillAddressID;
				copyorder.BillContactID = neworder.BillContactID;
				copyorder.OrigOrderType = quoteorder.OrderType;
				copyorder.OrigOrderNbr = quoteorder.OrderNbr;
				copyorder.ShipmentCntr = 0;
				copyorder.OpenShipmentCntr = 0;
				copyorder.OpenLineCntr = 0;
				copyorder.ReleasedCntr = 0;
				copyorder.BilledCntr = 0;
				copyorder.OrderQty = 0m;
				copyorder.OrderWeight = 0m;
				copyorder.OrderVolume = 0m;
				copyorder.OpenOrderQty = 0m;
				copyorder.UnbilledOrderQty = 0m;
				copyorder.CuryInfoID = neworder.CuryInfoID;
				copyorder.Status = neworder.Status;
				copyorder.Hold = neworder.Hold;
				copyorder.Completed = neworder.Completed;
				copyorder.Cancelled = neworder.Cancelled;
				copyorder.InclCustOpenOrders = neworder.InclCustOpenOrders;
				copyorder.OrderDate = neworder.OrderDate;
				copyorder.RequestDate = neworder.RequestDate;
				copyorder.ShipDate = neworder.ShipDate;
				copyorder.CuryMiscTot = 0m;
				copyorder.CuryUnbilledMiscTot = 0m;
				copyorder.CuryLineTotal = 0m;
				copyorder.CuryOpenLineTotal = 0m;
				copyorder.CuryUnbilledLineTotal = 0m;
				copyorder.CuryVatExemptTotal = 0m;
				copyorder.CuryVatTaxableTotal = 0m;
                copyorder.CuryTaxTotal = 0m;
                copyorder.CuryOrderTotal = 0m;
				copyorder.CuryOpenOrderTotal = 0m;
				copyorder.CuryOpenTaxTotal = 0m;
				copyorder.CuryUnbilledOrderTotal = 0m;
				copyorder.CuryUnbilledTaxTotal = 0m;

				if (quoteorder.Behavior == SOOrderTypeConstants.QuoteOrder)
				{
					copyorder.BillSeparately = neworder.BillSeparately;
					copyorder.ShipSeparately = neworder.ShipSeparately;
				}
				Document.Cache.SetDefaultExt<SOOrder.invoiceDate>(copyorder);
				Document.Cache.SetDefaultExt<SOOrder.finPeriodID>(copyorder);
				copyorder.NoteID = null;
				copyorder = Document.Update(copyorder);
				PXNoteAttribute.CopyNoteAndFiles(Document.Cache, quoteorder, Document.Cache, copyorder, ordertype);

				if (info != null)
				{
					info.CuryID = copyinfo.CuryID;
					info.CuryEffDate = copyinfo.CuryEffDate;
					info.CuryRateTypeID = copyinfo.CuryRateTypeID;
					info.CuryRate = copyinfo.CuryRate;
					info.RecipRate = copyinfo.RecipRate;
					info.CuryMultDiv = copyinfo.CuryMultDiv;
					this.currencyinfo.Update(info);
				}
			}
			AddressAttribute.CopyRecord<SOOrder.billAddressID>(Document.Cache, Document.Current, order, false);
			ContactAttribute.CopyRecord<SOOrder.billContactID>(Document.Cache, Document.Current, order, false);
			AddressAttribute.CopyRecord<SOOrder.shipAddressID>(Document.Cache, Document.Current, order, false);
			ContactAttribute.CopyRecord<SOOrder.shipContactID>(Document.Cache, Document.Current, order, false);

			bool isex = false;

			TaxAttribute.SetTaxCalc<SOLine.taxCategoryID>(this.Transactions.Cache, null, TaxCalc.ManualCalc);

            if (this.customer.Current != null && this.customer.Current.CreditRule == null)
            {
                Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<SOOrder.customerID>>>>.Select(this);
                if (customer != null)
                    this.customer.Current.CreditRule = customer.CreditRule;
            }

		    string[] notSupportingPOtoSOLinkBehaviors = {SOBehavior.QT, SOBehavior.CM, SOBehavior.IN};

			foreach (PXResult<SOLine> res in PXSelectReadonly<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>>.Select(this, order.OrderType, order.OrderNbr))
			{
				SOLine line = PXCache<SOLine>.CreateCopy((SOLine)res);
				line.OrigOrderType = line.OrderType;
				line.OrigOrderNbr = line.OrderNbr;
				line.OrigLineNbr = line.LineNbr;
				line.OrderType = null;
				line.OrderNbr = null;
				line.InvtMult = null;
				line.CuryInfoID = null;
				line.PlanType = null;
				line.TranType = null;
				line.RequireShipping = null;
				line.RequireAllocation = null;
				line.RequireLocation = null;
				line.RequireReasonCode = null;
				line.OpenLine = null;
				line.Completed = false;
				line.CancelDate = null;
				//line.POType = null;
				//line.PONbr = null;
				//line.POLineNbr = null;
				line.RequestDate = null;
				line.ShipDate = null;
                line.OrderDate = null;

                if (notSupportingPOtoSOLinkBehaviors.Contains(order.Behavior) || notSupportingPOtoSOLinkBehaviors.Contains(Document.Current.Behavior))
                {
				line.POCreate = null;
				line.POSource = null;
			    }

				if (soordertype.Current.RequireLocation == true && line.ShipComplete == SOShipComplete.BackOrderAllowed)
				{
					line.ShipComplete = null;
				}

				if (soordertype.Current.RequireLocation == false)
				{
					line.LocationID = null;
					line.LotSerialNbr = null;
					line.ExpireDate = null;
				}

				if (!IsCreditMemoOrder && !IsRMAOrder)
				{
					line.InvoiceNbr = null;
				}

				if (PXSelectReadonly<SOOrderTypeOperation,
						Where<SOOrderTypeOperation.orderType, Equal<Current<SOOrder.orderType>>,
						And<SOOrderTypeOperation.operation, Equal<Required<SOLine.operation>>,
						And<SOOrderTypeOperation.active, Equal<True>>>>>.Select(this, line.Operation).Count == 0)
				{
					line.Operation = null;
				}

				if (line.IsFree == true && line.ManualDisc == false && RecalcDiscounts)
				{
					continue;
				}

				if (OverrideManualDiscounts)
				{
					line.ManualDisc = false;
					line.ManualPrice = false;
				}

				if (RecalcUnitPrices && line.ManualPrice != true)
				{
					line.CuryUnitPrice = null;
					line.CuryExtPrice = null;
				}

                if (OverrideManualPrices)
                {
                    line.ManualPrice = false;
                }

				if (!RecalcDiscounts)
				{
					line.ManualDisc = true;
				}

				line.UnassignedQty = 0m;
				line.OpenQty = null;
				line.ClosedQty = 0m;
				line.BilledQty = 0m;
				line.UnbilledQty = null;
				line.ShippedQty = 0m;
				line.CuryBilledAmt = 0m;
				line.CuryUnbilledAmt = null;
				line.CuryOpenAmt = null;

				line.NoteID = null;

				try
				{
					FieldUpdated.RemoveHandler<SOLine.discountID>(SOLine_DiscountID_FieldUpdated);
					try
					{
						IDictionary values = Transactions.Cache.ToDictionary(line);

						if (Transactions.Cache.Insert(values) > 0)
						{
							line = Transactions.Current;
						}
						PXNoteAttribute.CopyNoteAndFiles(Transactions.Cache, (SOLine)res, Transactions.Cache, line, ordertype);
					}
					catch (PXSetPropertyException)
					{
						isex = true;
					}
				}
				finally
				{
					this.FieldUpdated.AddHandler<SOLine.discountID>(SOLine_DiscountID_FieldUpdated);
				}
			}

			if (isex)
			{
				TaxAttribute.SetTaxCalc<SOLine.taxCategoryID>(this.Transactions.Cache, null, TaxCalc.Calc);
			}

			foreach (PXResult<SOTaxTran> res in PXSelect<SOTaxTran, Where<SOTaxTran.orderType, Equal<Required<SOTaxTran.orderType>>, And<SOTaxTran.orderNbr, Equal<Required<SOTaxTran.orderNbr>>>>>.Select(this, order.OrderType, order.OrderNbr))
			{

                SOTaxTran tax = (SOTaxTran)res;
                SOTaxTran newtax = new SOTaxTran();
                newtax.OrderType = Document.Current.OrderType;
                newtax.OrderNbr = Document.Current.OrderNbr;
                newtax.LineNbr = int.MaxValue;
                newtax.TaxID = tax.TaxID;

                newtax = this.Taxes.Insert(newtax);

                if (!isex && newtax != null)
                {
                    newtax = PXCache<SOTaxTran>.CreateCopy(newtax);
                    newtax.TaxRate = tax.TaxRate;
                    newtax.CuryTaxableAmt = tax.CuryTaxableAmt;
                    newtax.CuryTaxAmt = tax.CuryTaxAmt;

					newtax.CuryUnshippedTaxableAmt = tax.CuryTaxableAmt;
					newtax.CuryUnshippedTaxAmt = tax.CuryTaxAmt;
					newtax.CuryUnbilledTaxableAmt = tax.CuryTaxableAmt;
					newtax.CuryUnbilledTaxAmt = tax.CuryTaxAmt;

                    newtax = this.Taxes.Update(newtax);
                }
			}

			if (!RecalcDiscounts)
			{
				//copy all discounts except free-items:
				foreach (SOOrderDiscountDetail soOrderDisc in PXSelect<SOOrderDiscountDetail, Where<SOOrderDiscountDetail.orderType, Equal<Required<SOOrderDiscountDetail.orderType>>, And<SOOrderDiscountDetail.orderNbr, Equal<Required<SOOrderDiscountDetail.orderNbr>>, And<SOOrderDiscountDetail.freeItemID, IsNull>>>>.Select(this, order.OrderType, order.OrderNbr))
				{
					SOOrderDiscountDetail disc = PXCache<SOOrderDiscountDetail>.CreateCopy((SOOrderDiscountDetail)soOrderDisc);
					disc.OrderType = Document.Current.OrderType;
					disc.OrderNbr = Document.Current.OrderNbr;
					disc.IsManual = true;
					DiscountDetails.Insert(disc);
				}
			}

			RecalcDiscountsParamFilter filter = recalcdiscountsfilter.Current;
			filter.OverrideManualDiscounts = OverrideManualDiscounts;
			filter.OverrideManualPrices = OverrideManualPrices;
			filter.RecalcDiscounts = RecalcDiscounts;
			filter.RecalcUnitPrices = RecalcUnitPrices;
			filter.RecalcTarget = RecalcDiscountsParamFilter.AllLines;
			DiscountEngine<SOLine>.RecalculatePricesAndDiscounts<SOOrderDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, DiscountDetails, Document.Current.CustomerLocationID, Document.Current.OrderDate, filter);

			RecalculateTotalDiscount();

			RefreshFreeItemLines(Transactions.Cache);
		}

		public PXAction<SOOrder> inventorySummary;
		[PXUIField(DisplayName = "Inventory Summary", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable InventorySummary(PXAdapter adapter)
		{
			PXCache tCache = Transactions.Cache;
			SOLine line = Transactions.Current;
			if (line == null) return adapter.Get();

			InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<SOLine.inventoryID>(tCache, line);
			if (item != null && item.StkItem == true)
			{
				INSubItem sbitem = (INSubItem)PXSelectorAttribute.Select<SOLine.subItemID>(tCache, line);
				InventorySummaryEnq.Redirect(item.InventoryID,
											 ((sbitem != null) ? sbitem.SubItemCD : null),
											 line.SiteID,
											 line.LocationID);
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> calculateFreight;
		[PXUIField(DisplayName = Messages.RefreshFreight, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CalculateFreight(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.IsManualPackage != true && Document.Current.IsPackageValid != true)
			{
				RecalculatePackagesForOrder(Document.Current);
			}

			CalculateFreightCost(false);

			return adapter.Get();
		}

		public PXAction<SOOrder> shopRates;
		[PXUIField(DisplayName = "Shop for Rates", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable ShopRates(PXAdapter adapter)
		{
			if (DocumentProperties.Current != null)
			{
				WebDialogResult res = DocumentProperties.AskExt();
				
				if (res == WebDialogResult.OK)
				{
					foreach (SOCarrierRate cr in CarrierRates.Cache.Cached)
					{
						if (cr.Selected == true)
						{
							DocumentProperties.SetValueExt<SOOrder.shipVia>(DocumentProperties.Current, cr.Method);

							if (CollectFreight)
							{
								decimal baseCost = ConvertAmtToBaseCury(Document.Current.CuryID, arsetup.Current.DefaultRateTypeID, Document.Current.OrderDate.Value, cr.Amount.Value);
								SetFreightCost(baseCost);
							}
						}
					}
				}

				CarrierRates.Cache.Clear();
			}


			
			return adapter.Get();
		}

		public PXAction<SOOrder> refreshRates;
		[PXUIField(DisplayName = "Refresh Rates", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable RefreshRates(PXAdapter adapter)
		{
			if (Document.Current != null)
			{
				CarrierRates.Cache.Clear();

				if (Document.Current.IsManualPackage == true)
				{
					PXResultset<SOPackageInfoEx> resultset = Packages.Select();

					if (resultset.Count == 0)
						throw new PXException(Messages.AtleastOnePackageIsRequired);
					else
					{
						bool failed = false;
						foreach (SOPackageInfoEx p in resultset)
						{
							if (p.SiteID == null)
							{
								Packages.Cache.RaiseExceptionHandling<SOPackageInfoEx.siteID>(p, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error, typeof(SOPackageInfoEx.siteID).Name));
								failed = true;
							}
						}
						if (failed)
							throw new PXException(Messages.AtleastOnePackageIsInvalid);
					}
				}

				int cx = 0;
				StringBuilder errorMessages = new StringBuilder();
				bool autoPackWarning = false;

				List<CarrierRequestInfo> requests = new List<CarrierRequestInfo>();
				foreach (CarrierPlugin plugin in PXSelect<CarrierPlugin>.Select(this))
				{
					ICarrierService cs = CarrierPluginMaint.CreateCarrierService(this, plugin);
					CarrierRequest cRequest = BuildQuoteRequest(Document.Current, plugin);
					if (cRequest.PackagesEx.Count == 0)
					{
						PXTrace.WriteWarning(Messages.AutoPackagingZeroPackWarning, plugin.CarrierPluginID);
						autoPackWarning = true;
						continue;
					}

					CarrierRequestInfo info = new CarrierRequestInfo();
					info.Plugin = plugin;
					info.Service = cs;
					info.Request = cRequest;

					requests.Add(info);
				}
				
				Parallel.ForEach(requests, info =>
				{
					info.Result = info.Service.GetRateList(info.Request);
				});
				
				foreach (CarrierRequestInfo info in requests)
				{
					CarrierResult<IList<RateQuote>> result = info.Result;

					if (result.IsSuccess)
					{
						foreach (RateQuote item in result.Result)
						{
							if (item.IsSuccess && item.Currency != Document.Current.CuryID)
							{
								if (string.IsNullOrEmpty(arsetup.Current.DefaultRateTypeID))
								{
									throw new PXException(Messages.RateTypeNotSpecified);
								}
							}
							
							PXSelectBase<Carrier> selectCarrier = new PXSelect<Carrier, 
								Where<Carrier.carrierPluginID, Equal<Required<Carrier.carrierPluginID>>, 
								And<Carrier.pluginMethod, Equal<Required<Carrier.pluginMethod>>,
								And<Carrier.isExternal, Equal<True>>>>>(this);

							foreach (Carrier shipVia in selectCarrier.Select(info.Plugin.CarrierPluginID, item.Method.Code))
							{
								SOCarrierRate r = new SOCarrierRate();
								r.LineNbr = cx++;
								r.Method = shipVia.CarrierID;
								r.Description = item.Method.Description;
								r.Amount = ConvertAmt(item.Currency, Document.Current.CuryID, arsetup.Current.DefaultRateTypeID, Document.Current.OrderDate.Value, item.Amount);
								r.DeliveryDate = item.DeliveryDate;
								if (item.DaysInTransit > 0)
									r.DaysInTransit = item.DaysInTransit;

								r.Selected = r.Method == Document.Current.ShipVia;
								r = CarrierRates.Insert(r);

								if (!item.IsSuccess)
								{
									CarrierRates.Cache.RaiseExceptionHandling(typeof(SOCarrierRate.method).Name, r, null, new PXSetPropertyException("{0}: {1}", PXErrorLevel.RowError, item.Messages[0].Code, item.Messages[0].Description));
								}
							}

						}

					}
					else
					{
						foreach (PX.Data.Message message in result.Messages)
						{
							errorMessages.AppendFormat("{0} Returned error:{1}", info.Plugin.CarrierPluginID, message.ToString());
						}

						if (!string.IsNullOrEmpty(result.RequestData))
						{
							PXTrace.WriteInformation(result.RequestData);
						}
					}
				}

				
				if (errorMessages.Length > 0)
				{
					throw new PXException(Messages.CarrierServiceError, errorMessages.ToString());
				}

				if (autoPackWarning)
				{
					throw new PXException(Messages.AutoPackagingIssuesCheckTrace);
				}

			}

			return adapter.Get();
		}

		public PXAction<SOOrder> recalculatePackages;
		[PXUIField(DisplayName = "Refresh Packages", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable RecalculatePackages(PXAdapter adapter)
		{
			if (Document.Current != null)
			{
				RecalculatePackagesForOrder(Document.Current);
			}

			return adapter.Get();
		}

		public PXAction<SOOrder> createPayment;
		[PXUIField(DisplayName = "Create Payment", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
		protected virtual void CreatePayment()
		{
			if (Document.Current != null)
			{
				this.Save.Press();

				PXGraph target;
				CreatePaymentProc(Document.Current, out target);

				throw new PXPopupRedirectException(target, "New Payment", true);
			}
		}

        public PXAction<SOOrder> createPrepayment;
        [PXUIField(DisplayName = "Create Prepayment", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
        protected virtual void CreatePrepayment()
        {
            if (Document.Current != null)
            {
                this.Save.Press();

                PXGraph target;
                CreatePaymentProc(Document.Current, out target, ARPaymentType.Prepayment);

                throw new PXPopupRedirectException(target, "New Payment", true);
            }
        }


        public virtual void CreatePaymentProc(SOOrder order, out PXGraph target, string paymentType = ARPaymentType.Payment)
		{
			ARPaymentEntry docgraph = PXGraph.CreateInstance<ARPaymentEntry>();
			target = docgraph;

			docgraph.Clear();
			ARPayment payment = new ARPayment()
			{
                DocType = paymentType,
			};

			payment = PXCache<ARPayment>.CreateCopy(docgraph.Document.Insert(payment));

			payment.CustomerID = order.CustomerID;
			payment.CustomerLocationID = order.CustomerLocationID;
			payment.PaymentMethodID = order.PaymentMethodID;
			payment.PMInstanceID = order.PMInstanceID;
			payment.CuryOrigDocAmt = order.IsCCCaptured == true ? order.CuryCCCapturedAmt : order.IsCCAuthorized == true ? order.CuryCCPreAuthAmount : 0m;
			payment.ExtRefNbr = order.ExtRefNbr;
			payment.DocDesc = order.OrderDesc;
			payment = docgraph.Document.Update(payment);

			if (String.IsNullOrEmpty(order.ExtRefNbr) && docgraph.paymentmethod.Current != null &&
				docgraph.paymentmethod.Current.PaymentType == PaymentMethodType.CreditCard && docgraph.paymentmethod.Current.ARIsProcessingRequired == true)
			{

				PaymentMethodAccount det = PXSelectReadonly<PaymentMethodAccount, 
					Where<PaymentMethodAccount.cashAccountID, Equal<Required<PaymentMethodAccount.cashAccountID>>,
					And<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>,
					And<PaymentMethodAccount.useForAR, Equal<True>>>>>.SelectSingleBound(docgraph, null, payment.CashAccountID, payment.PaymentMethodID);

				if (det == null || det.ARAutoNextNbr != true)
				{
					payment = PXCache<ARPayment>.CreateCopy(payment);
					payment.ExtRefNbr = PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.ARAutoPaymentRefNbrFormat, payment.DocDate);

					payment = docgraph.Document.Update(payment);
				}
			}
			InsertSOAdjustments(order, docgraph, payment);

			if (payment.CuryOrigDocAmt == 0m)
			{
				payment.CuryOrigDocAmt = payment.CurySOApplAmt;
				payment = docgraph.Document.Update(payment);
			}

			if (order.IsCCCaptured == true || order.IsCCAuthorized == true)
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					docgraph.Save.Press();

					PXDatabase.Update<CCProcTran>(
						new PXDataFieldAssign("DocType", docgraph.Document.Current.DocType),
						new PXDataFieldAssign("RefNbr", docgraph.Document.Current.RefNbr),
						new PXDataFieldRestrict("OrigDocType", PXDbType.Char, 3, order.OrderType, PXComp.EQ),
						new PXDataFieldRestrict("OrigRefNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ),
						new PXDataFieldRestrict("RefNbr", PXDbType.NVarChar, 15, null, PXComp.ISNULL)
						);

					if (order.IsCCCaptured == false && order.IsCCAuthorized == true)
					{
						PXDatabase.Update<SOOrder>(
							new PXDataFieldAssign("CuryCCPreAuthAmount", 0m),
							new PXDataFieldAssign("IsCCAuthorized", false),
							new PXDataFieldRestrict("OrderType", PXDbType.Char, 3, order.OrderType, PXComp.EQ),
							new PXDataFieldRestrict("OrderNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ)
							);
					}
					ts.Complete();
				}

				if (payment.Released == false && order.IsCCCaptured == true && arsetup.Current.IntegratedCCProcessing == true)
				{
					CCPaymentEntry.ReleaseARDocument(payment);
				}
			}
		}

		protected virtual void InsertSOAdjustments(SOOrder order, ARPaymentEntry docgraph, ARPayment payment)
		{
			SOAdjust adj = new SOAdjust()
			{
				AdjdOrderType = order.OrderType,
				AdjdOrderNbr = order.OrderNbr
			};

			try
			{
				docgraph.SOAdjustments.Insert(adj);
			}
			catch (PXSetPropertyException)
			{
				if (order.IsCCCaptured == true || order.IsCCAuthorized == true)
				{
					throw;
				}
				payment.CuryOrigDocAmt = 0m;
			}
		}

		public PXAction<SOOrder> viewPayment;
		[PXUIField(DisplayName = "View Payment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual void ViewPayment()
		{
			if (Document.Current != null && Adjustments.Current != null)
			{
				ARPaymentEntry pe = PXGraph.CreateInstance<ARPaymentEntry>();
				pe.Document.Current = pe.Document.Search<ARPayment.refNbr>(Adjustments.Current.AdjgRefNbr, Adjustments.Current.AdjgDocType);

				throw new PXRedirectRequiredException(pe, true, "Payment"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
			}
		}

		public class SOOrderMessageDisplay : IPXCustomInfo
		{
			public void Complete(PXLongRunStatus status, PXGraph graph)
			{
				if (status == PXLongRunStatus.Completed && graph is SOOrderEntry)
				{
					((SOOrderEntry)graph).RowSelected.AddHandler<SOOrder>((sender, e) =>
					{
						SOOrder order = e.Row as SOOrder;
						if (order != null)
						{
							sender.RaiseExceptionHandling<SOOrder.cCPaymentStateDescr>(order, order.CCPaymentStateDescr, _message);
						}
					});
				}
			}

			private PXSetPropertyException _message;

			public SOOrderMessageDisplay(PXSetPropertyException message)
			{
				_message = message;
			}
		}

        public virtual void UpdateDocState(SOOrder olddoc, CCTranType lastOperation)
		{
			this.Document.Current = Document.Search<SOOrder.orderNbr>(olddoc.OrderNbr, olddoc.OrderType);
            SOOrder doc = (SOOrder)this.Document.Cache.CreateCopy(this.Document.Current);
            
			CCPaymentEntry.CCTransState ccState = CCPaymentEntry.UpdateCCPaymentState<SOOrder>(doc, this.ccProcTran.Select());
			bool needsPersist = false;
			if (ccState.NeedUpdate)
			{
				doc.PreAuthTranNumber = null;
				doc = this.Document.Update(doc);
				Document.Search<SOOrder.orderNbr>(doc.OrderNbr, doc.OrderType);
				needsPersist = true;
			}
			try
			{
				if (soordertype.Current.CanHaveApplications)
				{
					//Can't be voided and captured at the same time
					if (((ccState.CCState & CCPaymentState.Voided) != 0 || (ccState.CCState & CCPaymentState.Refunded) != 0) 
						&& arsetup.Current.IntegratedCCProcessing == true)
					{
						PXSetPropertyException message = null;
						ARPaymentEntry docgraph = PXGraph.CreateInstance<ARPaymentEntry>();
						ARPayment payment = PXSelect<ARPayment, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
							And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(docgraph, ccState.LastTran.DocType, ccState.LastTran.RefNbr);
						if (payment != null && payment.Released == true && payment.Voided == false)
						{
							docgraph.VoidCheckProcExt(payment);
							docgraph.Save.Press();
							CCPaymentEntry.ReleaseARDocument(docgraph.Document.Current);
							needsPersist = true;
						}
						else if (payment == null)
						{
							message = new PXSetPropertyException(Messages.CouldNotFindCCTranPayment, PXErrorLevel.Warning);
						}
						else 
						{
							message = new PXSetPropertyException(Messages.CouldNotVoidCCTranPayment, PXErrorLevel.Warning, ccState.LastTran.RefNbr);
						}

						if (message != null)
						{
							PXLongOperation.SetCustomInfo(new SOOrderMessageDisplay(message));
						}
					}

					if ((ccState.CCState & CCPaymentState.Captured) != 0)
					{
						PXGraph target;
						CreatePaymentProc(Document.Current, out target);
						doc.IsCCCaptured = false;
						doc.IsCCAuthorized = false;
						doc.CuryCCCapturedAmt = 0m;
						needsPersist = true;
					}
				}
			}
			finally
			{
				if (needsPersist)
				{
					this.Save.Press();
				}
			}
		}

		public PXAction<SOOrder> authorizeCCPayment;
		[PXUIField(DisplayName = "Authorize CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable AuthorizeCCPayment(PXAdapter adapter)
		{
			foreach (SOOrder doc in adapter.Get<SOOrder>())
			{
				CCPaymentEntry.AuthorizeCCPayment<SOOrder>(doc, this.ccProcTran, UpdateSOOrderState);
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> voidCCPayment;
		[PXUIField(DisplayName = "Void CC Auth. / Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable VoidCCPayment(PXAdapter adapter)
		{
			foreach (SOOrder doc in adapter.Get<SOOrder>())
			{
				CCPaymentEntry.VoidCCPayment<SOOrder>(doc, ccProcTran, null, UpdateSOOrderState);
			}

			return adapter.Get();
		}

		public PXAction<SOOrder> captureCCPayment;
		[PXUIField(DisplayName = "Capture CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CaptureCCPayment(PXAdapter adapter)
		{
			foreach (SOOrder doc in adapter.Get<SOOrder>())
			{
				CCPaymentEntry.CaptureCCPayment<SOOrder>(doc, ccProcTran, null, UpdateSOOrderState);
			}

			return adapter.Get();
		}

		public PXAction<SOOrder> creditCCPayment;
		[PXUIField(DisplayName = "Refund CC Payment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CreditCCPayment(PXAdapter adapter)
		{
			foreach (SOOrder doc in adapter.Get<SOOrder>())
			{
				string PCRefTranNbr = doc.RefTranExtNbr;
				if (String.IsNullOrEmpty(doc.RefTranExtNbr))
				{
					this.Document.Cache.RaiseExceptionHandling<SOOrder.refTranExtNbr>(this.Document.Current, doc.RefTranExtNbr, new PXSetPropertyException(AR.Messages.ERR_PCTransactionNumberOfTheOriginalPaymentIsRequired));
				}
				else
				{
					CCPaymentEntry.CreditCCPayment<SOOrder>(doc, PCRefTranNbr, ccProcTran, null, UpdateSOOrderState);
				}
			}
			return adapter.Get();
		}
		
		public PXAction<SOOrder> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton(/*ImageKey = PX.Web.UI.Sprite.Main.Process*/)]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (SOOrder current in adapter.Get<SOOrder>())
			{
				if (current != null)
				{
					bool needSave = false;
					Save.Press();
					SOBillingAddress address = this.Billing_Address.Select();
					if (address != null && address.IsDefaultAddress == false && address.IsValidated == false)
					{
						if (PXAddressValidator.Validate<SOBillingAddress>(this, address, true))
							needSave = true;
					}

					SOShippingAddress shipAddress = this.Shipping_Address.Select();
					if (shipAddress != null && shipAddress.IsDefaultAddress == false && shipAddress.IsValidated == false)
					{
						if (PXAddressValidator.Validate<SOShippingAddress>(this, shipAddress, true))
							needSave = true;
					}
					if (needSave == true)
						this.Save.Press();
				}
				yield return current;
			}
		}

		public PXAction<SOOrder> recalculateDiscountsAction;
		[PXUIField(DisplayName = "Recalculate Prices", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
		{
			if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
			{
				DiscountEngine<SOLine>.RecalculatePricesAndDiscounts<SOOrderDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, DiscountDetails, Document.Current.CustomerLocationID, Document.Current.OrderDate, recalcdiscountsfilter.Current);
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> recalcOk;
		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable RecalcOk(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<SOOrder> createCCPaymentMethodHF;
		[PXUIField(DisplayName = "Create New", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable CreateCCPaymentMethodHF(PXAdapter adapter)
		{
			CustomerPaymentMethodC currentPaymentMethod = DefPaymentMethodInstance.Current;
			if (currentPaymentMethod.CCProcessingCenterID == null)
			{
				DefPaymentMethodInstance.Cache.SetDefaultExt<CustomerPaymentMethodC.cCProcessingCenterID>(currentPaymentMethod);
				DefPaymentMethodInstance.Cache.SetDefaultExt<CustomerPaymentMethodC.customerCCPID>(currentPaymentMethod);
			}
			CCCustomerInformationManager.CreatePaymentMethodHF(this, DefPaymentMethodInstance, currentPaymentMethod);
			return adapter.Get();
		}

		public PXAction<SOOrder> syncCCPaymentMethods;
		[PXUIField(DisplayName = "Sync", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable SyncCCPaymentMethods(PXAdapter adapter)
		{
			List<SOOrder> orderList = new List<SOOrder>();
			foreach (SOOrder order in adapter.Get<SOOrder>())
			{
				orderList.Add(order);
			}
			CustomerPaymentMethodC currentPaymentMethod = DefPaymentMethodInstance.Current;
			if (currentPaymentMethod.CCProcessingCenterID == null)
			{
				DefPaymentMethodInstance.Cache.SetDefaultExt<CustomerPaymentMethodC.cCProcessingCenterID>(currentPaymentMethod);
				DefPaymentMethodInstance.Cache.SetDefaultExt<CustomerPaymentMethodC.customerCCPID>(currentPaymentMethod);
			}
			CCCustomerInformationManager.SyncPaymentMethodsHF(this, DefPaymentMethodInstance, DefPaymentMethodInstanceDetailsAll, currentPaymentMethod);
			this.Save.Press();
			return orderList;
		}

		public PXAction<SOOrder> recalcAvalara;
		[PXUIField(DisplayName = "Recalculate Avalara Tax", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
		[PXButton()]
		protected virtual IEnumerable RecalcAvalara(PXAdapter adapter)
		{
			if (Document.Current != null && IsExternalTax == true)
			{
				CalculateAvalaraTax(Document.Current, true);
				this.Clear(PXClearOption.ClearAll);
			}

			return adapter.Get();
		}
		
		#endregion

		#region SiteStatus Lookup
		public PXFilter<SOSiteStatusFilter> sitestatusfilter;
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public SOSiteStatusLookup<SOSiteStatusSelected, SOSiteStatusFilter> sitestatus;

		public PXAction<SOOrder> addInvBySite;
		[PXUIField(DisplayName = "Add Stock Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
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

		public PXAction<SOOrder> addInvSelBySite;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddInvSelBySite(PXAdapter adapter)
		{
			foreach (SOSiteStatusSelected line in sitestatus.Cache.Cached)
			{
				if (line.Selected == true && line.QtySelected > 0)
				{
					SOLine newline = PXCache<SOLine>.CreateCopy(Transactions.Insert(new SOLine()));
					newline.SiteID = line.SiteID;
					newline.InventoryID = line.InventoryID;
					newline.SubItemID = line.SubItemID;
					newline.UOM = line.SalesUnit;
					newline.AlternateID = line.AlternateID;
					newline = PXCache<SOLine>.CreateCopy(Transactions.Update(newline));
					if(newline.RequireLocation != true || PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
						newline.LocationID = null;
					newline = PXCache<SOLine>.CreateCopy(Transactions.Update(newline));
					newline.Qty = line.QtySelected;
					cnt = 0;
					Transactions.Update(newline);
				}
			}
			sitestatus.Cache.Clear();
			return adapter.Get();
		}
		protected virtual void SOSiteStatusFilter_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			SOSiteStatusFilter row = (SOSiteStatusFilter)e.Row;
			if (row != null && Document.Current != null)
				row.SiteID = Document.Current.DefaultSiteID;
		}
		int cnt;
		public override IEnumerable<PXDataRecord> ProviderSelect(BqlCommand command, int topCount, params PXDataValue[] pars)
		{
			cnt++;
			return base.ProviderSelect(command, topCount, pars);
		}
		#endregion

		#region CurrencyInfo events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.Row != null && HasDetailRecords())
				{
					e.NewValue = ((CurrencyInfo)e.Row).CuryID ?? customer?.Current.CuryID;
					e.Cancel = true;
				}
				else
				{
					if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryID))
					{
						e.NewValue = customer.Current.CuryID;
						e.Cancel = true;
					}
				}

				
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.Row != null && HasDetailRecords())
				{
					e.NewValue = ((CurrencyInfo)e.Row).CuryRateTypeID;
					e.Cancel = true;
				}
				else
				{
					if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryRateTypeID))
					{
						e.NewValue = customer.Current.CuryRateTypeID;
						e.Cancel = true;
					}
				}
				
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((SOOrder)Document.Cache.Current).OrderDate;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info != null)
			{
				bool curyenabled = info.AllowUpdate(this.Transactions.Cache);
				
				if (customer.Current != null && !(bool)customer.Current.AllowOverrideRate)
				{
					curyenabled = false;
				}

				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyenabled);
			}
		}
		#endregion

		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<SOOrder.orderDate, SOOrder.curyID>(e.Row, e.OldRow))
			{
				foreach (SOLine tran in Transactions.Select())
				{
					if (Transactions.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Transactions.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}
			}
		}

		public SOOrderEntry()
		{
			RowUpdated.AddHandler<SOOrder>(ParentFieldUpdated);

            PXUIFieldAttribute.SetVisible<SOOrderShipment.operation>(shipmentlist.Cache, null, false);
            PXUIFieldAttribute.SetVisible<SOOrderShipment.orderType>(shipmentlist.Cache, null, false);
			PXUIFieldAttribute.SetVisible<SOOrderShipment.orderNbr>(shipmentlist.Cache, null, false);
			PXUIFieldAttribute.SetVisible<SOOrderShipment.shipmentNbr>(shipmentlist.Cache, null, true);

			{
				SOSetup record = sosetup.Current;
			}

			PXUIFieldAttribute.SetVisible<SOLine.promoDiscID>(Transactions.Cache, null, (sosetup.Current.PromoLineDisc == true));
			PXUIFieldAttribute.SetVisibility<SOLine.promoDiscID>(Transactions.Cache, null, (sosetup.Current.PromoLineDisc == true) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);

			PXFieldState state = (PXFieldState)this.Transactions.Cache.GetStateExt<SOLine.inventoryID>(null);
			viewInventoryID = state != null ? state.ViewName : null;

			PXUIFieldAttribute.SetVisible<SOLine.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.SO));

			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.CustomerType; });
			PXUIFieldAttribute.SetEnabled(invoicesplits.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<InvoiceSplits.selected>(invoicesplits.Cache, null, true);

			if (!PXAccess.FeatureInstalled<FeaturesSet.carrierIntegration>())
			{
				shopRates.SetCaption(PXMessages.LocalizeNoPrefix(Messages.Packages));
			}
		}

		#region SOAdjust Events
		protected virtual void SOAdjust_AdjgRefNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>>.Select(this);
			SOAdjust adj = (SOAdjust)e.Row;

			PXSelectBase<ARPayment> s = new PXSelectReadonly2<ARPayment,
				InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>,
				Where<ARPayment.customerID, Equal<Current<SOOrder.customerID>>,
				And2<Where<ARPayment.docType, Equal<ARDocType.payment>, Or<ARPayment.docType, Equal<ARDocType.prepayment>, Or<ARPayment.docType, Equal<ARDocType.creditMemo>>>>,
				And<ARPayment.openDoc, Equal<boolTrue>,
				And<ARPayment.docType, Equal<Required<ARPayment.docType>>,
				And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>>>>(this);
					
			foreach (PXResult<ARPayment, CurrencyInfo> res in s.Select(adj.AdjgDocType, adj.AdjgRefNbr))
			{
				ARPayment payment = PXCache<ARPayment>.CreateCopy(res);

				CurrencyInfo pay_info = (CurrencyInfo)res;

				adj.CustomerID = Document.Current.CustomerID;
				adj.AdjdOrderType = Document.Current.OrderType;
				adj.AdjdOrderNbr = Document.Current.OrderNbr;
				adj.AdjgDocType = payment.DocType;
				adj.AdjgRefNbr = payment.RefNbr;

				SOAdjust other = PXSelectGroupBy<SOAdjust, Where<SOAdjust.adjgDocType, Equal<Required<SOAdjust.adjgDocType>>, And<SOAdjust.adjgRefNbr, Equal<Required<SOAdjust.adjgRefNbr>>, And<Where<SOAdjust.adjdOrderType, NotEqual<Required<SOAdjust.adjdOrderType>>, Or<SOAdjust.adjdOrderNbr, NotEqual<Required<SOAdjust.adjdOrderNbr>>>>>>>, Aggregate<GroupBy<SOAdjust.adjgDocType, GroupBy<SOAdjust.adjgRefNbr, Sum<SOAdjust.curyAdjgAmt, Sum<SOAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdOrderType, adj.AdjdOrderNbr);
				if (other != null && other.AdjdOrderNbr != null)
				{
					payment.CuryDocBal -= other.CuryAdjgAmt;
					payment.DocBal -= other.AdjAmt;
				}

				ARAdjust fromar = PXSelectGroupBy<ARAdjust, Where<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>, And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>, And<ARAdjust.released, Equal<boolFalse>>>>, Aggregate<GroupBy<ARAdjust.adjgDocType, GroupBy<ARAdjust.adjgRefNbr, Sum<ARAdjust.curyAdjgAmt, Sum<ARAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);
				if (fromar != null && fromar.AdjdRefNbr != null)
				{
					payment.CuryDocBal -= fromar.CuryAdjgAmt;
					payment.DocBal -= fromar.AdjAmt;
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
						CuryDocBal = (decimal)payment.CuryDocBal;
					}
					else
					{
						PXDBCurrencyAttribute.CuryConvCury(Adjustments.Cache, inv_info, (decimal)payment.DocBal, out CuryDocBal);
					}
					adj.CuryDocBal = CuryDocBal;
				}
			}
		}

		protected virtual void SOAdjust_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = true;
			e.Cancel = true;
		}

		protected virtual void SOAdjust_CuryAdjdAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOAdjust adj = (SOAdjust)e.Row;
			Terms terms = PXSelect<Terms, Where<Terms.termsID, Equal<Current<SOOrder.termsID>>>>.Select(this);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single && (decimal)e.NewValue > 0m)
			{
				throw new PXSetPropertyException(AR.Messages.PrepaymentAppliedToMultiplyInstallments);
			}

			if (adj.AdjgCuryInfoID == null || adj.CuryDocBal == null)
			{
				PXResult<ARPayment, CurrencyInfo> res = (PXResult<ARPayment, CurrencyInfo>)PXSelectJoin<ARPayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>, And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);

				ARPayment payment = PXCache<ARPayment>.CreateCopy(res);
				CurrencyInfo pay_info = (CurrencyInfo)res;
				CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>>.Select(this);

				SOAdjust other = PXSelectGroupBy<SOAdjust, Where<SOAdjust.adjgDocType, Equal<Required<SOAdjust.adjgDocType>>, And<SOAdjust.adjgRefNbr, Equal<Required<SOAdjust.adjgRefNbr>>, And<Where<SOAdjust.adjdOrderType, NotEqual<Required<SOAdjust.adjdOrderType>>, Or<SOAdjust.adjdOrderNbr, NotEqual<Required<SOAdjust.adjdOrderNbr>>>>>>>, Aggregate<GroupBy<SOAdjust.adjgDocType, GroupBy<SOAdjust.adjgRefNbr, Sum<SOAdjust.curyAdjgAmt, Sum<SOAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr, adj.AdjdOrderType, adj.AdjdOrderNbr);
				if (other != null && other.AdjdOrderNbr != null)
				{
					payment.CuryDocBal -= other.CuryAdjgAmt;
					payment.DocBal -= other.AdjAmt;
				}

				ARAdjust fromar = PXSelectGroupBy<ARAdjust, Where<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>, And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>, And<ARAdjust.released, Equal<boolFalse>>>>, Aggregate<GroupBy<ARAdjust.adjgDocType, GroupBy<ARAdjust.adjgRefNbr, Sum<ARAdjust.curyAdjgAmt, Sum<ARAdjust.adjAmt>>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);
				if (fromar != null && fromar.AdjdRefNbr != null)
				{
					payment.CuryDocBal -= fromar.CuryAdjgAmt;
					payment.DocBal -= fromar.AdjAmt;
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
				adj.AdjgCuryInfoID = payment.CuryInfoID;
			}

			if (adj.AdjdCuryInfoID == null || adj.AdjdOrigCuryInfoID == null)
			{
				adj.AdjdCuryInfoID = Document.Current.CuryInfoID;
				adj.AdjdOrigCuryInfoID = Document.Current.CuryInfoID;
			}

			if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt - (decimal)e.NewValue < 0)
			{
				throw new PXSetPropertyException(AR.Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt).ToString());
			}
		}

		protected virtual void SOAdjust_CuryAdjdAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOAdjust adj = (SOAdjust)e.Row;
			decimal CuryAdjgAmt;
			decimal AdjdAmt;
			decimal AdjgAmt;

			PXDBCurrencyAttribute.CuryConvBase<SOAdjust.adjdCuryInfoID>(sender, e.Row, (decimal)adj.CuryAdjdAmt, out AdjdAmt);

			CurrencyInfo pay_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOAdjust.adjgCuryInfoID>>>>.SelectSingleBound(this, new object[] { adj });
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOAdjust.adjdCuryInfoID>>>>.SelectSingleBound(this, new object[] { adj });

			if (string.Equals(pay_info.CuryID, inv_info.CuryID))
			{
				CuryAdjgAmt = (decimal)adj.CuryAdjdAmt;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvCury<SOAdjust.adjgCuryInfoID>(sender, e.Row, AdjdAmt, out CuryAdjgAmt);
			}

			if (object.Equals(pay_info.CuryID, inv_info.CuryID) && object.Equals(pay_info.CuryRate, inv_info.CuryRate) && object.Equals(pay_info.CuryMultDiv, inv_info.CuryMultDiv))
			{
				AdjgAmt = AdjdAmt;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvBase<SOAdjust.adjgCuryInfoID>(sender, e.Row, CuryAdjgAmt, out AdjgAmt);
			}

			adj.CuryAdjgAmt = CuryAdjgAmt;
			adj.AdjAmt = AdjdAmt;
			adj.RGOLAmt = AdjgAmt - AdjdAmt;
			adj.CuryDocBal = adj.CuryDocBal + (decimal?)e.OldValue - adj.CuryAdjdAmt;
		}

		protected virtual void SOAdjust_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SOAdjust row = e.Row as SOAdjust;
			if (row == null) return;

			viewPayment.SetEnabled(!string.IsNullOrEmpty(row.AdjgRefNbr) && !string.IsNullOrEmpty(row.AdjgRefNbr));
			PXUIFieldAttribute.SetEnabled<SOAdjust.adjgDocType>(sender, row, row.AdjgRefNbr == null);
		}

		#endregion


		#region SOOrderShipment Events
		protected virtual void SOOrderShipment_ShipmentNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void SOOrderShipment_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<SOOrderShipment.selected>(sender, e.Row, true);
		}
		#endregion

		#region SOOrder Events

		protected virtual void SOOrder_Cancelled_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOOrder row = (SOOrder) e.Row;
			if (row!= null && (row.IsCCAuthorized == true || row.IsCCCaptured == true))
			{
				bool authIsValid = true;
				if (row.IsCCAuthorized == true)
				{
					if (row.CCAuthTranNbr != null)
					{
						CCProcTran authTran = PXSelect<CCProcTran, Where<CCProcTran.tranNbr, Equal<Required<CCProcTran.tranNbr>>>>.Select(this, row.CCAuthTranNbr);
						if (String.IsNullOrEmpty(authTran.DocType) == false && String.IsNullOrEmpty(authTran.RefNbr) == false)
						{
							authIsValid = false;
						}
					}
					else
					{
						CCProcTran authTran  = this.ccAuthTrans.Select(); //Double-checking for valid auth tran
						if(authTran == null)
							authIsValid = false;
					}

					if (authIsValid && row.CCAuthExpirationDate.HasValue)
					{
						authIsValid = row.CCAuthExpirationDate.Value > PXTimeZoneInfo.Now;
					}					
				}
				if (authIsValid) 
					throw new PXSetPropertyException(Messages.CannotCancelCCProcessed);
			}
		}

		protected virtual void SOOrder_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				//Do not redefault if value exists and overide flag is ON:
				if (!string.IsNullOrEmpty(row.TaxZoneID) && row.OverrideTaxZone == true)
				{
					e.NewValue = row.TaxZoneID;
					return;
				}

				Location customerLocation = location.Select();
				if (customerLocation != null)
				{
					if (!string.IsNullOrEmpty(customerLocation.CTaxZoneID))
					{
						TaxZone taxZone = PXSelect<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<TaxZone.taxZoneID>>>>.Select(this, customerLocation.CTaxZoneID);
						if (taxZone != null)
						{
							e.NewValue = customerLocation.CTaxZoneID;
							return;
						}
					}
				}

				if (IsCommonCarrier(row.ShipVia))
				{
					SOAddress address = Shipping_Address.Select();
					if (address != null && !string.IsNullOrEmpty(address.PostalCode))
						e.NewValue = TaxBuilderEngine.GetTaxZoneByZip(this, address.PostalCode);
				}
				else
				{
					BAccount companyAccount = PXSelectJoin<BAccountR, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, row.BranchID);
					if (companyAccount != null)
					{
						Location companyLocation = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this, companyAccount.BAccountID, companyAccount.DefLocationID);
						if ( companyLocation != null )
							e.NewValue = companyLocation.VTaxZoneID;
					}
				}
			}
		}

		protected virtual void SOOrder_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOOrder.taxZoneID>(e.Row);
		}

		protected virtual void SOOrder_CreditHold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null && PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>())
			{
				SOSetupApproval setupApproval = SetupApproval.Select();
				if (setupApproval != null && (setupApproval.IsActive ?? false))
				{
					sender.RaiseFieldUpdated<SOOrder.hold>(row, true);
				}
			}
		}

		protected virtual bool IsCommonCarrier(string carrierID)
		{
			if (string.IsNullOrEmpty(carrierID))
			{
				return false; //pickup;
			}
			else
			{
				Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(this, carrierID);
				if (carrier == null)
				{
					return false;
				}
				else
				{
					return carrier.IsCommonCarrier == true;
				}
			}
		}

		protected virtual void SOOrder_OrderType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (orderType != null)
			{
				e.NewValue = orderType;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_DestinationSiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || !IsTransferOrder)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_DestinationSiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Company.RaiseFieldUpdated(sender, e.Row);
			Branch company = null;
            string destinationSiteIdErrorMessage = string.Empty;

			using (new PXReadBranchRestrictedScope())
			{
				company = Company.Select();
			}

			if (IsTransferOrder && company != null)
			{
				sender.SetValueExt<SOOrder.customerID>(e.Row, company.BranchCD);
			} 

            try
            {
			SOShippingAddressAttribute.DefaultRecord<SOOrder.shipAddressID>(sender, e.Row);
            }
            catch (SharedRecordMissingException)
            {
                var missingaddressexception = new PXSetPropertyException(Messages.DestinationSiteAddressMayNotBeEmpty, PXErrorLevel.Error);
                if (UnattendedMode)
                    throw missingaddressexception;
                else
                    sender.RaiseExceptionHandling<SOOrder.destinationSiteID>(e.Row, sender.GetValueExt<SOOrder.destinationSiteID>(e.Row), missingaddressexception);

                destinationSiteIdErrorMessage = Messages.DestinationSiteAddressMayNotBeEmpty;
                sender.SetValueExt<SOOrder.shipAddressID>(e.Row, null);
            }
            try
            {
			SOShippingContactAttribute.DefaultRecord<SOOrder.shipContactID>(sender, e.Row);
		}
            catch (SharedRecordMissingException)
            {
                var missingcontactexception = new PXSetPropertyException(Messages.DestinationSiteContactMayNotBeEmpty, PXErrorLevel.Error);
                if (UnattendedMode)
                    throw missingcontactexception;
                else
                    sender.RaiseExceptionHandling<SOOrder.destinationSiteID>(e.Row, sender.GetValueExt<SOOrder.destinationSiteID>(e.Row), missingcontactexception);

                destinationSiteIdErrorMessage = Messages.DestinationSiteContactMayNotBeEmpty;
                sender.SetValueExt<SOOrder.shipContactID>(e.Row, null);
            }

            sender.SetValueExt<SOOrder.destinationSiteIdErrorMessage>(e.Row, destinationSiteIdErrorMessage);
        }

		protected virtual void SOOrder_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOOrder.branchID>(e.Row);
			sender.SetDefaultExt<SOOrder.salesPersonID>(e.Row);
			sender.SetDefaultExt<SOOrder.taxZoneID>(e.Row);
			sender.SetDefaultExt<SOOrder.avalaraCustomerUsageType>(e.Row);
			sender.SetDefaultExt<SOOrder.workgroupID>(e.Row);
			sender.SetDefaultExt<SOOrder.shipVia>(e.Row);
			sender.SetDefaultExt<SOOrder.fOBPoint>(e.Row);
			sender.SetDefaultExt<SOOrder.resedential>(e.Row);
			sender.SetDefaultExt<SOOrder.saturdayDelivery>(e.Row);
			sender.SetDefaultExt<SOOrder.groundCollect>(e.Row);
			sender.SetDefaultExt<SOOrder.insurance>(e.Row);
			sender.SetDefaultExt<SOOrder.shipTermsID>(e.Row);
			sender.SetDefaultExt<SOOrder.shipZoneID>(e.Row);
			sender.SetDefaultExt<SOOrder.defaultSiteID>(e.Row);
			sender.SetDefaultExt<SOOrder.priority>(e.Row);
			if (customerChanged)
			{
				if (!HasDetailRecords())
					sender.SetDefaultExt<SOOrder.shipComplete>(e.Row);
			}
			else
			{
				sender.SetDefaultExt<SOOrder.shipComplete>(e.Row);
			}
			sender.SetDefaultExt<SOOrder.shipDate>(e.Row);

			try
			{
                try
                {
				SOShippingAddressAttribute.DefaultRecord<SOOrder.shipAddressID>(sender, e.Row);
                }
                catch (SharedRecordMissingException)
                {
                    var missingaddressexception = new PXSetPropertyException(Messages.DestinationSiteAddressMayNotBeEmpty, PXErrorLevel.Error);
                    if (UnattendedMode)
                        throw missingaddressexception;
                    else
                        sender.RaiseExceptionHandling<SOOrder.destinationSiteID>(e.Row, sender.GetValueExt<SOOrder.destinationSiteID>(e.Row), missingaddressexception);
                }
                try
                {
				SOShippingContactAttribute.DefaultRecord<SOOrder.shipContactID>(sender, e.Row);
			}
                catch (SharedRecordMissingException)
                {
                    var missingcontactexception = new PXSetPropertyException(Messages.DestinationSiteContactMayNotBeEmpty, PXErrorLevel.Error);
                    if (UnattendedMode)
                        throw missingcontactexception;
                    else
                        sender.RaiseExceptionHandling<SOOrder.destinationSiteID>(e.Row, sender.GetValueExt<SOOrder.destinationSiteID>(e.Row), missingcontactexception);
                }
            }
			catch (PXFieldValueProcessingException ex)
			{
				ex.ErrorValue = location.Current.LocationCD;
				throw;
			}

			foreach (SOLine line in Transactions.Select())
			{				
				try
				{
					Transactions.Cache.SetDefaultExt<SOLine.salesAcctID>(line);
				}
				catch (PXSetPropertyException)
				{
					Transactions.Cache.SetValue<SOLine.salesAcctID>(line, null);
				}
			}
		}

		protected virtual void SOOrder_CustomerID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row == null) return;

			if (!HasDetailRecords())
				return;

			Customer newCustomer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, e.NewValue);
			if (newCustomer != null)
			{
				if (newCustomer.AllowOverrideCury != true && newCustomer.CuryID != row.CuryID && !string.IsNullOrEmpty(newCustomer.CuryID))
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithRestrictedCurrency);
				}

				if (currencyinfo.Current != null && 
					currencyinfo.Current.CuryID != currencyinfo.Current.BaseCuryID && 
					newCustomer.AllowOverrideRate != true && 
					newCustomer.CuryRateTypeID != currencyinfo.Current.CuryRateTypeID)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithRestrictedRateType);
				}
			}
						
			if (Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Inserted)
			{
				SOOrderShipment topShipment = shipmentlist.SelectSingle();
				if (topShipment != null)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnShippedOrder);
				}

				PXSelectBase<POLine> selectlinkedDropShips = new PXSelectJoin<POLine,
					InnerJoin<SOLineSplit, On<SOLineSplit.pOType, Equal<POLine.orderType>, And<SOLineSplit.pONbr, Equal<POLine.orderNbr>, And<SOLineSplit.pOLineNbr, Equal<POLine.lineNbr>>>>>,
					Where<SOLineSplit.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOLineSplit.orderNbr, Equal<Current<SOOrder.orderNbr>>,
					And<POLine.orderType, Equal<POOrderType.dropShip>>>>>(this);

				POLine topPOLine = selectlinkedDropShips.SelectWindowed(0, 1);
				if (topPOLine != null)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithDropShip);
				}

				SOOrder associatedOrder = PXSelect<SOOrder, Where<SOOrder.origOrderType, Equal<Current<SOOrder.orderType>>,
				And<SOOrder.origOrderNbr, Equal<Current<SOOrder.orderNbr>>>>>.Select(this);
				if (associatedOrder != null && associatedOrder.CustomerID != (int?) e.NewValue)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnQuoteWithOrder);
				}

				if (row.IsCCAuthorized == true || row.IsCCCaptured == true)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithCCProcessed);
				}

				SOAdjust topSOAdjust = Adjustments.SelectSingle();
				if (topSOAdjust != null)
				{
					RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithARPayments);
				}
			}

			SOLine invoiced = (SOLine)Transactions.Select().Where(res => !string.IsNullOrEmpty(((SOLine)res).InvoiceNbr)).FirstOrDefault();
			if (invoiced != null)
			{
				RaiseCustomerIDSetPropertyException(sender, row, e.NewValue, Messages.CustomerChangedOnOrderWithInvoices);
			}

			
		}

		public virtual void RaiseCustomerIDSetPropertyException(PXCache sender, SOOrder order, object newCustomerID, string error)
		{
			BAccountR newAccount = (BAccountR)PXSelectorAttribute.Select<SOOrder.customerID>(sender, order, newCustomerID);
			var ex = new PXSetPropertyException(error);
			ex.ErrorValue = newAccount?.AcctCD;
			throw ex;
		}

		public virtual bool HasDetailRecords()
		{
			if (Transactions.Current != null)
				return true;

			if ( Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Inserted )
			{
				return Transactions.Cache.IsDirty;
			}
			else
			{
				return Transactions.Select().Count > 0;
			}
		}

		protected bool customerChanged = false;
		protected virtual void SOOrder_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			int? oldCustomerID = (int?)e.OldValue;
			customerChanged = oldCustomerID != null && ((SOOrder)e.Row).CustomerID != oldCustomerID;

			sender.SetValue<SOOrder.creditHold>(e.Row, false);
			sender.SetValue<SOOrder.inclCustOpenOrders>(e.Row, true);
			sender.SetValue<SOOrder.customerRefNbr>(e.Row, null);
			sender.SetValue<SOOrder.extRefNbr>(e.Row, null);
			sender.SetValue<SOOrder.approvedCredit>(e.Row, false);
			sender.SetValue<SOOrder.approvedCreditAmt>(e.Row, 0m);
			sender.SetValue<SOOrder.overrideTaxZone>(e.Row, false);
			sender.SetDefaultExt<SOOrder.billSeparately>(e.Row);
			sender.SetDefaultExt<SOOrder.shipSeparately>(e.Row);
			sender.SetValue<SOOrder.origOrderType>(e.Row, null);
			sender.SetValue<SOOrder.origOrderNbr>(e.Row, null);
			

			if (!e.ExternalCall && customer.Current != null)
			{
				customer.Current.CreditRule = null;
			}

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<SOOrder.curyID>(e.Row) == null)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<SOOrder.curyInfoID>(sender, e.Row);

					string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<SOOrder.orderDate>(e.Row, ((SOOrder)e.Row).OrderDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						((SOOrder)e.Row).CuryID = info.CuryID;
					}
				}
			}

			{
				sender.SetDefaultExt<SOOrder.customerLocationID>(e.Row);
				if (e.ExternalCall || sender.GetValuePending<SOOrder.termsID>(e.Row) == null)
				{
					if (soordertype.Current.ARDocType != ARDocType.CreditMemo)
					{
						sender.SetDefaultExt<SOOrder.termsID>(e.Row);
					}
					else
					{
						sender.SetValueExt<SOOrder.termsID>(e.Row, null);
					}
				}
				sender.SetDefaultExt<SOOrder.paymentMethodID>(e.Row);
				//sender.SetDefaultExt<SOOrder.pMInstanceID>(e.Row);
			}


			try
			{
				SOBillingAddressAttribute.DefaultRecord<SOOrder.billAddressID>(sender, e.Row);
				SOBillingContactAttribute.DefaultRecord<SOOrder.billContactID>(sender, e.Row);
			}
			catch (PXFieldValueProcessingException ex)
			{
				ex.ErrorValue = customer.Current.AcctCD;
				throw;
			}
		    SOOrder row = e.Row as SOOrder;
		    if (row != null)
		    {
                sender.SetDefaultExt<SOOrder.ownerID>(row);
				
		    }
		    sender.SetDefaultExt<SOOrder.taxZoneID>(e.Row);
			sender.SetDefaultExt<SOOrder.createPMInstance>(e.Row);

			foreach( SOLine line in Transactions.Select())
			{
				line.CustomerID = Document.Current.CustomerID;
				Transactions.Update(line);
			}
			if (row.ProjectID != null)
			{
				//show warning if Project.Customer - Customer missmatch
				object val = row.ProjectID;
				sender.RaiseFieldVerifying<SOOrder.projectID>(row, ref val);
			}
		}

		protected virtual void SOOrder_PaymentMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOOrder.pMInstanceID>(e.Row);
			sender.SetDefaultExt<SOOrder.cashAccountID>(e.Row);            
		}

		protected virtual void SOOrder_PMInstanceID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOOrder.cashAccountID>(e.Row);
			sender.SetValueExt<SOOrder.refTranExtNbr>(e.Row, null);
		}

		protected virtual void SOOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			SOOrder doc = (SOOrder)e.Row;

			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (IsInvoiceOrder && doc.BillSeparately == true && doc.InvoiceDate == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.invoiceDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.invoiceDate).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.invoiceDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.invoiceDate).Name);
					}
				}

				if (IsInvoiceOrder && doc.BillSeparately == true && doc.FinPeriodID == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.finPeriodID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.finPeriodID).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.finPeriodID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.finPeriodID).Name);
					}
				}

				object val;

				if (IsInvoiceOrder && IsUserInvoiceNumbering && ((val = sender.GetValueExt<SOOrder.invoiceNbr>(e.Row)) is PXFieldState ? ((PXFieldState)val).Value : val) == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.invoiceNbr>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.invoiceNbr).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.invoiceNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.invoiceNbr).Name);
					}
				}

				bool pmInstanceRequired = false; 
				if (this.IsPaymentInfoEnabled && String.IsNullOrEmpty(doc.PaymentMethodID) == false) 
				{
					PaymentMethod pm = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
					pmInstanceRequired = (pm!= null && pm.IsAccountNumberRequired == true); 					
				}
				PXDefaultAttribute.SetPersistingCheck<SOOrder.pMInstanceID>(sender,doc,pmInstanceRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing); 					

				if (IsInvoiceOrder && doc.BillSeparately == true && doc.ARDocType != ARDocType.CreditMemo && doc.DueDate == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.dueDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.dueDate).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.dueDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.dueDate).Name);
					}
				}

				if (IsInvoiceOrder && doc.BillSeparately == true && doc.ARDocType != ARDocType.CreditMemo && doc.DiscDate == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.discDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.discDate).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.discDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.discDate).Name);
					}
				}


				if (IsInvoiceOrder && doc.BillSeparately == true && (doc.ARDocType == ARDocType.CashSale || doc.ARDocType == ARDocType.CashReturn) && sender.GetValueExt<SOOrder.cashAccountID>(e.Row) == null)
				{
					if (sender.RaiseExceptionHandling<SOOrder.cashAccountID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.cashAccountID).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.cashAccountID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.cashAccountID).Name);
					}
				}


				//if (IsInvoiceOrder && doc.BillSeparately == true && (doc.ARDocType == ARDocType.CashSale || doc.ARDocType == ARDocType.CashReturn) && doc.PMInstanceID == null)
				//{
				//    if (sender.RaiseExceptionHandling<SOOrder.pMInstanceID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.pMInstanceID).Name)))
				//    {
				//        throw new PXRowPersistingException(typeof(SOOrder.pMInstanceID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.pMInstanceID).Name);
				//    }
				//}


				if (IsInvoiceOrder && doc.BillSeparately == true && (doc.ARDocType == ARDocType.CashSale || doc.ARDocType == ARDocType.CashReturn) && string.IsNullOrEmpty(doc.ExtRefNbr))
				{
					if (sender.RaiseExceptionHandling<SOOrder.extRefNbr>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(SOOrder.extRefNbr).Name)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.extRefNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.extRefNbr).Name);
					}
				}

				if (((IsInvoiceOrder && doc.BillSeparately == true && (doc.ARDocType == ARDocType.CashSale || doc.ARDocType == ARDocType.CashReturn)) || doc.ARDocType == ARDocType.Invoice)
						&& string.IsNullOrEmpty(doc.PreAuthTranNumber) == false && doc.CCAuthTranNbr == null
						&& (doc.CuryCCPreAuthAmount <= Decimal.Zero))
				{
					if (sender.RaiseExceptionHandling<SOOrder.curyCCPreAuthAmount>(e.Row, doc.CuryCCPreAuthAmount, new PXSetPropertyException(Messages.PreAutorizationAmountShouldBeEntered)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.preAuthTranNumber).Name, doc.PreAuthTranNumber, Messages.PreAutorizationAmountShouldBeEntered);
					}
				}

				if ((doc.CuryDiscTot ?? 0m) > Math.Abs((doc.CuryLineTotal ?? 0m) + (doc.CuryMiscTot ?? 0m)))
				{
					if (sender.RaiseExceptionHandling<SOOrder.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(AR.Messages.DiscountGreaterLineMiscTotal, PXErrorLevel.Error)))
					{
						throw new PXRowPersistingException(typeof(SOOrder.curyDiscTot).Name, null, AR.Messages.DiscountGreaterLineMiscTotal);
					}
				}

                if (doc.Status == SOOrderStatus.Completed && doc.Hold == true)
                {
                    throw new PXRowPersistingException(typeof(SOOrder.status).Name, null, PXMessages.LocalizeFormatNoPrefixNLA(Messages.DocumentOnHoldCannotBeCompleted, doc.OrderNbr));
                }
				if (IsTransferOrder && doc.DestinationSiteID == null)
				{
					throw new PXRowPersistingException(typeof(SOOrder.destinationSiteID).Name, null, ErrorMessages.FieldIsEmpty, typeof(SOOrder.destinationSiteID).Name);
				}

                if (doc != null && IsTransferOrder)
                {
                    var destinationSiteIdErrorString = PXUIFieldAttribute.GetError<SOOrder.destinationSiteID>(sender, e.Row);
                    if (destinationSiteIdErrorString == null)
                    {
                        var destinationSiteIdErrorMessage = doc.DestinationSiteIdErrorMessage;
                        if (!string.IsNullOrWhiteSpace(destinationSiteIdErrorMessage))
                        {
                            throw new PXRowPersistingException(typeof(SOOrder.destinationSiteID).Name, sender.GetValueExt<SOOrder.destinationSiteID>(e.Row), destinationSiteIdErrorMessage);
                        }
                    }
                }
			}

			//if (e.Operation == PXDBOperation.Delete && doc.OrigPONbr != null && doc.OrigPOType != null)
			//{
			//    POOrderEntry poEntry = (POOrderEntry)POOrderEntry.CreateInstance(typeof(POOrderEntry));
			//    POOrder order = PXCache<POOrder>.CreateCopy(poEntry.Document.Search<POOrder.orderNbr>(doc.OrigPONbr, doc.OrigPOType));
			//    order.Cancelled = true;
			//    poEntry.Document.Update(order);
			//    poEntry.Document.Search<POOrder.orderNbr>(doc.OrigPONbr, doc.OrigPOType);
			//    poEntry.Save.Press();
			//}
			//}

			if (e.Operation == PXDBOperation.Update)
			{
				if (doc.ShipmentCntr < 0 || doc.OpenShipmentCntr < 0 || doc.ShipmentCntr < doc.BilledCntr + doc.ReleasedCntr && doc.Behavior == SOBehavior.SO)
				{
					throw new PXSetPropertyException(Messages.InvalidShipmentCounters);
				}
			}

            if (IsMobile) // check control total when persisting from mobile
            {
                if (((SOOrder)e.Row).Hold == false && ((SOOrder)e.Row).Completed == false)
                {
                    if (((SOOrder)e.Row).CuryOrderTotal != ((SOOrder)e.Row).CuryControlTotal)
                    {
                        sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, ((SOOrder)e.Row).CuryControlTotal, new PXSetPropertyException(Messages.DocumentOutOfBalance));
                    }
                    else if (((SOOrder)e.Row).CuryOrderTotal < 0m && ((SOOrder)e.Row).ARDocType != ARDocType.NoUpdate && doc.Behavior != SOBehavior.RM)
                    {
                        if (soordertype.Current.RequireControlTotal == true)
                        {
                            sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, ((SOOrder)e.Row).CuryControlTotal, new PXSetPropertyException(Messages.DocumentBalanceNegative));
                        }
                        else
                        {
                            sender.RaiseExceptionHandling<SOOrder.curyOrderTotal>(e.Row, ((SOOrder)e.Row).CuryOrderTotal, new PXSetPropertyException(Messages.DocumentBalanceNegative));
                        }
                    }
                    else
                    {
                        if (soordertype.Current.RequireControlTotal == true)
                        {
                            sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, null, null);
                        }
                        else
                        {
                            sender.RaiseExceptionHandling<SOOrder.curyOrderTotal>(e.Row, null, null);
                        }
                    }
                }
            }
        }

		protected virtual void SOOrder_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			var row = (SOOrder)e.Row;
			if (e.TranStatus == PXTranStatus.Open)
				foreach (CROpportunity opportunity in OpportunityBackReference.Select())
				{
					opportunity.OrderNbr = row.OrderNbr;
					opportunity.OrderType = row.OrderType;
				}
		}

		protected virtual void SOOrder_OrderDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CurrencyInfoAttribute.SetEffectiveDate<SOOrder.orderDate>(sender, e);
		}

		protected virtual void SOOrder_BillSeparately_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOOrder.invoiceDate>(e.Row);
			sender.SetDefaultExt<SOOrder.invoiceNbr>(e.Row);
			sender.SetDefaultExt<SOOrder.pMInstanceID>(e.Row);
			sender.SetDefaultExt<SOOrder.extRefNbr>(e.Row);

			if (((SOOrder)e.Row).BillSeparately == false)
			{
				sender.SetValuePending<SOOrder.invoiceDate>(e.Row, null);
				sender.SetValuePending<SOOrder.invoiceNbr>(e.Row, null);
				sender.SetValuePending<SOOrder.pMInstanceID>(e.Row, null);
				sender.SetValuePending<SOOrder.extRefNbr>(e.Row, null);
			}
		}

		protected virtual void SOOrder_FinPeriodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || ((SOOrder)e.Row).BillSeparately == false || soordertype.Current == null || IsInvoiceOrder == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_InvoiceDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || ((SOOrder)e.Row).BillSeparately == false || soordertype.Current == null || IsInvoiceOrder == false)
			{
				e.NewValue = null;
			}
			else
			{
				e.NewValue = sosetup.Current.UseShipDateForInvoiceDate == true ? sender.GetValue<SOOrder.shipDate>(e.Row) : sender.GetValue<SOOrder.orderDate>(e.Row);
			}
			e.Cancel = true;
		}

		protected virtual void SOOrder_InvoiceNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || ((SOOrder)e.Row).BillSeparately == false || soordertype.Current == null || IsInvoiceOrder == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_Priority_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				if (location.Current != null && location.Current.COrderPriority != null)
				{
					e.NewValue = location.Current.COrderPriority ?? 0;
				}
			}
		}

		protected virtual void SOOrder_ShipComplete_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				if (location.Current != null && !string.IsNullOrEmpty(location.Current.CShipComplete))
				{
					e.NewValue = location.Current.CShipComplete;
				}
				else
				{
					e.NewValue = SOShipComplete.CancelRemainder;
				}

				if ((string)e.NewValue == SOShipComplete.BackOrderAllowed && soordertype.Current != null && soordertype.Current.RequireLocation == true)
				{
					e.NewValue = SOShipComplete.CancelRemainder;
				}
			}
		}

		protected virtual void SOOrder_PMInstanceID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || soordertype.Current == null || IsPaymentInfoEnabled == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_PaymentMethodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || soordertype.Current == null || IsPaymentInfoEnabled == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_CashAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || soordertype.Current == null || IsPaymentInfoEnabled == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_OverrideTaxZone_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row == null) return;

			if (row.OverrideTaxZone != true)
			{
				sender.SetDefaultExt<SOOrder.taxZoneID>(e.Row);
			}
		}

		protected virtual void SOOrder_ShipVia_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				if (row.OverrideTaxZone != true  && (e.OldValue == null || (e.OldValue != null && IsCommonCarrier(e.OldValue.ToString()) != IsCommonCarrier(row.ShipVia))))
				sender.SetDefaultExt<SOOrder.taxZoneID>(e.Row);

				sender.SetDefaultExt<SOOrder.freightTaxCategoryID>(e.Row);
				row.UseCustomerAccount = CanUseCustomerAccount(row);

				if ((e.OldValue != null && e.OldValue.ToString() == row.ShipVia) || Document.Current.IsManualPackage == true)
				{
					// do not delete packages
				}
				else
				{
					//autopackaging
					if (string.IsNullOrEmpty(row.ShipVia))
					{
						foreach (SOPackageInfoEx package in Packages.Select())
						{
							Packages.Delete(package);
						}
						row.PackageWeight = 0;
					}
					else
					{
						RecalculatePackagesForOrder(Document.Current);
					}
				}
			}
		}

		protected virtual void SOOrder_IsManualPackage_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null && row.IsManualPackage != true)
			{
				foreach (SOPackageInfoEx pack in Packages.Select())
				{
					Packages.Delete(pack);
				}
				row.PackageWeight = 0;
				sender.SetValue<SOOrder.isPackageValid>(row, false);
			}
		}

		protected virtual void SOOrder_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				foreach (SOLine tran in Transactions.Select())
				{
					tran.ProjectID = row.ProjectID;
					Transactions.Update(tran);
				}

			}
		}

		protected virtual bool CanUseCustomerAccount(SOOrder row)
		{
			Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(this, row.ShipVia);
			if (carrier != null && !string.IsNullOrEmpty(carrier.CarrierPluginID))
			{
				foreach (CarrierPluginCustomer cpc in PXSelect<CarrierPluginCustomer, 
						Where<CarrierPluginCustomer.carrierPluginID, Equal<Required<CarrierPluginCustomer.carrierPluginID>>,
						And<CarrierPluginCustomer.customerID, Equal<Required<CarrierPluginCustomer.customerID>>,
						And<CarrierPluginCustomer.isActive, Equal<True>>>>>.Select(this, carrier.CarrierPluginID, row.CustomerID))
				{
					if ( !string.IsNullOrEmpty(cpc.CarrierAccount) &&
						(cpc.CustomerLocationID == row.CustomerLocationID || cpc.CustomerLocationID == null )
						)
					{
						return true;
					}
				}								
			}

			return false;
		}

		protected virtual void SOOrder_UseCustomerAccount_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null)
			{
				bool canBeTrue = CanUseCustomerAccount(row);

				if (e.NewValue != null && ((bool)e.NewValue) && !canBeTrue)
				{
					e.NewValue = false;
					throw new PXSetPropertyException(Messages.CustomeCarrierAccountIsNotSetup);
				}
			}
		}


		protected virtual void SOOrder_ShipDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			cache.SetDefaultExt<SOOrder.invoiceDate>(e.Row);

			DateTime? oldDate = (DateTime?) e.OldValue;
			if (oldDate != ((SOOrder)e.Row).ShipDate)
			{
				if ((SOLine)Transactions.Select() != null && Document.View.Answer == WebDialogResult.None && ((SOOrder)e.Row).ShipComplete == SOShipComplete.BackOrderAllowed)
					Document.Ask(GL.Messages.Confirmation, Messages.ConfirmShipDateRecalc, MessageButtons.YesNo);
			}
		}

		//protected virtual void SOOrder_TransferReqNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		//{
		//    foreach(SOOrderShipment s in this.shipmentlist.View.SelectMultiBound( new object[]{e.Row}))
		//    {
		//        SOOrderShipment upd = PXCache<SOOrderShipment>.CreateCopy(s);
		//        upd.OrigPONbr = ((SOOrder) e.Row).OrigPONbr;
		//        this.shipmentlist.Update(upd);
		//    }
		//}

		protected virtual void SOOrder_ExtRefNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || ((SOOrder)e.Row).BillSeparately == false || soordertype.Current == null || IsInvoiceOrder == false || soordertype.Current.ARDocType != ARDocType.CashSale && soordertype.Current.ARDocType != ARDocType.CashReturn)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_CCCardNumber_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = (SOOrder)e.Row;
			string ccCardNumber = row.CCCardNumber;
			if (string.IsNullOrEmpty(ccCardNumber) == false)
			{
				CustomerPaymentMethod existingCPM = null;
				CustomerPaymentMethodDetail existingCpmIDdetail = null;

				foreach (PXResult<CustomerPaymentMethod, PaymentMethod, PaymentMethodDetail, CustomerPaymentMethodDetail> it in PXSelectReadonly2<CustomerPaymentMethod,
										LeftJoin<PaymentMethod, On<PaymentMethod.paymentMethodID, Equal<CustomerPaymentMethod.paymentMethodID>,
											And<PaymentMethod.isActive, Equal<True>,
											And<PaymentMethod.paymentType, Equal<PaymentMethodType.creditCard>>>>,
										LeftJoin<PaymentMethodDetail, On<PaymentMethodDetail.paymentMethodID, Equal<CustomerPaymentMethod.paymentMethodID>,
											And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>, 
											And<PaymentMethodDetail.isIdentifier, Equal<True>>>>,
										LeftJoin<CustomerPaymentMethodDetail, On<CustomerPaymentMethodDetail.pMInstanceID, Equal<CustomerPaymentMethod.pMInstanceID>,
											And<CustomerPaymentMethodDetail.detailID, Equal<PaymentMethodDetail.detailID>>>>>>,
							Where<CustomerPaymentMethod.bAccountID, Equal<Required<CustomerPaymentMethod.bAccountID>>,
										And<CustomerPaymentMethod.isActive, Equal<True>
					//,And<CustomerPaymentMethod.descr, Equal<Required<CustomerPaymentMethod.descr>>>
										>>>.Select(this, row.CustomerID, ccCardNumber))
				{
					CustomerPaymentMethod cpm = (CustomerPaymentMethod)it;
					CustomerPaymentMethodDetail cpmDetail = (CustomerPaymentMethodDetail)it;
					//PaymentMethod def = (PaymentMethod) def; 
					//string id = CustomerPaymentMethodMaint.IDObfuscator.MaskID(row.Value, def.EntryMask, def.DisplayMask);
					if (cpmDetail.PMInstanceID.HasValue && cpmDetail.Value == ccCardNumber
							&& (cpm.ExpirationDate.HasValue == false || cpm.ExpirationDate.Value > DateTime.Now))
					{
						existingCPM = cpm;
						existingCpmIDdetail = cpmDetail;
						break;
					}
				}
				if (existingCPM != null && existingCpmIDdetail != null)
				{
					sender.SetValueExt<SOOrder.pMInstanceID>(e.Row, existingCPM.PMInstanceID);
					row.CCCardNumber = string.Empty;
					//SOOrder copy =(SOOrder)this.Document.Cache.CreateCopy(doc);
					//row.Value = existingCpmIDdetail.Value;
					//copy.CreatePMInstance= false;
					//doc = this.Document.Update(copy);
					//SOOrder copy1 = (SOOrder)this.Document.Cache.CreateCopy(doc);
					//copy1.PMInstanceID = existingCPM.PMInstanceID;
					//doc = this.Document.Update(copy1);
					//isIDChanged = true;
				}
				else
				{
					sender.SetValueExt<SOOrder.createPMInstance>(e.Row, true);
				}
			}
		}
		
		protected bool IsCreditMemoOrder
		{
			get
			{
				return ((soordertype.Current.ARDocType == ARDocType.CreditMemo || soordertype.Current.ARDocType == ARDocType.CashReturn) && (sooperation.Current.INDocType == INTranType.CreditMemo || sooperation.Current.INDocType == INTranType.Return || sooperation.Current.INDocType == INTranType.NoUpdate));
			}
		}

		protected bool IsRMAOrder
		{
			get
			{
				return (soordertype.Current.Behavior == SOOrderTypeConstants.RMAOrder);
			}
		}

		protected bool IsTransferOrder
		{
			get
			{
				return (sooperation.Current.INDocType == INTranType.Transfer);
			}
		}

		protected bool IsDebitMemoOrder
		{
			get
			{
				return (soordertype.Current.ARDocType == ARDocType.DebitMemo && sooperation.Current.INDocType == INTranType.DebitMemo);
			}
		}

		protected bool IsInvoiceOrder
		{
			get
			{
				return (soordertype.Current.RequireShipping == false && soordertype.Current.ARDocType != ARDocType.NoUpdate);
			}
		}

		protected bool  IsPaymentInfoEnabled
		{
			get
			{
				return (soordertype.Current.ARDocType == ARDocType.CashSale
					|| soordertype.Current.ARDocType == ARDocType.CashReturn
					|| soordertype.Current.ARDocType == ARDocType.Invoice);
			}
		}

		protected bool IsCashSale
		{
			get
			{
				return (soordertype.Current.ARDocType == ARDocType.CashSale
					|| soordertype.Current.ARDocType == ARDocType.CashReturn);
			}
		}

		protected bool IsUserInvoiceNumbering
		{
			get
			{
				return (soordertype.Current.UserInvoiceNumbering == true);
			}
		}

		public virtual void SOOrder_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			SOOrder order = (SOOrder)e.Row;
			if (order != null && !e.IsReadOnly)
			{
				RecalcTotals(sender, order);
			}
		}

		private void RecalcTotals(PXCache sender, SOOrder order)
		{
			using (new PXConnectionScope())
			{
				bool IsReadOnly = sender.GetStatus(order) == PXEntryStatus.Notchanged;

				CurrencyInfo inv_info = null;

				decimal? CuryApplAmt = 0m;

				PXView view = IsReadOnly ? this.TypedViews.GetView(Adjustments_Raw.View.BqlSelect, true) : Adjustments_Raw.View;
				foreach (PXResult<SOAdjust, ARRegisterAlias, ARPayment, CurrencyInfo> res in view.SelectMultiBound(new object[] { order }))
				{
					if (inv_info == null)
					{
						SOOrder cached = order;
						if (order.CuryInfoID == null)
						{
							cached = (SOOrder)sender.Locate(order);
						}
						inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>>.SelectSingleBound(this, new object[] { cached });
					}
					ARPayment payment = (ARPayment)res;
					SOAdjust adj = (SOAdjust)res;
					CurrencyInfo pay_info = (CurrencyInfo)res;

                    if (payment != null)
                        PXCache<ARRegister>.RestoreCopy(payment, (ARRegisterAlias)res);

					decimal CuryDocBal;
					if (string.Equals(pay_info.CuryID, inv_info.CuryID))
					{
						CuryDocBal = (decimal)payment.CuryDocBal;
					}
					else
					{
						PXDBCurrencyAttribute.CuryConvCury(Adjustments.Cache, inv_info, (decimal)payment.DocBal, out CuryDocBal);
					}

					if (adj != null)
					{
						if (adj.CuryAdjdAmt > CuryDocBal)
						{
							//if reconsidered need to calc RGOL
							adj.CuryDocBal = CuryDocBal;
							adj.CuryAdjdAmt = 0m;
						}

						CuryApplAmt += adj.CuryAdjdAmt;
					}
				}

				sender.SetValue<SOOrder.curyPaymentTotal>(order, CuryApplAmt);
				sender.RaiseFieldUpdated<SOOrder.curyPaymentTotal>(order, null);
			}
		}

		protected SOOrder _LastSelected;
		protected string orderType;
		protected virtual void SOOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			SOOrder doc = e.Row as SOOrder;

			if (doc == null)
			{
				return;
			}

			orderType = doc.OrderType;

			bool operationVisible = (soordertype.Current.Behavior == SOOrderTypeConstants.RMAOrder || soordertype.Current.Behavior == SOOrderTypeConstants.Invoice);
			if (operationVisible)
			{
				SOOrderTypeOperation nonDefault = PXSelect<SOOrderTypeOperation,
					Where<SOOrderTypeOperation.active, Equal<boolTrue>,
						And<SOOrderTypeOperation.orderType, Equal<Required<SOOrderTypeOperation.orderType>>,
							And<SOOrderTypeOperation.operation, NotEqual<Required<SOOrderTypeOperation.operation>>>>>>
					.SelectWindowed(this, 0, 1, soordertype.Current.OrderType, soordertype.Current.DefaultOperation);
				if (nonDefault == null)
					operationVisible = false;
			}
			if (!object.ReferenceEquals(doc, _LastSelected))
			{
				PXUIFieldAttribute.SetVisible<SOLine.operation>(this.Transactions.Cache, null, operationVisible);
                PXUIFieldAttribute.SetEnabled<SOLine.operation>(this.Transactions.Cache, null, operationVisible);
				PXUIFieldAttribute.SetVisible<SOPackageInfo.operation>(this.Packages.Cache, null, operationVisible);
				PXUIFieldAttribute.SetVisible<SOLine.autoCreateIssueLine>(this.Transactions.Cache, null, operationVisible);
				PXUIFieldAttribute.SetVisible<SOLine.curyUnitCost>(this.Transactions.Cache, null, IsRMAOrder || IsCreditMemoOrder);
				_LastSelected = doc;
			}
			PXUIFieldAttribute.SetVisible<SOOrder.curyID>(cache, doc,
				PXAccess.FeatureInstalled<FeaturesSet.multicurrency>() && orderType != SOOrderTypeConstants.TransferOrder);


			ValidateControlTotal(cache, e);

			bool curyenabled = true;

			if (customer.Current != null && customer.Current.AllowOverrideCury == false)
			{
				curyenabled = false;
			}
			bool isCCPayment = false;
            bool isFreightEditable = true;
			this.authorizeCCPayment.SetEnabled(false);
			this.voidCCPayment.SetEnabled(false);
			this.captureCCPayment.SetEnabled(false);
			this.creditCCPayment.SetEnabled(false);
			this.authorizeCCPayment.SetVisible(false);
			this.voidCCPayment.SetVisible(false);
			this.captureCCPayment.SetVisible(false);
			this.creditCCPayment.SetVisible(false);
			bool isTokenized = CCProcessingUtils.isTokenizedPaymentMethod(this, doc.PMInstanceID);
			bool isHFPM = CCProcessingUtils.isHFPaymentMethod(this, doc.PMInstanceID);
			bool isIDFilled = CCProcessingUtils.isCCPIDFilled(this, doc.PMInstanceID);
			this.createCCPaymentMethodHF.SetVisible(doc.CreatePMInstance == true && isHFPM);
			this.syncCCPaymentMethods.SetVisible(doc.CreatePMInstance == true && isHFPM);
			this.createCCPaymentMethodHF.SetEnabled(doc.CreatePMInstance == true && isHFPM && !isIDFilled);
			this.syncCCPaymentMethods.SetEnabled(doc.CreatePMInstance == true && isHFPM && !isIDFilled
				&& DefPaymentMethodInstance.Current != null && DefPaymentMethodInstance.Current.CustomerCCPID != null);

			PXUIFieldAttribute.SetVisible<CustomerPaymentMethodInputMode.inputMode>(InputModeFilter.Cache, null, doc.CreatePMInstance == true && isTokenized && !isHFPM);

			this.prepareInvoice.SetEnabled(doc.Hold == false && doc.Cancelled == false &&
				soordertype.Current.ARDocType != ARDocType.NoUpdate &&
				(doc.ShipmentCntr - doc.OpenShipmentCntr - doc.BilledCntr - doc.ReleasedCntr) > 0 ||
				(doc.OrderQty == 0 && (doc.CuryUnbilledMiscTot > 0 || doc.UnbilledOrderQty > 0)) ||
				(doc.Status == SOOrderStatus.Open && soordertype.Current.RequireShipping == false));

			PXUIFieldAttribute.SetVisible<SOOrder.refTranExtNbr>(cache, doc, doc.ARDocType == ARDocType.CashReturn);


			bool allowAllocation = soordertype.Current != null && soordertype.Current.RequireAllocation != true
				|| PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()
				|| PXAccess.FeatureInstalled<FeaturesSet.lotSerialTracking>()
				|| PXAccess.FeatureInstalled<FeaturesSet.subItem>()
				|| PXAccess.FeatureInstalled<FeaturesSet.replenishment>()
				|| PXAccess.FeatureInstalled<FeaturesSet.sOToPOLink>()
				;

			if (doc == null || doc.Completed == true || doc.Cancelled == true || !allowAllocation)
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				cache.AllowDelete = false;
				cache.AllowUpdate = allowAllocation;
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = false;
				Transactions.Cache.AllowInsert = false;

				Adjustments.Cache.AllowInsert = false;
				Adjustments.Cache.AllowUpdate = false;
				Adjustments.Cache.AllowDelete = false;

				DiscountDetails.Cache.AllowDelete = false;
				DiscountDetails.Cache.AllowUpdate = false;
				DiscountDetails.Cache.AllowInsert = false;

				Taxes.Cache.AllowUpdate = false;
			}
			else
			{
				doc.CCAuthTranNbr = null;
				CustomerPaymentMethodC cpmRow = null;
				if (doc.PMInstanceID.HasValue)
				{
					cpmRow = this.DefPaymentMethodInstance.Select();
					bool isCCInserted = this.DefPaymentMethodInstance.Cache.GetStatus(cpmRow) == PXEntryStatus.Inserted;
					doc.CreatePMInstance = isCCInserted;

					#region Credit Card Processing

					if (cpmRow != null && !String.IsNullOrEmpty(cpmRow.PaymentMethodID))
					{
						PaymentMethod pm = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, cpmRow.PaymentMethodID);
						if (pm != null && pm.PaymentType == CA.PaymentMethodType.CreditCard)
						{
							isCCPayment = true;
						}
					}

					#endregion
				}
				bool isCCVoided = false;
				bool isCCCaptured = false;
				bool isCCPreAuthorized = false;
				bool isCCRefunded = false;
				bool isCCVoidingAttempted = false; //Special flag for VoidPayment Release logic 
				doc.PCResponseReasonText = string.Empty;
				if (doc.CreatePMInstance == false && doc.PMInstanceID.HasValue && isCCPayment)
				{
					CCProcTran lastTran;
					CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select(), out lastTran);
					isCCVoided = (ccPaymentState & CCPaymentState.Voided) != 0;
					isCCCaptured = (ccPaymentState & CCPaymentState.Captured) != 0;
					isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
					isCCRefunded = (ccPaymentState & CCPaymentState.Refunded) != 0;
					isCCVoidingAttempted = (ccPaymentState & CCPaymentState.VoidFailed) != 0;
					doc.CCPaymentStateDescr = CCPaymentEntry.FormatCCPaymentState(ccPaymentState);
					doc.PCResponseReasonText = lastTran != null ? lastTran.PCResponseReasonText : String.Empty;
					if (isCCPreAuthorized)
					{
						doc.CCAuthTranNbr = lastTran.TranNbr;
						doc.PreAuthTranNumber = lastTran.PCTranNumber;
					}
					if (isCCCaptured)
					{
						doc.CCAuthTranNbr = lastTran.TranNbr;
						doc.CaptureTranNumber = lastTran.PCTranNumber;
					}
				}

				decimal? docBalance = ((ICCPayment)doc).CuryDocBal;
				bool isNotNullAmt = docBalance != null && docBalance > 0;

				bool canAuthorize = !(isCCPreAuthorized || isCCCaptured) && isNotNullAmt;
				bool canCapture = !(isCCCaptured) && isNotNullAmt;
				bool canVoid = (isCCCaptured || isCCPreAuthorized);
				bool isCashReturn = doc.ARDocType == ARDocType.CashReturn;


				bool enableCCAuthEntering = (doc.CreatePMInstance == true || doc.PMInstanceID.HasValue) && isCCPayment
					&& (doc.CCAuthTranNbr == null) && !isCashReturn;

				PXUIFieldAttribute.SetEnabled(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<SOOrder.refTranExtNbr>(cache, doc, (doc.CreatePMInstance == true || doc.PMInstanceID.HasValue) && isCCPayment && isCashReturn && !isCCRefunded);
				PXUIFieldAttribute.SetEnabled<SOOrder.status>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.orderQty>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.orderWeight>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.orderVolume>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.packageWeight>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyOrderTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyUnpaidBalance>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyLineTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyMiscTot>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyFreightCost>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.freightCostIsValid>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyFreightAmt>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyTaxTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.openOrderQty>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyOpenOrderTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyOpenLineTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyOpenTaxTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.unbilledOrderQty>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyUnbilledOrderTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyUnbilledLineTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyUnbilledTaxTotal>(cache, doc, false);

				PXUIFieldAttribute.SetEnabled<SOOrder.curyPaymentTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyID>(cache, doc, curyenabled);
				PXUIFieldAttribute.SetEnabled<SOOrder.preAuthTranNumber>(cache, doc, enableCCAuthEntering);
				PXUIFieldAttribute.SetEnabled<SOOrder.cCPaymentStateDescr>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.cCAuthExpirationDate>(cache, doc, enableCCAuthEntering && String.IsNullOrEmpty(doc.PreAuthTranNumber) == false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyCCPreAuthAmount>(cache, doc, enableCCAuthEntering && String.IsNullOrEmpty(doc.PreAuthTranNumber) == false);
				PXUIFieldAttribute.SetEnabled<SOOrder.pCResponseReasonText>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.captureTranNumber>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyCCCapturedAmt>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.origOrderType>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.origOrderNbr>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyVatExemptTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyVatTaxableTotal>(cache, doc, false);

				this.authorizeCCPayment.SetVisible(isCCPayment && !isCashReturn && !IsTransferOrder);
				this.voidCCPayment.SetVisible(isCCPayment && !isCashReturn && !IsTransferOrder);
				this.captureCCPayment.SetVisible(isCCPayment && !isCashReturn && !IsTransferOrder);
				this.creditCCPayment.SetVisible(isCCPayment && isCashReturn && !IsTransferOrder);

				if (soordertype.Current != null)
				{
					bool isInvoiceInfoEnabled = IsInvoiceOrder && doc.BillSeparately == true;

					//bool hasActiveCCTran = this.ccAuthTrans.Select().Count > 0;
					bool hasActiveCCTran = isCCCaptured || isCCPreAuthorized;
					bool enableCreateCC = (IsPaymentInfoEnabled && doc.CustomerID.HasValue && !hasActiveCCTran);
					bool isPMInstanceRequired = false;
					bool hasPaymentMethod = (String.IsNullOrEmpty(doc.PaymentMethodID) == false);
					if (IsPaymentInfoEnabled && hasPaymentMethod)
					{
						PaymentMethod pm = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
						isPMInstanceRequired = (pm.IsAccountNumberRequired == true);
					}
					PXUIFieldAttribute.SetEnabled<SOOrder.createPMInstance>(cache, doc, enableCreateCC && isPMInstanceRequired);
					PXUIFieldAttribute.SetEnabled<SOOrder.invoiceDate>(cache, doc, isInvoiceInfoEnabled);
					PXUIFieldAttribute.SetEnabled<SOOrder.invoiceNbr>(cache, doc, isInvoiceInfoEnabled);
					PXUIFieldAttribute.SetEnabled<SOOrder.finPeriodID>(cache, doc, isInvoiceInfoEnabled);

					bool enableCCSelection = (IsPaymentInfoEnabled && doc.CustomerID.HasValue
										 && doc.CreatePMInstance != true && !hasActiveCCTran);
					PXUIFieldAttribute.SetEnabled<SOOrder.paymentMethodID>(cache, doc, enableCCSelection);
					PXUIFieldAttribute.SetEnabled<SOOrder.pMInstanceID>(cache, doc, enableCCSelection && isPMInstanceRequired);
					PXUIFieldAttribute.SetEnabled<SOOrder.cCCardNumber>(cache, doc, enableCCSelection && enableCreateCC && !doc.PMInstanceID.HasValue
						&& isPMInstanceRequired && !isTokenized);
					PXUIFieldAttribute.SetEnabled<SOOrder.cashAccountID>(cache, doc, IsPaymentInfoEnabled && hasPaymentMethod);

					PXUIFieldAttribute.SetEnabled<SOOrder.extRefNbr>(cache, doc, IsCashSale);

					if (isInvoiceInfoEnabled && doc.InvoiceDate != null)
					{
						OpenPeriodAttribute.SetValidatePeriod<SOOrder.finPeriodID>(cache, doc, PeriodValidation.DefaultSelectUpdate);
					}
					else
					{
						OpenPeriodAttribute.SetValidatePeriod<SOOrder.finPeriodID>(cache, doc, PeriodValidation.Nothing);
					}
					PXUIFieldAttribute.SetEnabled<SOOrder.dueDate>(cache, doc, isInvoiceInfoEnabled && soordertype.Current.ARDocType != ARDocType.CreditMemo);
					PXUIFieldAttribute.SetEnabled<SOOrder.discDate>(cache, doc, isInvoiceInfoEnabled && soordertype.Current.ARDocType != ARDocType.CreditMemo);

					bool isInserted = cache.GetStatus(doc) == PXEntryStatus.Inserted;

					this.authorizeCCPayment.SetEnabled(!isInserted && isCCPayment && !isCashReturn && canAuthorize);
					this.voidCCPayment.SetEnabled(!isInserted && isCCPayment && !isCashReturn && canVoid);
					this.captureCCPayment.SetEnabled(!isInserted && isCCPayment && !isCashReturn && canCapture);
					this.creditCCPayment.SetEnabled(doc.RefTranExtNbr != null && !isCCRefunded);
				}
				cache.AllowDelete = !CCPaymentEntry.HasSuccessfulCCTrans(ccProcTran);
				cache.AllowUpdate = true;
				bool isAuthorizedCashSale = (doc.ARDocType == ARDocType.CashSale && (doc.IsCCAuthorized == true || doc.IsCCCaptured == true));
				bool isRefundedCashReturn = isCashReturn && isCCRefunded;
				Transactions.Cache.AllowDelete = !isAuthorizedCashSale && !isRefundedCashReturn;
				Transactions.Cache.AllowUpdate = !isAuthorizedCashSale && !isRefundedCashReturn;
				Transactions.Cache.AllowInsert = (doc.CustomerID != null && doc.CustomerLocationID != null && (doc.ProjectID != null || !PM.ProjectAttribute.IsPMVisible(this, BatchModule.SO)))
					&& !isAuthorizedCashSale && !isRefundedCashReturn;
				PXUIFieldAttribute.SetEnabled<SOOrder.curyDiscTot>(cache, doc, !isAuthorizedCashSale && !isRefundedCashReturn);
				PXUIFieldAttribute.SetEnabled<SOOrder.curyPremiumFreightAmt>(cache, doc, !isAuthorizedCashSale && !isRefundedCashReturn);
                isFreightEditable = !isAuthorizedCashSale && !isRefundedCashReturn;

				Taxes.Cache.AllowUpdate = true;

				bool PaymentsAndApplicationsEnabled = soordertype.Current.CanHaveApplications;
				createPayment.SetEnabled(PaymentsAndApplicationsEnabled && cache.GetStatus(e.Row) != PXEntryStatus.Inserted);
                createPrepayment.SetEnabled(PaymentsAndApplicationsEnabled && cache.GetStatus(e.Row) != PXEntryStatus.Inserted);
				Adjustments.Cache.AllowInsert = PaymentsAndApplicationsEnabled;
				Adjustments.Cache.AllowDelete = PaymentsAndApplicationsEnabled;
				Adjustments.Cache.AllowUpdate = PaymentsAndApplicationsEnabled;

				DiscountDetails.Cache.AllowDelete = true;
				DiscountDetails.Cache.AllowUpdate = true;
				DiscountDetails.Cache.AllowInsert = true;   
			}
            splits.Cache.AllowInsert = Transactions.Cache.AllowInsert;
            splits.Cache.AllowUpdate = Transactions.Cache.AllowUpdate;
            splits.Cache.AllowDelete = Transactions.Cache.AllowDelete;

            PXUIFieldAttribute.SetEnabled<SOOrder.orderType>(cache, doc);
			PXUIFieldAttribute.SetEnabled<SOOrder.orderNbr>(cache, doc);
			PXUIFieldAttribute.SetVisible<SOLine.invoiceNbr>(Transactions.Cache, null, IsCreditMemoOrder || IsRMAOrder);
			PXUIFieldAttribute.SetEnabled<SOLine.reasonCode>(Transactions.Cache, null, true);
			addInvoice.SetEnabled((IsCreditMemoOrder || IsRMAOrder) && Transactions.Cache.AllowInsert);

			Taxes.Cache.AllowDelete = Transactions.Cache.AllowDelete;
			Taxes.Cache.AllowInsert = Transactions.Cache.AllowInsert;

			PXUIFieldAttribute.SetVisible<SOLine.branchID>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyLineAmt>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyUnitPrice>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyExtCost>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyExtPrice>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.discPct>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.discAmt>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyDiscAmt>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyDiscPrice>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.manualDisc>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.discountID>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.curyUnbilledAmt>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.salesPersonID>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.taxCategoryID>(this.Transactions.Cache, null, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOLine.commissionable>(this.Transactions.Cache, null, !IsTransferOrder);


			if (soordertype.Current != null)
			{
				PXUIFieldAttribute.SetVisible<SOOrder.curyControlTotal>(cache, e.Row, soordertype.Current.RequireControlTotal == true);
			}

			if (soordertype.Current.RequireLocation == true)
			{
				PXStringListAttribute.SetList<SOLine.shipComplete>(Transactions.Cache, null, new string[] { SOShipComplete.CancelRemainder, SOShipComplete.ShipComplete }, new string[] { Messages.CancelRemainder, Messages.ShipComplete });
			}
			else
			{
				PXStringListAttribute.SetList<SOLine.shipComplete>(Transactions.Cache, null, new string[] { SOShipComplete.BackOrderAllowed, SOShipComplete.CancelRemainder, SOShipComplete.ShipComplete }, new string[] { Messages.BackOrderAllowed, Messages.CancelRemainder, Messages.ShipComplete });
			}
			PXUIFieldAttribute.SetVisible<SOOrder.destinationSiteID>(cache, e.Row, IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.customerOrderNbr>(cache, e.Row, !IsTransferOrder);

			PXUIFieldAttribute.SetVisible<SOOrder.creditHold>(cache, e.Row, ARDocType.SignBalance(soordertype.Current.ARDocType) == 1m);
			PXDefaultAttribute.SetPersistingCheck<SOOrder.destinationSiteID>(cache, e.Row,
				IsTransferOrder ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			Packages.Cache.AllowInsert = ((SOOrder)e.Row).IsManualPackage == true;
			Packages.Cache.AllowDelete = ((SOOrder)e.Row).IsManualPackage == true;
			PXUIFieldAttribute.SetEnabled<SOPackageInfo.inventoryID>(Packages.Cache, null, ((SOOrder)e.Row).IsManualPackage == true);
			PXUIFieldAttribute.SetEnabled<SOPackageInfo.boxID>(Packages.Cache, null, ((SOOrder)e.Row).IsManualPackage == true);

			if (!string.IsNullOrEmpty(((SOOrder)e.Row).ShipVia))
			{
				Carrier shipVia = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(this, ((SOOrder)e.Row).ShipVia);
				if (shipVia != null)
				{
                    PXUIFieldAttribute.SetVisible<SOPackageInfo.declaredValue>(Packages.Cache, null, shipVia.PluginMethod != null);
					PXUIFieldAttribute.SetVisible<SOPackageInfo.cOD>(Packages.Cache, null, shipVia.PluginMethod != null);
                    PXUIFieldAttribute.SetEnabled<SOOrder.curyFreightCost>(cache, doc, isFreightEditable && shipVia.CalcMethod == "M");
				}
			}
			PXUIFieldAttribute.SetEnabled<SOOrder.taxZoneID>(cache, e.Row, ((SOOrder)e.Row).OverrideTaxZone == true);

			if (!UnattendedMode)
			{
			SOShippingAddress shipAddress = this.Shipping_Address.Select();
			SOBillingAddress billingAddress = this.Billing_Address.Select();
				bool enableAddressValidation = (doc.Completed == false && doc.Cancelled == false) && ((shipAddress != null && shipAddress.IsDefaultAddress == false && shipAddress.IsValidated == false)
				                                ||(billingAddress != null && billingAddress.IsDefaultAddress == false && billingAddress.IsValidated == false));
			this.validateAddresses.SetEnabled(enableAddressValidation);
			}

			if (IsExternalTax == true && ((SOOrder)e.Row).IsTaxValid != true && !IsTransferOrder)
			{
				PXUIFieldAttribute.SetWarning<SOOrder.curyTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);
			}
			else if (IsExternalTax == true && ((SOOrder)e.Row).IsFreightTaxValid != true && !IsTransferOrder)
				PXUIFieldAttribute.SetWarning<SOOrder.curyTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);

			if (IsExternalTax == true && ((SOOrder)e.Row).IsOpenTaxValid != true && !IsTransferOrder)
				PXUIFieldAttribute.SetWarning<SOOrder.curyOpenTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);

			if (IsExternalTax == true && ((SOOrder)e.Row).IsUnbilledTaxValid != true && !IsTransferOrder)
			{
				PXUIFieldAttribute.SetWarning<SOOrder.curyUnbilledTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);
				PXUIFieldAttribute.SetWarning<SOOrder.curyUnbilledOrderTotal>(cache, e.Row, PX.Objects.SO.Messages.UnbilledBalanceWithoutTaxTaxIsNotUptodate);
			}


			Taxes.Cache.AllowInsert = !IsExternalTax;
			Taxes.Cache.AllowUpdate = !IsExternalTax;
			Taxes.Cache.AllowDelete = !IsExternalTax;

			bool isGroundCollectVisible = false;

			if (doc.ShipVia != null)
			{
				Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(this, doc.ShipVia);

				if (carrier != null && carrier.IsExternal == true && !string.IsNullOrEmpty(carrier.CarrierPluginID))
				{
					ICarrierService service = CarrierPluginMaint.CreateCarrierService(this, carrier.CarrierPluginID);
					if (service != null)
						isGroundCollectVisible = service.Attributes.Contains("COLLECT");
				}
			}
			
			PXUIFieldAttribute.SetVisible<SOOrder.groundCollect>(cache, doc, isGroundCollectVisible);

			PXUIFieldAttribute.SetVisible<SOOrder.customerID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.customerLocationID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyVatExemptTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyVatTaxableTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyTaxTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyOrderTotal>(cache, doc, !IsTransferOrder);

			PXUIFieldAttribute.SetVisible<SOOrder.taxZoneID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.overrideTaxZone>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.avalaraCustomerUsageType>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.billSeparately>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.invoiceNbr>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.invoiceDate>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.termsID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.dueDate>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.discDate>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.finPeriodID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.salesPersonID>(cache, doc, !IsTransferOrder);

			PXUIFieldAttribute.SetVisible<SOOrder.curyLineTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyMiscTot>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyDiscTot>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyFreightCost>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyFreightAmt>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyPremiumFreightAmt>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.freightTaxCategoryID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyOpenOrderTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.unbilledOrderQty>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyUnbilledOrderTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyPaymentTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyCCPreAuthAmount>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyUnpaidBalance>(cache, doc, !IsTransferOrder);

			Taxes.View.AllowSelect = !IsTransferOrder;
			SalesPerTran.View.AllowSelect = !IsTransferOrder;
			DiscountDetails.View.AllowSelect = !IsTransferOrder;
			Adjustments.View.AllowSelect = !IsTransferOrder;


			PXUIFieldAttribute.SetVisible<SOOrder.paymentMethodID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.pMInstanceID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.cashAccountID>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.extRefNbr>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.cCCardNumber>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.createPMInstance>(cache, doc, !IsTransferOrder);
			DefPaymentMethodInstance.View.AllowSelect = !IsTransferOrder;
			InputModeFilter.View.AllowSelect = !IsTransferOrder;
			DefPaymentMethodInstanceDetails.View.AllowSelect = !IsTransferOrder;
			PXUIFieldAttribute.SetVisible<SOOrder.cCPaymentStateDescr>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.pCResponseReasonText>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.preAuthTranNumber>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.cCAuthExpirationDate>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyCCPreAuthAmount>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyPaymentTotal>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyUnpaidBalance>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyCCCapturedAmt>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.captureTranNumber>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.refTranExtNbr>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.curyCCCapturedAmt>(cache, doc, !IsTransferOrder);
			PXUIFieldAttribute.SetVisible<SOOrder.captureTranNumber>(cache, doc, !IsTransferOrder);

			PXDefaultAttribute.SetPersistingCheck<SOOrder.termsID>(cache, doc, (doc.ARDocType != ARDocType.Undefined
				&& doc.ARDocType != ARDocType.CreditMemo && string.IsNullOrEmpty(doc.TermsID)) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			Billing_Contact.View.AllowSelect = !IsTransferOrder;
			Billing_Address.View.AllowSelect = !IsTransferOrder;

			PXUIFieldAttribute.SetVisible<SOOrder.approved>(cache, doc, !(bool)doc.DontApprove);
			Approval.AllowSelect = !(bool)doc.DontApprove;
			if (doc.Hold != true && doc.DontApprove != true)
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<SOOrder.orderType>(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<SOOrder.orderNbr>(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<SOOrder.hold>(cache, doc, true);
                PXUIFieldAttribute.SetEnabled<SOOrder.creditHold>(cache, doc, true);

                Transactions.Cache.AllowDelete = false;
                Transactions.Cache.AllowUpdate = false;
                Transactions.Cache.AllowInsert = false;

                splits.Cache.AllowDelete = false;
                splits.Cache.AllowUpdate = false;
                splits.Cache.AllowInsert = false;

                Adjustments.Cache.AllowInsert = false;
                Adjustments.Cache.AllowUpdate = false;
                Adjustments.Cache.AllowDelete = false;

                DiscountDetails.Cache.AllowDelete = false;
                DiscountDetails.Cache.AllowUpdate = false;
                DiscountDetails.Cache.AllowInsert = false;

                Taxes.Cache.AllowInsert = false;
                Taxes.Cache.AllowUpdate = false;
                Taxes.Cache.AllowDelete = false;

                bool PaymentsAndApplicationsEnabled = soordertype.Current.CanHaveApplications;
                createPayment.SetEnabled(!(doc == null || doc.Completed == true || doc.Cancelled == true || !allowAllocation) && PaymentsAndApplicationsEnabled && (doc.Approved ?? false));
                createPrepayment.SetEnabled(!(doc == null || doc.Completed == true || doc.Cancelled == true || !allowAllocation) && PaymentsAndApplicationsEnabled && (doc.Approved ?? false));
            }

            addInvBySite.SetEnabled(Transactions.Cache.AllowInsert);
            
            if (soordertype.Current == null || soordertype.Current.Active != true)
            {
                SetReadOnly(true);
                cache.AllowInsert = true;
                cache.RaiseExceptionHandling<SOOrder.orderType>(doc, doc.OrderType, new PXSetPropertyException(Messages.OrderTypeInactive, PXErrorLevel.Warning));
            }

            if (doc != null && IsTransferOrder)
            {
                var destinationSiteIdErrorString = PXUIFieldAttribute.GetError<SOOrder.destinationSiteID>(cache, e.Row);
                if (destinationSiteIdErrorString == null)
                {
                    var destinationSiteIdErrorMessage = doc.DestinationSiteIdErrorMessage;
                    if (!string.IsNullOrWhiteSpace(destinationSiteIdErrorMessage))
                    {
                        if (UnattendedMode)
                            throw new PXSetPropertyException(destinationSiteIdErrorMessage, PXErrorLevel.Error);
                        else
                            cache.RaiseExceptionHandling<POOrder.siteID>(e.Row, cache.GetValueExt<POOrder.siteID>(e.Row), new PXSetPropertyException(destinationSiteIdErrorMessage, PXErrorLevel.Error));
                    }
                }
            }
        }

        protected virtual void SOOrder_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (e.Row != null && ((SOOrder)e.Row).ShipmentCntr > 0)
			{
				throw new PXSetPropertyException(Messages.BackOrderCannotBeDeleted);
			}		

			if (this.Adjustments.Select().Count > 0 && Document.Ask(Messages.Warning, Messages.SalesOrderWillBeDeleted, MessageButtons.OKCancel) != WebDialogResult.OK  ) 
			{
				e.Cancel = true;
			}
		}

		protected virtual void SOOrder_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.Row, -((SOOrder)e.Row).UnbilledOrderTotal, -((SOOrder)e.Row).OpenOrderTotal);

			SOOrder quoteorder = PXSelect<SOOrder, Where<SOOrder.orderType, Equal<Current<SOOrder.origOrderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrder.origOrderNbr>>>>>.SelectSingleBound(this, new object[] { e.Row });
			if (quoteorder != null && quoteorder.Behavior == SOOrderTypeConstants.QuoteOrder)
			{
				quoteorder.Completed = false;
				Document.Cache.SetStatus(quoteorder, PXEntryStatus.Updated);
			}
		}

		public virtual void UpdateControlTotal(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;

			if (row.Completed == false)
			{
				if (soordertype.Current.RequireControlTotal == false)
				{
					if (row.CuryOrderTotal != row.CuryControlTotal)
					{
						if (row.CuryOrderTotal != null && row.CuryOrderTotal != 0)
							sender.SetValueExt<SOOrder.curyControlTotal>(e.Row, row.CuryOrderTotal);
						else
							sender.SetValueExt<SOOrder.curyControlTotal>(e.Row, 0m);
					}
				}
			}
		}

		public virtual void ValidateControlTotal(PXCache sender, PXRowSelectedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row == null) return;

			bool isex = false;

			if (row.Hold == false && row.Completed == false)
			{
				if (soordertype.Current.RequireControlTotal == true)
				{
					if (row.CuryOrderTotal != row.CuryControlTotal)
					{
						sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, row.CuryControlTotal, new PXSetPropertyException(Messages.DocumentOutOfBalance));
						isex = true;
					}
					else if (row.CuryOrderTotal < 0m && row.ARDocType != ARDocType.NoUpdate && row.Behavior != SOBehavior.RM)
					{
						sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, row.CuryControlTotal, new PXSetPropertyException(Messages.DocumentBalanceNegative));
						isex = true;
					}
				}
				else 
				{
					if (row.CuryOrderTotal < 0m && row.ARDocType != ARDocType.NoUpdate && row.Behavior != SOBehavior.RM)
					{
						sender.RaiseExceptionHandling<SOOrder.curyOrderTotal>(e.Row, row.CuryOrderTotal, new PXSetPropertyException(Messages.DocumentBalanceNegative));
						isex = true;
					}
				}
			}

			if (!isex)
			{
				if (soordertype.Current.RequireControlTotal == true)
				{
					sender.RaiseExceptionHandling<SOOrder.curyControlTotal>(e.Row, null, null);
				}
				else
				{
					sender.RaiseExceptionHandling<SOOrder.curyOrderTotal>(e.Row, null, null);
				}
			}
		}

		protected virtual void SOOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			SOOrder oldRow = e.OldRow as SOOrder;
			if (row == null) return;

			if (e.ExternalCall && (!sender.ObjectsEqual<SOOrder.orderDate>(e.OldRow, e.Row)))
			{
				DiscountEngine<SOLine>.AutoRecalculatePricesAndDiscounts<SOOrderDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, row.CustomerLocationID, row.OrderDate);
			}

			if (e.ExternalCall && sender.GetStatus(row) != PXEntryStatus.Deleted && !sender.ObjectsEqual<SOOrder.curyDiscTot>(e.OldRow, e.Row))
			{
				DiscountEngine<SOLine>.CalculateDocumentDiscountRate<SOOrderDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, (Document.Current.CuryLineTotal ?? 0m) + (Document.Current.MiscTot ?? 0m), Document.Current.CuryDiscTot ?? 0m);
			}

			UpdateControlTotal(sender, e);

			if (row.CustomerID != null && row.CuryDiscTot != null && row.CuryDiscTot > 0 && row.CuryLineTotal != null && row.CuryMiscTot != null && (row.CuryLineTotal > 0 || row.CuryMiscTot > 0))
			{
				decimal discountLimit = DiscountEngine.GetDiscountLimit(sender, row.CustomerID);
				if (((row.CuryLineTotal + row.CuryMiscTot) / 100 * discountLimit) < row.CuryDiscTot)
					PXUIFieldAttribute.SetWarning<SOOrder.curyDiscTot>(sender, row, PXMessages.LocalizeFormatNoPrefix(AR.Messages.DocDiscountExceedLimit, discountLimit));
			}

			if (!sender.ObjectsEqual<SOOrder.isCCAuthorized>(e.Row, e.OldRow) && row.IsCCAuthorized == true ||
				!sender.ObjectsEqual<SOOrder.isCCCaptured>(e.Row, e.OldRow) && row.IsCCCaptured == true)
			{
				List<object> records = new List<object>();
				records.Add(e.Row);
				PXView dummyView = new DummyView(this, Document.View.BqlSelect, records);
				foreach (object item in creditHold.Press(new PXAdapter(dummyView))) { ; }
			}

            if (e.OldRow != null)
            {
                ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.OldRow, -((SOOrder)e.OldRow).UnbilledOrderTotal, -((SOOrder)e.OldRow).OpenOrderTotal);
            }
            ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.Row, row.UnbilledOrderTotal, row.OpenOrderTotal);

            if (!sender.ObjectsEqual<SOOrder.lineTotal, SOOrder.orderWeight, SOOrder.packageWeight, SOOrder.orderVolume, SOOrder.shipTermsID, SOOrder.shipZoneID, SOOrder.shipVia, SOOrder.useCustomerAccount>(e.OldRow, e.Row) || !sender.ObjectsEqual<SOOrder.curyFreightCost>(e.OldRow, e.Row))
			{
				Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Current<SOOrder.shipVia>>>>.Select(sender.Graph);
                if (!sender.ObjectsEqual<SOOrder.shipVia>(e.OldRow, e.Row) && carrier != null && carrier.CalcMethod == "M")
                {
                    row.FreightCost = 0m;
                }
                if (carrier != null && carrier.IsExternal == true)
				{
					row.FreightCostIsValid = false;
				}
				else
				{
					if (!(soordertype.Current != null && soordertype.Current.CalculateFreight == false))
					{
                        PXResultset<SOLine> res = Transactions.Select();
						FreightCalculator fc = CreateFreightCalculator();
						fc.CalcFreight<SOOrder, SOOrder.curyFreightCost, SOOrder.curyFreightAmt>(sender, (SOOrder) e.Row, res.Count);
						row.FreightCostIsValid = true;
					}
				}
			}
			
			if (row.IsManualPackage != true &&
				( row.OrderWeight != oldRow.OrderWeight || 
				(row.ShipVia != oldRow.ShipVia && !string.IsNullOrEmpty(oldRow.ShipVia))))
			{
				row.IsPackageValid = false;
			}

				//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
				if (row != null && IsExternalTax)
			{
				if (!sender.ObjectsEqual<SOOrder.avalaraCustomerUsageType, SOOrder.orderDate, SOOrder.taxZoneID, SOOrder.customerID>(e.Row, e.OldRow))
				{
					row.IsTaxValid = false;
					row.IsOpenTaxValid = false;
					row.IsUnbilledTaxValid = false;
				}

				if (!sender.ObjectsEqual<SOLine.openAmt>(e.Row, e.OldRow))
				{
					row.IsOpenTaxValid = false;
				}

				if (!sender.ObjectsEqual<SOLine.unbilledAmt>(e.Row, e.OldRow))
				{
					row.IsUnbilledTaxValid = false;
				}
			}

			if (!sender.ObjectsEqual<SOOrder.curyFreightTot, SOOrder.freightTaxCategoryID>(e.OldRow, e.Row))
			{
				if ( IsExternalTax )
				{
					row.IsFreightTaxValid = false;
					row.IsTaxValid = false;
					row.IsOpenTaxValid = false;
					row.IsUnbilledTaxValid = false;
				}

				SOOrderTaxAttribute.Calculate<SOOrder.freightTaxCategoryID>(sender, e);
			}
			if (!sender.ObjectsEqual<SOOrder.hold>(e.Row, e.OldRow) && row.Hold != true)
			{
				if(soordertype.Current.RequireShipping == true && soordertype.Current.ARDocType != ARDocType.NoUpdate)
					foreach (SOLine line in Transactions.Select())
					{
						if ((line.SalesAcctID == null || line.SalesSubID == null ) && 
							Transactions.Cache.GetStatus(line) == PXEntryStatus.Notchanged)
							Transactions.Cache.SetStatus(line, PXEntryStatus.Updated);
						
						PXDefaultAttribute.SetPersistingCheck<SOLine.salesAcctID>(Transactions.Cache, line,PXPersistingCheck.NullOrBlank);

						PXDefaultAttribute.SetPersistingCheck<SOLine.salesSubID>(Transactions.Cache, line, PXPersistingCheck.NullOrBlank);
					}
			}

			if (!sender.ObjectsEqual<SOOrder.customerLocationID, SOOrder.orderDate>(e.Row, e.OldRow))
			{
				WebDialogResult old_answer = Document.View.Answer;

				Document.View.Answer = WebDialogResult.None;
				try
				{
					RecalcDiscountsParamFilter recalcFilter = new RecalcDiscountsParamFilter();
					recalcFilter.OverrideManualDiscounts = false;
					recalcFilter.OverrideManualPrices = false;
					recalcFilter.RecalcDiscounts = true;
					recalcFilter.RecalcUnitPrices = true;
					recalcFilter.RecalcTarget = RecalcDiscountsParamFilter.AllLines;
					DiscountEngine<SOLine>.RecalculatePricesAndDiscounts<SOOrderDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, DiscountDetails, Document.Current.CustomerLocationID, Document.Current.OrderDate, recalcFilter);
					RecalculateTotalDiscount();
				}
				finally
				{
					Document.View.Answer = old_answer;
				}
			}

			if (!sender.ObjectsEqual<SOOrder.completed, SOOrder.cancelled>(e.Row, e.OldRow) && (row.Completed == true && row.BilledCntr == 0 && row.ShipmentCntr <= row.BilledCntr + row.ReleasedCntr || row.Cancelled == true))
			{
				foreach (SOAdjust adj in Adjustments_Raw.Select())
				{
					SOAdjust copy = PXCache<SOAdjust>.CreateCopy(adj);
					copy.CuryAdjdAmt = 0m;
					copy.CuryAdjgAmt = 0m;
					copy.AdjAmt = 0m;
					Adjustments.Update(copy);
				}
			}

			if (row != null && this.sooperation.Current.INDocType == INTranType.Transfer && oldRow.Cancelled != true && row.Cancelled == true)
			{
				foreach (SOLine line in Transactions.Select())
				{
					ClearReplenishmentPlans(line);
				}
			}
		}

		protected virtual void SOOrder_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SOOrder row = (SOOrder)e.Row;
			if (row == null) return;
			ARReleaseProcess.UpdateARBalances(this, (SOOrder)e.Row, ((SOOrder)e.Row).UnbilledOrderTotal, ((SOOrder)e.Row).OpenOrderTotal);
			if (!PXAccess.FeatureInstalled<FeaturesSet.autoPackaging>())
			{
				row.IsManualPackage = true;
			}
		}

		//protected virtual void SOOrder_CuryDiscTot_FieldVerified(PXCache sender, PXFieldVerifyingEventArgs e)
		//{
		//    SOOrder row = e.Row as SOOrder;
		//    if (row.CuryLineTotal >= 0m && row.CuryLineTotal < Convert.ToDecimal(e.NewValue))
		//    {
		//        sender.RaiseExceptionHandling<SOOrder.curyDiscTot>(row, e.NewValue, new PXException(CS.Messages.Entry_LE, row.CuryLineTotal.ToString()));
		//    }

		//    if (row.CuryLineTotal < 0m && row.CuryLineTotal > Convert.ToDecimal(e.NewValue))
		//    {
		//        sender.RaiseExceptionHandling<SOOrder.curyDiscTot>(row, e.NewValue, new PXException(CS.Messages.Entry_GE, row.CuryLineTotal.ToString()));
		//    }
		//}

		protected virtual void SOOrder_CreatePMInstance_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;

			if (row != null && row.CreatePMInstance != (bool)e.OldValue)
			{
				bool needUpdate = false;
				int? oldID = row.PMInstanceID;
				if (row.CreatePMInstance == true)
				{
					CustomerPaymentMethod res = this.CreateDefPaymentMethod(row);
					row.PMInstanceID = res.PMInstanceID;
					needUpdate = true;
				}
				else
				{
					this.RemoveInsertedPMInstance();
					row.PMInstanceID = null;
					needUpdate = true;
				}
				if (needUpdate)
				{
					sender.RaiseFieldUpdated<SOOrder.pMInstanceID>(row, oldID);
					this.DefPaymentMethodInstance.View.RequestRefresh();
				}
			}
		}

		protected virtual void SOOrder_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null && row.CreatePMInstance == true)
			{
				CustomerPaymentMethodC pmInstance = (CustomerPaymentMethodC)this.DefPaymentMethodInstance.Current;
				if (pmInstance != null)
				{
					this.DefPaymentMethodInstance.SetValueExt<CustomerPaymentMethodC.cashAccountID>(pmInstance, row.CashAccountID);
				}
			}
		}
		
		protected virtual void SOOrder_PreAuthTranNumber_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOOrder row = e.Row as SOOrder;
			if (row != null && !row.CCAuthTranNbr.HasValue && !String.IsNullOrEmpty(row.PreAuthTranNumber))
			{
				Decimal amount = row.CuryOrderTotal.Value;
				sender.SetValue<SOOrder.curyCCPreAuthAmount>(e.Row, amount);
				sender.SetValuePending<SOOrder.curyCCPreAuthAmount>(e.Row, amount);
			}
		}

		#endregion

		#region Default Payment Method Events

		protected virtual void CustomerPaymentMethodC_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CustomerPaymentMethod row = (CustomerPaymentMethod)e.Row;
			if (row != null)
			{
				SOOrder doc = this.Document.Current;
				if (doc != null && doc.PMInstanceID == row.PMInstanceID)
				{
					bool isInserted = (cache.GetStatus(e.Row) == PXEntryStatus.Inserted);
					PXUIFieldAttribute.SetEnabled(cache, e.Row, isInserted); //Allow Edit new record only
					PXUIFieldAttribute.SetEnabled(this.DefPaymentMethodInstanceDetails.Cache, null, isInserted);
					PXUIFieldAttribute.SetEnabled<CustomerPaymentMethodC.paymentMethodID>(cache, e.Row, (isInserted || String.IsNullOrEmpty(row.PaymentMethodID)));
					PXUIFieldAttribute.SetEnabled<CustomerPaymentMethodC.descr>(cache, row, false);
					PXUIFieldAttribute.SetVisible<CustomerPaymentMethodC.cCProcessingCenterID>(cache, row, doc.CreatePMInstance == true);
					if (!String.IsNullOrEmpty(row.PaymentMethodID))
					{

						PaymentMethod pmDef = (PaymentMethod)this.PaymentMethodDef.Select();
						bool singleInstance = pmDef.ARIsOnePerCustomer ?? false;
						bool isIDMaskExists = false;
						if (!singleInstance)
						{

							foreach (PaymentMethodDetail iDef in this.PMDetails.Select(row.PaymentMethodID))
							{
								if ((iDef.IsIdentifier ?? false) && (!string.IsNullOrEmpty(iDef.DisplayMask)))
								{
									isIDMaskExists = true;
									break;
								}
							}
						}
						if (!(isIDMaskExists || singleInstance))
						{
							PXUIFieldAttribute.SetEnabled<CustomerPaymentMethodC.descr>(cache, row, true);
						}
					}
					if (!isInserted && (!String.IsNullOrEmpty(row.PaymentMethodID)))
					{
						CCProcTran ccTran = PXSelect<CCProcTran, Where<CCProcTran.pMInstanceID, Equal<Required<CCProcTran.pMInstanceID>>,
									And<CCProcTran.procStatus, Equal<CCProcStatus.finalized>,
									And<CCProcTran.tranStatus, Equal<CCTranStatusCode.approved>>>>>.Select(this, row.PMInstanceID);
						bool hasTransactions = (ccTran != null);
						this.DefPaymentMethodInstanceDetails.Cache.AllowDelete = !hasTransactions;
					}
				}

			}
		}

		protected virtual void CustomerPaymentMethodC_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (CurrentDocument.Current.CreatePMInstance != true)
				this.DefPaymentMethodInstance.Cache.Remove((CustomerPaymentMethodC)e.Row);
		}

		protected virtual void CustomerPaymentMethodC_PaymentMethodID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e) 
		{
			if (e.Row != null) 
			{
				SOOrder order = this.Document.Current;
				if(order!= null)
				{
					e.NewValue = order.PaymentMethodID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CustomerPaymentMethodC_CashAccountID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				SOOrder order = this.Document.Current;
				if (order != null)
				{
					e.NewValue = order.CashAccountID;                    
				}
			}
		}

		protected virtual void CustomerPaymentMethodC_PaymentMethodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CustomerPaymentMethod row = (CustomerPaymentMethod)e.Row;            
			this.ClearPMDetails();
			this.AddPMDetails();
			row.CashAccountID = null;
			cache.SetDefaultExt<CustomerPaymentMethodC.cashAccountID>(e.Row);
			PaymentMethod pmDef = this.PaymentMethodDef.Select();
			if (pmDef.ARIsOnePerCustomer ?? false)
			{
				row.Descr = pmDef.Descr;
			}
			this.DefPaymentMethodInstanceDetails.View.RequestRefresh();			
		}

		protected virtual void CustomerPaymentMethodC_Descr_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			CustomerPaymentMethod row = (CustomerPaymentMethod)e.Row;
			if (row!= null && row.PaymentMethodID != null)
			{
				PaymentMethod pmDef = this.PaymentMethodDef.Select(row.PaymentMethodID);
				if (pmDef != null)
				if(pmDef.ARIsOnePerCustomer ?? false)
				{
					row.Descr = pmDef.Descr;
				}               
			}
		}

		protected virtual void CustomerPaymentMethodC_Descr_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CustomerPaymentMethod row = (CustomerPaymentMethod)e.Row;
			PaymentMethod def = this.PaymentMethodDef.Select(row.PaymentMethodID);
			if (!(def.ARIsOnePerCustomer ?? false))
			{
				CustomerPaymentMethod existing = PXSelect<CustomerPaymentMethod,
				Where<CustomerPaymentMethod.bAccountID, Equal<Required<CustomerPaymentMethod.bAccountID>>,
				And<CustomerPaymentMethod.paymentMethodID, Equal<Required<CustomerPaymentMethod.paymentMethodID>>,
				And<CustomerPaymentMethod.pMInstanceID, NotEqual<Required<CustomerPaymentMethod.pMInstanceID>>,
				And<CustomerPaymentMethod.descr, Equal<Required<CustomerPaymentMethod.descr>>,
				And<CustomerPaymentMethod.isActive, Equal<True>>>>>>>.Select(this, row.BAccountID, row.PaymentMethodID, row.PMInstanceID, row.Descr);
				if (existing != null)
				{
					cache.RaiseExceptionHandling<CustomerPaymentMethodC.descr>(row, row.Descr, new PXSetPropertyException(AR.Messages.CustomerPMInstanceHasDuplicatedDescription, PXErrorLevel.Warning));
				}
			}
		}

		protected virtual void CustomerPaymentMethodC_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			CustomerPaymentMethod row = (CustomerPaymentMethod)e.Row;
			if (row == null) return;
			if (!string.IsNullOrEmpty(row.CustomerCCPID))
			{
				CustomerProcessingCenterID test = CustomerProcessingID.Select(row.CCProcessingCenterID, row.CustomerCCPID);
				if (test == null)
				{
					CustomerProcessingCenterID cPCID = new CustomerProcessingCenterID();
					cPCID.BAccountID = row.BAccountID;
					cPCID.CCProcessingCenterID = row.CCProcessingCenterID;
					cPCID.CustomerCCPID = row.CustomerCCPID;
					CustomerProcessingID.Insert(cPCID);
				}
			}
		}

		#region PM Details Events

		protected virtual void CustomerPaymentMethodDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CustomerPaymentMethodDetail row = (CustomerPaymentMethodDetail)e.Row;
			if (row != null)
			{
				bool isPMInserted = this.DefPaymentMethodInstance.Cache.GetStatus(this.DefPaymentMethodInstance.Current) == PXEntryStatus.Inserted;
				PaymentMethodDetail iTempl = this.FindTemplate(row);
				bool isRequired = (iTempl != null) && (iTempl.IsRequired ?? false);
				PXDefaultAttribute.SetPersistingCheck<CustomerPaymentMethodDetail.value>(cache, row, (isRequired) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXRSACryptStringAttribute.SetDecrypted<CustomerPaymentMethodDetail.value>(cache, row, !(iTempl.IsEncrypted ?? false));
				if (iTempl.IsExpirationDate == true)
				{
					PXUIFieldAttribute.SetEnabled<CustomerPaymentMethodDetail.value>(cache, row, isPMInserted);
				}
			}		
		}

		protected virtual void CustomerPaymentMethodDetail_Value_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CustomerPaymentMethodDetail row = e.Row as CustomerPaymentMethodDetail;
			PaymentMethodDetail def = this.FindTemplate(row);
			if (def != null)
			{
				if (def.IsIdentifier ?? false)
				{
					string id = CustomerPaymentMethodMaint.IDObfuscator.MaskID(row.Value, def.EntryMask, def.DisplayMask);
					if (this.DefPaymentMethodInstance.Current.Descr != id)
					{
						CustomerPaymentMethodC parent = this.DefPaymentMethodInstance.Current;
						parent.Descr = String.Format("{0}:{1}", parent.PaymentMethodID, id);
						this.DefPaymentMethodInstance.Update(parent);
					}
				}
				if (def.IsExpirationDate ?? false)
				{
					CustomerPaymentMethodC parent = this.DefPaymentMethodInstance.Current;
					parent.ExpirationDate = CustomerPaymentMethodMaint.ParseExpiryDate(this, parent, row.Value);
					this.DefPaymentMethodInstance.Update(parent);
				}
				if (CCProcessingUtils.isTokenizedPaymentMethod(this, row.PMInstanceID) && (def.IsCCProcessingID ?? false))
				{
					CCCustomerInformationManager.SyncNewPMI(this, DefPaymentMethodInstance, DefPaymentMethodInstanceDetailsAll);
				}
			}
		}

		protected virtual void CustomerPaymentMethodDetail_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			CustomerPaymentMethodDetail row = (CustomerPaymentMethodDetail)e.Row;
			PaymentMethodDetail def = this.FindTemplate(row);
			if (def.IsIdentifier ?? false)
			{
				this.DefPaymentMethodInstance.Current.Descr = null;
			}
		}

		#endregion

		protected virtual void AddPMDetails()
		{
			string pmID = this.DefPaymentMethodInstance.Current.PaymentMethodID;
			if (!String.IsNullOrEmpty(pmID))
			{
				foreach (PaymentMethodDetail it in this.PMDetails.Select())
				{
					CustomerPaymentMethodDetail det = new CustomerPaymentMethodDetail();
					det.DetailID = it.DetailID;
					det = this.DefPaymentMethodInstanceDetails.Insert(det);
				}
			}
		}
		protected virtual void ClearPMDetails()
		{
			foreach (CustomerPaymentMethodDetail iDet in this.DefPaymentMethodInstanceDetails.Select())
			{
				this.DefPaymentMethodInstanceDetails.Delete(iDet);
			}
		}
		protected virtual PaymentMethodDetail FindTemplate(CustomerPaymentMethodDetail aDet)
		{
			PaymentMethodDetail res = PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Required<PaymentMethodDetail.paymentMethodID>>,
						And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,     
						And<PaymentMethodDetail.detailID, Equal<Required<PaymentMethodDetail.detailID>>>>>>.Select(this, aDet.PaymentMethodID, aDet.DetailID);
			return res;
		}

		protected virtual CustomerPaymentMethod CreateDefPaymentMethod(SOOrder doc)
		{
			CustomerPaymentMethodC pmInstance = new CustomerPaymentMethodC();
			CustomerPaymentMethodC currentPM = this.DefPaymentMethodInstance.Current;
			if (currentPM != null)
			{
				RemoveInsertedPMInstance();
			}            
			pmInstance = this.DefPaymentMethodInstance.Insert(pmInstance);
			DefPaymentMethodInstance.Cache.SetDefaultExt<CustomerPaymentMethodC.cCProcessingCenterID>(pmInstance);
			if (pmInstance.BAccountID == null)
			{
				pmInstance.BAccountID = doc.CustomerID;
				
			}
			this.AddPMDetails();
			this.DefPaymentMethodInstance.Current = pmInstance;
			
			if (!String.IsNullOrEmpty(doc.CCCardNumber))
			{
				CustomerPaymentMethodDetail idDetail = null;
				foreach (PXResult<CustomerPaymentMethodDetail, PaymentMethodDetail> iDet in this.DefPaymentMethodInstanceDetails.Select(pmInstance.PMInstanceID))
				{
					if (((PaymentMethodDetail)iDet).IsIdentifier == true)
					{
						idDetail = iDet;
					}
				}
				if (idDetail != null)
				{
					this.DefPaymentMethodInstanceDetails.Cache.SetValueExt<CustomerPaymentMethodDetail.value>(idDetail, doc.CCCardNumber);
					this.Document.Cache.SetValueExt<SOOrder.cCCardNumber>(doc, string.Empty);
				}
			}


			return pmInstance;
		}

		protected virtual void RemoveInsertedPMInstance()
		{
			foreach (CustomerPaymentMethodC currentPM in this.DefPaymentMethodInstance.Select())
			{
				if (this.DefPaymentMethodInstance.Cache.GetStatus(currentPM) == PXEntryStatus.Inserted)
				{
					this.DefPaymentMethodInstance.Delete(currentPM);
					foreach (CustomerPaymentMethodDetail iDet in this.DefPaymentMethodInstanceDetails.Select())
					{
						this.DefPaymentMethodInstanceDetails.Delete(iDet);
					}
				}
			}
		}
		#endregion

		#region SOLine events

		protected object GetValue<Field>(object data)
			where Field : IBqlField
		{
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

		[PXBool]
		[DRTerms.Dates(typeof(SOLine.dRTermStartDate), typeof(SOLine.dRTermEndDate), typeof(SOLine.inventoryID), VerifyDatesPresent = false)]
		protected virtual void SOLine_ItemRequiresTerms_CacheAttached(PXCache sender) { }

		protected virtual void SOLine_SiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null && Document.Current.DefaultSiteID != null)
			{
				e.NewValue = Document.Current.DefaultSiteID;
				e.Cancel = true;
				return;
			}

			SOLine line = (SOLine) e.Row;
			if(line == null) return;
			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.stkItem, Equal<False>, And<InventoryItem.inventoryID, Equal<Current<SOLine.inventoryID>>>>>.SelectSingleBound(this, new object[] { line });
			if(item != null)
			{
				e.NewValue = item.DfltSiteID;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if (row == null) return;

			object oldValue = sender.GetValue<SOLine.inventoryID>(row);
				if (oldValue != null)
				{
				if (row.InvoiceNbr != null)
				{
					e.NewValue = oldValue;
				}
				else
				{
					foreach (SOLineSplit split in splits.Select())
					{
						if (split.Completed == true)
						{
							e.NewValue = oldValue;
							sender.RaiseExceptionHandling<SOLine.inventoryID>(row, oldValue, new PXSetPropertyException(PXMessages.LocalizeFormatNoPrefixNLA(Messages.InventoryIDCannotBeChanged, split.SplitLineNbr.ToString()), PXErrorLevel.Warning));
						}
					}
				}
			}
		}
		protected virtual void SOLine_POCreate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row == null) return;

			bool? newVal = (bool?) e.NewValue;
			POCreateVerifyValue(sender, row, newVal);
		}

		public virtual void POCreateVerifyValue(PXCache sender, SOLine row, bool? value)
		{
			if (row == null) return;
						
			if (row.InventoryID != null && value == true)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
				if (item != null && item.StkItem != true)
				{
					if (item.KitItem == true)
					{
						throw new PXSetPropertyException(Messages.SOPOLinkNotForNonStockKit);
					}
					else if (item.NonStockShip != true || item.NonStockReceipt != true)
					{
						sender.RaiseExceptionHandling<SOLine.pOCreate>(row, value, new PXSetPropertyException(Messages.NonStockShipReceiptIsOff, PXErrorLevel.Warning));
					}
				}
			}
		}

		protected virtual void SOLine_SalesAcctID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null && row.ProjectID != null && !PM.ProjectDefaultAttribute.IsNonProject(this, row.ProjectID) && row.TaskID != null)
			{
				Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, e.NewValue);
				if (account != null && account.AccountGroupID == null)
				{
					sender.RaiseExceptionHandling<SOLine.salesAcctID>(e.Row, account.AccountCD, new PXSetPropertyException(PM.Messages.NoAccountGroup, PXErrorLevel.Warning, account.AccountCD));
				}
			}
		}


		protected virtual void SOLine_SalesAcctID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null && row.TaskID == null)
			{
				sender.SetDefaultExt<SOLine.taskID>(e.Row);
			}
		}


		protected virtual void SOLine_SubItemID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((SOLine)e.Row).InvoiceNbr != null)
			{
				object oldValue = sender.GetValue<SOLine.subItemID>(e.Row);
				if (oldValue != null)
				{
					e.NewValue = oldValue;
				}
			}
		}

        protected virtual void SOLine_SubItemID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            SOLine line = (SOLine)e.Row;
            if (line != null && line.InvoiceNbr != null)
            {
                e.Cancel = true;
			}
		}

		protected virtual void SOLine_UOM_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (((SOLine)e.Row).InvoiceNbr != null)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}
		
		protected virtual void SOLine_UOM_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((SOLine)e.Row).InvoiceNbr != null)
			{
				object oldValue = sender.GetValue<SOLine.uOM>(e.Row);
				if (oldValue != null)
				{
					e.NewValue = oldValue;
				}
			}
		}

		protected virtual void SOLine_SalesAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine line = (SOLine)e.Row;

			if (line != null && IsTransferOrder == false)
			{
				InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<SOLine.inventoryID>(sender, e.Row);
				INSite site = (INSite)PXSelectorAttribute.Select<SOLine.siteID>(sender, e.Row);
				ReasonCode reasoncode = (ReasonCode)PXSelectorAttribute.Select<SOLine.reasonCode>(sender, e.Row);
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

				switch (soordertype.Current.SalesAcctDefault)
				{
					case SOSalesAcctSubDefault.MaskItem:
						e.NewValue = GetValue<InventoryItem.salesAcctID>(item);
						e.Cancel = true;
						break;
					case SOSalesAcctSubDefault.MaskSite:
						if (site != null)
						{
							e.NewValue = GetValue<INSite.salesAcctID>(site);
							e.Cancel = true;
						}
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

		protected virtual void SOLine_SalesSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine line = (SOLine)e.Row;

			if (line != null && IsTransferOrder == false && line.SalesAcctID != null)
			{
				InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<SOLine.inventoryID>(sender, e.Row);
				INSite site = (INSite)PXSelectorAttribute.Select<SOLine.siteID>(sender, e.Row);
				ReasonCode reasoncode = (ReasonCode)PXSelectorAttribute.Select<SOLine.reasonCode>(sender, e.Row);
				SalesPerson salesperson = (SalesPerson)PXSelectorAttribute.Select<SOLine.salesPersonID>(sender, e.Row);
				INPostClass postclass = new INPostClass();
				if (item != null)
				{
					postclass = PXSelectReadonly<INPostClass, Where<INPostClass.postClassID, Equal<Required<INPostClass.postClassID>>>>.Select(this, item.PostClassID);
				} 
				EPEmployee employee = (EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Current<SOOrder.ownerID>>>>.Select(this);
				CRLocation companyloc =
					PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>, Where<Branch.branchID, Equal<Required<SOLine.branchID>>>>.Select(this, line.BranchID);
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
					value = SOSalesSubAccountMaskAttribute.MakeSub<SOOrderType.salesSubMask>(this, soordertype.Current.SalesSubMask,
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

					sender.RaiseFieldUpdating<SOLine.salesSubID>(line, ref value);
				}
				catch (PXMaskArgumentException ex)
				{
					sender.RaiseExceptionHandling<SOLine.salesSubID>(e.Row, null, new PXSetPropertyException(ex.Message));
					value = null;
				}
				catch (PXSetPropertyException ex)
				{
					sender.RaiseExceptionHandling<SOLine.salesSubID>(e.Row, value, ex);
					value = null;
				}

				e.NewValue = (int?)value;
				e.Cancel = true;
			}
		}

		//protected virtual void SOLine_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		//{
		//    if (TaxAttribute.GetTaxCalc<SOLine.taxCategoryID>(sender, e.Row) == TaxCalc.Calc && taxzone.Current != null && !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) && ((SOLine)e.Row).InventoryID == null)
		//    {
		//        e.NewValue = taxzone.Current.DfltTaxCategoryID;
		//    }
		//}

		protected virtual void SOLine_Completed_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && ((SOLine)e.Row).LineType == SOLineType.MiscCharge)
			{
				e.NewValue = true;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_ShipComplete_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null)
			{
				if (location.Current != null && !string.IsNullOrEmpty(location.Current.CShipComplete))
				{
					e.NewValue = location.Current.CShipComplete;
				}
				else
				{
					e.NewValue = SOShipComplete.CancelRemainder;
				}

				if ((string)e.NewValue == SOShipComplete.BackOrderAllowed && soordertype.Current != null && soordertype.Current.RequireLocation == true)
				{
					e.NewValue = SOShipComplete.CancelRemainder;
				}
			}
		}

		protected virtual void SOLine_VendorID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = e.Row as SOLine;

			if (!(soordertype.Current.RequireShipping == true && soordertype.Current.RequireLocation == false && row != null && row.TranType != INDocType.Undefined))
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if(row != null && row.RequireLocation != true)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}
		protected virtual void SOLineSplit_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLineSplit row = (SOLineSplit)e.Row;
			if (row != null && row.RequireLocation != true)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_Completed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if ((bool?)sender.GetValue<SOLine.completed>(e.Row) != true)
			{
				sender.SetValueExt<SOLine.closedQty>(e.Row, sender.GetValue<SOLine.shippedQty>(e.Row));
			}
		}

		protected virtual void SOLine_TranType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOLine.pOCreate>(e.Row);
		}

		protected virtual void SOLine_POCreate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = e.Row as SOLine;

			if (!(soordertype.Current.RequireShipping == true && soordertype.Current.RequireLocation == false && soordertype.Current.RequireAllocation == false && row != null && row.TranType != INDocType.Undefined && row.Operation == SOOperation.Issue))
			{
				e.NewValue = false;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_POSiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if (row != null)
			{
				INItemSiteSettings itemSettings = PXSelect<INItemSiteSettings, Where<INItemSiteSettings.inventoryID, Equal<Current<SOLine.inventoryID>>, And<INItemSiteSettings.siteID, Equal<Current<SOLine.siteID>>>>>.SelectWindowed(this, 0, 1);
				object newVal = null;
				if (itemSettings != null)
				{
					if (itemSettings.ReplenishmentSource == INReplenishmentSource.Purchased ||
						itemSettings.ReplenishmentSource == INReplenishmentSource.PurchaseToOrder ||
						itemSettings.ReplenishmentSource == INReplenishmentSource.DropShipToOrder)
						newVal = itemSettings.ReplenishmentSourceSiteID;
				}
				if (newVal == null)
					newVal = row.SiteID;

				e.NewValue = newVal;
				e.Cancel = true;
			}
		}

		protected virtual void SOLine_POSource_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if (row != null)
			{
				if (row.POCreate != true)
				{
					e.NewValue = null;
					e.Cancel = true;
					return;
				}
                else if (PXAccess.FeatureInstalled<FeaturesSet.dropShipments>())
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
					if (item != null && item.StkItem != true && item.NonStockReceipt == true && item.NonStockShip == true)
				{
					e.NewValue = INReplenishmentSource.DropShipToOrder;
					e.Cancel = true;
					return;
				}
				}

				INItemSiteSettings itemSettings = PXSelect<INItemSiteSettings, Where<INItemSiteSettings.inventoryID, Equal<Current<SOLine.inventoryID>>, And<INItemSiteSettings.siteID, Equal<Current<SOLine.siteID>>>>>.SelectSingleBound(this, new object[] { row });
				object newVal = null;
				if (itemSettings != null)
				{
					if (itemSettings.POSource == INReplenishmentSource.PurchaseToOrder ||
						itemSettings.POSource == INReplenishmentSource.DropShipToOrder)
						newVal = itemSettings.POSource;
				}

				if (newVal != null)
				{
					e.NewValue = newVal;
					e.Cancel = true;
				}
			}
		}

		protected virtual void SOLine_POSource_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if (row == null) return;
			
			if ( e.NewValue != null && e.NewValue.ToString() == INReplenishmentSource.DropShipToOrder)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
				if (item != null && item.StkItem != true)
				{
					if (item.NonStockReceipt != true || item.NonStockShip != true || row.LineType == SOLineType.MiscCharge)
					{
						throw new PXSetPropertyException<SOLine.pOSource>(Messages.ReceiptShipmentRequiredForDropshipNonstock);
							
					}
				}
			}
		}

		protected virtual void SOLine_POCreate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;

			if (row.POCreate == true)
			{
				sender.SetDefaultExt<SOLine.pOSource>(e.Row);
				}
			else
			{
				sender.SetValueExt<SOLine.pOSource>(e.Row, null);
			}
            LSSOLine.ResetAvailabilityCounters(row);
		}

		protected virtual void SOLineSplit_POCreate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLineSplit row = e.Row as SOLineSplit;
			if (row.POCreate == true)
			{
				sender.SetDefaultExt<SOLineSplit.pOSource>(e.Row);
			}
			else
			{
				sender.SetValueExt<SOLineSplit.pOSource>(e.Row, null);
			}
		}

		protected virtual void SOLineSplit_POSource_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null && row.POCreate != true)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void SOLineSplit_POSource_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
			{
			SOLineSplit row = e.Row as SOLineSplit;

			//TODO: revisit this code
			//row.POType = null;
			//row.PONbr = null;
			//row.POLineNbr = null;
		}

        protected virtual void SOLineSplit_LotSerialNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            SOLineSplit row = (SOLineSplit)e.Row;
            if (row != null && row.RequireLocation != true)
            {
                e.Cancel = true;
            }
        }

		protected virtual void SOLine_UnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((SOLine)e.Row).UOM) == false && (((SOLine)e.Row).LineType == SOLineType.NonInventory || ((SOLine)e.Row).LineType == SOLineType.MiscCharge) && Document.Current != null)
			{
				InventoryItem initem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, ((SOLine)e.Row).InventoryID);
				if (initem != null)
				{
					if (initem.StdCostDate <= Document.Current.OrderDate)
						e.NewValue = initem.StdCost ?? 0m;
					else
						e.NewValue = initem.LastStdCost ?? 0m;
					e.Cancel = true;
				}
			}
		}

		protected virtual void SOLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((SOLine)e.Row).UOM) == false)
			{
				object unitcost;
				sender.RaiseFieldDefaulting<SOLine.unitCost>(e.Row, out unitcost);

				if (unitcost != null && (decimal)unitcost != 0m)
				{
					decimal newval = INUnitAttribute.ConvertToBase<SOLine.inventoryID, SOLine.uOM>(sender, e.Row, (decimal)unitcost, INPrecision.UNITCOST);
					PXCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(sender, e.Row, newval, out newval);
					e.NewValue = newval;
					e.Cancel = true;
				}
			}
		}

		protected virtual void SOLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			string customerPriceClass = ARPriceClass.EmptyPriceClass;
			Location c = location.Select();
			if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
				customerPriceClass = c.CPriceClassID;

			SOLine row = e.Row as SOLine;

			if (row.TranType == INTranType.Transfer)
			{
				((SOLine)e.Row).CuryUnitPrice = 0;
			}
			else if (row.ManualPrice != true && !sender.Graph.IsCopyPasteContext)
			{
				decimal price =
					ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, ((SOLine)e.Row).CustomerID,
					((SOLine)e.Row).InventoryID,
					currencyinfo.Select(),
					((SOLine)e.Row).UOM, ((SOLine)e.Row).Qty, Document.Current.OrderDate.Value, ((SOLine)e.Row).CuryUnitPrice) ?? 0m;
				sender.SetValueExt<SOLine.curyUnitPrice>(e.Row, price);
			}
						
			sender.SetDefaultExt<SOLine.curyUnitCost>(e.Row);
			sender.SetValueExt<SOLine.extWeight>(e.Row, row.BaseQty * row.UnitWeigth);
			sender.SetValueExt<SOLine.extVolume>(e.Row, row.BaseQty * row.UnitVolume);
		}

		protected virtual void SOLine_Operation_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOLine.tranType>(e.Row);
			sender.SetDefaultExt<SOLine.invtMult>(e.Row);
			sender.SetDefaultExt<SOLine.planType>(e.Row);
			sender.SetDefaultExt<SOLine.requireReasonCode>(e.Row);
			sender.SetDefaultExt<SOLine.autoCreateIssueLine>(e.Row);
		}

	    protected virtual void SOLine_SalesPersonID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOLine.salesSubID>(e.Row);
		}

		protected virtual void SOLine_ReasonCode_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null)
			{
				ReasonCode reasoncd = PXSelect<ReasonCode, Where<ReasonCode.reasonCodeID, Equal<Optional<ReasonCode.reasonCodeID>>>>.Select(this, e.NewValue);

				e.Cancel = (reasoncd != null) && (
					row.TranType == INTranType.Transfer && reasoncd.Usage == ReasonCodeUsages.Transfer ||
					row.TranType != INTranType.Transfer && reasoncd.Usage == ReasonCodeUsages.Issue ||
					row.TranType != INTranType.Issue && row.TranType != INTranType.Receipt && reasoncd.Usage == ReasonCodeUsages.Sales 
					);
			}
		}

		protected virtual void SOLine_ReasonCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOLine.salesAcctID>(e.Row);
			try
			{
				sender.SetDefaultExt<SOLine.salesSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<SOLine.salesSubID>(e.Row, null);
			}
		}

		protected virtual void SOLine_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SOLine.salesAcctID>(e.Row);
			try
			{
				sender.SetDefaultExt<SOLine.salesSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<SOLine.salesSubID>(e.Row, null);
			}
			sender.SetDefaultExt<SOLine.pOCreate>(e.Row);
			sender.SetDefaultExt<SOLine.pOSource>(e.Row);

			if (string.IsNullOrEmpty(((SOLine)e.Row).InvoiceNbr))
			{
				sender.SetDefaultExt<SOLine.curyUnitCost>(e.Row);
			}
			sender.SetDefaultExt<SOLine.pOSiteID>(e.Row);
		}
		protected virtual void SOLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
            SOLine row = e.Row as SOLine;
            if (e.ExternalCall && row != null)
                row.CuryUnitPrice = 0m;

            sender.SetDefaultExt<SOLine.lineType>(e.Row);
			if (row.Operation == null)
            sender.SetDefaultExt<SOLine.operation>(e.Row);
            sender.SetDefaultExt<SOLine.tranType>(e.Row);
			sender.RaiseExceptionHandling<SOLine.uOM>(e.Row, null, null);
			sender.SetDefaultExt<SOLine.uOM>(e.Row);
			sender.SetValue<SOLine.closedQty>(e.Row, 0m);
			sender.SetDefaultExt<SOLine.orderQty>(e.Row);
			if (IsImport)
			{
				sender.SetDefaultExt<SOLine.salesPersonID>(e.Row);
				sender.SetDefaultExt<SOLine.reasonCode>(e.Row);
			}
			sender.SetDefaultExt<SOLine.salesAcctID>(e.Row);
			try
			{
				sender.SetDefaultExt<SOLine.salesSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<SOLine.salesSubID>(e.Row, null);
			}
			sender.SetDefaultExt<SOLine.tranDesc>(e.Row);
			sender.SetDefaultExt<SOLine.taxCategoryID>(e.Row);
			sender.SetDefaultExt<SOLine.vendorID>(e.Row);
			sender.SetDefaultExt<SOLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<SOLine.unitWeigth>(e.Row);
			sender.SetDefaultExt<SOLine.unitVolume>(e.Row);
			sender.SetDefaultExt<SOLine.pOSiteID>(e.Row);
		}

		//TODO: remove the code
		//protected virtual void SOLine_LineType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		//{
		//    if (e.Row != null && ((SOLine)e.Row).InventoryID != null)
		//    {
		//        InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<SOLine.inventoryID>(sender, e.Row);

		//        if (item != null)
		//        {
		//            object component = null;
		//            if (item.KitItem == true && item.StkItem == false)
		//            {
		//                component = (INKitSpecStkDet)PXSelect<INKitSpecStkDet, Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>>>.SelectWindowed(this, 0, 1, item.InventoryID);
		//                if (component == null)
		//                {
		//                    component = (INKitSpecNonStkDet)PXSelect<INKitSpecNonStkDet, Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>>>.SelectWindowed(this, 0, 1, item.InventoryID);
		//                }
		//            }

		//            e.NewValue =
		//                item.StkItem == true || component != null ? SOLineType.Inventory :
		//                item.NonStockShip == true ? SOLineType.NonInventory :
		//                SOLineType.MiscCharge;
		//            e.Cancel = true;
		//        }
		//    }
		//}

		protected virtual void SOLine_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null)
			{
				if (row.Qty == 0)
				{
					sender.SetValueExt<SOLine.curyDiscAmt>(row, decimal.Zero);
					sender.SetValueExt<SOLine.discPct>(row, decimal.Zero);
				}
				if (row.TranType == INTranType.Transfer)
				{
					((SOLine)e.Row).CuryUnitPrice = 0;
				}
				else if (row.ManualPrice != true && row.IsFree != true && !sender.Graph.IsCopyPasteContext)
				{
					string customerPriceClass = ARPriceClass.EmptyPriceClass;
					Location c = location.Select();
					if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
						customerPriceClass = c.CPriceClassID;
					decimal price =
						ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, row.CustomerID,
						row.InventoryID,
						currencyinfo.Select(),
						row.UOM, row.Qty, Document.Current.OrderDate.Value, row.CuryUnitPrice) ?? 0m;
					sender.SetValueExt<SOLine.curyUnitPrice>(e.Row, price);
				}
			}
		}

		protected virtual void SOLine_ManualPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null)
			{
				if (row.ManualPrice != true && row.IsFree != true && !sender.Graph.IsCopyPasteContext)
				{
					string customerPriceClass = ARPriceClass.EmptyPriceClass;
					Location c = location.Select();
					if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
						customerPriceClass = c.CPriceClassID;
					decimal price =
						ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, row.CustomerID,
						row.InventoryID,
						currencyinfo.Select(),
						row.UOM, row.Qty, Document.Current.OrderDate.Value, row.CuryUnitPrice) ?? 0m;
					sender.SetValueExt<SOLine.curyUnitPrice>(e.Row, price);
				}
			}
		}

		protected virtual void SOLine_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row == null) return;
						
			sender.SetDefaultExt<SOLine.salesPersonID>(e.Row);
			
			try
			{
				sender.SetDefaultExt<SOLine.salesSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<SOLine.salesSubID>(e.Row, null);
			}
		}

		public virtual bool RecalculatePriceAndDiscount()
		{
			//TODO based on setting. Default is True, If UnattendedMode & PrompUser - use Default.
			return true;
		}

		protected virtual void SOLine_OrderQty_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if ((decimal?)e.NewValue < ((SOLine)e.Row).ClosedQty && ((SOLine)e.Row).RequireShipping == true && (((SOLine)e.Row).LineType == "GI" || ((SOLine)e.Row).LineType == "GN"))
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GE, sender.GetStateExt<SOLine.closedQty>(e.Row));
			}
		}

		protected virtual void SOLine_DiscPct_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
		}

		protected virtual void SOLine_DiscPct_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<SOLine>.ValidateMinGrossProfitPct(sender, row, row.UnitPrice, ref newValue);
			e.NewValue = newValue;
		}

		protected virtual void SOLine_CuryDiscAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<SOLine>.ValidateMinGrossProfitAmt(sender, row, row.UnitPrice, ref newValue);
			e.NewValue = newValue;
		}

		protected virtual void SOLine_CuryUnitPrice_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null && row.IsFree != true)
			{
				decimal? newValue = (decimal?)e.NewValue;
				SODiscountEngine<SOLine>.ValidateMinGrossProfitUnitPrice<SOLine.curyInfoID>(sender, row, ref newValue);
				e.NewValue = newValue;
			}
		}

		protected virtual void SOLine_IsFree_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (row != null)
			{
				if (row.IsFree == true)
				{
					sender.SetValueExt<SOLine.curyUnitPrice>(row, 0m);
					sender.SetValueExt<SOLine.discPct>(row, 0m);
					sender.SetValueExt<SOLine.curyDiscAmt>(row, 0m);
					if (e.ExternalCall)
						sender.SetValueExt<SOLine.manualDisc>(row, true);
				}
				else
				{
					if (row.ManualPrice != true && !sender.Graph.IsCopyPasteContext)
					{
						string customerPriceClass = ARPriceClass.EmptyPriceClass;
						Location c = location.Select();
						if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
							customerPriceClass = c.CPriceClassID;

						decimal salesPrice = ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, row.CustomerID, row.InventoryID, currencyinfo.Select(), row.UOM, row.Qty, Document.Current.OrderDate.Value, row.CuryUnitPrice) ?? 0m;
						sender.SetValueExt<SOLine.curyUnitPrice>(row, salesPrice);
					}
				}
			}
		}


		protected virtual void SOLine_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			if (e.ExternalCall && row != null)
			{
				DiscountEngine<SOLine>.UpdateManualLineDiscount<SOOrderDiscountDetail>(sender, Transactions, row, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate);
			}
		}

		protected virtual void SOLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SOLine row = (SOLine)e.Row;
			if (row == null) return;
			bool lineTypeInventory = row.LineType == SOLineType.Inventory;
			bool isNonStockKit = (!row.IsStockItem ?? false) && (row.IsKit ?? false);
			PXUIFieldAttribute.SetEnabled<SOLine.subItemID>(sender, row, lineTypeInventory && !isNonStockKit);
			PXUIFieldAttribute.SetEnabled<SOLine.locationID>(sender, row, lineTypeInventory);

			bool enabled = (soordertype.Current.RequireShipping == true && row.TranType != INDocType.Undefined && row.Operation == SOOperation.Issue);
			PXUIFieldAttribute.SetEnabled<SOLine.pOCreate>(sender, row, enabled);
			PXUIFieldAttribute.SetEnabled<SOLine.vendorID>(sender, row, enabled);

			bool editable = false;
			if (row.POCreate == true)
			{
				ARTran tran = PXSelect<ARTran, Where<ARTran.sOOrderType, Equal<Required<ARTran.sOOrderType>>, And<ARTran.sOOrderNbr, Equal<Required<ARTran.sOOrderNbr>>, And<ARTran.sOOrderLineNbr, Equal<Required<ARTran.sOOrderLineNbr>>>>>>.SelectWindowed(this, 0, 1, row.OrderType, row.OrderNbr, row.LineNbr);
				editable = (tran == null);
			}

			PXUIFieldAttribute.SetEnabled<POLine3.selected>(posupply.Cache, null, enabled && editable);
			PXUIFieldAttribute.SetEnabled<SOLine.curyUnitPrice>(sender, row, row.IsFree != true);

			bool autoFreeItem = row.ManualDisc != true && row.IsFree == true;
            bool freeItem = row.IsFree == true;

			PXUIFieldAttribute.SetEnabled<SOLine.manualDisc>(sender, e.Row, !freeItem);
			PXUIFieldAttribute.SetEnabled<SOLine.orderQty>(sender, e.Row, !autoFreeItem);
			PXUIFieldAttribute.SetEnabled<SOLine.isFree>(sender, e.Row, !autoFreeItem && row.InventoryID != null);
			PXUIFieldAttribute.SetEnabled<SOLine.pOSource>(sender, e.Row, enabled && row.POCreate == true && (row.POSource != INReplenishmentSource.PurchaseToOrder || row.ShippedQty == 0m));
			
			if (((SOLine)e.Row).ShippedQty > 0m)
			{
				bool? Cancelled = ((SOLine)e.Row).Completed;
				PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<SOLine.tranDesc>(sender, e.Row);
				PXUIFieldAttribute.SetEnabled<SOLine.orderQty>(sender, e.Row, Cancelled == false);
				PXUIFieldAttribute.SetEnabled<SOLine.shipComplete>(sender, e.Row, Cancelled == false);
				PXUIFieldAttribute.SetEnabled<SOLine.completeQtyMin>(sender, e.Row, Cancelled == false);
				PXUIFieldAttribute.SetEnabled<SOLine.completeQtyMax>(sender, e.Row, Cancelled == false);
				PXUIFieldAttribute.SetEnabled<SOLine.completed>(sender, e.Row, true);
			} 

            SOLine line = (SOLine)e.Row;
            if (line != null && line.Operation == SOOperation.Issue)
			{
                PXUIFieldAttribute.SetEnabled<SOLine.autoCreateIssueLine>(sender, e.Row, false);
            }
			// TODO: Low Priority refactor to SOLineSplit
			//else if (((SOLine)e.Row).PONbr != null)
			//{
			//    PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
			//    PXUIFieldAttribute.SetEnabled<SOLine.tranDesc>(sender, e.Row);
			//    PXUIFieldAttribute.SetEnabled<SOLine.manualDisc>(sender, e.Row, !autoFreeItem);
			//    PXUIFieldAttribute.SetEnabled<SOLine.orderQty>(sender, e.Row, !autoFreeItem);
			//    PXUIFieldAttribute.SetEnabled<SOLine.isFree>(sender, e.Row, !autoFreeItem);
			//    PXUIFieldAttribute.SetEnabled<SOLine.discPct>(sender, e.Row, !autoFreeItem);
			//    PXUIFieldAttribute.SetEnabled<SOLine.curyDiscAmt>(sender, e.Row, !autoFreeItem);
			//    PXUIFieldAttribute.SetEnabled<SOLine.curyLineAmt>(sender, e.Row, !autoFreeItem);

			//    PXUIFieldAttribute.SetEnabled<SOLine.curyUnitPrice>(sender, row, row.IsFree != true);
			//    PXUIFieldAttribute.SetEnabled<SOLine.pOCreate>(sender, row, enabled); 
			//    PXUIFieldAttribute.SetEnabled<SOLine.orderQty>(sender, e.Row, true);
			//    PXUIFieldAttribute.SetEnabled<SOLine.shipComplete>(sender, e.Row, true);
			//    PXUIFieldAttribute.SetEnabled<SOLine.completeQtyMin>(sender, e.Row, true);
			//    PXUIFieldAttribute.SetEnabled<SOLine.completeQtyMax>(sender, e.Row, true);
			//    PXUIFieldAttribute.SetEnabled<SOLine.shipDate>(sender, e.Row, this.Document.Current.ShipComplete == SOShipComplete.BackOrderAllowed);
			//    PXUIFieldAttribute.SetEnabled<SOLine.requestDate>(sender, e.Row, true);
			//}

			SOOrder header = Document.Current;

			if (header != null)
			{
				PXUIFieldAttribute.SetEnabled<SOLine.shipDate>(sender, row, header.ShipComplete == SOShipComplete.BackOrderAllowed);

				if (header.Hold != true && header.DontApprove != true)
			{
				PXUIFieldAttribute.SetEnabled(sender, row, false);
			}
			}

			POCreateVerifyValue(sender, row, row.POCreate);
		}


		protected virtual void SOLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SOLine row = e.Row as SOLine;

			if (row != null && row.ManualDisc != true)
			{
				RecalculateDiscounts(sender, (SOLine)e.Row);
			}

			TaxAttribute.Calculate<SOLine.taxCategoryID>(sender, e);
		}

		protected virtual void SOLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{				
				SOLine line = (SOLine)e.Row;
				
				PXDefaultAttribute.SetPersistingCheck<SOLine.salesAcctID>(sender, e.Row,
						soordertype.Current == null || Document.Current == null || 
						soordertype.Current.ARDocType == ARDocType.NoUpdate || 
						Document.Current.Hold == true
						? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

				PXDefaultAttribute.SetPersistingCheck<SOLine.salesSubID>(sender, e.Row,
						soordertype.Current == null || Document.Current == null ||
						soordertype.Current.ARDocType == ARDocType.NoUpdate ||
						Document.Current.Hold == true
						? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

				PXDefaultAttribute.SetPersistingCheck<SOLine.subItemID>(sender, e.Row,
					soordertype.Current == null || soordertype.Current.RequireLocation == true || line.LineType != SOLineType.Inventory || ((!line.IsStockItem ?? false) && (line.IsKit ?? false))
					? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

				PXDefaultAttribute.SetPersistingCheck<SOLine.reasonCode>(sender, e.Row,
					line.RequireReasonCode == true
					? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

				PXDefaultAttribute.SetPersistingCheck<SOLine.pOSource>(sender, e.Row, line.POCreate == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
				PXDefaultAttribute.SetPersistingCheck<SOLine.taskID>(sender, e.Row, ProjectDefaultAttribute.IsProject(this, ((SOLine)e.Row).ProjectID) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

				if ( !lsselect.MemoAvailabilityCheck(sender, line) )
				{
					throw new PXRowPersistingException("Document Details", line,  Messages.ValidationFailed);
				}
			}
		}

		protected virtual void SOLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOLine row = e.Row as SOLine;
			SOLine oldRow = e.OldRow as SOLine;
			
			if (row != null && row.RequireLocation == false)
				row.LocationID = null;

			if (!sender.ObjectsEqual<SOLine.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.customerID>(e.Row, e.OldRow)  || !sender.ObjectsEqual<SOLine.inventoryID>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<SOLine.siteID>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.orderQty>(e.Row, e.OldRow) || 
					!sender.ObjectsEqual<SOLine.curyUnitPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.curyExtPrice>(e.Row, e.OldRow) || 
					!sender.ObjectsEqual<SOLine.curyDiscAmt>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.discPct>(e.Row, e.OldRow) || 
					!sender.ObjectsEqual<SOLine.manualDisc>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.discountID>(e.Row, e.OldRow))
				if (row.ManualDisc != true)
				{
					if (oldRow.ManualDisc == true)//Manual Discount Unckecked
					{
						if (row.IsFree == true)
						{
							ResetQtyOnFreeItem(sender, row);
						}
					}

					if (row.IsFree == true)
					{
						DiscountEngine<SOLine>.ClearDiscount(sender, row);
					}

					RecalculateDiscounts(sender, row);
				}
				else
				{
					RecalculateDiscounts(sender, row);
				}

			if(row.ManualDisc != true)
			{
				var discountCode = (ARDiscount)PXSelectorAttribute.Select<SOLine.discountID>(sender, row);
				row.DiscPctDR = (discountCode != null && discountCode.IsAppliedToDR == true) ? row.DiscPct : 0.0m;
			}

            if ((e.ExternalCall || sender.Graph.IsImport)
				&& sender.ObjectsEqual<SOLine.customerID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<SOLine.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<SOLine.uOM>(e.Row, e.OldRow)
                && sender.ObjectsEqual<SOLine.orderQty>(e.Row, e.OldRow) && sender.ObjectsEqual<SOLine.branchID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<SOLine.siteID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<SOLine.manualPrice>(e.Row, e.OldRow)
				&& (!sender.ObjectsEqual<SOLine.curyUnitPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOLine.curyLineAmt>(e.Row, e.OldRow)))
                row.ManualPrice = true;

			if(row.ManualPrice != true)
			{
				row.CuryUnitPriceDR = row.CuryUnitPrice;
			}

			TaxAttribute.Calculate<SOLine.taxCategoryID>(sender, e);
			
			DirtyFormulaAttribute.RaiseRowUpdated<SOLine.openLine>(sender, e);
	
			//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
			if (Document.Current != null && IsExternalTax == true)
			{
				if (!sender.ObjectsEqual
						<SOLine.salesAcctID, SOLine.inventoryID, SOLine.tranDesc, SOLine.lineAmt, SOLine.orderDate,
						SOLine.taxCategoryID>(e.Row, e.OldRow))
				{
					Document.Current.IsTaxValid = false;
					Document.Current.IsOpenTaxValid = false;
					Document.Current.IsUnbilledTaxValid = false;
					Document.Current.IsFreightTaxValid = false;
				}

				if (!sender.ObjectsEqual<SOLine.openAmt>(e.Row, e.OldRow))
				{
					Document.Current.IsOpenTaxValid = false;
				}

				if (!sender.ObjectsEqual<SOLine.unbilledAmt>(e.Row, e.OldRow))
				{
					Document.Current.IsUnbilledTaxValid = false;
				}
			}
		}

		protected virtual void SOLine_RowDeleting(PXCache sedner, PXRowDeletingEventArgs e)
		{
			SOLine row = e.Row as SOLine;				
			if(row != null && row.ShippedQty > 0)
				throw new PXException(Messages.ShippedLineDeleting);
		}

        protected virtual void SOLineSplit_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOLineSplit row = (SOLineSplit)e.Row;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled<SOLineSplit.subItemID>(sender, row, false);
			PXDefaultAttribute.SetPersistingCheck<SOLineSplit.lotSerialNbr>(sender, row, (soordertype.Current != null && (soordertype.Current.RequireLotSerial == true || soordertype.Current.Behavior == SOBehavior.IN || soordertype.Current.Behavior == SOBehavior.CM) ) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			if (row.POCancelled == true && row.Completed == true && row.PONbr != null)
			{
				splits.Cache.RaiseExceptionHandling<SOLineSplit.refNoteID>(row, null, new PXSetPropertyException(Messages.POLinkedToSOCancelled, PXErrorLevel.RowWarning, row.PONbr));
			}
		}
        protected virtual void SOLineSplit_LotSerialNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOLineSplit row = (SOLineSplit)e.Row;
            if (row == null) return;

            if (soordertype.Current != null && soordertype.Current.RequireShipping == true && soordertype.Current.INDocType != INTranType.NoUpdate && row.LotSerialNbr != null)
            {
                PXResult<InventoryItem, INLotSerClass> inLotSerialClass = (PXResult<InventoryItem, INLotSerClass>)PXSelectJoin<InventoryItem, InnerJoin<INLotSerClass,
                    On<InventoryItem.lotSerClassID, Equal<INLotSerClass.lotSerClassID>>>,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);

                if (inLotSerialClass != null)
                {
                    INLotSerClass lotserialclass = (INLotSerClass)inLotSerialClass;
                    if (lotserialclass != null && lotserialclass.LotSerAssign != INLotSerAssign.WhenReceived)
                    {
                        splits.Cache.RaiseExceptionHandling<SOLineSplit.lotSerialNbr>(row, null,
                            new PXSetPropertyException(Messages.LotSerialSelectionForOnReceiptOnly, PXErrorLevel.Warning));
                        row.LotSerialNbr = null;
                    }
                }
            }
        }

		protected virtual void SOLineSplit_RowDeleting(PXCache sedner, PXRowDeletingEventArgs e)
		{
			SOLineSplit row = e.Row as SOLineSplit;
			if (row != null && row.ShippedQty > 0)
				throw new PXException(Messages.ShippedLineDeleting);
		}


		[PXDBString(15, IsUnicode = true, BqlField = typeof(SOLineSplit.sOOrderNbr))]
		[PXDBDefault(typeof(SOOrder.orderNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void SOLineSplit2_SOOrderNbr_CacheAttached(PXCache sender)
		{ 
		}

		protected virtual void SOLineSplit_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			SOLineSplit row = e.Row as SOLineSplit;

			foreach (PXResult<SOLineSplit2, INItemPlan, SOOrderTypeOperation> r in this.sodemand.View.SelectMultiBound(new object[] { e.Row }))
			{
				SOLineSplit2 upd = PXCache<SOLineSplit2>.CreateCopy(r);

				upd.SOOrderType = null;
				upd.SOOrderNbr = null;
				upd.SOLineNbr = null;
				upd.SOSplitLineNbr = null;
				upd.SiteID = upd.ToSiteID;
				upd.IsAllocated = false;
				upd.RefNoteID = null;

				this.sodemand.Update(upd);

				INItemPlan plan = r;
				SOOrderTypeOperation op = r;
				if (plan.PlanType != null)
				{
					if (op.OrderPlanType != null)
					{
						plan.PlanType = op.OrderPlanType;
					}
					plan.SupplyPlanID = null;
					plan.SiteID = upd.ToSiteID;
					plan.FixedSource = null;

					sender.Graph.Caches[typeof(INItemPlan)].Update(plan);
				}
			} 
		}

		protected virtual void SOLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			SOLine row = e.Row as SOLine;

			if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
			{
				DiscountEngine<SOLine>.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate);
						RecalculateTotalDiscount();
					RefreshFreeItemLines(sender);
				}

			if (row != null && this.sooperation.Current.INDocType == INTranType.Transfer)
			{
				ClearReplenishmentPlans(row);
			}

			if (Document.Current != null)
			{
				Document.Current.IsTaxValid = false;
				Document.Current.IsOpenTaxValid = false;
				Document.Current.IsUnbilledTaxValid = false;
				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
					Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
			}
		}

		public virtual void ClearReplenishmentPlans(SOLine row)
		{
			foreach (PXResult<INReplenishmentLine, INItemPlan> r in replenishment.View.SelectMultiBound(new object[] { row }))
			{
				INReplenishmentLine line = r;
				INItemPlan plan = r;
				replenishment.Delete(line);
				this.Caches[typeof(INItemPlan)].Delete(plan);
			}
		}

		protected virtual void SOLine_AvgCost_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				decimal? AvgCost = (decimal?)sender.GetValue<SOLine.avgCost>(e.Row);

				if (AvgCost != null)
				{
					AvgCost = INUnitAttribute.ConvertToBase<SOLine.inventoryID, SOLine.uOM>(sender, e.Row, (decimal)AvgCost, INPrecision.UNITCOST);

					if (!sender.Graph.Accessinfo.CuryViewState)
					{
						decimal CuryAvgCost;

						PXCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(sender, e.Row, (decimal)AvgCost, out CuryAvgCost, true);

						e.ReturnValue = (decimal)Math.Round(CuryAvgCost, CommonSetupDecPl.PrcCst, MidpointRounding.AwayFromZero);
					}
					else
					{
						e.ReturnValue = AvgCost;
					}
				}
			}
		}

		#endregion

		
		#region SOOrderDiscountDetail events

		protected virtual void SOOrderDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (Document != null && Document.Current != null)
				Document.Cache.SetValueExt<SOOrder.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<SOOrderDiscountDetail>(DiscountDetails, true));
		}

		protected virtual void SOOrderDiscountDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SOOrderDiscountDetail discountDetail = (SOOrderDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null && discountDetail.DiscountID != null)
			{
				discountDetail.IsManual = true;
				DiscountEngine<SOLine>.InsertDocGroupDiscount<SOOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, null, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate);
                RefreshTotalsAndFreeItems(sender);
			}
		}

		protected virtual void SOOrderDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOOrderDiscountDetail discountDetail = (SOOrderDiscountDetail)e.Row;
            if (e.ExternalCall == true && discountDetail != null)
            {
                if (!sender.ObjectsEqual<SOOrderDiscountDetail.skipDiscount>(e.Row, e.OldRow))
                {
                    DiscountEngine<SOLine>.UpdateDocumentDiscount<SOOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate, discountDetail.Type != DiscountType.Document);
                    RefreshTotalsAndFreeItems(sender);
                }
                if (!sender.ObjectsEqual<SOOrderDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOOrderDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
			{
                    discountDetail.IsManual = true;
                    DiscountEngine<SOLine>.InsertDocGroupDiscount<SOOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<SOOrderDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate);
                    RefreshTotalsAndFreeItems(sender);
                }
			}
		}

		protected virtual void SOOrderDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			SOOrderDiscountDetail discountDetail = (SOOrderDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null)
			{
                DiscountEngine<SOLine>.UpdateDocumentDiscount<SOOrderDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.OrderDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document));
			}
            RefreshTotalsAndFreeItems(sender);
		}
		#endregion

        [PXDBString(10, IsUnicode = true)]
        [PXDBDefault(typeof(SOOrder.taxZoneID))]
        [PXUIFieldAttribute(DisplayName = "Customer Tax Zone", Enabled = false)]
        protected virtual void SOTaxTran_TaxZoneID_CacheAttached(PXCache sender)
        { 
        }

        protected virtual void SOTaxTran_TaxZoneID_ExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs e)
        {
            Exception ex = e.Exception as PXSetPropertyException;
            if (ex != null)
            {
                Document.Cache.RaiseExceptionHandling<SOOrder.taxZoneID>(Document.Current, null, ex);
            }
        }

		protected virtual void SOTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<SOTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}

		protected virtual void CustomerPaymentMethodInputMode_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CustomerPaymentMethodInputMode inputMode = e.Row as CustomerPaymentMethodInputMode;
			if (inputMode == null) return;
			if (DefPaymentMethodInstance.Current != null)
			{
				bool? isVisible = null;
				foreach (PXEventSubscriberAttribute attribute in cache.GetAttributesReadonly<CustomerPaymentMethodInputMode.inputMode>(inputMode))
				{
					if (attribute is PXUIFieldAttribute)
					{
						isVisible = ((PXUIFieldAttribute)attribute).Visible;
					}
				}
				if (isVisible == false)
				{
					int? pmInstanceId = DefPaymentMethodInstance.Current.PMInstanceID;
					bool isTokenized = CCProcessingUtils.isTokenizedPaymentMethod(this, pmInstanceId);
					bool isHFPM = CCProcessingUtils.isHFPaymentMethod(this, pmInstanceId);
					inputMode.InputMode = isHFPM ? InputModeType.Token : InputModeType.Details;
				}
			}
		}

		protected virtual void SOShippingAddress_PostalCode_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SOShippingAddress row = e.Row as SOShippingAddress;
			if (row != null)
			{
				Document.Cache.SetDefaultExt<SOOrder.taxZoneID>(Document.Current);
			}
		}
	
		protected virtual void AddInvoiceFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			invoicesplits.Cache.Clear();
		}


		#region SOCarrierRate
		protected virtual void SOCarrierRate_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOCarrierRate row = e.Row as SOCarrierRate;
			if (row != null)
			{
				if (row.Selected == true)
				{
					Document.SetValueExt<SOOrder.shipVia>(Document.Current, row.Method);
					
					foreach (SOCarrierRate cr in sender.Cached)
					{
						if (cr.LineNbr != row.LineNbr)
						{
							cr.Selected = false;
						}
					}

					CarrierRates.View.RequestRefresh();
				}
				else
				{
					Document.SetValueExt<SOOrder.shipVia>(Document.Current, null);
				}
			}
		}

		protected virtual void SOCarrierRate_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}
		#endregion
		#region SOPackageInfo
		protected virtual void SOPackageInfoEx_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SOPackageInfo row = e.Row as SOPackageInfo;
			if (row != null && Document.Current != null)
			{
				row.WeightUOM = insetup.Current.WeightUOM;
				PXUIFieldAttribute.SetEnabled<SOPackageInfo.inventoryID>(sender, e.Row, Document.Current.IsManualPackage == true);
				PXUIFieldAttribute.SetEnabled<SOPackageInfo.siteID>(sender, e.Row, Document.Current.IsManualPackage == true);
			}
		}
		protected virtual void SOPackageInfoEx_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SOPackageInfo row = e.Row as SOPackageInfo;
			if (row != null)
			{
				CSBox box = PXSelect<CSBox, Where<CSBox.boxID, Equal<Required<CSBox.boxID>>>>.Select(this, row.BoxID);
				if (box != null && box.MaxWeight < row.GrossWeight)
				{
					sender.RaiseExceptionHandling<SOPackageInfo.grossWeight>(row, row.GrossWeight, new PXSetPropertyException(Messages.WeightExceedsBoxSpecs));
				}
			}
		}
		#endregion

		protected readonly string viewInventoryID;

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
                            ((PXAction<SOOrder>)this.Actions["Hold"]).PressImpl(false);
                        }
                    }

                    if (values.Contains("CreditHold") && values["CreditHold"] != PXCache.NotSetValue && values["CreditHold"] != null)
                    {
                        var creditHold = Document.Current.CreditHold ?? false;
                        if (Convert.ToBoolean(values["CreditHold"]) != creditHold)
                        {
                            ((PXAction<SOOrder>)this.Actions["CreditHold"]).PressImpl(false);
                        }
                    }
                }
                
                values["Hold"] = PXCache.NotSetValue;
				values["CreditHold"] = PXCache.NotSetValue;
			}
			return base.ExecuteUpdate(viewName, keys, values, parameters);
		}
		
		#region Discount
				
		protected virtual void RecalculateDiscounts(PXCache sender, SOLine line)
		{
			if (line.InventoryID != null && line.Qty != null && line.CuryLineAmt != null && line.IsFree != true)
			{
				DiscountEngine<SOLine>.SetDiscounts<SOOrderDiscountDetail>(sender, Transactions, line, DiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.CuryID, Document.Current.OrderDate, false, recalcdiscountsfilter.Current);

				RecalculateTotalDiscount();

				RefreshFreeItemLines(sender);
			}
		}

		private void RefreshFreeItemLines(PXCache sender)
		{
            if (sender.Graph.IsImport && !sender.Graph.IsMobile)
                return;

            Dictionary<int, decimal> groupedByInventory = new Dictionary<int, decimal>();

			PXSelectBase<SOOrderDiscountDetail> select = new PXSelect<SOOrderDiscountDetail,
				Where<SOOrderDiscountDetail.orderType, Equal<Current<SOOrder.orderType>>,
				And<SOOrderDiscountDetail.orderNbr, Equal<Current<SOOrder.orderNbr>>,
				And<SOOrderDiscountDetail.skipDiscount, NotEqual<boolTrue>>>>>(this);

			foreach (SOOrderDiscountDetail item in select.Select())
			{
				if (item.FreeItemID != null)
				{
					if (groupedByInventory.ContainsKey(item.FreeItemID.Value))
					{
						groupedByInventory[item.FreeItemID.Value] += item.FreeItemQty ?? 0;
					}
					else
					{
						groupedByInventory.Add(item.FreeItemID.Value, item.FreeItemQty ?? 0);
					}

				}

			}

			bool freeItemChanged = false;

			#region Delete Unvalid FreeItems
			foreach (SOLine line in FreeItems.Select())
			{
				if (line.ManualDisc == false && line.InventoryID != null)
				{
					if (groupedByInventory.ContainsKey(line.InventoryID.Value))
					{
						if (groupedByInventory[line.InventoryID.Value] == 0)
						{
							FreeItems.Delete(line);
							freeItemChanged = true;
						}
					}
					else
					{
						FreeItems.Delete(line);
						freeItemChanged = true;
					}
				}
			}

			#endregion

			int? defaultWarehouse = GetDefaultWarehouse();
			foreach (KeyValuePair<int, decimal> kv in groupedByInventory)
			{
				SOLine freeLine = GetFreeLineByItemID(kv.Key);

				if (freeLine == null)
				{
					if (kv.Value > 0)
					{
						SOLine line = new SOLine();
						line.InventoryID = kv.Key;
						line.IsFree = true;
						line.SiteID = defaultWarehouse;
						line.OrderQty = kv.Value;
						line.ShipComplete = SOShipComplete.CancelRemainder;
						line = FreeItems.Insert(line);

						freeItemChanged = true;
					}
				}
				else
				{
					SOLine copy = PXCache<SOLine>.CreateCopy(freeLine);
					copy.OrderQty = kv.Value;
					FreeItems.Cache.Update(copy);

					freeItemChanged = true;
				}
			}

			if (freeItemChanged)
			{
				Transactions.View.RequestRefresh();
			}
		}
		
		private SOLine GetFreeLineByItemID(int? inventoryID)
		{
			return PXSelect<SOLine,
				Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
				And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>,
				And<SOLine.lineType, NotEqual<SOLineType.miscCharge>,
				And<SOLine.isFree, Equal<boolTrue>,
				And<SOLine.inventoryID, Equal<Required<SOLine.inventoryID>>,
				And<SOLine.manualDisc, Equal<boolFalse>>>>>>>>.Select(this, inventoryID);
		}

		private void ResetQtyOnFreeItem(PXCache sender, SOLine line)
		{
			PXSelectBase<SOOrderDiscountDetail> select = new PXSelect<SOOrderDiscountDetail,
				Where<SOOrderDiscountDetail.orderType, Equal<Current<SOOrder.orderType>>,
				And<SOOrderDiscountDetail.orderNbr, Equal<Current<SOOrder.orderNbr>>,
				And<SOOrderDiscountDetail.freeItemID, Equal<Required<SOOrderDiscountDetail.freeItemID>>>>>>(this);

			decimal? qtyTotal = 0;
			foreach (SOOrderDiscountDetail item in select.Select(line.InventoryID))
			{
				if (item.SkipDiscount != true && item.FreeItemID != null && item.FreeItemQty != null && item.FreeItemQty.Value > 0)
				{
					qtyTotal += item.FreeItemQty.Value;
				}
			}

			sender.SetValueExt<SOLine.orderQty>(line, qtyTotal);
		}

		/// <summary>
		/// If all lines are from one site/warehouse - return this warehouse otherwise null;
		/// </summary>
		/// <returns>Default Wartehouse for Free Item</returns>
		private int? GetDefaultWarehouse()
		{
			PXResultset<SOOrderSite> osites = PXSelectJoin<SOOrderSite,
				InnerJoin<INSite, On<INSite.siteID, Equal<SOOrderSite.siteID>>>,
				Where<SOOrderSite.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOOrderSite.orderNbr, Equal<Current<SOOrder.orderNbr>>,
					And<Match<INSite, Current<AccessInfo.userName>>>>>>.Select(this);

			if (osites.Count == 1)
			{
				return ((SOOrderSite)osites).SiteID;
			}
			return null;
		}

		private void RecalculateTotalDiscount()
		{
			if (Document.Current != null)
			{
				SOOrder copy = PXCache<SOOrder>.CreateCopy(Document.Current);
				copy.CuryDiscTot = DiscountEngine.GetTotalGroupAndDocumentDiscount<SOOrderDiscountDetail>(DiscountDetails);
				Document.Update(copy);
			}
		}

        private void RefreshTotalsAndFreeItems(PXCache sender)
        {
            RecalculateTotalDiscount();
            RefreshFreeItemLines(sender);
		}
		#endregion

		#region Carrier Freight Cost

		protected virtual bool CollectFreight
		{
			get
			{
				if ( DocumentProperties.Current != null )
				{
					if (DocumentProperties.Current.UseCustomerAccount == true)
						return false;

					if (DocumentProperties.Current.GroundCollect == true)
						return false;
				}

				return true;
			}
		}

		private CarrierRequest BuildRateRequest(SOOrder order)
		{
			if (string.IsNullOrEmpty(order.ShipVia))
				return null;

			Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Current<SOOrder.shipVia>>>>.Select(this);
			if (carrier == null)
				throw new PXException(Messages.CarrierWithIdNotFound);

			if (carrier.IsExternal != true)
				return null;

			CarrierPlugin plugin = PXSelect<CarrierPlugin, Where<CarrierPlugin.carrierPluginID, Equal<Required<CarrierPlugin.carrierPluginID>>>>.Select(this, carrier.CarrierPluginID);
			if (plugin == null)
				throw new PXException(Messages.CarrierPluginWithIdNotFound);

			SOShippingAddress shipAddress = Shipping_Address.Select();
			BAccount companyAccount = PXSelectJoin<BAccountR, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, Accessinfo.BranchID);
			Address companyAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, companyAccount.DefAddressID);

			UnitsType unit = UnitsType.SI;
			if (string.IsNullOrEmpty(plugin.UOM))
			{
				throw new PXException(Messages.CarrierUOMIsRequired, plugin.CarrierPluginID);
			}

			if (plugin.UnitType == CarrierUnitsType.US)
			{
				unit = UnitsType.US;
			}

			IList<SOPackageEngine.PackSet> packSets = GetManualPackages(order);
				
			if (packSets.Count == 0)
			{
				throw new PXException(Messages.AtleastOnePackageIsRequired);
			}

			List<CarrierBoxEx> boxes = new List<CarrierBoxEx>();
			foreach (SOPackageEngine.PackSet packSet in packSets)
			{
				INSite shipToWarehouse = PXSelect<INSite, Where<INSite.siteID, Equal<Required<SOOrder.defaultSiteID>>>>.Select(this, packSet.SiteID);
				Address warehouseAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, shipToWarehouse.AddressID);
				boxes.Add(BuildCarrierBoxes(packSet.Packages, warehouseAddress, carrier.PluginMethod, plugin.UOM));
			}

			CarrierRequest cr = new CarrierRequest(unit, order.CuryID);
			cr.Shipper = companyAddress;
			cr.Origin = null;
			cr.Destination = shipAddress;
			cr.PackagesEx = boxes;
			cr.Resedential = order.Resedential == true;
			cr.SaturdayDelivery = order.SaturdayDelivery == true;
			cr.Insurance = order.Insurance == true;
			cr.ShipDate = order.ShipDate.Value;
			cr.Methods = GetCarrierMethods(carrier.CarrierPluginID);
			cr.Attributes = new List<string>();
			cr.InvoiceLineTotal = Document.Current.CuryLineTotal.GetValueOrDefault();

			if (order.GroundCollect == true)
			{
				cr.Attributes.Add("COLLECT");
			}

			return cr;
		}

		private CarrierRequest BuildQuoteRequest(SOOrder order, CarrierPlugin plugin)
		{
			SOShippingAddress shipAddress = Shipping_Address.Select();
			BAccount companyAccount = PXSelectJoin<BAccountR, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, Accessinfo.BranchID);
			Address companyAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, companyAccount.DefAddressID);

			UnitsType unit = UnitsType.SI;
			if (string.IsNullOrEmpty(plugin.UOM))
			{
				throw new PXException(Messages.CarrierUOMIsRequired, plugin.CarrierPluginID);
			}

			if (plugin.UnitType == CarrierUnitsType.US)
			{
				unit = UnitsType.US;
			}

			List<string> methods = GetCarrierMethods(plugin.CarrierPluginID);

			List<CarrierBoxEx> boxes = new List<CarrierBoxEx>();
			foreach (string method in methods)
			{
				IList<SOPackageEngine.PackSet> packSets = null;
				if (order.IsManualPackage == true)
					packSets = GetManualPackages(order);
				else
				{
					if (order.IsPackageValid == true)
					{
						Dictionary<int, SOPackageEngine.PackSet> packs = new Dictionary<int, SOPackageEngine.PackSet>();
						foreach (SOPackageInfoEx package in Packages.View.SelectMultiBound(new object[] { order }))
						{
							SOPackageEngine.PackSet set = null;
							if ( !packs.TryGetValue(package.SiteID.GetValueOrDefault(), out set))
							{
								set = new SOPackageEngine.PackSet(package.SiteID.GetValueOrDefault());
								packs.Add(set.SiteID, set);
							}

							set.Packages.Add(package);
						}

						packSets = packs.Values.ToList<SOPackageEngine.PackSet>();
					}
					else
					{
						packSets = CalculatePackages(order, null);
					}
				}

				foreach (SOPackageEngine.PackSet packSet in packSets)
				{
					INSite shipToWarehouse = PXSelect<INSite, Where<INSite.siteID, Equal<Required<SOOrder.defaultSiteID>>>>.Select(this, packSet.SiteID);
					Address warehouseAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, shipToWarehouse.AddressID);
					boxes.Add( BuildCarrierBoxes(packSet.Packages, warehouseAddress, method, plugin.UOM));
				}
			}

			CarrierRequest cr = new CarrierRequest(unit, order.CuryID);
			cr.Shipper = companyAddress;
			cr.Origin = null;
			cr.Destination = shipAddress;
			cr.PackagesEx = boxes;
			cr.Resedential = order.Resedential == true;
			cr.SaturdayDelivery = order.SaturdayDelivery == true;
			cr.Insurance = order.Insurance == true;
			cr.ShipDate = order.ShipDate.Value;
			cr.Methods = methods;
			cr.Attributes = new List<string>();
			cr.InvoiceLineTotal = Document.Current.CuryLineTotal.GetValueOrDefault();

			if (order.GroundCollect == true)
			{
				cr.Attributes.Add("COLLECT");
			}

			return cr;
		}

		private CarrierBoxEx BuildCarrierBoxes(List<SOPackageInfoEx> list, Address origin, string method, string pluginUOM)
		{
			List<CarrierBox> boxes = new List<CarrierBox>();
			foreach (SOPackageInfoEx boxInfo in list)
			{
				decimal weightInStandardUnit = CarrierMaint.ConvertGlobalUnits(this, insetup.Current.WeightUOM, pluginUOM, boxInfo.GrossWeight ?? 0);

				CarrierBox box = new CarrierBox(0, weightInStandardUnit);
				box.DeclaredValue = boxInfo.DeclaredValue.GetValueOrDefault();
				box.CarrierPackage = boxInfo.CarrierBox;
				box.Length = boxInfo.Length ?? 0;
				box.Width = boxInfo.Width ?? 0;
				box.Height = boxInfo.Height ?? 0;
				if (boxInfo.COD == true)
				{
					box.COD = boxInfo.DeclaredValue ?? 1;
				}
				
				boxes.Add(box);
			}

			CarrierBoxEx result = new CarrierBoxEx(0, 0);
			result.Packages = boxes;
			result.Method = method;
			result.Origin = origin;
			
			return result;
		}

		

		private void CalculateFreightCost(bool supressErrors)
		{
			if (Document.Current.ShipVia != null)
			{
				Carrier carrier = PXSelect<Carrier, Where<Carrier.carrierID, Equal<Current<SOOrder.shipVia>>>>.Select(this);
				if (carrier != null && carrier.IsExternal == true)
				{
					CarrierPlugin plugin = PXSelect<CarrierPlugin, Where<CarrierPlugin.carrierPluginID, Equal<Required<CarrierPlugin.carrierPluginID>>>>.Select(this, carrier.CarrierPluginID);
					ICarrierService cs = CarrierPluginMaint.CreateCarrierService(this, plugin);
					cs.Method = carrier.PluginMethod;

					CarrierRequest cr = BuildRateRequest(Document.Current);
					CarrierResult<RateQuote> result = cs.GetRateQuote(cr);

					if (result != null)
					{
						StringBuilder sb = new StringBuilder();
						foreach (PX.Data.Message message in result.Messages)
						{
							sb.AppendFormat("{0}:{1} ", message.Code, message.Description);
						}

						if (result.IsSuccess)
						{
							decimal baseCost = ConvertAmtToBaseCury(result.Result.Currency, arsetup.Current.DefaultRateTypeID, Document.Current.OrderDate.Value, result.Result.Amount);
							SetFreightCost(baseCost);

							//show warnings:
							if (!supressErrors && result.Messages.Count > 0)
							{
								Document.Cache.RaiseExceptionHandling<SOOrder.curyFreightCost>(Document.Current, Document.Current.CuryFreightCost,
									new PXSetPropertyException(sb.ToString(), PXErrorLevel.Warning));
							}

						}
						else if (!supressErrors)
						{
							Document.Cache.RaiseExceptionHandling<SOOrder.curyFreightCost>(Document.Current, Document.Current.CuryFreightCost,
									new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, sb.ToString()));

							throw new PXException(Messages.CarrierServiceError, sb.ToString());
						}

					}
				}
			}
		}
		
		private List<string> GetCarrierMethods(string carrierPluginID)
		{
			List<string> methods = new List<string>();
			PXSelectBase<Carrier> selectMethods = new PXSelect<Carrier, Where<Carrier.carrierPluginID, Equal<Required<Carrier.carrierPluginID>>, And<Carrier.isExternal, Equal<True>>>>(this);
			foreach (Carrier method in selectMethods.Select(carrierPluginID))
			{
				if (!string.IsNullOrEmpty(method.PluginMethod))
				{
					if (!methods.Contains(method.PluginMethod))
						methods.Add(method.PluginMethod);
				}
			}

			return methods;
		}
		
		public virtual FreightCalculator CreateFreightCalculator()
		{
			return new FreightCalculator(this);
		}

		protected virtual void SetFreightCost(decimal baseCost)
		{
			SOOrder copy = (SOOrder)Document.Cache.CreateCopy(Document.Current);

			if (soordertype.Current != null && soordertype.Current.CalculateFreight == false)
			{
				copy.FreightCost = 0;
				CM.PXCurrencyAttribute.CuryConvCury<SOOrder.curyFreightCost>(Document.Cache, copy);
			}
			else
			{
				if (!CollectFreight)
					baseCost = 0;

				copy.FreightCost = baseCost;
				CM.PXCurrencyAttribute.CuryConvCury<SOOrder.curyFreightCost>(Document.Cache, copy);
				PXResultset<SOLine> res = Transactions.Select();
				FreightCalculator fc = CreateFreightCalculator();
				fc.ApplyFreightTerms<SOOrder, SOOrder.curyFreightAmt>(Document.Cache, copy, res.Count);
			}

			copy.FreightCostIsValid = true;
			Document.Update(copy);
		}
		
		private decimal ConvertAmt(string from, string to, string rateType, DateTime effectiveDate, decimal amount)
		{
			if (from == to)
				return amount;

			decimal result = amount;
			
			using (ReadOnlyScope rs = new ReadOnlyScope(DummyCuryInfo.Cache))
			{

				if (from == Accessinfo.BaseCuryID)
				{
					CurrencyInfo ci = new CurrencyInfo();
					ci.CuryRateTypeID = rateType;
					ci.CuryID = to;
					ci = (CurrencyInfo)DummyCuryInfo.Cache.Insert(ci);
					ci.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
					DummyCuryInfo.Cache.Update(ci);
					PXCurrencyAttribute.CuryConvCury(DummyCuryInfo.Cache, ci, amount, out result);
					DummyCuryInfo.Cache.Delete(ci);
				}
				else if (to == Accessinfo.BaseCuryID)
				{
					CurrencyInfo ci = new CurrencyInfo();
					ci.CuryRateTypeID = rateType;
					ci.CuryID = from;
					ci = (CurrencyInfo)DummyCuryInfo.Cache.Insert(ci);
					ci.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
					DummyCuryInfo.Cache.Update(ci);
					PXCurrencyAttribute.CuryConvBase(DummyCuryInfo.Cache, ci, amount, out result);
					DummyCuryInfo.Cache.Delete(ci);
				}
				else
				{

					CurrencyInfo ciFrom = new CurrencyInfo();
					ciFrom.CuryRateTypeID = rateType;
					ciFrom.CuryID = from;
					ciFrom = (CurrencyInfo)DummyCuryInfo.Cache.Insert(ciFrom);
					ciFrom.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
					DummyCuryInfo.Cache.Update(ciFrom);
					decimal inBase;
					PXCurrencyAttribute.CuryConvBase(DummyCuryInfo.Cache, ciFrom, amount, out inBase, true);

					CurrencyInfo ciTo = new CurrencyInfo();
					ciTo.CuryRateTypeID = rateType;
					ciTo.CuryID = to;
					ciTo = (CurrencyInfo)DummyCuryInfo.Cache.Insert(ciTo);
					ciTo.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
					DummyCuryInfo.Cache.Update(ciFrom);
					PXCurrencyAttribute.CuryConvCury(DummyCuryInfo.Cache, ciTo, inBase, out result, true);

					DummyCuryInfo.Cache.Delete(ciFrom);
					DummyCuryInfo.Cache.Delete(ciTo);
				}
			}

			return result;
		}

		private decimal ConvertAmtToBaseCury(string from, string rateType, DateTime effectiveDate, decimal amount)
		{
			decimal result = amount;

			using (ReadOnlyScope rs = new ReadOnlyScope(DummyCuryInfo.Cache))
			{
				CurrencyInfo ci = new CurrencyInfo();
				ci.CuryRateTypeID = rateType;
				ci.CuryID = from;
				object newval;
				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.baseCuryID>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.baseCuryID>(ci, newval);

				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.basePrecision>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.basePrecision>(ci, newval);

				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.curyPrecision>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.curyPrecision>(ci, newval);

				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.curyRate>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.curyRate>(ci, newval);

				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.recipRate>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.recipRate>(ci, newval);
				
				DummyCuryInfo.Cache.RaiseFieldDefaulting<CurrencyInfo.curyMultDiv>(ci, out newval);
				DummyCuryInfo.Cache.SetValue<CurrencyInfo.curyMultDiv>(ci, newval);

				ci.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
				PXCurrencyAttribute.CuryConvBase(DummyCuryInfo.Cache, ci, amount, out result);
			}

			return result;
		}

		#endregion


		#region Packaging into boxes

		protected virtual IList<SOPackageEngine.PackSet> CalculatePackages(SOOrder order, string carrierID)
		{
			Dictionary<string, SOPackageEngine.ItemStats> stats = new Dictionary<string, SOPackageEngine.ItemStats>();

			PXSelectBase<SOLine> selectLines = new PXSelectJoin<SOLine,
						InnerJoin<InventoryItem, On<SOLine.inventoryID, Equal<InventoryItem.inventoryID>>>,
						Where<SOLine.orderType, Equal<Required<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Required<SOOrder.orderNbr>>>>
						, OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>>(this);

			SOPackageEngine.OrderInfo orderInfo = new SOPackageEngine.OrderInfo(carrierID);

			foreach (PXResult<SOLine, InventoryItem> res in selectLines.Select(order.OrderType, order.OrderNbr))
			{
				SOLine line = (SOLine) res;
				InventoryItem item = (InventoryItem) res;

				if (item.PackageOption == INPackageOption.Manual)
					continue; 
				
				orderInfo.AddLine(item, line.BaseQty);

				int inventoryID = SOPackageEngine.ItemStats.Mixed;
				if (item.PackSeparately == true)
				{
					inventoryID = line.InventoryID.Value;
				}

				string key = string.Format("{0}.{1}.{2}.{3}", line.SiteID, inventoryID, item.PackageOption, line.Operation);

				SOPackageEngine.ItemStats stat;
				if (stats.ContainsKey(key))
				{
					stat = stats[key];
					stat.BaseQty += line.BaseQty.GetValueOrDefault();
					stat.BaseWeight += line.ExtWeight.GetValueOrDefault();
					stat.DeclaredValue += line.CuryLineAmt.GetValueOrDefault();
					stat.AddLine(item, line.BaseQty);
				}
				else
				{
					stat = new SOPackageEngine.ItemStats();
					stat.SiteID = line.SiteID;
					stat.InventoryID = inventoryID;
					stat.Operation = line.Operation;
					stat.PackOption = item.PackageOption;
					stat.BaseQty += line.BaseQty.GetValueOrDefault();
					stat.BaseWeight += line.ExtWeight.GetValueOrDefault();
					stat.DeclaredValue += line.CuryLineAmt.GetValueOrDefault();
					stat.AddLine(item, line.BaseQty);
					stats.Add(key, stat);
				}
			}
			orderInfo.Stats.AddRange(stats.Values); 
			

			SOPackageEngine engine = CreatePackageEngine();
			return engine.Pack(orderInfo);
		}

		protected virtual IList<SOPackageEngine.PackSet> GetManualPackages(SOOrder order)
		{
			Dictionary<int, SOPackageEngine.PackSet> packs = new Dictionary<int, SOPackageEngine.PackSet>();

			foreach (SOPackageInfoEx pack in Packages.View.SelectMultiBound(new object[]{order}))
			{
				SOPackageEngine.PackSet ps = null;
				if (packs.ContainsKey(pack.SiteID.Value))
				{
					ps = packs[pack.SiteID.Value];
				}
				else
				{
					ps = new SOPackageEngine.PackSet(pack.SiteID.Value);
					packs.Add(ps.SiteID, ps);
				}

				ps.Packages.Add(pack);
			}

			return packs.Values.ToList();
		}
		
		protected virtual SOPackageEngine CreatePackageEngine()
		{
			return new SOPackageEngine(this);
		}

		protected virtual void RecalculatePackagesForOrder(SOOrder order)
		{
			if (order == null)
				throw new ArgumentNullException("order");

			if (string.IsNullOrEmpty(order.ShipVia))
			{
				throw new PXException(Messages.ShipViaMustBeSet);
			}

			foreach (SOPackageInfoEx package in Packages.View.SelectMultiBound(new object[] { order }))
			{
				Packages.Delete(package);
			}

			IList<SOPackageEngine.PackSet> packsets = CalculatePackages(order, order.ShipVia);
			foreach (SOPackageEngine.PackSet ps in packsets)
			{
				if (ps.Packages.Count > 1000)
				{
					PXTrace.WriteWarning("During autopackaging more than 1000 packages were generated. Please check your configuration.");
				}

				foreach (SOPackageInfoEx package in ps.Packages)
				{
					Packages.Insert(package);
				}
			}
			if (packsets.Count > 0)
			order.IsPackageValid = true;
		}
						
		#endregion

		#region Avalara Tax

		public bool IsExternalTax
		{
			get
			{
				TX.TaxZone tz = PXSelect<TX.TaxZone, Where<TX.TaxZone.taxZoneID, Equal<Current<SOOrder.taxZoneID>>>>.Select(this);
				if (tz != null)
					return tz.IsExternal.GetValueOrDefault(false);
				else
					return false;
			}
		}

		public virtual void CalculateAvalaraTax(SOOrder order)
		{
			if (IsExternalTax)
				CalculateAvalaraTax(order, false);
		}

		public virtual void CalculateAvalaraTax(SOOrder order, bool forceRecalculate)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Debug.Print("{0} Enter CalculateAvalaraTax", DateTime.Now.TimeOfDay);
			Debug.Indent();

			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);

			GetTaxRequest getRequest = null;
			GetTaxRequest getRequestOpen = null;
			GetTaxRequest getRequestUnbilled = null;
			GetTaxRequest getRequestFreight = null;

			bool isValidByDefault = true;

			SOOrderType orderType = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrder.orderType>>>>.Select(this, order.OrderType);

			if (orderType.INDocType != INTranType.Transfer)
			{
				if (order.IsTaxValid != true || forceRecalculate)
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

				if (order.IsOpenTaxValid != true || forceRecalculate)
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

				if (order.IsUnbilledTaxValid != true || forceRecalculate)
				{
					getRequestUnbilled = BuildGetTaxRequestUnbilled(order);
					if (getRequestUnbilled.Lines.Count > 0)
					{
						isValidByDefault = false;
					}
					else
					{
						getRequestUnbilled = null;
					}
				}

				if (order.IsFreightTaxValid != true || forceRecalculate)
				{
					getRequestFreight = BuildGetTaxRequestFreight(order);
					if (getRequestFreight.Lines.Count > 0)
					{
						isValidByDefault = false;
					}
					else
					{
						getRequestFreight = null;
					}
				}
			}

			if (isValidByDefault)
			{
				//PXDatabase.Update<SOOrder>(
				//	new PXDataFieldAssign("IsTaxValid", true),
				//	new PXDataFieldAssign("IsOpenTaxValid", true),
				//	new PXDataFieldAssign("IsUnbilledTaxValid", true),
				//	new PXDataFieldAssign("IsFreightTaxValid", true),
				//	new PXDataFieldRestrict("OrderType", PXDbType.Char, 2, order.OrderType, PXComp.EQ),
				//	new PXDataFieldRestrict("OrderNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ)
				//	);
				order.IsTaxValid = true;
				order.IsOpenTaxValid = true;
				order.IsUnbilledTaxValid = true;
				order.IsFreightTaxValid = true;
				//PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());
				Document.Update(order);

				using (var ts = new PXTransactionScope())
				{
				this.Persist(typeof(SOOrder), PXDBOperation.Update);
					PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());
					ts.Complete();
				}
				return;
			}

			GetTaxResult result = null;
			GetTaxResult resultOpen = null;
			GetTaxResult resultUnbilled = null;
			GetTaxResult resultFreight = null;

			bool getTaxFailed = false;
			if (getRequest != null)
			{
				Stopwatch sw2 = new Stopwatch();
				sw2.Start();
				result = service.GetTax(getRequest);
				sw2.Stop();
				Debug.Print("{0} GetTax(request) in {1} millisec", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);

				if (result.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}
			if (getRequestOpen != null)
			{
				if (getRequest != null && IsSame(getRequest, getRequestOpen))
				{
					getRequestOpen = getRequest;
					resultOpen = result;
				}
				else
				{
					Stopwatch sw2 = new Stopwatch();
					sw2.Start();
					resultOpen = service.GetTax(getRequestOpen);
					sw2.Stop();
					Debug.Print("{0} GetTax(requestOpen) in {1} millisec", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);
					if (resultOpen.ResultCode != SeverityLevel.Success)
					{
						getTaxFailed = true;
					}
				}
			}
			if (getRequestUnbilled != null)
			{
				if (getRequest != null && IsSame(getRequest, getRequestUnbilled))
				{
					getRequestUnbilled = getRequest;
					resultUnbilled = result;
				}
				else
				{
					Stopwatch sw2 = new Stopwatch();
					sw2.Start();
					resultUnbilled = service.GetTax(getRequestUnbilled);
					sw2.Stop();
					Debug.Print("{0} GetTax(requestUnbilled) in {1} millisec", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);
					if (resultUnbilled.ResultCode != SeverityLevel.Success)
					{
						getTaxFailed = true;
					}
				}
			}
			if (getRequestFreight != null)
			{
				Stopwatch sw2 = new Stopwatch();
				sw2.Start();
				resultFreight = service.GetTax(getRequestFreight);
				sw2.Stop();
				Debug.Print("{0} GetTax(requestFreight) in {1} millisec", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);
				if (resultFreight.ResultCode != SeverityLevel.Success)
				{
					getTaxFailed = true;
				}
			}

			if (!getTaxFailed)
			{
				try
				{
					ApplyAvalaraTax(order, result, resultOpen, resultUnbilled, resultFreight);
					Stopwatch sw2 = new Stopwatch();
					sw2.Start();
					//PXDatabase.Update<SOOrder>(
					//	new PXDataFieldAssign("IsTaxValid", true),
					//	new PXDataFieldAssign("IsOpenTaxValid", true),
					//	new PXDataFieldAssign("IsUnbilledTaxValid", true),
					//	new PXDataFieldAssign("IsFreightTaxValid", true),
					//	new PXDataFieldRestrict("OrderType", PXDbType.Char, 3, order.OrderType, PXComp.EQ),
					//	new PXDataFieldRestrict("OrderNbr", PXDbType.NVarChar, 15, order.OrderNbr, PXComp.EQ)
					//	);
					order.IsTaxValid = true;
					order.IsOpenTaxValid = true;
					order.IsUnbilledTaxValid = true;
					order.IsFreightTaxValid = true;
					//PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());

					sw2.Stop();
					Debug.Print("{0} PXDatabase.Update<SOOrder> in {1} millisec", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);
					using (var ts = new PXTransactionScope())
					{
					Document.Update(order);
					this.Persist(typeof(SOOrder), PXDBOperation.Update);
						PXTimeStampScope.PutPersisted(this.Document.Cache, order, PXDatabase.SelectTimeStamp());
						ts.Complete();
					}
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

			sw.Stop();
			Debug.Unindent();
			Debug.Print("{0} Exit CalculateAvalaraTax in {1} millisec", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);
		}

		protected virtual GetTaxRequest BuildGetTaxRequest(SOOrder order)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Debug.Indent();

			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);


			Customer cust = (Customer)customer.View.SelectSingleBound(new object[] { order });
			Location loc = (Location)location.View.SelectSingleBound(new object[] { order });

			IAddressBase fromAddress = GetFromAddress(order);
			IAddressBase toAddress = GetToAddress(order);

			Debug.Print("{0} Select Customer, Location, Addresses in {1} millisec", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);

			if ( fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);
			
			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, order.BranchID);
			request.CurrencyCode = order.CuryID;
			request.CustomerCode = cust.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("SO.{0}.{1}", order.OrderType, order.OrderNbr);
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(order.AvalaraCustomerUsageType))
			{
				request.CustomerUsageType = order.AvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			SOOrderType orderType = (SOOrderType) soordertype.View.SelectSingleBound(new object[] {order});
			
			if ( orderType.DefaultOperation == SOOperation.Receipt )
			{
				request.DocType = DocumentType.ReturnOrder;
				mult = -1;

				PXSelectBase<SOLine> selectLineWithInvoiceDate = new PXSelect<SOLine,
				Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
				And<SOLine.invoiceDate, IsNotNull>>>>(this);

				SOLine soLine = selectLineWithInvoiceDate.SelectSingle(order.OrderType, order.OrderNbr);
				if (soLine != null && soLine.TranDate != null)
				{
					request.TaxOverride.Reason = "Return";
					request.TaxOverride.TaxDate = soLine.TranDate.Value;
					request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
				}

			}
			else
			{
				request.DocType = DocumentType.SalesOrder;
			}

			
			PXSelectBase<SOLine> select = new PXSelectJoin<SOLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<SOLine.salesAcctID>>>>,
				Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>,
				OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>>(this);

			request.Discount = order.CuryDiscTot.GetValueOrDefault();

			Stopwatch sw2 = new Stopwatch();
			sw2.Start();
			foreach (PXResult<SOLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				SOLine tran = (SOLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);

				if (orderType.DefaultOperation != tran.Operation)
					line.Amount = -1 * mult * tran.CuryLineAmt.GetValueOrDefault();
				else
				line.Amount = mult * tran.CuryLineAmt.GetValueOrDefault();

				line.Description = tran.TranDesc;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Math.Abs(Convert.ToDouble(tran.Qty.GetValueOrDefault()));
				line.Discounted = request.Discount > 0;
				line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;
				
				if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
					line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;

				request.Lines.Add(line);
			}
			sw2.Stop();
			Debug.Print("{0} Select detail lines in {1} millisec.", DateTime.Now.TimeOfDay, sw2.ElapsedMilliseconds);

			Debug.Unindent();
			sw.Stop();
			Debug.Print("{0} BuildGetTaxRequest() in {1} millisec.", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);
			
			return request;
		}

		protected virtual GetTaxRequest BuildGetTaxRequestOpen(SOOrder order)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Customer cust = (Customer)customer.View.SelectSingleBound(new object[] { order });
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
			request.CustomerCode = cust.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("SO.{0}.{1}", order.OrderType, order.OrderNbr);
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(order.AvalaraCustomerUsageType))
			{
				request.CustomerUsageType = order.AvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			SOOrderType orderType = (SOOrderType)soordertype.View.SelectSingleBound(new object[] { order });

			if (orderType.DefaultOperation == SOOperation.Receipt)
			{
				request.DocType = DocumentType.ReturnOrder;
				mult = -1;

				PXSelectBase<SOLine> selectLineWithInvoiceDate = new PXSelect<SOLine,
				Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
				And<SOLine.invoiceDate, IsNotNull>>>>(this);

				SOLine soLine = selectLineWithInvoiceDate.SelectSingle(order.OrderType, order.OrderNbr);
				if (soLine != null && soLine.TranDate != null)
				{
					request.TaxOverride.Reason = "Return";
					request.TaxOverride.TaxDate = soLine.TranDate.Value;
					request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
				}

			}
			else
			{
				request.DocType = DocumentType.SalesOrder;
			}


			PXSelectBase<SOLine> select = new PXSelectJoin<SOLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<SOLine.salesAcctID>>>>,
				Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>,
				OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>>(this);

			request.Discount = order.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<SOLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				SOLine tran = (SOLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				if (tran.OpenAmt > 0)
				{
					Line line = new Line();
					line.No = Convert.ToString(tran.LineNbr);
					if (orderType.DefaultOperation != tran.Operation)
						line.Amount = -1 * mult * tran.CuryOpenAmt.GetValueOrDefault();
					else
					line.Amount = mult * tran.CuryOpenAmt.GetValueOrDefault();
					line.Description = tran.TranDesc;
					line.DestinationAddress = request.DestinationAddress;
					line.OriginAddress = request.OriginAddress;
					line.ItemCode = item.InventoryCD;
					line.Qty = Math.Abs(Convert.ToDouble(tran.OpenQty.GetValueOrDefault()));
					line.Discounted = request.Discount > 0;
					line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

					if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
						line.RevAcct = salesAccount.AccountCD;

					line.TaxCode = tran.TaxCategoryID;

					request.Lines.Add(line);
				}
			}

			sw.Stop();
			Debug.Print("BuildGetTaxRequestOpen() in {0} millisec.", sw.ElapsedMilliseconds);

			return request;
		}

		protected virtual GetTaxRequest BuildGetTaxRequestUnbilled(SOOrder order)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Customer cust = (Customer)customer.View.SelectSingleBound(new object[] { order });
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
			request.CustomerCode = cust.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("{0}.{1}.Open", order.OrderType, order.OrderNbr);
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(order.AvalaraCustomerUsageType))
			{
				request.CustomerUsageType = order.AvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			SOOrderType orderType = (SOOrderType)soordertype.View.SelectSingleBound(new object[] { order });

			if (orderType.DefaultOperation == SOOperation.Receipt)
			{
				request.DocType = DocumentType.ReturnOrder;
				mult = -1;

				PXSelectBase<SOLine> selectLineWithInvoiceDate = new PXSelect<SOLine,
				Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
				And<SOLine.invoiceDate, IsNotNull>>>>(this);

				SOLine soLine = selectLineWithInvoiceDate.SelectSingle(order.OrderType, order.OrderNbr);
				if (soLine != null && soLine.TranDate != null)
				{
					request.TaxOverride.Reason = "Return";
					request.TaxOverride.TaxDate = soLine.TranDate.Value;
					request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
				}

			}
			else
			{
				request.DocType = DocumentType.SalesOrder;
			}


			PXSelectBase<SOLine> select = new PXSelectJoin<SOLine,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<SOLine.salesAcctID>>>>,
				Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>,
				OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>>(this);

			request.Discount = order.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<SOLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				SOLine tran = (SOLine)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				if (tran.UnbilledAmt > 0)
				{
					Line line = new Line();
					line.No = Convert.ToString(tran.LineNbr);
					if (orderType.DefaultOperation != tran.Operation)
						line.Amount = -1 * mult * tran.CuryUnbilledAmt.GetValueOrDefault();
					else
					line.Amount = mult * tran.CuryUnbilledAmt.GetValueOrDefault();
					line.Description = tran.TranDesc;
					line.DestinationAddress = request.DestinationAddress;
					line.OriginAddress = request.OriginAddress;
					line.ItemCode = item.InventoryCD;
					line.Qty = Math.Abs(Convert.ToDouble(tran.UnbilledQty.GetValueOrDefault()));
					line.Discounted = request.Discount > 0;
					line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

					if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
						line.RevAcct = salesAccount.AccountCD;

					line.TaxCode = tran.TaxCategoryID;

					request.Lines.Add(line);
				}
			}

			sw.Stop();
			Debug.Print("BuildGetTaxRequestUnbilled() in {0} millisec.", sw.ElapsedMilliseconds);

			return request;
		}

		protected virtual GetTaxRequest BuildGetTaxRequestFreight(SOOrder order)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Customer cust = (Customer)customer.View.SelectSingleBound(new object[] { order });
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
			request.CustomerCode = cust.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("{0}.{1}.Freight", order.OrderType, order.OrderNbr);
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(order);

			int mult = 1;

			if (!string.IsNullOrEmpty(order.AvalaraCustomerUsageType))
			{
				request.CustomerUsageType = order.AvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			SOOrderType orderType = (SOOrderType)soordertype.View.SelectSingleBound(new object[] { order });

			if (orderType.ARDocType == ARDocType.CreditMemo)
			{
				request.DocType = DocumentType.ReturnOrder;
				mult = -1;
			}
			else
			{
				request.DocType = DocumentType.SalesOrder;
			}

			if (order.CuryFreightTot > 0)
			{
				Line line = new Line();
				line.No = short.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
				line.Amount = mult * order.CuryFreightTot.GetValueOrDefault();
				line.Description = "Freight";
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = "N/A";
				line.Discounted = false;
				line.TaxCode = order.FreightTaxCategoryID;

				request.Lines.Add(line);
			}
			
			sw.Stop();
			Debug.Print("BuildGetTaxRequestFreight() in {0} millisec.", sw.ElapsedMilliseconds);

			return request;
		}

		protected bool SkipAvalaraTaxProcessing = false;
		protected virtual void ApplyAvalaraTax(SOOrder order, GetTaxResult result, GetTaxResult resultOpen, GetTaxResult resultUnbilled, GetTaxResult resultFreight)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { order });
			if (taxZone == null)
			{
				throw new PXException(Messages.TaxZoneIsNotSet);
			}

			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			if (result != null)
			{
				//Clear all existing Tax transactions:
				PXSelectBase<SOTaxTran> TaxesSelect = new PXSelectJoin<SOTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<SOTaxTran.taxID>>>, Where<SOTaxTran.orderType, Equal<Current<SOOrder.orderType>>, And<SOTaxTran.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>(this);
				foreach (PXResult<SOTaxTran, Tax> res in TaxesSelect.View.SelectMultiBound(new object[] { order }))
				{
					SOTaxTran taxTran = (SOTaxTran)res;
					Taxes.Delete(taxTran);
				}
				
				this.Views.Caches.Add(typeof(Tax));

				decimal freightTax = 0;
				if (resultFreight != null)
					freightTax = Math.Abs(resultFreight.TotalTax);

				bool requireControlTotal = soordertype.Current.RequireControlTotal == true;

				if (order.Hold != true)
					soordertype.Current.RequireControlTotal = false;

				List<Avalara.AvaTax.Adapter.TaxService.TaxDetail> taxDetails = new List<Avalara.AvaTax.Adapter.TaxService.TaxDetail>();
				for (int i = 0; i < result.TaxSummary.Count; i++)
					taxDetails.Add(result.TaxSummary[i]);

				if (resultFreight != null)
				{
					for (int i = 0; i < resultFreight.TaxSummary.Count; i++)
						taxDetails.Add(resultFreight.TaxSummary[i]);
				}

				try
				{
					foreach (var taxDetail in taxDetails)
					{
						string taxID = taxDetail.TaxName;
						if (string.IsNullOrEmpty(taxID))
							taxID = taxDetail.JurisCode;

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
							tx.Descr = string.Format("Avalara {0} tax for {1}", taxDetail.JurisType, taxDetail.JurisName);
							tx.TaxType = CSTaxType.Sales;
							tx.TaxCalcType = CSTaxCalcType.Doc;
							tx.TaxCalcLevel = CSTaxCalcLevel.CalcOnItemAmt;
							tx.TaxApplyTermsDisc = CSTaxTermsDiscount.ToTaxableAmount;
							tx.SalesTaxAcctID = vendor.SalesTaxAcctID;
							tx.SalesTaxSubID = vendor.SalesTaxSubID;
							tx.ExpenseAccountID = vendor.TaxExpenseAcctID;
							tx.ExpenseSubID = vendor.TaxExpenseSubID;
							tx.TaxVendorID = taxZone.TaxVendorID;
							tx.IsExternal = true;

							this.Caches[typeof(Tax)].Insert(tx);
						}

							SOTaxTran tax = new SOTaxTran();
							tax.OrderType = order.OrderType;
							tax.OrderNbr = order.OrderNbr;
							tax.TaxID = taxID;
							tax.CuryTaxAmt = Math.Abs(taxDetail.Tax);
							tax.CuryTaxableAmt = Math.Abs(taxDetail.Taxable);

						tax.CuryUnshippedTaxAmt = Math.Abs(taxDetail.Tax);
						tax.CuryUnshippedTaxableAmt = Math.Abs(taxDetail.Taxable);
						tax.CuryUnbilledTaxAmt = Math.Abs(taxDetail.Tax);
						tax.CuryUnbilledTaxableAmt = Math.Abs(taxDetail.Taxable);

							tax.TaxRate = Convert.ToDecimal(taxDetail.Rate) * 100;
							tax.JurisType = taxDetail.JurisType.ToString();
							tax.JurisName = taxDetail.JurisName;

						Taxes.Insert(tax);

					}
					
					Document.SetValueExt<SOOrder.curyTaxTotal>(order, Math.Abs(result.TotalTax) + freightTax);
				}
				finally
				{
					soordertype.Current.RequireControlTotal = requireControlTotal;
				}
			}


			if (resultUnbilled != null)
				Document.SetValueExt<SOOrder.curyUnbilledTaxTotal>(order, Math.Abs(resultUnbilled.TotalTax));

			if (resultOpen != null)
				Document.SetValueExt<SOOrder.curyOpenTaxTotal>(order, Math.Abs(resultOpen.TotalTax));

			Document.Update(order);

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

		protected virtual IAddressBase GetFromAddress(SOOrder order)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(order.BranchID))
				return (Address)res;

			return null;
		}

		protected virtual IAddressBase GetToAddress(SOOrder order)
		{
			if (IsCommonCarrier(order.ShipVia))
			{
				PXSelectBase<SOShippingAddress> select =
					new PXSelect<SOShippingAddress, Where<SOShippingAddress.addressID, Equal<Required<SOOrder.shipAddressID>>>>(this);

				SOShippingAddress shipAddress = select.Select(order.ShipAddressID);

				return shipAddress;
			}
			else
			{
				return GetFromAddress(order);
			}
		}

		protected virtual string GetAvalaraLocationCode(SOOrder order)
		{
			return null;
		}


		protected virtual bool IsSame(GetTaxRequest x, GetTaxRequest y)
		{
			if (x.Lines.Count != y.Lines.Count)
				return false;

			for ( int i = 0; i < x.Lines.Count; i++)
			{
				if (x.Lines[i].Amount != y.Lines[i].Amount)
					return false;
			}

			return true;

		}

		#endregion


        public static void InvoiceOrder(Dictionary<string, object> parameters, List<SOOrder> list, DocumentList<ARInvoice, SOInvoice> created, bool isMassProcess = false)
        {
            SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
            SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();

            foreach (SOOrder order in list)
            {
                try
                {
                    if (isMassProcess) PXProcessing<SOOrder>.SetCurrentItem(order);
                    
                    ie.Clear();
                    ie.ARSetup.Current.RequireControlTotal = false;

                    List<PXResult<SOOrderShipment>> shipments = new List<PXResult<SOOrderShipment>>();
                    PXResultset<SOShipLine, SOLine> details = null;

                    foreach (PXResult<SOOrderType, SOOrderShipment, SOOrderTypeOperation, CurrencyInfo, SOAddress, SOContact, Customer> res in
                        PXSelectJoin<SOOrderType,
                        LeftJoin<SOOrderShipment,
                            On<SOOrderShipment.orderType, Equal<SOOrderType.orderType>,
                                And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                                And<SOOrderShipment.confirmed, Equal<True>,
                                And<SOOrderShipment.shipmentType, NotEqual<SOShipmentType.dropShip>,
								And<SOOrderShipment.invoiceNbr, IsNull>>>>>,
                      LeftJoin<SOOrderTypeOperation,
                                    On<SOOrderTypeOperation.orderType, Equal<SOOrderType.orderType>,
                                    And<Where2<Where<SOOrderShipment.operation, IsNull,
                                                    And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>,
                                                  Or<Where<SOOrderTypeOperation.operation, Equal<SOOrderShipment.operation>>>>>>,
                        CrossJoin<CurrencyInfo, CrossJoin<SOAddress, CrossJoin<SOContact, CrossJoin<Customer>>>>>>,
                        Where<SOOrderType.orderType, Equal<Required<SOOrder.orderType>>,
                            And<CurrencyInfo.curyInfoID, Equal<Required<SOOrder.curyInfoID>>,
                            And<SOAddress.addressID, Equal<Required<SOOrder.billAddressID>>,
                            And<SOContact.contactID, Equal<Required<SOOrder.billContactID>>,
                            And<Customer.bAccountID, Equal<Required<SOOrder.customerID>>>>>>>>
                            .Select(docgraph, order.OrderNbr, order.OrderType, order.CuryInfoID, order.BillAddressID, order.BillContactID, order.CustomerID))
                    {
                        SOOrderShipment shipment = (SOOrderShipment)res;

                        if (((SOOrderType)res).RequireShipping == false || ((SOOrderTypeOperation)res).INDocType == INTranType.NoUpdate)
                        {
                            //if order is created with zero lines, invoiced, and then new line added, this will save us
                            if (shipment.ShipmentNbr == null)
                            {
                                shipment = (SOOrderShipment)order;
                                shipment.ShipmentType = INTranType.DocType(((SOOrderTypeOperation)res).INDocType);
                            }

                            details = new PXResultset<SOShipLine, SOLine>();

                            foreach (SOLine line in PXSelectJoin<SOLine, InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>>, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineType, NotEqual<SOLineType.miscCharge>>>>>.Select(docgraph, order.OrderType, order.OrderNbr))
                            {
                                details.Add(new PXResult<SOShipLine, SOLine>((SOShipLine)line, line));
                            }
                        }
                        else if (order.OpenLineCntr == 0 && order.ShipmentCntr == 0)
                        {
                            if (shipment.ShipmentNbr == null)
                            {
                                shipment = (SOOrderShipment)order;
                                shipment.ShipmentType = INTranType.DocType(((SOOrderTypeOperation)res).INDocType);
                            }
                        }

					if (shipment.ShipmentNbr != null)
					{
                        shipments.Add(new PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation, Customer>(shipment, order, (CurrencyInfo)res, (SOAddress)res, (SOContact)res, (SOOrderType)res, (SOOrderTypeOperation)res, (Customer)res));
                    }
				}

                foreach (PXResult<SOOrderReceipt, SOOrder, SOOrderShipment, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation> res in PXSelectJoin<SOOrderReceipt,
                InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderReceipt.orderType>, And<SOOrder.orderNbr, Equal<SOOrderReceipt.orderNbr>>>,
                LeftJoin<SOOrderShipment, On<SOOrderShipment.orderType, Equal<SOOrderReceipt.orderType>, And<SOOrderShipment.orderNbr, Equal<SOOrderReceipt.orderNbr>, And<SOOrderShipment.shipmentNbr, Equal<SOOrderReceipt.receiptNbr>, And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>, And<SOOrderShipment.invoiceNbr, IsNull>>>>>,
                InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SOOrder.curyInfoID>>,
                InnerJoin<SOAddress, On<SOAddress.addressID, Equal<SOOrder.billAddressID>>,
                InnerJoin<SOContact, On<SOContact.contactID, Equal<SOOrder.billContactID>>,
                InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>,
                InnerJoin<SOOrderTypeOperation,
                             On<SOOrderTypeOperation.orderType, Equal<SOOrderType.orderType>,
                                    And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>>>>>>>>,
                Where<SOOrderReceipt.orderType, Equal<Required<SOOrderReceipt.orderType>>, And<SOOrderReceipt.orderNbr, Equal<Required<SOOrderReceipt.orderNbr>>, And<SOOrderShipment.invoiceNbr, IsNull>>>>.Select(docgraph, order.OrderType, order.OrderNbr))
                    {
                        SOOrderReceipt receipt = res;
                        SOOrderShipment shipment = res;

						if (details == null)
                        {
                            details = new PXResultset<SOShipLine, SOLine>();
                        }

                    foreach (PXResult<POReceiptLine, SOLineSplit, SOLine> line in PXSelectJoin<POReceiptLine,
                        InnerJoin<SOLineSplit, On<SOLineSplit.pOType, Equal<POReceiptLine.pOType>, And<SOLineSplit.pONbr, Equal<POReceiptLine.pONbr>, And<SOLineSplit.pOLineNbr, Equal<POReceiptLine.pOLineNbr>>>>,
                        InnerJoin<SOLine, On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>>,
                            Where2<Where<POReceiptLine.lineType, Equal<POLineType.goodsForDropShip>, Or<POReceiptLine.lineType, Equal<POLineType.nonStockForDropShip>>>, And<POReceiptLine.receiptNbr, Equal<Current<SOOrderReceipt.receiptNbr>>, And<SOLine.orderType, Equal<Current<SOOrderReceipt.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrderReceipt.orderNbr>>>>>>>.SelectMultiBound(docgraph, new object[] { receipt }))
                        {
                            details.Add(new PXResult<SOShipLine, SOLine>((SOShipLine)line, line));
                        }

                        if (shipment.ShipmentNbr == null)
                        {
                            shipment = (PXResult<SOOrderReceipt, SOOrder>)res;
                        }

                        shipments.Add(new PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation>(shipment, order, (CurrencyInfo)res, (SOAddress)res, (SOContact)res, (SOOrderType)res, (SOOrderTypeOperation)res));
                    }

					shipments = new List<PXResult<SOOrderShipment>>(shipments.OrderBy(s => PXResult.Unwrap<SOOrderShipment>(s).Operation == PXResult.Unwrap<SOOrderType>(s).DefaultOperation ? 0 : 1)
						.ThenBy(s => PXResult.Unwrap<SOOrderShipment>(s).ShipmentNbr));

                    foreach (PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation> res in shipments)
                    {
                        try
                        {
							using (var ts = new PXTransactionScope())
							{
								PXTimeStampScope.SetRecordComesFirst(typeof(SOOrder), true);
                            ie.InvoiceOrder((DateTime)ie.Accessinfo.BusinessDate, res, details, null, created);
								if (shipments.Count == 1)
								{
									PXAutomation.CompleteAction(ie);
									PXAutomation.RemovePersisted(ie, typeof(SOOrder), new List<object> { order });
								}
								ts.Complete();
							}

							PXProcessing<SOOrder>.SetProcessed();
                        }
                        catch
                        {
                            List<object> orders = new List<object>();
                            orders.Add(order);
                            PXAutomation.RemovePersisted(ie, ie.soorder.GetItemType(), orders);
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!isMassProcess)
                    {
                        throw;
                    }
                    PXProcessing<SOOrder>.SetError(ex);
                }
            }
        }

		public virtual void PostOrder(INIssueEntry docgraph, SOOrder order, DocumentList<INRegister> list)
		{
			PostOrder(docgraph, order, list, null);
		}

		public virtual void PostOrder(INIssueEntry docgraph, SOOrder order, DocumentList<INRegister> list, SOOrderShipment orderShipment)
		{
			this.Clear();
			docgraph.Clear();

			docgraph.insetup.Current.HoldEntry = false;
			docgraph.insetup.Current.RequireControlTotal = false;

			Document.Current = Document.Search<SOOrder.orderNbr>(order.OrderNbr, order.OrderType);

			if (orderShipment == null)
			{
				orderShipment = PXSelect<SOOrderShipment,
					Where<SOOrderShipment.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>.Select(this);
			}

			DateTime? invoiceDate = null;
			if (orderShipment != null)
			{
				ARRegister doc = PXSelect<ARRegister, Where<ARRegister.docType, Equal<Required<ARRegister.docType>>, And<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>>.Select(this, orderShipment.InvoiceType, orderShipment.InvoiceNbr);
				invoiceDate = doc?.DocDate;
			}

			INRegister newdoc = (this.sosetup.Current.ConsolidateIN == false
					? null
					: list.Find<INRegister.branchID, INRegister.docType, INRegister.siteID, INRegister.tranDate>(order.BranchID, INTranType.DocType(sooperation.Current.INDocType), null, invoiceDate))
					?? new INRegister();

			if (newdoc.RefNbr != null)
			{
				docgraph.issue.Current = docgraph.issue.Search<INRegister.refNbr>(newdoc.RefNbr);
			}
			else
			{
				newdoc.BranchID = order.BranchID;
				newdoc.DocType = INTranType.DocType(sooperation.Current.INDocType);
				newdoc.SiteID = null;
				newdoc.TranDate = invoiceDate;
				newdoc.OrigModule = GL.BatchModule.SO;

				if (docgraph.issue.Insert(newdoc) == null)
				{
					return;
				}
			}

			SOLine prev_line = null;
			ARTran prev_artran = null;
			INTran newline = null;

			foreach (PXResult<SOLine, SOLineSplit, SOOrderType, ARTran, INTran, INItemPlan, INPlanType> res in
				PXSelectJoin<SOLine,
				LeftJoin<SOLineSplit, On<SOLineSplit.orderType, Equal<SOLine.orderType>, And<SOLineSplit.orderNbr, Equal<SOLine.orderNbr>, And<SOLineSplit.lineNbr, Equal<SOLine.lineNbr>>>>,
				InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOLine.orderType>>,
				LeftJoin<ARTran, On<ARTran.sOOrderType, Equal<SOLine.orderType>, And<ARTran.sOOrderNbr, Equal<SOLine.orderNbr>, And<ARTran.sOOrderLineNbr, Equal<SOLine.lineNbr>, And<ARTran.lineType, Equal<SOLine.lineType>>>>>,
				LeftJoin<INTran, On<INTran.sOOrderType, Equal<SOLine.orderType>, And<INTran.sOOrderNbr, Equal<SOLine.orderNbr>, And<INTran.sOOrderLineNbr, Equal<SOLine.lineNbr>>>>,
				LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<SOLineSplit.planID>>,
				LeftJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>>>>>>>,
			Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>, And<SOLine.lineType, Equal<SOLineType.inventory>,
				And2<Where<SOOrderType.aRDocType, Equal<ARDocType.noUpdate>,
						Or<ARTran.tranType, Equal<SOOrderType.aRDocType>,
								And<ARTran.refNbr, IsNotNull>>>, And<INTran.refNbr, IsNull>>>>>,
			OrderBy<Asc<SOLine.orderType, Asc<SOLine.orderNbr, Asc<SOLine.lineNbr>>>>>.Select(this))
			{
				SOLine line = res;
				SOLineSplit split = ((SOLineSplit)res).SplitLineNbr != null ? (SOLineSplit)res : (SOLineSplit)line;
				INItemPlan plan = res;
				INPlanType plantype = res;
				ARTran artran = res;
				SOOrderType ordertype = res;

				//avoid ReadItem()
				if (plan.PlanID != null)
				{
					Caches[typeof(INItemPlan)].SetStatus(plan, PXEntryStatus.Notchanged);
				}

				if (plantype.DeleteOnEvent == true)
				{
					Caches[typeof(INItemPlan)].Delete(plan);
					Caches[typeof(SOLineSplit)].SetStatus(split, PXEntryStatus.Updated);
                    split = (SOLineSplit)Caches[typeof(SOLineSplit)].Locate(split);
                    if (split != null) split.PlanID = null;
					Caches[typeof(SOLineSplit)].IsDirty = true;
				}
				else if (string.IsNullOrEmpty(plantype.ReplanOnEvent) == false)
				{
					plan = PXCache<INItemPlan>.CreateCopy(plan);
					plan.PlanType = plantype.ReplanOnEvent;
					Caches[typeof(INItemPlan)].Update(plan);

					//split.Confirmed = true;
					Caches[typeof(SOLineSplit)].SetStatus(split, PXEntryStatus.Updated);
					Caches[typeof(SOLineSplit)].IsDirty = true;
				}

				if ((Caches[typeof(SOLine)].ObjectsEqual(prev_line, line) == false || object.Equals(line.InventoryID, split.InventoryID) == false) && split.IsStockItem == true)
				{
					newline = new INTran();
					newline.BranchID = line.BranchID;
					newline.TranType = line.TranType;
					newline.SOShipmentNbr = Constants.NoShipmentNbr;
					newline.SOShipmentType = docgraph.issue.Current.DocType;
					newline.SOShipmentLineNbr = null;
					newline.SOOrderType = line.OrderType;
					newline.SOOrderNbr = line.OrderNbr;
					newline.SOOrderLineNbr = line.LineNbr;
					newline.ARDocType = artran.TranType;
					newline.ARRefNbr = artran.RefNbr;
					newline.ARLineNbr = artran.LineNbr;
					newline.AcctID = artran.AccountID;
					newline.SubID = artran.SubID;

					newline.InventoryID = split.InventoryID;
					newline.SiteID = line.SiteID;
					newline.BAccountID = line.CustomerID;
					newline.InvtMult = line.InvtMult;
					newline.Qty = 0m;
					newline.ProjectID = line.ProjectID;
					newline.TaskID = line.TaskID;
					if (object.Equals(line.InventoryID, split.InventoryID) == false)
					{
						newline.SubItemID = split.SubItemID;
						newline.UOM = split.UOM;
						newline.UnitPrice = 0m;
						newline.UnitCost = 0m;
						newline.TranDesc = null;
					}
					else
					{
						newline.SubItemID = line.SubItemID;
						newline.UOM = line.UOM;
						newline.UnitPrice = artran.UnitPrice ?? 0m;
						newline.UnitCost = line.UnitCost;
						newline.TranDesc = line.TranDesc;
						newline.ReasonCode = line.ReasonCode;
					}

					newline = docgraph.lsselect.Insert(newline);
				}

				prev_line = line;
				prev_artran = artran;

				if (split.IsStockItem == true)
				{
					INTranSplit newsplit = (INTranSplit)newline;
					newsplit.SplitLineNbr = null;
					newsplit.SubItemID = split.SubItemID;
					newsplit.LocationID = split.LocationID;
					newsplit.LotSerialNbr = split.LotSerialNbr;
					newsplit.ExpireDate = split.ExpireDate;
					newsplit.UOM = split.UOM;
					newsplit.Qty = split.Qty;
					newsplit.BaseQty = null;

					docgraph.splits.Insert(newsplit);

					if (object.Equals(line.InventoryID, split.InventoryID))
					{
						newline.TranCost = prev_line.ExtCost;
						newline.TranAmt = string.Equals(ordertype.DefaultOperation, line.Operation) ? prev_artran.TranAmt ?? 0m : -prev_artran.TranAmt ?? 0m;
					}
				}
			}

			INRegister copy = PXCache<INRegister>.CreateCopy(docgraph.issue.Current);
			PXFormulaAttribute.CalcAggregate<INTran.qty>(docgraph.transactions.Cache, copy);
			PXFormulaAttribute.CalcAggregate<INTran.tranAmt>(docgraph.transactions.Cache, copy);
			PXFormulaAttribute.CalcAggregate<INTran.tranCost>(docgraph.transactions.Cache, copy);
			docgraph.issue.Update(copy);

			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					if (docgraph.transactions.Cache.IsDirty)
					{
						docgraph.Save.Press();

						foreach (SOOrderShipment item in PXSelect<SOOrderShipment, Where<SOOrderShipment.orderType, Equal<Current<SOOrder.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrder.orderNbr>>>>>.SelectMultiBound(this, new object[] { order }))
						{
							item.InvtDocType = docgraph.issue.Current.DocType;
							item.InvtRefNbr = docgraph.issue.Current.RefNbr;

							shipmentlist.Cache.Update(item);
						}

						this.Save.Press();

						INRegister existing;
						if ((existing = list.Find(docgraph.issue.Current)) == null)
						{
							list.Add(docgraph.issue.Current);
						}
						else
						{
							docgraph.issue.Cache.RestoreCopy(existing, docgraph.issue.Current);
						}
					}
					ts.Complete();
				}
			}
		}

		#region EPApproval Cahce Attached
		[PXDBDate()]
		[PXDefault(typeof(SOOrder.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(SOOrder.customerID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(SOOrder.orderDesc), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		[CurrencyInfo(typeof(SOOrder.curyInfoID))]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(SOOrder.curyOrderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(SOOrder.orderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		protected CCOrderList toRecord;

		public override void Persist()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Debug.Print("{0} Enter Persist()", DateTime.Now.TimeOfDay);
			Debug.Indent();

			if (toRecord == null)
				toRecord = new CCOrderList(this);
			else
				toRecord.Clear();

			foreach (SOOrder doc in Document.Cache.Cached)
			{
				if ((Document.Cache.GetStatus(doc) == PXEntryStatus.Inserted || Document.Cache.GetStatus(doc) == PXEntryStatus.Updated)
				 && doc.Completed == false)
				{
					SOOrderType orderType = (this.soordertype.Current != null && this.soordertype.Current.OrderType == doc.OrderType) ? this.soordertype.Current : this.soordertype.Select(doc.OrderType);
					if (orderType.ARDocType == ARDocType.Invoice)
					{
						decimal? CuryApplAmt = 0m;
						bool appliedToOrderUpdated = false;

						foreach (PXResult<SOAdjust, ARRegisterAlias, ARPayment, CurrencyInfo> res in Adjustments_Raw.View.SelectMultiBound(new object[] { doc }))
						{
							SOAdjust adj = (SOAdjust)res;

							if (adj != null)
							{
								CuryApplAmt += adj.CuryAdjdAmt;

								if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Updated || Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Inserted)
									appliedToOrderUpdated = true;

								if (doc.CuryDocBal - CuryApplAmt < 0m && CuryApplAmt > 0m)
								{
									if (appliedToOrderUpdated || ((decimal?)Document.Cache.GetValueOriginal<SOOrder.curyOrderTotal>(doc) != doc.CuryOrderTotal))
									{
									if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Notchanged)
									{
										Adjustments.Cache.SetStatus(adj, PXEntryStatus.Updated);
									}
									Adjustments.Cache.RaiseExceptionHandling<SOAdjust.curyAdjdAmt>(adj, adj.CuryAdjdAmt, new PXSetPropertyException(Messages.OrderApplyAmount_Cannot_Exceed_OrderTotal));
									throw new PXException(Messages.OrderApplyAmount_Cannot_Exceed_OrderTotal);
								}
							}
						}
					}
					}
					if (doc.PMInstanceID.HasValue
					 && string.IsNullOrEmpty(doc.PreAuthTranNumber) == false && (doc.CCAuthTranNbr == null))
					{
						SOOrder copy = (SOOrder)this.Document.Cache.CreateCopy(doc);
						copy.IsCCAuthorized = true;
						copy = this.Document.Update(copy);
						toRecord.Add(copy);
					}
				}
			}

			//assuming only one instance of CustomerPaymentMethodC could be inserted at a time
			if (DefPaymentMethodInstance.Cache.Inserted.Count() > 0)
			{
				IEnumerator cpmEnumerator = DefPaymentMethodInstance.Cache.Inserted.GetEnumerator();
				if (cpmEnumerator.MoveNext())
				{
					CustomerPaymentMethodC current = cpmEnumerator.Current as CustomerPaymentMethodC;
					if (current != null && CCProcessingUtils.isTokenizedPaymentMethod(this, current.PMInstanceID))
					{
						CCCustomerInformationManager.SyncNewPMI(this, DefPaymentMethodInstance, DefPaymentMethodInstanceDetailsAll);
					}
				}
			}

			if (Document.Current != null
				&& Document.Current.IsPackageValid != true
				&& !string.IsNullOrEmpty(Document.Current.ShipVia)
				&& soordertype.Current.RequireShipping == true)
			{
				try
				{
					if (Document.Current.IsManualPackage != true)
					{
						RecalculatePackagesForOrder(Document.Current);
					}
				}
				catch (Exception ex)
				{
					PXTrace.WriteError(ex);
				}
			}


			if (Document.Current != null
				&& Document.Current.FreightCostIsValid != true 
				&& !string.IsNullOrEmpty(Document.Current.ShipVia)
				&& soordertype.Current.RequireShipping == true )
			{
				try
				{
					CalculateFreightCost(true);
				}
				catch (Exception ex)
				{
					PXTrace.WriteError(ex);
				}
			}

			foreach (SOOrder order in Document.Cache.Updated)
			{
                if ((order.Behavior == SOBehavior.SO || order.Behavior == SOBehavior.RM) && order.ShipmentCntr > 0 && order.OpenShipmentCntr == 0 && order.OpenLineCntr == 0)
				{
					order.Completed = true;
					order.Hold = false;
					Document.Update(order);
					Document.Search<SOOrder.orderNbr>(Document.Current.OrderNbr, Document.Current.OrderType);
				}
			}
            DiscountEngine<SOLine>.ValidateDiscountDetails(DiscountDetails);

			//When the calling process is a long-running operation recalculate taxes on the same thread before the Persist.
			// PXAutomation.CompleteAction is called even if there is an Exception after the base.Persist() call.
			if (RecalculateAvalaraTaxesSync) 
				RecalculateAvalaraTaxes();

			if (toRecord.Count > 0)
			{
				this.RowPersisted.AddHandler<CustomerPaymentMethodC>(toRecord.Authorize);
				this.RowPersisted.AddHandler<SOOrder>(toRecord.Authorize);
				try
				{
					base.Persist();
				}
				finally
				{
					this.RowPersisted.RemoveHandler<SOOrder>(toRecord.Authorize);
					this.RowPersisted.RemoveHandler<CustomerPaymentMethodC>(toRecord.Authorize);

				}
			}
			else
				base.Persist();

			if (!RecalculateAvalaraTaxesSync) //When the calling process is the 'UI' thread.
				RecalculateAvalaraTaxes();

			sw.Stop();
			Debug.Unindent();
			Debug.Print("{0} Exit Persist in {1} millisec", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);
		}

		protected bool RecalculateAvalaraTaxesSync = false;
		protected virtual void RecalculateAvalaraTaxes()
		{
			if (Document.Current != null && IsExternalTax == true && !SkipAvalaraTaxProcessing && !IsTransferOrder && 
				(Document.Current.IsTaxValid != true || Document.Current.IsOpenTaxValid != true || Document.Current.IsUnbilledTaxValid != true)
				)
			{
				if (RecalculateAvalaraTaxesSync)
				{
					SOOrder doc = new SOOrder();
					doc.OrderType = Document.Current.OrderType;
					doc.OrderNbr = Document.Current.OrderNbr;
					SOExternalTaxCalc.Process(doc);

				}
				else
				{
					Debug.Print("{0} SOExternalTaxCalc.Process(doc) Async", DateTime.Now.TimeOfDay);
					PXLongOperation.StartOperation(this, delegate()
					{
						Debug.Print("{0} Inside PXLongOperation.StartOperation", DateTime.Now.TimeOfDay);
						SOOrder doc = new SOOrder();
						doc.OrderType = Document.Current.OrderType;
						doc.OrderNbr = Document.Current.OrderNbr;
						SOExternalTaxCalc.Process(doc);

					});
				}
			}
		}

        protected void SetReadOnly(bool isReadOnly)
        {
            foreach (PXCache cache in Caches.Values)
            {
                cache.AllowDelete = !isReadOnly;
                cache.AllowUpdate = !isReadOnly;
                cache.AllowInsert = !isReadOnly;
            }
        }

        protected sealed class CCOrderList : List<SOOrder>
		{
			CCPaymentProcessing ccProcGraph = PXGraph.CreateInstance<CCPaymentProcessing>();
			SOOrderEntry orderEntry;
			public CCOrderList(SOOrderEntry graph)
			{
				orderEntry = graph;
			}
			public void Authorize(PXCache sender, PXRowPersistedEventArgs e)
			{
				if (e.TranStatus == PXTranStatus.Open)
				{
					SOOrder iOrder = null;
					SOOrder row = e.Row as SOOrder;
					if (row != null && this.Contains(row)
						&& row.PMInstanceID.HasValue && row.PMInstanceID > 0)
					{
						iOrder = row;
					}
					else
					{
						CustomerPaymentMethod iCard = e.Row as CustomerPaymentMethod;
						if (iCard != null && e.Operation == PXDBOperation.Insert)
						{
							iOrder = this.Find((SOOrder op) => { return op.PMInstanceID == iCard.PMInstanceID && !string.IsNullOrEmpty(op.PreAuthTranNumber); });
						}
					}
					if (iOrder != null)
					{
						decimal authAmount = (iOrder.CuryCCPreAuthAmount) ?? Decimal.Zero;
						if (authAmount == Decimal.Zero)
							throw new PXException(Messages.OrderHaveZeroBalanceError);
						int? tranID = 0;
						if (ccProcGraph.RecordAuthorization(iOrder, iOrder.PreAuthTranNumber, iOrder.CuryCCPreAuthAmount, iOrder.CCAuthExpirationDate, ref tranID))
							iOrder.CCAuthTranNbr = tranID;
					}
				}
			}
		}


		public static void UpdateSOOrderState(IBqlTable aDoc, CCTranType aLastOperation)
		{
			SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
			graph.UpdateDocState(aDoc as SOOrder, aLastOperation);
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

		[Serializable]
		public partial class SOAdjust : PX.Objects.SO.SOAdjust
		{
			#region AdjdOrderType
			public new abstract class adjdOrderType : PX.Data.IBqlField
			{
			}
			[PXDBString(2, IsKey = true, IsFixed = true)]
			[PXDBDefault(typeof(SOOrder.orderType))]
			[PXUIField(DisplayName = "Order Type")]
			public override String AdjdOrderType
			{
				get
				{
					return this._AdjdOrderType;
				}
				set
				{
					this._AdjdOrderType = value;
				}
			}
			#endregion
			#region AdjdOrderNbr
			public new abstract class adjdOrderNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true, IsKey = true)]
			[PXDBDefault(typeof(SOOrder.orderNbr))]
			[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOAdjust.adjdOrderType>>, And<SOOrder.orderNbr, Equal<Current<SOAdjust.adjdOrderNbr>>>>>))]
			[PXUnboundFormula(typeof(Switch<Case<Where<SOAdjust.curyAdjdAmt, Greater<decimal0>>, int1>, int0>), typeof(SumCalc<SOOrder.paymentCntr>))]
			[PXUIField(DisplayName = "Order Nbr.")]
			public override String AdjdOrderNbr
			{
				get
				{
					return this._AdjdOrderNbr;
				}
				set
				{
					this._AdjdOrderNbr = value;
				}
			}
			#endregion
			#region AdjgDocType
			public new abstract class adjgDocType : PX.Data.IBqlField
			{
			}
			[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
			[ARPaymentType.SOList()]
			[PXDefault(ARDocType.Payment)]
			[PXUIField(DisplayName = "Doc. Type")]
			public override String AdjgDocType
			{
				get
				{
					return this._AdjgDocType;
				}
				set
				{
					this._AdjgDocType = value;
				}
			}
			#endregion
			#region AdjgRefNbr
			public new abstract class adjgRefNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
			[PXDefault()]
			[PXUIField(DisplayName = "Reference Nbr.")]
			[ARPaymentType.AdjgRefNbr(typeof(Search<ARPayment.refNbr,
				Where<ARPayment.customerID, Equal<Optional<SOOrder.customerID>>,
				And<ARPayment.docDate, LessEqual<Current<SOOrder.orderDate>>,
				And<ARPayment.docType, Equal<Optional<SOAdjust.adjgDocType>>,
				And<ARPayment.openDoc, Equal<True>>>>>>), Filterable = true)]
			public override String AdjgRefNbr
			{
				get
				{
					return this._AdjgRefNbr;
				}
				set
				{
					this._AdjgRefNbr = value;
				}
			}
			#endregion
			#region CustomerID
			public new abstract class customerID : PX.Data.IBqlField
			{
			}
			[PXDBInt]
			[PXDBDefault(typeof(SOOrder.customerID))]
			[PXUIField(DisplayName = "CustomerID", Visibility = PXUIVisibility.Visible, Visible = false)]
			public override Int32? CustomerID
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
			#region CuryAdjdAmt
			public new abstract class curyAdjdAmt : PX.Data.IBqlField
			{
			}
			[PXDBCurrency(typeof(SOAdjust.adjdCuryInfoID), typeof(SOAdjust.adjAmt))]
			[PXUIField(DisplayName = "Applied To Order")]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(null, typeof(SumCalc<SOOrder.curyPaymentTotal>))]
			[PXFormula(typeof(Sub<SOAdjust.curyOrigAdjdAmt, SOAdjust.curyAdjdBilledAmt>))]
			public override Decimal? CuryAdjdAmt
			{
				get
				{
					return this._CuryAdjdAmt;
				}
				set
				{
					this._CuryAdjdAmt = value;
				}
			}
			#endregion
			#region AdjAmt
			public new abstract class adjAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(typeof(Sub<SOAdjust.origAdjAmt, SOAdjust.adjBilledAmt>))]
			public override Decimal? AdjAmt
			{
				get
				{
					return this._AdjAmt;
				}
				set
				{
					this._AdjAmt = value;
				}
			}
			#endregion
			#region CuryAdjgAmt
			public new abstract class curyAdjgAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXFormula(typeof(Sub<SOAdjust.curyOrigAdjgAmt, SOAdjust.curyAdjgBilledAmt>))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? CuryAdjgAmt
			{
				get
				{
					return this._CuryAdjgAmt;
				}
				set
				{
					this._CuryAdjgAmt = value;
				}
			}
			#endregion
			#region AdjdOrigCuryInfoID
			public new abstract class adjdOrigCuryInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			[CurrencyInfo(typeof(SOOrder.curyInfoID), ModuleCode = "SO", CuryIDField = "AdjdOrigCuryID")]
			public override Int64? AdjdOrigCuryInfoID
			{
				get
				{
					return this._AdjdOrigCuryInfoID;
				}
				set
				{
					this._AdjdOrigCuryInfoID = value;
				}
			}
			#endregion
			#region AdjgCuryInfoID
			public new abstract class adjgCuryInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			[PXDefault()]
			[CurrencyInfo(ModuleCode = "SO", CuryIDField = "AdjgCuryID")]
			public override Int64? AdjgCuryInfoID
			{
				get
				{
					return this._AdjgCuryInfoID;
				}
				set
				{
					this._AdjgCuryInfoID = value;
				}
			}
			#endregion
			#region AdjdCuryInfoID
			public new abstract class adjdCuryInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			[CurrencyInfo(typeof(SOOrder.curyInfoID), ModuleCode = "SO", CuryIDField = "AdjdCuryID")]
			public override Int64? AdjdCuryInfoID
			{
				get
				{
					return this._AdjdCuryInfoID;
				}
				set
				{
					this._AdjdCuryInfoID = value;
				}
			}
			#endregion
			#region AdjgDocDate
			public new abstract class adjgDocDate : PX.Data.IBqlField
			{
			}
			[PXDBDate()]
			[PXDBDefault(typeof(SOOrder.orderDate))]
			public override DateTime? AdjgDocDate
			{
				get
				{
					return this._AdjgDocDate;
				}
				set
				{
					this._AdjgDocDate = value;
				}
			}
			#endregion
			#region AdjdOrderDate
			public new abstract class adjdOrderDate : PX.Data.IBqlField
			{
			}
			[PXDBDate()]
			[PXDBDefault(typeof(SOOrder.orderDate))]
			[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override DateTime? AdjdOrderDate
			{
				get
				{
					return this._AdjdOrderDate;
				}
				set
				{
					this._AdjdOrderDate = value;
				}
			}
			#endregion

			#region CuryDocBal
			public new abstract class curyDocBal : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(SOAdjust.adjgCuryInfoID), typeof(SOAdjust.docBal), BaseCalc = false)]
			[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override Decimal? CuryDocBal
			{
				get
				{
					return this._CuryDocBal;
				}
				set
				{
					this._CuryDocBal = value;
				}
			}
			#endregion
			#region DocBal
			public new abstract class docBal : PX.Data.IBqlField
			{
			}
			[PXDecimal(4)]
			[PXUnboundDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? DocBal
			{
				get
				{
					return this._DocBal;
				}
				set
				{
					this._DocBal = value;
				}
			}
			#endregion
		}

		#region CopyParamFilter
		protected virtual void CopyParamFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
			CopyParamFilter row = e.Row as CopyParamFilter;

			if (row.OrderType != null)
			{
				Numbering numbering = PXSelectJoin<Numbering, LeftJoin<SOOrderType, On<SOOrderType.orderNumberingID, Equal<Numbering.numberingID>>>, Where<SOOrderType.orderType, Equal<Current<CopyParamFilter.orderType>>>>.Select(this);
				PXUIFieldAttribute.SetEnabled<CopyParamFilter.orderNbr>(sender, e.Row, numbering.UserNumbering == true);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled<CopyParamFilter.orderNbr>(sender, e.Row, false);
			}
			checkCopyParams.SetEnabled(!string.IsNullOrEmpty(row.OrderType) && !string.IsNullOrEmpty(row.OrderNbr));
			if (string.IsNullOrEmpty(row.OrderType))
				PXUIFieldAttribute.SetWarning<CopyParamFilter.orderType>(sender, e.Row, PXMessages.LocalizeFormatNoPrefix(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<CopyParamFilter.orderType>(sender)));
			else
				PXUIFieldAttribute.SetWarning<CopyParamFilter.orderType>(sender, e.Row, null);
			if (string.IsNullOrEmpty(row.OrderNbr))
				PXUIFieldAttribute.SetWarning<CopyParamFilter.orderNbr>(sender, e.Row, PXMessages.LocalizeFormatNoPrefix(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<CopyParamFilter.orderNbr>(sender)));
			else
				PXUIFieldAttribute.SetWarning<CopyParamFilter.orderNbr>(sender, e.Row, null);
			sender.IsDirty = false;
		}

		protected virtual void CopyParamFilter_OrderType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CopyParamFilter row = e.Row as CopyParamFilter;
			if (row != null)
			{
				if (row.OrderType != null)
					sender.SetDefaultExt<CopyParamFilter.orderNbr>(e.Row);
				else
					row.OrderNbr = null;
			}
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
				//this._blockUIUpdate = true;
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


		private class CarrierRequestInfo
		{
			public CarrierPlugin Plugin;
			public ICarrierService Service;
			public CarrierRequest Request;
			public CarrierResult<IList<RateQuote>>  Result;
		}

		public static void ProcessPOReceipt(PXGraph graph, IEnumerable<PXResult<INItemPlan, INPlanType>> list, string POReceiptType, string POReceiptNbr)
		{
			var soorder = new PXSelect<SOOrder>(graph);
			if (!graph.Views.Caches.Contains(typeof(SOOrder)))
				graph.Views.Caches.Add(typeof(SOOrder));
			var solinesplit = new PXSelect<SOLineSplit>(graph);
			if (!graph.Views.Caches.Contains(typeof(SOLineSplit)))
				graph.Views.Caches.Add(typeof(SOLineSplit));
			var initemplan = new PXSelect<INItemPlan>(graph);

			List<SOLineSplit> splitsToDeletePlanID = new List<SOLineSplit>();

			List<SOLineSplit> insertedSchedules = new List<SOLineSplit>();
			List<INItemPlan> deletedPlans = new List<INItemPlan>();

			foreach (PXResult<INItemPlan, INPlanType> res in list)
			{
				INItemPlan plan = PXCache<INItemPlan>.CreateCopy(res);
				INPlanType plantype = res;

				//avoid ReadItem()
				if (initemplan.Cache.GetStatus(plan) != PXEntryStatus.Inserted)
				{
					initemplan.Cache.SetStatus(plan, PXEntryStatus.Notchanged);
	}

				//Original Schedule Marked for PO / Allocated on Remote Whse
				//SOLineSplit schedule = PXSelect<SOLineSplit, Where<SOLineSplit.planID, Equal<Required<SOLineSplit.planID>>, And<SOLineSplit.completed, Equal<False>>>>.Select(this, plan.DemandPlanID);
				SOLineSplit schedule = PXSelect<SOLineSplit, Where<SOLineSplit.planID, Equal<Required<SOLineSplit.planID>>>>.Select(graph, plan.DemandPlanID);

				if (schedule != null && (schedule.Completed == false || solinesplit.Cache.GetStatus(schedule) == PXEntryStatus.Updated))
				{
					schedule = PXCache<SOLineSplit>.CreateCopy(schedule);

					schedule.BaseReceivedQty += plan.PlanQty;
					schedule.ReceivedQty = INUnitAttribute.ConvertFromBase(solinesplit.Cache, schedule.InventoryID, schedule.UOM, (decimal)schedule.BaseReceivedQty, INPrecision.QUANTITY);
	
					solinesplit.Cache.Update(schedule);

					INItemPlan origplan = PXSelect<INItemPlan, Where<INItemPlan.planID, Equal<Required<INItemPlan.planID>>>>.Select(graph, plan.DemandPlanID);
					if (origplan != null)
					{
						origplan.PlanQty = schedule.BaseQty - schedule.BaseReceivedQty;
						initemplan.Cache.Update(origplan);
					}

					//select Allocated line if any, exclude allocated on Remote Whse
					PXSelectBase<INItemPlan> cmd = new PXSelectJoin<INItemPlan, InnerJoin<SOLineSplit, On<SOLineSplit.planID, Equal<INItemPlan.planID>>>, Where<INItemPlan.demandPlanID, Equal<Required<INItemPlan.demandPlanID>>, And<SOLineSplit.isAllocated, Equal<True>, And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>>>>>(graph);
					if (!string.IsNullOrEmpty(plan.LotSerialNbr))
					{
						cmd.WhereAnd<Where<INItemPlan.lotSerialNbr, Equal<Required<INItemPlan.lotSerialNbr>>>>();
					}
					PXResult<INItemPlan> allocres = cmd.Select(plan.DemandPlanID, plan.SiteID, plan.LotSerialNbr);

					if (allocres != null)
					{
						schedule = PXResult.Unwrap<SOLineSplit>(allocres);
						solinesplit.Cache.SetStatus(schedule, PXEntryStatus.Notchanged);
						schedule = PXCache<SOLineSplit>.CreateCopy(schedule);
						schedule.BaseQty += plan.PlanQty;
						schedule.Qty = INUnitAttribute.ConvertFromBase(solinesplit.Cache, schedule.InventoryID, schedule.UOM, (decimal)schedule.BaseQty, INPrecision.QUANTITY);
						schedule.POReceiptType = POReceiptType;
						schedule.POReceiptNbr = POReceiptNbr;

						solinesplit.Cache.Update(schedule);

						INItemPlan allocplan = PXCache<INItemPlan>.CreateCopy(res);
						allocplan.PlanQty += plan.PlanQty;

						initemplan.Cache.Update(allocplan);

						plantype = PXCache<INPlanType>.CreateCopy(plantype);
						plantype.ReplanOnEvent = null;
						plantype.DeleteOnEvent = true;
					}
					else
					{
						soorder.Current = (SOOrder)PXParentAttribute.SelectParent(solinesplit.Cache, schedule, typeof(SOOrder));
						schedule = PXCache<SOLineSplit>.CreateCopy(schedule);

						ClearScheduleReferences(ref schedule);

						schedule.IsAllocated = (plantype.ReplanOnEvent != INPlanConstants.Plan60);
						schedule.LotSerialNbr = plan.LotSerialNbr;
						schedule.POCreate = false;
						schedule.POSource = null;
						schedule.POReceiptType = POReceiptType;
						schedule.POReceiptNbr = POReceiptNbr;
						schedule.SiteID = plan.SiteID;
						schedule.VendorID = null;

						schedule.BaseReceivedQty = 0m;
						schedule.ReceivedQty = 0m;
						schedule.BaseQty = plan.PlanQty;
						schedule.Qty = INUnitAttribute.ConvertFromBase(solinesplit.Cache, schedule.InventoryID, schedule.UOM, (decimal)schedule.BaseQty, INPrecision.QUANTITY);

						//update SupplyPlanID in existing item plans (replenishment)
						foreach (PXResult<INItemPlan> demand_res in PXSelect<INItemPlan,
							Where<INItemPlan.supplyPlanID, Equal<Required<INItemPlan.supplyPlanID>>>>.Select(graph, schedule.PlanID))
						{
							INItemPlan demand_plan = PXCache<INItemPlan>.CreateCopy(demand_res);
							initemplan.Cache.SetStatus(demand_plan, PXEntryStatus.Notchanged);
							demand_plan.SupplyPlanID = plan.PlanID;
							initemplan.Cache.Update(demand_plan);
						}

						schedule.PlanID = plan.PlanID;

						schedule = (SOLineSplit)solinesplit.Cache.Insert(schedule);
						insertedSchedules.Add(schedule);
					}
				}
				else if (plan.DemandPlanID != null)
				{
					//Original schedule was completed/plan record deleted by Cancel Order or Confirm Shipment
					plantype = PXCache<INPlanType>.CreateCopy(plantype);
					plantype.ReplanOnEvent = null;
					plantype.DeleteOnEvent = true;
				}
				else
				{
					//Original schedule Marked for PO
					//TODO: verify this is sufficient for Original SO marked for TR.
					schedule = PXSelect<SOLineSplit, Where<SOLineSplit.planID, Equal<Required<SOLineSplit.planID>>, And<SOLineSplit.completed, Equal<False>>>>.Select(graph, plan.PlanID);
					if (schedule != null)
					{
						solinesplit.Cache.SetStatus(schedule, PXEntryStatus.Notchanged);
						schedule = PXCache<SOLineSplit>.CreateCopy(schedule);

						schedule.Completed = true;
						schedule.POCompleted = true;
						splitsToDeletePlanID.Add(schedule);
						solinesplit.Cache.Update(schedule);

						INItemPlan origplan = PXSelect<INItemPlan, Where<INItemPlan.planID, Equal<Required<INItemPlan.planID>>>>.Select(graph, plan.PlanID);
						deletedPlans.Add(origplan);

						initemplan.Cache.Delete(origplan);
					}
				}

				if (plantype.ReplanOnEvent != null)
				{
					plan.PlanType = plantype.ReplanOnEvent;
					plan.SupplyPlanID = null;
					plan.DemandPlanID = null;
					initemplan.Cache.Update(plan);
				}
				else if (plantype.DeleteOnEvent == true)
				{
					initemplan.Delete(plan);
				}
			}

			//Create new schedules for partially received schedules marked for PO. 
			foreach (SOLineSplit newsplit in insertedSchedules)
			{
				SOLineSplit parentschedule = PXSelect<SOLineSplit, Where<SOLineSplit.orderType, Equal<Required<SOLineSplit.orderType>>, 
					And<SOLineSplit.orderNbr, Equal<Required<SOLineSplit.orderNbr>>, 
					And<SOLineSplit.lineNbr, Equal<Required<SOLineSplit.lineNbr>>,
					And<SOLineSplit.splitLineNbr, Equal<Required<SOLineSplit.parentSplitLineNbr>>>>>>>.Select(graph, newsplit.OrderType, newsplit.OrderNbr, newsplit.LineNbr, newsplit.ParentSplitLineNbr);

				if (parentschedule != null && parentschedule.Completed == true && parentschedule.POCompleted == true && parentschedule.BaseQty > parentschedule.BaseReceivedQty
							&& deletedPlans.Exists(x => x.PlanID == parentschedule.PlanID))
				{
					parentschedule = PXCache<SOLineSplit>.CreateCopy(parentschedule);
					INItemPlan demand = PXCache<INItemPlan>.CreateCopy(deletedPlans.First(x => x.PlanID == parentschedule.PlanID));

					UpdateSchedulesFromCompletedPO(graph, solinesplit, initemplan, parentschedule, demand);
				}
			}

			//Added because of MySql AutoIncrement counters behavior
			foreach (SOLineSplit split in splitsToDeletePlanID)
			{
				SOLineSplit schedule = (SOLineSplit)solinesplit.Cache.Locate(split);
				if (schedule != null)
				{
					schedule.PlanID = null;
					solinesplit.Cache.Update(schedule);
				}
			}
		}

		public static void ProcessPOOrder(PXGraph graph, POOrder poOrder)
		{
			if (poOrder == null) return;

			var soorder = new PXSelect<SOOrder>(graph);
			if (!graph.Views.Caches.Contains(typeof(SOOrder)))
				graph.Views.Caches.Add(typeof(SOOrder));
			var solinesplit = new PXSelect<SOLineSplit>(graph);
			if (!graph.Views.Caches.Contains(typeof(SOLineSplit)))
				graph.Views.Caches.Add(typeof(SOLineSplit));
			var initemplan = new PXSelect<INItemPlan>(graph);

			//Search for completed/cancelled POLines with uncompleted linked schedules 
			foreach (PXResult<POLine, INItemPlan, SOLineSplit, INPlanType> res in PXSelectJoin<POLine,
				InnerJoin<INItemPlan, On<INItemPlan.supplyPlanID, Equal<POLine.planID>>,
				InnerJoin<SOLineSplit, On<SOLineSplit.planID, Equal<INItemPlan.planID>, And<SOLineSplit.pOType, Equal<POLine.orderType>, And<SOLineSplit.pONbr, Equal<POLine.orderNbr>, And<SOLineSplit.pOLineNbr, Equal<POLine.lineNbr>>>>>,
				InnerJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>>>>,
			Where<POLine.orderType, Equal<Required<POLine.orderType>>, And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>, And2<Where<POLine.cancelled, Equal<boolTrue>, Or<POLine.completed, Equal<boolTrue>>>,
				And<SOLineSplit.receivedQty, Less<SOLineSplit.qty>, And<SOLineSplit.pOCancelled, NotEqual<boolTrue>, And<SOLineSplit.completed, NotEqual<boolTrue>>>>>>>>.Select(graph, poOrder.OrderType, poOrder.OrderNbr))
			{
				POLine poline = res;
				INItemPlan plan = PXCache<INItemPlan>.CreateCopy(res);
				SOLineSplit parentschedule = PXCache<SOLineSplit>.CreateCopy(res);

				if (initemplan.Cache.GetStatus(plan) != PXEntryStatus.Inserted)
				{
					initemplan.Cache.SetStatus(plan, PXEntryStatus.Notchanged);
				}

				soorder.Current = (SOOrder)PXParentAttribute.SelectParent(solinesplit.Cache, parentschedule, typeof(SOOrder));

				//partially received Drop-Ships are skipped
				if (parentschedule.Completed != true && parentschedule.POCancelled != true && parentschedule.BaseQty > parentschedule.BaseReceivedQty && !(poline.LineType == POLineType.GoodsForDropShip && poline.BaseReceivedQty != 0m))
				{
					UpdateSchedulesFromCompletedPO(graph, solinesplit, initemplan, parentschedule, plan);

					solinesplit.Cache.SetStatus(parentschedule, PXEntryStatus.Notchanged);
					parentschedule = PXCache<SOLineSplit>.CreateCopy(parentschedule);

					parentschedule.PlanID = null;
					parentschedule.Completed = true;
					parentschedule.POCompleted = true;
					parentschedule.POCancelled = true;
					solinesplit.Cache.Update(parentschedule);
				}
			}
		}

		private static void UpdateSchedulesFromCompletedPO(PXGraph graph, PXSelect<SOLineSplit> solinesplit, PXSelect<INItemPlan> initemplan, SOLineSplit parentschedule, INItemPlan demand)
		{
			graph.FieldDefaulting.AddHandler<SOLineSplit.locationID>((sender, e) =>
		{
				if (e.Row != null && ((SOLineSplit)e.Row).RequireLocation != true)
				{
					e.NewValue = null;
					e.Cancel = true;
				}
			});

			SOLineSplit newschedule = PXCache<SOLineSplit>.CreateCopy(parentschedule);

			ClearScheduleReferences(ref newschedule);

			newschedule.LotSerialNbr = demand.LotSerialNbr;
			newschedule.SiteID = demand.SiteID;

			newschedule.BaseQty = parentschedule.BaseQty - parentschedule.BaseReceivedQty;
			newschedule.Qty = INUnitAttribute.ConvertFromBase(solinesplit.Cache, newschedule.InventoryID, newschedule.UOM, (decimal)newschedule.BaseQty, INPrecision.QUANTITY);
			newschedule.BaseReceivedQty = 0m;
			newschedule.ReceivedQty = 0m;
			newschedule.PlanID = demand.PlanID;
			
			solinesplit.Cache.Insert(newschedule);

			//Updating INItemPlan from orginal line
			demand.SupplyPlanID = null;
			demand.DemandPlanID = null;
			demand.PlanQty = newschedule.BaseQty;
			demand.VendorID = null;
			demand.VendorLocationID = null;

			demand = (INItemPlan)initemplan.Cache.Update(demand);
		}

		public static void ClearScheduleReferences(ref SOLineSplit schedule)
		{
			schedule.ParentSplitLineNbr = schedule.SplitLineNbr;
			schedule.SplitLineNbr = null;
			schedule.Completed = false;
			//clear PO references
			schedule.POCompleted = false;
			schedule.POCancelled = false;
			schedule.POType = null;
			schedule.PONbr = null;
			schedule.POLineNbr = null;
			schedule.POReceiptType = null;
			schedule.POReceiptNbr = null;
			//clear SO references
			schedule.SOOrderType = null;
			schedule.SOOrderNbr = null;
			schedule.SOLineNbr = null;
			schedule.SOSplitLineNbr = null;
			schedule.RefNoteID = null;
		}

	}

	[Serializable()]
	public partial class AddInvoiceFilter : IBqlTable
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXString(3, IsFixed = true)]
		[PXDefault(ARDocType.Invoice)]
		[PXStringList(
				new string[] { ARDocType.Invoice, ARDocType.CashSale, ARDocType.DebitMemo },
				new string[] { AR.Messages.Invoice, AR.Messages.CashSale, AR.Messages.DebitMemo })]
		[PXUIField(DisplayName = "Type")]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARInvoiceType.RefNbr(typeof(Search2<AR.Standalone.ARRegisterAlias.refNbr,
			InnerJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<AR.Standalone.ARRegisterAlias.docType>,
				And<ARInvoice.refNbr, Equal<AR.Standalone.ARRegisterAlias.refNbr>>>>,
			Where<AR.Standalone.ARRegisterAlias.docType, Equal<Optional<AddInvoiceFilter.docType>>,
				And<AR.Standalone.ARRegisterAlias.released, Equal<boolTrue>,
				And<AR.Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleSO>,
				And<AR.Standalone.ARRegisterAlias.customerID, Equal<Current<SOOrder.customerID>>>>>>,
			OrderBy<Desc<AR.Standalone.ARRegisterAlias.refNbr>>>), Filterable = true)]
		[PXFormula(typeof(Default<AddInvoiceFilter.docType>))]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region Expand
		public abstract class expand : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Show Non-Stock Kits by Components")]
		public virtual Boolean? Expand
		{
			get;set;
		}
		#endregion
	}
	[Serializable]
	public partial class SOParamFilter : IBqlTable
	{
		#region ShipDate
		public abstract class shipDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ShipDate;
		[PXDate]
		[PXUIField(DisplayName = "Shipment Date", Required = true)]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Warehouse ID", Required = true, FieldClass = SiteAttribute.DimensionName)]
		[OrderSiteSelector()]
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
	}

	[Serializable()]
	public partial class CopyParamFilter : IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsFixed = true, InputMask = ">aa")]
		[PXDefault(typeof(Search<SOSetup.defaultOrderType>))]
        [PXSelector(typeof(Search2<SOOrderType.orderType, InnerJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.orderType, Equal<SOOrderType.orderType>, And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>>>))]
        [PXRestrictor(typeof(Where<SOOrderTypeOperation.iNDocType, NotEqual<INTranType.transfer>, Or<FeatureInstalled<FeaturesSet.warehouse>>>), ErrorMessages.ElementDoesntExist, typeof(CopyParamFilter.orderType))]
        [PXRestrictor(typeof(Where<SOOrderType.requireAllocation, NotEqual<True>, Or<AllocationAllowed>>), ErrorMessages.ElementDoesntExist, typeof(CopyParamFilter.orderType))]
        [PXRestrictor(typeof(Where<SOOrderType.active, Equal<True>>), ErrorMessages.ElementDoesntExist, typeof(CopyParamFilter.orderType))]
        [PXUIField(DisplayName = "Order Type")]
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
		#region RecalcUnitPrices
		public abstract class recalcUnitPrices : PX.Data.IBqlField
		{
		}
		protected Boolean? _RecalcUnitPrices;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Recalculate Unit Prices")]
		public virtual Boolean? RecalcUnitPrices
		{
			get
			{
				return this._RecalcUnitPrices;
			}
			set
			{
				this._RecalcUnitPrices = value;
			}
		}
		#endregion
		#region OverrideManualPrices
		public abstract class overrideManualPrices : PX.Data.IBqlField
		{
		}
		protected Boolean? _OverrideManualPrices;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Manual Prices", Visible=false)]
		public virtual Boolean? OverrideManualPrices
		{
			get
			{
				return this._OverrideManualPrices;
			}
			set
			{
				this._OverrideManualPrices = value;
			}
		}
		#endregion
		#region RecalcDiscounts
		public abstract class recalcDiscounts : PX.Data.IBqlField
		{
		}
		protected Boolean? _RecalcDiscounts;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Recalculate Discounts")]
		public virtual Boolean? RecalcDiscounts
		{
			get
			{
				return this._RecalcDiscounts;
			}
			set
			{
				this._RecalcDiscounts = value;
			}
		}
		#endregion
		#region OverrideManualDiscounts
		public abstract class overrideManualDiscounts : PX.Data.IBqlField
		{
		}
		protected Boolean? _OverrideManualDiscounts;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Manual Discounts")]
		public virtual Boolean? OverrideManualDiscounts
		{
			get
			{
				return this._OverrideManualDiscounts;
			}
			set
			{
				this._OverrideManualDiscounts = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search2<Numbering.newSymbol, InnerJoin<SOOrderType, On<Numbering.numberingID, Equal<SOOrderType.orderNumberingID>>>, Where<SOOrderType.orderType, Equal<Current<CopyParamFilter.orderType>>, And<Numbering.userNumbering, Equal<False>>>>))]
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

	[Serializable()]
	[PXProjection(typeof(Select<POLine>), Persistent = true)]
	public partial class POLine3 : PX.Data.IBqlTable
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
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
		[PXDefault()]
		[PXUIField(DisplayName = "PO Type", Enabled = false)]
		[PO.POOrderType.List()]
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
		[PXUIField(DisplayName = "PO Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Current<POLine3.orderType>>>>), DescriptionField = typeof(POOrder.orderDesc))]
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
		#region VendorRefNbr
		public abstract class vendorRefNbr : PX.Data.IBqlField
		{
		}
		protected String _VendorRefNbr;
		[PXString(40)]
		[PXUIField(DisplayName = "Vendor Ref.", Enabled = false)]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(POLine.lineNbr))]
		[PXDefault()]
		[PXUIField(DisplayName = "PO Line Nbr.", Visible = false)]
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
		[PO.POLineType.List()]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true, BqlField = typeof(POLine.inventoryID), Enabled = false)]
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
		[SubItem(BqlField = typeof(POLine.subItemID), Enabled = false)]
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
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong(BqlField = typeof(POLine.planID))]
		[PXUIField(Visible = false, Enabled = false)]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[AP.Vendor(typeof(Search<BAccountR.bAccountID,
			Where<Vendor.type, NotEqual<BAccountType.employeeType>>>),
			BqlField = typeof(POLine.vendorID), Enabled = false)]
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
		#region OrderDate
		public abstract class orderDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrderDate;
		[PXDBDate(BqlField = typeof(POLine.orderDate))]
		[PXUIField(DisplayName = "Order Date", Enabled = false)]
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
		#region PromisedDate
		public abstract class promisedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PromisedDate;
		[PXDBDate(BqlField = typeof(POLine.promisedDate))]
		[PXUIField(DisplayName = "Promised", Enabled = false)]
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
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool(BqlField = typeof(POLine.cancelled))]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDBString(6, IsUnicode = true, BqlField = typeof(POLine.uOM))]
		[PXUIField(DisplayName = "UOM", Enabled = false)]
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
		[PXDBQuantity(BqlField = typeof(POLine.orderQty))]
		[PXUIField(DisplayName = "Order Qty.", Enabled = false)]
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
		[PXDBQuantity(BqlField = typeof(POLine.baseOrderQty))]
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
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(BqlField = typeof(POLine.openQty))]
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
		#region ReceivedQty
		public abstract class receivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedQty;
		[PXDBDecimal(6, BqlField = typeof(POLine.receivedQty))]
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
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true, BqlField = typeof(POLine.tranDesc))]
		[PXUIField(DisplayName = "Line Description", Enabled = false)]
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
		#region ReceiptStatus
		public abstract class receiptStatus : PX.Data.IBqlField
		{
		}
		protected String _ReceiptStatus;
		[PXDBString(1, IsFixed = true, BqlField = typeof(POLine.receiptStatus))]
		public virtual String ReceiptStatus
		{
			get
			{
				return this._ReceiptStatus;
			}
			set
			{
				this._ReceiptStatus = value;
			}
		}
		#endregion
		#region SOOrderType
		public abstract class sOOrderType : PX.Data.IBqlField
		{
		}
		protected String _SOOrderType;
		[PXString(2, IsFixed = true)]
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
		[PXString(15, IsUnicode = true)]
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
		[PXInt()]
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
		#region SOOrderSplitLineNbr
		public abstract class sOOrderSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOOrderSplitLineNbr;
		[PXInt()]
		public virtual Int32? SOOrderSplitLineNbr
		{
			get
			{
				return this._SOOrderSplitLineNbr;
			}
			set
			{
				this._SOOrderSplitLineNbr = value;
			}
		}
		#endregion
		#region DemandQty
		public abstract class demandQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _DemandQty;
		[PXDecimal(6)]
		public virtual Decimal? DemandQty
		{
			get
			{
				return this._DemandQty;
			}
			set
			{
				this._DemandQty = value;
			}
		}
		#endregion
	}

	[Serializable]
	public partial class SOSiteStatusFilter : INSiteStatusFilter
	{
			#region Inventory
			public new abstract class inventory : PX.Data.IBqlField
			{
			}
			#endregion
			#region Mode
			public abstract class mode : PX.Data.IBqlField
		{
		}
		protected int? _Mode;
		[PXDBInt]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Selection Mode")]
		[SOAddItemMode.List]
		public virtual int? Mode
		{
			get
			{
				return _Mode;
			}
			set
			{
				_Mode = value;
			}
		}
		#endregion
		#region HistoryDate
		public abstract class historyDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _HistoryDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Sold Since")]
		public virtual DateTime? HistoryDate
		{
			get
			{
				return this._HistoryDate;
			}
			set
			{
				this._HistoryDate = value;
			}
		}
		#endregion
	}

	public class SOAddItemMode
	{
		public const int BySite = 0;
		public const int ByCustomer = 1;
		public class ListAttribute : PXIntListAttribute
		{
			public ListAttribute()
				: base(
					new int[] { BySite, ByCustomer },
					new string[] { Messages.BySite, Messages.ByCustomer }
				)
			{
			}
		}
		public class bySite : Constant<int> { public bySite() : base(BySite) { } }
		public class byCustomer : Constant<int> { public byCustomer() : base(ByCustomer) { } }
	}

	[System.SerializableAttribute()]
	[PXProjection(typeof(Select2<InventoryItem,
		LeftJoin<INSiteStatus,
						On<INSiteStatus.inventoryID, Equal<InventoryItem.inventoryID>,
						And<InventoryItem.stkItem, Equal<boolTrue>>>,
		LeftJoin<INSubItem,
						On<INSubItem.subItemID, Equal<INSiteStatus.subItemID>>,
		LeftJoin<INSite,
						On<INSite.siteID, Equal<INSiteStatus.siteID>>,
		LeftJoin<INItemXRef,
						On<INItemXRef.inventoryID, Equal<InventoryItem.inventoryID>,						
						And2<Where<INItemXRef.subItemID, Equal<INSiteStatus.subItemID>,
								Or<INSiteStatus.subItemID, IsNull>>,
						And<Where<CurrentValue<SOSiteStatusFilter.barCode>, IsNotNull, 
						And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>>>,
		LeftJoin<INItemPartNumber,
						On<INItemPartNumber.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INItemPartNumber.alternateID, Like<CurrentValue<SOSiteStatusFilter.inventory_Wildcard>>,
						And2<Where<INItemPartNumber.bAccountID, Equal<Zero>, 
									  Or<INItemPartNumber.bAccountID, Equal<CurrentValue<SOOrder.customerID>>,
										Or<INItemPartNumber.alternateType, Equal<INAlternateType.vPN>>>>, 										
						And<Where<INItemPartNumber.subItemID, Equal<INSiteStatus.subItemID>,
								   Or<INSiteStatus.subItemID, IsNull>>>>>>,
		LeftJoin<INItemClass,
						On<INItemClass.itemClassID, Equal<InventoryItem.itemClassID>>,
		LeftJoin<INPriceClass,
						On<INPriceClass.priceClassID, Equal<InventoryItem.priceClassID>>,
		LeftJoin<BAccountR,
						On<BAccountR.bAccountID, Equal<InventoryItem.preferredVendorID>>,
		LeftJoin<INItemCustSalesStats,
				  On<CurrentValue<SOSiteStatusFilter.mode>, Equal<SOAddItemMode.byCustomer>,
							And<INItemCustSalesStats.inventoryID, Equal<InventoryItem.inventoryID>,
							And<INItemCustSalesStats.subItemID, Equal<INSiteStatus.subItemID>,
							And<INItemCustSalesStats.siteID, Equal<INSiteStatus.siteID>,
							And<INItemCustSalesStats.bAccountID, Equal<CurrentValue<SOOrder.customerID>>,
							And<INItemCustSalesStats.lastDate, GreaterEqual<CurrentValue<SOSiteStatusFilter.historyDate>>>>>>>>,
	LeftJoin<INUnit,
					On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
				 And<INUnit.unitType, Equal<int1>,
				 And<INUnit.fromUnit, Equal<InventoryItem.salesUnit>,
				 And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>>>
							>>>>>>>>>>,
		Where<CurrentValue<SOOrder.customerID>, IsNotNull,
		  And2<CurrentMatch<InventoryItem, AccessInfo.userName>,
			And2<Where<INSiteStatus.siteID, IsNull, Or<INSite.branchID, IsNotNull, And<CurrentMatch<INSite, AccessInfo.userName>>>>,
			And2<Where<INSiteStatus.subItemID, IsNull, Or<CurrentMatch<INSubItem, AccessInfo.userName>>>,
			And2<Where<CurrentValue<INSiteStatusFilter.onlyAvailable>, Equal<boolFalse>,
				   Or<INSiteStatus.qtyAvail, Greater<CS.decimal0>>>,
		  And2<Where<CurrentValue<SOSiteStatusFilter.mode>, Equal<SOAddItemMode.bySite>,
					 Or<INItemCustSalesStats.lastQty, Greater<CS.decimal0>>>,			
			And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
			And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noSales>>>>>>>>>>), Persistent = false)]
	public partial class SOSiteStatusSelected : IBqlTable
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
		[PXUIField(DisplayName = "Item Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
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
		[PXUIField(DisplayName = "Price Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
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
		[AP.VendorNonEmployeeActive(DisplayName = "Preferred Vendor ID", Required = false, DescriptionField = typeof(BAccountR.acctName), BqlField = typeof(InventoryItem.preferredVendorID), Visible = false, ErrorHandling = PXErrorHandling.Always)]
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
		[PXDBString(250, IsUnicode = true, BqlField = typeof(BAccountR.acctName))]
		[PXUIField(DisplayName = "Preferred Vendor Name", Visible = false, ErrorHandling = PXErrorHandling.Always)]
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
		[PXUIField(DisplayName = "Barcode", Visible=false)]
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
		[PXDBString(225, IsUnicode = true, InputMask = "",  BqlField = typeof(INItemPartNumber.alternateID))]		
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
		[SubItem(typeof(SOSiteStatusSelected.inventoryID), BqlField = typeof(INSubItem.subItemID), IsKey = true)]
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
		[PXDBString(BqlField = typeof(INSubItem.subItemCD), IsUnicode = true)]
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

		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXLong()]
		[CurrencyInfo()]
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

		#region SalesUnit
		public abstract class salesUnit : PX.Data.IBqlField
		{
		}
		protected string _SalesUnit;
		[INUnit(typeof(SOSiteStatusSelected.inventoryID), DisplayName = "Sales Unit", BqlField = typeof(InventoryItem.salesUnit))]
		public virtual String SalesUnit
		{
			get
			{
				return this._SalesUnit;
			}
			set
			{
				this._SalesUnit = value;
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

		#region QtyLast
		public abstract class qtyLast : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyLast;
		[PXDBQuantity(BqlField = typeof(INItemCustSalesStats.lastQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? QtyLast
		{
			get
			{
				return this._QtyLast;
			}
			set
			{
				this._QtyLast = value;
			}
		}
		#endregion

		#region BaseUnitPrice
		public abstract class baseUnitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseUnitPrice;
		[PXDBPriceCost(BqlField = typeof(INItemCustSalesStats.lastUnitPrice))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseUnitPrice
		{
			get
			{
				return this._BaseUnitPrice;
			}
			set
			{
				this._BaseUnitPrice = value;
			}
		}
		#endregion

		#region CuryUnitPrice
		public abstract class curyUnitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitPrice;
		[PXUnitPriceCuryConv(typeof(SOSiteStatusSelected.curyInfoID), typeof(SOSiteStatusSelected.baseUnitPrice))]
		[PXUIField(DisplayName = "Last Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnitPrice
		{
			get
			{
				return this._CuryUnitPrice;
			}
			set
			{
				this._CuryUnitPrice = value;
			}
		}
		#endregion

		#region QtyAvailSale
		public abstract class qtyAvailSale : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyAvailSale;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatus.qtyAvail, INUnit.unitRate>>,
			Div<INSiteStatus.qtyAvail, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvailSale
		{
			get
			{
				return this._QtyAvailSale;
			}
			set
			{
				this._QtyAvailSale = value;
			}
		}
		#endregion

		#region QtyOnHandSale
		public abstract class qtyOnHandSale : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHandSale;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatus.qtyOnHand, INUnit.unitRate>>,
			Div<INSiteStatus.qtyOnHand, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHandSale
		{
			get
			{
				return this._QtyOnHandSale;
			}
			set
			{
				this._QtyOnHandSale = value;
			}
		}
		#endregion

		#region QtyLastSale
		public abstract class qtyLastSale : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyLastSale;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INItemCustSalesStats.lastQty, INUnit.unitRate>>,
			Div<INItemCustSalesStats.lastQty, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		[PXUIField(DisplayName = "Qty. Last Sales")]
		public virtual Decimal? QtyLastSale
		{
			get
			{
				return this._QtyLastSale;
			}
			set
			{
				this._QtyLastSale = value;
			}
		}
		#endregion

		#region LastSalesDate
		public abstract class lastSalesDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastSalesDate;
		[PXDBDate(BqlField = typeof(INItemCustSalesStats.lastDate))]
		[PXUIField(DisplayName = "Last Sales Date")]
		public virtual DateTime? LastSalesDate
		{
			get
			{
				return this._LastSalesDate;
			}
			set
			{
				this._LastSalesDate = value;
			}
		}
		#endregion

	}
	[System.SerializableAttribute()]
	[PXSubstitute(GraphType = typeof(SOInvoiceEntry))]
	public partial class CustomerPaymentMethodC : CustomerPaymentMethod
	{
		public new abstract class pMInstanceID : PX.Data.IBqlField
		{
		}

		public new abstract class cCProcessingCenterID : PX.Data.IBqlField
		{
		}

		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}
		[PXDBInt]
		[PXDefault(typeof(SOOrder.customerID))]
		[PXParent(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<CustomerPaymentMethod.bAccountID>>>>))]
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

		#region PaymentMethodID
		public new abstract class paymentMethodID : PX.Data.IBqlField
		{
		}

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Card Type", Enabled = false)]
		[PXDefault()]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID, Where<PaymentMethod.isActive, Equal<boolTrue>,
				And<PaymentMethod.useForAR,Equal<True>,
				And<PaymentMethod.aRIsOnePerCustomer, Equal<False>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
		public override String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion

		#region CashAccountID
		public new abstract class cashAccountID : PX.Data.IBqlField
		{
		}

		[GL.CashAccount(null, typeof(Search2<CashAccount.cashAccountID, InnerJoin<PaymentMethodAccount,
		  On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>,
			And<PaymentMethodAccount.paymentMethodID, Equal<Current<CustomerPaymentMethodC.paymentMethodID>>>>>,
		  Where<Match<Current<AccessInfo.userName>>>>), DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Search<CA.PaymentMethodAccount.cashAccountID, 
				Where<CA.PaymentMethodAccount.paymentMethodID, Equal<Optional<CustomerPaymentMethodC.paymentMethodID>>,
				And<CA.PaymentMethodAccount.useForAR,Equal<True>>>,OrderBy<Desc<PaymentMethodAccount.aRIsDefault>>>))]
		public override Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion

		#region Descr
		public new abstract class descr : PX.Data.IBqlField
		{
		}
		
		[PXDBString(255, IsUnicode = true)]
		[PXDefault( PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Identifier", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public override String Descr
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
		
	}
	
	[System.SerializableAttribute()]
	public class InvoiceSplits : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool()]
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

		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXInt(IsKey = true)]
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

		#region ARTran
		#region TranType
		public abstract class tranTypeARTran : PX.Data.IBqlField
		{
		}
		protected String _TranTypeARTran;
		[PXString(3, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Tran. Type", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String TranTypeARTran
		{
			get
			{
				return this._TranTypeARTran;
			}
			set
			{
				this._TranTypeARTran = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbrARTran : PX.Data.IBqlField
		{
		}
		protected String _RefNbrARTran;
		[PXString(15, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String RefNbrARTran
		{
			get
			{
				return this._RefNbrARTran;
			}
			set
			{
				this._RefNbrARTran = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbrARTran : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbrARTran;
		[PXInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Int32? LineNbrARTran
		{
			get
			{
				return this._LineNbrARTran;
			}
			set
			{
				this._LineNbrARTran = value;
			}
		}
		#endregion

		#endregion

		#region SOLine
		#region OrderType
		public abstract class orderTypeSOLine : PX.Data.IBqlField
		{
		}
		protected String _OrderTypeSOLine;
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Order Type", Visible = false)]
		[PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
		public virtual String OrderTypeSOLine
		{
			get
			{
				return this._OrderTypeSOLine;
			}
			set
			{
				this._OrderTypeSOLine = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbrSOLine : PX.Data.IBqlField
		{
		}
		protected String _OrderNbrSOLine;
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Order Nbr.", Visible = false)]
		public virtual String OrderNbrSOLine
		{
			get
			{
				return this._OrderNbrSOLine;
			}
			set
			{
				this._OrderNbrSOLine = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbrSOLine : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbrSOLine;
		[PXInt()]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		public virtual Int32? LineNbrSOLine
		{
			get
			{
				return this._LineNbrSOLine;
			}
			set
			{
				this._LineNbrSOLine = value;
			}
		}
		#endregion

		#region LineTypeSOLine
		public abstract class lineTypeSOLine : PX.Data.IBqlField
		{
		}
		protected String _LineTypeSOLine;
		[PXString(2, IsFixed = true)]
		public virtual String LineTypeSOLine
		{
			get
			{
				return this._LineTypeSOLine;
			}
			set
			{
				this._LineTypeSOLine = value;
			}
		}
		#endregion


		#endregion

		#region INTran
		#region DocType
		public abstract class docTypeINTran : PX.Data.IBqlField
		{
		}
		protected String _DocTypeINTran;
		[PXString(1, IsFixed = true)]
		public virtual String DocTypeINTran
		{
			get
			{
				return this._DocTypeINTran;
			}
			set
			{
				this._DocTypeINTran = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbrINTran : PX.Data.IBqlField
		{
		}
		protected String _RefNbrINTran;
		[PXString(15, IsUnicode = true)]
		public virtual String RefNbrINTran
		{
			get
			{
				return this._RefNbrINTran;
			}
			set
			{
				this._RefNbrINTran = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbrINTran : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbrINTran;
		[PXInt()]
		public virtual Int32? LineNbrINTran
		{
			get
			{
				return this._LineNbrINTran;
			}
			set
			{
				this._LineNbrINTran = value;
			}
		}
		#endregion
		#region Inventory
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(DisplayName = "Inventory ID")]
		public virtual Int32? InventoryID
		{
			get
			{
				return _InventoryID;
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
		[IN.SubItem(typeof(inventoryID))]
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
		[IN.Site()]
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
		[IN.Location()]
		[PXDefault()]
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

		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(inventoryID), DisplayName = "UOM", Enabled = false)]
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

		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXQuantity()]
		[PXUIField(DisplayName = "Quantity")]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion

		#region BaseQty
		public abstract class baseQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseQty;
		[PXQuantity()]
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseQty;
			}
			set
			{
				this._BaseQty = value;
			}
		}
		#endregion

		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXString(256, IsUnicode = true)]
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

		#endregion

		#region INTranSplit
		#region DocType
		public abstract class docTypeINTranSplit : PX.Data.IBqlField
		{
		}
		protected String _DocTypeINTranSplit;
		[PXString(1, IsFixed = true)]
		public virtual String DocTypeINTranSplit
		{
			get
			{
				return this._DocTypeINTranSplit;
			}
			set
			{
				this._DocTypeINTranSplit = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbrINTranSplit : PX.Data.IBqlField
		{
		}
		protected String _RefNbrINTranSplit;
		[PXString(15, IsUnicode = true)]
		public virtual String RefNbrINTranSplit
		{
			get
			{
				return this._RefNbrINTranSplit;
			}
			set
			{
				this._RefNbrINTranSplit = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbrINTranSplit : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbrINTranSplit;
		[PXInt()]
		public virtual Int32? LineNbrINTranSplit
		{
			get
			{
				return this._LineNbrINTranSplit;
			}
			set
			{
				this._LineNbrINTranSplit = value;
			}
		}
		#endregion

		#region LineNbr
		public abstract class splitLineNbrINTranSplit : PX.Data.IBqlField
		{
		}
		protected Int32? _SplitLineNbrINTranSplit;
		[PXInt()]
		public virtual Int32? SplitLineNbrINTranSplit
		{
			get
			{
				return this._SplitLineNbrINTranSplit;
			}
			set
			{
				this._SplitLineNbrINTranSplit = value;
			}
		}
		#endregion
		#endregion

		#region SOSalesPerTran
		#region OrderType
		public abstract class orderTypeSOSalesPerTran : PX.Data.IBqlField
		{
		}
		protected String _OrderTypeSOSalesPerTran;
		[PXString(2, IsFixed = true)]
		public virtual String OrderTypeSOSalesPerTran
		{
			get
			{
				return this._OrderTypeSOSalesPerTran;
			}
			set
			{
				this._OrderTypeSOSalesPerTran = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbrSOSalesPerTran : PX.Data.IBqlField
		{
		}
		protected String _OrderNbrSOSalesPerTran;
		[PXString(15, IsUnicode = true)]
		public virtual String OrderNbrSOSalesPerTran
		{
			get
			{
				return this._OrderNbrSOSalesPerTran;
			}
			set
			{
				this._OrderNbrSOSalesPerTran = value;
			}
		}
		#endregion
		#region SalespersonID
		public abstract class salespersonIDSOSalesPerTran : PX.Data.IBqlField
		{
		}
		protected Int32? _SalespersonIDSOSalesPerTran;
		[PXInt()]
		public virtual Int32? SalespersonIDSOSalesPerTran
		{
			get
			{
				return this._SalespersonIDSOSalesPerTran;
			}
			set
			{
				this._SalespersonIDSOSalesPerTran = value;
			}
		}
		#endregion

		#endregion
	}

}
