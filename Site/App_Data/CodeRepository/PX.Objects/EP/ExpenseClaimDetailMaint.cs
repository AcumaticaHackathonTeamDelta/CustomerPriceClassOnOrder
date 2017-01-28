using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.TM;
using PX.Data.EP;
using PX.Objects.CS;

namespace PX.Objects.EP
{
    public class ExpenseClaimDetailMaint : PXGraph<ExpenseClaimDetailMaint>//, PXImportAttribute.IPXPrepareItems
    {

        [PXSelector(typeof(PX.SM.Users.pKID), SubstituteKey = typeof(PX.SM.Users.fullName), DescriptionField = typeof(PX.SM.Users.fullName), CacheGlobal = true)]
        [PXDBGuid(false)]
        [PXUIField(DisplayName = "Created by", Enabled = false)]
        protected virtual void EPExpenseClaimDetails_CreatedByID_CacheAttached(PXCache sender) { }


        #region Action

        public PXCancel<ExpenseClaimDetailsFilter> Cancel;

        public PXAction<ExpenseClaimDetailsFilter> AddNew;
        [PXInsertButton]
        [PXUIField(DisplayName = "")]
        [PXEntryScreenRights(typeof(ExpenseClaimDetailsFilter), nameof(ExpenseClaimDetailEntry.Insert))]
        protected virtual void addNew()
        {
            using (new PXPreserveScope())
            {
                ExpenseClaimDetailEntry graph = (ExpenseClaimDetailEntry)PXGraph.CreateInstance(typeof(ExpenseClaimDetailEntry));
                graph.Clear(PXClearOption.ClearAll);
                EPExpenseClaimDetails claimDetails = (EPExpenseClaimDetails)graph.ClaimDetails.Cache.CreateInstance();
                graph.ClaimDetails.Insert(claimDetails);
                graph.ClaimDetails.Cache.IsDirty = false;
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.InlineWindow);
            }
        }

        public PXAction<ExpenseClaimDetailsFilter> EditDetail;
        [PXEditDetailButton]
        [PXUIField(DisplayName = "")]
        protected virtual void editDetail()
        {
            EPExpenseClaimDetails row = ClaimDetails.Current;
            if (row == null) return;
            PXRedirectHelper.TryRedirect(this, row, PXRedirectHelper.WindowMode.InlineWindow);
        }

        public PXAction<ExpenseClaimDetailsFilter> delete;
        [PXDeleteButton]
        [PXUIField(DisplayName = "")]
        [PXEntryScreenRights(typeof(ExpenseClaimDetailsFilter))]
        protected void Delete()
        {
            if (ClaimDetails.Current == null) return;

            if (ClaimDetails.Current.RefNbr != null)
                throw new PXException(Messages.ReceiptMayNotBeDeleted);

            ClaimDetails.Cache.Delete(ClaimDetails.Current);
            this.Persist();
        }

        public PXAction<ExpenseClaimDetailsFilter> viewClaim;
        [PXButton]
        protected virtual void ViewClaim()
        {
            if (ClaimDetails.Current != null && ClaimDetails.Current.RefNbr != null)
            {
                EPExpenseClaim claim = PXSelect<EPExpenseClaim, Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaimDetails.refNbr>>>>.SelectSingleBound(this, null, ClaimDetails.Current.RefNbr);
                if (claim == null) return;
                PXRedirectHelper.TryRedirect(this, claim, PXRedirectHelper.WindowMode.NewWindow);
            }
        }


        #endregion


        #region Select
        public PXFilter<ExpenseClaimDetailsFilter> Filter;
        [PXFilterable()]
        [PXViewName(Messages.ClaimDetailsView)]
        public PXFilteredProcessingJoin<
                EPExpenseClaimDetails,
                ExpenseClaimDetailsFilter,
                LeftJoin<EPExpenseClaim,
                      On<EPExpenseClaim.refNbr, Equal<EPExpenseClaimDetails.refNbr>>,
                LeftJoin<EPEmployee,
                      On<EPEmployee.bAccountID, Equal<EPExpenseClaimDetails.employeeID>>>>,
                Where<Current2<ExpenseClaimDetailsFilter.employeeID>, IsNotNull,
                                    And<EPExpenseClaimDetails.employeeID, Equal<Current2<ExpenseClaimDetailsFilter.employeeID>>,
                                    Or<Current2<ExpenseClaimDetailsFilter.employeeID>, IsNull,
                                    And<Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
                                    Or<EPExpenseClaimDetails.createdByID, Equal<Current<AccessInfo.userID>>,
                                    Or<EPExpenseClaimDetails.employeeID, WingmanUser<Current<AccessInfo.userID>>,
                                    Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
                                    Or<EPExpenseClaimDetails.noteID, Approver<Current<AccessInfo.userID>>>>>>>>>>>,
                OrderBy<Desc<EPExpenseClaimDetails.claimDetailID>>> ClaimDetails;

        [PXHidden]
        public PXSelect<CR.BAccount> baccount;
        [PXHidden]
        public PXSelect<AP.Vendor> vendor;
        [PXHidden]
        public PXSelect<EPEmployee> employee;
        #endregion

