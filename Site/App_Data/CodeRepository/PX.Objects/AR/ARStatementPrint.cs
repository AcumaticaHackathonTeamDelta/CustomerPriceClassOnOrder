using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using System.Globalization;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.CR.Standalone;

namespace PX.Objects.AR
{
	[TableAndChartDashboardType]
	public class ARStatementPrint : PXGraph<ARStatementDetails>
	{
		#region Internal types definition
		[System.SerializableAttribute]
		public partial class PrintParameters : IBqlTable
		{
            #region BranchCD
            public abstract class branchCD : PX.Data.IBqlField
            {
            }
            protected String _BranchCD;
            [PXDefault("")]
            public virtual String BranchCD
            {
                get
                {
                    return this._BranchCD;
                }
                set
                {
                    this._BranchCD = value;
                }
            }
            #endregion

            #region BranchID
            public abstract class branchID : PX.Data.IBqlField
            {
            }
            protected Int32? _BranchID;
            [Branch()]
            public virtual Int32? BranchID
            {
                get
                {
                    return this._BranchID;
                }
                set
                {
                    this._BranchID = value;
                }
            }
            #endregion
            #region Action
			public abstract class action : PX.Data.IBqlField
			{
			}
			protected Int32? _Action;
			[PXDBInt]
			[PXDefault(0)]
			[PXUIField(DisplayName = "Actions")]
			[PXIntList(
                new int[] { Actions.Print, Actions.Email, Actions.MarkDontEmail, Actions.MarkDontPrint, Actions.Regenerate },
				new string[] { Messages.ProcessPrintStatement, Messages.ProcessEmailStatement, Messages.ProcessMarkDontEmail, Messages.ProcessMarkDontPrint, Messages.RegenerateStatement })]
			public virtual Int32? Action
			{
				get
				{
					return this._Action;
				}
				set
				{
					this._Action = value;
				}
			}
			#endregion
			#region StatementCycleId
			public abstract class statementCycleId : PX.Data.IBqlField
			{
			}
			protected String _StatementCycleId;
			[PXDBString(10, IsUnicode = true)]
			[PXDefault(typeof(ARStatementCycle))]
			[PXUIField(DisplayName = "Statement Cycle", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(ARStatementCycle.statementCycleId), DescriptionField = typeof(ARStatementCycle.descr))]
			public virtual String StatementCycleId
			{
				get
				{
					return this._StatementCycleId;
				}
				set
				{
					this._StatementCycleId = value;
				}
			}
			#endregion
			#region StatementDate
			public abstract class statementDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _StatementDate;
			[PXDate]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Statement Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? StatementDate
			{
				get
				{
					return this._StatementDate;
				}
				set
				{
					this._StatementDate = value;
				}
			}
			#endregion
			#region Cury Statements
			public abstract class curyStatements : PX.Data.IBqlField
			{
			}
			protected Boolean? _CuryStatements;
			[PXDBBool]
			[PXDefault(false)]
            [PXUIField(DisplayName = "Foreign Currency Statements")]
			public virtual Boolean? CuryStatements
			{
				get
				{
					return this._CuryStatements;
				}
				set
				{
					this._CuryStatements = value;
				}
			}
			#endregion
			#region ShowAll
			public abstract class showAll : IBqlField
			{
			}
			protected bool? _ShowAll = false;
			[PXDBBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Show All")]
			public virtual bool? ShowAll
			{
				get
				{
					return _ShowAll;
				}
				set
				{
					_ShowAll = value;
				}
			}
			#endregion

            public class Actions
            {
                public const int Print = 0;
                public const int Email = 1;
                public const int MarkDontEmail = 2;
                public const int MarkDontPrint = 3;
                public const int Regenerate = 4;
            }
        }

