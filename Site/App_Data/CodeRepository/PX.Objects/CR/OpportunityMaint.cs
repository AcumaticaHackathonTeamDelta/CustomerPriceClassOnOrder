using System;
using System.Collections.Specialized;
using System.Linq;
using PX.Common;
using PX.Data;
using System.Collections;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects.SO;
using System.Collections.Generic;
using System.Diagnostics;
using PX.Objects.GL;
using AvaAddress = Avalara.AvaTax.Adapter.AddressService;

namespace PX.Objects.CR
{
	public class OpportunityMaint : PXGraph<OpportunityMaint>, PXImportAttribute.IPXPrepareItems
	{
		
		[Serializable]
		[PXHidden]
		public partial class ConvertedFilter : IBqlTable
		{
			#region OpportunityID
			public abstract class opportunityID : PX.Data.IBqlField { }
			[PXDBInt]
			[PXUIField(DisplayName = "Opportunity ID", Visibility = PXUIVisibility.SelectorVisible)]
			[PXDefault(typeof(CROpportunity.opportunityID))]
			public virtual String OpportunityID { get; set; }
			#endregion
		}

		#region CreateSalesOrderFilter
		[Serializable()]
		[PXHidden]
		public partial class CreateSalesOrderFilter : IBqlTable
		{
			#region OrderType

			public abstract class orderType : IBqlField
			{
			}

            [PXRestrictor(typeof(Where<SOOrderType.active, Equal<boolTrue>>), Messages.OrderTypeIsNotActive, typeof(SOOrderType.descr))]
			[PXDBString(2, IsFixed = true, InputMask = ">aa")]
            [PXDefault(typeof(Search2<SOOrderType.orderType,
                    InnerJoin<SOSetup, On<SOOrderType.orderType, Equal<SOSetup.defaultOrderType>>>,
                    Where<SOOrderType.active, Equal<boolTrue>>>))]
			[PXSelector(typeof (Search<SOOrderType.orderType>),
				DescriptionField = typeof (SOOrderType.descr))]
			[PXUIField(DisplayName = "Order Type")]
			public virtual String OrderType { get; set; }

			#endregion

			#region RecalcDiscounts

			public abstract class recalcDiscount : IBqlField
			{
			}

			[PXDBBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Recalculate Prices and Discounts")]
			public virtual Boolean? RecalcDiscounts { get; set; }

			#endregion
		}
        #endregion

        #region CreateAccountsFilter
        [Serializable]
        public partial class CreateAccountsFilter : IBqlTable
        {
            #region BAccountID
            public abstract class bAccountID : IBqlField { }

