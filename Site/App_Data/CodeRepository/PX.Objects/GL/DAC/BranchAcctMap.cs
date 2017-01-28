using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.GL
{
    [PXCacheName(Messages.BranchAcctMap)]
    [Serializable]
    [PXHidden]
	public partial class BranchAcctMap : IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(Branch.branchID))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(Branch.acctMapNbr))]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region FromBranchID
		public abstract class fromBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _FromBranchID;
		[PXDBInt()]
		public virtual Int32? FromBranchID
		{
			get
			{
				return this._FromBranchID;
			}
			set
			{
				this._FromBranchID = value;
			}
		}
		#endregion
		#region ToBranchID
		public abstract class toBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToBranchID;
		[PXDBInt()]
		public virtual Int32? ToBranchID
		{
			get
			{
				return this._ToBranchID;
			}
			set
			{
				this._ToBranchID = value;
			}
		}
		#endregion
		#region FromAccountCD
		public abstract class fromAccountCD : PX.Data.IBqlField
		{
		}
		protected String _FromAccountCD;
		[PXDefault()]
		[AccountRaw()]
		public virtual String FromAccountCD
		{
			get
			{
				return this._FromAccountCD;
			}
			set
			{
				this._FromAccountCD = value;
			}
		}
		#endregion
		#region ToAccountCD
		public abstract class toAccountCD : PX.Data.IBqlField
		{
		}
		protected String _ToAccountCD;
		[PXDefault()]
		[AccountRaw()]
		public virtual String ToAccountCD
		{
			get
			{
				return this._ToAccountCD;
			}
			set
			{
				this._ToAccountCD = value;
			}
		}
		#endregion
		#region MapAccountID
		public abstract class mapAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapAccountID;
		[Account(DescriptionField = typeof(Account.description))]
		[PXDefault]
		public virtual Int32? MapAccountID
		{
			get
			{
				return this._MapAccountID;
			}
			set
			{
				this._MapAccountID = value;
			}
		}
		#endregion
		#region MapSubID
		public abstract class mapSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapSubID;
		[SubAccount(typeof(BranchAcctMap.mapAccountID), DescriptionField = typeof(Account.description))]
		[PXDefault]
		public virtual Int32? MapSubID
		{
			get
			{
				return this._MapSubID;
			}
			set
			{
				this._MapSubID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select<BranchAcctMap, Where<BranchAcctMap.branchID, Equal<BranchAcctMap.fromBranchID>>>), Persistent = true)]
    [PXCacheName(Messages.BranchAcctMapFrom)]
    [Serializable]
	public partial class BranchAcctMapFrom : IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt(IsKey = true, BqlTable = typeof(BranchAcctMap))]
		[PXDBLiteDefault(typeof(Branch.branchID))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true, BqlTable = typeof(BranchAcctMap))]
		[PXLineNbr(typeof(Branch.acctMapNbr))]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region FromBranchID
		public abstract class fromBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _FromBranchID;
		[PXDBInt(BqlTable = typeof(BranchAcctMap))]
		[PXDBLiteDefault(typeof(Branch.branchID))]
		public virtual Int32? FromBranchID
		{
			get
			{
				return this._FromBranchID;
			}
			set
			{
				this._FromBranchID = value;
			}
		}
		#endregion
		#region ToBranchID
		public abstract class toBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToBranchID;
		[PXDBInt(BqlTable = typeof(BranchAcctMap))]
		[PXSelector(typeof(Search<Branch.branchID, Where<Branch.branchID, NotEqual<Current<BranchAcctMapFrom.branchID>>>>), SubstituteKey = typeof(Branch.branchCD))]
        [PXUIField(DisplayName = "Destination Branch")]
        [PXRestrictor(typeof(Where<Branch.active, Equal<True>>), GL.Messages.BranchInactive)]
		public virtual Int32? ToBranchID
		{
			get
			{
				return this._ToBranchID;
			}
			set
			{
				this._ToBranchID = value;
			}
		}
		#endregion
		#region FromAccountCD
		public abstract class fromAccountCD : PX.Data.IBqlField
		{
		}
		protected String _FromAccountCD;
        [PXDBString(10, IsUnicode = true, InputMask = "", BqlTable = typeof(BranchAcctMap))]
        [PXDefault]
		[PXDimensionSelector("ACCOUNT", typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
		[PXUIField(DisplayName = "Account From")]
        public virtual String FromAccountCD
		{
			get
			{
				return this._FromAccountCD;
			}
			set
			{
				this._FromAccountCD = value;
			}
		}
		#endregion
		#region ToAccountCD
		public abstract class toAccountCD : PX.Data.IBqlField
		{
		}
		protected String _ToAccountCD;
        [PXDBString(10, IsUnicode = true, InputMask = "", BqlTable = typeof(BranchAcctMap))]
        [PXDefault]
        [PXDimensionSelector("ACCOUNT", typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "Account To")]
        public virtual String ToAccountCD
		{
			get
			{
				return this._ToAccountCD;
			}
			set
			{
				this._ToAccountCD = value;
			}
		}
		#endregion
		#region MapAccountID
		public abstract class mapAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapAccountID;
        [Account(null, typeof(Search2<Account.accountID, LeftJoin<CA.CashAccount, On<CA.CashAccount.accountID, Equal<Account.accountID>>>, Where<CA.CashAccount.accountID, IsNull>>), DescriptionField = typeof(Account.description), BqlTable = typeof(BranchAcctMap), DisplayName = "Offset Account")]
		[PXDefault]
		public virtual Int32? MapAccountID
		{
			get
			{
				return this._MapAccountID;
			}
			set
			{
				this._MapAccountID = value;
			}
		}
		#endregion
		#region MapSubID
		public abstract class mapSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapSubID;
        [SubAccount(typeof(BranchAcctMapFrom.mapAccountID), DisplayName = "Offset Subaccount", DescriptionField = typeof(Account.description), BqlTable = typeof(BranchAcctMap))]
		[PXDefault]
		public virtual Int32? MapSubID
		{
			get
			{
				return this._MapSubID;
			}
			set
			{
				this._MapSubID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select<BranchAcctMap,Where<BranchAcctMap.branchID, Equal<BranchAcctMap.toBranchID>>>), Persistent = true)]
    [PXCacheName(Messages.BranchAcctMapTo)]
    [Serializable]
	public partial class BranchAcctMapTo : IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt(IsKey = true, BqlTable = typeof(BranchAcctMap))]
		[PXDBLiteDefault(typeof(Branch.branchID))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true, BqlTable = typeof(BranchAcctMap))]
		[PXLineNbr(typeof(Branch.acctMapNbr))]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region FromBranchID
		public abstract class fromBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _FromBranchID;
		[PXDBInt(BqlTable = typeof(BranchAcctMap))]
		[PXSelector(typeof(Search<Branch.branchID, Where<Branch.branchID, NotEqual<Current<BranchAcctMapFrom.branchID>>>>), SubstituteKey = typeof(Branch.branchCD))]
        [PXUIField(DisplayName = "Destination Branch")]
        [PXRestrictor(typeof(Where<Branch.active, Equal<True>>), GL.Messages.BranchInactive)]
		public virtual Int32? FromBranchID
		{
			get
			{
				return this._FromBranchID;
			}
			set
			{
				this._FromBranchID = value;
			}
		}
		#endregion
		#region ToBranchID
		public abstract class toBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToBranchID;
		[PXDBInt(BqlTable = typeof(BranchAcctMap))]
		[PXDBLiteDefault(typeof(Branch.branchID))]
		public virtual Int32? ToBranchID
		{
			get
			{
				return this._ToBranchID;
			}
			set
			{
				this._ToBranchID = value;
			}
		}
		#endregion
		#region FromAccountCD
		public abstract class fromAccountCD : PX.Data.IBqlField
		{
		}
		protected String _FromAccountCD;
        [PXDBString(10, IsUnicode = true, InputMask = "", BqlTable = typeof(BranchAcctMap))]
        [PXDefault]
        [PXDimensionSelector("ACCOUNT", typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "Account From")]
        public virtual String FromAccountCD
		{
			get
			{
				return this._FromAccountCD;
			}
			set
			{
				this._FromAccountCD = value;
			}
		}
		#endregion
		#region ToAccountCD
		public abstract class toAccountCD : PX.Data.IBqlField
		{
		}
		protected String _ToAccountCD;
        [PXDBString(10, IsUnicode = true, InputMask = "", BqlTable = typeof(BranchAcctMap))]
        [PXDefault]
        [PXDimensionSelector("ACCOUNT", typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "Account To")]
        public virtual String ToAccountCD
		{
			get
			{
				return this._ToAccountCD;
			}
			set
			{
				this._ToAccountCD = value;
			}
		}
		#endregion
		#region MapAccountID
		public abstract class mapAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapAccountID;
        [Account(null, typeof(Search2<Account.accountID, LeftJoin<CA.CashAccount, On<CA.CashAccount.accountID, Equal<Account.accountID>>>, Where<CA.CashAccount.accountID, IsNull>>), DescriptionField = typeof(Account.description), BqlTable = typeof(BranchAcctMap), DisplayName = "Offset Account")]
		[PXDefault]
		public virtual Int32? MapAccountID
		{
			get
			{
				return this._MapAccountID;
			}
			set
			{
				this._MapAccountID = value;
			}
		}
		#endregion
		#region MapSubID
		public abstract class mapSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _MapSubID;
        [SubAccount(typeof(BranchAcctMapTo.mapAccountID), DisplayName = "Offset Subaccount", DescriptionField = typeof(Account.description), BqlTable = typeof(BranchAcctMap))]
		[PXDefault]
		public virtual Int32? MapSubID
		{
			get
			{
				return this._MapSubID;
			}
			set
			{
				this._MapSubID = value;
			}
		}
		#endregion
	}
}
