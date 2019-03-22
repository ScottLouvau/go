// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace go
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = (args.Length > 0 ? args[0] : @"C:\");

            DirectoryIndex index = DirectoryIndex.Build(rootPath);

            // 161k folders, 48k names, 13s (warm), 30s (cold).

            // Serialization format?
            //  Can I start with indented strings only?

        }
    }
}
