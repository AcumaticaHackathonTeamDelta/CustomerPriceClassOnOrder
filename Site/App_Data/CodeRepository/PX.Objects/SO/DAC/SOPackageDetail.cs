using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO
{
	[System.SerializableAttribute()]
	[PXCacheName(Messages.SOPackageDetail)]
	public partial class SOPackageDetail : PX.Data.IBqlTable
	{
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
		}
		protected String _ShipmentNbr;
		[PXParent(typeof(Select<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOPackageDetail.shipmentNbr>>>>))]
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(SOShipment.shipmentNbr))]
		[PXUIField(DisplayName = "Shipment Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ShipmentNbr
		{
			get
			{
				return this._ShipmentNbr;
			}
			set
			{
				this._ShipmentNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(SOShipment.packageLineCntr))]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region BoxID
		public abstract class boxID : PX.Data.IBqlField
		{
		}
		protected String _BoxID;
		[PXDBString(15, IsUnicode = true)]
		[PXDefault()]
		[PXSelector(typeof(Search2<CSBox.boxID, 
		    LeftJoin<CarrierPackage, On<CSBox.boxID, Equal<CarrierPackage.boxID>>>,
 			Where<Current<SOShipment.shipVia>, IsNull, 
			Or<Where<CarrierPackage.carrierID, Equal<Current<SOShipment.shipVia>>, And<Current<SOShipment.shipVia>, IsNotNull>>>>>))]
		[PXUIField(DisplayName = "Box ID")]
		public virtual String BoxID
		{
			get
			{
				return this._BoxID;
			}
			set
			{
				this._BoxID = value;
			}
		}
		#endregion
		#region Weight
		public abstract class weight : PX.Data.IBqlField
		{
		}
		protected Decimal? _Weight;
		/// <summary>
		/// Gross (Brutto) Weight. Weight of a box with contents. (includes weight of the box itself).
		/// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Weight")]
		[PXFormula(null, typeof(SumCalc<SOShipment.packageWeight>))]
		public virtual Decimal? Weight
		{
			get
			{
				return this._Weight;
			}
			set
			{
				this._Weight = value;
			}
		}
		#endregion
		#region WeightUOM
		public abstract class weightUOM : PX.Data.IBqlField
		{
		}
		protected String _WeightUOM;
		[PXUIField(DisplayName = "UOM", Enabled = false)]
		[PXString()]
		public virtual String WeightUOM
		{
			get
			{
				return this._WeightUOM;
			}
			set
			{
				this._WeightUOM = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Visible=false)]
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Description
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
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Qty", Enabled = false)]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region QtyUOM
		public abstract class qtyUOM : PX.Data.IBqlField
		{
		}
		protected String _QtyUOM;
		[PXUIField(DisplayName = "Qty. UOM", Enabled = false)]
		[PXDBString()]
		[PXDefault(typeof(Search<InventoryItem.baseUnit, Where<InventoryItem.inventoryID, Equal<Current<SOPackageDetail.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String QtyUOM
		{
			get
			{
				return this._QtyUOM;
			}
			set
			{
				this._QtyUOM = value;
			}
		}
		#endregion
		#region TrackNumber
		public abstract class trackNumber : PX.Data.IBqlField
		{
		}
		protected String _TrackNumber;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName="Tracking Number")]
		public virtual String TrackNumber
		{
			get
			{
				return this._TrackNumber;
			}
			set
			{
				this._TrackNumber = value;
			}
		}
		#endregion
		#region TrackData
		public abstract class trackData : PX.Data.IBqlField
		{
		}
		protected String _TrackData;
		[PXDBString(4000)]
		public virtual String TrackData
		{
			get
			{
				return this._TrackData;
			}
			set
			{
				this._TrackData = value;
			}
		}
		#endregion
		#region DeclaredValue
		public abstract class declaredValue : PX.Data.IBqlField
		{
		}
		protected Decimal? _DeclaredValue;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Declared Value")]
		public virtual Decimal? DeclaredValue
		{
			get
			{
				return this._DeclaredValue;
			}
			set
			{
				this._DeclaredValue = value;
			}
		}
		#endregion
		#region COD
		public abstract class cOD : PX.Data.IBqlField
		{
		}
		protected Decimal? _COD;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "C.O.D. Amount")]
		public virtual Decimal? COD
		{
			get
			{
				return this._COD;
			}
			set
			{
				this._COD = value;
			}
		}
		#endregion
		#region Confirmed
		public abstract class confirmed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Confirmed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Confirmed", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? Confirmed
		{
			get
			{
				return this._Confirmed;
			}
			set
			{
				this._Confirmed = value;
			}
		}
		#endregion
		#region CustomRefNbr1
		public abstract class customRefNbr1 : PX.Data.IBqlField
		{
		}
		protected String _CustomRefNbr1;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Custom Ref. Nbr. 1")]
		public virtual String CustomRefNbr1
		{
			get
			{
				return this._CustomRefNbr1;
			}
			set
			{
				this._CustomRefNbr1 = value;
			}
		}
		#endregion
		#region CustomRefNbr2
		public abstract class customRefNbr2 : PX.Data.IBqlField
		{
		}
		protected String _CustomRefNbr2;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Custom Ref. Nbr. 2")]
		public virtual String CustomRefNbr2
		{
			get
			{
				return this._CustomRefNbr2;
			}
			set
			{
				this._CustomRefNbr2 = value;
			}
		}
		#endregion
		#region PackageType
		public abstract class packageType : PX.Data.IBqlField
		{
		}
		protected String _PackageType;
		[PXDefault(SOPackageType.Manual)]
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Type", Enabled=false )]
		[SOPackageType.List]
		public virtual String PackageType
		{
			get
			{
				return this._PackageType;
			}
			set
			{
				this._PackageType = value;
			}
		}
		#endregion
		
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
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

	public class SOPackageType
	{
		public const string Auto = "A";
		public const string Manual = "M";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { Auto, Manual },
					new string[] { Messages.PackagingType_Auto, Messages.PackagingType_Manual })
			{
			}
		}
	}
}
