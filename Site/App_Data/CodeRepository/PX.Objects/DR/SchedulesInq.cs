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
    [Serializable]
	public class SchedulesInq : PXGraph<SchedulesInq>
	{
		#region Selects and Actions
		public PXCancel<SchedulesFilter> Cancel;
		public PXFilter<SchedulesFilter> Filter;

        public PXSelectReadonly<SchedulesInqResult> Records;
		public virtual IEnumerable records()
		{
			SchedulesFilter filter = this.Filter.Current;
			if (filter != null)
			{
				PXSelectBase<SchedulesInqResult> select = new PXSelectJoin<SchedulesInqResult,
					InnerJoin<DRSchedule, On<DRSchedule.scheduleID, Equal<SchedulesInqResult.scheduleID>>,
					InnerJoin<DRDeferredCode, On<DRDeferredCode.deferredCodeID, Equal<SchedulesInqResult.defCode>>,
                    LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SchedulesInqResult.componentID>>>>>,
					Where<DRDeferredCode.accountType, Equal<Current<SchedulesFilter.accountType>>>>(this);
				
				if (!string.IsNullOrEmpty(filter.DeferredCode))
				{
                    select.WhereAnd<Where<SchedulesInqResult.defCode, Equal<Current<SchedulesFilter.deferredCode>>>>();
				}

				if (filter.AccountID != null)
				{
                    select.WhereAnd<Where<SchedulesInqResult.defAcctID, Equal<Current<SchedulesFilter.accountID>>>>();
				}

				if (filter.SubID != null)
				{
                    select.WhereAnd<Where<SchedulesInqResult.defSubID, Equal<Current<SchedulesFilter.subID>>>>();
				}

				if (filter.BAccountID != null)
				{
                    select.WhereAnd<Where<SchedulesInqResult.bAccountID, Equal<Current<SchedulesFilter.bAccountID>>>>();
				}

				if (filter.ComponentID != null)
				{
                    select.WhereAnd<Where<SchedulesInqResult.componentID, Equal<Current<SchedulesFilter.componentID>>>>();
				}

				foreach (PXResult<SchedulesInqResult, DRSchedule, DRDeferredCode, InventoryItem> record in select.Select())
				{
                    SchedulesInqResult sd = (SchedulesInqResult) record;
                    InventoryItem item = (InventoryItem) record;

                    sd.ComponentCD = item.InventoryCD;
					sd.DocumentType = DRScheduleDocumentType.BuildDocumentType(sd.Module, sd.DocType);
                    
					yield return sd;
				}

			}
			else
				yield break;
		}

		public PXAction<SchedulesFilter> viewSchedule;
		[PXUIField(DisplayName = "")]
		[PXEditDetailButton]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			if (Records.Current != null)
			{
				DraftScheduleMaint target = PXGraph.CreateInstance<DraftScheduleMaint>();
				target.Schedule.Current = PXSelect<DRSchedule,
					Where<DRSchedule.scheduleID, Equal<Current<SchedulesInqResult.scheduleID>>>>.Select(this);
				throw new PXRedirectRequiredException(target, true, "ViewSchedule") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public PXAction<SchedulesFilter> viewDocument;
		[PXUIField(DisplayName = "")]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (Records.Current != null)
			{
				NavigationHelper.NavigateToOriginalDocument(this, Records.Current);
			}
			return adapter.Get();
		}
		#endregion

		protected virtual void SchedulesFilter_AccountType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SchedulesFilter row = e.Row as SchedulesFilter;
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
		public partial class SchedulesFilter : IBqlTable
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
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
            [Account(DisplayName = "Deferral Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
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
            [SubAccount(typeof(SchedulesFilter.accountID), DisplayName = "Deferral Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
			#region DeferredCode
			public abstract class deferredCode : PX.Data.IBqlField
			{
			}
			protected String _DeferredCode;
			[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
			[PXUIField(DisplayName = "Deferral Code")]
			[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID, Where<DRDeferredCode.accountType, Equal<Current<SchedulesFilter.accountType>>>>))]
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
			[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, Equal<Current<SchedulesFilter.bAccountType>>>>), new Type[] { typeof(BAccountR.acctCD), typeof(BAccountR.acctName), typeof(BAccountR.type) }, SubstituteKey = typeof(BAccountR.acctCD))]
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
			#region ComponentID
			public abstract class componentID : PX.Data.IBqlField
			{
			}
			protected Int32? _ComponentID;
            			
			[Inventory(DisplayName="Component")]
			public virtual Int32? ComponentID
			{
				get
				{
					return this._ComponentID;
				}
				set
				{
					this._ComponentID = value;
				}
			}

			#endregion
		}

        [Serializable]
        public partial class SchedulesInqResult : DRScheduleDetail
        {
            #region ComponentID
            public new abstract class componentID : PX.Data.IBqlField
            {
            }
            [PXDBInt(IsKey = true)]
            [PXUIField(DisplayName = Messages.ComponentID, Visibility = PXUIVisibility.Visible)]
            public override Int32? ComponentID
            {
                get
                {
                    return this._ComponentID;
                }
                set
                {
                    this._ComponentID = value;
                }
            }

            #endregion
            #region ComponentCD
            public abstract class componentCD : PX.Data.IBqlField
            {
            }
            protected string _ComponentCD;

            [PXString]
            [PXUIField(DisplayName = Messages.ComponentID, Visibility = PXUIVisibility.Visible)]
            public virtual string ComponentCD
            {
                get
                {
                    return this._ComponentCD;
                }
                set
                {
                    this._ComponentCD = value;
                }
            }

            #endregion

		}

		#endregion
	}
}
