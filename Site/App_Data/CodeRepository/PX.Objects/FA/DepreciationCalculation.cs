using System;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL;
using System.Collections;
using System.Collections.Generic;
using FABookHist = PX.Objects.FA.Overrides.AssetProcess.FABookHist;

namespace PX.Objects.FA
{
	public class DepreciationCalculation : PXGraph<DepreciationCalculation, FADepreciationMethod>, IDepreciationCalculation
	{
		protected int _Precision = 2;
		protected int monthsInYear = 12;
		protected int quarterToMonth = 3;
		protected int quartersCount = 4;

		protected FADestination _Destination = FADestination.Calculated;

		public enum FADestination
		{
			Calculated,
			Tax179,
			Bonus
		}
		
		#region Selects Declaration

		public PXSelect<FADepreciationMethod> DepreciationMethod;
		public PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Current<FADepreciationMethod.methodID>>>> 	DepreciationMethodLines;
		public PXSetup<FASetup> FASetup;
		public PXSelect<FABookHistory> RawHistory;
		public PXSelect<FABookHist> BookHistory;
		public PXSelect<FABookBalance> BookBalance;
		
		
		#endregion

		#region Constructor
		public DepreciationCalculation() 
		{
			FASetup setup = FASetup.Current;
			PXCache cache = DepreciationMethodLines.Cache;
			cache.AllowDelete = false;
			cache.AllowInsert = false;			
			PXCache methodCache = DepreciationMethod.Cache;
			PXDBCurrencyAttribute.SetBaseCalc<FADepreciationMethod.totalPercents> (methodCache, null, true);

			Currency cury = PXSelectJoin<Currency, InnerJoin<GL.Company, On<GL.Company.baseCuryID, Equal<Currency.curyID>>>>.Select(this);
			_Precision = cury.DecimalPlaces ?? 4;
		}

		public override void Clear()
		{
			PXDBBaseCuryAttribute.EnsurePrecision(this.Caches[typeof(FABookHist)]);
			base.Clear();
		}

		public decimal Round(decimal value)
		{
			return Math.Round(value, _Precision, MidpointRounding.AwayFromZero);
		}
		#endregion
		
