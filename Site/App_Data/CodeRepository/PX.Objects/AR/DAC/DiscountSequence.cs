namespace PX.Objects.AR
{
	using System;
	using PX.Data;
using PX.Objects.IN;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ARDiscountSequenceMaint))]
	[PXCacheName(Messages.DiscountSequence)]
	public partial class DiscountSequence : PX.Data.IBqlTable
	{
		#region DiscountID
		public abstract class discountID : PX.Data.IBqlField
		{
		}
		protected String _DiscountID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault()]
		[PXSelector(typeof(ARDiscount.discountID))]
		[PXUIField(DisplayName = "Discount Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXParent(typeof(Select<ARDiscount, Where<ARDiscount.discountID, Equal<Current<DiscountSequence.discountID>>>>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String DiscountID
		{
			get
			{
				return this._DiscountID;
			}
			set
			{
				this._DiscountID = value;
			}
		}
		#endregion
		#region DiscountSequenceID
		public abstract class discountSequenceID : PX.Data.IBqlField
		{
		}
		protected String _DiscountSequenceID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
		[PXDBDefault(typeof(ARDiscount.lastNumber), DefaultForUpdate = false)]
		[PXUIField(DisplayName="Sequence", Visibility=PXUIVisibility.SelectorVisible, Required = true)]
		[PXSelector(typeof(Search<DiscountSequence.discountSequenceID,
			Where<DiscountSequence.discountID, Equal<Current<DiscountSequence.discountID>>>>),
               typeof(DiscountSequence.discountID),
               typeof(DiscountSequence.discountSequenceID),
               typeof(DiscountSequence.isActive),
               typeof(DiscountSequence.description))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String DiscountSequenceID
		{
			get
			{
				return this._DiscountSequenceID;
			}
			set
			{
				this._DiscountSequenceID = value;
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(250, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DiscountedFor
		public abstract class discountedFor : PX.Data.IBqlField
		{
		}
		protected String _DiscountedFor;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(DiscountOption.Percent)]
		[DiscountOption.List]
		[PXUIField(DisplayName = "Discount by", Visibility = PXUIVisibility.Visible)]
		public virtual String DiscountedFor
		{
			get
			{
				return this._DiscountedFor;
			}
			set
			{
				this._DiscountedFor = value;
			}
		}
		#endregion
		#region BreakBy
		public abstract class breakBy : PX.Data.IBqlField
		{
		}
		protected String _BreakBy;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(BreakdownType.Amount)]
		[BreakdownType.List]
		[PXUIField(DisplayName = "Break by", Visibility = PXUIVisibility.Visible)]
		public virtual String BreakBy
		{
			get
			{
				return this._BreakBy;
			}
			set
			{
				this._BreakBy = value;
			}
		}
		#endregion
		#region IsPromotion
		public abstract class isPromotion : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsPromotion;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Promotional", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? IsPromotion
		{
			get
			{
				return this._IsPromotion;
			}
			set
			{
				this._IsPromotion = value;
			}
		}
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
        #region Prorate
        public abstract class prorate : PX.Data.IBqlField
        {
        }
        protected Boolean? _Prorate;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Prorate Discount", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? Prorate
        {
            get
            {
                return this._Prorate;
            }
            set
            {
                this._Prorate = value;
            }
        }
        #endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXDBDate()]
		[PXUIField(DisplayName = "Effective Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
        #region EndDate
        public abstract class endDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _EndDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Expiration Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual DateTime? EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }
        #endregion
        #region UpdateDate
        public abstract class updateDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _UpdateDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Update Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual DateTime? UpdateDate
        {
            get
            {
                return this._UpdateDate;
            }
            set
            {
                this._UpdateDate = value;
            }
        }
        #endregion
		#region FreeItemID
		public abstract class freeItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _FreeItemID;
		[Inventory(DisplayName="Free Item",Visibility=PXUIVisibility.Visible, Enabled=false) ]
		public virtual Int32? FreeItemID
		{
			get
			{
				return this._FreeItemID;
			}
			set
			{
				this._FreeItemID = value;
			}
		}
		#endregion
		#region PendingFreeItemID
		public abstract class pendingFreeItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _PendingFreeItemID;
		[Inventory(DisplayName = "Pending Free Item", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? PendingFreeItemID
		{
			get
			{
				return this._PendingFreeItemID;
			}
			set
			{
				this._PendingFreeItemID = value;
			}
		}
		#endregion
		#region LastFreeItemID
		public abstract class lastFreeItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _LastFreeItemID;
		[Inventory(DisplayName = "Last Free Item", Visibility = PXUIVisibility.Visible, Enabled=false)]
		public virtual Int32? LastFreeItemID
		{
			get
			{
				return this._LastFreeItemID;
			}
			set
			{
				this._LastFreeItemID = value;
			}
		}
		#endregion
        #region ShowFreeItem
        public abstract class showFreeItem : PX.Data.IBqlField
        {
        }
        protected Boolean? _ShowFreeItem;
        [PXBool()]
        [PXUIField(Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual Boolean? ShowFreeItem
        {
            get
            {
                return this.DiscountedFor == DiscountOption.FreeItem;
            }
            set
            {
            }
        }
        #endregion
		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(DiscountSequence.discountSequenceID),
			Selector = typeof(Search<DiscountSequence.discountSequenceID>))]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region System Columns
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
		#endregion
	}
}
