using System;
using PX.Data;
using PX.Data.EP;
using PX.TM;

namespace PX.Objects.CR
{
	[Serializable]
	[CRCacheIndependentPrimaryGraph(typeof(CRMarketingListMaint),
		typeof(Select<CRMarketingList, 
			Where<CRMarketingList.marketingListID, Equal<Current<CRMarketingList.marketingListID>>>>))]
	[PXCacheName(Messages.MailList)]
	public partial class CRMarketingList : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion

		#region MarketingListID
		public abstract class marketingListID : PX.Data.IBqlField
		{
		}
		protected Int32? MarketingListId;
		[PXDBIdentity()]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? MarketingListID
		{
			get
			{
				return this.MarketingListId;
			}
			set
			{
				this.MarketingListId = value;
			}
		}
		#endregion
		#region MailListCode
		public abstract class mailListCode : PX.Data.IBqlField
		{
		}
		protected String _MailListCode;
		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault]
		[PXUIField(DisplayName = "Marketing List ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDimensionSelector("MLISTCD", typeof(Search<CRMarketingList.marketingListID>), typeof(CRMarketingList.mailListCode))]
		public virtual String MailListCode
		{
			get
			{
				return this._MailListCode;
			}
			set
			{
				this._MailListCode = value;
			}
		}
		#endregion
		#region Name
		public abstract class name : PX.Data.IBqlField
		{
		}
		protected String _Name;
		[PXDBString(50, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBText(IsUnicode = true)]
		[PXDefault("", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active")]
		public virtual Boolean? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXUIField(DisplayName = "Workgroup")]
		[PXCompanyTreeSelector]
		public virtual int? WorkgroupID
		{
			get
			{
				return this._WorkgroupID;
			}
			set
			{
				this._WorkgroupID = value;
			}
		}
		#endregion
		#region OwnerID
		public abstract class ownerID : IBqlField { }
		protected Guid? _OwnerID;
		[PXDBGuid()]
		[PXOwnerSelector(typeof(CRMarketingList.workgroupID))]
		[PXUIField(DisplayName = "Owner")]
		public virtual Guid? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion
		#region Method

		public abstract class method : IBqlField { }

		private string _method;

		[PXDBString(1, IsFixed = true)]
		[CRContactMethods]
		[PXDefault(CRContactMethodsAttribute.Any)]
		[PXUIField(DisplayName = "Contact Method")]
		public virtual String Method
		{
			get { return _method ?? CRContactMethodsAttribute.Any; }
			set { _method = value; }
		}

		#endregion
		#region IsDynamic

		public abstract class isDynamic : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Dynamic List")]
		public virtual Boolean? IsDynamic { get; set; }

		#endregion
		#region IsSelfManaged

		public abstract class isSelfManaged : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Self-Managed")]
		public virtual Boolean? IsSelfManaged { get; set; }

		#endregion
		#region NoFax
		public abstract class noFax : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Fax")]
		public virtual bool? NoFax { get; set; }
		#endregion
		#region NoMail
		public abstract class noMail : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Mail")]
		public virtual bool? NoMail { get; set; }
		#endregion
		#region NoMarketing
		public abstract class noMarketing : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "No Marketing")]
		public virtual bool? NoMarketing { get; set; }
		#endregion
		#region NoCall
		public abstract class noCall : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Call")]
		public virtual bool? NoCall { get; set; }
		#endregion
		#region NoEMail
		public abstract class noEMail : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Email")]
		public virtual bool? NoEMail { get; set; }
		#endregion
		#region NoMassMail
		public abstract class noMassMail : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "No Mass Mail")]
		public virtual bool? NoMassMail { get; set; }
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(new Type[0])]
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
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date")]
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
        [PXUIField(DisplayName = "Modified Date")]
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
