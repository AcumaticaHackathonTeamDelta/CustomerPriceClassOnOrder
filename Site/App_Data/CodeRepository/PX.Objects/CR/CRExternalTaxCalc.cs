using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.AddressService;
using Avalara.AvaTax.Adapter.TaxService;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using PXMassProcessException = PX.Objects.AR.PXMassProcessException;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using Branch = PX.Objects.GL.Branch;

namespace PX.Objects.CR
{
	public class CRExternalTaxCalc : PXGraph<CRExternalTaxCalc>
	{
		[PXFilterable]
		public PXProcessingJoin<CROpportunity,
		InnerJoin<TX.TaxZone, On<TX.TaxZone.taxZoneID, Equal<CROpportunity.taxZoneID>>>,
		Where<TX.TaxZone.isExternal, Equal<True>,
			And<CROpportunity.isTaxValid, Equal<False>>>> Items;

		public CRExternalTaxCalc()
		{
			Items.SetProcessDelegate(
				delegate(List<CROpportunity> list)
				{
					List<CROpportunity> newlist = new List<CROpportunity>(list.Count);
					foreach (CROpportunity doc in list)
					{
						newlist.Add(doc);
					}
					Process(newlist, true);
				}
			);

		}

		public static void Process(CROpportunity doc)
		{
			List<CROpportunity> list = new List<CROpportunity>();

			list.Add(doc);
			Process(list, false);
		}

