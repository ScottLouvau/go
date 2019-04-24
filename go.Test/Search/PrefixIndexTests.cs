// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using go.Search;

using Xunit;

namespace go.Test.Search
{
    public class PrefixIndexTests
    {
        [Fact]
        public void PrefixIndex_Basics()
        {
            PrefixIndex index = new PrefixIndex();
            index.Add("ban", 0);
            index.Add("barn", 1);
            index.Add("bat", 2);
            index.Add("car", 3);
            index.Add("cat", 4);
            index.Add("cat", 14);

            // Multiple matches
            AssertSearch(new[] { 0, 1, 2 }, index, "ba");

            // Multiple matches including one term with multiple matches
            AssertSearch(new[] { 3, 4, 14 }, index, "ca");

            // Single term, multiple matches
            AssertSearch(new[] { 4, 14 }, index, "cat");

            // Missing term
            AssertSearch(Array.Empty<int>(), index, "bake");
        }

        private static void AssertSearch(ICollection<int> expected, PrefixIndex index, string prefix)
        {
            HashSet<int> matches = new HashSet<int>();
            index.AddMatchesStartingWith(prefix, matches);

            // Verify the correct number of matches
            Assert.Equal(expected.Count, matches.Count);

            // Verify all expected values were found and nothing was extra or missing
            matches.SymmetricExceptWith(expected);
            Assert.Empty(matches);
        }
    }
}
