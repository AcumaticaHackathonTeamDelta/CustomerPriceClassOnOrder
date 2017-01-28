namespace PX.Objects.CA
{
	using System;
	using PX.Data;
#if false
    public class PaymentTypeDetailUsage 
    {

        public const string UseForVendor = "V";
        public const string UseForCashAccount = "C";
        public const string UseForAll = "A";
        public const string UseForARCards = "R";
        public const string UseForAPCards = "P";
        

        public class useForVendor : Constant<string>
        {
            public useForVendor() : base(UseForVendor) { }
        }
        public class useForCashAccount : Constant<string>
        {
            public useForCashAccount() : base(UseForCashAccount) { }
        }
        public class useForAll : Constant<string>
        {
            public useForAll() : base(UseForAll) { }
        }

        public class useForARCards : Constant<string>
        {
            public useForARCards() : base(UseForARCards) { }
        }

        public class useForAPCards : Constant<string>
        {
            public useForAPCards() : base(UseForAPCards) { }
        }
        
    }


    [System.SerializableAttribute()]
    public partial class PaymentTypeDetail : PX.Data.IBqlTable
    {
    #region PaymentTypeID
        public abstract class paymentTypeID : PX.Data.IBqlField
        {
        }
        protected String _PaymentTypeID;
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDefault(typeof(PaymentType.paymentTypeID))]
        [PXUIField(DisplayName = "Payment Type", Visible = false)]
        [PXParent(typeof(Select<PaymentType, Where<PaymentType.paymentTypeID, Equal<Current<PaymentTypeDetail.paymentTypeID>>>>))]
        public virtual String PaymentTypeID
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
        protected Int32? _DetailID;
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "ID", Visible = false, Enabled = false)]
        [PXLineNbr(typeof(PaymentType))]
        public virtual Int32? DetailID
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
    #region UseFor
        public abstract class useFor : PX.Data.IBqlField
        {
        }
        protected String _UseFor;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(PaymentTypeDetailUsage.UseForAll)]
        [PXUIField(DisplayName = "Used In")]
        public virtual String UseFor
        {
            get
            {
                return this._UseFor;
            }
            set
            {
                this._UseFor = value;
            }
        }
    #endregion
    #region Descr
        public abstract class descr : PX.Data.IBqlField
        {
        }
        protected String _Descr;
        [PXDBString(60, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
    #region EntryMask
        public abstract class entryMask : PX.Data.IBqlField
        {
        }
        protected String _EntryMask;
        [PXDBString(255)]
        [PXUIField(DisplayName = "Entry Mask")]
        public virtual String EntryMask
        {
            get
            {
                return this._EntryMask;
            }
            set
            {
                this._EntryMask = value;
            }
        }
    #endregion
    #region ValidRegexp
        public abstract class validRegexp : PX.Data.IBqlField
        {
        }
        protected String _ValidRegexp;
        [PXDBString(255)]
        [PXUIField(DisplayName = "Validation Reg. Exp.")]
        public virtual String ValidRegexp
        {
            get
            {
                return this._ValidRegexp;
            }
            set
            {
                this._ValidRegexp = value;
            }
        }
    #endregion
    #region OrderIndex
        public abstract class orderIndex : PX.Data.IBqlField
        {
        }
        protected Int16? _OrderIndex;
        [PXDBShort()]
        [PXUIField(DisplayName = "Sort Order")]
        public virtual Int16? OrderIndex
        {
            get
            {
                return this._OrderIndex;
            }
            set
            {
                this._OrderIndex = value;
            }
        }
    #endregion
    #region DisplayMask
        public abstract class displayMask : PX.Data.IBqlField
        {
        }
        protected String _DisplayMask;
        [PXDBString(255)]
        [PXUIField(DisplayName = "Display Mask")]
        public virtual String DisplayMask
        {
            get
            {
                return this._DisplayMask;
            }
            set
            {
                this._DisplayMask = value;
            }
        }
    #endregion
    #region IsEncrypted
        public abstract class isEncrypted : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsEncrypted;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Encrypted")]
        public virtual Boolean? IsEncrypted
        {
            get
            {
                return this._IsEncrypted;
            }
            set
            {
                this._IsEncrypted = value;
            }
        }
    #endregion
    #region IsRequired
        public abstract class isRequired : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsRequired;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Required")]
        public virtual Boolean? IsRequired
        {
            get
            {
                return this._IsRequired;
            }
            set
            {
                this._IsRequired = value;
            }
        }
    #endregion
    #region IsIdentifier
        public abstract class isIdentifier : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsIdentifier;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Identifier")]
        public virtual Boolean? IsIdentifier
        {
            get
            {
                return this._IsIdentifier;
            }
            set
            {
                this._IsIdentifier = value;
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
#endif
}
