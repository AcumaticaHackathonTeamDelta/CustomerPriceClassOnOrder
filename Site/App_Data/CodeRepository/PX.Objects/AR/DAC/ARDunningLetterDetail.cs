namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.CA;

	/// <summary>
	/// List of invoises in Dunning Letter
	/// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARDunningLetterDetail)]
	public partial class ARDunningLetterDetail : PX.Data.IBqlTable
	{
		#region DunningLetterID
		public abstract class dunningLetterID : PX.Data.IBqlField
		{
		}
		protected Int32? _DunningLetterID;
		[PXDBInt(IsKey=true)]
		[PXDBLiteDefault(typeof(ARDunningLetter.dunningLetterID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Enabled = false)]
		[PXParent(typeof(Select<ARDunningLetter, Where<ARDunningLetter.dunningLetterID, Equal<Current<ARDunningLetterDetail.dunningLetterID>>>>))]
		public virtual Int32? DunningLetterID
		{
			get
			{
				return this._DunningLetterID;
			}
			set
			{
				this._DunningLetterID = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsFixed = true, IsKey=true)]
		[ARDocType.List()]
		[PXUIField(DisplayName="Type")]
		[PXDefault()]
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
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Reference Nbr.")]
		[PXDefault()]
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}

		[PXDefault]
		[PXUIField(DisplayName = "Customer", IsReadOnly = true, Visible = false)]
		[Customer(DescriptionField = typeof(Customer.acctName))]
		public virtual int? BAccountID { get; set; }
		#endregion
		#region DunningLetterBAccountID
		public abstract class dunningLetterBAccountID : PX.Data.IBqlField
		{
		}

		[PXDefault]
		[PXDBInt]
		public virtual int? DunningLetterBAccountID { get; set; }
		#endregion
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate()]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Due Date")]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		public virtual DateTime? DueDate
		{
			get
			{
				return this._DueDate;
			}
			set
			{
				this._DueDate = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
		[PXDBCury(typeof(ARDunningLetterDetail.curyID))]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
		[PXDBCury(typeof(ARDunningLetterDetail.curyID))]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXDefault("")]
		[PXSelector(typeof(PX.Objects.CM.Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Currency ID")]
		public virtual String CuryID
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
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Original Document Amount")]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;
		[PXDBBaseCury()]
		[PXUIField(DisplayName="Outstanding Balance")]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region Overdue
		public abstract class overdue : PX.Data.IBqlField
		{
		}
		protected Boolean? _Overdue;
		[PXDBBool()]
		[PXDefault(true)]
		public virtual Boolean? Overdue
		{
			get
			{
				return this._Overdue;
			}
			set
			{
				this._Overdue = value;
			}
		}
		#endregion
		#region OverdueBal
		public abstract class overdueBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _OverdueBal;
		[PXBaseCury]
		[PXDBCalced(typeof(Switch<Case<Where<overdue, Equal<True>>, docBal>, CS.decimal0>), typeof(decimal))]
		[PXUIField(DisplayName="Overdue Balance")]
		public virtual Decimal? OverdueBal
		{
			get
			{
				return this._OverdueBal;
			}
			set
			{
				this._OverdueBal = value;
			}
		}
		#endregion
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool()]
		[PXDBDefault(typeof(ARDunningLetter.voided))]     
		public virtual Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDBDefault(typeof(ARDunningLetter.released))]    
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
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
		#region DunningLetterLevel
		public abstract class dunningLetterLevel : PX.Data.IBqlField
		{
		}
		protected Int32? _DunningLetterLevel;
		[PXDBInt()]
		[PXDefault()]
		[PXUIField(DisplayName = "Dunning Level")]
		public virtual Int32? DunningLetterLevel
		{
			get
			{
				return this._DunningLetterLevel;
			}
			set
			{
				this._DunningLetterLevel = value;
			}
		}
		#endregion
	}
}
