namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CA;
	using PX.Objects.CR;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXEMailSource]
	[PXCacheName(Messages.CustomerPaymentMethod)]
	[PXPrimaryGraph(typeof(CustomerPaymentMethodMaint))]
	public partial class CustomerPaymentMethod : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDefault(typeof(Customer.bAccountID))]
		[Customer(DescriptionField = typeof(Customer.acctName), IsKey = true, DirtyRead = true)]
		[PXParent(typeof(Select<Customer, 
			Where<Customer.bAccountID, Equal<Current<CustomerPaymentMethod.bAccountID>>,
			And<BAccount.type, NotEqual<BAccountType.combinedType>>>>))]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
            /// <summary>
            /// Provides a selector for a Customer Payment Method - for example,<br/>
            /// a list a credit cards that customer has. Customer is taken from the row<br/>
            /// </summary>
			public class PMInstanceIDSelectorAttribute : PXSelectorAttribute
			{
				public PMInstanceIDSelectorAttribute()
					: base(typeof(Search<CustomerPaymentMethod.pMInstanceID, Where<CustomerPaymentMethod.bAccountID, Equal<Current<CustomerPaymentMethod.bAccountID>>>>))
				{
				}
				public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
				{
				}
			}
		}
		protected Int32? _PMInstanceID;
		[PXDBForeignIdentity(typeof(PMInstance), IsKey = true)]
		[pMInstanceID.PMInstanceIDSelector]
		[PXUIField(DisplayName = "Card Number")]
		public virtual Int32? PMInstanceID
		{
			get
			{
				return this._PMInstanceID;
			}
			set
			{
				this._PMInstanceID = value;
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
		[PXUIField(DisplayName = "Payment Method", Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID, Where<PaymentMethod.isActive,Equal<True>,
                            And<PaymentMethod.useForAR,Equal<True>>>>), DescriptionField = typeof(PaymentMethod.descr))]
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
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(null, typeof(Search2<CashAccount.cashAccountID, InnerJoin<PaymentMethodAccount, 
									On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>, 
									And<PaymentMethodAccount.paymentMethodID, Equal<Current<CustomerPaymentMethod.paymentMethodID>>,
									And<PaymentMethodAccount.useForAR,Equal<True>>>>>,
									Where<Match<Current<AccessInfo.userName>>>>), DisplayName = "Cash Account", DescriptionField = typeof(CashAccount.descr), Visibility = PXUIVisibility.Visible)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(255, IsUnicode = true)]
		[PXDefault("",PersistingCheck =PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Card/Account No", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
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
		#region IsDefault
		public abstract class isDefault : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsDefault;
		[PXBool()]
		[PXDefault(false, PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Is Default", Enabled=false)]
		public virtual Boolean? IsDefault
		{
			get
			{
				return this._IsDefault;
			}
			set
			{
				this._IsDefault = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		
		[PXNote(DescriptionField = typeof(CustomerPaymentMethod.descr))]
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
		#region ExpirationDate
		public abstract class expirationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpirationDate;
		[PXDBDateString(DateFormat = "MM/yy")]
		[PXUIField(DisplayName = "Expiration Date", Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public virtual DateTime? ExpirationDate
		{
			get
			{
				return this._ExpirationDate;
			}
			set
			{
				this._ExpirationDate = value;
			}
		}
		#endregion
		#region CVVVerifyTran
		public abstract class cVVVerifyTran : PX.Data.IBqlField
		{
		}
		protected Int32? _CVVVerifyTran;

		[PXDBInt()]
		public virtual Int32? CVVVerifyTran
		{
			get
			{
				return this._CVVVerifyTran;
			}
			set
			{
				this._CVVVerifyTran = value;
			}
		}
		#endregion
        #region BillAddressID
        public abstract class billAddressID : PX.Data.IBqlField
        {
        }

        protected Int32? _BillAddressID;
        [PXDBInt()]
        [PXDBChildIdentity(typeof(Address.addressID))]
        public virtual Int32? BillAddressID
        {
            get
            {
                return this._BillAddressID;
            }
            set
            {
                this._BillAddressID = value;
            }
        }

        #endregion
        #region BillContactID
        public abstract class billContactID : PX.Data.IBqlField
        {
        }

        protected int? _BillContactID;

        [PXDBInt()]
        [PXDBChildIdentity(typeof(Contact.contactID))]        
        public virtual Int32? BillContactID
        {
            get
            {
                return this._BillContactID;
            }
            set
            {
                this._BillContactID = value;
            }
        }
	
        #endregion
        #region ARHasBillingInfo
        public abstract class hasBillingInfo : PX.Data.IBqlField
        {
        }
        protected bool? _HasBillingInfo;
        [PXBool()]
        [PXUIField(DisplayName = "Has Billing Info", Visible= false, Enabled=false)]
        public virtual bool? HasBillingInfo
        {
            get
            {
                return this._HasBillingInfo;
            }
            set
            {
                this._HasBillingInfo = value;
            }
        }
        #endregion
		
        #region IsBillAddressSameAsMain
        public abstract class isBillAddressSameAsMain : PX.Data.IBqlField
        {
        }
        protected bool? _IsBillAddressSameAsMain;
        [PXBool()]
        [PXUIField(DisplayName = "Same as Main")]
        public virtual bool? IsBillAddressSameAsMain
        {
            get
            {
                return this._IsBillAddressSameAsMain;
            }
            set
            {
                this._IsBillAddressSameAsMain = value;
            }
        }
        #endregion
        #region IsBillContSameAsMain
        public abstract class isBillContSameAsMain : PX.Data.IBqlField
        {
        }
        protected bool? _IsBillContactSameAsMain;
        [PXBool()]
        [PXUIField(DisplayName = "Same as Main")]
        public virtual bool? IsBillContactSameAsMain
        {
            get
            {
                return this._IsBillContactSameAsMain;
            }
            set
            {
                this._IsBillContactSameAsMain = value;
            }
        }
        #endregion
		#region CCProcessingCenterID
		public abstract class cCProcessingCenterID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true)]
		[PXDBDefault(typeof(Search2<CCProcessingCenterPmntMethod.processingCenterID, 
			InnerJoin<PaymentMethod, On<CCProcessingCenterPmntMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>,
			Where<CCProcessingCenterPmntMethod.isDefault, Equal<True>, And<PaymentMethod.paymentMethodID, Equal<Current<CustomerPaymentMethod.paymentMethodID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<CCProcessingCenterPmntMethod.processingCenterID, Where<CCProcessingCenterPmntMethod.paymentMethodID, Equal<Current<CustomerPaymentMethod.paymentMethodID>>>>))]
		[PXUIField(DisplayName = "Proc. Center ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, Visible = false)]
		public virtual string CCProcessingCenterID { get; set; }
		#endregion
		#region CustomerCCPID
		public abstract class customerCCPID : PX.Data.IBqlField
		{
		}
		[PXDBString(1024, IsUnicode = true)]
		[PXDefault(typeof(Search<CustomerProcessingCenterID.customerCCPID, 
			Where<CustomerProcessingCenterID.bAccountID, Equal<Current<CustomerPaymentMethod.bAccountID>>,
			And<CustomerProcessingCenterID.cCProcessingCenterID, Equal<Current<CustomerPaymentMethod.cCProcessingCenterID>>>>>), 
			PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<CustomerProcessingCenterID.customerCCPID, Where<CustomerProcessingCenterID.bAccountID, Equal<Current<CustomerPaymentMethod.bAccountID>>,
			And<CustomerProcessingCenterID.cCProcessingCenterID, Equal<Current<CustomerPaymentMethod.cCProcessingCenterID>>>>>), 
			ValidateValue = false)]
		[PXUIField(DisplayName = "Customer Profile ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, Visible = false)]
		public virtual string CustomerCCPID { get; set; }
		#endregion
		#region Converted
		public abstract class converted : PX.Data.IBqlField
		{
		}
		protected Boolean? _Converted;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public virtual Boolean? Converted
		{
			get
			{
				return this._Converted;
			}
			set
			{
				this._Converted = value;
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

		#region LastNotificationDate
		public abstract class lastNotificationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastNotificationDate;
		[PXDBDate(PreserveTime = true)]		
		[PXUIField(DisplayName = "Notification Date")]
		public virtual DateTime? LastNotificationDate
		{
			get
			{
				return this._LastNotificationDate;
			}
			set
			{
				this._LastNotificationDate = value;
			}
		}
		#endregion

		
		
	}

	[Serializable]
	public partial class CustomerPaymentMethodInputMode : PX.Data.IBqlTable
	{
		#region InputMode
		public abstract class inputMode : PX.Data.IBqlField
		{
		}
		protected string _InputMode;
		[PXString(1)]
		[InputModeType.List]
		[PXDefault(InputModeType.Details, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Input Mode", Visible = false, Enabled = true)]
		public virtual string InputMode
		{
			get
			{
				return this._InputMode;
			}
			set
			{
				this._InputMode = value;
			}
		}
		#endregion
		
	}

	public class InputModeType
	{
		public const string Token = "T";
		public const string Details = "D";

		public class token : Constant<string>
		{
			public token() : base(Token) { }
		}

		public class details : Constant<string>
		{
			public details() : base(Details) { }
		}

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { Token, Details }, new string[] { Messages.TokenInputMode, Messages.DetailsInputMode })
			{
			}
		}
	}

	[Serializable]
	[PXProjection(typeof(Select2<PMInstance, LeftJoin<PaymentMethod, On<PMInstance.pMInstanceID, Equal<PaymentMethod.pMInstanceID>>, 
		LeftJoin<CustomerPaymentMethod, On<PMInstance.pMInstanceID, Equal<CustomerPaymentMethod.pMInstanceID>>, 
		LeftJoin<PaymentMethodActive, On<PaymentMethod.paymentMethodID, Equal<PaymentMethodActive.paymentMethodID>,
			Or<CustomerPaymentMethod.paymentMethodID, Equal<PaymentMethodActive.paymentMethodID>>>>>>,
		Where2<Where<PaymentMethod.aRIsOnePerCustomer, Equal<True>, Or<PaymentMethod.aRIsOnePerCustomer, IsNull>>,
		And2<Where<PaymentMethod.useForAR, Equal<True>, Or<PaymentMethod.useForAR, IsNull>>, 
		And<Where<PaymentMethod.pMInstanceID, IsNotNull,Or<CustomerPaymentMethod.pMInstanceID, IsNotNull>>>>>>))]
	public partial class CustomerPaymentMethodInfo : PX.Data.IBqlTable
	{
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(CustomerPaymentMethod.bAccountID))]
		public virtual Int32? BAccountID { get; set; }
		#endregion
		#region IsDefault
		public abstract class isDefault : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXUIField(DisplayName = "Is Default")]
		public virtual Boolean? IsDefault { get; set; }
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		[PXString(10, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDBCalced(typeof(Switch<Case<Where<PaymentMethod.paymentMethodID, IsNotNull>, PaymentMethod.paymentMethodID>, CustomerPaymentMethod.paymentMethodID>), typeof(string))]
		public virtual String PaymentMethodID { get; set; }
		#endregion
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true, BqlField = typeof(PMInstance.pMInstanceID))]
		public virtual Int32? PMInstanceID { get; set; }
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(CustomerPaymentMethod.cashAccountID))]
		[PXSelector(typeof(CashAccount.cashAccountID), SubstituteKey = typeof(CashAccount.cashAccountCD))]
		[PXUIField(DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Int32? CashAccountID { get; set; }
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(255, IsUnicode = true, NonDB = true, BqlField = typeof(PaymentMethod.descr))]
		[PXDefault("", PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDBCalced(typeof(Switch<Case<Where<PaymentMethod.descr, IsNotNull>, PaymentMethod.descr>, CustomerPaymentMethod.descr>), typeof(string))]
		public virtual String Descr { get; set; }
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDBCalced(
			typeof(
				Switch<
				Case<Where<PaymentMethodActive.isActive, Equal<True>>,
					Switch<
						Case<Where<CustomerPaymentMethod.isActive, IsNotNull>, CustomerPaymentMethod.isActive>, PaymentMethod.isActive>>, Null> 
				), 
				typeof(bool))]
		public virtual Boolean? IsActive { get; set; }
		#endregion
		#region ARIsOnePerCustomer
		public abstract class aRIsOnePerCustomer : PX.Data.IBqlField
        {
        }
        [PXDBBool(BqlField = typeof(PaymentMethod.aRIsOnePerCustomer))]
        [PXDefault(false)]
        public virtual Boolean? ARIsOnePerCustomer { get; set; }
        #endregion     
		#region IsCustomerPaymentMethod
		public abstract class isCustomerPaymentMethod : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDBCalced(typeof(Switch<Case<Where<CustomerPaymentMethod.pMInstanceID, IsNotNull>, True>, False>), typeof(bool))]
		[PXUIField(DisplayName = "Override", Enabled = false)]
		public virtual bool? IsCustomerPaymentMethod { get; set; }
		#endregion
	}
}
