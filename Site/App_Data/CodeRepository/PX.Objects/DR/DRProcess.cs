using System;
using System.Collections.Generic;
using System.Linq;

using PX.Common;
using PX.Data;

using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.DR.Descriptor;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.AP;
using PX.Objects.EP;

namespace PX.Objects.DR
{
	public class DRProcess 
		: PXGraph<DRProcess>, IDREntityStorage, IBusinessAccountProvider, IInventoryItemProvider
	{		
		public PXSelect<DRSchedule> Schedule;
		public PXSelect<DRScheduleDetail> ScheduleDetail;
		public PXSelect<DRScheduleTran> Transactions;
		public PXSelect<DRExpenseBalance> ExpenseBalance;
		public PXSelect<DRExpenseProjectionAccum> ExpenseProjection;
		public PXSelect<DRRevenueBalance> RevenueBalance;
		public PXSelect<DRRevenueProjectionAccum> RevenueProjection;

		protected virtual void DRScheduleDetail_RefNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void DRScheduleDetail_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void DRSchedule_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void DRScheduleDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as DRScheduleDetail;

			if (row == null)
				return;

			PXDefaultAttribute.SetPersistingCheck<DRScheduleDetail.defCode>(sender, row, row.IsResidual == true ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
		}

		public List<Batch> RunRecognition(SortedList<string, PX.Objects.DR.DRRecognition.DRBatch> list, DateTime? recDate)
		{			
			List<Batch> batchlist = new List<Batch>(list.Count);

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				foreach (KeyValuePair<string, PX.Objects.DR.DRRecognition.DRBatch> kv in list)
				{
					JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
					je.FieldVerifying.AddHandler<GLTran.referenceID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
					je.FieldVerifying.AddHandler<GLTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
					je.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
					je.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			
					Batch newbatch = new Batch();
					newbatch.Module = BatchModule.DR;
					newbatch.Status = "U";
					newbatch.Released = true;
					newbatch.Hold = false;
					newbatch.FinPeriodID = kv.Value.FinPeriod;
					newbatch.TranPeriodID = kv.Value.FinPeriod;
                    newbatch.DateEntered = recDate;
					je.BatchModule.Insert(newbatch);

					List<DRScheduleTran> drTranList = new List<DRScheduleTran>();
					foreach (PX.Objects.DR.DRRecognition.DRTranKey key in kv.Value.Trans)
					{
						DRScheduleTran drTran = PXSelect<DRScheduleTran, 
							Where<DRScheduleTran.scheduleID, Equal<Required<DRScheduleTran.scheduleID>>,
							And<DRScheduleTran.componentID, Equal<Required<DRScheduleTran.componentID>>,
							And<DRScheduleTran.lineNbr, Equal<Required<DRScheduleTran.lineNbr>>>>>>.Select(je, key.ScheduleID, key.ComponentID, key.LineNbr);
						drTranList.Add(drTran);
						DRSchedule schedule = GetDeferralSchedule(key.ScheduleID);
						DRScheduleDetail scheduleDetail = GetScheduleDetailForComponent(key.ScheduleID, key.ComponentID);
						
						GLTran tran = new GLTran();
						tran.SummPost = false;
						tran.TranType = schedule.DocType;
						tran.RefNbr = scheduleDetail.ScheduleID.ToString();
						if ( scheduleDetail.BAccountID != null && scheduleDetail.BAccountID != DRScheduleDetail.EmptyBAccountID )
							tran.ReferenceID = scheduleDetail.BAccountID;
						if ( scheduleDetail.ComponentID != null && scheduleDetail.ComponentID != DRScheduleDetail.EmptyComponentID )
							tran.InventoryID = scheduleDetail.ComponentID;

						var isReversed = IsReversed(scheduleDetail);

						if (drTran.Amount >= 0 && !isReversed ||
							drTran.Amount < 0 && isReversed)
						{
							tran.AccountID = scheduleDetail.DefAcctID;
							tran.SubID = scheduleDetail.DefSubID;
							tran.ReclassificationProhibited = true;
						}
						else
						{
							tran.AccountID = drTran.AccountID;
							tran.SubID = drTran.SubID;
						}

						tran.BranchID = drTran.BranchID;
						tran.CuryDebitAmt = scheduleDetail.Module == BatchModule.AR ? Math.Abs(drTran.Amount.Value) : 0;
						tran.DebitAmt = scheduleDetail.Module == BatchModule.AR ? Math.Abs(drTran.Amount.Value) : 0;
						tran.CuryCreditAmt = scheduleDetail.Module == BatchModule.AR ? 0 : Math.Abs(drTran.Amount.Value);
						tran.CreditAmt = scheduleDetail.Module == BatchModule.AR ? 0 : Math.Abs(drTran.Amount.Value);
						tran.TranDesc = schedule.TranDesc;
						tran.Released = true;
                        tran.TranDate = drTran.RecDate;
						tran.ProjectID = schedule.ProjectID;
						tran.TaskID = schedule.TaskID;
						tran.TranLineNbr = drTran.LineNbr;

						je.GLTranModuleBatNbr.Insert(tran);

						GLTran tran2 = new GLTran();
						tran2.SummPost = false;
						tran2.TranType = schedule.DocType;
						tran2.RefNbr = scheduleDetail.ScheduleID.ToString();
						if (scheduleDetail.BAccountID != null && scheduleDetail.BAccountID != DRScheduleDetail.EmptyBAccountID)
							tran2.ReferenceID = scheduleDetail.BAccountID;
						if (scheduleDetail.ComponentID != null && scheduleDetail.ComponentID != DRScheduleDetail.EmptyComponentID)
							tran2.InventoryID = scheduleDetail.ComponentID;

						if (drTran.Amount >= 0 && !isReversed ||
							drTran.Amount < 0 && isReversed)
						{
							tran2.AccountID = drTran.AccountID;
							tran2.SubID = drTran.SubID;
						}
						else
						{
							tran2.AccountID = scheduleDetail.DefAcctID;
							tran2.SubID = scheduleDetail.DefSubID;
							tran2.ReclassificationProhibited = true;
						}


						tran2.BranchID = drTran.BranchID;
						tran2.CuryDebitAmt = scheduleDetail.Module == BatchModule.AR ? 0 : Math.Abs(drTran.Amount.Value);
                        tran2.DebitAmt = scheduleDetail.Module == BatchModule.AR ? 0 : Math.Abs(drTran.Amount.Value);
						tran2.CuryCreditAmt = scheduleDetail.Module == BatchModule.AR ? Math.Abs(drTran.Amount.Value) : 0;
						tran2.CreditAmt = scheduleDetail.Module == BatchModule.AR ? Math.Abs(drTran.Amount.Value) : 0;
						tran2.TranDesc = schedule.TranDesc;
						tran2.Released = true;
                        tran2.TranDate = drTran.RecDate;
						tran2.ProjectID = schedule.ProjectID;
						tran2.TaskID = schedule.TaskID;
						tran2.TranLineNbr = drTran.LineNbr;

						je.GLTranModuleBatNbr.Insert(tran2);

					}
					je.Save.Press();


					batchlist.Add(je.BatchModule.Current);

					foreach ( DRScheduleTran drTran in drTranList )
					{
						drTran.BatchNbr = je.BatchModule.Current.BatchNbr;
						drTran.Status = DRScheduleTranStatus.Posted;
						drTran.TranDate = je.BatchModule.Current.DateEntered;
						drTran.FinPeriodID = je.BatchModule.Current.FinPeriodID;//Bug: 20528
						Transactions.Update(drTran);

						DRScheduleDetail scheduleDetail = GetScheduleDetailForComponent(drTran.ScheduleID, drTran.ComponentID);

						decimal tranAmt = drTran.Amount ?? 0m;
						decimal detailDefAmt = scheduleDetail.DefAmt ?? 0m;

						scheduleDetail.DefAmt -= Math.Sign(tranAmt) * Math.Min(Math.Abs(tranAmt), Math.Abs(detailDefAmt));
						scheduleDetail.LastRecFinPeriodID = drTran.FinPeriodID;

						if (scheduleDetail.DefAmt == 0)
						{
							scheduleDetail.Status = DRScheduleStatus.Closed;
							scheduleDetail.CloseFinPeriodID = drTran.FinPeriodID;
							scheduleDetail.IsOpen = false;
						}

						ScheduleDetail.Update(scheduleDetail);
																		
						DRDeferredCode deferralCode = PXSelect<
							DRDeferredCode, 
							Where<DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>
							.Select(this, scheduleDetail.DefCode);
												
						UpdateBalance(drTran, scheduleDetail, deferralCode.AccountType);
						
					}
					
				}

				this.Actions.PressSave();

				ts.Complete();
			}
			
			return batchlist;

		}

