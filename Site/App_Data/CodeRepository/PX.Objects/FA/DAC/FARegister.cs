using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
	
namespace PX.Objects.FA
{
    [Serializable]
	[PXPrimaryGraph(typeof(TransactionEntry))]
	[PXCacheName(Messages.Register)]
	public partial class FARegister : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
        #region BranchID
        public abstract class branchID : IBqlField {}
        [Branch]
        public virtual int? BranchID { get; set; }
        #endregion
        #region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
			#region Numbering
			public class NumberingAttribute : AutoNumberAttribute
			{
				public NumberingAttribute()
					: base(typeof(FASetup.registerNumberingID), typeof(FARegister.docDate)) { ; }
			}
			#endregion
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Number", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<FARegister.refNbr>))]
		[refNbr.Numbering()]
		[PX.Data.EP.PXFieldDescription]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Document Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[PXDBString(6, IsFixed = true)]
		[FinPeriodIDFormatting()]
		[PXUIField(DisplayName = "Period ID")]
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
		#region DocDesc
		public abstract class docDesc : PX.Data.IBqlField
		{
		}
		protected String _DocDesc;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
		public virtual String DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new string[] { Hold, Balanced, Unposted, Posted, Completed },
				new string[] { Messages.Hold, Messages.Balanced, Messages.Unposted, Messages.Posted, Messages.Completed }) { }
			}

			public const string Hold = "H";
			public const string Balanced = "B";
			public const string Unposted = "U";
			public const string Posted = "P";
			public const string Completed = "C";

			public class hold : Constant<string>
			{
				public hold() : base(Hold) { ;}
			}

			public class balanced : Constant<string>
			{
				public balanced() : base(Balanced) { ;}
			}

			public class unposted : Constant<string>
			{
				public unposted() : base(Unposted) { ;}
			}

			public class posted : Constant<string>
			{
				public posted() : base(Posted) { ;}
			}

			public class completed : Constant<string>
			{
				public completed() : base(Completed) { ;}
			}
			#endregion
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(status.Hold)]
		[status.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region Origin
		public abstract class origin : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new [] { Adjustment, Purchasing, Depreciation, Disposal, Transfer, Reconcilliation, Split, Reversal, DisposalReversal},
                new[] { Messages.OriginAdjustment, Messages.OriginPurchasing, Messages.OriginDepreciation, Messages.OriginDisposal, Messages.OriginTransfer, Messages.OriginReconcilliation, Messages.OriginSplit, Messages.OriginReversal, Messages.OriginDisposalReversal }) { }
			}

			public const string Adjustment   = "A";
			public const string Purchasing   = "P";
			public const string Depreciation = "D";
			public const string Disposal     = "S";
			public const string Transfer     = "T";
            public const string Reconcilliation = "R";
		    public const string Split = "I";
            public const string Reversal = "V";
		    public const string DisposalReversal = "L";

			public class  adjustment: Constant<string>
			{
				public adjustment() : base(Adjustment) { }
			}
			public class  purchasing: Constant<string>
			{
				public purchasing() : base(Purchasing) { }
			}
			public class  depreciation: Constant<string>
			{
				public depreciation() : base(Depreciation) { }
			}
			public class  disposal: Constant<string>
			{
				public disposal() : base(Disposal) { }
			}
			public class transfer : Constant<string>
			{
				public transfer() : base(Transfer) { }
			}
            public class reconcilliation : Constant<string>
            {
                public reconcilliation() : base(Reconcilliation) { }
            }
            public class split : Constant<string>
            {
                public split() : base(Split) { }
            }
            public class reversal : Constant<string>
            {
                public reversal() : base(Reversal) { }
            }
            public class disposalReversal : Constant<string>
            {
                public disposalReversal() : base(DisposalReversal) { }
            }
            #endregion
		}
		protected String _Origin;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(origin.Adjustment)]
		[origin.List]
		[PXUIField(DisplayName = "Origin", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Origin
		{
			get
			{
				return _Origin;
			}
			set
			{
				_Origin = value;
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
		[PXUIField(DisplayName = "Released", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "On Hold")]
		public virtual Boolean? Hold
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
		#region Posted
		public abstract class posted : PX.Data.IBqlField
		{
		}
		protected Boolean? _Posted;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Posted
		{
			get
			{
				return this._Posted;
			}
			set
			{
				this._Posted = value;
				this.SetStatus();
			}
		}
		#endregion
        #region Reason
        public abstract class reason : PX.Data.IBqlField
        {
        }
        protected String _Reason;
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Reason", Enabled = false)]
        public virtual String Reason
        {
            get
            {
                return this._Reason;
            }
            set
            {
                this._Reason = value;
            }
        }
        #endregion
        #region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.FA, "{0}", new Type[] { typeof(FARegister.refNbr) },
			new Type[] { typeof(FARegister.docDesc) },
			NumberFields = new Type[] { typeof(FARegister.refNbr) },
			Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(FARegister.docDate), typeof(FARegister.status), typeof(FARegister.origin) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(FARegister.docDesc) }
		)]
		[PXNote(DescriptionField = typeof(FARegister.refNbr), 
			Selector = typeof(FARegister.refNbr))]
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
        #region IsEmpty
        public abstract class isEmpty : PX.Data.IBqlField
        {
        }
        protected bool? _IsEmpty;
        [PXBool]
        [PXUIField(DisplayName = "Empty")]
        [PXDBCalced(typeof(Switch<Case<Where<FARegister.lineCntr, Equal<short0>>, True>, False>), typeof(bool))]
        public virtual bool? IsEmpty
        {
            get
            {
                return this._IsEmpty;
            }
            set
            {
                this._IsEmpty = value;
            }
        }
        #endregion
        #region TranAmt
        public abstract class tranAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _TranAmt;
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Document Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
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

		#region Methods
		private void SetStatus()
		{
			if (this._Hold != null && (bool)this._Hold)
			{
				this._Status = status.Hold;
			}
			else if (this._Released != null && (bool)this._Released == false)
			{
				this._Status = status.Balanced;
			}
			else if (this._Posted != null && (bool)this._Posted == false)
			{
				this._Status = status.Unposted;
			}
			else if (this._Posted != null && (bool)this._Posted)
			{
				this._Status = status.Posted;
			}
		}
		#endregion
	}
}
