using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.GL;
using System.Collections;

namespace PX.Objects.PM
{
	[GL.TableDashboardType]
    [Serializable]
	public class ProjectBalanceByPeriodEntry : PXGraph<ProjectBalanceByPeriodEntry>
	{
		#region DAC Attributes Override

        [PXDefault(typeof(ProjectBalanceFilter.projectID))]
        [PXDBInt(IsKey = true)]
        protected virtual void PMProjectStatus_ProjectID_CacheAttached(PXCache sender)
        {
        }

        [PXDefault(typeof(ProjectBalanceFilter.projectTaskID))]
        [PXDBInt(IsKey = true)]
        protected virtual void PMProjectStatus_ProjectTaskID_CacheAttached(PXCache sender)
        {
        }

        [PXDefault(typeof(ProjectBalanceFilter.accountGroupID))]
        [PXDBInt(IsKey = true)]
        protected virtual void PMProjectStatus_AccountGroupID_CacheAttached(PXCache sender)
        {
        }

        [PXDefault(typeof(ProjectBalanceFilter.inventoryID))]
        [PXDBInt(IsKey = true)]
        protected virtual void PMProjectStatus_InventoryID_CacheAttached(PXCache sender)
        {
        }

        [PMUnit(typeof(PMProjectStatus.inventoryID))]
        protected virtual void PMProjectStatus_UOM_CacheAttached(PXCache sender)
        {
        }

        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Budgeted Qty.")]
        protected virtual void PMProjectStatus_Qty_CacheAttached(PXCache sender)
        {
        }

		#endregion

        #region Views/Selects

		public PXFilter<ProjectBalanceFilter> Filter;
        public ProjectStatusSelectBase<PMProjectStatus,
            Where<PMProjectStatus.accountGroupID, Equal<Current<ProjectBalanceFilter.accountGroupID>>,
            And<PMProjectStatus.projectID, Equal<Current<ProjectBalanceFilter.projectID>>,
            And<PMProjectStatus.projectTaskID, Equal<Current<ProjectBalanceFilter.projectTaskID>>,
            And<PMProjectStatus.inventoryID, Equal<Current<ProjectBalanceFilter.inventoryID>>>>>>, OrderBy<Asc<PMProjectStatus.inventoryID>>> Items;

		public PXSelect<PMHistory2Accum> History2; //used by ProjectStatusSelectBase to accumulate Budget/Revised in PMHistory

		#endregion

        
		#region Buttons

		public PXSave<ProjectBalanceFilter> Save;
		public PXCancel<ProjectBalanceFilter> Cancel;
	
		#endregion

		#region Event Handlers

        protected virtual void ProjectBalanceFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ProjectBalanceFilter row = e.Row as ProjectBalanceFilter;
			if (row != null)
            {
                Items.Cache.AllowInsert = row.ProjectID != null && row.AccountGroupID != null && row.ProjectTaskID != null;
	            row.Qty = 0m;
				row.RevisedQty = 0m;
				row.ActualQty = 0m;
				row.Amount = 0m;
				row.RevisedAmount = 0m;
				row.ActualAmount = 0m;
				foreach (PMProjectStatus item in Items.Select())
	            {
					row.Qty += item.Qty;
					row.RevisedQty += item.RevisedQty;
					row.ActualQty += item.ActualQty;
					row.Amount += item.Amount;
					row.RevisedAmount += item.RevisedAmount;
					row.ActualAmount += item.ActualAmount;
				}
            }

            SetStateForColumns();
        }

        protected virtual void SetStateForColumns()
        {
            bool enableUom = Items.Select().Count < 2;

            PXUIFieldAttribute.SetEnabled<PMProjectStatus.uOM>(Items.Cache, null, enableUom);
            PXUIFieldAttribute.SetEnabled<PMProjectStatus.rate>(Items.Cache, null, enableUom);
        }

