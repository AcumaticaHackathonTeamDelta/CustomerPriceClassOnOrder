using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.TM;

namespace PX.Objects.EP
{
	[PXProjection(typeof(Select2<EPCompanyTree,
		LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>, And<EPCompanyTreeMember.isOwner, Equal<True>>>,
		LeftJoin<EPEmployee, On<EPEmployee.userID, Equal<EPCompanyTreeMember.userID>>>>>))]
    [Serializable]
    [PXHidden]
	public class EPCompanyTreeOwner : PX.Data.IBqlTable
	{
		#region WorkGroupID
		public abstract class workGroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkGroupID;
		[PXDBIdentity(BqlTable = typeof(EPCompanyTree))]
		[PXUIField(DisplayName = "Work Group", Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public virtual int? WorkGroupID
		{
			get
			{
				return this._WorkGroupID;
			}
			set
			{
				this._WorkGroupID = value;
			}
		}
		#endregion

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(50, IsKey = true, InputMask = "",BqlTable = typeof(EPCompanyTree))]
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

		#region ParentWGID
		public abstract class parentWGID : PX.Data.IBqlField
		{
		}
		protected int? _ParentWGID;
		[PXDBInt(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(0)]
		[PXDBLiteDefault(typeof(EPCompanyTree.workGroupID))]
		public virtual int? ParentWGID
		{
			get
			{
				return this._ParentWGID;
			}
			set
			{
				this._ParentWGID = value ?? 0;
			}
		}
		#endregion

		#region SortOrder
		public abstract class sortOrder : PX.Data.IBqlField
		{
		}
		protected Int32? _SortOrder;
		[PXDefault(0)]
		[PXDBInt(BqlTable = typeof(EPCompanyTree))]
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

		#region WaitTime
		public abstract class waitTime : PX.Data.IBqlField
		{
		}
		protected Int32? _WaitTime;
		[PXDBTimeSpanLong(Format = TimeSpanFormatType.DaysHoursMinites,BqlTable = typeof(EPCompanyTree))]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Wait Time")]
		public virtual Int32? WaitTime
		{
			get
			{
				return this._WaitTime;
			}
			set
			{
				this._WaitTime = value;
			}
		}
		#endregion

		#region BypassEscalation
		public abstract class bypassEscalation : PX.Data.IBqlField
		{
		}
		protected bool? _BypassEscalation;
		[PXDBBool(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Bypass Escalation")]
		public virtual bool? BypassEscalation
		{
			get
			{
				return this._BypassEscalation;
			}
			set
			{
				this._BypassEscalation = value;
			}
		}
		#endregion

		#region UseCalendarTime
		public abstract class wseCalendarTime : PX.Data.IBqlField
		{
		}
		protected bool? _UseCalendarTime;
		[PXDBBool(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use Calendar Time")]
		public virtual bool? UseCalendarTime
		{
			get
			{
				return this._UseCalendarTime;
			}
			set
			{
				this._UseCalendarTime = value;
			}
		}
		#endregion

		#region AccessRights
		public abstract class accessRights : IBqlField
		{
		}
		protected short? _AccessRights;
		[PXDBShort(BqlTable = typeof(EPCompanyTree))]
		[PXDefault((short)0)]
		public short? AccessRights
		{
			get
			{
				return this._AccessRights;
			}
			set
			{
				this._AccessRights = value;
			}
		}
		#endregion

		#region UserID
		public abstract class userID : IBqlField { }
		protected Guid? _UserID ;
		[PXDBGuid(BqlField = typeof(EPCompanyTreeMember.userID))]
		[PXUIField(DisplayName = "User ID")]
		public virtual Guid? UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				this._UserID = value;
			}
		}
		#endregion

		#region OwnerID
		public abstract class ownerID : IBqlField
		{
		}
		protected int? _OwnerID;
		[PXDBInt(BqlField = typeof(EPEmployee.bAccountID))]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Owner ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion
	}


}