		/// <summary>
		/// Creates a <see cref="DRSchedule"/> record with multiple <see cref="DRScheduleDetail"/> 
		/// records depending on the original AR document and document line transactions, as well as
		/// on the deferral code parameters.
		/// </summary>
		/// <param name="arTransaction">An AR document line transaction that corresponds to the original document.</param>
		/// <param name="deferralCode">The deferral code to be used in schedule details.</param>
		/// <param name="originalDocument">Original AR document.</param>
		/// <remarks>
		/// Records are created only in the cache. You have to persist them manually.
		/// </remarks>
		public virtual void CreateSchedule(ARTran arTransaction, DRDeferredCode deferralCode, ARRegister originalDocument, decimal amount, bool isDraft)
		{
			if (arTransaction == null) throw new ArgumentNullException(nameof(arTransaction));
			if (deferralCode == null) throw new ArgumentNullException(nameof(deferralCode));

			InventoryItem inventoryItem = GetInventoryItem(arTransaction.InventoryID);

			ARSetup arSetup = PXSelect<ARSetup>.Select(this);

			DRSchedule existingSchedule = PXSelect<
				DRSchedule,
				Where<
					DRSchedule.module, Equal<BatchModule.moduleAR>,
					And<DRSchedule.docType, Equal<Required<ARTran.tranType>>,
					And<DRSchedule.refNbr, Equal<Required<ARTran.refNbr>>,
					And<DRSchedule.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>>
				.Select(this, arTransaction.TranType, arTransaction.RefNbr, arTransaction.LineNbr);

			if (arTransaction.DefScheduleID == null)
			{
				decimal transactionQuantityInBaseUnits = (arTransaction.Qty ?? 0);

				if (arTransaction.InventoryID != null)
				{
					transactionQuantityInBaseUnits = INUnitAttribute.ConvertToBase(
						this.Caches[typeof(ARTran)], 
						arTransaction.InventoryID,
						arTransaction.UOM, 
						(arTransaction.Qty ?? 0), 
						INPrecision.QUANTITY);
				}

				decimal? compoundDiscountRate = (1.0m - (arTransaction.DiscPctDR ?? 0.0m) * 0.01m);

				decimal unitPriceDR;

				PXCurrencyAttribute.CuryConvBase<ARTran.curyInfoID>(
					Caches[typeof(ARTran)], 
					arTransaction, 
					arTransaction.CuryUnitPriceDR ?? 0.0m, 
					out unitPriceDR);
				
				DRScheduleParameters scheduleParameters = GetScheduleParameters(originalDocument, arTransaction);
				ScheduleCreator scheduleCreator = new ScheduleCreator(
					this, new ARSubaccountProvider(this), this, this,
					roundingFunction: x => PXCurrencyAttribute.BaseRound(this, x), 
					branchID: arTransaction.BranchID, isDraft: isDraft);

				if (existingSchedule == null)
				{
					scheduleCreator.CreateOriginalSchedule(
						scheduleParameters, 
						deferralCode, 
						inventoryItem, 
						new AccountSubaccountPair(arTransaction.AccountID, arTransaction.SubID), 
						amount, 
						unitPriceDR, 
						compoundDiscountRate, 
						transactionQuantityInBaseUnits);
				}
				else
				{
					scheduleCreator.ReevaluateSchedule(
						existingSchedule, 
						scheduleParameters, 
						deferralCode, 
						amount,
						attachedToOriginalSchedule: false);
				}
			}
			else
			{
				if (deferralCode.Method == DeferredMethodType.CashReceipt)
				{
					if (originalDocument.DocType == ARDocType.CreditMemo)
					{
						UpdateOriginalSchedule(arTransaction, deferralCode, amount, originalDocument.DocDate, originalDocument.FinPeriodID, originalDocument.CustomerID, originalDocument.CustomerLocationID);
					}
				}
				else
				{
                    if (originalDocument.DocType == ARDocType.CreditMemo)
					{
						if (existingSchedule == null)
						{
							CreateRelatedSchedule(arTransaction.BranchID, arTransaction.DefScheduleID, BatchModule.AR, arTransaction.TranType, arTransaction.RefNbr, arTransaction.LineNbr, amount, originalDocument.DocDate, originalDocument.CustomerID, originalDocument.CustomerLocationID, deferralCode, inventoryItem, originalDocument.FinPeriodID, arTransaction.AccountID, arTransaction.SubID, arTransaction.TranDesc, arTransaction.ProjectID, arTransaction.TaskID, isDraft, true);
						}
						else
						{
							DRScheduleParameters scheduleParameters = GetScheduleParameters(originalDocument, arTransaction);
							ScheduleCreator scheduleCreator = new ScheduleCreator(this, new ARSubaccountProvider(this), this, this,
								roundingFunction: x => PXCurrencyAttribute.BaseRound(this, x), branchID: arTransaction.BranchID, isDraft: false);

							scheduleCreator.ReevaluateSchedule(
								existingSchedule, 
								scheduleParameters, 
								deferralCode, 
								amount,
								attachedToOriginalSchedule: true);
						}
					}
                    else if (originalDocument.DocType == ARDocType.DebitMemo)
                    {
						if (existingSchedule == null)
						{
							CreateRelatedSchedule(arTransaction.BranchID, arTransaction.DefScheduleID, BatchModule.AR, arTransaction.TranType, arTransaction.RefNbr, arTransaction.LineNbr, amount, originalDocument.DocDate, originalDocument.CustomerID, originalDocument.CustomerLocationID, deferralCode, inventoryItem, originalDocument.FinPeriodID, arTransaction.AccountID, arTransaction.SubID, arTransaction.TranDesc, arTransaction.ProjectID, arTransaction.TaskID, isDraft, false);
						}
						else
						{
							DRScheduleParameters scheduleParameters = GetScheduleParameters(originalDocument, arTransaction);
							ScheduleCreator scheduleCreator = new ScheduleCreator(this, new ARSubaccountProvider(this), this, this,
								roundingFunction: x => PXCurrencyAttribute.BaseRound(this, x), branchID: arTransaction.BranchID, isDraft: false);

							scheduleCreator.ReevaluateSchedule(
								existingSchedule,
								scheduleParameters,
								deferralCode,
								amount,
								attachedToOriginalSchedule: true);
						}
                    }
				}
			}
		}

		/// <summary>
		/// Creates DRSchedule record with multiple DRScheduleDetail records depending on the DeferredCode schedule.
		/// </summary>
		/// <param name="tran">AP Transaction</param>
		/// <param name="defCode">Deferred Code</param>
		/// <param name="document">AP Invoice</param>
		/// <remarks>
		/// Records are created only in the Cache. You have to manually call Perist method.
		/// </remarks>
		public virtual void CreateSchedule(APTran tran, DRDeferredCode defCode, APInvoice document, decimal amount, bool isDraft)
		{
			if (tran == null) throw new ArgumentNullException(nameof(tran));
			if (defCode == null) throw new ArgumentNullException(nameof(defCode));

			InventoryItem inventoryItem = GetInventoryItem(tran.InventoryID);

			APSetup apSetup = PXSelect<APSetup>.Select(this);

			DRSchedule existingSchedule = PXSelect<
				DRSchedule,
				Where<
					DRSchedule.module, Equal<BatchModule.moduleAP>,
					And<DRSchedule.docType, Equal<Required<APTran.tranType>>,
					And<DRSchedule.refNbr, Equal<Required<APTran.refNbr>>,
					And<DRSchedule.lineNbr, Equal<Required<APTran.lineNbr>>>>>>>
				.Select(this, tran.TranType, tran.RefNbr, tran.LineNbr);

			if (tran.DefScheduleID == null)
			{
				decimal quantityInBaseUnit = tran.Qty ?? 0;

				if (tran.InventoryID != null)
				{
					quantityInBaseUnit = INUnitAttribute.ConvertToBase(
						Caches[typeof(APTran)], 
						tran.InventoryID, 
						tran.UOM,
						tran.Qty ?? 0, 
						INPrecision.QUANTITY);
				}

				DRScheduleParameters scheduleParams = GetScheduleParameters(document, tran);
				ScheduleCreator creator = new ScheduleCreator(this, new APSubaccountProvider(this), this, this,
					roundingFunction: x => PXCurrencyAttribute.BaseRound(this, x), 
					branchID: tran.BranchID, 
					isDraft: isDraft);

				if (existingSchedule != null)
				{
					creator.ReevaluateSchedule(
						existingSchedule, 
						scheduleParams, 
						defCode, 
						amount, 
						attachedToOriginalSchedule: false);
				}
				else
				{
					creator.CreateOriginalSchedule(
						scheduleParams, 
						defCode, 
						inventoryItem, 
						new AccountSubaccountPair(tran.AccountID, tran.SubID), 
						amount, 
						null, 
						null, 
						quantityInBaseUnit);
				}
			}
			else
			{
				if (document.DocType == APDocType.DebitAdj)
				{
					if (existingSchedule != null)
					{
						DRScheduleParameters scheduleParams = GetScheduleParameters(document, tran);
						ScheduleCreator creator = new ScheduleCreator(this, new APSubaccountProvider(this), this, this,
							roundingFunction: x => PXCurrencyAttribute.BaseRound(this, x), 
							branchID: tran.BranchID, 
							isDraft: isDraft);

						creator.ReevaluateSchedule(
							existingSchedule,
							scheduleParams,
							defCode,
							amount,
							attachedToOriginalSchedule: true);
					}
					else
					{
						CreateRelatedSchedule(tran.BranchID, tran.DefScheduleID, BatchModule.AP, tran.TranType, tran.RefNbr, tran.LineNbr,
							amount, document.DocDate, document.VendorID, document.VendorLocationID, defCode, inventoryItem, document.FinPeriodID,
							tran.AccountID, tran.SubID, tran.TranDesc, tran.ProjectID, tran.TaskID, isDraft, true);
					}
				}
			}
		}

