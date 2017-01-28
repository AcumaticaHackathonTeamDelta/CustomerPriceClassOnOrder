using System;
using System.Collections;

using PX.Data;

using PX.Objects.Common;

namespace PX.Objects.DR
{
    public class DRBalanceValidation : PXGraph<DRBalanceValidation>
    {
        public PXCancel<DRBalanceType> Cancel;
        public PXProcessing<DRBalanceType> Items;

        public virtual IEnumerable items()
        {
            bool found = false;
            foreach (DRBalanceType item in Items.Cache.Inserted)
            {
                found = true;
                yield return item;
            }
            if (found)
                yield break;

            DRBalanceType revenue = new DRBalanceType();
            revenue.AccountType = DeferredAccountType.Income;
            yield return Items.Insert(revenue);


            DRBalanceType expense = new DRBalanceType();
            expense.AccountType = DeferredAccountType.Expense;
            yield return Items.Insert(expense);

        	Items.Cache.IsDirty = false;
        }

        public DRBalanceValidation()
        {
            Items.SetProcessDelegate<DRProcess>(Validate);

            Items.SetProcessCaption(GL.Messages.ProcValidate);
            Items.SetProcessAllCaption(GL.Messages.ProcValidateAll);
        }

        private static void Validate(DRProcess graph, DRBalanceType item)
        {
            graph.Clear();
            graph.RunIntegrityCheck(item);
        }
                
        [Serializable]
        public partial class DRBalanceType : IBqlTable
        {
            #region Selected
            public abstract class selected : IBqlField
            {
            }
            protected bool? _Selected = false;
            [PXBool]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Visible)]
            public bool? Selected
            {
                get
                {
                    return _Selected;
                }
                set
                {
                    _Selected = value;
                }
            }
            #endregion
            #region AccountType
            public abstract class accountType : PX.Data.IBqlField
            {
            }
            protected string _AccountType;
            [PXString(1, IsKey=true)]
            [PXDefault(DeferredAccountType.Income)]
            [LabelList(typeof(DeferredAccountType))]
            [PXUIField(DisplayName = "Balance Type", Enabled=false)]
            public virtual string AccountType
            {
                get
                {
                    return this._AccountType;
                }
                set
                {
                    this._AccountType = value;
                }
            }
            #endregion
        }
    }



    
}
