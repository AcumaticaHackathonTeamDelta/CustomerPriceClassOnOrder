namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.CR;
	
	[System.SerializableAttribute()]
    [PXCacheName(Messages.XReferences)]
	public partial class INItemXRef : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(IsKey = true, DirtyRead = true)]
		[PXDBDefault(typeof(InventoryItem.inventoryID), DefaultForInsert=false, DefaultForUpdate = false)]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(INItemXRef.inventoryID), IsKey = true)]
		[PXDefault()]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region AlternateType
		public abstract class alternateType : PX.Data.IBqlField
		{
		}
		protected String _AlternateType;
		[PXDBString(4, IsKey = true)]
		[INAlternateType.List()]
		[PXDefault(INAlternateType.Global)]
		[PXUIField(DisplayName="Alternate Type")]
		public virtual String AlternateType
		{
			get
			{
				return this._AlternateType;
			}
			set
			{
				this._AlternateType = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDefault()]
		[PXRestrictor(typeof(Where<BAccountR.type, Equal<BAccountType.vendorType>, And<Optional<INItemXRef.alternateType>, Equal<INAlternateType.vPN>,
			Or<BAccountR.type, Equal<BAccountType.customerType>, And<Optional<INItemXRef.alternateType>, Equal<INAlternateType.cPN>,
			Or<BAccountR.type, Equal<BAccountType.combinedType>, And<Where<Optional<INItemXRef.alternateType>, Equal<INAlternateType.vPN>, Or<Optional<INItemXRef.alternateType>, Equal<INAlternateType.cPN>>>>>>>>>),
			Messages.DoesNotMatchWithAlternateType, typeof(BAccountR.acctCD), typeof(BAccountR.type))]
		[BAccount(DisplayName = "Vendor/Customer", IsKey = true)]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[PXDBString(50, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Alternate ID")]
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
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

    [PXHidden]
	[PXProjection(typeof(Select<INItemXRef, Where<INItemXRef.alternateType, Equal<INAlternateType.global>, 
			Or<INItemXRef.alternateType, Equal<INAlternateType.vPN>,
			Or<INItemXRef.alternateType, Equal<INAlternateType.cPN>>>>>))]
	public class INItemPartNumber : INItemXRef
	{		
		#region InventoryID
		public new abstract class inventoryID : IBqlField { }
		#endregion
		#region SubItemID
		public new abstract class subItemID : IBqlField { }
		#endregion
		#region AlternateType
		public new abstract class alternateType : IBqlField { }
		#endregion
		#region BAccountID
		public new abstract class bAccountID : IBqlField { }
		#endregion
		#region AlternateID
		public new abstract class alternateID : IBqlField { }
		#endregion
		#region Descr
		public new abstract class descr : IBqlField { }
		#endregion
	}


	public class INAlternateType
	{
		public class List : PXStringListAttribute
		{
			public List()
				: base(new string[] { CPN, VPN, Global, Barcode }, new string[] { Messages.CPN, Messages.VPN, Messages.Global, Messages.Barcode})
			{ 
			}
		}

		public const string CPN = "0CPN";
		public const string VPN = "0VPN";
		public const string MFPN = "MFPN";
		public const string Global = "GLBL";
		public const string Substitute = "SBST";
		public const string Obsolete = "OBSL";
		public const string SKU = "SKU";
		public const string UPC = "UPC";
		public const string EAN = "EAN";
		public const string ISBN = "ISBN";
		public const string GTIN = "GTIN";
		public const string Barcode = "BAR";

		public class cPN : Constant<string>
		{
			public cPN() : base(CPN) { ;}
		}

		public class vPN : Constant<string>
		{
			public vPN() : base(VPN) { ;}
		}

		public class global : Constant<string>
		{
			public global() : base(Global) { ; }
		}

		public class obsolete : Constant<string>
		{
			public obsolete() : base(Obsolete) { ; }
		}
		public class barcode : Constant<string>
		{
			public barcode() : base(Barcode) { ; }
		}
		public static string ConvertFromPrimary(INPrimaryAlternateType aPrimaryType)
		{
			return aPrimaryType == INPrimaryAlternateType.VPN ? INAlternateType.VPN : INAlternateType.CPN; 
		}

		public static INPrimaryAlternateType? ConvertToPrimary( string aAlternateType)
		{
			INPrimaryAlternateType? result = null;
			switch (aAlternateType)
			{
				case INAlternateType.VPN: 
					result =  INPrimaryAlternateType.VPN; break;
				case INAlternateType.CPN:
					result =  INPrimaryAlternateType.CPN; break;				
			}
			return result;
		}

	}
}
