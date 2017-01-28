using Avalara.AvaTax.Adapter;
using PX.Data;
using System.Collections;
using Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;
using PX.Objects.CS;

namespace PX.Objects.TX
{
	public class AvalaraMaint : PXGraph<AvalaraMaint>
	{
		public PXSelect<TXAvalaraSetup> Setup;
		public PXSave<TXAvalaraSetup> Save;
		public PXCancel<TXAvalaraSetup> Cancel;
		public PXSelect<TXAvalaraMapping> Mapping; 
        
		public PXAction<TXAvalaraSetup> test;
		[PXUIField(DisplayName = Messages.TestConnection)]
		[PXButton(ImageKey = Web.UI.Sprite.Main.World)]
		public void Test()
		{
			AddressSvc adrssSvc = new AddressSvc();

			SetupService(this, adrssSvc);
			
			PingResult pres = adrssSvc.Ping("");

			if (pres.ResultCode == SeverityLevel.Success)
			{
				string msg = string.Format("Connection to Avalara was successful. Version of service is {0}", pres.Version);
				Setup.Ask(Setup.Current, "Setup", msg, MessageButtons.OK, MessageIcon.Information);
			}
			else
			{
				LogMessages(pres);
				throw new PXException(Messages.ConnectionToAvalaraFailed);
			}
		}

		public static void SetupService(PXGraph graph, BaseSvc service)
		{
		    TXAvalaraSetup avalaraSetup = PXSelect<TXAvalaraSetup>.Select(graph);

		    if (avalaraSetup != null)
		    {
				if (avalaraSetup.IsActive != true)
					throw new PXSetPropertyException(Messages.AvalaraIsNotActive);

                if(avalaraSetup.Url == null)
                    throw new PXSetPropertyException(Messages.AvalaraUrlIsMissing, avalaraSetup.Url);
		        service.Configuration.Url = avalaraSetup.Url;
		        service.Configuration.Security.Account = avalaraSetup.Account;
		        service.Configuration.Security.License = avalaraSetup.Licence;
		    	service.Configuration.RequestTimeout = avalaraSetup.Timeout.GetValueOrDefault(30);
		        service.Profile.Client = "Acumatica,4.0.0.0";
		    	service.Configuration.LogTransactions = false;
		    	service.Configuration.LogSoap = false;
		    	service.Configuration.LogMessages = false;
				service.Configuration.LogLevel = LogLevel.NONE;
		    }
		}

		public static string CompanyCodeFromBranch(PXGraph graph, int? branchID)
		{
			TXAvalaraMapping m = PXSelect<TXAvalaraMapping, Where<TXAvalaraMapping.branchID, Equal<Required<TXAvalaraMapping.branchID>>>>.Select(graph, branchID);
			if ( m == null)
			{
				TXAvalaraSetup avalaraSetup = PXSelect<TXAvalaraSetup>.Select(graph);
				if (avalaraSetup == null)
				{
					throw new PXSetPropertyException(Messages.AvalaraIsNotActive);
				}

				throw new PXException(Messages.AvalaraBranchToCompanyCodeMappingIsMissing);
			}

			return m.CompanyCode;
		}

		public static bool IsExternalTax(PXGraph graph, string taxZoneID)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.avalaraTax>())
				return false;

			TX.TaxZone tz = PXSelect<TX.TaxZone, Where<TX.TaxZone.taxZoneID, Equal<Required<TaxZone.taxZoneID>>>>.Select(graph, taxZoneID);
			if (tz != null)
				return tz.IsExternal.GetValueOrDefault(false);
			else
				return false;
		}


		public static Address FromAddress(IAddressBase address)
		{
		    Address result = new Address();
		    result.City = address.City;
		    result.Country = address.CountryID;
		    result.Line1 = address.AddressLine1;
		    result.Line2 = address.AddressLine2;
		    result.Line3 = address.AddressLine3;
		    result.PostalCode = address.PostalCode;
		    result.Region = address.State;

		    return result;
		}

		protected virtual void LogMessages(BaseResult result)
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

        public static string GetTaxID(Avalara.AvaTax.Adapter.TaxService.TaxDetail taxDetail)
        {
            return string.Format("{0} {1:G6}", taxDetail.TaxName.ToUpperInvariant(), taxDetail.Rate * 100);
        }
	}
}
