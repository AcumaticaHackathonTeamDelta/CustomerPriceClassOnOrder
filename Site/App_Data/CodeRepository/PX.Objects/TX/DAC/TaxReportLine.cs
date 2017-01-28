namespace PX.Objects.TX
{
	using System;
	using PX.Data;
	using PX.Objects.AP;
	using PX.Objects.GL;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.TaxReportLine)]
	public partial class TaxReportLine : PX.Data.IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(Vendor.bAccountID))]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(Vendor))]
		[PXParent(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<TaxReportLine.vendorID>>>>))]
		[PXUIField(DisplayName="Report Line", Visibility=PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(TaxReportLineType.TaxAmount)]
		[PXUIField(DisplayName="Update With", Visibility=PXUIVisibility.Visible)]
		[TaxReportLineType.List]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region LineMult
		public abstract class lineMult : PX.Data.IBqlField
		{
		}
		protected Int16? _LineMult;
		[PXDBShort()]
		[PXDefault((short)1)]
		[PXUIField(DisplayName="Update Rule", Visibility=PXUIVisibility.Visible)]
		[PXIntList(new int[] { 1, -1 }, new string[] { "+Output-Input", "+Input-Output" })]
		public virtual Int16? LineMult
		{
			get
			{
				return this._LineMult;
			}
			set
			{
				this._LineMult = value;
			}
		}
		#endregion
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName="Tax Zone ID", Visibility=PXUIVisibility.Visible, Required=false)]
		[PXSelector(typeof(Search<TaxZone.taxZoneID>))]
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
		#region NetTax
		public abstract class netTax : PX.Data.IBqlField
		{
		}
		protected Boolean? _NetTax;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Net Tax", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual Boolean? NetTax
		{
			get
			{
				return this._NetTax;
			}
			set
			{
				this._NetTax = value;
			}
		}
		#endregion
		#region TempLine
		public abstract class tempLine : PX.Data.IBqlField
		{
		}
		protected Boolean? _TempLine;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Detail by Tax Zones", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual Boolean? TempLine
		{
			get
			{
				return this._TempLine;
			}
			set
			{
				this._TempLine = value;
			}
		}
		#endregion
		#region TempLineNbr
		public abstract class tempLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _TempLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Parent Line")]
		public virtual Int32? TempLineNbr
		{
			get
			{
				return this._TempLineNbr;
			}
			set
			{
				this._TempLineNbr = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.SelectorVisible)]
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
        #region ReportLineNbr
        public abstract class reportLineNbr : PX.Data.IBqlField
        {
        }
        protected String _ReportLineNbr;        
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Box Number", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String ReportLineNbr
        {
            get
            {
                return this._ReportLineNbr;
            }
            set
            {
                this._ReportLineNbr = value;
            }
        }
        #endregion
        #region BucketSum
        public abstract class bucketSum : PX.Data.IBqlField
        {
        }
        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Calc. Rule", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
        public virtual String BucketSum { get; set; }
        #endregion
		#region HideReportLine
		public abstract class hideReportLine : PX.Data.IBqlField
		{
		}
		protected Boolean? _HideReportLine;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Hide Report Line", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual Boolean? HideReportLine
		{
			get
			{
				return this._HideReportLine;
			}
			set
			{
				this._HideReportLine = value;
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
	}

	public class TaxReportLineType
	{
		public const string TaxAmount = "P";
		public const string TaxableAmount = "A";
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { TaxAmount, TaxableAmount }, new string[] { "Tax Amount", "Taxable Amount" })
			 {
			 }
		}

		public class taxAmount : Constant<string>
		{
			public taxAmount():base(TaxAmount)
			{
			}
		}
		public class taxableAmount : Constant<string>
		{
			public taxableAmount()
				: base(TaxableAmount)
			{
			}
		}

	}
}
