// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace go
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = (args.Length > 0 ? args[0] : @"C:\Code");

            DirectoryIndex index = DirectoryIndex.Build(rootPath);

            foreach (string resultPath in index.Search(new[] { "spam", "bR" }))
            {
                Console.WriteLine(resultPath);

                //Environment.CurrentDirectory = resultPath;
                //break;
            }

            // 161k folders, 48k names, 13s (warm), 30s (cold).

            // Serialization format?
            //  Can I start with indented strings only?

        }
    }
}
