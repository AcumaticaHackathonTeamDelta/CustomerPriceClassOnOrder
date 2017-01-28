using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CA;
using System.Collections;
using PX.Objects.CR;
using PX.Objects.CS;
using System.Web;
using System.Web.UI;
using PX.Web.UI;
using System.Linq;

namespace PX.Objects.GL
{
	[Serializable]
	public class GLVoucherBatchEntry : PXGraph<GLVoucherBatchEntry>
	{
		#region Type Override
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[CA.CAAPARTranType.ListByModuleRestricted(typeof(GLVoucher.module))]
		[PXUIField(Enabled = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
		protected virtual void GLVoucher_DocType_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[VoucherModule.List()]
		[PXUIField(Enabled=false, Visibility=PXUIVisibility.Invisible, Visible=false)]
		protected virtual void GLVoucher_Module_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region internal Types definition
		[Serializable]
		public class Filter : IBqlTable
		{
			#region WorkBookID
			public abstract class workBookID : PX.Data.IBqlField
			{
			}
			protected string _WorkBookID;
			[PXDBString(10, IsUnicode = true)]
			[PXDefault("APBILLS")]
			[PXUIField(DisplayName = "Account Class", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(GLWorkBook.workBookID))]
			public virtual string WorkBookID
			{
				get
				{
					return this._WorkBookID;
				}
				set
				{
					this._WorkBookID = value;
				}
			}
			#endregion

		}
		[Serializable]
		public class CreateBatchSettings : IBqlTable
		{
			public abstract class batchDescription : IBqlField { }

			[PXDBString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string BatchDescription { get; set; }

			public abstract class voucherBatchNbr : IBqlField { }
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
			[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = Messages.VoucherBatchNbr, Visible=true)]
			public virtual string VoucherBatchNbr { get; set; }
			
		}
		#endregion
		#region Selects & Cunstructor
		public GLVoucherBatchEntry()
		{			
			VouchersInBatch.AllowDelete = false;
			VouchersInBatch.AllowInsert = false;
			VouchersInBatch.AllowUpdate = false;
			VouchersInBatch.AllowSelect = false;
		}

		[PXFilterable]
		public PXProcessing<GLVoucherBatch, Where<GLVoucherBatch.workBookID, Equal<Current<Filter.workBookID>>, And<Match<Current<AccessInfo.userName>>>>, OrderBy<Desc<GLVoucherBatch.voucherBatchNbr>>> VoucherBatches;
		public PXSelect<GLWorkBook, Where<GLWorkBook.workBookID, Equal<Current<Filter.workBookID>>>> WorkBook;
		public PXSelect<GLVoucher, Where<GLVoucher.workBookID, Equal<Current<GLVoucherBatch.workBookID>>,
							And<GLVoucher.voucherBatchNbr, Equal<Current<GLVoucher.voucherBatchNbr>>>>, OrderBy<Asc<GLVoucher.refNbr>>> Vouchers;
		public PXSelect<GLVoucher, Where<GLVoucher.workBookID, Equal<Current<GLVoucherBatch.workBookID>>,
							And<GLVoucher.voucherBatchNbr, Equal<Current<GLVoucherBatch.voucherBatchNbr>>>>, OrderBy<Asc<GLVoucher.refNbr>>> VouchersInBatch;
		public PXSelectJoin<Numbering, InnerJoin<GLWorkBook, On<GLWorkBook.voucherBatchNumberingID, Equal<Numbering.numberingID>>>, Where<GLWorkBook.workBookID, Equal<Current<GLVoucherBatch.workBookID>>>> Numbering;
		
		public PXFilter<Filter> filter;
		public PXSelectReadonly<GLSetup> GLSetup;
		public PXSetup<Company> Company;
		public CMSetupSelect cmsetup;
		public PXFilter<CreateBatchSettings> BatchCreation;
		#endregion
		#region Actions
		public PXSavePerRow<GLVoucherBatch> Save;
		public PXCancel<GLVoucherBatch> Cancel;

		public PXCancel<GLVoucherBatch> CustomCancel;
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXCancelButton]
		protected virtual IEnumerable customCancel(PXAdapter adapter)
		{
			PXLongOperation.ClearStatus(this.UID);
			Cancel.Press();
			return adapter.Get();
		}
		public PXInsert<GLVoucherBatch> Insert;
		[PXUIField(DisplayName = ActionsMessages.Insert, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXInsertButton]
		protected virtual IEnumerable insert(PXAdapter adapter)
		{
			GLWorkBook wb = WorkBook.Select();
			if (wb.SingleOpenVoucherBatch == true)
			{
				int unreleasedCount=PXSelect<GLVoucherBatch, 
					Where<GLVoucherBatch.workBookID,Equal<Required<GLWorkBook.workBookID>>,
				And<GLVoucherBatch.released, Equal<False>>>>.Select(this, wb.WorkBookID).Count;
				if (unreleasedCount > 0)
				{
					throw new PXException(Messages.OnlyOneUnreleasedBatchAllowed);
				}
			}
			if (BatchCreation.View.Answer == WebDialogResult.None)
			{
				BatchCreation.Cache.Clear();
			}
			if (BatchCreation.AskExt() == WebDialogResult.OK)
			{
				if (VoucherBatches.Current != null)
					VoucherBatches.Cache.RaiseExceptionHandling<GLVoucherBatch.voucherBatchNbr>(VoucherBatches.Current, VoucherBatches.Current.VoucherBatchNbr, null);
				GLVoucherBatch batch = new GLVoucherBatch();
				batch.Descr = BatchCreation.Current.BatchDescription;
				Numbering numbering = Numbering.Select();
				bool isManualNumberig = numbering != null && numbering.UserNumbering == true;
				if (isManualNumberig)
				{
					batch.VoucherBatchNbr = BatchCreation.Current.VoucherBatchNbr;
				}
				batch = VoucherBatches.Insert(batch);
				this.VoucherBatches.Current = batch;
				Save.Press();
				editRecord.Press();
			}
			return adapter.Get();
		}

		public PXDelete<GLVoucherBatch> Delete;
		[PXUIField(DisplayName = CR.Messages.Delete, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXDeleteButton(ConfirmationType = PXConfirmationType.Unspecified, ImageUrl = Sprite.AliasMain + "@" + Sprite.Main.RecordDel)]
		public virtual IEnumerable delete(PXAdapter adapter)
		{
			GLVoucherBatch batch = VoucherBatches.Current;
			if (batch == null)
			{
				throw new PXException(Messages.NoBatchesForDelete);
			}
			if (VoucherBatches.Ask(PXMessages.LocalizeFormatNoPrefixNLA(Messages.BatchDeleteConfirmation, batch.VoucherBatchNbr), MessageButtons.OKCancel) == WebDialogResult.OK)
			{
				if (batch.Released == true)
				{
					throw new PXException(Messages.BatchDeleteReleased);
				}
				List<GLVoucherBatch> fullList = new List<GLVoucherBatch>();
				foreach (GLVoucherBatch voucherBatch in VoucherBatches.Select())//create list to show all records during processing
					fullList.Add(voucherBatch);
				PXLongOperation.ClearStatus(this.UID);
				PXLongOperation.StartOperation(this, delegate() { DeleteBatch(batch, fullList.ToArray<object>()); });
				TimeSpan timespan;
				Exception ex;
				PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
				DisableOnProcessing(status);
				VoucherBatches.View.RequestRefresh();
			}
			return adapter.Get();
		}

		public PXAction<GLVoucherBatch> editRecord;
		[PXUIField(DisplayName = Messages.ViewEditVoucherBatch, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(ShortcutCtrl=true,ShortcutChar = (char)69,Tooltip=GL.Messages.ViewEditVoucherBatchToolTip) ] //CTRL-E
		public virtual IEnumerable EditRecord(PXAdapter adapter)
		{
			RedirectToBatch(this.VoucherBatches.Current);
			return adapter.Get();
		}

		public PXAction<GLVoucherBatch> Release;
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		//MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select
		//should it be PXCacheRights.Update?
		[PXButton]
		public virtual IEnumerable release(PXAdapter adapter)
		{
			PXCache cache = Caches[typeof(GLVoucherBatch)];
			List<GLVoucherBatch> list = new List<GLVoucherBatch>();
			foreach (GLVoucherBatch batch in VoucherBatches.Select())
			{
				if (batch.Released != true && batch.Selected == true)
				{
					//cache.Update(batch);
					list.Add(batch);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.NoBatchesIsSelectedForRelease);
			}
			//Save.Press();
			if (list.Count > 0)
			{
				PXLongOperation.ClearStatus(this.UID);
				PXLongOperation.StartOperation(this, delegate() { ReleaseBatch(list); }); 
				TimeSpan timespan;
				Exception ex;
				PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
				DisableOnProcessing(status);
				VoucherBatches.View.RequestRefresh();
			}
			return adapter.Get();
		}

		public PXAction<GLVoucherBatch> ViewDocument;
		[PXUIField(DisplayName = Messages.WBViewEditVoucher, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			RedirectToBatch(this.VoucherBatches.Current, this.VouchersInBatch.Current);
			return adapter.Get();
		}
		#endregion
		#region Events
		protected virtual void CreateBatchSettings_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CreateBatchSettings row = (CreateBatchSettings)e.Row;
			if (row == null)
				return;
			Numbering numbering = Numbering.Select();
			bool isManualNumberig = numbering != null && numbering.UserNumbering == true;
			PXUIFieldAttribute.SetVisible<CreateBatchSettings.voucherBatchNbr>(BatchCreation.Cache, row, isManualNumberig);
			PXDefaultAttribute.SetPersistingCheck<CreateBatchSettings.voucherBatchNbr>(BatchCreation.Cache, row, isManualNumberig ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
		
		}
		protected virtual void Filter_WorkBookID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var page = HttpContext.Current.CurrentHandler as Page;
			string value = page.Request.QueryString["WorkBookID"];
			GLWorkBook wb = PXSelect<GLWorkBook, Where<GLWorkBook.workBookID, Equal<Required<GLWorkBook.workBookID>>>>.Select(this, value);
			if (wb != null)
			{
				e.NewValue = wb.WorkBookID;
				e.Cancel = true;
			}
		}
		protected virtual void GLVoucher_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GLVoucher row = (GLVoucher)e.Row;
			if (e.Row == null) return;

			Dictionary<string, Dictionary<Guid, CAMessage>> listMessages = PXLongOperation.GetCustomInfo(this.UID) as Dictionary<string, Dictionary<Guid, CAMessage>>;
			TimeSpan timespan;
			Exception ex;
			PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
			if ((status == PXLongRunStatus.Aborted || status == PXLongRunStatus.Completed) && listMessages != null)
			{
				string key = row.VoucherBatchNbr;
				if (listMessages.ContainsKey(key))
				{
					CAMessage message;
					if (listMessages[key].TryGetValue(row.RefNoteID ?? Guid.Empty, out message))
					{
						sender.RaiseExceptionHandling<GLVoucher.refNbr>(row, row.RefNbr, new PXSetPropertyException<GLVoucher.refNbr>(message.Message, message.ErrorLevel));
					}
				}
			}
			DisableOnProcessing(status);
		}

		private void DisableOnProcessing(PXLongRunStatus status)
		{
			bool noProcessing = status == PXLongRunStatus.NotExists;
			VoucherBatches.Cache.AllowUpdate = noProcessing;
			Insert.SetEnabled(noProcessing);
			Delete.SetEnabled(noProcessing);
			editRecord.SetEnabled(noProcessing);
			Release.SetEnabled(noProcessing);
		}
		protected virtual void GLVoucherBatch_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GLVoucherBatch row = (GLVoucherBatch)e.Row;
			if (e.Row == null) return;

			Dictionary<string, Dictionary<Guid, CAMessage>> listMessages = PXLongOperation.GetCustomInfo(this.UID) as Dictionary<string, Dictionary<Guid, CAMessage>>;
			TimeSpan timespan;
			Exception ex;
			PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
			if ((status == PXLongRunStatus.Aborted || status == PXLongRunStatus.Completed) && listMessages != null)
			{
				string key = row.VoucherBatchNbr;
				if (listMessages.ContainsKey(key))
				{
					VouchersInBatch.View.AllowSelect = true;
					CAMessage message;
					if (listMessages[key].TryGetValue(Guid.Empty, out message))
					{
						sender.RaiseExceptionHandling<GLVoucherBatch.voucherBatchNbr>(row, row.VoucherBatchNbr, new PXSetPropertyException<GLVoucherBatch.voucherBatchNbr>(message.Message, message.ErrorLevel));
					}
					else
					{
						sender.RaiseExceptionHandling<GLVoucherBatch.voucherBatchNbr>(row, row.VoucherBatchNbr, new PXSetPropertyException<GLVoucherBatch.voucherBatchNbr>("Processed", PXErrorLevel.RowInfo));
					}
				}
				else
				{
					VouchersInBatch.View.AllowSelect = false;
				}
			}
			else
			{
				VouchersInBatch.View.AllowSelect = false;
				PXUIFieldAttribute.SetEnabled(sender, null, status == PXLongRunStatus.NotExists);
			}
			DisableOnProcessing(status);
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled(sender, row, false);
				PXUIFieldAttribute.SetEnabled<GLVoucherBatch.selected>(sender, row, row.Released ==false);
				PXUIFieldAttribute.SetEnabled<GLVoucherBatch.descr>(sender, row, row.Released == false);
			}

			PXUIFieldAttribute.SetEnabled<GLVoucherBatch.released>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<GLVoucherBatch.voucherBatchNbr>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<GLVoucherBatch.docCount>(sender, null, false);
		}
		protected virtual void GLVoucherBatch_WorkBookID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = this.filter.Current.WorkBookID;
		}
		protected virtual void GLVoucherBatch_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			GLVoucherBatch row = (GLVoucherBatch)e.Row;
			if (row.Released == true)
				throw new PXException(Messages.BatchDeleteReleased);

