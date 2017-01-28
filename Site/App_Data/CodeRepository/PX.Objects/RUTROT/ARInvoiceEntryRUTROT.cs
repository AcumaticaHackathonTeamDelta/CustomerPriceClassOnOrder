using System;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.BQLConstants;
using System.Linq;

namespace PX.Objects.RUTROT
{
	public class ARInvoiceEntryRUTROT : PXGraphExtension<ARInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>();
		}
		RUTROTManager<ARInvoice,ARTran,ARInvoiceRUTROT, ARTranRUTROT> RRManager;
		public override void Initialize()
		{
			base.Initialize();
			RRManager = new RUTROTManager<ARInvoice, ARTran, ARInvoiceRUTROT, ARTranRUTROT>(this.Base, Rutrots, RRDistribution, Base.Document, Base.Transactions, Base.currencyinfo);
		}		

		public PXSelect<RUTROT, Where<RUTROT.docType, Equal<Current<ARInvoice.docType>>,
			And<RUTROT.refNbr, Equal<Current<ARInvoice.refNbr>>>>> Rutrots;
		public PXSelect<RUTROTDistribution,
					Where<RUTROTDistribution.docType, Equal<Current<RUTROT.docType>>,
					And<RUTROTDistribution.refNbr, Equal<Current<RUTROT.refNbr>>>>> RRDistribution;

		public delegate void ReverseInvoiceProcDelegate(ARRegister doc);
		[PXOverride]
		public virtual void ReverseInvoiceProc(ARRegister doc, ReverseInvoiceProcDelegate baseMethod)
		{
			baseMethod(doc);
			Base.Document.SetValueExt<ARInvoiceRUTROT.isRUTROTDeductible>(Base.Document.Current, false);
			foreach (ARTran tran in Base.Transactions.Select())
			{
				Base.Transactions.SetValueExt<ARTranRUTROT.isRUTROTDeductible>(tran, false);
			}
		}
		#region Events

		#region ARInvoice
		protected virtual void ARInvoice_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RRManager.UncheckRUTROTIfProhibited(RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>(e.Row as ARInvoice), Base.CurrentBranch.SelectSingle(((ARInvoice)e.Row).BranchID));
		}
		protected virtual void ARInvoice_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RRManager.UncheckRUTROTIfProhibited(RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>(e.Row as ARInvoice), Base.CurrentBranch.SelectSingle(((ARInvoice)e.Row).BranchID));
		}
		
		protected virtual void ARInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ARInvoiceRUTROT doc = RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>(e.Row as ARInvoice);
			if (doc == null) return;

			RRManager.Update((ARInvoice)e.Row);
			UpdateLinesControls(doc.IsRUTROTDeductible == true);
		}
		protected virtual void ARInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARInvoiceRUTROT row = RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>((ARInvoice)e.Row);
			ARInvoiceRUTROT oldRow = RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>((ARInvoice)e.OldRow);
			RRManager.RUTROTDeductibleUpdated(row, oldRow, Rutrots.SelectSingle());
		}
		#endregion
		#region ARTran
		public virtual void ARTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			RRManager.UpdateTranDeductibleFromInventory(RUTROTHelper.GetExtensionNullable<ARTran, ARTranRUTROT>((ARTran)e.Row), RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>(Base.Document.Current));
		}
		protected virtual void ARTran_RUTROTType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTranRUTROT row = RUTROTHelper.GetExtensionNullable<ARTran, ARTranRUTROT>((ARTran)e.Row);
			if (row != null)
			{
				row.RUTROTWorkTypeID = null;
			}
		}
		protected virtual void ARTran_RUTROTItemType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARTran row = (ARTran)e.Row;
			ARTranRUTROT rowRR = RUTROTHelper.GetExtensionNullable<ARTran, ARTranRUTROT>(row);
			
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<ARTranRUTROT.rUTROTWorkTypeID>(sender, row, rowRR.RUTROTItemType != RUTROTItemTypes.OtherCost);
				if (rowRR.RUTROTItemType == RUTROTItemTypes.OtherCost)
				{
					sender.SetValueExt<ARTranRUTROT.rUTROTWorkTypeID>(row, null);
				}
			}
		}
		protected virtual void ARTran_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall)
			{
				e.Cancel = true;
			}
			ARTran tran = (ARTran)e.Row;
			if (tran == null)
				return;
			if (Base.Document.Current == null)
				return;
			if (e.NewValue == null)
				return;

			string value = Rutrots.Current?.RUTROTType;
			InventoryItem item = (InventoryItem)Base.InventoryItem.Select((int)e.NewValue);
			InventoryItemRUTROT itemRR = RUTROTHelper.GetExtensionNullable<InventoryItem, InventoryItemRUTROT>(item);
			if (!RUTROTHelper.IsItemMatchRUTROTType(value, item, itemRR, itemRR?.IsRUTROTDeductible == true))
			{
				sender.RaiseExceptionHandling<ARTran.inventoryID>(tran, item.InventoryCD, new PXSetPropertyException<ARTran.inventoryID>(RUTROTMessages.LineDoesNotMatchDoc));
				e.NewValue = item.InventoryCD;
				throw new PXSetPropertyException<ARTran.inventoryID>(RUTROTMessages.LineDoesNotMatchDoc);
			}
		}
		public virtual void ARTran_IsRUTROTDeductible_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARTran tran = (ARTran)e.Row;
			if (tran == null)
				return;
			if (Base.Document.Current == null)
				return;
			if (e.NewValue == null || (bool)e.NewValue == false)
				return;

			string value = Rutrots.Current?.RUTROTType;
			InventoryItem item = (InventoryItem)Base.InventoryItem.Select(tran.InventoryID);
			InventoryItemRUTROT itemRR = RUTROTHelper.GetExtensionNullable<InventoryItem,InventoryItemRUTROT>(item);
			if (!RUTROTHelper.IsItemMatchRUTROTType(value, item, itemRR, (bool)e.NewValue))
			{
				sender.RaiseExceptionHandling<ARTranRUTROT.isRUTROTDeductible>(tran, false, new PXSetPropertyException<ARTranRUTROT.isRUTROTDeductible>(RUTROTMessages.LineDoesNotMatchDoc));
				e.NewValue = false;
				throw new PXSetPropertyException<ARTranRUTROT.isRUTROTDeductible>(RUTROTMessages.LineDoesNotMatchDoc);
			}
		}

		protected virtual void ARTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;
			ARTran row = e.Row as ARTran;
			ARTranRUTROT rowRR = RUTROTHelper.GetExtensionNullable<ARTran, ARTranRUTROT>(row);
			RUTROTWorkType workType = PXSelect<RUTROTWorkType, Where<RUTROTWorkType.workTypeID, Equal<Required<RUTROTWorkType.workTypeID>>>>.Select(this.Base, rowRR.RUTROTWorkTypeID);
			if (rowRR?.IsRUTROTDeductible == true && Base.Document.Current.Released != true && rowRR?.RUTROTItemType!=RUTROTItemTypes.OtherCost
				&& Base.Document.Current.Voided != true && !RUTROTHelper.IsUpToDateWorkType(workType, Base.Document.Current.DocDate ?? DateTime.Now))
			{
				sender.RaiseExceptionHandling<ARTranRUTROT.rUTROTWorkTypeID>(row, workType?.Description, new PXSetPropertyException(RUTROTMessages.ObsoleteWorkType));
			}
			else
			{
				sender.RaiseExceptionHandling<ARTranRUTROT.rUTROTWorkTypeID>(row, workType?.Description, null);
			}
		}
		protected virtual void ARTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARTranRUTROT row = RUTROTHelper.GetExtensionNullable<ARTran, ARTranRUTROT>(e.Row as ARTran);
			CheckRUTROTLine(sender, RUTROTHelper.GetExtensionNullable<ARInvoice, ARInvoiceRUTROT>(Base.Document.Current), row, Rutrots.Current);
		}
		#endregion

		#endregion
		public void CheckRUTROTLine(PXCache sender, IRUTROTable document, IRUTROTableLine line, RUTROT rutrot)
		{
			if (line != null && document != null && rutrot != null)
			{
				if (document.IsRUTROTDeductible == true)
				{
					if (line.GetInventoryID() != null)
					{
						string value = rutrot.RUTROTType;
						InventoryItem item = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, line.GetInventoryID());
						InventoryItemRUTROT itemRR = RUTROTHelper.GetExtensionNullable<InventoryItem, InventoryItemRUTROT>(item);
						if (!RUTROTHelper.IsItemMatchRUTROTType(value, item, itemRR, itemRR?.IsRUTROTDeductible == true))
						{
							Base.Transactions.Cache.RaiseExceptionHandling<ARTran.inventoryID>(line, item.InventoryCD, new PXSetPropertyException<ARTran.inventoryID>(RUTROTMessages.LineDoesNotMatchDoc));
							throw new PXSetPropertyException<ARTran.inventoryID>(RUTROTMessages.LineDoesNotMatchDoc);
						}
					}
				}
			}
		}
		private void UpdateLinesControls(bool showSection)
		{
			PXUIFieldAttribute.SetVisible<ARTranRUTROT.isRUTROTDeductible>(Base.Transactions.Cache, null, showSection);
			PXUIFieldAttribute.SetEnabled<ARTranRUTROT.isRUTROTDeductible>(Base.Transactions.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<ARTranRUTROT.rUTROTItemType>(Base.Transactions.Cache, null, showSection);
			PXUIFieldAttribute.SetEnabled<ARTranRUTROT.rUTROTItemType>(Base.Transactions.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<ARTranRUTROT.rUTROTWorkTypeID>(Base.Transactions.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<ARTranRUTROT.curyRUTROTAvailableAmt>(Base.Transactions.Cache, null, showSection);
		}
	}
}