        public ExpenseClaimDetailMaint()
        {
            ClaimDetails.SetProcessDelegate(ClaimDetail);
            ClaimDetails.SetProcessCaption(Messages.Claim);
            ClaimDetails.SetProcessAllCaption(Messages.ClaimAll);
            ClaimDetails.SetSelected<EPExpenseClaimDetails.selected>();
            ClaimDetails.AllowDelete = false;
            ClaimDetails.AllowInsert = false;
            ClaimDetails.AllowUpdate = true;

            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.claimDetailID>(ClaimDetails.Cache, null, false);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.branchID>(ClaimDetails.Cache, null, false);
        }


        #region Handler

        protected virtual void EPExpenseClaimDetails_Selected_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
            if (row != null && row.RefNbr != null && (bool)e.NewValue == true)
                sender.RaiseExceptionHandling<EPExpenseClaimDetails.selected>(row, e.NewValue, new PXSetPropertyException(Messages.ReceiptIsClaimed, PXErrorLevel.RowWarning));
        }


        #endregion

        [CM.CurrencyInfo(ModuleCode = "EP", CuryIDField = "curyID", CuryDisplayName = "Currency", Enabled = false)]
        [PXDBLong()]
        protected virtual void EPExpenseClaimDetails_CuryInfoID_CacheAttached(PXCache cache)
        {
        }

        [PXDBLong()]
        protected virtual void EPExpenseClaimDetails_ClaimCuryInfoID_CacheAttached(PXCache cache)
        {
        }

        #region Function

        private static void ClaimDetail(List<EPExpenseClaimDetails> details)
        {
            PXSetup<EPSetup> epsetup = new PXSetup<EPSetup>(PXGraph.CreateInstance(typeof(ExpenseClaimDetailEntry))); 
            bool enabledApprovalReceipt = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID != null;
            bool isError = false;
            bool notAllApproved = false;
            bool emptyClaim = true;
            var List = details.Where(item => string.IsNullOrEmpty(item.RefNbr)).OrderBy(detail => detail.ClaimDetailID).GroupBy(
                    item => new { item.EmployeeID, item.BranchID, item.CustomerID, item.CustomerLocationID }
                    , (key, item) => new { employee = key.EmployeeID, branch = key.BranchID, customer = key.CustomerID, customerLocation = key.CustomerLocationID, details = item }
                    );

            List<EPExpenseClaim> result = new List<EPExpenseClaim>();

            foreach (var item in List)
            {
                isError = false;
                notAllApproved = false;
                emptyClaim = true;
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    ExpenseClaimEntry expenseClaimEntry = CreateInstance<ExpenseClaimEntry>();
                    EPExpenseClaim expenseClaim = (EPExpenseClaim)expenseClaimEntry.ExpenseClaim.Cache.CreateInstance();
                    expenseClaim.EmployeeID = item.employee;
                    expenseClaim.BranchID = item.branch;
                    expenseClaim.CustomerID = item.customer;
                    expenseClaim.DocDesc = EP.Messages.SubmittedReceipt;
                    expenseClaim = expenseClaimEntry.ExpenseClaim.Update(expenseClaim);
                    expenseClaim.CustomerLocationID = item.customerLocation;

                    foreach (EPExpenseClaimDetails detail in item.details)
                    {                        
                        PXProcessing<EPExpenseClaimDetails>.SetCurrentItem(detail);
                        if (detail.Approved ?? false)
                        {
                            try
                            {
                                expenseClaimEntry.SubmitDetail(detail);
                                PXProcessing<EPExpenseClaimDetails>.SetProcessed();
                                emptyClaim = false;
                            }
                            catch (Exception ex)
                            {                               
                                PXProcessing<EPExpenseClaimDetails>.SetError(ex);
                                isError = true;
                            }
                        }
                        else
                        {
                            PXProcessing.SetError(enabledApprovalReceipt
                                ? Messages.ReceiptNotApproved
                                : Messages.ReceiptTakenOffHold);
                            notAllApproved = true;
                        }
                    }

                    if (!emptyClaim)
                    {
                        try
                        {
                            expenseClaimEntry.Actions.PressSave();
                            result.Add(expenseClaim);
                        }
                        catch (Exception ex)
                        {
                            foreach (EPExpenseClaimDetails detail in item.details)
                            {
                                PXProcessing<EPExpenseClaimDetails>.SetCurrentItem(detail);
                                PXProcessing<EPExpenseClaimDetails>.SetError(ex);
                            }
                            isError = true;
                        }
                    }
                    if (!isError)
                    {
                        ts.Complete();
                    }
                }
            }

            if (!isError && !notAllApproved)
            {
                if (result.Count == 1)
                {
                    ExpenseClaimEntry expenseClaimEntry = CreateInstance<ExpenseClaimEntry>();
                    PXRedirectHelper.TryRedirect(expenseClaimEntry, result[0], PXRedirectHelper.WindowMode.InlineWindow);
                }
            }
            else
            {
                PXProcessing<EPExpenseClaimDetails>.SetCurrentItem(null);
                throw new PXException(Messages.ErrorProcessingReceipts);
            }
        }
        #endregion

        [Serializable]
        public partial class ExpenseClaimDetailsFilter : IBqlTable
        {
            #region EmployeeID
            public abstract class employeeID : IBqlField { }

            [PXDBInt]
            [PXUIField(DisplayName = "Employee")]
            [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
            [PXSubordinateAndWingmenSelector()]
            [PXFieldDescription]
            public virtual Int32? EmployeeID
            {
                get;
                set;
            }

            #endregion
        }
    }
}
