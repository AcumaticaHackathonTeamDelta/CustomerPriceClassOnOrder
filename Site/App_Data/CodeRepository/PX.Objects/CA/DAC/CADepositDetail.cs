using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.CA;
using System.Collections.Generic;



namespace PX.Objects.CA
{
	public class CADepositDetailType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { CheckDeposit, VoidCheckDeposit, CashDeposit, VoidCashDeposit},
				new string[] { "CheckDeposit", "VoidCheckDeposit", "CashDeposit", "VoidCashDeposit"}) { }
		}

		public const string CheckDeposit = "CHD";
		public const string VoidCheckDeposit = "VCD";
		public const string CashDeposit = "CSD";
		public const string VoidCashDeposit = "VSD";		
			
		public class checkDeposit : Constant<string>
		{
			public checkDeposit() : base(CheckDeposit) { ;}
		}
		public class voidCheckDeposit : Constant<string>
		{
			public voidCheckDeposit() : base(VoidCheckDeposit) { ;}
		}
		public class cashDeposit : Constant<string>
		{
			public cashDeposit() : base(CashDeposit) { ;}
		}
		public class voidCashDeposit : Constant<string>
		{
			public voidCashDeposit() : base(VoidCashDeposit) { ;}
		}		
	}

	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.CADepositDetail)]
	public partial class CADepositDetail : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected Boolean? _Selected;
		[PXBool()]
		[PXUIField(DisplayName = "Selected")]
		public virtual Boolean? Selected
		{
			get
			{
				return this._Selected;
			}
			set
			{
				this._Selected = value;
			}
		}
		#endregion
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
		[PXParent(typeof(Select<CADeposit, Where<CADeposit.tranType, Equal<Current<CADepositDetail.tranType>>,
									And<CADeposit.refNbr, Equal<Current<CADepositDetail.refNbr>>>>>))]
		
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(CADeposit.lineCntr))]
		public virtual Int32? LineNbr
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
		#region DetailType
		public abstract class detailType : PX.Data.IBqlField
		{
		}
		protected String _DetailType;

		[PXDBString(3, IsFixed = true)]
		[CADepositDetailType.List()]
		[PXDefault(CADepositDetailType.CheckDeposit)]		
		[PXUIField(DisplayName = "Detail. Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DetailType
		{
			get
			{
				return this._DetailType;
			}
			set
			{
				this._DetailType = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(GL.BatchModule.AR)]
		[PXUIField(DisplayName = "Doc. Module", Visibility = PXUIVisibility.Visible)]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
		#region OrigDocType
		public abstract class origDocType : PX.Data.IBqlField
		{
		}
		protected String _OrigDocType;
		[PXDBString(3,IsFixed= true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Doc.Type", Visible = true)]	
        [CAAPARTranType.ListByModule(typeof(CADepositDetail.origModule))]
		public virtual String OrigDocType
		{
			get
			{
				return this._OrigDocType;
			}
			set
			{
				this._OrigDocType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Reference Nbr.")]		
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("",PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = "Payment Method",Visible= false )]
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
		[PXDefault()]
		[GL.CashAccount(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(CashAccount.descr))]
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
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.Visible)]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(CADepositDetail.curyInfoID), typeof(CADepositDetail.tranAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<CADeposit.curyDetailTotal>))]
		[PXUIField(DisplayName="Deposit Amount", Visibility=PXUIVisibility.Visible)]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Tran Amount")]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion	
		#region OrigCuryID
		public abstract class origCuryID : PX.Data.IBqlField
		{
		}
		protected String _OrigCuryID;
		[PXDBString(5, InputMask = ">LLLLL", IsUnicode = true)]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CADepositDetail.accountID>>>>))]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String OrigCuryID
		{
			get
			{
				return this._OrigCuryID;
			}
			set
			{
				this._OrigCuryID = value;
			}
		}
		#endregion
		#region OrigCuryInfoID
		public abstract class origCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _OrigCuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(CuryIDField = "OrigCuryID")]
		public virtual Int64? OrigCuryInfoID
		{
			get
			{
				return this._OrigCuryInfoID;
			}
			set
			{
				this._OrigCuryInfoID = value;
			}
		}
		#endregion
		#region CuryOrigAmt
		public abstract class curyOrigAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigAmt;
		[PXDBCurrency(typeof(CADepositDetail.origCuryInfoID), typeof(CADepositDetail.origAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Original Amount", Visible=false )]
		public virtual Decimal? CuryOrigAmt
		{
			get
			{
				return this._CuryOrigAmt;
			}
			set
			{
				this._CuryOrigAmt = value;
			}
		}
		#endregion		
		#region OrigAmt
		public abstract class origAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigAmt
		{
			get
			{
				return this._OrigAmt;
			}
			set
			{
				this._OrigAmt = value;
			}
		}
		#endregion		
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		
		[PXDBLong()]
		[PXUIField(DisplayName = "CA Tran ID")]
		[DepositDetailTranID()]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion		
		#region ChargeEntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _ChargeEntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Charge Type")]
		[PXSelector(typeof(CAEntryType.entryTypeId))]
		public virtual String ChargeEntryTypeID
		{
			get
			{
				return this._ChargeEntryTypeID;
			}
			set
			{
				this._ChargeEntryTypeID = value;
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

		public abstract class origDocSign : IBqlField { }
		public decimal OrigDocSign 
		{
			[PXDependsOnFields(typeof(drCr),typeof(origDocType),typeof(origModule))]
			get
			{
				bool isAP = (this.OrigModule == GL.BatchModule.AP);
				return ((!isAP)?
					((ARPaymentType.DrCr(this.OrigDocType) == CADrCr.CACredit) ? Decimal.MinusOne : Decimal.One) :
					((AP.APPaymentType.DrCr(this.OrigDocType) == CADrCr.CACredit) ? Decimal.MinusOne : Decimal.One));
			}
		}

		public bool IsAP
		{
			[PXDependsOnFields(typeof(origModule))]
			get
			{
				return (this.OrigModule == GL.BatchModule.AP);				
			}
		}
		#region CuryOrigAmtSigned
		[PXDecimal(4)]		
		public virtual Decimal? CuryOrigAmtSigned
		{
			[PXDependsOnFields(typeof(curyOrigAmt),typeof(origDocSign))]
			get
			{
				return this.CuryOrigAmt * this.OrigDocSign;
			}			
		}
		#endregion
		#region OrigAmt
		[PXDecimal(4)]		
		public virtual Decimal? OrigAmtSigned
		{
			[PXDependsOnFields(typeof(origAmt),typeof(origDocSign))]
			get
			{
				return this.OrigAmt * this.OrigDocSign;
			}			
		}
		#endregion		
		
	}

}

