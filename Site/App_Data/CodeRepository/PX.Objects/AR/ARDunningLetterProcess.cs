using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PX.Objects.AR
{
	public class ARDunningLetterProcess : PXGraph<ARDunningLetterProcess>
	{
		#region internal types definition
		public partial class ARDunningLetterDetail2 : ARDunningLetterDetail
		{
			public new abstract class dunningLetterBAccountID : PX.Data.IBqlField { }
			public new abstract class docType : PX.Data.IBqlField { }
			public new abstract class refNbr : PX.Data.IBqlField { }
			public new abstract class voided : PX.Data.IBqlField { }
			public new abstract class released : PX.Data.IBqlField { }
		}
		[System.SerializableAttribute()]
		[PXProjection(typeof(Select5<AR.Standalone.ARInvoice,
					InnerJoin<ARRegister,
						On<AR.Standalone.ARInvoice.docType, Equal<ARRegister.docType>,
						And<AR.Standalone.ARInvoice.refNbr, Equal<ARRegister.refNbr>,
						And<ARRegister.released, Equal<True>,
						And<ARRegister.openDoc, Equal<True>,
						And<ARRegister.voided, Equal<False>,
						And<ARRegister.pendingPPD, NotEqual<True>,
						And<Where<ARRegister.docType, Equal<ARDocType.invoice>,
							Or<ARRegister.docType, Equal<ARDocType.finCharge>,
							Or<ARRegister.docType, Equal<ARDocType.debitMemo>>>>>>>>>>>,
					InnerJoin<Customer, On<Customer.bAccountID, Equal<ARRegister.customerID>>,
					LeftJoin<ARDunningLetterDetail2,
						On<ARDunningLetterDetail2.dunningLetterBAccountID, Equal<Customer.sharedCreditCustomerID>,
						And<ARDunningLetterDetail2.docType, Equal<ARRegister.docType>,
						And<ARDunningLetterDetail2.refNbr, Equal<ARRegister.refNbr>,
						And<ARDunningLetterDetail2.voided, Equal<False>,
						And<ARDunningLetterDetail2.released, Equal<False>>>>>>,
					LeftJoin<ARDunningLetterDetail,
						On<ARDunningLetterDetail.dunningLetterBAccountID, Equal<Customer.sharedCreditCustomerID>,
						And<ARDunningLetterDetail.docType, Equal<ARRegister.docType>,
						And<ARDunningLetterDetail.refNbr, Equal<ARRegister.refNbr>,
						And<ARDunningLetterDetail.voided, Equal<False>,
						And<ARDunningLetterDetail.released, Equal<True>>>>>>,
					LeftJoin<ARDunningLetter,
						On<ARDunningLetter.dunningLetterID, Equal<ARDunningLetterDetail.dunningLetterID>,
						And<ARDunningLetter.voided, Equal<False>>>>>>>>,
					Where<ARDunningLetterDetail2.released, IsNull>,
					Aggregate<
						GroupBy<AR.Standalone.ARInvoice.revoked,
						GroupBy<ARRegister.released,
						GroupBy<ARRegister.openDoc,
						GroupBy<ARRegister.voided,
						GroupBy<ARRegister.refNbr,
						GroupBy<ARRegister.docType,
						GroupBy<ARDunningLetterDetail2.voided,
						GroupBy<ARDunningLetterDetail2.released,
						GroupBy<ARDunningLetter.voided,
						GroupBy<ARDunningLetter.released>>>>>>>>>>>>))]
		public partial class ARInvoiceWithDL : IBqlTable
		{
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[PXDBInt(BqlField = typeof(ARRegister.customerID))]
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
			#region SharedCreditCustomerID
			public abstract class sharedCreditCustomerID : PX.Data.IBqlField
			{
			}

			[PXDBInt(BqlField = typeof(Customer.sharedCreditCustomerID))]
			public virtual int? SharedCreditCustomerID { get; set; }
			#endregion
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[PXDBInt(BqlField = typeof(ARRegister.branchID))]
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
			#region DocBal
			public abstract class docBal : PX.Data.IBqlField
			{
			}
			protected Decimal? _DocBal;
			[PXDBDecimal(BqlField = typeof(ARRegister.docBal))]
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
			#region DueDate
			public abstract class dueDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DueDate;
			[PXDBDate(BqlField = typeof(ARRegister.dueDate))]
			public virtual DateTime? DueDate
			{
				get
				{
					return this._DueDate;
				}
				set
				{
					this._DueDate = value;
				}
			}
			#endregion
			#region Released
			public abstract class released : PX.Data.IBqlField
			{
			}
			protected Boolean? _Released;
			[PXDBBool(BqlField = typeof(ARRegister.released))]
			public virtual Boolean? Released
			{
				get
				{
					return this._Released;
				}
				set
				{
					this._Released = value;
				}
			}
			#endregion
			#region OpenDoc
			public abstract class openDoc : PX.Data.IBqlField
			{
			}
			protected Boolean? _OpenDoc;
			[PXDBBool(BqlField = typeof(ARRegister.openDoc))]
			public virtual Boolean? OpenDoc
			{
				get
				{
					return this._OpenDoc;
				}
				set
				{
					this._OpenDoc = value;
				}
			}
			#endregion
			#region Voided
			public abstract class voided : PX.Data.IBqlField
			{
			}
			protected Boolean? _Voided;
			[PXDBBool(BqlField = typeof(ARRegister.voided))]
			public virtual Boolean? Voided
			{
				get
				{
					return this._Voided;
				}
				set
				{
					this._Voided = value;
				}
			}
			#endregion
			#region Revoked
			public abstract class revoked : PX.Data.IBqlField
			{
			}
			protected Boolean? _Revoked;
			[PXDBBool(BqlField = typeof(AR.Standalone.ARInvoice.revoked))]
			public virtual Boolean? Revoked
			{
				get
				{
					return this._Revoked;
				}
				set
				{
					this._Revoked = value;
				}
			}
			#endregion
			#region DocType
			public abstract class docType : PX.Data.IBqlField
			{
			}
			protected String _DocType;
			[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(ARRegister.docType))]
			public virtual String DocType
			{
				get
				{
					return this._DocType;
				}
				set
				{
					this._DocType = value;
				}
			}
			#endregion
			#region RefNbr
			public abstract class refNbr : PX.Data.IBqlField
			{
			}
			protected String _RefNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARRegister.refNbr))]
			public virtual String RefNbr
			{
				get
				{
					return this._RefNbr;
				}
				set
				{
					this._RefNbr = value;
				}
			}
			#endregion
			#region DunningLetterLevel
			public abstract class dunningLetterLevel : PX.Data.IBqlField
			{
			}
			protected Int32? _DunningLetterLevel;
			[PXDBInt(BqlField = typeof(ARDunningLetterDetail.dunningLetterLevel))]
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
			#region DunningLetterDate
			public abstract class dunningLetterDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DunningLetterDate;
			[PXDBDate(BqlField = typeof(ARDunningLetter.dunningLetterDate))]
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
			#region DocDate
			public abstract class docDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DocDate;
			[PXDBDate(BqlField = typeof(ARRegister.docDate))]
			public virtual DateTime? DocDate
			{
				get
				{
					return this._DocDate;
				}
				set
				{
					this._DocDate = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, BqlField = typeof(ARRegister.curyID))]
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
			#region CuryOrigDocAmt
			public abstract class curyOrigDocAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryOrigDocAmt;
			[PXDBDecimal(BqlField = typeof(ARRegister.curyOrigDocAmt))]
			public virtual Decimal? CuryOrigDocAmt
			{
				get
				{
					return this._CuryOrigDocAmt;
				}
				set
				{
					this._CuryOrigDocAmt = value;
				}
			}
			#endregion
			#region OrigDocAmt
			public abstract class origDocAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrigDocAmt;
			[PXDBDecimal(BqlField = typeof(ARRegister.origDocAmt))]
			public virtual Decimal? OrigDocAmt
			{
				get
				{
					return this._OrigDocAmt;
				}
				set
				{
					this._OrigDocAmt = value;
				}
			}
			#endregion
			#region CuryDocBal
			public abstract class curyDocBal : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDocBal;
			[PXDBDecimal(BqlField = typeof(ARRegister.curyDocBal))]
			public virtual Decimal? CuryDocBal
			{
				get
				{
					return this._CuryDocBal;
				}
				set
				{
					this._CuryDocBal = value;
				}
			}
			#endregion
		}

		[System.SerializableAttribute()]
		public partial class ARDunningLetterRecordsParameters : IBqlTable
		{
			#region CustomerClass
			public abstract class customerClassID : PX.Data.IBqlField
			{
			}
			protected String _CustomerClassID;
			[PXDBString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Customer Class", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(CustomerClass.customerClassID))]
			public virtual String CustomerClassID
			{
				get
				{
					return this._CustomerClassID;
				}
				set
				{
					this._CustomerClassID = value;
				}
			}
			#endregion
			#region DocDate
			public abstract class docDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DocDate;
			[PXDate()]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Dunning Letter Date", Visibility = PXUIVisibility.Visible, Required = true)]
			public virtual DateTime? DocDate
			{
				get
				{
					return this._DocDate;
				}
				set
				{
					this._DocDate = value;
				}
			}
			#endregion

			#region IncludeNonOverdueDunning
			public abstract class includeNonOverdueDunning : PX.Data.IBqlField
			{
			}
			protected bool? _IncludeNonOverdueDunning;
			[PXDBBool()]
			[PXDefault(typeof(Search<ARSetup.includeNonOverdueDunning>))]
			[PXUIField(DisplayName = Messages.IncludeNonOverdue, Visibility = PXUIVisibility.Visible)]
			public virtual bool? IncludeNonOverdueDunning
			{
				get
				{
					return this._IncludeNonOverdueDunning;
				}
				set
				{
					this._IncludeNonOverdueDunning = value;
				}
			}
			#endregion
			#region IncludeType
			public abstract class includeType : PX.Data.IBqlField
			{
			}
			protected Int32? _IncludeType;
			[PXInt()]
			[PXDefault(0)]
			[PXUIField(DisplayName = "Include Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
			[PXIntList(new int[] { 0, 1 }, new string[] { Messages.IncludeAllToDL, Messages.IncludeLevelsToDL })]
			public virtual Int32? IncludeType
			{
				get
				{
					return this._IncludeType;
				}
				set
				{
					this._IncludeType = value;
				}
			}
			#endregion
			#region LevelFrom
			public abstract class levelFrom : PX.Data.IBqlField
			{
			}
			protected int? _LevelFrom;
			[PXInt()]
			[PXDefault(1, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "From", Enabled = false)]
			public virtual int? LevelFrom
			{
				get
				{
					return this._LevelFrom;
				}
				set
				{
					this._LevelFrom = value;
				}
			}
			#endregion
			#region LevelTo
			public abstract class levelTo : PX.Data.IBqlField
			{
			}
			protected int? _LevelTo;
			[PXInt()]
			[PXDefault(typeof(Search<ARDunningSetup.dunningLetterLevel, Where<True, Equal<True>>, OrderBy<Desc<ARDunningSetup.dunningLetterLevel>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "To", Enabled = false)]
			public virtual int? LevelTo
			{
				get
				{
					return this._LevelTo;
				}
				set
				{
					this._LevelTo = value;
				}
			}
			#endregion
		}

		[Serializable]
		public partial class ARDunningLetterList : IBqlTable
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
			#region CustomerClass
			public abstract class customerClassID : PX.Data.IBqlField
			{
			}
			protected String _CustomerClassID;
			[PXDBString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Customer Class", Visibility = PXUIVisibility.Visible)]
			public virtual String CustomerClassID
			{
				get
				{
					return this._CustomerClassID;
				}
				set
				{
					this._CustomerClassID = value;
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
			[Branch(typeof(PX.Objects.GL.Branch.branchID))]
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
			#region BAccountID
			public abstract class bAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _BAccountID;
			[PXDBInt(IsKey = true)]
			[PXDefault()]
			[Customer(DescriptionField = typeof(Customer.acctName))]
			[PXUIField(DisplayName = "Customer")]
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

			#region DueDate
			public abstract class dueDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DueDate;
			[PXDBDate(IsKey = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Earliest Due Date")]
			public virtual DateTime? DueDate
			{
				get
				{
					return this._DueDate;
				}
				set
				{
					this._DueDate = value;
				}
			}
			#endregion
			#region NumberOfDocuments
			public abstract class numberOfDocuments : PX.Data.IBqlField
			{
			}
			protected int? _NumberOfDocuments;
			[PXInt()]
			[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Number of Documents")]
			public virtual int? NumberOfDocuments
			{
				get
				{
					return this._NumberOfDocuments;
				}
				set
				{
					this._NumberOfDocuments = value;
				}
			}
			#endregion
			#region NumberOfOverdueDocuments
			public abstract class numberOfOverdueDocuments : PX.Data.IBqlField
			{
			}
			protected int? _NumberOfOverdueDocuments;
			[PXInt()]
			[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Number of Overdue Documents")]
			public virtual int? NumberOfOverdueDocuments
			{
				get
				{
					return this._NumberOfOverdueDocuments;
				}
				set
				{
					this._NumberOfOverdueDocuments = value;
				}
			}
			#endregion
			#region OrigDocAmt
			public abstract class origDocAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _OrigDocAmt;
			[PXDBBaseCury()]
			[PXUIField(DisplayName = "Customer Balance")]
			public virtual Decimal? OrigDocAmt
			{
				get
				{
					return this._OrigDocAmt;
				}
				set
				{
					this._OrigDocAmt = value;
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

			#region DunningLetterLevel
			public abstract class dunningLetterLevel : PX.Data.IBqlField
			{
			}
			protected int? _DunningLetterLevel;
			[PXInt()]
			[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Dunning Letter Level")]
			public virtual int? DunningLetterLevel
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
			#region LastDunningLetterDate
			public abstract class lastDunningLetterDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _LastDunningLetterDate;
			[PXDBDate(IsKey = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Last Dunning Letter Date")]
			public virtual DateTime? LastDunningLetterDate
			{
				get
				{
					return this._LastDunningLetterDate;
				}
				set
				{
					this._LastDunningLetterDate = value;
				}
			}
			#endregion
			#region DueDays
			public abstract class dueDays : PX.Data.IBqlField
			{
			}
			protected Int32? _DueDays;
			[PXDBInt()]
			[PXDefault()]
			[PXUIField(DisplayName = "Due Days")]
			public virtual Int32? DueDays
			{
				get
				{
					return this._DueDays;
				}
				set
				{
					this._DueDays = value;
				}
			}
			#endregion
		}

		#endregion

		#region selects+ctor
		public PXFilter<ARDunningLetterRecordsParameters> Filter;
		public PXCancel<ARDunningLetterRecordsParameters> Cancel;

		[PXFilterable]
		public PXFilteredProcessing<ARDunningLetterList, ARDunningLetterRecordsParameters> DunningLetterList;

		public PXSelect<ARSetup> arsetup;

		public List<ARDunningSetup> DunningSetupList = new List<ARDunningSetup>();

		public ARDunningLetterProcess()
		{
			DunningLetterList.Cache.AllowDelete = false;
			DunningLetterList.Cache.AllowInsert = false;
			DunningLetterList.Cache.AllowUpdate = true;

			bool processByCustomer = ((ARSetup)arsetup.Select()).DunningLetterProcessType == 0;
			PXUIFieldAttribute.SetVisible<ARDunningLetterRecordsParameters.includeType>(Filter.Cache, null, !processByCustomer);
			PXUIFieldAttribute.SetVisible<ARDunningLetterRecordsParameters.levelFrom>(Filter.Cache, null, !processByCustomer);
			PXUIFieldAttribute.SetVisible<ARDunningLetterRecordsParameters.levelTo>(Filter.Cache, null, !processByCustomer);

			foreach (ARDunningSetup setup in PXSelectOrderBy<ARDunningSetup, OrderBy<Asc<ARDunningSetup.dunningLetterLevel>>>.Select(this))
			{
				this.DunningSetupList.Add(setup);
			}

			DunningLetterList.SetProcessDelegate(list=>DunningLetterProc(list,Filter.Current));
			PXUIFieldAttribute.SetEnabled(DunningLetterList.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<ARDunningLetterList.selected>(DunningLetterList.Cache, null, true);
			DunningLetterList.SetSelected<ARDunningLetterList.selected>();
		}
		#endregion

		#region events
		protected virtual void ARDunningLetterRecordsParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARDunningLetterRecordsParameters o = (ARDunningLetterRecordsParameters)e.Row;
			if (o != null)
			{
				ARDunningLetterRecordsParameters filter = (ARDunningLetterRecordsParameters)this.Filter.Cache.CreateCopy(o);

				DunningLetterList.SetProcessDelegate(list => DunningLetterProc(list, filter));
			}
		}
		protected virtual void ARDunningLetterRecordsParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARDunningLetterRecordsParameters row = (ARDunningLetterRecordsParameters)e.Row;
			if (row != null)
			{
				bool includeAll = row.IncludeType == 0;
				PXUIFieldAttribute.SetEnabled<ARDunningLetterRecordsParameters.levelFrom>(sender, null, !includeAll);
				PXUIFieldAttribute.SetEnabled<ARDunningLetterRecordsParameters.levelTo>(sender, null, !includeAll);
			}
		}
		#endregion

		#region Delegate for select
		protected virtual IEnumerable dunningLetterList()
		{
			ARDunningLetterRecordsParameters header = Filter.Current;
			if (header == null || header.DocDate == null)
			{
				yield break;
			}

			bool processByCustomer = ((ARSetup)arsetup.Select()).DunningLetterProcessType == 0;
			List<ARDunningLetterList> result = PrepareList(this, header, processByCustomer, DunningSetupList);

			foreach (ARDunningLetterList item in result)
			{
				item.Selected = (DunningLetterList.Locate(item) ?? item).Selected;
			}

			this.DunningLetterList.Cache.Clear();
			foreach (ARDunningLetterList item in result)
			{
				this.DunningLetterList.Insert(item);
			}
			this.DunningLetterList.Cache.IsDirty = false;
			foreach (ARDunningLetterList item in this.DunningLetterList.Cache.Inserted)
			{
				yield return item;
			}
		}

		private static List<ARDunningLetterList> PrepareList(PXGraph graph, ARDunningLetterRecordsParameters header, bool processByCustomer, List<ARDunningSetup> DunningSetupList)
		{
			List<int> DueDaysByLevel = DunningSetupList.Select(setup => setup.DueDays ?? 0).ToList();
			int MaxDunningLevel = DunningSetupList.Count;
			List<PXResult<Customer>> results = new List<PXResult<Customer>>();
			if (processByCustomer)
			{
				results = PXSelectJoinGroupBy<Customer,
					InnerJoin<ARInvoiceWithDL,
						On<ARInvoiceWithDL.customerID, Equal<Customer.bAccountID>,
						And<ARInvoiceWithDL.dueDate, Less<Required<ARDunningLetterRecordsParameters.docDate>>>>,
					LeftJoin<ARBalances,
						On<ARBalances.customerID, Equal<Customer.sharedCreditCustomerID>,
						And<ARBalances.branchID, Equal<ARInvoiceWithDL.branchID>>>>>,
					Where2<Where<Customer.printDunningLetters, Equal<True>,
							Or<Customer.mailDunningLetters, Equal<True>>>,
						And2<Match<Current<AccessInfo.userName>>,
						And<Where<Customer.customerClassID, Equal<Required<ARDunningLetterRecordsParameters.customerClassID>>,
							Or<Required<ARDunningLetterRecordsParameters.customerClassID>, IsNull>>>>>,
					Aggregate<GroupBy<Customer.sharedCreditCustomerID,
						GroupBy<ARInvoiceWithDL.branchID,
						Min<ARInvoiceWithDL.dueDate,
						Sum<ARInvoiceWithDL.docBal,
						Count<ARInvoiceWithDL.refNbr>>>>>>>.Select(graph, header.DocDate, header.CustomerClassID, header.CustomerClassID).ToList();
			}
			else
			{
				var cmd = new PXSelectJoinGroupBy<Customer,
					InnerJoin<ARInvoiceWithDL,
						On<ARInvoiceWithDL.customerID, Equal<Customer.bAccountID>,
						And<ARInvoiceWithDL.revoked, Equal<False>,
						And<ARInvoiceWithDL.dueDate, Less<Required<ARDunningLetterRecordsParameters.docDate>>,
						And<Where<ARInvoiceWithDL.dunningLetterLevel, Equal<Required<ARInvoiceWithDL.dunningLetterLevel>>,
							Or<Where<ARInvoiceWithDL.dunningLetterLevel, IsNull, And<Required<ARInvoiceWithDL.dunningLetterLevel>, Equal<int0>>>>>>>>>,
					LeftJoin<ARBalances,
						On<ARBalances.customerID, Equal<Customer.sharedCreditCustomerID>,
						And<ARBalances.branchID, Equal<ARInvoiceWithDL.branchID>>>>>,
					Where2<Where<Customer.printDunningLetters, Equal<True>,
							Or<Customer.mailDunningLetters, Equal<True>>>,
					And2<Match<Current<AccessInfo.userName>>,
						And<Where<Customer.customerClassID, Equal<Required<ARDunningLetterRecordsParameters.customerClassID>>,
							Or<Required<ARDunningLetterRecordsParameters.customerClassID>, IsNull>>>>>,
					Aggregate<GroupBy<ARInvoiceWithDL.dunningLetterLevel,
						GroupBy<Customer.sharedCreditCustomerID,
						GroupBy<ARInvoiceWithDL.branchID,
						Min<ARInvoiceWithDL.dueDate,
						Sum<ARInvoiceWithDL.docBal,
						Count<ARInvoiceWithDL.refNbr>>>>>>>>(graph);
				if (header.IncludeType == 1)
				{
					cmd.WhereAnd<Where2<Where<ARInvoiceWithDL.dunningLetterLevel, GreaterEqual<Required<ARDunningLetterRecordsParameters.levelFrom>>,
										And<ARInvoiceWithDL.dunningLetterLevel, LessEqual<Required<ARDunningLetterRecordsParameters.levelTo>>>>,
					Or<Where<ARInvoiceWithDL.dunningLetterLevel, IsNull, And<Required<ARDunningLetterRecordsParameters.levelFrom>, Less<int1>>>>>>();
				}

				List<int> levels = new List<int>();
				for (int i = 0; i < DueDaysByLevel.Count; i++)
					levels.Add(i);

				results = levels.Aggregate(results, (current, level) => current.Concat(cmd.Select(
					header.DocDate.Value.AddDays(-1 * DueDaysByLevel[level]), level, level, header.CustomerClassID,
					header.CustomerClassID, header.LevelFrom - 1, header.LevelTo - 1, header.LevelFrom - 1)).ToList());
				}
			List<ARDunningLetterList> returnList = new List<ARDunningLetterList>();
			foreach (PXResult<Customer, ARInvoiceWithDL, ARBalances> res in results)
			{
				Customer customer = res;
				ARInvoiceWithDL invoice = res;
				ARBalances balance = res;
				int currentLevel = 0;
				if (invoice == null)
					continue;
				currentLevel = invoice.DunningLetterLevel ?? 0;
				if (currentLevel == MaxDunningLevel)
					continue;
				if (invoice.DueDate.Value.AddDays(DueDaysByLevel[currentLevel]) >= header.DocDate)
					continue;
				if (currentLevel > 0 && invoice.DunningLetterDate.Value.AddDays(DueDaysByLevel[currentLevel] - DueDaysByLevel[currentLevel - 1]) >= header.DocDate)
					continue;
				if (currentLevel == 0)
				{
					List<ARDunningLetterList> duplicateList = returnList.Where((duplicate) => duplicate.BAccountID == customer.BAccountID && duplicate.BranchID == invoice.BranchID && duplicate.DunningLetterLevel == 1).ToList();
					if (duplicateList.Count > 0)
					{
						duplicateList[0].NumberOfOverdueDocuments += res.RowCount;
						duplicateList[0].DocBal += invoice.DocBal;
						duplicateList[0].LastDunningLetterDate = duplicateList[0].LastDunningLetterDate ?? invoice.DunningLetterDate;
						duplicateList[0].DueDate = duplicateList[0].DueDate > invoice.DueDate ? invoice.DueDate : duplicateList[0].DueDate;
						continue;
					}
				}
				ARDunningLetterList item = new ARDunningLetterList();
				item.BAccountID = customer.SharedCreditCustomerID;
				item.BranchID = invoice.BranchID;
				item.CustomerClassID = customer.CustomerClassID;
				item.DocBal = invoice.DocBal;
				item.DueDate = invoice.DueDate;
				item.DueDays = (DunningSetupList[currentLevel].DaysToSettle ?? 0);
				item.DunningLetterLevel = currentLevel + 1;
				item.LastDunningLetterDate = invoice.DunningLetterDate;
				item.NumberOfOverdueDocuments = res.RowCount;
				item.OrigDocAmt = PXAccess.FeatureInstalled<FeaturesSet.parentChildAccount>()
					? PXSelectJoinGroupBy<ARBalances,
						InnerJoin<Customer, On<Customer.bAccountID, Equal<ARBalances.customerID>>>,
						Where<ARBalances.branchID, Equal<Required<ARBalances.branchID>>,
							And<Customer.sharedCreditCustomerID, Equal<Required<Customer.sharedCreditCustomerID>>>>,
						Aggregate<GroupBy<ARBalances.customerID,
							Sum<ARBalances.currentBal>>>>.Select(graph, item.BranchID, item.BAccountID)
						.Sum(cons => ((ARBalances)cons).CurrentBal)
					: balance.CurrentBal;
				item.NumberOfDocuments = PXSelectJoin<Customer,
					InnerJoin<ARRegister, On<ARRegister.customerID, Equal<Customer.bAccountID>>>,
					Where<Customer.sharedCreditCustomerID, Equal<Required<Customer.sharedCreditCustomerID>>,
						And<ARRegister.released, Equal<True>,
						And<ARRegister.openDoc, Equal<True>,
						And<ARRegister.voided, Equal<False>,
						And<ARRegister.pendingPPD, NotEqual<True>,
						And2<Where<ARRegister.docType, Equal<ARDocType.invoice>,
							Or<ARRegister.docType, Equal<ARDocType.finCharge>,
							Or<ARRegister.docType, Equal<ARDocType.debitMemo>>>>,
						And<ARRegister.docDate, LessEqual<Required<ARRegister.docDate>>>>>>>>>>.Select(graph, item.BAccountID, header.DocDate).Count();

				returnList.Add(item);
			}
			return returnList;
		}
		#endregion

		#region Processing
		private static void DunningLetterProc(List<ARDunningLetterList> list,ARDunningLetterRecordsParameters filter)
		{
			DunningLetterMassProcess graph = PXGraph.CreateInstance<DunningLetterMassProcess>();
			PXLongOperation.StartOperation(graph, delegate()
			{
				bool errorsInProcessing = false;
				ARSetup arsetup = PXSelect<ARSetup>.Select(graph);
				bool autoRelease = arsetup.AutoReleaseDunningLetter == true;
				bool processByCutomer = arsetup.DunningLetterProcessType == 0;
				List<int> DueDaysByLevel = new List<int>();
				foreach (ARDunningSetup setup in PXSelectOrderBy<ARDunningSetup, OrderBy<Asc<ARDunningSetup.dunningLetterLevel>>>.Select(graph))
				{
					DueDaysByLevel.Add(setup.DueDays ?? 0);
				}
				bool consolidateBranch = arsetup.ConsolidatedDunningLetter == true;
				int? consolidationBranch = null;
				if (consolidateBranch)
				{
					consolidationBranch = arsetup.DunningLetterBranchID;
				}

				List<ARDunningLetterList> uniqueList = consolidateBranch
					? DistinctBy(list, a => a.BAccountID).ToList()
					: DistinctBy(list, a => new {a.BAccountID, a.BranchID}).ToList();

				foreach (ARDunningLetterList uniqueItem in uniqueList)
				{
					int? BAccountID = uniqueItem.BAccountID;
					int? BranchID = consolidateBranch ? consolidationBranch : uniqueItem.BranchID;
					int? DueDays = uniqueItem.DueDays;
					List<int> levels = new List<int>();
					List<ARDunningLetterList> listToMerge = consolidateBranch ? 
						list.Where((item) => item.BAccountID == BAccountID).ToList() : 
						list.Where((item) => item.BAccountID == BAccountID && item.BranchID == BranchID).ToList();
					foreach (ARDunningLetterList item in listToMerge)
					{
						DueDays = DueDays < item.DueDays ? DueDays : item.DueDays;
						levels.Add(item.DunningLetterLevel ?? 0);
					}
					try
					{
						ARDunningLetter letterToRelease = CreateDunningLetter(graph, BAccountID, BranchID, filter.DocDate, DueDays, levels, 
							filter.IncludeNonOverdueDunning ?? false, processByCutomer, consolidateBranch, DueDaysByLevel);
						try
						{
							if (autoRelease)
							{
								ARDunningLetterMaint.ReleaseProcess(letterToRelease);
							}
							foreach (ARDunningLetterList item in listToMerge)
							{
								PXProcessing.SetCurrentItem(item);
								PXProcessing.SetProcessed();
							}
						}
						catch (Exception e)
						{
							foreach (ARDunningLetterList item in listToMerge)
							{
								PXProcessing.SetCurrentItem(item);
								PXProcessing.SetWarning(Messages.DunningLetterNotReleased + e.Message);
							}
						}
					}
					catch (Exception e)
					{
						foreach (ARDunningLetterList item in listToMerge)
						{
							PXProcessing.SetCurrentItem(item);
							PXProcessing.SetError(e);
							errorsInProcessing = true;
						}
					}
				}
				if (errorsInProcessing)
				{
					throw new PXException(Messages.DunningLetterNotCreated);
				}
			});
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		public static ARDunningLetter CreateDunningLetter(DunningLetterMassProcess graph, int? BAccountID, int? BranchID, DateTime? docDate, int? DueDays, List<int> includedLevels, bool includeNonOverdue, bool processByCutomer, bool consolidateBranch, List<int> DueDaysByLevel)
		{
			graph.Clear();

			int MaxDunningLevel = DueDaysByLevel.Count;
			ARDunningLetter doc = CreateDunningLetterHeader(graph, BAccountID, BranchID, docDate, DueDays, consolidateBranch);
			doc = graph.docs.Insert(doc);
			foreach (PXResult<ARInvoiceWithDL> result in GetInvoiceList(graph, BAccountID, BranchID, docDate, includedLevels, includeNonOverdue, processByCutomer, consolidateBranch, DueDaysByLevel))
			{
				ARDunningLetterDetail docDet = CreateDunningLetterDetail(docDate, processByCutomer, result, DueDaysByLevel);
				doc.DunningLetterLevel = Math.Max(doc.DunningLetterLevel ?? 0, docDet.DunningLetterLevel ?? 0);
				if (doc.DunningLetterLevel == MaxDunningLevel)
				{
					doc.LastLevel = true;
				}
				graph.docsDet.Insert(docDet);
			}

			graph.docs.Update(doc);
			graph.Actions.PressSave();
			return doc;
		}

		private static List<PXResult<ARInvoiceWithDL>> GetInvoiceList(PXGraph graph, int? BAccountID, int? BranchID, DateTime? docDate, List<int> includedLevels, bool includeNonOverdue, bool processByCutomer, bool consolidateBranch, List<int> DueDaysByLevel)
		{
			if (processByCutomer)
			{
				var cmd = new PXSelectGroupBy<ARInvoiceWithDL,
					Where<ARInvoiceWithDL.sharedCreditCustomerID, Equal<Required<ARInvoiceWithDL.customerID>>,
					   And<ARInvoiceWithDL.docDate, LessEqual<Required<ARInvoiceWithDL.docDate>>>>,
				   Aggregate<GroupBy<ARInvoiceWithDL.released,
					   GroupBy<ARInvoiceWithDL.refNbr,
					   GroupBy<ARInvoiceWithDL.docType>>>>>(graph);

				if (!includeNonOverdue)
				{
					cmd.WhereAnd<Where<ARInvoiceWithDL.dueDate, Less<Required<ARInvoice.dueDate>>>>();
				}
				else
				{
					cmd.WhereAnd<Where<Required<ARInvoice.dueDate>, IsNotNull>>();
				}
				if (!consolidateBranch)
				{
					cmd.WhereAnd<Where<ARInvoiceWithDL.branchID, Equal<Required<ARInvoiceWithDL.branchID>>>>();
				}
				return cmd.Select(BAccountID, docDate, docDate, BranchID).ToList();
			}
			else
			{
				List<PXResult<ARInvoiceWithDL>> results = new List<PXResult<ARInvoiceWithDL>>();

				foreach (int level in includedLevels)
				{
					var cmd = new PXSelectGroupBy<ARInvoiceWithDL,
						Where<ARInvoiceWithDL.sharedCreditCustomerID, Equal<Required<ARInvoiceWithDL.customerID>>,
							And<ARInvoiceWithDL.revoked, Equal<False>,
							And<ARInvoiceWithDL.dueDate, Less<Required<ARInvoice.dueDate>>,
							And<ARInvoiceWithDL.docDate, LessEqual<Required<ARInvoiceWithDL.docDate>>,
							And<Where<ARInvoiceWithDL.dunningLetterLevel, Equal<Required<ARDunningLetter.dunningLetterLevel>>,
								Or<Where<ARInvoiceWithDL.dunningLetterLevel, IsNull,
									And<Required<ARDunningLetter.dunningLetterLevel>, Equal<int0>>>>>>>>>>,
						Aggregate<GroupBy<ARInvoiceWithDL.dunningLetterLevel,
							 GroupBy<ARInvoiceWithDL.released,
							 GroupBy<ARInvoiceWithDL.refNbr,
							 GroupBy<ARInvoiceWithDL.docType>>>>>>(graph);
					if (!consolidateBranch)
					{
						cmd.WhereAnd<Where<ARInvoiceWithDL.branchID, Equal<Required<ARInvoiceWithDL.branchID>>>>();
					}
					results = results.Concat(cmd.Select(BAccountID, docDate.Value.AddDays(-1 * DueDaysByLevel[level - 1]), docDate, level - 1, level - 1, BranchID)).ToList();
					if (level == 1 && includeNonOverdue)
					{
						var cmdLvl1 = new PXSelectGroupBy<ARInvoiceWithDL,
					   Where<ARInvoiceWithDL.sharedCreditCustomerID, Equal<Required<ARInvoiceWithDL.customerID>>,
						   And<ARInvoiceWithDL.revoked, Equal<False>,
						   And2<Where<ARInvoiceWithDL.dunningLetterLevel, IsNull,
							Or<ARInvoiceWithDL.dunningLetterLevel, Equal<int0>>>,
						   And<ARInvoiceWithDL.dueDate, Less<Required<ARInvoiceWithDL.docDate>>,
						   And<ARInvoiceWithDL.dueDate, GreaterEqual<Required<ARInvoiceWithDL.docDate>>,
						   And<ARInvoiceWithDL.docDate, LessEqual<Required<ARInvoiceWithDL.docDate>>>>>>>>,
					   Aggregate<GroupBy<ARInvoiceWithDL.dunningLetterLevel,
							GroupBy<ARInvoiceWithDL.released,
							GroupBy<ARInvoiceWithDL.refNbr,
							GroupBy<ARInvoiceWithDL.docType>>>>>>(graph);
						if (!consolidateBranch)
						{
							cmdLvl1.WhereAnd<Where<ARInvoiceWithDL.branchID, Equal<Required<ARInvoiceWithDL.branchID>>>>();
						}
						results = results.Concat(cmdLvl1.Select(BAccountID, docDate, docDate.Value.AddDays(-1 * DueDaysByLevel[0]), docDate, BranchID)).ToList();
					}
				}
				if (includeNonOverdue)
				{
					var cmdNonOverdue = new PXSelectGroupBy<ARInvoiceWithDL,
						Where<ARInvoiceWithDL.sharedCreditCustomerID, Equal<Required<ARInvoiceWithDL.customerID>>,
							And<ARInvoiceWithDL.revoked, Equal<False>,
							And2<Where<ARInvoiceWithDL.dunningLetterLevel, IsNull,
								Or<ARInvoiceWithDL.dunningLetterLevel, Equal<int0>>>,
							And<ARInvoiceWithDL.dueDate, GreaterEqual<Required<ARInvoiceWithDL.docDate>>,
							And<ARInvoiceWithDL.docDate, LessEqual<Required<ARInvoiceWithDL.docDate>>>>>>>,
						Aggregate<GroupBy<ARInvoiceWithDL.dunningLetterLevel,
							 GroupBy<ARInvoiceWithDL.released,
							 GroupBy<ARInvoiceWithDL.refNbr,
							 GroupBy<ARInvoiceWithDL.docType>>>>>>(graph);
					if (!consolidateBranch)
					{
						cmdNonOverdue.WhereAnd<Where<ARInvoiceWithDL.branchID, Equal<Required<ARInvoiceWithDL.branchID>>>>();
					}
					results = results.Concat(cmdNonOverdue.Select(BAccountID, docDate, docDate, BranchID)).ToList();
				}
				return results;
			}
		}

		private static ARDunningLetter CreateDunningLetterHeader(PXGraph graph, int? BAccountID, int? BranchID, DateTime? docDate, int? DueDays, bool Consolidated)
		{
			ARDunningLetter doc = new ARDunningLetter();
			doc.BAccountID = BAccountID;
			doc.BranchID = BranchID;
			doc.DunningLetterDate = docDate;
			doc.Deadline = docDate.Value.AddDays(DueDays.Value);
			doc.Consolidated = Consolidated;
			doc.Released = false;
			doc.Printed = false;
			doc.Emailed = false;
			doc.LastLevel = false;
			Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(graph, BAccountID);
			doc.DontPrint = customer.PrintDunningLetters == false;
			doc.DontEmail = customer.MailDunningLetters == false;
			return doc;
		}

		private static ARDunningLetterDetail CreateDunningLetterDetail(DateTime? docDate, bool processByCutomer, ARInvoiceWithDL invoice, List<int> DueDaysByLevel)
		{
			ARDunningLetterDetail detail = new ARDunningLetterDetail();

			detail.CuryOrigDocAmt = invoice.CuryOrigDocAmt;
			detail.CuryDocBal = invoice.CuryDocBal;
			detail.CuryID = invoice.CuryID;
			detail.OrigDocAmt = invoice.OrigDocAmt;
			detail.DocBal = invoice.DocBal;
			detail.DueDate = invoice.DueDate;
			detail.DocType = invoice.DocType;
			detail.RefNbr = invoice.RefNbr;
			detail.BAccountID = invoice.CustomerID;
			detail.DunningLetterBAccountID = invoice.SharedCreditCustomerID;
			detail.DocDate = invoice.DocDate;
			detail.Overdue = invoice.DueDate < docDate;
			if ((processByCutomer && invoice.DueDate >= docDate) || invoice.DueDate.Value.AddDays(DueDaysByLevel[invoice.DunningLetterLevel ?? 0]) >= docDate)
			{
				detail.DunningLetterLevel = 0;
			}
			else
			{
				detail.DunningLetterLevel = (invoice.DunningLetterLevel ?? 0) + 1;
			}
			return detail;
		}
		#endregion
	}

	[PXHidden()]
	public class DunningLetterMassProcess : PXGraph<DunningLetterMassProcess>
	{
		[PXViewNameAttribute("DunningLetter")]
		public PXSelect<ARDunningLetter> docs;
		[PXViewNameAttribute("DunningLetterDetail")]
		public PXSelect<ARDunningLetterDetail, Where<ARDunningLetterDetail.dunningLetterID, Equal<Required<ARDunningLetter.dunningLetterID>>>> docsDet;
	}
}
