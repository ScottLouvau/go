// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

using go.Extensions;

namespace go.Search
{
    /// <summary>
    ///  PrefixIndex maps string keys to integer values, and allows
    ///  looking up the full set of values which start with a prefix.
    /// </summary>
    public class PrefixIndex : IBinarySerializable
    {
        private List<string> SortedKeys { get; set; }
        private Dictionary<string, IList<int>> Index { get; set; }

        public PrefixIndex()
        {
            Index = new Dictionary<string, IList<int>>();
        }

        /// <summary>
        ///  Add a key and value pair.
        /// </summary>
        /// <param name="key">String key</param>
        /// <param name="value">Integer value</param>
        public void Add(string key, int value)
        {
            IList<int> page;
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
            ConvertForSearch();

            Range matches = SortedKeys.RangeStartingWith(prefix);
            for (int i = matches.Start; i < matches.End; ++i)
            {
                set.UnionWith(Index[SortedKeys[i]]);
            }
        }

        private void ConvertForSearch()
        {
            if (SortedKeys == null)
            {
                SortedKeys = new List<string>(Index.Keys);
                SortedKeys.Sort(StringComparer.Ordinal);
            }
        }

        public void Read(BinaryReader r)
        {
            SortedKeys?.Clear();
            Index.Clear();

            int keyCount = r.ReadInt32();

            SortedKeys = new List<string>(keyCount);
            for (int i = 0; i < keyCount; ++i)
            {
                SortedKeys.Add(r.ReadString());
            }

            for(int i = 0; i < keyCount; ++i)
            {
                int valueCount = r.ReadInt32();

                int[] values = new int[valueCount];
                for(int j = 0; j < valueCount; ++j)
                {
                    values[j] = r.ReadInt32();
                }

                Index[SortedKeys[i]] = values;
            }
        }

        public void Write(BinaryWriter w)
        {
            ConvertForSearch();

            w.Write(SortedKeys.Count);
            for (int i = 0; i < SortedKeys.Count; ++i)
            {
                w.Write(SortedKeys[i]);
            }

            for (int i = 0; i < SortedKeys.Count; ++i)
            {
                IList<int> values = Index[SortedKeys[i]];
                w.Write(values.Count);

                for (int j = 0; j < values.Count; ++j)
                {
                    w.Write(values[j]);
                }
            }
        }
    }
}
