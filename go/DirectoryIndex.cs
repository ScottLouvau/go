// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using go.Extensions;
using go.Search;

namespace go
{
    internal class Directory
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public int ParentIndex { get; set; }
        public int DescendantCount { get; set; }
    }

    internal class DirectoryIndex
    {
        private List<Directory> Directories { get; set; }
        private PrefixIndex NameIndex { get; set; }
        private PrefixIndex ReversedAcronymIndex { get; set; }

        public DirectoryIndex()
        {
            this.Directories = new List<Directory>();
            this.NameIndex = new PrefixIndex();
            this.ReversedAcronymIndex = new PrefixIndex();
        }

        public static DirectoryIndex Build(string rootPath)
        {
            DirectoryIndex result = new DirectoryIndex();
            result.Build(new DirectoryInfo(rootPath), -1);
            return result;
        }

        private void Build(DirectoryInfo under, int parentIndex)
        {
            Directory here = new Directory()
            {
                Name = under.Name,
                Index = Directories.Count,
                ParentIndex = parentIndex
            };

            Directories.Add(here);

            try
            {
                foreach (DirectoryInfo child in under.GetDirectories())
                {
                    Build(child, here.Index);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // ... Directory we didn't have access to - skip
            }

            here.DescendantCount = (this.Directories.Count - here.Index);
            NameIndex.Add(here.Name, here.Index);
            ReversedAcronymIndex.Add(ReversedAcronym(here.Index), here.Index);
        }

        public IList<string> Search(IList<string> terms)
        {
            // Issue: Want prefix search in indices.
            //  Sorted array or tree, then single array of matches and first match.

            // Issue: How do I filter matches while handling hierarchy?
            //  - Ex: "bion bR" -> each term should logically match all paths under ones which contain the term.
            //  - So bion match should add whole Range for bion, and bR should "narrow"
            //   - For each term, get all Directories for term and sort.
            //   - Narrow each existing match to ranges within which match the new term.

            HashSet<int> matches = new HashSet<int>();
            HashSet<int> termMatches = new HashSet<int>();

            foreach (string term in terms)
            {
                termMatches.Clear();

                NameIndex.AddMatchesStartingWith(term, termMatches);
                ReversedAcronymIndex.AddMatchesStartingWith(term.Reverse(), termMatches);

                // TODO: Merge matches correctly.
            }

            // TODO: Ranking

            return matches.Select(index => FullPath(index)).ToList();
        }

        private string ReversedAcronym(int index)
        {
            StringBuilder result = new StringBuilder();

            while (index != -1)
            {
                Directory current = Directories[index];
                result.Append(current.Name[0]);
                index = current.ParentIndex;
            }

            return result.ToString();
        }

        private string FullPath(int index)
        {
            StringBuilder builder = new StringBuilder();
            FullPathRecursive(index, builder);
            return builder.ToString();
        }

        private void FullPathRecursive(int index, StringBuilder builder)
        {
            Directory here = Directories[index];
            int parentIndex = here.ParentIndex;
            if (parentIndex != -1)
            {
                FullPathRecursive(parentIndex, builder);
                builder.Append("/");
            }

            builder.Append(here.Name);
        }
    }
}
