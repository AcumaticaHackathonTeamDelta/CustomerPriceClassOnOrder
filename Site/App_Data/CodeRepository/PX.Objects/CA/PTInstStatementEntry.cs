using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;


namespace PX.Objects.CA
{
#if PTInstStatement
	public class PTInstStatementEntry: PXGraph<PTInstStatementEntry, PTInstStatement>
	{
		public ToggleCurrency<PTInstStatement> CurrencyView;
		public PTInstStatementEntry() 
		{
		}

		public PXSelect<PTInstStatement> Statement;
		public PXSelect<PTInstStmtDetail,Where<PTInstStmtDetail.statementID,Equal<Current<PTInstStatement.statementID>>>> StmtDetails;
		public PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<PTInstStatement.cashAccountID>>>> cashAccount;
		public PXSelect<CurrencyInfo> currencyinfo;

		protected virtual void PTInstStatement_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			/*if (acct != null)
			{
				if (acct != null && !string.IsNullOrEmpty(acct.CuryRateTypeID))
				{
					e.NewValue = acct.CuryRateTypeID;
					e.Cancel = true;
				}
			}*/
			//CurrencyInfo curInfo = new CurrencyInfo();
			//    curInfo.CuryEffDate = parameters.ReconDate;
			//    curInfo.CuryID = acct.CuryID;
			//    curInfo.ModuleCode = "GL";
			//    graph.currencyinfo.Insert(curInfo);

		}		
		protected virtual void PTInstStatement_StatementDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			foreach (CurrencyInfo info in PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<PTInstStatement.curyInfoID>>>>.Select(this))
			{
				currencyinfo.Cache.SetValueExt<CurrencyInfo.curyEffDate>(info, ((PTInstStatement)e.Row).StatementDate);
				string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) == false)
				{
					sender.RaiseExceptionHandling<PTInstStatement.statementDate>(e.Row, ((PTInstStatement)e.Row).StatementDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
				}
				else if (currencyinfo.Cache.GetStatus(info) != PXEntryStatus.Updated && currencyinfo.Cache.GetStatus(info) != PXEntryStatus.Inserted)
				{
					currencyinfo.Cache.SetStatus(info, PXEntryStatus.Updated);
				}
			}		
		}
		protected virtual void PTInstStatement_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (this.cashAccount.Current == null || this.cashAccount.Current.AccountID!= ((PTInstStatement)e.Row).CashAccountID)
			{
				this.cashAccount.Current = (CashAccount)PXSelectorAttribute.Select<CashAccount.cashAccountID>(sender, e.Row);
			}

			//if ((bool)CMSetup.Current.MCActivated)
			{
				if (e.ExternalCall || sender.GetValuePending<PTInstStatement.curyID>(e.Row) == null)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<PTInstStatement.curyInfoID>(sender, e.Row);

					string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<PTInstStatement.statementDate>(e.Row, ((PTInstStatement)e.Row).StatementDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						((PTInstStatement)e.Row).CuryID = info.CuryID;
					}
				}
			}

		}


		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CashAccount acct = this.cashAccount.Current;
			if (acct != null)
			{
				if (acct != null && !string.IsNullOrEmpty(acct.CuryRateTypeID))
				{
					e.NewValue = acct.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}
		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.Statement.Cache.Current != null)
			{
				e.NewValue = ((PTInstStatement)this.Statement.Cache.Current).StatementDate;
				e.Cancel = true;
			}
		}



	}
#endif
}
