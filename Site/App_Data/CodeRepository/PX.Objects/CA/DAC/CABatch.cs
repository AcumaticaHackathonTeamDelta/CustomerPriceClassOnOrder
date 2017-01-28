using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.TM;


namespace PX.Objects.CA
{
	[PXPrimaryGraph(typeof(CABatchEntry))]
	[PXCacheName(Messages.CABatch)]
    [Serializable]
	public partial class CABatch: IBqlTable
	{
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[CABatchType.Numbering()]
		[CABatchType.RefNbr(typeof(Search<CABatch.batchNbr>))]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Payment Method")]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID,Where<PaymentMethod.aPCreateBatchPayment,Equal<True>>>))]
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
		#region ReferenceID
		public abstract class referenceID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReferenceID;
		[PXDBInt()]
		[PXDefault(typeof(Search<CashAccount.referenceID,Where<CashAccount.cashAccountID,Equal<Current<CABatch.cashAccountID>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(BAccount.bAccountID),
						SubstituteKey = typeof(BAccount.acctCD),
					 DescriptionField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Bank", Visibility = PXUIVisibility.Visible,Enabled=false)]
		public virtual Int32? ReferenceID
		{
			get
			{
				return this._ReferenceID;
			}
			set
			{
				this._ReferenceID = value;
			}
		}
		#endregion
		#region BatchSeqNbr
		public abstract class batchSeqNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchSeqNbr;
		[PXDBString(15, IsUnicode = true)]		
        [AP.BatchRef(typeof(CABatch.cashAccountID), typeof(CABatch.paymentMethodID))]
		[PXUIField(DisplayName = "Batch Seq. Number", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String BatchSeqNbr
		{
			get
			{
				return this._BatchSeqNbr;
			}
			set
			{
				this._BatchSeqNbr = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Document Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ExtRefNbr
		{
			get
			{
				return this._ExtRefNbr;
			}
			set
			{
				this._ExtRefNbr = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Batch Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DateSeqNbr
		public abstract class dateSeqNbr : PX.Data.IBqlField
		{
		}
		protected Int16? _DateSeqNbr;
		[PXDBShort()]
		[PXDefault((short)0)]
        [PXUIField(DisplayName = "Seq. Number Within Day", Enabled = false)]
		public virtual Int16? DateSeqNbr
		{
			get
			{
				return this._DateSeqNbr;
			}
			set
			{
				this._DateSeqNbr = value;
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(Search<CASetup.holdEntry>))]
		[PXUIField(DisplayName = "Hold")]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
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
		[PXUIField(DisplayName="Released",Enabled = true)]
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
		#region Status
		protected String _Status;
		[PXString(1, IsFixed = true)]
		[PXDefault(CABatchStatus.Balanced, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[CADepositStatus.List()]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(hold),typeof(released))]
			get
			{
				if (Hold.HasValue && Hold == true)
				{
					_Status = CADepositStatus.Hold;
				}
				else
				{
					if (Released.HasValue && Released == true)
					{
						_Status = CADepositStatus.Released;
					}
					else
					{
						_Status = CADepositStatus.Balanced;
					}
				}
				return this._Status;
			}
			set
			{

			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
        [PXString(5, IsUnicode = true)]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXDBScalar(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<CABatch.cashAccountID>>>))]
        [PXUIField(DisplayName = "Currency")]
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
		#region CuryDetailTotal
		public abstract class curyDetailTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDetailTotal;
		[PXDBCury(typeof(CABatch.curyID))]
		[PXUIField(DisplayName = "Batch Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDetailTotal
		{
			get
			{
				return this._CuryDetailTotal;
			}
			set
			{
				this._CuryDetailTotal = value;
			}
		}
		#endregion
		#region DetailTotal
		public abstract class detailTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DetailTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DetailTotal
		{
			get
			{
				return this._DetailTotal;
			}
			set
			{
				this._DetailTotal = value;
			}
		}
		#endregion
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cleared",Visible=false)]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
			}
		}
		#endregion
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date",Visible = false)]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.Visible)]
		public virtual int? WorkgroupID
		{
			get
			{
				return this._WorkgroupID;
			}
			set
			{
				this._WorkgroupID = value;
			}
		}
		#endregion
		#region OwnerID
		public abstract class ownerID : IBqlField { }
		protected Guid? _OwnerID;
		[PXDBGuid()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXOwnerSelector(typeof(CADeposit.workgroupID))]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Guid? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.AP, Messages.SearchTitleBatchPayment, new Type[] { typeof(CABatch.batchNbr), typeof(CABatch.referenceID), typeof(BAccount.acctName) },
			new Type[] { typeof(CABatch.paymentMethodID), typeof(CABatch.batchSeqNbr), typeof(CABatch.tranDesc), typeof(CABatch.extRefNbr), typeof(BAccount.acctCD) },
			NumberFields = new Type[] { typeof(CABatch.batchNbr) },
			Line1Format = "{0}{1:d}{2}", Line1Fields = new Type[] { typeof(CABatch.extRefNbr), typeof(CABatch.tranDate), typeof(CABatch.batchSeqNbr) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(CABatch.tranDesc) }
		)]
		[PXNote(new Type[0])]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region ExportFileName
		public abstract class exportFileName : PX.Data.IBqlField
		{
		}
		protected String _ExportFileName;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Exported File Name", Visibility = PXUIVisibility.SelectorVisible,Enabled =false)]
		public virtual String ExportFileName
		{
			get
			{
				return this._ExportFileName;
			}
			set
			{
				this._ExportFileName = value;
			}
		}
		#endregion
		#region ExportTime
		public abstract class exportTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExportTime;
		[PXDBDate(PreserveTime = true)]
		[PXUIField(DisplayName="File Export Time",Enabled=false)]
		public virtual DateTime? ExportTime
		{
			get
			{
				return this._ExportTime;
			}
			set
			{
				this._ExportTime = value;
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
		#region Processing
		public abstract class processing : PX.Data.IBqlField
		{
		}
		protected Boolean? _Processing;
		[PXBool()]
		[PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName="Processing",Enabled = true)]
		public virtual Boolean? Processing
		{
			get
			{
				return this._Processing;
			}
			set
			{
				this._Processing = value;
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
		#region DetailTotal
		public abstract class total : PX.Data.IBqlField
		{
		}
		protected Decimal? _Total;
		[PXDecimal(4)]		
		public virtual Decimal? Total
		{
			get
			{
				return this._Total;
			}
			set
			{
				this._Total = value;
			}
		}
		#endregion
		#region Reconciled
		public abstract class reconciled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reconciled;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Reconciled
		{
			get
			{
				return this._Reconciled;
			}
			set
			{
				this._Reconciled = value;
			}
		}
		#endregion
		#region ReconDate
		public abstract class reconDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReconDate;
		[PXDBDate()]
		public virtual DateTime? ReconDate
		{
			get
			{
				return this._ReconDate;
			}
			set
			{
				this._ReconDate = value;
			}
		}
		#endregion
		#region ReconNbr
		public abstract class reconNbr : PX.Data.IBqlField
		{
		}
		protected String _ReconNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String ReconNbr
		{
			get
			{
				return this._ReconNbr;
			}
			set
			{
				this._ReconNbr = value;
			}
		}
		#endregion
		public CATran CopyTo(CATran destination)
		{
			destination.TranDate = this.TranDate;
			destination.OrigModule = BatchModule.AP;
			destination.OrigRefNbr = this.BatchNbr;
			destination.OrigTranType = CATranType.CABatch;
			destination.ExtRefNbr = this.ExtRefNbr;
			destination.TranDesc = this.TranDesc;
			destination.TranAmt = -this.DetailTotal;
			destination.CuryTranAmt = -this.CuryDetailTotal;
			destination.Released = this.Released;
			destination.Hold = this.Hold;
			destination.Status = this.Status;
			destination.Cleared = this.Cleared;
			destination.ClearDate = this.ClearDate;
			destination.ReconNbr = this.ReconNbr;
			destination.ReconDate = this.ReconDate;
			destination.Reconciled = this.Reconciled;
			destination.DrCr = DrCr.Credit;
			destination.TranDesc = this.TranDesc;
			destination.ExtRefNbr = this.ExtRefNbr;
			destination.CashAccountID = this.CashAccountID;
			destination.CuryID = this.CuryID;
			return destination;
		}
		#region Obsolete
		[Obsolete("Field curyTotal is obsolete. Please use CuryDetailTotal instead.")]
		public abstract class curyTotal : PX.Data.IBqlField
		{
		}
		[PXCury(typeof(CABatch.curyID))]
		[PXUIField(DisplayName = "Batch Total", Visibility = PXUIVisibility.Invisible, Enabled = false)]
		[Obsolete("Field CuryTotal is obsolete. Please use CuryDetailTotal instead.")]
		public virtual Decimal? CuryTotal
		{
			get
			{
				return this._CuryDetailTotal;
			}
			set
			{
				this._CuryDetailTotal = value;
			}
		}
		#endregion

	}

	public class CABatchType
	{
        /// <summary>
        /// Specialized selector for CABatch RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// CABatch.batchNbr, CABatch.tranDate, CABatch.cashAccountID,
		///	CABatch.paymentMethodID, CABatch.curyDetailTotal, CABatch.extRefNbr
        /// <example>
        /// [CABatchType.RefNbr(typeof(Search<CABatch.batchNbr>))]
        /// </example>
        /// </summary>
		public class RefNbrAttribute : PXSelectorAttribute
		{
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(CABatch.batchNbr),
				typeof(CABatch.tranDate),
				typeof(CABatch.cashAccountID),
				typeof(CABatch.paymentMethodID),
				typeof(CABatch.curyDetailTotal),
				typeof(CABatch.extRefNbr))
			{
			}
		}
        /// <summary>
        /// Specialized for CABatch version of the <see cref="AutoNumberAttribute"/><br/>
        /// It defines how the new numbers are generated for the AR Invoice. <br/>
        /// References CABatch.docType and CABatch.tranDate fields of the document,<br/>
        /// and also define a link between  numbering ID's defined in CASetup (namely CASetup.cABatchNumberingID)<br/>
        /// and CABatch: <br/>
        /// </summary>
		public class NumberingAttribute : CS.AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(CASetup.cABatchNumberingID), typeof(CABatch.tranDate)) { }
					
		}

	}

	public class CABatchStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Balanced, Hold, Released,Exported },
				new string[] { Messages.Balanced, Messages.Hold, Messages.Released, Messages.Exported }) { ; }
		}

		public const string Balanced = "B";
		public const string Hold = "H";
		public const string Released = "R";
		public const string Exported = "P";

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

		public class exported : Constant<string>
		{
			public exported() : base(Exported) { ;}
		}

	}
}
