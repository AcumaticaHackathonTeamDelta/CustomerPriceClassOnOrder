namespace PX.Objects.AR
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARSPCommissionYear)]
	public partial class ARSPCommissionYear : PX.Data.IBqlTable
	{
		#region Year
		public abstract class year : PX.Data.IBqlField
		{
		}
		protected String _Year;
		[PXDBString(4, IsKey=true, IsFixed = true)]
		[PXDefault()]
		public virtual String Year
		{
			get
			{
				return this._Year;
			}
			set
			{
				this._Year = value;
			}
		}
		#endregion
		#region Filed
		public abstract class filed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Filed;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Filed
		{
			get
			{
				return this._Filed;
			}
			set
			{
				this._Filed = value;
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
}
