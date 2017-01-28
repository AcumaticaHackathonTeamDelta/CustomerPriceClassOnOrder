using System;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects.BQLConstants;
using System.Linq;

namespace PX.Objects.RUTROT
{
	public class SOInvoiceEntryRUTROT : PXGraphExtension<SOInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>();
		}
		[PXRemoveBaseAttribute(typeof(PXUnboundFormulaAttribute))]
		protected virtual void SOTax_CuryRUTROTTaxAmt_CacheAttached(PXCache sender)
		{ }
		public delegate void InvoiceOrderDelegate(DateTime invoiceDate, PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType> order, PXResultset<SOShipLine, SOLine> details, Customer customer, DocumentList<ARInvoice, SOInvoice> list);
		[PXOverride]
		public virtual void InvoiceOrder(DateTime invoiceDate, PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType> order, PXResultset<SOShipLine, SOLine> details, Customer customer, DocumentList<ARInvoice, SOInvoice> list, InvoiceOrderDelegate baseMethod)
		{
			baseMethod(invoiceDate, order, details, customer, list);
			SOOrder soorder = (SOOrder)order;
			SOOrderRUTROT orderRR = PXCache<SOOrder>.GetExtension<SOOrderRUTROT>(soorder);
			if (orderRR?.IsRUTROTDeductible == true && Base.Document.Current != null)
			{
				Base.Document.SetValueExt<ARInvoiceRUTROT.isRUTROTDeductible>(Base.Document.Current, true);
				Base.Document.Update(Base.Document.Current);
			
				RUTROT rutrot = PXSelect<RUTROT, Where<RUTROT.docType, Equal<Required<SOOrder.orderType>>,
					And<RUTROT.refNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, soorder.OrderType, soorder.OrderNbr);
				rutrot = (RUTROT)Base.Rutrots.Cache.CreateCopy(rutrot);
				rutrot.DocType = Base.Document.Current.DocType;
				rutrot.RefNbr = Base.Document.Current.RefNbr;
				rutrot.CuryDistributedAmt = 0m;
				rutrot.CuryUndistributedAmt = 0m;
				rutrot = Base.Rutrots.Update(rutrot);
				RecalcFormulas(rutrot, PXCache<ARInvoice>.GetExtension<ARInvoiceRUTROT>(Base.Document.Current));
				foreach (RUTROTDistribution rutrotDetail in PXSelect<RUTROTDistribution, Where<RUTROTDistribution.docType, Equal<Required<SOOrder.orderType>>,
					And<RUTROTDistribution.refNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, soorder.OrderType, soorder.OrderNbr))
				{
					RUTROTDistribution new_detail = (RUTROTDistribution)Base.RRDistribution.Cache.CreateCopy(rutrotDetail);
					new_detail.RefNbr = null;
					new_detail.DocType = null;
					Base.RRDistribution.Insert(new_detail);
				}

				Base.Save.Press();
			}
		}
		public delegate ARTran CreateTranFromMiscLineDelegate(SOOrderShipment orderShipment, SOMiscLine2 orderline);
		[PXOverride]
		public virtual ARTran CreateTranFromMiscLine(SOOrderShipment orderShipment, SOMiscLine2 orderline, CreateTranFromMiscLineDelegate baseMethod)
		{
			ARTran tran = baseMethod(orderShipment, orderline);
			SOLine line = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
			And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
			And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>.Select(Base, orderline.OrderType, orderline.OrderNbr, orderline.LineNbr);
			SOLineRUTROT lineRR = PXCache<SOLine>.GetExtension<SOLineRUTROT>(line);
			ARTranRUTROT tranRR = PXCache<ARTran>.GetExtension<ARTranRUTROT>(tran);
			tranRR.IsRUTROTDeductible = lineRR.IsRUTROTDeductible;
			tranRR.RUTROTItemType = lineRR.RUTROTItemType;
			tranRR.RUTROTWorkTypeID = lineRR.RUTROTWorkTypeID;
			return tran;
		}
		public delegate ARTran CreateTranFromShipLineDelegate(ARInvoice newdoc, SOOrderType ordertype, string operation, SOLine orderline, ref SOShipLine shipline);
		[PXOverride]
		public virtual ARTran CreateTranFromShipLine(ARInvoice newdoc, SOOrderType ordertype, string operation, SOLine orderline, ref SOShipLine shipline, CreateTranFromShipLineDelegate baseMethod)
		{
			ARTran tran = baseMethod(newdoc, ordertype, operation, orderline, ref shipline);
			SOLine line = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
			And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
			And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>.Select(Base, orderline.OrderType, orderline.OrderNbr, orderline.LineNbr);
			SOLineRUTROT lineRR = PXCache<SOLine>.GetExtension<SOLineRUTROT>(line);
			ARTranRUTROT tranRR = PXCache<ARTran>.GetExtension<ARTranRUTROT>(tran);
			tranRR.IsRUTROTDeductible = lineRR.IsRUTROTDeductible;
			tranRR.RUTROTItemType = lineRR.RUTROTItemType;
			tranRR.RUTROTWorkTypeID = lineRR.RUTROTWorkTypeID;
			return tran;
		}

		public void RecalcFormulas(RUTROT rowRR, ARInvoiceRUTROT row)
		{
			if (row.IsRUTROTDeductible == true)
			{
				foreach (ARTran tran in Base.Transactions.Select())
				{
					if (PXCache<ARTran>.GetExtension<ARTranRUTROT>(tran).IsRUTROTDeductible == true)
					{
						Base.Transactions.Cache.RaiseFieldUpdated<ARTranRUTROT.isRUTROTDeductible>(tran, false);//this is required for updating formula on ARTranRUTROT.CuryRUTROTAvailableAmt
						Base.Transactions.Update(tran);
					}
				}
				PXUnboundFormulaAttribute.CalcAggregate<ARTranRUTROT.curyRUTROTTotal>(Base.Transactions.Cache, rowRR);
				PXFormulaAttribute.CalcAggregate<ARTranRUTROT.curyRUTROTAvailableAmt>(Base.Transactions.Cache, rowRR);
			}
		}
	}
}
