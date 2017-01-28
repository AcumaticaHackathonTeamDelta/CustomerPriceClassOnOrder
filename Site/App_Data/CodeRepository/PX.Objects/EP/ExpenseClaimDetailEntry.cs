using System;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.RQ;
using PX.Objects.CS;


namespace PX.Objects.EP
{
    public class ExpenseClaimDetailEntry : PXGraph<ExpenseClaimDetailEntry, EPExpenseClaimDetails>
    {
        #region Select
        [PXViewName(Messages.ExpenseReceipt)]
        [PXCopyPasteHiddenFields(typeof(EPExpenseClaimDetails.refNbr))]
        public PXSelectJoin<EPExpenseClaimDetails,
                    LeftJoin<EPExpenseClaim, On<EPExpenseClaim.refNbr, Equal<EPExpenseClaimDetails.refNbr>>,
                    LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPExpenseClaimDetails.employeeID>>>>,
                    Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
                          Or<EPExpenseClaimDetails.createdByID, Equal<Current<AccessInfo.userID>>,
                          Or<EPExpenseClaimDetails.employeeID, WingmanUser<Current<AccessInfo.userID>>,
                          Or<EPEmployee.userID, TM.OwnedUser<Current<AccessInfo.userID>>,
                          Or<EPExpenseClaimDetails.noteID, Approver<Current<AccessInfo.userID>>>>>>>,
                    OrderBy<Desc<EPExpenseClaimDetails.claimDetailID>>> ClaimDetails;
        [PXCopyPasteHiddenFields(typeof(EPExpenseClaimDetails.refNbr))]
        public PXSelect<EPExpenseClaimDetails, Where<EPExpenseClaimDetails.claimDetailID, Equal<Current<EPExpenseClaimDetails.claimDetailID>>>> CurrentClaimDetails;
        [PXCopyPasteHiddenView]
        public PXSelect<CurrencyInfo> currencyinfo;
        [PXCopyPasteHiddenView]
        public PXSetup<EPSetup> epsetup;
        [PXCopyPasteHiddenView]
        public PXSelect<Currency> currency;
        [PXCopyPasteHiddenView]
        public PXSelect<CurrencyList, Where<CurrencyList.isActive, Equal<True>>> currencyList;
        [PXCopyPasteHiddenView]
        public PXSetup<GL.Company> comapny;
        [PXViewName(CR.Messages.Employee)]
        [PXCopyPasteHiddenView]
        public PXSetup<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaimDetails.employeeID>>>> Employee;

        [CRReference(typeof(Select<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaimDetails.employeeID>>>>))]
        [CRDefaultMailTo(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<EPEmployee.defContactID>>>>))]
        public CRActivityList<EPExpenseClaimDetails>
            Activity;
        [PXCopyPasteHiddenView]
        [PXViewName(Messages.Approval)]
        public EPApprovalAutomation<EPExpenseClaimDetails, EPExpenseClaimDetails.approved, EPExpenseClaimDetails.rejected, EPExpenseClaimDetails.hold, EPSetup> Approval;
        [PXCopyPasteHiddenView]
        public PXSelect<Contract, Where<Contract.contractID, Equal<Current<EPExpenseClaimDetails.contractID>>>> CurrentContract;
        #endregion

        #region Action

