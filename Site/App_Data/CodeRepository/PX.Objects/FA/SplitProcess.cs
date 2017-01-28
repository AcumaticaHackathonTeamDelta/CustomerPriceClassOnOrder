using System;
using System.Collections;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.FA
{
	public class SplitProcess : PXGraph<SplitProcess>
	{
		public PXCancel<SplitFilter> Cancel;
		public PXFilter<SplitFilter> Filter;
		public PXFilteredProcessing<SplitParams, SplitFilter> Splits;
		public PXSelectJoin<Numbering, InnerJoin<FASetup, On<FASetup.assetNumberingID, Equal<Numbering.numberingID>>>> assetNumbering;
		public PXSetup<FASetup> fasetup; 

		public SplitProcess()
		{
			object setup = fasetup.Current;

			if (fasetup.Current.UpdateGL != true)
			{
				throw new PXSetupNotEnteredException<FASetup>(Messages.OperationNotWorkInInitMode, PXUIFieldAttribute.GetDisplayName<FASetup.updateGL>(fasetup.Cache));
			}
		}

		public IEnumerable splits(PXAdapter adapter)
		{
			return Splits.Cache.Inserted;
		}

		public void SplitFilter_SplitDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			SplitFilter filter = (SplitFilter) e.Row;
			if (filter == null) return;

			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<SplitFilter.assetID>>>, OrderBy<Desc<FABookBalance.updateGL>>>.SelectSingleBound(this, new object[] { filter });
			if(bal != null)
			{
				FABookPeriod todayPeriod = FABookPeriodIDAttribute.FABookPeriodFromDate(this, Accessinfo.BusinessDate, bal.BookID);
				if(string.IsNullOrEmpty(bal.CurrDeprPeriod) && !string.IsNullOrEmpty(bal.LastDeprPeriod))
				{
					e.NewValue = FABookPeriodIDAttribute.PeriodPlusPeriod(this, bal.LastDeprPeriod, 1, bal.BookID);
				}
				else
				{
					e.NewValue = string.Compare(bal.CurrDeprPeriod, todayPeriod.FinPeriodID) > 0 ? FABookPeriodIDAttribute.PeriodStartDate(this, bal.CurrDeprPeriod, bal.BookID) : Accessinfo.BusinessDate;
				}
			}
			else
			{
				e.NewValue = Accessinfo.BusinessDate;
			}
		}

		public void SplitFilter_AssetID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<SplitFilter.cost>(e.Row);
			sender.SetDefaultExt<SplitFilter.qty>(e.Row);
			sender.SetDefaultExt<SplitFilter.splitDate>(e.Row);
			sender.SetDefaultExt<SplitFilter.splitPeriodID>(e.Row);

			Splits.Cache.IsDirty = false;
			Splits.Cache.Clear();
		}

		public void SplitFilter_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			Splits.Cache.IsDirty = false;
			Splits.Cache.Clear();
		}

		public void SplitFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SplitFilter filter = (SplitFilter) e.Row;
			if (filter == null) return;

			PXProcessingStep[] targets = PXAutomation.GetProcessingSteps(this);
			if (targets.Length > 0)
			{
				Splits.SetProcessTarget(targets[0].GraphName,
					targets.Length > 1 ? null : targets[0].Name,
					targets[0].Actions[0].Name,
					targets[0].Actions[0].Menus[0],
					filter.SplitDate,
					filter.SplitPeriodID,
					null, null, null, null, null, 
					filter.DeprBeforeSplit,
					null,
					filter.AssetID);
			}
			else
			{
				throw new PXException(SO.Messages.MissingMassProcessWorkFlow);
			}

			Splits.SetProcessVisible(false);
			Splits.SetProcessAllEnabled(filter.AssetID != null && filter.SplitPeriodID != null);
			Splits.SetProcessAllCaption(Messages.Split);

			Splits.Cache.AllowInsert = filter.AssetID != null;
			Splits.Cache.AllowUpdate = filter.AssetID != null;
			Splits.Cache.AllowDelete = true;
			
			PXUIFieldAttribute.SetEnabled<SplitParams.cost>(Splits.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<SplitParams.splittedQty>(Splits.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<SplitParams.ratio>(Splits.Cache, null, true);
			Numbering nbr = assetNumbering.Select();
			PXUIFieldAttribute.SetEnabled<SplitParams.splittedAssetCD>(Splits.Cache, null, nbr == null || nbr.UserNumbering == true);

			PXUIFieldAttribute.SetEnabled<SplitFilter.deprBeforeSplit>(sender, filter, fasetup.Current.AutoReleaseDepr == true);

			FixedAsset asset = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<SplitFilter.assetID>>>>.Select(this, filter.AssetID);
			PXUIFieldAttribute.SetVisible<SplitFilter.deprBeforeSplit>(sender, filter, asset == null || asset.Depreciable == true);
		}

		public void SplitFilter_AssetID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			SplitFilter filter = (SplitFilter) e.Row;
			FATran tran = PXSelect<FATran, Where<FATran.assetID, Equal<Required<SplitFilter.assetID>>, And<FATran.released, NotEqual<True>>>>.Select(this, e.NewValue);
			if (tran != null)
			{
				if (filter.AssetID != null)
				{
					FixedAsset asset = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<SplitFilter.assetID>>>>.Select(this, filter.AssetID);
					e.NewValue = asset.AssetCD;
				}
				else
				{
					e.NewValue = null;
				}
				throw new PXSetPropertyException(Messages.AssetHasUnreleasedTran);
			}
		}

		public void SplitFilter_CurrPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (string.IsNullOrEmpty((string)e.NewValue))
			{
				throw new PXSetPropertyException(Messages.CurrentDeprPeriodIsNull);
			}
		}

		protected void SplitParams_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			FixedAsset sourceAsset = (FixedAsset) PXSelectorAttribute.Select<SplitFilter.assetID>(Filter.Cache, Filter.Current);
			if (sourceAsset != null)
			{
				SplitParams pars = (SplitParams) e.Row;
				PXCache<FixedAsset>.RestoreCopy(pars, sourceAsset);
			}
		}

		public void SplitParams_Cost_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (Filter.Current != null && e.NewValue as decimal? != null)
			{
				if ((decimal)e.NewValue > Filter.Current.Cost)
					throw new PXSetPropertyException(CS.Messages.Entry_LE, Filter.Current.Cost);
				if ((decimal)e.NewValue < 0m)
					throw new PXSetPropertyException(CS.Messages.Entry_GE, 0m);
			}
		}

		public void SplitParams_Cost_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SplitParams parms = (SplitParams) e.Row;
			SplitFilter filter = Filter.Current;
			if (parms == null || filter == null || (filter.Cost ?? 0m) == 0m) return;

			object rounded;
			if (parms.SplittedQty == null || parms.SplittedQty == 0m)
			{
				rounded = parms.Cost*filter.Qty/filter.Cost;
				sender.RaiseFieldUpdating<SplitParams.splittedQty>(parms, ref rounded);
				parms.SplittedQty = (decimal?) rounded;
			}
			rounded = parms.Cost * 100 / filter.Cost;
			sender.RaiseFieldUpdating<SplitParams.ratio>(parms, ref rounded);
			parms.Ratio = (decimal?)rounded;
		}

		public void SplitParams_SplittedQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SplitParams parms = (SplitParams)e.Row;
			SplitFilter filter = Filter.Current;
			if (parms == null || filter == null) return;

			object rounded = parms.SplittedQty * filter.Cost / filter.Qty;
			sender.RaiseFieldUpdating<SplitParams.cost>(parms, ref rounded);
			parms.Cost = (decimal?)rounded;
			rounded = parms.SplittedQty * 100 / filter.Qty;
			sender.RaiseFieldUpdating<SplitParams.ratio>(parms, ref rounded);
			parms.Ratio = (decimal?)rounded;
		}

		public void SplitParams_Ratio_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SplitParams parms = (SplitParams)e.Row;
			SplitFilter filter = Filter.Current;
			if (parms == null || filter == null) return;

			object rounded;
			if (parms.SplittedQty == null || parms.SplittedQty == 0m)
			{
				rounded = parms.Ratio * filter.Qty / 100;
				sender.RaiseFieldUpdating<SplitParams.splittedQty>(parms, ref rounded);
				parms.SplittedQty = (decimal?)rounded;
			}
			rounded = parms.Ratio * filter.Cost / 100;
			sender.RaiseFieldUpdating<SplitParams.cost>(parms, ref rounded);
			parms.Cost = (decimal?)rounded;
		}

		public PXAction<SplitFilter> viewAsset;
		[PXUIField(DisplayName = Messages.ViewAsset, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewAsset(PXAdapter adapter)
		{
			if (Splits.Current != null)
			{
				AssetMaint graph = CreateInstance<AssetMaint>();
				graph.CurrentAsset.Current = PXSelect<FixedAsset, Where<FixedAsset.assetCD, Equal<Current<SplitParams.splittedAssetCD>>>>.Select(this);
				if (graph.CurrentAsset.Current != null)
				{
					throw new PXRedirectRequiredException(graph, true, "ViewAsset") { Mode = PXBaseRedirectException.WindowMode.Same };
				}
			}
			return adapter.Get();
		}


		#region Inner DACs
		[Serializable]
		public partial class SplitFilter : IBqlTable
		{
			#region AssetID
			public abstract class assetID : PX.Data.IBqlField
			{
			}
			protected int? _AssetID;
			[PXDBInt]
			[PXSelector(typeof(Search2<FixedAsset.assetID, 
				InnerJoin<FADetails, On<FADetails.assetID, Equal<FixedAsset.assetID>>>, 
				Where<FixedAsset.recordType, Equal<FARecordType.assetType>, 
					And<Where<FADetails.status, Equal<FixedAssetStatus.active>,
						Or<FADetails.status, Equal<FixedAssetStatus.fullyDepreciated>>>>>>),
				SubstituteKey = typeof(FixedAsset.assetCD),
				DescriptionField = typeof(FixedAsset.description))]
			[PXUIField(DisplayName = "Fixed Asset")]
			public virtual int? AssetID
			{
				get
				{
					return _AssetID;
				}
				set
				{
					_AssetID = value;
				}
			}
			#endregion
			#region Cost
			public abstract class cost : IBqlField
			{
			}
			protected decimal? _Cost;
			[PXDBBaseCury]
			[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<FABookBalance.ytdDeprBase, Where<FABookBalance.assetID, Equal<Current<SplitFilter.assetID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Cost", Enabled = false)]
			public virtual decimal? Cost
			{
				get
				{
					return _Cost;
				}
				set
				{
					_Cost = value;
				}
			}
			#endregion
			#region Qty
			public abstract class qty : IBqlField
			{
			}
			protected Decimal? _Qty;
			[PXDBQuantity]
			[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<FixedAsset.qty, Where<FixedAsset.assetID, Equal<Current<SplitFilter.assetID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Quantity", Enabled = false)]
			public virtual Decimal? Qty
			{
				get
				{
					return _Qty;
				}
				set
				{
					_Qty = value;
				}
			}
			#endregion
			#region SplitDate
			public abstract class splitDate : IBqlField
			{
			}
			protected DateTime? _SplitDate;
			[PXDBDate]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Split Date")]
			public virtual DateTime? SplitDate
			{
				get
				{
					return _SplitDate;
				}
				set
				{
					_SplitDate = value;
				}
			}
			#endregion
			#region SplitPeriodID
			public abstract class splitPeriodID : IBqlField
			{
			}
			protected string _SplitPeriodID;
			[PXUIField(DisplayName = "Split Period")]
			[FAOpenPeriod(typeof(SplitFilter.splitDate))]
			public virtual string SplitPeriodID
			{
				get
				{
					return _SplitPeriodID;
				}
				set
				{
					_SplitPeriodID = value;
				}
			}
			#endregion
			#region DeprBeforeSplit
			public abstract class deprBeforeSplit : IBqlField
			{
			}
			protected bool? _DeprBeforeSplit;
			[PXDBBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Depreciate before split")]
			public virtual bool? DeprBeforeSplit
			{
				get
				{
					return _DeprBeforeSplit;
				}
				set
				{
					_DeprBeforeSplit = value;
				}
			}
			#endregion
		}
		#endregion
	}
}
