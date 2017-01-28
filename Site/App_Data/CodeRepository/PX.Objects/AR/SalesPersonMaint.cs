using System;
using PX.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;

namespace PX.Objects.AR
{
	public class SalesPersonMaint : PXGraph<SalesPersonMaint, SalesPerson>
	{
		#region Ctor+ Public Members
		public SalesPersonMaint() 
		{
			ARSetup setup = ARSetup.Current;
			this.CommissionsHistory.Cache.AllowInsert = false;
			this.CommissionsHistory.Cache.AllowDelete= false;
			this.CommissionsHistory.Cache.AllowUpdate = false;
			PXUIFieldAttribute.SetEnabled<CustSalesPeople.locationID>(this.SPCustomers.Cache, null, false);

			PXUIFieldAttribute.SetDisplayName<Contact.salutation>(Caches[typeof(Contact)], CR.Messages.Attention);
		}


		public PXSelect<SalesPerson> Salesperson;

		public PXSelect<SalesPerson, Where<SalesPerson.salesPersonID, Equal<Current<SalesPerson.salesPersonID>>>> SalespersonCurrent;
		public PXSelectJoin<CustSalesPeople,
			InnerJoinSingleTable<Customer, On<Customer.bAccountID, Equal<CustSalesPeople.bAccountID>>>,
			Where<CustSalesPeople.salesPersonID, Equal<Current<SalesPerson.salesPersonID>>,
			And<Match<Customer, Current<AccessInfo.userName>>>>> SPCustomers;
		public PXSelectGroupBy<ARSPCommnHistory, Where<ARSPCommnHistory.salesPersonID, Equal<Current<SalesPerson.salesPersonID>>>,
							   Aggregate<Sum<ARSPCommnHistory.commnAmt, 
										 Sum<ARSPCommnHistory.commnblAmt,
										 GroupBy<ARSPCommnHistory.commnPeriod, Max<ARSPCommnHistory.pRProcessedDate>>>>>> CommissionsHistory;
		#endregion
		#region Sub-screen Navigation Buttons
		public PXAction<SalesPerson> viewDetails;
		[PXUIField(DisplayName = "View Details", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable ViewDetails(PXAdapter adapter)
		{
			if (this.CommissionsHistory.Current != null)
			{
				ARSPCommnHistory current = this.CommissionsHistory.Current;
				ARSPCommissionDocEnq graph = PXGraph.CreateInstance<ARSPCommissionDocEnq>();
				SPDocFilter filter = graph.Filter.Current;
				filter.SalesPersonID = current.SalesPersonID;
				filter.CommnPeriod = current.CommnPeriod;
				graph.Filter.Update(filter);
				throw new PXRedirectRequiredException(graph, "Document");
			}
			return adapter.Get();
		}
		#endregion
		#region SalesPerson Events
		protected virtual void SalesPerson_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			SalesPerson row = (SalesPerson)e.Row;
			PXSelectBase<ARSalesPerTran> sel = new PXSelect<ARSalesPerTran, Where<ARSalesPerTran.salespersonID, Equal<Required<ARSalesPerTran.salespersonID>>>>(this);
			ARSalesPerTran tran = (ARSalesPerTran)sel.View.SelectSingle(row.SalesPersonID);
			if (tran != null)
			{
				throw new PXException(Messages.SalesPersonWithHistoryMayNotBeDeleted);
			}
		}
		#endregion	
		#region OldCode - contact
		#if (ALLOW_EDIT_CONTACT)
		public PXSelect<Contact, Where<Contact.bAccountID, Equal<Current<SalesPerson.bAccountID>>,
								And<Contact.contactID, Equal<Current<SalesPerson.contactID>>>>> Contact;

		public PXSelect<Address, Where<Address.bAccountID, Equal<Current<Contact.bAccountID>>,
						And<Address.addressID, Equal<Current<Contact.defAddressID>>>>> Address;

		
		
		#region Contact Events
		protected virtual void SalesPerson_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			SalesPerson row = e.Row as SalesPerson;
			bool allowChangeContact = false;
			if (row != null)
			{
				if (row.HasContact==null)
				{
					row.HasContact = row.ContactID.HasValue;
				}
				allowChangeContact = !(row.HasContact.Value); 
				bool isNewRecord = this.isNewRecord(row);
				PXEntryStatus status = this.Salesperson.Cache.GetStatus(row);
				bool isInserted = (status == PXEntryStatus.Inserted);
				if (!allowChangeContact)
					row.CreateNewContact = false;
				if (row.ContactID == null && (row.CreateNewContact ?? false))
				{
					SelectedContact contact = new SelectedContact();
					contact.BAccountID = row.BAccountID;
					Contact cnt = (Contact)this.Contact.Cache.Insert(contact);
					if (cnt != null)
						row.ContactID = cnt.ContactID;
					if (allowChangeContact)
					{
						this.Contact.Cache.IsDirty = false;
						this.Salesperson.Cache.IsDirty = false;
					}
				}
				PXUIFieldAttribute.SetVisible<SalesPerson.contactID>(cache, row, (allowChangeContact));
				PXUIFieldAttribute.SetVisible<SalesPerson.createNewContact>(cache, row, (allowChangeContact));
				PXUIFieldAttribute.SetEnabled<SalesPerson.contactID>(cache, row, (allowChangeContact && (!(row.CreateNewContact ?? false))));
				PXUIFieldAttribute.SetEnabled(this.Contact.Cache, this.Contact.Current, (row.CreateNewContact ?? false)||(row.ContactID !=null) );
				PXUIFieldAttribute.SetEnabled(this.Address.Cache, null, (row.CreateNewContact ?? false) || (row.ContactID != null));
			}	
		}

