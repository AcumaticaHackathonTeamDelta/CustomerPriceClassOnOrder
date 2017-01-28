using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CA;
using PX.Objects.Common;
using PX.Objects.CR;

namespace PX.Objects.TX
{

	public class RoundingManager
	{
		private readonly Vendor vendor;
		public RoundingManager(PXGraph graph, int? vendorId)
		{
			vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.Select(graph, vendorId);
		}

		public RoundingManager(Vendor vendor)
		{
			this.vendor = vendor;

		}

		public bool IsRequireRounding
		{
			get { return this.vendor != null && this.vendor.TaxUseVendorCurPrecision != true && vendor.TaxReportRounding != null && vendor.TaxReportPrecision != null; }
		}

		public Vendor CurrentVendor
		{
			get { return vendor; }
		}

		public decimal? Round(decimal? value)
		{

			if (value == null || vendor == null || !IsRequireRounding) return value;

			var decimals = vendor.TaxReportPrecision ?? 2;

			var input = (decimal)value;
			switch (vendor.TaxReportRounding)
			{
				case RoundingTypes.Mathematical:
					return PXRound.Math(input, decimals);
				case RoundingTypes.Ceil:
					return PXRound.Ceil(input, decimals);
				case RoundingTypes.Floor:
					return PXRound.Floor(input, decimals);
				default:
					return value;
			}
		}
	}

