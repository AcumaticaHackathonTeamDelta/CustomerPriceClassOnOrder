using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;

using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.CS;

using PX.Objects.PM;
using PX.Objects.GL.Reclassification.UI;
using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using AP1099Hist = PX.Objects.AP.Overrides.APDocumentRelease.AP1099Hist;
using AP1099Yr = PX.Objects.AP.Overrides.APDocumentRelease.AP1099Yr;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.AP
{
	public class APQuickCheckEntry : APDataEntryGraph<APQuickCheckEntry, APQuickCheck>
	{
		public PXSelect<InventoryItem, Where<InventoryItem.stkItem, Equal<False>, And<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>> nonStockItem;

		public ToggleCurrency<APQuickCheck> CurrencyView;
		[PXCopyPasteHiddenFields(typeof(APQuickCheck.extRefNbr))]
		public PXSelectJoin<APQuickCheck,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<APQuickCheck.vendorID>>>,
			Where<APQuickCheck.docType, Equal<Optional<APQuickCheck.docType>>,
			And<Where<Vendor.bAccountID, IsNull,
			Or<Match<Vendor, Current<AccessInfo.userName>>>>>>> Document;
		[PXCopyPasteHiddenFields(typeof(APQuickCheck.printCheck), typeof(APQuickCheck.cleared), typeof(APQuickCheck.clearDate))]
		public PXSelect<APQuickCheck, Where<APQuickCheck.docType, Equal<Current<APQuickCheck.docType>>, And<APQuickCheck.refNbr, Equal<Current<APQuickCheck.refNbr>>>>> CurrentDocument;

		public PXSelect<APTran, Where<APTran.tranType, Equal<Current<APQuickCheck.docType>>, And<APTran.refNbr, Equal<Current<APQuickCheck.refNbr>>>>> Transactions;
		public PXSelect<APTax> ItemTaxes;		
		public PXSelectJoin<APTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.module, Equal<BatchModule.moduleAP>, And<APTaxTran.tranType, Equal<Current<APQuickCheck.docType>>, And<APTaxTran.refNbr, Equal<Current<APQuickCheck.refNbr>>>>>> Taxes;
		public PXSelectJoin<APTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.module, Equal<BatchModule.moduleAP>, And<APTaxTran.tranType, Equal<Current<APQuickCheck.docType>>, And<APTaxTran.refNbr, Equal<Current<APQuickCheck.refNbr>>, 
			And<Tax.taxType, Equal<CSTaxType.use>>>>>> UseTaxes;

		[PXViewName(Messages.APAddress)]
		public PXSelect<APAddress, Where<APAddress.addressID, Equal<Current<APQuickCheck.remitAddressID>>>> Remittance_Address;
		[PXViewName(Messages.APContact)]
		public PXSelect<APContact, Where<APContact.contactID, Equal<Current<APQuickCheck.remitContactID>>>> Remittance_Contact;
				
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APQuickCheck.curyInfoID>>>> currencyinfo;
		public PXSelect<PaymentMethodAccount, Where<PaymentMethodAccount.cashAccountID, Equal<Current<APQuickCheck.cashAccountID>>,
					And<PaymentMethodAccount.useForAP,Equal<True>>>, OrderBy<Asc<PaymentMethodAccount.aPIsDefault>>> CashAcctDetail_AccountID;

		public APPaymentChargeSelect<APQuickCheck, APQuickCheck.paymentMethodID, APQuickCheck.cashAccountID, APQuickCheck.adjDate, APQuickCheck.adjFinPeriodID,
			Where<APPaymentChargeTran.docType, Equal<Current<APQuickCheck.docType>>,
				And<APPaymentChargeTran.refNbr, Equal<Current<APQuickCheck.refNbr>>>>> PaymentCharges;

		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Current<APQuickCheck.vendorID>>>> vendor;
		public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Optional<APQuickCheck.vendorID>>>> employee;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, And<Location.locationID, Equal<Optional<APQuickCheck.vendorLocationID>>>>> location;
		public PXSetup<CashAccount, Where<CashAccount.cashAccountID, Equal<Optional<APQuickCheck.cashAccountID>>>> cashaccount;
		public PXSetup<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Optional<APQuickCheck.paymentMethodID>>>> paymenttype;
		public PXSetup<PaymentMethodAccount, Where<PaymentMethodAccount.cashAccountID, Equal<Optional<APQuickCheck.cashAccountID>>, And<PaymentMethodAccount.paymentMethodID, Equal<Current<APQuickCheck.paymentMethodID>>>>> cashaccountdetail;

		public PXSetup<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Current<APQuickCheck.adjFinPeriodID>>>> finperiod;
		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<APQuickCheck.taxZoneID>>>> taxzone;

		public PXSetup<APSetup> apsetup;

		public PXSelect<AP1099Hist> ap1099hist;
		public PXSelect<AP1099Yr> ap1099year;
		
		public PXSetup<GLSetup> glsetup;

		public PXSelect<GLVoucher, Where<True, Equal<False>>> Voucher;

		#region Other Buttons
		public PXAction<APQuickCheck> printCheck;
		[PXUIField(DisplayName = "Print Check", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable PrintCheck(PXAdapter adapter)
		{
			APPayment doc = PXSelect<APPayment, Where<APPayment.docType, Equal<Current<APQuickCheck.docType>>, And<APPayment.refNbr, Equal<Current<APQuickCheck.refNbr>>>>>.SelectSingleBound(this, new object[] {Document.Current});
			APPrintChecks pp = PXGraph.CreateInstance<APPrintChecks>();
			PrintChecksFilter filter_copy = PXCache<PrintChecksFilter>.CreateCopy(pp.Filter.Current);
			filter_copy.PayAccountID = doc.CashAccountID;
			filter_copy.PayTypeID = doc.PaymentMethodID;
			pp.Filter.Cache.Update(filter_copy);
			doc.Selected = true;
			doc.Passed = true;
			pp.APPaymentList.Cache.Update(doc);
			pp.APPaymentList.Cache.SetStatus(doc, PXEntryStatus.Updated);
			pp.APPaymentList.Cache.IsDirty = false;
			throw new PXRedirectRequiredException(pp, "Preview");
		}

		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<APRegister> list = new List<APRegister>();
			foreach (APQuickCheck apdoc in adapter.Get<APQuickCheck>())
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
			Save.Press();
			PXLongOperation.StartOperation(this, delegate() { APDocumentRelease.ReleaseDoc(list, false); });
			return list;
		}

		public PXAction<APQuickCheck> prebook;
		[PXUIField(DisplayName = "Pre-release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable Prebook(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<APRegister> list = new List<APRegister>();

			foreach (APQuickCheck apdoc in adapter.Get<APQuickCheck>())
			{
				if (!(bool)apdoc.Hold && !(bool)apdoc.Released && apdoc.Prebooked != true && apdoc.Voided!= true)
				{
					if (apdoc.PrebookAcctID == null)
					{
						cache.RaiseExceptionHandling<APQuickCheck.prebookAcctID>(apdoc, apdoc.PrebookAcctID, new PXSetPropertyException(Messages.PrebookingAccountIsRequiredForPrebooking));
						continue;
					}
					if (apdoc.PrebookSubID == null)
					{
						cache.RaiseExceptionHandling<APQuickCheck.prebookSubID>(apdoc, apdoc.PrebookSubID, new PXSetPropertyException(Messages.PrebookingAccountIsRequiredForPrebooking));
						continue;
					}
					cache.Update(apdoc);
					list.Add(apdoc);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}
			Persist();
			PXLongOperation.StartOperation(this, delegate() { APDocumentRelease.ReleaseDoc(list, false, true); });
			return list;
		}


		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable VoidCheck(PXAdapter adapter)
		{
			if (Document.Current != null && (Document.Current.Released == true || Document.Current.Prebooked == true) && Document.Current.Voided == false && Document.Current.DocType == APDocType.QuickCheck)
			{
				APQuickCheck doc = PXCache<APQuickCheck>.CreateCopy(Document.Current);
				FiscalPeriodUtils.VerifyFinPeriod<APQuickCheck.finPeriodID, FinPeriod.aPClosed>(this, Document.Cache, doc, finperiod);
				
				try
				{
					_IsVoidCheckInProgress = true;
					this.VoidCheckProc(doc);
				}
				catch (PXSetPropertyException)
				{
					this.Clear();
					Document.Current = doc;
					throw;
				}
				finally
				{
					_IsVoidCheckInProgress = false;
				}

				Document.Cache.RaiseExceptionHandling<APQuickCheck.finPeriodID>(Document.Current, Document.Current.FinPeriodID, null);

				List<APQuickCheck> rs = new List<APQuickCheck>();
				rs.Add(Document.Current);
				return rs;
			}
			return adapter.Get();
		}

		public PXAction<APQuickCheck> reclassifyBatch;
		[PXUIField(DisplayName = Messages.ReclassifyGLBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReclassifyBatch(PXAdapter adapter)
		{
			var document = Document.Current;

			if (document != null)
			{
				ReclassifyTransactionsProcess.TryOpenForReclassificationOfDocument(Document.View, BatchModule.AP, document.BatchNbr, document.DocType,
					document.RefNbr);
			}

			return adapter.Get();
		}

		public PXAction<APQuickCheck> vendorDocuments;
		[PXUIField(DisplayName = "Vendor Details", Visible=false)]
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

		public PXAction<APQuickCheck> viewSchedule;
		[PXUIField(DisplayName = "View Schedule")]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Settings)]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			var currentTransaction = Transactions.Current;

			var invoicePart = PXSelect<
				APInvoice, 
				Where<
					APInvoice.docType, Equal<Current<APQuickCheck.docType>>, 
					And<APInvoice.refNbr, Equal<Current<APQuickCheck.refNbr>>>>>
				.Select(this);

			if (currentTransaction != null && 
				invoicePart != null &&
				Transactions.Cache.GetStatus(currentTransaction) == PXEntryStatus.Notchanged)
			{
				Save.Press();			
				APInvoiceEntry.ViewScheduleForLine(this, invoicePart, Transactions.Current);
			}

			return adapter.Get();
		}

		public PXAction<APQuickCheck> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddress, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (APQuickCheck current in adapter.Get<APQuickCheck>())
			{
				if (current != null)
				{
					bool needSave = false;
					Save.Press();
					APAddress address = this.Remittance_Address.Select();
					if (address != null && address.IsDefaultAddress == false && address.IsValidated == false)
					{
						if (PXAddressValidator.Validate<APAddress>(this, address, true))
							needSave = true;
					}
					if (needSave == true)
						this.Save.Press();

				}
				yield return current;
			}
		}
		#endregion

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

		public APQuickCheckEntry()
			:base()
		{
			{
				APSetup record = apsetup.Select();
			}

			{
				GLSetup record = glsetup.Select();
			}

			this.RowUpdated.AddHandler<APQuickCheck>(ParentFieldUpdated);

			PopulateBoxList();
			
			PXUIFieldAttribute.SetVisible<APTran.projectID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));
			PXUIFieldAttribute.SetVisible<APTran.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));
			PXUIFieldAttribute.SetVisible<APTran.nonBillable>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AP));

			FieldDefaulting.AddHandler<InventoryItem.stkItem>((sender, e) => { if (e.Row != null) e.NewValue = false; });
			Actions.Move(nameof(release), nameof(prebook));
		}

		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<APQuickCheck.docDate, APQuickCheck.finPeriodID, APQuickCheck.curyID>(e.Row, e.OldRow))
			{
				foreach (APTran tran in Transactions.Select())
				{
					Transactions.Cache.SmartSetStatus(tran, PXEntryStatus.Updated);
				}
			}
		}


		private object GetAcctSub<Field>(PXCache cache, object data)
			where Field : IBqlField
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

		#region InventoryItem
		#region COGSSubID
		[PXDefault(typeof(Search<INPostClass.cOGSSubID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>))]
		[SubAccount(typeof(InventoryItem.cOGSAcctID), DisplayName = "Expense Sub.", DescriptionField = typeof(Sub.description))]
		public virtual void InventoryItem_COGSSubID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion

		#region APQuickCheck Events

		protected virtual void APQuickCheck_DocType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = APDocType.QuickCheck;
			e.Cancel = true;
		}

		protected virtual void APQuickCheck_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			foreach (APTaxTran taxTran in Taxes.Select())
			{
				if (Taxes.Cache.GetStatus(taxTran) == PXEntryStatus.Notchanged)
				{
					Taxes.Cache.SetStatus(taxTran, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void APQuickCheck_DocDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && ((APQuickCheck)e.Row).Released == false) 
			{
				e.NewValue = ((APQuickCheck)e.Row).AdjDate;
				e.Cancel = true;
			}
		}

		protected virtual void APQuickCheck_FinPeriodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				e.NewValue = ((APQuickCheck)e.Row).AdjFinPeriodID;
				e.Cancel = true;
			}
		}

		protected virtual void APQuickCheck_TranPeriodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null)
			{
				e.NewValue = ((APQuickCheck)e.Row).AdjTranPeriodID;
				e.Cancel = true;
			}
		}

		protected virtual void APQuickCheck_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			vendor.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<APQuickCheck.vendorLocationID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.termsID>(e.Row);

		}

		protected virtual void APQuickCheck_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			location.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<APQuickCheck.paymentMethodID>(e.Row);				
			sender.SetDefaultExt<APQuickCheck.aPAccountID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.aPSubID>(e.Row);

			APAddressAttribute.DefaultRecord<APPayment.remitAddressID>(sender, e.Row);
			APContactAttribute.DefaultRecord<APPayment.remitContactID>(sender, e.Row);

			sender.SetDefaultExt<APQuickCheck.taxCalcMode>(e.Row);
			sender.SetDefaultExt<APQuickCheck.taxZoneID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.prebookAcctID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.prebookSubID>(e.Row);
			
		}

		protected virtual void APQuickCheck_APAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (vendor.Current != null && location.Current != null && e.Row != null)
			{
				e.NewValue = GetAcctSub<Location.aPAccountID>(location.Cache, location.Current);
			}
		}

		protected virtual void APQuickCheck_APSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (vendor.Current != null && location.Current != null && e.Row != null)
			{
				e.NewValue = GetAcctSub<Location.aPSubID>(location.Cache, location.Current);
			}
		}

		protected virtual void APQuickCheck_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APQuickCheck payment = (APQuickCheck)e.Row;
			cashaccount.RaiseFieldUpdated(sender, e.Row);

			if (_IsVoidCheckInProgress == false && PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<APQuickCheck.curyInfoID>(sender, e.Row);

				string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) == false)
				{
					sender.RaiseExceptionHandling<APQuickCheck.adjDate>(e.Row, payment.DocDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				}

				if (info != null)
				{
					payment.CuryID = info.CuryID;
				}
			}

			sender.SetValueExt<APQuickCheck.pTInstanceID>(e.Row, null);

			payment.Cleared = false;
			payment.ClearDate = null;

			if ((cashaccount.Current != null) && (cashaccount.Current.Reconcile == false))
			{
				payment.Cleared = true;
				payment.ClearDate = payment.DocDate;
			}
		}

		protected virtual void APQuickCheck_PaymentMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			paymenttype.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<APQuickCheck.cashAccountID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.pTInstanceID>(e.Row);
			sender.SetDefaultExt<APQuickCheck.printCheck>(e.Row);
		}

		protected virtual void APQuickCheck_PrintCheck_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			switch (((APQuickCheck)e.Row).DocType)
			{
				case APDocType.Refund:
				case APDocType.Prepayment:
					e.NewValue = false;
					e.Cancel = true;
					break;
			}
		}

		protected virtual bool MustPrintCheck(APQuickCheck payment)
		{
			if (paymenttype.Current != null && payment != null)
			{
				return payment.Printed == false && paymenttype.Current.PrintOrExport == true;
			}
			return false;
		}

		protected virtual void APQuickCheck_PrintCheck_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (MustPrintCheck((APQuickCheck)e.Row))
			{
				sender.SetValueExt<APQuickCheck.extRefNbr>(e.Row, null);
				sender.SetValueExt<APQuickCheck.hold>(e.Row, true);



			}
		}

		protected virtual void APQuickCheck_AdjDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((APQuickCheck)e.Row).Released == false && ((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				CurrencyInfoAttribute.SetEffectiveDate<APQuickCheck.adjDate>(sender, e);
			}
		}

		protected virtual void APQuickCheck_AdjDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if ((bool)((APQuickCheck)e.Row).VoidAppl == false)
			{
				if (vendor.Current != null && (bool)vendor.Current.Vendor1099)
				{
					string Year1099 = ((DateTime)e.NewValue).Year.ToString();
					AP1099Year year = PXSelect<AP1099Year, Where<AP1099Year.finYear, Equal<Required<AP1099Year.finYear>>>>.Select(this, Year1099);

					if (year != null && year.Status != "N")
					{
						throw new PXSetPropertyException(Messages.AP1099_PaymentDate_NotIn_OpenYear, PXUIFieldAttribute.GetDisplayName<APQuickCheck.adjDate>(sender));
					}
				}
			}
		}

		protected virtual void APQuickCheck_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APQuickCheck doc = (APQuickCheck)e.Row;

			if (doc.CashAccountID == null)
			{
				if (sender.RaiseExceptionHandling<APQuickCheck.cashAccountID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.cashAccountID).Name)))
				{
					throw new PXRowPersistingException(typeof(APQuickCheck.cashAccountID).Name, null, ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.cashAccountID).Name);
				}
			}

			if (string.IsNullOrEmpty(doc.PaymentMethodID))
			{
				if (sender.RaiseExceptionHandling<APQuickCheck.paymentMethodID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.paymentMethodID).Name)))
				{
					throw new PXRowPersistingException(typeof(APQuickCheck.paymentMethodID).Name, null, ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.paymentMethodID).Name);
				}
			}

			if ((bool)((APQuickCheck)e.Row).Hold == false && (bool)((APQuickCheck)e.Row).Released == false)
			{
				if (string.IsNullOrEmpty(doc.ExtRefNbr))
				{
					if (sender.RaiseExceptionHandling<APQuickCheck.extRefNbr>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.extRefNbr).Name)))
					{
						throw new PXRowPersistingException(typeof(APQuickCheck.extRefNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(APQuickCheck.extRefNbr).Name);
					}
				}
			}

			Terms terms = (Terms)PXSelectorAttribute.Select<APQuickCheck.termsID>(Document.Cache, doc);

			if (terms == null)
			{
				sender.SetValue<APQuickCheck.termsID>(doc, null);
				return;
			}

			if (PXCurrencyAttribute.IsNullOrEmpty(terms.DiscPercent) == false &&
				(EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<APQuickCheck.vendorID>>>>.Select(this) != null)
			{
				sender.RaiseExceptionHandling<APQuickCheck.termsID>(doc, doc.TermsID, new PXSetPropertyException(Messages.Employee_Cannot_Have_Discounts, typeof(APQuickCheck.termsID).Name));
			}

			if (terms.InstallmentType == TermsInstallmentType.Multiple)
			{
				sender.RaiseExceptionHandling<APQuickCheck.termsID>(doc, doc.TermsID, new PXSetPropertyException(Messages.Quick_Check_Cannot_Have_Multiply_Installments, typeof(APQuickCheck.termsID).Name));
			}

			PaymentRefAttribute.SetUpdateCashManager<APQuickCheck.extRefNbr>(sender, e.Row, ((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck && ((APQuickCheck)e.Row).DocType != APDocType.Refund);
		}

		protected bool InternalCall = false;

		protected virtual void APQuickCheck_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			APQuickCheck doc = e.Row as APQuickCheck;

			if (doc == null || InternalCall)
			{
				return;
			}

			bool docTypeNotDebitAdj = (doc.DocType != APDocType.DebitAdj);
			PXUIFieldAttribute.SetVisible<APQuickCheck.curyID>(cache, doc, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			PXUIFieldAttribute.SetVisible<APQuickCheck.cashAccountID>(cache, doc, docTypeNotDebitAdj);
			PXUIFieldAttribute.SetVisible<APQuickCheck.cleared>(cache, doc, docTypeNotDebitAdj);
			PXUIFieldAttribute.SetVisible<APQuickCheck.clearDate>(cache, doc, docTypeNotDebitAdj);
			PXUIFieldAttribute.SetVisible<APQuickCheck.paymentMethodID>(cache, doc, docTypeNotDebitAdj);
			PXUIFieldAttribute.SetVisible<APQuickCheck.extRefNbr>(cache, doc, docTypeNotDebitAdj);
			bool ptInstanceVisible = false;

			PXUIFieldAttribute.SetEnabled(Transactions.Cache, null, true);

			if (docTypeNotDebitAdj)
			{
				PaymentTypeInstance ptInstance = PXSelect<PaymentTypeInstance, Where<PaymentTypeInstance.cashAccountID, Equal<Required<APQuickCheck.cashAccountID>>,
					And<PaymentTypeInstance.paymentMethodID, Equal<Required<APQuickCheck.paymentMethodID>>>>>.Select(this, doc.CashAccountID, doc.PaymentMethodID);
				ptInstanceVisible = (ptInstance != null);
			}
			PXUIFieldAttribute.SetVisible<APQuickCheck.pTInstanceID>(cache, doc, ptInstanceVisible);
			//true for DebitAdj and Prepayment Requests
			bool docNotReleased = (doc.Released == false) && (doc.Prebooked!=true);
			bool docReleased = (doc.Released == true) || (doc.Prebooked == true);
			bool docNotOnHold = (doc.Hold == false);
			bool docOnHold = (doc.Hold == true);
			bool curyenabled = false;
			bool clearEnabled = docOnHold && (cashaccount.Current != null) && (cashaccount.Current.Reconcile == true);

			PXUIFieldAttribute.SetRequired<APQuickCheck.cashAccountID>(cache, docNotReleased);
			PXUIFieldAttribute.SetRequired<APQuickCheck.paymentMethodID>(cache, docNotReleased);
			PXUIFieldAttribute.SetRequired<APQuickCheck.extRefNbr>(cache, docNotReleased);
			this.prebook.SetEnabled(false);

			PaymentRefAttribute.SetUpdateCashManager<APQuickCheck.extRefNbr>(cache, e.Row, doc.DocType != APDocType.VoidQuickCheck && doc.DocType != APDocType.Refund);

			//if (vendor.Current != null && !(bool)vendor.Current.AllowOverrideCury)
			//{
			//  curyenabled = false;
			//}
			
			bool isPrebookedNotCompleted = (doc.Prebooked == true && doc.Released == false);
			if (doc.DocType == APDocType.VoidQuickCheck && docNotReleased)
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.adjDate>(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.adjFinPeriodID>(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.docDesc>(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.hold>(cache, doc, true);
				
				cache.AllowUpdate = true;
				cache.AllowDelete = true;
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = false;
				Transactions.Cache.AllowInsert = false;
				release.SetEnabled(docNotOnHold);

				Taxes.Cache.AllowUpdate = false;
			}
			else if (doc.Released == true || doc.Voided == true || doc.Prebooked== true)
			{
				bool Enable1099 = (vendor.Current != null && vendor.Current.Vendor1099 == true && doc.Voided == false);
				string Year1099 = "";
				foreach (APAdjust adj in PXSelect<APAdjust,
							Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>, 
								And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>,
								And<APAdjust.released, Equal<True>>>>>
					.Select(this, doc.DocType, doc.RefNbr))
				{
					Year1099 = ((DateTime)adj.AdjgDocDate).Year.ToString();

					AP1099Year year = PXSelect<AP1099Year, Where<AP1099Year.finYear, Equal<Required<AP1099Year.finYear>>>>.Select(this, Year1099);

					if (year != null && year.Status != "N")
					{
						Enable1099 = false;
					}
				}
				
				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				cache.AllowDelete = false;
				cache.AllowUpdate = Enable1099 || isPrebookedNotCompleted; 
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = Enable1099 || isPrebookedNotCompleted;
				Transactions.Cache.AllowInsert = false;

				Remittance_Address.Cache.AllowUpdate = false;
				Remittance_Contact.Cache.AllowUpdate = false;

				release.SetEnabled(isPrebookedNotCompleted && doc.Voided == false);				
				if (Enable1099)
				{
					PXUIFieldAttribute.SetEnabled(Transactions.Cache, null, false);
					PXUIFieldAttribute.SetEnabled<APTran.box1099>(Transactions.Cache, null, true);
				}

				if (isPrebookedNotCompleted)
				{
					PXUIFieldAttribute.SetEnabled(Transactions.Cache, null, false);
					PXUIFieldAttribute.SetEnabled<APTran.accountID>(Transactions.Cache, null, true);
					PXUIFieldAttribute.SetEnabled<APTran.subID>(Transactions.Cache, null, true);
					PXUIFieldAttribute.SetEnabled<APTran.branchID>(Transactions.Cache, null, true);
				}
				Taxes.Cache.AllowUpdate = false;
			}
			else
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.status>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.curyID>(cache, doc, curyenabled);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.printCheck>(cache, doc, (doc.DocType != APDocType.Prepayment && doc.DocType != APDocType.Refund));

				bool mustPrintCheck = MustPrintCheck(doc);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.hold>(cache, doc, !mustPrintCheck);
				PXUIFieldAttribute.SetEnabled<APQuickCheck.extRefNbr>(cache, doc, !mustPrintCheck);
				cache.RaiseExceptionHandling<APQuickCheck.hold>(doc, null,
															 mustPrintCheck
																 ? new PXSetPropertyException(
																	   Messages.Check_Cannot_Unhold_Until_Printed,
																	   PXErrorLevel.Warning)
																 : null);

				//calculate only on data entry, differences from the applications will be moved to RGOL upon closure
				PXDBCurrencyAttribute.SetBaseCalc<APQuickCheck.curyDocBal>(cache, null, true);
				PXDBCurrencyAttribute.SetBaseCalc<APQuickCheck.curyDiscBal>(cache, null, true);

				cache.AllowDelete = true;
				cache.AllowUpdate = true;
				Transactions.Cache.AllowDelete = true;
				Transactions.Cache.AllowUpdate = true;
				Transactions.Cache.AllowInsert = (doc.VendorID != null) && (doc.VendorLocationID != null);

				Remittance_Address.Cache.AllowUpdate = true;
				Remittance_Contact.Cache.AllowUpdate = true;
				this.release.SetEnabled(docNotOnHold);
				this.prebook.SetEnabled(docNotOnHold);

				Taxes.Cache.AllowUpdate = true;
			}

			bool prepaymentNotReleased = docNotReleased && doc.DocType == APDocType.Prepayment;
			PXUIFieldAttribute.SetEnabled<APQuickCheck.cashAccountID>(cache, doc, docNotReleased);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.paymentMethodID>(cache, doc, docNotReleased);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.pTInstanceID>(cache, doc, docNotReleased);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.cleared>(cache, doc, clearEnabled);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.clearDate>(cache, doc, clearEnabled && doc.Cleared == true);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.docType>(cache, doc);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.refNbr>(cache, doc);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.batchNbr>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.aPAccountID>(cache, doc, prepaymentNotReleased);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.aPSubID>(cache, doc, prepaymentNotReleased);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyDocBal>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyLineTotal>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyTaxTotal>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyOrigWhTaxAmt>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.printed>(cache, doc, doc.DocType != APDocType.VoidQuickCheck);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyVatExemptTotal>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<APQuickCheck.curyVatTaxableTotal>(cache, doc, false);

			voidCheck.SetEnabled((doc.Released == true || doc.Prebooked == true) && doc.Voided == false && doc.DocType == APDocType.QuickCheck);

			if (doc.VendorID != null)
			{
				if (Transactions.Any())
				{
					PXUIFieldAttribute.SetEnabled<APQuickCheck.vendorID>(cache, doc, false);
				}
			}

			PXUIFieldAttribute.SetVisible<APTran.box1099>(Transactions.Cache, null, vendor.Current != null && vendor.Current.Vendor1099 == true);
			APAddress address = this.Remittance_Address.Select();
			bool enableAddressValidation = docNotReleased && (address != null && address.IsDefaultAddress == false && address.IsValidated == false);
			this.validateAddresses.SetEnabled(enableAddressValidation);			
			PXUIFieldAttribute.SetVisible<APQuickCheck.prebookBatchNbr>(cache, doc, doc.Prebooked == true);
			PXUIFieldAttribute.SetVisible<APQuickCheck.voidBatchNbr>(cache, doc, false); //Now void is implemented through VoidQuickCheck

			this.PaymentCharges.Cache.AllowInsert = ((doc.DocType == APDocType.QuickCheck || doc.DocType == APDocType.VoidQuickCheck) && doc.Released != true && doc.Prebooked != true);
			this.PaymentCharges.Cache.AllowUpdate = ((doc.DocType == APDocType.QuickCheck || doc.DocType == APDocType.VoidQuickCheck) && doc.Released != true && doc.Prebooked != true);
			this.PaymentCharges.Cache.AllowDelete = ((doc.DocType == APDocType.QuickCheck || doc.DocType == APDocType.VoidQuickCheck) && doc.Released != true && doc.Prebooked != true);

			Taxes.Cache.AllowDelete = Transactions.Cache.AllowDelete;
			Taxes.Cache.AllowInsert = Transactions.Cache.AllowInsert;

			PXUIFieldAttribute.SetVisible<APQuickCheck.curyTaxAmt>(cache, doc, PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() &&
				(apsetup.Current.RequireControlTaxTotal == true || docReleased));

			bool showRoundingDiff = doc.CuryRoundDiff != 0 || PXAccess.FeatureInstalled<FeaturesSet.invoiceRounding>();
			PXUIFieldAttribute.SetVisible<APQuickCheck.curyRoundDiff>(cache, doc, showRoundingDiff);

			viewSchedule.SetEnabled(true);

			if (UseTaxes.Select().Count != 0)
			{
				cache.RaiseExceptionHandling<APQuickCheck.curyTaxTotal>(doc, doc.CuryTaxTotal, new PXSetPropertyException(TX.Messages.UseTaxExcludedFromTotals, PXErrorLevel.Warning));
			}
		}
		
		protected virtual void APQuickCheck_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			APQuickCheck doc = e.Row as APQuickCheck;
			if (doc.Released != true && doc.Prebooked !=true)
			{
				doc.DocDate = doc.AdjDate;
				doc.FinPeriodID = doc.AdjFinPeriodID;
				doc.TranPeriodID = doc.AdjTranPeriodID;

				sender.RaiseExceptionHandling<APQuickCheck.finPeriodID>(doc, doc.FinPeriodID, null);

				if (sender.ObjectsEqual<APQuickCheck.curyDocBal, APQuickCheck.curyOrigDiscAmt, APQuickCheck.curyOrigWhTaxAmt>(e.Row, e.OldRow) == false && doc.CuryDocBal - doc.CuryOrigDiscAmt - doc.CuryOrigWhTaxAmt != doc.CuryOrigDocAmt)
				{
					if (doc.CuryDocBal != null && doc.CuryOrigDiscAmt != null && doc.CuryOrigWhTaxAmt != null && doc.CuryDocBal != 0)
						sender.SetValueExt<APQuickCheck.curyOrigDocAmt>(doc, doc.CuryDocBal - doc.CuryOrigDiscAmt - doc.CuryOrigWhTaxAmt);
					else
						sender.SetValueExt<APQuickCheck.curyOrigDocAmt>(doc, 0m);
				}
				else if (sender.ObjectsEqual<APQuickCheck.curyOrigDocAmt>(e.Row, e.OldRow) == false)
				{
					if (doc.CuryDocBal != null && doc.CuryOrigDocAmt != null && doc.CuryOrigWhTaxAmt != null && doc.CuryDocBal != 0)
						sender.SetValueExt<APQuickCheck.curyOrigDiscAmt>(doc, doc.CuryDocBal - doc.CuryOrigDocAmt - doc.CuryOrigWhTaxAmt);
					else
						sender.SetValueExt<APQuickCheck.curyOrigDiscAmt>(doc, 0m);
				}

				if (doc.Hold != true && doc.Released != true && doc.Prebooked != true)
				{
					if (doc.CuryDocBal < doc.CuryOrigDocAmt)
					{
						sender.RaiseExceptionHandling<APQuickCheck.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, new PXSetPropertyException(Messages.QuickCheckOutOfBalance));
					}
					else if (doc.CuryOrigDocAmt < 0)
					{
						sender.RaiseExceptionHandling<APQuickCheck.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, new PXSetPropertyException(Messages.DocumentBalanceNegative));
					}
					else
					{
						sender.RaiseExceptionHandling<APQuickCheck.curyOrigDocAmt>(doc, doc.CuryOrigDocAmt, null);
					}
				}

					bool checkControlTaxTotal = apsetup.Current.RequireControlTaxTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>();

				if ((doc.Hold != true || doc.PrintCheck == true) && doc.Released != true && doc.Prebooked != true)
				{
					if (doc.CuryTaxTotal != doc.CuryTaxAmt && checkControlTaxTotal)
					{
						sender.RaiseExceptionHandling<APQuickCheck.curyTaxAmt>(doc, doc.CuryTaxAmt, new PXSetPropertyException(Messages.TaxTotalAmountDoesntMatch));
					}
					else
					{
						if (checkControlTaxTotal)
						{
							sender.RaiseExceptionHandling<APQuickCheck.curyTaxAmt>(doc, null, null);
						}
						else
						{
							sender.SetValueExt<APQuickCheck.curyTaxAmt>(doc, doc.CuryTaxTotal != null && doc.CuryTaxTotal != 0 ? doc.CuryTaxTotal : 0m);
						}
					}
				}

				sender.RaiseExceptionHandling<APQuickCheck.curyRoundDiff>(doc, null, null);

				if (doc.Hold != true && doc.RoundDiff != 0)
				{
					if (checkControlTaxTotal || PXAccess.FeatureInstalled<FeaturesSet.invoiceRounding>() && doc.TaxRoundDiff == 0)
					{
						if (Math.Abs(doc.RoundDiff.Value) > Math.Abs(glsetup.Current.RoundingLimit.Value))
						{
							sender.RaiseExceptionHandling<APQuickCheck.curyRoundDiff>(doc, doc.CuryRoundDiff,
								new PXSetPropertyException(Messages.RoundingAmountTooBig, currencyinfo.Current.BaseCuryID, PXDBQuantityAttribute.Round(doc.RoundDiff),
									PXDBQuantityAttribute.Round(glsetup.Current.RoundingLimit)));
						}
					}
					else
					{
						if (!PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
						{
							sender.RaiseExceptionHandling<APQuickCheck.curyRoundDiff>(doc, doc.CuryRoundDiff,
								new PXSetPropertyException(Messages.CannotEditTaxAmtWOFeature));
						}
						else
						{
							sender.RaiseExceptionHandling<APQuickCheck.curyRoundDiff>(doc, doc.CuryRoundDiff,
								new PXSetPropertyException(Messages.CannotEditTaxAmtWOAPSetup));
						}
					}
				}
			}
		}
		#endregion

		#region CurrencyInfo Events

		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (cashaccount.Current != null && !string.IsNullOrEmpty(cashaccount.Current.CuryID))
				{
					e.NewValue = cashaccount.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (cashaccount.Current != null && !string.IsNullOrEmpty(cashaccount.Current.CuryRateTypeID))
				{
					e.NewValue = cashaccount.Current.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((APQuickCheck)Document.Cache.Current).DocDate;
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

				APQuickCheck doc = PXSelect<APQuickCheck, Where<APQuickCheck.curyInfoID, Equal<Current<CurrencyInfo.curyInfoID>>>>.Select(sender.Graph);
				if (doc != null && doc.DocType == APDocType.VoidQuickCheck)
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

		#region APTran Events
		protected virtual void APTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran row = (APTran)e.Row;
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
			APTran row = (APTran)e.Row;
			if (row == null || row.AccountID == null || vendor.Current == null || vendor.Current.Type == null) return;
			if (!String.IsNullOrEmpty(row.PONbr) || !String.IsNullOrEmpty(row.ReceiptNbr)) return;
			InventoryItem item = nonStockItem.Select(row.InventoryID);
			EPEmployee employeeByUser = (EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.Select(this, PXAccess.GetUserID());
			CRLocation companyloc =
				PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>, Where<Branch.branchID, Equal<Required<APTran.branchID>>>>.Select(this, row.BranchID);
			CT.Contract project = PXSelect<CT.Contract, Where<CT.Contract.contractID, Equal<Required<CT.Contract.contractID>>>>.Select(this, row.ProjectID);
			string expenseSubMask = apsetup.Current.ExpenseSubMask;
			int? projectTask_SubID = null;
			if (project == null || project.BaseType == CT.Contract.ContractBaseType.Contract)
			{
				project = PXSelect<CT.Contract, Where<CT.Contract.nonProject, Equal<True>>>.Select(this);
				expenseSubMask = expenseSubMask.Replace(APAcctSubDefault.MaskTask, APAcctSubDefault.MaskProject);
			}
			else
			{
				PM.PMTask task = PXSelect<PM.PMTask, Where<PM.PMTask.taskID, Equal<Required<PM.PMTask.taskID>>>>.Select(this, row.TaskID);
				if (task != null)
					projectTask_SubID = task.DefaultSubID;
			}
			int? vendor_SubID = null;
			switch (vendor.Current.Type)
			{
				case BAccountType.VendorType:
				case BAccountType.CombinedType:
					if (location.Current.VExpenseAcctID != null)
					{
						vendor_SubID = (int?)Caches[typeof(Location)].GetValue<Location.vExpenseSubID>(location.Current);
					}
					break;
				case BAccountType.EmployeeType:
					if (employee.Current.ExpenseAcctID != null)
					{
						vendor_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(employee.Current);
					}
					break;
			}


			int? item_SubID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.cOGSSubID>(item);
			int? employeeByUser_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(employeeByUser);
			int? company_SubID = (int?)Caches[typeof(CRLocation)].GetValue<CRLocation.cMPExpenseSubID>(companyloc);
			int? project_SubID = project.DefaultSubID;

			object value = SubAccountMaskAttribute.MakeSub<APSetup.expenseSubMask>(this, apsetup.Current.ExpenseSubMask,
				new object[] { vendor_SubID, item_SubID, employeeByUser_SubID, company_SubID, project_SubID, projectTask_SubID },
								new Type[] { typeof(Location.vExpenseSubID), typeof(InventoryItem.cOGSSubID), typeof(EPEmployee.expenseSubID), typeof(Location.cMPExpenseSubID), typeof(PM.PMProject.defaultSubID), typeof(PM.PMTask.defaultSubID) });

			sender.RaiseFieldUpdating<APTran.subID>(row, ref value);

			e.NewValue = (int?)value;
			e.Cancel = true;

		}

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[APQuickCheckTax(typeof(APQuickCheck), typeof(APTax), typeof(APTaxTran), typeof(APQuickCheck.taxCalcMode))]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void APTran_TaxCategoryID_CacheAttached(PXCache sender)
		{ 
		}

		#region APPaymentChargeTran
		#region LineNbr
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(APQuickCheck.chargeCntr), DecrementOnDelete = false)]
		public virtual void APPaymentChargeTran_LineNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region CashAccountID
		[PXDBInt()]
		[PXDBDefault(typeof(APQuickCheck.cashAccountID))]
		[PXUIField(DisplayName = "Cash AccountID")]
		public virtual void APPaymentChargeTran_CashAccountID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region EntryTypeID
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId,
							InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
							Where<CashAccountETDetail.accountID, Equal<Current<APQuickCheck.cashAccountID>>,
							And<CAEntryType.drCr, Equal<CADrCr.cACredit>>>>))]
		[PXUIField(DisplayName = "Entry Type")]
		public virtual void APPaymentChargeTran_EntryTypeID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region TranDate
		[PXDBDate()]
		[PXDBDefault(typeof(APQuickCheck.adjDate))]
		[PXUIField(DisplayName = "TranDate")]
		public virtual void APPaymentChargeTran_TranDate_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region FinPeriodID
		[PXDBString(6, IsFixed = true)]
		[PXDBDefault(typeof(APQuickCheck.adjFinPeriodID))]
		[PXUIField(DisplayName = "FinPeriodID")]
		public virtual void APPaymentChargeTran_FinPeriodID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region TranPeriodID
		[PXDBString(6, IsFixed = true)]
		[PXDBDefault(typeof(APQuickCheck.adjTranPeriodID))]
		[PXUIField(DisplayName = "TranPeriodID")]
		public virtual void APPaymentChargeTran_TranPeriodID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion

		protected virtual void APTran_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran row = (APTran)e.Row;
			if (row == null || row.InventoryID != null || vendor == null || vendor.Current == null || vendor.Current.TaxAgency == true) return;

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
			if (tran == null) return;
			if (!PXCurrencyAttribute.IsNullOrEmpty(tran.UnitCost))
			{
				decimal CuryUnitCost = 0m;
				PXCurrencyAttribute.CuryConvCury<APTran.curyInfoID>(sender, tran, (decimal)tran.UnitCost, out CuryUnitCost);
				e.NewValue = INUnitAttribute.ConvertToBase<APTran.inventoryID>(sender, tran, tran.UOM, CuryUnitCost, INPrecision.UNITCOST);
				e.Cancel = true;
			}

			APQuickCheck doc = this.Document.Current;
			if (doc != null && doc.VendorID != null)
			{
				if (tran != null && tran.InventoryID != null && tran.UOM != null)
				{
					DateTime date = Document.Current.DocDate.Value;
					e.NewValue = APVendorPriceMaint.CalculateUnitCost(sender, tran.VendorID, doc.VendorLocationID, tran.InventoryID, currencyinfo.Select(), tran.UOM, tran.Qty, date, tran.CuryUnitCost) ?? 0m;
					e.Cancel = true;
				}
			}
		}

		protected virtual void APTran_Qty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row != null && row.Qty != 0)
			{
				sender.SetDefaultExt<APTran.curyUnitCost>(e.Row);
			}
		}

		protected virtual void APTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran tran = (APTran)e.Row;
			sender.SetDefaultExt<APTran.unitCost>(tran);
			sender.SetDefaultExt<APTran.curyUnitCost>(tran);
			sender.SetValue<APTran.unitCost>(tran, null);
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

					sender.SetDefaultExt<APTran.unitCost>(tran);
					sender.SetDefaultExt<APTran.curyUnitCost>(tran);
					sender.SetValue<APTran.unitCost>(tran, null);

					InventoryItem item = nonStockItem.Select(tran.InventoryID);
					if (item != null)
					{
						tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, "Descr", vendor.Current?.LocaleName);
					}
				}
			}
		}

		[ProjectDefault(BatchModule.AP, typeof(Search<Location.vDefProjectID, 
															Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, 
																	And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>>>>), 
			typeof(APTran.accountID))]
		[APActiveProjectAttibute()]
		protected virtual void APTran_ProjectID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void APTran_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row == null) return;

			sender.SetDefaultExt<APTran.subID>(row);
		}

		protected virtual void APTran_TaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran row = e.Row as APTran;
			if (row == null) return;

			sender.SetDefaultExt<APTran.subID>(row);
		}

		
		protected virtual void APTran_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			APTran row = e.Row as APTran;

			if (row == null)
				return;

			if (row != null && !(String.IsNullOrEmpty(row.PONbr) && String.IsNullOrEmpty(row.ReceiptNbr)))
			{
				PXUIFieldAttribute.SetEnabled<APTran.inventoryID>(cache, row, false);
				PXUIFieldAttribute.SetEnabled<APTran.uOM>(cache, row, false);
				PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, false);
				PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, false);
			}

			bool isProjectEditable = string.IsNullOrEmpty(row.PONbr);

			InventoryItem ns = (InventoryItem)PXSelectorAttribute.Select(cache, e.Row, cache.GetField(typeof(APTran.inventoryID)));
			if (ns != null && ns.StkItem != true && ns.NonStockReceipt != true)
			{
				isProjectEditable = true;
			}

			PXUIFieldAttribute.SetEnabled<APTran.projectID>(cache, row, isProjectEditable);
			PXUIFieldAttribute.SetEnabled<APTran.taskID>(cache, row, isProjectEditable);
		}

		protected virtual void APTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<APTran.box1099>(e.Row, e.OldRow))
			{
				foreach (APAdjust adj in PXSelect<APAdjust, Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>, And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>, And<APAdjust.released, Equal<True>>>>>.Select(this, ((APTran)e.Row).TranType, ((APTran)e.Row).RefNbr))
				{
					APReleaseProcess.Update1099Hist(this, -1m, adj, (APTran)e.OldRow, Document.Current);
					APReleaseProcess.Update1099Hist(this, 1m, adj, (APTran)e.Row, Document.Current);
				}
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

		protected virtual void APTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Row == null) return;

			DR.ScheduleHelper.DeleteAssociatedScheduleIfDeferralCodeChanged(this, e.Row as APTran);
		}

		protected virtual void AP1099Hist_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (((AP1099Hist)e.Row).BoxNbr == null)
			{
				e.Cancel = true;
			}
		}

		protected virtual void APTran_DrCr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null)
			{
				e.NewValue = APInvoiceType.DrCr(Document.Current.DocType);
				e.Cancel = true;
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

			PXUIFieldAttribute.SetEnabled<APTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}

		protected virtual void APTaxTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (Document.Current != null && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				((APTaxTran)e.Row).TaxZoneID = Document.Current.TaxZoneID;
			}
		}

		[PXDBDefault(typeof(APRegister.branchID))]
		[Branch(Enabled = false)]
		protected virtual void APTaxTran_BranchID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region APPaymentChargeTran Events
		public virtual void APPaymentChargeTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APPaymentChargeTran charge = (APPaymentChargeTran)e.Row;
			if (charge.CuryTranAmt <= 0m)
			{
				sender.RaiseExceptionHandling<APPaymentChargeTran.curyTranAmt>(e.Row, charge.CuryTranAmt, new PXSetPropertyException(CS.Messages.Entry_GT, "0"));
			}
			if (charge.AccountID == null)
			{
				sender.RaiseExceptionHandling<APPaymentChargeTran.accountID>(e.Row, charge.AccountID, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPaymentChargeTran.accountID).Name));
			}
			if (charge.SubID == null)
			{
				sender.RaiseExceptionHandling<APPaymentChargeTran.subID>(e.Row, charge.SubID, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APPaymentChargeTran.subID).Name));
			}

		}
		#endregion


		#region Voiding
		private bool _IsVoidCheckInProgress = false;

		protected virtual void APQuickCheck_RefNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_IsVoidCheckInProgress)
			{
				e.Cancel = true;
			}
		}

		protected virtual void APQuickCheck_FinPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_IsVoidCheckInProgress)
			{
				e.Cancel = true;
			}
		}

		protected virtual void APQuickCheck_AdjFinPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_IsVoidCheckInProgress)
			{
				e.Cancel = true;
			}
		}

		public virtual void VoidCheckProc(APQuickCheck doc)
		{
			this.Clear(PXClearOption.PreserveTimeStamp);
			this.Document.View.Answer = WebDialogResult.No;

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(this.Transactions.Cache, null, TaxCalc.NoCalc);

			foreach (PXResult<APQuickCheck, CurrencyInfo> res in PXSelectJoin<APQuickCheck, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APQuickCheck.curyInfoID>>>, Where<APQuickCheck.docType, Equal<Required<APQuickCheck.docType>>, And<APQuickCheck.refNbr, Equal<Required<APQuickCheck.refNbr>>>>>.Select(this, (object)doc.DocType, doc.RefNbr))
			{
				CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
				info.CuryInfoID = null;
				info.IsReadOnly = false;
				info = PXCache<CurrencyInfo>.CreateCopy(this.currencyinfo.Insert(info));

				APQuickCheck payment = new APQuickCheck();
				payment.DocType = APDocType.VoidQuickCheck;
				payment.RefNbr = ((APQuickCheck)res).RefNbr;
				payment.CuryInfoID = info.CuryInfoID;

				Document.Insert(payment);

				payment = PXCache<APQuickCheck>.CreateCopy((APQuickCheck)res);

				payment.DocType = APDocType.VoidQuickCheck;
				payment.CuryInfoID = info.CuryInfoID;
				payment.CATranID = null;
				payment.NoteID = null;

				//must set for _RowSelected
				payment.OpenDoc = true;
				payment.Released = false;
				if(doc.Released == true)
					payment.PrebookBatchNbr = null;
				payment.Prebooked = false; //Temporary, Check Later.  Rigth now only the Prebooked & Released Quick Checks may be voided
				Document.Cache.SetDefaultExt<APQuickCheck.hold>(payment);
				payment.LineCntr = 0;
				payment.BatchNbr = null;
				payment.AdjDate = doc.DocDate;
				payment.AdjFinPeriodID = doc.AdjFinPeriodID;
				payment.CuryDocBal = payment.CuryOrigDocAmt + payment.CuryOrigDiscAmt;
				payment.CuryChargeAmt = 0;
				payment.CuryVatTaxableTotal = 0;
				payment.ClosedFinPeriodID = null;
				payment.ClosedTranPeriodID = null;
				payment.Printed	= true;
				payment = this.Document.Update(payment);

				if (info != null)
				{
					CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APQuickCheck.curyInfoID>>>>.Select(this, null);
					b_info.CuryID = info.CuryID;
					b_info.CuryEffDate = info.CuryEffDate;
					b_info.CuryRateTypeID = info.CuryRateTypeID;
					b_info.CuryRate = info.CuryRate;
					b_info.RecipRate = info.RecipRate;
					b_info.CuryMultDiv = info.CuryMultDiv;
					this.currencyinfo.Update(b_info);
				}
			}

			foreach (APTran srcTran in PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				APTran tran = PXCache<APTran>.CreateCopy(srcTran);
				tran.TranType = null;
				tran.RefNbr = null;
				tran.DrCr = null;
				tran.Released = null;
				tran.CuryInfoID = null;
				tran.NoteID = null;

				tran = Transactions.Insert(tran);
				PXNoteAttribute.CopyNoteAndFiles(Transactions.Cache, srcTran, Transactions.Cache, tran);
			}

			TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APQuickCheckTaxAttribute>(this.Transactions.Cache, null, TaxCalc.ManualCalc);

			foreach (APTaxTran tax in PXSelect<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				APTaxTran new_artax = new APTaxTran();
				new_artax.TaxID = tax.TaxID;

				new_artax = this.Taxes.Insert(new_artax);

				if (new_artax != null)
				{
					new_artax = PXCache<APTaxTran>.CreateCopy(new_artax);
					new_artax.TaxRate = tax.TaxRate;
					new_artax.CuryTaxableAmt = tax.CuryTaxableAmt;
					new_artax.CuryTaxAmt = tax.CuryTaxAmt;
					new_artax = this.Taxes.Update(new_artax);
				}
			}

			foreach (PXResult<APPaymentChargeTran> paycharge in PXSelect<APPaymentChargeTran, Where<APPaymentChargeTran.docType, Equal<Required<APPayment.docType>>, And<APPaymentChargeTran.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				APPaymentChargeTran charge = PXCache<APPaymentChargeTran>.CreateCopy((APPaymentChargeTran)paycharge);
				charge.DocType = Document.Current.DocType;
				charge.Released = false;
				charge.CashTranID = null;
				charge.CuryInfoID = Document.Current.CuryInfoID;
				PaymentCharges.Insert(charge);
			}
		}
		#endregion

	}

	public class APDataEntryGraph<TGraph, TPrimary> : PXGraph<TGraph, TPrimary>, PX.Objects.GL.IVoucherEntry
		where TGraph : PXGraph
		where TPrimary : APRegister, new()
	{
		public PXAction DeleteButton
		{
			get
			{
				return this.Delete;
			}
		}
		public PXAction<TPrimary> release;
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<TPrimary> voidCheck;
		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update, Visible = false)]
		[PXProcessButton]
		public virtual IEnumerable VoidCheck(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<TPrimary> viewBatch;
		[PXUIField(DisplayName = "Review Batch", Visible = false, MapEnableRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			foreach (TPrimary apdoc in adapter.Get<TPrimary>())
			{
				if (!String.IsNullOrEmpty(apdoc.BatchNbr))
				{
					JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
					graph.BatchModule.Current = PXSelect<Batch,
						Where<Batch.module, Equal<BatchModule.moduleAP>,
						And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>>
						.Select(this, apdoc.BatchNbr);
					throw new PXRedirectRequiredException(graph, "Current batch record");
				}
			}
			return adapter.Get();
		}

		public PXAction<TPrimary> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights=PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
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
			return adapter.Get();
		}

		public PXAction<TPrimary> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
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

		public PXAction<TPrimary> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Report(PXAdapter adapter,
			[PXString(8)]
			[PXStringList(new string[] { "AP610500", "AP622000", "AP622500"}, new string[] { "AP Edit", "AP Register Detailed", "AP Payment Register" })]
			string reportID
			)
		{
			foreach (TPrimary doc in adapter.Get<TPrimary>())
			{
				object FinPeriodID;

				if (this.Caches[typeof(TPrimary)].GetStatus(doc) == PXEntryStatus.Notchanged)
				{
					this.Caches[typeof(TPrimary)].SetStatus(doc, PXEntryStatus.Updated);
				}

				this.Save.Press();

				var docPeriod = (FinPeriodID = this.Caches[typeof(TPrimary)].GetValueExt<APRegister.finPeriodID>(doc)) is PXFieldState ? (string)((PXFieldState)FinPeriodID).Value : (string)FinPeriodID;

				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["PeriodFrom"] = docPeriod;
				parameters["PeriodTo"] = docPeriod;
				parameters["BranchID"] = null;
				parameters["DocType"] = doc.DocType;
				parameters["RefNbr"] = doc.RefNbr;
				throw new PXReportRequiredException(parameters, reportID, "Report");
			}
			return adapter.Get();
		}

		public APDataEntryGraph()
			: base()
		{
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}
	}
}
