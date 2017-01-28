using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CR;
using PX.SM;

namespace PX.Objects.EP
{
	[PXCacheName(Messages.EarningType)]
	[Serializable]
	public partial class EPEarningType : IBqlTable
	{

		#region TypeCD
		public abstract class typeCD : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[PXDBString(2, IsUnicode = false, IsKey = true, InputMask = ">LL",  IsFixed = true)]
		[PXUIField(DisplayName = "Code", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TypeCD { get; set; }
		#endregion

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual String Description { get; set; }
		#endregion

		#region isOvertime
		public abstract class isOvertime : IBqlField 
		{
		}
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Overtime")]
		public virtual Boolean? IsOvertime { get; set; }
		#endregion

		#region isBillable
		public abstract class isbillable : IBqlField 
		{
		}
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Billable")]
		public virtual Boolean? isBillable { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		[PM.Project(DisplayName = "Default Project Code")]
		public virtual Int32? ProjectID { get; set; }
		#endregion

		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		[PM.ProjectTask(typeof(EPEarningType.projectID), DisplayName = "Default Task", AllowNull = true)]
		public virtual Int32? TaskID { get; set; }
		#endregion

		#region OvertimeMultiplier
		public abstract class overtimeMultiplier : IBqlField
		{
		}
		[PXDBDecimal(2)]
		[PXUIEnabled(typeof(isOvertime))]
		[PXFormula(typeof(Switch<Case<Where<isOvertime, NotEqual<True>>, CS.decimal1>, overtimeMultiplier>))]
		[PXUIField(DisplayName = "Multiplier")]
		public virtual Decimal? OvertimeMultiplier { get; set; }
		#endregion


		#region System Columns
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		[PXDBTimestamp()]
		public virtual Byte[] tstamp { get; set; }
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion

	}
}
