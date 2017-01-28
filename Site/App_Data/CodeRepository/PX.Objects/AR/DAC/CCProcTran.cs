namespace PX.Objects.AR
{
	using System;
	using PX.CCProcessingBase;
	using PX.Data;
	using CCPaymentProcessing;
	[System.SerializableAttribute()]
	[PXCacheName(Messages.CCProcTran)]
	public partial class CCProcTran : PX.Data.IBqlTable
	{
		#region TranNbr
		public abstract class tranNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _TranNbr;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Tran. Nbr.",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual Int32? TranNbr
		{
			get
			{
				return this._TranNbr;
			}
			set
			{
				this._TranNbr = value;
			}
		}
		#endregion
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PMInstanceID;
		[PXDBInt()]
		[PXDefault(0)]
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
		#region ProcessingCenterID
		public abstract class processingCenterID : PX.Data.IBqlField
		{
		}
		protected String _ProcessingCenterID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName="Proc. Center",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String ProcessingCenterID
		{
			get
			{
				return this._ProcessingCenterID;
			}
			set
			{
				this._ProcessingCenterID = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Doc. Type",Visibility=PXUIVisibility.SelectorVisible)]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Doc Reference Nbr.",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String RefNbr
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
		#region OrigDocType
		public abstract class origDocType : PX.Data.IBqlField
		{
		}
		protected String _OrigDocType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Orig. Doc. Type")]
		public virtual String OrigDocType
		{
			get
			{
				return this._OrigDocType;
			}
			set
			{
				this._OrigDocType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Orig. Doc. Ref. Nbr.")]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsFixed = true)]
		[PXDefault()]
		[CCTranTypeCode.List()]
		[PXUIField(DisplayName="Tran. Type")]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region ProcStatus
		public abstract class procStatus : PX.Data.IBqlField
		{
		}
		protected String _ProcStatus;
		[PXDBString(3, IsFixed = true)]
		[PXDefault(CCProcStatus.Opened)]
		[CCProcStatus.List()]
		[PXUIField(DisplayName = "Proc. Status")]
		public virtual String ProcStatus
		{
			get
			{
				return this._ProcStatus;
			}
			set
			{
				this._ProcStatus = value;
			}
		}
		#endregion
		#region TranStatus
		public abstract class tranStatus : PX.Data.IBqlField
		{
		}
		protected String _TranStatus;
		[PXDBString(3, IsFixed = true)]
		[CCTranStatusCode.List()]
		[PXUIField(DisplayName = "Tran. Status")]
		public virtual String TranStatus
		{
			get
			{
				return this._TranStatus;
			}
			set
			{
				this._TranStatus = value;
			}
		}
		#endregion
		#region CVVVerificationStatus
		public abstract class cVVVerificationStatus : PX.Data.IBqlField
		{
		}
		protected String _CVVVerificationStatus;
		[PXDBString(3, IsFixed = true)]
		[PXDefault(CVVVerificationStatusCode.NotVerified)]
		[CVVVerificationStatusCode.List()]
		[PXUIField(DisplayName = "CVV Verification")]

		public virtual String CVVVerificationStatus
		{
			get
			{
				return this._CVVVerificationStatus;
			}
			set
			{
				this._CVVVerificationStatus = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency",Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(CM.Currency.curyID))]
		public virtual String CuryID
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
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tran. Amount",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual Decimal? Amount
		{
			get
			{
				return this._Amount;
			}
			set
			{
				this._Amount = value;
			}
		}
		#endregion
		#region RefTranNbr
		public abstract class refTranNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _RefTranNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Referenced Tran. Nbr.")]
		public virtual Int32? RefTranNbr
		{
			get
			{
				return this._RefTranNbr;
			}
			set
			{
				this._RefTranNbr = value;
			}
		}
		#endregion
		#region RefPCTranNumber
		public abstract class refPCTranNumber : PX.Data.IBqlField
		{
		}
		protected String _RefPCTranNumber;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Ref. Tran. Nbr.")]
		public virtual String RefPCTranNumber
		{
			get
			{
				return this._RefPCTranNumber;
			}
			set
			{
				this._RefPCTranNumber = value;
			}
		}
		#endregion
		#region PCTranNumber
		public abstract class pCTranNumber : PX.Data.IBqlField
		{
		}
		protected String _PCTranNumber;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Tran. Nbr.",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String PCTranNumber
		{
			get
			{
				return this._PCTranNumber;
			}
			set
			{
				this._PCTranNumber = value;
			}
		}
		#endregion
		#region AuthNumber
		public abstract class authNumber : PX.Data.IBqlField
		{
		}
		protected String _AuthNumber;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Auth. Nbr.")]
		public virtual String AuthNumber
		{
			get
			{
				return this._AuthNumber;
			}
			set
			{
				this._AuthNumber = value;
			}
		}
		#endregion
		#region PCResponseCode
		public abstract class pCResponseCode : PX.Data.IBqlField
		{
		}
		protected String _PCResponseCode;
		[PXDBString(10, IsUnicode = true)]
		public virtual String PCResponseCode
		{
			get
			{
				return this._PCResponseCode;
			}
			set
			{
				this._PCResponseCode = value;
			}
		}
		#endregion
		#region PCResponseReasonCode
		public abstract class pCResponseReasonCode : PX.Data.IBqlField
		{
		}
		protected String _PCResponseReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		public virtual String PCResponseReasonCode
		{
			get
			{
				return this._PCResponseReasonCode;
			}
			set
			{
				this._PCResponseReasonCode = value;
			}
		}
		#endregion
		#region PCResponseReasonText
		public abstract class pCResponseReasonText : PX.Data.IBqlField
		{
		}
		protected String _PCResponseReasonText;
		[PXDBString(255,IsUnicode=true)]
		[PXUIField(DisplayName = "PC Response Reason")]
		public virtual String PCResponseReasonText
		{
			get
			{
				return this._PCResponseReasonText;
			}
			set
			{
				this._PCResponseReasonText = value;
			}
		}
		#endregion
		#region PCResponse
		public abstract class pCResponse : PX.Data.IBqlField
		{
		}
		protected String _PCResponse;
		[PXDBString(1024, IsUnicode = true)]
		public virtual String PCResponse
		{
			get
			{
				return this._PCResponse;
			}
			set
			{
				this._PCResponse = value;
			}
		}
		#endregion
		#region StartTime
		public abstract class startTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartTime;
		[PXDBDate(PreserveTime = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Tran. Time")]
		public virtual DateTime? StartTime
		{
			get
			{
				return this._StartTime;
			}
			set
			{
				this._StartTime = value;
			}
		}
		#endregion
		#region EndTime
		public abstract class endTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndTime;
		[PXDBDate(PreserveTime = true)]
		public virtual DateTime? EndTime
		{
			get
			{
				return this._EndTime;
			}
			set
			{
				this._EndTime = value;
			}
		}
		#endregion
		#region ExpirationDate
		public abstract class expirationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpirationDate;
		[PXDBDate(PreserveTime = true)]
		public virtual DateTime? ExpirationDate
		{
			get
			{
				return this._ExpirationDate;
			}
			set
			{
				this._ExpirationDate = value;
			}
		}
		#endregion
		#region ErrorSource
		public abstract class errorSource : PX.Data.IBqlField
		{
		}
		protected String _ErrorSource;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(Visible = false, DisplayName = "Error Source")]
		public virtual String ErrorSource
		{
			get
			{
				return this._ErrorSource;
			}
			set
			{
				this._ErrorSource = value;
			}
		}
		#endregion
		#region ErrorText
		public abstract class errorText : PX.Data.IBqlField
		{
		}
		protected String _ErrorText;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(Visible = false, DisplayName = "Error Text")]
		public virtual String ErrorText
		{
			get
			{
				return this._ErrorText;
			}
			set
			{
				this._ErrorText = value;
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
		
		public virtual void Copy(ICCPayment aPmtInfo) 
		{
			this.PMInstanceID = aPmtInfo.PMInstanceID;
			this.DocType = aPmtInfo.DocType;
			this.RefNbr = aPmtInfo.RefNbr;
			this.CuryID = aPmtInfo.CuryID;
			this.Amount = aPmtInfo.CuryDocBal;
			this.OrigDocType = aPmtInfo.OrigDocType;
			this.OrigRefNbr = aPmtInfo.OrigRefNbr;
		}

		public virtual bool IsManuallyEntered()
		{
			return (this.ProcStatus == CCProcStatus.Finalized 
				&& this.TranStatus == CCTranStatusCode.Approved
				&& string.IsNullOrEmpty(this.PCResponseCode));
		} 
	}

	public static class CCProcStatus 
	{
		public const string Opened = "OPN";
		public const string Finalized = "FIN";
		public const string Error = "ERR";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Opened, Finalized, Error },
				new string[] { "Open", "Completed", "Error" }) { }
		}
		public class opened: Constant<string>
		{
			public opened() : base(Opened) { ;}
		}

		public class finalized: Constant<string>
		{
			public finalized() : base(Finalized) { ;}
		}
		public class error : Constant<string>
		{
			public error() : base(Error) { ;}
		}
		
	}

	public static class CCTranStatusCode
	{
		public const string Approved = "APR";
		public const string Declined = "DEC";
		public const string HeldForReview = "HFR";
		public const string Error = "ERR";
		public const string Unknown = "UKN";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Approved, Declined, Error, HeldForReview, Unknown},
				new string[] { "Approved", "Declined", "Error", "HeldForReview","Unknown" }) { }
		}
		public class approved : Constant<string>
		{
			public approved() : base(Approved ) { ;}
		}

		public class declined : Constant<string>
		{
			public declined() : base(Declined) { ;}
		}

		public class heldForReview : Constant<string>
		{
			public heldForReview() : base(HeldForReview) { ;}
		}

		public class error : Constant<string>
		{
			public error() : base(Error) { ;}
		}

		public static string GetCode(CCTranStatus aAuthStatus)
		{
			return _StatusCodes[((int)aAuthStatus)];
		}

		private static string[] _StatusCodes = { Approved, Declined, Error, HeldForReview,Unknown};

	}

	public static class CCTranTypeCode 
	{
		public const string Authorize = "AUT";
		public const string AuthorizeAndCapture = "AAC";
		public const string PriorAuthorizedCapture = "PAC";
		public const string CaptureOnly = "CAP";
		public const string VoidTran = "VDG";
		public const string Credit = "CDT";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Authorize, AuthorizeAndCapture, PriorAuthorizedCapture, CaptureOnly, VoidTran, Credit },
				new string[] { "Authorize Only", "Authorize and Capture", "Capture Authorized", "Capture Manualy Authorized","Void","Refund" }) { }
		}

		public class authorize : Constant<string>
		{
			public authorize() : base(Authorize) { ;}
		}

		public class priorAuthorizedCapture : Constant<string>
		{
			public priorAuthorizedCapture() : base(PriorAuthorizedCapture) { ;}
		}

		public class authorizeAndCapture: Constant<string>
		{
			public authorizeAndCapture() : base(AuthorizeAndCapture) { ;}
		}

		public class captureOnly: Constant<string>
		{
			public captureOnly() : base(CaptureOnly)
			{
			}
		}

		public class voidTran : Constant<string>
		{
			public voidTran() : base(VoidTran) { ;}
		}

		public class credit : Constant<string>
		{
			public credit() : base(Credit) { ;}
		}
		
		public static string GetTypeCode (CCTranType aType)
		{
			return _typeCodes[(int)aType];
		}

		private static string[] _typeCodes = { AuthorizeAndCapture, Authorize, PriorAuthorizedCapture, CaptureOnly, Credit, VoidTran };
		
	}

	public static class CVVVerificationStatusCode 
	{
		public const string Match = "MTH";
		public const string NotMatch = "NMH";
		public const string NotVerified = "NOV";
		public const string ShouldHaveBeenPresented = "SBP";
		public const string IssuerUnableToVerify ="INV";
		public const string RelyOnPriorVerification = "RPV";
		public const string Unknown = "UKN";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { CVVVerificationStatusCode.Match, NotMatch, NotVerified, ShouldHaveBeenPresented, IssuerUnableToVerify, RelyOnPriorVerification, Unknown },
				new string[] { "Match", "Not Match", "Not Verified","Should Have Been Presented", "IssuerUnableToVerify", "Rely on Prior Verification","Unknown" }) { }
		}
		public static string GetCCVCode(CcvVerificationStatus aStatus)
		{
			return _Statuses[(int)aStatus];
		}

		public class match : Constant<string>
		{
			public match() : base(CVVVerificationStatusCode.Match) { ;}
		}

		private static string[] _Statuses = { Match, NotMatch, NotVerified, ShouldHaveBeenPresented, IssuerUnableToVerify, RelyOnPriorVerification, Unknown };
	
	}
}
