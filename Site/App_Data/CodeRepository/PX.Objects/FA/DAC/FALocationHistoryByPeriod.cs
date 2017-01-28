using System;
using PX.Data;

namespace PX.Objects.FA.DAC
{
	[PXProjection(typeof(Select5<FixedAsset,
		InnerJoin<FALocationHistory, On<FALocationHistory.assetID, Equal<FixedAsset.assetID>, And<FixedAsset.recordType, Equal<FARecordType.assetType>>>,
		InnerJoin<FABook, On<FABook.updateGL, Equal<True>>,
		InnerJoin<FABookPeriod, On<FABookPeriod.bookID, Equal<FABook.bookID>, And<FALocationHistory.periodID, LessEqual<FABookPeriod.finPeriodID>>>>>>,
		Aggregate<GroupBy<FALocationHistory.assetID, GroupBy<FABookPeriod.finPeriodID, Max<FALocationHistory.periodID, Max<FALocationHistory.revisionID>>>>>>))]
	[Serializable]
	[PXCacheName(Messages.FALocationHistoryByPeriod)]
	public partial class FALocationHistoryByPeriod : IBqlTable
	{
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected int? _AssetID;
		[PXDBInt(IsKey = true, BqlField = typeof(FixedAsset.assetID))]
		[PXDefault()]
		public virtual int? AssetID
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
		#region PeriodID
		public abstract class periodID : PX.Data.IBqlField
		{
		}
		protected string _PeriodID;
		[GL.FinPeriodID(BqlField = typeof(FABookPeriod.finPeriodID))]
		[PXDefault()]
		public virtual string PeriodID
		{
			get
			{
				return this._PeriodID;
			}
			set
			{
				this._PeriodID = value;
			}
		}
		#endregion
		#region LastPeriodID
		public abstract class lastPeriodID : PX.Data.IBqlField
		{
		}
		protected string _LastPeriodID;
		[GL.FinPeriodID(BqlField = typeof(FALocationHistory.periodID))]
		[PXDefault()]
		public virtual string LastPeriodID
		{
			get
			{
				return this._LastPeriodID;
			}
			set
			{
				this._LastPeriodID = value;
			}
		}
		#endregion
		#region LastRevisionID
		public abstract class lastRevisionID : PX.Data.IBqlField
		{
		}
		protected int? _LastRevisionID;
		[PXDBInt(IsKey = true, BqlField = typeof(FALocationHistory.revisionID))]
		[PXDefault(0)]
		public virtual int? LastRevisionID
		{
			get
			{
				return this._LastRevisionID;
			}
			set
			{
				this._LastRevisionID = value;
			}
		}
		#endregion
	}
}
