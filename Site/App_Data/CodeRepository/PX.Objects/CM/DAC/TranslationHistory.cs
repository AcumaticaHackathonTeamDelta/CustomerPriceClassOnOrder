using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.CM
{
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(TranslationHistoryMaint))]
	[PXCacheName(Messages.TranslationHistory)]
	public partial class TranslationHistory : PX.Data.IBqlTable
	{
		#region ReferenceNbr
		public abstract class referenceNbr : PX.Data.IBqlField
		{
		}
		protected String _ReferenceNbr;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Translation Number", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(TranslationHistory.referenceNbr))]
		[AutoNumber(typeof(CMSetup.translNumberingID),typeof(TranslationHistory.dateEntered))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String ReferenceNbr
		{
			get
			{
				return this._ReferenceNbr;
			}
			set
			{
				this._ReferenceNbr = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible, Enabled = false, Required = true)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
   		#region TranslDefId
		public abstract class translDefId : PX.Data.IBqlField
		{
		}
		protected String _TranslDefId;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Translation ID", Enabled = false)]
        [PXSelector(typeof(TranslDef.translDefId))]
		public virtual String TranslDefId
		{
			get
			{
				return this._TranslDefId;
			}
			set
			{
				this._TranslDefId = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(Enabled=false, Required = false)]
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
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		protected Int32? _LedgerID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Destination Ledger ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(Search<Ledger.ledgerID, Where<Ledger.balanceType, NotEqual<BudgetLedger>>>),
			//						new Type[] { typeof(Ledger.ledgerCD), typeof(Ledger.descr) },
			//						new string[] { "Ledger ID", "Description" }, 
						SubstituteKey = typeof(Ledger.ledgerCD))]
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
		#region DestCuryID
		public abstract class destCuryID : PX.Data.IBqlField
		{
		}
		protected String _DestCuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXUIField(DisplayName = "Destination Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String DestCuryID
		{
			get
			{
				return this._DestCuryID;
			}
			set
			{
				this._DestCuryID = value;
			}
		}
		#endregion
		#region DateEntered
		public abstract class dateEntered : PX.Data.IBqlField
		{
		}
		protected DateTime? _DateEntered;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Translation Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? DateEntered
		{
			get
			{
				return this._DateEntered;
			}
			set
			{
				this._DateEntered = value;
			}
		}
		#endregion		
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[ClosedPeriod(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(TranslationStatus.Balanced)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[TranslationStatus.List()]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleCM>>>))]
		[PXUIField(DisplayName="Translation Batch Number", Visibility=PXUIVisibility.Visible, Enabled =false)]
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
		/*
		#region ReversedBatchNbr
		public abstract class reversedBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _ReversedBatchNbr;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Voiding Batch Number", Enabled = false)]
			[PXSelector(typeof(Batch.batchNbr))]
			public virtual String ReversedBatchNbr
		{
			  get
			  {
				  return this._ReversedBatchNbr;
			  }
			  set
			  {
				  this._ReversedBatchNbr = value;
			  }
		}
		#endregion
		*/ 
		#region CuryEffDate
		public abstract class curyEffDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CuryEffDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Currency Effective Date", Enabled = false)]
		public virtual DateTime? CuryEffDate
		{
			get
			{
				return this._CuryEffDate;
			}
			set
			{
				this._CuryEffDate = value;
			}
		}
		#endregion
		#region DebitTot
		public abstract class debitTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _DebitTot;
		[PXUIField(DisplayName="Debit Total", Enabled=false)]
		[PXDBBaseCury(typeof(TranslationHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DebitTot
		{
			get
			{
				return this._DebitTot;
			}
			set
			{
				this._DebitTot = value;
			}
		}
		#endregion
		#region CreditTot
		public abstract class creditTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _CreditTot;
		[PXUIField(DisplayName = "Credit Total", Enabled = false)]
		[PXDBBaseCury(typeof(TranslationHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CreditTot
		{
			get
			{
				return this._CreditTot;
			}
			set
			{
				this._CreditTot = value;
			}
		}
		#endregion
		#region ControlTot
		public abstract class controlTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlTot;
		[PXUIField(DisplayName = "Control Total", Enabled = true, Required = true)]
		[PXDBBaseCury(typeof(TranslationHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ControlTot
		{
			get
			{
				return this._ControlTot;
			}
			set
			{
				this._ControlTot = value;
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(TranslationHistory.referenceNbr))]
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
	}

	public class TranslationStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { Hold, Balanced, Released, Voided },
			new string[] { Messages.Hold, Messages.Balanced, Messages.Released, Messages.Voided }) { }
		}

		public const string Hold	 = "H";
		public const string Balanced = "U";
		public const string Released = "R";
		public const string Voided	 = "V";

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

		public class voided : Constant<string>
		{
			public voided() : base(Voided) { ;}
		}
	}

}
