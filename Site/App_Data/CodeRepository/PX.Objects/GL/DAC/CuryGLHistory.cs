namespace PX.Objects.GL
{
	using System;
	using PX.Data;

	[Obsolete("CuryGLHistory is not used. It will be removed in the later versions.")]
	[System.SerializableAttribute()]
	public partial class CuryGLHistory : BaseGLHistory, PX.Data.IBqlTable
	{
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		#endregion
        #region FinYear
        public abstract class finYear : PX.Data.IBqlField
        {
        }
        [PXDBCalced(typeof(Substring<finPeriodID, CS.int1, CS.int4>), typeof(string))]
        public virtual string FinYear
        {
            get;
            set;
        }
        #endregion
		#region BalanceType
		public abstract class balanceType : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true, IsKey = true)]
		public override String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region DetDeleted
		public abstract class detDeleted : PX.Data.IBqlField
		{
		}
		protected Boolean? _DetDeleted;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? DetDeleted
		{
			get
			{
				return this._DetDeleted;
			}
			set
			{
				this._DetDeleted = value;
			}
		}
		#endregion
		#region FinPtdCredit
		public abstract class finPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdDebit
		public abstract class finPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinYtdBalance
		public abstract class finYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinBegBalance
		public abstract class finBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdRevalued
		[PXDecimal(4)]
		public override Decimal? FinPtdRevalued
		{
			get
			{
				return this._FinPtdRevalued;
			}
			set
			{
				this._FinPtdRevalued = value;
			}
		}
		#endregion
		#region TranPtdCredit
		public abstract class tranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdDebit
		public abstract class tranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranYtdBalance
		public abstract class tranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranBegBalance
		public abstract class tranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdCredit
		public abstract class curyFinPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdDebit
		public abstract class curyFinPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinYtdBalance
		public abstract class curyFinYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinBegBalance
		public abstract class curyFinBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdCredit
		public abstract class curyTranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdDebit
		public abstract class curyTranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranYtdBalance
		public abstract class curyTranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranBegBalance
		public abstract class curyTranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		#endregion
		#region BaseCuryID
		public abstract class baseCuryID : PX.Data.IBqlField
		{
		}
		protected String _BaseCuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXDefault("")]
		public virtual String BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
	}
}
