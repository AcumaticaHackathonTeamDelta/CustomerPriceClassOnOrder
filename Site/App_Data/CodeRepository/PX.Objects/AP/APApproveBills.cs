using System;
using System.Collections;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.AP
{
	[TableAndChartDashboardType]
	public class APApproveBills : PXGraph<APApproveBills>
	{
		public PXFilter<ApproveBillsFilter> Filter;
		public PXSave<ApproveBillsFilter> Save;
		/// <summary>
		/// 
		/// </summary>
		public PXCancel<ApproveBillsFilter> Cancel;
		
		public PXAction<ApproveBillsFilter> ViewDocument;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (APDocumentList.Current != null)
			{
				PXRedirectHelper.TryRedirect(APDocumentList.Cache, APDocumentList.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		[PXFilterable]
		public PXSelect<APInvoice> APDocumentList;
		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;

		protected virtual void ApproveBillsFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled(APDocumentList.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<APInvoice.paySel>(APDocumentList.Cache, null, true);
						PXUIFieldAttribute.SetEnabled<APInvoice.payLocationID>(APDocumentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APInvoice.payAccountID>(APDocumentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APInvoice.payTypeID>(APDocumentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APInvoice.payDate>(APDocumentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APInvoice.separateCheck>(APDocumentList.Cache, null, true);

			PXUIFieldAttribute.SetVisible<ApproveBillsFilter.curyID>(sender, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			PXUIFieldAttribute.SetDisplayName<APInvoice.selected>(APDocumentList.Cache, Messages.Approve);
			PXUIFieldAttribute.SetDisplayName<APInvoice.vendorID>(APDocumentList.Cache, Messages.VendorID);

			APDocumentList.Cache.AllowInsert = APDocumentList.Cache.AllowDelete = false;

			ApproveBillsFilter row = e.Row as ApproveBillsFilter;
			if (row != null)
			{
				row.Days = PXMessages.LocalizeNoPrefix(row.Days);
			}
		}

		protected virtual void APInvoice_PayLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<APInvoice.separateCheck>(e.Row);
			sender.SetDefaultExt<APInvoice.payAccountID>(e.Row);
			sender.SetDefaultExt<APInvoice.payTypeID>(e.Row);
		}

		protected virtual void APInvoice_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			APInvoice doc = (APInvoice)e.Row;

						if (doc.PaySel == true && doc.PayDate == null)
						{
							sender.RaiseExceptionHandling<APInvoice.payDate>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APInvoice.payDate).Name));
						}

			if (doc.PaySel == true && doc.DocDate != null && doc.PayDate != null 
				&& ((DateTime)doc.DocDate).CompareTo((DateTime)doc.PayDate) > 0)
			{
							sender.RaiseExceptionHandling<APInvoice.payDate>(e.Row, doc.PayDate, new PXSetPropertyException(Messages.ApplDate_Less_DocDate, PXErrorLevel.RowError, typeof(APInvoice.payDate).Name));
			}

						if (doc.PaySel == true && doc.PayLocationID == null)
						{
							sender.RaiseExceptionHandling<APInvoice.payLocationID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APInvoice.payLocationID).Name));
						}

						if (doc.PaySel == true && doc.PayAccountID == null)
						{
							sender.RaiseExceptionHandling<APInvoice.payAccountID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APInvoice.payAccountID).Name));
						}

						if (doc.PaySel == true && doc.PayTypeID == null)
						{
							sender.RaiseExceptionHandling<APInvoice.payTypeID>(doc, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(APInvoice.payTypeID).Name));
						}
		}

		protected virtual IEnumerable apdocumentlist()
		{
			ApproveBillsFilter filter = Filter.Current;

			if (filter != null && filter.SelectionDate != null)
			{
								DateTime PayInLessThan = ((DateTime)filter.SelectionDate).AddDays(filter.PayInLessThan.GetValueOrDefault());
								DateTime DueInLessThan = ((DateTime)filter.SelectionDate).AddDays(filter.DueInLessThan.GetValueOrDefault());
								DateTime DiscountExpiresInLessThan = ((DateTime)filter.SelectionDate).AddDays(filter.DiscountExpiresInLessThan.GetValueOrDefault());

				foreach (PXResult<APInvoice, Vendor, APAdjust> res in PXSelectJoin<APInvoice,
					InnerJoin<Vendor, On<Vendor.bAccountID, Equal<APInvoice.vendorID>>,
					LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>,
							 And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>,
							 And<APAdjust.released, Equal<False>>>>,
					LeftJoin<APPayment, On<APPayment.docType, Equal<APInvoice.docType>,
							And<APPayment.refNbr, Equal<APInvoice.refNbr>,
							And<Where<APPayment.docType, Equal<APDocType.prepayment>, Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>>,
					Where<APInvoice.openDoc, Equal<True>,
						And2<Where<APInvoice.released, Equal<True>, Or<APInvoice.prebooked, Equal<True>>>,
						And<APAdjust.adjgRefNbr, IsNull,
						And<APPayment.refNbr, IsNull,
						And2<Match<Vendor, Current<AccessInfo.userName>>,
						And2<Where<APInvoice.curyID, Equal<Current<ApproveBillsFilter.curyID>>,
							   Or<Current<ApproveBillsFilter.curyID>, IsNull>>,
						And2<Where2<Where<Current<ApproveBillsFilter.showApprovedForPayment>, Equal<True>,
										And<APInvoice.paySel, Equal<True>>>,
								 Or<Where<Current<ApproveBillsFilter.showNotApprovedForPayment>, Equal<True>,
										And<APInvoice.paySel, Equal<False>>>>>,
						And2<Where<Vendor.bAccountID, Equal<Current<ApproveBillsFilter.vendorID>>,
								 Or<Current<ApproveBillsFilter.vendorID>, IsNull>>,
						And2<Where<Vendor.vendorClassID, Equal<Current<ApproveBillsFilter.vendorClassID>>,
								 Or<Current<ApproveBillsFilter.vendorClassID>, IsNull>>,
						And<Where2<Where2<Where<Current<ApproveBillsFilter.showPayInLessThan>, Equal<True>, And<APInvoice.payDate, LessEqual<Required<APInvoice.payDate>>>>,
										Or2<Where<Current<ApproveBillsFilter.showDueInLessThan>, Equal<True>,
											And<APInvoice.dueDate, LessEqual<Required<APInvoice.dueDate>>>>,
										Or<Where<Current<ApproveBillsFilter.showDiscountExpiresInLessThan>, Equal<True>,
											And<APInvoice.discDate, LessEqual<Required<APInvoice.discDate>>>>>>>,
																Or<Where<Current<ApproveBillsFilter.showPayInLessThan>, Equal<False>,
									   And<Current<ApproveBillsFilter.showDueInLessThan>, Equal<False>,
											And<Current<ApproveBillsFilter.showDiscountExpiresInLessThan>, Equal<False>>>>>>>>>>>>>>>>>
					.Select(this, PayInLessThan, DueInLessThan, DiscountExpiresInLessThan))
				{
					APInvoice apdoc = res;

					if (string.IsNullOrEmpty(apdoc.PayTypeID))
					{
						try
						{
						APDocumentList.Cache.SetDefaultExt<APInvoice.payTypeID>(apdoc);
					}
						catch (PXSetPropertyException e)
						{
							APDocumentList.Cache.RaiseExceptionHandling<APInvoice.payTypeID>(apdoc, apdoc.PayTypeID, e);
						}
					}

					if (apdoc.PayAccountID == null)
					{
						try
						{
						APDocumentList.Cache.SetDefaultExt<APInvoice.payAccountID>(apdoc);
					}
						catch (PXSetPropertyException e)
						{
							APDocumentList.Cache.RaiseExceptionHandling<APInvoice.payAccountID>(apdoc, apdoc.PayAccountID, e);
						}
					}

					yield return apdoc;
				}
			}
		}

		public PXSetup<APSetup> APSetup;

		public APApproveBills()
		{
			APSetup setup = APSetup.Current;
			Views["_ApproveBillsFilter_curyInfoID_CurrencyInfo.CuryInfoID_"] = new PXView(this, false, new Select<ApproveBillsFilter>(), new PXSelectDelegate(Filter.Get));
			PXUIFieldAttribute.SetDisplayName<APInvoice.paySel>(APDocumentList.Cache, PXLocalizer.Localize(Messages.Selected, typeof(Messages).FullName));
		}

		protected virtual IEnumerable filter()
				{
			decimal approvedTotal = 0m;
			decimal docsTotal = 0m;

			foreach (APInvoice res in APDocumentList.Select())
				{
				approvedTotal += res.PaySel == true ? (Accessinfo.CuryViewState ? (res.DocBal ?? 0m) : (res.CuryDocBal ?? 0m)) : 0m;
				docsTotal += Accessinfo.CuryViewState ? (res.DocBal ?? 0m) : (res.CuryDocBal ?? 0m);
			}

			Filter.Current.CuryApprovedTotal = approvedTotal;
			Filter.Current.CuryDocsTotal = docsTotal;

			yield return Filter.Current;
		}
	}

	[Serializable]
	public partial class ApproveBillsFilter : PX.Data.IBqlTable
	{
		#region SelectionDate
		public abstract class selectionDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _SelectionDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Selection Date", Required = true, Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? SelectionDate
		{
			get
			{
				return this._SelectionDate;
			}
			set
			{
				this._SelectionDate = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(Visibility = PXUIVisibility.SelectorVisible, Required = false, DescriptionField = typeof(Vendor.acctName))]
		[PXDefault()]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region VendorClassID
		public abstract class vendorClassID : PX.Data.IBqlField
		{
		}
		protected String _VendorClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(VendorClass.vendorClassID), DescriptionField = typeof(VendorClass.descr))]
		[PXUIField(DisplayName = "Vendor Class", Required = false, Visibility = PXUIVisibility.SelectorVisible)]

		public virtual String VendorClassID
		{
			get
			{
				return this._VendorClassID;
			}
			set
			{
				this._VendorClassID = value;
			}
		}
		#endregion
		#region PayInLessThan
		public abstract class payInLessThan : PX.Data.IBqlField
		{
		}
		protected Int16? _PayInLessThan;
		[PXDBShort()]
		[PXUIField(Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(APSetup.paymentLeadTime), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int16? PayInLessThan
		{
			get
			{
				return this._PayInLessThan;
			}
			set
			{
				this._PayInLessThan = value;
			}
		}
		#endregion
		#region ShowPayInLessThan
		public abstract class showPayInLessThan : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowPayInLessThan;
		[PXDBBool()]
		[PXDefault(true)]
			[PXUIField(DisplayName = "Pay Date Within", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? ShowPayInLessThan
		{
			get
			{
				return this._ShowPayInLessThan;
			}
			set
			{
				this._ShowPayInLessThan = value;
			}
		}
		#endregion
		#region DueInLessThan
		public abstract class dueInLessThan : PX.Data.IBqlField
		{
		}
		protected Int16? _DueInLessThan;
		[PXDBShort()]
		[PXUIField(Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(APSetup.paymentLeadTime), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int16? DueInLessThan
		{
			get
			{
				return this._DueInLessThan;
			}
			set
			{
				this._DueInLessThan = value;
			}
		}
		#endregion
		#region ShowDueInLessThan
		public abstract class showDueInLessThan : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowDueInLessThan;
		[PXDBBool()]
		[PXDefault(false)]
			[PXUIField(DisplayName = "Due Date Within", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? ShowDueInLessThan
		{
			get
			{
				return this._ShowDueInLessThan;
			}
			set
			{
				this._ShowDueInLessThan = value;
			}
		}
		#endregion
		#region DiscountExpiredInLessThan
		public abstract class discountExpiresInLessThan : PX.Data.IBqlField
		{
		}
		protected Int16? _DiscountExpiresInLessThan;
		[PXDBShort()]
		[PXUIField(Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(APSetup.paymentLeadTime), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int16? DiscountExpiresInLessThan
		{
			get
			{
				return this._DiscountExpiresInLessThan;
			}
			set
			{
				this._DiscountExpiresInLessThan = value;
			}
		}
		#endregion
		#region ShowDiscountExpiresInLessThan
		public abstract class showDiscountExpiresInLessThan : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowDiscountExpiresInLessThan;
		[PXDBBool()]
		[PXDefault(false)]
			[PXUIField(DisplayName = "Cash Discount Expires Within", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? ShowDiscountExpiresInLessThan
		{
			get
			{
				return this._ShowDiscountExpiresInLessThan;
			}
			set
			{
				this._ShowDiscountExpiresInLessThan = value;
			}
		}
		#endregion
		#region ShowApprovedForPayment
		public abstract class showApprovedForPayment : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowApprovedForPayment;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Show Approved For Payment", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? ShowApprovedForPayment
		{
			get
			{
				return this._ShowApprovedForPayment;
			}
			set
			{
				this._ShowApprovedForPayment = value;
			}
		}
		#endregion
		#region ShowNotApprovedForPayment
		public abstract class showNotApprovedForPayment : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowNotApprovedForPayment;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Show Not Approved For Payment", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? ShowNotApprovedForPayment
		{
			get
			{
				return this._ShowNotApprovedForPayment;
			}
			set
			{
				this._ShowNotApprovedForPayment = value;
			}
		}
		#endregion
		#region CuryDocsTotal
		public abstract class curyDocsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ApproveBillsFilter.curyInfoID), typeof(ApproveBillsFilter.docsTotal))]
		[PXUIField(DisplayName = "Documents Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryDocsTotal
		{
			get
			{
				return this._CuryDocsTotal;
			}
			set
			{
				this._CuryDocsTotal = value;
			}
		}
		#endregion
		#region CuryApprovedTotal
		public abstract class curyApprovedTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryApprovedTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ApproveBillsFilter.curyInfoID), typeof(ApproveBillsFilter.docsTotal))]
		[PXUIField(DisplayName = "Approved For Payment", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryApprovedTotal
		{
			get
			{
				return this._CuryApprovedTotal;
			}
			set
			{
				this._CuryApprovedTotal = value;
			}
		}
		#endregion
		#region DocsTotal
		public abstract class docsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocsTotal;
		[PXDBDecimal(4)]
		public virtual Decimal? DocsTotal
		{
			get
			{
				return this._DocsTotal;
			}
			set
			{
				this._DocsTotal = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, Required = false)]
		[PXDefault()]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		//[CurrencyInfo(ModuleCode = "AP")]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region Days
		public abstract class days : PX.Data.IBqlField
		{
		}
		protected String _Days;
		[PXDBString]
		[PXUIField]
		[PXDefault("Days", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string Days
		{
			get
			{
				return this._Days;
			}
			set
			{
				this._Days = value;
			}
		}
		#endregion
	}
}
