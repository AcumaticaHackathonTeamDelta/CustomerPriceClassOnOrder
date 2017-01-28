using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.CA
{
	public class CashForecastEntry : PXGraph<CashForecastEntry>
	{
		#region Internal type definitions
		[Serializable]
		public partial class Filter : PX.Data.IBqlTable
		{
			#region CashAccountID
			public abstract class cashAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _CashAccountID;
			[GL.CashAccount(DisplayName = "Cash Account", DescriptionField = typeof(CashAccount.descr))]
			[PXDefault()]
			public virtual Int32? CashAccountID
			{
				get
				{
					return this._CashAccountID;
				}
				set
				{
					this._CashAccountID = value;
				}
			}
			#endregion
			#region StartDate
			public abstract class startDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _StartDate;
			[PXDBDate()]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = true)]
			public virtual DateTime? StartDate
			{
				get
				{
					return this._StartDate;
				}
				set
				{
					this._StartDate = value;
				}
			}
			#endregion
		}		

		[GL.CashAccount(DisplayName = "Cash Account", 
			Visibility = PXUIVisibility.SelectorVisible, 
			DescriptionField = typeof(CashAccount.descr), Visible= false)]
		[PXDefault(typeof(Filter.cashAccountID))]
		protected virtual void CashForecastTran_CashAccountID_CacheAttached(PXCache sender)
		{
		}
		
		#endregion
		#region Buttons

		public PXSave<Filter> Save;
		public PXCancel<Filter> Cancel;
		#endregion
		#region Ctor + Selects
		public PXFilter<Filter> filter;
		[PXImport(typeof(Filter))]
		public PXSelect<CashForecastTran,
							Where<CashForecastTran.tranDate, GreaterEqual<Current<Filter.startDate>>,
									And<CashForecastTran.cashAccountID, Equal<Current<Filter.cashAccountID>>>>,
									OrderBy<Asc<CashForecastTran.tranDate>>> cashForecastTrans;
		public PXSetup<CASetup> casetup;

		public CashForecastEntry()
		{
			CASetup setup = casetup.Current;
		}
		
		#endregion
		#region Event Handlers
		protected virtual void Filter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			Filter row = (Filter)e.Row;
			bool enableEdit = row != null && row.CashAccountID.HasValue;
			this.cashForecastTrans.Cache.AllowInsert = enableEdit;
			this.cashForecastTrans.Cache.AllowUpdate = enableEdit;
		}

		protected virtual void CashForecastTran_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CashForecastTran row = (CashForecastTran)e.Row;
			sender.SetDefaultExt<CashForecastTran.curyID>(e.Row);
		}

		protected virtual void CashForecastTran_TranDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			Filter filter = this.filter.Current;
			if (filter != null && filter.StartDate.HasValue)
			{
				e.NewValue = filter.StartDate;
				e.Cancel = true;
			}
		}

		protected virtual void CashForecastTran_CuryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			CashForecastTran row = (CashForecastTran)e.Row;
			ValidateCuryID(sender, row, (string)e.NewValue);
		}

		public override void Persist()
		{
			foreach (CashForecastTran row in cashForecastTrans.Cache.Inserted)
			{
				ValidateCuryID(cashForecastTrans.Cache, row, row.CuryID);
			}
			foreach (CashForecastTran row in cashForecastTrans.Cache.Updated)
			{
				ValidateCuryID(cashForecastTrans.Cache, row,row.CuryID);
			}
			base.Persist();
		}
		#endregion
		#region PrivateMethods
		private void ValidateCuryID(PXCache sender, CashForecastTran row, string CuryID)
		{
            CashAccount acct = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, filter.Current.CashAccountID);
            if (acct.CuryID != CuryID)
            {
                cashForecastTrans.Cache.RaiseExceptionHandling<CashForecastTran.curyID>(row, CuryID, 
                    new PXRowPersistingException(typeof(CashForecastTran.curyID).Name, CuryID, Messages.TranCuryNotMatchAcctCury));
            }
		}
		#endregion
	}
}
