using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;

namespace PX.Objects.IN
{
	public class INReceiptEntry : PXGraph<INReceiptEntry, INRegister>, PXImportAttribute.IPXPrepareItems
	{
		public PXSelect<INRegister, Where<INRegister.docType, Equal<INDocType.receipt>>> receipt;
		public PXSelect<INRegister, Where<INRegister.docType, Equal<Current<INRegister.docType>>, And<INRegister.refNbr, Equal<Current<INRegister.refNbr>>>>> CurrentDocument; 
        [PXImport(typeof(INRegister))]
		public PXSelect<INTran, Where<INTran.docType, Equal<Current<INRegister.docType>>, And<INTran.refNbr, Equal<Current<INRegister.refNbr>>>>> transactions;

		[PXCopyPasteHiddenView()]
		public PXSelect<INTranSplit, Where<INTranSplit.tranType, Equal<Current<INTran.tranType>>, And<INTranSplit.refNbr, Equal<Current<INTran.refNbr>>, And<INTranSplit.lineNbr, Equal<Current<INTran.lineNbr>>>>>> splits; // using Current<> is valid, PXSyncGridParams used in aspx
		public PXSetup<INSetup> insetup;

		public LSINTran lsselect;

        #region CacheAttached
        #region INTran
        [INUnit(typeof(INTran.inventoryID))]
        [UOMDefault(INUOMDefaulType.Purchase, typeof(INTran.inventoryID), typeof(INTran.siteID))]
        public virtual void INTran_UOM_CacheAttached(PXCache sender)
		{ 
		}
		#endregion
		#endregion

		public PXAction<INRegister> viewBatch;
		[PXUIField(DisplayName = "Review Batch", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			if (receipt.Current != null && !String.IsNullOrEmpty(receipt.Current.BatchNbr))
			{
				GL.JournalEntry graph = PXGraph.CreateInstance<GL.JournalEntry>();
				graph.BatchModule.Current = graph.BatchModule.Search<GL.Batch.batchNbr>(receipt.Current.BatchNbr, "IN");
				throw new PXRedirectRequiredException(graph, "Current batch record");
			}
			return adapter.Get();
		}

		public PXAction<INRegister> release;
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = receipt.Cache;
			List<INRegister> list = new List<INRegister>();
			foreach (INRegister indoc in adapter.Get<INRegister>())
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
			PXLongOperation.StartOperation(this, delegate() { INDocumentRelease.ReleaseDoc(list, false); });
			return list;
		}

