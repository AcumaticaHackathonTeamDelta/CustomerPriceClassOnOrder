using System;
using PX.Data;
using PX.Objects.PM;

namespace PX.Objects.EP
{
    [PXCacheName(Messages.EPEmployeeRateByProject)]
	[Serializable]
	public partial class EPEmployeeRateByProject : IBqlTable
	{
		#region RateID
		public abstract class rateID :IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(EPEmployeeRate.rateID))]
		[PXUIField(Visible = false)]
		public virtual int? RateID { get; set; }
		#endregion

		#region Line
		public abstract class line : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(EPEmployeeRate))]
		[PXParent(typeof(Select<EPEmployeeRate, Where<EPEmployeeRate.rateID, Equal<Current<EPEmployeeRateByProject.rateID>>>>), LeaveChildren = true)]
		[PXUIField(Visible = false)]
		public virtual Int32? Line { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : IBqlField { }

		[Project(WarnIfCompleted = true)]
		public virtual Int32? ProjectID { get; set; }
		#endregion

		#region TaskID
		public abstract class taskID : IBqlField { }

		[ProjectTask(typeof(EPEmployeeRateByProject.projectID), DisplayName = "Task", AllowNull = true)]
		[PXCheckUnique(typeof(EPEmployeeRateByProject.rateID), typeof(EPEmployeeRateByProject.projectID), IgnoreNulls = false)]
		public virtual Int32? TaskID { get; set; }
		#endregion

		#region HourlyRate
		public abstract class hourlyRate : IBqlField { }

		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Hourly Rate")]
		public virtual decimal? HourlyRate { get; set; }
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

	}
}
