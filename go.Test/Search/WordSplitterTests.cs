// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;

using go.Components;
using go.Search;

using Xunit;

namespace go.Test.Search
{
    public class WordSplitterTests
    {
        [Fact]
        public void WordSplitter_Basics()
        {
            PartialArray<Word> words = null;

            AssertSplit("", null, ref words);
            AssertSplit("", "", ref words);

            AssertSplit("go", "go", ref words);
            AssertSplit("go|2", "go2", ref words);
            AssertSplit("PascalCase", "PascalCase", ref words);
            AssertSplit("go|.|Release|2", "go.Release2", ref words);
        }

        private void AssertSplit(string pipeDelimitedExpectedWords, string valueToSplit, ref PartialArray<Word> buffer)
        {
            buffer = WordSplitter.Split(valueToSplit, buffer);
            string[] actual = buffer.Select(word => word.ToString(valueToSplit)).ToArray();
            string[] expected = pipeDelimitedExpectedWords.Split('|', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(expected, actual);
        }
    }
}
