namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.INPIReview)]
	public partial class INPIHeader : PX.Data.IBqlTable
	{
		#region PIID
		public abstract class pIID : PX.Data.IBqlField
		{
		}
		protected String _PIID;
		[PXDefault()]
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName="Reference Nbr.", Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INPIHeader.pIID, Where<boolTrue, Equal<boolTrue>>, OrderBy<Desc<INPIHeader.pIID>>>), Filterable = true)]
		[AutoNumber(typeof(INSetup.pINumberingID), typeof(AccessInfo.businessDate))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String PIID
		{
			get
			{
				return this._PIID;
			}
			set
			{
				this._PIID = value;
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

		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected int? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Number Of Lines", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual int? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
		#region TagNumbered
		public abstract class tagNumbered : PX.Data.IBqlField
		{
		}
		protected Boolean? _TagNumbered;
		[PXDBBool()]
		[PXUIField(DisplayName = "Tag Numbered", Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Search<INSetup.pIUseTags>))]
		public virtual Boolean? TagNumbered
		{
			get
			{
				return this._TagNumbered;
			}
			set
			{
				this._TagNumbered = value;
			}
		}
		#endregion
		#region FirstTagNbr
		public abstract class firstTagNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _FirstTagNbr;
		[PXDBInt()]
		public virtual Int32? FirstTagNbr
		{
			get
			{
				return this._FirstTagNbr;
			}
			set
			{
				this._FirstTagNbr = value;
			}
		}
		#endregion		
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodID()]
		public virtual String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[FinPeriodID()]
		public virtual String TranPeriodID
		{
			get
			{
				return this._TranPeriodID;
			}
			set
			{
				this._TranPeriodID = value;
			}
		}
		#endregion
		#region PIAdjAcctID
		public abstract class pIAdjAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PIAdjAcctID;
		[Account(Enabled = false)]
		[PXDefault(typeof(Search2<ReasonCode.accountID, InnerJoin<INSetup, On<INSetup.pIReasonCode, Equal<ReasonCode.reasonCodeID>>>>)  )]
		public virtual Int32? PIAdjAcctID
		{
			get
			{
				return this._PIAdjAcctID;
			}
			set
			{
				this._PIAdjAcctID = value;
			}
		}
		#endregion
		#region PIAdjSubID
		public abstract class pIAdjSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PIAdjSubID;
		[SubAccount(Enabled = false)]
		[PXDefault(typeof(Search2<ReasonCode.subID, InnerJoin<INSetup, On<INSetup.pIReasonCode, Equal<ReasonCode.reasonCodeID>>>>)  )]
		public virtual Int32? PIAdjSubID
		{
			get
			{
				return this._PIAdjSubID;
			}
			set
			{
				this._PIAdjSubID = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(Enabled = false)]
		[PXDefault()]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INPIHdrStatus.Counting)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[INPIHdrStatus.List()]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		
		#region CountDate		
		public abstract class countDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CountDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		// for the user interface : renamed to the 'Freeze Date'
		[PXUIField(DisplayName = "Freeze Date", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual DateTime? CountDate
		{
			get
			{
				return this._CountDate;
			}
			set
			{
				this._CountDate = value;
			}
		}
		#endregion

		#region PIAdjRefNbr
		public abstract class pIAdjRefNbr : PX.Data.IBqlField
		{
		}
		protected String _PIAdjRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Adjustment Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(INRegister.refNbr))]
		public virtual String PIAdjRefNbr
		{
			get
			{
				return this._PIAdjRefNbr;
			}
			set
			{
				this._PIAdjRefNbr = value;
			}
		}
		#endregion

		#region PIRcptRefNbr
		public abstract class pIRcptRefNbr : PX.Data.IBqlField
		{
		}
		protected String _PIRcptRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Receipt Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(INRegister.refNbr))]
		public virtual String PIRcptRefNbr
		{
			get
			{
				return this._PIRcptRefNbr;
			}
			set
			{
				this._PIRcptRefNbr = value;
			}
		}
		#endregion

		#region TotalPhysicalQty		
		public abstract class totalPhysicalQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalPhysicalQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Total Physical Qty.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? TotalPhysicalQty
		{
			get
			{
				return this._TotalPhysicalQty;
			}
			set
			{
				this._TotalPhysicalQty = value;
			}
		}
		#endregion

		#region TotalVarQty
		public abstract class totalVarQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalVarQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Variance Qty.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? TotalVarQty
		{
			get
			{
				return this._TotalVarQty;
			}
			set
			{
				this._TotalVarQty = value;
			}
		}
		#endregion
		#region TotalVarCost
		public abstract class totalVarCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalVarCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Variance Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? TotalVarCost
		{
			get
			{
				return this._TotalVarCost;
			}
			set
			{
				this._TotalVarCost = value;
			}
		}
		#endregion

		#region TotalNbrOfTags
		public abstract class totalNbrOfTags : PX.Data.IBqlField
		{
		}
		protected int? _TotalNbrOfTags;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Number Of Tags", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual int? TotalNbrOfTags
		{
			get
			{
				return this._TotalNbrOfTags;
			}
			set
			{
				this._TotalNbrOfTags = value;
			}
		}
		#endregion
		
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INPIHeader.pIID), Selector = typeof(INPIHeader.pIID))]
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

/*
	public class INPIType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { ByInventory, ByLocation },
			new string[] { Messages.ByInventory, Messages.ByLocation }) { }
		}

		public const string ByInventory = "I";
		public const string ByLocation = "L";

		public class byInventory : Constant<string>
		{
			public byInventory() : base(ByInventory) { ;}
		}

		public class byLocation : Constant<string>
		{
			public byLocation() : base(ByLocation) { ;}
		}
	}
*/
	public class INPIHdrStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Counting, Entering, Completed, Cancelled },
				new string[] { Messages.Counting, Messages.DataEntering, Messages.Completed, Messages.Cancelled }) { ; }
		}

		public const string Counting = "N";
		public const string Entering = "E";
		public const string Completed = "C";
		public const string Cancelled = "X";
		

		public class counting : Constant<string>
		{
			public counting() : base(Counting) { ;}
		}

		public class entering : Constant<string>
		{
			public entering() : base(Entering) { ;}
		}

		public class completed : Constant<string>
		{
			public completed() : base(Completed) { ;}
		}

		public class cancelled : Constant<string>
		{
			public cancelled() : base(Cancelled) { ;}
		}
	}



}

