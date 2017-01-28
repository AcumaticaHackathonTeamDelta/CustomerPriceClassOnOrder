using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CM;

namespace PX.Objects.PM
{
    [TaskTotalAccum]
    [Serializable]
    public partial class PMTaskTotal : PX.Data.IBqlTable
    {
        #region ProjectID
        public abstract class projectID : PX.Data.IBqlField
        {
        }
        protected Int32? _ProjectID;
        [PXDBInt(IsKey = true)]
        public virtual Int32? ProjectID
        {
            get
            {
                return this._ProjectID;
            }
            set
            {
                this._ProjectID = value;
            }
        }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.IBqlField
        {
        }
        protected Int32? _TaskID;
        [PXDBInt(IsKey=true)]
        public virtual Int32? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion
       
        #region Asset
        public abstract class asset : PX.Data.IBqlField
        {
        }
        protected Decimal? _Asset;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Asset", Enabled = false)]
        public virtual Decimal? Asset
        {
            get
            {
                return this._Asset;
            }
            set
            {
                this._Asset = value;
            }
        }
        #endregion
        #region Liability
        public abstract class liability : PX.Data.IBqlField
        {
        }
        protected Decimal? _Liability;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Liability", Enabled = false)]
        public virtual Decimal? Liability
        {
            get
            {
                return this._Liability;
            }
            set
            {
                this._Liability = value;
            }
        }
        #endregion
        #region Income
        public abstract class income : PX.Data.IBqlField
        {
        }
        protected Decimal? _Income;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Income", Enabled = false)]
        public virtual Decimal? Income
        {
            get
            {
                return this._Income;
            }
            set
            {
                this._Income = value;
            }
        }
        #endregion
        #region Expense
        public abstract class expense : PX.Data.IBqlField
        {
        }
        protected Decimal? _Expense;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Expense", Enabled = false)]
        public virtual Decimal? Expense
        {
            get
            {
                return this._Expense;
            }
            set
            {
                this._Expense = value;
            }
        }
        #endregion

        #region tstamp
        public abstract class Tstamp : PX.Data.IBqlField
        {
        }
        protected Byte[] _tstamp;
        [PXDBTimestamp()]
        public virtual Byte[] tstamp
        {
            get
            {
                return this._tstamp;
            }
            set
            {
                this._tstamp = value;
            }
        }
        #endregion
    }

    public class TaskTotalAccumAttribute : PXAccumulatorAttribute
    {
        public TaskTotalAccumAttribute()
        {
            base._SingleRecord = true;
        }
        protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
        {
            if (!base.PrepareInsert(sender, row, columns))
            {
                return false;
            }

            PMTaskTotal item = (PMTaskTotal)row;

            columns.Update<PMTaskTotal.asset>(item.Asset, PXDataFieldAssign.AssignBehavior.Summarize);
            columns.Update<PMTaskTotal.liability>(item.Liability, PXDataFieldAssign.AssignBehavior.Summarize);
            columns.Update<PMTaskTotal.income>(item.Income, PXDataFieldAssign.AssignBehavior.Summarize);
            columns.Update<PMTaskTotal.expense>(item.Expense, PXDataFieldAssign.AssignBehavior.Summarize);
			
            return true;
        }
    }

}