			GLVoucher detail = PXSelect<GLVoucher,
									Where<GLVoucher.workBookID, Equal<Required<GLVoucherBatch.workBookID>>,
										And<GLVoucher.voucherBatchNbr, Equal<Required<GLVoucherBatch.voucherBatchNbr>>>>>.Select(this, row.WorkBookID, row.VoucherBatchNbr);
			if (detail != null)
				throw new PXException(Messages.BatchDeleteWithDocuments);
			//if (row.Module == GL.BatchModule.AP)
			//{
			//	AP.APRegister doc = PXSelect<AP.APRegister, Where<AP.APRegister.workBookID, Equal<Required<AP.APRegister.workBookID>>, And<AP.APRegister.voucherBatchNbr, Equal<Required<AP.APRegister.workBookID>>>>>.Select(this, row.WorkBookID, row.VoucherBatchNbr);
			//	if (doc != null) 
			//	{
			//		throw new PXException("Voucher batch contains documents inside and may not be deleted");
			//	}
			//}
		}
		bool prevDirty;
		protected virtual void GLVoucherBatch_Selected_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GLVoucherBatch row = (GLVoucherBatch)e.Row;
			if (row!=null && (bool?)e.NewValue == true && row.Released == true)
				e.NewValue = false;
		}
		protected virtual void GLVoucherBatch_Selected_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			prevDirty = sender.IsDirty;
		}
		protected virtual void GLVoucherBatch_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			GLVoucherBatch row = (GLVoucherBatch)e.Row;
			GLVoucherBatch oldRow = (GLVoucherBatch)e.OldRow;
			if (prevDirty == false)
			{
				bool nothingChanged = true;
				foreach (Type fieldType in sender.BqlFields)
				{
					if (fieldType != typeof(GLVoucherBatch.selected))
					{
						string field = sender.GetField(fieldType);
						if (!Equals(sender.GetValue(row, field), sender.GetValue(oldRow, field)))
						{
							nothingChanged = false;
							break;
						}
					}
				}
				if (nothingChanged)
					sender.IsDirty = prevDirty;
				else prevDirty = sender.IsDirty;
			}
		}

		#endregion
		#region Methods
		protected static void SetVoucherAsCurrent2(PXGraph target, GLVoucher voucher)
		{
			if (voucher != null)
			{
				if (voucher.Module == GL.BatchModule.AP)
				{
					if (voucher.DocType == AP.APDocType.Check || voucher.DocType == AP.APDocType.Prepayment)
					{
						(target as AP.APPaymentEntry).Document.Current = (target as AP.APPaymentEntry).Document.Search<AP.APPayment.refNbr, AP.APPayment.docType>(voucher.RefNbr, voucher.DocType);
					}
					else if (voucher.DocType == AP.APDocType.Invoice || voucher.DocType == AP.APDocType.CreditAdj || voucher.DocType == AP.APDocType.DebitAdj)
					{
						(target as AP.APInvoiceEntry).Document.Current = (target as AP.APInvoiceEntry).Document.Search<AP.APInvoice.refNbr, AP.APInvoice.docType>(voucher.RefNbr, voucher.DocType);
					}
					else if (voucher.DocType == AP.APDocType.QuickCheck)
					{
						(target as AP.APQuickCheckEntry).Document.Current = (target as AP.APQuickCheckEntry).Document.Search<AP.Standalone.APQuickCheck.refNbr>(voucher.RefNbr, voucher.DocType);
					}
				}
				else if (voucher.Module == GL.BatchModule.AR)
				{
					if (voucher.DocType == AR.ARDocType.Payment || voucher.DocType == AR.ARDocType.Prepayment)
					{
						(target as AR.ARPaymentEntry).Document.Current = (target as AR.ARPaymentEntry).Document.Search<AR.ARPayment.refNbr, AR.ARPayment.docType>(voucher.RefNbr, voucher.DocType);
					}
					else if (voucher.DocType == AR.ARDocType.Invoice || voucher.DocType == AR.ARDocType.CreditMemo || voucher.DocType == AR.ARDocType.DebitMemo)
					{
						(target as AR.ARInvoiceEntry).Document.Current = (target as AR.ARInvoiceEntry).Document.Search<AR.ARInvoice.refNbr, AR.ARInvoice.docType>(voucher.RefNbr, voucher.DocType);
					}
					else if (voucher.DocType == AR.ARDocType.CashSale)
					{
						(target as AR.ARCashSaleEntry).Document.Current = (target as AR.ARCashSaleEntry).Document.Search<AR.Standalone.ARCashSale.refNbr, AR.Standalone.ARCashSale.docType>(voucher.RefNbr, voucher.DocType);
					}
				}
				else if (voucher.Module == GL.BatchModule.CA && voucher.DocType == CA.CATranType.CAAdjustment)
				{
					(target as CA.CATranEntry).CAAdjRecords.Current = (target as CA.CATranEntry).CAAdjRecords.Search<CA.CAAdj.adjRefNbr, CA.CAAdj.adjTranType>(voucher.RefNbr, voucher.DocType);
				}
				else if (voucher.Module == GL.BatchModule.GL && voucher.DocType == CAAPARTranType.GLEntry)
				{
					(target as GL.JournalEntry).BatchModule.Current = (target as GL.JournalEntry).BatchModule.Search<GL.Batch.refNbr, GL.Batch.batchType>(voucher.RefNbr, voucher.DocType);
				}
			}
		}
		public static void SetVoucherAsCurrent(PXGraph current, PXGraph target, GLVoucher voucher)
		{
			if (voucher != null)
			{
				object row = new EntityHelper(current).GetEntityRow(voucher.RefNoteID, true);
				if (row != null)
				{
					PXView primary = target.Views[target.PrimaryView];
					primary.Cache.Current = row;
					if (primary.Cache.Current == null)
						throw new PXException(Messages.GLVoucherIsLinkedIncorrectly, voucher.RefNbr, voucher.WorkBookID);
				}
			}
		}
		protected virtual void SetVoucherAsCurrent(PXGraph target, GLVoucher voucher)
		{
			SetVoucherAsCurrent(this, target, voucher);
		}
		public static void DeleteBatch(GLVoucherBatch batch, object[] processingList)
		{
			GLVoucherBatchMaint pg = PXGraph.CreateInstance<GLVoucherBatchMaint>();
			Dictionary<string, Dictionary<Guid, CAMessage>> errorLog = new Dictionary<string, Dictionary<Guid, CAMessage>>();
			PXLongOperation.SetCustomInfo(errorLog, processingList);
			pg.Clear(PXClearOption.PreserveData);
			pg.DeleteBatchProc(batch, errorLog);
		}
		private void RedirectToBatch(GLVoucherBatch batch, GLVoucher voucher = null)
		{
			if (batch != null)
			{
				GLWorkBook wb = PXSelect<GLWorkBook, Where<GLWorkBook.workBookID, Equal<Required<GLWorkBook.workBookID>>>>.Select(this, batch.WorkBookID);
				if (wb == null || wb.VoucherEditScreen == null)
					throw new PXException(Messages.GLWorkBookIsInvalidOrVoucherEditScreenIsNotConfiguredForIt, batch.WorkBookID);

				PXGraph target = null;
				PXSiteMapNode sm = PXSiteMap.Provider.FindSiteMapNodeFromKey((Guid)wb.VoucherEditScreen);
				if (sm == null)
					throw new PXException(Messages.CannotFindSitemapNode);
				Type graphType = System.Web.Compilation.BuildManager.GetType(sm.GraphType, true);
				Type parentGraphType = GLWorkBookMaint.GetGraphByDocType(batch.Module, wb.DocType);
				if (parentGraphType != null && parentGraphType.IsAssignableFrom(graphType))
				{
					target = PXGraph.CreateInstance(graphType);
				}
				else
				{
					throw new PXException(Messages.GLVoucherEditGraphMayNotBeAssignedToTheBasedDocumentGraph, graphType, parentGraphType);
				}
				if (voucher == null)
					voucher = PXSelect<GLVoucher, Where<GLVoucher.workBookID, Equal<Required<GLVoucher.workBookID>>,
						And<GLVoucher.voucherBatchNbr, Equal<Required<GLVoucher.voucherBatchNbr>>>>, OrderBy<Desc<GLVoucher.refNbr>>>.Select(this, batch.WorkBookID, batch.VoucherBatchNbr);
				else voucher = PXSelect<GLVoucher, Where<GLVoucher.workBookID, Equal<Required<GLVoucher.workBookID>>,
						And<GLVoucher.voucherBatchNbr, Equal<Required<GLVoucher.voucherBatchNbr>>, And<GLVoucher.refNbr, Equal<Required<GLVoucher.refNbr>>>>>, OrderBy<Desc<GLVoucher.refNbr>>>.Select(this, batch.WorkBookID, batch.VoucherBatchNbr, voucher.RefNbr);
				SetVoucherAsCurrent(target, voucher);
				throw new PXRedirectWithinContextException(this, target, "Voucher Edit", typeof(GLVoucherBatch.workBookID), typeof(GLVoucherBatch.voucherBatchNbr));
			}
		}
		public static void ReleaseBatch(List<GLVoucherBatch> list)
		{
			GLVoucherBatchMaint pg = PXGraph.CreateInstance<GLVoucherBatchMaint>();

			Dictionary<string, Dictionary<Guid, CAMessage>> errorLog = new Dictionary<string, Dictionary<Guid, CAMessage>>();
			List<int> failed = new List<int>();
			PXLongOperation.SetCustomInfo(errorLog);
			int failedCount = 0;
			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear(PXClearOption.PreserveData);
				GLVoucherBatch batch = list[i];
				try
				{
					pg.ReleaseBatchProc(batch, errorLog);
				}
				catch (Exception)
				{
					failedCount++;
				}
			}
			if (failedCount > 0)
			{
				throw new PXException(Messages.BatchReleaseFiled);
			}
		}
		#endregion
	}
}

