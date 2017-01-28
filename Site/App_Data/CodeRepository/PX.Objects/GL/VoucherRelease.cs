using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.CM;


namespace PX.Objects.GL
{
    [PX.Objects.GL.TableAndChartDashboardType]
    public class VoucherRelease : PXGraph<VoucherRelease>
    {
        public PXCancel<GLDocBatch> Cancel;

        public PXAction<GLDocBatch> EditDetail;

        [PXFilterable]
        public PXProcessing<GLDocBatch, Where<GLDocBatch.hold, Equal<boolFalse>,
                                        And<GLDocBatch.released, Equal<boolFalse>>>> Documents;



        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton]
        public virtual IEnumerable editDetail(PXAdapter adapter)
        {
            if (this.Documents.Current != null)
            {
                JournalWithSubEntry graph = PXGraph.CreateInstance<JournalWithSubEntry>();
                graph.BatchModule.Current = graph.BatchModule.Search<GLDocBatch.batchNbr>(this.Documents.Current.BatchNbr, this.Documents.Current.Module);
                if (graph.BatchModule.Current != null)
                {
                    throw new PXRedirectRequiredException(graph, true, "ViewBatch") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                }
            }
            return adapter.Get();
        }



        public static void ReleaseVoucher(GLBatchDocRelease graph, GLDocBatch batch)
        {
            graph.ReleaseBatchProc(batch, false);
        }

        public VoucherRelease()
        {
            Documents.SetProcessDelegate<GLBatchDocRelease>(ReleaseVoucher);
            Documents.SetProcessCaption(Messages.ProcRelease);
            Documents.SetProcessAllCaption(Messages.ProcReleaseAll);
        }
    }
}
