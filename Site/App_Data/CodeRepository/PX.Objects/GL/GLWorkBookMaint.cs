using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CM;
using PX.Objects.CA;
using System.Linq;
using System.Text;
using PX.Common;
using System.Threading.Tasks;
using PX.SM;


namespace PX.Objects.GL
{
	[Serializable]
	public class GLWorkBookMaint : PXGraph<GLWorkBookMaint, GLWorkBook>
	{
		#region Type Override
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[CA.CAAPARTranType.ListByModuleRestricted(typeof(GLWorkBook.module))]
		protected virtual void GLWorkBook_DocType_CacheAttached(PXCache sender)
		{
		}

		#endregion

		public const string WORKBOOK_URL = "~/Pages/GL/GL307000.aspx";
		public const string WORKBOOK_FOLDER_SCREEN_ID = "GL307010";

		[PXImport(typeof(GLWorkBook))]
		public PXSelect<GLWorkBook> WorkBooks;
		public PXSelect<GLVoucherBatch, Where<GLVoucherBatch.workBookID, Equal<Current<GLWorkBook.workBookID>>>> batches;

		public GLWorkBookMaint()
		{
			_screenToSiteMapAddHelper = new PXScreenToSiteMapAddHelper<GLWorkBook>("GL", GLWorkBookMaint.WORKBOOK_URL,
				typeof(GLWorkBook.sitemapParent), typeof(GLWorkBook.sitemapTitle), typeof(GLWorkBook.descr), typeof(GLWorkBook.sitemapID),
				typeof(GLWorkBook.workBookID), typeof(GLWorkBook.workBookID).Name.With(_ => char.ToUpper(_[0]) + _.Substring(1)), false, Caches[typeof(GLWorkBook)], SiteMapTree.Cache);
			numberingDetector = new CS.NumberingDetector(this, CS.ApplicationAreas.None);
			FillSequences();
		}

