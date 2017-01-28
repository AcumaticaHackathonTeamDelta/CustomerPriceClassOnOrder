namespace PX.Objects.DR
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CM;
	using PX.Objects.CR;
	using PX.Objects.AR;
	using PX.Objects.CT;
using PX.Objects.AP;

	[System.SerializableAttribute()]
	[PXAccumulator(
		new Type[] {
                typeof(DRRevenueBalance.endBalance),
				typeof(DRRevenueBalance.endProjected),
				typeof(DRRevenueBalance.endBalance),
				typeof(DRRevenueBalance.endProjected)
            },
			new Type[] {
                typeof(DRRevenueBalance.begBalance),
				typeof(DRRevenueBalance.begProjected),
				typeof(DRRevenueBalance.endBalance),
				typeof(DRRevenueBalance.endProjected)
            }
		)]
	[PXCacheName(Messages.DRRevenueBalance)]
	public partial class DRRevenueBalance : PX.Data.IBqlTable
	{
		#region AcctID
		public abstract class acctID : PX.Data.IBqlField
		{
		}
		protected Int32? _AcctID;
		[Account(IsKey=true, DisplayName = "Account", Visibility = PXUIVisibility.Invisible, DescriptionField = typeof(Account.description))]
		public virtual Int32? AcctID
		{
			get
			{
				return this._AcctID;
			}
			set
			{
				this._AcctID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(DRRevenueBalance.acctID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Invisible, DescriptionField = typeof(Sub.description), IsKey = true)]
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
		#region ComponentID
		public abstract class componentID : PX.Data.IBqlField
		{
		}
		protected Int32? _ComponentID;

		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodID(IsKey = true)]
		[PXUIField(DisplayName = "FinPeriod", Enabled = false)]
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
		
		#region BegBalance
		public abstract class begBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBalance;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Begin Balance")]
		public virtual Decimal? BegBalance
		{
			get
			{
				return this._BegBalance;
			}
			set
			{
				this._BegBalance = value;
			}
		}
		#endregion
		#region BegProjected
		public abstract class begProjected : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegProjected;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Begin Projected")]
		public virtual Decimal? BegProjected
		{
			get
			{
				return this._BegProjected;
			}
			set
			{
				this._BegProjected = value;
			}
		}
		#endregion
		#region PTDDeferred
		public abstract class pTDDeferred : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDDeferred;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Deferred Amount")]
		public virtual Decimal? PTDDeferred
		{
			get
			{
				return this._PTDDeferred;
			}
			set
			{
				this._PTDDeferred = value;
			}
		}
		#endregion
		#region PTDRecognized
		public abstract class pTDRecognized : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDRecognized;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Recognized Amount")]
		public virtual Decimal? PTDRecognized
		{
			get
			{
				return this._PTDRecognized;
			}
			set
			{
				this._PTDRecognized = value;
			}
		}
		#endregion
		#region PTDRecognizedSamePeriod
		public abstract class pTDRecognizedSamePeriod : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDRecognizedSamePeriod;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Recognized Amount in Same Period")]
		public virtual Decimal? PTDRecognizedSamePeriod
		{
			get
			{
				return this._PTDRecognizedSamePeriod;
			}
			set
			{
				this._PTDRecognizedSamePeriod = value;
			}
		}
		#endregion
		#region PTDProjected
		public abstract class pTDProjected : PX.Data.IBqlField
		{
		}
		protected Decimal? _PTDProjected;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Projected Amount")]
		public virtual Decimal? PTDProjected
		{
			get
			{
				return this._PTDProjected;
			}
			set
			{
				this._PTDProjected = value;
			}
		}
		#endregion
		#region EndBalance
		public abstract class endBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndBalance;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Endin Balance")]
		public virtual Decimal? EndBalance
		{
			get
			{
				return this._EndBalance;
			}
			set
			{
				this._EndBalance = value;
			}
		}
		#endregion
		#region EndProjected
		public abstract class endProjected : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndProjected;
		[PXDBBaseCuryAttribute()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Endin Projected")]
		public virtual Decimal? EndProjected
		{
			get
			{
				return this._EndProjected;
			}
			set
			{
				this._EndProjected = value;
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


    [PXProjection(typeof(Select5<DRRevenueBalance,
        InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, GreaterEqual<DRRevenueBalance.finPeriodID>>>,
       Aggregate<GroupBy<DRRevenueBalance.acctID,
       GroupBy<DRRevenueBalance.subID,
       GroupBy<DRRevenueBalance.componentID,
       GroupBy<DRRevenueBalance.customerID,
       GroupBy<DRRevenueBalance.projectID,
       Max<DRRevenueBalance.finPeriodID,
       GroupBy<FinPeriod.finPeriodID
        >>>>>>>>>))]
    [Serializable]
	[PXCacheName(Messages.DRRevenueBalanceByPeriod)]
	public partial class DRRevenueBalanceByPeriod : PX.Data.IBqlTable
    {
        #region AcctID
        public abstract class acctID : PX.Data.IBqlField
        {
        }
        protected Int32? _AcctID;
        [Account(IsKey = true, BqlField=typeof(DRRevenueBalance.acctID), DisplayName = "Account", Visibility = PXUIVisibility.Invisible, DescriptionField = typeof(Account.description))]
        public virtual Int32? AcctID
        {
            get
            {
                return this._AcctID;
            }
            set
            {
                this._AcctID = value;
            }
        }
        #endregion
        #region SubID
        public abstract class subID : PX.Data.IBqlField
        {
        }
        protected Int32? _SubID;
        [SubAccount(typeof(DRRevenueBalance.acctID), BqlField = typeof(DRRevenueBalance.subID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Invisible, DescriptionField = typeof(Sub.description), IsKey = true)]
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
        #region ComponentID
        public abstract class componentID : PX.Data.IBqlField
        {
        }
        protected Int32? _ComponentID;

        [PXDBInt(IsKey = true, BqlField=typeof(DRRevenueBalance.componentID))]
        [PXDefault()]
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
        #region CustomerID
        public abstract class customerID : PX.Data.IBqlField
        {
        }
        protected Int32? _CustomerID;
        [PXDBInt(IsKey = true, BqlField=typeof(DRRevenueBalance.customerID))]
        [PXDefault()]
        public virtual Int32? CustomerID
        {
            get
            {
                return this._CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }
        #endregion
        #region ProjectID
        public abstract class projectID : PX.Data.IBqlField
        {
        }
        protected Int32? _ProjectID;
        [PXDBInt(IsKey = true, BqlField=typeof(DRRevenueBalance.projectID))]
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

        #region LastActivityPeriod
        public abstract class lastActivityPeriod : PX.Data.IBqlField
        {
        }
        protected String _LastActivityPeriod;
        [FinPeriodID(BqlField = typeof(DRRevenueBalance.finPeriodID))]
        public virtual String LastActivityPeriod
        {
            get
            {
                return this._LastActivityPeriod;
            }
            set
            {
                this._LastActivityPeriod = value;
            }
        }
        #endregion
        #region FinPeriodID
        public abstract class finPeriodID : PX.Data.IBqlField
        {
        }
        protected String _FinPeriodID;
        [FinPeriodID(IsKey = true, BqlField = typeof(FinPeriod.finPeriodID))]
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
    }
}
