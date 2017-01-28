using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.IN;

namespace PX.Objects.SO
{
	[System.SerializableAttribute()]
	public partial class SOPackageInfo : PX.Data.IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsFixed = true, IsKey=true, InputMask = ">aa")]
		[PXDBDefault(typeof(SOOrder.orderType))]
		[PXSelector(typeof(Search<SOOrderType.orderType, Where<SOOrderType.active, Equal<boolTrue>>>))]
		public virtual String OrderType
		{
			get
			{
				return this._OrderType;
			}
			set
			{
				this._OrderType = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOPackageInfo.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOPackageInfo.orderNbr>>>>>))]
		[PXDBString(15, IsUnicode = true, IsKey=true, InputMask = "")]
		[PXDBDefault(typeof(SOOrder.orderNbr))]
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(SOOrder.packageLineCntr))]
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
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true, InputMask = ">a")]
		[PXUIField(DisplayName = "Operation", Visibility = PXUIVisibility.Dynamic, Enabled=false)]
		[PXDefault(typeof(SOOrderType.defaultOperation))]
		[SOOperation.List]
		public virtual String Operation
		{
			get
			{
				return this._Operation;
			}
			set
			{
				this._Operation = value;
			}
		}
		#endregion

		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDefault(typeof(SOOrder.defaultSiteID))]
		[IN.Site(DisplayName = "Ship From Warehouse", DescriptionField = typeof(INSite.descr))]
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

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory()]
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
		#region BoxID
		public abstract class boxID : PX.Data.IBqlField
		{
		}
		protected String _BoxID;
		[PXSelector(typeof(Search5<CSBox.boxID,
			LeftJoin<CarrierPackage, On<CSBox.boxID, Equal<CarrierPackage.boxID>>>,
			Where<Current<SOOrder.shipVia>, IsNull,
			Or<Where<CarrierPackage.carrierID, Equal<Current<SOOrder.shipVia>>, And<Current<SOOrder.shipVia>, IsNotNull>>>>,
			Aggregate<GroupBy<CSBox.boxID>>>))]
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Box ID", Visibility = PXUIVisibility.SelectorVisible)]
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
		public const int BoxWeightPrecision = 4;
		/// <summary>
		/// Net weight
		/// </summary>
		[PXDBDecimal(BoxWeightPrecision)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Net Weight", Enabled =false)]		
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
		#region GrossWeight
		public abstract class grossWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _GrossWeight;
		/// <summary>
		/// Gross weight
		/// </summary>
		[PXDBDecimal(4, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Gross Weight")]
		[PXFormula(null, typeof(SumCalc<SOOrder.packageWeight>))]
		public virtual Decimal? GrossWeight
		{
			get
			{
				return this._GrossWeight;
			}
			set
			{
				this._GrossWeight = value;
			}
		}
		#endregion
		#region WeightUOM
		public abstract class weightUOM : PX.Data.IBqlField
		{
		}
		protected String _WeightUOM;
		/// <summary>
		/// Constant from INSetup.
		/// </summary>
		[PXUIField(DisplayName="Weight UOM", Enabled=false)]
		[PXString()]
		[PXDefault(typeof(IN.INSetup.weightUOM))]
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
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty")]
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
		[PXDefault(typeof(Search<InventoryItem.baseUnit, Where<InventoryItem.inventoryID, Equal<Current<SOPackageInfo.inventoryID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
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
		public abstract class cOD : IBqlField
		{
		}
		protected bool? _COD = false;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "C.O.D.", Visibility = PXUIVisibility.Service)]
		public virtual bool? COD
		{
			get
			{
				return _COD;
			}
			set
			{
				_COD = value;
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

	//Virtual table - do not exists in DB
	[Serializable]
	public partial class SOCarrierRate : IBqlTable
	{
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXInt(IsKey = true)]
		public virtual int? LineNbr
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

		#region DeliveryDate
		public abstract class deliveryDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DeliveryDate;
		[PXDate()]
		[PXUIField(DisplayName = "Delivery Date", Enabled = false)]
		public virtual DateTime? DeliveryDate
		{
			get
			{
				return this._DeliveryDate;
			}
			set
			{
				this._DeliveryDate = value;
			}
		}
		#endregion
		#region DaysInTransit
		public abstract class daysInTransit : PX.Data.IBqlField
		{
		}
		protected int? _DaysInTransit;
		[PXInt()]
		[PXUIField(DisplayName = "Days In Transit", Enabled = false)]
		public virtual int? DaysInTransit
		{
			get
			{
				return this._DaysInTransit;
			}
			set
			{
				this._DaysInTransit = value;
			}
		}
		#endregion
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXPriceCost]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? Amount
		{
			get
			{
				return this._Amount;
			}
			set
			{
				this._Amount = value;
			}
		}
		#endregion
		#region Method
		public abstract class method : PX.Data.IBqlField
		{
		}
		protected String _Method;
		[PXString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Code", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion

	}

	[PXProjection(typeof(Select2<SOPackageInfo,
			InnerJoin<CSBox, On<CSBox.boxID, Equal<SOPackageInfo.boxID>>>>), new Type[]{typeof(SOPackageInfo)})]
	[Serializable]
	public partial class SOPackageInfoEx : SOPackageInfo
	{
		public new abstract class orderType : PX.Data.IBqlField { }
		public new abstract class orderNbr : PX.Data.IBqlField { }
		public new abstract class lineNbr : PX.Data.IBqlField { }
		public new abstract class operation : PX.Data.IBqlField { }
		public new abstract class siteID : PX.Data.IBqlField { }
		public new abstract class inventoryID : PX.Data.IBqlField { }
		public new abstract class boxID : PX.Data.IBqlField { }

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDefault(typeof(Search<CSBox.description, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
		[PXDBString(255, IsUnicode = true, BqlField = typeof(CSBox.description))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region CarrierBox
		public abstract class carrierBox : PX.Data.IBqlField
		{
		}
		protected String _CarrierBox;
		[PXDefault(typeof(Search<CSBox.carrierBox, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(60, IsUnicode = true, BqlField = typeof(CSBox.carrierBox))]
		[PXUIField(DisplayName = "Carrier's Package", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String CarrierBox
		{
			get
			{
				return this._CarrierBox;
			}
			set
			{
				this._CarrierBox = value;
			}
		}
		#endregion
		#region Length
		public abstract class length : PX.Data.IBqlField
		{
		}
		protected int? _Length;
		[PXDefault(typeof(Search<CSBox.length, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.length))]
		[PXUIField(DisplayName = "Length", Enabled = false)]
		public virtual int? Length
		{
			get
			{
				return this._Length;
			}
			set
			{
				this._Length = value;
			}
		}
		#endregion
		#region Width
		public abstract class width : PX.Data.IBqlField
		{
		}
		protected int? _Width;
		[PXDefault(typeof(Search<CSBox.width, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.width))]
		[PXUIField(DisplayName = "Width", Enabled = false)]
		public virtual int? Width
		{
			get
			{
				return this._Width;
			}
			set
			{
				this._Width = value;
			}
		}
		#endregion	
		#region Height
		public abstract class height : PX.Data.IBqlField
		{
		}
		protected int? _Height;
		[PXDefault(typeof(Search<CSBox.height, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.height))]
		[PXUIField(DisplayName = "Height", Enabled = false)]
		public virtual int? Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value;
			}
		}
		#endregion
		#region BoxWeight
		public abstract class boxWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _BoxWeight;
		[PXDefault(typeof(Search<CSBox.boxWeight, Where<CSBox.boxID, Equal<Current<boxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBDecimal(4, MinValue = 0, BqlField = typeof(CSBox.boxWeight))]
		[PXUIField(DisplayName = "Box Weight", Enabled=false)]
		public virtual Decimal? BoxWeight
		{
			get
			{
				return this._BoxWeight;
			}
			set
			{
				this._BoxWeight = value;
			}
		}
		#endregion

		#region GrossWeight
		public new abstract class grossWeight : PX.Data.IBqlField
		{
		}
		
		[PXDBDecimal(4, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Gross Weight")]
		[PXFormula(typeof(Add<weight, boxWeight>), typeof(SumCalc<SOOrder.packageWeight>))]
		public override Decimal? GrossWeight
		{
			get
			{
				return this._GrossWeight;
			}
			set
			{
				this._GrossWeight = value;
			}
		}
		#endregion


	}
}
