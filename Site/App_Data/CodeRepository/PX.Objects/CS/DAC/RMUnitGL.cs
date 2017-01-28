using System;
using PX.Data;
using PX.CS;

namespace PX.Objects.CS
{
	[Serializable]
	public partial class RMUnitGL : PXCacheExtension<RMUnit>
	{		
		#region DataSourceID
		public abstract class dataSourceID : IBqlField
		{
		}
		protected Int32? _DataSourceID;
		[RMDataSourceGL()]
		public virtual Int32? DataSourceID
		{
			get
			{
				return this._DataSourceID;
			}
			set
			{
				this._DataSourceID = value;
			}
		}
		#endregion

	}
}
