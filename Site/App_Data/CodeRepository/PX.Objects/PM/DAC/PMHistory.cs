using PX.Objects.GL;

namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.CM;

	[System.SerializableAttribute()]
    [PXCacheName(Messages.PMHistory)]
    public partial class PMHistory : PX.Data.IBqlTable
	{
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
        [PXUIField(DisplayName = "Project ID")]
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
		[PXDBInt(IsKey = true)]
		[PXDefault()]
        [PXUIField(DisplayName = "Project Task ID")]
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
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
        [PXUIField(DisplayName = "Account Group ID")]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
        [PXUIField(DisplayName = "Inventory ID")]
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
		#region PeriodID
		public abstract class periodID : PX.Data.IBqlField
		{
		}
		protected String _PeriodID;
		[GL.FinPeriodID(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(FinPeriod.finPeriodID), DescriptionField = typeof(FinPeriod.descr))]
		public virtual String PeriodID
		{
			get
			{
				return this._PeriodID;
			}
			set
			{
				this._PeriodID = value;
			}
		}
		#endregion
		
		#region BudgetQty
		public abstract class budgetQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BudgetQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BudgetQty
		{
			get
			{
				return this._BudgetQty;
			}
			set
			{
				this._BudgetQty = value;
			}
		}
		#endregion
		#region BudgetAmount
		public abstract class budgetAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _BudgetAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BudgetAmount
		{
			get
			{
				return this._BudgetAmount;
			}
			set
			{
				this._BudgetAmount = value;
			}
		}
		#endregion
		#region RevisedQty
		public abstract class revisedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _RevisedQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		[PXDBBaseCury]
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
		#region PTDBudgetQty
		public abstract class pTDBudgetQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDBudgetQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName= Messages.BudgetPTDQuantity)]
		public virtual Decimal? PTDBudgetQty
		{
			get
			{
				return this._PTDBudgetQty;
			}
			set
			{
				this._PTDBudgetQty = value;
			}
		}
		#endregion
		#region PTDBudgetAmount
		public abstract class pTDBudgetAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDBudgetAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = Messages.BudgetPTDAmount)]

        public virtual Decimal? PTDBudgetAmount
		{
			get
			{
				return this._PTDBudgetAmount;
			}
			set
			{
				this._PTDBudgetAmount = value;
			}
		}
		#endregion
		#region PTDRevisedQty
		public abstract class pTDRevisedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDRevisedQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = Messages.RevisedPTDQuantity)]
        public virtual Decimal? PTDRevisedQty
		{
			get
			{
				return this._PTDRevisedQty;
			}
			set
			{
				this._PTDRevisedQty = value;
			}
		}
		#endregion
		#region PTDRevisedAmount
		public abstract class pTDRevisedAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDRevisedAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = Messages.RevisedPTDAmount)]
        public virtual Decimal? PTDRevisedAmount
		{
			get
			{
				return this._PTDRevisedAmount;
			}
			set
			{
				this._PTDRevisedAmount = value;
			}
		}
		#endregion
		

		#region FinPTDQty
		public abstract class finPTDQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPTDQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = Messages.FinPTDQuantity)]
        public virtual Decimal? FinPTDQty
		{
			get
			{
				return this._FinPTDQty;
			}
			set
			{
				this._FinPTDQty = value;
			}
		}
		#endregion
		#region TranPTDQty
		public abstract class tranPTDQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPTDQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPTDQty
		{
			get
			{
				return this._TranPTDQty;
			}
			set
			{
				this._TranPTDQty = value;
			}
		}
		#endregion
		#region FinPTDAmount
		public abstract class finPTDAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPTDAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = Messages.FinPTDAmount)]
		public virtual Decimal? FinPTDAmount
		{
			get
			{
				return this._FinPTDAmount;
			}
			set
			{
				this._FinPTDAmount = value;
			}
		}
		#endregion
		#region TranPTDAmount
		public abstract class tranPTDAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPTDAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPTDAmount
		{
			get
			{
				return this._TranPTDAmount;
			}
			set
			{
				this._TranPTDAmount = value;
			}
		}
		#endregion
		#region FinYTDQty
		public abstract class finYTDQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYTDQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYTDQty
		{
			get
			{
				return this._FinYTDQty;
			}
			set
			{
				this._FinYTDQty = value;
			}
		}
		#endregion
		#region TranYTDQty
		public abstract class tranYTDQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYTDQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYTDQty
		{
			get
			{
				return this._TranYTDQty;
			}
			set
			{
				this._TranYTDQty = value;
			}
		}
		#endregion
		#region FinYTDAmount
		public abstract class finYTDAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYTDAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYTDAmount
		{
			get
			{
				return this._FinYTDAmount;
			}
			set
			{
				this._FinYTDAmount = value;
			}
		}
		#endregion
		#region TranYTDAmount
		public abstract class tranYTDAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYTDAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYTDAmount
		{
			get
			{
				return this._TranYTDAmount;
			}
			set
			{
				this._TranYTDAmount = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}

	[PMProjectAccum]
    [Serializable]
	public partial class PMHistoryAccum : PMHistory
	{
		#region ProjectID
		public new abstract class projectID : PX.Data.IBqlField
		{
		}
		#endregion
		#region ProjectTaskID
		public new abstract class projectTaskID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountGroupID
		public new abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		#endregion
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		#endregion
		#region PeriodID
		public new abstract class periodID : PX.Data.IBqlField
		{
		}
		[PXDBString(6, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String PeriodID
		{
			get
			{
				return this._PeriodID;
			}
			set
			{
				this._PeriodID = value;
			}
		}
		#endregion
        
	}

	[PMProjectAccum2]
    [Serializable]
	public partial class PMHistory2Accum : PMHistory
	{
		#region ProjectID
		public new abstract class projectID : PX.Data.IBqlField
		{
		}
		#endregion
		#region ProjectTaskID
		public new abstract class projectTaskID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountGroupID
		public new abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		#endregion
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		#endregion
		#region PeriodID
		public new abstract class periodID : PX.Data.IBqlField
		{
		}
		[PXDBString(6, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String PeriodID
		{
			get
			{
				return this._PeriodID;
			}
			set
			{
				this._PeriodID = value;
			}
		}
		#endregion
	}

	public class PMProjectAccumAttribute : PXAccumulatorAttribute
	{
		public PMProjectAccumAttribute()
			: base(new Type[] {
					typeof(PMHistory.finYTDQty),
					typeof(PMHistory.tranYTDQty),
					typeof(PMHistory.finYTDAmount),
					typeof(PMHistory.tranYTDAmount)
					},
					new Type[] {
					typeof(PMHistory.finYTDQty),
					typeof(PMHistory.tranYTDQty),
					typeof(PMHistory.finYTDAmount),
					typeof(PMHistory.tranYTDAmount)
					}
			)
		{
		}

		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}

			PMHistory hist = (PMHistory)row;

			columns.RestrictPast<PMHistory.periodID>(PXComp.GE, hist.PeriodID.Substring(0, 4) + "01");
			columns.RestrictFuture<PMHistory.periodID>(PXComp.LE, hist.PeriodID.Substring(0, 4) + "99");

			return true;
		}
	}

	public class PMProjectAccum2Attribute : PXAccumulatorAttribute
	{
		public PMProjectAccum2Attribute()
			: base(new Type[] {
					typeof(PMHistory.budgetQty),
					typeof(PMHistory.budgetAmount),
					typeof(PMHistory.revisedQty),
					typeof(PMHistory.revisedAmount)
					},
					new Type[] {
					typeof(PMHistory.budgetQty),
					typeof(PMHistory.budgetAmount),
					typeof(PMHistory.revisedQty),
					typeof(PMHistory.revisedAmount)
					}
			)
		{
		}
 	}
}