		[Serializable]        
		public partial class DetailsResult : IBqlTable
		{
			#region Selected
			public abstract class selected : IBqlField
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
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[Customer(DescriptionField = typeof(Customer.acctName), IsKey = true, DisplayName = "Customer")]
			public virtual Int32? CustomerID
			{
				get
				{
					return this._CustomerID;
				}
				set
				{
					this._CustomerID = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, IsKey = true)]
			[PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
			[PXUIField(DisplayName = "Currency")]
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
			#region StatementBalance
			public abstract class statementBalance : PX.Data.IBqlField
			{
			}
            protected Decimal? _StatementBalance;
			[PXDBBaseCury()]
			[PXUIField(DisplayName = "Statement Balance")]
            public virtual Decimal? StatementBalance
			{
				get
				{
					return this._StatementBalance;
				}
				set
				{
					this._StatementBalance = value;
				}
			}
			#endregion
			#region CuryStatementBalance
			public abstract class curyStatementBalance : PX.Data.IBqlField
			{
			}
            protected Decimal? _CuryStatementBalance;
			[PXCury(typeof(DetailsResult.curyID))]
            [PXUIField(DisplayName = "FC Statement Balance")]
            public virtual Decimal? CuryStatementBalance
			{
				get
				{
					return this._CuryStatementBalance;
				}
				set
				{
					this._CuryStatementBalance = value;
				}
			}
			#endregion
			#region UseCurrency
			public abstract class useCurrency : PX.Data.IBqlField
			{
			}
			protected Boolean? _UseCurrency;
			[PXDBBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "FC Statement")]
			public virtual Boolean? UseCurrency
			{
				get
				{
					return this._UseCurrency;
				}
				set
				{
					this._UseCurrency = value;
				}
			}
			#endregion						
			#region AgeBalance00
			public abstract class ageBalance00 : PX.Data.IBqlField
			{
			}
            protected Decimal? _AgeBalance00;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Age00 Balance")]
            public virtual Decimal? AgeBalance00
			{
				get
				{
					return this._AgeBalance00;
				}
				set
				{
					this._AgeBalance00 = value;
				}
			}
			#endregion
			#region CuryAgeBalance00
			public abstract class curyAgeBalance00 : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryAgeBalance00;
			[PXCury(typeof(DetailsResult.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "FC Age00 Balance")]
			public virtual Decimal? CuryAgeBalance00
			{
				get
				{
					return this._CuryAgeBalance00;
				}
				set
				{
					this._CuryAgeBalance00 = value;
				}
			}
			#endregion
			#region OverdueBalance
			public abstract class overdueBalance : PX.Data.IBqlField
			{
			}
			[PXBaseCury()]
			[PXUIField(DisplayName = "Overdue Balance")]
            public virtual Decimal? OverdueBalance
			{
				[PXDependsOnFields(typeof(statementBalance),typeof(ageBalance00))]
				get
				{
					return this.StatementBalance - this.AgeBalance00;
				}
			}
			#endregion
			#region CuryOverdueBalance
			public abstract class curyOverdueBalance : PX.Data.IBqlField
			{
			}
			[PXCury(typeof(DetailsResult.curyID))]
            [PXUIField(DisplayName = "FC Overdue Balance")]
            public virtual Decimal? CuryOverdueBalance
			{
				[PXDependsOnFields(typeof(curyStatementBalance),typeof(curyAgeBalance00))]
				get
				{
					return (this._CuryStatementBalance) - (this.CuryAgeBalance00 ?? Decimal.Zero);
				}
			}
			#endregion
			#region DontPrint
			public abstract class dontPrint : PX.Data.IBqlField
			{
			}
			protected Boolean? _DontPrint;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Don't Print")]
			public virtual Boolean? DontPrint
			{
				get
				{
					return this._DontPrint;
				}
				set
				{
					this._DontPrint = value;
				}
			}
			#endregion
			#region Printed
			public abstract class printed : PX.Data.IBqlField
			{
			}
			protected Boolean? _Printed;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Printed")]
			public virtual Boolean? Printed
			{
				get
				{
					return this._Printed;
				}
				set
				{
					this._Printed = value;
				}
			}
			#endregion
			#region DontEmail
			public abstract class dontEmail : PX.Data.IBqlField
			{
			}
			protected Boolean? _DontEmail;
			[PXDBBool()]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Don't Email")]
			public virtual Boolean? DontEmail
			{
				get
				{
					return this._DontEmail;
				}
				set
				{
					this._DontEmail = value;
				}
			}
			#endregion
			#region Emailed
			public abstract class emailed : PX.Data.IBqlField
			{
			}
			protected Boolean? _Emailed;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Emailed")]
			public virtual Boolean? Emailed
			{
				get
				{
					return this._Emailed;
				}
				set
				{
					this._Emailed = value;
				}
			}
			#endregion
            #region BranchID
            public abstract class branchID : PX.Data.IBqlField
            {
            }
            protected Int32? _BranchID;
            [PXDBInt(IsKey = true)]
            [PXDefault()]
            [Branch()]
            [PXUIField(DisplayName = "Branch", Visible=false)]
            public virtual Int32? BranchID
            {
                get
                {
                    return this._BranchID;
                }
                set
                {
                    this._BranchID = value;
                }
            }
            #endregion

			public virtual void Copy(ARStatement aSrc, Customer cust)
			{
				this.CustomerID = cust.BAccountID;
				this.UseCurrency = cust.PrintCuryStatements;
				this.StatementBalance = aSrc.EndBalance ?? decimal.Zero;
				this.AgeBalance00 = aSrc.AgeBalance00 ?? decimal.Zero;
				this.CuryID = aSrc.CuryID;
				this.CuryStatementBalance = aSrc.CuryEndBalance ?? decimal.Zero;
				this.CuryAgeBalance00 = aSrc.CuryAgeBalance00 ?? decimal.Zero;
				this.DontEmail = aSrc.DontEmail;
				this.DontPrint = aSrc.DontPrint;
				this.Emailed = aSrc.Emailed;
				this.Printed = aSrc.Printed;
                this.BranchID = aSrc.BranchID;
			}
			public virtual void Append(DetailsResult aSrc)
			{
				this._StatementBalance += aSrc.StatementBalance;
				this._AgeBalance00 += aSrc.AgeBalance00;
				if (this.CuryID == aSrc.CuryID)
				{
					this._CuryStatementBalance += aSrc.CuryStatementBalance;
					this._CuryAgeBalance00 += aSrc.CuryAgeBalance00;
				}
				else
				{
					this._CuryStatementBalance = Decimal.Zero;
					this._CuryAgeBalance00 = Decimal.Zero;
				}				
			}
			public virtual void ResetToBaseCury(string aBaseCuryID) 
			{
				this._CuryID = aBaseCuryID; 
				this._CuryStatementBalance = this._StatementBalance;
				this._CuryAgeBalance00 = this._AgeBalance00;
			}

		}