	public class TaxHistorySumManager
	{
		public static void UpdateTaxHistorySums(PXGraph graph, RoundingManager rmanager, String taxPeriodId, int? revisionId, int? branchID, Func<TaxReportLine, bool> ShowTaxReportLine = null)
		{
			if (!rmanager.IsRequireRounding)
			{
				return;
			}
            PXCache cache = graph.Caches[typeof(TaxHistory)];
		    using (new PXReadBranchRestrictedScope(branchID))
		    {

		        var lines = PXSelectJoinGroupBy<TaxHistory,
		            InnerJoin<TaxReportLine,
		                On<TaxReportLine.vendorID, Equal<TaxHistory.vendorID>,
		                    And<TaxReportLine.lineNbr, Equal<TaxHistory.lineNbr>>>>,
		            Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
		                And<TaxHistory.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>,
		                    And<TaxHistory.revisionID, Equal<Required<TaxHistory.revisionID>>>>>,
                    Aggregate<GroupBy<TaxHistory.branchID, GroupBy<TaxReportLine.lineNbr,
                        GroupBy<TaxReportLine.netTax, Sum<TaxHistory.reportFiledAmt>>>>>>
		            .Select(graph, rmanager.CurrentVendor.BAccountID, taxPeriodId, revisionId);
		        if (lines.Count == 0) return;

                Dictionary<int, List<int>> taxBucketsAggregatesDict = TaxReportMaint.AnalyseBuckets(graph, (int)rmanager.CurrentVendor.BAccountID,
                                                                                                    TaxReportLineType.TaxAmount, true, ShowTaxReportLine);
                Dictionary<int, int> taxBucketsAggregationDict = TaxReportMaint.TaxBucketAnalizer.TransposeDictionary(taxBucketsAggregatesDict);
                Dictionary<int, List<int>> taxableBucketsAggregatesDict = TaxReportMaint.AnalyseBuckets(graph, (int)rmanager.CurrentVendor.BAccountID,
                                                                                                        TaxReportLineType.TaxableAmount, true, ShowTaxReportLine);
                Dictionary<int, int> taxableBucketsAggregationDict = TaxReportMaint.TaxBucketAnalizer.TransposeDictionary(taxableBucketsAggregatesDict);
		        var bucketsArr = new[]
		            {
		                System.Tuple.Create(taxBucketsAggregatesDict, taxBucketsAggregationDict, TaxReportLineType.TaxAmount),
		                System.Tuple.Create(taxableBucketsAggregatesDict, taxableBucketsAggregationDict,TaxReportLineType.TaxableAmount)
		            };
                var p = (PXResult<TaxPeriod, Company>)PXSelectJoin<TaxPeriod, CrossJoin<Company>, Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
		                                                   And<TaxPeriod.taxPeriodID, Equal<Required<TaxPeriod.taxPeriodID>>>>>
                                                           .SelectWindowed(graph, 0, 1, rmanager.CurrentVendor.BAccountID, taxPeriodId);
                TaxPeriod period = p;
		        Company company = p;
                var r = (PXResult<Currency, CurrencyRateByDate>)PXSelectJoin<Currency,
		                                                             LeftJoin<CurrencyRateByDate,
		                                                             On<CurrencyRateByDate.fromCuryID, Equal<Currency.curyID>,
                                                                     And<CurrencyRateByDate.toCuryID, Equal<Required<Company.baseCuryID>>,
                                                                     And<CurrencyRateByDate.curyRateType, Equal<Required<CurrencyRateByDate.curyRateType>>,
                                                                     And<CurrencyRateByDate.curyEffDate, LessEqual<Required<CurrencyRateByDate.curyEffDate>>,
                                                                     And<Where<CurrencyRateByDate.nextEffDate, Greater<Required<CurrencyRateByDate.curyEffDate>>,
		                                                             Or<CurrencyRateByDate.nextEffDate, IsNull>>>>>>>>,
		                                                             Where<Currency.curyID, Equal<Required<Currency.curyID>>>>
                                                                     .SelectWindowed(graph, 0, 1, company.BaseCuryID,
		                                                                             rmanager.CurrentVendor.CuryRateTypeID,
		                                                                             period.EndDate, period.EndDate,
		                                                                             rmanager.CurrentVendor.CuryID ??
		                                                                             company.BaseCuryID);
                Currency currency = r;
		        CurrencyRateByDate rate = r;
		        foreach (var tuple in bucketsArr)
		        {
                    var netHistory = new Dictionary<int, Dictionary<int, PXResult<TaxHistory, TaxReportLine>>>();
                    var roundedNetAmt = new Dictionary<int, Dictionary<int, decimal>>();
		            if (tuple.Item1 != null && tuple.Item2 != null)
		            {
		                var aggregatesDict = tuple.Item1;
		                var aggregationDict = tuple.Item2;
		                var taxType = tuple.Item3;
		                foreach (PXResult<TaxHistory, TaxReportLine> rec in lines)
		                {
                            if (((TaxReportLine)rec).LineType == taxType)
		                    {
		                        TaxHistory record = rec;
		                        TaxReportLine line = rec;
		                        if (record.BranchID == null) continue;
                                int recbranchID = (int)record.BranchID;
                                int lineNbr = (int)record.LineNbr;
		                        if (aggregatesDict.ContainsKey(lineNbr))
		                        {
		                            if (!netHistory.ContainsKey(recbranchID))
		                            {
		                                netHistory[recbranchID] = new Dictionary<int, PXResult<TaxHistory, TaxReportLine>>();
		                            }
		                            netHistory[recbranchID][lineNbr] = rec;
		                        }
		                        else
		                        {
		                            decimal? filedAmt = record.ReportFiledAmt;
		                            decimal? roundedAmt = rmanager.Round(filedAmt);
			                        if (filedAmt != roundedAmt)
			                        {
				                        cache.Insert(CreateDeltaHistory(graph, record, currency,
                                                                        currency.CuryID != company.BaseCuryID ? rate : null, roundedAmt));
			                        }
			                        if (aggregationDict.ContainsKey(lineNbr))
		                            {
		                                int aggrLineNo = aggregationDict[lineNbr];
                                        AddAmt(ref roundedNetAmt, recbranchID, aggrLineNo, (roundedAmt ?? 0m) * (line.LineMult ?? 0m));
		                            }
		                        }
		                    }
		                }
		                foreach (var branchNetAmt in roundedNetAmt)
		                {
		                    foreach (var kvp in aggregatesDict)
		                    {
		                        int lineNbr = kvp.Key;
		                        if (netHistory[branchNetAmt.Key].ContainsKey(lineNbr))
		                        {
		                            TaxHistory record = netHistory[branchNetAmt.Key][lineNbr];
		                            TaxReportLine line = netHistory[branchNetAmt.Key][lineNbr];
									decimal? filedAmt = record.ReportFiledAmt;
		                            decimal? roundedAmt = branchNetAmt.Value[lineNbr];
			                        if (filedAmt != roundedAmt)
			                        {
				                        cache.Insert(CreateDeltaHistory(graph, record, currency,
				                                                        currency.CuryID != company.BaseCuryID ? rate : null, roundedAmt));
			                        }
			                        if (aggregationDict.ContainsKey(lineNbr))
		                            {
		                                int aggrLineNo = aggregationDict[lineNbr];
                                        AddAmt(ref roundedNetAmt, branchNetAmt.Key, aggrLineNo, (roundedAmt ?? 0m) * (line.LineMult ?? 0m));
		                            }
		                        }
		                    }
		                }
		            }
		        }
		    }
		    cache.Persist(PXDBOperation.Insert);
			cache.Persisted(false);
		}
        private static void AddAmt(ref Dictionary<int, Dictionary<int, decimal>> list, int branchId, int lineId, decimal amt)
        {
            if (!list.ContainsKey(branchId))
            {
                list[branchId] = new Dictionary<int, decimal>();
            }
            if (!list[branchId].ContainsKey(lineId))
            {
                list[branchId][lineId] = 0m;
            }
            list[branchId][lineId] += amt;
        }
		private static TaxHistory CreateDeltaHistory(PXGraph graph, TaxHistory original, Currency currency, CurrencyRateByDate rate, decimal? roundedAmt)
		{
			TaxHistory delta = new TaxHistory();
			delta.BranchID = original.BranchID;
			delta.VendorID = original.VendorID;
			delta.CuryID = original.CuryID;
			delta.TaxID = string.Empty;
			delta.TaxPeriodID = original.TaxPeriodID;
			delta.LineNbr = original.LineNbr;
			delta.RevisionID = original.RevisionID;
			delta.ReportFiledAmt = roundedAmt - original.ReportFiledAmt;
			if (rate == null)
				delta.FiledAmt = delta.ReportFiledAmt;
			else
			{
				if (rate.CuryRate == null)
					throw new PXException(CM.Messages.RateNotFound);
				delta.FiledAmt = RecalcCurrency(currency, rate, delta.ReportFiledAmt.GetValueOrDefault());
			}

			if (delta.ReportFiledAmt > 0)
			{
				delta.AccountID = currency.RoundingLossAcctID;
                delta.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(graph, delta.BranchID, currency);
			}
			else
			{
				delta.AccountID = currency.RoundingGainAcctID;
                delta.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(graph, delta.BranchID, currency);
			}
			if (delta.AccountID == null || delta.SubID == null)
				throw new PXException(Messages.CannotPrepareReportDefineRoundingAccounts);

			return delta;
		}
		public static PXResultset<TaxReportLine, TaxHistory> GetPreviewReport(PXGraph graph, Vendor vendor, PXResultset<TaxReportLine, TaxHistory> records, Func<TaxReportLine, bool> ShowTaxReportLine = null)
		{
            if (records.Count == 0) return records;
			RoundingManager rmanager = new RoundingManager(vendor);
            Dictionary<int, List<int>> taxAggregatesDict = TaxReportMaint.AnalyseBuckets(graph, (int)vendor.BAccountID, TaxReportLineType.TaxAmount, true, ShowTaxReportLine) ?? new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> taxableAggregatesDict = TaxReportMaint.AnalyseBuckets(graph, (int)vendor.BAccountID, TaxReportLineType.TaxableAmount, true, ShowTaxReportLine) ?? new Dictionary<int, List<int>>();
            Dictionary<int, List<int>>[] aggregatesArr = { taxAggregatesDict, taxableAggregatesDict };
            Dictionary<int, PXResult<TaxReportLine, TaxHistory>> lineDict = new Dictionary<int, PXResult<TaxReportLine, TaxHistory>>();
			foreach (PXResult<TaxReportLine, TaxHistory> record in records)
			{
                TaxReportLine lineTmp = record;
                ((TaxHistory)record).ReportUnfiledAmt = rmanager.Round(((TaxHistory)record).ReportUnfiledAmt);
                lineDict[(int)lineTmp.LineNbr] = record;
			}
            foreach (var aggregatesDict in aggregatesArr)
		    {
		        if (aggregatesDict != null)
		        {
		            foreach (KeyValuePair<int, List<int>> kvp in aggregatesDict)
		            {
		                decimal? sum = 0m;
						if (lineDict.ContainsKey(kvp.Key))
						{
                        TaxHistory aggrTaxHistory = (TaxHistory)lineDict[kvp.Key];
		                foreach (int line in kvp.Value)
		                {
		                    if (lineDict.ContainsKey(line))
		                    {
		                        PXResult<TaxReportLine, TaxHistory> currline = lineDict[line];
                                TaxReportLine taxLine = (TaxReportLine)currline;
                                TaxHistory taxHistory = (TaxHistory)currline;
		                        if (taxHistory.ReportUnfiledAmt != null)
		                        {
                                    sum += taxLine.LineMult * taxHistory.ReportUnfiledAmt;
		                        }
		                    }
		                }
		                aggrTaxHistory.ReportUnfiledAmt = sum;
		            }
		        }
		    }
		    }
            
			return records;
		}
		public static decimal RecalcCurrency(Currency cury, CurrencyRate rate, decimal value)
		{
			if (rate.CuryRate == null || rate.CuryRate == 0) return 0;

			return Math.Round(rate.CuryMultDiv == CuryMultDivType.Mult ? value * rate.CuryRate.Value :
												rate.CuryMultDiv == CuryMultDivType.Div ? value / rate.CuryRate.Value : 0, cury.DecimalPlaces ?? 2,
												MidpointRounding.AwayFromZero);
		}
	}
	[System.SerializableAttribute()]
	public partial class TaxPeriodFilter : IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[MasterBranch(IsDBField = false)]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
        [Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), DisplayName = "Tax Agency")]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID()]
		[PXDefault()]
		[PXUIField(DisplayName = "Reporting Period", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<TaxPeriod.taxPeriodID, Where<TaxPeriod.vendorID,
		Equal<Current<TaxPeriodFilter.vendorID>>>>), DirtyRead = true)]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionId : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionId;
		[PXDBInt()]
        [PXUIField(DisplayName = "Revision")]
		[PXSelector(typeof(Search4<TaxHistory.revisionID,
											 Where<TaxHistory.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
												 And<TaxHistory.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>>>,
											 Aggregate<GroupBy<TaxHistory.revisionID>>,
											 OrderBy<Asc<TaxHistory.revisionID>>>))]
		public virtual Int32? RevisionId
		{
			get
			{
				return this._RevisionId;
			}
			set
			{
				this._RevisionId = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDate()]
		[PXUIField(DisplayName = "From", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		[PXDate()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region ShowDifference
		public abstract class showDifference : PX.Data.IBqlField { }
		protected bool? _ShowDifference;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Show Difference", Visible = false)]
		public virtual bool? ShowDifference
		{
			get
			{
				return _ShowDifference;
			}
			set
			{
				this._ShowDifference = value;
			}
		}
		#endregion
		#region PreparedWarningMsg
		public abstract class preparedWarningMsg : PX.Data.IBqlField { }
		protected string _PreparedWarningMsg;
		[PXString()]
		public virtual string PreparedWarningMsg
		{
			get { return _PreparedWarningMsg; }
			set { this._PreparedWarningMsg = value; }
		}
		#endregion
	}

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class TaxPeriodMaster : TaxPeriod
	{
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), IsKey = true, DisplayName = "Tax Agency ID", Required = true)]
		public override Int32? VendorID
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
		#region TaxPeriodID
		public new abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		[GL.FinPeriodID(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Reporting Period", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXSelector(typeof(Search<TaxPeriodMaster.taxPeriodID, Where<TaxPeriodMaster.vendorID,
		Equal<Optional<TaxPeriodMaster.vendorID>>, And<Where<TaxPeriodMaster.status, Equal<TaxPeriodStatus.prepared>,
		Or<TaxPeriodMaster.status, Equal<TaxPeriodStatus.closed>, Or<TaxPeriodMaster.status, Equal<TaxPeriodStatus.open>>>>>>>))]
		public override String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region StartDate
		public new abstract class startDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "From", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public override DateTime? StartDate
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
		public new abstract class endDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public override DateTime? EndDate
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
		#region EndDateInclusive
		public abstract class endDateInclusive : PX.Data.IBqlField { }
		[PXDate()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? EndDateInclusive
		{
			[PXDependsOnFields(typeof(endDate))]
			get
			{
				return this._EndDate == null ? (DateTime?)null : ((DateTime)this._EndDate).AddDays(-1);
			}
			set
			{
				this._EndDate = value == null ? (DateTime?)null : ((DateTime)value).AddDays(+1);
			}
		}
		#endregion
		#region Status
		public new abstract class status : PX.Data.IBqlField
		{
		}
		#endregion
		#region RevisionID
		public abstract class revisionId : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionId;
		[PXInt()]
		[PXUIField(DisplayName = "Revision ID")]
		[PXSelector(typeof(Search4<TaxHistory.revisionID,
											 Where<TaxHistory.vendorID, Equal<Current<TaxPeriodMaster.vendorID>>,
												 And<TaxHistory.taxPeriodID, Equal<Current<TaxPeriodMaster.taxPeriodID>>>>,
											 Aggregate<GroupBy<TaxHistory.revisionID>>,
											 OrderBy<Asc<TaxHistory.revisionID>>>))]
		[PXDBScalar(typeof(Search4<TaxHistory.revisionID,
											 Where<TaxHistory.vendorID, Equal<TaxPeriod.vendorID>,
												 And<TaxHistory.taxPeriodID, Equal<TaxPeriod.taxPeriodID>>>,
											 Aggregate<Max<TaxHistory.revisionID>>>))]
		public virtual Int32? RevisionId
		{
			get
			{
				return this._RevisionId;
			}
			set
			{
				this._RevisionId = value;
			}
		}
		#endregion				

		public static implicit operator TaxPeriodFilter(TaxPeriodMaster master)
		{
			return new TaxPeriodFilter()
							{
								VendorID = master.VendorID,
								TaxPeriodID = master.TaxPeriodID,
								RevisionId = master.RevisionId
							};
		}
	}

	[System.SerializableAttribute()]
	public partial class TaxHistoryMaster : IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[MasterBranch(IsDBField = false)]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), DisplayName = "Tax Agency ID")]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID()]
		[PXDefault()]
		[PXUIField(DisplayName = "Reporting Period", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<TaxPeriod.taxPeriodID, Where<TaxPeriod.vendorID, Equal<Optional<TaxHistoryMaster.vendorID>>, And<Where<TaxPeriod.status, Equal<TaxPeriodStatus.prepared>, Or<TaxPeriod.status, Equal<TaxPeriodStatus.closed>>>>>>))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Report Line", Visibility = PXUIVisibility.SelectorVisible)]
		[PXIntList(new int[] { 0 }, new string[] { "undefined" })]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "From", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		[PXDefault()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		#region EndDateInclusive
		public abstract class endDateInclusive : PX.Data.IBqlField { }
		[PXDate()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? EndDateInclusive
		{
			get { return this._EndDate == null ? (DateTime?)null : ((DateTime)this._EndDate).AddDays(-1); }
			set { this._EndDate = value == null ? (DateTime?)null : ((DateTime)value).AddDays(+1); }
		}
		#endregion
	}

	[PXProjection(typeof(Select5<TaxPeriodMaster, LeftJoin<TaxPeriod,
		On<TaxPeriod.vendorID, Equal<TaxPeriodMaster.vendorID>,
		And<TaxPeriod.status, Equal<TaxPeriodStatus.open>>>>,
		Aggregate<
		GroupBy<TaxPeriodMaster.vendorID,
		GroupBy<TaxPeriodMaster.taxPeriodID,
		Max<TaxPeriodMaster.endDate,
		Min<TaxPeriod.taxPeriodID>>>>>>))]
    
    [PXHidden]
	public partial class TaxPeriodPlusOpen : PX.Data.IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxPeriodMaster.vendorID))]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(TaxPeriodMaster.taxPeriodID))]
		[PXUIField(DisplayName = "Tax Period ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxPeriodMaster.status))]
		[TaxPeriodStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate(BqlField = typeof(TaxPeriodMaster.endDate))]
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
		#region OpenPeriodID
		public abstract class openPeriodID : PX.Data.IBqlField
		{
		}
		protected String _OpenPeriodID;
		[PXDBString(6, IsFixed = true, BqlField = typeof(TaxPeriod.taxPeriodID))]
		public virtual String OpenPeriodID
		{
			get
			{
				return this._OpenPeriodID;
			}
			set
			{
				this._OpenPeriodID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select<TaxPeriod, Where<TaxPeriod.status, NotEqual<TaxPeriodStatus.open>>>))]
	public partial class TaxPeriodForReportShowing : PX.Data.IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), IsKey = true, BqlField = typeof(TaxPeriod.vendorID), DisplayName = "Tax Agency ID")]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected string _TaxPeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(TaxPeriod.taxPeriodID))]
		[PXSelector(typeof(Search<TaxPeriod.taxPeriodID, 
								Where<TaxPeriod.vendorID, Equal<Optional<TaxPeriodForReportShowing.vendorID>>,
										And<TaxPeriod.status, NotEqual<TaxPeriodStatus.open>>>>))]
		[PXUIField(DisplayName = "Tax Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected string _Status;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxPeriod.status))]
		[PXDefault(TaxPeriodStatus.Open)]
		[TaxPeriodStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate(BqlField = typeof(TaxPeriod.endDate))]
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

		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXSelector(typeof(Search4<TaxHistory.revisionID,
				Where<TaxHistory.vendorID, Equal<Optional<TaxPeriodForReportShowing.vendorID>>, And<Where<TaxHistory.taxPeriodID, Equal<Optional<TaxPeriodForReportShowing.taxPeriodID>>>>>,
				Aggregate<GroupBy<TaxHistory.revisionID>>>))]
		[PXUIField(DisplayName = "Revision ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXInt()]
		public virtual Int32? RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select<TaxPeriodPlusOpen, Where<TaxPeriodPlusOpen.taxPeriodID, Equal<TaxPeriodPlusOpen.openPeriodID>, Or<TaxPeriodPlusOpen.status, NotEqual<TaxPeriodStatus.open>>>>))]

	public partial class TaxPeriodEffective : PX.Data.IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), IsKey = true, BqlField = typeof(TaxPeriodPlusOpen.vendorID), DisplayName = "Tax Agency ID")]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected string _TaxPeriodID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(TaxPeriodPlusOpen.taxPeriodID))]
		[PXSelector(typeof(Search<TaxPeriodPlusOpen.taxPeriodID, Where<TaxPeriodPlusOpen.vendorID, Equal<Optional<TaxPeriodEffective.vendorID>>, And<Where<TaxPeriodPlusOpen.taxPeriodID, Equal<TaxPeriodPlusOpen.openPeriodID>, Or<TaxPeriodPlusOpen.status, NotEqual<TaxPeriodStatus.open>>>>>>))]
		[PXUIField(DisplayName = "Tax Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected string _Status;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxPeriodPlusOpen.status))]
		[PXDefault(TaxPeriodStatus.Open)]
		[TaxPeriodStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate(BqlField = typeof(TaxPeriodPlusOpen.endDate))]
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

		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXSelector(typeof(Search4<TaxHistory.revisionID,
				Where<TaxHistory.vendorID, Equal<Optional<TaxPeriodEffective.vendorID>>, And<Where<TaxHistory.taxPeriodID, Equal<Optional<TaxPeriodEffective.taxPeriodID>>>>>,
				Aggregate<GroupBy<TaxHistory.revisionID>>>))]
		[PXUIField(DisplayName = "Revision ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXInt()]
		public virtual Int32? RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select<TaxTran>))]
	[PXCacheName(Messages.TaxTranReport)]
	public partial class TaxTranReport : TaxTran
	{	
		#region BranchID
		public new abstract class branchID : IBqlField { }
		#endregion
		#region RefNbr
		public new abstract class refNbr : IBqlField { }
		#endregion
		#region VendorID
		public new abstract class vendorID : IBqlField { }
		#endregion
		#region AccountID
		public new abstract class accountID : IBqlField { }
		#endregion
		#region SubID
		public new abstract class subID : IBqlField { }
		#endregion
		#region TaxPeriodID
		public new abstract class taxPeriodID : IBqlField { }
		#endregion
		#region FinPeriodID
		public new abstract class finPeriodID : IBqlField { }
		#endregion
		#region tstamp
		public new abstract class tstamp : IBqlField { }
		#endregion
		#region Module
		public new abstract class module : IBqlField { }
		#endregion
		#region TranType
		public new abstract class tranType : IBqlField { }
		#endregion
		#region TranTypeInvoiceDiscriminated
		public abstract class tranTypeInv : IBqlField { }

		protected String _TranTypeInvoiceDiscriminated;
		[PXString]
		[TaxTranType.List()]
		[PXDBCalced(typeof(Switch<
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAP>,
					And<TaxTran.tranType, Equal<APDocType.invoice>>>,
				TaxTranType.apInvoice,
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAR>,
					And<TaxTran.tranType, Equal<ARDocType.invoice>>>,
				TaxTranType.arInvoice>>,
			TaxTran.tranType>),
			typeof(string))]
		[PXUIField(DisplayName = "Tran. Type")]
		public virtual String TranTypeInvoiceDiscriminated
		{
			get
			{
				return this._TranTypeInvoiceDiscriminated;
			}
			set
			{
				this._TranTypeInvoiceDiscriminated = value;
			}
		}

		#endregion
		#region Released
		public new abstract class released : IBqlField { }
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}
		#endregion
		#region TaxID
		public new abstract class taxID : IBqlField { }
		#endregion
		#region TaxRate
		public new abstract class taxRate : IBqlField { }
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : IBqlField { }
		#endregion
		#region CuryTaxableAmt
		public new abstract class curyTaxableAmt : IBqlField { }
		#endregion
		#region TaxableAmt
		public new abstract class taxableAmt : IBqlField { }
		#endregion
		#region CuryTaxAmt
		public new abstract class curyTaxAmt : IBqlField { }
		#endregion
		#region TaxAmt
		public new abstract class taxAmt : IBqlField { }
		#endregion
		#region CuryID
		public new abstract class curyID : IBqlField { }
		#endregion
		#region ReportCuryID
		public new abstract class reportCuryID : IBqlField { }
		#endregion
		#region ReportCuryRateTypeID
		public new abstract class reportCuryRateTypeID : IBqlField { }
		#endregion
		#region ReportCuryEffDate
		public new abstract class reportCuryEffDate : IBqlField { }
		#endregion
		#region ReportCuryMultDiv
		public new abstract class reportCuryMultDiv : IBqlField { }
		#endregion
		#region ReportCuryRate
		public new abstract class reportCuryRate : IBqlField { }
		#endregion
		#region ReportTaxableAmt
		public new abstract class reportTaxableAmt : IBqlField { }
		#endregion
		#region ReportTaxAmt
		public new abstract class reportTaxAmt : IBqlField { }
		#endregion
		#region CreatedByID
		public new abstract class createdByID : IBqlField { }
		#endregion
		#region CreatedByScreenID
		public new abstract class createdByScreenID : IBqlField { }
		#endregion
		#region CreatedDateTime
		public new abstract class createdDateTime : IBqlField { }
		#endregion
		#region LastModifiedByID
		public new abstract class lastModifiedByID : IBqlField { }
		#endregion
		#region LastModifiedByScreenID
		public new abstract class lastModifiedByScreenID : IBqlField { }
		#endregion
		#region LastModifiedDateTime
		public new abstract class lastModifiedDateTime : IBqlField { }
		#endregion
		#region TranDate
		public new abstract class tranDate : IBqlField { }
		#endregion
		#region TaxBucketID
		public new abstract class taxBucketID : IBqlField { }
		#endregion
		#region TaxType
		public new abstract class taxType : IBqlField { }
		#endregion
		#region TaxZoneID
		public new abstract class taxZoneID : IBqlField { }
		#endregion
		#region TaxInvoiceNbr
		public new abstract class taxInvoiceNbr : IBqlField { }
		#endregion
		#region TaxInvoiceDate
		public new abstract class taxInvoiceDate : IBqlField { }
		#endregion
		#region OrigTranType
		public new abstract class origTranType : IBqlField { }
		#endregion
		#region OrigRefNbr
		public new abstract class origRefNbr : IBqlField { }
		#endregion
		#region RevisionID
		public new abstract class revisionID : IBqlField { }
		#endregion
		#region BAccountID
		public new abstract class bAccountID : IBqlField { }
		#endregion
		#region FinDate
		public new abstract class finDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _FinDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "FinDate", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		public override DateTime? FinDate
		{
			get;
			set;
			}
		#endregion
		#region Sign
		public abstract class sign : PX.Data.IBqlField
		{
		}
		protected decimal? _Sign;

		/// <summary>
		/// Sign of TaxTran with which it will adjust net tax amount.
		/// Consists of following multipliers:
		/// - Tax type of TaxTran:
		///		- Sales (Output): 1
		///		- Purchase (Input): -1
		/// - Document type and module:
		///		- AP
		///			- Debit Adjustment, Voided Quick Check, Refund: -1  
		///			- Invoice, Credit Adjustment, Quik Check, Voided Check, any other not listed: 1  
		///		- AR
		///			- Credit Memo, Cash Return: -1  
		///			- Invoice, Debit Memo, Fin Charge, Cash Sale, any other not listed: 1 
		///		- GL
		///			- Reversing GL Entry: -1  
		///			- GL Entry, any other not listed: 1   
		///		- CA: 1 
		///		- Any other not listed combinations: -1
		/// </summary>
		[PXDecimal]
		public virtual decimal? Sign
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType))]
			get
			{
				return ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, 1);
			}
			set
			{
				this._Sign = value;
			}
		}
		#endregion
		#region Description
		public new abstract class description : PX.Data.IBqlField
		{
		}
		#endregion
		#region AdjdDocType
		public new abstract class adjdDocType : PX.Data.IBqlField
		{
		}
		#endregion
		#region AdjdRefNbr
		public new abstract class adjdRefNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region AdjNbr
		public new abstract class adjNbr : PX.Data.IBqlField
		{
		}
		#endregion
	}

	[PXProjection(typeof(Select2<TaxPeriodForReportShowing,
		InnerJoin<TaxReportLine,
			On<TaxReportLine.vendorID, Equal<TaxPeriodForReportShowing.vendorID>>,
		InnerJoin<TaxBucketLine,
			On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>, And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>>>,
		LeftJoin<TaxTran,
			On<TaxTran.vendorID, Equal<TaxBucketLine.vendorID>,
				And<TaxTran.released, Equal<True>,
				And<TaxTran.voided, Equal<False>,
				And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
				And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
				And<TaxTran.taxBucketID, Equal<TaxBucketLine.bucketID>,
				And2<Where<TaxReportLine.taxZoneID, IsNull,
						And<TaxReportLine.tempLine, Equal<False>,
						Or<TaxReportLine.taxZoneID, Equal<TaxTran.taxZoneID>>>>,
				And<TaxTran.taxPeriodID, Equal<TaxPeriodForReportShowing.taxPeriodID>>>>>>>>>>>>>))]
	[PXCacheName(Messages.TaxDetailReport)]
	public partial class TaxDetailReport : PX.Data.IBqlTable
	{
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxReportLine.lineNbr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region LineMult
		public abstract class lineMult : PX.Data.IBqlField
		{
		}
		protected Int16? _LineMult;
		[PXDBShort(BqlField = typeof(TaxReportLine.lineMult))]
		public virtual Int16? LineMult
		{
			get
			{
				return this._LineMult;
			}
			set
			{
				this._LineMult = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected string _LineType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxReportLine.lineType))]
		public virtual string LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.module))]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.tranType))]
		[TaxAdjustmentType.List()]
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
		#region TranTypeInvoiceDiscriminated
		public abstract class tranTypeInvoiceDiscriminated : PX.Data.IBqlField
		{
		}
		protected String _TranTypeInvoiceDiscriminated;

		[PXString]
		[PXDBCalced(typeof(Switch<
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAP>,
					And<TaxTran.tranType, Equal<APDocType.invoice>>>,
				TaxTranType.apInvoice,
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAR>,
					And<TaxTran.tranType, Equal<ARDocType.invoice>>>,
				TaxTranType.arInvoice>>,
			TaxTran.tranType>),
			typeof(string))]
		[TaxTranType.List()]
		public virtual String TranTypeInvoiceDiscriminated
		{
			get
			{
				return this._TranTypeInvoiceDiscriminated;
			}
			set
			{
				this._TranTypeInvoiceDiscriminated = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.refNbr))]
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
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxTran.recordID))]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool(BqlField = typeof(TaxTran.released))]
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool(BqlField = typeof(TaxTran.voided))]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(BqlField = typeof(TaxPeriodForReportShowing.taxPeriodID))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected string _TaxID;
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.taxID))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(TaxTran.vendorID))]
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(TaxTran.branchID))]
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
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(TaxTran.taxZoneID))]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(null, BqlField = typeof(TaxTran.accountID))]
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
		[SubAccount(typeof(TaxTran.accountID), BqlField = typeof(TaxTran.subID))]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate(BqlField = typeof(TaxTran.tranDate))]
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
		#region TaxType
		public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxTran.taxType))]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxRate
		public abstract class taxRate : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxRate;
		[PXDBDecimal(6, BqlField = typeof(TaxTran.taxRate))]
		public virtual Decimal? TaxRate
		{
			get
			{
				return this._TaxRate;
			}
			set
			{
				this._TaxRate = value;
			}
		}
		#endregion
		#region ReportTaxableAmt
		public abstract class reportTaxableAmt : IBqlField
		{
		}

		protected Decimal? _ReportTaxableAmt;

		[PXDBDecimal(BqlField = typeof(TaxTran.reportTaxableAmt))]
		public virtual Decimal? ReportTaxableAmt
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(lineMult))]
			get
			{
				decimal HistMult = ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, this._LineMult);
				return HistMult * this._ReportTaxableAmt;
			}
			set
			{
				this._ReportTaxableAmt = value;
			}
		}
		#endregion
		#region ReportTaxAmt
		public abstract class reportTaxAmt : IBqlField
		{
		}
		
		protected Decimal? _ReportTaxAmt;

		[PXDBDecimal(BqlField = typeof(TaxTran.reportTaxAmt))]
		public virtual Decimal? ReportTaxAmt
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(lineMult))]
			get
			{
				decimal HistMult = ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, this._LineMult);
				return HistMult * this._ReportTaxAmt;
			}
			set
			{
				this._ReportTaxAmt = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<TaxPeriodForReportShowing,
			 InnerJoin<TaxReportLine,
							On<TaxReportLine.vendorID, Equal<TaxPeriodForReportShowing.vendorID>,
							And<TaxReportLine.tempLine, NotEqual<True>>>,
			 LeftJoin<TaxHistorySum,
					 On<TaxHistorySum.vendorID, Equal<TaxReportLine.vendorID>,
					 And<TaxHistorySum.lineNbr, Equal<TaxReportLine.lineNbr>,
					 And<TaxHistorySum.taxPeriodID, Equal<TaxPeriodForReportShowing.taxPeriodID>>>>>>,
	 Where<TaxReportLine.tempLineNbr, IsNull, Or<TaxHistorySum.vendorID, IsNotNull>>>))]
	[PXCacheName(Messages.TaxReportSummary)]
	public partial class TaxReportSummary : PX.Data.IBqlTable
	{
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxReportLine.lineNbr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region LineMult
		public abstract class lineMult : PX.Data.IBqlField
		{
		}
		protected Int16? _LineMult;
		[PXDBShort(BqlField = typeof(TaxReportLine.lineMult))]
		public virtual Int16? LineMult
		{
			get
			{
				return this._LineMult;
			}
			set
			{
				this._LineMult = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected string _LineType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxReportLine.lineType))]
		public virtual string LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(BqlField = typeof(TaxPeriodForReportShowing.taxPeriodID))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(TaxReportLine.vendorID))]
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
		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected Int32? _RevisionID;
		[PXDBInt(IsKey = true, BqlTable = typeof(TaxHistorySum))]
		[PXDefault()]
		[PXUIField(DisplayName = "Revision ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Int32? RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(TaxHistorySum.branchID), IsKey = true)]
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
		#region FiledAmt
		public abstract class filedAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _FiledAmt;

		[PXDBBaseCury(BqlTable = typeof(TaxHistorySum))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? FiledAmt
		{
			get
			{
				return this._FiledAmt;
			}
			set
			{
				this._FiledAmt = value;
			}
		}
		#endregion
		#region UnfiledAmt
		public abstract class unfiledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnfiledAmt;
		[PXDBBaseCury(BqlTable = typeof(TaxHistorySum))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? UnfiledAmt
		{
			get
			{
				return this._UnfiledAmt;
			}
			set
			{
				this._UnfiledAmt = value;
			}
		}
		#endregion
		#region ReportFiledAmt
		public abstract class reportFiledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReportFiledAmt;

		[PXDBVendorCury(typeof(TaxReportSummary.vendorID), BqlTable = typeof(TaxHistorySum))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? ReportFiledAmt
		{
			get
			{
				return this._ReportFiledAmt;
			}
			set
			{
				this._ReportFiledAmt = value;
			}
		}
		#endregion
		#region ReportUnfiledAmt
		public abstract class reportUnfiledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReportUnfiledAmt;
		[PXDBVendorCury(typeof(TaxReportSummary.vendorID), BqlTable = typeof(TaxHistorySum))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? ReportUnfiledAmt
		{
			get
			{
				return this._ReportUnfiledAmt;
			}
			set
			{
				this._ReportUnfiledAmt = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<TaxPeriodEffective,
	 InnerJoin<TaxReportLine, On<TaxReportLine.vendorID, Equal<TaxPeriodEffective.vendorID>>,
	 InnerJoin<TaxBucketLine, On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>, And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>>>,
	 CrossJoin<Company,
	 InnerJoin<Vendor,
			 On<Vendor.bAccountID, Equal<TaxReportLine.vendorID>>,
	 LeftJoin<TaxTran,
			 On<TaxTran.vendorID, Equal<TaxBucketLine.vendorID>,
			 And<TaxTran.taxBucketID, Equal<TaxBucketLine.bucketID>,
			 And2<Where<TaxReportLine.taxZoneID, IsNull,
					 And<TaxReportLine.tempLine, Equal<False>,
							 Or<TaxReportLine.taxZoneID, Equal<TaxTran.taxZoneID>>>>,
			 And<Where<TaxTran.taxPeriodID, IsNull, And<TaxTran.origRefNbr, Equal<Empty>,
					 And<TaxTran.released, Equal<True>,
					 And<TaxTran.voided, Equal<False>,
			 And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
			 And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
			 And<TaxTran.tranDate, Less<TaxPeriodEffective.endDate>,
			 And<TaxPeriodEffective.status, Equal<TaxPeriodStatus.open>,
			 Or<TaxTran.taxPeriodID, Equal<TaxPeriodEffective.taxPeriodID>>>>>>>>>>>>>>,
	 LeftJoin<CurrencyInfo,
			 On<CurrencyInfo.curyInfoID, Equal<TaxTran.curyInfoID>>,
	 LeftJoin<CurrencyRateByDate,
			 On<CurrencyRateByDate.fromCuryID, Equal<CurrencyInfo.curyID>,
			 And<CurrencyRateByDate.toCuryID, Equal<Vendor.curyID>,
			 And<CurrencyRateByDate.curyRateType, Equal<Vendor.curyRateTypeID>,
			 And<CurrencyRateByDate.curyEffDate, LessEqual<TaxTran.tranDate>,
					 And<Where<CurrencyRateByDate.nextEffDate, Greater<TaxTran.tranDate>,
									 Or<CurrencyRateByDate.nextEffDate, IsNull>>>
>>>>>>>>>>>>))]

	[PXCacheName(Messages.TaxDetailReportCurrency)]
	public partial class TaxDetailReportCurrency : PX.Data.IBqlTable
	{
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxReportLine.lineNbr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region LineMult
		public abstract class lineMult : PX.Data.IBqlField
		{
		}
		protected Int16? _LineMult;
		[PXDBShort(BqlField = typeof(TaxReportLine.lineMult))]
		public virtual Int16? LineMult
		{
			get
			{
				return this._LineMult;
			}
			set
			{
				this._LineMult = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected string _LineType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxReportLine.lineType))]
		public virtual string LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.module))]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.tranType))]
		[TaxAdjustmentType.List()]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, BqlField = typeof(TaxTran.refNbr))]
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
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxTran.recordID))]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool(BqlField = typeof(TaxTran.released))]
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool(BqlField = typeof(TaxTran.voided))]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(BqlField = typeof(TaxPeriodEffective.taxPeriodID))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected string _TaxID;
		[PXDBString(Tax.taxID.Length, IsKey = true, BqlField = typeof(TaxTran.taxID))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(TaxTran.vendorID))]
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(TaxTran.branchID))]
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
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, BqlField = typeof(TaxTran.taxZoneID))]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(null, BqlField = typeof(TaxTran.accountID))]
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
		[SubAccount(typeof(TaxTran.accountID), BqlField = typeof(TaxTran.subID))]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate(BqlField = typeof(TaxTran.tranDate))]
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
		#region TaxType
		public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxTran.taxType))]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxRate
		public abstract class taxRate : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxRate;
		[PXDBDecimal(6, BqlField = typeof(TaxTran.taxRate))]
		public virtual Decimal? TaxRate
		{
			get
			{
				return this._TaxRate;
			}
			set
			{
				this._TaxRate = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxableAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxableAmt))]
		public virtual decimal? TaxableAmt
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(lineMult))]
			get
			{
				decimal HistMult = ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, this._LineMult);
				return HistMult * this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
		#region TaxAmt
		public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxAmt))]
		public virtual decimal? TaxAmt
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(lineMult))]
			get
			{
				decimal HistMult = ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, this._LineMult);
				return HistMult * this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;
			}
		}
		#endregion
		#region VendorCuryID
		public abstract class vendorCuryID : PX.Data.IBqlField
		{
		}
		protected String _VendorCuryID;
		[PXDBString(5, BqlField = typeof(Vendor.curyID))]
		public virtual String VendorCuryID
		{
			get
			{
				return this._VendorCuryID;
			}
			set
			{
				this._VendorCuryID = value;
			}
		}
		#endregion
		#region VendorCuryRateType
		public abstract class vendorCuryRateType : PX.Data.IBqlField
		{
		}
		protected String _VendorCuryRateType;
		[PXDBString(6, BqlField = typeof(Vendor.curyRateTypeID))]
		public virtual String VendorCuryRateType
		{
			get
			{
				return this._VendorCuryRateType;
			}
			set
			{
				this._VendorCuryRateType = value;
			}
		}
		#endregion
		#region CuryRate
		public abstract class curyRate : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryRate;
		[PXDBDecimal(6, BqlField = typeof(CurrencyRateByDate.curyRate))]
		public virtual Decimal? CuryRate
		{
			get
			{
				return this._CuryRate;
			}
			set
			{
				this._CuryRate = value;
			}
		}
		#endregion
		#region CuryMultDiv
		public abstract class curyMultDiv : PX.Data.IBqlField
		{
		}
		protected String _CuryMultDiv;
		[PXDBString(1, BqlField = typeof(CurrencyRateByDate.curyMultDiv))]
		public virtual String CuryMultDiv
		{
			get
			{
				return this._CuryMultDiv;
			}
			set
			{
				this._CuryMultDiv = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<TaxTran,
	InnerJoin<TaxBucket, On<TaxBucket.vendorID, Equal<TaxTran.vendorID>, And<TaxBucket.bucketID, Equal<TaxTran.taxBucketID>>>,
	LeftJoin<TaxPeriodEffective,
			On<TaxPeriodEffective.vendorID, Equal<TaxTran.vendorID>,
					And<Where<TaxTran.taxPeriodID, IsNull, And<TaxTran.origRefNbr, Equal<Empty>,
							And<TaxTran.released, Equal<True>,
							And<TaxTran.voided, Equal<False>,
							And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
							And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
							And<TaxTran.tranDate, Less<TaxPeriodEffective.endDate>,
							And<TaxPeriodEffective.status, Equal<TaxPeriodStatus.open>,
									Or<TaxTran.taxPeriodID, Equal<TaxPeriodEffective.taxPeriodID>>>>>>>>>>>>,
	InnerJoin<Vendor,
			On<Vendor.bAccountID, Equal<TaxTran.vendorID>>,
	LeftJoin<CurrencyInfo,
			On<CurrencyInfo.curyInfoID, Equal<TaxTran.curyInfoID>>,
	LeftJoin<CurrencyRateByDate,
			On<CurrencyRateByDate.fromCuryID, Equal<CurrencyInfo.curyID>,
					And<CurrencyRateByDate.toCuryID, Equal<Vendor.curyID>,
					And<CurrencyRateByDate.curyRateType, Equal<Vendor.curyRateTypeID>,
					And<CurrencyRateByDate.curyEffDate, LessEqual<TaxTran.tranDate>,
							And<Where<CurrencyRateByDate.nextEffDate, Greater<TaxTran.tranDate>,
											Or<CurrencyRateByDate.nextEffDate, IsNull>>>>>>>
											>>>>>>))]
    
    [PXHidden]
	public partial class TaxDetailByGLReportCurrency : PX.Data.IBqlTable
	{

		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.module))]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.tranType))]
		[TaxAdjustmentType.List()]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.refNbr))]
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
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxTran.recordID))]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool(BqlField = typeof(TaxTran.released))]
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool(BqlField = typeof(TaxTran.voided))]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(BqlField = typeof(TaxPeriodEffective.taxPeriodID))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected string _TaxID;
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.taxID))]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(TaxTran.vendorID))]
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(TaxTran.branchID))]
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
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(TaxTran.taxZoneID))]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(null, BqlField = typeof(TaxTran.accountID))]
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
		[SubAccount(typeof(TaxTran.accountID), BqlField = typeof(TaxTran.subID))]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate(BqlField = typeof(TaxTran.tranDate))]
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
		#region TaxType
		public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxTran.taxType))]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxRate
		public abstract class taxRate : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxRate;
		[PXDBDecimal(6, BqlField = typeof(TaxTran.taxRate))]
		public virtual Decimal? TaxRate
		{
			get
			{
				return this._TaxRate;
			}
			set
			{
				this._TaxRate = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxableAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxableAmt))]
		public virtual decimal? TaxableAmt
		{
			get
			{
				return this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
		#region TaxAmt
		public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxAmt))]
		public virtual decimal? TaxAmt
		{
			get
			{
				return this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;
			}
		}
		#endregion
		#region TaxableAmtIO
		public abstract class taxableAmtIO : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxableAmtIO;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxableAmt))]
		public virtual decimal? TaxableAmtIO
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(taxableAmt))]
			get
			{
				return ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, 1) * this._TaxableAmt;
			}
			set
			{
				this._TaxableAmtIO = value;
			}
		}
		#endregion
		#region TaxAmtIO
		public abstract class taxAmtIO : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmtIO;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxAmt))]
		public virtual decimal? TaxAmtIO
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(taxAmt))]
			get
			{
				return ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, 1) * this._TaxAmt;
			}
			set
			{
				this._TaxAmtIO = value;
			}
		}
		#endregion
		#region VendorCuryID
		public abstract class vendorCuryID : PX.Data.IBqlField
		{
		}
		protected String _VendorCuryID;
		[PXDBString(5, BqlField = typeof(Vendor.curyID))]
		public virtual String VendorCuryID
		{
			get
			{
				return this._VendorCuryID;
			}
			set
			{
				this._VendorCuryID = value;
			}
		}
		#endregion
		#region VendorCuryRateType
		public abstract class vendorCuryRateType : PX.Data.IBqlField
		{
		}
		protected String _VendorCuryRateType;
		[PXDBString(6, BqlField = typeof(Vendor.curyRateTypeID))]
		public virtual String VendorCuryRateType
		{
			get
			{
				return this._VendorCuryRateType;
			}
			set
			{
				this._VendorCuryRateType = value;
			}
		}
		#endregion
		#region CuryRate
		public abstract class curyRate : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryRate;
		[PXDBDecimal(6, BqlField = typeof(CurrencyRateByDate.curyRate))]
		public virtual Decimal? CuryRate
		{
			get
			{
				return this._CuryRate;
			}
			set
			{
				this._CuryRate = value;
			}
		}
		#endregion
		#region CuryMultDiv
		public abstract class curyMultDiv : PX.Data.IBqlField
		{
		}
		protected String _CuryMultDiv;
		[PXDBString(1, BqlField = typeof(CurrencyRateByDate.curyMultDiv))]
		public virtual String CuryMultDiv
		{
			get
			{
				return this._CuryMultDiv;
			}
			set
			{
				this._CuryMultDiv = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<TaxTran,
		InnerJoin<TaxBucket, On<TaxBucket.vendorID, Equal<TaxTran.vendorID>, And<TaxBucket.bucketID, Equal<TaxTran.taxBucketID>>>,
		LeftJoin<TaxPeriodEffective,
			On<TaxPeriodEffective.vendorID, Equal<TaxTran.vendorID>,
				And<Where<TaxTran.taxPeriodID, IsNull, 
								And<TaxTran.origRefNbr, Equal<Empty>,
								And<TaxTran.released, Equal<True>, 
								And<TaxTran.voided, Equal<False>,
								And<TaxTran.taxType, NotEqual<TaxType.pendingSales>, 
								And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>, 
								And<TaxTran.tranDate, Less<TaxPeriodEffective.endDate>, 
								And<TaxPeriodEffective.status, Equal<TaxPeriodStatus.open>, 
								Or<TaxTran.taxPeriodID, Equal<TaxPeriodEffective.taxPeriodID>>>>>>>>>>>>>>>))]
	[PXCacheName(Messages.TaxDetailReport)]
	public partial class TaxDetailByGLReport : PX.Data.IBqlTable
	{

		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.module))]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(TaxTran.tranType))]
		[TaxAdjustmentType.List()]
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
		#region TranTypeInvoiceDiscriminated
		public abstract class tranTypeInvoiceDiscriminated : PX.Data.IBqlField
		{
		}
		protected String _TranTypeInvoiceDiscriminated;

		[PXString]
		[PXDBCalced(typeof(Switch<
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAP>,
					And<TaxTran.tranType, Equal<APDocType.invoice>>>,
				TaxTranType.apInvoice,
			Case<
				Where<TaxTran.module, Equal<BatchModule.moduleAR>,
					And<TaxTran.tranType, Equal<ARDocType.invoice>>>,
				TaxTranType.arInvoice>>,
			TaxTran.tranType>),
			typeof(string))]
		[TaxTranType.List()]
		public virtual String TranTypeInvoiceDiscriminated
		{
			get
			{
				return this._TranTypeInvoiceDiscriminated;
			}
			set
			{
				this._TranTypeInvoiceDiscriminated = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.refNbr))]
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
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBInt(IsKey = true, BqlField = typeof(TaxTran.recordID))]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool(BqlField = typeof(TaxTran.released))]
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool(BqlField = typeof(TaxTran.voided))]
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
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;
		[GL.FinPeriodID(BqlField = typeof(TaxPeriodEffective.taxPeriodID))]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected string _TaxID;
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true, BqlField = typeof(TaxTran.taxID))]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(TaxTran.vendorID))]
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(TaxTran.branchID))]
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
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(TaxTran.taxZoneID))]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(null, BqlField = typeof(TaxTran.accountID))]
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
		[SubAccount(typeof(TaxTran.accountID), BqlField = typeof(TaxTran.subID))]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate(BqlField = typeof(TaxTran.tranDate))]
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
		#region TaxType
		public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(TaxTran.taxType))]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxRate
		public abstract class taxRate : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxRate;
		[PXDBDecimal(6, BqlField = typeof(TaxTran.taxRate))]
		public virtual Decimal? TaxRate
		{
			get
			{
				return this._TaxRate;
			}
			set
			{
				this._TaxRate = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxableAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxableAmt))]
		public virtual decimal? TaxableAmt
		{
			get
			{
				return this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
		#region TaxAmt
		public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmt;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxAmt))]
		public virtual decimal? TaxAmt
		{
			get
			{
				return this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;
			}
		}
		#endregion
		#region TaxableAmtIO
		public abstract class taxableAmtIO : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxableAmtIO;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxableAmt))]
		public virtual decimal? TaxableAmtIO
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(taxableAmt))]
			get
			{
				return ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, 1) * this._TaxableAmt;
			}
			set
			{
				this._TaxableAmtIO = value;
			}
		}
		#endregion
		#region TaxAmtIO
		public abstract class taxAmtIO : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmtIO;
		[PXDBBaseCury(BqlField = typeof(TaxTran.taxAmt))]
		public virtual decimal? TaxAmtIO
		{
			[PXDependsOnFields(typeof(module), typeof(tranType), typeof(taxType), typeof(taxAmt))]
			get
			{
				return ReportTaxProcess.GetMult(this._Module, this._TranType, this._TaxType, 1) * this._TaxAmt;
			}
			set
			{
				this._TaxAmtIO = value;
			}
		}
		#endregion
	}


	[PX.Objects.GL.TableAndChartDashboardType]
	public class ReportTaxDetail : PXGraph<ReportTaxDetail>
	{
		public PXSelect<BAccount> dummy_baccount;
		public PXCancel<TaxHistoryMaster> Cancel;
		public PXFilter<TaxHistoryMaster> History_Header;

		public PXSelect<TaxPeriod,
			Where<TaxPeriod.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
				And<TaxPeriod.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>>>> Current;
		public PXSelect<TaxReportLine, Where<TaxReportLine.vendorID, Equal<Current<TaxHistoryMaster.vendorID>>, And<TaxReportLine.lineNbr, Equal<Current<TaxHistoryMaster.lineNbr>>>>> TaxReportLine_Select;
		[PXFilterable]
		public PXSelectJoin<TaxTranReport,
			LeftJoin<TaxBucketLine,
				On<TaxBucketLine.vendorID, Equal<TaxTranReport.vendorID>, And<TaxBucketLine.bucketID, Equal<TaxTranReport.taxBucketID>>>>,
			Where<boolFalse, Equal<boolTrue>>> History_Detail;
		public PXSelectJoin<TaxReportLine,
			InnerJoin<TaxBucketLine,
						 On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>,
						And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>>>,
		  InnerJoin<TaxTranReport,
						 On<TaxTranReport.vendorID, Equal<TaxBucketLine.vendorID>,
						And<TaxTranReport.taxBucketID, Equal<TaxBucketLine.bucketID>,
						And<Where<TaxReportLine.taxZoneID, IsNull,
									And<TaxReportLine.tempLine, Equal<boolFalse>,
									 Or<TaxReportLine.taxZoneID, Equal<TaxTranReport.taxZoneID>>>>>>>,
			LeftJoin<BAccount,
						On<BAccount.bAccountID, Equal<TaxTranReport.bAccountID>>>>>,
			Where<TaxReportLine.vendorID, Equal<Current<TaxHistoryMaster.vendorID>>,
				And<TaxReportLine.lineNbr, Equal<Current<TaxHistoryMaster.lineNbr>>,
				And<TaxTranReport.taxPeriodID, Equal<Current<TaxHistoryMaster.taxPeriodID>>,
				And<TaxTranReport.released, Equal<boolTrue>,
				And<TaxTranReport.voided, Equal<boolFalse>>>>>>> History_Detail_Expanded;

		public PXAction<TaxHistoryMaster> viewBatch;
		public PXAction<TaxHistoryMaster> viewDocument;

		public TaxReportLine taxReportLine
		{
			get
			{
				return TaxReportLine_Select.Select();
			}
		}

		public ReportTaxDetail()
		{
			if (Company.Current.BAccountID.HasValue == false)
			{
				throw new PXSetupNotEnteredException(ErrorMessages.SetupNotEntered, typeof(Branch), CS.Messages.BranchMaint);
			}

			History_Detail.Cache.AllowInsert = false;
			History_Detail.Cache.AllowUpdate = false;
			History_Detail.Cache.AllowDelete = false;
		}

		public PXSetup<Branch> Company;

		public virtual IEnumerable history_Detail()
		{
			using (new PXReadBranchRestrictedScope(History_Header.Current.BranchID))
			foreach (PXResult<TaxReportLine, TaxBucketLine, TaxTranReport, BAccount> res in History_Detail_Expanded.Select(History_Header.Current.BranchID))
			{
				TaxBucketLine line = res;
				TaxTranReport tran = (TaxTranReport)this.Caches[typeof(TaxTranReport)].CreateCopy((TaxTranReport)res);
				BAccount baccount = res;

				decimal HistMult = 0m;

				if (TaxReportLine_Select.Current != null)
				{
					HistMult = ReportTaxProcess.GetMult(tran.Module, tran.TranType, tran.TaxType, TaxReportLine_Select.Current.LineMult);
				}

				tran.ReportTaxAmt = HistMult * tran.ReportTaxAmt;
				tran.ReportTaxableAmt = HistMult * tran.ReportTaxableAmt;

				yield return new PXResult<TaxTranReport, TaxBucketLine, BAccount>(tran, line, baccount);
			}
		}

		protected virtual void TaxHistoryMaster_BranchID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
			e.NewValue = branches != null ? (int?)branches[0] : null;
			e.Cancel = branches != null;
		}

		protected virtual void TaxHistoryMaster_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			int? firstLine = null;
			if (History_Header.Current != null && TaxReportLine_Select.Current != null && History_Header.Current.LineNbr != TaxReportLine_Select.Current.LineNbr)
			{
				TaxReportLine_Select.Current = null;
			}

			if (History_Header.Current != null && History_Header.Current.VendorID != null)
			{
				List<int> AllowedValues = new List<int>();
				List<string> AllowedLabels = new List<string>();

				foreach (TaxReportLine line in PXSelectReadonly<TaxReportLine, Where<TaxReportLine.vendorID, Equal<Current<TaxHistoryMaster.vendorID>>>>.Select(this, null))
				{
					AllowedValues.Add(line.LineNbr.GetValueOrDefault());
					StringBuilder bld = new StringBuilder();
					bld.Append(line.LineNbr.GetValueOrDefault());
					bld.Append("-");
					bld.Append(line.Descr);
					AllowedLabels.Add(bld.ToString());
				}

				if (AllowedValues.Count > 0)
				{
					firstLine = AllowedValues[0];
					PXIntListAttribute.SetList<TaxHistoryMaster.lineNbr>(History_Header.Cache, null, AllowedValues.ToArray(), AllowedLabels.ToArray());
				}
			}

			if (History_Header.Current != null && History_Header.Current.VendorID != null && string.IsNullOrEmpty(History_Header.Current.TaxPeriodID) == false)
			{
				TaxPeriod per = (TaxPeriod)PXSelectorAttribute.Select<TaxHistoryMaster.taxPeriodID>(History_Header.Cache, History_Header.Current);
				if (per != null)
				{
					History_Header.Current.StartDate = per.StartDate;
					History_Header.Current.EndDate = per.EndDate;
				}
			}
		}

		protected virtual void TaxHistoryMaster_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<TaxHistoryMaster.taxPeriodID>(e.Row);
			sender.SetDefaultExt<TaxHistoryMaster.startDate>(e.Row);
			sender.SetDefaultExt<TaxHistoryMaster.endDate>(e.Row);
		}

		[PXUIField(DisplayName = GL.Messages.ViewBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			TaxTranReport taxtran = History_Detail.Current;
			if (taxtran != null)
			{
				string batchNbr = null;
				switch (taxtran.Module)
				{
					case BatchModule.AP:
						{
							APRegister apreg = PXSelect<APRegister, Where<APRegister.docType, Equal<Current<TaxTranReport.tranType>>,
								And<APRegister.refNbr, Equal<Current<TaxTranReport.refNbr>>>>>.SelectSingleBound(this, new object[] { taxtran });
							batchNbr = apreg?.BatchNbr;
						}
						break;
					case BatchModule.AR:
						{
							ARRegister arreg = PXSelect<ARRegister, Where<ARRegister.docType, Equal<Current<TaxTranReport.tranType>>,
								And<ARRegister.refNbr, Equal<Current<TaxTranReport.refNbr>>>>>.SelectSingleBound(this, new object[] { taxtran });
							batchNbr = arreg?.BatchNbr;
						}
						break;
					case BatchModule.GL:
						{
							if (taxtran.TranType == TaxTran.tranType.TranForward ||
								taxtran.TranType == TaxTran.tranType.TranReversed)
							{
								batchNbr = taxtran.RefNbr;
							}
							else if ((taxtran.TranType == TaxAdjustmentType.OutputVAT ||
								taxtran.TranType == TaxAdjustmentType.InputVAT) &&
								!string.IsNullOrEmpty(taxtran.TaxInvoiceNbr) &&
								taxtran.TaxInvoiceDate != null)
							{
								SVATConversionHist docSVAT = PXSelect<SVATConversionHist,
									Where<SVATConversionHist.taxRecordID, Equal<Current<TaxTranReport.recordID>>,
										And<SVATConversionHist.processed, Equal<True>>>>.SelectSingleBound(this, new object[] { taxtran });
								batchNbr = docSVAT?.AdjBatchNbr;
							}
							else
							{
								TaxAdjustmentEntry graph = PXGraph.CreateInstance<TaxAdjustmentEntry>();
								TaxAdjustment taxadj = graph.Document.Search<TaxAdjustment.refNbr>(taxtran.RefNbr, taxtran.TranType);
								batchNbr = taxadj?.BatchNbr;
							}
						}
						break;
					case BatchModule.CA:
						{
							CATranEntry graph = PXGraph.CreateInstance<CATranEntry>();
							CAAdj caadj = graph.CAAdjRecords.Search<CAAdj.adjRefNbr>(taxtran.RefNbr);
							if (caadj != null)
							{
								batchNbr = (string)graph.CAAdjRecords.Cache.GetValue<CAAdj.tranID_CATran_batchNbr>(caadj);
							}
						}
						break;
				}

				if (!string.IsNullOrEmpty(batchNbr))
				{
					Batch batch = JournalEntry.FindBatch(this, taxtran.Module, batchNbr);
					if (batch != null)
					{
						JournalEntry.RedirectToBatch(batch);
					}
				}
			}

			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			TaxTranReport taxtran = History_Detail.Current;
			if (taxtran != null)
			{
				switch (taxtran.Module)
				{
					case BatchModule.AP:
						{
							APDocGraphCreator apDocGraphCreator = new APDocGraphCreator();
							PXGraph apDocGraph = apDocGraphCreator.Create(taxtran.TranType, taxtran.RefNbr, null);
							throw new PXRedirectRequiredException(apDocGraph, true, Messages.Document){ Mode = PXBaseRedirectException.WindowMode.NewWindow };
						}
					case BatchModule.AR:
						{
							ARDocGraphCreator arDocGraphCreator = new ARDocGraphCreator();
							PXGraph arDocGraph = arDocGraphCreator.Create(taxtran.TranType, taxtran.RefNbr, null);
							throw new PXRedirectRequiredException(arDocGraph, true, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
							}
					case BatchModule.GL:
						{
							if (taxtran.TranType == TaxTran.tranType.TranForward ||
								taxtran.TranType == TaxTran.tranType.TranReversed)
							{
								Batch batch = JournalEntry.FindBatch(this, taxtran.Module, taxtran.RefNbr);
								if (batch != null)
								{
									JournalEntry.RedirectToBatch(batch);
								}
							}
							else if ((taxtran.TranType == TaxAdjustmentType.OutputVAT ||
								taxtran.TranType == TaxAdjustmentType.InputVAT) &&
								!string.IsNullOrEmpty(taxtran.TaxInvoiceNbr) &&
								taxtran.TaxInvoiceDate != null)
							{
								ProcessSVATBase graph = PXGraph.CreateInstance<ProcessSVATBase>();
								SVATConversionHist docSVAT = PXSelect<SVATConversionHist, 
									Where<SVATConversionHist.taxRecordID, Equal<Required<SVATConversionHist.taxRecordID>>,
										And<SVATConversionHist.processed, Equal<True>>>>.SelectSingleBound(graph, null, taxtran.RecordID);
								if (docSVAT != null)
								{
									PXRedirectHelper.TryRedirect(graph.SVATDocuments.Cache, docSVAT, Messages.Document, PXRedirectHelper.WindowMode.NewWindow);
								}
							}
							else
							{
								TaxAdjustmentEntry graph = PXGraph.CreateInstance<TaxAdjustmentEntry>();
								TaxAdjustment apdoc = graph.Document.Search<TaxAdjustment.refNbr>(taxtran.RefNbr, taxtran.TranType);
								if (apdoc != null)
								{
									graph.Document.Current = apdoc;
									throw new PXRedirectRequiredException(graph, true, Messages.Document){ Mode = PXBaseRedirectException.WindowMode.NewWindow };
								}
							}
						}
						break;
					case BatchModule.CA:
						{
							CATranEntry graph = PXGraph.CreateInstance<CATranEntry>();
							CAAdj apdoc = graph.CAAdjRecords.Search<CAAdj.adjRefNbr>(taxtran.RefNbr);
							if (apdoc != null)
							{
								graph.CAAdjRecords.Current = apdoc;
								throw new PXRedirectRequiredException(graph, true, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
							}
						}
						break;
				}
			}
			return History_Header.Select();
		}
	}
	
	public static class TaxRevisionManager
	{
		public static int? CurrentRevisionId(PXGraph graph, int? vendorId, String taxperiodID)
		{
			var result = (PXResult<TaxPeriod, TaxHistory>)PXSelectJoin<TaxPeriod,
				LeftJoin<TaxHistory, 
			         On<TaxHistory.vendorID, Equal<TaxPeriod.vendorID>,
							And<TaxHistory.taxPeriodID, Equal<TaxPeriod.taxPeriodID>>>>,
				Where<TaxPeriod.vendorID, Equal<Required<TaxHistory.vendorID>>,
					And<TaxPeriod.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>>>, 
				OrderBy<Desc<TaxHistory.revisionID>>>
				.Select(graph, vendorId, taxperiodID);

			if (result == null) return null;

			TaxPeriod period = result;
			TaxHistory search = result;


			return search.RevisionID ?? (period.Status != TaxPeriodStatus.Open ? (int?)1 : null);
		}

		public static int NewRevisionID(PXGraph graph, int? vendorId, String taxperiodID)
		{
			var result = CurrentRevisionId(graph, vendorId, taxperiodID);
			return result == null ? 1 : result.Value + 1;
		}

		public static bool HasRevisions(PXGraph graph, int? vendorId, String taxperiodID)
		{
			return CurrentRevisionId(graph, vendorId, taxperiodID) != null;
		}
	}

	[PX.Objects.GL.TableAndChartDashboardType]
	public class ReportTaxReview : PXGraph<ReportTax>
	{
		public PXFilter<TaxPeriodFilter> Period_Header;
		public PXCancel<TaxPeriodFilter> Cancel;

		public PXSetup<TaxPeriod,
				Where<TaxPeriod.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
					And<TaxPeriod.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>>>> Period;

		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Current<TaxPeriodFilter.vendorID>>>> Vendor;
        public PXSelectJoin<TaxReportLine,
            LeftJoin<TaxHistory, On<TaxHistory.vendorID, Equal<TaxReportLine.vendorID>, And<TaxHistory.lineNbr, Equal<TaxReportLine.lineNbr>>>>,
            Where<boolFalse, Equal<boolTrue>>> Period_Details;

		public PXSelectJoinGroupBy<TaxReportLine,
						 LeftJoin<TaxHistory,
									 On<TaxHistory.vendorID, Equal<TaxReportLine.vendorID>,
									And<TaxHistory.lineNbr, Equal<TaxReportLine.lineNbr>,
									And<TaxHistory.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>,
									And2<Where<Current<TaxPeriodFilter.showDifference>, NotEqual<boolTrue>,
												 Or<TaxHistory.revisionID, Equal<Current<TaxPeriodFilter.revisionId>>>>,
									And<Where<Current<TaxPeriodFilter.showDifference>, Equal<boolTrue>,
													Or<TaxHistory.revisionID, LessEqual<Current<TaxPeriodFilter.revisionId>>>>>>>>>>,
						Where<TaxReportLine.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
							And<TaxReportLine.tempLine, Equal<False>,
                            And2<Where<TaxReportLine.tempLineNbr, IsNull, Or<TaxHistory.vendorID, IsNotNull>>
                            , And<Where<TaxReportLine.hideReportLine, IsNull, Or<TaxReportLine.hideReportLine, Equal<False>>>>>>>,
						Aggregate<GroupBy<TaxReportLine.lineNbr, Sum<TaxHistory.filedAmt, Sum<TaxHistory.reportFiledAmt>>>>> Period_Details_Expanded;

		[PXFilterable]
		public PXSelect<APInvoice,
		Where<APInvoice.docDate, GreaterEqual<Current<TaxPeriodFilter.startDate>>,
			And<APInvoice.docDate, LessEqual<Current<TaxPeriodFilter.endDate>>,
			And<APInvoice.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
			And<Where<APInvoice.docType, Equal<APDocType.invoice>,
			 Or<APInvoice.docType, Equal<APDocType.debitAdj>>>>>>>> APDocuments;
		
		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}
		protected virtual void TaxPeriodFilter_BranchID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
			e.NewValue = branches != null ? (int?)branches[0] : null;
			e.Cancel = branches != null;
		}

		protected virtual void TaxPeriodFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			TaxPeriodFilter filter = e.Row as TaxPeriodFilter;
            if (filter == null) return;

            bool OpenPeriod = Period.Current != null && Period.Current.Status == "P";

			bool openRevision = OpenPeriod;
            int? maxRevision = TaxRevisionManager.CurrentRevisionId(sender.Graph, filter.VendorID, filter.TaxPeriodID);
			if (maxRevision != null)
			{
				if (maxRevision.Value != filter.RevisionId)
					openRevision = false;
			}
			filter.StartDate = Period.Current?.StartDate;
			filter.EndDate = Period.Current?.EndDate != null
                ? (DateTime?)(((DateTime)Period.Current.EndDate).AddDays(-1))
				: null;

			PXUIFieldAttribute.SetEnabled<TaxPeriodFilter.revisionId>(sender, null, maxRevision > 1);
			PXUIFieldAttribute.SetVisible<TaxPeriodFilter.showDifference>(sender, null, maxRevision > 1 && filter.RevisionId > 1);
			PXUIFieldAttribute.SetEnabled<TaxPeriodFilter.taxPeriodID>(sender, null, true);
			voidReport.SetEnabled(openRevision);
			adjustTax.SetEnabled(OpenPeriod);
			closePeriod.SetEnabled(openRevision);
			if (!string.IsNullOrEmpty(filter.PreparedWarningMsg))
			{
				sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(filter, filter.TaxPeriodID, 
					new PXSetPropertyException(filter.PreparedWarningMsg, PXErrorLevel.Warning));
			}

			if (ReportTaxProcess.CheckForUnprocessedSVAT(this, filter.BranchID, Vendor.Current, Period.Current?.EndDate))
			{
				sender.RaiseExceptionHandling<TaxPeriodFilter.vendorID>(e.Row, filter.VendorID,
					new PXSetPropertyException(Messages.TaxReportHasUnprocessedSVAT, PXErrorLevel.Warning));
			}
		}
		protected virtual void TaxPeriodFilter_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetValue<TaxPeriodFilter.taxPeriodID>(e.Row, null);
		}

		protected virtual void TaxPeriodFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			TaxPeriodFilter filter = (TaxPeriodFilter)e.Row;
			if (filter == null) return;

			if (!sender.ObjectsEqual<TaxPeriodFilter.branchID>(e.Row, e.OldRow))
			{
				List<PXView> views = this.Views.Select(view => view.Value).ToList();
				foreach (var view in views) view.Clear();
			}

			if (!sender.ObjectsEqual<TaxPeriodFilter.vendorID>(e.Row, e.OldRow))
			{
				TaxPeriod taxper = PXSelect<TaxPeriod,
					Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
						And<TaxPeriod.status, Equal<TaxPeriodStatus.prepared>>>,
					OrderBy<Asc<TaxPeriod.taxPeriodID>>>
					.SelectWindowed(this, 0, 1, filter.VendorID);
				if (taxper != null)
				{
					filter.TaxPeriodID = taxper.TaxPeriodID;
				}
				else
				{
					taxper = PXSelect<TaxPeriod,
						Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
							And<TaxPeriod.status, Equal<TaxPeriodStatus.closed>>>,
						OrderBy<Desc<TaxPeriod.taxPeriodID>>>
						.SelectWindowed(this, 0, 1, filter.VendorID);
					filter.TaxPeriodID = taxper != null ? taxper.TaxPeriodID : null;
				}
			}

			if (!sender.ObjectsEqual<TaxPeriodFilter.vendorID>(e.Row, e.OldRow) || 
				  !sender.ObjectsEqual<TaxPeriodFilter.taxPeriodID>(e.Row, e.OldRow))
			{
				filter.RevisionId = TaxRevisionManager.CurrentRevisionId(this, filter.VendorID, filter.TaxPeriodID);
			}
		}

		protected virtual IEnumerable period_Details()
		{
			using (new PXReadBranchRestrictedScope(Period_Header.Current.BranchID))
			{
				return Period_Details_Expanded.Select().Where(line => ShowTaxReportLine(line.GetItem<TaxReportLine>(), Period_Header.Current.TaxPeriodID));				
			}
		}

		public virtual bool ShowTaxReportLine(TaxReportLine taxReportLine, string taxPeriodID)
		{
			return true;
		}

		public PXAction<TaxPeriodFilter> adjustTax;
		[PXUIField(DisplayName = Messages.AdjustTax, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable AdjustTax(PXAdapter adapter)
		{
			TaxAdjustmentEntry graph = PXGraph.CreateInstance<TaxAdjustmentEntry>();
			graph.Clear();

			TaxAdjustment newDoc = graph.Document.Insert(new TaxAdjustment());
			newDoc.VendorID = this.Period_Header.Current.VendorID;
			graph.Document.Cache.RaiseFieldUpdated<TaxAdjustment.vendorID>(newDoc, null);

			throw new PXRedirectRequiredException(graph, Messages.NewAdjustment);
		}

		public PXAction<TaxPeriodFilter> voidReport;
		[PXUIField(DisplayName = Messages.VoidReport, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable VoidReport(PXAdapter adapter)
		{
			TaxPeriodFilter tp = Period_Header.Current;
			PXLongOperation.StartOperation(this, () => ReportTaxReview.VoidReportProc(tp));
			return adapter.Get();
		}

		public PXAction<TaxPeriodFilter> closePeriod;
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable ClosePeriod(PXAdapter adapter)
		{
			TaxPeriodFilter tp = Period_Header.Current;
			PXLongOperation.StartOperation(this, () => ReportTaxReview.ClosePeriodProc(tp));
			return adapter.Get();
		}

		public PXAction<TaxPeriodFilter> viewDocument;
		[PXUIField(DisplayName = Messages.ViewDocuments, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (this.Period_Details.Current != null)
			{
                ReportTaxDetail graph = PXGraph.CreateInstance<ReportTaxDetail>();
				TaxHistoryMaster filter = new TaxHistoryMaster();
				filter.BranchID = Period_Header.Current.BranchID;
				filter.VendorID = Period_Header.Current.VendorID;
				filter.TaxPeriodID = Period_Header.Current.TaxPeriodID;
				filter.LineNbr = Period_Details.Current.LineNbr;
				graph.History_Header.Insert(filter);
				throw new PXRedirectRequiredException(graph, Messages.ViewDocuments);
			}
			return Period_Header.Select();
		}

		public PXAction<TaxPeriodFilter> checkDocument;
		[PXUIField(DisplayName = Messages.Document, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public virtual IEnumerable CheckDocument(PXAdapter adapter)
		{
			if (APDocuments.Current != null)
			{
				APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
				APInvoice apdoc = graph.Document.Search<APInvoice.refNbr>(APDocuments.Current.RefNbr, APDocuments.Current.DocType);
				if (apdoc != null)
				{
					graph.Document.Current = apdoc;
					throw new PXRedirectRequiredException(graph, true, Messages.Document) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}

			}
			return Period_Header.Select();
		}


		public static void VoidReportProc(TaxPeriodFilter p)
		{
			ReportTaxProcess fp = PXGraph.CreateInstance<ReportTaxProcess>();
			fp.VoidReportProc(p);
            ReportTax docgraph = PXGraph.CreateInstance<ReportTax>();
			docgraph.TimeStamp = fp.TimeStamp;
			docgraph.Period_Header.Insert();
			docgraph.Period_Header.Current.VendorID = p.VendorID;
			docgraph.Period_Header.Current.BranchID = p.BranchID;
			docgraph.Period_Header.Update(docgraph.Period_Header.Current);
			throw new PXRedirectRequiredException(docgraph, Messages.Report);
		}

		public static void ClosePeriodProc(TaxPeriodFilter p)
		{
			ReportTaxProcess fp = PXGraph.CreateInstance<ReportTaxProcess>();
			fp.ClosePeriodProc(p);
		}

		public static void ReleaseDoc(List<TaxAdjustment> list)
		{
			ReportTaxProcess rg = PXGraph.CreateInstance<ReportTaxProcess>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			List<Batch> batchlist = new List<Batch>();
			List<int> batchbind = new List<int>();

			list.Sort(new Comparison<TaxAdjustment>(delegate(TaxAdjustment a, TaxAdjustment b)
				{
					object aBranchID = a.BranchID;
					object bBranchID = b.BranchID;
					int ret = ((IComparable)aBranchID).CompareTo(bBranchID);

					if (ret != 0)
					{
						return ret;
					}
                    object aFinPeriodID = a.FinPeriodID;
                    object bFinPeriodID = b.FinPeriodID;
                    ret = ((IComparable)aFinPeriodID).CompareTo(bFinPeriodID);

					return ret;
				}
			));

			for (int i = 0; i < list.Count; i++)
			{
				TaxAdjustment doc = list[i];
				rg.ReleaseDocProc(je, doc);
				rg.Clear();

				if (je.BatchModule.Current != null && !batchlist.Contains(je.BatchModule.Current))
				{
					batchlist.Add(je.BatchModule.Current);
					batchbind.Add(i);
				}
			}

			foreach (Batch batch in batchlist)
			{
				pg.TimeStamp = batch.tstamp;
				pg.PostBatchProc(batch);
				pg.Clear();
			}
		}

		public ReportTaxReview()
		{
			APSetup setup = APSetup.Current;
			PXCache cache = Period_Details.Cache;
			cache.AllowDelete = false;
			cache.AllowInsert = false;
			cache.AllowUpdate = false;
			cache = APDocuments.Cache;
			cache.AllowDelete = false;
			cache.AllowInsert = false;
			cache.AllowUpdate = false;
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}

		public PXSetup<APSetup> APSetup;
	}



	[PX.Objects.GL.TableAndChartDashboardType]
	public class ReportTax : PXGraph<ReportTax>
	{
		public PXFilter<TaxPeriodFilter> Period_Header;
		public PXCancel<TaxPeriodFilter> Cancel; 

		public PXSelect<TaxYear, Where<TaxYear.vendorID, Equal<Required<TaxYear.vendorID>>, And<TaxYear.year, Equal<Required<TaxYear.year>>>>> TaxYear_Current;
		public PXSelect<TaxPeriod,
						Where<TaxPeriod.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
							And<TaxPeriod.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>>>> TaxPeriod_Current;
		public PXSelect<TaxPeriod, Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
				And<TaxPeriod.startDate, LessEqual<Required<TaxPeriod.startDate>>,
				And<TaxPeriod.endDate, Greater<Required<TaxPeriod.endDate>>>>>> TaxPeriod_ByDate;

		[PXFilterable]
        public PXSelectJoin<TaxReportLine,
            LeftJoin<TaxHistory, On<TaxHistory.vendorID, Equal<TaxReportLine.vendorID>, And<TaxHistory.lineNbr, Equal<TaxReportLine.lineNbr>>>>,
            Where<boolFalse, Equal<boolTrue>>> Period_Details;
        public PXSelectJoin<TaxReportLine,
			LeftJoin<TaxBucketLine,
						On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>,
					 And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>>>,
			LeftJoin<TaxRev,
						On<TaxRev.taxVendorID, Equal<TaxBucketLine.vendorID>,
						And<TaxRev.taxBucketID, Equal<TaxBucketLine.bucketID>,
						And<TaxRev.outdated, Equal<boolFalse>>>>,
			LeftJoin<TaxTranReport,
						On<TaxTranReport.taxID, Equal<TaxRev.taxID>,
					 And<TaxTranReport.taxType, Equal<TaxRev.taxType>,
					 And<TaxTranReport.tranDate, Between<TaxRev.startDate, TaxRev.endDate>,
					 And<TaxTranReport.released, Equal<True>,
					 And<TaxTranReport.voided, Equal<False>,
					 And<TaxTranReport.taxPeriodID, IsNull,
					 And<TaxTranReport.origRefNbr, Equal<Empty>,
					 And<TaxTranReport.taxType, NotEqual<TaxType.pendingPurchase>,
					 And<TaxTranReport.taxType, NotEqual<TaxType.pendingSales>,
                     And2<Where<Current<Vendor.taxReportFinPeriod>, Equal<boolTrue>,
                                    Or<TaxTranReport.tranDate, Less<Current<TaxPeriod.endDate>>>>,
                     And2<Where<Current<Vendor.taxReportFinPeriod>, Equal<boolFalse>,
                                    Or<TaxTranReport.finDate, Less<Current<TaxPeriod.endDate>>>>,
					 And<Where<TaxReportLine.taxZoneID, IsNull,
								 And<TaxReportLine.tempLine, Equal<boolFalse>,
									Or<TaxReportLine.taxZoneID, Equal<TaxTranReport.taxZoneID>>>>>>>>>>>>>>>>,
			LeftJoin<Currency,
									 On<Currency.curyID, Equal<Current<Vendor.curyID>>>,
			LeftJoin<CurrencyRate,
								On<CurrencyRate.fromCuryID, Equal<TaxTranReport.curyID>,
									And<CurrencyRate.toCuryID, Equal<Currency.curyID>,
									And<CurrencyRate.curyRateType, Equal<Current<Vendor.curyRateTypeID>>,
									And<CurrencyRate.curyEffDate, Equal<TaxTranReport.tranDate>>>>>,
			LeftJoin<CurrencyRateByDate,
								On<CurrencyRate.fromCuryID, IsNull,
									And<CurrencyRateByDate.fromCuryID, Equal<TaxTranReport.curyID>,
									And<CurrencyRateByDate.toCuryID, Equal<Currency.curyID>,
									And<CurrencyRateByDate.curyRateType, Equal<Current<Vendor.curyRateTypeID>>,
									And2<Where<CurrencyRateByDate.curyEffDate, LessEqual<TaxTranReport.tranDate>,
													Or<CurrencyRateByDate.curyEffDate, IsNull>>,
									And<Where<CurrencyRateByDate.nextEffDate, Greater<TaxTranReport.tranDate>,
									Or<CurrencyRateByDate.nextEffDate, IsNull>>>>>>>>>>>>>>,
			Where<TaxReportLine.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>>,
			OrderBy<Asc<TaxReportLine.vendorID, Asc<TaxReportLine.lineNbr>>>> Period_Details_Expanded;


		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Current<TaxPeriodFilter.vendorID>>>> Vendor;

		public PXSelectJoin<TaxTranReport,
				InnerJoin<Tax, On<Tax.taxID, Equal<TaxTranReport.taxID>>,
				InnerJoin<TaxReportLine, On<TaxReportLine.vendorID, Equal<Tax.taxVendorID>>,
				InnerJoin<TaxBucketLine,
					On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>,
				 And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>,
				 And<TaxBucketLine.bucketID, Equal<TaxTranReport.taxBucketID>>>>>>>,
				Where<Tax.taxVendorID, Equal<Required<TaxPeriodFilter.vendorID>>,
					And<TaxTranReport.released, Equal<True>, 
					And<TaxTranReport.voided, Equal<False>, 
					And<TaxTranReport.taxPeriodID, IsNull,
					And<TaxTranReport.taxType, NotEqual<TaxType.pendingPurchase>,
					And<TaxTranReport.taxType, NotEqual<TaxType.pendingSales>,
					And<TaxTranReport.origRefNbr, Equal<Empty>>>>>>>>,
				OrderBy<Asc<TaxTranReport.tranDate>>> OldestNotReportedTaxTran;

		public PXSelect<TaxHistory,
			Where<TaxHistory.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
				And<TaxHistory.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>>>,
			OrderBy<Desc<TaxHistory.revisionID>>> History_Last;

		public PXSetup<Company> company;

		protected virtual void TaxPeriodFilter_BranchID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			List<int> branches = PXAccess.GetMasterBranchID(Accessinfo.BranchID);
			e.NewValue = branches != null ? (int?)branches[0] : null;
			e.Cancel = branches != null;
		}

		protected virtual void TaxPeriodFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			TaxPeriodFilter filter = (TaxPeriodFilter)e.Row;
			if (filter == null)
				return;

			filter.StartDate = null;
			filter.EndDate = null;
			fileTax.SetEnabled(false);
			sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, filter.TaxPeriodID, null);
			sender.RaiseExceptionHandling<TaxPeriodFilter.vendorID>(e.Row, filter.VendorID, null);
			PXUIFieldAttribute.SetEnabled<TaxPeriodFilter.taxPeriodID>(sender, null, Vendor.Current != null && Vendor.Current.UpdClosedTaxPeriods == true);

			TaxPeriod taxper = TaxPeriod_Current.SelectSingle();
			if (taxper?.TaxPeriodID == null)
				return;

			bool allowProcess = (e.Row != null && taxper.VendorID != null);
			filter.StartDate = taxper.StartDate;
			filter.EndDate = (((DateTime)taxper.EndDate).AddDays(-1));

			if (allowProcess && taxper.Status == TaxPeriodStatus.Prepared)
			{
				int? maxRevision = TaxRevisionManager.CurrentRevisionId(sender.Graph, filter.VendorID, filter.TaxPeriodID);
				using (new PXReadBranchRestrictedScope(filter.BranchID))
				{
					TaxHistory history = History_Last.SelectWindowed(0, 1, filter.VendorID, filter.TaxPeriodID);
					if (history?.RevisionID == maxRevision)
					{
						allowProcess = false;
				}
			}
			}

			Vendor vendor = Vendor.SelectWindowed(0, 1, taxper.VendorID);
			if (vendor == null)
				return;

			string taxPeriodID = (string)Period_Header.GetValueExt<TaxPeriodFilter.taxPeriodID>(filter);
			TaxPeriod prepared = PXSelect<TaxPeriod,
										Where<TaxPeriod.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
										  And<TaxPeriod.taxPeriodID, NotEqual<Current<TaxPeriodFilter.taxPeriodID>>,
											And<TaxPeriod.status, Equal<TaxPeriodStatus.prepared>>>>>
										.SelectWindowed(this, 0, 1);

			if (prepared != null)
			{
				sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, taxPeriodID,
																																		 new PXSetPropertyException<TaxPeriodFilter.taxPeriodID>(
																																			Messages.CannotPrepareReportExistPrepared,
																																			PXErrorLevel.Error));
				allowProcess = false;
			}

			if (allowProcess)
			{
				using (new PXReadBranchRestrictedScope(filter.BranchID))
				{
					if (taxper.Status != TaxPeriodStatus.Dummy &&
					ReportTaxProcess.CheckForUnprocessedSVAT(this, filter.BranchID, vendor, taxper?.EndDate))
					{
						sender.RaiseExceptionHandling<TaxPeriodFilter.vendorID>(e.Row, filter.VendorID,
							new PXSetPropertyException(Messages.TaxReportHasUnprocessedSVAT, PXErrorLevel.Warning));
					}

					switch (taxper.Status)
					{
						case TaxPeriodStatus.Dummy:
							sender.RaiseExceptionHandling<TaxPeriodFilter.vendorID>(e.Row, taxper.VendorID,
								new PXSetPropertyException<TaxPeriodFilter.vendorID>(Messages.TaxAgencyWithoutTran, PXErrorLevel.Warning));
							break;
						case TaxPeriodStatus.Closed:
							{
								if (vendor.TaxReportFinPeriod == true)
								{
									OldestNotReportedTaxTran.OrderByNew<OrderBy<Asc<TaxTran.finDate>>>();
								}

								TaxTranReport tran = OldestNotReportedTaxTran.SelectWindowed(0, 1, taxper.VendorID);

								if (tran != null && tran.TranDate != null && 
									(vendor.TaxReportFinPeriod != true && taxper.StartDate > tran.TranDate ||
									vendor.TaxReportFinPeriod == true && taxper.StartDate > tran.FinDate))
								{
									sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, taxPeriodID,
										new PXSetPropertyException<TaxPeriodFilter.taxPeriodID>(
																																							Messages.OneOrMoreTaxTransactionsFromPreviousPeriodsWillBeReported,
																																							PXErrorLevel.Warning));
								}

								if (tran == null || tran.TranDate == null ||
									(vendor.TaxReportFinPeriod != true && taxper.EndDate <= tran.TranDate ||
										vendor.TaxReportFinPeriod == true && taxper.EndDate <= tran.FinDate))
								{
									sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, taxPeriodID,
																																						 new PXSetPropertyException<TaxPeriodFilter.taxPeriodID>(
																																							Messages.NoAdjustmentToReportedTaxPeriodWillBeMade,
																																							PXErrorLevel.Warning));
									allowProcess = false;
								}
							}
							break;
						default:
							{
								if (vendor.TaxReportFinPeriod == true)
								{
									OldestNotReportedTaxTran.OrderByNew<OrderBy<Asc<TaxTran.finDate>>>();
								}

								TaxTranReport tran = OldestNotReportedTaxTran.SelectWindowed(0, 1, taxper.VendorID);
								if (tran == null)
									break;

								if (taxper.Status == TaxPeriodStatus.Open)
								{
									TaxPeriod period = PXSelect<TaxPeriod,
											Where<TaxPeriod.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
												And<TaxPeriod.taxPeriodID, Less<Current<TaxPeriodFilter.taxPeriodID>>,
												And<TaxPeriod.status, Equal<TaxPeriodStatus.open>>>>>
											.SelectWindowed(this, 0, 1);

									if (period != null && vendor.UpdClosedTaxPeriods != true &&
										(vendor.TaxReportFinPeriod != true && taxper.StartDate > tran.TranDate ||
											vendor.TaxReportFinPeriod == true && taxper.StartDate > tran.FinDate))
									{
										sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, taxPeriodID,
																																							 new PXSetPropertyException<TaxPeriodFilter.taxPeriodID>(
																																								Messages.CannotPrepareReportPreviousOpen,
																																								PXErrorLevel.Error));
										allowProcess = false;
									}
								}

								if (allowProcess)
								{
									if (tran.TranDate != null && 
										(vendor.TaxReportFinPeriod != true && taxper.StartDate > tran.TranDate ||
										vendor.TaxReportFinPeriod == true && taxper.StartDate > tran.FinDate))
									{
										sender.RaiseExceptionHandling<TaxPeriodFilter.taxPeriodID>(e.Row, taxPeriodID,
											new PXSetPropertyException<TaxPeriodFilter.taxPeriodID>(
																																								Messages.OneOrMoreTaxTransactionsFromPreviousPeriodsWillBeReported,
																																								PXErrorLevel.Warning));
									}
								}
							}
							break;
					}
				}
			}

			fileTax.SetEnabled(allowProcess);
		}

		protected virtual void TaxPeriodFilter_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetValue<TaxPeriodFilter.taxPeriodID>(e.Row, null);
		}
        
		protected virtual TaxPeriod GetTaxPeriod(TaxPeriodFilter filter)
		{
			TaxPeriod taxper = null;

			Vendor vend = Vendor.Current;
			if (vend == null)
			{
				return taxper;
			}

			taxper = PXSelect<TaxPeriod,
													Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
															And<TaxPeriod.status, Equal<TaxPeriodStatus.prepared>>>,
													OrderBy<Asc<TaxPeriod.taxPeriodID>>>
													.SelectWindowed(this, 0, 1, vend.BAccountID);
					if (taxper != null)
					{
				return taxper;
					}

					if (vend.TaxReportFinPeriod == true)
					{
						OldestNotReportedTaxTran.OrderByNew<OrderBy<Asc<TaxTran.finDate>>>();
					}

                    TaxTranReport first_tran = OldestNotReportedTaxTran.SelectWindowed(0, 1, vend.BAccountID);
			DateTime? first_date = first_tran != null && first_tran.TranDate != null 
				? first_tran.TranDate 
				: this.Accessinfo.BusinessDate;

					if (first_tran != null && vend.TaxReportFinPeriod == true)
					{
							first_date = first_tran.FinDate;
					}

			TaxPeriod lastTaxPeriod = (TaxPeriod)PXSelect<TaxPeriod,
															Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>>,
															OrderBy<Desc<TaxPeriod.taxPeriodID>>>
						.SelectWindowed(this, 0, 1, vend.BAccountID);
					if (lastTaxPeriod?.Status == TaxPeriodStatus.Closed)
					{
						TaxCalendar.Create(this, TaxYear_Current, TaxPeriod_ByDate, vend, lastTaxPeriod.EndDate, lastTaxPeriod.EndDate);
					}

					taxper = PXSelect<TaxPeriod,
						Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
							And<TaxPeriod.startDate, LessEqual<Required<TaxPeriod.startDate>>,
					And<TaxPeriod.endDate, Greater<Required<TaxPeriod.endDate>>>>>>
				.SelectWindowed(this, 0, 1, vend.BAccountID, first_date, first_date);
			if (taxper != null && (vend.UpdClosedTaxPeriods == true || 
				taxper.Status == TaxPeriodStatus.Dummy || 
				taxper.Status == TaxPeriodStatus.Open))
					{
				return taxper;
					}

					taxper = PXSelect<TaxPeriod,
										Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
											And<TaxPeriod.status, Equal<TaxPeriodStatus.open>>>,
										OrderBy<Asc<TaxPeriod.taxPeriodID>>>
										.SelectWindowed(this, 0, 1, vend.BAccountID);
					if (taxper != null)
					{
				return taxper;
					}

					if (first_date != null)
					{
                        taxper = TaxCalendar.Create(this, TaxYear_Current, TaxPeriod_ByDate, vend,
					vend.EnableTaxStartDate == true 
						? vend.TaxYearStartDate ?? new DateTime(first_date.Value.Year, 1, 1) 
						: new DateTime(first_date.Value.Year, 1, 1),
							first_date);
						if (taxper != null)
						{
					taxper.Status = first_tran == null || first_tran.TranDate == null 
						? TaxPeriodStatus.Dummy
						: taxper.Status;
					return taxper;
							}
						}

			return taxper;
					}

		protected virtual void TaxPeriodFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			TaxPeriodFilter filter = (TaxPeriodFilter)e.Row;
			if (filter == null) return;

			if (!sender.ObjectsEqual<TaxPeriodFilter.branchID>(e.Row, e.OldRow))
			{
				List<PXView> views = this.Views.Select(view => view.Value).ToList();
				foreach (var view in views) view.Clear();
				this.Caches[typeof(TaxPeriod)].Clear();
				this.Caches[typeof(TaxPeriod)].ClearQueryCache();
				this.Caches[typeof(TaxYear)].Clear();
				}

			TaxPeriod taxper = TaxPeriod_Current.SelectSingle();
			if (!sender.ObjectsEqual<TaxPeriodFilter.branchID, TaxPeriodFilter.vendorID>(e.Row, e.OldRow))
			{
				History_Last.View.Clear();
				OldestNotReportedTaxTran.View.Clear();
				TaxYear_Current.Current = TaxYear_Current.SelectSingle();
				
				using (new PXReadBranchRestrictedScope(Period_Header.Current.BranchID))
				{
					taxper = GetTaxPeriod(filter);
					filter.TaxPeriodID = taxper?.TaxPeriodID ?? filter.TaxPeriodID;
				}
			}
		}

		protected virtual void TaxPeriodFilter_TaxPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		protected virtual IEnumerable period_Details()
		{
			TaxReportLine prev_line = null;
			TaxHistory hist = null;
			PXResultset<TaxReportLine, TaxHistory> ret = new PXResultset<TaxReportLine, TaxHistory>();
			TaxPeriod_Current.Current = TaxPeriod_Current.SelectSingle();

			if (Period_Header.Current.TaxPeriodID == null)
				return ret;

			Vendor vendor = Vendor.Current;
			using (new PXReadBranchRestrictedScope(Period_Header.Current.BranchID))
				foreach (PXResult<TaxReportLine, TaxBucketLine, TaxRev, TaxTranReport, Currency, CurrencyRate, CurrencyRateByDate> res 
					in Period_Details_Expanded.Select().Where(line => ShowTaxReportLine(line.GetItem<TaxReportLine>(), Period_Header.Current.TaxPeriodID)))
				{
					TaxReportLine line = res;
					TaxTranReport tran = res;
					Currency cury = res;
					CurrencyRate rate = res;
					if (rate.FromCuryID == null)
						rate = (CurrencyRateByDate)res;

					if (object.Equals(prev_line, line) == false || hist == null)
					{
						if (hist != null)
						{
							ret.Add(new PXResult<TaxReportLine, TaxHistory>(prev_line, hist));
						}

						hist = new TaxHistory();
						hist.BranchID = null;
						hist.VendorID = line.VendorID;
						hist.LineNbr = line.LineNbr;
						hist.TaxPeriodID = Period_Header.Current.TaxPeriodID;
						hist.UnfiledAmt = 0m;
						hist.ReportUnfiledAmt = 0m;
					}

					if (tran.RefNbr != null && tran.TaxType != TaxType.PendingSales && tran.TaxType != TaxType.PendingPurchase)
					{
						decimal HistMult = ReportTaxProcess.GetMult(tran.Module, tran.TranType, tran.TaxType, line.LineMult);

						switch (line.LineType)
						{
							case "P":
								hist.UnfiledAmt += HistMult * tran.TaxAmt.GetValueOrDefault();
								hist.ReportUnfiledAmt += HistMult *
									(cury.CuryID == tran.CuryID ? tran.CuryTaxAmt.GetValueOrDefault() :
									 cury.CuryID == company.Current.BaseCuryID || cury.CuryID == null ? tran.TaxAmt.GetValueOrDefault() :
									 TaxHistorySumManager.RecalcCurrency(cury, rate, tran.CuryTaxAmt.GetValueOrDefault()));
								break;
							case "A":
								hist.UnfiledAmt += HistMult * tran.TaxableAmt.GetValueOrDefault();
								hist.ReportUnfiledAmt += HistMult *
									(cury.CuryID == tran.CuryID ? tran.CuryTaxableAmt.GetValueOrDefault() :
									 cury.CuryID == company.Current.BaseCuryID || cury.CuryID == null ? tran.TaxableAmt.GetValueOrDefault() :
									 TaxHistorySumManager.RecalcCurrency(cury, rate, tran.CuryTaxableAmt.GetValueOrDefault()));
								break;
						}
					}
					if (line.TempLine == true || line.TempLineNbr != null && hist.ReportUnfiledAmt.GetValueOrDefault() == 0)
						hist = null;
					else
						prev_line = line;
				}

			if (hist != null)
			{
				ret.Add(new PXResult<TaxReportLine, TaxHistory>(prev_line, hist));
			}
			ret = (vendor != null && vendor.TaxUseVendorCurPrecision != true) ? 
				TaxHistorySumManager.GetPreviewReport(this, vendor, ret, (line) => ShowTaxReportLine(line, Period_Header.Current.TaxPeriodID)) : ret;
            PXResultset<TaxReportLine, TaxHistory> result = new PXResultset<TaxReportLine, TaxHistory>();
            foreach (PXResult<TaxReportLine, TaxHistory> pxResult in ret)
            {
                TaxReportLine line = pxResult;
                if (line.HideReportLine != true)
                {
                    result.Add(pxResult);
                }
            }
            return result;
				}

		public virtual bool ShowTaxReportLine(TaxReportLine taxReportLine, string taxPeriodID)
		{
			return true;
		}

		public PXAction<TaxPeriodFilter> fileTax;
		[PXUIField(DisplayName = Messages.PrepareTaxReport, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable FileTax(PXAdapter adapter)
		{
			Actions.PressSave();
			TaxPeriodFilter p = Period_Header.Current;
            TaxReportMaint.TaxBucketAnalizer.CheckTaxAgencySettings(this, p.VendorID == null ? 0 : (int)p.VendorID);
			PXLongOperation.StartOperation(this, () => ReportTax.FileTaxProc(p));
			return adapter.Get();
		}

       public static void FileTaxProc(TaxPeriodFilter p)
		{
			ReportTaxProcess fp = PXGraph.CreateInstance<ReportTaxProcess>();
			string warning = fp.FileTaxProc(p);
			ReportTaxReview docgraph = PXGraph.CreateInstance<ReportTaxReview>();
			docgraph.TimeStamp = fp.TimeStamp;
			docgraph.Period_Header.Insert();
			docgraph.Period_Header.Current.BranchID = p.BranchID;
			docgraph.Period_Header.Current.VendorID = p.VendorID;
			docgraph.Period_Header.Current.TaxPeriodID = p.TaxPeriodID;
			docgraph.Period_Header.Current.RevisionId = TaxRevisionManager.CurrentRevisionId(docgraph, p.VendorID, p.TaxPeriodID);
			docgraph.Period_Header.Current.PreparedWarningMsg = warning;
			throw new PXRedirectRequiredException(docgraph, Messages.Review);			
		}

		public ReportTax()
		{
			APSetup setup = APSetup.Current;

			Period_Details.Cache.AllowInsert = false;
			Period_Details.Cache.AllowUpdate = false;
			Period_Details.Cache.AllowDelete = false;
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}


		public PXSetup<APSetup> APSetup;
	}

	[PXHidden()]
	public class ReportTaxProcess : PXGraph<ReportTaxProcess>
	{

		public PXSelect<TaxHistory, Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
						And<TaxHistory.branchID, Equal<Required<TaxHistory.branchID>>, And<TaxHistory.accountID, Equal<Required<TaxHistory.accountID>>,
						And<TaxHistory.subID, Equal<Required<TaxHistory.subID>>, And<TaxHistory.taxID, Equal<Required<TaxHistory.taxID>>, And<TaxHistory.taxPeriodID,
						Equal<Required<TaxHistory.taxPeriodID>>, And<TaxHistory.lineNbr, Equal<Required<TaxHistory.lineNbr>>, And<TaxHistory.revisionID,
						Equal<Required<TaxHistory.revisionID>>>>>>>>>>> TaxHistory_Current;

		public PXSelectJoin<TaxReportLine,
			InnerJoin<TaxBucketLine,
						 On<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>,
						And<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>>>,
			InnerJoin<TaxTranReport, On<TaxTranReport.vendorID, Equal<TaxReportLine.vendorID>,
						And<TaxTranReport.taxBucketID, Equal<TaxBucketLine.bucketID>,
						And<TaxTranReport.revisionID, Equal<Required<TaxTranReport.revisionID>>,
						And<TaxTranReport.released, Equal<boolTrue>,
						And<TaxTranReport.voided, Equal<boolFalse>,
						And<TaxTranReport.taxPeriodID, Equal<Required<TaxTranReport.taxPeriodID>>,
						And<Where<TaxReportLine.taxZoneID, IsNull, And<TaxReportLine.tempLine, Equal<boolFalse>,
									 Or<TaxReportLine.taxZoneID, Equal<TaxTranReport.taxZoneID>>>>>>>>>>>>>,
			 Where<TaxReportLine.vendorID, Equal<Required<TaxReportLine.vendorID>>>,
			 OrderBy<Asc<TaxReportLine.vendorID,
							 Asc<TaxTranReport.branchID,
							 Asc<TaxReportLine.lineNbr,
							 Asc<TaxTranReport.accountID, Asc<TaxTranReport.subID,
							 Asc<TaxTranReport.taxID,
							 Asc<TaxTranReport.taxPeriodID,
							 Asc<TaxTranReport.module, Asc<TaxTranReport.tranType, Asc<TaxTranReport.refNbr>>>>>>>>>>>> Period_Details_Expanded;

		public PXSelect<TaxPeriod, Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>, And<TaxPeriod.taxPeriodID, Equal<Required<TaxPeriod.taxPeriodID>>>>> TaxPeriod_Current;
		public PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<TaxPeriod.vendorID>>>> Vendor_Current;

		public PXSelectJoin<TaxAdjustment,
			LeftJoin<TaxPeriod, On<TaxPeriod.vendorID, Equal<TaxAdjustment.vendorID>, And<TaxPeriod.taxPeriodID, Equal<TaxAdjustment.taxPeriod>>>,
			LeftJoin<TaxReportLine,
						On<TaxReportLine.vendorID, Equal<TaxAdjustment.vendorID>, And<TaxReportLine.netTax, Equal<boolTrue>, And<TaxReportLine.tempLine, Equal<boolTrue>>>>,
			InnerJoin<Vendor, On<Vendor.bAccountID, Equal<TaxAdjustment.vendorID>>>>>,
			Where<TaxAdjustment.docType, Equal<Required<TaxAdjustment.docType>>,
				And<TaxAdjustment.refNbr, Equal<Required<TaxAdjustment.refNbr>>>>> TaxAdjustment_Select;

		public PXSelectJoin<TaxTran,
			InnerJoin<TaxBucketLine, On<TaxBucketLine.vendorID, Equal<TaxTran.vendorID>, And<TaxBucketLine.bucketID, Equal<TaxTran.taxBucketID>>>,
			InnerJoin<TaxReportLine, On<TaxReportLine.vendorID, Equal<TaxBucketLine.vendorID>, And<TaxReportLine.lineNbr, Equal<TaxBucketLine.lineNbr>>>>>,
			Where<TaxTran.tranType, Equal<Required<TaxTran.tranType>>,
				And<TaxTran.refNbr, Equal<Required<TaxTran.refNbr>>,
				And<Where<TaxReportLine.taxZoneID, IsNull, And<TaxReportLine.tempLine, Equal<boolFalse>,
											 Or<TaxReportLine.taxZoneID, Equal<TaxTran.taxZoneID>>>>>>>> TaxTran_Select;

		public static TaxPeriod FindTaxPeriodByKey(PXGraph graph, int? taxAgencyID, string taxPeriodID)
		{
			return PXSelect<TaxPeriod, 
								Where<TaxPeriod.taxPeriodID,Equal<Required<TaxPeriod.taxPeriodID>>,
										And<TaxPeriod.vendorID, Equal<TaxPeriod.vendorID>>>>
								.Select(graph, taxPeriodID, taxAgencyID);
		}

		public static TaxPeriod GetTaxPeriodByKey(PXGraph graph, int? taxAgencyID, string taxPeriodID)
		{
			var taxPeriod = FindTaxPeriodByKey(graph, taxAgencyID, taxPeriodID);

			if (taxPeriod == null)
				throw new PXException(Messages.ReportingPeriodDoesNotExistForTheTaxAgency,
					taxPeriodID, VendorMaint.GetByID(graph, taxAgencyID).AcctCD.Trim());

			return taxPeriod;
		}
		
		public static bool PrepearedTaxPeriodForVendorExists(PXGraph graph, int? vendorID)
		{
			var prepearedTaxPeriod = (TaxPeriod)PXSelect<TaxPeriod,
												Where<TaxPeriod.vendorID, Equal<Required<TaxPeriod.vendorID>>,
														And<TaxPeriod.status, Equal<TaxPeriodStatus.prepared>>>>
												.Select(graph, vendorID);

			return prepearedTaxPeriod != null;
		}

        [PXDate()]
        [PXDBScalar(typeof(Search<CurrencyRate.curyEffDate,

            Where<CurrencyRate.fromCuryID, Equal<TaxTran.curyID>,
            And<CurrencyRate.toCuryID, Equal<CurrentValue<AP.Vendor.curyID>>,
            And<CurrencyRate.curyRateType, Equal<CurrentValue<AP.Vendor.curyRateTypeID>>,
            And<CurrencyRate.curyEffDate, LessEqual<TaxTran.tranDate>>>>>,
            OrderBy<Desc<CurrencyRate.curyEffDate>>>))]
        protected virtual void TaxTran_CuryEffDate_CacheAttached(PXCache sender)
        { 
        }

		public static decimal GetMult(TaxTran tran)
		{
			return GetMult(tran.Module, tran.TranType, tran.TaxType, 1);
		}

		public static decimal GetMultByTranType(string module, string tranType)
		{
			return (module == BatchModule.AP && APDocType.TaxDrCr(tranType) == DrCr.Debit ||
					module == BatchModule.AR && ARDocType.TaxDrCr(tranType) == DrCr.Credit ||
					(module == BatchModule.GL && tranType != TaxTran.tranType.TranReversed) ||
					module == BatchModule.CA ? 1m : -1m);
		}

		public static decimal GetMult(string module, string tranType, string tranTaxType, short? reportLineMult)
		{
			decimal lineMult = (reportLineMult == 1 && tranTaxType == TaxType.Sales ||
				reportLineMult == 1 && tranTaxType == TaxType.PendingSales ||
				reportLineMult == -1 && tranTaxType == TaxType.Purchase ||
				reportLineMult == -1 && tranTaxType == TaxType.PendingPurchase ? 1m : -1m);

			return GetMultByTranType(module, tranType) * lineMult;
		}

		private void SegregateBatch(JournalEntry je, int? BranchID, string CuryID, DateTime? DocDate, string FinPeriodID, string description)
		{
			Batch apbatch = je.BatchModule.Current;

			if (apbatch == null ||
				!object.Equals(apbatch.BranchID, BranchID) ||
				!object.Equals(apbatch.CuryID, CuryID) ||
				!object.Equals(apbatch.FinPeriodID, FinPeriodID))
			{
				je.Clear();

				CurrencyInfo info = new CurrencyInfo();
				info.CuryID = CuryID;
				info.CuryEffDate = DocDate;
				info = je.currencyinfo.Insert(info);

				apbatch = new Batch();
				apbatch.BranchID = BranchID;
				apbatch.Module = "GL";
				apbatch.Status = "U";
				apbatch.Released = true;
				apbatch.Hold = false;
				apbatch.DateEntered = DocDate;
				apbatch.FinPeriodID = FinPeriodID;
				apbatch.TranPeriodID = FinPeriodID;
				apbatch.CuryID = CuryID;
				apbatch.CuryInfoID = info.CuryInfoID;
				apbatch.Description = description;
				apbatch = je.BatchModule.Insert(apbatch);

				CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<Batch.curyInfoID>>>>.Select(je, null);
				b_info.CuryID = CuryID;
				b_info.CuryEffDate = DocDate;
				je.currencyinfo.Update(b_info);
			}
		}

		private void UpdateHistory(TaxTran tran, TaxReportLine line)
		{
			TaxHistory hist = TaxHistory_Current.Select(tran.VendorID, tran.BranchID, tran.AccountID, tran.SubID, tran.TaxID, tran.TaxPeriodID, line.LineNbr, tran.RevisionID);

			if (hist == null)
			{
				hist = new TaxHistory();
				hist.RevisionID = tran.RevisionID;
				hist.VendorID = tran.VendorID;
				hist.BranchID = tran.BranchID;
				hist.AccountID = tran.AccountID;
				hist.SubID = tran.SubID;
				hist.TaxID = tran.TaxID;
				hist.TaxPeriodID = tran.TaxPeriodID;
				hist.LineNbr = line.LineNbr;

				hist = (TaxHistory)TaxHistory_Current.Cache.Insert(hist);
			}

			decimal HistMult = GetMult(tran.Module, tran.TranType, tran.TaxType, line.LineMult);

			switch (line.LineType)
			{
				case "P":
					hist.ReportFiledAmt += HistMult * tran.ReportTaxAmt.GetValueOrDefault();
					hist.FiledAmt += HistMult * tran.TaxAmt.GetValueOrDefault();
					break;
				case "A":
					hist.ReportFiledAmt += HistMult * tran.ReportTaxableAmt.GetValueOrDefault();
					hist.FiledAmt += HistMult * tran.TaxableAmt.GetValueOrDefault();
					break;
			}
			TaxHistory_Current.Cache.Update(hist);
		}

		public virtual void ReleaseDocProc(JournalEntry je, TaxAdjustment doc)
		{
			if (doc.Hold == true)
			{
				throw new PXException(AP.Messages.Document_OnHold_CannotRelease);
			}

			using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					RoundingManager rmanager = new RoundingManager(this, doc.VendorID);
					int? revisionId = TaxRevisionManager.CurrentRevisionId(this, doc.VendorID, doc.TaxPeriod);

					foreach (PXResult<TaxAdjustment, TaxPeriod, TaxReportLine, Vendor> res in TaxAdjustment_Select.Select(doc.DocType, doc.RefNbr))
					{
						TaxAdjustment taxdoc = res;
						TaxPeriod taxper = res;
						TaxReportLine taxline = res;
						Vendor vend = res;

						if (taxper.Status == null || taxper.Status == TaxPeriodStatus.Open || taxper.Status == TaxPeriodStatus.Closed && taxline.NetTax != null)
						{
							throw new PXException(Messages.Only_Prepared_CanBe_Adjusted);
						}

						//use timestamp to handle concurrent report releasing
						Caches[typeof (TaxPeriod)].SetStatus(taxper, PXEntryStatus.Updated);

                        SegregateBatch(je, taxdoc.BranchID, taxdoc.CuryID, taxdoc.DocDate, taxdoc.FinPeriodID, taxdoc.DocDesc);
                        bool debit = (doc.DocType == TaxAdjustmentType.AdjustOutput ? 1 : -1) * Math.Sign(taxdoc.OrigDocAmt ?? 0m) > 0;

						GLTran tran = new GLTran();
						tran.AccountID = taxdoc.AdjAccountID;
						tran.SubID = taxdoc.AdjSubID;
						tran.CuryDebitAmt = debit ? Math.Abs(taxdoc.CuryOrigDocAmt ?? 0m) : 0m;
                        tran.DebitAmt = debit ? Math.Abs(taxdoc.OrigDocAmt ?? 0m) : 0m;
                        tran.CuryCreditAmt = debit ? 0m : Math.Abs(taxdoc.CuryOrigDocAmt ?? 0m);
                        tran.CreditAmt = debit ? 0m : Math.Abs(taxdoc.OrigDocAmt ?? 0m);
						tran.TranType = taxdoc.DocType;
						tran.TranClass = GLTran.tranClass.Normal;
						tran.RefNbr = taxdoc.RefNbr;
						tran.TranDesc = taxdoc.DocDesc;
                        tran.TranPeriodID = taxdoc.FinPeriodID;
                        tran.FinPeriodID = taxdoc.FinPeriodID;
						tran.TranDate = taxdoc.DocDate;
						tran.CuryInfoID = je.BatchModule.Current.CuryInfoID;
						tran.Released = true;

						je.GLTranModuleBatNbr.Insert(tran);

					}

					TaxTran prev_n = null;

					foreach (PXResult<TaxTran, TaxBucketLine, TaxReportLine> res in TaxTran_Select.Select(doc.DocType, doc.RefNbr))
					{
						TaxTran n = res;
						TaxReportLine line = res;
                        bool debit = (doc.DocType == TaxAdjustmentType.AdjustOutput ? 1 : -1) * Math.Sign(n.TaxAmt ?? 0m) > 0;

						if (object.Equals(n, prev_n) == false)
						{
							GLTran tran = new GLTran();
							tran.AccountID = n.AccountID;
							tran.SubID = n.SubID;
							tran.CuryDebitAmt = debit ? 0m : Math.Abs(n.CuryTaxAmt ?? 0m);
                            tran.DebitAmt = debit ? 0m : Math.Abs(n.TaxAmt ?? 0m);
                            tran.CuryCreditAmt = debit ? Math.Abs(n.CuryTaxAmt ?? 0m) : 0m;
                            tran.CreditAmt = debit ? Math.Abs(n.TaxAmt ?? 0m) : 0m;
							tran.TranType = doc.DocType;
							tran.TranClass = GLTran.tranClass.Normal;
							tran.RefNbr = doc.RefNbr;
							tran.TranDesc = n.Description;
                            tran.TranPeriodID = doc.FinPeriodID;
                            tran.FinPeriodID = doc.FinPeriodID;
							tran.TranDate = doc.DocDate;
							tran.CuryInfoID = je.BatchModule.Current.CuryInfoID;
							tran.Released = true;

							je.GLTranModuleBatNbr.Insert(tran);

							n.Released = true;
							n.RevisionID = revisionId;
							TaxTran_Select.Cache.Update(n);
						}
						prev_n = n;
						UpdateHistory(n, line);
					}

					je.Save.Press();

					doc.Released = true;
					doc.BatchNbr = je.BatchModule.Current.BatchNbr;
					doc = TaxAdjustment_Select.Update(doc);

					foreach (TaxHistory rounding in PXSelect<TaxHistory,
						Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
							And<TaxHistory.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>,
							And<TaxHistory.revisionID, Equal<Required<TaxHistory.revisionID>>,
							And<TaxHistory.taxID, Equal<StringEmpty>>>>>>.Select(this, doc.VendorID, doc.TaxPeriod, revisionId))
					{
						TaxHistory_Current.Cache.Delete(rounding);
					}

					this.Persist();
                    TaxHistorySumManager.UpdateTaxHistorySums(this, rmanager, doc.TaxPeriod, revisionId, doc.BranchID,
						(line) => ShowTaxReportLine(line, doc.TaxPeriod));
					ts.Complete(this);
				}

				TaxAdjustment_Select.Cache.Persisted(false);
				TaxTran_Select.Cache.Persisted(false);
				TaxHistory_Current.Cache.Persisted(false);
			}
		}

		public virtual void VoidReportProc(TaxPeriodFilter taxper)
		{
			using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					using (new PXReadBranchRestrictedScope(taxper.BranchID))
					{
						PXUpdate<
							Set<TaxTran.taxPeriodID, Null,
								Set<TaxTran.revisionID, Null>>,
							TaxTran,
							Where<TaxTran.vendorID, Equal<Required<TaxTran.vendorID>>,
								And<TaxTran.taxPeriodID, Equal<Required<TaxTran.taxPeriodID>>,
								And<TaxTran.revisionID, Equal<Required<TaxTran.revisionID>>,
								And<TaxTran.released, Equal<True>,
								And<TaxTran.voided, Equal<False>>>>>>>
							.Update(this, taxper.VendorID, taxper.TaxPeriodID, taxper.RevisionId);

						foreach (TaxHistory history in PXSelect<TaxHistory,
							Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
								And<TaxHistory.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>,
									And<TaxHistory.revisionID, Equal<Required<TaxHistory.revisionID>>>>>>
							.Select(this, taxper.VendorID, taxper.TaxPeriodID, taxper.RevisionId))
						{
							TaxHistory_Current.Cache.Delete(history);
						}
					}

					TaxHistory_Current.Cache.Persist(PXDBOperation.Delete);
					TaxHistory_Current.Cache.Persisted(false);
					bool needUpdatePeriodState;
					using (new PXReadBranchRestrictedScope())
					{
						TaxHistory history = PXSelect<TaxHistory,
							Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
								And<TaxHistory.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>,
								And<TaxHistory.revisionID, Equal<Required<TaxHistory.revisionID>>>>>>
							.SelectWindowed(this, 0, 1, taxper.VendorID, taxper.TaxPeriodID, taxper.RevisionId);
						needUpdatePeriodState = history == null;
					}

                    if (needUpdatePeriodState)
					{
						foreach (TaxPeriod res in TaxPeriod_Current.Select(taxper.VendorID, taxper.TaxPeriodID))
						{
							if (res.Status != TaxPeriodStatus.Prepared)
							{
								throw new PXException();
							}
							res.Status = taxper.RevisionId > 1 ? TaxPeriodStatus.Closed : TaxPeriodStatus.Open;
							TaxPeriod_Current.Cache.Update(res);
						}

						TaxPeriod_Current.Cache.Persist(PXDBOperation.Update);

						TaxPeriod_Current.Cache.Persisted(false);
						bool yearHasNoProcessedTransactions;
						using (new PXReadBranchRestrictedScope())
						{
							TaxPeriod anyProcessedTransactions = PXSelectReadonly2<TaxPeriod, 
								InnerJoin<TaxHistory,
									On<TaxPeriod.vendorID, Equal<TaxHistory.vendorID>,
										And<TaxPeriod.taxPeriodID, Equal<TaxHistory.taxPeriodID>>>>,
								Where<TaxPeriod.vendorID,Equal<Required<TaxPeriod.vendorID>>,
									And<TaxPeriod.taxYear,Equal<Required<TaxPeriod.taxYear>>>>>.
								Select(this, taxper.VendorID, taxper.TaxPeriodID.Substring(0, 4));
							yearHasNoProcessedTransactions = anyProcessedTransactions == null;
						}
						if (yearHasNoProcessedTransactions)
						{
							PXDatabase.Delete<TaxPeriod>(
								new PXDataFieldRestrict<TaxPeriod.vendorID>(taxper.VendorID),
								new PXDataFieldRestrict<TaxPeriod.taxYear>(taxper.TaxPeriodID.Substring(0, 4)));

							PXDatabase.Delete<TaxYear>(
								new PXDataFieldRestrict<TaxYear.vendorID>(taxper.VendorID),
								new PXDataFieldRestrict<TaxYear.year>(taxper.TaxPeriodID.Substring(0, 4)));
						}
					}

					ts.Complete(this);
					
				}
			}
		}

		public virtual void ClosePeriodProc(TaxPeriodFilter p)
		{
			List<APRegister> doclist = new List<APRegister>();

			using (new PXReadBranchRestrictedScope())
			using (new PXConnectionScope())
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				CheckUnreleasedTaxAdjustmentsDoNotExist(p.TaxPeriodID);

				if (CheckForUnprocessedPPD(this, p.VendorID, p.EndDate))
				{
					throw new PXSetPropertyException(AR.Messages.UnprocessedPPDExists, PXErrorLevel.Error);
				}
				
				TaxPeriod taxper = TaxPeriod_Current.Select(p.VendorID, p.TaxPeriodID);

				if (taxper.Status != "P")
				{
					throw new PXException(Messages.CannotCloseReportForNotPreparedPeriod);
				}

				taxper.Status = "C";
				TaxPeriod_Current.Cache.Update(taxper);

				Vendor vendor = Vendor_Current.Select(p.VendorID);
				
				string FinPeriodID = null;
                DateTime docDate = ((DateTime)taxper.EndDate).AddDays(-1);

				VerifyTaxConfigurationErrors(p, taxper, vendor)
					.RaiseIfHasError();

				APInvoiceEntry docgraph = PXGraph.CreateInstance<APInvoiceEntry>();

				Dictionary<int?, KeyValuePair<APInvoice, List<APTran>>> tranlist = new Dictionary<int?, KeyValuePair<APInvoice, List<APTran>>>();

				foreach (PXResult<TaxHistory, TaxReportLine> res in
					PXSelectJoinGroupBy<TaxHistory,
						InnerJoin<TaxReportLine, On<TaxReportLine.vendorID, Equal<TaxHistory.vendorID>,
							And<TaxReportLine.lineNbr, Equal<TaxHistory.lineNbr>>>>,
						Where<TaxHistory.vendorID, Equal<Required<TaxHistory.vendorID>>,
							And<TaxHistory.taxPeriodID, Equal<Required<TaxHistory.taxPeriodID>>,
							And<TaxHistory.revisionID, Equal<Required<TaxHistory.revisionID>>>>>,
						Aggregate<GroupBy<TaxHistory.vendorID,
							GroupBy<TaxHistory.branchID,
							GroupBy<TaxHistory.lineNbr,
							GroupBy<TaxHistory.accountID,
							GroupBy<TaxHistory.subID,
							GroupBy<TaxHistory.taxID,
							GroupBy<TaxReportLine.netTax,
							Sum<TaxHistory.filedAmt>>>>>>>>>>.Select(docgraph, p.VendorID, p.TaxPeriodID, p.RevisionId))
				{
					TaxHistory hist = res;
					TaxReportLine line = res;

					if (line.NetTax == true && hist.BranchID != null)
					{
						KeyValuePair<APInvoice, List<APTran>> pair;
						if (!tranlist.TryGetValue(hist.BranchID, out pair))
						{
							tranlist[hist.BranchID] = pair = new KeyValuePair<APInvoice, List<APTran>>(new APInvoice(), new List<APTran>());
							pair.Key.CuryLineTotal = 0m;
							pair.Key.LineTotal = 0m;
						}

						APTran new_aptran = new APTran();
						new_aptran.BranchID = hist.BranchID;
						new_aptran.AccountID = hist.AccountID;
						new_aptran.SubID = hist.SubID;
						new_aptran.LineAmt = hist.FiledAmt;
						new_aptran.CuryLineAmt = hist.ReportFiledAmt;
						new_aptran.TranAmt = hist.FiledAmt;
						new_aptran.CuryTranAmt = hist.ReportFiledAmt;

						new_aptran.TranDesc = string.IsNullOrEmpty(line.TaxZoneID)
						                      	? string.Format(Messages.TranDesc1, hist.TaxID)
						                      	: string.Format(Messages.TranDesc2, hist.TaxID, line.TaxZoneID);

						pair.Value.Add(new_aptran);
						pair.Key.CuryLineTotal += new_aptran.CuryLineAmt ?? 0m;
						pair.Key.LineTotal += new_aptran.LineAmt ?? 0m;
					}
				}

				if (tranlist.Count > 0)
				{
					foreach (KeyValuePair<int?, KeyValuePair<APInvoice, List<APTran>>> pair in tranlist)
					{
						int? BranchID = pair.Key;
						decimal? TranTotal = pair.Value.Key.LineTotal;
						decimal? CuryTranTotal = pair.Value.Key.CuryLineTotal;

						docgraph.Clear();
						docgraph.vendor.Current = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.Select(docgraph, taxper.VendorID);
                       
                        bool postingToClosed=(bool)((GLSetup)PXSelect<GLSetup>.Select(docgraph)).PostClosedPeriods;
                        FinPeriod currentFinPeriodID = PXSelect<FinPeriod,
                                            Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.SelectWindowed(docgraph, 0, 1, FinPeriodSelectorAttribute.PeriodFromDate(docDate));
                        if (FinPeriodID == null && (!(bool)currentFinPeriodID.Active || ((bool)currentFinPeriodID.APClosed && !postingToClosed)))
						{
							FinPeriodID = docgraph.vendor.Current.TaxPeriodType == "F"
							              	? taxper.TaxPeriodID
                                            : FinPeriodSelectorAttribute.PeriodFromDate(((DateTime)taxper.EndDate).AddDays(-1));

							FinPeriod openPeriod =
								PXSelect<FinPeriod,
									Where<FinPeriod.finPeriodID, GreaterEqual<Required<FinPeriod.finPeriodID>>,
										And<FinPeriod.aPClosed, Equal<False>>>,
									OrderBy<Asc<FinPeriod.finPeriodID>>>.SelectWindowed(docgraph, 0, 1, FinPeriodID);

							if (openPeriod != null)
							{
								FinPeriodID = openPeriod.FinPeriodID;
                                docDate = ((DateTime)openPeriod.EndDate).AddDays(-1);
							}
						}
						CurrencyInfo new_info = new CurrencyInfo();
						if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
						{
							new_info.CuryID = docgraph.vendor.Current.CuryID;

							if (!string.IsNullOrEmpty(docgraph.vendor.Current.CuryRateTypeID))
							{
							new_info.CuryRateTypeID = docgraph.vendor.Current.CuryRateTypeID;
							}

							new_info.BaseCalc = false;
						}

						new_info = docgraph.currencyinfo.Insert(new_info);

						APInvoice new_apdoc = new APInvoice();

						decimal amountsSign = CuryTranTotal >= 0m ? 1m : -1m;
						new_apdoc.DocType = (CuryTranTotal >= 0m ? APDocType.Invoice : APDocType.DebitAdj);
						new_apdoc.TaxCalcMode = VendorClass.taxCalcMode.TaxSetting;
						new_apdoc.VendorID = taxper.VendorID;
						new_apdoc.DocDate = docDate;
						new_apdoc.Released = false;
						new_apdoc.Hold = false;
						new_apdoc.CuryID = new_info.CuryID;
						new_apdoc.CuryInfoID = new_info.CuryInfoID;
						new_apdoc = docgraph.Document.Insert(new_apdoc);

						new_apdoc.BranchID = BranchID;
						new_apdoc.TaxZoneID = null;

						docgraph.APSetup.Current.RequireControlTotal = false;
						docgraph.APSetup.Current.RequireControlTaxTotal = false;

						foreach (APTran new_aptran in pair.Value.Value)
						{
							new_aptran.LineAmt = amountsSign * new_aptran.LineAmt;
							new_aptran.CuryLineAmt = amountsSign * new_aptran.CuryLineAmt;
							new_aptran.TranAmt = amountsSign * new_aptran.TranAmt;
							new_aptran.CuryTranAmt = amountsSign * new_aptran.CuryTranAmt;
							docgraph.Transactions.Insert(new_aptran);
						}

						/// Because with Multi-Currency enabled (in particular for foreign currency
						/// tag agencies) we disable automatic calculation of base amounts in the bill
						/// (<see cref="CurrencyInfo.BaseCalc"/>), we become fully responsible for
						/// updating document totals in base amounts properly.
						/// Normally they would be calculated from the corresponding currency amounts
						/// by the PXCurrency attributes.
						/// .
						new_apdoc.LineTotal = amountsSign * TranTotal;
						new_apdoc.CuryLineTotal = amountsSign * CuryTranTotal;
						new_apdoc.OrigDocAmt = amountsSign * TranTotal;
						new_apdoc.CuryOrigDocAmt = amountsSign * CuryTranTotal;
						new_apdoc.DocBal = amountsSign * TranTotal;
						new_apdoc.CuryDocBal = amountsSign * CuryTranTotal;
						new_apdoc.CuryOrigDiscAmt = 0m;
						new_apdoc.OrigDiscAmt = 0m;

						docgraph.Save.Press();
						doclist.Add(docgraph.Document.Current);
					}
				}
				TaxPeriod_Current.Cache.Persist(PXDBOperation.Update);

				TaxPeriod_Current.Cache.Persisted(false);

				ts.Complete(this);
			}
			APDocumentRelease.ReleaseDoc(doclist, false);
		}

		private void CheckUnreleasedTaxAdjustmentsDoNotExist(string taxPeriodID)
		{
			var unreleasedTaxAdjustment = (TaxAdjustment)PXSelect<TaxAdjustment,
															Where<TaxAdjustment.taxPeriod, Equal<Required<TaxAdjustment.taxPeriod>>,
																	And<TaxAdjustment.released, Equal<False>>>>
															.Select(this, taxPeriodID);

			if (unreleasedTaxAdjustment != null)
			{
				throw new PXException(Messages.TaxReportCannotBeReleased);
			}
		}

		public class ErrorNotifications
		{
			private List<string> _errorMessages = new List<string> { };

			public void AddMessage(string message, params object[] args)
			{
				_errorMessages.Add(PXMessages.LocalizeFormatNoPrefix(message, args));
			}

			public string Message
			{
				get { return String.Join(" ", _errorMessages.ToArray()); }
			}

			public void RaiseIfAny()
			{
				if(_errorMessages.Any())
					throw new PXException(Message);
			}
		}

		protected virtual ProcessingResult VerifyTaxConfigurationErrors(TaxPeriodFilter filter, TaxPeriod taxPeriod, Vendor vendor)
		{
			Dictionary<string, HashSet<string>> unreportedTaxes = new Dictionary<string, HashSet<string>>();
			HashSet<string> misconfiguredTaxes = new HashSet<string>();

			var validationResult = new ProcessingResult();

			foreach (PXResult<Branch, TaxHistory, TaxTranReport, TaxBucketLine> taxTrans in PXSelectJoin<
				Branch,
					LeftJoin<TaxHistory,
						On<TaxHistory.branchID, Equal<Branch.branchID>,
							And<TaxHistory.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
								And<TaxHistory.taxPeriodID, Equal<Current<TaxPeriodFilter.taxPeriodID>>,
									And<TaxHistory.revisionID, Equal<Current<TaxPeriodFilter.revisionId>>>>>>,
						LeftJoin<TaxTranReport,
							On<TaxTranReport.branchID, Equal<Branch.branchID>,
								And<TaxTranReport.vendorID, Equal<Current<TaxPeriodFilter.vendorID>>,
								And<TaxTranReport.released, Equal<True>,
								And<TaxTranReport.voided, Equal<False>,
								And<TaxTranReport.taxPeriodID, IsNull,
								And<TaxTranReport.origRefNbr, Equal<Empty>,
								And<TaxTranReport.taxType, NotEqual<TaxType.pendingPurchase>,
								And<TaxTranReport.taxType, NotEqual<TaxType.pendingSales>,
						And2<Where<
							Current<Vendor.taxReportFinPeriod>, Equal<True>,
											  Or<TaxTranReport.tranDate, Less<Current<TaxPeriod.endDate>>>>,
						And<Where<
							Current<Vendor.taxReportFinPeriod>, Equal<False>,
												Or<TaxTranReport.finDate, Less<Current<TaxPeriod.endDate>>>>>>>>>>>>>>,
						LeftJoin<TaxBucketLine,
								On<TaxBucketLine.vendorID, Equal<TaxTranReport.vendorID>,
									And<TaxBucketLine.bucketID, Equal<TaxTranReport.taxBucketID>>>>>>,
				Where<
					TaxHistory.taxPeriodID, IsNull,
						And<TaxTranReport.refNbr, IsNotNull>>>
					.SelectMultiBound(this, new object[] { filter, taxPeriod, vendor }))
			{
				Branch branch = taxTrans;
				TaxTranReport tran = taxTrans;
				TaxBucketLine bucketLine = taxTrans;
				if (bucketLine.VendorID.HasValue)
				{
					HashSet<string> taxIds;
					if (!unreportedTaxes.TryGetValue(branch.BranchCD, out taxIds))
					{
						taxIds = new HashSet<string>();
						unreportedTaxes[branch.BranchCD] = taxIds;
					}
					taxIds.Add(tran.TaxID);
				}
				else
				{
					misconfiguredTaxes.Add(tran.TaxID);
				}
			}

			if (unreportedTaxes.Count != 0 || misconfiguredTaxes.Count != 0)
			{
				validationResult.AddErrorMessage(Messages.CannotClosePeriod);
				foreach (var kvp in unreportedTaxes)
				{
					var taxIDs = String.Join(", ", kvp.Value.ToArray());
					validationResult.AddErrorMessage(Messages.ThereAreUnreportedTrans, taxIDs, kvp.Key.Trim());
				}

				if (misconfiguredTaxes.Count != 0)
				{
					var taxIDs = String.Join(", ", misconfiguredTaxes.ToArray());
					validationResult.AddErrorMessage(Messages.WrongTaxConfig, taxIDs);
					validationResult.AddErrorMessage(Messages.CheckTaxConfig);
				}
			}

			return validationResult;
		}

		public virtual string FileTaxProc(TaxPeriodFilter p)
		{
			var validationResult = new ProcessingResult();
				using (new PXConnectionScope())
				{
					using (PXTransactionScope ts = new PXTransactionScope())
					{
						if (CheckForUnprocessedPPD(this, p.VendorID, p.EndDate))
						{
							throw new PXSetPropertyException(AR.Messages.UnprocessedPPDExists, PXErrorLevel.Error);
						}
						
						TaxPeriod taxper = TaxPeriod_Current.SelectSingle(p.VendorID, p.TaxPeriodID);

						RoundingManager rmanager = new RoundingManager(this, taxper.VendorID);
						Company company = new PXSetup<Company>(this).Current;

						int revisionId = TaxRevisionManager.CurrentRevisionId(this, taxper.VendorID, taxper.TaxPeriodID) ?? 1;
						
						if (taxper.Status == TaxPeriodStatus.Closed)
							revisionId += 1;

						Vendor vendor = PXSelect<Vendor,
							Where<Vendor.bAccountID, Equal<Current<TaxPeriod.vendorID>>>>
							.SelectSingleBound(this, new object[] { taxper });

						string reportCuryID = rmanager.CurrentVendor.CuryID ?? company.BaseCuryID;
						PXDatabase.Update<TaxPeriod>(
							new PXDataFieldAssign("Status", TaxPeriodStatus.Closed),
							new PXDataFieldRestrict("VendorID", PXDbType.Int, 4, taxper.VendorID, PXComp.EQ),
							new PXDataFieldRestrict("TaxYear", PXDbType.Char, 4, taxper.TaxYear, PXComp.EQ),
							new PXDataFieldRestrict("TaxPeriodID", PXDbType.Char, 6, taxper.TaxPeriodID, PXComp.LT)
							);
					

						using (new PXReadBranchRestrictedScope(p.BranchID))
						{
                            this.Defaults[typeof(Vendor)] = () => { return vendor; };

							PXUpdateJoin<Set<TaxTran.taxBucketID, TaxRev.taxBucketID,
								Set<TaxTran.vendorID, TaxRev.taxVendorID>>,
								TaxTran,
								InnerJoin<TaxRev,
									On<TaxRev.taxID, Equal<TaxTran.taxID>,
										And<TaxRev.taxType, Equal<TaxTran.taxType>,
										And<TaxTran.tranDate, Between<TaxRev.startDate, TaxRev.endDate>,
										And<TaxRev.outdated, Equal<False>>>>>>,
								Where<TaxRev.taxVendorID, Equal<Required<TaxPeriod.vendorID>>,
									And<TaxTran.released, Equal<True>,
									And<TaxTran.voided, Equal<False>,
									And<TaxTran.taxPeriodID, IsNull,
									And<TaxTran.origRefNbr, Equal<StringEmpty>,
									And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
									And<TaxTran.taxType, NotEqual<TaxType.pendingSales>>>>>>>>>
								.Update(this, taxper.VendorID);

							PXResultset<TaxTran> taxTransWOTaxRevs = PXSelectJoin<TaxTran, LeftJoin<TaxRev,
								On<TaxRev.taxID, Equal<TaxTran.taxID>,
									And<TaxRev.taxType, Equal<TaxTran.taxType>,
									And<TaxTran.tranDate, Between<TaxRev.startDate, TaxRev.endDate>,
									And<TaxRev.outdated, Equal<False>>>>>>,
								Where<TaxRev.revisionID, IsNull,
									And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
									And<TaxTran.taxType, NotEqual<TaxType.pendingSales>>>>>.Select(this);
 
							PXUpdateJoin<Set<TaxTran.taxPeriodID, Required<TaxPeriod.taxPeriodID>,
								Set<TaxTran.revisionID, Required<TaxTran.revisionID>,
								Set<TaxTran.reportTaxAmt, Switch<
										Case<Where<TaxTran.curyID, Equal<Currency.curyID>>, TaxTran.curyTaxAmt,
										Case<Where<Currency.curyID, Equal<Required<TaxTran.curyID>>>, TaxTran.taxAmt,
										Case<Where<CurrencyRateByDate.curyMultDiv, Equal<CuryMultDivType.mult>>, Round<Mult<TaxTran.curyTaxAmt, CurrencyRateByDate.curyRate>, Currency.decimalPlaces>,
										Case<Where<CurrencyRateByDate.curyMultDiv, Equal<CuryMultDivType.div>>, Round<Div<TaxTran.curyTaxAmt, CurrencyRateByDate.curyRate>, Currency.decimalPlaces>>>>>>,
								Set<TaxTran.reportTaxableAmt, Switch<
										Case<Where<TaxTran.curyID, Equal<Currency.curyID>>, TaxTran.curyTaxableAmt,
										Case<Where<Currency.curyID, Equal<Required<TaxTran.curyID>>>, TaxTran.taxableAmt,
										Case<Where<CurrencyRateByDate.curyMultDiv, Equal<CuryMultDivType.mult>>, Round<Mult<TaxTran.curyTaxableAmt, CurrencyRateByDate.curyRate>, Currency.decimalPlaces>,
										Case<Where<CurrencyRateByDate.curyMultDiv, Equal<CuryMultDivType.div>>, Round<Div<TaxTran.curyTaxableAmt, CurrencyRateByDate.curyRate>, Currency.decimalPlaces>>>>>>,
								Set<TaxTran.reportCuryID, Currency.curyID,
								Set<TaxTran.reportCuryRateTypeID, CurrencyRateByDate.curyRateType,
								Set<TaxTran.reportCuryEffDate, CurrencyRateByDate.curyEffDate,
								Set<TaxTran.reportCuryRate, Switch<Case<Where<CurrencyRateByDate.curyRate, IsNotNull>, CurrencyRateByDate.curyRate>, decimal1>,
								Set<TaxTran.reportCuryMultDiv, Switch<Case<Where<CurrencyRateByDate.curyMultDiv, IsNotNull>, CurrencyRateByDate.curyMultDiv>, CuryMultDivType.mult>>>>>>>>>>,
								TaxTran,
								InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, Equal<TaxTran.finPeriodID>>, 
								InnerJoin<TaxBucketLine,
										On<TaxTran.vendorID, Equal<TaxBucketLine.vendorID>,
											And<TaxTran.taxBucketID, Equal<TaxBucketLine.bucketID>>>,
										LeftJoin<Currency,
											On<Currency.curyID, Equal<Required<Currency.curyID>>>,
											LeftJoin<CurrencyRateByDate,
												On<CurrencyRateByDate.fromCuryID, Equal<TaxTran.curyID>,
                                                And<CurrencyRateByDate.toCuryID, Equal<Required<CurrencyRateByDate.toCuryID>>,
												And<CurrencyRateByDate.curyRateType, Equal<Required<CurrencyRateByDate.curyRateType>>,
												And<CurrencyRateByDate.curyEffDate, Equal<TaxTran.curyEffDate>>>>>>>>>,
								Where<TaxTran.vendorID, Equal<Required<TaxPeriod.vendorID>>,
									And<TaxTran.released, Equal<True>,
									And<TaxTran.voided, Equal<False>,
									And<TaxTran.taxPeriodID, IsNull,
									And<TaxTran.origRefNbr, Equal<Empty>,
									And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
									And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
                                    And2<Where<Required<Vendor.taxReportFinPeriod>, Equal<True>,
                                                    Or<TaxTran.tranDate, Less<Required<TaxPeriod.endDate>>>>,
                                                And<Where<Required<Vendor.taxReportFinPeriod>, Equal<False>,
                                                 Or<Sub<FinPeriod.endDate, int1>, Less<Required<TaxPeriod.endDate>>>>>>>>>>>>>>
								.Update(this, taxper.TaxPeriodID, revisionId,
                                                company.BaseCuryID, company.BaseCuryID, reportCuryID, reportCuryID, rmanager.CurrentVendor.CuryRateTypeID, taxper.VendorID,
												vendor.TaxReportFinPeriod, taxper.EndDate, vendor.TaxReportFinPeriod, taxper.EndDate);

							PXResultset<TaxTran> unreportedTaxTrans = PXSelectJoin<TaxTran, InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, Equal<TaxTran.finPeriodID>>>, 
								Where<TaxTran.vendorID, Equal<Required<TaxPeriod.vendorID>>,
								And<TaxTran.released, Equal<True>,
								And<TaxTran.voided, Equal<False>,
								And<TaxTran.taxPeriodID, IsNull,
								And<TaxTran.origRefNbr, Equal<StringEmpty>,
								And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
								And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
								And2<Where<Required<Vendor.taxReportFinPeriod>, Equal<True>,
									Or<TaxTran.tranDate, Less<Required<TaxPeriod.endDate>>>>,
									And2<Where<Required<Vendor.taxReportFinPeriod>, Equal<False>,
										Or<Sub<FinPeriod.endDate, int1>, Less<Required<TaxPeriod.endDate>>>>,
											And<TaxTran.taxPeriodID, IsNull>>>>>>>>>>>.Select(this, taxper.VendorID,
												vendor.TaxReportFinPeriod, taxper.EndDate, vendor.TaxReportFinPeriod, taxper.EndDate);

							PXUpdateJoin<Set<TaxAdjustment.taxPeriod, TaxTran.taxPeriodID>,
										TaxAdjustment,
											InnerJoin<TaxTran,
												On<TaxAdjustment.docType, Equal<TaxTran.tranType>,
													And<TaxAdjustment.refNbr, Equal<TaxTran.refNbr>>>>,
										Where<TaxAdjustment.taxPeriod, NotEqual<TaxTran.taxPeriodID>,
												And<TaxAdjustment.released, Equal<True>>>>
										.Update(this);

							TaxTran rateNotFound =
								PXSelect<TaxTran,
									Where<TaxTran.taxPeriodID, Equal<Required<TaxTran.taxPeriodID>>,
										And<TaxTran.vendorID, Equal<Required<TaxTran.vendorID>>,
										And<TaxTran.revisionID, Equal<Required<TaxTran.revisionID>>,
										And<TaxTran.taxType, NotEqual<TaxType.pendingPurchase>,
										And<TaxTran.taxType, NotEqual<TaxType.pendingSales>,
										And<Where<TaxTran.reportTaxAmt, IsNull,
											Or<TaxTran.reportTaxableAmt, IsNull>>>>>>>>,
								OrderBy<Asc<TaxTran.tranDate>>>.SelectWindowed(this, 0, 1, taxper.TaxPeriodID, taxper.VendorID, revisionId);

							if (rateNotFound != null)
								throw new PXException(Messages.TaxReportRateNotFound, rateNotFound.CuryID, reportCuryID, rateNotFound.TranDate.Value.ToShortDateString());

							if (taxTransWOTaxRevs.Count != 0 || unreportedTaxTrans.Count != 0)
							{
								validationResult.AddErrorMessage(AP.Messages.Warning);
								validationResult.AddErrorMessage(":");

								if (taxTransWOTaxRevs.Count != 0)
								{
									Dictionary<String, HashSet<string>> misconfiguredTaxes = new Dictionary<string, HashSet<string>>();
									foreach (TaxTran tran in taxTransWOTaxRevs)
									{
										HashSet<string> taxTypes;
										if (!misconfiguredTaxes.TryGetValue(tran.TaxID, out taxTypes))
										{
											taxTypes = new HashSet<string>();
											misconfiguredTaxes[tran.TaxID] = taxTypes;
										}
										taxTypes.Add(tran.TaxType);
									}

									foreach (var kvp in misconfiguredTaxes)
									{
										foreach (var taxType in kvp.Value)
										{
											validationResult.AddErrorMessage(Messages.NoTaxRevForTaxType, kvp.Key,
												PXMessages.LocalizeNoPrefix(GetLabel.For<TaxType>(taxType)));
										}
									}
								}

								if (unreportedTaxTrans.Count != 0)
								{
									var taxesString = String.Join(",", unreportedTaxTrans
										.RowCast<TaxTran>()
										.Select(tran => tran.TaxID)
										.Distinct()
										.OrderBy(x => x).ToArray());
									validationResult.AddErrorMessage(Messages.WrongTaxConfig, taxesString);

								}
								validationResult.AddErrorMessage(Messages.CheckTaxConfig);
							}

							TaxHistory prev_hist = null;
							foreach (TaxPeriod res in TaxPeriod_Current.Select(taxper.VendorID, taxper.TaxPeriodID))
							{
								res.Status = TaxPeriodStatus.Prepared;
								TaxPeriod_Current.Cache.Update(res);
							}

							foreach (PXResult<TaxReportLine, TaxBucketLine, TaxTranReport> res 
								in Period_Details_Expanded.Select(revisionId, taxper.TaxPeriodID, taxper.VendorID)
									.Where(line => ShowTaxReportLine(line.GetItem<TaxReportLine>(), taxper.TaxPeriodID)))
							{
								TaxReportLine line = res;
								TaxTranReport tran = res;


								if (prev_hist == null ||
								    object.Equals(prev_hist.BranchID, tran.BranchID) == false ||
								    object.Equals(prev_hist.AccountID, tran.AccountID) == false ||
								    object.Equals(prev_hist.SubID, tran.SubID) == false ||
								    object.Equals(prev_hist.TaxID, tran.TaxID) == false ||
								    object.Equals(prev_hist.LineNbr, line.LineNbr) == false)
								{
									if (prev_hist != null)
									{
										TaxHistory_Current.Cache.Update(prev_hist);
									}

									prev_hist = TaxHistory_Current.Select(tran.VendorID, tran.BranchID, tran.AccountID, tran.SubID, tran.TaxID, tran.TaxPeriodID, line.LineNbr, revisionId);
									if (prev_hist == null)
									{
										prev_hist = new TaxHistory();
										prev_hist.VendorID = tran.VendorID;
										prev_hist.BranchID = tran.BranchID;
										prev_hist.TaxPeriodID = tran.TaxPeriodID;
										prev_hist.AccountID = tran.AccountID;
										prev_hist.SubID = tran.SubID;
										prev_hist.TaxID = tran.TaxID;
										prev_hist.LineNbr = line.LineNbr;
										prev_hist.CuryID = rmanager.CurrentVendor.CuryID ?? company.BaseCuryID;

										prev_hist.FiledAmt = 0m;
										prev_hist.UnfiledAmt = 0m;
										prev_hist.ReportFiledAmt = 0m;
										prev_hist.ReportUnfiledAmt = 0m;
										prev_hist.RevisionID = revisionId;

                                    prev_hist = (TaxHistory)TaxHistory_Current.Cache.Insert(prev_hist);
									}
								}

								decimal HistMult = GetMult(tran.Module, tran.TranType, tran.TaxType, line.LineMult);

								switch (line.LineType)
								{
									case "P":
                                    prev_hist.FiledAmt += HistMult * tran.TaxAmt.GetValueOrDefault();
                                    prev_hist.ReportFiledAmt += HistMult * tran.ReportTaxAmt.GetValueOrDefault();
										break;
									case "A":
                                    prev_hist.FiledAmt += HistMult * tran.TaxableAmt.GetValueOrDefault();
                                    prev_hist.ReportFiledAmt += HistMult * tran.ReportTaxableAmt.GetValueOrDefault();
										break;
								}
								if (line.TempLine == true || line.TempLineNbr != null && prev_hist.FiledAmt.GetValueOrDefault() == 0)
								{
									TaxHistory_Current.Cache.Delete(prev_hist);
									prev_hist = null;
								}
							}
							if (prev_hist != null)
							{
								TaxHistory_Current.Cache.Update(prev_hist);
							}
						}
						TaxPeriod_Current.Cache.Persist(PXDBOperation.Insert);
						TaxPeriod_Current.Cache.Persist(PXDBOperation.Update);

						TaxHistory_Current.Cache.Persist(PXDBOperation.Insert);
						TaxHistory_Current.Cache.Persist(PXDBOperation.Update);

						TaxPeriod_Current.Cache.Persisted(false);
						TaxHistory_Current.Cache.Persisted(false);

                        TaxHistorySumManager.UpdateTaxHistorySums(this, rmanager, taxper.TaxPeriodID, revisionId, p.BranchID, 
							(line) => ShowTaxReportLine(line, taxper.TaxPeriodID));
						ts.Complete(this);					
					}
				}
				return validationResult.GetGeneralMessage();
		}

		public virtual bool ShowTaxReportLine(TaxReportLine taxReportLine, string taxPeriodID)
		{
			return true;
		}

		public static bool CheckForUnprocessedPPD(PXGraph graph, int? vendorID, DateTime? endDate)
		{
			bool exist = false;

			Tax tax = PXSelect<Tax, Where<Tax.taxType, Equal<CSTaxType.vat>,
				And<Tax.taxApplyTermsDisc, Equal<CSTaxTermsDiscount.toPromtPayment>,
				And<Tax.taxVendorID, Equal<Required<Tax.taxVendorID>>>>>>.SelectSingleBound(graph, null, vendorID);
			if (tax != null)
			{
				ARInvoice doc = PXSelectJoin<ARInvoice,
					InnerJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
						And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
						And<ARAdjust.released, Equal<True>,
						And<ARAdjust.voided, NotEqual<True>,
						And<ARAdjust.pendingPPD, Equal<True>,
						And<ARAdjust.adjgDocDate, LessEqual<Required<ARAdjust.adjgDocDate>>>>>>>>>,
					Where<ARInvoice.pendingPPD, Equal<True>,
						And<ARInvoice.released, Equal<True>,
						And<ARInvoice.openDoc, Equal<True>>>>>.SelectSingleBound(graph, null, endDate);
				exist = doc != null;
			}

			return exist;
		}

		public static bool CheckForUnprocessedSVAT(PXGraph graph, int? branchID, Vendor vendor, DateTime? endDate)
		{
			bool result = false;
			if (PXAccess.FeatureInstalled<FeaturesSet.vATReporting>() &&
				branchID != null && vendor?.BAccountID != null && endDate != null)
			{
				PXSelectBase<SVATConversionHist> select = vendor.TaxReportFinPeriod == true
				? new PXSelectJoin<SVATConversionHist,
					InnerJoin<FinPeriod, On<FinPeriod.startDate, LessEqual<SVATConversionHist.adjdDocDate>,
						And<FinPeriod.endDate, Greater<SVATConversionHist.adjdDocDate>,
						And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>>>,
					Where<FinPeriod.endDate, LessEqual<Required<FinPeriod.endDate>>>>(graph)
				: (PXSelectBase<SVATConversionHist>)new PXSelect<SVATConversionHist,
					Where<SVATConversionHist.adjdDocDate, LessEqual<Required<FinPeriod.endDate>>>>(graph);

				select.WhereAnd<Where<SVATConversionHist.processed, NotEqual<True>,
					And<SVATConversionHist.adjdBranchID, Equal<Required<SVATConversionHist.adjdBranchID>>,
					And<SVATConversionHist.vendorID, Equal<Required<SVATConversionHist.vendorID>>,
					And<SVATConversionHist.reversalMethod, Equal<SVATTaxReversalMethods.onPayments>,
					And<Where<SVATConversionHist.adjdDocType, NotEqual<SVATConversionHist.adjgDocType>,
						Or<SVATConversionHist.adjdRefNbr, NotEqual<SVATConversionHist.adjgRefNbr>>>>>>>>>();

				SVATConversionHist hist = select.SelectSingle(endDate.Value.AddDays(-1), branchID, vendor.BAccountID);
				result = hist != null;
			}

			return result;
		}
	}
}

