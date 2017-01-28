namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.CS;

	[PXCacheName(Messages.INLocation)]
	[System.SerializableAttribute()]
	public partial class INLocation : PX.Data.IBqlTable
	{
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[PXDBForeignIdentity(typeof(INCostSite))]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(IsKey = true, Visible = false)]
		[PXParent(typeof(Select<INSite, Where<INSite.siteID,Equal<Current<INLocation.siteID>>>>))]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region LocationCD
		public abstract class locationCD : PX.Data.IBqlField
		{
		}
		protected String _LocationCD;
		[IN.LocationRaw(IsKey=true)]
		[PXDefault()]
		public virtual String LocationCD
		{
			get
			{
				return this._LocationCD;
			}
			set
			{
				this._LocationCD = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region IsCosted
		public abstract class isCosted : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsCosted;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cost Separately")]
		public virtual Boolean? IsCosted
		{
			get
			{
				return this._IsCosted;
			}
			set
			{
				this._IsCosted = value;
			}
		}
		#endregion
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected int? _CostSiteID;
		[PXDBCalced(typeof(Switch<Case<Where<INLocation.isCosted, Equal<boolTrue>>, INLocation.locationID>, INLocation.siteID>), typeof(int))]
		public virtual int? CostSiteID
		{
			get
			{
				return this._CostSiteID;
			}
			set
			{
				this._CostSiteID = value;
			}
		}
		#endregion
		#region InclQtyAvail
		public abstract class inclQtyAvail : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyAvail;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName="Include in Qty. Available")]
		public virtual Boolean? InclQtyAvail
		{
			get
			{
				return this._InclQtyAvail;
			}
			set
			{
				this._InclQtyAvail = value;
			}
		}
		#endregion
		#region AssemblyValid
		public abstract class assemblyValid : PX.Data.IBqlField
		{
		}
		protected Boolean? _AssemblyValid;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Assembly Allowed")]
		public virtual Boolean? AssemblyValid
		{
			get
			{
				return this._AssemblyValid;
			}
			set
			{
				this._AssemblyValid = value;
			}
		}
		#endregion
		#region PickPriority
		public abstract class pickPriority : PX.Data.IBqlField
		{
		}
		protected Int16? _PickPriority;
		[PXDBShort(MinValue = 0, MaxValue = 999)]
		[PXDefault((short)1)]
		[PXUIField(DisplayName = "Pick Priority")]
		public virtual Int16? PickPriority
		{
			get
			{
				return this._PickPriority;
			}
			set
			{
				this._PickPriority = value;
			}
		}
		#endregion
		#region SalesValid
		public abstract class salesValid : PX.Data.IBqlField
		{
		}
		protected Boolean? _SalesValid;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Sales Allowed")]
		public virtual Boolean? SalesValid
		{
			get
			{
				return this._SalesValid;
			}
			set
			{
				this._SalesValid = value;
			}
		}
		#endregion
		#region ReceiptsValid
		public abstract class receiptsValid : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReceiptsValid;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Receipts Allowed")]
		public virtual Boolean? ReceiptsValid
		{
			get
			{
				return this._ReceiptsValid;
			}
			set
			{
				this._ReceiptsValid = value;
			}
		}
		#endregion
		#region TransfersValid
		public abstract class transfersValid : PX.Data.IBqlField
		{
		}
		protected Boolean? _TransfersValid;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Transfers Allowed")]
		public virtual Boolean? TransfersValid
		{
			get
			{
				return this._TransfersValid;
			}
			set
			{
				this._TransfersValid = value;
			}
		}
		#endregion
		#region PrimaryItemValid
		public abstract class primaryItemValid : PX.Data.IBqlField
		{
		}
		protected String _PrimaryItemValid;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INPrimaryItemValid.PrimaryNothing)]
		[INPrimaryItemValid.List()]
		[PXUIField(DisplayName="Primary Item Validation")]
		public virtual String PrimaryItemValid
		{
			get
			{
				return this._PrimaryItemValid;
			}
			set
			{
				this._PrimaryItemValid = value;
			}
		}
		#endregion
        #region PrimaryItemID
		public abstract class primaryItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _PrimaryItemID;
		[StockItem(DisplayName="Primary Item")]
		public virtual Int32? PrimaryItemID
		{
			get
			{
				return this._PrimaryItemID;
			}
			set
			{
				this._PrimaryItemID = value;
			}
		}
		#endregion
		#region PrimaryItemClassID
		public abstract class primaryItemClassID : PX.Data.IBqlField
		{
		}
		protected string _PrimaryItemClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Primary Item Class")]
		[PXSelector(typeof(Search<INItemClass.itemClassID>))]
		public virtual string PrimaryItemClassID
		{
			get
			{
				return this._PrimaryItemClassID;
			}
			set
			{
				this._PrimaryItemClassID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PM.Project]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		[PM.ProjectTask(typeof(INLocation.projectID), AllowNull = true)]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region Active
		public abstract class active : IBqlField
		{
		}
		protected bool? _Active;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active
		{
			get
			{
				return _Active;
			}
			set
			{
				_Active = value;
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
		public const string Main = "MAIN";
		public class main : Constant<string>
		{
			public main()
				: base(Main)
			{
			}
		}
	}

	public class INPrimaryItemValid
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { PrimaryNothing, PrimaryItemWarning, PrimaryItemError, PrimaryItemClassWarning, PrimaryItemClassError }, 
                new string[] { Messages.PrimaryNothing, Messages.PrimaryItemWarning, Messages.PrimaryItemError,  Messages.PrimaryItemClassWarning, Messages.PrimaryItemClassError })
			{ 
			}
		}

		public const string PrimaryNothing = "N";
		public const string PrimaryItemError = "I";
		public const string PrimaryItemClassError = "C";
        public const string PrimaryItemWarning = "X";
        public const string PrimaryItemClassWarning = "Y";


		public class primaryNothing : Constant<string>
		{
			public primaryNothing() : base(PrimaryNothing) { ;}
		}

		public class primaryItem : Constant<string>
		{
			public primaryItem() : base(PrimaryItemError) { ;}
		}

		public class primaryItemClass : Constant<string>
		{
			public primaryItemClass() : base(PrimaryItemClassError) { ;}
		}

        public class primaryItemWarn : Constant<string>
        {
            public primaryItemWarn() : base(PrimaryItemWarning) { ;}
        }

        public class primaryItemClassWarn : Constant<string>
        {
            public primaryItemClassWarn() : base(PrimaryItemClassWarning) { ;}
        }
	}

    
}
