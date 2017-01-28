using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace PX.Objects.CR
{
	public class CRSMEmailMaint : PXGraph<CRSMEmailMaint>
	{
		public PXSelectReadonly<SMEmail> Email;

		public PXDelete<SMEmail> Delete;
		
		protected virtual void SMEmail_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			SMEmail row = (SMEmail)e.Row;
			
			Delete.SetEnabled(row != null && row.MPStatus != MailStatusListAttribute.Processed);
		}
	}
}
