// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;

using go.Diagnostics;
using go.IO;

namespace go
{
    /// <summary>
    ///  TODO:
    ///   - Tests for DirectoryIndex, serialization
    ///   - Factor out Term matches, Intersect, Rank.
    ///   - Acronym character at each character type boundary.
    /// </summary>
    class Program
    {
        public const string Usage = @"'go' navigates to directories quickly.
  Run 'go --index [path]' to index under a given folder.
  Run 'go [term] [term] ...' to navigate.
  Terms may be the prefix of any folder name in the path, or
  the the suffix of the acronym of path (ex: bin\Release -> bR)";

        private static readonly string[] Colors = new[] { "1F", "3F", "5F", "8F", "9F" };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(Usage);
                return;
            }

            string serializationPath = Path.Combine(Path.GetTempPath(), "go", "DirectoryIndex.bin");
            DirectoryIndex index = null;

            if (args[0] == "--index")
            {
                // Indexing Mode
                string rootPath = Path.GetFullPath(args[1] ?? ".");
                using (new ConsoleWatch($"Indexing folders under {rootPath}...", () => $"Done. Indexed {index.Count:n0} folders"))
                {
                    index = DirectoryIndex.Build(rootPath);
                    BinarySerializer.SaveToFile(index, serializationPath);
                }
            }
            else
            {
                // Search Mode
                index = new DirectoryIndex();
                BinarySerializer.LoadFromFile(index, serializationPath);

                bool hadMatches = false;
                foreach (string resultPath in index.Search(args).Take(5))
                {
                    hadMatches = true;
                    Console.WriteLine($" {ColorHash(resultPath)} | {Pad(DirectoryIndex.Acronym(resultPath), 15)} | {resultPath}");
                }

                if (!hadMatches)
                {
                    Console.WriteLine($"No folders found matching {String.Join(' ', args)}.");
                }
            }
        }

        static string Pad(string value, int length)
        {
            if (value.Length >= length) { return value; }
            return value + new string(' ', length - value.Length);
        }

        static string ColorHash(string path)
        {
            uint hash = 0;
            for (int i = 0; i < path.Length; ++i)
            {
                hash = hash * 31 + path[i];
            }

            return Colors[hash % Colors.Length];
        }
    }
}
