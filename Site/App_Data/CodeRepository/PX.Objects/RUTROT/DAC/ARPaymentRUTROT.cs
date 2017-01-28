using System;
using PX.Data;
using PX.Objects.AR;
using PX.Common;

namespace PX.Objects.RUTROT
{
	[System.SerializableAttribute()]
	public partial class ARPaymentRUTROT : PXCacheExtension<ARPayment>
	{
		#region IsRUTROTPayment
		public abstract class isRUTROTPayment : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ROT or RUT payment", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual bool? IsRUTROTPayment { get; set; }
		#endregion
	}
}
