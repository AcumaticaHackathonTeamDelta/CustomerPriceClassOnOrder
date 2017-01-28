using System;
using PX.Data;

namespace PX.Objects.EP
{
    [Serializable]
	[PXPrimaryGraph(typeof(EPLoginTypeMaint))]
	[PXCacheName(Messages.LoginType)]
	public partial class EPLoginType : IBqlTable
    {

        #region LoginTypeID

        public abstract class loginTypeID : IBqlField {}

        [PXDBIdentity(IsKey = true)]
        public virtual int? LoginTypeID { get; set; }

        #endregion

        #region LoginTypeName

        public abstract class loginTypeName : IBqlField {}

        [PXDBString(50, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "User Type", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(EPLoginType.loginTypeName))]
        public virtual String LoginTypeName { get; set; }

        #endregion
        #region Entity

        public abstract class entity : IBqlField
        {
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
						new string[] { Contact, Employee },
						new string[] { Messages.ContactType, Messages.EmployeeType }
						) { }
			}

			public const string Contact = "C";
			public const string Employee = "E";

			public class contact : Constant<string>
			{
				public contact() : base(Contact) { }
			}
			public class employee : Constant<string>
			{
				public employee() : base(Employee) { }
			}
		}

        [PXDBString(1, IsFixed = true)]
        [PXDefault(entity.Employee)]
		[PXUIField(DisplayName = "Linked Entity", Visibility = PXUIVisibility.SelectorVisible)]
        [entity.List]
        public virtual String Entity { get; set; }
        #endregion
        #region Description

        public abstract class description : IBqlField { }

        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Description")]
        public virtual String Description { get; set; }

        #endregion
        #region IsExternal

        public abstract class isExternal : IBqlField {}

        [PXBool]
		[PXFormula(typeof(Switch<Case<Where<entity, Equal<entity.contact>>, True>, False>))]
        public virtual bool? IsExternal { get; set; }
        #endregion
        #region EmailAsLogin

        public abstract class emailAsLogin : IBqlField { }

        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Use Email as Login")]
		[PXUIEnabled(typeof(Where<EPLoginType.entity, Equal<EPLoginType.entity.contact>>))]
		[PXFormula(typeof(Switch<Case<Where<EPLoginType.entity, Equal<EPLoginType.entity.employee>>, False>, EPLoginType.emailAsLogin>))]
        public virtual bool? EmailAsLogin { get; set; }
        #endregion
        #region ResetPasswordOnLogin

        public abstract class resetPasswordOnLogin : IBqlField { }

        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Reset Password on First Login")]
        public virtual bool? ResetPasswordOnLogin { get; set; }
        #endregion
        #region RequireLoginActivation

        public abstract class requireLoginActivation : IBqlField { }

        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Require Login Activation")]
        public virtual bool? RequireLoginActivation { get; set; }
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