namespace PX.Objects.GL
{
	[Serializable]
	public class GLVoucherBatchMaint : PXGraph<GLVoucherBatchMaint,GLVoucherBatch>	
	{
		public PXSelect<GLVoucherBatch, Where<GLVoucherBatch.workBookID, Equal<Required<GLVoucherBatch.workBookID>>,
								And<GLVoucherBatch.voucherBatchNbr, Equal<Required<GLVoucherBatch.voucherBatchNbr>>>>> Document;

		public PXSelectJoin<GLVoucher, 
					LeftJoin<AP.APRegister, On<AP.APRegister.docType,Equal<GLVoucher.docType>,
												And<AP.APRegister.refNbr,Equal<GLVoucher.refNbr>,
												And<GLVoucher.module,Equal<GL.BatchModule.moduleAP>>>>,

					LeftJoin<AR.ARRegister, On<AR.ARRegister.docType,Equal<GLVoucher.docType>,
												And<AR.ARRegister.refNbr,Equal<GLVoucher.refNbr>,
												And<GLVoucher.module,Equal<GL.BatchModule.moduleAR>>>>,
					LeftJoin<CAAdj, On<CAAdj.adjTranType,Equal<GLVoucher.docType>,
												And<CAAdj.adjRefNbr,Equal<GLVoucher.refNbr>,
												And<GLVoucher.module,Equal<GL.BatchModule.moduleCA>>>>,
					LeftJoin<Batch, On<Batch.module,Equal<GL.BatchModule.moduleGL>,
												And<Batch.batchNbr,Equal<GLVoucher.refNbr>,
												And<GLVoucher.module,Equal<GL.BatchModule.moduleGL>>>>>>>>,					
				Where<GLVoucher.workBookID,Equal<Current<GLVoucherBatch.workBookID>>, 
						And<GLVoucher.voucherBatchNbr,Equal<Current<GLVoucherBatch.voucherBatchNbr>>>>> Details;

