using PX.Objects.GL;

namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.CM;

	[System.SerializableAttribute()]
	public partial class PMUnbilledDailySummary : PX.Data.IBqlTable
	{
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDBInt(IsKey = true)]
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
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDBInt(IsKey = true)]
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
		#region Date
		public abstract class date : PX.Data.IBqlField
		{
		}
		protected DateTime? _Date;
		[PXDBDate(IsKey=true)]
		[PXDefault]
		public virtual DateTime? Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				this._Date = value;
			}
		}
		#endregion
		#region Billable
		public abstract class billable : PX.Data.IBqlField
		{
		}
		protected int? _Billable;
		[PXDefault(0)]
		[PXDBInt]
		public virtual int? Billable
		{
			get
			{
				return this._Billable;
			}
			set
			{
				this._Billable = value;
			}
		}
		#endregion
		#region NonBillable
		public abstract class nonBillable : PX.Data.IBqlField
		{
		}
		protected int? _NonBillable;
		[PXDefault(0)]
		[PXDBInt]
		public virtual int? NonBillable
		{
			get
			{
				return this._NonBillable;
			}
			set
			{
				this._NonBillable = value;
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

	public class PMUnbilledDailySummaryAccumAttribute : PXAccumulatorAttribute
	{
		public PMUnbilledDailySummaryAccumAttribute()
		{
			base._SingleRecord = true;
		}
		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}
					
			PMUnbilledDailySummaryAccum item = (PMUnbilledDailySummaryAccum)row;
			columns.Update<PMUnbilledDailySummaryAccum.billable>(item.Billable, PXDataFieldAssign.AssignBehavior.Summarize);
			columns.Update<PMUnbilledDailySummaryAccum.nonBillable>(item.NonBillable, PXDataFieldAssign.AssignBehavior.Summarize);
			
			return true;
		}
	}

	[Serializable]
	[PMUnbilledDailySummaryAccum]
	public partial class PMUnbilledDailySummaryAccum : PMUnbilledDailySummary
	{
		public new abstract class projectID : PX.Data.IBqlField {}
		public new abstract class taskID : PX.Data.IBqlField {}
		public new abstract class accountGroupID : PX.Data.IBqlField { }
		public new abstract class date : PX.Data.IBqlField { }
		public new abstract class billable : PX.Data.IBqlField { }
		public new abstract class nonBillable : PX.Data.IBqlField { }
	}
}
