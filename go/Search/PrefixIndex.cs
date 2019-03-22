// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

using go.Extensions;

namespace go.Search
{
    /// <summary>
    ///  PrefixIndex maps string keys to integer values, and allows
    ///  looking up the full set of values which start with a prefix.
    /// </summary>
    public class PrefixIndex
    {
        private List<string> SortedKeys { get; set; }
        private Dictionary<string, List<int>> Index { get; set; }

        public PrefixIndex()
        {
            Index = new Dictionary<string, List<int>>();
        }

        /// <summary>
        ///  Add a key and value pair.
        /// </summary>
        /// <param name="key">String key</param>
        /// <param name="value">Integer value</param>
        public void Add(string key, int value)
        {
            List<int> page;
            if (!Index.TryGetValue(key, out page))
            {
                SortedKeys = null;
                page = new List<int>();
                Index[key] = page;
            }

            page.Add(value);
        }

        /// <summary>
        ///  Find all keys starting with prefix and add all corresponding
        ///  values to the given set.
        /// </summary>
        /// <param name="prefix">Prefix to search for</param>
        /// <param name="set">HashSet to add all values for keys starting with prefix to</param>
        public void AddMatchesStartingWith(string prefix, HashSet<int> set)
        {
            if (SortedKeys == null)
            {
                SortedKeys = new List<string>(Index.Keys);
                SortedKeys.Sort();
            }

            Range matches = SortedKeys.RangeStartingWith(prefix);
            for (int i = matches.Start; i < matches.End; ++i)
            {
                set.UnionWith(Index[SortedKeys[i]]);
            }
        }
    }
}
