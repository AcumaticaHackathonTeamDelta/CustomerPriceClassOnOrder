using PX.Objects.CT;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.BQLConstants;
using PX.Objects.TX;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.TM;
using PX.Objects.PM;
using PX.Common;
using System.Linq;

namespace PX.Objects.EP
{
    public class ExpenseClaimEntry : PXGraph<ExpenseClaimEntry, EPExpenseClaim>, PXImportAttribute.IPXPrepareItems
    {
        public class ExpenceClaimApproval<SourceAssign> : EPApprovalAction<SourceAssign, EPExpenseClaim.approved, EPExpenseClaim.rejected>
            where SourceAssign : EPExpenseClaim
        {
            public ExpenceClaimApproval(PXGraph graph, Delegate @delegate)
                : base(graph, @delegate)
            {
                Initialize();
            }

            public ExpenceClaimApproval(PXGraph graph)
                : base(graph)
            {
                Initialize();
            }

            private void Initialize()
            {
                if (!PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>())
                {
                    this.Cache.Graph.FieldVerifying.AddHandler<EPApproval.ownerID>((sender, e) =>
                    {
                        e.Cancel = true;
                    });
                }
            }
            protected override bool DoAssignApprover(SourceAssign source, int map)
            {
                if (PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>())
                    return base.DoAssignApprover(source, map);

                PXCache cache = this._Graph.Caches[typeof(EPEmployee)];
                EPEmployee employee = (EPEmployee)cache.Current;
                if (employee.SupervisorID != null)
                {
                    EPEmployee super = (EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPEmployee.supervisorID>>>>.Select(this._Graph, employee.BAccountID);
                    if (super != null)
                    {
                        source.WorkgroupID = null;
                        source.OwnerID = super.UserID;
                        return true;
                    }
                }
                return false;
            }
        }

        public class EPExpenseClaimDetailsForSubmit : EPExpenseClaimDetails
        {
            #region ClaimDetailID
            public new abstract class claimDetailID : PX.Data.IBqlField
            {
            }
            [PXDBIdentity(IsKey = true)]
            public override Int32? ClaimDetailID
            {
                get;
                set;
            }
            #endregion

            #region RefNbr
            public new abstract class refNbr : PX.Data.IBqlField
            {
            }
            [PXDBString(15, IsUnicode = true)]
            public override String RefNbr
            {
                get;
                set;
            }
            #endregion
            #region TaxCategoryID

            public new abstract class taxCategoryID : PX.Data.IBqlField
            {
            }

            [PXDBString(10, IsUnicode = true)]
            public override String TaxCategoryID
            {
                get;
                set;
            }
            #endregion
        };

        #region Buttons declaration

        public PXAction<EPExpenseClaim> action;
        [PXUIField(DisplayName = Messages.Actions)]
        [PXButton]
        protected virtual IEnumerable Action(PXAdapter adapter)
        {
            foreach (EPExpenseClaim claim in adapter.Get<EPExpenseClaim>())
            {
                ExpenseClaim.Search<EPExpenseClaim.refNbr>(((EPExpenseClaim)claim).RefNbr);
            }
            return adapter.Get();
        }

        public PXAction<EPExpenseClaim> submit;
        [PXUIField(DisplayName = Messages.Submit)]
        [PXButton]
        protected virtual void Submit()
        {
            Save.Press();
            SubmitClaim(ExpenseClaim.Current);
        }

        protected virtual void SubmitClaim(EPExpenseClaim claim)
        {
            if (claim != null)
            {
                bool erroroccurred = false;
                foreach (EPExpenseClaimDetails detail in ExpenseClaimDetails.Select())
                {
                    if (detail.Rejected == false && detail.Hold == false && detail.Approved == false)
                    {
                        erroroccurred = true;
                        ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.claimDetailID>(detail, detail.ClaimDetailID, new PXSetPropertyException(Messages.ReceiptNotApproved, PXErrorLevel.RowError));
                    }
                    else if(detail.Rejected == true)
                    {
                        erroroccurred = true;
                        ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.claimDetailID>(detail, detail.ClaimDetailID, new PXSetPropertyException(Messages.RemovedRejectedReceipt, PXErrorLevel.RowError));
                    }
                    else if (detail.Hold == true)
                    {
                        erroroccurred = true;
                        ExpenseClaimDetails.Cache.RaiseExceptionHandling<EPExpenseClaimDetails.claimDetailID>(detail, detail.ClaimDetailID, new PXSetPropertyException(Messages.ReceiptTakenOffHold, PXErrorLevel.RowError));
                    }
                }
                if (erroroccurred)
                    throw new PXException(Messages.NotAllReceiptsOpenStatus);
                int? assignmentMap = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>()
                    ? epsetup.Current.ClaimAssignmentMapID
                    : 0;

                if (assignmentMap != null)
                {
                    if (Approval.Assign(claim, assignmentMap, epsetup.Current.ClaimAssignmentNotificationID))
                        claim.Approved = false;
                    else
                    {
                        PXTrace.WriteWarning(Messages.ApprovalMapCouldNotAssign);
                        claim.Approved = true;
                    }
                }
                else
                {
                    claim.Approved = true;
                }
                claim.Hold = false;
                ExpenseClaim.Update(claim);
                ExpenseClaim.Search<EPExpenseClaim.refNbr>(((EPExpenseClaim)claim).RefNbr);
            }
        }

        public PXAction<EPExpenseClaim> edit;
        [PXUIField(DisplayName = Messages.PutOnHold)]
        [PXButton]
        protected virtual void Edit()
        {
            if (ExpenseClaim.Current != null)
            {
                ExpenseClaim.Current.Approved = false;
                ExpenseClaim.Current.Rejected = false;
                ExpenseClaim.Current.Hold = true;
                ExpenseClaim.Update(ExpenseClaim.Current);
                PXSelectBase<EPApproval> select = new PXSelect<EPApproval, Where<EPApproval.refNoteID, Equal<Required<EPApproval.refNoteID>>>>(this);
                foreach (EPApproval approval in select.Select((ExpenseClaim.Current.NoteID)))
                {
                    this.Caches[typeof (EPApproval)].Delete(approval);
                }
            }
        }

        public PXAction<EPExpenseClaim> release;
        [PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton]
        public virtual IEnumerable Release(PXAdapter adapter)
        {
            foreach (PXResult<EPExpenseClaim> rec in adapter.Get())
            {
                EPExpenseClaim claim = rec;
                if (claim.Approved == false || claim.Released == true)
                {
                    throw new PXException(Messages.Document_Status_Invalid);
                }
                Save.Press();
                PXLongOperation.StartOperation(this, () => EPDocumentRelease.ReleaseDoc(claim));
            }
            return adapter.Get();
        }

        public ToggleCurrency<EPExpenseClaim> CurrencyView;

        public PXAction<EPExpenseClaim> expenseClaimPrint;
        [PXUIField(DisplayName = Messages.PrintExpenseClaim, MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report)]
        protected virtual IEnumerable ExpenseClaimPrint(PXAdapter adapter)
        {
            if (ExpenseClaim.Current != null)
            {
                var parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = ExpenseClaim.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "EP612000", Messages.PrintExpenseClaim);
            }

            return adapter.Get();
        }

