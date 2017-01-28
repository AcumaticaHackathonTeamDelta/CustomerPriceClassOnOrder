using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.TX;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.AR;
using System.Reflection;
using System.Linq;


namespace PX.Objects.CA
{
	#region CashBalanceAttribute
	/// <summary>
	/// This attribute allows to display current CashAccount balance from CADailySummary<br/>
	/// Read-only. Should be placed on Decimal? field<br/>
	/// <example>
	/// [CashBalance(typeof(PayBillsFilter.payAccountID))]
	/// </example>
	/// </summary>
	public class CashBalanceAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		protected string _CashAccount = null;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="CashAccountType">Must be IBqlField. Refers to the cashAccountID field in the row</param>
		public CashBalanceAttribute(Type CashAccountType)
		{
			_CashAccount = CashAccountType.Name;
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CASetup caSetup = PXSelect<CASetup>.Select(sender.Graph);
			decimal? result = 0m;
			object CashAccountID = sender.GetValue(e.Row, _CashAccount);

			CADailySummary caBalance = PXSelectGroupBy<CADailySummary,
														 Where<CADailySummary.cashAccountID, Equal<Required<CADailySummary.cashAccountID>>>,
																	Aggregate<Sum<CADailySummary.amtReleasedClearedCr,
																	 Sum<CADailySummary.amtReleasedClearedDr,
																	 Sum<CADailySummary.amtReleasedUnclearedCr,
																	 Sum<CADailySummary.amtReleasedUnclearedDr,
																	 Sum<CADailySummary.amtUnreleasedClearedCr,
																	 Sum<CADailySummary.amtUnreleasedClearedDr,
																	 Sum<CADailySummary.amtUnreleasedUnclearedCr,
																	 Sum<CADailySummary.amtUnreleasedUnclearedDr>>>>>>>>>>.
																	 Select(sender.Graph, CashAccountID);
			if ((caBalance != null) && (caBalance.CashAccountID != null))
			{
				result = caBalance.AmtReleasedClearedDr - caBalance.AmtReleasedClearedCr;

				if ((bool)caSetup.CalcBalDebitClearedUnreleased)
					result += caBalance.AmtUnreleasedClearedDr;
				if ((bool)caSetup.CalcBalCreditClearedUnreleased)
					result -= caBalance.AmtUnreleasedClearedCr;
				if ((bool)caSetup.CalcBalDebitUnclearedReleased)
					result += caBalance.AmtReleasedUnclearedDr;
				if ((bool)caSetup.CalcBalCreditUnclearedReleased)
					result -= caBalance.AmtReleasedUnclearedCr;
				if ((bool)caSetup.CalcBalDebitUnclearedUnreleased)
					result += caBalance.AmtUnreleasedUnclearedDr;
				if ((bool)caSetup.CalcBalCreditUnclearedUnreleased)
					result -= caBalance.AmtUnreleasedUnclearedCr;
			}
			e.ReturnValue = result;
			e.Cancel = true;
		}
	} 
	#endregion

	#region GLBalanceAttribute
	/// <summary>
	/// This attribute allows to display a  CashAccount balance from GLHistory for <br/>
	/// the defined Fin. Period. If the fin date is provided, the period, containing <br/>
	/// the date will be selected (Fin. Period parameter will be ignored in this case)<br/>
	/// Balance corresponds to the CuryFinYtdBalance for the period <br/>
	/// Read-only. Should be placed on the field having type Decimal?<br/>
	/// <example>
	/// [GLBalance(typeof(CATransfer.outAccountID), null, typeof(CATransfer.outDate))]
	/// or
	/// [GLBalance(typeof(PrintChecksFilter.payAccountID), typeof(PrintChecksFilter.payFinPeriodID))]
	/// </example>
	/// </summary>
	public class GLBalanceAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		protected string _CashAccount = null;
		protected string _FinDate = null;
		protected string _FinPeriodID = null;
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="CashAccountType">Must be IBqlField type. Refers CashAccountID field of the row.</param>
		/// <param name="FinPeriodID">Must be IBqlField type. Refers FinPeriodID field of the row.</param>
		public GLBalanceAttribute(Type CashAccountType, Type FinPeriodID)
		{
			_CashAccount = CashAccountType.Name;
			_FinPeriodID = FinPeriodID.Name;

		}
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="CashAccountType">Must be IBqlField type. Refers CashAccountID field of the row.</param>
		/// <param name="FinPeriodID">Not used.Value is ignored</param>
		/// <param name="FinDateType">Must be IBqlField type. Refers FinDate field of the row.</param>
		public GLBalanceAttribute(Type CashAccountType, Type FinPeriodID, Type FinDateType)
		{
			_CashAccount = CashAccountType.Name;
			_FinDate = FinDateType.Name;
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			GLSetup gLSetup = PXSelect<GLSetup>.Select(sender.Graph);
			decimal? result = 0m;
			object CashAccountID = sender.GetValue(e.Row, _CashAccount);

			object FinPeriodID = null;

			if (string.IsNullOrEmpty(_FinPeriodID))
			{
				object FinDate = sender.GetValue(e.Row, _FinDate);
				FinPeriod finPeriod = PXSelect<FinPeriod, Where<FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>,
															And<FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>>>>.Select(sender.Graph, FinDate, FinDate);
				if (finPeriod != null)
				{
					FinPeriodID = finPeriod.FinPeriodID;
				}
			}
			else
			{
				FinPeriodID = sender.GetValue(e.Row, _FinPeriodID);
			}

			if (CashAccountID != null && FinPeriodID != null)
			{
				// clear glhistory cache for ReleasePayments longrun
				sender.Graph.Caches<GLHistory>().ClearQueryCache();
				sender.Graph.Caches<GLHistory>().Clear();

				GLHistory gLHistory = PXSelectJoin<GLHistory,
													InnerJoin<GLHistoryByPeriod,
															On<GLHistoryByPeriod.accountID, Equal<GLHistory.accountID>,
															And<GLHistoryByPeriod.branchID, Equal<GLHistory.branchID>,
															And<GLHistoryByPeriod.ledgerID, Equal<GLHistory.ledgerID>,
															And<GLHistoryByPeriod.subID, Equal<GLHistory.subID>,
															And<GLHistoryByPeriod.lastActivityPeriod, Equal<GLHistory.finPeriodID>>>>>>,
													InnerJoin<Branch,
															On<Branch.branchID, Equal<GLHistory.branchID>,
															And<Branch.ledgerID, Equal<GLHistory.ledgerID>>>,
													InnerJoin<CashAccount,
															On<GLHistoryByPeriod.branchID, Equal<CashAccount.branchID>, 
															And<GLHistoryByPeriod.accountID, Equal<CashAccount.accountID>,
															And<GLHistoryByPeriod.subID, Equal<CashAccount.subID>>>>,
													InnerJoin<Account,
															On<GLHistoryByPeriod.accountID, Equal<Account.accountID>, 
															And<Match<Account, Current<AccessInfo.userName>>>>,
													InnerJoin<Sub,
															On<GLHistoryByPeriod.subID, Equal<Sub.subID>, And<Match<Sub, Current<AccessInfo.userName>>>>>>>>>,
													Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>,
													   And<GLHistoryByPeriod.finPeriodID, Equal<Required<GLHistoryByPeriod.finPeriodID>>>
													 >>.Select(sender.Graph, CashAccountID, FinPeriodID);

				if (gLHistory != null)
				{
					result = gLHistory.CuryFinYtdBalance;
				}
			}
			e.ReturnValue = result;
			e.Cancel = true;
		}
	} 
	#endregion

	#region CATaxAttribute
	public class CATaxAttribute : ManualVATAttribute
	{
		public CATaxAttribute(Type ParentType, Type TaxType, Type TaxSumType)
			: base(ParentType, TaxType, TaxSumType)
		{
			Init();
		}

		public CATaxAttribute(Type ParentType, Type TaxType, Type TaxSumType, Type CalcMode)
			: base(ParentType, TaxType, TaxSumType, CalcMode)
		{
			Init();
		}
		
		private void Init()
		{
			this.CuryDocBal = typeof(CAAdj.curyTranAmt);
			this.CuryLineTotal = typeof(CAAdj.curySplitTotal);
			this.DocDate = typeof(CAAdj.tranDate);
		}

		protected override List<object> SelectTaxes<Where>(PXGraph graph, object row, PXTaxCheck taxchk, params object[] parameters)
		{
			Dictionary<string, PXResult<Tax, TaxRev>> tail = new Dictionary<string, PXResult<Tax, TaxRev>>();
			object[] currents = new object[] { row, ((CATranEntry)graph).CAAdjRecords.Current };
			foreach (PXResult<Tax, TaxRev> record in PXSelectReadonly2<Tax,
				LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
					And<TaxRev.outdated, Equal<boolFalse>,
					And2<
						Where<Current<CAAdj.drCr>, Equal<CADrCr.cACredit>, And<TaxRev.taxType, Equal<TaxType.purchase>, And<Tax.reverseTax, Equal<boolFalse>,
						Or<Current<CAAdj.drCr>, Equal<CADrCr.cACredit>, And<TaxRev.taxType, Equal<TaxType.sales>, And2<Where<Tax.reverseTax, Equal<boolTrue>, 
							Or<Tax.taxType, Equal<CSTaxType.use>>>,
						Or<Current<CAAdj.drCr>, Equal<CADrCr.cADebit>, And<TaxRev.taxType, Equal<TaxType.sales>, And<Tax.reverseTax, Equal<boolFalse>, 
							And<Tax.taxType, NotEqual<CSTaxType.withholding>, And<Tax.taxType, NotEqual<CSTaxType.use>>>>>>>>>>>>,
					And<Current<CAAdj.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>,
				Where>
				.SelectMultiBound(graph, currents, parameters))
			{
				Tax adjdTax = AdjustTaxLevel(graph, (Tax)record);
				tail[((Tax)record).TaxID] = new PXResult<Tax, TaxRev>(adjdTax, (TaxRev)record);
			}
			List<object> ret = new List<object>();
			switch (taxchk)
			{
				case PXTaxCheck.Line:
					foreach (CATax record in PXSelect<CATax,
						Where<CATax.adjTranType, Equal<Current<CASplit.adjTranType>>,
							And<CATax.adjRefNbr, Equal<Current<CASplit.adjRefNbr>>,
							And<CATax.lineNbr, Equal<Current<CASplit.lineNbr>>>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<CATax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<CATax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcLine:
					foreach (CATax record in PXSelect<CATax,
						Where<CATax.adjTranType, Equal<Current<CAAdj.adjTranType>>,
							And<CATax.adjRefNbr, Equal<Current<CAAdj.adjRefNbr>>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& ((CATax)(PXResult<CATax, Tax, TaxRev>)ret[idx - 1]).LineNbr == record.LineNbr
								&& String.Compare(((Tax)(PXResult<CATax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<CATax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcTotals:
					foreach (CATaxTran record in PXSelect<CATaxTran,
						Where<CATaxTran.module, Equal<BatchModule.moduleCA>,
							And<CATaxTran.tranType, Equal<Current<CAAdj.adjTranType>>,
							And<CATaxTran.refNbr, Equal<Current<CAAdj.adjRefNbr>>>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (record.TaxID != null && tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<CATaxTran, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<CATaxTran, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				default:
					return ret;
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			if (sender.Graph is CATranEntry)
			{
				base.CacheAttached(sender);
			}
			else
			{
				this.TaxCalc = TaxCalc.NoCalc;
			}
		}

		protected override decimal CalcLineTotal(PXCache sender, object row)
		{
			if (sender.Graph is CATranEntry)
			{
				decimal curyLineTotal = 0m;
				foreach (CASplit detrow in ((CATranEntry)sender.Graph).CASplitRecords.View.SelectMultiBound(new object[1] { row }))
				{
					curyLineTotal += detrow.CuryTranAmt.GetValueOrDefault();
				}
				return curyLineTotal;
			}
			else
			{
				return base.CalcLineTotal(sender, row);
			}
		}

		protected override void SetTaxableAmt(PXCache sender, object row, decimal? value)
		{
			sender.SetValue<CASplit.curyTaxableAmt>(row, value);
		}

		protected override void SetTaxAmt(PXCache sender, object row, decimal? value)
		{
			sender.SetValue<CASplit.curyTaxAmt>(row, value);
		}

		protected override decimal? GetTaxableAmt(PXCache sender, object row)
		{
			return (decimal?)sender.GetValue<CASplit.curyTaxableAmt>(row);
		}

		protected override decimal? GetTaxAmt(PXCache sender, object row)
		{
			return (decimal?)sender.GetValue<CASplit.curyTaxAmt>(row);
		}

		protected override void SetExtCostExt(PXCache sender, object child, decimal? value)
		{
			CASplit row = child as CASplit;
			if (row != null)
			{
				row.CuryTranAmt = value;
				sender.Update(row);
			}
		}

		protected override string GetExtCostLabel(PXCache sender, object row)
		{
			return ((PXDecimalState) sender.GetValueExt<CASplit.curyTranAmt>(row)).DisplayName;
		}
		
		protected override bool isControlTaxTotalRequired(PXCache sender)
		{
			CASetup setup = new PXSetup<CASetup>(sender.Graph).Select();
			return setup != null && setup.RequireControlTaxTotal == true;
		}

		protected override bool isControlTotalRequired(PXCache sender)
		{
			CASetup setup = new PXSetup<CASetup>(sender.Graph).Select();
			return setup != null && setup.RequireControlTotal == true;
		}

	} 
	#endregion

	#region CashTranID family
	#region TransferCashTranIDAttribute
	/// <summary>
	/// Specialized for the Transfer version of the CashTranID attribute<br/>
	/// Defines methods to create new CATran from (and for) CATransfer document<br/>
	/// Should be used on the CATransfer - derived types only.
	/// <example>
	/// [TransferCashTranID()]
	/// </example>
	/// </summary>
	public class TransferCashTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			CATransfer parentDoc = (CATransfer)orig_Row;
			if ((parentDoc.Released == true) && (catran_Row.TranID != null))
			{
				return null;
			}
            if (catran_Row.TranID == null || catran_Row.TranID < 0)
            {
                catran_Row.OrigModule = BatchModule.CA;
                catran_Row.OrigRefNbr = parentDoc.TransferNbr;
            }

			if (object.Equals(_FieldOrdinal, sender.GetFieldOrdinal<CATransfer.tranIDOut>()))
			{
				catran_Row.CashAccountID = parentDoc.OutAccountID;
				catran_Row.OrigTranType = CATranType.CATransferOut;
				catran_Row.ExtRefNbr = parentDoc.OutExtRefNbr;
				catran_Row.CuryID = parentDoc.OutCuryID;
				catran_Row.CuryInfoID = parentDoc.OutCuryInfoID;
				catran_Row.CuryTranAmt = -parentDoc.CuryTranOut;
				catran_Row.TranAmt = -parentDoc.TranOut;
				catran_Row.DrCr = DrCr.Credit;
				catran_Row.Cleared = parentDoc.ClearedOut;
				catran_Row.ClearDate = parentDoc.ClearDateOut;
				catran_Row.TranDate = parentDoc.OutDate;
			}
			else if (object.Equals(_FieldOrdinal, sender.GetFieldOrdinal<CATransfer.tranIDIn>()))
			{
				catran_Row.CashAccountID = parentDoc.InAccountID;
				catran_Row.OrigTranType = CATranType.CATransferIn;
				catran_Row.ExtRefNbr = parentDoc.InExtRefNbr;
				catran_Row.CuryID = parentDoc.InCuryID;
				catran_Row.CuryInfoID = parentDoc.InCuryInfoID;
				catran_Row.CuryTranAmt = parentDoc.CuryTranIn;
				catran_Row.TranAmt = parentDoc.TranIn;
				catran_Row.DrCr = DrCr.Debit;
				catran_Row.Cleared = parentDoc.ClearedIn;
				catran_Row.ClearDate = parentDoc.ClearDateIn;
				catran_Row.TranDate = parentDoc.InDate;
			}
			else
			{
			    throw new PXException(AP.Messages.UnknownDocumentType);    
			}
			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}

			catran_Row.TranDesc = parentDoc.Descr;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;

			return catran_Row;
		}

		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is TransferCashTranIDAttribute)
				{
					((TransferCashTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((TransferCashTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}
	}
	#endregion

	#region AdjCashTranIDAttribute
	public class AdjCashTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			CAAdj parentDoc = (CAAdj)orig_Row;
			if ((parentDoc.Released == true) && (catran_Row.TranID != null))
			{
				return null;
            }
            if (catran_Row.TranID == null || catran_Row.TranID < 0)
            {
                catran_Row.OrigModule = BatchModule.CA;
                catran_Row.OrigTranType = parentDoc.AdjTranType;

                if (parentDoc.TransferNbr == null)
                {
                    catran_Row.OrigRefNbr = parentDoc.AdjRefNbr;
                }
                else
                {
                    catran_Row.OrigRefNbr = parentDoc.TransferNbr;
                }
            }

			catran_Row.CashAccountID = parentDoc.CashAccountID;
			catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
			catran_Row.CuryID = parentDoc.CuryID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.CuryTranAmt = parentDoc.CuryTranAmt * (parentDoc.DrCr == DrCr.Debit ? 1 : -1);
			catran_Row.TranAmt = parentDoc.TranAmt * (parentDoc.DrCr == DrCr.Debit ? 1 : -1);
			catran_Row.DrCr = parentDoc.DrCr;
			catran_Row.TranDate = parentDoc.TranDate;
			catran_Row.TranDesc = parentDoc.TranDesc;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;
			catran_Row.FinPeriodID = parentDoc.FinPeriodID;
			catran_Row.TranPeriodID = parentDoc.TranPeriodID;
			catran_Row.Cleared = parentDoc.Cleared;
			catran_Row.ClearDate = parentDoc.ClearDate;

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}

			return catran_Row;
		}

		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is AdjCashTranIDAttribute)
				{
					((AdjCashTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((AdjCashTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}
	}
	#endregion

	#region DepositTranIDAttribute
	public class DepositTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			return DefaultValues(sender, catran_Row, (CADeposit)orig_Row, this._FieldName);
		}

		public static CATran DefaultValues(PXCache sender, CATran catran_Row, CADeposit parentDoc, string fieldName)
		{
			if ((parentDoc.Released == true) && (catran_Row.TranID != null))
			{
				return null;
			}
			
				catran_Row.OrigModule = BatchModule.CA;
				catran_Row.OrigTranType = parentDoc.TranType;
				catran_Row.OrigRefNbr = parentDoc.RefNbr;
			

			catran_Row.CashAccountID = parentDoc.CashAccountID;
			catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
			catran_Row.CuryID = parentDoc.CuryID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.CuryTranAmt = parentDoc.CuryTranAmt * (parentDoc.DrCr == DrCr.Debit ? 1 : -1);
			catran_Row.TranAmt = parentDoc.TranAmt * (parentDoc.DrCr == DrCr.Debit ? 1 : -1);
			catran_Row.DrCr = parentDoc.DrCr;
			catran_Row.TranDate = parentDoc.TranDate;
			catran_Row.TranDesc = parentDoc.TranDesc;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;
			catran_Row.FinPeriodID = parentDoc.FinPeriodID;
			catran_Row.TranPeriodID = parentDoc.TranPeriodID;
			catran_Row.Cleared = parentDoc.Cleared;
			catran_Row.ClearDate = parentDoc.ClearDate;
			if (parentDoc.DocType == CATranType.CAVoidDeposit)
			{
				CADeposit voidedDoc = PXSelectReadonly<CADeposit, Where<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>,
												And<CADeposit.tranType, Equal<Required<CADeposit.tranType>>>>>.Select(sender.Graph, parentDoc.RefNbr, CATranType.CADeposit);
				if (voidedDoc != null)
				{
					catran_Row.VoidedTranID = (long?)sender.GetValue(voidedDoc, fieldName);
				}
			}


			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}
			return catran_Row;
		}

		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is DepositTranIDAttribute)
				{
					((DepositTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((DepositTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}
	}
	#endregion

	#region DepositCashTranIDAttribute
	public class DepositCashTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			return DefaultValues(sender, catran_Row, (CADeposit)orig_Row, this._FieldName);
		}

		public static CATran DefaultValues(PXCache sender, CATran catran_Row, CADeposit parentDoc, string fieldName)
		{
			if ((parentDoc.Released == true) && (catran_Row.TranID != null) ||
				IsCreationNeeded(parentDoc) == false)
			{
				return null;
			}
			
				catran_Row.OrigModule = BatchModule.CA;
				catran_Row.OrigTranType = parentDoc.TranType;
				catran_Row.OrigRefNbr = parentDoc.RefNbr;
			
			catran_Row.CashAccountID = parentDoc.ExtraCashAccountID;
			catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
			catran_Row.CuryID = parentDoc.CuryID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.DrCr = parentDoc.DrCr == CADrCr.CADebit ? CADrCr.CACredit : CADrCr.CADebit;
			catran_Row.CuryTranAmt = parentDoc.CuryExtraCashTotal * (catran_Row.DrCr == CADrCr.CADebit ? 1 : -1);
			catran_Row.TranAmt = parentDoc.ExtraCashTotal * (catran_Row.DrCr == CADrCr.CADebit ? 1 : -1);

			catran_Row.TranDate = parentDoc.TranDate;
			catran_Row.TranDesc = parentDoc.TranDesc;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;
			catran_Row.FinPeriodID = parentDoc.FinPeriodID;
			catran_Row.TranPeriodID = parentDoc.TranPeriodID;
			catran_Row.Cleared = parentDoc.Cleared;
			catran_Row.ClearDate = parentDoc.ClearDate;
			if (parentDoc.DocType == CATranType.CAVoidDeposit)
			{
				CADeposit voidedDoc = PXSelectReadonly<CADeposit, Where<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>,
												And<CADeposit.tranType, Equal<Required<CADeposit.tranType>>>>>.Select(sender.Graph, parentDoc.RefNbr, CATranType.CADeposit);
				if (voidedDoc != null)
				{
					catran_Row.VoidedTranID = (long?)sender.GetValue(voidedDoc, fieldName);
				}
			}

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}
			return catran_Row;
		}

		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is DepositCashTranIDAttribute)
				{
					((DepositCashTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((DepositCashTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{

				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}

		protected static bool IsCreationNeeded(CADeposit parentDoc)
		{
			return (parentDoc.ExtraCashAccountID != null
					&& parentDoc.ExtraCashTotal != null
					&& parentDoc.ExtraCashTotal != Decimal.Zero);
		}
	}
	#endregion

	#region DepositChargeTranIDAttribute
	public class DepositChargeTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			return DefaultValues(sender, catran_Row, (CADeposit)orig_Row, this._FieldName);
		}

		public static CATran DefaultValues(PXCache sender, CATran catran_Row, CADeposit parentDoc, string fieldName)
		{
			if ((parentDoc.Released == true) && (catran_Row.TranID != null)
				|| IsCreationNeeded(parentDoc) == false)
			{
				return null;
			}
			
				catran_Row.OrigModule = BatchModule.CA;
				catran_Row.OrigTranType = parentDoc.TranType;
				catran_Row.OrigRefNbr = parentDoc.RefNbr;
			
			catran_Row.CashAccountID = parentDoc.CashAccountID;
			catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
			catran_Row.CuryID = parentDoc.CuryID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.DrCr = parentDoc.DrCr == CADrCr.CADebit ? CADrCr.CACredit : CADrCr.CADebit;
			catran_Row.CuryTranAmt = parentDoc.CuryChargeTotal * (catran_Row.DrCr == CADrCr.CADebit ? 1 : -1);
			catran_Row.TranAmt = parentDoc.ChargeTotal * (catran_Row.DrCr == CADrCr.CADebit ? 1 : -1);
			catran_Row.TranDate = parentDoc.TranDate;
			catran_Row.TranDesc = parentDoc.TranDesc;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;
			catran_Row.FinPeriodID = parentDoc.FinPeriodID;
			catran_Row.TranPeriodID = parentDoc.TranPeriodID;
			catran_Row.Cleared = parentDoc.Cleared;
			catran_Row.ClearDate = parentDoc.ClearDate;
			if (parentDoc.DocType == CATranType.CAVoidDeposit)
			{
				CADeposit voidedDoc = PXSelectReadonly<CADeposit, Where<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>,
												And<CADeposit.tranType, Equal<Required<CADeposit.tranType>>>>>.Select(sender.Graph, parentDoc.RefNbr, CATranType.CADeposit);
				if (voidedDoc != null)
				{
					catran_Row.VoidedTranID = (long?)sender.GetValue(voidedDoc, fieldName);
				}
			}

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}
			return catran_Row;
		}


		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is DepositCashTranIDAttribute)
				{
					((DepositChargeTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((DepositChargeTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
				{
					object key = sender.GetValue(e.Row, _FieldOrdinal);
					PXCache cache = sender.Graph.Caches[typeof(CATran)];
					CATran info = null;
					if (key != null)
					{
						info = PXSelect<CATran, Where<CATran.tranID, Equal<Required<CATran.tranID>>>>.Select(sender.Graph, key);
						if (info == null)
						{
							key = null;
							sender.SetValue(e.Row, _FieldOrdinal, null);
						}
					}
					if (info != null && !IsCreationNeeded((CADeposit)e.Row))
					{
						cache.Delete(info);
						cache.PersistDeleted(info);
					}
				}
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}
		protected static bool IsCreationNeeded(CADeposit parentDoc)
		{
			return !(parentDoc.ChargesSeparate != true || parentDoc.CuryChargeTotal == null ||
				 parentDoc.CuryChargeTotal == 0);

		}
	}
	#endregion

	#region DepositDetailTranIDAttribute
	public class DepositDetailTranIDAttribute : CashTranIDAttribute
	{
		protected bool _IsIntegrityCheck = false;

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			return DepositDetailTranIDAttribute.DefaultValues(sender, catran_Row, (CADepositDetail)orig_Row);
		}

		public static CATran DefaultValues(PXCache sender, CATran catran_Row, CADepositDetail orig_Row)
		{
			CADepositDetail parentDoc = orig_Row;
			CADeposit deposit = PXSelect<CADeposit, Where<CADeposit.tranType, Equal<Required<CADeposit.tranType>>,
															And<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>>.Select(sender.Graph, parentDoc.TranType, parentDoc.RefNbr);
			if ((parentDoc.Released == true) && (catran_Row.TranID != null) ||
				 parentDoc.CuryTranAmt == null ||
				 parentDoc.CuryTranAmt == 0)
			{
				return null;
			}
			
				catran_Row.OrigModule = BatchModule.CA;
				catran_Row.OrigTranType = parentDoc.TranType;
				catran_Row.OrigRefNbr = parentDoc.RefNbr;
			
			catran_Row.CashAccountID = parentDoc.AccountID;

			catran_Row.CuryID = parentDoc.OrigCuryID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.CuryTranAmt = parentDoc.CuryOrigAmtSigned * (parentDoc.DrCr == CADrCr.CADebit ? 1 : -1);
			catran_Row.TranAmt = parentDoc.OrigAmtSigned * (parentDoc.DrCr == CADrCr.CADebit ? 1 : -1);
			catran_Row.DrCr = parentDoc.DrCr;
			catran_Row.TranDesc = parentDoc.TranDesc;
			catran_Row.ReferenceID = null;
			catran_Row.Released = parentDoc.Released;
			if (parentDoc.DetailType == CADepositDetailType.CheckDeposit || parentDoc.DetailType == CADepositDetailType.VoidCheckDeposit)
				catran_Row.TranDesc = string.Format("{0}-{1}", parentDoc.OrigDocType, parentDoc.OrigRefNbr);
			if (deposit != null)
			{
				catran_Row.Hold = deposit.Hold;
				catran_Row.FinPeriodID = deposit.FinPeriodID;
				catran_Row.TranPeriodID = deposit.TranPeriodID;
				catran_Row.ExtRefNbr = deposit.ExtRefNbr;
				catran_Row.TranDate = deposit.TranDate;
				catran_Row.Cleared = deposit.Cleared;
				catran_Row.ClearDate = deposit.ClearDate;             
			}
			if (parentDoc.DetailType == CADepositDetailType.VoidCheckDeposit)
			{
				CADepositDetail voidedDoc = PXSelectReadonly<CADepositDetail, Where<CADepositDetail.refNbr, Equal<Required<CADepositDetail.refNbr>>,
												And<CADepositDetail.tranType, Equal<Required<CADepositDetail.tranType>>,
													And<CADepositDetail.lineNbr, Equal<Required<CADepositDetail.lineNbr>>>>>>.Select(sender.Graph, parentDoc.RefNbr, CATranType.CADeposit, parentDoc.LineNbr);
				if (voidedDoc != null)
				{
					catran_Row.VoidedTranID = voidedDoc.TranID;
				}
			}

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}
			return catran_Row;
		}


		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is DepositDetailTranIDAttribute)
				{
					((DepositDetailTranIDAttribute)attr)._IsIntegrityCheck = true;
					return ((DepositDetailTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
				}
			}
			return null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}
	}
	#endregion

	#region CashTranIDAttribute

	public abstract class CashTranIDAttribute : PXEventSubscriberAttribute, IPXRowPersistingSubscriber, IPXRowPersistedSubscriber, IPXRowDeletedSubscriber
	{
		#region State
		protected object _KeyToAbort;
		protected Type _ChildType;
		object _SelfKeyToAbort;
		Dictionary<long?, object> _persisted;
		#endregion

		public abstract CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row);

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_ChildType = sender.GetItemType();
			_persisted = new Dictionary<long?, object>();

			sender.Graph.RowPersisting.AddHandler<CATran>(CATran_RowPersisting);
			sender.Graph.RowPersisted.AddHandler<CATran>(CATran_RowPersisted);
		}

		public virtual void CATran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert)
			{
				_SelfKeyToAbort = sender.GetValue<CATran.tranID>(e.Row);
			}
		}

		public virtual void CATran_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			PXCache cache = sender.Graph.Caches[_ChildType];
			long? NewKey;

			if (e.Operation == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Open && _SelfKeyToAbort != null)
			{
				NewKey = (long?)sender.GetValue<CATran.tranID>(e.Row);

				if (!_persisted.ContainsKey(NewKey))
				{
					_persisted.Add(NewKey, _SelfKeyToAbort);
				}

				foreach (object item in cache.Inserted)
				{
					if ((long?)cache.GetValue(item, _FieldOrdinal) == (long?)_SelfKeyToAbort)
					{
						cache.SetValue(item, _FieldOrdinal, NewKey);
					}
				}

				foreach (object item in cache.Updated)
				{
					if ((long?)cache.GetValue(item, _FieldOrdinal) == (long?)_SelfKeyToAbort)
					{
						cache.SetValue(item, _FieldOrdinal, NewKey);
					}
				}

				_SelfKeyToAbort = null;
			}

			if (e.Operation == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Aborted)
			{
				foreach (object item in cache.Inserted)
				{
					if ((NewKey = (long?)cache.GetValue(item, _FieldOrdinal)) != null && _persisted.TryGetValue(NewKey, out _SelfKeyToAbort))
					{
						cache.SetValue(item, _FieldOrdinal, _SelfKeyToAbort);
					}
				}

				foreach (object item in cache.Updated)
				{
					if ((NewKey = (long?)cache.GetValue(item, _FieldOrdinal)) != null && _persisted.TryGetValue(NewKey, out _SelfKeyToAbort))
					{
						cache.SetValue(item, _FieldOrdinal, _SelfKeyToAbort);
					}
				}
			}
		}

		public virtual void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (sender.Graph.Views.Caches.Contains(typeof(CATran)))
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				PXCache cache = sender.Graph.Caches[typeof(CATran)];
				if (key != null)
				{
					CATran info = new CATran { TranID = (long?)key };
					cache.Delete(info);

					sender.SetValue(e.Row, _FieldOrdinal, null);
				}
			}
		}

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			object key = sender.GetValue(e.Row, _FieldOrdinal);
			PXCache cache = sender.Graph.Caches[typeof(CATran)];
			CATran info = null;

			if (key != null)
			{
				//do not read cached record in release processes
				if ((info = PXSelectReadonly<CATran, Where<CATran.tranID, Equal<Required<CATran.tranID>>>>.Select(sender.Graph, key)) != null)
				{
					CATran cached = (CATran)cache.Locate(info);
					if (cached != null)
					{
						if ((cached.OrigModule != null && info.OrigModule != null && cached.OrigModule != info.OrigModule) ||
							(cached.OrigRefNbr != null && info.OrigRefNbr != null && cached.OrigRefNbr != info.OrigRefNbr) ||
							(cached.OrigTranType != null && info.OrigTranType != null && cached.OrigTranType != info.OrigTranType))
							throw new PXException(Messages.CouldNotInsertCATran);// TO Be removed after solving CATran issue (check JIRA item AC-57875 for details)
						if (cache.GetStatus(cached) == PXEntryStatus.Notchanged)
						{
							PXCache<CATran>.RestoreCopy(cached, info);
						}
						info = cached;
					}
					else
					{
						cache.SetStatus(info, PXEntryStatus.Notchanged);
					}
				}

				if ((long)key < 0L && info == null)
				{
					info = new CATran();
					info.TranID = (long)key;
					info = (CATran)cache.Locate(info);
				}

				if (info == null)
				{
					key = null;
					sender.SetValue(e.Row, _FieldOrdinal, null);
				}
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				if (info != null)
				{
					cache.Delete(info);
					cache.PersistDeleted(info);
				}
			}
			else if (info == null)
			{
                if (!this.NeedPreventCashTransactionCreation(sender, e.Row))
                {
                    info = DefaultValues(sender, new CATran(), e.Row);
                    if (info != null)
                    {
                        info = (CATran)cache.Insert(info);
                        if (!(info.TranID < 0L))
                            throw new PXException(Messages.CouldNotInsertCATran);
                        // TO Be removed after solving CATran issue (check JIRA item AC-57875 for details)
                        sender.SetValue(e.Row, _FieldOrdinal, info.TranID);
                        _KeyToAbort = info.TranID;
                        cache.PersistInserted(info);
                        long id = info.TranID ?? 0;
                        if (id == 0 || id < 0)
                        {
                            throw new PXException(Messages.CATranNotSaved, sender.GetItemType().Name);
                        }
                        sender.SetValue(e.Row, _FieldOrdinal, id);
                        info.TranID = id;
                        cache.Normalize();
                    }
                }
			}
			else if (info.TranID < 0L)
			{
				info = DefaultValues(sender, PXCache<CATran>.CreateCopy(info), e.Row);
				if (info != null)
				{
				info = (CATran)cache.Update(info);

				sender.SetValue(e.Row, _FieldOrdinal, info.TranID);
				_KeyToAbort = info.TranID;
				cache.PersistInserted(info);
				long id = info.TranID ?? 0;
				if (id == 0 || id < 0)
				{
					throw new PXException(Messages.CATranNotSaved, sender.GetItemType().Name);
				}
				sender.SetValue(e.Row, _FieldOrdinal, id);
				info.TranID = id;
				cache.Normalize();
			}
			}
			else
			{
				CATran copy = PXCache<CATran>.CreateCopy(info);
				copy = DefaultValues(sender, copy, e.Row);
				if (copy != null)
				{
					if ((copy.OrigModule != null && info.OrigModule != null && copy.OrigModule != info.OrigModule) ||
						(copy.OrigRefNbr != null && info.OrigRefNbr != null && copy.OrigRefNbr != info.OrigRefNbr) ||
						(copy.OrigTranType != null && info.OrigTranType != null && copy.OrigTranType != info.OrigTranType))
						throw new PXException(Messages.CouldNotInsertCATran); // TO Be removed after solving CATran issue (check JIRA item AC-57875 for details)
					info = (CATran)cache.Update(copy);
					//to avoid another process updated DefaultValues will return null for Released docs, except for GLTran
					cache.PersistUpdated(info);
				}
				//JournalEntry is usually persisted prior to ReleaseGraph to obtain BatchNbr reference, read info should contain set Released flag
				else if (info.Released == false)
				{
					key = null;
					sender.SetValue(e.Row, _FieldOrdinal, null);

					cache.Delete(info);
				}
			}
			foreach (CATran toDelete in cache.Deleted)
			{
				cache.PersistDeleted(toDelete);
			}
		}

		public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			PXCache cache = sender.Graph.Caches[typeof(CATran)];

			if (e.TranStatus == PXTranStatus.Open)
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
			}
			else if (e.TranStatus == PXTranStatus.Aborted)
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				if (_KeyToAbort != null && (long)_KeyToAbort < 0L)
				{
					sender.SetValue(e.Row, _FieldOrdinal, _KeyToAbort);
					foreach (CATran data in cache.Inserted)
					{
						if (Equals(key, data.TranID))
						{
							data.TranID = (long)_KeyToAbort;
							cache.ResetPersisted(data);
							break;
						}
					}
				}
				else
				{
					foreach (CATran data in cache.Updated)
					{
						if (object.Equals(key, data.TranID))
						{
							cache.ResetPersisted(data);
						}
					}
				}

				cache.Normalize();
			}
			else
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				foreach (CATran data in cache.Inserted)
				{
					if (object.Equals(key, data.TranID))
					{
						cache.RaiseRowPersisted(data, PXDBOperation.Insert, e.TranStatus, e.Exception);
						cache.SetStatus(data, PXEntryStatus.Notchanged);
						PXTimeStampScope.PutPersisted(cache, data, sender.Graph.TimeStamp);
						cache.ResetPersisted(data);
					}
				}
				foreach (CATran data in cache.Updated)
				{
					if (object.Equals(key, data.TranID))
					{
						cache.RaiseRowPersisted(data, PXDBOperation.Update, e.TranStatus, e.Exception);
						cache.SetStatus(data, PXEntryStatus.Notchanged);
						PXTimeStampScope.PutPersisted(cache, data, sender.Graph.TimeStamp);
						cache.ResetPersisted(data);
					}
				}
				foreach (CATran data in cache.Deleted)
				{
					cache.RaiseRowPersisted(data, PXDBOperation.Delete, e.TranStatus, e.Exception);
					cache.SetStatus(data, PXEntryStatus.Notchanged);
					PXTimeStampScope.PutPersisted(cache, data, sender.Graph.TimeStamp);
					cache.ResetPersisted(data);
				}
				cache.IsDirty = false;
				cache.Normalize();
			}
		}

		/// <summary>
		/// Returns <c>true</c> if, during parent row persist, the
		/// corresponding cash transaction row should not be created, e.g.
		/// in case of a document that is part of a recurring schedule.
		/// </summary>
		protected virtual bool NeedPreventCashTransactionCreation(PXCache sender, object row)
		{
			return false;
		}
	}
	#endregion
	
	#endregion

	#region DynamicValueValidationAttribute

	/// <summary>
	/// This attribute allows to provide a dynamic validation rules for the field.<br/>
	/// The rule is defined as regexp and may be stored in some external field.<br/>
	/// In the costructor, one should provide a search method for this rule. <br/>
	/// <example>
	/// [DynamicValueValidation(typeof(Search<PaymentMethodDetail.validRegexp, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<VendorPaymentMethodDetail.paymentMethodID>>,
	///								   And<PaymentMethodDetail.detailID, Equal<Current<VendorPaymentMethodDetail.detailID>>>>>))]
	/// </example>
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class DynamicValueValidationAttribute : PXEventSubscriberAttribute, IPXFieldVerifyingSubscriber
	{
		#region State
		protected Type _RegexpSearch;
		protected Type _SourceType;
		protected string _SourceField;
		protected BqlCommand _Select;
		protected int _Length;
		#endregion

		#region Ctor
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="aRegexpSearch">Must be IBqlSearch returning a validation regexp.</param>
		public DynamicValueValidationAttribute(Type aRegexpSearch)
		{
			if (aRegexpSearch == null)
			{
				throw new PXArgumentException("type", ErrorMessages.ArgumentNullException);
			}
			if (!typeof(IBqlSearch).IsAssignableFrom(aRegexpSearch))
			{
				throw new PXArgumentException("aSearchRegexp", Messages.ValueValidationNotValid);
			}
			_RegexpSearch = aRegexpSearch;
			_Select = BqlCommand.CreateInstance(aRegexpSearch);
			_SourceType = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
			_SourceField = ((IBqlSearch)_Select).GetField().Name;

		}

		#endregion

		#region Implementation

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			string value = (string)e.NewValue;
			if (!string.IsNullOrEmpty(value))
			{
				int? controlType;
				string regexp = this.FindValidationRegexp(sender, e.Row, out controlType);
				if (!this.ValidateValue(value, regexp))
					throw new PXSetPropertyException(Messages.ValueIsNotValid);

			}
		}

		protected virtual void MultiSelectFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			int? controlType;
			if (e.NewValue is string)
			{
				if (this.FindValidationRegexp(sender, e.Row, out controlType) == null)
				{
					if (controlType == 6 && ((string)e.NewValue).TrimEnd().Length > _Length)
					{
						throw new PXSetPropertyException(ErrorMessages.StringLengthExceeded, _FieldName);
					}
					else if (controlType == 5 && sender.Graph.IsImport && !sender.Graph.IsCopyPasteContext)
					{
						DateTime dt;
						if (DateTime.TryParse((string)e.NewValue, sender.Graph.Culture, System.Globalization.DateTimeStyles.None, out dt))
						{
							e.NewValue = dt.ToString("M/d/yyyy");
						}
					}
				}
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			foreach (PXDBStringAttribute attr in sender.GetAttributesReadonly(_FieldName).Where(attr => attr is PXDBStringAttribute).Cast<PXDBStringAttribute>())
			{
				_Length = attr.Length;
			}
			if (_Length > 0)
			{
				sender.Graph.FieldUpdating.AddHandler(sender.GetItemType(), _FieldName.ToLower(), MultiSelectFieldUpdating);
			}
		}

		protected virtual string FindValidationRegexp(PXCache sender, Object row, out int? controlType)
		{
			controlType = null;
			if (_Select != null)
			{
				PXView view = sender.Graph.TypedViews.GetView(_Select, false);
				object item = view.SelectSingleBound(new object[] { row });
				if (item != null && item is PXResult)
				{
					item = ((PXResult)item)[_SourceType];
				}
				string result = (string)sender.Graph.Caches[_SourceType].GetValue(item, _SourceField == null ? _FieldName : _SourceField);
				if (typeof(CSAttribute).IsAssignableFrom(_SourceType) && item != null)
				{
					controlType = ((int?)sender.Graph.Caches[_SourceType].GetValue<CSAttribute.controlType>(item));
				}
				return result;
			}
			return null;
		}

		protected virtual bool ValidateValue(string val, string regex)
		{
			if (val == null || regex == null)
				return true;
			System.Text.RegularExpressions.Regex regexobject = new System.Text.RegularExpressions.Regex(regex);
			return regexobject.IsMatch(val);
		}
		#endregion
		
	}
	#endregion

	#region PXDBStringWithMaskAttribute
	/// <summary>
	/// This attribute defines a PXDBStringAttribute with a dynamically created entry mask <br/>
	/// Should be used, when the entry mask must be set during a runtime, rather then at compile-time <br/>
	/// May be combined with the DynamicValueValidationAttribute <br/>
	/// <example>
	/// [PXDBStringWithMask(255,typeof(Search<PaymentMethodDetail.entryMask, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<VendorPaymentMethodDetail.paymentMethodID>>,
	///								   And<PaymentMethodDetail.detailID, Equal<Current<VendorPaymentMethodDetail.detailID>>>>>),IsUnicode = true)]
	/// </example>
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Method)]
	public class PXDBStringWithMaskAttribute : PXDBStringAttribute, IPXFieldSelectingSubscriber 
	{
		#region State
		protected Type _MaskSearch;
		protected Type _SourceType;
		protected string _SourceField;
		protected BqlCommand _Select;
		#endregion

		#region Ctor

		/// <summary>        
		/// </summary>
		/// <param name="length">Length of the string in the database. Passed to PXDBString.</param>
		/// <param name="aMaskSearch">Must be a IBqlSearch type returning a valid mask expression</param>
		public PXDBStringWithMaskAttribute(int length,Type aMaskSearch):base(length)
		{
			AssignMaskSearch(aMaskSearch);
		}
		///<summary>Calls the default constructor of the PXDBString</summary>
		/// <param name="aMaskSearch">Must be a IBqlSearch type returning a valid mask expression</param>
		public PXDBStringWithMaskAttribute(Type aMaskSearch)
			: base()
		{
			AssignMaskSearch(aMaskSearch);
		}
		#endregion
		protected virtual void AssignMaskSearch(Type aMaskSearch)
		{
			if (aMaskSearch == null)
			{
				throw new PXArgumentException("type", ErrorMessages.ArgumentNullException);
			}
			if (!typeof(IBqlSearch).IsAssignableFrom(aMaskSearch))
			{
				throw new PXArgumentException("aMaskSearch", Messages.ValueMaskNotValid);
			}
			this._MaskSearch = aMaskSearch;
			this._Select = BqlCommand.CreateInstance(aMaskSearch);
			this._SourceType = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
			this._SourceField = ((IBqlSearch)_Select).GetField().Name;
		}
		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				string mask = this.FindMask(sender, e.Row);
				if (!string.IsNullOrEmpty(mask))
				{
					e.ReturnState = PXStringState.CreateInstance(e.ReturnState, _Length, null, _FieldName, _IsKey, null, mask, null, null, null, null);
				}
				else
					base.FieldSelecting(sender, e);
			}
			else
			{
				base.FieldSelecting(sender, e);
			}
		}
		protected virtual string FindMask(PXCache sender, Object row)
		{
			if (_Select != null)
			{
				PXView view = sender.Graph.TypedViews.GetView(_Select, false);
				object item = view.SelectSingleBound(new object[] { row });
				if (item != null && item is PXResult)
				{
					item = ((PXResult)item)[_SourceType];
				}
				string result = (string)sender.Graph.Caches[_SourceType].GetValue(item, _SourceField == null ? _FieldName : _SourceField);
				return result;
			}
			return null;
		}

	}
	#endregion

	#region PXRSACryptStringWithMaskAttribute
	/// <summary>
	/// This as a specialized version of PXRSACryptStringAttribute<br/>
	/// which allows to define entry mask dynamically. Works identically to the PXDBStringWithMask <br/>
	/// and PXRSACryptString attributes - namely providing run-time entry mask definition for the crypted strings.<br/> 
	/// <example>
	/// [PXRSACryptStringWithMaskAttribute(1028, 
	///     typeof(Search<PaymentMethodDetail.entryMask, 
	///         Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CustomerPaymentMethodDetail.paymentMethodID>>,
	///			  And<PaymentMethodDetail.detailID, Equal<Current<CustomerPaymentMethodDetail.detailID>>>>>), IsUnicode = true)]
	/// <example/>	
	/// </summary>
	public class PXRSACryptStringWithMaskAttribute : PXRSACryptStringAttribute, IPXFieldSelectingSubscriber
	{
		#region State
		protected Type _MaskSearch;
		protected Type _SourceType;
		protected string _SourceField;
		protected BqlCommand _Select;
		#endregion
		#region Ctor

		/// <summary>        
		/// </summary>
		/// <param name="length">Length of the string in the database. Passed to PXDBString.</param>
		/// <param name="aMaskSearch">Must be a IBqlSearch type returning a valid mask expression</param>		
		public PXRSACryptStringWithMaskAttribute(int length, Type aMaskSearch)
			: base(length)
		{
			AssignMaskSearch(aMaskSearch);
		}

		///<summary>Calls the default constructor of the PXDBString</summary>
		/// <param name="aMaskSearch">Must be a IBqlSearch type returning a valid mask expression</param>
		public PXRSACryptStringWithMaskAttribute(Type aMaskSearch)
			: base()
		{
			AssignMaskSearch(aMaskSearch);
		}
		#endregion
		protected virtual void AssignMaskSearch(Type aMaskSearch)
		{
			if (aMaskSearch == null)
			{
				throw new PXArgumentException("type", ErrorMessages.ArgumentNullException);
			}
			if (!typeof(IBqlSearch).IsAssignableFrom(aMaskSearch))
			{
				throw new PXArgumentException("aMaskSearch", Messages.ValueMaskNotValid);
			}
			this._MaskSearch = aMaskSearch;
			this._Select = BqlCommand.CreateInstance(aMaskSearch);
			this._SourceType = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
			this._SourceField = ((IBqlSearch)_Select).GetField().Name;
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				string mask = this.FindMask(sender, e.Row);
				if (!string.IsNullOrEmpty(mask))
				{
					if (!string.IsNullOrEmpty((string)e.ReturnValue))
					{
						string viewAs = mask.Replace("#", "0").Replace("-", "").Replace("/", "");
						this.ViewAsString = viewAs;
						base.FieldSelecting(sender, e);

						PXStringState state = e.ReturnState as PXStringState;
						if (state != null)
						{
							e.ReturnState = PXStringState.CreateInstance(e.ReturnState, _Length, null, _FieldName, _IsKey, null, mask, null, null, null, null);
						}						
					}
					else
					{
						e.ReturnState = PXStringState.CreateInstance(e.ReturnState, _Length, null, _FieldName, _IsKey, null, mask, null, null, null, null);
					}
				}
				else
					base.FieldSelecting(sender, e);
			}
			else 
			{
				base.FieldSelecting(sender, e);
			}
		}
		protected virtual string FindMask(PXCache sender, Object row)
		{
			if (_Select != null)
			{
				PXView view = sender.Graph.TypedViews.GetView(_Select, false);
				object item = view.SelectSingleBound(new object[] { row });
				if (item != null && item is PXResult)
				{
					item = ((PXResult)item)[_SourceType];
				}
				string result = (string)sender.Graph.Caches[_SourceType].GetValue(item, _SourceField == null ? _FieldName : _SourceField);
				return result;
			}
			return null;
		}
	}
	#endregion
	#region PXRSACryptStringWithConditionalAttribute
	/// <summary>
	/// Works very much like PXRSACryptStringAttribute. Encryption is used conditionally, depending on value of EncryptionRequiredField.
	/// isEncryptedField is used to store current state of field - encrypted or not.
	/// </summary>
	/// <param name="EncryptionRequiredField">BQL field</param>
	/// <param name="isEncryptedField">BQL field</param>
	public class PXRSACryptStringWithConditionalAttribute : PXRSACryptStringAttribute, IPXRowPersistingSubscriber
	{
		protected Type _EncryptionRequiredField;
		protected Type _isEncryptedField;

		public PXRSACryptStringWithConditionalAttribute(Type EncryptionRequiredField, Type isEncryptedField)
			: base()
		{
			checkParams(EncryptionRequiredField, isEncryptedField);
			_EncryptionRequiredField = EncryptionRequiredField;
			_isEncryptedField = isEncryptedField;
		}

		public PXRSACryptStringWithConditionalAttribute(int length, Type EncryptionRequiredField, Type isEncryptedField)
			: base(length)
		{
			checkParams(EncryptionRequiredField, isEncryptedField);
			_EncryptionRequiredField = EncryptionRequiredField;
			_isEncryptedField = isEncryptedField;
		}

		private void checkParams(Type EncryptionRequiredField, Type isEncryptedField)
		{
			if (EncryptionRequiredField == null)
			{
				throw new PXArgumentException("EncryptionRequiredField", ErrorMessages.ArgumentNullException);
			}
			if (isEncryptedField == null)
			{
				throw new PXArgumentException("isEncryptedField", ErrorMessages.ArgumentNullException);
			}
			if (!typeof(IBqlField).IsAssignableFrom(EncryptionRequiredField))
			{
				throw new PXArgumentException("EncryptionRequiredField", Messages.ShouldContainBQLField);
			}
			if (!typeof(IBqlField).IsAssignableFrom(isEncryptedField))
			{
				throw new PXArgumentException("isEncryptedField", Messages.ShouldContainBQLField);
			}
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				bool? isEncrypted = (sender.GetValue(e.Row, _isEncryptedField.Name) as bool?) ?? false;
				isViewDeprypted = (isEncrypted == false);
			}
			base.FieldSelecting(sender, e);
		}

		public override void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				bool? isEncrypted = (sender.GetValue(e.Row, _isEncryptedField.Name) as bool?) ?? false;
				isEncryptionRequired = (isEncrypted == true);
			}
			base.RowSelecting(sender, e);
		}

		public void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			bool? EncryptionRequired = sender.GetValue(e.Row, _EncryptionRequiredField.Name) as bool?;
			sender.SetValue(e.Row, _isEncryptedField.Name, EncryptionRequired == true);
		}

		public override void CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				bool? EncryptionRequired = sender.GetValue(e.Row, _EncryptionRequiredField.Name) as bool?;
				isEncryptionRequired = (EncryptionRequired == true);
			}
			base.CommandPreparing(sender, e);
		}

		public override bool EncryptOnCertificateReplacement(PXCache cache, object row)
		{
			bool? isEncrypted = cache.GetValue(row, _isEncryptedField.Name) as bool?;
			return isEncrypted == true;
		}
	}
	#endregion

	#region CATranRefNbr
	[System.SerializableAttribute()]
	public partial class CATranRefNbr : PX.Data.IBqlTable
	{
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Ref. Number")]
		[PXSelector(typeof(Search<CATran.origRefNbr, Where<CATran.origModule, Equal<GL.BatchModule.moduleCA>,
			And<CATran.origTranType, Like<Optional<CATran.origTranType>>>>>),
			typeof(CATran.origRefNbr), typeof(CATran.origTranType), typeof(CATran.tranDate), typeof(CATran.finPeriodID))]
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
	} 
	#endregion

	#region PXVendorCustomerSelectorAttribute

	/// <summary>
	/// Selector. Allows to select either vendors or customers depending <br/>
	/// upon the value in the BatchModule field. Will return a list of Customers for AR<br/>
	/// and a list of Vendors for AP, other types are not supported. If a currency ID is provided, <br/>
	/// list of Vendors/Customers will be restricted by those, having this CuryID set <br/>
	/// or having AllowOverrideCury set on. For example this allows to select only Customers/Vendors <br/>
	/// which may pay to/from a specific cash account<br/>
	/// <example> 
	/// [PXVendorCustomerSelector(typeof(CABankStatementDetail.origModule))]
	/// </example>
	/// </summary>    
	[Serializable]
	public class PXVendorCustomerSelectorAttribute : PXCustomSelectorAttribute
	{
		protected Type BatchModule;
		[Serializable]
        [PXHidden]
		public partial class VendorR : Vendor
		{
			#region BAccountID
			public new abstract class bAccountID : PX.Data.IBqlField
			{
			}
			#endregion
            public new abstract class curyID : IBqlField {}
            public new abstract class allowOverrideCury : IBqlField { }
		}
		[Serializable]
        [PXHidden]
		public partial class CustomerR : Customer
		{
			#region BAccountID
			public new abstract class bAccountID : PX.Data.IBqlField
			{
			}
			#endregion
            public new abstract class curyID : IBqlField { }
            public new abstract class allowOverrideCury : IBqlField { }

		}
		protected Type _CuryID;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="BatchModule">Must be IBqlField. Refers to the BatchModule field of the row.</param>
		public PXVendorCustomerSelectorAttribute(Type BatchModule)
			: base(typeof(Search<BAccountR.bAccountID, Where<True, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>),
				   typeof(BAccountR.acctCD), 
				   typeof(BAccountR.acctName))
		{
			this.SubstituteKey    = typeof(BAccountR.acctCD);
			this.DescriptionField = typeof(BAccountR.acctName);
			this.BatchModule      = BatchModule;
			this._CuryID = null;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="BatchModule">Must be IBqlField. Refers to the BatchModule field of the row.</param>		
		/// <param name="CuryID">Must be IBqlField.Refers to the CuryID field of the row.</param>
		public PXVendorCustomerSelectorAttribute(Type BatchModule, Type CuryID)
			: base(typeof(Search<BAccountR.bAccountID, Where<True, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>),
				   typeof(BAccountR.acctCD),
				   typeof(BAccountR.acctName))
		{
			this.SubstituteKey = typeof(BAccountR.acctCD);
			this.DescriptionField = typeof(BAccountR.acctName);
			this.BatchModule = BatchModule;
			this._CuryID = CuryID;
		}
				
		protected virtual void GetRecords()
		{			
			PXCache cache = this._Graph.Caches[BqlCommand.GetItemType(this.BatchModule)];
			object current = null;
			foreach (object item in PXView.Currents)
			{
				if (item != null && (item.GetType() == BqlCommand.GetItemType(this.BatchModule) || item.GetType().IsSubclassOf(BqlCommand.GetItemType(this.BatchModule))))
				{
					current = item;
					break;
				}
			}

            if (current == null)
            {
                current = cache.Current;
            }

            PXView view = PXView.View;
            if (view != null)
            {
                if (_CuryID != null)
                {
                    Object curyID = cache.GetValue(current, this._CuryID.Name) ?? cache.GetValuePending(current, _CuryID.Name);
                    PX.Common.PXContext.SetSlot(this._CuryID.FullName, curyID);
                }
                switch ((string)(cache.GetValue(current, this.BatchModule.Name) ?? cache.GetValuePending(current, this.BatchModule.Name)))
                {
                    case GL.BatchModule.AP:
						view.WhereNew<Where2<Match<VendorR, Current<AccessInfo.userName>>, And<Where<BAccountR.type, Equal<BAccountType.vendorType>, Or<BAccountR.type, Equal<BAccountType.employeeType>, Or<BAccountR.type, Equal<BAccountType.combinedType>, Or<BAccountR.type, Equal<BAccountType.empCombinedType>>>>>>>>();

                        if (_CuryID != null)
                        {
                            Type join = BqlCommand.Compose(
                                typeof(InnerJoin<,>),
                                typeof(VendorR),
                                typeof(On<,,>),
                                typeof(VendorR.bAccountID),
                                typeof(Equal<>),
                                typeof(BAccountR.bAccountID),
                                typeof(And<>),
                                typeof(Where<,,>),
                                typeof(VendorR.curyID),
                                typeof(Equal<>),
                                typeof(SavedStringValue<>),
                                _CuryID,
                                typeof(Or<,,>),
                                typeof(VendorR.allowOverrideCury),
                                typeof(Equal<>),
                                typeof(True),
								typeof(Or<,>),
								typeof(VendorR.curyID),
								typeof(IsNull));

                            view.JoinNew(join);
                        }
						else
						{
							Type join = BqlCommand.Compose(
								typeof(InnerJoin<,>),
								typeof(VendorR),
								typeof(On<,>),
								typeof(VendorR.bAccountID),
								typeof(Equal<>),
								typeof(BAccountR.bAccountID));

							view.JoinNew(join);
						}

                        break;
                    case GL.BatchModule.AR:
						view.WhereNew<Where2<Match<CustomerR, Current<AccessInfo.userName>>, And<Where<BAccountR.type, Equal<BAccountType.customerType>, Or<BAccountR.type, Equal<BAccountType.combinedType>, Or<BAccountR.type, Equal<BAccountType.empCombinedType>>>>>>>();

                        if (_CuryID != null)
                        {
                            Type join = BqlCommand.Compose(
                                typeof(InnerJoin<,>),
                                typeof(CustomerR),
                                typeof(On<,,>),
                                typeof(CustomerR.bAccountID),
                                typeof(Equal<>),
                                typeof(BAccountR.bAccountID),
                                typeof(And<>),
                                typeof(Where<,,>),
                                typeof(CustomerR.curyID),
                                typeof(Equal<>),
                                typeof(SavedStringValue<>),
                                _CuryID,
                                typeof(Or<,,>),
                                typeof(CustomerR.allowOverrideCury),
                                typeof(Equal<>),
                                typeof(True),
								typeof(Or<,>),
								typeof(CustomerR.curyID),
								typeof(IsNull));

                            view.JoinNew(join);
                        }
						else
						{
							Type join = BqlCommand.Compose(
								typeof(InnerJoin<,>),
								typeof(CustomerR),
								typeof(On<,>),
								typeof(CustomerR.bAccountID),
								typeof(Equal<>),
								typeof(BAccountR.bAccountID));

							view.JoinNew(join);
						}
                        break;
                }
            }
            //return null;
		}

		private static object SelectSingleBound(PXView view, object[] currents, params object[] pars)
		{
			List<object> ret = view.SelectMultiBound(currents, pars);
			if (ret.Count > 0)
			{
				if (ret[0] is PXResult)
				{
					return ((PXResult)ret[0])[0];
				}
				return ret[0];
			}

			return null;
		}

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue == null || !ValidateValue)
			{
				return;
			}

			PXView view = GetView(sender, _PrimarySelect, !_DirtyRead);
			if (sender.Keys.Count == 0 || _FieldName != sender.Keys[sender.Keys.Count - 1])
			{
				object[] pars = new object[_ParsCount + 1];
				pars[pars.Length - 1] = e.NewValue;
				object item = null;
				try
				{
					item = SelectSingleBound(view, new object[] { e.Row }, pars);
				}
				catch (FormatException) { } // thrown by SqlServer
				catch (InvalidCastException) { } // thrown by MySql

				if (item == null)
				{
					throwNoItem(hasRestrictedAccess(sender, _PrimarySelect, e.Row), e.ExternalCall, e.NewValue);
				}
			}
		}

	}
	#endregion


	#region PXProviderTypeSelectorAttribute

	public class PXProviderTypeSelectorAttribute : PXCustomSelectorAttribute
	{
		private Type _providerInterfaceType;

		[Serializable]
        [PXHidden]
		public partial class ProviderRec : IBqlTable
		{
			#region TypeName
			public abstract class typeName : PX.Data.IBqlField { }
			[PXString(255, InputMask = "", IsKey = true)]
			[PXUIField(DisplayName = "Type Name", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string TypeName { get; set; }
			#endregion
		}

		public PXProviderTypeSelectorAttribute(Type providerType)
			: base(typeof(ProviderRec.typeName))
		{
			this._providerInterfaceType = providerType;
		}

		protected virtual IEnumerable GetRecords()
		{
			foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (PXSubstManager.IsSuitableTypeExportAssembly(ass, false))
				{
					Type[] types = null;
					try
					{
						if ( !ass.IsDynamic )
						types = ass.GetExportedTypes();
					}
					catch (ReflectionTypeLoadException te)
					{
						types = te.Types;
					}
					catch
					{
						continue;
					}
					if (types != null)
					{
						foreach (Type t in types)
						{
							if (t != null && this._providerInterfaceType.IsAssignableFrom(t) && t != this._providerInterfaceType)
							{
								yield return new ProviderRec { TypeName = t.FullName };
							}
						}
					}
				}
			}
		}
	} 
	#endregion

	#region CAOpenPeriodAttribute
	/// <summary>
	/// Specialized for CA version of the <see cref="OpenPeriodAttribut"/><br/>
	/// Selector. Provides  a list  of the active Fin. Periods, having CAClosed flag = false <br/>
	/// <example>
	/// [CAOpenPeriod(typeof(CATran.tranDate))]
	/// </example>
	/// </summary>
	public class CAOpenPeriodAttribute : OpenPeriodAttribute
	{
		#region Ctor

		/// <summary>
		/// Extended Ctor. 
		/// </summary>
		/// <param name="SourceType">Must be IBqlField. Refers a date, based on which "current" period will be defined</param>
		public CAOpenPeriodAttribute(Type SourceType)
			: base(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.cAClosed, Equal<False>, And<FinPeriod.active, Equal<True>>>>), SourceType)
		{
		}

		public CAOpenPeriodAttribute()
			: this(null)
		{
		}
		#endregion

		#region Implementation
		public override void IsValidPeriod(PXCache sender, object Row, object NewValue)
		{
			string OldValue = (string)sender.GetValue(Row, _FieldName);
			base.IsValidPeriod(sender, Row, NewValue);

			if (NewValue != null && _ValidatePeriod != PeriodValidation.Nothing)
			{
				FinPeriod p = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(sender.Graph, (string)NewValue);
				if (p.CAClosed == true)
				{
					if (PostClosedPeriods(sender.Graph))
					{
						sender.RaiseExceptionHandling(_FieldName, Row, null, new FiscalPeriodClosedException(p.FinPeriodID, PXErrorLevel.Warning));
						return;
					}
					else
					{
						throw new FiscalPeriodClosedException(p.FinPeriodID);
					}
				}
			}
			return;
		}
		#endregion
	}
	#endregion
	
	#region UniqueBoolAttribute
	/// <summary>
	/// Attribute ensures that there is only one row with "true" value of given field within scope restricted by key value
	/// Works only on boolean fields. Key field must have PXParent Attribute on it
	/// <param name="key">Key field with PXParentAttribute on it</param>
	/// </summary>
	public class UniqueBoolAttribute : PXEventSubscriberAttribute, IPXRowInsertedSubscriber, IPXRowUpdatedSubscriber
	{
		private Type _key;
		private PXParentAttribute _parentAttribute;
		#region Ctor
		public UniqueBoolAttribute(Type key)
		{
			if (key == null)
			{
				throw new PXArgumentException("scope", Messages.ParameterShouldNotNull);
			}
			if (!(key.IsNested && typeof(IBqlField).IsAssignableFrom(key)))
			{
				throw new PXArgumentException("scope", "{0} is not a BqlField", key.Name);
			}
			_key = key;
		}
		#endregion

		#region Implementation
		private void findParentAttribute(PXCache sender, object row)
		{
			if (_parentAttribute != null) return;
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributesReadonly(row, _key.Name))
			{
				if (attr is PXParentAttribute)
				{
					_parentAttribute = (PXParentAttribute)attr;
					break;
				}
			}
		}

		private void updateOtherSiblings(PXCache sender, object row)
		{
			bool needRefresh = false;
			if (_parentAttribute == null) return;
			PXFieldState state = (PXFieldState)sender.GetStateExt(row, _FieldName);
			if (state.DataType != typeof(bool)) return;
			if ((bool?) sender.GetValue(row, _FieldName) == false) return;
			foreach (object sibling in PXParentAttribute.SelectSiblings(sender, row, _parentAttribute.ParentType))
			{
				if (!sender.ObjectsEqual(sibling, row))
				{
					if ((bool?) sender.GetValue(sibling, _FieldName) == true)
					{
						sender.SetValue(sibling, _FieldName, false);
						sender.Update(sibling);
						needRefresh = true;
					}
				}
			}

			if (needRefresh)
			{
				foreach (var kvp in sender.Graph.Views)
				{
					PXView view = kvp.Value;
					if (_BqlTable.IsAssignableFrom(view.GetItemType()))
					{
						view.RequestRefresh();
					}
				}
			}
		}

		public void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			findParentAttribute(sender, e.Row);
			updateOtherSiblings(sender, e.Row);
		}

		public void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			findParentAttribute(sender, e.Row);
			updateOtherSiblings(sender, e.Row);
		}
		#endregion
	}

	#endregion
}
