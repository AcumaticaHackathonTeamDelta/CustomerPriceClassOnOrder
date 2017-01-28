using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.Common.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool IsSingleElement<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null) throw new ArgumentNullException(nameof(sequence));

			IEnumerator<T> enumerator = sequence.GetEnumerator();
			return enumerator.MoveNext() && !enumerator.MoveNext();
		}

		public static bool HasAtLeastTwoItems<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null) throw new ArgumentNullException(nameof(sequence));

			IEnumerator<T> enumerator = sequence.GetEnumerator();
			return
				enumerator.MoveNext() &&
				enumerator.MoveNext();
		}

		/// <summary>
		/// Flattens a sequence of element groups into a sequence of elements.
		/// </summary>
		public static IEnumerable<TValue> Flatten<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> sequenceOfGroups)
			=> sequenceOfGroups.SelectMany(x => x);
	}
}
