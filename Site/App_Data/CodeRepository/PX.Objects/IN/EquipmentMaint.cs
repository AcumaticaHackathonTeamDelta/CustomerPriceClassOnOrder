using System;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.IN
{
	[PXCacheName(Messages.Equipment)]
	[Serializable]
	public partial class Equipment : INServiceItem
	{
		#region ServiceItemID

		public new abstract class serviceItemID : IBqlField { }

		#endregion

		#region InventoryID

		public new abstract class inventoryID : IBqlField { }

		#endregion
		
		#region Model

		public abstract class model : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Model", Enabled = false)]
		[AttributeValue(typeof(INSetup.modelAttribute), typeof(Equipment.noteID))]
		public virtual String Model { get; set; }

		#endregion

		#region Manufacture

		public abstract class manufacture : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Manufacture", Enabled = false)]
		[AttributeValue(typeof(INSetup.manufactureAttribute), typeof(Equipment.inventoryID))]
		public virtual String Manufacture { get; set; }

		#endregion

	}

	public class EquipmentMaint : PXGraph<EquipmentMaint, Equipment>
	{
		[PXHidden]
		public PXSetup<INSetup>
			Setup;

		[PXViewName(Messages.Equipment)]
		public PXSelect<Equipment> 
			Equipment;

		public EquipmentMaint()
		{
			var bAcctCache = Caches[typeof(BAccount)];
			PXUIFieldAttribute.SetDisplayName<BAccount.acctCD>(bAcctCache, Messages.Customer);
			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(bAcctCache, Messages.CustomerName);

			var contactCache = Caches[typeof(Contact)];
			PXUIFieldAttribute.SetDisplayName<Contact.displayName>(contactCache, Messages.Contact);
		}

		protected virtual void Equipment_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			var row = e.Row as Equipment;
			if (row == null) return;

			if (row.ExpireDate != null && row.StartDate != null && row.ExpireDate < row.StartDate)
				sender.RaiseExceptionHandling(typeof(INServiceItem.expireDate).Name, row, row.ExpireDate,
					new PXSetPropertyException(Messages.ExpireDateLessThanStartDate));
		}
	}
}
