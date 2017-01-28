namespace PX.Objects.CA
{
	using System;
	using System.Collections.Generic;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.GL;
	using PX.Objects.CS;

	
	[System.SerializableAttribute()]
	[CADailySummaryAccumulator()]
	[PXPrimaryGraph(typeof(CATranEnq), Filter = typeof(CAEnqFilter))]
	[PXCacheName(Messages.CADailySummary)]
	public partial class CADailySummary : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault()]
		[GL.CashAccount(DisplayName = "Cash Account", IsKey = true)]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate(IsKey = true)]
		[PXDefault()]
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
		#region AmtReleasedClearedDr
		public abstract class amtReleasedClearedDr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtReleasedClearedDr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtReleasedClearedDr
		{
			get
			{
				return this._AmtReleasedClearedDr;
			}
			set
			{
				this._AmtReleasedClearedDr = value;
			}
		}
		#endregion
		#region AmtUnreleasedClearedDr
		public abstract class amtUnreleasedClearedDr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtUnreleasedClearedDr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtUnreleasedClearedDr
		{
			get
			{
				return this._AmtUnreleasedClearedDr;
			}
			set
			{
				this._AmtUnreleasedClearedDr = value;
			}
		}
		#endregion
		#region AmtReleasedUnclearedDr
		public abstract class amtReleasedUnclearedDr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtReleasedUnclearedDr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtReleasedUnclearedDr
		{
			get
			{
				return this._AmtReleasedUnclearedDr;
			}
			set
			{
				this._AmtReleasedUnclearedDr = value;
			}
		}
		#endregion
		#region AmtUnreleasedUnclearedDr
		public abstract class amtUnreleasedUnclearedDr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtUnreleasedUnclearedDr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtUnreleasedUnclearedDr
		{
			get
			{
				return this._AmtUnreleasedUnclearedDr;
			}
			set
			{
				this._AmtUnreleasedUnclearedDr = value;
			}
		}
		#endregion
		#region AmtReleasedClearedCr
		public abstract class amtReleasedClearedCr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtReleasedClearedCr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtReleasedClearedCr
		{
			get
			{
				return this._AmtReleasedClearedCr;
			}
			set
			{
				this._AmtReleasedClearedCr = value;
			}
		}
		#endregion
		#region AmtUnreleasedClearedCr
		public abstract class amtUnreleasedClearedCr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtUnreleasedClearedCr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtUnreleasedClearedCr
		{
			get
			{
				return this._AmtUnreleasedClearedCr;
			}
			set
			{
				this._AmtUnreleasedClearedCr = value;
			}
		}
		#endregion
		#region AmtReleasedUnclearedCr
		public abstract class amtReleasedUnclearedCr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtReleasedUnclearedCr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtReleasedUnclearedCr
		{
			get
			{
				return this._AmtReleasedUnclearedCr;
			}
			set
			{
				this._AmtReleasedUnclearedCr = value;
			}
		}
		#endregion
		#region AmtUnreleasedUnclearedCr
		public abstract class amtUnreleasedUnclearedCr : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtUnreleasedUnclearedCr;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AmtUnreleasedUnclearedCr
		{
			get
			{
				return this._AmtUnreleasedUnclearedCr;
			}
			set
			{
				this._AmtUnreleasedUnclearedCr = value;
			}
		}
		#endregion
	}

	public class CADailySummaryAccumulatorAttribute: PXAccumulatorAttribute
	{
		protected PXAccumulatorCollection _columns;
		protected Dictionary<object, List<object>> _chunks;
		protected HashSet<object> _persisted;

		public CADailySummaryAccumulatorAttribute()
			:base()
		{
			this.SingleRecord = true;
		}

		public override bool PersistInserted(PXCache sender, object row)
		{
			if (base.PersistInserted(sender, row))
			{
				_persisted.Add(row);
				return true;
			}
			return false;
		}

		public override object Insert(PXCache sender, object row)
		{
			object existing = sender.Locate(row);
			if (existing != null)
			{
				bool contains = false;
				if (!(contains = _persisted.Contains(existing)) && sender.GetStatus(existing) == PXEntryStatus.Inserted)
				{
					sender.Current = existing;
					return existing;
				}
				sender.Remove(existing);

				if (contains)
				{
					object newrow = base.Insert(sender, row);
					sender.ResetPersisted(existing);

					List<object> chunk;
					if (!_chunks.TryGetValue(existing, out chunk))
					{
						chunk = new List<object>();
					}

					_chunks[newrow] = chunk;
					chunk.Add(existing);
					_chunks.Remove(existing);
					_persisted.Remove(existing);

					return newrow;
				}
				else
				{
					return base.Insert(sender, row);
				}
			}
			return base.Insert(sender, row);
		}

		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			_columns = columns;
			return base.PrepareInsert(sender, row, columns);
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.RowPersisted.AddHandler(sender.GetItemType(), RowPersisted);
			_chunks = new Dictionary<object, List<object>>();
			_persisted = new HashSet<object>();
		}

		protected virtual object Aggregate(PXCache cache, object a, object b)
		{
			object ret = cache.CreateCopy(a);

			foreach (KeyValuePair<string, PXAccumulatorItem> column in _columns)
			{
				if (column.Value.CurrentUpdateBehavior == PXDataFieldAssign.AssignBehavior.Summarize)
				{
					object aVal = cache.GetValue(a, column.Key);
					object bVal = cache.GetValue(b, column.Key);
					object retVal = null;

					if (aVal.GetType() == typeof(decimal))
					{
						retVal = (decimal)aVal + (decimal)bVal;
					}

					if (aVal.GetType() == typeof(double))
					{
						retVal = (double)aVal + (double)bVal;
					}

					if (aVal.GetType() == typeof(long))
					{
						retVal = (long)aVal + (long)bVal;
					}

					if (aVal.GetType() == typeof(int))
					{
						retVal = (int)aVal + (int)bVal;
					}

					if (aVal.GetType() == typeof(short))
					{
						retVal = (short)aVal + (short)bVal;
					}

					cache.SetValue(ret, column.Key, retVal);
				}
			}

			return ret;
		}


		protected virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.TranStatus == PXTranStatus.Aborted)
			{
				foreach (KeyValuePair<object, List<object>> kv in _chunks)
				{ 
					foreach (object chunk in kv.Value)
					{
						object sum = Aggregate(sender, kv.Key, chunk);
						sender.RestoreCopy(kv.Key, sum);
					}
				}
			}

			if (e.TranStatus != PXTranStatus.Open)
			{
				_chunks = new Dictionary<object, List<object>>();
				_persisted = new HashSet<object>();
			}
		}
	}

	public class CADailyAccumulatorAttribute : PXEventSubscriberAttribute, IPXRowInsertedSubscriber, IPXRowDeletedSubscriber, IPXRowUpdatedSubscriber
	{
		private void update(CATran tran, CADailySummary summary, int sign)
		{
			if (tran.Cleared == true)
			{
				if (tran.Released == true)
				{
					summary.AmtReleasedClearedCr += (tran.CuryCreditAmt * sign);
					summary.AmtReleasedClearedDr += (tran.CuryDebitAmt * sign);
				}
				else
				{
					summary.AmtUnreleasedClearedCr += (tran.CuryCreditAmt * sign);
					summary.AmtUnreleasedClearedDr += (tran.CuryDebitAmt * sign);
				}
			}
			else
			{
				if (tran.Released == true)
				{
					summary.AmtReleasedUnclearedCr += (tran.CuryCreditAmt * sign);
					summary.AmtReleasedUnclearedDr += (tran.CuryDebitAmt * sign);
				}
				else
				{
					summary.AmtUnreleasedUnclearedCr += (tran.CuryCreditAmt * sign);
					summary.AmtUnreleasedUnclearedDr += (tran.CuryDebitAmt * sign);
				}
			}
		}

		public static void RowInserted<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is CADailyAccumulatorAttribute)
				{
					((CADailyAccumulatorAttribute)attr).RowInserted(sender, new PXRowInsertedEventArgs(data, false));
				}
			}
		}

		public virtual void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			CATran tran = (CATran)e.Row;
			if (tran.CashAccountID != null && tran.TranDate != null)
			{
				CADailySummary summary = new CADailySummary();
				summary.CashAccountID = tran.CashAccountID;
				summary.TranDate = tran.TranDate;
				summary = (CADailySummary)sender.Graph.Caches[typeof(CADailySummary)].Insert(summary);
				update(tran, summary, 1);
			}
		}
		public virtual void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			CATran tran = (CATran)e.Row;
			if (tran.CashAccountID != null && tran.TranDate != null)
			{
				CADailySummary summary = new CADailySummary();
				summary.CashAccountID = tran.CashAccountID;
				summary.TranDate = tran.TranDate;
				summary = (CADailySummary)sender.Graph.Caches[typeof(CADailySummary)].Insert(summary);
				update(tran, summary, -1);
			}
		}
		public virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			CATran tran = (CATran)e.Row;
			if (tran.CashAccountID != null && tran.TranDate != null)
			{
				CADailySummary summary = new CADailySummary();
				summary.CashAccountID = tran.CashAccountID;
				summary.TranDate = tran.TranDate;
				summary = (CADailySummary)sender.Graph.Caches[typeof(CADailySummary)].Insert(summary);
				update(tran, summary, 1);
			}
			tran = (CATran)e.OldRow;
			if (tran.CashAccountID != null && tran.TranDate != null)
			{
				CADailySummary summary = new CADailySummary();
				summary.CashAccountID = tran.CashAccountID;
				summary.TranDate = tran.TranDate;
				summary = (CADailySummary)sender.Graph.Caches[typeof(CADailySummary)].Insert(summary);
				update(tran, summary, -1);
			}
		}
		private void CATranRowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			sender.Graph.Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);
		}
		private void CATranRowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.TranStatus != PXTranStatus.Open)
			{
				sender.Graph.Caches[typeof(CADailySummary)].Persisted(e.TranStatus == PXTranStatus.Aborted);
			}
		}
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			
			sender.Graph.RowPersisting.AddHandler<CATran>(CATranRowPersisting);
			sender.Graph.RowPersisted.AddHandler<CATran>(CATranRowPersisted);
		}
	}
}