        #region MyButtons (MMK)
        public PXAction<INRegister> report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report)]
        protected virtual IEnumerable Report(PXAdapter adapter)
        {
            return adapter.Get();
        }
        #endregion

		public PXAction<INRegister> iNEdit;
		[PXUIField(DisplayName = Messages.INEditDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable INEdit(PXAdapter adapter)
		{
			if (receipt.Current != null)
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["DocType"] = receipt.Current.DocType;
				parameters["RefNbr"] = receipt.Current.RefNbr;
				parameters["PeriodTo"] = null;
				parameters["PeriodFrom"] = null;
				throw new PXReportRequiredException(parameters, "IN611000", Messages.INEditDetails);
			}
			return adapter.Get();
		}

		public PXAction<INRegister> iNRegisterDetails;
		[PXUIField(DisplayName = Messages.INRegisterDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable INRegisterDetails(PXAdapter adapter)
		{
			if (receipt.Current != null)
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["PeriodID"] = (string)receipt.GetValueExt<INRegister.finPeriodID>(receipt.Current);
				parameters["DocType"] = receipt.Current.DocType;
				parameters["RefNbr"] = receipt.Current.RefNbr;
				throw new PXReportRequiredException(parameters, "IN614000", Messages.INRegisterDetails);
			}
			return adapter.Get();
		}

		public PXAction<INRegister> iNItemLabels;
		[PXUIField(DisplayName = Messages.INItemLabels, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable INItemLabels(PXAdapter adapter)
		{
			if (receipt.Current != null)
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters["RefNbr"] = receipt.Current.RefNbr;
				throw new PXReportRequiredException(parameters, "IN619200", Messages.INItemLabels);
			}
			return adapter.Get();
		}
		#region SiteStatus Lookup
		public PXFilter<INSiteStatusFilter> sitestatusfilter;
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public INSiteStatusLookup<INSiteStatusSelected, INSiteStatusFilter> sitestatus;

		public PXAction<INRegister> addInvBySite;
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

		public PXAction<INRegister> addInvSelBySite;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddInvSelBySite(PXAdapter adapter)
		{
			foreach (INSiteStatusSelected line in sitestatus.Cache.Cached)
			{
				if (line.Selected == true && line.QtySelected > 0)
				{
					INTran newline = PXCache<INTran>.CreateCopy(this.transactions.Insert(new INTran()));
					newline.SiteID = line.SiteID;
					newline.InventoryID = line.InventoryID;
					newline.SubItemID = line.SubItemID;
					newline.UOM = line.BaseUnit;
					newline = PXCache<INTran>.CreateCopy(transactions.Update(newline));
					newline.LocationID = line.LocationID;
					newline = PXCache<INTran>.CreateCopy(transactions.Update(newline));
					newline.Qty = line.QtySelected;
					transactions.Update(newline);
				}
			}
			sitestatus.Cache.Clear();
			return adapter.Get();
		}

		protected virtual void INSiteStatusFilter_OnlyAvailable_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = false;
		}

		protected virtual void INSiteStatusFilter_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			INSiteStatusFilter row = (INSiteStatusFilter)e.Row;
			if (row != null && receipt.Current != null)
				row.SiteID = receipt.Current.SiteID;
		}
		#endregion
		public INReceiptEntry()
		{
			INSetup record = insetup.Current;

			PXUIFieldAttribute.SetVisible<INTran.tranType>(transactions.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<INTran.tranType>(transactions.Cache, null, false);
		}


		public override void Persist()
		{
			foreach (INTran tran in transactions.Select())
			{
				INTran trtran = PXSelect<INTran, Where<INTran.tranType, Equal<Current<INTran.origTranType>>,
					And<INTran.refNbr, Equal<Current<INTran.origRefNbr>>,
					And<INTran.lineNbr, Equal<Current<INTran.origLineNbr>>,
					And<INTran.tranType, Equal<INTranType.transfer>>>>>>.SelectSingleBound(this, new object[] { tran });

                if (receipt.Current.Hold != true && trtran != null && trtran.BaseQty != tran.BaseQty)
                {
                    decimal qty = INUnitAttribute.ConvertFromBase(transactions.Cache, tran.InventoryID, tran.UOM, trtran.BaseQty ?? 0m, INPrecision.QUANTITY);
                    INTran copy = (INTran)transactions.Cache.CreateCopy(tran);
                    transactions.Update(copy);
                    transactions.Cache.RaiseExceptionHandling<INTran.qty>(tran, tran.Qty, new PXSetPropertyException(CS.Messages.Entry_EQ, qty));
                }
			}

			base.Persist();
		}

		protected virtual void INRegister_DocType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = INDocType.Receipt;
		}

		protected virtual void INRegister_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (insetup.Current.RequireControlTotal == false)
			{
				if (PXCurrencyAttribute.IsNullOrEmpty(((INRegister)e.Row).TotalCost) == false)
				{
					sender.SetValue<INRegister.controlCost>(e.Row, ((INRegister)e.Row).TotalCost);
				}
				else
				{
					sender.SetValue<INRegister.controlCost>(e.Row, 0m);
				}

				if (PXCurrencyAttribute.IsNullOrEmpty(((INRegister)e.Row).TotalQty) == false)
				{
					sender.SetValue<INRegister.controlQty>(e.Row, ((INRegister)e.Row).TotalQty);
				}
				else
				{
					sender.SetValue<INRegister.controlQty>(e.Row, 0m);
				}
			}

			if (((INRegister)e.Row).Hold == false && ((INRegister)e.Row).Released == false)
			{
				if ((bool)insetup.Current.RequireControlTotal)
				{
					if (((INRegister)e.Row).TotalCost != ((INRegister)e.Row).ControlCost)
					{
						sender.RaiseExceptionHandling<INRegister.controlCost>(e.Row, ((INRegister)e.Row).ControlCost, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else
					{
						sender.RaiseExceptionHandling<INRegister.controlCost>(e.Row, ((INRegister)e.Row).ControlCost, null);
					}

					if (((INRegister)e.Row).TotalQty != ((INRegister)e.Row).ControlQty)
					{
						sender.RaiseExceptionHandling<INRegister.controlQty>(e.Row, ((INRegister)e.Row).ControlQty, new PXSetPropertyException(Messages.DocumentOutOfBalance));
					}
					else
					{
						sender.RaiseExceptionHandling<INRegister.controlQty>(e.Row, ((INRegister)e.Row).ControlQty, null);
					}
				}
			}
		}

		protected virtual void INRegister_TransferNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INTran newtran = null;
			INTran prev_tran = null;
			int? prev_linenbr = null;

            using (new PXReadBranchRestrictedScope())
            {
                foreach (PXResult<INTran, INTranSplit, INItemPlan, INSite> res in
                    PXSelectJoin<INTran,
                    InnerJoin<INTranSplit,
                                 On<INTranSplit.tranType, Equal<INTran.tranType>,
                                And<INTranSplit.refNbr, Equal<INTran.refNbr>,
                                And<INTranSplit.lineNbr, Equal<INTran.lineNbr>>>>,
                    InnerJoin<INItemPlan,
                                 On<INItemPlan.planID, Equal<INTranSplit.planID>,
                                And<INItemPlan.planType, Equal<INPlanConstants.plan42>>>,
                    InnerJoin<INSite, On<INSite.siteID, Equal<INTran.toSiteID>>>>>,
                    Where<INTran.docType, Equal<Required<INTran.docType>>,
                        And<INTran.refNbr, Equal<Required<INTran.refNbr>>,
                        And<INTran.docType, Equal<INDocType.transfer>,
                        And<INTran.invtMult, Equal<shortMinus1>>>>>,
                    OrderBy<Asc<INTran.tranType, Asc<INTran.refNbr, Asc<INTran.lineNbr>>>>>
                    .Select(this, INDocType.Transfer, ((INRegister)e.Row).TransferNbr))
                {
                    INTran tran = res;
                    INTranSplit split = res;
                    INSite site = res;

                    if (object.Equals(prev_tran, tran) == false)
                    {
                        if (!object.Equals(receipt.Current.BranchID, site.BranchID))
                        {
                            INRegister copy = PXCache<INRegister>.CreateCopy(receipt.Current);
                            copy.BranchID = site.BranchID;
                            receipt.Update(copy);
                        }
                        newtran = PXCache<INTran>.CreateCopy(tran);
                        newtran.OrigBranchID = newtran.BranchID;
                        newtran.OrigTranType = newtran.TranType;
                        newtran.OrigRefNbr = newtran.RefNbr;
                        newtran.OrigLineNbr = newtran.LineNbr;
                        newtran.BranchID = site.BranchID;
                        newtran.DocType = ((INRegister)e.Row).DocType;
                        newtran.RefNbr = ((INRegister)e.Row).RefNbr;
                        newtran.LineNbr = (int)PXLineNbrAttribute.NewLineNbr<INTran.lineNbr>(transactions.Cache, e.Row);
                        newtran.InvtMult = (short)1;
                        newtran.SiteID = newtran.ToSiteID;
                        newtran.LocationID = newtran.ToLocationID;
                        newtran.ToSiteID = null;
                        newtran.ToLocationID = null;
                        newtran.Qty = 0m;
                        newtran.BaseQty = 0m;
                        newtran.Released = false;
                        newtran.InvtAcctID = null;
                        newtran.InvtSubID = null;
                        newtran.ReasonCode = null;
                        newtran.ARDocType = null;
                        newtran.ARRefNbr = null;
                        newtran.ARLineNbr = null;
	                    newtran.ProjectID = null;
	                    newtran.TaskID = null;

                        splits.Current = null;

                        newtran = transactions.Insert(newtran);

                        if (splits.Current != null)
                        {
                            splits.Delete(splits.Current);
                        }

                        prev_tran = tran;
                        prev_linenbr = newtran.LineNbr;
                    }

                    INTranSplit newsplit = PXCache<INTranSplit>.CreateCopy(split);
                    newsplit.DocType = ((INRegister)e.Row).DocType;
                    newsplit.RefNbr = ((INRegister)e.Row).RefNbr;
                    newsplit.LineNbr = prev_linenbr;
                    newsplit.SplitLineNbr = (int)PXLineNbrAttribute.NewLineNbr<INTranSplit.splitLineNbr>(splits.Cache, e.Row);
                    newsplit.InvtMult = (short)1;
                    newsplit.SiteID = tran.ToSiteID;
                    newsplit.LocationID = tran.ToLocationID;
                    newsplit.PlanID = null;
                    newsplit.Released = false;
	                newsplit.ProjectID = null;
	                newsplit.TaskID = null;

                    newsplit = splits.Insert(newsplit);

                    if (newtran.BaseQty == tran.BaseQty)
                    {
                        //fix conversion rounding
                        newtran = PXCache<INTran>.CreateCopy(newtran);
                        newtran.Qty = tran.Qty;
                        newtran.UnitCost = PXPriceCostAttribute.Round(tran.Qty != 0m ? (decimal)(tran.TranCost / tran.Qty) : 0m);
                        newtran.TranCost = tran.TranCost;
                        newtran = transactions.Update(newtran);
                    }
                }
            }
		}

        INRegister copy;

		protected virtual void INRegister_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
			{
				return;
			}

			release.SetEnabled(e.Row != null && ((INRegister)e.Row).Hold == false && ((INRegister)e.Row).Released == false);
			iNEdit.SetEnabled(e.Row != null && ((INRegister)e.Row).Hold == false && ((INRegister)e.Row).Released == false);
			iNRegisterDetails.SetEnabled(e.Row != null && ((INRegister)e.Row).Released == true);
			iNItemLabels.SetEnabled(e.Row != null && ((INRegister)e.Row).Released == true);
            addInvBySite.SetEnabled(e.Row != null && ((INRegister)e.Row).Released == false);

			PXUIFieldAttribute.SetEnabled(sender, e.Row, ((INRegister)e.Row).Released == false && ((INRegister)e.Row).OrigModule == GL.BatchModule.IN);
			PXUIFieldAttribute.SetEnabled<INRegister.refNbr>(sender, e.Row, true);
			PXUIFieldAttribute.SetEnabled<INRegister.totalQty>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<INRegister.totalCost>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<INRegister.status>(sender, e.Row, false);

			sender.AllowInsert = true;
			sender.AllowUpdate = (((INRegister)e.Row).Released == false);
			sender.AllowDelete = (((INRegister)e.Row).Released == false && ((INRegister)e.Row).OrigModule == GL.BatchModule.IN);

			lsselect.AllowInsert = (((INRegister)e.Row).Released == false && ((INRegister)e.Row).OrigModule == GL.BatchModule.IN);
			lsselect.AllowUpdate = (((INRegister)e.Row).Released == false);
			lsselect.AllowDelete = (((INRegister)e.Row).Released == false && ((INRegister)e.Row).OrigModule == GL.BatchModule.IN);

			PXUIFieldAttribute.SetVisible<INRegister.controlQty>(sender, e.Row, (bool)insetup.Current.RequireControlTotal);
			PXUIFieldAttribute.SetVisible<INRegister.controlCost>(sender, e.Row, (bool)insetup.Current.RequireControlTotal);

            INTran tran = transactions.Current ?? transactions.SelectWindowed(0, 1);
			if (tran != null)
			{
				((INRegister)e.Row).TransferNbr = tran.OrigRefNbr;
			}
            PXUIFieldAttribute.SetEnabled<INRegister.transferNbr>(sender, e.Row, sender.AllowUpdate && tran == null);
			PXUIFieldAttribute.SetEnabled<INRegister.branchID>(sender, e.Row, sender.AllowUpdate && ((INRegister)e.Row).TransferNbr == null);

            if (sender.Graph.IsImport == true && copy != null && sender.ObjectsEqual<INRegister.transferNbr, INRegister.released>(e.Row, copy))
            {
                return;
            }

            PXUIFieldAttribute.SetEnabled<INTran.branchID>(transactions.Cache, null, sender.AllowUpdate && ((INRegister)e.Row).TransferNbr == null);
            PXUIFieldAttribute.SetEnabled<INTran.inventoryID>(transactions.Cache, null, sender.AllowUpdate && ((INRegister)e.Row).TransferNbr == null);
            PXUIFieldAttribute.SetEnabled<INTran.subItemID>(transactions.Cache, null, sender.AllowUpdate && ((INRegister)e.Row).TransferNbr == null);
            PXUIFieldAttribute.SetEnabled<INTran.siteID>(transactions.Cache, null, sender.AllowUpdate && ((INRegister)e.Row).TransferNbr == null);

			PXUIFieldAttribute.SetVisible<INTran.projectID>(transactions.Cache, null, IsPMVisible);
			PXUIFieldAttribute.SetVisible<INTran.taskID>(transactions.Cache, null, IsPMVisible);

            copy = PXCache<INRegister>.CreateCopy((INRegister)e.Row);
		}

        public class TransferNbrSelectorAttribute : PXSelectorAttribute
        {
            protected BqlCommand _RestrictedSelect;

            public TransferNbrSelectorAttribute(Type searchType)
                : base(searchType)
            { 
                _RestrictedSelect = BqlCommand.CreateInstance(typeof(Search2<INRegister.refNbr, InnerJoin<INSite, On<INSite.siteID, Equal<INRegister.toSiteID>>>, Where<MatchWithBranch<INSite.branchID>>>));
            }

            public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
            {
                using (new PXReadBranchRestrictedScope())
                {
                    base.FieldVerifying(sender, e);
                }
            }

			public override void CacheAttached(PXCache sender)
			{
				base.CacheAttached(sender);
				PXView outerview = new PXView(sender.Graph, true, _Select);
				PXView view = sender.Graph.Views[_ViewName] = new PXView(sender.Graph, true, _Select, (PXSelectDelegate)delegate()
				{
					int startRow = PXView.StartRow;
					int totalRows = 0;
					List<object> res;

					using (new PXReadBranchRestrictedScope())
					{
						res = outerview.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
						PXView.StartRow = 0;
					}

					PXCache cache = outerview.Graph.Caches[typeof(INSite)];

					return res.FindAll((item) =>
					{
						return _RestrictedSelect.Meet(cache, item is PXResult ? PXResult.Unwrap<INSite>(item) : item);
					});
				});

				if (_DirtyRead)
				{
					view.IsReadOnly = false;
				}

			}
		}


        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Transfer Nbr.")]
        [TransferNbrSelector(typeof(Search2<INRegister.refNbr, 
            InnerJoin<INSite, On<INSite.siteID, Equal<INRegister.toSiteID>>, 
            InnerJoin<INTransferInTransit, On<INTransferInTransit.refNoteID, Equal<INRegister.noteID>>, 
            LeftJoin<INTran, On<INTran.origTranType, Equal<INTranType.transfer>, And<INTran.origRefNbr, Equal<INRegister.refNbr>, And<INTran.released, Equal<boolFalse>>>>>>>, 
        Where<INRegister.origModule, Equal<GL.BatchModule.moduleIN>, And<INRegister.docType, Equal<INDocType.transfer>, And<INRegister.released, Equal<boolTrue>, And<INTran.refNbr, IsNull, And<Match<INSite, Current<AccessInfo.userName>>>>>>>>))]
        protected virtual void INRegister_TransferNbr_CacheAttached(PXCache sender)
        { 
        }

		protected virtual void INRegister_TransferNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
            INTran tran = transactions.SelectWindowed(0, 1);
			if (tran != null)
            {
				e.Cancel = true;
			}
		}

		protected virtual void INTran_DocType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = INDocType.Receipt;
		}

        [PXDBString(3, IsFixed = true)]
        [PXDefault(INTranType.Receipt)]
        [PXUIField(Enabled = false, Visible = false)]
        protected virtual void INTran_TranType_CacheAttached(PXCache sender)
        { 
        }

		protected virtual void INTran_InvtMult_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = INTranType.InvtMult(((INTran)e.Row).TranType);
		}

		protected virtual void INTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<INTran.uOM>(e.Row);
			sender.SetDefaultExt<INTran.tranDesc>(e.Row);
		}

		protected virtual void INTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DefaultUnitCost(sender, e);
		}

		protected virtual void INTran_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
            DefaultUnitCost(sender, e);
		}

		protected virtual void INTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			INTran row = (INTran)e.Row;
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				if (!string.IsNullOrEmpty(row.POReceiptNbr))
				{
					if (PXDBQuantityAttribute.Round((decimal)(row.Qty + row.OrigQty)) > 0m)
					{
						sender.RaiseExceptionHandling<INTran.qty>(row, row.Qty, new PXSetPropertyException(CS.Messages.Entry_LE, -row.OrigQty));
					}
					else if (PXDBQuantityAttribute.Round((decimal)(row.Qty + row.OrigQty)) < 0m)
					{
						sender.RaiseExceptionHandling<INTran.qty>(row, row.Qty, new PXSetPropertyException(CS.Messages.Entry_GE, -row.OrigQty));
					}
				}

				
			}

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update))
			{
				if (row.Qty == 0 && row.TranCost > 0)
                { 
                    if(row.POReceiptNbr!=null && row.POReceiptLineNbr != null && row.POReceiptType!=null)
                    {
                        PO.POReceiptLine poreceiptline = new PO.POReceiptLine();
                        poreceiptline.ReceiptType = row.POReceiptType;
                        poreceiptline.LineNbr = row.POReceiptLineNbr;
                        poreceiptline.ReceiptNbr = row.POReceiptNbr;
                        throw
                            new PXErrorContextProcessingException(this, poreceiptline, new PXSetPropertyException(Messages.ZeroQtyWhenNonZeroCost));
                    }                
                    sender.RaiseExceptionHandling<INTran.qty>(row, row.Qty,
                            new PXSetPropertyException(Messages.ZeroQtyWhenNonZeroCost));
                }
                CheckForSingleLocation(sender, row);
				CheckSplitsForSameTask(sender, row);
				CheckLocationTaskRule(sender, row);
			}
		}

		protected virtual void INTran_ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INTran row = e.Row as INTran;
			if (row == null) return;

			if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.IN))
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
						if (project != null && project.ContractCD != null && project.VisibleInIN == true)
						{
							e.NewValue = project.ContractCD;
							return;
						}
					}
				}
			}
		}

		protected virtual void INTran_TaskID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INTran row = e.Row as INTran;
			if (row == null) return;
			
			if (PM.ProjectAttribute.IsPMVisible(this, BatchModule.IN))
			{
				if (row.LocationID != null )
				{
					PXResultset<INLocation> result = PXSelectJoin<INLocation,
						LeftJoin<PMTask, On<PMTask.projectID, Equal<INLocation.projectID>, And<PMTask.taskID, Equal<INLocation.taskID>>>>,
						Where<INLocation.siteID, Equal<Required<INLocation.siteID>>,
						And<INLocation.locationID, Equal<Required<INLocation.locationID>>>>>.Select(sender.Graph, row.SiteID, row.LocationID);

					foreach (PXResult<INLocation, PMTask> res in result)
					{
						PMTask task = (PMTask)res;
						if (task != null && task.TaskCD != null && task.VisibleInIN == true && task.IsActive == true)
						{
							e.NewValue = task.TaskCD;
							return;
						}
					}

				}
			}
		}

		protected virtual void INTran_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INTran row = e.Row as INTran;
			if (row == null) return;
			
			sender.SetDefaultExt<INTran.projectID>(e.Row); //will set pending value for TaskID to null if project is changed. This is the desired behavior for all other screens.
			if (sender.GetValuePending<INTran.taskID>(e.Row) == null) //To redefault the TaskID in currecnt screen - set the Pending value from NULL to NOTSET
				sender.SetValuePending<INTran.taskID>(e.Row, PXCache.NotSetValue);
			sender.SetDefaultExt<INTran.taskID>(e.Row);
		}

		protected virtual void DefaultUnitCost(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object UnitCost;
			sender.RaiseFieldDefaulting<INTran.unitCost>(e.Row, out UnitCost);

			if (UnitCost != null && (decimal)UnitCost != 0m)
			{
				decimal? unitcost = INUnitAttribute.ConvertToBase<INTran.inventoryID>(sender, e.Row, ((INTran)e.Row).UOM, (decimal)UnitCost, INPrecision.UNITCOST);
				sender.SetValueExt<INTran.unitCost>(e.Row, unitcost);
			}
		}

		protected virtual bool IsPMVisible
		{
			get
			{
				PM.PMSetup setup = PXSelect<PM.PMSetup>.Select(this);
				if (setup == null)
				{
					return false;
				}
				else
				{
					if (setup.IsActive != true)
						return false;
					else
						return setup.VisibleInIN == true;
				}
			}
		}

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (string.Compare(viewName, "transactions", StringComparison.OrdinalIgnoreCase) == 0)
			{
				CorrectKey("DocType", CurrentDocument.Current.DocType, keys, values);
				CorrectKey("RefNbr", CurrentDocument.Current.RefNbr, keys, values);
				INTran tran = PXSelect<INTran, Where<INTran.docType, Equal<Current<INRegister.docType>>, And<INTran.refNbr, Equal<Current<INRegister.refNbr>>>>, OrderBy<Desc<INTran.lineNbr>>>.SelectSingleBound(this, new object[] { CurrentDocument.Current });
				CorrectKey("LineNbr", tran == null? 1 : tran.LineNbr + 1, keys, values);
			}
			return true;
		}

		protected static void CorrectKey(string name, object value, IDictionary keys, IDictionary values)
		{
			CorrectKey(name, value, keys);
			CorrectKey(name, value, values);
		}

		protected static void CorrectKey(string name, object value, IDictionary dict)
		{
			if (dict.Contains(name))
				dict[name] = value;
			else
				dict.Add(name, value);
		}

		public virtual bool RowImporting(string viewName, object row) { return row == null; }
		public virtual bool RowImported(string viewName, object row, object oldRow) { return oldRow == null; }
		public virtual void PrepareItems(string viewName, IEnumerable items){}


		protected virtual void CheckLocationTaskRule(PXCache sender, INTran row)
		{
			if (row.TaskID != null)
			{
				INLocation selectedLocation = (INLocation)PXSelectorAttribute.Select(sender, row, sender.GetField(typeof(INTran.locationID)));

				if (selectedLocation != null && selectedLocation.TaskID != row.TaskID)
				{
					sender.RaiseExceptionHandling<INTran.locationID>(row, selectedLocation.LocationCD,
						new PXSetPropertyException(Messages.LocationIsMappedToAnotherTask, PXErrorLevel.Warning ));

				}
			}
		}

		protected virtual void CheckForSingleLocation(PXCache sender, INTran row)
		{
			InventoryItem item = (InventoryItem)PXSelectorAttribute.Select(sender, row, sender.GetField(typeof(INTran.inventoryID)));
			if (item != null && item.StkItem == true && row.TaskID != null && row.LocationID == null)
			{
				sender.RaiseExceptionHandling<INTran.locationID>(row, null, new PXSetPropertyException(Messages.RequireSingleLocation));
			}
		}

		protected virtual void CheckSplitsForSameTask(PXCache sender, INTran row)
		{
			if (row.HasMixedProjectTasks == true)
			{
				sender.RaiseExceptionHandling<INTran.locationID>(row, null, new PXSetPropertyException(Messages.MixedProjectsInSplits));
			}

		}
	}
}
