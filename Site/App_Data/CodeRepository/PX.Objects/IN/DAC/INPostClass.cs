namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INPostClassMaint))]
	[PXCacheName(Messages.PostingClass)]
	public partial class INPostClass : PX.Data.IBqlTable
	{
		#region PostClassID
		public abstract class postClassID : PX.Data.IBqlField
		{
		}
		protected String _PostClassID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Class ID", Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INPostClass.postClassID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String PostClassID
		{
			get
			{
				return this._PostClassID;
			}
			set
			{
				this._PostClassID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
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
		#region ReasonCodeSubID
		public abstract class reasonCodeSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReasonCodeSubID;
		[SubAccount(DisplayName = "Reason Code Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? ReasonCodeSubID
		{
			get
			{
				return this._ReasonCodeSubID;
			}
			set
			{
				this._ReasonCodeSubID = value;
			}
		}
		#endregion
		#region SalesAcctID
		public abstract class salesAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesAcctID;
		[Account(DisplayName = "Sales Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? SalesAcctID
		{
			get
			{
				return this._SalesAcctID;
			}
			set
			{
				this._SalesAcctID = value;
			}
		}
		#endregion
		#region SalesSubID
		public abstract class salesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesSubID;
		[SubAccount(typeof(INPostClass.salesAcctID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? SalesSubID
		{
			get
			{
				return this._SalesSubID;
			}
			set
			{
				this._SalesSubID = value;
			}
		}
		#endregion
		#region InvtAcctID
		public abstract class invtAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtAcctID;
		[Account(DisplayName = "Inventory/Accrual Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? InvtAcctID
		{
			get
			{
				return this._InvtAcctID;
			}
			set
			{
				this._InvtAcctID = value;
			}
		}
		#endregion
		#region InvtSubID
		public abstract class invtSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtSubID;
		[SubAccount(typeof(INPostClass.invtAcctID), DisplayName = "Inventory/Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? InvtSubID
		{
			get
			{
				return this._InvtSubID;
			}
			set
			{
				this._InvtSubID = value;
			}
		}
		#endregion
		#region COGSAcctID
		public abstract class cOGSAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSAcctID;
		[Account(DisplayName = "COGS/Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? COGSAcctID
		{
			get
			{
				return this._COGSAcctID;
			}
			set
			{
				this._COGSAcctID = value;
			}
		}
		#endregion
		#region COGSSubID
		public abstract class cOGSSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSSubID;
		[SubAccount(typeof(INPostClass.cOGSAcctID), DisplayName = "COGS/Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? COGSSubID
		{
			get
			{
				return this._COGSSubID;
			}
			set
			{
				this._COGSSubID = value;
			}
		}
		#endregion
		#region DiscAcctID
		public abstract class discAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscAcctID;
		[Account(DisplayName = "Discount Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? DiscAcctID
		{
			get
			{
				return this._DiscAcctID;
			}
			set
			{
				this._DiscAcctID = value;
			}
		}
		#endregion
		#region DiscSubID
		public abstract class discSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscSubID;
		[SubAccount(typeof(INPostClass.discAcctID), DisplayName = "Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? DiscSubID
		{
			get
			{
				return this._DiscSubID;
			}
			set
			{
				this._DiscSubID = value;
			}
		}
		#endregion
		#region StdCstRevAcctID
		public abstract class stdCstRevAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstRevAcctID;
		[Account(DisplayName = "Standard Cost Revaluation Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? StdCstRevAcctID
		{
			get
			{
				return this._StdCstRevAcctID;
			}
			set
			{
				this._StdCstRevAcctID = value;
			}
		}
		#endregion
		#region StdCstRevSubID
		public abstract class stdCstRevSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstRevSubID;
		[SubAccount(typeof(INPostClass.stdCstRevAcctID), DisplayName = "Standard Cost Revaluation Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? StdCstRevSubID
		{
			get
			{
				return this._StdCstRevSubID;
			}
			set
			{
				this._StdCstRevSubID = value;
			}
		}
		#endregion
		#region StdCstVarAcctID
		public abstract class stdCstVarAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstVarAcctID;
		[Account(DisplayName = "Standard Cost Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? StdCstVarAcctID
		{
			get
			{
				return this._StdCstVarAcctID;
			}
			set
			{
				this._StdCstVarAcctID = value;
			}
		}
		#endregion
		#region StdCstVarSubID
		public abstract class stdCstVarSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstVarSubID;
		[SubAccount(typeof(INPostClass.stdCstVarAcctID), DisplayName = "Standard Cost Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? StdCstVarSubID
		{
			get
			{
				return this._StdCstVarSubID;
			}
			set
			{
				this._StdCstVarSubID = value;
			}
		}
		#endregion
		#region PPVAcctID
		public abstract class pPVAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PPVAcctID;
		[Account(DisplayName = "Purchase Price Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? PPVAcctID
		{
			get
			{
				return this._PPVAcctID;
			}
			set
			{
				this._PPVAcctID = value;
			}
		}
		#endregion
		#region PPVSubID
		public abstract class pPVSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PPVSubID;
		[SubAccount(typeof(INPostClass.pPVAcctID), DisplayName = "Purchase Price Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? PPVSubID
		{
			get
			{
				return this._PPVSubID;
			}
			set
			{
				this._PPVSubID = value;
			}
		}
		#endregion
		#region POAccrualAcctID
		public abstract class pOAccrualAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualAcctID;
		[Account(DisplayName = "PO Accrual Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? POAccrualAcctID
		{
			get
			{
				return this._POAccrualAcctID;
			}
			set
			{
				this._POAccrualAcctID = value;
			}
		}
		#endregion
		#region POAccrualSubID
		public abstract class pOAccrualSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualSubID;
		[SubAccount(typeof(INPostClass.pOAccrualAcctID), DisplayName = "PO Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? POAccrualSubID
		{
			get
			{
				return this._POAccrualSubID;
			}
			set
			{
				this._POAccrualSubID = value;
			}
		}
		#endregion
        #region PIReasonCode
        public abstract class pIReasonCode : PX.Data.IBqlField
        {
        }
        protected String _PIReasonCode;
		[PXDBString(ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.adjustment>>>), DescriptionField = typeof(ReasonCode.descr))]
        [PXUIField(DisplayName = "Phys.Inventory Reason Code")]
        public virtual String PIReasonCode
        {
            get
            {
                return this._PIReasonCode;
            }
            set
            {
                this._PIReasonCode = value;
            }
        }
        #endregion
		#region InvtAcctDefault
		public abstract class invtAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _InvtAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Inventory/Accrual Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String InvtAcctDefault
		{
			get
			{
				return this._InvtAcctDefault;
			}
			set
			{
				this._InvtAcctDefault = value;
			}
		}
		#endregion
		#region COGSSubFromSales
		public abstract class cOGSSubFromSales : PX.Data.IBqlField
		{
		}
		protected Boolean? _COGSSubFromSales;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Copy COGS Sub. from Sales", FieldClass = SubAccountAttribute.DimensionName)]
		public virtual Boolean? COGSSubFromSales
		{
			get
			{
				return this._COGSSubFromSales;
			}
			set
			{
				this._COGSSubFromSales = value;
			}
		}
		#endregion
		#region COGSAcctDefault
		public abstract class cOGSAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _COGSAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use COGS/Expense Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String COGSAcctDefault
		{
			get
			{
				return this._COGSAcctDefault;
			}
			set
			{
				this._COGSAcctDefault = value;
			}
		}
		#endregion
		#region SalesAcctDefault
		public abstract class salesAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _SalesAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Sales Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String SalesAcctDefault
		{
			get
			{
				return this._SalesAcctDefault;
			}
			set
			{
				this._SalesAcctDefault = value;
			}
		}
		#endregion
		#region DiscAcctDefault
		public abstract class discAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _DiscAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Discount Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String DiscAcctDefault
		{
			get
			{
				return this._DiscAcctDefault;
			}
			set
			{
				this._DiscAcctDefault = value;
			}
		}
		#endregion
		#region StdCstRevAcctDefault
		public abstract class stdCstRevAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _StdCstRevAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Std. Cost Revaluation Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String StdCstRevAcctDefault
		{
			get
			{
				return this._StdCstRevAcctDefault;
			}
			set
			{
				this._StdCstRevAcctDefault = value;
			}
		}
		#endregion
		#region StdCstVarAcctDefault
		public abstract class stdCstVarAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _StdCstVarAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Std. Cost Variance Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String StdCstVarAcctDefault
		{
			get
			{
				return this._StdCstVarAcctDefault;
			}
			set
			{
				this._StdCstVarAcctDefault = value;
			}
		}
		#endregion
		#region PPVAcctDefault
		public abstract class pPVAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _PPVAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Purchase Price Variance Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String PPVAcctDefault
		{
			get
			{
				return this._PPVAcctDefault;
			}
			set
			{
				this._PPVAcctDefault = value;
			}
		}
		#endregion
		#region POAccrualAcctDefault
		public abstract class pOAccrualAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _POAccrualAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use PO Accrual Account from")]
		[INAcctSubDefault.POAccrualList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String POAccrualAcctDefault
		{
			get
			{
				return this._POAccrualAcctDefault;
			}
			set
			{
				this._POAccrualAcctDefault = value;
			}
		}
		#endregion

		#region InvtSubMask
		public abstract class invtSubMask : PX.Data.IBqlField
		{
		}
		protected String _InvtSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Inventory/Accrual Sub. from")]
		public virtual String InvtSubMask
		{
			get
			{
				return this._InvtSubMask;
			}
			set
			{
				this._InvtSubMask = value;
			}
		}
		#endregion
		#region COGSSubMask
		public abstract class cOGSSubMask : PX.Data.IBqlField
		{
		}
		protected String _COGSSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine COGS/Expense Sub. from")]
		public virtual String COGSSubMask
		{
			get
			{
				return this._COGSSubMask;
			}
			set
			{
				this._COGSSubMask = value;
			}
		}
		#endregion
		#region SalesSubMask
		public abstract class salesSubMask : PX.Data.IBqlField
		{
		}
		protected String _SalesSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Sales Sub. from")]
		public virtual String SalesSubMask
		{
			get
			{
				return this._SalesSubMask;
			}
			set
			{
				this._SalesSubMask = value;
			}
		}
		#endregion
		#region StdCstVarSubMask
		public abstract class stdCstVarSubMask : PX.Data.IBqlField
		{
		}
		protected String _StdCstVarSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Std. Cost Variance Sub. from")]
		public virtual String StdCstVarSubMask
		{
			get
			{
				return this._StdCstVarSubMask;
			}
			set
			{
				this._StdCstVarSubMask = value;
			}
		}
		#endregion
		#region StdCstRevSubMask
		public abstract class stdCstRevSubMask : PX.Data.IBqlField
		{
		}
		protected String _StdCstRevSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Std. Cost Revaluation Sub. from")]
		public virtual String StdCstRevSubMask
		{
			get
			{
				return this._StdCstRevSubMask;
			}
			set
			{
				this._StdCstRevSubMask = value;
			}
		}
		#endregion
		#region PPVSubMask
		public abstract class pPVSubMask : PX.Data.IBqlField
		{
		}
		protected String _PPVSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Purchase Price Variance Sub. from")]
		public virtual String PPVSubMask
		{
			get
			{
				return this._PPVSubMask;
			}
			set
			{
				this._PPVSubMask = value;
			}
		}
		#endregion
		#region DiscSubMask
		public abstract class discSubMask : PX.Data.IBqlField
		{
		}
		protected String _DiscSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Discount Sub. from")]
		public virtual String DiscSubMask
		{
			get
			{
				return this._DiscSubMask;
			}
			set
			{
				this._DiscSubMask = value;
			}
		}
		#endregion
		#region POAccrualSubMask
		public abstract class pOAccrualSubMask : PX.Data.IBqlField
		{
		}
		protected String _POAccrualSubMask;
		[PXDefault()]
		[POAccrualSubAccountMask(DisplayName = "Combine PO Accrual Sub. from")]
		public virtual String POAccrualSubMask
		{
			get
			{
				return this._POAccrualSubMask;
			}
			set
			{
				this._POAccrualSubMask = value;
			}
		}
		#endregion
		#region LCVarianceAcctID
		public abstract class lCVarianceAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceAcctID;
		[Account(DisplayName = "Landed Cost Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? LCVarianceAcctID
		{
			get
			{
				return this._LCVarianceAcctID;
			}
			set
			{
				this._LCVarianceAcctID = value;
			}
		}
		#endregion
		#region LCVarianceSubID
		public abstract class lCVarianceSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceSubID;
		[SubAccount(typeof(INPostClass.lCVarianceAcctID), DisplayName = "Landed Cost Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? LCVarianceSubID
		{
			get
			{
				return this._LCVarianceSubID;
			}
			set
			{
				this._LCVarianceSubID = value;
			}
		}
		#endregion
		#region LCVarianceAcctDefault
		public abstract class lCVarianceAcctDefault : PX.Data.IBqlField
		{
		}
		protected String _LCVarianceAcctDefault;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Use Landed Cost Variance Account from")]
		[INAcctSubDefault.ClassList()]
		[PXDefault(INAcctSubDefault.MaskItem)]
		public virtual String LCVarianceAcctDefault
		{
			get
			{
				return this._LCVarianceAcctDefault;
			}
			set
			{
				this._LCVarianceAcctDefault = value;
			}
		}
		#endregion
		#region LCVarianceSubMask
		public abstract class lCVarianceSubMask : PX.Data.IBqlField
		{
		}
		protected String _LCVarianceSubMask;
		[PXDefault()]
		[SubAccountMask(DisplayName = "Combine Landed Cost Variance Sub. from")]
		public virtual String LCVarianceSubMask
		{
			get
			{
				return this._LCVarianceSubMask;
			}
			set
			{
				this._LCVarianceSubMask = value;
			}
		}
		#endregion
		#region DeferralAcctID
		public abstract class deferralAcctID : IBqlField { }

		[Account(DisplayName = "Deferral Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public int? DeferralAcctID { get; set; }
		#endregion
		#region DeferralSubID
		public abstract class deferralSubID : IBqlField { }

		[SubAccount(typeof(INPostClass.deferralAcctID), DisplayName = "Deferral Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public int? DeferralSubID { get; set; }
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INPostClass.postClassID), 
			Selector = typeof(INPostClass.postClassID))]
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
