using System;
using PX.Objects.EP;
using PX.Data;

namespace PX.Objects.CR
{
	public class CRActivitySetupMaint : PXGraph<CRActivitySetupMaint>
	{
		#region Selects
		[PXViewName(EP.Messages.ActivityTypes)]
		public PXSelect<EPActivityType> ActivityTypes;
		#endregion

		#region Action

		public PXSave<EPActivityType> Save;
		public PXCancel<EPActivityType> Cancel;
		public PXSelect<EPSetup, Where<EPSetup.defaultActivityType, Equal<Current<EPActivityType.type>>>> epsetup;
		public PXSelect<CRActivity, Where<CRActivity.type, Equal<Current<EPActivityType.type>>>> Activities; 
		#endregion		

		protected virtual void EPActivityType_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			if (!sender.ObjectsEqual<EPActivityType.type>(e.Row, e.NewRow))
			{
				ValidateUsage(e.Row, epsetup, Messages.ActivityTypeUsageChanged);
				ValidateUsage(e.Row, Activities, Messages.ActivityTypeUsageChanged);
			}
		}
		protected virtual void EPActivityType_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			ValidateUsage(e.Row, epsetup, Messages.ActivityTypeUsage);
			ValidateUsage(e.Row, Activities, Messages.ActivityTypeUsage);			
		}

		private void ValidateUsage(object row, PXSelectBase select, string message)
		{
			if(select.View.SelectSingleBound(new object[]{row}) != null)
				throw new PXException(message);
		}
	}
}
