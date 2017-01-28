using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.AR
{
	[Serializable()]
	[PXCacheName(Messages.ARContact)]
	public partial class ARContact : IBqlTable, IContact
	{
		#region ContactID
		public abstract class contactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContactID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Contact ID", Visible = false)]
		public virtual Int32? ContactID
		{
			get
			{
				return this._ContactID;
			}
			set
			{
				this._ContactID = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDBInt()]
		[PXDBDefault(typeof(ARRegister.customerID))]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		public virtual Int32? BAccountID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region CustomerContactID
		public abstract class customerContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerContactID;
		[PXDBInt()]
		public virtual Int32? CustomerContactID
		{
			get
			{
				return this._CustomerContactID;
			}
			set
			{
				this._CustomerContactID = value;
			}
		}
		public virtual Int32? BAccountContactID
		{
			get
			{
				return this._CustomerContactID;
			}
			set
			{
				this._CustomerContactID = value;
			}
		}
		#endregion
		#region IsDefaultContact
		public abstract class isDefaultContact : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsDefaultContact;
		[PXDBBool()]
		[PXUIField(DisplayName = "Default Customer Contact", Visibility = PXUIVisibility.Visible)]
		[PXDefault(true)]
		public virtual Boolean? IsDefaultContact
		{
			get
			{
				return this._IsDefaultContact;
			}
			set
			{
				this._IsDefaultContact = value;
			}
		}
		#endregion
		#region OverrideContact
		public abstract class overrideContact : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXUIField(DisplayName = "Override Contact", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? OverrideContact
		{
			[PXDependsOnFields(typeof(isDefaultContact))]
			get
			{
				return (this._IsDefaultContact == null ? this._IsDefaultContact : this._IsDefaultContact == false);
			}
			set
			{
				this._IsDefaultContact = (value == null ? value : value == false);
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXDBInt()]
		[PXDefault()]
		public virtual Int32? RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region Title
		public abstract class title : PX.Data.IBqlField
		{
		}
		protected String _Title;
		[PXDBString(50, IsUnicode = true)]
		[CR.Titles]
		[PXUIField(DisplayName = "Title")]
		public virtual String Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				this._Title = value;
			}
		}
		#endregion
		#region Salutation
		public abstract class salutation : PX.Data.IBqlField
		{
		}
		protected String _Salutation;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Attention", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Salutation
		{
			get
			{
				return this._Salutation;
			}
			set
			{
				this._Salutation = value;
			}
		}
		#endregion
		#region FullName
		public abstract class fullName : PX.Data.IBqlField
		{
		}
		protected String _FullName;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Business Name")]
		public virtual String FullName
		{
			get
			{
				return this._FullName;
			}
			set
			{
				this._FullName = value;
			}
		}
		#endregion
		#region Email
		public abstract class email : PX.Data.IBqlField
		{
		}
		protected String _Email;
		[PXDBEmail]
		[PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
			}
		}
		#endregion
		#region Fax
		public abstract class fax : PX.Data.IBqlField
		{
		}
		protected String _Fax;
		[PXDBString(50)]
		[PXUIField(DisplayName = "Fax")]
		[CR.PhoneValidation()]
		public virtual String Fax
		{
			get
			{
				return this._Fax;
			}
			set
			{
				this._Fax = value;
			}
		}
		#endregion
		#region FaxType
		public abstract class faxType : PX.Data.IBqlField
		{
		}
		protected String _FaxType;
		[PXDBString(3)]
		[PXDefault(CR.PhoneTypesAttribute.BusinessFax, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Fax")]
		[CR.PhoneTypes]
		public virtual String FaxType
		{
			get
			{
				return this._FaxType;
			}
			set
			{
				this._FaxType = value;
			}
		}
		#endregion
		#region Phone1
		public abstract class phone1 : PX.Data.IBqlField
		{
		}
		protected String _Phone1;
		[PXDBString(50)]
		[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
		[CR.PhoneValidation()]
		public virtual String Phone1
		{
			get
			{
				return this._Phone1;
			}
			set
			{
				this._Phone1 = value;
			}
		}
		#endregion
		#region Phone1Type
		public abstract class phone1Type : PX.Data.IBqlField
		{
		}
		protected String _Phone1Type;
		[PXDBString(3)]
		[PXDefault(CR.PhoneTypesAttribute.Business1, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 1")]
		[CR.PhoneTypes]
		public virtual String Phone1Type
		{
			get
			{
				return this._Phone1Type;
			}
			set
			{
				this._Phone1Type = value;
			}
		}
		#endregion
		#region Phone2
		public abstract class phone2 : PX.Data.IBqlField
		{
		}
		protected String _Phone2;
		[PXDBString(50)]
		[PXUIField(DisplayName = "Phone 2")]
		[CR.PhoneValidation()]
		public virtual String Phone2
		{
			get
			{
				return this._Phone2;
			}
			set
			{
				this._Phone2 = value;
			}
		}
		#endregion
		#region Phone2Type
		public abstract class phone2Type : PX.Data.IBqlField
		{
		}
		protected String _Phone2Type;
		[PXDBString(3)]
		[PXDefault(CR.PhoneTypesAttribute.Business2, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 2")]
		[CR.PhoneTypes]
		public virtual String Phone2Type
		{
			get
			{
				return this._Phone2Type;
			}
			set
			{
				this._Phone2Type = value;
			}
		}
		#endregion
		#region Phone3
		public abstract class phone3 : PX.Data.IBqlField
		{
		}
		protected String _Phone3;
		[PXDBString(50)]
		[PXUIField(DisplayName = "Phone 3")]
		[CR.PhoneValidation()]
		public virtual String Phone3
		{
			get
			{
				return this._Phone3;
			}
			set
			{
				this._Phone3 = value;
			}
		}
		#endregion
		#region Phone3Type
		public abstract class phone3Type : PX.Data.IBqlField
		{
		}
		protected String _Phone3Type;
		[PXDBString(3)]
		[PXDefault(CR.PhoneTypesAttribute.Home, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 3")]
		[CR.PhoneTypes]
		public virtual String Phone3Type
		{
			get
			{
				return this._Phone3Type;
			}
			set
			{
				this._Phone3Type = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		[PXUIField(DisplayName = "Created By")]
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
		[PXUIField(DisplayName = "Last Modified By")]
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
		[PXUIField(DisplayName = "Last Modified Date")]
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