		#region Numbering sequence checker
		IEnumerable<string> otherNumberingIDs;
		CS.NumberingDetector numberingDetector;
		protected virtual void FillSequences()
		{
			otherNumberingIDs = GetOtherNumberingIDs();
		}
		protected virtual void CheckNumbering(PXCache sender, GLWorkBook row)
		{
			if (row != null & row.VoucherNumberingID != null &&
				PXUIFieldAttribute.GetErrors(sender, row, PXErrorLevel.Error, PXErrorLevel.RowError).Count < 1)
			{
				bool foundCollision = false;
				if (row.VoucherBatchNumberingID != null && row.VoucherBatchNumberingID == row.VoucherNumberingID)
				{
					foundCollision = true;
					sender.RaiseExceptionHandling<GLWorkBook.voucherNumberingID>(row, row.VoucherNumberingID, new PXSetPropertyException(Messages.SameNumberingForVoucherAndBatch, PXErrorLevel.Warning));
				}
				if (foundCollision == false)
				{
					string workBookID;
					if (numberingDetector.IsInUseWorkbooks(row.VoucherNumberingID, out workBookID, row.WorkBookID))
					{
						foundCollision = true;
						sender.RaiseExceptionHandling<GLWorkBook.voucherNumberingID>(row, row.VoucherNumberingID, new PXSetPropertyException(Messages.NumberingSequenceOtherWorkBook, PXErrorLevel.Warning, workBookID));
					}
				}
				if (foundCollision == false)
				{
					string cacheName;
					string fieldName;
					if (numberingDetector.IsInUseSetups(row.VoucherNumberingID, out cacheName, out fieldName))
						{
							foundCollision = true;
						sender.RaiseExceptionHandling<GLWorkBook.voucherNumberingID>(row, row.VoucherNumberingID, new PXSetPropertyException(Messages.NumberingSequencePreferences, PXErrorLevel.Warning, cacheName, fieldName));
					}
				}
				if (foundCollision == false && otherNumberingIDs != null && otherNumberingIDs.Any_())
				{
					foreach (string numberingID in otherNumberingIDs)
					{
						if (CS.NumberingDetector.CanNumberingIntersect(row.VoucherNumberingID, numberingID, this))
						{
							foundCollision = true;
							sender.RaiseExceptionHandling<GLWorkBook.voucherNumberingID>(row, row.VoucherNumberingID, new PXSetPropertyException(Messages.NumberingSequencesIntersection, PXErrorLevel.RowWarning, numberingID));
							break;
						}
					}
				}
				if (foundCollision == false)
					sender.RaiseExceptionHandling<GLWorkBook.voucherNumberingID>(row, row.VoucherNumberingID, null);
			}
		}
		protected virtual IEnumerable<string> GetOtherNumberingIDs()
		{
			if (WorkBooks.Current != null && WorkBooks.Current.Module != null && WorkBooks.Current.DocType != null)
			{
				yield return GetBasicNumberingID();
				foreach (GLWorkBook wb in PXSelect<GLWorkBook, Where<GLWorkBook.module, Equal<Current<GLWorkBook.module>>, And<GLWorkBook.docType, Equal<Current<GLWorkBook.docType>>, And<GLWorkBook.workBookID, NotEqual<Current<GLWorkBook.workBookID>>>>>>.Select(this))
				{
					yield return wb.VoucherNumberingID;
				}
			}
		}
		protected virtual string GetBasicNumberingID()
		{
			GLWorkBook currentWB = this.WorkBooks.Current;
			if (currentWB == null || currentWB.Module == null || currentWB.DocType == null)
				return null;
			Type numberingField;
			switch (currentWB.Module)
			{
				case BatchModule.CA:
					numberingField = typeof(CASetup.registerNumberingID);
					break;
				case BatchModule.GL:
					numberingField = BatchModule.NumberingAttribute.GetNumberingIDField(BatchModule.GL);
					break;
				case BatchModule.AP:
					numberingField = AP.APInvoiceType.NumberingAttribute.GetNumberingIDField(currentWB.DocType);
					if (numberingField == null)
						numberingField = AP.APPaymentType.NumberingAttribute.GetNumberingIDField(currentWB.DocType);
					break;
				case BatchModule.AR:
					numberingField = AR.ARInvoiceType.NumberingAttribute.GetNumberingIDField(currentWB.DocType);
					if (numberingField == null)
						numberingField = AR.ARPaymentType.NumberingAttribute.GetNumberingIDField(currentWB.DocType);
					break;
				default: return null;
			}
			if (numberingField == null)
				return null;
			Type select =
				BqlCommand.Compose(
					typeof(Select<,>), typeof(CS.Numbering),
					typeof(Where<,>), typeof(CS.Numbering.numberingID),
					typeof(Equal<>), typeof(Current<>), numberingField);
			PXView view = new PXView(this, false, BqlCommand.CreateInstance(select));
			var item = view.SelectSingle();
			return ((CS.Numbering)item).NumberingID;
		}
		#endregion
		#region Site-Map selectors
		#region Voucher Batch Accessor SiteMap node Selector

		[PXCopyPasteHiddenView]
		public PXSelectSiteMapTree<False, False, False, False, False> SiteMapTree;
		public PX.SM.PXScreenToSiteMapAddHelper<GLWorkBook> _screenToSiteMapAddHelper;
		public bool IsSiteMapAltered { get; protected set; } 
		#endregion