        /// <summary>
        /// Rebuilds DR Balance History Tables.
        /// </summary>
        /// <param name="item">Type of Balance to rebuild</param>
        public virtual void RunIntegrityCheck(DRBalanceValidation.DRBalanceType item)
        {
	        string requiredModule;

            switch (item.AccountType)
            {
                case DeferredAccountType.Income:
		            requiredModule = BatchModule.AR;
                    break;

				case DeferredAccountType.Expense:
		            requiredModule = BatchModule.AP;
                    break;

				default:
                    throw new PXException(
						Messages.InvalidAccountType, 
						DeferredAccountType.Expense, 
						DeferredAccountType.Income, 
						item.AccountType);
            }

			PXSelectBase<DRScheduleTran> incomingTransactions = new PXSelectJoin<
				DRScheduleTran,
				InnerJoin<DRScheduleDetail,
					On<DRScheduleDetail.scheduleID, Equal<DRScheduleTran.scheduleID>,
					And<DRScheduleDetail.componentID, Equal<DRScheduleTran.componentID>>>>,
				Where<
					DRScheduleTran.lineNbr, Equal<DRScheduleDetail.creditLineNbr>,
					And<DRScheduleDetail.module, Equal<Required<DRScheduleDetail.module>>>>>
				(this);

			foreach (PXResult<DRScheduleTran, DRScheduleDetail> detailAndTransaction in 
				incomingTransactions.Select(requiredModule))
			{
				DRScheduleTran deferralTransaction = detailAndTransaction;
				DRScheduleDetail scheduleDetail = detailAndTransaction;

				InitBalance(deferralTransaction, scheduleDetail, item.AccountType);
			}

			PXSelectBase<DRScheduleTran> openTransactions = new PXSelectJoin<
				DRScheduleTran,
				InnerJoin<DRScheduleDetail,
					On<DRScheduleDetail.scheduleID, Equal<DRScheduleTran.scheduleID>,
					And<DRScheduleDetail.componentID, Equal<DRScheduleTran.componentID>>>>,
				Where<
					DRScheduleTran.lineNbr, NotEqual<DRScheduleDetail.creditLineNbr>,
					And<DRScheduleDetail.module, Equal<Required<DRScheduleDetail.module>>,
					And<DRScheduleTran.status, Equal<DRScheduleTranStatus.OpenStatus>>>>>
				(this);

			foreach (PXResult<DRScheduleTran, DRScheduleDetail> detailAndTransaction in 
				openTransactions.Select(requiredModule))
			{
				DRScheduleTran deferralTransaction = detailAndTransaction;
				DRScheduleDetail scheduleDetail = detailAndTransaction;

				UpdateBalanceProjection(deferralTransaction, scheduleDetail, item.AccountType);
			}

			PXSelectBase<DRScheduleTran> postedTransactions = new PXSelectJoin<
				DRScheduleTran,
				InnerJoin<DRScheduleDetail,
					On<DRScheduleDetail.scheduleID, Equal<DRScheduleTran.scheduleID>,
					And<DRScheduleDetail.componentID, Equal<DRScheduleTran.componentID>>>>,
				Where<
					DRScheduleTran.lineNbr, NotEqual<DRScheduleDetail.creditLineNbr>,
					And<DRScheduleDetail.module, Equal<Required<DRScheduleDetail.module>>,
					And<DRScheduleTran.status, Equal<DRScheduleTranStatus.PostedStatus>>>>>
				(this);

			foreach (PXResult<DRScheduleTran, DRScheduleDetail> detailAndTransaction in 
				postedTransactions.Select(requiredModule))
			{
				DRScheduleTran deferralTransaction = detailAndTransaction;
				DRScheduleDetail scheduleDetail = detailAndTransaction;

				UpdateBalance(deferralTransaction, scheduleDetail, item.AccountType, isIntegrityCheck: true);
			}

			using (new PXConnectionScope())
			{
				using (PXTransactionScope transactionScope = new PXTransactionScope())
				{
					if (requiredModule == BatchModule.AR)
					{
						PXDatabase.Delete<DRRevenueBalance>();
						PXDatabase.Delete<DRRevenueProjection>();
					}
					else if (requiredModule == BatchModule.AP)
					{
						PXDatabase.Delete<DRExpenseBalance>();
						PXDatabase.Delete<DRExpenseProjection>();
					}

					this.Actions.PressSave();
					transactionScope.Complete(this);
				}
			}
		}

		/// <summary>
		/// Encapsulates the information necessary to create
		/// a deferral schedule. Includes all fields from DRSchedule
		/// along with the necessary document transaction information.
		/// </summary>
		public class DRScheduleParameters : DRSchedule
		{
			public int? EmployeeID { get; set; }
			public int? SalesPersonID { get; set; }
			public int? SubID { get; set; }
		}

		/// <summary>
		/// Gets the deferral schedule parameters based on the 
		/// original document and document line transaction.
		/// </summary>
		public static DRScheduleParameters GetScheduleParameters(ARRegister document, ARTran tran)
		{
			return new DRScheduleParameters
			{
				Module = BatchModule.AR,
				DocType = tran.TranType,
				RefNbr = tran.RefNbr,
				LineNbr = tran.LineNbr,
				DocDate = document.DocDate,
				BAccountID = document.CustomerID,
				BAccountLocID = document.CustomerLocationID,
				FinPeriodID = document.FinPeriodID,
				TranDesc = tran.TranDesc,
				ProjectID = tran.ProjectID,
				TaskID = tran.TaskID,

				EmployeeID = tran.EmployeeID,
				SalesPersonID = tran.SalesPersonID,
				SubID = tran.SubID,

				TermStartDate = tran.DRTermStartDate,
				TermEndDate = tran.DRTermEndDate
			};
		}

		/// <summary>
		/// Gets the deferral schedule parameters based on the 
		/// original document and document line transaction.
		/// </summary>
		public static DRScheduleParameters GetScheduleParameters(APRegister document, APTran tran)
		{
			return new DRScheduleParameters
			{
				Module = BatchModule.AP,
				DocType = tran.TranType,
				RefNbr = tran.RefNbr,
				LineNbr = tran.LineNbr,
				DocDate = document.DocDate,
				BAccountID = document.VendorID,
				BAccountLocID = document.VendorLocationID,
				FinPeriodID = document.FinPeriodID,
				TranDesc = tran.TranDesc,
				ProjectID = tran.ProjectID,
				TaskID = tran.TaskID,

				EmployeeID = tran.EmployeeID,
				SubID = tran.SubID
			};
		}

