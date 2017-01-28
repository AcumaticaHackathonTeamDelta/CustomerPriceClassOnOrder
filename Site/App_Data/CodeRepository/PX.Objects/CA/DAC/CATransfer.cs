using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.CA
{
	[PXCacheName(Messages.CATransfer)]
	[Serializable]
	public partial class CATransfer : PX.Data.IBqlTable, ICADocument
	{
		public string DocType
		{
			get
			{
				return CAAPARTranType.CATransfer;
			}
		}

		public string RefNbr
		{
			get
			{
				return TransferNbr;
			}
		}
		#region TransferNbr
		public abstract class transferNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault]
		[PXUIField(DisplayName = "Transfer Number", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(CATransfer.transferNbr))]
		[AutoNumber(typeof(CASetup.transferNumberingID), typeof(CATransfer.inDate))]
		public virtual string TransferNbr
		{
			get;
			set;
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual string Descr
		{
			get;
			set;
		}
		#endregion
		#region OutAccountID
		public abstract class outAccountID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[GL.CashAccount(DisplayName = "Account", DescriptionField = typeof(CashAccount.descr))]
		public virtual int? OutAccountID
		{
			get;
			set;
		}
		#endregion
		#region InAccountID
		public abstract class inAccountID : PX.Data.IBqlField
		{
		}
		[PXDefault]
		[GL.CashAccount(DisplayName = "Account", DescriptionField = typeof(CashAccount.descr))]
		public virtual int? InAccountID
		{
			get;
			set;
		}
		#endregion
		#region OutCuryInfoID
		public abstract class outCuryInfoID : PX.Data.IBqlField
		{
		}
		[PXDBLong]
		[CurrencyInfo(ModuleCode = "CA", CuryIDField = "OutCuryID", CuryRateField = "OutCuryRate")]
		public virtual long? OutCuryInfoID
		{
			get;
			set;
		}
		#endregion
		#region InCuryInfoID
		public abstract class inCuryInfoID : PX.Data.IBqlField
		{
		}
		[PXDBLong]
		[CurrencyInfo(ModuleCode = "CA", CuryIDField = "InCuryID", CuryRateField = "InCuryRate")]
		public virtual long? InCuryInfoID
		{
			get;
			set;
		}
		#endregion
		#region InCuryID
		public abstract class inCuryID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CATransfer.inAccountID>>>>))]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string InCuryID
		{
			get;
			set;
		}
		#endregion
		#region OutCuryID
		public abstract class outCuryID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CATransfer.outAccountID>>>>))]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string OutCuryID
		{
			get;
			set;
		}
		#endregion
		#region CuryTranOut
		public abstract class curyTranOut : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
		[PXDBCurrency(typeof(CATransfer.outCuryInfoID), typeof(CATransfer.tranOut))]
		public virtual decimal? CuryTranOut
		{
			get;
			set;
		}
		#endregion
		#region CuryTranIn
		public abstract class curyTranIn : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
		[PXDBCurrency(typeof(CATransfer.inCuryInfoID), typeof(CATransfer.tranIn))]
		public virtual decimal? CuryTranIn
		{
			get;
			set;
		}
		#endregion
		#region TranOut
		public abstract class tranOut : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Base Currency Amount", Enabled = false)]
		public virtual decimal? TranOut
		{
			get;
			set;
		}
		#endregion
		#region TranIn
		public abstract class tranIn : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Base Currency Amount", Enabled = false)]
		public virtual decimal? TranIn
		{
			get;
			set;
		}
		#endregion
		#region InDate
		public abstract class inDate : PX.Data.IBqlField
		{
		}
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Receipt Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? InDate
		{
			get;
			set;
		}
		#endregion
		#region OutDate
		public abstract class outDate : PX.Data.IBqlField
		{
		}
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Transfer Date")]
		public virtual DateTime? OutDate
		{
			get;
			set;
		}
		#endregion
		#region OutExtRefNbr
		public abstract class outExtRefNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(40, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Document Ref.")]
		public virtual string OutExtRefNbr
		{
			get;
			set;
		}
		#endregion
		#region InExtRefNbr
		public abstract class inExtRefNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(40, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Document Ref.")]
		public virtual string InExtRefNbr
		{
			get;
			set;
		}
		#endregion
		#region TranIDOut
		public abstract class tranIDOut : PX.Data.IBqlField
		{
		}
		[PXDBLong]
		[TransferCashTranID]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
		public virtual long? TranIDOut
		{
			get;
			set;
		}
		#endregion
		#region TranIDIn
		public abstract class tranIDIn : PX.Data.IBqlField
		{
		}
		[PXDBLong]
		[TransferCashTranID]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
		public virtual long? TranIDIn
		{
			get;
			set;
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		[PXDBInt]
		[PXDefault(0)]
		public virtual int? LineCntr
		{
			get;
			set;
		}
		#endregion
		#region RGOLAmt
		public abstract class rGOLAmt : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "RGOL", Enabled = false)]
		public virtual decimal? RGOLAmt
		{
			get;
			set;
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool]
		[PXDefault(typeof(Search<CASetup.holdEntry>))]
		[PXUIField(DisplayName = "Hold")]
		public virtual bool? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? Released
		{
			get
			{
				return _Released;
			}
			set
			{
				this._Released = value;
				this.SetStatus();
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		[PXSearchable(SM.SearchCategory.CA, Messages.SearchTitleCATransfer, new Type[] { typeof(CATransfer.transferNbr) },
			new Type[] { typeof(CATransfer.descr), typeof(CATransfer.outExtRefNbr), typeof(CATransfer.inExtRefNbr) },
			NumberFields = new Type[] { typeof(CATransfer.transferNbr) },
			Line1Format = "{0}{1}", Line1Fields = new Type[] { typeof(CATransfer.outExtRefNbr), typeof(CATransfer.inExtRefNbr) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(CATransfer.descr) }
		)]
		[PXNote(DescriptionField = typeof(CATransfer.transferNbr))]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		[PXDBTimestamp]
		public virtual byte[] tstamp
		{
			get;
			set;
		}
		#endregion
		#region Status
		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[CATransferStatus.List]
		public virtual string Status
		{
			[PXDependsOnFields(typeof(released), typeof(hold))]
			get;
			set;
		}
		#endregion
		#region ClearedOut
		public abstract class clearedOut : PX.Data.IBqlField
		{
		}
		[PXDBBool]
		[PXUIField(DisplayName = "Cleared")]
		[PXDefault(false)]
		public virtual bool? ClearedOut
		{
			get;
			set;
		}
		#endregion
		#region ClearDateOut
		public abstract class clearDateOut : PX.Data.IBqlField
		{
		}
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date", Required = false)]
		public virtual DateTime? ClearDateOut
		{
			get;
			set;
		}
		#endregion
		#region ClearedIn
		public abstract class clearedIn : PX.Data.IBqlField
		{
		}
		[PXDBBool]
		[PXUIField(DisplayName = "Cleared")]
		[PXDefault(false)]
		public virtual bool? ClearedIn
		{
			get;
			set;
		}
		#endregion
		#region ClearDateIn
		public abstract class clearDateIn : PX.Data.IBqlField
		{
		}
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date", Required = false)]
		public virtual DateTime? ClearDateIn
		{
			get;
			set;
		}
		#endregion
		#region CashBalanceIn
		public abstract class cashBalanceIn : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCury(typeof(CATransfer.inCuryID))]
		[PXUIField(DisplayName = "Available Balance", Enabled = false)]
		[CashBalance(typeof(CATransfer.inAccountID))]
		public virtual decimal? CashBalanceIn
		{
			get;
			set;
		}
		#endregion
		#region CashBalanceOut
		public abstract class cashBalanceOut : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCury(typeof(CATransfer.outCuryID))]
		[PXUIField(DisplayName = "Available Balance", Enabled = false)]
		[CashBalance(typeof(CATransfer.outAccountID))]
		public virtual decimal? CashBalanceOut
		{
			get;
			set;
		}
		#endregion
		#region InGLBalance
		public abstract class inGLBalance : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCury(typeof(CATransfer.inCuryID))]
		[PXUIField(DisplayName = "GL Balance", Enabled = false)]
		[GLBalance(typeof(CATransfer.inAccountID), null, typeof(CATransfer.inDate))]
		public virtual decimal? InGLBalance
		{
			get;
			set;
		}
		#endregion
		#region OutGLBalance
		public abstract class outGLBalance : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCury(typeof(CATransfer.outCuryID))]
		[PXUIField(DisplayName = "GL Balance", Enabled = false)]
		[GLBalance(typeof(CATransfer.outAccountID), null, typeof(CATransfer.outDate))]
		public virtual decimal? OutGLBalance
		{
			get;
			set;
		}
		#endregion
		#region TranIDOut_CATran_batchNbr
		public abstract class tranIDOut_CATran_batchNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranIDIn_CATran_batchNbr
		public abstract class tranIDIn_CATran_batchNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region Methods
		public virtual void SetStatus()
		{
			if (Released == true)
			{
				Status = CATransferStatus.Released;
			}
			else if (Hold == true)
			{
				Status = CATransferStatus.Hold;
			}
			else
			{
				Status = CATransferStatus.Balanced;
			}
		}
		#endregion
	}

	public class caCredit : Constant<string>
	{
		public caCredit() : base(DrCr.Credit) { }
	}

	public class CATransferStatus
	{
		public const string Balanced = "B";
		public const string Hold = "H";
		public const string Released = "R";
		public const string Rejected = "J";
		public const string Pending = "P";

		public static readonly string[] Values =
		{
			Balanced,
			Hold,
			Released,
			Pending,
			Rejected
		};

		public static readonly string[] Labels =
		{
			Messages.Balanced,
			Messages.Hold,
			Messages.Released,
			Messages.Pending,
			Messages.Rejected
		};

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(Values, Labels)
			{
			}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { }
		}

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { }
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { }
		}

		public class rejected : Constant<string>
		{
			public rejected() : base(Rejected) { }
		}

		public class pending : Constant<string>
		{
			public pending() : base(Pending) { }
		}
	}
}
