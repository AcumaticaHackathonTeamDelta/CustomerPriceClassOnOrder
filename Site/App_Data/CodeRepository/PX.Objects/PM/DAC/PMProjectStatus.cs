using System;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.GL;
namespace PX.Objects.PM
{

	public interface IPMProjectStatus
	{
		int? ProjectID { get; }
		int? ProjectTaskID { get; }
		int? AccountGroupID { get; }
		int? InventoryID { get; }
		string UOM { get; }
		decimal? ActualQty { get; }
		decimal? ActualAmount { get; }
		decimal? Qty { get; }
		decimal? Amount { get; }
		decimal? RevisedQty { get; set;  }
		decimal? RevisedAmount { get; set;  }

	}
	
	[Serializable]
	public partial class PMProjectStatus : PX.Data.IBqlTable, IPMProjectStatus
	{
		public const int EmptyInventoryID = 0;
        #region RowID
        public abstract class rowID : IBqlField
        {
        }
        protected int? _RowID;
        [PXDBIdentity]
        public virtual int? RowID
        {
            get
            {
                return _RowID;
            }
            set
            {
                _RowID = value;
            }
        }
        #endregion
        #region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDefault]
		[AccountGroupAttribute(IsKey=true)]
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
		[PXDBLiteDefault(typeof(PMTask.projectID))]
        [PXDBInt(IsKey = true)]
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
        [PXDBLiteDefault(typeof(PMTask.taskID))]
        [PXDBInt(IsKey = true)]
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
        #region PeriodID
        public abstract class periodID : PX.Data.IBqlField
        {
        }
        protected String _PeriodID;
        [PXUIField(DisplayName = "Fin. Period")]
        [FinPeriodID(IsKey = true)]
        [PXDefault()]
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
        #region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
        [PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Visible)]
		[PMInventorySelector(typeof(Search2<InventoryItem.inventoryID,
            InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Current<PMProjectStatus.accountGroupID>>>,
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
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<PMProjectStatus.inventoryID>>>>))]
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
		

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Budgeted Qty.")]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PMUnit(typeof(PMProjectStatus.inventoryID))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Rate
		public abstract class rate : PX.Data.IBqlField
		{
		}
		protected Decimal? _Rate;
		[PXDBPriceCost]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Rate")]
		public virtual Decimal? Rate
		{
			get
			{
				return this._Rate;
			}
			set
			{
				this._Rate = value;
			}
		}
		#endregion
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXDBBaseCury]
		[PXFormula(typeof(Mult<PMProjectStatus.qty, PMProjectStatus.rate>))]
		[PXDefault(TypeCode.Decimal,"0.0")]
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
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Revised Qty.")]
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
		[PXFormula(typeof(Mult<PMProjectStatus.revisedQty, PMProjectStatus.rate>))]
		[PXDefault(TypeCode.Decimal,"0.0")]
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
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Actual Qty.", Enabled=false)]
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
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Actual Amount", Enabled=false)]
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
		#region IsProduction
		public abstract class isProduction : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsProduction;
		[PXDefault(false)]
		[PXDBBool()]
		[PXUIField(DisplayName = "Production")]
		public virtual Boolean? IsProduction
		{
			get
			{
				return this._IsProduction;
			}
			set
			{
				this._IsProduction = value;
			}
		}
		#endregion
        #region IsTemplate
        public abstract class isTemplate : IBqlField
        {
        }
	    protected bool? _IsTemplate;
        [PXDBBool]
        [PXDefault(false)]
        public virtual bool? IsTemplate
        {
            get
            {
                return _IsTemplate;
            }
            set
            {
                _IsTemplate = value;
            }
        }
        #endregion

		#region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
        [PXNote]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
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
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#endregion
	}
		
	[ProjectStatusAccum]
    [Serializable]
    [PXHidden]
	public partial class PMProjectStatusAccum : PMProjectStatus
	{
		#region PeriodID
		public new abstract class periodID : PX.Data.IBqlField
		{
		}		
		[PXDBString(6, IsFixed = true, IsKey=true)]
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
		#region ProjectID
		public new abstract class projectID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[PXDBInt(IsKey=true)]
		public override Int32? ProjectID
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
		public new abstract class projectTaskID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[PXDBInt(IsKey = true)]
		public override Int32? ProjectTaskID
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
		public new abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[PXDBInt(IsKey = true)]
		public override Int32? AccountGroupID
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
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[PXDBInt(IsKey = true)]
		public override Int32? InventoryID
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

		#region ActualQty
		public new abstract class actualQty : PX.Data.IBqlField
		{
		}
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? ActualQty
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
		public new abstract class actualAmount : PX.Data.IBqlField
		{
		}
		
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? ActualAmount
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
}
