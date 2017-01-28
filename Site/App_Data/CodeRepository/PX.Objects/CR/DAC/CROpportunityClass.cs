using PX.SM;
using System;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.CR
{
	[PXCacheName(Messages.OpportunityClass)]
	[PXPrimaryGraph(typeof(CROpportunityClassMaint))]
	[Serializable]
	public partial class CROpportunityClass : PX.Data.IBqlTable
	{
		#region CROpportunityClassID
		public abstract class cROpportunityClassID : PX.Data.IBqlField
		{
		}
		protected String _CROpportunityClassID;
		[PXSelector(typeof(CROpportunityClass.cROpportunityClassID))]
		[PXUIField(DisplayName = "Opportunity Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		public virtual String CROpportunityClassID
		{
			get
			{
				return this._CROpportunityClassID;
			}
			set
			{
				this._CROpportunityClassID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(250, IsUnicode = true)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region DefaultEMailAccount
		public abstract class defaultEMailAccountID : PX.Data.IBqlField { }

		[PXSelector(typeof(EMailAccount.emailAccountID), typeof(EMailAccount.description), DescriptionField = typeof(EMailAccount.description))]
		[PXUIField(DisplayName = "Default Email Account")]
		[PXDBInt]
		public virtual int? DefaultEMailAccountID { get; set; }
		#endregion
		#region DiscountAcctID
		public abstract class discountAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscountAcctID;
		[Account(DisplayName = "Cash Discount Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? DiscountAcctID
		{
			get
			{
				return this._DiscountAcctID;
			}
			set
			{
				this._DiscountAcctID = value;
			}
		}
		#endregion
		#region DiscountSubID
		public abstract class discountSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscountSubID;
		[SubAccount(typeof(CROpportunityClass.discountAcctID), DisplayName = "Cash Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? DiscountSubID
		{
			get
			{
				return this._DiscountSubID;
			}
			set
			{
				this._DiscountSubID = value;
			}
		}
		#endregion
		#region IsInternal

		public abstract class isInternal : PX.Data.IBqlField { }
		protected Boolean? _IsInternal;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Internal", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsInternal
		{
			get
			{
				return this._IsInternal;
			}
			set
			{
				this._IsInternal = value;
			}
		}

		#endregion

        #region ShowContactActivities
        public abstract class showContactActivities : PX.Data.IBqlField { }
        protected Boolean? _showContactActivities;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Show Contact Activities")]
        public virtual Boolean? ShowContactActivities
        {
            get
            {
                return this._showContactActivities;
            }
            set
            {
                this._showContactActivities = value;
            }
        }
        #endregion
        #region NoteID
        public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
        #region TargetBAccountClassID
        public abstract class targetBAccountClassID : PX.Data.IBqlField
        {
        }
        protected String _TargetBAccountClassID;
        [PXSelector(typeof(CRCustomerClass.cRCustomerClassID))]
        [PXUIField(DisplayName = "Target Account Class ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBString(10, IsUnicode = true)]
        public virtual String TargetBAccountClassID
        {
            get
            {
                return this._TargetBAccountClassID;
            }
            set
            {
                this._TargetBAccountClassID = value;
            }
        }
        #endregion

        #region System Columns
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
}