		#region Buttons
		#region Button Create Lines
		public PXAction<FADepreciationMethod> CalculatePercents;
		[PXUIField(DisplayName = "Calculate Percents", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable calculatePercents(PXAdapter adapter)
		{
			FADepreciationMethod method = DepreciationMethod.Current;
			CalculateLinesPercents(method); 
			return adapter.Get();
		}
		#endregion
		#endregion

		#region Events Lines
		protected virtual void FADepreciationMethodLines_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;

			if (line.MethodID != header.MethodID && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				line.MethodID = header.MethodID;
			}
		}
		protected virtual void FADepreciationMethodLines_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;

			if (line.MethodID != header.MethodID && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				line.MethodID = header.MethodID;
			}
		}
		protected virtual void FADepreciationMethodLines_RatioPerYear_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth1_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth2_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth3_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth4_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth5_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth6_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth7_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth8_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth9_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth10_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth11_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		protected virtual void FADepreciationMethodLines_RatioPerMonth12_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod) DepreciationMethod.Current;
			FADepreciationMethodLines line = (FADepreciationMethodLines)e.Row;
			if (header == null || line == null) return;
			SetMethodTotalPercents(header, sender.Locate(e.Row) == null ? line : null);
		}
		#endregion
		
		#region Events Headers
		protected virtual void FADepreciationMethod_RecoveryPeriod_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
			CreateMethodLines(header);
		}

		protected virtual void FADepreciationMethod_DepreciationStartDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
			CreateMethodLines(header);
		}

		protected virtual void FADepreciationMethod_AveragingConvention_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
			CreateMethodLines(header);
		}

		protected virtual void FADepreciationMethod_DepreciationStopDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
			CreateMethodLines(header);
		}

		protected virtual void FADepreciationMethod_DepreciationPeriodsInYear_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
		}

		protected virtual void FADepreciationMethod_YearlyAccountancy_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			ClearMethodLines(header);
			PXCache cache = DepreciationMethodLines.Cache;
			foreach(FADepreciationMethodLines line in PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>>>.Select(this, header.MethodID))
			{
				FADepreciationMethodLines newLine = (FADepreciationMethodLines) cache.CreateCopy(line);	
				newLine.SetYearRatioByUser = (header.IsTableMethod == true) && (header.YearlyAccountancy == true);
				newLine = (FADepreciationMethodLines) cache.Update(newLine);
			}
		}

		protected virtual void FADepreciationMethod_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FADepreciationMethod header = (FADepreciationMethod)e.Row;
			if (header == null) return;
			CalculatePercents.SetEnabled(header.RecoveryPeriod != 0m);
			bool halfYear	= header.DepreciationPeriodsInYear == 2;
			bool quarter	= header.DepreciationPeriodsInYear == 4;
			bool month		= header.DepreciationPeriodsInYear == 12;
				
			PXCache cache = DepreciationMethodLines.Cache;
			PXUIFieldAttribute.SetEnabled<FADepreciationMethodLines.ratioPerYear>	 (cache, null, header.YearlyAccountancy == true && header.IsTableMethod == true);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod1> (cache, null, halfYear || quarter || month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod2> (cache, null, halfYear || quarter || month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod3> (cache, null, quarter || month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod4> (cache, null, quarter || month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod5> (cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod6> (cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod7> (cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod8> (cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod9> (cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod10>(cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod11>(cache, null, month);
			PXUIFieldAttribute.SetVisible<FADepreciationMethodLines.ratioPerPeriod12>(cache, null, month);
		}	
		#endregion

		#region Functions
		private void CreateMethodLines(FADepreciationMethod method)
		{
			int wholeRecoveryPeriods  = method.RecoveryPeriod ?? 0;
			int count = 0;
			DateTime depreciationStartDate = method.DepreciationStartDate ?? DateTime.Now;

			FinPeriod depreciationStartBookPeriod	= (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>, And<FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>>>>.Select(this, depreciationStartDate, depreciationStartDate);

			if (depreciationStartBookPeriod == null)
				throw new PXException(Messages.FinPeriodsNotDefined, depreciationStartDate.Year);
				
			int depreciationStartPeriod;
			int.TryParse(depreciationStartBookPeriod.PeriodNbr, out depreciationStartPeriod);

			int financialYears = DepreciationCalc.GetFinancialYears(wholeRecoveryPeriods, depreciationStartPeriod, method.DepreciationPeriodsInYear ?? 12, ((depreciationStartDate - depreciationStartBookPeriod.StartDate.Value).Days == 0));
	
			foreach (FADepreciationMethodLines existLine in PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>>>.Select(this, method.MethodID))
			{
				count++;
				if (count > financialYears )
				{
					DepreciationMethodLines.Delete(existLine);	
				}
			}
			if(count < financialYears)
			{
				for(int i = count + 1; i <= financialYears; i++)
				{	
					FADepreciationMethodLines line = new FADepreciationMethodLines();
					line.MethodID = method.MethodID;
					line.Year = i;
					line.RatioPerYear = 0m;
					line = (FADepreciationMethodLines) DepreciationMethodLines.Insert(line);
				}
			}
			SetMethodTotalPercents(method, null);
		}

		private void ClearMethodLines(FADepreciationMethod method)
		{
			foreach (FADepreciationMethodLines line in
				PXSelect<FADepreciationMethodLines,Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>>>.Select(this, method.MethodID)
					.RowCast<FADepreciationMethodLines>()
					.Select(existLine => (FADepreciationMethodLines) DepreciationMethodLines.Cache.CreateCopy(existLine)))
			{
				line.RatioPerPeriod1 = null;
				line.RatioPerPeriod2 = null;
				line.RatioPerPeriod3 = null;	
				line.RatioPerPeriod4 = null;
				line.RatioPerPeriod5 = null;
				line.RatioPerPeriod6 = null;	
				line.RatioPerPeriod7 = null;
				line.RatioPerPeriod8 = null;
				line.RatioPerPeriod9 = null;	
				line.RatioPerPeriod10 = null;
				line.RatioPerPeriod11 = null;
				line.RatioPerPeriod12 = null;	
				DepreciationMethodLines.Update(line);
			}
		}

		private void CalculateLinesPercents(FADepreciationMethod method)
		{
			string depreciationMethod		= method.DepreciationMethod ?? "";
			bool yearlyAccountancy			= method.YearlyAccountancy ?? false;
			bool isTableMethod				= method.IsTableMethod ?? false;
			decimal multiPlier				= method.DBMultiPlier ?? 0m;
			bool switchToSL					= method.SwitchToSL ?? false;
			int bookID						= method.BookID ?? 0;

			if (depreciationStartDate		== null			||
				method.RecoveryPeriod		== 0			|| 
				bookID						== 0			||
				string.IsNullOrEmpty(depreciationMethod)	|| 
				depreciationMethod == FADepreciationMethod.depreciationMethod.DecliningBalance && isTableMethod != true && multiPlier == 0m
				) return;
					
			FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Required<FABook.bookID>>>>.Select(this, bookID);
			DepreciationCalc.SetParameters(this, this, bookID, true, method.DepreciationStartDate.Value, method.DepreciationStopDate, null,
				method, method.AveragingConvention, book.MidMonthType, book.MidMonthDay, method.UsefulLife ?? 0m);

			depreciationBasis				= 1m;				
			FADepreciationMethodLines line;
			int count						= 0;
			decimal otherDepreciation		= 0m;
			decimal lastDepreciation		= 0m;
			decimal	rounding				= 0;
			DateTime previousEndDate		= DateTime.MinValue;
			previousCalculatedPeriods = 0;

			foreach (FADepreciationMethodLines existLine in PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>>>.Select(this, method.MethodID))
			{
				count++;
				if (count <= depreciationYears )
				{
					line = (FADepreciationMethodLines) DepreciationMethodLines.Cache.CreateCopy(existLine);
					SetLinePercents(isTableMethod, false, yearlyAccountancy, line, count, null, bookID, depreciationMethod, multiPlier, switchToSL, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);
					DepreciationMethodLines.Update(line);
				}
				else
				{
					DepreciationMethodLines.Delete(existLine);	
				}
			}

			if(count < depreciationYears)
			{
				for(int i = count + 1; i <= depreciationYears; i++)
				{
					line = new FADepreciationMethodLines
					{
						MethodID = method.MethodID,
						Year = i
					};
					SetLinePercents(isTableMethod, false, yearlyAccountancy, line, i, null, bookID, depreciationMethod, multiPlier, switchToSL, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);
					DepreciationMethodLines.Insert(line);
				}
			}

			SetMethodTotalPercents(method, null);
		}

		Dictionary<string, FABookHist> histdict;

		public static bool UseAcceleratedDepreciation(FixedAsset cls, FADepreciationMethod method)
		{
			return (cls.AcceleratedDepreciation == true && method != null && method.IsPureStraightLine) ||
				(method != null && method.IsTableMethod != true && method.DepreciationMethod == FADepreciationMethod.depreciationMethod.RemainingValue);
		}
			
		public virtual void CalculateDepreciationAddition(FixedAsset cls, FABookBalance bookbal, FADepreciationMethod method, FABookHistory next)
		{
			string bookDeprFromPeriod = bookbal.DeprFromPeriod;
			string additionDeprFromPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(this, next.FinPeriodID, -(next.YtdReversed ?? 0), next.BookID);

			DateTime? additionEndDate = null;
			if (UseAcceleratedDepreciation(cls, method) && string.CompareOrdinal(additionDeprFromPeriod, bookDeprFromPeriod) > 0)
			{
				DepreciationCalc.SetParameters(this, this, bookbal, method);
				additionEndDate = recoveryEndDate;
			}

			bookbal.DeprFromPeriod = additionDeprFromPeriod;
			bookbal.DeprFromDate = (additionDeprFromPeriod == bookDeprFromPeriod) ? bookbal.DeprFromDate : FABookPeriodIDAttribute.PeriodStartDate(this, additionDeprFromPeriod, bookbal.BookID);
			bookbal.DeprToPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(this, bookbal.DeprToPeriod, -(bookbal.YtdSuspended ?? 0), bookbal.BookID);

			if (bookbal.DeprToDate != null)
			{
				FABookPeriod per1 = FABookPeriodIDAttribute.FABookPeriodFromDate(this, bookbal.DeprToDate, bookbal.BookID);
				FABookPeriod per2 = DepreciationCalc.GetBookPeriod(this, bookbal.DeprToPeriod, bookbal.BookID);

				if (Equals(bookbal.DeprToDate, ((DateTime)per1.EndDate).AddDays(-1)))
				{
					bookbal.DeprToDate = ((DateTime)per2.EndDate).AddDays(-1);
				}
				else
				{
					int days = ((TimeSpan)(bookbal.DeprToDate - per1.StartDate)).Days;
					bookbal.DeprToDate = days < ((TimeSpan)(per2.EndDate - per2.StartDate)).Days ? ((DateTime)per2.StartDate).AddDays(days) : per2.EndDate;
				}

				FABookBalance bookbal2 = PXCache<FABookBalance>.CreateCopy(bookbal);
				bookbal2.DeprToDate = null;

				DepreciationCalc.SetParameters(this, this, bookbal2, method, additionEndDate);

				if (DateTime.Compare(recoveryEndDate, (DateTime)bookbal.DeprToDate) < 0)
				{
					bookbal.DeprToDate = recoveryEndDate;
					bookbal.DeprToPeriod = FABookPeriodIDAttribute.PeriodFromDate(this, recoveryEndDate, bookbal.BookID);
				}
			}

			bookbal.AcquisitionCost = Round((decimal)(next.PtdDeprBase * bookbal.BusinessUse * 0.01m));
			bookbal.AcquisitionCost -= (additionDeprFromPeriod == bookDeprFromPeriod) ? bookbal.Tax179Amount : 0m;
			bookbal.AcquisitionCost -= (additionDeprFromPeriod == bookDeprFromPeriod) ? bookbal.BonusAmount : 0m;
			bookbal.SalvageAmount = (additionDeprFromPeriod == bookDeprFromPeriod) ? bookbal.SalvageAmount : 0m;

			_Destination = FADestination.Calculated;

			CalculateDepreciation(method, bookbal, additionEndDate);
		}

		public virtual void CalculateDepreciation(FABookBalance assetBalance)
		{
			CalculateDepreciation(assetBalance, null);
		}

		public virtual void CalculateDepreciation(FABookBalance assetBalance, string maxPeriodID)
		{
			histdict = new Dictionary<string, FABookHist>();
			foreach (FABookHist item in  PXSelectReadonly<FABookHist, Where<FABookHist.assetID, Equal<Required<FABookHist.assetID>>, And<FABookHist.bookID, Equal<Required<FABookHist.bookID>>>>>.Select(this, assetBalance.AssetID, assetBalance.BookID))
			{
				histdict[item.FinPeriodID] = item;
			}

			FADepreciationMethod method = PXSelect<FADepreciationMethod, Where<FADepreciationMethod.methodID, Equal<Required<FADepreciationMethod.methodID>>>>.Select(this, assetBalance.DepreciationMethodID);

			if (assetBalance.Depreciate == true && (string.IsNullOrEmpty(assetBalance.DeprFromPeriod) || string.IsNullOrEmpty(assetBalance.DeprToPeriod) || string.CompareOrdinal(assetBalance.DeprFromPeriod, assetBalance.DeprToPeriod) > 0))
			{
				FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Current<FABookBalance.bookID>>>>.SelectSingleBound(this, new object[] { assetBalance });
				throw new PXException(Messages.IncorrectDepreciationPeriods, book.BookCode);
			}

			// FIXME
			/*
			FAClass cls = PXSelectJoin<FAClass, 
				InnerJoin<FixedAsset, On<FAClass.assetID, Equal<FixedAsset.classID>>>, 
				Where<FixedAsset.assetID, Equal<Required<FABookBalance.assetID>>>>.Select(this, assetBalance.AssetID);
			*/

			FAClass cls = (PXResult<FixedAsset, FAClass>)PXSelectJoin<FixedAsset,
				LeftJoin<FAClass, On<FAClass.assetID, Equal<FixedAsset.classID>>>,
				Where<FixedAsset.assetID, Equal<Required<FABookBalance.assetID>>>>.Select(this, assetBalance.AssetID).FirstOrDefault();

			FADetails details = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FABookBalance.assetID>>>>.Select(this, assetBalance.AssetID);

			string minPeriod = assetBalance.DeprFromPeriod;

			if (maxPeriodID == null || string.CompareOrdinal(maxPeriodID, assetBalance.DeprToPeriod) > 0)
			{
				maxPeriodID = assetBalance.DeprToPeriod;
			}

			PXRowInserting FABookHistRowInserting = delegate(PXCache sender, PXRowInsertingEventArgs e)
			{
				FABookHist item = e.Row as FABookHist;
				if(item == null) return;

				if(string.CompareOrdinal(item.FinPeriodID, maxPeriodID) > 0)
				{
					e.Cancel = true;
				}
			};

			RowInserting.AddHandler<FABookHist>(FABookHistRowInserting);

			foreach (PXResult<FABookHistoryNextPeriod, FABookHistory> res in PXSelectReadonly2<FABookHistoryNextPeriod, InnerJoin<FABookHistory, 
																					On<FABookHistory.assetID, Equal<FABookHistoryNextPeriod.assetID>, 
																						And<FABookHistory.bookID, Equal<FABookHistoryNextPeriod.bookID>, 
																						And<FABookHistory.finPeriodID, Equal<FABookHistoryNextPeriod.nextPeriodID>>>>>, 
																					Where<FABookHistoryNextPeriod.assetID, Equal<Current<FABookBalance.assetID>>, 
																						And<FABookHistoryNextPeriod.bookID, Equal<Current<FABookBalance.bookID>>, 
																						And<FABookHistoryNextPeriod.ptdDeprBase, NotEqual<decimal0>, 
																						And<FABookHistoryNextPeriod.finPeriodID, LessEqual<Current<FABookBalance.deprToPeriod>>>>>>, 
																					OrderBy<Asc<FABookHistoryNextPeriod.finPeriodID>>>.SelectMultiBound(this, new object[] { assetBalance }))
			{
				FABookHistory next = res;
				next.PtdDeprBase = ((FABookHistoryNextPeriod)res).PtdDeprBase;

				FABookBalance bookbal = PXCache<FABookBalance>.CreateCopy(assetBalance);
				string bookDeprFromPeriod = bookbal.DeprFromPeriod;
				string additionDeprFromPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(this, next.FinPeriodID, -(next.YtdReversed ?? 0), next.BookID);

				if (string.CompareOrdinal(additionDeprFromPeriod, minPeriod) < 0)
				{
					minPeriod = additionDeprFromPeriod;
				}
				PXRowInserting AdditionInserting = delegate(PXCache sender, PXRowInsertingEventArgs e)
				{
					FABookHist item = e.Row as FABookHist;
					if (item == null) return;

					if (string.CompareOrdinal(item.FinPeriodID, additionDeprFromPeriod) < 0)
					{
						e.Cancel = true;
					}
				};

				RowInserting.AddHandler<FABookHist>(AdditionInserting);

				CalculateDepreciationAddition(cls, bookbal, method, next);

				if (additionDeprFromPeriod == bookDeprFromPeriod && bookbal.Tax179Amount > 0m)
				{
					FABookHist accuhist = new FABookHist
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						FinPeriodID = bookDeprFromPeriod
					};

					accuhist = BookHistory.Insert(accuhist);

					accuhist.PtdCalculated += bookbal.Tax179Amount;
					accuhist.YtdCalculated += bookbal.Tax179Amount;
					accuhist.PtdTax179Calculated += bookbal.Tax179Amount;
					accuhist.YtdTax179Calculated += bookbal.Tax179Amount;

					this._Destination = FADestination.Tax179;

					bookbal.AcquisitionCost = bookbal.Tax179Amount;
					bookbal.SalvageAmount = 0m;

					CalculateDepreciation(method, bookbal);
				}

				if (additionDeprFromPeriod == bookDeprFromPeriod && bookbal.BonusAmount > 0m)
				{
					FABookHist accuhist = new FABookHist
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						FinPeriodID = bookDeprFromPeriod
					};

					accuhist = BookHistory.Insert(accuhist);

					accuhist.PtdCalculated += bookbal.BonusAmount;
					accuhist.YtdCalculated += bookbal.BonusAmount;
					accuhist.PtdBonusCalculated += bookbal.BonusAmount;
					accuhist.YtdBonusCalculated += bookbal.BonusAmount;

					_Destination = FADestination.Bonus;

					bookbal.AcquisitionCost = bookbal.BonusAmount;
					bookbal.SalvageAmount = 0m;

					CalculateDepreciation(method, bookbal);
				}

				RowInserting.RemoveHandler<FABookHist>(AdditionInserting);
			}

			PXCache cache = Caches[typeof(FABookHist)];

			List<FABookHist> inserted = new List<FABookHist>((IEnumerable<FABookHist>)cache.Inserted);
			inserted.Sort((a, b) => string.CompareOrdinal(a.FinPeriodID, b.FinPeriodID));

			decimal? running = 0m;
			string lastGoodPeriodID = minPeriod;
			decimal? lastGoodVal = 0m;
			bool lastGoodFlag = true;

			foreach (FABookHist item in inserted)
			{
				item.YtdCalculated += running;
				running += item.PtdCalculated;

				FABookHist existing;
				if (histdict.TryGetValue(item.FinPeriodID, out existing))
				{
					item.PtdDepreciated = existing.PtdDepreciated;
					item.YtdDepreciated = existing.YtdDepreciated;

					if (UseAcceleratedDepreciation(cls, method) 
						&& string.Equals(existing.FinPeriodID, assetBalance.CurrDeprPeriod) 
						&& Math.Abs((decimal)item.YtdCalculated - (decimal)item.PtdCalculated - (decimal)existing.YtdDepreciated) > 0.00005m)
					{
						//previous period YtdCalculated - YtdDepreciated
						decimal? catchup = item.YtdCalculated - item.PtdCalculated - existing.YtdDepreciated;

						FABookBalance bookbal = PXCache<FABookBalance>.CreateCopy(assetBalance);

						FABookHistory next = new FABookHistory()
						{
							FinPeriodID = assetBalance.CurrDeprPeriod,
							BookID = assetBalance.BookID,
							PtdDeprBase = catchup,
							YtdSuspended = existing.YtdSuspended,
							YtdReversed = existing.YtdReversed
						};

						CalculateDepreciationAddition(cls, bookbal, method, next);

						item.YtdCalculated = existing.YtdDepreciated + item.PtdCalculated;
						running = item.YtdCalculated;
					}

					if (lastGoodFlag)
					{
						lastGoodPeriodID = item.FinPeriodID;
						if (UseAcceleratedDepreciation(cls, method) && string.CompareOrdinal(item.FinPeriodID, assetBalance.CurrDeprPeriod) < 0)
						{
							if (Math.Abs((decimal)existing.YtdDepreciated - (decimal)existing.YtdCalculated) >= 0.00005m)
							{
								lastGoodFlag = false;
								continue;
							}
							lastGoodVal = existing.YtdDepreciated;
						}
						else
						{
							if (Math.Abs((decimal)existing.YtdCalculated - (decimal)item.YtdCalculated) >= 0.00005m)
							{
								lastGoodFlag = false;
								continue;
							}
							lastGoodVal = existing.YtdCalculated;
						}
						cache.SetStatus(item, PXEntryStatus.Notchanged);
					}
				}
				else
				{
					//in case of hole found in existing depreciation mark as last good
					if (lastGoodFlag)
					{
						lastGoodPeriodID = item.FinPeriodID;
						lastGoodFlag = false;
					}
				}
			}
			RowInserting.RemoveHandler<FABookHist>(FABookHistRowInserting);

			foreach (FABookHist item in inserted)
			{
				decimal adjusted = 0m;

				if (UseAcceleratedDepreciation(cls, method) && string.CompareOrdinal(item.FinPeriodID, assetBalance.CurrDeprPeriod) < 0)
				{
					item.PtdCalculated = item.PtdDepreciated;

				}

				if (UseAcceleratedDepreciation(cls, method) && string.CompareOrdinal(item.FinPeriodID, lastGoodPeriodID) >= 0)
				{
					FABookHist existing;
					if (histdict.TryGetValue(item.FinPeriodID, out existing))
					{
						adjusted = existing.PtdAdjusted ?? 0m;
					}
				}

				item.YtdCalculated = item.PtdCalculated + adjusted;
				item.PtdDepreciated = 0m;
				item.YtdDepreciated = 0m;
			}

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				string receiptPeriodID = FABookPeriodIDAttribute.PeriodFromDate(this, details.ReceiptDate, assetBalance.BookID, false) ?? minPeriod;
				PXDatabase.Delete<FABookHistory>(
						new PXDataFieldRestrict<FABookHistory.assetID>(PXDbType.Int, 4, assetBalance.AssetID, PXComp.EQ),
						new PXDataFieldRestrict<FABookHistory.bookID>(PXDbType.Int, 4, assetBalance.BookID, PXComp.EQ),
						new PXDataFieldRestrict<FABookHistory.finPeriodID>(PXDbType.Char, 6, PeriodIDAttribute.Min(receiptPeriodID, minPeriod), PXComp.LT));

				if(string.CompareOrdinal(assetBalance.LastDeprPeriod, assetBalance.DeprToPeriod) < 0)
				{
					PXDatabase.Delete<FABookHistory>(
							new PXDataFieldRestrict<FABookHistory.assetID>(PXDbType.Int, 4, assetBalance.AssetID, PXComp.EQ),
							new PXDataFieldRestrict<FABookHistory.bookID>(PXDbType.Int, 4, assetBalance.BookID, PXComp.EQ),
							new PXDataFieldRestrict<FABookHistory.finPeriodID>(PXDbType.Char, 6, assetBalance.DeprToPeriod, PXComp.GT));
				}

				//otherwise PtdDepreciated will be reset to zero on the last period
				if (!lastGoodFlag)
				{
					PXDatabase.Update<FABookHistory>(
							new PXDataFieldRestrict<FABookHistory.assetID>(PXDbType.Int, 4, assetBalance.AssetID, PXComp.EQ),
							new PXDataFieldRestrict<FABookHistory.bookID>(PXDbType.Int, 4, assetBalance.BookID, PXComp.EQ),
							new PXDataFieldRestrict<FABookHistory.finPeriodID>(PXDbType.Char, 6, lastGoodPeriodID, PXComp.GE),
							new PXDataFieldRestrict<FABookHistory.finPeriodID>( PXDbType.Char, 6, maxPeriodID, PXComp.LE),
							new PXDataFieldAssign<FABookHistory.ptdCalculated>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ytdCalculated>(PXDbType.Decimal, lastGoodVal),
							new PXDataFieldAssign<FABookHistory.ptdBonusCalculated>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ytdBonusCalculated>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ptdTax179Calculated>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ytdTax179Calculated>( PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ptdBonusTaken>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ytdBonusTaken>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ptdTax179Taken>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<FABookHistory.ytdTax179Taken>(PXDbType.Decimal, 0m)
							);
				}
				Save.Press();

				if (assetBalance.UpdateGL == true)
				{
					string maxClosedPeriod = null;
					foreach (FABookHistory hist in PXSelectJoin<FABookHistory, InnerJoin<FinPeriod, On<FABookHistory.finPeriodID, Equal<FinPeriod.finPeriodID>>>, Where<FABookHistory.closed, NotEqual<True>, And<FinPeriod.fAClosed, Equal<True>, And<FABookHistory.assetID, Equal<Current<FABookBalance.assetID>>, And<FABookHistory.bookID, Equal<Current<FABookBalance.bookID>>>>>>>.SelectMultiBound(this, new object[] { assetBalance }))
					{
						FABookHist accuhist = new FABookHist
						{
							AssetID = assetBalance.AssetID,
							BookID = assetBalance.BookID,
							FinPeriodID = hist.FinPeriodID
						};

						accuhist = BookHistory.Insert(accuhist);

						accuhist.Closed = true;

						if (maxClosedPeriod == null || string.CompareOrdinal(hist.FinPeriodID, maxClosedPeriod) > 0)
						{
							maxClosedPeriod = hist.FinPeriodID;
						}
					}

					AssetProcess.SetLastDeprPeriod(BookBalance, assetBalance, maxClosedPeriod);
					Save.Press();
				}

				ts.Complete();
			}
		}

		public virtual void CalculateDepreciation(FADepreciationMethod method, FABookBalance assetBalance, DateTime? additionEndDate = null)
		{
			if (assetBalance.Depreciate != true) return;

			FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Required<FABook.bookID>>>>.Select(this, assetBalance.BookID);
			IYearSetup yearSetup = FABookPeriodIDAttribute.GetBookCalendar(this, book);

			string calculationMethod = method.DepreciationMethod ?? "";
			decimal multiPlier = method.DBMultiPlier ?? 0m;
			bool switchToSL = method.SwitchToSL ?? false;
			bool isTableMethod = method.IsTableMethod ?? false;
			bool yearlyAccountancy = method.YearlyAccountancy ?? false;

			DepreciationCalc.SetParameters(this, this, assetBalance, method, additionEndDate);

			depreciationBasis = method.DepreciationMethod == FADepreciationMethod.depreciationMethod.DecliningBalance
				? assetBalance.AcquisitionCost ?? 0m
				: (assetBalance.AcquisitionCost ?? 0m) - (assetBalance.SalvageAmount ?? 0m);

			if (depreciationPeriodsInYear == 0 ||
				depreciationBasis == 0m ||
				string.IsNullOrEmpty(calculationMethod) ||
				calculationMethod == FADepreciationMethod.depreciationMethod.DecliningBalance && isTableMethod == false && multiPlier == 0m
				) return;

			decimal otherDepreciation = 0m;
			decimal lastDepreciation = 0m;
			decimal rounding = 0m;
			int yearCount = 0;
			DateTime previousEndDate = DateTime.MinValue;

			previousCalculatedPeriods = 0;

			foreach(FABookPeriod per in PXSelect<FABookPeriod, 
				Where<FABookPeriod.finPeriodID, Between<Required<FABookPeriod.finPeriodID>, Required<FABookPeriod.finPeriodID>>,
					And<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>>>, 
				OrderBy<Asc<FABookPeriod.finYear>>>.Select(this, depreciationStartBookPeriod.FinPeriodID, depreciationStopBookPeriod.FinPeriodID, assetBalance.BookID))
			{
				int currYear;
				int.TryParse(per.FinYear, out currYear);
				if(yearCount != currYear)	
				{
					FADepreciationMethodLines methodLine = null;
					int yearNumber = currYear - depreciationStartYear + 1;
					if (method.IsTableMethod == true)
					{
						methodLine =PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>,
							And<FADepreciationMethodLines.year, Equal<Required<FADepreciationMethodLines.year>>>>>.Select(this, method.MethodID, yearNumber);
						if (methodLine == null)
						{
							throw new PXException(Messages.TableMethodHasNoLineForYear, method.MethodCD, per.FinYear);
						}
					}

					if (yearSetup.IsFixedLengthPeriod)
					{
						depreciationPeriodsInYear = FABookPeriodIDAttribute.GetBookPeriodsInYear(this, book, currYear);
					}

					SetLinePercents(isTableMethod, true, yearlyAccountancy, methodLine, yearNumber, assetBalance, (book.BookID ?? 0), calculationMethod, multiPlier, switchToSL, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);
					yearCount = currYear;
				}
			}
		}

		private void SetLinePercents(bool isTableMethod, bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, int bookID, string depreciationMethod, decimal multiPlier, bool switchToSL, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding, ref DateTime previousEndDate)
		{
			if (isTableMethod && yearlyAccountancy)
			{
				averagingConvention = FAAveragingConvention.FullPeriod;
				SetSLDeprOther(writeToAsset, yearlyAccountancy, line, year, assetBalance, bookID, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);					
			}
			else
			{
				switch(depreciationMethod)
				{
					case FADepreciationMethod.depreciationMethod.StraightLine:
					case FADepreciationMethod.depreciationMethod.RemainingValue:
					if (averagingConvention == FAAveragingConvention.FullDay)
					{
						SetSLDeprOther(writeToAsset, yearlyAccountancy, line, year, assetBalance, bookID, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);
					}
					else switch (depreciationPeriodsInYear)
					{
						case 12:
							SetSLDepr12(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case 4:
							SetSLDepr4(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case 2:
							SetSLDepr2(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case 1:
							SetSLDepr1(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						default:
							SetSLDeprOther(writeToAsset, yearlyAccountancy, line, year, assetBalance, bookID, ref otherDepreciation, ref lastDepreciation, ref rounding, ref previousEndDate);
							break;
					}
						break;
					case FADepreciationMethod.depreciationMethod.DecliningBalance:
						SetDBDepr(writeToAsset, yearlyAccountancy, line, year, assetBalance, bookID, multiPlier, switchToSL, ref rounding, ref previousEndDate);
						break;
					case FADepreciationMethod.depreciationMethod.SumOfTheYearsDigits:
						SetYDDepr(writeToAsset, yearlyAccountancy, line, year, assetBalance, bookID, ref rounding, ref previousEndDate);
						break;
					case FADepreciationMethod.depreciationMethod.Dutch1:
						SetNL1Depr(writeToAsset, line, year, assetBalance, ref rounding);
						break;
					case FADepreciationMethod.depreciationMethod.Dutch2:
						SetNL2Depr(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref rounding);
						break;
				}
			}
		}

		private decimal GetRoundingDelta(decimal rounding)
		{
			decimal decimals = (decimal)Math.Pow((double)0.1, (double)_Precision);
			return rounding > 0m 
				? rounding >= decimals 
					? decimals 
					: 0m 
				: rounding < 0m 
					? rounding <= (- decimals) 
						? - decimals 
						: 0m 
					: 0m;
		}

		private void SetFinalRounding(ref decimal rounding)
		{
			decimal decimals = (decimal)Math.Pow((double)0.1, (double)_Precision);
			decimal delta  = depreciationBasis - accumulatedDepreciation;
			if (delta != decimals && delta != -decimals) return;
				
			decimal centIsAppear = rounding > 0m 
				? (decimal) this.Round(rounding) == decimals 
					?  decimals : 0m 
					: rounding < 0m 
						? (decimal) this.Round(rounding) == (- decimals) ? -decimals 
						: 0m 
				: 0m;
			if (centIsAppear != delta)
			{
				centIsAppear = (delta ==  decimals) 
					? decimals 
					: (delta == -decimals) ? -decimals : 0m;
			}
			rounding = centIsAppear;
		}

		private void SetDepreciationPerPeriod(FADepreciationMethodLines line, int period, decimal value, bool useRounding, ref decimal rounding)
		{
			if (line == null) return;

			if (useRounding && rounding != 0m)
			{
				SetFinalRounding(ref rounding);
				accumulatedDepreciation += GetRoundingDelta(rounding);
				value += GetRoundingDelta(rounding);
				rounding -= GetRoundingDelta(rounding);
			}
			value = (decimal) this.Round(value);
			switch(period)
			{
				case 1:
					line.RatioPerPeriod1 = value;
					break;
				case 2:
					line.RatioPerPeriod2 = value;
					break;
				case 3:
					line.RatioPerPeriod3 = value;
					break;
				case 4:
					line.RatioPerPeriod4 = value;
					break;
				case 5:
					line.RatioPerPeriod5 = value;
					break;
				case 6:
					line.RatioPerPeriod6 = value;
					break;
				case 7:
					line.RatioPerPeriod7 = value;
					break;
				case 8:
					line.RatioPerPeriod8 = value;
					break;
				case 9:
					line.RatioPerPeriod9 = value;
					break;
				case 10:
					line.RatioPerPeriod10 = value;
					break;
				case 11:
					line.RatioPerPeriod11 = value;
					break;
				case 12:
					line.RatioPerPeriod12 = value;
					break;
			}
		}

		private void SetBookDepreciationPerPeriod(FABookBalance assetBalance, int year, int period, decimal value, bool useRounding, ref decimal rounding)
		{
			if (assetBalance == null || assetBalance.DeprFromDate == null || string.IsNullOrEmpty(assetBalance.DeprFromPeriod)) return;

			int finYear = depreciationStartYear + year - 1;
				
			if (useRounding && rounding != 0m)
			{
				SetFinalRounding(ref rounding);
				accumulatedDepreciation += GetRoundingDelta(rounding);
				value += GetRoundingDelta(rounding);
				rounding -= GetRoundingDelta(rounding);
			}
			value = (decimal)this.Round(value);

			string PeriodID = string.Format("{0:0000}{1:00}", finYear, period);

			FABookHist hist;
			FABookHist newhist = null;
			if (histdict.TryGetValue(PeriodID, out hist) && hist.YtdSuspended > 0)
			{
				for (int i = 0; i <= hist.YtdSuspended; i++)
				{
					//insert suspended periods + next open
					newhist = new FABookHist();
					newhist.AssetID = assetBalance.AssetID;
					newhist.BookID = assetBalance.BookID;
					newhist.FinPeriodID = FABookPeriodIDAttribute.PeriodPlusPeriod(this, PeriodID, i, hist.BookID);
					newhist = BookHistory.Insert(newhist);

					if (newhist != null && !histdict.ContainsKey(newhist.FinPeriodID))
					{ 
						FABookHist copy = PXCache<FABookHist>.CreateCopy(newhist);
						copy.YtdSuspended = hist.YtdSuspended;
						copy.PtdCalculated = 0m;
						copy.YtdCalculated = 0m;
						copy.PtdDepreciated = 0m;
						copy.YtdDepreciated = 0m;

						histdict.Add(newhist.FinPeriodID, copy);
					}
				}
			}
			else
			{
				newhist = new FABookHist();
				newhist.AssetID = assetBalance.AssetID;
				newhist.BookID = assetBalance.BookID;
				newhist.FinPeriodID = PeriodID;
				newhist = BookHistory.Insert(newhist);
			}

			if(newhist != null)
			{
				switch (_Destination)
				{
					case FADestination.Bonus:
						newhist.PtdBonusTaken += value;
						newhist.YtdBonusTaken += value;
						break;
					case FADestination.Tax179:
						newhist.PtdTax179Taken += value;
						newhist.YtdTax179Taken += value;
						break;
					default:
						newhist.PtdCalculated += value;
						newhist.YtdCalculated += value;
						break;
				}
			}
		}

		private void WhereToWriteDepreciation(bool writeToAsset, FADepreciationMethodLines methodLine, FABookBalance assetBalance, int year, int period, decimal value, bool useRounding, ref decimal rounding)
		{
			if (writeToAsset != true)
			{
				SetDepreciationPerPeriod(methodLine, period, value, useRounding, ref rounding);
			}
			else
			{
				SetBookDepreciationPerPeriod(assetBalance, year, period, value, useRounding, ref rounding);	
			}
		}

		private void SetSLDepr12(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal yearDepreciation = depreciationBasis / wholeRecoveryPeriods * depreciationPeriodsInYear;

			if (wholeRecoveryPeriods <= 2 && averagingConvention == FAAveragingConvention.HalfPeriod ||
				usefulLife <= 2 && averagingConvention == FAAveragingConvention.HalfYear ||
				(wholeRecoveryPeriods / (decimal) quartersCount) <= 2 && averagingConvention == FAAveragingConvention.HalfQuarter ||
				!(averagingConvention == FAAveragingConvention.HalfYear ||
					averagingConvention == FAAveragingConvention.FullYear ||
					averagingConvention == FAAveragingConvention.HalfQuarter ||
					averagingConvention == FAAveragingConvention.FullQuarter ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod) ||
				recoveryYears == 1 && 
				!(averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.HalfQuarter ||
					averagingConvention == FAAveragingConvention.FullQuarter ||
					(averagingConvention == FAAveragingConvention.FullYear && wholeRecoveryPeriods == depreciationPeriodsInYear))
				) return;

			if (year == 1 && year < recoveryYears)
			{
				otherDepreciation = yearDepreciation / depreciationPeriodsInYear;

				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
						SetSLDeprHalfPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearNotEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);							
						break;
					case FAAveragingConvention.HalfQuarter: // do not use with other metrics
					SetSLDeprHalfQuarterFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullQuarter: // do not use with other metrics
					SetSLDeprFullQuarterFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, yearDepreciation, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					SetSLDeprFullYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation,ref rounding);
						break;
				}
					
			}
			else if (year == 1 && year == recoveryYears)
			{
				otherDepreciation = (decimal) depreciationBasis / (decimal) wholeRecoveryPeriods;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
						SetSLDeprHalfPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.FullYear:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if(year == recoveryYears)
			{
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprHalfPeriodLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.HalfQuarter:
						SetSLDeprHalfQuarterLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullQuarter:
						SetSLDeprFullQuarterLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearLastYear(writeToAsset, line, year, assetBalance, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
						SetSLDeprFullYearLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else
			{
				SetSLDeprOtherYears(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
			}
		}

		private void SetSLDepr4(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			if (wholeRecoveryPeriods <= 2 && (averagingConvention == FAAveragingConvention.HalfPeriod || 
				averagingConvention == FAAveragingConvention.HalfQuarter) ||
				usefulLife <= 2 && averagingConvention == FAAveragingConvention.HalfYear ||
				!(averagingConvention == FAAveragingConvention.HalfYear ||
					averagingConvention == FAAveragingConvention.FullYear ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.HalfQuarter ||
					averagingConvention == FAAveragingConvention.FullQuarter) ||
				recoveryYears == 1 && 
				!(averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.HalfQuarter ||
					averagingConvention == FAAveragingConvention.FullQuarter ||
					(averagingConvention == FAAveragingConvention.FullYear && wholeRecoveryPeriods == depreciationPeriodsInYear))
				) return;

			if (year == 1 && year < recoveryYears)
			{
				otherDepreciation = (decimal) depreciationBasis / (decimal) wholeRecoveryPeriods;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.HalfQuarter:
						SetSLDeprHalfPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.FullQuarter:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearNotEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);							
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					SetSLDeprFullYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if (year == 1 && year == recoveryYears)
			{
				otherDepreciation = (decimal) depreciationBasis / (decimal) wholeRecoveryPeriods;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.HalfQuarter:
						SetSLDeprHalfPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
					case FAAveragingConvention.FullQuarter:
					case FAAveragingConvention.FullYear:
						SetSLDeprFullPeriodFirstYearEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if (year == recoveryYears)
			{
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.HalfQuarter:
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprHalfPeriodLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
					case FAAveragingConvention.FullQuarter:
						SetSLDeprFullPeriodLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearLastYear(writeToAsset, line, year, assetBalance, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
						SetSLDeprFullYearLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else
			{
				SetSLDeprOtherYears(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
			}
		}
			
		private void SetSLDepr2(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal totalHalfYears = wholeRecoveryPeriods;
				
			if (totalHalfYears <= 2 && (averagingConvention == FAAveragingConvention.HalfPeriod || 
				averagingConvention == FAAveragingConvention.FullQuarter) ||
				usefulLife <= 2 && averagingConvention == FAAveragingConvention.HalfYear ||
				!(averagingConvention == FAAveragingConvention.HalfYear ||
					averagingConvention == FAAveragingConvention.FullYear ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullQuarter ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod) ||
				recoveryYears == 1 && 
				!(averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullQuarter ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.HalfYear ||
					(averagingConvention == FAAveragingConvention.FullYear && totalHalfYears == depreciationPeriodsInYear))
				) return;
		
				if (year == 1 && year < recoveryYears)
				{
					otherDepreciation = (decimal) depreciationBasis / (decimal) totalHalfYears;
					switch(averagingConvention)
					{
						case FAAveragingConvention.HalfPeriod:
						case FAAveragingConvention.FullQuarter:
							SetSLDeprHalfPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case FAAveragingConvention.ModifiedPeriod:
						case FAAveragingConvention.ModifiedPeriod2:
							SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case FAAveragingConvention.FullPeriod:
						case FAAveragingConvention.NextPeriod:
						case FAAveragingConvention.HalfYear:
							SetSLDeprFullPeriodFirstYearNotEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);							
							break;
						case FAAveragingConvention.FullYear:
						SetSLDeprFullYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
							break;
					}
				}
				else if (year == 1 && year == recoveryYears)
				{
					otherDepreciation = (decimal) depreciationBasis / (decimal) totalHalfYears;
					switch(averagingConvention)
					{
						case FAAveragingConvention.HalfPeriod:
						case FAAveragingConvention.FullQuarter:
							SetSLDeprHalfPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
							break;
						case FAAveragingConvention.ModifiedPeriod:
						case FAAveragingConvention.ModifiedPeriod2:
							SetSLDeprModifiedPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case FAAveragingConvention.FullPeriod:
						case FAAveragingConvention.NextPeriod:
						case FAAveragingConvention.HalfYear:
						case FAAveragingConvention.FullYear:
							SetSLDeprFullPeriodFirstYearEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
							break;
					}
				}
				else if (year == recoveryYears)
				{
					switch(averagingConvention)
					{
						case FAAveragingConvention.HalfPeriod:
						case FAAveragingConvention.FullQuarter:
						case FAAveragingConvention.ModifiedPeriod:
						case FAAveragingConvention.ModifiedPeriod2:
							SetSLDeprHalfPeriodLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
							break;
						case FAAveragingConvention.FullPeriod:
						case FAAveragingConvention.NextPeriod:
						case FAAveragingConvention.HalfYear:
							SetSLDeprFullPeriodLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
							break;
						case FAAveragingConvention.FullYear:
							SetSLDeprFullYearLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
							break;
					}
				}
				else
				{
					SetSLDeprOtherYears(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
				}
		}

		private void SetSLDepr1(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			depreciationStartPeriod = 1;
			depreciationStopPeriod = 1;
			recoveryEndPeriod = 1;
				
			if (depreciationYears <= 2 && (averagingConvention == FAAveragingConvention.HalfYear || 
				averagingConvention == FAAveragingConvention.HalfPeriod) ||
				!(averagingConvention == FAAveragingConvention.HalfYear ||
					averagingConvention == FAAveragingConvention.FullYear ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod) ||
				recoveryYears == 1 && 
				!(averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.FullYear)
				) return;
		
			if (year == 1 && year < recoveryYears)
			{
				otherDepreciation = (decimal) depreciationBasis / (decimal) depreciationYears;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfYear:
					case FAAveragingConvention.HalfPeriod:
						SetSLDeprHalfPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearNotEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);							
						break;
				}
			}
			else if (year == 1 && year == recoveryYears)
			{
				otherDepreciation = (decimal) depreciationBasis / (decimal) depreciationYears;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if (year == recoveryYears)
			{
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfYear:
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprHalfPeriodLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else
			{
				SetSLDeprOtherYears(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
			}
		}
			
		private void SetSLDeprOther(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, int bookID, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding, ref DateTime previousEndDate)
		{
			if (wholeRecoveryPeriods <= 2 && averagingConvention == FAAveragingConvention.HalfPeriod ||
				usefulLife <= 2 && averagingConvention == FAAveragingConvention.HalfYear ||
				!(averagingConvention == FAAveragingConvention.HalfYear ||
					averagingConvention == FAAveragingConvention.FullYear ||
					averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.FullDay && (depreciationStartDate != null && depreciationStopDate != null && wholeRecoveryDays != 0 && depreciationStartYear != 0)) ||
				recoveryYears == 1 &&
				!(averagingConvention == FAAveragingConvention.HalfPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod ||
					averagingConvention == FAAveragingConvention.ModifiedPeriod2 ||
					averagingConvention == FAAveragingConvention.FullPeriod ||
					averagingConvention == FAAveragingConvention.NextPeriod ||
					averagingConvention == FAAveragingConvention.FullDay && (depreciationStartDate != null && depreciationStopDate != null && wholeRecoveryDays != 0 && depreciationStartYear != 0) ||
					(averagingConvention == FAAveragingConvention.FullYear && wholeRecoveryPeriods == depreciationPeriodsInYear))
				) return;

			if (averagingConvention == FAAveragingConvention.FullDay)
			{
				decimal periodDepr = depreciationBasis / wholeRecoveryPeriods;
				int deprYear = depreciationStartYear + year - 1;
				int firstPeriod, lastPeriod;
					
				if (year == 1 && year < recoveryYears)
				{
					firstPeriod = depreciationStartPeriod;
					lastPeriod = depreciationPeriodsInYear;
				}
				else if (year == 1 && year == recoveryYears)
				{
					firstPeriod = depreciationStartPeriod;
					lastPeriod = depreciationStopPeriod;
				}
				else if (year == recoveryYears)
				{
					firstPeriod = 1;
					lastPeriod = depreciationStopPeriod;
				}
				else
				{
					firstPeriod = 1;
					lastPeriod = depreciationPeriodsInYear;
				}

				for (int i = firstPeriod; i <= lastPeriod; i++)
				{
					if (i == depreciationStartPeriod && deprYear == depreciationStartYear) // first period
					{
						int allPeriodDays = DepreciationCalc.GetPeriodLength(this, deprYear, i, bookID);
						int deprPeriodDays = DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, depreciationStopDate, deprYear, i, bookID, ref previousEndDate);
						otherDepreciation = periodDepr * deprPeriodDays / allPeriodDays;
					}
					else if (i == depreciationStopPeriod && deprYear == int.Parse(depreciationStopBookPeriod.FinYear)) // last period
					{
						if (depreciationStopDate != null)
						{
							int allPeriodDays = DepreciationCalc.GetPeriodLength(this, deprYear, i, bookID);
							int deprPeriodDays = DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, depreciationStopDate, deprYear, i, bookID, ref previousEndDate);
							otherDepreciation = periodDepr * deprPeriodDays / allPeriodDays;
						}
						else
						{
							otherDepreciation = depreciationBasis - accumulatedDepreciation;
						}
					}
					else
					{
						otherDepreciation = periodDepr;
					}

					accumulatedDepreciation += Round(otherDepreciation);
					rounding += otherDepreciation - Round(otherDepreciation);
					WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
				}
			}
			else if (year == 1 && year < recoveryYears)
			{
				otherDepreciation = depreciationBasis / wholeRecoveryPeriods;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
						SetSLDeprHalfPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;	
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodFirstYearNotEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);							
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
					SetSLDeprFullYearFirstYearNotEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if (year == 1 && year == recoveryYears)
			{
				otherDepreciation = depreciationBasis / wholeRecoveryPeriods;
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
						SetSLDeprHalfPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprModifiedPeriodFirstYearEqualLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
					case FAAveragingConvention.FullYear:
						SetSLDeprFullPeriodFirstYearEqualLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else if(year == recoveryYears)
			{
				switch(averagingConvention)
				{
					case FAAveragingConvention.HalfPeriod:
					case FAAveragingConvention.ModifiedPeriod:
					case FAAveragingConvention.ModifiedPeriod2:
						SetSLDeprHalfPeriodLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullPeriod:
					case FAAveragingConvention.NextPeriod:
						SetSLDeprFullPeriodLastYear(writeToAsset, yearlyAccountancy, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
					case FAAveragingConvention.HalfYear:
						SetSLDeprHalfYearLastYear(writeToAsset, line, year, assetBalance, ref lastDepreciation, ref rounding);
						break;
					case FAAveragingConvention.FullYear:
						SetSLDeprFullYearLastYear(writeToAsset, line, year, assetBalance, ref otherDepreciation, ref rounding);
						break;
				}
			}
			else
			{
				bool isTableMethod = yearlyAccountancy && line != null && averagingConvention == FAAveragingConvention.FullPeriod;
				if (isTableMethod)
				{
					otherDepreciation = (line.RatioPerYear ?? 0m) * depreciationBasis / depreciationPeriodsInYear;	
				}

				for(int i = 1; i <= depreciationPeriodsInYear; i++)
				{
					if (isTableMethod)
					{
						accumulatedDepreciation += Round(otherDepreciation);
						rounding+= otherDepreciation - Round(otherDepreciation);
					}

					WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
				}
			}
		}

		private void SetDBDepr(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, int bookID, decimal multiPlier, bool switchToSL, ref decimal rounding, ref DateTime previousEndDate)
		{
			decimal depreciation;
			decimal yearDepreciation = 0m;
			decimal slYearDepreciation = 0m;
			decimal slAdjustedYearDepr = 0m;
			decimal avg_mult = 1m;
			int yearDays = 0;
	
			int fromPeriod;
			int toPeriod; 

			if (yearlyAccountancy && recoveryYears > usefulLife)
			{
				switch (averagingConvention)
				{
					case FAAveragingConvention.HalfYear:
						avg_mult = 0.5m;
						break;
					case FAAveragingConvention.HalfQuarter:
						avg_mult = (9m - 2m * ((((DateTime)assetBalance.DeprFromDate).Month + 2)/3))/8m;
						break;
				}
			}

			if (yearlyAccountancy)
			{
			yearDepreciation = (depreciationBasis - accumulatedDepreciation) * multiPlier / usefulLife;
			slAdjustedYearDepr = slYearDepreciation = depreciationBasis / usefulLife;
			}

			if (year == 1 && year < recoveryYears)
			{
				yearDepreciation *= avg_mult;
				slAdjustedYearDepr *= avg_mult;
				fromPeriod = depreciationStartPeriod;
				toPeriod = depreciationPeriodsInYear;	
			}
			else if (year == 1 && year == recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = recoveryEndPeriod;
			}
			else if(year == recoveryYears)
			{
				if (avg_mult < 1.0m)
				{
					slAdjustedYearDepr *= 1m - avg_mult;
				}
				fromPeriod = 1;
				toPeriod = recoveryEndPeriod;
			}
			else
			{
				fromPeriod = 1;
				toPeriod = depreciationPeriodsInYear;
			}

			if (year == depreciationYears)
			{
				toPeriod = depreciationStopPeriod;
			}

			decimal slDepreciation;
			if (yearlyAccountancy)
			{
				if (averagingConvention == FAAveragingConvention.FullDay)
				{
					yearDays = 0;
					DateTime periodPreviousEndDate = previousEndDate;
					for(int i = fromPeriod; i <= toPeriod; i++)
					{
						yearDays += DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, recoveryEndDate, depreciationStartYear + year - 1, i, bookID, ref previousEndDate);
					}
					previousEndDate = periodPreviousEndDate;
				}

				decimal prevAccumulatedDepreciation = accumulatedDepreciation;
				decimal slAccumulatedDepreciation = slYearDepreciation * (year - 1) - (1m - avg_mult) * (1m > avg_mult ? slYearDepreciation : 0m);
				for (int i = fromPeriod; i <= toPeriod; i++)
				{
					if (depreciationBasis == accumulatedDepreciation) return;
					if (averagingConvention == FAAveragingConvention.FullDay)
					{
						int periodDays = DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, depreciationStopDate, depreciationStartYear + year - 1, i, bookID, ref previousEndDate);
						depreciation   = yearDepreciation * periodDays / yearDays;
					}
					else
					{
						depreciation = yearDepreciation / (toPeriod - fromPeriod + 1);
					}
					if (switchToSL)
					{
						decimal DBRate = multiPlier / usefulLife;
						decimal SLRate = slAdjustedYearDepr / (depreciationBasis - slAccumulatedDepreciation);
						slDepreciation = SLRate * (depreciationBasis - prevAccumulatedDepreciation) / (toPeriod - fromPeriod + 1);
						if (SLRate > DBRate)
						{
							depreciation = slDepreciation > (depreciationBasis - accumulatedDepreciation)
								? (depreciationBasis - accumulatedDepreciation)
								: slDepreciation;
						}
					}

					accumulatedDepreciation += Round(depreciation);
					if (Round(depreciation) != (depreciationBasis - accumulatedDepreciation))
						rounding += depreciation - Round(depreciation);
					WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
				}
			}
			else
			{
				for (int i = fromPeriod; i <= toPeriod; i++)
				{	
					if (depreciationBasis == accumulatedDepreciation || wholeRecoveryPeriods == previousCalculatedPeriods) return;

					depreciation = (depreciationBasis - accumulatedDepreciation) * multiPlier / wholeRecoveryPeriods;
					slDepreciation = (depreciationBasis - accumulatedDepreciation)/(wholeRecoveryPeriods - previousCalculatedPeriods++);
					depreciation = switchToSL 
						? slDepreciation > depreciation 
							? slDepreciation > (depreciationBasis - accumulatedDepreciation) 
								? (depreciationBasis - accumulatedDepreciation) 
								: slDepreciation 
							: depreciation 
						: depreciation;

					accumulatedDepreciation += Round(depreciation);
					if (Round(depreciation) != (depreciationBasis - accumulatedDepreciation))
						rounding += depreciation - Round(depreciation);
					WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
				}
			}
		}

		private void SetNL1Depr(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal rounding)
		{
			int fromPeriod;
			int toPeriod;

			if (year == 1 && year < recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = depreciationPeriodsInYear;
			}
			else if (year == 1 && year == recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = recoveryEndPeriod;
			}
			else if (year == recoveryYears)
			{
				fromPeriod = 1;
				toPeriod = recoveryEndPeriod;
			}
			else
			{
				fromPeriod = 1;
				toPeriod = depreciationPeriodsInYear;
			}
			int depreciateToPeriod = toPeriod;

			if (year == depreciationYears)
			{
				depreciateToPeriod = depreciationStopPeriod;
			}

			decimal acquisitionCost = depreciationBasis + (assetBalance.SalvageAmount ?? 0m);
			for (int i = fromPeriod; i <= depreciateToPeriod; i++)
			{
				if (acquisitionCost == accumulatedDepreciation) return;
				decimal depreciation = (decimal)((double)(acquisitionCost - accumulatedDepreciation) * (1 - Math.Pow((double)(assetBalance.SalvageAmount ?? 0) / (double)acquisitionCost, (double)(1m / wholeRecoveryPeriods))));
				accumulatedDepreciation += Round(depreciation);
				if (Round(depreciation) != (acquisitionCost - accumulatedDepreciation))
					rounding += depreciation - Round(depreciation);
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
			}
		}

		private void SetNL2Depr(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal rounding)
		{
			int fromPeriod;
			int toPeriod;

			if (year == 1 && year < recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = depreciationPeriodsInYear;
			}
			else if (year == 1 && year == recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = recoveryEndPeriod;
			}
			else if (year == recoveryYears)
			{
				fromPeriod = 1;
				toPeriod = recoveryEndPeriod;
			}
			else
			{
				fromPeriod = 1;
				toPeriod = depreciationPeriodsInYear;
			}
			int depreciateToPeriod = toPeriod;

			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			decimal yearDepreciation = (depreciationBasis - accumulatedDepreciation) * (depreciationMethod.PercentPerYear ?? 0);
			for (int i = fromPeriod; i <= depreciateToPeriod; i++)
			{
				if (depreciationBasis == accumulatedDepreciation) return;
				decimal depreciation = yearlyAccountancy
					? yearDepreciation/depreciationPeriodsInYear
					: (decimal)((double) (depreciationBasis - accumulatedDepreciation)*(1 - Math.Pow(1 - (double) (depreciationMethod.PercentPerYear ?? 0m), 1.0/depreciationPeriodsInYear)));
				accumulatedDepreciation += Round(depreciation);
				if (Round(depreciation) != (depreciationBasis - accumulatedDepreciation))
					rounding += depreciation - Round(depreciation);
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
			}
		}

		private void SetYDDepr(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, int bookID, ref decimal rounding, ref DateTime previousEndDate)
		{
			if (yearlyAccountancy != true) return;

			decimal wholeYears = wholeRecoveryPeriods / (decimal) depreciationPeriodsInYear;
			decimal	sumOfYears = wholeYears * (wholeYears + 1) / 2m;
			decimal yearDepreciation1 = 0m;
			decimal yearDepreciation2 = 0m;
			decimal remainingYears = wholeYears - yearOfUsefulLife + 1m;
			decimal depreciation = 0m;
	
			int fromPeriod;
			int toPeriod;

			yearDepreciation1 = (depreciationBasis) * (decimal) (remainingYears)  / ((decimal) sumOfYears);
			yearDepreciation2 = (depreciationBasis) * (decimal) (remainingYears - 1) / ((decimal) sumOfYears);

			if (year == 1 && year < recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = depreciationPeriodsInYear;	
			}
			else if (year == 1 && year == recoveryYears)
			{
				fromPeriod = depreciationStartPeriod;
				toPeriod = recoveryEndPeriod;
			}
			else if(year == recoveryYears)
			{
				fromPeriod = 1;
				toPeriod = recoveryEndPeriod;
			}
			else
			{
				fromPeriod = 1;
				toPeriod = depreciationPeriodsInYear;
			}
			
			if (year == depreciationYears)
			{
				toPeriod = depreciationStopPeriod;
			}

			bool edgeOfUsefulYear = (year == 1);
			switch (averagingConvention)
			{
				case FAAveragingConvention.FullDay:
					int yearDays = 0;
					DateTime periodPreviousEndDate = previousEndDate;
					for(int i = fromPeriod; i <= toPeriod; i++)
					{
						yearDays += DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, recoveryEndDate, depreciationStartYear + year - 1, i, bookID, ref previousEndDate);
					}
					previousEndDate = periodPreviousEndDate;
						
					for(int i = fromPeriod; i <= toPeriod; i++)
					{
						if (depreciationBasis == accumulatedDepreciation) return;
						if (averagingConvention == FAAveragingConvention.FullDay)
						{
							if(edgeOfUsefulYear == false) 
							{
								edgeOfUsefulYear = (i == depreciationStartPeriod);
								yearOfUsefulLife += edgeOfUsefulYear ? 1 : 0;
							}

							int periodDays = DepreciationCalc.GetDaysOnPeriod(this, depreciationStartDate, depreciationStopDate, depreciationStartYear + year - 1, i, bookID, ref previousEndDate);
							depreciation = (wholeYears - remainingYears + 1m == yearOfUsefulLife ? yearDepreciation1 : yearDepreciation2) * (decimal) periodDays / (decimal) yearDays;
							accumulatedDepreciation += (decimal) this.Round(depreciation);
							rounding += depreciation - (decimal) this.Round(depreciation);
							WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
						}
					}
					break;
				case FAAveragingConvention.FullPeriod:
					for(int i = fromPeriod; i <= toPeriod; i++)
					{	
						if(edgeOfUsefulYear == false) 
						{
							edgeOfUsefulYear = (i == depreciationStartPeriod);
							yearOfUsefulLife += edgeOfUsefulYear ? 1 : 0;
						}

						depreciation = (wholeYears - remainingYears + 1m == yearOfUsefulLife ? yearDepreciation1 : yearDepreciation2) / (decimal) depreciationPeriodsInYear;
						accumulatedDepreciation += (decimal) this.Round(depreciation);
						rounding += depreciation - (decimal) this.Round(depreciation);
						WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciation, true, ref rounding);
					}
					break;
			}
		}

		private void SetMethodTotalPercents(FADepreciationMethod header, FADepreciationMethodLines line)
		{
			decimal sum = PXSelect<FADepreciationMethodLines, Where<FADepreciationMethodLines.methodID, Equal<Required<FADepreciationMethodLines.methodID>>>>.Select(this, header.MethodID).RowCast<FADepreciationMethodLines>().Sum(l => l.RatioPerYear ?? 0m);
			if (line != null)
			{
				sum += line.RatioPerYear ?? 0m;
			}
			header = (FADepreciationMethod) DepreciationMethod.Cache.CreateCopy(header);
			if (sum != 0m)
			{
				header.TotalPercents = sum;
				DepreciationMethod.Update(header);
			}
		}

		private void SetSLDeprHalfPeriodFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal firstDepreciation = depreciationBasis / (wholeRecoveryPeriods - 1) / 2m;
			lastDepreciation = firstDepreciation;

			decimal checkZero = (wholeRecoveryPeriods - 2);
			rounding = depreciationBasis - (decimal) this.Round(firstDepreciation) * 2m;
			if (checkZero > 0)
			{
				otherDepreciation = (depreciationBasis - firstDepreciation * 2m) / checkZero;
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;
			}
			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
			{
				depreciateToPeriod = depreciationStopPeriod;
			}

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, (depreciationStartPeriod == i) ? firstDepreciation: otherDepreciation, i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprHalfPeriodFirstYearEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)
		{
			decimal firstDepreciation = depreciationBasis / (wholeRecoveryPeriods - 1) / 2m;

			decimal checkZero = (wholeRecoveryPeriods - 2);
			if (checkZero > 0 )
			otherDepreciation = (depreciationBasis - firstDepreciation * 2) / checkZero;

			rounding = depreciationBasis - (decimal)this.Round(firstDepreciation) * 2;

			if (checkZero > 0 )
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;

			for (int i = depreciationStartPeriod; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciationStartPeriod == i || recoveryEndPeriod == i ? firstDepreciation : otherDepreciation, i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprHalfPeriodLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			for (int i = 1; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, recoveryEndPeriod == i ? lastDepreciation : otherDepreciation, true, ref rounding);
			}
		}

		private void SetSLDeprHalfQuarterFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			// use only with metric 12
			decimal deprStartPeriods = (lastDepreciationStartQuarterPeriod - depreciationStartPeriod + 1);
			decimal deprEndPeriods = (recoveryEndPeriod - firstRecoveryEndQuarterPeriod + 1);

			decimal firstDepreciation = depreciationBasis / (decimal)(recoveryYears - 1) / quartersCount / 2m / deprStartPeriods;
			lastDepreciation = depreciationBasis / (decimal)(recoveryYears - 1) / 2m / quartersCount / deprEndPeriods;

			decimal checkZero = (wholeRecoveryPeriods - deprStartPeriods - deprEndPeriods);
			if (checkZero > 0 )
				otherDepreciation = (depreciationBasis - firstDepreciation * deprStartPeriods - lastDepreciation * deprEndPeriods) / checkZero;

			rounding = depreciationBasis - (decimal)this.Round(firstDepreciation) * deprStartPeriods;
			rounding -= (decimal)this.Round(lastDepreciation) * deprEndPeriods;
			if (checkZero > 0)
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;

			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i,
					depreciationStartPeriod <= i && lastDepreciationStartQuarterPeriod >= i
						? firstDepreciation
						: otherDepreciation,
					i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprHalfQuarterLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			for (int i = 1; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, (recoveryEndPeriod >= i && firstRecoveryEndQuarterPeriod <= i) ? lastDepreciation : otherDepreciation, true, ref rounding);
			}
		}

		private void SetSLDeprModifiedPeriodFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal periods = (wholeRecoveryPeriods + startDepreciationMidPeriodRatio + stopDepreciationMidPeriodRatio - 2);
			decimal firstDepreciation = depreciationBasis / periods * startDepreciationMidPeriodRatio;
			lastDepreciation = depreciationBasis / periods * stopDepreciationMidPeriodRatio;

			rounding  = depreciationBasis - (decimal) this.Round(firstDepreciation);
			rounding -= (decimal)this.Round(lastDepreciation);

			decimal checkZero = wholeRecoveryPeriods - 2;
				if (checkZero > 0 )
			{
				otherDepreciation = (depreciationBasis - firstDepreciation - lastDepreciation) / checkZero;
					rounding -= (decimal) this.Round(otherDepreciation) * checkZero;
			}

			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, depreciationStartPeriod == i ? firstDepreciation : (year == depreciationYears && i == recoveryEndPeriod) ? lastDepreciation : otherDepreciation, i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprModifiedPeriodFirstYearEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal periods = (wholeRecoveryPeriods + startDepreciationMidPeriodRatio + stopDepreciationMidPeriodRatio - 2);
			decimal firstDepreciation = depreciationBasis / periods * startDepreciationMidPeriodRatio;
			lastDepreciation = depreciationBasis / periods * stopDepreciationMidPeriodRatio;

			decimal checkZero = wholeRecoveryPeriods - 2;
			if(checkZero > 0)
			otherDepreciation = (depreciationBasis - firstDepreciation - lastDepreciation) / checkZero;

			rounding = depreciationBasis - (decimal)this.Round(firstDepreciation);
			rounding -= (decimal)this.Round(lastDepreciation);

			if(checkZero > 0)
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;

			for (int i = depreciationStartPeriod; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i,
					depreciationStartPeriod == i ? firstDepreciation : recoveryEndPeriod == i ? lastDepreciation : otherDepreciation,
					i > depreciationStartPeriod, ref rounding);
			}
		}

		private void SetSLDeprFullYearFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)
		{
			decimal firstDepreciation = otherDepreciation * depreciationPeriodsInYear / (depreciationPeriodsInYear - depreciationStartPeriod + 1);

			decimal checkZero = wholeRecoveryPeriods - depreciationPeriodsInYear;
			if(checkZero > 0)
				otherDepreciation = (depreciationBasis - firstDepreciation * (depreciationPeriodsInYear - depreciationStartPeriod + 1)) / checkZero; 

			rounding  = depreciationBasis - (decimal) this.Round(firstDepreciation) * (depreciationPeriodsInYear - depreciationStartPeriod + 1);

			if(checkZero > 0)
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;

			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, firstDepreciation, i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprFullYearLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)
		{
			for (int i = 1; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
			}
		}

		private void SetSLDeprFullPeriodFirstYearNotEqualLastYear(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)
		{
			bool isTableMethod = yearlyAccountancy && line != null;
			if (isTableMethod)
			{
				otherDepreciation = (line.RatioPerYear ?? 0m) * depreciationBasis / (depreciationPeriodsInYear - depreciationStartPeriod + 1);	
			}
			else
			{
				rounding = depreciationBasis - (decimal)this.Round(otherDepreciation) * wholeRecoveryPeriods;
			}

			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
			{
				depreciateToPeriod = depreciationStopPeriod;
			}

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				if (isTableMethod)
				{
					accumulatedDepreciation += (decimal)this.Round(otherDepreciation);
					rounding += otherDepreciation - (decimal)this.Round(otherDepreciation);
				}

				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, 
					depreciationStartPeriod >= depreciationPeriodsInYear || (i > depreciationStartPeriod || yearlyAccountancy), ref rounding);
			}
		}
		private void SetSLDeprFullPeriodFirstYearEqualLastYear(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)	
		{
			bool isTableMethod = yearlyAccountancy && line != null;
			if (isTableMethod)
			{
				otherDepreciation = (line.RatioPerYear ?? 0m) * depreciationBasis / wholeRecoveryPeriods;	
			}
			else
			{
				rounding = depreciationBasis - (decimal) this.Round(otherDepreciation) * wholeRecoveryPeriods;
			}

			for(int i = depreciationStartPeriod; i <= depreciationStopPeriod; i++)
			{
				if (isTableMethod)
				{
					accumulatedDepreciation += (decimal) this.Round(otherDepreciation);
					rounding += otherDepreciation - (decimal) this.Round(otherDepreciation);
				}

				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, i > depreciationStartPeriod || (yearlyAccountancy), ref rounding);
			}
		}
		private void SetSLDeprFullPeriodLastYear(bool writeToAsset, bool yearlyAccountancy, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)	
		{
			bool isTableMethod = yearlyAccountancy && line != null;
			if (isTableMethod)
			{
				otherDepreciation = (line.RatioPerYear ?? 0m) * depreciationBasis / recoveryEndPeriod;	
			}

			for(int i = 1; i <= depreciationStopPeriod; i++)
			{
				if (isTableMethod)
				{
					accumulatedDepreciation += (decimal) this.Round(otherDepreciation);
					rounding += otherDepreciation - (decimal) this.Round(otherDepreciation);
				}

				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
			}	
		}

		private void SetSLDeprFullQuarterFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, decimal yearDepreciation, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			// use only with metric 12
			decimal firstDepreciation = yearDepreciation / quartersCount / (lastDepreciationStartQuarterPeriod - depreciationStartPeriod + 1);
			decimal checkZero = wholeRecoveryPeriods - quarterToMonth;
			if (checkZero > 0)
			{
				otherDepreciation = (depreciationBasis - firstDepreciation * (lastDepreciationStartQuarterPeriod - depreciationStartPeriod + 1)) / checkZero;
			}

			rounding  = depreciationBasis - (decimal)this.Round(firstDepreciation) * (lastDepreciationStartQuarterPeriod - depreciationStartPeriod + 1);
			if (checkZero > 0)
			{
				rounding -= (decimal)this.Round(otherDepreciation) * checkZero;
			}

			int depreciateToPeriod = depreciationPeriodsInYear;

			if (year == depreciationYears)
			{
				depreciateToPeriod = depreciationStopPeriod;
			}

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i,
					depreciationStartPeriod <= i && lastDepreciationStartQuarterPeriod >= i
						? firstDepreciation
						: otherDepreciation,
					i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprFullQuarterLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)
		{
			for (int i = 1; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
			}
		}

		private void SetSLDeprHalfYearFirstYearNotEqualLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal lastDepreciation, ref decimal rounding)
		{
			decimal deprStartPeriods = (depreciationPeriodsInYear - depreciationStartPeriod + 1);
			decimal deprEndPeriods = recoveryEndPeriod;

			decimal firstDepreciation = depreciationBasis / (decimal)(recoveryYears - 1) / 2m / deprStartPeriods;
			lastDepreciation = depreciationBasis / (decimal)(recoveryYears - 1) / 2m / deprEndPeriods;

			decimal checkZero = (wholeRecoveryPeriods - deprStartPeriods - deprEndPeriods);
			if(checkZero > 0)
				otherDepreciation = (depreciationBasis - firstDepreciation * deprStartPeriods - lastDepreciation * deprEndPeriods) / checkZero;

			rounding = depreciationBasis - (decimal)this.Round(firstDepreciation) * deprStartPeriods;
			rounding -= (decimal)this.Round(lastDepreciation) * deprEndPeriods;

			if(checkZero > 0)
				rounding -= (decimal) this.Round(otherDepreciation) * checkZero;

			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			for(int i = depreciationStartPeriod; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, firstDepreciation, i > depreciationStartPeriod, ref rounding);
			}
		}
		private void SetSLDeprHalfYearLastYear(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal lastDepreciation, ref decimal rounding)
		{
			for (int i = 1; i <= depreciationStopPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, lastDepreciation, true, ref rounding);
			}
		}

		private void SetSLDeprOtherYears(bool writeToAsset, FADepreciationMethodLines line, int year, FABookBalance assetBalance, ref decimal otherDepreciation, ref decimal rounding)	
		{
			int depreciateToPeriod = depreciationPeriodsInYear;
			if (year == depreciationYears)
				depreciateToPeriod = depreciationStopPeriod;

			for(int i = 1; i <= depreciateToPeriod; i++)
			{
				WhereToWriteDepreciation(writeToAsset, line, assetBalance, year, i, otherDepreciation, true, ref rounding);
			}
		}
		#endregion

		#region IDepreciationCalculation Members
		public decimal depreciationBasis { get; set; }
		public decimal accumulatedDepreciation { get; set; }
		public int depreciationStartYear { get; set; }
		public FABookPeriod depreciationStopBookPeriod { get; set; }
		public FABookPeriod depreciationStartBookPeriod { get; set; }
		public int depreciationStartPeriod { get; set; }
		public int depreciationStopPeriod { get; set; }
		public int recoveryEndPeriod { get; set; }
		public int firstRecoveryEndQuarterPeriod { get; set; }
		public int firstDepreciationStopQuarterPeriod { get; set; }
		public int lastDepreciationStartQuarterPeriod { get; set; }
		public DateTime recoveryEndDate { get; set; }
		public int recoveryYears { get; set; }
		public decimal usefulLife { get; set; }
		public int depreciationYears { get; set; }
		public decimal startDepreciationMidPeriodRatio { get; set; }
		public decimal stopDepreciationMidPeriodRatio { get; set; }
		public DateTime depreciationStartDate { get; set; }
		public DateTime? depreciationStopDate { get; set; }
		public int wholeRecoveryDays { get; set; }
		public string averagingConvention { get; set; }
		public decimal wholeRecoveryPeriods { get; set; }
		public int depreciationPeriodsInYear { get; set; }
		public FADepreciationMethod depreciationMethod { get; set; }
		public int yearOfUsefulLife { get; set; }
		public DateTime recoveryStartDate { get; set; }
		public FABookPeriod recoveryStartBookPeriod { get; set; }
		public FABookPeriod recoveryEndBookPeriod { get; set; }
		#endregion

		private int previousCalculatedPeriods; // used only in SetDBDepr
	}

	public class DepreciationCalc : IDepreciationCalculation
	{
		protected static int monthsInYear = 12;
		protected static int quarterToMonth = 3;
		protected static int quartersCount = 4;

		public static DateTime GetRecoveryEndDate(PXGraph graph, FABookBalance assetBalance)
		{
			DepreciationCalc calc = new DepreciationCalc();
			SetParameters(graph, calc, assetBalance);
			return calc.recoveryEndDate;
		}

		public static string GetRecoveryStartPeriod(PXGraph graph, FABookBalance assetBalance)
		{
			DepreciationCalc calc = new DepreciationCalc();
			SetParameters(graph, calc, assetBalance);
			string startPeriodID = string.Format("{0:0000}{1:00}", calc.depreciationStartYear, calc.depreciationStartPeriod) ;
			if (calc.startDepreciationMidPeriodRatio == 0m && (assetBalance.AveragingConvention == FAAveragingConvention.ModifiedPeriod || assetBalance.AveragingConvention == FAAveragingConvention.ModifiedPeriod2))
			{
				return FABookPeriodIDAttribute.PeriodPlusPeriod(graph, startPeriodID, 1, assetBalance.BookID);
			}
			return startPeriodID;
		}

		public static bool IsTableMethod(FADepreciationMethod method)
			{
				if (method == null)
				{	
					return false;
				}

				return method.IsTableMethod == true && method.YearlyAccountancy == true;
			}

		public static void InitParameters(IDepreciationCalculation calc)
		{ 
			calc.depreciationBasis = 0m;
			calc.accumulatedDepreciation = 0m;
			calc.depreciationStartYear = 0;
			calc.depreciationStopBookPeriod = null;
			calc.depreciationStartBookPeriod = null;
			calc.depreciationStartPeriod = 0;
			calc.depreciationStopPeriod = 0;
			calc.recoveryEndPeriod = 0;
			calc.firstRecoveryEndQuarterPeriod = 0;
			calc.firstDepreciationStopQuarterPeriod = 0;
			calc.lastDepreciationStartQuarterPeriod = 0;
			calc.recoveryEndDate = DateTime.MinValue;
			calc.recoveryYears = 0;
			calc.depreciationYears = 0;
			calc.startDepreciationMidPeriodRatio = 0m;
			calc.stopDepreciationMidPeriodRatio = 0m;
			calc.wholeRecoveryDays = 0;
			calc.averagingConvention = null;
			calc.wholeRecoveryPeriods = 0;
			calc.depreciationPeriodsInYear = 12;
			calc.depreciationMethod = null;
			calc.yearOfUsefulLife = 1;
			calc.usefulLife = 0;
		}

		public static void SetParameters(PXGraph graph, IDepreciationCalculation calc, FABookBalance assetBalance, 
			FADepreciationMethod depreciationMethod = null, DateTime? recoveryEndDate = null)
		{
			PXSelectBase<FADepreciationMethod> selectMethod = new PXSelect<FADepreciationMethod, 
				Where<FADepreciationMethod.methodID, Equal<Required<FADepreciationMethod.methodID>>>>(graph);
			
			SetParameters(graph, calc, assetBalance.BookID,
				assetBalance.Depreciate == true,
				assetBalance.DeprFromDate.Value, 
				assetBalance.DeprToDate,
				recoveryEndDate,
				depreciationMethod ?? selectMethod.Select(assetBalance.DepreciationMethodID), 
				assetBalance.AveragingConvention,
				assetBalance.MidMonthType, 
				assetBalance.MidMonthDay,
				assetBalance.UsefulLife ?? 0m);
		}

		public static void SetParameters(PXGraph graph,
			IDepreciationCalculation calc,
			int? bookID,
			bool depreciate,
			DateTime depreciationStartDate,
			DateTime? depreciationStopDate,
			DateTime? recoveryEndDate,
			FADepreciationMethod depreciationMethod,
			string averagingConvention,
			string midMonthType,
			int? midMonthDay,
			decimal usefulLife)
		{
			InitParameters(calc);
			calc.depreciationStopDate = depreciationStopDate;
			calc.usefulLife = usefulLife;

			FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Required<FABook.bookID>>>>.Select(graph, bookID);
			IYearSetup yearSetup = FABookPeriodIDAttribute.GetBookCalendar(graph, book);

			FillRecoveryParams(graph, calc, yearSetup, book, depreciationStartDate, depreciationMethod, recoveryEndDate, averagingConvention, midMonthType, midMonthDay);
			if (!depreciate) return;

			int recoveryEndYear = int.Parse(calc.recoveryEndBookPeriod.FinYear);
			calc.recoveryEndPeriod = int.Parse(calc.recoveryEndBookPeriod.PeriodNbr);
			calc.recoveryYears = recoveryEndYear - calc.depreciationStartYear + 1;
			calc.wholeRecoveryDays = (calc.recoveryEndDate - calc.recoveryStartDate).Days;

			if (calc.depreciationStopDate != null)
			{
				if (calc.depreciationStopDate > calc.recoveryEndDate)
				{
					throw new PXException(Messages.InvalidDeprToDate);
				}
				
				calc.depreciationStopBookPeriod = FABookPeriodIDAttribute.FABookPeriodFromDate(graph, calc.depreciationStopDate, bookID, false);
				if (calc.depreciationStopBookPeriod == null)
				{
					throw new PXException(Messages.FABookPeriodsNotDefinedFromTo, calc.depreciationStartDate.ToShortDateString(), 
						((DateTime)calc.depreciationStopDate).ToShortDateString(), calc.depreciationStartDate.Year, ((DateTime)calc.depreciationStopDate).Year, book.BookCode);
				}

				int depreciationStopYear = int.Parse(calc.depreciationStopBookPeriod.FinYear);
				calc.depreciationYears = depreciationStopYear - calc.depreciationStartYear + 1;
				calc.depreciationStopPeriod = int.Parse(calc.depreciationStopBookPeriod.PeriodNbr);
			}
			else
			{
				calc.depreciationStopDate = calc.recoveryEndDate;
				calc.depreciationStopPeriod = calc.recoveryEndPeriod;
				calc.depreciationYears = calc.recoveryYears;
				calc.depreciationStopBookPeriod = calc.recoveryEndBookPeriod;
			}

			switch (calc.averagingConvention)
			{
				case FAAveragingConvention.FullDay:
				int recoveryStartYear = int.Parse(calc.recoveryStartBookPeriod.FinYear);
				int recoveryStartPeriod = int.Parse(calc.recoveryStartBookPeriod.PeriodNbr);

				DateTime previousEndDate = DateTime.MinValue;
				int allPeriodDays = DepreciationCalc.GetPeriodLength(graph, recoveryStartYear, recoveryStartPeriod, bookID);
				int deprPeriodDays = DepreciationCalc.GetDaysOnPeriod(graph, calc.recoveryStartDate, calc.recoveryEndDate, recoveryStartYear, recoveryStartPeriod, bookID, ref previousEndDate);
				decimal rate = (decimal)deprPeriodDays / (decimal)allPeriodDays;
				calc.wholeRecoveryPeriods += rate - 1;

				allPeriodDays = DepreciationCalc.GetPeriodLength(graph, recoveryEndYear, calc.recoveryEndPeriod, bookID);
				deprPeriodDays = DepreciationCalc.GetDaysOnPeriod(graph, calc.recoveryStartDate, calc.recoveryEndDate, recoveryEndYear, calc.recoveryEndPeriod, bookID, ref previousEndDate);
				rate = (decimal)deprPeriodDays / (decimal)allPeriodDays;
				calc.wholeRecoveryPeriods += rate - 1;
				break;
				case FAAveragingConvention.FullQuarter:
				case FAAveragingConvention.HalfQuarter:
				if (calc.depreciationPeriodsInYear == monthsInYear)
				{
					decimal recoveryEndPeriodDivide3 = (decimal)calc.recoveryEndPeriod / (decimal)quarterToMonth;
					int recoveryEndQuarter = (int)Decimal.Ceiling(recoveryEndPeriodDivide3);
					calc.firstRecoveryEndQuarterPeriod = (recoveryEndQuarter - 1) * quarterToMonth + 1;

					decimal depreciationStopPeriodDivide3 = (decimal)calc.depreciationStopPeriod / (decimal)quarterToMonth;
					int depreciationStopQuarter = (int)Decimal.Ceiling(depreciationStopPeriodDivide3);
					calc.firstDepreciationStopQuarterPeriod = (depreciationStopQuarter - 1) * quarterToMonth + 1;

					decimal depreciationStartPeriodDivide3 = (decimal)calc.depreciationStartPeriod / (decimal)quarterToMonth;
					int depreciationStartPeriodQuarter = (int)Decimal.Ceiling(depreciationStartPeriodDivide3);
					calc.lastDepreciationStartQuarterPeriod = depreciationStartPeriodQuarter * quarterToMonth;
				}
				break;
			}
		}

		public static void FillRecoveryParams(PXGraph graph,
			IDepreciationCalculation calc,
			IYearSetup yearSetup,
			FABook book,
			DateTime depreciationStartDate,
			FADepreciationMethod depreciationMethod,
			DateTime? recoveryEndDate,
			string averagingConvention,
			string midMonthType,
			int? midMonthDay)
		{
			calc.depreciationStartDate = depreciationStartDate;
			calc.depreciationMethod = depreciationMethod;
			calc.averagingConvention = averagingConvention;
				
			if (yearSetup.FPType == FiscalPeriodSetupCreator.FPType.Quarter)
			{
				switch (calc.averagingConvention)
				{
					case FAAveragingConvention.FullQuarter:
						calc.averagingConvention = FAAveragingConvention.FullPeriod;
						break;
					case FAAveragingConvention.HalfQuarter:
						calc.averagingConvention = FAAveragingConvention.HalfPeriod;
						break;
				}
			}

			calc.depreciationStartBookPeriod = FABookPeriodIDAttribute.FABookPeriodFromDate(graph, depreciationStartDate, book.BookID, false);
			if (calc.depreciationStartBookPeriod == null)
			{
				throw new PXException(Messages.FABookPeriodsNotDefinedFrom, depreciationStartDate.ToShortDateString(), depreciationStartDate.Year, book.BookCode);
			}

			calc.depreciationStartYear = int.Parse(calc.depreciationStartBookPeriod.FinYear);
			calc.depreciationStartPeriod = int.Parse(calc.depreciationStartBookPeriod.PeriodNbr);
			calc.depreciationPeriodsInYear = FABookPeriodIDAttribute.GetBookPeriodsInYear(graph, book, calc.depreciationStartYear);

			calc.recoveryStartDate = calc.depreciationStartDate;
			calc.recoveryStartBookPeriod = calc.depreciationStartBookPeriod;

			bool addPeriodToEnd = false;
			int recoveryStartPeriod;
			int depreciationStartQuarter;
			decimal depreciationStartPeriodDivide3;

			switch (calc.averagingConvention)
			{
				case FAAveragingConvention.FullDay:
					break;
				case FAAveragingConvention.ModifiedPeriod:
				case FAAveragingConvention.ModifiedPeriod2:
					calc.recoveryStartDate = calc.depreciationStartBookPeriod.StartDate.Value;
					
					switch (midMonthType)
					{
						case FABook.midMonthType.PeriodDaysHalve:
							if ((calc.depreciationStartDate - calc.depreciationStartBookPeriod.StartDate.Value).Days + 1 >
								(calc.depreciationStartBookPeriod.EndDate.Value - calc.depreciationStartBookPeriod.StartDate.Value).Days / 2m)
							{
								calc.startDepreciationMidPeriodRatio = calc.averagingConvention == FAAveragingConvention.ModifiedPeriod2 ? 0m : 0.5m;
								addPeriodToEnd = true;
							}
							else
							{
								calc.startDepreciationMidPeriodRatio = 1m;
							}
							break;
						case FABook.midMonthType.FixedDay:
							if (((calc.depreciationStartDate - calc.depreciationStartBookPeriod.StartDate.Value).Days + 1) > midMonthDay)
							{
								calc.startDepreciationMidPeriodRatio = calc.averagingConvention == FAAveragingConvention.ModifiedPeriod2 ? 0m : 0.5m;
								addPeriodToEnd = true;
							}
							else
							{
								calc.startDepreciationMidPeriodRatio = 1m;
							}
							break;
						case FABook.midMonthType.NumberOfDays:
							int previousPeriod = calc.depreciationStartPeriod - 1;
							int previousYear = calc.depreciationStartYear;
							if (previousPeriod == 0)
							{
								previousYear--;
								previousPeriod = yearSetup.IsFixedLengthPeriod
									? FABookPeriodIDAttribute.GetBookPeriodsInYear(graph, book, previousYear)
									: calc.depreciationPeriodsInYear;
							}
								
							FABookPeriod previousBookPeriod = GetBookPeriod(graph, previousPeriod, previousYear, book.BookID);
							if (((calc.depreciationStartDate - previousBookPeriod.EndDate.Value).Days + 1) > midMonthDay)
							{
								calc.startDepreciationMidPeriodRatio = calc.averagingConvention == FAAveragingConvention.ModifiedPeriod2 ? 0m : 0.5m;
								addPeriodToEnd = true;
							}
							else
							{
								calc.startDepreciationMidPeriodRatio = 1m;
							}
							break;
					}

					calc.stopDepreciationMidPeriodRatio = calc.averagingConvention == FAAveragingConvention.ModifiedPeriod
						? calc.startDepreciationMidPeriodRatio
						: 1m;
					break;
				case FAAveragingConvention.FullPeriod:
					calc.recoveryStartDate = calc.depreciationStartBookPeriod.StartDate.Value;
					break;
				case FAAveragingConvention.NextPeriod:
					string nextPeriodID = GetNextPeriodID(graph, yearSetup, book, calc.depreciationStartBookPeriod.FinPeriodID, calc.depreciationPeriodsInYear);
					calc.recoveryStartBookPeriod = GetBookPeriod(graph, nextPeriodID, book.BookID);
					if (calc.recoveryStartBookPeriod == null)
					{
						throw new PXException(Messages.FABookPeriodsNotDefinedFrom, FABookPeriodIDAttribute.FormatForError(nextPeriodID),
							FiscalPeriodUtils.FiscalYear(nextPeriodID), book.BookCode);
					}

					calc.depreciationStartPeriod = int.Parse(FiscalPeriodUtils.PeriodInYear(nextPeriodID));
					calc.depreciationStartYear = int.Parse(FiscalPeriodUtils.FiscalYear(nextPeriodID));
					calc.recoveryStartDate = calc.recoveryStartBookPeriod.StartDate.Value;
					break;
				case FAAveragingConvention.FullQuarter:
					if (calc.depreciationPeriodsInYear == monthsInYear)
					{
						depreciationStartPeriodDivide3 = (decimal)calc.depreciationStartPeriod / (decimal)quarterToMonth;
						depreciationStartQuarter = (int)Decimal.Ceiling(depreciationStartPeriodDivide3);
						recoveryStartPeriod = (depreciationStartQuarter - 1) * quarterToMonth + 1;
						calc.recoveryStartBookPeriod = GetBookPeriod(graph, recoveryStartPeriod, calc.depreciationStartYear, book.BookID);
						if (calc.recoveryStartBookPeriod == null)
						{
							throw new PXException(Messages.FABookPeriodsNotDefinedFrom, string.Format("{0}-{1}",
								recoveryStartPeriod.ToString("00"), calc.depreciationStartYear), calc.depreciationStartYear, book.BookCode);
						}

						calc.recoveryStartDate = calc.recoveryStartBookPeriod.StartDate.Value;
					}
					break;
				case FAAveragingConvention.FullYear:
					recoveryStartPeriod = 1;
					calc.recoveryStartBookPeriod = GetBookPeriod(graph, recoveryStartPeriod, calc.depreciationStartYear, book.BookID);
					if (calc.recoveryStartBookPeriod == null)
					{
						throw new PXException(Messages.FABookPeriodsNotDefinedFrom, string.Format("{0}-{1}",
							recoveryStartPeriod.ToString("00"), calc.depreciationStartYear), calc.depreciationStartYear, book.BookCode);
					}

					calc.recoveryStartDate = calc.recoveryStartBookPeriod.StartDate.Value;
					break;
				case FAAveragingConvention.HalfPeriod:
					calc.recoveryStartDate = calc.depreciationStartBookPeriod.StartDate.Value;
					addPeriodToEnd = true;
					break;
				case FAAveragingConvention.HalfQuarter:
					if (calc.depreciationPeriodsInYear == monthsInYear)
					{
						if (calc.wholeRecoveryPeriods % quarterToMonth != 0)
						{
							throw new PXException(Messages.CanNotUseAveragingConventionWhithRecoveryPeriods, calc.averagingConvention, calc.wholeRecoveryPeriods);
						}

						depreciationStartPeriodDivide3 = (decimal)calc.depreciationStartPeriod / (decimal)quarterToMonth;
						depreciationStartQuarter = (int)Decimal.Ceiling(depreciationStartPeriodDivide3);
						recoveryStartPeriod = (depreciationStartQuarter - 1) * quarterToMonth + 1;
						calc.recoveryStartBookPeriod = GetBookPeriod(graph, recoveryStartPeriod, calc.depreciationStartYear, book.BookID);
						if (calc.recoveryStartBookPeriod == null)
						{
							throw new PXException(Messages.FABookPeriodsNotDefinedFrom, string.Format("{0}-{1}",
								recoveryStartPeriod.ToString("00"), calc.depreciationStartYear), calc.depreciationStartYear, book.BookCode);
						}

						addPeriodToEnd = calc.depreciationStartDate == calc.recoveryStartBookPeriod.StartDate;
						calc.recoveryStartBookPeriod = calc.depreciationStartBookPeriod;
					}
					break;
				case FAAveragingConvention.HalfYear:
					recoveryStartPeriod = 1;
					calc.recoveryStartBookPeriod = GetBookPeriod(graph, recoveryStartPeriod, calc.depreciationStartYear, book.BookID);
					if (calc.recoveryStartBookPeriod == null)
					{
						throw new PXException(Messages.FABookPeriodsNotDefinedFrom, string.Format("{0}-{1}",
							recoveryStartPeriod.ToString("00"), calc.depreciationStartYear), calc.depreciationStartYear, book.BookCode);
					}

					addPeriodToEnd = calc.depreciationStartDate == calc.recoveryStartBookPeriod.StartDate;
					calc.recoveryStartBookPeriod = calc.depreciationStartBookPeriod;
					break;
			}

			calc.recoveryEndDate = recoveryEndDate ?? DepreciationCalc.DatePlusYears(calc.recoveryStartDate, calc.usefulLife);
			calc.recoveryEndBookPeriod = FABookPeriodIDAttribute.FABookPeriodFromDate(graph, calc.recoveryEndDate, book.BookID, false);
			if (calc.recoveryEndBookPeriod == null)
			{
				throw new PXException(Messages.FABookPeriodsNotDefinedFromTo, calc.depreciationStartDate.ToShortDateString(),
					calc.recoveryEndDate.ToShortDateString(), calc.depreciationStartDate.Year, calc.recoveryEndDate.Year, book.BookCode);
			}

			if (addPeriodToEnd && recoveryEndDate == null)
			{
				string nextPeriodID = GetNextPeriodID(graph, yearSetup, book, calc.recoveryEndBookPeriod.FinPeriodID, calc.depreciationPeriodsInYear);
				calc.recoveryEndBookPeriod = GetBookPeriod(graph, nextPeriodID, book.BookID);
				if (calc.recoveryEndBookPeriod == null)
				{
					throw new PXException(Messages.FABookPeriodsNotDefinedFrom, FABookPeriodIDAttribute.FormatForError(nextPeriodID),
						FiscalPeriodUtils.FiscalYear(nextPeriodID), book.BookCode);
				}

				calc.recoveryEndDate = calc.recoveryEndBookPeriod.EndDate.Value.AddDays(-1);
			}

			calc.wholeRecoveryPeriods = (FABookPeriodIDAttribute.PeriodMinusPeriod(graph, calc.recoveryEndBookPeriod.FinPeriodID, calc.recoveryStartBookPeriod.FinPeriodID, book.BookID) ?? 0m) + 1m;
		}

		public static string GetNextPeriodID(PXGraph graph, IYearSetup yearSetup, FABook book, string periodID, int periodsInYear)
		{
			int nextPeriod = int.Parse(FiscalPeriodUtils.PeriodInYear(periodID)) + 1;
			int nextYear = int.Parse(FiscalPeriodUtils.FiscalYear(periodID));

			int periodsInNextYear = yearSetup.IsFixedLengthPeriod
				? FABookPeriodIDAttribute.GetBookPeriodsInYear(graph, book, nextYear)
				: periodsInYear;
			if (nextPeriod > periodsInNextYear)
			{
				nextYear++;
				periodsInNextYear = yearSetup.IsFixedLengthPeriod
					? FABookPeriodIDAttribute.GetBookPeriodsInYear(graph, book, nextYear)
					: periodsInYear;
				nextPeriod = nextPeriod % periodsInNextYear;
			}

			return string.Format("{0:0000}{1:00}", nextYear, nextPeriod);
		}

		public static int GetFinancialYears(int wholeRecoveryPeriods, int startPeriod, int depreciationPeriodsInYear, bool startPeriodIsWhole)
		{
			if (wholeRecoveryPeriods == 0 || startPeriod == 0) return 0;
			decimal financialYearsToCalendar = (decimal)(wholeRecoveryPeriods + startPeriod - 1 + (startPeriodIsWhole == false ? 1 : 0)) / (decimal)depreciationPeriodsInYear;
			int financialYears = (int)Decimal.Ceiling(financialYearsToCalendar);
			return financialYears;
		}

		public static int GetPeriodLength(PXGraph graph, int currYear, int currPeriod, int? bookID)
		{
			FABookPeriod period = GetBookPeriod(graph, currPeriod, currYear, bookID);
			if (period == null || period.StartDate == null || period.EndDate == null)
			{
				FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Required<FABook.bookID>>>>.Select(graph, bookID);
				throw new PXException(Messages.FABookPeriodsNotDefined, book.BookCode, currYear);
			}

			return (period.EndDate.Value - period.StartDate.Value).Days;
		}

		public static int GetDaysOnPeriod(PXGraph graph, DateTime recoveryStartDate, DateTime? recoveryEndDate, int currYear, int currPeriod, int? bookID, ref DateTime previousEndDate)
		{
			FABookPeriod existBookPeriod = GetBookPeriod(graph, currPeriod, currYear, bookID);
			if (existBookPeriod == null)
			{
				FABook book = PXSelect<FABook, Where<FABook.bookID, Equal<Required<FABook.bookID>>>>.Select(graph, bookID);
				throw new PXException(Messages.FABookPeriodsNotDefined, book.BookCode, currYear);
			}

			DateTime? existPeriodStartDate = existBookPeriod.StartDate.Value;
			DateTime? existPeriodEndDate = existBookPeriod.EndDate.Value;

			int recoveryDays = 0;
			if (recoveryStartDate	 <= existPeriodEndDate &&
				existPeriodStartDate <= recoveryEndDate)
			{
				DateTime? periodStartDate	= existPeriodStartDate	> recoveryStartDate ? existPeriodStartDate	: recoveryStartDate;
				DateTime? periodEndDate		= existPeriodEndDate	< recoveryEndDate	? existPeriodEndDate	: recoveryEndDate;
				recoveryDays  = (periodEndDate.Value - periodStartDate.Value).Days;
				recoveryDays += (periodStartDate == previousEndDate) ?
									(periodEndDate == recoveryEndDate) ? 1 : 0 :
									(previousEndDate == DateTime.MinValue) ? 0 : 1;
				previousEndDate = periodEndDate.Value;
			}

			return recoveryDays;
		}

		public static FABookYear GetBookYear(PXGraph graph, int? BookID, int Year)
		{
			return PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Required<FABookYear.bookID>>,
				And<FABookYear.year, Equal<Required<FABookYear.year>>>>>
				.Select(graph, BookID, Year.ToString());
		}

		public static FABookPeriod GetBookPeriod(PXGraph graph, int Period, int Year, int? BookID)
		{
			return PXSelect<FABookPeriod, Where<FABookPeriod.periodNbr, Equal<Required<FABookPeriod.periodNbr>>,
				And<FABookPeriod.finYear, Equal<Required<FABookPeriod.finYear>>,
				And<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>>>>>
				.Select(graph, Period.ToString("00"), Year.ToString(), BookID);
		}

		public static FABookPeriod GetBookPeriod(PXGraph graph, string PeriodID, int? BookID)
		{
			return PXSelect<FABookPeriod, Where<FABookPeriod.finPeriodID, Equal<Required<FABookPeriod.finPeriodID>>,
				And<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>>>>
				.Select(graph, PeriodID, BookID);
		}

		public static DateTime DatePlusYears(DateTime DeprFromDate, decimal usefulLife)
		{
			DateTime deprToDate = DeprFromDate;
			if (usefulLife > 0m)
			{
				decimal fullYears = decimal.Truncate(usefulLife);
				deprToDate = DeprFromDate.AddYears((int)fullYears).AddDays(-1);

				decimal diff = usefulLife - fullYears;
				if (diff != 0m)
				{
					decimal nextYear = decimal.Ceiling(usefulLife);
					DateTime nextDate = DeprFromDate.AddYears((int)nextYear).AddDays(-1);

					int days = (int)((nextDate - deprToDate).Days * diff);
					deprToDate = deprToDate.AddDays(days - 1);
				}
			}

			return deprToDate;
		}

		#region IDepreciationCalculation Members
		public decimal depreciationBasis { get; set; }
		public decimal accumulatedDepreciation { get; set; }
		public int depreciationStartYear { get; set; }
		public FABookPeriod depreciationStopBookPeriod { get; set; }
		public FABookPeriod depreciationStartBookPeriod { get; set; }
		public int depreciationStartPeriod { get; set; }
		public int depreciationStopPeriod { get; set; }
		public int recoveryEndPeriod { get; set; }
		public int firstRecoveryEndQuarterPeriod { get; set; }
		public int firstDepreciationStopQuarterPeriod { get; set; }
		public int lastDepreciationStartQuarterPeriod { get; set; }
		public DateTime recoveryEndDate { get; set; }
		public int recoveryYears { get; set; }
		public decimal usefulLife { get; set; }
		public int depreciationYears { get; set; }
		public decimal startDepreciationMidPeriodRatio { get; set; }
		public decimal stopDepreciationMidPeriodRatio { get; set; }
		public DateTime depreciationStartDate { get; set; }
		public DateTime? depreciationStopDate { get; set; }
		public int wholeRecoveryDays { get; set; }
		public string averagingConvention { get; set; }
		public decimal wholeRecoveryPeriods { get; set; }
		public int depreciationPeriodsInYear { get; set; }
		public FADepreciationMethod depreciationMethod { get; set; }
		public int yearOfUsefulLife { get; set; }
		public DateTime recoveryStartDate { get; set; }
		public FABookPeriod recoveryStartBookPeriod { get; set; }
		public FABookPeriod recoveryEndBookPeriod { get; set; }
			#endregion
	}

	public interface IDepreciationCalculation
	{
		decimal depreciationBasis { get; set; }
		decimal accumulatedDepreciation { get; set; }
		int depreciationStartYear { get; set; }
		FABookPeriod depreciationStopBookPeriod { get; set; }
		FABookPeriod depreciationStartBookPeriod { get; set; }
		int depreciationStartPeriod { get; set; }
		int depreciationStopPeriod { get; set; }
		int recoveryEndPeriod { get; set; }
		int firstRecoveryEndQuarterPeriod { get; set; }
		int firstDepreciationStopQuarterPeriod { get; set; }
		int lastDepreciationStartQuarterPeriod { get; set; }
		DateTime recoveryEndDate { get; set; }
		int recoveryYears { get; set; }
		decimal usefulLife { get; set; }
		int depreciationYears { get; set; }
		decimal startDepreciationMidPeriodRatio { get; set; }
		decimal stopDepreciationMidPeriodRatio { get; set; }
		DateTime depreciationStartDate { get; set; }
		DateTime? depreciationStopDate { get; set; }
		int wholeRecoveryDays { get; set; }
		FADepreciationMethod depreciationMethod { get; set; }
		string averagingConvention { get; set; }
		decimal wholeRecoveryPeriods { get; set; }
		int depreciationPeriodsInYear { get; set; }
		int yearOfUsefulLife { get; set; }
		DateTime recoveryStartDate { get; set; }
		FABookPeriod recoveryStartBookPeriod { get; set; }
		FABookPeriod recoveryEndBookPeriod { get; set; }
	}
}
