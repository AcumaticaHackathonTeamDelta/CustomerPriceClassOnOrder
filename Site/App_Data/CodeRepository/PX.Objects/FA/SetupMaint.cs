using System;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	public class SetupMaint : PXGraph<SetupMaint>
	{
		public PXSelect<FASetup> FASetupRecord;
		public PXSave<FASetup> Save;
		public PXCancel<FASetup> Cancel;
		
		protected virtual void FASetup_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
            FASetup setup = (FASetup)e.Row;
            if (setup == null) return;

            PXUIFieldAttribute.SetEnabled<FASetup.summPostDepreciation>(FASetupRecord.Cache, setup, setup.SummPost != true);
		}
    }

}
