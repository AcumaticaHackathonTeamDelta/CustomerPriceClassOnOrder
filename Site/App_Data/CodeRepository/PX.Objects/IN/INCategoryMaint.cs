using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.SM;
using PX.Web.UI;

namespace PX.Objects.IN
{
    [Serializable]
    public class INCategoryMaint : PXGraph<INCategoryMaint>
    {
        public static class AddItemsTypesList
        {
            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute()
                    : base(
                    new string[] { AddAllItems, AddItemsByClass },
                    new string[] { "All Items", "By Class" })
                {; }
            }
            public const string AddAllItems = "A";
            public const string AddItemsByClass = "I";
        }

        #region DAC
        [Serializable]
        public partial class ClassFilter : IBqlTable
        {
            #region PriceBasis
            public abstract class addItemsTypes : PX.Data.IBqlField
            {
            }
            protected String _AddItemsTypes;
            [PXString(1, IsFixed = true)]
            [PXUIField(DisplayName = ActionsMessages.AddItems)]
            [AddItemsTypesList.List()]
            [PXDefault(AddItemsTypesList.AddItemsByClass)]
            public virtual String AddItemsTypes
            {
                get
                {
                    return this._AddItemsTypes;
                }
                set
                {
                    this._AddItemsTypes = value;
                }
            }
            #endregion

            #region ItemClassID
            public abstract class itemClassID : PX.Data.IBqlField
            {
            }
            protected String _ItemClassID;
            [PXString(10, IsUnicode = true)]
            [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
            [PXSelector(typeof(Search<INItemClass.itemClassID>), DescriptionField = typeof(INItemClass.descr))]
            public virtual String ItemClassID
            {
                get
                {
                    return this._ItemClassID;
                }
                set
                {
                    this._ItemClassID = value;
                }
            }
            #endregion
        }

        [Serializable]
        [PXHidden]
        public partial class SelectedNode : IBqlTable
        {
            #region FolderID
            public abstract class folderID : PX.Data.IBqlField
            {
            }
            protected int? _FolderID;
            [PXInt()]
            [PXUIField(Visible = false)]
            public virtual int? FolderID
            {
                get
                {
                    return this._FolderID;
                }
                set
                {
                    this._FolderID = value;
                }
            }
            #endregion

            #region CategoryID
            public abstract class categoryID : PX.Data.IBqlField
            {
            }
            protected int? _CategoryID;
            [PXInt()]
            [PXUIField(Visible = false)]
            public virtual int? CategoryID
            {
                get
                {
                    return this._CategoryID;
                }
                set
                {
                    this._CategoryID = value;
                }
            }
            #endregion
        }

        [SerializableAttribute]
        public class INFolderCategory : INCategory
        {
        }

        [SerializableAttribute]
        public class INCategoryCurrent : INCategory
        {
        }

        #endregion

        #region Select
        public PXFilter<ClassFilter> ClassInfo;
        public PXFilter<SelectedNode> SelectedFolders;
        public PXSelectOrderBy<INCategory, OrderBy<Asc<INCategory.sortOrder>>> Folders;
        public PXSelect<INCategory, Where<INCategory.categoryID, Equal<Current<INCategory.categoryID>>>> CurrentCategory;
        public PXSelectJoin<INItemCategory,
                LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<INItemCategory.inventoryID>>>,
                Where<INItemCategory.categoryID, Equal<Current<INCategory.categoryID>>>>
            Members;

        public PXSelectOrderBy<INFolderCategory, OrderBy<Asc<INFolderCategory.sortOrder>>> ParentFolders;

        public PXFilter<INItemCategoryBuffer> Buffer;

        #endregion

        #region delegates
        protected virtual IEnumerable folders(
            [PXInt]
            int? categoryID
        )
        {
            if (categoryID == null)
            {
                yield return new INCategory()
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }
            foreach (INCategory item in PXSelect<INCategory,
            Where<INCategory.parentID,
                Equal<Required<INCategory.parentID>>>,
                OrderBy<Asc<INCategory.sortOrder>>>.Select(this, categoryID))
            {
                if (!string.IsNullOrEmpty(item.Description))
                    yield return item;
            }
        }

