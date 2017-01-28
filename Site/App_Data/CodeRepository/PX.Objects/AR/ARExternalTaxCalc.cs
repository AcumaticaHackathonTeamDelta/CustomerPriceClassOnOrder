using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.AR.Standalone;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;
using AvaMessage = Avalara.AvaTax.Adapter.Message;

using System.Web.Compilation;
using System.Web;
using System.Text;
using PX.Objects.SO;


namespace PX.Objects.AR
{
	public class ARExternalTaxCalc : PXGraph<ARExternalTaxCalc>
	{
        public PXCancel<ARInvoice> Cancel;
        [PXFilterable]
        [PX.SM.PXViewDetailsButton(typeof(ARInvoice.refNbr), WindowMode = PXRedirectHelper.WindowMode.NewWindow)]
		public PXProcessingJoin<ARInvoice,
		InnerJoin<TX.TaxZone, On<TX.TaxZone.taxZoneID, Equal<ARInvoice.taxZoneID>>>, 
		Where<TX.TaxZone.isExternal, Equal<True>,
			And<ARInvoice.isTaxValid, Equal<False>,
			And<ARInvoice.released, Equal<False>>>>> Items;

		public ARExternalTaxCalc()
		{
			Items.SetProcessDelegate(ProcessAll);
		}

		public static ARInvoice Process(ARInvoice doc)
		{
			if ( doc.OrigModule == GL.BatchModule.SO )
			{
				SOInvoiceEntry rg = PXGraph.CreateInstance<SOInvoiceEntry>();
				rg.Document.Current = PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(rg, doc.DocType, doc.RefNbr);
				rg.Document.Current.ApplyPaymentWhenTaxAvailable = doc.ApplyPaymentWhenTaxAvailable;
				rg.SODocument.Current = PXSelect<SOInvoice, Where<SOInvoice.docType, Equal<Required<SOInvoice.docType>>, And<SOInvoice.refNbr, Equal<Required<SOInvoice.refNbr>>>>>.Select(rg, doc.DocType, doc.RefNbr);
				
				return rg.CalculateAvalaraTax(rg.Document.Current);

			}
			else
			{
				List<ARInvoice> list = new List<ARInvoice>();
				list.Add(doc);

				List<ARInvoice> listWithTax = Process(list, false);

				return listWithTax[0];
			}
		}

		public static ARCashSale Process(ARCashSale doc)
		{
			List<ARCashSale> list = new List<ARCashSale>();
			list.Add(doc);
			List<ARCashSale> listWithTax = Process(list, false);

			return listWithTax[0];
		}

		public static ARInvoice Process(ARInvoiceEntry ie)
		{
			if (ie != null)
			{
				if (ie.Document.Current.OrigModule == GL.BatchModule.SO)
				{
					return ie.CalculateAvalaraTax(ie.Document.Current);
				}
				else
				{
					List<ARInvoice> list = new List<ARInvoice>();
					list.Add(ie.Document.Current);

					List<ARInvoice> listWithTax = Process(list, false);

					return listWithTax[0];
				}
			}
			return null;
		}

		public static ARCashSale Process(ARCashSaleEntry ie)
		{
			if (ie != null)
			{
				List<ARCashSale> list = new List<ARCashSale>();
				list.Add(ie.Document.Current);

				List<ARCashSale> listWithTax = Process(list, false);

				return listWithTax[0];
			}
			return null;
		}

		public static List<ARCashSale> Process(List<ARCashSale> list, bool isMassProcess)
		{
			List<ARCashSale> listWithTax = new List<ARCashSale>(list.Count);
			ARCashSaleEntry rg = PXGraph.CreateInstance<ARCashSaleEntry>();
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					rg.Clear();
					rg.Document.Current = PXSelect<ARCashSale, Where<ARCashSale.docType, Equal<Required<ARCashSale.docType>>, And<ARCashSale.refNbr, Equal<Required<ARCashSale.refNbr>>>>>.Select(rg, list[i].DocType, list[i].RefNbr);
					listWithTax.Add(rg.CalculateAvalaraTax(rg.Document.Current));
					PXProcessing<ARCashSale>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						PXProcessing<ARCashSale>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}

			}

			return listWithTax;
		}

		public static List<ARInvoice> Process(List<ARInvoice> list, bool isMassProcess)
		{
			List<ARInvoice> listWithTax = new List<ARInvoice>(list.Count);
			ARInvoiceEntry rg = PXGraph.CreateInstance<ARInvoiceEntry>();
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					rg.Clear();
					rg.Document.Current = PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(rg, list[i].DocType, list[i].RefNbr);
					listWithTax.Add(rg.CalculateAvalaraTax(rg.Document.Current));
					PXProcessing<ARInvoice>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						PXProcessing<ARInvoice>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}

			}

			return listWithTax;
		}

		public static void ProcessAll(List<ARInvoice> list)
		{
			Process(list, true);
		}
	}

	
}

namespace PX.Objects.CS
{

