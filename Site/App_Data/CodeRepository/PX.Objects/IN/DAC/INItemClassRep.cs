namespace PX.Objects.IN
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
	public partial class INItemClassRep : PX.Data.IBqlTable
	{
		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDefault(typeof(INItemClass.itemClassID))]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Class ID", Visible = false)]
		[PXSelector(typeof(Search<INItemClass.itemClassID>), DirtyRead = true)]
		[PXParent(typeof(Select<INItemClass, Where<INItemClass.itemClassID, Equal<Current<INItemClassRep.itemClassID>>>>))]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion
		
		#region ReplenishmentClassID
		public abstract class replenishmentClassID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentClassID;
		[PXDefault()]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Replenishment Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INReplenishmentClass.replenishmentClassID>))]
		public virtual String ReplenishmentClassID
		{
			get
			{
				return this._ReplenishmentClassID;
			}
			set
			{
				this._ReplenishmentClassID = value;
			}
		}
		#endregion
		#region ReplenishmentMethod
		public abstract class replenishmentMethod : PX.Data.IBqlField
		{
		}
		protected string _ReplenishmentMethod;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Method")]		
		[PXDefault(INReplenishmentMethod.None)]
		[INReplenishmentMethod.List]
		public virtual string ReplenishmentMethod
		{
			get
			{
				return this._ReplenishmentMethod;
			}
			set
			{
				this._ReplenishmentMethod = value;
			}
		}
		#endregion

		#region ReplenishmentSource
		public abstract class replenishmentSource : PX.Data.IBqlField
		{
		}
		protected string _ReplenishmentSource;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Source")]
		[PXDefault(
			INReplenishmentSource.Purchased,
			typeof(Select<INReplenishmentClass, Where<INReplenishmentClass.replenishmentClassID, Equal<Current<INItemClassRep.replenishmentClassID>>>>),
			SourceField = typeof(INReplenishmentClass.replenishmentSource))]
		[INReplenishmentSource.List]
		[PXFormula(typeof(Default<INItemClassRep.replenishmentClassID>))]
		public virtual string ReplenishmentSource
		{
			get
			{
				return this._ReplenishmentSource;
			}
			set
			{
				this._ReplenishmentSource = value;
			}
		}
		#endregion
		#region ReplenishmentSourceSiteID
		public abstract class replenishmentSourceSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReplenishmentSourceSiteID;
		[IN.ReplenishmentSourceSite(typeof(INItemClassRep.replenishmentSource), DisplayName = "Replenishment Warehouse")]
		public virtual Int32? ReplenishmentSourceSiteID
		{
			get
			{
				return this._ReplenishmentSourceSiteID;
			}
			set
			{
				this._ReplenishmentSourceSiteID = value;
			}
		}
		#endregion
		#region ReplenishmentPolicyID
		public abstract class replenishmentPolicyID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentPolicyID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Seasonality")]
		[PXSelector(typeof(Search<INReplenishmentPolicy.replenishmentPolicyID>), DescriptionField = typeof(INReplenishmentPolicy.descr))]
		[PXDefault()]
		public virtual String ReplenishmentPolicyID
		{
			get
			{
				return this._ReplenishmentPolicyID;
			}
			set
			{
				this._ReplenishmentPolicyID = value;
			}
		}
		#endregion
		#region TransferLeadTime
		public abstract class transferLeadTime : PX.Data.IBqlField
		{
		}
		protected Int16? _TransferLeadTime;
		[PXDBShort(MinValue = 0, MaxValue = 20000)]
		[PXDefault((short)0,PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Transfer Lead Time")]
		public virtual Int16? TransferLeadTime
		{
			get
			{
				return this._TransferLeadTime;
			}
			set
			{
				this._TransferLeadTime = value;
			}
		}
		#endregion
		#region TransferERQ
		public abstract class transferERQ : PX.Data.IBqlField
		{
		}
		protected Decimal? _TransferERQ;
		[PXDBQuantity(MinValue=0)]
		[PXDefault(TypeCode.Decimal,"0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Transfer ERQ")]
		public virtual Decimal? TransferERQ
		{
			get
			{
				return this._TransferERQ;
			}
			set
			{
				this._TransferERQ = value;
			}
		}
		#endregion
		
		#region ServiceLevel
		public abstract class serviceLevel : PX.Data.IBqlField
		{
		}
		protected decimal? _ServiceLevel;
		[PXDBDecimal(6, MinValue = 0.500001, MaxValue = 0.999999)]
		[PXUIField(DisplayName = "Service Level", Visible = true)]
		[PXDefault(TypeCode.Decimal, "0.8400000")]
		public virtual decimal? ServiceLevel
		{
			get
			{
				return this._ServiceLevel;
			}
			set
			{
				this._ServiceLevel = value;
			}
		}
		#endregion
		#region ServiceLevelPct
		public abstract class serviceLevelPct : PX.Data.IBqlField
		{
		}

		[PXDecimal(4, MinValue = 50.0001, MaxValue = 99.9999)]
		[PXUIField(DisplayName = "Service Level (%)", Visible = true)]
		[PXDefault(TypeCode.Decimal, "84.0000")]
		public virtual decimal? ServiceLevelPct
		{
			[PXDependsOnFields(typeof(serviceLevel))]
			get
			{
				return this._ServiceLevel * 100.0m;
			}
			set
			{
				this._ServiceLevel = value / 100.0m;
			}
		}
		#endregion

		#region ForecastModelType
		public abstract class forecastModelType : PX.Data.IBqlField
		{
		}
		protected String _ForecastModelType;
		[PXDBString(3, IsFixed = true)]
		[DemandForecastModelType.List()]
		[PXUIField(DisplayName = "Demand Forecast Model")]
		[PXDefault(DemandForecastModelType.None, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String ForecastModelType
		{
			get
			{
				return this._ForecastModelType;
			}
			set
			{
				this._ForecastModelType = value;
			}
		}
		#endregion
		#region ForecastPeriodType
		public abstract class forecastPeriodType : PX.Data.IBqlField
		{
		}
		protected String _ForecastPeriodType;
		[PXDBString(2, IsFixed = true)]
		[DemandPeriodType.List()]
		[PXDefault(DemandPeriodType.Month, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Forecast Period Type")]

		public virtual String ForecastPeriodType
		{
			get
			{
				return this._ForecastPeriodType;
			}
			set
			{
				this._ForecastPeriodType = value;
			}
		}
		#endregion

		#region HistoryDepth
		public abstract class historyDepth : PX.Data.IBqlField
		{
		}
		protected Int32? _HistoryDepth;
		[PXDBInt(MinValue = 0, MaxValue = 20000)]
		[PXDefault(0,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Periods to Analyze")]
		public virtual Int32? HistoryDepth
		{
			get
			{
				return this._HistoryDepth;
			}
			set
			{
				this._HistoryDepth = value;
			}
		}
		#endregion
		#region ESSmoothingConstantL
		public abstract class eSSmoothingConstantL : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantL;
		[PXDBDecimal(9, MinValue = 0.0, MaxValue = 1.0)]
		[PXUIField(DisplayName = "Level Smoothing Constant")]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? ESSmoothingConstantL
		{
			get
			{
				return this._ESSmoothingConstantL;
			}
			set
			{
				this._ESSmoothingConstantL = value;
			}
		}
		#endregion
		#region ESSmoothingConstantT
		public abstract class eSSmoothingConstantT : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantT;
		[PXDBDecimal(9)]
		[PXUIField(DisplayName = "Trend Smoothing Constant")]

		public virtual Decimal? ESSmoothingConstantT
		{
			get
			{
				return this._ESSmoothingConstantT;
			}
			set
			{
				this._ESSmoothingConstantT = value;
			}
		}
		#endregion
		#region ESSmoothingConstantS
		public abstract class eSSmoothingConstantS : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantS;
		[PXDBDecimal(9)]
		[PXUIField(DisplayName = "Seasonality Smoothing Constant")]
		public virtual Decimal? ESSmoothingConstantS
		{
			get
			{
				return this._ESSmoothingConstantS;
			}
			set
			{
				this._ESSmoothingConstantS = value;
			}
		}
		#endregion
		#region AutoFitModel
		public abstract class autoFitModel : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoFitModel;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Auto Fit Model", Visibility = PXUIVisibility.Invisible)]
		public virtual Boolean? AutoFitModel
		{
			get
			{
				return this._AutoFitModel;
			}
			set
			{
				this._AutoFitModel = value;
			}
		}
		#endregion

		#region LaunchDate
		public abstract class launchDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LaunchDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Launch Date")]
		public virtual DateTime? LaunchDate
		{
			get
			{
				return this._LaunchDate;
			}
			set
			{
				this._LaunchDate = value;
			}
		}
		#endregion
		#region TerminationDate
		public abstract class terminationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TerminationDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Termination Date")]
		public virtual DateTime? TerminationDate
		{
			get
			{
				return this._TerminationDate;
			}
			set
			{
				this._TerminationDate = value;
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
}