            [PXDefault]
            [PXString(30, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
            [PXUIField(DisplayName = "Business Account ID", Required = true)]
            public virtual string BAccountID { get; set; }
            #endregion

            #region AccountName
            public abstract class accountName : IBqlField { }
            protected string _AccountName;
            [PXDefault()]
            [PXString(60, IsUnicode = true)]
            [PXUIField(DisplayName = "Company Name", Required = true)]
            public virtual string AccountName
            {
                get { return _AccountName; }
                set { _AccountName = value; }
            }
            #endregion

            #region AccountClass
            public abstract class accountClass : IBqlField { }

            [PXDefault(typeof(CRSetup.defaultCustomerClassID), PersistingCheck = PXPersistingCheck.Nothing)]
            [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
            [PXUIField(DisplayName = "Business Account Class", Required = true)]
            [PXSelector(typeof(CRCustomerClass.cRCustomerClassID))]
            public virtual string AccountClass { get; set; }
            #endregion

            #region LinkContactToAccount
            public abstract class linkContactToAccount : IBqlField { }
            [PXBool()]
            [PXUIField(DisplayName = "Link Contact to Account", Visible = false, Enabled = false )]
            public virtual bool? LinkContactToAccount { get; set; }
            #endregion
        }
		#endregion

		#region Selects / Views
        [PXHidden()]
        public PXSelect<BAccount> BAccounts;

		//TODO: need review
		[PXHidden]
		public PXSelect<BAccount>
			bAccountBasic;

        [PXHidden]
        public PXSelect<BAccountR>
            bAccountRBasic;

        [PXHidden]
		public PXSetupOptional<SOSetup>
			sosetup;

		[PXHidden]
		public PXSetup<CRSetup>
			Setup;

        [PXCopyPasteHiddenFields(typeof(CROpportunity.resolution))]
        [PXViewName(Messages.Opportunity)]
        public PXSelect<CROpportunity>
            Opportunity;

		[PXHidden]
		public PXSelect<Contact> Leads;

		public PXSelect<CurrencyInfo,
			Where<CurrencyInfo.curyInfoID, Equal<Current<CROpportunity.curyInfoID>>>>
			currencyinfo;

		public ToggleCurrency<CROpportunity>
			CurrencyView;

	

		[PXHidden]
		[PXCopyPasteHiddenFields(typeof(CROpportunity.description))]
		public PXSelect<CROpportunity,
			Where<CROpportunity.opportunityID, Equal<Current<CROpportunity.opportunityID>>>>
			OpportunityCurrent;

		[PXHidden]
		public PXSelect<CROpportunityProbability,
			Where<CROpportunityProbability.stageCode, Equal<Current<CROpportunity.stageID>>>>
			ProbabilityCurrent;

		[PXHidden]
		public PXSelect<Address>
			Address;

		[PXHidden]
		public PXSetup<Contact, Where<Contact.contactID, Equal<Optional<CROpportunity.contactID>>>> Contacts;

		[PXHidden]
		public PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<CROpportunity.bAccountID>>>> customer;


		[PXViewName(Messages.Answers)]
		public CRAttributeSourceList<CROpportunity, CROpportunity.contactID>
			Answers;

        public PXSetup<CROpportunityClass, Where<CROpportunityClass.cROpportunityClassID, Equal<Current<CROpportunity.cROpportunityClassID>>>> OpportunityClass;

		[PXViewName(Messages.Activities)]
		[PXFilterable]
        [CRReference(typeof(Select<BAccountR, Where<BAccountR.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>),
			typeof(Select<Contact, 
				Where<Current<CROpportunity.allowOverrideContactAddress>, NotEqual<True>, 
					And<Contact.contactID, Equal<Current<CROpportunity.contactID>>>>>))]
        public OpportunityActivities Activities;

		[PXViewName(Messages.Relations)]
		[PXFilterable]
		public CRRelationsList<CROpportunity.noteID>
			Relations;

		[PXViewName(Messages.OpportunityProducts)]
		[PXImport(typeof(CROpportunity))]
		public PXSelect<CROpportunityProducts,
			Where<CROpportunityProducts.cROpportunityID, Equal<Current<CROpportunity.opportunityID>>>>
			Products;
		

	    public PXSelect<CROpportunityTax,
            Where<CROpportunityTax.opportunityID, Equal<Current<CROpportunity.opportunityID>>>> TaxLines;

		 [PXViewName(Messages.OpportunityTax)]
		 public PXSelectJoin<CRTaxTran,
			 InnerJoin<Tax, On<Tax.taxID, Equal<CRTaxTran.taxID>>>,
			 Where<CRTaxTran.opportunityID, Equal<Current<CROpportunity.opportunityID>>>> Taxes;

        [PXViewName(Messages.DiscountDetails)]
		public PXSelect<CROpportunityDiscountDetail,
			Where<CROpportunityDiscountDetail.opportunityID, Equal<Current<CROpportunity.opportunityID>>>>
			DiscountDetails;

		[PXViewName(Messages.CreateSalesOrder)]
		public PXFilter<CreateSalesOrderFilter>
			CreateOrderParams;

	    public PXSetup<Location, 
            Where<Location.bAccountID, Equal<Current<CROpportunity.bAccountID>>,
              And<Location.locationID, Equal<Optional<CROpportunity.locationID>>>>> location;

		#region Create Account
		public PXFilter<ConvertedFilter> ConvertedInfo;

		[PXViewName(Messages.CreateAccount)]
		public PXFilter<CreateAccountsFilter> AccountInfo;
        #endregion

        [PXViewName(Messages.OpportunityContact)]
        public PXSelect<CRContact, Where<CRContact.contactID, Equal<Current<CROpportunity.opportunityContactID>>>> Opportunity_Contact;

        [PXViewName(Messages.OpportunityAddress)]
        public PXSelect<CRAddress, Where<CRAddress.addressID, Equal<Current<CROpportunity.opportunityAddressID>>>> Opportunity_Address;

        [PXHidden]
        public PXSelectJoin<Contact,
            LeftJoin<Address, On<Contact.defAddressID, Equal<Address.addressID>>>,  
            Where<Contact.contactID, Equal<Current<CROpportunity.contactID>>>> CurrentContact;       
		

        [PXHidden]
        public PXSelect<SOBillingContact> CurrentSOBillingContact;
		#endregion

		protected bool isAutoNumberOn;

		#region Ctors

		public OpportunityMaint()
		{
			var crsetup = Setup.Current;

			if (string.IsNullOrEmpty(Setup.Current.OpportunityNumberingID))
			{
				throw new PXSetPropertyException(Messages.NumberingIDIsNull, Messages.CRSetup);
			}

			Activities.GetNewEmailAddress =
				() =>
				{
					var current = Opportunity.Current;
					if (current != null)
					{
						var contact = current.OpportunityContactID.
							With(_ => (CRContact)PXSelect<CRContact,
								Where<CRContact.contactID, Equal<Required<CRContact.contactID>>>>.
							Select(this, _.Value));
						if (contact != null && !string.IsNullOrWhiteSpace(contact.Email))
							return new Email(contact.DisplayName, contact.Email);
					}
					return Email.Empty;
				};

			foreach (Segment segment in PXSelect<Segment,
				Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>>.Select(this, "BIZACCT"))
			{
				if (segment.AutoNumber == true)
				{
					isAutoNumberOn = true;
					break;
				}
			}
			if (isAutoNumberOn)
			{
				FieldDefaulting.AddHandler<CreateAccountsFilter.bAccountID>(delegate(PXCache sender, PXFieldDefaultingEventArgs e)
				{
					object cd;
					Caches[typeof(BAccount)].RaiseFieldDefaulting<BAccount.acctCD>(null, out cd);
					e.NewValue = (string)cd;
					e.Cancel = true;
				});
			}

		    if (!PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
		    {
		        createSalesOrder.SetEnabled(false);
		    }
			// Kesha insert it but.....  its bad for Is Dirty ask him
			//this.Views.Caches.Remove(typeof (Contact));
			var bAccountCache = Caches[typeof(BAccount)];
			PXUIFieldAttribute.SetDisplayName<BAccount.acctCD>(bAccountCache, Messages.BAccountCD);
			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(bAccountCache, Messages.BAccountName);
		}

		#endregion

		#region Actions

		public PXSave<CROpportunity> Save;
		public PXCancel<CROpportunity> Cancel;
		public PXInsert<CROpportunity> Insert;
		public PXCopyPasteAction<CROpportunity> CopyPaste;
		public PXDelete<CROpportunity> Delete;
		public PXFirst<CROpportunity> First;
		public PXPrevious<CROpportunity> Previous;
		public PXNext<CROpportunity> Next;
		public PXLast<CROpportunity> Last;

		public PXMenuAction<CROpportunity> Action;
		public PXMenuInquiry<CROpportunity> Inquiry;


		public PXAction<CROpportunity> createInvoice;
		[PXUIField(DisplayName = Messages.CreateInvoice, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable CreateInvoice(PXAdapter adapter)
		{
			CROpportunity opportunity = this.Opportunity.Current;
			Customer customer = (Customer)PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);

			if (customer == null)
			{
				throw new PXException(Messages.ProspectNotCustomer);
			}

			var stokItems = PXSelectJoinGroupBy<InventoryItem,
				InnerJoin<CROpportunityProducts, On<CROpportunityProducts.inventoryID, Equal<InventoryItem.inventoryID>>>,
				Where<InventoryItem.stkItem, Equal<True>,
					And<CROpportunityProducts.cROpportunityID, Equal<Required<CROpportunityProducts.cROpportunityID>>>>,
				Aggregate<Count<InventoryItem.inventoryID>>>.
				Select(this, opportunity.OpportunityID);
			if (stokItems.RowCount > 0)
				throw new PXException(Messages.NeedOrderInsteadOfInvoice);

			if (opportunity.ARRefNbr != null)
			{
				ARInvoice doc = PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<ARDocType.invoice>, And<ARInvoice.refNbr, Equal<Current<CROpportunity.aRRefNbr>>>>>.Select(this);

				if (doc == null)
				{
					WebDialogResult result = Opportunity.View.Ask(opportunity, Messages.AskConfirmation, Messages.InvoiceAlreadyCreatedDeleted, MessageButtons.YesNo, MessageIcon.Question);
					if (result == WebDialogResult.Yes)
					{
						opportunity.ARRefNbr = null;
					}
				}
				else
				{
					throw new PXException(Messages.InvoiceAlreadyCreated);
				}
			}

			if (customer != null && opportunity.ARRefNbr == null)
			{
                PXLongOperation.StartOperation(this, delegate()
                {
                    var grapph = PXGraph.CreateInstance<OpportunityMaint>();
                    grapph.Opportunity.Current = opportunity;
                    grapph.DoCreateInvoice();
                });
			}

			return adapter.Get();
		}

        protected virtual void DoCreateInvoice()
        {
            var opportunity = this.Opportunity.Current;
            ARInvoiceEntry docgraph = PXGraph.CreateInstance<ARInvoiceEntry>();

            Customer customer = (Customer)PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);
            docgraph.customer.Current = customer;

            CurrencyInfo info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CROpportunity.curyInfoID>>>>.Select(this);
            info.CuryInfoID = null;
            info = docgraph.currencyinfo.Insert(info);

            ARInvoice invoice = new ARInvoice();
            invoice.DocType = ARDocType.Invoice;
            invoice.CuryID = info.CuryID;
            invoice.CuryInfoID = info.CuryInfoID;
            invoice.DocDate = opportunity.CloseDate;
            invoice.Hold = true;
            invoice.BranchID = opportunity.BranchID;
            invoice = PXCache<ARInvoice>.CreateCopy(docgraph.Document.Insert(invoice));

            invoice.TermsID = customer.TermsID;
            invoice.InvoiceNbr = opportunity.OpportunityID;
            invoice.DocDesc = opportunity.OpportunityName;
            invoice.CustomerID = opportunity.BAccountID;
            invoice.CustomerLocationID = opportunity.LocationID ?? customer.DefLocationID;
            if (opportunity.TaxZoneID != null)
            {
                invoice.TaxZoneID = opportunity.TaxZoneID;
                TaxAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(docgraph.Transactions.Cache, null, TaxCalc.ManualCalc);
            }
            invoice.ProjectID = opportunity.ProjectID;
            invoice = docgraph.Document.Update(invoice);

            var products = PXSelectJoin<CROpportunityProducts, LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<CROpportunityProducts.inventoryID>>>, Where<CROpportunityProducts.cROpportunityID, Equal<Current<CROpportunity.opportunityID>>>>.Select(this);
            if (!products.Any() && Opportunity.Current.ManualTotalEntry == true)
            {
                ARTran tran = new ARTran();
                tran = docgraph.Transactions.Insert(tran);
                if (tran != null)
                {
                    tran.Qty = 1;
                    tran.CuryUnitPrice = Opportunity.Current.CuryAmount;
					using (new PXLocaleScope(customer.LocaleName))
					{
						tran.TranDesc = PXMessages.LocalizeNoPrefix(Messages.ManualAmount);
					}
                }
                tran = docgraph.Transactions.Update(tran);
            }
            else
            {
                foreach (PXResult<CROpportunityProducts, InventoryItem> res in products)
                {
                    CROpportunityProducts product = (CROpportunityProducts)res;
                    InventoryItem item = (InventoryItem)res;

                    ARTran tran = new ARTran();
                    tran = docgraph.Transactions.Insert(tran);
                    if (tran != null)
                    {
                        tran.InventoryID = product.InventoryID;
						using (new PXLocaleScope(customer.LocaleName))
						{
							tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(this.Caches[typeof(CROpportunityProducts)],
														  product, typeof(CROpportunityProducts.transactionDescription).Name, this.Culture.Name);
						}
                        tran.Qty = product.Quantity;
                        tran.UOM = product.UOM;
                        tran.CuryUnitPrice = product.CuryUnitPrice;
                        tran.CuryTranAmt = product.CuryAmount;
                        tran.TaxCategoryID = product.TaxCategoryID;
                        tran.ProjectID = product.ProjectID;
                        tran.TaskID = product.TaskID;
                        if (item.Commisionable.HasValue)
                            tran.Commissionable = item.Commisionable;
                        tran.ManualPrice = true;

                        if (product.ManualDisc == true)
                        {
                            tran.ManualDisc = true;
                            tran.DiscAmt = product.DiscAmt;
                            tran.DiscPct = product.DiscPct;
                        }
                    }

                    tran = docgraph.Transactions.Update(tran);

                    PXNoteAttribute.CopyNoteAndFiles(Products.Cache, product, docgraph.Transactions.Cache, tran, Setup.Current);
                }
            }
            PXNoteAttribute.CopyNoteAndFiles(Opportunity.Cache, opportunity, docgraph.Document.Cache, invoice, Setup.Current);

            //Skip all customer dicounts
            foreach (ARInvoiceDiscountDetail discountDetail in docgraph.ARDiscountDetails.Select())
            {
                docgraph.ARDiscountDetails.SetValueExt<ARInvoiceDiscountDetail.skipDiscount>(discountDetail, true);
            }
            ARInvoice old_row = PXCache<ARInvoice>.CreateCopy(docgraph.Document.Current);
            docgraph.Document.Cache.SetValueExt<ARInvoice.curyDiscTot>(docgraph.Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<ARInvoiceDiscountDetail>(docgraph.ARDiscountDetails));
            docgraph.Document.Cache.RaiseRowUpdated(docgraph.Document.Current, old_row);
                        
            if (opportunity.CuryDiscTot > 0)
            {
                //Add Document Discount Tran:
                ARTran tran = new ARTran();
                tran.LineType = SOLineType.Discount;
				using (new PXLocaleScope(customer.LocaleName))
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.OpportunityDocDisc);
                tran.CuryTranAmt = opportunity.CuryDiscTot;
                tran.DrCr = (invoice.DrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
                tran.BatchNbr = "";
                tran.Commissionable = false;
                tran = docgraph.Transactions.Insert(tran);

                Location loc = PXSelect<Location, Where<Location.bAccountID, Equal<Current<CROpportunity.bAccountID>>, And<Location.locationID, Equal<Current<CROpportunity.locationID>>>>>.Select(this);
                if (loc != null && loc.CDiscountAcctID != null)
                {
                    tran.AccountID = loc.CDiscountAcctID;
                    tran.SubID = loc.CDiscountSubID;
                }
                else
                {
                    CROpportunityClass oc = PXSelect<CROpportunityClass, Where<CROpportunityClass.cROpportunityClassID, Equal<Current<CROpportunity.cROpportunityClassID>>>>.Select(this);
                    if (oc != null && oc.DiscountAcctID != null)
                    {
                        tran.AccountID = oc.DiscountAcctID;
                        tran.SubID = oc.DiscountSubID;
                    }
                }

                docgraph.Transactions.Cache.SetDefaultExt<ARTran.taskID>(tran);
                if (tran.TaskID == null && !PM.ProjectDefaultAttribute.IsNonProject(this, tran.ProjectID))
                {
                    Account ac = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, tran.AccountID);
                    throw new PXException(SO.Messages.TaskWasNotAssigned, ac.AccountCD);
                }
            }

            invoice = docgraph.Document.Update(invoice);

            foreach (CRTaxTran tax in PXSelect<CRTaxTran, Where<CRTaxTran.opportunityID, Equal<Current<CROpportunity.opportunityID>>>>.Select(this))
            {
                if (opportunity.TaxZoneID == null)
                {
                    this.OpportunityCurrent.Cache.RaiseExceptionHandling<CROpportunity.taxZoneID>(
                        opportunity, null,
                        new PXSetPropertyException<CROpportunity.taxZoneID>(ErrorMessages.FieldIsEmpty,
                            typeof(CROpportunity.taxZoneID).Name));

                }

                ARTaxTran new_artax = new ARTaxTran();
                new_artax.TaxID = tax.TaxID;

                new_artax = docgraph.Taxes.Insert(new_artax);

                if (new_artax != null)
                {
                    new_artax = PXCache<ARTaxTran>.CreateCopy(new_artax);
                    new_artax.TaxRate = tax.TaxRate;
                    new_artax.CuryTaxableAmt = tax.CuryTaxableAmt;
                    new_artax.CuryTaxAmt = tax.CuryTaxAmt;
                    new_artax = docgraph.Taxes.Update(new_artax);
                }
            }

            invoice.CuryOrigDocAmt = invoice.CuryDocBal;
            invoice.Hold = true;
            docgraph.Document.Update(invoice);

            using (PXConnectionScope cs = new PXConnectionScope())
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    if (this.Opportunity.Cache.GetStatus(opportunity) == PXEntryStatus.Notchanged)
                    {
                        this.Opportunity.Cache.SetStatus(opportunity, PXEntryStatus.Updated);
                    }
                    this.Save.Press();
                    ts.Complete();
                }
            }
            docgraph.OpportunityBackReference.Update(opportunity);
			docgraph.customer.Current.CreditRule = customer.CreditRule;

            if (opportunity.AllowOverrideContactAddress == true)
            {
                CRContact _CRContact = Opportunity_Contact.SelectSingle();
                CRAddress _CRAddress = Opportunity_Address.SelectSingle();
                // Insert
                if (_CRContact != null)
                {
                    ARContact _billingContact = docgraph.Billing_Contact.Select();

                    if (_billingContact != null)
                    {
                        _billingContact.FullName = _CRContact.FullName;
                        _billingContact.Salutation = _CRContact.Salutation;
                        _billingContact.Phone1 = _CRContact.Phone1;
                        _billingContact.Email = _CRContact.Email;
                        _billingContact = docgraph.Billing_Contact.Update(_billingContact);
                        _billingContact.IsDefaultContact = false;
                        _billingContact = docgraph.Billing_Contact.Update(_billingContact);
                    }
                }

                if (_CRAddress != null)
                {
                    ARAddress _billingAddress = docgraph.Billing_Address.Select();

                    if (_billingAddress != null)
                    {
                        _billingAddress.AddressLine1 = _CRAddress.AddressLine1;
                        _billingAddress.AddressLine2 = _CRAddress.AddressLine2;
                        _billingAddress.City = _CRAddress.City;
                        _billingAddress.CountryID = _CRAddress.CountryID;
                        _billingAddress.State = _CRAddress.State;
                        _billingAddress.PostalCode = _CRAddress.PostalCode;
                        _billingAddress = docgraph.Billing_Address.Update(_billingAddress);
                        _billingAddress.IsDefaultAddress = false;
                        _billingAddress = docgraph.Billing_Address.Update(_billingAddress);
                    }
                }
            }

            throw new PXRedirectRequiredException(docgraph, "");
        }

