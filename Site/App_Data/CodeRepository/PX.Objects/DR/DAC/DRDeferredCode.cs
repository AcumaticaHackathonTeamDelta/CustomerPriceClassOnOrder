using System;
using System.Collections.Generic;

using PX.Common;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.GL;

namespace PX.Objects.DR
{
	public class DRScheduleOption : ILabelProvider
	{
		private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ ScheduleOptionStart, Messages.StartOfFinancialPeriod },
			{ ScheduleOptionEnd, Messages.EndOfFinancialPeriod },
			{ ScheduleOptionFixedDate, Messages.FixedDayOfThePeriod },
		};

		public const string ScheduleOptionStart = "S";
		public const string ScheduleOptionEnd = "E";
		public const string ScheduleOptionFixedDate = "D";

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;
	}

	[Serializable]
	[PXPrimaryGraph(typeof(DeferredCodeMaint))]
	[PXCacheName(Messages.DeferredCode)]
	public class DRDeferredCode : PX.Data.IBqlTable
	{
		#region DeferredCodeID
		public abstract class deferredCodeID : PX.Data.IBqlField
		{
		}
		protected String _DeferredCodeID;
		[PXDefault()]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Deferral Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String DeferredCodeID
		{
			get
			{
				return this._DeferredCodeID;
			}
			set
			{
				this._DeferredCodeID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region AccountType
		public abstract class accountType : PX.Data.IBqlField
		{
		}
		protected string _AccountType;
		[PXDBString(1)]
		[PXDefault(DeferredAccountType.Income)]
		[LabelList(typeof(DeferredAccountType))]
		[PXUIField(DisplayName = "Code Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string AccountType
		{
			get
			{
				return this._AccountType;
			}
			set
			{
				this._AccountType = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(DescriptionField = typeof(Account.description), Visibility = PXUIVisibility.SelectorVisible, DisplayName="Deferral Account")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(DRDeferredCode.accountID), DescriptionField = typeof(Sub.description), Visibility = PXUIVisibility.SelectorVisible, DisplayName="Deferral Sub.")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region ReconNowPct
		public abstract class reconNowPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReconNowPct;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBDecimal(2, MinValue = 0, MaxValue = 100)]
		[PXUIField(DisplayName = "Recognize Now %", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? ReconNowPct
		{
			get
			{
				return this._ReconNowPct;
			}
			set
			{
				this._ReconNowPct = value;
			}
		}
		#endregion
		#region StartOffset
		public abstract class startOffset : PX.Data.IBqlField
		{
		}
		protected Int16? _StartOffset;
		[PXDefault((short)0)]
		[PXDBShort]
		[PXUIField(DisplayName = "Start Offset")]
		public virtual Int16? StartOffset
		{
			get
			{
				return this._StartOffset;
			}
			set
			{
				this._StartOffset = value;
			}
		}
		#endregion
		#region Occurrences
		public abstract class occurrences : PX.Data.IBqlField
		{
		}
		protected Int16? _Occurrences;
		[PXDBShort(MinValue = 0)]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Occurrences")]
		public virtual Int16? Occurrences
		{
			get
			{
				return this._Occurrences;
			}
			set
			{
				this._Occurrences = value;
			}
		}
		#endregion
		#region Frequency
		public abstract class frequency : PX.Data.IBqlField
		{
		}
		protected Int16? _Frequency;
		[PXDBShort(MinValue = 1)]
		[PXUIField(DisplayName = "Every", Visibility = PXUIVisibility.Visible)]
		[PXDefault((short)1)]
		public virtual Int16? Frequency
		{
			get
			{
				return this._Frequency;
			}
			set
			{
				this._Frequency = value;
			}
		}
		#endregion
		#region ScheduleOption
		public abstract class scheduleOption : PX.Data.IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Schedule Options", Visibility = PXUIVisibility.Visible)]
		[PXDefault(DRScheduleOption.ScheduleOptionStart)]
		[LabelList(typeof(DRScheduleOption))]
		public virtual String ScheduleOption
		{
			get;
			set;
		}
		#endregion
		#region FixedDay
		public abstract class fixedDay : PX.Data.IBqlField
		{
		}
		protected Int16? _FixedDay;
		[PXDBShort(MinValue = 1, MaxValue = 31)]
		[PXUIField(DisplayName = "Fixed Day of the Period", Visibility = PXUIVisibility.Visible)]
		[PXDefault((short)1)]
		public virtual Int16? FixedDay
		{
			get
			{
				return this._FixedDay;
			}
			set
			{
				this._FixedDay = value;
			}
		}
		#endregion
		#region Method
		public abstract class method : PX.Data.IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Recognition Method", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(DeferredMethodType.EvenPeriods)]
		[LabelList(typeof(DeferredMethodType))]
		public virtual string Method
		{
			get;
			set;
		}
		#endregion
		#region IsMDA
		public abstract class multiDeliverableArrangement : PX.Data.IBqlField { }

		protected bool? _MultiDeliverableArrangement;

		/// <summary>
		/// When set to <c>true</c> indicates that this deferral code is used for items
		/// that represent multiple deliverable arrangements and
		/// for which the revenue should be split into multiple components.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Multiple-Deliverable Arrangement")]
		public virtual bool? MultiDeliverableArrangement
		{
			get { return _MultiDeliverableArrangement; }
			set { _MultiDeliverableArrangement = value; }
		}
		#endregion
		#region AccountSource
		public abstract class accountSource : IBqlField { }

		protected string _AccountSource;

		[PXDBString(1, IsFixed = true)]
		[PXDefault(DeferralAccountSource.DeferralCode)]
		[DeferralAccountSource.List]
		[PXUIField(DisplayName = "Use Deferral Account from")]
		public virtual string AccountSource
		{
			get { return _AccountSource; }
			set { _AccountSource = value; }
		}
		#endregion
		#region CopySubFromSourceTran
		public abstract class copySubFromSourceTran : IBqlField { }

		protected bool? _CopySubFromSourceTran;

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Copy Sub. from Sales/Expense Sub.")]
		public virtual bool? CopySubFromSourceTran
		{
			get { return _CopySubFromSourceTran; }
			set { _CopySubFromSourceTran = value; }
		}
		#endregion
		#region DeferralSubMaskAR
		public abstract class deferralSubMaskAR : IBqlField { }

		protected string _DeferralSubMaskAR;

		[PXDefault]
		[SubAccountMaskAR(DisplayName = "Combine Deferral Sub. from")]
		public virtual string DeferralSubMaskAR
		{
			get { return _DeferralSubMaskAR; }
			set { _DeferralSubMaskAR = value; }
		}
		#endregion
		#region DeferralSubMaskAP
		public abstract class deferralSubMaskAP : IBqlField { }

		protected string _DeferralSubMaskAP;

		[PXDefault]
		[SubAccountMaskAP(DisplayName = "Combine Deferral Sub. from")]
		public virtual string DeferralSubMaskAP
		{
			get { return _DeferralSubMaskAP; }
			set { _DeferralSubMaskAP = value; }
		}
		#endregion
		#region Periods
		public abstract class periods : IBqlField { }

		/// <summary>
		/// The field is used for UI rendering purpose only.
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "Period(s) ")]
		public virtual string Periods { get; set; }
		#endregion
		#region NoteID

		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(DRDeferredCode.deferredCodeID))]
		public virtual Guid? NoteID { get; set; }

		#endregion
		#region RecognizeInPastPeriods
		public abstract class recognizeInPastPeriods : IBqlField { }

		/// <summary>
		/// For flexible recognition methods, specifies if the system should
		/// allow recognition in periods that are earlier than the document date.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Recognition in Previous Periods")]
		public virtual bool? RecognizeInPastPeriods
		{
			get;
			set;
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

	public class DeferredAccountType : ILabelProvider
	{
		private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ Income, Messages.Income },
			{ Expense, Messages.Expense },
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public const string Income = GL.AccountType.Income;
		public const string Expense = GL.AccountType.Expense;

		public class income : Constant<string>
		{
			public income() : base(Income) { ;}
		}

		public class expense : Constant<string>
		{
			public expense() : base(Expense) { ;}
		}

	}

	public class DeferredMethodType : ILabelProvider
	{
		private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ EvenPeriods, Messages.EvenPeriods },
			{ ProrateDays, Messages.ProrateDays },
			{ ExactDays, Messages.ExactDays },
			{ FlexibleProrateDays, Messages.FlexibleProrateDays },
			{ FlexibleExactDays, Messages.FlexibleExactDays },
			{ CashReceipt, Messages.CashReceipt },
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public const string EvenPeriods = "E";
		public const string ProrateDays = "P";
		public const string ExactDays = "D";
		public const string FlexibleProrateDays = "F";
		public const string FlexibleExactDays = "L";
		public const string CashReceipt = "C";

		public class EvenPeriodMethod : Constant<string>
		{
			public EvenPeriodMethod() : base(EvenPeriods) { ;}
		}
		public class ProrateDaysMethod : Constant<string>
		{
			public ProrateDaysMethod() : base(ProrateDays) { ;}
		}
		public class ExactDaysMethod : Constant<string>
		{
			public ExactDaysMethod() : base(ExactDays) { ;}
		}

		public class cashReceipt : Constant<string>
		{
			public cashReceipt() : base(CashReceipt) { ;}
		}

		public class flexibleProrateDays : Constant<string>
		{
			public flexibleProrateDays() : base(FlexibleProrateDays) { }
		}

		public class flexibleExactDays : Constant<string>
		{
			public flexibleExactDays() : base(FlexibleExactDays) { }
		}

		public static bool RequiresTerms(string method)
		{
			return method == FlexibleExactDays || method == FlexibleProrateDays;
		}

		public static bool RequiresTerms(DRDeferredCode code)
		{
			return code != null && RequiresTerms(code.Method);
		}
	}
}
