using System.Collections.Generic;
using PX.Data;
using System.Collections;
using PX.Objects.CS;

namespace PX.Objects.CR
{
	public class CROpportunityClassMaint : PXGraph<CROpportunityClassMaint, CROpportunityClass>
	{
		[PXViewName(Messages.OpportunityClass)]
		public PXSelect<CROpportunityClass>
			OpportunityClass;

		[PXHidden]
		public PXSelect<CROpportunityClass,
			Where<CROpportunityClass.cROpportunityClassID, Equal<Current<CROpportunityClass.cROpportunityClassID>>>>
			OpportunityClassProperties;

        [PXViewName(Messages.Attributes)]
        public CSAttributeGroupList<CROpportunityClass, CROpportunity> Mapping;

        [PXHidden]
		public PXSelect<CRSetup>
			Setup;

		protected virtual void CROpportunityClass_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			var row = e.Row as CROpportunityClass;
			if (row == null) return;
			
			CRSetup s = Setup.Select();
			if (s != null && s.DefaultOpportunityClassID == row.CROpportunityClassID)
			{
				s.DefaultOpportunityClassID = null;
				Setup.Update(s);
			}
		}

		protected virtual void CROpportunityClass_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CROpportunityClass;
			if (row == null) return;

			Delete.SetEnabled(CanDelete(row));
		}

		protected virtual void CROpportunityClass_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			var row = e.Row as CROpportunityClass;
			if (row == null) return;
			
			if (!CanDelete(row))
			{
				throw new PXException(Messages.RecordIsReferenced);
			}
		}

		private bool CanDelete(CROpportunityClass row)
		{
			if (row != null)
			{
				CROpportunity c = PXSelect<CROpportunity, 
					Where<CROpportunity.cROpportunityClassID, Equal<Required<CRContactClass.classID>>>>.
					SelectWindowed(this, 0, 1, row.CROpportunityClassID);
				if (c != null)
				{
					return false;
				}
			}

			return true;
		}

	}
}

