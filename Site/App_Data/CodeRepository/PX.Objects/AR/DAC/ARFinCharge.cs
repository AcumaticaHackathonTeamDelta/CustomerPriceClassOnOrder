using PX.Objects.CM;

namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.TX;

	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ARFinChargesMaint))]
	[PXCacheName(Messages.ARFinCharge)]
	public partial class ARFinCharge : PX.Data.IBqlTable
	{
		#region FinChargeID
		public abstract class finChargeID : PX.Data.IBqlField
		{
		}
		protected String _FinChargeID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Overdue Charge ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(ARFinCharge.finChargeID))]
		public virtual String FinChargeID
		{
			get
			{
				return this._FinChargeID;
			}
			set
			{
				this._FinChargeID = value;
			}
		}
		#endregion
		#region FinChargeDesc
		public abstract class finChargeDesc : PX.Data.IBqlField
		{
		}
		protected String _FinChargeDesc;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String FinChargeDesc
		{
			get
			{
				return this._FinChargeDesc;
			}
			set
			{
				this._FinChargeDesc = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault()]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.customer>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		public virtual String TermsID
		{
			get
			{
				return this._TermsID;
			}
			set
			{
				this._TermsID = value;
			}
		}
		#endregion
		#region BaseCurFlag
		public abstract class baseCurFlag : PX.Data.IBqlField
		{
		}
		protected Boolean? _BaseCurFlag;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Base Currency")]
		public virtual Boolean? BaseCurFlag
		{
			get
			{
				return this._BaseCurFlag;
			}
			set
			{
				this._BaseCurFlag = value;
			}
		}
		#endregion
		#region MinFinChargeFlag
		public abstract class minFinChargeFlag : PX.Data.IBqlField
		{
		}
		protected Boolean? _MinFinChargeFlag;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Use Line Minimum Amount")]
		public virtual Boolean? MinFinChargeFlag
		{
			get
			{
				return this._MinFinChargeFlag;
			}
			set
			{
				this._MinFinChargeFlag = value;
			}
		}
        #endregion
        #region MinFinChargeAmount
        public abstract class minFinChargeAmount : PX.Data.IBqlField
        {
        }
        protected Decimal? _MinFinChargeAmount;
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Min. Amount")]
        public virtual Decimal? MinFinChargeAmount
        {
            get
            {
                return this._MinFinChargeAmount;
            }
            set
            {
                this._MinFinChargeAmount = value;
            }
        }
        #endregion
        #region LineThreshold
        public abstract class lineThreshold : PX.Data.IBqlField
        {
        }
        [PXBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Threshold")]
        public virtual Decimal? LineThreshold
        {
            get
            {
                return this._MinFinChargeAmount;
            }
            set
            {
                this._MinFinChargeAmount = value;
            }
        }
        #endregion
        #region FixedAmount
        public abstract class fixedAmount : PX.Data.IBqlField
		{
		}
		[PXBaseCury]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Amount")]
		public virtual Decimal? FixedAmount
		{
			get
			{
				return this._MinFinChargeAmount;
			}
			set
			{
				this._MinFinChargeAmount = value;
			}
		}
		#endregion
		#region MinChargeDocumentAmt
		public abstract class minChargeDocumentAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinChargeDocumentAmt;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Total Threshold")]
		public virtual Decimal? MinChargeDocumentAmt
		{
			get
			{
				return this._MinChargeDocumentAmt;
			}
			set
			{
				this._MinChargeDocumentAmt = value;
			}
		}
		#endregion
		#region PercentFlag
		public abstract class percentFlag : PX.Data.IBqlField
		{
		}
		protected Boolean? _PercentFlag;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Use Percent Rate")]
		public virtual Boolean? PercentFlag
		{
			get
			{
				return this._PercentFlag;
			}
			set
			{
				this._PercentFlag = value;
			}
		}
		#endregion
		#region FinChargeAcctID
		public abstract class finChargeAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _FinChargeAccountID;

		[PXDefault()]
		[PXNonCashAccount(DisplayName = "Overdue Charge Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description),Required=true)]
		public virtual Int32? FinChargeAccountID
		{
			get
			{
				return this._FinChargeAccountID;
			}
			set
			{
				this._FinChargeAccountID = value;
			}
		}
		#endregion
		#region FinChargeSubID
		public abstract class finChargeSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _FinChargeSubID;

		[PXDefault()]
		[SubAccount(typeof(ARFinCharge.finChargeAccountID), DisplayName = "Overdue Charge Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description),Required=true)]
		public virtual Int32? FinChargeSubID
		{
			get
			{
				return this._FinChargeSubID;
			}
			set
			{
				this._FinChargeSubID = value;
			}
		}
		#endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField		
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion
		#region FeeAcctID
		public abstract class feeAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _FeeAccountID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXNonCashAccount(DisplayName = "Fee Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? FeeAccountID
		{
			get
			{
				return this._FeeAccountID;
			}
			set
			{
				this._FeeAccountID = value;
			}
		}
		#endregion
		#region FeeSubID
		public abstract class feeSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _FeeSubID;

		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[SubAccount(typeof(ARFinCharge.finChargeAccountID), DisplayName = "Fee Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? FeeSubID
		{
			get
			{
				return this._FeeSubID;
			}
			set
			{
				this._FeeSubID = value;
			}
		}
        #endregion
        #region FeeAmount
        public abstract class feeAmount : PX.Data.IBqlField
        {
        }
        protected Decimal? _FeeAmount;
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Fee Amount", Visibility = PXUIVisibility.Visible, Enabled = true)]
        public virtual Decimal? FeeAmount
        {
            get
            {
                return this._FeeAmount;
            }
            set
            {
                this._FeeAmount = value;
            }
        }
        #endregion
        #region FeeDesc
        public abstract class feeDesc : PX.Data.IBqlField
        {
        }
        protected String _FeeDesc;
        [PXDBLocalizableString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Fee Description")]
        public virtual String FeeDesc
        {
            get
            {
                return this._FeeDesc;
            }
            set
            {
                this._FeeDesc = value;
            }
        }
        #endregion
        #region CalculationMethod
        public abstract class calculationMethod : PX.Data.IBqlField
        {
        }
        protected Int32? _CalculationMethod;
        [PXDBInt()]
        [PXDefault(0)]
        [PXIntList(new int[]
        {
            OverdueCalculationMethod.InterestOnBalance,
            OverdueCalculationMethod.InterestOnProratedBalance,
            OverdueCalculationMethod.InterestOnArrears
        }, new string[]
        {
            Messages.InterestOnBalance,
            Messages.InterestOnProratedBalance,
            Messages.InterestOnArrears
        })]
        [PXUIField(DisplayName = "Calculation Method", Visibility = PXUIVisibility.Visible)]
        public virtual Int32? CalculationMethod
        {
            get
            {
                return this._CalculationMethod;
            }
            set
            {
                this._CalculationMethod = value;
            }
        }
        #endregion
        #region ChargingMethod
        public abstract class chargingMethod : PX.Data.IBqlField
        {
        }
        [PXInt()]
        [PXDefault(1)]
        [PXIntList(new int[]
        {
            OverdueChargingMethod.FixedAmount,
            OverdueChargingMethod.PercentWithThreshold,
            OverdueChargingMethod.PercentWithMinAmount
        }, new string[]
        {
            Messages.FixedAmount,
            Messages.PercentWithThreshold,
            Messages.PercentWithMinAmount
        })]
        [PXUIField(DisplayName = "Charging Method")]
        public virtual int? ChargingMethod
        {
            get
            {
                if (MinFinChargeFlag == true && PercentFlag == false)
                {
                    return OverdueChargingMethod.FixedAmount;
                }
                else if (MinFinChargeFlag == false && PercentFlag == true)
                {
                    return OverdueChargingMethod.PercentWithThreshold;
                }
                else if (MinFinChargeFlag == true && PercentFlag == true)
                {
                    return OverdueChargingMethod.PercentWithMinAmount;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                switch (value)
                {
                    case OverdueChargingMethod.FixedAmount:
                        MinFinChargeFlag = true; PercentFlag = false; break;
                    case OverdueChargingMethod.PercentWithThreshold:
                        MinFinChargeFlag = false; PercentFlag = true; break;
                    case OverdueChargingMethod.PercentWithMinAmount:
                        MinFinChargeFlag = true; PercentFlag = true; break;
                }
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote]
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
    }

    public class OverdueChargingMethod
    {
        public const int FixedAmount = 1;
        public const int PercentWithThreshold = 2;
        public const int PercentWithMinAmount = 3;
    }

    public class OverdueCalculationMethod
    {
        public const int InterestOnBalance = 0;
        public const int InterestOnProratedBalance = 1;
        public const int InterestOnArrears = 2;
    }
}
