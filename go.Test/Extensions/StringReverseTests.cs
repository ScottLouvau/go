// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using go.Extensions;

using Xunit;

namespace go.Test.Extensions
{
    public class StringReverseTests
    {
        [Fact]
        public void StringReverse_Basics()
        {
            Assert.Null(((string)null).Reverse());
            Assert.Equal("", "".Reverse());
            Assert.Equal("cba", "abc".Reverse());
        }
    }
}
