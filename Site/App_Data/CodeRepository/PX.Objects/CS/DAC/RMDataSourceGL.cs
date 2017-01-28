using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.CS;

namespace PX.Objects.CS
{
	[Serializable]
	public partial class RMDataSourceGL : PXCacheExtension<RMDataSource>
	{	
		#region LedgerID
		public abstract class ledgerID : IBqlField
		{
		}
		protected Int32? _LedgerID;
		[PXDBInt]
		[PXUIField(DisplayName = "Ledger")]
		[PXSelector(typeof(GL.Ledger.ledgerID), SubstituteKey = typeof(GL.Ledger.ledgerCD), DescriptionField = typeof(GL.Ledger.descr))]
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
		#region StartAccount
		public abstract class startAccount : PX.Data.IBqlField
		{
		}
		protected string _StartAccount;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Start Account")]
		[PXSelector(typeof(GL.Account.accountCD), DescriptionField = typeof(GL.Account.description), ValidateValue = false)]
		public virtual string StartAccount
		{
			get
			{
				return this._StartAccount;
			}
			set
			{
				this._StartAccount = value;
			}
		}
		#endregion
		#region StartSub
		public abstract class startSub : PX.Data.IBqlField
		{
		}
		protected string _StartSub;
		[GL.SubAccountRaw(DisplayName = "Start Sub.")]
		public virtual string StartSub
		{
			get
			{
				return this._StartSub;
			}
			set
			{
				this._StartSub = value;
			}
		}
		#endregion
		#region StartBranch
		public abstract class startBranch : IBqlField
		{
		}
		protected string _StartBranch;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Start Branch")]
		[PXSelector(typeof(Search<Branch.branchCD, Where<MatchWithBranch<Branch.branchID>>>), DescriptionField = typeof(CR.BAccount.acctName), ValidateValue = false)]
		public virtual string StartBranch
		{
			get
			{
				return this._StartBranch;
			}
			set
			{
				this._StartBranch = value;
			}
		}
		#endregion
		#region StartPeriod
		public abstract class startPeriod : PX.Data.IBqlField
		{
		}
		protected string _StartPeriod;
		[GL.FinPeriodSelector(DescriptionField = typeof(FinPeriod.descr), SelectorMode = PXSelectorMode.DisplayModeValue)]
		[PXUIField(DisplayName = "Start Period")]
		public virtual string StartPeriod
		{
			get
			{
				return this._StartPeriod;
			}
			set
			{
				this._StartPeriod = value;
			}
		}
		#endregion		
		#region EndPeriod
		public abstract class endPeriod : PX.Data.IBqlField
		{
		}
		protected string _EndPeriod;
		[GL.FinPeriodSelector(DescriptionField = typeof(FinPeriod.descr), SelectorMode = PXSelectorMode.DisplayModeValue)]
		[PXUIField(DisplayName = "End Period")]
		public virtual string EndPeriod
		{
			get
			{
				return this._EndPeriod;
			}
			set
			{
				this._EndPeriod = value;
			}
		}
		#endregion		
		#region AccountClassID
		public abstract class accountClassID : PX.Data.IBqlField
		{
		}
		protected string _AccountClassID;
		[PXDBString(20, IsUnicode = true)]
		[PXUIField(DisplayName = "Account Class")]
		[PXSelector(typeof(GL.AccountClass.accountClassID), DescriptionField = typeof(GL.AccountClass.descr))]
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
		#region EndAccount
		public abstract class endAccount : PX.Data.IBqlField
		{
		}
		protected string _EndAccount;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "End Account")]
		[PXSelector(typeof(GL.Account.accountCD), DescriptionField = typeof(GL.Account.description), ValidateValue = false)]
		public virtual string EndAccount
		{
			get
			{
				return this._EndAccount;
			}
			set
			{
				this._EndAccount = value;
			}
		}
		#endregion				
		#region EndSub
		public abstract class endSub : PX.Data.IBqlField
		{
		}
		protected string _EndSub;
		[GL.SubAccountRaw(DisplayName = "End Sub.")]
		public virtual string EndSub
		{
			get
			{
				return this._EndSub;
			}
			set
			{
				this._EndSub = value;
			}
		}
		#endregion			
		#region EndBranch
		public abstract class endBranch : IBqlField
		{
		}
		protected string _EndBranch;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "End Branch")]
		[PXSelector(typeof(Search<Branch.branchCD, Where<MatchWithBranch<Branch.branchID>>>), DescriptionField = typeof(CR.BAccount.acctName), ValidateValue = false)]
		public virtual string EndBranch
		{
			get
			{
				return this._EndBranch;
			}
			set
			{
				this._EndBranch = value;
			}
		}
		#endregion
		#region StartPeriodYearOffset
		public abstract class startPeriodYearOffset : PX.Data.IBqlField
		{
		}
		protected short? _StartPeriodYearOffset;
		[PXDBShort]
		[PXUIField(DisplayName = "Offset (Year, Period)")]//[PXUIField(DisplayName = "Year Offset")]
		public virtual short? StartPeriodYearOffset
		{
			get
			{
				return this._StartPeriodYearOffset;
			}
			set
			{
				this._StartPeriodYearOffset = value;
			}
		}
		#endregion
		#region StartPeriodOffset
		public abstract class startPeriodOffset : PX.Data.IBqlField
		{
		}
		protected short? _StartPeriodOffset;
		[PXDBShort]
		[PXUIField(DisplayName = "")] //[PXUIField(DisplayName = "Offset (Year, Period)")]
		public virtual short? StartPeriodOffset
		{
			get
			{
				return this._StartPeriodOffset;
			}
			set
			{
				this._StartPeriodOffset = value;
			}
		}
		#endregion
		#region EndPeriodYearOffset
		public abstract class endPeriodYearOffset : PX.Data.IBqlField
		{
		}
		protected short? _EndPeriodYearOffset;
		[PXDBShort]
		[PXUIField(DisplayName = "Offset (Year, Period)")]//[PXUIField(DisplayName = "Year Offset")]
		public virtual short? EndPeriodYearOffset
		{
			get
			{
				return this._EndPeriodYearOffset;
			}
			set
			{
				this._EndPeriodYearOffset = value;
			}
		}
		#endregion
		#region EndPeriodOffset
		public abstract class endPeriodOffset : PX.Data.IBqlField
		{
		}
		protected short? _EndPeriodOffset;
		[PXDBShort]
		[PXUIField(DisplayName = "")]//[PXUIField(DisplayName = "Offset (Year, Period)")]		
		public virtual short? EndPeriodOffset
		{
			get
			{
				return this._EndPeriodOffset;
			}
			set
			{
				this._EndPeriodOffset = value;
			}
		}
		#endregion		
	}
}
