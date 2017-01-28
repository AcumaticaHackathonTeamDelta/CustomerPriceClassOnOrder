using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.TX;
using PX.SM;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.RQ;

namespace PX.Objects.AP
{	
	public class VendorContactType : NotificationContactType
	{
		public class ClassListAttribute : PXStringListAttribute
		{
			public ClassListAttribute()
				: base(new string[] { Primary, Remittance, Shipping, Employee },
							 new string[] { CR.Messages.Primary, Messages.Remittance, Messages.Shipping, EP.Messages.Employee })
			{
			}
		}
		public new class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { Primary, Remittance, Shipping, Employee, Contact },
							 new string[] { CR.Messages.Primary, Messages.Remittance, Messages.Shipping, EP.Messages.Employee, CR.Messages.Contact })
			{
			}
		}
	}	
	
}
