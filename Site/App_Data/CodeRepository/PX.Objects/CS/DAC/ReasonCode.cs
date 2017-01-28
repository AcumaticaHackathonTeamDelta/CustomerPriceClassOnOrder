using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace PX.Objects.CS
{
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ReasonCodeMaint))]
	[PXCacheName(Messages.ReasonCode)]
	public partial class ReasonCode : PX.Data.IBqlTable
	{
		#region ReasonCodeID
		public abstract class reasonCodeID : PX.Data.IBqlField
		{
			public const int Length = 20;
		}
		protected String _ReasonCodeID;
		[PXDefault()]
		[PXDBString(reasonCodeID.Length, IsKey = true, InputMask = ">aaaaaaaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Reason Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID>))]
		public virtual String ReasonCodeID
		{
			get
			{
				return this._ReasonCodeID;
			}
			set
			{
				this._ReasonCodeID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(60, IsUnicode = true)]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(DescriptionField = typeof(Account.description), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(ReasonCode.accountID), DescriptionField = typeof(Sub.description), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region Usage
		public abstract class usage : PX.Data.IBqlField
		{
		}
		protected String _Usage;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Usage", Visibility=PXUIVisibility.SelectorVisible)]
		[ReasonCodeUsages.List()]
		[PXDefault(ReasonCodeUsages.Sales)]
		public virtual String Usage
		{
			get
			{
				return this._Usage;
			}
			set
			{
				this._Usage = value;
			}
		}
		#endregion

		#region SubMask
		public abstract class subMask : PX.Data.IBqlField
		{
		}
		protected String _SubMask;
		[PXDefault()]
		[PXDBString(IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Combine Sub From Combined", Visible=false)]//required for Copy-Paste
		public virtual String SubMask
		{
			get
			{
				return this._SubMask;
			}
			set
			{
				this._SubMask = value;
			}
		}
		#endregion
		#region SubMaskInventory
		public abstract class subMaskInventory : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[PXString(30, IsUnicode = true, InputMask = "")]
		[IN.ReasonCodeSubAccountMask(DisplayName = "Combine Sub from")]
		public virtual String SubMaskInventory
		{
			get
			{
				return this._SubMask;
			}
			set
			{
				this._SubMask = value;
			}
		}
		#endregion
		#region SubMaskFinance
		public abstract class subMaskFinance : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[PXString(30, IsUnicode = true, InputMask = "")]
		[CS.ReasonCodeSubAccountMask(DisplayName = "Combine Sub from")]
		public virtual String SubMaskFinance
		{
			get
			{
				return this._SubMask;
			}
			set
			{
				this._SubMask = value;
			}
		}
		#endregion

		#region SalesAcctID
		public abstract class salesAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesAcctID;
		[Account(DisplayName = "Sales Account", DescriptionField = typeof(Account.description), Visibility = PXUIVisibility.SelectorVisible)]
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
		[SubAccount(typeof(ReasonCode.salesAcctID), DisplayName = "Sales Sub.", DescriptionField = typeof(Sub.description), Visibility = PXUIVisibility.SelectorVisible)]
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

	public class ReasonCodeUsages
	{
		public const string Sales = "S";
		public const string CreditWriteOff = "C";
		public const string BalanceWriteOff = "B";
		public const string Issue = INDocType.Issue;
		public const string Receipt = INDocType.Receipt;
		public const string Transfer = INDocType.Transfer;
		public const string Adjustment = INDocType.Adjustment;
		public const string Disassembly = INDocType.Disassembly;

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { Sales, CreditWriteOff, BalanceWriteOff, Issue, Receipt, Adjustment, Transfer, Disassembly },
			new string[] { Messages.Sales, Messages.CreditWriteOff, Messages.BalanceWriteOff, IN.Messages.Issue, IN.Messages.Receipt, IN.Messages.Adjustment, IN.Messages.Transfer, IN.Messages.Disassembly }) { }
		}

		public class sales : Constant<string>
		{
			public sales() : base(Sales) {}
		}

		public class creditWriteOff : Constant<string>
		{
			public creditWriteOff() : base(CreditWriteOff) {}
		}

		public class balanceWriteOff : Constant<string>
		{
			public balanceWriteOff() : base(BalanceWriteOff) {}
		}

		public class issue : Constant<string>
		{
			public issue() : base(Issue) {}
		}

		public class receipt : Constant<string>
		{
			public receipt() : base(Receipt) {}
		}

		public class transfer : Constant<string>
		{
			public transfer() : base(Transfer) {}
		}

		public class adjustment : Constant<string>
		{
			public adjustment() : base(Adjustment) {}
		}

		public class disassembly : Constant<string>
		{
			public disassembly() : base(Disassembly) {}
		}

	}


}
