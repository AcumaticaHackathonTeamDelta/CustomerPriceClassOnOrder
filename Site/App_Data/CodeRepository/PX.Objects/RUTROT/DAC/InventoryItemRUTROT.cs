using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
namespace PX.Objects.RUTROT
{
	[System.SerializableAttribute()]

	public partial class InventoryItemRUTROT : PXCacheExtension<InventoryItem>
	{
		#region IsRUTROTDeductible
		public abstract class isRUTROTDeductible : IBqlField { }

		/// <summary>
		/// Indicates whether the item is subjected to RUT or ROT deductions.
		/// This field is relevant only if the <see cref="FeaturesSet.RutRotDeduction">RUT and ROT Deduction</see> feature is enabled.
		/// </summary>
		/// <value>
		/// The value of this field is used to set default value of the <see cref="ARTranRUTROT.IsRUTROTDeductible">ARTran.IsRUTROTDeductible</see> field
		/// for the lines of a <see cref="ARInvoiceRUTROT.IsRUTROTDeductible">RUT or ROT deductible</see> Accounts Receivable Invoice.
		/// </value>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ROT or RUT Deductible Item", FieldClass = RUTROTMessages.FieldClass)]
		public virtual bool? IsRUTROTDeductible { get; set; }
		#endregion
		#region RUTROTType
		public abstract class rUTROTType : IBqlField { }
		protected String _RUTROTType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(RUTROTTypes.RUT, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Type", FieldClass = RUTROTMessages.FieldClass, Enabled = true)]
		[RUTROTTypes.List]
		public virtual String RUTROTType
		{
			get
			{
				return this._RUTROTType;
			}
			set
			{
				this._RUTROTType = value;
			}
		}
		#endregion
		#region RUTROTItemType
		public abstract class rUTROTItemType : IBqlField { }
		protected String _RUTROTItemType;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Type", FieldClass = RUTROTMessages.FieldClass, Enabled = true)]
		[RUTROTItemTypes.List]
		public virtual String RUTROTItemType
		{
			get
			{
				return this._RUTROTItemType;
			}
			set
			{
				this._RUTROTItemType = value;
			}
		}
		#endregion
		#region RUTROTWorkTypeID
		public abstract class rUTROTWorkTypeID : IBqlField { }
		protected Int32? _RUTROTWorkTypeID;
		[PXDBInt]
		[PXUIField(DisplayName = "Type of Work", FieldClass = RUTROTMessages.FieldClass, Enabled = false)]
		[PXSelector(typeof(Search<RUTROTWorkType.workTypeID, Where<RUTROTWorkType.rUTROTType, Equal<Current<rUTROTType>>>>),
			SubstituteKey =typeof(RUTROTWorkType.description), DescriptionField =typeof(RUTROTWorkType.xmlTag))]
		public virtual Int32? RUTROTWorkTypeID
		{
			get
			{
				return this._RUTROTWorkTypeID;
			}
			set
			{
				this._RUTROTWorkTypeID = value;
			}
		}
		#endregion

	}
}
