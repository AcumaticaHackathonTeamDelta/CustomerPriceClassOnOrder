namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using System.Text.RegularExpressions;

	[System.SerializableAttribute()]
	public partial class GLHistoryEnqFilter : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(Required = false)]
        [PXRestrictor(typeof(Where<True, Equal<True>>), GL.Messages.BranchInactive, ReplaceInherited = true)]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[AnyPeriodFilterable(typeof(AccessInfo.businessDate))]
		[PXDefault()]
		[PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Visible)]
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
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		protected Int32? _LedgerID;
		[PXDBInt()]
		[PXDefault(typeof(Search<Branch.ledgerID, Where<Branch.branchID, Equal<Current<AccountByPeriodFilter.branchID>>>>))]
		[PXUIField(DisplayName = "Ledger", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Ledger.ledgerID), DescriptionField = typeof(Ledger.descr), SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[AccountAny] //Allow YtdNetIncome be visible 
		[PXDefault()]
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
		//[PXDBInt()]
		[SubAccount(Visibility = PXUIVisibility.Visible)]
		[PXDefault()]
		//[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Invisible)]
		//[PXSelector(typeof(Sub.subID), DescriptionField = typeof(Sub.description), SubstituteKey = typeof(Sub.subCD))]
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
		#region SubCD
		public abstract class subCD : PX.Data.IBqlField
		{
		}
		protected String _SubCD;
		[SubAccountRestrictedRaw(DisplayName = "Subaccount",SuppressValidation=true)]
		public virtual String SubCD
		{
			get
			{
				return this._SubCD;
			}
			set
			{
				this._SubCD = value;
			}
		}
		#endregion
		
		#region BegFinPeriod
		public abstract class begFinPeriod : PX.Data.IBqlField
		{
		}
		public virtual String BegFinPeriod
		{
			get
			{
				if (this._FinPeriodID != null)
					return FirstPeriodOfYear(FiscalPeriodUtils.FiscalYear(this._FinPeriodID));
				else
					return null;
			}
		}
		#endregion BegFinPeriod
		#region SubCD Wildcard 
		public abstract class subCDWildcard : PX.Data.IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual String SubCDWildcard
		{
			[PXDependsOnFields(typeof(subCD))]
			get
			{
				return SubCDUtils.CreateSubCDWildcard(this._SubCD, SubAccountAttribute.DimensionName);
			}
		}

		
	
		#endregion
		#region ShowCuryDetails
		public abstract class showCuryDetail : PX.Data.IBqlField
		{
		}
		protected bool? _ShowCuryDetail;
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "Show Currency Details", Visibility = PXUIVisibility.Visible)]
		public virtual bool? ShowCuryDetail
		{
			get
			{
				return this._ShowCuryDetail;
			}
			set
			{
				this._ShowCuryDetail = value;
			}
		}
		#endregion
		#region AccountClassID
		public abstract class accountClassID : PX.Data.IBqlField
		{
		}
		protected string _AccountClassID;
		[PXDBString(20, IsUnicode = true)]
		[PXUIField(DisplayName = "Account Class", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(AccountClass.accountClassID))]
		public virtual string AccountClassID
		{
			get
			{
				return this._AccountClassID;
			}
			set
			{
				this._AccountClassID = value;
			}
		}
		#endregion
		#region Internal Functions 
		protected static string FirstPeriodOfYear(string year) 
		{
			return string.Concat(year, CS_FIRST_PERIOD);
		}
		private const string CS_FIRST_PERIOD = "01";
		#endregion 
	}
}
