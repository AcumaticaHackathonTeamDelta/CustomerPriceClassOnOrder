using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PX.Common;
using PX.Data;

using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.CA;
using PX.Objects.BQLConstants;
using PX.Objects.EP;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.DR;
using PX.Objects.AR;

using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using AP1099Hist = PX.Objects.AP.Overrides.APDocumentRelease.AP1099Hist;
using AP1099Yr = PX.Objects.AP.Overrides.APDocumentRelease.AP1099Yr;
using PX.Objects.GL.Reclassification.UI;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using Branch = PX.Objects.GL.Branch;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.AP
{ 
	[Serializable]
	public class APInvoiceEntry : APDataEntryGraph<APInvoiceEntry, APInvoice>, PXImportAttribute.IPXPrepareItems
	{
		#region Internal Definitions + Cache Attached Events 
		#region InventoryItem
		#region COGSSubID
		[PXDefault(typeof(Search<INPostClass.cOGSSubID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>))]
		[SubAccount(typeof(InventoryItem.cOGSAcctID), DisplayName = "Expense Sub.", DescriptionField = typeof(Sub.description))]
		public virtual void InventoryItem_COGSSubID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion
		#region LandedCostTran
		#region Source

		[PXDBString(2, IsFixed = true)]
		[PXDefault(LandedCostTranSource.FromAP)]
		[PXUIField(DisplayName = "PO Receipt Type")]
		protected virtual void LandedCostTran_Source_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region VendorID

		[AP.VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
		[PXDefault(typeof(APInvoice.vendorID))]
		[PXUIField(DisplayName = "Vendor ID", Visible = false)]
		protected virtual void LandedCostTran_VendorID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region VendorLocationID

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<LandedCostTran.vendorID>>>), DescriptionField = typeof(Location.descr), Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(APInvoice.vendorLocationID))]        
		protected virtual void LandedCostTran_VendorLocationID_CacheAttached(PXCache sender)
		{
		}
		#endregion
        #region InvoiceNbr
        protected String _InvoiceNbr;
        [PXDBString(40, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Vendor Ref.")]
        protected virtual void LandedCostTran_InvoiceNbr_CacheAttached(PXCache sender)
        {
        }
        #endregion
        [PXDBInt()]
        [PXLineNbr(typeof(APInvoice.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Enabled = false, Visible = false)]
        protected virtual void LandedCostTran_LineNbr_CacheAttached(PXCache sender)
        {
        }

		#region LandedCostCodeID

		[PXDBString(15, IsUnicode = true, IsFixed = false)]
		[PXDefault()]
		[PXUIField(DisplayName = "Landed Cost Code")]
		[PXSelector(typeof(Search<LandedCostCode.landedCostCodeID,
									Where<LandedCostCode.applicationMethod, Equal<LandedCostApplicationMethod.fromAP>,
												Or<LandedCostCode.applicationMethod, Equal<LandedCostApplicationMethod.fromBoth>>>>))]
		protected virtual void LandedCostTran_LandedCostCodeID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region POReceiptType

		[PXDBString(3, IsFixed = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[POReceiptType.List()]
		[PXUIField(DisplayName = "PO Receipt Type")]
		protected virtual void LandedCostTran_POReceiptType_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region POReceiptNbr

		[PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr, 
                                    Where<POReceipt.released, Equal<True>,
                                        And<POReceipt.receiptType, NotEqual<POReceiptType.poreturn>>>, OrderBy<Desc<POReceipt.receiptNbr>>>))]
		[PXUIField(DisplayName = "PO Receipt Nbr.")]
		protected virtual void LandedCostTran_POReceiptNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region POReceiptLineNbr

		[PXDBInt()]
		[PXUIField(DisplayName = "PO Receipt Line Nbr.")]
		protected virtual void LandedCostTran_POReceiptLineNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region APDocType

		[PXDBString(3, IsFixed = true)]
		[PXDBDefault(typeof(APInvoice.docType))]
		[PXUIField(DisplayName = "AP Doc. Type")]
		protected virtual void LandedCostTran_APDocType_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region APRefNbr

		[PXDBString(15, IsUnicode = true)]
		[PXDBDefault(typeof(APInvoice.refNbr))]
		[PXUIField(DisplayName = "AP Ref. Nbr.")]
        [PXParent(typeof(Select<APInvoice, Where<APInvoice.docType, Equal<Current<LandedCostTran.aPDocType>>,
                            And<APInvoice.refNbr, Equal<Current<LandedCostTran.aPRefNbr>>>>>),LeaveChildren = true)]
        protected virtual void LandedCostTran_APRefNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region INDocType

		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "IN Doc. Type")]
		protected virtual void LandedCostTran_INDocType_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region CuryID

		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBDefault(typeof(APInvoice.curyID))]
		protected virtual void LandedCostTran_CuryID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region APCuryInfoID

		[PXDBLong()]
		[CurrencyInfo(typeof(APInvoice.curyInfoID), ModuleCode = "AP")]        
		protected virtual void LandedCostTran_APCuryInfoID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region CuryInfoID

		[PXDBLong()]
        [PXDBDefault(typeof(APInvoice.curyInfoID))]
		protected virtual void LandedCostTran_CuryInfoID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region InventoryID

		[IN.Inventory(typeof(Search2<InventoryItem.inventoryID, 
			InnerJoin<POReceiptLine, On<POReceiptLine.inventoryID, Equal<InventoryItem.inventoryID>>>, 
			Where2<Match<Current<AccessInfo.userName>>, And<POReceiptLine.receiptNbr, Equal<Current<LandedCostTran.pOReceiptNbr>>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr), DisplayName = "Inventory ID")]
		protected virtual void LandedCostTran_InventoryID_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region CuryLCAPAmount

		[PXDBCurrency(typeof(LandedCostTran.aPCuryInfoID), typeof(LandedCostTran.lCAPAmount), MinValue = 0)]
		[PXFormula(typeof(Mult<LandedCostTran.curyLCAPEffAmount, LandedCostTran.amountSign>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
		protected virtual void LandedCostTran_CuryLCAPAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region LCAPAmount

		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void LandedCostTran_LCAPAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region CuryLCAmount

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Switch<Case<Where<LandedCostTran.iNDocCreated, Equal<True>>, LandedCostTran.curyLCAmount>, LandedCostTran.curyLCAPAmount>))]
		protected virtual void LandedCostTran_CuryLCAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region CuryLCAPEffAmount
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<LandedCostTran.curyLCAPEffAmount, NotEqual<decimal0>>), PXErrorLevel.Error, EP.Messages.ValueMustBeGreaterThanZero, CheckOnInserted = false)]
		protected virtual void LandedCostTran_CuryLCAPEffAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region LCAmount

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void LandedCostTran_LCAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region InvoiceDate

		[PXDBDate()]
		[PXDBDefault(typeof(APInvoice.docDate))]
		protected virtual void LandedCostTran_InvoiceDate_CacheAttached(PXCache sender)
		{
		}
		#endregion
		
		#endregion        
		#region APTran
		#region LCTranID

		[PXDBInt()]
		[PXDBLiteDefault(typeof(LandedCostTran.lCTranID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Enabled = false, Visible = false)]
		protected virtual void APTran_LCTranID_CacheAttached(PXCache sender)
		{
		}        
		#endregion

		[PXDBDefault(typeof(APRegister.branchID))]
		[Branch(Enabled = false)]
		protected virtual void APTaxTran_BranchID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region LandedCostTranR

		[PXProjection(typeof(Select2<LandedCostTran, InnerJoin<POReceipt, On<POReceipt.receiptNbr, Equal<LandedCostTran.pOReceiptNbr>, And<POReceipt.receiptType, Equal<LandedCostTran.pOReceiptType>>>>, Where<LandedCostTran.postponeAP, Equal<True>, And<LandedCostTran.aPRefNbr, IsNull, And<POReceipt.released, Equal<True>>>>>), Persistent = false)]
		[PXCacheName(Messages.LandedCostTranR)]
		[Serializable]
		public partial class LandedCostTranR : LandedCostTran
		{
			#region Selected
			public abstract class selected : IBqlField
			{
			}
			protected bool? _Selected = false;
			[PXBool]
			[PXUIField(DisplayName = "Selected")]
			public bool? Selected
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
		}
		#endregion
		#region APInvoice
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		[VendorActiveOrHoldPayments(
			Visibility = PXUIVisibility.SelectorVisible,
			DescriptionField = typeof(Vendor.acctName),
			CacheGlobal = true,
			Filterable = true)]
		[PXDefault]
		protected virtual void APInvoice_VendorID_CacheAttached(PXCache sender) { }
		#endregion
		#endregion

		#region EP Approval Defaulting
		[PXDBDate]
		[PXDefault(typeof(APInvoice.docDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt]
		[PXDefault(typeof(APInvoice.vendorID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(APInvoice.docDesc), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong]
		[CurrencyInfo(typeof(APInvoice.curyInfoID))]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal]
		[PXDefault(typeof(APInvoice.curyOrigDocAmt), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal]
		[PXDefault(typeof(APInvoice.origDocAmt), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}

		protected virtual void EPApproval_SourceItemType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null)
			{
				e.NewValue = new APDocType.ListAttribute()
					.ValueLabelDic[Document.Current.DocType];

				e.Cancel = true;
			}		
		}
		#endregion

		#region EP Approval Actions

		public PXAction<APInvoice> approve;
		public PXAction<APInvoice> reject;

		[PXUIField(DisplayName = "Approve", Visible = false)]
		public IEnumerable Approve(PXAdapter adapter)
		{
			IEnumerable<APInvoice> invoices = adapter.Get<APInvoice>().ToArray();

			Save.Press();

			foreach (APInvoice invoice in invoices)
			{
				invoice.Approved = true;
				Document.Update(invoice);

				Save.Press();

				yield return invoice;
			}
		}

		[PXUIField(DisplayName = "Reject", Visible = false)]
		public IEnumerable Reject(PXAdapter adapter)
		{
			IEnumerable<APInvoice> invoices = adapter.Get<APInvoice>().ToArray();

			Save.Press();

			foreach (APInvoice invoice in invoices)
			{
				invoice.Rejected = true;
				Document.Update(invoice);

				Save.Press();

				yield return invoice;
			}
		}

		#endregion

		#region Buttons
		public ToggleCurrency<APInvoice> CurrencyView;

		public static void ViewScheduleForLine(PXGraph graph, APInvoice document, APTran tran)
		{
			PXSelectBase<DRSchedule> correspondingScheduleView = new PXSelect<
				DRSchedule,
				Where<
					DRSchedule.module, Equal<BatchModule.moduleAP>,
					And<DRSchedule.docType, Equal<Current<APTran.tranType>>,
					And<DRSchedule.refNbr, Equal<Current<APTran.refNbr>>,
					And<DRSchedule.lineNbr, Equal<Current<APTran.lineNbr>>>>>>>
				(graph);

			DRSchedule correspondingSchedule = correspondingScheduleView.Select();

			if (correspondingSchedule == null || correspondingSchedule.IsDraft == true)
				{
				PXResult<APTax, Tax> tax = (PXResult<APTax, Tax>)PXSelectJoin<
					APTax,
						LeftJoin<Tax, 
						On<Tax.taxID, Equal<APTax.taxID>>>,
					Where<
						APTax.tranType, Equal<Required<APTax.tranType>>, 
						And<APTax.refNbr, Equal<Required<APTax.refNbr>>, 
						And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>>>>>
						.Select(graph, tran.TranType, tran.RefNbr, tran.LineNbr);

				var actualAmount = APReleaseProcess.GetExpensePostingAmount(tran, tax, tax,
						amount => PXDBCurrencyAttribute.Round(graph.Caches[typeof(APTran)], tran, amount, CMPrecision.TRANCURY));

				DRDeferredCode deferralCode = PXSelect<
					DRDeferredCode, 
					Where<
						DRDeferredCode.deferredCodeID, Equal<Current2<APTran.deferredCode>>>>
					.Select(graph);

				if (deferralCode != null)
					{
						DRProcess process = PXGraph.CreateInstance<DRProcess>();
					process.CreateSchedule(tran, deferralCode, document, actualAmount.Base.Value, isDraft: true);
						process.Actions.PressSave();

					correspondingScheduleView.Cache.Clear();
					correspondingScheduleView.Cache.ClearQueryCache();

					correspondingSchedule = correspondingScheduleView.Select();
					}
				}

			if (correspondingSchedule != null)
				{
					DraftScheduleMaint target = PXGraph.CreateInstance<DraftScheduleMaint>();
					target.Clear();
				target.Schedule.Current = correspondingSchedule;

					throw new PXRedirectRequiredException(target, true, "ViewSchedule") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
		}

		public PXAction<APInvoice> viewSchedule;
		[PXUIField(DisplayName = "View Schedule", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Settings)]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			APTran currentLine = Transactions.Current;

			if (currentLine != null && 
				Transactions.Cache.GetStatus(currentLine) == PXEntryStatus.Notchanged)
			{
				Save.Press();
				ViewScheduleForLine(this, Document.Current, Transactions.Current);
			}

			return adapter.Get();
		}


		public PXAction<APInvoice> newVendor;
		[PXUIField(DisplayName = "New Vendor", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewVendor(PXAdapter adapter)
		{
			VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
			throw new PXRedirectRequiredException(graph, "New Vendor");
		}

		public PXAction<APInvoice> editVendor;
		[PXUIField(DisplayName = "Edit Vendor", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditVendor(PXAdapter adapter)
		{
			if (vendor.Current != null)
			{
				VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
				graph.BAccount.Current = (VendorR)vendor.Current;
				throw new PXRedirectRequiredException(graph, "Edit Vendor");
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> vendorDocuments;
		[PXUIField(DisplayName = "Vendor Details", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable VendorDocuments(PXAdapter adapter)
		{
			if (vendor.Current != null)
			{
				APDocumentEnq graph = PXGraph.CreateInstance<APDocumentEnq>();
				graph.Filter.Current.VendorID = vendor.Current.BAccountID;
				graph.Filter.Select();
				throw new PXRedirectRequiredException(graph, "Vendor Details");
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<APRegister> list = new List<APRegister>();
			foreach (APInvoice apdoc in adapter.Get<APInvoice>())
			{
				if (!(bool)apdoc.Hold && !(bool)apdoc.Released)
				{
					cache.Update(apdoc);
					list.Add(apdoc);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}

			if (!IsExternalTax)
			{
				Save.Press();
				PXLongOperation.StartOperation(this, delegate() { APDocumentRelease.ReleaseDoc(list, false); });
			}
			else
			{
				try
				{
					skipAvalaraCallOnSave = true;
					Save.Press();
				}
				finally
				{
					skipAvalaraCallOnSave = false;
				}

				PXLongOperation.StartOperation(this, delegate()
				{
					List<APRegister> listWithTax = new List<APRegister>();
					foreach (APInvoice apdoc in list)
					{
						if (apdoc.IsTaxValid != true && AvalaraMaint.IsExternalTax(this,apdoc.TaxZoneID) && apdoc.InstallmentNbr == null)
						{
							APInvoice doc = new APInvoice();
							doc.DocType = apdoc.DocType;
							doc.RefNbr = apdoc.RefNbr;
							doc.OrigModule = apdoc.OrigModule;
							listWithTax.Add(APExternalTaxCalc.Process(doc));
						}
						else
						{
							listWithTax.Add(apdoc);
						}
					}

					APDocumentRelease.ReleaseDoc(listWithTax, false);
				});
			}
			
			return list;
		}

		public PXAction<APInvoice> prebook;
		[PXUIField(DisplayName = "Pre-release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable Prebook(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<APRegister> list = new List<APRegister>();
			foreach (APInvoice apdoc in adapter.Get<APInvoice>())
			{
				if (!(bool)apdoc.Hold && !(bool)apdoc.Released && apdoc.Prebooked != true)
				{
					if (apdoc.PrebookAcctID == null)
					{
						cache.RaiseExceptionHandling<APInvoice.prebookAcctID>(apdoc, apdoc.PrebookAcctID, new PXSetPropertyException(Messages.PrebookingAccountIsRequiredForPrebooking)); 
						continue;
					}
					if (apdoc.PrebookSubID == null)
					{
						cache.RaiseExceptionHandling<APInvoice.prebookSubID>(apdoc, apdoc.PrebookSubID, new PXSetPropertyException(Messages.PrebookingAccountIsRequiredForPrebooking));
						continue;
					}						
					cache.Update(apdoc);
					list.Add(apdoc);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(ErrorMessages.RecordRaisedErrors, "Updating", this.Document.Cache.GetItemType().Name);
			}
			Persist();
			PXLongOperation.StartOperation(this, delegate() { APDocumentRelease.ReleaseDoc(list, false, true); });
			return list;
		}

		public PXAction<APInvoice> voidInvoice;
		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable VoidInvoice(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<APRegister> list = new List<APRegister>();
			foreach (APInvoice apdoc in adapter.Get<APInvoice>())
			{
				if (apdoc.Released == true || apdoc.Prebooked == true)
				{
					cache.Update(apdoc);
					list.Add(apdoc);
				}
			}

			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}
			Persist();
			PXLongOperation.StartOperation(this, delegate() { APDocumentRelease.VoidDoc(list); });
			return list;
		}

		public PXAction<APInvoice> vendorRefund;
		[PXUIField(DisplayName = "Vendor Refund", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable VendorRefund(PXAdapter adapter)
		{
			if (Document.Current != null && (bool)Document.Current.Released && Document.Current.DocType == APDocType.DebitAdj && (bool)Document.Current.OpenDoc)
			{
				APPaymentEntry pe = PXGraph.CreateInstance<APPaymentEntry>();

				APAdjust adj = PXSelect<APAdjust, Where<APAdjust.adjdDocType, Equal<Current<APInvoice.docType>>,
					And<APAdjust.adjdRefNbr, Equal<Current<APInvoice.refNbr>>, And<APAdjust.released, Equal<False>>>>>.Select(this);

				if (adj != null)
				{
					pe.Document.Current = pe.Document.Search<APPayment.refNbr>(adj.AdjgRefNbr, adj.AdjgDocType);
				}
				else
				{
					pe.Clear();
					pe.CreatePayment(Document.Current);
				}
				throw new PXRedirectRequiredException(pe, "PayInvoice");
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> reverseInvoice;
		[PXUIField(DisplayName = Messages.APReverseBill, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReverseInvoice(PXAdapter adapter)
		{
			if (Document.Current != null && (Document.Current.DocType == APDocType.Invoice ||
											 Document.Current.DocType == APDocType.CreditAdj))
			{
				if (Document.Current.InstallmentNbr != null && string.IsNullOrEmpty(Document.Current.MasterRefNbr) == false)
				{
					throw new PXSetPropertyException(Messages.Multiply_Installments_Cannot_be_Reversed, Document.Current.MasterRefNbr);
				}

				this.Save.Press();
				bool isPOReferenced = false;
				foreach (APTran itr in this.Transactions.Select())
				{
					if (!string.IsNullOrEmpty(itr.ReceiptNbr))
					{
						isPOReferenced = true;
						break;
					}
				}
				if (isPOReferenced)
				{
					this.Document.Ask(Messages.Warning, Messages.DebitAdjustmentRowReferecesPOOrderOrPOReceipt, MessageButtons.OK, MessageIcon.Warning);
				}
				APInvoice doc = PXCache<APInvoice>.CreateCopy(Document.Current);
				FiscalPeriodUtils.VerifyFinPeriod<APInvoice.finPeriodID, FinPeriod.aPClosed>(this, Document.Cache, doc, finperiod);
				
				try
				{
					this.ReverseInvoiceProc(doc);

					Document.Cache.RaiseExceptionHandling<APInvoice.finPeriodID>(Document.Current, Document.Current.FinPeriodID, null);

					return new List<APInvoice> { Document.Current };
				}
				catch (PXException)
				{
					this.Clear(PXClearOption.PreserveTimeStamp);
					Document.Current = doc;
					throw;
				}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> reclassifyBatch;
		[PXUIField(DisplayName = Messages.ReclassifyGLBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReclassifyBatch(PXAdapter adapter)
		{
			var document = Document.Current;

			if (document != null)
			{
				ReclassifyTransactionsProcess.TryOpenForReclassificationOfDocument(Document.View, 
					BatchModule.AP, document.BatchNbr, document.DocType, document.RefNbr);
			}

			return adapter.Get();
		}

		public PXAction<APInvoice> payInvoice;
		[PXUIField(DisplayName = Messages.APPayBill, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable PayInvoice(PXAdapter adapter)
		{
			if (Document.Current != null && (Document.Current.Released == true || Document.Current.Prebooked == true))
			{
				APPaymentEntry pe = PXGraph.CreateInstance<APPaymentEntry>();
				if (Document.Current.OpenDoc == true && (Document.Current.Payable == true || Document.Current.DocType == APInvoiceType.Prepayment))
				{
					string adjRefNbr = null;
					string adjDocType = null;

					APAdjust adj = PXSelect<APAdjust, Where<APAdjust.adjdDocType, Equal<Current<APInvoice.docType>>,
						And<APAdjust.adjdRefNbr, Equal<Current<APInvoice.refNbr>>>>>.Select(this);

					if (adj != null)
					{
						if (adj.Released == true)
						{
							if 
							(	adj.AdjdDocType == APDocType.Prepayment &&
								PXSelect<APPayment, Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
									And<APPayment.docType, Equal<Required<APPayment.docType>>>>>.Select(this, Document.Current.RefNbr, Document.Current.DocType).Count > 0)
							{
								adjRefNbr = Document.Current.RefNbr;
								adjDocType = Document.Current.DocType;
							}
						}
						else
						{
							adjRefNbr = adj.AdjgRefNbr;
							adjDocType = adj.AdjgDocType;
						}
					}

					if (adjRefNbr != null && adjDocType != null)
					{
						pe.Document.Current = pe.Document.Search<APPayment.refNbr>(adjRefNbr, adjDocType);
					}
					else
					{
						pe.Clear();
						pe.CreatePayment(Document.Current);
					}

					throw new PXRedirectRequiredException(pe, "PayInvoice");
				}
				else if (Document.Current.DocType == APDocType.DebitAdj)
				{
					pe.Document.Current = pe.Document.Search<APPayment.refNbr>(Document.Current.RefNbr, Document.Current.DocType);
					throw new PXRedirectRequiredException(pe, "PayInvoice");
				}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> createSchedule;
		[PXUIField(DisplayName = "Assign to Schedule", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton(ImageKey = PX.Web.UI.Sprite.Main.Shedule)]
		public virtual IEnumerable CreateSchedule(PXAdapter adapter)
		{
			APInvoice currentDocument = Document.Current;

			if (currentDocument == null) return adapter.Get();

			Save.Press();

			if (currentDocument.Released != true && currentDocument.Hold != true)
			{
				APScheduleMaint scheduleMaint = PXGraph.CreateInstance<APScheduleMaint>();

				if (currentDocument.Scheduled == true && currentDocument.ScheduleID != null)
				{
					scheduleMaint.Schedule_Header.Current = 
						scheduleMaint.Schedule_Header.Search<Schedule.scheduleID>(currentDocument.ScheduleID);
				}
				else
				{
					scheduleMaint.Schedule_Header.Cache.Insert();

					APRegister scheduledDocumentRecord = 
						scheduleMaint.Document_Detail.Cache.CreateInstance() as APRegister;

					PXCache<APRegister>.RestoreCopy(scheduledDocumentRecord, currentDocument);

					scheduledDocumentRecord = 
						scheduleMaint.Document_Detail.Cache.Update(scheduledDocumentRecord) as APRegister;
				}

				throw new PXRedirectRequiredException(scheduleMaint, "create Schedule");
			}

			return adapter.Get();
		}

		public PXAction<APInvoice> addPOReceipt;
		[PXUIField(DisplayName = PO.Messages.AddPOReceipt, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true, FieldClass = "DISTR")]
		[PXLookupButton]
		public virtual IEnumerable AddPOReceipt(PXAdapter adapter)
		{
			checkTaxCalcMode();
			if (this.Document.Current != null &&
					this.Document.Current.Released == false &&
					this.Document.Current.Prebooked == false &&
				this.poreceiptslist.AskExt(
					(graph, view) =>
					{
						filter.Cache.ClearQueryCache();
						filter.View.Clear();
						filter.Cache.Clear();

						poreceiptslist.Cache.ClearQueryCache();
						poreceiptslist.View.Clear();
						poreceiptslist.Cache.Clear();
					}
					, true) == WebDialogResult.OK)
			{
				updateTaxCalcMode();
				AddPOReceipt2(adapter);
			}
			poreceiptslist.View.Clear();
			poreceiptslist.Cache.Clear();
			return adapter.Get();
		}
		public PXAction<APInvoice> addPOReceipt2;
		[PXUIField(DisplayName = PO.Messages.AddPOReceipt, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddPOReceipt2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released == false &&
					this.Document.Current.Prebooked == false )
			{
				foreach (POReceipt rc in poreceiptslist.Cache.Updated)
				{
					if (rc.Selected == true)
					{
						this.InvoicePOReceipt(rc, null, null, false, false);
					}
				}
			}
			poreceiptslist.View.Clear();
			poreceiptslist.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<APInvoice> addReceiptLine;
		[PXUIField(DisplayName = PO.Messages.AddPOReceiptLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true, FieldClass = "DISTR")]
		[PXLookupButton]
		public virtual IEnumerable AddReceiptLine(PXAdapter adapter)
		{
			checkTaxCalcMode();
			if (this.Document.Current != null &&
				this.Document.Current.Released == false &&
				this.Document.Current.Prebooked == false &&
				this.poReceiptLinesSelection.AskExt(
					(graph, view) =>
					{
						filter.Cache.ClearQueryCache();
						filter.View.Clear();
						filter.Cache.Clear();

						poReceiptLinesSelection.Cache.ClearQueryCache();
						poReceiptLinesSelection.View.Clear();
						poReceiptLinesSelection.Cache.Clear();
					}
					, true) == WebDialogResult.OK)
			{
				updateTaxCalcMode();
				return AddReceiptLine2(adapter);
			}
			return adapter.Get();
		}
		public PXAction<APInvoice> addReceiptLine2;
		[PXUIField(DisplayName = PO.Messages.AddPOReceiptLine, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddReceiptLine2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				this.Document.Current.Released == false &&
				this.Document.Current.Prebooked == false)
			{
				HashSet<APTran> duplicates = new HashSet<APTran>(new POReceiptLineComparer());
				foreach (APTran tran in this.Transactions.Select())
				{
					try
					{
						duplicates.Add(tran);
					}
					catch (NullReferenceException) { }
				}

				foreach (POReceiptLineAdd it in poReceiptLinesSelection.Select())
				{
					if (it.Selected == true)
					{
						this.AddPOReceiptLine(it, duplicates);
					}
				}
			}
			poReceiptLinesSelection.View.Clear();
			poReceiptLinesSelection.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<APInvoice> addPOOrder;
		[PXUIField(DisplayName = PO.Messages.AddPOOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true, FieldClass = "DISTR")]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrder(PXAdapter adapter)
		{
			checkTaxCalcMode();
			if (this.Document.Current != null &&
				this.Document.Current.DocType == APDocType.Invoice &&
				this.Document.Current.Released == false &&
				this.Document.Current.Prebooked == false &&
				this.poorderslist.AskExt(
					(graph, view) =>
					{
						filter.Cache.ClearQueryCache();
						filter.View.Clear();
						filter.Cache.Clear();

						poorderslist.Cache.ClearQueryCache();
						poorderslist.View.Clear();
						poorderslist.Cache.Clear();
					}, true) == WebDialogResult.OK)
			{
				updateTaxCalcMode();
				return AddPOOrder2(adapter);
			}
			return adapter.Get();
		}
		public PXAction<APInvoice> addPOOrder2;
		[PXUIField(DisplayName = PO.Messages.AddPOOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddPOOrder2(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				this.Document.Current.DocType == APDocType.Invoice &&
				this.Document.Current.Released == false &&
				this.Document.Current.Prebooked == false)
			{
				foreach (POOrder rc in poorderslist.Cache.Updated)
				{
					if (rc.Selected == true)
					{
						this.InvoicePOOrder(rc, false);
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> linkLine;
		[PXUIField(DisplayName = Messages.LinkLine, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, FieldClass = "DISTR")]
		[PXLookupButton]
		public virtual IEnumerable LinkLine(PXAdapter adapter)
		{
			if (Transactions.Current.InventoryID != null)
			{
				WebDialogResult res;
				if ((res = linkLineFilter.AskExt(
					(graph, view) =>
					{
						linkLineFilter.Cache.SetValueExt<LinkLineFilter.inventoryID>(linkLineFilter.Current, Transactions.Current.InventoryID);
						linkLineFilter.Current.UOM = Transactions.Current.UOM;
						linkLineFilter.Current.POOrderNbr = null;
						linkLineFilter.Current.SiteID = null;

						APTran apTran = Transactions.Current;

						linkLineOrderTran.Cache.Clear(); //TODO: closing modal window can't be handled
						linkLineReceiptTran.Cache.Clear(); //TODO: closing modal window can't be handled
						linkLineOrderTran.View.Clear(); //TODO: closing modal window can't be handled
						linkLineReceiptTran.View.Clear(); //TODO: closing modal window can't be handled
						linkLineOrderTran.Cache.ClearQueryCache(); //TODO: closing modal window can't be handled
						linkLineReceiptTran.Cache.ClearQueryCache(); //TODO: closing modal window can't be handled
					}, true
					)) != WebDialogResult.None)
				{
					if (res == WebDialogResult.Yes && (linkLineReceiptTran.Cache.Updated.Count() > 0 || linkLineOrderTran.Cache.Updated.Count() > 0))
					{
						APTran apTran = Transactions.Current;
						apTran.ReceiptType = null;
						apTran.ReceiptNbr = null;
						apTran.ReceiptLineNbr = null;
						apTran.POOrderType = null;
						apTran.PONbr = null;
						apTran.POLineNbr = null;
						apTran.AccountID = null;
						apTran.SubID = null;

						if (linkLineFilter.Current.SelectedMode == LinkLineFilter.selectedMode.Receipt)
						{
							foreach (POReceiptLineS receipt in linkLineReceiptTran.Cache.Updated)
							{
								if ((bool)receipt.Selected == true)
								{
									apTran.POOrderType = receipt.POType;
									apTran.PONbr = receipt.PONbr;
									apTran.POLineNbr = receipt.POLineNbr;
									apTran.ReceiptType = receipt.ReceiptType;
									apTran.ReceiptNbr = receipt.ReceiptNbr;
									apTran.ReceiptLineNbr = receipt.LineNbr;
									apTran.AccountID = receipt.POAccrualAcctID ?? receipt.ExpenseAcctID;
									apTran.SubID = receipt.POAccrualSubID ?? receipt.ExpenseSubID;
									break;
								}
							}
						}
						if (linkLineFilter.Current.SelectedMode == LinkLineFilter.selectedMode.Order)
						{
							foreach (POLine order in linkLineOrderTran.Cache.Updated)
							{
								if ((bool?) order.Selected == true)
								{
									apTran.POOrderType = order.OrderType;
									apTran.PONbr = order.OrderNbr;
									apTran.POLineNbr = order.LineNbr;
									apTran.AccountID = order.ExpenseAcctID;
									apTran.SubID = order.ExpenseSubID;
									break;
								}
							}
						}
						Transactions.Cache.Update(apTran);
						if (string.IsNullOrEmpty(apTran.ReceiptNbr) && string.IsNullOrEmpty(apTran.PONbr))
						{
							Transactions.Cache.SetDefaultExt<APTran.accountID>(apTran);
							Transactions.Cache.SetDefaultExt<APTran.subID>(apTran);
						}
					}
				}

			}
			return adapter.Get();
		}


		private void checkTaxCalcMode()
		{
			APInvoice current = Document.Current;
			if (PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() && current.TaxCalcMode != VendorClass.taxCalcMode.TaxSetting)
			{
				if (Transactions.Select().Count != 0)
				{
					throw new PXException(Messages.BillShouldInBeTaxSettingsMode);
				}
			}
		}

		private void updateTaxCalcMode()
		{
			checkTaxCalcMode();
			APInvoice current = Document.Current;
			current.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
			Document.Update(current);
		}


		public PXAction<APInvoice> viewPODocument;
		[PXUIField(DisplayName = PO.Messages.ViewPODocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewPODocument(PXAdapter adapter)
		{
			if (this.Transactions.Current != null)
			{
				APTran row = this.Transactions.Current;
				if (!String.IsNullOrEmpty(row.ReceiptNbr))
				{
					POReceiptEntry docGraph = PXGraph.CreateInstance<POReceiptEntry>();
					docGraph.Document.Current = docGraph.Document.Search<POReceipt.receiptNbr>(row.ReceiptNbr);
					if (docGraph.Document.Current != null)
						throw new PXRedirectRequiredException(docGraph, "View PO Receipt");
				}
				else
					if (!(String.IsNullOrEmpty(row.POOrderType) || String.IsNullOrEmpty(row.PONbr)))
					{
						POOrderEntry docGraph = PXGraph.CreateInstance<POOrderEntry>();
						docGraph.Document.Current = docGraph.Document.Search<POOrder.orderNbr>(row.PONbr, row.POOrderType);
						if (docGraph.Document.Current != null)
							throw new PXRedirectRequiredException(docGraph, "View PO Order");
					}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> autoApply;
		[PXUIField(DisplayName = "Auto Apply", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AutoApply(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.DocType == APDocType.Invoice
				&& Document.Current.Released == false &&
				this.Document.Current.Prebooked == false)
			{
				foreach (PXResult<APAdjust, APPayment, Standalone.APRegisterAlias, CurrencyInfo> res in Adjustments_Raw.Select())
				{
					APAdjust adj = res;

					adj.CuryAdjdAmt = 0m;
				}

				decimal? CuryApplAmt = Document.Current.CuryDocBal;

				foreach (PXResult<APAdjust, APPayment> res in Adjustments.Select())
				{
					APAdjust adj = (APAdjust)res;

					if (adj.CuryDocBal > 0m)
					{
						if (adj.CuryDocBal > CuryApplAmt)
						{
							adj.CuryAdjdAmt = CuryApplAmt;
							CuryApplAmt = 0m;
							Adjustments.Cache.RaiseFieldUpdated<APAdjust.curyAdjdAmt>(adj, 0m);
							if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Notchanged)
							{
								Adjustments.Cache.SetStatus(adj, PXEntryStatus.Updated);
							}
							break;
						}
						else
						{
							adj.CuryAdjdAmt = adj.CuryDocBal;
							CuryApplAmt -= adj.CuryDocBal;
							Adjustments.Cache.RaiseFieldUpdated<APAdjust.curyAdjdAmt>(adj, 0m);
							if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Notchanged)
							{
								Adjustments.Cache.SetStatus(adj, PXEntryStatus.Updated);
							}
						}
					}
				}
				Adjustments.View.RequestRefresh();
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> voidDocument;
		[PXUIField(DisplayName = Messages.Void, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable VoidDocument(PXAdapter adapter)
		{
			APRegister doc = (APRegister)Document.Current;
			if (doc != null && doc.DocType == APInvoiceType.Prepayment)
			{
				APAdjust adj = PXSelect<APAdjust, Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>,
													And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>>>>
							   .Select(this, doc.DocType, doc.RefNbr);

				if (adj != null)
					throw new PXException(
						adj.Released == true ?
							Messages.PrepaymentCannotBeVoidedDueToReleasedCheck :
							Messages.PrepaymentCannotBeVoidedDueToUnreleasedCheck,
						adj.AdjgRefNbr);

				this.VoidPrepayment(doc);

				List<APInvoice> rs = new List<APInvoice>();
				rs.Add(Document.Current);
				return rs;
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> viewPayment;
		[PXUIField(
			DisplayName = "View Document", 
			MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select,
			Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewPayment(PXAdapter adapter)
		{
			if (Document.Current != null && Adjustments.Current != null)
			{
				switch(Adjustments.Current.AdjType)
				{
					case ARAdjust.adjType.Adjusting:
					{
						APPaymentEntry pe = CreateInstance<APPaymentEntry>();
				pe.Document.Current = pe.Document.Search<APPayment.refNbr>(Adjustments.Current.AdjgRefNbr, Adjustments.Current.AdjgDocType);

						throw new PXRedirectRequiredException(pe, true, "Payment") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
					case ARAdjust.adjType.Adjusted:
					{
						APInvoiceEntry pe = CreateInstance<APInvoiceEntry>();
						pe.Document.Current = pe.Document.Search<APInvoice.refNbr>(Adjustments.Current.AdjdRefNbr, Adjustments.Current.AdjdDocType);

						throw new PXRedirectRequiredException(pe, true, "Invoice") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> recalculateDiscountsAction;
		[PXUIField(DisplayName = "Recalculate Prices", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXButton]
		public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
		{
			if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
			{
				DiscountEngine<APTran>.RecalculatePricesAndDiscounts<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, DiscountDetails, Document.Current.VendorLocationID, Document.Current.DocDate, recalcdiscountsfilter.Current);
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> recalcOk;
		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable RecalcOk(PXAdapter adapter)
		{
			return adapter.Get();
		}

		#region Landed Cost
		public PXAction<APInvoice> viewLCPOReceipt;
		[PXUIField(DisplayName = PO.Messages.ViewPODocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewLCPOReceipt(PXAdapter adapter)
		{
			if (this.landedCostTrans.Current != null)
			{
				LandedCostTran row = this.landedCostTrans.Current;
				if (!String.IsNullOrEmpty(row.POReceiptNbr))
				{
					POReceiptEntry docGraph = PXGraph.CreateInstance<POReceiptEntry>();
					docGraph.Document.Current = docGraph.Document.Search<POReceipt.receiptNbr>(row.POReceiptNbr);
					if (docGraph.Document.Current != null)
						throw new PXRedirectRequiredException(docGraph, true, "View PO Receipt"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
				}
			}
			return adapter.Get();
		}

		public PXAction<APInvoice> viewLCINDocument;
		[PXUIField(DisplayName = PO.Messages.ViewINDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ViewLCINDocument(PXAdapter adapter)
		{
			LandedCostTran doc = this.landedCostTrans.Current;
			if (doc != null && String.IsNullOrEmpty(doc.INRefNbr) != true)
			{
				INAdjustmentEntry inReceiptGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
				inReceiptGraph.adjustment.Current = inReceiptGraph.adjustment.Search<INRegister.refNbr>(doc.INRefNbr);
				throw new PXRedirectRequiredException(inReceiptGraph, true, PO.Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };

			}
			return adapter.Get();
		}


		public PXAction<APInvoice> addPostLandedCostTran;
		[PXUIField(DisplayName = Messages.AddPostponedLandedCost, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable AddPostLandedCostTran(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Released == false &&
				this.landedCostTranSelection.AskExt() == WebDialogResult.OK)
			{
				foreach (LandedCostTranR rc in landedCostTranSelection.Cache.Updated)
				{
					if (rc.Selected == true)
					{
						LandedCostTran newTran = (LandedCostTran) this.landedCostTrans.Cache.CreateCopy(rc);
						newTran.APDocType = this.Document.Current.DocType;
						newTran.APRefNbr = this.Document.Current.RefNbr;
						newTran.APCuryInfoID = this.Document.Current.CuryInfoID;
						newTran = this.landedCostTrans.Update(newTran);	
					}
				}
			}
			poreceiptslist.View.Clear();
			poreceiptslist.Cache.Clear();
			return adapter.Get();
		}


		public PXAction<APInvoice> lsLCSplits;
		[PXUIField(DisplayName = Messages.LandedCostSplit, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]

		public virtual IEnumerable LsLCSplits(PXAdapter adapter)
		{
			LandedCostTran doc = this.landedCostTrans.Current;
			if (doc != null)
			{
				this.landedCostTrans.View.AskExt(true);
			}
			return adapter.Get();
		}
		#endregion


		#endregion

		#region Selects
		public PXSelect<
			InventoryItem, 
			Where<
				InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> 
			nonStockItem;

		[PXCopyPasteHiddenFields(typeof(APInvoice.invoiceNbr), FieldsToShowInSimpleImport = new[] { typeof(APInvoice.invoiceNbr) })]
		public PXSelectJoin<
			APInvoice, 
					LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<APInvoice.vendorID>>>,
			Where<
				APInvoice.docType, Equal<Optional<APInvoice.docType>>,
				And2<Where<
					APInvoice.origModule, NotEqual<BatchModule.moduleTX>, 
					Or<APInvoice.released, Equal<True>>>,
				And<Where<
					Vendor.bAccountID, IsNull, 
					Or<Match<Vendor, Current<AccessInfo.userName>>>>>>>> 
			Document;

		[PXCopyPasteHiddenFields(typeof(APInvoice.paySel), typeof(APInvoice.payDate))]
		public PXSelect<APInvoice, Where<APInvoice.docType, Equal<Current<APInvoice.docType>>, And<APInvoice.refNbr, Equal<Current<APInvoice.refNbr>>>>> CurrentDocument;

		// This view must be declared earlier than APTran views
		// for correct work of DBLiteDefault attribute in APTran.
		// -
		[PXCopyPasteHiddenView]
		public PXSelect<
			LandedCostTran, 
			Where<
				LandedCostTran.aPDocType, Equal<Current<APInvoice.docType>>,
											And<LandedCostTran.aPRefNbr, Equal<Current<APInvoice.refNbr>>,
				And<Where<
					LandedCostTran.source, Equal<LandedCostTranSource.fromAP>,
					Or<LandedCostTran.postponeAP, Equal<True>>>>>>, 
			OrderBy<Asc<LandedCostTran.lCTranID>>> 
			landedCostTrans;

		[PXImport(typeof(APInvoice))]
		[PXCopyPasteHiddenFields(
			typeof(APTran.pOOrderType), 
			typeof(APTran.pONbr), 
			typeof(APTran.pOLineNbr), 
			typeof(APTran.receiptNbr), 
			typeof(APTran.receiptLineNbr), 
			typeof(APTran.pPVDocType), 
			typeof(APTran.pPVRefNbr))]
		public PXSelectJoin<
			APTran, 
				LeftJoin<POReceiptLine, 
					On<POReceiptLine.receiptNbr, Equal<APTran.receiptNbr>, 
				And<POReceiptLine.lineNbr, Equal<APTran.receiptLineNbr>>>>,
			Where<
				APTran.tranType, Equal<Current<APInvoice.docType>>,
				And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>>>,
			OrderBy<
				Asc<APTran.tranType, 
				Asc<APTran.refNbr, 
				Asc<APTran.lineNbr>>>>>
			Transactions;

		public PXSelectJoin<
			APTran, 
				LeftJoin<POLine, 
					On<POLine.orderType, Equal<APTran.pOOrderType>,
										And<POLine.orderNbr, Equal<APTran.pONbr>,
										And<POLine.lineNbr, Equal<APTran.pOLineNbr>>>>>,
			Where<
				APTran.tranType, Equal<Current<APInvoice.docType>>,
				And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>>>,
			OrderBy<
				Asc<APTran.tranType, 
				Asc<APTran.refNbr, 
				Asc<APTran.lineNbr>>>>>
		TransactionsPOLine;

		public PXSelect<
			APTran, 
			Where<
				APTran.tranType, Equal<Current<APInvoice.docType>>, 
				And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>, 
				And<APTran.lineType, Equal<SOLineType.discount>>>>, 
			OrderBy<
				Asc<APTran.tranType, 
				Asc<APTran.refNbr, 
				Asc<APTran.lineNbr>>>>> 
			Discount_Row;

		[PXCopyPasteHiddenView]
		public PXSelect<
			APTax, 
			Where<
				APTax.tranType, Equal<Current<APInvoice.docType>>, 
				And<APTax.refNbr, Equal<Current<APInvoice.refNbr>>>>, 
			OrderBy<
				Asc<APTax.tranType, 
				Asc<APTax.refNbr, Asc<APTax.taxID>>>>> 
			Tax_Rows;

		[PXCopyPasteHiddenView]
		public TaxTranSelect<APInvoice, APInvoice.termsID, APTaxTran, APTaxTran.taxID,
			Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
				And<APTaxTran.tranType, Equal<Current<APInvoice.docType>>,
				And<APTaxTran.refNbr, Equal<Current<APInvoice.refNbr>>>>>> Taxes;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<
			APTaxTran, 
				InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, 
			Where<
				APTaxTran.module, Equal<BatchModule.moduleAP>, 
				And<APTaxTran.tranType, Equal<Current<APInvoice.docType>>, 
				And<APTaxTran.refNbr, Equal<Current<APInvoice.refNbr>>, 
				And<Tax.taxType, Equal<CSTaxType.use>>>>>> 
			UseTaxes;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<
			APAdjust, 
				InnerJoin<APPayment, 
					On<APPayment.docType, Equal<APAdjust.adjgDocType>,
					And<APPayment.refNbr, Equal<APAdjust.adjgRefNbr>>>>> 
			Adjustments;

		public PXSelectJoin<
			APAdjust, 
				InnerJoinSingleTable<APPayment, 
					On<APPayment.docType, Equal<APAdjust.adjgDocType>, 
				And<APPayment.refNbr, Equal<APAdjust.adjgRefNbr>>>, 
				InnerJoin<Standalone.APRegisterAlias,
					On<Standalone.APRegisterAlias.docType, Equal<APAdjust.adjgDocType>,
					And<Standalone.APRegisterAlias.refNbr, Equal<APAdjust.adjgRefNbr>>>,
				InnerJoin<CurrencyInfo, 
					On<CurrencyInfo.curyInfoID, Equal<Standalone.APRegisterAlias.curyInfoID>>>>>,
			Where<
				APAdjust.adjdDocType, Equal<Current<APInvoice.docType>>, 
				And<APAdjust.adjdRefNbr, Equal<Current<APInvoice.refNbr>>,
				And<Where<
					Current<APInvoice.released>, Equal<True>, 
					Or<Current<APInvoice.prebooked>, Equal<True>, 
					Or<APAdjust.released, Equal<Current<APInvoice.released>>>>>>>>> 
			Adjustments_Raw;

		[PXCopyPasteHiddenView]
		public PXSelect<
			APInvoiceDiscountDetail, 
			Where<
				APInvoiceDiscountDetail.docType, Equal<Current<APInvoice.docType>>, 
				And<APInvoiceDiscountDetail.refNbr, Equal<Current<APInvoice.refNbr>>>>, 
			OrderBy<
				Asc<APInvoiceDiscountDetail.docType, 
				Asc<APInvoiceDiscountDetail.refNbr>>>> 
			DiscountDetails;

		public PXSelect<
			CurrencyInfo, 
			Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>> 
			currencyinfo;

		public PXSetup<APSetup> APSetup;
		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<APInvoice.vendorID>>>> vendor;
		public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Optional<APInvoice.vendorID>>>> employee;
		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;
		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<APInvoice.taxZoneID>>>> taxzone;

		public PXSetup<
			Location, 
			Where<
				Location.bAccountID, Equal<Current<APInvoice.vendorID>>, 
				And<Location.locationID, Equal<Optional<APInvoice.vendorLocationID>>>>> 
			location;

		public PXSetup<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Current<APInvoice.finPeriodID>>>> finperiod;

		[PXCopyPasteHiddenView]
		public PXSelect<POOrderRS> poorderslist;

		[PXCopyPasteHiddenView]
		public PXSelect<POReceipt> poreceiptslist;

		public PXSelect<POReceiptLineR> receiptLinesUPD;

		public PXSelect<POLineAP> orderLinesUPD;
		
		public PXFilter<POReceiptFilter> filter;

		[PXCopyPasteHiddenView()]
		public PXSelect<
			LandedCostTranR,
			Where<
				LandedCostTranR.vendorID,Equal<Current<APInvoice.vendorID>>,
			And<LandedCostTran.vendorLocationID,Equal<Current<APInvoice.vendorLocationID>>,
				And<LandedCostTran.curyID,Equal<Current<APInvoice.curyID>>>>>> 
			landedCostTranSelection;
		
		[PXCopyPasteHiddenView()]
		public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;

		public PXSelect<
			LandedCostTranSplit, 
			Where<LandedCostTranSplit.lCTranID, Equal<Optional<LandedCostTran.lCTranID>>>> 
			LCTranSplit;

		public PXSelect<AP1099Hist> ap1099hist;
		public PXSelect<AP1099Yr> ap1099year;

		public PXSetup<GLSetup> glsetup;
		public PXSetupOptional<TXAvalaraSetup> avalaraSetup;
		public PXSetupOptional<INSetup> insetup;
		public PXSetupOptional<CommonSetup> commonsetup;
		public PXSetup<POSetup> posetup;

		public PXSelect<DRSchedule> dummySchedule_forPXParent;
		public PXSelect<DRScheduleDetail> dummyScheduleDetail_forPXParent;
		public PXSelect<DRScheduleTran> dummyScheduleTran_forPXParent;

		public PXSelect<
			EPExpenseClaim, 
			Where<EPExpenseClaim.aPRefNbr, Equal<Current<APInvoice.refNbr>>>> 
			expenseclaim;

		public PXFilter<DuplicateFilter> duplicatefilter;
		public PXSelect<GLVoucher, Where<True, Equal<False>>> Voucher;

		public PXSelect<
			APTran, 
			Where<
				APTran.refNbr, Equal<Optional<APTran.refNbr>>, 
				And<APTran.tranType, Equal<Optional<APTran.tranType>>>>> 
			siblingTrans;

		public virtual IEnumerable transactions()
		{
			foreach (PXResult<APTran, POReceiptLine> tran in PXSelectJoin<APTran, LeftJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<APTran.receiptNbr>,
										And<POReceiptLine.lineNbr, Equal<APTran.receiptLineNbr>>>>,
				Where<APTran.tranType, Equal<Current<APInvoice.docType>>,
				And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>>>,
			OrderBy<Asc<APTran.tranType, Asc<APTran.refNbr, Asc<APTran.lineNbr>>>>>.Select(this))
			{
				if (((APTran)tran).LineType != SOLineType.Discount)
					yield return tran;
			}
		}

		public PXFilter<LinkLineFilter> linkLineFilter;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<
			POReceiptLineS, 
				LeftJoin<POReceipt, 
					On<POReceiptLineS.receiptNbr, Equal<POReceipt.receiptNbr>, 
					And<POReceiptLineS.receiptType, Equal<POReceipt.receiptType>>>>> 
			linkLineReceiptTran;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<
			POReceiptLineS, 
				LeftJoin<POReceipt, 
					On<POReceiptLineS.receiptNbr, Equal<POReceipt.receiptNbr>, 
					And<POReceiptLineS.receiptType, Equal<POReceipt.receiptType>>>>, 
			Where<
				POReceiptLineS.receiptNbr, Equal<Required<LinkLineReceipt.receiptNbr>>, 
				And<POReceiptLineS.receiptType, Equal<Required<LinkLineReceipt.receiptType>>, 
				And<POReceiptLineS.lineNbr, Equal<Required<LinkLineReceipt.receiptLineNbr>>>>>> 
			ReceipLineLinked;

		[PXCopyPasteHiddenView]
		public PXSelect<POReceiptLineAdd> poReceiptLinesSelection;
      [PXCopyPasteHiddenView]
      public PXSelectJoin<POReceiptLineAdd, 
          LeftJoin<POReceipt, 
          On<POReceiptLineAdd.receiptNbr, Equal<POReceipt.receiptNbr>, 
          And<POReceiptLineAdd.receiptType, Equal<POReceipt.receiptType>>>>, 
          Where<
          POReceiptLineAdd.receiptNbr, Equal<Required<LinkLineReceipt.receiptNbr>>, 
          And<POReceiptLineAdd.receiptType, Equal<Required<LinkLineReceipt.receiptType>>, 
          And<POReceiptLineAdd.lineNbr, Equal<Required<LinkLineReceipt.receiptLineNbr>>>>>> 
          ReceipLineAdd;

		public PXSelect<APSetupApproval> SetupApproval;
		public EPApprovalAutomation<APInvoice, APInvoice.approved, APInvoice.rejected, APInvoice.hold, APSetupApproval> Approval;

		public virtual IEnumerable LinkLineReceiptTran()
		{
			APTran currentAPTran = Transactions.Current;
			if (currentAPTran == null)
				yield break;

			HashSet<APTran> usedRecceiptLine = new HashSet<APTran>(new POReceiptLineComparer());
			HashSet<APTran> unusedReceiptLine = new HashSet<APTran>(new POReceiptLineComparer());

			foreach (APTran aPTran in Transactions.Cache.Inserted)
			{
				if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null && currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM)
					usedRecceiptLine.Add(aPTran);
			}

			foreach (APTran aPTran in Transactions.Cache.Deleted)
			{
				if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null && currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM
					&& Transactions.Cache.GetStatus(aPTran) != PXEntryStatus.InsertedDeleted)
					if (!usedRecceiptLine.Remove(aPTran))
						unusedReceiptLine.Add(aPTran);
			}

			foreach (APTran aPTran in Transactions.Cache.Updated)
			{
				if (currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM)
				{
					APTran originAPTran = new APTran();
					originAPTran.ReceiptNbr = (String)Transactions.Cache.GetValueOriginal<APTran.receiptNbr>(aPTran);
					originAPTran.ReceiptType = (String)Transactions.Cache.GetValueOriginal<APTran.receiptType>(aPTran);
					originAPTran.ReceiptLineNbr = (Int32?)Transactions.Cache.GetValueOriginal<APTran.receiptLineNbr>(aPTran);

					if (originAPTran.ReceiptNbr != null && originAPTran.ReceiptType != null && originAPTran.ReceiptLineNbr != null)
					{
						if (!usedRecceiptLine.Remove(originAPTran))
							unusedReceiptLine.Add(originAPTran);
					}

					if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null)
					{
						if (!unusedReceiptLine.Remove(aPTran))
							usedRecceiptLine.Add(aPTran);
					}
				}
			}

			if (currentAPTran.ReceiptNbr != null && currentAPTran.ReceiptType != null && currentAPTran.ReceiptLineNbr != null)
				unusedReceiptLine.Add(currentAPTran);

			foreach (LinkLineReceipt item in PXSelect<LinkLineReceipt
													, Where2<Where<Current<LinkLineFilter.pOOrderNbr>, Equal<LinkLineReceipt.orderNbr>, Or<Current<LinkLineFilter.pOOrderNbr>, IsNull>>
														, And2<Where<Current<LinkLineFilter.siteID>, IsNull, Or<LinkLineReceipt.receiptSiteID, Equal<Current<LinkLineFilter.siteID>>>>
															, And<LinkLineReceipt.inventoryID, Equal<Current<APTran.inventoryID>>
																, And<LinkLineReceipt.uOM, Equal<Current<APTran.uOM>>
																	, And<LinkLineReceipt.vendorID, Equal<Current<APInvoice.vendorID>>
																		, And<LinkLineReceipt.vendorLocationID, Equal<Current<APInvoice.vendorLocationID>>
																			, And<LinkLineReceipt.receiptCuryID, Equal<Current<APInvoice.curyID>>,
                                                                            And<LinkLineReceipt.receiptType, Equal<POReceiptType.poreceipt>>>
																			>
																		>
																	>
																>
															>
														>
													>.Select(this))
			{
				APTran aPTran = new APTran();
				aPTran.ReceiptType = item.ReceiptType;
				aPTran.ReceiptNbr = item.ReceiptNbr;
				aPTran.ReceiptLineNbr = item.ReceiptLineNbr;
				if (!usedRecceiptLine.Contains(aPTran))
				{
					PXResult<POReceiptLineS, POReceipt> res = (PXResult<POReceiptLineS, POReceipt>)ReceipLineLinked.Select(item.ReceiptNbr, item.ReceiptType, item.ReceiptLineNbr);
					if (linkLineReceiptTran.Cache.GetStatus((POReceiptLineS)res) != PXEntryStatus.Updated && ((POReceiptLineS)res).CompareReferenceKey(currentAPTran))
					{
						linkLineReceiptTran.Cache.SetValue<POReceiptLineS.selected>((POReceiptLineS)res, true);
						linkLineReceiptTran.Cache.SetStatus((POReceiptLineS)res, PXEntryStatus.Updated);
					}
					yield return res;
				}
			}

			foreach (APTran item in unusedReceiptLine)
			{
				PXResult<POReceiptLineS, POReceipt> res = (PXResult<POReceiptLineS, POReceipt>)ReceipLineLinked.Select(item.ReceiptNbr, item.ReceiptType, item.ReceiptLineNbr);
				if (linkLineReceiptTran.Cache.GetStatus((POReceiptLineS) res) != PXEntryStatus.Updated && ((POReceiptLineS) res).CompareReferenceKey(currentAPTran))
				{
					linkLineReceiptTran.Cache.SetValue<POReceiptLineS.selected>((POReceiptLineS)res, true);
					linkLineReceiptTran.Cache.SetStatus((POReceiptLineS)res, PXEntryStatus.Updated);
				}
				yield return res;
			}

		}

		[PXCopyPasteHiddenView]
		public PXSelectJoin<POLineS, LeftJoin<POOrder, On<POLineS.orderNbr, Equal<POOrder.orderNbr>, And<POLineS.orderType, Equal<POOrder.orderType>>>>> linkLineOrderTran;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<POLineS
						, LeftJoin<POOrder, On<POLineS.orderNbr, Equal<POOrder.orderNbr>, And<POLineS.orderType, Equal<POOrder.orderType>>>>
						, Where<POLineS.orderNbr, Equal<Required<LinkLineOrder.orderNbr>>, And<POLineS.orderType, Equal<Required<LinkLineOrder.orderType>>, And<POLineS.lineNbr, Equal<Required<LinkLineOrder.orderLineNbr>>>>>> POLineLink;


		public virtual IEnumerable LinkLineOrderTran()
		{
			APTran currentAPTran = Transactions.Current;
			if(currentAPTran == null)
				yield break;

			HashSet<APTran> usedPOLine = new HashSet<APTran>(new POLineComparer());
			HashSet<APTran> unusedPOLine = new HashSet<APTran>(new POLineComparer());

			foreach (APTran aPTran in Transactions.Cache.Inserted)
			{
				if (aPTran.PONbr != null && aPTran.POOrderType != null && aPTran.POLineNbr != null && currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM)
					usedPOLine.Add(aPTran);
			}

			foreach (APTran aPTran in Transactions.Cache.Deleted)
			{
				if (aPTran.PONbr != null && aPTran.POOrderType != null && aPTran.POLineNbr != null && currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM
					&& Transactions.Cache.GetStatus(aPTran) != PXEntryStatus.InsertedDeleted)
					if (!usedPOLine.Remove(aPTran))
						unusedPOLine.Add(aPTran);

			}

			foreach (APTran aPTran in Transactions.Cache.Updated)
			{
				if (currentAPTran.InventoryID == aPTran.InventoryID && currentAPTran.UOM == aPTran.UOM)
				{
					APTran originAPTran = new APTran();
					originAPTran.PONbr = (String)Transactions.Cache.GetValueOriginal<APTran.pONbr>(aPTran);
					originAPTran.POOrderType = (String)Transactions.Cache.GetValueOriginal<APTran.pOOrderType>(aPTran);
					originAPTran.POLineNbr = (Int32?)Transactions.Cache.GetValueOriginal<APTran.pOLineNbr>(aPTran);

					if (originAPTran.PONbr != null && originAPTran.POOrderType != null && originAPTran.POLineNbr != null)
					{
						if (!usedPOLine.Remove(originAPTran))
							unusedPOLine.Add(originAPTran);
					}

					if (aPTran.PONbr != null && aPTran.POOrderType != null && aPTran.POLineNbr != null)
					{
						if (!unusedPOLine.Remove(aPTran))
							usedPOLine.Add(aPTran);
					}
				}
			}

			if (currentAPTran.PONbr != null && currentAPTran.POOrderType != null && currentAPTran.POLineNbr != null)
				unusedPOLine.Add(currentAPTran);


			foreach (LinkLineOrder item in PXSelect<LinkLineOrder
													, Where2<Where<Current<LinkLineFilter.pOOrderNbr>, Equal<LinkLineOrder.orderNbr>, Or<Current<LinkLineFilter.pOOrderNbr>, IsNull>>
														, And2<Where<Current<LinkLineFilter.siteID>, IsNull, Or<LinkLineOrder.orderSiteID, Equal<Current<LinkLineFilter.siteID>>>>
															, And<LinkLineOrder.inventoryID, Equal<Current<APTran.inventoryID>>
																, And<LinkLineOrder.uOM, Equal<Current<APTran.uOM>>
																	, And<LinkLineOrder.vendorID, Equal<Current<APInvoice.vendorID>>
																		, And<LinkLineOrder.vendorLocationID, Equal<Current<APInvoice.vendorLocationID>>
																			, And<LinkLineOrder.orderCuryID, Equal<Current<APInvoice.curyID>>>
																			>
																		>
																	>
																>
															>
														>
													>.Select(this))
			{
				APTran aPTran = new APTran();
				aPTran.POOrderType = item.OrderType;
				aPTran.PONbr = item.OrderNbr;
				aPTran.POLineNbr = item.OrderLineNbr;
				if (!usedPOLine.Contains(aPTran))
				{
					PXResult<POLineS, POOrder> res = (PXResult<POLineS, POOrder>)POLineLink.Select(item.OrderNbr, item.OrderType, item.OrderLineNbr);
					if (linkLineOrderTran.Cache.GetStatus((POLineS) res) != PXEntryStatus.Updated &&
						((POLineS) res).CompareReferenceKey(currentAPTran))
					{
						linkLineOrderTran.Cache.SetValue<POLineS.selected>((POLineS)res, true);
						linkLineOrderTran.Cache.SetStatus((POLineS)res, PXEntryStatus.Updated);
					}
					yield return res;
				}
			}

			foreach (APTran item in unusedPOLine)
			{
				PXResult<POLineS, POOrder> res = (PXResult<POLineS, POOrder>) POLineLink.Select(item.PONbr, item.POOrderType, item.POLineNbr);
				if (linkLineOrderTran.Cache.GetStatus((POLineS) res) != PXEntryStatus.Updated &&
					((POLineS) res).CompareReferenceKey(currentAPTran))
				{
					linkLineOrderTran.Cache.SetValue<POLineS.selected>((POLineS)res, true);
					linkLineOrderTran.Cache.SetStatus((POLineS)res, PXEntryStatus.Updated);
				}
				yield return res;
			}

		}

		#endregion

		#region Function
		public virtual void VoidPrepayment(APRegister doc)
		{
			APInvoice invoice = PXCache<APInvoice>.CreateCopy((APInvoice)doc);
			invoice.OpenDoc = false;
			invoice.Voided = true;
			Document.Update(invoice);
			Save.Press();
		}
		public virtual void ReverseInvoiceProc(APRegister doc)
		{
			AR.DuplicateFilter dfilter = PXCache<AR.DuplicateFilter>.CreateCopy(duplicatefilter.Current);
			WebDialogResult dialogRes = duplicatefilter.View.Answer;

			this.Clear(PXClearOption.PreserveTimeStamp);

			//Magic. We need to prevent rewriting of CurrencyInfo.IsReadOnly by true in CurrencyInfoView
			CurrentDocument.Cache.AllowUpdate = true;

			foreach (PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res in APInvoice_CurrencyInfo_Terms_Vendor.Select(this, (object)doc.DocType, doc.RefNbr, doc.VendorID))
			{
				CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
				info.CuryInfoID = null;
				info.IsReadOnly = false;
				info = PXCache<CurrencyInfo>.CreateCopy(this.currencyinfo.Insert(info));

				APInvoice invoice = PXCache<APInvoice>.CreateCopy((APInvoice)res);
				invoice.CuryInfoID = info.CuryInfoID;
				invoice.DocType = APDocType.DebitAdj;
				invoice.RefNbr = null;

				//must set for _RowSelected
				invoice.OpenDoc = true;
				invoice.Released = false;
				Document.Cache.SetDefaultExt<APInvoice.hold>(invoice);
				invoice.BatchNbr = null;
				invoice.PrebookBatchNbr = null;
				invoice.Prebooked = false;
				invoice.ScheduleID = null;
				invoice.Scheduled = false;
				invoice.NoteID = null;

				invoice.TermsID = null;
				invoice.InstallmentCntr = null;
				invoice.InstallmentNbr = null;
				invoice.DueDate = null;
				invoice.DiscDate = null;
				invoice.CuryOrigDiscAmt = 0m;
				invoice.FinPeriodID = doc.FinPeriodID;
				invoice.OrigDocType = doc.DocType;
				invoice.OrigRefNbr = doc.RefNbr;
				invoice.OrigDocDate = invoice.DocDate;

				invoice.PaySel = false;
				//PaySel does not affect these fields
				//invoice.PayTypeID = null;
				//invoice.PayDate = null;
				//invoice.PayAccountID = null;
				invoice.CuryDocBal = invoice.CuryOrigDocAmt;
				invoice.CuryLineTotal = 0m;
				invoice.IsTaxPosted = false;
				invoice.IsTaxValid = false;
				invoice.CuryVatTaxableTotal = 0m;
				invoice.CuryVatExemptTotal = 0m;

				invoice = this.Document.Insert(invoice);

				if (invoice.RefNbr == null)
				{
					//manual numbering, check for occasional duplicate
					APInvoice duplicate = PXSelect<APInvoice>.Search<APInvoice.docType, APInvoice.refNbr>(this, invoice.DocType, invoice.OrigRefNbr);
					if (duplicate != null)
					{
						PXCache<AR.DuplicateFilter>.RestoreCopy(duplicatefilter.Current, dfilter);
						duplicatefilter.View.Answer = dialogRes;
						if (duplicatefilter.AskExt() == WebDialogResult.OK)
						{
							duplicatefilter.Cache.Clear();
							if (duplicatefilter.Current.RefNbr == null)
								throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(AR.DuplicateFilter.refNbr).Name);
							duplicate = PXSelect<APInvoice>.Search<APInvoice.docType, APInvoice.refNbr>(this, invoice.DocType, duplicatefilter.Current.RefNbr);
							if (duplicate != null)
						throw new PXException(ErrorMessages.RecordExists);
							invoice.RefNbr = duplicatefilter.Current.RefNbr;
						}
					}
					else
					invoice.RefNbr = invoice.OrigRefNbr;
					this.Document.Cache.Normalize();
					invoice = this.Document.Update(invoice);
				}

				if (info != null)
				{
					CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(this, null);
					b_info.CuryID = info.CuryID;
					b_info.CuryEffDate = info.CuryEffDate;
					b_info.CuryRateTypeID = info.CuryRateTypeID;
					b_info.CuryRate = info.CuryRate;
					b_info.RecipRate = info.RecipRate;
					b_info.CuryMultDiv = info.CuryMultDiv;
					this.currencyinfo.Update(b_info);
				}
			}

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(this.Transactions.Cache, null, TaxCalc.ManualCalc);

			foreach (APTran srcTran in PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				APTran tran = PXCache<APTran>.CreateCopy(srcTran);
				tran.TranType = null;
				tran.RefNbr = null;
				string origDrCr = tran.DrCr;
				tran.DrCr = null;
				tran.Released = null;
				tran.CuryInfoID = null;
				tran.ManualDisc = true;

				tran.PPVDocType = null;
				tran.PPVRefNbr = null;
				tran.NoteID = null;

				if (!string.IsNullOrEmpty(tran.DeferredCode))
				{
					DRSchedule schedule = PXSelect<DRSchedule,
						Where<DRSchedule.module, Equal<moduleAP>,
						And<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
						And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>,
						And<DRSchedule.lineNbr, Equal<Required<DRSchedule.lineNbr>>>>>>>.Select(this, doc.DocType, doc.RefNbr, tran.LineNbr);

					if (schedule != null)
					{
						tran.DefScheduleID = schedule.ScheduleID;
					}
				}

				Decimal? curyTranAmt = tran.CuryTranAmt;
				APTran tranNew = this.Transactions.Insert(tran);
				PXNoteAttribute.CopyNoteAndFiles(Transactions.Cache, srcTran, Transactions.Cache, tranNew);


				if (tranNew != null && tranNew.CuryTranAmt != curyTranAmt)
				{
					tranNew.CuryTranAmt = curyTranAmt;
					this.Transactions.Cache.Update(tranNew);
				}

				if (tranNew.LineType == "DS")
				{
					tranNew.DrCr = (origDrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
					tranNew.FreezeManualDisc = true;
					tranNew.TaxCategoryID = null;
					this.Transactions.Update(tranNew);
				}

			}

			foreach (APInvoiceDiscountDetail discountDetail in PXSelect<APInvoiceDiscountDetail, Where<APInvoiceDiscountDetail.docType, Equal<Required<APInvoice.docType>>, And<APInvoiceDiscountDetail.refNbr, Equal<Required<APInvoice.refNbr>>>>, OrderBy<Asc<APInvoiceDiscountDetail.docType, Asc<APInvoiceDiscountDetail.refNbr>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				APInvoiceDiscountDetail newDiscountDetail = PXCache<APInvoiceDiscountDetail>.CreateCopy(discountDetail);

				newDiscountDetail.DocType = Document.Current.DocType;
				newDiscountDetail.RefNbr = Document.Current.RefNbr;
				newDiscountDetail.IsManual = true;
				this.DiscountDetails.Update(newDiscountDetail);
			}    

			if (!IsExternalTax)
			{
				foreach (APTaxTran tax in PXSelect<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
				{
					APTaxTran new_aptax = new APTaxTran();
					new_aptax.TaxID = tax.TaxID;

					new_aptax = this.Taxes.Insert(new_aptax);

					if (new_aptax != null)
					{
						new_aptax = PXCache<APTaxTran>.CreateCopy(new_aptax);
						new_aptax.TaxRate = tax.TaxRate;
						new_aptax.CuryTaxableAmt = tax.CuryTaxableAmt;
						new_aptax.CuryTaxAmt = tax.CuryTaxAmt;
						new_aptax = this.Taxes.Update(new_aptax);
					}
				}
			}
		}

		private void PopulateBoxList()
		{
			List<int> AllowedValues = new List<int>();
			List<string> AllowedLabels = new List<string>();

			foreach (AP1099Box box in PXSelectReadonly<AP1099Box>.Select(this, null))
			{
				AllowedValues.Add((int)box.BoxNbr);
				StringBuilder bld = new StringBuilder(box.BoxNbr.ToString());
				bld.Append("-");
				bld.Append(box.Descr);
				AllowedLabels.Add(bld.ToString());
			}

			if (AllowedValues.Count > 0)
			{
				PXIntListAttribute.SetList<APTran.box1099>(Transactions.Cache, null, AllowedValues.ToArray(), AllowedLabels.ToArray());
			}
		}

		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<APInvoice.docDate, APInvoice.finPeriodID, APInvoice.curyID>(e.Row, e.OldRow))
			{
				foreach (APTran tran in Transactions.Select())
				{
					if (Transactions.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Transactions.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}

				foreach (APAdjust tran in Adjustments.Select())
				{
					if (Adjustments.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Adjustments.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}
			}

			if (!sender.ObjectsEqual<APInvoice.docDate, APInvoice.curyID>(e.Row, e.OldRow))
			{
				foreach (LandedCostTran lcTran in this.landedCostTrans.Select())
				{
					if (this.landedCostTrans.Cache.GetStatus(lcTran) == PXEntryStatus.Notchanged)
					{
						this.landedCostTrans.Cache.SetStatus(lcTran, PXEntryStatus.Updated);
					}
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

		
		public APInvoiceEntry()
		{
			APSetup setup = APSetup.Current;

			PopulateBoxList();

			RowUpdated.AddHandler<APInvoice>(ParentFieldUpdated);

			this.poorderslist.Cache.AllowDelete = false;
			this.poorderslist.Cache.AllowInsert = false;
			this.poreceiptslist.Cache.AllowInsert = false;
			this.poreceiptslist.Cache.AllowDelete = false;
			this.poReceiptLinesSelection.Cache.AllowDelete = false;
			this.poReceiptLinesSelection.Cache.AllowInsert = false;
			this.landedCostTranSelection.Cache.AllowDelete = false;
			this.landedCostTranSelection.Cache.AllowInsert = false;
			this.linkLineReceiptTran.Cache.AllowDelete = false;
			this.linkLineReceiptTran.Cache.AllowInsert = false;
			this.linkLineOrderTran.Cache.AllowDelete = false;
			this.linkLineOrderTran.Cache.AllowInsert = false;

			PXUIFieldAttribute.SetEnabled(this.landedCostTranSelection.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTranR.selected>(this.landedCostTranSelection.Cache, null, true);

			PXUIFieldAttribute.SetEnabled(linkLineReceiptTran.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<POReceiptLineS.selected>(linkLineReceiptTran.Cache, null, true);
			PXUIFieldAttribute.SetEnabled(linkLineOrderTran.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<LinkLineOrder.selected>(linkLineOrderTran.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<POReceiptLine.subItemID>(Caches[typeof(POReceiptLine)], null, false);

			PXUIFieldAttribute.SetVisible<APTran.projectID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));
			PXUIFieldAttribute.SetVisible<APTran.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));
			PXUIFieldAttribute.SetVisible<APTran.nonBillable>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(Transactions.Cache, null, TaxCalc.ManualLineCalc);

			FieldDefaulting.AddHandler<InventoryItem.stkItem>((sender, e) => { if (e.Row != null) e.NewValue = false; });
			Views.Caches.Remove(typeof(POOrderRS));//This prevents POOrderRS from persisting and throwing error "'IsOpenTaxValid' may not be empty"

			Actions.Move(nameof(release), nameof(prebook));
			PXAction action = Actions["action"];
			if (action != null)
			{
				action.AddMenuAction(voidInvoice);
			}

			APOpenPeriodAttribute.SetValidatePeriod<APInvoice.finPeriodID>(Document.Cache, null, PeriodValidation.DefaultSelectUpdate);
		}

		public override void Persist()
		{
			if (Document.Current != null && !Discount_Row.Any() && Document.Current.CuryDiscTot != 0m)
			{
				AddDiscount(Document.Cache, Document.Current);
				DiscountEngine<APTran>.CalculateDocumentDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m);
			}
			
			foreach (APAdjust adj in Adjustments.Cache.Inserted)
			{
				if (adj.CuryAdjdAmt == 0m || Document.Current?.Rejected == true)
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.InsertedDeleted);
				}
			}

			foreach (APAdjust adj in Adjustments.Cache.Updated)
			{
				if (adj.CuryAdjdAmt == 0m || Document.Current?.Rejected == true)
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.Deleted);
				}
			}

			Adjustments.Cache.ClearQueryCache();

			foreach (APInvoice apdoc in Document.Cache.Cached)
			{
				if ((Document.Cache.GetStatus(apdoc) == PXEntryStatus.Inserted || Document.Cache.GetStatus(apdoc) == PXEntryStatus.Updated) 
					&& apdoc.DocType == APDocType.Invoice 
					&& apdoc.Released != true && apdoc.Prebooked != true)
				{
					decimal? CuryApplAmt = 0m;

					foreach (APAdjust adj in Adjustments_Raw.View
						.SelectMultiBound(new object[] { apdoc })
						.RowCast<APAdjust>()
						.Where(adj => adj != null))
					{
						CuryApplAmt += adj.CuryAdjdAmt;

						if (apdoc.CuryDocBal - CuryApplAmt < 0m)
						{
							if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Notchanged)
							{
								Adjustments.Cache.SetStatus(adj, PXEntryStatus.Updated);
							}
							Adjustments.Cache.RaiseExceptionHandling<APAdjust.curyAdjdAmt>(adj, adj.CuryAdjdAmt, new PXSetPropertyException(Messages.Application_Amount_Cannot_Exceed_Document_Amount));
							throw new PXException(Messages.Application_Amount_Cannot_Exceed_Document_Amount);
						}
					}
				}
			}

			if (!PorateDiscounts())
			{
				DiscountEngine<APTran>.ValidateDiscountDetails(DiscountDetails);
			}

			using (PXTransactionScope transactionScope = new PXTransactionScope())
			{
				foreach (APInvoice inv in Document.Cache.Deleted)
				{
					if (inv.DocType == APDocType.Prepayment)
					{
						PXDatabase.Update<POOrder>(
							new PXDataFieldAssign<POOrder.prepaymentDocType>(null),
							new PXDataFieldAssign<POOrder.prepaymentRefNbr>(null),
							new PXDataFieldRestrict<POOrder.prepaymentDocType>(PXDbType.VarChar, 3, inv.DocType, PXComp.EQ),
							new PXDataFieldRestrict<POOrder.prepaymentRefNbr>(PXDbType.NVarChar, 15, inv.RefNbr, PXComp.EQ));
					}
				}

				foreach (APInvoice inv in Document.Cache.Updated)
				{
					if (inv.DocType == APDocType.Prepayment && inv.Voided == true)
					{
						PXDatabase.Update<POOrder>(
							new PXDataFieldAssign<POOrder.prepaymentDocType>(null),
							new PXDataFieldAssign<POOrder.prepaymentRefNbr>(null),
							new PXDataFieldRestrict<POOrder.prepaymentDocType>(PXDbType.VarChar, 3, inv.DocType, PXComp.EQ),
							new PXDataFieldRestrict<POOrder.prepaymentRefNbr>(PXDbType.NVarChar, 15, inv.RefNbr, PXComp.EQ));
					}
				}

				base.Persist();

				if (Document.Current != null &&
					Document.Current.DocType == APDocType.Prepayment &&
					Document.Current.PONumber != null &&
					Document.Current.Voided == false)
				{
					PXDatabase.Update<POOrder>(
						new PXDataFieldAssign<POOrder.prepaymentDocType>(Document.Current.DocType),
						new PXDataFieldAssign<POOrder.prepaymentRefNbr>(Document.Current.RefNbr),
						new PXDataFieldRestrict<POOrder.orderType>(PXDbType.VarChar, 2, POOrderType.RegularOrder, PXComp.EQ),
						new PXDataFieldRestrict<POOrder.orderNbr>(PXDbType.NVarChar, 15, Document.Current.PONumber, PXComp.EQ));
				}

				transactionScope.Complete(this);
			}

			if (Document.Current != null && 
				IsExternalTax && 
				Document.Current.InstallmentNbr == null && 
				Document.Current.IsTaxValid != true && 
				!skipAvalaraCallOnSave)
			{
				PXLongOperation.StartOperation(this, () =>
				{
					APInvoice doc = new APInvoice();
					doc.DocType = Document.Current.DocType;
					doc.RefNbr = Document.Current.RefNbr;
					doc.OrigModule = Document.Current.OrigModule;
					APExternalTaxCalc.Process(doc);
				});
			}
		}

		public virtual IEnumerable adjustments()
		{
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(this);

			foreach (PXResult<APAdjust, APPayment, Standalone.APRegisterAlias, CurrencyInfo> res in Adjustments_Raw.Select())
			{
				APPayment payment = res;
				APAdjust adj = res;
				CurrencyInfo pay_info = res;

                PXCache<APRegister>.RestoreCopy(payment, (Standalone.APRegisterAlias)res);
                decimal CuryDocBal = BalanceCalculation.CalculateApplicationDocumentBalance(
					Adjustments.Cache, 
					pay_info, 
					inv_info, 
					payment.DocBal, 
					payment.CuryDocBal);

				if (adj != null)
				{
					if (adj.Released == false)
					{
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
					}
					else
					{
						adj.CuryDocBal = CuryDocBal;
					}
					adj.AdjType = ARAdjust.adjType.Adjusting;
					this.Caches<APAdjust>().RaiseFieldUpdated<APAdjust.adjType>(adj, null);
				}

				yield return new PXResult<APAdjust, APPayment, CurrencyInfo>(adj, payment, pay_info);
			}

			if (Document.Current?.Released == true)
			{
				foreach (PXResult<APAdjust, Standalone.APRegisterAlias, APInvoice, CurrencyInfo> res in 
					PXSelectJoin<
						APAdjust,
							InnerJoin<Standalone.APRegisterAlias,
								On<Standalone.APRegisterAlias.docType, Equal<APAdjust.adjdDocType>,
								And<Standalone.APRegisterAlias.refNbr, Equal<APAdjust.adjdRefNbr>>>,
							InnerJoinSingleTable<APInvoice, 
								On<APInvoice.docType, Equal<APAdjust.adjdDocType>,
								And<APInvoice.refNbr, Equal<APAdjust.adjdRefNbr>>>,
							InnerJoin<CurrencyInfo, 
								On<CurrencyInfo.curyInfoID, Equal<Standalone.APRegisterAlias.curyInfoID>>>>>,
						Where<
							APAdjust.adjgDocType, Equal<Current<APInvoice.docType>>,
							And<APAdjust.adjgRefNbr, Equal<Current<APInvoice.refNbr>>>>>
					.Select(this))
				{
					APAdjust adj = res;
					APInvoice invoice = res;
					CurrencyInfo adjd_info = res;
					
					PXCache<APRegister>.RestoreCopy(invoice, (Standalone.APRegisterAlias)res);

					adj.AdjType = ARAdjust.adjType.Adjusted;
					this.Caches<APAdjust>().RaiseFieldUpdated<APAdjust.adjType>(adj, null);

					adj.CuryDocBal = BalanceCalculation.CalculateApplicationDocumentBalance(
						Adjustments.Cache, 
						adjd_info, 
						inv_info, 
						invoice.DocBal, 
						invoice.CuryDocBal);

                    yield return new PXResult<APAdjust, APInvoice, CurrencyInfo>(adj, invoice, adjd_info);
				}
			}

			if (Document.Current != null 
				&& (Document.Current.DocType == APDocType.Invoice 
					|| Document.Current.DocType == APDocType.CreditAdj) 
				&& Document.Current.Released != true 
				&& Document.Current.Prebooked != true
				&& Document.Current.Rejected != true
				&& Document.Current.Scheduled != true
				&& Document.Current.Voided != true
				&& !IsImport)
			{
				using (new ReadOnlyScope(Adjustments.Cache, Document.Cache))
				// The display fields are filled manually because
				// the selector + formulas severely affect performance 
				// in presence of many payment documents.
				// -
				using (var firstPerformanceScope = new DisableSelectorValidationScope(Adjustments.Cache))
				using (var secondPerformanceScope = new DisableFormulaCalculationScope(
					Adjustments.Cache,
					typeof(APAdjust.displayDocType),
					typeof(APAdjust.displayRefNbr),
					typeof(APAdjust.displayDocDate),
					typeof(APAdjust.displayDocDesc),
					typeof(APAdjust.displayCuryID),
					typeof(APAdjust.displayFinPeriodID),
					typeof(APAdjust.displayStatus)))
				{
					foreach (PXResult<Standalone.APRegisterAlias, CurrencyInfo, APAdjust, APPayment> res in 
						PXSelectJoin<
							Standalone.APRegisterAlias,
								InnerJoin<CurrencyInfo, 
									On<CurrencyInfo.curyInfoID, Equal<Standalone.APRegisterAlias.curyInfoID>>, 
								LeftJoin<APAdjust, 
									On<APAdjust.adjgDocType, Equal<Standalone.APRegisterAlias.docType>, 
									And<APAdjust.adjgRefNbr, Equal<Standalone.APRegisterAlias.refNbr>, 
									And<APAdjust.adjNbr, Equal<Standalone.APRegisterAlias.lineCntr>, 
									And<APAdjust.released, NotEqual<True>, 
									And<
										Where<APAdjust.adjdDocType, NotEqual<Current<APInvoice.docType>>, 
										Or<APAdjust.adjdRefNbr, NotEqual<Current<APInvoice.refNbr>>>>>>>>>,
								InnerJoinSingleTable<APPayment,
									On<APPayment.docType, Equal<Standalone.APRegisterAlias.docType>,
									And<APPayment.refNbr, Equal<Standalone.APRegisterAlias.refNbr>>>>>>, 
							Where<
								Standalone.APRegisterAlias.vendorID, Equal<Current<APInvoice.vendorID>>, 
								And2<Where<
									Standalone.APRegisterAlias.docType, Equal<APDocType.prepayment>, 
									Or<Standalone.APRegisterAlias.docType, Equal<APDocType.debitAdj>>>, 
								And2<Where<
								Standalone.APRegisterAlias.docDate, LessEqual<Current<APInvoice.docDate>>, 
								And<Standalone.APRegisterAlias.finPeriodID, LessEqual<Current<APRegister.finPeriodID>>, 
								And<Standalone.APRegisterAlias.released, Equal<True>, 
								And<Standalone.APRegisterAlias.openDoc, Equal<True>, 
								And<APAdjust.adjdRefNbr, IsNull>>>>>,
								And<Standalone.APRegisterAlias.hold, NotEqual<True>>>>>>
						.Select(this))
					{
						APPayment payment = res;
						APAdjust adj = new APAdjust();
						CurrencyInfo pay_info = res;

						PXCache<APRegister>.RestoreCopy(payment, (Standalone.APRegisterAlias)res);

						adj.VendorID = Document.Current.VendorID;
						adj.AdjdDocType = Document.Current.DocType;
						adj.AdjdRefNbr = Document.Current.RefNbr;
						adj.AdjdBranchID = Document.Current.BranchID;
						adj.AdjgDocType = payment.DocType;
						adj.AdjgRefNbr = payment.RefNbr;
						adj.AdjgBranchID = payment.BranchID;
						adj.AdjNbr = payment.LineCntr;

						if (Adjustments.Cache.Locate(adj) == null)
						{
							adj.AdjgCuryInfoID = payment.CuryInfoID;
							adj.AdjdOrigCuryInfoID = Document.Current.CuryInfoID;
							//if LE constraint is removed from payment selection this must be reconsidered
							adj.AdjdCuryInfoID = Document.Current.CuryInfoID;

							adj.AdjType = ARAdjust.adjType.Adjusting;
							adj.CuryDocBal = BalanceCalculation.CalculateApplicationDocumentBalance(
								Adjustments.Cache, 
								pay_info, 
								inv_info, 
								payment.DocBal, 
								payment.CuryDocBal);
							adj.DisplayDocType = payment.DocType;
							adj.DisplayRefNbr = payment.RefNbr;
							adj.DisplayDocDate = payment.DocDate;
							adj.DisplayDocDesc = payment.DocDesc;
							adj.DisplayCuryID = payment.CuryID;
							adj.DisplayFinPeriodID = payment.FinPeriodID;
							adj.DisplayStatus = payment.Status;

							adj = Adjustments.Insert(adj);
							yield return new PXResult<APAdjust, APPayment, CurrencyInfo>(adj, payment, pay_info);
						}
					}
				}
			}
		}

		#endregion

		#region APInvoice Events

		protected virtual void APInvoice_DocType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = APDocType.Invoice;
		}

		protected virtual void APInvoice_APAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (location.Current != null && e.Row != null)
			{
				e.NewValue = null;
				if (((APInvoice)e.Row).DocType == APDocType.Prepayment)
				{
					e.NewValue = GetAcctSub<Vendor.prepaymentAcctID>(vendor.Cache, vendor.Current);
				}
				if (string.IsNullOrEmpty((string)e.NewValue))
				{
					e.NewValue = GetAcctSub<Location.aPAccountID>(location.Cache, location.Current);
				}
			}
		}

		protected virtual void APInvoice_APSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (location.Current != null && e.Row != null)
			{
				e.NewValue = null;
				if (((APInvoice)e.Row).DocType == APDocType.Prepayment)
				{
					e.NewValue = GetAcctSub<Vendor.prepaymentSubID>(vendor.Cache, vendor.Current);
				}
				if (string.IsNullOrEmpty((string)e.NewValue))
				{
					e.NewValue = GetAcctSub<Location.aPSubID>(location.Cache, location.Current);
				}
			}
		}

		protected virtual void APInvoice_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			location.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<APInvoice.aPAccountID>(e.Row);
			sender.SetDefaultExt<APInvoice.aPSubID>(e.Row);
			sender.SetDefaultExt<APInvoice.termsID>(e.Row);
			sender.SetDefaultExt<APInvoice.branchID>(e.Row);
			sender.SetValue<APInvoice.payLocationID>(e.Row, sender.GetValue<APInvoice.vendorLocationID>(e.Row));
			sender.SetDefaultExt<APInvoice.payTypeID>(e.Row);
			sender.SetDefaultExt<APInvoice.separateCheck>(e.Row);
			sender.SetDefaultExt<APInvoice.taxCalcMode>(e.Row);
			sender.SetDefaultExt<APInvoice.taxZoneID>(e.Row);
			sender.SetDefaultExt<APInvoice.prebookAcctID>(e.Row);
			sender.SetDefaultExt<APInvoice.prebookSubID>(e.Row);
		}

		protected virtual void APInvoice_PayLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<APInvoice.separateCheck>(e.Row);
			sender.SetDefaultExt<APInvoice.payTypeID>(e.Row);

		}


		protected virtual void APInvoice_TermsID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APInvoice invoice = (APInvoice)e.Row;

			if (invoice != null && (invoice.DocType == APDocType.DebitAdj || invoice.DocType == APDocType.Prepayment))
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void APInvoice_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APInvoice invoice = (APInvoice)e.Row;

			vendor.RaiseFieldUpdated(sender, e.Row);

			Adjustments_Raw.Cache.Clear();
			Adjustments_Raw.Cache.ClearQueryCache(); // remove this line after AC-62581 fix in appropriate versions

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<APInvoice.curyInfoID>(sender, invoice);

				string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) == false)
				{
					sender.RaiseExceptionHandling<APInvoice.docDate>(invoice, invoice.DocDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				}

				if (info != null)
				{
					invoice.CuryID = info.CuryID;
				}
			}
		   
			sender.SetDefaultExt<APInvoice.vendorLocationID>(invoice);
			sender.SetDefaultExt<APInvoice.termsID>(invoice);

		}

		protected virtual void APInvoice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APInvoice doc = (APInvoice)e.Row;

			if (doc.DocType != APDocType.DebitAdj && doc.DocType != APDocType.Prepayment && string.IsNullOrEmpty(doc.TermsID))
			{
				sender.RaiseExceptionHandling<APInvoice.termsID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			Terms terms = (Terms)PXSelectorAttribute.Select<APInvoice.termsID>(Document.Cache, doc);

			if (vendor.Current != null && (bool)vendor.Current.Vendor1099 && terms != null)
			{

				if (terms.InstallmentType == CS.TermsInstallmentType.Multiple)
				{
					sender.RaiseExceptionHandling<APInvoice.termsID>(doc, doc.TermsID, new PXSetPropertyException(Messages.AP1099_Vendor_Cannot_Have_Multiply_Installments));
				}
			}

			EPEmployee emp = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<APInvoice.vendorID>>>>.Select(this);
			if (emp != null && terms != null)
			{
				if (PXCurrencyAttribute.IsNullOrEmpty(terms.DiscPercent) == false)
				{
					sender.RaiseExceptionHandling<APInvoice.termsID>(doc, doc.TermsID, new PXSetPropertyException(Messages.Employee_Cannot_Have_Discounts));
				}

				if (terms.InstallmentType == TermsInstallmentType.Multiple)
				{
					sender.RaiseExceptionHandling<APInvoice.termsID>(e.Row, doc.TermsID, new PXSetPropertyException(Messages.Employee_Cannot_Have_Multiply_Installments));
				}
			}

			if (doc.DocType != APDocType.DebitAdj && doc.DueDate == null)
			{
				sender.RaiseExceptionHandling<APInvoice.dueDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType != APDocType.DebitAdj && doc.PaySel == true && doc.PayDate == null)
			{
				sender.RaiseExceptionHandling<APInvoice.payDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType != APDocType.DebitAdj && doc.PaySel == true && doc.PayDate != null && ((DateTime)doc.DocDate).CompareTo((DateTime)doc.PayDate) > 0)
			{
				sender.RaiseExceptionHandling<APInvoice.payDate>(e.Row, doc.PayDate, new PXSetPropertyException(Messages.ApplDate_Less_DocDate, PXErrorLevel.RowError));
			}

			if (doc.DocType != APDocType.DebitAdj && doc.PaySel == true && doc.PayLocationID == null)
			{
				sender.RaiseExceptionHandling<APInvoice.payLocationID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType != APDocType.DebitAdj && doc.PaySel == true && doc.PayAccountID == null)
			{
				sender.RaiseExceptionHandling<APInvoice.payAccountID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType != APDocType.DebitAdj && doc.PaySel == true && doc.PayTypeID == null)
			{
				sender.RaiseExceptionHandling<APInvoice.payTypeID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType == APDocType.Prepayment && doc.PaySel == true && doc.PayAccountID != null)
			{
				object PayAccountID = doc.PayAccountID;

				try
				{
					sender.RaiseFieldVerifying<APInvoice.payAccountID>(doc, ref PayAccountID);
				}
				catch (PXSetPropertyException ex)
				{
					sender.RaiseExceptionHandling<APInvoice.payAccountID>(doc, PayAccountID, ex);
				}
			}

			if (doc.DocType != APDocType.DebitAdj && doc.DocType != APDocType.Prepayment && doc.DiscDate == null)
			{
				sender.RaiseExceptionHandling<APInvoice.discDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (string.IsNullOrEmpty(doc.InvoiceNbr) && IsInvoiceNbrRequired(doc))
			{
				sender.RaiseExceptionHandling<APInvoice.invoiceNbr>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
			}

			if (doc.DocType == APDocType.Prepayment && doc.OpenDoc == true && doc.Voided == true)
			{
				doc.OpenDoc = false;
				doc.ClosedFinPeriodID = doc.FinPeriodID;
				doc.ClosedTranPeriodID = doc.TranPeriodID;
			}

			if (doc.CuryDiscTot > Math.Abs(doc.CuryLineTotal ?? 0m))
			{
				sender.RaiseExceptionHandling<APInvoice.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(Messages.DiscountGreaterLineTotal, PXErrorLevel.Error));
			}

			//Cancel tax if document is deleted
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete && doc.IsTaxSaved == true)
			{
				CancelTax(doc, CancelCode.DocDeleted);
			}

			//Cancel tax if last line in the document is deleted
			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
				&& doc.IsTaxSaved == true && !Transactions.Any())
			{
				CancelTax(doc, CancelCode.DocDeleted);
			}

			//Cancel tax if IsExternalTax has changed to False (Document was changed from Avalara TaxEngine to Acumatica Tax Engine )
			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
				&& IsExternalTax == false && doc.IsTaxSaved == true)
			{
				CancelTax(doc, CancelCode.DocDeleted);
			}

			ValidateAPAndReclassificationAccoutsAndSubs(sender, doc);
		}

		protected virtual bool IsInvoiceNbrRequired(APInvoice doc)
		{
			return APSetup.Current.RequireVendorRef == true
				&& doc.DocType != APDocType.DebitAdj
				&& doc.DocType != APDocType.CreditAdj
				&& doc.DocType != APDocType.Prepayment
				&& (vendor.Current == null
				|| vendor.Current.TaxAgency == false)
				&& doc.OrigDocType == null 
				&& doc.OrigRefNbr == null;
		}

		protected virtual void APInvoice_DocDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APInvoice doc = (APInvoice)e.Row;

			CurrencyInfoAttribute.SetEffectiveDate<APInvoice.docDate>(sender, e);

			if (doc.DocType == APDocType.Prepayment)// && doc.DueDate != null && DateTime.Compare((DateTime)doc.DocDate, (DateTime)doc.DueDate) > 0)
			{
				sender.SetDefaultExt<APInvoice.dueDate>(doc);
			}
		}

		protected virtual void APInvoice_DueDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APInvoice invoice = (APInvoice)e.Row;
			if (invoice.DocType == APDocType.Prepayment)
			{
				e.NewValue = invoice.DocDate;
			}
		}

		protected virtual void APInvoice_TermsID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Terms terms = (Terms)PXSelectorAttribute.Select<APInvoice.termsID>(sender, e.Row);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single)
			{
				foreach (APAdjust adj in Adjustments.Select())
				{
					Adjustments.Cache.Delete(adj);
				}
			}
		}

		protected virtual void APInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			APInvoice document = e.Row as APInvoice;

			if (document == null) return;

			bool hasPOLink = PXSelect<
				APTran, 
				Where<
					APTran.tranType, Equal<Current<APInvoice.docType>>, 
					And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>,
					And<Where<
						APTran.pOLineNbr, IsNotNull, 
						Or<APTran.pONbr, IsNotNull, 
						Or<APTran.receiptLineNbr, IsNotNull , 
						Or<APTran.receiptNbr, IsNotNull>>>>>>>>
				.Select(this).Count != 0;

			PXUIFieldAttribute.SetRequired<APInvoice.invoiceNbr>(cache, IsInvoiceNbrRequired(document));
			PXUIFieldAttribute.SetVisible<APInvoice.curyID>(cache, document, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			bool isDocumentPrepayment = document.DocType == APDocType.Prepayment;
			bool isDocumentInvoice = document.DocType == APDocType.Invoice;
			bool isDocumentDebitAdjustment = document.DocType == APDocType.DebitAdj;

			bool isDocumentOnHold = document.Hold == true;
			bool isDocumentScheduled = document.Scheduled == true;
			bool isDocumentPrebookedNotCompleted = document.Prebooked == true && document.Released == false && document.Voided == false;
			bool isDocumentReleasedOrPrebooked = document.Released == true || document.Prebooked == true;
			bool isDocumentVoided = document.Voided == true;
			bool isDocumentRejected = document.Rejected == true;
			bool isDocumentRejectedOrPendingApproval =
				!isDocumentOnHold && 
				!isDocumentScheduled &&
				!isDocumentReleasedOrPrebooked && 
				!isDocumentVoided && (
					isDocumentRejected ||
					document.Approved != true && document.DontApprove != true);
			bool isDocumentApprovedBalanced =
				!isDocumentOnHold &&
				!isDocumentScheduled &&
				!isDocumentReleasedOrPrebooked &&
				!isDocumentVoided &&
				document.Approved == true &&
				document.DontApprove != true;

			PXUIFieldAttribute.SetRequired<APInvoice.termsID>(cache, !isDocumentDebitAdjustment && !isDocumentPrepayment);
			PXUIFieldAttribute.SetRequired<APInvoice.dueDate>(cache, !isDocumentDebitAdjustment);
			PXUIFieldAttribute.SetRequired<APInvoice.discDate>(cache, !isDocumentDebitAdjustment && !isDocumentPrepayment);

			this.addPOOrder.SetVisible(isDocumentInvoice);
			this.addPOReceipt.SetVisible(isDocumentInvoice || isDocumentDebitAdjustment);
			this.addReceiptLine.SetVisible(isDocumentInvoice);
			this.viewPODocument.SetVisible(isDocumentInvoice || isDocumentDebitAdjustment);
			this.addPostLandedCostTran.SetVisible(isDocumentInvoice);
			this.lsLCSplits.SetVisible(isDocumentInvoice);
			this.linkLine.SetVisible(isDocumentInvoice);

			this.prebook.SetVisible(document.DocType != APDocType.Prepayment);
			this.voidInvoice.SetVisible(document.DocType != APDocType.Prepayment); 

			this.addPOReceipt.SetEnabled(false);
			this.addPOOrder.SetEnabled(false);
			this.addReceiptLine.SetEnabled(false);
			this.prebook.SetEnabled(false);
			this.voidInvoice.SetEnabled(false);

			this.addPostLandedCostTran.SetEnabled(false);
			this.lsLCSplits.SetEnabled(false);

			bool curyenabled = true;
			bool vendor1099 = false;

			PXUIFieldAttribute.SetEnabled(this.poReceiptLinesSelection.Cache, null, false);
			PXUIFieldAttribute.SetEnabled(this.poreceiptslist.Cache, null, false);
			PXUIFieldAttribute.SetEnabled(this.poorderslist.Cache, null, false);

			if (vendor.Current?.AllowOverrideCury != true)
			{
				curyenabled = false;
			}

			if (vendor.Current?.Vendor1099 == true)
			{
				vendor1099 = true;
			}

			if (vendor.Current?.TaxAgency == true)
			{
				PXUIFieldAttribute.SetEnabled<APInvoice.taxZoneID>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APTran.taxCategoryID>(Transactions.Cache, null, false);
			}

			bool landedCostEnabled = false;

			if (document.VendorID.HasValue && document.VendorLocationID.HasValue && (isDocumentInvoice || isDocumentDebitAdjustment))
			{
				if (this.vendor.Current != null)
				{
					landedCostEnabled = ((bool)this.vendor.Current.LandedCostVendor) || (this.landedCostTrans.Any());
				}
			}

			document.LCEnabled = landedCostEnabled;

			if (isDocumentReleasedOrPrebooked || isDocumentVoided)
			{
				bool Enable1099 = (vendor.Current != null && vendor.Current.Vendor1099 == true && document.Voided == false);
				bool hasAdjustments = false;
				foreach (APAdjust adj in Adjustments.Select())
				{
					string Year1099 = ((DateTime)adj.AdjgDocDate).Year.ToString();

					AP1099Year year = PXSelect<AP1099Year, Where<AP1099Year.finYear, Equal<Required<AP1099Year.finYear>>>>.Select(this, Year1099);

					if (year != null && year.Status != "N" || adj.AdjDiscAmt != 0m)
					{
						Enable1099 = false;
					}
					hasAdjustments = true;
				}
				PXUIFieldAttribute.SetEnabled(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.dueDate>(cache, document, !isDocumentDebitAdjustment && (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.paySel>(cache, document, !isDocumentDebitAdjustment && (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payLocationID>(cache, document, (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payAccountID>(cache, document, (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payTypeID>(cache, document, (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payDate>(cache, document, !isDocumentDebitAdjustment && (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.discDate>(cache, document, !isDocumentDebitAdjustment && !isDocumentPrepayment && (bool)document.OpenDoc);

				cache.AllowDelete = false;
				cache.AllowUpdate = (bool)document.OpenDoc || Enable1099 || isDocumentPrebookedNotCompleted;
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = Enable1099 || isDocumentPrebookedNotCompleted;
				Transactions.Cache.AllowInsert = false;

				DiscountDetails.Cache.AllowDelete = false;
				DiscountDetails.Cache.AllowUpdate = false;
				DiscountDetails.Cache.AllowInsert = false;

				Taxes.Cache.AllowUpdate = false;

				release.SetEnabled(isDocumentPrebookedNotCompleted);

				bool hasPOLinks = false;

				if (!hasAdjustments)
				{
					APTran tran = PXSelectReadonly<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
													And<Where<APTran.lCTranID, IsNotNull, Or<APTran.pONbr, IsNotNull, Or<APTran.receiptNbr, IsNotNull>>>>>>>.Select(this, document.DocType, document.RefNbr);
					hasPOLinks = (tran != null);
				}
				if (this._allowToVoidReleased)
				{
					voidInvoice.SetEnabled((document.Released == true || document.Prebooked == true) && !hasPOLinks && document.Voided == false && !hasAdjustments);
				}
				else
				{
					voidInvoice.SetEnabled(isDocumentPrebookedNotCompleted && !hasPOLinks);
				}

				createSchedule.SetEnabled(false);
				payInvoice.SetEnabled((bool)document.OpenDoc && ((bool)document.Payable || document.DocType == APInvoiceType.Prepayment));
				this.landedCostTrans.Cache.AllowDelete = false;
				this.landedCostTrans.Cache.AllowUpdate = false;
				this.landedCostTrans.Cache.AllowInsert = false;

				if (Enable1099 || isDocumentPrebookedNotCompleted)
				{
					PXUIFieldAttribute.SetEnabled(Transactions.Cache, null, false);
					PXUIFieldAttribute.SetEnabled<APTran.box1099>(Transactions.Cache, null, Enable1099);

					PXUIFieldAttribute.SetEnabled<APTran.accountID>(Transactions.Cache, null, isDocumentPrebookedNotCompleted);
					PXUIFieldAttribute.SetEnabled<APTran.subID>(Transactions.Cache, null, isDocumentPrebookedNotCompleted);
					PXUIFieldAttribute.SetEnabled<APTran.branchID>(Transactions.Cache, null, isDocumentPrebookedNotCompleted);
					PXUIFieldAttribute.SetEnabled<APTran.projectID>(Transactions.Cache, null, isDocumentPrebookedNotCompleted);
					PXUIFieldAttribute.SetEnabled<APTran.taskID>(Transactions.Cache, null, isDocumentPrebookedNotCompleted);
				}
			}
			else if (
				isDocumentRejectedOrPendingApproval ||
				isDocumentApprovedBalanced)
			{
				PXUIFieldAttribute.SetEnabled(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APRegister.hold>(cache, document);

				// For non-rejected documents, default payment info should be editable.
				// -
				PXUIFieldAttribute.SetEnabled<APInvoice.separateCheck>(cache, document, !isDocumentRejected);
				PXUIFieldAttribute.SetEnabled<APInvoice.paySel>(cache, document, !isDocumentRejected);
				PXUIFieldAttribute.SetEnabled<APInvoice.payDate>(cache, document, !isDocumentRejected);
				PXUIFieldAttribute.SetEnabled<APInvoice.payLocationID>(cache, document, !isDocumentRejected);
				PXUIFieldAttribute.SetEnabled<APInvoice.payTypeID>(cache, document, !isDocumentRejected);
				PXUIFieldAttribute.SetEnabled<APInvoice.payAccountID>(cache, document, !isDocumentRejected);

				Transactions.Cache.AllowDelete =
				Transactions.Cache.AllowUpdate = 
				Transactions.Cache.AllowInsert =
				DiscountDetails.Cache.AllowDelete =
				DiscountDetails.Cache.AllowUpdate =
				DiscountDetails.Cache.AllowInsert =
				Taxes.Cache.AllowDelete =
				Taxes.Cache.AllowUpdate =
				Taxes.Cache.AllowInsert = false;

				release.SetEnabled(isDocumentApprovedBalanced);
				prebook.SetEnabled(isDocumentApprovedBalanced);

				createSchedule.SetEnabled(false);
				payInvoice.SetEnabled(false);
				recalculateDiscountsAction.SetEnabled(false);

				addPOReceipt.SetEnabled(false);
				addPOOrder.SetEnabled(false);
				addReceiptLine.SetEnabled(false);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled(cache, document, true);
				PXUIFieldAttribute.SetEnabled<APInvoice.status>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyDocBal>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyLineTotal>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyTaxTotal>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyOrigWhTaxAmt>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyVatExemptTotal>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyVatTaxableTotal>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.batchNbr>(cache, document, false);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyID>(cache, document, curyenabled);
				PXUIFieldAttribute.SetEnabled<APInvoice.hold>(cache, document, ((bool)document.Scheduled == false));

				PXUIFieldAttribute.SetEnabled<APInvoice.termsID>(cache, document, !isDocumentDebitAdjustment && !isDocumentPrepayment);
				PXUIFieldAttribute.SetEnabled<APInvoice.dueDate>(cache, document, !isDocumentDebitAdjustment);
				PXUIFieldAttribute.SetEnabled<APInvoice.discDate>(cache, document, !isDocumentDebitAdjustment && !isDocumentPrepayment);
				PXUIFieldAttribute.SetEnabled<APInvoice.curyOrigDiscAmt>(cache, document, !isDocumentDebitAdjustment && !isDocumentPrepayment);

				PXUIFieldAttribute.SetEnabled(Transactions.Cache, null, true);

				//PXUIFieldAttribute.SetEnabled<APTran.deferredCode>(Transactions.Cache, null, (doc.DocType == APDocType.Invoice || doc.DocType == APDocType.CreditAdj));
				PXUIFieldAttribute.SetEnabled<APTran.defScheduleID>(Transactions.Cache, null, (document.DocType == APDocType.DebitAdj));
				PXUIFieldAttribute.SetEnabled<APTran.curyTranAmt>(Transactions.Cache, null, false);
				PXUIFieldAttribute.SetEnabled<APTran.discountSequenceID>(Transactions.Cache, null, false);

				//calculate only on data entry, differences from the applications will be moved to RGOL upon closure
				PXDBCurrencyAttribute.SetBaseCalc<APInvoice.curyDocBal>(cache, null, true);
				PXDBCurrencyAttribute.SetBaseCalc<APInvoice.curyDiscBal>(cache, null, true);

				PXUIFieldAttribute.SetEnabled<APInvoice.payAccountID>(cache, document, (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payTypeID>(cache, document, (bool)document.OpenDoc);
				PXUIFieldAttribute.SetEnabled<APInvoice.payDate>(cache, document, !isDocumentDebitAdjustment);

				bool hasLCTrans = this.landedCostTrans.Any();
				PXUIFieldAttribute.SetEnabled<APInvoice.vendorID>(cache, document, !hasLCTrans);
				PXUIFieldAttribute.SetEnabled<APInvoice.vendorLocationID>(cache, document, !hasLCTrans);

				cache.AllowDelete = true;
				cache.AllowUpdate = true;
				Transactions.Cache.AllowDelete = true;
				Transactions.Cache.AllowUpdate = true;
				Transactions.Cache.AllowInsert = (document.VendorID != null) && (document.VendorLocationID != null);

				DiscountDetails.Cache.AllowDelete = true;
				DiscountDetails.Cache.AllowUpdate = true;
				DiscountDetails.Cache.AllowInsert = true;

				this.landedCostTrans.Cache.AllowDelete = true;

				Vendor vendorRecord = this.vendor.Select();

				if (vendorRecord != null)
				{
					bool isLCVendor = (bool)vendorRecord.LandedCostVendor;
					this.landedCostTrans.Cache.AllowUpdate = isLCVendor;
					this.landedCostTrans.Cache.AllowInsert = isLCVendor;
					if (hasLCTrans && !isLCVendor)
					{
						cache.RaiseExceptionHandling<APInvoice.vendorID>(document, vendorRecord.AcctCD, new PXSetPropertyKeepPreviousException(Messages.APLanededCostTranForNonLCVendor, PXErrorLevel.Warning));
					}
					this.addPostLandedCostTran.SetEnabled(isLCVendor);
					if (vendorRecord.Status == Vendor.status.Inactive)
					{
						cache.RaiseExceptionHandling<APInvoice.vendorID>(document, vendorRecord.AcctCD, new PXSetPropertyException(Messages.VendorIsInStatus, PXErrorLevel.Warning, CR.Messages.Inactive));
					}
				}

				Taxes.Cache.AllowUpdate = true;

				release.SetEnabled(!isDocumentOnHold && !isDocumentScheduled);
				prebook.SetEnabled(document.DocType != APDocType.Prepayment && !isDocumentOnHold && !isDocumentScheduled);
				createSchedule.SetEnabled(!isDocumentOnHold && document.DocType == APDocType.Invoice);
				payInvoice.SetEnabled(false);
				addPOReceipt.SetEnabled(true);
				addPOOrder.SetEnabled(true);
				addReceiptLine.SetEnabled(true);

				PXUIFieldAttribute.SetEnabled<POReceiptLineS.selected>(this.poReceiptLinesSelection.Cache, null, true);
				PXUIFieldAttribute.SetEnabled<POReceipt.selected>(this.poreceiptslist.Cache, null, true);
				PXUIFieldAttribute.SetEnabled<POOrderRS.selected>(this.poorderslist.Cache, null, true);
			}

			PXUIFieldAttribute.SetEnabled<APInvoice.docType>(cache, document);
			PXUIFieldAttribute.SetEnabled<APInvoice.refNbr>(cache, document);

			Taxes.Cache.AllowDelete = Transactions.Cache.AllowDelete;
			Taxes.Cache.AllowInsert = Transactions.Cache.AllowInsert;

			Adjustments.Cache.AllowInsert = false;
			Adjustments.Cache.AllowDelete = false;
			Adjustments.Cache.AllowUpdate = 
				isDocumentRejectedOrPendingApproval || isDocumentApprovedBalanced ?
				!isDocumentRejected :
				Transactions.Cache.AllowUpdate && !isDocumentPrebookedNotCompleted;

			editVendor.SetEnabled(vendor?.Current != null);

			reverseInvoice.SetEnabled(!isDocumentDebitAdjustment && isDocumentReleasedOrPrebooked && !isDocumentPrepayment);
			vendorRefund.SetEnabled(isDocumentDebitAdjustment && isDocumentReleasedOrPrebooked && !isDocumentPrepayment);
			reclassifyBatch.SetEnabled(document.Released == true && !isDocumentPrepayment);

			if (document.VendorID != null)
			{
				if (Transactions.Any())
				{
					PXUIFieldAttribute.SetEnabled<APInvoice.vendorID>(cache, document, false);
				}               
			}
			if (document.VendorLocationID != null)
			{
				bool hasLCTrans = this.landedCostTrans.Any();

				PXUIFieldAttribute.SetEnabled<APInvoice.vendorLocationID>(
					cache, 
					document, 
					!(
						hasLCTrans || 
						isDocumentReleasedOrPrebooked ||
						isDocumentRejectedOrPendingApproval ||
						isDocumentApprovedBalanced ||
						(bool)document.Voided
					));
			}
			PXUIFieldAttribute.SetVisible<APInvoice.curyOrigDocAmt>(cache, document, APSetup.Current.RequireControlTotal == true || isDocumentReleasedOrPrebooked);
			PXUIFieldAttribute.SetVisible<APInvoice.curyTaxAmt>(cache, document, PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() && 
				(APSetup.Current.RequireControlTaxTotal == true));
			PXUIFieldAttribute.SetVisible<APTran.box1099>(Transactions.Cache, null, vendor1099);
			PXUIFieldAttribute.SetVisible<APInvoice.prebookBatchNbr>(cache, document, document.Prebooked == true);
			PXUIFieldAttribute.SetVisible<APInvoice.voidBatchNbr>(cache, document, document.Voided == true);
			PXUIFieldAttribute.SetVisible<APInvoice.batchNbr>(cache, document, document.Voided != true);

			this.addPOOrder.SetEnabled(
				!isDocumentReleasedOrPrebooked && 
				!isDocumentRejectedOrPendingApproval && 
				!isDocumentApprovedBalanced &&
				this.vendor.Current != null);

			this.addPOReceipt.SetEnabled(
				!isDocumentReleasedOrPrebooked && 
				!isDocumentRejectedOrPendingApproval && 
				!isDocumentApprovedBalanced &&
				this.vendor.Current != null);

			this.addReceiptLine.SetEnabled(
				!isDocumentReleasedOrPrebooked && 
				!isDocumentRejectedOrPendingApproval && 
				!isDocumentApprovedBalanced &&
				this.vendor.Current != null);

			this.lsLCSplits.SetEnabled(landedCostEnabled);

			this.LCTranSplit.Cache.AllowInsert = 
			this.LCTranSplit.Cache.AllowUpdate = 
			this.LCTranSplit.Cache.AllowDelete = 
				landedCostEnabled && 
				!isDocumentReleasedOrPrebooked && 
				!isDocumentRejectedOrPendingApproval &&
				!isDocumentApprovedBalanced;

			PXUIFieldAttribute.SetEnabled(
				LCTranSplit.Cache, 
				null, 
					landedCostEnabled && 
					!isDocumentReleasedOrPrebooked && 
					!isDocumentRejectedOrPendingApproval &&
					!isDocumentApprovedBalanced);

			linkLine.SetEnabled(
				!isDocumentReleasedOrPrebooked && 
				!isDocumentRejectedOrPendingApproval &&
				!isDocumentApprovedBalanced &&
				(!PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() || document.TaxCalcMode == VendorClass.taxCalcMode.TaxSetting));

			if (IsExternalTax == true && ((APInvoice)e.Row).IsTaxValid != true)
				PXUIFieldAttribute.SetWarning<APInvoice.curyTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);

			if (hasPOLink)
			{
				PXUIFieldAttribute.SetEnabled<APInvoice.taxCalcMode>(cache, document, false);
			}

			bool showRoundingDiff = document.CuryRoundDiff != 0 || PXAccess.FeatureInstalled<FeaturesSet.invoiceRounding>();
			PXUIFieldAttribute.SetVisible<APInvoice.curyRoundDiff>(cache, document, showRoundingDiff);

			if (UseTaxes.Select().Count != 0)
			{
				cache.RaiseExceptionHandling<APInvoice.curyTaxTotal>(document, document.CuryTaxTotal, new PXSetPropertyException(TX.Messages.UseTaxExcludedFromTotals, PXErrorLevel.Warning));
			}
			PXUIFieldAttribute.SetVisible<APInvoice.usesManualVAT>(cache, document, document.UsesManualVAT == true);

			Taxes.Cache.AllowInsert = document.UsesManualVAT != true;
			Taxes.Cache.AllowDelete = document.UsesManualVAT != true;
			Taxes.Cache.AllowUpdate = document.UsesManualVAT != true;
		}

		protected virtual void APInvoice_PayTypeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<APInvoice.payAccountID>(e.Row);
		}

		protected virtual void APInvoice_PayAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			APInvoice doc = e.Row as APInvoice;
			if (doc == null) return;

			CashAccount cashacct = (CashAccount)PXSelectorAttribute.Select<APInvoice.payAccountID>(sender, doc, e.NewValue);
			if (cashacct != null)
			{
				if (doc.DocType == APDocType.Prepayment && object.Equals(cashacct.CuryID, doc.CuryID) == false)
				{
					e.NewValue = cashacct.CashAccountCD;
					throw new PXSetPropertyException(Messages.CashCuryNotPPCury);
				}

				if (cashacct.RestrictVisibilityWithBranch == true && cashacct.BranchID != doc.BranchID)
				{
					e.NewValue = null;
				}
			}
		}

		protected virtual void APInvoice_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			foreach (APTaxTran taxTran in Taxes.Select())
			{
				if (Taxes.Cache.GetStatus(taxTran) == PXEntryStatus.Notchanged)
				{
					Taxes.Cache.SetStatus(taxTran, PXEntryStatus.Updated);
				}
			}

			sender.SetDefaultExt<APInvoice.payAccountID>(e.Row);
		}

		private void ValidateAPAndReclassificationAccoutsAndSubs(PXCache sender, APInvoice invoice)
		{
			string errorMsg = null;

			var subsEnbaled = PXAccess.FeatureInstalled<FeaturesSet.subAccount>();

			var accountIdentical = invoice.PrebookAcctID == invoice.APAccountID;

			if (accountIdentical && !subsEnbaled)
			{
				errorMsg = Messages.APAndReclassAccountBoxesShouldNotHaveTheSameAccounts;
			}
			else if (accountIdentical && subsEnbaled && invoice.PrebookSubID == invoice.APSubID)
			{
				errorMsg = Messages.APAndReclassAccountSubaccountBoxesShouldNotHaveTheSameAccountSubaccountPairs;
			}

			if (errorMsg != null)
			{
				var errorEx = new PXSetPropertyException(errorMsg, PXErrorLevel.Error);

				var acctIDState = (PXFieldState) sender.GetStateExt<APInvoice.prebookAcctID>(invoice);
				sender.RaiseExceptionHandling<APInvoice.prebookAcctID>(invoice, acctIDState.Value, errorEx);

				var subIDState = (PXFieldState) sender.GetStateExt<APInvoice.prebookSubID>(invoice);
				sender.RaiseExceptionHandling<APInvoice.prebookSubID>(invoice, subIDState.Value, errorEx);
			}
		}

		protected virtual void APInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			APInvoice doc = e.Row as APInvoice;
			if (doc == null) return;
			if (doc.Released != true)
			{
				if (e.ExternalCall && ((!sender.ObjectsEqual<APInvoice.docDate>(e.OldRow, e.Row) && doc.OrigDocType == null && doc.OrigRefNbr == null) || !sender.ObjectsEqual<APInvoice.vendorLocationID>(e.OldRow, e.Row)))
				{
					DiscountEngine<APTran>.AutoRecalculatePricesAndDiscounts<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, doc.VendorLocationID, doc.DocDate);
				}
				if (sender.GetStatus(doc) != PXEntryStatus.Deleted && !sender.ObjectsEqual<APInvoice.curyDiscTot>(e.OldRow, e.Row))
				{
					if (!sender.Graph.IsImport) AddDiscount(sender, doc);
					if (e.ExternalCall) DiscountEngine<APTran>.CalculateDocumentDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, null, DiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m);
				}
				if (doc.Released != true && doc.Prebooked != true)
				{
					if (APSetup.Current.RequireControlTotal != true)
					{
						if (doc.CuryDocBal != doc.CuryOrigDocAmt)
						{
							if (doc.CuryDocBal != null && doc.CuryDocBal != 0)
								sender.SetValueExt<APInvoice.curyOrigDocAmt>(doc, doc.CuryDocBal);
							else
								sender.SetValueExt<APInvoice.curyOrigDocAmt>(doc, 0m);
						}
					}

					if (doc.DocType == APDocType.Prepayment && doc.DueDate == null)
					{
						sender.SetValue<APInvoice.dueDate>(e.Row, this.Accessinfo.BusinessDate);
					}
				}

				if (doc.Hold != true && doc.Released != true && doc.Prebooked != true)
				{
					if (doc.CuryDocBal != doc.CuryOrigDocAmt)
					{
						sender.RaiseExceptionHandling<APInvoice.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else if (doc.CuryOrigDocAmt < 0m)
					{
						if (APSetup.Current.RequireControlTotal == true)
						{
							sender.RaiseExceptionHandling<APInvoice.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, new PXSetPropertyException(Messages.DocumentBalanceNegative));
						}
						else
						{
							sender.RaiseExceptionHandling<APInvoice.curyDocBal>(doc, doc.CuryDocBal, new PXSetPropertyException(Messages.DocumentBalanceNegative));
						}
					}
					else
					{
						if (APSetup.Current.RequireControlTotal == true)
						{
							sender.RaiseExceptionHandling<APInvoice.curyOrigDocAmt>(doc, null, null);
						}
						else
						{
							sender.RaiseExceptionHandling<APInvoice.curyDocBal>(doc, null, null);
						}
					}
				}
			}

			bool checkControlTaxTotal = APSetup.Current.RequireControlTaxTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>();

			if (doc.Hold != true && doc.Released != true && doc.Prebooked != true
				&& doc.CuryTaxTotal != doc.CuryTaxAmt && checkControlTaxTotal)
			{
				sender.RaiseExceptionHandling<APInvoice.curyTaxAmt>(doc, doc.CuryTaxAmt, new PXSetPropertyException(Messages.TaxTotalAmountDoesntMatch));
			}
			else
			{
				if (checkControlTaxTotal)
				{
					if (PXAccess.FeatureInstalled<FeaturesSet.manualVATEntryMode>() && doc.CuryTaxAmt < 0)
					{
						sender.RaiseExceptionHandling<APInvoice.curyTaxAmt>(doc, doc.CuryTaxAmt, new PXSetPropertyException(Messages.ValueMustBeGreaterThanZero));
					}
					else
					{
					sender.RaiseExceptionHandling<APInvoice.curyTaxAmt>(doc, null, null);
				}
				}
				else
				{
					sender.SetValueExt<APInvoice.curyTaxAmt>(doc, doc.CuryTaxTotal != null && doc.CuryTaxTotal != 0 ? doc.CuryTaxTotal : 0m);
				}
			}

			sender.RaiseExceptionHandling<APInvoice.curyRoundDiff>(doc, null, null);

			if (doc.Hold != true && doc.Released != true && doc.Prebooked != true && doc.RoundDiff != 0)
			{
				if (checkControlTaxTotal || PXAccess.FeatureInstalled<FeaturesSet.invoiceRounding>() && doc.TaxRoundDiff == 0)
				{
					if (Math.Abs(doc.RoundDiff.Value) > Math.Abs(glsetup.Current.RoundingLimit.Value))
					{
						sender.RaiseExceptionHandling<APInvoice.curyRoundDiff>(doc, doc.CuryRoundDiff,
							new PXSetPropertyException(Messages.RoundingAmountTooBig, currencyinfo.Current.BaseCuryID, PXDBQuantityAttribute.Round(doc.RoundDiff),
								PXDBQuantityAttribute.Round(glsetup.Current.RoundingLimit)));
					}
				}
				else
				{
					if (!PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
					{
						sender.RaiseExceptionHandling<APInvoice.curyRoundDiff>(doc, doc.CuryRoundDiff,
							new PXSetPropertyException(Messages.CannotEditTaxAmtWOFeature));
					}
					else
					{
						sender.RaiseExceptionHandling<APInvoice.curyRoundDiff>(doc, doc.CuryRoundDiff,
							new PXSetPropertyException(Messages.CannotEditTaxAmtWOAPSetup));
					}
				}
			}
		}

		protected virtual void APInvoice_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			EPExpenseClaim claim = expenseclaim.Select();
			if (claim != null)
			{
				throw new PXException(Messages.DocumentCannotBeDeleted);
				}
			}
		#endregion

		#region APTran Events
		protected virtual void APTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran row = (APTran)e.Row;

			if (row != null && row.InventoryID != null)
			{
				InventoryItem item = nonStockItem.Select(row.InventoryID);
				if (item != null && item.StkItem == true)
				{
					e.NewValue = null;
					e.Cancel = true;
					return;
				}
			}
			
			if (vendor.Current == null || row == null || row.InventoryID != null) return;
			
			switch (vendor.Current.Type)
			{
				case BAccountType.VendorType:
				case BAccountType.CombinedType:
					if (location.Current.VExpenseAcctID != null)
					{
						e.NewValue = location.Current.VExpenseAcctID;
					}
					break;
				case BAccountType.EmployeeType:
					if (employee.Current.ExpenseAcctID != null)
					{
						e.NewValue = employee.Current.ExpenseAcctID;
					}
					break;
			}
			e.Cancel = true;
		}

		protected virtual void APTran_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (vendor.Current != null && (bool)vendor.Current.Vendor1099)
			{
				sender.SetDefaultExt<APTran.box1099>(e.Row);				
			}
			sender.SetDefaultExt<APTran.projectID>(e.Row);
		}

		protected virtual void APTran_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran documentLine = e.Row as APTran;

			if (documentLine == null) return;

			InventoryItem item = nonStockItem.Select(documentLine.InventoryID);

			if (item != null && item.StkItem == true)
			{
				e.NewValue = null;
				e.Cancel = true;
				return;
			}

			if (documentLine.AccountID == null || 
				vendor.Current?.Type == null ||
				!string.IsNullOrEmpty(documentLine.PONbr) ||
				!string.IsNullOrEmpty(documentLine.ReceiptNbr))
			{
				return;
			}

			EPEmployee employeeByUser = PXSelect<
				EPEmployee, 
				Where<
					EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>
				.Select(this, Document.Current?.EmployeeID ?? PXAccess.GetUserID());

			CRLocation companyLocation = PXSelectJoin<
				CRLocation, 
				InnerJoin<BAccountR, 
					On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, 
					And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, 
				InnerJoin<Branch, 
					On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>, 
				Where<
					Branch.branchID, Equal<Required<APTran.branchID>>>>
				.Select(this, documentLine.BranchID);

			Contract project = PXSelect<
				Contract, 
				Where<
					Contract.contractID, Equal<Required<Contract.contractID>>>>
				.Select(this, documentLine.ProjectID);

			string expenseSubMask = APSetup.Current.ExpenseSubMask;

			int? projectTaskSubaccountID = null;

			if (project == null || project.BaseType == CT.Contract.ContractBaseType.Contract)
			{
				project = PXSelect<CT.Contract, Where<CT.Contract.nonProject, Equal<True>>>.Select(this);
				expenseSubMask = expenseSubMask.Replace(APAcctSubDefault.MaskTask, APAcctSubDefault.MaskProject);
			}
			else
			{
				PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, documentLine.TaskID);
				if (task != null)
					projectTaskSubaccountID = task.DefaultSubID;
			}
			
			int? vendorSubaccountID = null;

			switch (vendor.Current.Type)
			{
				case BAccountType.VendorType:
				case BAccountType.CombinedType:
					vendorSubaccountID = (int?)Caches[typeof(Location)].GetValue<Location.vExpenseSubID>(location.Current);
					break;
				case BAccountType.EmployeeType:
					vendorSubaccountID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(employee.Current);
					break;
			}

			int? itemSubaccountID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.cOGSSubID>(item);
			int? employeeByUserSubaccountID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(employeeByUser);
			int? companySubaccountID = (int?)Caches[typeof(CRLocation)].GetValue<CRLocation.cMPExpenseSubID>(companyLocation);
			int? projectSubaccountID = project.DefaultSubID;

			object subaccountValue = SubAccountMaskAttribute.MakeSub<APSetup.expenseSubMask>(
				this, 
				expenseSubMask,
				new object[]
				{
					vendorSubaccountID,
					itemSubaccountID,
					employeeByUserSubaccountID,
					companySubaccountID,
					projectSubaccountID,
					projectTaskSubaccountID
				},
				new []
				{
					typeof(Location.vExpenseSubID),
					typeof(InventoryItem.cOGSSubID),
					typeof(EPEmployee.expenseSubID),
					typeof(Location.cMPExpenseSubID),
					typeof(PMProject.defaultSubID),
					typeof(PMTask.defaultSubID)
				});

			sender.RaiseFieldUpdating<APTran.subID>(documentLine, ref subaccountValue);

			e.NewValue = (int?)subaccountValue;
			e.Cancel = true;
		}

		protected virtual void APTran_LCTranID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.Cancel = true;
		}

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[APTax(typeof(APRegister), typeof(APTax), typeof(APTaxTran), typeof(APInvoice.taxCalcMode))]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void APTran_TaxCategoryID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void APTran_TaxCategoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = (APTran)e.Row;
			if (row == null) return;
			APInvoice doc = Document.Current;
			if (!this.IsCopyPasteContext && APSetup.Current.RequireControlTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>()
				&& row.Qty == 0 && row.CuryLineAmt.HasValue && row.CuryLineAmt.Value != 0 && doc.TaxCalcMode == VendorClass.taxCalcMode.Net)
			{
				PXResultset<APTax> taxes = PXSelect<APTax, Where<APTax.tranType, Equal<Required<APTax.tranType>>, And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
					And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>>>>>.Select(this, row.TranType, row.RefNbr, row.LineNbr);
				decimal curyTaxSum = 0;
				foreach (APTax tax in taxes)
				{
					curyTaxSum += tax.CuryTaxAmt.Value;
				}
				decimal? taxableAmount = TaxAttribute.CalcTaxableFromTotalAmount(sender, row, doc.TaxZoneID,
							row.TaxCategoryID, doc.DocDate.Value, row.CuryLineAmt.Value + curyTaxSum, false, GLTaxAttribute.TaxCalcLevelEnforcing.EnforceCalcOnItemAmount);
				sender.SetValueExt<APTran.curyLineAmt>(row, taxableAmount);
			}
		}

		protected virtual void APTran_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran row = (APTran)e.Row;
			if (row == null || row.InventoryID != null || vendor == null || vendor.Current == null || vendor.Current.TaxAgency == true || row.LCTranID!=null) return;

			if (TaxAttribute.GetTaxCalc<APTran.taxCategoryID>(sender, row) == TaxCalc.Calc &&
			 taxzone.Current != null &&
			 !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID))
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
				e.Cancel = true;
			}
		}

		protected virtual void APTran_UnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran row = (APTran)e.Row;
			if (row == null || row.InventoryID != null) return;
			e.NewValue = 0m;
			e.Cancel = true;
		}

		protected virtual void APTran_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran tran = (APTran)e.Row;

			if (APSetup.Current.RequireControlTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() && tran.Qty == 0)
			{
				e.NewValue = 0.0m;
				e.Cancel = true;
				return;
			}
			
			if (tran == null || tran.InventoryID == null || !string.IsNullOrEmpty(tran.PONbr))
			{
				e.NewValue = sender.GetValue<APTran.curyUnitCost>(e.Row);
				e.Cancel = e.NewValue != null;
				return;
			}
						
			APInvoice doc = this.Document.Current;
			if (doc != null && doc.VendorID != null && tran != null)
			{
				if (tran.ManualPrice != true || tran.CuryUnitCost == null)
			{
				decimal? vendorUnitCost = null;
					if (tran.InventoryID != null && tran.UOM != null)
				{
					DateTime date = Document.Current.DocDate.Value;

					vendorUnitCost = APVendorPriceMaint.CalculateUnitCost(sender, tran.VendorID, doc.VendorLocationID, tran.InventoryID, currencyinfo.Select(), tran.UOM, tran.Qty, date, tran.CuryUnitCost);
					e.NewValue = vendorUnitCost;
				}
				if (vendorUnitCost == null)
				e.NewValue = POItemCostManager.Fetch<APTran.inventoryID, APTran.curyInfoID>(sender.Graph, tran,
					doc.VendorID, doc.VendorLocationID, doc.DocDate, doc.CuryID, tran.InventoryID, null, null, tran.UOM);
				}
				else
					e.NewValue = tran.CuryUnitCost ?? 0m;
				e.Cancel = true;
			}						
		}

		protected virtual void APTran_CuryLineAmt_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran tran = (APTran)e.Row;
			if (tran == null) return;
			if (!this.IsImport && APSetup.Current.RequireControlTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
			{
				APInvoice doc = Document.Current;
				if (doc == null) return;
				decimal? newVal = 0;
				if (String.IsNullOrEmpty(tran.TaxCategoryID))
				{
					sender.SetDefaultExt<APTran.taxCategoryID>(tran);
				}
				newVal = TaxAttribute.CalcResidualAmt(sender, tran, doc.TaxZoneID, tran.TaxCategoryID, doc.DocDate.Value,
					doc.TaxCalcMode, doc.CuryOrigDocAmt.Value, doc.CuryLineTotal.Value, doc.CuryTaxTotal.Value);
				e.NewValue = newVal >= 0 ? newVal : 0;
				e.Cancel = true;
			}
		}

		protected virtual void APTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran tran = (APTran)e.Row;
			if (APSetup.Current.RequireControlTotal != true || !PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() || tran.Qty != 0)
			{
				sender.SetDefaultExt<APTran.unitCost>(tran);
				sender.SetDefaultExt<APTran.curyUnitCost>(tran);
				sender.SetValue<APTran.unitCost>(tran, null);
			}
		}

		protected virtual void APTran_Qty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row != null)
			{
				if (row.Qty == 0)
				{
					sender.SetValueExt<APTran.curyDiscAmt>(row, decimal.Zero);
					sender.SetValueExt<APTran.discPct>(row, decimal.Zero);
				}
				else
				{
					sender.SetDefaultExt<APTran.curyUnitCost>(e.Row);
				}
			}
		}

		protected virtual void APTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran tran = e.Row as APTran;
			if (tran != null)
			{
				if (String.IsNullOrEmpty(tran.ReceiptNbr) && string.IsNullOrEmpty(tran.PONbr))
				{
					sender.SetDefaultExt<APTran.accountID>(tran);
					sender.SetDefaultExt<APTran.subID>(tran);
					sender.SetDefaultExt<APTran.taxCategoryID>(tran);
					sender.SetDefaultExt<APTran.deferredCode>(tran);
					sender.SetDefaultExt<APTran.uOM>(tran);

					if (APSetup.Current.RequireControlTotal != true || !PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() || tran.Qty != 0)
					{
						if (e.ExternalCall && tran != null)
							tran.CuryUnitCost = 0m;
						sender.SetDefaultExt<APTran.unitCost>(tran);
						sender.SetDefaultExt<APTran.curyUnitCost>(tran);
						sender.SetValue<APTran.unitCost>(tran, null);
					}

					InventoryItem item = nonStockItem.Select(tran.InventoryID);

					if (item != null)
					{
						tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, "Descr", vendor.Current?.LocaleName);
					}
				}
			}
		}

		protected virtual void APTran_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row == null) return;

			if (APSetup.Current.ExpenseSubMask != null && APSetup.Current.ExpenseSubMask.Contains(APAcctSubDefault.MaskProject))
				sender.SetDefaultExt<APTran.subID>(row);

			foreach (APTran discTran in Discount_Row.Select())
			{
				SetProjectIDForDiscountTran(discTran);

				if (!PM.ProjectDefaultAttribute.IsNonProject(this, discTran.ProjectID))
				{
					SetTaskIDForDiscountTran(discTran);
				}
				else
				{
					discTran.TaskID = null;
				}
			}
		}

		protected virtual void APTran_TaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row == null) return;

			if (APSetup.Current.ExpenseSubMask != null && APSetup.Current.ExpenseSubMask.Contains(APAcctSubDefault.MaskTask))
				sender.SetDefaultExt<APTran.subID>(row);
		}

		protected virtual void APTran_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row == null) return;
			
			if (e.NewValue is Int32)
				CheckOrderTaskRule(sender, row, (int?)e.NewValue);
		}

		protected virtual void APTran_DefScheduleID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DRSchedule sc = PXSelect<DRSchedule, Where<DRSchedule.scheduleID, Equal<Required<DRSchedule.scheduleID>>>>.Select(this, ((APTran)e.Row).DefScheduleID);
			if (sc != null)
			{
				APTran defertran = PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>,
					And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
					And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>.Select(this, sc.DocType, sc.RefNbr, sc.LineNbr);

				if (defertran != null)
				{
					((APTran)e.Row).DeferredCode = defertran.DeferredCode;
				}
			}
		}

		protected virtual void APTran_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row != null && e.ExternalCall)
			{
				DiscountEngine<APTran>.UpdateManualLineDiscount<APInvoiceDiscountDetail>(sender, Transactions, row, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate);
			}
		}

		protected virtual void APTran_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			APTran row = e.Row as APTran;			
			if (row != null)
			{
				APInvoice doc = this.Document.Current;
				
				bool isPrebookedNotCompleted = (doc != null) &&  (doc.Prebooked == true && doc.Released == false && doc.Voided == false);
				bool isLCBased = (row.LCTranID != null);
				bool isPOReceiptRelated = string.IsNullOrEmpty(row.ReceiptNbr)==false;				
				bool isLCBasedTranAP = false;
				if (row.LCTranID != null)
				{
					LandedCostTran origin = null;
					foreach (LandedCostTran it in this.landedCostTrans.Select())
					{
						if (it.LCTranID == row.LCTranID)
						{
							origin = it;
							break;
						}
					}
					isLCBasedTranAP = (origin != null && (origin.Source == LandedCostTranSource.FromAP || origin.PostponeAP == true));					
				}
				bool isStockItem = false;
				if (row.InventoryID != null)
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
					isStockItem = (item != null && (item.StkItem == true));
				}
				if (row.TranType != APDocType.Prepayment && String.IsNullOrEmpty(row.ReceiptNbr) && isStockItem)
					cache.RaiseExceptionHandling<APTran.inventoryID>(row, row.InventoryID, new PXSetPropertyException(Messages.NoLinkedtoReceipt, PXErrorLevel.Warning));

				if (!isLCBasedTranAP)
				{
					if (!(String.IsNullOrEmpty(row.PONbr) && String.IsNullOrEmpty(row.ReceiptNbr)))
					{
						PXUIFieldAttribute.SetEnabled<APTran.inventoryID>(cache, row, false);
						PXUIFieldAttribute.SetEnabled<APTran.uOM>(cache, row, false);
						bool isPOPrepayment = isStockItem && row.TranType == APDocType.Prepayment;

						PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, isPOPrepayment ? true : !isStockItem);
						PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, isPOPrepayment ? true : !isStockItem);
					}
					bool allowEdit = (doc != null) && (doc.Prebooked == false && doc.Released == false && doc.Voided == false);
					PXUIFieldAttribute.SetEnabled<APTran.defScheduleID>(cache, row, allowEdit && row.TranType == APInvoiceType.DebitAdj);
					PXUIFieldAttribute.SetEnabled<APTran.deferredCode>(cache, row, allowEdit && row.DefScheduleID == null);
					if (isPrebookedNotCompleted)
					{
						PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, (!isLCBased) && (!isPOReceiptRelated));
						PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, (!isLCBased) && (!isPOReceiptRelated));
						PXUIFieldAttribute.SetEnabled<APTran.branchID>(cache, row, (!isLCBased) && (!isPOReceiptRelated));
					}

					if (doc.Released == false && doc.Prebooked == false)
					{
						bool currencyChanged = false;
						if (!String.IsNullOrEmpty(row.ReceiptNbr))
						{
							POReceipt sourceDoc =
								PXSelect<POReceipt, Where<POReceipt.receiptNbr, Equal<Required<POReceipt.receiptNbr>>>>.Select(this,
									row.ReceiptNbr);
							if (sourceDoc != null && sourceDoc.ReceiptNbr != null)
							{
								currencyChanged = (doc.CuryID != sourceDoc.CuryID);
							}
						}
						else if (!String.IsNullOrEmpty(row.PONbr))
						{
							POOrder sourceDoc =
								PXSelect
									<POOrder,
										Where
											<POOrder.orderType, Equal<Required<POOrder.orderType>>,
												And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, row.POOrderType, row.PONbr);
							if (sourceDoc != null && !string.IsNullOrEmpty(sourceDoc.OrderNbr))
							{
								currencyChanged = (doc.CuryID != sourceDoc.CuryID);
							}
						}
						if (currencyChanged)
						{
							cache.RaiseExceptionHandling<APTran.curyLineAmt>(row, row.CuryLineAmt,
								new PXSetPropertyException(AP.Messages.APDocumentCurrencyDiffersFromSourceDocument, PXErrorLevel.Warning));
							cache.RaiseExceptionHandling<APTran.curyUnitCost>(row, row.CuryUnitCost,
								new PXSetPropertyException(AP.Messages.APDocumentCurrencyDiffersFromSourceDocument, PXErrorLevel.Warning));
						}
					}

					PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, !isStockItem);
					PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, !isStockItem);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled(cache, row, false);
				}

				if (row.TranType != APDocType.Invoice && row.TranType != APDocType.Prepayment &&
					!(string.IsNullOrEmpty(row.PONbr) && string.IsNullOrEmpty(row.ReceiptNbr)))
				{
					PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, false);
					PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, false);
				}

				bool isProjectEditable = string.IsNullOrEmpty(row.PONbr);

				InventoryItem ns = (InventoryItem)PXSelectorAttribute.Select(cache, e.Row, cache.GetField(typeof (APTran.inventoryID)));
				if (ns != null && ns.StkItem != true && ns.NonStockReceipt != true)
				{
					isProjectEditable = true;
				}

				PXUIFieldAttribute.SetEnabled<APTran.projectID>(cache, row, isProjectEditable );
				PXUIFieldAttribute.SetEnabled<APTran.taskID>(cache, row, isProjectEditable);
			}
		}

		protected virtual void APTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APTran tran = e.Row as APTran;

			if (tran == null) return;

			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, tran.InventoryID);

			PXDefaultAttribute.SetPersistingCheck<APTran.accountID>(sender, tran, item != null && (item.StkItem == true) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
			PXDefaultAttribute.SetPersistingCheck<APTran.subID>(sender, tran, item != null && (item.StkItem == true) ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

			bool hasPOLinkWDeductibleVAT = false;

			if (tran.InventoryID != null)
			{
				item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
					.SelectWindowed(this, 0, 1, tran.InventoryID);

				// Inventory ID selector guarantees that stock items always have PO links.
				// -
				if (item.StkItem == true || item.NonStockReceipt == true && (!String.IsNullOrEmpty(tran.PONbr) || !String.IsNullOrEmpty(tran.ReceiptNbr)))
				{
					hasPOLinkWDeductibleVAT = PXSelectJoin<APTax, InnerJoin<Tax, On<APTax.taxID, Equal<Tax.taxID>>>,
						Where<APTax.tranType, Equal<Required<APTax.tranType>>, And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
							And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>,
							And<Tax.deductibleVAT, Equal<True>>>>>>.Select(this, tran.TranType, tran.RefNbr, tran.LineNbr).Count != 0;
				}
			}

			if (hasPOLinkWDeductibleVAT)
			{
				string prefix;
				switch (sender.GetStatus(tran))
				{
					case PXEntryStatus.Inserted:
						prefix = ErrorMessages.GetLocal(ErrorMessages.Inserting);
						break;
					default:
						prefix = ErrorMessages.GetLocal(ErrorMessages.Updating);
						break;
				}
				sender.RaiseExceptionHandling(null, tran, null,
					new PXSetPropertyException<APTran.inventoryID>(Messages.DeductibleVATNotAllowedWPOLink, PXErrorLevel.RowError));
				throw new PXException(ErrorMessages.RecordRaisedErrors, prefix, PXUIFieldAttribute.GetItemName(sender));
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || 
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				CheckOrderTaskRule(sender, tran, tran.TaskID);

				if (tran.Qty != 0 && string.IsNullOrEmpty(tran.UOM) && tran.TaskID != null && tran.InventoryID != null)
				{
					if (!sender.RaiseExceptionHandling(null, tran, null,
						new PXSetPropertyException<APTran.uOM>(Messages.NotZeroQtyRequireUOM, PXErrorLevel.Error)))
		{
						throw new PXSetPropertyException<APTran.uOM>(Messages.NotZeroQtyRequireUOM);
					}
				}
			}

			ScheduleHelper.DeleteAssociatedScheduleIfDeferralCodeChanged(this, tran);
		}

		protected virtual void APTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (e.Row == null) return;
			TaxAttribute.Calculate<APTran.taxCategoryID, APTaxAttribute>(sender, e);
		}

		protected virtual void APTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row != null)
			{
				//Validate that Expense Account <> Deferral Account:
				if (!sender.ObjectsEqual<APTran.accountID, APTran.deferredCode>(e.Row, e.OldRow))
				{
					if (!string.IsNullOrEmpty(row.DeferredCode))
					{
						DRDeferredCode defCode = PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>.Select(this, row.DeferredCode);
						if (defCode != null)
						{
							if (defCode.AccountID == row.AccountID)
							{
								sender.RaiseExceptionHandling<APTran.accountID>(e.Row, row.AccountID,
									new PXSetPropertyException(Messages.AccountIsSameAsDeferred, PXErrorLevel.Warning));
							}
						}
					}
				}
			}

			if ((!sender.ObjectsEqual<APTran.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<APTran.inventoryID>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<APTran.qty>(e.Row, e.OldRow) || !sender.ObjectsEqual<APTran.curyUnitCost>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<APTran.curyLineAmt>(e.Row, e.OldRow) || !sender.ObjectsEqual<APTran.curyDiscAmt>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<APTran.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<APTran.manualDisc>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<APTran.discountID>(e.Row, e.OldRow)) && row.LineType != SOLineType.Discount)
				RecalculateDiscounts(sender, row);

			if ((e.ExternalCall || sender.Graph.IsImport)
				&& sender.ObjectsEqual<APTran.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<APTran.uOM>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<APTran.qty>(e.Row, e.OldRow) && sender.ObjectsEqual<APTran.branchID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<APTran.siteID>(e.Row, e.OldRow)
				&& (!sender.ObjectsEqual<APTran.curyUnitCost>(e.Row, e.OldRow) || !sender.ObjectsEqual<APTran.curyLineAmt>(e.Row, e.OldRow)))
				row.ManualPrice = true;

			TaxAttribute.Calculate<APTran.taxCategoryID, APTaxAttribute>(sender, e);

			if (!sender.ObjectsEqual<APTran.box1099>(e.Row, e.OldRow))
			{
				foreach (APAdjust adj in PXSelect<APAdjust, Where<APAdjust.adjdDocType, Equal<Current<APInvoice.docType>>, And<APAdjust.adjdRefNbr, Equal<Current<APInvoice.refNbr>>, And<APAdjust.released, Equal<True>>>>>.Select(this))
				{
					APReleaseProcess.Update1099Hist(this, -1m, adj, (APTran)e.OldRow, Document.Current);
					APReleaseProcess.Update1099Hist(this, 1m, adj, (APTran)e.Row, Document.Current);
				}
			}

			//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
			if (Document.Current != null && IsExternalTax == true && Document.Current.InstallmentNbr == null &&
				!sender.ObjectsEqual<APTran.accountID, APTran.inventoryID, APTran.tranDesc, APTran.tranAmt, APTran.tranDate, APTran.taxCategoryID>(e.Row, e.OldRow))
			{
				Document.Current.IsTaxValid = false;
				Document.Update(Document.Current);
			}
			//}
		}

		protected virtual void APTran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			APTran row = (APTran)e.Row;

            if (row.LCTranID != null && row.ReceiptNbr != null && row.ReceiptType != null)
            {
                LandedCostTran lct = PXSelect<LandedCostTran,
                    Where<LandedCostTran.pOReceiptType, Equal<Current<APTran.receiptType>>,
                    And<LandedCostTran.pOReceiptNbr, Equal<Current<APTran.receiptNbr>>,
                    And<LandedCostTran.lCTranID, Equal<Current<APTran.lCTranID>>,
                    And<LandedCostTran.source, Equal<LandedCostTranSource.fromPO>>>>>>.SelectSingleBound(this, new object[] { row });

                if (lct != null)
                {
                    var lccache = this.Caches[typeof(LandedCostTran)];
                    var old = lccache.CreateCopy(lct);

                    lccache.SetValue<LandedCostTran.processed>(lct, false);
                    lccache.SetValue<LandedCostTran.postponeAP>(lct, true);
                    lccache.SetValue<LandedCostTran.aPDocType>(lct, null);
                    lccache.SetValue<LandedCostTran.aPRefNbr>(lct, null);
                    lccache.SetValue<LandedCostTran.invoiceDate>(lct, null);
                    //lccache.RaiseRowUpdated(lct, old);
                    lccache.SetStatus(lct, PXEntryStatus.Updated);
                }
            }

			if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
			{
				if (row.LineType != SOLineType.Discount)
				{
					if (PorateDiscounts() && row.ReceiptType != null && row.ReceiptNbr != null)
					{
						bool keepPODiscountLine = false;
						foreach (APTran line in Transactions.Select())
						{
							if (line.ReceiptNbr == row.ReceiptNbr && line.ReceiptNbr == row.ReceiptNbr)
							{
								ProratePODiscounts(sender, line, true);
								keepPODiscountLine = true;
							}
						}
						if (!keepPODiscountLine)
							DeletePODiscounts(row.ReceiptType, row.ReceiptNbr);
					}
					else
					DiscountEngine<APTran>.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate);
				}
				RecalculateTotalDiscount();
			}
		}

		protected virtual void APTran_Box1099_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (vendor.Current == null || vendor.Current.Vendor1099 == false)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void APTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			object existing;
			if ((existing = sender.Locate(e.Row)) != null && sender.GetStatus(existing) == PXEntryStatus.Deleted)
			{
				sender.SetValue<APTran.tranID>(e.Row, sender.GetValue<APTran.tranID>(existing));
			}
		}

		protected virtual void POReceiptLineR_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POReceiptLineR source = (POReceiptLineR)e.Row;
			if (e.Operation == PXDBOperation.Update)
			{
				if (source.VoucheredQty > source.ReceiptQty)
				{
					APTran row = Transactions.Search<APTran.receiptNbr, APTran.receiptLineNbr>(source.ReceiptNbr, source.LineNbr);

					if (row != null)
					{
						Transactions.Cache.RaiseExceptionHandling<APTran.qty>(row, row.Qty,
									new PXSetPropertyException(Messages.QuantityBilledIsGreaterThenPOReceiptQuantity, PXErrorLevel.Error));

						string prefix;
						switch (Transactions.Cache.GetStatus(row))
						{
							case PXEntryStatus.Inserted:
								prefix = ErrorMessages.GetLocal(ErrorMessages.Inserting);
								break;
							default:
								prefix = ErrorMessages.GetLocal(ErrorMessages.Updating);
								break;
						}

						throw new PXException(ErrorMessages.RecordRaisedErrors, prefix, PXUIFieldAttribute.GetItemName(Transactions.Cache));
					}
				}
			}
		}

		protected virtual void APTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row != null)
			{
				bool isLCBasedTranAP = false;
				if (row.LCTranID != null)
				{
					LandedCostTran origin = null;
					foreach (LandedCostTran it in this.landedCostTrans.Select())
					{
						if (it.LCTranID == row.LCTranID)
						{
							origin = it;
							break;
						}
					}
					isLCBasedTranAP = (origin != null && (origin.Source == LandedCostTranSource.FromAP || origin.PostponeAP == true));
				}
				if (isLCBasedTranAP && !this._isLCSync)
					e.Cancel = true;
			}
		}
		private bool _isLCSync = false;

		protected virtual void AP1099Hist_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (((AP1099Hist)e.Row).BoxNbr == null)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region CurrencyInfo Events
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
				e.NewValue = ((APInvoice)Document.Cache.Current).DocDate;
				e.Cancel = true;
			}
		}
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

		protected virtual void CurrencyInfo_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			CurrencyInfo currencyInfo = e.Row as CurrencyInfo;

			if (currencyInfo?.CuryRate == null) return;

			if (Document.Current?.ReleasedOrPrebooked != true)
			{
				Adjustments.Select().ForEach(x => RecalculateApplicationAmounts(x));
			}
		}

		#endregion

		#region APTaxTran Events
		protected virtual void APTaxTran_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null)
			{
				e.NewValue = Document.Current.TaxZoneID;
				e.Cancel = true;
			}
		}

		protected virtual void APTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			bool usesManualVAT = this.Document.Current != null && this.Document.Current.UsesManualVAT == true;
			PXUIFieldAttribute.SetEnabled<APTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted && !usesManualVAT );
		}

		protected virtual void APTaxTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			PXParentAttribute.SetParent(sender, e.Row, typeof(APRegister), this.Document.Current);
		}

		protected virtual void APTaxTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (Document.Current != null && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				((APTaxTran)e.Row).TaxZoneID = Document.Current.TaxZoneID;
			}
		}

		protected virtual void APTaxTran_TaxZoneID_ExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs e)
		{
			if (e.Exception is PXSetPropertyException)
			{
				Document.Cache.RaiseExceptionHandling<APInvoice.taxZoneID>(Document.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
				e.Cancel = false;
			}
		}

		#endregion     

		#region APAdjust Events
		protected virtual void APAdjust_CuryAdjdAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			APAdjust adj = (APAdjust)e.Row;
			Terms terms = PXSelect<Terms, Where<Terms.termsID, Equal<Current<APInvoice.termsID>>>>.Select(this);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single && (decimal)e.NewValue > 0m)
			{
				throw new PXSetPropertyException(Messages.PrepaymentAppliedToMultiplyInstallments);
			}

			if (adj.CuryDocBal == null)
			{
				PXResult<APPayment, CurrencyInfo> res = (PXResult<APPayment, CurrencyInfo>)PXSelectJoin<APPayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>>, Where<APPayment.docType, Equal<Required<APPayment.docType>>, And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);

				APPayment payment = res;
				CurrencyInfo pay_info = res;
				CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(this);

				decimal CuryDocBal = BalanceCalculation.CalculateApplicationDocumentBalance(
					sender, 
					pay_info, 
					inv_info, 
					payment.DocBal, 
					payment.CuryDocBal);
				adj.CuryDocBal = CuryDocBal - adj.CuryAdjdAmt;
			}

			if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt - (decimal)e.NewValue < 0)
			{
				throw new PXSetPropertyException(Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt).ToString());
			}
		}

		protected virtual void APAdjust_CuryAdjdAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APAdjust application = e.Row as APAdjust;

			if (application == null) return;

			RecalculateApplicationAmounts(application);

			application.CuryDocBal = application.CuryDocBal + (decimal?)e.OldValue - application.CuryAdjdAmt;
		}

		protected virtual void APAdjust_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = true;
			e.Cancel = true;
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Switch<
			Case<Where<APAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, APAdjust.adjdDocType,
			Case<Where<APAdjust.adjType, Equal<ARAdjust.adjType.adjusting>>, APAdjust.adjgDocType>>>))]
		protected virtual void APAdjust_DisplayDocType_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Switch<
			Case<Where<APAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, APAdjust.adjdRefNbr,
			Case<Where<APAdjust.adjType, Equal<ARAdjust.adjType.adjusting>>, APAdjust.adjgRefNbr>>>))]
		protected virtual void APAdjust_DisplayRefNbr_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<APAdjust.displayRefNbr, Standalone.APRegister.docDate>))]
		protected virtual void APAdjust_DisplayDocDate_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<APAdjust.displayRefNbr, Standalone.APRegister.docDesc>))]
		protected virtual void APAdjust_DisplayDocDesc_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<APAdjust.displayRefNbr, Standalone.APRegister.curyID>))]
		protected virtual void APAdjust_DisplayCuryID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(FormatPeriodID<FormatDirection.display, Selector<APAdjust.displayRefNbr, Standalone.APRegister.finPeriodID>>))]
		protected virtual void APAdjust_DisplayFinPeriodID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<APAdjust.displayRefNbr, Standalone.APRegister.status>))]
		protected virtual void APAdjust_DisplayStatus_CacheAttached(PXCache sender) { }

		#endregion

		#region Landed Cost Trans Events
		protected virtual void LandedCostTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				if (row != null && row.Source == LandedCostTranSource.FromAP && String.IsNullOrEmpty(row.LandedCostCodeID) == false)
				{
					bool hasReceipt = String.IsNullOrEmpty(row.POReceiptNbr) == false;
					if (!hasReceipt) 
					{
						foreach (LandedCostTranSplit iSplit in this.LCTranSplit.Select(row.LCTranID)) 
						{
							hasReceipt = String.IsNullOrEmpty(iSplit.POReceiptNbr) == false;
							if (hasReceipt) break; 
						}
					}
					if (!hasReceipt)
					{
						if (sender.RaiseExceptionHandling<LandedCostTran.pOReceiptNbr>(e.Row, row.POReceiptNbr, new PXSetPropertyException(Messages.APLandedCost_NoPOReceiptNumberSpecified, PXErrorLevel.RowError)))
						{
							throw new PXRowPersistingException(typeof(LandedCostTran.pOReceiptNbr).Name, row.POReceiptNbr, Messages.APLandedCost_NoPOReceiptNumberSpecified);
						}
					}						
				}
				if (row != null && !String.IsNullOrEmpty(row.LandedCostCodeID) && (row.Source == LandedCostTranSource.FromAP))
				{
					LandedCostCode code = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, row.LandedCostCodeID);
					decimal valueToCompare = decimal.Zero;
					int count = 0;

					List<POReceiptLine> allocateOn = new List<POReceiptLine>();
					List<LandedCostTranSplit> splits = new List<LandedCostTranSplit>();
					Dictionary<string, int> used = new Dictionary<string, int>();
					PXSelectBase<POReceiptLine> rctSelect = new PXSelect<POReceiptLine, Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>>>(this);
					if (String.IsNullOrEmpty(row.POReceiptNbr) == false)
					{
						used[row.POReceiptNbr] = 1;
						foreach (POReceiptLine it in rctSelect.Select(row.POReceiptNbr, row.InventoryID))
						{
							allocateOn.Add(it);							
						}
					}

					foreach (LandedCostTranSplit iSplit in this.LCTranSplit.Select(row.LCTranID))
					{
						splits.Add(iSplit);
						if(!used.ContainsKey(iSplit.POReceiptNbr))
						{
							used[iSplit.POReceiptNbr] = 1;
							foreach (POReceiptLine iLn in rctSelect.Select(iSplit.POReceiptNbr, row.InventoryID))
							{
								allocateOn.Add(iLn);							
							}
						}
					}

					foreach (POReceiptLine it in allocateOn)
					{
                        var lch = new LandedCostHelper(this, false);
                        if (lch.IsApplicable(row,splits, code, it))
						{
							valueToCompare += LandedCostHelper.GetBaseValue(code, it);
							count++;
						}						
					}

					string message = String.Empty;
					switch (code.AllocationMethod)
					{
						case LandedCostAllocationMethod.ByCost:
							message = PO.Messages.LandedCostReceiptTotalCostIsZero;
							break;
						case LandedCostAllocationMethod.ByWeight:
							message = PO.Messages.LandedCostReceiptTotalWeightIsZero;
							break;
						case LandedCostAllocationMethod.ByVolume:
							message = PO.Messages.LandedCostReceiptTotalVolumeIsZero;
							break;
						case LandedCostAllocationMethod.ByQuantity:
							message = PO.Messages.LandedCostReceiptTotalQuantityIsZero;
							break;
						case LandedCostAllocationMethod.None:
							message = PO.Messages.LandedCostReceiptNoReceiptLines;
							valueToCompare = count;
							break;
						default:
							message = PO.Messages.LandedCostUnknownAllocationType;
							break;
					}
					if (valueToCompare == Decimal.Zero)
					{
						if (sender.RaiseExceptionHandling<LandedCostTran.pOReceiptNbr>(e.Row, row.POReceiptNbr, new PXSetPropertyException(message, PXErrorLevel.RowError)))
						{
							throw new PXRowPersistingException(typeof(LandedCostTran.pOReceiptNbr).Name, row.POReceiptNbr, message);
						}
					}
					if (this.Document.Current != null)
						row.InvoiceNbr = this.Document.Current.InvoiceNbr;
				}
				if (row != null && row.Source == LandedCostTranSource.FromPO && String.IsNullOrEmpty(row.APDocType)) 
				{
					PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.aPDocType>(sender, e.Row, false);
					PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.aPRefNbr>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.curyID>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.curyInfoID>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.invoiceDate>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.invoiceDate>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.curyID>(sender, e.Row, false);
                    PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.curyInfoID>(sender, e.Row, false);
					PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.aPDocType>(sender, e.Row, false);
					PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.aPRefNbr>(sender, e.Row, false);				
				}

			}
		}

		protected virtual void LandedCostTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			
			PXUIFieldAttribute.SetEnabled<LandedCostTran.curyID>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.vendorID>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.vendorLocationID>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<LandedCostTran.pOReceiptType>(sender, null, false);
			if(row!= null)
			{
				bool hasINDoc = (row.INDocCreated == true);
				PXUIFieldAttribute.SetEnabled<LandedCostTran.landedCostCodeID>(sender, e.Row, !hasINDoc);
				PXUIFieldAttribute.SetEnabled<LandedCostTran.pOReceiptNbr>(sender, e.Row, !hasINDoc);				
				PXUIFieldAttribute.SetEnabled<LandedCostTran.inventoryID>(sender, e.Row, !hasINDoc);			
			}
		}

		protected virtual void LandedCostTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			updateTaxCalcMode();
		}

		protected virtual void LandedCostTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			if (row != null && !String.IsNullOrEmpty(row.LandedCostCodeID))
			{
				LandedCostCode code = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, row.LandedCostCodeID);
				this.AddLandedCostTran(row, code);
			}
		}

		protected virtual void LandedCostTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			LandedCostTran oldRow = (LandedCostTran)e.OldRow;
			APTran tran = null;
			foreach (APTran it in this.Transactions.Select())
			{
				if(it.LCTranID == row.LCTranID)
				{
					tran = it; break;					
				}
			}
			LandedCostCode code = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, row.LandedCostCodeID);
			if (tran == null)
			{
                foreach(APTran it in this.Transactions.Cache.Deleted)
                {
                    if (it.LCTranID == row.LCTranID && it.ReceiptNbr != null && it.ReceiptType != null && row.APRefNbr ==null && row.APDocType == null && row.Source == LandedCostTranSource.FromPO)
                    {
                        return;
                    }
                }
				this.AddLandedCostTran(row, code);
			}
			else
			{
				APTran copy = (APTran)this.Transactions.Cache.CreateCopy(tran);
				Copy(copy, row, code);
				tran = (APTran)this.Transactions.Cache.Update(copy);				
			}
		}

		protected virtual void LandedCostTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			if (row.PostponeAP == true) 
			{
				row.APDocType = null;
				row.APRefNbr = null;				
				sender.RaiseRowDeleted(e.Row);
				PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.aPDocType>(sender, e.Row, false);
				PXDBDefaultAttribute.SetDefaultForUpdate<LandedCostTran.aPRefNbr>(sender, e.Row, false);
				PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.aPDocType>(sender, e.Row, false);
				PXDBDefaultAttribute.SetDefaultForInsert<LandedCostTran.aPRefNbr>(sender, e.Row, false);
				
				this.landedCostTrans.View.RequestRefresh();
				e.Cancel = true;
			}
		}

		protected virtual void LandedCostTran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			//APTran tran = null;
			List<APTran> trans = new List<APTran>(3);
			foreach (APTran it in this.Transactions.Select())
			{
				if (it.LCTranID == row.LCTranID)
				{
					trans.Add(it);					
				}
			}
			this._isLCSync = true;
			try
			{
				foreach (APTran itr in trans)
				{
					this.Transactions.Delete(itr);
				}
			}
			finally 
			{
				this._isLCSync = false;
			}
		}

		protected virtual void LandedCostTran_POReceiptNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
            var poreceipt = PXSelectorAttribute.Select<LandedCostTran.pOReceiptNbr>(this.Caches[typeof(LandedCostTran)], row) as POReceipt;
            sender.SetValueExt<LandedCostTran.pOReceiptType>(row, poreceipt == null ? null : poreceipt.ReceiptType);
			sender.SetDefaultExt<LandedCostTran.inventoryID>(row);
			sender.SetValuePending<LandedCostTran.inventoryID>(row, null);

		}

		protected virtual void LandedCostTran_LandedCostCodeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			LandedCostTran row = (LandedCostTran)e.Row;
			sender.SetDefaultExt<LandedCostTran.descr>(e.Row);
			sender.SetValuePending<LandedCostTran.descr>(e.Row, row.Descr);
			sender.SetDefaultExt<LandedCostTran.taxCategoryID>(e.Row);
			sender.SetDefaultExt<LandedCostTran.lCAccrualAcct>(e.Row);
			sender.SetDefaultExt<LandedCostTran.lCAccrualSub>(e.Row);			
		}

		protected virtual void LandedCostTranR_RowPersisting(PXCache sender, PXRowPersistingEventArgs e) 
		{
			e.Cancel = true;
		}

		#endregion

		#region APInvoiceDiscountDetail events

		protected virtual void APInvoiceDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (Document != null && Document.Current != null)
				Document.Cache.SetValueExt<APInvoice.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<APInvoiceDiscountDetail>(DiscountDetails, true));
		}

		protected virtual void APInvoiceDiscountDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			APInvoiceDiscountDetail discountDetail = (APInvoiceDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null && discountDetail.DiscountID != null)
			{
				discountDetail.IsManual = true;

				DiscountEngine<APTran>.InsertDocGroupDiscount<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate);
				RecalculateTotalDiscount();
			}
		}

		protected virtual void APInvoiceDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			APInvoiceDiscountDetail discountDetail = (APInvoiceDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null)
			{
				if (!sender.ObjectsEqual<APInvoiceDiscountDetail.skipDiscount>(e.Row, e.OldRow))
				{
				DiscountEngine<APTran>.UpdateDocumentDiscount<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate, discountDetail.Type != DiscountType.Document);
				RecalculateTotalDiscount();
			}
				if (!sender.ObjectsEqual<APInvoiceDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<APInvoiceDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
			{
					discountDetail.IsManual = true;
					DiscountEngine<APTran>.InsertDocGroupDiscount<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<APInvoiceDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate);
				RecalculateTotalDiscount();
			}
		}
		}

		protected virtual void APInvoiceDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			APInvoiceDiscountDetail discountDetail = (APInvoiceDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null)
			{
				DiscountEngine<APTran>.UpdateDocumentDiscount<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.DocDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document));
			}
			RecalculateTotalDiscount();
		}
		#endregion

		#region POReceipt & POOrder Events
		protected virtual void POReceipt_CuryID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POReceipt row = (POReceipt)e.Row;
			APInvoice doc = this.Document.Current;
			if (row != null && doc != null)
			{
				if (row.CuryID != doc.CuryID)
				{
					string fieldName = typeof(POReceipt.curyID).Name;
					PXErrorLevel msgLevel = PXErrorLevel.RowWarning;
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
					 null, null, Messages.APDocumentCurrencyDiffersFromSourceDocument, msgLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
					e.IsAltered = true;
				}
			}
		}

		protected virtual void POReceiptLineS_ReceiptNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POReceiptLineS row = (POReceiptLineS)e.Row;
			APInvoice doc = this.Document.Current;
			if (row != null && doc != null)
			{
				POReceipt receipt = PXSelect<POReceipt, Where<POReceipt.receiptNbr,Equal<Required<POReceipt.receiptNbr>>>>.Select(this, row.ReceiptNbr);
				if (receipt.CuryID != doc.CuryID)
				{
					string fieldName = typeof(POReceipt.curyID).Name;
					PXErrorLevel msgLevel = PXErrorLevel.RowWarning;
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
					 null, null, Messages.APDocumentCurrencyDiffersFromSourceDocument, msgLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
					e.IsAltered = true;
				}
			}
		}

		protected virtual void POOrderRS_CuryID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POOrderRS row = (POOrderRS)e.Row;
			APInvoice doc = this.Document.Current;
			if (row != null && doc != null)
			{
				if (row.CuryID != doc.CuryID)
				{
					string fieldName = typeof(POOrderRS.curyID).Name;
					PXErrorLevel msgLevel = PXErrorLevel.RowWarning;
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
					 null, null, Messages.APDocumentCurrencyDiffersFromSourceDocument, msgLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
					e.IsAltered = true;
				}
			}
		}
		#endregion

		#region POReceiptFilter events
#if false
		protected virtual void POReceiptFilter_OrderNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			POReceiptFilter row = (POReceiptFilter)e.Row;
			APInvoice doc = this.Document.Current;
			if (row != null && doc != null)
			{
				bool doSet = false;
				if (!String.IsNullOrEmpty(row.OrderNbr))
				{

					POOrder order = PXSelectJoin<POOrder, 
												InnerJoin<POReceiptLineS,On<POOrder.orderType,Equal<POReceiptLineS.pOType>, And<POOrder.orderNbr,Equal<POReceiptLineS.pONbr>>>>,
												Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>,  
													And<Where<POReceiptLineS.receiptType, Equal<POReceiptType.poreceipt>, And<Optional<APInvoice.docType>, Equal<APInvoiceType.invoice>,
														Or<POReceiptLineS.receiptType, Equal<POReceiptType.poreturn>, And<Optional<APInvoice.docType>, Equal<APInvoiceType.debitAdj>>>>>>>>.Select(this, row.OrderNbr,doc.DocType, doc.DocType);
					if (order != null && order.CuryID != doc.CuryID)
					{
						doSet = true;
					}
				}
				if (doSet)
				{
					string fieldName = typeof(POReceiptFilter.orderNbr).Name;
					PXErrorLevel msgLevel = PXErrorLevel.Warning;
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
					 null, null, Messages.APDocumentCurrencyDiffersFromSourceDocument, msgLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
					e.IsAltered = true;
				}
			}
		} 
#endif

		protected virtual void APInvoiceDiscountDetail_DiscountSequenceID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}
		protected virtual void APInvoiceDiscountDetail_DiscountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}
		#endregion

		#region LinkLine events

		protected virtual void LinkLineFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			LinkLineFilter row = (LinkLineFilter)e.Row;
			if (row != null)
			{
				PXCache orderReceiptCache = linkLineReceiptTran.Cache;
				bool isReceiptMode = row.SelectedMode == LinkLineFilter.selectedMode.Receipt;

				linkLineReceiptTran.View.AllowSelect = isReceiptMode;
				linkLineOrderTran.View.AllowSelect = !isReceiptMode;
			}

		}


		protected virtual void POLineS_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLineS row = (POLineS)e.Row;
			if (row != null && !(bool) e.OldValue && (bool) row.Selected)
			{
				foreach (POLineS item in sender.Updated)
				{
					if (item.Selected == true && item != row)
					{
						sender.SetValue<POLineS.selected>(item, false);
						linkLineOrderTran.View.RequestRefresh();
					}

				}
				foreach (POReceiptLineS item in linkLineReceiptTran.Cache.Updated)
				{
					if (item.Selected == true)
					{
						linkLineReceiptTran.Cache.SetValue<POReceiptLineS.selected>(item, false);
						linkLineReceiptTran.View.RequestRefresh();
					}
				}
				
			}
		}

		protected virtual void POLineS_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			sender.IsDirty = false;
		}

		protected virtual void POLineS_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void POReceiptLineS_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POReceiptLineS row = (POReceiptLineS)e.Row;
			if (row != null && !(bool)e.OldValue && (bool)row.Selected)
			{
				foreach (POReceiptLineS item in linkLineReceiptTran.Cache.Updated)
				{
					if (item.Selected == true && item != row)
					{
						sender.SetValue<POReceiptLineS.selected>(item, false);
						linkLineReceiptTran.View.RequestRefresh();
					}
				}
				foreach (POLineS item in linkLineOrderTran.Cache.Updated)
				{
					if (item.Selected == true)
					{
						linkLineOrderTran.Cache.SetValue<POLineS.selected>(item, false);
						linkLineOrderTran.View.RequestRefresh();
					}
				}

			}
		}

		protected virtual void POReceiptLineS_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			sender.IsDirty = false;
		}

		protected virtual void POReceiptLineS_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion

		#region Select Overrides

		public virtual IEnumerable pOreceiptslist()
		{
			APInvoice doc = this.Document.Current;
			if (doc == null || doc.VendorID == null || doc.VendorLocationID == null) yield break;
			if (doc.DocType != APDocType.Invoice && doc.DocType != APDocType.DebitAdj)
				yield break;

			string poReceiptType = (doc.DocType == APDocType.Invoice) ? POReceiptType.POReceipt : POReceiptType.POReturn;

			Dictionary<APTran, int> usedReceipt = new Dictionary<APTran, int>(new POReceiptComparer());

			int count;
			foreach (APTran aPTran in Transactions.Cache.Inserted)
			{
				if (aPTran.ReceiptNbr != null)
				{
					usedReceipt.TryGetValue(aPTran, out count);
					usedReceipt[aPTran] = count + 1;
				}
			}

			foreach (APTran aPTran in Transactions.Cache.Deleted)
			{
				if (aPTran.ReceiptNbr != null && Transactions.Cache.GetStatus(aPTran) != PXEntryStatus.InsertedDeleted)
				{
					usedReceipt.TryGetValue(aPTran, out count);
					usedReceipt[aPTran] = count - 1;
				}
			}

			foreach (APTran aPTran in Transactions.Cache.Updated)
			{
				String originalValue = (String)Transactions.Cache.GetValueOriginal<APTran.receiptNbr>(aPTran);
				if (aPTran.ReceiptNbr != originalValue)
				{
					if (originalValue != null)
					{
						APTran originTran = new APTran();
						originTran.ReceiptNbr = originalValue;
						usedReceipt.TryGetValue(originTran, out count);
						usedReceipt[originTran] = count - 1;
					}
					if (aPTran.ReceiptNbr != null)
					{
						usedReceipt.TryGetValue(aPTran, out count);
						usedReceipt[aPTran] = count + 1;
					}
				}
			}

			foreach (PXResult<POReceipt, POReceiptLineS, APTran> order in PXSelectJoinGroupBy<POReceipt,
				InnerJoin<POReceiptLineS, On<POReceiptLineS.receiptNbr, Equal<POReceipt.receiptNbr>>,
				LeftJoin<APTran, On<APTran.receiptNbr, Equal<POReceiptLineS.receiptNbr>,
					And<APTran.receiptLineNbr, Equal<POReceiptLineS.lineNbr>,
					And<APTran.released, Equal<False>>>>>>,
				Where<POReceipt.vendorID, Equal<Required<APInvoice.vendorID>>,
					 And<POReceipt.vendorLocationID, Equal<Required<APInvoice.vendorLocationID>>,
					 And<POReceipt.hold, Equal<False>,
					 And<POReceipt.released, Equal<True>,
					 And<POReceipt.receiptType, Equal<Required<POReceipt.receiptType>>,
					 And<POReceipt.curyID, Equal<Required<APInvoice.curyID>>,
					 And<APTran.refNbr, IsNull,
					 And2<Where<POReceiptLineS.unbilledQty, Greater<decimal0>,
						 Or<POReceiptLineS.curyUnbilledAmt, NotEqual<decimal0>>>,
					 And<Where<POReceiptLineS.pONbr, Equal<Current<POReceiptFilter.orderNbr>>, Or<Current<POReceiptFilter.orderNbr>, IsNull>>>
					 >>>>>>>>,
				Aggregate<GroupBy<POReceipt.receiptNbr, Sum<POReceiptLineS.receiptQty, Sum<POReceiptLineS.unbilledQty, Sum<POReceiptLineS.curyUnbilledAmt, GroupBy<POReceipt.receiptType, Count<POReceiptLineS.lineNbr>>>>>>>>.Select(this, doc.VendorID, doc.VendorLocationID, poReceiptType, doc.CuryID))
			{
				APTran aPTran = new APTran();
				aPTran.ReceiptNbr = ((POReceipt) order).ReceiptNbr;
				if (usedReceipt.TryGetValue(aPTran, out count))
				{
					usedReceipt.Remove(aPTran);
					if (count < order.RowCount)
						yield return (POReceipt)order;
				}
				else
					yield return (POReceipt)order;
			}
			foreach (APTran deletedTran in usedReceipt.Where(_=> _.Value < 0).Select(_=> _.Key))
			{
				yield return (POReceipt)PXSelect<POReceipt, Where<POReceipt.receiptNbr, Equal<Required<APTran.receiptNbr>>>>.Select(this, deletedTran.ReceiptNbr);
			}
		}

		public virtual IEnumerable pOOrderslist()
		{
			APInvoice doc = this.Document.Current;
			if (doc == null || doc.VendorID == null || doc.VendorLocationID == null) yield break;
			if (doc.DocType != APDocType.Invoice && doc.DocType != APDocType.DebitAdj)
				yield break;

			Dictionary<APTran, int> usedOrder = new Dictionary<APTran, int>(new POOrderComparer());

			int count;
			foreach (APTran aPTran in Transactions.Cache.Inserted)
			{
				if (aPTran.PONbr != null && aPTran.POOrderType != null)
				{
					usedOrder.TryGetValue(aPTran, out count);
					usedOrder[aPTran] = count + 1;
				}
			}

			foreach (APTran aPTran in Transactions.Cache.Deleted)
			{
				if (aPTran.PONbr != null && aPTran.POOrderType != null && Transactions.Cache.GetStatus(aPTran) != PXEntryStatus.InsertedDeleted)
				{
					usedOrder.TryGetValue(aPTran, out count);
					usedOrder[aPTran] = count - 1;
				}
			}

			foreach (APTran aPTran in Transactions.Cache.Updated)
			{
				String originalNbr = (String)Transactions.Cache.GetValueOriginal<APTran.pONbr>(aPTran);
				String originalType = (String)Transactions.Cache.GetValueOriginal<APTran.pOOrderType>(aPTran);
				if (aPTran.PONbr != originalNbr || aPTran.POOrderType != originalType)
				{
					if (originalNbr != null && originalType != null)
					{
						APTran originTran = new APTran();
						originTran.PONbr = originalNbr;
						originTran.POOrderType = originalType;
						usedOrder.TryGetValue(originTran, out count);
						usedOrder[originTran] = count - 1;
					}
					if (aPTran.PONbr != null)
					{
						usedOrder.TryGetValue(aPTran, out count);
						usedOrder[aPTran] = count + 1;
					}
				}
			}

			foreach (PXResult<POOrder, POLine> order in PXSelectJoinGroupBy<POOrder,
				InnerJoin<POLine, On<POLine.orderType, Equal<POOrder.orderType>,
					And<POLine.orderNbr, Equal<POOrder.orderNbr>,
					And<POLine.lineType, Equal<POLineType.service>,
					And<POLine.cancelled, NotEqual<True>,
					And<POLine.completed, NotEqual<True>
					>>>>>,
					LeftJoin<APTran, On<APTran.pOOrderType, Equal<POLine.orderType>,
												And<APTran.pONbr, Equal<POLine.orderNbr>,
												And<APTran.pOLineNbr, Equal<POLine.lineNbr>,
												And<APTran.released, Equal<False>>>>>>>,
					Where<APTran.refNbr, IsNull, And<POOrder.vendorID, Equal<Required<APInvoice.vendorID>>, 
					And<POOrder.vendorLocationID, Equal<Required<APInvoice.vendorLocationID>>, 
					And<POOrder.curyID, Equal<Required<APInvoice.curyID>>,
					And<POOrder.orderType, NotEqual<POOrderType.dropShip>,
					And<POOrder.status, Equal<POOrderStatus.open>>>>>>>,
					Aggregate
						<GroupBy<POOrder.orderType,
						GroupBy<POOrder.orderNbr,
						GroupBy<POOrder.orderDate,
						GroupBy<POOrder.curyID,
						GroupBy<POOrder.curyOrderTotal,
						GroupBy<POOrder.hold,
						GroupBy<POOrder.receipt,
						GroupBy<POOrder.cancelled,
						Sum<POLine.orderQty,
						Sum<POLine.voucheredQty,
						Sum<POLine.curyVoucheredCost,
						Sum<POLine.voucheredCost,
						Sum<POLine.curyExtCost,
						Sum<POLine.extCost,
						Count<POLine.lineNbr>>>>>>>>>>>>>>>>>.Select(this, doc.VendorID, doc.VendorLocationID, doc.CuryID))
			{
				APTran aPTran = new APTran();
				aPTran.PONbr = ((POOrder)order).OrderNbr;
				aPTran.POOrderType = ((POOrder)order).OrderType;
				if (usedOrder.TryGetValue(aPTran, out count))
				{
					usedOrder.Remove(aPTran);
					if (count < order.RowCount)
						yield return (POOrder)order;
				}
				else
					yield return (POOrder)order;
			}
			foreach (APTran deletedTran in usedOrder.Where(_ => _.Value < 0).Select(_ => _.Key))
			{
				yield return (POOrder)PXSelect<POOrder, Where<POOrder.orderNbr, Equal<Required<APTran.pONbr>>, And<POOrder.orderType, Equal<Required<APTran.pOOrderType>>>>>.Select(this, deletedTran.PONbr, deletedTran.POOrderType);
			}
		
		}

		public virtual IEnumerable POReceiptLinesSelection()
		{
			APInvoice doc = this.Document.Current;
			if (doc == null || doc.VendorID == null || doc.VendorLocationID == null) yield break;
            if (doc.DocType != APDocType.Invoice && doc.DocType != APDocType.DebitAdj)
                yield break;

            string poReceiptType = (doc.DocType == APDocType.Invoice) ? POReceiptType.POReceipt : POReceiptType.POReturn;

			HashSet<APTran> usedRecceiptLine = new HashSet<APTran>(new POReceiptLineComparer());
			HashSet<APTran> unusedReceiptLine = new HashSet<APTran>(new POReceiptLineComparer());

			foreach (APTran aPTran in Transactions.Cache.Inserted)
			{
				if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null)
					usedRecceiptLine.Add(aPTran);
			}

			foreach (APTran aPTran in Transactions.Cache.Deleted)
			{
				if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null && Transactions.Cache.GetStatus(aPTran) != PXEntryStatus.InsertedDeleted)
					if (!usedRecceiptLine.Remove(aPTran))
						unusedReceiptLine.Add(aPTran);
			}

			foreach (APTran aPTran in Transactions.Cache.Updated)
			{
				APTran originAPTran = new APTran();
				originAPTran.ReceiptNbr = (String)Transactions.Cache.GetValueOriginal<APTran.receiptNbr>(aPTran);
				originAPTran.ReceiptType = (String)Transactions.Cache.GetValueOriginal<APTran.receiptType>(aPTran);
				originAPTran.ReceiptLineNbr = (Int32?)Transactions.Cache.GetValueOriginal<APTran.receiptLineNbr>(aPTran);

				if (originAPTran.ReceiptNbr != null && originAPTran.ReceiptType != null && originAPTran.ReceiptLineNbr != null)
				{
					if (!usedRecceiptLine.Remove(originAPTran))
						unusedReceiptLine.Add(originAPTran);
				}

				if (aPTran.ReceiptNbr != null && aPTran.ReceiptType != null && aPTran.ReceiptLineNbr != null)
				{
					if (!unusedReceiptLine.Remove(aPTran))
						usedRecceiptLine.Add(aPTran);
				}
			}

			foreach (LinkLineReceipt item in PXSelect<LinkLineReceipt, 
				Where<LinkLineReceipt.vendorID, Equal<Current<APInvoice.vendorID>>, 
				And<LinkLineReceipt.vendorLocationID, Equal<Current<APInvoice.vendorLocationID>>,
				And<LinkLineReceipt.receiptCuryID, Equal<Current<APInvoice.curyID>>,
                And<LinkLineReceipt.receiptType, Equal<Required<POReceipt.receiptType>>,
				And<Where<LinkLineReceipt.orderNbr, Equal<Current<POReceiptFilter.orderNbr>>, Or<Current<POReceiptFilter.orderNbr>, IsNull>>>
				>>>>>.SelectMultiBound(this, new object[] { doc }, poReceiptType))
			{
				APTran aPTran = new APTran();
				aPTran.ReceiptType = item.ReceiptType;
				aPTran.ReceiptNbr = item.ReceiptNbr;
				aPTran.ReceiptLineNbr = item.ReceiptLineNbr;
				if (!usedRecceiptLine.Contains(aPTran))
					yield return (PXResult<POReceiptLineAdd, POReceipt>)ReceipLineAdd.Select(item.ReceiptNbr, item.ReceiptType, item.ReceiptLineNbr);
			}

			foreach (APTran item in unusedReceiptLine)
			{
				yield return (PXResult<POReceiptLineAdd, POReceipt>)ReceipLineAdd.Select(item.ReceiptNbr, item.ReceiptType, item.ReceiptLineNbr);
			}

		}


		#endregion

		#region Source-Specific  functions

		public class POReceiptLineComparer : IEqualityComparer<APTran>
		{
			public POReceiptLineComparer()
			{ 
			}

			#region IEqualityComparer<APTran> Members

			public bool Equals(APTran x, APTran y)
			{
				return x.ReceiptNbr == y.ReceiptNbr && x.ReceiptLineNbr == y.ReceiptLineNbr;
			}

			public int GetHashCode(APTran obj)
			{
				return obj.ReceiptNbr.GetHashCode() + 109 * obj.ReceiptLineNbr.GetHashCode();
			}

			#endregion
		}

		public class POLineComparer : IEqualityComparer<APTran>
		{
			public POLineComparer()
			{
			}

			#region IEqualityComparer<APTran> Members

			public bool Equals(APTran x, APTran y)
			{
				return x.POOrderType == y.POOrderType && x.PONbr == y.PONbr && x.POLineNbr == y.POLineNbr;
			}

			public int GetHashCode(APTran obj)
			{
				return obj.POOrderType.GetHashCode() + 109 * obj.PONbr.GetHashCode() + 37 * obj.POLineNbr.GetHashCode();
			}

			#endregion
		}

		public class POReceiptComparer : IEqualityComparer<APTran>
		{
			public POReceiptComparer()
			{
			}

			#region IEqualityComparer<APTran> Members

			public bool Equals(APTran x, APTran y)
			{
				return x.ReceiptNbr == y.ReceiptNbr;
			}

			public int GetHashCode(APTran obj)
			{
				return obj.ReceiptNbr.GetHashCode();
			}

			#endregion
		}

		public class POOrderComparer : IEqualityComparer<APTran>
		{
			public POOrderComparer()
			{
			}

			#region IEqualityComparer<APTran> Members

			public bool Equals(APTran x, APTran y)
			{
				return x.POOrderType == y.POOrderType && x.PONbr == y.PONbr;
			}

			public int GetHashCode(APTran obj)
			{
				return obj.POOrderType.GetHashCode() + 109 * obj.PONbr.GetHashCode();
			}

			#endregion
		}

		public virtual void InvoicePOReceipt(POReceipt order, CurrencyInfo aCuryInfo, DocumentList<APInvoice> list, bool saveAndAdd, bool keepReceiptTaxes)
		{
			APInvoice newdoc;

			if (list != null)
			{
				bool billSeparately = false;
				if (billSeparately == false)
				{
					newdoc = list.Find<APInvoice.docType, APInvoice.branchID, APInvoice.vendorID, APInvoice.vendorLocationID, APInvoice.curyID, APInvoice.termsID, APInvoice.invoiceDate>(((POReceipt)order).GetAPDocType(), ((POReceipt)order).BranchID, ((POReceipt)order).VendorID, ((POReceipt)order).VendorLocationID, ((POReceipt)order).CuryID, ((POReceipt)order).TermsID, ((POReceipt)order).ReceiptDate, false) ?? new APInvoice();
				}
				else
				{
					newdoc = new APInvoice();					
				}

				if (newdoc.RefNbr != null)
				{
					this.Document.Current = this.Document.Search<APInvoice.refNbr>(newdoc.RefNbr, newdoc.DocType);
				}
				else
				{
					newdoc.DocType = ((POReceipt)order).GetAPDocType();
					newdoc.DocDate = order.InvoiceDate;
					if (DateTime.Compare((DateTime)order.ReceiptDate, (DateTime)order.InvoiceDate) == 0)
					{
						newdoc.FinPeriodID = order.FinPeriodID;
					}
					newdoc = PXCache<APInvoice>.CreateCopy(this.Document.Insert(newdoc));
					newdoc.BranchID = order.BranchID;
					newdoc.VendorID = ((POReceipt)order).VendorID;
					newdoc.VendorLocationID = ((POReceipt)order).VendorLocationID;
                    if (order.AutoCreateInvoice ?? false)
                    {
                        newdoc.RefNoteID = order.NoteID;
                    }

					newdoc.TermsID = ((POReceipt)order).TermsID;
					newdoc.InvoiceNbr = order.InvoiceNbr;
					newdoc.DueDate = order.DueDate;
					newdoc.DiscDate = order.DiscDate;
					newdoc.CuryOrigDiscAmt = order.CuryDiscAmt;
					newdoc.TaxZoneID = order.TaxZoneID;
					newdoc.CuryOrigDocAmt = order.CuryControlTotal;
					if (aCuryInfo == null)
						newdoc.CuryID = order.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
					if (aCuryInfo != null)
					{
						foreach (CurrencyInfo info in this.currencyinfo.Select())
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, aCuryInfo);
							info.CuryInfoID = newdoc.CuryInfoID;
							newdoc.CuryID = info.CuryID;
						}
					}
				}
			}
			else
			{
				newdoc = PXCache<APInvoice>.CreateCopy(Document.Current);
				if (newdoc.VendorID == null && newdoc.VendorLocationID == null)
				{
					newdoc.VendorID = ((POReceipt)order).VendorID;
					newdoc.VendorLocationID = ((POReceipt)order).VendorLocationID;

					newdoc.DocDate = order.InvoiceDate;
					if (String.IsNullOrEmpty(newdoc.InvoiceNbr))
					{
						newdoc.InvoiceNbr = order.InvoiceNbr;
					}
					if (String.IsNullOrEmpty(newdoc.TermsID))
						newdoc.TermsID = ((POReceipt)order).TermsID;
					newdoc.DueDate = order.DueDate;
					newdoc.DiscDate = order.DiscDate;
					newdoc.TaxZoneID = order.TaxZoneID;
					newdoc.CuryOrigDocAmt = order.CuryControlTotal;
					//newdoc.CuryOrigDiscAmt = order.CuryDiscAmt;					
					if (aCuryInfo == null)
						newdoc.CuryID = order.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
				}
				else
				{
					if (newdoc.VendorID != ((POReceipt)order).VendorID || newdoc.VendorID != ((POReceipt)order).VendorID)
						throw new PXException("PO Receipt {0} belongs to another vendor", ((POReceipt)order).ReceiptNbr);
				}
				if (aCuryInfo != null)
				{
					foreach (CurrencyInfo info in this.currencyinfo.Select())
					{
						PXCache<CurrencyInfo>.RestoreCopy(info, aCuryInfo);
						info.CuryInfoID = newdoc.CuryInfoID;
						newdoc.CuryID = info.CuryID;
					}
				}
			}
			newdoc = this.Document.Current;
			if (String.IsNullOrEmpty(newdoc.InvoiceNbr))
			{
				newdoc.InvoiceNbr = ((POReceipt)order).InvoiceNbr;
			}

			if (keepReceiptTaxes)
			{
				//This is required to transfer taxes from the original document(possibly modified there) instead of counting them by default rules
				TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(this.Transactions.Cache, null, TaxCalc.ManualCalc);
			}

			HashSet<APTran> duplicates = new HashSet<APTran>(new POReceiptLineComparer());
			foreach (APTran tran in this.Transactions.Select())
			{
				try
				{
					duplicates.Add(tran);
				}
				catch (NullReferenceException) { }
			}

			object dummy_polineap = new POLineAP();

			foreach (PXResult<POReceiptLineR, POReceiptLineS> res in
								PXSelectJoin<POReceiptLineR,
											InnerJoin<POReceiptLineS, On<POReceiptLineS.receiptNbr, Equal<POReceiptLineR.receiptNbr>, And<POReceiptLineS.lineNbr, Equal<POReceiptLineR.lineNbr>>>,
											LeftJoin<APTran, On<APTran.receiptNbr, Equal<POReceiptLineS.receiptNbr>,
												And<APTran.receiptLineNbr, Equal<POReceiptLineS.lineNbr>,
													And<APTran.released, Equal<False>>>>>>,
								Where<POReceiptLineS.receiptNbr, Equal<Required<POReceiptLineS.receiptNbr>>,
										And2<Where<APTran.refNbr, IsNull, Or<APTran.refNbr, Equal<Required<APTran.refNbr>>, And<APTran.tranType, Equal<Required<APTran.tranType>>>>>,
										And<Where<POReceiptLineS.unbilledQty, Greater<decimal0>,
											Or<POReceiptLineS.curyUnbilledAmt, NotEqual<decimal0>>
											>>>>>.Select(this, ((POReceipt)order).ReceiptNbr, newdoc.RefNbr, newdoc.DocType))
			{
				POReceiptLineS orderline = res;
				POReceiptLineR aggregate = res;

				PXRowInserting handler = new PXRowInserting((sender, e) =>
				{ 
					PXParentAttribute.SetParent(sender, e.Row, typeof(APRegister), Document.Current);
					//PXParentAttribute.SetParent(sender, e.Row, typeof(POLineAP), dummy_polineap);
					PXParentAttribute.SetParent(sender, e.Row, typeof(POReceiptLineR), aggregate);
				});

				this.RowInserting.AddHandler<APTran>(handler);

				APTran tran = AddPOReceiptLine(orderline, duplicates);

				this.RowInserting.RemoveHandler<APTran>(handler);

			}

			foreach (POReceiptTaxTran tax in PXSelect<POReceiptTaxTran, Where<POReceiptTaxTran.receiptNbr, Equal<Required<POReceiptTaxTran.receiptNbr>>>>.Select(this, ((POReceipt)order).ReceiptNbr))
			{
				APTaxTran newtax = new APTaxTran();
				newtax.TaxID = tax.TaxID;
				newtax.TaxRate = 0m;
				newtax = this.Taxes.Insert(newtax);
			}
			newdoc.CuryTaxAmt = order.CuryTaxTotal;
			newdoc.CuryOrigDiscAmt = order.CuryDiscAmt;
			if (list != null && saveAndAdd)
			{

				this.Save.Press();

				if (list.Find(this.Document.Current) == null)
				{
					list.Add(this.Document.Current);
				}
			}
		}
		public virtual void AttachPrepayment()
		{
			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(this);
			if (Document.Current != null && (Document.Current.DocType == APDocType.Invoice || Document.Current.DocType == APDocType.CreditAdj) && Document.Current.Released == false && Document.Current.Prebooked == false)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Adjustments.Cache, Document.Cache))
				{
					foreach (PXResult<APPayment, CurrencyInfo, APAdjust> res in
						PXSelectJoin<APPayment,
						InnerJoin<CurrencyInfo,
							On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>,
						LeftJoin<APAdjust,
							On<APAdjust.adjgDocType, Equal<APPayment.docType>,
							And<APAdjust.adjgRefNbr, Equal<APPayment.refNbr>,
							And<APAdjust.adjNbr, Equal<APPayment.lineCntr>,
							And<APAdjust.released, Equal<False>,
							And<Where<
								APAdjust.adjdDocType, NotEqual<Current<APInvoice.docType>>,
								Or<APAdjust.adjdRefNbr, NotEqual<Current<APInvoice.refNbr>>>>>>>>>>>,
						Where<
							APPayment.vendorID, Equal<Current<APInvoice.vendorID>>,
							And<APPayment.docType, Equal<APDocType.prepayment>,
							And<APPayment.docDate, LessEqual<Current<APInvoice.docDate>>,
							And<APPayment.finPeriodID, LessEqual<Current<APInvoice.finPeriodID>>,
							And<APPayment.released, Equal<True>,
							And<APPayment.openDoc, Equal<True>,
							And<APAdjust.adjdRefNbr, IsNull>>>>>>>>.Select(this))
					{
						APPayment payment = (APPayment)res;
						APAdjust adj = new APAdjust();
						CurrencyInfo pay_info = (CurrencyInfo)res;

						adj.VendorID = Document.Current.VendorID;
						adj.AdjdDocType = Document.Current.DocType;
						adj.AdjdRefNbr = Document.Current.RefNbr;
						adj.AdjdBranchID = Document.Current.BranchID;
						adj.AdjgDocType = payment.DocType;
						adj.AdjgRefNbr = payment.RefNbr;
						adj.AdjgBranchID = payment.BranchID;
						adj.AdjNbr = payment.LineCntr;

						POOrder order = PXSelect<POOrder, Where<POOrder.prepaymentDocType, Equal<Required<APPayment.docType>>, And<POOrder.prepaymentRefNbr, Equal<Required<APPayment.refNbr>>>>>.Select(this, payment.DocType, payment.RefNbr);
						if (order != null)
						{
							APAdjust existing;
							if ((existing = Adjustments.Locate(adj)) == null)
							{
								adj.AdjgCuryInfoID = payment.CuryInfoID;
								adj.AdjdOrigCuryInfoID = Document.Current.CuryInfoID;
								adj.AdjdCuryInfoID = Document.Current.CuryInfoID;
								adj.CuryDocBal = BalanceCalculation.CalculateApplicationDocumentBalance(
									Adjustments.Cache, 
									pay_info, 
									inv_info, 
									payment.DocBal, 
									payment.CuryDocBal); ;

								existing = Adjustments.Insert(adj);
							}

							decimal? summPOOrderAmt = 0m;
							PXResultset<APTran> result = Transactions.Select();
							result.Where( a => ((APTran)a).POOrderType == order.OrderType && ((APTran)a).PONbr == order.OrderNbr)
								.ForEach(a => { summPOOrderAmt += ((APTran)a).CuryTranAmt; });

							existing.CuryAdjdAmt = (existing.CuryDocBal <= summPOOrderAmt ? existing.CuryDocBal : summPOOrderAmt);
							Adjustments.Update(existing);

							break;
						}
					}
				}
			}
		}
		public virtual void InvoicePOOrder(POOrder order, CurrencyInfo aCuryInfo, DocumentList<APInvoice> list)
		{
			APInvoice newdoc;

			if (list != null)
			{
				bool billSeparately = false;
				if (billSeparately == false)
				{
					newdoc = list.Find<APInvoice.docType, APInvoice.branchID, APInvoice.vendorID, APInvoice.vendorLocationID, APInvoice.curyID, APInvoice.termsID, APInvoice.invoiceDate>(APDocType.Invoice, order.BranchID, order.VendorID, order.VendorLocationID, order.CuryID, order.TermsID, order.OrderDate, false) ?? new APInvoice();
				}
				else
				{
					newdoc = new APInvoice();
				}

				if (newdoc.RefNbr != null)
				{
					this.Document.Current = this.Document.Search<APInvoice.refNbr>(newdoc.RefNbr, newdoc.DocType);
				}
				else
				{
					newdoc.DocType = APDocType.Invoice;
					newdoc.DocDate = order.OrderDate;
					newdoc.BranchID = order.BranchID;
					newdoc = PXCache<APInvoice>.CreateCopy(this.Document.Insert(newdoc));

					newdoc.VendorID = order.VendorID;
					newdoc.VendorLocationID = order.VendorLocationID;

					newdoc.TermsID = order.TermsID;
					newdoc.InvoiceNbr = order.VendorRefNbr;

					if (aCuryInfo == null)
						newdoc.CuryID = order.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
					if (aCuryInfo != null)
					{
						foreach (CurrencyInfo info in this.currencyinfo.Select())
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, aCuryInfo);
							info.CuryInfoID = newdoc.CuryInfoID;
							newdoc.CuryID = info.CuryID;
						}
					}
				}
			}
			else
			{
				newdoc = PXCache<APInvoice>.CreateCopy(Document.Current);
				if (!newdoc.VendorID.HasValue || !newdoc.VendorLocationID.HasValue)
				{
					newdoc.VendorID = order.VendorID;
					newdoc.VendorLocationID = order.VendorLocationID;
					if (String.IsNullOrEmpty(newdoc.TermsID))
						newdoc.TermsID = order.TermsID;
					if (string.IsNullOrEmpty(newdoc.DocDesc))
						newdoc.DocDesc = order.OrderDesc;
					if (aCuryInfo == null)
						newdoc.CuryID = order.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
					if (aCuryInfo != null)
					{
						foreach (CurrencyInfo info in this.currencyinfo.Select())
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, aCuryInfo);
							info.CuryInfoID = newdoc.CuryInfoID;
							newdoc.CuryID = info.CuryID;
						}
					}
				}
			}
			newdoc = this.Document.Current;
			if (String.IsNullOrEmpty(newdoc.InvoiceNbr))
				newdoc.InvoiceNbr = order.VendorRefNbr;

			HashSet<APTran> duplicates = new HashSet<APTran>(new POLineComparer());
			foreach (APTran tran in this.Transactions.Select())
			{
				try
				{
					duplicates.Add(tran);
				}
				catch (NullReferenceException) { }
			}

			foreach (PXResult<POLine> res in
								PXSelect<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
										And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
										And<POLine.lineType, Equal<POLineType.service>,
										And<POLine.cancelled, Equal<False>,
										And<POLine.completed, Equal<False>,
										And<POLine.voucheredQty, LessEqual<POLine.orderQty>>>>>>>>.Select(this, order.OrderType, order.OrderNbr))
			{
				POLine orderline = (POLine)res;
				APTran tran = AddPOReceiptLine(orderline, duplicates);
			}
			//Copying of taxes from original PO Order doesn't have much sense - Invoice is not created from the PO Order directly 
			//and PO Order most likely has  items of other types

			if (list != null)
			{
				this.Save.Press();
				if (list.Find(this.Document.Current) == null)
				{
					list.Add(this.Document.Current);
				}
			}
		}

		public virtual void InvoicePOOrder(POOrder order, bool createNew)
		{
			APInvoice doc;
			if (createNew)
			{
				doc = new APInvoice();
				doc.DocType = APDocType.Invoice;
				doc.BranchID = order.BranchID;
				doc = PXCache<APInvoice>.CreateCopy(this.Document.Insert(doc));
				doc.VendorID = order.VendorID;
				doc.VendorLocationID = order.VendorLocationID;
				doc.TermsID = order.TermsID;
				doc.InvoiceNbr = order.VendorRefNbr;
				doc.CuryID = order.CuryID;
				doc.DocDesc = order.OrderDesc;
				doc = this.Document.Update(doc);
				doc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
				doc = this.Document.Update(doc);
			}
			else
			{
				doc = PXCache<APInvoice>.CreateCopy(Document.Current);
				if (string.IsNullOrEmpty(doc.DocDesc))
					doc.DocDesc = order.OrderDesc;
				if (string.IsNullOrEmpty(doc.InvoiceNbr))
					doc.InvoiceNbr = order.VendorRefNbr;
				doc = this.Document.Update(doc);
			}
			if ((doc.VendorID.HasValue && doc.VendorID != order.VendorID)
				|| (doc.VendorLocationID.HasValue && doc.VendorLocationID != order.VendorLocationID)
				|| doc.CuryID != order.CuryID)
				throw new PXException(Messages.PurchaseOrderBelongAnotherVendor);

			HashSet<APTran> duplicates = new HashSet<APTran>(new POLineComparer());
			foreach (APTran tran in this.Transactions.Select())
			{
				try
				{
					duplicates.Add(tran);
				}
				catch (NullReferenceException) { }
			}

			object dummy_poreceiptliner = new POReceiptLineR();

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(Transactions.Cache, null, TaxCalc.ManualCalc);

			foreach (PXResult<POLineAP, POLine> res in
							PXSelectJoin<POLineAP, 
							InnerJoin<POLine, On<POLineAP.orderType, Equal<POLine.orderType>, 
							And<POLineAP.orderNbr, Equal<POLine.orderNbr>, And<POLineAP.lineNbr, Equal<POLine.lineNbr>>>>>, 
							Where<POLine.orderType, Equal<Required<POLine.orderType>>,
										And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
										And<POLine.lineType, Equal<POLineType.service>,
										And<POLine.cancelled, Equal<False>,
										And<POLine.completed, Equal<False>,
										And<POLine.voucheredQty, LessEqual<POLine.orderQty>>>>>>>>.Select(this, order.OrderType, order.OrderNbr))
			{
				POLine orderline = res;
				POLineAP lineupd = res;

				PXRowInserting handler = new PXRowInserting((sender, e) =>
				{ 
					PXParentAttribute.SetParent(sender, e.Row, typeof(APRegister), Document.Current);
					PXParentAttribute.SetParent(sender, e.Row, typeof(POLineAP), lineupd);
					//PXParentAttribute.SetParent(sender, e.Row, typeof(POReceiptLineR), dummy_poreceiptliner);
				});

				this.RowInserting.AddHandler<APTran>(handler);

				APTran tran = AddPOReceiptLine(orderline, duplicates);

				this.RowInserting.RemoveHandler<APTran>(handler);
			}

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(Transactions.Cache, null, TaxCalc.ManualLineCalc);

			object copy = PXCache<APInvoice>.CreateCopy(Document.Current);
			Document.Current.TaxZoneID = null;
			Document.Cache.Update(copy);

			//Copying of taxes from original PO Order doesn't have much sense - Invoice is not created from the PO Order directly 
			//and PO Order most likely has  items of other types			
			this.Document.Cache.RaiseFieldUpdated<APInvoice.curyOrigDocAmt>(doc, 0m);
		}

		public virtual void InvoiceLandedCost(LandedCostTran aTran, DocumentList<APInvoice> list, bool saveAndAdd)
		{

			bool? requireControlTotal = APSetup.Current.RequireControlTotal;
			APSetup.Current.RequireControlTotal = false;
			APInvoice newdoc;
			if (!(String.IsNullOrEmpty(aTran.APDocType) || string.IsNullOrEmpty(aTran.APRefNbr))) return; // Invoice is already created
			LandedCostCode LCCode = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, aTran.LandedCostCodeID);
			CurrencyInfo tranCuryInfo = null;
			if (aTran.CuryInfoID != null)
				tranCuryInfo = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<LandedCostTran.curyInfoID>>>>.Select(this, aTran.CuryInfoID);

			string apDocType = aTran.CuryLCAmount > Decimal.Zero ? APDocType.Invoice : APDocType.DebitAdj;
			string terms = String.IsNullOrEmpty(aTran.TermsID) ? LCCode.TermsID : aTran.TermsID;
			if (list != null)
			{
				bool billSeparately = false;
				if (billSeparately == false)
				{
					
					newdoc = list.Find<APInvoice.docType, APInvoice.vendorID, APInvoice.vendorLocationID, APInvoice.curyID, APInvoice.invoiceNbr, APInvoice.docDate, APInvoice.termsID>(apDocType, aTran.VendorID, aTran.VendorLocationID, aTran.CuryID, aTran.InvoiceNbr,aTran.InvoiceDate, aTran.TermsID)?? new APInvoice();
				}
				else
				{
					newdoc = new APInvoice();
				}

				if (newdoc.RefNbr != null)
				{
					this.Document.Current = this.Document.Search<APInvoice.refNbr>(newdoc.RefNbr, newdoc.DocType);
				}
				else
				{
					newdoc.DocType = apDocType;
					newdoc.DocDate = aTran.InvoiceDate;
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = PXCache<APInvoice>.CreateCopy(this.Document.Insert(newdoc));
					newdoc.VendorID = aTran.VendorID;
					newdoc.VendorLocationID = aTran.VendorLocationID;
					if(apDocType!= AP.APDocType.DebitAdj)
						newdoc.TermsID = terms;
					newdoc.InvoiceNbr = aTran.InvoiceNbr;
					if (tranCuryInfo == null)
						newdoc.CuryID = aTran.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
					if (tranCuryInfo != null)
					{
						foreach (CurrencyInfo info in this.currencyinfo.Select())
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, tranCuryInfo);
							info.CuryInfoID = newdoc.CuryInfoID;
							newdoc.CuryID = info.CuryID;
						}
					}
				}
			}
			else
			{
				if(this.Document.Current != null && this.Document.Current.DocType == apDocType )
				{
					newdoc = PXCache<APInvoice>.CreateCopy(this.Document.Current);
				}
				else
				{
					newdoc = new APInvoice();
					newdoc.DocType = apDocType;
					newdoc.DocDate = aTran.InvoiceDate;
					newdoc = PXCache<APInvoice>.CreateCopy(this.Document.Insert(newdoc));					
				}

				if (newdoc.VendorID == null && newdoc.VendorLocationID == null)
				{
					newdoc.DocDate = aTran.InvoiceDate;
					newdoc.VendorID = aTran.VendorID;
					newdoc.VendorLocationID = aTran.VendorLocationID;
					if (apDocType != AP.APDocType.DebitAdj)
						newdoc.TermsID = terms;
					newdoc.InvoiceNbr = aTran.InvoiceNbr;
					if (tranCuryInfo == null)
						newdoc.CuryID = aTran.CuryID;
					newdoc = this.Document.Update(newdoc);
					newdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
					newdoc = this.Document.Update(newdoc);
					if (tranCuryInfo != null)
					{
						foreach (CurrencyInfo info in this.currencyinfo.Select())
						{
							PXCache<CurrencyInfo>.RestoreCopy(info, tranCuryInfo);
							info.CuryInfoID = newdoc.CuryInfoID;
							newdoc.CuryID = info.CuryID;
						}
					}
				}
			}
			newdoc = this.Document.Current;
			newdoc.InvoiceNbr = aTran.InvoiceNbr;

            PXCache receiptCache = this.Caches[typeof(PO.POReceipt)];
            PO.POReceipt receiptRow = PXSelect<PO.POReceipt, Where<PO.POReceipt.receiptType, Equal<Current<PO.LandedCostTran.pOReceiptType>>, And<PO.POReceipt.receiptNbr, Equal<Current<PO.LandedCostTran.pOReceiptNbr>>>>>.SelectSingleBound(this, new object[] { aTran });
            newdoc.RefNoteID = PXNoteAttribute.GetNoteID<PO.POReceipt.noteID>(receiptCache, receiptRow).Value;

			APTran tran = this.AddLandedCostTran(aTran, LCCode);
			newdoc.CuryTaxAmt = newdoc.CuryTaxTotal;
			this.Document.Cache.RaiseFieldUpdated<APInvoice.curyOrigDocAmt>(newdoc, 0m);
			newdoc.CuryOrigDocAmt = newdoc.CuryDocBal;
			this.Document.Cache.RaiseFieldUpdated<APInvoice.curyTaxAmt>(newdoc, 0m);
			APSetup.Current.RequireControlTotal = requireControlTotal;
			if (list != null && saveAndAdd)
			{
				this.Save.Press();
				aTran.APDocType = this.Document.Current.DocType;
				aTran.APRefNbr = this.Document.Current.RefNbr;
				if(!aTran.APCuryInfoID.HasValue)
					aTran.APCuryInfoID = this.Document.Current.CuryInfoID;
				if (list.Find(this.Document.Current) == null)
				{
					list.Add(this.Document.Current);
				}
			}
		}

		#endregion

		#region Utility Functions

		public virtual APTran AddPOReceiptLine(IAPTranSource aLine, HashSet<APTran> checkForDuplicates)
		{
			APTran newtran = new APTran();
			aLine.SetReferenceKeyTo(newtran);
			CurrencyInfo lineCuryInfo = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(this, aLine.CuryInfoID);
			CurrencyInfo billCuryInfo = this.currencyinfo.Select();

			if (checkForDuplicates != null)
			{
				if (checkForDuplicates.Contains(newtran))
				{
					return null;
				}
			}
			newtran.TranType = Document.Current.DocType;
			newtran.RefNbr = Document.Current.RefNbr;
			newtran.LineNbr = (int?)PXLineNbrAttribute.NewLineNbr<APTran.lineNbr>(Transactions.Cache, Document.Current);
			newtran.BranchID = aLine.BranchID;
			newtran.AccountID = aLine.POAccrualAcctID ?? aLine.ExpenseAcctID;
			newtran.SubID = aLine.POAccrualSubID ?? aLine.ExpenseSubID;
			newtran.LineType = aLine.LineType;
			newtran.InventoryID = aLine.InventoryID;
			newtran.UOM = string.IsNullOrEmpty(aLine.UOM) ? null : aLine.UOM;
			newtran.Qty = aLine.BillQty;
			newtran.ManualPrice = true;
			newtran.ManualDisc = true;
			newtran.FreezeManualDisc = true;
			newtran.DiscountID = aLine.DiscountID;
			newtran.DiscountSequenceID = aLine.DiscountSequenceID;
			if (billCuryInfo.CuryID == lineCuryInfo.CuryID)
			{
				newtran.CuryUnitCost = aLine.CuryUnitCost;
				newtran.CuryLineAmt = aLine.CuryLineAmt;
				newtran.CuryDiscAmt = aLine.CuryDiscAmt;
			}
			else
			{
				decimal curyUnitCost;
				decimal curyLineAmt;
				decimal curyDiscAmount;
				int costPrecision = this.commonsetup.Current.DecPlPrcCst.Value;
				PXCurrencyAttribute.PXCurrencyHelper.CuryConvCury(this.Document.Cache, this.Document.Current, aLine.UnitCost.Value, out curyUnitCost, costPrecision);
				PXCurrencyAttribute.PXCurrencyHelper.CuryConvCury(this.Document.Cache, this.Document.Current, aLine.LineAmt.Value, out curyLineAmt);
				PXCurrencyAttribute.PXCurrencyHelper.CuryConvCury(this.Document.Cache, this.Document.Current, aLine.DiscAmt.Value, out curyDiscAmount);
				newtran.CuryUnitCost = curyUnitCost;
				newtran.CuryLineAmt = curyLineAmt;
				newtran.CuryDiscAmt = curyDiscAmount;
			}
			newtran.DiscPct = aLine.DiscPct;
			newtran.TranDesc = aLine.TranDesc;
			newtran.TaxCategoryID = aLine.TaxCategoryID;
			newtran.TaxID = aLine.TaxID;

			//bool useProject = false;
			//if (aLine.ProjectID != null)
			//{
			//	if (insetup.Current.UpdateGL != true ||
			//		aLine.LineType == POLineType.GoodsForDropShip ||
			//		aLine.LineType == POLineType.NonStockForDropShip ||
			//	aLine.IsReturn ||
			//	POLineType.IsService(aLine.LineType))
			//	{
			//		useProject = true;
			//	}

			//	if (!useProject && POLineType.IsNonStock(aLine.LineType))
			//	{
			//		InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, aLine.InventoryID);
			//		if (item != null && item.NonStockReceipt != true)
			//		{
			//			useProject = true;
			//		}
			//	}
			//}

			//if (useProject)
			//{
				newtran.ProjectID = aLine.ProjectID;
				newtran.TaskID = aLine.TaskID;
			//}
			//else
			//{
			//	newtran.ProjectID = PM.GLProjectDefaultAttribute.NonProject(this);
			//}
			
			newtran = this.Transactions.Insert(newtran);

			if (aLine.GroupDiscountRate != 1m || aLine.DocumentDiscountRate != 1m)
			ProratePODiscounts(this.Transactions.Cache, newtran);

			//Special handling for the TaxCategoryID intentionally cleared in PO Receipt to prevent it's defaulting in AP
			if (aLine.TaxCategoryID == null && newtran.TaxCategoryID != null)
			{
				//APTran copy =(APTran) this.Transactions.Cache.CreateCopy(newtran);
				//copy.TaxCategoryID = aLine.TaxCategoryID;
				//copy = this.Transactions.Update(copy);
				//return copy;
			}
			return newtran;
		}

		public virtual APTran AddLandedCostTran(LandedCostTran aLine, LandedCostCode aLCCode)
		{
			APTran newtran = new APTran();
			Copy(newtran, aLine, aLCCode);
			newtran = this.Transactions.Insert(newtran);
			newtran.ProjectID = ProjectDefaultAttribute.NonProject(this);

			return newtran;
		}

		public virtual void Copy(APTran aDest, LandedCostTran aSrc, LandedCostCode aLCCode)
		{
            
            if (aSrc.LCAccrualAcct != null)
            {
                aDest.AccountID = aSrc.LCAccrualAcct.HasValue ? aSrc.LCAccrualAcct : aLCCode.LCAccrualAcct;
			aDest.SubID = aSrc.LCAccrualAcct.HasValue ? aSrc.LCAccrualSub : aLCCode.LCAccrualSub;
            }
			aDest.UOM = null;
			aDest.Qty = Decimal.One;
            if (aSrc.CuryLCAPAmount != null)
            {
			aDest.CuryUnitCost = Math.Abs(aSrc.CuryLCAPAmount.Value);
			aDest.CuryTranAmt = Math.Abs(aSrc.CuryLCAPAmount.Value);
            }
			aDest.TranDesc = aSrc.Descr;
			aDest.InventoryID = null;
			aDest.TaxCategoryID = aSrc.TaxCategoryID;
			aDest.TaxID = aSrc.TaxID;
			aDest.ReceiptType = aSrc.POReceiptType;
			aDest.ReceiptNbr = aSrc.POReceiptNbr;

            var poreceipt = (POReceipt)PXSelect<POReceipt, Where<POReceipt.receiptType, Equal<Current<LandedCostTran.pOReceiptType>>, And<POReceipt.receiptNbr, Equal<Current<LandedCostTran.pOReceiptNbr>>>>>.SelectSingleBound(this, new object[] { aSrc });
            if (poreceipt != null)
                aDest.BranchID = poreceipt.BranchID;

			aDest.ReceiptLineNbr = null;
			aDest.LCTranID = aSrc.LCTranID;
			aDest.LandedCostCodeID = aSrc.LandedCostCodeID;
		}


#if false
		public static void CopyCorrection(APTran aDest, LandedCostTran aSrc, Decimal aCuryAmount, Decimal aBaseAmount)
		{
			aDest.UOM = null;
			aDest.Qty = Decimal.One;
			aDest.CuryUnitCost = aCuryAmount;
			aDest.CuryTranAmt = aCuryAmount;
			aDest.UnitCost = aBaseAmount;
			aDest.TranAmt = aBaseAmount;
			aDest.TranDesc = "Correction";
			aDest.InventoryID = null;
			aDest.TaxCategoryID = null;
			aDest.ReceiptNbr = aSrc.POReceiptNbr;
			aDest.ReceiptLineNbr = null;
			aDest.LCTranID = aSrc.LCTranID;
			aDest.LandedCostCodeID = aSrc.LandedCostCodeID;
			aDest.LCAdjustment = true;
		}

		public static void CopyCorrections(CurrencyInfo aCuryInfo, LandedCostTran aSrc, LandedCostCode aLCCode, ref APTran aDest1, ref APTran aDest2)
		{
			decimal baseAmount = (aSrc.LCAPAmount ?? 0m) - (aSrc.LCAmount ?? 0m);
			decimal curyAmount;
			PXCurrencyAttribute.CuryConvCury(aCuryInfo, baseAmount, out curyAmount);
			if (curyAmount != Decimal.Zero || baseAmount != decimal.Zero || aDest1 != null || aDest2 != null)
			{
				if (aDest1 == null)
				{
					aDest1 = new APTran();
					aDest1.AccountID = aSrc.LCAccrualAcct.HasValue ? aSrc.LCAccrualAcct : aLCCode.LCAccrualAcct;
					aDest1.SubID = aSrc.LCAccrualAcct.HasValue ? aSrc.LCAccrualSub : aLCCode.LCAccrualSub;
				}
				if (aDest2 == null)
				{
					aDest2 = new APTran();
					aDest2.AccountID = aLCCode.LCVarianceAcct; // Variance here
					aDest2.SubID = aLCCode.LCVarianceSub;		 // Variance here	
				}
				CopyCorrection(aDest1, aSrc, -curyAmount, -baseAmount);
				CopyCorrection(aDest2, aSrc, curyAmount, baseAmount);
			}
		}
		
#endif
		private InventoryItem InventoryItemGetByID(int? inventoryID)
		{
			return PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);
		}

		private DRDeferredCode DeferredCodeGetByID(string deferredCodeID)
		{
			return PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>.Select(this, deferredCodeID);
		}

		#endregion

		protected virtual void CheckOrderTaskRule(PXCache sender, APTran row, int? newTaskID)
		{
			if (row.POOrderType != null && row.PONbr != null && row.POLineNbr != null && !POLineType.IsStock(row.LineType))
			{
				POLine poLine = PXSelectReadonly<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
								And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
								And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.POOrderType, row.PONbr, row.POLineNbr);

				if (poLine != null && poLine.TaskID != null && poLine.TaskID != newTaskID)
				{
					PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.TaskID);
					string taskCd = task != null ? taskCd = task.TaskCD : null;

					if (posetup.Current.OrderRequestApproval == true)
					{
						sender.RaiseExceptionHandling<APTran.taskID>(row, taskCd,
							new PXSetPropertyException(PO.Messages.TaskDiffersError, PXErrorLevel.Error));
					}
					else
					{
						sender.RaiseExceptionHandling<APTran.taskID>(row, taskCd,
							new PXSetPropertyException(PO.Messages.TaskDiffersWarning, PXErrorLevel.Warning));
					}
				}
			}
		}
		protected virtual void RecalculateDiscounts(PXCache sender, APTran line)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && line.Qty != null && line.CuryLineAmt != null)
			{
				if (PorateDiscounts() && line.ReceiptType != null && line.ReceiptNbr != null)
					ProratePODiscounts(sender, line, true);
				else
				{
					DeletePODiscounts();
				DiscountEngine<APTran>.SetDiscounts<APInvoiceDiscountDetail>(sender, Transactions, line, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.CuryID, Document.Current.DocDate, true, recalcdiscountsfilter.Current);
				}
			}
			if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && line.CuryLineAmt != null)
			{
				RecalculateTotalDiscount();
		}
		}

		private void RecalculateApplicationAmounts(APAdjust application)
		{
			PXCache applicationCache = this.Caches[typeof(APAdjust)];

			decimal currencyAdjustingAmount;
			decimal baseAdjustedAmount;
			decimal baseAdjustingAmount;

			PXDBCurrencyAttribute.CuryConvBase<APAdjust.adjdCuryInfoID>(
				applicationCache, 
				application, 
				application.CuryAdjdAmt.Value, 
				out baseAdjustedAmount);

			CurrencyInfo applicationCurrencyInfo = PXSelect<
				CurrencyInfo, 
				Where<CurrencyInfo.curyInfoID, Equal<Current<APAdjust.adjgCuryInfoID>>>>
				.SelectSingleBound(this, new object[] { application });

			CurrencyInfo documentCurrencyInfo = PXSelect<
				CurrencyInfo, 
				Where<CurrencyInfo.curyInfoID, Equal<Current<APAdjust.adjdCuryInfoID>>>>
				.SelectSingleBound(this, new object[] { application });

			if (string.Equals(applicationCurrencyInfo.CuryID, documentCurrencyInfo.CuryID))
			{
				currencyAdjustingAmount = (decimal)application.CuryAdjdAmt;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvCury<APAdjust.adjgCuryInfoID>(
					applicationCache, 
					application, 
					baseAdjustedAmount, 
					out currencyAdjustingAmount);
			}

			if (object.Equals(applicationCurrencyInfo.CuryID, documentCurrencyInfo.CuryID) && 
				object.Equals(applicationCurrencyInfo.CuryRate, documentCurrencyInfo.CuryRate) && 
				object.Equals(applicationCurrencyInfo.CuryMultDiv, documentCurrencyInfo.CuryMultDiv))
			{
				baseAdjustingAmount = baseAdjustedAmount;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvBase<APAdjust.adjgCuryInfoID>(
					applicationCache, 
					application, 
					currencyAdjustingAmount, 
					out baseAdjustingAmount);
			}

			application.CuryAdjgAmt = currencyAdjustingAmount;
			application.AdjAmt = baseAdjustedAmount;
			application.RGOLAmt = baseAdjustingAmount - baseAdjustedAmount;
		}

		private bool PorateDiscounts()
		{
			bool prorateDiscounts = false;
			foreach (APInvoiceDiscountDetail detail in DiscountDetails.Select())
			{
				if (detail.ReceiptNbr != null) prorateDiscounts = true;
			}
			return prorateDiscounts;
		}

		protected virtual Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<POOrderDiscountDetail, POLine>> CollectGroupDiscountToPOLineCorrelation(POOrder order)
		{
			PXCache cache = this.Caches[typeof(POLine)];
			PXSelectBase<POLine> transactions = new PXSelect<POLine, Where<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>>>(this);
			PXSelectBase<POOrderDiscountDetail> discountdetail = new PXSelect<POOrderDiscountDetail, Where<POOrderDiscountDetail.orderType, Equal<Current<POOrder.orderType>>, And<POOrderDiscountDetail.orderNbr, Equal<Current<POOrder.orderNbr>>>>, OrderBy<Asc<POOrderDiscountDetail.orderType, Asc<POOrderDiscountDetail.orderNbr>>>>(this);

			this.Defaults.Add(typeof(POOrder), () => { return order; });
			try
			{
				return DiscountEngine<POLine>.CollectGroupDiscountToLineCorrelation<POOrderDiscountDetail>(cache, transactions, discountdetail, order.VendorLocationID, (DateTime)order.OrderDate, false);
			}
			finally { this.Defaults.Remove(typeof(POOrder)); }
		}

		protected virtual Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<POReceiptDiscountDetail, POReceiptLine>> CollectGroupDiscountToPOReceiptLineCorrelation(POReceipt receipt)
		{
			PXCache cache = this.Caches[typeof(POReceiptLine)];
			PXSelectBase<POReceiptLine> transactions = new PXSelect<POReceiptLine, Where<POReceiptLine.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>(this);
			this.Caches[typeof(POReceipt)].Current = receipt;
			PXSelectBase<POReceiptDiscountDetail> discountdetail = new PXSelect<POReceiptDiscountDetail, Where<POReceiptDiscountDetail.receiptType, Equal<Current<POReceipt.receiptType>>, And<POReceiptDiscountDetail.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>, OrderBy<Asc<POReceiptDiscountDetail.receiptType, Asc<POReceiptDiscountDetail.receiptNbr>>>>(this);

			this.Defaults.Add(typeof(POReceipt), () => { return receipt; });
			try
			{
				return DiscountEngine<POReceiptLine>.CollectGroupDiscountToLineCorrelation<POReceiptDiscountDetail>(cache, transactions, discountdetail, receipt.VendorLocationID, (DateTime)receipt.ReceiptDate, false);
			}
			finally { this.Defaults.Remove(typeof(POReceipt)); }
		}

		protected virtual void ProratePODiscounts(PXCache sender, APTran line, bool skipInsert = false)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() != true) return;

			bool isPOrderDiscount = line.PONbr != null && line.ReceiptNbr == null;

			if (isPOrderDiscount)
				ProratePOOrderDiscounts(sender, line, skipInsert);
			else
				ProratePOReceiptDiscounts(sender, line, skipInsert);
		}

		protected virtual void ProratePOOrderDiscounts(PXCache sender, APTran line, bool skipInsert = false)
		{
			decimal apTranAmt = 0m;
			decimal poAmt = 0m;

			bool groupDiscountRecalculationNeeded = false;
			bool docDiscountRecalculationNeeded = false;

			List<APTran> transactionsList = new List<APTran>();

			foreach (PXResult<APTran, POLine> tran in TransactionsPOLine.Select())
			{
				APTran apTran = (APTran)tran;
				POLine poLine = (POLine)tran;
				if (apTran.PONbr != null && line.PONbr != null && apTran.POOrderType == line.POOrderType && apTran.PONbr == line.PONbr)
				{
					apTranAmt += apTran.CuryTranAmt ?? 0m;
					poAmt += poLine.CuryExtCost ?? 0m;
					if (poLine.GroupDiscountRate != 1m)
						groupDiscountRecalculationNeeded = true;
					if (poLine.DocumentDiscountRate != 1m)
						docDiscountRecalculationNeeded = true;
				}
				transactionsList.Add(apTran);
			}

			decimal? rate = 1m;
			if (poAmt > 0m)
				rate = apTranAmt / poAmt;

			if (groupDiscountRecalculationNeeded || docDiscountRecalculationNeeded)
			{
				PXSelectBase<POOrderDiscountDetail> selectOrderDocGroupDiscounts = new PXSelect<POOrderDiscountDetail,
					Where<POOrderDiscountDetail.orderType, Equal<Required<POOrderDiscountDetail.orderType>>,
					And<POOrderDiscountDetail.orderNbr, Equal<Required<POOrderDiscountDetail.orderNbr>>>>>(this);

				Dictionary<DiscountSequenceKey, List<APTran>> invoiceToPOCorrelation = new Dictionary<DiscountSequenceKey, List<APTran>>();

				bool documentModified = false;
				foreach (POOrderDiscountDetail docGroupDisc in selectOrderDocGroupDiscounts.Select(line.POOrderType, line.PONbr))
				{
					APInvoiceDiscountDetail dd = new APInvoiceDiscountDetail();
					dd.SkipDiscount = docGroupDisc.SkipDiscount;
					dd.Type = docGroupDisc.Type;
					dd.DiscountID = docGroupDisc.DiscountID;
					dd.DiscountSequenceID = docGroupDisc.DiscountSequenceID;
					dd.ReceiptType = null;
					dd.ReceiptNbr = null;
					dd.OrderType = line.POOrderType;
					dd.OrderNbr = line.PONbr;
					dd.DocType = line.TranType;
					dd.RefNbr = line.RefNbr;
					dd.IsManual = docGroupDisc.IsManual;
					dd.DiscountPct = docGroupDisc.DiscountPct;
					dd.FreeItemID = docGroupDisc.FreeItemID;
					dd.FreeItemQty = docGroupDisc.FreeItemQty;

					if (docGroupDisc.Type == DiscountType.Group)
					{
						POOrder poorder = PXSelect<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>, And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, line.POOrderType, line.PONbr);

						Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<POOrderDiscountDetail, POLine>> grLinesOrderCorrelation = CollectGroupDiscountToPOLineCorrelation(poorder);

						foreach (KeyValuePair<DiscountSequenceKey, DiscountEngine<POLine>.DiscountDetailToLineCorrelation<POOrderDiscountDetail, POLine>> dsGroup in grLinesOrderCorrelation)
						{
							if (dsGroup.Key.DiscountID == docGroupDisc.DiscountID && dsGroup.Key.DiscountSequenceID == docGroupDisc.DiscountSequenceID)
							{
								DiscountSequenceKey dsKey = new DiscountSequenceKey();
								dsKey.DiscountID = dsGroup.Key.DiscountID;
								dsKey.DiscountSequenceID = dsGroup.Key.DiscountSequenceID;
								decimal invoicedGroupAmt = 0m;
								foreach (POLine poLine in dsGroup.Value.listOfApplicableLines)
								{
									foreach (APTran tran in Transactions.Select())
									{
										if (poLine.OrderType == tran.POOrderType && poLine.OrderNbr == tran.PONbr && poLine.LineNbr == tran.POLineNbr)
										{
											invoicedGroupAmt += (tran.CuryTranAmt ?? 0m);
											if (invoiceToPOCorrelation.ContainsKey(dsKey))
											{
												invoiceToPOCorrelation[dsKey].Add(tran);
											}
											else
											{
												invoiceToPOCorrelation.Add(dsKey, new List<APTran>());
												invoiceToPOCorrelation[dsKey].Add(tran);
											}
										}
									}
								}
								rate = (invoicedGroupAmt / (decimal)dsGroup.Value.discountDetailLine.CuryDiscountableAmt);
							}
						}
					}

					APInvoiceDiscountDetail located = DiscountDetails.Locate(dd);
					//LineNbr prevents Locate() from work as intended. To review.
					if (located == null)
					{
						foreach (APInvoiceDiscountDetail detail in DiscountDetails.Select())
						{
							if (detail.DiscountID == dd.DiscountID && detail.DiscountSequenceID == dd.DiscountSequenceID && detail.DocType == dd.DocType
								&& detail.RefNbr == dd.RefNbr && detail.OrderType == dd.OrderType && detail.OrderNbr == dd.OrderNbr && detail.Type == dd.Type)
								located = detail;
						}
					}
					if (located != null)
					{
						located.DiscountAmt = docGroupDisc.DiscountAmt * rate;
						located.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
						located.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
						located.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
						located.DiscountableQty = docGroupDisc.DiscountableQty * rate;

						DiscountDetails.Update(located);
						documentModified = true;
					}
					else if (!skipInsert)
					{
						dd.DiscountAmt = docGroupDisc.DiscountAmt * rate;
						dd.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
						dd.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
						dd.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
						dd.DiscountableQty = docGroupDisc.DiscountableQty * rate;

						DiscountDetails.Insert(dd);
						documentModified = true;
					}
					if (poAmt > 0m)
						rate = apTranAmt / poAmt;
				}


				if (documentModified)
				{
					RecalculateTotalDiscount();

					if (groupDiscountRecalculationNeeded)
						DiscountEngine<APTran>.CalculateGroupDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, transactionsList, null, invoiceToPOCorrelation, DiscountDetails, false, null);
					if (docDiscountRecalculationNeeded)
						DiscountEngine<APTran>.CalculateDocumentDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, transactionsList, null, DiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m, Document.Current.DocType, Document.Current.RefNbr, true, false, true);
				}
			}
		}

		protected virtual void ProratePOReceiptDiscounts(PXCache sender, APTran line, bool skipInsert = false)
		{
			decimal apTranAmt = 0m;
			decimal poAmt = 0m;

			bool groupDiscountRecalculationNeeded = false;
			bool docDicountRecalculationNeeded = false;

			List<APTran> transactionsList = new List<APTran>();

			foreach (PXResult<APTran, POReceiptLine> tran in Transactions.Select())
			{
				APTran apTran = (APTran)tran;
				POReceiptLine receiptLine = (POReceiptLine)tran;
				if (apTran.ReceiptNbr != null && line.ReceiptNbr != null && apTran.ReceiptType == line.ReceiptType && apTran.ReceiptNbr == line.ReceiptNbr)
				{
					apTranAmt += apTran.CuryTranAmt ?? 0m;
					poAmt += receiptLine.CuryExtCost ?? 0m;
					if (receiptLine.GroupDiscountRate != 1m)
						groupDiscountRecalculationNeeded = true;
					if (receiptLine.DocumentDiscountRate != 1m)
						docDicountRecalculationNeeded = true;
				}
				transactionsList.Add(apTran);
			}

			decimal? rate = 1m;
			if (poAmt > 0m)
				rate = apTranAmt / poAmt;

			if (groupDiscountRecalculationNeeded || docDicountRecalculationNeeded)
			{
			PXSelectBase<POReceiptDiscountDetail> selectReceiptDocGroupDiscounts = new PXSelect<POReceiptDiscountDetail,
				Where<POReceiptDiscountDetail.receiptType, Equal<Required<POReceiptDiscountDetail.receiptType>>,
				And<POReceiptDiscountDetail.receiptNbr, Equal<Required<POReceiptDiscountDetail.receiptNbr>>>>>(this);

				Dictionary<DiscountSequenceKey, List<APTran>> invoiceToPOCorrelation = new Dictionary<DiscountSequenceKey, List<APTran>>();

				bool documentModified = false;
			foreach (POReceiptDiscountDetail docGroupDisc in selectReceiptDocGroupDiscounts.Select(line.ReceiptType, line.ReceiptNbr))
			{
				APInvoiceDiscountDetail dd = new APInvoiceDiscountDetail();
				dd.SkipDiscount = docGroupDisc.SkipDiscount;
				dd.Type = docGroupDisc.Type;
				dd.DiscountID = docGroupDisc.DiscountID;
				dd.DiscountSequenceID = docGroupDisc.DiscountSequenceID;
				dd.ReceiptType = docGroupDisc.ReceiptType;
				dd.ReceiptNbr = docGroupDisc.ReceiptNbr;
				dd.DocType = line.TranType;
				dd.RefNbr = line.RefNbr;
					dd.OrderType = line.POOrderType;
					dd.OrderNbr = line.PONbr;
				dd.IsManual = docGroupDisc.IsManual;
				dd.DiscountPct = docGroupDisc.DiscountPct;
				dd.FreeItemID = docGroupDisc.FreeItemID;
				dd.FreeItemQty = docGroupDisc.FreeItemQty;

				if (docGroupDisc.Type == DiscountType.Group)
				{
					POReceipt poreceipt = PXSelect<POReceipt, Where<POReceipt.receiptType, Equal<Required<POReceipt.receiptType>>, And<POReceipt.receiptNbr, Equal<Required<POReceipt.receiptNbr>>>>>.Select(this, line.ReceiptType, line.ReceiptNbr);

						Dictionary<DiscountSequenceKey, DiscountEngine.DiscountDetailToLineCorrelation<POReceiptDiscountDetail, POReceiptLine>> grLinesOrderCorrelation = CollectGroupDiscountToPOReceiptLineCorrelation(poreceipt);

					foreach (KeyValuePair<DiscountSequenceKey, DiscountEngine<POReceiptLine>.DiscountDetailToLineCorrelation<POReceiptDiscountDetail, POReceiptLine>> dsGroup in grLinesOrderCorrelation)
					{
						if (dsGroup.Key.DiscountID == docGroupDisc.DiscountID && dsGroup.Key.DiscountSequenceID == docGroupDisc.DiscountSequenceID)
						{
							DiscountSequenceKey dsKey = new DiscountSequenceKey();
							dsKey.DiscountID = dsGroup.Key.DiscountID;
							dsKey.DiscountSequenceID = dsGroup.Key.DiscountSequenceID;
							decimal invoicedGroupAmt = 0m;
							foreach (POReceiptLine poLine in dsGroup.Value.listOfApplicableLines)
							{
								foreach (APTran tran in Transactions.Select())
								{
										if (poLine.ReceiptType == tran.ReceiptType && poLine.ReceiptNbr == tran.ReceiptNbr && poLine.LineNbr == tran.ReceiptLineNbr)
									{
										invoicedGroupAmt += (tran.CuryTranAmt ?? 0m);
											if (invoiceToPOCorrelation.ContainsKey(dsKey))
										{
												invoiceToPOCorrelation[dsKey].Add(tran);
										}
										else
										{
												invoiceToPOCorrelation.Add(dsKey, new List<APTran>());
												invoiceToPOCorrelation[dsKey].Add(tran);
										}
									}
								}
							}
							rate = (invoicedGroupAmt / (decimal)dsGroup.Value.discountDetailLine.CuryDiscountableAmt);
						}
					}
				}

				APInvoiceDiscountDetail located = DiscountDetails.Locate(dd);
					//LineNbr prevents Locate() from work as intended. To review.
				if (located == null)
				{
					foreach (APInvoiceDiscountDetail detail in DiscountDetails.Select())
					{
						if (detail.DiscountID == dd.DiscountID && detail.DiscountSequenceID == dd.DiscountSequenceID && detail.DocType == dd.DocType
								&& detail.RefNbr == dd.RefNbr && detail.ReceiptType == dd.ReceiptType && detail.ReceiptNbr == dd.ReceiptNbr && detail.Type == dd.Type)
							located = detail;
					}
				}
				if (located != null)
				{
					located.DiscountAmt = docGroupDisc.DiscountAmt * rate;
					located.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
					located.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
					located.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
					located.DiscountableQty = docGroupDisc.DiscountableQty * rate;

					DiscountDetails.Update(located);
						documentModified = true;
				}
				else if (!skipInsert)
				{
					dd.DiscountAmt = docGroupDisc.DiscountAmt * rate;
					dd.CuryDiscountAmt = docGroupDisc.CuryDiscountAmt * rate;
					dd.DiscountableAmt = docGroupDisc.DiscountableAmt * rate;
					dd.CuryDiscountableAmt = docGroupDisc.CuryDiscountableAmt * rate;
					dd.DiscountableQty = docGroupDisc.DiscountableQty * rate;

					DiscountDetails.Insert(dd);
						documentModified = true;
				}
					if (poAmt > 0m)
						rate = apTranAmt / poAmt;
			}


				if (documentModified)
				{
				RecalculateTotalDiscount();

					if (groupDiscountRecalculationNeeded)
						DiscountEngine<APTran>.CalculateGroupDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, transactionsList, null, invoiceToPOCorrelation, DiscountDetails, false, null);
					if (docDicountRecalculationNeeded)
						DiscountEngine<APTran>.CalculateDocumentDiscountRate<APInvoiceDiscountDetail>(Transactions.Cache, Transactions, transactionsList, null, DiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m, Document.Current.DocType, Document.Current.RefNbr, true, false, true);
				}
			}
		}

		private void DeletePODiscounts(string ReceiptType = null, string ReceiptNbr = null)
		{
			foreach (APInvoiceDiscountDetail detail in DiscountDetails.Select())
			{
				if (detail.ReceiptNbr != null && detail.ReceiptType != null && 
					((ReceiptType == null && ReceiptNbr == null) 
					|| (ReceiptType != null && ReceiptNbr != null && ReceiptType == detail.ReceiptType && ReceiptNbr == detail.ReceiptNbr)))
				DiscountDetails.Delete(detail);
			}
		}

		private void RecalculateTotalDiscount()
		{
			if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.InsertedDeleted)
			{
				APInvoice old_row = PXCache<APInvoice>.CreateCopy(Document.Current);
				Document.Cache.SetValueExt<APInvoice.curyDiscTot>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<APInvoiceDiscountDetail>(DiscountDetails));
				Document.Cache.RaiseRowUpdated(Document.Current, old_row);
			}
		}


		public virtual void SetProjectIDForDiscountTran(APTran apTran)
		{
			var docProjectIDs = Transactions.Select()
											.RowCast<APTran>()
											.Where(tran => tran.ProjectID != null)
											.Select(tran => tran.ProjectID)
											.Distinct()
											.ToArray();

			var projectID = docProjectIDs.Length == 1
								? docProjectIDs.Single()
								: ProjectDefaultAttribute.NonProject(this);

			apTran.ProjectID = projectID;
		}

		public virtual void SetTaskIDForDiscountTran(APTran apTran)
		{
			PM.PMProject project = PXSelect<PM.PMProject, 
											Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>
											.Select(this, apTran.ProjectID);

			if (project != null && project.BaseType != Contract.ContractBaseType.Contract)
			{
				PM.PMAccountTask task = PXSelect<PM.PMAccountTask, 
													Where<PM.PMAccountTask.projectID, Equal<Required<PM.PMAccountTask.projectID>>, 
														And<PM.PMAccountTask.accountID, Equal<Required<PM.PMAccountTask.accountID>>>>>
													.Select(this, apTran.ProjectID, apTran.AccountID);
				if (task != null)
				{
					apTran.TaskID = task.TaskID;
					return;
				}

				Account ac = PXSelect<Account,
										Where<Account.accountID, Equal<Required<Account.accountID>>>>
										.Select(this, apTran.AccountID);

				throw new PXException(Messages.AccountMappingNotConfigured, project.ContractCD, ac.AccountCD);
			}
		}

		protected virtual void AddDiscount(PXCache sender, APInvoice row)
		{
			APTran discount = (APTran)Discount_Row.Cache.CreateInstance();
			discount.LineType = SOLineType.Discount;
			discount.DrCr = (Document.Current.DrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
			discount = (APTran)Discount_Row.Select() ?? (APTran)Discount_Row.Cache.Insert(discount);

			APTran old_row = (APTran)Discount_Row.Cache.CreateCopy(discount);

			discount.CuryTranAmt = (decimal?)sender.GetValue<APInvoice.curyDiscTot>(row);
			discount.TaxCategoryID = null;
			using (new PXLocaleScope(vendor.Current?.LocaleName))
			{
			discount.TranDesc = PXMessages.LocalizeNoPrefix(Messages.DocDiscDescr);
			}

			DefaultDiscountAccountAndSubAccount(discount);

			//ToDo: create separate discount lines for different projects 
			if (discount.ProjectID == null)
			{
				SetProjectIDForDiscountTran(discount);
			}

			if (discount.ProjectID == null && old_row.ProjectID != null)
				discount.ProjectID = old_row.ProjectID;

			if (discount.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, discount.ProjectID))
			{
				SetTaskIDForDiscountTran(discount);
			}

			if (Discount_Row.Cache.GetStatus(discount) == PXEntryStatus.Notchanged)
			{
				Discount_Row.Cache.SetStatus(discount, PXEntryStatus.Updated);
			}

			discount.ManualDisc = true; //escape SOManualDiscMode.RowUpdated
			Discount_Row.Cache.RaiseRowUpdated(discount, old_row);

			decimal auotDocDisc = DiscountEngine.GetTotalGroupAndDocumentDiscount<APInvoiceDiscountDetail>(DiscountDetails);
			if (auotDocDisc == discount.CuryTranAmt)
			{
				discount.ManualDisc = false;
			}
		}

		protected object GetValue<Field>(object data)
			where Field : IBqlField
		{
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

		private void DefaultDiscountAccountAndSubAccount(APTran tran)
		{
			Location vendorloc = location.Current;

			object vendor_LocationAcctID = GetValue<Location.vDiscountAcctID>(vendorloc);

			if (vendor_LocationAcctID != null)
			{
				tran.AccountID = (int?)vendor_LocationAcctID;
				Discount_Row.Cache.RaiseFieldUpdated<APTran.accountID>(tran, null);
			}

			if (tran.AccountID != null)
			{
				object vendor_LocationSubID = GetValue<Location.vDiscountSubID>(vendorloc);
				if (vendor_LocationSubID != null)
				{
					tran.SubID = (int?)vendor_LocationSubID;
					Discount_Row.Cache.RaiseFieldUpdated<APTran.subID>(tran, null);
				}
			}

		}

#if false
		public virtual APTran AddPOOrderLine(POLine aLine, bool checkForDuplicates)
		{
			if (checkForDuplicates)
			{
				foreach (APTran iTr in this.Transactions.Select())
				{
					if (CompareSourceKey(iTr, aLine))
						return iTr;
				}
			}

			APTran newtran = new APTran();
			newtran.AccountID = aLine.ExpenseAcctID;
			newtran.SubID = aLine.ExpenseSubID;
			newtran.POOrderType = aLine.OrderType;
			newtran.PONbr = aLine.OrderNbr;
			newtran.POLineNbr = aLine.LineNbr;
			newtran.LineType = aLine.LineType;
			newtran.InventoryID = aLine.InventoryID;
			newtran.UOM = aLine.UOM;
			newtran.Qty = aLine.OrderQty - aLine.VoucheredQty;
			newtran.CuryUnitCost = aLine.CuryUnitCost;
			newtran.CuryTranAmt = (aLine.CuryExtCost - aLine.CuryReceivedCost);
			newtran.TranDesc = aLine.TranDesc;
			newtran.TaxCategoryID = aLine.TaxCategoryID;

			newtran = this.Transactions.Insert(newtran);
			return newtran;
		}

		static bool CompareSourceKey(APTran aTran, POLine aLine)
		{
			return (aTran.PONbr == aLine.OrderNbr && aTran.POOrderType == aLine.OrderType && aTran.POLineNbr == aLine.LineNbr);
		}

		static bool CompareSourceKey(APTran aTran, POReceiptLine aLine)
		{
			return (aTran.ReceiptNbr == aLine.ReceiptNbr && aTran.ReceiptLineNbr == aLine.LineNbr);
		} 
#endif	

		#region Private Variables
		private bool _allowToVoidReleased = false; 
		#endregion

		#region Internal Member Definitions

		#region POOrder and Receipt Filter
		public partial class LinkLineFilter : IBqlTable
		{
			#region POOrderNbr
			public abstract class pOOrderNbr : PX.Data.IBqlField
			{
			}
			protected String _POOrderNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXUIField(DisplayName = "PO Nbr.")]
			[PXSelector(typeof(Search5<
				POOrder.orderNbr
				, LeftJoin<LinkLineReceipt, On<POOrder.orderNbr, Equal<LinkLineReceipt.orderNbr>, And<POOrder.orderType, Equal<LinkLineReceipt.orderType>, And<Current<LinkLineFilter.selectedMode>, Equal<LinkLineFilter.selectedMode.receipt>>>>
					, LeftJoin<LinkLineOrder, On<POOrder.orderNbr, Equal<LinkLineOrder.orderNbr>, And<POOrder.orderType, Equal<LinkLineOrder.orderType>, And<Current<LinkLineFilter.selectedMode>, Equal<LinkLineFilter.selectedMode.order>>>>>
				>
				, Where<LinkLineReceipt.orderNbr, IsNotNull, Or<LinkLineOrder.orderType, IsNotNull>>
				, Aggregate<GroupBy<POOrder.orderNbr, GroupBy<POOrder.orderType>>>
				>)
				)]
			public virtual String POOrderNbr
			{
				get
				{
					return this._POOrderNbr;
				}
				set
				{
					this._POOrderNbr = value;
				}
			}
			#endregion
			#region POReceiptNbr
			public abstract class pOReceiptNbr : PX.Data.IBqlField
			{
			}
			protected String _POReceiptNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXUIField(DisplayName = "PO Receipt Nbr.")]
			[PXSelector(typeof(Search<POReceipt.receiptNbr>))]
			public virtual String POReceiptNbr
			{
				get
				{
					return this._POReceiptNbr;
				}
				set
				{
					this._POReceiptNbr = value;
				}
			}
			#endregion

			#region SiteID
			public abstract class siteID : PX.Data.IBqlField
			{
			}
			protected Int32? _SiteID;
			[PXDBInt()]
			[PXUIField(DisplayName = "Warehouse", FieldClass = SiteAttribute.DimensionName)]
			[PXSelector(typeof(Search5<
				INSite.siteID
					, LeftJoin<LinkLineReceipt, On<INSite.siteID, Equal<LinkLineReceipt.receiptSiteID>, And<Current<LinkLineFilter.selectedMode>, Equal<LinkLineFilter.selectedMode.receipt>>>
						, LeftJoin<LinkLineOrder, On<INSite.siteID, Equal<LinkLineOrder.orderSiteID>, And<Current<LinkLineFilter.selectedMode>, Equal<LinkLineFilter.selectedMode.order>>>>
					>
				, Where<LinkLineReceipt.receiptSiteID, IsNotNull, Or<LinkLineOrder.orderSiteID, IsNotNull>>
				, Aggregate<GroupBy<INSite.siteID>>>), SubstituteKey = typeof(INSite.siteCD), DescriptionField = typeof(INSite.descr))]
			[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
			[PXFormula(typeof(Default<LinkLineFilter.selectedMode>))]
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

			#region selectedMode
			public abstract class selectedMode : PX.Data.IBqlField
			{
				public const string Order = "O";
				public const string Receipt = "R";
				public class order : Constant<string>
				{
					public order() : base(Order) { }
				}

				public class receipt : Constant<string>
				{
					public receipt() : base(Receipt) { }
				}
			}
			protected String _SelectedMode;

			[PXDBString(1)]
			[PXFormula(typeof(Switch<Case<Where<Selector<inventoryID, InventoryItem.stkItem>, NotEqual<True>, And<Selector<inventoryID, InventoryItem.nonStockReceipt>, NotEqual<True>>>, selectedMode.order>, selectedMode.receipt>))]
			[PXUIEnabled(typeof(Where<Selector<inventoryID, InventoryItem.stkItem>, NotEqual<True>, And<Selector<inventoryID, InventoryItem.nonStockReceipt>, NotEqual<True>>>))]
			[PXUIField(DisplayName = "Selected Mode")]
			[PXStringList(new[] { selectedMode.Order, selectedMode.Receipt }, new[] { Messages.POOrderMode, Messages.POReceiptMode })]
			public virtual String SelectedMode
			{
				get
				{
					return this._SelectedMode;
				}
				set
				{
					this._SelectedMode = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;

			[Inventory(Enabled = false)]
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

			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;

			[INUnit(typeof(inventoryID), Enabled = false)]
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

		}
		#endregion

		#region Receipt for link
		[PXProjection(typeof(
			Select2<
				POReceiptLine
				, LeftJoin<POReceipt
				, On<POReceipt.receiptType, Equal<POReceiptLine.receiptType>, And<POReceipt.receiptNbr, Equal<POReceiptLine.receiptNbr>>>
					, LeftJoin<APTran
						, On<APTran.released, NotEqual<True>
							, And<APTran.receiptNbr, Equal<POReceiptLine.receiptNbr>, And<APTran.receiptType, Equal<POReceiptLine.receiptType>, And<APTran.receiptLineNbr, Equal<POReceiptLine.lineNbr>>>>
							>
						>
					>
				, Where2<
					Where<POReceiptLine.pOType, Equal<POOrderType.regularOrder>, Or<POReceiptLine.pOType, IsNull>>
					, And<POReceiptLine.unbilledAmt, NotEqual<int0>
						, And<POReceiptLine.unbilledQty, Greater<int0>
							, And<POReceipt.released, Equal<True>
								, And<APTran.refNbr, IsNull>
								>
							>
						>
					>
				>
			), Persistent = false)]
		[Serializable]
		public partial class LinkLineReceipt : IBqlTable
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
			[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.pOType))]
			[POOrderType.List()]
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
			[PXDBString(15, IsUnicode = true, BqlField = typeof(POReceiptLine.pONbr))]
			[PXUIField(DisplayName = "Order Nbr.")]
			[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Current<orderType>>>>))]
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
			#region OrderLineNbr
			public abstract class orderLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderLineNbr;
			[PXUIField(DisplayName = "PO Line", Visible = false)]
			[PXDBInt(BqlField = typeof(POReceiptLine.pOLineNbr))]
			public virtual Int32? OrderLineNbr
			{
				get
				{
					return this._OrderLineNbr;
				}
				set
				{
					this._OrderLineNbr = value;
				}
			}
			#endregion


			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[POLineInventoryItem(Filterable = true, BqlField = typeof(POReceiptLine.inventoryID))]
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
			[PXDBInt(BqlField = typeof(POReceipt.vendorID))]
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
			[PXDBInt(BqlField = typeof(POReceipt.vendorLocationID))]
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

			
			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;
			[INUnit(typeof(inventoryID),DisplayName="UOM", BqlField = typeof(POReceiptLine.uOM))]
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

			#region ReceiptType
			public abstract class receiptType : PX.Data.IBqlField
			{
			}
			protected String _ReceiptType;
			[PXDBString(2, IsFixed = true, IsKey = true, InputMask = "", BqlField = typeof(POReceiptLine.receiptType))]
			[PXDefault(POReceiptType.POReceipt)]
			[POReceiptType.List()]
			[PXUIField(DisplayName = "Type")]
			public virtual String ReceiptType
			{
				get
				{
					return this._ReceiptType;
				}
				set
				{
					this._ReceiptType = value;
				}
			}
			#endregion
			#region ReceiptNbr
			public abstract class receiptNbr : PX.Data.IBqlField
			{
			}
			protected String _ReceiptNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(POReceiptLine.receiptNbr))]
			[PXSelector(typeof(Search<POReceipt.receiptNbr, Where<POReceipt.receiptType, Equal<Current<receiptType>>>>), ValidateValue = false)]
			[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PXUIVerify(typeof(Where<receiptCuryID, Equal<Current<APInvoice.curyID>>>), PXErrorLevel.RowWarning, Messages.APDocumentCurrencyDiffersFromSourceDocument, true)]
			public virtual String ReceiptNbr
			{
				get
				{
					return this._ReceiptNbr;
				}
				set
				{
					this._ReceiptNbr = value;
				}
			}
			#endregion
			#region ReceiptLineNbr
			public abstract class receiptLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _ReceiptLineNbr;
			[PXUIField(DisplayName = "PO Receipt Line", Visible = false)]
			[PXDBInt(IsKey = true, BqlField = typeof(POReceiptLine.lineNbr))]
			[PXDefault(1)]
			public virtual Int32? ReceiptLineNbr
			{
				get
				{
					return this._ReceiptLineNbr;
				}
				set
				{
					this._ReceiptLineNbr = value;
				}
			}
			#endregion		

			#region ReceiptQty
			public abstract class receiptQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptQty;

			[PXDBQuantity(BqlField = typeof(POReceiptLine.receiptQty))]
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

			#endregion
			#region ReceiptCuryInfoID
			public abstract class receiptCuryInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _ReceiptCuryInfoID;

			[PXDBLong(BqlField = typeof(POReceiptLine.curyInfoID))]
			[CurrencyInfo(CuryIDField = "ReceiptCuryID")]
			public virtual Int64? ReceiptCuryInfoID
			{
				get
				{
					return this._ReceiptCuryInfoID;
				}
				set
				{
					this._ReceiptCuryInfoID = value;
				}
			}
			#endregion

			#region ReceiptAmount
			public abstract class receiptAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptAmount;
			[PXDBDecimal(BqlField = typeof(POReceiptLine.extCost))]
			public virtual Decimal? ReceiptAmount
			{
				get
				{
					return this._ReceiptAmount;
				}
				set
				{
					this._ReceiptAmount = value;
				}
			}
			#endregion
			#region ReceiptCuryAmount
			public abstract class receiptCuryAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptCuryAmount;
			[PXDBCurrency(typeof(receiptCuryInfoID), typeof(receiptAmount), BqlField = typeof(POReceiptLine.curyExtCost))]
			[PXUIField(DisplayName = "Amount")]
			public virtual Decimal? ReceiptCuryAmount
			{
				get
				{
					return this._ReceiptCuryAmount;
				}
				set
				{
					this._ReceiptCuryAmount = value;
				}
			}
			#endregion
			#region ReceiptCuryID
			public abstract class receiptCuryID : PX.Data.IBqlField
			{
			}
			protected String _ReceiptCuryID;
			[PXUIField(DisplayName = "Currency")]
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POReceipt.curyID))]
			[PXSelector(typeof(Currency.curyID))]
			public virtual String ReceiptCuryID
			{
				get
				{
					return this._ReceiptCuryID;
				}
				set
				{
					this._ReceiptCuryID = value;
				}
			}
			#endregion

			#region ReceiptReceiptUnbilledQty
			public abstract class receiptUnbilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptUnbilledQty;
			[PXDBQuantity(typeof(uOM), typeof(receiptBaseUnbilledQty), HandleEmptyKey = true, BqlField = typeof(POReceiptLine.unbilledQty))]
			[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
			public virtual Decimal? ReceiptUnbilledQty
			{
				get
				{
					return this._ReceiptUnbilledQty;
				}
				set
				{
					this._ReceiptUnbilledQty = value;
				}
			}

			#endregion
			#region ReceiptBaseUnbilledQty
			public abstract class receiptBaseUnbilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptBaseUnbilledQty;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.baseUnbilledQty))]
			public virtual Decimal? ReceiptBaseUnbilledQty
			{
				get
				{
					return this._ReceiptBaseUnbilledQty;
				}
				set
				{
					this._ReceiptBaseUnbilledQty = value;
				}
			}
			#endregion
			#region ReceiptCuryUnbilledAmt
			public abstract class receiptCuryUnbilledAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptCuryUnbilledAmt;
			[PXDBCurrency(typeof(receiptCuryInfoID), typeof(receiptUnbilledAmt), BqlField = typeof(POReceiptLine.curyUnbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Enabled = false)]
			public virtual Decimal? ReceiptCuryUnbilledAmt
			{
				get
				{
					return this._ReceiptCuryUnbilledAmt;
				}
				set
				{
					this._ReceiptCuryUnbilledAmt = value;
				}
			}

			#endregion
			#region ReceiptUnbilledAmt
			public abstract class receiptUnbilledAmt : PX.Data.IBqlField
			{
			}

			protected Decimal? _ReceiptUnbilledAmt;
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.unbilledAmt))]
			public virtual Decimal? ReceiptUnbilledAmt
			{
				get
				{
					return this._ReceiptUnbilledAmt;
				}
				set
				{
					this._ReceiptUnbilledAmt = value;
				}
			}
			#endregion
			#region ReceiptSiteID
			public abstract class receiptSiteID : PX.Data.IBqlField
			{
			}
			protected Int32? _ReceiptSiteID;
			[Site(BqlField = typeof(POReceiptLine.siteID))]
			public virtual Int32? ReceiptSiteID
			{
				get
				{
					return this._ReceiptSiteID;
				}
				set
				{
					this._ReceiptSiteID = value;
				}
			}
			#endregion
			#region ReceiptSubItemID
			public abstract class receiptSubItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _ReceiptSubItemID;
			[SubItem(typeof(inventoryID), BqlField = typeof(POReceiptLine.subItemID))]
			public virtual Int32? ReceiptSubItemID
			{
				get
				{
					return this._ReceiptSubItemID;
				}
				set
				{
					this._ReceiptSubItemID = value;
				}
			}
			#endregion		
			#region ReceiptCuryUnbilledDiscountAmt
			public abstract class receiptCuryUnbilledDiscountAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptCuryUnbilledDiscountAmt;
			[PXDBCurrency(typeof(receiptCuryInfoID), typeof(receiptUnbilledDiscountAmt), BqlField = typeof(POReceiptLine.curyUnbilledDiscountAmt))]
			public virtual Decimal? ReceiptCuryUnbilledDiscountAmt
			{
				get
				{
					return this._ReceiptCuryUnbilledDiscountAmt;
				}
				set
				{
					this._ReceiptCuryUnbilledDiscountAmt = value;
				}
			}
			#endregion
			#region ReceiptUnbilledDiscountAmt
			public abstract class receiptUnbilledDiscountAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptUnbilledDiscountAmt;
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.unbilledDiscountAmt))]
			public virtual Decimal? ReceiptUnbilledDiscountAmt
			{
				get
				{
					return this._ReceiptUnbilledDiscountAmt;
				}
				set
				{
					this._ReceiptUnbilledDiscountAmt = value;
				}
			}
			#endregion
			#region ReceiptTotalCuryUnbilledAmount
			public abstract class receiptTotalCuryUnbilledAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptTotalCuryUnbilledAmount;
			[PXCurrency(typeof(receiptCuryInfoID), typeof(receiptTotalUnbilledAmount))]
			[PXFormula(typeof(Sub<receiptCuryUnbilledAmt, receiptCuryUnbilledDiscountAmt>))]
			[PXUIField(DisplayName = "Unbilled Amount")]
			public virtual Decimal? ReceiptTotalCuryUnbilledAmount
			{
				get
				{
					return this._ReceiptTotalCuryUnbilledAmount;
				}
				set
				{
					this._ReceiptTotalCuryUnbilledAmount = value;
				}
			}
			#endregion
			#region ReceiptTotalUnbilledAmount
			public abstract class receiptTotalUnbilledAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ReceiptTotalUnbilledAmount;
			[PXBaseCury()]
			[PXFormula(typeof(Sub<receiptUnbilledAmt, receiptUnbilledDiscountAmt>))]
			public virtual Decimal? ReceiptTotalUnbilledAmount
			{
				get
				{
					return this._ReceiptTotalUnbilledAmount;
				}
				set
				{
					this._ReceiptTotalUnbilledAmount = value;
				}
			}
			#endregion


			#region POAccrualAcctID
			public abstract class pOAccrualAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _POAccrualAcctID;
			[PXDBInt(BqlField = typeof(POReceiptLine.pOAccrualAcctID))]
			public virtual Int32? POAccrualAcctID
			{
				get
				{
					return this._POAccrualAcctID;
				}
				set
				{
					this._POAccrualAcctID = value;
				}
			}
			#endregion
			#region POAccrualSubID
			public abstract class pOAccrualSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _POAccrualSubID;
			[PXDBInt(BqlField = typeof(POReceiptLine.pOAccrualSubID))]
			public virtual Int32? POAccrualSubID
			{
				get
				{
					return this._POAccrualSubID;
				}
				set
				{
					this._POAccrualSubID = value;
				}
			}
			#endregion

			#region ReceiptExpenseAcctID
			public abstract class receiptExpenseAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _ReceiptExpenseAcctID;
			[PXDBInt(BqlField = typeof(POReceiptLine.expenseAcctID))]
			public virtual Int32? ReceiptExpenseAcctID
			{
				get
				{
					return this._ReceiptExpenseAcctID;
				}
				set
				{
					this._ReceiptExpenseAcctID = value;
				}
			}
			#endregion
			#region ReceiptExpenseSubID
			public abstract class receiptExpenseSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _ReceiptExpenseSubID;
			[PXDBInt(BqlField = typeof(POReceiptLine.expenseSubID))]
			public virtual Int32? ReceiptExpenseSubID
			{
				get
				{
					return this._ReceiptExpenseSubID;
				}
				set
				{
					this._ReceiptExpenseSubID = value;
				}
			}
			#endregion

			#region ReciptTranDesc
			public abstract class reciptTranDesc : PX.Data.IBqlField
			{
			}
			protected String _ReciptTranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(POReceiptLine.tranDesc))]
			[PXUIField(DisplayName = "Transaction Descr.")]
			public virtual String ReciptTranDesc
			{
				get
				{
					return this._ReciptTranDesc;
				}
				set
				{
					this._ReciptTranDesc = value;
				}
			}
			#endregion


			#region ReceiptVendorRefNbr
			public abstract class receiptVendorRefNbr : PX.Data.IBqlField
			{
			}
			protected String _ReceiptVendorRefNbr;
			[PXDBString(40, IsUnicode = true, BqlField = typeof(POReceipt.invoiceNbr))]
			[PXUIField(DisplayName = "Vendor Ref.")]
			public virtual String ReceiptVendorRefNbr
			{
				get
				{
					return this._ReceiptVendorRefNbr;
				}
				set
				{
					this._ReceiptVendorRefNbr = value;
				}
			}
			#endregion

		}
		#endregion

		#region POOrder list for link
		[PXProjection(typeof(
			Select2<POLine
				, LeftJoin<POOrder, On<POOrder.orderType, Equal<POLine.orderType>, And<POOrder.orderNbr, Equal<POLine.orderNbr>>>
					, LeftJoin<APTran
						, On<APTran.released, NotEqual<True>
							, And<APTran.pOOrderType, Equal<POLine.orderType>
								, And<APTran.pONbr, Equal<POLine.orderNbr>
									, And<APTran.pOLineNbr, Equal<POLine.lineNbr>
										>
									>
								>
							>
						>
					>
				, Where<POOrder.orderType, Equal<POOrderType.regularOrder>
					, And<POLine.completed, NotEqual<True>
						, And<POOrder.status, Equal<POOrderStatus.open>
							, And<APTran.refNbr, IsNull
								, And<POLine.lineType, Equal<POLineType.service>>
								>
							>
						>
					>
				>
			), Persistent = false)]
		[Serializable]
		public partial class LinkLineOrder : IBqlTable
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
			[POOrderType.List()]
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
			[PXDBString(15, IsKey = true, IsUnicode = true, BqlField = typeof(POLine.orderNbr))]
			[PXUIField(DisplayName = "Order Nbr.")]
			[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Current<orderType>>>>))]
			[PXUIVerify(typeof(Where<orderCuryID, Equal<Current<APInvoice.curyID>>>), PXErrorLevel.RowWarning, Messages.APDocumentCurrencyDiffersFromSourceDocument, true)]
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
			#region OrderLineNbr
			public abstract class orderLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderLineNbr;
			[PXUIField(DisplayName = "PO Line", Visible = false)]
			[PXDBInt(IsKey = true, BqlField = typeof(POLine.lineNbr))]
			public virtual Int32? OrderLineNbr
			{
				get
				{
					return this._OrderLineNbr;
				}
				set
				{
					this._OrderLineNbr = value;
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[PXDBInt(BqlField = typeof(POOrder.vendorID))]
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
			[PXDBInt(BqlField = typeof(POOrder.vendorLocationID))]
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


			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;


			[INUnit(typeof(POLine.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
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

			#region OrderBaseQty
			public abstract class orderBaseQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderBaseQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseOrderQty))]
			public virtual Decimal? OrderBaseQty
			{
				get
				{
					return this._OrderBaseQty;
				}
				set
				{
					this._OrderBaseQty = value;
				}
			}
			#endregion
			#region OrderQty
			public abstract class orderQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderQty;
			[PXDBQuantity(BqlField = typeof(POLine.orderQty))]
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
			#region OrderCuryInfoID
			public abstract class orderCuryInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _OrderCuryInfoID;

			[PXDBLong(BqlField = typeof(POLine.curyInfoID))]
			[CurrencyInfo(CuryIDField = "OrderCuryID")]
			public virtual Int64? OrderCuryInfoID
			{
				get
				{
					return this._OrderCuryInfoID;
				}
				set
				{
					this._OrderCuryInfoID = value;
				}
			}
			#endregion
			#region OrderAmount
			public abstract class orderAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderAmount;
			[PXDBDecimal(BqlField = typeof(POLine.lineAmt))]
			public virtual Decimal? OrderAmount
			{
				get
				{
					return this._OrderAmount;
				}
				set
				{
					this._OrderAmount = value;
				}
			}
			#endregion
			#region OrderCuryAmount
			public abstract class orderCuryAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderCuryAmount;
			[PXDBCurrency(typeof(orderCuryInfoID), typeof(orderAmount), BqlField = typeof(POLine.curyLineAmt))]
			[PXUIField(DisplayName = "Amount")]
			public virtual Decimal? OrderCuryAmount
			{
				get
				{
					return this._OrderCuryAmount;
				}
				set
				{
					this._OrderCuryAmount = value;
				}
			}
			#endregion
			#region OrderCuryID
			public abstract class orderCuryID : PX.Data.IBqlField
			{
			}
			protected String _OrderCuryID;
			[PXUIField(DisplayName = "Currency")]
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POOrder.curyID))]
			[PXSelector(typeof(Currency.curyID))]
			public virtual String OrderCuryID
			{
				get
				{
					return this._OrderCuryID;
				}
				set
				{
					this._OrderCuryID = value;
				}
			}
			#endregion

			#region OrderOrderBilledQty
			public abstract class orderBilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderBilledQty;
			[PXDBQuantity(typeof(uOM), typeof(orderBaseBilledQty), HandleEmptyKey = true, BqlField = typeof(POLine.voucheredQty))]
			[PXUIField(DisplayName = "Billed Qty.", Enabled = false)]
			public virtual Decimal? OrderBilledQty
			{
				get
				{
					return this._OrderBilledQty;
				}
				set
				{
					this._OrderBilledQty = value;
				}
			}

			#endregion
			#region OrderBaseBilledQty
			public abstract class orderBaseBilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderBaseBilledQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseVoucheredQty))]
			public virtual Decimal? OrderBaseBilledQty
			{
				get
				{
					return this._OrderBaseBilledQty;
				}
				set
				{
					this._OrderBaseBilledQty = value;
				}
			}
			#endregion
			#region OrderCuryBilledAmt
			public abstract class orderCuryBilledAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrderCuryBilledAmt;
			[PXDBCurrency(typeof(orderCuryInfoID), typeof(orderBilledAmt), BqlField = typeof(POLine.curyVoucheredCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Billed Amount", Enabled = false)]
			public virtual Decimal? OrderCuryBilledAmt
			{
				get
				{
					return this._OrderCuryBilledAmt;
				}
				set
				{
					this._OrderCuryBilledAmt = value;
				}
			}

			#endregion
			#region OrderBilledAmt
			public abstract class orderBilledAmt : PX.Data.IBqlField
			{
			}

			protected Decimal? _OrderBilledAmt;
			[PXDBBaseCury(BqlField = typeof(POLine.voucheredCost))]
			public virtual Decimal? OrderBilledAmt
			{
				get
				{
					return this._OrderBilledAmt;
				}
				set
				{
					this._OrderBilledAmt = value;
				}
			}
			#endregion


			#region OrderOrderUnbilledQty
			public abstract class orderUnbilledQty : PX.Data.IBqlField
			{
			}
			[PXQuantity(typeof(uOM), typeof(orderBaseUnbilledQty), HandleEmptyKey = true)]
			[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
			public virtual Decimal? OrderUnbilledQty
			{
				get
				{
					return this._OrderQty - this._OrderBilledQty;
				}
			}

			#endregion
			#region OrderBaseUnbilledQty
			public abstract class orderBaseUnbilledQty : PX.Data.IBqlField
			{
			}
			[PXDecimal(6)]
			public virtual Decimal? OrderBaseUnbilledQty
			{
				get
				{
					return this._OrderBaseQty - this._OrderBaseBilledQty;
				}
			}
			#endregion
			#region OrderCuryUnbilledAmt
			public abstract class orderCuryUnbilledAmt : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(orderCuryInfoID), typeof(orderUnbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Enabled = false)]
			public virtual Decimal? OrderCuryUnbilledAmt
			{
				get
				{
					return this._OrderCuryAmount - this._OrderCuryBilledAmt;
				}
			}

			#endregion
			#region OrderUnbilledAmt
			public abstract class orderUnbilledAmt : PX.Data.IBqlField
			{
			}

			[PXBaseCury()]
			public virtual Decimal? OrderUnbilledAmt
			{
				get
				{
					return this._OrderAmount - this._OrderBilledAmt;
				}
			}
			#endregion
			#region OrderSiteID
			public abstract class orderSiteID : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderSiteID;
			[Site(BqlField = typeof(POLine.siteID))]
			public virtual Int32? OrderSiteID
			{
				get
				{
					return this._OrderSiteID;
				}
				set
				{
					this._OrderSiteID = value;
				}
			}
			#endregion
			#region OrderSubItemID
			public abstract class orderSubItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderSubItemID;
			[SubItem(typeof(inventoryID), BqlField = typeof(POLine.subItemID))]
			public virtual Int32? OrderSubItemID
			{
				get
				{
					return this._OrderSubItemID;
				}
				set
				{
					this._OrderSubItemID = value;
				}
			}
			#endregion
			#region OrderExpenseAcctID
			public abstract class rrderExpenseAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderExpenseAcctID;
			[PXDBInt(BqlField = typeof(POLine.expenseAcctID))]
			public virtual Int32? OrderExpenseAcctID
			{
				get
				{
					return this._OrderExpenseAcctID;
				}
				set
				{
					this._OrderExpenseAcctID = value;
				}
			}
			#endregion
			#region OrderExpenseSubID
			public abstract class rrderExpenseSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _OrderExpenseSubID;
			[PXDBInt(BqlField = typeof(POLine.expenseSubID))]
			public virtual Int32? OrderExpenseSubID
			{
				get
				{
					return this._OrderExpenseSubID;
				}
				set
				{
					this._OrderExpenseSubID = value;
				}
			}
			#endregion
			#region OrderTranDesc
			public abstract class orderTranDesc : PX.Data.IBqlField
			{
			}
			protected String _OrderTranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(POLine.tranDesc))]
			[PXUIField(DisplayName = "Transaction Descr.")]
			public virtual String OrderTranDesc
			{
				get
				{
					return this._OrderTranDesc;
				}
				set
				{
					this._OrderTranDesc = value;
				}
			}
			#endregion

			#region VendorRefNbr
			public abstract class vendorRefNbr : PX.Data.IBqlField
			{
			}
			protected String _VendorRefNbr;
			[PXDBString(40, IsUnicode = true, BqlField = typeof(POOrder.vendorRefNbr))]
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

		}
		#endregion
		#region Receipt +Receipt Line Selection

		[Serializable]
		public partial class POReceiptFilter : IBqlTable
		{
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}

			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
			[PXDefault(typeof(APInvoice.vendorID))]
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

			/*
			#region ReceiptType
			public abstract class receiptType : PX.Data.IBqlField
			{
			}
			protected String _ReceiptType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(POReceiptType.POReceipt)]			
			[POReceiptType.List]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual String ReceiptType
			{
				get
				{
					return this._ReceiptType;
				}
				set
				{
					this._ReceiptType = value;
				}
			}
			#endregion
			#region ReceiptNbr
			public abstract class receiptNbr : PX.Data.IBqlField
			{
			}
			protected String _ReceiptNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXDefault()]
			[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PXSelector(typeof(Search2<POReceiptS.receiptNbr,
			InnerJoin<Vendor, On<POReceiptS.vendorID, Equal<Vendor.bAccountID>>>,
			Where<POReceiptS.vendorID, Equal<Current<APInvoice.vendorID>>,
				And<POReceiptS.vendorLocationID, Equal<Optional<APInvoice.vendorLocationID>>,				
				And<POReceiptS.curyID, Equal<Optional<APInvoice.curyID>>,
				And<Where2<Where<POReceiptS.receiptType, Equal<POReceiptType.poreceipt>,And<Optional<APInvoice.docType>, Equal<APInvoiceType.invoice>>>,
				   Or<Where<POReceiptS.receiptType, Equal<POReceiptType.poreturn>, 
				 And<Optional<APInvoice.docType>, Equal<APInvoiceType.debitAdj>>>>>
				>>>>>),
				typeof(POReceiptS.receiptNbr),
				typeof(POReceiptS.receiptDate),
				typeof(POReceiptS.vendorID),
				typeof(POReceiptS.vendorID_Vendor_acctName),
				typeof(POReceiptS.vendorLocationID),
				typeof(POReceiptS.curyID),
				typeof(POReceiptS.curyUnbilledAmtL), Filterable = true)]
			public virtual String ReceiptNbr
			{
				get
				{
					return this._ReceiptNbr;
				}
				set
				{
					this._ReceiptNbr = value;
				}
			}
			#endregion
			*/
			#region OrderNbr
			public abstract class orderNbr : PX.Data.IBqlField
			{
			}
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PXSelector(typeof(Search5<POReceiptLineS.pONbr,
				InnerJoin<POReceipt, On<POReceipt.receiptNbr, Equal<POReceiptLineS.receiptNbr>>,
				InnerJoin<POOrder, On<POOrder.orderNbr, Equal<POReceiptLineS.pONbr>>,
				LeftJoin<APTran, On<APTran.receiptNbr, Equal<POReceiptLineS.receiptNbr>,
					And<APTran.receiptLineNbr, Equal<POReceiptLineS.lineNbr>,
					And<APTran.released, Equal<False>>>>>>>,
				Where<POReceipt.vendorID, Equal<Current<APInvoice.vendorID>>,
					 And<POReceipt.vendorLocationID, Equal<Optional<APInvoice.vendorLocationID>>,
					 And<POReceipt.hold, Equal<False>,
					 And<POReceipt.released, Equal<True>,
					 And<APTran.refNbr, IsNull,
					 And2<Where<POReceiptLineS.unbilledQty, Greater<decimal0>,
					 Or<POReceiptLineS.curyUnbilledAmt, NotEqual<decimal0>>>,
					 And<Where<POReceiptLineS.receiptType, Equal<POReceiptType.poreceipt>, And<Optional<APInvoice.docType>, Equal<APInvoiceType.invoice>,
									Or<POReceiptLineS.receiptType, Equal<POReceiptType.poreturn>, And<Optional<APInvoice.docType>, Equal<APInvoiceType.debitAdj>>>>>>>>>>>>,
				Aggregate<GroupBy<POReceiptLineS.pONbr>>>),
			   typeof(POReceiptLineS.pONbr),
			   typeof(POOrder.orderDate),
			   typeof(POOrder.vendorID),
			   typeof(POOrder.vendorID_Vendor_acctName),
			   typeof(POOrder.vendorLocationID),
			   typeof(POOrder.curyID),
			   typeof(POOrder.curyOrderTotal), Filterable = true)]
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

		[PXProjection(typeof(Select5<POReceipt,
										InnerJoin<POReceiptLineS, On<POReceiptLineS.receiptNbr, Equal<POReceipt.receiptNbr>,
												And<Where<POReceiptLineS.unbilledQty, Greater<decimal0>,
													Or<POReceiptLineS.curyUnbilledAmt, NotEqual<decimal0>>>>>,
										LeftJoin<APTran, On<APTran.receiptNbr, Equal<POReceiptLineS.receiptNbr>,
														And<APTran.receiptLineNbr, Equal<POReceiptLineS.lineNbr>,
														And<APTran.released, Equal<False>>>>>>,
										Where<POReceipt.released, Equal<True>,
											And<APTran.refNbr, IsNull>>,
											Aggregate
												<GroupBy<POReceipt.receiptNbr,
												GroupBy<POReceipt.receiptDate,
												GroupBy<POReceipt.curyID,
												GroupBy<POReceipt.curyOrderTotal,
												GroupBy<POReceipt.hold,
												Sum<POReceiptLineS.receiptQty,
												Sum<POReceiptLineS.curyUnbilledAmt,
												Sum<POReceiptLineS.unbilledQty,
												Sum<POReceiptLineS.curyExtCost
												>>>>>>>>>>>), Persistent = false)]
		[Serializable]
		[PXHidden]
		public partial class POReceiptS : IBqlTable
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
			#region ReceiptNbr
			public abstract class receiptNbr : PX.Data.IBqlField
			{
			}
			protected String _ReceiptNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POReceipt.receiptNbr))]
			[POReceiptType.RefNbr(typeof(Search<POReceiptS.receiptNbr>), Filterable = true)]
			[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String ReceiptNbr
			{
				get
				{
					return this._ReceiptNbr;
				}
				set
				{
					this._ReceiptNbr = value;
				}
			}
			#endregion
			#region ReceiptType
			public abstract class receiptType : PX.Data.IBqlField
			{
			}
			protected String _ReceiptType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(POReceipt.receiptType))]
			[POReceiptType.List()]
			public virtual String ReceiptType
			{
				get
				{
					return this._ReceiptType;
				}
				set
				{
					this._ReceiptType = value;
				}
			}
			#endregion
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(POReceipt.vendorID))]
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
			[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POReceipt.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(POReceipt.vendorLocationID))]


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
			#region ReceiptDate
			public abstract class receiptDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ReceiptDate;
			[PXDBDate(BqlField = typeof(POReceipt.receiptDate))]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual DateTime? ReceiptDate
			{
				get
				{
					return this._ReceiptDate;
				}
				set
				{
					this._ReceiptDate = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.IBqlField
			{
			}
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(POReceipt.hold))]
			[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]

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
			#region Released
			public abstract class released : PX.Data.IBqlField
			{
			}
			protected Boolean? _Released;
			[PXDBBool(BqlField = typeof(POReceipt.released))]
			[PXUIField(DisplayName = "Released")]
			[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual Boolean? Released
			{
				get
				{
					return this._Released;
				}
				set
				{
					this._Released = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POReceipt.curyID))]
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

			#region VoucheredQty
			public abstract class voucheredQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _VoucheredQty;
			[PXDBQuantity(BqlField = typeof(POReceiptLine.voucheredQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Vouchered Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? VoucheredQty
			{
				get
				{
					return this._VoucheredQty;
				}
				set
				{
					this._VoucheredQty = value;
				}
			}
			#endregion
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			protected Int64? _CuryInfoID;
			[PXDBLong(BqlField = typeof(POReceiptLine.curyInfoID))]
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
			#region CuryVoucheredCost
			public abstract class curyVoucheredCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryVoucheredCost;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.curyVoucheredCost))]
			[PXUIField(DisplayName = "Vouchered Amt", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryVoucheredCost
			{
				get
				{
					return this._CuryVoucheredCost;
				}
				set
				{
					this._CuryVoucheredCost = value;
				}
			}
			#endregion
			#region VoucheredCost
			public abstract class voucheredCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _VoucheredCost;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.voucheredCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Vouchered Cost")]
			public virtual Decimal? VoucheredCost
			{
				get
				{
					return this._VoucheredCost;
				}
				set
				{
					this._VoucheredCost = value;
				}
			}
			#endregion

			#region UnbilledQty
			public abstract class unbilledQtyL : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnbilledQtyL;
			[PXDBQuantity(BqlField = typeof(POReceiptLineS.unbilledQty))]
			[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
			public virtual Decimal? UnbilledQtyL
			{
				get
				{
					return this._UnbilledQtyL;
				}
				set
				{
					this._UnbilledQtyL = value;
				}
			}
			#endregion
			#region CuryUnbilledAmt
			public abstract class curyUnbilledAmtL : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryUnbilledAmtL;
			[PXDBCurrency(typeof(POReceiptS.curyInfoID), typeof(POReceiptS.unbilledAmt), BqlField = typeof(POReceiptLineS.curyUnbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? CuryUnbilledAmtL
			{
				get
				{
					return this._CuryUnbilledAmtL;
				}
				set
				{
					this._CuryUnbilledAmtL = value;
				}
			}
			#endregion
			#region UnbilledAmt
			public abstract class unbilledAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnbilledAmt;
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.unbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? UnbilledAmt
			{
				get
				{
					return this._UnbilledAmt;
				}
				set
				{
					this._UnbilledAmt = value;
				}
			}
			#endregion
			#region VendorID_Vendor_acctName
			public abstract class vendorID_Vendor_acctName : PX.Data.IBqlField
			{
			}
			#endregion
			#region CuryOrderTotal
			public abstract class curyOrderTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryOrderTotal;

			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POReceiptS.curyInfoID), typeof(POReceiptS.orderTotal), BqlField = typeof(POReceipt.curyOrderTotal))]
			[PXUIField(DisplayName = "Total Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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

			[PXDBBaseCury(BqlField = typeof(POReceipt.orderTotal))]
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
			[PXDBQuantity(BqlField = typeof(POReceipt.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Total Qty.")]
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
			#region CuryLineTotal
			public abstract class curyLineTotal : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryLineTotal;

			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POReceiptS.curyInfoID), typeof(POReceiptS.lineTotal), BqlField = typeof(POReceipt.curyLineTotal))]
			[PXUIField(DisplayName = "Lines Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
			[PXDBBaseCury(BqlField = typeof(POReceipt.lineTotal))]
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

		//Read-only class for selector
		[PXProjection(typeof(Select<POReceiptLine>), Persistent = false)]
		[Serializable]
		public partial class POReceiptLineS : IBqlTable, IAPTranSource
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
			[Branch(BqlField = typeof(POReceiptLine.branchID))]
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
			#region ReceiptNbr
			public abstract class receiptNbr : PX.Data.IBqlField
			{
			}
			protected String _ReceiptNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POReceiptLine.receiptNbr))]
			[PXUIField(DisplayName = "Receipt Nbr.")]
			public virtual String ReceiptNbr
			{
				get
				{
					return this._ReceiptNbr;
				}
				set
				{
					this._ReceiptNbr = value;
				}
			}
			#endregion
			#region ReceiptType
			public abstract class receiptType : PX.Data.IBqlField
			{
			}
			protected String _ReceiptType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.receiptType))]
			[PXUIField(DisplayName = "Receipt Type")]
			public virtual String ReceiptType
			{
				get
				{
					return this._ReceiptType;
				}
				set
				{
					this._ReceiptType = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(POReceiptLine.lineNbr))]
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
			[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.lineType))]
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
			[Inventory(Filterable = true, BqlField = typeof(POReceiptLine.inventoryID))]
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
			[Vendor(BqlField = typeof(POReceiptLine.vendorID))]
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
			#region ReceiptDate
			public abstract class receiptDate : PX.Data.IBqlField
			{
			}

			protected DateTime? _ReceiptDate;
			[PXDBDate(BqlField = typeof(POReceiptLine.receiptDate))]
			public virtual DateTime? ReceiptDate
			{
				get
				{
					return this._ReceiptDate;
				}
				set
				{
					this._ReceiptDate = value;
				}
			}
			#endregion
			#region SubItemID
			public abstract class subItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _SubItemID;
			[SubItem(typeof(POReceiptLineS.inventoryID), BqlField = typeof(POReceiptLine.subItemID))]
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
			[IN.SiteAvail(typeof(POReceiptLineS.inventoryID), typeof(POReceiptLineS.subItemID), BqlField = typeof(POReceiptLine.siteID))]
			[PXDefault()]
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
			[INUnit(typeof(POReceiptLineS.inventoryID), BqlField = typeof(POReceiptLine.uOM))]
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
			#region ReceiptQty
			public abstract class receiptQty : PX.Data.IBqlField
			{
			}

			protected Decimal? _ReceiptQty;
			[PXDBQuantity(typeof(POReceiptLineS.uOM), typeof(POReceiptLineS.baseReceiptQty), HandleEmptyKey = true, BqlField = typeof(POReceiptLine.receiptQty))]
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

			#endregion
			#region BaseReceiptQty
			public abstract class baseReceiptQty : PX.Data.IBqlField
			{
			}

			protected Decimal? _BaseReceiptQty;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.baseReceiptQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
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

			public virtual Decimal? BaseQty
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
			[PXDBLong(BqlField = typeof(POReceiptLine.curyInfoID))]
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

			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.curyUnitCost))]
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

			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.unitCost))]
			[PXDefault(TypeCode.Decimal, "0.0")]
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

			#region CuryExtCost
			public abstract class curyExtCost : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryExtCost;
			[PXDBCurrency(typeof(POReceiptLineS.curyInfoID), typeof(POReceiptLineS.extCost), BqlField = typeof(POReceiptLine.curyExtCost))]
			[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
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
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.extCost))]
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

			#region UnbilledQty
			public abstract class unbilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _UnbilledQty;
			[PXDBQuantity(typeof(POReceiptLineS.uOM), typeof(POReceiptLineS.baseUnbilledQty), HandleEmptyKey = true, BqlField = typeof(POReceiptLine.unbilledQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
			public virtual Decimal? UnbilledQty
			{
				get
				{
					return this._UnbilledQty;
				}
				set
				{
					this._UnbilledQty = value;
				}
			}

			public virtual Decimal? BillQty
			{
				get
				{
					return this._UnbilledQty;
				}
			}
			#endregion
			#region BaseUnbilledQty
			public abstract class baseUnbilledQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _BaseUnbilledQty;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.baseUnbilledQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? BaseUnbilledQty
			{
				get
				{
					return this._BaseUnbilledQty;
				}
				set
				{
					this._BaseUnbilledQty = value;
				}
			}
			#endregion
			#region CuryUnbilledAmt
			public abstract class curyUnbilledAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryUnbilledAmt;
			[PXDBCurrency(typeof(POReceiptLineS.curyInfoID), typeof(POReceiptLineS.unbilledAmt), BqlField = typeof(POReceiptLine.curyUnbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Enabled = false)]
			public virtual Decimal? CuryUnbilledAmt
			{
				get
				{
					return this._CuryUnbilledAmt;
				}
				set
				{
					this._CuryUnbilledAmt = value;
				}
			}

			#endregion

			#region CuryUnbilledDiscountAmt
			public abstract class curyUnbilledDiscountAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryUnbilledDiscountAmt;
			[PXDBCurrency(typeof(POReceiptLineS.curyInfoID), typeof(POReceiptLineS.unbilledDiscountAmt), BqlField = typeof(POReceiptLine.curyUnbilledDiscountAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Enabled = false)]
			public virtual Decimal? CuryUnbilledDiscountAmt
			{
				get
				{
					return this._CuryUnbilledDiscountAmt;
				}
				set
				{
					this._CuryUnbilledDiscountAmt = value;
				}
			}

			#endregion
			public virtual Decimal? CuryDiscAmt
			{
				get
				{
					return this._CuryUnbilledDiscountAmt;
				}
				set
				{
					this._CuryUnbilledDiscountAmt = value;
				}
			}
			#region DiscPct
			public abstract class discPct : PX.Data.IBqlField
			{
			}
			protected Decimal? _DiscPct;
			[PXDBDecimal(6, MinValue = -100, MaxValue = 100, BqlField = typeof(POReceiptLine.discPct))]
			[PXUIField(DisplayName = "Discount Percent")]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? DiscPct
			{
				get
				{
					return this._DiscPct;
				}
				set
				{
					this._DiscPct = value;
				}
			}
			#endregion
			public virtual Decimal? DiscAmt
			{
				get
				{
					return this._UnbilledDiscountAmt;
				}
				set
				{
					this._UnbilledDiscountAmt = value;
				}
			}
			public virtual Decimal? CuryLineAmt
			{
				get
				{
					return this._CuryUnbilledAmt;
				}
				set
				{
					this._CuryUnbilledAmt = value;
				}
			}
			public virtual Decimal? LineAmt
			{
				get
				{
					return this._UnbilledAmt;
				}
				set
				{
					this._UnbilledAmt = value;
				}
			}

			#region UnbilledDiscountAmt
			public abstract class unbilledDiscountAmt : PX.Data.IBqlField
			{
			}

			protected Decimal? _UnbilledDiscountAmt;
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.unbilledDiscountAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? UnbilledDiscountAmt
			{
				get
				{
					return this._UnbilledDiscountAmt;
				}
				set
				{
					this._UnbilledDiscountAmt = value;
				}
			}
			#endregion
			#region UnbilledAmt
			public abstract class unbilledAmt : PX.Data.IBqlField
			{
			}

			protected Decimal? _UnbilledAmt;
			[PXDBBaseCury(BqlField = typeof(POReceiptLine.unbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? UnbilledAmt
			{
				get
				{
					return this._UnbilledAmt;
				}
				set
				{
					this._UnbilledAmt = value;
				}
			}
			#endregion

			#region GroupDiscountRate
			public abstract class groupDiscountRate : PX.Data.IBqlField
			{
			}
			protected Decimal? _GroupDiscountRate;
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.groupDiscountRate))]
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
			[PXDBDecimal(6, BqlField = typeof(POReceiptLine.documentDiscountRate))]
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

			[PXDBString(10, IsUnicode = true, BqlField = typeof(POReceiptLine.taxCategoryID))]
			[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
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

			[PXDBInt(BqlField = typeof(POReceiptLine.expenseAcctID))]
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

			[PXDBInt(BqlField = typeof(POReceiptLine.expenseSubID))]
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
			#region POAccrualAcctID
			public abstract class pOAccrualAcctID : PX.Data.IBqlField
			{
			}
			protected Int32? _POAccrualAcctID;
			[PXDBInt(BqlField = typeof(POReceiptLine.pOAccrualAcctID))]
			public virtual Int32? POAccrualAcctID
			{
				get
				{
					return this._POAccrualAcctID;
				}
				set
				{
					this._POAccrualAcctID = value;
				}
			}
			#endregion
			#region POAccrualSubID
			public abstract class pOAccrualSubID : PX.Data.IBqlField
			{
			}
			protected Int32? _POAccrualSubID;
			[PXDBInt(BqlField = typeof(POReceiptLine.pOAccrualSubID))]
			public virtual Int32? POAccrualSubID
			{
				get
				{
					return this._POAccrualSubID;
				}
				set
				{
					this._POAccrualSubID = value;
				}
			}
			#endregion
			#region TranDesc
			public abstract class tranDesc : PX.Data.IBqlField
			{
			}
			protected String _TranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(POReceiptLine.tranDesc))]
			[PXUIField(DisplayName = "Transaction Descr.", Visibility = PXUIVisibility.Visible)]
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
			#region TaxID
			public abstract class taxID : PX.Data.IBqlField
			{
			}
			protected String _TaxID;
			[PXDBString(Tax.taxID.Length, IsUnicode = true, BqlField = typeof(POReceiptLine.taxID))]
			[PXUIField(DisplayName = "Tax ID", Visible = false)]			
			public virtual String TaxID
			{
				get
				{
					return this._TaxID;
				}
				set
				{
					this._TaxID = value;
				}
			}
			#endregion

			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected int? _ProjectID;
			[PXDBInt(BqlField = typeof(POReceiptLine.projectID))]
			public virtual int? ProjectID
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
			protected int? _TaskID;
			[PXDBInt(BqlField = typeof(POReceiptLine.taskID))]
			public virtual int? TaskID
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
		

			#region POType
			public abstract class pOType : PX.Data.IBqlField
			{
			}
			protected String _POType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.pOType))]
			[POOrderType.List()]
			[PXUIField(DisplayName = "Order Type", Enabled = false)]
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
			[PXDBString(15, IsUnicode = true, BqlField = typeof(POReceiptLine.pONbr))]
			[PXUIField(DisplayName = "Order Nbr.")]
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
			[PXDBInt(BqlField = typeof(POReceiptLine.pOLineNbr))]
			[PXUIField(DisplayName = "PO Line Nbr.")]
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

			#region DiscountID
			public abstract class discountID : PX.Data.IBqlField
			{
			}
			protected String _DiscountID;
			[PXDBString(10, IsUnicode = true, BqlField = typeof(POReceiptLine.discountID))]
			public virtual String DiscountID
			{
				get
				{
					return this._DiscountID;
				}
				set
				{
					this._DiscountID = value;
				}
			}
			#endregion

			#region DiscountID
			public abstract class discountSequenceID : PX.Data.IBqlField
			{
			}
			protected String _DiscountSequenceID;
			[PXDBString(10, IsUnicode = true, BqlField = typeof(POReceiptLine.discountSequenceID))]
			public virtual String DiscountSequenceID
			{
				get
				{
					return this._DiscountSequenceID;
				}
				set
				{
					this._DiscountSequenceID = value;
				}
			}
			#endregion

			#region TotalCuryUnbilledAmount
			public abstract class totalCuryUnbilledAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _TotalCuryUnbilledAmount;
			[PXCurrency(typeof(curyInfoID), typeof(totalUnbilledAmount))]
			[PXFormula(typeof(Sub<curyUnbilledAmt, curyUnbilledDiscountAmt>))]
			[PXUIField(DisplayName = "Unbilled Amount")]
			public virtual Decimal? TotalCuryUnbilledAmount
			{
				get
				{
					return this._TotalCuryUnbilledAmount;
				}
				set
				{
					this._TotalCuryUnbilledAmount = value;
				}
			}
			#endregion

			#region TotalUnbilledAmount
			public abstract class totalUnbilledAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _TotalUnbilledAmount;
			[PXBaseCury()]
			[PXFormula(typeof(Sub<unbilledAmt, unbilledDiscountAmt>))]
			public virtual Decimal? TotalUnbilledAmount
			{
				get
				{
					return this._TotalUnbilledAmount;
				}
				set
				{
					this._TotalUnbilledAmount = value;
				}
			}
			#endregion



			public virtual bool CompareReferenceKey(APTran aTran)
			{
				return (aTran.ReceiptNbr == this.ReceiptNbr && aTran.ReceiptLineNbr == this.LineNbr);
			}
			public virtual void SetReferenceKeyTo(APTran aTran)
			{
				aTran.ReceiptType = this.ReceiptType;
				aTran.ReceiptNbr = this.ReceiptNbr;
				aTran.ReceiptLineNbr = this.LineNbr;
				aTran.POOrderType = this.POType;
				aTran.PONbr = this.PONbr;
				aTran.POLineNbr = this.POLineNbr;
			}

			public virtual bool IsReturn
			{
				get { return _ReceiptType == POReceiptType.POReturn; }
		}

		}

		public partial class POReceiptLineAdd : POReceiptLineS
		{
		}

		#endregion

		#region POOrder + Unbilled Service Items Projection
		[Serializable]
		public partial class POOrderRS : POOrder
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
			#region CuryLeftToReceiveCost
			public abstract class curyLeftToBillCost : PX.Data.IBqlField
			{
			}

			public Decimal? _CuryLeftToBillCost;
			[PXDecimal(6)]
			[PXUIField(DisplayName = "Unbilled Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryLeftToBillCost
			{
				get
				{
					return _CuryLeftToBillCost;
				}
				set
				{
					_CuryLeftToBillCost = value;
				}
			}
			#endregion
			#region LeftToBillQty
			public abstract class leftToReceiveQty : PX.Data.IBqlField
			{
			}
			public Decimal? _LeftToBillQty;
			[PXQuantity()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? LeftToBillQty
			{
				get 
				{ 
					return _LeftToBillQty;
				}
				set
				{
					_LeftToBillQty = value;
				}
			}
			#endregion
			#region CuryInfoID
			public new abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			#endregion

		}
		#endregion

		[Serializable]
		public partial class POLineS : POLine
		{
			#region OrderOrderUnbilledQty
			public abstract class orderUnbilledQty : PX.Data.IBqlField
			{
			}
			[PXQuantity(typeof(uOM), typeof(orderBaseUnbilledQty), HandleEmptyKey = true)]
			[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
			public virtual Decimal? OrderUnbilledQty
			{
				get
				{
					return this._OrderQty - this._VoucheredQty;
				}
			}
			#endregion
			#region OrderBaseUnbilledQty
			public abstract class orderBaseUnbilledQty : PX.Data.IBqlField
			{
			}
			[PXDecimal(6)]
			public virtual Decimal? OrderBaseUnbilledQty
			{
				get
				{
					return this._BaseOrderQty - this._BaseVoucheredQty;
				}
			}
			#endregion

			#region OrderCuryUnbilledAmt
			public abstract class orderCuryUnbilledAmt : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(curyInfoID), typeof(orderUnbilledAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unbilled Amount", Enabled = false)]
			public virtual Decimal? OrderCuryUnbilledAmt
			{
				get
				{
					return this._CuryLineAmt - this._CuryVoucheredCost;
				}
			}
			#endregion
			#region OrderUnbilledAmt
			public abstract class orderUnbilledAmt : PX.Data.IBqlField
			{
			}

			[PXBaseCury()]
			public virtual Decimal? OrderUnbilledAmt
			{
				get
				{
					return this._LineAmt - this._VoucheredCost;
				}
			}
			#endregion

		}

		#endregion

		#region APAdjust
		[Serializable]        
		public partial class APAdjust : PX.Objects.AP.APAdjust
		{
			#region VendorID
			public new abstract class vendorID : PX.Data.IBqlField
			{
			}
			[PXDBInt]
			[PXDBDefault(typeof(AP.APInvoice.vendorID))]
			[PXUIField(DisplayName = "Vendor ID", Visibility = PXUIVisibility.Visible, Visible = false)]
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
			#region AdjgDocType
			public new abstract class adjgDocType : PX.Data.IBqlField
			{
			}
			[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
			[APPaymentType.List()]
			[PXDefault()]
			[PXUIField(DisplayName = "Doc. Type", Enabled = false)]
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
			[PXDBString(15, IsUnicode = true, IsKey = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Reference Nbr.", Enabled = false)]
			[APPaymentType.AdjgRefNbr(typeof(Search<APPayment.refNbr, Where<APPayment.docType, Equal<Optional<APAdjust.adjgDocType>>>>), Filterable = true)]
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
			#region AdjgBranchID
			public new abstract class adjgBranchID : PX.Data.IBqlField
			{
			}
			[Branch(null)]
			public override Int32? AdjgBranchID
			{
				get
				{
					return this._AdjgBranchID;
				}
				set
				{
					this._AdjgBranchID = value;
				}
			}
			#endregion
			#region AdjdCuryInfoID
			public new abstract class adjdCuryInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			[CurrencyInfo(typeof(AP.APInvoice.curyInfoID), ModuleCode = "AP", CuryIDField = "AdjdCuryID")]
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
			#region AdjdDocType
			public new abstract class adjdDocType : PX.Data.IBqlField
			{
			}
			[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
			[PXDBDefault(typeof(AP.APInvoice.docType))]
			[PXUIField(DisplayName = "Document Type", Visibility = PXUIVisibility.Invisible, Visible = false)]
			public override String AdjdDocType
			{
				get
				{
					return this._AdjdDocType;
				}
				set
				{
					this._AdjdDocType = value;
				}
			}
			#endregion
			#region AdjdRefNbr
			public new abstract class adjdRefNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true, IsKey = true)]
			[PXDBDefault(typeof(AP.APInvoice.refNbr))]
			[PXParent(typeof(Select<AP.APInvoice, Where<AP.APInvoice.docType, Equal<Current<APAdjust.adjdDocType>>, And<AP.APInvoice.refNbr, Equal<Current<APAdjust.adjdRefNbr>>>>>))]
			[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
			public override String AdjdRefNbr
			{
				get
				{
					return this._AdjdRefNbr;
				}
				set
				{
					this._AdjdRefNbr = value;
				}
			}
			#endregion
			#region AdjdBranchID
			public new abstract class adjdBranchID : PX.Data.IBqlField
			{
			}
			[Branch(typeof(AP.APInvoice.branchID))]
			public override Int32? AdjdBranchID
			{
				get
				{
					return this._AdjdBranchID;
				}
				set
				{
					this._AdjdBranchID = value;
				}
			}
			#endregion
			#region AdjNbr
			public new abstract class adjNbr : PX.Data.IBqlField
			{
			}
			[PXDBInt(IsKey = true)]
			[PXUIField(DisplayName = "Adjustment Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
			[PXDefault()]
			public override Int32? AdjNbr
			{
				get
				{
					return this._AdjNbr;
				}
				set
				{
					this._AdjNbr = value;
				}
			}
			#endregion
			#region StubNbr
			public new abstract class stubNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true)]
			public override String StubNbr
			{
				get
				{
					return this._StubNbr;
				}
				set
				{
					this._StubNbr = value;
				}
			}
			#endregion
			#region AdjBatchNbr
			public new abstract class adjBatchNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "Batch Number", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
			public override String AdjBatchNbr
			{
				get
				{
					return this._AdjBatchNbr;
				}
				set
				{
					this._AdjBatchNbr = value;
				}
			}
			#endregion
			#region AdjdOrigCuryInfoID
			public new abstract class adjdOrigCuryInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong()]
			[PXDBDefault(typeof(AP.APInvoice.curyInfoID))]
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
			[CurrencyInfo(ModuleCode = "AP", CuryIDField = "AdjgCuryID")]
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
			#region AdjgDocDate
			public new abstract class adjgDocDate : PX.Data.IBqlField
			{
			}
			[PXDBDate()]
			[PXDBDefault(typeof(AP.APInvoice.docDate))]
			[PXUIField(DisplayName = "Transaction Date")]
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
			#region AdjgFinPeriodID
			public new abstract class adjgFinPeriodID : PX.Data.IBqlField
			{
			}
			[GL.FinPeriodID()]
			[PXDBDefault(typeof(AP.APInvoice.finPeriodID))]
			[PXUIField(DisplayName = "Application Period")]
			public override String AdjgFinPeriodID
			{
				get
				{
					return this._AdjgFinPeriodID;
				}
				set
				{
					this._AdjgFinPeriodID = value;
				}
			}
			#endregion
			#region AdjgTranPeriodID
			public new abstract class adjgTranPeriodID : PX.Data.IBqlField
			{
			}
			[GL.FinPeriodID()]
			[PXDBDefault(typeof(AP.APInvoice.tranPeriodID))]
			public override String AdjgTranPeriodID
			{
				get
				{
					return this._AdjgTranPeriodID;
				}
				set
				{
					this._AdjgTranPeriodID = value;
				}
			}
			#endregion
			#region AdjdDocDate
			public new abstract class adjdDocDate : PX.Data.IBqlField
			{
			}
			[PXDBDate()]
			[PXDBDefault(typeof(AP.APInvoice.docDate))]
			[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override DateTime? AdjdDocDate
			{
				get
				{
					return this._AdjdDocDate;
				}
				set
				{
					this._AdjdDocDate = value;
				}
			}
			#endregion
			#region AdjdFinPeriodID
			public new abstract class adjdFinPeriodID : PX.Data.IBqlField
			{
			}
			[FinPeriodID(typeof(APAdjust.adjdDocDate))]
			[PXDBDefault(typeof(AP.APInvoice.finPeriodID))]
			[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
			public override String AdjdFinPeriodID
			{
				get
				{
					return this._AdjdFinPeriodID;
				}
				set
				{
					this._AdjdFinPeriodID = value;
				}
			}
			#endregion
			#region AdjdTranPeriodID
			public new abstract class adjdTranPeriodID : PX.Data.IBqlField
			{
			}
			[FinPeriodID(typeof(APAdjust.adjdDocDate))]
			[PXDBDefault(typeof(AP.APInvoice.tranPeriodID))]
			public override String AdjdTranPeriodID
			{
				get
				{
					return this._AdjdTranPeriodID;
				}
				set
				{
					this._AdjdTranPeriodID = value;
				}
			}
			#endregion
			#region CuryAdjdDiscAmt
			public new abstract class curyAdjdDiscAmt : PX.Data.IBqlField
			{
			}
			[PXDBCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.adjDiscAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? CuryAdjdDiscAmt
			{
				get
				{
					return this._CuryAdjdDiscAmt;
				}
				set
				{
					this._CuryAdjdDiscAmt = value;
				}
			}
			#endregion
			#region CuryAdjdAmt
			public new abstract class curyAdjdAmt : PX.Data.IBqlField
			{
			}
			[PXDBCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.adjAmt))]
			[PXUIField(DisplayName = "Amount Paid", Visibility = PXUIVisibility.Visible)]
			[PXDefault(TypeCode.Decimal, "0.0")]
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
			#region CuryAdjdWhTaxAmt
			public new abstract class curyAdjdWhTaxAmt : PX.Data.IBqlField
			{
			}
			[PXDBCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.adjWhTaxAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? CuryAdjdWhTaxAmt
			{
				get
				{
					return this._CuryAdjdWhTaxAmt;
				}
				set
				{
					this._CuryAdjdWhTaxAmt = value;
				}
			}
			#endregion
			#region AdjAmt
			public new abstract class adjAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Amount")]
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
			#region AdjDiscAmt
			public new abstract class adjDiscAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Cash Discount Amount")]
			public override Decimal? AdjDiscAmt
			{
				get
				{
					return this._AdjDiscAmt;
				}
				set
				{
					this._AdjDiscAmt = value;
				}
			}
			#endregion
			#region AdjWhTaxAmt
			public new abstract class adjWhTaxAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Withholding Tax Amount")]
			public override Decimal? AdjWhTaxAmt
			{
				get
				{
					return this._AdjWhTaxAmt;
				}
				set
				{
					this._AdjWhTaxAmt = value;
				}
			}
			#endregion
			#region CuryAdjgDiscAmt
			public new abstract class curyAdjgDiscAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? CuryAdjgDiscAmt
			{
				get
				{
					return this._CuryAdjgDiscAmt;
				}
				set
				{
					this._CuryAdjgDiscAmt = value;
				}
			}
			#endregion
			#region CuryAdjgAmt
			public new abstract class curyAdjgAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
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
			#region CuryAdjgWhTaxAmt
			public new abstract class curyAdjgWhTaxAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? CuryAdjgWhTaxAmt
			{
				get
				{
					return this._CuryAdjgWhTaxAmt;
				}
				set
				{
					this._CuryAdjgWhTaxAmt = value;
				}
			}
			#endregion
			#region RGOLAmt
			public new abstract class rGOLAmt : PX.Data.IBqlField
			{
			}
			[PXDBDecimal(4)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName="Realized Gain/Loss Amount")]
			public override Decimal? RGOLAmt
			{
				get
				{
					return this._RGOLAmt;
				}
				set
				{
					this._RGOLAmt = value;
				}
			}
			#endregion
			#region Released
			public new abstract class released : PX.Data.IBqlField
			{
			}
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName="Released")]
			public override Boolean? Released
			{
				get
				{
					return this._Released;
				}
				set
				{
					this._Released = value;
				}
			}
			#endregion
			#region Voided
			public new abstract class voided : PX.Data.IBqlField
			{
			}
			[PXDBBool()]
			[PXDefault(false)]
			public override Boolean? Voided
			{
				get
				{
					return this._Voided;
				}
				set
				{
					this._Voided = value;
				}
			}
			#endregion
			#region VoidAdjNbr
			public new abstract class voidAdjNbr : PX.Data.IBqlField
			{
			}
			[PXDBInt()]
			public override Int32? VoidAdjNbr
			{
				get
				{
					return this._VoidAdjNbr;
				}
				set
				{
					this._VoidAdjNbr = value;
				}
			}
			#endregion
			#region AdjdAPAcct
			public new abstract class adjdAPAcct : PX.Data.IBqlField
			{
			}
			[Account()]
			[PXDBDefault(typeof(AP.APInvoice.aPAccountID))]
			public override Int32? AdjdAPAcct
			{
				get
				{
					return this._AdjdAPAcct;
				}
				set
				{
					this._AdjdAPAcct = value;
				}
			}
			#endregion
			#region AdjdAPSub
			public new abstract class adjdAPSub : PX.Data.IBqlField
			{
			}
			[SubAccount()]
			[PXDBDefault(typeof(AP.APInvoice.aPSubID))]
			public override Int32? AdjdAPSub
			{
				get
				{
					return this._AdjdAPSub;
				}
				set
				{
					this._AdjdAPSub = value;
				}
			}
			#endregion
			#region AdjdWhTaxAcctID
			public new abstract class adjdWhTaxAcctID : PX.Data.IBqlField
			{
			}
			[Account()]
			[PXDefault(typeof(Search2<APTaxTran.accountID, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.tranType, Equal<Current<APAdjust.adjdDocType>>, And<APTaxTran.refNbr, Equal<Current<APAdjust.adjdRefNbr>>, And<Tax.taxType, Equal<CSTaxType.withholding>>>>, OrderBy<Asc<APTaxTran.taxID>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			public override Int32? AdjdWhTaxAcctID
			{
				get
				{
					return this._AdjdWhTaxAcctID;
				}
				set
				{
					this._AdjdWhTaxAcctID = value;
				}
			}
			#endregion
			#region AdjdWhTaxSubID
			public new abstract class adjdWhTaxSubID : PX.Data.IBqlField
			{
			}
			[SubAccount()]
			[PXDefault(typeof(Search2<APTaxTran.subID, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.tranType, Equal<Current<APAdjust.adjdDocType>>, And<APTaxTran.refNbr, Equal<Current<APAdjust.adjdRefNbr>>, And<Tax.taxType, Equal<CSTaxType.withholding>>>>, OrderBy<Asc<APTaxTran.taxID>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			public override Int32? AdjdWhTaxSubID
			{
				get
				{
					return this._AdjdWhTaxSubID;
				}
				set
				{
					this._AdjdWhTaxSubID = value;
				}
			}
			#endregion
			#region NoteID
			public new abstract class noteID : PX.Data.IBqlField
			{
			}
			[PXNote()]
			public override Guid? NoteID
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
			#region CuryDocBal
			public new abstract class curyDocBal : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.docBal), BaseCalc = false)]
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
			#region CuryDiscBal
			public new abstract class curyDiscBal : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.discBal), BaseCalc = false)]
			[PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override Decimal? CuryDiscBal
			{
				get
				{
					return this._CuryDiscBal;
				}
				set
				{
					this._CuryDiscBal = value;
				}
			}
			#endregion
			#region DiscBal
			public new abstract class discBal : PX.Data.IBqlField
			{
			}
			[PXDecimal(4)]
			[PXUnboundDefault()]
			public override Decimal? DiscBal
			{
				get
				{
					return this._DiscBal;
				}
				set
				{
					this._DiscBal = value;
				}
			}
			#endregion
			#region CuryWhTaxBal
			public new abstract class curyWhTaxBal : PX.Data.IBqlField
			{
			}
			[PXCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.whTaxBal), BaseCalc = false)]
			[PXUIField(DisplayName = "With. Tax Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override Decimal? CuryWhTaxBal
			{
				get
				{
					return this._CuryWhTaxBal;
				}
				set
				{
					this._CuryWhTaxBal = value;
				}
			}
			#endregion
			#region WhTaxBal
			public new abstract class whTaxBal : PX.Data.IBqlField
			{
			}
			[PXDecimal(4)]
			public override Decimal? WhTaxBal
			{
				get
				{
					return this._WhTaxBal;
				}
				set
				{
					this._WhTaxBal = value;
				}
			}
			#endregion
			#region VoidAppl
			public new abstract class voidAppl : PX.Data.IBqlField
			{
			}
			[PXBool()]
			[PXUIField(DisplayName = "Void Application", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public override Boolean? VoidAppl
			{
				[PXDependsOnFields(typeof(adjgDocType))]
				get
				{
					return (this._AdjgDocType == APDocType.VoidCheck);
				}
				set
				{
					if ((bool)value)
					{
						this._AdjgDocType = APDocType.VoidCheck;
						this.Voided = true;
					}
				}
			}
			#endregion
			#region ReverseGainLoss
			public new abstract class reverseGainLoss : PX.Data.IBqlField
			{
			}
			[PXBool()]
			public override Boolean? ReverseGainLoss
			{
				[PXDependsOnFields(typeof(adjgDocType))]
				get
				{
					return (APPaymentType.DrCr(this._AdjgDocType) == DrCr.Debit);
				}
				set
				{
				}
			}
			#endregion
		}
		#endregion

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (string.Compare(viewName, "Transactions", true) == 0)
			{
				if (values.Contains("tranType")) values["tranType"] = Document.Current.DocType;
				else values.Add("tranType", Document.Current.DocType);
				if (values.Contains("refNbr")) values["refNbr"] = Document.Current.RefNbr;
				else values.Add("refNbr", Document.Current.RefNbr);
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

		public virtual void PrepareItems(string viewName, IEnumerable items) { }

		#region Avalara Tax

		protected bool IsExternalTax
		{
			get
			{
				if (Document.Current == null)
					return false;

				return AvalaraMaint.IsExternalTax(this, Document.Current.TaxZoneID);
			}
		}

		public virtual APInvoice CalculateAvalaraTax(APInvoice invoice)
		{
			if (invoice.InstallmentNbr != null)
			{
				//do not calculate tax for installments
				return invoice;
			}

			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);

			AvaAddress.AddressSvc addressService = new AvaAddress.AddressSvc();
			AvalaraMaint.SetupService(this, addressService);

			GetTaxRequest getRequest = BuildGetTaxRequest(invoice);

			if (getRequest.Lines.Count == 0)
			{
				Document.SetValueExt<APInvoice.isTaxValid>(invoice, true);
				if (invoice.IsTaxSaved == true)
					Document.SetValueExt<APInvoice.isTaxSaved>(invoice, false);

				try
				{
					skipAvalaraCallOnSave = true;
					this.Save.Press();
				}
				finally
				{
					skipAvalaraCallOnSave = false;
				}
			}

			GetTaxResult result = service.GetTax(getRequest);
			if (result.ResultCode == SeverityLevel.Success)
			{
				try
				{
					ApplyAvalaraTax(invoice, result);

					try
					{
						skipAvalaraCallOnSave = true;
						this.Save.Press();
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						skipAvalaraCallOnSave = false;
					}

				}
				catch (PXOuterException ex)
				{
					try
					{
						CancelTax(invoice, CancelCode.Unspecified);
					}
					catch (Exception)
					{
						throw new PXException(new PXException(ex, TX.Messages.FailedToApplyTaxes), TX.Messages.FailedToCancelTaxes);
					}

					string msg = TX.Messages.FailedToApplyTaxes;
					foreach (string err in ex.InnerMessages)
					{
						msg += Environment.NewLine + err;
					}

					throw new PXException(ex, msg);
				}
				catch (Exception ex)
				{
					try
					{
						CancelTax(invoice, CancelCode.Unspecified);
					}
					catch (Exception)
					{
						throw new PXException(new PXException(ex, TX.Messages.FailedToApplyTaxes), TX.Messages.FailedToCancelTaxes);
					}

					string msg = TX.Messages.FailedToApplyTaxes;
					msg += Environment.NewLine + ex.Message;

					throw new PXException(ex, msg);
				}

				PostTaxRequest request = new PostTaxRequest();
				request.CompanyCode = getRequest.CompanyCode;
				request.DocCode = getRequest.DocCode;
				request.DocDate = getRequest.DocDate;
				request.DocType = getRequest.DocType;
				request.TotalAmount = result.TotalAmount;
				request.TotalTax = result.TotalTax;
				PostTaxResult postResult = service.PostTax(request);
				if (postResult.ResultCode == SeverityLevel.Success)
				{
					invoice.IsTaxValid = true;
					invoice = Document.Update(invoice);
					try
					{
						skipAvalaraCallOnSave = true;
						this.Save.Press();
					}
					finally
					{
						skipAvalaraCallOnSave = false;
					}
				}

			}
			else
			{
				LogMessages(result);

				throw new PXException(TX.Messages.FailedToGetTaxes);
			}

			return invoice;
		}

		protected virtual GetTaxRequest BuildGetTaxRequest(APInvoice invoice)
		{
			if (invoice == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Vendor vend = (Vendor)vendor.View.SelectSingleBound(new object[] { invoice });
			
			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CurrencyCode = invoice.CuryID;
			request.CustomerCode = vend.AcctCD;
			IAddressBase fromAddress = GetFromAddress(invoice);
			IAddressBase toAddress = GetToAddress(invoice);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFrom);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetTo);

			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("AP.{0}.{1}", invoice.DocType, invoice.RefNbr);
			request.DocDate = invoice.DocDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(invoice);

			CRLocation branchLoc = GetBranchLocation(invoice);

			if (branchLoc != null)
			{
				request.CustomerUsageType = branchLoc.CAvalaraCustomerUsageType;
				request.ExemptionNo = branchLoc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.PurchaseInvoice;
			int mult = 1;
			switch (invoice.DocType)
			{
				case APDocType.Invoice:
				case APDocType.CreditAdj:
					request.DocType = DocumentType.PurchaseInvoice;
					break;
				case APDocType.DebitAdj:
					if (invoice.OrigDocDate != null)
					{
						request.TaxOverride.Reason = "Debit Adjustment";
						request.TaxOverride.TaxDate = invoice.OrigDocDate.Value;
						request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
						mult = -1;
					}
					request.DocType = DocumentType.ReturnInvoice;
					break;

				default:
					throw new PXException(Messages.DocTypeNotSupported);
			}

			PXSelectBase<APTran> select = new PXSelectJoin<APTran,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<APTran.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<APTran.accountID>>>>,
				Where<APTran.tranType, Equal<Current<APInvoice.docType>>,
					And<APTran.refNbr, Equal<Current<APInvoice.refNbr>>,
					And<APTran.lineType, NotEqual<SOLineType.discount>>>>,
				OrderBy<Asc<APTran.tranType, Asc<APTran.refNbr, Asc<APTran.lineNbr>>>>>(this);

			request.Discount = GetDocDiscount().GetValueOrDefault();
			foreach (PXResult<APTran, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { invoice }))
			{
				APTran tran = (APTran)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);
				line.Amount = mult * tran.CuryTranAmt.GetValueOrDefault();
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

			return request;
		}

		protected bool skipAvalaraCallOnSave = false;
		protected virtual void ApplyAvalaraTax(APInvoice invoice, GetTaxResult result)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { invoice });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(TX.Messages.ExternalTaxVendorNotFound);

			if (vendor.SalesTaxAcctID == null)
				throw new PXSetPropertyException(TX.Messages.TaxPayableAccountNotSpecified, vendor.AcctCD);

			if (vendor.SalesTaxSubID == null)
				throw new PXException(TX.Messages.TaxPayableSubNotSpecified, vendor.AcctCD);

			//Clear all existing Tax transactions:
			foreach (PXResult<APTaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { invoice }))
			{
				APTaxTran taxTran = (APTaxTran)res;
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

				APTaxTran tax = new APTaxTran();
				tax.Module = BatchModule.AP;
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

			bool requireControlTotal = APSetup.Current.RequireControlTotal == true;
			if (invoice.Hold != true)
				APSetup.Current.RequireControlTotal = false;

			try
			{
				invoice.CuryTaxTotal = Math.Abs(result.TotalTax);
				Document.Cache.SetValueExt<APInvoice.isTaxSaved>(invoice, true);
			}
			finally
			{
				APSetup.Current.RequireControlTotal = requireControlTotal;
			}
		}

		protected virtual void CancelTax(APInvoice invoice, CancelCode code)
		{
			CancelTaxRequest request = new CancelTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CancelCode = code;
			request.DocCode = string.Format("AP.{0}.{1}", invoice.DocType, invoice.RefNbr);
			request.DocType = DocumentType.PurchaseInvoice;

			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);
			CancelTaxResult result = service.CancelTax(request);

			bool raiseError = false;
			if (result.ResultCode != SeverityLevel.Success)
			{
				LogMessages(result);

				if (result.ResultCode == SeverityLevel.Error && result.Messages[0].Name == "DocumentNotFoundError")
				{
					//just ignore this error. There is no document to delete in avalara.
				}
				else
				{
					raiseError = true;
				}
			}

			if (raiseError)
			{
				throw new PXException(TX.Messages.FailedToDeleteFromAvalara);
			}
			else
			{
				invoice.IsTaxSaved = false;
				invoice.IsTaxValid = false;
				if (Document.Cache.GetStatus(invoice) == PXEntryStatus.Notchanged)
					Document.Cache.SetStatus(invoice, PXEntryStatus.Updated);
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

		protected virtual IAddressBase GetToAddress(APInvoice invoice)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(invoice.BranchID))
				return (Address)res;

			return null;
		}

		protected virtual CRLocation GetBranchLocation(APInvoice invoice)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, CRLocation> res in select.Select(invoice.BranchID))
				return (CRLocation)res;

			return null;
		}

		protected virtual IAddressBase GetFromAddress(APInvoice invoice)
		{
			Address vendorAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, vendor.Current.DefAddressID);

			return vendorAddress;
		}

		protected virtual decimal? GetDocDiscount()
		{
			return null;
		}

		protected virtual string GetAvalaraLocationCode(APInvoice invoice)
		{
			return null;
		}

		#endregion
	}
}
namespace PX.Objects.PO
{
	//This class is used for Update of vouchered (billed) amounts in POReceiptLine
	[PXProjection(typeof(Select<POReceiptLine>), Persistent = true)]
	[Serializable]
	public partial class POReceiptLineR : IBqlTable
	{
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POReceiptLine.receiptNbr))]
		[PXDefault()]
		[PXParent(typeof(Select<POReceipt, Where<POReceipt.receiptNbr, Equal<Current<POReceiptLineR1.receiptNbr>>>>))]
		[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(POReceiptLine.lineNbr))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true, BqlField = typeof(POReceiptLine.inventoryID))]
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
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort(BqlField = typeof(POReceiptLine.invtMult))]
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

		#region ReceiptQty
		public abstract class receiptQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceiptQty;

		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.receiptQty))]
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
		#endregion
		#region VoucheredQty
		public abstract class voucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredQty;

		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.voucheredQty))]
		public virtual Decimal? VoucheredQty
		{
			get
			{
				return this._VoucheredQty;
			}
			set
			{
				this._VoucheredQty = value;
			}
		}
		#endregion
		#region BaseVoucheredQty
		public abstract class baseVoucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseVoucheredQty;

		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.baseVoucheredQty))]
		public virtual Decimal? BaseVoucheredQty
		{
			get
			{
				return this._BaseVoucheredQty;
			}
			set
			{
				this._BaseVoucheredQty = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong(BqlField = typeof(POReceiptLine.curyInfoID))]
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
		#region CuryVoucheredCost
		public abstract class curyVoucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVoucheredCost;

		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.curyVoucheredCost))]
		public virtual Decimal? CuryVoucheredCost
		{
			get
			{
				return this._CuryVoucheredCost;
			}
			set
			{
				this._CuryVoucheredCost = value;
			}
		}
		#endregion
		#region VoucheredCost
		public abstract class voucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredCost;
		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.voucheredCost))]		
		public virtual Decimal? VoucheredCost
		{
			get
			{
				return this._VoucheredCost;
			}
			set
			{
				this._VoucheredCost = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(POReceiptLineR.inventoryID), DisplayName = "UOM", BqlField = typeof(POReceiptLine.uOM))]
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

		#region SignedVoucheredQty
		public abstract class signedVoucheredQty : PX.Data.IBqlField
		{
		}
		
		
		[PXQuantity(typeof(POReceiptLineR.uOM), typeof(POReceiptLineR.signedBaseVoucheredQty), HandleEmptyKey = true)]
		public virtual Decimal? SignedVoucheredQty
		{
			[PXDependsOnFields(typeof(voucheredQty))]
			get
			{
				return this._VoucheredQty * this.Sign;
			}
			set
			{
				this._VoucheredQty = value * this.Sign;
			}
		}
		#endregion
		#region SignedBaseVoucheredQty
		public abstract class signedBaseVoucheredQty : PX.Data.IBqlField
		{
		}
	

		[PXDecimal(6)]
		public virtual Decimal? SignedBaseVoucheredQty
		{
			[PXDependsOnFields(typeof(baseVoucheredQty))]
			get
			{
				return this._BaseVoucheredQty * this.Sign;
			}
			set
			{
				this._BaseVoucheredQty = value * this.Sign;
			}
		}
		#endregion
		#region SignedCuryVoucheredCost
		public abstract class signedCuryVoucheredCost : PX.Data.IBqlField
		{
		}

		[PXCurrency(typeof(POReceiptLineR.curyInfoID), typeof(POReceiptLineR.signedVoucheredCost))]
		public virtual Decimal? SignedCuryVoucheredCost
		{
			[PXDependsOnFields(typeof(curyVoucheredCost))]
			get
			{
				return this._CuryVoucheredCost * this.Sign;
			}
			set
			{
				this._CuryVoucheredCost = value * this.Sign;
			}
		}
		#endregion
		#region SignedVoucheredCost
		public abstract class signedVoucheredCost : PX.Data.IBqlField
		{
		}
		
		[PXDecimal(6)]
		public virtual Decimal? SignedVoucheredCost
		{
			[PXDependsOnFields(typeof(voucheredCost))]
			get
			{
				return this._VoucheredCost * this.Sign;
			}
			set
			{
				this._VoucheredCost = value * this.Sign;
			}
		}
		#endregion


		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp(BqlField = typeof(POReceiptLine.Tstamp))]
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

		protected decimal Sign
		{
			get { return this._InvtMult < 0 ? Decimal.MinusOne : Decimal.One; }
		}
	}

	//This class is used for Update of vouchered (billed amounts) in POLine
	[PXProjection(typeof(Select<POLine>), Persistent = true)]
	[Serializable]
	public partial class POLineAP : IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true, BqlField = typeof(POLine.inventoryID))]
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
		#region VoucheredQty
		public abstract class voucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredQty;

		[PXDBQuantity(typeof(POLineAP.uOM), typeof(POLineAP.baseVoucheredQty), HandleEmptyKey = true, BqlField = typeof(POLine.voucheredQty))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vouchered Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? VoucheredQty
		{
			get
			{
				return this._VoucheredQty;
			}
			set
			{
				this._VoucheredQty = value;
			}
		}
		#endregion
		#region BaseVoucheredQty
		public abstract class baseVoucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseVoucheredQty;

		[PXDBDecimal(6, BqlField = typeof(POLine.baseVoucheredQty))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Base Vouchered Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? BaseVoucheredQty
		{
			get
			{
				return this._BaseVoucheredQty;
			}
			set
			{
				this._BaseVoucheredQty = value;
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
		#region CuryVoucheredCost
		public abstract class curyVoucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVoucheredCost;

		[PXDBCurrency(typeof(POLineAP.curyInfoID), typeof(POLineAP.voucheredCost), BqlField = typeof(POLine.curyVoucheredCost))]
		[PXUIField(DisplayName = "Vouchered Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVoucheredCost
		{
			get
			{
				return this._CuryVoucheredCost;
			}
			set
			{
				this._CuryVoucheredCost = value;
			}
		}
		#endregion
		#region VoucheredCost
		public abstract class voucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredCost;
		[PXDBDecimal(6, BqlField = typeof(POLine.voucheredCost))]
		[PXUIField(DisplayName = "Vouchered Cost")]
		public virtual Decimal? VoucheredCost
		{
			get
			{
				return this._VoucheredCost;
			}
			set
			{
				this._VoucheredCost = value;
			}
		}
		#endregion
		#region ReceivedQty
		public abstract class receivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedQty;
		[PXDBQuantity(typeof(POLineAP.uOM), typeof(POLineAP.baseReceivedQty), HandleEmptyKey = true, BqlField = typeof(POLine.receivedQty))]
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

		[PXDBDecimal(6, BqlField = typeof(POLine.baseReceivedQty))]
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
		#region CuryReceivedCost
		public abstract class curyReceivedCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryReceivedCost;

		[PXDBCurrency(typeof(POLineAP.curyInfoID), typeof(POLineAP.receivedCost), BqlField = typeof(POLine.curyReceivedCost))]
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
		[INUnit(typeof(POLineAP.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
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
	}

	//This class is used for Update of unbilled amounts in POReceiptLine ( during Release AP Document process)
	[PXProjection(typeof(Select<POReceiptLine>), Persistent = true)]
	[Serializable]
	public partial class POReceiptLineR1 : IBqlTable
	{
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POReceiptLine.receiptNbr))]
		[PXDefault()]
		[PXParent(typeof(Select<POReceipt, Where<POReceipt.receiptNbr, Equal<Current<POReceiptLineR1.receiptNbr>>>>))]
		[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
			}
		}
		#endregion
		#region ReceiptType
		public abstract class receiptType : PX.Data.IBqlField
		{
		}
		protected String _ReceiptType;
		[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.receiptType))]
		[PXDBDefault(typeof(POReceipt.receiptType))]
		public virtual String ReceiptType
		{
			get
			{
				return this._ReceiptType;
			}
			set
			{
				this._ReceiptType = value;
			}
		}
		#endregion

		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(POReceiptLine.lineNbr))]
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong(BqlField = typeof(POReceiptLine.curyInfoID))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true, BqlField = typeof(POReceiptLine.inventoryID))]
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
		[SubItem(BqlField = typeof(POReceiptLine.subItemID))]
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
		[INUnit(typeof(POReceiptLineR1.inventoryID), DisplayName = "UOM", BqlField = typeof(POReceiptLine.uOM))]
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
		#region ReceiptQty
		public abstract class receiptQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceiptQty;

		[PXDBQuantity(MinValue = 0, BqlField = typeof(POReceiptLine.receiptQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
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
		#region CuryExtCost
		public abstract class curyExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExtCost;
		[PXDBDecimal(4, BqlField = typeof(POReceiptLine.curyLineAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort(BqlField = typeof(POReceiptLine.invtMult))]		
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
		#region UnbilledQty
		public abstract class unbilledQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledQty;
		[PXDBQuantity(typeof(POReceiptLineR1.uOM), typeof(POReceiptLineR1.baseUnbilledQty), HandleEmptyKey = true, BqlField = typeof(POReceiptLine.unbilledQty))]
		[PXFormula(null, typeof(SumCalc<POReceipt.unbilledQty>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
		public virtual Decimal? UnbilledQty
		{
			get
			{
				return this._UnbilledQty;
			}
			set
			{
				this._UnbilledQty = value;
			}
		}
		#endregion
		#region BaseUnbilledQty
		public abstract class baseUnbilledQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseUnbilledQty;
		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.baseUnbilledQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseUnbilledQty
		{
			get
			{
				return this._BaseUnbilledQty;
			}
			set
			{
				this._BaseUnbilledQty = value;
			}
		}
		#endregion
		#region CuryUnbilledAmt
		public abstract class curyUnbilledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledAmt;
		[PXDBCurrency(typeof(POReceiptLineR1.curyInfoID), typeof(POReceiptLineR1.unbilledAmt), BqlField = typeof(POReceiptLine.curyUnbilledAmt))]
		[PXFormula(typeof(Mult<POReceiptLineR1.unbilledQty, POReceiptLineR1.curyUnitCost>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnbilledAmt
		{
			get
			{
				return this._CuryUnbilledAmt;
			}
			set
			{
				this._CuryUnbilledAmt = value;
			}
		}
		#endregion
		#region UnbilledAmt
		public abstract class unbilledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledAmt;
		[PXDBDecimal(4, BqlField = typeof(POReceiptLine.unbilledAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledAmt
		{
			get
			{
				return this._UnbilledAmt;
			}
			set
			{
				this._UnbilledAmt = value;
			}
		}
		#endregion
		#region CuryUnbilledDiscountAmt
		public abstract class curyUnbilledDiscountAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledDiscountAmt;
		[PXDBCurrency(typeof(POReceiptLineR1.curyInfoID), typeof(POReceiptLineR1.unbilledDiscountAmt), BqlField = typeof(POReceiptLine.curyUnbilledDiscountAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnbilledDiscountAmt
		{
			get
			{
				return this._CuryUnbilledDiscountAmt;
			}
			set
			{
				this._CuryUnbilledDiscountAmt = value;
			}
		}
		#endregion
		#region UnbilledDiscountAmt
		public abstract class unbilledDiscountAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledDiscountAmt;
		[PXDBDecimal(4, BqlField = typeof(POReceiptLine.unbilledDiscountAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledDiscountAmt
		{
			get
			{
				return this._UnbilledDiscountAmt;
			}
			set
			{
				this._UnbilledDiscountAmt = value;
			}
		}
		#endregion
		#region CuryUnbilledExtCost
		public abstract class curyUnbilledExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledExtCost;
		[PXCurrency(typeof(POReceiptLineR1.curyInfoID), typeof(POReceiptLineR1.unbilledExtCost))]
		[PXFormula(typeof(Sub<POReceiptLineR1.curyUnbilledAmt, POReceiptLineR1.curyUnbilledDiscountAmt>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnbilledExtCost
		{
			get
			{
				return this._CuryUnbilledExtCost;
			}
			set
			{
				this._CuryUnbilledExtCost = value;
			}
		}
		#endregion
		#region UnbilledExtCost
		public abstract class unbilledExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledExtCost;
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledExtCost
		{
			get
			{
				return this._UnbilledExtCost;
			}
			set
			{
				this._UnbilledExtCost = value;
			}
		}
		#endregion

		#region CuryUnitCost
		public abstract class curyUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitCost;

		[PXDBDecimal(4, BqlField = typeof(POReceiptLine.curyUnitCost))]
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
		#region BillPPVAmt
		public abstract class billPPVAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _BillPPVAmt;
		[PXDBBaseCury(BqlField = typeof(POReceiptLine.billPPVAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BillPPVAmt
		{
			get
			{
				return this._BillPPVAmt;
			}
			set
			{
				this._BillPPVAmt = value;
			}
		}
		#endregion
		#region ReversedBillPPVAmt
		public abstract class reversedBillPPVAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReversedBillPPVAmt;
		[PXDBBaseCury(BqlField = typeof(POReceiptLine.reversedBillPPVAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ReversedBillPPVAmt
		{
			get
			{
				return this._ReversedBillPPVAmt;
			}
			set
			{
				this._ReversedBillPPVAmt = value;
			}
		}
		#endregion

		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;

		[PXDBPriceCost(BqlField = typeof(POReceiptLine.unitCost))]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region GroupDiscountRate
		public abstract class groupDiscountRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _GroupDiscountRate;
		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.groupDiscountRate))]
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
		[PXDBDecimal(6, BqlField = typeof(POReceiptLine.documentDiscountRate))]
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
		[PXDBString(10, IsUnicode = true, BqlField = typeof(POReceiptLine.taxCategoryID))]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[POReceiptUnbilledTaxR(typeof(POReceipt), typeof(POReceiptTax), typeof(POReceiptTaxTran))]
		//[POUnbilledTaxR(typeof(POReceipt), typeof(POTax), typeof(POTaxTran))]
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
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(2, IsFixed = true, BqlField = typeof(POReceiptLine.lineType))]
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


		public decimal APSign
		{
			[PXDependsOnFields(typeof(invtMult))]
			get { return this._InvtMult < 0 ? Decimal.MinusOne : Decimal.One; }
		}
	}

	public interface IAPTranSource
	{
		Int32? BranchID { get; }
		Int32? ExpenseAcctID { get; }
		Int32? ExpenseSubID { get; }
		Int32? POAccrualAcctID { get; }
		Int32? POAccrualSubID { get; }
		String LineType { get; }
		Int32? InventoryID { get; }
		String UOM { get; }
		Int64? CuryInfoID { get; }
		decimal? BillQty { get; }
		decimal? CuryUnitCost { get; }
		decimal? UnitCost { get; }
		decimal? CuryDiscAmt { get; }
		decimal? DiscAmt { get; }
		decimal? DiscPct { get; }
		decimal? CuryLineAmt { get; }
		decimal? LineAmt { get; }
		String TaxCategoryID { get; }
		String TranDesc { get; }
		String TaxID { get; }
		int? ProjectID { get; }
		int? TaskID { get; }
		bool IsReturn { get; }

		String DiscountID { get; }

		String DiscountSequenceID { get; }

		decimal? GroupDiscountRate { get; }

		decimal? DocumentDiscountRate { get; }
		//String ReceiptNbr { get; }
		//Int16? LineNbr { get; }

		bool CompareReferenceKey(AP.APTran aTran);
		void SetReferenceKeyTo(AP.APTran aTran);

	}
}
  
