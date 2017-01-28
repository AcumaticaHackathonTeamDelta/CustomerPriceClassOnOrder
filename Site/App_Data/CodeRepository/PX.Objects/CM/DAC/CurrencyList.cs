using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.CM
{
	[PXPrimaryGraph(
		new Type[] { typeof(CurrencyMaint) },
		new Type[] { typeof(Select<CurrencyList, 
			Where<CurrencyList.curyID, Equal<Current<CurrencyList.curyID>>>>)
		})]
	[PXCacheName(Messages.Currency)]
	[Serializable]
	public partial class CurrencyList : PX.Data.IBqlTable
	{
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">LLLLL")]
		[PXDefault()]
		[PXUIField(DisplayName = "Currency ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CurrencyList.curyID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region CurySymbol
		public abstract class curySymbol : PX.Data.IBqlField
		{
		}
		protected String _CurySymbol;
		[PXDBString(10, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Currency Symbol")]
		public virtual String CurySymbol
		{
			get
			{
				return this._CurySymbol;
			}
			set
			{
				this._CurySymbol = value;
			}
		}
		#endregion
		#region CuryCaption
		public abstract class curyCaption : PX.Data.IBqlField
		{
		}
		protected String _CuryCaption;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Currency Caption")]
		public virtual String CuryCaption
		{
			get
			{
				return this._CuryCaption;
			}
			set
			{
				this._CuryCaption = value;
			}
		}
		#endregion
		#region DecimalPlaces
		public abstract class decimalPlaces : PX.Data.IBqlField
		{
		}
		protected Int16? _DecimalPlaces;
		[PXDBShort(MinValue = 0, MaxValue = 4)]
		[PXDefault((short)2)]
		[PXUIField(DisplayName = "Decimal Precision")]
		public virtual short? DecimalPlaces
		{
			get
			{
				return this._DecimalPlaces;
			}
			set
			{
				this._DecimalPlaces = value;
			}
		}
		#endregion
		//#region NoteID
		//public abstract class noteID : IBqlField { }

		//[PXNote(DescriptionField = typeof(Currency.curyID))]
		//public virtual Guid? NoteID { get; set; }
		//#endregion
		//#region tstamp
		//public abstract class Tstamp : PX.Data.IBqlField
		//{
		//}
		//protected Byte[] _tstamp;
		//[PXDBTimestamp()]
		//public virtual Byte[] tstamp
		//{
		//	get
		//	{
		//		return this._tstamp;
		//	}
		//	set
		//	{
		//		this._tstamp = value;
		//	}
		//}
		//#endregion
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
		#region isActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		private Boolean? _IsActive;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIEnabled(typeof(Where<isFinancial, NotEqual<True>>))]
		[PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsActive
		{
			get { return _IsActive; }
			set { _IsActive = value; }
		}

		#endregion

		#region isFinancial
		public abstract class isFinancial : PX.Data.IBqlField
		{
		}
		private Boolean? _IsFinancial;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIEnabled(typeof(isActive))]
		[PXUIField(DisplayName = "Use for Accounting", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsFinancial
		{
			get { return _IsFinancial; }
			set { _IsFinancial = value; }
		}

		#endregion
	}


}