        public PXAction<EPExpenseClaimDetails> action;
        [PXUIField(DisplayName = Messages.Actions, MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable Action(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public ToggleCurrency<EPExpenseClaimDetails> CurrencyView;

        #endregion

        public ExpenseClaimDetailEntry()
        {
            FieldDefaulting.AddHandler<InventoryItem.stkItem>((sender, e) => { if (e.Row != null) e.NewValue = false; });
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetails.contractID>(ClaimDetails.Cache, null, PXAccess.FeatureInstalled<CS.FeaturesSet.contractManagement>() || PXAccess.FeatureInstalled<CS.FeaturesSet.projectModule>());
        }


        #region Handler

        protected void EPExpenseClaimDetails_ExpenseDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
            if (row == null || e.NewValue != null)
                return;
            e.NewValue = Accessinfo.BusinessDate;
        }

        protected virtual void EPExpenseClaimDetails_ExpenseAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetDefaultExt<EPExpenseClaimDetails.contractID>(e.Row);
        }

        protected virtual void EPExpenseClaimDetails_Hold_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID != null;
            e.Cancel = true;
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
                EPEmployee employee = (EPEmployee)PXSelect<EPEmployee>.Search<EPEmployee.bAccountID>(this, e.Row != null ? ((EPExpenseClaimDetails)e.Row).EmployeeID : null);

                Location customerLocation = (Location)PXSelectorAttribute.Select<EPExpenseClaimDetails.customerLocationID>(sender, e.Row);

                int? employee_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.expenseSubID>(employee);
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
                    EPEmployee employee = (EPEmployee)PXSelect<EPEmployee>.Search<EPEmployee.bAccountID>(this, e.Row != null ? ((EPExpenseClaimDetails)e.Row).EmployeeID : null);

                    int? employee_SubID = (int?)Caches[typeof(EPEmployee)].GetValue<EPEmployee.salesSubID>(employee);
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


        protected virtual void EPExpenseClaimDetails_ExpenseDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            CurrencyInfoAttribute.SetEffectiveDate<EPExpenseClaimDetails.expenseDate>(cache, e);
        }

