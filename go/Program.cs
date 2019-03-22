// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using go.Diagnostics;
using go.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace go
{
    /// <summary>
    ///  TODO:
    ///   - Tests for DirectoryIndex, serialization
    ///   - New acronym model with fewer dupes?
    /// </summary>
    class Program
    {
        public const string Usage = @"'go' navigates to directories quickly.
  Run 'go --index [path]' to index under a given folder.
  Run 'go [term] [term] ...' to navigate.
  Terms may be the prefix of any folder name in the path, or
  the the suffix of the acronym of path (ex: bin\Release -> bR)";

        static void Main(string[] args)
        {
            if(args.Length == 0)
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

                string firstPath = null;
                foreach (string resultPath in index.Search(args).Take(5))
                {
                    Console.WriteLine($"{DirectoryIndex.Acronym(resultPath)}: {resultPath}");
                    if (firstPath == null) { firstPath = resultPath; }
                }

                // Navigate to the first match
                if (firstPath != null)
                {
                    Console.Title = $"{DirectoryIndex.Acronym(firstPath)}: {firstPath}";
                    ChangeDirectory(firstPath);
                }
                else
                {
                    Console.WriteLine($"No folders found matching {String.Join(' ', args)}.");
                }
            }
        }

        private static void ChangeDirectory(string targetPath)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/K CD /D \"{targetPath}\" && TITLE {DirectoryIndex.Acronym(targetPath)}: {targetPath}";
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }
    }
}
