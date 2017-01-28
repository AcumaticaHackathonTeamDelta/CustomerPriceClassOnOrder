using PX.Objects.CR;
using PX.Objects.GL;
using PX.Data;

namespace PX.Objects.CS
{
	public class BuildingMaint : PXGraph<BuildingMaint, Branch>
	{
		#region Selects
		public PXSetup<Company> company;
		public PXSelectJoin<Branch, LeftJoin<Ledger, On<Ledger.defBranchID, Equal<Branch.branchID>>>, Where2<MatchWithBranch<Branch.branchID>, And<Ledger.ledgerID, IsNull>>> filter;
		public PXSelect<Building, Where<Building.branchID, Equal<Current<Branch.branchID>>>> building;
		#endregion

        [PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXDBLiteDefault(typeof(BAccount.acctCD))]
        [PXDimensionSelector("BIZACCT", typeof(Search2<Branch.branchID, LeftJoin<Ledger, On<Ledger.defBranchID, Equal<Branch.branchID>>>, Where2<MatchWithBranch<Branch.branchID>, And<Ledger.ledgerID, IsNull>>>), typeof(Branch.branchCD))]
        [PXUIField(DisplayName = "Branch", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void Branch_BranchCD_CacheAttached(PXCache sender){}

		public BuildingMaint()
		{
			filter.Cache.AllowDelete = false;
            filter.Cache.AllowInsert = false;
        }

        public virtual void Branch_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }
	}
}

