namespace PX.Objects.TX
{
	using System;
	using PX.Data;
	using PX.Data.Maintenance;
	using PX.Objects.GL;
	
	[System.SerializableAttribute()]
	public partial class TXAvalaraSetup : PX.Data.IBqlTable
	{
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
		#region Account
		public abstract class account : PX.Data.IBqlField
		{
		}
		protected String _Account;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Account")]
		public virtual String Account
		{
			get
			{
				return this._Account;
			}
			set
			{
				this._Account = value;
			}
		}
		#endregion
		#region Licence
		public abstract class licence : PX.Data.IBqlField
		{
		}
		protected String _Licence;
		[PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "License Key")]
		public virtual String Licence
		{
			get
			{
				return this._Licence;
			}
			set
			{
				this._Licence = value;
			}
		}
		#endregion
		#region Url
		public abstract class url : PX.Data.IBqlField
		{
		}
		protected String _Url;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "URL")]
		public virtual String Url
		{
			get
			{
				return this._Url;
			}
			set
			{
				this._Url = value;
			}
		}
		#endregion
		#region SendRevenueAccount
		public abstract class sendRevenueAccount : PX.Data.IBqlField
		{
		}
		protected Boolean? _SendRevenueAccount;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Send Sales Account to AvaTax")]
		public virtual Boolean? SendRevenueAccount
		{
			get
			{
				return this._SendRevenueAccount;
			}
			set
			{
				this._SendRevenueAccount = value;
			}
		}
		#endregion
		#region ShowAllWarnings
		public abstract class showAllWarnings : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowAllWarnings;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Display all warning messages")]
		public virtual Boolean? ShowAllWarnings
		{
			get
			{
				return this._ShowAllWarnings;
			}
			set
			{
				this._ShowAllWarnings = value;
			}
		}
		#endregion
		#region EnableLogging
		public abstract class enableLogging : PX.Data.IBqlField
		{
		}
		protected Boolean? _EnableLogging;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable Logging")]
		public virtual Boolean? EnableLogging
		{
			get
			{
				return this._EnableLogging;
			}
			set
			{
				this._EnableLogging = value;
			}
		}
		#endregion
		#region Timeout
		public abstract class timeout : PX.Data.IBqlField
		{
		}
		protected Int32? _Timeout;
		[PXDBInt()]
		[PXDefault(30)]
		[PXUIField(DisplayName = "Request Timeout (sec.)")]
		public virtual Int32? Timeout
		{
			get
			{
				return this._Timeout;
			}
			set
			{
				this._Timeout = value;
			}
		}
		#endregion
		#region DisableTaxCalculation
		public abstract class disableTaxCalculation : PX.Data.IBqlField
		{
		}
		protected Boolean? _DisableTaxCalculation;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Disable Tax Calculation")]
		public virtual Boolean? DisableTaxCalculation
		{
			get
			{
				return this._DisableTaxCalculation;
			}
			set
			{
				this._DisableTaxCalculation = value;
			}
		}
		#endregion
		#region AlwaysCheckAddress
		public abstract class alwaysCheckAddress : PX.Data.IBqlField
		{
		}
		protected Boolean? _AlwaysCheckAddress;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Always check address before calculating tax")]
		public virtual Boolean? AlwaysCheckAddress
		{
			get
			{
				return this._AlwaysCheckAddress;
			}
			set
			{
				this._AlwaysCheckAddress = value;
			}
		}
		#endregion
		#region ShowTaxDetails
		public abstract class showTaxDetails : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowTaxDetails;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Show Tax Details")]
		public virtual Boolean? ShowTaxDetails
		{
			get
			{
				return this._ShowTaxDetails;
			}
			set
			{
				this._ShowTaxDetails = value;
			}
		}
		#endregion
		#region DisableAddressValidation
		public abstract class disableAddressValidation : PX.Data.IBqlField
		{
		}
		protected Boolean? _DisableAddressValidation;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Disable address validation")]
		public virtual Boolean? DisableAddressValidation
		{
			get
			{
				return this._DisableAddressValidation;
			}
			set
			{
				this._DisableAddressValidation = value;
			}
		}
		#endregion
		#region AddressInUppercase
		public abstract class addressInUppercase : PX.Data.IBqlField
		{
		}
		protected Boolean? _AddressInUppercase;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Return results in uppercase")]
		public virtual Boolean? AddressInUppercase
		{
			get
			{
				return this._AddressInUppercase;
			}
			set
			{
				this._AddressInUppercase = value;
			}
		}
		#endregion
		#region IsInclusiveTax
		public abstract class isInclusiveTax : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsInclusiveTax;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Inclusive Tax")]
		public virtual Boolean? IsInclusiveTax
		{
			get
			{
				return this._IsInclusiveTax;
			}
			set
			{
				this._IsInclusiveTax = value;
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

	public static class TXAvalaraCustomerUsageType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { A, B, C,D,E,F,G,H,I,J,K,L,N,P,Q,R },
				new string[] { Messages.A, Messages.B, Messages.C, Messages.D, Messages.E, Messages.F, Messages.G, Messages.H, Messages.I, Messages.J, Messages.K, Messages.L, Messages.N, Messages.P, Messages.Q, Messages.R }) { ; }
		}
		public const string A = "A";
		public const string B = "B";
		public const string C = "C";
		public const string D = "D";
		public const string E = "E";
		public const string F = "F";
		public const string G = "G";
		public const string H = "H";
		public const string I = "I";
		public const string J = "J";
		public const string K = "K";
		public const string L = "L";
		public const string N = "N";
		public const string P = "P";
		public const string Q = "Q";
		public const string R = "R";
	}
}
