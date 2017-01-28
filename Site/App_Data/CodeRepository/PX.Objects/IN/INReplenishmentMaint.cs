using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.PO;

namespace PX.Objects.IN
{
	[PXHidden]
	public class INReplenishmentMaint : PXGraph<INReplenishmentMaint,INReplenishmentOrder>
	{
		public PXSelect<INReplenishmentOrder> Document;
		public PXSelect<POLine> planRelease;
		public PXSelect<INItemPlan,
			Where<INItemPlan.refNoteID, Equal<INReplenishmentOrder.noteID>>> Plans;
		public PXSetup<INSetup> setup;

		protected virtual void INReplenishmentOrder_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			PXNoteAttribute.GetNoteID<INReplenishmentOrder.noteID>(sender, e.Row);
		}

		protected virtual void INItemPlan_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			INItemPlan plan = (INItemPlan)e.Row;
			if (plan != null)
			{
				PXCache parent = this.Document.Cache;
				plan.RefNoteID = PXNoteAttribute.GetNoteID<INReplenishmentOrder.noteID>(parent, parent.Current);
			}
		}
	}
}
