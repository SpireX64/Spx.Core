using System;
using System.Collections.Generic;

namespace Spx.Collections.Linq
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Finds the index of the first item matching an expression in an enumerable.
        /// </summary>
        /// <param name="items">The enumerable to search.</param>
        /// <param name="predicate">The expression to test the items against.</param>
        /// <returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindPosition<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var index = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Finds the index of the first occurrence of an item in an enumerable.
        /// </summary>
        /// <param name="items">The enumerable to search.</param>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int PositionOf<T>(this IEnumerable<T> items, T item)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            var index = 0;
            foreach (var currentItem in items)
            {
                var isEquals = EqualityComparer<T>.Default.Equals(item, currentItem);
                if (isEquals) return index;
                index++;
            }

            return -1;
        }
    }
}
