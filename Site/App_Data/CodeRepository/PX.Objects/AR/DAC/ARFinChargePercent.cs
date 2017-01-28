using PX.Objects.CM;
using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.TX;

namespace PX.Objects.AR
{
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARFinChargePercent)]
	public partial class ARFinChargePercent : PX.Data.IBqlTable
	{
		public const string DefaultStartDate = "01/01/1900";
		#region FinChargeID
		public abstract class finChargeID : PX.Data.IBqlField
		{
		}
		protected String _FinChargeID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXDBDefault(typeof(ARFinCharge.finChargeID))]
		[PXParent(typeof(Select<ARFinCharge, Where<ARFinCharge.finChargeID, Equal<Current<ARFinChargePercent.finChargeID>>>>))]

		public virtual String FinChargeID
		{
			get
			{
				return this._FinChargeID;
			}
			set
			{
				this._FinChargeID = value;
			}
		}
		#endregion
		#region FinChargePercent
		public abstract class finChargePercent : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinChargePercent;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Percent Rate")]
		public virtual Decimal? FinChargePercent
		{
			get
			{
				return this._FinChargePercent;
			}
			set
			{
				this._FinChargePercent = value;
			}
		}
		#endregion
		#region BeginDate
		public abstract class beginDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _BeginDate;
		[PXDBDate(IsKey = true)]
		[PXDefault(TypeCode.DateTime, DefaultStartDate)]
		[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.Visible)]
		[PXCheckUnique(new Type[] { typeof(finChargeID) })]
		public virtual DateTime? BeginDate
		{
			get
			{
				return this._BeginDate;
			}
			set
			{
				this._BeginDate = value;
			}
		}
		#endregion
		#region PercentID
		public abstract class percentID : PX.Data.IBqlField
		{
		}
		protected Int32? _PercentID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? PercentID
		{
			get
			{
				return this._PercentID;
			}
			set
			{
				this._PercentID = value;
			}
		}
		#endregion
	}
}
