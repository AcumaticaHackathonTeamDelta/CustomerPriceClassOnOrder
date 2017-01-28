namespace PX.Objects.CA
{
	using PX.Objects.AR.CCPaymentProcessing;
	using PX.CCProcessingBase;
	using PX.Data;
	using PX.Data.EP;
	using PX.Objects.AR;
	using System;

	[PXCacheName(Messages.CCProcessingCenter)]
	[System.SerializableAttribute()]
	public partial class CCProcessingCenter : PX.Data.IBqlTable
	{
		#region ProcessingCenterID
		public abstract class processingCenterID : PX.Data.IBqlField
		{
		}
		protected String _ProcessingCenterID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXSelector(typeof(Search<CCProcessingCenter.processingCenterID>))]
		[PXUIField(DisplayName = "Proc. Center ID",Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ProcessingCenterID
		{
			get
			{
				return this._ProcessingCenterID;
			}
			set
			{
				this._ProcessingCenterID = value;
			}
		}
		#endregion
		#region Name
		public abstract class name : PX.Data.IBqlField
		{
		}
		protected String _Name;
		[PXDBString(255, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Name", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual String Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active")]
		public virtual Boolean? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(CashAccount.descr))]
		[PXDefault()]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
			#endregion
		#region ProcessingTypeName
		public abstract class processingTypeName : PX.Data.IBqlField
		{
		}
		protected String _ProcessingTypeName;
		[PXDBString(255)]
		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[PXProviderTypeSelector(typeof(ICCPaymentProcessing))]
		[PXUIField(DisplayName = "Payment Plug-In (Type)")]
		public virtual String ProcessingTypeName
		{
			get
			{
				return this._ProcessingTypeName;
			}
			set
			{
				this._ProcessingTypeName = value;
			}
		}
		#endregion
		#region ProcessingAssemblyName
		public abstract class processingAssemblyName : PX.Data.IBqlField
		{
		}
		protected String _ProcessingAssemblyName;
		[PXDBString(255)]
		[PXUIField(DisplayName = "Assembly Name")]
		public virtual String ProcessingAssemblyName
		{
			get
			{
				return this._ProcessingAssemblyName;
			}
			set
			{
				this._ProcessingAssemblyName = value;
			}
		}
		#endregion
		#region OpenTranTimeout
		public abstract class openTranTimeout : PX.Data.IBqlField
		{
		}
		protected Int32? _OpenTranTimeout;
		[PXDBInt(MinValue = 0, MaxValue = 60)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Transaction Timeout (s)", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? OpenTranTimeout
		{
			get
			{
				return this._OpenTranTimeout;
			}
			set
			{
				this._OpenTranTimeout = value;
			}
		}
		#endregion
		#region AllowDirectInput
		public abstract class allowDirectInput : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowDirectInput;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Direct Input", Visible = false)]
		public virtual Boolean? AllowDirectInput
		{
			get
			{
				return this._AllowDirectInput;
			}
			set
			{
				this._AllowDirectInput = value;
			}
		}
		#endregion
		#region NeedsExpDateUpdate
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? NeedsExpDateUpdate
		{
			set; get;
		}
		#endregion
		#region SyncronizeDeletion
		public abstract class syncronizeDeletion : PX.Data.IBqlField
		{
		}
		protected Boolean? _syncronizeDeletion;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Synchronize Deletion", Visible = false)]
		public virtual Boolean? SyncronizeDeletion
		{
			get
			{
				return this._syncronizeDeletion;
			}
			set
			{
				this._syncronizeDeletion = value;
			}
		}
		#endregion
		#region SyncRetryAttemptsNo
		public abstract class syncRetryAttemptsNo : PX.Data.IBqlField
		{
		}
		protected Int32? _SyncRetryAttemptsNo;
		[PXDBInt(MinValue = 0, MaxValue = 10)]
		[PXDefault(TypeCode.Int32, "3", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Number of Additional Synchronization Attempts", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? SyncRetryAttemptsNo
		{
			get
			{
				return this._SyncRetryAttemptsNo;
			}
			set
			{
				this._SyncRetryAttemptsNo = value;
			}
		}
		#endregion
		#region SyncRetryDelayMs
		public abstract class syncRetryDelayMs : PX.Data.IBqlField
		{
		}
		protected Int32? _SyncRetryDelayMs;
		[PXDBInt(MinValue = 0, MaxValue = 1000)]
		[PXDefault(TypeCode.Int32, "500", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Delay Between Synchronization Attempts (ms)", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? SyncRetryDelayMs
		{
			get
			{
				return this._SyncRetryDelayMs;
			}
			set
			{
				this._SyncRetryDelayMs = value;
			}
		}
		#endregion
		#region NoteID

		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(CCProcessingCenter.processingCenterID))]
		public virtual Guid? NoteID { get; set; }

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
}