		#region Public Methods
		public void ReleaseBatchProc(GLVoucherBatch aBatch, Dictionary<string, Dictionary<Guid, CAMessage>> errorList)
		{
			this.Clear();
			GLVoucherBatch row = (GLVoucherBatch)this.Document.Select(aBatch.WorkBookID, aBatch.VoucherBatchNbr);
			this.Document.Current = row;
			Dictionary<Guid, CAMessage> errorLog = null;
			if (!errorList.ContainsKey(row.VoucherBatchNbr))
			{
				errorList.Add(row.VoucherBatchNbr, new Dictionary<Guid, CAMessage>());
			}
			errorLog = errorList[row.VoucherBatchNbr];
			List<Batch> toPost = new List<Batch>();
			try
			{
				Exception releaseException = null;
				try
				{
					this.ReleaseBatchDetailsProc(aBatch, toPost, errorLog);
					if (HasUnreleasedDetails(aBatch) == false)
					{
						GLVoucherBatch copy = (GLVoucherBatch)this.Document.Cache.CreateCopy(row);
						copy.Released = true;
						this.Document.Update(copy);
						this.Actions.PressSave();
					}
				}
				catch (Exception ex)
				{ 
					releaseException = ex; 
				}
				
				List<Batch> postFailedList = new List<Batch>();
				if (toPost.Count > 0)
				{
					PostGraph pg = PXGraph.CreateInstance<PostGraph>();
					foreach (Batch iBatch in toPost)
					{
						try
						{
							//if (rg.AutoPost)
							{
								pg.Clear();
								pg.PostBatchProc(iBatch);
							}
						}
						catch (Exception)
						{
							postFailedList.Add(iBatch);
						}
					}
				}
				if (releaseException != null)
				{
					throw releaseException;
				}
				if (postFailedList.Count > 0)
				{
					throw new PXException(Messages.PostingOfSomeDocumentsFailed, postFailedList.Count, toPost.Count);
				}
			}
			catch (PXException ex)
			{
				errorLog.Add(Guid.Empty, FormatError(ex));
				throw ex;
			}
		}
		public void DeleteBatchProc(GLVoucherBatch aBatch, Dictionary<string, Dictionary<Guid, CAMessage>> errorList)
		{
			this.Clear();
			this.Document.Current = aBatch;
			Dictionary<Guid, CAMessage> errorLog = null;
			if (!errorList.ContainsKey(aBatch.VoucherBatchNbr))
			{
				errorList.Add(aBatch.VoucherBatchNbr, new Dictionary<Guid, CAMessage>());
			}
			errorLog = errorList[aBatch.VoucherBatchNbr];

			try
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					List<Guid> failed = new List<Guid>();
					Dictionary<string, List<GLVoucher>> processQueue = new Dictionary<string, List<GLVoucher>>();
					foreach (GLVoucher voucher in Details.Select())
					{
						if (!processQueue.ContainsKey(voucher.Module + voucher.DocType))
						{
							processQueue.Add(voucher.Module + voucher.DocType, new List<GLVoucher>());
						}
						processQueue[voucher.Module + voucher.DocType].Add(voucher);
					}
					foreach (KeyValuePair<string, List<GLVoucher>> pair in processQueue)
					{
						string module = pair.Value[0].Module;
						string docType = pair.Value[0].DocType;
						Type graphType = GLWorkBookMaint.GetGraphByDocType(module, docType);
						if (graphType == null)
							throw new PXException(GL.Messages.ModuleDocTypeIsNotSupported, module, docType);
						PXGraph deletegraph = PXGraph.CreateInstance(graphType);
						if (!(deletegraph is IVoucherEntry))
							throw new PXException(GL.Messages.ModuleDocTypeIsNotSupported, module, docType);
						foreach (GLVoucher voucher in pair.Value)
						{
							Guid key = voucher.RefNoteID.Value;
							try
							{
								deletegraph.Clear();
								GLVoucherBatchEntry.SetVoucherAsCurrent(this, deletegraph, voucher);
								(deletegraph as IVoucherEntry).DeleteButton.Press();
							}
							catch (Exception ex)
							{
								failed.Add(key);
								errorLog.Add(key, FormatError(ex));
								throw new PXException(Messages.DeletingFailed);
							}
						}
					}
					this.Document.Delete(aBatch);
					this.Save.Press();
					ts.Complete(this);
				}
			}
			catch (PXException ex)
			{
				errorLog.Add(Guid.Empty, FormatError(ex));
				throw ex;
			}
		}
		protected virtual bool HasUnreleasedDetails(GLVoucherBatch batch)
		{
			foreach (PXResult<GLVoucher, AP.APRegister, AR.ARRegister, CAAdj, Batch> it in this.Details.Select(batch.WorkBookID, batch.VoucherBatchNbr))
			{
				GLVoucher iVoucher = it;
				AP.APRegister iAPR = it;
				AR.ARRegister iARR = it;
				CAAdj iCAR = it;
				Batch iGL = it;
				switch (iVoucher.Module)
				{
					case GL.BatchModule.AP:
						if (iAPR == null || String.IsNullOrEmpty(iAPR.RefNbr))
							throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
						return iAPR.Released != true;						
					case GL.BatchModule.AR:
						if (iARR == null || String.IsNullOrEmpty(iARR.RefNbr))
							throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
						return iARR.Released != true;						
					case GL.BatchModule.CA:
						if (iCAR == null || String.IsNullOrEmpty(iCAR.AdjRefNbr))
							throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
						return iCAR.Released != true;						
					case GL.BatchModule.GL:
						if (iGL == null || String.IsNullOrEmpty(iGL.BatchNbr))
							throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
						return iGL.Released != true;						
					default:
						throw new PXException(GL.Messages.ModuleIsNotSupported, iVoucher.Module);
				}		
			}
			return true;
		}
		protected virtual void ReleaseBatchDetailsProc(GLVoucherBatch aBatch, List<Batch> toPost, Dictionary<Guid,CAMessage> errors)
		{			
			int toProcessCount = 0;
			List<Guid> failed = new List<Guid>();
			Dictionary<string, IList> processQueue = new Dictionary<string,IList>();
			foreach (PXResult<GLVoucher,AP.APRegister, AR.ARRegister,CAAdj, Batch> it in this.Details.Select(aBatch.WorkBookID,aBatch.VoucherBatchNbr))
			{
				GLVoucher iVoucher = it;
				Guid key = iVoucher.RefNoteID.Value;
				try
				{
					switch (iVoucher.Module)
					{
						case GL.BatchModule.AP:
							AP.APRegister iAPR = it;
							if (iAPR == null || String.IsNullOrEmpty(iAPR.RefNbr))
								throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
							if (iAPR.Released != true)
							{
								toProcessCount++;
								List<AP.APRegister> list = new List<AP.APRegister>(1);
								list.Add(iAPR);
								if (iAPR.Scheduled == true)
									throw new PXException(Messages.CannotReleaseScheduled);
								AP.APDocumentRelease.ReleaseDoc(list, false, false, toPost);				
							}
							break;
						case GL.BatchModule.AR:
							AR.ARRegister iARR = it;
							if (iARR == null || String.IsNullOrEmpty(iARR.RefNbr))
								throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
							if (iARR.Released != true)
							{
								toProcessCount++;
								List<AR.ARRegister> list = new List<AR.ARRegister>(1);
								list.Add(iARR);
								if (iARR.Scheduled == true)
									throw new PXException(Messages.CannotReleaseScheduled);
								AR.ARDocumentRelease.ReleaseDoc(list, false, toPost, null);
							}
							break;
						case GL.BatchModule.CA:
							CAAdj iCAR = it;
							if (iCAR == null || String.IsNullOrEmpty(iCAR.AdjRefNbr))
								throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
							if (iCAR.Released != true)
							{
								toProcessCount++;
								CATrxRelease.ReleaseDoc<CAAdj>(iCAR, 0, toPost);
							}
							break;

						case GL.BatchModule.GL:
							Batch iGL = it;
							if (iGL == null || String.IsNullOrEmpty(iGL.BatchNbr))
								throw new PXException(GL.Messages.DocumentIsInvalidForVoucher, iVoucher.RefNbr, iVoucher.VoucherBatchNbr);
							if (iGL.Released != true)
							{
								toProcessCount++;
								List<Batch> list = new List<Batch>(1);
								list.Add(iGL);
								if (iGL.Scheduled == true)
									throw new PXException(Messages.CannotReleaseScheduled);
								JournalEntry.ReleaseBatch(list, toPost);
							}
							break;
						default: 
							throw new PXException(GL.Messages.ModuleIsNotSupported, iVoucher.Module);
					}
				}
				catch (Exception ex)
				{
					failed.Add(key); //prevent process from stoping on first error										
					errors.Add(key,FormatError(ex));
				}						
			}							
			if (failed.Count > 0)
			{
				throw new PXException(Messages.ReleasingOfSomeOfTheIncludedDocumentsFailed, failed.Count, toProcessCount);
			}			
		}

		protected static CAMessage FormatError(Exception ex) 
		{
			string message = ex is PXOuterException ? (ex.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)ex).InnerMessages)) : ex.Message;
			return new CAMessage(0, PXErrorLevel.RowError, message);
		}
		#endregion
	}

}

