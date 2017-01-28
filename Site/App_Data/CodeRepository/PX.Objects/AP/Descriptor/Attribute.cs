using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PX.Data;

using PX.Objects.GL;
using PX.Objects.TX;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.EP;

using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;

namespace PX.Objects.AP
{
    /// <summary>
    /// Specialized for AP version of the Address attribute.<br/>
    /// Uses APAddress tables for Address versions storage <br/>
    /// Prameters AddressID, IsDefault address are assigned to the <br/>
    /// corresponded fields in the APAddress table. <br/>
    /// Cache for APAddress must be present in the graph <br/>
    /// Depends upon row instance.
    /// <example>
    /// [APAddress(typeof(Select2<Location,
	///	    InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
	///		InnerJoin<Address, On<Address.addressID, Equal<Location.remitAddressID>, 
    ///		    And<Where<Address.bAccountID, Equal<Location.bAccountID>, 
    ///		    Or<Address.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
	///		LeftJoin<APAddress, On<APAddress.vendorID, Equal<Address.bAccountID>, 
    ///		    And<APAddress.vendorAddressID, Equal<Address.addressID>, 
    ///		    And<APAddress.revisionID, Equal<Address.revisionID>, 
    ///		    And<APAddress.isDefaultAddress, Equal<True>>>>>>>>,
	///		Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, 
    ///		    And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>>>>))]
    /// </example>
    /// </summary>
	public class APAddressAttribute : AddressAttribute
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SelectType">Must have type IBqlSelect. This select is used for both selecting <br/> 
        /// a source Address record from which AP address is defaulted and for selecting default version of APAddress, <br/>
        /// created  from source Address (having  matching ContactID, revision and IsDefaultContact = true) <br/>
        /// if it exists - so it must include both records. See example above. <br/>
        /// </param>
		public APAddressAttribute(Type SelectType)
			: base(typeof(APAddress.addressID), typeof(APAddress.isDefaultAddress), SelectType)
		{
		}
        
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldVerifying.AddHandler<APAddress.overrideAddress>(Record_Override_FieldVerifying);
		}

		public override void DefaultRecord(PXCache sender, object DocumentRow, object Row)
		{
			DefaultAddress<APAddress, APAddress.addressID>(sender, DocumentRow, Row);
		}

		public override void CopyRecord(PXCache sender, object DocumentRow, object SourceRow, bool clone)
		{
			CopyAddress<APAddress, APAddress.addressID>(sender, DocumentRow, SourceRow, clone);
		}

		public override void Record_IsDefault_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
		}

		public virtual void Record_Override_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.NewValue = (e.NewValue == null ? e.NewValue : (bool?)e.NewValue == false);
			try
			{
				Address_IsDefaultAddress_FieldVerifying<APAddress>(sender, e);
			}
			finally
			{
				e.NewValue = (e.NewValue == null ? e.NewValue : (bool?)e.NewValue == false);
			}
		}

		protected override void Record_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.Record_RowSelected(sender, e);
			if (e.Row != null)
			{
				PXUIFieldAttribute.SetEnabled<APAddress.overrideAddress>(sender, e.Row, sender.AllowUpdate);
				PXUIFieldAttribute.SetEnabled<APAddress.isValidated>(sender, e.Row, false);
			}			
		}
	}

    /// <summary>
    /// Specialized for AP version of the Contact attribute.<br/>
    /// Uses APContact tables for Contact versions storage <br/>
    /// Parameters ContactID, IsDefaultContact are assigned to the <br/>
    /// corresponded fields in the APContact table. <br/>
    /// Cache for APContact must be present in the graph <br/>
    /// Depends upon row instance.
    /// <example>
    /// [APContact(typeof(Select2<Location,
    ///		InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
	///		InnerJoin<Contact, On<Contact.contactID, Equal<Location.remitContactID>, 
    ///		    And<Where<Contact.bAccountID, Equal<Location.bAccountID>, 
    ///		    Or<Contact.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
	///		LeftJoin<APContact, On<APContact.vendorID, Equal<Contact.bAccountID>, 
    ///		    And<APContact.vendorContactID, Equal<Contact.contactID>, 
    ///		    And<APContact.revisionID, Equal<Contact.revisionID>, 
    ///		    And<APContact.isDefaultContact, Equal<True>>>>>>>>,
	///		Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, 
    ///		And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>>>>))]
    /// </example>
    /// </summary>
	public class APContactAttribute : ContactAttribute
	{
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="SelectType">Must have type IBqlSelect. This select is used for both selecting <br/> 
        /// a source Contact record from which AP address is defaulted and for selecting version of APContact, <br/>
        /// created from source Contact (having  matching ContactID, revision and IsDefaultContact = true).<br/>
        /// - so it must include both records. See example above. <br/>
        /// </param>
		public APContactAttribute(Type SelectType)
			: base(typeof(APContact.contactID), typeof(APContact.isDefaultContact), SelectType)
		{
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldVerifying.AddHandler<APContact.overrideContact>(Record_Override_FieldVerifying);
		}

		public override void DefaultRecord(PXCache sender, object DocumentRow, object Row)
		{
			DefaultContact<APContact, APContact.contactID>(sender, DocumentRow, Row);
			OverrideEmployeeFullName(sender, DocumentRow);
		}
		public override void CopyRecord(PXCache sender, object DocumentRow, object SourceRow, bool clone)
		{
			CopyContact<APContact, APContact.contactID>(sender, DocumentRow, SourceRow, clone);
		}

		public override void Record_IsDefault_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
		}

		public virtual void Record_Override_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.NewValue = (e.NewValue == null ? e.NewValue : (bool?)e.NewValue == false);
			try
			{
				Contact_IsDefaultContact_FieldVerifying<APContact>(sender, e);
			}
			finally
			{
				e.NewValue = (e.NewValue == null ? e.NewValue : (bool?)e.NewValue == false);
			}
		}

		protected override void Record_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.Record_RowSelected(sender, e);

			if (e.Row != null)
			{
				PXUIFieldAttribute.SetEnabled<APContact.overrideContact>(sender, e.Row, sender.AllowUpdate);
			}
		}

		/// <summary>
		/// For employees, the AP contact's full name should be defaulted from
		/// <see cref="Contact.DisplayName"/> and not <see cref="Contact.FullName"/>.
		/// This method ensures correct defaulting in case of employees.
		/// </summary>
	    private void OverrideEmployeeFullName(PXCache sender, object documentRow)
	    {
			APContact apContact = PXSelect<
				APContact, 
				Where<APContact.contactID, Equal<Required<APContact.contactID>>>>
				.Select(sender.Graph, sender.GetValue(documentRow, FieldName));

			if (apContact == null) return;

			Contact contact = PXSelect<
				Contact,
				Where<Contact.contactID, Equal<Required<Contact.contactID>>>>
				.Select(sender.Graph, apContact.BAccountContactID);

			if (contact?.ContactType != ContactTypesAttribute.Employee) return;

			apContact.FullName = contact.DisplayName;
			sender.Graph.Caches<APContact>().Update(apContact);
	    }
	}

	/// <summary>
	/// FinPeriod selector that extends <see cref="AnyPeriodFilterableAttribute"/>. 
	/// Displays any periods (active, closed, etc), maybe filtered. 
	/// When Date is supplied through aSourceType parameter FinPeriod is defaulted with the FinPeriod for the given date.
	/// Default columns list includes 'Active' and  'Closed in GL' and 'Closed in AP'  columns
	/// </summary>
	public class APAnyPeriodFilterableAttribute : AnyPeriodFilterableAttribute
	{
		public APAnyPeriodFilterableAttribute(Type aSearchType, Type aSourceType)
			: base(aSearchType, aSourceType, typeof(FinPeriod.finPeriodID),typeof(FinPeriod.descr), typeof(FinPeriod.active), typeof(FinPeriod.closed), typeof(FinPeriod.aPClosed))
		{

		}

		public APAnyPeriodFilterableAttribute(Type aSourceType)
			: this(null, aSourceType)
		{

		}
		public APAnyPeriodFilterableAttribute()
			: this(null)
		{

		}
	}

	public class APAcctSubDefault
	{
		public class CustomListAttribute : PXStringListAttribute
		{
			public string[] AllowedValues
			{
				get
				{
					return _AllowedValues;
				}
			}

			public string[] AllowedLabels
			{
				get
				{
					return _AllowedLabels;
				}
			}

			public CustomListAttribute(string[] AllowedValues, string[] AllowedLabels)
				: base(AllowedValues, AllowedLabels)
			{
			}

		}

        /// <summary>
        /// Specialized version of the string list attribute which represents <br/>
        /// the list of the possible sources of the segments for the sub-account <br/>
        /// defaulting in the AP transactions. <br/>
        /// </summary>
		public class ClassListAttribute : CustomListAttribute
		{
			public ClassListAttribute()
                : base(new string[] { MaskLocation, MaskItem, MaskEmployee, MaskCompany, MaskProject, MaskTask }, new string[] { !PXAccess.FeatureInstalled<FeaturesSet.accountLocations>() ? AP.Messages.MaskVendor : AP.Messages.MaskLocation, Messages.MaskItem, Messages.MaskEmployee, Messages.MaskCompany, Messages.MaskProject, Messages.MaskTask })
			{
			}

			public override void CacheAttached(PXCache sender)
			{
				_AllowedValues = new[] { MaskLocation, MaskItem, MaskEmployee, MaskCompany, MaskProject, MaskTask };
				_AllowedLabels = new[] { !PXAccess.FeatureInstalled<FeaturesSet.accountLocations>() ? AP.Messages.MaskVendor : AP.Messages.MaskLocation, Messages.MaskItem, Messages.MaskEmployee, Messages.MaskCompany, Messages.MaskProject, Messages.MaskTask };
				_NeutralAllowedLabels = _AllowedLabels;

				base.CacheAttached(sender);
			}
		}
		public const string MaskLocation = "L";
		public const string MaskItem = "I";
		public const string MaskEmployee = "E";
		public const string MaskCompany = "C";
		public const string MaskProject = "P";
		public const string MaskTask = "T";
	}

	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
    [APAcctSubDefault.ClassList]
	public sealed class SubAccountMaskAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "SUBACCOUNT";
		private const string _MaskName = "APSETUP";
		public SubAccountMaskAttribute()
			: base()
		{
			PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, APAcctSubDefault.MaskLocation, new APAcctSubDefault.ClassListAttribute().AllowedValues, new APAcctSubDefault.ClassListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

        public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
        {
            base.GetSubscriber<ISubscriber>(subscribers);
            subscribers.Remove(_Attributes.OfType<APAcctSubDefault.ClassListAttribute>().FirstOrDefault() as ISubscriber);
        }
        
        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            var stringlist = (APAcctSubDefault.ClassListAttribute)_Attributes.First(x => x.GetType() == typeof(APAcctSubDefault.ClassListAttribute));
            var dimensionmaskattribute = (PXDimensionMaskAttribute)_Attributes.First(x => x.GetType() == typeof(PXDimensionMaskAttribute));
            dimensionmaskattribute.SynchronizeLabels(stringlist.AllowedValues, stringlist.AllowedLabels);
        }

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField 
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new APAcctSubDefault.ClassListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException)
			{
                // default source subID is null
                return null;

                //PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
                //string fieldName = fields[ex.SourceIdx].Name;
                //throw new PXMaskArgumentException(new APAcctSubDefault.ClassListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}
		}
	}

    /// <summary>
    /// This attribute implements auto-generation of the next check sequential number for APPayment Document<br/>
    /// according to the settings in the CashAccount and PaymentMethod. <br/>
    /// It's also creates and inserts a related record into the CashAccountCheck table <br/>
    /// and keeps it in synch. with AP Payment (delets it if the the payment is deleted.<br/>
    /// Depends upon CashAccountID, PaymentMethodID, StubCntr fields of the row.<br/>
    /// Cache(s) for the CashAccountCheck must be present in the graph <br/>
    /// </summary>
	public class PaymentRefAttribute : PXEventSubscriberAttribute, IPXRowPersistingSubscriber, IPXFieldDefaultingSubscriber, IPXFieldVerifyingSubscriber 
	{
		protected Type _CashAccountID;
		protected Type _PaymentTypeID;
		protected Type _StubCntr;
		protected Type _UpdateNextNumber;
		protected Type _ClassType;
        protected string _TargetDisplayName;

		protected bool _UpdateCashManager = true;

        /// <summary>
        /// This flag defines wether the field is defaulted from the PaymentMethodAccount record by the next check number<br/>
        /// If it set to false - the field on which attribute is set will not be initialized by the next value.<br/>
        /// This flag doesn't affect persisting behavior - if user enter next number manually, it will be saved.<br/>
        /// </summary>
		public bool UpdateCashManager
		{
			get
			{
				return this._UpdateCashManager;
			}
			set
			{
				this._UpdateCashManager = value;
			}
		}

		private PaymentMethodAccount GetCashAccountDetail(PXCache sender, object row)
		{
			object CashAccountID = sender.GetValue(row, _CashAccountID.Name);
			object PaymentTypeID = sender.GetValue(row, _PaymentTypeID.Name);

			if (_UpdateCashManager && CashAccountID != null && PaymentTypeID != null)
			{
                PXSelectBase<PaymentMethodAccount> cmd = new PXSelectReadonly<PaymentMethodAccount,
                                                                Where<PaymentMethodAccount.cashAccountID, Equal<Required<PaymentMethodAccount.cashAccountID>>,
                                                                    And<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>,
                                                                    And<PaymentMethodAccount.useForAP,Equal<True>>>>>(sender.Graph);
                PaymentMethodAccount det = cmd.Select(CashAccountID, PaymentTypeID);
				cmd.View.Clear();

				if (det != null && det.APLastRefNbr == null)
				{
					det.APLastRefNbr = string.Empty;
					det.APLastRefNbrIsNull = true;
				}
				return det;
			}
			return null;
		}

		private void GetPaymentMethodSettings(PXCache sender, object row, out PaymentMethod aPaymentMethod, out PaymentMethodAccount aPMAccount ) 
		{
			aPaymentMethod = null;
			aPMAccount = null; 
			object CashAccountID = sender.GetValue(row, _CashAccountID.Name);
			object PaymentTypeID = sender.GetValue(row, _PaymentTypeID.Name);
			if (_UpdateCashManager && CashAccountID != null && PaymentTypeID != null)
			{
                PXSelectBase<PaymentMethodAccount> cmd = new PXSelectReadonly2<PaymentMethodAccount, 
																	InnerJoin<PaymentMethod,On<PaymentMethod.paymentMethodID,Equal<PaymentMethodAccount.paymentMethodID>>>,
                                                                Where<PaymentMethodAccount.cashAccountID, Equal<Required<PaymentMethodAccount.cashAccountID>>,
                                                                    And<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>,
                                                                    And<PaymentMethodAccount.useForAP,Equal<True>>>>>(sender.Graph);
                PXResult<PaymentMethodAccount,PaymentMethod> res = (PXResult<PaymentMethodAccount,PaymentMethod>) cmd.Select(CashAccountID, PaymentTypeID);
				aPaymentMethod = res;
				cmd.View.Clear();
				PaymentMethodAccount det = res;
				if (det != null && det.APLastRefNbr == null)
				{
					det.APLastRefNbr = string.Empty;
					det.APLastRefNbrIsNull = true;
				}
				aPMAccount = det;
			}
			
		}

		void IPXFieldVerifyingSubscriber.FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
            PaymentMethodAccount det = GetCashAccountDetail(sender, e.Row);
			if (det != null && (bool)det.APAutoNextNbr)
			{
				string OldValue = (string)sender.GetValue(e.Row, _FieldOrdinal);

				if (string.IsNullOrEmpty(OldValue) == false && string.IsNullOrEmpty((string)e.NewValue) == false && object.Equals(OldValue, e.NewValue) == false)
				{
					try
					{
						if (sender.Graph.Views.ContainsKey("Document") && sender.Graph.Views["Document"].Cache.GetItemType() == sender.GetItemType())
						{
							WebDialogResult result = sender.Graph.Views["Document"].Ask(e.Row, Messages.AskConfirmation, Messages.AskUpdateLastRefNbr, MessageButtons.YesNo, MessageIcon.Question);
							if (result == WebDialogResult.Yes)
							{
								//will be persisted via Graph
								sender.SetValue(e.Row, _UpdateNextNumber.Name, true);
							}
						}
					}
					catch (PXException ex)
					{
						if (ex is PXDialogRequiredException)
						{
							throw;
						}
					}
				}
			}
            
		}

		protected Type _Table = null;

        /// <summary>
        /// Defines a table, from where oldValue of the field is taken from.<br/>
        /// If not set - the table associated with the sender will be used<br/>
        /// </summary>
		public Type Table
		{
			get
			{
				return this._Table;
			}
			set
			{
				this._Table = value;
			}
		}

		protected virtual string GetOldField(PXCache sender, object Row)
		{
			List<PXDataField> fields = new List<PXDataField> {new PXDataField(_FieldName)};
			fields.AddRange(sender.Keys.Select(key => new PXDataFieldValue(key, sender.GetValue(Row, key))));

			using (PXDataRecord OldRow = PXDatabase.SelectSingle(_Table, fields.ToArray()))
			{
				if (OldRow != null)
				{
					return OldRow.GetString(0);
				}
			}
			return null;
		}

	    public static void DeleteCheck(int CashAccountID, string PaymentMethodID, string CheckNbr)
	    {
			PXDatabase.Delete<CashAccountCheck>(
				new PXDataFieldRestrict<CashAccountCheck.accountID>(CashAccountID),
				new PXDataFieldRestrict<CashAccountCheck.paymentMethodID>(PaymentMethodID),
				new PXDataFieldRestrict<CashAccountCheck.checkNbr>(CheckNbr));
		}

        protected virtual void DeleteCheck(PaymentMethodAccount det, string CheckNbr)
		{
			DeleteCheck((int)det.CashAccountID, det.PaymentMethodID, CheckNbr);
		}

        protected virtual void InsertCheck(PXCache sender, PaymentMethodAccount det, APRegister payment, string CheckNbr)
		{
			PXCache<CashAccountCheck> cache = sender.Graph.Caches<CashAccountCheck>();
			CashAccountCheck check = new CashAccountCheck();

			List<PXDataFieldAssign> fields = new List<PXDataFieldAssign>();

			Dictionary<string, object> foreign_values = new Dictionary<string, object>
			{
				{typeof(CashAccountCheck.accountID).Name.ToLower(), det.CashAccountID},
				{typeof(CashAccountCheck.paymentMethodID).Name.ToLower(), det.PaymentMethodID},
				{typeof(CashAccountCheck.checkNbr).Name.ToLower(), CheckNbr},
				{typeof(CashAccountCheck.docType).Name.ToLower(), payment.DocType},
				{typeof(CashAccountCheck.refNbr).Name.ToLower(), payment.RefNbr},
				{typeof(CashAccountCheck.finPeriodID).Name.ToLower(), payment.FinPeriodID},
				{typeof(CashAccountCheck.docDate).Name.ToLower(), payment.DocDate},
				{typeof(CashAccountCheck.vendorID).Name.ToLower(), payment.VendorID},
				{typeof(CashAccountCheck.Tstamp).Name.ToLower(), PXCache.NotSetValue},
			};

			foreach (string field in cache.Fields)
			{
				object NewValue;
				if (!foreign_values.TryGetValue(field.ToLower(), out NewValue))
				{
					cache.RaiseFieldDefaulting(field, check, out NewValue);
					if (NewValue == null)
					{
						cache.RaiseRowInserting(check);
						NewValue = cache.GetValue(check, field);
					}
				}
				if (NewValue != PXCache.NotSetValue)
				{
					PXCommandPreparingEventArgs.FieldDescription descr;
					cache.RaiseCommandPreparing(field, check, NewValue, PXDBOperation.Insert, typeof(CashAccountCheck), out descr);

					if (descr != null && !string.IsNullOrEmpty(descr.FieldName))
					{
						fields.Add(new PXDataFieldAssign(descr.FieldName, descr.DataType, descr.DataLength, descr.DataValue));
					}
				}
			}
			PXDatabase.Insert<CashAccountCheck>(fields.ToArray());
		}

		protected virtual object GetStubCntr(PXCache sender, object Row)
		{
			object StubCntr = null;

			if (_StubCntr != null)
			{
				StubCntr = sender.GetValue(Row, _StubCntr.Name);
			}

			if (StubCntr == null || (int)StubCntr == 0)
			{
				StubCntr = 1;
			}
			return StubCntr;
		}

		public class LastCashAccountCheckSelect : PXSelectReadonly<CashAccountCheck,
			Where<CashAccountCheck.accountID, Equal<Optional<PaymentMethodAccount.cashAccountID>>,
				And<CashAccountCheck.paymentMethodID, Equal<Optional<PaymentMethodAccount.paymentMethodID>>>>,
			OrderBy<Desc<CashAccountCheck.cashAccountCheckID>>> 
		{
			public LastCashAccountCheckSelect(PXGraph graph) : base(graph) {}
			public LastCashAccountCheckSelect(PXGraph graph, Delegate handler) : base(graph, handler) {}
		};

		void IPXRowPersistingSubscriber.RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
            PaymentMethodAccount det;
			PaymentMethod pm;
			GetPaymentMethodSettings(sender, e.Row, out pm, out det);

			if (det == null || pm == null || pm.PrintOrExport == true) return;

			object SetNumber = sender.GetValue(e.Row, _FieldOrdinal);

			string OldNumber = GetOldField(sender, e.Row);

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update && Equals(OldNumber, SetNumber) == false ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				if (!string.IsNullOrEmpty(OldNumber))
				{
					DeleteCheck(det, OldNumber);
				}
			}
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update && Equals(OldNumber, SetNumber) == false ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				if (!string.IsNullOrEmpty((string) SetNumber))
				{
					try
					{
						InsertCheck(sender, det, (APRegister)e.Row, (string)SetNumber);
					}
					catch (PXDatabaseException)
					{
						if (pm.APRequirePaymentRef == true)
						{
                        throw new PXCommandPreparingException(_FieldName, SetNumber, Messages.ConflictWithExistingCheckNumber);
					}
				}
			}
			}

			// sync PaymentMethodAccount.APLastRefNbr with actual last CashAccountCheck number
			CashAccountCheck cacheck = LastCashAccountCheckSelect.SelectSingleBound(sender.Graph, new object[]{det});
			string lastCheckNbr = cacheck == null ? null : cacheck.CheckNbr;
			try
			{
				PXDatabase.Update<PaymentMethodAccount>(
					new PXDataFieldAssign<PaymentMethodAccount.aPLastRefNbr>(lastCheckNbr),
					new PXDataFieldRestrict<PaymentMethodAccount.cashAccountID>(det.CashAccountID),
					new PXDataFieldRestrict<PaymentMethodAccount.paymentMethodID>(det.PaymentMethodID),
					new PXDataFieldRestrict<PaymentMethodAccount.aPLastRefNbr>(PXDbType.NVarChar, 15, det.APLastRefNbr, det.APLastRefNbrIsNull == true ? PXComp.ISNULL : PXComp.EQ),
					PXDataFieldRestrict.OperationSwitchAllowed);
			}
			catch (PXDbOperationSwitchRequiredException)
			{
				PXDatabase.Insert<PaymentMethodAccount>(
					new PXDataFieldAssign<PaymentMethodAccount.cashAccountID>(det.CashAccountID),
					new PXDataFieldAssign<PaymentMethodAccount.paymentMethodID>(det.PaymentMethodID),
					new PXDataFieldAssign<PaymentMethodAccount.useForAP>(det.UseForAP),
					new PXDataFieldAssign<PaymentMethodAccount.aPLastRefNbr>(lastCheckNbr),
					new PXDataFieldAssign<PaymentMethodAccount.aPAutoNextNbr>(det.APAutoNextNbr),
					new PXDataFieldAssign<PaymentMethodAccount.aPIsDefault>(det.APIsDefault),
					new PXDataFieldAssign<PaymentMethodAccount.useForAR>(det.UseForAR),
					new PXDataFieldAssign<PaymentMethodAccount.aRIsDefault>(det.ARIsDefault),
					new PXDataFieldAssign<PaymentMethodAccount.aRAutoNextNbr>(det.ARIsDefault));
			}
		}

		void IPXFieldDefaultingSubscriber.FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PaymentMethod pm;
			PaymentMethodAccount det;
            GetPaymentMethodSettings(sender, e.Row, out pm, out det);
			e.NewValue = null;
			if (pm != null && det != null)
			{
				bool autoNumberOnPrint = (pm.APPrintChecks == true || pm.APCreateBatchPayment == true);
				if (autoNumberOnPrint == false && det.APAutoNextNbr == true)
				{
					int i = 0;
					do
					{
						try
						{
							e.NewValue = AutoNumberAttribute.NextNumber(det.APLastRefNbr, ++i);
						}
						catch (Exception)
						{
							sender.RaiseExceptionHandling(_FieldName, e.Row, null, new AutoNumberException(_TargetDisplayName));
						}

						if (i > 100)
						{
							e.NewValue = null;
							new AutoNumberException(_TargetDisplayName);
						}
					}
					while (PXSelect<CashAccountCheck, Where<CashAccountCheck.accountID, Equal<Current<PaymentMethodAccount.cashAccountID>>, And<CashAccountCheck.paymentMethodID, Equal<Current<PaymentMethodAccount.paymentMethodID>>, And<CashAccountCheck.checkNbr, Equal<Required<CashAccountCheck.checkNbr>>>>>>.SelectSingleBound(sender.Graph, new object[] { det }, new object[] { e.NewValue }).Count == 1);

					if (i > 1)
					{
						//will be persisted via Graph
						sender.SetValue(e.Row, _UpdateNextNumber.Name, true);
					}
				}
				else
				{
					sender.RaiseExceptionHandling(_FieldName, e.Row, e.NewValue, null);
				}
			}			
		}


		public PaymentRefAttribute(Type CashAccountID, Type PaymentTypeID, Type StubCntr, Type UpdateNextNumber)
		{
			_CashAccountID = CashAccountID;
			_PaymentTypeID = PaymentTypeID;
			_StubCntr = StubCntr;
			this._UpdateNextNumber = UpdateNextNumber;
		}
				

		private void DefaultRef(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object oldValue = sender.GetValue(e.Row, _FieldName);
			sender.SetValue(e.Row, _FieldName, null);	
			sender.SetDefaultExt(e.Row, _FieldName);
			if (sender.GetValue(e.Row, _FieldName) == null)
			{
				sender.SetValue(e.Row, _FieldName, oldValue);
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			_ClassType = sender.GetItemType();
			if (_Table == null)
			{
				_Table = sender.BqlTable;
			}

			sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_CashAccountID), _CashAccountID.Name, DefaultRef);
			sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_PaymentTypeID), _PaymentTypeID.Name, DefaultRef);

            _TargetDisplayName = PXUIFieldAttribute.GetDisplayName<PaymentMethodAccount.aPLastRefNbr>(sender.Graph.Caches[typeof(PaymentMethodAccount)]);
		}

        /// <summary>
        /// Sets IsUpdateCashManager flag for each instances of the Attibute specifed on the on the cache.
        /// </summary>
        /// <typeparam name="Field"> field, on which attribute is set</typeparam>
        /// <param name="cache"></param>
        /// <param name="data">Row. If omited, Field is set as altered for the cache</param>
        /// <param name="isUpdateCashManager">Value of the flag</param>
		public static void SetUpdateCashManager<Field>(PXCache cache, object data, bool isUpdateCashManager)
			where Field : IBqlField
		{
			if (data == null)
			{
				cache.SetAltered<Field>(true);
			}
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Field>(data))
			{
				if (attr is PaymentRefAttribute)
				{
					((PaymentRefAttribute)attr).UpdateCashManager = isUpdateCashManager;
				}
			}
		}
	}


    /// <summary>
    /// This attribute implements auto-generation of the next Batch sequential number for CABatch Document<br/>
    /// according to the settings in the CashAccount and PaymentMethod. <br/>
    /// Depends upon CashAccountID, PaymentMethodID fields of the row.<br/>    
    /// </summary>
    public class BatchRefAttribute : PXEventSubscriberAttribute, IPXRowPersistingSubscriber, IPXFieldDefaultingSubscriber, IPXFieldVerifyingSubscriber
    {
        protected Type _CashAccountID;
        protected Type _PaymentTypeID;
        protected Type _ClassType;

        protected bool _UpdateCashManager = true;

        /// <summary>
        /// This flag defines wether the field is defaulted from the PaymentMethodAccount record by the next check number<br/>
        /// If it set to false - the field on which attribute is set will not be initialized by the next value.<br/>
        /// This flag doesn't affect persisting behavior - if user enter next number manually, it will be saved.<br/>
        /// </summary>
        public bool UpdateCashManager
        {
            get
            {
                return this._UpdateCashManager;
            }
            set
            {
                this._UpdateCashManager = value;
            }
        }

        private PaymentMethodAccount GetCashAccountDetail(PXCache sender, object row)
        {
            object CashAccountID = sender.GetValue(row, _CashAccountID.Name);
            object PaymentTypeID = sender.GetValue(row, _PaymentTypeID.Name);
            object Hold = false;

            if (_UpdateCashManager && CashAccountID != null && PaymentTypeID != null)
            {
                PXSelectBase<PaymentMethodAccount> cmd = new PXSelectReadonly<PaymentMethodAccount, Where<PaymentMethodAccount.cashAccountID, Equal<Required<PaymentMethodAccount.cashAccountID>>, And<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>>>>(sender.Graph);
                PaymentMethodAccount det = cmd.Select(CashAccountID, PaymentTypeID);
                cmd.View.Clear();
                if (det != null && det.APBatchLastRefNbr == null)
                {
                    det.APBatchLastRefNbr = string.Empty;                    
                }
                return det;
            }
            return null;
        }

        void IPXFieldVerifyingSubscriber.FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
#if false
            PaymentMethodAccount det = GetCashAccountDetail(sender, e.Row);
            if (det != null)
            {
                string OldValue = (string)sender.GetValue(e.Row, _FieldOrdinal);
                if (string.IsNullOrEmpty(OldValue) == false && string.IsNullOrEmpty((string)e.NewValue) == false && object.Equals(OldValue, e.NewValue) == false)
                {
                    try
                    {
                        if (sender.Graph.Views.ContainsKey("Document") && sender.Graph.Views["Document"].Cache.GetItemType() == sender.GetItemType())
                        {
                            WebDialogResult result = sender.Graph.Views["Document"].Ask(e.Row, Messages.AskConfirmation, Messages.AskUpdateLastRefNbr, MessageButtons.YesNo, MessageIcon.Question);
                            if (result == WebDialogResult.Yes)
                            {
                                //will be persisted via Graph
                                det.LastRefNbr = (string)e.NewValue;
                                sender.Graph.Caches[typeof(PaymentMethodAccount)].Update(det);
                            }
                        }
                    }
                    catch (PXException ex)
                    {
                        if (ex is PXDialogRequiredException)
                        {
                            throw;
                        }
                    }
                }
            } 
#endif
        }
               
        void IPXRowPersistingSubscriber.RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            object SetNumber = sender.GetValue(e.Row, _FieldOrdinal);
            PaymentMethodAccount det = GetCashAccountDetail(sender, e.Row);

            if (det == null)
            {
                return;
            }
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
            {
                string NewNumber = AutoNumberAttribute.NextNumber(det.APBatchLastRefNbr);                
                if (SetNumber != null)
                {
                    if (object.Equals((string)SetNumber, NewNumber))
                    {
                         PXDatabase.Update<PaymentMethodAccount>(
                                new PXDataFieldAssign("APBatchLastRefNbr", NewNumber),
                                new PXDataFieldRestrict("CashAccountID", det.CashAccountID),
                                new PXDataFieldRestrict("PaymentMethodID", det.PaymentMethodID),
                                new PXDataFieldRestrict("APBatchLastRefNbr", det.APBatchLastRefNbr),
                                PXDataFieldRestrict.OperationSwitchAllowed);                        
                    }                    
                }
            }       
        }

        void IPXFieldDefaultingSubscriber.FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            PaymentMethodAccount det = GetCashAccountDetail(sender, e.Row);
            if (det != null)
            {
                try
                {
                   e.NewValue = AutoNumberAttribute.NextNumber(det.APBatchLastRefNbr);
                }
                catch (Exception)
                {
                    sender.RaiseExceptionHandling(_FieldName, e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<PaymentMethodAccount.aPBatchLastRefNbr>(sender.Graph.Caches[typeof(PaymentMethodAccount)])));
                }
            }
            else
            {
                e.NewValue = null;
            }
        }
        
        public BatchRefAttribute(Type CashAccountID, Type PaymentTypeID)
        {
            _CashAccountID = CashAccountID;
            _PaymentTypeID = PaymentTypeID;            
        }


        private void DefaultRef(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetValue(e.Row, _FieldName, null);
            sender.SetDefaultExt(e.Row, _FieldName);
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            _ClassType = sender.GetItemType();           
            sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_CashAccountID), _CashAccountID.Name, DefaultRef);
            sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_PaymentTypeID), _PaymentTypeID.Name, DefaultRef);
        }

        /// <summary>
        /// Sets IsUpdateCashManager flag for each instances of the Attibute specifed on the on the cache.
        /// </summary>
        /// <typeparam name="Field"> field, on which attribute is set</typeparam>
        /// <param name="cache"></param>
        /// <param name="data">Row. If omited, Field is set as altered for the cache</param>
        /// <param name="isUpdateCashManager">Value of the flag</param>
        public static void SetUpdateCashManager<Field>(PXCache cache, object data, bool isUpdateCashManager)
            where Field : IBqlField
        {
            if (data == null)
            {
                cache.SetAltered<Field>(true);
            }
            foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Field>(data))
            {
                if (attr is PaymentRefAttribute)
                {
                    ((PaymentRefAttribute)attr).UpdateCashManager = isUpdateCashManager;
                }
            }
        }
    }


    /// <summary>
    /// This is a specialized version of the Vendor attribute.<br/>
    /// Displays only Active or OneTime vendors<br/>
    /// See VendorAttribute for description. <br/>
    /// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Vendor", Visibility = PXUIVisibility.Visible)]
	[PXRestrictor(typeof(Where<Vendor.status, Equal<BAccount.status.active>,
						Or<Vendor.status, Equal<BAccount.status.oneTime>>>), Messages.VendorIsInStatus, typeof(Vendor.status))]
	public class VendorActiveAttribute : VendorAttribute
	{
		public VendorActiveAttribute(Type search)
			:base(search)
		{
		}

		public VendorActiveAttribute()
			: base()
		{ 
		}
	}

	/// <summary>
	/// A specialized version of the <see cref="VendorAttribute"/>
	/// allowing only selection of Active, One-Time, or Hold Payments 
	/// vendors.
	/// </summary>
	[PXDBInt]
	[PXUIField(DisplayName = Messages.Vendor, Visibility = PXUIVisibility.Visible)]
	[PXRestrictor(
		typeof(Where<
			Vendor.status, Equal<BAccount.status.active>,
			Or<Vendor.status, Equal<BAccount.status.oneTime>,
			Or<Vendor.status, Equal<BAccount.status.holdPayments>>>>),
		Messages.VendorIsInStatus,
		typeof(Vendor.status))]
	public class VendorActiveOrHoldPaymentsAttribute : VendorAttribute
	{
		public VendorActiveOrHoldPaymentsAttribute(Type search)
			: base(search)
		{ }

		public VendorActiveOrHoldPaymentsAttribute()
		{ }
	}

    /// <summary>
    /// This is a specialized version of the Vendor attribute.<br/>
    /// Displays only Active or OneTime vendors, filtering out Employees<br/>
    /// See VendorAttribute for description. <br/>
    /// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Vendor", Visibility = PXUIVisibility.Visible)]
	[PXRestrictor(typeof(Where<Vendor.type, NotEqual<BAccountType.employeeType>>), Messages.VendorCannotBe, typeof(Vendor.type))]
	public class VendorNonEmployeeActiveAttribute : VendorActiveAttribute
	{
		public VendorNonEmployeeActiveAttribute(Type search)
			:base(search)
		{ 
		}

		public VendorNonEmployeeActiveAttribute()
			: base()
		{
		}
	}

    /// <summary>
    /// Provides a UI Selector for Vendors.<br/>
    /// Properties of the selector - mask, length of the key, etc.<br/>
    /// are defined in the Dimension with predefined name "VENDOR".<br/>
    /// By default, search uses the following tables (linked) BAccount, Vendor (strict), Contact, Address (optional).<br/> 
    /// List of the Vendors is filtered based on the user's access rights.<br/>
    /// Default column's list in the Selector - Vendor.acctCD,Vendor.acctName,<br/>
    ///	Vendor.vendorClassID, Vendor.status, Contact.phone1, Address.city, Address.countryID<br/>
    ///	As most Dimention Selector - substitutes BAccountID by AcctCD.<br/>
    ///	List of properties - inherited from AcctSubAttribute <br/>
    ///	<example> 
    ///	[Vendor(typeof(Search<BAccountR.bAccountID,Where<BAccountR.type, Equal<BAccountType.companyType>,
	///		Or<Where<Vendor.type, NotEqual<BAccountType.employeeType>, And<Where<Vendor.status, Equal<BAccount.status.active>,
    ///	    Or<Vendor.status, Equal<BAccount.status.oneTime>>>>>>>>,
    ///	    DescriptionField = typeof(BAccount.acctName), CacheGlobal = true, Filterable = true)]
    ///	</example>
    /// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Vendor", Visibility = PXUIVisibility.Visible)]
    [Serializable]
	public class VendorAttribute : AcctSubAttribute
	{
		#region Override DACs

		[Serializable]
		[PXHidden]
		public partial class Location : IBqlTable
		{
			#region BAccountID
			public abstract class bAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _BAccountID;
			[PXDBInt(IsKey = true)]
			public virtual Int32? BAccountID
			{
				get
				{
					return this._BAccountID;
				}
				set
				{
					this._BAccountID = value;
				}
			}
			#endregion
			#region LocationID
			public abstract class locationID : PX.Data.IBqlField
			{
			}
			protected Int32? _LocationID;
			[PXDBIdentity()]
			public virtual Int32? LocationID
			{
				get
				{
					return this._LocationID;
				}
				set
				{
					this._LocationID = value;
				}
			}
			#endregion
			#region LocationCD
			public abstract class locationCD : PX.Data.IBqlField
			{
			}
			protected String _LocationCD;
			[PXDBString(IsKey = true, IsUnicode = true)]
			public virtual String LocationCD
			{
				get
				{
					return this._LocationCD;
				}
				set
				{
					this._LocationCD = value;
				}
			}
			#endregion
			#region TaxRegistrationID
			public abstract class taxRegistrationID : PX.Data.IBqlField
			{
			}
			protected String _TaxRegistrationID;
			[PXDBString(50, IsUnicode = true)]
			[PXUIField(DisplayName = "Tax Registration ID")]
			public virtual String TaxRegistrationID
			{
				get
				{
					return this._TaxRegistrationID;
				}
				set
				{
					this._TaxRegistrationID = value;
				}
			}
			#endregion
			#region VShipTermsID
			public abstract class vShipTermsID : IBqlField { }
			[PXDBString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Shipping Terms")]
			public virtual String VShipTermsID { get; set; }
			#endregion
			#region VLeadTime
            public abstract class vLeadTime : PX.Data.IBqlField
            {
            }
            protected Int16? _VLeadTime;
            [PXDBShort(MinValue = 0, MaxValue = 100000)]
            [PXUIField(DisplayName = "Lead Time (days)")]
            public virtual Int16? VLeadTime
            {
                get
                {
                    return this._VLeadTime;
                }
                set
                {
                    this._VLeadTime = value;
                }
            }
            #endregion
            #region VCarrierID
            public abstract class vCarrierID : PX.Data.IBqlField
            {
            }
            protected String _VCarrierID;
            [PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
            [PXUIField(DisplayName = "Ship Via")]
            public virtual String VCarrierID
            {
                get
                {
                    return this._VCarrierID;
                }
                set
                {
                    this._VCarrierID = value;
                }
            }
            #endregion
		}

        [Serializable]
		[PXHidden(ServiceVisible = true)]
		public class Contact : IBqlTable
		{
			#region BAccountID
			public abstract class bAccountID : PX.Data.IBqlField { }

			[PXDBInt]
			[CRContactBAccountDefault]
			public virtual Int32? BAccountID { get; set; }
			#endregion

			#region ContactID
			public abstract class contactID : IBqlField { }

			[PXDBIdentity(IsKey = true)]
			[PXUIField(DisplayName = "Contact ID", Visibility = PXUIVisibility.Invisible)]
			public virtual Int32? ContactID { get; set; }
			#endregion

			#region RevisionID
			public abstract class revisionID : PX.Data.IBqlField { }

			[PXDBInt]
			[PXDefault(0)]
			[AddressRevisionID()]
			public virtual Int32? RevisionID { get; set; }
			#endregion						

			#region Title
			public abstract class title : PX.Data.IBqlField { }

			[PXDBString(50, IsUnicode = true)]
			[Titles]
			[PXUIField(DisplayName = "Title")]
			public virtual String Title { get; set; }
			#endregion

			#region FirstName
			public abstract class firstName : PX.Data.IBqlField { }

			[PXDBString(50, IsUnicode = true)]
			[PXUIField(DisplayName = "First Name")]
			public virtual String FirstName { get; set; }
			#endregion

			#region MidName
			public abstract class midName : PX.Data.IBqlField { }

			[PXDBString(50, IsUnicode = true)]
			[PXUIField(DisplayName = "Middle Name")]
			public virtual String MidName { get; set; }
			#endregion

			#region LastName
			public abstract class lastName : PX.Data.IBqlField { }

			[PXDBString(100, IsUnicode = true)]
			[PXUIField(DisplayName = "Last Name")]
			[CRLastNameDefault]
			public virtual String LastName { get; set; }
			#endregion

			#region Salutation

			public abstract class salutation : PX.Data.IBqlField { }

			[PXDBString(255, IsUnicode = true)]
			[PXUIField(DisplayName = "Attention", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String Salutation { get; set; }
			#endregion

			#region Phone1
			public abstract class phone1 : PX.Data.IBqlField { }

			[PXDBString(50)]
			[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
			[PhoneValidation()]
			public virtual String Phone1 { get; set; }
			#endregion

			#region Phone1Type
			public abstract class phone1Type : PX.Data.IBqlField { }

			[PXDBString(3)]
			[PXDefault(PhoneTypesAttribute.Business1, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Phone 1")]
			[PhoneTypes]
			public virtual String Phone1Type { get; set; }
			#endregion

			#region Phone2
			public abstract class phone2 : PX.Data.IBqlField { }

			[PXDBString(50)]
			[PXUIField(DisplayName = "Phone 2")]
			[PhoneValidation()]
			public virtual String Phone2 { get; set; }
			#endregion

			#region Phone2Type
			public abstract class phone2Type : PX.Data.IBqlField { }

			[PXDBString(3)]
			[PXDefault(PhoneTypesAttribute.Business2, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Phone 2")]
			[PhoneTypes]
			public virtual String Phone2Type { get; set; }
			#endregion

			#region Phone3
			public abstract class phone3 : PX.Data.IBqlField { }

			[PXDBString(50)]
			[PXUIField(DisplayName = "Phone 3")]
			[PhoneValidation()]
			public virtual String Phone3 { get; set; }
			#endregion

			#region Phone3Type
			public abstract class phone3Type : PX.Data.IBqlField { }

			[PXDBString(3)]
			[PXDefault(PhoneTypesAttribute.Home, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Phone 3")]
			[PhoneTypes]
			public virtual String Phone3Type { get; set; }
			#endregion			

			#region WebSite
			public abstract class webSite : PX.Data.IBqlField { }

			[PXDBWeblink]
			[PXUIField(DisplayName = "Web")]
			public virtual String WebSite { get; set; }
			#endregion

			#region Fax
			public abstract class fax : PX.Data.IBqlField { }

			[PXDBString(50)]
			[PXUIField(DisplayName = "Fax")]
			[PhoneValidation()]
			public virtual String Fax { get; set; }
			#endregion

			#region EMail
			public abstract class eMail : PX.Data.IBqlField { }

			[PXDBEmail]
			[PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String EMail { get; set; }
			#endregion
		}
		#endregion

		public const string DimensionName = "VENDOR";		
			public VendorAttribute()
			: this(typeof(Search<BAccountR.bAccountID>))
		{
		}
        public VendorAttribute(Type search)
				: this(search,
					typeof(Vendor.acctCD), typeof(Vendor.acctName),
					typeof(Address.addressLine1), typeof(Address.addressLine2), typeof(Address.postalCode),
					typeof(Contact.phone1), typeof(Address.city), typeof(Address.countryID),
					typeof(Location.taxRegistrationID), typeof(Vendor.curyID),
					typeof(Contact.salutation),
					typeof(Vendor.vendorClassID), typeof(Vendor.status))
		{			
		}

        /// <summary>
        /// </summary>
        /// <param name="search">Defines BQL Select. Must be a type, compatible with BQL Search<></param>
        /// <param name="fields">List of the fields to display in the selector. Must be PX.Data.IBqlField</param>
        public VendorAttribute(Type search, params Type[] fields)
		{
			Type searchType = search.GetGenericTypeDefinition();
			Type[] searchArgs = search.GetGenericArguments();

			Type cmd;
			if (searchType == typeof(Search<>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								typeof(Where<Vendor.bAccountID, IsNotNull>));
			}
			else if (searchType == typeof(Search<,>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								searchArgs[1]);
			}
			else if (searchType == typeof(Search<,,>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								searchArgs[1],
								searchArgs[2]);
			}
			else if (searchType == typeof(Search2<,>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								searchArgs[1],
								typeof(Where<Vendor.bAccountID, IsNotNull>));
			}
			else if (searchType == typeof(Search2<,,>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								searchArgs[1],
								searchArgs[2]);
			}
			else if (searchType == typeof(Search2<,,,>))
			{
				cmd = BqlCommand.Compose(
								typeof(Search2<,,>),
								typeof(BAccountR.bAccountID),
								typeof(LeftJoin<,,>),
								typeof(Vendor),
								typeof(On<Vendor.bAccountID, Equal<BAccountR.bAccountID>, And<Match<Vendor, Current<AccessInfo.userName>>>>),
								typeof(LeftJoin<,,>),
								typeof(Contact),
								typeof(On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>),
								typeof(LeftJoin<,,>),
								typeof(Address),
								typeof(On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>),
								typeof(LeftJoin<,,>),
								typeof(Location),
								typeof(On<Location.bAccountID, Equal<BAccountR.bAccountID>, And<Location.locationID, Equal<BAccountR.defLocationID>>>),
								searchArgs[1],
								searchArgs[2],
								searchArgs[3]);
			}
			else
			{
				throw new PXArgumentException("search", ErrorMessages.ArgumentException);
			}
			_fields = fields;
			PXDimensionSelectorAttribute attr;
			_Attributes.Add(attr = new PXDimensionSelectorAttribute(DimensionName, cmd, typeof(BAccountR.acctCD),
				fields));			
			attr.DescriptionField = typeof(Vendor.acctName);
			attr.CacheGlobal = true;
	        attr.FilterEntity = typeof (Vendor);
			_SelAttrIndex = _Attributes.Count - 1;
			this.Filterable = true;
		}
		private readonly Type[] _fields;
		protected string[] _HeaderList = null;
		protected string[] _FieldList = null;

		public override void CacheAttached(PXCache sender)
		{
			//should go before standard code because of anonymous delegate in PXSelectorAttribute
			EmitColumnForVendorField(sender);

			base.CacheAttached(sender);

			string name = _FieldName.ToLower();
			sender.Graph.FieldSelecting.RemoveHandler(sender.GetItemType(), name, GetAttribute<PXDimensionSelectorAttribute>().FieldSelecting);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), name, FieldSelecting);
			
		}

		protected virtual void PopulateFields(PXCache sender)
		{
			if (_FieldList == null)
			{
				_FieldList = new string[this._fields.Length];
				_HeaderList = new string[this._fields.Length];

			for (int i = 0; i < this._fields.Length; i++)
			{
				Type cacheType = BqlCommand.GetItemType(_fields[i]);
				PXCache cache = sender.Graph.Caches[cacheType];
				if (cacheType.IsAssignableFrom(typeof(BAccountR)) || 
					_fields[i].Name == typeof(BAccountR.acctCD).Name ||
					_fields[i].Name == typeof(BAccountR.acctName).Name)
				{
						_FieldList[i] = _fields[i].Name;
				}
				else
				{
						_FieldList[i] = cacheType.Name + "__" + _fields[i].Name;
				}
					_HeaderList[i] = PXUIFieldAttribute.GetDisplayName(cache, _fields[i].Name);
			}
			}

			var attr = GetAttribute<PXDimensionSelectorAttribute>().GetAttribute<PXSelectorAttribute>();
			attr.SetColumns(_FieldList, _HeaderList);
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (this.AttributeLevel == PXAttributeLevel.Item || e.IsAltered)
			{
				PopulateFields(sender);
			}

			PXFieldSelecting handler = GetAttribute<PXDimensionSelectorAttribute>().FieldSelecting;
			handler(sender, e);
		}

		protected void EmitColumnForVendorField(PXCache sender)
		{
			if (this.DescriptionField  == null) 
				return;			

			{
				string alias = _FieldName + "_" + typeof(Vendor).Name + "_" + DescriptionField.Name;
				if (!sender.Fields.Contains(alias))
				{
					sender.Fields.Add(alias);
					sender.Graph.FieldSelecting.AddHandler
						(sender.GetItemType(), alias,
						 (s, e) =>
						 {
							 PopulateFields(sender);
							 GetAttribute<PXDimensionSelectorAttribute>().GetAttribute<PXSelectorAttribute>().DescriptionFieldSelecting(s, e, alias);
						 });

					sender.Graph.CommandPreparing.AddHandler
						(sender.GetItemType(), alias,
						 (s, e) => GetAttribute<PXDimensionSelectorAttribute>().GetAttribute<PXSelectorAttribute>().DescriptionFieldCommandPreparing(s, e));
				}
			}			
			{
				string alias = _FieldName + "_description";
				if (!sender.Fields.Contains(alias))
				{
					sender.Fields.Add(alias);
					sender.Graph.FieldSelecting.AddHandler
						(sender.GetItemType(), alias,
						 (s, e) => 
						 {
							 PopulateFields(sender);
							 GetAttribute<PXDimensionSelectorAttribute>().GetAttribute<PXSelectorAttribute>().DescriptionFieldSelecting(s, e, alias); 
						 });

					sender.Graph.CommandPreparing.AddHandler
						(sender.GetItemType(), alias,
						 (s, e) => GetAttribute<PXDimensionSelectorAttribute>().GetAttribute<PXSelectorAttribute>().DescriptionFieldCommandPreparing(s, e));
				}
			}
		}
	}

    /// <summary>
    /// Provides a UI Selector for Vendors AcctCD as a string. <br/>
    /// Should be used where the definition of the AccountCD is needed - mostly, in a Vendor DAC class.<br/>
    /// Properties of the selector - mask, length of the key, etc.<br/>
    /// are defined in the Dimension with predefined name "VENDOR".<br/>
    /// By default, search uses the following tables (linked) BAccount, Vendor (strict), Contact, Address (optional).<br/> 
    /// List of the Vendors is filtered based on the user's access rights.<br/>
    /// Default column's list in the Selector - Vendor.acctCD,Vendor.acctName,<br/>
    ///	Vendor.vendorClassID, Vendor.status, Contact.phone1, Address.city, Address.countryID<br/>    
    ///	List of properties - inherited from AcctSubAttribute <br/>
    ///	<example> 
    ///	[VendorRaw(typeof(Where<Vendor.type, Equal<BAccountType.vendorType>,
	///   Or<Vendor.type, Equal<BAccountType.combinedType>>>), DescriptionField = typeof(Vendor.acctName), IsKey = true, DisplayName = "Vendor ID")]
    ///	</example>
    /// </summary>    
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Vendor", Visibility = PXUIVisibility.Visible)]
	public sealed class VendorRawAttribute : AcctSubAttribute
	{
		public const string DimensionName = "VENDOR";		
		public VendorRawAttribute() : this(null) {}
        /// <summary>
        /// </summary>
        /// <param name="where">Defines a where clause for the BQL Select. Must be compatible with BQL Where<></param>
		public VendorRawAttribute(Type where) 
		{			
			Type SearchType = BqlCommand.Compose(
				typeof (Search2<,,>),
				typeof (Vendor.acctCD),
				typeof (
					LeftJoin<Contact, On<Contact.bAccountID, Equal<Vendor.bAccountID>, And<Contact.contactID, Equal<Vendor.defContactID>>>,
					LeftJoin<Address, On<Address.bAccountID, Equal<Vendor.bAccountID>, And<Address.addressID, Equal<Vendor.defAddressID>>>>>),
				where == null
					? typeof (Where<Match<Current<AccessInfo.userName>>>)
					: BqlCommand.Compose(
						typeof (Where2<,>),
						typeof (Where<Match<Current<AccessInfo.userName>>>),
						typeof (And<>),
						where));			
				
			PXDimensionSelectorAttribute attr;
			_Attributes.Add(attr = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(Vendor.acctCD),
					typeof(Vendor.acctCD), typeof(Vendor.acctName), typeof(Vendor.vendorClassID), typeof(Vendor.status), typeof(Contact.phone1), typeof(Address.city), typeof(Address.countryID)
				));
			attr.DescriptionField = typeof(Vendor.acctName);
			_SelAttrIndex = _Attributes.Count - 1;
			this.Filterable = true;
			((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).CacheGlobal = true;
		}
	}

    /// <summary>
    /// Specialized for APQuickCheck version of the APTaxAttribute(override).<br/>
    /// Provides Tax calculation for APTran, by default is attached to APTran (details) and APQuickCheck (header) <br/>
    /// Normally, should be placed on the TaxCategoryID field. <br/>
    /// Internally, it uses APQuickCheckEntry graph, otherwise taxes are not calculated (tax calc Level is set to NoCalc).<br/>
    /// As a result of this attribute work a set of APTax tran related to each AP Tran and to their parent will created <br/>
    /// May be combined with other attrbibutes with similar type - for example, APTaxAttribute <br/>
    /// <example>
    ///[APQuickCheckTax(typeof(Standalone.APQuickCheck), typeof(APTax), typeof(APTaxTran))]
    /// </example>    
    /// </summary>
	public class APQuickCheckTaxAttribute : APTaxAttribute
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentType">Type of parent(main) document. Must Be IBqlTable </param>
        /// <param name="TaxType">Type of the TaxTran records for the row(details). Must be IBqlTable</param>
        /// <param name="TaxSumType">Type of the TaxTran recorde for the main documect (summary). Must be iBqlTable</param>		
		public APQuickCheckTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType)
			:base(ParentType, TaxType, TaxSumType)
        {
	        Init();
        }

	    public APQuickCheckTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType, Type CalcMode)
		    : base(ParentType, TaxType, TaxSumType, CalcMode)
	    {
			Init();
	    }

	    private void Init()
	    {
			DocDate = typeof(APQuickCheck.adjDate);
			FinPeriodID = typeof(APQuickCheck.adjFinPeriodID);
			CuryLineTotal = typeof(APQuickCheck.curyLineTotal);
			CuryTranAmt = typeof(APTran.curyTranAmt);

			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(APTran.curyTranAmt), typeof(SumCalc<APQuickCheck.curyLineTotal>)));
	    }

		protected override void ParentFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APQuickCheck doc;
			if (e.Row is APQuickCheck && ((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				base.ParentFieldUpdated(sender, e);
			}
			else if (e.Row is CurrencyInfo && (doc = PXSelect<APQuickCheck, Where<APQuickCheck.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(sender.Graph, ((CurrencyInfo)e.Row).CuryInfoID)) != null && doc.DocType != APDocType.VoidQuickCheck)
			{
				base.ParentFieldUpdated(sender, e);
			}
		}

		protected override void ZoneUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				base.ZoneUpdated(sender, e);
			}
		}

		protected override void DateUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				base.DateUpdated(sender, e);
			}
		}
	}

    /// <summary>
    /// Specialized for AP version of the TaxAttribute. <br/>
    /// Provides Tax calculation for APTran, by default is attached to APTran (details) and APInvoice (header) <br/>
    /// Normally, should be placed on the TaxCategoryID field. <br/>
    /// Internally, it uses APInvoiceEntry graphs, otherwise taxes are not calculated (tax calc Level is set to NoCalc).<br/>
    /// As a result of this attribute work a set of APTax tran related to each AP Tran  and to their parent will created <br/>
    /// May be combined with other attrbibutes with similar type - for example, APTaxAttribute <br/>
    /// <example>
    /// [APTax(typeof(APRegister), typeof(APTax), typeof(APTaxTran))]
    /// </example>    
    /// </summary>
	public class APTaxAttribute : ManualVATAttribute
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentType">Type of parent(main) document. Must Be IBqlTable </param>
        /// <param name="TaxType">Type of the TaxTran records for the row(details). Must be IBqlTable</param>
        /// <param name="TaxSumType">Type of the TaxTran recorde for the main documect (summary). Must be iBqlTable</param>
		public APTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType)
			:base(ParentType, TaxType, TaxSumType)
        {
			Init();
        }

	    public APTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType, Type CalcMode)
		    : base(ParentType, TaxType, TaxSumType, CalcMode)
	    {
			Init();
	    }

	    private void Init()
	    {
			CuryTranAmt = typeof(APTran.curyTranAmt);
			GroupDiscountRate = typeof(APTran.groupDiscountRate);
			CuryLineTotal = typeof(APInvoice.curyLineTotal);

			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<APTran.lineType, NotEqual<SO.SOLineType.discount>>, APTran.curyTranAmt>, decimal0>), typeof(SumCalc<APInvoice.curyLineTotal>)));
	    }

		public override object Insert(PXCache cache, object item)
		{
			var recordsList = new List<object>(PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>.Select(cache.Graph).RowCast<APTax>());

			PXRowInserted inserted = delegate(PXCache sender, PXRowInsertedEventArgs e)
			{
				recordsList.Add(e.Row);

				PXCache pcache = cache.Graph.Caches[typeof(APRegister)];
				object TranType = pcache.GetValue<APRegister.docType>(pcache.Current);
				object RefNbr = pcache.GetValue<APRegister.refNbr>(pcache.Current);

				PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>.StoreCached(cache.Graph, new PXCommandKey(new object[] { TranType, RefNbr }), recordsList);
			};

			cache.Graph.RowInserted.AddHandler<APTax>(inserted);

			try
			{
				return base.Insert(cache, item);
			}
			finally
			{
				cache.Graph.RowInserted.RemoveHandler<APTax>(inserted);
			}
		}

		public override object Delete(PXCache cache, object item)
		{
			var recordsList = new List<object>(PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>.Select(cache.Graph).RowCast<APTax>());

			PXRowDeleted deleted = delegate(PXCache sender, PXRowDeletedEventArgs e)
			{
				recordsList.Remove(e.Row);

				PXCache pcache = cache.Graph.Caches[typeof(APRegister)];
				object TranType = pcache.GetValue<APRegister.docType>(pcache.Current);
				object RefNbr = pcache.GetValue<APRegister.refNbr>(pcache.Current);

				PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>.StoreCached(cache.Graph, new PXCommandKey(new object[] { TranType, RefNbr }), recordsList);
			};

			cache.Graph.RowDeleted.AddHandler<APTax>(deleted);

			try
			{
				return base.Delete(cache, item);
			}
			finally
			{
				cache.Graph.RowDeleted.RemoveHandler<APTax>(deleted);
			}
		}

	    protected override decimal? GetCuryTranAmt(PXCache sender, object row, string TaxCalcType)
        {
            decimal? CuryTranAmt = base.GetCuryTranAmt(sender, row) * (decimal?)sender.GetValue(row, _GroupDiscountRate) * (decimal?)sender.GetValue(row, _DocumentDiscountRate);
            return PXDBCurrencyAttribute.Round(sender, row, (decimal)CuryTranAmt, CMPrecision.TRANCURY);
        }

        protected override void SetTaxableAmt(PXCache sender, object row, decimal? value)
        {
            sender.SetValue<APTran.curyTaxableAmt>(row, value);
        }

        protected override void SetTaxAmt(PXCache sender, object row, decimal? value)
        {
            sender.SetValue<APTran.curyTaxAmt>(row, value);
        }

		protected override decimal? GetTaxableAmt(PXCache sender, object row)
		{
			return (decimal?)sender.GetValue<APTran.curyTaxableAmt>(row);
		}

		protected override decimal? GetTaxAmt(PXCache sender, object row)
		{
			return (decimal?)sender.GetValue<APTran.curyTaxAmt>(row);
		}

        protected virtual bool IsRoundingNeeded(PXGraph graph) 
        {
            PXCache cache = graph.Caches[typeof(APSetup)];
			bool hasTaxDescrepancy = (decimal)ParentGetValue(graph, _CuryTaxRoundDiff) != 0;
			return ((APSetup)cache.Current).InvoiceRounding != RoundingType.Currency || hasTaxDescrepancy;
        }

        protected virtual decimal? RoundAmount(PXGraph graph, decimal? value) 
        {
	        if (PXAccess.FeatureInstalled<FeaturesSet.invoiceRounding>())
	        {
		        PXCache cache = graph.Caches[typeof (APSetup)];
		        return APReleaseProcess.RoundAmount(value, ((APSetup) cache.Current).InvoiceRounding, ((APSetup) cache.Current).InvoicePrecision);
	        }
	        else
	        {
		        return value;
	        }
        }

        protected virtual void ResetRoundingDiff(PXGraph graph) 
        {
            base.ParentSetValue(graph, typeof(APRegister.curyRoundDiff).Name, 0m);
            base.ParentSetValue(graph, typeof(APRegister.roundDiff).Name, 0m);
        }

        protected override void ParentSetValue(PXGraph graph, string fieldname, object value)
        {
            PXCache cache = graph.Caches[typeof(APSetup)];
            if (fieldname == _CuryDocBal && cache.Current != null && IsRoundingNeeded(graph))
            {
                decimal? oldval = (decimal?)value;
                value = RoundAmount(graph, (decimal?)value);   
         
                decimal oldbaseval;
                decimal baseval;
                PXDBCurrencyAttribute.CuryConvBase(ParentCache(graph), ParentRow(graph), (decimal)oldval, out oldbaseval);
                PXDBCurrencyAttribute.CuryConvBase(ParentCache(graph), ParentRow(graph), (decimal)value, out baseval);

                oldbaseval -= baseval;
                oldval -= (decimal?)value;

				decimal taxDiscrepancy = (decimal)ParentGetValue(graph, _CuryTaxRoundDiff);
	            decimal taxDiscrepancyBase = (decimal)ParentGetValue(graph, _TaxRoundDiff);
	            oldbaseval += taxDiscrepancyBase;
	            oldval += taxDiscrepancy;

                base.ParentSetValue(graph, typeof(APRegister.curyRoundDiff).Name, oldval);
                base.ParentSetValue(graph, typeof(APRegister.roundDiff).Name, oldbaseval);
            }
            else
            {
                ResetRoundingDiff(graph);             
            }

            base.ParentSetValue(graph, fieldname, value);
        }

		protected override decimal CalcLineTotal(PXCache sender, object row)
		{
			return (decimal?)ParentGetValue(sender.Graph, _CuryLineTotal) ?? 0m;
		}

		protected override List<object> SelectTaxes<Where>(PXGraph graph, object row, PXTaxCheck taxchk, params object[] parameters)
		{
			Dictionary<string, PXResult<Tax, TaxRev>> tail = new Dictionary<string, PXResult<Tax, TaxRev>>();
			foreach (PXResult<Tax, TaxRev> record in PXSelectReadonly2<Tax,
				LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
					And<TaxRev.outdated, Equal<False>,
					And2<Where<TaxRev.taxType, Equal<TaxType.purchase>, And<Tax.reverseTax, Equal<False>,
						Or<TaxRev.taxType, Equal<TaxType.sales>, And<Where<Tax.reverseTax, Equal<True>,
						Or<Tax.taxType, Equal<CSTaxType.use>,
						Or<Tax.taxType, Equal<CSTaxType.withholding>>>>>>>>,
					And<Current<APRegister.docDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>,
				Where>
				.SelectMultiBound(graph, new object[] { row }, parameters))
			{
				Tax adjdTax = AdjustTaxLevel(graph, (Tax)record);
				tail[((Tax)record).TaxID] = new PXResult<Tax, TaxRev>(adjdTax, (TaxRev)record);
			}
			List<object> ret = new List<object>();
			switch (taxchk)
			{
				case PXTaxCheck.Line:
					int? lineNbr = (int?)graph.Caches[typeof(APTran)].GetValue<APTran.lineNbr>(row);

					foreach (APTax record in PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>
						.SelectMultiBound(graph, new object[] { row }))
					{
						if (record.LineNbr == lineNbr)
						{
							PXResult<Tax, TaxRev> line;
							if (tail.TryGetValue(record.TaxID, out line))
							{
								int idx;
								for (idx = ret.Count;
									(idx > 0)
									&& String.Compare(((Tax)(PXResult<APTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
									idx--)
									;
								ret.Insert(idx, new PXResult<APTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
							}
						}
					}
					return ret;
				case PXTaxCheck.RecalcLine:
					foreach (APTax record in PXSelect<APTax,
						Where<APTax.tranType, Equal<Current<APRegister.docType>>,
							And<APTax.refNbr, Equal<Current<APRegister.refNbr>>>>>
						.SelectMultiBound(graph, new object[] { row }))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& ((APTax)(PXResult<APTax, Tax, TaxRev>)ret[idx - 1]).LineNbr == record.LineNbr
								&& String.Compare(((Tax)(PXResult<APTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--)
								;
							ret.Insert(idx, new PXResult<APTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcTotals:
					foreach (APTaxTran record in PXSelect<APTaxTran,
						Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
							And<APTaxTran.tranType, Equal<Current<APRegister.docType>>,
							And<APTaxTran.refNbr, Equal<Current<APRegister.refNbr>>>>>>
						.SelectMultiBound(graph, new object[] { row }))
					{
						PXResult<Tax, TaxRev> line;
						if (record.TaxID != null && tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<APTaxTran, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--)
								;
							ret.Insert(idx, new PXResult<APTaxTran, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				default:
					return ret;
			}
		}

		protected override void CalcDocTotals(PXCache sender, object row, decimal CuryTaxTotal, decimal CuryInclTaxTotal, decimal CuryWhTaxTotal)
		{
			base.CalcDocTotals(sender, row, CuryTaxTotal, CuryInclTaxTotal, CuryWhTaxTotal);

			if (ParentGetStatus(sender.Graph) != PXEntryStatus.Deleted)
			{
				decimal doc_CuryWhTaxTotal = (decimal)(ParentGetValue(sender.Graph, _CuryWhTaxTotal) ?? 0m);

				if (object.Equals(CuryWhTaxTotal, doc_CuryWhTaxTotal) == false)
				{
					ParentSetValue(sender.Graph, _CuryWhTaxTotal, CuryWhTaxTotal);
				}
			}
		}

        protected override void _CalcDocTotals(PXCache sender, object row, decimal CuryTaxTotal, decimal CuryInclTaxTotal, decimal CuryWhTaxTotal)
        {
            decimal CuryDiscountTotal = (decimal)(ParentGetValue<APRegister.curyDiscTot>(sender.Graph) ?? 0m);

            decimal CuryLineTotal = CalcLineTotal(sender, row);

            decimal CuryDocTotal = CuryLineTotal + CuryTaxTotal - CuryInclTaxTotal - CuryDiscountTotal;

            decimal doc_CuryLineTotal = (decimal)(ParentGetValue(sender.Graph, _CuryLineTotal) ?? 0m);
            decimal doc_CuryTaxTotal = (decimal)(ParentGetValue(sender.Graph, _CuryTaxTotal) ?? 0m);

            if (object.Equals(CuryLineTotal, doc_CuryLineTotal) == false ||
                object.Equals(CuryTaxTotal, doc_CuryTaxTotal) == false)
            {
                ParentSetValue(sender.Graph, _CuryLineTotal, CuryLineTotal);
                ParentSetValue(sender.Graph, _CuryTaxTotal, CuryTaxTotal);
                if (!string.IsNullOrEmpty(_CuryDocBal))
                {
                    ParentSetValue(sender.Graph, _CuryDocBal, CuryDocTotal);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_CuryDocBal))
            {
                decimal doc_CuryDocBal = (decimal)(ParentGetValue(sender.Graph, _CuryDocBal) ?? 0m);

                if (object.Equals(CuryDocTotal, doc_CuryDocBal) == false)
                {
                    ParentSetValue(sender.Graph, _CuryDocBal, CuryDocTotal);
                }
            }
        }

        protected override void AdjustTaxableAmount(PXCache sender, object row, List<object> taxitems, ref decimal CuryTaxableAmt, string TaxCalcType)
        {
            decimal CuryLineTotal = (decimal?)ParentGetValue<APInvoice.curyLineTotal>(sender.Graph) ?? 0m;
            decimal CuryDiscountTotal = (decimal)(ParentGetValue<APInvoice.curyDiscTot>(sender.Graph) ?? 0m);

            if (CuryLineTotal != 0m && CuryTaxableAmt != 0m)
            {
                if (Math.Abs(CuryTaxableAmt - CuryLineTotal) < 0.00005m)
                {
                    CuryTaxableAmt -= CuryDiscountTotal;
                }
            }
        }
		
		protected override void SetExtCostExt(PXCache sender, object child, decimal? value)
		{
			APTran row = child as APTran;
			if (row != null)
			{
				row.CuryLineAmt = value;
				sender.Update(row);
			}
		}

		protected override string GetExtCostLabel(PXCache sender, object row)
		{
			return ((PXDecimalState)sender.GetValueExt<APTran.curyLineAmt>(row)).DisplayName;
		}

		public virtual IEnumerable<T> DistributeExpenseDiscrepancy<T>(PXGraph graph, IEnumerable<T> taxDetList, decimal CuryExpenseAmt)
			where T : TaxDetail
		{
			decimal curyExpenseSum = 0m;
			decimal curyTaxableSum = 0m;
			int linectr = 0;
			T maxDetail = null;
			PXCache taxDetCache = graph.Caches[_TaxType];

			foreach (var taxLine in taxDetList)
			{
				curyExpenseSum += taxLine.CuryExpenseAmt.Value;

				curyTaxableSum += Math.Abs((decimal)(taxDetCache.GetValue(taxLine, _CuryTaxableAmt) ?? 0m));
				linectr++;
                if (maxDetail == null)
				{
					maxDetail = taxLine;
				}
				else if (Math.Abs((decimal)(taxDetCache.GetValue(maxDetail, _CuryTaxableAmt) ?? 0m)) < Math.Abs((decimal)(taxDetCache.GetValue(taxLine, _CuryTaxableAmt) ?? 0m)))
				{
					maxDetail = taxLine;
				}
			}

			decimal discrepancy = CuryExpenseAmt - curyExpenseSum;
			if (Math.Abs(discrepancy) > 0m)
			{
				decimal discrSum = 0m;
				foreach (T taxLine in taxDetList)
				{
					decimal partDiscr = PXDBCurrencyAttribute.RoundCury(graph.Caches[typeof(T)], taxLine,
							discrepancy * (curyTaxableSum != 0 ? Math.Abs((decimal)(taxDetCache.GetValue(taxLine, _CuryTaxableAmt) ?? 0m) / curyTaxableSum) : (1m / linectr)));
					taxLine.CuryExpenseAmt += partDiscr;
					discrSum += partDiscr;
                    decimal expenseAmt;
					PXDBCurrencyAttribute.CuryConvBase(graph.Caches[typeof(T)], taxLine, taxLine.CuryExpenseAmt.Value, out expenseAmt);
					taxLine.ExpenseAmt = expenseAmt;
				}
				if (discrSum != discrepancy)
				{
					maxDetail.CuryExpenseAmt += discrepancy - discrSum;
					decimal expenseAmt;
					PXDBCurrencyAttribute.CuryConvBase(graph.Caches[typeof(T)], maxDetail, maxDetail.CuryExpenseAmt.Value, out expenseAmt);
					maxDetail.ExpenseAmt = expenseAmt;
				}
			}

			return taxDetList;
        }

		protected override bool isControlTaxTotalRequired(PXCache sender)
		{
			APSetup setup = new PXSetup<APSetup>(sender.Graph).Select();
			return setup != null && setup.RequireControlTaxTotal == true;
		}

		protected override bool isControlTotalRequired(PXCache sender)
		{
			APSetup setup = new PXSetup<APSetup>(sender.Graph).Select();
			return setup != null && setup.RequireControlTotal == true;
		}
	}

	public class APQuickCheckCashTranIDAttribute : CashTranIDAttribute
	{
		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			//will be null for DebitAdj and Prepayment request
			APQuickCheck parentDoc = (APQuickCheck)orig_Row;
			if (parentDoc.CashAccountID == null ||
				(parentDoc.Released == true) && (catran_Row.TranID != null) ||
				 parentDoc.CuryOrigDocAmt == null ||
				 parentDoc.CuryOrigDocAmt == 0)
			{
				return null;
			}
			catran_Row.OrigModule = BatchModule.AP;
			catran_Row.OrigTranType = parentDoc.DocType;
			catran_Row.OrigRefNbr = parentDoc.RefNbr;
			catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
			catran_Row.CashAccountID = parentDoc.CashAccountID;
			catran_Row.CuryInfoID = parentDoc.CuryInfoID;
			catran_Row.CuryID = parentDoc.CuryID;
            string voidedType = string.Empty;
			switch (parentDoc.DocType)
			{
				case APDocType.QuickCheck:
					catran_Row.CuryTranAmt = -parentDoc.CuryOrigDocAmt;
					catran_Row.DrCr = DrCr.Credit;
					break;
				case APDocType.VoidQuickCheck:
					catran_Row.CuryTranAmt = parentDoc.CuryOrigDocAmt;
					catran_Row.DrCr = DrCr.Debit;
                    voidedType = APDocType.QuickCheck;
					break;
				default:
					throw new PXException();
			}

			catran_Row.TranDate = parentDoc.DocDate;
			catran_Row.TranDesc = parentDoc.DocDesc;
			catran_Row.FinPeriodID = parentDoc.FinPeriodID;
			catran_Row.ReferenceID = parentDoc.VendorID;
			catran_Row.Released = parentDoc.Released;
			catran_Row.Hold = parentDoc.Hold;
			catran_Row.Cleared = parentDoc.Cleared;
			catran_Row.ClearDate = parentDoc.ClearDate;

            if (!string.IsNullOrEmpty(voidedType))
            {
                APPayment voidedDoc = PXSelectReadonly<APPayment, Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
                                                And<APPayment.docType, Equal<Required<APPayment.docType>>>>>.Select(sender.Graph, parentDoc.RefNbr, voidedType);
                if (voidedDoc != null)
                {
                    catran_Row.VoidedTranID = voidedDoc.CATranID;
                }
            }

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc						  = (CashAccount) selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared   = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}

			return catran_Row;
		}
	}

    /// <summary>
    /// Specialized for the APPayment version of the <see cref="CashTranIDAttribute"/><br/>
    /// Since CATran created from the source row, it may be used only the fields <br/>
    /// of APPayment compatible DAC. <br/>
    /// The main purpuse of the attribute - to create CATran <br/>
    /// for the source row and provide CATran and source synchronization on persisting.<br/>
    /// CATran cache must exists in the calling Graph.<br/>
    /// </summary>
	public class APCashTranIDAttribute : CashTranIDAttribute
	{
		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			//will be null for DebitAdj and Prepayment request
            APPayment parentDoc = (APPayment)orig_Row;
			if ( parentDoc.CashAccountID == null ||
				(parentDoc.Released == true) && (catran_Row.TranID != null) ||
				 parentDoc.CuryOrigDocAmt == null || 
				 parentDoc.CuryOrigDocAmt == 0)
			{
				return null;
			}
			catran_Row.OrigModule    = BatchModule.AP;
			catran_Row.OrigTranType  = parentDoc.DocType;
			catran_Row.OrigRefNbr    = parentDoc.RefNbr;
			catran_Row.ExtRefNbr     = parentDoc.ExtRefNbr;
			catran_Row.CashAccountID = parentDoc.CashAccountID;
			catran_Row.CuryInfoID    = parentDoc.CuryInfoID;
			catran_Row.CuryID		 = parentDoc.CuryID;
			string voidedType = string.Empty;
			string alterVoidedType = string.Empty;
            switch (parentDoc.DocType)
			{
				case APDocType.Check:
				case APDocType.Prepayment:
				case APDocType.QuickCheck:
					catran_Row.CuryTranAmt = -parentDoc.CuryOrigDocAmt;
					catran_Row.DrCr = DrCr.Credit;
					break;
				case APDocType.VoidCheck:
					catran_Row.CuryTranAmt = -parentDoc.CuryOrigDocAmt;
					catran_Row.DrCr = DrCr.Debit;
                    voidedType = APDocType.Check;
					alterVoidedType = APDocType.Prepayment;
					break;
				case APDocType.Refund:
				case APDocType.VoidQuickCheck:
					catran_Row.CuryTranAmt = parentDoc.CuryOrigDocAmt;
					catran_Row.DrCr = DrCr.Debit;
                    if(parentDoc.DocType == APDocType.VoidQuickCheck) 
                        voidedType = APDocType.QuickCheck;
					break;
				default:
					throw new PXException();
			}

			catran_Row.TranDate      = parentDoc.DocDate;
			catran_Row.TranDesc      = parentDoc.DocDesc;
			catran_Row.FinPeriodID   = parentDoc.FinPeriodID;
			catran_Row.ReferenceID   = parentDoc.VendorID;
			catran_Row.Released      = parentDoc.Released;
			catran_Row.Hold          = parentDoc.Hold;
			catran_Row.Cleared       = parentDoc.Cleared;
            catran_Row.ClearDate     = parentDoc.ClearDate;
			//This coping is required in one specfic case - when payment reclassification is made
			if (parentDoc.CARefTranID.HasValue)
			{
				catran_Row.RefTranAccountID = parentDoc.CARefTranAccountID;
				catran_Row.RefTranID = parentDoc.CARefTranID;
                catran_Row.RefSplitLineNbr = parentDoc.CARefSplitLineNbr;
			}

			if (!string.IsNullOrEmpty(voidedType))
			{
				APPayment voidedDoc = PXSelectReadonly<APPayment, Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
												And<APPayment.docType, Equal<Required<APPayment.docType>>>>>.Select(sender.Graph, parentDoc.RefNbr, voidedType);
				if (voidedDoc != null)
				{
					catran_Row.VoidedTranID = voidedDoc.CATranID;
				}
				else if (!string.IsNullOrEmpty(alterVoidedType))
				{
					APPayment alterVoidedDoc = PXSelectReadonly<APPayment, Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
											   And<APPayment.docType, Equal<Required<APPayment.docType>>>>>.Select(sender.Graph, parentDoc.RefNbr, alterVoidedType);
					if (alterVoidedDoc != null)
					{
						catran_Row.VoidedTranID = alterVoidedDoc.CATranID;
					}
				}
			}

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
			CashAccount cashacc						  = (CashAccount) selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
			{
				catran_Row.Cleared   = true;
				catran_Row.ClearDate = catran_Row.TranDate;
			}

			return catran_Row;
		}
	}

	public static class LangEN
	{
		public static string ToWords(Decimal amt, int Precision)
		{
			StringBuilder sb = new StringBuilder(ToWords(amt));
			Decimal Cents = Math.Floor((amt - Math.Truncate(amt)) * (decimal)Math.Pow(10, Precision));

			if (amt != 0m)
			{
				if (Cents != 0m)
				{
					sb.Append(" and ");
					sb.Append((int)Cents);
					sb.Append("/");
					sb.Append((int)Math.Pow(10, Precision));
				}
				else
				{
					sb.Append(" Only");
				}
			}
			else
			{
				sb.Append("Zero");
			}
			return sb.ToString();
		}

		public static string ToWords(Decimal? amt)
		{
			Decimal baseamt = Math.Floor((Decimal)amt);

			string[] less10 = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
			string[] less20 = new string[] { "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
			string[] less100 = new string[] { "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
			string[] great100 = new string[] { "", "Thousand", "Million", "Billion", "Trillion", "Quadrillion" };
			string is100 = "Hundred";
			string space = " ";

			int count = (int)Math.Floor(Math.Log10((double)baseamt));
			int tricount = (int)Math.Floor((double)count / 3) + 1;

			StringBuilder sb = new StringBuilder();

			for (int i = tricount; i > 0; i--)
			{
				int prevlen = sb.Length;
				int triamt = (int)Math.Floor((double)baseamt / Math.Pow(10, (i - 1) * 3));
				{
					if (triamt >= 100)
					{
						int h = (int)Math.Floor((double)triamt / 100);
						sb.Append(less10[h - 1]);
						sb.Append(space);
						sb.Append(is100);
						sb.Append(space);
						triamt = triamt - 100 * h;
						//Six Hundred
					}
					if (triamt >= 20 || triamt == 10)
					{
						int h = (int)Math.Floor((double)triamt / 10);
						sb.Append(less100[h - 1]);
						sb.Append(space);
						triamt = triamt - 10 * h;
						//Six Hundred Twenty
					}
					if (triamt < 20 && triamt > 10)
					{
						sb.Append(less20[triamt - 10 - 1]);
						sb.Append(space);
						//Six Hundred Eleven
					}
					if (triamt > 0 && triamt < 10)
					{
						sb.Append(less10[triamt - 1]);
						sb.Append(space);
						//Six Hundred Twenty One
					}
				}

				if (sb.Length > prevlen)
				{
					sb.Append(great100[i - 1]);
					sb.Append(space);
				}

				long newbase;
				Math.DivRem((long)baseamt, (long)Math.Pow(10, (i - 1) * 3), out newbase);

				baseamt = (Decimal)newbase;
			}

			return sb.ToString().TrimEnd();
		}
	}
    /// <summary>
    /// Converts Decimal value to it's word representation (English only) one way only<br/>
    /// For example, 1921.14 would be converted to "One thousand nine hundred twenty one and fourteen".<br/>
    /// Should be placed on the string field   
    /// <example>
    /// [ToWords(typeof(APPayment.curyOrigDocAmt))]
    /// </example>
    /// </summary>
	public class ToWordsAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		protected string _DecimalField = null;
		protected short? _Precision = null;
		public ToWordsAttribute(Type DecimalField)
		{
			_DecimalField = DecimalField.Name;
		}

		public ToWordsAttribute(short Precision)
		{
			_Precision = Precision; ;
		}

		public virtual void  FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 255, null, _FieldName, null, null, null, null, null, false, null);

			object DecimalVal;
			if (!string.IsNullOrEmpty(_DecimalField))
			{
				DecimalVal = sender.GetValue(e.Row, _DecimalField);
				sender.RaiseFieldSelecting(_DecimalField, e.Row, ref DecimalVal, true);
			}
			else
			{
				DecimalVal = PXDecimalState.CreateInstance(e.ReturnValue, (short)_Precision, _FieldName, false, 0, Decimal.MinValue, Decimal.MaxValue);
			}

			if (DecimalVal is PXDecimalState)
			{
				if (((PXDecimalState)DecimalVal).Value == null)
				{
					e.ReturnValue = string.Empty;
					return;
				}

				e.ReturnValue = LangEN.ToWords((decimal)((PXDecimalState)DecimalVal).Value, ((PXDecimalState)DecimalVal).Precision);
			}
		}
	}

    /// <summary>
    /// Specialized version of the selector for AP Open Financial Periods.<br/>
    /// Displays a list of FinPeriods, having flags Active = true and  APClosed = false.<br/>
    /// </summary>
	public class APOpenPeriodAttribute : OpenPeriodAttribute
	{
		#region Ctor
		public APOpenPeriodAttribute(Type SourceType)
			: base(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.aPClosed, Equal<False>, And<FinPeriod.active, Equal<True>>>>), SourceType)
		{
		}

		public APOpenPeriodAttribute()
			: this(null)
		{
		}
		#endregion
		
		#region Implementation
		public override void IsValidPeriod(PXCache sender, object Row, object NewValue)
		{
			string OldValue = (string)sender.GetValue(Row, _FieldName);
			base.IsValidPeriod(sender, Row, NewValue);			

			if (NewValue != null && _ValidatePeriod != PeriodValidation.Nothing)
			{
				FinPeriod p = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(sender.Graph, (string)NewValue);
				if (p.APClosed == true)
				{
                    if (PostClosedPeriods(sender.Graph))
                    {
                        sender.RaiseExceptionHandling(_FieldName, Row, null, new FiscalPeriodClosedException(p.FinPeriodID, PXErrorLevel.Warning));
                        return;
                    }
                    else
                    {
                        throw new FiscalPeriodClosedException(p.FinPeriodID);
                    }
                }
			}
		}
		#endregion
	}

	/// <summary>
	/// FinPeriod selector that extends <see cref="FinPeriodSelectorAttribute"/>. 
	/// Displays and accepts only Closed Fin Periods. 
	/// When Date is supplied through aSourceType parameter FinPeriod is defaulted with the FinPeriod for the given date.
	/// </summary>
	public class APClosedPeriodAttribute : FinPeriodSelectorAttribute
	{
		public APClosedPeriodAttribute(Type aSourceType)
            : base(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.closed, Equal<True>, Or<FinPeriod.aPClosed, Equal<True>, Or<FinPeriod.active, Equal<True>>>>, OrderBy<Desc<FinPeriod.finPeriodID>>>), aSourceType)
        {
		}

		public APClosedPeriodAttribute()
			: this(null)
		{

		}
	}

	#region accountsPayableModule

	public sealed class accountsPayableModule : Constant<string>
	{
		public accountsPayableModule() : base(typeof(accountsPayableModule).Namespace) { }
	}

	#endregion
	#region vendorType
	public sealed class vendorType : Constant<string>
	{
		public vendorType()
			: base(typeof(PX.Objects.AP.Vendor).FullName)
		{
		}
	}
	#endregion

	#region PXDBVendorCuryAttribute	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Method)]
	public class PXDBVendorCuryAttribute : PXDBDecimalAttribute
	{
		public PXDBVendorCuryAttribute(Type vendorID)
			: base(BqlCommand.Compose(typeof(Search2<,,>), typeof(Vendor.taxReportPrecision),
				typeof(CrossJoin<Company,
						   LeftJoin<Currency, On<Currency.curyID, Equal<Vendor.curyID>>,
							 LeftJoin<Currency2, On<Currency2.curyID, Equal<Company.baseCuryID>>>>>),
				typeof(Where<,>), typeof(Vendor.bAccountID), typeof(Equal<>), typeof(Current<>), vendorID))
		{			
		}

		protected override int? GetItemPrecision(PXView view, object item)
		{
			var result = item as PX.Data.PXResult<Vendor, Company, Currency, Currency2>;
			if (result == null) return null;

			PXCache vendor = view.Graph.Caches[typeof (Vendor)];
			bool? useVendorCur = (bool?)vendor.GetValue<Vendor.taxUseVendorCurPrecision>((Vendor)result);
			if(useVendorCur == true)
			{
				return
					(short?)view.Graph.Caches[typeof(Currency)].GetValue<Currency.decimalPlaces>((Currency)result) ??
					(short?)view.Graph.Caches[typeof(Currency2)].GetValue<Currency2.decimalPlaces>((Currency2)result);
			}
			return (short?)vendor.GetValue<Vendor.taxReportPrecision>((Vendor)result);
		}

        public override void CacheAttached(PXCache sender)
        {
            sender.SetAltered(_FieldName, true);
            base.CacheAttached(sender);
        }


	}
	#endregion

    public class APPaymentChargeCashTranIDAttribute : CashTranIDAttribute
    {
        protected bool _IsIntegrityCheck = false;

        public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
        {
            APPaymentChargeTran parentDoc = (APPaymentChargeTran)orig_Row;
            if (parentDoc.Released == true || parentDoc.CuryTranAmt == null || parentDoc.CuryTranAmt == 0m)
            {
                return null;
            }

            catran_Row.OrigModule = BatchModule.AP;
            catran_Row.OrigTranType = parentDoc.DocType;
            catran_Row.OrigRefNbr = parentDoc.RefNbr;
            catran_Row.CashAccountID = parentDoc.CashAccountID;
            catran_Row.CuryInfoID = parentDoc.CuryInfoID;
            catran_Row.CuryTranAmt = -parentDoc.CuryTranAmt;
            catran_Row.ExtRefNbr = parentDoc.ExtRefNbr;
            catran_Row.DrCr = parentDoc.DrCr;
            catran_Row.FinPeriodID = parentDoc.FinPeriodID;

            APRegister register = PXSelect<APRegister, Where<APRegister.docType, Equal<Required<APPaymentChargeTran.docType>>,
                                                                            And<APRegister.refNbr, Equal<Required<APPaymentChargeTran.refNbr>>>>>.Select(sender.Graph, parentDoc.DocType, parentDoc.RefNbr);
            catran_Row.ReferenceID = register.VendorID;
            catran_Row.TranPeriodID = parentDoc.TranPeriodID;
            catran_Row.TranDate = parentDoc.TranDate;
            catran_Row.TranDesc = parentDoc.TranDesc;
            catran_Row.Released = parentDoc.Released;

			PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, 
				Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
            CashAccount cashacc = (CashAccount)selectStatement.View.SelectSingle(catran_Row.CashAccountID);
			catran_Row.CuryID = cashacc?.CuryID;
            if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
            {
                catran_Row.Cleared = true;
                catran_Row.ClearDate = catran_Row.TranDate;
            }

            return catran_Row;
        }

        public static CATran DefaultValues<Field>(PXCache sender, object data)
            where Field : IBqlField
        {
            foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
            {
                if (attr is APPaymentChargeCashTranIDAttribute)
                {
                    ((APPaymentChargeCashTranIDAttribute)attr)._IsIntegrityCheck = true;
                    return ((APPaymentChargeCashTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
                }
            }
            return null;
        }

        public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (_IsIntegrityCheck == false)
            {
                base.RowPersisting(sender, e);
            }
        }

        public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            if (_IsIntegrityCheck == false)
            {
                base.RowPersisted(sender, e);
            }
        }
    }

    public enum PaddingEnum
    {
        None,
        Left,
        Right
    };

    public enum AlphaCharacterCaseEnum
    {
        None,
        Lower,
        Upper
    };

    /// <summary>
    /// Attribute to create Fixed Width file, works in conjuction with FixedLengthFile class
    /// <example>
    /// [FixedLength(StartPosition = 21, FieldLength = 4, RegexReplacePattern = @"[^0-9a-zA-Z]")]
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class FixedLengthAttribute : System.Attribute
    {
        private int _FieldLength = 0;
        public int FieldLength
        {
            get
            {
                return _FieldLength;
            }
            set
            {
                _FieldLength = value;
            }
        }

        private int _StartPosition = 0;
        public int StartPosition
        {
            get
            {
                return _StartPosition;
            }
            set
            {
                _StartPosition = value;
            }
        }

        private PaddingEnum _PaddingStyle = PaddingEnum.Right;
        public PaddingEnum PaddingStyle
        {
            get
            {
                return _PaddingStyle;
            }
            set
            {
                _PaddingStyle = value;
            }
        }

        private AlphaCharacterCaseEnum _AlphaCharacterCaseStyle = AlphaCharacterCaseEnum.None;
        public AlphaCharacterCaseEnum AlphaCharacterCaseStyle
        {
            get
            {
                return _AlphaCharacterCaseStyle;
            }
            set
            {
                _AlphaCharacterCaseStyle = value;
            }
        }

        private char _PaddingChar = ' ';
        public char PaddingChar
        {
            get
            {
                return _PaddingChar;
            }
            set
            {
                _PaddingChar = value;
            }
        }

        private string _RegexReplacePattern = String.Empty;
        public string RegexReplacePattern
        {
            get
            {
                return _RegexReplacePattern;
            }
            set
            {
                _RegexReplacePattern = value;
            }
        }

        public FixedLengthAttribute()
        {
        }
    }


	public class APActiveProjectAttibute : PM.ActiveProjectForModuleAttribute
	{
		public APActiveProjectAttibute(): base(BatchModule.AP, false)
		{
			AccountFieldType = typeof (APTran.accountID);
		}

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			APTran row = e.Row as APTran;

			if (row != null)
			{
				if (!string.IsNullOrEmpty(row.ReceiptNbr))
				{
					//do not validate Account - AccountGroup rule.
					return;
				}

			}


			base.FieldVerifying(sender, e);
		}
	}

	public class APPaymentChargeSelect<PaymentTable, PaymentMethodID, CashAccountID, AdjDate, AdjFinPeriodID, WhereSelect> :
		PaymentChargeSelect<PaymentTable, PaymentMethodID, CashAccountID, AdjDate, AdjFinPeriodID,
			APPaymentChargeTran, APPaymentChargeTran.entryTypeID, WhereSelect>
		where PaymentTable : class, IBqlTable, new()
		where PaymentMethodID : IBqlField
		where CashAccountID : IBqlField
		where AdjDate : IBqlField
		where AdjFinPeriodID : IBqlField
		where WhereSelect : IBqlWhere, new()
	{
		public APPaymentChargeSelect(PXGraph graph)
			: base(graph)
		{
		}
	}

	public class PaymentChargeSelect<PaymentTable, PaymentMethodID, CashAccountID, AdjDate, AdjFinPeriodID, 
			ChargeTable, EntryTypeID, WhereSelect> : PXSelect<ChargeTable, WhereSelect>
		where PaymentTable : class, IBqlTable, new()
		where PaymentMethodID : IBqlField
		where CashAccountID : IBqlField
		where AdjDate : IBqlField
		where AdjFinPeriodID : IBqlField
		where ChargeTable : class, IBqlTable, new()
		where EntryTypeID : IBqlField
		where WhereSelect : IBqlWhere, new()
	{
		#region Ctor
		public PaymentChargeSelect(PXGraph graph)
			: base(graph)
		{
			graph.RowUpdated.AddHandler<PaymentTable>(PaymentRowUpdated);
			graph.FieldUpdating.AddHandler<PaymentMethodID>(PaymentMethodID_FieldUpdating);
			graph.FieldUpdating.AddHandler<CashAccountID>(CashAccountID_FieldUpdating);
		}
		#endregion

		#region Implementation
		protected virtual void PaymentRowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<AdjDate, AdjFinPeriodID, CashAccountID>(e.Row, e.OldRow))
			{
				foreach (ChargeTable charge in this.View.SelectMulti())
				{
					this.View.Cache.SmartSetStatus(charge, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void PaymentMethodID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			PaymentTable payment = (PaymentTable)e.Row;
			if (payment == null) return;

			object oldPaymentMethod = sender.GetValue<PaymentMethodID>(payment);
			if (!Equals(oldPaymentMethod, e.NewValue))
			{
				PaymentTable copy = (PaymentTable)sender.CreateCopy(payment);
				sender.SetValue<PaymentMethodID>(copy, e.NewValue);

				RelatedFieldsDefaulting(sender, copy);

				object newCashAccountID;
				sender.RaiseFieldDefaulting<CashAccountID>(copy, out newCashAccountID);
				sender.SetValue<CashAccountID>(copy, newCashAccountID);

				CashAccount cashAccount = (CashAccount)PXSelectorAttribute.Select<CashAccountID>(sender, copy);

				object oldCashAccountID = sender.GetValue<CashAccountID>(payment);
				if (!Equals(oldCashAccountID, newCashAccountID) &&
					!CheckPaymentCharge(sender.Graph, cashAccount?.CashAccountCD, Messages.SomeChargeNotRelatedWithPaymentMethod))
				{
					e.NewValue = oldPaymentMethod;
				}
			}
		}

		protected virtual void RelatedFieldsDefaulting(PXCache sender, PaymentTable payment)
		{
		}

		protected virtual void CashAccountID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			PaymentTable payment = (PaymentTable)e.Row;
			if (payment == null) return;

			if (!CheckPaymentCharge(sender.Graph, e.NewValue, Messages.SomeChargeNotRelatedWithCashAccount))
			{
				e.NewValue = sender.GetValue<CashAccountID>(payment);
			}
		}

		protected virtual bool CheckPaymentCharge(PXGraph graph, object cashAccountCD, string message)
		{
			bool result = true;
			bool wasFirstErrorCharge = false;

			foreach (ChargeTable charge in this.View.SelectMulti())
			{
				object entryTypeID = this.View.Cache.GetValue<EntryTypeID>(charge);
				CashAccountETDetail eTDetail = (CashAccountETDetail)PXSelectJoin<CashAccountETDetail,
					InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CashAccountETDetail.accountID>>>,
					Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>,
						And<CashAccountETDetail.entryTypeID, Equal<Required<EntryTypeID>>>>>.
					Select(graph, cashAccountCD, entryTypeID);
				if (eTDetail == null)
				{
					if (!wasFirstErrorCharge)
					{
						if (this.View.Ask(Messages.Warning, message, MessageButtons.YesNo) == WebDialogResult.No)
						{
							result = false;
							break;
						}
						wasFirstErrorCharge = true;
					}
					this.View.Cache.Delete(charge);
				}
			}

			return result;
		}
		#endregion
	}
    public abstract class BaseVendorRefNbrAttribute : PXEventSubscriberAttribute, IPXRowUpdatedSubscriber, IPXFieldVerifyingSubscriber, IPXRowDeletedSubscriber, IPXRowInsertedSubscriber, IPXRowPersistedSubscriber
	{
        #region State
        protected int DETAIL_DUMMY = 0;
        protected class EntityKey
        {
            public Guid? _MasterID;
            public Int32 _DetailID;
        }
		protected Type _VendorID;
        protected PXGraph _Graph;
        protected PXCache _Cache;
        protected List<APVendorRefNbr> _ToCheck;
        protected PXCache Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = _Graph.Caches[typeof(APVendorRefNbr)];
                return _Cache;
            }
        }
        private EntityHelper _EntityHelper;
        protected EntityHelper EntityHelper
        { get
            {
                if (_EntityHelper == null)
                    _EntityHelper = new EntityHelper(_Graph);
                return _EntityHelper;
            }
        }
		#endregion

		#region Ctor
		public BaseVendorRefNbrAttribute(Type VendorIDField)
		{
            _VendorID = VendorIDField;
		}
		#endregion

		#region Initialization
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
            _Graph = sender.Graph;
            _Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_VendorID), _VendorID.Name, DataUpdatedCheck);
        }
        #endregion

        #region Event Handlers

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.ExternalCall && !String.IsNullOrEmpty(Convert.ToString(e.NewValue)))
            {
                    var copy = sender.CreateCopy(e.Row);
                    sender.SetValue(copy, _FieldName, e.NewValue);
                try
                {
                    CheckUniqueness(sender, copy);
                    sender.RaiseExceptionHandling(_FieldName, e.Row, null, null);
                }
                catch (PXSetPropertyException ex)
                {
                    if (IsRequiredUniqueRefNbr(sender, copy))
                        sender.RaiseExceptionHandling(_FieldName, e.Row, e.NewValue, ex);
                    else
                        sender.RaiseExceptionHandling(_FieldName, e.Row, e.NewValue, new PXSetPropertyException(ex.Message, PXErrorLevel.Warning));
                }
            }
        }

        protected void DataUpdatedCheck(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            FieldVerifying(sender, new PXFieldVerifyingEventArgs(e.Row, sender.GetValue(e.Row, _FieldName), e.ExternalCall));
        }

        public virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            if (e.Row == null)
                return;

            if (sender.GetStatus(e.Row) == PXEntryStatus.Deleted || sender.GetStatus(e.Row) == PXEntryStatus.InsertedDeleted)
                return;

            APVendorRefNbr vrn = InitVendorRefObject(sender, e.Row);
            if (vrn == null)
                return;
            if (vrn.IsIgnored == true)
            {
                Cache.Delete(vrn);
            }
            else
            {
                vrn = (APVendorRefNbr)Cache.Update(vrn);
            }
            _Graph.Caches[typeof(APVendorRefNbr)].IsDirty = false;
        }

        public virtual void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            if (e.Row == null)
                return;

            var vrn = InitVendorRefObject(sender, e.Row);
            if (vrn == null)
                return;
            Cache.Delete(vrn);
            _Graph.Caches[typeof(APVendorRefNbr)].IsDirty = false;
        }

        public virtual void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            if (e.Row == null)
                return;

            if (sender.GetStatus(e.Row) == PXEntryStatus.Deleted || sender.GetStatus(e.Row) == PXEntryStatus.InsertedDeleted)
                return;

            var vrn = InitVendorRefObject(sender, e.Row);
            if (vrn == null)
                return;
            vrn = (APVendorRefNbr)Cache.Insert(vrn);
            if(vrn.IsIgnored ?? true)
                Cache.Delete(vrn);
            _Graph.Caches[typeof(APVendorRefNbr)].IsDirty = false;
        }

        public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            switch(e.TranStatus)
            {
                case PXTranStatus.Open:
                    _ToCheck = Cache.Cached.Cast<APVendorRefNbr>()
                        .Where(x => (Cache.GetStatus(x) == PXEntryStatus.Inserted || Cache.GetStatus(x) == PXEntryStatus.Updated) 
                        && (x.IsChecked ?? false) && !(x.IsProcessed ?? false)).ToList();

                    if (_ToCheck.Count > 0)
                    {
                        Cache.ClearQueryCache();
                    }
                    foreach (APVendorRefNbr vrn in _ToCheck)
                    {
                        CheckUniqueness(vrn);
                    }
                    foreach (APVendorRefNbr vrn in Cache.Cached)
                    {
                        if (vrn.IsProcessed == true)
                            break;

                        if (vrn.IsIgnored ?? true)
                        {
                            DeleteKey(vrn);
                            continue;
                        }

                        if ((Cache.GetStatus(vrn) == PXEntryStatus.Inserted || Cache.GetStatus(vrn) == PXEntryStatus.Updated))
                        {
                            UpdateKey(vrn);
                        }
                        if ((Cache.GetStatus(vrn) == PXEntryStatus.Deleted || Cache.GetStatus(vrn) == PXEntryStatus.InsertedDeleted))
                        {
                            DeleteKey(vrn);
                        }
                        vrn.IsProcessed = true;
                    }


                    break;

                case PXTranStatus.Aborted:
                    foreach (APVendorRefNbr vrn in Cache.Cached)
                    {
                        vrn.IsProcessed = false;
                    }
                    break;
                case PXTranStatus.Completed:
                    Cache.Clear();
                    break;
            }
        }

        #endregion

        #region Implementation

        protected virtual Guid? GetMasterNoteId(Type masterEntity, Type masterNoteField, object masterRow)
        {
            bool dirtystate = _Graph.Caches[typeof(Note)].IsDirty;
            var result = PXNoteAttribute.GetNoteID<PO.POReceipt.noteID>(_Graph.Caches[masterEntity], masterRow);
            _Graph.Caches[typeof(Note)].IsDirty = dirtystate;
            return result;
        }

        protected virtual void UpdateKey(APVendorRefNbr vrn)
        {
            DeleteKey(vrn);
            PXDatabase.Insert<APVendorRefNbr>(
                new PXDataFieldAssign<APVendorRefNbr.masterID>(vrn.MasterID),
                new PXDataFieldAssign<APVendorRefNbr.detailID>(vrn.DetailID),
                new PXDataFieldAssign<APVendorRefNbr.vendorID>(vrn.VendorID),
                new PXDataFieldAssign<APVendorRefNbr.vendorDocumentID>(vrn.VendorDocumentID),
                new PXDataFieldAssign<APVendorRefNbr.siblingID>(vrn.SiblingID));
        }

        protected virtual void DeleteKey(APVendorRefNbr vrn)
        {
            PXDatabase.Delete<APVendorRefNbr>(
             new PXDataFieldRestrict<APVendorRefNbr.masterID>(vrn.MasterID),
             new PXDataFieldRestrict<APVendorRefNbr.detailID>(vrn.DetailID));
        }

        protected virtual APVendorRefNbr InitVendorRefObject(PXCache sender, object row)
        {
            var vrn = new APVendorRefNbr();
            var ek = GetEntityKey(sender, row);
            if (ek == null)
                return null;
            vrn.MasterID = ek._MasterID;
            vrn.DetailID = ek._DetailID;
            APVendorRefNbr located = null;
           
            located = PXSelect<APVendorRefNbr, 
                Where<APVendorRefNbr.masterID, Equal<Current<APVendorRefNbr.masterID>>,
                And<APVendorRefNbr.detailID, Equal<Current<APVendorRefNbr.detailID>>>>>.SelectSingleBound(_Graph, new object[] { vrn });
           
            if (located == null)
                located = vrn;
            located.IsProcessed = false;
            located.SiblingID = GetSiblingID(sender, row);
            located.IsIgnored = this.IsIgnored(sender, row);
            located.IsChecked = this.IsRequiredUniqueRefNbr(sender, row);
            located.VendorID = (Int32?)sender.GetValue(row, _VendorID.Name);
            located.VendorDocumentID = Convert.ToString(sender.GetValue(row, _FieldName));
            return located;
        }

        protected virtual void CheckUniqueness(PXCache sender, object row)
        {
            var vendorref = InitVendorRefObject(sender, row);
            CheckUniqueness(vendorref);
        }

        protected virtual void CheckUniqueness(APVendorRefNbr vendorref)
        {
            if (vendorref.SiblingID == null || vendorref.MasterID == null || vendorref.DetailID == null || String.IsNullOrEmpty(vendorref.VendorDocumentID) || vendorref.VendorID == null)
                return;
            APVendorRefNbr dup = PXSelect<APVendorRefNbr,
                    Where<
                        APVendorRefNbr.vendorID, Equal<Current<APVendorRefNbr.vendorID>>,
                        And<APVendorRefNbr.vendorDocumentID, Equal<Current<APVendorRefNbr.vendorDocumentID>>,
                        And<APVendorRefNbr.siblingID, NotEqual<Current<APVendorRefNbr.siblingID>>,
                        And<APVendorRefNbr.isIgnored, NotEqual<True>,
                        And < Where < APVendorRefNbr.masterID, NotEqual<Current<APVendorRefNbr.masterID>>,
                        Or < APVendorRefNbr.detailID, NotEqual < Current < APVendorRefNbr.detailID >>>>>>>>>>
                        .SelectSingleBound(_Graph, new object[] { vendorref });
            if (dup != null)
            {
                throw new PXSetPropertyException(GetExceptionMessage(dup));
            }

        }

        protected virtual String GetExceptionMessage(APVendorRefNbr row)
        {
            if (row.DetailID == DETAIL_DUMMY)
            {
                return PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.EntityDuplicateInvoiceNbr, row.VendorDocumentID, EntityHelper.GetEntityRowID(row.MasterID));
            }
            return PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.SubEntityDuplicateInvoiceNbr, row.VendorDocumentID, row.DetailID.ToString(), EntityHelper.GetEntityRowID(row.MasterID));
        }

        public abstract Guid? GetSiblingID(PXCache sender, object row);

        protected abstract EntityKey GetEntityKey(PXCache sender, object row);

        protected virtual bool IsIgnored(PXCache sender, object row)
        {
            return String.IsNullOrEmpty(Convert.ToString(sender.GetValue(row, _FieldName))) || (Int32?)sender.GetValue(row, _VendorID.Name) == null;
        }

        protected virtual bool IsRequiredUniqueRefNbr(PXCache sender, object row)
		{
			PXCache APSetupCache = sender.Graph.Caches[typeof(APSetup)];
			return !IsIgnored(sender, row) && (bool?)APSetupCache.GetValue<APSetup.raiseErrorOnDoubleInvoiceNbr>(APSetupCache.Current) == true;
		}

        #endregion

    }


	public class APVendorRefNbrAttribute : BaseVendorRefNbrAttribute
	{
		#region Ctor
		public APVendorRefNbrAttribute() : base(typeof (APInvoice.vendorID))
		{
		}
        #endregion

        #region Implementation
        protected override bool IsRequiredUniqueRefNbr(PXCache sender, object row)
        {
            APInvoice r = (APInvoice)row;
            return r.InstallmentNbr == null && base.IsRequiredUniqueRefNbr(sender, row); // Feature multi instance...
        }

        protected override EntityKey GetEntityKey(PXCache sender, object row)
        {
            var ek = new EntityKey();
            ek._DetailID = DETAIL_DUMMY;
            ek._MasterID = GetMasterNoteId(typeof(APInvoice), typeof(APInvoice.noteID), row);
            
            return ek;
        }

        public override Guid? GetSiblingID(PXCache sender, object row)
        {
            APInvoice r = (APInvoice)row;
            if (r == null)
                return null;
            if (r.RefNoteID != null)
                return r.RefNoteID;
            if (r?.OrigDocType != null && r.OrigRefNbr != null)
            {
                APInvoice orig = PXSelectReadonly<APInvoice,
                                Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                                And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.SelectSingleBound(_Graph, null, r.OrigDocType, r.OrigRefNbr);
                if (orig == null)
                    return r.NoteID;
                return this.GetSiblingID(sender, orig);
            }
            return r.NoteID;
        }
        #endregion

    }

	public class POVendorRefNbrAttribute : BaseVendorRefNbrAttribute
	{
		#region Ctor
		public POVendorRefNbrAttribute() : base(typeof(PO.POReceipt.vendorID))
		{
		}
        #endregion

        #region Implementation
        protected override bool IsIgnored(PXCache sender, object row)
        {
            PO.POReceipt r = (PO.POReceipt)row;
            return r.ReceiptType == PO.POReceiptType.TransferReceipt || r.Released == true || r.AutoCreateInvoice != true || base.IsIgnored(sender, row);
        }

        protected override EntityKey GetEntityKey(PXCache sender, object row)
        {
            var ek = new EntityKey();
            ek._DetailID = DETAIL_DUMMY;
            ek._MasterID = GetMasterNoteId(typeof(PO.POReceipt), typeof(PO.POReceipt.noteID), row);

            return ek;
        }

        public override Guid? GetSiblingID(PXCache sender, object row)
        {
            return (Guid?)sender.GetValue<PO.POReceipt.noteID>(row);
        }
        #endregion
    }

    public class LCTranVendorRefNbrAttribute : BaseVendorRefNbrAttribute
	{
        #region State
        protected Guid? _MasterNoteID = null;
        #endregion

        #region Ctor
        public LCTranVendorRefNbrAttribute() : base(typeof(PO.LandedCostTran.vendorID))
		{
		}
		#endregion

		#region Initialization
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldUpdated.AddHandler<PO.LandedCostTran.postponeAP>(base.DataUpdatedCheck);
		}
		#endregion

		
		#region Implementation

        protected override bool IsIgnored(PXCache sender, object row)
        {
            return (bool?)sender.GetValue<PO.LandedCostTran.processed>(row) == true || (bool?)sender.GetValue<PO.LandedCostTran.postponeAP>(row) == true || base.IsIgnored(sender, row);
        }

        protected override EntityKey GetEntityKey(PXCache sender, object row)
        {
            var ek = new EntityKey();
            ek._DetailID = ((int?)sender.GetValue<PO.LandedCostTran.lineNbr>(row)).GetValueOrDefault();
            if (ek._DetailID == 0 || sender.GetValue<PO.LandedCostTran.pOReceiptNbr>(row) == null)
                return null;

            PO.POReceipt receiptRow = PXSelect<PO.POReceipt, Where<PO.POReceipt.receiptType, Equal<Current<PO.LandedCostTran.pOReceiptType>>, And<PO.POReceipt.receiptNbr, Equal<Current<PO.LandedCostTran.pOReceiptNbr>>>>>.SelectSingleBound(_Graph, new object[] { row });
            ek._MasterID = GetMasterNoteId(typeof(PO.POReceipt), typeof(PO.POReceipt.noteID), receiptRow);

            return ek;
        }

        public override Guid? GetSiblingID(PXCache sender, object row)
        {
            if (((int?)sender.GetValue<PO.LandedCostTran.lCTranID>(row)).GetValueOrDefault() == 0 || sender.GetValue<PO.LandedCostTran.pOReceiptNbr>(row) == null)
                return null;

            PXCache receiptCache = _Graph.Caches[typeof(PO.POReceipt)];
            PO.POReceipt receiptRow = PXSelect<PO.POReceipt, Where<PO.POReceipt.receiptType, Equal<Current<PO.LandedCostTran.pOReceiptType>>, And<PO.POReceipt.receiptNbr, Equal<Current<PO.LandedCostTran.pOReceiptNbr>>>>>.SelectSingleBound(_Graph, new object[] { row });

            return PXNoteAttribute.GetNoteID<PO.POReceipt.noteID>(receiptCache, receiptRow).Value;
        }

        #endregion

    }

}