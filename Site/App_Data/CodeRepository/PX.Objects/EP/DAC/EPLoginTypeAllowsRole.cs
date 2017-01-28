using System;
using PX.Data;
using PX.SM;

namespace PX.Objects.EP
{
    [Serializable]
	[PXCacheName(Messages.LoginTypeAllowsRole)]
	public partial class EPLoginTypeAllowsRole : IBqlTable
    {
		#region Selected
		public abstract class selected : IBqlField {}
		[PXBool]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected { get; set; }
		#endregion
		#region IsDefault
		public abstract class isDefault : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Default")]
		[PXDefault(false)]
		public bool? IsDefault { get; set; }
		#endregion
		#region LoginType
        public abstract class loginTypeID : IBqlField { }
        [PXDBInt(IsKey = true)]
        [PXDBChildIdentity(typeof(EPLoginType.loginTypeID))]
        [PXParent(typeof(Select<EPLoginType, Where<EPLoginType.loginTypeID, Equal<EPLoginTypeAllowsRole.loginTypeID>>>))]
        [PXDefault(typeof(EPLoginType.loginTypeID))]
        [PXUIField(DisplayName = "User Type")]
        public virtual int? LoginTypeID { get; set; }
        #endregion
        #region Rolename
        public abstract class rolename : IBqlField { }
        [PXDBString(64, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Role Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<Roles.rolename, Where<Current<EPLoginType.isExternal>, Equal<Roles.guest>, Or<Roles.guest, Equal<True>>>>), DescriptionField = typeof(Roles.descr))]
        public virtual String Rolename { get; set; }
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
        #region CreatedByID
        public abstract class createdByID : PX.Data.IBqlField
        {
        }
        protected Guid? _CreatedByID;
        [PXDBCreatedByID()]
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
        [PXDBCreatedByScreenID()]
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
        [PXDBCreatedDateTime()]
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
        [PXDBLastModifiedByID()]
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
        [PXDBLastModifiedByScreenID()]
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

        [PXDBLastModifiedDateTimeUtc]
        [PXUIField(DisplayName = "Last Modified Date", Enabled = false)]
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
