using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.SM;
using PX.Data;
using PX.TM;

namespace PX.SM
{
	[Serializable]
	[PXSubstitute(GraphType = typeof(WikiStatusMaint))]
	public partial class WikiPageStatusFilterExt : WikiPageStatusFilter
	{
		#region OwnerID
		public new abstract class ownerID : PX.Data.IBqlField
		{
		}		
		[PXDBGuid]
		[PXUIField(DisplayName = "Assigned to")]
		[PXSubordinateOwnerSelector]
		public override Guid? OwnerID
		{
			get
			{
				return (_MyOwner == true) ? CurrentOwnerID : _OwnerID;
			}
			set
			{
				_OwnerID = value;
			}
		}
		#endregion
	}
}