        protected virtual IEnumerable currentCategory()
        {
            if (Folders.Current != null)
            {
                PXUIFieldAttribute.SetEnabled<INCategory.description>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                PXUIFieldAttribute.SetEnabled<INCategory.parentID>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                Caches[typeof(INItemCategory)].AllowInsert = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowDelete = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowUpdate = Folders.Current.ParentID != null;
                Actions["Copy"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Cut"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Paste"].SetEnabled(Folders.Current.ParentID != null);
                Actions["AddItemsbyClass"].SetEnabled(Folders.Current.ParentID != null);

                foreach (INCategory item in PXSelect<INCategory,
                Where<INCategory.categoryID, Equal<Required<INCategory.categoryID>>>>.
                Select(this, Folders.Current.CategoryID))
                {
                    yield return item;
                }
            }
        }

        protected virtual IEnumerable members()
        {
            if (Folders.Current != null)
            {
                PXUIFieldAttribute.SetEnabled<INCategory.description>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                PXUIFieldAttribute.SetEnabled<INCategory.parentID>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                Caches[typeof(INItemCategory)].AllowInsert = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowUpdate = Folders.Current.ParentID != null;
                Actions["Copy"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Cut"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Paste"].SetEnabled(Folders.Current.ParentID != null);
                Actions["AddItemsbyClass"].SetEnabled(Folders.Current.ParentID != null);

                PXSelectBase<INItemCategory> cmd;
                cmd = new PXSelectJoin<INItemCategory,
                    LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<INItemCategory.inventoryID>>>,
                Where<INItemCategory.categoryID, Equal<Required<INCategory.categoryID>>>>(this);

                int startRow = PXView.StartRow;
                int totalRows = 0;

                foreach (
                    var res in
                        cmd.View.Select(PXView.Currents, new object[] { Folders.Current.CategoryID }, PXView.Searches, PXView.SortColumns, PXView.Descendings,
                            PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
                {
                    yield return res;
                    PXView.StartRow = 0;
                }
            }
        }

        protected virtual IEnumerable parentFolders(
            [PXInt]
            int? categoryID
        )
        {
            if (categoryID == null)
            {
                yield return new INFolderCategory()
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }
            foreach (INFolderCategory item in PXSelect<INFolderCategory,
            Where<INFolderCategory.parentID,
                Equal<Required<INFolderCategory.categoryID>>>,
                OrderBy<Asc<INFolderCategory.sortOrder>>>.Select(this, categoryID))
            {
                if (!string.IsNullOrEmpty(item.Description) && item.CategoryID != Folders.Current.CategoryID)
                    yield return item;
            }
        }
        #endregion

        #region Actions
        public PXSave<SelectedNode> Save;
        public PXCancel<SelectedNode> Cancel;

        public PXAction<SelectedNode> AddCategory;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton()]
        public virtual IEnumerable addCategory(PXAdapter adapter)
        {
            int ParentID = (int)Folders.Current.CategoryID;
            var inserted = (INCategory) Caches[typeof(INCategory)].Insert(new INCategory
            {
                CategoryCD = "<NEW>",
                Description = "<NEW>",
                ParentID = ParentID,
            });
            inserted.TempChildID = inserted.CategoryID;
            inserted.TempParentID = ParentID;
            INCategory previous = PXSelect<INCategory,
                Where<INCategory.parentID, Equal<Required<INCategory.parentID>>>,
                OrderBy<Desc<INCategory.sortOrder>>>.SelectSingleBound(this, null, ParentID);

            int sortOrder = (int)previous.SortOrder;
            sortOrder = sortOrder + 1;
            inserted.SortOrder = previous != null ? sortOrder : 1;
            Folders.Cache.ActiveRow = inserted;
            return adapter.Get();
        }

        public PXAction<SelectedNode> DeleteCategory;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton()]
        public virtual IEnumerable deleteCategory(PXAdapter adapter)
        {
            // ToDo recursive delete 
            Caches[typeof(INCategory)].Delete(Folders.Current);
            return adapter.Get();
        }