		/// <param name="takeFromSales">
		/// If <c>true</c>, deferral transactions that are already posted will be handled.
		/// If <c>false</c>, only open deferral transactions will be used to create a related schedule.
		/// </param>
		private void CreateRelatedSchedule(int? branchID, int? defScheduleID, string module, string tranType, string refNbr, int? lineNbr, decimal? tranAmt, DateTime? docDate, int? bAccountID, int? locationID, DRDeferredCode defCode, InventoryItem inventoryItem, string docFinPeriod, int? acctID, int? subID, string tranDesc, int? projectID, int? projectTaskID, bool isDraft, bool accountForPostedTransactions)
		{
			DRSchedule originalDeferralSchedule = GetDeferralSchedule(defScheduleID);

			DRSchedule relatedSchedule = new DRSchedule
			{
				DocType = tranType,
				RefNbr = refNbr,
				LineNbr = lineNbr,
				DocDate = docDate,
				BAccountID = bAccountID,
				BAccountLocID = locationID,
				Module = module,
				FinPeriodID = docFinPeriod,
				TranDesc = tranDesc,
				IsDraft = isDraft,
				IsCustom = false,
				ProjectID = projectID,
				TaskID = projectTaskID
			};

			relatedSchedule = Schedule.Insert(relatedSchedule);

			PXResultset<DRScheduleDetail> originalDetails = PXSelect<
				DRScheduleDetail, 
				Where<
					DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>>>
				.Select(this, originalDeferralSchedule.ScheduleID);

			decimal originalDetailsTotal = SumTotal(originalDetails);
			decimal adjustmentTotal = tranAmt.Value;

			List<DRScheduleDetail> nonResidualOriginalDetails = originalDetails
				.RowCast<DRScheduleDetail>()
				.Where(detail => detail.IsResidual != true)
				.ToList();

			decimal newDetailTotal = 0m;

			foreach (DRScheduleDetail originalDetail in nonResidualOriginalDetails)
			{
				decimal detailPartRaw = originalDetailsTotal == 0 ? 
					0 :
					originalDetail.TotalAmt.Value * adjustmentTotal / originalDetailsTotal;

				decimal detailPart = PXDBCurrencyAttribute.BaseRound(this, detailPartRaw);

				decimal takeFromPostedRaw = 0;

				if (accountForPostedTransactions && originalDetail.TotalAmt.Value != 0)
				{
					takeFromPostedRaw = 
						detailPartRaw * (originalDetail.TotalAmt.Value - originalDetail.DefAmt.Value) / originalDetail.TotalAmt.Value;
				}

				decimal takeFromPosted = PXDBCurrencyAttribute.BaseRound(this, takeFromPostedRaw);

				decimal adjustmentDeferredAmountRaw = detailPartRaw - takeFromPosted;
				decimal adjustmentDeferredAmount = PXDBCurrencyAttribute.BaseRound(this, adjustmentDeferredAmountRaw);

				INComponent inventoryItemComponent = null;
				DRDeferredCode componentDeferralCode = null;

				if (inventoryItem != null)
				{
					inventoryItemComponent = GetInventoryItemComponent(inventoryItem.InventoryID, originalDetail.ComponentID);

					if (inventoryItemComponent != null)
					{
						componentDeferralCode = PXSelect<
							DRDeferredCode, 
							Where<
								DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>
							.Select(this, inventoryItemComponent.DeferredCode);
					}
				}
								
				InventoryItem component = GetInventoryItem(originalDetail.ComponentID);

				DRScheduleDetail relatedScheduleDetail;

				if (componentDeferralCode != null)
				{
					// Use component's deferral code
					// -
					relatedScheduleDetail = InsertScheduleDetail(
						branchID, 
						relatedSchedule, 
						inventoryItemComponent, 
						component, 
						componentDeferralCode, 
						detailPart, 
						originalDetail.DefAcctID, 
						originalDetail.DefSubID, 
						isDraft);                    
				}
				else
				{
					// Use deferral code and accounts from the document line
					// -
					relatedScheduleDetail = InsertScheduleDetail(
						branchID, 
						relatedSchedule, 
						component == null ? DRScheduleDetail.EmptyComponentID : component.InventoryID, 
						defCode, 
						detailPart, 
						originalDetail.DefAcctID, 
						originalDetail.DefSubID, 
						acctID, 
						subID, 
						isDraft);
				}

				newDetailTotal += detailPart;

				IList<DRScheduleTran> relatedTransactions = new List<DRScheduleTran>();
				DRDeferredCode relatedTransactionsDeferralCode = componentDeferralCode ?? defCode;

				IEnumerable<DRScheduleTran> originalPostedTransactions = null;

				if (accountForPostedTransactions)
				{
					originalPostedTransactions = PXSelect<
						DRScheduleTran,
						Where<
							DRScheduleTran.status, Equal<DRScheduleTranStatus.PostedStatus>,
							And<DRScheduleTran.scheduleID, Equal<Required<DRScheduleTran.scheduleID>>,
							And<DRScheduleTran.componentID, Equal<Required<DRScheduleTran.componentID>>,
							And<DRScheduleTran.lineNbr, NotEqual<Required<DRScheduleTran.lineNbr>>>>>>>
						.Select(
							this,
							originalDetail.ScheduleID,
							originalDetail.ComponentID,
							originalDetail.CreditLineNbr)
						.RowCast<DRScheduleTran>();
				}

				if (adjustmentDeferredAmount != 0)
				{
					string requiredTransactionStatus =
						relatedTransactionsDeferralCode.Method == DeferredMethodType.CashReceipt ?
							DRScheduleTranStatus.Projected :
							DRScheduleTranStatus.Open;

					IEnumerable<DRScheduleTran> originalOpenTransactions = PXSelect<
						DRScheduleTran,
						Where<
							DRScheduleTran.status, Equal<Required<DRScheduleTran.status>>,
							And<DRScheduleTran.scheduleID, Equal<Required<DRScheduleTran.scheduleID>>,
							And<DRScheduleTran.componentID, Equal<Required<DRScheduleTran.componentID>>>>>>
						.Select(
							this,
							requiredTransactionStatus,
							originalDetail.ScheduleID,
							originalDetail.ComponentID)
						.RowCast<DRScheduleTran>();

					IList<DRScheduleTran> relatedDeferralTransactions = 
						new TransactionsGenerator(this, relatedTransactionsDeferralCode).GenerateRelatedTransactions(
							relatedScheduleDetail,
							originalOpenTransactions,
							originalPostedTransactions,
							adjustmentDeferredAmount,
							takeFromPosted,
							branchID);

					foreach (DRScheduleTran deferralTransaction in relatedDeferralTransactions)
					{
						Transactions.Insert(deferralTransaction);
						relatedTransactions.Add(deferralTransaction);
					}
				}

				UpdateBalanceProjection(relatedTransactions, relatedScheduleDetail, defCode.AccountType);
			}

			DRScheduleDetail residualDetail = originalDetails
				.RowCast<DRScheduleDetail>()
				.FirstOrDefault(detail => detail.IsResidual == true);

			if (residualDetail != null)
			{
				decimal residualAmount = adjustmentTotal - newDetailTotal;

				InsertResidualScheduleDetail(
					relatedSchedule, 
					residualDetail.ComponentID, 
					residualAmount, 
					residualDetail.AccountID, 
					residualDetail.SubID, 
					isDraft);
			}
		}

