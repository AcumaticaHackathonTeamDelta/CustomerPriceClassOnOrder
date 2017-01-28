using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.EP;

namespace PX.Objects.PM
{
	[System.SerializableAttribute()]
	public partial class PMEmployeeRate : PX.Data.IBqlTable
	{
		#region RateDefinitionID
		public abstract class rateDefinitionID : PX.Data.IBqlField
		{
		}
		protected int? _RateDefinitionID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(PMRateSequence.rateDefinitionID))]
		[PXParent(typeof(Select<PMRateSequence, Where<PMRateSequence.rateDefinitionID, Equal<Current<PMEmployeeRate.rateDefinitionID>>, And<PMRateSequence.rateCodeID, Equal<Current<PMEmployeeRate.rateCodeID>>>>>))]
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
		#region EmployeeID

		public abstract class employeeID : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Employee ID")]
		[PXEPEmployeeSelectorAttribute]
		public virtual Int32? EmployeeID { get; set; }

		#endregion
	}
}