		[Serializable]
		public class ContactR : Contact
		{
			public new abstract class contactID : IBqlField { }
			public new abstract class bAccountID : IBqlField { }
		}
		#endregion


		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Customer", IsReadOnly = true)]
		[PXSelector(typeof(Search<Customer.bAccountID, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>>), SubstituteKey = typeof(Customer.acctCD), DescriptionField = typeof(Customer.acctCD), ValidateValue = false)]
		public virtual void ARStatement_CustomerID_CacheAttached() { }

		#region Ctor
		public ARStatementPrint()
		{
			ARSetup setup = ARSetup.Current;
            Details.Cache.AllowDelete = false;
			Details.Cache.AllowInsert = false;			
			Details.SetSelected<ARInvoice.selected>();
			Details.SetProcessCaption(IN.Messages.Process);
			Details.SetProcessAllCaption(IN.Messages.ProcessAll);

            InquiriesFolder.MenuAutoOpen = true;
            InquiriesFolder.AddMenuAction(ViewDetails);
		}
		#endregion

		#region Public Members
        public PXCancel<PrintParameters> Cancel;
        public PXAction<PrintParameters> prevStatementDate;
        public PXAction<PrintParameters> nextStatementDate;


		public PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Current<ARStatement.customerID>>>> dummyBaccountView;
		public PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>> dummyCustomerView;
		public PXSelect<Vendor> dummyVendorView;
		public PXSelect<EPEmployee> dummyEmployeeView;

		[PXViewName(Messages.Statement)]
		public PXSelect<ARStatement> Statement;
		[PXViewName(Messages.Customer)]
		public PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>> Customer; 

		public PXFilter<PrintParameters> Filter;


		[PXFilterable]
		public PXFilteredProcessing<DetailsResult, PrintParameters> Details;

		public PXSetup<ARSetup> ARSetup;		
        public PXAction<PrintParameters> InquiriesFolder;
        public PXAction<PrintParameters> ViewDetails;

