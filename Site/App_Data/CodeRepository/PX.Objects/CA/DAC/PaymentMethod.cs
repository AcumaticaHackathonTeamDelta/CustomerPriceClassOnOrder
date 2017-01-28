using System;
using PX.Data;
using PX.SM;
using PX.Api;
using PX.Data.EP;

namespace PX.Objects.CA
{
	[PXCacheName(Messages.PaymentMethod)]
	[Serializable]
	[PXPrimaryGraph(typeof(PaymentMethodMaint))]
	public partial class PaymentMethod : IBqlTable
	{
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = " Payment Method ID",Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID>))]
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
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PMInstanceID;
		[PXDBForeignIdentity(typeof(PMInstance))]
		public virtual Int32? PMInstanceID
		{
			get
			{
				return this._PMInstanceID;
			}
			set
			{
				this._PMInstanceID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description",Visibility=PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
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
		#region PaymentType
		public abstract class paymentType : PX.Data.IBqlField
		{
		}
		protected String _PaymentType;
		[PXDBString(3, IsFixed = true)]
		[PXDefault(PaymentMethodType.CashOrCheck, PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PaymentMethodType.List()]
		[PXUIField(DisplayName = "Means of Payment",Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String PaymentType
		{
			get
			{
				return this._PaymentType;
			}
			set
			{
				this._PaymentType = value;
			}
		}
		#endregion
		#region DefaultCashAccountID
		public abstract class defaultCashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultCashAccountID;
		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[GL.CashAccount(DisplayName = "Default Cash Account")]
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
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
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
		#region UseForAR
		public abstract class useForAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseForAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use in AR", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? UseForAR
		{
			get
			{
				return this._UseForAR;
			}
			set
			{
				this._UseForAR = value;
			}
		}
		#endregion
		#region UseForAP
		public abstract class useForAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseForAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use in AP", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? UseForAP
		{
			get
			{
				return this._UseForAP;
			}
			set
			{
				this._UseForAP = value;
			}
		}
		#endregion
		#region UseForCA
		public abstract class useForCA : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseForCA;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Require Remittance Information for Cash Account")]
		public virtual Boolean? UseForCA
		{
			get
			{
				return this._UseForCA;
			}
			set
			{
				this._UseForCA = value;
			}
		}
		#endregion
		#region APAdditionalProcessing

		public abstract class aPAdditionalProcessing : IBqlField
		{
			public const string PrintChecks = "P";
			public const string CreateBatchPayment = "B";
			public const string NotRequired = "N";

			public class printChecks : Constant<string>
			{
				public printChecks() : base(PrintChecks) { }
			}

			public class createBatchPayment : Constant<string>
			{
				public createBatchPayment() : base(CreateBatchPayment) { }
			}

			public class notRequired : Constant<string>
			{
				public notRequired() : base(NotRequired) { }
			}

			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(new string[] { PrintChecks, CreateBatchPayment, NotRequired },
							new string[] { "Print Checks", "Create Batch Payments", "Not Required" })
				{
				}
			}
		}

		private string _APAdditionalProcessing;

		[PXString]
		[aPAdditionalProcessing.List]
		[PXDefault(aPAdditionalProcessing.NotRequired)]
		[PXDBCalced(typeof(Switch<Case<Where<PaymentMethod.aPPrintChecks, Equal<True>>, aPAdditionalProcessing.printChecks,
								  Case<Where<PaymentMethod.aPCreateBatchPayment, Equal<True>>, aPAdditionalProcessing.createBatchPayment>>,
								  aPAdditionalProcessing.notRequired>), typeof(string))]
		public virtual string APAdditionalProcessing
		{
			get { return _APAdditionalProcessing; }
			set { _APAdditionalProcessing = value; }
		}
		#endregion
		#region APCreateBatchPayment
		public abstract class aPCreateBatchPayment : PX.Data.IBqlField
		{
		}
		protected Boolean? _APCreateBatchPayment;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Create Batch Payment")]
		public virtual Boolean? APCreateBatchPayment
		{
			get
			{
				return this._APCreateBatchPayment;
			}
			set
			{
				this._APCreateBatchPayment = value;
			}
		}
		#endregion 
		#region APBatchExportSYMappingID
		public abstract class aPBatchExportSYMappingID : IBqlField {}
		protected Guid? _APBatchExportSYMappingID;
		[PXDBGuid]
		[PXUIField(DisplayName = "Export Scenario", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<SYMapping.mappingID, Where<SYMapping.mappingType, Equal<SYMapping.mappingType.typeExport>>>), SubstituteKey = typeof(SYMapping.name))]
		[PXDefault]
		[PXUIRequired(typeof(Where<PaymentMethod.useForAP, Equal<True>,
			And<PaymentMethod.aPCreateBatchPayment, Equal<True>>>))]
		public virtual Guid? APBatchExportSYMappingID
		{
			get { return _APBatchExportSYMappingID; }
			set { _APBatchExportSYMappingID = value; }
		}
		#endregion 
		#region APPrintChecks
		public abstract class aPPrintChecks : PX.Data.IBqlField
		{
		}
		protected Boolean? _APPrintChecks;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Print Checks")]
		public virtual Boolean? APPrintChecks
		{
			get
			{
				return this._APPrintChecks;
			}
			set
			{
				this._APPrintChecks = value;
			}
		}
		#endregion 
		#region APCheckReportID
		public abstract class aPCheckReportID : IBqlField {}
		protected string _APCheckReportID;
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report")]
		[PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.screenID, Like<PXModule.ap_>, And<SiteMap.url, Like<urlReports>>>>), typeof(SiteMap.screenID), typeof(SiteMap.title), Headers = new string[] { Messages.ReportID, Messages.ReportName }, DescriptionField = typeof(SiteMap.title))]
		[PXDefault]
		[PXUIRequired(typeof(Where<PaymentMethod.useForAP, Equal<True>, 
			And<PaymentMethod.aPPrintChecks, Equal<True>>>))]
		public virtual string APCheckReportID
		{
			get { return _APCheckReportID; }
			set { _APCheckReportID = value; }
		}
		#endregion
		#region APStubLines
		public abstract class aPStubLines : IBqlField {}
		protected short? _APStubLines;
		[PXDBShort]
		[PXDefault((short)10)]
		[PXUIField(DisplayName = "Lines per Stub")]
		public virtual short? APStubLines
		{
			get
			{
				return this._APStubLines;
			}
			set
			{
				this._APStubLines = value;
			}
		}
		#endregion
		#region APPrintRemittance
		public abstract class aPPrintRemittance : PX.Data.IBqlField
		{
		}
		protected Boolean? _APPrintRemittance;
		[PXDBBool()]
		[PXUIField(DisplayName = "Print Remittance Report", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? APPrintRemittance
		{
			get
			{
				return this._APPrintRemittance;
			}
			set
			{
				this._APPrintRemittance = value;
			}
		}
		#endregion
		#region APRemittanceReportReportID
		public abstract class aPRemittanceReportID : IBqlField {}
		protected string _APRemittanceReportID;
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Remittance Report")]
		[PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.screenID, Like<PXModule.ap_>, And<SiteMap.url, Like<urlReports>>>>), typeof(SiteMap.screenID), typeof(SiteMap.title), Headers = new string[] { Messages.ReportID, Messages.ReportName }, DescriptionField = typeof(SiteMap.title))]
		[PXDefault]
		[PXUIRequired(typeof(Where<PaymentMethod.useForAP, Equal<True>,
			And<PaymentMethod.aPPrintChecks, Equal<True>,
			And<PaymentMethod.aPPrintRemittance, Equal<True>>>>))]
		public virtual string APRemittanceReportID
		{
			get { return _APRemittanceReportID; }
			set { _APRemittanceReportID = value; }
		}
		#endregion
		#region APRequirePaymentRef
		public abstract class aPRequirePaymentRef : IBqlField { }

		protected bool? _APRequirePaymentRef;

		[PXDBBool]
		[PXUIField(DisplayName = "Require Unique Payment Ref.")]
		[PXDefault(true)]
		public virtual bool? APRequirePaymentRef
		{
			get { return _APRequirePaymentRef; }
			set { _APRequirePaymentRef = value; }
		}
		#endregion

		#region ARIsProcessingRequired
		public abstract class aRIsProcessingRequired : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARIsProcessingRequired;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Integrated Processing")]
		public virtual Boolean? ARIsProcessingRequired
		{
			get
			{
				return this._ARIsProcessingRequired;
			}
			set
			{
				this._ARIsProcessingRequired = value;
			}
		}
		#endregion
		#region ARIsOnePerCustomer
		public abstract class aRIsOnePerCustomer : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARIsOnePerCustomer;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "One Instance Per Customer")]
		public virtual Boolean? ARIsOnePerCustomer
		{
			get
			{
				return this._ARIsOnePerCustomer;
			}
			set
			{
				this._ARIsOnePerCustomer = value;
			}
		}
		#endregion        
		#region ARDepositAsBatch
		public abstract class aRDepositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARDepositAsBatch;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Batch Deposit")]
		public virtual Boolean? ARDepositAsBatch
		{
			get
			{
				return this._ARDepositAsBatch;
			}
			set
			{
				this._ARDepositAsBatch = value;
			}
		}
		#endregion 
		#region ARVoidOnDepositAccount
		public abstract class aRVoidOnDepositAccount : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARVoidOnDepositAccount;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Void On Clearing Account")]
		public virtual Boolean? ARVoidOnDepositAccount
		{
			get
			{
				return this._ARVoidOnDepositAccount;
			}
			set
			{
				this._ARVoidOnDepositAccount = value;
			}
		}
		#endregion
		#region ARDefaultVoidDateToDocumentDate
		public abstract class aRDefaultVoidDateToDocumentDate : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARDefaultVoidDateToDocumentDate;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Default Void Date to Document Date")]
		public virtual Boolean? ARDefaultVoidDateToDocumentDate
		{
			get
			{
				return this._ARDefaultVoidDateToDocumentDate;
			}
			set
			{
				this._ARDefaultVoidDateToDocumentDate = value;
			}
		}
		#endregion 
		#region ARHasBillingInfo
		public abstract class aRHasBillingInfo : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARHasBillingInfo;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Has Billing Information")]
		public virtual Boolean? ARHasBillingInfo
		{
			get
			{
				return this._ARHasBillingInfo;
			}
			set
			{
				this._ARHasBillingInfo = value;
			}
		}
		#endregion
	
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(PaymentMethod.paymentMethodID))]
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
		#region IsAccountNumberRequired
		public abstract class isAccountNumberRequired : PX.Data.IBqlField
		{
		}

		[PXBool()]
		[PXUIField(DisplayName = "Require Card/Account Number")]
		public virtual Boolean? IsAccountNumberRequired
		{
			[PXDependsOnFields(typeof(aRIsOnePerCustomer))]
			get
			{
				return this._ARIsOnePerCustomer.HasValue ? (!this._ARIsOnePerCustomer.Value) : this._ARIsOnePerCustomer;
			}
			set
			{
				this._ARIsOnePerCustomer = (value.HasValue ? !(value.Value) : value);
			}
		}
		#endregion        
		#region PrintOrExport
		public abstract class printOrExport : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXUIField(DisplayName = "Print Checks/Export", Visibility = PXUIVisibility.Visible)]

		public virtual Boolean? PrintOrExport
		{
			[PXDependsOnFields(typeof(aPPrintChecks), typeof(aPCreateBatchPayment))]
			get
			{
				return (this._APPrintChecks == true || this._APCreateBatchPayment == true);
			}
			set
			{

			}
		}
		#endregion
		#region APAllowInstances
		public abstract class aPAllowInstances : PX.Data.IBqlField
		{
		}
		protected Boolean? _APAllowInstances = false;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Use for Corporate Credit Cards")]
		public virtual Boolean? APAllowInstances
		{
			get
			{
				return this._APAllowInstances;
			}
			set
			{
				this._APAllowInstances = value;
			}
		}
		#endregion
	}

	public static class PaymentMethodType 
	{
		public const string CreditCard = "CCD";
		public const string CashOrCheck = "CHC";
		public const string DirectDeposit = "DDT";

		public class ListAttribute : PXStringListAttribute 
		{
			public ListAttribute() : base(new string[] { CreditCard, CashOrCheck, DirectDeposit },
										  new string[] { "Credit Card", "Cash/Check", "Direct Deposit" }) 
			{ }
		}

		public class creditCard : Constant<string>
		{
			public creditCard() : base(CreditCard) { ;}
		}

		public class cashOrCheck : Constant<string>
		{
			public cashOrCheck() : base(CashOrCheck) { ;}
		}
		public class directDeposit : Constant<string>
		{
			public directDeposit() : base(DirectDeposit) { ;}
		}
	}

	[System.SerializableAttribute()]
	[PXHidden]
	[PXCacheName(Messages.PMInstance)]
	public partial class PMInstance : PX.Data.IBqlTable
	{
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		[PXDBIdentity()]
		public virtual int PMInstanceID { get; set; }
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

	[PXHidden]
	public partial class PaymentMethodActive : PaymentMethod
	{
		#region PaymentMethodID
		new public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsActive
		new public abstract class isActive : PX.Data.IBqlField
		{
		}
		#endregion
	}
}
