namespace PX.Objects.CA
{
	using System;
	using PX.Data;
    using PX.Objects.AP;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.CashAccountCheck)]
	public partial class CashAccountCheck : PX.Data.IBqlTable
	{
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Cash Account ID", Visible = false)]
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
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Payment Method")]
		[PXSelector(typeof(PaymentMethod.paymentMethodID))]
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
		#region CashAccountCheckID
		public abstract class cashAccountCheckID : IBqlField {}
		[PXDBIdentity]
		public virtual int? CashAccountCheckID { get; set; }
		#endregion
		#region CheckNbr
		public abstract class checkNbr : IBqlField {}
		protected string _CheckNbr;
		[PXDBString(40, IsKey=true, IsUnicode = true)]
		[PXUIField(DisplayName = "Check Number")]
		[PXDefault]
		[PXParent(typeof(Select<APAdjust, Where<APAdjust.stubNbr, Equal<Current<CashAccountCheck.checkNbr>>,
			And<APAdjust.paymentMethodID, Equal<Current<CashAccountCheck.paymentMethodID>>,
			And<APAdjust.cashAccountID, Equal<Current<CashAccountCheck.accountID>>>>>>))]
		public virtual string CheckNbr
		{
			get
			{
				return _CheckNbr;
			}
			set
			{
				_CheckNbr = value;
			}
		}
		#endregion
        #region DocType
        public abstract class docType : PX.Data.IBqlField
        {
        }
        protected String _DocType;
        [PXDBString(3, IsFixed = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Document Type", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [APDocType.List()]
        public virtual String DocType
        {
            get
            {
                return this._DocType;
            }
            set
            {
                this._DocType = value;
            }
        }
        #endregion
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        protected String _RefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
        #region FinPeriodID
        public abstract class finPeriodID : PX.Data.IBqlField
        {
        }
        [PXDBString(6, IsFixed = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Application Period", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [GL.FinPeriodIDFormatting]
        public virtual String FinPeriodID { get; set; }
        #endregion
        #region DocDate
        public abstract class docDate : PX.Data.IBqlField
        {
        }
        [PXDBDate()]
        [PXDefault()]
        [PXUIField(DisplayName = "Document Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual DateTime? DocDate { get; set; }
        #endregion
        #region VendorID
        public abstract class vendorID : PX.Data.IBqlField
        {
        }
        protected Int32? _VendorID;
        [PXDefault()]
        [VendorAttribute]
        public virtual Int32? VendorID { get; set; }
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