	public interface IAddressValidationService
	{
		void SetupService(PXGraph aGraph);
		bool IsServiceActive(PXGraph aGraph);
		bool ValidateAddress(IAddressBase aAddress, out bool isValid, Dictionary<PXAddressValidator.Fields, string> messages);
	}
		

	public static class PXAddressValidator
	{
		public enum Fields
		{
			None,
			CountryID,
			City,
			State,
			PostalCode,
			AddressLine1,
			AddressLine2,
			AddressLine3,
		};

		public static bool Validate<T>(PXGraph aGraph, T aAddress, bool aSynchronous)
			where T : IAddressBase, IValidatedAddress
		{
			return Validate(aGraph, aAddress, aSynchronous, false); 
		}
		public static bool Validate<T>(PXGraph aGraph, T aAddress, bool aSynchronous, bool updateToValidAddress)
			where T : IAddressBase, IValidatedAddress
		{
			CS.Country country = PXSelect<CS.Country, Where<CS.Country.countryID, Equal<Required<CS.Country.countryID>>>>.Select(aGraph, aAddress.CountryID);

			if (string.IsNullOrEmpty(country.AddressVerificationTypeName))
			{
				if (aSynchronous)
				{
					PXCache cache = aGraph.Caches[typeof(T)];
					string countryFieldName = GetFieldName(typeof(T), Fields.CountryID);
					cache.RaiseExceptionHandling(countryFieldName, aAddress, aAddress.CountryID,
							new PXSetPropertyException(Messages.AddressVerificationServiceIsNotSetup,
								PXErrorLevel.Warning, aAddress.CountryID));
					return false;
				}
				else
				{
					throw new PXException(Messages.AddressVerificationServiceIsNotSetup, aAddress.CountryID);
				}
			}

			IAddressValidationService service = Create(country);
			if (service != null)
			{
				PXCache cache = aGraph.Caches[typeof(T)];
				if (!service.IsServiceActive(aGraph))
				{
					if (aSynchronous)
					{
						string countryFieldName = GetFieldName(typeof(T), Fields.CountryID);
						cache.RaiseExceptionHandling(countryFieldName, aAddress, aAddress.CountryID,
								new PXSetPropertyException(Messages.AddressVerificationServiceIsNotActive, 
									PXErrorLevel.Error, aAddress.CountryID));
						return false;
					}
					else
					{
						throw new PXException(Messages.AddressVerificationServiceIsNotActive, aAddress.CountryID);
					}
				}
				service.SetupService(aGraph);
				bool isValid;
				Dictionary<Fields, string> messages = new Dictionary<Fields, string>();
				
				T copy = (T)cache.CreateCopy(aAddress);
				if (!service.ValidateAddress(copy, out isValid, messages))
					throw new PXException(Messages.UnknownErrorOnAddressValidation);				
				if (isValid)
				{
					T copyToUpdate = copy; 
					if (!updateToValidAddress)
					{
						copyToUpdate = (T)cache.CreateCopy(aAddress);//Clear changes made by ValidateAddress
					}
					copyToUpdate.IsValidated = true;
					aAddress = (T)cache.Update(copyToUpdate);
					if(!updateToValidAddress && aSynchronous) 
					{
						string[] fields = { GetFieldName(typeof(T), Fields.AddressLine1), 
											GetFieldName(typeof(T), Fields.AddressLine2), 
											GetFieldName(typeof(T), Fields.City), 
											GetFieldName(typeof(T), Fields.State), 
											GetFieldName(typeof(T), Fields.PostalCode) };
						foreach( string iFld in fields)
						{
							string ourValue = ((string) cache.GetValue(aAddress, iFld)) ?? String.Empty;
							string extValue = ((string)cache.GetValue(copy, iFld)) ?? String.Empty;
							
							if (String.Compare( ourValue.Trim(), extValue.Trim(), true) != 0)
							{
								cache.RaiseExceptionHandling(iFld, aAddress, ourValue, 
									new PXSetPropertyException(Messages.AddressVerificationServiceReturnsField,PXErrorLevel.Warning, extValue));
							}
						}
					}
				}
				else
				{
					string message = string.Empty;
					StringBuilder messageBuilder = new StringBuilder();
					int count = 0;
					foreach (KeyValuePair<Fields, string> iMsg in messages)
					{
						string fieldName = GetFieldName(typeof(T), iMsg.Key);
						if (!aSynchronous)
						{
							if (count > 0) messageBuilder.Append(",");
							messageBuilder.AppendFormat("{0}:{1}", fieldName, iMsg.Value);
							count++;
						}
						else
						{
							object value = cache.GetValue(aAddress, fieldName);
							cache.RaiseExceptionHandling(fieldName, aAddress, value, new PXSetPropertyException(iMsg.Value));
						}
					}
					if (!aSynchronous)
					{
						throw new PXException(messageBuilder.ToString());
					}
					return false;
				}			
			}
			return true;
		}

		public static void Validate<T>(PXGraph aGraph, List<T> aAddresses, bool aSynchronous, bool updateToValidAddress)
			where T : IAddressBase, IValidatedAddress
		{
			foreach (T it in aAddresses) 
			{
				Validate<T>(aGraph, it, aSynchronous, updateToValidAddress);
			}
		}

