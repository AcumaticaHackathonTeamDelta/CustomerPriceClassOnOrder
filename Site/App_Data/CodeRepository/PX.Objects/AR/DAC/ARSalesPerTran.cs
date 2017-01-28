namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.TX;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARSalesPerTran)]
	public partial class ARSalesPerTran : PX.Data.IBqlTable
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault(typeof(ARRegister.docType))]
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
		[PXDefault(typeof(ARRegister.refNbr))]
		[PXParent(typeof(Select<ARRegister, Where<ARRegister.docType, Equal<Current<ARSalesPerTran.docType>>,
						 And<ARRegister.refNbr, Equal<Current<ARSalesPerTran.refNbr>>>>>))]
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
		#region SalespersonID
		public abstract class salespersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalespersonID;

		/*[PXDefault(typeof(Search<CustSalesPeople.salesPersonID, Where<CustSalesPeople.bAccountID, Equal<Current<ARInvoice.customerID>>,
				And<CustSalesPeople.locationID, Equal<Current<ARInvoice.customerLocationID>>>>>))]*/
		[PXDBInt(IsKey = true)]
		/*[SalesPerson(typeof(Where<CustSalesPeople.bAccountID, Equal<Current<ARRegister.customerID>>,
			And<CustSalesPeople.locationID,Equal<Current<ARRegister.customerLocationID>>>>), 
			typeof(InnerJoin<CustSalesPeople,On<CustSalesPeople.salesPersonID,Equal<SalesPerson.salesPersonID>>>),DirtyRead=true,Enabled =true,IsKey=true,DescriptionField =typeof(CS.Contact.displayName))]*/
		public virtual Int32? SalespersonID
		{
			get
			{
				return this._SalespersonID;
			}
			set
			{
				this._SalespersonID = value;
			}
		}
		#endregion
		#region RefCntr
		public abstract class refCntr : IBqlField
		{
		}
		protected Int32? _RefCntr;
		[PXDBInt]
		[PXDefault(0)]
		public virtual Int32? RefCntr
		{
			get
			{
				return this._RefCntr;
			}
			set
			{
				this._RefCntr = value;
			}
		}
		#endregion
		#region AdjNbr
		public abstract class adjNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault(0)]
		public virtual Int32? AdjNbr
		{
			get
			{
				return this._AdjNbr;
			}
			set
			{
				this._AdjNbr = value;
			}
		}
		#endregion
		#region AdjdDocType
		public abstract class adjdDocType : PX.Data.IBqlField
		{
		}
		protected String _AdjdDocType;
		[PXDBString(3, IsFixed = true,IsKey=true)]
		[PXDefault(ARDocType.Undefined)]
		public virtual String AdjdDocType
		{
			get
			{
				return this._AdjdDocType;
			}
			set
			{
				this._AdjdDocType = value;
			}
		}
		#endregion
		#region AdjdRefNbr
		public abstract class adjdRefNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjdRefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault("")]
		public virtual String AdjdRefNbr
		{
			get
			{
				return this._AdjdRefNbr;
			}
			set
			{
				this._AdjdRefNbr = value;
			}
		}
		#endregion
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;

        [PXDBInt()]
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(ARRegister.curyInfoID))]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CommnPct
		public abstract class commnPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnPct;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Coalesce<
			Search<CustSalesPeople.commisionPct, Where<CustSalesPeople.bAccountID, Equal<Current<ARRegister.customerID>>,
				And<CustSalesPeople.locationID, Equal<Current<ARRegister.customerLocationID>>,
				And<CustSalesPeople.salesPersonID,Equal<Current<ARSalesPerTran.salespersonID>>>>>>, 
			Search<SalesPerson.commnPct, Where<SalesPerson.salesPersonID, Equal<Current<ARSalesPerTran.salespersonID>>>>>))]
		[PXUIField(DisplayName = "Commission %")]
		public virtual Decimal? CommnPct
		{
			get
			{
				return this._CommnPct;
			}
			set
			{
				this._CommnPct = value;
			}
		}
		#endregion
		#region CuryCommnblAmt
		public abstract class curyCommnblAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCommnblAmt;
		[PXDBCurrency(typeof(ARSalesPerTran.curyInfoID), typeof(ARSalesPerTran.commnblAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Commissionable Amount", Enabled = false)]
		public virtual Decimal? CuryCommnblAmt
		{
			get
			{
				return this._CuryCommnblAmt;
			}
			set
			{
				this._CuryCommnblAmt = value;
			}
		}
		#endregion
		#region CommnblAmt
		public abstract class commnblAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnblAmt;
		[PXDBBaseCury()]
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
		#region CuryCommnAmt
		public abstract class curyCommnAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCommnAmt;
		[PXDBCurrency(typeof(ARSalesPerTran.curyInfoID), typeof(ARSalesPerTran.commnAmt))]
    [PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Commission Amt.",Enabled = false)]
		public virtual Decimal? CuryCommnAmt
		{
			get
			{
				return this._CuryCommnAmt;
			}
			set
			{
				this._CuryCommnAmt = value;
			}
		}
		#endregion
		#region CommnAmt
		public abstract class commnAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnAmt;
		[PXDBBaseCury()]
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
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]		
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
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

		#region ActuallyUsed
		public abstract class actuallyUsed : PX.Data.IBqlField
		{
		}
		protected Boolean? _ActuallyUsed;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? ActuallyUsed
		{
			get
			{
				return this._ActuallyUsed;
			}
			set
			{
				this._ActuallyUsed = value;
			}
		}
		#endregion
		#region CommnPaymntPeriod
		public abstract class commnPaymntPeriod : PX.Data.IBqlField
		{
		}
		protected String _CommnPaymntPeriod;
		[GL.FinPeriodID()]
		public virtual String CommnPaymntPeriod
		{
			get
			{
				return this._CommnPaymntPeriod;
			}
			set
			{
				this._CommnPaymntPeriod = value;
			}
		}
		#endregion
		#region CommnPaymntDate
		public abstract class commnPaymntDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CommnPaymntDate;
		[PXDBDate()]
		public virtual DateTime? CommnPaymntDate
		{
			get
			{
				return this._CommnPaymntDate;
			}
			set
			{
				this._CommnPaymntDate = value;
			}
		}
		#endregion
		
	}
}