		protected virtual void SalesPerson_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			SalesPerson row = e.Row as SalesPerson;
			if (row != null)
			{
				if (row.ContactID.HasValue)
				{
					CS.Contact cnt = this.Contact.Current;
					if (cnt.ContactID == row.ContactID && cnt.ContactType != CS.Constants.ContactTypes.SalesPerson)
					{
						cnt.ContactType = CS.Constants.ContactTypes.SalesPerson;
						this.Contact.Cache.Update(cnt);
					}
				}
			}
		}
		protected virtual void SalesPerson_RowPersisted(PXCache cache, PXRowPersistedEventArgs e)
		{
			SalesPerson row = e.Row as SalesPerson;
			if (e.TranStatus == PXTranStatus.Completed)
			{
				if (row != null)
				{
					row.HasContact = row.ContactID.HasValue;
                    Salesperson.Current = row;
				}
			}
		}
		protected virtual void Contact_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;
            
			Contact row = e.Row as Contact;
			BAccount acct = BAccountUtility.FindAccount(this, row.BAccountID);
			if (acct != null)
			{
				SelectedContact selContact = row as SelectedContact;
				if (!row.DefAddressID.HasValue)
				{
					row.DefAddressID = acct.DefAddressID; //Set default value
				}
				if (selContact != null)
				{
					selContact.IsAddressSameAsMain = (row.DefAddressID == acct.DefAddressID);
				}
			}
			PXUIFieldAttribute.SetVisible<Contact.bAccountID>(cache, row, false);
			PXUIFieldAttribute.SetVisible<Contact.contactID>(cache, row, false);
			PXUIFieldAttribute.SetVisible<Contact.defAddressID>(cache, row, false);
			
		}
		protected virtual void Contact_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			Contact row = e.Row as Contact;
			BAccount acct = BAccountUtility.FindAccount(this, row.BAccountID); ;
			if (acct != null)
			{
				if (row.DefAddressID.HasValue && row.DefAddressID != acct.DefAddressID)
				{
					Address addr = (Address)this.Address.Current;
					if (row.DefAddressID == addr.AddressID)
						this.Address.Delete(addr);
				}
			}
		}
		protected virtual void Contact_ContactID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			Contact row = (Contact)e.Row;
			e.NewValue = BAccountUtility.GetNextContactID(this, row.BAccountID);
		}
		protected virtual void Contact_ContactType_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			Contact row = (Contact)e.Row;
			e.NewValue = CS.Constants.ContactTypes.SalesPerson;
		}
		protected virtual void Contact_ContactType_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			Contact row = (Contact)e.Row;
			if ((string)e.NewValue != CS.Constants.ContactTypes.SalesPerson)
			{
				e.NewValue = CS.Constants.ContactTypes.SalesPerson;
			}
		}
		
		protected virtual void Contact_ContactID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			Contact row = (Contact)e.Row;
			if ((int)e.NewValue == -1)
			{
				e.NewValue = BAccountUtility.GetNextContactID(this, row.BAccountID);
			}
		}
		protected virtual void Contact_DefAddressID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			Contact row = e.Row as Contact;
			if (row != null)
			{
				BAccount acct = BAccountUtility.FindAccount(this, row.BAccountID); ;
				if (acct != null)
				{
					e.NewValue = acct.DefAddressID;
				}
			}
		}
		public virtual void Contact_IsAddressSameAsMain_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			SelectedContact row = (SelectedContact)e.Row;
			if (row.IsAddressSameAsMain.HasValue)
			{
				if (row.IsAddressSameAsMain == true)
				{
					BAccount account = BAccountUtility.FindAccount(this, row.BAccountID);
					Address addr = this.Address.Current;
					if (account != null)
					{
						if (row.DefAddressID != account.DefAddressID)
						{
							if (addr.AddressID == row.DefAddressID)
							{
								this.Address.Cache.Delete(addr);
							}
							row.DefAddressID = account.DefAddressID;
							this.Contact.View.RequestRefresh();
						}
					}
				}
				else
				{
					Address defAddress = this.Address.Current;
					Address addr = PXCache<Address>.CreateCopy(defAddress);
					addr.BAccountID = row.BAccountID;
					addr.AddressID = null;
					//Copy from default here
					addr = (Address)this.Address.Cache.Insert(addr);
					row.DefAddressID = addr.AddressID;
					this.Contact.View.RequestRefresh();
				}
			}
		}
        #endregion

        #region Address Events
		protected virtual void Address_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			Address row = e.Row as Address;
			BAccount acct = BAccountUtility.FindAccount(this, row.BAccountID); ;
			bool isSameAsMain = false;
			if (acct != null)
			{
				isSameAsMain = (row.AddressID == acct.DefAddressID);
			}
			PXUIFieldAttribute.SetEnabled(cache, row, !isSameAsMain);
			PXUIFieldAttribute.SetVisible<Address.addressID>(cache, row);
			PXUIFieldAttribute.SetVisible<Address.bAccountID>(cache, row);
		}
		protected virtual void Address_AddressID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			Address row = e.Row as Address;
			e.NewValue = BAccountUtility.GetNextAddressID(this, row.BAccountID);
		}
        #endregion


		protected virtual bool isNewRecord(SalesPerson row)
		{
			return string.IsNullOrEmpty(row.SalesPersonCD);
		}
		
		public virtual void SalesPerson_CreateNewContact_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			SalesPerson row = (SalesPerson)e.Row;
			if ((row.CreateNewContact??false) ==true) 
			{
				row.ContactID = null;
			}
			else
			{
				CS.Contact cnt = this.Contact.Current;
				if (cnt.ContactID == row.ContactID)
				{
					if (this.Contact.Cache.GetStatus(cnt) == PXEntryStatus.Inserted)
					{
						this.Contact.Delete(cnt);
						row.ContactID = null;
					}
				}
				
			}
		}
