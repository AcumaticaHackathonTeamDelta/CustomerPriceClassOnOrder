using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.IN
{
    [Serializable]
	public class INCategory : PX.Data.IBqlTable
	{
		#region CategoryID
		public abstract class categoryID : PX.Data.IBqlField
		{
		}
		protected int? _CategoryID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Category ID", Enabled = false, Visibility = PXUIVisibility.Invisible)]
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

		#region CategoryCD
		public abstract class categoryCD : PX.Data.IBqlField
		{
		}
		protected String _CategoryCD;
		[PXDefault()]
		[PXDBString(10, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Category ID")]
		[PX.Data.EP.PXFieldDescription]
		public virtual String CategoryCD
		{
			get
			{
				return this._CategoryCD;
			}
			set
			{
				this._CategoryCD = value;
			}
		}
		#endregion

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBLocalizableString(50, InputMask = "", IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion	

		#region ParentID
		public abstract class parentID : PX.Data.IBqlField
		{
		}
		protected int? _ParentID;
		[PXDBInt]
		[PXDefault(0)]
        [PXUIField(DisplayName = "Parent Category")]
		public virtual int? ParentID
		{
			get
			{
				return this._ParentID;
			}
			set
			{
				this._ParentID = value ?? 0;
			}
		}
		#endregion


        #region TempChildID
        public abstract class tempChildID : PX.Data.IBqlField
        {
        }
        protected int? _TempChildID;
        [PXInt]
        public virtual int? TempChildID
        {
            get
            {
                return this._TempChildID;
            }
            set
            {
                this._TempChildID = value;
            }
        }
        #endregion

        #region TempparentID
        public abstract class tempParentID : PX.Data.IBqlField
        {
        }
        protected int? _TempParentID;
        [PXInt]
        public virtual int? TempParentID
        {
            get
            {
                return this._TempParentID;
            }
            set
            {
                this._TempParentID = value;
            }
        }
        #endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion		

		#region SortOrder
		public abstract class sortOrder : PX.Data.IBqlField
		{
		}
		protected Int32? _SortOrder;
		[PXDefault(0)]
		[PXDBInt]
		public virtual Int32? SortOrder
		{
			get
			{
				return this._SortOrder;
			}
			set
			{
				this._SortOrder = value;
			}
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion

		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
	}
}