        public PXAction<EPExpenseClaim> showSubmitReceipt;
        [PXUIField(DisplayName = Messages.AddReceipts, MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report, Tooltip = Messages.AddReceipts)]
        protected virtual IEnumerable ShowSubmitReceipt(PXAdapter adapter)
        {			
			if (ReceiptsForSubmit.AskExt(true) == WebDialogResult.OK)
            {
                return SubmitReceipt(adapter);
            }
			ReceiptsForSubmit.Cache.Clear();
			return adapter.Get();
        }

        public PXAction<EPExpenseClaim> submitReceipt;
        [PXUIField(DisplayName = Messages.Add, MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report)]
        protected virtual IEnumerable SubmitReceipt(PXAdapter adapter)
        {
            if (ExpenseClaim.Current != null)
            {
                foreach (EPExpenseClaimDetails item in ReceiptsForSubmit.Select())
                {
                    if (item.RefNbr == null)
                    {
                        if (item.Selected == true)
                        {
                            item.Submited = true;
                            item.Selected = false;
                            EPExpenseClaimDetails details = (EPExpenseClaimDetails)ExpenseClaimDetails.Cache.CreateCopy(item);
                            SubmitDetail(details);
                        }
                    }
                }
            }
            ReceiptsForSubmit.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<EPExpenseClaim> cancelSubmitReceipt;
        [PXUIField(DisplayName = Messages.Close, MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report)]
        protected virtual IEnumerable CancelSubmitReceipt(PXAdapter adapter)
        {
            ReceiptsForSubmit.Cache.Clear();
            return adapter.Get();
        }


        public PXAction<EPExpenseClaim> createNew;
        [PXUIField(DisplayName = Messages.AddNewReceipt)]
        [PXButton(Tooltip = Messages.AddReceiptToolTip)]
        protected virtual void CreateNew()
        {
            Save.Press();
            ExpenseClaimDetailEntry graph = (ExpenseClaimDetailEntry)PXGraph.CreateInstance(typeof(ExpenseClaimDetailEntry));
            graph.Clear(PXClearOption.ClearAll);
            EPExpenseClaimDetails claimDetails = (EPExpenseClaimDetails)graph.ClaimDetails.Cache.CreateInstance();
            if (ExpenseClaim.Current != null && ExpenseClaim.Current.EmployeeID != null)
            {
                claimDetails.EmployeeID = ExpenseClaim.Current.EmployeeID;
                claimDetails.BranchID = ExpenseClaim.Current.BranchID;
                claimDetails.CustomerID = ExpenseClaim.Current.CustomerID;
                claimDetails.CustomerLocationID = ExpenseClaim.Current.CustomerLocationID;
                bool enabledApprovalReceipt = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID != null;
                claimDetails.Hold = enabledApprovalReceipt;
                claimDetails.Approved = !enabledApprovalReceipt;
                claimDetails = graph.ClaimDetails.Insert(claimDetails);
                graph.ClaimDetails.SetValueExt<EPExpenseClaimDetails.refNbr>(claimDetails, ExpenseClaim.Current.RefNbr);
                graph.ClaimDetails.SetValueExt<EPExpenseClaimDetails.claimCuryInfoID>(claimDetails, ExpenseClaim.Current.CuryInfoID);
                this.Clear();
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            }
        }

        public PXAction<EPExpenseClaim> editDetail;
        [PXUIField(DisplayName = ActionsMessages.Edit)]
        [PXEditDetailButton]
        protected virtual void EditDetail()
        {
            if (ExpenseClaim.Current != null && ExpenseClaimDetails.Current != null)
            {
                Save.Press();
                ExpenseClaimDetailEntry graph = (ExpenseClaimDetailEntry)PXGraph.CreateInstance(typeof(ExpenseClaimDetailEntry));
                graph.Clear(PXClearOption.ClearAll);
                graph.ClaimDetails.Current = graph.ClaimDetails.Search<EPExpenseClaimDetails.claimDetailID>(ExpenseClaimDetails.Current.ClaimDetailID);
                this.Clear();
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            }
        }


        public PXAction<EPExpenseClaim> viewUnsubmitReceipt;
        [PXButton()]
        protected virtual void ViewUnsubmitReceipt()
        {
            if (ReceiptsForSubmit.Current != null)
            {
                ExpenseClaimDetailEntry graph = (ExpenseClaimDetailEntry)PXGraph.CreateInstance(typeof(ExpenseClaimDetailEntry));
                PXRedirectHelper.TryRedirect(graph, ReceiptsForSubmit.Current, PXRedirectHelper.WindowMode.NewWindow);
            }
        }


        public PXAction<EPExpenseClaim> changeOk;
        [PXUIField(DisplayName = "Change")]
        [PXButton()]
        protected virtual IEnumerable ChangeOk(PXAdapter adapter)
        {
            if (CustomerUpdateAsk.Current.CustomerUpdateAnsver != EPCustomerUpdateAsk.Nothing)
            {
                var query = ExpenseClaimDetails.Select().Where(_ => ((EPExpenseClaimDetails)_).ContractID == null || ProjectDefaultAttribute.IsNonProject(this, ((EPExpenseClaimDetails)_).ContractID));
                foreach (EPExpenseClaimDetails item in query)
                {
                    if (CustomerUpdateAsk.Current.CustomerUpdateAnsver == EPCustomerUpdateAsk.AllLines ||
                        item.CustomerID == CustomerUpdateAsk.Current.OldCustomerID)
                    {
                        ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.customerID>(item, CustomerUpdateAsk.Current.NewCustomerID);
                        ExpenseClaimDetails.Cache.Update(item);
                    }
                }
            }
            return adapter.Get();
        }

        public PXAction<EPExpenseClaim> changeCancel;
        [PXUIField(DisplayName = "Cancel")]
        [PXButton()]
        protected virtual IEnumerable ChangeCancel(PXAdapter adapter)
        {
            ExpenseClaim.Cache.SetValueExt<EPExpenseClaim.customerID>(ExpenseClaim.Current, CustomerUpdateAsk.Current.OldCustomerID);
            return adapter.Get();
        }

        #endregion

        #region Selects Declartion

        [PXHidden]
        public PXSelect<Contract> Dummy;

        [PXCopyPasteHiddenFields(typeof(EPExpenseClaim.approved), typeof(EPExpenseClaim.released), typeof(EPExpenseClaim.hold), typeof(EPExpenseClaim.rejected), typeof(EPExpenseClaim.status))]
        [PXViewName(Messages.ExpenseClaim)]
        public PXSelectJoin<EPExpenseClaim,
            InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPExpenseClaim.employeeID>>>,
            Where<EPExpenseClaim.createdByID, Equal<Current<AccessInfo.userID>>,
                         Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
                         Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
                Or<EPExpenseClaim.noteID, Approver<Current<AccessInfo.userID>>,
                Or<EPExpenseClaim.employeeID, WingmanUser<Current<AccessInfo.userID>>>>>>>> ExpenseClaim;

        public PXSelectJoin<EPExpenseClaim, LeftJoin<APInvoice, On<APInvoice.refNbr, Equal<EPExpenseClaim.aPRefNbr>>>,
            Where<EPExpenseClaim.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>> ExpenseClaimCurrent;

        [PXImport(typeof(EPExpenseClaim))]
        public PXSelect<EPExpenseClaimDetails, Where<EPExpenseClaimDetails.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>> ExpenseClaimDetails;
        public PXSelect<CurrencyInfo> currencyinfo;
        public PXSetup<EPSetup> epsetup;
        [PXViewName(Messages.Employee)]
        public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Optional<EPExpenseClaim.employeeID>>>> EPEmployee;

        public PXSelect<EPTax, Where<EPTax.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>, OrderBy<Asc<EPTax.refNbr, Asc<EPTax.taxID>>>> Tax_Rows;
        public PXSelectJoin<EPTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<EPTaxTran.taxID>>>, Where<EPTaxTran.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>> Taxes;

        [PXCopyPasteHiddenView]
        [PXViewName(Messages.Approval)]
        public ExpenceClaimApproval<EPExpenseClaim> Approval;
        private BqlCommand _approvalCommand;
        protected IEnumerable approval()
        {
            return this.QuickSelect(_approvalCommand);
        }

        [PXReadOnlyView]
        public PXSelect<EPExpenseClaimDetailsForSubmit,Where<True,Equal<False>>> ReceiptsForSubmit;

        public PXFilter<EPCustomerUpdateAsk> CustomerUpdateAsk;

		#endregion
		#region Execute Select
		protected virtual IEnumerable receiptsforsubmit()
		{
			PXSelectBase<EPExpenseClaimDetailsForSubmit> receiptsForSubmit = new PXSelect<EPExpenseClaimDetailsForSubmit,
			  Where<EPExpenseClaimDetailsForSubmit.employeeID, Equal<Current<EPExpenseClaim.employeeID>>,
				  And<EPExpenseClaimDetailsForSubmit.refNbr, IsNull,
				  And<EPExpenseClaimDetailsForSubmit.rejected, NotEqual<True>>>>>(this);
			HashSet<Int32?> receiptsInClaim = new HashSet<Int32?>();
			foreach (EPExpenseClaimDetails receiptInClaim in ExpenseClaimDetails.Select())
			{
				receiptsInClaim.Add(receiptInClaim.ClaimDetailID);
			}
			foreach (EPExpenseClaimDetailsForSubmit receiptForSubmit in receiptsForSubmit.Select())
			{
				if (receiptsInClaim.Contains(receiptForSubmit.ClaimDetailID))
				{
					continue;
				}
				yield return receiptForSubmit;
			}											
		}
		#endregion

		private bool ignoreDetailReadOnly = false;
        protected bool enabledApprovalReceipt => PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID!= null;

        public ExpenseClaimEntry()
        {
            if (epsetup.Current == null)
                throw new PXSetupNotEnteredException(ErrorMessages.SetupNotEntered, typeof(EPSetup), Messages.EPSetup);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.contractID>(ExpenseClaimDetails.Cache, null, PXAccess.FeatureInstalled<CS.FeaturesSet.contractManagement>() || PXAccess.FeatureInstalled<CS.FeaturesSet.projectModule>());

            //InitializeApproval:
            var command = Approval.View.BqlSelect.GetType();
            command = BqlCommand.AppendJoin(command,
                typeof(LeftJoin<ApproverEmployee, On<ApproverEmployee.userID, Equal<EPApproval.ownerID>>>));
            command = BqlCommand.AppendJoin(command,
                typeof(LeftJoin<ApprovedByEmployee, On<ApprovedByEmployee.userID, Equal<EPApproval.approvedByID>>>));
            _approvalCommand = BqlCommand.CreateInstance(command);
        }

        public void CheckAllowedUser()
        {
            EPEmployee employeeByUserID = PXSelect<EPEmployee, Where<EP.EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>.Select(this);
            if (employeeByUserID == null && System.Web.HttpContext.Current != null)
            {
                throw new PXException(Messages.MustBeEmployee);
            }
        }

        #region CurrencyInfo events
        protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
            {
                if (EPEmployee.Current != null && !string.IsNullOrEmpty(EPEmployee.Current.CuryID))
                {
                    e.NewValue = EPEmployee.Current.CuryID;
                    e.Cancel = true;
                }
            }
        }

        protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
            {
                if (EPEmployee.Current != null && !string.IsNullOrEmpty(EPEmployee.Current.CuryRateTypeID))
                {
                    e.NewValue = EPEmployee.Current.CuryRateTypeID;
                    e.Cancel = true;
                }
            }
        }

        protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            if (ExpenseClaim.Cache.Current != null)
            {
                e.NewValue = ((EPExpenseClaim)ExpenseClaim.Cache.Current).DocDate;
                e.Cancel = true;
            }
        }

        protected virtual void CurrencyInfo_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            CurrencyInfo info = e.Row as CurrencyInfo;
            if (info != null)
            {
                bool curyenabled = info.AllowUpdate(this.ExpenseClaimDetails.Cache);
                if (ExpenseClaim.Current != null && EPEmployee.Current != null && !(bool)EPEmployee.Current.AllowOverrideRate)
                {
                    curyenabled = false;
                }

                PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(cache, info, curyenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(cache, info, curyenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(cache, info, curyenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(cache, info, curyenabled);
            }
        }

        protected virtual void CurrencyInfo_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            foreach (EPExpenseClaimDetails detail in ExpenseClaimDetails.Select())
            {
                if (detail.ClaimCuryInfoID != detail.CuryInfoID)
                {
                    EPExpenseClaimDetails oldDetail = (EPExpenseClaimDetails)ExpenseClaimDetails.Cache.CreateCopy(detail);
                    ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetails.claimCuryTranAmt>(detail);
                    ExpenseClaimDetails.Cache.RaiseRowUpdated(detail, oldDetail);
                }
            }
        }

        #endregion

        #region Expense Claim Events		
        protected virtual void EPExpenseClaim_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            EPExpenseClaim doc = e.Row as EPExpenseClaim;
            if (doc == null)
            {
                return;
            }

            APInvoice apdoc =
                PXSelect<APInvoice, Where<APInvoice.refNbr, Equal<Current<EPExpenseClaim.aPRefNbr>>>>.Select(this);
            if (apdoc != null)
            {
                doc.APStatus = apdoc.Status;
            }
            ExpenseClaimDetails.Cache.AllowDelete = true;
            ExpenseClaimDetails.Cache.AllowUpdate = true;
            ExpenseClaimDetails.Cache.AllowInsert = true;
            Taxes.Cache.AllowDelete = true;
            Taxes.Cache.AllowUpdate = true;
            Taxes.Cache.AllowInsert = true;

            PXUIFieldAttribute.SetVisible<EPExpenseClaim.curyID>(cache, doc, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

            bool curyenabled = true;

            if (EPEmployee.Current != null && (bool)(EPEmployee.Current.AllowOverrideCury ?? false) == false)
            {
                curyenabled = false;
            }

            if (doc.Released == true)
            {
                PXUIFieldAttribute.SetEnabled(cache, doc, false);
                cache.AllowDelete = false;
                cache.AllowUpdate = false;
                ExpenseClaimDetails.Cache.AllowDelete = false;
                ExpenseClaimDetails.Cache.AllowUpdate = false;
                ExpenseClaimDetails.Cache.AllowInsert = false;
                Taxes.Cache.AllowDelete = false;
                Taxes.Cache.AllowUpdate = false;
                Taxes.Cache.AllowInsert = false;
                release.SetEnabled(false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.refNbr>(cache, doc, true);
                editDetail.SetEnabled(true);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled(cache, doc, true);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.status>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.aPRefNbr>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.aPDocType>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.approverID>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.workgroupID>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.curyDocBal>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.curyVatExemptTotal>(cache, doc, false);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.curyVatTaxableTotal>(cache, doc, false);

                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.curyID>(cache, doc, curyenabled);

                if (doc.Hold != true)
                {
                    PXUIFieldAttribute.SetEnabled(cache, doc, false);
                    PXUIFieldAttribute.SetEnabled<EPExpenseClaim.refNbr>(cache, doc, true);
                    PXUIFieldAttribute.SetEnabled<EPExpenseClaim.docDesc>(cache, doc, cache.GetStatus(e.Row) == PXEntryStatus.Inserted);
                    ExpenseClaimDetails.Cache.AllowInsert = false;
                    ExpenseClaimDetails.Cache.AllowDelete = false;
                    ExpenseClaimDetails.Cache.AllowUpdate = ignoreDetailReadOnly;
                    Taxes.Cache.AllowDelete = false;
                    Taxes.Cache.AllowUpdate = false;
                    Taxes.Cache.AllowInsert = false;
                }

                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.hold>(cache, e.Row, doc.Released != true);
                bool isApprover = doc.Status == EPExpenseClaimStatus.OpenStatus && Approval.IsApprover(doc);

                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.finPeriodID>(cache, e.Row, isApprover || doc.Status == EPExpenseClaimStatus.ApprovedStatus);
                if ((isApprover || doc.Status == EPExpenseClaimStatus.ApprovedStatus) && doc.Released != true)
                    APOpenPeriodAttribute.SetValidatePeriod<EPExpenseClaim.finPeriodID>(cache, e.Row, PeriodValidation.DefaultSelectUpdate);
                else
                    APOpenPeriodAttribute.SetValidatePeriod<EPExpenseClaim.finPeriodID>(cache, e.Row, PeriodValidation.Nothing);

                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.approverID>(cache, doc, isApprover && doc.Status == EPExpenseClaimStatus.OpenStatus);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.workgroupID>(cache, doc, isApprover && doc.Status == EPExpenseClaimStatus.OpenStatus);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.approvedByID>(cache, doc, false);

                bool isNotExistDetail = null == ExpenseClaimDetails.SelectSingle();
                PXUIFieldAttribute.SetEnabled<EPExpenseClaim.employeeID>(cache, doc, isNotExistDetail);

                cache.AllowDelete = true;
                cache.AllowUpdate = true;
                release.SetEnabled(doc.Approved == true);

                if (doc.EmployeeID == null || doc.BranchID == null)
                {
                    ExpenseClaimDetails.Cache.AllowDelete = false;
                    ExpenseClaimDetails.Cache.AllowUpdate = false;
                    ExpenseClaimDetails.Cache.AllowInsert = false;
                }

            }
            CurrencyInfo info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<EPExpenseClaim.curyInfoID>>>>.SelectSingleBound(this, new object[] { doc });
            string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyID>(currencyinfo.Cache, info);
            if (string.IsNullOrEmpty(message) && info != null && info.CuryRate == null)
                message = CM.Messages.RateNotFound;
            if (string.IsNullOrEmpty(message))
                cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyID>(e.Row, null, null);
            else
                cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyID>(e.Row, null, new PXSetPropertyException(message, PXErrorLevel.Warning));

            createNew.SetEnabled(ExpenseClaimDetails.Cache.AllowInsert && cache.GetStatus(doc) != PXEntryStatus.Inserted);
            editDetail.SetEnabled(cache.GetStatus(doc) != PXEntryStatus.Inserted);
            submit.SetEnabled(cache.GetStatus(doc) != PXEntryStatus.Inserted);
            submitReceipt.SetEnabled(ExpenseClaimDetails.Cache.AllowInsert);
            showSubmitReceipt.SetEnabled(ExpenseClaimDetails.Cache.AllowInsert);

            bool allowEdit = this.Accessinfo.UserID == doc.CreatedByID;

            if (EPEmployee.Current != null)
            {
                if (!allowEdit && this.Accessinfo.UserID == EPEmployee.Current.UserID)
                {
                    allowEdit = true;
                }

                if (!allowEdit)
                {
                    EPWingman wingMan = PXSelectJoin<EPWingman, 
                                                    InnerJoin<EPEmployee, On<EPWingman.wingmanID, Equal<EPEmployee.bAccountID>>>, 
                                                    Where<EPWingman.employeeID, Equal<Required<EPWingman.employeeID>>, 
                                                      And<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>>.Select(this, doc.EmployeeID, Accessinfo.UserID);
                    if (wingMan != null)
                    {
                        allowEdit = true;
                    }
                }
            }

            edit.SetEnabled(allowEdit);


        }

        protected virtual void EPExpenseClaim_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            CurrencyInfoAttribute.SetDefaults<EPExpenseClaimDetails.curyInfoID>(cache, e.Row);
        }

        protected virtual void EPExpenseClaim_LocationID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            cache.SetDefaultExt<EPExpenseClaim.taxZoneID>(e.Row);
        }

        protected virtual void EPExpenseClaim_DocDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            CurrencyInfoAttribute.SetEffectiveDate<EPExpenseClaim.docDate>(cache, e);
        }

        protected virtual void EPExpenseClaim_EmployeeID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPEmployee.RaiseFieldUpdated(cache, e.Row);

            if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
            {
                CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<EPExpenseClaim.curyInfoID>(cache, e.Row);

                string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
                if (string.IsNullOrEmpty(message) == false)
                {
                    cache.RaiseExceptionHandling<EPExpenseClaim.docDate>(e.Row, ((EPExpenseClaim)e.Row).DocDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
                }
                if (info != null)
                {
                    ((EPExpenseClaim)e.Row).CuryID = info.CuryID;
                }
            }
            cache.SetDefaultExt<EPExpenseClaim.locationID>(e.Row);
            cache.SetDefaultExt<EPExpenseClaim.departmentID>(e.Row);
        }

        protected virtual void EPApproval_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            EPApproval row = e.Row as EPApproval;
            ExpenseClaim.Cache.SetValueExt<EPExpenseClaim.approveDate>(ExpenseClaim.Current, row.ApproveDate);
        }

        protected virtual void EPApproval_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            EPApproval row = e.Row as EPApproval;
            ExpenseClaim.Cache.SetValueExt<EPExpenseClaim.approveDate>(ExpenseClaim.Current, row.ApproveDate);
        }

        protected virtual void EPApproval_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            ExpenseClaim.Cache.SetValueExt<EPExpenseClaim.approveDate>(ExpenseClaim.Current, null);
        }

        protected virtual void EPExpenseClaim_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            EPExpenseClaim oldRow = e.OldRow as EPExpenseClaim;

            PXResultset<EPExpenseClaimDetails> receipts = ExpenseClaimDetails.Select();

            if (row == null || oldRow == null)
                return;

            if (row.CustomerID != oldRow.CustomerID)
            {
                var query = receipts.Where(_ => ((EPExpenseClaimDetails)_).ContractID == null || ProjectDefaultAttribute.IsNonProject(this, ((EPExpenseClaimDetails)_).ContractID));
                if (query.Count() != 0)
                {
                    CustomerUpdateAsk.Current.NewCustomerID = row.CustomerID;
                    CustomerUpdateAsk.Current.OldCustomerID = oldRow.CustomerID;
                    CustomerUpdateAsk.AskExt();
                }
            }

            if (ExpenseClaim.Current != null && ExpenseClaim.Current.Released == true)
            {
                foreach (EPExpenseClaimDetails receipt in receipts)
                {
                    receipt.Status = EPExpenseClaimDetailsStatus.ReleasedStatus;
                    receipt.Released = true;
                }
            }
        }

        protected virtual void EPExpenseClaimDetails_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;

            if (row.ContractID != null)
            {
                if ((bool)((EPExpenseClaimDetails)e.Row).Billable && row.TaskID != null)
                {
                    PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.TaskID);
                    if (task != null && !(bool)task.VisibleInAP)
                        cache.RaiseExceptionHandling<EPExpenseClaimDetails.taskID>(e.Row, task.TaskCD,
                            new PXSetPropertyException(PM.Messages.TaskInvisibleInModule, task.TaskCD, BatchModule.AP));
                }
            }
        }

        protected virtual void EPExpenseClaimDetails_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
            if (row != null)
            {
                bool enabledEditReceipt = row.Hold == true || !enabledApprovalReceipt;
                bool enabledFinancialDetails = (row.Rejected != true) && (row.Released != true) && (row.HoldClaim != false);

                if (!enabledEditReceipt)
                {
                    PXUIFieldAttribute.SetEnabled(cache, row, false);
                }
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.expenseAccountID>(cache, row, enabledFinancialDetails);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.expenseSubID>(cache, row, enabledFinancialDetails);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.salesAccountID>(cache, row, enabledFinancialDetails && (row.Billable == true));
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.salesSubID>(cache, row, enabledFinancialDetails && (row.Billable == true));
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.taxCategoryID>(cache, row, enabledFinancialDetails);
            }
        }

        public override void Persist()
        {
            HashSet<Guid> notes = new HashSet<Guid>();
            foreach (EPExpenseClaimDetails deletedRow in ExpenseClaimDetails.Cache.Deleted)
            {
                if (ExpenseClaimDetails.Cache.GetStatus(deletedRow) != PXEntryStatus.InsertedDeleted)
                {
                    deletedRow.RefNbr = null;
                    deletedRow.Submited = false;
                    deletedRow.ClaimCuryInfoID = null;
                    deletedRow.ClaimCuryTranAmt = 0;
                    deletedRow.ClaimTranAmt = 0;
                    if (deletedRow.NoteID != null)
                        notes.Add((Guid)deletedRow.NoteID);
                    ExpenseClaimDetails.Cache.SetStatus(deletedRow, PXEntryStatus.Updated);
                }
            }

            if (notes.Count > 0)
            {
                PXCache noteCache = Caches[typeof(Note)];
                foreach (Note note in noteCache.Deleted)
                {
                    if (notes.Contains((Guid)note.NoteID))
                        noteCache.SetStatus(note, PXEntryStatus.Notchanged);
                }

                PXCache noteDocCache = Caches[typeof(NoteDoc)];
                foreach (NoteDoc noteDoc in noteDocCache.Deleted)
                {
                    if (notes.Contains((Guid)noteDoc.NoteID))
                        noteDocCache.SetStatus(noteDoc, PXEntryStatus.Notchanged);
                }
            }

            List<EPExpenseClaim> inserted = null;
            if (epsetup.Current.HoldEntry != true)
            {
                inserted = new List<EPExpenseClaim>();
                foreach (EPExpenseClaim item in this.ExpenseClaim.Cache.Inserted)
                    inserted.Add(item);
            }
            base.Persist();
            if (inserted != null)
            {
                foreach (EPExpenseClaim item in inserted)
                {
                    if (this.ExpenseClaim.Cache.GetStatus(item) != PXEntryStatus.Inserted)
                    {
                        SubmitClaim(item);
                    }
                }
                base.Persist();
            }
            if (ExpenseClaim.Current != null)
                ExpenseClaim.Search<EPExpenseClaim.refNbr>(ExpenseClaim.Current.RefNbr);
        }

        protected virtual void EPExpenseClaimDetails_InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;

            InventoryItem item = PXSelectorAttribute.Select<InventoryItem.inventoryID>(cache, row) as InventoryItem;
            decimal curyStdCost;
            if (item != null)
                PXCurrencyAttribute.CuryConvCury<EPExpenseClaimDetails.curyInfoID>(cache, e.Row, item.StdCost.Value, out curyStdCost, true);
            else
                curyStdCost = 0m;
            cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(row, curyStdCost);
        }

        protected virtual void EPExpenseClaimDetails_ExpenseSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (((EPExpenseClaimDetails)e.Row).ExpenseAccountID != null)
            {
                InventoryItem item = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).InventoryID);
                Location companyloc =
                    (Location)PXSelectJoin<Location, InnerJoin<BAccountR, On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<GL.Branch, On<BAccountR.bAccountID, Equal<GL.Branch.bAccountID>>>>, Where<GL.Branch.branchID, Equal<Current<EPExpenseClaimDetails.branchID>>>>.Select(this);
                Contract contract = PXSelect<Contract, Where<Contract.contractID, Equal<Required<Contract.contractID>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).ContractID);
                PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).ContractID, ((EPExpenseClaimDetails)e.Row).TaskID);
                Location customerLocation = (Location)PXSelectorAttribute.Select<EPExpenseClaimDetails.customerLocationID>(sender, e.Row);

                int? employee_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(EPEmployee.Current);
                int? item_SubID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.cOGSSubID>(item);
                int? company_SubID = (int?)Caches[typeof(Location)].GetValue<Location.cMPExpenseSubID>(companyloc);
                int? project_SubID = (int?)Caches[typeof(Contract)].GetValue<Contract.defaultSubID>(contract);
                int? task_SubID = (int?)Caches[typeof(PMTask)].GetValue<PMTask.defaultSubID>(task);
                int? location_SubID = (int?)Caches[typeof(Location)].GetValue<Location.cSalesSubID>(customerLocation);

                object value = SubAccountMaskAttribute.MakeSub<EPSetup.expenseSubMask>(this, epsetup.Current.ExpenseSubMask,
                    new object[] { employee_SubID, item_SubID, company_SubID, project_SubID, task_SubID, location_SubID },
                    new Type[] { typeof(EPEmployee.expenseSubID), typeof(InventoryItem.cOGSSubID), typeof(Location.cMPExpenseSubID), typeof(Contract.defaultSubID), typeof(PMTask.defaultSubID), typeof(Location.cSalesSubID) });

                sender.RaiseFieldUpdating<EPExpenseClaimDetails.expenseSubID>(e.Row, ref value);

                e.NewValue = (int?)value;
                e.Cancel = true;
            }
        }

        protected virtual void EPExpenseClaimDetails_SalesSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (((EPExpenseClaimDetails)e.Row).SalesAccountID != null)
            {
                object value = null;
                if (((EPExpenseClaimDetails)e.Row).Billable == true)
                {
                    InventoryItem item = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).InventoryID);
                    Location companyloc =
                        (Location)PXSelectJoin<Location, InnerJoin<BAccountR, On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>, InnerJoin<GL.Branch, On<BAccountR.bAccountID, Equal<GL.Branch.bAccountID>>>>, Where<GL.Branch.branchID, Equal<Current<EPExpenseClaimDetails.branchID>>>>.Select(this);
                    Contract contract = PXSelect<Contract, Where<Contract.contractID, Equal<Required<Contract.contractID>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).ContractID);
                    PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, ((EPExpenseClaimDetails)e.Row).ContractID, ((EPExpenseClaimDetails)e.Row).TaskID);
                    Location customerLocation = (Location)PXSelectorAttribute.Select<EPExpenseClaimDetails.customerLocationID>(sender, e.Row);

                    int? employee_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.salesSubID>(EPEmployee.Current);
                    int? item_SubID = (int?)Caches[typeof(InventoryItem)].GetValue<InventoryItem.salesSubID>(item);
                    int? company_SubID = (int?)Caches[typeof(Location)].GetValue<Location.cMPSalesSubID>(companyloc);
                    int? project_SubID = (int?)Caches[typeof(Contract)].GetValue<Contract.defaultSubID>(contract);
                    int? task_SubID = (int?)Caches[typeof(Location)].GetValue<PMTask.defaultSubID>(task);
                    int? location_SubID = (int?)Caches[typeof(Location)].GetValue<Location.cSalesSubID>(customerLocation);

                    value = SubAccountMaskAttribute.MakeSub<EPSetup.salesSubMask>(this, epsetup.Current.SalesSubMask,
                        new object[] { employee_SubID, item_SubID, company_SubID, project_SubID, task_SubID, location_SubID },
                        new Type[] { typeof(EPEmployee.salesSubID), typeof(InventoryItem.salesSubID), typeof(Location.cMPSalesSubID), typeof(Contract.defaultSubID), typeof(PMTask.defaultSubID), typeof(Location.cSalesSubID) });
                }

                sender.RaiseFieldUpdating<EPExpenseClaimDetails.salesSubID>(e.Row, ref value);

                e.NewValue = (int?)value;
                e.Cancel = true;
            }
        }

        protected virtual void EPExpenseClaimDetails_ClaimCuryTranAmt_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;
            if (row != null && row.TranAmt != null)
            {
                CurrencyInfo expenseCuriInfo = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<EPExpenseClaimDetails.curyInfoID>>>>.SelectSingleBound(this, null, row.CuryInfoID);
                if (currencyinfo.Current == null)
                    currencyinfo.Current = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<EPExpenseClaimDetails.curyInfoID>>>>.SelectSingleBound(this, null, row.ClaimCuryInfoID);
                decimal curyClaim = 0m;
                if (row.CuryInfoID == row.ClaimCuryInfoID || expenseCuriInfo != null && currencyinfo.Current != null && expenseCuriInfo.CuryID == currencyinfo.Current.CuryID)
                    curyClaim = row.CuryTranAmt.Value;
                else if (currencyinfo.Current != null && currencyinfo.Current.CuryRate != null)
                {
                    PXCurrencyAttribute.CuryConvCury<EPExpenseClaimDetails.claimCuryInfoID>(sender, row, row.TranAmt.Value, out curyClaim);
                }
                e.NewValue = curyClaim;
            }

        }

        protected virtual void EPExpenseClaimDetails_CuryEmployeePart_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e) //\
        {
            Decimal? newVal = e.NewValue as Decimal?;
            if (newVal < 0)
                throw new PXSetPropertyException(CS.Messages.FieldShouldNotBeNegative, PXUIFieldAttribute.GetDisplayName<EPExpenseClaimDetails.curyEmployeePart>(cache));
        }
        protected virtual void EPExpenseClaimDetails_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {         
            e.NewValue = enabledApprovalReceipt;
            e.Cancel = true;
        }
        protected virtual void EPExpenseClaim_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            if (row != null)
            {
                foreach (EPExpenseClaimDetails detail in ExpenseClaimDetails.Select())
                {
                    ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.holdClaim>(detail, row.Hold);
                }
            }
        }
        protected virtual void EPExpenseClaimDetails_Approved_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = !enabledApprovalReceipt;
            e.Cancel = true;
        }
        protected virtual void EPExpenseClaimDetails_Status_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = enabledApprovalReceipt ? EPExpenseClaimDetailsStatus.HoldStatus : EPExpenseClaimDetailsStatus.ApprovedStatus;
            e.Cancel = true;
        }

        protected virtual void EPExpenseClaimDetails_CustomerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;
            
            if(row?.CustomerID == null)
                cache.SetValueExt<EPExpenseClaimDetails.customerLocationID>(row, null);
        }

        [PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
        [PXUIField(DisplayName = "Currency", Enabled = false)]
        protected virtual void EPExpenseClaimDetails_CuryID_CacheAttached(PXCache cache)
        {
        }

        #endregion

        #region EPTaxTran Events
        protected virtual void EPTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
                return;

            PXUIFieldAttribute.SetEnabled<EPTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
        }
        #endregion

        #region EPApproval Cahce Attached
        [PXDBDate()]
        [PXDefault(typeof(EPExpenseClaim.docDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt()]
        [PXDefault(typeof(EPExpenseClaim.employeeID), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
        {
        }

        [PXDBString(60, IsUnicode = true)]
        [PXDefault(typeof(EPExpenseClaim.docDesc), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
        {
        }

        [PXDBLong()]
        [CurrencyInfo(typeof(EPExpenseClaim.curyInfoID))]
        protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
        {
        }

        [PXDBDecimal(4)]
        [PXDefault(typeof(EPExpenseClaim.curyDocBal), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
        {
        }

        [PXDBDecimal(4)]
        [PXDefault(typeof(EPExpenseClaim.docBal), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region Inner DACs

		[Serializable]
		[PXHidden]
		public partial class EPCustomerUpdateAsk : PX.Data.IBqlTable
		{

            #region PeriodDateSel
            public abstract class customerUpdateAnsver : PX.Data.IBqlField
            {
            }
            protected String _CustomerUpdateAnsver;
            [PXDBString(1, IsFixed = true)]
            [PXUIField(DisplayName = "Date Based On", Visibility = PXUIVisibility.Visible)]
            [PXDefault(SelectedCustomer)]
            [PXStringListAttribute(new string[] { SelectedCustomer, AllLines, Nothing },
                new string[] { Messages.SelectedCustomer, Messages.AllLines, Messages.Nothing })]
            public virtual String CustomerUpdateAnsver
            {
                get
                {
                    return this._CustomerUpdateAnsver;
                }
                set
                {
                    this._CustomerUpdateAnsver = value;
                }
            }
            #endregion

            #region NewCustomerID
            public abstract class newCustomerID : PX.Data.IBqlField
            {
            }
            [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
            [AR.CustomerActive(DescriptionField = typeof(AR.Customer.acctName))]
            public virtual Int32? NewCustomerID
            {
                get;
                set;
            }
            #endregion

            #region NewCustomerID
            public abstract class oldCustomerID : PX.Data.IBqlField
            {
            }
            protected Int32? _OldCustomerID;
            [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
            [AR.CustomerActive(DescriptionField = typeof(AR.Customer.acctName))]
            public virtual Int32? OldCustomerID
            {
                get
                {
                    return this._OldCustomerID;
                }
                set
                {
                    this._OldCustomerID = value;
                }
            }
            #endregion

            public const String SelectedCustomer = "S";
            public const String AllLines = "A";
            public const String Nothing = "N";

        }


        #endregion

        #region Function

        public void SubmitDetail(EPExpenseClaimDetails details)
        {
            details.RefNbr = ExpenseClaim.Current.RefNbr;
            details.ClaimCuryInfoID = ExpenseClaim.Current.CuryInfoID;
            ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetails.claimCuryTranAmt>(details);
            ExpenseClaimDetails.Update(details);
            EPExpenseClaimDetails oldRow = (EPExpenseClaimDetails)ExpenseClaimDetails.Cache.CreateCopy(details);
            oldRow.RefNbr = null;
            ExpenseClaimDetails.Cache.RaiseFieldUpdated<EPExpenseClaimDetails.refNbr>(details, oldRow);
        }

        #endregion

        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
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

        public void PrepareItems(string viewName, IEnumerable items) { }
    }
}
