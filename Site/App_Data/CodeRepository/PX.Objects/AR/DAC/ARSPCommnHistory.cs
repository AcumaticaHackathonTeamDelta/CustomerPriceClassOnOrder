using PX.Objects.PR;

namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARSPCommnHistory)]
	public partial class ARSPCommnHistory : PX.Data.IBqlTable
	{        
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesPersonID;
		[PXDefault()]
		[SalesPerson(DescriptionField = typeof(SalesPerson.descr),IsKey = true)]
		public virtual Int32? SalesPersonID
		{
			get
			{
				return this._SalesPersonID;
			}
			set
			{
				this._SalesPersonID = value;
			}
		}
		#endregion
		#region CommnPeriod
		public abstract class commnPeriod : PX.Data.IBqlField
		{
		}
		protected String _CommnPeriod;
		[PXDefault()]
		[GL.FinPeriodID(IsKey=true)]
		[PXUIField(DisplayName = "Commission Period")]
		public virtual String CommnPeriod
		{
			get
			{
				return this._CommnPeriod;
			}
			set
			{
				this._CommnPeriod = value;
			}
		}
		#endregion        
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [GL.Branch(IsKey = true)]
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
		#region CustomerLocationID
		public abstract class customerLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerLocationID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? CustomerLocationID
		{
			get
			{
				return this._CustomerLocationID;
			}
			set
			{
				this._CustomerLocationID = value;
			}
		}
		#endregion
		#region CommnAmt
		public abstract class commnAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Commission Amount")]
		public virtual Decimal? CommnAmt
		{
			get
			{
				return this._CommnAmt;
			}
			set
			{
				this._CommnAmt = value;
			}
		}
		#endregion
		#region CommnblAmt
		public abstract class commnblAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnblAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Commissionable Amount")]
		public virtual Decimal? CommnblAmt
		{
			get
			{
				return this._CommnblAmt;
			}
			set
			{
				this._CommnblAmt = value;
			}
		}
		#endregion
		#region PRProcessedDate
		public abstract class pRProcessedDate : PX.Data.IBqlField
		{
		}
		[PXDBDate]
		[PXUIField(DisplayName = "PR Processed Date", FieldClass=PRSetup.PayrollFieldClass)]
		public virtual DateTime? PRProcessedDate { get; set; }
		#endregion
		
		#region Type
		public abstract class type : IBqlField
		{
		}
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Type", FieldClass = PRSetup.PayrollFieldClass, Enabled=false)]
		[CR.BAccountType.SalesPersonTypeListAttribute]
		public virtual String Type { get; set; }
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
}
