using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.TX;
using PX.Objects.CR;
using PX.Objects.CA;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.AP.Standalone
{
	[PXProjection(typeof(Select2<APRegister, InnerJoin<APInvoice, On<APInvoice.docType, Equal<APRegister.docType>, And<APInvoice.refNbr, Equal<APRegister.refNbr>>>, InnerJoin<APPayment, On<APPayment.docType, Equal<APRegister.docType>, And<APPayment.refNbr, Equal<APRegister.refNbr>>>>>, Where<APRegister.docType, Equal<APDocType.quickCheck>, Or<APRegister.docType, Equal<APDocType.voidQuickCheck>>>>), Persistent = true)]
	[PXSubstitute(GraphType = typeof(APQuickCheckEntry))]
	[PXPrimaryGraph(typeof(APQuickCheckEntry))]
	[Serializable]
	public partial class APQuickCheck : APRegister 
	{
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APRegister.docType))]
		[PXDefault(APDocType.QuickCheck)]
		[APQuickCheckType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXFieldDescription]
		public override String DocType
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
		public new abstract class refNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(APRegister.refNbr))]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[APQuickCheckType.RefNbr(typeof(Search2<APQuickCheck.refNbr,
			InnerJoinSingleTable<Vendor, On<APQuickCheck.vendorID, Equal<Vendor.bAccountID>>>,
			Where<APQuickCheck.docType, Equal<Current<APQuickCheck.docType>>,
			And<Match<Vendor, Current<AccessInfo.userName>>>>, OrderBy<Desc<APQuickCheck.refNbr>>>), Filterable = true)]
		[APQuickCheckType.Numbering()]
		[PXFieldDescription]
		public override String RefNbr
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
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[VendorActive(
			Visibility = PXUIVisibility.SelectorVisible, 
			DescriptionField = typeof(Vendor.acctName), 
			CacheGlobal = true, 
			Filterable = true, 
			BqlField = typeof(APRegister.vendorID))]
		[PXDefault]
		public override int? VendorID
		{
			get;
			set;
		}
		#endregion
		#region VendorLocationID
		public new abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		[LocationID(
			typeof(Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>,
				And<Location.isActive, Equal<True>,
				And<MatchWithBranch<Location.vBranchID>>>>),
			DescriptionField = typeof(Location.descr),
			Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(APRegister.vendorLocationID))]
		[PXDefault(typeof(Coalesce<
			Search2<Vendor.defLocationID,
			InnerJoin<CRLocation,
				On<CRLocation.locationID, Equal<Vendor.defLocationID>,
				And<CRLocation.bAccountID, Equal<Vendor.bAccountID>>>>,
			Where<Vendor.bAccountID, Equal<Current<APQuickCheck.vendorID>>,
				And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>,
			Search<CRLocation.locationID, 
			Where<CRLocation.bAccountID, Equal<Current<APQuickCheck.vendorID>>, 
			And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>>))]
		public override Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region RemitAddressID
		public abstract class remitAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitAddressID;
		[PXDBInt(BqlField = typeof(APPayment.remitAddressID))]
		[APAddress(typeof(Select2<Location,
			InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
			InnerJoin<Address, On<Address.addressID, Equal<Location.remitAddressID>, And<Where<Address.bAccountID, Equal<Location.bAccountID>, Or<Address.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
			LeftJoin<APAddress, On<APAddress.vendorID, Equal<Address.bAccountID>, And<APAddress.vendorAddressID, Equal<Address.addressID>, And<APAddress.revisionID, Equal<Address.revisionID>, And<APAddress.isDefaultAddress, Equal<True>>>>>>>>,
			Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>>>>))]
		public virtual Int32? RemitAddressID
		{
			get
			{
				return this._RemitAddressID;
			}
			set
			{
				this._RemitAddressID = value;
			}
		}
		#endregion
		#region RemitContactID
		public abstract class remitContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitContactID;
		[PXDBInt(BqlField = typeof(APPayment.remitContactID))]
		[APContact(typeof(Select2<Location,
			InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
			InnerJoin<Contact, On<Contact.contactID, Equal<Location.remitContactID>, And<Where<Contact.bAccountID, Equal<Location.bAccountID>, Or<Contact.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
			LeftJoin<APContact, On<APContact.vendorID, Equal<Contact.bAccountID>, And<APContact.vendorContactID, Equal<Contact.contactID>, And<APContact.revisionID, Equal<Contact.revisionID>, And<APContact.isDefaultContact, Equal<True>>>>>>>>,
			Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>>>>))]
		public virtual Int32? RemitContactID
		{
			get
			{
				return this._RemitContactID;
			}
			set
			{
				this._RemitContactID = value;
			}
		}
		#endregion
		#region APAccountID
		public new abstract class aPAccountID : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[Account(typeof(APQuickCheck.branchID), typeof(Search<Account.accountID,
					Where2<Match<Current<AccessInfo.userName>>,
						 And<Account.active, Equal<True>,
						 And<Account.isCashAccount, Equal<False>,
						 And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
						  Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>>>), DisplayName = "AP Account", BqlField = typeof(APRegister.aPAccountID))]
		public override Int32? APAccountID
		{
			get
			{
				return this._APAccountID;
			}
			set
			{
				this._APAccountID = value;
			}
		}
		#endregion
		#region APSubID
		public new abstract class aPSubID : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[SubAccount(typeof(APQuickCheck.aPAccountID), DescriptionField = typeof(Sub.description), DisplayName = "AP Subaccount", Visibility = PXUIVisibility.Visible, BqlField = typeof(APRegister.aPSubID))]
		public override Int32? APSubID
		{
			get
			{
				return this._APSubID;
			}
			set
			{
				this._APSubID = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.termsID))]
		[PXDefault(typeof(Search<Vendor.termsID, Where<Vendor.bAccountID, Equal<Current<APQuickCheck.vendorID>>>>))]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		[Terms(typeof(APQuickCheck.docDate), null, null, typeof(APQuickCheck.curyDocBal), typeof(APQuickCheck.curyOrigDiscAmt))]
		[PXUIField(DisplayName = "Terms")]
		public virtual String TermsID
		{
			get
			{
				return this._TermsID;
			}
			set
			{
				this._TermsID = value;
			}
		}
		#endregion
		#region LineCntr
		public new abstract class lineCntr : PX.Data.IBqlField
		{
		}
		[PXDBInt(BqlField = typeof(APRegister.lineCntr))]
		[PXDefault(0)]
		public override Int32? LineCntr
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
		#region CuryInfoID
		public new abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		[PXDBLong(BqlField = typeof(APRegister.curyInfoID))]
		[CurrencyInfo(ModuleCode = "AP")]
		public override Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public new abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.origDocAmt), BqlField = typeof(APRegister.curyOrigDocAmt))]
		[PXUIField(DisplayName = "Payment Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public override Decimal? CuryOrigDocAmt
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
		#region OrigDocAmt
		public new abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.origDocAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? OrigDocAmt
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
		#region CuryTaxAmt
		public abstract class curyTaxAmt : IBqlField { }
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.taxAmt), BqlField = typeof(APInvoice.curyTaxAmt))]
		[PXUIField(DisplayName = "Tax Amount")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTaxAmt { get; set; }
		#endregion
		#region TaxAmt
		public abstract class taxAmt : IBqlField { }
		[PXDBDecimal(4, BqlField = typeof(APInvoice.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxAmt { get; set; }
		#endregion
		#region CuryDocBal
		public new abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.docBal), BaseCalc = false, BqlField = typeof(APRegister.curyDocBal))]
		[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public override Decimal? CuryDocBal
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
		#region DocBal
		public new abstract class docBal : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.docBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? DocBal
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
		#region CuryOrigDiscAmt
		public new abstract class curyOrigDiscAmt : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.origDiscAmt), BqlField = typeof(APRegister.curyOrigDiscAmt))]
		[PXUIField(DisplayName = "Cash Discount Taken", Visibility = PXUIVisibility.SelectorVisible)]
		public override Decimal? CuryOrigDiscAmt
		{
			get
			{
				return this._CuryOrigDiscAmt;
			}
			set
			{
				this._CuryOrigDiscAmt = value;
			}
		}
		#endregion
		#region OrigDiscAmt
		public new abstract class origDiscAmt : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.origDiscAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? OrigDiscAmt
		{
			get
			{
				return this._OrigDiscAmt;
			}
			set
			{
				this._OrigDiscAmt = value;
			}
		}
		#endregion
		#region CuryDiscTaken
		public new abstract class curyDiscTaken : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.discTaken), BqlField = typeof(APRegister.curyDiscTaken))]
		public override Decimal? CuryDiscTaken
		{
			get
			{
				return this._CuryDiscTaken;
			}
			set
			{
				this._CuryDiscTaken = value;
			}
		}
		#endregion
		#region DiscTaken
		public new abstract class discTaken : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.discTaken))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? DiscTaken
		{
			get
			{
				return this._DiscTaken;
			}
			set
			{
				this._DiscTaken = value;
			}
		}
		#endregion
		#region CuryDiscBal
		public new abstract class curyDiscBal : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.discBal), BaseCalc = false, BqlField = typeof(APRegister.curyDiscBal))]
		[PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public override Decimal? CuryDiscBal
		{
			get
			{
				return this._CuryDiscBal;
			}
			set
			{
				this._CuryDiscBal = value;
			}
		}
		#endregion
		#region DiscBal
		public new abstract class discBal : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.discBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? DiscBal
		{
			get
			{
				return this._DiscBal;
			}
			set
			{
				this._DiscBal = value;
			}
		}
		#endregion
		#region CuryOrigWhTaxAmt
		public new abstract class curyOrigWhTaxAmt : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.origWhTaxAmt), BqlField = typeof(APRegister.curyOrigWhTaxAmt))]
		[PXUIField(DisplayName = "With. Tax", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public override Decimal? CuryOrigWhTaxAmt
		{
			get
			{
				return this._CuryOrigWhTaxAmt;
			}
			set
			{
				this._CuryOrigWhTaxAmt = value;
			}
		}
		#endregion
		#region OrigWhTaxAmt
		public new abstract class origWhTaxAmt : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.origWhTaxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? OrigWhTaxAmt
		{
			get
			{
				return this._OrigWhTaxAmt;
			}
			set
			{
				this._OrigWhTaxAmt = value;
			}
		}
		#endregion
		#region CuryWhTaxBal
		public new abstract class curyWhTaxBal : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.whTaxBal), BaseCalc = false, BqlField = typeof(APRegister.curyWhTaxBal))]
		public override Decimal? CuryWhTaxBal
		{
			get
			{
				return this._CuryWhTaxBal;
			}
			set
			{
				this._CuryWhTaxBal = value;
			}
		}
		#endregion
		#region WhTaxBal
		public new abstract class whTaxBal : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.whTaxBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? WhTaxBal
		{
			get
			{
				return this._WhTaxBal;
			}
			set
			{
				this._WhTaxBal = value;
			}
		}
		#endregion
		#region CuryTaxWheld
		public new abstract class curyTaxWheld : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.taxWheld), BqlField = typeof(APRegister.curyTaxWheld))]
		public override Decimal? CuryTaxWheld
		{
			get
			{
				return this._CuryTaxWheld;
			}
			set
			{
				this._CuryTaxWheld = value;
			}
		}
		#endregion
		#region TaxWheld
		public new abstract class taxWheld : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury(BqlField = typeof(APRegister.taxWheld))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? TaxWheld
		{
			get
			{
				return this._TaxWheld;
			}
			set
			{
				this._TaxWheld = value;
			}
		}
		#endregion
		#region DocDesc
		public new abstract class docDesc : PX.Data.IBqlField
		{
		}
		[PXDBString(60, IsUnicode = true, BqlField = typeof(APRegister.docDesc))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public override String DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
			}
		}
		#endregion
		#region CreatedByID
		public new abstract class createdByID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByID(BqlField = typeof(APRegister.createdByID))]
		public override Guid? CreatedByID
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
		public new abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByScreenID(BqlField = typeof(APRegister.createdByScreenID))]
		public override String CreatedByScreenID
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
		public new abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		[PXDBCreatedDateTime(BqlField = typeof(APRegister.createdDateTime))]
		public override DateTime? CreatedDateTime
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
		public new abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByID(BqlField = typeof(APRegister.lastModifiedByID))]
		public override Guid? LastModifiedByID
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
		public new abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByScreenID(BqlField = typeof(APRegister.lastModifiedByScreenID))]
		public override String LastModifiedByScreenID
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
		public new abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedDateTime(BqlField = typeof(APRegister.lastModifiedDateTime))]
		public override DateTime? LastModifiedDateTime
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
		public new abstract class Tstamp : PX.Data.IBqlField
		{
		}
		[PXDBTimestamp(BqlField = typeof(APRegister.Tstamp))]
		public override Byte[] tstamp
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
		#region BatchNbr
		public new abstract class batchNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APRegister.batchNbr))]
		[PXUIField(DisplayName = "Batch Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAP>>>))]
		public override String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region PrebookBatchNbr
		public new abstract class prebookBatchNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APRegister.prebookBatchNbr))]
		[PXUIField(DisplayName = "Pre-releasing Batch Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Batch.batchNbr))]
		public override String PrebookBatchNbr
		{
			get
			{
				return this._PrebookBatchNbr;
			}
			set
			{
				this._PrebookBatchNbr = value;
			}
		}
		#endregion
		#region VoidBatchNbr
		public new abstract class voidBatchNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APRegister.voidBatchNbr))]
		[PXUIField(DisplayName = "Void Batch Nbr.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Batch.batchNbr))]
		public override String VoidBatchNbr
		{
			get
			{
				return this._VoidBatchNbr;
			}
			set
			{
				this._VoidBatchNbr = value;
			}
		}
		#endregion
		#region Status
		public new abstract class status : IBqlField {}
		[PXDBString(1, IsFixed = true, BqlField = typeof(APRegister.status))]
		[PXDefault(APDocStatus.Hold)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[APDocStatus.List]
		[SetStatus]
		[PXDependsOnFields(
			typeof(APQuickCheck.voided),
			typeof(APQuickCheck.hold),
			typeof(APQuickCheck.scheduled),
			typeof(APQuickCheck.released),
			typeof(APQuickCheck.printed),
			typeof(APQuickCheck.prebooked),
			typeof(APQuickCheck.openDoc))]
		public override string Status
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
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.released))]
		[PXDefault(false)]
		public override Boolean? Released
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
		#region OpenDoc
		public new abstract class openDoc : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.openDoc))]
		[PXDefault(true)]
		public override Boolean? OpenDoc
		{
			get
			{
				return this._OpenDoc;
			}
			set
			{
				this._OpenDoc = value;
			}
		}
		#endregion
		#region Hold
		public new abstract class hold : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.hold))]
		[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
		[PXDefault(true, typeof(APSetup.holdEntry))]
		public override Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
			}
		}
		#endregion
		#region Scheduled
		public new abstract class scheduled : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.scheduled))]
		[PXDefault(false)]
		public override Boolean? Scheduled
		{
			get
			{
				return this._Scheduled;
			}
			set
			{
				this._Scheduled = value;
			}
		}
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.voided))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Void", Visible = false)]
		public override Boolean? Voided
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
		#region Printed
		public new abstract class printed : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.printed))]
		[PXDefault(false)]
		public override Boolean? Printed
		{
			get
			{
				return this._Printed;
			}
			set
			{
				this._Printed = value;
			}
		}
		#endregion
		#region Prebooked
		public new abstract class prebooked : PX.Data.IBqlField
		{
		}
		[PXDBBool(BqlField = typeof(APRegister.prebooked))]
		[PXDefault(false)]
		[PXUIField(DisplayName = Messages.Prebooked, Visible = false)]
		public override Boolean? Prebooked
		{
			get
			{
				return this._Prebooked;
			}
			set
			{
				this._Prebooked = value;
			}
		}
		#endregion
		#region NoteID
		public new abstract class noteID : PX.Data.IBqlField
		{
		}
		[PXSearchable(SM.SearchCategory.AP, "AP {0}: {1} - {3}", new Type[] { typeof(APQuickCheck.docType), typeof(APQuickCheck.refNbr), typeof(APQuickCheck.vendorID), typeof(Vendor.acctName) },
			new Type[] { typeof(APQuickCheck.invoiceNbr), typeof(APQuickCheck.docDesc) },
			 NumberFields = new Type[] { typeof(APQuickCheck.refNbr) },
			Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(APQuickCheck.docDate), typeof(APQuickCheck.status), typeof(APQuickCheck.invoiceNbr) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(APQuickCheck.docDesc) },
			MatchWithJoin = typeof(InnerJoin<Vendor, On<Vendor.bAccountID, Equal<APQuickCheck.vendorID>>>),
			SelectForFastIndexing = typeof(Select2<APQuickCheck, InnerJoin<Vendor, On<APQuickCheck.vendorID, Equal<Vendor.bAccountID>>>>)
		)]
		[PXNote(BqlField = typeof(APRegister.noteID), DescriptionField = typeof(APQuickCheck.refNbr))]
		public override Guid? NoteID
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
		#region ClosedFinPeriodID
		public new abstract class closedFinPeriodID : PX.Data.IBqlField
		{
		}
		[FinPeriodID(BqlField = typeof(APRegister.closedFinPeriodID))]
		[PXUIField(DisplayName = "Closed Period", Visibility = PXUIVisibility.Invisible)]
		public override String ClosedFinPeriodID
		{
			get
			{
				return this._ClosedFinPeriodID;
			}
			set
			{
				this._ClosedFinPeriodID = value;
			}
		}
		#endregion
		#region ClosedTranPeriodID
		public new abstract class closedTranPeriodID : PX.Data.IBqlField
		{
		}
		[FinPeriodID(BqlField = typeof(APRegister.closedTranPeriodID))]
		[PXUIField(DisplayName = "Closed Period", Visibility = PXUIVisibility.Invisible)]
		public override String ClosedTranPeriodID
		{
			get
			{
				return this._ClosedTranPeriodID;
			}
			set
			{
				this._ClosedTranPeriodID = value;
			}
		}
		#endregion
		#region RGOLAmt
		public new abstract class rGOLAmt : PX.Data.IBqlField
		{
		}
		[PXDBDecimal(4, BqlField = typeof(APRegister.rGOLAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override Decimal? RGOLAmt
		{
			get
			{
				return this._RGOLAmt;
			}
			set
			{
				this._RGOLAmt = value;
			}
		}
		#endregion
		#region ScheduleID
		public new abstract class scheduleID : IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APRegister.scheduleID))]
		public override string ScheduleID
		{
			get
			{
				return this._ScheduleID;
			}
			set
			{
				this._ScheduleID = value;
			}
		}
		#endregion
		#region ImpRefNbr
		public new abstract class impRefNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APRegister.impRefNbr))]
		public override String ImpRefNbr
		{
			get
			{
				return this._ImpRefNbr;
			}
			set
			{
				this._ImpRefNbr = value;
			}
		}
		#endregion
		//APInvoice
		#region APInvoiceDocType
		public abstract class aPInvoiceDocType : PX.Data.IBqlField
		{
		}
		[PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.docType))]
		[PXDefault()]
		[PXRestriction()]
		public virtual String APInvoiceDocType
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
		#region APInvoiceRefNbr
		public abstract class aPInvoiceRefNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
		[PXDefault()]
		[PXRestriction()]
		public virtual String APInvoiceRefNbr
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
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
		[PXDBString(40, IsUnicode = true, BqlField = typeof(APInvoice.invoiceNbr))]
		[PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String InvoiceNbr
		{
			get
			{
				return this._InvoiceNbr;
			}
			set
			{
				this._InvoiceNbr = value;
			}
		}
		#endregion
		#region InvoiceDate
		public abstract class invoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate(BqlField = typeof(APInvoice.invoiceDate))]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = "Vendor Ref. Date", Visibility = PXUIVisibility.Invisible)]
		public virtual DateTime? InvoiceDate
		{
			get
			{
				return this._InvoiceDate;
			}
			set
			{
				this._InvoiceDate = value;
			}
		}
		#endregion
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.taxZoneID))]
		[PXUIField(DisplayName = "Vendor Tax Zone", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TaxZone.taxZoneID), DescriptionField = typeof(TaxZone.descr), Filterable = true)]
		[PXDefault(typeof(Search<Location.vTaxZoneID, Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXRestrictor(typeof(Where<TaxZone.isManualVATZone, Equal<False>>), TX.Messages.CantUseManualVAT)]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region CuryTaxTotal
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxTotal;
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.taxTotal), BqlField = typeof(APInvoice.curyTaxTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTaxTotal
		{
			get
			{
				return this._CuryTaxTotal;
			}
			set
			{
				this._CuryTaxTotal = value;
			}
		}
		#endregion
		#region TaxTotal
		public abstract class taxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxTotal;
		[PXDBDecimal(4, BqlField = typeof(APInvoice.taxTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxTotal
		{
			get
			{
				return this._TaxTotal;
			}
			set
			{
				this._TaxTotal = value;
			}
		}
		#endregion
		#region CuryLineTotal
		public abstract class curyLineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryLineTotal;
		[PXUIField(DisplayName = "Detail Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.lineTotal), BqlField = typeof(APInvoice.curyLineTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryLineTotal
		{
			get
			{
				return this._CuryLineTotal;
			}
			set
			{
				this._CuryLineTotal = value;
			}
		}
		#endregion
		#region LineTotal
		public abstract class lineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineTotal;
		[PXDBDecimal(4, BqlField = typeof(APInvoice.lineTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? LineTotal
		{
			get
			{
				return this._LineTotal;
			}
			set
			{
				this._LineTotal = value;
			}
		}
		#endregion

		#region CuryVatExemptTotal
		public abstract class curyVatExemptTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVatExemptTotal;
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.vatExemptTotal), BqlField = typeof(APInvoice.curyVatExemptTotal))]
		[PXUIField(DisplayName = "VAT Exempt Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVatExemptTotal
		{
			get
			{
				return this._CuryVatExemptTotal;
			}
			set
			{
				this._CuryVatExemptTotal = value;
			}
		}
		#endregion

		#region VatExemptTotal
		public abstract class vatExemptTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _VatExemptTotal;
		[PXDBDecimal(4, BqlField = typeof(APInvoice.vatExemptTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? VatExemptTotal
		{
			get
			{
				return this._VatExemptTotal;
			}
			set
			{
				this._VatExemptTotal = value;
			}
		}
		#endregion

		#region CuryVatTaxableTotal
		public abstract class curyVatTaxableTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVatTaxableTotal;
		[PXDBCurrency(typeof(APQuickCheck.curyInfoID), typeof(APQuickCheck.vatTaxableTotal), BqlField = typeof(APInvoice.curyVatTaxableTotal))]
		[PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVatTaxableTotal
		{
			get
			{
				return this._CuryVatTaxableTotal;
			}
			set
			{
				this._CuryVatTaxableTotal = value;
			}
		}
		#endregion

		#region VatTaxableTotal
		public abstract class vatTaxableTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _VatTaxableTotal;
		[PXDBDecimal(4, BqlField = typeof(APInvoice.vatTaxableTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? VatTaxableTotal
		{
			get
			{
				return this._VatTaxableTotal;
			}
			set
			{
				this._VatTaxableTotal = value;
			}
		}
		#endregion
		
		#region SeparateCheck
		public abstract class separateCheck : PX.Data.IBqlField
		{
		}
		protected Boolean? _SeparateCheck;
		[PXDBBool(BqlField = typeof(APInvoice.separateCheck))]
		[PXDefault(true)]
		public virtual Boolean? SeparateCheck
		{
			get
			{
				return this._SeparateCheck;
			}
			set
			{
				this._SeparateCheck = value;
			}
		}
		#endregion
		#region PaySel
		public abstract class paySel : IBqlField
		{
		}
		protected bool? _PaySel = false;
		[PXDBBool(BqlField = typeof(APInvoice.paySel))]
		[PXDefault(false)]
		public bool? PaySel
		{
			get
			{
				return _PaySel;
			}
			set
			{
				_PaySel = value;
			}
		}
		#endregion
		#region PrebookAcctID
		public abstract class prebookAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PrebookAcctID;

		[PXDefault(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<APQuickCheck.vendorID>>>>), SourceField = typeof(Vendor.prebookAcctID), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Reclassification Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), BqlField = typeof(APInvoice.prebookAcctID))]
		public virtual Int32? PrebookAcctID
		{
			get
			{
				return this._PrebookAcctID;
			}
			set
			{
				this._PrebookAcctID = value;
			}
		}
		#endregion
		#region PrebookSubID
		public abstract class prebookSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PrebookSubID;

		[PXDefault(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<APQuickCheck.vendorID>>>>), SourceField = typeof(Vendor.prebookSubID), PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(APQuickCheck.prebookAcctID), DisplayName = "Reclassification Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), BqlField = typeof(APInvoice.prebookSubID))]
		public virtual Int32? PrebookSubID
		{
			get
			{
				return this._PrebookSubID;
			}
			set
			{
				this._PrebookSubID = value;
			}
		}
		#endregion
		//APPayment
		#region APPaymentDocType
		public abstract class aPPaymentDocType : PX.Data.IBqlField
		{
		}
		[PXDBString(3, IsFixed = true, BqlField = typeof(APPayment.docType))]
		[PXRestriction()]
		[PXDefault()]
		public virtual String APPaymentDocType
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
		#region APPaymentRefNbr
		public abstract class aPPaymentRefNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, InputMask = "", BqlField = typeof(APPayment.refNbr))]
		[PXRestriction()]
		[PXDefault()]
		public virtual String APPaymentRefNbr
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
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APPayment.paymentMethodID))]
		[PXDefault(typeof(Search<Location.paymentMethodID, Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>, And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
						  Where<PaymentMethod.useForAP, Equal<True>,
							And<PaymentMethod.isActive, Equal<True>>>>))]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		
		[PXDefault(typeof(Coalesce<Search2<Location.cashAccountID,
										InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID, Equal<Location.cashAccountID>,
											And<PaymentMethodAccount.paymentMethodID, Equal<Location.vPaymentMethodID>,
											And<PaymentMethodAccount.useForAP, Equal<True>>>>>,
										Where<Location.bAccountID, Equal<Current<APQuickCheck.vendorID>>,
										And<Location.locationID, Equal<Current<APQuickCheck.vendorLocationID>>,
										And<Location.vPaymentMethodID, Equal<Current<APQuickCheck.paymentMethodID>>>>>>,
								   Search2<PaymentMethodAccount.cashAccountID, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<PaymentMethodAccount.cashAccountID>>>,
										Where<PaymentMethodAccount.paymentMethodID, Equal<Current<APQuickCheck.paymentMethodID>>,
											And<CashAccount.branchID, Equal<Current<APQuickCheck.branchID>>,
											And<PaymentMethodAccount.useForAP, Equal<True>,
											And<PaymentMethodAccount.aPIsDefault, Equal<True>>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[CashAccount(typeof(APQuickCheck.branchID), typeof(Search2<CashAccount.cashAccountID,
						InnerJoin<PaymentMethodAccount,
							On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>>>,
						Where2<Match<Current<AccessInfo.userName>>,
							And<CashAccount.clearingAccount, Equal<False>,
							And<PaymentMethodAccount.paymentMethodID, Equal<Current<APQuickCheck.paymentMethodID>>,
							And<PaymentMethodAccount.useForAP, Equal<True>>>>>>), 
								Visibility = PXUIVisibility.Visible, BqlField = typeof(APPayment.cashAccountID))]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[Branch(IsDetail = false, BqlField = typeof(APRegister.branchID))]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(APRegister.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Company.baseCuryID>))]
		[PXSelector(typeof(Currency.curyID))]
		public override String CuryID
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
		#region UpdateNextNumber
		public abstract class updateNextNumber : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateNextNumber;

		[PXBool()]
		[PXUIField(DisplayName = "Update Next Number", Visibility = PXUIVisibility.Invisible)]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? UpdateNextNumber
		{
			get
			{
				return this._UpdateNextNumber;
			}
			set
			{
				this._UpdateNextNumber = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true, BqlField = typeof(APPayment.extRefNbr))]
		[PXUIField(DisplayName = "Payment Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		[PaymentRef(typeof(APQuickCheck.cashAccountID), typeof(APQuickCheck.paymentMethodID), typeof(APQuickCheck.stubCntr), typeof(APQuickCheck.updateNextNumber), Table = typeof(APPayment))]
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
		#region AdjDate
		public abstract class adjDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _AdjDate;
		[PXDBDate(BqlField = typeof(APPayment.adjDate))]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? AdjDate
		{
			get
			{
				return this._AdjDate;
			}
			set
			{
				this._AdjDate = value;
			}
		}
		#endregion
		#region AdjFinPeriodID
		public abstract class adjFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjFinPeriodID;
		[APOpenPeriod(typeof(APQuickCheck.adjDate), BqlField = typeof(APPayment.adjFinPeriodID))]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AdjFinPeriodID
		{
			get
			{
				return this._AdjFinPeriodID;
			}
			set
			{
				this._AdjFinPeriodID = value;
			}
		}
		#endregion
		#region AdjTranPeriodID
		public abstract class adjTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjTranPeriodID;
		[TranPeriodID(typeof(APQuickCheck.adjDate), BqlField = typeof(APPayment.adjTranPeriodID))]
		public virtual String AdjTranPeriodID
		{
			get
			{
				return this._AdjTranPeriodID;
			}
			set
			{
				this._AdjTranPeriodID = value;
			}
		}
		#endregion
		#region StubCntr
		public abstract class stubCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _StubCntr;
		[PXDBInt(BqlField = typeof(APPayment.stubCntr))]
		[PXDefault(0)]
		public virtual Int32? StubCntr
		{
			get
			{
				return this._StubCntr;
			}
			set
			{
				this._StubCntr = value;
			}
		}
		#endregion
		#region BillCntr
		public abstract class billCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _BillCntr;
		[PXDBInt(BqlField = typeof(APPayment.billCntr))]
		[PXDefault(0)]
		public virtual Int32? BillCntr
		{
			get
			{
				return this._BillCntr;
			}
			set
			{
				this._BillCntr = value;
			}
		}
		#endregion
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool(BqlField = typeof(APPayment.cleared))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cleared")]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
			}
		}
		#endregion
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate(BqlField = typeof(APPayment.clearDate))]
		[PXUIField(DisplayName = "Clear Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;
		[PXDBLong(BqlField = typeof(APPayment.cATranID))]
		[APQuickCheckCashTranID()]
		public virtual Int64? CATranID
		{
			get
			{
				return this._CATranID;
			}
			set
			{
				this._CATranID = value;
			}
		}
		#endregion
		#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;
		[PXDBInt(BqlField = typeof(APPayment.pTInstanceID))]
		[PXUIField(DisplayName = "Card ID", Enabled = false, Visible = false)]
		public virtual Int32? PTInstanceID
		{
			get
			{
				return this._PTInstanceID;
			}
			set
			{
				this._PTInstanceID = value;
			}
		}
		#endregion
		#region ChargeCntr
		public abstract class chargeCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _ChargeCntr;
		[PXDBInt(BqlField = typeof(APPayment.chargeCntr))]
		[PXDefault(0)]
		public virtual Int32? ChargeCntr
		{
			get
			{
				return this._ChargeCntr;
			}
			set
			{
				this._ChargeCntr = value;
			}
		}
		#endregion
		//APRegister
		#region DocDate
		public new abstract class docDate : PX.Data.IBqlField
		{
		}
		[PXDBDate(BqlField = typeof(APRegister.docDate))]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public override DateTime? DocDate
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
		#region TranPeriodID
		public new abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		[FinPeriodID(typeof(APQuickCheck.docDate), BqlField = typeof(APRegister.tranPeriodID))]
		public override String TranPeriodID
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
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[APOpenPeriod(typeof(APQuickCheck.docDate), BqlField = typeof(APRegister.finPeriodID))]
		[PXDefault()]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
		public override String FinPeriodID
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
		#region VendorID_Vendor_acctName
		public new abstract class vendorID_Vendor_acctName : PX.Data.IBqlField
		{
		}
		#endregion
		#region VoidAppl
		public abstract class voidAppl : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDefault(false)]
		public virtual Boolean? VoidAppl
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APPaymentType.VoidAppl(this._DocType);
			}
			set
			{
				if ((bool)value)
				{
					this._DocType = APPaymentType.VoidCheck;
				}
			}
		}
		#endregion
		#region PrintCheck
		public abstract class printCheck : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDefault(typeof(Search<PaymentMethod.printOrExport, Where<PaymentMethod.paymentMethodID, Equal<Current<APQuickCheck.paymentMethodID>>>>))]
		[PXUIField(DisplayName = "Print Check")]
		public virtual Boolean? PrintCheck
		{
			[PXDependsOnFields(typeof(printed))]
			get
			{
				return (this._Printed == null ? (Boolean?)null : (bool)this._Printed == false);
			}
			set
			{
				this._Printed = (value == null ? (Boolean?)null : (bool)value == false);
			}
		}
		#endregion

		#region DepositAsBatch
		public abstract class depositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _DepositAsBatch;
		[PXDBBool(BqlField = typeof(APPayment.depositAsBatch))]
		[PXUIField(DisplayName = "Batch Deposit", Enabled = false)]
		[PXDefault(false)]
		public virtual Boolean? DepositAsBatch
		{
			get
			{
				return this._DepositAsBatch;
			}
			set
			{
				this._DepositAsBatch = value;
			}
		}
		#endregion
		#region DepositAfter
		public abstract class depositAfter : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositAfter;
		[PXDBDate(BqlField = typeof(APPayment.depositAfter))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deposit After", Enabled = false, Visible = false)]
		public virtual DateTime? DepositAfter
		{
			get
			{
				return this._DepositAfter;
			}
			set
			{
				this._DepositAfter = value;
			}
		}
		#endregion
		#region Deposited
		public abstract class deposited : PX.Data.IBqlField
		{
		}
		protected Boolean? _Deposited;
		[PXDBBool(BqlField = typeof(APPayment.deposited))]
		[PXUIField(DisplayName = "Deposited", Enabled = false)]
		[PXDefault(false)]
		public virtual Boolean? Deposited
		{
			get
			{
				return this._Deposited;
			}
			set
			{
				this._Deposited = value;
			}
		}
		#endregion
		#region DepositDate
		public abstract class depositDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositDate;
		[PXDBDate(BqlField = typeof(APPayment.depositDate))]
		[PXUIField(DisplayName = "Batch Deposit Date", Enabled = false)]
		public virtual DateTime? DepositDate
		{
			get
			{
				return this._DepositDate;
			}
			set
			{
				this._DepositDate = value;
			}
		}
		#endregion
		#region DepositType
		public abstract class depositType : PX.Data.IBqlField
		{
		}
		protected String _DepositType;
		[PXUIField(Enabled = false)]
		[PXDBString(3, IsFixed = true, BqlField = typeof(APPayment.depositType))]
		public virtual String DepositType
		{
			get
			{
				return this._DepositType;
			}
			set
			{
				this._DepositType = value;
			}
		}
		#endregion
		#region DepositNbr
		public abstract class depositNbr : PX.Data.IBqlField
		{
		}
		protected String _DepositNbr;
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APPayment.depositNbr))]
		[PXUIField(DisplayName = "Batch Deposit Nbr.", Enabled = false)]
		public virtual String DepositNbr
		{
			get
			{
				return this._DepositNbr;
			}
			set
			{
				this._DepositNbr = value;
			}
		}
		#endregion

		#region TaxCalcMode
		public new abstract class taxCalcMode : IBqlField { }
		#endregion
		#region HasWithHoldTax
		public abstract class hasWithHoldTax : IBqlField { }
		protected bool? _HasWithHoldTax;
		[PXBool]
		[RestrictWithholdingTaxCalcMode(typeof(APQuickCheck.taxZoneID), typeof(APQuickCheck.taxCalcMode))]
		public virtual bool? HasWithHoldTax
		{
			get { return this._HasWithHoldTax; }
			set { this._HasWithHoldTax = value; }
		}
		#endregion
		#region HasUseTax
		public abstract class hasUseTax : IBqlField { }
		protected bool? _HasUseTax;
		[PXBool]
		[RestrictUseTaxCalcMode(typeof(APQuickCheck.taxZoneID), typeof(APQuickCheck.taxCalcMode))]
		public virtual bool? HasUseTax
		{
			get { return this._HasUseTax; }
			set { this._HasUseTax = value; }
		}
		#endregion
		#region Approval Process Overrides
		public new abstract class approved : IBqlField { }

		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public override bool? Approved
		{
			get;
			set;
		}

		public new abstract class dontApprove : IBqlField { }
		public override bool? DontApprove
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		#endregion
	}

	public class APQuickCheckType : APDocType
	{
		public class RefNbrAttribute : PXSelectorAttribute
		{
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(APQuickCheck.refNbr),
				typeof(APQuickCheck.docDate),
				typeof(APQuickCheck.finPeriodID),
				typeof(APQuickCheck.vendorID),
				typeof(APQuickCheck.vendorID_Vendor_acctName),
				typeof(APQuickCheck.vendorLocationID),
				typeof(APQuickCheck.curyID),
				typeof(APQuickCheck.curyOrigDocAmt),
				typeof(APQuickCheck.curyDocBal),
				typeof(APQuickCheck.status),
				typeof(APQuickCheck.cashAccountID),
				typeof(APQuickCheck.paymentMethodID),
				typeof(APQuickCheck.extRefNbr))
			{
			}
		}

		/// <summary>
		/// Specialized for APQuickCheck version of the <see cref="AutoNumberAttribute"/><br/>
		/// It defines how the new numbers are generated for the AP Payment. <br/>
		/// References APQuickCheck.docType and APQuickCheck.docDate fields of the document,<br/>
		/// and also define a link between  numbering ID's defined in AP Setup and APQuickCheck types:<br/>
		/// namely - APSetup.checkNumberingID for QuickCheck and null for VoidQuickCheck<br/>
		/// </summary>				
		public class NumberingAttribute : AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(APQuickCheck.docType), typeof(APQuickCheck.docDate),
					new string[] { QuickCheck, VoidQuickCheck },
					new Type[] { typeof(APSetup.checkNumberingID), null }) { ; }
		}

		public new class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { QuickCheck, VoidQuickCheck },
				new string[] { Messages.QuickCheck, Messages.VoidQuickCheck }) { ; }
		}

		public static bool VoidAppl(string DocType)
		{
			return (DocType == VoidCheck);
		}
	}
}
