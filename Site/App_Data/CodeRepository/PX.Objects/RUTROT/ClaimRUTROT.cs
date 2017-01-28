using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Data;
using PX.Objects.AR;

namespace PX.Objects.RUTROT
{
    public class ClaimRUTROT : PXGraph<ClaimRUTROT>
    {
        public PXCancel<ClaimRUTROTFilter> Cancel;
        public PXFilter<ClaimRUTROTFilter> Filter;

        [PXFilterable]
        public PXFilteredProcessingJoin<ARInvoice, ClaimRUTROTFilter, 
			InnerJoin<RUTROT, On<RUTROT.docType, Equal<ARInvoice.docType>, And<RUTROT.refNbr, Equal<ARInvoice.refNbr>>>>> Documents;

        protected virtual IEnumerable documents()
        {
            if (Filter.Current != null)
            {
                var items = new List<ARInvoice>();

                var select = new PXSelectJoin<ARInvoice,
                                InnerJoin<ARAdjust, On<ARInvoice.docType, Equal<ARAdjust.adjdDocType>, And<ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>,
								InnerJoin<RUTROT,On<RUTROT.docType, Equal<ARInvoice.docType>, And<RUTROT.refNbr, Equal<ARInvoice.refNbr>>>,
                                InnerJoin<ARPayment, On<ARPayment.docType, Equal<ARAdjust.adjgDocType>, And<ARPayment.refNbr, Equal<ARAdjust.adjgRefNbr>>>>>>,
                                Where2<Where<RUTROT.rUTROTType, Equal<Current<ClaimRUTROTFilter.rUTROTType>>,
                                         And<ARInvoice.released, Equal<True>,
                                         And<ARInvoice.openDoc, Equal<True>,
                                         And<ARInvoiceRUTROT.isRUTROTDeductible, Equal<True>>>>>,
                                        And<Where2<Where<Current<ClaimRUTROTFilter.action>, Equal<ClaimActions.claim>,
                                                    And<ARPayment.released, Equal<True>,
                                                    And<RUTROT.isClaimed, NotEqual<True>>>>,
                                            Or<Where<Current<ClaimRUTROTFilter.action>, Equal<ClaimActions.export>,
                                                    And<RUTROT.isClaimed, Equal<True>>>>>>>>(this);

                foreach (var r in select.Select())
                {
                    var doc = (ARInvoice)r;
                    if (items.FirstOrDefault(d => d.RefNbr == doc.RefNbr && d.DocType == doc.DocType) == null)
                    {
                        items.Add(doc);
                    }
                }

                foreach (var doc in items)
                {
                    yield return doc;
                }
            }
        }

        public static void ExportDocs(List<ARInvoice> documents)
        {
            var processing = PXGraph.CreateInstance<ClaimRUTROTProcess>();
            processing.ExportDocuments(documents);
        }

        public static void ClaimDocs(List<ARInvoice> documents)
        {
            var processing = PXGraph.CreateInstance<ClaimRUTROTProcess>();
            processing.ClaimDocuments(documents);
        }

        public virtual void ClaimRUTROTFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var filter = (ClaimRUTROTFilter)e.Row;

            if (filter != null && !String.IsNullOrEmpty(filter.Action))
            {
                if (filter.Action == ClaimActions.Export)
                {
                    PXUIFieldAttribute.SetVisible(Documents.Cache, "RUTROTClaimDate", true);
                    PXUIFieldAttribute.SetVisible(Documents.Cache, "RUTROTExportRefNbr", true);
                    Documents.SetProcessDelegate(ExportDocs);
                }
                else if (filter.Action == ClaimActions.Claim)
                {
                    PXUIFieldAttribute.SetVisible(Documents.Cache, "RUTROTClaimDate", false);
                    PXUIFieldAttribute.SetVisible(Documents.Cache, "RUTROTExportRefNbr", false);
                    Documents.SetProcessDelegate(ClaimDocs);
                }
            }
        }

        public virtual void ClaimRUTROTFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            this.Documents.Cache.Clear();
        }
    }

}
