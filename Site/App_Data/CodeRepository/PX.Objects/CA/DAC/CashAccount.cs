using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;

namespace PX.Objects.CA
{
	[PXCacheName(Messages.CashAccount)]
	[System.SerializableAttribute()]
    [PXPrimaryGraph(
        new Type[] { typeof(CashAccountMaint) },
        new Type[] { typeof(Select<CashAccount, 
            Where<CashAccount.cashAccountID, Equal<Current<CashAccount.cashAccountID>>>>)
        })]
    public partial class CashAccount : PX.Data.IBqlTable
	{
        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public bool? Selected
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
        #region Active
        public abstract class active : PX.Data.IBqlField
        {
        }
        protected Boolean? _Active;
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? Active
        {
            get
            {
                return this._Active;
            }
            set
            {
                this._Active = value;
            }
        }
        #endregion
        #region CashAccountID
        public abstract class cashAccountID : PX.Data.IBqlField
        {
        }
        protected int? _CashAccountID;
        [PXDBIdentity()]
        [PXUIField(Enabled = false)]
        public virtual int? CashAccountID
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
        #region CashAccountCD
        public abstract class cashAccountCD : PX.Data.IBqlField
        {
        }
        protected string _CashAccountCD;
        [CashAccountRaw(IsKey = true, Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault]
        public virtual string CashAccountCD
        {
            get
            {
                return this._CashAccountCD;
            }
            set
            {
                this._CashAccountCD = value;
            }
        }
        #endregion
        #region AccountID
        public abstract class accountID : PX.Data.IBqlField
        {
        }
        protected int? _AccountID;
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[Account(Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? AccountID
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
        protected Int32? _BranchID;
		[Branch(Visibility = PXUIVisibility.SelectorVisible)]
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
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[SubAccount(typeof(CashAccount.accountID), DisplayName = "Subaccount", DescriptionField = typeof(Sub.description), 
            Required = true, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility=PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
        #region CuryID
        public abstract class curyID : PX.Data.IBqlField
        {
        }
        protected string _CuryID;
        [PXDBString(5, IsUnicode = true)]
        [PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, Required = true)]
        [PXSelector(typeof(CM.Currency.curyID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CuryID
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
		#region CuryRateTypeID
		public abstract class curyRateTypeID : PX.Data.IBqlField
		{
		}
		protected String _CuryRateTypeID;
		[PXDBString(6, IsUnicode = true)]
		[PXSelector(typeof(CM.CurrencyRateType.curyRateTypeID))]
		[PXUIField(DisplayName = "Curr. Rate Type ")]
		public virtual String CuryRateTypeID
		{
			get
			{
				return this._CuryRateTypeID;
			}
			set
			{
				this._CuryRateTypeID = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "External Ref. Number",Visibility=PXUIVisibility.SelectorVisible)]
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
        #region NoteID
        public abstract class noteID : IBqlField { }
        [PXNote(DescriptionField = typeof(CashAccount.cashAccountCD))]
		public virtual Guid? NoteID { get; set; }
        #endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CashCreatedByID;
		[PXDBCreatedByID(BqlField = typeof(CashAccount.createdByID))]
		public virtual Guid? CashCreatedByID
		{
			get
			{
				return this._CashCreatedByID;
			}
			set
			{
				this._CashCreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CashCreatedByScreenID;
		[PXDBCreatedByScreenID(BqlField = typeof(CashAccount.createdByScreenID))]
		public virtual String CashCreatedByScreenID
		{
			get
			{
				return this._CashCreatedByScreenID;
			}
			set
			{
				this._CashCreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CashCreatedDateTime;
		[PXDBCreatedDateTime(BqlField = typeof(CashAccount.createdDateTime))]
		public virtual DateTime? CashCreatedDateTime
		{
			get
			{
				return this._CashCreatedDateTime;
			}
			set
			{
				this._CashCreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CashLastModifiedByID;
		[PXDBLastModifiedByID(BqlField = typeof(CashAccount.lastModifiedByID))]
		public virtual Guid? CashLastModifiedByID
		{
			get
			{
				return this._CashLastModifiedByID;
			}
			set
			{
				this._CashLastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CashLastModifiedByScreenID;
		[PXDBLastModifiedByScreenID(BqlField = typeof(CashAccount.lastModifiedByScreenID))]
		public virtual String CashLastModifiedByScreenID
		{
			get
			{
				return this._CashLastModifiedByScreenID;
			}
			set
			{
				this._CashLastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CashLastModifiedDateTime;
		[PXDBLastModifiedDateTime(BqlField = typeof(CashAccount.lastModifiedDateTime))]
		public virtual DateTime? CashLastModifiedDateTime
		{
			get
			{
				return this._CashLastModifiedDateTime;
			}
			set
			{
				this._CashLastModifiedDateTime = value;
			}
		}
		#endregion
		#region Reconcile
		public abstract class reconcile : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reconcile;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Requires Reconciliation")]
		public virtual Boolean? Reconcile
		{
			get
			{
				return this._Reconcile;
			}
			set
			{
				this._Reconcile = value;
			}
		}
		#endregion
		#region ReferenceID
		public abstract class referenceID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReferenceID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[Vendor(DescriptionField = typeof(Vendor.acctName), DisplayName = "Bank ID")]
		[PXUIField(DisplayName = "Bank ID")]
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
        #region ReconNumberingID
        public abstract class reconNumberingID : PX.Data.IBqlField
        {
        }
        protected String _ReconNumberingID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Numbering.numberingID),
                     DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Reconciliation Numbering Sequence", Required = false)]
        public virtual String ReconNumberingID
        {
            get
            {
                return this._ReconNumberingID;
            }
            set
            {
                this._ReconNumberingID = value;
            }
        }
        #endregion
		#region ClearingAccount
		public abstract class clearingAccount : PX.Data.IBqlField
		{
		}
		protected Boolean? _ClearingAccount;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Clearing Account")]
		public virtual Boolean? ClearingAccount
		{
			get
			{
				return this._ClearingAccount;
			}
			set
			{
				this._ClearingAccount = value;
			}
		}
		#endregion
        #region Signature
        public abstract class signature : PX.Data.IBqlField
        {
        }
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Signature")]
        public virtual string Signature { get; set; }
        #endregion
        #region SignatureDescr
        public abstract class signatureDescr : PX.Data.IBqlField
        {
        }
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Name")]
        public virtual string SignatureDescr { get; set; }
        #endregion
		#region StatementImportTypeName
		public abstract class statementImportTypeName : PX.Data.IBqlField
		{
		}
		protected String _StatementImportTypeName;
		[PXDBString(255)]
		[PXUIField(DisplayName = "Statement Import Service")]
		[CA.PXProviderTypeSelector(typeof(IStatementReader))]
		public virtual String StatementImportTypeName
		{
			get
			{
				return this._StatementImportTypeName;
			}
			set
			{
				this._StatementImportTypeName = value;
			}
		}
		#endregion
        #region RestrictVisibilityWithBranch
        public abstract class restrictVisibilityWithBranch : PX.Data.IBqlField
        {
        }
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Restrict Visibility with Branch")]
        public virtual bool? RestrictVisibilityWithBranch { get; set; }
        #endregion
        #region PTInstanceAllowed
        public abstract class pTInstancesAllowed : PX.Data.IBqlField
        {
        }
        protected Boolean? _PTInstancesAllowed;
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Cards Allowed", Visible = false, Enabled = false)]
        public virtual Boolean? PTInstancesAllowed
        {
            get
            {
                return this._PTInstancesAllowed;
            }
            set
            {
                this._PTInstancesAllowed = value;
            }
        }
        #endregion
        #region AcctSettingsAllowed
        public abstract class acctSettingsAllowed : PX.Data.IBqlField
        {
        }
        protected Boolean? _AcctSettingsAllowed;
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Account Settings Allowed", Visible = false, Enabled = false)]
        public virtual Boolean? AcctSettingsAllowed
        {
            get
            {
                return this._AcctSettingsAllowed;
            }
            set
            {
                this._AcctSettingsAllowed = value;
            }
        }
        #endregion
		#region MatchToBatch
		public abstract class matchToBatch : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Match Bank Transactions to Batch Payments")]
		public virtual bool? MatchToBatch { get; set; }
		#endregion
    }
}
