namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.CA;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.CustomerPaymentMethodDetail)]
	public partial class CustomerPaymentMethodDetail : PX.Data.IBqlTable
	{
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PMInstanceID;
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(CustomerPaymentMethod.pMInstanceID))]
		[PXParent(typeof(Select<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Current<CustomerPaymentMethodDetail.pMInstanceID>>>>))]
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
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(Search<CustomerPaymentMethod.paymentMethodID, Where<CustomerPaymentMethod.pMInstanceID,Equal<Current<CustomerPaymentMethodDetail.pMInstanceID>>>>))]		
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
		#region DetailID
		public abstract class detailID : PX.Data.IBqlField
		{
			//To Override Unconditional Select
			public class DetailIDSelectorAttribute : PXSelectorAttribute
			{
				public DetailIDSelectorAttribute()
					: base(typeof(Search<PaymentMethodDetail.detailID,
						Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CustomerPaymentMethodDetail.paymentMethodID>>,
						And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>>>>))
				{
					_UnconditionalSelect = _PrimarySelect;
					DescriptionField = typeof(PaymentMethodDetail.descr);                    
				}
			}
		}
		protected String _DetailID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Description", Enabled=false, IsReadOnly=true)]
		[detailID.DetailIDSelector()] //To Override Unconditional Select
		public virtual String DetailID
		{
			get
			{
				return this._DetailID;
			}
			set
			{
				this._DetailID = value;
			}
		}
		#endregion
		#region Value
		public abstract class value : PX.Data.IBqlField
		{
		}
		protected String _Value;
		
		//[PXDBStringWithMask(255, typeof(Search<PaymentMethodDetail.entryMask, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CustomerPaymentMethodDetail.paymentMethodID>>,
		//                              And<PaymentMethodDetail.detailID, Equal<Current<CustomerPaymentMethodDetail.detailID>>>>>), IsUnicode = true)]
		[DynamicValueValidation(typeof(Search<PaymentMethodDetail.validRegexp, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CustomerPaymentMethodDetail.paymentMethodID>>,
										And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,
									  And<PaymentMethodDetail.detailID, Equal<Current<CustomerPaymentMethodDetail.detailID>>>>>>))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXRSACryptStringWithMaskAttribute(1028, typeof(Search<PaymentMethodDetail.entryMask, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CustomerPaymentMethodDetail.paymentMethodID>>,
									  And<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForARCards>,
									  And<PaymentMethodDetail.detailID, Equal<Current<CustomerPaymentMethodDetail.detailID>>>>>>), IsUnicode = true)]
		//[PXRSACryptStringAttribute(1028)]
		[PXUIField(DisplayName = "Value")]
		public virtual String Value
		{
			get
			{
				return this._Value;
			}
			set
			{
				this._Value = value;
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
