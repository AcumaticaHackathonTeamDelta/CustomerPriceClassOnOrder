using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL.Overrides.PostGraph;

namespace PX.Objects.GL
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class GLHistoryValidate : PXGraph<GLHistoryValidate>
	{
		public GLHistoryValidate()
		{
			GLSetup setup = glsetup.Current;

			LedgerList.SetProcessDelegate<PostGraph>(Validate);

			LedgerList.SetProcessCaption(Messages.ProcValidate);
			LedgerList.SetProcessAllCaption(Messages.ProcValidateAll);

			PXUIFieldAttribute.SetEnabled<Ledger.selected>(LedgerList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<Ledger.ledgerCD>(LedgerList.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<Ledger.descr>(LedgerList.Cache, null, false);
		}

		public PXCancel<Ledger> Cancel;

		[PXFilterable]
		public PXProcessing<Ledger, Where<Ledger.balanceType, Equal<LedgerBalanceType.actual>, Or<Ledger.balanceType,Equal<LedgerBalanceType.report>>>> LedgerList;
		public PXSetup<GLSetup> glsetup;

		private static void Validate(PostGraph graph, Ledger ledger)
		{
            while (RunningFlagScope<PostGraph>.IsRunning) { }
            using (new RunningFlagScope<GLHistoryValidate>())
            {
                graph.Clear();
                graph.IntegrityCheckProc(ledger);
            }
		}
	}
}
