using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using System.Diagnostics;
using PX.Objects.CS;
using PX.Objects.EP;

namespace PX.Objects.PM
{
    [Serializable]
	public class ProjectAttributeGroupMaint : PXGraph<ProjectAttributeGroupMaint>
	{

		#region Actions/Buttons
		public PXSave<GroupTypeFilter> Save;
		public PXCancel<GroupTypeFilter> Cancel;
		#endregion

		#region Views/Selects

		public PXFilter<GroupTypeFilter> Filter;
		public PXSelect<CSAttributeGroup,
			Where<CSAttributeGroup.entityClassID, Equal<Current<GroupTypeFilter.classID>>,
			And<CSAttributeGroup.entityType, Equal<Current<GroupTypeFilter.entityType>>>>> Mapping;

		#endregion

	    #region Event Handlers

		protected virtual void CSAttributeGroup_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			CSAttributeGroup row = e.Row as CSAttributeGroup;

			if (row != null && Filter.Current != null)
			{
				row.EntityClassID = Filter.Current.ClassID;
				row.EntityType = Filter.Current.EntityType;
			}
		}

		#endregion

		#region Local Types
        [Serializable]
		public partial class GroupTypeFilter : IBqlTable
		{
			#region ClassID
			public abstract class classID : PX.Data.IBqlField
			{
			}
			protected string _ClassID;
			[PXUIField(DisplayName = "Name", Visibility = PXUIVisibility.SelectorVisible)]
			[PXDBString(20, IsUnicode = true)]
			[GroupTypes.List()]
			[PXDefault(GroupTypes.Project)]
			public virtual String ClassID
			{
				get
				{
					return this._ClassID;
				}
				set
				{
					this._ClassID = value;
				}
			}
			#endregion
			#region EntityType
			public abstract class entityType : PX.Data.IBqlField
			{
			}
			[PXString(200, IsUnicode = true)]
			public virtual String EntityType
			{
				get
				{
					switch (ClassID)
					{
						case GroupTypes.AccountGroup:
					        return typeof (PMAccountGroup).FullName;
						//case GroupTypes.Transaction:
					    //  return CSAnswerType.ProjectTran;
						case GroupTypes.Task:
					        return typeof (PMTask).FullName;
						case GroupTypes.Project:
					        return typeof (PMProject).FullName;
						case GroupTypes.Equipment:
					        return typeof (EPEquipment).FullName;

						default:
							return null;
					}
				}
				set
				{
				}
			}
			#endregion
		}

		#endregion

	}
}
