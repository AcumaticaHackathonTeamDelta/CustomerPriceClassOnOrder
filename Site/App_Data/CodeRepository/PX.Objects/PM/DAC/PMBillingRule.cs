using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.GL;
	using PX.Objects.CS;
	using System.Collections.Generic;

	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(BillingMaint))]
	public partial class PMBillingRule : PX.Data.IBqlTable
	{
		#region BillingID
		public abstract class billingID : PX.Data.IBqlField
		{
		}
		protected String _BillingID;
		[PXDBString(PMBilling.billingID.Length, IsKey = true, IsUnicode = true)]
		[PXDefault(typeof(PMBilling.billingID))]
		[PXParent(typeof(Select<PMBilling, Where<PMBilling.billingID, Equal<Current<PMBillingRule.billingID>>>>))]
		public virtual String BillingID
		{
			get
			{
				return this._BillingID;
			}
			set
			{
				this._BillingID = value;
			}
		}
		#endregion
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDefault]
		[AccountGroup(IsKey=true)]
		public virtual Int32? AccountGroupID
		{
			get
			{
				return this._AccountGroupID;
			}
			set
			{
				this._AccountGroupID = value;
			}
		}
		#endregion
		#region WipAccountGroupID
		public abstract class wipAccountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _WipAccountGroupID;
		[AccountGroup(DisplayName="WIP Account Group")]
		public virtual Int32? WipAccountGroupID
		{
			get
			{
				return this._WipAccountGroupID;
			}
			set
			{
				this._WipAccountGroupID = value;
			}
		}
		#endregion
		#region InvoiceDescription
		public abstract class invoiceDescription : PX.Data.IBqlField
		{
		}
		protected String _InvoiceDescription;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Invoice Description")]
		public virtual String InvoiceDescription
		{
			get
			{
				return this._InvoiceDescription;
			}
			set
			{
				this._InvoiceDescription = value;
			}
		}
		#endregion
		#region TaxCode
		public abstract class accountSource : PX.Data.IBqlField
		{
		}
		protected String _AccountSource;
		[PXDBString(1, IsFixed = true)]
		[PMAccountSource.List()]
		[PXDefault(PMAccountSource.BillingRule)]
		[PXUIField(DisplayName = "Account Source", Required = true)]
		public virtual String AccountSource
		{
			get
			{
				return this._AccountSource;
			}
			set
			{
				this._AccountSource = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
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
		#region SubMask
		public abstract class subMask : PX.Data.IBqlField
		{
		}
		protected String _SubMask;
		[PMBillSubAccountMaskAttribute]
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
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(PMBillingRule.accountID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
		#region IncludeNonBillable
		public abstract class includeNonBillable : PX.Data.IBqlField
		{
		}
		protected Boolean? _IncludeNonBillable;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Include Non-Billable")]
		public virtual Boolean? IncludeNonBillable
		{
			get
			{
				return this._IncludeNonBillable;
			}
			set
			{
				this._IncludeNonBillable = value;
			}
		}
		#endregion
        #region LimitQty
        public abstract class limitQty : PX.Data.IBqlField
        {
        }
        protected Boolean? _LimitQty;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Limit Qty.")]
        public virtual Boolean? LimitQty
        {
            get
            {
                return this._LimitQty;
            }
            set
            {
                this._LimitQty = value;
            }
        }
        #endregion
        #region LimitAmt
        public abstract class limitAmt : PX.Data.IBqlField
        {
        }
        protected Boolean? _LimitAmt;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Limit Amount")]
        public virtual Boolean? LimitAmt
        {
            get
            {
                return this._LimitAmt;
            }
            set
            {
                this._LimitAmt = value;
            }
        }
        #endregion
		#region CapsAccountGroupID
		public abstract class capsAccountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _CapsAccountGroupID;
		[AccountGroup(DisplayName="Max. Limits Account Group")]
		public virtual Int32? CapsAccountGroupID
		{
			get
			{
				return this._CapsAccountGroupID;
			}
			set
			{
				this._CapsAccountGroupID = value;
			}
		}
		#endregion
		#region OverflowAccountGroupID
		public abstract class overflowAccountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _OverflowAccountGroupID;
		[AccountGroup(typeof(Where<PMAccountGroup.type, Equal<PMAccountType.offBalance>>), DisplayName = "Over the Limits Account Group")]
		public virtual Int32? OverflowAccountGroupID
		{
			get
			{
				return this._OverflowAccountGroupID;
			}
			set
			{
				this._OverflowAccountGroupID = value;
			}
		}
		#endregion
		#region CopyNotes
		public abstract class copyNotes : PX.Data.IBqlField
		{
		}
		protected Boolean? _CopyNotes;
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName = "Copy Notes", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? CopyNotes
		{
			get
			{
				return this._CopyNotes;
			}
			set
			{
				this._CopyNotes = value;
			}
		}
		#endregion

		#region System Columns
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
		#endregion
	}

	public static class PMAccountSource
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { None, BillingRule, Project, Task, InventoryItem, Customer, Resource },
				new string[] { Messages.AccountSource_None, Messages.AccountSource_BillingRule, Messages.AccountSource_Project, Messages.AccountSource_Task, Messages.AccountSource_InventoryItem, Messages.AccountSource_Customer, Messages.AccountSource_Resource }) { ; }
		}

		public class RecurentListAttribute : PXStringListAttribute
		{
			public RecurentListAttribute()
				: base(
				new string[] { None, RecurringBillingItem, Project, Task, InventoryItem, Customer },
				new string[] { Messages.AccountSource_None, Messages.AccountSource_RecurentBillingItem, Messages.AccountSource_Project, Messages.AccountSource_Task, Messages.AccountSource_InventoryItem, Messages.AccountSource_Customer }) { ; }
		}

		public const string None = "N";
		public const string BillingRule = "B";
		public const string RecurringBillingItem = "B";
		public const string Project = "P";
		public const string Task = "T";
		public const string InventoryItem = "I";
		public const string Customer = "C";
		public const string Resource = "E";
	}
}
