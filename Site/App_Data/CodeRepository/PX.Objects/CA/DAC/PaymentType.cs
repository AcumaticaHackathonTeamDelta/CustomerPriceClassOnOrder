using PX.Data.EP;

namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.SM;
	using PX.Api;
	

	public class namespaceAP : Constant<string>
	{
		public namespaceAP() : base("PX.Objects.AP.Reports") { ;}
	}

	public class urlReports : Constant<string>
	{
		public urlReports() : base("~/Frames/ReportLauncher%") { ;}
	}

#if false
    [PXCacheName(Messages.PaymentType)]
    [System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(PaymentTypeMaint))]
    public partial class PaymentType : PX.Data.IBqlTable
    {
        #region PaymentTypeID
        public abstract class paymentTypeID : PX.Data.IBqlField
        {
        }
        protected String _PaymentTypeID;
        [PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault()]
        [PXUIField(DisplayName = "Payment Type ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PaymentType.paymentTypeID>))]
        public virtual String PaymentTypeID
        {
            get
            {
                return this._PaymentTypeID;
            }
            set
            {
                this._PaymentTypeID = value;
            }
        }
        #endregion
        #region Descr
        public abstract class descr : PX.Data.IBqlField
        {
        }
        protected String _Descr;
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
        #region PrintChecks
        public abstract class printChecks : PX.Data.IBqlField
        {
        }
        protected Boolean? _PrintChecks;
        [PXDBBool()]
        [PXUIField(DisplayName = "Print Checks", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false)]
        public virtual Boolean? PrintChecks
        {
            get
            {
                return this._PrintChecks;
            }
            set
            {
                this._PrintChecks = value;
            }
        }
        #endregion
        #region ReportID
        public abstract class reportID : PX.Data.IBqlField
        {
        }
        protected String _ReportID;
        [PXDBString(8, InputMask = "CC.CC.CC.CC")]
        [PXUIField(DisplayName = "Report")]
        [PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.screenID, Like<PXModule.ap_>, And<SiteMap.url, Like<urlReports>>>>), typeof(SiteMap.screenID), typeof(SiteMap.title), Headers = new string[] { Messages.ReportID, Messages.ReportName }, DescriptionField = typeof(SiteMap.title))]
        public virtual String ReportID
        {
            get
            {
                return this._ReportID;
            }
            set
            {
                this._ReportID = value;
            }
        }
        #endregion
        #region PrintRemittanceReport
        public abstract class printRemittanceReport : PX.Data.IBqlField
        {
        }
        protected Boolean? _PrintRemittanceReport;
        [PXDBBool()]
        [PXUIField(DisplayName = "Print Remittance Report", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false)]
        public virtual Boolean? PrintRemittanceReport
        {
            get
            {
                return this._PrintRemittanceReport;
            }
            set
            {
                this._PrintRemittanceReport = value;
            }
        }
        #endregion
        #region RemittanceReportReportID
        public abstract class remittanceReportID : PX.Data.IBqlField
        {
        }
        protected String _RemittanceReportID;
        [PXDBString(8, InputMask = "CC.CC.CC.CC")]
        [PXUIField(DisplayName = "Remittance Report")]
        [PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.screenID, Like<PXModule.ap_>, And<SiteMap.url, Like<urlReports>>>>), typeof(SiteMap.screenID), typeof(SiteMap.title), Headers = new string[] { Messages.ReportID, Messages.ReportName }, DescriptionField = typeof(SiteMap.title))]
        public virtual String RemittanceReportID
        {
            get
            {
                return this._RemittanceReportID;
            }
            set
            {
                this._RemittanceReportID = value;
            }
        }
        #endregion
        #region AllowInstances
        public abstract class allowInstances : PX.Data.IBqlField
        {
        }
        protected Boolean? _AllowInstances;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Use for Corporate Credit Cards")]
        public virtual Boolean? AllowInstances
        {
            get
            {
                return this._AllowInstances;
            }
            set
            {
                this._AllowInstances = value;
            }
        }
        #endregion
        #region VendorSettings
        public abstract class vendorSettings : PX.Data.IBqlField
        {
        }
        protected Boolean? _VendorSettings;
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Use Details in Vendor Payment Instructions")]
        public virtual Boolean? VendorSettings
        {
            get
            {
                return this._VendorSettings;
            }
            set
            {
                this._VendorSettings = value;
            }
        }
        #endregion
        #region CashAccountSettings
        public abstract class cashAccountSettings : PX.Data.IBqlField
        {
        }
        protected Boolean? _CashAccountSettings;
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Use Details in Cash Account Remittance Info")]
        public virtual Boolean? CashAccountSettings
        {
            get
            {
                return this._CashAccountSettings;
            }
            set
            {
                this._CashAccountSettings = value;
            }
        }
        #endregion
        #region StubLines
        public abstract class stubLines : PX.Data.IBqlField
        {
        }
        protected Int16? _StubLines;
        [PXDBShort()]
        [PXDefault((short)12)]
        [PXUIField(DisplayName = "Lines per Stub")]
        public virtual Int16? StubLines
        {
            get
            {
                return this._StubLines;
            }
            set
            {
                this._StubLines = value;
            }
        }
        #endregion
        #region CreateBatch
        public abstract class createBatch : PX.Data.IBqlField
        {
        }
        protected Boolean? _CreateBatch;
        [PXDBBool()]
        [PXUIField(DisplayName = "Create Batch Payment", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false)]
        public virtual Boolean? CreateBatch
        {
            get
            {
                return this._CreateBatch;
            }
            set
            {
                this._CreateBatch = value;
            }
        }
        #endregion
        #region BatchExportSYMappingID
        public abstract class batchExportSYMappingID : PX.Data.IBqlField
        {
        }
        protected Guid? _BatchExportSYMappingID;
        [PXDBGuid]
        [PXUIField(DisplayName = "Batch Export Scenario", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Search<SYMapping.mappingID, Where<SYMapping.mappingType, Equal<SYMapping.mappingType.typeExport>>>), SubstituteKey = typeof(SYMapping.name))]
        public virtual Guid? BatchExportSYMappingID
        {
            get
            {
                return this._BatchExportSYMappingID;
            }
            set
            {
                this._BatchExportSYMappingID = value;
            }
        }
        #endregion
        #region NoteID

        public abstract class noteID : IBqlField { }

        [PXNote(DescriptionField = typeof(PaymentType.paymentTypeID))]
        public virtual Guid? NoteID { get; set; }

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
        #region PrintOrExport
        public abstract class printOrExport : PX.Data.IBqlField
        {
        }
        [PXBool()]
        [PXUIField(DisplayName = "Print Checks/Export", Visibility = PXUIVisibility.Visible)]

        public virtual Boolean? PrintOrExport
        {
            [PXDependsOnFields(typeof(printChecks), typeof(createBatch))]
            get
            {
                return (this._PrintChecks == true || this._CreateBatch == true);
            }
            set
            {

            }
        }
        #endregion
    } 
#endif
}
