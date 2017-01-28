using System;
using PX.Data;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;

namespace PX.Objects.CR
{

	[Serializable]
	[PXCacheName(Messages.Address)]
	public partial class Address : PX.Data.IBqlTable, IAddressBase, IValidatedAddress, IPXSelectable
	{
		#region Selected
		public abstract class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Service)]
		public virtual bool? Selected { get; set; }
		#endregion
		#region AddressID
		public abstract class addressID : PX.Data.IBqlField
		{
		}
		protected Int32? _AddressID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Address ID", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? AddressID
		{
			get
			{
				return this._AddressID;
			}
			set
			{
				this._AddressID = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt(IsKey = false)]
		[PXDBLiteDefault(typeof(BAccount.bAccountID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Business Account ID", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		[PXParent(typeof(Select<BAccount,
			Where<BAccount.bAccountID,
				Equal<Current<Address.bAccountID>>, 
			And<BAccount.type, NotEqual<BAccountType.combinedType>>>>)
			)]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXDBInt()]
		[PXDefault(0)]
		[AddressRevisionID()]
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
		#region AddressType
		public abstract class addressType : PX.Data.IBqlField
		{
		}
		protected String _AddressType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(AddressTypes.BusinessAddress)]
		[AddressTypes.List()]
		[PXUIField(DisplayName = "AddressType", Visibility = PXUIVisibility.SelectorVisible)]
		[PXMassMergableField]
		public virtual String AddressType
		{
			get
			{
				return this._AddressType;
			}
			set
			{
				this._AddressType = value;
			}
		}
		#endregion
		#region DisplayName
		public abstract class displayName : PX.Data.IBqlField
		{
		}
		[PXString]
		[PXUIField(DisplayName = "Address", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(SmartJoin<Space, Address.addressLine1, Address.addressLine2, Address.addressLine3>))]
		public virtual String DisplayName { get; set; }
		
		#endregion
		#region AddressLine1
		public abstract class addressLine1 : PX.Data.IBqlField
		{
		}
		protected String _AddressLine1;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Address Line 1", Visibility = PXUIVisibility.SelectorVisible)]
		[PXMassMergableField]
		public virtual String AddressLine1
		{
			get
			{
				return this._AddressLine1;
			}
			set
			{
				this._AddressLine1 = value;
			}
		}
		#endregion
		#region AddressLine2
		public abstract class addressLine2 : PX.Data.IBqlField
		{
		}
		protected String _AddressLine2;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Address Line 2")]
		[PXMassMergableField]
		public virtual String AddressLine2
		{
			get
			{
				return this._AddressLine2;
			}
			set
			{
				this._AddressLine2 = value;
			}
		}
		#endregion
		#region AddressLine3
		public abstract class addressLine3 : PX.Data.IBqlField
		{
		}
		protected String _AddressLine3;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Address Line 3")]
		[PXMassMergableField]
		public virtual String AddressLine3
		{
			get
			{
				return this._AddressLine3;
			}
			set
			{
				this._AddressLine3 = value;
			}
		}
		#endregion
		#region City
		public abstract class city : PX.Data.IBqlField
		{
		}
		protected String _City;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "City", Visibility = PXUIVisibility.SelectorVisible)]
		[PXMassMergableField]
		public virtual String City
		{
			get
			{
				return this._City;
			}
			set
			{
				this._City = value;
			}
		}
		#endregion
		#region CountryID
		public abstract class countryID : PX.Data.IBqlField
		{
		}
		protected String _CountryID;
		[PXDefault(typeof(Search<GL.Branch.countryID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Country")]
		[PXSelector(typeof(Search<Country.countryID>), DescriptionField = typeof(Country.description), CacheGlobal = true)]
		[PXMassMergableField]
		public virtual String CountryID
		{
			get
			{
				return this._CountryID;
			}
			set
			{
				this._CountryID = value;
			}
		}
		#endregion
		#region State
		public abstract class state : PX.Data.IBqlField
		{
		}
		protected String _State;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[State(typeof(Address.countryID), DescriptionField = typeof(State.name))]
		[PXMassMergableField]
		public virtual String State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
			}
		}
		#endregion
		#region PostalCode
		public abstract class postalCode : PX.Data.IBqlField
		{
		}
		protected String _PostalCode;
		[PXDBString(20)]
		[PXUIField(DisplayName = "Postal Code")]
		[PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), countryIdField: typeof(Address.countryID))]
		[PXDynamicMask(typeof(Search<Country.zipCodeMask, Where<Country.countryID, Equal<Current<Address.countryID>>>>))]
		[PXMassMergableField]
		public virtual String PostalCode
		{
			get
			{
				return this._PostalCode;
			}
			set
			{
				this._PostalCode = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(Address.displayName))]
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
		#region IsValidated
		public abstract class isValidated : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsValidated;

		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBBool()]
		[CS.ValidatedAddress()]
		[PXUIField(DisplayName = "Validated", Enabled = false, FieldClass = CS.Messages.ValidateAddress)]
		public virtual Boolean? IsValidated
		{
			get
			{
				return this._IsValidated;
			}
			set
			{
				this._IsValidated = value;
			}
		}
		#endregion
        #region TaxLocationCode
        public abstract class taxLocationCode : PX.Data.IBqlField
        {
        }
        protected String _TaxLocationCode;
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Location Code", FieldClass = PR.PRSetup.PayrollFieldClass)]
        [PXMassMergableField]
        public virtual String TaxLocationCode
        {
            get
            {
                return this._TaxLocationCode;
            }
            set
            {
                this._TaxLocationCode = value;
            }
        }
        #endregion
        #region TaxMunicipalCode
        public abstract class taxMunicipalCode : PX.Data.IBqlField
        {
        }
        protected String _TaxMunicipalCode;
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Municipal Code", FieldClass = PR.PRSetup.PayrollFieldClass)]
        [PXMassMergableField]
        public virtual String TaxMunicipalCode
        {
            get
            {
                return this._TaxMunicipalCode;
            }
            set
            {
                this._TaxMunicipalCode = value;
            }
        }
        #endregion
        #region TaxSchoolCode
        public abstract class taxSchoolCode : PX.Data.IBqlField
        {
        }
        protected String _TaxSchoolCode;
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax School Code", FieldClass = PR.PRSetup.PayrollFieldClass)]
        [PXMassMergableField]
        public virtual String TaxSchoolCode
        {
            get
            {
                return this._TaxSchoolCode;
            }
            set
            {
                this._TaxSchoolCode = value;
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

		#region AddressTypes
		public class AddressTypes
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
					 new string[] { BusinessAddress, HomeAddress, OtherAddress },
					 new string[] { Messages.BusinessAddress, Messages.HomeAddress, Messages.OtherAddress }) { ; }
			}

			public const string BusinessAddress = "BS";
			public const string HomeAddress = "HM";
			public const string OtherAddress = "UN";

			public class businessAddress : Constant<string>
			{
				public businessAddress() : base(BusinessAddress) { ;}
			}
			public class homeAddress : Constant<string>
			{
				public homeAddress() : base(HomeAddress) { ;}
			}
			public class otherAddress : Constant<string>
			{
				public otherAddress() : base(OtherAddress) { ;}
			}
		}
		#endregion
		
	}
}
