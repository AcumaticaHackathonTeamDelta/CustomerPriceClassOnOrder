namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CR;

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class INCostSite : PX.Data.IBqlTable
	{
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSiteID;
		[PXDBIdentity()]
		public virtual Int32? CostSiteID
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
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(
		new Type[] { typeof(INSiteMaint)},
		new Type[] { typeof(Select<INSite, 
			Where<INSite.siteID, Equal<Current<INSite.siteID>>>>)
		})]
	[PXCacheName(Messages.INSite)]
	public partial class INSite : PX.Data.IBqlTable, PX.SM.IIncludable
	{
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBForeignIdentity(typeof(INCostSite))]
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
		#region SiteCD
		public abstract class siteCD : PX.Data.IBqlField
		{
		}
		protected String _SiteCD;
		[IN.SiteRaw(IsKey=true)]
		[PXDefault()]
		[PX.Data.EP.PXFieldDescription]
		public virtual String SiteCD
		{
			get
			{
				return this._SiteCD;
			}
			set
			{
				this._SiteCD = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
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
		#region ReasonCodeSubID
		public abstract class reasonCodeSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReasonCodeSubID;
		[SubAccount(DisplayName = "Reason Code Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? ReasonCodeSubID
		{
			get
			{
				return this._ReasonCodeSubID;
			}
			set
			{
				this._ReasonCodeSubID = value;
			}
		}
		#endregion
		#region SalesAcctID
		public abstract class salesAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesAcctID;
		[Account(DisplayName = "Sales Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? SalesAcctID
		{
			get
			{
				return this._SalesAcctID;
			}
			set
			{
				this._SalesAcctID = value;
			}
		}
		#endregion
		#region SalesSubID
		public abstract class salesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesSubID;
		[SubAccount(typeof(INSite.salesAcctID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? SalesSubID
		{
			get
			{
				return this._SalesSubID;
			}
			set
			{
				this._SalesSubID = value;
			}
		}
		#endregion
		#region InvtAcctID
		public abstract class invtAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtAcctID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Inventory Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? InvtAcctID
		{
			get
			{
				return this._InvtAcctID;
			}
			set
			{
				this._InvtAcctID = value;
			}
		}
		#endregion
		#region InvtSubID
		public abstract class invtSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtSubID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(INSite.invtAcctID), DisplayName = "Inventory Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? InvtSubID
		{
			get
			{
				return this._InvtSubID;
			}
			set
			{
				this._InvtSubID = value;
			}
		}
		#endregion
		#region COGSAcctID
		public abstract class cOGSAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSAcctID;
		[Account(DisplayName = "COGS/Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? COGSAcctID
		{
			get
			{
				return this._COGSAcctID;
			}
			set
			{
				this._COGSAcctID = value;
			}
		}
		#endregion
		#region COGSSubID
		public abstract class cOGSSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSSubID;
		[SubAccount(typeof(INSite.cOGSAcctID), DisplayName = "COGS/Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? COGSSubID
		{
			get
			{
				return this._COGSSubID;
			}
			set
			{
				this._COGSSubID = value;
			}
		}
		#endregion
		#region DiscAcctID
		public abstract class discAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscAcctID;
		[Account(DisplayName = "Discount Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? DiscAcctID
		{
			get
			{
				return this._DiscAcctID;
			}
			set
			{
				this._DiscAcctID = value;
			}
		}
		#endregion
		#region DiscSubID
		public abstract class discSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _DiscSubID;
		[SubAccount(typeof(INSite.discAcctID), DisplayName = "Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? DiscSubID
		{
			get
			{
				return this._DiscSubID;
			}
			set
			{
				this._DiscSubID = value;
			}
		}
		#endregion
		#region StdCstRevAcctID
		public abstract class stdCstRevAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstRevAcctID;
		[Account(DisplayName = "Standard Cost Revaluation Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? StdCstRevAcctID
		{
			get
			{
				return this._StdCstRevAcctID;
			}
			set
			{
				this._StdCstRevAcctID = value;
			}
		}
		#endregion
		#region StdCstRevSubID
		public abstract class stdCstRevSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstRevSubID;
		[SubAccount(typeof(INSite.stdCstRevAcctID), DisplayName = "Standard Cost Revaluation Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? StdCstRevSubID
		{
			get
			{
				return this._StdCstRevSubID;
			}
			set
			{
				this._StdCstRevSubID = value;
			}
		}
		#endregion
		#region StdCstVarAcctID
		public abstract class stdCstVarAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstVarAcctID;
		[Account(DisplayName = "Standard Cost Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? StdCstVarAcctID
		{
			get
			{
				return this._StdCstVarAcctID;
			}
			set
			{
				this._StdCstVarAcctID = value;
			}
		}
		#endregion
		#region StdCstVarSubID
		public abstract class stdCstVarSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _StdCstVarSubID;
		[SubAccount(typeof(INSite.stdCstVarAcctID), DisplayName = "Standard Cost Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? StdCstVarSubID
		{
			get
			{
				return this._StdCstVarSubID;
			}
			set
			{
				this._StdCstVarSubID = value;
			}
		}
		#endregion
		#region PPVAcctID
		public abstract class pPVAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PPVAcctID;
		[Account(DisplayName = "Purchase Price Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? PPVAcctID
		{
			get
			{
				return this._PPVAcctID;
			}
			set
			{
				this._PPVAcctID = value;
			}
		}
		#endregion
		#region PPVSubID
		public abstract class pPVSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PPVSubID;
		[SubAccount(typeof(INSite.pPVAcctID), DisplayName = "Purchase Price Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? PPVSubID
		{
			get
			{
				return this._PPVSubID;
			}
			set
			{
				this._PPVSubID = value;
			}
		}
		#endregion
		#region FreightAcctID
		public abstract class freightAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _FreightAcctID;
		[Account(DisplayName = "Freight Charge Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? FreightAcctID
		{
			get
			{
				return this._FreightAcctID;
			}
			set
			{
				this._FreightAcctID = value;
			}
		}
		#endregion
		#region FreightSubID
		public abstract class freightSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _FreightSubID;
		[SubAccount(typeof(INSite.freightAcctID), DisplayName = "Freight Charge Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? FreightSubID
		{
			get
			{
				return this._FreightSubID;
			}
			set
			{
				this._FreightSubID = value;
			}
		}
		#endregion
		#region MiscAcctID
		public abstract class miscAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _MiscAcctID;
		[Account(DisplayName = "Misc. Charge Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? MiscAcctID
		{
			get
			{
				return this._MiscAcctID;
			}
			set
			{
				this._MiscAcctID = value;
			}
		}
		#endregion
		#region MiscSubID
		public abstract class miscSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _MiscSubID;
		[SubAccount(typeof(INSite.miscAcctID), DisplayName = "Misc. Charge Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? MiscSubID
		{
			get
			{
				return this._MiscSubID;
			}
			set
			{
				this._MiscSubID = value;
			}
		}
		#endregion
		#region POAccrualAcctID
		public abstract class pOAccrualAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualAcctID;
		[Account(DisplayName = "PO Accrual Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? POAccrualAcctID
		{
			get
			{
				return this._POAccrualAcctID;
			}
			set
			{
				this._POAccrualAcctID = value;
			}
		}
		#endregion
		#region POAccrualSubID
		public abstract class pOAccrualSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualSubID;
		[SubAccount(typeof(INSite.pOAccrualAcctID), DisplayName = "PO Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? POAccrualSubID
		{
			get
			{
				return this._POAccrualSubID;
			}
			set
			{
				this._POAccrualSubID = value;
			}
		}
		#endregion
		#region LCVarianceAcctID
		public abstract class lCVarianceAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceAcctID;
		[Account(DisplayName = "Landed Cost Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? LCVarianceAcctID
		{
			get
			{
				return this._LCVarianceAcctID;
			}
			set
			{
				this._LCVarianceAcctID = value;
			}
		}
		#endregion
		#region LCVarianceSubID
		public abstract class lCVarianceSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceSubID;
		[SubAccount(typeof(INSite.lCVarianceAcctID), DisplayName = "Landed Cost Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? LCVarianceSubID
		{
			get
			{
				return this._LCVarianceSubID;
			}
			set
			{
				this._LCVarianceSubID = value;
			}
		}
		#endregion
		#region ReceiptLocationID
		public abstract class receiptLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReceiptLocationID;
		[PXRestrictor(typeof(Where<INLocation.active, Equal<True>>), Messages.LocationIsNotActive)]
		[IN.Location(typeof(INSite.siteID), DisplayName = "Receiving Location", DescriptionField = typeof(INLocation.descr), DirtyRead = true)]
		public virtual Int32? ReceiptLocationID
		{
			get
			{
				return this._ReceiptLocationID;
			}
			set
			{
				this._ReceiptLocationID = value;
			}
		}
		#endregion
		#region ReceiptLocationIDOverride
		public abstract class receiptLocationIDOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReceiptLocationIDOverride;
		[PXBool()]		
		public virtual Boolean? ReceiptLocationIDOverride
		{
			get
			{
				return this._ReceiptLocationIDOverride;
			}
			set
			{
				this._ReceiptLocationIDOverride = value;
			}
		}
		#endregion
		#region ShipLocationID
		public abstract class shipLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipLocationID;
		[PXRestrictor(typeof(Where<INLocation.active, Equal<True>>), Messages.LocationIsNotActive)]
		[IN.Location(typeof(INSite.siteID), DisplayName = "Shipping Location", DescriptionField = typeof(INLocation.descr), DirtyRead = true)]
		public virtual Int32? ShipLocationID
		{
			get
			{
				return this._ShipLocationID;
			}
			set
			{
				this._ShipLocationID = value;
			}
		}
		#endregion
		#region ShipLocationIDOverride
		public abstract class shipLocationIDOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShipLocationIDOverride;
		[PXBool()]		
		public virtual Boolean? ShipLocationIDOverride
		{
			get
			{
				return this._ShipLocationIDOverride;
			}
			set
			{
				this._ShipLocationIDOverride = value;
			}
		}
		#endregion
		#region DropShipLocationID
		public abstract class dropShipLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _DropShipLocationID;
		[PXRestrictor(typeof(Where<INLocation.active, Equal<True>>), Messages.LocationIsNotActive)]
		[IN.Location(typeof(INSite.siteID), DisplayName = "Drop-Ship Location", DescriptionField = typeof(INLocation.descr), DirtyRead = true)]
		public virtual Int32? DropShipLocationID
		{
			get
			{
				return this._DropShipLocationID;
			}
			set
			{
				this._DropShipLocationID = value;
			}
		}
		#endregion
		#region ReturnLocationID
		public abstract class returnLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReturnLocationID;
		[PXRestrictor(typeof(Where<INLocation.active, Equal<True>>), Messages.LocationIsNotActive)]
		[IN.Location(typeof(INSite.siteID), DisplayName = "RMA Location", DescriptionField = typeof(INLocation.descr), DirtyRead = true)]
		public virtual Int32? ReturnLocationID
		{
			get
			{
				return this._ReturnLocationID;
			}
			set
			{
				this._ReturnLocationID = value;
			}
		}
		#endregion
		#region LocationValid
		public abstract class locationValid : PX.Data.IBqlField
		{
		}
		protected String _LocationValid;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Location Entry")]
		[INLocationValid.List()]
		[PXDefault(INLocationValid.Validate)]
		public virtual String LocationValid
		{
			get
			{
				return this._LocationValid;
			}
			set
			{
				this._LocationValid = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch()]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region AddressID
		public abstract class addressID : PX.Data.IBqlField
		{
		}
		protected Int32? _AddressID;
		[PXDBInt]
		[PXDBChildIdentity(typeof(Address.addressID))]
		/*[PXSelector(typeof(Search<Address.addressID,
			Where<Address.bAccountID, Equal<Current<BAccount.bAccountID>>>>),
			DescriptionField = typeof(Address.displayName), DirtyRead = true)]*/
		public virtual Int32? AddressID
		{
			get
			{
				return this._AddressID;
			}
			set
			{
				this._AddressID = value;
			}
		}
		#endregion
		#region ContactID
		public abstract class contactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContactID;
		[PXDBInt()]
		[PXDBChildIdentity(typeof(Contact.contactID))]
		//[PXSelector(typeof(Search<Contact.contactID,Where<Contact.bAccountID,Equal<Current<BAccount.bAccountID>>>>))]
		public virtual Int32? ContactID
		{
			get
			{
				return this._ContactID;
			}
			set
			{
				this._ContactID = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INSite.siteCD), 
			Selector = typeof(INSite.siteCD), 
			FieldList = new [] { typeof(INSite.siteCD), typeof(INSite.descr), typeof(INSite.replenishmentClassID) })]
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
		#region ReplenishmentClassID
		public abstract class replenishmentClassID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Replenishment Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INReplenishmentClass.replenishmentClassID>), DescriptionField = typeof(INReplenishmentClass.descr))]
		public virtual String ReplenishmentClassID
		{
			get
			{
				return this._ReplenishmentClassID;
			}
			set
			{
				this._ReplenishmentClassID = value;
			}
		}
		#endregion
		#region LockSitePICountEntry
		public abstract class lockSitePICountEntry : PX.Data.IBqlField
		{
		}
		protected Boolean? _LockSitePICountEntry;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Freeze Inventory When PI Count Is in Data Entry State.")]
		public virtual Boolean? LockSitePICountEntry
		{
			get
			{
				return this._LockSitePICountEntry;
			}
			set
			{
				this._LockSitePICountEntry = value;
			}
		}
		#endregion
		#region AvgDefaultCost
		public abstract class avgDefaultCost : IBqlField
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
						new [] { AverageCost, LastCost },
						new [] { Messages.AverageCost, Messages.LastCost }) {}
			}

			public const string AverageCost = "A";
			public const string LastCost = "L";

			public class averageCost : Constant<string>
			{
				public averageCost() : base(AverageCost){}
			}
			public class lastCost : Constant<string>
			{
				public lastCost() : base(LastCost) {}
			}
		}
		protected String _AvgDefaultCost;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(avgDefaultCost.AverageCost, PersistingCheck = PXPersistingCheck.Nothing)]
		[avgDefaultCost.List]
		[PXUIField(DisplayName = "Avg. Default Returns Cost")]
		public virtual String AvgDefaultCost
		{
			get
			{
				return _AvgDefaultCost;
			}
			set
			{
				_AvgDefaultCost = value;
			}
		}
		#endregion
		#region FIFODefaultCost
		public abstract class fIFODefaultCost : IBqlField
		{
		}
		protected String _FIFODefaultCost;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(avgDefaultCost.AverageCost, PersistingCheck = PXPersistingCheck.Nothing)]
		[avgDefaultCost.List]
		[PXUIField(DisplayName = "FIFO Default Returns Cost")]
		public virtual String FIFODefaultCost
		{
			get
			{
				return _FIFODefaultCost;
			}
			set
			{
				_FIFODefaultCost = value;
			}
		}
		#endregion
		#region OverrideInvtAccSub
		public abstract class overrideAccSub : IBqlField
		{
		}
		protected bool? _OverrideInvtAccSub;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = Messages.OverrideInventoryAcctSub)]
		public virtual bool? OverrideInvtAccSub
		{
			get
			{
				return _OverrideInvtAccSub;
			}
			set
			{
				_OverrideInvtAccSub = value;
			}
		}
		#endregion
        #region Active
        public abstract class active : IBqlField { }
        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
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
		#region GroupMask
		public abstract class groupMask : IBqlField
		{
		}
		protected Byte[] _GroupMask;
		[PXDBGroupMask()]
		public virtual Byte[] GroupMask
		{
			get
			{
				return this._GroupMask;
			}
			set
			{
				this._GroupMask = value;
			}
		}
		#endregion
		#region Included
		public abstract class included : PX.Data.IBqlField
		{
		}
		protected bool? _Included;
		[PXBool]
		[PXUIField(DisplayName = "Included")]
		[PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
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
		public const string Main = "MAIN";
		public class main : Constant<string>
		{
			public main()
				: base(Main)
			{
			}
		}
	}
	#region Attributes
	public class INLocationValid
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { Validate, Warn, NoValidate },
					new string[] { Messages.LocValidate, Messages.LocWarn, Messages.LocNoValidate }) { ; }
		}

		public const string Validate = "V";
		public const string Warn = "W";
		public const string NoValidate = "N";

		public class validate : Constant<string>
		{
			public validate() : base(Validate) { ;}
		}

		public class noValidate : Constant<string>
		{
			public noValidate() : base(NoValidate) { ;}
		}

		public class warn : Constant<string>
		{
			public warn() : base(Warn) { ;}
		}
	}
	#endregion
}
