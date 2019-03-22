// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace go
{
    /// <summary>
    ///  Range represents a set of values in an array.
    ///  
    ///  Usage:
    ///  for(int i = range.Start; i &lt; range.End; ++i)
    ///  {
    ///     ...
    ///  }
    /// </summary>
    public struct Range : IEquatable<Range>
    {
        public static readonly Range Empty = new Range(0, 0);

        public int Start { get; set; }
        public int Count { get; set; }
        public int End => (Start + Count);

        public Range(int start, int count)
        {
            // Canonicalize empty ranges to (0, 0).
            Start = (count <= 0 ? 0 : start);
            Count = (count <= 0 ? 0 : count);
        }

        public bool Equals(Range other)
        {
            return this.Start == other.Start && this.Count == other.Count;
        }

        public override string ToString()
        {
            return $"@{Start:n0} for {Count:n0}";
        }
    }
}
