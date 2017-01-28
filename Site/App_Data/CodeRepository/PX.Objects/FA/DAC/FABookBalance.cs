using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;

namespace PX.Objects.FA
{
	[Serializable]
	[PXProjection(typeof(Select2<FABookBalance,
		LeftJoin<FABookHistory, On<FABookHistory.assetID, Equal<FABookBalance.assetID>,
		And<FABookHistory.bookID, Equal<FABookBalance.bookID>,
		And<FABookHistory.finPeriodID, Equal<IsNull<FABookBalance.currDeprPeriod, FABookBalance.lastPeriod>>>>>,
		InnerJoin<FABook, On<FABook.bookID, Equal<FABookBalance.bookID>>>>>), new Type[] { typeof(FABookBalance) })]
	[PXCacheName(Messages.FABookBalance)]
	public partial class FABookBalance : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool()]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(FixedAsset.assetID))]
		[PXParent(typeof(Select<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FABookBalance.assetID>>>>))]
		[PXSelector(typeof(Search<FixedAsset.assetID>),
			SubstituteKey = typeof(FixedAsset.assetCD), DirtyRead = true, DescriptionField = typeof(FixedAsset.description))]
		[PXUIField(DisplayName = "Fixed Asset", Enabled = false)]
		public virtual Int32? AssetID
		{
			get
			{
				return this._AssetID;
			}
			set
			{
				this._AssetID = value;
			}
		}
		#endregion
		#region ClassID
		public abstract class classID : PX.Data.IBqlField
		{
		}
		protected Int32? _ClassID;
		[PXInt()]
		[PXDBScalar(typeof(Search<FixedAsset.classID, Where<FixedAsset.assetID, Equal<FABookBalance.assetID>>>))]
		[PXDefault(typeof(Search<FixedAsset.classID, Where<FixedAsset.assetID, Equal<Current<FABookBalance.assetID>>>>))]
		[PXSelector(typeof(Search<FixedAsset.assetID>), SubstituteKey = typeof(FixedAsset.assetCD), CacheGlobal = true, DescriptionField = typeof(FixedAsset.description))]
		[PXUIField(DisplayName = "Asset Class", Enabled = false)]
		public virtual Int32? ClassID
		{
			get
			{
				return this._ClassID;
			}
			set
			{
				this._ClassID = value;
			}
		}
		#endregion
		#region BookID
		public abstract class bookID : PX.Data.IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXSelector(typeof(Search2<FABook.bookID,
								InnerJoin<FABookSettings, On<FABookSettings.bookID, Equal<FABook.bookID>>>,
								Where<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>>>),
					SubstituteKey = typeof(FABook.bookCode),
					DescriptionField = typeof(FABook.description), CacheGlobal = true)]
		[PXUIField(DisplayName = "Book")]
		public virtual Int32? BookID
		{
			get
			{
				return this._BookID;
			}
			set
			{
				this._BookID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		[FixedAssetStatus.List()]
		[PXDefault(FixedAssetStatus.Active)]
		public virtual String Status
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
		#region UpdateGL
		public abstract class updateGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateGL;
		[PXDBBool(BqlField = typeof(FABook.updateGL))]
		[PXDefault(false, typeof(Search<FABook.updateGL, Where<FABook.bookID, Equal<Current<FABookBalance.bookID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Update GL", Enabled = false)]
		public virtual Boolean? UpdateGL
		{
			get
			{
				return this._UpdateGL;
			}
			set
			{
				this._UpdateGL = value;
			}
		}
		#endregion
		#region Depreciate
		public abstract class depreciate : PX.Data.IBqlField
		{
		}
		protected Boolean? _Depreciate;
		[PXDBBool]
		[PXDefault(typeof(Search<FixedAsset.depreciable, Where<FixedAsset.assetID, Equal<Current<FABookBalance.assetID>>>>))]
		[PXUIField(DisplayName = "Depreciate", Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Boolean? Depreciate
		{
			get
			{
				return this._Depreciate;
			}
			set
			{
				this._Depreciate = value;
			}
		}
		#endregion
		#region AveragingConvention
		public abstract class averagingConvention : PX.Data.IBqlField
		{
		}
		protected String _AveragingConvention;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Averaging Convention")]
		[FAAveragingConvention.List()]
		[PXDefault(typeof(Search<FABookSettings.averagingConvention,
								Where<FABookSettings.bookID, Equal<Current<FABookBalance.bookID>>,
								  And<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIRequired(typeof(FABookBalance.depreciate))]
		[PXUIEnabled(typeof(FABookBalance.depreciate))]
		public virtual String AveragingConvention
		{
			get
			{
				return this._AveragingConvention;
			}
			set
			{
				this._AveragingConvention = value;
			}
		}
		#endregion
		#region UsefulLife
		public abstract class usefulLife : PX.Data.IBqlField
		{
		}
		protected Decimal? _UsefulLife;
		[PXDBDecimal(4)]
		[PXDefault(typeof(Search<FABookSettings.usefulLife, Where<FABookSettings.bookID, Equal<Current<FABookBalance.bookID>>, And<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>>>>))]
		[PXUIField(DisplayName = "Useful Life, Years")]
		[PXUIRequired(typeof(FABookBalance.depreciate))]
		[PXUIEnabled(typeof(FABookBalance.depreciate))]
		public virtual Decimal? UsefulLife
		{
			get
			{
				return this._UsefulLife;
			}
			set
			{
				this._UsefulLife = value;
			}
		}
		#endregion
		#region MidMonthType
		public abstract class midMonthType : IBqlField { }
		protected string _MidMonthType;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Mid-Period Type", Enabled = false)]
		[PXDefault(FABook.midMonthType.FixedDay, typeof(Search<FABookSettings.midMonthType,
			Where<FABookSettings.bookID, Equal<Current<FABookBalance.bookID>>,
				 And<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>>>>))]
		[FABook.midMonthType.List]
		[PXUIRequired(typeof(FABookSettings.midMonthType.IsRequired<FABookBalance.depreciate, FABookBalance.averagingConvention>))]
		[PXUIEnabled(typeof(FABookSettings.midMonthType.IsRequired<FABookBalance.depreciate, FABookBalance.averagingConvention>))]
		public virtual string MidMonthType
		{
			get
			{
				return this._MidMonthType;
			}
			set
			{
				this._MidMonthType = value;
			}
		}
		#endregion
		#region MidMonthDay
		public abstract class midMonthDay : IBqlField { }
		protected short? _MidMonthDay;
		[PXDBShort]
		[PXDefault(typeof(Search<FABookSettings.midMonthDay,
			Where<FABookSettings.bookID, Equal<Current<FABookBalance.bookID>>,
				 And<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>>>>))]
		[PXUIField(DisplayName = "Mid-Period Day")]
		[PXUIRequired(typeof(FABookSettings.midMonthType.IsRequired<FABookBalance.depreciate, FABookBalance.averagingConvention>))]
		[PXUIEnabled(typeof(FABookSettings.midMonthType.IsRequired<FABookBalance.depreciate, FABookBalance.averagingConvention>))]
		public virtual short? MidMonthDay
		{
			get
			{
				return this._MidMonthDay;
			}
			set
			{
				this._MidMonthDay = value;
			}
		}
		#endregion
		#region ADSLife
		public abstract class aDSLife : PX.Data.IBqlField
		{
		}
		protected Decimal? _ADSLife;
		[PXDBDecimal(2)]
		[PXFormula(typeof(usefulLife))]
		[PXUIField(DisplayName = "ADS Life, Years")]
		public virtual Decimal? ADSLife
		{
			get
			{
				return this._ADSLife;
			}
			set
			{
				this._ADSLife = value;
			}
		}
		#endregion
		#region AcquisitionCost
		public abstract class acquisitionCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _AcquisitionCost;
		[PXDBBaseCury()]
		[PXDefault(typeof(Search<FADetails.acquisitionCost, Where<FADetails.assetID, Equal<Current<FABookBalance.assetID>>>>))]
		[PXUIField(DisplayName = "Orig. Acquisition Cost")]
		public virtual Decimal? AcquisitionCost
		{
			get
			{
				return this._AcquisitionCost;
			}
			set
			{
				this._AcquisitionCost = value;
			}
		}
		#endregion
		#region SalvageAmount
		public abstract class salvageAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _SalvageAmount;
		[PXDBBaseCury()]
		[PXDefault(typeof(Search<FADetails.salvageAmount, Where<FADetails.assetID, Equal<Current<FABookBalance.assetID>>>>))]
		[PXUIField(DisplayName = "Salvage Amount")]
		public virtual Decimal? SalvageAmount
		{
			get
			{
				return this._SalvageAmount;
			}
			set
			{
				this._SalvageAmount = value;
			}
		}
		#endregion
		#region BusinessUse
		public abstract class businessUse : PX.Data.IBqlField
		{
		}
		protected Decimal? _BusinessUse;
		[PXDBDecimal(4, MinValue = 0.0, MaxValue = 100.0)]
		[PXDefault(TypeCode.Decimal, "100.0")]
		[PXUIField(DisplayName = "Business Use, %")]
		public virtual Decimal? BusinessUse
		{
			get
			{
				return this._BusinessUse;
			}
			set
			{
				this._BusinessUse = value;
			}
		}
		#endregion
		#region BonusID
		public abstract class bonusID : PX.Data.IBqlField
		{
		}
		protected Int32? _BonusID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Bonus")]
		[PXSelector(typeof(Search2<FABonus.bonusID,
									CrossJoin<FABookSettings>,
									Where<FABookSettings.bookID, Equal<Current<FABookBalance.bookID>>,
									  And<FABookSettings.assetID, Equal<Current<FABookBalance.classID>>,
									  And<FABookSettings.bonus, Equal<True>>>>>),
					typeof(FABonus.bonusCD),
					SubstituteKey = typeof(FABonus.bonusCD),
					DescriptionField = typeof(FABonus.description))]
		public virtual Int32? BonusID
		{
			get
			{
				return this._BonusID;
			}
			set
			{
				this._BonusID = value;
			}
		}
		#endregion
		#region BonusRate
		public abstract class bonusRate : IBqlField
		{
		}
		protected Decimal? _BonusRate;
		[PXDBDecimal(6, MinValue = 0.0, MaxValue = 100.0)]
		[PXUIField(DisplayName = "Bonus Rate")]
		[PXFormula(typeof(GetBonusRate<deprFromDate, bonusID>))]
		public virtual Decimal? BonusRate
		{
			get
			{
				return _BonusRate;
			}
			set
			{
				_BonusRate = value;
			}
		}
		#endregion
		#region BonusAmount
		public abstract class bonusAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _BonusAmount;
		[PXDBBaseCury()]
		[PXFormula(typeof(Mult<Sub<acquisitionCost, tax179Amount>, Div<bonusRate, decimal100>>))]
		[PXUIField(DisplayName = "Bonus Amount")]
		public virtual Decimal? BonusAmount
		{
			get
			{
				return this._BonusAmount;
			}
			set
			{
				this._BonusAmount = value;
			}
		}
		#endregion
		#region Tax179Amount
		public abstract class tax179Amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Tax179Amount;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax 179 Amount")]
		public virtual Decimal? Tax179Amount
		{
			get
			{
				return this._Tax179Amount;
			}
			set
			{
				this._Tax179Amount = value;
			}
		}
		#endregion
		#region DeprFromDate
		public abstract class deprFromDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DeprFromDate;
		[PXDBDate]
		[PXUIField(DisplayName = Messages.DeprFromDate)]
		[PXDefault(typeof(Search<FADetails.depreciateFromDate, Where<FADetails.assetID, Equal<Current<assetID>>>>))]
		public virtual DateTime? DeprFromDate
		{
			get
			{
				return this._DeprFromDate;
			}
			set
			{
				this._DeprFromDate = value;
			}
		}
		#endregion
		#region DeprFromPeriod
		public abstract class deprFromPeriod : PX.Data.IBqlField
		{
		}
		protected String _DeprFromPeriod;
		[PXUIField(DisplayName = Messages.DeprFromPeriod, Enabled = false)]
		[FABookPeriodID(typeof(FABookBalance.bookID))]
		[PXFormula(typeof(RecoveryStartPeriod<deprFromDate, bookID, depreciationMethodID, averagingConvention, midMonthType, midMonthDay>))]
		public virtual String DeprFromPeriod
		{
			get
			{
				return this._DeprFromPeriod;
			}
			set
			{
				this._DeprFromPeriod = value;
			}
		}
		#endregion
		#region DeprFromYear
		public abstract class deprFromYear : IBqlField
		{
		}
		protected String _DeprFromYear;
		[PXDBCalced(typeof(Left4<FABookBalance.deprFromPeriod>), typeof(string))]
		[PXDefault(typeof(Search<FABookPeriod.finYear, Where<FABookPeriod.finPeriodID, Equal<Current<FABookBalance.deprFromPeriod>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DeprFromYear
		{
			get
			{
				return _DeprFromYear;
			}
			set
			{
				_DeprFromYear = value;
			}
		}
		#endregion
		#region DepreciationMethodID
		public abstract class depreciationMethodID : IBqlField
		{
		}
		protected Int32? _DepreciationMethodID;
		[PXDBInt]
		[PXSelector(typeof(Search<FADepreciationMethod.methodID,
			Where2<Where<Current2<usefulLife>, IsNull,
				Or<FADepreciationMethod.usefulLife, IsNull,
				Or<FADepreciationMethod.usefulLife, Equal<Current2<usefulLife>>,
				Or<FADepreciationMethod.usefulLife, Equal<decimal0>>>>>,
			And<Where<FADepreciationMethod.recordType, NotEqual<FARecordType.classType>>>>>),
					typeof(FADepreciationMethod.methodCD),
					typeof(FADepreciationMethod.depreciationMethod),
					typeof(FADepreciationMethod.usefulLife),
					typeof(FADepreciationMethod.recoveryPeriod),
					typeof(FADepreciationMethod.averagingConvention),
					typeof(FADepreciationMethod.recordType),
					typeof(FADepreciationMethod.description),
					SubstituteKey = typeof(FADepreciationMethod.methodCD),
					DescriptionField = typeof(FADepreciationMethod.description)
			)]
		[PXFormula(typeof(SelectDepreciationMethod<deprFromDate, classID, bookID>))]
		[PXUIField(DisplayName = "Depreciation Method")]
		[PXDefault]
		[PXUIRequired(typeof(FABookBalance.depreciate))]
		[PXUIEnabled(typeof(FABookBalance.depreciate))]
		public virtual Int32? DepreciationMethodID
		{
			get
			{
				return _DepreciationMethodID;
			}
			set
			{
				_DepreciationMethodID = value;
			}
		}
		#endregion
		#region RecoveryPeriod
		public abstract class recoveryPeriod : IBqlField
		{
		}
		protected Int32? _RecoveryPeriod;
		[PXDBInt(MinValue = 1)]
		[PXUIField(DisplayName = "Recovery Periods", Visible = false)]
		public virtual Int32? RecoveryPeriod
		{
			get
			{
				return _RecoveryPeriod;
			}
			set
			{
				_RecoveryPeriod = value;
			}
		}
		#endregion
		#region DeprToDate
		public abstract class deprToDate : IBqlField
		{
		}
		protected DateTime? _DeprToDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Depr. to")]
		public virtual DateTime? DeprToDate
		{
			get
			{
				return this._DeprToDate;
			}
			set
			{
				this._DeprToDate = value;
			}
		}
		#endregion
		#region DeprToPeriod
		public abstract class deprToPeriod : PX.Data.IBqlField
		{
		}
		protected String _DeprToPeriod;
		[PXUIField(DisplayName = "Depr. to Period", Enabled = false)]
		[FABookPeriodID(typeof(bookID))]
		[PXFormula(typeof(Switch<Case<Where<depreciate, Equal<True>>, OffsetBookDateToPeriod<deprFromDate, bookID, depreciationMethodID, averagingConvention, usefulLife>>>))]
		public virtual String DeprToPeriod
		{
			get
			{
				return _DeprToPeriod;
			}
			set
			{
				_DeprToPeriod = value;
			}
		}
		#endregion
		#region DeprToYear
		public abstract class deprToYear : IBqlField
		{
		}
		protected String _DeprToYear;
		[PXDBCalced(typeof(Left4<FABookBalance.deprToPeriod>), typeof(string))]
		[PXDefault(typeof(Search<FABookPeriod.finYear, Where<FABookPeriod.finPeriodID, Equal<Current<FABookBalance.deprToPeriod>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DeprToYear
		{
			get
			{
				return _DeprToYear;
			}
			set
			{
				_DeprToYear = value;
			}
		}
		#endregion
		#region LastDeprPeriod
		public abstract class lastDeprPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastDeprPeriod;
		[PXDBString(6, IsFixed = true, BqlField = typeof(FABookBalance.lastDeprPeriod), InputMask = "")]
		[PXUIField(DisplayName = "Last Depr. Period", Enabled = false)]
		[PXSelector(typeof(Search<FABookPeriod.finPeriodID, Where<FABookPeriod.bookID, Equal<Current<FABookBalance.bookID>>>>))]
		[GL.FinPeriodIDFormatting()]
		public virtual String LastDeprPeriod
		{
			get
			{
				return this._LastDeprPeriod;
			}
			set
			{
				this._LastDeprPeriod = value;
			}
		}
		#endregion
		#region CurrDeprPeriod
		public abstract class currDeprPeriod : PX.Data.IBqlField
		{
		}
		protected String _CurrDeprPeriod;
		[PXDBString(6, IsFixed = true, BqlField = typeof(FABookBalance.currDeprPeriod))]
		[PXUIField(DisplayName = "Current Period", Enabled = false)]
		[GL.FinPeriodIDFormatting()]
		public virtual String CurrDeprPeriod
		{
			get
			{
				return this._CurrDeprPeriod;
			}
			set
			{
				this._CurrDeprPeriod = value;
			}
		}
		#endregion
		#region InitPeriod
		public abstract class initPeriod : IBqlField
		{
		}
		protected String _InitPeriod;
		[PXDBString(6, IsFixed = true, BqlField = typeof(FABookBalance.initPeriod))]
		[GL.FinPeriodIDFormatting()]
		public virtual String InitPeriod
		{
			get
			{
				return this._InitPeriod;
			}
			set
			{
				this._InitPeriod = value;
			}
		}
		#endregion
		#region LastPeriod
		public abstract class lastPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastPeriod;
		[PXDBString(6, IsFixed = true, BqlField = typeof(FABookBalance.lastPeriod))]
		[GL.FinPeriodIDFormatting()]
		public virtual String LastPeriod
		{
			get
			{
				return _LastPeriod;
			}
			set
			{
				_LastPeriod = value;
			}
		}
		#endregion
		#region HistPeriod
		public abstract class histPeriod : PX.Data.IBqlField
		{
		}
		protected String _HistPeriod;
		[PXString(6, IsFixed = true)]
		[PXDBCalced(typeof(Switch<
			Case<Where<FABookBalance.currDeprPeriod, IsNull, And<FABookBalance.deprToPeriod, IsNull>>, FABookBalance.deprFromPeriod,
			Case<Where<FABookBalance.currDeprPeriod, IsNull>, FABookBalance.deprToPeriod>>, FABookBalance.currDeprPeriod>), typeof(string))]
		public virtual String HistPeriod
		{
			get
			{
				return this._HistPeriod;
			}
			set
			{
				this._HistPeriod = value;
			}
		}
		#endregion
		#region DisposalPeriodID
		public abstract class disposalPeriodID : IBqlField
		{
		}
		protected string _DisposalPeriodID;
		[PXDBString(6, IsFixed = true)]
		[GL.FinPeriodIDFormatting]
		public virtual string DisposalPeriodID
		{
			get
			{
				return _DisposalPeriodID;
			}
			set
			{
				_DisposalPeriodID = value;
			}
		}
		#endregion
		#region YtdDeprBase
		public abstract class ytdDeprBase : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdDeprBase;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdDeprBase))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Basis", Enabled = false)]
		public virtual Decimal? YtdDeprBase
		{
			get
			{
				return this._YtdDeprBase;
			}
			set
			{
				this._YtdDeprBase = value;
			}
		}
		#endregion
		#region YtdDepreciated
		public abstract class ytdDepreciated : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdDepreciated;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdDepreciated))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Accum. Depr.", Enabled = false)]
		public virtual Decimal? YtdDepreciated
		{
			get
			{
				return this._YtdDepreciated;
			}
			set
			{
				this._YtdDepreciated = value;
			}
		}
		#endregion
		#region YtdBal
		public abstract class ytdBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdBal;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdBal))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Net Value", Enabled = false)]
		public virtual Decimal? YtdBal
		{
			get
			{
				return this._YtdBal;
			}
			set
			{
				this._YtdBal = value;
			}
		}
		#endregion
		#region YtdAcquired
		public abstract class ytdAcquired : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdAcquired;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdAcquired))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Current Cost", Enabled = false)]
		public virtual Decimal? YtdAcquired
		{
			get
			{
				return this._YtdAcquired;
			}
			set
			{
				this._YtdAcquired = value;
			}
		}
		#endregion
		#region YtdTax179Recap
		public abstract class ytdTax179Recap : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdTax179Recap;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdTax179Recap))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Tax 179 Recapture", Enabled = false)]
		public virtual Decimal? YtdTax179Recap
		{
			get
			{
				return this._YtdTax179Recap;
			}
			set
			{
				this._YtdTax179Recap = value;
			}
		}
		#endregion
		#region YtdBonusRecap
		public abstract class ytdBonusRecap : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdBonusRecap;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdBonusRecap))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Bonus Recapture", Enabled = false)]
		public virtual Decimal? YtdBonusRecap
		{
			get
			{
				return this._YtdBonusRecap;
			}
			set
			{
				this._YtdBonusRecap = value;
			}
		}
		#endregion
		#region YtdRGOL
		public abstract class ytdRGOL : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdRGOL;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdRGOL))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Gain/Loss Amount", Enabled = false)]
		public virtual Decimal? YtdRGOL
		{
			get
			{
				return this._YtdRGOL;
			}
			set
			{
				this._YtdRGOL = value;
			}
		}
		#endregion
		#region PtdDeprDisposed
		public abstract class ptdDeprDisposed : PX.Data.IBqlField
		{
		}
		protected Decimal? _PtdDeprDisposed;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ptdDeprDisposed))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? PtdDeprDisposed
		{
			get
			{
				return this._PtdDeprDisposed;
			}
			set
			{
				this._PtdDeprDisposed = value;
			}
		}
		#endregion
		#region YtdSuspended
		public abstract class ytdSuspended : PX.Data.IBqlField
		{
		}
		protected Int32? _YtdSuspended;
		[PXDBInt(BqlField = typeof(FABookHistory.ytdSuspended))]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? YtdSuspended
		{
			get
			{
				return this._YtdSuspended;
			}
			set
			{
				this._YtdSuspended = value;
			}
		}
		#endregion
		#region YtdReconciled
		public abstract class ytdReconciled : PX.Data.IBqlField
		{
		}
		protected Decimal? _YtdReconciled;
		[PXDBBaseCury(BqlField = typeof(FABookHistory.ytdReconciled))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? YtdReconciled
		{
			get
			{
				return this._YtdReconciled;
			}
			set
			{
				this._YtdReconciled = value;
			}
		}
		#endregion
		#region DisposalAmount
		public abstract class disposalAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _DisposalAmount;
		[PXBaseCury()]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Disposal Amount")]
		public virtual Decimal? DisposalAmount
		{
			get
			{
				return this._DisposalAmount;
			}
			set
			{
				this._DisposalAmount = value;
			}
		}
		#endregion
		#region OrigDeprToDate
		public abstract class origDeprToDate : IBqlField
		{
		}
		protected DateTime? _OrigDeprToDate;
		[PXDBDate]
		public virtual DateTime? OrigDeprToDate
		{
			get
			{
				return this._OrigDeprToDate;
			}
			set
			{
				this._OrigDeprToDate = value;
			}
		}
		#endregion
		#region AllowChangeDeprFromPeriod
		public abstract class allowChangeDeprFromPeriod : IBqlField { }
		[PXBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual bool? AllowChangeDeprFromPeriod { get; set; }
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
		#region FABookPeriod2
		[Serializable]
		[PXHidden]
		public class FABookPeriod2 : FABookPeriod
		{
			public new abstract class bookID : IBqlField { }
			public new abstract class finPeriodID : IBqlField { }
			public new abstract class finYear : IBqlField { }
		}
		#endregion
	}

	[PXProjection(typeof(Select5<FABookBalance,
		InnerJoin<FABookPeriod, On<FABookPeriod.bookID, Equal<FABookBalance.bookID>>,
		LeftJoin<GL.FinPeriod, On<GL.FinPeriod.finPeriodID, Equal<FABookPeriod.finPeriodID>>>>,
		Where<FABookPeriod.finPeriodID, Greater<FABookBalance.lastDeprPeriod>, And<Where<FABookBalance.updateGL, Equal<False>, Or<GL.FinPeriod.fAClosed, Equal<False>>>>>,
		Aggregate<GroupBy<FABookBalance.assetID, GroupBy<FABookBalance.bookID, Min<FABookPeriod.finPeriodID>>>>>))]
	[Serializable]
	[PXHidden]
	public partial class FABookBalanceNext : IBqlTable
	{
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true, BqlField = typeof(FABookBalance.assetID))]
		[PXDefault()]
		public virtual Int32? AssetID
		{
			get
			{
				return this._AssetID;
			}
			set
			{
				this._AssetID = value;
			}
		}
		#endregion
		#region BookID
		public abstract class bookID : PX.Data.IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBInt(IsKey = true, BqlField = typeof(FABookBalance.bookID))]
		[PXDefault()]
		public virtual Int32? BookID
		{
			get
			{
				return this._BookID;
			}
			set
			{
				this._BookID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[GL.FinPeriodID(BqlField = typeof(FABookPeriod.finPeriodID))]
		[PXDefault()]
		public virtual String FinPeriodID
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
	}


	[PXProjection(typeof(Select2<FABookBalance,
	LeftJoin<FABookBalanceNext, On<FABookBalanceNext.assetID, Equal<FABookBalance.assetID>, And<FABookBalanceNext.bookID, Equal<FABookBalance.bookID>>>>,
	Where<FABookBalance.currDeprPeriod, IsNotNull, Or<FABookBalance.lastDeprPeriod, IsNotNull>>>))]
	[Serializable]
	[PXHidden]
	public partial class FABookBalanceTransfer : IBqlTable
	{
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true, BqlField = typeof(FABookBalance.assetID))]
		[PXDefault()]
		public virtual Int32? AssetID
		{
			get
			{
				return this._AssetID;
			}
			set
			{
				this._AssetID = value;
			}
		}
		#endregion
		#region BookID
		public abstract class bookID : PX.Data.IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBInt(IsKey = true, BqlField = typeof(FABookBalance.bookID))]
		[PXDefault()]
		public virtual Int32? BookID
		{
			get
			{
				return this._BookID;
			}
			set
			{
				this._BookID = value;
			}
		}
		#endregion
		#region UpdateGL
		public abstract class updateGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateGL;
		[PXDBBool(BqlField = typeof(FABookBalance.updateGL))]
		public virtual Boolean? UpdateGL
		{
			get
			{
				return this._UpdateGL;
			}
			set
			{
				this._UpdateGL = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[PXDBCalcedAttribute(typeof(Switch<Case<Where<FABookBalance.currDeprPeriod, IsNull>, FABookBalanceNext.finPeriodID>, FABookBalance.currDeprPeriod>), typeof(string))]
		[GL.FinPeriodIDFormatting()]
		public virtual String FinPeriodID
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
	}
}
