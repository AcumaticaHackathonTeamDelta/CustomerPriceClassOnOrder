using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic; // to use Space(int)

using PX.SM;
using PX.Data;
using PX.Objects.BQLConstants;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.IN
{
	#region FilterDAC

	[Serializable]
	public partial class InventoryTranByAcctEnqFilter : PX.Data.IBqlTable
	{

		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		//[FinPeriodID(typeof(AccessInfo.businessDate))]
		[FinPeriodSelector(typeof(AccessInfo.businessDate))]
		[PXDefault()]
		[PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.Visible)]
		public virtual String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region ByFinancialPeriod
		public abstract class byFinancialPeriod : PX.Data.IBqlField
		{
		}
		protected bool? _ByFinancialPeriod;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "By Financial Period")]
		public virtual bool? ByFinancialPeriod
		{
			get
			{
				return this._ByFinancialPeriod;
			}
			set
			{
				this._ByFinancialPeriod = value;
			}
		}
		#endregion
		#region PeriodStartDate
		public abstract class periodStartDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PeriodStartDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Period Start Date", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
		public virtual DateTime? PeriodStartDate
		{
			get
			{
				return this._PeriodStartDate;
			}
			set
			{
				this._PeriodStartDate = value;
			}
		}
		#endregion
		#region PeriodEndDate
		public abstract class periodEndDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PeriodEndDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Period End Date", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
		public virtual DateTime? PeriodEndDate
		{
			get
			{
				return this._PeriodEndDate;
			}
			set
			{
				this._PeriodEndDate = value;
			}
		}
		#endregion

		#region PeriodEndDateInclusive
		public abstract class periodEndDateInclusive : PX.Data.IBqlField { };
		[PXDBDate()]
		[PXUIField(DisplayName = "Period End Date", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
		public virtual DateTime? PeriodEndDateInclusive
		{
			get
			{
				if (this._PeriodEndDate == null)
				{
					return null;
				}
				else
				{
					return ((DateTime)this._PeriodEndDate).AddDays(-1);
				}
			}
		}
		#endregion


		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date")]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate()]
		//[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "End Date")]
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

		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDefault()]
		[GL.Account(null, typeof(Search5<Account.accountID,
						InnerJoin<INItemCostHist, On<Account.accountID, Equal<INItemCostHist.accountID>>>,
						Where<Match<Current<AccessInfo.userName>>>,
						Aggregate<GroupBy<Account.accountID>>>),
			 DisplayName = "Inventory Account", DescriptionField = typeof(GL.Account.description))]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubCD
		public abstract class subCD : PX.Data.IBqlField
		{
		}
		protected String _SubCD;
		[SubAccountRaw(DisplayName = "Subaccount")]
		public virtual String SubCD
		{
			get
			{
				return this._SubCD;
			}
			set
			{
				this._SubCD = value;
			}
		}
		#endregion
		#region SubCD Wildcard
		public abstract class subCDWildcard : PX.Data.IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual String SubCDWildcard
		{
			get
			{
				return SubCDUtils.CreateSubCDWildcard(this._SubCD, SubAccountAttribute.DimensionName);
			}
		}
		#endregion
		#region SummaryByDay
		public abstract class summaryByDay : PX.Data.IBqlField
		{
		}
		protected bool? _SummaryByDay;
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "Summary By Day")]
		public virtual bool? SummaryByDay
		{
			get
			{
				return this._SummaryByDay;
			}
			set
			{
				this._SummaryByDay = value;
			}
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem, NotEqual<boolFalse>, And<Where<Match<Current<AccessInfo.userName>>>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;		
		[Site(DescriptionField = typeof(INSite.descr), DisplayName = "Warehouse")]		
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
	}

	#endregion

	#region ResultSet
    [Serializable]
	public partial class InventoryTranByAcctEnqResult : PX.Data.IBqlTable
	{

		#region GridLineNbr
		// just for sorting in gris
		public abstract class gridLineNbr : PX.Data.IBqlField { }
		protected Int32? _GridLineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Grid Line Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
		public virtual Int32? GridLineNbr
		{
			get
			{
				return this._GridLineNbr;
			}
			set
			{
				this._GridLineNbr = value;
			}
		}
		#endregion

		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion

		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		//[PXDBInt()]
		[SubAccount(Visibility = PXUIVisibility.Visible)]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion

		#region TranType
		public abstract class tranType : PX.Data.IBqlField { }
		protected String _TranType;
		[PXString(3)] // ???
		[INTranType.List()]
		[PXUIField(DisplayName = "Tran. Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region DocRefNbr
		public abstract class docRefNbr : PX.Data.IBqlField
		{
		}
		protected String _DocRefNbr;
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INRegister.refNbr>))]
		public virtual String DocRefNbr
		{
			get
			{
				return this._DocRefNbr;
			}
			set
			{
				this._DocRefNbr = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField { }
		protected Int32? _InventoryID;
		[Inventory(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Inventory ID")]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion		

		#region SubItemCD
		public abstract class subItemCD : PX.Data.IBqlField
		{
		}
		protected String _SubItemCD;
		[SubItemRawExt(typeof(InventoryTranByAcctEnqResult.inventoryID), DisplayName = "Cost Subitem")]
		public virtual String SubItemCD
		{
			get
			{
				return this._SubItemCD;
			}
			set
			{
				this._SubItemCD = value;
			}
		}
		#endregion

		#region SiteId
		public abstract class siteID : PX.Data.IBqlField { }
		protected Int32? _SiteID;
		//[PXDBInt(IsKey = true)] //???
		[Site(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Warehouse")]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField { }
		protected Int32? _LocationID;
		//            [PXDBInt(IsKey = true)] //???
		[Location(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location (if costed)")]
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
		#region CostAdj
		public abstract class costAdj : PX.Data.IBqlField
		{
		}
		protected Boolean? _CostAdj;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cost Adjustment")]
		public virtual Boolean? CostAdj
		{
			get
			{
				return this._CostAdj;
			}
			set
			{
				this._CostAdj = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion

		#region FinPerNbr
		public abstract class finPerNbr : PX.Data.IBqlField { };
		protected String _FinPerNbr;
		[GL.FinPeriodID()]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String FinPerNbr
		{
			get
			{
				return this._FinPerNbr;
			}
			set
			{
				this._FinPerNbr = value;
			}
		}
		#endregion
		#region TranPerNbr
		public abstract class tranPerNbr : PX.Data.IBqlField { };
		protected String _TranPerNbr;
		[GL.FinPeriodID()]
		[PXUIField(DisplayName = "Tran. Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TranPerNbr
		{
			get
			{
				return this._TranPerNbr;
			}
			set
			{
				this._TranPerNbr = value;
			}
		}
		#endregion

		//  Qtys in stock UOM here
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion

		#region BegBalance
		public abstract class begBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Beginning Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? BegBalance
		{
			get
			{
				return this._BegBalance;
			}
			set
			{
				this._BegBalance = value;
			}
		}
		#endregion
		#region Debit
		public abstract class debit : PX.Data.IBqlField
		{
		}
		protected Decimal? _Debit;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Debit", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? Debit
		{
			get
			{
				return this._Debit;
			}
			set
			{
				this._Debit = value;
			}
		}
		#endregion
		#region Credit
		public abstract class credit : PX.Data.IBqlField
		{
		}
		protected Decimal? _Credit;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Credit", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? Credit
		{
			get
			{
				return this._Credit;
			}
			set
			{
				this._Credit = value;
			}
		}
		#endregion
		#region EndBalance
		public abstract class endBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ending Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? EndBalance
		{
			get
			{
				return this._EndBalance;
			}
			set
			{
				this._EndBalance = value;
			}
		}
		#endregion

		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion

	}
	#endregion


	[PX.Objects.GL.TableAndChartDashboardType]
	public class InventoryTranByAcctEnq : PXGraph<InventoryTranByAcctEnq>
	{

		public PXFilter<InventoryTranByAcctEnqFilter> Filter;

		[PXFilterable]
		public PXSelectJoin<InventoryTranByAcctEnqResult, 
			CrossJoin<INTran>,
			Where<True, Equal<True>>, 
		OrderBy<Asc<InventoryTranByAcctEnqResult.gridLineNbr>>> ResultRecords;
        public PXSelectJoin<InventoryTranByAcctEnqResult,
            CrossJoin<INTran>,
            Where<True, Equal<True>>,
        OrderBy<Asc<InventoryTranByAcctEnqResult.gridLineNbr>>> InternalResultRecords;
		public PXCancel<InventoryTranByAcctEnqFilter> Cancel;
		public PXAction<InventoryTranByAcctEnqFilter> PreviousPeriod;
		public PXAction<InventoryTranByAcctEnqFilter> NextPeriod;

		#region Cache Attached
    public PXSelect<INTran> Tran;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "SO Order Type", Visible = false, Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<SO.SOOrderType.orderType>))]
		protected virtual void INTran_SOOrderType_CacheAttached(PXCache sender)
		{
		}
		protected String _SOOrderNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "SO Order Nbr.", Visible = false, Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<SO.SOOrder.orderNbr>))]
		protected virtual void INTran_SOOrderNbr_CacheAttached(PXCache sender)
		{
		}
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "PO Receipt Nbr.", Visible = false, Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<PO.POReceipt.receiptNbr>))]
		protected virtual void INTran_POReceiptNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		/*
					 protected virtual void InventoryTranByAcctEnqFilter_StartDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
					 {
							 if ( true )
							 {
									 e.NewValue = new DateTime( ((DateTime)this.Accessinfo.BusinessDate).Year, 01, 01);
									 e.Cancel = true;
							 }
					 }
	 */
		public InventoryTranByAcctEnq()
		{
			ResultRecords.Cache.AllowInsert = false;
			ResultRecords.Cache.AllowDelete = false;
			ResultRecords.Cache.AllowUpdate = false;
		}


        protected virtual IEnumerable resultRecords()
        {
            InventoryTranByAcctEnqFilter filter = Filter.Current;


            int startRow = 0;
            int totalRows = 0;

            PXResultset<InventoryTranByAcctEnqResult> usortedList = InternalResultRecords.Select();
            if (usortedList.Count == 0)
                return usortedList;
            decimal beginBalance = ((InventoryTranByAcctEnqResult)usortedList[0]).BegBalance ?? 0m;
            List<object> list = InternalResultRecords.View.Select(PXView.Currents, PXView.Parameters, new object[PXView.SortColumns.Length], PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, 0, ref totalRows);
            foreach (PXResult<InventoryTranByAcctEnqResult> item in list)
            {
                InventoryTranByAcctEnqResult it = (InventoryTranByAcctEnqResult)item;
                it.BegBalance = beginBalance;
                decimal? debit = it.Debit;
                decimal? credit = it.Credit;
                beginBalance += (debit ?? 0m) - (credit ?? 0m);
                it.EndBalance = beginBalance;
            }
            return list;
        }
		protected virtual IEnumerable internalResultRecords()
		{
			const bool useItemCostHistoryToOptimizeBalanceCalculation = true; // false to debug and ensure that INItemCostHistory corresponds to INCostTran

			InventoryTranByAcctEnqFilter filter = Filter.Current;

			bool summaryByDay = filter.SummaryByDay ?? false;

            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.tranType>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.docRefNbr>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.subItemCD>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.siteID>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.locationID>(ResultRecords.Cache, null, !summaryByDay);

            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.accountID>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.subID>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.inventoryID>(ResultRecords.Cache, null, !summaryByDay);

            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.costAdj>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.finPerNbr>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.tranPerNbr>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.qty>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible<InventoryTranByAcctEnqResult.unitCost>(ResultRecords.Cache, null, !summaryByDay);
            PXUIFieldAttribute.SetVisible(Tran.Cache, null, !summaryByDay);
			var resultList = new List<PXResult<InventoryTranByAcctEnqResult, INTran>>();

			decimal cumulativeBalance = 0m;

			if (filter.AccountID == null)
			{
				return resultList;  //empty
			}

			if (filter.FinPeriodID == null)
			{
				return resultList;  //empty
			}

			if (useItemCostHistoryToOptimizeBalanceCalculation)
			{
				PXSelectBase<INItemCostHist> cmd_CostHist = new PXSelectJoinGroupBy<INItemCostHist,

						InnerJoin<Sub,
										On<Sub.subID, Equal<INItemCostHist.subID>>>,

						Where<INItemCostHist.finPeriodID, Less<Current<InventoryTranByAcctEnqFilter.finPeriodID>>>,

						Aggregate<
								Sum<INItemCostHist.tranYtdCost,
								Sum<INItemCostHist.tranBegCost,
								Sum<INItemCostHist.finYtdCost,
								Sum<INItemCostHist.finBegCost>>>>>>(this);

				//if (filter.AccountID != null) // checked above
				{
					cmd_CostHist.WhereAnd<Where<INItemCostHist.accountID, Equal<Current<InventoryTranByAcctEnqFilter.accountID>>>>();
				}

				if (!SubCDUtils.IsSubCDEmpty(filter.SubCD))
				{
					cmd_CostHist.WhereAnd<Where<Sub.subCD, Like<Current<InventoryTranByAcctEnqFilter.subCDWildcard>>>>();
				}
				
				PXResultset<INItemCostHist> costHistResult = (PXResultset<INItemCostHist>)cmd_CostHist.Select();
				if (costHistResult.Count == 1) // 0 is possible too
				{
					if (filter.ByFinancialPeriod == true)
					{
						cumulativeBalance += (((INItemCostHist)costHistResult[0]).FinYtdCost ?? 0m) - (((INItemCostHist)costHistResult[0]).FinBegCost ?? 0m);
					}
					else
					{
						cumulativeBalance += (((INItemCostHist)costHistResult[0]).TranYtdCost ?? 0m) - (((INItemCostHist)costHistResult[0]).TranBegCost ?? 0m);
					}
				}
			}

			PXSelectBase<INTranCost> cmd = new PXSelectReadonly2<INTranCost,
				  InnerJoin<INTran,
									On<INTran.tranType, Equal<INTranCost.tranType>,
									And<INTran.refNbr, Equal<INTranCost.refNbr>,
									And<INTran.lineNbr, Equal<INTranCost.lineNbr>>>>, 
					InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<INTranCost.inventoryID>,
						And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
					InnerJoin<Sub, On<Sub.subID, Equal<INTranCost.invtSubID>>,
					InnerJoin<INSubItem, On<INSubItem.subItemID, Equal<INTranCost.costSubItemID>>,
					LeftJoin<INSite, On<INSite.siteID, Equal<INTranCost.costSiteID>>,
					LeftJoin<INLocation, On<INLocation.locationID, Equal<INTranCost.costSiteID>>>>>>>>,
					Where<INSite.siteID, IsNull,
						Or<Match<INSite, Current<AccessInfo.userName>>>>,
					OrderBy<Asc<INTranCost.tranDate,
						Asc<INTranCost.lastModifiedDateTime>>>>(this);

			//if (filter.FinPeriodID != null) // checked above
			if (filter.ByFinancialPeriod == true)
			{
				cmd.WhereAnd<Where<INTranCost.finPeriodID, Equal<Current<InventoryTranByAcctEnqFilter.finPeriodID>>>>();
			}
			else
			{
				cmd.WhereAnd<Where<INTranCost.tranPeriodID, Equal<Current<InventoryTranByAcctEnqFilter.finPeriodID>>>>();
			}

			//if (filter.AccountID != null) // checked above
			{
				cmd.WhereAnd<Where<INTranCost.invtAcctID, Equal<Current<InventoryTranByAcctEnqFilter.accountID>>>>();
			}

			if (!SubCDUtils.IsSubCDEmpty(filter.SubCD))
			{
				cmd.WhereAnd<Where<Sub.subCD, Like<Current<InventoryTranByAcctEnqFilter.subCDWildcard>>>>();
			}

			if (filter.EndDate != null)
			{
				cmd.WhereAnd<Where<INTranCost.tranDate, LessEqual<Current<InventoryTranByAcctEnqFilter.endDate>>>>();
			}
			if (filter.InventoryID != null)
				cmd.WhereAnd<Where<INTranCost.inventoryID, Equal<Current<InventoryTranByAcctEnqFilter.inventoryID>>>>();

			if (filter.SiteID != null)
				cmd.WhereAnd<Where<INTranCost.costSiteID, Equal<Current<InventoryTranByAcctEnqFilter.siteID>>>>();

			int gridLineNbr = 0;

			foreach (PXResult<INTranCost, INTran, InventoryItem, Sub, INSubItem, INSite, INLocation> it in cmd.Select())
			{
				INTranCost tc_rec = (INTranCost)it;
				INTran t_rec = (INTran)it;				
				INSite s_rec = (INSite)it;
				INLocation l_rec = (INLocation)it;
				INSubItem si_rec = (INSubItem)it;

				decimal rowCost = (tc_rec.InvtMult * tc_rec.TranCost) ?? 0m;

				if (tc_rec.TranDate < filter.StartDate)
				{
					cumulativeBalance += rowCost;
				}
				else
				{
					if (summaryByDay)
					{
						if ((resultList.Count > 0) && (((InventoryTranByAcctEnqResult)resultList[resultList.Count - 1]).TranDate == tc_rec.TranDate))
						{
							InventoryTranByAcctEnqResult lastItem = resultList[resultList.Count - 1];
							if (rowCost >= 0)
							{
								lastItem.Debit += rowCost;
							}
							else
							{
								lastItem.Credit -= rowCost;
							}
							lastItem.EndBalance += rowCost;
							resultList[resultList.Count - 1] = new PXResult<InventoryTranByAcctEnqResult, INTran>(lastItem,null);
						}
						else
						{
							InventoryTranByAcctEnqResult item = new InventoryTranByAcctEnqResult();
							item.BegBalance = cumulativeBalance;
							item.TranDate = tc_rec.TranDate;
							if (rowCost >= 0)
							{
								item.Debit = rowCost;
								item.Credit = 0m;
							}
							else
							{
								item.Debit = 0m;
								item.Credit = -rowCost;
							}
							item.EndBalance = item.BegBalance + rowCost;
							item.GridLineNbr = ++gridLineNbr;
							resultList.Add( new PXResult<InventoryTranByAcctEnqResult, INTran>(item, null));
						}
						cumulativeBalance += rowCost;
					}
					else
					{
						InventoryTranByAcctEnqResult item = new InventoryTranByAcctEnqResult();
						item.BegBalance = cumulativeBalance;
						item.TranDate = tc_rec.TranDate;
						if (rowCost >= 0)
						{
							item.Debit = rowCost;
							item.Credit = 0m;
						}
						else
						{
							item.Debit = 0m;
							item.Credit = -rowCost;
						}
						item.EndBalance = item.BegBalance + rowCost;

						item.AccountID = tc_rec.InvtAcctID;
						item.SubID = tc_rec.InvtSubID;
						item.TranType = tc_rec.TranType;
						item.DocRefNbr = tc_rec.RefNbr;
						item.InventoryID = tc_rec.InventoryID;
						item.SubItemCD = si_rec.SubItemCD;
						if (s_rec.SiteID != null)
						{
							item.SiteID = s_rec.SiteID;
							item.LocationID = null;
						}
						else
							if (l_rec.LocationID != null) //though it's more or less guaranteed
							{
								item.SiteID = l_rec.SiteID;
								item.LocationID = l_rec.LocationID;
							}
						item.TranDate = tc_rec.TranDate;
						item.FinPerNbr = tc_rec.FinPeriodID;
						item.TranPerNbr = tc_rec.TranPeriodID;
						item.Qty = tc_rec.Qty*tc_rec.InvtMult;
						item.UnitCost = (tc_rec.Qty ?? 0m) == 0m ? null : ((tc_rec.TranCost ?? 0m) + (tc_rec.VarCost ?? 0m)) / tc_rec.Qty;
						item.CostAdj = tc_rec.CostRefNbr != tc_rec.RefNbr;
						item.GridLineNbr = ++gridLineNbr;
						resultList.Add( new PXResult<InventoryTranByAcctEnqResult, INTran>(item, t_rec));
						cumulativeBalance += rowCost;
					}
				}
			}
			return resultList;
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		protected virtual void ResetFilterDates(InventoryTranByAcctEnqFilter aRow)
		{
			FinPeriod period = PXSelectReadonly<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(this, aRow.FinPeriodID);
			if (period != null)
			{
				aRow.PeriodStartDate = period.StartDate;
				aRow.PeriodEndDate = period.EndDate;
				aRow.EndDate = null;
				aRow.StartDate = null;
			}
		}

		protected virtual void InventoryTranByAcctEnqFilter_FinPeriodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			InventoryTranByAcctEnqFilter row = (InventoryTranByAcctEnqFilter)e.Row;
			this.ResetFilterDates(row);
		}

		protected virtual void InventoryTranByAcctEnqFilter_SubCD_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
        protected virtual void InventoryTranByAcctEnqFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            ResultRecords.Select();
        }

		#region Button Delegates
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable previousperiod(PXAdapter adapter)
		{
			InventoryTranByAcctEnqFilter filter = Filter.Current as InventoryTranByAcctEnqFilter;
			FinPeriod nextperiod = PXSelect<FinPeriod,
																	Where<FinPeriod.finPeriodID,
																	Less<Current<InventoryTranByAcctEnqFilter.finPeriodID>>>,
																	OrderBy<Desc<FinPeriod.finPeriodID>>
																	>.SelectWindowed(this, 0, 1);
			if (nextperiod == null)
			{
				nextperiod = PXSelectOrderBy<FinPeriod,
														OrderBy<Desc<FinPeriod.finPeriodID>>>.SelectWindowed(this, 0, 1);
			}

			filter.FinPeriodID = nextperiod.FinPeriodID;
            ResetFilterDates(filter);
            return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable nextperiod(PXAdapter adapter)
		{
			InventoryTranByAcctEnqFilter filter = Filter.Current as InventoryTranByAcctEnqFilter;
			FinPeriod nextperiod = PXSelect<FinPeriod,
																	Where<FinPeriod.finPeriodID,
																	Greater<Current<InventoryTranByAcctEnqFilter.finPeriodID>>>,
																	OrderBy<Asc<FinPeriod.finPeriodID>>
																	>.SelectWindowed(this, 0, 1);
			if (nextperiod == null)
			{
				nextperiod = PXSelectOrderBy<FinPeriod,
														OrderBy<Asc<FinPeriod.finPeriodID>>>.SelectWindowed(this, 0, 1);
			}
			filter.FinPeriodID = nextperiod.FinPeriodID;
            ResetFilterDates(filter);
            return adapter.Get();
		}
		#endregion

		#region View Actions
		public PXAction<InventoryTranByAcctEnqFilter> viewItem;
		[PXButton()]
		[PXUIField(DisplayName = "")]
		protected virtual IEnumerable ViewItem(PXAdapter a)
		{
			if (this.ResultRecords.Current != null)
				InventoryItemMaint.Redirect(this.ResultRecords.Current.InventoryID, true);
			return a.Get();
		}

		public PXAction<InventoryTranByAcctEnqFilter> viewSummary;
		[PXButton()]
		[PXUIField(DisplayName = Messages.InventorySummary)]
		protected virtual IEnumerable ViewSummary(PXAdapter a)
		{
			if (this.ResultRecords.Current != null)
				InventorySummaryEnq.Redirect(
					this.ResultRecords.Current.InventoryID,
					this.ResultRecords.Current.SubItemCD,
					this.ResultRecords.Current.SiteID,
					this.ResultRecords.Current.LocationID, false);
			return a.Get();
		}

		public PXAction<InventoryTranByAcctEnqFilter> viewAllocDet;
		[PXButton()]
		[PXUIField(DisplayName = Messages.InventoryAllocDet)]
		protected virtual IEnumerable ViewAllocDet(PXAdapter a)
		{
			if (this.ResultRecords.Current != null)
				InventoryAllocDetEnq.Redirect(
					this.ResultRecords.Current.InventoryID,
					this.ResultRecords.Current.SubItemCD,
					null,
					this.ResultRecords.Current.SiteID,
					this.ResultRecords.Current.LocationID);
			return a.Get();
		}
		#endregion

	}
}


