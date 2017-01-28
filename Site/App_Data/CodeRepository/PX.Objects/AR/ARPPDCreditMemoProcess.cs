using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.TX;


namespace PX.Objects.AR
{
	[System.SerializableAttribute()]
	public partial class ARPPDCreditMemoParameters : IBqlTable
	{
		#region ApplicationDate
		public abstract class applicationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ApplicationDate;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible, Required = true)]
		public virtual DateTime? ApplicationDate
		{
			get
			{
				return _ApplicationDate;
			}
			set
			{
				_ApplicationDate = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected int? _BranchID;
		[Branch]
		public virtual int? BranchID
		{
			get
			{
				return _BranchID;
			}
			set
			{
				_BranchID = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected int? _CustomerID;
		[Customer]
		public virtual int? CustomerID
		{
			get
			{
				return _CustomerID;
			}
			set
			{
				_CustomerID = value;
			}
		}
		#endregion
		#region GenerateOnePerCustomer
		public abstract class generateOnePerCustomer : PX.Data.IBqlField
		{
		}
		protected bool? _GenerateOnePerCustomer;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Generate One Credit Memo per Customer", Visibility = PXUIVisibility.Visible)]
		public virtual bool? GenerateOnePerCustomer
		{
			get
			{
				return _GenerateOnePerCustomer;
			}
			set
			{
				_GenerateOnePerCustomer = value;
			}
		}
		#endregion
		#region CreditMemoDate
		public abstract class creditMemoDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreditMemoDate;
		[PXDBDate]
		[PXFormula(typeof(Switch<Case<Where<ARPPDCreditMemoParameters.generateOnePerCustomer, Equal<True>>, Current<AccessInfo.businessDate>>, Null>))]
		[PXUIField(DisplayName = "Credit Memo Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? CreditMemoDate
		{
			get
			{
				return _CreditMemoDate;
			}
			set
			{
				_CreditMemoDate = value;
			}
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected string _FinPeriodID;
		[AROpenPeriod(typeof(ARPPDCreditMemoParameters.creditMemoDate))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.Visible)]
		public virtual string FinPeriodID
		{
			get
			{
				return _FinPeriodID;
			}
			set
			{
				_FinPeriodID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<ARAdjust,
		InnerJoin<AR.ARInvoice, On<AR.ARInvoice.docType, Equal<ARAdjust.adjdDocType>, And<AR.ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>>,
		Where<AR.ARInvoice.released, Equal<True>,
			And<AR.ARInvoice.pendingPPD, Equal<True>,
			And<AR.ARInvoice.openDoc, Equal<True>,
			And<ARAdjust.released, Equal<True>,
			And<ARAdjust.voided, NotEqual<True>,
			And<ARAdjust.pendingPPD, Equal<True>,
			And<ARAdjust.pPDCrMemoRefNbr, IsNull,
			And<Where<ARAdjust.adjgDocType, Equal<ARDocType.payment>,
				Or<ARAdjust.adjgDocType, Equal<ARDocType.prepayment>>>>>>>>>>>>))]
	[Serializable]
	public partial class PendingPPDCreditMemoApp : ARAdjust
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region Index
		public abstract class index : PX.Data.IBqlField
		{
		}
		protected int _Index;
		[PXInt]
		public virtual int Index
		{
			get
			{
				return _Index;
			}
			set
			{
				_Index = value;
			}
		}
		#endregion

		#region ARAdjust key fields

		#region PayDocType
		public abstract class payDocType : PX.Data.IBqlField
		{
		}
		protected string _PayDocType;
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "", BqlField = typeof(ARAdjust.adjgDocType))]
		public virtual string PayDocType
		{
			get
			{
				return _PayDocType;
			}
			set
			{
				_PayDocType = value;
			}
		}
		#endregion
		#region PayRefNbr
		public abstract class payRefNbr : PX.Data.IBqlField
		{
		}
		protected string _PayRefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARAdjust.adjgRefNbr))]
		public virtual string PayRefNbr
		{
			get
			{
				return _PayRefNbr;
			}
			set
			{
				_PayRefNbr = value;
			}
		}
		#endregion
		#region InvDocType
		public abstract class invDocType : PX.Data.IBqlField
		{
		}
		protected string _InvDocType;
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "", BqlField = typeof(ARAdjust.adjdDocType))]
		public virtual string InvDocType
		{
			get
			{
				return _InvDocType;
			}
			set
			{
				_InvDocType = value;
			}
		}
		#endregion
		#region InvRefNbr
		public abstract class invRefNbr : PX.Data.IBqlField
		{
		}
		protected string _InvRefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(ARAdjust.adjdRefNbr))]
		public virtual string InvRefNbr
		{
			get
			{
				return _InvRefNbr;
			}
			set
			{
				_InvRefNbr = value;
			}
		}
		#endregion
		
		#endregion
		#region ARInvoice fields

		#region InvCuryID
		public abstract class invCuryID : PX.Data.IBqlField
		{
		}
		protected string _InvCuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(AR.ARInvoice.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string InvCuryID
		{
			get
			{
				return _InvCuryID;
			}
			set
			{
				_InvCuryID = value;
			}
		}
		#endregion

		#region InvCuryInfoID
		public abstract class invCuryInfoID : IBqlField {}
		[PXDBLong(BqlField = typeof(AR.ARInvoice.curyInfoID))]
		[CurrencyInfo(ModuleCode = nameof(AR), CuryIDField = nameof(InvCuryID))]
		public virtual long? InvCuryInfoID { get; set; }
		#endregion

		#region InvCustomerLocationID
		public abstract class invCustomerLocationID : IBqlField { }
		protected int? _InvCustomerLocationID;
		[PXDBInt(BqlField = typeof(AR.ARInvoice.customerLocationID))]
		public virtual int? InvCustomerLocationID
		{
			get
			{
				return _InvCustomerLocationID;
			}
			set
			{
				_InvCustomerLocationID = value;
			}
		}
		#endregion
		#region InvTaxZoneID
		public abstract class invTaxZoneID : PX.Data.IBqlField
		{
		}
		protected string _InvTaxZoneID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(AR.ARInvoice.taxZoneID))]
		public virtual string InvTaxZoneID
		{
			get
			{
				return _InvTaxZoneID;
			}
			set
			{
				_InvTaxZoneID = value;
			}
		}
		#endregion
		#region InvTermsID
		public abstract class invTermsID : PX.Data.IBqlField
		{
		}
		protected string _InvTermsID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(AR.ARInvoice.termsID))]
		[PXUIField(DisplayName = "Credit Terms", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.customer>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		[Terms(typeof(AR.ARInvoice.docDate), typeof(AR.ARInvoice.dueDate), typeof(AR.ARInvoice.discDate), typeof(AR.ARInvoice.curyOrigDocAmt), typeof(AR.ARInvoice.curyOrigDiscAmt))]
		public virtual string InvTermsID
		{
			get
			{
				return _InvTermsID;
			}
			set
			{
				_InvTermsID = value;
			}
		}
		#endregion
		#region InvCuryOrigDocAmt
		public abstract class invCuryOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _InvCuryOrigDocAmt;
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.origDocAmt), BqlField = typeof(AR.ARInvoice.curyOrigDocAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryOrigDocAmt
		{
			get
			{
				return _InvCuryOrigDocAmt;
			}
			set
			{
				_InvCuryOrigDocAmt = value;
			}
		}
		#endregion
		#region InvCuryOrigDiscAmt
		public abstract class invCuryOrigDiscAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _InvCuryOrigDiscAmt;
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.origDiscAmt), BqlField = typeof(AR.ARInvoice.curyOrigDiscAmt))]
		[PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual decimal? InvCuryOrigDiscAmt
		{
			get
			{
				return _InvCuryOrigDiscAmt;
			}
			set
			{
				_InvCuryOrigDiscAmt = value;
			}
		}
		#endregion
		#region InvCuryVatTaxableTotal
		public abstract class invCuryVatTaxableTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _InvCuryVatTaxableTotal;
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.vatTaxableTotal), BqlField = typeof(AR.ARInvoice.curyVatTaxableTotal))]
		[PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryVatTaxableTotal
		{
			get
			{
				return _InvCuryVatTaxableTotal;
			}
			set
			{
				_InvCuryVatTaxableTotal = value;
			}
		}
		#endregion
		#region InvCuryTaxTotal
		public abstract class invCuryTaxTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _InvCuryTaxTotal;
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(AR.ARInvoice.taxTotal), BqlField = typeof(AR.ARInvoice.curyTaxTotal))]
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryTaxTotal
		{
			get
			{
				return _InvCuryTaxTotal;
			}
			set
			{
				_InvCuryTaxTotal = value;
			}
		}
		#endregion
		#region InvCuryDocBal
		public abstract class invCuryDocBal : PX.Data.IBqlField
		{
		}
		protected decimal? _InvCuryDocBal;
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.docBal), BaseCalc = false, BqlField = typeof(AR.ARInvoice.curyDocBal))]
		public virtual decimal? InvCuryDocBal
		{
			get
			{
				return _InvCuryDocBal;
			}
			set
			{
				_InvCuryDocBal = value;
			}
		}
		#endregion
		
		#endregion
	}

	public class PPDCreditMemoKey
	{
		private readonly FieldInfo[] _fields;
		
		public int? BranchID;
		public int? CustomerID;
		public int? CustomerLocationID;
		public string CuryID;
		public decimal? CuryRate;
		public int? ARAccountID;
		public int? ARSubID;
		public string TaxZoneID;

		public PPDCreditMemoKey()
		{
			_fields = GetType().GetFields();
		}

		public override bool Equals(object obj)
		{
			FieldInfo info = _fields.FirstOrDefault(field => !Equals(field.GetValue(this), field.GetValue(obj)));
			return info == null;
		}
		public override int GetHashCode()
		{
			int hashCode = 17;
			_fields.ForEach(field => hashCode = hashCode * 23 + field.GetValue(this).GetHashCode());
			return hashCode;
		}
	}

	[TableAndChartDashboardType]
	public class ARPPDCreditMemoProcess : PXGraph<ARPPDCreditMemoProcess>
	{
		public PXCancel<ARPPDCreditMemoParameters> Cancel;
		public PXFilter<ARPPDCreditMemoParameters> Filter;
		
		[PXFilterable]
		public PXFilteredProcessing<PendingPPDCreditMemoApp, ARPPDCreditMemoParameters> Applications;
		public PXSetup<ARSetup> arsetup;
        
        public override bool IsDirty
		{
			get { return false; }
        }

		#region Cache Attached
		[Customer]
		protected virtual void PendingPPDCreditMemoApp_AdjdCustomerID_CacheAttached(PXCache sender) { }

		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARInvoiceType.RefNbr(typeof(Search2<
			Standalone.ARRegisterAlias.refNbr,
				InnerJoinSingleTable<ARInvoice, 
					On<ARInvoice.docType, Equal<Standalone.ARRegisterAlias.docType>,
					And<ARInvoice.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
				InnerJoinSingleTable<Customer, 
					On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<
				Standalone.ARRegisterAlias.docType, Equal<Optional<PendingPPDCreditMemoApp.invDocType>>,
				And2<Where<
					Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleAR>, 
					Or<Standalone.ARRegisterAlias.released, Equal<True>>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>>, 
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>))]
		[ARInvoiceType.Numbering]
		[ARInvoiceNbr]
		[PXFieldDescription]
		protected virtual void PendingPPDCreditMemoApp_AdjdRefNbr_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIField(DisplayName = "Doc. Date")]
		protected virtual void PendingPPDCreditMemoApp_AdjdDocDate_CacheAttached(PXCache sender) { }

		[PXDBCurrency(typeof(ARAdjust.adjdCuryInfoID), typeof(ARAdjust.adjPPDAmt))]
		[PXUIField(DisplayName = "Cash Discount")]
		protected virtual void PendingPPDCreditMemoApp_CuryAdjdPPDAmt_CacheAttached(PXCache sender) { }

		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Payment Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARPaymentType.RefNbr(typeof(Search2<
			Standalone.ARRegisterAlias.refNbr,
				InnerJoinSingleTable<ARPayment, 
					On<ARPayment.docType, Equal<Standalone.ARRegisterAlias.docType>,
					And<ARPayment.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
				InnerJoinSingleTable<Customer, 
					On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<
				Standalone.ARRegisterAlias.docType, Equal<Current<PendingPPDCreditMemoApp.payDocType>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>,
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>))]
		[ARPaymentType.Numbering]
		[PXFieldDescription]
		protected virtual void PendingPPDCreditMemoApp_AdjgRefNbr_CacheAttached(PXCache sender) { }

		#endregion

		public ARPPDCreditMemoProcess()
		{
			Applications.AllowDelete = true;
			Applications.AllowInsert = false;
			Applications.SetSelected<PendingPPDCreditMemoApp.selected>();
		}

		public virtual IEnumerable applications(PXAdapter adapter)
		{
			ARPPDCreditMemoParameters filter = Filter.Current;
			if (filter == null || filter.ApplicationDate == null || filter.BranchID == null) yield break;

			PXSelectBase<PendingPPDCreditMemoApp> select = new PXSelect<PendingPPDCreditMemoApp,
				Where<PendingPPDCreditMemoApp.adjgDocDate, LessEqual<Current<ARPPDCreditMemoParameters.applicationDate>>,
					And<PendingPPDCreditMemoApp.adjdBranchID, Equal<Current<ARPPDCreditMemoParameters.branchID>>>>>(this);

			if (filter.CustomerID != null)
			{
				select.WhereAnd<Where<PendingPPDCreditMemoApp.customerID, Equal<Current<ARPPDCreditMemoParameters.customerID>>>>();
			}

			foreach (PendingPPDCreditMemoApp res in select.Select())
			{
				yield return res;
			}

			Filter.Cache.IsDirty = false;
		}

		protected virtual void ARPPDCreditMemoParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARPPDCreditMemoParameters filter = (ARPPDCreditMemoParameters)e.Row;
			if (filter == null) return;

			ARSetup setup = arsetup.Current;
			Applications.SetProcessDelegate(list => CreatePPDCreditMemos(sender, filter, setup, list));

			bool generateOnePerCustomer = filter.GenerateOnePerCustomer == true;
			PXUIFieldAttribute.SetEnabled<ARPPDCreditMemoParameters.creditMemoDate>(sender, filter, generateOnePerCustomer);
			PXUIFieldAttribute.SetEnabled<ARPPDCreditMemoParameters.finPeriodID>(sender, filter, generateOnePerCustomer);
			PXUIFieldAttribute.SetRequired<ARPPDCreditMemoParameters.creditMemoDate>(sender, generateOnePerCustomer);
			PXUIFieldAttribute.SetRequired<ARPPDCreditMemoParameters.finPeriodID>(sender, generateOnePerCustomer);
		}

		public virtual void ARPPDCreditMemoParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARPPDCreditMemoParameters row = (ARPPDCreditMemoParameters)e.Row;
			ARPPDCreditMemoParameters oldRow = (ARPPDCreditMemoParameters)e.OldRow;
			if (row == null || oldRow == null) return;

			if (!sender.ObjectsEqual<ARPPDCreditMemoParameters.applicationDate, ARPPDCreditMemoParameters.branchID, ARPPDCreditMemoParameters.customerID>(oldRow, row))
			{
				Applications.Cache.Clear();
				Applications.Cache.ClearQueryCache();
			}
		}

		protected virtual void ARPPDCreditMemoParameters_GenerateOnePerCustomer_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARPPDCreditMemoParameters filter = (ARPPDCreditMemoParameters)e.Row;
			if (filter == null) return;

			if (filter.GenerateOnePerCustomer != true && (bool?)e.OldValue == true)
			{
				filter.CreditMemoDate = null;
				filter.FinPeriodID = null;
				
				sender.SetValuePending<ARPPDCreditMemoParameters.creditMemoDate>(filter, null);
				sender.SetValuePending<ARPPDCreditMemoParameters.finPeriodID>(filter, null);
			}
		}

		public static void CreatePPDCreditMemos(PXCache cache, ARPPDCreditMemoParameters filter, ARSetup setup, List<PendingPPDCreditMemoApp> docs)
		{
			int i = 0;
			bool failed = false;

			List<ARRegister> toRelease = new List<ARRegister>();
			ARInvoiceEntry ie = PXGraph.CreateInstance<ARInvoiceEntry>();
			
			if (filter.GenerateOnePerCustomer == true)
			{
				if (filter.CreditMemoDate == null)
					throw new PXSetPropertyException(CR.Messages.EmptyValueErrorFormat,
						PXUIFieldAttribute.GetDisplayName<ARPPDCreditMemoParameters.creditMemoDate>(cache));

				if (filter.FinPeriodID == null)
					throw new PXSetPropertyException(CR.Messages.EmptyValueErrorFormat,
						PXUIFieldAttribute.GetDisplayName<ARPPDCreditMemoParameters.finPeriodID>(cache));

				Dictionary<PPDCreditMemoKey, List<PendingPPDCreditMemoApp>> dict = new Dictionary<PPDCreditMemoKey, List<PendingPPDCreditMemoApp>>();
				foreach (PendingPPDCreditMemoApp doc in docs)
				{
					CurrencyInfo info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(ie, doc.InvCuryInfoID);

					PPDCreditMemoKey key = new PPDCreditMemoKey();
					doc.Index = i++;
					key.BranchID = doc.AdjdBranchID;
					key.CustomerID = doc.AdjdCustomerID;
					key.CustomerLocationID = doc.InvCustomerLocationID;
					key.CuryID = info.CuryID;
					key.CuryRate = info.CuryRate;
					key.ARAccountID = doc.AdjdARAcct;
					key.ARSubID = doc.AdjdARSub;
					key.TaxZoneID = doc.InvTaxZoneID;

					List<PendingPPDCreditMemoApp> list;
					if (!dict.TryGetValue(key, out list))
					{
						dict[key] = list = new List<PendingPPDCreditMemoApp>();
					}

					list.Add(doc);
				}

				foreach (List<PendingPPDCreditMemoApp> list in dict.Values)
				{
					ARInvoice invoice = CreatePPDCreditMemo(ie, filter, setup, list);
					if (invoice != null) { toRelease.Add(invoice); }
					else failed = true;
				}
			}
			else foreach (PendingPPDCreditMemoApp doc in docs)
			{
				List<PendingPPDCreditMemoApp> list = new List<PendingPPDCreditMemoApp>(1);
				doc.Index = i++;
				list.Add(doc);

				ARInvoice invoice = CreatePPDCreditMemo(ie, filter, setup, list);
				if (invoice != null) { toRelease.Add(invoice); }
				else failed = true;
			}

			if (setup.AutoReleasePPDCreditMemo == true && toRelease.Count > 0)
			{
				using (new PXTimeStampScope(null))
				{
					ARDocumentRelease.ReleaseDoc(toRelease, true);
				}
			}

			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}

		private static ARInvoice CreatePPDCreditMemo(ARInvoiceEntry ie, ARPPDCreditMemoParameters filter, ARSetup setup, List<PendingPPDCreditMemoApp> list)
		{
			int index = 0;
			ARInvoice invoice;

			try
			{
				ie.Clear(PXClearOption.ClearAll);
				PXUIFieldAttribute.SetError(ie.Document.Cache, null, null, null);

				Customer customer = null;
				invoice = (ARInvoice)ie.Document.Cache.CreateInstance();

				bool firstApp = true;
				foreach (PendingPPDCreditMemoApp doc in list)
				{
					if (firstApp)
					{
						firstApp = false;
						index = doc.Index;

						customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(ie, doc.AdjdCustomerID);
						CurrencyInfo info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(ie, doc.InvCuryInfoID);
						info.CuryInfoID = null;
						info = ie.currencyinfo.Insert(info);

						invoice.DocType = ARDocType.CreditMemo;
						invoice.DocDate = filter.GenerateOnePerCustomer == true ? filter.CreditMemoDate : doc.AdjgDocDate;
						invoice.FinPeriodID = filter.GenerateOnePerCustomer == true ? filter.FinPeriodID : doc.AdjgFinPeriodID;
						invoice = PXCache<ARInvoice>.CreateCopy(ie.Document.Insert(invoice));

						invoice.CustomerID = doc.AdjdCustomerID;
						invoice.CustomerLocationID = doc.InvCustomerLocationID;
						invoice.CuryInfoID = info.CuryInfoID;
						invoice.CuryID = info.CuryID;
						invoice.DocDesc = setup.PPDCreditMemoDescr;
						invoice.BranchID = doc.AdjdBranchID;
						invoice.ARAccountID = doc.AdjdARAcct;
						invoice.ARSubID = doc.AdjdARSub;
						invoice.TaxZoneID = doc.InvTaxZoneID;
						invoice.Hold = false;
						invoice.PendingPPD = true;

						invoice = ie.Document.Update(invoice);
						
						invoice.DontPrint = true;
						invoice.DontEmail = true;
					}

					AddTaxesAndApplications(ie, doc, customer, invoice);
				}

				ie.ARDiscountDetails.Select().RowCast<ARInvoiceDiscountDetail>().
					ForEach(discountDetail => ie.ARDiscountDetails.Cache.Delete(discountDetail));

				if (setup.RequireControlTotal == true)
				{
					invoice.CuryOrigDocAmt = invoice.CuryDocBal;
					ie.Document.Cache.Update(invoice);
				}

				ie.Save.Press();
				string refNbr = invoice.RefNbr;
				list.ForEach(doc => PXUpdate<Set<ARAdjust.pPDCrMemoRefNbr, Required<ARAdjust.pPDCrMemoRefNbr>>, ARAdjust,
					Where<ARAdjust.adjdDocType, Equal<Required<ARAdjust.adjdDocType>>,
						And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
						And<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>,
						And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>,
						And<ARAdjust.released, Equal<True>,
						And<ARAdjust.voided, NotEqual<True>,
						And<ARAdjust.pendingPPD, Equal<True>>>>>>>>>
						.Update(ie, refNbr, doc.AdjdDocType, doc.AdjdRefNbr, doc.AdjgDocType, doc.AdjgRefNbr));
				PXProcessing<PendingPPDCreditMemoApp>.SetInfo(index, ActionsMessages.RecordProcessed);
			}
			catch (Exception e)
			{
				PXProcessing<PendingPPDCreditMemoApp>.SetError(index, e);
				invoice = null;
			}

			return invoice;
		}

		private static readonly Dictionary<string, string> DocTypes = new ARInvoiceType.AdjdListAttribute().ValueLabelDic;
		private static void AddTaxesAndApplications(ARInvoiceEntry ie, PendingPPDCreditMemoApp doc, Customer customer, ARInvoice invoice)
		{
			ARTaxTran artaxMax = null;
			decimal? TaxTotal = 0m;
			decimal? InclusiveTotal = 0m;
			decimal? DiscountedTaxableTotal = 0m;
			decimal? DiscountedPriceTotal = 0m;
			decimal CashDiscPercent = (decimal)(doc.CuryAdjdPPDAmt / doc.InvCuryOrigDocAmt);

			PXResultset<ARTaxTran> taxes = PXSelectJoin<ARTaxTran, 
				InnerJoin<Tax, On<Tax.taxID, Equal<ARTaxTran.taxID>>>, 
				Where<ARTaxTran.module, Equal<BatchModule.moduleAR>,
					And<ARTaxTran.tranType, Equal<Required<ARTaxTran.tranType>>,
					And<ARTaxTran.refNbr, Equal<Required<ARTaxTran.refNbr>>>>>>.Select(ie, doc.AdjdDocType, doc.AdjdRefNbr);

			//add taxes
			foreach (PXResult<ARTaxTran, Tax> res in taxes)
			{
				Tax tax = res;
				ARTaxTran artax = PXCache<ARTaxTran>.CreateCopy(res);
				ARTaxTran artaxNew = ie.Taxes.Search<ARTaxTran.taxID>(artax.TaxID);
				
				if (artaxNew == null)
				{
					artax.TranType = null;
					artax.RefNbr = null;
					artax.TaxPeriodID = null;
					artax.Released = false;
					artax.Voided = false;
					artax.CuryInfoID = invoice.CuryInfoID;

					TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(ie.Transactions.Cache, null, TaxCalc.NoCalc);
					artaxNew = ie.Taxes.Insert(artax);

					artaxNew.CuryTaxableAmt = 0m;
					artaxNew.CuryTaxAmt = 0m;
					artaxNew.TaxRate = artax.TaxRate;
				}

				bool isTaxable = CalculateDiscountedTaxes(ie.Taxes.Cache, artax, CashDiscPercent);
				DiscountedPriceTotal += artax.CuryDiscountedPrice;

				decimal? CuryTaxableAmt = artax.CuryTaxableAmt - artax.CuryDiscountedTaxableAmt;
				decimal? CuryTaxAmt = artax.CuryTaxAmt - artax.CuryDiscountedPrice;

				artaxNew.CuryTaxableAmt += CuryTaxableAmt;
				artaxNew.CuryTaxAmt += CuryTaxAmt;

				TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(ie.Transactions.Cache, null, TaxCalc.ManualCalc);
				ie.Taxes.Update(artaxNew);

				if (isTaxable)
				{
					DiscountedTaxableTotal += artax.CuryDiscountedTaxableAmt;
					if (artaxMax == null || artaxNew.CuryTaxableAmt > artaxMax.CuryTaxableAmt)
					{
						artaxMax = artaxNew;
					}
				}

				if (tax.TaxCalcLevel == CSTaxCalcLevel.Inclusive) { InclusiveTotal += CuryTaxAmt; }
				else { TaxTotal += CuryTaxAmt; }
			}

			//adjust taxes according to parent ARInvoice
			decimal? DiscountedInvTotal = doc.InvCuryOrigDocAmt - doc.InvCuryOrigDiscAmt;
			decimal? DiscountedDocTotal = DiscountedTaxableTotal + DiscountedPriceTotal;

			if (doc.InvCuryOrigDiscAmt == doc.CuryAdjdPPDAmt &&
				artaxMax != null &&
				doc.InvCuryVatTaxableTotal + doc.InvCuryTaxTotal == doc.InvCuryOrigDocAmt &&
				DiscountedDocTotal != DiscountedInvTotal)
			{
				artaxMax.CuryTaxableAmt += DiscountedDocTotal - DiscountedInvTotal;
				TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID, ARTaxAttribute>(ie.Transactions.Cache, null, TaxCalc.ManualCalc);
				ie.Taxes.Update(artaxMax);
			}

			//add document details
			ARTran tranNew = ie.Transactions.Insert();
			
			tranNew.BranchID = doc.AdjdBranchID;
			using (new PXLocaleScope(customer.LocaleName))
				tranNew.TranDesc = string.Format("{0} {1}, {2} {3}", PXMessages.LocalizeNoPrefix(DocTypes[doc.AdjdDocType]), doc.AdjdRefNbr, PXMessages.LocalizeNoPrefix(Messages.Payment), doc.AdjgRefNbr);
			tranNew.CuryExtPrice = doc.CuryAdjdPPDAmt - TaxTotal;
			tranNew.CuryTaxableAmt = tranNew.CuryExtPrice - InclusiveTotal;
			tranNew.CuryTaxAmt = TaxTotal + InclusiveTotal;
			tranNew.AccountID = customer.DiscTakenAcctID;
			tranNew.SubID = customer.DiscTakenSubID;
			tranNew.TaxCategoryID = null;
			tranNew.IsFree = true;
			tranNew.ManualDisc = true;
			tranNew.CuryDiscAmt = 0m;
			tranNew.DiscPct = 0m;
			tranNew.GroupDiscountRate = 1m;
			tranNew.DocumentDiscountRate = 1m;

			if (taxes.Count == 1)
			{
				ARTaxTran artax = taxes[0];
				ARTran artran = PXSelectJoin<ARTran,
					InnerJoin<ARTax, On<ARTax.tranType, Equal<ARTran.tranType>,
						And<ARTax.refNbr, Equal<ARTran.refNbr>,
						And<ARTax.lineNbr, Equal<ARTran.lineNbr>>>>>,
					Where<ARTax.tranType, Equal<Required<ARTax.tranType>>,
						And<ARTax.refNbr, Equal<Required<ARTax.refNbr>>,
						And<ARTax.taxID, Equal<Required<ARTax.taxID>>>>>,
					OrderBy<Asc<ARTran.lineNbr>>>.SelectSingleBound(ie, null, artax.TranType, artax.RefNbr, artax.TaxID);
				if (artran != null)
				{
					tranNew.TaxCategoryID = artran.TaxCategoryID;
				}
			}

			ie.Transactions.Update(tranNew);

			//add applications
			ARAdjust adj = new ARAdjust();
			adj.AdjdDocType = doc.AdjdDocType;
			adj.AdjdRefNbr = doc.AdjdRefNbr;
			adj = ie.Adjustments_1.Insert(adj);

			adj.CuryAdjgAmt = doc.InvCuryDocBal;
			ie.Adjustments_1.Update(adj);
		}

		public static bool CalculateDiscountedTaxes(PXCache cache, ARTaxTran artax, decimal cashDiscPercent)
		{
			bool? result = null;
			object value = null;

			IBqlCreator whereTaxable = (IBqlCreator)Activator.CreateInstance(typeof(WhereTaxable<Required<ARTaxTran.taxID>>));
			whereTaxable.Verify(cache, artax, new List<object> { artax.TaxID }, ref result, ref value);
			
			artax.CuryDiscountedTaxableAmt = cashDiscPercent == 0m
				? artax.CuryTaxableAmt
				: PXDBCurrencyAttribute.RoundCury(cache, artax, 
					(decimal)(artax.CuryTaxableAmt * (1 - Decimal.Round(cashDiscPercent, 4))));

			artax.CuryDiscountedPrice = cashDiscPercent == 0m
				? artax.CuryTaxAmt
				: PXDBCurrencyAttribute.RoundCury(cache, artax, 
					(decimal)(artax.TaxRate / 100m * artax.CuryDiscountedTaxableAmt));

			return result == true;
		}
	}
}
