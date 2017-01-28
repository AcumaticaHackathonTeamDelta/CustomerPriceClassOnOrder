using System;
using PX.Data;﻿

namespace PX.Objects.GL
 {
	 [System.SerializableAttribute()]
	 [PXPrimaryGraph(typeof(GLBudgetTreeMaint))]
	[PXCacheName(Messages.GLBudgetTree)]
	public partial class GLBudgetTree : PX.Data.IBqlTable, PX.SM.IIncludable
	 {
		 #region GroupID
		 public abstract class groupID : PX.Data.IBqlField
		 {
		 }
		 protected Guid? _GroupID;
		 [PXDBGuid(IsKey=true)]
		 [PXDefault()]
		 [PXUIField(DisplayName = "GroupID", Visibility = PXUIVisibility.Invisible)]
		 public virtual Guid? GroupID
		 {
			 get
			 {
				 return this._GroupID;
			 }
			 set
			 {
				 this._GroupID = value;
			 }
		 }
		 #endregion
		 #region ParentGroupID
		 public abstract class parentGroupID : PX.Data.IBqlField
		 {
		 }
		 protected Guid? _ParentGroupID;
		 [PXDBGuid()]
		 [PXUIField(DisplayName = "ParentGroupID", Visibility = PXUIVisibility.Invisible)]
		 public virtual Guid? ParentGroupID
		 {
			 get
			 {
				 return this._ParentGroupID;
			 }
			 set
			 {
				 this._ParentGroupID = value;
			 }
		 }
		 #endregion
		 #region SortOrder
		 public abstract class sortOrder : PX.Data.IBqlField
		 {
		 }
		 protected int? _SortOrder;
		 [PXDBInt()]
		 [PXDefault(0)]
		 [PXUIField(DisplayName = "Sort Order", Visibility = PXUIVisibility.Invisible)]
		 public virtual int? SortOrder
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
		 #region IsGroup
		 public abstract class isGroup : PX.Data.IBqlField
		 {
		 }
		 protected bool? _IsGroup;
		 [PXDBBool()]
		 [PXUIField(DisplayName = "Node")]
		 [PXDefault(false)]
		 public virtual bool? IsGroup
		 {
			 get
			 {
				 return this._IsGroup;
			 }
			 set
			 {
				 this._IsGroup = value;
			 }
		 }
		 #endregion
		 #region Rollup
		 public abstract class rollup : PX.Data.IBqlField
		 {
		 }
		 protected bool? _Rollup;
		 [PXDBBool()]
		 [PXUIField(DisplayName = "Rollup", Enabled = false)]
		 [PXDefault(false)]
		 public virtual bool? Rollup
		 {
			 get
			 {
				 return this._Rollup;
			 }
			 set
			 {
				 this._Rollup = value;
			 }
		 }
		 #endregion
		 #region AccountID
		 public abstract class accountID : PX.Data.IBqlField
		 {
		 }
		 protected Int32? _AccountID;
		 [PXDBInt()]
         [PXDimensionSelector(AccountAttribute.DimensionName, (typeof(Search<Account.accountID, Where<Account.accountCD, Like<Current<SelectedNode.accountMaskWildcard>>>, OrderBy<Asc<Account.accountCD>>>)), typeof(Account.accountCD), DescriptionField = typeof(Account.description))]
		 [PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible)]
		 public virtual Int32? AccountID
		 {
			 get
			 {
				 return this._AccountID;
			 }
			 set
			 {
				 this._AccountID = value;
			 }
		 }
		 #endregion
		 #region Description
		 public abstract class description : PX.Data.IBqlField
		 {
		 }
		 protected string _Description;
		 [PXDBString(150, IsUnicode = true)]
		 [PXUIField(DisplayName = "Description", Required = true)]
		 [PXDefault(typeof(Search<Account.description, Where<Account.accountID, Equal<Current<GLBudgetTree.accountID>>>>))]
		 public virtual string Description
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
		 #region SubID
		 public abstract class subID : PX.Data.IBqlField
		 {
		 }
		 protected int? _SubID;
		 [SubAccount]
		 public virtual int? SubID
		 {
			 get
			 {
				 return this._SubID;
			 }
			 set
			 {
				 this._SubID = value;
			 }
		 }
		 #endregion
		 #region AccountMask
		 public abstract class accountMask : PX.Data.IBqlField
		 {
		 }
		 protected string _AccountMask;
		 [PXDBString(10, IsUnicode = true)]
		 [PXUIField(DisplayName = "Account Mask", Required = false)]
		 [PXDefault(typeof(Search<Account.accountCD, Where<Account.accountID, Equal<Current<GLBudgetTree.accountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		 public virtual string AccountMask
		 {
			 get
			 {
				 return this._AccountMask;
			 }
			 set
			 {
				 this._AccountMask = value;
			 }
		 }
		 #endregion
		 #region SubMask
		 public abstract class subMask : PX.Data.IBqlField
		 {
		 }
		 protected string _SubMask;
		 [PXDBString(30, IsUnicode = true)]
		 [PXUIField(DisplayName = "Subaccount Mask", Required = false)]
		 public virtual string SubMask
		 {
			 get
			 {
				 return this._SubMask;
			 }
			 set
			 {
				 this._SubMask = value;
			 }
		 }
		 #endregion
		 #region Secured
		 public abstract class secured : PX.Data.IBqlField
		 {
		 }
		 protected bool? _Secured;
		 [PXBool]
		 [PXUIField(DisplayName = "Secured", Enabled=false)]
		 [PXUnboundDefault(false)]
		 public virtual bool? Secured
		 {
			 get
			 {
				 return this._Secured;
			 }
			 set
			 {
				 this._Secured = value;
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
		 protected string _CreatedByScreenID;
		 [PXDBCreatedByScreenID()]
		 public virtual string CreatedByScreenID
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
		 protected string _LastModifiedByScreenID;
		 [PXDBLastModifiedByScreenID()]
		 public virtual string LastModifiedByScreenID
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
		 [PXDBLastModifiedDateTime()]
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
		 #region TStamp
		 public abstract class tStamp : PX.Data.IBqlField
		 {
		 }
		 protected byte[] _TStamp;
		 [PXDBTimestamp()]
		 public virtual byte[] TStamp
		 {
			 get
			 {
				 return this._TStamp;
			 }
			 set
			 {
				 this._TStamp = value;
			 }
		 }
		 #endregion
		 #region GroupMask
		 public abstract class groupMask : IBqlField { }
		 [PXDBGroupMask]
		 public virtual byte[] GroupMask { get; set; }
		 #endregion
		 #region Included
		 public abstract class included : PX.Data.IBqlField
		 {
		 }
		 protected bool? _Included;
		 [PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		 [PXBool]
		 [PXUIField(DisplayName = "Included")]
		 public virtual bool? Included
		 {
			 get
			 {
				 return this._Included;
			 }
			 set
			 {
				 this._Included = value;
			 }
		 }
		 #endregion
	 }
 }