        public PXAction<CROpportunity> createContact;
        [PXUIField(DisplayName = Messages.CreateContact, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton(OnClosingPopup = PXSpecialButtonType.Cancel)]
        public virtual IEnumerable CreateContact(PXAdapter adapter)
        {
            var row = OpportunityCurrent.Current;
            if (row == null) return adapter.Get();
            
            Actions.PressSave();

            var row1 = Opportunity_Contact.SelectSingle();
            var row2 = Opportunity_Address.SelectSingle();

            ContactMaint graph = CreateInstance<ContactMaint>();
            Contact contact = (Contact)graph.ContactCurrent.Cache.CreateInstance();
            contact = graph.ContactCurrent.Cache.Insert(contact) as Contact;           
            contact.FullName = row1.FullName;
            contact.Title = row1.Title;
            contact.FirstName = row1.FirstName;
            contact.LastName = row1.LastName;
            contact.Salutation = row1.Salutation;
            contact.EMail = row1.Email;
            contact.WebSite = row1.WebSite;
            contact.Phone1 = row1.Phone1;
            contact.Phone1Type = row1.Phone1Type;
            contact.Phone2 = row1.Phone2;
            contact.Phone2Type = row1.Phone2Type;
            contact.Phone3 = row1.Phone3;
            contact.Phone3Type = row1.Phone3Type;
            contact.Fax = row1.Fax;
            contact.FaxType = row1.FaxType;
            if (row.BAccountID != null)
                contact.BAccountID = row.BAccountID;
            graph.ContactCurrent.Cache.Update(contact);


            Address address = (Address)graph.AddressCurrent.Cache.CreateInstance();
            address = graph.AddressCurrent.Cache.Insert(address) as Address;
            address.AddressLine1 = row2.AddressLine1;
            address.AddressLine2 = row2.AddressLine2;
            address.City = row2.City;
            address.CountryID = row2.CountryID;
            address.State = row2.State;
            address.PostalCode = row2.PostalCode;
            graph.AddressCurrent.Cache.Update(address);

            contact.DefAddressID = address.AddressID;
            OpportunityCurrent.Current.ContactID = contact.ContactID;

            graph.Opportunities.Current = row;
            graph.Opportunities.Cache.SetStatus(graph.Opportunities.Current, PXEntryStatus.Updated);

            throw new PXRedirectRequiredException(graph, "Contact");            
        }

		public PXAction<CROpportunity> createAccount;
		[PXUIField(DisplayName = Messages.CreateAccount, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton()]
		public virtual IEnumerable CreateAccount(PXAdapter adapter)
		{
			List<CROpportunity> opportunities = new List<CROpportunity>(adapter.Get().Cast<CROpportunity>());
			foreach (CROpportunity opp in opportunities)
			{
			    if (AccountInfo.View.Answer == WebDialogResult.None)
			    {
                    AccountInfo.Cache.Clear();
                    CreateAccountsFilter filterdata = AccountInfo.Cache.Insert() as CreateAccountsFilter;
                    filterdata.LinkContactToAccount = true;
                }
                if (AccountInfo.AskExt() != WebDialogResult.OK)
                    return opportunities;
				bool empty_required = !AccountInfo.VerifyRequired();
				BAccount existing = PXSelect<BAccount, Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>.SelectSingleBound(this, null, AccountInfo.Current.BAccountID);
				if (existing != null)
				{
					AccountInfo.Cache.RaiseExceptionHandling<CreateAccountsFilter.bAccountID>(AccountInfo.Current, AccountInfo.Current.BAccountID, new PXSetPropertyException(Messages.BAccountAlreadyExists, AccountInfo.Current.BAccountID));
					return opportunities;
				}
				if (empty_required) return opportunities;

				Save.Press();
				PXLongOperation.StartOperation(this, () => ConvertToAccount(opp, AccountInfo.Current));
			}
			return opportunities;
		}

		public void ConvertToAccount(CROpportunity opportunity, CreateAccountsFilter param)
		{
			BusinessAccountMaint accountMaint = CreateInstance<BusinessAccountMaint>();
			object cd = param.BAccountID;
			accountMaint.BAccount.Cache.RaiseFieldUpdating<BAccount.acctCD>(null, ref cd);
			BAccount account = new BAccount
			{
				AcctCD = (string)cd,
				AcctName = param.AccountName,
				Type = BAccountType.ProspectType,
				ParentBAccountID = opportunity.ParentBAccountID,
				ClassID = param.AccountClass,
				WorkgroupID = opportunity.WorkgroupID,
				OwnerID = opportunity.OwnerID
			};

			try
			{
				object newValue = account.OwnerID;
				accountMaint.BAccount.Cache.RaiseFieldVerifying<BAccount.ownerID>(account, ref newValue);
			}
			catch (PXSetPropertyException)
			{
				account.OwnerID = null;
			}

			account = accountMaint.BAccount.Insert(account);
			accountMaint.Answers.CopyAllAttributes(account, opportunity);

            if (accountMaint.DefLocation.Current != null)
            {
                accountMaint.DefLocation.Current.CTaxZoneID = opportunity.TaxZoneID;
                opportunity.TaxZoneID = null;
            }

			#region Set Contact and Address fields

		    if (param.LinkContactToAccount == true)
		    {
			Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<CROpportunity.contactID>>>>.Select(accountMaint, opportunity.ContactID);
			if (contact != null)
			{
                accountMaint.Answers.CopyAttributes(account, contact);
                    contact.BAccountID = account.BAccountID;
                    contact.Status = LeadStatusesAttribute.Converted;
                    contact.MajorStatus = LeadMajorStatusesAttribute._CONVERTED;
                    accountMaint.Contacts.Update(contact);
                }
			}

            Contact defContact = PXCache<Contact>.CreateCopy(PXSelect<Contact, Where<Contact.contactID, Equal<Current<BAccount.defContactID>>>>.SelectSingleBound(accountMaint, new object[] { account }));
			
		    defContact = FillFromOpportunityContact(defContact);
			defContact.ContactType = ContactTypesAttribute.BAccountProperty;
			defContact.FullName = account.AcctName;
			defContact.ContactID = account.DefContactID;
			defContact.DefAddressID = account.DefAddressID;
			defContact.BAccountID = account.BAccountID;
			defContact = accountMaint.DefContact.Update(defContact);

			Address defAddress = PXCache<Address>.CreateCopy(PXSelect<Address, Where<Address.addressID, Equal<Current<BAccount.defAddressID>>>>.SelectSingleBound(accountMaint, new object[] { account }));
            defAddress = FillFromOpportunityAddress(defAddress);
			defAddress.AddressID = account.DefAddressID;
			defAddress.BAccountID = account.BAccountID;
			defAddress = accountMaint.AddressCurrent.Update(defAddress);

			opportunity.BAccountID = account.BAccountID;
            accountMaint.Opportunities.Cache.RaiseFieldUpdated<CROpportunity.bAccountID>(opportunity, null);
            opportunity.LocationID = account.DefLocationID;

			foreach (CRPMTimeActivity a in PXSelect<CRPMTimeActivity, Where<CRPMTimeActivity.refNoteID, Equal<Required<CROpportunity.noteID>>>>.Select(accountMaint, opportunity.NoteID))
			{
				a.BAccountID = account.BAccountID;
				accountMaint.Activities.Cache.Update(a);
			}

			accountMaint.Opportunities.Current = opportunity;
            accountMaint.OpportunityLink.Cache.SetStatus(accountMaint.Opportunities.Current, PXEntryStatus.Updated);

            #endregion
            throw new PXRedirectRequiredException(accountMaint, "Business Account");
		}

		public PXAction<CROpportunity> viewInvoice;
		[PXUIField(DisplayName = Messages.ViewInvoice, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public virtual IEnumerable ViewInvoice(PXAdapter adapter)
		{
			if (Opportunity.Current != null && !string.IsNullOrEmpty(Opportunity.Current.ARRefNbr))
			{
				ARInvoiceEntry target = PXGraph.CreateInstance<ARInvoiceEntry>();
				target.Clear();
				target.Document.Current = target.Document.Search<ARInvoice.refNbr>(Opportunity.Current.ARRefNbr);
				throw new PXRedirectRequiredException(target, "ViewInvoice");
			}

			return adapter.Get();
		}

		public PXAction<CROpportunity> createSalesOrder;
		[PXUIField(DisplayName = Messages.CreateSalesOrder, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable CreateSalesOrder(PXAdapter adapter)
		{
			foreach (CROpportunity opportunity in adapter.Get())
			{
				Customer customer = (Customer)PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);
				if (customer == null)
				{
					throw new PXException(Messages.ProspectNotCustomer);
				}

				if (!string.IsNullOrEmpty(opportunity.OrderNbr))
				{
					SOOrder doc = PXSelect<SOOrder,
						Where<SOOrder.orderType, Equal<Current<CROpportunity.orderType>>,
							And<SOOrder.orderNbr, Equal<Current<CROpportunity.orderNbr>>>>>.
						Select(this);

					if (doc == null)
					{
						WebDialogResult result = Opportunity.View.Ask(opportunity, Messages.AskConfirmation, Messages.OrderAlreadyCreatedDeleted, MessageButtons.YesNo, MessageIcon.Question);
						if (result == WebDialogResult.Yes)
						{
							opportunity.OrderNbr = null;
						}
					}
					else
					{
						WebDialogResult result = Opportunity.View.Ask(opportunity, Messages.AskConfirmation, Messages.OrderView, MessageButtons.YesNo, MessageIcon.Question);
						if (result == WebDialogResult.Yes)
						{
							SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();
							docgraph.Document.Current = doc;
							throw new PXRedirectRequiredException(docgraph, "");
						}
						//throw new PXException(Messages.OrderAlreadyCreated);
					}
				}

				if (customer != null && opportunity.OrderNbr == null &&                    
					CreateOrderParams.AskExt() == WebDialogResult.OK &&
                    CreateOrderParams.VerifyRequired())
				{
				    PXLongOperation.StartOperation(this, delegate()
				    {
				        var grapph = PXGraph.CreateInstance<OpportunityMaint>();
				        grapph.Opportunity.Current = opportunity;
                        grapph.CreateOrderParams.Current = CreateOrderParams.Current;
				        grapph.DoCreateSalesOrder();
				    });
				}
				yield return opportunity;
			}

		}

        protected virtual void DoCreateSalesOrder()
        {
            var opportunity = this.Opportunity.Current;
            Customer customer = (Customer)PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);

            var recalcDiscounts = CreateOrderParams.Current.RecalcDiscounts == true;

            SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();

            CurrencyInfo info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CROpportunity.curyInfoID>>>>.Select(this);
            info.CuryInfoID = null;
            info = docgraph.currencyinfo.Insert(info);

            SOOrder doc = new SOOrder();
            doc.OrderType = CreateOrderParams.Current.OrderType ?? SOOrderTypeConstants.SalesOrder;
            doc = docgraph.Document.Insert(doc);
            doc = PXCache<SOOrder>.CreateCopy(docgraph.Document.Search<SOOrder.orderNbr>(doc.OrderNbr));

            doc.CuryInfoID = info.CuryInfoID;
            doc = PXCache<SOOrder>.CreateCopy(docgraph.Document.Update(doc));

            doc.CuryID = info.CuryID;
            doc.OrderDate = Accessinfo.BusinessDate;
            doc.OrderDesc = opportunity.OpportunityName;
            doc.TermsID = customer.TermsID;
            doc.CustomerID = opportunity.BAccountID;
            doc.CustomerLocationID = opportunity.LocationID ?? customer.DefLocationID;
            if (opportunity.TaxZoneID != null)
            {
            doc.TaxZoneID = opportunity.TaxZoneID;
                SOTaxAttribute.SetTaxCalc<SOLine.taxCategoryID>(docgraph.Transactions.Cache, null, TaxCalc.ManualCalc);
            }
            doc.ProjectID = opportunity.ProjectID;
            doc.BranchID = opportunity.BranchID;
            doc = docgraph.Document.Update(doc);

            bool failed = false;
            foreach (CROpportunityProducts product in SelectProducts(opportunity.OpportunityID))
            {
                if (product.SiteID == null)
                {
                    InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<CROpportunityProducts.inventoryID>(Products.Cache, product);
                    if (item != null && item.NonStockShip == true)
                    {
                        Products.Cache.RaiseExceptionHandling<CROpportunityProducts.siteID>(product, null,
                            new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CROpportunityProducts.siteID).Name));
                        failed = true;
                    }
                }

                if (product.IsFree == true && product.ManualDisc == false && recalcDiscounts)
                {
                    continue;
                }

                SOLine tran = new SOLine();
                tran = docgraph.Transactions.Insert(tran);
                if (tran != null)
                {
                tran.InventoryID = product.InventoryID;
                tran.SubItemID = product.SubItemID;
                tran.TranDesc = product.TransactionDescription;
                tran.Qty = product.Quantity;
                tran.UOM = product.UOM;
                tran.CuryUnitPrice = product.CuryUnitPrice;
                tran.CuryLineAmt = product.CuryAmount;
                tran.TaxCategoryID = product.TaxCategoryID;
                tran.SiteID = product.SiteID;
                tran.IsFree = product.IsFree;
                tran.ProjectID = product.ProjectID;
                tran.TaskID = product.TaskID;

                if (recalcDiscounts)
                {
                    //tran.CuryUnitPrice = null;
                    //tran.CuryExtPrice = null;
                    tran.ManualDisc = false;
                    tran.ManualPrice = false;
                }
                else
                {
                    tran.ManualDisc = true;
                    tran.ManualPrice = true;
                }

                if (tran.ManualDisc == true)
                {
                    tran.DiscAmt = product.DiscAmt;
                    tran.DiscPct = product.DiscPct;
                }

                    tran = docgraph.Transactions.Update(tran);
                }

                PXNoteAttribute.CopyNoteAndFiles(Products.Cache, product, docgraph.Transactions.Cache, tran, Setup.Current);
            }
            PXNoteAttribute.CopyNoteAndFiles(Opportunity.Cache, opportunity, docgraph.Document.Cache, doc, Setup.Current);

            if (failed)
                throw new PXException(Messages.SiteNotDefined);

            //Skip all customer dicounts
            foreach (SOOrderDiscountDetail discountDetail in docgraph.DiscountDetails.Select())
            {
                docgraph.DiscountDetails.SetValueExt<SOOrderDiscountDetail.skipDiscount>(discountDetail, true);
            }
            SOOrder old_row = PXCache<SOOrder>.CreateCopy(docgraph.Document.Current);
            docgraph.Document.Cache.SetValueExt<SOOrder.curyDiscTot>(docgraph.Document.Current, DiscountEngine.GetTotalGroupAndDocumentDiscount<SOOrderDiscountDetail>(docgraph.DiscountDetails));
            docgraph.Document.Cache.RaiseRowUpdated(docgraph.Document.Current, old_row);

            if (!recalcDiscounts)
            {
                doc = docgraph.Document.Update(doc);
            }

            if (opportunity.TaxZoneID != null)
            {
                foreach (CRTaxTran tax in PXSelect<CRTaxTran, Where<CRTaxTran.opportunityID, Equal<Current<CROpportunity.opportunityID>>>>.Select(this))
                {
                    SOTaxTran newtax = new SOTaxTran();
                    newtax.LineNbr = int.MaxValue;
                    newtax.TaxID = tax.TaxID;

                    newtax = docgraph.Taxes.Insert(newtax);

                    if (newtax != null)
                    {
                        newtax = PXCache<SOTaxTran>.CreateCopy(newtax);
                        newtax.TaxRate = tax.TaxRate;
                        newtax.CuryTaxableAmt = tax.CuryTaxableAmt;
                        newtax.CuryTaxAmt = tax.CuryTaxAmt;

						newtax.CuryUnshippedTaxableAmt = tax.CuryTaxableAmt;
						newtax.CuryUnshippedTaxAmt = tax.CuryTaxAmt;
						newtax.CuryUnbilledTaxableAmt = tax.CuryTaxableAmt;
						newtax.CuryUnbilledTaxAmt = tax.CuryTaxAmt;

                        newtax = docgraph.Taxes.Update(newtax);
                    }
                }
            }

            docgraph.OpportunityBackReference.Update(opportunity);

            if (opportunity.AllowOverrideContactAddress == true)
            {
                CRContact _CRContact = Opportunity_Contact.SelectSingle();
                CRAddress _CRAddress = Opportunity_Address.SelectSingle();
                // Insert
                if (_CRContact != null)
                {
                    SOBillingContact _billingContact = docgraph.Billing_Contact.Select();                                    

                    if (_billingContact != null)
                    {
                        _billingContact.FullName = _CRContact.FullName;
                        _billingContact.Salutation = _CRContact.Salutation;
                        _billingContact.Phone1 = _CRContact.Phone1;
                        _billingContact.Email = _CRContact.Email; 
                                           
                        _billingContact = docgraph.Billing_Contact.Update(_billingContact);
                        _billingContact.IsDefaultContact = false;
                        _billingContact = docgraph.Billing_Contact.Update(_billingContact);
                    }                    
                }

                if (_CRAddress != null)
                {
                    SOBillingAddress _billingAddress = docgraph.Billing_Address.Select();

                    if (_billingAddress != null)
                    {
                        _billingAddress.AddressLine1 = _CRAddress.AddressLine1;
                        _billingAddress.AddressLine2 = _CRAddress.AddressLine2;
                        _billingAddress.City = _CRAddress.City;
                        _billingAddress.CountryID = _CRAddress.CountryID;
                        _billingAddress.State = _CRAddress.State;
                        _billingAddress.PostalCode = _CRAddress.PostalCode;
                        _billingAddress = docgraph.Billing_Address.Update(_billingAddress);
                        _billingAddress.IsDefaultAddress = false;
                        _billingAddress = docgraph.Billing_Address.Update(_billingAddress);
                    }
                }
            }
            throw new PXRedirectRequiredException(docgraph, "");
	    }

	    public PXAction<CROpportunity> viewSalesOrder;
		[PXUIField(DisplayName = Messages.ViewSalesOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public virtual IEnumerable ViewSalesOrder(PXAdapter adapter)
		{
			var row = Opportunity.Current;
			if (row != null && !string.IsNullOrEmpty(row.OrderNbr))
			{
				SOOrderEntry target = PXGraph.CreateInstance<SOOrderEntry>();
				target.Clear();
				target.Document.Current = target.Document.Search<SOOrder.orderNbr>(row.OrderNbr, row.OrderType);
				throw new PXRedirectRequiredException(target, "ViewSalesOrder");
			}

			return adapter.Get();
		}

		public PXAction<CROpportunity> updateClosingDate;
		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable UpdateClosingDate(PXAdapter adapter)
		{
			var opportunity = Opportunity.Current;
			if (opportunity != null)
			{
				opportunity.ClosingDate = Accessinfo.BusinessDate;
				Opportunity.Cache.Update(opportunity);
				Save.Press();
			}
			return adapter.Get();
		}

        public PXAction<CROpportunity> viewMainOnMap;
        [PXUIField(DisplayName = Messages.ViewOnMap)]
        [PXButton]
        public virtual void ViewMainOnMap()
        {
            var address = Opportunity_Address.SelectSingle();
            if (address != null)
            {
                BAccountUtility.ViewOnMap(address);
            }
        }

		#endregion

		#region Event Handlers

		#region Contacts

		[CustomerProspectVendor(DisplayName = "Business Account", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual void Contact_BAccountID_CacheAttached(PXCache sender)
		{

		}

		#endregion

		#region CROpportunity

		[PXDBString(2, IsFixed = true)]
		[PXStringList(new string[0], new string[0])]
		[PXUIField(DisplayName = "Reason")]
		[CRDropDownAutoValue(typeof(CROpportunity.status))]
		public virtual void CROpportunity_Resolution_CacheAttached(PXCache sender)
		{

		}

		protected virtual void CROpportunity_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var row = e.Row as CROpportunity;
			if (row == null) return;

			var customerLocation = (Location)PXSelect<Location,
				Where<Location.bAccountID, Equal<Required<CROpportunity.bAccountID>>,
					And<Location.locationID, Equal<Required<CROpportunity.locationID>>>>>.
				Select(this, row.BAccountID, row.LocationID);
            if (customerLocation != null)
            {
                if (!string.IsNullOrEmpty(customerLocation.CTaxZoneID))
                {
                    e.NewValue = customerLocation.CTaxZoneID;
                }
                else
                {
                    var address = (Address)PXSelect<Address,
                        Where<Address.addressID, Equal<Required<Address.addressID>>>>.
                        Select(this, customerLocation.DefAddressID);
                    if (address != null && !string.IsNullOrEmpty(address.PostalCode))
                    {
                        e.NewValue = TaxBuilderEngine.GetTaxZoneByZip(this, address.PostalCode);
                    }
                }
            }
            if (e.NewValue == null)
            {
                var branchLocation = (Location)PXSelectJoin<Location,
                    InnerJoin<Branch, On<Branch.branchID, Equal<Current<CROpportunity.branchID>>>,
                    InnerJoin<BAccount, On<Branch.bAccountID, Equal<BAccount.bAccountID>>>>,
                        Where<Location.locationID, Equal<BAccount.defLocationID>>>.Select(this);
                if (branchLocation != null && branchLocation.VTaxZoneID != null)
                    e.NewValue = branchLocation.VTaxZoneID;
                else
                    e.NewValue = row.TaxZoneID;
            }
		}

		protected virtual void CROpportunity_CROpportunityClassID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = Setup.Current.DefaultOpportunityClassID;
		}

		protected virtual void CROpportunity_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var row = e.Row as CROpportunity;
			if (row == null || row.BAccountID == null) return;

			var baccount = (BAccount)PXSelect<BAccount, 
				Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(this, row.BAccountID);

			if (baccount != null)
			{
				e.NewValue = baccount.DefLocationID;
				e.Cancel = true;
			}
		}


		protected virtual void CROpportunity_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled<CROpportunity.opportunityID>(cache, e.Row, true);

            CROpportunity row = e.Row as CROpportunity;
            if (row == null) return;

            PXUIFieldAttribute.SetEnabled<CROpportunity.allowOverrideContactAddress>(cache, row, !(row.BAccountID == null && row.ContactID == null));
            Caches[typeof(CRContact)].AllowUpdate = row.AllowOverrideContactAddress == true;
            Caches[typeof(CRAddress)].AllowUpdate = row.AllowOverrideContactAddress == true;            


			PXUIFieldAttribute.SetVisible<CROpportunity.curyID>(cache, row, IsMultyCurrency);
			CROpportunityClass source = PXSelectorAttribute.Select<CROpportunity.cROpportunityClassID>(cache, e.Row) as CROpportunityClass;
			if (source != null)
			{
				Activities.DefaultEMailAccountId = source.DefaultEMailAccountID;
			}

			// The code	inside this	if Allows CuryID if	BAccount of	current	Opportunity	is Customer	and	AllowOverrideCury is enables/disables CuryID
			if (row.BAccountID != null && IsMultyCurrency)
			{
				bool allowCuryID = true;
				BAccount bAccount = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(this, row.BAccountID);
				if (bAccount != null)
				{
					if (bAccount.Type == BAccountType.CustomerType)
					{
						Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, row.BAccountID);
						if (customer != null)
						{
							allowCuryID = (customer.AllowOverrideCury ?? false);
						}
					}
				}
				PXUIFieldAttribute.SetEnabled<CROpportunity.curyID>(cache, row, allowCuryID);
			}

			if (row.BAccountID != null)
			{
				if (SelectProducts(row.OpportunityID).GetEnumerator().MoveNext()) //Count > 0
				{
					PXUIFieldAttribute.SetEnabled<CROpportunity.bAccountID>(cache, row, false);
				}
			}

			viewInvoice.SetEnabled(SelectInvoice(row.ARRefNbr) != null);
			viewSalesOrder.SetEnabled(SelectOrder(row.OrderNbr) != null);

			PXUIFieldAttribute.SetEnabled<CROpportunity.curyAmount>(cache, e.Row, row.ManualTotalEntry == true);

			decimal? curyWgtAmount = null;
			var oppProbability = row.StageID.
				With(_ => (CROpportunityProbability)PXSelect<CROpportunityProbability,
					Where<CROpportunityProbability.stageCode, Equal<Required<CROpportunityProbability.stageCode>>>>.
				Select(this, _));
			if (oppProbability != null && oppProbability.Probability != null)
                curyWgtAmount = oppProbability.Probability * (this.Accessinfo.CuryViewState ? row.ProductsAmount : row.CuryProductsAmount) / 100;
			row.CuryWgtAmount = curyWgtAmount;

            if (row.ContactID != null && row.BAccountID != null)
            {
                Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.SelectSingleBound(this, null, row.ContactID);
                if (contact != null && contact.BAccountID != row.BAccountID)
                    Opportunity.Cache.RaiseExceptionHandling<CROpportunity.contactID>(row, row.ContactID,
                        new PXSetPropertyException(Messages.ContractBAccountDiffer, PXErrorLevel.Warning));
                else
                {
                    Opportunity.Cache.RaiseExceptionHandling<CROpportunity.contactID>(row, null, null);
                }
            }

            createAccount.SetEnabled(row.BAccountID == null);
            createContact.SetEnabled(row.ContactID == null || row.AllowOverrideContactAddress == true);
		}

		private object SelectInvoice(string arRefNbr)
		{
			if (string.IsNullOrEmpty(arRefNbr)) return null;
			return (ARInvoice)PXSelectJoin<ARInvoice,
				LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<ARInvoice.customerID>>>,
				Where2<Where<ARInvoice.origModule, Equal<BatchModule.moduleAR>, Or<ARInvoice.released, Equal<True>>>,
					And2<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>>.
				Select(this, arRefNbr);
		}

		private object SelectOrder(string orderNbr)
		{
			if (string.IsNullOrEmpty(orderNbr)) return null;
			return (SOOrder)PXSelectJoin<SOOrder,
				LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<SOOrder.customerID>>>,
				Where2<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>,
					And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.
				Select(this, orderNbr);
		}


		protected virtual void CROpportunity_BAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			if (IsMultyCurrency && (e.ExternalCall || sender.GetValuePending<CROpportunity.curyID>(row) == null))
			{
				CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<CROpportunity.curyInfoID>(sender, e.Row);

				if (info != null)
				{
					row.CuryID = info.CuryID;
				}
			}

			Contact oldContact = null;
			Address oldAddress = null;

			int? oldBAccountID = (int?) e.OldValue;
			int? oldLocationID = null;

			var baccount = (BAccount) PXSelect<BAccount,
				Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(this, oldBAccountID);

			if (baccount != null)
			{
				Location oldLocation = PXSelect<Location, Where<Location.locationID,
					Equal<Required<Location.locationID>>>>.
					Select(this, baccount.DefLocationID);
				if (oldLocation != null)
				{
					oldLocationID = oldLocation.LocationID;
				}
			}
			
			if (row.ContactID != null)
			{
				oldContact = PXSelect<Contact,
					Where<Contact.contactID, Equal<Current<CROpportunity.contactID>>>>.Select(this);

				oldAddress = PXSelectJoin<Address,
					LeftJoin<Contact, On<Contact.defAddressID, Equal<Address.addressID>>>,
					Where<Contact.contactID, Equal<Current<CROpportunity.contactID>>>>.Select(this);
			}

			else if (oldLocationID != null)
			{
				oldContact = PXSelectJoin<Contact,
					LeftJoin<Location, 
						On<Location.locationID, Equal<Required<CROpportunity.locationID>>>>,
					Where<Contact.contactID, Equal<Location.defContactID>>>.Select(this, oldLocationID);

				oldAddress = PXSelectJoin<Address,
					LeftJoin<Contact, 
						On<Contact.defAddressID, Equal<Address.addressID>>,
					LeftJoin<Location, 
						On<Location.locationID, Equal<Required<CROpportunity.locationID>>>>>,
					Where<Address.addressID, Equal<Location.defAddressID>>>.Select(this, oldLocationID);
			}

			if (row.AllowOverrideContactAddress == true && row.BAccountID != null && !IsDefaultContactAdress() &&
			    row.ContactID == null && !IsContactAddressNoChanged(oldContact, oldAddress))
			{
				WebDialogResult dialogResult = this.Setup.Ask(Messages.Warning, Messages.ReplaceContactDetails, MessageButtons.YesNoCancel, MessageIcon.Warning);

				if (dialogResult == WebDialogResult.Yes)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = false;
				}
			}
			else
			{
				if (row.BAccountID != null && row.ContactID == null)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = false;
				}
			}

