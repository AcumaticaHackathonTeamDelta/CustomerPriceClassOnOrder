using System;
using PX.Data;

namespace PX.Objects.GL
{
	[Serializable]
	[PXPrimaryGraph(typeof(GLBudgetEntry), Filter = typeof(BudgetFilter))]
	[PXCacheName(Messages.BudgetArticle)]
	public partial class GLBudgetLine : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region GroupID
		public abstract class groupID : PX.Data.IBqlField
		{
		}
		protected Guid? _GroupID;
		[PXDBGuid(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "GroupID", Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? GroupID
		{
			get
			{
				return this._GroupID;
			}
			set
			{
				this._GroupID = value;
			}
		}
		#endregion
		#region ParentGroupID
		public abstract class parentGroupID : PX.Data.IBqlField
		{
		}
		protected Guid? _ParentGroupID;
		[PXDBGuid(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "ParentGroupID", Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? ParentGroupID
		{
			get
			{
				return this._ParentGroupID;
			}
			set
			{
				this._ParentGroupID = value;
			}
		}
		#endregion
		#region Rollup
		public abstract class rollup : PX.Data.IBqlField
		{
		}
		protected bool? _Rollup;
		[PXDBBool()]
		[PXUIField(DisplayName = "Rollup", Visible = false, Enabled=false, Visibility=PXUIVisibility.Invisible)]
		[PXDefault(false)]
		public virtual bool? Rollup
		{
			get
			{
				return this._Rollup;
			}
			set
			{
				this._Rollup = value;
			}
		}
		#endregion
		#region IsGroup
		public abstract class isGroup : PX.Data.IBqlField
		{
		}
		protected bool? _IsGroup;
		[PXDBBool()]
        [PXUIField(DisplayName = "Node", Enabled = false)]
		[PXDefault(false)]
		public virtual bool? IsGroup
		{
			get
			{
				return this._IsGroup;
			}
			set
			{
				this._IsGroup = value;
			}
		}
		#endregion
		#region IsPreloaded
		public abstract class isPreloaded : PX.Data.IBqlField
		{
		}
		protected bool? _IsPreloaded;
		[PXDBBool()]
		[PXUIField(DisplayName = "Preloaded", Visible = false, Enabled = false)]
		[PXDefault(false)]
		public virtual bool? IsPreloaded
		{
			get
			{
				return this._IsPreloaded;
			}
			set
			{
				this._IsPreloaded = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : IBqlField {}
        [Branch(typeof(BudgetFilter.branchID), IsKey = true, Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual int? BranchID { get; set; }
		#endregion
		#region LedgerID
		public abstract class ledgerID : IBqlField {}
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(BudgetFilter.ledgerID))]
		[PXUIField(Enabled = false, Visible = false, Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual int? LedgerID { get; set; }
		#endregion
		#region FinYear
		public abstract class finYear : IBqlField {}
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXUIField(Visible = false, DisplayName = "Financial Year")]
		[PXDefault(typeof(BudgetFilter.finYear))]
		public virtual string FinYear { get; set; }
		#endregion
		#region AccountID
		public abstract class accountID : IBqlField {}
		[PXDBInt]
		[PXDimensionSelector(AccountAttribute.DimensionName, (typeof(Search<Account.accountID, Where<Account.accountCD, Like<Current<SelectedGroup.accountMaskWildcard>>>, OrderBy<Asc<Account.accountCD>>>)), typeof(Account.accountCD), DescriptionField=typeof(Account.description))]
        [PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Dynamic)]
		[PXDefault]
		public virtual int? AccountID { get; set; }
		#endregion
		#region SubID
		public abstract class subID : IBqlField {}
		[SubAccount(Visibility = PXUIVisibility.Dynamic)]
		[PXDefault]
		public virtual int? SubID { get; set; }
		#endregion
		#region Description
		public abstract class description : IBqlField {}
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		[PXDefault(typeof(Search<Account.description, Where<Account.accountID, Equal<Current<GLBudgetLine.accountID>>>>))]
		public virtual string Description { get; set; }
		#endregion
		#region Amount
		public abstract class amount : IBqlField {}
		[PXDBDecimal(typeof(Search2<CM.Currency.decimalPlaces,
			InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<CM.Currency.curyID>>>,
			Where<Ledger.ledgerID, Equal<Current<GLBudgetLine.ledgerID>>>>))]
		[PXUIField(DisplayName = "Amount")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? Amount { get; set; }
		#endregion
		#region AllocatedAmount
		public abstract class allocatedAmount : IBqlField {}
		protected Decimal? _AllocatedAmount;
		[PXDBDecimal(typeof(Search2<CM.Currency.decimalPlaces,
			InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<CM.Currency.curyID>>>,
			Where<Ledger.ledgerID, Equal<Current<GLBudgetLine.ledgerID>>>>))]
		[PXUIField(DisplayName = "Distributed Amount", Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? AllocatedAmount
		{
			get
			{
				return this._AllocatedAmount;
			}
			set
			{
				this._AllocatedAmount = value;
			}
		}
		#endregion
		#region ReleasedAmount
		public abstract class releasedAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReleasedAmount;
		[PXDBDecimal(typeof(Search2<CM.Currency.decimalPlaces,
			InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<CM.Currency.curyID>>>,
			Where<Ledger.ledgerID, Equal<Current<GLBudgetLine.ledgerID>>>>))]
		[PXUIField(DisplayName = "Released Amount", Enabled = false, Visible= false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ReleasedAmount
		{
			get
			{
				return this._ReleasedAmount;
			}
			set
			{
				this._ReleasedAmount = value;
			}
		}
		#endregion
		#region AccountMask
		public abstract class accountMask : PX.Data.IBqlField
		{
		}
		protected string _AccountMask;
		[PXUIField(DisplayName = "Account Mask", Enabled = false, Visible = false)]
		[PXDefault("", PersistingCheck=PXPersistingCheck.Nothing)]
		[PXDBString(10, IsUnicode = true)]
		public virtual string AccountMask { get; set; }
		#endregion
		#region SubMask
		public abstract class subMask : PX.Data.IBqlField
		{
		}
		protected string _SubMask;
		[PXUIField(DisplayName = "Subaccount Mask", Enabled = false, Visible = false)]
		[PXDefault("", PersistingCheck=PXPersistingCheck.Nothing)]
		[PXDBString(30, IsUnicode = true)]
		public virtual string SubMask { get; set; }
		#endregion
		#region Released
		public abstract class released : IBqlField {}
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released", Enabled = false)]
		public virtual bool? Released { get; set; }
		#endregion
		#region Comparison
		public abstract class comparison : IBqlField {}
		protected bool? _Comparison;
		[PXBool]
		public virtual bool? Comparison {
			get
			{
				return this._Comparison;
			}
			set
			{
				this._Comparison = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : IBqlField {}
		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
		#region GroupMask
		public abstract class groupMask : IBqlField
		{
		}
		protected Byte[] _GroupMask;
		[PXDBGroupMask]
		public virtual Byte[] GroupMask
		{
			get
			{
				return this._GroupMask;
			}
			set
			{
				this._GroupMask = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : IBqlField {}
		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }
		#endregion
        #region CreatedByID
        public abstract class createdByID : IBqlField { }
        [PXDBCreatedByID]
        [PXUIField(Visibility = PXUIVisibility.Invisible)]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField {}
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField {}
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField {}
		[PXDBLastModifiedByID]
        [PXUIField(Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField {}
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField {}
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
		public decimal[] Allocated;
		public decimal[] _Compared;
		public virtual decimal[] Compared
		{
			get
			{
				return this._Compared;
			}
			set
			{
				this._Compared = value;
			}
		}
		#region TreeSortOrder
		public abstract class treeSortOrder : PX.Data.IBqlField
		{
		}
		protected int? _TreeSortOrder;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "TreeSortOrder", Visibility = PXUIVisibility.Invisible)]
		public virtual int? TreeSortOrder
		{
			get
			{
				return this._TreeSortOrder;
			}
			set
			{
				this._TreeSortOrder = value;
			}
		}
		#endregion
		#region SortOrder
		public abstract class sortOrder : IBqlField { }
		public virtual int? SortOrder { get; set; }
		#endregion
		#region IsUploaded
		public abstract class isUploaded : IBqlField { }
		public virtual bool? IsUploaded { get; set; }
		#endregion
		#region Cleared
		public abstract class cleared : IBqlField { }
		public virtual bool? Cleared { get; set; }
		#endregion
	}
}
