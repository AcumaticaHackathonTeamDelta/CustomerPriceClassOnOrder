using PX.Objects.AP;

namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.TX;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.CashAccountETDetail)]
	public partial class CashAccountETDetail : PX.Data.IBqlTable
	{
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(CashAccount.cashAccountID))]
		[PXUIField(DisplayName = "AccountID", Visible = false)]
        [PXParent(typeof(Select<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<CashAccountETDetail.accountID>>>>))]
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
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Entry Type ID")]
		[PXSelector(typeof(CAEntryType.entryTypeId))]
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
		#region OffsetAccountID
		public abstract class offsetAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _OffsetAccountID;
		[Account(DescriptionField = typeof(Account.description), DisplayName = "Offset Account Override")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? OffsetAccountID
		{
			get
			{
				return this._OffsetAccountID;
			}
			set
			{
				this._OffsetAccountID = value;
			}
		}
		#endregion
		#region OffsetSubID
		public abstract class offsetSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _OffsetSubID;
		[SubAccount(typeof(CashAccountETDetail.offsetAccountID),DisplayName = "Offset Subaccount Override")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? OffsetSubID
		{
			get
			{
				return this._OffsetSubID;
			}
			set
			{
				this._OffsetSubID = value;
			}
		}
		#endregion
        #region OffsetBranchID
        public abstract class offsetBranchID : PX.Data.IBqlField
        {
        }
        protected Int32? _OffsetBranchID;
		[Branch(DisplayName = "Offset Account Branch Override", PersistingCheck = PXPersistingCheck.Nothing, Enabled = false)]
        public virtual Int32? OffsetBranchID
        {
            get
            {
                return this._OffsetBranchID;
            }
            set
            {
                this._OffsetBranchID = value;
            }
        }
        #endregion
        #region OffsetCashAccountID
        public abstract class offsetCashAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _OffsetCashAccountID;
        [PXRestrictor(typeof(Where<CashAccount.cashAccountID, NotEqual<Current<CashAccount.cashAccountID>>>), Messages.SetOffsetAccountInSameCurrency)]
        [PXRestrictor(typeof(Where<CashAccount.curyID, Equal<Current<CashAccount.curyID>>>), Messages.SetOffsetAccountInSameCurrency)]
        [CashAccountScalar(DisplayName = "Reclassification Account Override", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
        [PXDBScalar(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.accountID, Equal<CashAccountETDetail.offsetAccountID>,
                                   And<CashAccount.subID, Equal<CashAccountETDetail.offsetSubID>, And<CashAccount.branchID, Equal<CashAccountETDetail.offsetBranchID>>>>>))]
        public virtual Int32? OffsetCashAccountID
        {
            get
            {
                return this._OffsetCashAccountID;
            }
            set
            {
                this._OffsetCashAccountID = value;
            }
        }
        #endregion
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Zone")]
		[PXSelector(typeof(TaxZone.taxZoneID), DescriptionField = typeof(TaxZone.descr), Filterable = true)]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region TaxCalcMode
		public abstract class taxCalcMode : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(VendorClass.taxCalcMode.taxSetting))]
		[VendorClass.taxCalcMode.List]
		[PXUIField(DisplayName = "Tax Calculation Mode")]
		public virtual string TaxCalcMode { get; set; }
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
}