		private void UpdateOriginalSchedule(ARTran tran, DRDeferredCode defCode, decimal amount, DateTime? docDate, string docFinPeriod, int? bAccountID, int? locationID)
		{
			DRSchedule origSchedule = GetDeferralSchedule(tran.DefScheduleID);

			DRScheduleDetail origDetail = PXSelect<DRScheduleDetail, 
				Where<
					DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>>>
				.Select(this, origSchedule.ScheduleID);

			decimal origTotalAmt = origDetail.TotalAmt.Value;

			decimal adjustTotalAmt;
			decimal extra = 0;

            if (origTotalAmt >= 0 && origTotalAmt <= amount || origTotalAmt < 0 && origTotalAmt >= amount)
			{
				adjustTotalAmt = origTotalAmt;
				extra = amount - origTotalAmt;
			}
			else
			{
				adjustTotalAmt = amount;
			}

            decimal part = origDetail.TotalAmt.Value * adjustTotalAmt / origTotalAmt;
            decimal takeFromSalesRaw = part * (origDetail.TotalAmt.Value - origDetail.DefAmt.Value) / origDetail.TotalAmt.Value;
			decimal takeFromSales = PXDBCurrencyAttribute.BaseRound(this, takeFromSalesRaw);
            decimal partWithExtra = origDetail.TotalAmt.Value * amount / origTotalAmt;
			decimal adjustDeferredAmountRaw = partWithExtra - takeFromSales;
			decimal adjustDeferredAmount = PXDBCurrencyAttribute.BaseRound(this, adjustDeferredAmountRaw);
			InventoryItem component = GetInventoryItem(origDetail.ComponentID);
			
			if (takeFromSales != 0)
			{
				DRScheduleTran nowTran = new DRScheduleTran();
				nowTran.AccountID = origDetail.AccountID;
				nowTran.SubID = origDetail.SubID;
				nowTran.Amount = -takeFromSales;
				nowTran.RecDate = this.Accessinfo.BusinessDate;
				nowTran.FinPeriodID = docFinPeriod;
				nowTran.ScheduleID = origDetail.ScheduleID;
				nowTran.ComponentID = origDetail.ComponentID;
				nowTran.Status = DRScheduleTranStatus.Open;

				Transactions.Insert(nowTran);
				UpdateBalanceProjection(nowTran, origDetail, defCode.AccountType);

				origDetail.DefAmt -= takeFromSales;
			}

			if (adjustDeferredAmount != 0)
			{
				origDetail.DefAmt -= adjustDeferredAmount;

				PXSelectBase<DRScheduleTran> projectedTranSelect = new
					PXSelect<DRScheduleTran, Where<DRScheduleTran.scheduleID, Equal<Required<DRSchedule.scheduleID>>,
					And<DRScheduleTran.componentID, Equal<Required<DRScheduleTran.componentID>>,
					And<DRScheduleTran.status, Equal<DRScheduleTranStatus.ProjectedStatus>>>>>(this);
				PXResultset<DRScheduleTran> projectedTrans = projectedTranSelect.Select(origDetail.ScheduleID, origDetail.ComponentID);

				decimal deltaRaw = adjustDeferredAmount / projectedTrans.Count;
				decimal delta = PXCurrencyAttribute.BaseRound(this, deltaRaw);

				foreach (DRScheduleTran dt in projectedTrans)
				{
					DRRevenueBalance histMin = new DRRevenueBalance();
					histMin.FinPeriodID = tran.FinPeriodID;
					histMin.AcctID = origDetail.DefAcctID;
					histMin.SubID = origDetail.DefSubID;
					histMin.ComponentID = origDetail.ComponentID ?? 0;
					histMin.ProjectID = origDetail.ProjectID ?? 0;
					histMin.CustomerID = origDetail.BAccountID;
					histMin = RevenueBalance.Insert(histMin);
					histMin.PTDProjected -= dt.Amount;
					histMin.EndProjected += dt.Amount;

					DRRevenueProjectionAccum projMin = new DRRevenueProjectionAccum();
					projMin.FinPeriodID = tran.FinPeriodID;
					projMin.AcctID = origDetail.AccountID;
					projMin.SubID = origDetail.SubID;
					projMin.ComponentID = origDetail.ComponentID ?? 0;
					projMin.ProjectID = origDetail.ProjectID ?? 0;
					projMin.CustomerID = origDetail.BAccountID;
					projMin = RevenueProjection.Insert(projMin);
					projMin.PTDProjected -= dt.Amount;
					
					dt.Amount -= delta;
					Transactions.Update(dt);

					DRRevenueBalance histPlus = new DRRevenueBalance();
					histPlus.FinPeriodID = tran.FinPeriodID;
					histPlus.AcctID = origDetail.DefAcctID;
					histPlus.SubID = origDetail.DefSubID;
					histPlus.ComponentID = origDetail.ComponentID ?? 0;
					histPlus.ProjectID = origDetail.ProjectID ?? 0;
					histPlus.CustomerID = origDetail.BAccountID;
					histPlus = RevenueBalance.Insert(histPlus);
					histPlus.PTDProjected += dt.Amount;
					histPlus.EndProjected -= dt.Amount;

					DRRevenueProjectionAccum projPlus = new DRRevenueProjectionAccum();
					projPlus.FinPeriodID = tran.FinPeriodID;
					projPlus.AcctID = origDetail.AccountID;
					projPlus.SubID = origDetail.SubID;
					projPlus.ComponentID = origDetail.ComponentID ?? 0;
					projPlus.ProjectID = origDetail.ProjectID ?? 0;
					projPlus.CustomerID = origDetail.BAccountID;
					projPlus = RevenueProjection.Insert(projPlus);
					projPlus.PTDProjected += dt.Amount;
				}
			}

			ScheduleDetail.Update(origDetail);

			DRRevenueBalance hist = new DRRevenueBalance();
			hist.FinPeriodID =  docFinPeriod;
			hist.AcctID = origDetail.DefAcctID;
			hist.SubID = origDetail.DefSubID;
			hist.ComponentID = origDetail.ComponentID ?? 0;
			hist.ProjectID = origDetail.ProjectID ?? 0;
			hist.CustomerID = origDetail.BAccountID;

			hist = RevenueBalance.Insert(hist);
			hist.PTDDeferred -= amount;
			hist.EndBalance -= amount;
			
		}
		
		private decimal SumTotal(PXResultset<DRScheduleDetail> details)
		{
			decimal result = 0;

			foreach (DRScheduleDetail row in details)
				result += row.TotalAmt.Value;

			return result;
		}

		private DRSchedule GetDeferralSchedule(int? scheduleID)
		{
			return PXSelect<
				DRSchedule, 
				Where<
					DRSchedule.scheduleID, Equal<Required<DRSchedule.scheduleID>>>>
				.Select(this, scheduleID);
		}

		private InventoryItem GetInventoryItem(int? inventoryItemID)
		{
			return PXSelect<
				InventoryItem, 
				Where<
					InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
				.Select(this, inventoryItemID);
		}

		private INComponent GetInventoryItemComponent(int? inventoryID, int? componentID)
		{
			return PXSelect<
				INComponent, 
				Where<
					INComponent.inventoryID, Equal<Required<INComponent.inventoryID>>,
					And<INComponent.componentID, Equal<Required<INComponent.componentID>>>>>
				.Select(this, inventoryID, componentID);
		}

		private DRScheduleDetail GetScheduleDetailForComponent(int? scheduleID, int? componentID)
		{
			return PXSelect<
				DRScheduleDetail,
				Where<
					DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>,
					And<DRScheduleDetail.componentID, Equal<Required<DRScheduleDetail.componentID>>>>>
				.Select(this, scheduleID, componentID);
		}

		/// <summary>
		/// Retrieves all schedule details for the specified 
		/// deferral schedule ID.
		/// </summary>
		public IList<DRScheduleDetail> GetScheduleDetails(int? scheduleID)
		{
			return PXSelect<
				DRScheduleDetail,
				Where<
					DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>>>
				.Select(this, scheduleID)
				.RowCast<DRScheduleDetail>()
				.ToList();
		}

		public EPEmployee GetEmployee(int? employeeID)
		{
			return PXSelect<
				EPEmployee,
				Where<
					EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>
				.Select(this, employeeID);
		}

		public Location GetLocation(int? businessAccountID, int? businessAccountLocationId)
		{
			return PXSelect<
				Location,
				Where<
					Location.bAccountID, Equal<Required<Location.bAccountID>>,
					And<Location.locationID, Equal<Required<Location.locationID>>>>>
				.Select(this, businessAccountID, businessAccountLocationId);
		}

		/// <summary>
		/// Generates and adds deferral transactions to the specified 
		/// deferral schedule and schedule detail.
		/// </summary>
		public void GenerateAndAddDeferralTransactions(
			DRSchedule deferralSchedule, 
			DRScheduleDetail scheduleDetail, 
			DRDeferredCode deferralCode, 
			int? branchID)
		{
			IList<DRScheduleTran> deferralTransactions = new TransactionsGenerator(this, deferralCode)
				.GenerateTransactions(deferralSchedule, scheduleDetail, branchID);

			deferralTransactions.ForEach(transaction => Transactions.Insert(transaction));

			UpdateBalanceProjection(deferralTransactions, scheduleDetail, deferralCode.AccountType);
		}

		#region History & Accumulator functions
		private void UpdateBalanceProjection(IList<DRScheduleTran> tranList, DRScheduleDetail sd, string deferredAccountType)
		{
			foreach (DRScheduleTran tran in tranList)
			{
				UpdateBalanceProjection(tran, sd, deferredAccountType);
			}
		}

		private void UpdateBalanceProjection(DRScheduleTran tran, DRScheduleDetail sd, string deferredAccountType)
		{
			switch (deferredAccountType)
			{
				case DeferredAccountType.Expense:
					UpdateExpenseBalanceProjection(tran, sd);
					UpdateExpenseProjection(tran, sd);
					break;
				case DeferredAccountType.Income:
					UpdateRevenueBalanceProjection(tran, sd);
					UpdateRevenueProjection(tran, sd);
					break;

				default:
					throw new PXException(Messages.InvalidAccountType, DeferredAccountType.Expense, DeferredAccountType.Income, deferredAccountType);
			}
		}

		private void UpdateRevenueBalanceProjection(DRScheduleTran tran, DRScheduleDetail sd)
		{
			DRRevenueBalance hist = new DRRevenueBalance();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.DefAcctID;
			hist.SubID = sd.DefSubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.CustomerID = sd.BAccountID ?? 0;

			hist = RevenueBalance.Insert(hist);

			if (IsReversed(sd))
			{
				hist.PTDProjected -= tran.Amount;
				hist.EndProjected += tran.Amount;
			}
			else
			{
				hist.PTDProjected += tran.Amount;
				hist.EndProjected -= tran.Amount;
			}
		}

		private void UpdateExpenseBalanceProjection(DRScheduleTran tran, DRScheduleDetail sd)
		{
			DRExpenseBalance hist = new DRExpenseBalance();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.DefAcctID;
			hist.SubID = sd.DefSubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.VendorID = sd.BAccountID ?? 0;

			hist = ExpenseBalance.Insert(hist);

			if (IsReversed(sd))
			{
				hist.PTDProjected -= tran.Amount;
				hist.EndProjected += tran.Amount;
			}
			else
			{
				hist.PTDProjected += tran.Amount;
				hist.EndProjected -= tran.Amount;
			}
		}