		[PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable PrevStatementDate(PXAdapter adapter)
		{
			PrintParameters filter = this.Filter.Current;

			if (filter != null && !string.IsNullOrEmpty(filter.StatementCycleId))
			{
				ARStatement statement = PXSelect<ARStatement, Where<ARStatement.statementCycleId, Equal<Required<ARStatement.statementCycleId>>,
																					And<
												Where<ARStatement.statementDate, Less<Required<ARStatement.statementDate>>,
														Or<Required<ARStatement.statementDate>, IsNull>>>>,
																					OrderBy<Desc<ARStatement.statementDate>>>.Select(this, filter.StatementCycleId, filter.StatementDate, filter.StatementDate);

				if (statement != null)
				{
					filter.StatementDate = statement.StatementDate;
				}
			}
			Details.Cache.Clear();
			return adapter.Get();
		}

		[PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable NextStatementDate(PXAdapter adapter)
		{

			PrintParameters filter = this.Filter.Current;

			if (filter != null && !string.IsNullOrEmpty(filter.StatementCycleId))
			{
				ARStatement statement = PXSelect<ARStatement, Where<ARStatement.statementCycleId, Equal<Required<ARStatement.statementCycleId>>,
												And<
												Where<ARStatement.statementDate, Greater<Required<ARStatement.statementDate>>,
														Or<Required<ARStatement.statementDate>, IsNull>>>>,
												OrderBy<Asc<ARStatement.statementDate>>>.Select(this, filter.StatementCycleId, filter.StatementDate, filter.StatementDate);

				if (statement != null)
				{
					filter.StatementDate = statement.StatementDate;
				}
			}
			Details.Cache.Clear();
			return adapter.Get();
		}

		[PXViewName(Messages.BillingContactView)]
		public PXSelectJoin<
			ContactR,
				InnerJoin<Customer,
				On<ContactR.contactID, Equal<Customer.defBillContactID>>>,
			Where<
				Customer.bAccountID, Equal<Current<ARStatement.customerID>>>>
			CustomerDefaultBillingContact;

		#region Delegates
		protected virtual IEnumerable details()
		{
			ARSetup setup = ARSetup.Current;

			PrintParameters header = Filter.Current;
			List<DetailsResult> result = new List<DetailsResult>();
			if (header == null)
				yield break;

			GL.Company company = PXSelect<GL.Company>.Select(this);
			foreach (PXResult<ARStatement, Customer> it in PXSelectJoin<ARStatement,
					InnerJoin<Customer, On<Customer.bAccountID, Equal<ARStatement.statementCustomerID>>>,
					Where<ARStatement.statementDate, Equal<Required<ARStatement.statementDate>>,
						And<ARStatement.statementCycleId, Equal<Required<ARStatement.statementCycleId>>>>,
					OrderBy<Asc<ARStatement.statementCustomerID, Asc<ARStatement.curyID>>>>
					.Select(this, header.StatementDate, header.StatementCycleId))
			{
				DetailsResult res = new DetailsResult();
				ARStatement st = it;
				Customer cust = it;
				res.Copy(st, cust);

				if (setup.ConsolidatedStatement != true && st.BranchID != header.BranchID)
						continue;

				if (Filter.Current.Action == 0 &&
					header.ShowAll != true &&
					(st.DontPrint == true || st.Printed == true))
						continue;

				if ((Filter.Current.Action == 1 || Filter.Current.Action == 2) &&
					header.ShowAll != true &&
					(st.DontEmail == true || st.Emailed == true))
						continue;

				if (cust.PrintCuryStatements == true)
				{
					if (Filter.Current.CuryStatements != true)
						continue;

					DetailsResult last = result.Count > 0 ? result[result.Count - 1] : null;
					if (last?.CustomerID == res.CustomerID && last?.CuryID == res.CuryID)
					{
						last.Append(res);
					}
					else
					{
						result.Add(res);
					}
				}
				else
				{
					if (Filter.Current.CuryStatements == true)
						continue;

					res.ResetToBaseCury(company.BaseCuryID);

					DetailsResult last = result.Count > 0 ? result[result.Count - 1] : null;
					if (last?.CustomerID == res.CustomerID)
					{
						last.Append(res);
					}
					else
					{
						result.Add(res);
					}
				}
			}

			foreach (var item in result)
			{
				var located = Details.Cache.Locate(item);
				if (located != null)
				{
					yield return located;
				}
				else
				{
					Details.Cache.SetStatus(item, PXEntryStatus.Held);
					yield return item;
				}
			}

			Details.Cache.IsDirty = false;
		}

		public PXSelectJoin<Contact, InnerJoin<Customer, On<Contact.contactID, Equal<Customer.defBillContactID>>>, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>> contact;

        [PXViewName(CR.Messages.MainContact)]
        public PXSelect<Contact> DefaultCompanyContact;
        protected virtual IEnumerable defaultCompanyContact()
        {
            List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
            foreach (PXResult<Branch, BAccountR, Contact> res in PXSelectJoin<Branch,
                                        LeftJoin<BAccountR, On<Branch.bAccountID, Equal<BAccountR.bAccountID>>,
                                        LeftJoin<Contact, On<BAccountR.defContactID, Equal<Contact.contactID>>>>,
                                        Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, branches != null ? (int?)branches[0] : null))
            {
                yield return (Contact)res;
                break;
            }
        }
		#endregion
        #endregion

        public override bool IsDirty
        {
            get
            {
                return false;
            }
        }		

		

		#region Filter Events
		protected virtual void PrintParameters_Action_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SetStatementDateFromCycle((PrintParameters)e.Row);
		}

