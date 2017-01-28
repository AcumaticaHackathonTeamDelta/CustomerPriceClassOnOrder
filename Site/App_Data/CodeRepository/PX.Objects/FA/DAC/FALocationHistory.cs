using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.EP;
using PX.Objects.CS;

namespace PX.Objects.FA
{
	[Serializable]
	[PXCacheName(Messages.FALocationHistory)]
	public partial class FALocationHistory : PX.Data.IBqlTable, IFALocation
	{
		#region AssetID
		public abstract class assetID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true)]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		[PXParent(typeof(Select<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FALocationHistory.assetID>>>>))]
		[PXDBLiteDefault(typeof(FixedAsset.assetID))]
		public virtual Int32? AssetID
		{
			get
			{
				return this._AssetID;
			}
			set
			{
				this._AssetID = value;
			}
		}
		#endregion
		#region TransactionType
		public abstract class transactionType : PX.Data.IBqlField
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
					new string[] { Displacement, ChangeResponsible },
					new string[] { Messages.Displacement, Messages.ChangeResponsible }) { ; }
			}

			public const string Displacement = "D";
			public const string ChangeResponsible = "R";

			public class displacement : Constant<string>
			{
				public displacement() : base(Displacement) { ;}
			}
			public class changeResponsible : Constant<string>
			{
				public changeResponsible() : base(ChangeResponsible) { ;}
			}
		}
		protected String _TransactionType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(transactionType.Displacement)]
		[transactionType.List()]
		[PXUIField(DisplayName = "Transaction Type")]
		public virtual String TransactionType
		{
			get
			{
				return this._TransactionType;
			}
			set
			{
				this._TransactionType = value;
			}
		}
		#endregion
		#region TransactionDate
		public abstract class transactionDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TransactionDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Transaction Date")]
		[PXDefault()]
		public virtual DateTime? TransactionDate
		{
			get
			{
				return this._TransactionDate;
			}
			set
			{
				this._TransactionDate = value;
			}
		}
		#endregion
		#region PeriodID
		public abstract class periodID : IBqlField
		{
		}
		protected string _PeriodID;
		[PeriodID]
		public virtual string PeriodID
		{
			get
			{
				return _PeriodID;
			}
			set
			{
				_PeriodID = value;
			}
		}
		#endregion
		#region BuildingID
		public abstract class buildingID : IBqlField
		{
		}
		protected int? _BuildingID;
		[PXDBInt]
		[PXSelector(typeof(Search<Building.buildingID, Where<Building.branchID, Equal<Current<FALocationHistory.locationID>>>>), 
			SubstituteKey=typeof(Building.buildingCD), DescriptionField = typeof(Building.description))]
		[PXUIField(DisplayName = "Building")]
		public virtual int? BuildingID
		{
			get
			{
				return this._BuildingID;
			}
			set
			{
				this._BuildingID = value;
			}
		}
		#endregion
		#region Floor
		public abstract class floor : PX.Data.IBqlField
		{
		}
		protected String _Floor;
		[PXDBString(5, IsUnicode = true)]
		[PXUIField(DisplayName = "Floor")]
		public virtual String Floor
		{
			get
			{
				return this._Floor;
			}
			set
			{
				this._Floor = value;
			}
		}
		#endregion
		#region Room
		public abstract class room : PX.Data.IBqlField
		{
		}
		protected String _Room;
		[PXDBString(5, IsUnicode = true)]
		[PXUIField(DisplayName = "Room")]
		public virtual String Room
		{
			get
			{
				return this._Room;
			}
			set
			{
				this._Room = value;
			}
		}
		#endregion
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected int? _EmployeeID;
		[PXDBInt]
		[PXSelector(typeof(EPEmployee.bAccountID), SubstituteKey = typeof(EPEmployee.acctCD), DescriptionField = typeof(EPEmployee.acctName))]
		[PXUIField(DisplayName = "Custodian")]
		public virtual int? EmployeeID
		{
			get
			{
				return _EmployeeID;
			}
			set
			{
				_EmployeeID = value;
			}
		}
		#endregion
		#region Custodian
		public abstract class custodian : PX.Data.IBqlField
		{
		}
		protected Guid? _Custodian;
		[PXDBField()]
		[PXFormula(typeof(Selector<FALocationHistory.employeeID, EPEmployee.userID>))]
		public virtual Guid? Custodian
		{
			get
			{
				return this._Custodian;
			}
			set
			{
				this._Custodian = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Branch(typeof(Coalesce<
			Search2<Location.vBranchID, InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<Location.bAccountID>, And<EPEmployee.defLocationID, Equal<Location.locationID>>>>, Where<EPEmployee.bAccountID, Equal<Current<FALocationHistory.employeeID>>>>,
			Search<Branch.branchID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>), IsDetail = false)]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region Department
		public abstract class department : PX.Data.IBqlField
		{
		}
		protected String _Department;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<EPEmployee.departmentID, Where<EPEmployee.bAccountID, Equal<Current<FALocationHistory.employeeID>>>>))]
		[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
		[PXUIField(DisplayName = "Department")]
		public virtual String Department
		{
			get
			{
				return this._Department;
			}
			set
			{
				this._Department = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXDBInt(IsKey = true)]
		[PXDefault(0)]
		public virtual Int32? RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region PrevRevisionID
		public abstract class prevRevisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _PrevRevisionID;
		[PXInt]
		[PXDBCalced(typeof(Sub<FALocationHistory.revisionID, int1>), typeof(Int32))]
		public virtual Int32? PrevRevisionID
		{
			get
			{
				return this._PrevRevisionID;
			}
			set
			{
				this._PrevRevisionID = value;
			}
		}
		#endregion
		#region FAAccountID
		public abstract class fAAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _FAAccountID;
		[PXDefault(typeof(Search<FixedAsset.fAAccountID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>))]
		[Account(DisplayName = "Fixed Assets Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? FAAccountID
		{
			get
			{
				return this._FAAccountID;
			}
			set
			{
				this._FAAccountID = value;
			}
		}
		#endregion
		#region FASubID
		public abstract class fASubID : PX.Data.IBqlField
		{
		}
		protected Int32? _FASubID;
		[PXDefault]
		[SubAccount(typeof(FALocationHistory.fAAccountID), DisplayName = "Fixed Assets Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? FASubID
		{
			get
			{
				return this._FASubID;
			}
			set
			{
				this._FASubID = value;
			}
		}
		#endregion
		#region AccumulatedDepreciationAccountID
		public abstract class accumulatedDepreciationAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccumulatedDepreciationAccountID;
		[PXDefault(typeof(Search<FixedAsset.accumulatedDepreciationAccountID,
								 Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>))]
		[Account(DisplayName = "Accumulated Depreciation Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXUIRequired(typeof(Parent<FixedAsset.depreciable>))]
		public virtual Int32? AccumulatedDepreciationAccountID
		{
			get
			{
				return this._AccumulatedDepreciationAccountID;
			}
			set
			{
				this._AccumulatedDepreciationAccountID = value;
			}
		}
		#endregion
		#region AccumulatedDepreciationSubID
		public abstract class accumulatedDepreciationSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccumulatedDepreciationSubID;
		[PXDefault]
		[SubAccount(typeof(FALocationHistory.accumulatedDepreciationAccountID), DisplayName = "Accumulated Depreciation Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXUIRequired(typeof(Parent<FixedAsset.depreciable>))]
		public virtual Int32? AccumulatedDepreciationSubID
		{
			get
			{
				return this._AccumulatedDepreciationSubID;
			}
			set
			{
				this._AccumulatedDepreciationSubID = value;
			}
		}
		#endregion
		#region DepreciatedExpenseAccountID
		public abstract class depreciatedExpenseAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _DepreciatedExpenseAccountID;
		[PXDefault(typeof(Search<FixedAsset.depreciatedExpenseAccountID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>))]
		[Account(DisplayName = "Depreciation Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXUIRequired(typeof(Parent<FixedAsset.depreciable>))]
		public virtual Int32? DepreciatedExpenseAccountID
		{
			get
			{
				return this._DepreciatedExpenseAccountID;
			}
			set
			{
				this._DepreciatedExpenseAccountID = value;
			}
		}
		#endregion
		#region DepreciatedExpenseSubID
		public abstract class depreciatedExpenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _DepreciatedExpenseSubID;
		[PXDefault]
		[SubAccount(typeof(FALocationHistory.depreciatedExpenseAccountID), DisplayName = "Depreciation Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXUIRequired(typeof(Parent<FixedAsset.depreciable>))]
		public virtual Int32? DepreciatedExpenseSubID
		{
			get
			{
				return this._DepreciatedExpenseSubID;
			}
			set
			{
				this._DepreciatedExpenseSubID = value;
			}
		}
		#endregion

		#region DisposalAccountID
		public abstract class disposalAccountID : IBqlField {}
		[PXDefault(typeof(Search<FixedAsset.disposalAccountID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>),
					PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Proceeds Account", DescriptionField = typeof(Account.description))]
		public virtual int? DisposalAccountID { get; set; }
		#endregion
		#region DisposalSubID
		public abstract class disposalSubID : IBqlField {}
		[SubAccount(typeof(FALocationHistory.disposalAccountID), DisplayName = "Proceeds Sub.", DescriptionField = typeof(Sub.description))]
		public virtual int? DisposalSubID { get; set; }
		#endregion

		#region GainAcctID
		public abstract class gainAcctID : IBqlField {}
		[PXDefault(typeof(Search<FixedAsset.gainAcctID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>))]
		[Account(DisplayName = "Gain Account", DescriptionField = typeof(Account.description))]
		public virtual int? GainAcctID { get; set; }
		#endregion
		#region GainSubID
		public abstract class gainSubID : IBqlField {}
		[PXDefault]
		[SubAccount(typeof(FALocationHistory.gainAcctID), DescriptionField = typeof(Sub.description), DisplayName = "Gain Sub.")]
		public virtual int? GainSubID { get; set; }
		#endregion

		#region LossAcctID
		public abstract class lossAcctID : IBqlField {}
		[PXDefault(typeof(Search<FixedAsset.lossAcctID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>))]
		[Account(DisplayName = "Loss Account", DescriptionField = typeof(Account.description))]
		public virtual int? LossAcctID { get; set; }
		#endregion
		#region LossSubID
		public abstract class lossSubID : IBqlField {}
		[PXDefault]
		[SubAccount(typeof(FALocationHistory.lossAcctID), DescriptionField = typeof(Sub.description), DisplayName = "Loss Sub.")]
		public virtual int? LossSubID { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDBDefault(typeof(FARegister.refNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region Reason
		public abstract class reason : PX.Data.IBqlField
		{
		}
		protected String _Reason;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Reason")]
		public virtual String Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
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
		[PXDBLastModifiedDateTime]
		[PXUIField(DisplayName = "Modification Date")]
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
		#region IFALocation Members
		public virtual Int32? BranchID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
	}
	
}