		protected virtual void PMProjectStatus_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMProjectStatus row = e.Row as PMProjectStatus;
			if (row != null)
			{
				PMProjectStatus ps = PXSelect<PMProjectStatus,
					Where<PMProjectStatus.accountGroupID, Equal<Current<ProjectBalanceFilter.accountGroupID>>,
					And<PMProjectStatus.projectID, Equal<Current<ProjectBalanceFilter.projectID>>,
					And<PMProjectStatus.projectTaskID, Equal<Current<ProjectBalanceFilter.projectTaskID>>,
					And<PMProjectStatus.inventoryID, Equal<Current<ProjectBalanceFilter.inventoryID>>,
					And<PMProjectStatus.periodID, NotEqual<Current<PMProjectStatus.periodID>>>>>>>>.SelectWindowed(this, 0, 1);

				PXUIFieldAttribute.SetEnabled<PMProjectStatus.uOM>(Items.Cache, null, ps == null);
				PXUIFieldAttribute.SetEnabled<PMProjectStatus.rate>(Items.Cache, null, ps == null);
			}
		}

		protected virtual void PMProjectStatus_PeriodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMProjectStatus row = (PMProjectStatus) e.Row;
			if (e.OldValue == null && row.PeriodID != null)
				sender.SetDefaultExt<PMProjectStatus.rate>(row);
		}

	    #endregion

		public override void Persist()
		{
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				base.Persist();
				this.Persist(typeof(PMHistory2Accum), PXDBOperation.Insert);
				ts.Complete();
			}
			this.Caches[typeof(PMHistory2Accum)].Clear();
		}


		#region Local Types

        [Serializable]
		public partial class ProjectBalanceFilter : IBqlTable
		{
			#region AccountGroupID
			public abstract class accountGroupID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountGroupID;
			[PXDefault]
			[AccountGroupAttribute()]
			public virtual Int32? AccountGroupID
			{
				get
				{
					return this._AccountGroupID;
				}
				set
				{
					this._AccountGroupID = value;
				}
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[PXDefault]
			[Project(typeof(Where<PMProject.nonProject, Equal<False>>))]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region ProjectTaskID
			public abstract class projectTaskID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectTaskID;
			[PXDefault]
			[ProjectTask(typeof(ProjectBalanceFilter.projectID))]
			public virtual Int32? ProjectTaskID
			{
				get
				{
					return this._ProjectTaskID;
				}
				set
				{
					this._ProjectTaskID = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[PXDefault]
			[PXDBInt]
			[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Visible)]
			[PMInventorySelector(typeof(Search2<InventoryItem.inventoryID,
              InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Current<ProjectBalanceFilter.accountGroupID>>>,
              LeftJoin<PMInventorySelectorAttribute.Cogs, On<PMInventorySelectorAttribute.Cogs.accountID, Equal<InventoryItem.cOGSAcctID>>,
							LeftJoin<PMInventorySelectorAttribute.Exp, On<PMInventorySelectorAttribute.Exp.accountID, Equal<InventoryItem.cOGSAcctID>>,
              LeftJoin<PMInventorySelectorAttribute.Sale, On<PMInventorySelectorAttribute.Sale.accountID, Equal<InventoryItem.salesAcctID>>>>>>,
			  Where<PMAccountGroup.type, Equal<AccountType.expense>,
			And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Cogs.accountGroupID>,
			And<InventoryItem.stkItem, Equal<True>,
			Or<PMAccountGroup.type, Equal<AccountType.expense>,
				And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Exp.accountGroupID>,
			And<InventoryItem.stkItem, Equal<False>,
			Or<PMAccountGroup.type, Equal<AccountType.income>,
			And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Sale.accountGroupID>,
			Or<PMAccountGroup.type, Equal<AccountType.liability>,
			Or<PMAccountGroup.type, Equal<AccountType.asset>,
			Or<PMAccountGroup.type, Equal<PMAccountType.offBalance>>>>>>>>>>>>>), SubstituteKey = typeof(InventoryItem.inventoryCD), Filterable = true)]
            public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion

			#region Qty
			public abstract class qty : PX.Data.IBqlField
			{
			}
			protected Decimal? _Qty;
			[PXDecimal]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Budgeted")]
			public virtual Decimal? Qty
			{
				get
				{
					return this._Qty;
				}
				set
				{
					this._Qty = value;
				}
			}
			#endregion
			#region Amount
			public abstract class amount : PX.Data.IBqlField
			{
			}
			protected Decimal? _Amount;
			[PXBaseCury]
//			[PXFormula(typeof(Mult<PMProjectStatusEx.qty, PMProjectStatusEx.rate>))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Budgeted Amount")]
			public virtual Decimal? Amount
			{
				get
				{
					return this._Amount;
				}
				set
				{
					this._Amount = value;
				}
			}
			#endregion

			#region RevisedQty
			public abstract class revisedQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _RevisedQty;
			[PXQuantity]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Revised")]
			public virtual Decimal? RevisedQty
			{
				get
				{
					return this._RevisedQty;
				}
				set
				{
					this._RevisedQty = value;
				}
			}
			#endregion
			#region RevisedAmount
			public abstract class revisedAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _RevisedAmount;
			[PXBaseCury]
//			[PXFormula(typeof(Mult<PMProjectStatusEx.revisedQty, PMProjectStatusEx.rate>))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Revised Amount")]
			public virtual Decimal? RevisedAmount
			{
				get
				{
					return this._RevisedAmount;
				}
				set
				{
					this._RevisedAmount = value;
				}
			}
			#endregion

			#region ActualQty
			public abstract class actualQty : PX.Data.IBqlField
			{
			}
			protected Decimal? _ActualQty;
			[PXQuantity]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Actual", Enabled = false)]
			public virtual Decimal? ActualQty
			{
				get
				{
					return this._ActualQty;
				}
				set
				{
					this._ActualQty = value;
				}
			}
			#endregion
			#region ActualAmount
			public abstract class actualAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ActualAmount;
			[PXBaseCury]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Actual Amount", Enabled = false)]
			public virtual Decimal? ActualAmount
			{
				get
				{
					return this._ActualAmount;
				}
				set
				{
					this._ActualAmount = value;
				}
			}
			#endregion



		}

		#endregion
	}
}