#endif
        #endregion
        #region CustSalesPeople Events
        protected virtual void CustSalesPeople_RowInserting(PXCache cache, PXRowInsertingEventArgs e) 
		{
			CustSalesPeople row = (CustSalesPeople)e.Row;
			if (row != null && row.BAccountID.HasValue)
			{
				List<CustSalesPeople> current = new List<CustSalesPeople>();
				bool duplicated = false;
				foreach (CustSalesPeople iSP in this.SPCustomers.Select())
				{
					if (row.BAccountID == iSP.BAccountID)
					{
						current.Add(iSP);
						if (row.LocationID == iSP.LocationID)
							duplicated = true;
					}
				}
				if (duplicated)
				{
					Location freeLocation = null;
					PXSelectBase<Location> sel = new PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>>>(this);
					foreach (Location iLoc in sel.Select(row.BAccountID))
					{
						bool found = current.Exists(new Predicate<CustSalesPeople>(delegate(CustSalesPeople op) { return (op.LocationID == iLoc.LocationID); }));
						if (!found)
						{
							freeLocation = iLoc;
							break;
						}
					}
					if (freeLocation != null)
					{
						row.LocationID = freeLocation.LocationID;
					}
					else
					{
						throw new PXException(Messages.AllCustomerLocationsAreAdded);
						//cache.RaiseExceptionHandling<CustSalesPeople.locationID>(e.Row, null, new PXException(Messages.SalesPersonAddedForAllLocations));
						//e.Cancel = true;
					}
				}
			}
		}
		#if (false)
		protected virtual void CustSalesPeople_LocationID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CustSalesPeople row = (CustSalesPeople)e.Row;
			int? id = (int?)e.NewValue;
			if (id.HasValue)
			{
				foreach (CustSalesPeople iSP in this.SPCustomers.Select())
				{
					if (object.ReferenceEquals(e.Row, iSP)) continue;
					if ((iSP.BAccountID == row.BAccountID) && (iSP.LocationID == id))
					{
						throw new PXException("This Customer&Location  is already included");
					}
				}
			}
		}
		#endif	

		#endregion
		public PXSetup<ARSetup> ARSetup;
	}
}
