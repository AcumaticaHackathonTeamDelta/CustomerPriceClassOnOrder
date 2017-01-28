using PX.Objects.CM;
using PX.Objects.IN;
using PX.SM;
using System;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.PM
{
	
	[System.SerializableAttribute()]
	[PXProjection(typeof(Select5<PMHistory,
		InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, GreaterEqual<PMHistory.periodID>>>,
	   Aggregate<
	   GroupBy<PMHistory.projectID,
	   GroupBy<PMHistory.projectTaskID,
	   GroupBy<PMHistory.accountGroupID,
	   GroupBy<PMHistory.inventoryID,
	   Max<PMHistory.periodID,
		GroupBy<FinPeriod.finPeriodID
		>>>>>>>>))]
    [PXHidden]
	public class PMHistoryByPeriod : IBqlTable
	{
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDBInt(IsKey = true, BqlField = typeof(PMHistory.projectID))]
		[PXDefault()]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region ProjectTaskID
		public abstract class projectTaskID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectTaskID;
		[PXDBInt(IsKey = true, BqlField = typeof(PMHistory.projectTaskID))]
		[PXDefault()]
		public virtual Int32? ProjectTaskID
		{
			get
			{
				return this._ProjectTaskID;
			}
			set
			{
				this._ProjectTaskID = value;
			}
		}
		#endregion
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDBInt(IsKey = true, BqlField = typeof(PMHistory.accountGroupID))]
		[PXDefault()]
		public virtual Int32? AccountGroupID
		{
			get
			{
				return this._AccountGroupID;
			}
			set
			{
				this._AccountGroupID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true, BqlField = typeof(PMHistory.inventoryID))]
		[PXDefault()]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region PeriodID
		public abstract class periodID : PX.Data.IBqlField
		{
		}
		protected String _PeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(PMHistory.projectID))]
		[PXDefault()]
		[PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(FinPeriod.finPeriodID), DescriptionField = typeof(FinPeriod.descr))]
		public virtual String PeriodID
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
	}
}
