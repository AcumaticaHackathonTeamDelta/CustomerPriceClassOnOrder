using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;

using System.Web.Compilation;
using System.Web;
using System.Text;
using PX.Objects.SO;


namespace PX.Objects.AP
{
	public class APExternalTaxCalc : PXGraph<APExternalTaxCalc>
	{
		[PXFilterable]
		public PXProcessingJoin<APInvoice,
		InnerJoin<TX.TaxZone, On<TX.TaxZone.taxZoneID, Equal<APInvoice.taxZoneID>>>,
		Where<TX.TaxZone.isExternal, Equal<True>,
			And<APInvoice.isTaxValid, Equal<False>,
			And<APInvoice.released, Equal<False>>>>> Items;

		public APExternalTaxCalc()
		{
			Items.SetProcessDelegate(
				delegate(List<APInvoice> list)
				{
					List<APInvoice> newlist = new List<APInvoice>(list.Count);
					foreach (APInvoice doc in list)
					{
						newlist.Add(doc);
					}
					Process(newlist, true);
				}
			);

		}

		public static APInvoice Process(APInvoice doc)
		{
			List<APInvoice> list = new List<APInvoice>();

			list.Add(doc);

			List<APInvoice> listWithTax = Process(list, false);

			return listWithTax[0];
		}

		public static List<APInvoice> Process(List<APInvoice> list, bool isMassProcess)
		{
			List<APInvoice> listWithTax = new List<APInvoice>(list.Count);
			APInvoiceEntry rg = PXGraph.CreateInstance<APInvoiceEntry>();
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					rg.Clear();
					rg.Document.Current = PXSelect<APInvoice, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>, And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(rg, list[i].DocType, list[i].RefNbr);
					listWithTax.Add(rg.CalculateAvalaraTax(rg.Document.Current));
					PXProcessing<APInvoice>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						PXProcessing<APInvoice>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
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

