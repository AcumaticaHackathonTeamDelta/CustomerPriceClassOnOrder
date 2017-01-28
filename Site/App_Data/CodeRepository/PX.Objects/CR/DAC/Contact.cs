using PX.Data;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using System;
using PX.TM;
using PX.SM;
using PX.Objects.EP;

namespace PX.Objects.CR
{
	[Serializable]
	[CRCacheIndependentPrimaryGraph(
		typeof(EmployeeMaint),
		typeof(Where<Contact.contactType, Equal<ContactTypesAttribute.employee>, And<Contact.contactID, Less<int0>>>))]
	[CRCacheIndependentPrimaryGraph(
		typeof(EmployeeMaint),
		typeof(Select<EPEmployee,
			Where<EPEmployee.defContactID, Equal<Current<Contact.contactID>>>>))]
	[CRCacheIndependentPrimaryGraph(
		typeof(BusinessAccountMaint),
		typeof(Select<BAccount,
			Where<BAccount.defContactID, Equal<Current<Contact.contactID>>, And<BAccount.type, NotEqual<ContactTypesAttribute.employee>>>>))]
	[CRCacheIndependentPrimaryGraph(
		typeof(ContactMaint),
		typeof(Select<Contact,
		  Where<Contact.contactID, IsNull, And<Contact.contactType, Equal<ContactTypesAttribute.person>,
			Or<Where<Contact.contactID, Equal<Current<Contact.contactID>>,
					 And<Where<Contact.contactType, Equal<ContactTypesAttribute.person>,
								Or<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>>>>>>>))]
	[CRCacheIndependentPrimaryGraph(
		typeof(LeadMaint),
		typeof(Select<Contact,
			Where<Contact.contactID, Equal<Current<Contact.contactID>>,
				And<Contact.contactType, Equal<ContactTypesAttribute.lead>>>>))]
	[CRContactCacheName(Messages.LeadContact)]
	[CREmailContactsView(typeof(Select2<Contact, 
		LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>>, 
		Where<Contact.contactID, Equal<Optional<Contact.contactID>>,
			  Or2<Where<Optional<Contact.bAccountID>, IsNotNull, And<BAccount.bAccountID, Equal<Optional<Contact.bAccountID>>>>,
				Or<Contact.contactType, Equal<ContactTypesAttribute.employee>>>>>))]
	[PXEMailSource]//NOTE: for assignment map
	public partial class Contact : IBqlTable, IContactBase, IAssign, IPXSelectable, CRDefaultMailToAttribute.IEmailMessageTarget
	{
		#region Selected
		public abstract class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Service)]
		public virtual bool? Selected { get; set; }
		#endregion

		#region DisplayName
		public abstract class displayName : PX.Data.IBqlField { }

		[PXUIField(DisplayName = "Display Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDependsOnFields(typeof(Contact.lastName), typeof(Contact.firstName), typeof(Contact.midName), typeof(Contact.title))]
		[ContactDisplayName(typeof(Contact.lastName), typeof(Contact.firstName), typeof(Contact.midName), typeof(Contact.title), true)]
		[PXFieldDescription]
		[PXDefault]
		[PXUIRequired(typeof(Where<Contact.contactType, Equal<ContactTypesAttribute.lead>, Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>))]
		[PXNavigateSelector(typeof(Search<Contact.displayName,
			Where<Contact.contactType, Equal<ContactTypesAttribute.
			lead>,
				Or<Contact.contactType, Equal<ContactTypesAttribute.person>,
				Or<Contact.contactType, Equal<ContactTypesAttribute.employee>>>>>))]
		public virtual String DisplayName { get;set; }

		#endregion

		#region ContactID
		public abstract class contactID : IBqlField { }

		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Contact ID", Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? ContactID { get; set; }
		#endregion

		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField { }

		[PXDBInt]
		[PXDefault(0)]
		[AddressRevisionID()]
		public virtual Int32? RevisionID { get; set; }
		#endregion

		#region IsAddressSameAsMain
		public abstract class isAddressSameAsMain : PX.Data.IBqlField { }

		[PXBool]
		[PXUIField(DisplayName = "Same As In Account")]
		public virtual bool? IsAddressSameAsMain { get; set; }
		#endregion

		#region DefAddressID
		public abstract class defAddressID : IBqlField { }
	    protected Int32? _defAddressID;
		[PXDBInt]
		[PXSelector(typeof(Address.addressID), DirtyRead = true)]
		[PXUIField(DisplayName = "Address")]
        [PXDBChildIdentity(typeof(Address.addressID))]
		public virtual Int32? DefAddressID 
        {
		    get
		    {
		        return _defAddressID;
		    }
		    set
		    {
		        _defAddressID = value;
		    }
        }
		#endregion

		#region Title
		public abstract class title : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[Titles]
		[PXUIField(DisplayName = "Title")]
		[PXMassMergableField]
		public virtual String Title { get; set; }
		#endregion

		#region FirstName
		public abstract class firstName : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "First Name")]
		[PXMassMergableField]
        public virtual String FirstName { get; set; }
		#endregion

		#region MidName
		public abstract class midName : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Middle Name")]
		[PXMassMergableField]
		public virtual String MidName { get; set; }
		#endregion

		#region LastName
		public abstract class lastName : PX.Data.IBqlField { }

		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Last Name")]
		[CRLastNameDefault]
		[PXMassMergableField]
        public virtual String LastName { get; set; }
		#endregion

		#region Salutation
		public abstract class salutation : PX.Data.IBqlField { }
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Position", Visibility = PXUIVisibility.SelectorVisible)]
		[PXMassMergableField]
		[PXMassUpdatableField]
        public virtual String Salutation { get; set; }
		#endregion

		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField { }

		[PXDBInt]
		[CRContactBAccountDefault]
		[PXParent(typeof(Select<BAccount, 
			Where<BAccount.bAccountID, Equal<Current<Contact.bAccountID>>, 
			And<BAccount.type, NotEqual<BAccountType.combinedType>>>>))]
		[PXUIField(DisplayName = "Business Account")]
		[PXSelector(typeof(BAccount.bAccountID), SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName), DirtyRead = true)]
		[PXMassUpdatableField]
		public virtual Int32? BAccountID { get; set; }
		#endregion

		#region FullName
		public abstract class fullName : PX.Data.IBqlField { }
	    private string _fullName;

	    [PXMassMergableField]
	    [PXDBString(255, IsUnicode = true)]
	    [PXUIField(DisplayName = "Company Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
	    public virtual String FullName
	    {
	        get { return _fullName; }
	        set { _fullName = value; }
	    }
		#endregion

		#region ParentBAccountID
		public abstract class parentBAccountID : IBqlField { }
		[CustomerProspectVendor(DisplayName = "Parent Business Account")]
		[PXMassUpdatableField]
		public virtual Int32? ParentBAccountID { get; set; }
		#endregion

		#region EMail
		public abstract class eMail : PX.Data.IBqlField { }
        private string _eMail;

	    [PXDBEmail]
	    [PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
	    [PXMassMergableField]
	    [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]        
	    public virtual String EMail
	    {
	        get { return _eMail; }
	        set { _eMail = value != null ? value.Trim() : null; }
	    }

	    #endregion

		#region WebSite
		public abstract class webSite : PX.Data.IBqlField { }

		[PXDBWeblink]
		[PXUIField(DisplayName = "Web")]
		[PXMassMergableField]
		public virtual String WebSite { get; set; }
		#endregion

		#region Fax
		public abstract class fax : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Fax")]
		[PhoneValidation()]
		[PXMassMergableField]
		public virtual String Fax { get; set; }
		#endregion

		#region FaxType
		public abstract class faxType : PX.Data.IBqlField { }

		[PXDBString(3)]
		[PXDefault(PhoneTypesAttribute.BusinessFax, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Fax Type")]
		[PhoneTypes]
		[PXMassMergableField]
		public virtual String FaxType { get; set; }
		#endregion

		#region Phone1
		public abstract class phone1 : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
		[PhoneValidation()]
		[PXMassMergableField]
        public virtual String Phone1 { get; set; }
		#endregion

		#region Phone1Type
		public abstract class phone1Type : PX.Data.IBqlField { }

		[PXDBString(3)]
		[PXDefault(PhoneTypesAttribute.Business1, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 1 Type")]
		[PhoneTypes]
		[PXMassMergableField]
		public virtual String Phone1Type { get; set; }
		#endregion

		#region Phone2
		public abstract class phone2 : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Phone 2")]
		[PhoneValidation()]
		[PXMassMergableField]
		public virtual String Phone2 { get; set; }
		#endregion

		#region Phone2Type
		public abstract class phone2Type : PX.Data.IBqlField { }

		[PXDBString(3)]
		[PXDefault(PhoneTypesAttribute.Business2, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 2 Type")]
		[PhoneTypes]
		[PXMassMergableField]
		public virtual String Phone2Type { get; set; }
		#endregion

		#region Phone3
		public abstract class phone3 : PX.Data.IBqlField { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Phone 3")]
		[PhoneValidation()]
		[PXMassMergableField]
		public virtual String Phone3 { get; set; }
		#endregion

		#region Phone3Type
		public abstract class phone3Type : PX.Data.IBqlField { }

		[PXDBString(3)]
		[PXDefault(PhoneTypesAttribute.Home, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Phone 3 Type")]
		[PhoneTypes]
		[PXMassMergableField]
		public virtual String Phone3Type { get; set; }
		#endregion

		#region DateOfBirth
		public abstract class dateOfBirth : PX.Data.IBqlField { }

		[PXDBDate]
		[PXUIField(DisplayName = "Date Of Birth")]
		[PXMassMergableField]
		public virtual DateTime? DateOfBirth { get; set; }
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField { }
		[PXSearchable(SM.SearchCategory.CR, "{0} {1}", new Type[] { typeof(Contact.contactType), typeof(Contact.displayName) },
			new Type[] { typeof(Contact.eMail), typeof(Contact.phone1), typeof(Contact.phone2), typeof(Contact.phone3), typeof(Contact.webSite) },
			 WhereConstraint = typeof(Where<Current<Contact.contactType>, NotEqual<ContactTypesAttribute.bAccountProperty>, And<Current<Contact.contactType>, NotEqual<ContactTypesAttribute.employee>>>),
			 Line1Format = "{0}{1}{2}", Line1Fields = new Type[] { typeof(Contact.salutation), typeof(Contact.phone1), typeof(Contact.eMail) },
			 Line2Format = "{1}{2}{3}", Line2Fields = new Type[] { typeof(Contact.defAddressID), typeof(Address.displayName), typeof(Address.city), typeof(Address.state), typeof(Address.countryID) }
			)]
		[PXNote(
			DescriptionField = typeof(Contact.displayName),
			Selector = typeof(Search<Contact.contactID, Where<Contact.contactType, Equal<ContactTypesAttribute.person>, Or<Contact.contactType, Equal<ContactTypesAttribute.lead>>>>),
			ShowInReferenceSelector = true)]
		[PXTimeTagAttribute(typeof(Contact.noteID))]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region IsActive
		public abstract class isActive : PX.Data.IBqlField { }

		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive { get; set; }
		#endregion

		#region NoFax
		public abstract class noFax : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Fax")]
		[PXMassUpdatableField]
		public virtual bool? NoFax { get; set; }
		#endregion

		#region NoMail
		public abstract class noMail : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Mail")]
		[PXMassUpdatableField]
		public virtual bool? NoMail { get; set; }
		#endregion

		#region NoMarketing
		public abstract class noMarketing : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "No Marketing")]
		[PXMassUpdatableField]
		public virtual bool? NoMarketing { get; set; }
		#endregion

		#region NoCall
		public abstract class noCall : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Call")]
		[PXMassUpdatableField]
		public virtual bool? NoCall { get; set; }
		#endregion

		#region NoEMail
		public abstract class noEMail : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Do Not Email")]
		[PXMassUpdatableField]
		public virtual bool? NoEMail { get; set; }
		#endregion

		#region NoMassMail
		public abstract class noMassMail : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "No Mass Mail")]
		[PXMassUpdatableField]
		public virtual bool? NoMassMail { get; set; }
		#endregion

		#region Gender
		public abstract class gender : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Gender")]
		[Genders(typeof(Contact.title))]
		public virtual String Gender { get; set; }
		#endregion

		#region MaritalStatus
		public abstract class maritalStatus : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Marital Status")]
		[MaritalStatuses]
		public virtual String MaritalStatus { get; set; }
		#endregion

	    #region Anniversary
		public abstract class anniversary : IBqlField { }

		[PXDBDate]
		[PXUIField(DisplayName = "Wedding Date")]
		public virtual DateTime? Anniversary { get; set; }
		#endregion

		#region Spouse
		public abstract class spouse : IBqlField { }

		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Spouse/Partner Name")]
		public virtual String Spouse { get; set; }
		#endregion

		#region Img
		public abstract class img : IBqlField { }
		[PXUIField(DisplayName = "Image", Visibility = PXUIVisibility.Invisible)]
		[PXDBString(IsUnicode = true)]
		public string Img { get; set; }	
		#endregion

		#region Synchronize
		public abstract class synchronize : PX.Data.IBqlField { }

		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Synchronize")]
		public virtual bool? Synchronize { get; set; }
		#endregion

		#region ContactType
		public abstract class contactType : PX.Data.IBqlField { }

		[PXDBString(2, IsFixed = true)]
		[PXDefault(ContactTypesAttribute.Person)]
		[ContactTypes]
		[PXUIField(DisplayName = "Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ContactType { get; set; }
		#endregion

		#region ContactPriority
		public abstract class contactPriority : PX.Data.IBqlField { }
		[PXDBCalced(typeof(
			Switch<Case<Where<contactType, Equal<ContactTypesAttribute.bAccountProperty>>, ContactTypesAttribute.bAccountPriority,
				   Case<Where<Where<contactType, Equal<ContactTypesAttribute.salesPerson>>>, ContactTypesAttribute.salesPersonPriority,
				   Case<Where<Where<contactType, Equal<ContactTypesAttribute.employee>>>, ContactTypesAttribute.employeePriority,
				   Case<Where<Where<contactType, Equal<ContactTypesAttribute.person>>>, ContactTypesAttribute.personPriority,
				   Case<Where<Where<contactType, Equal<ContactTypesAttribute.lead>>>, ContactTypesAttribute.leadPriority>>>>>>
			), typeof(int))]
		[PXUIField(DisplayName = "Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? ContactPriority { get; set; }
		#endregion

		#region DuplicateStatus
		public abstract class duplicateStatus : IBqlField { }

		protected String _DuplicateStatus;
		[PXDBString(2, IsFixed = true)]
		[DuplicateStatusAttribute]
		[PXDefault(DuplicateStatusAttribute.NotValidated)]
		[PXUIField(DisplayName = "Duplicate", FieldClass = FeaturesSet.contactDuplicate.FieldClass)]
		public virtual String DuplicateStatus
		{
			get
			{
				return this._DuplicateStatus;
			}
			set
			{
				this._DuplicateStatus = value;
			}
		}
		#endregion

		#region DuplicateFound
		public abstract class duplicateFound : IBqlField { }
		[PXBool]
		[PXUIField(DisplayName = "Duplicate Found")]
		[PXDBCalced(typeof(Switch<Case<Where<Contact.duplicateStatus, Equal<DuplicateStatusAttribute.possibleDuplicated>,
			And<FeatureInstalled<FeaturesSet.contactDuplicate>>>, True>,False>), typeof(Boolean))]
		public virtual bool? DuplicateFound { get; set; }
		#endregion

		#region MajorStatus
		public abstract class majorStatus : IBqlField { }
	    private int? _majorStatus;
		[PXDBInt]
		[LeadMajorStatuses]
		[PXDefault(LeadMajorStatusesAttribute._RECORD, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Visible = false)]
		public virtual Int32? MajorStatus 
        {
		    get { return _majorStatus; } 
		    set { _majorStatus = value; } 
        }
		#endregion

		#region Status
		public abstract class status : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status")]
		[LeadStatuses]
		public virtual String Status { get; set; }

		#endregion

		#region Resolution
		public abstract class resolution : PX.Data.IBqlField { }

		[PXDBString(2, IsFixed = true)]
		[LeadResolutions]
		[PXUIField(DisplayName = "Reason")]
		public virtual String Resolution { get; set; }
		#endregion

		#region AssignDate
		public abstract class assignDate : IBqlField { }

		private DateTime? _assignDate;
		[PXUIField(DisplayName = "Assignment Date")]
		[AssignedDate(typeof(Contact.workgroupID), typeof(Contact.ownerID))]
		public virtual DateTime? AssignDate 
		{
			get { return _assignDate ?? CreatedDateTime; }
			set { _assignDate = value;}
		}
		#endregion

		#region QualificationDate
		public abstract class qualificationDate : IBqlField { }

		[PXDBDate(PreserveTime = true)]
		[PXUIField(DisplayName = "Qualification Date")]
		public virtual DateTime? QualificationDate { get; set; }
		#endregion

		#region ClassID
		public abstract class classID : IBqlField { }

		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Class ID")]
		[PXSelector(typeof(CRContactClass.classID), DescriptionField = typeof(CRContactClass.description), CacheGlobal = true)]
		[PXMassMergableField]
		[PXMassUpdatableField]
		public virtual String ClassID { get; set; }
		#endregion

		#region Source
		public abstract class source : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Source")]
		[CRMSources]
		[PXMassMergableField]
		[PXFormula(typeof(Switch<Case<Where<Current2<source>, IsNull, Or<EntryStatus, Equal<EntryStatus.inserted>>>, Selector<Contact.classID, CRContactClass.defaultSource>>, source>))]
		public virtual String Source { get; set; }
		#endregion

        #region ResetOwner
		public abstract class resetOwner : IBqlField { }
		[PXBool]
		[PXFormula(typeof(Switch<Case<Where<Current2<Contact.workgroupID>, IsNull, And<Current2<Contact.ownerID>, IsNull, And<Current2<Contact.classID>, IsNotNull, Or<EntryStatus, Equal<EntryStatus.inserted>>>>>, True>, False>))]
		public virtual bool? ResetOwner { get; set; }
		#endregion

		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Workgroup")]
		[PXCompanyTreeSelector]
		[PXFormula(typeof(Switch<Case<Where<Current2<Contact.resetOwner>, Equal<True>>, DefaultContactWorkgroup<Contact.classID>>, workgroupID>))]
		[PXMassUpdatableField]
		[PXMassMergableField]
		public virtual int? WorkgroupID { get; set; }
		#endregion

		#region OwnerID
		public abstract class ownerID : IBqlField { }

		[PXDBGuid]
		[PXOwnerSelector(typeof(Contact.workgroupID))]
		[PXUIField(DisplayName = "Owner")]        
        [PXFormula(typeof(Switch<Case<Where<Current2<Contact.resetOwner>, Equal<True>>, DefaultContactOwner<Contact.classID>>, ownerID>))]        
		[PXMassUpdatableField]
		[PXMassMergableField]
		public virtual Guid? OwnerID { get; set; }

		#endregion

		#region UserID
		public abstract class userID : IBqlField { }

		[PXDBGuid]
		public virtual Guid? UserID { get; set; }
		#endregion

		#region CampaignID
		public abstract class campaignID : PX.Data.IBqlField
		{
		}
		protected String _CampaignID;
		[PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = Messages.CampaignID)]
		[PXSelector(typeof(CRCampaign.campaignID), DescriptionField = typeof(CRCampaign.campaignName))]
		[PXMassMergableField]
		public virtual String CampaignID
		{
			get
			{
				return this._CampaignID;
			}
			set
			{
				this._CampaignID = value;
			}
		}
		#endregion

		#region Method
		public abstract class method : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[CRContactMethods]
		[PXDefault(CRContactMethodsAttribute.Any, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Contact Method")]
		[PXMassUpdatableField]
		[PXMassMergableField]
		public virtual String Method { get; set; }

		#endregion

		#region IsConvertable
		public abstract class isConvertable : IBqlField { }

		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Can Be Converted", Visible = false)]
		public virtual bool? IsConvertable { get; set; }
		#endregion

		#region GrammValidationDateTime
		public abstract class grammValidationDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _GrammValidationDateTime;

	    [CRValidateDate]		
		public virtual DateTime? GrammValidationDateTime
		{
			get
			{
				return this._GrammValidationDateTime;
			}
			set
			{
				this._GrammValidationDateTime = value;
			}
		}
		#endregion

        #region ConvertedBy
        public abstract class convertedBy : IBqlField { }

        [PXDBGuid]
        [PXSelector(typeof(Users.pKID), SubstituteKey = typeof(Users.username), DescriptionField = typeof(Users.fullName), CacheGlobal = true, DirtyRead = true, ValidateValue = false)]
        [PXUIField(DisplayName = "Converted By")]
        public virtual Guid? ConvertedBy { get; set; }
        #endregion

		#region Attributes
		public abstract class attributes : IBqlField { }

		[CRAttributesField(typeof(Contact.classID))]
		public virtual string[] Attributes { get; set; }
		#endregion

		#region IEmailMessageTarget Members

		public string Address
		{
			get { return EMail; }
		}

		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField { }

		[PXDBCreatedByID]
		[PXUIField(DisplayName = "Created By")]
		public virtual Guid? CreatedByID { get; set; }
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField { }

		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField { }

		[PXDBCreatedDateTime]
		[PXUIField(DisplayName = "Created Date")]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField { }

		[PXDBLastModifiedByID]
		[PXUIField(DisplayName = "Last Modified By")]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion

		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField { }

		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField { }

		[PXDBLastModifiedDateTimeUtc]
		[PXUIField(DisplayName = "Last Modified Date")]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion

        #region SuggestField
        public abstract class searchSuggestion : PX.Data.IBqlField { }

        [PXString(150, IsUnicode = true)]
        [PXUIField(DisplayName = "Search Suggestion")]
		[PXDBCalced(typeof(Add<Quotes, Add<Contact.displayName, Add<Quotes, Add<Space, Add<OpenBracket, Add<Contact.eMail, CloseBracket>>>>>>), typeof(string))]
        public virtual String SearchSuggestion 
		  {
	        get { return _SearchSuggestion; }
	        set { _SearchSuggestion = value; }
        }
			protected string _SearchSuggestion;
        #endregion

        #region ExtRefNbr
        public abstract class extRefNbr : PX.Data.IBqlField
        {
        }
        protected String _ExtRefNbr;
        [PXDBString(40, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Ext Ref Nbr", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        public virtual String ExtRefNbr
        {
            get
            {
                return this._ExtRefNbr;
            }
            set
            {
                this._ExtRefNbr = value;
            }
        }
        #endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField { }

		[PXDBTimestamp]
		public virtual Byte[] tstamp { get; set; }
		#endregion

		#region DeletedDatabaseRecord
		public abstract class deletedDatabaseRecord : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? DeletedDatabaseRecord { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select<Contact>), Persistent = false)]
	public class ContactBAccount : Contact
	{
		#region ContactID
		public new abstract class contactID : IBqlField { }

		[PXDBInt(IsKey = true, BqlTable = typeof(Contact))]
		[PXUIField(DisplayName = "Contact ID", Visibility = PXUIVisibility.Invisible)]
		public override Int32? ContactID { get; set; }
		#endregion		

		#region Contact
		public abstract class contact : PX.Data.IBqlField { }
		[PXString]
		[PXUIField(DisplayName = "Contact", Visibility = PXUIVisibility.Invisible, Visible = false, Enabled = false)]
		[PXDependsOnFields(typeof(ContactBAccount.displayName), typeof(ContactBAccount.fullName))]
		public virtual String Contact { get { return string.IsNullOrEmpty(DisplayName) ?  FullName : DisplayName; }}
		#endregion		

        public new abstract class contactType : PX.Data.IBqlField { }
        public new abstract class bAccountID : PX.Data.IBqlField { }
        public new abstract class parentBAccountID : PX.Data.IBqlField { }

	}

	[Serializable]
	[PXProjection(typeof(Select2<Contact, LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>>>), Persistent = false)]
	public class ContactAccount : Contact
	{
		#region AcctName
		public abstract class acctName : PX.Data.IBqlField
		{
		}
		protected String _AcctName;
		[PXDBString(60, IsUnicode = true, BqlTable = typeof(BAccount))]
		[PXDefault()]
		[PXUIField(DisplayName = "Account Name", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXMassMergableField]
		public virtual String AcctName
		{
			get
			{
				return this._AcctName;
			}
			set
			{
				this._AcctName = value;
			}
		}
		#endregion
		#region Type
		public abstract class type : PX.Data.IBqlField
		{
		}
		protected String _Type;
		[PXDBString(2, IsFixed = true, BqlTable = typeof(BAccount))]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[BAccountType.List()]
		public virtual String Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}
		#endregion
		#region AccountStatus
		public abstract class accountStatus : PX.Data.IBqlField
		{
		}

		protected String _AccountStatus;
		[PXDBString(1, IsFixed = true, BqlField = typeof(BAccount.status))]
		[PXUIField(DisplayName = "Status")]
		[BAccount.status.List()]
		[PXMassUpdatableField]
		[PXMassMergableField]
		public virtual String AccountStatus
		{
			get
			{
				return this._AccountStatus;
			}
			set
			{
				this._AccountStatus = value;
			}
		}
		#endregion
		#region DefContactID
		public abstract class defContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefContactID;
		[PXDBInt(BqlTable = typeof(BAccount))]				
		[PXMassMergableField]
		public virtual Int32? DefContactID
		{
			get
			{
				return this._DefContactID;
			}
			set
			{
				this._DefContactID = value;
			}
		}
		#endregion
	}
}
