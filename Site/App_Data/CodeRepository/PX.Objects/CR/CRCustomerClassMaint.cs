using System.Collections.Generic;
using PX.Data;
using System.Collections;
using PX.Objects.CS;

namespace PX.Objects.CR
{
	public class CRCustomerClassMaint : PXGraph<CRCustomerClassMaint, CRCustomerClass>
	{

		[PXViewName(Messages.CustomerClass)]
		public PXSelect<CRCustomerClass>
			CustomerClass;

        [PXViewName(Messages.Attributes)]
        public CSAttributeGroupList<CRCustomerClass, BAccount> Mapping;

        [PXHidden]
		public PXSelect<CRSetup> 
			Setup;

		protected virtual void CRCustomerClass_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CRCustomerClass;
			if (row == null) return;
			
			Delete.SetEnabled(CanDelete(row));
		}

		protected virtual void CRCustomerClass_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			var row = e.Row as CRCustomerClass;
			if (row == null) return;
			
			CRSetup s = Setup.Select();

			if (s != null && s.DefaultCustomerClassID == row.CRCustomerClassID)
			{
				s.DefaultCustomerClassID = null;
				Setup.Update(s);
			}
		}

		protected virtual void CRCustomerClass_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			var row = e.Row as CRCustomerClass;
			if (row == null) return;
			
			if (!CanDelete(row))
			{
				throw new PXException(Messages.RecordIsReferenced);
			}
		}

		private bool CanDelete(CRCustomerClass row)
		{
			if (row != null)
			{
				BAccount c = PXSelect<BAccount, 
					Where<BAccount.classID, Equal<Required<BAccount.classID>>>>.
					SelectWindowed(this, 0, 1, row.CRCustomerClassID);
				if (c != null)
				{
					return false;
				}
			}

			return true;
		}
	}
}
