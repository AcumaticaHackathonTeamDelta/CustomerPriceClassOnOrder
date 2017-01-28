using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects.EP;
using PX.Objects.CR;

using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.CA
{
	public class CATranEntry : PXGraph<CATranEntry, CAAdj>, PX.Objects.GL.IVoucherEntry
	{
		public PXAction DeleteButton
		{
			get
			{
				return this.Delete;
			}
		}
		#region Cache Attached Events
		[Account(typeof(CASplit.branchID), typeof(Search2<Account.accountID, LeftJoin<CashAccount, On<CashAccount.accountID, Equal<Account.accountID>>,
													InnerJoin<CAEntryType, On<CAEntryType.entryTypeId, Equal<Current<CAAdj.entryTypeID>>>>>,
												Where<CAEntryType.useToReclassifyPayments, Equal<False>,
                                                    Or<Where<CashAccount.cashAccountID, IsNotNull,
														And<CashAccount.curyID, Equal<Current<CAAdj.curyID>>,
                                                        And<CashAccount.cashAccountID, NotEqual<Current<CAAdj.cashAccountID>>>>>>>>), 
														DisplayName = "Offset Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), CacheGlobal = false)]
		[PXDefault()]
		protected virtual void CASplit_AccountID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		
		public PXCache dailycache
		{
			get
			{
				return this.Caches[typeof(CADailySummary)];
			}
		}

		public PXCache catrancache
		{
			get
			{
				return this.Caches[typeof(CATran)];
			}
		}

		public PXCache gltrancache
		{
			get
			{
				return this.Caches[typeof(GLTran)];
			}
		}

		public CATranEntry()
		{
			CASetup setup = casetup.Current;
			PXUIFieldAttribute.SetDisplayName<Account.description>(Caches[typeof(Account)], Messages.AccountDescription);
			this.FieldSelecting.AddHandler<CAAdj.tranID_CATran_batchNbr>(CAAdj_TranID_CATran_BatchNbr_FieldSelecting);
			PXUIFieldAttribute.SetVisible<CASplit.projectID>(CASplitRecords.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.CA));
			PXUIFieldAttribute.SetVisible<CASplit.taskID>(CASplitRecords.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.CA));
			PXUIFieldAttribute.SetVisible<CASplit.nonBillable>(CASplitRecords.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.CA));

			Approval.Cache.AllowUpdate = false;
            Approval.Cache.AllowInsert = false;
            Approval.Cache.AllowDelete = false;
        }

		public TaxZone TAXZONE
		{
			get
			{
				return taxzone.Select();
			}
		}

		#region Buttons
		public PXAction<CAAdj> hold;
		[PXUIField(DisplayName = "Hold", Visible = false)]
		[PXButton]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			return adapter.Get();
		}
        public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
        {
            if (viewName.ToLower() == "caadjrecords" && values != null)
            {
                if (IsExport || IsMobile || IsContractBasedAPI)
                {
                    CAAdjRecords.Cache.Locate(keys);
                    if (values.Contains("Hold") && values["Hold"] != PXCache.NotSetValue && values["Hold"] != null)
                    {
                        var hold = CAAdjRecords.Current.Hold ?? false;
                        if (Convert.ToBoolean(values["Hold"]) != hold)
                        {
                            ((PXAction<CAAdj>)this.Actions["Hold"]).PressImpl(false);
                        }
                    }
                }

                values["Hold"] = PXCache.NotSetValue;
            }

            return base.ExecuteUpdate(viewName, keys, values, parameters);
        }

        public PXAction<CAAdj> flow;
		[PXUIField(DisplayName = "Flow")]
		protected virtual IEnumerable Flow(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<CAAdj> action;
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
			var result = new List<CAAdj>();
			if (actionName != null)
			{
				PXAction run = this.Actions[actionName];
				if (run != null)
					foreach (var item in action.Press(adapter)) ;
			}
			if (refresh)
			{
				foreach (CAAdj order in adapter.Get<CAAdj>())
					result.Add(CAAdjRecords.Search<CAAdj.adjRefNbr>(order.AdjRefNbr));
			}
			else
			{
				foreach (CAAdj order in adapter.Get<CAAdj>())
					result.Add(order);
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
		public PXAction<CAAdj> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { 1, 2 }, new string[] { "View Batch", "Activities" })]
			int? inquiryID
			)
		{
			switch (inquiryID)
			{
				case 1:
					ViewBatch();
					break;
				case 2:
					if (CAAdjRecords.Current != null)
						this.Activity.ButtonViewAllActivities.PressButton();
					break;
			}
			return adapter.Get();
		}

		public PXAction<CAAdj> viewBatch;
		[PXUIField(DisplayName = "View Batch", MapEnableRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.InquiryF)]
		public virtual void ViewBatch()
		{
			string BatchNbr = (string)this.CAAdjRecords.GetValueExt<CAAdj.tranID_CATran_batchNbr>(this.CAAdjRecords.Current);
			if (BatchNbr != null)
			{
				JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
				graph.Clear();
				Batch newBatch = new Batch();
				graph.BatchModule.Current = PXSelect<GL.Batch,
						Where<GL.Batch.module, Equal<GL.BatchModule.moduleCA>,
						And<GL.Batch.batchNbr, Equal<Required<GL.Batch.batchNbr>>>>>
						.Select(this, BatchNbr);
				throw new PXRedirectRequiredException(graph, "Batch Record");
			}
		}

		public PXAction<CAAdj> Release;
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update, Visible = false)]
		[PXProcessButton]
		public virtual IEnumerable release(PXAdapter adapter)
		{
			if (PXLongOperation.Exists(UID))
			{
				throw new ApplicationException(GL.Messages.PrevOperationNotCompleteYet);
			}
			Save.Press();

			CAAdj adj = CAAdjRecords.Current;
			List<CARegister> registerList = new List<CARegister>();
			registerList.Add(CATrxRelease.CARegister(adj));
			PXLongOperation.StartOperation(this, () => CATrxRelease.GroupRelease(registerList, false));
			List<CAAdj> ret = new List<CAAdj>();
			ret.Add(adj);
			return ret;
		}
		protected bool reversingContext;
		public PXAction<CAAdj> Reverse;
		[PXUIField(DisplayName = "Reverse", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable reverse(PXAdapter adapter)
		{
			CAAdj current = CAAdjRecords.Current;
			if (current.Released != true)
				return adapter.Get();

			CAAdj adj =(CAAdj)CAAdjRecords.Cache.CreateCopy(CAAdjRecords.Current);
			adj.AdjRefNbr = null;
			adj.Status = null;
			adj.Approved = null;
			adj.Hold = null;
			adj.Released = null;
			adj.ClearDate = null;
			adj.Cleared = null;
			adj.TranID = null;
			adj.NoteID = null;
			adj.CurySplitTotal = null;
			adj.CuryVatExemptTotal = null;
			adj.CuryVatTaxableTotal = null;
			adj.SplitTotal = null;
			adj.VatExemptTotal = null;
			adj.VatTaxableTotal = null;
			adj.CuryTaxRoundDiff *= -1;
			adj.CuryControlAmt *= -1;
			adj.CuryTaxAmt *= -1;
			adj.CuryTaxTotal *= -1;
			adj.CuryTranAmt *= -1;
			adj.TaxRoundDiff *= -1;
			adj.TaxAmt *= -1;
			adj.ControlAmt *= -1;
			adj.TaxTotal *= -1;
			adj.TranAmt *= -1;
			List<Tuple<CASplit,CASplit>> splits = new List<Tuple<CASplit,CASplit>>();
			foreach (CASplit split in CASplitRecords.Select())
			{
				CASplit newSplit = (CASplit)CASplitRecords.Cache.CreateCopy(split);
				newSplit.AdjRefNbr = null;
				newSplit.NoteID = null;
				newSplit.CuryTranAmt *= -1;
				newSplit.CuryUnitPrice *= -1;
				newSplit.CuryTaxAmt *= -1;
				newSplit.CuryTaxableAmt *= -1;
				newSplit.TranAmt *= -1;
				newSplit.UnitPrice *= -1;
				newSplit.TaxAmt *= -1;
				newSplit.TaxableAmt *= -1;
				splits.Add(new Tuple<CASplit, CASplit>(split, newSplit));
			}
			List<CATaxTran> taxes = new List<CATaxTran>();
			foreach (CATaxTran taxTran in Taxes.Select())
			{
				CATaxTran newTaxTran = new CATaxTran();
				newTaxTran.AccountID = taxTran.AccountID;
				newTaxTran.BranchID = taxTran.BranchID;
				newTaxTran.FinPeriodID = taxTran.FinPeriodID;
				newTaxTran.SubID = taxTran.SubID;
				newTaxTran.TaxBucketID = taxTran.TaxBucketID;
				newTaxTran.TaxID = taxTran.TaxID;
				newTaxTran.TaxType = taxTran.TaxType;
				newTaxTran.TaxZoneID = taxTran.TaxZoneID;
				newTaxTran.TranDate = taxTran.TranDate;
				newTaxTran.VendorID = taxTran.VendorID;
				newTaxTran.CuryID = taxTran.CuryID;
				newTaxTran.Description = taxTran.Description;
				newTaxTran.NonDeductibleTaxRate = taxTran.NonDeductibleTaxRate;
				newTaxTran.TaxRate = taxTran.TaxRate;
				newTaxTran.CuryTaxableAmt = -taxTran.CuryTaxableAmt;
				newTaxTran.CuryTaxAmt = -taxTran.CuryTaxAmt;
				newTaxTran.CuryExpenseAmt = -taxTran.CuryExpenseAmt;
				newTaxTran.TaxableAmt = -taxTran.TaxableAmt;
				newTaxTran.TaxAmt = -taxTran.TaxAmt;
				newTaxTran.ExpenseAmt = -taxTran.ExpenseAmt;

				taxes.Add(newTaxTran);
			}

			Clear();
			reversingContext = true;
			CAAdj insertedAdj = CAAdjRecords.Insert(adj);
			PXNoteAttribute.CopyNoteAndFiles(CAAdjRecords.Cache, current, CAAdjRecords.Cache, insertedAdj);
			foreach (Tuple<CASplit, CASplit> pair in splits)
			{
				CASplit newSplit = pair.Item2;
				newSplit = CASplitRecords.Insert(newSplit);
				PXNoteAttribute.CopyNoteAndFiles(CASplitRecords.Cache, pair.Item1, CASplitRecords.Cache, newSplit);
			}
			reversingContext = false;
			foreach (CATaxTran newTaxTran in taxes)
			{
				Taxes.Insert(newTaxTran);
			}
			//We should reenter totals depending on taxes as TaxAttribute does not recalculate them if externalCall==false
			CAAdjRecords.Cache.SetValue<CAAdj.taxRoundDiff>(insertedAdj, adj.TaxRoundDiff);
			CAAdjRecords.Cache.SetValue<CAAdj.curyTaxRoundDiff>(insertedAdj, adj.CuryTaxRoundDiff);
			CAAdjRecords.Cache.SetValue<CAAdj.taxTotal>(insertedAdj, adj.TaxAmt);
			CAAdjRecords.Cache.SetValue<CAAdj.curyTaxTotal>(insertedAdj, adj.CuryTaxAmt);
			CAAdjRecords.Cache.SetValue<CAAdj.tranAmt>(insertedAdj, adj.TranAmt);
			CAAdjRecords.Cache.SetValue<CAAdj.curyTranAmt>(insertedAdj, adj.CuryTranAmt);
			List<CAAdj> ret = new List<CAAdj> {insertedAdj};
			return ret;
		}

		#endregion

		#region Selects
		[PXViewName(Messages.CashTransactions)]
		public PXSelect<CAAdj, Where<CAAdj.draft, Equal<False>>> CAAdjRecords; //, Where<CAAdj.adjTranType, Equal<CAAPARTranType.cAAdjustment>>
		public PXSelect<CAAdj, Where<CAAdj.adjRefNbr, Equal<Current<CAAdj.adjRefNbr>>>> CurrentDocument;
		[PXImport(typeof(CAAdj))]
		public PXSelect<CASplit, Where<CASplit.adjRefNbr, Equal<Current<CAAdj.adjRefNbr>>,
															 And<CASplit.adjTranType, Equal<Current<CAAdj.adjTranType>>>>> CASplitRecords;
		public PXSelect<CATax, Where<CATax.adjTranType, Equal<Current<CAAdj.adjTranType>>, And<CATax.adjRefNbr, Equal<Current<CAAdj.adjRefNbr>>>>, OrderBy<Asc<CATax.adjTranType, Asc<CATax.adjRefNbr, Asc<CATax.taxID>>>>> Tax_Rows;
		public PXSelectJoin<CATaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<CATaxTran.taxID>>>, Where<CATaxTran.module, Equal<BatchModule.moduleCA>, And<CATaxTran.tranType, Equal<Current<CAAdj.adjTranType>>, And<CATaxTran.refNbr, Equal<Current<CAAdj.adjRefNbr>>>>>> Taxes;
		public PXSelectJoin<CATaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<CATaxTran.taxID>>>, Where<CATaxTran.module, Equal<BatchModule.moduleCA>, And<CATaxTran.tranType, Equal<Current<CAAdj.adjTranType>>, And<CATaxTran.refNbr, Equal<Current<CAAdj.adjRefNbr>>, 
			And<Tax.taxType, Equal<CSTaxType.use>>>>>> UseTaxes;

		public PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<CAAdj.cashAccountID>>>> cashAccount;
		public PXSelect<CASetupApproval> SetupApproval;
        [PXViewName(Messages.Approval)]
		public EPApprovalAutomation<CAAdj, CAAdj.approved, CAAdj.rejected, CAAdj.hold, CASetupApproval> Approval;

		//public PXSelect<CurrencyInfo> currencyinfo;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CAAdj.curyInfoID>>>> currencyinfo;
		public ToggleCurrency<CAAdj> CurrencyView;
		public PXSelect<GLVoucher, Where<True, Equal<False>>> Voucher;

		public PXSetup<CASetup> casetup;
		public PXSetup<GLSetup> glsetup;
		public PXSelect<CATran> catran;
		public PXSelect<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<CAAdj.taxZoneID>>>> taxzone;
		public CMSetupSelect CMSetup;
		public PXSetupOptional<TXAvalaraSetup> avalaraSetup; 

		public CRActivityList<CAAdj>
			Activity;

		#endregion

		#region EPApproval Cahce Attached
		[PXDBDate()]
		[PXDefault(typeof(CAAdj.tranDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(Search<EPEmployee.bAccountID,
			Where<EPEmployee.userID, Equal<Current<CAAdj.ownerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(CAAdj.tranDesc), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		[CurrencyInfo(typeof(CAAdj.curyInfoID))]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(CAAdj.curyTranAmt), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDBDecimal(4)]
		[PXDefault(typeof(CAAdj.tranAmt), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region Functions
		public virtual void updateAmountPrice(CASplit oldSplit, CASplit newSplit)
		{
			if (newSplit != null)
			{
				bool priceChanged = ((oldSplit.CuryUnitPrice ?? decimal.Zero) != (newSplit.CuryUnitPrice ?? decimal.Zero));
				bool amtChanged = ((oldSplit.CuryTranAmt ?? decimal.Zero) != (newSplit.CuryTranAmt ?? decimal.Zero));
				bool qtyChanged = ((oldSplit.Qty ?? decimal.Zero) != (newSplit.Qty ?? decimal.Zero));

				if (amtChanged)
				{
					if (newSplit.Qty != null && newSplit.Qty != (decimal)0.0)
					{
						newSplit.CuryUnitPrice = newSplit.CuryTranAmt / newSplit.Qty;
					}
					else
					{
						newSplit.CuryUnitPrice = newSplit.CuryTranAmt;
						newSplit.Qty = (decimal)1.0;
					}
				}
				else if (priceChanged || qtyChanged)
					newSplit.CuryTranAmt = newSplit.Qty * newSplit.CuryUnitPrice;
			}
		}
		#endregion

		#region CATran Envents
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Number", Enabled = false)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleCA>>>))]
		public virtual void CATran_BatchNbr_CacheAttached(PXCache sender)
		{
		}

		protected virtual void CATran_BatchNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_ReferenceID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		#endregion

		#region CAAdj Events
		protected virtual void CAAdj_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			CAAdj adj = (CAAdj)e.Row;

			if (casetup.Current.RequireControlTotal != true)
			{
				sender.SetValue<CAAdj.curyControlAmt>(adj, adj.CuryTranAmt);
			}
			else
			{
				if (adj.Hold != true)
					if (adj.CuryControlAmt != adj.CuryTranAmt)
					{
						sender.RaiseExceptionHandling<CAAdj.curyControlAmt>(adj, adj.CuryControlAmt, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else
					{
						sender.RaiseExceptionHandling<CAAdj.curyControlAmt>(adj, adj.CuryControlAmt, null);
					}
			}

			bool checkControlTaxTotal = casetup.Current.RequireControlTaxTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>();

			if (adj.CuryTaxTotal != adj.CuryTaxAmt && adj.Hold != true && checkControlTaxTotal)
			{
				sender.RaiseExceptionHandling<CAAdj.curyTaxAmt>(adj, adj.CuryTaxAmt, new PXSetPropertyException(AP.Messages.TaxTotalAmountDoesntMatch));
			}
			else
			{
				if (checkControlTaxTotal)
				{
					sender.RaiseExceptionHandling<CAAdj.curyTaxAmt>(adj, null, null);
				}
				else
				{
					sender.SetValueExt<CAAdj.curyTaxAmt>(adj, adj.CuryTaxTotal != null && adj.CuryTaxTotal != 0 ? adj.CuryTaxTotal : 0m);
				}
			}

			sender.RaiseExceptionHandling<CAAdj.curyTaxRoundDiff>(adj, null, null);

			if (adj.Hold != true && adj.Released != true && adj.TaxRoundDiff != 0)
			{
				if (checkControlTaxTotal)
				{
					if (Math.Abs(adj.TaxRoundDiff.Value) > Math.Abs(glsetup.Current.RoundingLimit.Value))
					{
						sender.RaiseExceptionHandling<CAAdj.curyTaxRoundDiff>(adj, adj.CuryTaxRoundDiff,
							new PXSetPropertyException(AP.Messages.RoundingAmountTooBig, currencyinfo.Current.BaseCuryID, PXDBQuantityAttribute.Round(adj.TaxRoundDiff),
								PXDBQuantityAttribute.Round(glsetup.Current.RoundingLimit)));
					}
				}
				else
				{
					if (!PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
					{
						sender.RaiseExceptionHandling<CAAdj.curyTaxRoundDiff>(adj, adj.CuryTaxRoundDiff,
							new PXSetPropertyException(AP.Messages.CannotEditTaxAmtWOFeature));
					}
					else
					{
						sender.RaiseExceptionHandling<CAAdj.curyTaxRoundDiff>(adj, adj.CuryTaxRoundDiff,
							new PXSetPropertyException(Messages.CannotEditTaxAmtWOCASetup));
					}
				}
			}
		}

		protected virtual void CAAdj_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CAAdj adj = e.Row as CAAdj;
			if (adj == null) return;
			CMSetup cmsetup = CMSetup.Current;

			if (taxzone.Current != null && adj.TaxZoneID != taxzone.Current.TaxZoneID)
			{
				taxzone.Current = null;
			}
			bool adjNotReleased = (adj.Released != true);

			sender.AllowUpdate = adjNotReleased;
			sender.AllowDelete = adjNotReleased;

			CASplitRecords.Cache.AllowInsert = adjNotReleased;
			CASplitRecords.Cache.AllowUpdate = adjNotReleased;
			CASplitRecords.Cache.AllowDelete = adjNotReleased;

			PXUIFieldAttribute.SetEnabled(sender, adj, false);
			PXUIFieldAttribute.SetEnabled<CAAdj.adjRefNbr>(sender, adj, true);

			CashAccount cashaccount = (CashAccount)PXSelectorAttribute.Select<CAAdj.cashAccountID>(sender, adj);
			bool requireControlTotal = (bool)casetup.Current.RequireControlTotal;
			bool clearEnabled = (adj.Released != true) && (cashaccount != null) && (cashaccount.Reconcile == true);
            bool hasNoDetailRecords = !this.CASplitRecords.Any();
            
			if (adjNotReleased)
			{
				PXUIFieldAttribute.SetEnabled<CAAdj.hold>(sender, adj, adjNotReleased);
                PXUIFieldAttribute.SetEnabled<CAAdj.cashAccountID>(sender, adj, hasNoDetailRecords);
				PXUIFieldAttribute.SetEnabled<CAAdj.entryTypeID>(sender, adj, hasNoDetailRecords);
                PXUIFieldAttribute.SetEnabled<CAAdj.extRefNbr>(sender, adj);
				PXUIFieldAttribute.SetEnabled<CAAdj.tranDate>(sender, adj);
				PXUIFieldAttribute.SetEnabled<CAAdj.finPeriodID>(sender, adj);
				PXUIFieldAttribute.SetEnabled<CAAdj.tranDesc>(sender, adj);
				PXUIFieldAttribute.SetEnabled<CAAdj.taxZoneID>(sender, adj);
				PXUIFieldAttribute.SetEnabled<CAAdj.curyControlAmt>(sender, adj, requireControlTotal);
				PXUIFieldAttribute.SetEnabled<CAAdj.cleared>(sender, adj, clearEnabled);
				PXUIFieldAttribute.SetEnabled<CAAdj.clearDate>(sender, adj, clearEnabled && (adj.Cleared == true));
				CAEntryType entryType = PXSelect<CAEntryType, Where<CAEntryType.entryTypeId, Equal<Required<CAEntryType.entryTypeId>>>>.Select(this, adj.EntryTypeID);
				bool isReclassyPaymentEntry = (entryType != null && entryType.UseToReclassifyPayments == true);
				PXUIFieldAttribute.SetEnabled<CASplit.inventoryID>(this.CASplitRecords.Cache, null, !isReclassyPaymentEntry);
			}

			Release.SetEnabled((adj.AdjTranType == CAAPARTranType.CAAdjustment) && adjNotReleased && (adj.Hold != true));
			Reverse.SetEnabled(adj.Released == true && adj.AdjTranType == CATranType.CAAdjustment);
			PXUIFieldAttribute.SetVisible<CAAdj.curyControlAmt>(sender, null, requireControlTotal);
			PXUIFieldAttribute.SetVisible<CAAdj.curyID>(sender, adj, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			PXUIFieldAttribute.SetVisible<CAAdj.approved>(sender, null, casetup.Current.RequestApproval == true);
			PXUIFieldAttribute.SetRequired<CAAdj.curyControlAmt>(sender, requireControlTotal);

			if (IsExternalTax == true && ((CAAdj)e.Row).IsTaxValid != true)
				PXUIFieldAttribute.SetWarning<CAAdj.curyTaxTotal>(sender, e.Row, AR.Messages.TaxIsNotUptodate);
            bool isReclassification = adj.PaymentsReclassification == true;
            PXUIFieldAttribute.SetVisible<CASplit.cashAccountID>(this.CASplitRecords.Cache, null, isReclassification);
			PXUIFieldAttribute.SetEnabled<CASplit.cashAccountID>(this.CASplitRecords.Cache, null, isReclassification);

            PXDefaultAttribute.SetPersistingCheck<CAAdj.extRefNbr>(sender, adj,
                casetup.Current.RequireExtRefNbr == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            PXUIFieldAttribute.SetRequired<CAAdj.extRefNbr>(sender, casetup.Current.RequireExtRefNbr == true);

			bool RequireTaxControlTotal = PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>() &&
			                              (casetup.Current.RequireControlTaxTotal == true);

			PXUIFieldAttribute.SetVisible<CAAdj.curyTaxAmt>(sender, adj, RequireTaxControlTotal);
			PXUIFieldAttribute.SetEnabled<CAAdj.curyTaxAmt>(sender, adj, RequireTaxControlTotal);
			PXUIFieldAttribute.SetRequired<CAAdj.curyTaxAmt>(sender, RequireTaxControlTotal);

			PXUIFieldAttribute.SetEnabled<CAAdj.taxCalcMode>(sender, adj, adj.Released != true);

			bool showRoundingDiff = adj.CuryTaxRoundDiff != 0;
			PXUIFieldAttribute.SetVisible<CAAdj.curyTaxRoundDiff>(sender, adj, showRoundingDiff);

			if (UseTaxes.Select().Count != 0)
			{
				sender.RaiseExceptionHandling<CAAdj.curyTaxTotal>(adj, adj.CuryTaxTotal, new PXSetPropertyException(TX.Messages.UseTaxExcludedFromTotals, PXErrorLevel.Warning));
			}

			PXUIFieldAttribute.SetVisible<CAAdj.usesManualVAT>(sender, adj, adj.UsesManualVAT == true);
			Taxes.Cache.AllowDelete = adj.UsesManualVAT != true;
			Taxes.Cache.AllowInsert = adj.UsesManualVAT != true;
			Taxes.Cache.AllowUpdate = adj.UsesManualVAT != true;
		}
		protected virtual void CAAdj_EntryTypeId_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CAAdj adj = e.Row as CAAdj;
		    if (adj != null)
		    {
		        CAEntryType entryType = PXSelect<CAEntryType,
		            Where<CAEntryType.entryTypeId, Equal<Required<CAEntryType.entryTypeId>>>>.
		            Select(this, adj.EntryTypeID);
		        if (entryType != null)
		        {
                    adj.DrCr = entryType.DrCr;
                    adj.PaymentsReclassification = entryType.UseToReclassifyPayments == true;
                    if (entryType.UseToReclassifyPayments == true && adj.CashAccountID.HasValue)
                    {
                        CashAccount availableAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID, NotEqual<Required<CashAccount.cashAccountID>>,
                            And<CashAccount.curyID, Equal<Required<CashAccount.curyID>>>>>.SelectWindowed(sender.Graph, 0, 1, adj.CashAccountID, adj.CuryID);
                        if (availableAccount == null)
                        {
                            sender.RaiseExceptionHandling<CAAdj.entryTypeID>(adj, null, new PXSetPropertyException(Messages.EntryTypeRequiresCashAccountButNoOneIsConfigured, PXErrorLevel.Warning, adj.CuryID));
                        }
                    }		            
		        }
				sender.SetDefaultExt<CAAdj.taxCalcMode>(adj);
				sender.SetDefaultExt<CAAdj.taxZoneID>(adj);
		    }
		}
		protected virtual void CAAdj_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = casetup.Current.RequestApproval == true || casetup.Current.HoldEntry == true;
		}
		protected virtual void CAAdj_Status_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = casetup.Current.RequestApproval == true || casetup.Current.HoldEntry == true
										? CATransferStatus.Hold
										: CATransferStatus.Balanced;
		}
		protected virtual void CAAdj_Cleared_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CAAdj adj = e.Row as CAAdj;
			if (adj.Cleared == true)
			{
				if (adj.ClearDate == null)
				{
					adj.ClearDate = adj.TranDate;
				}
			}
			else
			{
				adj.ClearDate = null;
			}
		}
		protected virtual void CAAdj_TranDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CAAdj adj = e.Row as CAAdj;
			if (adj == null) return;

			CurrencyInfoAttribute.SetEffectiveDate<CAAdj.tranDate>(sender, e);
		}

		protected virtual void CAAdj_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CAAdj adj = (CAAdj)e.Row;

			adj.Cleared = false;
			adj.ClearDate = null;

			if (cashAccount.Current == null || cashAccount.Current.CashAccountID != adj.CashAccountID)
			{
				cashAccount.Current = (CashAccount)PXSelectorAttribute.Select<CAAdj.cashAccountID>(sender, adj);
			}
			if (cashAccount.Current.Reconcile != true)
			{
				adj.Cleared = true;
				adj.ClearDate = adj.TranDate;
			}

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<CAAdj.curyID>(adj) == null)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<CAAdj.curyInfoID>(sender, adj);

					string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<CAAdj.tranDate>(adj, adj.TranDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						adj.CuryID = info.CuryID;
					}
				}
			}
            adj.BranchID = cashAccount.Current.BranchID;
			sender.SetDefaultExt<CAAdj.entryTypeID>(e.Row);
		}

		protected virtual void CAAdj_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CAAdj adj = (CAAdj)e.Row;

			if (adj.CashAccountID == null)
			{
				sender.RaiseExceptionHandling<CAAdj.cashAccountID>(adj, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CAAdj.cashAccountID).Name));
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				switch (this.Accessinfo.ScreenID)
				{
					case "CA.30.40.00":
						if (adj.Status == CATransferStatus.Released)
						{
							e.Cancel = true;
							throw new PXException(Messages.ReleasedDocCanNotBeDel);
						}
						if (adj.AdjTranType == CATranType.CATransferExp)
						{
							e.Cancel = true;
							throw new PXException(Messages.TransferDocCanNotBeDel);
						}
						break;
				}
			}
		}


		protected virtual void CAAdj_Approved_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = casetup.Current != null ? casetup.Current.RequestApproval != true : true;
		}

		protected virtual void CAAdj_Rejected_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CAAdj doc = (CAAdj)e.Row;
			if (doc.Rejected == true)
			{
				doc.Approved = false;
				doc.Hold = true;
				doc.Status = CATransferStatus.Rejected;
				cache.RaiseFieldUpdated<CAAdj.hold>(e.Row, null);
				PXUIFieldAttribute.SetEnabled<CAAdj.hold>(cache, e.Row, true);
			}
		}

		protected virtual void CAAdj_TranID_CATran_BatchNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row == null || e.IsAltered)
			{
				string ViewName = null;
				PXCache cache = sender.Graph.Caches[typeof(CATran)];
				PXFieldState state = cache.GetStateExt<CATran.batchNbr>(null) as PXFieldState;
				if (state != null)
				{
					ViewName = state.ViewName;
				}

				e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, null, false, false, 0, 0, null, null, null, null, null, null, PXErrorLevel.Undefined, false, true, true, PXUIVisibility.Visible, ViewName, null, null);
			}
		}

		#endregion

		#region CASplit Events

		protected virtual void CASplit_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			CAAdj adj = CAAdjRecords.Current;
			if ((adj == null) || (adj.EntryTypeID == null) || (split == null)) return;
            e.NewValue = GetDefaultAccountValues(this, adj.CashAccountID, adj.EntryTypeID).AccountID;
            e.Cancel = e.NewValue != null; 
		}

		protected virtual void CASplit_AccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CASplit row = e.Row as CASplit;
			if (row == null) return;
			Account glAccount = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, row.AccountID);
			if (glAccount != null && glAccount.IsCashAccount == true)
			{
				CashAccount cashAcct = null;
				if (row.SubID != null && row.BranchID != null)
				{
					cashAcct = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CABankTranDetail.accountID>>,
						  And<CashAccount.subID, Equal<Required<CABankTranDetail.subID>>,
						  And<CashAccount.branchID, Equal<Required<CABankTranDetail.branchID>>>>>>.Select(this, glAccount.AccountID, row.SubID, row.BranchID);
				}
				else if (row.BranchID != null)
				{
					cashAcct = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CABankTranDetail.accountID>>,
						  And<CashAccount.branchID, Equal<Required<CABankTranDetail.branchID>>>>>.Select(this, glAccount.AccountID, row.BranchID);
				}
				else if (row.SubID != null)
				{
					cashAcct = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CABankTranDetail.accountID>>,
						  And<CashAccount.subID, Equal<Required<CABankTranDetail.subID>>>>>.Select(this, glAccount.AccountID, row.SubID);
				}
				if (cashAcct == null)
				{
					cashAcct = PXSelect<CashAccount,
					   Where<CashAccount.accountID, Equal<Required<CashAccount.accountID>>>>.Select(this, glAccount.AccountID);
				}
				if (cashAccount != null)
				{
					cache.SetValueExt<CASplit.branchID>(row, cashAcct.BranchID);
					cache.SetValueExt<CASplit.subID>(row, cashAcct.SubID);
				}
			}
		}

		protected virtual void CASplit_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			CAAdj adj = CAAdjRecords.Current;
			if ((adj == null) || (adj.EntryTypeID == null) || (split == null)) return;
		    e.NewValue = GetDefaultAccountValues(this, adj.CashAccountID, adj.EntryTypeID).SubID;
		    e.Cancel = e.NewValue != null; 
		}

        protected virtual void CASplit_BranchID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            CAAdj adj = CAAdjRecords.Current;
            CASplit split = e.Row as CASplit;
            if ((adj == null) || (adj.EntryTypeID == null) || (split == null)) return;
	        e.NewValue = GetDefaultAccountValues(this, adj.CashAccountID, adj.EntryTypeID).BranchID;
			e.Cancel = e.NewValue != null; 

        }

        private CASplit GetDefaultAccountValues(PXGraph graph, int? CashAccountID, string EntryTypeID)
        {
            CAEntryType entryType = PXSelect<CAEntryType,
                    Where<CAEntryType.entryTypeId, Equal<Required<CAEntryType.entryTypeId>>>>.Select(graph, EntryTypeID);
            CashAccountETDetail acctSettings = PXSelect<CashAccountETDetail, Where<CashAccountETDetail.accountID, Equal<Required<CashAccountETDetail.accountID>>,
                                        And<CashAccountETDetail.entryTypeID, Equal<Required<CashAccountETDetail.entryTypeID>>>>>.Select(graph, CashAccountID, EntryTypeID);
            CASplit result = new CASplit();
            if (acctSettings != null && (acctSettings.OffsetAccountID.HasValue || acctSettings.OffsetSubID.HasValue || acctSettings.OffsetBranchID.HasValue))
	        {
		        result.AccountID = acctSettings.OffsetAccountID;
		        result.SubID = acctSettings.OffsetSubID;
		        result.BranchID = acctSettings.OffsetBranchID;
	        }
	        if (entryType != null)
            {
                result.AccountID = result.AccountID ?? entryType.AccountID;
                result.SubID = result.SubID ?? entryType.SubID;
                result.BranchID = result.BranchID ?? entryType.BranchID;
            }
            return result;
        }

		protected virtual void CASplit_CashAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			if (split != null && split.CashAccountID == null)
			{
				CashAccount CashAccount = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CASplit.accountID>>,
						And<CashAccount.subID, Equal<Required<CASplit.subID>>, 
						And<CashAccount.branchID, Equal<Required<CASplit.branchID>>>>>>.Select(sender.Graph, split.AccountID, split.SubID, split.BranchID);
				if (CashAccount != null)
				{
					e.NewValue = CashAccount.CashAccountCD;
				}
			}
		}
		protected virtual void CASplit_CashAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            CASplit split = e.Row as CASplit;
            if (split == null) return;
            CashAccount CashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID,
                Equal<Required<CashAccount.cashAccountID>>>>.Select(sender.Graph, e.NewValue);
            if (CashAccount != null)
            {
                if (CashAccount.AccountID == CAAdjRecords.Current.CashAccountID)
                {
                    e.NewValue = CashAccount.CashAccountCD;
                    throw new PXSetPropertyException(Messages.OffsetAccountMayNotBeTheSameAsCurrentAccount, PXErrorLevel.Error);
                }
            }
        }
		protected virtual void CASplit_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			if (split == null) return;
			if (split.CashAccountID != (int?)e.OldValue)
			{
				if (split.CashAccountID != null)
				{
					CashAccount offsetCashAcct = PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID,
						Equal<Required<CashAccount.cashAccountID>>>>.Select(sender.Graph, split.CashAccountID);
					if (offsetCashAcct != null)
					{
						sender.SetValue<CASplit.accountID>(split, offsetCashAcct.AccountID);
						sender.SetValue<CASplit.subID>(split, offsetCashAcct.SubID);
						sender.SetValue<CASplit.branchID>(split, offsetCashAcct.BranchID);
					}
				}
				else
				{
					sender.SetValue<CASplit.accountID>(split, null);
					sender.SetValue<CASplit.subID>(split, null);
					sender.SetValue<CASplit.branchID>(split, null);
				}
			}
		}
		protected virtual void CASplit_TranDesc_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;

			CAAdj adj = CAAdjRecords.Current;

			if ((adj != null) && (adj.EntryTypeID != null))
			{
				CAEntryType entryType = PXSelect<CAEntryType,
					Where<CAEntryType.entryTypeId, Equal<Required<CAEntryType.entryTypeId>>>>.
					Select(this, adj.EntryTypeID);

				if (entryType != null)
				{
					e.NewValue = entryType.Descr;
				}
			}
		}

		protected virtual void CASplit_CuryTranAmt_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			if (split == null) return;
			if (casetup.Current.RequireControlTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
			{
				CAAdj adj = CAAdjRecords.Current;
				if (adj == null) return;
				decimal? newVal = 0;
				if (String.IsNullOrEmpty(split.TaxCategoryID))
				{
					sender.SetDefaultExt<CASplit.taxCategoryID>(split);
				}
				newVal = TaxAttribute.CalcResidualAmt(sender, split, adj.TaxZoneID, split.TaxCategoryID, adj.TranDate.Value,
					adj.TaxCalcMode, adj.CuryControlAmt.Value, adj.CurySplitTotal.Value, adj.CuryTaxTotal.Value);
				e.NewValue = Math.Sign(newVal.Value) == Math.Sign(adj.CuryControlAmt.Value) ? newVal : 0;
				e.Cancel = true;
			}
		}

		protected virtual void CASplit_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (reversingContext) return;
			CASplit split = e.Row as CASplit;
			if (split == null) return;

			if (TaxAttribute.GetTaxCalc<CASplit.taxCategoryID>(sender, split) == TaxCalc.Calc &&
				taxzone.Current != null &&
				!string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) &&
				split.InventoryID == null)
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
			}
		}

		protected virtual void CASplit_TaxCategoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (reversingContext) return;
			CASplit row = (CASplit)e.Row;
			if (row == null) return;
			CAAdj doc = CAAdjRecords.Current;
			if (!this.IsCopyPasteContext && casetup.Current.RequireControlTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>()
				&& row.CuryTranAmt.HasValue && row.CuryTranAmt.Value != 0 && row.Qty == (decimal)1.0 && doc.TaxCalcMode == VendorClass.taxCalcMode.Net)
			{
				PXResultset<CATax> taxes = PXSelect<CATax, Where<CATax.adjTranType, Equal<Required<CATax.adjTranType>>, And<CATax.adjRefNbr, Equal<Required<CATax.adjRefNbr>>,
					And<CATax.lineNbr, Equal<Required<CATax.lineNbr>>>>>>.Select(this, row.AdjTranType, row.AdjRefNbr, row.LineNbr);
				decimal curyTaxSum = 0;
				foreach (CATax tax in taxes)
				{
					curyTaxSum += tax.CuryTaxAmt.Value;
				}
				decimal? taxableAmount = TaxAttribute.CalcTaxableFromTotalAmount(sender, row, doc.TaxZoneID,
							row.TaxCategoryID, doc.TranDate.Value, row.CuryTranAmt.Value + curyTaxSum, false, GLTaxAttribute.TaxCalcLevelEnforcing.EnforceCalcOnItemAmount);
				sender.SetValueExt<CASplit.curyTranAmt>(row, taxableAmount);
			}
		}

		protected virtual void CASplit_InventoryId_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			CAAdj adj = CAAdjRecords.Current;

			if (split != null && split.InventoryID != null)
			{
				InventoryItem invItem = PXSelect<InventoryItem,
					Where<InventoryItem.inventoryID, Equal<Required<CASplit.inventoryID>>>>.
					Select(this, split.InventoryID);

				if (invItem != null && adj != null)
				{
					if (adj.DrCr == CADrCr.CADebit)
					{
						split.AccountID = invItem.SalesAcctID;
						split.SubID = invItem.SalesSubID;
					}
					else
					{
						split.AccountID = invItem.COGSAcctID;
						split.SubID = invItem.COGSSubID;
					}
				}

				sender.SetDefaultExt<CASplit.taxCategoryID>(split);
				sender.SetDefaultExt<CASplit.uOM>(split);
			}
		}

		protected virtual void CASplit_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CASplit row = (CASplit)e.Row;
			if (row == null) return;
		    bool isReclassification = this.CAAdjRecords.Current.PaymentsReclassification == true;
            PXUIFieldAttribute.SetEnabled<CASplit.accountID>(sender, row, !isReclassification);
            PXUIFieldAttribute.SetEnabled<CASplit.subID>(sender, row, !isReclassification);
            PXUIFieldAttribute.SetEnabled<CASplit.branchID>(sender, row, !isReclassification);
		}
        
		protected virtual void CASplit_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			CASplit oldSplit = e.Row as CASplit;
			CASplit newSplit = e.NewRow as CASplit;
			if (newSplit == null) return;
			updateAmountPrice(oldSplit, newSplit);

			if (newSplit.AccountID != null)
			{
				Account currentAcc = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, newSplit.AccountID);
				if (currentAcc.IsCashAccount == true)
				{
					CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CashAccount.accountID>>,
						And<CashAccount.subID, Equal<Required<CashAccount.subID>>,
							And<CashAccount.branchID, Equal<Required<CashAccount.branchID>>>>>>.Select(this, newSplit.AccountID, newSplit.SubID, newSplit.BranchID);
					if (cashAccount == null)
					{
						string branchCD = (string)PXSelectorAttribute.GetField(sender, newSplit, typeof(CASplit.branchID).Name, newSplit.BranchID, typeof(Branch.branchCD).Name);
						sender.RaiseExceptionHandling<CASplit.branchID>(newSplit, branchCD,
							new PXSetPropertyException(Messages.NoCashAccountForBranchAndSub, PXErrorLevel.Error));
						string subCD = (string)PXSelectorAttribute.GetField(sender, newSplit, typeof(CASplit.subID).Name, newSplit.SubID, typeof(Sub.subCD).Name);
						sender.RaiseExceptionHandling<CASplit.subID>(newSplit, subCD,
							new PXSetPropertyException(Messages.NoCashAccountForBranchAndSub, PXErrorLevel.Error));
						e.Cancel = true;
						return;
					}
				}
			}
		}

		protected virtual void CASplit_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			//if any of the fields that was saved in avalara has changed mark doc as TaxInvalid.
			if (CAAdjRecords.Current != null && IsExternalTax == true &&
				!sender.ObjectsEqual<CASplit.accountID, CASplit.inventoryID, CASplit.tranDesc, CASplit.tranAmt, CASplit.taxCategoryID>(e.Row, e.OldRow))
			{
				CAAdjRecords.Current.IsTaxValid = false;
				CAAdjRecords.Update(CAAdjRecords.Current);
			}
		}

		protected virtual void CASplit_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			CASplit newSplit = e.Row as CASplit;
			updateAmountPrice(new CASplit(), newSplit);
		}

		protected virtual void CASplit_Qty_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CASplit split = e.Row as CASplit;
			e.NewValue = (decimal)1.0;
		}
	
		#endregion

		#region CATaxTran Events
		protected virtual void CATaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			bool usesManualVAT = this.CurrentDocument.Current != null && this.CurrentDocument.Current.UsesManualVAT == true;
			PXUIFieldAttribute.SetEnabled<CATaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted && !usesManualVAT);
		}
		protected virtual void CATaxTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (reversingContext) e.Cancel = true;
		}
		protected virtual void CATaxTran_TaxType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && CAAdjRecords.Current != null)
			{
				if (CAAdjRecords.Current.DrCr == CADrCr.CACredit)
				{
					AP.PurchaseTax tax = PXSelect<AP.PurchaseTax, Where<AP.PurchaseTax.taxID, Equal<Required<AP.PurchaseTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.TranTaxType;
						e.Cancel = true;
					}
				}
				else
				{
					AR.SalesTax tax = PXSelect<AR.SalesTax, Where<AR.SalesTax.taxID, Equal<Required<AR.SalesTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.TranTaxType;
						e.Cancel = true;
					}
				}
			}
		}

		protected virtual void CATaxTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && CAAdjRecords.Current != null)
			{
				if (CAAdjRecords.Current.DrCr == CADrCr.CACredit)
				{
					AP.PurchaseTax tax = PXSelect<AP.PurchaseTax, Where<AP.PurchaseTax.taxID, Equal<Required<AP.PurchaseTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.HistTaxAcctID;
						e.Cancel = true;
					}
				}
				else
				{
					AR.SalesTax tax = PXSelect<AR.SalesTax, Where<AR.SalesTax.taxID, Equal<Required<AR.SalesTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.HistTaxAcctID;
						e.Cancel = true;
					}
				}
			}
		}

		protected virtual void CATaxTran_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row != null && CAAdjRecords.Current != null)
			{
				if (CAAdjRecords.Current.DrCr == CADrCr.CACredit)
				{
					AP.PurchaseTax tax = PXSelect<AP.PurchaseTax, Where<AP.PurchaseTax.taxID, Equal<Required<AP.PurchaseTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.HistTaxSubID;
						e.Cancel = true;
					}
				}
				else
				{
					AR.SalesTax tax = PXSelect<AR.SalesTax, Where<AR.SalesTax.taxID, Equal<Required<AR.SalesTax.taxID>>>>.Select(sender.Graph, ((CATaxTran)e.Row).TaxID);
					if (tax != null)
					{
						e.NewValue = tax.HistTaxSubID;
						e.Cancel = true;
					}
				}
			}
		}
		#endregion

		#region CurrencyInfo Events

		protected virtual void CurrencyInfo_ModuleCode_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = "GL";
			e.Cancel = true;
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CurrencyInfo currencyInfo = (CurrencyInfo)e.Row;
			if (currencyInfo == null || CAAdjRecords.Current == null || CAAdjRecords.Current.TranDate == null) return;
			e.NewValue = CAAdjRecords.Current.TranDate;
			e.Cancel = true;
		}

		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (cashAccount.Current != null && !string.IsNullOrEmpty(cashAccount.Current.CuryID))
				{
					e.NewValue = cashAccount.Current.CuryID;
					e.Cancel = true;
				}				
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (cashAccount.Current != null && !string.IsNullOrEmpty(cashAccount.Current.CuryRateTypeID))
				{
					e.NewValue = cashAccount.Current.CuryRateTypeID;
					e.Cancel = true;
				}
				else if (CMSetup.Current != null && !string.IsNullOrEmpty(CMSetup.Current.CARateTypeDflt))
				{
					e.NewValue = CMSetup.Current.CARateTypeDflt;
					e.Cancel = true;
				}
			}
		}
		/* May be need later
		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
				CurrencyInfo info = e.Row as CurrencyInfo;
				if (info != null)
				{
						bool curyenabled = !(info.IsReadOnly == true || (info.CuryID == info.BaseCuryID));

						if (cashAccount.Current != null && !(bool)cashAccount.Current.AllowOverrideRate)
						{
								curyenabled = false;
						}

						PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID> (sender, info, curyenabled);
						PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>    (sender, info, curyenabled);
						PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate> (sender, info, curyenabled);
						PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyenabled);
				}
		}
		*/

		#endregion

		public override void Persist()
		{
			base.Persist();

			if (CAAdjRecords.Current != null && IsExternalTax == true && CAAdjRecords.Current.IsTaxValid != true && !skipAvalaraCallOnSave)
			{
				PXLongOperation.StartOperation(this, delegate()
				{
					CAAdj doc = new CAAdj();
					doc.AdjRefNbr = CAAdjRecords.Current.RefNbr;
					CAExternalTaxCalc.Process(doc);
				});
			}
		}

		#region Avalara Tax

		protected bool IsExternalTax
		{
			get
			{
				if (CAAdjRecords.Current == null)
					return false;

				return AvalaraMaint.IsExternalTax(this, CAAdjRecords.Current.TaxZoneID);
			}
		}

		public virtual CAAdj CalculateAvalaraTax(CAAdj invoice)
		{
			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(this, service);

			AvaAddress.AddressSvc addressService = new AvaAddress.AddressSvc();
			AvalaraMaint.SetupService(this, addressService);

			GetTaxRequest getRequest = BuildGetTaxRequest(invoice);

			if (getRequest.Lines.Count == 0)
			{
				invoice.IsTaxValid = true;
				invoice.IsTaxSaved = false;
				invoice = CAAdjRecords.Update(invoice);
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
					invoice = CAAdjRecords.Update(invoice);
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

		protected virtual GetTaxRequest BuildGetTaxRequest(CAAdj invoice)
		{
			if (invoice == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CurrencyCode = invoice.CuryID;
			request.CustomerCode = "N/A";
			IAddressBase fromAddress = GetToAddress(invoice);
			IAddressBase toAddress = fromAddress;

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFrom);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetTo);

			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("CA.{0}", invoice.AdjRefNbr);
			request.DocDate = invoice.TranDate.GetValueOrDefault();
			request.LocationCode = GetAvalaraLocationCode(invoice);
			CRLocation branchLoc = GetBranchLocation(invoice);

			if (branchLoc != null)
			{
				request.CustomerUsageType = branchLoc.CAvalaraCustomerUsageType;
				request.ExemptionNo = branchLoc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.PurchaseInvoice;
			int mult = 1;

			if (invoice.DrCr == CADrCr.CADebit)
				request.DocType = DocumentType.SalesInvoice;
			else
				request.DocType = DocumentType.PurchaseInvoice;

			PXSelectBase<CASplit> select = new PXSelectJoin<CASplit,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<CASplit.inventoryID>>>,
				Where<CASplit.adjRefNbr, Equal<Current<CAAdj.adjRefNbr>>>,
				OrderBy<Asc<CASplit.adjRefNbr, Asc<CASplit.lineNbr>>>>(this);

			foreach (PXResult<CASplit, InventoryItem> res in select.View.SelectMultiBound(new object[] { invoice }))
			{
				CASplit tran = (CASplit)res;
				InventoryItem item = (InventoryItem)res;
				
				Line line = new Line();
				line.No = Convert.ToString(tran.LineNbr);
				line.Amount = mult * tran.CuryTranAmt.GetValueOrDefault();
				line.Description = tran.TranDesc;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Math.Abs(Convert.ToDouble(tran.Qty.GetValueOrDefault()));
				line.Discounted = request.Discount > 0;
				line.TaxCode = tran.TaxCategoryID;
				line.TaxIncluded = avalaraSetup.Current.IsInclusiveTax == true;

				request.Lines.Add(line);
			}

			return request;
		}

		protected bool skipAvalaraCallOnSave = false;
		protected virtual void ApplyAvalaraTax(CAAdj invoice, GetTaxResult result)
		{
			TaxZone taxZone = (TaxZone)taxzone.View.SelectSingleBound(new object[] { invoice });
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(this, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			//Clear all existing Tax transactions:
			foreach (PXResult<CATaxTran, Tax> res in Taxes.View.SelectMultiBound(new object[] { invoice }))
			{
				CATaxTran taxTran = (CATaxTran)res;
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

				CATaxTran tax = new CATaxTran();
				tax.Module = BatchModule.CA;
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
			
			bool requireControlTotal = casetup.Current.RequireControlTotal == true;
			if (invoice.Hold != true)
				casetup.Current.RequireControlTotal = false;

			try
			{
				invoice.CuryTaxTotal = Math.Abs(result.TotalTax);
				CAAdjRecords.Cache.SetValueExt<CAAdj.isTaxSaved>(invoice, true);
				CAAdjRecords.Update(invoice);
			}
			finally
			{
				casetup.Current.RequireControlTotal = requireControlTotal;
			}
		}

		protected virtual void CancelTax(CAAdj invoice, CancelCode code)
		{
			CancelTaxRequest request = new CancelTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, invoice.BranchID);
			request.CancelCode = code;
			request.DocCode = string.Format("CA.{0}", invoice.AdjRefNbr);

			if (invoice.DrCr == CADrCr.CADebit)
				request.DocType = DocumentType.SalesInvoice;
			else
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
				if (CAAdjRecords.Cache.GetStatus(invoice) == PXEntryStatus.Notchanged)
					CAAdjRecords.Cache.SetStatus(invoice, PXEntryStatus.Updated);
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

		protected virtual IAddressBase GetToAddress(CAAdj invoice)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, Address> res in select.Select(invoice.BranchID))
				return (Address)res;

			return null;
		}

		protected virtual CRLocation GetBranchLocation(CAAdj invoice)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(this);

			foreach (PXResult<Branch, BAccountR, CRLocation> res in select.Select(invoice.BranchID))
				return (CRLocation)res;

			return null;
		}

		protected virtual string GetAvalaraLocationCode(CAAdj order)
		{
			return null;
		}


		#endregion

	}
}
