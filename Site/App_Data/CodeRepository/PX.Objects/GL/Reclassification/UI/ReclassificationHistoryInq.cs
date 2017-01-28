using System;
using System.Collections;
using System.Linq;
using PX.Api;
using PX.Data;

namespace PX.Objects.GL.Reclassification.UI
{
	public class ReclassificationHistoryInq : PXGraph<ReclassificationHistoryInq>
	{
		public PXAction<GLTran> reclassify;
		public PXAction<GLTran> viewBatch;

		public PXSelectOrderBy<GLTran,
								OrderBy<Asc<GLTran.reclassSeqNbr,
										Asc<GLTran.lineNbr>>>> TransView;

		public PXSelect<GLTranKey> SrcOfReclassTranView;

		public ReclassificationHistoryInq()
		{
			TransView.AllowDelete = false;
			TransView.AllowInsert = false;
			TransView.AllowUpdate = false;

			PXUIFieldAttribute.SetVisible<GLTran.batchNbr>(TransView.Cache, null);

			//to hide the red asterisk on column header
			PXDefaultAttribute.SetPersistingCheck<GLTran.branchID>(TransView.Cache, null, PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<GLTran.accountID>(TransView.Cache, null, PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<GLTran.subID>(TransView.Cache, null, PXPersistingCheck.Nothing);
		}

		protected GLTranKey SrcTranOfReclassKey()
		{
			return SrcOfReclassTranView.Cache.Inserted.Select<GLTranKey>().SingleOrDefault(); 
		}

		protected virtual IEnumerable transView()
		{
			var srcTranKey = SrcTranOfReclassKey();

			if (srcTranKey == null)
				return new GLTran[0];

			var srcTran = (GLTran)PXSelect<GLTran,
								Where<GLTran.module, Equal<Required<GLTran.module>>,
										And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
										And<GLTran.lineNbr, Equal<Required<GLTran.lineNbr>>>>>>
								.Select(this,
									srcTranKey.Module,
									srcTranKey.BatchNbr,
									srcTranKey.LineNbr);

			var releasedReversingBatches = JournalEntry.GetReleasedReversingBatches(this, srcTran.Module, srcTran.BatchNbr);

			if (releasedReversingBatches.Any())
			{
				TransView.Cache.RaiseExceptionHandling<GLTran.batchNbr>(srcTran, null,
					new PXSetPropertyException(Messages.BatchOfTranHasBeenReversed, PXErrorLevel.RowWarning));
			}

			var reclassTrans = PXSelect<GLTran,
										   Where<GLTran.reclassSourceTranModule, Equal<Required<GLTran.reclassSourceTranModule>>,
													   And<GLTran.reclassSourceTranBatchNbr, Equal<Required<GLTran.reclassSourceTranBatchNbr>>,
													   And<GLTran.reclassSourceTranLineNbr, Equal<Required<GLTran.reclassSourceTranLineNbr>>>>>,
											OrderBy<Asc<GLTran.reclassSeqNbr>>>
										   .Select(this,
												   srcTranKey.Module,
												   srcTranKey.BatchNbr,
												   srcTranKey.LineNbr)
										   .RowCast<GLTran>();

			var sortedTrans = new[] { srcTran }.Union(reclassTrans).ToArray();

			var lastTran = sortedTrans.Last();

			if (lastTran.Released == false)
			{
				TransView.Cache.RaiseExceptionHandling<GLTran.batchNbr>(lastTran, null,
					new PXSetPropertyException(Messages.TheTransactionCannotBeReclassifiedBecauseItIsNotReleased, PXErrorLevel.RowWarning));
			}

			return sortedTrans;
		}

		public override bool IsDirty
		{
			get { return false; }
		}

		[PXUIField(DisplayName = Messages.Reclassify, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable Reclassify(PXAdapter adapter)
		{
			var tran = TransView.Select().RowCast<GLTran>().LastOrDefault();

			if (tran == null)
				return new GLTran[0];

			if (tran.Released == false)
				throw new PXException(Messages.TheTransactionCannotBeReclassifiedBecauseItIsNotReleased);

			ReclassifyTransactionsProcess.OpenForReclassification(new[] { tran }, PXBaseRedirectException.WindowMode.Same);

			return adapter.Get();
		}

		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			var tran = TransView.Current;

			if (tran != null)
			{
				JournalEntry.RedirectToBatch(this, tran.Module, tran.BatchNbr);
			}

			return adapter.Get();
		}

		public static void OpenForTransaction(GLTran tran)
		{
			if (tran == null)
				throw new ArgumentNullException("tran");

			var graph = PXGraph.CreateInstance<ReclassificationHistoryInq>();

			var srcTrankey = graph.GetSrcTranOfReclassKey(tran);

			graph.SrcOfReclassTranView.Insert(srcTrankey);

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				Mode = PXBaseRedirectException.WindowMode.NewWindow
			};
		}

		private GLTranKey GetSrcTranOfReclassKey(GLTran tran)
		{
			if (JournalEntry.IsReclassifacationTran(tran))
			{
				return new GLTranKey()
				{
					Module = tran.ReclassSourceTranModule,
					BatchNbr = tran.ReclassSourceTranBatchNbr,
					LineNbr = tran.ReclassSourceTranLineNbr
				};
			}
			else
			{
				return new GLTranKey()
				{
					Module = tran.Module,
					BatchNbr = tran.BatchNbr,
					LineNbr = tran.LineNbr
				};
			}
		}
	}
}