		/// <summary>
		/// Using the information from the provided <see cref="DRScheduleTran"/> and <see cref="DRScheduleDetail"/> objects
		/// to update the deferral balances and projections.
		/// </summary>
		private void UpdateBalance(
			DRScheduleTran deferralTransaction,
			DRScheduleDetail deferralScheduleDetail,
			string deferredAccountType, 
			bool isIntegrityCheck = false)
		{
			switch (deferredAccountType)
			{
				case DeferredAccountType.Expense:
					UpdateExpenseBalance(
						deferralTransaction, 
						deferralScheduleDetail, 
						updateEndProjected: isIntegrityCheck);

					UpdateExpenseProjectionUponRecognition(
						deferralTransaction, 
						deferralScheduleDetail,
						updatePTDRecognizedSamePeriod: isIntegrityCheck,
						updatePTDProjected: isIntegrityCheck);
					break;

				case DeferredAccountType.Income:
					UpdateRevenueBalance(
						deferralTransaction, 
						deferralScheduleDetail, 
						updateEndProjected: isIntegrityCheck);

					UpdateRevenueProjectionUponRecognition(
						deferralTransaction, 
						deferralScheduleDetail,
						updatePTDRecognizedSamePeriod: isIntegrityCheck,
						updatePTDProjected: isIntegrityCheck);
					break;

				default:
					throw new PXException(Messages.InvalidAccountType, DeferredAccountType.Expense, DeferredAccountType.Income, deferredAccountType);
			}
		}

		/// <param name="updateEndProjected">
		/// A boolean flag indicating whether <see cref="DRRevenueBalance.EndProjected"/> should be updated.
		/// This can be required during the Balance Validation process.
		/// </param>
		private void UpdateRevenueBalance(DRScheduleTran tran, DRScheduleDetail sd, bool updateEndProjected = false)
		{
			DRRevenueBalance hist = new DRRevenueBalance();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.DefAcctID;
			hist.SubID = sd.DefSubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.CustomerID = sd.BAccountID ?? 0;

			hist = RevenueBalance.Insert(hist);

			if (IsReversed(sd))
			{
				hist.PTDRecognized -= tran.Amount;
				hist.EndBalance += tran.Amount;

				if (tran.FinPeriodID == sd.FinPeriodID)
				{
					hist.PTDRecognizedSamePeriod -= tran.Amount;
				}

				if (updateEndProjected)
				{
					hist.EndProjected += tran.Amount;
				}
			}
			else
			{
				hist.PTDRecognized += tran.Amount;
				hist.EndBalance -= tran.Amount;

				if (tran.FinPeriodID == sd.FinPeriodID)
				{
					hist.PTDRecognizedSamePeriod += tran.Amount;
				}

				if (updateEndProjected)
				{
					hist.EndProjected -= tran.Amount;
				}
			}
		}

		/// <param name="updateEndProjected">
		/// A boolean flag indicating whether <see cref="DRExpenseBalance.EndProjected"/> balances should be updated. 
		/// This can be required during the Balance Validation process.
		/// </param>
		private void UpdateExpenseBalance(DRScheduleTran tran, DRScheduleDetail sd, bool updateEndProjected = false)
		{
			DRExpenseBalance hist = new DRExpenseBalance();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.DefAcctID;
			hist.SubID = sd.DefSubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.VendorID = sd.BAccountID ?? 0;

			hist = ExpenseBalance.Insert(hist);

			if (IsReversed(sd))
			{
				hist.PTDRecognized -= tran.Amount;
				hist.EndBalance += tran.Amount;

				if (tran.FinPeriodID == sd.FinPeriodID)
				{
					hist.PTDRecognizedSamePeriod -= tran.Amount;
				}

				if (updateEndProjected)
				{
					hist.EndProjected += tran.Amount;
				}
			}
			else
			{
				hist.PTDRecognized += tran.Amount;
				hist.EndBalance -= tran.Amount;

				if (tran.FinPeriodID == sd.FinPeriodID)
				{
					hist.PTDRecognizedSamePeriod += tran.Amount;
				}

				if (updateEndProjected)
				{
					hist.EndProjected -= tran.Amount;
				}
			}
		}
				
		private void UpdateRevenueProjection(DRScheduleTran tran, DRScheduleDetail sd)
		{
			DRRevenueProjectionAccum hist = new DRRevenueProjectionAccum();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.AccountID;
			hist.SubID = sd.SubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.CustomerID = sd.BAccountID ?? 0;

			hist = RevenueProjection.Insert(hist);

			if (IsReversed(sd))
			{
				if (tran.FinPeriodID == sd.FinPeriodID)
					hist.PTDRecognizedSamePeriod -= tran.Amount;
				hist.PTDProjected -= tran.Amount;
			}
			else
			{
				if (tran.FinPeriodID == sd.FinPeriodID)
					hist.PTDRecognizedSamePeriod += tran.Amount;
				hist.PTDProjected += tran.Amount;
			}
		}

		private void UpdateExpenseProjection(DRScheduleTran tran, DRScheduleDetail sd)
		{
			DRExpenseProjectionAccum hist = new DRExpenseProjectionAccum();
			hist.FinPeriodID = tran.FinPeriodID;
			hist.AcctID = sd.AccountID;
			hist.SubID = sd.SubID;
			hist.ComponentID = sd.ComponentID ?? 0;
			hist.ProjectID = sd.ProjectID ?? 0;
			hist.VendorID = sd.BAccountID ?? 0;

			hist = ExpenseProjection.Insert(hist);

			if (IsReversed(sd))
			{
				if (tran.FinPeriodID == sd.FinPeriodID)
					hist.PTDRecognizedSamePeriod -= tran.Amount;
				hist.PTDProjected -= tran.Amount;
			}
			else
			{
				if (tran.FinPeriodID == sd.FinPeriodID)
					hist.PTDRecognizedSamePeriod += tran.Amount;
				hist.PTDProjected += tran.Amount;
			}
		}

		/// <summary>
		/// Updates revenue projection history table to reflect the recognition of a deferral transaction.
		/// </summary>
		/// <param name="updatePTDRecognizedSamePeriod">
		/// A boolean flag indicating whether <see cref="DRRevenueProjection.PTDRecognizedSamePeriod"/> balances 
		/// should be updated. This can be required during the Balance Validation process.
		/// </param>
		/// <param name="updatePTDProjected">
		/// A boolean flag indicating whether <see cref="DRRevenueProjection.PTDProjected"/> balances
		/// should be updated. This can be required during the Balance Validation process, but undesirable
		/// during normal recognition.
		/// </param>
		private void UpdateRevenueProjectionUponRecognition(
			DRScheduleTran transaction, 
			DRScheduleDetail scheduleDetail, 
			bool updatePTDRecognizedSamePeriod = false,
			bool updatePTDProjected = false)
		{
			DRRevenueProjectionAccum hist = new DRRevenueProjectionAccum
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.AccountID,
				SubID = scheduleDetail.SubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				CustomerID = scheduleDetail.BAccountID ?? 0
			};

			hist = RevenueProjection.Insert(hist);

			decimal? transactionAmount = IsReversed(scheduleDetail) ? -transaction.Amount : transaction.Amount;

			hist.PTDRecognized += transactionAmount;

			if (updatePTDRecognizedSamePeriod && transaction.FinPeriodID == scheduleDetail.FinPeriodID)
			{
				hist.PTDRecognizedSamePeriod += transactionAmount;
			}