		private static string GetFieldName(Type addressType, Fields field)
		{
			switch (field)
			{
				case Fields.None: return Fields.City.ToString();
				default: return field.ToString();
			}
		}

		private static string GetFieldName(string aTypeName, Fields field)
		{
			switch (field)
			{
				case Fields.None: return Fields.City.ToString();
				default: return field.ToString();
			}
		}

		private static IAddressValidationService Create(Country country)
		{
			IAddressValidationService processor = null;
			string processorTypeAsString = country.AddressVerificationTypeName;
			if (string.IsNullOrEmpty(processorTypeAsString) == false)
			{
				try
				{
					Type processorType = PXBuildManager.GetType(processorTypeAsString, true);
					processor = (IAddressValidationService)Activator.CreateInstance(processorType);
				}
				catch (HttpException e)
				{
					throw new PXException(e, Messages.AddressVerificationServiceCreationErrorHTTP, processorTypeAsString);
				}
				catch (Exception e)
				{
					throw new PXException(e, Messages.AddressVerificationServiceCreationError, processorTypeAsString);
				}
			}
			return processor;

		}
	}

	public class AvalaraAddressValidator : IAddressValidationService
	{
		private AvaAddress.AddressSvc _service;
		public AvalaraAddressValidator()
		{
			this._service = new AvaAddress.AddressSvc();
		}
		#region IAddressValidationService Members

		public void SetupService(PXGraph aGraph)
		{
			PX.Objects.TX.AvalaraMaint.SetupService(aGraph, this._service);
		}

		public bool IsServiceActive(PXGraph aGraph) 
		{
			if(PXAccess.FeatureInstalled<FeaturesSet.avalaraTax>() == false|| PXAccess.FeatureInstalled<FeaturesSet.addressValidation>()== false) 
				return false;			
			TX.TXAvalaraSetup avalaraSetup = PXSelect<TX.TXAvalaraSetup>.Select(aGraph);
			return (avalaraSetup != null && (avalaraSetup.IsActive?? false));
		}

		public bool ValidateAddress(IAddressBase aAddress, out bool isValid, Dictionary<PXAddressValidator.Fields, string> messages)
		{
			isValid = false;
			AvaAddress.ValidateRequest request = new AvaAddress.ValidateRequest();
			request.Address = FromAddress(aAddress);
			AvaAddress.ValidateResult result = this._service.Validate(request);
			isValid = (result.ResultCode == Avalara.AvaTax.Adapter.SeverityLevel.Success ||
						result.ResultCode == Avalara.AvaTax.Adapter.SeverityLevel.Warning);
			if (result.ResultCode != Avalara.AvaTax.Adapter.SeverityLevel.Success)
			{
				foreach (Avalara.AvaTax.Adapter.Message iMsg in result.Messages)
				{
					PXAddressValidator.Fields id = GetExtFieldName(iMsg.RefersTo);
					messages.Add(id, String.IsNullOrEmpty(iMsg.Details) ? 
													(String.IsNullOrEmpty(iMsg.Summary)?
														(String.IsNullOrEmpty(iMsg.Source)? Messages.AvalaraAVSUnknownError:iMsg.Source)
														:iMsg.Summary)
														:iMsg.Details);
				}
			}
			if (isValid && result.Addresses.Count>0 ) 
			{
				Copy(aAddress, result.Addresses[0]);
			}
			return true;
		}

		private static AvaAddress.Address FromAddress(IAddressBase address)
		{
			AvaAddress.Address result = new AvaAddress.Address();
			result.City = address.City;
			result.Country = address.CountryID;
			result.Line1 = address.AddressLine1;
			result.Line2 = address.AddressLine2;
			result.Line3 = address.AddressLine3;
			result.PostalCode = address.PostalCode;
			result.Region = address.State;

			return result;
		}

		private static void Copy(IAddressBase dest, AvaAddress.ValidAddress src)
		{
			dest.City = src.City;
			dest.CountryID = src.Country;
			dest.AddressLine1 = src.Line1;
			dest.AddressLine2 = src.Line2;
			dest.AddressLine3 = src.Line3;
			dest.PostalCode = src.PostalCode;
			dest.State = src.Region;			
		}
		#endregion

		private static PXAddressValidator.Fields GetExtFieldName(string aAvaFieldName)
		{
			switch (aAvaFieldName)
			{
				case "Address.Line1": return PXAddressValidator.Fields.AddressLine1;
				case "Address.Line2": return PXAddressValidator.Fields.AddressLine2;
				case "Address.Line3": return PXAddressValidator.Fields.AddressLine3;
				case "Address.City": return PXAddressValidator.Fields.City;
				case "Address.State": return PXAddressValidator.Fields.State;
				case "Address.PostalCode": return PXAddressValidator.Fields.PostalCode;
				case "Address.Country": return PXAddressValidator.Fields.CountryID;
			}
			return PXAddressValidator.Fields.None;
		}

	}

	
	
}
