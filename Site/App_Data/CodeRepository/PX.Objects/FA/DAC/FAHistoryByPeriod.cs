namespace PX.Objects.FA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CM;

	/// <summary>
	/// The DAC used to simplify selection and aggregation of proper <see cref="FABookHistory"/> records
	/// on various inquiry and processing screens of the Fixed Assets module.
	/// </summary>
	[System.SerializableAttribute()]
	[PXProjection(typeof(Select5<FABookHistory,
		InnerJoin<FABookPeriod, On<FABookPeriod.bookID, Equal<FABookHistory.bookID>, And<FABookPeriod.finPeriodID, GreaterEqual<FABookHistory.finPeriodID>>>>,
		Aggregate<
		GroupBy<FABookHistory.assetID,
		GroupBy<FABookHistory.bookID,
		Max<FABookHistory.finPeriodID,
		GroupBy<FABookPeriod.finPeriodID>>>>>>))]
	[PXCacheName(FA.Messages.FAHistoryByPeriod)]
	public partial class FAHistoryByPeriod : PX.Data.IBqlTable
	{
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true, BqlField = typeof(FABookHistory.assetID))]
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
		[PXDBInt(IsKey = true, BqlField = typeof(FABookHistory.bookID))]
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
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(FABookPeriod.finPeriodID))]
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;
		[GL.FinPeriodID(BqlField = typeof(FABookHistory.finPeriodID))]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
	}
}
