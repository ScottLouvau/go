// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace go.Extensions
{
    public static class ListOfStringRangeStartingWith
    {
        /// <summary>
        ///  Return the range within an (ordinal) sorted string list which start with the given prefix.
        /// </summary>
        /// <param name="sortedList">List to search</param>
        /// <param name="prefix">Prefix to find</param>
        /// <returns>Range of indices which start with the prefix</returns>
        public static Range RangeStartingWith(this IList<string> sortedList, string prefix)
        {
            int firstIndex = ~BinarySearch(sortedList, prefix, PrefixComparer.FirstWithPrefix);
            int afterIndex = ~BinarySearch(sortedList, prefix, PrefixComparer.FirstAfterPrefix);

            return new Range(firstIndex, (afterIndex - firstIndex));
        }

        private class PrefixComparer : IComparer<string>
        {
            // FirstWithPrefix makes words starting with the prefix sort after prefix, so we find the first
            // FirstAfterPrefix makes words starting with the prefix sort before prefix, so we find the last
            public static IComparer<string> FirstWithPrefix = new PrefixComparer(1);
            public static IComparer<string> FirstAfterPrefix = new PrefixComparer(-1);

            private int _returnIfMatch;

            public PrefixComparer(int returnIfMatch = 0)
            {
                _returnIfMatch = returnIfMatch;
            }

            // If value after prefix, return positive.
            public int Compare(string value, string prefix)
            {
                int length = Math.Min(prefix.Length, value.Length);
                for (int i = 0; i < length; ++i)
                {
                    // If value is after prefix, return positive
                    int cmp = value[i] - prefix[i];
                    if (cmp != 0) { return cmp; }
                }

                // If value is shorter, value is before prefix
                if (value.Length < prefix.Length) { return -1; }

                return _returnIfMatch;
            }
        }

        public static int BinarySearch(this IList<string> sortedList, string prefix, IComparer<string> comparer)
        {
            int min = 0;
            int max = sortedList.Count - 1;

            while (min <= max)
            {
                int mid = min + (max - min) / 2;
                string midValue = sortedList[mid];
                int cmp = comparer.Compare(midValue, prefix);

                if (cmp == 0)
                {
                    return mid;
                }
                else if (cmp < 0)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid - 1;
                }
            }

            return ~min;
        }
    }
}
