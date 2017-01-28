using System;
using System.Collections;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.CR;

namespace PX.Objects.DR
{
	[TableAndChartDashboardType]
	public class ScheduleTransInq : PXGraph<ScheduleTransInq>
	{
		public PXCancel<ScheduleTransFilter> Cancel;
		public PXFilter<ScheduleTransFilter> Filter;
		public PXSelectJoin<DRScheduleTran,
					InnerJoin<DRScheduleDetail, On<DRScheduleTran.scheduleID, Equal<DRScheduleDetail.scheduleID>, And<DRScheduleTran.componentID, Equal<DRScheduleDetail.componentID>>>,
					InnerJoin<DRDeferredCode, On<DRDeferredCode.deferredCodeID, Equal<DRScheduleDetail.defCode>>,
					LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<DRScheduleDetail.componentID>>>>>,
					Where<DRDeferredCode.accountType, Equal<Current<ScheduleTransFilter.accountType>>,
					And<DRScheduleTran.status, Equal<DRScheduleTranStatus.PostedStatus>,
					And<DRScheduleTran.finPeriodID, Equal<Current<ScheduleTransFilter.finPeriodID>>>>>> Records;

		public virtual IEnumerable records()
		{
			ScheduleTransFilter filter = this.Filter.Current;
			if (filter != null)
			{
				PXSelectBase<DRScheduleTran> select = new PXSelectJoin<DRScheduleTran,
					InnerJoin<DRScheduleDetail, On<DRScheduleTran.scheduleID, Equal<DRScheduleDetail.scheduleID>, And<DRScheduleTran.componentID, Equal<DRScheduleDetail.componentID>>>,
					InnerJoin<DRDeferredCode, On<DRDeferredCode.deferredCodeID, Equal<DRScheduleDetail.defCode>>,
					LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<DRScheduleDetail.componentID>>>>>,
					Where<DRDeferredCode.accountType, Equal<Current<ScheduleTransFilter.accountType>>,
					And<DRScheduleTran.status, Equal<DRScheduleTranStatus.PostedStatus>,
					And<DRScheduleTran.finPeriodID, Equal<Current<ScheduleTransFilter.finPeriodID>>>>>>(this);

				if (!string.IsNullOrEmpty(filter.DeferredCode))
				{
					select.WhereAnd<Where<DRScheduleDetail.defCode, Equal<Current<ScheduleTransFilter.deferredCode>>>>();
				}
								
				if (filter.AccountID != null)
				{
					select.WhereAnd<Where<DRScheduleTran.accountID, Equal<Current<ScheduleTransFilter.accountID>>>>();
				}

				if (filter.SubID != null)
				{
					select.WhereAnd<Where<DRScheduleTran.subID, Equal<Current<ScheduleTransFilter.subID>>>>();
				}

				if (filter.BAccountID != null)
				{
					select.WhereAnd<Where<DRScheduleDetail.bAccountID, Equal<Current<ScheduleTransFilter.bAccountID>>>>();
				}
								
				foreach (object x in select.Select())
				{
					yield return x;
				}
			}
			else yield break;
		}

		public PXAction<ScheduleTransFilter> viewSchedule;
		public PXAction<ScheduleTransFilter> previousPeriod;
		public PXAction<ScheduleTransFilter> nextPeriod;
		
		[PXUIField(DisplayName = "")]
		[PXEditDetailButton]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			if (Records.Current != null)
			{
				DraftScheduleMaint target = PXGraph.CreateInstance<DraftScheduleMaint>();
				target.Schedule.Current = PXSelect<DRSchedule,
					Where<DRSchedule.scheduleID, Equal<Current<DRScheduleTran.scheduleID>>>>.Select(this);

				throw new PXRedirectRequiredException(target, true, "ViewSchedule") { Mode = PXBaseRedirectException.WindowMode.NewWindow };

			}
			return adapter.Get();
		}

		public PXAction<ScheduleTransFilter> viewDoc;
		[PXUIField(DisplayName = "")]
		[PXButton]
		public virtual IEnumerable ViewDoc(PXAdapter adapter)
		{
			if (Records.Current != null)
			{
				NavigationHelper.NavigateToOriginalDocument(this, Records.Current);
			}

			return adapter.Get();
		}

