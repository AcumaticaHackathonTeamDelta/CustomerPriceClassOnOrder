using System;
using System.Collections.Generic;
using System.Linq;

using PX.Data;

namespace PX.Objects.Common
{
	/// <summary>
	/// Compares two data records based on the equality of a subset of their fields.
	/// The set of fields to compare the records on is explicitly specified in the
	/// comparer's constructor.
	/// </summary>
	public class FieldSubsetEqualityComparer<TRecord>: IEqualityComparer<TRecord>
		where TRecord : class, IBqlTable, new()
	{
		private PXCache _cache;
		private IEnumerable<Type> _fields;

		public FieldSubsetEqualityComparer(PXCache cache, IEnumerable<Type> fields)
		{
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			if (fields == null) throw new ArgumentNullException(nameof(fields));

			_cache = cache;
			_fields = fields;
		}

		public FieldSubsetEqualityComparer(PXCache cache, params Type[] fields)
			: this(cache, fields as IEnumerable<Type>)
		{ }

		private IEnumerable<object> CollectFieldValues(TRecord record)
		{
			return _fields.Select(
				fieldType => _cache.GetValue(record, fieldType.Name));
		}

		public bool Equals(TRecord first, TRecord second)
		{
			if (first == null) throw new ArgumentNullException(nameof(first));
			if (second == null) throw new ArgumentNullException(nameof(second));

			IEnumerable<Tuple<object, object>> fieldValuePairs = 
				CollectFieldValues(first).Zip(
					CollectFieldValues(second), (x, y) => Tuple.Create(x, y));

			foreach (Tuple<object, object> fieldValuePair in fieldValuePairs)
			{
				if (!Equals(fieldValuePair.Item1, fieldValuePair.Item2))
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(TRecord record)
		{
			if (record == null) throw new ArgumentNullException(nameof(record));

			return HashcodeCombiner.Combine(CollectFieldValues(record));
		}
	}
}
