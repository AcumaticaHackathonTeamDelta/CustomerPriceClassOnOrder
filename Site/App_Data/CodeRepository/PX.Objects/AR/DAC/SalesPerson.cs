using PX.Data.EP;

namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.GL;
	

	[System.SerializableAttribute()]
	[PXCacheName(Messages.SalesPerson)]
	[PXPrimaryGraph(typeof(SalesPersonMaint))]
	public partial class SalesPerson : PX.Data.IBqlTable
	{
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesPersonID;
		[PXDBIdentity()]
		[PXUIField(DisplayName = "SalesPerson ID", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Int32? SalesPersonID
		{
			get
			{
				return this._SalesPersonID;
			}
			set
			{
				this._SalesPersonID = value;
			}
		}
		#endregion
		#region SalesPersonCD
		public abstract class salesPersonCD : PX.Data.IBqlField
		{
		}
		protected String _SalesPersonCD;
		[PXDefault()]
		[SalesPersonRaw(IsKey = true, Visibility = PXUIVisibility.SelectorVisible,DisplayName="Salesperson ID")]
		[PXFieldDescription]
		public virtual String SalesPersonCD
		{
			get
			{
				return this._SalesPersonCD;
			}
			set
			{
				this._SalesPersonCD = value;
			}
		}
		#endregion
		#region CommnPct
		public abstract class commnPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnPct;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Default Commission %")]
		public virtual Decimal? CommnPct
		{
			get
			{
				return this._CommnPct;
			}
			set
			{
				this._CommnPct = value;
			}
		}
		#endregion
		#region SalesSubID
		public abstract class salesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesSubID;
		[SubAccount(DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Name")]
		[PXFieldDescription]
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
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected bool? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Is Active")]
		public virtual bool? IsActive
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField { }
		[PXNote(DescriptionField = typeof(SalesPerson.salesPersonCD))]
		public virtual Guid? NoteID { get; set; }
		#endregion

#if ALLOW_EDIT_CONTACT
		#region CreateNewContact
		public abstract class createNewContact : PX.Data.IBqlField
		{
		}
		protected bool? _CreateNewContact;
		[PXBool()]
		[PXDefault(false,PersistingCheck= PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Create New Contact")]
		public virtual bool? CreateNewContact
		{
			get
			{
				return this._CreateNewContact;
			}
			set
			{
				this._CreateNewContact = value;
			}
		}
		#endregion
		#region HasContact
		public abstract class hasContact : PX.Data.IBqlField
		{
		}
		protected bool? _HasContact;
		[PXBool()]
		[PXUIField(DisplayName = "Create New Contact")]
		public virtual bool? HasContact
		{
			get
			{
				return this._HasContact;
			}
			set
			{
				this._HasContact = value;
			}
		}
		#endregion

#endif
	}
}