		public static void Process(List<CROpportunity> list, bool isMassProcess)
		{
			OpportunityMaint rg = PXGraph.CreateInstance<OpportunityMaint>();
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					rg.Clear();
					rg.Opportunity.Current = PXSelect<CROpportunity, Where<CROpportunity.opportunityID, Equal<Required<CROpportunity.opportunityID>>>>.Select(rg, list[i].OpportunityID);
					CalculateAvalaraTax(rg, rg.Opportunity.Current);
					PXProcessing<CROpportunity>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						PXProcessing<CROpportunity>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}

			}
		}

		public static void CalculateAvalaraTax(OpportunityMaint rg, CROpportunity order)
		{
			TaxSvc service = new TaxSvc();
			AvalaraMaint.SetupService(rg, service);

			AddressSvc addressService = new AddressSvc();
			AvalaraMaint.SetupService(rg, addressService);

			GetTaxRequest getRequest = null;
			bool isValidByDefault = true;

			if (order.IsTaxValid != true)
			{
				getRequest = BuildGetTaxRequest(rg, order);

				if (getRequest.Lines.Count > 0)
				{
					isValidByDefault = false;
				}
				else
				{
					getRequest = null;
				}
			}

			if (isValidByDefault)
			{
				PXDatabase.Update<CROpportunity>(
					new PXDataFieldAssign("IsTaxValid", true),
					new PXDataFieldRestrict("OpportunityID", PXDbType.NVarChar, CROpportunity.OpportunityIDLength, order.OpportunityID, PXComp.EQ)
					);
				return;
			}

			GetTaxResult result = service.GetTax(getRequest);
			if (result.ResultCode == SeverityLevel.Success)
			{
				try
				{
					ApplyAvalaraTax(rg, order, result);
					PXDatabase.Update<CROpportunity>(
						new PXDataFieldAssign("IsTaxValid", true),
						new PXDataFieldRestrict("OpportunityID", PXDbType.NVarChar, CROpportunity.OpportunityIDLength, order.OpportunityID, PXComp.EQ)
						);
				}
				catch (PXOuterException ex)
				{
					string msg = TX.Messages.FailedToApplyTaxes;
					foreach (string err in ex.InnerMessages)
					{
						msg += Environment.NewLine + err;
					}

					throw new PXException(ex, msg);
				}
				catch (Exception ex)
				{
					throw new PXException(ex, TX.Messages.FailedToApplyTaxes);
				}
			}
			else
			{
				LogMessages(result);

				throw new PXException(TX.Messages.FailedToGetTaxes);
			}
		}

		protected static GetTaxRequest BuildGetTaxRequest(OpportunityMaint rg, CROpportunity order)
		{
			if (order == null)
				throw new PXArgumentException(ErrorMessages.ArgumentNullException);

			BAccount cust = (BAccount)PXSelect<BAccount,
				Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(rg, order.BAccountID);
			Location loc = (Location)PXSelect<Location,
				Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.
				Select(rg, order.BAccountID, order.LocationID);

			IAddressBase fromAddress = GetFromAddress(rg);
			IAddressBase toAddress = GetToAddress(rg, order);

			if (fromAddress == null)
				throw new PXException(Messages.FailedGetFromAddressSO);

			if (toAddress == null)
				throw new PXException(Messages.FailedGetToAddressSO);

			var avalaraSetup = (TXAvalaraSetup)PXSetupOptional<TXAvalaraSetup>.Select(rg);
				
			GetTaxRequest request = new GetTaxRequest();
			request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(rg, rg.Accessinfo.BranchID);
			request.CurrencyCode = order.CuryID;
			request.CustomerCode = cust.AcctCD;
			request.OriginAddress = AvalaraMaint.FromAddress(fromAddress);
			request.DestinationAddress = AvalaraMaint.FromAddress(toAddress);
			request.DetailLevel = DetailLevel.Summary;
			request.DocCode = string.Format("CR.{0}", order.OpportunityID);
			request.DocDate = order.CloseDate.GetValueOrDefault();

			int mult = 1;

			if (!string.IsNullOrEmpty(loc.CAvalaraCustomerUsageType))
			{
				request.CustomerUsageType = loc.CAvalaraCustomerUsageType;
			}
			if (!string.IsNullOrEmpty(loc.CAvalaraExemptionNumber))
			{
				request.ExemptionNo = loc.CAvalaraExemptionNumber;
			}

			request.DocType = DocumentType.SalesOrder;

			PXSelectBase<CROpportunityProducts> select = new PXSelectJoin<CROpportunityProducts,
				LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<CROpportunityProducts.inventoryID>>,
					LeftJoin<Account, On<Account.accountID, Equal<InventoryItem.salesAcctID>>>>,
				Where<CROpportunityProducts.cROpportunityID, Equal<Current<CROpportunity.opportunityID>>>,
				OrderBy<Asc<CROpportunityProducts.cROpportunityProductID>>>(rg);

			//request.Discount = order.CuryDiscTot.GetValueOrDefault();

			foreach (PXResult<CROpportunityProducts, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				CROpportunityProducts tran = (CROpportunityProducts)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				Line line = new Line();
				line.No = Convert.ToString(tran.CROpportunityProductID);
				line.Amount = mult * tran.CuryAmount.GetValueOrDefault();
				line.Description = tran.TransactionDescription;
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = item.InventoryCD;
				line.Qty = Math.Abs(Convert.ToDouble(tran.Qty.GetValueOrDefault()));
				line.Discounted = request.Discount > 0;
				line.TaxIncluded = avalaraSetup.IsInclusiveTax == true;

				if (avalaraSetup != null && avalaraSetup.SendRevenueAccount == true)
					line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;

				request.Lines.Add(line);
			}

			return request;
		}

		protected static void ApplyAvalaraTax(OpportunityMaint rg, CROpportunity order, GetTaxResult result)
		{
			var avalaraSetup = (TXAvalaraSetup)PXSetupOptional<TXAvalaraSetup>.Select(rg);
			
			TaxZone taxZone = (TaxZone)PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<CROpportunity.taxZoneID>>>>.Select(rg, order.TaxZoneID);
			AP.Vendor vendor = PXSelect<AP.Vendor, Where<AP.Vendor.bAccountID, Equal<Required<AP.Vendor.bAccountID>>>>.Select(rg, taxZone.TaxVendorID);

			if (vendor == null)
				throw new PXException(Messages.ExternalTaxVendorNotFound);

			//Clear all existing Tax transactions:
			foreach (PXResult<CRTaxTran, Tax> res in rg.Taxes.View.SelectMultiBound(new object[] { order }))
			{
				CRTaxTran taxTran = (CRTaxTran)res;
				rg.Taxes.Delete(taxTran);
			}

			rg.Views.Caches.Add(typeof(Tax));

			for (int i = 0; i < result.TaxSummary.Count; i++)
			{
                string taxID = AvalaraMaint.GetTaxID(result.TaxSummary[i]);
				
				//Insert Tax if not exists - just for the selectors sake
				Tax tx = PXSelect<Tax, Where<Tax.taxID, Equal<Required<Tax.taxID>>>>.Select(rg, taxID);
				if (tx == null)
				{
					tx = new Tax();
					tx.TaxID = taxID;
					//tx.Descr = string.Format("Avalara {0} {1}%", taxID, Convert.ToDecimal(result.TaxSummary[i].Rate)*100);
					tx.Descr = string.Format("Avalara {0}", taxID);
					tx.TaxType = CSTaxType.Sales;
					tx.TaxCalcType = CSTaxCalcType.Doc;
					tx.TaxCalcLevel = avalaraSetup.IsInclusiveTax == true ? CSTaxCalcLevel.Inclusive : CSTaxCalcLevel.CalcOnItemAmt;
					tx.TaxApplyTermsDisc = CSTaxTermsDiscount.ToTaxableAmount;
					tx.SalesTaxAcctID = vendor.SalesTaxAcctID;
					tx.SalesTaxSubID = vendor.SalesTaxSubID;
					tx.ExpenseAccountID = vendor.TaxExpenseAcctID;
					tx.ExpenseSubID = vendor.TaxExpenseSubID;
					tx.TaxVendorID = taxZone.TaxVendorID;
					tx.IsExternal = true;

					rg.Caches[typeof(Tax)].Insert(tx);
				}

				CRTaxTran tax = new CRTaxTran();
				tax.OpportunityID = order.OpportunityID;
				tax.TaxID = taxID;
				tax.CuryTaxAmt = Math.Abs(result.TaxSummary[i].Tax);
				tax.CuryTaxableAmt = Math.Abs(result.TaxSummary[i].Taxable);
				tax.TaxRate = Convert.ToDecimal(result.TaxSummary[i].Rate) * 100;

				rg.Taxes.Insert(tax);
			}

			rg.Opportunity.SetValueExt<CROpportunity.curyTaxTotal>(order, Math.Abs(result.TotalTax));

			try
			{
				rg.SkipAvalaraTaxProcessing = true;
				rg.Save.Press();
			}
			finally
			{
				rg.SkipAvalaraTaxProcessing = false;
			}
		}

		protected static void LogMessages(BaseResult result)
		{
			foreach (AvaMessage msg in result.Messages)
			{
				switch (result.ResultCode)
				{
					case SeverityLevel.Exception:
					case SeverityLevel.Error:
						PXTrace.WriteError(msg.Summary + ": " + msg.Details);
						break;
					case SeverityLevel.Warning:
						PXTrace.WriteWarning(msg.Summary + ": " + msg.Details);
						break;
				}
			}
		}

		protected static IAddressBase GetFromAddress(OpportunityMaint rg)
		{
			PXSelectBase<Branch> select = new PXSelectJoin
				<Branch, InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Branch.bAccountID>>,
					InnerJoin<Address, On<Address.addressID, Equal<BAccount.defAddressID>>>>,
					Where<Branch.branchID, Equal<Required<Branch.branchID>>>>(rg);

			foreach (PXResult<Branch, BAccount, Address> res in select.Select(rg.Accessinfo.BranchID))
				return (Address)res;

			return null;
		}



		protected static IAddressBase GetToAddress(OpportunityMaint rg, CROpportunity order)
		{
			Address shipAddress = null;

			Location loc = (Location)PXSelect<Location,
				Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>.
				Select(rg, order.BAccountID, order.LocationID);
			if (loc != null)
			{
				shipAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(rg, loc.DefAddressID);
			}

			return shipAddress;
		}
	}
}
