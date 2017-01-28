using PX.Data;
using PX.Objects;
using System;
using System.Collections;
using ContextFieldDescriptor = PX.Data.PXGraph.ContextFieldDescriptor;
using PX.Objects.CS;

namespace PX.Objects.GL
{

	public class JournalEntryExt : PXGraphExtension<JournalEntry>
	{
		public override void Initialize()
		{
			base.Initialize();
			Base.SetContextFields(new ContextFieldDescriptor[] { 
															new ContextFieldDescriptor<GLVoucher.refNbr>(false),															
															new ContextFieldDescriptor<Batch.batchNbr>(true, true), 
															new ContextFieldDescriptor<Batch.batchType>(true, false),
															new ContextFieldDescriptor<Batch.dateEntered>(false),
															new ContextFieldDescriptor<Batch.curyDebitTotal>(false),
															new ContextFieldDescriptor<Batch.curyID>(false),
															new ContextFieldDescriptor<Batch.status>(false),
															new ContextFieldDescriptor<Batch.branchID>(false),
															new ContextFieldDescriptor<Batch.ledgerID>(false)
															});

			Base.SetContextHeaderFields(typeof(GLVoucherBatch.workBookID), typeof(GLVoucherBatch.voucherBatchNbr));
			Base.BatchModule.Join<LeftJoin<GLVoucher, On<GLVoucher.docType, Equal<Batch.batchType>,
					And<GLVoucher.refNbr, Equal<Batch.batchNbr>, And<GLVoucher.module, Equal<GL.BatchModule.moduleGL>>>>>>();
		}

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.gLWorkBooks>();
		}

		#region Buttons
		public PXAction<Batch> SaveAndAdd;
		[PXUIField(DisplayName = GL.Messages.SaveAndAdd, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton(ShortcutCtrl = true, ShortcutChar = (char)65, Tooltip = GL.Messages.SaveAndAddToolTip)] //Ctrl-A
		public virtual IEnumerable saveAndAdd(PXAdapter adapter)
		{
			Base.Save.Press();
			return Base.Insert.Press(adapter);
		}
		public PXAction<Batch> viewVoucherBatch;
		[PXUIField(DisplayName = GL.Messages.ViewVoucherBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable ViewVoucherBatch(PXAdapter adapter)
		{
			GLVoucherBatchEntry graph = PXGraph.CreateInstance<GLVoucherBatchEntry>();
			graph.filter.Current = new GLVoucherBatchEntry.Filter();
			graph.filter.Current.WorkBookID = Voucher.Current.WorkBookID;
			graph.VoucherBatches.Current = graph.VoucherBatches.Search<GLVoucherBatch.voucherBatchNbr>(Voucher.Current.VoucherBatchNbr);
			graph.VouchersInBatch.Current = graph.VouchersInBatch.Search<GLVoucher.refNbr, GLVoucher.docType, GLVoucher.module>(Voucher.Current.RefNbr, Voucher.Current.DocType, Voucher.Current.Module);
			graph.ViewDocument.Press();
			return adapter.Get();
		}
		public PXAction<Batch> viewWorkBook;
		[PXUIField(DisplayName = GL.Messages.ViewWorkBook, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable ViewWorkBook(PXAdapter adapter)
		{
			GLVoucherBatchEntry graph = PXGraph.CreateInstance<GLVoucherBatchEntry>();
			graph.filter.Current = new GLVoucherBatchEntry.Filter();
			graph.filter.Current.WorkBookID = Voucher.Current.WorkBookID;
			graph.VoucherBatches.Current = graph.VoucherBatches.Search<GLVoucherBatch.voucherBatchNbr>(Voucher.Current.VoucherBatchNbr);
			string url = GLWorkBookMaint.WORKBOOK_URL + "?WorkBookID=" + Voucher.Current.WorkBookID;
			throw new PXRedirectRequiredException(url, graph, "");
		}
		#endregion

		#region Selects & Delegates
		protected virtual IEnumerable batchModule()
		{
			return Base.SelectWithinContext(
				new Select2<Batch,
						InnerJoin<GLVoucher, On<GLVoucher.refNoteID, Equal<Batch.noteID>>>,
						Where<Batch.module, Equal<GL.BatchModule.moduleGL>,
							And<Batch.draft, Equal<False>,
							And<GLVoucher.workBookID, Equal<Context<GLVoucherBatch.workBookID>>,
							And<GLVoucher.voucherBatchNbr, Equal<Context<GLVoucherBatch.voucherBatchNbr>>>>>>>());
		}

		public PXSelect<GLVoucher,
					Where<GLVoucher.docType, Equal<Current<Batch.batchType>>,
						And<GLVoucher.refNbr, Equal<Current<Batch.batchNbr>>,
						And<GLVoucher.module, Equal<GL.BatchModule.moduleGL>>>>> Voucher;

		public PXSelect<GLVoucherBatch,
					Where<GLVoucherBatch.workBookID, Equal<Optional<GLVoucher.workBookID>>,
						And<GLVoucherBatch.voucherBatchNbr, Equal<Optional<GLVoucher.voucherBatchNbr>>>>> VoucherBatch;

		#endregion

		#region CacheAttached -Voucher Batch

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXDBDefault(typeof(Batch.batchNbr))]
		[PXParent(typeof(Select<Batch, Where<Batch.batchType, Equal<Current<GLVoucher.docType>>,
							And<Batch.batchNbr, Equal<Current<GLVoucher.refNbr>>>>>))]
		protected virtual void GLVoucher_RefNbr_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(3, IsUnicode = true)]
		[PXDBDefault(typeof(Batch.batchType))]
		protected virtual void GLVoucher_DocType_CacheAttached(PXCache sender)
		{
		}

		[PXDBGuid()]
		[PXDefault(typeof(Batch.noteID), PersistingCheck = PXPersistingCheck.Null)]
		protected virtual void GLVoucher_RefNoteID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region Events - GLVoucher
		protected virtual void GLVoucher_WorkbookID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = Base.GetContextValue<GLVoucherBatch.workBookID>();
		}

		protected virtual void GLVoucher_Module_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = GL.BatchModule.GL;
		}

		protected virtual void GLVoucher_VoucherBatchNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = Base.GetContextValue<GLVoucherBatch.voucherBatchNbr>();
		}
		#endregion

