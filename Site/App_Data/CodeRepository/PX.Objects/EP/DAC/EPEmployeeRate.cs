using System;
using PX.Data;
using PX.Objects.CM;

namespace PX.Objects.EP
{
	[PXCacheName(Messages.EPEmployeeRate)]
	[Serializable]
	public partial class EPEmployeeRate : IBqlTable
	{
		#region RateID
		public abstract class rateID :IBqlField
		{
		}
		protected int? _RateID;
		[PXDBIdentity(IsKey=true)]
		public virtual int? RateID
		{
			get
			{
				return _RateID;
			}
			set
			{
				_RateID = value;
			}
		}
		#endregion
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDBDefault(typeof(EPEmployee.bAccountID))]
		[PXParent(typeof(Select<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPEmployeeRate.employeeID>>>>))]
		public virtual Int32? EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				this._EmployeeID = value;
			}
		}
		#endregion
		#region EffectiveDate
		public abstract class effectiveDate : IBqlField
		{
		}
		protected DateTime? _EffectiveDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXCheckUnique(typeof(EPEmployeeRate.employeeID))]
		[PXUIField(DisplayName = "Effective Date")]
		public virtual DateTime? EffectiveDate
		{
			get
			{
				return _EffectiveDate;
			}
			set
			{
				_EffectiveDate = value;
			}
		}
		#endregion
		#region RateType
		public abstract class rateType : IBqlField
		{
			
		}
		protected string _RateType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(RateTypesAttribute.Hourly)]
		[PXUIField(DisplayName = "Type of Employment", Visibility = PXUIVisibility.SelectorVisible)]
		[RateTypes]
		public virtual string RateType
		{
			get
			{
				return _RateType;
			}
			set
			{
				_RateType = value;
			}
		}
		#endregion
		#region HourlyRate
		public abstract class hourlyRate : IBqlField
		{
		}
		protected decimal? _HourlyRate;
		[IN.PXDBPriceCost]
		[PXUIField(DisplayName = "Hourly Rate")]
		public virtual decimal? HourlyRate
		{
			get
			{
				return _HourlyRate;
			}
			set
			{
				_HourlyRate = value;
			}
		}
		#endregion
		#region RegularHours
		public abstract class regularHours : IBqlField
		{
		}
		protected decimal? _RegularHours;
		[PXDBDecimal(1)]
		[PXDefault()]
		[PXUIField(DisplayName = "Regular Hours per week")]
		public virtual decimal? RegularHours
		{
			get
			{
				return _RegularHours;
			}
			set
			{
				_RegularHours = value;
			}
		}
		#endregion
		#region AnnualSalary
		public abstract class annualSalary : IBqlField
		{
		}
		protected decimal? _AnnualSalary;
		[PXDBBaseCury]
		[PXUIField(DisplayName = "Annual Salary Amount")]
		public virtual decimal? AnnualSalary
		{
			get
			{
				return _AnnualSalary;
			}
			set
			{
				_AnnualSalary = value;
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
