using System;
using System.Collections.Generic;
using System.Linq;

using PX.Objects.Common.StringProcessing;

namespace PX.Objects.Common.Extensions
{
	public static class CollectionExtensions
	{
		public static string JoinIntoStringForMessage<T>(this ICollection<T> items, int maxCount = 0, string separator = Separators.Comma, string edgingSymbol = Edging.Quote)
		{
			IEnumerable<T> itemsToJoin = null;
			var postfix = string.Empty;

			if (items.Count > maxCount && maxCount != 0)
			{
				itemsToJoin = items.Take(maxCount);
				postfix = "...";
			}
			else
			{
				itemsToJoin = items;
			}

			var edgedItems = itemsToJoin.Select(item => string.Concat(edgingSymbol, item, edgingSymbol)); 

			return string.Concat(string.Join(separator, edgedItems), postfix);
		}

		/// <summary>
		/// Performs an action upon each element of a sequence by 
		/// incorporating the element's index.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
		{
			int index = 0;

			foreach (T item in sequence)
			{
				action(item, index);
				++index;
			}
		}

		/// <summary>
		/// Drops the specified number of elements from the tail of the sequence.
		/// Performs this efficiently, without enumerating the sequence several times.
		/// </summary>
		/// <param name="elementsToSkip">The number of elements to drop from the tail.</param>
		public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int elementsToSkip)
		{
			if (elementsToSkip < 0) throw new ArgumentOutOfRangeException(nameof(elementsToSkip));

			IEnumerator<T> enumerator = source.GetEnumerator();

			bool hasRemainingItems;
			Queue<T> elementCache = new Queue<T>(elementsToSkip + 1);

			do
			{
				hasRemainingItems = enumerator.MoveNext();

				if (hasRemainingItems)
				{
					elementCache.Enqueue(enumerator.Current);

					if (elementCache.Count > elementsToSkip)
					{
						yield return elementCache.Dequeue();
					}
				}
			}
			while (hasRemainingItems);
		}

		/// <summary>
		/// Returns a collection consisting of all elements 
		/// with even zero-based indices from a given collection.
		/// </summary>
		public static IEnumerable<T> EvenElements<T>(this IEnumerable<T> source)
		{
			int index = 0;

			foreach (T element in source)
			{
				if (index % 2 == 0)
				{
					yield return element;
				}

				++index;
			}
		}

		/// <summary>
		/// Returns a collection consisting of all elements 
		/// with odd zero-based indices from a given collection.
		/// </summary>
		public static IEnumerable<T> OddElements<T>(this IEnumerable<T> source)
		{
			int index = 0;

			foreach (T element in source)
			{
				if (index % 2 != 0)
				{
					yield return element;
				}

				++index;
			}
		}

		public static TItem GetItemWithMax<TItem, TValue>(this IReadOnlyCollection<TItem> items, Func<TItem, TValue> getValue)
			where TItem : class
			where TValue : IComparable<TValue>
		{
			return GetPreferredItem(items, getValue, (curValue, maxValue) => curValue.CompareTo(maxValue) > 0);
		}

		public static TItem GetItemWithMin<TItem, TValue>(this IReadOnlyCollection<TItem> items, Func<TItem, TValue> getValue)
			where TItem : class
			where TValue : IComparable<TValue>
		{
			return GetPreferredItem(items, getValue, (curValue, minValue) => curValue.CompareTo(minValue) < 0);
		}

		public static TItem GetPreferredItem<TItem, TValue>(this IReadOnlyCollection<TItem> items, Func<TItem, TValue> getValue, Func<TValue, TValue, bool> compareFunc)
			where TItem : class
			where TValue : IComparable<TValue>
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			if (items == null)
				throw new ArgumentNullException(nameof(getValue));

			if (!items.Any())
				return null;

			var preferredItem = items.First();
			var preferredValue = getValue(preferredItem);

			foreach (var item in items)
			{
				var curValue = getValue(item);

				if (compareFunc(curValue, preferredValue))
				{
					preferredValue = curValue;
					preferredItem = item;
				}
			}

			return preferredItem;
		}

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
			{
				collection.Add(item);
			}
		}
	}
}
