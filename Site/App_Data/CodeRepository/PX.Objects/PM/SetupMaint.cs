using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.GL;

namespace PX.Objects.PM
{
	public class SetupMaint : PXGraph<SetupMaint>
	{
        #region DAC Overrides
        [PXDBIdentity(IsKey = true)]
        protected virtual void PMProject_ContractID_CacheAttached(PXCache sender)
        { }

        [PXDBString(30, IsUnicode = true)]
        [PXDefault()]
        protected virtual void PMProject_ContractCD_CacheAttached(PXCache sender)
        { }

        [PXDBInt]
        protected virtual void PMProject_CustomerID_CacheAttached(PXCache sender)
        { }

        [PXDBBool]
        [PXDefault(true)]
        protected virtual void PMProject_NonProject_CacheAttached(PXCache sender)
        { }

        [PXDBInt]
        protected virtual void PMProject_TemplateID_CacheAttached(PXCache sender)
        { }
        #endregion

		public PXSelect<PMSetup> Setup;
		public PXSave<PMSetup> Save;
		public PXCancel<PMSetup> Cancel;
		public PXSetup<Company> Company;
		public PXSelect<PMProject,
			Where<PMProject.nonProject, Equal<True>>> DefaultProject;

		public SetupMaint()
		{
			PXDefaultAttribute.SetPersistingCheck<PM.PMProject.defaultSubID>(DefaultProject.Cache, null, PXPersistingCheck.Nothing);
		}

		public virtual void PMSetup_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			PMSetup row = (PMSetup)e.Row;
			if (row == null) return;

            PMProject rec = DefaultProject.SelectWindowed(0, 1);
            if (rec == null)
            {
                InsertDefaultProject(row);
            }
            else
            {
                rec.ContractCD = row.NonProjectCode;
                if (DefaultProject.Cache.GetStatus(rec) == PXEntryStatus.Notchanged)
                    DefaultProject.Cache.SetStatus(rec, PXEntryStatus.Updated);
            }
		}
		public virtual void PMSetup_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			PMProject rec = DefaultProject.SelectWindowed(0, 1);
			PMSetup row = (PMSetup)e.Row;
			if (row == null) return;

			if(rec == null)
			{
				InsertDefaultProject(row);
			}
			else if(!sender.ObjectsEqual<PMSetup.nonProjectCode>(e.Row, e.OldRow))
			{
				rec.ContractCD = row.NonProjectCode;
				if (DefaultProject.Cache.GetStatus(rec) == PXEntryStatus.Notchanged)
					DefaultProject.Cache.SetStatus(rec, PXEntryStatus.Updated);
			}
		}
		private void InsertDefaultProject(PMSetup row)
		{
			PMProject rec = new PMProject();
			rec.CustomerID = null;
			rec.ContractCD = row.NonProjectCode;
			rec.Description = Messages.NonProjectDescription;
			rec.StartDate = new DateTime(DateTime.Now.Year, 1, 1);
			rec.CuryID = Company.Current.BaseCuryID;
			rec.IsActive = true;
			rec.Status = ProjectStatus.Active;
			rec.ServiceActivate = false;
            rec = DefaultProject.Insert(rec);
		}

    }
}