		#region Voucher Edit Screen  Selector
		[PXCopyPasteHiddenView]
		public PXSelect<SiteMap, Where<SiteMap.parentID, Equal<Argument<Guid?>>>, OrderBy<Asc<SiteMap.position>>> VoucherEntryScreenTree;
		PXSiteMapEnumeratorBase _siteMapEnumerator;
		protected virtual IEnumerable voucherEntryScreenTree(
			[PXGuid]
			Guid? NodeID)
		{
			//var primaryViews = new HashSet<string>(Tables.Select().Select(t => ((GITable)t).Name));
			GLWorkBook row = this.WorkBooks.Current;

			if (NodeID == null)
				_siteMapEnumerator = new PXSiteMapFilteredPathEnumerator(false, new PXSiteMapNodeFilter(
					new Func<PXSiteMapNode, bool>[] 
					{ 
						PXSiteMapFilterRuleStorage.IsEmptyFolder, 
						PXSiteMapFilterRuleStorage.IsWiki, 
						PXSiteMapFilterRuleStorage.CantBeAutomated,
						smn =>
						{
							SiteMap sm = PXSiteMapEnumeratorBase.CreateSiteMap(smn);

							if (PXSiteMap.IsFolder(sm.Url)) return true;
							if (PXSiteMapFilterRuleStorage.IsGenericInquiry(smn)) return true;
							if (String.IsNullOrEmpty(sm.Graphtype)) return true;
							Type t = System.Web.Compilation.BuildManager.GetType(sm.Graphtype, false);
							if (typeof(SO.SOInvoiceEntry).IsAssignableFrom(t))
								return true;	
							if (typeof(SO.SOPaymentEntry).IsAssignableFrom(t))
								return true;
							if (typeof(TX.TXInvoiceEntry).IsAssignableFrom(t))
								return true;
							Type baseEditGraph = GetGraphByDocType(row.Module, row.DocType);
							if (baseEditGraph != null && baseEditGraph.IsAssignableFrom(t)) return false;
							return true;
						}
					}));
			return _siteMapEnumerator.SiteMapNodes(NodeID);
		}
		#endregion
		#endregion
		public static Type GetGraphByDocType(string module, string docType)
		{
			Type baseEditGraph = null;
			switch (module)
			{
				case GL.BatchModule.AP:
					{
						switch (docType)
						{
							case AP.APInvoiceType.Invoice:
							case AP.APInvoiceType.DebitAdj:
							case AP.APInvoiceType.CreditAdj:
								baseEditGraph = typeof(AP.APInvoiceEntry); break;
							case AP.APInvoiceType.Check:
							case AP.APInvoiceType.Prepayment:
								baseEditGraph = typeof(AP.APPaymentEntry); break;
							case AP.APInvoiceType.QuickCheck:
								baseEditGraph = typeof(AP.APQuickCheckEntry); break;
						}
						break;
					}
				case GL.BatchModule.AR:
					switch (docType)
					{
						case AR.ARInvoiceType.Invoice:
						case AR.ARInvoiceType.CreditMemo:
						case AR.ARInvoiceType.DebitMemo:
							baseEditGraph = typeof(AR.ARInvoiceEntry); break;
						case AR.ARInvoiceType.Payment:
						case AR.ARInvoiceType.Prepayment:
							baseEditGraph = typeof(AR.ARPaymentEntry); break;
						case AR.ARInvoiceType.CashSale:
							baseEditGraph = typeof(AR.ARCashSaleEntry); break;
					}
					break;
				case GL.BatchModule.CA:
					baseEditGraph = typeof(CA.CATranEntry);
					break;
				case GL.BatchModule.GL:
					baseEditGraph = typeof(GL.JournalEntry);
					break;
			}
			return baseEditGraph;
		}
		public override void Persist()
		{
			Boolean result = _screenToSiteMapAddHelper.Persist(WorkBooks.Current);
			IsSiteMapAltered = result | IsSiteMapAltered;
			base.Persist();
		}
		#region EventHandler
		protected virtual void GLWorkBook_SitemapID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = Guid.NewGuid();
		}
		protected virtual void GLWorkBook_SitemapParent_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<CS.FeaturesSet.gLWorkBooks>())
			{
				SiteMap node = PXSelect<SiteMap, Where<SiteMap.screenID, Equal<Required<SiteMap.screenID>>>>.Select(this, WORKBOOK_FOLDER_SCREEN_ID);
				if (node != null)
				{
					e.NewValue = node.NodeID;					
				}
			}
		}
		protected virtual void GLWorkBook_SitemapParent_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			_screenToSiteMapAddHelper.UpdateSiteMapParent(e.Row as GLWorkBook);
		}

		protected virtual void GLWorkBook_Module_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			GLWorkBook row = (GLWorkBook)e.Row;
			if (row == null || row.Module == (string)e.OldValue)
				return;
			cache.SetValueExt<GLWorkBook.defaultBAccountID>(row, null);
			cache.SetValueExt<GLWorkBook.defaultLocationID>(row, null);
			cache.SetValueExt<GLWorkBook.defaultCashAccountID>(row, null);
			cache.SetValueExt<GLWorkBook.defaultEntryTypeID>(row, null);
			FillSequences();
		}
		protected virtual void GLWorkBook_DocType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			GLWorkBook row = (GLWorkBook)e.Row;
			if (row == null || row.DocType == (string)e.OldValue)
				return;
			FillSequences();
		}
		protected virtual void GLWorkBook_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
		{
			_screenToSiteMapAddHelper.InitializeSiteMapFields(e.Row as GLWorkBook);
		}

		public virtual void GLWorkBook_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GLWorkBook row = (GLWorkBook)e.Row;
			if (row != null)
			{
				bool isSupported = IsSupported(row);
				if (!isSupported)
				{
					PXErrorLevel level = PXErrorLevel.Error;
					sender.RaiseExceptionHandling<GLWorkBook.docType>(row, row.DocType, new PXSetPropertyException(Messages.DocumentTypeIsNotSupportedYet, level));
				}
				bool moduleAP = row.Module == BatchModule.AP;
				bool moduleAR = row.Module == BatchModule.AR;
				bool moduleCA = row.Module == BatchModule.CA;
				PXUIFieldAttribute.SetVisible<GLWorkBook.defaultVendorID>(sender, row, moduleAP);
				PXUIFieldAttribute.SetVisible<GLWorkBook.defaultCustomerID>(sender, row, moduleAR);
				PXUIFieldAttribute.SetVisible<GLWorkBook.defaultLocationID>(sender, row, moduleAP || moduleAR);
				PXUIFieldAttribute.SetVisible<GLWorkBook.defaultCashAccountID>(sender, row, moduleCA);
				PXUIFieldAttribute.SetVisible<GLWorkBook.defaultEntryTypeID>(sender, row, moduleCA);

				PXUIFieldAttribute.SetEnabled<GLWorkBook.defaultVendorID>(sender, row, moduleAP);
				PXUIFieldAttribute.SetEnabled<GLWorkBook.defaultCustomerID>(sender, row, moduleAR);
				PXUIFieldAttribute.SetEnabled<GLWorkBook.defaultLocationID>(sender, row, moduleAP || moduleAR);
				PXUIFieldAttribute.SetEnabled<GLWorkBook.defaultCashAccountID>(sender, row, moduleCA);
				PXUIFieldAttribute.SetEnabled<GLWorkBook.defaultEntryTypeID>(sender, row, moduleCA);

				bool haveBatches = batches.SelectSingle() != null;
				WorkBooks.Cache.AllowDelete = !haveBatches;
				PXUIFieldAttribute.SetEnabled<GLWorkBook.module>(sender, row, !haveBatches);
				PXUIFieldAttribute.SetEnabled<GLWorkBook.docType>(sender, row, !haveBatches);

				CheckNumbering(sender, row);
			}
		}

		public virtual void GLWorkBook_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			GLWorkBook row = e.Row as GLWorkBook;
			if (row != null)
			{

				if (!IsSupported(row))
				{
					sender.RaiseExceptionHandling<GLWorkBook.docType>(row, row.DocType, new PXSetPropertyException(Messages.DocumentTypeIsNotSupportedYet, PXErrorLevel.Error));
				}
				Boolean result = _screenToSiteMapAddHelper.DeleteObsoleteSiteMapNode(row, e.Operation);
				IsSiteMapAltered = result ? result : IsSiteMapAltered;
				PXPageCacheUtils.InvalidateCachedPages();
			}
		}

		public virtual void GLWorkBook_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (e.Row == null) return;
			PXSelectorAttribute.CheckAndRaiseForeignKeyException(sender, e.Row, typeof(GLVoucherBatch.workBookID));
		}
		
		#endregion
		protected static bool IsSupported(GLWorkBook row)
		{
			bool isSupported = true;
			if ((row.Module == GL.BatchModule.AP &&
					(row.DocType == AP.APPaymentType.Refund ||
				//row.DocType == AP.APPaymentType.Check ||
						row.DocType == AP.APPaymentType.VoidCheck ||
						row.DocType == AP.APPaymentType.VoidQuickCheck))
				|| (row.Module == GL.BatchModule.AR
					&& (row.DocType == AR.ARPaymentType.Refund ||
						row.DocType == AR.ARPaymentType.FinCharge ||
						row.DocType == AR.ARPaymentType.SmallBalanceWO ||
						row.DocType == AR.ARPaymentType.SmallCreditWO ||
						row.DocType == AR.ARPaymentType.NoUpdate ||
						row.DocType == AR.ARPaymentType.Undefined ||
						row.DocType == AR.ARPaymentType.VoidPayment ||
						row.DocType == AR.ARPaymentType.CashReturn))
				|| (row.Module == GL.BatchModule.CA
					&& (row.DocType == CA.CATranType.CAAdjustmentRGOL ||
						row.DocType == CA.CATranType.CADeposit ||
						row.DocType == CA.CATranType.CAVoidDeposit ||
						row.DocType == CA.CATranType.CATransferExp ||
						row.DocType == CA.CATranType.CATransferOut ||
						row.DocType == CA.CATranType.CATransferIn)))
			{
				isSupported = false;
			}
			return isSupported;
		}
	}
}