			if (row.LocationID == null && row.ContactID == null && row.BAccountID == null)
			{
				if (row.AllowOverrideContactAddress == false)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = true;
				}
			}

			if (IsDefaultContactAdress())
			{
				row.AllowOverrideContactAddress = true;
			}
		}

		protected virtual void CROpportunity_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			Contact oldContact = null;
			Address oldAddress = null;

			int? oldLocationID = (int?) e.OldValue;

			if (row.ContactID != null)
			{
				oldContact = PXSelect<Contact,
					Where<Contact.contactID, Equal<Current<CROpportunity.contactID>>>>.Select(this);

				oldAddress = PXSelectJoin<Address,
					LeftJoin<Contact, On<Contact.defAddressID, Equal<Address.addressID>>>,
					Where<Contact.contactID, Equal<Current<CROpportunity.contactID>>>>.Select(this);
			}

			else if (oldLocationID != null)
			{
				oldContact = PXSelectJoin<Contact,
					LeftJoin<Location, 
						On<Location.locationID, Equal<Required<CROpportunity.locationID>>>>,
					Where<Contact.contactID, Equal<Location.defContactID>>>.Select(this, oldLocationID);

				oldAddress = PXSelectJoin<Address,
					LeftJoin<Contact, 
						On<Contact.defAddressID, Equal<Address.addressID>>,
					LeftJoin<Location, 
						On<Location.locationID, Equal<Required<CROpportunity.locationID>>>>>,
					Where<Address.addressID, Equal<Location.defAddressID>>>.Select(this, oldLocationID);
			}

