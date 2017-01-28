using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.IN
{
	#region IN PI Class
	[Serializable]
	public partial class INPIClass : PX.Data.IBqlTable
	{
		#region PIClassID
		public abstract class pIClassID : PX.Data.IBqlField
		{
		}
		protected String _PIClassID;
		[PXDBString(30, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Type ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INPIClass.pIClassID>))]
		[PXDefault()]
		public virtual String PIClassID
		{
			get
			{
				return this._PIClassID;
			}
			set
			{
				this._PIClassID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDefault]
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
		#region Method
		public abstract class method : PX.Data.IBqlField
		{
		}
		protected String _Method;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Generation Method", Visibility = PXUIVisibility.SelectorVisible)]
		[PIMethod.List]
		[PXDefault(PIMethod.FullPhysicalInventory)]
		public virtual String Method
		{
			get
			{
				return this._Method;
			}
			set
			{
				this._Method = value;
			}
		}
		#endregion

		#region CycleID
		public abstract class cycleID : PX.Data.IBqlField
		{
		}
		protected String _CycleID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(INPICycle.cycleID), DescriptionField = typeof(INPICycle.descr))]
		[PXUIField(DisplayName = "Cycle ID")]
		public virtual String CycleID
		{
			get
			{
				return this._CycleID;
			}
			set
			{
				this._CycleID = value;
			}
		}
		#endregion
		#region ABCCodeID
		public abstract class aBCCodeID : PX.Data.IBqlField
		{
		}
		protected String _ABCCodeID;
		[PXDBString(1, IsFixed = true)]
		[PXSelector(typeof(INABCCode.aBCCodeID), DescriptionField = typeof(INABCCode.descr))]
		[PXUIField(DisplayName = "ABC Code")]
		public virtual String ABCCodeID
		{
			get
			{
				return this._ABCCodeID;
			}
			set
			{
				this._ABCCodeID = value;
			}
		}
		#endregion
		#region MovementClassID
		public abstract class movementClassID : PX.Data.IBqlField
		{
		}
		protected String _MovementClassID;
		[PXDBString(1, IsFixed = true)]
		[PXSelector(typeof(INMovementClass.movementClassID), DescriptionField = typeof(INMovementClass.descr))]
		[PXUIField(DisplayName = "Movement Class ID")]
		public virtual String MovementClassID
		{
			get
			{
				return this._MovementClassID;
			}
			set
			{
				this._MovementClassID = value;
			}
		}
		#endregion
		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Item Class ID")]
		[PXSelector(typeof(Search<INItemClass.itemClassID, Where<INItemClass.stkItem, Equal<boolTrue>>>), DescriptionField = typeof(INItemClass.descr))]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion

		#region SelectedMethod
		public abstract class selectedMethod : PX.Data.IBqlField
		{
		}
		protected String _SelectedMethod;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Selection Method", Visibility = PXUIVisibility.Visible)]
		[PIInventoryMethod.List]
		[PXDefault(PIInventoryMethod.ItemsHavingNegativeBookQty)]
		public virtual String SelectedMethod
		{
			get
			{
				return this._SelectedMethod;
			}
			set
			{
				this._SelectedMethod = value;
			}
		}
		#endregion

		#region ByFrequency
		public abstract class byFrequency : PX.Data.IBqlField
		{
		}
		protected Boolean? _ByFrequency;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? ByFrequency
		{
			get
			{
				return this._ByFrequency;
			}
			set
			{
				this._ByFrequency = value;
			}
		}
		#endregion
		#region ByABCFrequency
		public abstract class byABCFrequency : PX.Data.IBqlField
		{
		}
		protected Boolean? _ByABCFrequency;
		[PXBool()]
		[PXUIField(DisplayName = "By Frequency")]
		public virtual Boolean? ByABCFrequency
		{
			get
			{
				return this._ByFrequency;
			}
			set
			{
				if (this.Method == PIMethod.ByABCClass)
					this._ByFrequency = value;
			}
		}
		#endregion
		#region ByMovementClassFrequency
		public abstract class byMovementClassFrequency : PX.Data.IBqlField
		{
		}
		protected Boolean? _ByMovementClassFrequency;
		[PXBool()]
		[PXUIField(DisplayName = "By Frequency")]
		public virtual Boolean? ByMovementClassFrequency
		{
			get
			{
				return this._ByFrequency;
			}
			set
			{
				if (this.Method == PIMethod.ByMovementClass)
					this._ByFrequency = value;
			}
		}
		#endregion
		#region ByCycleFrequency
		public abstract class byCycleFrequency : PX.Data.IBqlField
		{
		}
		protected Boolean? _ByCycleFrequency;
		[PXBool()]
		[PXUIField(DisplayName = "By Frequency")]
		public virtual Boolean? ByCycleFrequency
		{
			get
			{
				return this._ByFrequency;
			}
			set
			{
				if (this.Method == PIMethod.ByCycle)
					this._ByFrequency = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site]
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
		#region NAO1
		public abstract class nAO1 : PX.Data.IBqlField
		{
		}
		protected String _NAO1;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "1", Visibility = PXUIVisibility.Visible)]
		[PINumberAssignmentOrder.List]
		[PXDefault(PINumberAssignmentOrder.ByLocationID)]
		public virtual String NAO1
		{
			get
			{
				return this._NAO1;
			}
			set
			{
				this._NAO1 = value;
			}
		}
		#endregion
		#region NAO2
		public abstract class nAO2 : PX.Data.IBqlField
		{
		}
		protected String _NAO2;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "2", Visibility = PXUIVisibility.Visible)]
		[PINumberAssignmentOrder.List]
		[PXDefault(PINumberAssignmentOrder.ByInventoryID)]
		public virtual String NAO2
		{
			get
			{
				return this._NAO2;
			}
			set
			{
				this._NAO2 = value;
			}
		}
		#endregion
		#region NAO3
		public abstract class nAO3 : PX.Data.IBqlField
		{
		}
		protected String _NAO3;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "3", Visibility = PXUIVisibility.Visible)]
		[PINumberAssignmentOrder.List]
		[PXDefault(PINumberAssignmentOrder.BySubItem)]
		public virtual String NAO3
		{
			get
			{
				return this._NAO3;
			}
			set
			{
				this._NAO3 = value;
			}
		}
		#endregion
		#region NAO4
		public abstract class nAO4 : PX.Data.IBqlField
		{
		}
		protected String _NAO4;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "4", Visibility = PXUIVisibility.Visible)]
		[PINumberAssignmentOrder.List]
		[PXDefault(PINumberAssignmentOrder.ByLotSerial)]
		public virtual String NAO4
		{
			get
			{
				return this._NAO4;
			}
			set
			{
				this._NAO4 = value;
			}
		}
		#endregion

		#region BlankLines
		public abstract class blankLines : IBqlField
		{
		}
		protected Int16? _BlankLines;
		[PXDBShort(MinValue = 0, MaxValue = 10000)]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Blank Lines To Append")]
		public virtual Int16? BlankLines
		{
			get
			{
				return this._BlankLines;
			}
			set
			{
				this._BlankLines = value;
			}
		}
		#endregion

		#region RandomItemsLimit
		public abstract class randomItemsLimit : IBqlField
		{
		}
		protected Int16? _RandomItemsLimit;
		[PXShort(MinValue = 0, MaxValue = 10000)]
		[PXUIField(DisplayName = "Number of Items (up to)")]
		public virtual Int16? RandomItemsLimit
		{
			get
			{
				return this._RandomItemsLimit;
			}
			set
			{
				this._RandomItemsLimit = value;
			}
		}
		#endregion
		#region LastCountPeriod
		public abstract class lastCountPeriod : IBqlField
		{
		}
		protected Int16? _LastCountPeriod;
		[PXDBShort(MinValue = 0, MaxValue = 10000)]
		[PXUIField(DisplayName = "Last Count Before (days)", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int16? LastCountPeriod
		{
			get
			{
				return this._LastCountPeriod;
			}
			set
			{
				this._LastCountPeriod = value;
			}
		}
		#endregion

		#region System Columns
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
		#endregion
	}
	#endregion

    [Serializable]
	public partial class INPIClassItem : IBqlTable
	{
		#region PIClassID
		public abstract class pIClassID : PX.Data.IBqlField
		{
		}
		protected String _PIClassID;
		[PXDBString(30, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(INPIClass.pIClassID))]
		[PXUIField(DisplayName = "Type", Visible = false)]
		[PXParent(typeof(Select<INPIClass, Where<INPIClass.pIClassID, Equal<Current<INPIClassItem.pIClassID>>>>))]
		public virtual String PIClassID
		{
			get
			{
				return this._PIClassID;
			}
			set
			{
				this._PIClassID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INPIClassItem.inventoryID>>>>))]
        [StockItem(DisplayName = "Inventory ID", IsKey = true)]
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
	}

    [Serializable]
	public partial class INPIClassLocation : IBqlTable
	{
		#region PIClassID
		public abstract class pIClassID : PX.Data.IBqlField
		{
		}
		protected String _PIClassID;
		[PXDBString(30, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(INPIClass.pIClassID))]
		[PXUIField(DisplayName = "Code", Visible = false)]
		[PXParent(typeof(Select<INPIClass, Where<INPIClass.pIClassID, Equal<Current<INPIClassLocation.pIClassID>>>>))]
		public virtual String PIClassID
		{
			get
			{
				return this._PIClassID;
			}
			set
			{
				this._PIClassID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Location(typeof(INPIClass.siteID), IsKey = true)]
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
	}
}
