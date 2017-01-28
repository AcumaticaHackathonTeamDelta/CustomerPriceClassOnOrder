namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARStatementDetail)]
	public partial class ARStatementDetail : PX.Data.IBqlTable
	{
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [GL.Branch()]
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
		[PXDefault(typeof(ARStatement.customerID))]
		[PXUIField(DisplayName = "Customer ID")]
		[PXParent(typeof(Select<ARStatement,Where<ARStatement.customerID,Equal<Current<ARStatementDetail.customerID>>,
								And<ARStatement.statementDate,Equal<Current<ARStatementDetail.statementDate>>,
								And<ARStatement.curyID,Equal<Current<ARStatementDetail.curyID>>>>>>))]
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

		#region StatementDate
		public abstract class statementDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StatementDate;
		[PXDBDate(IsKey = true)]
		[PXDefault(typeof(ARStatement.statementDate))]
		[PXUIField(DisplayName = "Statement Date")]
		public virtual DateTime? StatementDate
		{
			get
			{
				return this._StatementDate;
			}
			set
			{
				this._StatementDate = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(ARStatement.curyID))]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Currency ID")]
		public virtual String CuryID
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "DocType")]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Ref. Nbr.")]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region DocBalance
		public abstract class docBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBalance;
		[PXDBDecimal(4)]
		[PXUIField(DisplayName = "Doc. Balance")]
		public virtual Decimal? DocBalance
		{
			get
			{
				return this._DocBalance;
			}
			set
			{
				this._DocBalance = value;
			}
		}
		#endregion
		#region CuryDocBalance
		public abstract class curyDocBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBalance;
		[PXDBDecimal(4)]
		[PXUIField(DisplayName = "Cury. Doc. Balance")]
		public virtual Decimal? CuryDocBalance
		{
			get
			{
				return this._CuryDocBalance;
			}
			set
			{
				this._CuryDocBalance = value;
			}
		}
		#endregion
		#region OpenDoc
		public abstract class isOpen : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsOpen;
		[PXDBBool()]
		[PXDefault(true)]
		public virtual Boolean? IsOpen
		{
			get
			{
				return this._IsOpen;
			}
			set
			{
				this._IsOpen = value;
			}
		}
		#endregion
		
	}
}
