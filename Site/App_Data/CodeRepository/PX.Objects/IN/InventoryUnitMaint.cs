using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;

namespace PX.Objects.IN
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class INUnitMaint : PXGraph<INUnitMaint>
	{
		public PXSelect<INUnit, Where<INUnit.unitType,Equal<INUnitType.global>>> Unit;
        public PXSavePerRow<INUnit> Save;
        public PXCancel<INUnit> Cancel;

        protected virtual void INUnit_FromUnit_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void INUnit_ToUnit_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void INUnit_UnitRate_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
        {
            Decimal? conversion = (Decimal?)e.NewValue;
            if (conversion == 0m && (string)cache.GetValue<INUnit.unitMultDiv>(e.Row) == "D")
            {
                throw new PXSetPropertyException(CS.Messages.Entry_NE, "0");
            }
        }
	}
}