			if (updatePTDProjected)
			{
				hist.PTDProjected += transactionAmount;
			}
		}

		/// <summary>
		/// Updates expense projection history table to reflect the recognition of a deferral transaction.
		/// </summary>
		/// <param name="updatePTDRecognizedSamePeriod">
		/// A boolean flag indicating whether <see cref="DRRevenueProjection.PTDRecognizedSamePeriod"/> balances 
		/// should be updated. This can be required during the Balance Validation process.
		/// </param>
		/// <param name="updatePTDProjected">
		/// A boolean flag indicating whether <see cref="DRRevenueProjection.PTDProjected"/> balances
		/// should be updated. This can be required during the Balance Validation process, but undesirable
		/// during normal recognition.
		/// </param>
		private void UpdateExpenseProjectionUponRecognition(
			DRScheduleTran transaction, 
			DRScheduleDetail scheduleDetail, 
			bool updatePTDRecognizedSamePeriod = false,
			bool updatePTDProjected = false)
		{
			DRExpenseProjectionAccum hist = new DRExpenseProjectionAccum
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.AccountID,
				SubID = scheduleDetail.SubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				VendorID = scheduleDetail.BAccountID ?? 0
			};

			hist = ExpenseProjection.Insert(hist);

			decimal? transactionAmount = IsReversed(scheduleDetail) ? -transaction.Amount : transaction.Amount;
			
			hist.PTDRecognized += transactionAmount;

			if (updatePTDRecognizedSamePeriod && transaction.FinPeriodID == scheduleDetail.FinPeriodID)
			{
				hist.PTDRecognizedSamePeriod += transactionAmount;
			}

			if (updatePTDProjected)
			{
				hist.PTDProjected += transactionAmount;
			}
		}

		private void InitBalance(DRScheduleTran transaction, DRScheduleDetail scheduleDetail, string deferredAccountType)
		{
			switch (deferredAccountType)
			{
				case DeferredAccountType.Expense:
					InitExpenseBalance(transaction, scheduleDetail);
					break;
				case DeferredAccountType.Income:
					InitRevenueBalance(transaction, scheduleDetail);
					break;
				default:
					throw new PXException(Messages.InvalidAccountType, DeferredAccountType.Expense, DeferredAccountType.Income, deferredAccountType);
			}
		}

		private void InitRevenueBalance(DRScheduleTran transaction, DRScheduleDetail scheduleDetail)
		{
			DRRevenueBalance hist = new DRRevenueBalance();
			hist.FinPeriodID = transaction.FinPeriodID;
			hist.AcctID = scheduleDetail.DefAcctID;
			hist.SubID = scheduleDetail.DefSubID;
			hist.ComponentID = scheduleDetail.ComponentID ?? 0;
			hist.ProjectID = scheduleDetail.ProjectID ?? 0;
			hist.CustomerID = scheduleDetail.BAccountID ?? 0;

			hist = RevenueBalance.Insert(hist);

			if (IsReversed(scheduleDetail))
			{
				hist.PTDDeferred -= transaction.Amount;
				hist.EndBalance -= transaction.Amount;
				hist.EndProjected -= transaction.Amount;
			}
			else
			{
				hist.PTDDeferred += transaction.Amount;
				hist.EndBalance += transaction.Amount;
				hist.EndProjected += transaction.Amount;
			}
		}

		private void InitExpenseBalance(DRScheduleTran transaction, DRScheduleDetail scheduleDetail)
		{
			DRExpenseBalance hist = new DRExpenseBalance();
			hist.FinPeriodID = transaction.FinPeriodID;
			hist.AcctID = scheduleDetail.DefAcctID;
			hist.SubID = scheduleDetail.DefSubID;
			hist.ComponentID = scheduleDetail.ComponentID ?? 0;
			hist.ProjectID = scheduleDetail.ProjectID ?? 0;
			hist.VendorID = scheduleDetail.BAccountID ?? 0;

			hist = ExpenseBalance.Insert(hist);

			if (IsReversed(scheduleDetail))
			{
				hist.PTDDeferred -= transaction.Amount;
				hist.EndBalance -= transaction.Amount;
				hist.EndProjected -= transaction.Amount;
			}
			else
			{
				hist.PTDDeferred += transaction.Amount;
				hist.EndBalance += transaction.Amount;
				hist.EndProjected += transaction.Amount;
			}
		}


		public void Subtract(DRScheduleDetail scheduleDetail, DRScheduleTran transaction, string deferralCodeType)
		{
			Add(scheduleDetail, transaction, deferralCodeType, amountMultiplier: -1);
		}

		public void Add(
			DRScheduleDetail scheduleDetail, 
			DRScheduleTran transaction, 
			string deferralCodeType, 
			decimal amountMultiplier = 1m)
		{
			if (deferralCodeType == DeferredAccountType.Expense)
			{
				AddExpenseToProjection(transaction, scheduleDetail, amountMultiplier);
				AddExpenseToBalance(transaction, scheduleDetail, amountMultiplier);
			}
			else if (deferralCodeType == DeferredAccountType.Income)
			{
				AddRevenueToProjection(transaction, scheduleDetail, amountMultiplier);
				AddRevenueToBalance(transaction, scheduleDetail, amountMultiplier);
			}
			else
			{
				throw new PXArgumentException(
					Messages.InvalidAccountType,
					DeferredAccountType.Expense,
					DeferredAccountType.Income,
					deferralCodeType);
			}
		}

		private void AddRevenueToProjection(DRScheduleTran transaction, DRScheduleDetail scheduleDetail, decimal amountMultiplier)
		{
			DRRevenueProjectionAccum hist = new DRRevenueProjectionAccum
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.AccountID,
				SubID = scheduleDetail.SubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				CustomerID = scheduleDetail.BAccountID ?? 0
			};

			hist = RevenueProjection.Insert(hist);

			hist.PTDProjected += amountMultiplier * transaction.Amount;
		}

		private void AddExpenseToProjection(DRScheduleTran transaction, DRScheduleDetail scheduleDetail, decimal amountMultiplier)
		{
			DRExpenseProjectionAccum hist = new DRExpenseProjectionAccum
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.AccountID,
				SubID = scheduleDetail.SubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				VendorID = scheduleDetail.BAccountID ?? 0
			};

			hist = ExpenseProjection.Insert(hist);

			hist.PTDProjected += amountMultiplier * transaction.Amount;
		}

		private void AddRevenueToBalance(DRScheduleTran transaction, DRScheduleDetail scheduleDetail, decimal amountMultiplier)
		{
			DRRevenueBalance hist = new DRRevenueBalance
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.DefAcctID,
				SubID = scheduleDetail.DefSubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				CustomerID = scheduleDetail.BAccountID ?? 0
			};

			hist = RevenueBalance.Insert(hist);

			hist.PTDProjected += amountMultiplier * transaction.Amount;
			hist.EndProjected -= amountMultiplier * transaction.Amount;
		}

		private void AddExpenseToBalance(DRScheduleTran transaction, DRScheduleDetail scheduleDetail, decimal amountMultiplier)
		{
			DRExpenseBalance hist = new DRExpenseBalance
			{
				FinPeriodID = transaction.FinPeriodID,
				AcctID = scheduleDetail.DefAcctID,
				SubID = scheduleDetail.DefSubID,
				ComponentID = scheduleDetail.ComponentID ?? 0,
				ProjectID = scheduleDetail.ProjectID ?? 0,
				VendorID = scheduleDetail.BAccountID ?? 0
			};

			hist = ExpenseBalance.Insert(hist);

			hist.PTDProjected += amountMultiplier * transaction.Amount;
			hist.EndProjected -= amountMultiplier * transaction.Amount;
		} 
		#endregion


        private DRScheduleDetail InsertScheduleDetail(int? branchID, DRSchedule sc, INComponent component, InventoryItem compItem, DRDeferredCode defCode, decimal amount, int? defAcctID, int? defSubID, bool isDraft)
		{
			int? acctID = sc.Module == BatchModule.AP ? compItem.COGSAcctID : component.SalesAcctID;
			int? subID = sc.Module == BatchModule.AP ? compItem.COGSSubID : component.SalesSubID;

			return InsertScheduleDetail(branchID, sc, compItem.InventoryID, defCode, amount, defAcctID, defSubID, acctID, subID, isDraft);
		}

		private DRScheduleDetail InsertScheduleDetail(int? branchID, DRSchedule sc, int? componentID, DRDeferredCode defCode, decimal amount, int? defAcctID, int? defSubID, int? acctID, int? subID, bool isDraft)
		{
			DRScheduleDetail sd = new DRScheduleDetail();
			sd.ScheduleID = sc.ScheduleID;
			sd.ComponentID = componentID;
			sd.TotalAmt = amount;
			sd.DefAmt = amount;
			sd.DefCode = defCode.DeferredCodeID;
			sd.Status = DRScheduleStatus.Open;
			sd.IsOpen = true;
			sd.Module = sc.Module;
			sd.DocType = sc.DocType;
			sd.RefNbr = sc.RefNbr;
			sd.LineNbr = sc.LineNbr;
			sd.FinPeriodID = sc.FinPeriodID;
			sd.BAccountID = sc.BAccountID;
			sd.AccountID = acctID;
			sd.SubID = subID;
			sd.DefAcctID = defAcctID;
			sd.DefSubID = defSubID;
			sd.CreditLineNbr = 0;
			sd.DocDate = sc.DocDate;
			sd.BAccountType = sc.Module == BatchModule.AP ? PX.Objects.CR.BAccountType.VendorType : PX.Objects.CR.BAccountType.CustomerType;
			sd = ScheduleDetail.Insert(sd);
			sd.Status = isDraft ? DRScheduleStatus.Draft : DRScheduleStatus.Open;
			sd.IsCustom = false;

			if (!isDraft)
			{
				//create credit line:
				CreateCreditLine(sd, defCode, branchID);
			}

			return sd;
		}

		private void InsertResidualScheduleDetail(DRSchedule schedule, int? componentID, decimal amount, int? acctID, int? subID, bool isDraft)
		{
			DRScheduleDetail sd = new DRScheduleDetail();
			sd.ScheduleID = schedule.ScheduleID;
			sd.ComponentID = componentID;
			sd.TotalAmt = amount;
			sd.DefAmt = 0.0m;
			sd.DefCode = null;
			sd.IsOpen = false;
			sd.CloseFinPeriodID = sd.FinPeriodID;
			sd.Module = schedule.Module;
			sd.DocType = schedule.DocType;
			sd.RefNbr = schedule.RefNbr;
			sd.LineNbr = schedule.LineNbr;
			sd.FinPeriodID = schedule.FinPeriodID;
			sd.BAccountID = schedule.BAccountID;
			sd.AccountID = acctID;
			sd.SubID = subID;
			sd.DefAcctID = acctID;
			sd.DefSubID = subID;
			sd.CreditLineNbr = 0;
			sd.DocDate = schedule.DocDate;
			sd.BAccountType = schedule.Module == BatchModule.AP ? PX.Objects.CR.BAccountType.VendorType : PX.Objects.CR.BAccountType.CustomerType;

			sd.Status = isDraft ? DRScheduleStatus.Draft : DRScheduleStatus.Closed;
			sd.IsCustom = false;
			sd.IsResidual = true;

			ScheduleDetail.Insert(sd);
		}

		public void CreateCreditLine(DRScheduleDetail scheduleDetail, DRDeferredCode deferralCode, int? branchID)
		{
			DRScheduleTran creditLineTransaction = new DRScheduleTran
			{
				BranchID = branchID,
				AccountID = scheduleDetail.AccountID,
				SubID = scheduleDetail.SubID,
				Amount = scheduleDetail.TotalAmt,
				RecDate = this.Accessinfo.BusinessDate,
				TranDate = this.Accessinfo.BusinessDate,
				FinPeriodID = scheduleDetail.FinPeriodID,
				LineNbr = 0,
				ScheduleID = scheduleDetail.ScheduleID,
				ComponentID = scheduleDetail.ComponentID,
				Status = DRScheduleTranStatus.Posted
			};

			creditLineTransaction = Transactions.Insert(creditLineTransaction);

			InitBalance(creditLineTransaction, scheduleDetail, deferralCode.AccountType);
		}

		/// <summary>
		/// Determines whether a given deferral schedule detail originates
		/// from a reversal document.
		/// </summary>
		private static bool IsReversed(DRScheduleDetail scheduleDetail)
		{
			return 
				scheduleDetail.DocType == ARDocType.CreditMemo || scheduleDetail.DocType == APDocType.DebitAdj || 
				scheduleDetail.DocType == ARDocType.CashReturn || scheduleDetail.DocType == APDocType.VoidQuickCheck;
		}

		#region Explicit Interface Implementation

		SalesPerson IBusinessAccountProvider.GetSalesPerson(int? salesPersonID)
		{
			return PXSelect<
				SalesPerson,
				Where<
					SalesPerson.salesPersonID, Equal<Required<SalesPerson.salesPersonID>>>>
				.Select(this, salesPersonID);
		}

		IEnumerable<InventoryItemComponentInfo> IInventoryItemProvider.GetInventoryItemComponents(
			int? inventoryItemID,
			string requiredAllocationMethod)
		{
			bool hasDeferralCode = requiredAllocationMethod != INAmountOption.Residual;

			if (hasDeferralCode)
			{
				return PXSelectJoin<
					INComponent,
						InnerJoin<DRDeferredCode, On<INComponent.deferredCode, Equal<DRDeferredCode.deferredCodeID>>,
						InnerJoin<InventoryItem, On<INComponent.componentID, Equal<InventoryItem.inventoryID>>>>,
					Where<
						INComponent.inventoryID, Equal<Required<INComponent.inventoryID>>,
						And<INComponent.amtOption, Equal<Required<INComponent.amtOption>>>>>
					.Select(this, inventoryItemID, requiredAllocationMethod)
					.Cast<PXResult<INComponent, DRDeferredCode, InventoryItem>>()
					.Select(result => new InventoryItemComponentInfo
					{
						Component = result,
						Item = result,
						DeferralCode = result,
					});
			}
			else
			{
				return PXSelectJoin<
					INComponent,
						InnerJoin<InventoryItem, On<INComponent.componentID, Equal<InventoryItem.inventoryID>>>,
					Where<
						INComponent.inventoryID, Equal<Required<INComponent.inventoryID>>,
						And<INComponent.amtOption, Equal<Required<INComponent.amtOption>>>>>
					.Select(this, inventoryItemID, requiredAllocationMethod)
					.Cast<PXResult<INComponent, InventoryItem>>()
					.Select(result => new InventoryItemComponentInfo
					{
						Component = result,
						Item = result,
						DeferralCode = null,
					});
			}
		}

		string IInventoryItemProvider.GetComponentName(INComponent component)
			=> this.Caches[typeof(INComponent)].GetValueExt<INComponent.componentID>(component) as string;

		class APSubaccountProvider : SubaccountProviderBase
		{
			public APSubaccountProvider(PXGraph graph) : base(graph)
			{ }

			public override string MakeSubaccount<Field>(string mask, object[] sourceFieldValues, Type[] sourceFields)
			{
				return SubAccountMaskAPAttribute.MakeSub<Field>(
					_graph,
					mask,
					sourceFieldValues,
					sourceFields);
			}
		}

		class ARSubaccountProvider : SubaccountProviderBase
		{
			public ARSubaccountProvider(PXGraph graph) : base(graph)
			{ }

			public override string MakeSubaccount<Field>(string mask, object[] sourceFieldValues, Type[] sourceFields)
			{
				return SubAccountMaskARAttribute.MakeSub<Field>(
					_graph,
					mask,
					sourceFieldValues,
					sourceFields);
			}
		}

		DRSchedule IDREntityStorage.CreateCopy(DRSchedule originalSchedule)
			=> Schedule.Cache.CreateCopy(originalSchedule) as DRSchedule;

		DRScheduleTran IDREntityStorage.CreateCopy(DRScheduleTran originalTransaction)
			=> Transactions.Cache.CreateCopy(originalTransaction) as DRScheduleTran;

		IList<DRScheduleTran> IDREntityStorage.GetDeferralTransactions(int? scheduleID, int? componentID)
		{
			return PXSelect<
				DRScheduleTran,
				Where<
					DRScheduleTran.scheduleID, Equal<Required<DRScheduleTran.scheduleID>>,
					And<DRScheduleTran.componentID, Equal<Required<DRScheduleTran.componentID>>>>>
				.Select(this, scheduleID, componentID)
				.RowCast<DRScheduleTran>()
				.ToList();
		}

		DRDeferredCode IDREntityStorage.GetDeferralCode(string deferralCodeID)
		{
			return PXSelect<
				DRDeferredCode,
				Where<
					DRDeferredCode.deferredCodeID, Equal<Required<DRDeferredCode.deferredCodeID>>>>
				.Select(this, deferralCodeID);
		}

		DRSchedule IDREntityStorage.Insert(DRSchedule schedule)
			=> this.Schedule.Insert(schedule);

		DRSchedule IDREntityStorage.Update(DRSchedule schedule)
			=> this.Schedule.Update(schedule);

		DRScheduleDetail IDREntityStorage.Insert(DRScheduleDetail scheduleDetail)
			=> this.ScheduleDetail.Insert(scheduleDetail);

		DRScheduleDetail IDREntityStorage.Update(DRScheduleDetail scheduleDetail)
			=> this.ScheduleDetail.Update(scheduleDetail);

		void IDREntityStorage.ScheduleTransactionModified(
			DRScheduleDetail scheduleDetail,
			DRDeferredCode deferralCode, 
			DRScheduleTran oldTransaction, 
			DRScheduleTran newTransaction)
		{
			Subtract(scheduleDetail, oldTransaction, deferralCode.AccountType);
			Add(scheduleDetail, newTransaction, deferralCode.AccountType);

			Transactions.Update(newTransaction);
		}

		void IDREntityStorage.CreateDeferralTransactions(
			DRSchedule deferralSchedule, 
			DRScheduleDetail scheduleDetail,
			DRDeferredCode deferralCode, 
			int? branchID) =>
				this.GenerateAndAddDeferralTransactions(deferralSchedule, scheduleDetail, deferralCode, branchID);

		void IDREntityStorage.CreateCreditLineTransaction(
			DRScheduleDetail scheduleDetail,
			DRDeferredCode deferralCode,
			int? branchID) =>
				this.CreateCreditLine(scheduleDetail, deferralCode, branchID);

		#endregion
	}
}