		#region Events - Batch
		protected virtual void Batch_BatchNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e, PXFieldDefaulting bs)
		{
			Batch row = e.Row as Batch;
			SetNumbering(sender, row);
			if (bs != null)
				bs(sender, e);
		}
		protected virtual void Batch_RowInserted(PXCache sender, PXRowInsertedEventArgs e, PXRowInserted bs)
		{
			if (bs != null)
				bs(sender, e);

			if (Base.IsWithinContext)
			{
				string vb = Base.GetContextValue<GLVoucherBatch.voucherBatchNbr>();
				string wbID = Base.GetContextValue<GLVoucherBatch.workBookID>();
				this.VoucherBatch.Current = this.VoucherBatch.Select(wbID, vb);
				GLWorkBook wb = PXSelect<GLWorkBook,
					Where<GLWorkBook.workBookID, Equal<Required<GLVoucherBatch.workBookID>>>>.Select(this.Base, Base.GetContextValue<GLVoucherBatch.workBookID>());
				if (!String.IsNullOrEmpty(vb))
				{
					Guid? noteID = PXNoteAttribute.GetNoteID<Batch.noteID>(sender, e.Row);
					this.Voucher.Insert(new GLVoucher());
					this.Voucher.Cache.IsDirty = false;
					Base.Caches[typeof(Note)].IsDirty = false;
				}
				if (wb.DefaultDescription != null)
					sender.SetValueExt<Batch.description>(e.Row, wb.DefaultDescription);
			}
		}
		protected virtual void Batch_RowPersisting(PXCache sender, PXRowPersistingEventArgs e, PXRowPersisting bs)
		{
			if (bs != null)
				bs(sender, e);
			Batch row = e.Row as Batch;
			SetNumbering(sender, row);
		}

		private void SetNumbering(PXCache sender, Batch row)
		{
			if (row == null) return;
			if (Base.IsWithinContext)
			{
				Numbering numbering = PXSelectJoin<Numbering,
					InnerJoin<GLWorkBook, On<GLWorkBook.voucherNumberingID, Equal<Numbering.numberingID>>>,
					Where<GLWorkBook.workBookID, Equal<Required<GLWorkBook.workBookID>>>>.Select(this.Base, Base.GetContextValue<GLVoucherBatch.workBookID>());
				AutoNumberAttribute.SetNumberingId<Batch.batchNbr>(sender, row.Module, numbering.NumberingID);
				if (numbering.UserNumbering == true)
				{
					sender.RaiseExceptionHandling<Batch.batchNbr>(row, row.BatchNbr, new PXException(GL.Messages.ManualNumberingDisabled));
				}			
			}
		}

		protected virtual void Batch_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected bs)
		{
			if (bs != null)
				bs(sender, e);
			Batch row = e.Row as Batch;
			if (row == null) return;
			bool isWithinContext = Base.IsWithinContext;
			GLVoucher voucher = this.Voucher.Select();
			GLVoucherBatch voucherBatch = this.VoucherBatch.Select();
			PXCache voucherCache = this.Voucher.Cache;
			bool isDetached = (voucher == null);
			PXUIFieldAttribute.SetEnabled<GLVoucher.refNbr>(voucherCache, voucher, false);
			PXUIFieldAttribute.SetVisible<GLVoucher.refNbr>(voucherCache, voucher, !isDetached);
			PXUIFieldAttribute.SetEnabled<Batch.batchNbr>(sender, e.Row, !isWithinContext);
			this.SaveAndAdd.SetVisible(!isDetached && isWithinContext);
			this.SaveAndAdd.SetEnabled(!isDetached && isWithinContext);
			if (isWithinContext)
			{
				Base.release.SetVisible(false);
				Base.release.SetEnabled(false);
				Base.reverseBatch.SetVisible(false);
				Base.reverseBatch.SetEnabled(false);
				PXUIFieldAttribute.SetVisible<Batch.autoReverse>(sender, row, false);
				PXUIFieldAttribute.SetEnabled<Batch.autoReverse>(sender, row, false);
			}
			if (!isDetached)
			{
				Base.createSchedule.SetVisible(false);
				Base.createSchedule.SetEnabled(false);
			}
			PXUIFieldAttribute.SetVisible<GLVoucher.voucherBatchNbr>(Voucher.Cache, null, !isDetached && !isWithinContext);
			PXUIFieldAttribute.SetVisible<GLVoucher.workBookID>(Voucher.Cache, null, !isDetached && !isWithinContext);
			if (isWithinContext && !isDetached)
			{
				sender.AllowInsert = !(voucherBatch != null && voucherBatch.Released == true);
			}
		}
		#endregion
	}
}
