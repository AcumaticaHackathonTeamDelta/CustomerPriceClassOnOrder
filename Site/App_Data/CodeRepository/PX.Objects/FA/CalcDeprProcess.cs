using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	public class CalcDeprProcess : PXGraph<CalcDeprProcess>
	{
		public PXCancel<BalanceFilter> Cancel;
		public PXFilter<BalanceFilter> Filter;
		public PXAction<BalanceFilter> ViewAsset;
		public PXAction<BalanceFilter> ViewBook;
		public PXAction<BalanceFilter> ViewClass;
		[PXFilterable]
		public PXFilteredProcessingJoin<FABookBalance, BalanceFilter, 
				InnerJoin<FixedAsset, On<FixedAsset.assetID, Equal<FABookBalance.assetID>>,
				InnerJoin<FADetails, On<FADetails.assetID, Equal<FABookBalance.assetID>>,
				LeftJoin<Account, On<Account.accountID, Equal<FixedAsset.fAAccountID>>>>>> Balances;
		public PXSetup<Company> company;
		public PXSetup<FASetup> fasetup; 

		public CalcDeprProcess()
		{
			object setup = fasetup.Current;
		}

		[PXDBInt(IsKey = true)]
		[PXSelector(typeof(Search<FixedAsset.assetID>),
			SubstituteKey = typeof(FixedAsset.assetCD), CacheGlobal = true, DescriptionField = typeof(FixedAsset.description))]
		[PXUIField(DisplayName = "Fixed Asset", Enabled = false)]
		public virtual void FABookBalance_AssetID_CacheAttached(PXCache sender)
		{
		}

		protected virtual IEnumerable balances()
		{
			BalanceFilter filter = Filter.Current;

			PXSelectBase<FABookBalance> cmd;
			
			if (PXView.MaximumRows == 1) // SyncPosition call on PXGrid
			{
				cmd = new PXSelectReadonly2<FABookBalance,
					  InnerJoin<FixedAsset, On<FixedAsset.assetID, Equal<FABookBalance.assetID>>,
					  InnerJoin<FADetails, On<FADetails.assetID, Equal<FABookBalance.assetID>>,
					  LeftJoin<Account, On<Account.accountID, Equal<FixedAsset.fAAccountID>>>>>,
					  Where<FABookBalance.depreciate, Equal<True>, And<FABookBalance.status, Equal<FixedAssetStatus.active>, And<FADetails.status, Equal<FixedAssetStatus.active>>>>>(this);
			}
			else
			{
				cmd = new PXSelectJoin<FABookBalance,
					  InnerJoin<FixedAsset, On<FixedAsset.assetID, Equal<FABookBalance.assetID>>,
					  InnerJoin<FADetails, On<FADetails.assetID, Equal<FABookBalance.assetID>>,
					  LeftJoin<Account, On<Account.accountID, Equal<FixedAsset.fAAccountID>>>>>,
					  Where<FABookBalance.depreciate, Equal<True>, And<FABookBalance.status, Equal<FixedAssetStatus.active>, And<FADetails.status, Equal<FixedAssetStatus.active>>>>>(this);
			}
			if (filter.BookID != null)
			{
				cmd.WhereAnd<Where<FABookBalance.bookID, Equal<Current<ProcessAssetFilter.bookID>>>>();
			}
			if (filter.ClassID != null)
			{
				cmd.WhereAnd<Where<FixedAsset.classID, Equal<Current<ProcessAssetFilter.classID>>>>();
			}
			if (filter.BranchID != null)
			{
				cmd.Join<LeftJoin<FALocationHistory, On<FALocationHistory.revisionID, Equal<FADetails.locationRevID>,
												And<FALocationHistory.assetID, Equal<FADetails.assetID>>>>>();
				cmd.WhereAnd<Where<FALocationHistory.locationID, Equal<Current<ProcessAssetFilter.branchID>>>>();
			}
			if (!string.IsNullOrEmpty(filter.PeriodID))
			{
				cmd.WhereAnd<Where<FABookBalance.currDeprPeriod, LessEqual<Current<BalanceFilter.periodID>>>>();
			}
			if(filter.ParentAssetID != null)
			{
				cmd.WhereAnd<Where<FixedAsset.parentAssetID, Equal<Current<ProcessAssetFilter.parentAssetID>>>>();
			}

			int startRow = PXView.StartRow;
			int totalRows = 0;

			List<PXFilterRow> newFilters = new List<PXFilterRow>();
			foreach (PXFilterRow f in PXView.Filters)
			{
				if (f.DataField.ToLower() == "status")
				{
					f.DataField = "FADetails__Status";
				}
				newFilters.Add(f);
			} 
			List<object> list = cmd.View.Select(PXView.Currents, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, newFilters.ToArray(), ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}

		protected virtual void FABookBalance_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;
			if (bal == null || PXLongOperation.Exists(UID)) return;

			try
			{
				AssetProcess.CheckUnreleasedTransactions(this, bal.AssetID);
			}
			catch (PXException exc)
			{
				PXUIFieldAttribute.SetEnabled<FABookBalance.selected>(sender, bal, false);
				sender.RaiseExceptionHandling<FABookBalance.selected>(bal, null, new PXSetPropertyException(exc.MessageNoNumber, PXErrorLevel.RowWarning));
			}
		}

		private static List<FABookBalance> GetProcessableRecords(List<FABookBalance> list)
		{
			PXGraph graph = CreateInstance<PXGraph>();
			return list.Where(balance => !AssetProcess.UnreleasedTransactionsExistsForAsset(graph, balance.AssetID)).ToList();
		}

		private void SetProcessDelegate()
		{
			BalanceFilter filter = Filter.Current;
			bool depriciate = filter.Action == BalanceFilter.action.Depreciate;

			Balances.SetProcessDelegate(delegate(List<FABookBalance> list)
			{
				if (PXLongOperation.GetTaskList().Where(_ => _.Screen == "FA.50.20.00").ToArray().Length > 1)
				{
					throw new PXException(Messages.AnotherDeprRunning);
				}

				List<FABookBalance> balances = GetProcessableRecords(list);

				if (depriciate)
				{
					AssetProcess.DepreciateAsset(balances, null, filter.PeriodID, true);
				}
				else
				{
					AssetProcess.CalculateAsset(balances, filter.PeriodID);
				}
			});

			bool canDepreciate = !string.IsNullOrEmpty(filter.PeriodID) && fasetup.Current.UpdateGL == true;
			PXUIFieldAttribute.SetEnabled<BalanceFilter.action>(Filter.Cache, filter, canDepreciate);
			if (!canDepreciate)
			{
				filter.Action = BalanceFilter.action.Calculate;
			}
		}

		protected virtual void BalanceFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
			SetProcessDelegate();
		}

		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable actionsFolder(PXAdapter adapter)
		{
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable viewAsset(PXAdapter adapter)
		{
			if (Balances.Current != null)
			{
				AssetMaint graph = CreateInstance<AssetMaint>();
				graph.CurrentAsset.Current = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FABookBalance.assetID>>>>.Select(this);
				if (graph.CurrentAsset.Current != null)
				{
					throw new PXRedirectRequiredException(graph, true, "ViewAsset") { Mode = PXBaseRedirectException.WindowMode.Same };
				}
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.ViewBook, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewBook(PXAdapter adapter)
		{
			if (Balances.Current != null)
			{
				BookMaint graph = CreateInstance<BookMaint>();
				graph.Book.Current = PXSelect<FABook, Where<FABook.bookID, Equal<Current<FABookBalance.bookID>>>>.Select(this);
				if (graph.Book.Current != null)
				{
					throw new PXRedirectRequiredException(graph, true, "ViewBook") { Mode = PXBaseRedirectException.WindowMode.Same };
				}
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.ViewClass, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewClass(PXAdapter adapter)
		{
			if (Balances.Current != null)
			{
				AssetClassMaint graph = CreateInstance<AssetClassMaint>();
				graph.CurrentAssetClass.Current = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FABookBalance.classID>>>>.Select(this);
				if (graph.CurrentAssetClass.Current != null)
				{
					throw new PXRedirectRequiredException(graph, true, "ViewClass") { Mode = PXBaseRedirectException.WindowMode.Same };
				}
			}
			return adapter.Get();
		}
	}

	[Serializable]
	public partial class BalanceFilter : ProcessAssetFilter
	{
		#region Action
		public abstract class action : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new[] { Calculate, Depreciate },
				new[] { Messages.Calculate, Messages.Depreciate }) { }
			}

			public const string Calculate = "C";
			public const string Depreciate = "D";

			public class calculate : Constant<string>
			{
				public calculate() : base(Calculate) { }
			}
			public class depreciate : Constant<string>
			{
				public depreciate() : base(Depreciate) { }
			}
			#endregion
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(BalanceFilter.action.Calculate)]
		[BalanceFilter.action.List]
		[PXUIField(DisplayName = "Action", Required = true)]
		public virtual string Action { get; set; }
		#endregion
		#region PeriodID
		public abstract class periodID : IBqlField
		{
		}
		protected String _PeriodID;
		[PXUIField(DisplayName = "To Period", Required = true)]
		[FAOpenPeriod]
		public virtual String PeriodID
		{
			get
			{
				return _PeriodID;
			}
			set
			{
				_PeriodID = value;
			}
		}
		#endregion
	}
}
