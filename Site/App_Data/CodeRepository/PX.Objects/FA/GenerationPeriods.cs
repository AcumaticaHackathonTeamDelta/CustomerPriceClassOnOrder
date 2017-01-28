using System.Collections;
using System.Collections.Generic;
using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	public class GenerationPeriods : PXGraph<GenerationPeriods>
	{
		public PXCancel<BoundaryYears> Cancel;
		public PXFilter<BoundaryYears> Years;
		[PXFilterable]
		public PXFilteredProcessing<FABook, BoundaryYears> Books;

		public PXSelect<FABookYear> bookyears;
		public PXSelect<FABookPeriod> bookperiods;


		protected virtual void BoundaryYears_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row != null)
			{
				BoundaryYears filter = (BoundaryYears)e.Row;
				Books.SetProcessDelegate(list => GeneratePeriods(filter, list));
			}
		}

		protected virtual void BoundaryYears_YearTo_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			int? minyear = null;
			foreach (FABook book in Books.Select())
			{
				if(book.LastCalendarYear != null && (minyear == null || minyear > Convert.ToInt32(book.LastCalendarYear)))
				{
					minyear = Convert.ToInt32(book.LastCalendarYear);
				}
			}
			e.NewValue = Convert.ToString(minyear + 1);
		}

		public static void GeneratePeriods(BoundaryYears filter, List<FABook> books)
		{
            GenerationPeriods graph = PXGraph.CreateInstance<GenerationPeriods>();
			graph.GeneratePeriodsProc(filter, books);
		}

        protected virtual void FABook_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            FABook book = (FABook) e.Row;
            if (book == null) return;

            FABookYearSetup setup = PXSelect<FABookYearSetup, Where<FABookYearSetup.bookID, Equal<Current<FABook.bookID>>>>.SelectSingleBound(this, new object[] { book });
            if(setup == null && book.UpdateGL != true)
            {
                PXUIFieldAttribute.SetEnabled<FABook.selected>(sender, book, false);
                sender.RaiseExceptionHandling<FABook.selected>(book, null, new PXSetPropertyException(Messages.CalendarSetupNotFound, PXErrorLevel.RowWarning, book.BookCode));
            }
        }

		protected virtual void FABookPeriod_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (e.Row != null)
			{
				((FABookPeriod)e.Row).FinPeriodID = ((FABookPeriod)e.Row).FinYear + ((FABookPeriod)e.Row).FinPeriodID;
			}
		}

		public virtual void GeneratePeriodsProc(BoundaryYears filter, List<FABook> books)
		{
			for (int i = 0; i < books.Count; ++i )
			{
				FABook book = books[i];

				FieldDefaulting.AddHandler<FABookYear.bookID>(delegate(PXCache sender, PXFieldDefaultingEventArgs e)
				{
					if (!e.Cancel)
					{
						e.NewValue = book.BookID;
					}
					e.Cancel = true;
				});

				FieldDefaulting.AddHandler<FABookPeriod.bookID>(delegate(PXCache sender, PXFieldDefaultingEventArgs e)
				{
					if (!e.Cancel)
					{
						e.NewValue = book.BookID;
					}
					e.Cancel = true;
				});

				bool yearcreated = false;

				FABookYear bookyear = PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABook.bookID>>>, OrderBy<Desc<FABookYear.year>>>.SelectSingleBound(this, new object[] { book });
				FABookYear newYear = new FABookYear {Year = bookyear != null ? bookyear.Year : null};

				if (book.UpdateGL == false)
				{
					FABookYearSetup calendar = PXSelect<FABookYearSetup, Where<FABookYearSetup.bookID, Equal<Current<FABook.bookID>>>>.SelectSingleBound(this, new object[] { book });
					IEnumerable periods = PXSelect<FABookPeriodSetup, Where<FABookPeriodSetup.bookID, Equal<Current<FABook.bookID>>>>.SelectMultiBound(this, new object[] { book });

					while (newYear != null && string.Compare(newYear.Year, filter.YearTo) < 0)
					{
						newYear = FiscalYearCreator<FABookYear, FABookPeriod, FABookPeriodSetup>.CreateNextYear(this, calendar, periods.RowCast<FABookPeriodSetup>(), bookyear);
						bookyear = newYear;

						yearcreated |= newYear != null;
					}

					bookyear = PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABook.bookID>>>, OrderBy<Asc<FABookYear.year>>>.SelectSingleBound(this, new object[] { book });
					newYear = new FABookYear { Year = bookyear != null ? bookyear.Year : null };
					while (newYear != null && string.Compare(newYear.Year, calendar.FirstFinYear) > 0)
					{
						newYear = FiscalYearCreator<FABookYear, FABookPeriod, FABookPeriodSetup>.CreatePrevYear(this, calendar, periods.RowCast<FABookPeriodSetup>(), bookyear);
						bookyear = newYear;

						yearcreated |= newYear != null;
					}
				}
				else
				{
					FinYearSetup calendar = PXSelect<FinYearSetup>.Select(this);
					IEnumerable periods = PXSelect<FinPeriodSetup>.Select(this);
					while (newYear != null && string.Compare(newYear.Year, filter.YearTo) < 0)
					{
						newYear = FiscalYearCreator<FABookYear, FABookPeriod, FinPeriodSetup>.CreateNextYear(this, calendar, periods.RowCast<FinPeriodSetup>(), bookyear);
						bookyear = newYear;

						yearcreated |= newYear != null;
					}

					bookyear = PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABook.bookID>>>, OrderBy<Asc<FABookYear.year>>>.SelectSingleBound(this, new object[] { book });
					newYear = new FABookYear { Year = bookyear != null ? bookyear.Year : null };
					while (newYear != null && string.Compare(newYear.Year, calendar.FirstFinYear) > 0)
					{
						newYear = FiscalYearCreator<FABookYear, FABookPeriod, FinPeriodSetup>.CreatePrevYear(this, calendar, periods.RowCast<FinPeriodSetup>(), bookyear);
						bookyear = newYear;

						yearcreated |= newYear != null;
					}
				}

				if (!IsImport)
				{
					if (yearcreated)
					{
						PXProcessing<FABook>.SetInfo(i, ActionsMessages.RecordProcessed);
					}
					else
					{
						PXProcessing<FABook>.SetWarning(i, Messages.CalendarAlreadyExists);
					}
				}
			}

			Actions.PressSave();
		}
	}

	[Serializable]
	public partial class BoundaryYears : IBqlTable
	{
		#region YearTo
		public abstract class yearTo : IBqlField
		{
		}
		protected String _YearTo;
		[PXString(4)]
		[PXUIField(DisplayName = "Generate Through Year")]
		public virtual String YearTo
		{
			get
			{
				return _YearTo;
			}
			set
			{
				_YearTo = value;
			}
		}
		#endregion
	}
}
