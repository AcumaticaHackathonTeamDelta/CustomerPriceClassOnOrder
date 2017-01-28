namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.EP;
	using PX.Objects.CS;	
	using System.Collections;
	using System.Collections.Generic;
	using PX.Objects.CR;
	using PX.SM;
	using PX.Objects.CA;

	/// <summary>
	/// 
	/// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLWorkBook)]
	public partial class GLWorkBook : PX.Data.IBqlTable
	{
		#region WorkBookID
		public abstract class workBookID : PX.Data.IBqlField
		{
		}
		protected String _WorkBookID;

		/// <summary>
		/// !REV!
		/// The unique voucher code for the selected <see cref="Module"/> and <see cref="TranType">Type</see> of the document.
		/// The code is selected by user on the Journal Vouchers (GL.30.40.00) screen (<see cref="GLWorkBookID.WorkBookID"/> field)
		/// when entering the lines of the batch and determines the module and type of the document or transaction
		/// to be created from the corresponding line of the document batch.
		/// Identifies the record of this DAC associated with a <see cref="GLWorkBookID">line</see> of a <see cref="GLDocBatch">document batch</see>.
		/// Only one code can be created for any combination of <see cref="Module"/> and <see cref="TranType">Document/Transaction Type</see>.
		/// </summary>
		[PXDBString(10, IsUnicode = true, IsKey=true, InputMask = ">aaaaaaaaaa")]
		[PXDefault()]
		[PXSelector(typeof(Search<GLWorkBook.workBookID>))]
		[PXUIField(DisplayName = "Workbook ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String WorkBookID
		{
			get
			{
				return this._WorkBookID;
			}
			set
			{
				this._WorkBookID = value;
			}
		}
		#endregion
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;

		/// <summary>
		/// The code of the module where a document or transaction will be generated according to this entry code.
		/// </summary>
		/// <value>
		/// Allowed values are: <c>"GL"</c>, <c>"AP"</c>, <c>"AR"</c> and <c>"CA"</c>.
		/// </value>
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[VoucherModule.List()]
		[PXFieldDescription]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;

		/// <summary>
		/// The type of the document or transaction generated according to the code.
		/// </summary>
		/// <value>
		/// Allowed values set depends on the selected <see cref="Module"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Transaction Type", Visibility = PXUIVisibility.SelectorVisible, Required=true)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;

		/// <summary>
		/// Description of the entry code.
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXDefault]
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
		#region VoucherBatchNumberingID
		public abstract class voucherBatchNumberingID : PX.Data.IBqlField
		{
		}
		protected String _VoucherBatchNumberingID;

		/// <summary>
		/// The identifier of the <see cref="Numbering">Numbering Sequence</see> used for batches.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Numbering.NumberingID"/> field.
		/// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Voucher Batch")]
		[PXSelector(typeof(Numbering.numberingID),
			DescriptionField = typeof(Numbering.descr))]
		[PXRestrictor(typeof(Where<Numbering.userNumbering, Equal<False>>), Messages.ManualNumberingDisabled)]
		public virtual String VoucherBatchNumberingID
		{
			get
			{
				return this._VoucherBatchNumberingID;
			}
			set
			{
				this._VoucherBatchNumberingID = value;
			}
		}
		#endregion
		#region VoucherNumberingID
		public abstract class voucherNumberingID : PX.Data.IBqlField
		{
		}
		protected String _VoucherNumberingID;

		/// <summary>
		/// The identifier of the <see cref="Numbering">Numbering Sequence</see> used for batches of documents
		/// created using the Journal Vouchers (GL.30.40.00) form. See the <see cref="GLDocBatch"/> DAC.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Numbering.NumberingID"/> field.
		/// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Voucher")]
		[PXSelector(typeof(Numbering.numberingID),
			DescriptionField = typeof(Numbering.descr))]
		[PXRestrictor(typeof(Where<Numbering.userNumbering, Equal<False>>), Messages.ManualNumberingDisabled)]
		public virtual String VoucherNumberingID
		{
			get
			{
				return this._VoucherNumberingID;
			}
			set
			{
				this._VoucherNumberingID = value;
			}
		}
		#endregion
		#region SitemapID
		public abstract class sitemapID : IBqlField { }
		[PXDBGuid(true)]
		[PXDefault]
		public Guid? SiteMapID { get; set; }
		#endregion

		#region SitemapParent
		public abstract class sitemapParent : IBqlField { }
		[PXGuid]
		[PXUIField(DisplayName = "Location")]
		public Guid? SitemapParent { get; set; }
		#endregion
		#region SitemapTitle
		public abstract class sitemapTitle : IBqlField { }
		[PXString(IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Title")]
		public string SitemapTitle { get; set; }
		#endregion
		#region VoucherEditScreen
		public abstract class voucherEditScreen : IBqlField { }
		[PXDBGuid]
		[PXUIField(DisplayName = "Voucher Entry Form", Required=true)]
		[PXDefault]
		public Guid? VoucherEditScreen { get; set; }
		#endregion
#if USE_DOC_TEMPLATE
		
		#region ScreenID
		public abstract class screenID : IBqlField
		{
		}
		protected String _ScreenID;
		[PXString(8,IsFixed = true, InputMask = "CC.CC.CC.CC")]
		[PXDBScalar(typeof(Search<SiteMap.screenID, Where<SiteMap.nodeID, Equal<GLWorkBook.voucherEditScreen>>>))]
		[PXUIField(DisplayName = "Screen ID", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ScreenID
		{			
			get
			{
				return this._ScreenID;				
			}
			set 
			{
				this._ScreenID = value;
			}			
		}
		#endregion
		#region TemplateID
		public abstract class templateID : IBqlField
		{
		}
		protected Int32? _TemplateID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Template ID")]
		[PXSelector(typeof(Search<AUTemplate.templateID,
		    Where<AUTemplate.screenID, Equal<Optional<GLWorkBook.screenID>>>>),
		    DescriptionField = typeof(AUTemplate.description))]

		//[PXSelector(typeof(PX.SM.AUTemplate.templateID))]
		public virtual Int32? TemplateID
		{
			get
			{
				return this._TemplateID;
			}
			set
			{
				this._TemplateID = value;
			}
		}
		#endregion

#endif	
		#region DefaultDescription
		public abstract class defaultDescription : PX.Data.IBqlField
		{
		}
		protected String _DefaultDescription;

		/// <summary>
		/// Default description for the voucher document.
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Default Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DefaultDescription
		{
			get
			{
				return this._DefaultDescription;
			}
			set
			{
				this._DefaultDescription = value;
			}
		}
		#endregion		
		#region DefaultCashAccountID
		public abstract class defaultCashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultCashAccountID;
		[CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<False>>>>), Visible=false)]
		public virtual Int32? DefaultCashAccountID
		{
			get
			{
				return this._DefaultCashAccountID;
			}
			set
			{
				this._DefaultCashAccountID = value;
			}
		}
		#endregion
		#region DefaultEntryTypeID
		public abstract class defaultEntryTypeID : PX.Data.IBqlField
		{
		}
		protected String _DefaultEntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId, InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
									Where<CashAccountETDetail.accountID, Equal<Current<GLWorkBook.defaultCashAccountID>>, And<CAEntryType.module, Equal<BatchModule.moduleCA>>>>), DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Default Entry Type", Visible = false)]
		public virtual String DefaultEntryTypeID
		{
			get
			{
				return this._DefaultEntryTypeID;
			}
			set
			{
				this._DefaultEntryTypeID = value;
			}
		}
		#endregion	
		#region DefaultBAccountID
		public abstract class defaultBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultBAccountID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Business Account", Visible = false)]
		public virtual Int32? DefaultBAccountID
		{
			get
			{
				return this._DefaultBAccountID;
			}
			set
			{
				this._DefaultBAccountID = value;
			}
		}
		#endregion
		#region DefaultVendorID
		public abstract class defaultVendorID : PX.Data.IBqlField
		{
		}
		[PXInt]
		[PXSelector(typeof(Search<AP.Vendor.bAccountID>),typeof(AP.Vendor.acctCD), typeof(AP.Vendor.acctName) , 
			SubstituteKey =typeof(AP.Vendor.acctCD), DescriptionField =typeof(AP.Vendor.acctName))]
		[PXUIField(DisplayName = AP.Messages.Vendor, Visible = false)]
		public virtual Int32? DefaultVendorID
		{
			[PXDependsOnFields(typeof(GLWorkBook.defaultBAccountID))]
			get
			{
				return this._DefaultBAccountID;
			}
			set
			{
				this._DefaultBAccountID = value;
			}
		}
		#endregion	
		#region DefaultCustomerID
		public abstract class defaultCustomerID : PX.Data.IBqlField
		{
		}
		[PXInt]
		[PXSelector(typeof(Search<AR.Customer.bAccountID>), typeof(AR.Customer.acctCD), typeof(AR.Customer.acctName),
			SubstituteKey = typeof(AR.Customer.acctCD), DescriptionField = typeof(AR.Customer.acctName))]
		[PXUIField(DisplayName = AR.Messages.Customer, Visible = false)]
		public virtual Int32? DefaultCustomerID
		{
			[PXDependsOnFields(typeof(GLWorkBook.defaultBAccountID))]
			get
			{
				return this._DefaultBAccountID;
			}
			set
			{
				this._DefaultBAccountID = value;
			}
		}
		#endregion
		#region DefaultLocationID
		public abstract class defaultLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<GLWorkBook.defaultBAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr), Visible = false, FieldClass = "LOCATION")]
		[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<GLWorkBook.defaultBAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? DefaultLocationID
		{
			get
			{
				return this._DefaultLocationID;
			}
			set
			{
				this._DefaultLocationID = value;
			}
		}
		#endregion
		#region SingleOpenVoucherBatch
		public abstract class singleOpenVoucherBatch : IBqlField { }
		protected bool? _SingleOpenVoucherBatch;

		/// <summary>
		/// 
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Only One Unreleased Voucher Batch Per Workbook")]
		public virtual bool? SingleOpenVoucherBatch
		{
			get
			{
				return this._SingleOpenVoucherBatch;
			}
			set
			{
				this._SingleOpenVoucherBatch = value;
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
		[PXUIField(DisplayName = "Last Modified", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
	}

}
	

