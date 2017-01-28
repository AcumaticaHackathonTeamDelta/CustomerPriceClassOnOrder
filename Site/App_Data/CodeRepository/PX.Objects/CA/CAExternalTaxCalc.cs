using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.AP;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;

using System.Web.Compilation;
using System.Web;
using System.Text;
using PX.Objects.SO;


namespace PX.Objects.CA
{
	public class CAExternalTaxCalc : PXGraph<CAExternalTaxCalc>
	{
		[PXFilterable]
		public PXProcessingJoin<CAAdj,
		InnerJoin<TX.TaxZone, On<TX.TaxZone.taxZoneID, Equal<CAAdj.taxZoneID>>>,
		Where<TX.TaxZone.isExternal, Equal<True>,
			And<CAAdj.isTaxValid, Equal<False>,
			And<CAAdj.released, Equal<False>>>>> Items;

		public CAExternalTaxCalc()
		{
			Items.SetProcessDelegate(
				delegate(List<CAAdj> list)
				{
					List<CAAdj> newlist = new List<CAAdj>(list.Count);
					foreach (CAAdj doc in list)
					{
						newlist.Add(doc);
					}
					Process(newlist, true);
				}
			);

		}

		public static CAAdj Process(CAAdj doc)
		{
			List<CAAdj> list = new List<CAAdj>();

			list.Add(doc);

			List<CAAdj> listWithTax = Process(list, false);

			return listWithTax[0];
		}

		public static List<CAAdj> Process(List<CAAdj> list, bool isMassProcess)
        {
            List<CAAdj> listWithTax = new List<CAAdj>(list.Count);
            CATranEntry rg = PXGraph.CreateInstance<CATranEntry>();
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    rg.Clear();
                    rg.CAAdjRecords.Current = PXSelect<CAAdj, Where<CAAdj.adjRefNbr, Equal<Required<CAAdj.adjRefNbr>>>>.Select(rg, list[i].AdjRefNbr);
                    listWithTax.Add(rg.CalculateAvalaraTax(rg.CAAdjRecords.Current));
                    PXProcessing<CAAdj>.SetInfo(i, ActionsMessages.RecordProcessed);
                }
                catch (Exception e)
                {
                    if (isMassProcess)
                    {
                        PXProcessing<CAAdj>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
                    }
                    else
                    {
                        throw new PXMassProcessException(i, e);
                    }
                }

            }

            return listWithTax;
        }

	}


}

