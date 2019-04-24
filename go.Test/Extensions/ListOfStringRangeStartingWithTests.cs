// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using go.Extensions;

using Xunit;

namespace go.Test.Extensions
{
    public class ListOfStringRangeStartingWithTests
    {
        //                   0      1       2       3     4      5      6      7       8
        String[] samples = { "ban", "barn", "cab", "can", "car", "cat", "dab", "data", "dog" };

        [Fact]
        public void BinarySearch_Basics()
        {
            IComparer<string> comparer = StringComparer.Ordinal;

            // Before first: Insert at zero
            Assert.Equal(~0, ListOfStringRangeStartingWith.BinarySearch(samples, "ba", comparer));

            // Equals first
            Assert.Equal(0, ListOfStringRangeStartingWith.BinarySearch(samples, "ban", comparer));

            // Equals second
            Assert.Equal(1, ListOfStringRangeStartingWith.BinarySearch(samples, "barn", comparer));

            // Middle
            Assert.Equal(2, ListOfStringRangeStartingWith.BinarySearch(samples, "cab", comparer));

            // Between middle values: Insert between
            Assert.Equal(~2, ListOfStringRangeStartingWith.BinarySearch(samples, "ca", comparer));

            // Last
            Assert.Equal(8, ListOfStringRangeStartingWith.BinarySearch(samples, "dog", comparer));

            // After Last
            Assert.Equal(~9, ListOfStringRangeStartingWith.BinarySearch(samples, "dogs", comparer));
        }

        [Fact]
        public void RangeStartingWith_Basics()
        {
            // Range in middle
            Assert.Equal(new Range(2, 4), samples.RangeStartingWith("ca"));

            // Range at start
            Assert.Equal(new Range(0, 2), samples.RangeStartingWith("ba"));

            // Range at end
            Assert.Equal(new Range(6, 3), samples.RangeStartingWith("d"));

            // Single value
            Assert.Equal(new Range(4, 1), samples.RangeStartingWith("car"));

            // Missing value
            Assert.Equal(new Range(0, 0), samples.RangeStartingWith("bat"));

            // Before start
            Assert.Equal(new Range(0, 0), samples.RangeStartingWith("ack"));

            // After end
            Assert.Equal(new Range(0, 0), samples.RangeStartingWith("zack"));
        }
    }
}
