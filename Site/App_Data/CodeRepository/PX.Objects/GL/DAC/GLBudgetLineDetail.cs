using System;
using PX.Data;

namespace PX.Objects.GL
{
	[Serializable]
	[PXCacheName(Messages.GLBudgetLineDetail)]
	public partial class GLBudgetLineDetail : IBqlTable
	{
		#region BranchID
		public abstract class branchID : IBqlField { }
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual int? BranchID { get; set; }
		#endregion
		#region LedgerID
		public abstract class ledgerID : IBqlField { }
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual int? LedgerID { get; set; }
		#endregion
		#region FinYear
		public abstract class finYear : IBqlField { }
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXDefault]
		public virtual string FinYear { get; set; }
		#endregion
		#region GroupID
		public abstract class groupID : PX.Data.IBqlField
		{
		}
		protected Guid? _GroupID;
		[PXDBGuid(IsKey = true)]
		[PXDefault]
        [PXParent(typeof(Select<GLBudgetLine, Where<GLBudgetLine.groupID, Equal<Current<GLBudgetLineDetail.groupID>>>>))]
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
		#region AccountID
		public abstract class accountID : IBqlField { }
		[PXDBInt]
		[PXDefault]
		public virtual int? AccountID { get; set; }
		#endregion
		#region SubID
		public abstract class subID : IBqlField { }
		[PXDefault]
		[SubAccount]
		public virtual int? SubID { get; set; }
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : IBqlField { }
		[PXDBString(6, IsKey = true)]
		[PXDefault]
		public virtual string FinPeriodID { get; set; }
		#endregion
		#region Amount
		public abstract class amount : IBqlField { }
		[PXDBDecimal(typeof(Search2<CM.Currency.decimalPlaces,
			InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<CM.Currency.curyID>>>,
			Where<Ledger.ledgerID, Equal<Current<GLBudgetLineDetail.ledgerID>>>>))]
		[PXUIField(DisplayName = "Budget Amount")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<GLBudgetLine.allocatedAmount>))]
		public virtual decimal? Amount { get; set; }
		#endregion
		#region ReleasedAmount
		public abstract class releasedAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReleasedAmount;
		[PXDBDecimal(typeof(Search2<CM.Currency.decimalPlaces,
			InnerJoin<Ledger, On<Ledger.baseCuryID, Equal<CM.Currency.curyID>>>,
			Where<Ledger.ledgerID, Equal<Current<GLBudgetLineDetail.ledgerID>>>>))]
		[PXUIField(DisplayName = "Released Amount", Enabled = false)]
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
		#region tstamp
		public abstract class Tstamp : IBqlField { }
		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }
		#endregion
        #region CreatedByID
        public abstract class createdByID : IBqlField { }
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField { }
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField { }
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField { }
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField { }
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField { }
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
	}
}