		protected virtual void PrintParameters_StatementCycleId_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SetStatementDateFromCycle((PrintParameters)e.Row);
		}

		private void SetStatementDateFromCycle(PrintParameters filter)
		{
			if (!string.IsNullOrEmpty(filter.StatementCycleId))
			{
				ARStatementCycle cycle = PXSelect<ARStatementCycle,
							Where<ARStatementCycle.statementCycleId,
							Equal<Required<ARStatementCycle.statementCycleId>>>>.Select(this, filter.StatementCycleId);
				filter.StatementDate = cycle.LastStmtDate;
			}
		}
		
		protected virtual void PrintParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            ARSetup setup = ARSetup.Current;
            bool bb=!(setup.ConsolidatedStatement??false);

            PrintParameters row = (PrintParameters)e.Row;
            if (row != null)
			{
                row.BranchCD = null;
				if (!bb) 
					row.BranchID = null; //Force null for the non-consolidated statements
                if (row.BranchID != null)
                {
                    Branch BrS = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, row.BranchID);
                    if (BrS != null)
                        row.BranchCD = BrS.BranchCD;
                }

                PXUIFieldAttribute.SetVisible<PrintParameters.branchID>(sender, null, bb);
				PXUIFieldAttribute.SetEnabled<PrintParameters.statementDate>(sender, row, row.Action != PrintParameters.Actions.Regenerate);

                PrintParameters filter = (PrintParameters)this.Filter.Cache.CreateCopy(row);
                switch (row.Action)
				{
					case 0:
						Details.SetProcessDelegate(list => PrintStatements(filter, list, false));
						break;
					case 1:
						Details.SetProcessDelegate(list => EmailStatements(filter, list, false));
						break;
					case 2:
						Details.SetProcessDelegate(list => EmailStatements(filter, list, true));
						break;
					case 3:
						Details.SetProcessDelegate(list => PrintStatements(filter, list, true));
						break;
                    case 4:
                        Details.SetProcessDelegate(list => RegenerateStatements(filter, list));
                        break;
				}
			}
		}