        public PXAction<SelectedNode> down;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageUrl = "~/Icons/NavBar/xps_collapse.gif", DisabledImageUrl = "~/Icons/NavBar/xps_collapseD.gif")]
        public virtual IEnumerable Down(PXAdapter adapter)
        {
            INCategory curr = Folders.Current;
            INCategory next = PXSelect<INCategory,
                Where<INCategory.parentID, Equal<Required<INCategory.parentID>>,
                    And<INCategory.sortOrder, Greater<Required<INCategory.parentID>>>>,
                OrderBy<Asc<INCategory.sortOrder>>>.SelectSingleBound(this, null, Folders.Current.ParentID,
                    Folders.Current.SortOrder);

            if (next != null && curr != null)
            {
                int temp = (int)curr.SortOrder;
                curr.SortOrder = next.SortOrder;
                next.SortOrder = temp;
                Caches[typeof(INCategory)].Update(next);
                Caches[typeof(INCategory)].Update(curr);
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> up;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageUrl = "~/Icons/NavBar/xps_expand.gif", DisabledImageUrl = "~/Icons/NavBar/xps_expandD.gif")]
        public virtual IEnumerable Up(PXAdapter adapter)
        {
            INCategory curr = Folders.Current;
            INCategory prev = PXSelect<INCategory,
                Where<INCategory.parentID, Equal<Required<INCategory.parentID>>,
                    And<INCategory.sortOrder, Less<Required<INCategory.parentID>>>>,
                OrderBy<Desc<INCategory.sortOrder>>>.SelectSingleBound(this, null, Folders.Current.ParentID,
                    Folders.Current.SortOrder);

            if (prev != null && curr != null)
            {
                int temp = (int)curr.SortOrder;
                curr.SortOrder = prev.SortOrder;
                prev.SortOrder = temp;
                Caches[typeof(INCategory)].Update(prev);
                Caches[typeof(INCategory)].Update(curr);
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> Copy;
        [PXButton(ImageKey = Sprite.Main.Copy, Tooltip = ActionsMessages.ttipCopyRec)]
        [PXUIField(DisplayName = ActionsMessages.CopyRec, Enabled = false)]
        public IEnumerable copy(PXAdapter adapter)
        {
            Buffer.Cache.Clear();
            foreach (INItemCategory pxResult in PXSelect<INItemCategory,
            Where<INItemCategory.categoryID, Equal<Required<INItemCategory.categoryID>>>>.
            Select(this, Folders.Current.CategoryID))
            {
                if (pxResult.CategorySelected == true)
                {
                    INItemCategoryBuffer insertnode = Buffer.Cache.CreateInstance() as INItemCategoryBuffer;
                    insertnode.InventoryID = pxResult.InventoryID;
                    Buffer.Cache.Insert(insertnode);
                }
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> Cut;
        [PXButton(ImageKey = Sprite.Main.Cut, Tooltip = ActionsMessages.ttipCut)]
        [PXUIField(DisplayName = ActionsMessages.Cut, Enabled = false)]
        internal IEnumerable cut(PXAdapter adapter)
        {
            Buffer.Cache.Clear();
            var delbuffer = new List<INItemCategory>();
            foreach (INItemCategory pxResult in PXSelect<INItemCategory,
            Where<INItemCategory.categoryID, Equal<Required<INItemCategory.categoryID>>>,
            OrderBy<Asc<InventoryItem.inventoryCD>>>.Select(this, Folders.Current.CategoryID))
            {
                if (pxResult.CategorySelected == true)
                {
                    INItemCategoryBuffer insertnode = Buffer.Cache.CreateInstance() as INItemCategoryBuffer;
                    insertnode.InventoryID = pxResult.InventoryID;
                    Buffer.Cache.Insert(insertnode);
                    delbuffer.Add(pxResult);
                }
            }

            foreach (INItemCategory pxResult in delbuffer)
            {
                Members.Cache.Delete(pxResult);
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> Paste;
        [PXButton(ImageKey = Sprite.Main.Paste, Tooltip = ActionsMessages.ttipPaste)]
        [PXUIField(DisplayName = ActionsMessages.Paste, Enabled = false)]
        internal IEnumerable paste(PXAdapter adapter)
        {
            foreach (INItemCategoryBuffer pxResult in Buffer.Cache.Cached)
            {
                INItemCategory insertnode = Members.Cache.CreateInstance() as INItemCategory;
                insertnode.InventoryID = pxResult.InventoryID;
                Members.Cache.Insert(insertnode);
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> AddItemsbyClass;
        [PXButton(Tooltip = ActionsMessages.AddItems)]
        [PXUIField(DisplayName = ActionsMessages.AddItems, Enabled = false)]
        internal IEnumerable addItemsbyClass(PXAdapter adapter)
        {
            if (ClassInfo.AskExt() == WebDialogResult.OK)
            {
                if (ClassInfo.Current.AddItemsTypes == AddItemsTypesList.AddAllItems)
                {
                    foreach (InventoryItem pxResult in PXSelect<InventoryItem>.Select(this))
                    {
                        INItemCategory insertnode = Members.Cache.CreateInstance() as INItemCategory;
                        insertnode.InventoryID = pxResult.InventoryID;
                        insertnode.CategorySelected = false;
                        Members.Cache.Update(insertnode);

                    }
                }
                else
                {
                    foreach (InventoryItem pxResult in PXSelect<InventoryItem, Where<InventoryItem.itemClassID, Equal<Required<InventoryItem.itemClassID>>>>.Select(this, ClassInfo.Current.ItemClassID))
                    {
                        INItemCategory insertnode = Members.Cache.CreateInstance() as INItemCategory;
                        insertnode.InventoryID = pxResult.InventoryID;
                        insertnode.CategorySelected = false;
                        Members.Cache.Update(insertnode);
                    }
                }
            }
            Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<SelectedNode> ViewDetails;
        [PXButton()]
        [PXUIField(DisplayName = "Inventory Details", Visible = false)]
        public virtual IEnumerable viewDetails(PXAdapter adapter)
        {
            if (Members.Current != null)
            {
                InventoryItem _inventoryItem = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.
                    SelectSingleBound(this, null, Members.Current.InventoryID);

                if (_inventoryItem != null)
                {
                    if (_inventoryItem.StkItem == true)
                    {
                        InventoryItemMaint graph = PXGraph.CreateInstance<InventoryItemMaint>();
                        PXResult result = graph.Item.Search<InventoryItem.inventoryID>(Members.Current.InventoryID);
                        if (result != null)
                        {
                            InventoryItem _ninventoryItem = result[typeof(InventoryItem)] as InventoryItem;
                            graph.Item.Current = _ninventoryItem;
                            PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Same);
                        }
                    }

                    else
                    {
                        NonStockItemMaint graph = PXGraph.CreateInstance<NonStockItemMaint>();
                        PXResult result = graph.Item.Search<InventoryItem.inventoryID>(Members.Current.InventoryID);
                        if (result != null)
                        {
                            InventoryItem _sinventoryItem = result[typeof(InventoryItem)] as InventoryItem;
                            graph.Item.Current = _sinventoryItem;
                            PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Same);
                        }
                    }
                }
            }
            return adapter.Get();
        }
        #endregion

        #region Event Handler
        protected virtual void INCategory_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
        {
            INCategory row = e.Row as INCategory;
            if (row == null || row.CategoryID == null) return;
            deleteRecurring(row);
        }


        protected virtual void INItemCategory_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            INItemCategory row = e.Row as INItemCategory;
            if (row != null)
            {
                PXDefaultAttribute.SetDefault<INItemCategory.categorySelected>(cache, row, false);
            }
        }

        protected virtual void INItemCategoryBuffer_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void ClassFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            ClassFilter row = e.Row as ClassFilter;
            if (row == null) return;
            if (row.AddItemsTypes == AddItemsTypesList.AddAllItems)
            {
                row.ItemClassID = null;
            }
            PXUIFieldAttribute.SetEnabled<ClassFilter.itemClassID>(cache, row, row.AddItemsTypes != AddItemsTypesList.AddAllItems);
        }

        public override void Persist()
        {
            Buffer.Cache.Clear();
            base.Persist();
            foreach (INCategory item in Caches[typeof(INCategory)].Cached)
            {
                if (item.TempParentID < 0)
                {
                    foreach (INCategory item2 in Caches[typeof(INCategory)].Cached)
                    {
                        if (item2.TempChildID == item.TempParentID)
                        {
                            item.ParentID = item2.CategoryID;
                            item.TempParentID = item2.CategoryID;
                            Caches[typeof(INCategory)].SetStatus(item, PXEntryStatus.Updated);
                        }
                    }
                }
            }
            base.Persist();
            Members.View.RequestRefresh();
        }

        private void deleteRecurring(INCategory map)
        {
            if (map != null)
            {
                foreach (INCategory child in PXSelect<INCategory,
                                                 Where<INCategory.parentID, Equal<Required<INCategory.categoryID>>>>
                                             .Select(this, map.CategoryID))
                    deleteRecurring(child);
                Caches[typeof(INCategory)].Delete(map);
            }
        }
        #endregion
    }
}
