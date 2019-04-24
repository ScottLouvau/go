// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace go.Extensions
{
    public static class StringReverse
    {
        public static string Reverse(this string value)
        {
            if (String.IsNullOrEmpty(value)) { return value; }

            int length = value.Length;
            char[] result = new char[length];
            for (int i = 0; i < length; ++i)
            {
                result[i] = value[length - 1 - i];
            }

            return new string(result);
        }
    }
}
