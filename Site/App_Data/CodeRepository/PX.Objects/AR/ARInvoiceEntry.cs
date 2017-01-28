using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PX.Common;
using PX.Objects.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.BQLConstants;
using PX.Objects.EP;
using PX.Objects.SO;
using PX.Objects.DR;

using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;

using SOInvoice = PX.Objects.SO.SOInvoice;
using SOInvoiceEntry = PX.Objects.SO.SOInvoiceEntry;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.GL.Reclassification.UI;

namespace PX.Objects.AR
{
	[Serializable]
	public class ARInvoiceEntry : ARDataEntryGraph<ARInvoiceEntry, ARInvoice>, PXImportAttribute.IPXPrepareItems
	{
		#region Cache Attached
		#region InventoryItem
		#region COGSSubID
		[PXDefault(typeof(Search<INPostClass.cOGSSubID, Where<INPostClass.postClassID, Equal<Current<InventoryItem.postClassID>>>>))]
		[SubAccount(typeof(InventoryItem.cOGSAcctID), DisplayName = "Expense Sub.", DescriptionField = typeof(Sub.description))]
		public virtual void InventoryItem_COGSSubID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion
		#region ARSalesPerTran
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(ARInvoice.docType))]
		protected virtual void ARSalesPerTran_DocType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(ARInvoice.refNbr))]
		[PXParent(typeof(Select<ARInvoice, Where<ARInvoice.docType, Equal<Current<ARSalesPerTran.docType>>,
						 And<ARInvoice.refNbr, Equal<Current<ARSalesPerTran.refNbr>>>>>))]
		protected virtual void ARSalesPerTran_RefNbr_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDBDefault(typeof(ARInvoice.branchID), DefaultForInsert = true, DefaultForUpdate = true)]
		protected virtual void ARSalesPerTran_BranchID_CacheAttached(PXCache sender)
		{
		}

		[SalesPerson(DirtyRead = true, Enabled = false, IsKey = true, DescriptionField = typeof(Contact.displayName))]
		protected virtual void ARSalesPerTran_SalespersonID_CacheAttached(PXCache sender)
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault(0)]
		protected virtual void ARSalesPerTran_AdjNbr_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(3, IsFixed = true, IsKey = true)]
		[PXDefault(ARDocType.Undefined)]
		protected virtual void ARSalesPerTran_AdjdDocType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault("")]
		protected virtual void ARSalesPerTran_AdjdRefNbr_CacheAttached(PXCache sender)
		{
		}
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Coalesce<Search<CustSalesPeople.commisionPct, Where<CustSalesPeople.bAccountID, Equal<Current<ARInvoice.customerID>>,
				And<CustSalesPeople.locationID, Equal<Current<ARInvoice.customerLocationID>>,
				And<CustSalesPeople.salesPersonID, Equal<Current<ARSalesPerTran.salespersonID>>>>>>,
			Search<SalesPerson.commnPct, Where<SalesPerson.salesPersonID, Equal<Current<ARSalesPerTran.salespersonID>>>>>))]
		[PXUIField(DisplayName = "Commission %")]
		protected virtual void ARSalesPerTran_CommnPct_CacheAttached(PXCache sender)
		{
		}
		[PXDBCurrency(typeof(ARSalesPerTran.curyInfoID), typeof(ARSalesPerTran.commnblAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Commissionable Amount", Enabled = false)]
		[PXFormula(null, typeof(SumCalc<ARInvoice.curyCommnblAmt>))]
		protected virtual void ARSalesPerTran_CuryCommnblAmt_CacheAttached(PXCache sender)
		{
		}
		[PXDBCurrency(typeof(ARSalesPerTran.curyInfoID), typeof(ARSalesPerTran.commnAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<ARSalesPerTran.curyCommnblAmt, Div<ARSalesPerTran.commnPct, decimal100>>), typeof(SumCalc<ARInvoice.curyCommnAmt>))]
		[PXUIField(DisplayName = "Commission Amt.", Enabled = false)]
		protected virtual void ARSalesPerTran_CuryCommnAmt_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region PMTran
		[PXDBString(3, IsFixed = true)]
		[PXDBDefault(typeof(ARInvoice.docType), PersistingCheck = PXPersistingCheck.Nothing)]
		public void PMTran_ARTranType_CacheAttached(PXCache sender) { }

		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "AR Reference Nbr.")]
		[PXDBDefault(typeof(ARInvoice.refNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		public void PMTran_ARRefNbr_CacheAttached(PXCache sender) { }

		[PXDBInt]
		[PXDBDefault(typeof(ARTran.lineNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		public void PMTran_RefLineNbr_CacheAttached(PXCache sender) { }
		#endregion
		[PXDBDefault(typeof(ARRegister.branchID))]
		[Branch(Enabled = false)]
		protected virtual void ARTaxTran_BranchID_CacheAttached(PXCache sender)
		{
		}

		[PXHidden]
		public PXSelect<CR.CROpportunity>
			OpportunityBackReference;

		protected virtual IEnumerable opportunityBackReference()
		{
			return OpportunityBackReference.Cache.Updated;
		}
		#endregion
		#region Actions
		public ToggleCurrency<ARInvoice> CurrencyView;

		public PXAction<ARInvoice> viewSchedule;
		[PXUIField(DisplayName = "View Schedule", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			ARTran currentLine = Transactions.Current;

			if (currentLine != null && 
				Transactions.Cache.GetStatus(currentLine) == PXEntryStatus.Notchanged)
			{
				Save.Press();
				ViewScheduleForLine(this, Document.Current, currentLine);
			}

			return adapter.Get();
		}

		public static void ViewScheduleForLine(PXGraph graph, ARRegister document, ARTran documentLine)
		{
			PXSelectBase<DRSchedule> correspondingScheduleView = new PXSelect<
				DRSchedule,
					Where<
						DRSchedule.module, Equal<BatchModule.moduleAR>,
				And<DRSchedule.docType, Equal<Current<ARTran.tranType>>,
				And<DRSchedule.refNbr, Equal<Current<ARTran.refNbr>>,
						And<DRSchedule.lineNbr, Equal<Current<ARTran.lineNbr>>>>>>>
				(graph);

			DRSchedule correspondingSchedule = correspondingScheduleView.Select();

			if (correspondingSchedule == null || correspondingSchedule.IsDraft == true)
			{
				PXResult<ARTax, Tax> tax = (PXResult<ARTax, Tax>)PXSelectJoin<
					ARTax,
						LeftJoin<Tax, On<Tax.taxID, Equal<ARTax.taxID>>>,
					Where<
						ARTax.tranType, Equal<Required<ARTax.tranType>>, 
						And<ARTax.refNbr, Equal<Required<ARTax.refNbr>>, 
						And<ARTax.lineNbr, Equal<Required<ARTax.lineNbr>>>>>>
					.Select(graph, documentLine.TranType, documentLine.RefNbr, documentLine.LineNbr);

				var actualAmount = ARReleaseProcess.GetSalesPostingAmount(graph, document, documentLine, tax, tax,
					amount => PXDBCurrencyAttribute.Round(graph.Caches[typeof(ARTran)], documentLine, amount, CMPrecision.TRANCURY));

				DRDeferredCode deferralCode = PXSelect<
					DRDeferredCode, 
					Where<
						DRDeferredCode.deferredCodeID, Equal<Current2<ARTran.deferredCode>>>>
					.Select(graph);

				if (deferralCode != null)
				{
					DRProcess process = PXGraph.CreateInstance<DRProcess>();
					process.CreateSchedule(documentLine, deferralCode, document, actualAmount.Base.Value, isDraft: true);
					process.Actions.PressSave();

					correspondingScheduleView.Cache.Clear();
					correspondingScheduleView.Cache.ClearQueryCache();

					correspondingSchedule = correspondingScheduleView.Select();
				}
			}

			if (correspondingSchedule != null)
			{
				PXRedirectHelper.TryRedirect(
					graph.Caches[typeof(DRSchedule)],
					correspondingSchedule,
					"View Schedule",
					PXRedirectHelper.WindowMode.NewWindow);
			}
		}


		public PXAction<ARInvoice> newCustomer;
		[PXUIField(DisplayName = "New Customer", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewCustomer(PXAdapter adapter)
		{
			CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
			throw new PXRedirectRequiredException(graph, "New Customer");
		}

		public PXAction<ARInvoice> editCustomer;
		[PXUIField(DisplayName = "Edit Customer", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditCustomer(PXAdapter adapter)
		{
			if (customer.Current != null)
			{
				CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
				graph.BAccount.Current = customer.Current;
				throw new PXRedirectRequiredException(graph, "Edit Customer");
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> customerDocuments;
		[PXUIField(DisplayName = "Customer Details", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable CustomerDocuments(PXAdapter adapter)
		{
			if (customer.Current != null)
			{
				ARDocumentEnq graph = PXGraph.CreateInstance<ARDocumentEnq>();
				graph.Filter.Current.CustomerID = customer.Current.BAccountID;
				graph.Filter.Select();
				throw new PXRedirectRequiredException(graph, "Customer Details");
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> sOInvoice;
		[PXUIField(DisplayName = "SO Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable SOInvoice(PXAdapter adapter)
		{
			ARInvoice invoice = Document.Current;
			SOInvoiceEntry graph = CreateInstance<SOInvoiceEntry>();
			graph.Document.Current = graph.Document.Search<ARInvoice.refNbr>(invoice.RefNbr, invoice.DocType);
			throw new PXRedirectRequiredException(graph, "SO Invoice");
		}

		public PXAction<ARInvoice> sendARInvoiceMemo;
		[PXUIField(DisplayName = "Send AR Invoice/Memo", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable SendARInvoiceMemo(PXAdapter adapter,
					[PXString]
					string reportID
			)
		{
			ARInvoice invoice = Document.Current;
			if (reportID == null) reportID = "AR641000";
			if (invoice != null)
			{
				Dictionary<string, string> mailParams = new Dictionary<string, string>();
				mailParams["DocType"] = invoice.DocType;
				mailParams["RefNbr"] = invoice.RefNbr;
				if (!ReportNotificationGenerator.Send(reportID, mailParams).Any())
				{
					throw new PXException(ErrorMessages.MailSendFailed);
				}
				Clear();
				Document.Current = Document.Search<ARInvoice.refNbr>(invoice.RefNbr, invoice.DocType);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<ARRegister> list = new List<ARRegister>();
			foreach (ARInvoice ardoc in adapter.Get<ARInvoice>())
			{
				if (ardoc.Hold == false && ardoc.Released == false)
				{
					if (cache.GetStatus(ardoc) == PXEntryStatus.Notchanged) cache.SetStatus(ardoc, PXEntryStatus.Updated);
					list.Add(ardoc);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}

			skipAvalaraCallOnSave = true;
			Save.Press();

			PXLongOperation.StartOperation(this, delegate()
			{
				PXTimeStampScope.SetRecordComesFirst(typeof(ARInvoice), true);

				List<ARRegister> listWithTax = new List<ARRegister>();
				foreach (ARInvoice ardoc in list)
				{
					if (ardoc.IsTaxValid != true && ardoc.InstallmentNbr == null && AvalaraMaint.IsExternalTax(this, ardoc.TaxZoneID))
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

				ARDocumentRelease.ReleaseDoc(listWithTax, false, null, (a, b) => { });
			});

			return list;
		}

		public PXAction<ARInvoice> writeOff;
		[PXUIField(DisplayName = "Write Off", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable WriteOff(PXAdapter adapter)
		{
			if (Document.Current != null && (Document.Current.DocType == ARDocType.Invoice || Document.Current.DocType == ARDocType.DebitMemo || Document.Current.DocType == ARDocType.CreditMemo
				 || Document.Current.DocType == ARDocType.FinCharge || Document.Current.DocType == ARDocType.SmallCreditWO))
			{
				this.Save.Press();

				Customer c = customer.Select(Document.Current.CustomerID);
				if (c != null)
				{
					if (c.SmallBalanceAllow != true)
					{
						throw new PXException(Messages.WriteOffIsDisabled);
					}
					else if (c.SmallBalanceLimit < Document.Current.CuryDocBal)
					{
						decimal limit = c.SmallBalanceLimit ?? 0m;
						int precision = currencyinfo.Current != null && currencyinfo.Current.BasePrecision != null ? (int)currencyinfo.Current.BasePrecision : 2;
						throw new PXException(Messages.WriteOffIsOutOfLimit, limit.ToString("N" + precision));
					}
				}

				ARCreateWriteOff target = PXGraph.CreateInstance<ARCreateWriteOff>();
				if (Document.Current.DocType == ARDocType.CreditMemo)
					target.Filter.Cache.SetValueExt<ARWriteOffFilter.woType>(target.Filter.Current, ARWriteOffType.SmallCreditWO);
				target.Filter.Cache.SetValueExt<ARWriteOffFilter.branchID>(target.Filter.Current, Document.Current.BranchID);
				target.Filter.Cache.SetValueExt<ARWriteOffFilter.customerID>(target.Filter.Current, Document.Current.CustomerID);

				foreach (PX.Objects.AR.ARCreateWriteOff.ARRegisterEx doc in target.ARDocumentList.Select())
				{
					if (doc.DocType == Document.Current.DocType && doc.RefNbr == Document.Current.RefNbr)
					{
						doc.Selected = true;
						target.ARDocumentList.Update(doc);
					}
				}

				throw new PXRedirectRequiredException(target, "Create Write-Off");
			}

			return adapter.Get();
		}

		public PXAction<ARInvoice> reverseInvoice;
		[PXUIField(DisplayName = "Reverse Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReverseInvoice(PXAdapter adapter)
		{
			if (Document.Current != null)
			{	
				string docType = Document.Current.DocType;
				if (docType == ARDocType.Invoice || docType == ARDocType.DebitMemo || docType == ARDocType.CreditMemo)
				{
					bool reverse = true;
					string refNbr = Document.Current.RefNbr;
					string reversingDocType = GetReversingDocType(docType);

					ARRegister reversingDoc = PXSelect<ARRegister,
						Where<ARRegister.docType, Equal<Required<ARRegister.docType>>,
							And<ARRegister.origDocType, Equal<Required<ARRegister.origDocType>>,
								And<ARRegister.origRefNbr, Equal<Required<ARRegister.origRefNbr>>>>>,
						OrderBy<Desc<ARRegister.createdDateTime>>>.SelectSingleBound(this, null, reversingDocType, docType, refNbr);

					if (reversingDoc != null)
					{
						string descr;
						ARDocType.ListAttribute list = new ARDocType.ListAttribute();
						list.ValueLabelDic.TryGetValue(reversingDocType, out descr);
						reverse = Document.View.Ask(PXMessages.LocalizeFormatNoPrefix(Messages.ReversingDocumentExists, descr, reversingDoc.RefNbr), MessageButtons.YesNo) == WebDialogResult.Yes;
					}
				
					if (reverse)
					{
						if (Document.Current.InstallmentNbr != null && string.IsNullOrEmpty(Document.Current.MasterRefNbr) == false)
						{
							throw new PXSetPropertyException(Messages.Multiply_Installments_Cannot_be_Reversed, Document.Current.MasterRefNbr);
						}

						this.Save.Press();
						ARInvoice doc = PXCache<ARInvoice>.CreateCopy(Document.Current);
						FiscalPeriodUtils.VerifyFinPeriod<ARInvoice.finPeriodID, FinPeriod.aRClosed>(this, Document.Cache, doc, finperiod);

						try
						{
							this.ReverseInvoiceProc(doc);

							Document.Cache.RaiseExceptionHandling<ARInvoice.finPeriodID>(Document.Current, Document.Current.FinPeriodID, null);

							return new List<ARInvoice> {Document.Current};
						}
						catch (PXException)
						{
							this.Clear(PXClearOption.PreserveTimeStamp);
							Document.Current = doc;
							throw;
						}
					}
				}
			}
			
			return adapter.Get();
		}

		public PXAction<ARInvoice> payInvoice;
		[PXUIField(DisplayName = "Enter Payment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable PayInvoice(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.Released == true)
			{
				ARPaymentEntry pe = PXGraph.CreateInstance<ARPaymentEntry>();

				if (Document.Current.Payable == true && Document.Current.OpenDoc == true)
				{
					if (Document.Current.PendingPPD == true)
					{
						throw new PXSetPropertyException(Messages.PaidPPD);
					}
					
					ARAdjust2 adj = PXSelect<ARAdjust2, Where<ARAdjust2.adjdDocType, Equal<Current<ARInvoice.docType>>,
						And<ARAdjust2.adjdRefNbr, Equal<Current<ARInvoice.refNbr>>, And<ARAdjust2.released, Equal<False>, And<ARAdjust2.voided, Equal<False>>>>>>.Select(this);
					if (adj != null)
					{
						pe.Document.Current = pe.Document.Search<ARPayment.refNbr>(adj.AdjgRefNbr, adj.AdjgDocType);
					}
					else
					{
						pe.CreatePayment(Document.Current);
					}
					throw new PXRedirectRequiredException(pe, "PayInvoice");
				}
				else if (Document.Current.DocType == ARDocType.CreditMemo)
				{
					pe.Document.Current = pe.Document.Search<ARPayment.refNbr>(Document.Current.RefNbr, Document.Current.DocType);
					throw new PXRedirectRequiredException(pe, "PayInvoice");
				}
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> createSchedule;
		[PXUIField(DisplayName = "Assign to Schedule", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton(ImageKey = PX.Web.UI.Sprite.Main.Shedule)]
		public virtual IEnumerable CreateSchedule(PXAdapter adapter)
		{
			this.Save.Press();
			if (Document.Current != null && (bool)Document.Current.Released == false && (bool)Document.Current.Hold == false)
			{
				PX.Objects.AR.ARScheduleMaint sm = PXGraph.CreateInstance<PX.Objects.AR.ARScheduleMaint>();
				if ((bool)Document.Current.Scheduled && Document.Current.ScheduleID != null)
				{
					sm.Schedule_Header.Current = sm.Schedule_Header.Search<Schedule.scheduleID>(Document.Current.ScheduleID);
				}
				else
				{
					sm.Schedule_Header.Cache.Insert();
					ARRegister doc = (ARRegister)sm.Document_Detail.Cache.CreateInstance();
					PXCache<ARRegister>.RestoreCopy(doc, Document.Current);
					doc = (ARRegister)sm.Document_Detail.Cache.Update(doc);
				}
				throw new PXRedirectRequiredException(sm, "create Schedule");
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> reclassifyBatch;
		[PXUIField(DisplayName = AP.Messages.ReclassifyGLBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReclassifyBatch(PXAdapter adapter)
		{
			var document = Document.Current;

			if (document != null)
			{
				ReclassifyTransactionsProcess.TryOpenForReclassificationOfDocument(Document.View, BatchModule.AR, document.BatchNbr, document.DocType,
					document.RefNbr);
			}

			return adapter.Get();
		}

		public PXAction<ARInvoice> autoApply;
		[PXUIField(DisplayName = "Auto Apply", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AutoApply(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.DocType == ARDocType.Invoice && Document.Current.Released == false)
			{
				foreach (ARAdjust2 adj in Adjustments_Inv.Select())
				{
					if (adj == null) continue;

					adj.CuryAdjdAmt = 0m;
				}

				decimal? CuryApplAmt = Document.Current.CuryDocBal;

				foreach (ARAdjust2 adj in Adjustments
					.Select()
					.RowCast<ARAdjust2>()
					.Where(adj => adj.CuryDocBal > 0m))
					{
						if (adj.CuryDocBal > CuryApplAmt)
						{
							adj.CuryAdjdAmt = CuryApplAmt;
							Adjustments.Cache.RaiseFieldUpdated<ARAdjust2.curyAdjdAmt>(adj, 0m);
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
							Adjustments.Cache.RaiseFieldUpdated<ARAdjust2.curyAdjdAmt>(adj, 0m);
							if (Adjustments.Cache.GetStatus(adj) == PXEntryStatus.Notchanged)
							{
								Adjustments.Cache.SetStatus(adj, PXEntryStatus.Updated);
							}
						}
					}
				Adjustments.View.RequestRefresh();
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> viewPayment;
		[PXUIField(
			DisplayName = "View Payment", 
			MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select,
			Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewPayment(PXAdapter adapter)
		{
			if (Document.Current != null && Adjustments.Current != null)
			{
				ARPaymentEntry pe = PXGraph.CreateInstance<ARPaymentEntry>();
				pe.Document.Current = pe.Document.Search<ARPayment.refNbr>(Adjustments.Current.AdjgRefNbr, Adjustments.Current.AdjgDocType);

				throw new PXRedirectRequiredException(pe, true, "Payment") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> viewInvoice;
		[PXUIField(
			DisplayName = "View Invoice", 
			MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select,
			Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewInvoice(PXAdapter adapter)
		{
			if (Document.Current != null && Adjustments_1.Current != null)
			{
				ARInvoiceEntry pe = CreateInstance<ARInvoiceEntry>();
				pe.Document.Current = pe.Document.Search<ARInvoice.refNbr>(Adjustments_1.Current.AdjdRefNbr, Adjustments_1.Current.AdjdDocType);

				throw new PXRedirectRequiredException(pe, true, "Invoice") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> viewInvoice2;
		[PXUIField(
			DisplayName = "View Document", 
			MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select,
			Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewInvoice2(PXAdapter adapter)
		{
			if (Document.Current != null && Adjustments_2.Current != null)
			{
				switch(Adjustments_2.Current.AdjType)
				{
					case ARAdjust.adjType.Adjusted:
					{
						ARInvoiceEntry pe = CreateInstance<ARInvoiceEntry>();
						pe.Document.Current = pe.Document.Search<ARInvoice.refNbr>(Adjustments_2.Current.AdjdRefNbr, Adjustments_2.Current.AdjdDocType);

						throw new PXRedirectRequiredException(pe, true, "Invoice") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
					case ARAdjust.adjType.Adjusting:
					{
						ARPaymentEntry pe = CreateInstance<ARPaymentEntry>();
						pe.Document.Current = pe.Document.Search<ARPayment.refNbr>(Adjustments_2.Current.AdjgRefNbr, Adjustments_2.Current.AdjgDocType);

						throw new PXRedirectRequiredException(pe, true, "Payment") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> viewItem;
		[PXUIField(DisplayName = "View Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewItem(PXAdapter adapter)
		{
			if (Transactions.Current != null)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID,
					Equal<Current<ARTran.inventoryID>>>>.SelectSingleBound(this, null);
				if (item != null)
					PXRedirectHelper.TryRedirect(this.Caches[typeof(InventoryItem)], item, "View Item", PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}


		public PXAction<ARInvoice> validateAddresses;
		[PXUIField(DisplayName = CS.Messages.ValidateAddress, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (ARInvoice current in adapter.Get<ARInvoice>())
			{
				if (current != null)
				{
					bool needSave = false;
					Save.Press();
					ARAddress address = this.Billing_Address.Select();
					if (address != null && address.IsDefaultAddress == false && address.IsValidated == false)
					{
						if (PXAddressValidator.Validate<ARAddress>(this, address, true))
							needSave = true;
					}

					if (needSave == true)
						this.Save.Press();
				}
				yield return current;
			}
		}

		public PXAction<ARInvoice> recalculateDiscountsAction;
		[PXUIField(DisplayName = "Recalculate Prices", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXButton]
		public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
		{
			if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
			{
				DiscountEngine<ARTran>.RecalculatePricesAndDiscounts<ARInvoiceDiscountDetail>(Transactions.Cache, Transactions, Transactions.Current, ARDiscountDetails, Document.Current.CustomerLocationID, Document.Current.DocDate, recalcdiscountsfilter.Current);
			}
			return adapter.Get();
		}

		public PXAction<ARInvoice> recalcOk;
		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable RecalcOk(PXAdapter adapter)
		{
			return adapter.Get();
		}
#endregion
		#region Override PXGraph.GetStateExt
		private object disableJoined(object val)
		{
			PXFieldState stat = val as PXFieldState;
			if (stat != null)
			{
				stat.Enabled = false;
			}
			return val;
		}
		public override object GetStateExt(string viewName, object data, string fieldName)
		{
			if (viewName == "Adjustments")
			{
				if (data == null)
				{
					int pos = fieldName.IndexOf("__");
					if (pos > 0 && pos < fieldName.Length - 2)
					{
						string s = fieldName.Substring(0, pos);
						PXCache cache = null;
						foreach (Type t in Views[viewName].GetItemTypes())
						{
							if (t.Name == s)
							{
								cache = Caches[t];
							}
						}
						if (cache == null)
						{
							cache = Caches[s];
						}
						if (cache != null)
						{
							return disableJoined(cache.GetStateExt(null, fieldName.Substring(pos + 2)));
						}
						return null;
					}
					else
					{
						return Caches[GetItemType(viewName)].GetStateExt(null, fieldName);
					}
				}
				else
				{
					return base.GetStateExt(viewName, data, fieldName);
				}
			}
			else
			{
				return base.GetStateExt(viewName, data, fieldName);
			}
		}
		#endregion
		#region Selects

		public PXSelect<Standalone.ARRegister> dummy_register;
		public PXSelect<InventoryItem> dummy_nonstockitem_for_redirect_newitem;
		public PXSelect<AP.Vendor> dummy_vendor_taxAgency_for_avalara;

		[PXViewName(Messages.ARInvoice)]
		[PXCopyPasteHiddenFields(typeof(ARInvoice.invoiceNbr), FieldsToShowInSimpleImport = new[] { typeof(ARInvoice.invoiceNbr) })]
		public PXSelectJoin<ARInvoice,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<ARInvoice.customerID>>>,
			Where<ARInvoice.docType, Equal<Optional<ARInvoice.docType>>,
			And2<Where<ARInvoice.origModule, Equal<BatchModule.moduleAR>, Or<ARInvoice.released, Equal<True>>>,
			And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>>> Document;
		public PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<Current<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Current<ARInvoice.refNbr>>>>> CurrentDocument;

		public PXSelect<RUTROT.RUTROT, Where<True, Equal<False>>> Rutrots;

		[PXViewName(Messages.ARTran)]
		[PXImport(typeof(ARInvoice))]
		public PXSelectJoin<ARTran, LeftJoin<SOLine,
				On<SOLine.orderType, Equal<ARTran.sOOrderType>,
				And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
				And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>>,
			Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>>>,
			OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>
			Transactions;

		public PXSelect<ARTran, Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>, And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>, And<ARTran.lineType, Equal<SOLineType.discount>>>>, OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>> Discount_Row;

		[PXCopyPasteHiddenView]
		public PXSelect<ARTax, Where<ARTax.tranType, Equal<Current<ARInvoice.docType>>, And<ARTax.refNbr, Equal<Current<ARInvoice.refNbr>>>>, OrderBy<Asc<ARTax.tranType, Asc<ARTax.refNbr, Asc<ARTax.taxID>>>>> Tax_Rows;
		[PXCopyPasteHiddenView]
		public TaxTranSelect<ARInvoice, ARInvoice.termsID, ARTaxTran, ARTaxTran.taxID, 
			Where<ARTaxTran.module, Equal<BatchModule.moduleAR>, 
				And<ARTaxTran.tranType, Equal<Current<ARInvoice.docType>>, 
				And<ARTaxTran.refNbr, Equal<Current<ARInvoice.refNbr>>>>>> Taxes;

		/// <summary>
		/// Applications for the current document, except 
		/// when it is a credit memo.
		/// </summary>
		[PXCopyPasteHiddenView]
		public PXSelectJoin<ARAdjust2, 
			InnerJoin<ARPayment, On<ARPayment.docType, Equal<ARAdjust2.adjgDocType>, 
				And<ARPayment.refNbr, Equal<ARAdjust2.adjgRefNbr>>>>> Adjustments;

		/// <summary>
		/// Applications for the current document,
		/// when it is an unreleased credit memo.
		/// </summary>
		[PXCopyPasteHiddenView]
		public PXSelectJoin<ARAdjust,
			InnerJoin<ARInvoice, On<ARInvoice.docType, Equal<ARAdjust.adjgDocType>, 
				And<ARInvoice.refNbr, Equal<ARAdjust.adjgRefNbr>>>>> Adjustments_1;

		/// <summary>
		/// Applications for the current document,
		/// when it is a released credit memo.
		/// </summary>
		[PXCopyPasteHiddenView]
		public PXSelectJoin<ARAdjust,
			InnerJoin<ARInvoice, On<ARInvoice.docType, Equal<ARAdjust.adjgDocType>,
				And<ARInvoice.refNbr, Equal<ARAdjust.adjgRefNbr>>>>> Adjustments_2;

		public PXSelectJoin<
			ARAdjust2,
				InnerJoinSingleTable<ARPayment, 
					On<ARPayment.docType, Equal<ARAdjust2.adjgDocType>, 
				And<ARPayment.refNbr, Equal<ARAdjust2.adjgRefNbr>>>,
				InnerJoin<Standalone.ARRegisterAlias,
					On<Standalone.ARRegisterAlias.docType, Equal<ARAdjust2.adjgDocType>,
					And<Standalone.ARRegisterAlias.refNbr, Equal<ARAdjust2.adjgRefNbr>>>,
				InnerJoin<CurrencyInfo, 
					On<CurrencyInfo.curyInfoID, Equal<Standalone.ARRegisterAlias.curyInfoID>>>>>,
			Where<
				ARAdjust2.adjdDocType, Equal<Current<ARInvoice.docType>>,
								And<ARAdjust2.adjdRefNbr, Equal<Current<ARInvoice.refNbr>>,
				And<Where<
					Current<ARInvoice.released>, Equal<True>,
					Or<ARAdjust2.released, Equal<Current<ARInvoice.released>>>>>>>> 
			Adjustments_Inv;

		public PXSelectJoin<
			ARAdjust,
				InnerJoinSingleTable<ARInvoice, 
					On<ARInvoice.docType, Equal<ARAdjust.adjdDocType>, 
				And<ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>,
				InnerJoin<Standalone.ARRegisterAlias,
					On<Standalone.ARRegisterAlias.docType, Equal<ARAdjust.adjdDocType>,
					And<Standalone.ARRegisterAlias.refNbr, Equal<ARAdjust.adjdRefNbr>>>,
				InnerJoin<CurrencyInfo, 
					On<CurrencyInfo.curyInfoID, Equal<Standalone.ARRegisterAlias.curyInfoID>>>>>,
			Where<
				ARAdjust.adjgDocType, Equal<Current<ARInvoice.docType>>,
								And<ARAdjust.adjgRefNbr, Equal<Current<ARInvoice.refNbr>>,
				And<Where<
					Current<ARInvoice.released>, Equal<True>, 
					Or<ARAdjust.released, Equal<Current<ARInvoice.released>>>>>>>> 
			Adjustments_Crm;

		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>> CurrencyInfo_CuryInfoID;
		public PXSelect<ARInvoice, Where<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>, And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>> ARInvoice_CustomerID_DocType_RefNbr;

		public PXSelect<PM.PMTran, Where<PM.PMTran.tranID, Equal<Required<ARTran.pMTranID>>>> RefPMTran;
		public PXSelect<PM.PMTran, Where<PM.PMTran.aRTranType, Equal<Required<ARTran.tranType>>, And<PM.PMTran.aRRefNbr, Equal<Required<ARTran.refNbr>>, And<PM.PMTran.refLineNbr, Equal<Required<ARTran.lineNbr>>>>>> RefContractUsageTran;
		[PXViewName(Messages.ARAddress)]
		public PXSelect<ARAddress, Where<ARAddress.addressID, Equal<Current<ARInvoice.billAddressID>>>> Billing_Address;
		[PXViewName(Messages.ARContact)]
		public PXSelect<ARContact, Where<ARContact.contactID, Equal<Current<ARInvoice.billContactID>>>> Billing_Contact;

		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>> currencyinfo;

		[PXViewName(Messages.Customer)]
		public PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<ARInvoice.customerID>>>> customer;
		public PXSetup<CustomerClass, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>> customerclass;
		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<ARInvoice.taxZoneID>>>> taxzone;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<ARInvoice.customerID>>, And<Location.locationID, Equal<Optional<ARInvoice.customerLocationID>>>>> location;
		public PXSelect<ARBalances> arbalances;
		public PXSetup<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Current<ARInvoice.finPeriodID>>>> finperiod;
		public PXSetup<ARSetup> ARSetup;
		public PXSetup<GLSetup> glsetup;
		public PXSetupOptional<TXAvalaraSetup> avalaraSetup;
		public PXSetupOptional<SOSetup> soSetup;
		[PXCopyPasteHiddenView]
		public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;

		[PXCopyPasteHiddenView()]
		public PXSelectJoinGroupBy<ARDunningLetterDetail, 
			InnerJoin<Customer, On<Customer.bAccountID, Equal<Current<ARInvoice.customerID>>>, 
			LeftJoin<ARDunningLetter, On<ARDunningLetter.dunningLetterID, Equal<ARDunningLetterDetail.dunningLetterID>>>>, 
			Where<ARDunningLetterDetail.dunningLetterBAccountID, Equal<Customer.sharedCreditCustomerID>,
				And<ARDunningLetterDetail.refNbr, Equal<Current<ARInvoice.refNbr>>, 
				And<ARDunningLetterDetail.docType, Equal<Current<ARInvoice.docType>>, 
				And<ARDunningLetter.voided, Equal<False>, 
				And<ARDunningLetter.released, Equal<True>, 
				And<ARDunningLetterDetail.dunningLetterLevel, Greater<int0>>>>>>>, 
			Aggregate<GroupBy<ARDunningLetter.voided,
				GroupBy<ARDunningLetter.released,
				GroupBy<ARDunningLetterDetail.refNbr, 
				GroupBy<ARDunningLetterDetail.docType>>>>>> dunningLetterDetail;
		
		[PXCopyPasteHiddenView()]
		public PXSelect<ARInvoiceDiscountDetail, Where<ARInvoiceDiscountDetail.docType, Equal<Current<ARInvoice.docType>>, And<ARInvoiceDiscountDetail.refNbr, Equal<Current<ARInvoice.refNbr>>>>, OrderBy<Asc<ARInvoiceDiscountDetail.docType, Asc<ARInvoiceDiscountDetail.refNbr>>>> ARDiscountDetails;

		public PXSelect<CustSalesPeople, Where<CustSalesPeople.bAccountID, Equal<Current<ARInvoice.customerID>>,
												And<CustSalesPeople.locationID, Equal<Current<ARInvoice.customerLocationID>>>>> salesPerSettings;
		public PXSelectJoin<ARSalesPerTran, LeftJoin<ARSPCommissionPeriod, On<ARSPCommissionPeriod.commnPeriodID, Equal<ARSalesPerTran.commnPaymntPeriod>>>,
												Where<ARSalesPerTran.docType, Equal<Current<ARInvoice.docType>>,
												And<ARSalesPerTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
												And<ARSalesPerTran.adjdDocType, Equal<ARDocType.undefined>,
												And2<Where<Current<ARSetup.sPCommnCalcType>, Equal<SPCommnCalcTypes.byInvoice>, Or<Current<ARInvoice.released>, Equal<boolFalse>>>,
												Or<ARSalesPerTran.adjdDocType, Equal<Current<ARInvoice.docType>>,
												And<ARSalesPerTran.adjdRefNbr, Equal<Current<ARInvoice.refNbr>>,
												And<Current<ARSetup.sPCommnCalcType>, Equal<SPCommnCalcTypes.byPayment>>>>>>>>> salesPerTrans;
		public PXSelect<ARFinChargeTran, Where<ARFinChargeTran.tranType, Equal<Current<ARInvoice.docType>>,
												And<ARFinChargeTran.refNbr, Equal<Current<ARInvoice.refNbr>>>>> finChargeTrans;

		[CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARInvoice.customerID>>>>))]
		[CRDefaultMailTo(typeof(Select<ARContact, Where<ARContact.contactID, Equal<Current<ARInvoice.billContactID>>>>))]
		public CRActivityList<ARInvoice>
			Activity;

		public PXSelect<RUTROT.RUTROTDistribution,
					Where<True, Equal<False>>> RRDistribution;
		

		public PXSelect<DRSchedule> dummySchedule_forPXParent;
		public PXSelect<DRScheduleDetail> dummyScheduleDetail_forPXParent;
		public PXSelect<DRScheduleTran> dummyScheduleTran_forPXParent;

		public PXFilter<DuplicateFilter> duplicatefilter;

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

		public PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>> CurrentBranch;
		public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> InventoryItem;
		

		[PXCopyPasteHiddenView()]
		[PXViewName(CR.Messages.CustomerPaymentMethodDetails)]
		public PXSelect<CustomerPaymentMethod,
					Where<CustomerPaymentMethod.bAccountID, Equal<Current<ARInvoice.customerID>>,
					And<CustomerPaymentMethod.paymentMethodID, Equal<Current<ARInvoice.paymentMethodID>>>>> CustomerPaymentMethodDetails;
		public PXSelect<GLVoucher, Where<True, Equal<False>>> Voucher;

		[PXViewName(Messages.CustomerCredit)]
		public CustomerCreditHelper<ARInvoice, ARInvoice.customerID, ARInvoice.hold, ARInvoice.released> CustomerCreditHelper;

		#endregion

		internal Dictionary<string, HashSet<string>> TaxesByTaxCategory;

		public virtual IEnumerable transactions()
		{
			foreach (PXResult<ARTran, SOLine> tran in PXSelectJoin<ARTran, LeftJoin<SOLine,
				On<SOLine.orderType, Equal<ARTran.sOOrderType>,
				And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
				And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>>,
			Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>>>,
			OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>.Select(this))
			{
				if (((ARTran)tran).LineType != SOLineType.Discount)
					yield return tran;
			}
		}

		public virtual IEnumerable taxes()
		{
			bool hasPPDTaxes = false;
			bool vatReportingInstalled = PXAccess.FeatureInstalled<FeaturesSet.vATReporting>();

			ARTaxTran artaxMax = null;
			decimal? DiscountedTaxableTotal = 0m;
			decimal? DiscountedPriceTotal = 0m;
			
			foreach (PXResult<ARTaxTran, Tax> res in PXSelectJoin<ARTaxTran, 
				InnerJoin<Tax, On<Tax.taxID, Equal<ARTaxTran.taxID>>>, 
				Where<ARTaxTran.module, Equal<BatchModule.moduleAR>, 
					And<ARTaxTran.tranType, Equal<Current<ARInvoice.docType>>, 
					And<ARTaxTran.refNbr, Equal<Current<ARInvoice.refNbr>>>>>>.Select(this))
			{
				if (vatReportingInstalled)
				{
					Tax tax = res;
					ARTaxTran artax = res;
					hasPPDTaxes = tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToPromtPayment || hasPPDTaxes;

					if (hasPPDTaxes &&
						Document.Current != null &&
						Document.Current.CuryOrigDocAmt != null &&
						Document.Current.CuryOrigDocAmt != 0m &&
						Document.Current.CuryOrigDiscAmt != null)
					{
						decimal CashDiscPercent = (decimal) (Document.Current.CuryOrigDiscAmt / Document.Current.CuryOrigDocAmt);
						bool isTaxable = ARPPDCreditMemoProcess.CalculateDiscountedTaxes(Taxes.Cache, artax, CashDiscPercent);
						DiscountedPriceTotal += artax.CuryDiscountedPrice;
						if (isTaxable)
						{
							DiscountedTaxableTotal += artax.CuryDiscountedTaxableAmt;
							if (artaxMax == null || artax.CuryDiscountedTaxableAmt > artaxMax.CuryDiscountedTaxableAmt)
							{
								artaxMax = artax;
							}
						}
					}
				}

				yield return res;
			}

			if (vatReportingInstalled && Document.Current != null)
			{
				Document.Current.HasPPDTaxes = hasPPDTaxes;
				if (hasPPDTaxes)
				{
					decimal? DiscountedDocTotal = DiscountedTaxableTotal + DiscountedPriceTotal;
					Document.Current.CuryDiscountedDocTotal = Document.Current.CuryOrigDocAmt - Document.Current.CuryOrigDiscAmt;

					if (artaxMax != null &&
						Document.Current.CuryVatTaxableTotal + Document.Current.CuryTaxTotal == Document.Current.CuryOrigDocAmt &&
						DiscountedDocTotal != Document.Current.CuryDiscountedDocTotal)
					{
						artaxMax.CuryDiscountedTaxableAmt += Document.Current.CuryDiscountedDocTotal - DiscountedDocTotal;
						DiscountedTaxableTotal = Document.Current.CuryDiscountedDocTotal - DiscountedPriceTotal;
					}

					Document.Current.CuryDiscountedPrice = DiscountedPriceTotal;
					Document.Current.CuryDiscountedTaxableTotal = DiscountedTaxableTotal;
				}
			}
		}

		public string GetReversingDocType(string docType)
		{
			switch (docType)
			{
				case ARDocType.Invoice:
				case ARDocType.DebitMemo:
					docType = ARDocType.CreditMemo;
					break;
				case ARDocType.CreditMemo:
					docType = ARDocType.DebitMemo;
					break;
			}

			return docType;
		}

		public virtual void ReverseInvoiceProc(ARRegister doc)
		{
			DuplicateFilter filter = PXCache<DuplicateFilter>.CreateCopy(duplicatefilter.Current);
			WebDialogResult dialogRes = duplicatefilter.View.Answer;

			this.Clear(PXClearOption.PreserveTimeStamp);

			foreach (PXResult<ARInvoice, CurrencyInfo, Terms, Customer> res in ARInvoice_CurrencyInfo_Terms_Customer.Select(this, (object)doc.DocType, doc.RefNbr, doc.CustomerID))
			{
				CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
				info.CuryInfoID = null;
				info.IsReadOnly = false;
				info = PXCache<CurrencyInfo>.CreateCopy(this.currencyinfo.Insert(info));

				ARInvoice invoice = PXCache<ARInvoice>.CreateCopy((ARInvoice)res);
				invoice.CuryInfoID = info.CuryInfoID;
				invoice.DocType = GetReversingDocType(invoice.DocType);

				invoice.OrigModule = GL.BatchModule.AR;
				invoice.RefNbr = null;
				invoice.OrigModule = GL.BatchModule.AR;

				//must set for _RowSelected
				invoice.OpenDoc = true;
				invoice.Released = false;
				Document.Cache.SetDefaultExt<ARInvoice.hold>(invoice);
				invoice.Printed = false;
				invoice.Emailed = false;
				invoice.BatchNbr = null;
				invoice.ScheduleID = null;
				invoice.Scheduled = false;
				invoice.NoteID = null;
				invoice.RefNoteID = null;

				invoice.TermsID = null;
				invoice.InstallmentCntr = null;
				invoice.InstallmentNbr = null;
				invoice.DueDate = null;
				invoice.DiscDate = null;
				invoice.OrigDocType = doc.DocType;
				invoice.OrigRefNbr = doc.RefNbr;
				invoice.CuryOrigDiscAmt = 0m;
				invoice.FinPeriodID = doc.FinPeriodID;
				invoice.CuryDocBal = invoice.CuryOrigDocAmt;
				invoice.OrigDocDate = invoice.DocDate;
				invoice.CuryLineTotal = 0m;
				invoice.IsTaxPosted = false;
				invoice.IsTaxValid = false;
				invoice.CuryVatTaxableTotal = 0m;
				invoice.CuryVatExemptTotal = 0m;
				invoice.StatementDate = null;
				invoice.PendingPPD = false;

				if (String.IsNullOrEmpty(invoice.PaymentMethodID) == false)
				{
					CA.PaymentMethod pm = null;
					if (invoice.CashAccountID.HasValue)
					{
						CA.PaymentMethodAccount pmAccount = null;
						PXResult<CA.PaymentMethod, CA.PaymentMethodAccount> pmResult = (PXResult<CA.PaymentMethod, CA.PaymentMethodAccount>)PXSelectJoin<CA.PaymentMethod,
														LeftJoin<CA.PaymentMethodAccount,
															On<CA.PaymentMethod.paymentMethodID, Equal<CA.PaymentMethodAccount.paymentMethodID>>>,
														Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>,
															And<CA.PaymentMethodAccount.cashAccountID, Equal<Required<CA.PaymentMethodAccount.cashAccountID>>>>>.Select(this, invoice.PaymentMethodID, invoice.CashAccountID);
						pm = pmResult;
						pmAccount = pmResult;
						if (pm == null || pm.UseForAR == false || pm.IsActive == false)
						{
							invoice.PaymentMethodID = null;
							invoice.CashAccountID = null;
						}
						else if (pmAccount == null || pmAccount.CashAccountID == null || pmAccount.UseForAR != true)
						{
							invoice.CashAccountID = null;
						}
					}
					else
					{
						pm = PXSelect<CA.PaymentMethod,
								Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>>>.Select(this, invoice.PaymentMethodID);
						if (pm == null || pm.UseForAR == false || pm.IsActive == false)
						{
							invoice.PaymentMethodID = null;
							invoice.CashAccountID = null;
							invoice.PMInstanceID = null;
						}
					}
					if (invoice.PMInstanceID.HasValue)
					{
						CustomerPaymentMethod cpm = PXSelect<CustomerPaymentMethod,
												Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CustomerPaymentMethod.pMInstanceID>>>>.Select(this, invoice.PMInstanceID);
						if (string.IsNullOrEmpty(invoice.PaymentMethodID) || cpm == null || cpm.IsActive == false || cpm.PaymentMethodID != invoice.PaymentMethodID)
						{
							invoice.PMInstanceID = null;
						}
					}
				}
				else
				{
					invoice.CashAccountID = null;
					invoice.PMInstanceID = null;
				}
				isReverse = true;
				SalesPerson sp = (SalesPerson)PXSelectorAttribute.Select<ARInvoice.salesPersonID>(this.Document.Cache, invoice);
				if (sp == null || sp.IsActive == false)
					invoice.SalesPersonID = null;
				invoice = this.Document.Insert(invoice);
				isReverse = false;
				if (invoice.RefNbr == null)
				{
					//manual numbering, check for occasional duplicate
					ARInvoice duplicate = PXSelect<ARInvoice>.Search<ARInvoice.docType, ARInvoice.refNbr>(this, invoice.DocType, invoice.OrigRefNbr);
					if (duplicate != null)
					{
						PXCache<DuplicateFilter>.RestoreCopy(duplicatefilter.Current, filter);
						duplicatefilter.View.Answer = dialogRes;
						if (duplicatefilter.AskExt() == WebDialogResult.OK)
						{
							duplicatefilter.Cache.Clear();
							if (duplicatefilter.Current.RefNbr == null)
								throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(DuplicateFilter.refNbr).Name);
							duplicate = PXSelect<ARInvoice>.Search<ARInvoice.docType, ARInvoice.refNbr>(this, invoice.DocType, duplicatefilter.Current.RefNbr);
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
					CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this, null);
					b_info.CuryID = info.CuryID;
					b_info.CuryEffDate = info.CuryEffDate;
					b_info.CuryRateTypeID = info.CuryRateTypeID;
					b_info.CuryRate = info.CuryRate;
					b_info.RecipRate = info.RecipRate;
					b_info.CuryMultDiv = info.CuryMultDiv;
					this.currencyinfo.Update(b_info);
				}
			}

			TaxAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(this.Transactions.Cache, null, TaxCalc.ManualCalc);

			this.FieldDefaulting.AddHandler<ARTran.salesPersonID>((sender, e) => { e.NewValue = null; e.Cancel = true; });

			foreach (ARTran srcTran in PXSelect<ARTran, Where<ARTran.tranType, Equal<Required<ARTran.tranType>>, And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				ARTran tran = PXCache<ARTran>.CreateCopy(srcTran);
				tran.TranType = null;
				tran.RefNbr = null;
				string origDrCr = tran.DrCr;
				tran.DrCr = null;
				tran.Released = null;
				tran.CuryInfoID = null;
				tran.SOOrderNbr = null;
				tran.SOShipmentNbr = null;
				tran.OrigInvoiceDate = tran.TranDate;
				tran.NoteID = null;

				if (!string.IsNullOrEmpty(tran.DeferredCode))
				{
					DRSchedule schedule = PXSelect<DRSchedule,
						Where<DRSchedule.module, Equal<moduleAR>,
						And<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
						And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>,
						And<DRSchedule.lineNbr, Equal<Required<DRSchedule.lineNbr>>>>>>>.Select(this, doc.DocType, doc.RefNbr, tran.LineNbr);

					if (schedule != null)
					{
						tran.DefScheduleID = schedule.ScheduleID;
					}
				}

				SalesPerson sp = (SalesPerson)PXSelectorAttribute.Select<ARTran.salesPersonID>(this.Transactions.Cache, tran);
				if (sp == null || sp.IsActive == false)
					tran.SalesPersonID = null;

				isReverse = true; //added to prevent ARTran_TaxCategoryID_FieldDefaulting
				ARTran insertedTran = this.Transactions.Insert(tran);
				PXNoteAttribute.CopyNoteAndFiles(Transactions.Cache, srcTran, Transactions.Cache, insertedTran);
				isReverse = false;

				insertedTran.ManualDisc = true;

				if (insertedTran.LineType == "DS")
				{
					insertedTran.DrCr = (origDrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
					insertedTran.FreezeManualDisc = true;
					insertedTran.TaxCategoryID = null;
					this.Transactions.Update(insertedTran);
				}
			}

			this.RowInserting.AddHandler<ARSalesPerTran>((sender, e) => { e.Cancel = true; });

			foreach (ARSalesPerTran salespertran in PXSelect<ARSalesPerTran, Where<ARSalesPerTran.docType, Equal<Required<ARSalesPerTran.docType>>, And<ARSalesPerTran.refNbr, Equal<Required<ARSalesPerTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				ARSalesPerTran newtran = PXCache<ARSalesPerTran>.CreateCopy(salespertran);

				newtran.DocType = Document.Current.DocType;
				newtran.RefNbr = Document.Current.RefNbr;
				newtran.Released = false;
				newtran.CuryInfoID = null;
				newtran.CuryCommnblAmt *= -1m;
				newtran.CuryCommnAmt *= -1m;

				SalesPerson sp = (SalesPerson)PXSelectorAttribute.Select<ARSalesPerTran.salespersonID>(this.salesPerTrans.Cache, newtran);
				if (!(sp == null || sp.IsActive == false))
				{
					this.salesPerTrans.Update(newtran);
				}
			}

			foreach (ARInvoiceDiscountDetail discountDetail in PXSelect<ARInvoiceDiscountDetail, Where<ARInvoiceDiscountDetail.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoiceDiscountDetail.refNbr, Equal<Required<ARInvoice.refNbr>>>>, OrderBy<Asc<ARInvoiceDiscountDetail.docType, Asc<ARInvoiceDiscountDetail.refNbr>>>>.Select(this, doc.DocType, doc.RefNbr))
			{
				ARInvoiceDiscountDetail newDiscountDetail = PXCache<ARInvoiceDiscountDetail>.CreateCopy(discountDetail);

				newDiscountDetail.DocType = Document.Current.DocType;
				newDiscountDetail.RefNbr = Document.Current.RefNbr;
				newDiscountDetail.IsManual = true;
				this.ARDiscountDetails.Update(newDiscountDetail);
			}

			if (IsExternalTax != true)
			{
				bool disableTaxCalculation = doc.PendingPPD == true && doc.DocType == ARDocType.CreditMemo;
				foreach (ARTaxTran tax in PXSelect<ARTaxTran, Where<ARTaxTran.tranType, Equal<Required<ARTaxTran.tranType>>, And<ARTaxTran.refNbr, Equal<Required<ARTaxTran.refNbr>>>>>.Select(this, doc.DocType, doc.RefNbr))
				{
					ARTaxTran new_artax = new ARTaxTran();
					new_artax.TaxID = tax.TaxID;

					if (disableTaxCalculation)
					{
						TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(Transactions.Cache, null, TaxCalc.NoCalc);
					}
					new_artax = this.Taxes.Insert(new_artax);

					if (new_artax != null)
					{
						new_artax = PXCache<ARTaxTran>.CreateCopy(new_artax);
						new_artax.TaxRate = tax.TaxRate;
						new_artax.CuryTaxableAmt = tax.CuryTaxableAmt;
						new_artax.CuryTaxAmt = tax.CuryTaxAmt;

						if (disableTaxCalculation)
						{
							TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(Transactions.Cache, null, TaxCalc.ManualCalc);
						}
						new_artax = this.Taxes.Update(new_artax);
					}
				}
			}
		}

		protected string salesSubMask;

		public virtual string SalesSubMask
		{
			get
			{
				if (salesSubMask == null)
				{
					salesSubMask = ARSetup.Current.SalesSubMask;
				}

				return salesSubMask;
			}
			set
			{
				salesSubMask = value;
			}
		}

		#region CurrencyInfo events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryID))
				{
					e.NewValue = customer.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryRateTypeID))
				{
					e.NewValue = customer.Current.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((ARInvoice)Document.Cache.Current).DocDate;
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

		protected virtual void CurrencyInfo_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			CurrencyInfo currencyInfo = e.Row as CurrencyInfo;

			if (currencyInfo?.CuryRate == null) return;

			if (Document.Current?.Released != true)
			{
				if (Document.Current?.DocType != ARDocType.CreditMemo)
				{
					Adjustments.Select().ForEach(adjustment => RecalculateApplicationAmounts(adjustment));
				}
				else
				{
					Adjustments_1.Select().ForEach(adjustment => CalcBalances(adjustment, true, false));
				}
			}
		}
		#endregion

		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<ARInvoice.docDate, ARInvoice.finPeriodID, ARInvoice.curyID>(e.Row, e.OldRow))
			{
				foreach (ARTran tran in Transactions.Select())
				{
					if (Transactions.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Transactions.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}

				foreach (ARAdjust2 tran in Adjustments.Select())
				{
					if (Adjustments.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Adjustments.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}

				foreach (ARAdjust tran in Adjustments_1.Select())
				{
					if (Adjustments_1.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						Adjustments_1.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}

				foreach (ARSalesPerTran tran in salesPerTrans.Select())
				{
					if (this.salesPerTrans.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						this.salesPerTrans.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}

				foreach (ARFinChargeTran tran in this.finChargeTrans.Select())
				{
					if (this.finChargeTrans.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						this.finChargeTrans.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}
			}

			if (!sender.ObjectsEqual<ARInvoice.branchID>(e.Row, e.OldRow))
			{
				foreach (ARSalesPerTran tran in salesPerTrans.Select())
				{
					if (this.salesPerTrans.Cache.GetStatus(tran) == PXEntryStatus.Notchanged)
					{
						this.salesPerTrans.Cache.SetStatus(tran, PXEntryStatus.Updated);
					}
				}
			}
		}

		public bool IsProcessingMode { get; set; }

		public ARInvoiceEntry()
		{
			ARSetup setup = ARSetup.Current;
			RowUpdated.AddHandler<ARInvoice>(ParentFieldUpdated);

			TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(Transactions.Cache, null, TaxCalc.ManualLineCalc);

			FieldDefaulting.AddHandler<InventoryItem.stkItem>((sender, e) => { if (e.Row != null) e.NewValue = false; });

			if (setup != null && setup.DunningLetterProcessType == 1)
			{
				PXUIFieldAttribute.SetVisible<ARInvoice.revoked>(Document.Cache, null, true);
			}

			PXUIFieldAttribute.SetVisible<ARAdjust.customerID>(Adjustments.Cache, null, PXAccess.FeatureInstalled<FeaturesSet.parentChildAccount>());
			PXUIFieldAttribute.SetVisible<ARAdjust.adjdCustomerID>(Adjustments_1.Cache, null, PXAccess.FeatureInstalled<FeaturesSet.parentChildAccount>());

			OpenPeriodAttribute.SetValidatePeriod<ARRegister.finPeriodID>(Document.Cache, null, PeriodValidation.DefaultSelectUpdate);
			TaxesByTaxCategory = new Dictionary<string, HashSet<string>>();
		}

		public override void Persist()
		{
			if (Document.Current != null && !Discount_Row.Any() && Document.Current.CuryDiscTot != 0m)
			{
				AddDiscount(Document.Cache, Document.Current);
				DiscountEngine<ARTran>.CalculateDocumentDiscountRate<ARInvoiceDiscountDetail>(Transactions.Cache, Transactions, null, ARDiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m);
			}
			
			foreach(ARAdjust2 adj in Adjustments.Cache.Inserted
				.Cast<ARAdjust2>()
				.Where(adj => adj.CuryAdjdAmt == 0m))
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.InsertedDeleted);
				}

			foreach(ARAdjust2 adj in Adjustments.Cache.Updated
				.Cast<ARAdjust2>()
				.Where(adj => adj.CuryAdjdAmt == 0m))
				{
					Adjustments.Cache.SetStatus(adj, PXEntryStatus.Deleted);
				}

			foreach (ARInvoice ardoc in Document.Cache.Cached)
			{
				PXEntryStatus status = Document.Cache.GetStatus(ardoc);

				if (status == PXEntryStatus.Deleted && ardoc.PendingPPD == true && ardoc.DocType == ARDocType.CreditMemo)
				{
					PXUpdate<Set<ARAdjust.pPDCrMemoRefNbr, Null>, ARAdjust,
						Where<ARAdjust. pendingPPD, Equal<True>,
							And<ARAdjust.pPDCrMemoRefNbr, Equal<Required<ARAdjust.pPDCrMemoRefNbr>>>>>
						.Update(this, ardoc.RefNbr);
				}

				if ((status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated) && ardoc.DocType == ARDocType.Invoice && ardoc.Released == false && ardoc.ApplyPaymentWhenTaxAvailable != true)
				{
					decimal? CuryApplAmt = 0m;

					foreach (ARAdjust2 adj in Adjustments_Inv.View
						.SelectMultiBound(new object[] { ardoc })
						.RowCast<ARAdjust2>()
						.WhereNotNull())
					{
						CuryApplAmt += adj.CuryAdjdAmt;

						if (ardoc.CuryDocBal - CuryApplAmt < 0m && CuryApplAmt > 0m)
						{
							Adjustments.Cache.SmartSetStatus(adj, PXEntryStatus.Updated);
							Adjustments.Cache.RaiseExceptionHandling<ARAdjust2.curyAdjdAmt>(adj, adj.CuryAdjdAmt, new PXSetPropertyException(Messages.Application_Amount_Cannot_Exceed_Document_Amount));
							throw new PXException(Messages.Application_Amount_Cannot_Exceed_Document_Amount);
						}
					}
				}

				if ((status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated) && ardoc.DocType == ARDocType.CreditMemo && ardoc.Released == false)
				{
					decimal? CuryApplAmt = 0m;

					foreach (ARAdjust adj in Adjustments_Crm.View
						.SelectMultiBound(new object[] { ardoc })
						.RowCast<ARAdjust>()
						.WhereNotNull())
					{
						CuryApplAmt += adj.CuryAdjgAmt;

						if (ardoc.CuryDocBal - CuryApplAmt < 0m && CuryApplAmt > 0m)
						{
							Adjustments_1.Cache.SmartSetStatus(adj, PXEntryStatus.Updated);
							Adjustments_1.Cache.RaiseExceptionHandling<ARAdjust.curyAdjgAmt>(adj, adj.CuryAdjgAmt, new PXSetPropertyException(Messages.Application_Amount_Cannot_Exceed_Document_Amount));
							throw new PXException(Messages.Application_Amount_Cannot_Exceed_Document_Amount);
						}
					}
				}
			}

			DiscountEngine<ARTran>.ValidateDiscountDetails(ARDiscountDetails);

			base.Persist();

			if (Document.Current != null && IsExternalTax == true && Document.Current.InstallmentNbr == null && Document.Current.IsTaxValid != true && !skipAvalaraCallOnSave && Document.Current.Released != true)
			{
				if (PXLongOperation.GetCurrentItem() == null)
				{
					PXLongOperation.StartOperation(this, delegate
					{
						ARInvoice doc = new ARInvoice
					{
							DocType = Document.Current.DocType,
							RefNbr = Document.Current.RefNbr,
							OrigModule = Document.Current.OrigModule,
							ApplyPaymentWhenTaxAvailable = Document.Current.ApplyPaymentWhenTaxAvailable
						};
						ARExternalTaxCalc.Process(doc);

						RecalcUnbilledTax();
					});
				}
				else
				{

					ARExternalTaxCalc.Process(this);

					RecalcUnbilledTax();
				}
			}
		}

		public virtual void RecalcUnbilledTax()
		{

		}

		public PXAction<ARInvoice> notification;
		[PXUIField(DisplayName = "Notifications", Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Notification(PXAdapter adapter,
		[PXString]
		string notificationCD)
		{
			foreach (ARInvoice doc in adapter.Get())
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>
				{
					["DocType"] = doc.DocType,
					["RefNbr"] = doc.RefNbr
				};

				using (var ts = new PXTransactionScope())
				{
				Activity.SendNotification(ARNotificationSource.Customer, notificationCD, doc.BranchID, parameters);
					this.Save.Press();
					
					ts.Complete();
				}
								
				yield return doc;
			}
		}
		public override string GetCustomerReportID(string reportID, ARInvoice doc)
		{
			Document.Current = doc;
			return new NotificationUtility(this).SearchReport(ARNotificationSource.Customer, customer.SelectSingle(), reportID, doc.BranchID);
		}

		public virtual IEnumerable adjustments()
		{
			Adjustments.Cache.ClearQueryCache();

			CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this);

			foreach (PXResult<ARAdjust2, ARPayment, Standalone.ARRegisterAlias, CurrencyInfo> res in Adjustments_Inv.Select())
			{
				ARPayment payment = res;
				ARAdjust2 adj = res;
				CurrencyInfo pay_info = res;

				PXCache<ARRegister>.RestoreCopy(payment, (Standalone.ARRegisterAlias)res);

				if (adj == null) continue;

				BalanceCalculation.CalculateApplicationDocumentBalance(
					Adjustments.Cache, 
					payment, 
					adj, 
					pay_info, 
					inv_info);

                yield return new PXResult<ARAdjust2, ARPayment>(adj, payment);
			}

			if (Document.Current != null
				&& (Document.Current.DocType == ARDocType.Invoice
					|| Document.Current.DocType == ARDocType.DebitMemo)
				&& Document.Current.Released != true
				&& Document.Current.Scheduled != true
				&& Document.Current.Voided != true)
			{
				using (new ReadOnlyScope(Adjustments.Cache, Document.Cache, arbalances.Cache))
                {
					foreach (PXResult<Standalone.ARRegisterAlias, CurrencyInfo, ARAdjust2, SOAdjust, ARPayment> res in PXSelectReadonly2<
						Standalone.ARRegisterAlias,
							InnerJoin<CurrencyInfo, 
								On<CurrencyInfo.curyInfoID, Equal<Standalone.ARRegisterAlias.curyInfoID>>,
							LeftJoin<ARAdjust2,
								On<ARAdjust2.adjgDocType, Equal<Standalone.ARRegisterAlias.docType>,
								And<ARAdjust2.adjgRefNbr, Equal<Standalone.ARRegisterAlias.refNbr>,
								And<ARAdjust2.adjNbr, Equal<Standalone.ARRegisterAlias.adjCntr>,
								And<ARAdjust2.released, Equal<False>,
								And<ARAdjust2.voided, Equal<False>,
								And<Where<
									ARAdjust2.adjdDocType, NotEqual<Current<ARInvoice.docType>>,
									Or<ARAdjust2.adjdRefNbr, NotEqual<Current<ARInvoice.refNbr>>>>>>>>>>,
							LeftJoin<SOAdjust,
								On<SOAdjust.adjgDocType, Equal<Standalone.ARRegisterAlias.docType>,
								And<SOAdjust.adjgRefNbr, Equal<Standalone.ARRegisterAlias.refNbr>,
								And<SOAdjust.adjAmt, Greater<decimal0>>>>,
							InnerJoinSingleTable<ARPayment,
								On<ARPayment.docType, Equal<Standalone.ARRegisterAlias.docType>,
								And<ARPayment.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>>>>>,
						Where2<
							Where<
								Standalone.ARRegisterAlias.customerID, Equal<Current<ARInvoice.customerID>>,
								Or<Standalone.ARRegisterAlias.customerID, Equal<Current<Customer.consolidatingBAccountID>>>>,
							And2<Where<
								Standalone.ARRegisterAlias.docType, Equal<ARDocType.payment>,
								Or<Standalone.ARRegisterAlias.docType, Equal<ARDocType.prepayment>,
								Or<Standalone.ARRegisterAlias.docType, Equal<ARDocType.creditMemo>>>>,
							And<Standalone.ARRegisterAlias.docDate, LessEqual<Current<ARInvoice.docDate>>,
							And<Standalone.ARRegisterAlias.finPeriodID, LessEqual<Current<ARInvoice.finPeriodID>>,
							And<Standalone.ARRegisterAlias.released, Equal<boolTrue>,
							And<Standalone.ARRegisterAlias.openDoc, Equal<boolTrue>,
                            And<Standalone.ARRegisterAlias.hold, Equal<False>,
							And<ARAdjust2.adjdRefNbr, IsNull,
							And<SOAdjust.adjgRefNbr, IsNull>>>>>>>>>>
						.Select(this))
                    {
                        ARPayment payment = res;
                        CurrencyInfo pay_info = res;

						PXCache<ARRegister>.RestoreCopy(payment, (Standalone.ARRegisterAlias)res);

						ARAdjust2 adj = new ARAdjust2
						{
							AdjdDocType = Document.Current.DocType,
							AdjdRefNbr = Document.Current.RefNbr,
							AdjgDocType = payment.DocType,
							AdjgRefNbr = payment.RefNbr,
							AdjNbr = payment.AdjCntr,
							CustomerID = payment.CustomerID,
							AdjdCustomerID = Document.Current.CustomerID,
							AdjdBranchID = Document.Current.BranchID,
							AdjgBranchID = payment.BranchID,
							AdjgCuryInfoID = payment.CuryInfoID,
							AdjdOrigCuryInfoID = Document.Current.CuryInfoID,
							//if LE constraint is removed from payment selection this must be reconsidered
							AdjdCuryInfoID = Document.Current.CuryInfoID
						};

						if (Adjustments.Cache.Locate(adj) == null 
							|| Adjustments.Cache.GetStatus(adj) == PXEntryStatus.InsertedDeleted)
						{
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

		public virtual IEnumerable adjustments_1()
		{
			foreach (PXResult<ARAdjust, ARInvoice, Standalone.ARRegisterAlias, CurrencyInfo> res in Adjustments_Crm.Select())
			{
				ARAdjust adj = res;
				ARInvoice invoice = res;

				PXCache<ARRegister>.RestoreCopy(invoice, (Standalone.ARRegisterAlias)res);

				if (adj != null)
				{
					if (adj.Released == false)
				{
					if (Adjustments_1.Cache.GetStatus((ARAdjust)res) == PXEntryStatus.Notchanged)
					{
							CalcBalances<ARInvoice>(res, invoice, true);
					}
					}
					adj.AdjType = ARAdjust.adjType.Adjusted;
					this.Caches<ARAdjust>().RaiseFieldUpdated<ARAdjust.adjType>(adj, null);
				}

				yield return new PXResult<ARAdjust, ARInvoice>(adj, invoice);
			}

			Adjustments_1.View.RequestRefresh();
		}

		public virtual IEnumerable adjustments_2()
		{
			foreach (object res in adjustments_1())
			{
				yield return res;
			}

			if (Document.Current != null && Document.Current.Released == true)
			{
				CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this);
				
				foreach (PXResult<ARAdjust, Standalone.ARRegisterAlias, ARPayment, CurrencyInfo> res in PXSelectJoin<
					ARAdjust,
						InnerJoin<Standalone.ARRegisterAlias,
							On<Standalone.ARRegisterAlias.docType, Equal<ARAdjust.adjgDocType>,
							And<Standalone.ARRegisterAlias.refNbr, Equal<ARAdjust.adjgRefNbr>>>,
						InnerJoinSingleTable<ARPayment, 
							On<ARPayment.docType, Equal<ARAdjust.adjgDocType>, 
						And<ARPayment.refNbr, Equal<ARAdjust.adjgRefNbr>>>,
						InnerJoin<CurrencyInfo, 
							On<CurrencyInfo.curyInfoID, Equal<Standalone.ARRegisterAlias.curyInfoID>>>>>,
					Where<
						ARAdjust.adjdDocType, Equal<Current<ARInvoice.docType>>,
						And<ARAdjust.adjdRefNbr, Equal<Current<ARInvoice.refNbr>>>>>
					.Select(this))
				{
					ARAdjust adj = res;
					ARPayment payment = res;
					CurrencyInfo pay_info = res;
					
					PXCache<ARRegister>.RestoreCopy(payment, (Standalone.ARRegisterAlias)res);

					adj.AdjType = ARAdjust.adjType.Adjusting;
					this.Caches<ARAdjust>().RaiseFieldUpdated<ARAdjust.adjType>(adj, null);

					BalanceCalculation.CalculateApplicationDocumentBalance(
						Adjustments_2.Cache, 
						payment, 
						adj, 
						pay_info, 
						inv_info);

                    yield return new PXResult<ARAdjust, ARPayment>(adj, payment);
				}
			}

			Adjustments_2.View.RequestRefresh();
		}

		private class PXLoadInvoiceException : Exception
		{
			public PXLoadInvoiceException() { }

			public PXLoadInvoiceException(SerializationInfo info, StreamingContext context)
				: base(info, context) { }
		}

		public virtual void LoadInvoicesProc()
		{
			try
			{
				if (Document.Current?.CustomerID == null || Document.Current.OpenDoc == false || Document.Current.DocType != ARDocType.Invoice)
				{
					throw new PXLoadInvoiceException();
				}

				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged || Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Held)
				{
					Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
				}
				Document.Cache.IsDirty = true;

				decimal? CuryUnappliedBal = Document.Current.CuryDocBal;

				foreach(ARAdjust2 copy in Adjustments.Select().RowCast<ARAdjust2>().Select(PXCache<ARAdjust2>.CreateCopy))
				{
					if (CuryUnappliedBal > copy.CuryDocBal)
					{
						copy.CuryAdjdAmt = copy.CuryDocBal;
						CuryUnappliedBal -= copy.CuryAdjdAmt;
					}
					else
					{
						copy.CuryAdjdAmt = CuryUnappliedBal;
						CuryUnappliedBal = 0m;
					}

					Adjustments.Cache.Update(copy);

					if (CuryUnappliedBal == 0m)
					{
						throw new PXLoadInvoiceException();
					}
				}
			}
			catch (PXLoadInvoiceException)
			{
			}
		}

		protected virtual void ARTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARTran documentLine = e.Row as ARTran;

			if (documentLine == null) return;

			if (documentLine.PMTranID != null && documentLine.TranType != ARDocType.CreditMemo)
			{
				PM.PMTran pmTran = PXSelect<PM.PMTran, Where<PM.PMTran.tranID, Equal<Required<PM.PMTran.tranID>>>>.Select(this,
					documentLine.PMTranID);

				if (documentLine.PMDeltaOption == null)
				{
					if (pmTran.Amount > documentLine.TranAmt)
					{
						bool raiseError = true;

						PX.Objects.PM.PMAccountGroup ag =
							PXSelect
								<PX.Objects.PM.PMAccountGroup,
									Where<PX.Objects.PM.PMAccountGroup.groupID, Equal<Required<PX.Objects.PM.PMAccountGroup.groupID>>>>
								.Select(this, pmTran.AccountGroupID);
						if (ag.Type == GL.AccountType.Liability || ag.Type == GL.AccountType.Income)
						{
							if (-pmTran.Amount <= documentLine.TranAmt)
								raiseError = false;
						}


						if (raiseError)
						{
							if (sender.RaiseExceptionHandling(typeof(ARTran.pMDeltaOption).Name, documentLine, null,
								new PXSetPropertyException(Messages.PMDeltaOptionRequired,
									PXErrorLevel.Error)))
							{
								throw new PXRowPersistingException(typeof(ARTran.pMDeltaOption).Name, null,
									Messages.PMDeltaOptionRequired);
							}
						}
					}
				}
				else if (documentLine.TranAmt == 0 && documentLine.PMDeltaOption != ARTran.pMDeltaOption.CompleteLine)
				{
					if (sender.RaiseExceptionHandling(typeof(ARTran.pMDeltaOption).Name, documentLine, documentLine.PMDeltaOption,
						new PXSetPropertyException(Messages.PMDeltaOptionNotValid, PXErrorLevel.Error)))
					{
						throw new PXRowPersistingException(typeof(ARTran.pMDeltaOption).Name, documentLine.PMDeltaOption,
							Messages.PMDeltaOptionNotValid);
					}
				}
			}

			if (documentLine.DeferredCode != null)
			{
				var code = (DRDeferredCode)PXSelectorAttribute.Select<ARTran.deferredCode>(sender, documentLine);
				if (code != null & code.MultiDeliverableArrangement == true)
				{
					var item = InventoryItemGetByID(documentLine.InventoryID);
					var itemCode = item == null ? null : DeferredCodeGetByID(item.DeferredCode);

					if (itemCode == null || itemCode.MultiDeliverableArrangement != true)
					{
						if (sender.RaiseExceptionHandling<ARTran.deferredCode>(documentLine, code.DeferredCodeID,
							new PXSetPropertyException<ARTran.deferredCode>(DR.Messages.MDANotAllowedForItem)))
						{
							throw new PXRowPersistingException(typeof(ARTran.deferredCode).Name, code.DeferredCodeID, DR.Messages.MDANotAllowedForItem);
						}
					}
				}
			}

			ScheduleHelper.DeleteAssociatedScheduleIfDeferralCodeChanged(this, documentLine);
		}

		#region ARInvoince Events

		protected virtual void ARInvoice_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			ARInvoice row = e.Row as ARInvoice;
			if (row != null)
			{
				Location customerLocation = location.Select();
				if (customerLocation != null)
				{
					if (!string.IsNullOrEmpty(customerLocation.CTaxZoneID))
					{
						e.NewValue = customerLocation.CTaxZoneID;
					}
					else
					{
						BAccount companyAccount = PXSelectJoin<BAccountR, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, row.BranchID);
						if (companyAccount != null)
						{
							Location companyLocation = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this, companyAccount.BAccountID, companyAccount.DefLocationID);
							if (companyLocation != null)
								e.NewValue = companyLocation.VTaxZoneID;
						}
					}
				}
			}
		}

		protected virtual void ARInvoice_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARInvoice.taxZoneID>(e.Row);

			foreach(ARTaxTran taxTran in Taxes.Select())
			{
				if(Taxes.Cache.GetStatus(taxTran) == PXEntryStatus.Notchanged)
				{
					Taxes.Cache.SetStatus(taxTran, PXEntryStatus.Updated);
				}
			}
		}

		private bool IsTaxZoneDerivedFromCustomer()
		{
			Location customerLocation = location.Select();
			if (customerLocation != null)
			{
				if (!string.IsNullOrEmpty(customerLocation.CTaxZoneID))
				{
					return true;
				}
			}

			return false;
		}

		protected virtual void ARAddress_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARAddress row = e.Row as ARAddress;
			ARAddress oldRow = e.OldRow as ARAddress;
			if (row != null)
			{
				if (!IsTaxZoneDerivedFromCustomer() && !string.IsNullOrEmpty(row.PostalCode) && oldRow.PostalCode != row.PostalCode)
				{
					string taxZone = TaxBuilderEngine.GetTaxZoneByZip(this, row.PostalCode);
					Document.Cache.SetValueExt<ARInvoice.taxZoneID>(Document.Current, taxZone);
				}
			}
		}


		protected virtual void ARInvoice_DocType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = ARDocType.Invoice;
		}
		protected virtual void ARInvoice_DontPrint_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.customer.Current != null)
				e.NewValue = !(this.customer.Current.PrintInvoices == true);
		}
		protected virtual void ARInvoice_DontEmail_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.customer.Current != null)
				e.NewValue = !(this.customer.Current.MailInvoices == true);
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

		protected virtual void ARInvoice_ARAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (location.Current != null && e.Row != null)
			{
				e.NewValue = GetAcctSub<CR.Location.aRAccountID>(location.Cache, location.Current);
			}
		}

		protected virtual void ARInvoice_ARSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (location.Current != null && e.Row != null)
			{
				e.NewValue = GetAcctSub<CR.Location.aRSubID>(location.Cache, location.Current);
			}
		}

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(Visibility = PXUIVisibility.Invisible, DisplayName = "Location")]
		protected virtual void ARInvoice_CustomerLocationID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void ARInvoice_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			location.RaiseFieldUpdated(sender, e.Row);

			sender.SetDefaultExt<ARInvoice.aRAccountID>(e.Row);
			sender.SetDefaultExt<ARInvoice.aRSubID>(e.Row);
			sender.SetDefaultExt<ARInvoice.branchID>(e.Row);
			sender.SetDefaultExt<ARInvoice.taxZoneID>(e.Row);
			sender.SetDefaultExt<ARInvoice.avalaraCustomerUsageType>(e.Row);
			sender.SetDefaultExt<ARInvoice.salesPersonID>(e.Row);
			sender.SetDefaultExt<ARInvoice.workgroupID>(e.Row);
			sender.SetDefaultExt<ARInvoice.ownerID>(e.Row);

			if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.AR))
			{
				sender.SetDefaultExt<ARInvoice.projectID>(e.Row);
			}
		}

		protected virtual void ARInvoice_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARInvoice invoice = (ARInvoice)e.Row;
			customer.RaiseFieldUpdated(sender, e.Row);
			if (customer.Current != null)
			{
				invoice.ApplyOverdueCharge = customer.Current.FinChargeApply;

				if (!e.ExternalCall)
				{
					customer.Current.CreditRule = null;
				}
			}

			Adjustments_Inv.Cache.Clear();
			Adjustments_Inv.Cache.ClearQueryCache();

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<ARInvoice.curyID>(e.Row) == null)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<ARInvoice.curyInfoID>(sender, e.Row);

					string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<ARInvoice.docDate>(e.Row, ((ARInvoice)e.Row).DocDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						((ARInvoice)e.Row).CuryID = info.CuryID;
					}
				}
			}

			{
				sender.SetDefaultExt<ARInvoice.customerLocationID>(e.Row);
				sender.SetDefaultExt<ARInvoice.dontPrint>(e.Row);
				sender.SetDefaultExt<ARInvoice.dontEmail>(e.Row);

				if (e.ExternalCall || sender.GetValuePending<ARInvoice.termsID>(e.Row) == null)
				{
					if (((ARInvoice)e.Row).DocType != ARDocType.CreditMemo)
					{
						sender.SetDefaultExt<ARInvoice.termsID>(e.Row);
					}
					else
					{
						sender.SetValueExt<ARInvoice.termsID>(e.Row, null);
					}
				}
			}

			try
			{
				ARAddressAttribute.DefaultRecord<ARInvoice.billAddressID>(sender, e.Row);
				ARContactAttribute.DefaultRecord<ARInvoice.billContactID>(sender, e.Row);
			}
			catch (PXFieldValueProcessingException ex)
			{
				ex.ErrorValue = customer.Current.AcctCD;
				throw;
			}

			sender.SetDefaultExt<ARInvoice.taxZoneID>(e.Row);
			sender.SetDefaultExt<ARInvoice.paymentMethodID>(e.Row);
		}

		protected virtual void ARInvoice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARInvoice doc = (ARInvoice)e.Row;

			bool isDiscountableDoc = doc.DocType != ARDocType.CreditMemo && doc.DocType != ARDocType.SmallCreditWO;

			if (isDiscountableDoc && string.IsNullOrEmpty(doc.TermsID))
			{
				if (sender.RaiseExceptionHandling<ARInvoice.termsID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARInvoice.termsID).Name)))
				{
					throw new PXRowPersistingException(typeof(ARInvoice.termsID).Name, null, ErrorMessages.FieldIsEmpty, typeof(ARInvoice.termsID).Name);
				}
			}

			if (isDiscountableDoc && doc.DueDate == null)
			{
				if (sender.RaiseExceptionHandling<ARInvoice.dueDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARInvoice.dueDate).Name)))
				{
					throw new PXRowPersistingException(typeof(ARInvoice.dueDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(ARInvoice.dueDate).Name);
				}
			}

			if (isDiscountableDoc && doc.DiscDate == null)
			{
				if (sender.RaiseExceptionHandling<ARInvoice.discDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ARInvoice.discDate).Name)))
				{
					throw new PXRowPersistingException(typeof(ARInvoice.discDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(ARInvoice.discDate).Name);
				}
			}

			if (doc.DocType == ARDocType.FinCharge)
			{
				AutoNumberAttribute.SetNumberingId<ARInvoice.refNbr>(sender, doc.DocType, ARSetup.Current.FinChargeNumberingID);
			}

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert) && (doc.DocType == ARDocType.FinCharge))
			{
				if (this.Accessinfo.ScreenID == "AR.30.10.00")
				{
					throw new PXException(PX.Objects.AR.Messages.FinChargeCanNotBeDeleted);
				}
			}

			if (doc.CuryDiscTot > Math.Abs(doc.CuryLineTotal ?? 0m))
			{
				if (sender.RaiseExceptionHandling<ARInvoice.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(Messages.DiscountGreaterLineTotal, PXErrorLevel.Error)))
				{
					throw new PXRowPersistingException(typeof(ARInvoice.curyDiscTot).Name, null, Messages.DiscountGreaterLineTotal);
				}
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

		}

		protected virtual void ARInvoice_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			var row = (ARInvoice)e.Row;
			if (e.TranStatus == PXTranStatus.Open)
				foreach (CROpportunity opportunity in OpportunityBackReference.Select())
				{
					opportunity.ARRefNbr = row.RefNbr;
				}
		}

		protected virtual void ARInvoice_DocDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CurrencyInfoAttribute.SetEffectiveDate<ARInvoice.docDate>(sender, e);
		}

		protected virtual void ARInvoice_TermsID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Terms terms = (Terms)PXSelectorAttribute.Select<ARInvoice.termsID>(sender, e.Row);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single)
			{
				foreach (ARAdjust2 adj in Adjustments.Select())
				{
					Adjustments.Cache.Delete(adj);
				}
			}
		}

		protected virtual void ARInvoice_PaymentMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARInvoice.pMInstanceID>(e.Row);
			sender.SetDefaultExt<ARInvoice.cashAccountID>(e.Row);
		}

		protected virtual void ARInvoice_PMInstanceID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARInvoice.cashAccountID>(e.Row);
		}

		protected virtual void ARInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ARInvoice doc = e.Row as ARInvoice;
			if (doc == null) return;

			PXUIFieldAttribute.SetVisible<ARInvoice.curyID>(cache, doc, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			PXUIFieldAttribute.SetRequired<ARInvoice.termsID>(cache, (doc.DocType != ARDocType.CreditMemo));
			PXUIFieldAttribute.SetRequired<ARInvoice.dueDate>(cache, (doc.DocType != ARDocType.CreditMemo));
			PXUIFieldAttribute.SetRequired<ARInvoice.discDate>(cache, (doc.DocType != ARDocType.CreditMemo));
			PXUIFieldAttribute.SetVisible<ARTran.origInvoiceDate>(Transactions.Cache, null, doc.DocType == ARInvoiceType.CreditMemo);
			bool curyenabled = !(customer.Current != null && customer.Current.AllowOverrideCury != true);

			autoApply.SetEnabled(doc.DocType == ARDocType.Invoice && doc.Released == false);

			var shouldDisable = doc.Released == true 
								|| doc.Voided == true
								|| doc.DocType == ARDocType.SmallCreditWO
								|| doc.PendingPPD == true
								|| doc.DocType == ARDocType.FinCharge && !IsProcessingMode && cache.GetStatus(doc) == PXEntryStatus.Inserted;

			if (shouldDisable)
			{
				bool isUnreleasedWO = doc.Released != true && doc.DocType == ARDocType.SmallCreditWO;
				bool isUnreleasedPPD = doc.Released != true && doc.PendingPPD == true;

				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.dueDate>(cache, doc, (doc.DocType != ARDocType.CreditMemo && doc.DocType != ARDocType.SmallCreditWO && doc.DocType != ARDocType.FinCharge) && doc.OpenDoc == true && doc.PendingPPD != true);
				PXUIFieldAttribute.SetEnabled<ARInvoice.discDate>(cache, doc, (doc.DocType != ARDocType.CreditMemo && doc.DocType != ARDocType.SmallCreditWO && doc.DocType != ARDocType.FinCharge) && doc.OpenDoc == true && doc.PendingPPD != true);
				PXUIFieldAttribute.SetEnabled<ARInvoice.emailed>(cache, doc, true);
				cache.AllowDelete = isUnreleasedWO || isUnreleasedPPD;
				cache.AllowUpdate = true;
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = false;
				Transactions.Cache.AllowInsert = false;

				ARDiscountDetails.Cache.AllowDelete = false;
				ARDiscountDetails.Cache.AllowUpdate = false;
				ARDiscountDetails.Cache.AllowInsert = false;

				Taxes.Cache.AllowUpdate = false;

				release.SetEnabled(isUnreleasedWO || isUnreleasedPPD);
				createSchedule.SetEnabled(false);
				payInvoice.SetEnabled(doc.OpenDoc == true && doc.Payable == true);
				if (isUnreleasedPPD)
				{
					recalculateDiscountsAction.SetEnabled(false);
				}
				
				bool enablePM = doc.DocType == ARDocType.Invoice && (doc.Scheduled == true);
				PXUIFieldAttribute.SetEnabled<ARInvoice.paymentMethodID>(cache, doc, enablePM);
				if (enablePM)
				{
					bool hasPaymentMethod = (String.IsNullOrEmpty(doc.PaymentMethodID) == false);
					bool isPMInstanceRequired = false;
					if (hasPaymentMethod)
					{
						CA.PaymentMethod pm = PXSelect<CA.PaymentMethod, Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
						isPMInstanceRequired = pm.IsAccountNumberRequired == true;
					}
					PXUIFieldAttribute.SetEnabled<ARInvoice.pMInstanceID>(cache, doc, enablePM && hasPaymentMethod && isPMInstanceRequired);
					PXUIFieldAttribute.SetEnabled<ARInvoice.cashAccountID>(cache, e.Row, enablePM && hasPaymentMethod);
				}

			}
			else
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<ARInvoice.status>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyDocBal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyLineTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyTaxTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.batchNbr>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyID>(cache, doc, curyenabled);
				PXUIFieldAttribute.SetEnabled<ARInvoice.hold>(cache, doc, (doc.Scheduled != true));
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyVatExemptTotal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyVatTaxableTotal>(cache, doc, false);


				bool hasPaymentMethod = (String.IsNullOrEmpty(doc.PaymentMethodID) == false);
				bool isPMInstanceRequired = false;
				if (doc.DocType == ARDocType.Invoice && hasPaymentMethod)
				{
					CA.PaymentMethod pm = PXSelect<CA.PaymentMethod, Where<CA.PaymentMethod.paymentMethodID, Equal<Required<CA.PaymentMethod.paymentMethodID>>>>.Select(this, doc.PaymentMethodID);
					isPMInstanceRequired = pm.IsAccountNumberRequired == true;
				}
				PXUIFieldAttribute.SetEnabled<ARInvoice.paymentMethodID>(cache, e.Row, doc.DocType == ARDocType.Invoice);
				PXUIFieldAttribute.SetEnabled<ARInvoice.pMInstanceID>(cache, e.Row, doc.DocType == ARDocType.Invoice && isPMInstanceRequired);
				PXUIFieldAttribute.SetEnabled<ARInvoice.cashAccountID>(cache, e.Row, hasPaymentMethod);


				PXUIFieldAttribute.SetEnabled<ARInvoice.termsID>(cache, doc, (doc.DocType != ARDocType.CreditMemo));
				PXUIFieldAttribute.SetEnabled<ARInvoice.dueDate>(cache, doc, (doc.DocType != ARDocType.CreditMemo));
				PXUIFieldAttribute.SetEnabled<ARInvoice.discDate>(cache, doc, (doc.DocType != ARDocType.CreditMemo));
				PXUIFieldAttribute.SetEnabled<ARInvoice.curyOrigDiscAmt>(cache, doc, (doc.DocType != ARDocType.CreditMemo));

				//PXUIFieldAttribute.SetEnabled<ARTran.deferredCode>(Transactions.Cache, null, (doc.DocType == ARDocType.Invoice || doc.DocType == ARDocType.DebitMemo));
				PXUIFieldAttribute.SetEnabled<ARTran.defScheduleID>(Transactions.Cache, null, (doc.DocType == ARDocType.CreditMemo || doc.DocType == ARDocType.DebitMemo));

				//calculate only on data entry, differences from the applications will be moved to RGOL upon closure
				PXDBCurrencyAttribute.SetBaseCalc<ARInvoice.curyDocBal>(cache, doc, true);
				PXDBCurrencyAttribute.SetBaseCalc<ARInvoice.curyDiscBal>(cache, doc, true);

				cache.AllowDelete = true;
				cache.AllowUpdate = true;
				Transactions.Cache.AllowDelete = true;
				Transactions.Cache.AllowUpdate = true;
				Transactions.Cache.AllowInsert =
					doc.CustomerID != null &&
					doc.CustomerLocationID != null &&
					doc.DocType != ARDocType.FinCharge &&
					(doc.ProjectID != null || !PM.ProjectAttribute.IsPMVisible(this, BatchModule.AR));

				if (Document.Current != null)
				{
					ARDiscountDetails.Cache.AllowDelete = true;
					ARDiscountDetails.Cache.AllowUpdate = true;
					ARDiscountDetails.Cache.AllowInsert = true;
				}
				else
				{
					ARDiscountDetails.Cache.AllowDelete = false;
					ARDiscountDetails.Cache.AllowUpdate = false;
					ARDiscountDetails.Cache.AllowInsert = false;
				}

				Taxes.Cache.AllowUpdate = true;
				release.SetEnabled(doc.Hold != true && doc.Scheduled != true);
				createSchedule.SetEnabled(doc.Hold != true && (doc.DocType == ARDocType.Invoice));
				payInvoice.SetEnabled(false);
			}
			Billing_Address.Cache.AllowUpdate = !(doc.Printed == true || doc.Emailed == true);
			Billing_Contact.Cache.AllowUpdate = doc.Emailed == false;

			PXUIFieldAttribute.SetEnabled<ARInvoice.docType>(cache, doc);
			PXUIFieldAttribute.SetEnabled<ARInvoice.refNbr>(cache, doc);

			Taxes.Cache.AllowDelete = Transactions.Cache.AllowDelete && !IsExternalTax;
			Taxes.Cache.AllowInsert = Transactions.Cache.AllowInsert && !IsExternalTax;
			Taxes.Cache.AllowUpdate = Taxes.Cache.AllowUpdate && !IsExternalTax;

			Adjustments.AllowSelect = doc.DocType != ARDocType.CreditMemo;
			Adjustments.Cache.AllowInsert = false;
			Adjustments.Cache.AllowDelete = false;
			Adjustments.Cache.AllowUpdate = Transactions.Cache.AllowUpdate;

			Adjustments_1.AllowSelect = doc.DocType == ARDocType.CreditMemo && doc.Released != true;
			Adjustments_1.Cache.AllowInsert = Transactions.Cache.AllowUpdate && doc.Scheduled != true;
			Adjustments_1.Cache.AllowDelete = Transactions.Cache.AllowUpdate && doc.Scheduled != true;
			Adjustments_1.Cache.AllowUpdate = Transactions.Cache.AllowUpdate && doc.Scheduled != true;

			Adjustments_2.AllowSelect = doc.DocType == ARDocType.CreditMemo && doc.Released == true;
			Adjustments_2.Cache.AllowInsert = Transactions.Cache.AllowUpdate && doc.Scheduled != true;
			Adjustments_2.Cache.AllowDelete = Transactions.Cache.AllowUpdate && doc.Scheduled != true;
			Adjustments_2.Cache.AllowUpdate = Transactions.Cache.AllowUpdate && doc.Scheduled != true;

			if (doc == null || customer.Current == null)
			{
				editCustomer.SetEnabled(false);
			}
			else
			{
				editCustomer.SetEnabled(true);
			}
			reverseInvoice.SetEnabled(doc != null && ((doc.Released ?? false) == true));

			if (doc.CustomerID != null)
			{
				if (Transactions.Any())
				{
					PXUIFieldAttribute.SetEnabled<ARInvoice.customerID>(cache, doc, false);
				}
			}

			if (ARSetup.Current != null)
			{
				PXUIFieldAttribute.SetVisible<ARInvoice.curyOrigDocAmt>(cache, e.Row, (bool)ARSetup.Current.RequireControlTotal || e.Row != null && (bool)((ARInvoice)e.Row).Released);
			}

			PXUIFieldAttribute.SetEnabled<ARInvoice.curyCommnblAmt>(cache, doc, false);
			PXUIFieldAttribute.SetEnabled<ARInvoice.curyCommnAmt>(cache, doc, false);

			if (ARSetup.Current != null)
			{
				PXUIFieldAttribute.SetVisible<ARInvoice.commnPct>(cache, e.Row, false);
				if ((bool)doc.Released || (bool)doc.Voided)
				{
					this.salesPerTrans.Cache.AllowInsert = false;
					this.salesPerTrans.Cache.AllowDelete = false;

					PXResult<ARSalesPerTran, ARSPCommissionPeriod> sptRes = (PXResult<ARSalesPerTran, ARSPCommissionPeriod>)this.salesPerTrans.Select();
					bool isCommnPeriodClosed = false;
					if (sptRes != null)
					{
						ARSPCommissionPeriod commnPeriod = (ARSPCommissionPeriod)sptRes;
						if (!String.IsNullOrEmpty(commnPeriod.CommnPeriodID) && commnPeriod.Status == ARSPCommissionPeriodStatus.Closed)
						{
							isCommnPeriodClosed = true;
						}
					}
					this.salesPerTrans.Cache.AllowUpdate = !isCommnPeriodClosed;

					PXUIFieldAttribute.SetEnabled<ARInvoice.workgroupID>(cache, e.Row, false);
					PXUIFieldAttribute.SetEnabled<ARInvoice.ownerID>(cache, e.Row, false);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled<ARInvoice.workgroupID>(cache, e.Row, true);
					PXUIFieldAttribute.SetEnabled<ARInvoice.ownerID>(cache, e.Row, true);
				}

			}

			PXUIFieldAttribute.SetVisible<ARTran.taskID>(Transactions.Cache, null, PM.ProjectAttribute.IsPMVisible(this, BatchModule.AR));
			if (IsExternalTax == true && ((ARInvoice)e.Row).IsTaxValid != true)
				PXUIFieldAttribute.SetWarning<ARInvoice.curyTaxTotal>(cache, e.Row, AR.Messages.TaxIsNotUptodate);

			ARAddress address = this.Billing_Address.Select();
			bool enableAddressValidation = doc.Released == false && (address != null && address.IsDefaultAddress == false && address.IsValidated == false);
			this.validateAddresses.SetEnabled(enableAddressValidation);

			//PXUIFieldAttribute.SetVisible<ARInvoice.avalaraCustomerUsageType>(cache, null, IsExternalTax);
			CT.ContractBillingTrace cbt = PXSelect<CT.ContractBillingTrace, Where<CT.ContractBillingTrace.contractID, Equal<Required<CT.ContractBillingTrace.contractID>>,
				And<CT.ContractBillingTrace.docType, Equal<Required<CT.ContractBillingTrace.docType>>, And<CT.ContractBillingTrace.refNbr, Equal<Required<CT.ContractBillingTrace.refNbr>>>>>>.SelectWindowed(this, 0, 1, doc.ProjectID, doc.DocType, doc.RefNbr);
			if (cbt != null)
			{
				//this invoice was created as a result of Contract/Project billing. Changing Project/Contract for this Invoice is not allowed.
				PXUIFieldAttribute.SetEnabled<ARInvoice.projectID>(cache, doc, false);
			}
			PXUIFieldAttribute.SetEnabled<ARInvoice.revoked>(cache, e.Row, true);
			bool applyFinChargeVisible = customer.Current != null && customer.Current.FinChargeApply == true && (doc.DocType == ARInvoiceType.Invoice || doc.DocType == ARInvoiceType.DebitMemo || (doc.DocType == ARInvoiceType.FinCharge && ARSetup.Current != null && ARSetup.Current.FinChargeOnCharge == true));
			PXUIFieldAttribute.SetVisible<ARInvoice.applyOverdueCharge>(cache, null, applyFinChargeVisible);
			bool applyFinChargeEnable = doc.Status != ARDocStatus.Closed || doc.LastFinChargeDate == null || doc.LastPaymentDate == null || doc.LastFinChargeDate <= doc.LastPaymentDate;
			PXUIFieldAttribute.SetEnabled<ARInvoice.applyOverdueCharge>(cache, null, applyFinChargeEnable);
	  
			SetDocTypeList(cache, e);

			bool showCashDiscountInfo = false;
			if (PXAccess.FeatureInstalled<FeaturesSet.vATReporting>() && 
				doc.CuryOrigDiscAmt > 0m &&
				doc.DocType != ARDocType.CreditMemo &&
				doc.DocType != ARDocType.SmallCreditWO)
			{
				Taxes.Select();
				showCashDiscountInfo = doc.HasPPDTaxes == true;
			}
			
			PXUIFieldAttribute.SetVisible<ARInvoice.curyDiscountedDocTotal>(cache, e.Row, showCashDiscountInfo);
			PXUIFieldAttribute.SetVisible<ARInvoice.curyDiscountedTaxableTotal>(cache, e.Row, showCashDiscountInfo);
			PXUIFieldAttribute.SetVisible<ARInvoice.curyDiscountedPrice>(cache, e.Row, showCashDiscountInfo);
			
			PXUIVisibility visibility = showCashDiscountInfo ? PXUIVisibility.Visible : PXUIVisibility.Invisible;
			PXUIFieldAttribute.SetVisibility<ARTaxTran.curyDiscountedPrice>(Taxes.Cache, null, visibility);
			PXUIFieldAttribute.SetVisibility<ARTaxTran.curyDiscountedTaxableAmt>(Taxes.Cache, null, visibility);
		}

		public virtual void SetDocTypeList(PXCache cache, PXRowSelectedEventArgs e)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.overdueFinCharges>())
			{
				Dictionary<string, string> allowed = new ARInvoiceType.ListAttribute().ValueLabelDic;
				allowed.Remove(ARInvoiceType.FinCharge);
				PXStringListAttribute.SetList<ARInvoice.docType>(cache, e.Row, allowed.Keys.ToArray(), allowed.Values.ToArray());
			}
		}

		protected virtual void ARInvoice_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row != null && ((ARInvoice)e.Row).Hold != null)
			{
				if ((bool)((ARInvoice)e.Row).Hold)
				{
					if ((((ARInvoice)e.Row).Status != "H"))
						sender.SetValue<ARInvoice.status>(e.Row, "H");
				}
				else if (!(bool)((ARInvoice)e.Row).Hold && ((ARInvoice)e.Row).Status == "H")
				{
					sender.SetValue<ARInvoice.status>(e.Row, "B");
				}
			}
		}

		protected virtual void ARInvoice_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARInvoice row = e.Row as ARInvoice;
			if (row != null)
			{
				foreach (ARTran tran in Transactions.Select())
				{
					tran.ProjectID = row.ProjectID;
					Transactions.Update(tran);
				}

			}
		}

		bool isReverse = false;
		protected virtual void ARInvoice_ProjectID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (isReverse)
				e.Cancel = true;
		}

		protected virtual void ARInvoice_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			ARReleaseProcess.UpdateARBalances(this, (ARInvoice)e.Row, -(((ARInvoice)e.Row).OrigDocAmt));
		}

		protected virtual void ARInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARInvoice row = (ARInvoice)e.Row;
			ARInvoice oldRow = (ARInvoice)e.OldRow;

			if (row.Released != true)
			{
				if (e.ExternalCall && !sender.ObjectsEqual<ARInvoice.docDate>(oldRow, row) && row.OrigDocType == null && row.OrigRefNbr == null)
				{
					DiscountEngine<ARTran>.AutoRecalculatePricesAndDiscounts<ARInvoiceDiscountDetail>
						(Transactions.Cache, Transactions, null, ARDiscountDetails, row.CustomerLocationID, row.DocDate);
				}

				if (IsExternalTax && Document.Current.InstallmentNbr == null && !sender.ObjectsEqual<ARInvoice.avalaraCustomerUsageType>(row, oldRow))
				{
					row.IsTaxValid = false;
					Document.Cache.SmartSetStatus(Document.Current, PXEntryStatus.Updated);
				}

				if (sender.GetStatus(row) != PXEntryStatus.Deleted && !sender.ObjectsEqual<ARInvoice.curyDiscTot>(oldRow, row))
				{
					if (!sender.Graph.IsImport)
					{
						AddDiscount(sender, row);
					}
					if (e.ExternalCall) 
					{
						DiscountEngine<ARTran>.CalculateDocumentDiscountRate<ARInvoiceDiscountDetail>
							(Transactions.Cache, Transactions, null, ARDiscountDetails, Document.Current.CuryLineTotal ?? 0m, Document.Current.CuryDiscTot ?? 0m);
					}
				}

				if (ARSetup.Current.RequireControlTotal != true && !sender.Graph.IsCopyPasteContext)
				{
					if (row.CuryDocBal != row.CuryOrigDocAmt)
					{
						sender.SetValueExt<ARInvoice.curyOrigDocAmt>(row, row.CuryDocBal != null && row.CuryDocBal != 0m ? row.CuryDocBal : 0m);
					}
				}

				if (row.Hold != true)
				{
					if (row.CuryDocBal != row.CuryOrigDocAmt)
					{
						sender.RaiseExceptionHandling<ARInvoice.curyOrigDocAmt>(row, row.CuryOrigDocAmt, 
							new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else if (row.CuryOrigDocAmt < 0m)
					{
						if (ARSetup.Current.RequireControlTotal == true)
						{
							sender.RaiseExceptionHandling<ARInvoice.curyOrigDocAmt>(row, row.CuryOrigDocAmt, 
								new PXSetPropertyException(Messages.DocumentBalanceNegative));
						}
						else
						{
							sender.RaiseExceptionHandling<ARInvoice.curyDocBal>(row, row.CuryDocBal, 
								new PXSetPropertyException(Messages.DocumentBalanceNegative));
						}
					}
					else
					{
							sender.RaiseExceptionHandling<ARInvoice.curyOrigDocAmt>(row, null, null);
							sender.RaiseExceptionHandling<ARInvoice.curyDocBal>(row, null, null);
					}
				}

				if (row.CustomerID != null && row.CuryDiscTot != null && row.CuryDiscTot > 0 && row.CuryLineTotal != null && row.CuryLineTotal > 0)
				{
					decimal discountLimit = DiscountEngine.GetDiscountLimit(sender, row.CustomerID);
					if ((row.CuryLineTotal / 100 * discountLimit) < row.CuryDiscTot)
					{
						PXUIFieldAttribute.SetWarning<ARInvoice.curyDiscTot>(sender, row,
                            PXMessages.LocalizeFormatNoPrefix(Messages.DocDiscountExceedLimit, discountLimit));
					}
				}
			}

			if (oldRow != null)
			{
				ARReleaseProcess.UpdateARBalances(this, oldRow, -oldRow.OrigDocAmt);
			}
			ARReleaseProcess.UpdateARBalances(this, row, row.OrigDocAmt);
		}

		protected virtual void ARInvoice_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			ARReleaseProcess.UpdateARBalances(this, (ARInvoice)e.Row, (((ARInvoice)e.Row).OrigDocAmt));
		}
		#endregion

		#region ARTran events

		protected virtual void ARTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			ARTran tran = (ARTran)e.Row;

			if (tran != null && tran.InventoryID == null && location.Current != null)
			{
				e.NewValue = location.Current.CSalesAcctID;
				e.Cancel = true;
			}
		}

		protected virtual void ARTran_AccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null && row.ProjectID != null && !PM.ProjectDefaultAttribute.IsNonProject(this, row.ProjectID) && row.TaskID != null)
			{
				Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, e.NewValue);
				if (account != null && account.AccountGroupID == null)
				{
					sender.RaiseExceptionHandling<ARTran.accountID>(e.Row, account.AccountCD, new PXSetPropertyException(PM.Messages.NoAccountGroup, PXErrorLevel.Warning, account.AccountCD));
				}
			}
		}

		protected virtual void ARTran_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null && row.TaskID == null)
			{
				sender.SetDefaultExt<ARTran.taskID>(e.Row);
			}
		}

		protected virtual void ARTran_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			ARTran tran = (ARTran)e.Row;
			if (tran != null && tran.AccountID != null && location.Current != null)
			{
				InventoryItem item = InventoryItemGetByID(tran.InventoryID);
				EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<ARTran.employeeID>>>>.SelectSingleBound(this, new object[] { e.Row });

				CRLocation companyloc =
					(CRLocation)PXSelectJoin<CRLocation, InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<GL.Branch, On<BAccountR.bAccountID, Equal<GL.Branch.bAccountID>>>>, Where<GL.Branch.branchID, Equal<Required<ARTran.branchID>>>>.Select(this, tran.BranchID);
				SalesPerson salesperson = (SalesPerson)PXSelect<SalesPerson, Where<SalesPerson.salesPersonID, Equal<Current<ARTran.salesPersonID>>>>.SelectSingleBound(this, new object[] { e.Row });

				int? customer_SubID = (int?)Caches[typeof(Location)].GetValue<Location.cSalesSubID>(location.Current);
				int? item_SubID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.salesSubID>(item);
				int? employee_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.salesSubID>(employee);
				int? company_SubID = (int?)Caches[typeof(CRLocation)].GetValue<CRLocation.cMPSalesSubID>(companyloc);
				int? salesperson_SubID = (int?)Caches[typeof(SalesPerson)].GetValue<SalesPerson.salesSubID>(salesperson);

				object value;
				try
				{
					value = SubAccountMaskAttribute.MakeSub<ARSetup.salesSubMask>(this, SalesSubMask,
						new object[] { customer_SubID, item_SubID, employee_SubID, company_SubID, salesperson_SubID },
						new Type[] { typeof(Location.cSalesSubID), typeof(InventoryItem.salesSubID), typeof(EPEmployee.salesSubID), typeof(Location.cMPSalesSubID), typeof(SalesPerson.salesSubID) });

					sender.RaiseFieldUpdating<ARTran.subID>(e.Row, ref value);
				}
				catch (PXException)
				{
					value = null;
				}

				e.NewValue = (int?)value;
				e.Cancel = true;
			}
		}

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category")]
		[ARTax(typeof(ARInvoice), typeof(ARTax), typeof(ARTaxTran))]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing, SearchOnDefault = false)]
		protected virtual void ARTran_TaxCategoryID_CacheAttached(PXCache sender)
		{
		}

		[PXBool]
		[DRTerms.Dates(typeof(ARTran.dRTermStartDate), typeof(ARTran.dRTermEndDate), typeof(ARTran.inventoryID), typeof(ARTran.deferredCode))]
		protected virtual void ARTran_RequiresTerms_CacheAttached(PXCache sender) { }

		protected virtual void ARTran_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if 
			(	TaxAttribute.GetTaxCalc<ARTran.taxCategoryID>(sender, e.Row) == TaxCalc.Calc && 
				taxzone.Current != null && !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) && 
				((ARTran)e.Row).InventoryID == null &&
				!isReverse
			)
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
			}
		}

		protected virtual void ARTran_UnitPrice_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (((ARTran)e.Row).InventoryID == null)
			{
				e.NewValue = 0m;
			}
		}

		protected virtual void ARTran_CuryUnitPrice_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row == null || row.InventoryID == null || row.UOM == null)
			{
				e.NewValue = sender.GetValue<ARTran.curyUnitPrice>(e.Row);
				e.Cancel = e.NewValue != null;
				return;
			}

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
				e.NewValue = ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, ((ARTran)e.Row).CustomerID, ((ARTran)e.Row).InventoryID, currencyinfo.Select(), ((ARTran)e.Row).UOM, ((ARTran)e.Row).Qty, date, row.CuryUnitPrice) ?? 0m;
			}
		}

		protected virtual void ARTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARTran.curyUnitPrice>(e.Row);
		}

		protected virtual void ARTran_OrigInvoiceDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARTran.curyUnitPrice>(e.Row);
		}

		protected virtual void ARTran_Qty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				if (row.Qty == 0)
				{
					sender.SetValueExt<ARTran.curyDiscAmt>(row, decimal.Zero);
					sender.SetValueExt<ARTran.discPct>(row, decimal.Zero);
				}
				else
				{
					sender.SetDefaultExt<ARTran.curyUnitPrice>(e.Row);
				}
			}
		}

		protected virtual void ARTran_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}

		protected virtual void ARTran_SOShipmentNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void ARTran_SalesPersonID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARTran.subID>(e.Row);
		}

		protected virtual void ARTran_EmployeeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARTran.subID>(e.Row);
		}

		protected virtual void ARTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran tran = e.Row as ARTran;

			Int32? accountID = tran.AccountID;
			sender.SetDefaultExt<ARTran.accountID>(e.Row);
			tran.AccountID = tran.AccountID ?? accountID;

			try
			{
				sender.SetDefaultExt<ARTran.subID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<ARTran.subID>(e.Row, null);
			}
			sender.SetDefaultExt<ARTran.taxCategoryID>(e.Row);
			sender.SetDefaultExt<ARTran.deferredCode>(e.Row);

			if (e.ExternalCall && tran != null)
				tran.CuryUnitPrice = 0m;

			sender.SetDefaultExt<ARTran.uOM>(e.Row);

			sender.SetDefaultExt<ARTran.curyUnitPrice>(e.Row);
			IN.InventoryItem item = PXSelectorAttribute.Select<IN.InventoryItem.inventoryID>(sender, tran) as IN.InventoryItem;
			if (item != null && tran != null)
			{
				tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, "Descr", customer.Current?.LocaleName);
			}

			SetDiscounts(sender, (ARTran)e.Row);
		}

		protected virtual void ARTran_ManualPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				if (row.ManualPrice != true && row.IsFree != true && !sender.Graph.IsCopyPasteContext)
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

					decimal price =
						ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, row.CustomerID,
						row.InventoryID,
						currencyinfo.Select(),
						row.UOM, row.Qty, date, row.CuryUnitPrice) ?? 0m;
					sender.SetValueExt<ARTran.curyUnitPrice>(e.Row, price);
				}
			}
		}

		protected virtual void ARTran_DefScheduleID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DRSchedule sc = PXSelect<DRSchedule, Where<DRSchedule.scheduleID, Equal<Required<DRSchedule.scheduleID>>>>.Select(this, ((ARTran)e.Row).DefScheduleID);
			if (sc != null)
			{
				ARTran defertran = PXSelect<ARTran, Where<ARTran.tranType, Equal<Required<ARTran.tranType>>,
					And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
					And<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>.Select(this, sc.DocType, sc.RefNbr, sc.LineNbr);

				if (defertran != null)
				{
					((ARTran)e.Row).DeferredCode = defertran.DeferredCode;
				}
			}
		}

		protected virtual void ARTran_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (e.ExternalCall && row != null)
			{
				DiscountEngine<ARTran>.UpdateManualLineDiscount<ARInvoiceDiscountDetail>(sender, Transactions, row, ARDiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate);
			}
		}

		protected virtual void ARTran_DeferredCode_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				if (row.TranType == ARDocType.CreditMemo)
				{
					DRDeferredCode dc = PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>.Select(this, e.NewValue);

					if (dc != null && dc.Method == DeferredMethodType.CashReceipt)
					{
						e.Cancel = true;
						if (sender.RaiseExceptionHandling<ARTran.deferredCode>(e.Row, e.NewValue, new PXSetPropertyException(Messages.InvalidCashReceiptDeferredCode)))
						{
							throw new PXSetPropertyException(Messages.InvalidCashReceiptDeferredCode);
						}
					}
				}
			}
		}

		protected virtual void ARTran_DiscPct_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<ARTran>.ValidateMinGrossProfitPct(sender, row, row.UnitPrice, ref newValue);
			e.NewValue = newValue;
		}

		protected virtual void ARTran_CuryDiscAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<ARTran>.ValidateMinGrossProfitAmt(sender, row, row.UnitPrice, ref newValue);
			e.NewValue = newValue;
		}

		protected virtual void ARTran_CuryUnitPrice_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				decimal? newValue = (decimal?)e.NewValue;
				SODiscountEngine<ARTran>.ValidateMinGrossProfitUnitPrice<ARTran.curyInfoID>(sender, row, ref newValue);
				e.NewValue = newValue;
			}
		}

		protected virtual void ARTran_CuryUnitPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null && row.ManualDisc == true)
			{
				row.CuryDiscAmt = (row.CuryUnitPrice ?? 0) * (row.Qty ?? 0) * (row.DiscPct ?? 0) * 0.01m;
				row.CuryTranAmt = (row.CuryUnitPrice ?? 0) * (row.Qty ?? 0) - row.CuryDiscAmt;
			}
		}

		protected virtual void ARTran_DRTermStartDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var line = e.Row as ARTran;

			if (line != null && line.RequiresTerms == true)
			{
				e.NewValue = Document.Current.DocDate;
			}
		}

		protected virtual void ARTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARTran row = (ARTran)e.Row;
			ARTran oldRow = (ARTran)e.OldRow;
			if (row != null)
			{
				if ((!sender.ObjectsEqual<ARTran.branchID>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.inventoryID>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<ARTran.qty>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyUnitPrice>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<ARTran.curyExtPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyDiscAmt>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<ARTran.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.manualDisc>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<ARTran.discountID>(e.Row, e.OldRow)) && row.LineType != SOLineType.Discount)
					RecalculateDiscounts(sender, row);

				if(row.ManualDisc != true)
				{
					var discountCode = (ARDiscount)PXSelectorAttribute.Select<SOLine.discountID>(sender, row);
					row.DiscPctDR = (discountCode != null && discountCode.IsAppliedToDR == true) ? row.DiscPct : 0.0m;
				}

				if ((e.ExternalCall || sender.Graph.IsImport)
					&& sender.ObjectsEqual<ARTran.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<ARTran.uOM>(e.Row, e.OldRow)
					&& sender.ObjectsEqual<ARTran.qty>(e.Row, e.OldRow) && sender.ObjectsEqual<ARTran.branchID>(e.Row, e.OldRow)
					 && sender.ObjectsEqual<ARTran.siteID>(e.Row, e.OldRow)
					&& (!sender.ObjectsEqual<ARTran.curyUnitPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARTran.curyExtPrice>(e.Row, e.OldRow))
					&& row.ManualPrice == oldRow.ManualPrice)
					row.ManualPrice = true;

				if(row.ManualPrice != true)
				{
					row.CuryUnitPriceDR = row.CuryUnitPrice;
				}

				TaxAttribute.Calculate<ARTran.taxCategoryID, ARTaxAttribute>(sender, e);

				//Validate that Sales Account <> Deferral Account:
				if (!sender.ObjectsEqual<ARTran.accountID, ARTran.deferredCode>(e.Row, e.OldRow))
				{
					if (!string.IsNullOrEmpty(row.DeferredCode))
					{
						DRDeferredCode defCode = PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>.Select(this, row.DeferredCode);
						if (defCode != null)
						{
							if (defCode.AccountID == row.AccountID)
							{
								sender.RaiseExceptionHandling<ARTran.accountID>(e.Row, row.AccountID,
									new PXSetPropertyException(Messages.AccountIsSameAsDeferred, PXErrorLevel.Warning));
							}
						}
					}
				}

				//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
				if (Document.Current != null && IsExternalTax == true && Document.Current.InstallmentNbr == null &&
					!sender.ObjectsEqual<ARTran.accountID, ARTran.inventoryID, ARTran.tranDesc, ARTran.tranAmt, ARTran.tranDate, ARTran.taxCategoryID>(e.Row, e.OldRow))
				{
					Document.Current.IsTaxValid = false;
					if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
						Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void ARTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			PXParentAttribute.SetParent(sender, e.Row, typeof(ARRegister), this.Document.Current);
		}

		protected virtual void ARTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			TaxAttribute.Calculate<ARTran.taxCategoryID, ARTaxAttribute>(sender, e);

			if (Document.Current != null && IsExternalTax == true && Document.Current.InstallmentNbr == null)
			{
				Document.Current.IsTaxValid = false;
				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
					Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
			}
		}

		protected virtual void ARTran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			ARTran row = (ARTran)e.Row;

			if (row != null)
			{
				PM.PMTran reference = RefPMTran.Select(row.PMTranID);
				if (reference != null)
				{
					reference.Billed = false;
					PM.RegisterReleaseProcess.AddToUnbilledSummary(this, reference);
					RefPMTran.Update(reference);
				}

				foreach (PM.PMTran pMRef in RefContractUsageTran.Select(row.TranType, row.RefNbr, row.LineNbr))
				{
					if (pMRef != null)
					{
						pMRef.ARRefNbr = null;
						pMRef.ARTranType = null;
						pMRef.RefLineNbr = null;
						pMRef.Billed = false;
						RefContractUsageTran.Update(pMRef);
					}
				}
			}

			if (Document.Current != null && Document.Current.InstallmentNbr == null)
			{
				Document.Current.IsTaxValid = false;
				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Notchanged)
					Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
			}

			if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
			{
				if (row.LineType != SOLineType.Discount)
					DiscountEngine<ARTran>.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, ARDiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate, true);
				RecalculateTotalDiscount();
			}
		}

		protected virtual void ARTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARTran row = e.Row as ARTran;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<ARTran.defScheduleID>(sender, row, row.TranType == ARInvoiceType.CreditMemo || row.TranType == ARInvoiceType.DebitMemo);
				PXUIFieldAttribute.SetEnabled<ARTran.deferredCode>(sender, row, row.DefScheduleID == null);
					}
				}


		protected virtual void ARTran_DiscountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region ARTaxTran Events
		protected virtual void ARTaxTran_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null)
			{
				e.NewValue = Document.Current.TaxZoneID;
				e.Cancel = true;
			}
		}

		protected virtual void ARTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<ARTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}

		protected virtual void ARTaxTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			PXParentAttribute.SetParent(sender, e.Row, typeof(ARRegister), this.Document.Current);
		}

		protected virtual void ARTaxTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (Document.Current != null && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				((ARTaxTran)e.Row).TaxZoneID = Document.Current.TaxZoneID;
			}
		}
		#endregion

		#region ARSalesPerTran events

		protected virtual void ARSalesPerTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			ARSalesPerTran row = (ARSalesPerTran)e.Row;
			foreach (ARSalesPerTran iSpt in this.salesPerTrans.Select())
			{
				if (iSpt.SalespersonID == row.SalespersonID)
				{
					PXEntryStatus status = this.salesPerTrans.Cache.GetStatus(iSpt);
					if (!(status == PXEntryStatus.InsertedDeleted || status == PXEntryStatus.Deleted))
					{
						sender.RaiseExceptionHandling<ARSalesPerTran.salespersonID>(e.Row, null, new PXException(Messages.ERR_DuplicatedSalesPersonAdded));
						e.Cancel = true;
						break;
					}
				}
			}
		}
		#endregion

		#region ARAdjust2 Events
		protected virtual void ARAdjust2_CuryAdjdAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARAdjust2 adj = (ARAdjust2)e.Row;
			Terms terms = PXSelect<Terms, Where<Terms.termsID, Equal<Current<ARInvoice.termsID>>>>.Select(this);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single && (decimal)e.NewValue > 0m)
			{
				throw new PXSetPropertyException(Messages.PrepaymentAppliedToMultiplyInstallments);
			}

			if (adj.CuryDocBal == null)
			{
				PXResult<ARPayment, CurrencyInfo> res = (PXResult<ARPayment, CurrencyInfo>)PXSelectReadonly2<ARPayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>, And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(this, adj.AdjgDocType, adj.AdjgRefNbr);

				ARPayment payment = res;
				CurrencyInfo pay_info = res;
				CurrencyInfo inv_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<ARInvoice.curyInfoID>>>>.Select(this);

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
				throw new PXSetPropertyException(Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjdAmt).ToString());
			}
		}

		protected virtual void ARAdjust2_CuryAdjdAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARAdjust2 adj = (ARAdjust2)e.Row;

			RecalculateApplicationAmounts(adj);

			adj.CuryDocBal = adj.CuryDocBal + (decimal?)e.OldValue - adj.CuryAdjdAmt;
		}

		protected virtual void ARAdjust2_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = true;
			e.Cancel = true;
		}
		#endregion

		#region ARInvoiceDiscountDetail events

		protected virtual void ARInvoiceDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARInvoiceDiscountDetail discountDetail = (ARInvoiceDiscountDetail)e.Row;
			if (discountDetail != null)
				PXUIFieldAttribute.SetEnabled<ARInvoiceDiscountDetail.skipDiscount>(sender, discountDetail, discountDetail.DiscountID != null);

			PXDefaultAttribute.SetPersistingCheck<ARInvoiceDiscountDetail.discountID>(sender, discountDetail, PXPersistingCheck.Nothing);

			if (Document != null && Document.Current != null)
				Document.Cache.SetValueExt<ARInvoice.curyDocDisc>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<ARInvoiceDiscountDetail>(ARDiscountDetails, true));
		}

		protected virtual void ARInvoiceDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARInvoiceDiscountDetail discountDetail = (ARInvoiceDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null && discountDetail.DiscountID != null)
			{
				if (!sender.ObjectsEqual<ARInvoiceDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARInvoiceDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
				{
					discountDetail.IsManual = true;
					DiscountEngine<ARTran>.InsertDocGroupDiscount<ARInvoiceDiscountDetail>(Transactions.Cache, Transactions, ARDiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<ARInvoiceDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate.Value);
					RecalculateTotalDiscount();
				}
				if (!sender.ObjectsEqual<ARInvoiceDiscountDetail.skipDiscount>(e.Row, e.OldRow))
			{
					DiscountEngine<ARTran>.UpdateDocumentDiscount<ARInvoiceDiscountDetail>(Transactions.Cache, Transactions, ARDiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate, discountDetail.Type != DiscountType.Document);
					RecalculateTotalDiscount();
				}
			}
		}

		protected virtual void ARInvoiceDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			ARInvoiceDiscountDetail discountDetail = (ARInvoiceDiscountDetail)e.Row;
			if (e.ExternalCall == true && discountDetail != null)
			{
				DiscountEngine<ARTran>.UpdateDocumentDiscount<ARInvoiceDiscountDetail>(Transactions.Cache, Transactions, ARDiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.DocDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document));
			}
			RecalculateTotalDiscount();
		}

		protected virtual void ARInvoiceDiscountDetail_DiscountSequenceID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}
		protected virtual void ARInvoiceDiscountDetail_DiscountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
		}       
		#endregion

		private InventoryItem InventoryItemGetByID(int? inventoryID)
		{
			return PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);
		}

		private DRDeferredCode DeferredCodeGetByID(string deferredCodeID)
		{
			return PXSelect<DRDeferredCode, Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>.Select(this, deferredCodeID);
		}

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (string.Compare(viewName, "Transactions", true) == 0)
			{
				keys["tranType"] = Document.Current.DocType;
				keys["refNbr"] = Document.Current.RefNbr;
				if (DontUpdateExistRecords)
				{
					keys["lineNbr"] = Document.Current.LineCntr + 1;
				}
			}
			return true;
		}

		private static bool DontUpdateExistRecords
		{
			get
			{
				object dontUpdateExistRecords;
				return
					PX.Common.PXExecutionContext.Current.Bag.TryGetValue(PXImportAttribute._DONT_UPDATE_EXIST_RECORDS,
																		 out dontUpdateExistRecords) &&
																		 true.Equals(dontUpdateExistRecords);
			}
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

		public virtual ARInvoice CalculateAvalaraTax(ARInvoice invoice)
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
				Document.SetValueExt<ARInvoice.isTaxValid>(invoice, true);
				if (invoice.IsTaxSaved == true)
					Document.SetValueExt<ARInvoice.isTaxSaved>(invoice, false);
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

		protected virtual GetTaxRequest BuildGetTaxRequest(ARInvoice invoice)
		{
			if (invoice == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			Customer cust = (Customer)customer.View.SelectSingleBound(new object[] { invoice });
			Location loc = (Location)location.View.SelectSingleBound(new object[] { invoice });

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CurrencyCode = invoice.CuryID;
			request.CustomerCode = cust.AcctCD;
			IAddressBase fromAddress = GetFromAddress(invoice);
			IAddressBase toAddress = GetToAddress(invoice);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFrom);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetTo);

			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("AR.{0}.{1}", invoice.DocType, invoice.RefNbr);
			request.DocDate = invoice.DocDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(invoice);
			if (!string.IsNullOrEmpty(invoice.AvalaraCustomerUsageType))
			{
				request.CustomerUsageType = invoice.AvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.SalesInvoice;
			int mult = invoice.DocType == ARDocType.CreditMemo ? -1 : 1;

			PXSelectBase<ARTran> select = new PXSelectJoin<ARTran,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARTran.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<ARTran.accountID>>>>,
				Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
					And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
					And<Where<ARTran.lineType, NotEqual<SOLineType.discount>, Or<ARTran.lineType, IsNull>>>>>,
				OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>(this);

			request.Discount = GetDocDiscount().GetValueOrDefault();
			DateTime? taxDate = invoice.OrigDocDate;

			foreach (PXResult<ARTran, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { invoice }))
			{
				ARTran tran = (ARTran)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);
				line.Amount = mult * tran.CuryTranAmt.GetValueOrDefault();
				line.Description = tran.TranDesc;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Convert.ToDouble(tran.Qty.GetValueOrDefault());
				line.Discounted = tran.LineType != SOLineType.Freight ? request.Discount > 0 : false;
				line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

				if (avalaraSetup.Current != null && avalaraSetup.Current.SendRevenueAccount == true)
					line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;

				if (tran.OrigInvoiceDate != null)
					taxDate = tran.OrigInvoiceDate;

				request.Lines.Add(line);
			}



			switch (invoice.DocType)
			{
				case ARDocType.Invoice:
				case ARDocType.DebitMemo:
				case ARDocType.FinCharge:
				case ARDocType.CashSale:
					request.DocType = DocumentType.SalesInvoice;
					break;
				case ARDocType.CreditMemo:
				case ARDocType.CashReturn:
					if (invoice.OrigDocDate != null)
					{
						request.TaxOverride.Reason = "Return";
						request.TaxOverride.TaxDate = taxDate.Value;
						request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
						mult = -1;
					}
					request.DocType = DocumentType.ReturnInvoice;
					break;

				default:
					throw new PXException(Messages.DocTypeNotSupported);
			}

			return request;
		}

		protected bool skipAvalaraCallOnSave = false;
		public virtual void ApplyAvalaraTax(ARInvoice invoice, GetTaxResult result)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { invoice });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(TX.Messages.ExternalTaxVendorNotFound);

			if (vendor.SalesTaxAcctID == null)
				throw new PXException(TX.Messages.TaxPayableAccountNotSpecified, vendor.AcctCD);

			if (vendor.SalesTaxSubID == null)
				throw new PXException(TX.Messages.TaxPayableSubNotSpecified, vendor.AcctCD);

			//Clear all existing Tax transactions:
			foreach (PXResult<ARTaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { invoice }))
			{
				ARTaxTran taxTran = (ARTaxTran)res;
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

				ARTaxTran tax = new ARTaxTran();
				tax.Module = BatchModule.AR;
				tax.TranType = invoice.DocType;
				tax.RefNbr = invoice.RefNbr;
				tax.TaxID = taxID;
				tax.CuryTaxAmt = Math.Abs(result.TaxSummary[i].Tax);
				tax.CuryTaxableAmt = Math.Abs(result.TaxSummary[i].Taxable);
				tax.TaxRate = Convert.ToDecimal(result.TaxSummary[i].Rate) * 100;
				tax.JurisType = result.TaxSummary[i].JurisType.ToString();
				tax.JurisName = result.TaxSummary[i].JurisName;
				tax.TaxType = "S";
				tax.TaxBucketID = 0;
				tax.AccountID = vendor.SalesTaxAcctID;
				tax.SubID = vendor.SalesTaxSubID;

				Taxes.Insert(tax);
			}


			bool requireControlTotal = ARSetup.Current.RequireControlTotal == true;

			if (invoice.Hold != true)
				ARSetup.Current.RequireControlTotal = false;


			try
			{
				invoice.CuryTaxTotal = Math.Abs(result.TotalTax);
				Document.Cache.SetValueExt<ARInvoice.isTaxSaved>(invoice, true);
			}
			finally
			{
				ARSetup.Current.RequireControlTotal = requireControlTotal;
			}
		}

		public virtual void CancelTax(ARInvoice invoice, CancelCode code)
		{
			CancelTaxRequest request = new CancelTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CancelCode = code;
			request.DocCode = string.Format("AR.{0}.{1}", invoice.DocType, invoice.RefNbr);
			request.DocType = DocumentType.SalesInvoice;

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

		public virtual IAddressBase GetFromAddress(ARInvoice invoice)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(invoice.BranchID))
				return (Address)res;

			return null;
		}

		
		/// <summary>
		/// Returns Ship To address. 
		/// Validates that Common carrier and Local carrier are not be mixed in one invoice.
		/// Validates that Invoice referencing multiple shipments are not shipped to different locations.
		/// </summary>
		/// <param name="invoice"></param>
		/// <returns></returns>
		public virtual IAddressBase GetToAddress(ARInvoice invoice)
		{
			IAddressBase result;
			if (invoice.OrigModule == BatchModule.SO)
			{
				PXSelectBase<SOOrderShipment> orderShipments = new PXSelectJoin<SOOrderShipment,
					LeftJoin<SOShipment, On<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>,
							And<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>>>,
					LeftJoin<Carrier, On<Carrier.carrierID, Equal<SOShipment.shipVia>>>>,
					Where<SOOrderShipment.invoiceType, Equal<Required<SOOrderShipment.invoiceType>>,
						And<SOOrderShipment.invoiceNbr, Equal<Required<SOOrderShipment.invoiceNbr>>>>>(this);

				bool? isCommonCarrier = null;
				List<int> shipments = new List<int>();
				foreach (PXResult<SOOrderShipment, SOShipment, Carrier> res in orderShipments.Select(invoice.DocType, invoice.RefNbr))
				{
					Carrier carrier = (Carrier)res;
					SOOrderShipment ship = (SOOrderShipment)res;

					if (carrier.CarrierID == null)
					{
					   Carrier orderCarrier = PXSelectJoin<Carrier, InnerJoin<SOOrder, On<Carrier.carrierID, Equal<SOOrder.shipVia>>>,
							Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
							And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(this, ship.OrderType,ship.OrderNbr);

						if (orderCarrier != null )
							carrier = orderCarrier;
					}


					if (carrier.CarrierID != null)
					{
						if (isCommonCarrier == null)
						{
							isCommonCarrier = carrier.IsCommonCarrier == true;
						}
						else
						{
							if (isCommonCarrier != carrier.IsCommonCarrier)
							{
								throw new PXException(Messages.CarriersCannotBeMixed);
							}
						}
					}

					if (ship.ShipAddressID != null && !shipments.Contains(ship.ShipAddressID.Value))
						shipments.Add(ship.ShipAddressID.Value);
				}
				if (shipments.Count == 0)
				{
					var orders = new PXSelectJoin<SOOrderShipment,
						LeftJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>,
							And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
							LeftJoin<Carrier, On<Carrier.carrierID, Equal<SOOrder.shipVia>>>>,
						Where<SOOrderShipment.invoiceType, Equal<Required<SOOrderShipment.invoiceType>>,
							And<SOOrderShipment.invoiceNbr, Equal<Required<SOOrderShipment.invoiceNbr>>>>>(this);
					foreach (PXResult<SOOrderShipment, SOOrder, Carrier> res in orders.Select(invoice.DocType, invoice.RefNbr))
					{
						Carrier carrier = (Carrier)res;
						if (carrier != null && carrier.CarrierID != null)
						{
							if (isCommonCarrier == null)
							{
								isCommonCarrier = carrier.IsCommonCarrier == true;
							}
						}
					}
				}

				if (isCommonCarrier == true)
				{
					if (shipments.Count > 1)
					{
						throw new PXException(Messages.MultipleShipAddressOnInvoice);
					}

					PXSelectBase<SOAddress> select = new PXSelectJoin<SOAddress,
				InnerJoin<SOInvoice, On<SOInvoice.shipAddressID, Equal<SOAddress.addressID>>>,
				Where<SOInvoice.docType, Equal<Required<SOInvoice.docType>>, And<SOInvoice.refNbr, Equal<Required<SOInvoice.refNbr>>>>>(this);

					result = (SOAddress)select.SelectSingle(invoice.DocType, invoice.RefNbr);
				}
				else
				{
					//When this is not a common carrier, we calculate tax at brand where goods are sold, delivered from or picked up
					return GetFromAddress(invoice);
				}

			}
			else
			{
				result = (Address)PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(this, customer.Current.DefAddressID);
			}

			return result;
		}

		public virtual decimal? GetDocDiscount()
		{
			return null;
		}

		#endregion


		private void RecalculateApplicationAmounts(ARAdjust2 application)
		{
			PXCache applicationCache = this.Caches[typeof(ARAdjust2)];

			decimal currencyAdjustingAmount;
			decimal baseAdjustedAmount;
			decimal baseAdjustingAmount;

			PXDBCurrencyAttribute.CuryConvBase<ARAdjust2.adjdCuryInfoID>(
				applicationCache, 
				application, 
				(decimal)application.CuryAdjdAmt, 
				out baseAdjustedAmount);

			CurrencyInfo applicationCurrencyInfo = PXSelect<
				CurrencyInfo, 
				Where<CurrencyInfo.curyInfoID, Equal<Current<ARAdjust2.adjgCuryInfoID>>>>
				.SelectSingleBound(this, new object[] { application });

			CurrencyInfo invoiceCurrencyInfo = PXSelect<
				CurrencyInfo, 
				Where<CurrencyInfo.curyInfoID, Equal<Current<ARAdjust2.adjdCuryInfoID>>>>
				.SelectSingleBound(this, new object[] { application });

			if (string.Equals(applicationCurrencyInfo.CuryID, invoiceCurrencyInfo.CuryID))
			{
				currencyAdjustingAmount = (decimal)application.CuryAdjdAmt;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvCury<ARAdjust2.adjgCuryInfoID>(
					applicationCache, 
					application, 
					baseAdjustedAmount, 
					out currencyAdjustingAmount);
			}

			if (object.Equals(applicationCurrencyInfo.CuryID, invoiceCurrencyInfo.CuryID) && 
				object.Equals(applicationCurrencyInfo.CuryRate, invoiceCurrencyInfo.CuryRate) && 
				object.Equals(applicationCurrencyInfo.CuryMultDiv, invoiceCurrencyInfo.CuryMultDiv))
			{
				baseAdjustingAmount = baseAdjustedAmount;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvBase<ARAdjust2.adjgCuryInfoID>(
					applicationCache, 
					application, 
					currencyAdjustingAmount, 
					out baseAdjustingAmount);
			}

			application.CuryAdjgAmt = currencyAdjustingAmount;
			application.AdjAmt = baseAdjustedAmount;
			application.RGOLAmt = baseAdjustingAmount - baseAdjustedAmount;
		}

		public override void RecalculateDiscounts(PXCache sender, ARTran line)
		{
			if (line.Qty != null && line.CuryLineAmt != null && line.IsFree != true)
				DiscountEngine<ARTran>.SetDiscounts<ARInvoiceDiscountDetail>(sender, Transactions, line, ARDiscountDetails, Document.Current.BranchID, Document.Current.CustomerLocationID, Document.Current.CuryID, Document.Current.DocDate, true, recalcdiscountsfilter.Current);
			if (line.CuryLineAmt != null && line.IsFree != true)
			{
				RecalculateTotalDiscount();
		}
		}

		private void RecalculateTotalDiscount()
		{
			if (Document.Current != null)
			{
				ARInvoice old_row = PXCache<ARInvoice>.CreateCopy(Document.Current);
				Document.Cache.SetValueExt<ARInvoice.curyDiscTot>(Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<ARInvoiceDiscountDetail>(ARDiscountDetails));
				Document.Cache.RaiseRowUpdated(Document.Current, old_row);
			}
		}

		private void CheckApplicationDateAndPeriod(PXCache sender, ARInvoice document, ARAdjust application)
		{
			if (document == null) throw new ArgumentNullException(nameof(document));
			if (application == null) throw new ArgumentNullException(nameof(application));

			if (application.AdjdDocDate > document.DocDate)
			{
				if (sender.RaiseExceptionHandling<ARAdjust.adjdRefNbr>(
					application,
					application.AdjdRefNbr,
					new PXSetPropertyException(
						Messages.UnableToApplyDocumentApplicationDateEarlierThanDocumentDate,
						PXErrorLevel.RowError)))
				{
					throw new PXRowPersistingException(
						PXDataUtils.FieldName<ARAdjust.adjdDocDate>(),
						application.AdjdDocDate,
						Messages.UnableToApplyDocumentApplicationDateEarlierThanDocumentDate);
				}
			}

			if (application.AdjdFinPeriodID?.CompareTo(document.FinPeriodID) > 0)
			{
				if (sender.RaiseExceptionHandling<ARAdjust.adjdRefNbr>(
					application,
					application.AdjdRefNbr,
					new PXSetPropertyException(
						Messages.UnableToApplyDocumentApplicationPeriodPrecedesDocumentPeriod,
						PXErrorLevel.RowError)))
				{
					throw new PXRowPersistingException(
						PXDataUtils.FieldName<ARAdjust.adjdFinPeriodID>(),
						application.AdjdFinPeriodID,
						Messages.UnableToApplyDocumentApplicationPeriodPrecedesDocumentPeriod);
				}
			}
		}

		public virtual void AddDiscount(PXCache sender, ARInvoice row)
		{
			ARTran discount = (ARTran)Discount_Row.Cache.CreateInstance();
			discount.LineType = SOLineType.Discount;
			discount.DrCr = (Document.Current.DrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
			discount.FreezeManualDisc = true;
			discount = (ARTran)Discount_Row.Select() ?? (ARTran)Discount_Row.Cache.Insert(discount);

			ARTran old_row = (ARTran)Discount_Row.Cache.CreateCopy(discount);

			discount.CuryTranAmt = (decimal?)sender.GetValue<ARInvoice.curyDiscTot>(row);
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

			if (Discount_Row.Cache.GetStatus(discount) == PXEntryStatus.Notchanged)
			{
				Discount_Row.Cache.SetStatus(discount, PXEntryStatus.Updated);
			}

			discount.ManualDisc = true; //escape SOManualDiscMode.RowUpdated
			Discount_Row.Cache.RaiseRowUpdated(discount, old_row);

			decimal auotDocDisc = DiscountEngine.GetTotalGroupAndDocumentDiscount<ARInvoiceDiscountDetail>(ARDiscountDetails);
			if (auotDocDisc == discount.CuryTranAmt)
			{
				discount.ManualDisc = false;
			}
		}

		public object GetValue<Field>(object data)
			where Field : IBqlField
		{
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

		private void DefaultDiscountAccountAndSubAccount(ARTran tran)
		{
			Location customerloc = location.Current;
			//Location companyloc = (Location)PXSelectJoin<Location, InnerJoin<BAccountR, On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<Branch, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>>>, Where<Branch.branchID, Equal<Current<ARRegister.branchID>>>>.Select(this);

			object customer_LocationAcctID = GetValue<Location.cDiscountAcctID>(customerloc);
			//object company_LocationAcctID = GetValue<Location.cDiscountAcctID>(companyloc);


			if (customer_LocationAcctID != null)
			{
				tran.AccountID = (int?)customer_LocationAcctID;
				Discount_Row.Cache.RaiseFieldUpdated<ARTran.accountID>(tran, null);
			}

			if (tran.AccountID != null)
			{
				object customer_LocationSubID = GetValue<Location.cDiscountSubID>(customerloc);
				if (customer_LocationSubID != null)
				{
					tran.SubID = (int?)customer_LocationSubID;
					Discount_Row.Cache.RaiseFieldUpdated<ARTran.subID>(tran, null);
				}
			}
		}

		#region CreditMemo Application

		[PXDBInt(IsKey = true)]
		[PXDefault(0)]
		protected virtual void ARAdjust_AdjNbr_CacheAttached(PXCache sender)
		{ 
		}

		[PXDBDate()]
		[PXDBDefault(typeof(ARInvoice.docDate))]
		protected virtual void ARAdjust_AdjgDocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(6, IsFixed = true)]
		[PXDBDefault(typeof(ARInvoice.finPeriodID))]
		protected virtual void ARAdjust_AdjgFinPeriodID_CacheAttached(PXCache sender)
		{
			}

		[PXDBString(6, IsFixed = true)]
		[PXDBDefault(typeof(ARInvoice.tranPeriodID))]
		protected virtual void ARAdjust_AdjgTranPeriodID_CacheAttached(PXCache sender)
		{
		}

		[PXDBCurrency(typeof(ARAdjust.adjgCuryInfoID), typeof(ARAdjust.adjAmt), BaseCalc = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Paid", Visibility = PXUIVisibility.Visible)]
		protected virtual void ARAdjust_CuryAdjgAmt_CacheAttached(PXCache sender)
		{
		}

		[PXDBCurrency(typeof(ARAdjust.adjgCuryInfoID), typeof(ARAdjust.adjDiscAmt), BaseCalc = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void ARAdjust_CuryAdjgDiscAmt_CacheAttached(PXCache sender)
		{
		}

		[PXDBCurrency(typeof(ARAdjust.adjgCuryInfoID), typeof(ARAdjust.adjWOAmt), BaseCalc = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		protected virtual void ARAdjust_CuryAdjgWOAmt_CacheAttached(PXCache sender)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Switch<
				Case<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, ARAdjust.adjdDocType,
				Case<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusting>>, ARAdjust.adjgDocType>>>))]
		protected virtual void ARAdjust_DisplayDocType_CacheAttached(PXCache sender) {}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Switch<
				Case<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, ARAdjust.adjdRefNbr,
				Case<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusting>>, ARAdjust.adjgRefNbr>>>))]
		protected virtual void ARAdjust_DisplayRefNbr_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<ARAdjust.displayRefNbr, Selector<Standalone.ARRegister.customerID, BAccount.acctCD>>))]
		protected virtual void ARAdjust_DisplayCustomerID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<ARAdjust.displayRefNbr, Standalone.ARRegister.docDate>))]
		protected virtual void ARAdjust_DisplayDocDate_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<ARAdjust.displayRefNbr, Standalone.ARRegister.docDesc>))]
		protected virtual void ARAdjust_DisplayDocDesc_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(FormatPeriodID<FormatDirection.display, Selector<ARAdjust.displayRefNbr, Standalone.ARRegister.finPeriodID>>))]
		protected virtual void ARAdjust_DisplayFinPeriodID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<ARAdjust.displayRefNbr, Standalone.ARRegister.curyID>))]
		protected virtual void ARAdjust_DisplayCuryID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Selector<ARAdjust.displayRefNbr, Standalone.ARRegister.status>))]
		protected virtual void ARAdjust_DisplayStatus_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(IIf<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, ARAdjust.curyAdjgAmt, ARAdjust.curyAdjdAmt>))]
		protected virtual void ARAdjust_DisplayCuryAmt_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[CurrencyInfo]
		[PXFormula(typeof(IIf<Where<ARAdjust.adjType, Equal<ARAdjust.adjType.adjusted>>, ARAdjust.adjgCuryInfoID, ARAdjust.adjdCuryInfoID>))]
		protected virtual void ARAdjust_DisplayCuryInfoID_CacheAttached(PXCache sender) { }

		protected virtual void ARAdjust_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (((ARAdjust)e.Row).AdjdCuryInfoID != ((ARAdjust)e.Row).AdjgCuryInfoID && ((ARAdjust)e.Row).AdjdCuryInfoID != ((ARAdjust)e.Row).AdjdOrigCuryInfoID && ((ARAdjust)e.Row).VoidAdjNbr == null)
			{
				foreach (CurrencyInfo info in CurrencyInfo_CuryInfoID.Select(((ARAdjust)e.Row).AdjdCuryInfoID))
				{
					currencyinfo.Delete(info);
				}
			}
		}

		protected virtual void ARAdjust_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARAdjust doc = (ARAdjust)e.Row;

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				return;
			}

			if (Document.Current != null)
			{
				CheckApplicationDateAndPeriod(sender, Document.Current, doc);
			}

			if (doc.CuryDocBal < 0m)
			{
				sender.RaiseExceptionHandling<ARAdjust.curyAdjgAmt>(e.Row, doc.CuryAdjgAmt, new PXSetPropertyException(Messages.DocumentBalanceNegative));
			}
		}

		protected virtual void ARAdjust_AdjdRefNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			try
			{
				ARAdjust adj = (ARAdjust)e.Row;
				if (adj.AdjdCuryInfoID == null)
				{
					foreach (PXResult<ARInvoice, CurrencyInfo> res in PXSelectJoin<ARInvoice, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<ARInvoice.curyInfoID>>>, Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(this, adj.AdjdDocType, adj.AdjdRefNbr))
					{
						ARAdjust_AdjdRefNbr_FieldUpdated<ARInvoice>(res, adj);
						return;
					}
				}
			}
			catch (PXSetPropertyException ex)
			{
				throw new PXException(ex.Message);
			}
		}

		private void ARAdjust_AdjdRefNbr_FieldUpdated<T>(PXResult<T, CurrencyInfo> res, ARAdjust adj)
			where T : ARRegister, IInvoice, new()
		{
			CurrencyInfo info_copy = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
			info_copy.CuryInfoID = null;
			info_copy = (CurrencyInfo)currencyinfo.Cache.Insert(info_copy);
			T invoice = (T)res;

			//currencyinfo.Cache.SetValueExt<CurrencyInfo.curyEffDate>(info_copy, Document.Current.DocDate);
			info_copy.SetCuryEffDate(currencyinfo.Cache, Document.Current.DocDate);

			adj.CustomerID = Document.Current.CustomerID;
			adj.AdjgDocDate = Document.Current.DocDate;
			adj.AdjgCuryInfoID = Document.Current.CuryInfoID;
			adj.AdjdCuryInfoID = info_copy.CuryInfoID;
			adj.AdjdCustomerID = invoice.CustomerID;
			adj.AdjdOrigCuryInfoID = invoice.CuryInfoID;
			adj.AdjdBranchID = invoice.BranchID;
			adj.AdjdARAcct = invoice.ARAccountID;
			adj.AdjdARSub = invoice.ARSubID;
			adj.AdjdDocDate = invoice.DocDate;
			adj.AdjdFinPeriodID = invoice.FinPeriodID;
			adj.Released = false;

			CalcBalances<T>(adj, invoice, false);

			decimal? CuryApplAmt = adj.CuryDocBal;
			//TODO: accumulate Unapplied Balance
			decimal? CuryUnappliedBal = 0m;

			if (Document.Current != null && CuryUnappliedBal > 0m)
			{
				CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);
			}
			else if (Document.Current != null && CuryUnappliedBal <= 0m && Document.Current.CuryOrigDocAmt > 0)
			{
				CuryApplAmt = 0m;
			}

			adj.CuryAdjgAmt = CuryApplAmt;
			adj.CuryAdjgDiscAmt = 0m;
			adj.CuryAdjgWOAmt = 0m;

			CalcBalances<T>(adj, invoice, true);
		}

		protected bool internalCall;

		protected virtual void ARAdjust_CuryDocBal_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (!internalCall)
			{
				if (e.Row != null && ((ARAdjust)e.Row).AdjdCuryInfoID != null && ((ARAdjust)e.Row).CuryDocBal == null && sender.GetStatus(e.Row) != PXEntryStatus.Deleted)
				{
					CalcBalances((ARAdjust)e.Row, false, false);
				}
				if (e.Row != null)
				{
					e.NewValue = ((ARAdjust)e.Row).CuryDocBal;
				}
			}
			e.Cancel = true;
		}

		protected virtual void ARAdjust_CuryAdjgAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARAdjust adj = (ARAdjust)e.Row;

			if (adj.CuryDocBal == null || adj.CuryDiscBal == null || adj.CuryWOBal == null)
			{
				CalcBalances((ARAdjust)e.Row, false, false);
			}

			if (adj.CuryDocBal == null)
			{
				throw new PXSetPropertyException<ARAdjust.adjdRefNbr>(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<ARAdjust.adjdRefNbr>(sender));
			}

			if ((decimal)e.NewValue < 0m)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GE, ((int)0).ToString());
			}

			if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgAmt - (decimal)e.NewValue < 0)
			{
				throw new PXSetPropertyException(Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgAmt).ToString());
			}
		}

		protected virtual void ARAdjust_CuryAdjgAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CalcBalances((ARAdjust)e.Row, true, false);
		}

		protected void CalcBalances(ARAdjust row, bool isCalcRGOL)
		{
			CalcBalances(row, isCalcRGOL, true);
		}

		protected void CalcBalances(ARAdjust adj, bool isCalcRGOL, bool DiscOnDiscDate)
		{
			foreach (ARInvoice invoice in ARInvoice_CustomerID_DocType_RefNbr.Select(adj.AdjdCustomerID, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				CalcBalances(adj, invoice, isCalcRGOL, DiscOnDiscDate);
				return;
			}
		}

		protected void CalcBalances<T>(ARAdjust adj, T invoice, bool isCalcRGOL)
			where T : class, IBqlTable, IInvoice, new()
		{
			CalcBalances<T>(adj, invoice, isCalcRGOL, true);
		}

		protected void CalcBalances<T>(ARAdjust adj, T invoice, bool isCalcRGOL, bool DiscOnDiscDate)
			where T : class, IBqlTable, IInvoice, new()
		{
			PaymentEntry.CalcBalances(CurrencyInfo_CuryInfoID, adj.AdjgCuryInfoID, adj.AdjdCuryInfoID, invoice, adj);
			if (DiscOnDiscDate)
			{
				PaymentEntry.CalcDiscount(adj.AdjgDocDate, invoice, adj);
			}

			adj.CuryWOBal = 0m;
			adj.WOBal = 0m;

			invoice.CuryWhTaxBal = 0m;
			invoice.WhTaxBal = 0m;

			PaymentEntry.AdjustBalance(CurrencyInfo_CuryInfoID, adj);
			if (isCalcRGOL && (adj.Voided != true))
			{
				PaymentEntry.CalcRGOL(CurrencyInfo_CuryInfoID, invoice, adj);
				adj.RGOLAmt = adj.ReverseGainLoss == true ? -1m * adj.RGOLAmt : adj.RGOLAmt;
			}
		}
		#endregion
		
	}

	[Serializable]
	public partial class DuplicateFilter : IBqlTable
	{
		#region RefNbr
		public abstract class refNbr : IBqlField {}
		protected string _RefNbr;
		[PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "New Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string RefNbr
			{
			get { return this._RefNbr; }
			set { this._RefNbr = value; }
		}
		#endregion
	}
}
