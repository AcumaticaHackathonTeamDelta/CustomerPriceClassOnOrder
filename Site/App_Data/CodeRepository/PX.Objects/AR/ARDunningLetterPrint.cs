using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.GL;
using System;
using System.Collections;
using System.Collections.Generic;
using Branch = PX.Objects.GL.Branch;


namespace PX.Objects.AR
{
    public class ARDunningLetterPrint : PXGraph<ARDunningLetterPrint>
    {
        #region Internal types definitions
        [System.SerializableAttribute]
        public partial class PrintParameters : IBqlTable
        {
            #region Action
            public abstract class action : PX.Data.IBqlField
            {
            }
            protected Int32? _Action;
            [PXDBInt]
            [PXDefault(4)]
            [PXUIField(DisplayName = "Action")]
            [PXIntList(new int[] { 0, 1, 2, 3, 4 },
                new string[] { Messages.ProcessPrintDL, Messages.ProcessEmailDL, Messages.ProcessMarkDontEmail, Messages.ProcessMarkDontPrint, Messages.ProcessReleaseDunningLetter })]
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

            #region BeginDate
            public abstract class beginDate : PX.Data.IBqlField
            {
            }
            protected DateTime? _BeginDate;
            [PXDate]
            [PXDefault(typeof(AccessInfo.businessDate))]
            [PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.Visible)]
            public virtual DateTime? BeginDate
            {
                get
                {
                    return this._BeginDate;
                }
                set
                {
                    this._BeginDate = value;
                }
            }
            #endregion
            #region EndDate
            public abstract class endDate : PX.Data.IBqlField
            {
            }
            protected DateTime? _EndDate;
            [PXDate]
            [PXDefault(typeof(AccessInfo.businessDate))]
            [PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.Visible)]
            public virtual DateTime? EndDate
            {
                get
                {
                    return this._EndDate;
                }
                set
                {
                    this._EndDate = value;
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
            [PXUIField(DisplayName = "Show All", Enabled = false)]
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

            #region DunningLetterID
            public abstract class dunningLetterID : PX.Data.IBqlField
            {
            }
            protected Int32? _DunningLetterID;
            [PXDBInt(IsKey = true)]
            [PXUIField(Enabled = false)]
            public virtual Int32? DunningLetterID
            {
                get
                {
                    return this._DunningLetterID;
                }
                set
                {
                    this._DunningLetterID = value;
                }
            }
            #endregion

            #region BranchID
            public abstract class branchID : PX.Data.IBqlField
            {
            }
            protected Int32? _BranchID;
            [PXDefault()]
            [Branch(DescriptionField = typeof(PX.Objects.GL.Branch.branchID))]
            [PXUIField(DisplayName = "Branch")]
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
            #region CustomerId
            public abstract class customerId : PX.Data.IBqlField
            {
            }
            protected Int32? _CustomerId;
            [PXUIField(DisplayName = "Customer", Visibility = PXUIVisibility.Visible)]
            [Customer(DescriptionField = typeof(Customer.acctName))]
            public virtual Int32? CustomerId
            {
                get
                {
                    return this._CustomerId;
                }
                set
                {
                    this._CustomerId = value;
                }
            }
            #endregion

            #region DunningLetterDate
            public abstract class dunningLetterDate : PX.Data.IBqlField
            {
            }
            protected DateTime? _DunningLetterDate;
            [PXDBDate()]
            [PXDefault(TypeCode.DateTime, "01/01/1900")]
            [PXUIField(DisplayName = "Dunning Letter Date")]
            public virtual DateTime? DunningLetterDate
            {
                get
                {
                    return this._DunningLetterDate;
                }
                set
                {
                    this._DunningLetterDate = value;
                }
            }
            #endregion

            #region DunningLetterLevel
            public abstract class dunningLetterLevel : PX.Data.IBqlField
            {
            }
            protected Int32? _DunningLetterLevel;
            [PXDBInt()]
            [PXDefault()]
            [PXUIField(DisplayName = Messages.DunningLetterLevel)]
            public virtual Int32? DunningLetterLevel
            {
                get
                {
                    return this._DunningLetterLevel;
                }
                set
                {
                    this._DunningLetterLevel = value;
                }
            }
            #endregion

            #region DocBal
            public abstract class docBal : PX.Data.IBqlField
            {
            }
            protected Decimal? _DocBal;
            [PXDBBaseCury()]
            [PXUIField(DisplayName = "Overdue Balance")]
            public virtual Decimal? DocBal
            {
                get
                {
                    return this._DocBal;
                }
                set
                {
                    this._DocBal = value;
                }
            }
            #endregion
            #region LastLevel
            public abstract class lastLevel : PX.Data.IBqlField
            {
            }
            protected Boolean? _LastLevel;
            [PXDBBool()]
            [PXUIField(DisplayName = "Final Reminder")]
            public virtual Boolean? LastLevel
            {
                get
                {
                    return this._LastLevel;
                }
                set
                {
                    this._LastLevel = value;
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


            /// <summary>
            /// Copy field values from selection and aggregate details
            /// </summary>
            /// <param name="G">Graph</param>
            /// <param name="aSrc">Selected DunningLetter</param>
            /// <param name="cust">Selected Customer</param>
            public virtual void Copy(PXGraph G, ARDunningLetter aSrc, Customer cust)
            {
                this.BranchID = aSrc.BranchID;
                this.CustomerId = aSrc.BAccountID;
                this.DunningLetterID = aSrc.DunningLetterID;
                this.DunningLetterDate = aSrc.DunningLetterDate;
                this.DunningLetterLevel = aSrc.DunningLetterLevel;
                this.LastLevel = aSrc.LastLevel;
                this.DontEmail = aSrc.DontEmail;
                this.DontPrint = aSrc.DontPrint;
                this.Emailed = aSrc.Emailed;
                this.Printed = aSrc.Printed;
                this.DocBal = 0;

                foreach (PXResult<ARDunningLetterDetail> it in PXSelect<ARDunningLetterDetail,
                    Where<ARDunningLetterDetail.dunningLetterID, Equal<Required<ARDunningLetterDetail.dunningLetterID>>>>
                    .Select(G, aSrc.DunningLetterID))
                {
                    ARDunningLetterDetail d = it;
                    this.DocBal += d.OverdueBal;
                }
            }

        }
        #endregion

        #region Public members
        public ARDunningLetterPrint()
        {
            try
            {
                ARSetup setup = ARSetup.Current;
                Consolidated = setup.ConsolidatedDunningLetter.GetValueOrDefault(false);
                if (setup.AutoReleaseDunningLetter == true)
                {
                    PXDefaultAttribute.SetDefault<PrintParameters.action>(Filter.Cache, 0);
                    PXUIFieldAttribute.SetEnabled<PrintParameters.showAll>(Filter.Cache, null, true);
                }
                Details.Cache.AllowDelete = false;
                Details.Cache.AllowInsert = false;
                //Details.Cache.AllowUpdate = false;
                Details.SetSelected<ARInvoice.selected>();  //???
                Details.SetProcessCaption(IN.Messages.Process);
                Details.SetProcessAllCaption(IN.Messages.ProcessAll);
            }
            catch { }
        }

        public bool Consolidated = false;

        public PXCancel<PrintParameters> Cancel;

        public PXFilter<PrintParameters> Filter;

        [PXFilterable]
        public PXFilteredProcessing<DetailsResult, PrintParameters> Details;

        public PXSetup<ARSetup> ARSetup;

        #endregion
        #region Actions
        public PXAction<PrintParameters> ViewDocument;
        [PXUIField(DisplayName = Messages.ViewDunningLetter, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable viewDocument(PXAdapter adapter)
        {
            if (Details.Current != null)
            {
                var doc = PXSelect<ARDunningLetter, Where<ARDunningLetter.dunningLetterID, Equal<Required<DetailsResult.dunningLetterID>>>>.Select(this, Details.Current.DunningLetterID);
                if (doc != null)
                {
                    ARDunningLetterMaint graph = PXGraph.CreateInstance<ARDunningLetterMaint>();
                    graph.Document.Current = doc;
                    PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
                }
            }
            return adapter.Get();
        }
        #endregion
        #region Delegates

        /// <summary>
        /// Generates a list of documents that meet the filter criteria.
        /// This list is used for display in the processing screen
        /// </summary>
        /// <returns>List of Dunning Letters</returns>
        protected virtual IEnumerable details()
        {
            Details.Cache.Clear();
            PrintParameters header = Filter.Current;
            List<DetailsResult> result = new List<DetailsResult>();
            if (header == null)
                yield break;

            GL.Company company = PXSelect<GL.Company>.Select(this);
            PXSelectBase<ARDunningLetter> cmd = new PXSelectJoin<ARDunningLetter, InnerJoin<Customer, On<Customer.bAccountID, Equal<ARDunningLetter.bAccountID>>>,
                    Where<ARDunningLetter.dunningLetterDate,
                    Between<Required<ARDunningLetter.dunningLetterDate>, Required<ARDunningLetter.dunningLetterDate>>,
                        And<ARDunningLetter.consolidated, Equal<Required<ARDunningLetter.consolidated>>>>,
                    OrderBy<Asc<ARDunningLetter.bAccountID>>>(this);
            if (Filter.Current.Action == 4)
            {
                cmd.WhereAnd<Where<ARDunningLetter.released, Equal<False>, And<ARDunningLetter.voided, Equal<False>>>>();
            }
            else
            {
                cmd.WhereAnd<Where<ARDunningLetter.released, Equal<True>, And<ARDunningLetter.voided, Equal<False>>>>();
            }
            foreach (PXResult<ARDunningLetter, Customer> it in cmd.Select(header.BeginDate, header.EndDate, this.Consolidated))
            {
                DetailsResult res = new DetailsResult();
                ARDunningLetter dl = it;
                Customer cust = it;
                res.Copy(this, dl, cust);

                if (this.Filter.Current.Action == 0)
                    if (header.ShowAll == false && (dl.DontPrint == true || dl.Printed == true))
                        continue;

                if (this.Filter.Current.Action == 1)
                    if (header.ShowAll == false && (dl.DontEmail == true || dl.Emailed == true))
                        continue;

                if (this.Filter.Current.Action == 2)
                    if (header.ShowAll == false && (dl.DontEmail == true || dl.Emailed == true))
                        continue;

                result.Add(res);
            }
            foreach (var item in result)
            {
                Details.Cache.SetStatus(item, PXEntryStatus.Held);
                yield return item;
            }
            Details.Cache.IsDirty = false;
        }

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

        [PXViewName(Messages.ARContact)]
        public PXSelectJoin<Contact, InnerJoin<Customer, On<Contact.contactID, Equal<Customer.defBillContactID>>>, Where<Customer.bAccountID, Equal<Current<ARDunningLetter.bAccountID>>>> contact;

        #endregion
        #region Filter Events

        protected virtual void PrintParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PrintParameters o = (PrintParameters)e.Row;
            if (o != null)
            {
                PrintParameters filter = (PrintParameters)this.Filter.Cache.CreateCopy(o);
                switch (o.Action)
                {
                    case 0:
                        Details.SetProcessDelegate(list => Print(filter, list, false));
                        break;
                    case 1:
                        Details.SetProcessDelegate(list => Email(filter, list, false));
                        break;
                    case 2:
                        Details.SetProcessDelegate(list => Email(filter, list, true));
                        break;
                    case 3:
                        Details.SetProcessDelegate(list => Print(filter, list, true));
                        break;
                    case 4:
                        Details.SetProcessDelegate(list => Release(filter, list));
                        break;
                }
            }
        }

        protected virtual void PrintParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            PrintParameters row = (PrintParameters)e.Row;
            if (row != null)
            {
                if (row.Action == 4)
                {
                    row.ShowAll = false;
                }
                PXUIFieldAttribute.SetEnabled<PrintParameters.showAll>(sender, row, row.Action != 4);
            }
            Details.Cache.Clear();
        }

        protected virtual void DetailsResult_RowPersisting(PXCache sedner, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion
        #region Process Delegates

        public static void Print(PrintParameters filter, List<DetailsResult> list, bool markOnly)
        {
			bool failed = false;
            ARDunningLetterUpdate graph = PXGraph.CreateInstance<ARDunningLetterUpdate>();
            PXReportRequiredException ex = null;
            foreach (DetailsResult t in list)
            {
                int? L = t.DunningLetterID;
                ARDunningLetter doc = graph.DL.Select(L.Value);
                PXFilteredProcessing<DetailsResult, PrintParameters>.SetCurrentItem(t);
				if (doc.Released == false || doc.Voided == true)
				{
					PXFilteredProcessing<DetailsResult, PrintParameters>.SetError(CA.Messages.DocumentStatusInvalid);
					failed = true;
					continue;
				}
                if (markOnly)
                {
                    if (filter.ShowAll != true)
                    {
                        doc.DontPrint = true;
                        graph.docs.Cache.Update(doc);
                        PXFilteredProcessing<DetailsResult, PrintParameters>.SetProcessed();
                    }
                }
                else
                {

                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d["ARDunningLetter.DunningLetterID"] = L.ToString();

                    if (doc.Printed != true)
                    {
                        doc.Printed = true;
                        graph.docs.Cache.Update(doc);
                        PXFilteredProcessing<DetailsResult, PrintParameters>.SetProcessed();
                    }
                    ex = PXReportRequiredException.CombineReport(ex, GetCustomerReportID(graph, "AR661000", t), d);
                }
            }


            graph.Save.Press();

            if (ex != null) throw ex;

			if (failed)
				throw new PXException(Messages.OneOrMoreItemsAreNotProcessed);
        }

        public static string GetCustomerReportID(PXGraph graph, string reportID, DetailsResult statement)
        {
			return GetCustomerReportID(graph, reportID, statement.CustomerId, statement.BranchID);
        }
		public static string GetCustomerReportID(PXGraph graph, string reportID, int? CustomerID, int? BranchID)
		{
			Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.SelectWindowed(graph, 0, 1, CustomerID);
			return new NotificationUtility(graph).SearchReport(ARNotificationSource.Customer, customer, reportID, BranchID);
		}
        public static void Email(PrintParameters filter, List<DetailsResult> list, bool markOnly)
        {
            ARDunningLetterUpdate graph = CreateInstance<ARDunningLetterUpdate>();
            int i = 0;
            bool failed = false;
            foreach (DetailsResult it in list)
            {
                try
                {
                    graph.EMailDL(it.DunningLetterID.Value, markOnly, filter.ShowAll == true);
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
            if (failed)
                throw new PXException(ErrorMessages.MailSendFailed);
        }
		public static void Release(PrintParameters filter, List<DetailsResult> list)
		{
			if (list.Count > 0)
			{
				bool failed = false;
				ARDunningLetterMaint graph = PXGraph.CreateInstance<ARDunningLetterMaint>();

				int i = 0;
				foreach (DetailsResult res in list)
				{
					try
					{
						ARDunningLetter doc = PXSelect<ARDunningLetter, Where<ARDunningLetter.dunningLetterID, Equal<Required<DetailsResult.dunningLetterID>>>>.Select(graph, res.DunningLetterID);
						ARDunningLetterMaint.ReleaseProcess(graph, doc);
						PXFilteredProcessing<DetailsResult, PrintParameters>.SetProcessed();
					}
					catch (Exception e)
					{
						failed = true;
						PXFilteredProcessing<DetailsResult, PrintParameters>.SetError(i, e);
					}

					i++;
				}

				if (failed)
				{
					throw new PXException(Messages.OneOrMoreItemsAreNotReleased);
				}
			}
		}
		#endregion
	}

    public class ARDunningLetterUpdate : PXGraph<ARDunningLetterUpdate, Customer>
    {
        [PXViewName(Messages.DunningLetter)]
        public PXSelect<ARDunningLetter> docs;

        public PXSelect<ARDunningLetter, 
                Where<ARDunningLetter.dunningLetterID, Equal<Required<ARDunningLetter.dunningLetterID>>>>   DL;

        public PXSelect<Customer, Where<Customer.bAccountID, Equal<Optional<ARDunningLetter.bAccountID>>>> Customer;

        [PXViewName(Messages.ARContact)]
        public PXSelectJoin<Contact, InnerJoin<Customer, On<Contact.contactID, Equal<Customer.defBillContactID>>>, Where<Customer.bAccountID, Equal<Current<ARDunningLetter.bAccountID>>>> contact;
            
        [CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARDunningLetter.bAccountID>>>>))]
        [CRDefaultMailTo(typeof(Select<ARContact, Where<ARContact.contactID, Equal<Current<Customer.defBillContactID>>>>))]
        public CRActivityList<ARDunningLetter>
            Activity;

        public string notificationCD = "DUNNINGLETTER";
        
        public virtual void EMailDL(int DocID, bool aMarkOnly, bool ShowAll)
        {
            ARDunningLetter iDoc = DL.Select(DocID);
			if (iDoc.Released == false || iDoc.Voided == true)
			{
				throw new PXException(CA.Messages.DocumentStatusInvalid);
			}
            Customer customer = this.Customer.Select(iDoc.BAccountID);
            this.Customer.Current = customer;

            if (aMarkOnly)
            {
                iDoc.DontEmail = true;
            }
            else
            {
                this.DL.Current = iDoc;
                Activity.SendNotification(ARNotificationSource.Customer, notificationCD, iDoc.BranchID, new Dictionary<string, string>() { { "LetterID", DocID.ToString() } });
				iDoc.Emailed = true;
			}
			DL.Cache.Update(iDoc);
			this.Save.Press();
		}
    }
}
