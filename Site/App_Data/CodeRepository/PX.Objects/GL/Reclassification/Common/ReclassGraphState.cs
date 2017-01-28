using System.Collections.Generic;
using PX.Data;

namespace PX.Objects.GL.Reclassification.Common
{
	public class ReclassGraphState : IBqlTable
	{
		public virtual ReclassScreenMode ReclassScreenMode { get; set; }

		public virtual string EditingBatchModule { get; set; }
		public virtual string EditingBatchNbr { get; set; }
		public virtual string EditingBatchFinPeriodID { get; set; }
		public virtual string EditingBatchCuryID { get; set; }

		public virtual string OrigBatchModuleToReverse { get; set; }
		public virtual string OrigBatchNbrToReverse { get; set; }

		public virtual HashSet<GLTranForReclassification> GLTranForReclassToDelete { get; set; }

		public ReclassGraphState()
		{
			GLTranForReclassToDelete = new HashSet<GLTranForReclassification>();
		}
	}
}
