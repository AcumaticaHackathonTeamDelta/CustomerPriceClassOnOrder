namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	public partial class PMTaskRate : PX.Data.IBqlTable
	{
		#region RateDefinitionID
		public abstract class rateDefinitionID : PX.Data.IBqlField
		{
		}
		protected int? _RateDefinitionID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(PMRateSequence.rateDefinitionID))]
		[PXParent(typeof(Select<PMRateSequence, Where<PMRateSequence.rateDefinitionID, Equal<Current<PMTaskRate.rateDefinitionID>>, And<PMRateSequence.rateCodeID, Equal<Current<PMTaskRate.rateCodeID>>>>>))]
		public virtual int? RateDefinitionID
		{
			get
			{
				return this._RateDefinitionID;
			}
			set
			{
				this._RateDefinitionID = value;
			}
		}
		#endregion
		#region RateCodeID
		public abstract class rateCodeID : PX.Data.IBqlField
		{
		}
		protected String _RateCodeID;

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(PMRateSequence.rateCodeID))]
		public virtual String RateCodeID
		{
			get
			{
				return this._RateCodeID;
			}
			set
			{
				this._RateCodeID = value;
			}
		}
		#endregion
		#region TaskCD
		public abstract class taskCD : PX.Data.IBqlField
		{
		}
		protected String _TaskCD;
		[PXDimensionWildcardAttribute(ProjectTaskAttribute.DimensionName,
			typeof(Search4<PMTask.taskCD,Aggregate<GroupBy<PMTask.taskCD>>>),
			typeof(PMTask.taskCD), typeof(PMTask.description))]
		[PXDBString(IsKey = true, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Project Task")]
		public virtual String TaskCD
		{
			get
			{
				return this._TaskCD;
			}
			set
			{
				this._TaskCD = value;
			}
		}
		#endregion
	}
}
