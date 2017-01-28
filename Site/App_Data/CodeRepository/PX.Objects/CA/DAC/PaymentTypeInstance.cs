namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.EP;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.PaymentTypeInstance)]
	public partial class PaymentTypeInstance : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault()]
		[PXUIField(DisplayName = "Cash account")]
        [GL.CashAccount(null, typeof(Search5<CashAccount.cashAccountID, InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>,
                                            And<PaymentMethodAccount.useForAP,Equal<True>>>,
											InnerJoin<PaymentMethod,On<PaymentMethod.paymentMethodID,Equal<PaymentMethodAccount.paymentMethodID>,
											And<PaymentMethod.aPAllowInstances,Equal<BQLConstants.BitOn>>>>>,
											Where<Match<Current<AccessInfo.userName>>>,
                                            Aggregate<GroupBy<CashAccount.cashAccountID>>>), 
			DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible, IsKey = true)]
					
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
		#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Card")]
		[PXSelector(typeof(Search<PaymentTypeInstance.pTInstanceID, Where<PaymentTypeInstance.cashAccountID, Equal<Current<PaymentTypeInstance.cashAccountID>>>>), DescriptionField = typeof(PaymentTypeInstance.descr))]
		public virtual Int32? PTInstanceID
		{
			get
			{
				return this._PTInstanceID;
			}
			set
			{
				this._PTInstanceID = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search2<PaymentMethodAccount.paymentMethodID,
                            InnerJoin<PaymentMethod, On<PaymentMethod.paymentMethodID, Equal<PaymentMethodAccount.paymentMethodID>>>,
                            Where<PaymentMethodAccount.cashAccountID, Equal<Current<PaymentTypeInstance.cashAccountID>>,
                            And<PaymentMethodAccount.useForAP, Equal<True>,
							And<PaymentMethod.aPAllowInstances, Equal<True>>>>>))]
		[PXUIField(DisplayName = "Payment Method")]
		[PXSelector(typeof(Search2<PaymentMethod.paymentMethodID,
								InnerJoin<PaymentMethodAccount,
                                        On<PaymentMethodAccount.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>>,
                                Where<PaymentMethodAccount.cashAccountID, Equal<Current<PaymentTypeInstance.cashAccountID>>,
								And<PaymentMethod.aPAllowInstances,Equal<True>>>>), 
								DescriptionField = typeof(PaymentMethod.descr))]
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt()]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Employee ID",Visibility =PXUIVisibility.SelectorVisible)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(255, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Description",Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region ExpirationDate
		public abstract class expirationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpirationDate;
		[PXDBDate()]
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
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
