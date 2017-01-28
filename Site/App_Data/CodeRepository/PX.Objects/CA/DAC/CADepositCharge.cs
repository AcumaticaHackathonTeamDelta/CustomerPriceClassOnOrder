using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.AR;



namespace PX.Objects.CA
{

	[System.SerializableAttribute()]
	[PXCacheName(Messages.CADepositCharge)]
	public partial class CADepositCharge : PX.Data.IBqlTable
	{
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsFixed = true, IsKey = true)]
		[CATranType.DepositList()]
		[PXDefault(typeof(CADeposit.tranType))]
		[PXUIField(DisplayName = "Tran. Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, InputMask = "", IsUnicode = true)]
		[PXDBDefault(typeof(CADeposit.refNbr))]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXParent(typeof(Select<CADeposit, Where<CADeposit.tranType, Equal<Current<CADepositCharge.tranType>>,
									And<CADeposit.refNbr, Equal<Current<CADepositCharge.refNbr>>>>>))]
		
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
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField { };
		protected String _EntryTypeID;
		[PXDBString(10, IsKey = true, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Charge", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId,InnerJoin<CashAccountETDetail,
								On<CashAccountETDetail.entryTypeID,Equal<CAEntryType.entryTypeId>,
								And<CashAccountETDetail.accountID,Equal<Current<CADeposit.cashAccountID>>>>>,
								Where<CAEntryType.module,Equal<GL.BatchModule.moduleCA>,
								And<CAEntryType.useToReclassifyPayments,Equal<False>>>>))]
		public virtual String EntryTypeID
		{
			get
			{
				return this._EntryTypeID;
			}
			set
			{
				this._EntryTypeID = value;
			}
		}
		#endregion
		#region DepositAcctID
		public abstract class depositAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _DepositAcctID;
		[PXDefault(typeof(CADeposit.cashAccountID))]
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID>), IsKey = true, DisplayName = "Clearing Account")]
		public virtual Int32? DepositAcctID
		{
			get
			{
				return this._DepositAcctID;
			}
			set
			{
				this._DepositAcctID = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsKey = true, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Payment Method")]
		[PXSelector(typeof(PaymentMethod.paymentMethodID))]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDefault(typeof(Search<CAEntryType.accountID,Where<CAEntryType.entryTypeId,Equal<Current<CADepositCharge.entryTypeID>>>>))]
		[GL.Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(GL.Account.description))]
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
		[PXDefault(typeof(Search<CAEntryType.subID, Where<CAEntryType.entryTypeId, Equal<Current<CADepositCharge.entryTypeID>>>>))]
		[SubAccount(typeof(CADepositCharge.accountID), DisplayName = "Clearing Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDefault(CADrCr.CACredit)]
		[PXDBString(1, IsFixed = true)]
		[CADrCr.List()]
		[PXUIField(DisplayName = "Disb. / Receipt")]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
			}
		}
		#endregion
		#region ChargeRate
		public abstract class chargeRate: PX.Data.IBqlField { };
		protected Decimal? _ChargeRate;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		[PXUIField(DisplayName = "Charge Rate", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? ChargeRate
		{
			get
			{
				return this._ChargeRate;
			}
			set
			{
				this._ChargeRate = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(CADeposit.curyInfoID))]
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
		#region CuryChargeableAmt
		public abstract class curyChargeableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryChargeableAmt;
		[PXDBCurrency(typeof(CADepositCharge.curyInfoID), typeof(CADepositCharge.chargeableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		
		[PXUIField(DisplayName = "Chargeable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryChargeableAmt
		{
			get
			{
				return this._CuryChargeableAmt;
			}
			set
			{
				this._CuryChargeableAmt = value;
			}
		}
		#endregion
		#region ChargeableAmt
		public abstract class chargeableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ChargeableAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? ChargeableAmt
		{
			get
			{
				return this._ChargeableAmt;
			}
			set
			{
				this._ChargeableAmt = value;
			}
		}
		#endregion	
		#region CuryChargeAmt
		public abstract class curyChargeAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryChargeAmt;
		[PXDBCurrency(typeof(CADepositCharge.curyInfoID), typeof(CADepositCharge.chargeAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<CADepositCharge.curyChargeableAmt, Div<CADepositCharge.chargeRate, CS.decimal100>>), typeof(SumCalc<CADeposit.curyChargeTotal>))]
		//[PXFormula(null, typeof(SumCalc<CADeposit.curyChargeTotal>))]
		[PXUIField(DisplayName="Charge Amount", Visibility=PXUIVisibility.Visible)]
		public virtual Decimal? CuryChargeAmt
		{
			get
			{
				return this._CuryChargeAmt;
			}
			set
			{
				this._CuryChargeAmt = value;
			}
		}
		#endregion
		#region ChargeAmt
		public abstract class chargeAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ChargeAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Charge Amount")]
		public virtual Decimal? ChargeAmt
		{
			get
			{
				return this._ChargeAmt;
			}
			set
			{
				this._ChargeAmt = value;
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

