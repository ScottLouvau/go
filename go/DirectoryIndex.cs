// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using go.Extensions;
using go.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace go
{
    internal class Directory
    {
        public string Name { get; set; }
        public int ParentIndex { get; set; }
    }

    internal class DirectoryIndex : IBinarySerializable
    {
        private List<Directory> Directories { get; set; }
        private PrefixIndex NameIndex { get; set; }
        private PrefixIndex ReversedAcronymIndex { get; set; }

        public int Count => Directories.Count;

        public DirectoryIndex()
        {
            this.Directories = new List<Directory>();
            this.NameIndex = new PrefixIndex();
            this.ReversedAcronymIndex = new PrefixIndex();
        }

        public static DirectoryIndex Build(string rootPath)
        {
            DirectoryInfo root = new DirectoryInfo(rootPath);

            DirectoryIndex result = new DirectoryIndex();
            result.BuildUp(root.Parent);
            result.BuildDown(root, result.Directories.Count - 1);
            return result;
        }

        private void BuildUp(DirectoryInfo current)
        {
            if(current == null) { return; }
            BuildUp(current.Parent);

            Directory here = new Directory()
            {
                Name = current.Name.TrimEnd('\\'),
                ParentIndex = Directories.Count - 1
            };

            Directories.Add(here);
        }

        private void BuildDown(DirectoryInfo current, int parentIndex)
        {
            int newIndex = Directories.Count;

            Directory here = new Directory()
            {
                Name = current.Name,
                ParentIndex = parentIndex
            };

            Directories.Add(here);

            try
            {
                foreach (DirectoryInfo child in current.GetDirectories())
                {
                    BuildDown(child, newIndex);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // ... Directory we didn't have access to - skip
            }
            catch (DirectoryNotFoundException)
            {
                // ... Redirect - skip
            }

            NameIndex.Add(here.Name, newIndex);
            ReversedAcronymIndex.Add(ReversedAcronym(newIndex), newIndex);
        }

        public IEnumerable<string> Search(IList<string> terms, string preferUnderPath = null)
        {
            HashSet<int> currentMatches = null;
            HashSet<int> termMatches = new HashSet<int>();
            HashSet<int> working = new HashSet<int>();

            foreach (string term in terms)
            {
                termMatches.Clear();
                NameIndex.AddMatchesStartingWith(term, termMatches);
                ReversedAcronymIndex.AddMatchesStartingWith(term.Reverse(), termMatches);

                if (currentMatches == null)
                {
                    // First term: Keep all matches
                    currentMatches = new HashSet<int>(termMatches);
                }
                else
                {
                    // Otherwise intersect
                    IntersectHierarchy(currentMatches, termMatches, working);

                    // If there are no more valid matches, stop
                    if (currentMatches.Count == 0)
                    {
                        break;
                    }
                }
            }

            // Ranking: Restrict to under current path, if any matches
            if (preferUnderPath != null)
            {
                HashSet<int> beforeFiltering = new HashSet<int>(currentMatches);

                int pathIndex = IndexOfPath(preferUnderPath);
                if (pathIndex != -1)
                {
                    termMatches.Clear();
                    termMatches.Add(pathIndex);
                    IntersectHierarchy(currentMatches, termMatches, working);
                    if (currentMatches.Count == 0) { currentMatches = beforeFiltering; }
                }
            }

            // Ranking: Return shallowest directory
            return currentMatches.OrderBy(index => Depth(index)).Select(index => FullPath(index));
        }

        private void IntersectHierarchy(HashSet<int> matches, HashSet<int> termMatches, HashSet<int> working)
        {
            // Hierarchy matches are kept if they or an ancestor is in the other set.
            //  Suppose we're intersecting "C:\Code" and "C:\Code\go".
            //  "C:\Code" is not kept because no ancestor is in the set.
            //  "C:\Code\go" is kept because the ancestor "C:\Code" is in the set.

            // Collect the matches from each side
            working.Clear();
            working.UnionWith(matches.Where(index => ItemOrAncestorInSet(index, termMatches)));
            working.UnionWith(termMatches.Where(index => ItemOrAncestorInSet(index, matches)));

            // Reset matches so far to this set
            matches.Clear();
            matches.UnionWith(working);
        }

        private bool ItemOrAncestorInSet(int index, HashSet<int> set)
        {
            int current = index;
            while (current != -1)
            {
                if (set.Contains(current)) { return true; }
                current = Directories[current].ParentIndex;
            }

            return false;
        }

        private int IndexOfPath(string path)
        {
            string fullPath = Path.GetFullPath(path);
            string revAcronym = ReversedAcronym(fullPath);

            HashSet<int> candidates = new HashSet<int>();
            ReversedAcronymIndex.AddMatchesStartingWith(revAcronym, candidates);

            foreach (int index in candidates)
            {
                string candidateFullPath = FullPath(index);
                if (fullPath.Equals(candidateFullPath)) { return index; }
            }

            return -1;
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

        public static string ReversedAcronym(string fullPath)
        {
            string[] parts = fullPath.Split('\\');

            StringBuilder result = new StringBuilder(parts.Length);
            for (int i = parts.Length - 1; i >= 0; --i)
            {
                result.Append(parts[i][0]);
            }

            return result.ToString();
        }

        public static string Acronym(string path)
        {
            return ReversedAcronym(Path.GetFullPath(path)).Reverse();
        }

        private int Depth(int index)
        {
            int depth = -1;

            int current = index;
            while (current != -1)
            {
                current = Directories[current].ParentIndex;
                depth++;
            }

            return depth;
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
                builder.Append("\\");
            }

            builder.Append(here.Name);
        }

        public void Read(BinaryReader r)
        {
            Directories.Clear();

            int directoryCount = r.ReadInt32();
            for (int i = 0; i < directoryCount; ++i)
            {
                Directories.Add(new Directory() { Name = r.ReadString(), ParentIndex = r.ReadInt32() });
            }

            NameIndex.Read(r);
            ReversedAcronymIndex.Read(r);
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Directories.Count);
            for(int i = 0; i < Directories.Count; ++i)
            {
                w.Write(Directories[i].Name);
                w.Write(Directories[i].ParentIndex);
            }

            NameIndex.Write(w);
            ReversedAcronymIndex.Write(w);
        }
    }
}
