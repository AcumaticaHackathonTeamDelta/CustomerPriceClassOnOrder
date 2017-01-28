using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.IN
{
    [Serializable]
	public class INItemCategory : PX.Data.IBqlTable
	{
        #region CategoryID
        public abstract class categoryID : PX.Data.IBqlField
        {
        }
        protected int? _CategoryID;
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(INCategory.categoryID))]
        [PXParent(typeof(Select<INCategory, Where<INCategory.categoryID, Equal<Current<INItemCategory.categoryID>>>>))]
        [PXUIField(DisplayName = "Category")]
        public virtual int? CategoryID
        {
            get { return this._CategoryID; }
            set { this._CategoryID = value; }
        }
        #endregion
        
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Visible)]
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemCategory.inventoryID>>>>))]
        [PXSelector(typeof(Search<InventoryItem.inventoryID, Where<Match<Current<AccessInfo.userName>>>>), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion

        #region CategorySelected
        public abstract class categorySelected : PX.Data.IBqlField
        {
        }
        protected bool? _CategorySelected;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Category Selected", Visibility = PXUIVisibility.Service)]
        public virtual bool? CategorySelected
        {
            get
            {
                return this._CategorySelected;
            }
            set
            {
                this._CategorySelected = value;
            }
        }
        #endregion
	}

    [Serializable]
    public class INItemCategoryBuffer : PX.Data.IBqlTable
    {
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [PXInt(IsKey = true)]
        [PXDefault()]
        public virtual Int32? InventoryID
        {
            get
            {
                return this._InventoryID;
            }
            set
            {
                this._InventoryID = value;
            }
        }
        #endregion
    }
}
