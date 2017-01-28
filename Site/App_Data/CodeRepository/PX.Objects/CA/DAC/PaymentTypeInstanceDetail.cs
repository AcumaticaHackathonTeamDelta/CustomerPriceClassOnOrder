namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.PaymentTypeInstanceDetail)]
	public partial class PaymentTypeInstanceDetail : PX.Data.IBqlTable
	{
		#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;
		
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(PaymentTypeInstance.pTInstanceID))]
		[PXParent(typeof(Select<PaymentTypeInstance, Where<PaymentTypeInstance.pTInstanceID, Equal<Current<PaymentTypeInstanceDetail.pTInstanceID>>>>))]
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
		protected String _PaymentTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<PaymentTypeInstance.paymentMethodID, Where<PaymentTypeInstance.pTInstanceID, Equal<Current<PaymentTypeInstanceDetail.pTInstanceID>>>>))]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentTypeID;
			}
			set
			{
				this._PaymentTypeID = value;
			}
		}
			#endregion
		#region DetailID
		public abstract class detailID : PX.Data.IBqlField
		{
		}
		protected String _DetailID;
        [PXDBString(10, IsUnicode = true, IsKey=true)]
		[PXDefault()]
		[PXUIField(DisplayName = "ID")]
		[PXSelector(typeof(Search<PaymentMethodDetail.detailID, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<PaymentTypeInstanceDetail.paymentMethodID>>,
                                    And<PaymentMethodDetail.useFor,Equal<PaymentMethodDetailUsage.useForAPCards>>>>), DescriptionField = typeof(PaymentMethodDetail.descr))]
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

		[PXUIField(DisplayName = "Value")]
		[DynamicValueValidation(typeof(Search<PaymentMethodDetail.validRegexp,
										Where<PaymentMethodDetail.paymentMethodID, Equal<Current<PaymentTypeInstanceDetail.paymentMethodID>>,
                                        And<PaymentMethodDetail.useFor,Equal<PaymentMethodDetailUsage.useForAPCards>,
										And<PaymentMethodDetail.detailID, Equal<Current<PaymentTypeInstanceDetail.detailID>>>>>>))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXRSACryptStringWithMaskAttribute(1028, typeof(Search<PaymentMethodDetail.entryMask, 
										Where<PaymentMethodDetail.paymentMethodID, Equal<Current<PaymentTypeInstanceDetail.paymentMethodID>>,
                                        And<PaymentMethodDetail.useFor,Equal<PaymentMethodDetailUsage.useForAPCards>,
                                        And<PaymentMethodDetail.detailID, Equal<Current<PaymentTypeInstanceDetail.detailID>>>>>>), IsUnicode = true)]
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