		public PXAction<ScheduleTransFilter> viewBatch;
		[PXUIField(DisplayName = "")]
		[PXButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			JournalEntry target = PXGraph.CreateInstance<JournalEntry>();
			target.Clear();
			Batch batch = PXSelect<Batch, Where<Batch.module, Equal<BatchModule.moduleDR>, And<Batch.batchNbr, Equal<Current<DRScheduleTran.batchNbr>>>>>.Select(this);
			if (batch != null)
			{
				target.BatchModule.Current = batch;
				throw new PXRedirectRequiredException(target, true, "ViewBatch") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}

			return adapter.Get();
		}
		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable PreviousPeriod(PXAdapter adapter)
		{
			ScheduleTransFilter filter = Filter.Current as ScheduleTransFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindPrevPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable NextPeriod(PXAdapter adapter)
		{
			ScheduleTransFilter filter = Filter.Current as ScheduleTransFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindNextPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		protected virtual void ScheduleTransFilter_AccountType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ScheduleTransFilter row = e.Row as ScheduleTransFilter;
			if (row != null)
			{
				row.BAccountID = null;
				row.DeferredCode = null;
				row.AccountID = null;
				row.SubID = null;
			}
		}


		#region Local Types
		[Serializable]
		public partial class ScheduleTransFilter : IBqlTable
		{
			#region AccountType
			public abstract class accountType : PX.Data.IBqlField
			{
			}
			protected string _AccountType;
			[PXDBString(1)]
			[PXDefault(DeferredAccountType.Income)]
			[LabelList(typeof(DeferredAccountType))]
			[PXUIField(DisplayName = "Code Type", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string AccountType
			{
				get
				{
					return this._AccountType;
				}
				set
				{
					this._AccountType = value;
					switch (value)
					{
						case DeferredAccountType.Expense:
							_BAccountType = CR.BAccountType.VendorType;
							break;
						default:
							_BAccountType = CR.BAccountType.CustomerType;
							break;
					}
				}
			}
			#endregion
			#region FinPeriodID
			public abstract class finPeriodID : PX.Data.IBqlField
			{
			}
			protected String _FinPeriodID;
			[PXDefault]
            [FinPeriodSelector]
			[PXUIField(DisplayName = "Fin. Period")]
			public virtual String FinPeriodID
			{
				get
				{
					return this._FinPeriodID;
				}
				set
				{
					this._FinPeriodID = value;
				}
			}
			#endregion
			#region DeferredCode
			public abstract class deferredCode : PX.Data.IBqlField
			{
			}
			protected String _DeferredCode;
			[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
			[PXUIField(DisplayName = "Deferral Code")]
			[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID, Where<DRDeferredCode.accountType, Equal<Current<ScheduleTransFilter.accountType>>>>))]
			public virtual String DeferredCode
			{
				get
				{
					return this._DeferredCode;
				}
				set
				{
					this._DeferredCode = value;
				}
			}
			#endregion

			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
			public virtual Int32? AccountID
			{
				get
				{
					return this._AccountID;
				}
				set
				{
					this._AccountID = value;
				}
			}
			#endregion
			#region SubID
			public abstract class subID : PX.Data.IBqlField
			{
			}
			protected Int32? _SubID;
			[SubAccount(typeof(ScheduleTransFilter.accountID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
			public virtual Int32? SubID
			{
				get
				{
					return this._SubID;
				}
				set
				{
					this._SubID = value;
				}
			}
			#endregion
			#region BAccountType
			public abstract class bAccountType : PX.Data.IBqlField
			{
			}
			protected String _BAccountType;
			[PXDefault(CR.BAccountType.CustomerType)]
			[PXString(2, IsFixed = true)]
			[PXStringList(new string[] { CR.BAccountType.VendorType, CR.BAccountType.CustomerType },
					new string[] { CR.Messages.VendorType, CR.Messages.CustomerType })]
			public virtual String BAccountType
			{
				get
				{
					return this._BAccountType;
				}
				set
				{
					this._BAccountType = value;
				}
			}
			#endregion
			#region BAccountID
			public abstract class bAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _BAccountID;
			[PXDBInt()]
			[PXUIField(DisplayName = "Customer/Vendor")]
			[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, Equal<Current<ScheduleTransFilter.bAccountType>>>>), new Type[] { typeof(BAccountR.acctCD), typeof(BAccountR.acctName), typeof(BAccountR.type) }, SubstituteKey = typeof(BAccountR.acctCD))]
			public virtual Int32? BAccountID
			{
				get
				{
					return this._BAccountID;
				}
				set
				{
					this._BAccountID = value;
				}
			}
			#endregion
		}

		#endregion
	}
}