		protected virtual void PrintParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			Details.Cache.Clear();
		}

		protected virtual void DetailsResult_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion

		#region Sub-screen Navigation Button
        [PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable inquiriesFolder(PXAdapter adapter)
        {
            return adapter.Get();
        }

		[PXUIField(DisplayName = Messages.CustomerStatementHistory, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewDetails(PXAdapter adapter)
		{
			if (this.Details.Current != null && this.Filter.Current != null)
			{
				DetailsResult res = this.Details.Current;
				ARStatementForCustomer graph = PXGraph.CreateInstance<ARStatementForCustomer>();

				ARStatementForCustomer.ARStatementForCustomerParameters filter = graph.Filter.Current;
				filter.CustomerID = res.CustomerID;
				graph.Filter.Update(filter);
				graph.Filter.Select();
                throw new PXRedirectRequiredException(graph, Messages.CustomerStatementHistory);
			}
			return Filter.Select();
		}
		#endregion

		#region Utility Functions

		protected static void Export(Dictionary<string, string> aRes, PrintParameters aSrc)
		{
			aRes[ARStatementReportParams.Fields.StatementCycleID] = aSrc.StatementCycleId;
			aRes[ARStatementReportParams.Fields.StatementDate]    = aSrc.StatementDate.Value.ToString("d", CultureInfo.InvariantCulture);
		}
		protected static void Export(Dictionary<string, string> aRes, int index, PrintParameters aSrc)
		{
      aRes[ARStatementReportParams.Fields.StatementCycleID + index] = aSrc.StatementCycleId;
			aRes[ARStatementReportParams.Fields.StatementDate + index] = aSrc.StatementDate.Value.ToString("d", CultureInfo.InvariantCulture);
		}
		#endregion

		#region Process Delegates
		public static void PrintStatements(PrintParameters filter,  List<DetailsResult> list, bool markOnly)
		{			

			PXGraph graph = new PXGraph();
			PXView docview = 
				filter.CuryStatements == true ?
				new PXView(graph, false, BqlCommand.CreateInstance(typeof(Select<ARStatement,
													Where<ARStatement.statementCycleId, Equal<Required<ARStatement.statementCycleId>>,
														And<ARStatement.statementDate, Equal<Required<ARStatement.statementDate>>,
														And<ARStatement.statementCustomerID, Equal<Required<ARStatement.customerID>>,
														And<ARStatement.curyID, Equal<Required<ARStatement.curyID>>>>>>>)))			
			  :
				new PXView(graph, false, BqlCommand.CreateInstance(typeof(Select<ARStatement,
													Where<ARStatement.statementCycleId, Equal<Required<ARStatement.statementCycleId>>,
														And<ARStatement.statementDate, Equal<Required<ARStatement.statementDate>>,
														And<ARStatement.statementCustomerID, Equal<Required<ARStatement.customerID>>>>>>)));			
			graph.Views["_ARStatement_"] = docview;
			graph.Views.Caches.Add(typeof(ARStatement));
			PXReportRequiredException ex = null;
			string reportID = (filter.CuryStatements ?? false) ?
					ARStatementReportParams.CS_CuryStatementReportID :
					ARStatementReportParams.CS_StatementReportID;

			foreach (DetailsResult t in list)
			{
				if (markOnly)
				{
					if (filter.ShowAll != true)
						foreach (ARStatement doc in docview.SelectMulti(filter.StatementCycleId, filter.StatementDate, t.CustomerID, t.CuryID))
						{
							doc.DontPrint = true;
							docview.Cache.Update(doc);
						}
				}
				else
				{

					Dictionary<string, string> d = new Dictionary<string, string>();
					
					d[ARStatementReportParams.Parameters.BranchID] = filter.BranchCD;					
					d[ARStatementReportParams.Fields.StatementCycleID] = filter.StatementCycleId;
					d[ARStatementReportParams.Fields.StatementDate]    = filter.StatementDate.Value.ToString("d", CultureInfo.InvariantCulture);
					d[ARStatementReportParams.Fields.CustomerID] = t.CustomerID.ToString();

					if (filter.ShowAll == true)
					{
						d[ARStatementReportParams.Parameters.IgnorePrintFlags] = ARStatementReportParams.BoolValues.True;
					}
					else
					{
						d[ARStatementReportParams.Fields.PrintStatements] = ARStatementReportParams.BoolValues.True;
					}
					if (filter.CuryStatements ?? false)
						d[ARStatementReportParams.Fields.CuryID] = t.CuryID;

					foreach (ARStatement doc in docview.SelectMulti(filter.StatementCycleId, filter.StatementDate, t.CustomerID, t.CuryID))
					{
						if (doc.Printed != true)
						{
							doc.Printed = true;
							docview.Cache.Update(doc);
						}
					}

					ex = PXReportRequiredException.CombineReport(ex,  GetCustomerReportID(graph, reportID, t), d);
				}
			}

			graph.Actions.PressSave();
			if(ex != null) throw ex;						
		}

		private static string GetCustomerReportID(PXGraph graph, string reportID, DetailsResult statement)
		{
            return GetCustomerReportID(graph, reportID, statement.CustomerID, statement.BranchID);
		}

        public static string GetCustomerReportID(PXGraph graph, string reportID, int? customerID, int? branchID)
        {
            Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.SelectWindowed(graph, 0, 1, customerID);
            return new NotificationUtility(graph).SearchReport(ARNotificationSource.Customer, customer, reportID, branchID);
        }

        public static void EmailStatements(PrintParameters filter, List<DetailsResult> list, bool markOnly) 
        {
            ARStatementUpdate graph = CreateInstance<ARStatementUpdate>();
            int i = 0;
            bool failed = false;
            foreach (DetailsResult it in list) 
            {                
                try
                {
                    graph.EMailStatement(filter.BranchCD, it.CustomerID, filter.StatementDate, filter.CuryStatements == true? it.CuryID: null, markOnly, filter.ShowAll == true);
					if (!markOnly)
					{
						it.Emailed = true;
					}

                    PXFilteredProcessing<DetailsResult, PrintParameters>.SetCurrentItem(it);
                    PXFilteredProcessing<DetailsResult, PrintParameters>.SetProcessed();
                }
                catch (Exception e) 
                {
                    PXFilteredProcessing<DetailsResult, PrintParameters>.SetError(i, e);
                    failed = true;
                }
                i++;
            }
            if(failed)
                throw new PXException(ErrorMessages.MailSendFailed);            
        }

		private static void RegenerateStatements(PrintParameters pars, List<DetailsResult> statements)
		{
			var process = PXGraph.CreateInstance<StatementCycleProcessBO>();
			var cycle = process.CyclesList.SelectSingle(pars.StatementCycleId);
			var customerSelect = new PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<DetailsResult.customerID>>>>(process);
			
			int i = 0;
			List<Customer> customers = new List<Customer>();
			
			foreach (DetailsResult res in statements)
			{
				if (ARStatementProcess.CheckForUnprocessedPPD(process, pars.StatementCycleId, pars.StatementDate, res.CustomerID))
				{
					PXFilteredProcessing<DetailsResult, PrintParameters>.SetError(i, 
						new PXSetPropertyException(Messages.UnprocessedPPDExists, PXErrorLevel.RowError));
				}
				else
				{
					Customer customer = customerSelect.SelectSingle(res.CustomerID);
					if (customer != null)
					{
						customers.Add(customer);
					}
				}
				i++;
			}

			if (cycle == null || customers.Any() == false) return;
			StatementCycleProcessBO.RegenerateStatements(process, cycle, customers);
		}

		#endregion

	}

    public class ARStatementUpdate : PXGraph<ARStatementUpdate,Customer> 
    {
        public PXSelect<Customer, Where<Customer.bAccountID, Equal<Optional<ARStatement.customerID>>>> Customer;
        [PXViewName(Messages.ARContact)]
        public PXSelectJoin<Contact, InnerJoin<Customer, On<Contact.contactID, Equal<Customer.defBillContactID>>>, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>> contact;
        public PXSelect<ARStatement, Where<ARStatement.statementCustomerID, Equal<Optional<Customer.bAccountID>>,
							    And<ARStatement.statementDate, Equal<Required<ARStatement.statementDate>>,														
                                And<Where<ARStatement.curyID, Equal<Required<ARStatement.curyID>>,
                                        Or<Required<ARStatement.curyID>,IsNull>>>>>> StatementMC;

        public PXSelect<ARStatement, Where<ARStatement.customerID, Equal<Required<ARStatement.customerID>>,
                                And<ARStatement.statementDate, Equal<Required<ARStatement.statementDate>>,
                                And<ARStatement.curyID, Equal<Required<ARStatement.curyID>>>>>> Statement;


        [CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARStatement.customerID>>>>))]
		[CRDefaultMailTo(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<Customer.defBillContactID>>>>))]
        public CRActivityList<ARStatement>
            Activity;

        public virtual void EMailStatement(string BranchCD, int? aCustomerID, DateTime? aStatementDate, string aCurrency, bool aMarkOnly, bool ShowAll) 
        {
            Customer customer = this.Customer.Search<Customer.bAccountID>(aCustomerID, aCustomerID);
			Customer.Current = customer;
			bool useCurrency = string.IsNullOrEmpty(aCurrency) == false;
            string notificationCD = (useCurrency? "STATEMENTMC":"STATEMENT");
            bool sent = false;
            
			foreach (ARStatement iDoc in StatementMC.Select(aCustomerID, aStatementDate, aCurrency, aCurrency))
            {
                if (aMarkOnly)
                {
                    iDoc.DontEmail = true;
                    StatementMC.Cache.Update(iDoc);
                }
				else 
				{
					if (iDoc.StatementCustomerID == iDoc.CustomerID)
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();

                    parameters[ARStatementReportParams.Parameters.BranchID] = BranchCD;

                    if (ShowAll)
					{
						parameters[ARStatementReportParams.Parameters.IgnorePrintFlags] = ARStatementReportParams.BoolValues.True;						
					}
					else
					{
						parameters[ARStatementReportParams.Fields.SendStatementsByEmail] = ARStatementReportParams.BoolValues.True;
					}
					if (useCurrency)
						{
						parameters[ARStatementReportParams.Fields.CuryID] = iDoc.CuryID;
						}

						StatementMC.Current = iDoc;
                    if (useCurrency || !sent)
                    {
                        Export(parameters, iDoc);
						Activity.SendNotification(ARNotificationSource.Customer, notificationCD, iDoc.BranchID, parameters);
                        sent = true;
                    }
					}

                    iDoc.Emailed = true;
                    StatementMC.Cache.Update(iDoc);
                }
            }
			Save.Press();
        }

        protected static void Export(Dictionary<string, string> aRes, ARStatement aSrc)
        {
            aRes[ARStatementReportParams.Fields.StatementCycleID] = aSrc.StatementCycleId;
            aRes[ARStatementReportParams.Fields.StatementDate] = aSrc.StatementDate.Value.ToString("d", CultureInfo.InvariantCulture);
            aRes[ARStatementReportParams.Fields.CustomerID] = aSrc.CustomerID.ToString(); 
        }
    }

	public static class ARStatementReportParams
	{
		public static class Fields
		{
            public const string BranchID = "ARStatement.BranchID";
            public const string StatementCycleID = "ARStatement.StatementCycleId";
			public const string StatementDate = "ARStatement.StatementDate";
			public const string CustomerID = "ARStatement.StatementCustomerID";
			public const string CuryID = "ARStatement.CuryID";
			public const string SendStatementsByEmail = "Customer.SendStatementByEmail";
			public const string PrintStatements = "Customer.PrintStatements";			
		}

		public static class Parameters
		{
            public const string BranchID = "BranchID";
            public const string StatementCycleID = "StatementCycleId";
			public const string StatementDate = "StatementDate";
			public const string CustomerID = "CustomerId";
			public const string SendByEmail = "SendByEmail";
			public const string PrintOnPaper = "PrintOnPaper";
			public const string IgnorePrintFlags = "IgnorePrintFlags";
		}

		public static class BoolValues
		{
			public const string True = "true";
			public const string False = "false";
		}

		public const string CS_StatementReportID = "AR641500";
		public const string CS_CuryStatementReportID = "AR642000";

		public static void Export(Dictionary<string, string> aRes, Customer aAcct)
		{
			aRes[ARStatementReportParams.Fields.CustomerID] = aAcct.AcctCD;
			if (aAcct.PrintStatements == true)
				aRes[ARStatementReportParams.Fields.PrintStatements] = aAcct.PrintStatements.Value ? ARStatementReportParams.BoolValues.True : ARStatementReportParams.BoolValues.False;
		}

		public static void Export(Dictionary<string, string> aRes, int index, Customer aAcct)
		{
			
		}

        public static Dictionary<string, string> FromCustomer(Customer customer)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters[ARStatementReportParams.Parameters.CustomerID] = customer.AcctCD;
            parameters[ARStatementReportParams.Parameters.StatementCycleID] = customer.StatementCycleId;
			
            parameters[ARStatementReportParams.Parameters.PrintOnPaper] =
                customer.PrintStatements == true ?
                ARStatementReportParams.BoolValues.True :
                ARStatementReportParams.BoolValues.False;
            parameters[ARStatementReportParams.Parameters.SendByEmail] =
                customer.SendStatementByEmail == true ?
                ARStatementReportParams.BoolValues.True :
                ARStatementReportParams.BoolValues.False;

            return parameters;
        }

        public static string ReportIDForCustomer(PXGraph graph, Customer customer, int? branchID)
        {
            var reportID = customer.PrintCuryStatements == true ? ARStatementReportParams.CS_CuryStatementReportID : ARStatementReportParams.CS_StatementReportID;
            return ARStatementPrint.GetCustomerReportID(graph, reportID, customer.BAccountID, branchID);
		}
	}

}