			if (row.AllowOverrideContactAddress == true && row.LocationID != null && row.ContactID == null &&
			    !IsDefaultContactAdress() && !IsContactAddressNoChanged(oldContact, oldAddress))
			{
				WebDialogResult dialogResult = this.Setup.Ask(Messages.Warning, Messages.ReplaceContactDetails, MessageButtons.YesNoCancel, MessageIcon.Warning);

				if (dialogResult == WebDialogResult.Yes)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = false;
				}
			}
			else
			{
				if (row.LocationID != null && row.ContactID == null)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = false;
				}
			}

			if (row.LocationID == null && row.ContactID == null && row.BAccountID == null)
			{
				if (row.AllowOverrideContactAddress == false)
				{
					CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
					CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
					row.AllowOverrideContactAddress = true;
				}
			}

			if (IsDefaultContactAdress())
			{
				row.AllowOverrideContactAddress = true;
			}
		}

		protected virtual void CROpportunity_ContactID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			Contact oldContact = null;
            Address oldAddress = null;

            int? oldContactid = (int?)e.OldValue;

            if (oldContactid != null)
            {
                oldContact = PXSelect<Contact,                
                   Where<Contact.contactID, Equal<Required<CROpportunity.contactID>>>>.Select(this, oldContactid);

                oldAddress = PXSelectJoin<Address,
                    LeftJoin<Contact, On<Contact.defAddressID, Equal<Address.addressID>>>,
                    Where<Contact.contactID, Equal<Required<CROpportunity.contactID>>>>.Select(this, oldContactid);
            }

            else if (row.LocationID != null)
            {
                oldContact = PXSelectJoin<Contact,
                    LeftJoin<Location, On<Location.locationID, Equal<Current<CROpportunity.locationID>>>>,             
                    Where<Contact.contactID, Equal<Location.defContactID>>>.Select(this);

                oldAddress = PXSelectJoin<Address,
                    LeftJoin<Contact, On<Contact.defAddressID, Equal<Address.addressID>>,
                    LeftJoin<Location, On<Location.locationID, Equal<Current<CROpportunity.locationID>>>>>,
                   Where<Address.addressID, Equal<Location.defAddressID>>>.Select(this);                
            }

            if (row != null)
            {
                if (row.AllowOverrideContactAddress == true && (row.LocationID != null || row.ContactID != null) && !IsDefaultContactAdress() && !IsContactAddressNoChanged(oldContact, oldAddress))
                {
                    WebDialogResult dialogResult = this.Setup.Ask(Messages.Warning, Messages.ReplaceContactDetails,
                            MessageButtons.YesNoCancel, MessageIcon.Warning);

                    if (dialogResult == WebDialogResult.Yes)
                    {
                        CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
                        CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
                        row.AllowOverrideContactAddress = false;
                    }                    
                }
                else 
                {
                    if (row.LocationID != null || row.ContactID != null)
                    {
                        CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
                        CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
                        row.AllowOverrideContactAddress = false;
                    }                  
                }

                if (row.LocationID == null && row.ContactID == null && row.BAccountID == null)
                {
                    if (row.AllowOverrideContactAddress == false)
                    {
                        CROpportunityAddressAttribute.DefaultRecord<CROpportunity.opportunityAddressID>(sender, e.Row);
                        CROpportunityContactAttribute.DefaultRecord<CROpportunity.opportunityContactID>(sender, e.Row);
                        row.AllowOverrideContactAddress = true;                                              
                    }                    
                }
            }
            if (IsDefaultContactAdress())
            {
                row.AllowOverrideContactAddress = true;
            }
        }

        protected virtual void CROpportunity_AllowOverrideContactAddress_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			CRAddress address = Opportunity_Address.SelectSingle();
            CRContact contact = Opportunity_Contact.SelectSingle();
            if (address != null)
            {
                address.OverrideAddress = row.AllowOverrideContactAddress;
                Caches[typeof(CRAddress)].Update(address);                
            }

            if (contact != null)
            {
                contact.OverrideContact = row.AllowOverrideContactAddress;
                Caches[typeof(CRContact)].Update(contact);
            }            
        }

        protected virtual void CROpportunity_AllowOverrideContactAddress_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CROpportunity row = e.Row as CROpportunity;
	        if (row == null) return;

            CRAddress address = Opportunity_Address.SelectSingle();
            CRContact contact = Opportunity_Contact.SelectSingle();           

            address.OverrideAddress = row.AllowOverrideContactAddress;
            contact.OverrideContact = row.AllowOverrideContactAddress;
            Caches[typeof(CRAddress)].Update(address);
            Caches[typeof(CRContact)].Update(contact);
		}

		protected virtual void CROpportunity_AllowOverrideContactAddress_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			if (row.BAccountID == null && row.LocationID == null && row.ContactID == null)
				e.ReturnValue = false;
		}

		protected virtual void CROpportunity_BAccountID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			if (row.BAccountID < 0)
				e.ReturnValue = "";
		}

		protected virtual void CROpportunity_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			var row = e.Row as CROpportunity;
			if (row == null) return;

			object newContactId = row.ContactID;
			if (newContactId != null && !VerifyField<CROpportunity.contactID>(row, newContactId))
				row.ContactID = null;

			if (row.ContactID != null)
			{
				object newCustomerId = row.BAccountID;
				if (newCustomerId == null)
					FillDefaultBAccountID(row);
			}

			if (row.BAccountID != null)
			{
				var bAccount = (BAccount)PXSelect<BAccount, 
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(this, row.BAccountID);
				if (bAccount != null && IsMultyCurrency)
				{
					if (bAccount.Type == BAccountType.CustomerType || bAccount.Type == BAccountType.CombinedType)
					{
						Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, row.BAccountID);
						if (customer != null && customer.CuryID != null)
						{
							row.CuryID = customer.CuryID;
						}
					}
				}
			}

			object newLocationId = row.LocationID;
			if (newLocationId == null || !VerifyField<CROpportunity.locationID>(row, newLocationId))
			{
				cache.SetDefaultExt<CROpportunity.locationID>(row);
			}

			if (row.ContactID == null)
				cache.SetDefaultExt<CROpportunity.contactID>(row);

			if (row.TaxZoneID == null) 
				cache.SetDefaultExt<CROpportunity.taxZoneID>(row);
		}


		protected virtual void CROpportunity_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
            var oldRow = e.OldRow as CROpportunity;
            var row = e.Row as CROpportunity;
            if (oldRow == null || row == null) return;

            if (row.ContactID != null && row.ContactID != oldRow.ContactID)
            {
                object newCustomerId = row.BAccountID;
                if (newCustomerId == null)
                    FillDefaultBAccountID(row);
            }

            var customerChanged = row.BAccountID != oldRow.BAccountID;
			if (customerChanged && row.BAccountID != null)
			{
				var bAccount = (BAccount)PXSelect<BAccount,
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(this, row.BAccountID);
				if (bAccount != null && IsMultyCurrency)
				{
					if (bAccount.Type == BAccountType.CustomerType || bAccount.Type == BAccountType.CombinedType)
					{
						Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, row.BAccountID);
						if (customer != null && customer.CuryID != null)
						{
							row.CuryID = customer.CuryID;
						}
					}
				}
			}

			object newLocationId = row.LocationID;
			if (newLocationId == null || !VerifyField<CROpportunity.locationID>(row, newLocationId))
			{
				sender.SetDefaultExt<CROpportunity.locationID>(row);
			}		

			if (customerChanged)
				sender.SetDefaultExt<CROpportunity.taxZoneID>(row);

			var locationChanged = row.LocationID != oldRow.LocationID;
			var closeDateChanged = row.CloseDate != oldRow.CloseDate;
			var projectChanged = row.ProjectID != oldRow.ProjectID;
			if (locationChanged || closeDateChanged || projectChanged || customerChanged)
			{
				var productsCache = Products.Cache;
				foreach (CROpportunityProducts line in SelectProducts(row.OpportunityID))
				{
					var lineCopy = (CROpportunityProducts)productsCache.CreateCopy(line);
					if (locationChanged || closeDateChanged) lineCopy.ManualDisc = false;
					if (locationChanged) lineCopy.OpportunityLocationIDChanged = true;
					if (closeDateChanged) lineCopy.OpportunityCloseDateChanged = true;
					lineCopy.ProjectID = row.ProjectID;
					lineCopy.CustomerID = row.BAccountID;
					productsCache.Update(lineCopy);
				}
			}

			if (row.OwnerID == null)
			{
				row.AssignDate = null;
			}
			else if (oldRow.OwnerID == null)
			{
				row.AssignDate = PXTimeZoneInfo.Now;
			}
		}

		protected virtual void CROpportunity_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			var row = (CROpportunity)e.Row;
			if (row == null) return;

			if ((e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update) && row.BAccountID != null && row.LocationID == null)
			{
				sender.RaiseExceptionHandling<CROpportunity.locationID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
				e.Cancel = true;
			}
		}

		protected virtual void CROpportunity_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			CROpportunity row = e.Row as CROpportunity;
			if (row == null) return;

			if (row.ContactID != null && e.TranStatus == PXTranStatus.Open && 
				 (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update ))
			{
				Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<CROpportunity.contactID>>>>.Select(this, row.ContactID);
				if (contact != null && contact.ContactType == ContactTypesAttribute.Lead)
				{
					contact.ContactType = ContactTypesAttribute.Person;
                    contact.QualificationDate = PXTimeZoneInfo.Now;
                    contact.Status = LeadStatusesAttribute.Converted;
				    contact.MajorStatus = LeadMajorStatusesAttribute._CONVERTED;
				    contact.Resolution = null;
				    contact.ConvertedBy = PXAccess.GetUserID();
					if (contact.DuplicateStatus == DuplicateStatusAttribute.PossibleDuplicated)
					{
						contact.DuplicateStatus = DuplicateStatusAttribute.NotValidated;
					}
					Contacts.Update(contact);
					Save.Press();
				}
			}
		}

		#endregion

		#region CROpportunityProducts

		protected virtual void CROpportunityProducts_DiscPct_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			var row = e.Row as CROpportunityProducts;
			if (row == null) return;

			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<CROpportunityProducts>.ValidateMinGrossProfitPct(sender, row, row.UnitPrice, ref newValue);

			e.NewValue = newValue;
		}

		protected virtual void CROpportunityProducts_CuryDiscAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			var row = e.Row as CROpportunityProducts;
			if (row == null) return;

			decimal? newValue = (decimal?)e.NewValue;
			SODiscountEngine<CROpportunityProducts>.ValidateMinGrossProfitAmt(sender, row, row.UnitPrice, ref newValue);
			e.NewValue = newValue;
		}

		protected virtual void CROpportunityProducts_CuryUnitPrice_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CROpportunityProducts row = e.Row as CROpportunityProducts;
		    if (row != null)
		    {
                if (row.ManualPrice != true && row.InventoryID != null && row.IsFree != true)
                {
                    string customerPriceClass = ARPriceClass.EmptyPriceClass;
                    Location c = location.Select();
                    if (c != null && !string.IsNullOrEmpty(c.CPriceClassID))
                        customerPriceClass = c.CPriceClassID;

                    e.NewValue =
                        ARSalesPriceMaint.CalculateSalesPrice(sender, customerPriceClass, Opportunity.Current.BAccountID,
                            row.InventoryID,
                            currencyinfo.Select(),
                            row.UOM, row.Qty, Accessinfo.BusinessDate.Value, row.CuryUnitPrice) ?? 0m;
                    e.Cancel = true;
                }
                else if (row.ManualPrice == true && row.InventoryID != null && row.IsFree != true)
                    e.NewValue = row.CuryUnitPrice;
		    }		    		     
		}

        protected virtual void CROpportunityProducts_InventoryID_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            CROpportunityProducts row = e.Row as CROpportunityProducts;
            if (row == null) return;

            row.ManualPrice = false;
        }

		protected virtual void CROpportunityProducts_InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CROpportunityProducts row = e.Row as CROpportunityProducts;
			if (row == null) return;

			InventoryItem item = PXSelectorAttribute.Select<CROpportunityProducts.inventoryID>(cache, row) as InventoryItem;
			if (item != null)
			{
				SetDiscounts(cache, row);
				Customer customer = PXSelectReadonly<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);
				row.TransactionDescription = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, nameof(InventoryItem.Descr), customer?.LocaleName); 
				cache.SetDefaultExt<CROpportunityProducts.uOM>(e.Row);
				cache.SetDefaultExt<CROpportunityProducts.curyUnitPrice>(e.Row);
				cache.SetDefaultExt<CROpportunityProducts.siteID>(e.Row);
			}
		}

        protected virtual void CROpportunityProducts_IsFree_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            CROpportunityProducts row = e.Row as CROpportunityProducts;
            if (row == null) return;

            row.ManualPrice = false;
        }

        protected virtual void CROpportunityProducts_IsFree_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            cache.SetDefaultExt<CROpportunityProducts.curyUnitPrice>(e.Row);
        }

        protected virtual void CROpportunityProducts_UOM_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            CROpportunityProducts row = e.Row as CROpportunityProducts;
            if (row == null) return;

            row.ManualPrice = false;
        }

		protected virtual void CROpportunityProducts_UOM_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			cache.SetDefaultExt<CROpportunityProducts.curyUnitPrice>(e.Row);
		}

		protected virtual void CROpportunityProducts_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CROpportunityProducts;
			if (row == null) return;

			PXUIFieldAttribute.SetEnabled<CROpportunityProducts.curyUnitPrice>(sender, row, row.IsFree != true);
            PXUIFieldAttribute.SetEnabled<CROpportunityProducts.manualDisc>(sender, row, row.IsFree != true);

			bool autoFreeItem = row.ManualDisc != true && row.IsFree == true;

			if (autoFreeItem)
			{
				PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<CROpportunityProducts.taxCategoryID>(sender, e.Row);
				PXUIFieldAttribute.SetEnabled<CROpportunityProducts.transactionDescription>(sender, e.Row);
				PXUIFieldAttribute.SetEnabled<CROpportunityProducts.siteID>(sender, e.Row);
			}
			PXUIFieldAttribute.SetEnabled<CROpportunityProducts.quantity>(sender, e.Row, !autoFreeItem);
			PXUIFieldAttribute.SetEnabled<CROpportunityProducts.isFree>(sender, e.Row, !autoFreeItem && row.InventoryID != null);
		}

		protected virtual void CROpportunityProducts_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			var row = e.Row as CROpportunityProducts;
			if (row == null) return;

			if (row.InventoryID != null)
			{
				var item = PXSelectorAttribute.Select<CROpportunityProducts.inventoryID>(sender, row) as InventoryItem;
				if (item != null)
				{
					Customer customer = PXSelectReadonly<Customer, Where<Customer.bAccountID, Equal<Current<CROpportunity.bAccountID>>>>.Select(this);
					row.TransactionDescription = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, nameof(InventoryItem.Descr), customer?.LocaleName); 				
				}
			}

			if (row.Quantity == null) sender.SetValueExt<CROpportunityProducts.quantity>(row, 0m);			
			
			if (row.ManualDisc == true)
			{
				sender.SetValueExt<CROpportunityProducts.curyDiscAmt>(row, (row.CuryUnitPrice ?? 0m) * row.Quantity * (row.DiscPct ?? 0m) * 0.01m);
			}
			else
			{
				sender.SetValueExt<CROpportunityProducts.detDiscIDC1>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscSeqIDC1>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscIDC2>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscSeqIDC2>(row, null);

				CROpportunity opportunity;
				if (row.IsFree != true && row.InventoryID != null && row.CustomerID != null && 
					(opportunity = SelectOpportunity(row.CROpportunityID)) != null && opportunity.CloseDate != null)
				{
					sender.SetValueExt<CROpportunityProducts.curyAmount>(row, (row.CuryUnitPrice ?? 0m) * row.Quantity);

					SODiscountEngine<CROpportunityProducts>.SetDiscounts(sender, row, opportunity.LocationID, (DateTime)opportunity.CloseDate);

					SODiscountEngine<CROpportunityProducts>.CalculateLineDiscounts(sender, row, opportunity.LocationID, (DateTime)opportunity.CloseDate);

					SODiscountEngine<CROpportunityProducts>.UpdateDiscountDetails<CROpportunityDiscountDetail>(sender, Products, DiscountDetails,
						row, opportunity.LocationID, (DateTime)opportunity.CloseDate, ProrateDiscount);

					SODiscountEngine<CROpportunityProducts>.RemoveUnappliableDiscountDetails<CROpportunityDiscountDetail>(Products, DiscountDetails);

					RefreshFreeItemLines(row.CROpportunityID);
				}
				else
				{
					sender.SetValueExt<CROpportunityProducts.curyDiscAmt>(row, 0m);
				}
			}

			if (row.CuryDiscAmt == null) sender.SetValueExt<CROpportunityProducts.curyDiscAmt>(row, 0m);

			sender.SetValueExt<CROpportunityProducts.curyAmount>(row, (row.CuryUnitPrice ?? 0m) * row.Quantity - row.CuryDiscAmt);

			TaxAttribute.Calculate<CROpportunityProducts.taxCategoryID>(sender, e);
		}

        protected virtual void CROpportunityProducts_Quantity_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CROpportunityProducts row = e.Row as CROpportunityProducts;
            if (row != null)
            {
                sender.SetDefaultExt<CROpportunityProducts.curyUnitPrice>(e.Row);
            }
        }

		protected virtual void CROpportunityProducts_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			Debug.WriteLine("CROpportunityProducts_RowUpdated");

			CROpportunityProducts row = e.Row as CROpportunityProducts;
			CROpportunityProducts oldRow = e.OldRow as CROpportunityProducts;
			
			if (row.ManualDisc != true)
			{
				if (oldRow.ManualDisc == true)
				{
					SetDiscounts(sender, (CROpportunityProducts)e.Row);
					SetFlatUnitPrice(sender, row);

					if (row.IsFree == true)
					{
						ResetQtyOnFreeItem(row);
					}
				}

				RecalculateDiscounts(sender, row);
			}
			else
			{
				SODiscountEngine<CROpportunityProducts>.RemoveUnappliableDiscountDetails<CROpportunityDiscountDetail>(Products, DiscountDetails);
			}
			TaxAttribute.Calculate<CROpportunityProducts.taxCategoryID>(sender, e);

            if (sender.ObjectsEqual<CROpportunityProducts.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<CROpportunityProducts.uOM>(e.Row, e.OldRow)
                && sender.ObjectsEqual<CROpportunityProducts.quantity>(e.Row, e.OldRow)
                && sender.ObjectsEqual<CROpportunityProducts.isFree>(e.Row, e.OldRow)
                && sender.ObjectsEqual<CROpportunityProducts.uOM>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<CROpportunityProducts.siteID>(e.Row, e.OldRow)
                && (!sender.ObjectsEqual<CROpportunityProducts.curyUnitPrice>(e.Row, e.OldRow) || !sender.ObjectsEqual<CROpportunityProducts.curyExtPrice>(e.Row, e.OldRow)))
                row.ManualPrice = true;
		}

		protected virtual void CROpportunityProducts_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			var row = e.Row as CROpportunityProducts;
			if (row == null) return;

			bool autoFreeItem = row.ManualDisc != true && row.IsFree == true;
			if (autoFreeItem) return;

			var opportunity = SelectOpportunity(row.CROpportunityID);
			if (opportunity != null && opportunity.CloseDate != null && row.InventoryID != null && row.Quantity != null)
			{
				sender.SetValueExt<CROpportunityProducts.detDiscIDC1>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscSeqIDC1>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscIDC2>(row, null);
				sender.SetValueExt<CROpportunityProducts.detDiscSeqIDC2>(row, null);

				SODiscountEngine<CROpportunityProducts>.UpdateDiscountDetails<CROpportunityDiscountDetail>(sender, Products, DiscountDetails,
					row, opportunity.LocationID, (DateTime)opportunity.CloseDate, ProrateDiscount);

				SODiscountEngine<CROpportunityProducts>.RemoveUnappliableDiscountDetails<CROpportunityDiscountDetail>(Products, DiscountDetails);

				RefreshFreeItemLines(row.CROpportunityID);
			}
		}

		#endregion

		#region CRRelation
		[PXDBGuid]
		[PXDBDefault(typeof(CROpportunity.noteID))]
		public virtual void CRRelation_RefNoteID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region CurrencyInfo

		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!IsMultyCurrency) return;

			if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryID))
			{
				e.NewValue = customer.Current.CuryID;
				e.Cancel = true;
			}
			else if (Setup.Current != null && !string.IsNullOrEmpty(Setup.Current.DefaultCuryID))
			{
				e.NewValue = Setup.Current.DefaultCuryID;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!IsMultyCurrency) return;

			if (customer.Current != null && !string.IsNullOrEmpty(customer.Current.CuryRateTypeID))
			{
				e.NewValue = customer.Current.CuryRateTypeID;
				e.Cancel = true;
			}
			else if (Setup.Current != null && !string.IsNullOrEmpty(Setup.Current.DefaultRateTypeID))
			{
				e.NewValue = Setup.Current.DefaultRateTypeID;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = Accessinfo.BusinessDate;
			e.Cancel = true;
		}

		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info == null) return;

			bool curyenabled = info.AllowUpdate(Opportunity.Cache);

			if ((customer.Current != null && customer.Current.AllowOverrideRate != true) || (Setup.Current != null && Setup.Current.AllowOverrideRate != true))
			{
				curyenabled = false;
			}

			PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyenabled);
			PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyenabled);
			PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyenabled);
			PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyenabled);
		}

		#endregion

		#region CRTaxTran
		protected virtual void CRTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<CRTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);
		}
		#endregion

        #region CROpportunityTax
		protected virtual void CRTaxTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
			CRTaxTran row = e.Row as CRTaxTran;
            if (row == null) return;

            if (e.Operation == PXDBOperation.Delete)
			{
				CROpportunityTax tax = (CROpportunityTax)this.TaxLines.Cache.Locate((CROpportunityTax)row);
				if (this.TaxLines.Cache.GetStatus(tax) == PXEntryStatus.Deleted ||
					 this.TaxLines.Cache.GetStatus(tax) == PXEntryStatus.InsertedDeleted)
                e.Cancel = true;
        }
			if (e.Operation == PXDBOperation.Update)
			{
				CROpportunityTax tax = (CROpportunityTax)this.TaxLines.Cache.Locate((CROpportunityTax)row);
				if (this.TaxLines.Cache.GetStatus(tax) == PXEntryStatus.Updated)
					e.Cancel = true;
			}		
		}
        #endregion

		protected virtual void CreateAccountsFilter_BAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.IsDimensionAutonumbered(CustomerAttribute.DimensionName))
			{
				e.NewValue = this.GetDimensionAutonumberingNewValue(CustomerAttribute.DimensionName);
			}
		}
		protected virtual void CreateAccountsFilter_AccountName_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
            CRContact contact = Opportunity_Contact.SelectSingle();
            if (contact == null) return;
			    e.NewValue = contact.FullName;			
		}

        protected virtual void CreateAccountsFilter_CreateContact_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            CRContact contact = Opportunity_Contact.SelectSingle();
            if (contact == null) return;
			e.NewValue = contact.FullName;
		}

        protected virtual void CreateAccountsFilter_AccountClass_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            CROpportunity opportunity = Opportunity.Current;
            if (opportunity == null) return;

            CROpportunityClass opcls = PXSelect<CROpportunityClass, 
                                            Where<CROpportunityClass.cROpportunityClassID, 
                                            Equal<Required<CROpportunity.cROpportunityClassID>>>>.Select(this, opportunity.CROpportunityClassID);
            if (opcls != null )
            {
                if (opcls.TargetBAccountClassID == null)
                {
                    Contact contact = sender.Graph.Caches<Contact>().Current as Contact;

                    if (contact != null)
                    {
                        CRCustomerClass cls = PXSelect<CRCustomerClass,
                            Where<CRCustomerClass.cRCustomerClassID,
                                Equal<Required<Contact.classID>>>>.Select(this, contact.ClassID);

                        if (cls != null)
                        {
                            e.NewValue = cls.CRCustomerClassID;
                        }
                    }
                }
                else
                {
                    e.NewValue = opcls.TargetBAccountClassID;
                }
            }
        }

		protected virtual void CreateAccountsFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
		    CreateAccountsFilter row = e.Row as CreateAccountsFilter;            

            if (row != null)
		    {
                PXUIFieldAttribute.SetEnabled<CreateAccountsFilter.bAccountID>(sender, row, !this.IsDimensionAutonumbered(CustomerAttribute.DimensionName));

                CROpportunity opportunity = Opportunity.Current;
                if (opportunity.ContactID != null)
                {
                    PXUIFieldAttribute.SetVisible<CreateAccountsFilter.linkContactToAccount>(sender, row, true);
                    Contact _Contact = CurrentContact.SelectSingle();
                    if (_Contact == null)
                    {
                        row.LinkContactToAccount = false;
                        PXUIFieldAttribute.SetEnabled<CreateAccountsFilter.linkContactToAccount>(sender, row, false);
                    }
                    else
                    {
                        if (_Contact.BAccountID != null)
                        {
                            PXUIFieldAttribute.SetWarning<CreateAccountsFilter.linkContactToAccount>(sender, row, Messages.AccountContactValidation);
                            row.LinkContactToAccount = false;
                        }
                        else
                        {
                            PXUIFieldAttribute.SetEnabled<CreateAccountsFilter.linkContactToAccount>(sender, row, true);
                        }
                    }                    
                }                
            }
		}
		#endregion

        #region CacheAttached

        [PXBool]
        [PXDefault(false)]
        [PXDBCalced(typeof(True), typeof(Boolean))]
        protected virtual void BAccountR_ViewInCrm_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region Private Methods

        private BAccount SelectAccount(string acctCD)
		{
			if (string.IsNullOrEmpty(acctCD)) return null;
			return (BAccount)PXSelectReadonly<BAccount,
				Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>.
				Select(this, acctCD);
		}

		private bool VerifyField<TField>(object row, object newValue)
			where TField : IBqlField
		{
			if (row == null) return true;

			var result = false;
			var cache = Caches[row.GetType()];
			try
			{
				result = cache.RaiseFieldVerifying<TField>(row, ref newValue);
			}
			catch (StackOverflowException) { throw; }
			catch (OutOfMemoryException) { throw; }
			catch (Exception) { }

			return result;
		}

		private void FillDefaultBAccountID(CROpportunity row)
		{
			if (row == null) return;

			if (row.ContactID != null)
			{
				var contact = (Contact)PXSelectReadonly<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
					Select(this, row.ContactID);
				if (contact != null)
				{
					row.BAccountID = contact.BAccountID;
					row.ParentBAccountID = contact.ParentBAccountID;
				}
			}

		}

		private bool IsMultyCurrency
		{
			get { return PXAccess.FeatureInstalled<FeaturesSet.multicurrency>(); }
		}

		private bool ProrateDiscount
		{
			get
			{
				SOSetup sosetup = PXSelect<SOSetup>.Select(this);

				if (sosetup == null)
					return true; //default true

				if (sosetup.ProrateDiscounts == null)
					return true;

				return sosetup.ProrateDiscounts == true;
			}
		}

		private CROpportunity SelectOpportunity(string opportunityId)
		{
			if (opportunityId == null) return null;

			var opportunity = (CROpportunity)PXSelect<CROpportunity,
				Where<CROpportunity.opportunityID, Equal<Required<CROpportunity.opportunityID>>>>.
				Select(this, opportunityId);
			return opportunity;
		}

		private IEnumerable SelectProducts(object opportunityId)
		{
			if (opportunityId == null)
				return new CROpportunityProducts[0];

			return PXSelect<CROpportunityProducts,
				Where<CROpportunityProducts.cROpportunityID, Equal<Required<CROpportunity.opportunityID>>>>.
				Select(this, opportunityId).
				RowCast<CROpportunityProducts>();
		}

		private IEnumerable SelectDiscountDetails(object opportunityId)
		{
			if (opportunityId == null)
				return new CROpportunityDiscountDetail[0];

			return PXSelect<CROpportunityDiscountDetail,
				Where<CROpportunityDiscountDetail.opportunityID, Equal<Required<CROpportunity.opportunityID>>>>.
				Select(this, opportunityId).
				RowCast<CROpportunityDiscountDetail>();
		}

		private void RefreshFreeItemLines(object opportunityId)
		{
			var groupedByInventory = new Dictionary<int, decimal>();

			foreach (CROpportunityDiscountDetail item in SelectDiscountDetails(opportunityId))
			{
				if (item.FreeItemID == null) continue;
				
				var itemId = (int)item.FreeItemID;
				if (groupedByInventory.ContainsKey(itemId))
				{
					groupedByInventory[itemId] += item.FreeItemQty ?? 0;
				}
				else
				{
					groupedByInventory.Add(itemId, item.FreeItemQty ?? 0);
				}
			}

			bool freeItemChanged = false;
			foreach (CROpportunityProducts line in SelectProducts(opportunityId))
			{
				if (line.IsFree != true || line.ManualDisc == true || line.InventoryID == null) continue;
					
				var inventoryId = (int)line.InventoryID;
				decimal itemsQty;
				var isFinded = false;
				if (!(isFinded = groupedByInventory.TryGetValue(inventoryId, out itemsQty)) || itemsQty == 0)
				{
					Products.Delete(line);
					freeItemChanged = true;
					if (isFinded) groupedByInventory.Remove(inventoryId);
				}
				else 
				{
					if (line.Quantity != itemsQty)
					{
						var rowCopy = (CROpportunityProducts)Products.Cache.CreateCopy(line);
						rowCopy.Quantity = itemsQty;
						Products.Update(rowCopy);
						freeItemChanged = true;
					}
					groupedByInventory.Remove(inventoryId);
				}
			}

			foreach (KeyValuePair<int, decimal> kv in groupedByInventory)
			{
				var inventoryId = kv.Key;
				var quantity = kv.Value;
				if (quantity > 0m)
				{
					var line = (CROpportunityProducts)Products.Cache.CreateInstance();
					line.InventoryID = inventoryId;
					line.IsFree = true;
					line.Quantity = quantity;

					Products.Insert(line);
					freeItemChanged = true;
				}
			}

			if (freeItemChanged)
			{
				Products.View.RequestRefresh();
			}
		}

		private void ResetQtyOnFreeItem(CROpportunityProducts line)
		{
			decimal? qtyTotal = 0;
			foreach (CROpportunityDiscountDetail item in
				PXSelect<CROpportunityDiscountDetail,
				Where<CROpportunityDiscountDetail.opportunityID, Equal<Required<CROpportunityDiscountDetail.opportunityID>>,
					And<CROpportunityDiscountDetail.freeItemID, Equal<Required<CROpportunityDiscountDetail.freeItemID>>>>>.
				Select(this, line.CROpportunityID, line.InventoryID))
			{
				if (item.FreeItemID != null && item.FreeItemQty != null && item.FreeItemQty > 0)
					qtyTotal += item.FreeItemQty;
			}

			Caches[typeof(CROpportunityDiscountDetail)].SetValueExt<CROpportunityProducts.quantity>(line, qtyTotal);
		}

		private void SetDiscounts(PXCache sender, CROpportunityProducts row)
		{
			if (row != null && row.InventoryID != null
							&& row.IsFree != true)
			{
				SODiscountEngine<CROpportunityProducts>.SetDiscounts(sender, row, Opportunity.Current.LocationID, Opportunity.Current.CloseDate.Value);
				SODiscountEngine<CROpportunityProducts>.RemoveUnappliableDiscountDetails<CROpportunityDiscountDetail>(Products, DiscountDetails);
			}
		}

		/// <summary>
		/// Finds Flat price in Discounts and sets UnitPrice.
		/// True if Flat price is applicable for the given line; otherwise false.
		/// </summary>
		private bool SetFlatUnitPrice(PXCache sender, CROpportunityProducts row)
		{
			decimal? curyFlatPrice = GetFlatUnitPrice(sender, row);
			if (curyFlatPrice != null)
			{
				sender.SetValueExt<CROpportunityProducts.curyUnitPrice>(row, curyFlatPrice);
				return true;
			}

			return false;
		}

		private decimal? GetFlatUnitPrice(PXCache sender, CROpportunityProducts row)
		{
			if (row.InventoryID != null && row.UOM != null && SODiscountEngine<CROpportunityProducts>.IsFlatPriceApplicable(sender, row))
			{
				return SODiscountEngine<CROpportunityProducts>.CalculateFlatPrice(sender, row, Opportunity.Current.LocationID, Opportunity.Current.CloseDate.Value);
			}

			return null;
		}

		private void RecalculateDiscounts(PXCache sender, CROpportunityProducts line)
		{
			if (line.InventoryID != null && line.Qty != null && line.CuryLineAmt != null && line.IsFree != true && line.InventoryID != null)
			{
				if (line.ManualDisc == false)
					SODiscountEngine<CROpportunityProducts>.CalculateLineDiscounts(sender, line, Opportunity.Current.LocationID, Opportunity.Current.CloseDate.Value);

				SODiscountEngine<CROpportunityProducts>.UpdateDiscountDetails<CROpportunityDiscountDetail>(sender, Products, DiscountDetails, line, Opportunity.Current.LocationID, Opportunity.Current.CloseDate.Value, ProrateDiscount);

				RefreshFreeItemLines(line.CROpportunityID);
			}
		}

        private Contact FillFromOpportunityContact(Contact Contact)
        {
            CRContact _CRContact = Opportunity_Contact.SelectSingle();

            Contact.FullName = _CRContact.FullName;
            Contact.Title = _CRContact.Title;
            Contact.FirstName = _CRContact.FirstName;
            Contact.LastName = _CRContact.LastName;
            Contact.Salutation = _CRContact.Salutation;
            Contact.EMail = _CRContact.Email;
            Contact.WebSite = _CRContact.WebSite;
            Contact.Phone1 = _CRContact.Phone1;
            Contact.Phone1Type = _CRContact.Phone1Type;
            Contact.Phone2 = _CRContact.Phone2;
            Contact.Phone2Type = _CRContact.Phone2Type;
            Contact.Phone3 = _CRContact.Phone3;
            Contact.Phone3Type = _CRContact.Phone3Type;
            Contact.Fax = _CRContact.Fax;
            Contact.FaxType = _CRContact.FaxType;
            return Contact;
        }
        private Address FillFromOpportunityAddress(Address Address)
        {
            CRAddress _CRAddress = Opportunity_Address.SelectSingle();

            Address.AddressLine1 = _CRAddress.AddressLine1;
            Address.AddressLine2 = _CRAddress.AddressLine2;
            Address.City = _CRAddress.City;
            Address.CountryID = _CRAddress.CountryID;
            Address.State = _CRAddress.State;
            Address.PostalCode = _CRAddress.PostalCode;
            return Address;
        }

        private bool IsDefaultContactAdress()
        {
            CRAddress _CRAddress = Opportunity_Address.SelectSingle();
            CRContact _CRContact = Opportunity_Contact.SelectSingle();

            if (_CRContact != null && _CRAddress != null)
            {
                bool IsDirtya = Opportunity_Address.Cache.IsDirty;
                bool IsDirtyc = Opportunity_Contact.Cache.IsDirty;

                CRAddress _etalonCRAddress = Opportunity_Address.Insert();
                CRContact _etalonCRContact = Opportunity_Contact.Insert();

                Opportunity_Address.Cache.SetStatus(_etalonCRAddress, PXEntryStatus.Held);
                Opportunity_Contact.Cache.SetStatus(_etalonCRContact, PXEntryStatus.Held);

                Opportunity_Address.Cache.IsDirty = IsDirtya;
                Opportunity_Contact.Cache.IsDirty = IsDirtyc;

                if (_CRContact.FullName != _etalonCRContact.FullName)
                    return false;
                if (_CRContact.Title != _etalonCRContact.Title)
                    return false;
                if (_CRContact.FirstName != _etalonCRContact.FirstName)
                    return false;
                if (_CRContact.LastName != _etalonCRContact.LastName)
                    return false;
                if (_CRContact.Salutation != _etalonCRContact.Salutation)
                    return false;
                if (_CRContact.Email != _etalonCRContact.Email)
                    return false;
                if (_CRContact.Phone1 != _etalonCRContact.Phone1)
                    return false;
                if (_CRContact.Phone1Type != _etalonCRContact.Phone1Type)
                    return false;
                if (_CRContact.Phone2 != _etalonCRContact.Phone2)
                    return false;
                if (_CRContact.Phone2Type != _etalonCRContact.Phone2Type)
                    return false;
                if (_CRContact.Phone3 != _etalonCRContact.Phone3)
                    return false;
                if (_CRContact.Phone3Type != _etalonCRContact.Phone3Type)
                    return false;
                if (_CRContact.Fax != _etalonCRContact.Fax)
                    return false;
                if (_CRContact.FaxType != _etalonCRContact.FaxType)
                    return false;

                if (_CRAddress.AddressLine1 != _etalonCRAddress.AddressLine1)
                    return false;
                if (_CRAddress.AddressLine2 != _CRAddress.AddressLine2)
                    return false;
                if (_CRAddress.City != _CRAddress.City)
                    return false;
                if (_CRAddress.State != _CRAddress.State)
                    return false;
                if (_CRAddress.CountryID != _CRAddress.CountryID)
                    return false;
                if (_CRAddress.PostalCode != _CRAddress.PostalCode)
                    return false;                
            }
            return true;
        }

        private bool IsContactAddressNoChanged(Contact _etalonCRContact, Address _etalonCRAddress)
        {
            if (_etalonCRContact == null || _etalonCRAddress == null)
            {
                return false;
            }

            CRAddress _CRAddress = Opportunity_Address.SelectSingle();
            CRContact _CRContact = Opportunity_Contact.SelectSingle();

            if (_CRContact != null && _CRAddress != null)
            {
                if (_CRContact.FullName != _etalonCRContact.FullName)
                    return false;
                if (_CRContact.Title != _etalonCRContact.Title)
                    return false;
                if (_CRContact.LastName != _etalonCRContact.LastName)
                    return false;
                if(_CRContact.FirstName != _etalonCRContact.FirstName)
                    return false;
                if (_CRContact.Salutation != _etalonCRContact.Salutation)
                    return false;
                if (_CRContact.Email != _etalonCRContact.EMail)
                    return false;
                if (_CRContact.Phone1 != _etalonCRContact.Phone1)
                    return false;
                if (_CRContact.Phone1Type != _etalonCRContact.Phone1Type)
                    return false;
                if (_CRContact.Phone2 != _etalonCRContact.Phone2)
                    return false;
                if (_CRContact.Phone2Type != _etalonCRContact.Phone2Type)
                    return false;
                if (_CRContact.Phone3 != _etalonCRContact.Phone3)
                    return false;
                if (_CRContact.Phone3Type != _etalonCRContact.Phone3Type)
                    return false;
                if (_CRContact.Fax != _etalonCRContact.Fax)
                    return false;
                if (_CRContact.FaxType != _etalonCRContact.FaxType)
                    return false;

                if (_CRAddress.AddressLine1 != _etalonCRAddress.AddressLine1)
                    return false;
                if (_CRAddress.AddressLine2 != _etalonCRAddress.AddressLine2)
                    return false;
                if (_CRAddress.City != _etalonCRAddress.City)
                    return false;
                if (_CRAddress.State != _etalonCRAddress.State)
                    return false;
                if (_CRAddress.CountryID != _etalonCRAddress.CountryID)
                    return false;
                if (_CRAddress.PostalCode != _etalonCRAddress.PostalCode)
                    return false;
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Avalara Tax

        protected bool IsExternalTax
		{
			get
			{
				if (Opportunity.Current == null)
					return false;

				return AvalaraMaint.IsExternalTax(this, Opportunity.Current.TaxZoneID);
			}
		}

		public bool SkipAvalaraTaxProcessing { get; set; }

		#endregion

		public override void Persist()
		{
			base.Persist();

			if (Opportunity.Current != null && IsExternalTax == true && !SkipAvalaraTaxProcessing && Opportunity.Current.IsTaxValid != true)
			{
				PXLongOperation.StartOperation(this, delegate()
				{
					CROpportunity doc = new CROpportunity {OpportunityID = Opportunity.Current.OpportunityID};
					CRExternalTaxCalc.Process(doc);
				});
			}
		}	  

        #region Implementation of IPXPrepareItems

        public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (string.Compare(viewName, "Products", true) == 0)
            {
                if (values.Contains("cROpportunityID"))
                    values["cROpportunityID"] = Opportunity.Current.OpportunityID;
                else
                    values.Add("cROpportunityID", Opportunity.Current.OpportunityID);
            }

            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return row == null;
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return oldRow == null;
        }

        public virtual void PrepareItems(string viewName, IEnumerable items)
        {
        }

        #endregion
	}
}