        protected virtual void EPExpenseClaimDetails_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<EPExpenseClaimDetails.curyInfoID>(cache, e.Row);
            if (info != null)
            {
                ((EPExpenseClaimDetails)e.Row).CuryID = info.CuryID;
            }
        }

        protected virtual void EPExpenseClaimDetails_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;

            if (row != null)
            {
                EPExpenseClaim claim = (EPExpenseClaim)PXSelect<EPExpenseClaim,Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaimDetails.refNbr>>>>.SelectSingleBound(this, new object[] { null }, row.RefNbr);
               
                bool enabledApprovalReceipt = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID != null;              
                bool enabledEditReceipt = row.Hold == true || !enabledApprovalReceipt;
                bool enabledRefNbr = true;
                bool enabledEmployeeAndBranch = enabledEditReceipt && !(row.ClaimCuryInfoID == null && cache.AllowUpdate && !string.IsNullOrEmpty(row.RefNbr));
                bool enabledFinancialDetails = (row.Rejected != true) && (row.Released != true);

                if (claim != null)
                {
                    bool enabledEditClaim = (row.HoldClaim == true);
                    enabledEditReceipt = enabledEditReceipt && enabledEditClaim;
                    EPExpenseClaimDetails receiptnBase = (EPExpenseClaimDetails)PXSelectReadonly<EPExpenseClaimDetails, Where<EPExpenseClaimDetails.claimDetailID, Equal<Required<EPExpenseClaimDetails.claimDetailID>>>>.SelectSingleBound(this, new object[] { null }, row.ClaimDetailID);                    
                    enabledRefNbr = (receiptnBase?.RefNbr == null);
                    enabledEmployeeAndBranch = false;
                    enabledFinancialDetails = enabledFinancialDetails && enabledEditClaim;
                }

                Approval.AllowSelect = enabledApprovalReceipt;
                Delete.SetEnabled(enabledEditReceipt && claim == null);
                PXUIFieldAttribute.SetEnabled(cache, row, enabledEditReceipt);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.claimDetailID>(cache, row, true);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.refNbr>(cache, row, enabledRefNbr);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.employeeID>(cache, row, enabledEmployeeAndBranch);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.branchID>(cache, row, enabledEmployeeAndBranch);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.expenseAccountID>(cache, row, enabledFinancialDetails);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.expenseSubID>(cache, row, enabledFinancialDetails);
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.salesAccountID>(cache, row, enabledFinancialDetails && (row.Billable==true));
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.salesSubID>(cache, row, enabledFinancialDetails && (row.Billable == true));
                PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.taxCategoryID>(cache, row, enabledFinancialDetails);
                action.SetEnabled("Submit", cache.GetStatus(row) != PXEntryStatus.Inserted && row.Hold == true);
                
                if (row.ContractID != null && (bool)row.Billable && row.TaskID != null)
                {
                    PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.TaskID);
                    if (task != null && !(bool)task.VisibleInAP)
                        cache.RaiseExceptionHandling<EPExpenseClaimDetails.taskID>(e.Row, task.TaskCD, new PXSetPropertyException(PM.Messages.TaskInvisibleInModule, task.TaskCD, GL.BatchModule.AP));
                }

                CurrencyInfo info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<EPExpenseClaimDetails.curyInfoID>>>>.SelectSingleBound(this, new object[] { row });
                if (info != null && info.CuryRateTypeID != null && info.CuryEffDate != null && row.ExpenseDate != null && info.CuryEffDate < row.ExpenseDate)
                {
                    CurrencyRateType ratetype = (CurrencyRateType)PXSelectorAttribute.Select<CurrencyInfo.curyRateTypeID>(currencyinfo.Cache, info);
                    if (ratetype != null && ratetype.RateEffDays > 0 &&
                        ((TimeSpan)(row.ExpenseDate - info.CuryEffDate)).Days > ratetype.RateEffDays)
                    {
                        PXRateIsNotDefinedForThisDateException exc = new PXRateIsNotDefinedForThisDateException(info.CuryRateTypeID, info.BaseCuryID, info.CuryID, (DateTime)row.ExpenseDate);
                        cache.RaiseExceptionHandling<EPExpenseClaimDetails.expenseDate>(e.Row, ((EPExpenseClaimDetails)e.Row).ExpenseDate, exc);
                    }
                }
                string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyID>(currencyinfo.Cache, info);
                if (string.IsNullOrEmpty(message) && info != null && info.CuryRate == null)
                    message = CM.Messages.RateNotFound;
                if (string.IsNullOrEmpty(message))
                    cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyID>(e.Row, null, null);
                else
                    cache.RaiseExceptionHandling<EPExpenseClaimDetails.curyID>(e.Row, null, new PXSetPropertyException(message, PXErrorLevel.Warning));
            }
        }

        protected virtual void EPExpenseClaimDetails_RowPersisted(PXCache cache, PXRowPersistedEventArgs e)
        {
            EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
            if (!string.IsNullOrEmpty(row.RefNbr) && e.TranStatus == PXTranStatus.Completed)
            {
                ExpenseClaimEntry expenseClaimEntry = CreateInstance<ExpenseClaimEntry>();
                expenseClaimEntry.ExpenseClaim.Current = expenseClaimEntry.ExpenseClaim.Search<EPExpenseClaim.refNbr>(row.RefNbr);
                if (expenseClaimEntry.ExpenseClaim.Current != null)
                {
                    expenseClaimEntry.currencyinfo.Current = expenseClaimEntry.currencyinfo.Search<EPExpenseClaim.curyInfoID>(expenseClaimEntry.ExpenseClaim.Current.CuryInfoID);
                    expenseClaimEntry.SubmitDetail(row);
                    expenseClaimEntry.Actions.PressSave();
                    Cancel.Press();
                }
            }
        }

        protected virtual void EPExpenseClaimDetails_EmployeeID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<EPExpenseClaim.curyInfoID>(cache, e.Row);

            if (info != null)
            {
                ((EPExpenseClaimDetails)e.Row).CuryID = info.CuryID;
            }

        }


        protected virtual void EPExpenseClaimDetails_InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;

            InventoryItem item = PXSelectorAttribute.Select<InventoryItem.inventoryID>(cache, row) as InventoryItem;
            decimal curyStdCost;
            if (item != null && currencyinfo.Current != null && currencyinfo.Current.CuryRate != null)
                PXCurrencyAttribute.CuryConvCury<EPExpenseClaimDetails.curyInfoID>(cache, e.Row, item.StdCost.Value, out curyStdCost, true);
            else
                curyStdCost = 0m;
            cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(row, curyStdCost);
        }

        protected virtual void EPExpenseClaimDetails_CuryEmployeePart_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e) //\
        {
            Decimal? newVal = e.NewValue as Decimal?;
            if (newVal < 0)
                throw new PXSetPropertyException(CS.Messages.FieldShouldNotBeNegative, PXUIFieldAttribute.GetDisplayName<EPExpenseClaimDetails.curyEmployeePart>(cache));
        }

        protected virtual void EPExpenseClaimDetails_CustomerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            EPExpenseClaimDetails row = e.Row as EPExpenseClaimDetails;

            if (row?.CustomerID == null)
                cache.SetValueExt<EPExpenseClaimDetails.customerLocationID>(row, null);
        }
        #endregion

        #region CurrencyInfo events
        protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            EPEmployee employee = (EPEmployee)PXSelect<EPEmployee>.Search<EPEmployee.bAccountID>(this, ClaimDetails.Current != null ? ClaimDetails.Current.EmployeeID : null);
            if (employee != null && employee.CuryID != null)
            {
                e.NewValue = employee.CuryID;
                e.Cancel = true;
            }
            else if (comapny.Current != null)
            {
                e.NewValue = comapny.Current.BaseCuryID;
            }
        }

        protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            if (ClaimDetails.Current != null && ClaimDetails.Current.EmployeeID != null)
            {
                EPEmployee employee = (EPEmployee)PXSelect<EPEmployee>.Search<EPEmployee.bAccountID>(this, ClaimDetails.Current != null ? ClaimDetails.Current.EmployeeID : null);
                if (employee != null && employee.CuryRateTypeID != null)
                {
                    e.NewValue = employee.CuryRateTypeID;
                    e.Cancel = true;
                }
            }
        }

        protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            if (ClaimDetails.Current != null)
            {
                e.NewValue = ClaimDetails.Current.ExpenseDate;
                e.Cancel = true;
            }
        }

        protected virtual void CurrencyInfo_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            CurrencyInfo info = e.Row as CurrencyInfo;
            if (info != null && ClaimDetails.Current != null)
            {
                bool rateenabled = info.AllowUpdate(ClaimDetails.Cache) && ClaimDetails.Current.EmployeeID != null;
                if (rateenabled)
                {
                    CurrencyList curyList = (CurrencyList)PXSelectorAttribute.Select<CurrencyInfo.curyID>(cache, info);
                    if (curyList != null && curyList.IsFinancial == true)
                    {
                        EPEmployee employee = (EPEmployee)PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaimDetails.employeeID>>>>.SelectSingleBound(this, new object[] { ClaimDetails.Current });
                        rateenabled = employee != null && employee.AllowOverrideRate == true;
                    }
                }

                PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(cache, info, rateenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(cache, info, rateenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(cache, info, rateenabled);
                PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(cache, info, rateenabled);
            }
        }

        protected String _CuryID;
        [PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
        [PXDefault]
        [PXUIField(DisplayName = "Currency", ErrorHandling = PXErrorHandling.Never)]
        [PXSelector(typeof(CurrencyList.curyID))]
        [CM.CurrencyInfo.CuryID]
        protected virtual void CurrencyInfo_CuryId_CacheAttached(PXCache cache)
        {
        }

        #endregion

        #region DAC Overrides

        [GL.Branch()]
        protected virtual void EPExpenseClaimDetails_BranchID_CacheAttached(PXCache cache)
        {
        }


        [PXDBInt()]
        [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        [PXSubordinateAndWingmenSelector]
        [PXUIField(DisplayName = "Claimed by", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Switch<Case<Where<
            Selector<EPExpenseClaimDetails.refNbr, EPExpenseClaim.employeeID>, IsNotNull,
                And<Current2<EPExpenseClaimDetails.employeeID>, IsNull>>,
            Selector<EPExpenseClaimDetails.refNbr, EPExpenseClaim.employeeID>>, EPExpenseClaimDetails.employeeID>))]
        [PXUIEnabled(typeof(Where<EPExpenseClaimDetails.claimCuryInfoID, IsNull>))]
        protected virtual void EPExpenseClaimDetails_employeeID_CacheAttached(PXCache cache)
        {
        }

        [CurrencyInfo(ModuleCode = "EP", CuryIDField = "curyID", CuryDisplayName = "Currency")]
        [PXDBLong()]
        protected virtual void EPExpenseClaimDetails_CuryInfoID_CacheAttached(PXCache cache)
        {
        }

        [PXDBLong()]
        protected virtual void EPExpenseClaimDetails_ClaimCuryInfoID_CacheAttached(PXCache cache)
        {
        }

        protected String _RefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Expense Claim", Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIEnabled(typeof(Where<EPExpenseClaimDetails.claimCuryInfoID, IsNull>))]
        [PXSelector(typeof(Search<EPExpenseClaim.refNbr,
                                Where<Current<EPExpenseClaimDetails.holdClaim>, Equal<False>,
                                    Or<EPExpenseClaim.hold, Equal<True>,
                                    And2<Where<EPExpenseClaim.employeeID, Equal<Current2<EPExpenseClaimDetails.employeeID>>,
                                    Or<Current2<EPExpenseClaimDetails.employeeID>, IsNull>>,
                                        And<Where<Current<EPExpenseClaimDetails.rejected>, Equal<False>>>>>>>),
                    new Type[] {typeof(EPExpenseClaim.refNbr),
                                typeof(EPExpenseClaim.employeeID),
                                typeof(EPExpenseClaim.locationID),
                                typeof(EPExpenseClaim.docDate),
                                typeof(EPExpenseClaim.docDesc),
                                typeof(EPExpenseClaim.curyID),
                                typeof(EPExpenseClaim.curyDocBal)},
                    DescriptionField = typeof(EPExpenseClaim.docDesc))]
        protected virtual void EPExpenseClaimDetails_RefNbr_CacheAttached(PXCache cache)
        {
        }

        #endregion

        #region EPApproval Cahce Attached
        [PXDBDate]
        [PXDefault(typeof(EPExpenseClaimDetails.expenseDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt]
        [PXDefault(typeof(EPExpenseClaimDetails.employeeID), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
        {
        }

        [PXDBString(60, IsUnicode = true)]
        [PXDefault(typeof(EPExpenseClaimDetails.tranDesc), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
        {
        }

        [PXDBLong]
        [CurrencyInfo(typeof(EPExpenseClaimDetails.curyInfoID))]
        protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
        {
        }

        [PXDBDecimal(4)]
        [PXDefault(typeof(EPExpenseClaimDetails.curyTranAmt), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
        {
        }

        [PXDBDecimal(4)]
        [PXDefault(typeof(EPExpenseClaimDetails.tranAmt), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region EPSetup Cahce Attached
        [PXInt]
        [PXDBScalar(typeof(Search<EPSetup.claimDetailsAssignmentMapID>))]
        protected virtual void EPSetup_AssignmentMapID_CacheAttached(PXCache cache)
        {
        }

        [PXInt]
        [PXDBScalar(typeof(Search<EPSetup.claimDetailsAssignmentNotificationID>))]
        protected virtual void EPSetup_AssignmentNotificationID_CacheAttached(PXCache cache)
        {
        }

        [PXBool]
        [PXDefault(true)]
        [PXFormula(typeof(IIf<FeatureInstalled<CS.FeaturesSet.approvalWorkflow>, True, False>))]
        protected virtual void EPSetup_IsActive_CacheAttached(PXCache cache)
        {
        }
        #endregion
    }
}
